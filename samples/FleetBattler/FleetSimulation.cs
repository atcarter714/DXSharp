using System.Numerics;
using System.Runtime.InteropServices;

namespace FleetBattler;

[StructLayout( LayoutKind.Sequential )]
internal readonly record struct ShipInstance( Vector2 Center, Vector2 Heading, Vector4 Tint, Vector4 Motion = default );

internal readonly record struct FleetSelection( int Squad, int Ship = -1 ) {
    internal bool IsShip => Ship >= 0;
}

internal enum FlightOrder { Idle, Move, Orbit, Survey, Attack }
internal readonly record struct FlightPlan( FlightOrder Order, Vector2 Destination, int Body, int Target );

internal sealed class FleetClock {
    private double _remaining;
    internal float Interpolation => (float)(_remaining / FleetSimulation.StepSeconds);
    internal void Reset( ) => _remaining = 0;

    internal float Advance( FleetSimulation fleet, double elapsed, int speed, bool paused, Action? afterStep = null ) {
        if ( paused ) return 0;
        double scaled = Math.Clamp( elapsed, 0, 0.125 ) * (speed * 0.25);
        _remaining += scaled;
        while ( _remaining >= FleetSimulation.StepSeconds ) {
            fleet.Step( );
            afterStep?.Invoke( );
            _remaining -= FleetSimulation.StepSeconds;
        }
        return (float)scaled;
    }
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
    internal const float OrbitalTurnStep = 2.8f * (float)StepSeconds;
    private readonly int[] _grid, _next, _squadOfShip;
    private readonly int _gridSide;
    private readonly Order[] _shipOrders;
    private readonly float[] _thrust;
    private readonly BodyPose[] _bodies, _previousBodies;
    private readonly Vector2[] _positions, _previousPositions, _velocities, _headings, _previousHeadings, _offsets;
    private readonly Squad[] _squads;
    private readonly int[] _shipGeneration;
    private readonly bool _rivalPatrol;
    private readonly float _targetLimit;
    private int _amberFirst, _incomeTicks;
    private uint _seed;

    private struct Squad {
        internal int First, Count, Faction, Alive, FleetId;
        internal HullRole Role;
        internal FleetStance Stance;
        internal ShipDesign Design;
        internal Vector2 Home, Center, Direction, Anchor;
        internal Order Order;
    }

    private struct Order {
        internal Vector2 Target;
        internal float Phase, Radius;
        internal Transfer Transfer;
        internal byte Body, Construction;
        internal ServiceOrder Service;
        internal int Enemy, ServiceTicks;
        internal bool Detached, Survey, Waypoint, Rejoining, Settlement;
    }

    internal FleetSimulation( int ships, uint seed, bool rivalPatrol = false, int system = 0, bool battle = false, bool frontier = false ) {
        if ( ships is < 2 or > MaxShips ) throw new ArgumentOutOfRangeException( nameof(ships) );
        if ( (uint)system >= CelestialSystem.Systems.Length ) throw new ArgumentOutOfRangeException( nameof(system) );
        SystemIndex = system;
        Frontier = frontier;
        _bodies = new BodyPose[frontier ? (system == 0 ? CelestialSystem.BodyCount : CelestialSystem.FrontierBodyCount) : CelestialSystem.OuterBelt];
        _previousBodies = new BodyPose[_bodies.Length];
        _seed = seed;
        _rivalPatrol = rivalPatrol;
        int capacity = frontier ? FrontierCapacity : ships + WingGoal * WingSize;
        _squads = new Squad[frontier ? FrontierCapacity : 2 * SquadsPerFaction + WingGoal];
        _shipGeneration = new int[capacity];
        _refitDesigns = frontier ? new ShipDesign[capacity] : [];
        _battle = battle;
        _hull = new byte[capacity];
        _shield = new byte[capacity];
        _armor = new byte[capacity];
        _lastHit = new int[capacity];
        _railIncoming = new ushort[capacity];
        _incoming = new byte[capacity];
        _nextShot = new int[capacity];
        _positions = new Vector2[capacity];
        _previousPositions = new Vector2[capacity];
        _velocities = new Vector2[capacity];
        _headings = new Vector2[capacity];
        _previousHeadings = new Vector2[capacity];
        _offsets = new Vector2[capacity];
        _shipOrders = new Order[capacity];
        _squadOfShip = new int[capacity];
        _thrust = new float[capacity];
        _next = new int[capacity];
        int cyan = frontier ? 3 : (ships + 1) / 2;
        int largestSquad = (cyan + SquadsPerFaction - 1) / SquadsPerFaction;
        float formationRadius = MathF.Ceiling( MathF.Sqrt( largestSquad * 1.6f ) ) * Pitch;
        BoardRadius = MathF.Max( 92 + system * 12, formationRadius * 2.85f + 22 );
        _gridSide = (int)MathF.Ceiling( 2.8f * NavigationRadius / AvoidRadius ) + 3;
        _grid = new int[_gridSide * _gridSide];
        CelestialSystem.Evaluate( SystemIndex, 0, BoardRadius, _bodies );
        _bodies.CopyTo( _previousBodies, 0 );
        _targetLimit = NavigationRadius - formationRadius * 0.6f;
        Site = new( BoardRadius * 0.14f, -BoardRadius * 0.07f );
        SiteRadius = MathF.Max( 8, BoardRadius * 0.093f );
        if ( frontier ) {
            if ( Solar ) OpenFrontier( );
            else {
                var contact = system switch { 3 => FactionId.Compact, 4 or 5 => FactionId.Consortium, _ => FactionId.Merchants };
                int merchant = AddSquad( 1, (int)contact, _bodies[2].Center + new Vector2( 30, 5 ), HullRole.Freighter );
                Park( merchant, 2 );
                if ( system >= 3 ) {
                    var visitor = contact == FactionId.Compact ? FactionId.Consortium : FactionId.Compact;
                    Park( AddSquad( 1, (int)visitor, _bodies[1].Center + new Vector2( -17, 6 ), HullRole.Freighter ), 1 );
                }
            }
        }
        else {
            AddFleet( cyan, 0 );
            _amberFirst = SquadCount;
            AddFleet( ships - cyan, 1 );
        }
    }

    internal int SystemIndex { get; }
    internal bool CanChartSystem( int system ) => (uint)system < CelestialSystem.Systems.Length &&
        (!Frontier || system == 0 || AstrometryUpgrade);
    internal bool Solar => Frontier && SystemIndex == 0;
    internal string BodyName( int body ) => _bodyNames[body] ?? (Solar ? SolarCatalog.Get( body ).Name : CelestialSystem.Name( SystemIndex, body ));
    internal string SystemName => Solar ? "SOLAR SYSTEM" : CelestialSystem.Systems[SystemIndex];
    internal ReadOnlySpan<BodyPose> Bodies => _bodies;
    internal BodyPose VisualBody( int body, float interpolation ) =>
        new( Vector2.Lerp( _previousBodies[body].Center, _bodies[body].Center, interpolation ), _bodies[body].Radius );
    internal float BoardRadius { get; }
    internal Vector2 Site { get; }
    internal float SiteRadius { get; }
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

    internal bool AnySelectedInSquad( int index, ReadOnlySpan<bool> selected ) {
        ref readonly var squad = ref _squads[index];
        return selected.Slice( squad.First, squad.Count ).Contains( true );
    }

    internal Vector2 Center( int squad ) => _squads[squad].Center;
    internal Vector2 Target( int squad ) => _squads[squad].Anchor;
    internal Vector2 Center( FleetSelection selected ) => selected.IsShip ? _positions[selected.Ship] : Center( selected.Squad );
    internal Vector2 VisualPosition( int ship, float interpolation ) => Vector2.Lerp( _previousPositions[ship], _positions[ship], interpolation );
    internal Vector2 VisualCenter( FleetSelection selected, float interpolation ) {
        if ( selected.IsShip ) return VisualPosition( selected.Ship, interpolation );
        ref readonly var squad = ref _squads[selected.Squad];
        var sum = Vector2.Zero;
        for ( int ship = squad.First; ship < squad.First + squad.Count; ++ship )
            if ( Alive( ship ) ) sum += VisualPosition( ship, interpolation );
        return squad.Alive == 0 ? squad.Center : sum / squad.Alive;
    }

    private Order EffectiveOrder( FleetSelection selected ) => selected.IsShip && _shipOrders[selected.Ship].Detached
        ? _shipOrders[selected.Ship] : _squads[selected.Squad].Order;

    internal bool Orbiting( FleetSelection selected ) => EffectiveOrder( selected ).Body != 0;

    internal FlightPlan Plan( FleetSelection selected ) {
        var order = EffectiveOrder( selected );
        if ( order.Enemy != 0 && Alive( order.Enemy - 1 ) )
            return new( FlightOrder.Attack, _positions[order.Enemy - 1], -1, order.Enemy - 1 );
        if ( order.Body != 0 ) {
            int body = order.Body - 1;
            if ( Frontier ) {
                var plotted = order.Transfer.End > Tick ? order.Transfer.To : Center( selected );
                return new( order.Survey ? FlightOrder.Survey : order.Waypoint && order.Transfer.End > Tick ? FlightOrder.Move : FlightOrder.Orbit,
                    plotted, order.Waypoint ? -1 : body, -1 );
            }
            int count = selected.IsShip && order.Detached ? 1 : Math.Max( 1, ShipCount( selected.Squad ) );
            var (sin, cos) = MathF.SinCos( order.Phase );
            var destination = _bodies[body].Center + new Vector2( cos, sin ) * OrbitRadius( body, count );
            return new( order.Survey ? FlightOrder.Survey : FlightOrder.Orbit, destination, body, -1 );
        }
        var target = order.Target;
        if ( selected.IsShip && !order.Detached )
            target += RotateSlot( _offsets[selected.Ship], _squads[selected.Squad].Direction );
        return new( Vector2.DistanceSquared( Center( selected ), target ) > 1 ? FlightOrder.Move : FlightOrder.Idle, target, -1, -1 );
    }
    internal static bool UsesHaulerModel( HullRole role ) => role is HullRole.Miner or HullRole.Freighter or HullRole.Colony;

    internal int ActiveFrigates {
        get {
            int count = 0;
            for ( int squad = 0; squad < SquadCount; ++squad )
                if ( Role( squad ) == HullRole.Frigate ) count += ShipCount( squad );
            return count;
        }
    }

    internal int ActiveMiners( ) {
        int count = 0;
        for ( int i = 0; i < SquadCount; ++i )
            if ( IsPlayerSquad( i ) && Role( i ) == HullRole.Miner ) count += ShipCount( i );
        return count;
    }

    internal int FirstPlayerSquad( ) {
        for ( int i = 0; i < SquadCount; ++i )
            if ( IsPlayerSquad( i ) && ShipCount( i ) > 0 ) return i;
        return -1;
    }

    internal int ShipCount( int squad ) => _squads[squad].Alive;
    internal bool IsPlayerSquad( int squad ) => _squads[squad].Faction == 0;
    internal bool IsScoutSquad( int squad ) => IsPlayerSquad( squad ) && Role( squad ) == HullRole.Scout;

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

    internal bool Move( FleetSelection selected, Vector2 destination ) {
        if ( !float.IsFinite( destination.X ) || !float.IsFinite( destination.Y ) )
            throw new ArgumentOutOfRangeException( nameof(destination) );
        var target = Vector2.Clamp( destination, new( -_targetLimit ), new( _targetLimit ) );
        if ( !CanReach( selected, target ) ) return false;
        SetOrder( selected, Frontier ? PlotTransfer( selected, 0, false, target ) : new Order { Target = target, Detached = selected.IsShip } );
        return true;
    }

    internal bool Orbit( FleetSelection selected, int body ) {
        if ( (uint)body >= (uint)_bodies.Length ) throw new ArgumentOutOfRangeException( nameof(body) );
        if ( Frontier && !CanReach( selected, body ) ) return false;
        var order = OrbitalOrder( selected, body, survey: false );
        order.Construction = 0;
        order.Service = ServiceOrder.None;
        order.Settlement = false;
        SetOrder( selected, order );
        return true;
    }

    private Order OrbitalOrder( FleetSelection selected, int body, bool survey ) {
        if ( Frontier ) return PlotTransfer( selected, body, survey );
        var delta = Center( selected ) - _bodies[body].Center;
        return new( ) { Body = (byte)(body + 1), Phase = MathF.Atan2( delta.Y, delta.X ), Detached = selected.IsShip, Survey = survey };
    }

    internal void Rejoin( FleetSelection selected ) {
        if ( !selected.IsShip ) return;
        _shipOrders[selected.Ship] = new Order { Rejoining = Frontier && _shipOrders[selected.Ship].Detached };
        RefreshSurveys( );
    }

    private void SetOrder( FleetSelection selected, Order order ) {
        if ( (uint)selected.Squad >= (uint)SquadCount ||
             selected.IsShip && ((uint)selected.Ship >= (uint)Count || _squadOfShip[selected.Ship] != selected.Squad) )
            throw new ArgumentOutOfRangeException( nameof(selected) );
        if ( selected.IsShip ) {
            _shipOrders[selected.Ship] = order;
            RefreshSurveys( );
            return;
        }
        ref var squad = ref _squads[selected.Squad];
        squad.Order = order;
        if ( order.Body == 0 && Vector2.DistanceSquared( order.Target, squad.Center ) > 0.25f )
            squad.Direction = Vector2.Normalize( order.Target - squad.Center );
        for ( int i = squad.First; i < squad.First + squad.Count; ++i )
            _shipOrders[i] = new Order { Rejoining = Frontier && (_shipOrders[i].Detached || _shipOrders[i].Rejoining) };
        RefreshSurveys( );
    }

    internal float OrbitRadius( int body, int count ) =>
        _bodies[body].Radius * (body == 2 ? 2.4f : 1.35f) + 6 + (count == 1 ? 0 : MathF.Sqrt( count ) * Pitch * 0.55f);

    internal void Step( ) {
        if ( !Frontier ) UpdateRival( );

        _bodies.CopyTo( _previousBodies, 0 );
        CelestialSystem.Evaluate( SystemIndex, Tick + 1, BoardRadius, _bodies );
        Array.Fill( _grid, -1 );
        for ( int ship = Count - 1; ship >= 0; --ship ) {
            if ( !Alive( ship ) ) continue;
            var cell = Cell( _positions[ship] );
            int index = cell.Y * _gridSide + cell.X;
            _next[ship] = _grid[index];
            _grid[index] = ship;
        }

        AdvanceAggressiveOrders( );
        ResolveVolley( );
        int cyan = 0, amber = 0;
        for ( int i = 0; i < SquadCount; ++i ) {
            ref var squad = ref _squads[i];
            if ( squad.Alive == 0 ) continue;
            var flight = Frontier ? Flight( i ) : default;
            float orbitalTurn = Frontier ? OrbitalTurnStep * flight.TurnRate : OrbitalTurnStep;
            float localSpeed = Frontier ? 1.3f * flight.Cruise : MaxSpeed;
            float localResponse = Frontier ? Math.Clamp( Response * flight.Acceleration / 55, 0.04f, 0.24f ) : Response;
            float squadDrive = 0;
            Vector2 squadAim = squad.Direction;
            if ( squad.Order.Enemy != 0 ) {
                squad.Anchor = Chase( ref squad.Order, squad.Center, i, out var facing );
                squad.Direction = facing;
                if ( Frontier && squad.Faction == 0 && squad.Order.Enemy == 0 )
                    squad.Order = PlotTransfer( new( i ), 0, false, squad.Center );
            } else {
                var tangent = Vector2.UnitX;
                squad.Anchor = Frontier && squad.Order.Body != 0 ? Navigate( ref squad.Order, out tangent, out squadAim, out squadDrive )
                    : Resolve( ref squad.Order, squad.Alive, out tangent );
                if ( squad.Order.Body != 0 ) squad.Direction = Frontier
                    ? TurnToward( squad.Direction, tangent, orbitalTurn ) : tangent;
            }
            var sum = Vector2.Zero;
            int inZone = 0;
            int end = squad.First + squad.Count;
            for ( int ship = squad.First; ship < end; ++ship ) {
                if ( !Alive( ship ) ) continue;
                _previousPositions[ship] = _positions[ship];
                _previousHeadings[ship] = _headings[ship];
                ref var solo = ref _shipOrders[ship];
                var slot = squad.Anchor + RotateSlot( _offsets[ship], squad.Direction );
                if ( solo.Rejoining && Vector2.DistanceSquared( _positions[ship], slot ) <= 0.3f ) solo = default;
                bool onCourse = Frontier && !solo.Rejoining && (solo.Detached ? solo.Body != 0 : squad.Order.Body != 0);
                float thrust = squadDrive;
                var aim = squadAim;
                var goal = solo.Detached
                    ? solo.Enemy != 0 ? Chase( ref solo, _positions[ship], i, out _ )
                        : onCourse ? Navigate( ref solo, out _, out aim, out thrust ) : Resolve( ref solo, 1, out _ )
                    : slot;
                if ( Frontier && squad.Faction == 0 && solo.Detached && solo.Body == 0 && solo.Enemy == 0 )
                    solo = PlotTransfer( new( i, ship ), 0, false, _positions[ship] );
                if ( onCourse ) {
                    _velocities[ship] = (goal - _positions[ship]) / (float)StepSeconds;
                    _positions[ship] = goal;
                    _headings[ship] = TurnToward( _headings[ship], aim, orbitalTurn );
                    float aligned = MathF.Max( 0, Vector2.Dot( _headings[ship], aim ) );
                    _thrust[ship] = float.Lerp( _thrust[ship], thrust * aligned * aligned, 0.18f );
                    sum += goal;
                    continue;
                }
                var error = goal - _positions[ship];
                var avoid = Separation( ship );
                var desired = error * ApproachGain + avoid;
                float speedSquared = desired.LengthSquared( );
                if ( speedSquared > localSpeed * localSpeed ) desired *= localSpeed / MathF.Sqrt( speedSquared );
                if ( speedSquared > 0.25f ) {
                    var wish = Vector2.Normalize( desired );
                    _headings[ship] = TurnToward( _headings[ship], wish, Frontier ? TurnStep * flight.TurnRate : TurnStep );
                    desired *= 0.22f + 0.78f * MathF.Max( Vector2.Dot( _headings[ship], wish ), 0 );
                }
                var velocity = Vector2.Lerp( _velocities[ship], desired, localResponse );
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
        if ( !Frontier ) AdvanceSite( cyan, amber );
        AdvancePlanets( );
        AdvanceProduction( );
        AdvanceIndustry( );
        AdvanceShipService( );
        ++Tick;
        AdvanceFrontier( );
        if ( _expeditionArrived ) AdvanceReceiver( );
    }

    private static Vector2 TurnToward( Vector2 facing, Vector2 goal, float limit ) {
        float cross = facing.X * goal.Y - facing.Y * goal.X;
        float dot = Vector2.Dot( facing, goal );
        float angle = dot < -0.999f && MathF.Abs( cross ) < 0.002f ? MathF.PI : MathF.Atan2( cross, dot );
        var (sin, cos) = MathF.SinCos( Math.Clamp( angle, -limit, limit ) );
        return Vector2.Normalize( new Vector2( facing.X * cos - facing.Y * sin, facing.X * sin + facing.Y * cos ) );
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
        (Math.Clamp( (int)((position.X + NavigationRadius * 1.4f) / AvoidRadius), 1, _gridSide - 2 ),
         Math.Clamp( (int)((position.Y + NavigationRadius * 1.4f) / AvoidRadius), 1, _gridSide - 2 ));

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

    internal int FillInstances( Span<ShipInstance> destination, FleetSelection selected, ReadOnlySpan<bool> selection = default,
        bool groupHulls = false, float interpolation = 1, bool groupMiners = false, bool groupFrigates = false ) {
        if ( destination.Length != ActiveCount ) throw new ArgumentException( "Instance span must match the active ship count.", nameof(destination) );
        int corvettes = ActiveCount, miners = 0, frigates = 0;
        if ( groupHulls ) {
            for ( int i = 0; i < SquadCount; ++i ) {
                if ( IsScoutSquad( i ) ) corvettes -= ShipCount( i );
                if ( groupFrigates && Role( i ) == HullRole.Frigate ) frigates += ShipCount( i );
                if ( groupMiners && UsesHaulerModel( Role( i ) ) ) miners += ShipCount( i );
            }
            corvettes -= miners + frigates;
        }
        int index = 0, frigateIndex = corvettes, scoutIndex = corvettes + frigates, minerIndex = ActiveCount - miners;
        for ( int i = 0; i < SquadCount; ++i ) {
            bool frigate = groupHulls && groupFrigates && Role( i ) == HullRole.Frigate;
            bool scout = groupHulls && IsScoutSquad( i );
            bool miner = groupMiners && UsesHaulerModel( Role( i ) );
            ref readonly var squad = ref _squads[i];
            var tint = Factions.ShipTint( (FactionId)squad.Faction );
            int end = squad.First + squad.Count;
            for ( int ship = squad.First; ship < end; ++ship ) {
                if ( !Alive( ship ) ) continue;
                bool highlighted = selection.IsEmpty ? ship == selected.Ship || !selected.IsShip && i == selected.Squad : selection[ship];
                var color = squad.Role == HullRole.Colony ? new Vector4( 0.58f, 1.9f, 1.31f, 1 ) : tint;
                if ( highlighted ) color.W = 1.66f;
                destination[frigate ? frigateIndex++ : scout ? scoutIndex++ : miner ? minerIndex++ : index++] = new( VisualPosition( ship, interpolation ),
                    interpolation == 1 ? _headings[ship] : Vector2.Normalize( Vector2.Lerp( _previousHeadings[ship], _headings[ship], interpolation ) ), color,
                    new Vector4( _thrust[ship], (ship % 173) * 0.618f, scout ? 0.75f : miner ? 0.56f : squad.Role == HullRole.Frigate && !groupFrigates ? 1.55f : 1, (float)_hull[ship] / MaxHull( squad.Role ) + (highlighted ? 2 : 0) ) );
            }
        }
        return corvettes;
    }

    internal ulong StateHash( ) {
        ulong hash = 14695981039346656037;
        if ( Frontier ) {
            Append( (uint)Charter );
            Append( _expeditionArrived ? 1u : 0u ); Append( _expeditionDeparted ? 1u : 0u );
            Append( (uint)_nextPatrol ); Append( PaidPatrol ? 1u : 0u );
            Append( (uint)_nextFleetId ); Append( (uint)_raidTick ); Append( (uint)_raidsLaunched ); Append( (uint)_deposits ); Append( _secured ? 1u : 0u );
            Append( DriveUpgrade ? 1u : 0u ); Append( HabitationUpgrade ? 1u : 0u ); Append( AstrometryUpgrade ? 1u : 0u );
            AppendDesign( Design ); AppendDesign( FrigateDesign );
            Append( EscortsUpgrade ? 1u : 0u ); Append( FusionUpgrade ? 1u : 0u );
            Append( (uint)PiracyCycles ); Append( (uint)PirateOreLost ); Append( (uint)PirateEnergyLost );
            Append( (uint)MerchantAgreement ); Append( (uint)MerchantShipments );
            Append( (uint)Energy ); Append( (uint)ShipsLaunched ); Append( (uint)CorvettesLaunched ); Append( (uint)_researchWork ); Append( ActiveResearch is { } topic ? (uint)topic + 1 : 0 );
            for ( int body = 1; body < _bodies.Length; ++body ) {
                Append( (uint)_yardShipReady[body] ); Append( (uint)_yardQueueSizes[body] );
                foreach ( var order in YardQueue( body ) ) {
                    Append( (uint)order.Hull ); AppendDesign( order.Design );
                    Append( (uint)order.Cost.Ore ); Append( (uint)order.Cost.Energy ); Append( (uint)order.Duration );
                }
                foreach ( var facility in Facilities ) {
                    var site = Construction( body, facility );
                    Append( (uint)(site.Worker?.Squad ?? -1) ); Append( (uint)(site.Worker?.Ship ?? -1) );
                    AppendVector( site.Orbit ); Append( site.Working ? 1u : 0u );
                }
            }
        }
        Append( (uint)SystemIndex ); Append( (uint)Tick ); Append( (uint)Count ); Append( (uint)SquadCount ); Append( _seed );
        Append( (uint)Owner ); Append( (uint)Capturing ); Append( (uint)CaptureTicks ); Append( Contested ? 1u : 0u );
        Append( (uint)ShotsFired ); Append( (uint)ShipsLost ); Append( (uint)Science ); Append( WeaponUpgrade ? 1u : 0u ); Append( (uint)YardBody ); Append( (uint)YardRemaining ); Append( IndustryUpgrade ? 1u : 0u );
        Append( (uint)_wingFinishTick ); Append( (uint)QueuedWings );
        for ( int i = 0; i < QueuedWings; ++i ) Append( (uint)_wingQueue[i].Duration );
        for ( int body = 0; body < _bodies.Length; ++body ) {
            Append( (uint)(_surveyUnits[body]?.Squad ?? -1) ); Append( (uint)(_surveyUnits[body]?.Ship ?? -1) );
            Append( (uint)_survey[body] ); Append( (uint)_mines[body] ); Append( (uint)_labs[body] );
            if ( Frontier ) {
                Append( (uint)_population[body] ); Append( (uint)_nextGrowth[body] ); Append( (uint)_colonyWork[body] );
                Append( (uint)(_colonyShips[body]?.Squad ?? -1) ); Append( (uint)(_colonyShips[body]?.Ship ?? -1) );
                foreach ( var facility in Facilities ) Append( (uint)SiteDeadlines( facility )[body] );
            }
        }
        Append( (uint)Ore ); Append( (uint)WingsBuilt ); Append( (uint)_incomeTicks ); Append( _rivalPatrol ? 1u : 0u ); Append( _battle ? 1u : 0u );
        for ( int i = 0; i < SquadCount; ++i ) {
            ref readonly var squad = ref _squads[i];
            Append( (uint)squad.Count ); Append( (uint)squad.Faction );
            if ( Frontier ) { Append( (uint)squad.FleetId ); Append( (uint)squad.Role ); Append( (uint)squad.Stance ); AppendDesign( squad.Design ); }
            AppendVector( squad.Home ); AppendVector( squad.Center ); AppendVector( squad.Direction ); AppendOrder( squad.Order );
        }
        for ( int i = 0; i < Count; ++i ) {
            AppendVector( _offsets[i] ); AppendVector( _positions[i] );
            AppendVector( _velocities[i] ); AppendVector( _headings[i] ); AppendOrder( _shipOrders[i] );
            Append( _hull[i] ); Append( (uint)_nextShot[i] );
            if ( Frontier ) {
                Append( _shield[i] ); Append( _armor[i] ); Append( (uint)_lastHit[i] );
                if ( _shipOrders[i].Service == ServiceOrder.Refit ) AppendDesign( _refitDesigns[i] );
            }
        }
        return hash;

        void AppendDesign( ShipDesign design ) {
            Append( (uint)design.Weapon ); Append( design.Plating ? 1u : 0u ); Append( (uint)design.Shielding );
            Append( (uint)design.Computer ); Append( (uint)design.Reactor ); Append( (uint)design.Drive );
        }

        void AppendOrder( Order order ) {
            AppendVector( order.Target ); Append( BitConverter.SingleToUInt32Bits( order.Phase ) );
            if ( Frontier ) {
                var leg = order.Transfer;
                Append( BitConverter.SingleToUInt32Bits( order.Radius ) ); Append( order.Waypoint ? 1u : 0u ); Append( order.Rejoining ? 1u : 0u );
                Append( order.Construction ); Append( order.Settlement ? 1u : 0u ); Append( (uint)order.Service ); Append( (uint)order.ServiceTicks );
                Append( (uint)leg.Start ); Append( (uint)leg.End ); AppendVector( leg.From ); AppendVector( leg.To );
                AppendVector( leg.Departure ); AppendVector( leg.Arrival ); AppendVector( leg.Bend );
            }
            Append( order.Body ); Append( order.Detached ? 1u : 0u ); Append( order.Survey ? 1u : 0u ); Append( (uint)order.Enemy );
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
            AddSquad( ships / squads + (i < ships % squads ? 1 : 0), faction, home, faction == 0 && i is 0 or 3 ? HullRole.Scout : HullRole.Corvette );
        }
    }

    internal static int MaxHull( HullRole role ) => role == HullRole.Frigate ? 124 : HullCapacity;
    internal int ShipGeneration( int ship ) => _shipGeneration[ship];

    private int AddSquad( int count, int faction, Vector2 center, HullRole role = HullRole.Corvette, ShipDesign design = default ) {
        int squadIndex = SquadCount, first = Count;
        if ( Frontier ) {
            for ( int i = 0; i < SquadCount; ++i ) {
                if ( _squads[i].Alive != 0 || _squads[i].Count != count || _squads[i].Faction != faction ) continue;
                squadIndex = i;
                first = _squads[i].First;
                ClearRetiredOrders( i, first, count );
                break;
            }
        }
        var heading = faction == 0 ? Vector2.UnitX : -Vector2.UnitX;
        int columns = (int)MathF.Ceiling( MathF.Sqrt( count * 1.6f ) );
        var mean = Vector2.Zero;
        for ( int i = 0; i < count; ++i ) {
            int row = count <= 10 ? (int)((MathF.Sqrt( 8 * i + 1 ) - 1) * 0.5f) : i / columns;
            float column = count <= 10 ? i - row * (row + 2) * 0.5f : i % columns + (row & 1) * 0.5f;
            var jitter = (new Vector2( NextUnit( ), NextUnit( ) ) - new Vector2( 0.5f )) * 0.28f;
            var local = new Vector2( column, row * 0.91f ) * Pitch + jitter;
            _offsets[first + i] = local;
            mean += _offsets[first + i];
        }
        mean /= count;
        for ( int i = first; i < first + count; ++i ) {
            _offsets[i] -= mean;
            _previousPositions[i] = _positions[i] = center + RotateSlot( _offsets[i], heading );
            _squadOfShip[i] = squadIndex;
            ++_shipGeneration[i];
            if ( Frontier ) _shipNames[i] = null;
            _shipOrders[i] = default;
            _velocities[i] = default;
            _thrust[i] = 0;
            _incoming[i] = 0;
            _railIncoming[i] = 0;
            _nextShot[i] = Frontier ? Tick : 0;
            _lastHit[i] = Frontier ? Tick : 0;
            _hull[i] = (byte)MaxHull( role );
            _shield[i] = Frontier && IsWarship( role ) ? (byte)design.Shield : (byte)0;
            _armor[i] = Frontier && IsWarship( role ) ? (byte)design.Armor : (byte)0;
            _previousHeadings[i] = _headings[i] = heading;
        }
        _squads[squadIndex] = new( ) { First = first, Count = count, Alive = count, Faction = faction, Home = center, Center = center,
            FleetId = ++_nextFleetId, Role = role, Design = design, Anchor = center, Direction = heading, Order = new Order { Target = center } };
        if ( squadIndex == SquadCount ) {
            ++SquadCount;
            Count += count;
        }
        _factionCount[faction] += count;
        return squadIndex;
    }

    private float NextUnit( ) {
        _seed = unchecked(_seed + 0x9E3779B9u);
        uint bits = _seed;
        bits = unchecked((bits ^ (bits >> 16)) * 0x85EBCA6Bu);
        bits = unchecked((bits ^ (bits >> 13)) * 0xC2B2AE35u);
        return ((bits ^ (bits >> 16)) >> 8) * (1f / 16_777_216);
    }
}
