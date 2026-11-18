using System.Numerics;

namespace FleetBattler;

internal enum SiteLayer { Surface, Orbit }
internal readonly record struct FacilityPlan( string Name, ResourceCost Cost, int Duration, SiteLayer Layer = SiteLayer.Orbit,
    int Output = 0, int UpgradeBonus = 0, int UpgradeTicks = 0, ResourceCost UpgradeCost = default, ResearchTopic? Requirement = null );
internal readonly record struct SiteSlots( int Surface, int Orbit ) {
    internal int For( SiteLayer layer ) => layer == SiteLayer.Surface ? Surface : Orbit;
}
internal record struct ConstructionSite( FleetSelection? Worker, Vector2 Orbit, bool Working );

internal sealed partial class FleetSimulation {
    internal const int FacilityCount = 10;
    internal static ReadOnlySpan<PlanetFacility> Facilities => [
        PlanetFacility.Mine, PlanetFacility.Observatory, PlanetFacility.Skimmer, PlanetFacility.Lab,
        PlanetFacility.Yard, PlanetFacility.Depot, PlanetFacility.Solar, PlanetFacility.Habitat,
        PlanetFacility.SurfaceStore, PlanetFacility.OrbitalStore
    ];
    private readonly int[] _solarArrays = new int[CelestialSystem.BodyCount];
    private readonly int[] _habitats = new int[CelestialSystem.BodyCount], _observatories = new int[CelestialSystem.BodyCount],
        _skimmers = new int[CelestialSystem.BodyCount], _surfaceStores = new int[CelestialSystem.BodyCount],
        _orbitalStores = new int[CelestialSystem.BodyCount];
    private readonly ConstructionSite[] _siteConstruction = new ConstructionSite[CelestialSystem.BodyCount * FacilityCount];
    internal SiteOrder LastStartedSite { get; private set; }

    internal static FacilityPlan Facility( PlanetFacility facility ) => GameCatalog.Facility( facility );

    private readonly int[] _siteUpgrades = new int[CelestialSystem.BodyCount * FacilityCount];
    private ref int UpgradeDeadline( int body, PlanetFacility facility ) => ref _siteUpgrades[body * FacilityCount + (int)facility];

    internal int SiteUpgradeRemaining( int body, PlanetFacility facility ) => Remaining( UpgradeDeadline( body, facility ) );
    internal int SiteUpgradeBonus( int body, PlanetFacility facility ) =>
        SiteUpgradeRemaining( body, facility ) == 0 ? Facility( facility ).UpgradeBonus : 0;
    internal int StructureOutput( PlanetFacility facility ) {
        int output = 0;
        var plan = Facility( facility );
        for ( int body = 1; body < _bodies.Length; ++body )
            if ( SiteRemaining( body, facility ) == 0 ) output += plan.Output + SiteUpgradeBonus( body, facility );
        return output;
    }

    internal string? UpgradeBlock( int body, PlanetFacility facility ) {
        if ( !Frontier || body <= 0 || body >= _bodies.Length || SiteRemaining( body, facility ) != 0 ) return "NEEDS AN ONLINE SITE";
        var plan = Facility( facility );
        if ( plan.UpgradeBonus == 0 ) return "NO UPGRADE PACKAGE";
        if ( SiteUpgradeRemaining( body, facility ) >= 0 ) return "UPGRADE ALREADY ORDERED";
        if ( plan.Requirement is { } requirement && !Researched( requirement ) ) return $"REQUIRES {Tech( requirement ).Name}";
        return CanAfford( plan.UpgradeCost ) ? null : "INSUFFICIENT RESOURCES";
    }

    internal bool UpgradeSite( int body, PlanetFacility facility ) {
        if ( UpgradeBlock( body, facility ) is not null ) return false;
        var plan = Facility( facility );
        UpgradeDeadline( body, facility ) = checked(Tick + plan.UpgradeTicks);
        Spend( plan.UpgradeCost );
        return true;
    }

    private ref ConstructionSite Construction( int body, PlanetFacility facility ) =>
        ref _siteConstruction[body * FacilityCount + (int)facility];

    internal SiteSlots Slots( int body ) {
        if ( body <= 0 || body >= _bodies.Length ) return default;
        float radius = _bodies[body].Radius / BoardRadius;
        int surface = !Colonizable( body ) ? 0 : radius >= 0.045f ? 3 : radius >= 0.030f ? 2 : 1;
        int orbit = radius >= 0.060f ? 5 : radius >= 0.030f ? 4 : 3;
        return new( surface, orbit );
    }

    internal bool FitsBody( int body, PlanetFacility facility ) {
        var slots = Slots( body );
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
        int used = OccupiedSlots( body, plan.Layer );
        if ( used >= Slots( body ).For( plan.Layer ) ) return "NO FREE SITE SLOTS";
        if ( plan.Layer == SiteLayer.Surface && used >= SurfaceWorkLimit( body ) )
            return _population[body] == 0 ? "FOUND A COLONY FIRST" : "NEEDS MORE POPULATION";
        if ( facility is PlanetFacility.Mine or PlanetFacility.Skimmer && !HasDeposit( body ) ) return "NO ORE DEPOSIT";
        if ( facility == PlanetFacility.Yard && !(Established && body == Homeworld) ) {
            if ( _population[body] < YardPopulation ) return "SHIPYARD NEEDS 6 POPULATION";
            if ( DepotAt( body ) != 0 ) return "SHIPYARD NEEDS AN ONLINE DEPOT";
        }
        if ( facility == PlanetFacility.Habitat && !Researched( ResearchTopic.Habitation ) ) return "REQUIRES HABITAT ENGINEERING";
        if ( plan.Layer == SiteLayer.Orbit ) {
            if ( unit is not { } worker || !ValidConstructor( worker ) ) return "SELECT A CONSTRUCTOR";
            if ( !CanReach( worker, body ) ) return "OUTSIDE CONSTRUCTOR RANGE";
            foreach ( var assignment in _siteConstruction )
                if ( assignment.Worker?.Squad == worker.Squad ) return "CONSTRUCTOR ALREADY ASSIGNED";
        }
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
        var plan = Facility( facility );
        if ( plan.Layer == SiteLayer.Orbit ) {
            var worker = unit!.Value;
            var orbit = OrbitalOrder( worker, body, survey: false );
            orbit.Construction = (byte)((int)facility + 1);
            orbit.Service = ServiceOrder.None;
            orbit.ServiceTicks = 0;
            Construction( body, facility ) = new( worker, new( orbit.Radius, orbit.Phase + 0.24f ), false );
            SetOrder( worker, orbit );
        }
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
        if ( deadlines[body] <= Tick + 1 ) {
            ReleaseConstructor( body, facility, worker );
            site.Worker = null;
        }
    }

    private void ReleaseConstructor( int body, PlanetFacility facility, FleetSelection worker ) {
        ref var order = ref (worker.IsShip ? ref _shipOrders[worker.Ship] : ref _squads[worker.Squad].Order);
        if ( order.Body == body + 1 && order.Construction == (int)facility + 1 ) order.Construction = 0;
    }

    internal Vector2 SitePosition( int body, PlanetFacility facility, float interpolation ) =>
        SitePositionAt( body, facility, Math.Max( 0, Tick - 1 + interpolation ), VisualBody( body, interpolation ).Center );

    private Vector2 SitePositionAt( int body, PlanetFacility facility, float tick, Vector2 center ) {
        var orbit = Construction( body, facility ).Orbit;
        if ( Frontier && Facility( facility ).Layer == SiteLayer.Surface ) {
            float offset = facility switch { PlanetFacility.Mine => -1.4f, PlanetFacility.Observatory => 1.2f, _ => 2.8f };
            var (surfaceY, surfaceX) = MathF.SinCos( offset + 0.003f * tick );
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
