namespace FleetBattler;

internal enum CampaignEventKind { ResearchComplete, Distress, Arrival, ReceiverDecision, CrossingComplete }

internal readonly record struct CampaignEvent( CampaignEventKind Kind, FleetSimulation System, ResearchTopic Technology = default );

internal sealed class FleetCampaign( FleetSimulation sol, uint seed ) {
    private readonly FleetSimulation?[] _systems = new FleetSimulation?[CelestialSystem.Systems.Length];
    private readonly Queue<CampaignEvent> _events = new( );
    private int _dispatchesSeen = sol.DispatchCount, _researchSeen = sol.CompletedResearch;
    private int _remoteResearchSeen;
    private bool _offeredCharter, _reportedEnding;

    internal Expedition? InTransit { get; private set; }
    internal bool Arrived => _systems[FleetSimulation.Proxima]?.ExpeditionArrived == true;
    internal int HullsAt( int index ) => index == 0 ? sol.CyanCount : _systems[index]?.CyanCount ?? 0;

    internal FleetSimulation System( int index ) {
        if ( index == 0 ) return sol;
        int ships = sol.Frontier ? FleetSimulation.DefaultShips : sol.Capacity - FleetSimulation.WingSize * FleetSimulation.WingGoal;
        return _systems[index] ??= new FleetSimulation( ships, unchecked(seed + (uint)index * 1177),
            rivalPatrol: true, system: index, battle: true, frontier: sol.Frontier );
    }

    internal bool Depart( ReadOnlySpan<bool> selected ) {
        if ( sol.Depart( selected, sol.Tick ) is not { } flight ) return false;
        InTransit = flight;
        return true;
    }

    // Called immediately after a Sol tick. Remote charts stay at their surveyed epoch until arrival.
    internal void Advance( ) {
        if ( !sol.Frontier ) return;
        var destination = _systems[FleetSimulation.Proxima];
        if ( destination?.ExpeditionArrived == true ) destination.Step( );

        ReportResearch( sol, ref _researchSeen );
        while ( _dispatchesSeen < sol.DispatchCount )
            if ( sol.Dispatch( _dispatchesSeen++ ) == FrontierEvent.Distress )
                _events.Enqueue( new( CampaignEventKind.Distress, sol ) );

        if ( InTransit is { } flight && sol.Tick >= flight.ArrivalTick ) {
            destination = System( flight.Destination );
            if ( !destination.Receive( flight, sol.Tick ) )
                throw new InvalidOperationException( "The scheduled expedition could not be received." );
            InTransit = null;
            _events.Enqueue( new( CampaignEventKind.Arrival, destination ) );
        }

        if ( destination?.ExpeditionArrived != true ) return;
        ReportResearch( destination, ref _remoteResearchSeen );
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
