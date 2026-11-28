namespace FleetBattler;

internal enum ReceiverCharter { Pending, Shared, Independent }
internal readonly record struct ExpeditionHull( HullRole Role, ShipDesign Design, byte Hull, byte Shield, byte Armor, FleetStance Stance, string? Name,
    int Group = 0, string? FleetName = null );
internal readonly record struct Expedition( int Destination, int ArrivalTick, ExpeditionHull[] Hulls, ResourceCost Supplies );

internal sealed partial class FleetSimulation {
    internal const int Proxima = 1, ExpeditionDuration = 1800;
    internal static readonly ResourceCost CrossingCost = new( 65, 100 );
    private static readonly ResourceCost ArrivalSupplies = new( 45, 20 );
    private bool _expeditionArrived, _expeditionDeparted;
    internal bool ExpeditionArrived => _expeditionArrived;
    internal bool CanSeeContacts => !Frontier || Solar || _expeditionArrived;
    internal bool ChartOnly => Frontier && !CanSeeContacts;

    internal bool ExpeditionReadyInOrbit( int ship ) {
        if ( !Alive( ship ) || FactionOf( ship ) != FactionId.Sol ) return false;
        var order = EffectiveOrder( new( SquadronOf( ship ), ship ) );
        return order.Body == Homeworld + 1 && order.Transfer.End == 0 && order.Service == ServiceOrder.None &&
            order.Construction == 0 && !order.Settlement && !order.Survey;
    }

    internal int SelectExpedition( CommandSelection selection ) {
        var (scout, builder) = ReadyExpeditionCrew();
        if ( scout < 0 || builder < 0 ) return 0;
        selection.Click( this, scout, false, SelectionMode.Replace );
        selection.Click( this, builder, false, SelectionMode.Add );
        return selection.Count;
    }

    internal (int Scout, int Builder) ReadyExpeditionCrew() {
        int scout = -1, builder = -1;
        for ( int ship = 0; ship < Count; ++ship ) {
            if ( !ExpeditionReadyInOrbit( ship ) ) continue;
            var role = Role( SquadronOf( ship ) );
            if ( scout < 0 && role == HullRole.Scout ) scout = ship;
            if ( builder < 0 && role == HullRole.Miner ) builder = ship;
            if ( scout >= 0 && builder >= 0 ) break;
        }
        return (scout, builder);
    }

    internal bool RallyExpedition( ) {
        bool changed = false;
        for ( int ship = 0; ship < Count; ++ship ) {
            if ( !Alive( ship ) || FactionOf( ship ) != FactionId.Sol || ExpeditionReadyInOrbit( ship ) ) continue;
            int squad = SquadronOf( ship );
            if ( Role( squad ) is not (HullRole.Scout or HullRole.Miner) ) continue;
            var order = EffectiveOrder( new( squad, ship ) );
            if ( order.Service != ServiceOrder.None || order.Survey || order.Construction != 0 || order.Settlement ||
                 order.Body == Homeworld + 1 ) continue;
            changed |= Orbit( new( squad, ship ), Homeworld );
        }
        return changed;
    }

    internal string? LaunchBlock( ReadOnlySpan<bool> selected ) {
        if ( _expeditionDeparted ) return "EXPEDITION ALREADY DISPATCHED";
        if ( !Solar || !FusionUpgrade ) return "RESEARCH PULSED FUSION IN SOL";
        if ( !FrontierProgression && ColoniesFounded == 0 ) return "FOUND A COLONY BEFORE LAUNCH";
        if ( YardAt( Homeworld ) != 0 ) return "FINISH EARTH'S ORBITAL SHIPYARD";
        if ( SiteIntegrity( Homeworld, PlanetFacility.Yard ) < 50 ) return "RESTORE EARTH'S ORBITAL SHIPYARD";
        int count = 0;
        bool scout = false, constructor = false;
        for ( int ship = 0; ship < Math.Min( selected.Length, Count ); ++ship ) {
            if ( !selected[ship] || !Alive( ship ) || FactionOf( ship ) != FactionId.Sol ) continue;
            if ( !ExpeditionReadyInOrbit( ship ) ) return "SELECT IDLE SHIPS IN EARTH ORBIT";
            int squad = SquadronOf( ship );
            scout |= Role( squad ) == HullRole.Scout;
            constructor |= Role( squad ) == HullRole.Miner;
            ++count;
        }
        if ( !scout || !constructor ) return "SELECT A SCOUT AND A CONSTRUCTOR";
        if ( count >= CyanCount ) return "LEAVE ONE SHIP IN SOL";
        if ( count > 8 ) return "MAXIMUM EIGHT EXPEDITION HULLS";
        return CanAfford( ExpeditionCost ) ? null : $"NEEDS {ExpeditionCost.Ore} ORE AND {ExpeditionCost.Energy} ENERGY";
    }

    // The first crossing to Proxima: a fixed sandbox flight, or the campaign's paced trip shortened by propulsion research.
    internal int CrossingTicks => FrontierProgression
        ? Pacing.InterstellarTicks( Pacing.ProximaDistance, FusionUpgrade, Researched )
        : ExpeditionDuration;

    internal ResourceCost ExpeditionCost => FrontierProgression ? new( 145, 185 ) : CrossingCost;

    internal Expedition? Depart( ReadOnlySpan<bool> selected, int campaignTick ) {
        if ( LaunchBlock( selected ) is not null ) return null;
        int arrival = checked(campaignTick + CrossingTicks);
        Spend( ExpeditionCost );
        _expeditionDeparted = true;
        return new( Proxima, arrival, RemoveTravelHulls( selected ), ArrivalSupplies );
    }

    internal bool TravelReady( int ship ) {
        if ( !Alive( ship ) || FactionOf( ship ) != FactionId.Sol ) return false;
        var order = EffectiveOrder( new( SquadronOf( ship ), ship ) );
        return order.Enemy == 0 && order.Transfer.End <= Tick && order.Service == ServiceOrder.None &&
            order.Construction == 0 && !order.Settlement && !order.Survey;
    }

    internal int TravelBerths {
        get {
            int room = Capacity - Count;
            for ( int squad = 0; squad < SquadCount; ++squad )
                if ( _squads[squad] is { Alive: 0, Count: 1, Faction: (int)FactionId.Sol } ) ++room;
            return room - _reservedTravelBerths - _yardQueueSizes.Sum( );
        }
    }

    private int _reservedTravelBerths;
    internal void ReserveTravelBerths( int amount ) => _reservedTravelBerths += amount;

    internal ExpeditionHull[] RemoveTravelHulls( ReadOnlySpan<bool> selected ) {
        var hulls = new List<ExpeditionHull>( 8 );
        for ( int ship = 0; ship < Math.Min( selected.Length, Count ); ++ship ) {
            if ( !selected[ship] || !Alive( ship ) || FactionOf( ship ) != FactionId.Sol ) continue;
            int squad = SquadronOf( ship );
            ref var state = ref _squads[squad];
            hulls.Add( new( state.Role, state.Design, _hull[ship], _shield[ship], _armor[ship], state.Stance,
                ShipName( ship ), state.FleetId, FleetName( squad ) ) );
            _hull[ship] = 0;
            --state.Alive;
            --_factionCount[(int)FactionId.Sol];
            ReleaseFleetName( squad );
        }
        return hulls.ToArray( );
    }

    internal bool Receive( Expedition flight, int campaignTick ) {
        if ( !Frontier || SystemIndex != Proxima || _expeditionArrived || flight.Destination != SystemIndex ||
             campaignTick < flight.ArrivalTick || flight.Hulls is not { Length: > 0 and <= 8 } || flight.Hulls.Length > TravelBerths ) return false;
        ReceiveTravelHulls( flight.Hulls, flight.Supplies, survey: false );
        return true;
    }

    internal void ReceiveTravelHulls( ReadOnlySpan<ExpeditionHull> hulls, ResourceCost supplies, bool survey ) {
        bool firstArrival = !_expeditionArrived;
        _expeditionArrived = true;
        Refund( supplies );
        Span<int> groups = stackalloc int[hulls.Length];
        for ( int i = 0; i < hulls.Length; ++i ) {
            var hull = hulls[i];
            float angle = MathF.Tau * i / hulls.Length;
            var offset = new System.Numerics.Vector2( MathF.Cos( angle ), MathF.Sin( angle ) ) * OrbitRadius( Homeworld, 1 );
            int squad = AddSquad( 1, (int)FactionId.Sol, _bodies[Homeworld].Center + offset, hull.Role, hull.Design );
            Park( squad, Homeworld, Tick );
            int ship = _squads[squad].First;
            _hull[ship] = hull.Hull;
            _shield[ship] = hull.Shield;
            _armor[ship] = hull.Armor;
            _squads[squad].Stance = hull.Stance;
            _shipNames[ship] = hull.Name;
            if ( hull.Group != 0 ) {
                for ( int prior = 0; prior < i; ++prior )
                    if ( hulls[prior].Group == hull.Group ) {
                        _squads[squad].FleetId = groups[prior];
                        break;
                    }
                groups[i] = FleetId( squad );
                if ( hull.FleetName is { } name ) _fleetNames[groups[i]] = name;
            }
            if ( !survey || hull.Role != HullRole.Scout ) continue;
            for ( int body = 1; body < _bodies.Length; ++body )
                if ( BeginSurvey( new( squad, ship ), body ) ) break;
        }
        if ( firstArrival && SystemIndex >= CelestialSystem.AuthoredCount && SystemIndex % 3 == 0 && TravelBerths >= 2 ) {
            var faction = SystemIndex % 2 == 0 ? FactionId.Asteri : FactionId.Veyran;
            int patrol = AddSquad( 2, (int)faction, _bodies[2].Center, HullRole.Frigate );
            Park( patrol, 2, Tick );
        }
    }

    internal bool ReceiverOnline => SystemIndex == Proxima && _expeditionArrived && SiteRemaining( Homeworld, PlanetFacility.Lab ) == 0;
    internal ReceiverCharter Charter { get; private set; }
    internal bool ReceiverSecured => ReceiverOnline && (Charter == ReceiverCharter.Shared ||
        Charter == ReceiverCharter.Independent && AmberCount == 0);
    internal bool CanClaimReceiver => ReceiverOnline && NavalUsed > 0;

    internal bool CommissionReceiver( ReceiverCharter charter ) {
        if ( !ReceiverOnline || Charter != ReceiverCharter.Pending ||
             charter is not (ReceiverCharter.Shared or ReceiverCharter.Independent) ||
             charter == ReceiverCharter.Independent && !CanClaimReceiver ) return false;
        Charter = charter;
        if ( charter == ReceiverCharter.Shared ) {
            Refund( new( 0, 0, 30 ) );
        } else {
            var point = _bodies[Homeworld].Center + new System.Numerics.Vector2( 45, 15 );
            int rival = AddSquad( 2, (int)FactionId.Rival, point, design: new( WeaponKind.Rail, false ) );
            Orbit( new( rival ), Homeworld );
        }
        return true;
    }

    private void AdvanceReceiver( ) {
        if ( Charter != ReceiverCharter.Independent || AmberCount == 0 || Tick % 120 != 0 ) return;
        for ( int squad = 0; squad < SquadCount; ++squad ) {
            if ( _squads[squad].Faction != (int)FactionId.Rival || ShipCount( squad ) == 0 ) continue;
            int first = _squads[squad].First;
            int end = first + _squads[squad].Count;
            while ( first < end && !Alive( first ) ) ++first;
            int target = ClosestHostile( first, (int)FactionId.Rival, 35 );
            if ( target >= 0 ) Attack( new( squad ), target );
            else if ( _squads[squad].Order.Body != Homeworld + 1 ) Orbit( new( squad ), Homeworld );
        }
    }
}
