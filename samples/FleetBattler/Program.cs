using System.Diagnostics;
using System.Numerics;
using DXSharp.Assets;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;
using DXSharp.Samples;

namespace FleetBattler;

internal static class Program {
    private const int StationSlots = 2 + 2 * (CelestialSystem.BodyCount - 1);

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
        bool frontier = ticks == 0 && !args.Contains( "--skirmish" ) &&
            !args.Any( arg => arg.StartsWith( "--ships=", StringComparison.Ordinal ) );
        var fleet = new FleetSimulation( ships, seed, rivalPatrol: ticks == 0, battle: ticks == 0, frontier: frontier );
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
        int trafficCount = int.Parse( Option( args, "--traffic=", ComputeTraffic.DefaultAgents.ToString( ) ) );
        int debrisCount = int.Parse( Option( args, "--debris=", ComputeTraffic.DefaultDebris.ToString( ) ) );

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
        string models = Path.Combine( AppContext.BaseDirectory, "Models" );
        string corvettePath = Path.Combine( models, "Corvette.gltf" );
        string shipModel = Option( args, "--ship-model=", corvettePath );
        Console.WriteLine( device.AdapterName );
        Console.WriteLine( $"{fleet.Count:N0} ships, {fleet.Count * FleetDraw.InstanceStride:N0} tactical instance bytes per frame." );
        Console.WriteLine( $"{trafficCount:N0} compute-steered traffic ships and {debrisCount:N0} compute-orbiting debris bodies." );
        Console.WriteLine( "Click or drag to select. Shift adds, Ctrl toggles. Right-click a body for survey and orbit orders. Left-click a body to inspect it." );

        using var compiler = new ShaderCompiler( );
        using var root = device.CreateTextureRootSignature( FleetDraw.MaterialConstants, linearWrap: true );
        using var pipeline = FleetDraw.CreatePipeline( device, root, compiler, depthEnabled: true );
        using var thrustRoot = device.CreateGraphicsConstantsRootSignature( 20 );
        using var thrust = FleetDraw.CreateThrusters( device, thrustRoot, compiler );
        using var battleRoot = device.CreateGraphicsConstantsRootSignature( BattleDraw.ConstantCount );
        using var effects = BattleDraw.CreateEffects( device, battleRoot, compiler );
        using var markers = BattleDraw.CreateMarkers( device, battleRoot, compiler );
        using var traffic = new ComputeTraffic( device, compiler, trafficCount, debrisCount );
        using var audio = new FleetAudio( args.Contains( "--mute" ) );
        using var spaceRoot = device.CreateTextureRootSignature( SpaceDraw.Constants, linearWrap: true );
        using var space = SpaceDraw.CreatePipeline( device, spaceRoot, compiler );
        using var hudRoot = device.CreateGraphicsConstantsRootSignature( 4 );
        using var hud = FleetHud.CreatePipeline( device, hudRoot, compiler );
        using var first = device.CreateBuffer( (ulong)(fleet.Capacity + StationSlots) * FleetDraw.InstanceStride, BufferMemory.Upload );
        using var second = device.CreateBuffer( (ulong)(fleet.Capacity + StationSlots) * FleetDraw.InstanceStride, BufferMemory.Upload );
        using var firstHud = device.CreateBuffer( FleetHud.BufferBytes, BufferMemory.Upload );
        using var secondHud = device.CreateBuffer( FleetHud.BufferBytes, BufferMemory.Upload );
        using var firstEffects = device.CreateBuffer( FleetSimulation.EffectCapacity * BattleDraw.EffectStride, BufferMemory.Upload );
        using var secondEffects = device.CreateBuffer( FleetSimulation.EffectCapacity * BattleDraw.EffectStride, BufferMemory.Upload );
        using var queue = device.CreateCommandQueue( );
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        using var corvettes = new StaticMeshSet( device, commands, GltfModel.Load( shipModel ), shipModel == corvettePath ? 0.05f : 1,
            flattenScene: shipModel != corvettePath );
        using var scouts = LoadModel( "--scout-model=", "SurveyScout.gltf", 0.083f );
        using var freighters = LoadModel( "--freighter-model=", "Freighter.gltf", 0.05f );
        string? minerPath = args.FirstOrDefault( arg => arg.StartsWith( "--miner-model=", StringComparison.Ordinal ) )?[14..];
        using var minerOverride = minerPath is null ? null : new StaticMeshSet( device, commands, GltfModel.Load( minerPath ), flattenScene: true );
        using var relay = LoadModel( "--station-model=", "MiningRelay.gltf", 0.12f );
        string defaultMoon = Path.Combine( AppContext.BaseDirectory, "Celestial", "Moon.jpg" );
        string? moonPath = args.FirstOrDefault( value => value.StartsWith( "--moon-map=", StringComparison.Ordinal ) )?[11..];
        if ( moonPath is null && File.Exists( defaultMoon ) ) moonPath = defaultMoon;
        using var moon = SpaceDraw.LoadMoon( device, commands, moonPath );
        commands.Close( );
        queue.Wait( queue.Submit( commands ) );

        var result = Present( device, pipeline, thrust, space, moon, moonPath is not null, hud, effects, markers, traffic, audio, fleet, seed,
            corvettes, scouts, freighters, minerOverride ?? freighters, relay, [first, second], [firstHud, secondHud], [firstEffects, secondEffects],
            frameLimit, resizeTest, viewTest, args.Contains( "--fullscreen" ), args.Contains( "--galaxy" ) );
        var diagnostics = device.GetDiagnostics( );
        foreach ( string message in diagnostics.Messages ) Console.WriteLine( message );
        if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any(
            x => x.Contains( "SEVERITY_ERROR" ) || x.Contains( "SEVERITY_CORRUPTION" ) ) ) return 1;
        if ( frameLimit > 0 && result.Frames != frameLimit || resizeTest && result.Resizes != 2 || viewTest && result.Resizes < 2 ) return 1;
        Console.WriteLine( $"PASS: Fleet Battler presented {result.Frames} frames, {result.Resizes} resizes, {fleet.Tick} simulation ticks." );
        return 0;

        StaticMeshSet LoadModel( string option, string bundled, float bundledScale ) {
            string? custom = args.FirstOrDefault( arg => arg.StartsWith( option, StringComparison.Ordinal ) )?[option.Length..];
            return new( device, commands, GltfModel.Load( custom ?? Path.Combine( models, bundled ) ),
                custom is null ? bundledScale : 1, flattenScene: custom is not null );
        }
    }

    private static DesktopRunResult Present( GraphicsDevice device, PipelineState pipeline, PipelineState thrust, PipelineState space, SampledTexture2D moon, bool useMoonMap,
        PipelineState hudPipeline, PipelineState effectsPipeline, PipelineState markers, ComputeTraffic traffic, FleetAudio audio,
        FleetSimulation initial, uint seed, StaticMeshSet corvettes, StaticMeshSet scouts, StaticMeshSet freighters, StaticMeshSet minerMeshes, StaticMeshSet relay,
        GpuResource[] uploads, GpuResource[] hudUploads, GpuResource[] effectsUploads, int frameLimit, bool resizeTest, bool viewTest, bool startFullscreen, bool startGalaxy ) {
        var systems = new FleetSimulation?[CelestialSystem.Systems.Length];
        systems[0] = initial;
        var fleet = initial;
        var instances = new ShipInstance[fleet.Capacity + StationSlots];
        var screenBodies = new Vector4[CelestialSystem.BodyCount];
        var hud = new FleetHud( );
        var display = new FullscreenToggle( );
        var selection = new CommandSelection( fleet.Capacity );
        selection.Squadron( fleet, 0, SelectionMode.Replace );
        var effects = new BattleEffect[FleetSimulation.EffectCapacity];
        int planet = initial.Frontier ? FleetSimulation.Homeworld : -1, shotsHeard = 0, lossesHeard = 0, nextCue = 0;
        long noticeUntil = 0;
        string? notice = null;
        bool oreShort = false, galaxy = startGalaxy || viewTest, sawGalaxy = false;
        int slotsUsed = 0, systemsVisited = 0, previousOwner = -1, speed = 1;
        int surveysHeard = 0, wingsHeard = 0, yardState = -1;
        BodyMenu? menu = null;
        CommandTab commandTab = initial.Frontier ? CommandTab.Events : CommandTab.Hidden;
        int dispatch = 0, dispatchesSeen = initial.DispatchCount;
        var clock = new FleetClock( );
        bool paused = false;
        float zoom = 0.85f;
        Vector2 cameraFocus = default;
        Point? panStart = null, dragStart = null;
        Point dragEnd = default, lastClickPoint = default;
        int lastClickShip = -1;
        long lastClickTime = 0;
        SelectionMode dragMode = SelectionMode.Replace;
        float interpolation = 1;
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
                SpaceDraw.RecordGalaxy( frame.Commands, space, moon, fleet.SystemIndex, viewport.Width, viewport.Height );
                hud.RecordGalaxy( frame.Commands, hudPipeline, hudUpload, fleet.SystemIndex, viewport.Width, viewport.Height );
                return;
            }

            systemsVisited |= 1 << fleet.SystemIndex;
            float elapsed = clock.Advance( fleet, frame.Elapsed.TotalSeconds, speed, paused );
            interpolation = clock.Interpolation;
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
            if ( surveysHeard != fleet.CompletedSurveys ) {
                surveysHeard = fleet.CompletedSurveys;
                Notify( $"{UnitName( fleet.LastSurveyUnit )} SURVEYED {CelestialSystem.Name( fleet.SystemIndex, fleet.LastSurveyBody )}" );
                audio.Cue( "survey-complete" );
            }
            if ( wingsHeard != fleet.WingsBuilt ) {
                wingsHeard = fleet.WingsBuilt;
                Notify( $"SQUAD {fleet.SquadCount:D2} LAUNCHED FROM YARD" );
                audio.Cue( "launch" );
            } else if ( yardState > 0 && fleet.YardRemaining == 0 ) {
                Notify( "ORBITAL SHIPYARD ONLINE" );
                audio.Cue( "launch" );
            }
            yardState = fleet.YardRemaining;
            if ( dispatchesSeen < fleet.DispatchCount ) {
                dispatchesSeen = fleet.DispatchCount;
                dispatch = dispatchesSeen - 1;
                var incident = fleet.Dispatch( dispatch );
                Notify( FleetHud.IncidentTitle( incident ) );
                audio.Cue( "order" );
            }
            selection.Prune( fleet );
            var unit = selection.SurveyUnit( fleet );
            var selected = unit ?? selection.Focus( fleet );
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
            SpaceDraw.Record( frame.Commands, space, moon, useMoonMap, fleet, selected, viewProjection, inverseViewProjection,
                viewport.Width, viewport.Height, halfHeight, showSelection: unit is not null, interpolation: interpolation );
            if ( computeActive ) {
                traffic.RecordDebris( frame.Commands, viewProjection, viewport.Width, viewport.Height, halfHeight );
                traffic.RecordFreighters( frame.Commands, pipeline, freighters, viewProjection, viewport.Height / (2 * halfHeight) );
                traffic.Finish( frame.Commands );
            }

            var active = instances.AsSpan( 0, fleet.ActiveCount );
            uint corvetteCount = (uint)fleet.FillInstances( active, selected, selection.Ships, groupHulls: true, interpolation: interpolation, groupMiners: fleet.Frontier );
            int stationStart = active.Length;
            int nextStation = stationStart;
            if ( !fleet.Frontier ) {
                Vector4 relayTint = fleet.Owner == 0 ? new( 0.48f, 1.5f, 1.8f, 1 ) : fleet.Owner == 1
                    ? new( 2.1f, 0.91f, 0.5f, 1 ) : Vector4.One;
                instances[nextStation++] = new( fleet.Site, Vector2.UnitX, relayTint );
            }
            Vector2? yardPixel = null;
            if ( fleet.YardBody >= 0 ) {
                var yard = fleet.Shipyard;
                yardPixel = CommandSelection.Project( yard, viewProjection, new( viewport.Width, viewport.Height ) );
                AddStation( yard, 0.77f, fleet.YardRemaining, FleetSimulation.YardDuration, new( 1.8f, 1.42f, 0.57f, 1 ) );
            }
            if ( fleet.Frontier ) {
                for ( int body = 1; body < fleet.Bodies.Length; ++body ) {
                    var status = fleet.Planet( body );
                    var center = fleet.Bodies[body].Center;
                    float orbitRadius = fleet.OrbitRadius( body, 1 );
                    if ( status.MineRemaining >= 0 )
                        AddStation( center + new Vector2( orbitRadius, 0 ), 0.50f, status.MineRemaining, FleetSimulation.FacilityDuration, new( 1.8f, 1.12f, 0.47f, 1 ) );
                    if ( status.LabRemaining >= 0 )
                        AddStation( center + new Vector2( -orbitRadius, 0 ), 0.46f, status.LabRemaining, FleetSimulation.FacilityDuration, new( 0.57f, 1.3f, 1.8f, 1 ) );
                }
            }
            var upload = uploads[frame.FrameSlotIndex];
            upload.Write<ShipInstance>( instances.AsSpan( 0, nextStation ) );
            FleetDraw.Record( frame.Commands, pipeline, relay, upload, (uint)(nextStation - stationStart), viewProjection, (uint)stationStart );

            void AddStation( Vector2 position, float size, int remaining, int duration, Vector4 tint ) {
                float progress = 1 - Math.Clamp( (float)remaining / duration, 0, 1 );
                instances[nextStation++] = new( position, Vector2.UnitY, remaining > 0 ? new( 0.91f, 0.68f, 0.32f, 1 ) : tint,
                    new( 0, 0, size * (0.38f + 0.62f * progress), remaining > 0 ? -1 : 0 ) );
            }
            if ( !active.IsEmpty ) {
                FleetDraw.RecordThrusters( frame.Commands, thrust, upload, (uint)active.Length, viewProjection, (float)(fleet.Tick * FleetSimulation.StepSeconds) );
                FleetDraw.Record( frame.Commands, pipeline, corvettes, upload, corvetteCount, viewProjection );
                uint miners = fleet.Frontier ? (uint)fleet.ActiveMiners( ) : 0;
                uint scoutsEnd = (uint)active.Length - miners;
                FleetDraw.Record( frame.Commands, pipeline, scouts, upload, scoutsEnd - corvetteCount, viewProjection, corvetteCount );
                FleetDraw.Record( frame.Commands, pipeline, minerMeshes, upload, miners, viewProjection, scoutsEnd );
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
                selection.Count, planet, Environment.TickCount64 < noticeUntil ? notice : null, drag,
                fleet.Frontier ? null : CommandSelection.Project( fleet.Site, viewProjection, new( viewport.Width, viewport.Height ) ),
                selection.Ships, speed, paused, unit, menu, yardPixel, commandTab, dispatch );
        }, ( window, result ) => {
            if ( result.Frames == 1 ) {
                if ( startFullscreen ) display.Toggle( window );
                window.KeyPreview = true;
                window.KeyDown += ( _, e ) => {
                    switch ( e.KeyCode ) {
                        case Keys.F11: display.Toggle( window ); break;
                        case Keys.G: ToggleGalaxy( window ); break;
                        case Keys.Escape when menu is not null: menu = null; break;
                        case Keys.Escape when commandTab != CommandTab.Hidden: commandTab = CommandTab.Hidden; break;
                        case Keys.Tab when !galaxy && fleet.Frontier: Command( CommandAction.Toggle ); break;
                        case Keys.F4 when !galaxy && fleet.Frontier: Command( CommandAction.Events ); break;
                        case Keys.Space when !galaxy: paused = !paused; break;
                        case Keys.Add or Keys.Oemplus when !galaxy: ChangeSpeed( ); break;
                        case Keys.M: audio.Toggle( ); break;
                        case Keys.R when !galaxy:
                            for ( int ship = 0; ship < fleet.Count; ++ship )
                                if ( selection.Ships[ship] && fleet.Alive( ship ) ) fleet.Rejoin( new( fleet.SquadronOf( ship ), ship ) );
                            break;
                        case Keys.A when !galaxy && e.Control: selection.All( fleet ); break;
                        case Keys.E when !galaxy: PlanetOrder( PlanetAction.Survey, planet ); break;
                        case Keys.N when !galaxy: PlanetOrder( PlanetAction.Mine, planet ); break;
                        case Keys.L when !galaxy: PlanetOrder( PlanetAction.Lab, planet ); break;
                        case Keys.T when !galaxy: PlanetOrder( PlanetAction.Research, planet ); break;
                        case Keys.I when !galaxy: PlanetOrder( PlanetAction.Industry, planet ); break;
                        case Keys.B when !galaxy: QueueWing( ); break;
                        default: return;
                    }
                    e.SuppressKeyPress = true;
                };
                window.MouseDown += ( _, e ) => {
                    if ( galaxy ) {
                        if ( e.Button != MouseButtons.Left || viewport.Width == 0 ) return;
                        var mouse = new Vector2( e.X, e.Y );
                        if ( FleetHud.IsGalaxyReturn( mouse, viewport.Width, viewport.Height ) ) {
                            ToggleGalaxy( window );
                            return;
                        }
                        var mapSize = new Vector2( viewport.Width, viewport.Height );
                        var points = CelestialSystem.MapPositions;
                        for ( int i = 0; i < points.Length; ++i ) {
                            var star = points[i] * mapSize;
                            if ( Vector2.DistanceSquared( mouse, star ) > MathF.Pow( MathF.Max( 29, viewport.Height * 0.042f ), 2 ) ) continue;
                            Enter( i );
                            break;
                        }
                        return;
                    }
                    var pointer = new Vector2( e.X, e.Y );
                    if ( menu is { } popup ) {
                        var popupHit = FleetHud.Hit( pointer, viewport.Width, viewport.Height, fleet, planet, popup );
                        menu = null;
                        if ( e.Button == MouseButtons.Left ) {
                            if ( popupHit.Action == HudAction.Context ) PlanetOrder( (PlanetAction)popupHit.Index, popup.Body );
                            return;
                        }
                    }
                    var hit = FleetHud.Hit( pointer, viewport.Width, viewport.Height, fleet, planet, commandTab: commandTab );
                    if ( hit.Action != HudAction.None ) {
                        if ( e.Button == MouseButtons.Left ) {
                            switch ( hit.Action ) {
                                case HudAction.Pause: paused = !paused; break;
                                case HudAction.Speed: ChangeSpeed( ); break;
                                case HudAction.Galaxy: ToggleGalaxy( window ); break;
                                case HudAction.Wing: QueueWing( ); break;
                                case HudAction.Command: Command( (CommandAction)hit.Index ); break;
                                case HudAction.Planet: PlanetOrder( (PlanetAction)hit.Index, planet ); break;
                                case HudAction.Squad when fleet.ShipCount( hit.Index ) > 0:
                                    selection.Squadron( fleet, hit.Index, ModifierSelection( ) );
                                    planet = -1;
                                    if ( e.Clicks >= 2 ) cameraFocus = ClampCamera( fleet.Center( hit.Index ) );
                                    break;
                            }
                        }
                        return;
                    }
                    if ( e.Button == MouseButtons.Middle ) { panStart = e.Location; return; }
                    if ( e.Button == MouseButtons.Left ) {
                        dragStart = dragEnd = e.Location;
                        dragMode = ModifierSelection( );
                        window.Capture = true;
                        return;
                    }
                    if ( e.Button != MouseButtons.Right ) return;
                    var dimensions = new Vector2( viewport.Width, viewport.Height );
                    int enemy = selection.Count == 0 ? -1 : CommandSelection.Hit( fleet, pointer, viewProjection, dimensions, 1, interpolation );
                    if ( enemy >= 0 ) {
                        if ( fleet.Frontier && !HasArmedSelection( ) ) { Notify( "SELECT CORVETTES TO ATTACK" ); return; }
                        selection.Issue( fleet, default, hostile: enemy );
                        Notify( $"{SelectionName( )} ATTACKING SHIP {enemy + 1:D2}" );
                    } else if ( PickBody( e.Location ) is >= 0 and var body ) {
                        menu = new( pointer, body );
                        return;
                    } else {
                        if ( selection.Count == 0 || !TryPick( e.Location, viewport, inverseViewProjection, out var target ) ) return;
                        selection.Issue( fleet, target );
                        Notify( $"{SelectionName( )} MOVING TO WAYPOINT" );
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
                        selection.Box( fleet, DragRect( start, e.Location ), viewProjection, dimensions, dragMode, interpolation );
                        planet = -1;
                        lastClickShip = -1;
                    } else {
                        int ship = CommandSelection.Hit( fleet, new( e.X, e.Y ), viewProjection, dimensions, 0, interpolation );
                        int hitBody = ship < 0 ? PickBody( e.Location ) : -1;
                        if ( hitBody >= 0 ) planet = hitBody;
                        else {
                            long now = Environment.TickCount64;
                            var tolerance = SystemInformation.DoubleClickSize;
                            bool squad = ship >= 0 && dragMode == SelectionMode.Replace && (e.Clicks >= 2 ||
                                lastClickShip == ship && now - lastClickTime <= SystemInformation.DoubleClickTime &&
                                Math.Abs( lastClickPoint.X - e.X ) <= tolerance.Width / 2 &&
                                Math.Abs( lastClickPoint.Y - e.Y ) <= tolerance.Height / 2);
                            selection.Click( fleet, ship, squad, dragMode );
                            lastClickShip = squad || dragMode != SelectionMode.Replace ? -1 : ship;
                            lastClickPoint = e.Location;
                            lastClickTime = now;
                            planet = -1;
                            return;
                        }
                        lastClickShip = -1;
                    }
                };
                window.MouseWheel += ( _, e ) => {
                    if ( galaxy || menu is not null || FleetHud.Hit( new( e.X, e.Y ), viewport.Width, viewport.Height, fleet, planet, commandTab: commandTab ).Action != HudAction.None ) return;
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

        static SelectionMode ModifierSelection( ) {
            var mods = Control.ModifierKeys;
            return (mods & Keys.Control) != 0 ? SelectionMode.Toggle
                : (mods & Keys.Shift) != 0 ? SelectionMode.Add : SelectionMode.Replace;
        }

        void ToggleGalaxy( Form window ) {
            galaxy = !galaxy;
            menu = null;
            panStart = dragStart = null;
            window.Capture = false;
        }

        void ChangeSpeed( ) => speed = speed == 4 ? 1 : speed * 2;

        void QueueWing( ) {
            if ( fleet.QueueWing( ) ) {
                oreShort = false;
                Notify( $"CORVETTE WING QUEUED / {fleet.QueuedWings} IN PRODUCTION" );
                audio.Cue( "order" );
                return;
            }
            oreShort = fleet.Ore < fleet.CurrentWingCost;
            Notify( fleet.Lost ? "CYAN FLEET LOST" : fleet.YardRemaining < 0 ? "BUILD AN ORBITAL SHIPYARD FIRST"
                : fleet.YardRemaining > 0 ? "SHIPYARD UNDER CONSTRUCTION"
                : fleet.WingsBuilt + fleet.QueuedWings >= FleetSimulation.WingGoal ? "WING PRODUCTION LIMIT REACHED"
                : $"NEED {fleet.CurrentWingCost} ORE TO QUEUE A WING" );
        }

        void Enter( int index ) {
            int ships = initial.Frontier ? FleetSimulation.DefaultShips : initial.Capacity - FleetSimulation.WingSize * FleetSimulation.WingGoal;
            fleet = systems[index] ??= new FleetSimulation( ships, unchecked(seed + (uint)index * 1177),
                rivalPatrol: true, system: index, battle: true, frontier: initial.Frontier );
            selection.Squadron( fleet, 0, SelectionMode.Replace );
            planet = fleet.Frontier ? FleetSimulation.Homeworld : -1;
            commandTab = CommandTab.Hidden;
            dispatchesSeen = fleet.DispatchCount;
            dispatch = Math.Max( 0, dispatchesSeen - 1 );
            lastClickShip = -1;
            notice = null;
            shotsHeard = fleet.ShotsFired;
            lossesHeard = fleet.ShipsLost;
            nextCue = 0;
            previousOwner = fleet.Owner;
            surveysHeard = fleet.CompletedSurveys;
            wingsHeard = fleet.WingsBuilt;
            yardState = fleet.YardRemaining;
            menu = null;
            oreShort = galaxy = false;
            zoom = 0.85f;
            cameraFocus = default;
            clock.Reset( );
        }

        bool HasArmedSelection( ) {
            for ( int i = 0; i < fleet.Count; ++i )
                if ( selection.Ships[i] && fleet.Alive( i ) && fleet.Role( fleet.SquadronOf( i ) ) == HullRole.Corvette ) return true;
            return false;
        }

        void Command( CommandAction action ) {
            switch ( action ) {
                case CommandAction.Toggle:
                    commandTab = commandTab == CommandTab.Hidden ? CommandTab.Economy : CommandTab.Hidden;
                    break;
                case CommandAction.Economy: commandTab = CommandTab.Economy; break;
                case CommandAction.Fleet: commandTab = CommandTab.Fleet; break;
                case CommandAction.Events: commandTab = CommandTab.Events; break;
                case CommandAction.Worker:
                    if ( fleet.QueueWorker( ) ) {
                        Notify( "MINER QUEUED AT HOMEWORLD" );
                        audio.Cue( "order" );
                    } else Notify( fleet.WorkerRemaining > 0 ? "MINER ALREADY IN PRODUCTION"
                        : fleet.Ore < FleetSimulation.WorkerCost ? "NEED 20 ORE FOR A MINER" : "MINER PRODUCTION LIMIT REACHED" );
                    break;
                case CommandAction.Wing: QueueWing( ); break;
                case CommandAction.Weapon: fleet.ConfigureDesign( weapon: true ); break;
                case CommandAction.Defense: fleet.ConfigureDesign( weapon: false ); break;
                case CommandAction.Earlier: dispatch = Math.Max( 0, dispatch - 1 ); break;
                case CommandAction.Later: dispatch = Math.Min( fleet.DispatchCount - 1, dispatch + 1 ); break;
            }
        }

        void Notify( string text ) {
            notice = text;
            noticeUntil = Environment.TickCount64 + 4200;
        }

        static string UnitName( FleetSelection unit ) => unit.IsShip ? $"SHIP {unit.Ship + 1:D2}" : $"SQUAD {unit.Squad + 1:D2}";
        string SelectionName( ) => selection.SurveyUnit( fleet ) is { } unit ? UnitName( unit ) : $"{selection.Count} SHIPS";

        void PlanetOrder( PlanetAction action, int body ) {
            if ( action == PlanetAction.Inspect ) { planet = body; return; }
            if ( action == PlanetAction.Orbit ) {
                if ( selection.Count == 0 ) { Notify( "SELECT SHIPS TO ORBIT" ); return; }
                selection.Issue( fleet, default, orbit: body );
                Notify( $"{SelectionName( )} ORBITING {CelestialSystem.Name( fleet.SystemIndex, body )}" );
                audio.Cue( "order" );
                return;
            }
            if ( action == PlanetAction.Research ) {
                Notify( fleet.ResearchWeapons( ) ? "WEAPONS UPGRADE ACTIVE" : "WEAPONS RESEARCH NEEDS 30 SCIENCE" );
                return;
            }
            if ( action == PlanetAction.Industry ) {
                if ( fleet.IndustryUpgrade ) { Notify( "INDUSTRY UPGRADE ACTIVE" ); return; }
                if ( !fleet.ResearchIndustry( ) ) { Notify( "INDUSTRY RESEARCH NEEDS 24 SCIENCE" ); return; }
                Notify( "INDUSTRY UPGRADED / NEW WINGS 42 ORE" );
                audio.Cue( "launch" );
                return;
            }
            if ( body <= 0 ) { Notify( "SELECT A PLANET OR MOON" ); return; }
            planet = body;
            var status = fleet.Planet( body );
            if ( action == PlanetAction.CancelSurvey ) {
                if ( fleet.CancelSurvey( body ) ) {
                    Notify( "SURVEY CANCELED / ORBIT RETAINED" );
                    audio.Cue( "order" );
                }
                return;
            }
            if ( action == PlanetAction.Survey ) {
                if ( status.Surveyed ) { Notify( "SURVEY ALREADY COMPLETE" ); return; }
                if ( status.SurveyUnit is { } assigned ) { Notify( $"{UnitName( assigned )} ALREADY ASSIGNED" ); return; }
                if ( selection.SurveyUnit( fleet ) is not { } unit || !fleet.BeginSurvey( unit, body ) ) {
                    Notify( fleet.Frontier ? "SELECT YOUR SCIENCE SCOUT" : "SELECT ONE SHIP OR ONE SQUAD" );
                    return;
                }
                Notify( $"{UnitName( unit )} SURVEYING {CelestialSystem.Name( fleet.SystemIndex, body )}" );
                audio.Cue( "survey-assign" );
                return;
            }
            if ( action == PlanetAction.Yard ) {
                if ( fleet.BuildYard( body ) ) {
                    Notify( "ORBITAL SHIPYARD UNDER CONSTRUCTION" );
                    audio.Cue( "order" );
                } else Notify( fleet.YardBody >= 0 ? "SYSTEM ALREADY HAS A SHIPYARD" : "SHIPYARD NEEDS A SURVEY AND 55 ORE" );
                return;
            }
            var facility = action == PlanetAction.Mine ? PlanetFacility.Mine : PlanetFacility.Lab;
            var builder = selection.SurveyUnit( fleet );
            bool started = fleet.BuildFacility( body, facility, builder );
            Notify( started ? facility == PlanetFacility.Mine && fleet.Frontier ? "MINER DISPATCHED TO BUILD"
                : facility == PlanetFacility.Mine ? "MINE CONSTRUCTION STARTED" : "LAB CONSTRUCTION STARTED"
                : !status.Surveyed ? "SURVEY THIS BODY BEFORE BUILDING"
                : fleet.Ore < (facility == PlanetFacility.Mine ? FleetSimulation.MineCost : FleetSimulation.LabCost) ? "INSUFFICIENT ORE FOR CONSTRUCTION"
                : facility == PlanetFacility.Mine && fleet.Frontier ? "SELECT AN AVAILABLE MINER AT A DEPOSIT" : "CONSTRUCTION SLOT UNAVAILABLE" );
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
