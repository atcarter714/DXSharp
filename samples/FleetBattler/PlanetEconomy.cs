using System.Numerics;

namespace FleetBattler;

internal enum PlanetFacility { Mine, Lab }

internal readonly record struct PlanetStatus( int Survey, int MineRemaining, int LabRemaining, FleetSelection? SurveyUnit ) {
    internal bool Surveyed => Survey >= FleetSimulation.SurveyDuration;
}

internal sealed partial class FleetSimulation {
    internal const int SurveyDuration = 120, MineCost = 35, LabCost = 45, UpgradeCost = 30;
    internal const int FacilityDuration = 180;
    internal const int YardCost = 55, YardDuration = 360, IndustryCost = 24;
    internal const int StandardWingDuration = 480, ImprovedWingDuration = 300, ImprovedWingCost = 42;
    private readonly FleetSelection?[] _surveyUnits = new FleetSelection?[CelestialSystem.BodyCount];
    private readonly int[] _queuedWingTicks = new int[WingGoal];
    private readonly ShipDesign[] _queuedDesigns = new ShipDesign[WingGoal];
    private int _yardBody = -1, _yardReadyTick, _wingFinishTick;
    private readonly int[] _survey = new int[CelestialSystem.BodyCount];
    private readonly int[] _mines = new int[CelestialSystem.BodyCount];
    private readonly int[] _labs = new int[CelestialSystem.BodyCount];

    internal int Science { get; private set; }
    internal bool WeaponUpgrade { get; private set; }
    internal bool IndustryUpgrade { get; private set; }
    internal int YardBody => _yardBody;
    internal int YardRemaining => Remaining( _yardReadyTick );
    internal Vector2 Shipyard => YardPosition( Tick );

    private Vector2 YardPosition( int tick ) {
        if ( _yardBody < 0 ) return default;
        var (sin, cos) = MathF.SinCos( tick * (float)StepSeconds * 0.23f - MathF.PI / 2 );
        return _bodies[_yardBody].Center + new Vector2( cos, sin ) * (OrbitRadius( _yardBody, 1 ) + 4);
    }
    internal int QueuedWings { get; private set; }
    internal int WingRemaining => Remaining( _wingFinishTick );
    internal int CurrentWingCost => (IndustryUpgrade ? ImprovedWingCost : WingCost) + (Frontier ? Design.Surcharge : 0);
    internal int CurrentWingDuration => (IndustryUpgrade ? ImprovedWingDuration : StandardWingDuration) + (Frontier ? Design.BuildTicks : 0);
    internal int ActiveWingDuration => QueuedWings == 0 ? 0 : _queuedWingTicks[0];
    internal ShipDesign ActiveWingDesign => _queuedDesigns[0];
    internal bool CanBuildWing => YardRemaining == 0 && Ore >= CurrentWingCost && WingsBuilt + QueuedWings < WingGoal && !Lost;
    internal int CompletedSurveys { get; private set; }
    internal int LastSurveyBody { get; private set; }
    internal FleetSelection LastSurveyUnit { get; private set; }

    internal PlanetStatus Planet( int body ) =>
        new( _survey[body], Remaining( _mines[body] ), Remaining( _labs[body] ), _surveyUnits[body] );

    private int Remaining( int deadline ) => deadline == 0 ? -1 : Math.Max( 0, deadline - Tick );

    internal bool SurveyWorking( int body ) => _surveyUnits[body] is { } unit && InSurveyRange( unit, body );

    internal bool CanSurvey( FleetSelection unit, int body ) =>
        body > 0 && body < _bodies.Length && _survey[body] < SurveyDuration && _surveyUnits[body] is null &&
        (uint)unit.Squad < (uint)SquadCount && IsPlayerSquad( unit.Squad ) && (!Frontier || IsScoutSquad( unit.Squad )) &&
        (unit.IsShip ? Alive( unit.Ship ) && SquadronOf( unit.Ship ) == unit.Squad : ShipCount( unit.Squad ) > 0);

    internal bool BeginSurvey( FleetSelection unit, int body ) {
        if ( !CanSurvey( unit, body ) ) return false;
        SetOrder( unit, OrbitalOrder( unit, body, survey: true ) );
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

    internal bool BuildYard( int body ) {
        if ( body <= 0 || body >= _bodies.Length || _yardBody >= 0 || !Planet( body ).Surveyed || Lost || Ore < YardCost ) return false;
        Ore -= YardCost;
        _yardBody = body;
        _yardReadyTick = Tick + YardDuration;
        return true;
    }

    internal bool ResearchIndustry( ) {
        if ( IndustryUpgrade || Science < IndustryCost || Lost ) return false;
        Science -= IndustryCost;
        IndustryUpgrade = true;
        return true;
    }

    internal bool QueueWing( ) {
        if ( !CanBuildWing ) return false;
        Ore -= CurrentWingCost;
        _queuedDesigns[QueuedWings] = Design;
        _queuedWingTicks[QueuedWings++] = CurrentWingDuration;
        if ( QueuedWings == 1 ) _wingFinishTick = Tick + _queuedWingTicks[0];
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
        var arrival = YardPosition( Tick + 1 ) + new Vector2( 4 + WingsBuilt * 4, 0 );
        AddSquad( Frontier ? FrontierWingSize : WingSize, 0, arrival, design: _queuedDesigns[0] );
        ++WingsBuilt;
        --QueuedWings;
        _queuedWingTicks.AsSpan( 1, QueuedWings ).CopyTo( _queuedWingTicks );
        _queuedDesigns.AsSpan( 1, QueuedWings ).CopyTo( _queuedDesigns );
        _wingFinishTick = QueuedWings == 0 ? 0 : Tick + 1 + _queuedWingTicks[0];
    }

    internal bool BuildFacility( int body, PlanetFacility facility, FleetSelection? worker = null ) {
        if ( body <= 0 || body >= CelestialSystem.BodyCount || !Planet( body ).Surveyed || Lost ) return false;
        var projects = facility == PlanetFacility.Mine ? _mines : _labs;
        int cost = facility == PlanetFacility.Mine ? MineCost : LabCost;
        if ( projects[body] != 0 || Ore < cost ) return false;
        if ( Frontier && facility == PlanetFacility.Mine && !AssignMine( body, worker ) ) return false;
        Ore -= cost;
        projects[body] = Tick + FacilityDuration;
        return true;
    }

    internal bool ResearchWeapons( ) {
        if ( Science < UpgradeCost || WeaponUpgrade || Lost ) return false;
        Science -= UpgradeCost;
        WeaponUpgrade = true;
        return true;
    }

    private void AdvancePlanets( ) {
        RefreshSurveys( );
        for ( int body = 1; body < _bodies.Length; ++body ) {
            if ( Frontier ) AdvanceMine( body );
            if ( _surveyUnits[body] is { } unit && InSurveyRange( unit, body ) ) {
                int rate = IsScoutSquad( unit.Squad ) ? 2 : 1;
                _survey[body] = Math.Min( SurveyDuration, _survey[body] + rate );
                if ( _survey[body] == SurveyDuration ) {
                    Ore = Math.Min( 999, Ore + (Frontier ? 15 : 40) );
                    if ( Frontier ) RevealProspect( body );
                    ++CompletedSurveys;
                    LastSurveyBody = body;
                    LastSurveyUnit = unit;
                    CancelSurvey( body );
                }
            }
            if ( (Tick + 1) % IncomeInterval != 0 ) continue;
            if ( _mines[body] > 0 && _mines[body] <= Tick + 1 ) Ore = Math.Min( 999, Ore + (Frontier ? DepositYield( body ) : 8) );
            if ( _labs[body] > 0 && _labs[body] <= Tick + 1 ) Science = Math.Min( 999, Science + 6 );
        }
    }

    private bool InSurveyRange( FleetSelection unit, int body ) {
        ref readonly var group = ref _squads[unit.Squad];
        int first = unit.IsShip ? unit.Ship : group.First;
        int end = unit.IsShip ? first + 1 : first + group.Count;
        float radius = OrbitRadius( body, unit.IsShip ? 1 : Math.Max( 1, group.Alive ) ) + 8;
        var center = _bodies[body].Center;
        for ( int ship = first; ship < end; ++ship )
            if ( Alive( ship ) && (unit.IsShip || !_shipOrders[ship].Detached) &&
                 Vector2.DistanceSquared( _positions[ship], center ) <= radius * radius ) return true;
        return false;
    }
}
