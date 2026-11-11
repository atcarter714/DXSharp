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
        var fleet = new FleetSimulation( ships, seed, rivalPatrol: ticks == 0, battle: ticks == 0 );
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
        string shipModel = Option( args, "--ship-model=", Path.Combine( AppContext.BaseDirectory, "Models", "Patrol.gltf" ) );
        var model = GltfModel.Load( shipModel );
        Console.WriteLine( device.AdapterName );
        Console.WriteLine( $"{fleet.Count:N0} ships, {model.Primitives.Length} shared primitives, {fleet.Count * FleetDraw.InstanceStride:N0} instance bytes per frame." );
        Console.WriteLine( $"{trafficCount:N0} compute-steered traffic ships and {debrisCount:N0} compute-orbiting debris bodies." );
        Console.WriteLine( "Click or drag to select. Shift adds, Ctrl toggles. Right-click to move, orbit, or attack. Click a planet to develop it." );

        using var compiler = new ShaderCompiler( );
        using var root = device.CreateGraphicsConstantsRootSignature( 16 );
        using var pipeline = FleetDraw.CreatePipeline( device, root, compiler, depthEnabled: true );
        using var thrustRoot = device.CreateGraphicsConstantsRootSignature( 20 );
        using var thrust = FleetDraw.CreateThrusters( device, thrustRoot, compiler );
        using var battleRoot = device.CreateGraphicsConstantsRootSignature( BattleDraw.ConstantCount );
        using var effects = BattleDraw.CreateEffects( device, battleRoot, compiler );
        using var markers = BattleDraw.CreateMarkers( device, battleRoot, compiler );
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
        using var firstEffects = device.CreateBuffer( FleetSimulation.EffectCapacity * BattleDraw.EffectStride, BufferMemory.Upload );
        using var secondEffects = device.CreateBuffer( FleetSimulation.EffectCapacity * BattleDraw.EffectStride, BufferMemory.Upload );
        var meshes = new Mesh[model.Primitives.Length];
        try {
            UploadMeshes( device, model, meshes );
            var result = Present( device, pipeline, thrust, space, hud, effects, markers, traffic, audio, fleet, seed, meshes,
                [first, second], [firstHud, secondHud], [firstEffects, secondEffects], frameLimit, resizeTest, viewTest, args.Contains( "--fullscreen" ), args.Contains( "--galaxy" ) );
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
        PipelineState hudPipeline, PipelineState effectsPipeline, PipelineState markers, ComputeTraffic traffic, FleetAudio audio,
        FleetSimulation initial, uint seed, Mesh[] meshes, GpuResource[] uploads, GpuResource[] hudUploads, GpuResource[] effectsUploads, int frameLimit, bool resizeTest, bool viewTest, bool startFullscreen, bool startGalaxy ) {
        var systems = new FleetSimulation?[CelestialSystem.Systems.Length];
        systems[0] = initial;
        var fleet = initial;
        var instances = new ShipInstance[fleet.Capacity];
        var screenBodies = new Vector4[CelestialSystem.BodyCount];
        var hud = new FleetHud( );
        var display = new FullscreenToggle( );
        var selection = new CommandSelection( fleet.Capacity );
        selection.Squadron( fleet, 0, SelectionMode.Replace );
        var effects = new BattleEffect[FleetSimulation.EffectCapacity];
        int planet = -1, noticeUntil = 0, shotsHeard = 0, lossesHeard = 0, nextCue = 0;
        string? notice = null;
        bool oreShort = false, galaxy = startGalaxy || viewTest, sawGalaxy = false;
        int slotsUsed = 0, systemsVisited = 0, previousOwner = -1;
        float zoom = 0.85f;
        Vector2 cameraFocus = default;
        Point? panStart = null, dragStart = null;
        Point dragEnd = default;
        SelectionMode dragMode = SelectionMode.Replace;
        double lag = 0;
        Matrix4x4 inverseViewProjection = default, viewProjection = default;
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
            audio.SetSituation( galaxy, fleet.Contested || fleet.InBattle, fleet.Won );
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

            if ( fleet.Tick >= nextCue && shotsHeard != fleet.ShotsFired ) {
                audio.Cue( lossesHeard != fleet.ShipsLost ? "burst" : "laser" );
                shotsHeard = fleet.ShotsFired;
                lossesHeard = fleet.ShipsLost;
                nextCue = fleet.Tick + 20;
            }
            selection.Prune( fleet );
            var selected = selection.Focus( fleet );
            float radius = fleet.BoardRadius;
            float aspect = (float)viewport.Width / viewport.Height;
            float halfHeight = radius * zoom / MathF.Min( aspect, 1 );
            var focus = new Vector3( cameraFocus.X, 0, cameraFocus.Y );
            var view = Matrix4x4.CreateLookAt( focus + new Vector3( 0, radius * 1.6f, radius * 0.88f ), focus, Vector3.UnitY );
            var projection = Matrix4x4.CreateOrthographic( 2 * halfHeight * aspect, 2 * halfHeight, 0.1f, 4 * radius );
            viewProjection = view * projection;
            Matrix4x4.Invert( viewProjection, out inverseViewProjection );
            for ( int i = 0; i < fleet.Bodies.Length; ++i )
                screenBodies[i] = CelestialSystem.Project( fleet.Bodies[i], radius, viewProjection, viewport.Width, viewport.Height, halfHeight );

            bool computeActive = traffic.ShipCount != 0 || traffic.DebrisCount != 0;
            if ( computeActive ) traffic.RecordCompute( frame.Commands, fleet, elapsed );
            SpaceDraw.Record( frame.Commands, space, fleet, selected, viewProjection, inverseViewProjection,
                viewport.Width, viewport.Height, halfHeight, showSelection: selection.Count == 1 );
            if ( computeActive ) {
                traffic.RecordDebris( frame.Commands, viewProjection, viewport.Width, viewport.Height, halfHeight );
                traffic.RecordFreighters( frame.Commands, viewProjection );
                traffic.Finish( frame.Commands );
            }

            var active = instances.AsSpan( 0, fleet.ActiveCount );
            fleet.FillInstances( active, selected, selection.Ships );
            var upload = uploads[frame.FrameSlotIndex];
            if ( !active.IsEmpty ) {
                upload.Write<ShipInstance>( active );
                FleetDraw.RecordThrusters( frame.Commands, thrust, upload, (uint)active.Length, viewProjection, (float)(fleet.Tick * FleetSimulation.StepSeconds) );
                FleetDraw.Record( frame.Commands, pipeline, meshes, upload, (uint)active.Length, viewProjection );
                BattleDraw.Record( frame.Commands, markers, upload, (uint)active.Length, viewProjection, viewport.Width, viewport.Height, halfHeight, effects: false );
            }
            int pulseCount = fleet.FillEffects( effects );
            if ( pulseCount != 0 ) {
                var pulses = effectsUploads[frame.FrameSlotIndex];
                pulses.Write<BattleEffect>( effects.AsSpan( 0, pulseCount ) );
                BattleDraw.Record( frame.Commands, effectsPipeline, pulses, (uint)pulseCount, viewProjection,
                    viewport.Width, viewport.Height, halfHeight, effects: true );
            }
            var drag = dragStart is { } start && IsDrag( start, dragEnd ) ? DragRect( start, dragEnd ) : (Vector4?)null;
            hud.Record( frame.Commands, hudPipeline, hudUpload, fleet, selected, viewport.Width, viewport.Height, oreShort, screenBodies,
                selection.Count, planet, fleet.Tick < noticeUntil ? notice : null, drag,
                CommandSelection.Project( fleet.Site, viewProjection, new( viewport.Width, viewport.Height ) ) );
        }, ( window, result ) => {
            if ( result.Frames == 1 ) {
                if ( startFullscreen ) display.Toggle( window );
                window.KeyPreview = true;
                window.KeyDown += ( _, e ) => {
                    switch ( e.KeyCode ) {
                        case Keys.F11: display.Toggle( window ); break;
                        case Keys.G: galaxy = !galaxy; panStart = dragStart = null; window.Capture = false; break;
                        case Keys.M: audio.Toggle( ); break;
                        case Keys.R when !galaxy:
                            for ( int ship = 0; ship < fleet.Count; ++ship )
                                if ( selection.Ships[ship] && fleet.Alive( ship ) ) fleet.Rejoin( new( fleet.SquadronOf( ship ), ship ) );
                            break;
                        case Keys.A when !galaxy && e.Control: selection.All( fleet ); break;
                        case Keys.E when !galaxy: PlanetOrder( PlanetAction.Survey ); break;
                        case Keys.N when !galaxy: PlanetOrder( PlanetAction.Mine ); break;
                        case Keys.L when !galaxy: PlanetOrder( PlanetAction.Lab ); break;
                        case Keys.T when !galaxy: PlanetOrder( PlanetAction.Research ); break;
                        case Keys.B when !galaxy:
                            int wing = fleet.BuildWing( );
                            oreShort = wing < 0;
                            if ( wing >= 0 ) {
                                selection.Squadron( fleet, wing, SelectionMode.Replace );
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
                    var pointer = new Vector2( e.X, e.Y );
                    if ( e.Button == MouseButtons.Left ) {
                        var action = FleetHud.ActionAt( pointer, viewport.Width, viewport.Height, planet );
                        if ( action != PlanetAction.None ) { PlanetOrder( action ); return; }
                        dragStart = dragEnd = e.Location;
                        var mods = Control.ModifierKeys;
                        dragMode = (mods & Keys.Control) != 0 ? SelectionMode.Toggle
                            : (mods & Keys.Shift) != 0 ? SelectionMode.Add : SelectionMode.Replace;
                        window.Capture = true;
                        return;
                    }
                    if ( e.Button != MouseButtons.Right || selection.Count == 0 ||
                         !TryPick( e.Location, viewport, inverseViewProjection, out var target ) ) return;
                    int enemy = CommandSelection.Hit( fleet, pointer, viewProjection, new( viewport.Width, viewport.Height ), 1 );
                    if ( enemy >= 0 ) {
                        selection.Issue( fleet, target, hostile: enemy );
                        Notify( "ATTACK ORDER" );
                    } else {
                        int orbit = PickBody( e.Location );
                        selection.Issue( fleet, target, orbit );
                        Notify( orbit >= 0 ? "ORBIT ORDER" : "MOVE ORDER" );
                    }
                    audio.Cue( "order" );
                };
                window.MouseMove += ( _, e ) => {
                    if ( galaxy ) return;
                    if ( dragStart is not null ) { dragEnd = e.Location; return; }
                    if ( panStart is not { } previous ) return;
                    if ( e.Button != MouseButtons.Middle ) { panStart = null; return; }
                    if ( TryPick( previous, viewport, inverseViewProjection, out var from ) &&
                         TryPick( e.Location, viewport, inverseViewProjection, out var to ) )
                        cameraFocus = ClampCamera( cameraFocus + from - to );
                    panStart = e.Location;
                };
                window.MouseCaptureChanged += ( _, _ ) => { if ( !window.Capture ) dragStart = null; };
                window.MouseUp += ( _, e ) => {
                    if ( e.Button == MouseButtons.Middle ) { panStart = null; return; }
                    if ( e.Button != MouseButtons.Left || dragStart is not { } start ) return;
                    dragStart = null;
                    window.Capture = false;
                    var dimensions = new Vector2( viewport.Width, viewport.Height );
                    if ( IsDrag( start, e.Location ) ) {
                        selection.Box( fleet, DragRect( start, e.Location ), viewProjection, dimensions, dragMode );
                        planet = -1;
                    } else {
                        int ship = CommandSelection.Hit( fleet, new( e.X, e.Y ), viewProjection, dimensions, 0 );
                        int hitBody = ship < 0 ? PickBody( e.Location ) : -1;
                        if ( hitBody >= 0 ) planet = hitBody;
                        else {
                            selection.Click( fleet, ship, e.Clicks >= 2, dragMode );
                            planet = -1;
                        }
                    }
                };
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
                unchecked(seed + (uint)index * 1177), rivalPatrol: true, system: index, battle: true );
            selection.Squadron( fleet, 0, SelectionMode.Replace );
            planet = -1;
            notice = null;
            shotsHeard = fleet.ShotsFired;
            lossesHeard = fleet.ShipsLost;
            nextCue = 0;
            previousOwner = fleet.Owner;
            oreShort = galaxy = false;
            zoom = 0.85f;
            cameraFocus = default;
            lag = 0;
        }

        void Notify( string text ) {
            notice = text;
            noticeUntil = fleet.Tick + 180;
        }

        void PlanetOrder( PlanetAction action ) {
            if ( action == PlanetAction.Research ) {
                Notify( fleet.ResearchWeapons( ) ? "WEAPONS UPGRADE ACTIVE" : "RESEARCH NEEDS 30 SCIENCE" );
                return;
            }
            if ( planet <= 0 ) { Notify( "SELECT A PLANET OR MOON" ); return; }
            if ( action == PlanetAction.Survey ) {
                if ( selection.Count == 0 ) { Notify( "SELECT A SURVEY SHIP FIRST" ); return; }
                if ( fleet.Planet( planet ).Surveyed ) { Notify( "SURVEY ALREADY COMPLETE" ); return; }
                fleet.BeginSurvey( planet );
                selection.Issue( fleet, default, orbit: planet );
                Notify( "SURVEY TEAM DISPATCHED" );
                audio.Cue( "order" );
                return;
            }
            var facility = action == PlanetAction.Mine ? PlanetFacility.Mine : PlanetFacility.Lab;
            bool started = fleet.BuildFacility( planet, facility );
            Notify( started ? facility == PlanetFacility.Mine ? "MINE CONSTRUCTION STARTED" : "LAB CONSTRUCTION STARTED"
                : "BUILD REQUIRES SURVEY AND ORE" );
            if ( started ) audio.Cue( "launch" );
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

    private static bool IsDrag( Point from, Point to ) => Math.Abs( from.X - to.X ) > 5 || Math.Abs( from.Y - to.Y ) > 5;

    private static Vector4 DragRect( Point a, Point b ) =>
        new( Math.Min( a.X, b.X ), Math.Min( a.Y, b.Y ), Math.Max( a.X, b.X ), Math.Max( a.Y, b.Y ) );

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
