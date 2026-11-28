using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Assets;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;
using DXSharp.Samples;

namespace FleetBattler;

internal static class Program {
    [DllImport( "user32.dll", EntryPoint = "SendMessageW" )]
    private static extern nint SendWindowMessage( nint window, uint message, nuint buttons, nint coordinate );

    private const uint MouseMoveMessage = 0x0200, LeftDownMessage = 0x0201, LeftUpMessage = 0x0202;
    private const float DefaultTilt = 1.068f;
    private const int StationSlots = 1 + FleetSimulation.FacilityCount * (CelestialSystem.BodyCount - 1);
    private const int EffectSlots = FleetSimulation.EffectCapacity + BattleDraw.RouteCapacity +
        BattleDraw.SolarTrackCapacity + BattleDraw.SiteEffectCapacity + FleetSimulation.FrontierRepairCapacity + 1;

    [STAThread]
    public static int Main( string[] args ) {
        try {
            FleetContent.Configure( args.Where( arg => arg.StartsWith( "--mod=", StringComparison.Ordinal ) ).Select( arg => arg[6..] ) );
            var settings = new GameSettings { Muted = args.Contains( "--mute" ), Fullscreen = args.Contains( "--fullscreen" ) };
            if ( args.Contains( "--validate-content" ) ) {
                _ = ScenarioCatalog.Load( Option( args, "--scenario=", "frontier" ) );
                _ = ResearchCatalog.Current;
                _ = SolarCatalog.Get( 0 );
                GameCatalog.Validate( );
                foreach ( var file in HullModel.Read( args ).Select( model => model.File ).Distinct( ) ) _ = GltfModel.Load( file );
                Console.WriteLine( $"PASS: scenarios, lore, events, {ResearchCatalog.Current.Count} projects and hull models loaded." );
                return 0;
            }
            if ( int.Parse( Option( args, "--benchmark-ticks=", "0" ) ) > 0 ) return Run( args, settings );
            Application.SetUnhandledExceptionMode( UnhandledExceptionMode.ThrowException );
            ApplicationConfiguration.Initialize( );
            bool menu = !args.Any( a => a.StartsWith( "--scenario=" ) || a.StartsWith( "--ships=" ) || a.StartsWith( "--frames=" ) ||
                a is "--skirmish" or "--view-test" or "--resize-test" or "--galaxy" );
            settings.AudioTest = args.Contains( "--audio-test" );
            settings.ShowMenu = menu || args.Contains( "--menu-test" ) || settings.AudioTest;
            settings.MenuTest = args.Contains( "--menu-test" );
            return Run( args, settings );
        } catch ( Exception error ) {
            Console.Error.WriteLine( error );
            return 1;
        }
    }

    private static int Run( string[] args, GameSettings settings ) {
        int ticks = int.Parse( Option( args, "--benchmark-ticks=", "0" ) );
        ArgumentOutOfRangeException.ThrowIfNegative( ticks );
        int ships = int.Parse( Option( args, "--ships=", FleetSimulation.DefaultShips.ToString( ) ) );
        uint seed = uint.Parse( Option( args, "--seed=", "37" ) );
        bool frontier = ticks == 0 && !args.Contains( "--skirmish" ) &&
            !args.Any( arg => arg.StartsWith( "--ships=", StringComparison.Ordinal ) );
        FleetScenario? scenario = frontier ? ScenarioCatalog.Load( Option( args, "--scenario=", "frontier" ) ) : null;
        if ( frontier ) { _ = ResearchCatalog.Current; GameCatalog.Validate( ); }
        var fleet = new FleetSimulation( ships, seed, rivalPatrol: ticks == 0, battle: ticks == 0 && (scenario?.Raids ?? true), frontier: frontier );
        if ( scenario is not null && !settings.ShowMenu ) fleet.StartScenario( scenario );
        if ( ticks > 0 ) {
            Benchmark( fleet, ticks );
            return 0;
        }

        int frameLimit = settings.MenuTest ? 12 : int.Parse( Option( args, "--frames=", "0" ) );
        ArgumentOutOfRangeException.ThrowIfNegative( frameLimit );
        bool resizeTest = args.Contains( "--resize-test" );
        if ( resizeTest && frameLimit is > 0 and < 6 )
            throw new ArgumentException( "Resize qualification requires at least six frames." );
        if ( resizeTest && frameLimit == 0 ) frameLimit = 6;
        bool viewTest = args.Contains( "--view-test" );
        if ( viewTest && (!frontier || frameLimit is > 0 and < 14) )
            throw new ArgumentException( "View qualification requires the Frontier scenario and at least fourteen frames." );
        if ( viewTest ) {
            QualifyCampaign();
            if ( frameLimit == 0 ) frameLimit = 15;
        }
        int trafficCount = frontier ? 0 : int.Parse( Option( args, "--traffic=", "0" ) );
        int debrisCount = int.Parse( Option( args, "--debris=", ComputeTraffic.DefaultDebris.ToString( ) ) );

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
        Console.WriteLine( device.AdapterName );
        Console.WriteLine( $"{fleet.Count:N0} ships, {fleet.Count * FleetDraw.InstanceStride:N0} tactical instance bytes per frame." );
        Console.WriteLine( $"{trafficCount:N0} compute-steered traffic ships and {debrisCount:N0} compute-orbiting debris bodies." );
        Console.WriteLine( "Click or drag to select. Shift adds, Ctrl toggles. Right-click a body for survey and orbit orders. Left-click a body to inspect it." );

        using var compiler = new ShaderCompiler( );
        using var bloom = new SceneBloom( device, compiler );
        using var root = device.CreateTextureRootSignature( FleetDraw.MaterialConstants, linearWrap: true );
        using var pipeline = FleetDraw.CreatePipeline( device, root, compiler, depthEnabled: true );
        using var thrustRoot = device.CreateGraphicsConstantsRootSignature( FleetDraw.ThrusterConstants );
        using var thrust = FleetDraw.CreateThrusters( device, thrustRoot, compiler );
        using var battleRoot = device.CreateGraphicsConstantsRootSignature( BattleDraw.ConstantCount );
        using var effects = BattleDraw.CreateEffects( device, battleRoot, compiler );
        using var markers = BattleDraw.CreateMarkers( device, battleRoot, compiler );
        using var traffic = new ComputeTraffic( device, compiler, trafficCount, debrisCount );
        using var audio = new FleetAudio( settings.Muted );
        using var spaceRoot = device.CreateTextureRootSignature( SpaceDraw.Constants, linearWrap: true );
        using var space = SpaceDraw.CreatePipeline( device, spaceRoot, compiler );
        using var hudRoot = device.CreateTextureRootSignature( 4, linearWrap: true );
        using var hud = FleetHud.CreatePipeline( device, hudRoot, compiler );
        using var first = device.CreateBuffer( (ulong)(Math.Max( fleet.Capacity, 320 ) + StationSlots + 1) * FleetDraw.InstanceStride, BufferMemory.Upload );
        using var second = device.CreateBuffer( (ulong)(Math.Max( fleet.Capacity, 320 ) + StationSlots + 1) * FleetDraw.InstanceStride, BufferMemory.Upload );
        using var firstHud = device.CreateBuffer( FleetHud.BufferBytes, BufferMemory.Upload );
        using var secondHud = device.CreateBuffer( FleetHud.BufferBytes, BufferMemory.Upload );
        using var firstEffects = device.CreateBuffer( EffectSlots * BattleDraw.EffectStride, BufferMemory.Upload );
        using var secondEffects = device.CreateBuffer( EffectSlots * BattleDraw.EffectStride, BufferMemory.Upload );
        using var queue = device.CreateCommandQueue( );
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        using var hullMeshes = new HullMeshes( device, commands, HullModel.Read( args ) );
        using var freighters = LoadModel( "--freighter-model=", "Freighter.gltf", 0.05f );
        using var relay = LoadModel( "--station-model=", "MiningRelay.gltf", 0.12f );
        using var habitats = LoadModel( "--habitat-model=", "ONeillHabitat.gltf", 0.05f );
        using var receiver = LoadModel( "--receiver-model=", "ProximaReceiver.gltf", 0.13f );
        string defaultMoon = Path.Combine( AppContext.BaseDirectory, "Celestial", "Moon.jpg" );
        string? moonPath = args.FirstOrDefault( value => value.StartsWith( "--moon-map=", StringComparison.Ordinal ) )?[11..];
        if ( moonPath is null && File.Exists( defaultMoon ) ) moonPath = defaultMoon;
        using var moon = SpaceDraw.LoadMoon( device, commands, moonPath );
        using var menuArt = FleetHud.LoadMenuArtwork( device, commands );
        using var atlas = FleetHud.LoadAtlas( device, commands );
        using var particles = new SpaceParticles( device, commands, compiler, 2 );
        commands.Close( );
        queue.Wait( queue.Submit( commands ) );

        var result = Present( device, pipeline, thrust, space, moon, moonPath is not null, atlas, menuArt, hud, effects, markers, traffic, audio, bloom, particles, fleet, seed,
            hullMeshes, freighters, relay, habitats, receiver, [first, second], [firstHud, secondHud], [firstEffects, secondEffects],
            frameLimit, resizeTest, viewTest, settings, args.Contains( "--galaxy" ), scenario );
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

    private static void QualifyCampaign() {
        var assembly = new FleetSimulation( FleetSimulation.DefaultShips, 37, frontier: true, battle: false );
        assembly.StartScenario( ScenarioCatalog.Load( "assembly" ) );
        var damaged = new ExpeditionHull( HullRole.Corvette, default, 20, 0, 0, FleetStance.Passive, "SERVICE TEST" );
        int first = assembly.Count;
        assembly.ReceiveTravelHulls( [damaged, damaged], default, survey: false );
        if ( !assembly.OrderService( first, ServiceOrder.Repair ) || !assembly.OrderService( first + 1, ServiceOrder.Repair ) )
            throw new InvalidOperationException( "Repair qualification could not select the precommissioned yard." );
        for ( int tick = 0; tick < 1800 && assembly.CompletedServices < 2; ++tick ) {
            assembly.Step();
            for ( int ship = first; ship <= first + 1; ++ship ) {
                var position = assembly.Positions[ship];
                if ( !float.IsFinite( position.X ) || !float.IsFinite( position.Y ) )
                    throw new InvalidOperationException( "A repair berth produced a non-finite ship position." );
            }
        }
        if ( assembly.CompletedServices != 2 ) throw new InvalidOperationException( "Both corvettes must finish repairs." );

        var frontier = new FleetSimulation( FleetSimulation.DefaultShips, 37, frontier: true, battle: false );
        frontier.StartScenario( ScenarioCatalog.Load( "frontier" ) );
        var campaign = new FleetCampaign( frontier, 37 );
        if ( campaign.TravelBlock( frontier, FleetSimulation.Proxima, [true, false, false], false ) != "RESEARCH PULSED FUSION IN SOL" )
            throw new InvalidOperationException( "The galaxy map bypassed the first-crossing prerequisites." );
        frontier.Step();
        var proxima = campaign.System( FleetSimulation.Proxima );
        var scout = new ExpeditionHull( HullRole.Scout, default, 50, 0, 0, FleetStance.Passive, "CHARTER TEST" );
        int solOre = frontier.Ore;
        if ( !proxima.Receive( new( FleetSimulation.Proxima, frontier.Tick, [scout], new( 45, 20 ) ), frontier.Tick ) ||
             proxima.WorldTick != frontier.WorldTick || proxima.Ore != 45 || frontier.Ore != solOre ||
             proxima.RequiresDrive( CelestialSystem.OuterBelt ) || !frontier.RequiresDrive( CelestialSystem.OuterBelt ) ||
             !proxima.BeginSurvey( new( proxima.FirstPlayerSquad() ), CelestialSystem.OuterBelt ) )
            throw new InvalidOperationException( "The arriving scout must be able to survey Proxima's outer belt without Sol's drive unlock." );
        frontier.PlayerFaction.Unlock( ResearchTopic.Drive );
        if ( !proxima.DriveUpgrade ) throw new InvalidOperationException( "Completed technology must be available to the faction in Proxima." );
        proxima.ReceiveTravelHulls( [new( HullRole.Colony, default, 50, 0, 0, FleetStance.Passive, "COLONY TEST" )], default, survey: false );
        if ( proxima.FindColonyShip( FleetSimulation.Homeworld, [] ) is null )
            throw new InvalidOperationException( "An orbiting colony ship must be offered without selecting a hull." );

        var stagingFleet = new FleetSimulation( FleetSimulation.DefaultShips, 37, frontier: true, battle: false );
        var staging = ScenarioCatalog.Load( "frontier" ) with { StartingSites = [PlanetFacility.Yard], Stores = new( 200, 200 ) };
        stagingFleet.StartScenario( staging );
        stagingFleet.PlayerFaction.Unlock( ResearchTopic.Fusion );
        var crew = new CommandSelection( stagingFleet.Capacity );
        if ( stagingFleet.SelectExpedition( crew ) != 2 || stagingFleet.Depart( crew.Ships, stagingFleet.Tick ) is not { Hulls.Length: 2 } )
            throw new InvalidOperationException( "A commissioned yard, fusion, and ready expedition pair must unlock the first crossing." );
        int favorable = 0;
        for ( int system = CelestialSystem.AuthoredCount; system < CelestialSystem.SystemCount; ++system )
            if ( Exoplanets.Get( system ).Habitability >= 70 ) ++favorable;
        var alienSystem = campaign.System( 126 );
        alienSystem.ReceiveTravelHulls( [new( HullRole.Scout, default, 50, 0, 0, FleetStance.Passive, "CHARTER TEST" )], default, survey: false );
        if ( CelestialSystem.MapPositions.Length != 128 || favorable is < 1 or > 10 || alienSystem.FactionCount( FactionId.Asteri ) != 2 )
            throw new InvalidOperationException( $"Procedural galaxy qualification failed with {favorable} favorable worlds." );
        float earth = SolarCatalog.SpinRadians( 360, 1, SolarCatalog.Get( 1 ).RotationHours );
        float venus = SolarCatalog.SpinRadians( 360, 1, SolarCatalog.Get( 6 ).RotationHours );
        if ( earth is < 1.55f or > 1.65f || venus >= 0 || SolarCatalog.SpinRadians( 360, 1, 0 ) != 0 )
            throw new InvalidOperationException( "Expected sidereal Earth rotation, retrograde Venus, and a fixed surface at zero hours." );
        var route = new GalaxyRoute( 0, FleetSimulation.Proxima, "", "RESEARCH PULSED FUSION", 0 );
        var chart = GalaxyView.Overview.ZoomAt( new( .54f, .48f ), 2 ).Pan( new( .05f, -.04f ) );
        var hulls = new int[CelestialSystem.SystemCount];
        bool picked = false;
        for ( int i = CelestialSystem.AuthoredCount; i < CelestialSystem.SystemCount && !picked; ++i ) {
            var node = FleetHud.GalaxyNodePixel( i, 1280, 800, true, chart );
            picked = FleetHud.HitGalaxy( node, 1280, 800, true, hulls, route, chart: chart ) == new GalaxyHit( GalaxyAction.Star, i );
        }
        if ( !picked ) throw new InvalidOperationException( "Generated star markers must remain selectable after map zoom and pan." );
        Console.WriteLine( $"PASS: Two corvettes repaired, 128 systems charted, {favorable} favorable generated worlds, first contact spawned." );
    }

    private static DesktopRunResult Present( GraphicsDevice device, PipelineState pipeline, PipelineState thrust, PipelineState space, SampledTexture2D moon, bool useMoonMap,
        SampledTexture2D atlas, SampledTexture2D menuArt, PipelineState hudPipeline, PipelineState effectsPipeline, PipelineState markers, ComputeTraffic traffic, FleetAudio audio, SceneBloom bloom, SpaceParticles particles,
        FleetSimulation initial, uint seed, HullMeshes hullMeshes, StaticMeshSet freighters, StaticMeshSet relay, StaticMeshSet habitats, StaticMeshSet? receiver,
        GpuResource[] uploads, GpuResource[] hudUploads, GpuResource[] effectsUploads, int frameLimit, bool resizeTest, bool viewTest, GameSettings settings, bool startGalaxy, FleetScenario? scenario ) {
        var campaign = new FleetCampaign( initial, seed );
        var fleet = initial;
        var instances = new ShipInstance[Math.Max( fleet.Capacity, 320 ) + StationSlots + 1];
        var habitatInstances = new ShipInstance[CelestialSystem.BodyCount * 2];
        var screenBodies = new Vector4[CelestialSystem.BodyCount];
        var screenSites = new MapSite[StationSlots];
        int siteCount = 0;
        Point? mouse = null;
        var hud = new FleetHud( atlas );
        var display = new FullscreenToggle( );
        var selections = new CommandSelection?[CelestialSystem.Systems.Length];
        var selection = selections[0] = new CommandSelection( fleet.Capacity );
        bool galaxyRoster = false;
        int rosterPage = 0;
        selection.Squadron( fleet, fleet.FirstPlayerSquad( ), SelectionMode.Replace );
        var effects = new BattleEffect[EffectSlots];
        int planet = initial.Frontier ? FleetSimulation.Homeworld : -1, shotsHeard = 0, lossesHeard = 0, nextCue = 0;
        var notices = new MinorNotice[4];
        string? hostileAlert = null;
        long hostileAlertUntil = 0;
        string? routeNotice = null;
        int routeTarget = FleetSimulation.Proxima;
        var galaxyView = GalaxyView.Overview;
        var galaxyHulls = new int[CelestialSystem.Systems.Length];
        var systemsVisited = new HashSet<int>();
        bool oreShort = false, galaxy = startGalaxy || viewTest, sawGalaxy = false;
        int slotsUsed = 0, previousOwner = -1;
        float speed = initial.Frontier ? FleetClock.DefaultSpeed : 1;
        int surveysHeard = 0, wingsHeard = 0, yardState = -1, sitesHeard = 0, buildsHeard = 0;
        BodyMenu? menu = null;
        Form? renameEditor = null;
        CommandTab commandTab = CommandTab.Hidden;
        int dispatch = 0, dispatchesSeen = initial.DispatchCount, fleetScroll = 0;
        var designer = new ShipDesigner( );
        var researchDesk = new ResearchDesk();
        bool shipDetails = false;
        int raidersSeen = 0;
        var pendingStories = scenario?.Events.ToList( ) ?? [];
        var briefings = new Queue<(Briefing Text, Action? Accept, Action? Dismiss)>();
        Briefing? CurrentBriefing( ) => briefings.TryPeek( out var next ) ? next.Text : null;
        int yardInspector = -1, launchesSeen = 0, servicesSeen = 0, scansHeard = 0;
        bool buildCatalog = false;
        ResearchBranch researchBranch = ResearchBranch.Industry;
        int yardPage = 0;
        SiteLayer catalogLayer = SiteLayer.Orbit;
        int piracySeen = 0, coloniesSeen = 0;
        var clock = new FleetClock( );
        bool paused = false;
        float zoom = initial.Solar ? 0.38f : 0.85f, tilt = DefaultTilt, yaw = 0;
        Vector2 cameraFocus = initial.Solar ? initial.Bodies[FleetSimulation.Homeworld].Center : default;
        float viewZoom = zoom, viewTilt = tilt, viewYaw = yaw;
        Vector2 viewFocus = cameraFocus;
        int trackedBody = initial.Solar ? FleetSimulation.Homeworld : -1;
        bool tilting = false;
        Point? panStart = null, dragStart = null;
        Point dragEnd = default, lastClickPoint = default;
        int lastClickShip = -1, lastClickBody = -1;
        long lastClickTime = 0;
        SelectionMode dragMode = SelectionMode.Replace;
        float interpolation = 1;
        Matrix4x4 inverseViewProjection = default, viewProjection = default;
        Size viewport = default;
        var work = Screen.PrimaryScreen?.WorkingArea.Size ?? new Size( 1600, 900 );
        var windowSize = new Size( Math.Max( 320, work.Width - 20 ), Math.Max( 240, work.Height - 52 ) );
        using var catalogs = FleetContent.Read( "scenarios", "scenarios" );
        var choices = catalogs.RootElement.GetProperty( "scenarios" ).EnumerateArray( ).Select( row => new ScenarioChoice(
            Id: row.GetProperty( "id" ).GetString( )!, Name: row.GetProperty( "name" ).GetString( )!.ToUpperInvariant( ),
            Description: row.TryGetProperty( "description", out var text ) ? text.GetString( )!.ToUpperInvariant( ) : "EXPLORE SOL AND ESTABLISH AN INTERSTELLAR COLONY." ) )
            .Append( new( "", "FLEET SKIRMISH", "COMMAND A FLEET IN A CONTESTED SYSTEM." ) ).ToArray();
        using var quotes = FleetContent.Read( "quotes", "quotes" );
        var sayings = quotes.RootElement.GetProperty( "quotes" ).EnumerateArray().Select( row =>
            $"{row.GetProperty( "text" ).GetString()}  /  {row.GetProperty( "credit" ).GetString()}".ToUpperInvariant() ).ToArray();
        int quoteIndex = sayings.Length > 0 ? Random.Shared.Next( sayings.Length ) : 0;
        string quote = sayings.Length > 0 ? sayings[quoteIndex] : "";
        long audioTestDeadline = Environment.TickCount64 + 35_000;
        TimeSpan? pausePosition = null;
        int firstScenario = Math.Max( 0, Array.FindIndex( choices, entry => entry.Id == (settings.MenuTest ? "assembly" : "frontier") ) );
        var flow = new GameFlow( choices, firstScenario, settings.ShowMenu );
        var archive = new ResearchDesk();
        var menuLabels = new string[8];
        string factionName = initial.PlayerFaction.Name;

        string ActionLabel( MenuAction action ) => action switch {
            MenuAction.Launch => flow.Scenario.Skirmish ? "LAUNCH SKIRMISH" : "LAUNCH CAMPAIGN",
            MenuAction.NextScenario => "CHANGE SCENARIO  >",
            MenuAction.Faction => $"FACTION / {factionName}",
            MenuAction.ResearchArchive => "TECHNOLOGY ARCHIVE",
            MenuAction.Resume => "RESUME FLIGHT",
            MenuAction.ReturnToTitle => "RETURN TO TITLE",
            MenuAction.Sound => audio.Muted ? "SOUND  OFF" : "SOUND  ON",
            MenuAction.Display => display.Active ? "DISPLAY  FULLSCREEN" : "DISPLAY  WINDOWED",
            MenuAction.Exit => "EXIT GAME",
            MenuAction.Music => $"[-]   MUSIC {audio.MusicPercent}%   [+]",
            _ => throw new ArgumentOutOfRangeException( nameof(action) )
        };

        void Activate( Form window, MenuAction action, int volumeStep = 5 ) {
            switch ( action ) {
                case MenuAction.Music: audio.AdjustMusic( volumeStep ); return;
                case MenuAction.Faction: RenameFaction( window ); return;
                case MenuAction.Sound: audio.Toggle(); return;
                case MenuAction.Display: display.Toggle( window ); return;
                case MenuAction.Exit: window.Close(); return;
                default: flow.Navigate( action ); break;
            }
            if ( action == MenuAction.Launch && sayings.Length > 1 ) {
                quoteIndex = (quoteIndex + 1) % sayings.Length;
                quote = sayings[quoteIndex];
            }
        }

        void StartCampaign() {
            var choice = flow.Scenario;
            scenario = choice.Skirmish ? null : ScenarioCatalog.Load( choice.Id );
            initial = new FleetSimulation( FleetSimulation.DefaultShips, seed, rivalPatrol: true,
                battle: scenario?.Raids ?? true, frontier: !choice.Skirmish );
            if ( scenario is not null ) initial.StartScenario( scenario );
            initial.PlayerFaction.Name = factionName;
            campaign = new FleetCampaign( initial, seed );
            traffic.Reset();
            Array.Clear( selections );
            Array.Clear( notices );
            briefings.Clear();
            pendingStories = scenario?.Events.ToList() ?? [];
            designer = new ShipDesigner();
            researchDesk = new ResearchDesk();
            clock.Reset();
            paused = false;
            speed = initial.Frontier ? FleetClock.DefaultSpeed : 1;
            galaxyView = GalaxyView.Overview;
            routeTarget = FleetSimulation.Proxima;
            routeNotice = null;
            rosterPage = yardPage = 0;
            Enter( 0 );
            designer.Open( fleet, HullBlueprint.Corvette );
            flow.FinishLoading();
        }

        var run = DesktopGame.Run( device, new( ) {
            Title = "DXSharp · Fleet Battler", EnableDepth = true, FramesInFlight = uploads.Length, FrameLimit = frameLimit,
            ClientSize = viewTest ? new Size( 780, 490 ) : windowSize, ClearColor = new( 0.017f, 0.025f, 0.048f, 1 )
        }, frame => {
            viewport = frame.Size;
            slotsUsed |= 1 << frame.FrameSlotIndex;
            var hudUpload = hudUploads[frame.FrameSlotIndex];
            audio.SetSituation( fleet.SystemIndex, galaxy, fleet.MusicContact(), fleet.Won,
                title: flow.IsTitle );
            if ( flow.Screen == GameScreen.ResearchArchive ) {
                hud.ResearchStation( archive, null, viewport.Width, viewport.Height, frame.Commands, hudPipeline, hudUpload );
                return;
            }
            if ( flow.Screen != GameScreen.Flight ) {
                bool pause = flow.Screen == GameScreen.PauseMenu;
                var actions = flow.Actions;
                var labels = menuLabels.AsSpan( 0, actions.Length );
                for ( int row = 0; row < labels.Length; ++row ) labels[row] = ActionLabel( actions[row] );
                hud.DrawMenu( frame.Commands, hudPipeline, hudUpload, menuArt, viewport.Width, viewport.Height, flow.Screen,
                    pause ? scenario?.Name ?? "SKIRMISH" : flow.Scenario.Name,
                    pause ? "RETURN TO FLIGHT OR ADJUST DISPLAY AND SOUND." : flow.Scenario.Description,
                    labels, flow.Focus, quote );
                if ( flow.Screen == GameScreen.Loading ) StartCampaign();
                return;
            }
            float elapsed = clock.Advance( initial.Frontier ? initial : fleet, frame.Elapsed.TotalSeconds, speed, paused || briefings.Count > 0, campaign.Advance );
            QueueCampaignBriefings( );
            interpolation = clock.Interpolation;
            if ( !galaxy ) systemsVisited.Add( fleet.SystemIndex );
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
            if ( scansHeard != fleet.StartedScans ) {
                scansHeard = fleet.StartedScans;
                audio.Cue( "scan" );
            }
            if ( servicesSeen != fleet.CompletedServices ) {
                servicesSeen = fleet.CompletedServices;
                Notify( "SHIPYARD SERVICE COMPLETE" );
                audio.Cue( "launch" );
            }
            if ( surveysHeard != fleet.CompletedSurveys ) {
                surveysHeard = fleet.CompletedSurveys;
                Notify( $"{UnitName( fleet.LastSurveyUnit )} SURVEYED {fleet.BodyName( fleet.LastSurveyBody )}" );
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
            if ( buildsHeard != fleet.StartedConstruction ) {
                buildsHeard = fleet.StartedConstruction;
                Notify( $"CONSTRUCTOR ON SITE / ASSEMBLING {FleetHud.SiteName( fleet.LastStartedSite.Facility )}" );
                audio.Cue( "order" );
            }
            if ( sitesHeard != fleet.CompletedSites ) {
                sitesHeard = fleet.CompletedSites;
                var site = fleet.LastCompletedSite;
                Notify( $"{FleetHud.SiteName( site.Facility )} ONLINE / {fleet.BodyName( site.Body )}" );
                audio.Cue( "launch" );
            }
            if ( launchesSeen != fleet.ShipsLaunched ) {
                launchesSeen = fleet.ShipsLaunched;
                var launch = fleet.LastLaunch;
                Notify( $"{fleet.Hull( launch.Hull ).Name} LAUNCHED / {fleet.BodyName( launch.Body )}" );
                audio.Cue( "launch" );
            }
            if ( dispatchesSeen < fleet.DispatchCount ) {
                bool hostile = false;
                while ( dispatchesSeen < fleet.DispatchCount ) {
                    var received = fleet.Dispatch( dispatchesSeen++ );
                    if ( received is not (FrontierEvent.Raid or FrontierEvent.Counterattack or FrontierEvent.Interdiction) ) continue;
                    hostile = true;
                    hostileAlert = FleetHud.IncidentTitle( received );
                    hostileAlertUntil = Environment.TickCount64 + 9_000;
                }
                dispatch = dispatchesSeen - 1;
                var incident = fleet.Dispatch( dispatch );
                Notify( FleetHud.IncidentTitle( incident ) );
                audio.Cue( hostile ? "hostile" : incident == FrontierEvent.MerchantOffer ? "dispatch" : "order" );
            }
            if ( fleet.ActiveRaids > raidersSeen ) {
                raidersSeen = fleet.ActiveRaids;
                if ( raidersSeen > 3 ) {
                    hostileAlert = $"{Factions.Name( FactionId.Raiders )} RETURN";
                    hostileAlertUntil = Environment.TickCount64 + 9_000;
                    audio.Cue( "hostile" );
                }
            }
            if ( piracySeen != fleet.PiracyCycles ) {
                if ( piracySeen == 0 || piracySeen / 4 != fleet.PiracyCycles / 4 ) audio.Cue( "piracy" );
                piracySeen = fleet.PiracyCycles;
                Notify( $"PIRATES INTERCEPTED CARGO / {fleet.BodyName( fleet.LastPiracyBody )}" );
            }
            if ( coloniesSeen != fleet.ColoniesFounded ) {
                coloniesSeen = fleet.ColoniesFounded;
                Notify( $"COLONY FOUNDED / {fleet.BodyName( fleet.LastFoundedBody )}" );
                audio.Cue( "launch" );
            }
            if ( galaxy ) {
                sawGalaxy = true;
                SpaceDraw.RecordGalaxy( frame.Commands, space, moon, viewport.Width, viewport.Height, fleet.Frontier, galaxyView );
                UpdateGalaxyHulls( );
                selection.Prune( fleet );
                var route = RouteView();
                var hoverHit = mouse is { } chartCursor && CurrentBriefing() is null ? FleetHud.HitGalaxy( new( chartCursor.X, chartCursor.Y ),
                    viewport.Width, viewport.Height, fleet.Frontier, galaxyHulls, route, galaxyRoster ? fleet : null, rosterPage, galaxyView ) : default;
                hud.RecordGalaxy( frame.Commands, hudPipeline, hudUpload, fleet.SystemIndex, viewport.Width, viewport.Height,
                    fleet.Frontier, initial.AstrometryUpgrade, galaxyHulls, route, initial.Frontier ? initial.Tick : fleet.Tick, speed, paused, CurrentBriefing( ), galaxyRoster ? fleet : null, selection, rosterPage, campaign.Transfers, notices, Environment.TickCount64, campaign, galaxyView, hoverHit.Action == GalaxyAction.Star ? hoverHit.System : -1 );
                return;
            }

            selection.Prune( fleet );
            var unit = selection.SurveyUnit( fleet );
            var selected = unit ?? selection.Focus( fleet );
            float radius = fleet.BoardRadius;
            float aspect = (float)viewport.Width / viewport.Height;
            float blend = 1 - MathF.Exp( -12 * (float)Math.Clamp( frame.Elapsed.TotalSeconds, 0, .1 ) );
            if ( trackedBody >= 0 ) {
                var center = fleet.VisualBody( trackedBody, interpolation ).Center;
                viewFocus += center - cameraFocus;
                cameraFocus = center;
            }
            viewFocus = Vector2.Lerp( viewFocus, cameraFocus, blend );
            viewZoom = float.Lerp( viewZoom, zoom, blend );
            viewTilt = float.Lerp( viewTilt, tilt, blend );
            viewYaw += MathF.IEEERemainder( yaw - viewYaw, MathF.Tau ) * blend;
            float halfHeight = radius * viewZoom / MathF.Min( aspect, 1 );
            var cameraTarget = new Vector3( viewFocus.X, 0, viewFocus.Y );
            var (elevation, horizontal) = MathF.SinCos( viewTilt );
            var (side, forward) = MathF.SinCos( viewYaw );
            var view = Matrix4x4.CreateLookAt( cameraTarget + 1.83f * radius * new Vector3( side * horizontal, elevation, forward * horizontal ),
                cameraTarget, Vector3.UnitY );
            var projection = Matrix4x4.CreateOrthographic( 2 * halfHeight * aspect, 2 * halfHeight, 0.1f, 5 * radius );
            viewProjection = view * projection;
            Matrix4x4.Invert( viewProjection, out inverseViewProjection );
            for ( int i = 0; i < fleet.Bodies.Length; ++i )
                screenBodies[i] = CelestialSystem.Project( fleet.VisualBody( i, interpolation ), radius, viewProjection, viewport.Width, viewport.Height, halfHeight, fleet.Frontier ? CelestialSystem.FrontierBodyDisplayScale : 1 );

            float visualTime = ComputeTraffic.RenderTime( fleet.WorldTick, interpolation );
            particles.Begin( screenBodies[0], viewport, visualTime );
            var stellarCenter = fleet.VisualBody( 0, interpolation ).Center;
            bool computeActive = traffic.ShipCount != 0 || traffic.DebrisCount != 0;
            if ( computeActive ) traffic.RecordCompute( frame.Commands, fleet, elapsed, interpolation, frame.FrameSlotIndex );
            SpaceDraw.Record( frame.Commands, space, moon, useMoonMap, fleet, selected, viewProjection, inverseViewProjection,
                viewport.Width, viewport.Height, halfHeight, showSelection: unit is not null, interpolation: interpolation );
            if ( computeActive ) {
                traffic.RecordDebris( frame.Commands, viewProjection, viewport.Width, viewport.Height, halfHeight );
                if ( !fleet.Frontier && fleet.CanSeeContacts )
                    traffic.RecordFreighters( frame.Commands, pipeline, freighters, viewProjection, viewport.Height / (2 * halfHeight), stellarCenter );
                traffic.Finish( frame.Commands );
            }

            var active = instances.AsSpan( 0, fleet.CanSeeContacts ? fleet.ActiveCount : 0 );
            Span<int> roleCounts = stackalloc int[hullMeshes.RoleCount];
            if ( !active.IsEmpty ) fleet.FillInstances( active, selected, selection.Ships, interpolation: interpolation, roleCounts: roleCounts );
            siteCount = 0;
            int stationStart = active.Length;
            int nextStation = stationStart, habitatCount = 0;
            ShipInstance? receiverInstance = null;
            if ( !fleet.Frontier ) {
                Vector4 relayTint = fleet.Owner == 0 ? new( 0.48f, 1.5f, 1.8f, 1 ) : fleet.Owner == 1
                    ? new( 2.1f, 0.91f, 0.5f, 1 ) : Vector4.One;
                instances[nextStation++] = new( fleet.Site, Vector2.UnitX, relayTint );
            }
            Vector2? yardPixel = null;
            for ( int body = 1; body < fleet.Bodies.Length; ++body ) {
                if ( !fleet.Frontier ) {
                    if ( fleet.YardAt( body ) < 0 ) continue;
                    var yard = fleet.SitePosition( body, PlanetFacility.Yard, interpolation );
                    yardPixel = CommandSelection.Project( yard, viewProjection, new( viewport.Width, viewport.Height ) );
                    AddStation( yard, 0.77f, fleet.YardAt( body ), FleetSimulation.YardDuration, new( 1.8f, 1.42f, 0.57f, 1 ) );
                    continue;
                }
                foreach ( var facility in FleetSimulation.Facilities ) RecordSite( body, facility );
            }

            void RecordSite( int body, PlanetFacility facility ) {
                int remaining = fleet.SiteRemaining( body, facility );
                bool yard = facility == PlanetFacility.Yard && fleet.CanOpenYard( body );
                if ( FleetSimulation.Facility( facility ).Layer == SiteLayer.Surface || remaining < 0 && !yard ) return;
                var position = fleet.SitePosition( body, facility, interpolation );
                var pixel = CommandSelection.Project( position, viewProjection, new( viewport.Width, viewport.Height ) );
                int duration = fleet.ConstructionDuration( facility );
                int shipRemaining = yard ? fleet.ShipRemaining( body ) : 0;
                bool producing = shipRemaining > 0;
                screenSites[siteCount++] = new( pixel, body, facility, producing ? shipRemaining : remaining,
                    producing ? fleet.YardQueue( body )[0].Duration : duration, producing );
                var (size, tint) = facility switch {
                    PlanetFacility.Yard => (0.77f, new Vector4( 1.8f, 1.42f, 0.57f, 1 )),
                    PlanetFacility.Mine or PlanetFacility.Skimmer => (0.50f, new Vector4( 1.8f, 1.12f, 0.47f, 1 )),
                    PlanetFacility.Lab or PlanetFacility.Observatory => (0.46f, new Vector4( 0.57f, 1.3f, 1.8f, 1 )),
                    PlanetFacility.Solar => (0.52f, new Vector4( 1.25f, 1.54f, 2f, 1 )),
                    _ => (0.55f, new Vector4( 0.63f, 1.9f, 1.35f, 1 ))
                };
                if ( !fleet.SiteEnRoute( body, facility ) || yard ) {
                    if ( receiver is not null && fleet.SystemIndex == FleetSimulation.Proxima &&
                         body == FleetSimulation.Homeworld && facility == PlanetFacility.Lab )
                        receiverInstance = Station( position, 1, remaining, duration, Vector4.One );
                    else if ( facility == PlanetFacility.Soletta )
                        habitatInstances[habitatCount++] = Station( position, 2.72f, remaining, duration, new( 1.18f, .92f, .62f, 1 ) );
                    else if ( facility == PlanetFacility.Habitat )
                        habitatInstances[habitatCount++] = Station( position, 1.18f, remaining, duration, new( 1.15f, 1.37f, 1.85f, 1 ) );
                    else AddStation( position, size, Math.Max( 0, remaining ), duration, tint );
                }
            }
            var upload = uploads[frame.FrameSlotIndex];
            habitatInstances.AsSpan( 0, habitatCount ).CopyTo( instances.AsSpan( nextStation ) );
            int receiverStart = nextStation + habitatCount;
            if ( receiverInstance is { } station ) instances[receiverStart] = station;
            upload.Write<ShipInstance>( instances.AsSpan( 0, receiverStart + (receiverInstance.HasValue ? 1 : 0) ) );
            FleetDraw.Record( frame.Commands, pipeline, relay, upload, (uint)(nextStation - stationStart), viewProjection, (uint)stationStart, stellarCenter );
            FleetDraw.Record( frame.Commands, pipeline, habitats, upload, (uint)habitatCount, viewProjection, (uint)nextStation, stellarCenter );
            if ( receiverInstance.HasValue )
                FleetDraw.Record( frame.Commands, pipeline, receiver!, upload, 1, viewProjection, (uint)receiverStart, stellarCenter );

            void AddStation( Vector2 position, float size, int remaining, int duration, Vector4 tint ) =>
                instances[nextStation++] = Station( position, size, remaining, duration, tint );

            static ShipInstance Station( Vector2 position, float size, int remaining, int duration, Vector4 tint ) {
                float progress = 1 - Math.Clamp( (float)remaining / duration, 0, 1 );
                return new( position, Vector2.UnitY, remaining > 0 ? new( 0.91f, 0.68f, 0.32f, 1 ) : tint,
                    new( 0, 0, size * (0.38f + 0.62f * progress), remaining > 0 ? -1 : 0 ) );
            }
            if ( !active.IsEmpty ) {
                uint firstInstance = 0;
                for ( int role = 0; role < roleCounts.Length; ++role ) {
                    uint count = (uint)roleCounts[role];
                    if ( count == 0 ) continue;
                    FleetDraw.Record( frame.Commands, pipeline, hullMeshes[role], upload, count, viewProjection, firstInstance, stellarCenter );
                    FleetDraw.RecordThrusters( frame.Commands, thrust, upload, count, viewProjection,
                        (float)(fleet.Tick * FleetSimulation.StepSeconds), firstInstance, hullMeshes.Emitters( role ) );
                    particles.Exhaust( active.Slice( (int)firstInstance, (int)count ), hullMeshes.Emitters( role ), viewProjection,
                        viewport, viewport.Height / (2 * halfHeight), visualTime );
                    firstInstance += count;
                }
            }
            int pulseCount = fleet.FillEffects( effects.AsSpan( 0, FleetSimulation.EffectCapacity ) );
            if ( fleet.Frontier && selection.Primary >= 0 ) {
                int primary = selection.Primary;
                var navigator = unit ?? new FleetSelection( fleet.SquadronOf( primary ), primary );
                pulseCount += BattleDraw.FillRoute( fleet, navigator, effects.AsSpan( pulseCount ), interpolation );
            }
            if ( fleet.Frontier ) {
                float workTime = (fleet.Tick - 1 + interpolation) * (float)FleetSimulation.StepSeconds;
                for ( int body = 1; body < fleet.Bodies.Length; ++body ) {
                    foreach ( var facility in FleetSimulation.Facilities )
                        if ( fleet.SiteWorker( body, facility ) is { } worker && !fleet.SiteEnRoute( body, facility ) )
                            WorkBeam( fleet.VisualCenter( worker, interpolation ), fleet.SitePosition( body, facility, interpolation ), 0 );
                    if ( fleet.ShipRemaining( body ) > 0 ) {
                        var yard = fleet.SitePosition( body, PlanetFacility.Yard, interpolation );
                        var (sine, cosine) = MathF.SinCos( workTime * 2 );
                        WorkBeam( yard, yard + new Vector2( cosine, sine ) * 3, 0 );
                    }
                    if ( fleet.Planet( body ).SurveyUnit is { } scout && fleet.SurveyWorking( body ) ) {
                        var from = fleet.VisualCenter( scout, interpolation );
                        var target = fleet.VisualBody( body, interpolation );
                        var delta = target.Center - from;
                        var sweepAxis = delta.LengthSquared( ) < 1e-6f ? Vector2.UnitX : Vector2.Normalize( new Vector2( -delta.Y, delta.X ) );
                        float sweep = MathF.Sin( workTime * 3.2f ) * target.Radius * 0.63f;
                        WorkBeam( from, target.Center + sweepAxis * (sweep - target.Radius * 0.23f), 0.85f );
                        WorkBeam( from, target.Center + sweepAxis * (sweep + target.Radius * 0.23f), 1 );
                    }
                }

                void WorkBeam( Vector2 from, Vector2 to, float scanner ) =>
                    effects[pulseCount++] = new( new( from.X, from.Y, to.X, to.Y ), new( workTime, 0, 6, scanner ) );
            }
            if ( fleet.ReceiverOnline && receiverInstance is { } transmitter )
                effects[pulseCount++] = BattleDraw.ReceiverSignal( transmitter.Center, fleet.Tick, interpolation );
            if ( fleet.Frontier ) pulseCount += BattleDraw.FillRepairs( fleet, effects.AsSpan( pulseCount ), interpolation );
            pulseCount += BattleDraw.FillSolarTracks( fleet, effects.AsSpan( pulseCount ), interpolation );
            if ( pulseCount != 0 ) {
                var pulses = effectsUploads[frame.FrameSlotIndex];
                pulses.Write<BattleEffect>( effects.AsSpan( 0, pulseCount ) );
                BattleDraw.Record( frame.Commands, effectsPipeline, pulses, (uint)pulseCount, viewProjection,
                    viewport.Width, viewport.Height, halfHeight, effects: true );
            }
            particles.Explosions( effects.AsSpan( 0, pulseCount ), viewProjection, viewport );
            particles.Draw( frame );
            var drag = dragStart is { } start && IsDrag( start, dragEnd ) ? DragRect( start, dragEnd ) : (Vector4?)null;
            MapHover hover = default;
            if ( fleet.Frontier && menu is null && panStart is null && dragStart is null && mouse is { } pointer &&
                 FleetHud.Hit( new( pointer.X, pointer.Y ), viewport.Width, viewport.Height, fleet, planet, commandTab: commandTab, yardInspector: yardInspector, buildCatalog: buildCatalog, hasSelection: selection.Count != 0, dispatch: dispatch, researchBranch: researchBranch, catalogLayer: catalogLayer, fleetScroll: fleetScroll, briefing: CurrentBriefing( ), shipDetails: shipDetails, yardPage: yardPage ).Action == HudAction.None ) {
                var pixel = new Vector2( pointer.X, pointer.Y );
                var dimensions = new Vector2( viewport.Width, viewport.Height );
                int ship = CommandSelection.Hit( fleet, pixel, viewProjection, dimensions, -1, interpolation );
                if ( ship >= 0 )
                    hover = new( HoverKind.Ship, ship, CommandSelection.Project( fleet.VisualPosition( ship, interpolation ), viewProjection, dimensions ), pixel );
                else if ( PickSite( pixel ) is >= 0 and var site )
                    hover = new( HoverKind.Site, site, screenSites[site].Pixel, pixel );
                else if ( PickBody( pointer ) is >= 0 and var body )
                    hover = new( HoverKind.Body, body, new( screenBodies[body].X, screenBodies[body].Y ), pixel );
            }
            Span<Vector2> kuiperLabels = stackalloc Vector2[8];
            if ( fleet.Solar ) {
                float axis = SolarCatalog.Axis( (SolarCatalog.KuiperInnerAu + SolarCatalog.KuiperOuterAu) * 0.5f ) * radius;
                for ( int i = 0; i < kuiperLabels.Length; ++i ) {
                    var (sine, cosine) = MathF.SinCos( i * MathF.Tau / kuiperLabels.Length );
                    kuiperLabels[i] = CommandSelection.Project( new Vector2( cosine, sine ) * axis,
                        viewProjection, new( viewport.Width, viewport.Height ) );
                }
            }
            if ( commandTab == CommandTab.Research && CurrentBriefing() is null && !galaxy ) {
                hud.ResearchStation( researchDesk, fleet, viewport.Width, viewport.Height, frame.Commands, hudPipeline, hudUpload );
                return;
            }
            bloom.Apply( frame, screenBodies[0] );
            if ( !active.IsEmpty )
                BattleDraw.Record( frame.Commands, markers, upload, (uint)active.Length, viewProjection, viewport.Width, viewport.Height, halfHeight, effects: false );
            hud.Record( frame.Commands, hudPipeline, hudUpload, fleet, selected, viewport.Width, viewport.Height, oreShort, screenBodies.AsSpan( 0, fleet.Bodies.Length ),
                selection.Count, planet, null, drag,
                fleet.Frontier ? null : CommandSelection.Project( fleet.Site, viewProjection, new( viewport.Width, viewport.Height ) ),
                selection.Ships, speed, paused, unit, menu, yardPixel, commandTab, dispatch, screenSites.AsSpan( 0, siteCount ), hover, yardInspector, buildCatalog, researchBranch, catalogLayer,
                Environment.TickCount64 < hostileAlertUntil ? hostileAlert : null, fleetScroll, designer, CurrentBriefing( ), fleet.Solar ? kuiperLabels : default, shipDetails, initial.Frontier ? initial.Tick : fleet.Tick, yardPage,
                mouse is { } cursor ? new Vector2( cursor.X, cursor.Y ) : null, notices, Environment.TickCount64, interpolation: interpolation );
            if ( commandTab == CommandTab.Fleet && CurrentBriefing() is null && menu is null && !designer.Dragging ) {
                var box = FleetHud.DesignerPreview( viewport.Width, viewport.Height );
                if ( box.W > 0 ) {
                    int role = (int)FleetSimulation.BlueprintRole( designer.Hull );
                    var (center, extent) = hullMeshes.Bounds( role );
                    float angle = (Environment.TickCount64 % 60000) * (MathF.Tau / 60000);
                    var eye = center + extent * new Vector3( 3 * MathF.Sin( angle ), 1.4f, 3 * MathF.Cos( angle ) );
                    float size = 2.1f * extent;
                    var camera = Matrix4x4.CreateLookAt( eye, center, Vector3.UnitY ) *
                        Matrix4x4.CreateOrthographic( size * Math.Max( 1, box.Z / box.W ), size * Math.Max( 1, box.W / box.Z ), extent, 6 * extent );
                    var placement = Matrix4x4.CreateScale( box.Z / viewport.Width, box.W / viewport.Height, 0.00001f ) *
                        Matrix4x4.CreateTranslation( (2*box.X + box.Z)/viewport.Width - 1, 1 - (2*box.Y + box.W)/viewport.Height, 0 );
                    uint slot = (uint)(instances.Length - 1);
                    upload.Write<ShipInstance>( [new( Vector2.Zero, -Vector2.UnitY, Vector4.One, new( 0, 0, 1, 1 ) )],
                        (ulong)slot * FleetDraw.InstanceStride );
                    FleetDraw.Record( frame.Commands, pipeline, hullMeshes[role], upload, 1, camera * placement, slot );
                }
            }
        }, ( window, result ) => {
            if ( result.Frames == 1 ) {
                if ( settings.Fullscreen ) display.Toggle( window );
                window.KeyPreview = true;
                window.KeyDown += ( _, e ) => {
                    if ( flow.Screen != GameScreen.Flight ) {
                        e.SuppressKeyPress = true;
                        if ( e.KeyCode == Keys.Escape ) { flow.Back(); return; }
                        var actions = flow.Actions;
                        if ( actions.IsEmpty ) return;
                        if ( e.KeyCode == Keys.Down ) flow.MoveFocus( 1 );
                        if ( e.KeyCode == Keys.Up ) flow.MoveFocus( -1 );
                        if ( e.KeyCode == Keys.Enter ) Activate( window, actions[flow.Focus] );
                        if ( actions[flow.Focus] == MenuAction.Music && e.KeyCode is Keys.Left or Keys.Right )
                            audio.AdjustMusic( e.KeyCode == Keys.Left ? -5 : 5 );
                        return;
                    }
                    if ( e.KeyCode == Keys.Escape ) {
                        e.SuppressKeyPress = true;
                        flow.Back();
                        dragStart = panStart = null;
                        designer.Release( default );
                        window.Capture = false;
                        return;
                    }
                    if ( briefings.Count > 0 ) {
                        if ( e.KeyCode == Keys.Enter ) ResolveBriefing( accept: true );
                        e.SuppressKeyPress = true;
                        return;
                    }
                    switch ( e.KeyCode ) {
                        case Keys.F11: display.Toggle( window ); break;
                        case Keys.G: ToggleGalaxy( window ); break;
                        case Keys.Home when galaxy: galaxyView = GalaxyView.Overview; break;
                        case Keys.Home when !galaxy: ResetCamera( ); break;
                        case Keys.End when !galaxy:
                            TrackBody( 0 );
                            zoom = OverviewZoom();
                            break;
                        case Keys.F2 when e.Control && e.Shift: RenameFaction( window ); break;
                        case Keys.F2 when !galaxy && fleet.Frontier && !fleet.ChartOnly: Rename( window, e.Control ); break;
                        case Keys.F when !galaxy:
                            if ( planet >= 0 ) TrackBody( planet );
                            else if ( selection.SurveyUnit( fleet ) is { } focusUnit ) SetCameraFocus( fleet.Center( focusUnit ) );
                            break;
                        case Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.W or Keys.A or Keys.S or Keys.D when !galaxy && !e.Control:
                            var motion = e.KeyCode switch {
                                Keys.Left or Keys.A => new Vector2( -1, 0 ), Keys.Right or Keys.D => new Vector2( 1, 0 ),
                                Keys.Up or Keys.W => new Vector2( 0, -1 ), _ => new Vector2( 0, 1 )
                            };
                            var (sine, cosine) = MathF.SinCos( yaw );
                            SetCameraFocus( cameraFocus + new Vector2( cosine * motion.X + sine * motion.Y,
                                -sine * motion.X + cosine * motion.Y ) * (0.07f * zoom * fleet.BoardRadius) );
                            break;
                        case Keys.Tab when !galaxy && !fleet.ChartOnly && fleet.Frontier: Command( CommandAction.Toggle ); break;
                        case Keys.F4 when !galaxy && !fleet.ChartOnly && fleet.Frontier: Command( CommandAction.Events ); break;
                        case Keys.Space: paused = !paused; break;
                        case Keys.Add or Keys.Oemplus: ChangeSpeed( increase: true ); break;
                        case Keys.Subtract or Keys.OemMinus: ChangeSpeed( increase: false ); break;
                        case Keys.M: audio.Toggle( ); break;
                        case Keys.R when !galaxy:
                            for ( int ship = 0; ship < fleet.Count; ++ship )
                                if ( selection.Ships[ship] && fleet.Alive( ship ) ) fleet.Rejoin( new( fleet.SquadronOf( ship ), ship ) );
                            break;
                        case Keys.A when !galaxy && e.Control: selection.All( fleet ); break;
                        case Keys.E when !galaxy && !fleet.ChartOnly: PlanetOrder( PlanetAction.Survey, planet ); break;
                        case Keys.N when !galaxy && !fleet.ChartOnly: PlanetOrder( PlanetAction.Mine, planet ); break;
                        case Keys.L when !galaxy && !fleet.ChartOnly: PlanetOrder( PlanetAction.Lab, planet ); break;
                        case Keys.T when !galaxy && !fleet.ChartOnly: PlanetOrder( PlanetAction.Research, planet ); break;
                        case Keys.I when !galaxy && !fleet.ChartOnly: PlanetOrder( PlanetAction.Industry, planet ); break;
                        case Keys.B when !galaxy && !fleet.ChartOnly && fleet.Frontier:
                            if ( fleet.CanOpenYard( planet ) ) OpenYard( planet );
                            else if ( fleet.CanOpenYard( fleet.YardBody ) ) OpenYard( fleet.YardBody );
                            else if ( fleet.Established ) OpenYard( FleetSimulation.Homeworld );
                            break;
                        case Keys.B when !galaxy && !fleet.ChartOnly: QueueWing( ); break;
                        case Keys.Q when !galaxy && fleet.Frontier: CycleStance( ); break;
                        case Keys.Y when !galaxy && fleet.Frontier: Service( ServiceOrder.Repair ); break;
                        case Keys.U when !galaxy && fleet.Frontier: Service( ServiceOrder.Refit ); break;
                        case Keys.V when !galaxy && fleet.Frontier: Service( ServiceOrder.Retreat ); break;
                        case Keys.J when !galaxy && fleet.Frontier: OrganizeFleets( e.Shift ); break;
                        default: return;
                    }
                    e.SuppressKeyPress = true;
                };
                window.MouseDown += ( _, e ) => {
                    if ( flow.Screen == GameScreen.ResearchArchive ) {
                        if ( e.Button == MouseButtons.Left && hud.ResearchStation( archive, null, viewport.Width, viewport.Height, click: new( e.X, e.Y ) ) )
                            flow.Back();
                        return;
                    }
                    if ( flow.Screen != GameScreen.Flight ) {
                        var actions = flow.Actions;
                        int row = FleetHud.MenuHit( viewport.Width, viewport.Height, new( e.X, e.Y ), actions.Length );
                        if ( row >= 0 ) {
                            flow.FocusRow( row );
                            if ( e.Button == MouseButtons.Left ) Activate( window, actions[row],
                                FleetHud.MenuRightHalf( viewport.Width, viewport.Height, e.X ) ? 5 : -5 );
                        }
                        return;
                    }
                    if ( CurrentBriefing( ) is not null ) {
                        var briefingHit = FleetHud.HitBriefing( new( e.X, e.Y ), viewport.Width, viewport.Height );
                        if ( e.Button == MouseButtons.Left && briefingHit.Action == HudAction.Briefing ) ResolveBriefing( briefingHit.Index == 1 );
                        return;
                    }
                    if ( commandTab == CommandTab.Research && !galaxy ) {
                        if ( e.Button == MouseButtons.Left && hud.ResearchStation( researchDesk, fleet, viewport.Width, viewport.Height, click: new( e.X, e.Y ) ) )
                            commandTab = CommandTab.Hidden;
                        return;
                    }
                    if ( galaxy ) {
                        if ( e.Button == MouseButtons.Middle ) {
                            panStart = e.Location;
                            window.Capture = true;
                        }
                        if ( e.Button is MouseButtons.Left or MouseButtons.Right )
                            GalaxyCommand( window, new( e.X, e.Y ), e.Button == MouseButtons.Right, e.Clicks );
                        return;
                    }
                    var pointer = new Vector2( e.X, e.Y );
                    if ( commandTab == CommandTab.Fleet ) {
                        if ( e.Button != MouseButtons.Left ) return;
                        var control = FleetHud.HitDesigner( pointer, viewport.Width, viewport.Height, designer );
                        if ( control.Control == DesignerControl.Close ) commandTab = CommandTab.Hidden;
                        else {
                            designer.Click( fleet, control );
                            if ( designer.Dragging ) window.Capture = true;
                        }
                        return;
                    }
                    if ( briefings.Count == 0 && menu is { } popup ) {
                        var popupHit = FleetHud.Hit( pointer, viewport.Width, viewport.Height, fleet, planet, popup );
                        menu = null;
                        if ( e.Button == MouseButtons.Left ) {
                            if ( popupHit.Action == HudAction.Context ) PlanetOrder( (PlanetAction)popupHit.Index, popup.Body );
                            return;
                        }
                    }
                    var hit = FleetHud.Hit( pointer, viewport.Width, viewport.Height, fleet, planet, commandTab: commandTab, yardInspector: yardInspector, buildCatalog: buildCatalog, hasSelection: selection.Count != 0, dispatch: dispatch, researchBranch: researchBranch, catalogLayer: catalogLayer, fleetScroll: fleetScroll, briefing: CurrentBriefing( ), shipDetails: shipDetails, yardPage: yardPage );
                    if ( hit.Action != HudAction.None ) {
                        if ( e.Button == MouseButtons.Right && hit.Action == HudAction.Squad && fleet.Frontier ) {
                            Rename( window, true, hit.Index );
                            return;
                        }
                        if ( e.Button == MouseButtons.Left ) {
                            switch ( hit.Action ) {
                                case HudAction.Pause: paused = !paused; break;
                                case HudAction.Speed: ChangeSpeed( increase: true ); break;
                                case HudAction.Slower: ChangeSpeed( increase: false ); break;
                                case HudAction.ExpeditionRoute:
                                    ToggleGalaxy( window );
                                    routeTarget = FleetSimulation.Proxima;
                                    break;
                                case HudAction.ResearchDetails:
                                    var topic = (ResearchTopic)hit.Index;
                                    var tech = FleetSimulation.Tech( topic );
                                    var successors = Enumerable.Range( 0, ResearchCatalog.Current.Count ).Select( i => (ResearchTopic)i )
                                        .Where( id => FleetSimulation.Tech( id ).Requires.Contains( topic ) );
                                    string leads = string.Join( ", ", successors.Select( id => FleetSimulation.Tech( id ).Name ) );
                                    string dependencies = string.Join( ", ", tech.Requires.Select( id => FleetSimulation.Tech( id ).Name ) );
                                    string dossier = tech.Description + (dependencies.Length == 0 ? "" : $"\nREQUIRES {dependencies}") +
                                        (leads.Length == 0 ? "" : $"\nOPENS {leads}");
                                    briefings.Enqueue( (new( tech.Name, "RESEARCH DOSSIER", dossier, tech.Artwork,
                                        "BACK TO RESEARCH", "CLOSE", tech.Benefit ), null, null) );
                                    break;
                                case HudAction.Galaxy: ToggleGalaxy( window ); break;
                                case HudAction.Wing: QueueWing( ); break;
                                case HudAction.Command: Command( (CommandAction)hit.Index ); break;
                                case HudAction.Planet: PlanetOrder( (PlanetAction)hit.Index, planet ); break;
                                case HudAction.OpenYard: OpenYard( hit.Index ); break;
                                case HudAction.CloseYard: yardInspector = -1; break;
                                case HudAction.BuildShip:
                                    var hull = (HullBlueprint)hit.Index;
                                    if ( fleet.QueueShip( yardInspector, hull ) ) {
                                        Notify( $"{fleet.Hull( hull ).Name} QUEUED / {fleet.BodyName( yardInspector )}" );
                                        audio.Cue( "order" );
                                    }
                                    break;
                                case HudAction.CancelShip: CancelOrder( fleet.CancelShip( yardInspector, hit.Index ) ); break;
                                case HudAction.ToggleBuildCatalog: buildCatalog = !buildCatalog; catalogLayer = SiteLayer.Orbit; break;
                                case HudAction.BuildLayer: catalogLayer = catalogLayer == SiteLayer.Orbit ? SiteLayer.Surface : SiteLayer.Orbit; break;
                                case HudAction.Colonize: Settle( hit.Index ); break;
                                case HudAction.Climate:
                                    if ( fleet.ClimateBlock( hit.Index ) is { } climateBlock ) Notify( climateBlock );
                                    else if ( fleet.BeginClimateProject( hit.Index ) ) Notify( "CLIMATE PROJECT STARTED" );
                                    break;
                                case HudAction.CancelColony:
                                    if ( fleet.CancelColony( hit.Index ) ) Notify( "SETTLEMENT ORDER CANCELED" );
                                    break;
                                case HudAction.Stance: CycleStance( ); break;
                                case HudAction.ShipService: Service( (ServiceOrder)hit.Index ); break;
                                case HudAction.YardPage: yardPage = hit.Index; break;
                                case HudAction.ShipDetails:
                                    if ( selection.Count > 0 ) shipDetails = !shipDetails;
                                    break;
                                case HudAction.ScrollFleet: fleetScroll = hit.Index; break;
                                case HudAction.ResearchPage: researchBranch = (ResearchBranch)hit.Index; break;
                                case HudAction.ResearchStart:
                                    var project = (ResearchTopic)hit.Index;
                                    if ( fleet.StartResearch( project ) ) Notify( $"{FleetSimulation.Tech( project ).Name} RESEARCH STARTED" );
                                    break;
                                case HudAction.MerchantDecision:
                                    if ( fleet.AcceptContract( (MerchantDecision)hit.Index ) ) {
                                        Notify( "MERCHANT CONTRACT CONFIRMED" );
                                        audio.Cue( "dispatch" );
                                    }
                                    break;
                                case HudAction.UpgradeSite:
                                    var upgrade = (PlanetFacility)hit.Index;
                                    if ( fleet.UpgradeBlock( planet, upgrade ) is { } block ) Notify( block );
                                    else if ( fleet.UpgradeSite( planet, upgrade ) ) Notify( $"{FleetHud.SiteName( upgrade )} UPGRADE QUEUED" );
                                    break;
                                case HudAction.BuildSite: BuildProject( planet, (PlanetFacility)hit.Index ); break;
                                case HudAction.CancelSite: CancelOrder( fleet.CancelSite( planet, (PlanetFacility)hit.Index ) ); break;
                                case HudAction.MergeFleets: OrganizeFleets( false ); break;
                                case HudAction.SplitFleets: OrganizeFleets( true ); break;
                                case HudAction.Squad when fleet.ShipCount( hit.Index ) > 0:
                                    selection.Squadron( fleet, hit.Index, ModifierSelection( ) );
                                    planet = yardInspector = -1;
                                    if ( e.Clicks >= 2 ) SetCameraFocus( fleet.Center( hit.Index ) );
                                    break;
                            }
                        }
                        return;
                    }
                    if ( e.Button == MouseButtons.Middle ) {
                        panStart = e.Location;
                        tilting = (Control.ModifierKeys & Keys.Shift) != 0;
                        window.Capture = true;
                        return;
                    }
                    if ( e.Button == MouseButtons.Left ) {
                        dragStart = dragEnd = e.Location;
                        dragMode = ModifierSelection( );
                        window.Capture = true;
                        return;
                    }
                    if ( e.Button != MouseButtons.Right ) return;
                    var dimensions = new Vector2( viewport.Width, viewport.Height );
                    int enemy = selection.Count == 0 ? -1 : CommandSelection.Hit( fleet, pointer, viewProjection, dimensions, -2, interpolation );
                    if ( enemy >= 0 ) {
                        if ( fleet.Frontier && !HasArmedSelection( ) ) { Notify( "SELECT A WARSHIP TO ATTACK" ); return; }
                        selection.Issue( fleet, default, hostile: enemy );
                        Notify( $"{SelectionName( )} ATTACKING SHIP {enemy + 1:D2}" );
                    } else if ( PickBody( e.Location ) is >= 0 and var body ) {
                        if ( fleet.ChartOnly ) { planet = body; return; }
                        menu = new( pointer, body );
                        return;
                    } else {
                        if ( selection.Count == 0 || !TryPick( e.Location, viewport, inverseViewProjection, out var target ) ) return;
                        if ( !selection.Issue( fleet, target ) ) { Notify( "OUTSIDE ROUND TRIP RANGE / RESEARCH DRIVE" ); return; }
                        Notify( $"{SelectionName( )} TRANSFER PLOTTED" );
                    }
                    audio.Cue( "order" );
                };
                window.MouseLeave += ( _, _ ) => mouse = null;
                window.MouseMove += ( _, e ) => {
                    if ( flow.Screen != GameScreen.Flight ) {
                        flow.FocusRow( FleetHud.MenuHit( viewport.Width, viewport.Height, new( e.X, e.Y ), flow.Actions.Length ) );
                        return;
                    }
                    mouse = e.Location;
                    if ( galaxy ) {
                        if ( panStart is { } last && e.Button == MouseButtons.Middle ) {
                            galaxyView = galaxyView.Pan( new( (e.X - last.X) / (float)viewport.Width, (e.Y - last.Y) / (float)viewport.Height ) );
                            panStart = e.Location;
                        }
                        return;
                    }
                    if ( dragStart is not null ) { dragEnd = e.Location; return; }
                    if ( panStart is not { } previous ) return;
                    if ( e.Button != MouseButtons.Middle ) { panStart = null; return; }
                    if ( tilting ) {
                        yaw = MathF.IEEERemainder( yaw - (e.X - previous.X) * 0.006f, MathF.Tau );
                        tilt = Math.Clamp( tilt + (e.Y - previous.Y) * 0.005f, 0.50f, 1.36f );
                    } else if ( TryPick( previous, viewport, inverseViewProjection, out var from ) &&
                         TryPick( e.Location, viewport, inverseViewProjection, out var to ) )
                        SetCameraFocus( cameraFocus + from - to );
                    panStart = e.Location;
                };
                window.MouseCaptureChanged += ( _, _ ) => {
                    // WinForms releases capture before raising MouseUp. Let that handler finish the gesture.
                    if ( window.Capture || window.IsDisposed ) return;
                    window.BeginInvoke( () => {
                        if ( window.Capture ) return;
                        panStart = dragStart = null;
                        designer.Release( default );
                    } );
                };
                window.MouseUp += ( _, e ) => {
                    if ( flow.Screen != GameScreen.Flight ) return;
                    if ( commandTab == CommandTab.Fleet && designer.Dragging && e.Button == MouseButtons.Left ) {
                        designer.Release( CurrentBriefing( ) is null
                            ? FleetHud.HitDesigner( new( e.X, e.Y ), viewport.Width, viewport.Height, designer ) : default );
                        window.Capture = false;
                        return;
                    }
                    if ( e.Button == MouseButtons.Middle ) { panStart = null; window.Capture = false; return; }
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
                        int hitSite = fleet.Frontier ? PickSite( new( e.X, e.Y ) ) : -1;
                        if ( hitSite >= 0 && screenSites[hitSite] is { Facility: PlanetFacility.Yard } yard && fleet.CanOpenYard( yard.Body ) ) {
                            OpenYard( yard.Body );
                            lastClickShip = -1;
                            return;
                        }
                        int hitBody = ship < 0 ? hitSite >= 0 ? screenSites[hitSite].Body : PickBody( e.Location ) : -1;
                        if ( hitBody >= 0 ) {
                            long now = Environment.TickCount64;
                            var tolerance = SystemInformation.DoubleClickSize;
                            bool doubleClick = e.Clicks >= 2 || lastClickBody == hitBody && now - lastClickTime <= SystemInformation.DoubleClickTime &&
                                Math.Abs( lastClickPoint.X - e.X ) <= tolerance.Width && Math.Abs( lastClickPoint.Y - e.Y ) <= tolerance.Height;
                            if ( doubleClick ) TrackBody( hitBody );
                            lastClickBody = doubleClick ? -1 : hitBody;
                            lastClickTime = now;
                            lastClickPoint = e.Location;
                            planet = hitBody;
                            yardInspector = -1;
                            buildCatalog = false;
                        }
                        else {
                            lastClickBody = -1;
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
                    if ( flow.Screen != GameScreen.Flight ) return;
                    if ( galaxy ) {
                        if ( CurrentBriefing() is null ) galaxyView = galaxyView.ZoomAt(
                            new( e.X / (float)viewport.Width, e.Y / (float)viewport.Height ), MathF.Pow( 1.24f, e.Delta / 120f ) );
                        return;
                    }
                    if ( commandTab == CommandTab.Research ) return;
                    if ( !galaxy && briefings.Count == 0 && menu is null && FleetHud.ScrollOutliner( new( e.X, e.Y ), viewport.Width,
                        viewport.Height, fleet, Math.Sign( e.Delta ), ref fleetScroll ) ) return;
                    if ( galaxy || menu is not null || FleetHud.Hit( new( e.X, e.Y ), viewport.Width, viewport.Height, fleet, planet, commandTab: commandTab, yardInspector: yardInspector, buildCatalog: buildCatalog, hasSelection: selection.Count != 0, dispatch: dispatch, researchBranch: researchBranch, catalogLayer: catalogLayer, fleetScroll: fleetScroll, briefing: CurrentBriefing( ), shipDetails: shipDetails, yardPage: yardPage ).Action != HudAction.None ) return;
                    float nextZoom = Math.Clamp( zoom * MathF.Pow( 0.85f, e.Delta / 120f ), 0.045f, OverviewZoom() );
                    if ( trackedBody < 0 && TryPick( e.Location, viewport, inverseViewProjection, out var anchor ) )
                        SetCameraFocus( cameraFocus + (anchor - cameraFocus) * (1 - nextZoom / zoom) );
                    zoom = nextZoom;
                };
            }
            if ( settings.AudioTest ) {
                if ( Environment.TickCount64 > audioTestDeadline )
                    throw new InvalidOperationException( $"Music did not advance during {flow.Screen}." );
                if ( flow.Screen == GameScreen.MainMenu && audio.MusicPosition > TimeSpan.FromMilliseconds( 500 ) )
                    Activate( window, MenuAction.Launch );
                else if ( flow.Screen == GameScreen.Flight && audio.MusicPosition > TimeSpan.FromMilliseconds( 500 ) ) {
                    pausePosition = audio.MusicPosition;
                    flow.Back();
                } else if ( flow.Screen == GameScreen.PauseMenu && audio.MusicPosition > pausePosition + TimeSpan.FromMilliseconds( 750 ) ) {
                    Console.WriteLine( "PASS: Title music played and the flight soundtrack advanced through pause." );
                    window.Close();
                }
            }
            if ( settings.MenuTest ) {
                switch ( result.Frames ) {
                    case 1: Activate( window, MenuAction.ResearchArchive ); break;
                    case 3:
                        if ( flow.Screen != GameScreen.ResearchArchive || fleet.Tick != 0 )
                            throw new InvalidOperationException( "Browsing the archive must leave the campaign idle." );
                        if ( hud.ResearchStation( archive, null, viewport.Width, viewport.Height,
                             click: FleetHud.ResearchBackPixel( viewport.Width, viewport.Height ) ) ) flow.Back();
                        if ( flow.Screen != GameScreen.MainMenu ) throw new InvalidOperationException( "The archive did not return to the title." );
                        break;
                    case 4: Activate( window, MenuAction.Launch ); break;
                    case 7:
                        if ( flow.Screen != GameScreen.Flight || scenario?.StartingSites.Length == 0 || !fleet.StartResearch( ResearchTopic.Industry ) )
                            throw new InvalidOperationException( "The explicit launch did not start a playable campaign." );
                        flow.Back();
                        break;
                    case 8: Activate( window, MenuAction.Resume ); break;
                    case 9:
                        SelectTab( CommandTab.Fleet );
                        designer.Open( fleet, HullBlueprint.Corvette );
                        break;
                    case 10:
                        if ( flow.Screen != GameScreen.Flight || commandTab != CommandTab.Fleet || designer.Hull != HullBlueprint.Corvette )
                            throw new InvalidOperationException( "Designer launch failed." );
                        designer.Click( fleet, new( DesignerControl.Slot, (int)ModuleSlot.Power ) );
                        designer.Click( fleet, new( DesignerControl.Module, 2 ) );
                        designer.Release( new( DesignerControl.Module, 2 ) );
                        designer.Click( fleet, new( DesignerControl.Save ) );
                        if ( fleet.Blueprint( HullBlueprint.Corvette ).Variant( ModuleSlot.Power ) != 2 )
                            throw new InvalidOperationException( "New reactor fit failed to save." );
                        break;
                }
            }
            if ( resizeTest && result.Frames == 2 ) window.ClientSize = new( 800, 520 );
            if ( resizeTest && result.Frames == 4 ) window.ClientSize = new( 710, 575 );
            if ( viewTest ) {
                if ( result.Frames == 2 ) { display.Toggle( window ); Enter( 2 ); }
                if ( result.Frames == 4 ) display.Toggle( window );
                if ( result.Frames == 5 ) {
                    Enter( 0 );
                    galaxy = galaxyRoster = true;
                }
                if ( result.Frames == 6 ) {
                    Enter( 0 );
                    OpenYard( FleetSimulation.Homeworld );
                    if ( !fleet.QueueShip( FleetSimulation.Homeworld, HullBlueprint.Miner ) )
                        throw new InvalidOperationException( "View qualification could not place the civilian dock order." );
                }
                if ( result.Frames == 7 ) {
                    yardInspector = -1;
                    commandTab = CommandTab.Hidden;
                    float scale = Math.Clamp( MathF.Min( viewport.Height / 500f, viewport.Width / 660f ), 0.8f, 2.5f );
                    ClickWindow( new( (int)(40 * scale), (int)(114 * scale) ) );
                    if ( selection.Count == 0 )
                        throw new InvalidOperationException( "The Fleet Outliner did not select a fleet." );
                    var (start, end) = (new Point( viewport.Width / 2, viewport.Height / 3 ),
                        new Point( viewport.Width / 2 + 24, viewport.Height / 3 + 24 ));
                    SendWindowMessage( window.Handle, LeftDownMessage, 1, EncodePoint( start ) );
                    SendWindowMessage( window.Handle, MouseMoveMessage, 1, EncodePoint( end ) );
                    SendWindowMessage( window.Handle, LeftUpMessage, 0, EncodePoint( end ) );
                    if ( selection.Count != 0 )
                        throw new InvalidOperationException( "World drag could not replace the Fleet Outliner selection." );
                    SelectTab( CommandTab.Research );

                    void ClickWindow( Point point ) {
                        SendWindowMessage( window.Handle, LeftDownMessage, 1, EncodePoint( point ) );
                        SendWindowMessage( window.Handle, LeftUpMessage, 0, EncodePoint( point ) );
                    }
                    static nint EncodePoint( Point point ) => (point.Y << 16) | (point.X & 0xffff);
                }
                if ( result.Frames == 8 ) { SelectTab( CommandTab.Hidden ); buildCatalog = true; }
                if ( result.Frames == 9 ) {
                    buildCatalog = false;
                    SelectTab( CommandTab.Fleet );
                    designer.Open( fleet, HullBlueprint.Frigate );
                }
                if ( result.Frames == 10 ) {
                    var scout = new FleetSelection( fleet.FirstPlayerSquad( ) );
                    if ( !fleet.BeginSurvey( scout, 3 ) )
                        throw new InvalidOperationException( "View qualification could not dispatch the scout." );
                    int scanWindow = fleet.RouteTicks( scout ) + fleet.Planet( 3 ).SurveyRequired + 2;
                    for ( int i = 0; i < scanWindow && !fleet.HasMerchantOffer; ++i ) fleet.Step( );
                    if ( !fleet.HasMerchantOffer ) throw new InvalidOperationException( "View qualification did not reach the merchant offer." );
                    dispatch = fleet.DispatchCount - 1;
                    SelectTab( CommandTab.Events );
                }
                if ( result.Frames == 11 ) {
                    SelectTab( CommandTab.Hidden );
                    menu = new( new( viewport.Width - 5, viewport.Height - 5 ), 3 );
                }
                if ( result.Frames == 13 ) {
                    galaxy = true;
                    if ( pendingStories.FirstOrDefault( s => s.Id == "receiver-cache" ) is { } cache ) {
                        var remote = campaign.System( FleetSimulation.Proxima );
                        var scout = new ExpeditionHull( HullRole.Scout, default, 50, 0, 0, FleetStance.Passive, "SURVEYOR" );
                        if ( campaign.OccupiedSystem( remote.SystemIndex ) is not null ||
                             !remote.Receive( new( remote.SystemIndex, 0, [scout], default ), initial.Tick ) )
                            throw new InvalidOperationException( "Proxima stockpile qualification could not establish the expedition." );
                        var homeStores = (initial.Ore, initial.Energy, initial.Science);
                        QueueCampaignBriefings();
                        var dispatchOffer = briefings.Single( offer => offer.Text == cache.Briefing );
                        dispatchOffer.Accept!();
                        if ( new ResourceCost( remote.Ore, remote.Energy, remote.Science ) != cache.Accept.Stores ||
                             homeStores != (initial.Ore, initial.Energy, initial.Science) )
                            throw new InvalidOperationException( "The expedition cache must credit Proxima's stockpile only." );
                    }
                    briefings.Enqueue( (new( "EXPEDITION ARRIVED", "PROXIMA / FLIGHT OPERATIONS",
                        "Receiver World is in sensor range.", ContentArt.Receiver, "FOCUS ARRIVAL", "KEEP CURRENT VIEW" ), null, null) );
                }
                if ( result.Frames == 12 ) {
                    menu = null;
                    buildCatalog = true;
                    catalogLayer = SiteLayer.Surface;
                    planet = FleetSimulation.Homeworld;
                    Rename( window, forceFleet: false );
                    if ( renameEditor is not { Modal: false, Visible: true } || paused )
                        throw new InvalidOperationException( "Rename qualification did not open a modeless editor with the simulation running." );
                }
                if ( result.Frames == 14 ) {
                    if ( renameEditor is not { Visible: true } || paused )
                        throw new InvalidOperationException( "Rendering did not advance with the rename editor open." );
                    renameEditor.Close( );
                    galaxy = false;
                    briefings.Clear();
                    buildCatalog = false;
                    planet = 0;
                    SetCameraFocus( fleet.Bodies[0].Center );
                    zoom = .28f;
                    paused = true;
                }
            }
        } );
        if ( resizeTest && slotsUsed != 3 )
            throw new InvalidOperationException( "Resize qualification did not exercise both frame slots." );
        if ( viewTest && (!sawGalaxy || !systemsVisited.SetEquals( [0, 2] ) || display.Active) )
            throw new InvalidOperationException( "View qualification did not exercise the galaxy, both systems, and fullscreen restoration." );
        return run;

        Vector2 ClampCamera( Vector2 center ) => float.IsFinite( center.X ) && float.IsFinite( center.Y )
            ? Vector2.Clamp( center, new( -fleet.NavigationRadius ), new( fleet.NavigationRadius ) )
            : fleet.Bodies[FleetSimulation.Homeworld].Center;

        float OverviewZoom() => fleet.Solar ? fleet.NavigationRadius / fleet.BoardRadius * .88f : fleet.Frontier ? 2.8f : 2.5f;

        void ResetCamera( ) {
            zoom = fleet.Solar ? 0.38f : 0.85f;
            yaw = 0;
            tilt = DefaultTilt;
            cameraFocus = fleet.Solar ? fleet.Bodies[FleetSimulation.Homeworld].Center : default;
            viewFocus = cameraFocus;
            viewZoom = zoom;
            viewTilt = tilt;
            viewYaw = yaw;
            lastClickBody = -1;
            trackedBody = fleet.Solar ? FleetSimulation.Homeworld : -1;
        }

        void TrackBody( int body ) {
            trackedBody = body;
            cameraFocus = fleet.VisualBody( body, interpolation ).Center;
        }

        void SetCameraFocus( Vector2 position ) {
            trackedBody = -1;
            cameraFocus = ClampCamera( position );
        }

        static SelectionMode ModifierSelection( ) {
            var mods = Control.ModifierKeys;
            return (mods & Keys.Control) != 0 ? SelectionMode.Toggle
                : (mods & Keys.Shift) != 0 ? SelectionMode.Add : SelectionMode.Replace;
        }

        void ToggleGalaxy( Form window ) {
            designer.Release( default );
            galaxy = !galaxy;
            galaxyRoster = galaxy && fleet.CyanCount > 0;
            routeNotice = null;
            menu = null;
            panStart = dragStart = null;
            window.Capture = false;
        }

        void ChangeSpeed( bool increase ) => speed = FleetClock.Change( speed, increase );

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

        void UpdateGalaxyHulls( ) {
            for ( int i = 0; i < galaxyHulls.Length; ++i ) galaxyHulls[i] = campaign.HullsAt( i );
        }

        void QueueCampaignBriefings( ) {
            for ( int i = 0; i < pendingStories.Count; ) {
                var story = pendingStories[i];
                var origin = campaign.OccupiedSystem( story.SystemIndex );
                if ( origin is null || !origin.MeetsTrigger( story ) ) { ++i; continue; }
                pendingStories.RemoveAt( i );
                briefings.Enqueue( (story.Briefing, () => origin.ClaimDispatch( story.Accept ), () => origin.ClaimDispatch( story.Decline )) );
            }
            while ( campaign.TryTakeEvent( out var entry ) ) {
                var system = entry.System;
                switch ( entry.Kind ) {
                    case CampaignEventKind.ResearchComplete:
                        var tech = FleetSimulation.Tech( entry.Technology );
                        briefings.Enqueue( (new( "RESEARCH COMPLETED", tech.Name, tech.Description, tech.Artwork,
                            "OPEN RESEARCH", "DISMISS", tech.Benefit ), () => {
                            if ( fleet != system ) Enter( system.SystemIndex );
                            galaxy = false;
                            researchBranch = ResearchCatalog.Current.PageOf( entry.Technology );
                            researchDesk.Select( entry.Technology );
                            yardInspector = -1;
                            commandTab = CommandTab.Research;
                        }, null) );
                        audio.Cue( "launch" );
                        break;
                    case CampaignEventKind.EconomicCharter:
                        briefings.Enqueue( (new( "THE FRONTIER CHARTER", $"{system.PlayerFaction.Name} / FIRST SURVEY",
                            "The first confirmed deposit has unlocked public investment. Set a permanent priority for Sol: extraction grants for new operations or long-term power contracts.",
                            ContentArt.Mine, "FUND EXTRACTION", "FUND POWER",
                            "+1 ORE PER ACTIVE EXTRACTION SITE EACH CYCLE", "+1 ENERGY EACH CYCLE IN SOL",
                            EscapeDismisses: false ), () => {
                                if ( system.AdoptCharter( EconomicCharter.Extraction ) ) Notify( "SOL ADOPTED THE EXTRACTION CHARTER" );
                            }, () => {
                                if ( system.AdoptCharter( EconomicCharter.Power ) ) Notify( "SOL ADOPTED THE POWER CHARTER" );
                            }) );
                        break;
                    case CampaignEventKind.Distress:
                        briefings.Enqueue( (new( "THE FIRST PIRATES", $"{system.PlayerFaction.Name} / EXTRACTION DISPATCH",
                            "Unregistered hulls are following our outbound ore shipments. Their transponders went dark as the first convoy left. " +
                            "The Ceres Combine offers a patrol to hold them off while our own escorts get ready.",
                            ContentArt.Corvette, "FUND CERES PATROL", "PREPARE OUR OWN FLEET",
                            "15 ENERGY / +60 SECONDS AT 1X / ONE FEWER RAIDER", "KEEP OUR ENERGY / PREPARE FOR THE FULL RAID",
                            EscapeDismisses: false ), () => Notify( system.FundPatrol( )
                                ? "CERES PATROL FUNDED / RAID DELAYED" : "PATROL UNAVAILABLE / PREPARE YOUR FLEET" ), null) );
                        break;
                    case CampaignEventKind.TravelArrival:
                        Notify( $"FLEET ARRIVED / {system.SystemName}" );
                        break;
                    case CampaignEventKind.Arrival:
                        briefings.Enqueue( (new( "EXPEDITION ARRIVED", "PROXIMA / FLIGHT OPERATIONS",
                            "The first crossing is complete. Other star routes are open. Survey Receiver World and send a constructor to establish an orbital lab and restore the transmitter.",
                            ContentArt.Receiver, "FOCUS ARRIVAL", "KEEP CURRENT VIEW" ), () => Enter( system.SystemIndex ), null) );
                        audio.Cue( "launch" );
                        break;
                    case CampaignEventKind.ReceiverDecision:
                        bool canClaim = system.CanClaimReceiver;
                        briefings.Enqueue( (new( "WHO CONTROLS THE RECEIVER?", "PROXIMA / SURVEY CONSORTIUM",
                            "The receiver is answering. The Consortium wants an open channel so independent crews can study the precursor data. " +
                            (canClaim ? "Rival Command is approaching and will challenge a sole claim." : "A sole claim requires an expedition escort."),
                            ContentArt.Receiver, "SHARE WITH CONSORTIUM", canClaim ? "CLAIM FOR SOL" : "SHARE AND CLOSE",
                            "OPEN ACCESS / +30 SCIENCE", canClaim ? "RIVAL COMMAND WILL INTERCEPT" : "OPEN ACCESS / +30 SCIENCE",
                            EscapeDismisses: false ), () => system.CommissionReceiver( ReceiverCharter.Shared ),
                            () => system.CommissionReceiver( canClaim ? ReceiverCharter.Independent : ReceiverCharter.Shared )) );
                        break;
                    case CampaignEventKind.CrossingComplete:
                        briefings.Enqueue( (new( "THE FIRST CROSSING COMPLETE", "PROXIMA / EXPEDITION DISPATCH",
                            system.Charter == ReceiverCharter.Shared
                                ? "The Proxima receiver is transmitting. Sol and the Consortium share the link. Independent crews can now read the precursor record."
                                : "The Proxima receiver is transmitting. With Rival Command driven off, Sol holds the first working interstellar link.",
                            ContentArt.Receiver, "CONTINUE CAMPAIGN", "CLOSE" ), null, null) );
                        break;
                }
            }
        }

        GalaxyRoute RouteView( ) {
            int source = fleet.SystemIndex;
            if ( source == 0 && routeTarget == FleetSimulation.Proxima && campaign.InTransit is { } expedition ) {
                float progress = (float)(initial.Tick - campaign.FirstDepartureTick) / (expedition.ArrivalTick - campaign.FirstDepartureTick);
                return new( 0, routeTarget, $"FIRST CROSSING / {expedition.Hulls.Length} HULLS IN TRANSIT",
                    null, 0, InTransit: true, Progress: progress );
            }
            foreach ( var flight in campaign.Transfers ) {
                if ( flight.Source != source || flight.Destination != routeTarget || selection.Count != 0 ) continue;
                int remaining = Math.Max( 0, flight.Arrival - initial.Tick );
                return new( source, routeTarget, $"IN TRANSIT / {flight.Hulls.Length} HULLS / {Math.Ceiling( remaining * FleetSimulation.StepSeconds / speed )} SEC",
                    null, 0, InTransit: true, Progress: (float)(initial.Tick - flight.Departure) / (flight.Arrival - flight.Departure) );
            }
            var block = campaign.TravelBlock( fleet, routeTarget, selection.Ships, survey: false );
            bool firstCrossing = fleet.FrontierProgression && !campaign.Arrived && source == 0 && routeTarget == FleetSimulation.Proxima;
            bool automaticCrew = firstCrossing && block is not null && SuggestedCrewBlock() is null;
            if ( automaticCrew ) block = null;
            bool canSurvey = block is null && campaign.TravelBlock( fleet, routeTarget, selection.Ships, survey: true ) is null;
            string status = routeNotice ?? (campaign.Arrived ? "SELECT A FLEET / RIGHT CLICK A DESTINATION TO MOVE"
                : fleet.FrontierProgression ? "FIRST CROSSING / EQUIP A SCOUT AND CONSTRUCTOR FOR PROXIMA"
                : "FIRST CROSSING / SEND ONE IDLE SCOUT FROM SOL TO PROXIMA");
            string? quote = source == routeTarget ? null : $"TRANSIT / {(int)Math.Ceiling( campaign.FlightTicks( source, routeTarget ) * FleetSimulation.StepSeconds / speed )} SEC";
            var cost = fleet.ExpeditionCost;
            var (scout, builder) = firstCrossing ? fleet.ReadyExpeditionCrew() : (-1, -1);
            string? readiness = firstCrossing ? $"CREW {(scout >= 0 ? 1 : 0) + (builder >= 0 ? 1 : 0)}/2 / {fleet.Ore}/{cost.Ore} ORE / {fleet.Energy}/{cost.Energy} EN" : null;
            return new( source, routeTarget, status, block, automaticCrew ? 2 : selection.Count, CanSurvey: canSurvey, Quote: quote,
                RallyCrew: firstCrossing, FusionResearch: firstCrossing && !fleet.FusionUpgrade, Readiness: readiness );
        }

        string? SuggestedCrewBlock() {
            var (scout, builder) = fleet.ReadyExpeditionCrew();
            if ( scout < 0 || builder < 0 ) return "RALLY A SCOUT AND CONSTRUCTOR TO EARTH";
            Span<bool> crew = stackalloc bool[fleet.Count];
            crew.Clear();
            crew[scout] = crew[builder] = true;
            return fleet.LaunchBlock( crew );
        }

        void GalaxyCommand( Form window, Vector2 pointer, bool rightClick, int clicks ) {
            selection.Prune( fleet );
            UpdateGalaxyHulls( );
            var hit = FleetHud.HitGalaxy( pointer, viewport.Width, viewport.Height, fleet.Frontier, galaxyHulls, RouteView( ),
                galaxyRoster ? fleet : null, rosterPage, galaxyView );
            if ( rightClick && hit.Action != GalaxyAction.Star ) return;
            switch ( hit.Action ) {
                case GalaxyAction.Return: ToggleGalaxy( window ); break;
                case GalaxyAction.Pause: paused = !paused; break;
                case GalaxyAction.Slower: ChangeSpeed( increase: false ); break;
                case GalaxyAction.Faster: ChangeSpeed( increase: true ); break;
                case GalaxyAction.Star:
                    if ( !rightClick && clicks >= 2 || !fleet.Frontier ) { Enter( hit.System ); break; }
                    routeTarget = hit.System;
                    routeNotice = null;
                    if ( rightClick && selection.Count > 0 ) IssueTravel( survey: false );
                    break;
                case GalaxyAction.Fleet:
                    if ( clicks >= 2 ) { Enter( hit.System ); break; }
                    if ( fleet.SystemIndex != hit.System ) Enter( hit.System );
                    galaxy = galaxyRoster = true;
                    rosterPage = 0;
                    routeNotice = "SELECT A FLEET ROW / SHIFT ADD / CTRL TOGGLE";
                    break;
                case GalaxyAction.RosterRow:
                    selection.Squadron( fleet, hit.System, ModifierSelection( ) );
                    routeNotice = null;
                    break;
                case GalaxyAction.RosterClose: galaxyRoster = false; break;
                case GalaxyAction.RosterPrevious: --rosterPage; break;
                case GalaxyAction.RosterNext: ++rosterPage; break;
                case GalaxyAction.FindScout:
                    if ( fleet.FrontierProgression && !campaign.Arrived && fleet.SystemIndex == 0 ) {
                        routeNotice = fleet.SelectExpedition( selection ) > 0 ? "EXPEDITION CREW SELECTED" :
                            fleet.RallyExpedition() ? "CREW ORDERED TO EARTH ORBIT / TRY AGAIN AFTER ARRIVAL" :
                            "NEEDS AN IDLE SCOUT AND CONSTRUCTOR IN EARTH ORBIT";
                        break;
                    }
                    int scout = -1;
                    for ( int i = 0; i < fleet.Count; ++i )
                        if ( fleet.TravelReady( i ) && fleet.Role( fleet.SquadronOf( i ) ) == HullRole.Scout ) { scout = i; break; }
                    if ( scout >= 0 ) selection.Click( fleet, scout, false, SelectionMode.Replace );
                    routeNotice = scout < 0 ? "NO IDLE SCOUT IN THIS SYSTEM / FINISH OR CANCEL ITS ORDER" : "SCOUT SELECTED / CHOOSE MOVE OR SURVEY";
                    break;
                case GalaxyAction.View: Enter( routeTarget ); break;
                case GalaxyAction.FusionResearch:
                    galaxy = false;
                    SelectTab( CommandTab.Research );
                    researchBranch = ResearchCatalog.Current.PageOf( ResearchTopic.Fusion );
                    researchDesk.Select( ResearchTopic.Fusion );
                    break;
                case GalaxyAction.Move: IssueTravel( survey: false ); break;
                case GalaxyAction.Survey: IssueTravel( survey: true ); break;
            }
        }

        void IssueTravel( bool survey ) {
            routeNotice = campaign.TravelBlock( fleet, routeTarget, selection.Ships, survey );
            if ( routeNotice is not null && !survey && fleet.FrontierProgression && fleet.Solar && !campaign.Arrived &&
                 routeTarget == FleetSimulation.Proxima && SuggestedCrewBlock() is null ) {
                fleet.SelectExpedition( selection );
                routeNotice = null;
            }
            if ( routeNotice is not null || !campaign.Travel( fleet, routeTarget, selection.Ships, survey ) ) return;
            selection.Prune( fleet );
            audio.Cue( "launch" );
        }

        void Enter( int index ) {
            renameEditor?.Close( );
            fleetScroll = 0;
            shipDetails = false;
            fleet = campaign.System( index );
            if ( selections[index] is not { } activeSelection ) {
                selections[index] = activeSelection = new CommandSelection( fleet.Capacity );
                activeSelection.Squadron( fleet, fleet.FirstPlayerSquad( ), SelectionMode.Replace );
            }
            selection = activeSelection;
            selection.Prune( fleet );
            galaxyRoster = false;
            planet = fleet.Established ? FleetSimulation.Homeworld : -1;
            yardInspector = -1;
            launchesSeen = fleet.ShipsLaunched;
            raidersSeen = fleet.ActiveRaids;
            servicesSeen = fleet.CompletedServices;
            scansHeard = fleet.StartedScans;
            piracySeen = fleet.PiracyCycles;
            coloniesSeen = fleet.ColoniesFounded;
            catalogLayer = SiteLayer.Orbit;
            buildCatalog = false;
            researchBranch = ResearchBranch.Industry;
            commandTab = CommandTab.Hidden;
            dispatchesSeen = fleet.DispatchCount;
            dispatch = Math.Max( 0, dispatchesSeen - 1 );
            lastClickShip = -1;
            hostileAlert = null;
            shotsHeard = fleet.ShotsFired;
            lossesHeard = fleet.ShipsLost;
            nextCue = 0;
            previousOwner = fleet.Owner;
            surveysHeard = fleet.CompletedSurveys;
            sitesHeard = fleet.CompletedSites;
            buildsHeard = fleet.StartedConstruction;
            wingsHeard = fleet.WingsBuilt;
            yardState = fleet.YardRemaining;
            menu = null;
            oreShort = galaxy = false;
            ResetCamera( );
            if ( !initial.Frontier ) clock.Reset( );
        }

        void ResolveBriefing( bool accept ) {
            if ( !briefings.TryDequeue( out var next ) ) return;
            (accept ? next.Accept : next.Dismiss)?.Invoke( );
        }

        bool HasArmedSelection( ) {
            for ( int i = 0; i < fleet.Count; ++i )
                if ( selection.Ships[i] && fleet.Alive( i ) && FleetSimulation.IsWarship( fleet.Role( fleet.SquadronOf( i ) ) ) ) return true;
            return false;
        }

        void Command( CommandAction action ) {
            switch ( action ) {
                case CommandAction.Toggle:
                    SelectTab( commandTab == CommandTab.Hidden ? CommandTab.Economy : commandTab );
                    break;
                case CommandAction.Economy: SelectTab( CommandTab.Economy ); break;
                case CommandAction.Fleet: SelectTab( CommandTab.Fleet ); break;
                case CommandAction.Orders: SelectTab( CommandTab.Orders ); break;
                case CommandAction.Events: SelectTab( CommandTab.Events ); break;
                case CommandAction.Research: SelectTab( CommandTab.Research ); break;
                case CommandAction.CancelResearch: CancelOrder( fleet.CancelResearch( ) ); break;
                case CommandAction.Earlier: dispatch = Math.Max( 0, dispatch - 1 ); break;
                case CommandAction.Later: dispatch = Math.Clamp( dispatch + 1, 0, Math.Max( 0, fleet.DispatchCount - 1 ) ); break;
            }
        }

        void SelectTab( CommandTab tab ) {
            yardInspector = -1;
            commandTab = commandTab == tab ? CommandTab.Hidden : tab;
            if ( commandTab == CommandTab.Fleet ) designer.Open( fleet, designer.Hull );
        }

        void OpenYard( int body ) {
            if ( !fleet.CanOpenYard( body ) ) return;
            yardInspector = planet = body;
            yardPage = 0;
            buildCatalog = false;
            commandTab = CommandTab.Hidden;
        }

        void CycleStance( ) {
            if ( selection.Count == 0 ) return;
            var next = fleet.SelectionStance( selection.Ships ) switch {
                FleetStance.Normal => FleetStance.Aggressive, FleetStance.Aggressive => FleetStance.Passive, _ => FleetStance.Normal
            };
            fleet.SetStance( selection.Ships, next );
            Notify( $"STANCE / {next.ToString().ToUpperInvariant()}" );
        }

        void Service( ServiceOrder order ) {
            int sent = 0;
            for ( int ship = 0; ship < fleet.Count; ++ship )
                if ( selection.Ships[ship] && fleet.OrderService( ship, order ) ) ++sent;
            if ( sent == 0 ) {
                Notify( fleet.YardCount == 0 && order != ServiceOrder.Retreat ? "SERVICE REQUIRES AN ORBITAL SHIPYARD"
                    : order == ServiceOrder.Refit && !fleet.Design.Valid ? "BLUEPRINT EXCEEDS REACTOR POWER" : "NO ELIGIBLE SHIPS SELECTED" );
                return;
            }
            Notify( $"{sent} SHIP(S) / {order.ToString().ToUpperInvariant()} ORDERED" );
            audio.Cue( "order" );
        }

        void Settle( int body ) {
            var unit = fleet.FindColonyShip( body, selection.Ships );
            if ( fleet.ColonyBlock( body, unit ) is { } reason ) { Notify( reason ); return; }
            if ( unit is { } selected && fleet.BeginColony( body, selected ) ) {
                Notify( $"COLONY SHIP DISPATCHED / {fleet.BodyName( body )}" );
                audio.Cue( "order" );
            }
        }

        void RenameFaction( Form window ) {
            if ( renameEditor is { } existing ) { existing.Activate(); return; }
            var editor = CreateNameEditor( "faction", factionName, name => {
                if ( string.IsNullOrWhiteSpace( name ) ) return;
                factionName = name.Trim();
                campaign.PlayerFaction.Name = factionName;
                if ( !flow.IsTitle ) Notify( $"FACTION / {factionName}" );
            } );
            renameEditor = editor;
            editor.FormClosed += ( _, _ ) => renameEditor = null;
            editor.Show( window );
        }

        void Rename( Form window, bool forceFleet, int outlinerSquad = -1 ) {
            if ( renameEditor is { } existing ) {
                existing.Activate( );
                return;
            }
            int ship = selection.Primary;
            int squad = outlinerSquad >= 0 ? outlinerSquad : ship >= 0 ? fleet.SquadronOf( ship ) : -1;
            int body = !forceFleet ? planet : -1;
            bool fleetLabel = body < 0 && (forceFleet || selection.Count > 1);
            if ( body < 0 && (fleetLabel ? squad < 0 || !fleet.IsPlayerSquad( squad ) || fleet.ShipCount( squad ) == 0
                : ship < 0 || !fleet.Alive( ship ) || fleet.FactionOf( ship ) != FactionId.Sol) ) return;

            var target = fleet;
            int fleetId = fleetLabel ? target.FleetId( squad ) : -1;
            int generation = body < 0 && !fleetLabel ? target.ShipGeneration( ship ) : -1;
            string kind = body >= 0 ? "body" : fleetLabel ? "fleet" : "ship";
            string current = body >= 0 ? target.BodyName( body )
                : fleetLabel ? target.FleetName( squad ) ?? "" : target.ShipName( ship ) ?? "";

            var editor = CreateNameEditor( kind, current, name => {
                bool renamed = target == fleet && (body >= 0 ? target.RenameBody( body, name )
                    : fleetLabel ? target.RenameFleetById( fleetId, name )
                    : target.ShipGeneration( ship ) == generation && target.RenameShip( ship, name ));
                Notify( renamed ? "NAME UPDATED" : "RENAME TARGET NO LONGER AVAILABLE" );
            } );
            renameEditor = editor;
            editor.FormClosed += ( _, _ ) => renameEditor = null;
            editor.Show( window );
        }

        void OrganizeFleets( bool split ) {
            bool changed = split ? fleet.SplitFleets( selection.Ships ) : fleet.MergeFleets( selection.Ships );
            Notify( split ? changed ? "FLEET SPLIT INTO INDEPENDENT UNITS" : "SELECT SHIPS IN A MERGED FLEET"
                : changed ? "COMBAT GROUPS MERGED" : "SELECT TWO OR MORE COMBAT GROUPS" );
            if ( changed ) audio.Cue( "order" );
        }

        void BuildProject( int body, PlanetFacility facility ) {
            var builder = selection.SurveyUnit( fleet );
            if ( fleet.ConstructionBlock( body, facility, builder ) is { } reason ) { Notify( reason ); return; }
            if ( fleet.BuildFacility( body, facility, builder ) ) {
                buildCatalog = false;
                string crew = FleetSimulation.Facility( facility ).Layer == SiteLayer.Surface ? "LOCAL CREW WORKING"
                    : $"{UnitName( builder!.Value )} DISPATCHED";
                Notify( $"{crew} / {FleetHud.SiteName( facility )}" );
                audio.Cue( "order" );
            }
        }
        void CancelOrder( bool canceled ) {
            if ( !canceled ) return;
            Notify( "ORDER CANCELED / COST RETURNED" );
            audio.Cue( "order" );
        }

        void Notify( string text ) {
            if ( string.Equals( notices[0].Text, text, StringComparison.Ordinal ) ) return;
            notices.AsSpan( 0, notices.Length - 1 ).CopyTo( notices.AsSpan( 1 ) );
            notices[0] = new( text, Environment.TickCount64 );
        }

        string UnitName( FleetSelection unit ) => unit.IsShip ? fleet.ShipName( unit.Ship ) ?? FleetSimulation.RoleName( fleet.Role( unit.Squad ) )
            : fleet.FleetName( unit.Squad ) ?? fleet.FirstShipName( unit.Squad ) ?? FleetSimulation.RoleName( fleet.Role( unit.Squad ) );
        string SelectionName( ) => selection.SurveyUnit( fleet ) is { } unit ? UnitName( unit ) : $"{selection.Count} SHIPS";

        void PlanetOrder( PlanetAction action, int body ) {
            if ( action is PlanetAction.Inspect or PlanetAction.Construction ) {
                planet = body;
                yardInspector = -1;
                buildCatalog = action == PlanetAction.Construction;
                catalogLayer = SiteLayer.Orbit;
                return;
            }
            if ( action == PlanetAction.Close ) { planet = -1; yardInspector = -1; buildCatalog = false; return; }
            if ( fleet.Frontier && action is PlanetAction.Research or PlanetAction.Industry or PlanetAction.Drive ) {
                SelectTab( CommandTab.Research );
                return;
            }
            if ( fleet.Frontier && action is PlanetAction.Yard or PlanetAction.Mine or PlanetAction.Lab or PlanetAction.Depot ) {
                BuildProject( body, action switch {
                    PlanetAction.Yard => PlanetFacility.Yard,
                    PlanetAction.Mine => body > 0 && fleet.Colony( body ).Population > 0 ? PlanetFacility.Mine : PlanetFacility.Skimmer,
                    PlanetAction.Lab => PlanetFacility.Lab, _ => PlanetFacility.Depot
                } );
                return;
            }
            if ( action == PlanetAction.Flyby ) {
                if ( selection.Count == 0 ) { Notify( "SELECT SHIPS FOR A FLYBY" ); return; }
                if ( !selection.Issue( fleet, default, flyby: body ) ) { Notify( "NO FLYBY SOLUTION WITH CURRENT DRIVE" ); return; }
                Notify( $"VIA {fleet.BodyName( body )} TO {fleet.BodyName( fleet.FlybyDestination( body ) )}" );
                audio.Cue( "order" );
                return;
            }
            if ( action == PlanetAction.Orbit ) {
                if ( selection.Count == 0 ) { Notify( "SELECT SHIPS TO ORBIT" ); return; }
                if ( !selection.Issue( fleet, default, orbit: body ) ) { Notify( "OUTER BELT REQUIRES LONG BURN DRIVE" ); return; }
                Notify( $"{SelectionName( )} TRANSFER TO {fleet.BodyName( body )}" );
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
                if ( fleet.RequiresDrive( body ) && !fleet.DriveUpgrade ) { Notify( "OUTER BELT REQUIRES LONG BURN DRIVE" ); return; }
                if ( status.SurveyUnit is { } assigned ) { Notify( $"{UnitName( assigned )} ALREADY ASSIGNED" ); return; }
                if ( selection.SurveyUnit( fleet ) is not { } unit || !fleet.BeginSurvey( unit, body ) ) {
                    Notify( fleet.Frontier ? "SELECT YOUR SCIENCE SCOUT" : "SELECT ONE SHIP OR ONE SQUAD" );
                    return;
                }
                Notify( $"{UnitName( unit )} SURVEYING {fleet.BodyName( body )}" );
                audio.Cue( "survey-assign" );
                return;
            }
            if ( action == PlanetAction.Yard ) {
                if ( fleet.BuildYard( body ) ) {
                    Notify( "ORBITAL SHIPYARD UNDER CONSTRUCTION" );
                    audio.Cue( "order" );
                } else Notify( (fleet.Frontier ? fleet.YardAt( body ) >= 0 : fleet.YardBody >= 0) ? "SHIPYARD ALREADY ASSIGNED" : "SHIPYARD NEEDS A SURVEY AND 55 ORE" );
                return;
            }
            var (facility, cost, name) = action switch {
                PlanetAction.Mine => (PlanetFacility.Mine, FleetSimulation.MineCost, "MINE"),
                PlanetAction.Depot => (PlanetFacility.Depot, FleetSimulation.DepotCost, "DEPOT"),
                _ => (PlanetFacility.Lab, FleetSimulation.LabCost, "LAB")
            };
            var builder = selection.SurveyUnit( fleet );
            bool started = fleet.BuildFacility( body, facility, builder );
            Notify( started ? facility == PlanetFacility.Mine && fleet.Frontier ? "MINER DISPATCHED TO BUILD" : $"{name} CONSTRUCTION STARTED"
                : !status.Surveyed ? "SURVEY THIS BODY BEFORE BUILDING"
                : fleet.Ore < cost ? "INSUFFICIENT ORE FOR CONSTRUCTION"
                : facility == PlanetFacility.Mine && fleet.Frontier ? "SELECT AN AVAILABLE MINER AT A DEPOSIT" : "CONSTRUCTION SLOT UNAVAILABLE" );
            if ( started ) audio.Cue( "launch" );
        }

        int PickSite( Vector2 pixel ) {
            float best = MathF.Pow( MathF.Max( 11, viewport.Height * 0.013f ), 2 );
            int result = -1;
            for ( int i = 0; i < siteCount; ++i ) {
                float distance = Vector2.DistanceSquared( pixel, screenSites[i].Pixel );
                if ( distance >= best ) continue;
                best = distance;
                result = i;
            }
            return result;
        }

        int PickBody( Point pixel ) {
            int body = -1;
            float closest = 1;
            for ( int i = 0; i < fleet.Bodies.Length; ++i ) {
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

    private static Form CreateNameEditor( string kind, string current, Action<string> saveName ) {
        var editor = new Form {
            Text = $"Rename {kind}", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog,
            AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, AutoScaleMode = AutoScaleMode.Font,
            MinimizeBox = false, MaximizeBox = false, ShowInTaskbar = false
        };
        var layout = new TableLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding( 12 ) };
        var prompt = new Label { AutoSize = true, Text = "Use 1–18 letters, numbers, spaces or dashes." };
        var input = new TextBox { Text = current, MaxLength = 18, Anchor = AnchorStyles.Left | AnchorStyles.Right };
        var buttons = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Anchor = AnchorStyles.Right, WrapContents = false };
        var save = new Button { Text = "Save", AutoSize = true };
        var cancel = new Button { Text = "Cancel", AutoSize = true };
        save.Enabled = FleetSimulation.NormalizeName( input.Text ) is not null;
        input.TextChanged += ( _, _ ) => save.Enabled = FleetSimulation.NormalizeName( input.Text ) is not null;
        save.Click += ( _, _ ) => {
            if ( FleetSimulation.NormalizeName( input.Text ) is not { } name ) return;
            saveName( name );
            editor.Close( );
        };
        cancel.Click += ( _, _ ) => editor.Close( );
        buttons.Controls.AddRange( [save, cancel] );
        layout.Controls.Add( prompt, 0, 0 );
        layout.Controls.Add( input, 0, 1 );
        layout.Controls.Add( buttons, 0, 2 );
        editor.Controls.Add( layout );
        editor.AcceptButton = save;
        editor.CancelButton = cancel;
        editor.Shown += ( _, _ ) => {
            input.Focus( );
            input.SelectAll( );
        };
        return editor;
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
