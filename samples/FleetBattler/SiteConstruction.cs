using System.Numerics;

namespace FleetBattler;

internal enum SiteLayer { Surface, Orbit }
internal readonly record struct FacilityPlan( string Name, ResourceCost Cost, int Duration, SiteLayer Layer = SiteLayer.Orbit );
internal readonly record struct SiteSlots( int Surface, int Orbit ) {
    internal int For( SiteLayer layer ) => layer == SiteLayer.Surface ? Surface : Orbit;
}
internal record struct ConstructionSite( FleetSelection? Worker, Vector2 Orbit, bool Working );

internal sealed partial class FleetSimulation {
    internal const int FacilityCount = 8;
    internal static ReadOnlySpan<PlanetFacility> Facilities => [
        PlanetFacility.Mine, PlanetFacility.Observatory, PlanetFacility.Skimmer, PlanetFacility.Lab,
        PlanetFacility.Yard, PlanetFacility.Depot, PlanetFacility.Solar, PlanetFacility.Habitat
    ];
    private readonly int[] _solarArrays = new int[CelestialSystem.BodyCount];
    private readonly int[] _habitats = new int[CelestialSystem.BodyCount], _observatories = new int[CelestialSystem.BodyCount],
        _skimmers = new int[CelestialSystem.BodyCount];
    private readonly ConstructionSite[] _siteConstruction = new ConstructionSite[CelestialSystem.BodyCount * FacilityCount];
    internal SiteOrder LastStartedSite { get; private set; }

    internal static FacilityPlan Facility( PlanetFacility facility ) => facility switch {
        PlanetFacility.Mine => new( "EXTRACTION RIG", new( MineCost, 8 ), FacilityDuration, SiteLayer.Surface ),
        PlanetFacility.Lab => new( "RESEARCH LAB", new( LabCost, 10 ), FacilityDuration ),
        PlanetFacility.Yard => new( "ORBITAL SHIPYARD", new( YardCost, 15 ), YardDuration ),
        PlanetFacility.Depot => new( "LOGISTICS DEPOT", new( DepotCost, 9 ), FacilityDuration ),
        PlanetFacility.Solar => new( "SOLAR ARRAY", new( 28, 4 ), FacilityDuration ),
        PlanetFacility.Habitat => new( "O NEILL HABITAT", new( 100, 32 ), 660 ),
        PlanetFacility.Observatory => new( "SURFACE OBSERVATORY", new( 56, 12 ), 300, SiteLayer.Surface ),
        PlanetFacility.Skimmer => new( "ORBITAL EXTRACTION", new( 35, 8 ), FacilityDuration ),
        _ => throw new ArgumentOutOfRangeException( nameof(facility) )
    };

    private ref ConstructionSite Construction( int body, PlanetFacility facility ) =>
        ref _siteConstruction[body * FacilityCount + (int)facility];

    internal SiteSlots Slots( int body ) {
        if ( body <= 0 || body >= _bodies.Length ) return default;
        float radius = _bodies[body].Radius / BoardRadius;
        bool solid = body != CelestialSystem.OuterBelt && body != 2 && (!Solar || body < 8);
        int surface = !solid ? 0 : radius >= 0.045f ? 2 : 1;
        int orbit = radius >= 0.060f ? 5 : radius >= 0.030f ? 4 : 3;
        return new( surface, orbit );
    }

    internal bool FitsBody( int body, PlanetFacility facility ) {
        var slots = Slots( body );
        if ( facility == PlanetFacility.Skimmer ) return slots is { Surface: 0, Orbit: > 0 };
        return slots.For( Facility( facility ).Layer ) > 0;
    }

    internal int OccupiedSlots( int body, SiteLayer layer ) {
        int used = 0;
        foreach ( var facility in Facilities )
            if ( Facility( facility ).Layer == layer && SiteRemaining( body, facility ) >= 0 ) ++used;
        return used;
    }

    internal int ExtractionRemaining( int body ) => Math.Max( Remaining( _mines[body] ), Remaining( _skimmers[body] ) );

    internal string? ConstructionBlock( int body, PlanetFacility facility, FleetSelection? unit ) {
        if ( body <= 0 || body >= _bodies.Length || !Planet( body ).Surveyed ) return "SURVEY THIS BODY FIRST";
        if ( !FitsBody( body, facility ) ) return "INCOMPATIBLE BODY TYPE";
        if ( SiteRemaining( body, facility ) >= 0 ) return "SITE ALREADY ASSIGNED";
        var plan = Facility( facility );
        if ( OccupiedSlots( body, plan.Layer ) >= Slots( body ).For( plan.Layer ) ) return "NO FREE SITE SLOTS";
        if ( facility is PlanetFacility.Mine or PlanetFacility.Skimmer && !HasDeposit( body ) ) return "NO ORE DEPOSIT";
        if ( facility == PlanetFacility.Habitat && !Researched( ResearchTopic.Habitation ) ) return "REQUIRES HABITAT ENGINEERING";
        if ( unit is not { } worker || !ValidConstructor( worker ) ) return "SELECT A CONSTRUCTOR";
        if ( !CanReach( worker, body ) ) return "OUTSIDE CONSTRUCTOR RANGE";
        foreach ( var assignment in _siteConstruction )
            if ( assignment.Worker?.Squad == worker.Squad ) return "CONSTRUCTOR ALREADY ASSIGNED";
        var cost = plan.Cost;
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
        orbit.Construction = (byte)((int)facility + 1);
        orbit.Service = ServiceOrder.None;
        orbit.ServiceTicks = 0;
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
        var order = EffectiveOrder( worker );
        if ( !ValidConstructor( worker ) || order.Body != body + 1 || order.Construction != (int)facility + 1 ) {
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
        if ( Frontier && Facility( facility ).Layer == SiteLayer.Surface ) {
            var (surfaceY, surfaceX) = MathF.SinCos( (facility == PlanetFacility.Mine ? -1.4f : 1.2f) + 0.003f * tick );
            return center + new Vector2( surfaceX, surfaceY ) * (_bodies[body].Radius * 0.84f);
        }
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
