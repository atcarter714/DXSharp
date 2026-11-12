using System.Numerics;

namespace FleetBattler;

internal enum NavigationPhase { Parked, Burn, Coast, Brake, Intercept, Correction, Turnover }

internal sealed partial class FleetSimulation {
    private const float TransferSpeed = 24;
    private readonly record struct Transfer( int Start, int End, Vector2 From, Vector2 To, Vector2 Departure,
        Vector2 Arrival, Vector2 Bend ) {
        private const float BurnFraction = 0.30f;
        private Vector2 Cruise => (To - From - (Departure + Arrival) * (BurnFraction * 0.5f)) / (1 - BurnFraction);

        internal float PeakAcceleration( ) {
            float time = (End - Start) * (float)StepSeconds;
            var cruise = Cruise;
            float main = MathF.Max( (cruise - Departure).Length( ), (cruise - Arrival).Length( ) ) / BurnFraction;
            float correction = Bend.Length( ) * 32 / MathF.Pow( 1 - 2 * BurnFraction, 2 );
            return MathF.Max( main, correction ) / (time * time);
        }

        internal Vector2 Position( float t ) {
            float b = BurnFraction;
            Vector2 cruise = Cruise;
            if ( t <= b ) return From + Departure * t + (cruise - Departure) * (t * t / (2 * b));
            if ( t >= 1 - b ) {
                float timeLeft = 1 - t;
                return To - Arrival * timeLeft - (cruise - Arrival) * (timeLeft * timeLeft / (2 * b));
            }
            float progress = (t - b) / (1 - 2 * b);
            float shape = 16 * progress * progress * (1 - progress) * (1 - progress);
            return From + (Departure + cruise) * (b * 0.5f) + cruise * (t - b) + Bend * shape;
        }

        internal Vector2 Aim( float t, out float throttle ) {
            var cruise = Cruise;
            float b = BurnFraction;
            Vector2 departure = cruise - Departure;
            Vector2 brake = Arrival - cruise;
            Vector2 aim;
            if ( t < b ) {
                aim = departure;
                throttle = 0.9f;
            } else if ( t >= 1 - b ) {
                aim = brake;
                throttle = 0.8f;
            } else {
                float progress = (t - b) / (1 - 2 * b);
                float correction = 32 * (1 - 6 * progress + 6 * progress * progress);
                switch ( Phase( t ) ) {
                    case NavigationPhase.Correction: aim = Bend * correction; throttle = 0.28f; break;
                    case NavigationPhase.Coast: aim = cruise; throttle = 0; break;
                    default: aim = brake; throttle = 0; break;
                }
            }
            return aim.LengthSquared( ) > 1e-7f ? Vector2.Normalize( aim ) : Vector2.Zero;
        }

        internal NavigationPhase Phase( float t ) {
            if ( t < BurnFraction ) return NavigationPhase.Burn;
            if ( t >= 1 - BurnFraction ) return NavigationPhase.Brake;
            float progress = (t - BurnFraction) / (1 - 2 * BurnFraction);
            if ( progress >= 0.38f ) return NavigationPhase.Turnover;
            return Bend != Vector2.Zero && MathF.Abs( 32 * (1 - 6 * progress + 6 * progress * progress) ) > 6
                ? NavigationPhase.Correction : NavigationPhase.Coast;
        }
    }

    internal float NavigationRadius => BoardRadius * (Solar ? 3.04f : Frontier ? 2.28f : 1);
    internal bool CanReach( FleetSelection unit, int body ) =>
        !RequiresDrive( body ) || DriveUpgrade || !IsPlayerSquad( unit.Squad );

    internal bool RequiresDrive( int body ) => body == CelestialSystem.OuterBelt || Solar && SolarCatalog.Get( body ).Axis > 1.69f;

    internal bool CanReach( FleetSelection unit, Vector2 target ) =>
        !Frontier || !IsPlayerSquad( unit.Squad ) ||
        Vector2.DistanceSquared( target, _bodies[0].Center ) <= MathF.Pow( BoardRadius * (Solar ? DriveUpgrade ? 2.94f : 1.69f : DriveUpgrade ? 2.04f : 1.26f), 2 );

    internal int RouteTicks( FleetSelection unit ) => Math.Max( 0, EffectiveOrder( unit ).Transfer.End - Tick );
    internal NavigationPhase RoutePhase( FleetSelection unit ) {
        var order = EffectiveOrder( unit );
        if ( order.Enemy != 0 ) return NavigationPhase.Intercept;
        var leg = order.Transfer;
        if ( leg.End == 0 ) return NavigationPhase.Parked;
        float progress = (float)(Tick - leg.Start) / (leg.End - leg.Start);
        return leg.Phase( progress );
    }

    internal bool CopyRoute( FleetSelection unit, Span<Vector2> points, out Vector2 destination, float interpolation = 1 ) {
        var leg = EffectiveOrder( unit ).Transfer;
        destination = leg.To;
        if ( leg.End <= Tick || points.Length < 2 ) return false;
        float start = Math.Clamp( (Tick - 1 + interpolation - leg.Start) / (leg.End - leg.Start), 0, 1 );
        var origin = VisualCenter( unit, interpolation );
        points[0] = origin;
        for ( int i = 1; i < points.Length; ++i ) {
            float sample = (float)i / (points.Length - 1);
            points[i] = sample <= start ? origin : leg.Position( sample );
        }
        return true;
    }

    private void Park( int squadIndex, int host ) {
        ref var squad = ref _squads[squadIndex];
        var delta = squad.Center - _bodies[host].Center;
        squad.Order = new( ) { Body = (byte)(host + 1), Phase = MathF.Atan2( delta.Y, delta.X ), Radius = MathF.Max( delta.Length( ), OrbitRadius( host, squad.Alive ) ) };
        squad.Order.Phase -= Tick * OrbitRate( squad.Order.Radius );
        Vector2 center = Berth( squad.Order, Tick, out var tangent );
        squad.Center = squad.Anchor = center;
        squad.Direction = tangent;
        for ( int i = squad.First; i < squad.First + squad.Count; ++i ) {
            _positions[i] = _previousPositions[i] = center + RotateSlot( _offsets[i], tangent );
            _headings[i] = _previousHeadings[i] = tangent;
            _velocities[i] = (Berth( squad.Order, Tick + 1, out _ ) - center) / (float)StepSeconds;
        }
    }

    private Order PlotTransfer( FleetSelection unit, int host, bool survey, Vector2? waypoint = null ) {
        var from = Center( unit );
        var previous = EffectiveOrder( unit );
        if ( waypoint is null && previous.Body == host + 1 && !previous.Waypoint && (!unit.IsShip || previous.Detached) ) {
            previous.Survey = survey;
            return previous;
        }
        int count = unit.IsShip ? 1 : Math.Max( 1, ShipCount( unit.Squad ) );
        var local = (waypoint ?? from) - _bodies[host].Center;
        float radius = waypoint is null ? OrbitRadius( host, count ) : MathF.Max( local.Length( ), OrbitRadius( 0, count ) );
        float distance = waypoint is null ? MathF.Abs( Vector2.Distance( from, _bodies[host].Center ) - radius ) : Vector2.Distance( from, waypoint.Value );
        int arrival = Tick + (int)MathF.Ceiling( (2.1f + distance / TransferSpeed) / (float)StepSeconds );
        var velocity = unit.IsShip ? _velocities[unit.Ship] : SquadVelocity( unit.Squad );
        float maxAcceleration = DriveUpgrade && IsPlayerSquad( unit.Squad ) ? 80 : 55;
        var order = new Order {
            Body = (byte)(host + 1), Radius = radius, Survey = survey, Detached = unit.IsShip, Waypoint = waypoint is not null
        };
        for ( int attempt = 0; ; ++attempt ) {
            var endpoint = BodyCenterAt( host, arrival );
            if ( waypoint is null ) local = from - endpoint;
            order.Phase = MathF.Atan2( local.Y, local.X ) - OrbitRate( radius ) * arrival;
            var to = Berth( order, arrival, out _ );
            float duration = (arrival - Tick) * (float)StepSeconds;
            var arrivalVelocity = (Berth( order, arrival + 1, out _ ) - to) / (float)StepSeconds;
            var leg = new Transfer( Tick, arrival, from, to, velocity * duration, arrivalVelocity * duration,
                AvoidBody( from, to, Tick + (arrival - Tick) / 2 ) );
            float ratio = leg.PeakAcceleration( ) / maxAcceleration;
            if ( ratio <= 1.04f || attempt == 7 ) {
                order.Transfer = leg;
                order.Target = to;
                break;
            }
            arrival = Tick + (int)MathF.Ceiling( (arrival - Tick) * MathF.Sqrt( ratio ) * 1.04f );
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
        CelestialSystem.Evaluate( SystemIndex, middle, BoardRadius, bodies );
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
        CelestialSystem.Evaluate( SystemIndex, tick, BoardRadius, bodies );
        return bodies[host].Center;
    }

    private static float OrbitRate( float radius ) => 8 * (float)StepSeconds / radius;

    private Vector2 Berth( Order order, int tick, out Vector2 tangent, bool evaluated = false ) {
        var (sin, cos) = MathF.SinCos( order.Phase + tick * OrbitRate( order.Radius ) );
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
