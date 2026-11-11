using System.Numerics;
using System.Runtime.InteropServices;

namespace FleetBattler;

[StructLayout( LayoutKind.Sequential )]
internal readonly record struct BattleEffect( Vector4 Segment, Vector4 Style );

internal sealed partial class FleetSimulation {
    internal const int HullCapacity = 72, EffectCapacity = 512;
    private const int ShotDuration = 9, BurstDuration = 23;
    private readonly bool _battle;
    private readonly byte[] _hull, _incoming;
    private readonly int[] _nextShot;
    private readonly int[] _factionCount = new int[2];
    private readonly CombatEvent[] _events = new CombatEvent[EffectCapacity];
    private int _eventCursor;

    private readonly record struct CombatEvent( Vector2 From, Vector2 To, int Tick, int Faction, bool Burst );

    internal int ActiveCount => CyanCount + AmberCount;
    internal int CyanCount => _factionCount[0];
    internal int AmberCount => _factionCount[1];
    internal int ShotsFired { get; private set; }
    internal int ShipsLost { get; private set; }
    internal int LastEngagementTick { get; private set; } = -10_000;
    internal bool InBattle => Tick - LastEngagementTick < 150;
    internal bool Won => WingsBuilt == WingGoal || AmberCount == 0 && CyanCount > 0;
    internal bool Lost => CyanCount == 0 && !Won;

    internal bool Alive( int ship ) => (uint)ship < (uint)Count && _hull[ship] > 0;
    internal int SquadronOf( int ship ) => _squadOfShip[ship];
    internal int SideOf( int ship ) => _squads[_squadOfShip[ship]].Faction;
    internal int Hull( int ship ) => _hull[ship];

    internal void Attack( FleetSelection selection, int target ) {
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
        if ( !_battle || CyanCount == 0 || AmberCount == 0 ) return;
        for ( int ship = 0; ship < Count; ++ship ) {
            if ( !Alive( ship ) || Tick < _nextShot[ship] || (Tick + ship) % 4 != 0 ) continue;
            int side = SideOf( ship );
            float range = side == 0 && WeaponUpgrade ? 19 : 16;
            int preferred = (_shipOrders[ship].Detached ? _shipOrders[ship].Enemy : _squads[_squadOfShip[ship]].Order.Enemy) - 1;
            int target = Alive( preferred ) && Vector2.DistanceSquared( _positions[preferred], _positions[ship] ) <= range * range
                ? preferred : ClosestHostile( ship, side, range );
            if ( target < 0 ) continue;
            int damage = side == 0 ? WeaponUpgrade ? 17 : 13 : 11;
            _incoming[target] = (byte)Math.Min( HullCapacity, _incoming[target] + damage );
            _nextShot[ship] = Tick + 38 + ship % 9;
            ++ShotsFired;
            LastEngagementTick = Tick;
            Emit( _positions[ship], _positions[target], side, burst: false );
        }

        for ( int ship = 0; ship < Count; ++ship ) {
            int damage = _incoming[ship];
            if ( damage == 0 ) continue;
            _incoming[ship] = 0;
            _hull[ship] = (byte)Math.Max( 0, _hull[ship] - damage );
            if ( Alive( ship ) ) continue;
            ref var squad = ref _squads[_squadOfShip[ship]];
            --squad.Alive;
            --_factionCount[squad.Faction];
            ++ShipsLost;
            Emit( _positions[ship], _positions[ship], squad.Faction, burst: true );
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

    private void Emit( Vector2 from, Vector2 to, int faction, bool burst ) {
        _events[_eventCursor % EffectCapacity] = new( from, to, Tick, faction, burst );
        ++_eventCursor;
    }

    internal int FillEffects( Span<BattleEffect> output ) {
        int count = 0;
        for ( int i = 0; i < Math.Min( EffectCapacity, _eventCursor ) && count < output.Length; ++i ) {
            var pulse = _events[i];
            int lifetime = pulse.Burst ? BurstDuration : ShotDuration;
            int age = Tick - pulse.Tick;
            if ( age >= lifetime ) continue;
            output[count++] = new( new Vector4( pulse.From.X, pulse.From.Y, pulse.To.X, pulse.To.Y ),
                new Vector4( (float)age / lifetime, pulse.Faction, pulse.Burst ? 1 : 0, 0 ) );
        }
        return count;
    }
}
