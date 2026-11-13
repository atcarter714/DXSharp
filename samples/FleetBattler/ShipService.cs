using System.Numerics;

namespace FleetBattler;

internal enum FleetStance { Normal, Aggressive, Passive, Mixed }
internal enum ServiceOrder : byte { None, Repair, Refit, Retreat }

internal sealed partial class FleetSimulation {
    internal const int FrontierRepairCapacity = FrontierCapacity;
    private readonly ShipDesign[] _refitDesigns;
    internal int CompletedServices { get; private set; }

    internal bool RepairingAtYard( int ship ) => Alive( ship ) &&
        _shipOrders[ship] is { Service: ServiceOrder.Repair, Transfer.End: 0 };

    internal ServiceOrder Service( FleetSelection unit ) => EffectiveOrder( unit ).Service;

    internal FleetStance SelectionStance( ReadOnlySpan<bool> selected ) {
        FleetStance? stance = null;
        for ( int ship = 0; ship < Math.Min( selected.Length, Count ); ++ship ) {
            if ( !selected[ship] || !Alive( ship ) || FactionOf( ship ) != FactionId.Sol ) continue;
            var current = _squads[SquadronOf( ship )].Stance;
            if ( stance is not null && stance != current ) return FleetStance.Mixed;
            stance = current;
        }
        return stance ?? FleetStance.Normal;
    }

    internal void SetStance( ReadOnlySpan<bool> selected, FleetStance stance ) {
        for ( int ship = 0; ship < Math.Min( selected.Length, Count ); ++ship ) {
            if ( !selected[ship] || !Alive( ship ) || FactionOf( ship ) != FactionId.Sol ) continue;
            _squads[SquadronOf( ship )].Stance = stance;
        }
    }

    internal bool NeedsRepair( int ship ) {
        var design = Loadout( ship );
        var role = Role( SquadronOf( ship ) );
        return _hull[ship] < MaxHull( role ) || IsWarship( role ) && _armor[ship] < design.Armor;
    }

    private int FindServiceYard( FleetSelection unit, Vector2 from ) {
        int result = -1;
        float distance = float.PositiveInfinity;
        for ( int body = 1; body < _bodies.Length; ++body ) {
            if ( YardAt( body ) != 0 || !CanReach( unit, body ) ) continue;
            float candidate = Vector2.DistanceSquared( from, YardAtPosition( body ) );
            if ( candidate >= distance ) continue;
            result = body;
            distance = candidate;
        }
        return result;
    }

    internal bool OrderService( int ship, ServiceOrder service ) {
        if ( !Frontier || service == ServiceOrder.None || !Alive( ship ) || FactionOf( ship ) != FactionId.Sol ) return false;
        int squad = SquadronOf( ship );
        if ( service == ServiceOrder.Repair && !NeedsRepair( ship ) ) return false;
        var blueprint = Blueprint( Role( squad ) == HullRole.Frigate ? HullBlueprint.Frigate : HullBlueprint.Corvette );
        if ( service == ServiceOrder.Refit && (!IsWarship( Role( squad ) ) || _squads[squad].Count != 1 || !blueprint.Valid || blueprint == Loadout( ship )) )
            return false;
        var unit = new FleetSelection( squad, ship );
        int yard = FindServiceYard( unit, _positions[ship] );
        if ( service == ServiceOrder.Retreat ) {
            if ( yard < 0 && !Established ) return false;
            int host = yard < 0 ? Homeworld : yard;
            var retreat = PlotTransfer( unit, host, false );
            retreat.Construction = 0;
            retreat.Service = ServiceOrder.Retreat;
            retreat.Settlement = false;
            retreat.ServiceTicks = 0;
            SetOrder( unit, retreat );
            _squads[squad].Stance = FleetStance.Passive;
            return true;
        }
        if ( yard < 0 ) return false;
        var orbit = Construction( yard, PlanetFacility.Yard ).Orbit;
        var order = PlotTransfer( unit, yard, false, fixedBerth: new( orbit.X, orbit.Y + 0.14f ) );
        order.Service = service;
        order.ServiceTicks = 0;
        if ( service == ServiceOrder.Refit ) _refitDesigns[ship] = blueprint;
        SetOrder( unit, order );
        return true;
    }

    private void AdvanceShipService( ) {
        if ( !Frontier ) return;
        for ( int ship = 0; ship < Count; ++ship ) {
            if ( !Alive( ship ) ) continue;
            ref var order = ref _shipOrders[ship];
            if ( order.Service == ServiceOrder.None || order.Transfer.End != 0 ) continue;
            if ( order.Service == ServiceOrder.Retreat ) { order.Service = ServiceOrder.None; continue; }
            if ( Tick - _lastHit[ship] < 60 || YardAt( order.Body - 1 ) != 0 ) continue;
            int period = order.Service == ServiceOrder.Refit ? 180 : 60;
            if ( order.ServiceTicks < period ) ++order.ServiceTicks;
            if ( order.ServiceTicks < period ) continue;
            if ( order.Service == ServiceOrder.Refit ) {
                var design = _refitDesigns[ship];
                var cost = new ResourceCost( 10 + design.ExtraOre, 4 + design.ExtraEnergy );
                if ( !CanAfford( cost ) ) continue;
                Spend( cost );
                ref var squad = ref _squads[SquadronOf( ship )];
                if ( design.Plating != squad.Design.Plating ) _armor[ship] = (byte)design.Armor;
                squad.Design = design;
                _shield[ship] = (byte)Math.Min( _shield[ship], design.Shield );
            } else {
                if ( Energy == 0 ) continue;
                --Energy;
                var design = Loadout( ship );
                _hull[ship] = (byte)Math.Min( MaxHull( Role( SquadronOf( ship ) ) ), _hull[ship] + 12 );
                if ( IsWarship( Role( SquadronOf( ship ) ) ) )
                    _armor[ship] = (byte)Math.Min( design.Armor, _armor[ship] + 8 );
                order.ServiceTicks = 0;
                if ( NeedsRepair( ship ) ) continue;
            }
            order.Service = ServiceOrder.None;
            ++CompletedServices;
        }
    }

    private void AdvanceAggressiveOrders( ) {
        if ( !Frontier || !_battle || AmberCount == 0 || Tick % 30 != 0 ) return;
        for ( int ship = 0; ship < Count; ++ship ) {
            if ( !Alive( ship ) ) continue;
            int squad = SquadronOf( ship );
            ref readonly var group = ref _squads[squad];
            if ( group.Faction != (int)FactionId.Sol || !IsWarship( group.Role ) || group.Stance != FleetStance.Aggressive ) continue;
            var unit = new FleetSelection( squad, ship );
            var order = EffectiveOrder( unit );
            if ( order.Enemy != 0 || order.Service != ServiceOrder.None || order.Transfer.End != 0 ) continue;
            int target = ClosestHostile( ship, group.Faction, 30 );
            if ( target >= 0 ) Attack( unit, target );
        }
    }
}
