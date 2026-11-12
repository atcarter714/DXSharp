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
        VerifyFrontier( );
        VerifyFleetRecovery( );
        VerifyOrbitalTransfers( );
        VerifyProjectCancellation( );
        VerifyOrbits( );
        VerifyAudio( );
        Console.WriteLine( $"PASS: fleet replay, selection modifiers, moving orbits, skirmish casualties, owned surveys, simulation pacing, orbital production, frontier progression, fleet recovery, transfer guidance, work cancellation, capture, and zero-allocation updates ({first.AfterOrder:X16})." );
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

    private static void VerifyFrontier( ) {
        const int prospect = 2;
        var fleet = new FleetSimulation( 3, 37, battle: true, frontier: true );
        var scout = new FleetSelection( 0, 0 );
        var miner = new FleetSelection( 1, 1 );
        Require( fleet.CyanCount == 3 && fleet.AmberCount == 0 && !fleet.Won && fleet.Planet( FleetSimulation.Homeworld ).Surveyed &&
            fleet.Role( 0 ) == HullRole.Scout && fleet.Role( 1 ) == HullRole.Miner && fleet.Role( 2 ) == HullRole.Miner &&
            fleet.Dispatch( 0 ) == FrontierEvent.Arrival, "homeworld, science ship and workers open in peacetime" );
        Require( !fleet.QueueWing( ) && !fleet.BuildFacility( prospect, PlanetFacility.Mine, miner ) &&
            !fleet.BeginSurvey( miner, prospect ) && fleet.BeginSurvey( scout, prospect ), "exploration and production gates enforce roles" );
        Require( fleet.QueueWorker( ) && fleet.Ore == 30 && fleet.WorkerRemaining == FleetSimulation.WorkerDuration &&
            !fleet.QueueWorker( ) && fleet.CyanCount == 3, "the homeworld charges and schedules a worker" );
        AdvanceUntil( fleet, () => fleet.HasDeposit( prospect ), "scout reaches and surveys the hidden deposit", 1800 );
        Require( fleet.DepositYield( prospect ) == 12 && fleet.Ore == 45 && fleet.Dispatch( 1 ) == FrontierEvent.Prospect &&
            !fleet.BuildFacility( prospect, PlanetFacility.Mine, scout ), "the survey reveals a specific yield and miners alone can build" );
        Require( fleet.BuildFacility( prospect, PlanetFacility.Mine, miner ) && fleet.Ore == 10 &&
            fleet.MineEnRoute( prospect ), "placing a mine reserves ore and assigns a named worker" );
        fleet.Step( );
        Require( fleet.Planet( prospect ).MineRemaining == FleetSimulation.FacilityDuration, "travel does not consume construction time" );
        fleet.Move( miner, new( -38, 10 ) );
        fleet.Step( );
        Require( fleet.Planet( prospect ).MineRemaining < 0 && fleet.Ore == 45 &&
            fleet.BuildFacility( prospect, PlanetFacility.Mine, miner ), "reassigning a worker refunds the unfinished mine" );
        AdvanceUntil( fleet, () => fleet.Planet( prospect ).MineRemaining == 0, "arrival starts and completes construction", 1800 );
        Require( fleet.CyanCount == 4 && fleet.WorkerRemaining < 0 && fleet.Role( 3 ) == HullRole.Miner &&
            fleet.Dispatch( 2 ) == FrontierEvent.Distress && fleet.AmberCount == 0, "workers launch after construction and mining triggers a warning" );
        fleet.Move( scout, new( -70, 40 ) );
        fleet.Move( miner, new( -70, 45 ) );
        fleet.Move( 2, new( -70, 50 ) );
        fleet.Move( 3, new( -70, 55 ) );
        AdvanceUntil( fleet, () => fleet.Ore >= FleetSimulation.YardCost, "operating mine pays for a yard" );
        int bank = fleet.Ore;
        Require( fleet.BuildYard( FleetSimulation.Homeworld ) && fleet.YardRemaining == FleetSimulation.YardDuration &&
            fleet.Ore == bank - FleetSimulation.YardCost && !fleet.QueueWing( ), "building a yard commits cost and blocks production during construction" );
        fleet.ConfigureDesign( weapon: true );
        ShipDesign design = fleet.Design;
        AdvanceUntil( fleet, () => fleet.CanBuildWing, "mine funds the first corvette wing" );
        bank = fleet.Ore;
        int before = fleet.CyanCount;
        Require( fleet.QueueWing( ) && fleet.Ore == bank - 60 && fleet.WingRemaining == 480 &&
            fleet.CyanCount == before && fleet.ActiveWingDesign == design, "rail design is priced and captured when queued" );
        fleet.ConfigureDesign( weapon: true );
        fleet.ConfigureDesign( weapon: false );
        AdvanceUntil( fleet, () => fleet.WingsBuilt == 1, "first queued wing launches" );
        int corvette = Enumerable.Range( 0, fleet.Count ).First( ship => fleet.Loadout( ship ) == design );
        Require( fleet.ActiveRaids == 1 && fleet.ShipCount( fleet.SquadronOf( corvette ) ) == 3 &&
            fleet.Shield( corvette ) == 34 && fleet.Armor( corvette ) == 10 &&
            fleet.CurrentWingCost == 68 && fleet.CurrentWingDuration == 540 && !fleet.Won, "the launch uses the queued loadout and the first raid is in play" );

        bool sawRailBypass = false, sawPulseAbsorption = false;
        int combatLimit = fleet.Tick + 3600;
        while ( !fleet.Won && !fleet.Lost && fleet.Tick < combatLimit ) {
            if ( fleet.CanBuildWing ) Require( fleet.QueueWing( ), "a second corvette wing can reinforce the defense" );
            if ( fleet.Tick % 30 == 0 && fleet.AmberCount > 0 ) {
                int target = Enumerable.Range( 0, fleet.Count ).First( ship => fleet.Alive( ship ) && fleet.SideOf( ship ) == 1 );
                for ( int squad = 0; squad < fleet.SquadCount; ++squad )
                    if ( fleet.IsPlayerSquad( squad ) && fleet.Role( squad ) == HullRole.Corvette && fleet.ShipCount( squad ) > 0 )
                        fleet.Attack( new( squad ), target );
            }
            fleet.Step( );
            for ( int ship = 0; ship < fleet.Count; ++ship ) {
                if ( !fleet.Alive( ship ) || fleet.Role( fleet.SquadronOf( ship ) ) != HullRole.Corvette ) continue;
                var loadout = fleet.Loadout( ship );
                sawRailBypass |= fleet.SideOf( ship ) == 1 && fleet.Armor( ship ) < loadout.Armor && fleet.Shield( ship ) == loadout.Shield;
                sawPulseAbsorption |= fleet.SideOf( ship ) == 0 && fleet.ActiveRaids == 1 &&
                    fleet.Shield( ship ) < loadout.Shield && fleet.Armor( ship ) == loadout.Armor;
            }
        }
        Require( sawRailBypass && sawPulseAbsorption, "rail bypasses shields and pulse strips them before reaching armor" );
        Require( fleet.Won && fleet.ActiveRaids == 2 && fleet.Dispatch( fleet.DispatchCount - 1 ) == FrontierEvent.Secured,
            "the player can overcome both escalating raids and secure the frontier" );
        int miners = fleet.ActiveMiners( );
        Require( fleet.QueueWorker( ), "the homeworld can continue growing after victory" );
        AdvanceUntil( fleet, () => fleet.ActiveMiners( ) > miners, "a post-victory worker leaves the queue" );
        Require( fleet.ActiveMiners( ) == miners + 1 && fleet.ActiveRaids == 2, "post-victory production does not restart raids" );
    }

    private static void VerifyFleetRecovery( ) {
        const int home = FleetSimulation.Homeworld, prospect = 2;
        var fleet = new FleetSimulation( 3, 37, frontier: true );
        Require( fleet.BeginSurvey( new( 0 ), prospect ), "recovery test begins a survey" );
        AdvanceUntil( fleet, () => fleet.HasDeposit( prospect ), "recovery prospect surveyed", 1800 );
        Require( fleet.BuildFacility( prospect, PlanetFacility.Mine, new( 1 ) ), "recovery funding" );
        AdvanceUntil( fleet, () => fleet.Ore >= 235, "mine funds fleet and infrastructure", 3000 );
        Require( fleet.BuildYard( home ) && fleet.BuildFacility( home, PlanetFacility.Depot ) &&
            fleet.BuildYard( prospect ) && fleet.NavalCapacity == 0 && !fleet.CanBuildWing,
            "incomplete infrastructure cannot grant capacity" );
        AdvanceUntil( fleet, () => fleet.DepotCount == 1, "logistics depot constructed" );
        Require( fleet.NavalCapacity == FleetSimulation.DepotNavalCapacity && !fleet.CanBuildWing, "depot alone supplies no shipyard" );
        AdvanceUntil( fleet, () => fleet.YardCount == 2, "both yards constructed" );
        Require( fleet.NavalCapacity == 15 && fleet.QueueWing( prospect ) && fleet.ActiveYard == prospect &&
            fleet.QueueWing( home ) && fleet.NavalReserved == 6 && !fleet.CanBuildWing, "queue reserves actual hull slots and stores its yard" );
        for ( int ship = 0; ship < fleet.Count; ++ship )
            if ( fleet.SideOf( ship ) == 0 ) fleet.ApplyHullDamage( ship, FleetSimulation.HullCapacity );
        Require( fleet.Recovering && !fleet.Lost && fleet.NavalReserved == 6, "total loss leaves infrastructure and queued orders intact" );
        AdvanceUntil( fleet, () => fleet.WingsBuilt == 2, "both yards can fulfill work after a fleet wipe", 1300 );
        Require( fleet.NavalUsed == 6 && fleet.NavalReserved == 0 && !fleet.Recovering, "launch converts reserved capacity to live hulls" );
        var chosen = new CommandSelection( fleet.Capacity );
        chosen.All( fleet );
        int storage = fleet.Count;
        for ( int iteration = 0; iteration < 35; ++iteration ) {
            for ( int ship = 0; ship < fleet.Count; ++ship )
                if ( fleet.SideOf( ship ) == 0 ) fleet.ApplyHullDamage( ship, FleetSimulation.HullCapacity );
            Require( fleet.NavalUsed == 0 && fleet.Recovering && fleet.QueueWing( home ), "losses release naval capacity for every replacement" );
            int built = fleet.WingsBuilt;
            AdvanceUntil( fleet, () => fleet.WingsBuilt > built, "replacement wing launches" );
        }
        chosen.Prune( fleet );
        Require( chosen.Count == 0 && fleet.Count == storage && fleet.NavalUsed == 3 && fleet.WingsBuilt == 37,
            "replacement recycles storage without inheriting a dead hull's selection" );
        Require( fleet.QueueScout( ), "destroyed scout can be ordered from the homeworld" );
        AdvanceUntil( fleet, () => fleet.WorkerRemaining < 0, "replacement scout launches" );
        Require( fleet.CyanCount == 4 && fleet.IsScoutSquad( 0 ), "replacement civilian restores scouting without occupying naval capacity" );
    }

    private static void VerifyOrbitalTransfers( ) {
        var fleet = new FleetSimulation( 3, 37, frontier: true );
        var scout = new FleetSelection( 0 );
        const int belt = CelestialSystem.OuterBelt;
        Require( fleet.Bodies.Length == 11 && fleet.BodyName( 1 ) == "EARTH" && fleet.BodyName( 10 ) == "NEPTUNE" &&
            SolarCatalog.Get( 3 ).Parent == FleetSimulation.Homeworld && !fleet.Orbit( scout, belt ) && !fleet.BeginSurvey( scout, belt ) &&
            !fleet.Move( scout, fleet.Bodies[belt].Center ), "base drive rejects out-of-range orders without changing the orbit" );
        Require( fleet.BuildFacility( FleetSimulation.Homeworld, PlanetFacility.Lab ), "homeworld lab funds navigation research" );
        AdvanceUntil( fleet, () => fleet.Science >= FleetSimulation.DriveCost, "lab produces drive research" );
        Require( fleet.ResearchDrive( ) && fleet.BeginSurvey( scout, belt ), "long-burn drive unlocks belt rendezvous" );
        int end = fleet.Tick + fleet.RouteTicks( scout );
        Span<Vector2> route = stackalloc Vector2[17];
        Require( fleet.RoutePhase( scout ) == NavigationPhase.Burn && fleet.CopyRoute( scout, route, out var endpoint ) &&
            route[0] == fleet.Center( scout ) && route[^1] == endpoint && fleet.Plan( scout ).Destination == endpoint,
            "displayed route and destination use the actual rendezvous calculation" );
        Vector2 expected = fleet.Plan( scout ).Destination;
        var originalBelt = fleet.Bodies[belt].Center;
        bool sawCoast = false, sawBrake = false;
        Span<ShipInstance> visual = stackalloc ShipInstance[3];
        Span<BattleEffect> preview = stackalloc BattleEffect[BattleDraw.RouteCapacity];
        fleet.FillInstances( visual, scout );
        Vector2 previousHeading = visual[0].Heading;
        int initialSegments = BattleDraw.FillRoute( fleet, scout, preview, 1 );
        Require( initialSegments == BattleDraw.RouteCapacity && preview[initialSegments - 1].Segment.X == expected.X,
            "rendered route has a single rendezvous marker" );
        for ( ; fleet.Tick < end; fleet.Step( ) ) {
            sawCoast |= fleet.RoutePhase( scout ) is NavigationPhase.Coast or NavigationPhase.Correction;
            sawBrake |= fleet.RoutePhase( scout ) == NavigationPhase.Brake;
            fleet.FillInstances( visual, scout );
            float minimumAlignment = MathF.Cos( FleetSimulation.OrbitalTurnStep ) - 1e-5f;
            Require( Vector2.Dot( previousHeading, visual[0].Heading ) >= minimumAlignment,
                "heading remains rate limited through turnover and insertion" );
            previousHeading = visual[0].Heading;
        }
        Require( sawCoast && sawBrake && fleet.RouteTicks( scout ) == 0 && fleet.RoutePhase( scout ) == NavigationPhase.Parked &&
            Vector2.Distance( fleet.Center( scout ), expected ) < 0.01f && Vector2.Distance( fleet.Bodies[belt].Center, originalBelt ) > 1,
            "burn, coast, brake and moving-body insertion match the forecast" );
        float radius = Vector2.Distance( fleet.Center( scout ), fleet.Bodies[belt].Center );
        AdvanceUntil( fleet, () => fleet.HasDeposit( belt ), "survey completes from orbit" );
        Require( fleet.DepositYield( belt ) == 20 && MathF.Abs( Vector2.Distance( fleet.Center( scout ), fleet.Bodies[belt].Center ) - radius ) < 0.01f &&
            fleet.Orbit( scout, FleetSimulation.Homeworld ), "scout remains in host-relative orbit and can plot a return" );
        Require( FleetHud.Hit( new( 465, 177 ), 640, 400, fleet, belt ) == new HudHit( HudAction.Planet, (int)PlanetAction.Mine ),
            "planet inspector site card matches the mine action" );
    }

    private static void VerifyProjectCancellation( ) {
        const int home = FleetSimulation.Homeworld, moon = 3;
        var fleet = new FleetSimulation( 3, 37, frontier: true );
        var scout = new FleetSelection( 0 );
        var miner = new FleetSelection( 1 );
        int originalOre = fleet.Ore;
        Require( fleet.QueueWorker( ) && fleet.CancelCivilian( ) && fleet.WorkerRemaining < 0 && fleet.Ore == originalOre &&
            !fleet.CancelCivilian( ), "civilian cancellation refunds one order and releases its berth" );
        Require( fleet.BuildFacility( home, PlanetFacility.Lab ) &&
            FleetHud.Hit( new( 605, 208 ), 640, 400, fleet, home ) == new HudHit( HudAction.Planet, (int)PlanetAction.CancelLab ) &&
            fleet.CancelSite( home, PlanetFacility.Lab ) && fleet.Ore == originalOre, "site card cancels construction and returns ore" );
        Require( fleet.BeginSurvey( scout, moon ), "survey the Moon to discover a construction site" );
        AdvanceUntil( fleet, () => fleet.HasDeposit( moon ), "Moon survey completes", 1800 );
        originalOre = fleet.Ore;
        Require( fleet.BuildFacility( moon, PlanetFacility.Mine, miner ) && fleet.MineEnRoute( moon ), "mine assigns a traveling constructor" );
        AdvanceUntil( fleet, () => fleet.StartedConstruction == 1, "mine starts when its constructor reaches the site" );
        var site = fleet.SitePosition( moon, PlanetFacility.Mine, 1 );
        Require( fleet.SiteWorker( moon ) == miner && Vector2.Distance( site, fleet.Center( miner ) ) < 6 &&
            fleet.CancelSite( moon, PlanetFacility.Mine ) && fleet.SiteWorker( moon ) is null &&
            fleet.Ore == originalOre && !fleet.CancelSite( moon, PlanetFacility.Mine ), "active mine stays near its builder and can be canceled" );
        Require( fleet.BuildFacility( moon, PlanetFacility.Mine, miner ), "canceled mine can be rebuilt" );
        AdvanceUntil( fleet, () => fleet.Ore >= 205, "production cancellation test funds its yard and wings", 4000 );
        Require( fleet.BuildYard( home ), "queue fixture has an orbital yard" );
        AdvanceUntil( fleet, () => fleet.YardCount == 1, "orbital yard comes online" );
        fleet.SetPlating( true );
        Require( fleet.QueueWing( ), "armored order queued" );
        fleet.SetPlating( false );
        Require( fleet.QueueWing( ) && fleet.WingQueue[0].Cost == 68 && fleet.WingQueue[1].Cost == 60, "orders preserve purchase prices" );
        originalOre = fleet.Ore;
        Require( fleet.CancelWing( 0 ) && fleet.Ore == originalOre + 68 && fleet.NavalReserved == 3 &&
            fleet.WingRemaining == FleetSimulation.StandardWingDuration && fleet.WingQueue[0].Cost == 60,
            "cancelling active work refunds its snapshot and starts the next job" );
        Require( fleet.CancelWing( 0 ) && fleet.NavalReserved == 0 && fleet.WingRemaining < 0 && !fleet.CancelWing( 0 ) &&
            !fleet.CancelSite( home, PlanetFacility.Yard ), "completed infrastructure and empty queues reject cancellation" );
    }

    private static void VerifyAudio( ) {
        using var custom = JsonDocument.Parse( """
            {"playlists":{"system":["@system-theme.ogg"],"galaxy":["chart.wav"],"contested":["@signal"],"victory":["@arrival"]}}
            """ );
        var jukebox = Jukebox.Parse( custom.RootElement );
        Require( ReferenceEquals( jukebox.Tracks( 0 ), jukebox.Tracks( 2 ) ) &&
            ReferenceEquals( jukebox.Tracks( 0 ), jukebox.Tracks( 3 ) ) &&
            jukebox.Tracks( 0 ).SequenceEqual( new[] { "system-theme.ogg" } ) &&
            jukebox.Tracks( 1 ).SequenceEqual( new[] { "chart.wav" } ) &&
            jukebox.Fallback( 3 ) == "@survey", "custom music continues through default battle and victory playlists" );
        using var optIn = JsonDocument.Parse( """
            {"useProceduralTransitions":true,"playlists":{"system":["my-track.wav"],"contested":["@signal"]}}
            """ );
        Require( Jukebox.Parse( optIn.RootElement ).Tracks( 2 ).SequenceEqual( new[] { "@signal" } ),
            "explicit procedural transitions still work" );
        Require( new Jukebox( ).Tracks( 1 ).SequenceEqual( new[] { "@cartography" } ), "default soundtrack still has context" );

        using var vorbis = File.OpenRead( Path.Combine( AppContext.BaseDirectory, "Audio", "TestTone.ogg" ) );
        using var pcm = new MemoryStream( );
        VorbisTrack.Decode( vorbis, pcm );
        var wave = pcm.GetBuffer( ).AsSpan( 0, (int)pcm.Length );
        Require( vorbis.CanRead && wave[..4].SequenceEqual( "RIFF"u8 ) && wave.Slice( 36, 4 ).SequenceEqual( "data"u8 ) &&
            System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian( wave[24..] ) == 16000 &&
            System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian( wave[40..] ) == 1920 * 2 &&
            wave[44..].IndexOfAnyExcept( (byte)0 ) >= 0, "mono Vorbis decodes to a complete, non-silent 16 kHz PCM track" );

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
