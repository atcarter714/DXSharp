using System.Numerics;

namespace FleetBattler;

// Positive semi-major-axis magnitude. Time zero is periapsis. The state is expressed
// in the moving body's orbital plane in tactical length units and simulation seconds.
internal readonly record struct HyperbolicPass( double Mu, double Periapsis, double ExcessSpeed ) {
    internal (Vector2 Position, Vector2 Velocity) State( double seconds ) {
        double a = Mu / (ExcessSpeed * ExcessSpeed);
        double e = 1 + Periapsis / a;
        double mean = seconds * ExcessSpeed / a;
        double h = Math.Asinh( mean / e );
        for ( int i = 0; i < 12; ++i ) {
            double correction = (e * Math.Sinh( h ) - h - mean) / (e * Math.Cosh( h ) - 1);
            h -= correction;
            if ( Math.Abs( correction ) < 1e-12 ) break;
        }
        double ch = Math.Cosh( h ), sh = Math.Sinh( h ), minor = Math.Sqrt( e * e - 1 );
        double speed = ExcessSpeed / (e * ch - 1);
        return (new( (float)(a * (e - ch)), (float)(a * minor * sh) ),
            new( (float)(-speed * sh), (float)(speed * minor * ch) ));
    }
}

internal sealed partial class FleetSimulation {
    private sealed record Encounter( FleetSimulation Fleet, int Body, int Middle, HyperbolicPass Orbit,
        Vector2 Radial, Vector2 Tangent ) {
        internal (Vector2 Position, Vector2 Velocity) State( float tick ) {
            int whole = (int)MathF.Floor( tick );
            var center = Fleet.BodyCenterAt( Body, whole );
            var next = Fleet.BodyCenterAt( Body, whole + 1 );
            var (p, v) = Orbit.State( (tick - Middle) * StepSeconds );
            return (Mix( center, next, tick - whole ) + Radial * p.X + Tangent * p.Y,
                (next - center) / (float)StepSeconds + Radial * v.X + Tangent * v.Y);
        }
    }

    private sealed record FlybyRoute( Transfer Approach, Encounter Pass, Transfer Capture ) {
        private Transfer PoweredLeg( float tick ) => tick < Approach.End ? Approach : Capture;
        private bool Ballistic( float tick ) => tick >= Approach.End && tick < Capture.Start;
        private static float Fraction( Transfer leg, float tick ) =>
            Math.Clamp( (tick - leg.Start) / (leg.End - leg.Start), 0, 1 );

        internal Vector2 Position( float tick ) {
            if ( Ballistic( tick ) ) return Pass.State( tick ).Position;
            var leg = PoweredLeg( tick );
            return leg.Position( Fraction( leg, tick ) );
        }

        internal NavigationPhase Phase( float tick ) {
            if ( Ballistic( tick ) ) return NavigationPhase.GravityAssist;
            var leg = PoweredLeg( tick );
            return leg.Phase( Fraction( leg, tick ) );
        }

        internal Vector2 Aim( float tick, out float throttle ) {
            if ( Ballistic( tick ) ) {
                throttle = 0;
                return Vector2.Normalize( Pass.State( tick ).Velocity );
            }
            var leg = PoweredLeg( tick );
            return leg.Aim( Fraction( leg, tick ), out throttle );
        }
    }

    internal int FlybyDestination( int body ) {
        if ( !Solar ) return body == Homeworld ? 3 : Homeworld;
        int parent = SolarCatalog.Get( body ).Parent;
        if ( parent > 0 && body != 3 ) return parent;
        return body switch {
            Homeworld => 3, 3 => 7, 2 => SolarCatalog.Europa, 8 => SolarCatalog.Titan,
            10 => SolarCatalog.Triton, SolarCatalog.Pluto => SolarCatalog.Charon, _ => Homeworld
        };
    }

    internal bool FlyPast( FleetSelection unit, int body ) {
        if ( !Frontier || body <= 0 || body >= _bodies.Length || !CanReach( unit, body ) ) return false;
        int target = FlybyDestination( body );
        if ( !CanReach( unit, target ) ) return false;
        var from = Center( unit );
        var velocity = unit.IsShip ? _velocities[unit.Ship] : SquadVelocity( unit.Squad );
        var flight = Flight( unit.Squad );
        float periapsis = OrbitRadius( body, unit.IsShip ? 1 : ShipCount( unit.Squad ) );
        var pass = new HyperbolicPass( GravityParameter( body ), periapsis, Math.Clamp( flight.Cruise * .31f, 5, 11 ) );
        int halfPass = (int)Math.Ceiling( 2 * periapsis / pass.ExcessSpeed / StepSeconds );
        float distance = Vector2.Distance( from, _bodies[body].Center );
        int initialApproach = (int)Math.Ceiling( (9 + distance / flight.Cruise) / StepSeconds );
        Order best = default;
        float bestCost = float.PositiveInfinity;
        // Test both sides of the moving body. Match velocity at both boundaries of the unpowered arc.
        for ( int side = -1; side <= 1; side += 2 ) {
            int approachTicks = initialApproach;
            for ( int attempt = 0; attempt < 10; ++attempt ) {
                int entry = Tick + approachTicks, middle = entry + halfPass, exit = middle + halfPass;
                var center = BodyCenterAt( body, middle );
                var direction = center - from;
                var tangent = direction.LengthSquared() > 1e-6f ? Vector2.Normalize( direction ) : Vector2.UnitX;
                var radial = new Vector2( -tangent.Y, tangent.X ) * side;
                var encounter = new Encounter( this, body, middle, pass, radial, tangent );
                var incoming = encounter.State( entry );
                float seconds = approachTicks * (float)StepSeconds;
                var approach = new Transfer( Tick, entry, from, incoming.Position, velocity * seconds,
                    incoming.Velocity * seconds, AvoidBody( from, incoming.Position, Tick + approachTicks / 2 ) );
                float ratio = approach.PeakAcceleration() / flight.Acceleration;
                if ( ratio > 1.04f ) {
                    approachTicks = (int)MathF.Ceiling( approachTicks * MathF.Sqrt( ratio ) * 1.04f );
                    continue;
                }
                var outgoing = encounter.State( exit );
                var order = PlotTransfer( unit, target, false, departureTick: exit,
                    departurePosition: outgoing.Position, departureVelocity: outgoing.Velocity );
                var capture = order.Transfer;
                float deltaV = approach.PoweredDeltaV() + capture.PoweredDeltaV();
                if ( capture.PeakAcceleration() <= flight.Acceleration * 1.04f &&
                     (deltaV < bestCost || deltaV == bestCost && capture.End < best.Transfer.End) ) {
                    bestCost = deltaV;
                    best = order;
                    best.Transfer = new( Tick, capture.End, from, capture.To, default, default, default,
                        new FlybyRoute( approach, encounter, capture ) );
                }
                break;
            }
        }
        if ( best.Transfer.Flyby is null ) return false;
        SetOrder( unit, best );
        return true;
    }
}
