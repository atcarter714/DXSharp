using System.Numerics;

namespace FleetBattler;

internal enum PlanetFacility { Mine, Lab, Depot, Yard, Solar, Habitat, Observatory, Skimmer }
internal readonly record struct WingOrder( int Body, ShipDesign Design, int Duration, int Cost );
internal readonly record struct SiteOrder( int Body, PlanetFacility Facility );

internal readonly record struct PlanetStatus( int Survey, int MineRemaining, int LabRemaining, FleetSelection? SurveyUnit, int SurveyRequired ) {
    internal bool Surveyed => Survey >= SurveyRequired;
    internal float SurveyProgress => (float)Survey / SurveyRequired;
}

internal sealed partial class FleetSimulation {
    internal const int SurveyDuration = 120, MineCost = 35, LabCost = 45, UpgradeCost = 30;
    internal const int FacilityDuration = 180, FrontierSurveyDuration = 480;
    internal const int YardCost = 55, YardDuration = 360, IndustryCost = 24, DepotCost = 40;
    internal const int YardNavalCapacity = 6, DepotNavalCapacity = 3, DriveCost = 36;
    internal const int StandardWingDuration = 480, ImprovedWingDuration = 300, ImprovedWingCost = 42;
    private readonly FleetSelection?[] _surveyUnits = new FleetSelection?[CelestialSystem.BodyCount];
    private readonly WingOrder[] _wingQueue = new WingOrder[WingGoal];
    private readonly int[] _yards = new int[CelestialSystem.BodyCount], _depots = new int[CelestialSystem.BodyCount];
    private int _wingFinishTick;
    private readonly int[] _survey = new int[CelestialSystem.BodyCount];
    private readonly int[] _mines = new int[CelestialSystem.BodyCount];
    private readonly int[] _labs = new int[CelestialSystem.BodyCount];

    internal int Science { get; private set; }
    internal bool WeaponUpgrade { get; private set; }
    internal bool IndustryUpgrade { get; private set; }
    internal bool DriveUpgrade { get; private set; }
    internal int YardBody {
        get {
            int pending = -1;
            for ( int body = 1; body < _bodies.Length; ++body ) {
                int remaining = YardAt( body );
                if ( remaining == 0 ) return body;
                if ( remaining > 0 && pending < 0 ) pending = body;
            }
            return pending;
        }
    }
    internal int YardRemaining => YardBody is >= 0 and var body ? YardAt( body ) : -1;
    internal Vector2 Shipyard => YardBody is >= 0 and var body ? YardPosition( body, Tick ) : default;
    internal int YardAt( int body ) => Remaining( _yards[body] );
    internal int DepotAt( int body ) => Remaining( _depots[body] );
    internal Vector2 YardAtPosition( int body ) => YardPosition( body, Tick );
    internal int YardCount => Online( _yards );
    internal int DepotCount => Online( _depots );
    internal const int HabitatNavalCapacity = 4;
    internal int NavalCapacity => YardCount * YardNavalCapacity + DepotCount * DepotNavalCapacity + Online( _habitats ) * HabitatNavalCapacity;
    internal int NavalUsed {
        get {
            int used = 0;
            for ( int squad = 0; squad < SquadCount; ++squad )
                if ( IsPlayerSquad( squad ) && Role( squad ) == HullRole.Corvette ) used += ShipCount( squad );
            return used;
        }
    }
    internal int NavalReserved => Frontier ? _navalReserved : QueuedWings * WingSize;

    private int Online( ReadOnlySpan<int> projects ) {
        int count = 0;
        foreach ( int deadline in projects )
            if ( deadline > 0 && deadline <= Tick ) ++count;
        return count;
    }

    private Vector2 YardPosition( int body, int tick ) =>
        SitePositionAt( body, PlanetFacility.Yard, tick, _bodies[body].Center );
    internal int QueuedWings { get; private set; }
    internal int WingRemaining => Remaining( _wingFinishTick );
    internal int CurrentWingCost => IndustryUpgrade ? ImprovedWingCost : WingCost;
    internal int CurrentWingDuration => IndustryUpgrade ? ImprovedWingDuration : StandardWingDuration;
    internal int ActiveWingDuration => QueuedWings == 0 ? 0 : _wingQueue[0].Duration;
    internal bool CanBuildWing => !Frontier && YardRemaining == 0 && Ore >= CurrentWingCost &&
        WingsBuilt + QueuedWings < WingGoal && !Lost;
    internal int CompletedSites { get; private set; }
    internal SiteOrder LastCompletedSite { get; private set; }
    internal int StartedConstruction { get; private set; }
    internal int CompletedSurveys { get; private set; }
    internal int StartedScans { get; private set; }
    internal int LastSurveyBody { get; private set; }
    internal FleetSelection LastSurveyUnit { get; private set; }

    internal PlanetStatus Planet( int body ) =>
        new( _survey[body], ExtractionRemaining( body ), Remaining( _labs[body] ), _surveyUnits[body], SurveyRequired );

    private int SurveyRequired => Frontier ? FrontierSurveyDuration : SurveyDuration;

    private int Remaining( int deadline ) => deadline == 0 ? -1 : Math.Max( 0, deadline - Tick );

    internal bool SurveyWorking( int body ) => _surveyUnits[body] is { } unit && InSurveyRange( unit, body );

    internal bool CanSurvey( FleetSelection unit, int body ) =>
        body > 0 && body < _bodies.Length && _survey[body] < SurveyRequired && _surveyUnits[body] is null &&
        (uint)unit.Squad < (uint)SquadCount && IsPlayerSquad( unit.Squad ) && (!Frontier || IsScoutSquad( unit.Squad )) &&
        (unit.IsShip ? Alive( unit.Ship ) && SquadronOf( unit.Ship ) == unit.Squad : ShipCount( unit.Squad ) > 0) && (!Frontier || CanReach( unit, body ));

    internal bool BeginSurvey( FleetSelection unit, int body ) {
        if ( !CanSurvey( unit, body ) ) return false;
        var order = OrbitalOrder( unit, body, survey: true );
        order.Construction = 0;
        order.Service = ServiceOrder.None;
        SetOrder( unit, order );
        _surveyUnits[body] = unit;
        return true;
    }

    internal bool CancelSurvey( int body ) {
        if ( body <= 0 || body >= _bodies.Length || _surveyUnits[body] is not { } unit ) return false;
        ref var order = ref (unit.IsShip ? ref _shipOrders[unit.Ship] : ref _squads[unit.Squad].Order);
        order.Survey = false;
        _surveyUnits[body] = null;
        return true;
    }

    private void RefreshSurveys( ) {
        for ( int body = 1; body < _surveyUnits.Length; ++body ) {
            if ( _surveyUnits[body] is not { } unit ) continue;
            var order = unit.IsShip ? _shipOrders[unit.Ship] : _squads[unit.Squad].Order;
            if ( !order.Survey || order.Body != body + 1 ) _surveyUnits[body] = null;
            else if ( unit.IsShip ? !Alive( unit.Ship ) : !SquadCanSurvey( unit.Squad ) ) CancelSurvey( body );
        }
    }

    private bool SquadCanSurvey( int squad ) {
        ref readonly var group = ref _squads[squad];
        for ( int ship = group.First; ship < group.First + group.Count; ++ship )
            if ( Alive( ship ) && !_shipOrders[ship].Detached ) return true;
        return false;
    }

    internal bool BuildYard( int body, FleetSelection? worker = null ) {
        if ( Frontier ) return StartConstruction( body, PlanetFacility.Yard, worker );
        if ( body <= 0 || body >= _bodies.Length || _yards[body] != 0 || YardBody >= 0 || !Planet( body ).Surveyed || Lost || Ore < YardCost ) return false;
        Ore -= YardCost;
        _yards[body] = Tick + YardDuration;
        return true;
    }

    internal bool ResearchIndustry( ) {
        if ( Frontier ) return StartResearch( ResearchTopic.Industry );
        if ( IndustryUpgrade || Science < IndustryCost || Lost ) return false;
        Science -= IndustryCost;
        IndustryUpgrade = true;
        return true;
    }

    internal bool QueueWing( ) {
        if ( !CanBuildWing ) return false;
        int cost = CurrentWingCost;
        _wingQueue[QueuedWings++] = new( YardBody, Design, CurrentWingDuration, cost );
        Ore -= cost;
        if ( QueuedWings == 1 ) _wingFinishTick = Tick + _wingQueue[0].Duration;
        return true;
    }

    private void AdvanceProduction( ) {
        if ( QueuedWings == 0 ) return;
        if ( Lost ) {
            QueuedWings = 0;
            _wingFinishTick = 0;
            return;
        }
        if ( Tick + 1 < _wingFinishTick ) return;
        int body = _wingQueue[0].Body;
        var arrival = YardPosition( body, Tick + 1 ) + new Vector2( 4 + WingsBuilt * 4, 0 );
        AddSquad( WingSize, 0, arrival );
        ++WingsBuilt;
        --QueuedWings;
        _wingQueue.AsSpan( 1, QueuedWings ).CopyTo( _wingQueue );
        _wingFinishTick = QueuedWings == 0 ? 0 : Tick + 1 + _wingQueue[0].Duration;
    }

    internal bool BuildFacility( int body, PlanetFacility facility, FleetSelection? worker = null ) {
        if ( Frontier ) return StartConstruction( body, facility, worker );
        if ( facility == PlanetFacility.Yard ) return BuildYard( body );
        if ( body <= 0 || body >= _bodies.Length || !Planet( body ).Surveyed || Lost ||
             facility is not (PlanetFacility.Mine or PlanetFacility.Lab) ) return false;
        var projects = SiteDeadlines( facility );
        int cost = Facility( facility ).Cost.Ore;
        if ( projects[body] != 0 || Ore < cost ) return false;
        Ore -= cost;
        projects[body] = Tick + FacilityDuration;
        return true;
    }

    internal int SiteRemaining( int body, PlanetFacility facility ) => Remaining( SiteDeadlines( facility )[body] );

    internal bool CancelSite( int body, PlanetFacility facility ) {
        if ( !Frontier || body <= 0 || body >= _bodies.Length ) return false;
        var projects = SiteDeadlines( facility );
        if ( projects[body] <= Tick ) return false;
        projects[body] = 0;
        Construction( body, facility ) = default;
        Refund( Facility( facility ).Cost );
        return true;
    }

    private int[] SiteDeadlines( PlanetFacility facility ) => facility switch {
        PlanetFacility.Mine => _mines, PlanetFacility.Lab => _labs, PlanetFacility.Depot => _depots,
        PlanetFacility.Yard => _yards, PlanetFacility.Solar => _solarArrays, PlanetFacility.Habitat => _habitats,
        PlanetFacility.Observatory => _observatories, PlanetFacility.Skimmer => _skimmers,
        _ => throw new ArgumentOutOfRangeException( nameof(facility) )
    };

    internal bool ResearchWeapons( ) {
        if ( Frontier ) return StartResearch( ResearchTopic.Weapons );
        if ( Science < UpgradeCost || WeaponUpgrade || Lost ) return false;
        Science -= UpgradeCost;
        WeaponUpgrade = true;
        return true;
    }

    private void AdvancePlanets( ) {
        RefreshSurveys( );
        for ( int body = 1; body < _bodies.Length; ++body ) {
            if ( _surveyUnits[body] is { } unit && InSurveyRange( unit, body ) ) {
                int rate = IsScoutSquad( unit.Squad ) ? AstrometryUpgrade ? 3 : 2 : 1;
                if ( _survey[body] == 0 ) ++StartedScans;
                _survey[body] = Math.Min( SurveyRequired, _survey[body] + rate );
                if ( _survey[body] == SurveyRequired ) {
                    Ore = Math.Min( 999, Ore + (Frontier ? 15 : 40) );
                    if ( Frontier ) RevealProspect( body );
                    ++CompletedSurveys;
                    LastSurveyBody = body;
                    LastSurveyUnit = unit;
                    CancelSurvey( body );
                }
            }
            if ( Frontier ) {
                foreach ( var facility in Facilities ) {
                    AdvanceConstruction( body, facility );
                    MarkSite( body, facility, SiteDeadlines( facility )[body] );
                }
            }
            if ( (Tick + 1) % IncomeInterval != 0 ) continue;
            if ( _mines[body] > 0 && _mines[body] <= Tick + 1 || _skimmers[body] > 0 && _skimmers[body] <= Tick + 1 )
                Ore = Math.Min( 999, Ore + (Frontier ? DepositYield( body ) : 8) );
            if ( _labs[body] > 0 && _labs[body] <= Tick + 1 ) Science = Math.Min( 999, Science + 6 );
            if ( _observatories[body] > 0 && _observatories[body] <= Tick + 1 ) Science = Math.Min( 999, Science + 10 );
        }
    }

    private void MarkSite( int body, PlanetFacility facility, int deadline ) {
        if ( deadline != Tick + 1 ) return;
        ++CompletedSites;
        LastCompletedSite = new( body, facility );
    }

    private bool InSurveyRange( FleetSelection unit, int body ) {
        ref readonly var group = ref _squads[unit.Squad];
        int first = unit.IsShip ? unit.Ship : group.First;
        int end = unit.IsShip ? first + 1 : first + group.Count;
        if ( Frontier && (EffectiveOrder( unit ) is not { Body: var host, Transfer.End: 0 } || host != body + 1) )
            return false;
        float radius = OrbitRadius( body, unit.IsShip ? 1 : Math.Max( 1, group.Alive ) ) + 8;
        var center = _bodies[body].Center;
        for ( int ship = first; ship < end; ++ship )
            if ( Alive( ship ) && (unit.IsShip || (!_shipOrders[ship].Detached && !_shipOrders[ship].Rejoining)) &&
                 Vector2.DistanceSquared( _positions[ship], center ) <= radius * radius ) return true;
        return false;
    }
}
