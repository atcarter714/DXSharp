using System.Numerics;

namespace FleetBattler;

internal readonly record struct ColonyStatus( int Population, int Housing, int WorkRemaining, FleetSelection? Ship );

internal sealed partial class FleetSimulation {
    internal const int ColonyDuration = 900, ColonyLandingPopulation = 3, YardPopulation = 6;
    private const int SkirmishGrowthInterval = 1350;
    private int ColonyGrowthInterval => FrontierProgression ? Pacing.Table.ColonyGrowthTicks : SkirmishGrowthInterval;
    internal int ColonyLandingTicks => FrontierProgression ? Pacing.Table.ColonyLandingTicks : ColonyDuration;
    private readonly int[] _population = new int[CelestialSystem.BodyCount], _nextGrowth = new int[CelestialSystem.BodyCount],
        _colonyWork = new int[CelestialSystem.BodyCount];
    private readonly FleetSelection?[] _colonyShips = new FleetSelection?[CelestialSystem.BodyCount];
    private readonly int[] _climateReady = new int[CelestialSystem.BodyCount];

    internal bool ColonySupplied( int body ) => body == Homeworld && Established ||
        SiteRemaining( body, PlanetFacility.Solar ) == 0 || DepotAt( body ) == 0;

    internal int ColoniesFounded { get; private set; }
    internal int LastFoundedBody { get; private set; }

    internal bool Colonizable( int body ) => body > 0 && body < _bodies.Length && body != 2 && body != CelestialSystem.OuterBelt &&
        (!Solar || body is not (8 or 9 or 10 or SolarCatalog.Halley));

    internal int NaturalHabitability( int body ) => Solar ? SolarCatalog.Get( body ).Habitability : body switch {
        1 when SystemIndex >= CelestialSystem.AuthoredCount => Exoplanets.Get( SystemIndex ).Habitability,
        1 => SystemIndex == Proxima ? 42 : 33,
        3 => 14,
        _ => 0
    };

    internal int Habitability( int body ) => NaturalHabitability( body ) + (ClimateRemaining( body ) == 0 ? 20 : 0);
    internal int ClimateRemaining( int body ) => Remaining( _climateReady[body] );
    private static readonly ResourceCost ClimateCost = new( 55, 100, 30 );

    internal string? ClimateBlock( int body ) {
        if ( !Frontier || !Colonizable( body ) || NaturalHabitability( body ) is < 20 or >= 70 || !Planet( body ).Surveyed )
            return "NO MARGINAL WORLD TO CONDITION";
        if ( ClimateRemaining( body ) >= 0 ) return "CLIMATE PROJECT ALREADY ORDERED";
        if ( !Researched( ResearchTopic.ClimateEngineering ) ) return "RESEARCH CLIMATE ENGINEERING";
        if ( SiteRemaining( body, PlanetFacility.Habitat ) != 0 || SiteRemaining( body, PlanetFacility.Solar ) != 0 )
            return "NEEDS LOCAL HABITAT AND SOLAR ARRAY";
        return CanAfford( ClimateCost ) ? null : "NEEDS 55 ORE / 100 ENERGY / 30 SCI";
    }

    internal bool BeginClimateProject( int body ) {
        if ( ClimateBlock( body ) is not null ) return false;
        Spend( ClimateCost );
        _climateReady[body] = Tick + (FrontierProgression ? Pacing.Table.ClimateTicks : 3600);
        return true;
    }

    internal ColonyStatus Colony( int body ) => new( _population[body], Slots( body ).Surface * 6, _colonyWork[body], _colonyShips[body] );
    internal int SurfaceWorkLimit( int body ) => Math.Min( Slots( body ).Surface, (_population[body] + 3) / 4 );

    internal int ActiveColonyShips( ) {
        int count = 0;
        for ( int squad = 0; squad < SquadCount; ++squad )
            if ( IsPlayerSquad( squad ) && Role( squad ) == HullRole.Colony ) count += ShipCount( squad );
        return count;
    }

    internal FleetSelection? FindColonyShip( int body, ReadOnlySpan<bool> selected ) {
        if ( !Colonizable( body ) ) return null;
        FleetSelection? nearby = null;
        float closest = float.PositiveInfinity;
        float range = OrbitRadius( body, 1 ) + 10;
        for ( int ship = 0; ship < Count; ++ship ) {
            var unit = new FleetSelection( SquadronOf( ship ), ship );
            if ( !ValidColonyShip( unit ) || !CanReach( unit, body ) ) continue;
            var order = EffectiveOrder( unit );
            if ( order.Settlement || order.Service != ServiceOrder.None || order.Enemy != 0 ) continue;
            if ( ship < selected.Length && selected[ship] ) return unit;
            float distance = Vector2.DistanceSquared( _positions[ship], _bodies[body].Center );
            if ( order.Body != body + 1 && distance > range * range || distance >= closest ) continue;
            closest = distance;
            nearby = unit;
        }
        return nearby;
    }

    internal string? ColonyBlock( int body, FleetSelection? selection ) {
        if ( !Frontier || !Colonizable( body ) ) return "NO SETTLEMENT SITE";
        if ( !Planet( body ).Surveyed ) return "SURVEY THE BODY FIRST";
        if ( _population[body] > 0 ) return "COLONY ALREADY ESTABLISHED";
        if ( FrontierProgression && Habitability( body ) < 70 ) {
            if ( !HabitationUpgrade ) return "RESEARCH HABITAT ENGINEERING";
            if ( Habitability( body ) < 38 && !Researched( ResearchTopic.ClosedEcology ) )
                return "RESEARCH CLOSED-LOOP ECOLOGY";
            if ( Habitability( body ) < 38 && SiteRemaining( body, PlanetFacility.Habitat ) != 0 )
                return "BUILD A LOCAL ORBITAL HABITAT";
        }
        if ( DepotAt( body ) != 0 ) return "BUILD A LANDING DEPOT FIRST";
        if ( _colonyShips[body] is not null ) return "COLONY SHIP ALREADY ASSIGNED";
        if ( selection is not { } unit || !ValidColonyShip( unit ) ) return "SELECT A COLONY SHIP";
        return CanReach( unit, body ) ? null : "OUTSIDE COLONY SHIP RANGE";
    }

    internal bool BeginColony( int body, FleetSelection unit ) {
        if ( ColonyBlock( body, unit ) is not null ) return false;
        var order = OrbitalOrder( unit, body, survey: false );
        order.Construction = 0;
        order.Service = ServiceOrder.None;
        order.Settlement = true;
        SetOrder( unit, order );
        _colonyShips[body] = unit;
        _colonyWork[body] = ColonyLandingTicks;
        return true;
    }

    internal bool CancelColony( int body ) {
        if ( (uint)body >= (uint)_colonyShips.Length || _colonyShips[body] is not { } unit ) return false;
        if ( ValidColonyShip( unit ) ) {
            ref var order = ref (unit.IsShip ? ref _shipOrders[unit.Ship] : ref _squads[unit.Squad].Order);
            if ( order.Body == body + 1 ) order.Settlement = false;
        }
        _colonyShips[body] = null;
        _colonyWork[body] = 0;
        return true;
    }

    internal bool Settling( int body ) => _colonyShips[body] is { } unit && InSurveyRange( unit, body );

    private bool ValidColonyShip( FleetSelection unit ) => (uint)unit.Squad < (uint)SquadCount &&
        IsPlayerSquad( unit.Squad ) && Role( unit.Squad ) == HullRole.Colony &&
        (unit.IsShip ? Alive( unit.Ship ) && SquadronOf( unit.Ship ) == unit.Squad : ShipCount( unit.Squad ) > 0);

    private void AdvanceColony( int body ) {
        if ( _colonyShips[body] is { } unit ) {
            var order = EffectiveOrder( unit );
            if ( !ValidColonyShip( unit ) || !order.Settlement || order.Body != body + 1 ) {
                // A different order can already be active on the same hull.
                _colonyShips[body] = null;
                _colonyWork[body] = 0;
            } else if ( InSurveyRange( unit, body ) && --_colonyWork[body] == 0 ) {
                _colonyShips[body] = null;
                int ship = unit.IsShip ? unit.Ship : _squads[unit.Squad].First;
                _hull[ship] = 0;
                --_squads[unit.Squad].Alive;
                --_factionCount[(int)FactionId.Sol];
                ReleaseFleetName( unit.Squad );
                _population[body] = ColonyLandingPopulation;
                _nextGrowth[body] = Tick + ColonyGrowthInterval;
                ++ColoniesFounded;
                LastFoundedBody = body;
            }
        }
        if ( _population[body] > 0 && _population[body] < Colony( body ).Housing && ColonySupplied( body ) && Tick >= _nextGrowth[body] ) {
            ++_population[body];
            _nextGrowth[body] = Tick + ColonyGrowthInterval;
        }
    }
}
