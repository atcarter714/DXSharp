namespace FleetBattler;

internal sealed partial class FleetSimulation {
    private int _nextFleetId;
    internal int FleetId( int squad ) => _squads[squad].FleetId;
    internal bool IsFleetLeader( int squad ) {
        if ( !Frontier ) return true;
        int id = FleetId( squad );
        for ( int i = 0; i < squad; ++i )
            if ( ShipCount( i ) > 0 && FleetId( i ) == id ) return false;
        return true;
    }

    internal int FleetStrength( int squad ) {
        int id = FleetId( squad ), count = 0;
        for ( int i = 0; i < SquadCount; ++i )
            if ( FleetId( i ) == id ) count += ShipCount( i );
        return count;
    }

    internal bool CanMergeFleets( ReadOnlySpan<bool> selected ) {
        int first = 0;
        for ( int i = 0; i < SquadCount; ++i ) {
            if ( !SelectedCombatSquad( i, selected ) ) continue;
            int id = FleetId( i );
            if ( first != 0 && first != id ) return true;
            first = id;
        }
        return false;
    }

    internal bool MergeFleets( ReadOnlySpan<bool> selected ) {
        if ( !Frontier || !CanMergeFleets( selected ) ) return false;
        Span<int> groups = stackalloc int[SquadCount];
        int count = 0;
        for ( int i = 0; i < SquadCount; ++i )
            if ( SelectedCombatSquad( i, selected ) ) groups[count++] = FleetId( i );
        int target = groups[0];
        for ( int i = 0; i < SquadCount; ++i )
            if ( ShipCount( i ) > 0 && groups[..count].Contains( FleetId( i ) ) ) _squads[i].FleetId = target;
        return true;
    }

    internal bool CanSplitFleets( ReadOnlySpan<bool> selected ) {
        for ( int i = 0; i < SquadCount; ++i )
            if ( SelectedCombatSquad( i, selected ) && FleetStrength( i ) > ShipCount( i ) ) return true;
        return false;
    }

    internal bool SplitFleets( ReadOnlySpan<bool> selected ) {
        if ( !Frontier || !CanSplitFleets( selected ) ) return false;
        for ( int i = 0; i < SquadCount; ++i )
            if ( SelectedCombatSquad( i, selected ) && FleetStrength( i ) > ShipCount( i ) )
                _squads[i].FleetId = ++_nextFleetId;
        return true;
    }

    private bool SelectedCombatSquad( int squad, ReadOnlySpan<bool> selected ) =>
        selected.Length >= Count && IsPlayerSquad( squad ) && ShipCount( squad ) > 0 &&
        Role( squad ) == HullRole.Corvette && AnySelectedInSquad( squad, selected );
}
