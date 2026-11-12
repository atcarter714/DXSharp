using System.Numerics;
using System.Runtime.InteropServices;

namespace FleetBattler;

[StructLayout( LayoutKind.Sequential )]
internal readonly record struct BattleEffect( Vector4 Segment, Vector4 Style );

internal sealed partial class FleetSimulation {
    internal const int HullCapacity = 72, EffectCapacity = 512;
    private const int ShotDuration = 9, BurstDuration = 23;
    private readonly bool _battle;
    private readonly byte[] _hull, _incoming, _shield, _armor;
    private readonly ushort[] _railIncoming;
    private readonly int[] _lastHit;
    private readonly int[] _nextShot;
    private readonly int[] _factionCount = new int[2];
    private readonly CombatEvent[] _events = new CombatEvent[EffectCapacity];
    private int _eventCursor, _eventCount;

    private readonly record struct CombatEvent( Vector2 From, Vector2 To, int Tick, int Faction, bool Burst, WeaponKind Weapon );

    internal int ActiveCount => CyanCount + AmberCount;
    internal int CyanCount => _factionCount[0];
    internal int AmberCount => _factionCount[1];
    internal int ShotsFired { get; private set; }
    internal int ShipsLost { get; private set; }
    internal int LastEngagementTick { get; private set; } = -10_000;
    internal bool InBattle => Tick - LastEngagementTick < 150;
    internal bool Won => Frontier ? _secured : WingsBuilt == WingGoal || AmberCount == 0 && CyanCount > 0;
    internal bool Lost => !Frontier && CyanCount == 0 && !Won;

    internal bool Alive( int ship ) => (uint)ship < (uint)Count && _hull[ship] > 0;
    internal int SquadronOf( int ship ) => _squadOfShip[ship];
    internal int SideOf( int ship ) => _squads[_squadOfShip[ship]].Faction;
    internal int Hull( int ship ) => _hull[ship];
    internal int Shield( int ship ) => _shield[ship];
    internal int Armor( int ship ) => _armor[ship];

    internal void Attack( FleetSelection selection, int target ) {
        if ( Frontier && Role( selection.Squad ) != HullRole.Corvette ) return;
        if ( !Alive( target ) || _squads[selection.Squad].Faction == SideOf( target ) ) return;
        SetOrder( selection, new Order {
            Enemy = target + 1, Target = _positions[target], Detached = selection.IsShip
        } );
    }

    private Vector2 Chase( ref Order order, Vector2 from, int faction, out Vector2 facing ) {
        int target = order.Enemy - 1;
        if ( !Alive( target ) ) {
            int squad = _squadOfShip[target];
            var unit = _squads[squad];
            target = -1;
            for ( int i = unit.First; i < unit.First + unit.Count; ++i ) {
                    if ( !Alive( i ) ) continue;
                target = i;
                break;
            }
            order.Enemy = target + 1;
            if ( target < 0 ) {
                order.Target = from;
                facing = Vector2.UnitX;
                return from;
            }
        }
        Vector2 delta = _positions[target] - from;
        float distance = delta.Length( );
        facing = distance > 0.01f ? delta / distance : Vector2.UnitX;
        float standOff = faction == 0 && WeaponUpgrade ? 12 : 10;
        order.Target = _positions[target] - facing * standOff;
        return order.Target;
    }

    private void UpdateRival( ) {
        if ( !_rivalPatrol || Tick < 240 || (Tick - 240) % 90 != 0 || CyanCount == 0 ) return;
        int reserves = Tick >= 1_140 ? 3 : Tick >= 600 ? 2 : 1;
        int dispatched = 0;
        for ( int i = _amberFirst; i < 2 * SquadsPerFaction && i < SquadCount && dispatched < reserves; ++i ) {
            ref var squad = ref _squads[i];
            if ( squad.Faction != 1 || squad.Alive == 0 ) continue;
            ++dispatched;
            int target = -1;
            float nearest = MathF.Pow( SiteRadius + 20, 2 );
            for ( int enemy = 0; enemy < Count; ++enemy ) {
                if ( !Alive( enemy ) || SideOf( enemy ) != 0 ) continue;
                float threat = Vector2.DistanceSquared( _positions[enemy], Site );
                if ( threat >= nearest ) continue;
                nearest = threat;
                target = enemy;
            }
            if ( _battle && target >= 0 ) {
                if ( squad.Order.Enemy != target + 1 ) Attack( new( i ), target );
            } else if ( squad.Order.Enemy != 0 || squad.Order.Target != Site ) Move( i, Site );
        }

        for ( int i = _amberFirst; i < SquadCount; ++i ) {
            ref var squad = ref _squads[i];
            if ( squad.Faction != 1 || squad.Alive == 0 || squad.Order.Enemy != 0 || squad.Order.Target == Site ) continue;
            float direction = ((Tick / 360 + i) & 1) == 0 ? -1 : 1;
            Move( i, squad.Home + new Vector2( -5, direction * 6 ) );
        }
    }

    private void ResolveVolley( ) {
        if ( !_battle || CyanCount == 0 ) return;
        if ( Frontier && Tick % 15 == 0 ) {
            for ( int ship = 0; ship < Count; ++ship )
                if ( Alive( ship ) && Role( SquadronOf( ship ) ) == HullRole.Corvette && Tick - _lastHit[ship] >= 180 )
                    _shield[ship] = (byte)Math.Min( Loadout( ship ).Shield, _shield[ship] + 2 );
        }
        if ( AmberCount == 0 ) return;
        for ( int ship = 0; ship < Count; ++ship ) {
            if ( !Alive( ship ) || Tick < _nextShot[ship] || (Tick + ship) % 4 != 0 ) continue;
            if ( Frontier && Role( SquadronOf( ship ) ) != HullRole.Corvette ) continue;
            int side = SideOf( ship );
            var design = Loadout( ship );
            var weapon = Frontier ? design.Weapon : WeaponKind.Pulse;
            float range = Frontier ? design.Range + (side == 0 && WeaponUpgrade ? 3 : 0) : side == 0 && WeaponUpgrade ? 19 : 16;
            int preferred = (_shipOrders[ship].Detached ? _shipOrders[ship].Enemy : _squads[_squadOfShip[ship]].Order.Enemy) - 1;
            int target = Alive( preferred ) && Vector2.DistanceSquared( _positions[preferred], _positions[ship] ) <= range * range
                ? preferred : ClosestHostile( ship, side, range );
            if ( target < 0 ) continue;
            int damage = Frontier ? design.Damage + (side == 0 && WeaponUpgrade ? 4 : 0) : side == 0 ? WeaponUpgrade ? 17 : 13 : 11;
            if ( weapon == WeaponKind.Rail ) _railIncoming[target] = (ushort)Math.Min( 512, _railIncoming[target] + damage );
            else _incoming[target] = (byte)Math.Min( Frontier ? 255 : HullCapacity, _incoming[target] + damage );
            _nextShot[ship] = Tick + (Frontier ? design.Reload : 38) + ship % 9;
            ++ShotsFired;
            LastEngagementTick = Tick;
            Emit( _positions[ship], _positions[target], side, burst: false, weapon );
        }

        for ( int ship = 0; ship < Count; ++ship ) {
            int damage = _incoming[ship];
            int rail = _railIncoming[ship];
            if ( damage == 0 && rail == 0 ) continue;
            _incoming[ship] = 0;
            _railIncoming[ship] = 0;
            if ( Frontier ) {
                _lastHit[ship] = Tick;
                int shield = Math.Min( _shield[ship], damage );
                _shield[ship] -= (byte)shield;
                damage -= shield;
                // Rail rounds bypass shields. Armor absorbs rail and residual pulse damage before hull.
                damage += rail;
                int armor = Math.Min( _armor[ship], damage );
                _armor[ship] -= (byte)armor;
                damage -= armor;
            }
            ApplyHullDamage( ship, damage );
        }
    }

    internal void ApplyHullDamage( int ship, int damage ) {
        if ( !Alive( ship ) || damage <= 0 ) return;
        _hull[ship] = (byte)Math.Max( 0, _hull[ship] - damage );
        if ( Alive( ship ) ) return;
        ref var squad = ref _squads[_squadOfShip[ship]];
        --squad.Alive;
        --_factionCount[squad.Faction];
        ++ShipsLost;
        Emit( _positions[ship], _positions[ship], squad.Faction, burst: true );
    }

    private void ClearRetiredOrders( int squad, int first, int count ) {
        RefreshSurveys( );
        for ( int body = 1; body < _constructionWorkers.Length; ++body )
            if ( _constructionWorkers[body]?.Squad == squad ) AdvanceMine( body );
        for ( int i = 0; i < SquadCount; ++i ) ClearTarget( ref _squads[i].Order, _squads[i].Center );
        for ( int ship = 0; ship < Count; ++ship ) ClearTarget( ref _shipOrders[ship], _positions[ship] );

        void ClearTarget( ref Order order, Vector2 position ) {
            if ( (uint)(order.Enemy - 1 - first) >= (uint)count ) return;
            order.Enemy = 0;
            order.Target = position;
        }
    }

    private int ClosestHostile( int ship, int side, float range ) {
        var origin = _positions[ship];
        var cell = Cell( origin );
        int reach = (int)MathF.Ceiling( range / AvoidRadius ) + 1;
        float best = range * range;
        int target = -1;
        for ( int y = Math.Max( 1, cell.Y - reach ); y <= Math.Min( _gridSide - 2, cell.Y + reach ); ++y ) {
            for ( int x = Math.Max( 1, cell.X - reach ); x <= Math.Min( _gridSide - 2, cell.X + reach ); ++x ) {
                int other = _grid[y * _gridSide + x];
                for ( int visited = 0; other >= 0 && visited < 8; ++visited, other = _next[other] ) {
                    if ( SideOf( other ) == side ) continue;
                    float d = Vector2.DistanceSquared( origin, _positions[other] );
                    if ( d >= best ) continue;
                    best = d;
                    target = other;
                }
            }
        }
        return target;
    }

    private void Emit( Vector2 from, Vector2 to, int faction, bool burst, WeaponKind weapon = WeaponKind.None ) {
        _events[_eventCursor] = new( from, to, Tick, faction, burst, weapon );
        _eventCursor = (_eventCursor + 1) % EffectCapacity;
        _eventCount = Math.Min( _eventCount + 1, EffectCapacity );
    }

    internal int FillEffects( Span<BattleEffect> output ) {
        int count = 0;
        for ( int i = 0; i < _eventCount && count < output.Length; ++i ) {
            var pulse = _events[i];
            int lifetime = pulse.Burst ? BurstDuration : ShotDuration;
            int age = Tick - pulse.Tick;
            if ( age >= lifetime ) continue;
            output[count++] = new( new Vector4( pulse.From.X, pulse.From.Y, pulse.To.X, pulse.To.Y ),
                new Vector4( (float)age / lifetime, pulse.Faction, pulse.Burst ? 1 : 0, pulse.Weapon == WeaponKind.Rail ? 1 : 0 ) );
        }
        return count;
    }
}
