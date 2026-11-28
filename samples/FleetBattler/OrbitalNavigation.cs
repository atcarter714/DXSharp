using System.Numerics;

namespace FleetBattler;

internal enum NavigationPhase { Parked, Burn, Coast, Brake, Intercept, Correction, Turnover, GravityAssist }

internal sealed partial class FleetSimulation {
    private readonly record struct Transfer( int Start, int End, Vector2 From, Vector2 To, Vector2 Departure,
        Vector2 Arrival, Vector2 Bend, FlybyRoute? Flyby = null ) {
        private const float BurnFraction = 0.30f, CorrectionEnd = 0.56f;
        private const float CorrectionWindow = CorrectionEnd - BurnFraction;
        private Vector2 Cruise => (To - From - (Departure + Arrival) * (BurnFraction * 0.5f)) / (1 - BurnFraction);

        internal float PeakAcceleration( ) {
            float time = (End - Start) * (float)StepSeconds;
            var cruise = Cruise;
            float main = 1.5f * MathF.Max( (cruise - Departure).Length( ), (cruise - Arrival).Length( ) ) / BurnFraction;
            float correction = Bend.Length( ) * 24 / (CorrectionWindow * CorrectionWindow);
            return MathF.Max( main, correction ) / (time * time);
        }

        internal float PoweredDeltaV() {
            float time = (End - Start) * (float)StepSeconds;
            var cruise = Cruise;
            // Four changes in lateral velocity, each bounded by the peak speed of the correction envelope.
            float steering = 14 * Bend.Length() / CorrectionWindow;
            return ((cruise - Departure).Length() + (Arrival - cruise).Length() + steering) / time;
        }

        // Integrate a smoothstep change in velocity. Both thrust and its contribution to velocity are continuous at cutoff.
        private static float BurnTravel( float progress ) => progress * progress * progress * (1 - 0.5f * progress);
        private static float BurnThrottle( float progress ) => 4 * progress * (1 - progress);

        internal Vector2 Position( float t ) {
            if ( Flyby is { } route ) return route.Position( Start + t * (End - Start) );
            float b = BurnFraction;
            Vector2 cruise = Cruise;
            if ( t <= b ) return From + Departure * t + (cruise - Departure) * (b * BurnTravel( t / b ));
            if ( t >= 1 - b ) {
                float timeLeft = 1 - t;
                return To - Arrival * timeLeft - (cruise - Arrival) * (b * BurnTravel( timeLeft / b ));
            }
            float progress = Math.Min( 1, (t - b) / CorrectionWindow );
            float envelope = progress * (1 - progress);
            float shape = 64 * envelope * envelope * envelope;
            return From + (Departure + cruise) * (b * 0.5f) + cruise * (t - b) + Bend * shape;
        }

        internal Vector2 Aim( float t, out float throttle ) {
            if ( Flyby is { } route ) return route.Aim( Start + t * (End - Start), out throttle );
            var cruise = Cruise;
            float b = BurnFraction;
            Vector2 departure = cruise - Departure;
            Vector2 brake = Arrival - cruise;
            Vector2 aim;
            if ( t < b ) {
                aim = departure;
                throttle = BurnThrottle( t / b );
            } else if ( t >= 1 - b ) {
                aim = brake;
                throttle = BurnThrottle( (1 - t) / b );
            } else {
                switch ( Phase( t ) ) {
                    case NavigationPhase.Correction:
                        float progress = (t - b) / CorrectionWindow;
                        float envelope = progress * (1 - progress);
                        float correction = 384 * envelope * (1 - 5 * envelope);
                        aim = Bend * correction;
                        throttle = MathF.Abs( correction ) / 24 * 0.36f;
                        break;
                    case NavigationPhase.Coast: aim = cruise; throttle = 0; break;
                    default: aim = brake; throttle = 0; break;
                }
            }
            return aim.LengthSquared( ) > 1e-7f ? Vector2.Normalize( aim ) : Vector2.Zero;
        }

        internal NavigationPhase Phase( float t ) {
            if ( Flyby is { } route ) return route.Phase( Start + t * (End - Start) );
            if ( t < BurnFraction ) return NavigationPhase.Burn;
            if ( t >= 1 - BurnFraction ) return NavigationPhase.Brake;
            if ( t >= CorrectionEnd ) return NavigationPhase.Turnover;
            return Bend != Vector2.Zero ? NavigationPhase.Correction : NavigationPhase.Coast;
        }
    }

    internal float NavigationRadius => BoardRadius * (Solar ? 3.52f * SolarCatalog.Spacing : Frontier ? 2.28f : 1);
    internal bool CanReach( FleetSelection unit, int body ) => !RequiresDrive( body ) || DriveUpgrade || !IsPlayerSquad( unit.Squad ) ||
        Solar && body != CelestialSystem.OuterBelt && SolarCatalog.HeliocentricAxis( body ) <= 1.69f * SolarCatalog.Spacing * Flight( unit.Squad ).Range;

    internal bool RequiresDrive( int body ) => Solar && (body == CelestialSystem.OuterBelt || SolarCatalog.HeliocentricAxis( body ) > 1.69f * SolarCatalog.Spacing);

    internal bool CanReach( FleetSelection unit, Vector2 target ) =>
        !Frontier || !IsPlayerSquad( unit.Squad ) ||
        Vector2.DistanceSquared( target, _bodies[0].Center ) <= MathF.Pow( BoardRadius * (Solar ? (DriveUpgrade ? 3.38f : 1.69f) * SolarCatalog.Spacing : DriveUpgrade ? 2.04f : 1.26f) * Flight( unit.Squad ).Range, 2 );

    internal float RouteProgress( FleetSelection unit ) {
        var leg = EffectiveOrder( unit ).Transfer;
        return leg.End > Tick ? (float)(Tick - leg.Start) / (leg.End - leg.Start) : 1;
    }

    internal int RouteTicks( FleetSelection unit ) => Math.Max( 0, EffectiveOrder( unit ).Transfer.End - Tick );
    internal NavigationPhase RoutePhase( FleetSelection unit ) {
        var order = EffectiveOrder( unit );
        if ( order.Enemy != 0 ) return NavigationPhase.Intercept;
        var leg = order.Transfer;
        if ( leg.End == 0 ) return NavigationPhase.Parked;
        float progress = (float)(Tick - leg.Start) / (leg.End - leg.Start);
        return leg.Phase( progress );
    }

    internal Vector2? TrackedRouteTarget( FleetSelection unit, float interpolation ) {
        var order = EffectiveOrder( unit );
        if ( Alive( order.Enemy - 1 ) ) return VisualPosition( order.Enemy - 1, interpolation );
        return order.Body > 1 && order.Transfer.End > Tick
            ? order.Service is ServiceOrder.Repair or ServiceOrder.Refit
                ? SitePosition( order.Body - 1, PlanetFacility.Yard, interpolation )
                : VisualBody( order.Body - 1, interpolation ).Center
            : null;
    }

    internal bool CopyRoute( FleetSelection unit, Span<Vector2> points, out Vector2 destination, float interpolation = 1 ) {
        var order = EffectiveOrder( unit );
        var leg = order.Transfer;
        destination = leg.To;
        if ( points.Length < 2 ) return false;
        var origin = VisualCenter( unit, interpolation );
        int target = order.Enemy - 1;
        if ( Alive( target ) ) {
            float standOff = IsPlayerSquad( unit.Squad ) && WeaponUpgrade ? 12 : 10;
            destination = InterceptPoint( origin, VisualPosition( target, interpolation ), _velocities[target], Flight( unit.Squad ).Cruise, standOff );
            for ( int i = 0; i < points.Length; ++i )
                points[i] = Vector2.Lerp( origin, destination, (float)i / (points.Length - 1) );
            return true;
        }
        if ( leg.End <= Tick ) return false;
        float start = Math.Clamp( (Tick - 1 + interpolation - leg.Start) / (leg.End - leg.Start), 0, 1 );
        points[0] = origin;
        for ( int i = 1; i < points.Length; ++i )
            points[i] = leg.Position( float.Lerp( start, 1, (float)i / (points.Length - 1) ) );
        return true;
    }

    private void Park( int squadIndex, int host, int tick = -1 ) {
        if ( tick < 0 ) tick = Tick;
        ref var squad = ref _squads[squadIndex];
        var delta = squad.Center - _bodies[host].Center;
        squad.Order = new( ) { Body = (byte)(host + 1), Phase = MathF.Atan2( delta.Y, delta.X ), Radius = MathF.Max( delta.Length( ), OrbitRadius( host, squad.Alive ) ) };
        squad.Order.Phase -= tick * OrbitRate( host, squad.Order.Radius );
        Vector2 center = Berth( squad.Order, tick, out var tangent );
        squad.Center = squad.Anchor = center;
        squad.Direction = tangent;
        for ( int i = squad.First; i < squad.First + squad.Count; ++i ) {
            _positions[i] = _previousPositions[i] = center + RotateSlot( _offsets[i], tangent );
            _headings[i] = _previousHeadings[i] = tangent;
            _velocities[i] = (Berth( squad.Order, tick + 1, out _ ) - center) / (float)StepSeconds;
        }
    }

    private Order PlotTransfer( FleetSelection unit, int host, bool survey, Vector2? waypoint = null, Vector2? fixedBerth = null,
        int? departureTick = null, Vector2? departurePosition = null, Vector2? departureVelocity = null ) {
        int start = departureTick ?? Tick;
        var from = departurePosition ?? Center( unit );
        var previous = EffectiveOrder( unit );
        if ( departureTick is null && previous.Transfer.Flyby is null && waypoint is null && fixedBerth is null && previous.Body == host + 1 && !previous.Waypoint && (!unit.IsShip || previous.Detached) ) {
            previous.Survey = survey;
            return previous;
        }
        int count = unit.IsShip ? 1 : Math.Max( 1, ShipCount( unit.Squad ) );
        var local = (waypoint ?? from) - _bodies[host].Center;
        float radius = fixedBerth?.X ?? (waypoint is null ? OrbitRadius( host, count ) : MathF.Max( local.Length( ), OrbitRadius( 0, count ) ));
        float distance = waypoint is null ? MathF.Abs( Vector2.Distance( from, _bodies[host].Center ) - radius ) : Vector2.Distance( from, waypoint.Value );
        var flight = Flight( unit.Squad );
        // On the campaign calendar the cruise leg of an interplanetary transfer takes weeks to months.
        float cruise = distance / (Solar ? flight.Cruise * 0.78f : flight.Cruise) * (FrontierProgression ? Pacing.Table.InSystemTransfer : 1);
        float traverse = (Solar ? 3.4f : 2.1f) + cruise;
        float turn = MathF.PI / (OrbitalTurnStep * flight.TurnRate) * (float)StepSeconds;
        int arrival = start + (int)MathF.Ceiling( MathF.Max( traverse, turn * 5 ) / (float)StepSeconds );
        var velocity = departureVelocity ?? (unit.IsShip ? _velocities[unit.Ship] : SquadVelocity( unit.Squad ));
        float maxAcceleration = flight.Acceleration;
        var order = new Order {
            Body = (byte)(host + 1), Radius = radius, Survey = survey, Detached = unit.IsShip, Waypoint = waypoint is not null
        };
        for ( int attempt = 0; ; ++attempt ) {
            var endpoint = BodyCenterAt( host, arrival );
            if ( waypoint is null ) local = from - endpoint;
            order.Phase = fixedBerth?.Y ?? (MathF.Atan2( local.Y, local.X ) - OrbitRate( host, radius ) * arrival);
            var to = Berth( order, arrival, out _ );
            float duration = (arrival - start) * (float)StepSeconds;
            var arrivalVelocity = (Berth( order, arrival + 1, out _ ) - to) / (float)StepSeconds;
            var leg = new Transfer( start, arrival, from, to, velocity * duration, arrivalVelocity * duration,
                AvoidBody( from, to, start + (arrival - start) / 2 ) );
            float ratio = leg.PeakAcceleration( ) / maxAcceleration;
            if ( ratio <= 1.04f || attempt == 7 ) {
                order.Transfer = leg;
                order.Target = to;
                break;
            }
            arrival = start + (int)MathF.Ceiling( (arrival - start) * MathF.Sqrt( ratio ) * 1.04f );
        }
        return order;
    }

    private Vector2 SquadVelocity( int index ) {
        ref readonly var squad = ref _squads[index];
        Vector2 sum = default;
        for ( int ship = squad.First; ship < squad.First + squad.Count; ++ship )
            if ( Alive( ship ) ) sum += _velocities[ship];
        return sum / Math.Max( 1, squad.Alive );
    }

    private Vector2 AvoidBody( Vector2 from, Vector2 to, int middle ) {
        Vector2 chord = to - from;
        float length = chord.Length( );
        if ( length < 1 ) return default;
        Vector2 normal = new( -chord.Y / length, chord.X / length );
        Span<BodyPose> bodies = stackalloc BodyPose[_bodies.Length];
        EvaluateBodies( _ephemerisEpoch + middle, bodies );
        float bend = 0;
        for ( int i = 0; i < bodies.Length; ++i ) {
            var delta = bodies[i].Center - from;
            float along = Vector2.Dot( delta, chord ) / (length * length);
            if ( along is < 0.34f or > 0.66f ) continue;
            float side = Vector2.Dot( delta, normal );
            float clearance = bodies[i].Radius + 5 - MathF.Abs( side );
            if ( clearance <= 0 ) continue;
            float t = (along - 0.30f) / 0.40f;
            float weight = MathF.Max( 0.25f, 16 * t * t * (1 - t) * (1 - t) );
            float correction = clearance / weight;
            if ( correction > MathF.Abs( bend ) ) bend = side > 0 ? -correction : correction;
        }
        return normal * bend;
    }

    private Vector2 BodyCenterAt( int host, int tick ) {
        Span<BodyPose> bodies = stackalloc BodyPose[_bodies.Length];
        EvaluateBodies( _ephemerisEpoch + tick, bodies );
        return bodies[host].Center;
    }

    // Tactical GM sets an eight-unit/s circular speed at a single hull's standard berth.
    private float GravityParameter( int host ) => 64 * OrbitRadius( host, 1 );

    private Vector2 GravityAt( Vector2 position ) {
        var acceleration = Vector2.Zero;
        for ( int body = 0; body < _bodies.Length; ++body ) {
            var offset = _bodies[body].Center - position;
            float interior = MathF.Max( _bodies[body].Radius, 1 );
            float distanceSquared = MathF.Max( offset.LengthSquared(), interior * interior );
            acceleration += offset * (GravityParameter( body ) / (distanceSquared * MathF.Sqrt( distanceSquared )));
        }
        return acceleration;
    }

    private float OrbitRate( int host, float radius ) =>
        (float)StepSeconds * MathF.Sqrt( GravityParameter( host ) / (radius * radius * radius) );

    private Vector2 Berth( Order order, int tick, out Vector2 tangent, bool evaluated = false ) {
        var (sin, cos) = MathF.SinCos( order.Phase + tick * OrbitRate( order.Body - 1, order.Radius ) );
        tangent = new( -sin, cos );
        var host = order.Body - 1;
        return (evaluated ? _bodies[host].Center : BodyCenterAt( host, tick )) + new Vector2( cos, sin ) * order.Radius;
    }

    private Vector2 Navigate( ref Order order, out Vector2 tangent, out Vector2 aim, out float drive ) {
        int tick = Tick + 1;
        var leg = order.Transfer;
        drive = 0;
        if ( tick < leg.End ) {
            float duration = leg.End - leg.Start;
            float t = (tick - leg.Start) / duration;
            float dt = 1 / duration;
            var position = leg.Position( t );
            Vector2 velocity = leg.Position( t + dt ) - position;
            aim = leg.Aim( t, out drive );
            tangent = velocity.LengthSquared( ) > 1e-8f ? Vector2.Normalize( velocity ) : Vector2.Zero;
            return position;
        }
        order.Transfer = default;
        var berth = Berth( order, tick, out tangent, evaluated: true );
        aim = tangent;
        return berth;
    }
}
