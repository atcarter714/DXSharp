using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using FleetBattler;

namespace InteropQualification;

internal static class FleetQualification {
    internal static void Run( ) {
        var first = Replay( 37, 0 );
        Require( first == Replay( 37, 0 ), "the same seed and orders reproduce all checkpoints" );
        Require( first != Replay( 38, 0 ), "a different seed changes the state" );
        var changed = Replay( 37, 1 );
        Require( first.BeforeOrder == changed.BeforeOrder && first.AfterOrder != changed.AfterOrder,
            "a changed order first diverges after its tick" );

        VerifyFormations( );
        VerifyIndividualOrders( );
        VerifySkirmish( );
        VerifySelection( );
        VerifySurveyOrders( );
        VerifyClock( );
        VerifyPlanetEconomy( );
        VerifyEconomy( );
        VerifyOrbits( );
        VerifyAudio( );
        Console.WriteLine( $"PASS: fleet replay, selection modifiers, moving orbits, skirmish casualties, owned surveys, simulation pacing, orbital production, capture, and zero-allocation updates ({first.AfterOrder:X16})." );
    }

    private static void VerifyFormations( ) {
        var fleet = new FleetSimulation( 129, 37 );
        var previous = fleet.Positions.ToArray( );
        var instances = new ShipInstance[fleet.Count];
        int movedCount = fleet.ShipCount( 0 );
        Require( fleet.SquadCount == 12 && Marshal.SizeOf<ShipInstance>( ) == FleetDraw.InstanceStride, "squad count and instance ABI" );
        Require( fleet.FindSquad( fleet.Center( 0 ) ) == 0 && fleet.FindSquad( fleet.Center( 3 ) ) == 3, "independent cyan selection" );
        fleet.Move( 0, new( 8, -20 ) );
        for ( int i = 0; i < 420; ++i ) fleet.Step( );
        fleet.FillInstances( instances, new( 0 ) );
        var moved = fleet.Positions[0] - previous[0];
        Require( moved.X > 35, "move order advances only the selected squad" );
        for ( int i = 0; i < fleet.Count; ++i ) {
            var instance = instances[i];
            Require( instance.Center == fleet.Positions[i] && MathF.Abs( instance.Heading.LengthSquared( ) - 1 ) < 1e-5f,
                "instance data preserves position and a normalized heading" );
            if ( i < movedCount ) {
                Require( MathF.Abs( Vector2.Distance( fleet.Positions[i], fleet.Positions[0] ) - Vector2.Distance( previous[i], previous[0] ) ) < 0.02f,
                    "the ordered squad preserves formation spacing" );
                Require( instance.Tint.Z > instance.Tint.X && instance.Tint.W > 1.4f, "selected squad tint" );
            } else {
                Require( fleet.Positions[i] == previous[i] && instance.Tint.W < 1.4f, "other squads stay in place" );
            }
        }

        int corvettes = fleet.FillInstances( instances, new( 0 ), groupHulls: true );
        Require( corvettes == fleet.Count - fleet.ShipCount( 0 ) - fleet.ShipCount( 3 ) &&
            instances.Take( corvettes ).All( item => item.Motion.Z == 1 ) &&
            instances.Skip( corvettes ).All( item => item.Motion.Z == 0.75f ) &&
            instances[corvettes].Center == fleet.Positions[0], "contiguous class batches preserve ship positions" );

        long start = GC.GetAllocatedBytesForCurrentThread( );
        for ( int i = 0; i < 120; ++i ) {
            fleet.Step( );
            fleet.FillInstances( instances, new( 1 ), groupHulls: true );
        }
        Require( GC.GetAllocatedBytesForCurrentThread( ) == start, "steady-state stepping and packing allocate zero bytes" );
    }

    private static void VerifyIndividualOrders( ) {
        var fleet = new FleetSimulation( 72, 37, system: 2 );
        var old = fleet.Positions.ToArray( );
        var selected = fleet.Pick( old[0], group: false, 1 );
        Require( selected == new FleetSelection( 0, 0 ) && fleet.Pick( old[0], group: true, 1 ) == new FleetSelection( 0 ),
            "pick an individual ship or its squad" );
        var waypoint = old[0] + new Vector2( 18, 13 );
        fleet.Move( selected, waypoint );
        var shipData = new ShipInstance[fleet.Count];
        for ( int i = 0; i < 25; ++i ) fleet.Step( );
        fleet.FillInstances( shipData, selected );
        Require( shipData[0].Motion.X > 0.12f && shipData[1].Motion.X == 0 && shipData[0].Heading.Y > 0.12f,
            "individual steering turns and drives only the selected ship's thrusters" );
        AdvanceUntil( fleet, () => Vector2.DistanceSquared( fleet.Positions[0], waypoint ) < 0.01f, "solo ship reaches its waypoint" );
        Require( old.AsSpan( 1 ).SequenceEqual( fleet.Positions[1..] ), "remaining ships hold formation" );

        fleet.Orbit( selected, 1 );
        for ( int i = 0; i < 450; ++i ) fleet.Step( );
        var a = fleet.Positions[0] - fleet.Bodies[1].Center;
        for ( int i = 0; i < 45; ++i ) fleet.Step( );
        var b = fleet.Positions[0] - fleet.Bodies[1].Center;
        float radius = fleet.OrbitRadius( 1, 1 );
        Require( fleet.Orbiting( selected ) && MathF.Abs( b.Length( ) - radius ) < 2.5f &&
            Vector2.Distance( a, b ) > 3, "ship tracks a moving body's orbit" );
        fleet.Rejoin( selected );
        AdvanceUntil( fleet, () => Vector2.DistanceSquared( fleet.Positions[0], old[0] ) < 0.01f, "solo ship rejoins its slot" );
        Require( !fleet.Orbiting( selected ), "rejoin clears the orbit order" );

        var group = new FleetSelection( 0 );
        fleet.Orbit( group, 2 );
        for ( int i = 0; i < 600; ++i ) fleet.Step( );
        Require( fleet.Orbiting( group ) &&
            MathF.Abs( Vector2.Distance( fleet.Center( group ), fleet.Bodies[2].Center ) - fleet.OrbitRadius( 2, fleet.ShipCount( 0 ) ) ) < 4,
            "squadron steers as a group around the ringed planet" );
    }

    private static void VerifySkirmish( ) {
        var fleet = new FleetSimulation( 18, 37, battle: true );
        int target = 9;
        var selection = new CommandSelection( fleet.Capacity );
        selection.All( fleet );
        selection.Issue( fleet, fleet.Site, hostile: target );
        var pulses = new BattleEffect[FleetSimulation.EffectCapacity];
        AdvanceUntil( fleet, () => fleet.ShotsFired > 0, "explicit attack closes weapon range" );
        Require( fleet.InBattle && fleet.FillEffects( pulses ) > 0, "live weapon pulses" );
        AdvanceUntil( fleet, () => !fleet.Alive( target ), "focused enemy ship is destroyed" );
        Require( fleet.ActiveCount < fleet.Count && fleet.ShipsLost == fleet.Count - fleet.ActiveCount, "stable IDs and casualty accounting" );
        Require( fleet.FillEffects( pulses ) > 0 && pulses.Any( x => x.Style.Z == 1 ), "destruction burst" );
        var data = new ShipInstance[fleet.ActiveCount];
        selection.Prune( fleet );
        fleet.FillInstances( data, selection.Focus( fleet ), selection.Ships );
        Require( selection.Count == fleet.CyanCount && data.All( x => x.Motion.W > 0 ), "dead ships excluded from selection and rendering" );

        var rival = new FleetSimulation( 72, 37, rivalPatrol: true, battle: true );
        var amberStart = rival.Center( 6 );
        rival.Move( 0, rival.Site );
        rival.Move( 2, rival.Site );
        Require( rival.BeginSurvey( new( 4 ), 1 ), "rival skirmish survey assignment" );
        bool mine = false, lab = false, yard = false;
        int launched = 0;
        for ( int tick = 0; tick < 4_800 && !rival.Lost && (rival.WingsBuilt < FleetSimulation.WingGoal || !rival.WeaponUpgrade); ++tick ) {
            rival.Step( );
            if ( !mine ) mine = rival.BuildFacility( 1, PlanetFacility.Mine );
            if ( mine && !lab ) lab = rival.BuildFacility( 1, PlanetFacility.Lab );
            if ( lab && !yard ) yard = rival.BuildYard( 1 );
            if ( yard ) rival.QueueWing( );
            if ( launched != rival.WingsBuilt ) {
                launched = rival.WingsBuilt;
                rival.Move( rival.SquadCount - 1, rival.Site );
            }
            if ( !rival.WeaponUpgrade ) rival.ResearchWeapons( );
            if ( tick == 900 ) rival.Move( 1, rival.Site );
            if ( tick == 1_320 ) rival.Move( 3, rival.Site );
        }
        Require( Vector2.DistanceSquared( rival.Center( 6 ), amberStart ) > 30 && rival.ShotsFired > 12 &&
            Enumerable.Range( 0, 36 ).Any( i => rival.Hull( i ) < FleetSimulation.HullCapacity ), "rival squad advances and lands damage" );
        Require( rival.Won && rival.WingsBuilt == FleetSimulation.WingGoal && rival.WeaponUpgrade && rival.ShipsLost > 0,
            "complete the default skirmish while developing a colony and fighting the rival" );
    }

    private static void VerifySelection( ) {
        var fleet = new FleetSimulation( 72, 37 );
        var selection = new CommandSelection( fleet.Capacity );
        var dimensions = new Vector2( 1_280, 800 );
        var matrix = Matrix4x4.CreateLookAt( new( 0, 145, 80 ), Vector3.Zero, Vector3.UnitY ) *
            Matrix4x4.CreateOrthographic( 250, 165, 0.1f, 380 );
        var p = CommandSelection.Project( fleet.Positions[0], matrix, dimensions );
        Require( CommandSelection.Hit( fleet, p, matrix, dimensions, 0 ) == 0, "point picking uses camera projection" );
        selection.Box( fleet, new( p.X - 2, p.Y - 2, p.X + 2, p.Y + 2 ), matrix, dimensions, SelectionMode.Replace );
        Require( selection.Count == 1 && selection.Primary == 0, "drag replaces selection" );
        selection.Click( fleet, 1, false, SelectionMode.Add );
        selection.Click( fleet, 0, false, SelectionMode.Toggle );
        Require( selection.Count == 1 && selection.Primary == 1, "shift adds and control toggles" );
        selection.Click( fleet, 1, true, SelectionMode.Replace );
        Require( selection.Count == 6 && selection.Focus( fleet ).Squad == 0, "double click selects a squad" );
        selection.Box( fleet, new( 0, 0, dimensions.X, dimensions.Y ), matrix, dimensions, SelectionMode.Add );
        Require( selection.Count == fleet.CyanCount, "box selection excludes hostile units" );
        Require( FleetHud.Hit( new( 510, 179 ), 640, 400, fleet, 1 ) == new HudHit( HudAction.Planet, (int)PlanetAction.Mine ) &&
            FleetHud.Hit( new( 30, 94 ), 640, 400, fleet, 1 ) == new HudHit( HudAction.Squad, 0 ) &&
            FleetHud.Hit( new( 485, 23 ), 640, 400, fleet, 1 ).Action == HudAction.Pause &&
            FleetHud.Hit( new( 510, 170 ), 640, 400, fleet, 1 ).Action == HudAction.Panel &&
            FleetHud.Hit( new( 320, 200 ), 640, 400, fleet, 1 ).Action == HudAction.None,
            "HUD routes buttons, squad rows and panel gaps without issuing scene orders" );
        Require( FleetHud.Hit( new( 1_020, 358 ), 1_280, 800, fleet, 1 ) == new HudHit( HudAction.Planet, (int)PlanetAction.Mine ),
            "HUD scales its hit regions with the viewport" );
        var popup = new BodyMenu( new( 320, 200 ), 1 );
        Require( FleetHud.Hit( new( 348, 263 ), 640, 400, fleet, 1, popup ) ==
            new HudHit( HudAction.Context, (int)PlanetAction.Survey ) &&
            FleetHud.Hit( new( 510, 179 ), 640, 400, fleet, 1, popup ).Action == HudAction.None,
            "context menu takes precedence over world and panel input" );
    }

    private static void VerifySurveyOrders( ) {
        var fleet = new FleetSimulation( 72, 37 );
        const int body = 1;
        var selection = new CommandSelection( fleet.Capacity );
        selection.Squadron( fleet, 0, SelectionMode.Replace );
        Require( selection.SurveyUnit( fleet ) == new FleetSelection( 0 ), "whole squad is a survey unit" );
        selection.Squadron( fleet, 1, SelectionMode.Add );
        Require( selection.SurveyUnit( fleet ) is null, "multi-squad selection needs an explicit survey unit" );
        selection.Click( fleet, 0, false, SelectionMode.Replace );
        Require( selection.SurveyUnit( fleet ) == new FleetSelection( 0, 0 ), "single ship is a survey unit" );

        fleet.Orbit( new( 2 ), body );
        AdvanceUntil( fleet, () => Vector2.Distance( fleet.Center( 2 ), fleet.Bodies[body].Center ) < fleet.OrbitRadius( body, fleet.ShipCount( 2 ) ) + 2,
            "unassigned bystander arrives near the world" );
        Require( fleet.Planet( body ).Survey == 0, "orbiting alone earns no progress" );

        var scout = new FleetSelection( 0, 0 );
        Require( fleet.BeginSurvey( scout, body ) && fleet.Plan( scout ) is { Order: FlightOrder.Survey, Body: body } &&
            fleet.Planet( body ).SurveyUnit == scout && !fleet.BeginSurvey( new( 2 ), body ),
            "survey owns one explicit ship and an immediate flight plan" );
        fleet.Step( );
        Require( fleet.Planet( body ).Survey == 0, "an orbiting bystander cannot scan for an assigned scout" );
        AdvanceUntil( fleet, () => fleet.Planet( body ).Survey > 0, "assigned scout arrives" );
        int progress = fleet.Planet( body ).Survey;
        fleet.Step( );
        Require( fleet.Planet( body ).Survey == progress + 2, "scout scans at twice the normal rate" );
        fleet.Move( scout, Vector2.Zero );
        Require( fleet.Planet( body ).SurveyUnit is null && fleet.Plan( scout ).Order == FlightOrder.Move, "overriding a survey releases its assignment" );
        int retained = fleet.Planet( body ).Survey;
        for ( int i = 0; i < 30; ++i ) fleet.Step( );
        Require( fleet.Planet( body ).Survey == retained, "unassigned survey progress remains paused" );
        Require( fleet.BeginSurvey( new( 2 ), body ) && fleet.CancelSurvey( body ) &&
            fleet.Plan( new( 2 ) ).Order == FlightOrder.Orbit && fleet.Planet( body ).SurveyUnit is null, "explicit cancellation retains orbit only" );
        Require( fleet.BeginSurvey( new( 2 ), body ), "reassignment resumes survey progress" );
        AdvanceUntil( fleet, () => fleet.Planet( body ).Surveyed, "assigned corvette squad completes the survey" );
        Require( fleet.CompletedSurveys == 1 && fleet.LastSurveyBody == body && fleet.LastSurveyUnit == new FleetSelection( 2 ) &&
            fleet.Planet( body ).SurveyUnit is null && fleet.Ore == 40, "completion is attributed once and pays the reward" );
    }

    private static void VerifyClock( ) {
        var fleet = new FleetSimulation( 12, 37 );
        var clock = new FleetClock( );
        fleet.Move( new FleetSelection( 0, 0 ), Vector2.Zero );
        for ( int frame = 0; frame < 120; ++frame ) clock.Advance( fleet, 1.0 / 60, 1, paused: false );
        Require( fleet.Tick is 29 or 30 && clock.Interpolation is >= 0 and <= 1 &&
            fleet.VisualPosition( 0, 0 ) != fleet.VisualPosition( 0, 1 ), "1x advances at quarter speed with a presentation interval" );
        int before = fleet.Tick;
        float interpolation = clock.Interpolation;
        clock.Advance( fleet, 1, 4, paused: true );
        Require( fleet.Tick == before && clock.Interpolation == interpolation, "pause freezes both simulation and interpolated presentation" );
        for ( int frame = 0; frame < 120; ++frame ) clock.Advance( fleet, 1.0 / 60, 4, paused: false );
        Require( fleet.Tick - before is 119 or 120, "4x reaches the original simulation speed" );
    }

    private static void VerifyPlanetEconomy( ) {
        var fleet = new FleetSimulation( 12, 37 );
        const int body = 1;
        Require( !fleet.BuildFacility( body, PlanetFacility.Mine ), "construction needs exploration" );
        Require( fleet.BeginSurvey( new( 2 ), body ), "ship group receives survey order" );
        AdvanceUntil( fleet, () => fleet.Planet( body ).Surveyed, "survey completes while the team follows the planet" );
        Require( fleet.Ore == 40 && !fleet.BuildFacility( 0, PlanetFacility.Mine ), "survey payout and star construction exclusion" );
        Require( fleet.BuildFacility( body, PlanetFacility.Mine ) && !fleet.BuildFacility( body, PlanetFacility.Mine ) &&
            fleet.Ore == 5 && fleet.Planet( body ).MineRemaining == FleetSimulation.FacilityDuration, "mine cost and construction queue" );
        AdvanceUntil( fleet, () => fleet.Ore >= FleetSimulation.LabCost, "active mine generates ore" );
        Require( fleet.BuildFacility( body, PlanetFacility.Lab ) && fleet.Science == 0, "lab begins construction" );
        AdvanceUntil( fleet, () => fleet.Science >= FleetSimulation.UpgradeCost, "active lab produces science" );
        int bank = fleet.Science;
        Require( fleet.ResearchWeapons( ) && fleet.WeaponUpgrade && fleet.Science == bank - FleetSimulation.UpgradeCost &&
            !fleet.ResearchWeapons( ), "one-time science-funded weapons upgrade" );
    }

    private static void VerifyAudio( ) {
        using var custom = JsonDocument.Parse( """
            {"playlists":{"system":["system-theme.mp3"],"galaxy":["chart.wav"],"contested":["@signal"],"victory":["@arrival"]}}
            """ );
        var jukebox = Jukebox.Parse( custom.RootElement );
        Require( ReferenceEquals( jukebox.Tracks( 0 ), jukebox.Tracks( 2 ) ) &&
            ReferenceEquals( jukebox.Tracks( 0 ), jukebox.Tracks( 3 ) ) &&
            jukebox.Tracks( 1 ).SequenceEqual( new[] { "chart.wav" } ) &&
            jukebox.Fallback( 3 ) == "@survey", "custom music continues through default battle and victory playlists" );
        using var optIn = JsonDocument.Parse( """
            {"useProceduralTransitions":true,"playlists":{"system":["my-track.wav"],"contested":["@signal"]}}
            """ );
        Require( Jukebox.Parse( optIn.RootElement ).Tracks( 2 ).SequenceEqual( new[] { "@signal" } ),
            "explicit procedural transitions still work" );
        Require( new Jukebox( ).Tracks( 1 ).SequenceEqual( new[] { "@cartography" } ), "default soundtrack still has context" );

        foreach ( string name in new[] { "survey", "cartography", "signal", "arrival", "order", "launch", "capture", "laser", "burst" } ) {
            using var stream = new MemoryStream( );
            SynthAudio.WriteWave( stream, name );
            var data = stream.GetBuffer( ).AsSpan( 0, (int)stream.Length );
            Require( data[..4].SequenceEqual( "RIFF"u8 ) && data.Slice( 8, 8 ).SequenceEqual( "WAVEfmt "u8 ) &&
                System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian( data[24..] ) == SynthAudio.SampleRate &&
                System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian( data[40..] ) == data.Length - 44 &&
                data[44..].IndexOfAnyExcept( (byte)0 ) >= 0, "procedural cue or track is a non-silent PCM WAV" );
        }
    }

    private static void VerifyEconomy( ) {
        var fleet = new FleetSimulation( 12, 37 );
        Require( !fleet.QueueWing( ) && fleet.Count == 12 && fleet.Owner == -1, "shipyard gate at neutral start" );
        fleet.Move( 0, fleet.Site );
        AdvanceUntil( fleet, () => fleet.Capturing == 0, "cyan enters the beacon" );
        Require( fleet.CaptureTicks == 1 && fleet.Owner == -1, "capture requires sustained presence" );
        AdvanceUntil( fleet, () => fleet.Owner == 0, "cyan captures the beacon" );
        AdvanceUntil( fleet, () => fleet.Ore >= FleetSimulation.WingCost, "owned beacon generates ore" );
        Require( !fleet.QueueWing( ) && !fleet.BuildYard( 1 ), "ore alone cannot build wings or bypass the survey" );
        Require( fleet.BeginSurvey( new( 2 ), 1 ), "survey crew dispatched" );
        AdvanceUntil( fleet, () => fleet.Planet( 1 ).Surveyed, "site survey completes" );
        int bank = fleet.Ore;
        Require( fleet.BuildYard( 1 ) && fleet.Ore == bank - FleetSimulation.YardCost &&
            fleet.YardRemaining == FleetSimulation.YardDuration && !fleet.QueueWing( ) && !fleet.BuildYard( 1 ),
            "a single shipyard costs ore and must finish construction" );
        Require( fleet.BuildFacility( 1, PlanetFacility.Lab ), "lab starts before production research" );
        AdvanceUntil( fleet, () => fleet.YardRemaining == 0 && fleet.Ore >= FleetSimulation.WingCost, "yard online and funds available" );

        bank = fleet.Ore;
        Require( fleet.QueueWing( ) && fleet.Ore == bank - FleetSimulation.WingCost && fleet.QueuedWings == 1 &&
            fleet.WingRemaining == FleetSimulation.StandardWingDuration && fleet.Count == 12 && !fleet.Won,
            "queue debits ore immediately and reserves capacity without spawning" );
        AdvanceUntil( fleet, () => fleet.Science >= FleetSimulation.IndustryCost && fleet.Ore >= FleetSimulation.ImprovedWingCost,
            "lab and relay fund the production upgrade" );
        Require( fleet.WingsBuilt == 0 && fleet.ResearchIndustry( ) &&
            fleet.ActiveWingDuration == FleetSimulation.StandardWingDuration, "industry upgrades future jobs while preserving active contract" );
        bank = fleet.Ore;
        Require( fleet.QueueWing( ) && fleet.Ore == bank - FleetSimulation.ImprovedWingCost && fleet.QueuedWings == 2 &&
            !fleet.QueueWing( ), "improved wing costs less and waits behind active production" );
        AdvanceUntil( fleet, () => fleet.WingsBuilt == 1, "first wing launches" );
        int firstWing = 12;
        Require( fleet.Count == 18 && fleet.ShipCount( firstWing ) == FleetSimulation.WingSize &&
            Vector2.Distance( fleet.Center( firstWing ), fleet.Shipyard ) < 4.01f && fleet.QueuedWings == 1 &&
            fleet.WingRemaining == FleetSimulation.ImprovedWingDuration && !fleet.Won, "wing launches from orbit before next job begins" );

        fleet.Move( 6, fleet.Site );
        AdvanceUntil( fleet, () => fleet.Contested, "amber contests the beacon" );
        bank = fleet.Ore;
        for ( int i = 0; i < 60; ++i ) fleet.Step( );
        Require( fleet.Contested && fleet.Ore == bank && fleet.Owner == 0 && fleet.Capturing == -1, "equal presence stalls relay income" );
        AdvanceUntil( fleet, () => fleet.WingsBuilt == FleetSimulation.WingGoal, "upgraded wing finishes" );
        Require( fleet.Count == fleet.Capacity && fleet.QueuedWings == 0 && fleet.Won && !fleet.QueueWing( ),
            "victory counts completed wings and respects spawn capacity" );
    }

    private static void AdvanceUntil( FleetSimulation fleet, Func<bool> condition, string name, int limit = 900 ) {
        for ( int i = 0; i < limit && !condition( ); ++i ) fleet.Step( );
        Require( condition( ), name );
    }

    private static void VerifyOrbits( ) {
        const float axis = 40, e = CelestialSystem.RockyEccentricity;
        var near = CelestialSystem.OrbitPoint( axis, e, 0 );
        var far = CelestialSystem.OrbitPoint( axis, e, 0.5 );
        Require( Vector2.Distance( near, new( axis * (1 - e), 0 ) ) < 0.00001f &&
            Vector2.Distance( far, new( -axis * (1 + e), 0 ) ) < 0.00001f, "ellipse apsides" );
        var point = CelestialSystem.OrbitPoint( axis, e, 0.25 );
        Require( Vector2.Distance( point, CelestialSystem.OrbitPoint( axis, e, -1.75 ) ) < 0.00001f && point.Y > 0,
            "normalized orbital phase wraps across negative and positive cycles" );
    }

    private static (ulong BeforeOrder, ulong AfterOrder) Replay( uint seed, float orderDelta ) {
        var fleet = new FleetSimulation( 1025, seed );
        fleet.Move( 1, new( 11, 20 ) );
        for ( int i = 0; i < 180; ++i ) fleet.Step( );
        ulong checkpoint = fleet.StateHash( );
        fleet.Move( 0, new( 15 + orderDelta, -35 ) );
        for ( int i = 0; i < 480; ++i ) fleet.Step( );
        Require( fleet.Tick == 660, "fixed tick count" );
        return (checkpoint, fleet.StateHash( ));
    }

    private static void Require( bool condition, string name ) {
        if ( !condition ) throw new InvalidOperationException( $"Fleet qualification failed: {name}." );
    }
}
