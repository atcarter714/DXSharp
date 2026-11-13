namespace FleetBattler;

internal readonly record struct ColonyStatus( int Population, int Housing, int WorkRemaining, FleetSelection? Ship );

internal sealed partial class FleetSimulation {
    internal const int ColonyDuration = 900, ColonyLandingPopulation = 3, YardPopulation = 6;
    private const int ColonyGrowthInterval = 1350;
    private readonly int[] _population = new int[CelestialSystem.BodyCount], _nextGrowth = new int[CelestialSystem.BodyCount],
        _colonyWork = new int[CelestialSystem.BodyCount];
    private readonly FleetSelection?[] _colonyShips = new FleetSelection?[CelestialSystem.BodyCount];

    internal bool ColonySupplied( int body ) => body == Homeworld && Established ||
        SiteRemaining( body, PlanetFacility.Solar ) == 0 || DepotAt( body ) == 0;

    internal int ColoniesFounded { get; private set; }
    internal int LastFoundedBody { get; private set; }

    internal bool Colonizable( int body ) => body > 0 && body < _bodies.Length && body != 2 && body != CelestialSystem.OuterBelt &&
        (!Solar || body is not (8 or 9 or 10 or SolarCatalog.Halley));

    internal ColonyStatus Colony( int body ) => new( _population[body], Slots( body ).Surface * 6, _colonyWork[body], _colonyShips[body] );
    internal int SurfaceWorkLimit( int body ) => Math.Min( Slots( body ).Surface, (_population[body] + 3) / 4 );

    internal int ActiveColonyShips( ) {
        int count = 0;
        for ( int squad = 0; squad < SquadCount; ++squad )
            if ( IsPlayerSquad( squad ) && Role( squad ) == HullRole.Colony ) count += ShipCount( squad );
        return count;
    }

    internal string? ColonyBlock( int body, FleetSelection? selection ) {
        if ( !Frontier || !Colonizable( body ) ) return "NO SETTLEMENT SITE";
        if ( !Planet( body ).Surveyed ) return "SURVEY THE BODY FIRST";
        if ( _population[body] > 0 ) return "COLONY ALREADY ESTABLISHED";
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
        _colonyWork[body] = ColonyDuration;
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
