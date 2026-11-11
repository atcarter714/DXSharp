using System.Numerics;
using System.Runtime.InteropServices;

namespace FleetBattler;

[StructLayout( LayoutKind.Sequential )]
internal readonly record struct ShipInstance( Vector2 Center, Vector2 Heading, Vector4 Tint, Vector4 Motion = default );

internal readonly record struct FleetSelection( int Squad, int Ship = -1 ) {
    internal bool IsShip => Ship >= 0;
}

internal sealed partial class FleetSimulation {
    internal const int MaxShips = 10_000;
    internal const int DefaultShips = 72;
    internal const int WingSize = 6, WingCost = 60, WingGoal = 2;
    internal const int CaptureDuration = 90, IncomeInterval = 60, IncomeAmount = 15;
    internal const double StepSeconds = 1.0 / 60;
    private const int SquadsPerFaction = 6;
    private const float Pitch = 3.9f, MaxSpeed = 36, ApproachGain = 1.8f, Response = 0.09f;
    private const float AvoidRadius = 2.55f, TurnStep = 5.3f * (float)StepSeconds;
    private readonly int[] _grid, _next, _squadOfShip;
    private readonly int _gridSide;
    private readonly Order[] _shipOrders;
    private readonly float[] _thrust;
    private readonly BodyPose[] _bodies = new BodyPose[CelestialSystem.BodyCount];
    private readonly Vector2[] _positions, _velocities, _headings, _offsets;
    private readonly Squad[] _squads = new Squad[2 * SquadsPerFaction + WingGoal];
    private readonly bool _rivalPatrol;
    private readonly float _targetLimit;
    private int _amberFirst, _incomeTicks;
    private uint _seed;

    private struct Squad {
        internal int First, Count, Faction, Alive;
        internal Vector2 Home, Center, Direction, Anchor;
        internal Order Order;
    }

    private struct Order {
        internal Vector2 Target;
        internal float Phase;
        internal byte Body;
        internal int Enemy;
        internal bool Detached;
    }

    internal FleetSimulation( int ships, uint seed, bool rivalPatrol = false, int system = 0, bool battle = false ) {
        if ( ships is < 2 or > MaxShips ) throw new ArgumentOutOfRangeException( nameof(ships) );
        if ( (uint)system >= CelestialSystem.Systems.Length ) throw new ArgumentOutOfRangeException( nameof(system) );
        SystemIndex = system;
        _seed = seed;
        _rivalPatrol = rivalPatrol;
        int capacity = ships + WingGoal * WingSize;
        _battle = battle;
        _hull = new byte[capacity];
        _incoming = new byte[capacity];
        _nextShot = new int[capacity];
        _positions = new Vector2[capacity];
        _velocities = new Vector2[capacity];
        _headings = new Vector2[capacity];
        _offsets = new Vector2[capacity];
        _shipOrders = new Order[capacity];
        _squadOfShip = new int[capacity];
        _thrust = new float[capacity];
        _next = new int[capacity];
        int cyan = (ships + 1) / 2;
        int largestSquad = (cyan + SquadsPerFaction - 1) / SquadsPerFaction;
        float formationRadius = MathF.Ceiling( MathF.Sqrt( largestSquad * 1.6f ) ) * Pitch;
        BoardRadius = MathF.Max( 92 + system * 12, formationRadius * 2.85f + 22 );
        _gridSide = (int)MathF.Ceiling( 2.8f * BoardRadius / AvoidRadius ) + 3;
        _grid = new int[_gridSide * _gridSide];
        CelestialSystem.Evaluate( SystemIndex, 0, BoardRadius, _bodies );
        _targetLimit = BoardRadius - formationRadius * 0.6f;
        Site = new( BoardRadius * 0.14f, -BoardRadius * 0.07f );
        SiteRadius = MathF.Max( 8, BoardRadius * 0.093f );
        AddFleet( cyan, 0 );
        _amberFirst = SquadCount;
        AddFleet( ships - cyan, 1 );
    }

    internal int SystemIndex { get; }
    internal ReadOnlySpan<BodyPose> Bodies => _bodies;
    internal float BoardRadius { get; }
    internal Vector2 Site { get; }
    internal float SiteRadius { get; }
    internal Vector2 Shipyard => new( -0.59f * BoardRadius, 0.62f * BoardRadius );
    internal int Count { get; private set; }
    internal int Capacity => _positions.Length;
    internal int SquadCount { get; private set; }
    internal int Tick { get; private set; }
    internal int Owner { get; private set; } = -1;
    internal int Capturing { get; private set; } = -1;
    internal int CaptureTicks { get; private set; }
    internal bool Contested { get; private set; }
    internal int Ore { get; private set; }
    internal int WingsBuilt { get; private set; }
    internal ReadOnlySpan<Vector2> Positions => _positions.AsSpan( 0, Count );

    internal Vector2 Center( int squad ) => _squads[squad].Center;
    internal Vector2 Target( int squad ) => _squads[squad].Anchor;
    internal Vector2 Center( FleetSelection selected ) => selected.IsShip ? _positions[selected.Ship] : Center( selected.Squad );
    internal Vector2 Target( FleetSelection selected ) =>
        selected.IsShip && _shipOrders[selected.Ship].Detached ? _shipOrders[selected.Ship].Target : Target( selected.Squad );
    internal bool Orbiting( FleetSelection selected ) => (selected.IsShip && _shipOrders[selected.Ship].Detached
        ? _shipOrders[selected.Ship].Body : _squads[selected.Squad].Order.Body) != 0;
    internal int ShipCount( int squad ) => _squads[squad].Alive;

    internal int FindSquad( Vector2 point ) {
        int closest = 0;
        float best = float.PositiveInfinity;
        for ( int i = 0; i < SquadCount; ++i ) {
            if ( _squads[i].Faction != 0 || _squads[i].Alive == 0 ) continue;
            float distance = Vector2.DistanceSquared( point, _squads[i].Center );
            if ( distance >= best ) continue;
            best = distance;
            closest = i;
        }
        return closest;
    }

    internal FleetSelection Pick( Vector2 location, bool group, float maxDistance ) {
        int closest = -1;
        float best = maxDistance * maxDistance;
        for ( int i = 0; i < Count; ++i ) {
            if ( !Alive( i ) || SideOf( i ) != 0 ) continue;
            float distance = Vector2.DistanceSquared( _positions[i], location );
            if ( distance >= best ) continue;
            best = distance;
            closest = i;
        }
        return closest >= 0 ? new( _squadOfShip[closest], group ? -1 : closest ) : new( FindSquad( location ) );
    }

    internal void Move( int squad, Vector2 destination ) => Move( new FleetSelection( squad ), destination );

    internal void Move( FleetSelection selected, Vector2 destination ) {
        if ( !float.IsFinite( destination.X ) || !float.IsFinite( destination.Y ) )
            throw new ArgumentOutOfRangeException( nameof(destination) );
        var target = Vector2.Clamp( destination, new( -_targetLimit ), new( _targetLimit ) );
        SetOrder( selected, new Order { Target = target, Detached = selected.IsShip } );
    }

    internal void Orbit( FleetSelection selected, int body ) {
        if ( (uint)body >= CelestialSystem.BodyCount ) throw new ArgumentOutOfRangeException( nameof(body) );
        var delta = Center( selected ) - _bodies[body].Center;
        SetOrder( selected, new Order { Body = (byte)(body + 1), Phase = MathF.Atan2( delta.Y, delta.X ), Detached = selected.IsShip } );
    }

    internal void Rejoin( FleetSelection selected ) {
        if ( selected.IsShip ) _shipOrders[selected.Ship] = default;
    }

    private void SetOrder( FleetSelection selected, Order order ) {
        if ( (uint)selected.Squad >= (uint)SquadCount ||
             selected.IsShip && ((uint)selected.Ship >= (uint)Count || _squadOfShip[selected.Ship] != selected.Squad) )
            throw new ArgumentOutOfRangeException( nameof(selected) );
        if ( selected.IsShip ) {
            _shipOrders[selected.Ship] = order;
            return;
        }
        ref var squad = ref _squads[selected.Squad];
        squad.Order = order;
        if ( order.Body == 0 && Vector2.DistanceSquared( order.Target, squad.Center ) > 0.25f )
            squad.Direction = Vector2.Normalize( order.Target - squad.Center );
        _shipOrders.AsSpan( squad.First, squad.Count ).Clear( );
    }

    internal float OrbitRadius( int body, int count ) =>
        _bodies[body].Radius * (body == 2 ? 2.4f : 1.35f) + 6 + (count == 1 ? 0 : MathF.Sqrt( count ) * Pitch * 0.55f);

    internal int BuildWing( ) {
        if ( Ore < WingCost || WingsBuilt == WingGoal || Lost ) return -1;
        int index = SquadCount;
        var arrival = Shipyard + new Vector2( WingsBuilt * 13, 0 );
        AddSquad( WingSize, 0, arrival );
        Ore -= WingCost;
        ++WingsBuilt;
        return index;
    }

    internal void Step( ) {
        UpdateRival( );

        CelestialSystem.Evaluate( SystemIndex, Tick + 1, BoardRadius, _bodies );
        Array.Fill( _grid, -1 );
        for ( int ship = Count - 1; ship >= 0; --ship ) {
            if ( !Alive( ship ) ) continue;
            var cell = Cell( _positions[ship] );
            int index = cell.Y * _gridSide + cell.X;
            _next[ship] = _grid[index];
            _grid[index] = ship;
        }

        ResolveVolley( );
        int cyan = 0, amber = 0;
        for ( int i = 0; i < SquadCount; ++i ) {
            ref var squad = ref _squads[i];
            if ( squad.Alive == 0 ) continue;
            if ( squad.Order.Enemy != 0 ) {
                squad.Anchor = Chase( ref squad.Order, squad.Center, squad.Faction, out var facing );
                squad.Direction = facing;
            } else {
                squad.Anchor = Resolve( ref squad.Order, squad.Alive, out var tangent );
                if ( squad.Order.Body != 0 ) squad.Direction = tangent;
            }
            var sum = Vector2.Zero;
            int inZone = 0;
            int end = squad.First + squad.Count;
            for ( int ship = squad.First; ship < end; ++ship ) {
                if ( !Alive( ship ) ) continue;
                ref var solo = ref _shipOrders[ship];
                var goal = solo.Detached
                    ? solo.Enemy != 0 ? Chase( ref solo, _positions[ship], squad.Faction, out _ ) : Resolve( ref solo, 1, out _ )
                    : squad.Anchor + RotateSlot( _offsets[ship], squad.Direction );
                var error = goal - _positions[ship];
                var avoid = Separation( ship );
                var desired = error * ApproachGain + avoid;
                float speedSquared = desired.LengthSquared( );
                if ( speedSquared > MaxSpeed * MaxSpeed ) desired *= MaxSpeed / MathF.Sqrt( speedSquared );
                if ( speedSquared > 0.25f ) {
                    var wish = Vector2.Normalize( desired );
                    var facing = _headings[ship];
                    float turn = Math.Clamp( MathF.Atan2( facing.X * wish.Y - facing.Y * wish.X, Vector2.Dot( facing, wish ) ),
                        -TurnStep, TurnStep );
                    var (sin, cos) = MathF.SinCos( turn );
                    _headings[ship] = Vector2.Normalize( new Vector2( facing.X * cos - facing.Y * sin, facing.X * sin + facing.Y * cos ) );
                    desired *= 0.22f + 0.78f * MathF.Max( Vector2.Dot( _headings[ship], wish ), 0 );
                }
                var velocity = Vector2.Lerp( _velocities[ship], desired, Response );
                if ( error.LengthSquared( ) < 0.0001f && velocity.LengthSquared( ) < 0.0001f && avoid == Vector2.Zero ) {
                    _positions[ship] = goal;
                    velocity = Vector2.Zero;
                } else _positions[ship] += velocity * (float)StepSeconds;
                _velocities[ship] = velocity;
                _thrust[ship] = Math.Clamp( velocity.Length( ) / 20, 0, 1 );
                sum += _positions[ship];
                if ( Vector2.DistanceSquared( _positions[ship], Site ) <= SiteRadius * SiteRadius ) ++inZone;
            }
            squad.Center = sum / squad.Alive;
            if ( inZone * 2 >= squad.Alive ) {
                if ( squad.Faction == 0 ) ++cyan;
                else ++amber;
            }
        }
        AdvanceSite( cyan, amber );
        AdvancePlanets( );
        ++Tick;
    }

    private Vector2 Resolve( ref Order order, int count, out Vector2 tangent ) {
        tangent = Vector2.UnitX;
        if ( order.Body == 0 ) return order.Target;
        int body = order.Body - 1;
        float radius = OrbitRadius( body, count );
        order.Phase = (order.Phase + 14 * (float)StepSeconds / radius) % MathF.Tau;
        var (sin, cos) = MathF.SinCos( order.Phase );
        tangent = new( -sin, cos );
        order.Target = _bodies[body].Center + new Vector2( cos, sin ) * radius;
        return order.Target;
    }

    private (int X, int Y) Cell( Vector2 position ) =>
        (Math.Clamp( (int)((position.X + BoardRadius * 1.4f) / AvoidRadius), 1, _gridSide - 2 ),
         Math.Clamp( (int)((position.Y + BoardRadius * 1.4f) / AvoidRadius), 1, _gridSide - 2 ));

    private Vector2 Separation( int ship ) {
        var position = _positions[ship];
        var cell = Cell( position );
        var steering = Vector2.Zero;
        for ( int y = cell.Y - 1; y <= cell.Y + 1; ++y ) {
            for ( int x = cell.X - 1; x <= cell.X + 1; ++x ) {
                int other = _grid[y * _gridSide + x];
                // Bound work even when a stress scenario piles many units into one cell.
                for ( int visited = 0; other >= 0 && visited < 6; ++visited, other = _next[other] ) {
                    if ( other == ship || !Alive( other ) ) continue;
                    var delta = position - _positions[other];
                    float distanceSquared = delta.LengthSquared( );
                    if ( distanceSquared >= AvoidRadius * AvoidRadius ) continue;
                    float distance = MathF.Sqrt( distanceSquared );
                    var away = distance > 0.001f ? delta / distance : new Vector2( ship < other ? -1 : 1, 0 );
                    steering += away * (AvoidRadius - distance) * 3.8f;
                }
            }
        }
        return steering;
    }

    private static Vector2 RotateSlot( Vector2 offset, Vector2 heading ) =>
        new Vector2( -heading.Y, heading.X ) * offset.X - heading * offset.Y;

    private void AdvanceSite( int cyan, int amber ) {
        Contested = cyan != 0 && amber != 0;
        int dominant = cyan == amber ? -1 : cyan > amber ? 0 : 1;
        if ( dominant != -1 && dominant != Owner ) {
            if ( Capturing != dominant ) {
                CaptureTicks = 0;
                Capturing = dominant;
            }
            if ( ++CaptureTicks == CaptureDuration ) {
                Owner = dominant;
                Capturing = -1;
                CaptureTicks = 0;
                _incomeTicks = 0;
            }
        } else {
            Capturing = -1;
            CaptureTicks = 0;
        }

        if ( Owner != 0 || dominant != 0 || WingsBuilt == WingGoal ) {
            _incomeTicks = 0;
            return;
        }
        if ( ++_incomeTicks == IncomeInterval ) {
            Ore = Math.Min( Ore + IncomeAmount, 999 );
            _incomeTicks = 0;
        }
    }

    internal void FillInstances( Span<ShipInstance> destination, FleetSelection selected, ReadOnlySpan<bool> selection = default ) {
        if ( destination.Length != ActiveCount ) throw new ArgumentException( "Instance span must match the active ship count.", nameof(destination) );
        int index = 0;
        for ( int i = 0; i < SquadCount; ++i ) {
            ref readonly var squad = ref _squads[i];
            var tint = squad.Faction == 0 ? new Vector4( 0.66f, 1.55f, 2.25f, 1.04f )
                : new Vector4( 2.9f, 0.89f, 0.37f, 1.11f );
            int end = squad.First + squad.Count;
            for ( int ship = squad.First; ship < end; ++ship ) {
                if ( !Alive( ship ) ) continue;
                bool highlighted = selection.IsEmpty ? ship == selected.Ship || !selected.IsShip && i == selected.Squad : selection[ship];
                var color = tint;
                if ( highlighted ) color.W = 1.66f;
                destination[index++] = new( _positions[ship], _headings[ship], color,
                    new Vector4( _thrust[ship], (ship % 173) * 0.618f, 1, (float)_hull[ship] / HullCapacity + (highlighted ? 2 : 0) ) );
            }
        }
    }

    internal ulong StateHash( ) {
        ulong hash = 14695981039346656037;
        Append( (uint)SystemIndex ); Append( (uint)Tick ); Append( (uint)Count ); Append( (uint)SquadCount ); Append( _seed );
        Append( (uint)Owner ); Append( (uint)Capturing ); Append( (uint)CaptureTicks ); Append( Contested ? 1u : 0u );
        Append( (uint)ShotsFired ); Append( (uint)ShipsLost ); Append( (uint)Science ); Append( WeaponUpgrade ? 1u : 0u ); Append( (uint)_surveyRequests );
        for ( int body = 0; body < _bodies.Length; ++body ) {
            Append( (uint)_survey[body] ); Append( (uint)_mines[body] ); Append( (uint)_labs[body] );
        }
        Append( (uint)Ore ); Append( (uint)WingsBuilt ); Append( (uint)_incomeTicks ); Append( _rivalPatrol ? 1u : 0u );
        for ( int i = 0; i < SquadCount; ++i ) {
            ref readonly var squad = ref _squads[i];
            Append( (uint)squad.Count ); Append( (uint)squad.Faction );
            AppendVector( squad.Home ); AppendVector( squad.Center ); AppendVector( squad.Direction ); AppendOrder( squad.Order );
        }
        for ( int i = 0; i < Count; ++i ) {
            AppendVector( _offsets[i] ); AppendVector( _positions[i] );
            AppendVector( _velocities[i] ); AppendVector( _headings[i] ); AppendOrder( _shipOrders[i] );
            Append( _hull[i] ); Append( (uint)_nextShot[i] );
        }
        return hash;

        void AppendOrder( Order order ) {
            AppendVector( order.Target ); Append( BitConverter.SingleToUInt32Bits( order.Phase ) );
            Append( order.Body ); Append( order.Detached ? 1u : 0u ); Append( (uint)order.Enemy );
        }

        void AppendVector( Vector2 value ) {
            Append( BitConverter.SingleToUInt32Bits( value.X ) );
            Append( BitConverter.SingleToUInt32Bits( value.Y ) );
        }

        void Append( uint bits ) {
            for ( int i = 0; i < 32; i += 8 )
                hash = unchecked((hash ^ (byte)(bits >> i)) * 1099511628211);
        }
    }

    private void AddFleet( int ships, int faction ) {
        int squads = Math.Min( ships, SquadsPerFaction );
        for ( int i = 0; i < squads; ++i ) {
            float angle = -0.93f + 1.86f * (i + 0.5f) / squads;
            float radius = BoardRadius * (i % 2 == 0 ? 0.56f : 0.77f);
            var home = new Vector2( MathF.Cos( angle ) * (faction == 0 ? -1 : 1), MathF.Sin( angle ) ) * radius;
            AddSquad( ships / squads + (i < ships % squads ? 1 : 0), faction, home );
        }
    }

    private void AddSquad( int count, int faction, Vector2 center ) {
        var heading = faction == 0 ? Vector2.UnitX : -Vector2.UnitX;
        int columns = (int)MathF.Ceiling( MathF.Sqrt( count * 1.6f ) );
        var mean = Vector2.Zero;
        for ( int i = 0; i < count; ++i ) {
            int row = count <= 10 ? (int)((MathF.Sqrt( 8 * i + 1 ) - 1) * 0.5f) : i / columns;
            float column = count <= 10 ? i - row * (row + 2) * 0.5f : i % columns + (row & 1) * 0.5f;
            var jitter = (new Vector2( NextUnit( ), NextUnit( ) ) - new Vector2( 0.5f )) * 0.28f;
            var local = new Vector2( column, row * 0.91f ) * Pitch + jitter;
            _offsets[Count + i] = local;
            mean += _offsets[Count + i];
        }
        mean /= count;
        for ( int i = Count; i < Count + count; ++i ) {
            _offsets[i] -= mean;
            _positions[i] = center + RotateSlot( _offsets[i], heading );
            _squadOfShip[i] = SquadCount;
            _hull[i] = HullCapacity;
            _headings[i] = heading;
        }
        _squads[SquadCount++] = new( ) { First = Count, Count = count, Alive = count, Faction = faction, Home = center, Center = center,
            Anchor = center, Direction = heading, Order = new Order { Target = center } };
        Count += count;
        _factionCount[faction] += count;
    }

    private float NextUnit( ) {
        _seed = unchecked(_seed + 0x9E3779B9u);
        uint bits = _seed;
        bits = unchecked((bits ^ (bits >> 16)) * 0x85EBCA6Bu);
        bits = unchecked((bits ^ (bits >> 13)) * 0xC2B2AE35u);
        return ((bits ^ (bits >> 16)) >> 8) * (1f / 16_777_216);
    }
}
