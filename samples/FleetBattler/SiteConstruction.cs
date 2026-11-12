using System.Numerics;

namespace FleetBattler;

internal readonly record struct FacilityPlan( string Name, ResourceCost Cost, int Duration );
internal record struct ConstructionSite( FleetSelection? Worker, Vector2 Orbit, bool Working );

internal sealed partial class FleetSimulation {
    internal const int FacilityCount = 5;
    internal static ReadOnlySpan<PlanetFacility> Facilities => [
        PlanetFacility.Mine, PlanetFacility.Lab, PlanetFacility.Yard, PlanetFacility.Depot, PlanetFacility.Solar
    ];
    private readonly int[] _solarArrays = new int[CelestialSystem.BodyCount];
    private readonly ConstructionSite[] _siteConstruction = new ConstructionSite[CelestialSystem.BodyCount * FacilityCount];
    internal SiteOrder LastStartedSite { get; private set; }

    internal static FacilityPlan Facility( PlanetFacility facility ) => facility switch {
        PlanetFacility.Mine => new( "EXTRACTION RIG", new( MineCost, 8 ), FacilityDuration ),
        PlanetFacility.Lab => new( "RESEARCH LAB", new( LabCost, 10 ), FacilityDuration ),
        PlanetFacility.Yard => new( "ORBITAL SHIPYARD", new( YardCost, 15 ), YardDuration ),
        PlanetFacility.Depot => new( "LOGISTICS DEPOT", new( DepotCost, 9 ), FacilityDuration ),
        PlanetFacility.Solar => new( "SOLAR ARRAY", new( 28, 4 ), FacilityDuration ),
        _ => throw new ArgumentOutOfRangeException( nameof(facility) )
    };

    private ref ConstructionSite Construction( int body, PlanetFacility facility ) =>
        ref _siteConstruction[body * FacilityCount + (int)facility];

    internal string? ConstructionBlock( int body, PlanetFacility facility, FleetSelection? unit ) {
        if ( body <= 0 || body >= _bodies.Length || !Planet( body ).Surveyed ) return "SURVEY THIS BODY FIRST";
        if ( SiteRemaining( body, facility ) >= 0 ) return "SITE ALREADY ASSIGNED";
        if ( facility == PlanetFacility.Mine && !HasDeposit( body ) ) return "NO ORE DEPOSIT";
        if ( unit is not { } worker || !ValidConstructor( worker ) ) return "SELECT A CONSTRUCTOR";
        if ( !CanReach( worker, body ) ) return "OUTSIDE CONSTRUCTOR RANGE";
        foreach ( var assignment in _siteConstruction )
            if ( assignment.Worker?.Squad == worker.Squad ) return "CONSTRUCTOR ALREADY ASSIGNED";
        var cost = Facility( facility ).Cost;
        if ( Ore < cost.Ore ) return "INSUFFICIENT ORE";
        if ( Energy < cost.Energy ) return "INSUFFICIENT ENERGY";
        return null;
    }

    private bool ValidConstructor( FleetSelection worker ) => (uint)worker.Squad < (uint)SquadCount &&
        IsPlayerSquad( worker.Squad ) && Role( worker.Squad ) == HullRole.Miner &&
        (worker.IsShip ? Alive( worker.Ship ) && SquadronOf( worker.Ship ) == worker.Squad : ShipCount( worker.Squad ) > 0);

    private bool StartConstruction( int body, PlanetFacility facility, FleetSelection? unit ) {
        if ( ConstructionBlock( body, facility, unit ) is not null ) return false;
        var worker = unit!.Value;
        var orbit = OrbitalOrder( worker, body, survey: false );
        Construction( body, facility ) = new( worker, new( orbit.Radius, orbit.Phase + 0.24f ), false );
        SetOrder( worker, orbit );
        var plan = Facility( facility );
        Spend( plan.Cost );
        SiteDeadlines( facility )[body] = Tick + plan.Duration;
        return true;
    }

    internal FleetSelection? SiteWorker( int body, PlanetFacility facility ) => Construction( body, facility ).Worker;
    internal bool SiteEnRoute( int body, PlanetFacility facility ) =>
        SiteWorker( body, facility ) is { } worker && !InSurveyRange( worker, body );

    private void AdvanceConstruction( int body, PlanetFacility facility ) {
        ref var site = ref Construction( body, facility );
        if ( site.Worker is not { } worker ) return;
        if ( !ValidConstructor( worker ) || EffectiveOrder( worker ).Body != body + 1 ) {
            CancelSite( body, facility );
            return;
        }
        var deadlines = SiteDeadlines( facility );
        if ( !InSurveyRange( worker, body ) ) {
            ++deadlines[body];
            return;
        }
        if ( !site.Working ) {
            site.Working = true;
            LastStartedSite = new( body, facility );
            ++StartedConstruction;
        }
        if ( deadlines[body] <= Tick + 1 ) site.Worker = null;
    }

    internal Vector2 SitePosition( int body, PlanetFacility facility, float interpolation ) =>
        SitePositionAt( body, facility, Math.Max( 0, Tick - 1 + interpolation ), VisualBody( body, interpolation ).Center );

    private Vector2 SitePositionAt( int body, PlanetFacility facility, float tick, Vector2 center ) {
        var orbit = Construction( body, facility ).Orbit;
        if ( orbit.X > 0 ) {
            var (sin, cos) = MathF.SinCos( orbit.Y + tick * OrbitRate( orbit.X ) );
            return center + new Vector2( cos, sin ) * orbit.X;
        }
        float phase = facility switch {
            PlanetFacility.Lab => MathF.PI, PlanetFacility.Depot => MathF.PI / 2,
            PlanetFacility.Solar => MathF.PI / 4, _ => -MathF.PI / 2
        };
        var (y, x) = MathF.SinCos( tick * (float)StepSeconds * 0.23f + phase );
        return center + new Vector2( x, y ) * (OrbitRadius( body, 1 ) + 4);
    }
}
