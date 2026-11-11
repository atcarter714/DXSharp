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
        int ships = int.Parse( Option( args, "--ships=", FleetSimulation.DefaultShips.ToString( ) ) );
        uint seed = uint.Parse( Option( args, "--seed=", "37" ) );
        var fleet = new FleetSimulation( ships, seed, rivalPatrol: ticks == 0 );
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
        bool viewTest = args.Contains( "--view-test" );
        if ( viewTest && frameLimit is > 0 and < 8 )
            throw new ArgumentException( "View qualification requires at least eight frames." );
        if ( viewTest && frameLimit == 0 ) frameLimit = 8;
        int trafficCount = int.Parse( Option( args, "--traffic=", "192" ) );
        int debrisCount = int.Parse( Option( args, "--debris=", "384" ) );

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
        Console.WriteLine( $"{trafficCount:N0} compute-steered traffic ships and {debrisCount:N0} compute-orbiting debris bodies." );
        Console.WriteLine( "Click a ship, Shift-click a squadron, right-click to move or orbit. G opens the galaxy map. F11 toggles fullscreen." );

        using var compiler = new ShaderCompiler( );
        using var root = device.CreateGraphicsConstantsRootSignature( 16 );
        using var pipeline = FleetDraw.CreatePipeline( device, root, compiler, depthEnabled: true );
        using var thrustRoot = device.CreateGraphicsConstantsRootSignature( 20 );
        using var thrust = FleetDraw.CreateThrusters( device, thrustRoot, compiler );
        using var traffic = new ComputeTraffic( device, compiler, trafficCount, debrisCount );
        using var audio = new FleetAudio( args.Contains( "--mute" ) );
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
            var result = Present( device, pipeline, thrust, space, hud, traffic, audio, fleet, seed, meshes, [first, second], [firstHud, secondHud],
                frameLimit, resizeTest, viewTest, args.Contains( "--fullscreen" ), args.Contains( "--galaxy" ) );
            var diagnostics = device.GetDiagnostics( );
            foreach ( string message in diagnostics.Messages ) Console.WriteLine( message );
            if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any(
                x => x.Contains( "SEVERITY_ERROR" ) || x.Contains( "SEVERITY_CORRUPTION" ) ) ) return 1;
            if ( frameLimit > 0 && result.Frames != frameLimit || resizeTest && result.Resizes != 2 || viewTest && result.Resizes < 2 ) return 1;
            Console.WriteLine( $"PASS: Fleet Battler presented {result.Frames} frames, {result.Resizes} resizes, {fleet.Tick} simulation ticks." );
            return 0;
        } finally {
            foreach ( var mesh in meshes ) mesh?.Dispose( );
        }
    }

    private static DesktopRunResult Present( GraphicsDevice device, PipelineState pipeline, PipelineState thrust, PipelineState space,
        PipelineState hudPipeline, ComputeTraffic traffic, FleetAudio audio, FleetSimulation initial, uint seed, Mesh[] meshes,
        GpuResource[] uploads, GpuResource[] hudUploads, int frameLimit, bool resizeTest, bool viewTest, bool startFullscreen, bool startGalaxy ) {
        var systems = new FleetSimulation?[CelestialSystem.Systems.Length];
        systems[0] = initial;
        var fleet = initial;
        var instances = new ShipInstance[fleet.Capacity];
        var screenBodies = new Vector4[CelestialSystem.BodyCount];
        var hud = new FleetHud( );
        var display = new FullscreenToggle( );
        var selected = new FleetSelection( 0 );
        bool oreShort = false, galaxy = startGalaxy || viewTest, sawGalaxy = false;
        int slotsUsed = 0, systemsVisited = 0, previousOwner = -1;
        float zoom = 0.85f, worldPerPixel = 1;
        Vector2 cameraFocus = default;
        Point? panStart = null;
        double lag = 0;
        Matrix4x4 inverseViewProjection = default;
        Size viewport = default;
        var work = Screen.PrimaryScreen?.WorkingArea.Size ?? new Size( 1600, 900 );
        var windowSize = new Size( Math.Min( 1600, Math.Max( 480, work.Width - 48 ) ),
            Math.Min( 900, Math.Max( 320, work.Height - 92 ) ) );
        var run = DesktopGame.Run( device, new( ) {
            Title = "DXSharp · Fleet Battler", EnableDepth = true, FramesInFlight = uploads.Length, FrameLimit = frameLimit,
            ClientSize = viewTest ? new Size( 780, 490 ) : windowSize, ClearColor = new( 0.017f, 0.025f, 0.048f, 1 )
        }, frame => {
            viewport = frame.Size;
            slotsUsed |= 1 << frame.FrameSlotIndex;
            var hudUpload = hudUploads[frame.FrameSlotIndex];
            audio.SetSituation( galaxy, fleet.Contested, fleet.WingsBuilt == FleetSimulation.WingGoal );
            if ( galaxy ) {
                sawGalaxy = true;
                SpaceDraw.RecordGalaxy( frame.Commands, space, fleet.SystemIndex, viewport.Width, viewport.Height );
                hud.RecordGalaxy( frame.Commands, hudPipeline, hudUpload, fleet.SystemIndex, viewport.Width, viewport.Height );
                return;
            }

            systemsVisited |= 1 << fleet.SystemIndex;
            float elapsed = (float)Math.Clamp( frame.Elapsed.TotalSeconds, 0, 0.25 );
            lag += elapsed;
            while ( lag >= FleetSimulation.StepSeconds ) {
                fleet.Step( );
                lag -= FleetSimulation.StepSeconds;
            }
            if ( previousOwner != fleet.Owner ) {
                if ( fleet.Owner == 0 ) audio.Cue( "capture" );
                previousOwner = fleet.Owner;
            }

            float radius = fleet.BoardRadius;
            float aspect = (float)viewport.Width / viewport.Height;
            float halfHeight = radius * zoom / MathF.Min( aspect, 1 );
            worldPerPixel = 2 * halfHeight / viewport.Height;
            var focus = new Vector3( cameraFocus.X, 0, cameraFocus.Y );
            var view = Matrix4x4.CreateLookAt( focus + new Vector3( 0, radius * 1.6f, radius * 0.88f ), focus, Vector3.UnitY );
            var projection = Matrix4x4.CreateOrthographic( 2 * halfHeight * aspect, 2 * halfHeight, 0.1f, 4 * radius );
            var viewProjection = view * projection;
            Matrix4x4.Invert( viewProjection, out inverseViewProjection );
            for ( int i = 0; i < fleet.Bodies.Length; ++i )
                screenBodies[i] = CelestialSystem.Project( fleet.Bodies[i], radius, viewProjection, viewport.Width, viewport.Height, halfHeight );

            bool computeActive = traffic.ShipCount != 0 || traffic.DebrisCount != 0;
            if ( computeActive ) traffic.RecordCompute( frame.Commands, fleet, elapsed );
            SpaceDraw.Record( frame.Commands, space, fleet, selected, viewProjection, inverseViewProjection, viewport.Width, viewport.Height, halfHeight );
            if ( computeActive ) {
                traffic.RecordDebris( frame.Commands, viewProjection, viewport.Width, viewport.Height, halfHeight );
                if ( traffic.ShipCount != 0 )
                    FleetDraw.Record( frame.Commands, pipeline, meshes, traffic.Instances, traffic.ShipCount, viewProjection );
                traffic.Finish( frame.Commands );
            }

            var active = instances.AsSpan( 0, fleet.Count );
            fleet.FillInstances( active, selected );
            var upload = uploads[frame.FrameSlotIndex];
            upload.Write<ShipInstance>( active );
            FleetDraw.RecordThrusters( frame.Commands, thrust, upload, (uint)active.Length, viewProjection, (float)(fleet.Tick * FleetSimulation.StepSeconds) );
            FleetDraw.Record( frame.Commands, pipeline, meshes, upload, (uint)active.Length, viewProjection );
            hud.Record( frame.Commands, hudPipeline, hudUpload, fleet, selected, viewport.Width, viewport.Height, oreShort, screenBodies );
        }, ( window, result ) => {
            if ( result.Frames == 1 ) {
                if ( startFullscreen ) display.Toggle( window );
                window.KeyPreview = true;
                window.KeyDown += ( _, e ) => {
                    switch ( e.KeyCode ) {
                        case Keys.F11: display.Toggle( window ); break;
                        case Keys.G: galaxy = !galaxy; panStart = null; break;
                        case Keys.M: audio.Toggle( ); break;
                        case Keys.R when !galaxy: fleet.Rejoin( selected ); break;
                        case Keys.B when !galaxy:
                            int wing = fleet.BuildWing( );
                            oreShort = wing < 0;
                            if ( wing >= 0 ) {
                                selected = new( wing );
                                audio.Cue( "launch" );
                            }
                            break;
                        default: return;
                    }
                    e.SuppressKeyPress = true;
                };
                window.MouseDown += ( _, e ) => {
                    if ( galaxy ) {
                        if ( e.Button != MouseButtons.Left || viewport.Width == 0 ) return;
                        var mouse = new Vector2( e.X, e.Y );
                        var dimensions = new Vector2( viewport.Width, viewport.Height );
                        var points = CelestialSystem.MapPositions;
                        for ( int i = 0; i < points.Length; ++i ) {
                            var star = points[i] * dimensions;
                            if ( Vector2.DistanceSquared( mouse, star ) > MathF.Pow( MathF.Max( 29, viewport.Height * 0.042f ), 2 ) ) continue;
                            Enter( i );
                            break;
                        }
                        return;
                    }
                    if ( e.Button == MouseButtons.Middle ) { panStart = e.Location; return; }
                    if ( !TryPick( e.Location, viewport, inverseViewProjection, out var target ) ) return;
                    if ( e.Button == MouseButtons.Left ) selected = fleet.Pick( target, (Control.ModifierKeys & Keys.Shift) != 0, worldPerPixel * 14 );
                    else if ( e.Button == MouseButtons.Right ) {
                        int orbit = PickBody( e.Location );
                        if ( orbit >= 0 ) fleet.Orbit( selected, orbit );
                        else fleet.Move( selected, target );
                        audio.Cue( "order" );
                    }
                };
                window.MouseMove += ( _, e ) => {
                    if ( galaxy || panStart is not { } previous ) return;
                    if ( e.Button != MouseButtons.Middle ) { panStart = null; return; }
                    if ( TryPick( previous, viewport, inverseViewProjection, out var from ) &&
                         TryPick( e.Location, viewport, inverseViewProjection, out var to ) )
                        cameraFocus = ClampCamera( cameraFocus + from - to );
                    panStart = e.Location;
                };
                window.MouseUp += ( _, e ) => { if ( e.Button == MouseButtons.Middle ) panStart = null; };
                window.MouseWheel += ( _, e ) => {
                    if ( galaxy ) return;
                    float nextZoom = Math.Clamp( zoom * MathF.Pow( 0.85f, e.Delta / 120f ), 0.15f, 2.5f );
                    if ( TryPick( e.Location, viewport, inverseViewProjection, out var anchor ) )
                        cameraFocus = ClampCamera( cameraFocus + (anchor - cameraFocus) * (1 - nextZoom / zoom) );
                    zoom = nextZoom;
                };
            }
            if ( resizeTest && result.Frames == 2 ) window.ClientSize = new( 800, 520 );
            if ( resizeTest && result.Frames == 4 ) window.ClientSize = new( 710, 575 );
            if ( viewTest ) {
                if ( result.Frames == 2 ) { display.Toggle( window ); Enter( 2 ); }
                if ( result.Frames == 4 ) display.Toggle( window );
                if ( result.Frames == 5 ) galaxy = true;
                if ( result.Frames == 6 ) Enter( 0 );
            }
        } );
        if ( resizeTest && slotsUsed != 3 )
            throw new InvalidOperationException( "Resize qualification did not exercise both frame slots." );
        if ( viewTest && (!sawGalaxy || systemsVisited != 5 || display.Active) )
            throw new InvalidOperationException( "View qualification did not exercise the galaxy, both systems, and fullscreen restoration." );
        return run;

        Vector2 ClampCamera( Vector2 center ) => Vector2.Clamp( center, new( -fleet.BoardRadius ), new( fleet.BoardRadius ) );

        void Enter( int index ) {
            fleet = systems[index] ??= new FleetSimulation( initial.Capacity - FleetSimulation.WingSize * FleetSimulation.WingGoal,
                unchecked(seed + (uint)index * 1177), rivalPatrol: true, system: index );
            selected = new( 0 );
            previousOwner = fleet.Owner;
            oreShort = galaxy = false;
            zoom = 0.85f;
            cameraFocus = default;
            lag = 0;
        }

        int PickBody( Point pixel ) {
            int body = -1;
            float closest = 1;
            for ( int i = 0; i < screenBodies.Length; ++i ) {
                var screen = screenBodies[i];
                float radius = MathF.Max( 8, screen.Z );
                float distance = Vector2.DistanceSquared( new( pixel.X, pixel.Y ), new( screen.X, screen.Y ) ) / (radius * radius);
                if ( distance >= closest ) continue;
                closest = distance;
                body = i;
            }
            return body;
        }
    }

    private sealed class FullscreenToggle {
        private Rectangle _bounds;
        private FormWindowState _state;
        private FormBorderStyle _border;
        internal bool Active { get; private set; }

        internal void Toggle( Form window ) {
            if ( Active ) {
                window.FormBorderStyle = _border;
                window.Bounds = _bounds;
                window.WindowState = _state;
                Active = false;
            } else {
                _state = window.WindowState;
                _bounds = _state == FormWindowState.Normal ? window.Bounds : window.RestoreBounds;
                _border = window.FormBorderStyle;
                var screen = Screen.FromControl( window ).Bounds;
                window.WindowState = FormWindowState.Normal;
                window.FormBorderStyle = FormBorderStyle.None;
                window.Bounds = screen;
                Active = true;
            }
        }
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
            fleet.FillInstances( instances, new( 0 ) );
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
            fleet.FillInstances( instances, new( 0 ) );
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
