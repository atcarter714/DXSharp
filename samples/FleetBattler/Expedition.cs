namespace FleetBattler;

internal enum ReceiverCharter { Pending, Shared, Independent }
internal readonly record struct ExpeditionHull( HullRole Role, ShipDesign Design, byte Hull, byte Shield, byte Armor, FleetStance Stance, string? Name );
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
        Span<int> crew = stackalloc int[2] { -1, -1 };
        for ( int ship = 0; ship < Count; ++ship ) {
            if ( !ExpeditionReadyInOrbit( ship ) ) continue;
            int role = Role( SquadronOf( ship ) ) switch { HullRole.Scout => 0, HullRole.Miner => 1, _ => -1 };
            if ( role >= 0 && crew[role] < 0 ) crew[role] = ship;
        }
        if ( crew[0] < 0 || crew[1] < 0 ) return 0;
        selection.Click( this, crew[0], false, SelectionMode.Replace );
        selection.Click( this, crew[1], false, SelectionMode.Add );
        return selection.Count;
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
        if ( ColoniesFounded == 0 ) return "FOUND A COLONY BEFORE LAUNCH";
        if ( YardAt( Homeworld ) != 0 ) return "FINISH EARTH'S ORBITAL SHIPYARD";
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
        return CanAfford( CrossingCost ) ? null : "NEEDS 65 ORE AND 100 ENERGY";
    }

    internal Expedition? Depart( ReadOnlySpan<bool> selected, int campaignTick ) {
        if ( LaunchBlock( selected ) is not null ) return null;
        var hulls = new List<ExpeditionHull>( 8 );
        for ( int ship = 0; ship < Math.Min( selected.Length, Count ); ++ship ) {
            if ( !selected[ship] || !Alive( ship ) || FactionOf( ship ) != FactionId.Sol ) continue;
            ref var squad = ref _squads[SquadronOf( ship )];
            hulls.Add( new( squad.Role, squad.Design, _hull[ship], _shield[ship], _armor[ship], squad.Stance, _shipNames[ship] ) );
        }
        int arrival = checked(campaignTick + ExpeditionDuration);
        Spend( CrossingCost );
        _expeditionDeparted = true;
        for ( int ship = 0; ship < Math.Min( selected.Length, Count ); ++ship ) {
            if ( !selected[ship] || !Alive( ship ) || FactionOf( ship ) != FactionId.Sol ) continue;
            int squad = SquadronOf( ship );
            _hull[ship] = 0;
            --_squads[squad].Alive;
            --_factionCount[(int)FactionId.Sol];
            ReleaseFleetName( squad );
        }
        return new( Proxima, arrival, hulls.ToArray( ), ArrivalSupplies );
    }

    internal bool Receive( Expedition flight, int campaignTick ) {
        if ( !Frontier || SystemIndex != Proxima || _expeditionArrived || flight.Destination != SystemIndex ||
             campaignTick < flight.ArrivalTick || flight.Hulls is not { Length: > 0 and <= 8 } ) return false;
        _expeditionArrived = true;
        Refund( flight.Supplies );
        int berth = 0;
        foreach ( var hull in flight.Hulls ) {
            float angle = MathF.Tau * berth++ / flight.Hulls.Length;
            var offset = new System.Numerics.Vector2( MathF.Cos( angle ), MathF.Sin( angle ) ) * OrbitRadius( Homeworld, 1 );
            int squad = AddSquad( 1, (int)FactionId.Sol, _bodies[Homeworld].Center + offset, hull.Role, hull.Design );
            Park( squad, Homeworld, Tick );
            int ship = _squads[squad].First;
            _hull[ship] = hull.Hull;
            _shield[ship] = hull.Shield;
            _armor[ship] = hull.Armor;
            _squads[squad].Stance = hull.Stance;
            _shipNames[ship] = hull.Name;
        }
        return true;
    }

    internal bool ReceiverOnline => _expeditionArrived && SiteRemaining( Homeworld, PlanetFacility.Lab ) == 0;
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
