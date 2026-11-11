using System.Diagnostics;
using System.Numerics;
using DXSharp.Assets;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal static class Program {
    [STAThread]
    public static int Main( string[] args ) {
        try {
            return Run( args );
        } catch ( Exception error ) {
            Console.Error.WriteLine( error );
            return 1;
        }
    }

    private static int Run( string[] args ) {
        int ticks = int.Parse( Option( args, "--benchmark-ticks=", "0" ) );
        ArgumentOutOfRangeException.ThrowIfNegative( ticks );
        var fleet = new FleetSimulation( int.Parse( Option( args, "--ships=", FleetSimulation.DefaultShips.ToString( ) ) ),
            uint.Parse( Option( args, "--seed=", "37" ) ), rivalPatrol: ticks == 0 );
        if ( ticks > 0 ) {
            Benchmark( fleet, ticks );
            return 0;
        }

        int frameLimit = int.Parse( Option( args, "--frames=", "0" ) );
        ArgumentOutOfRangeException.ThrowIfNegative( frameLimit );
        bool resizeTest = args.Contains( "--resize-test" );
        if ( resizeTest && frameLimit is > 0 and < 6 )
            throw new ArgumentException( "Resize qualification requires at least six frames." );
        if ( resizeTest && frameLimit == 0 ) frameLimit = 6;

        Application.SetUnhandledExceptionMode( UnhandledExceptionMode.ThrowException );
        ApplicationConfiguration.Initialize( );
#if DEBUG
        const bool debugLayer = true;
#else
        const bool debugLayer = false;
#endif
        using var device = GraphicsDevice.Create( new( ) {
            UseWarp = args.Contains( "--warp" ),
            UseAgility = !args.Contains( "--system-runtime" ),
            EnableDebugLayer = debugLayer && !args.Contains( "--no-debug" )
        } );
        var model = GltfModel.Load( Path.Combine( AppContext.BaseDirectory, "Models", "Patrol.gltf" ) );
        Console.WriteLine( device.AdapterName );
        Console.WriteLine( $"{fleet.Count:N0} ships, {model.Primitives.Length} shared primitives, {fleet.Count * FleetDraw.InstanceStride:N0} instance bytes per frame." );
        Console.WriteLine( "Left-click to select a cyan squadron, right-click to move, B to build, wheel to zoom." );

        using var compiler = new ShaderCompiler( );
        using var root = device.CreateGraphicsConstantsRootSignature( 16 );
        using var pipeline = FleetDraw.CreatePipeline( device, root, compiler, depthEnabled: true );
        using var spaceRoot = device.CreateGraphicsConstantsRootSignature( SpaceDraw.Constants );
        using var space = SpaceDraw.CreatePipeline( device, spaceRoot, compiler );
        using var hudRoot = device.CreateGraphicsConstantsRootSignature( 4 );
        using var hud = FleetHud.CreatePipeline( device, hudRoot, compiler );
        using var first = device.CreateBuffer( (ulong)fleet.Capacity * FleetDraw.InstanceStride, BufferMemory.Upload );
        using var second = device.CreateBuffer( (ulong)fleet.Capacity * FleetDraw.InstanceStride, BufferMemory.Upload );
        using var firstHud = device.CreateBuffer( FleetHud.BufferBytes, BufferMemory.Upload );
        using var secondHud = device.CreateBuffer( FleetHud.BufferBytes, BufferMemory.Upload );
        var meshes = new Mesh[model.Primitives.Length];
        try {
            UploadMeshes( device, model, meshes );
            var result = Present( device, pipeline, space, hud, fleet, meshes, [first, second], [firstHud, secondHud], frameLimit, resizeTest );
            var diagnostics = device.GetDiagnostics( );
            foreach ( string message in diagnostics.Messages ) Console.WriteLine( message );
            if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any(
                x => x.Contains( "SEVERITY_ERROR" ) || x.Contains( "SEVERITY_CORRUPTION" ) ) ) return 1;
            if ( frameLimit > 0 && result.Frames != frameLimit || resizeTest && result.Resizes != 2 ) return 1;
            Console.WriteLine( $"PASS: Fleet Battler presented {result.Frames} frames, {result.Resizes} resizes, {fleet.Tick} simulation ticks." );
            return 0;
        } finally {
            foreach ( var mesh in meshes ) mesh?.Dispose( );
        }
    }

    private static DesktopRunResult Present( GraphicsDevice device, PipelineState pipeline, PipelineState space, PipelineState hudPipeline,
        FleetSimulation fleet, Mesh[] meshes, GpuResource[] uploads, GpuResource[] hudUploads, int frameLimit, bool resizeTest ) {
        var instances = new ShipInstance[fleet.Capacity];
        var hud = new FleetHud( );
        bool oreShort = false;
        int selected = 0, slotsUsed = 0;
        float zoom = 0.94f;
        Vector2 cameraFocus = default;
        Point? panStart = null;
        double lag = 0;
        Matrix4x4 inverseViewProjection = default;
        Size viewport = default;
        float radius = fleet.BoardRadius;
        var run = DesktopGame.Run( device, new( ) {
            Title = "DXSharp · Fleet Battler", EnableDepth = true, FramesInFlight = uploads.Length, FrameLimit = frameLimit,
            ClearColor = new( 0.017f, 0.025f, 0.048f, 1 )
        }, frame => {
            lag += Math.Clamp( frame.Elapsed.TotalSeconds, 0, 0.25 );
            while ( lag >= FleetSimulation.StepSeconds ) {
                fleet.Step( );
                lag -= FleetSimulation.StepSeconds;
            }

            viewport = frame.Size;
            float aspect = (float)viewport.Width / viewport.Height;
            float halfHeight = radius * zoom / MathF.Min( aspect, 1 );
            var focus = new Vector3( cameraFocus.X, 0, cameraFocus.Y );
            var view = Matrix4x4.CreateLookAt( focus + new Vector3( 0, radius * 1.6f, radius * 0.88f ), focus, Vector3.UnitY );
            var projection = Matrix4x4.CreateOrthographic( 2 * halfHeight * aspect, 2 * halfHeight, 0.1f, 4 * radius );
            var viewProjection = view * projection;
            Matrix4x4.Invert( viewProjection, out inverseViewProjection );

            SpaceDraw.Record( frame.Commands, space, fleet, selected, viewProjection, inverseViewProjection, viewport.Width, viewport.Height, halfHeight );
            var active = instances.AsSpan( 0, fleet.Count );
            fleet.FillInstances( active, selected );
            // DesktopGame waits for this slot's fence before invoking the callback.
            slotsUsed |= 1 << frame.FrameSlotIndex;
            var upload = uploads[frame.FrameSlotIndex];
            upload.Write<ShipInstance>( active );
            FleetDraw.Record( frame.Commands, pipeline, meshes, upload, (uint)active.Length, viewProjection );
            hud.Record( frame.Commands, hudPipeline, hudUploads[frame.FrameSlotIndex], fleet, selected, viewport.Width, viewport.Height, oreShort );
        }, ( window, result ) => {
            if ( result.Frames == 1 ) {
                window.KeyPreview = true;
                window.KeyDown += ( _, e ) => {
                    if ( e.KeyCode != Keys.B ) return;
                    int wing = fleet.BuildWing( );
                    oreShort = wing < 0;
                    if ( wing >= 0 ) selected = wing;
                    e.SuppressKeyPress = true;
                };
                window.MouseDown += ( _, e ) => {
                    if ( e.Button == MouseButtons.Middle ) { panStart = e.Location; return; }
                    if ( !TryPick( e.Location, viewport, inverseViewProjection, out var target ) ) return;
                    if ( e.Button == MouseButtons.Left ) selected = fleet.FindSquad( target );
                    else if ( e.Button == MouseButtons.Right ) fleet.Move( selected, target );
                };
                window.MouseMove += ( _, e ) => {
                    if ( panStart is not { } previous ) return;
                    if ( e.Button != MouseButtons.Middle ) { panStart = null; return; }
                    if ( TryPick( previous, viewport, inverseViewProjection, out var from ) &&
                         TryPick( e.Location, viewport, inverseViewProjection, out var to ) )
                        cameraFocus = ClampCamera( cameraFocus + from - to );
                    panStart = e.Location;
                };
                window.MouseUp += ( _, e ) => { if ( e.Button == MouseButtons.Middle ) panStart = null; };
                window.MouseWheel += ( _, e ) => {
                    float nextZoom = Math.Clamp( zoom * MathF.Pow( 0.85f, e.Delta / 120f ), 0.18f, 2.2f );
                    if ( TryPick( e.Location, viewport, inverseViewProjection, out var anchor ) )
                        cameraFocus = ClampCamera( cameraFocus + (anchor - cameraFocus) * (1 - nextZoom / zoom) );
                    zoom = nextZoom;
                };
            }
            if ( resizeTest && result.Frames == 2 ) window.ClientSize = new( 800, 520 );
            if ( resizeTest && result.Frames == 4 ) window.ClientSize = new( 710, 575 );
        } );
        if ( resizeTest && slotsUsed != 3 )
            throw new InvalidOperationException( "Resize qualification did not exercise both frame slots." );
        return run;

        Vector2 ClampCamera( Vector2 center ) => Vector2.Clamp( center, new( -radius ), new( radius ) );

    }

    private static bool TryPick( Point pixel, Size viewport, Matrix4x4 inverse, out Vector2 point ) {
        point = default;
        if ( viewport.Width <= 0 || viewport.Height <= 0 ||
             pixel.X < 0 || pixel.Y < 0 || pixel.X >= viewport.Width || pixel.Y >= viewport.Height ) return false;
        float x = 2 * (pixel.X + 0.5f) / viewport.Width - 1;
        float y = 1 - 2 * (pixel.Y + 0.5f) / viewport.Height;
        var near = Vector4.Transform( new Vector4( x, y, 0, 1 ), inverse );
        var far = Vector4.Transform( new Vector4( x, y, 1, 1 ), inverse );
        var from = new Vector3( near.X, near.Y, near.Z ) / near.W;
        var direction = new Vector3( far.X, far.Y, far.Z ) / far.W - from;
        if ( MathF.Abs( direction.Y ) < 0.00001f ) return false;
        var hit = from - direction * (from.Y / direction.Y);
        point = new( hit.X, hit.Z );
        return float.IsFinite( point.X ) && float.IsFinite( point.Y );
    }

    private static void UploadMeshes( GraphicsDevice device, StaticModel model, Mesh[] meshes ) {
        using var queue = device.CreateCommandQueue( );
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        FleetDraw.RecordUploads( device, commands, model, meshes );
        commands.Close( );
        queue.Wait( queue.Submit( commands ) );
    }

    private static void Benchmark( FleetSimulation fleet, int ticks ) {
        var instances = new ShipInstance[fleet.Count];
        fleet.Move( 0, Vector2.Zero );
        for ( int squad = 1; squad < fleet.SquadCount; ++squad ) fleet.Move( squad, Vector2.Zero );
        for ( int i = 0; i < 128; ++i ) {
            fleet.Step( );
            fleet.FillInstances( instances, 0 );
        }

        long elapsedStep = 0, elapsedPack = 0;
        long allocated = GC.GetAllocatedBytesForCurrentThread( );
        for ( int i = 0; i < ticks; ++i ) {
            // Reverse the target periodically so long runs keep exercising movement.
            if ( i % 60 == 0 ) {
                float offset = (i / 60 & 1) == 0 ? fleet.BoardRadius * 0.4f : -fleet.BoardRadius * 0.4f;
                for ( int squad = 0; squad < fleet.SquadCount; ++squad ) {
                    float target = (squad & 1) == 0 ? offset : -offset;
                    fleet.Move( squad, new( target, target ) );
                }
            }
            long start = Stopwatch.GetTimestamp( );
            fleet.Step( );
            long packed = Stopwatch.GetTimestamp( );
            fleet.FillInstances( instances, 0 );
            elapsedStep += packed - start;
            elapsedPack += Stopwatch.GetTimestamp( ) - packed;
        }
        allocated = GC.GetAllocatedBytesForCurrentThread( ) - allocated;
        double microseconds = 1_000_000.0 / (Stopwatch.Frequency * (double)ticks);
        Console.WriteLine( $"{fleet.Count:N0} ships, {ticks:N0} ticks, simulation {elapsedStep * microseconds:F2} µs/tick, packing {elapsedPack * microseconds:F2} µs/tick." );
        Console.WriteLine( $"{allocated:N0} thread-allocated bytes, {fleet.Count * FleetDraw.InstanceStride:N0} instance bytes per frame, state {fleet.StateHash( ):X16}." );
    }

    private static string Option( string[] args, string prefix, string fallback ) =>
        args.FirstOrDefault( x => x.StartsWith( prefix, StringComparison.Ordinal ) )?[prefix.Length..] ?? fallback;
}
