namespace FleetBattler;

internal enum CampaignEventKind { ResearchComplete, Distress, Arrival, ReceiverDecision, CrossingComplete, TravelArrival, EconomicCharter }

internal readonly record struct CampaignEvent( CampaignEventKind Kind, FleetSimulation System, ResearchTopic Technology = default );

internal readonly record struct FleetTransit( int Source, int Destination, int Departure, int Arrival, ExpeditionHull[] Hulls, bool Survey );

internal sealed class FleetCampaign( FleetSimulation sol, uint seed ) {
    private readonly FleetSimulation?[] _systems = new FleetSimulation?[CelestialSystem.Systems.Length];
    private readonly Queue<CampaignEvent> _events = new( );
    private int _dispatchesSeen = sol.DispatchCount, _researchSeen = sol.CompletedResearch;
    private readonly int[] _remoteResearchSeen = new int[CelestialSystem.Systems.Length];
    private readonly List<FleetTransit> _transfers = new( );
    internal const int MaxTravelHulls = 24;
    internal ReadOnlySpan<FleetTransit> Transfers => global::System.Runtime.InteropServices.CollectionsMarshal.AsSpan( _transfers );
    private bool _offeredCharter, _reportedEnding;

    internal CampaignFaction PlayerFaction => sol.PlayerFaction;
    // Campaign scenarios run on the campaign calendar (see Pacing).
    internal bool Calendar => sol.FrontierProgression;
    internal int FirstDepartureTick => _firstDepartureTick;
    private int _firstDepartureTick;
    internal Expedition? InTransit { get; private set; }
    internal bool Arrived => _systems[FleetSimulation.Proxima]?.ExpeditionArrived == true;
    internal int HullsAt( int index ) => index == 0 ? sol.CyanCount : _systems[index]?.CyanCount ?? 0;
    internal FleetSimulation? OccupiedSystem( int index ) =>
        index == 0 ? sol : _systems[index] is { ExpeditionArrived: true } colony ? colony : null;

    internal FleetSimulation System( int index ) {
        if ( index == 0 ) return sol;
        int ships = sol.Frontier ? FleetSimulation.DefaultShips : sol.Capacity - FleetSimulation.WingSize * FleetSimulation.WingGoal;
        if ( _systems[index] is not { } system ) {
            system = new FleetSimulation( ships, unchecked(seed + (uint)index * 1177),
                rivalPatrol: true, system: index, battle: true, frontier: sol.Frontier ) { FrontierProgression = sol.FrontierProgression };
            system.AttachFaction( PlayerFaction );
            _systems[index] = system;
        }
        if ( sol.FrontierProgression ) system.AlignEphemeris( sol.Tick );
        return system;
    }

    internal bool Depart( ReadOnlySpan<bool> selected ) {
        if ( sol.Depart( selected, sol.Tick ) is not { } flight ) return false;
        InTransit = flight;
        return true;
    }

    internal int FlightTicks( int source, int destination ) {
        if ( sol.FrontierProgression )
            return !Arrived ? sol.CrossingTicks : Pacing.InterstellarTicks( SystemDistance( source, destination ), sol.FusionUpgrade, sol.Researched );
        int duration = 420 + (int)(SystemDistance( source, destination ) * 1080);
        return sol.FusionUpgrade ? duration * 3 / 4 : duration;
    }

    private static float SystemDistance( int source, int destination ) =>
        global::System.Numerics.Vector2.Distance( CelestialSystem.MapPositions[source], CelestialSystem.MapPositions[destination] );

    internal string? TravelBlock( FleetSimulation origin, int destination, ReadOnlySpan<bool> selected, bool survey ) {
        if ( !origin.Frontier || (uint)destination >= (uint)_systems.Length || destination == origin.SystemIndex ) return "CHOOSE ANOTHER SYSTEM";
        if ( sol.FrontierProgression && !Arrived ) {
            if ( origin.SystemIndex != 0 || destination != FleetSimulation.Proxima ) return "FIRST CROSSING MUST DEPART SOL FOR PROXIMA";
            if ( survey ) return "SURVEY AFTER THE FIRST LANDING";
            return origin.LaunchBlock( selected );
        }
        int count = 0;
        bool scout = false, civilianOnly = true;
        for ( int ship = 0; ship < Math.Min( origin.Count, selected.Length ); ++ship ) {
            if ( !selected[ship] || !origin.Alive( ship ) || origin.FactionOf( ship ) != FactionId.Sol ) continue;
            if ( !origin.TravelReady( ship ) ) return "SELECT IDLE SHIPS OR FINISH THEIR LOCAL ORDERS";
            var role = origin.Role( origin.SquadronOf( ship ) );
            scout |= role == HullRole.Scout;
            civilianOnly &= role == HullRole.Scout;
            ++count;
        }
        if ( count == 0 ) return "SELECT SHIPS FROM A FLEET BADGE";
        if ( !Arrived && (origin.SystemIndex != 0 || destination != FleetSimulation.Proxima) )
            return "SEND A SCOUT TO PROXIMA TO OPEN OTHER ROUTES";
        if ( !Arrived && !civilianOnly ) return "FIRST CROSSING / SELECT AN IDLE SCOUT ONLY";
        if ( survey && !scout ) return "SURVEY ORDERS NEED A SCOUT";
        if ( count > MaxTravelHulls ) return "SPLIT THE TRANSFER INTO GROUPS OF 24";
        var destinationFleet = destination == 0 ? sol : _systems[destination];
        if ( destinationFleet is not null && count > destinationFleet.TravelBerths )
            return "DESTINATION HAS NO FREE BERTHS";
        return null;
    }

    internal bool Travel( FleetSimulation origin, int destination, ReadOnlySpan<bool> selected, bool survey ) {
        if ( TravelBlock( origin, destination, selected, survey ) is not null ) return false;
        if ( sol.FrontierProgression && !Arrived ) {
            if ( !Depart( selected ) ) return false;
            _firstDepartureTick = sol.Tick;
            return true;
        }
        var target = System( destination );
        int arrival = checked(sol.Tick + FlightTicks( origin.SystemIndex, destination ));
        var hulls = origin.RemoveTravelHulls( selected );
        target.ReserveTravelBerths( hulls.Length );
        _transfers.Add( new( origin.SystemIndex, destination, sol.Tick, arrival, hulls, survey ) );
        return true;
    }

    // Called immediately after a Sol tick. Remote charts stay at their surveyed epoch until arrival.
    internal void Advance( ) {
        if ( !sol.Frontier ) return;
        var destination = _systems[FleetSimulation.Proxima];
        for ( int i = 1; i < _systems.Length; ++i ) {
            if ( _systems[i] is not { } active ) continue;
            if ( !active.ExpeditionArrived ) {
                if ( sol.FrontierProgression ) active.AlignEphemeris( sol.Tick );
                continue;
            }
            active.Step( );
            ReportResearch( active, ref _remoteResearchSeen[i] );
        }

        ReportResearch( sol, ref _researchSeen );
        while ( _dispatchesSeen < sol.DispatchCount ) {
            var dispatch = sol.Dispatch( _dispatchesSeen++ );
            if ( dispatch == FrontierEvent.Prospect && sol.CharterEconomy == EconomicCharter.Pending )
                _events.Enqueue( new( CampaignEventKind.EconomicCharter, sol ) );
            if ( dispatch == FrontierEvent.Distress )
                _events.Enqueue( new( CampaignEventKind.Distress, sol ) );
        }

        if ( InTransit is { } flight && sol.Tick >= flight.ArrivalTick ) {
            destination = System( flight.Destination );
            if ( !destination.Receive( flight, sol.Tick ) )
                throw new InvalidOperationException( "The scheduled expedition could not be received." );
            InTransit = null;
            _events.Enqueue( new( CampaignEventKind.Arrival, destination ) );
        }

        for ( int i = 0; i < _transfers.Count; ) {
            var transfer = _transfers[i];
            if ( transfer.Arrival > sol.Tick ) { ++i; continue; }
            var target = System( transfer.Destination );
            bool first = !Arrived && transfer.Destination == FleetSimulation.Proxima;
            target.ReserveTravelBerths( -transfer.Hulls.Length );
            target.ReceiveTravelHulls( transfer.Hulls, first ? new( 45, 20 ) : default, transfer.Survey );
            _transfers.RemoveAt( i );
            _events.Enqueue( new( first ? CampaignEventKind.Arrival : CampaignEventKind.TravelArrival, target ) );
        }
        destination = _systems[FleetSimulation.Proxima];
        if ( destination?.ExpeditionArrived != true ) return;
        if ( !_offeredCharter && destination.ReceiverOnline ) {
            _offeredCharter = true;
            _events.Enqueue( new( CampaignEventKind.ReceiverDecision, destination ) );
        }
        if ( !_reportedEnding && destination.ReceiverSecured ) {
            _reportedEnding = true;
            _events.Enqueue( new( CampaignEventKind.CrossingComplete, destination ) );
        }
    }

    internal bool TryTakeEvent( out CampaignEvent entry ) => _events.TryDequeue( out entry );

    private void ReportResearch( FleetSimulation system, ref int seen ) {
        if ( seen == system.CompletedResearch ) return;
        seen = system.CompletedResearch;
        _events.Enqueue( new( CampaignEventKind.ResearchComplete, system, system.LastTechnology ) );
    }
}
