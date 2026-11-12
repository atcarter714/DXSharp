using System.Numerics;

namespace FleetBattler;

internal enum PlanetFacility { Mine, Lab, Depot }

internal readonly record struct PlanetStatus( int Survey, int MineRemaining, int LabRemaining, FleetSelection? SurveyUnit ) {
    internal bool Surveyed => Survey >= FleetSimulation.SurveyDuration;
}

internal sealed partial class FleetSimulation {
    internal const int SurveyDuration = 120, MineCost = 35, LabCost = 45, UpgradeCost = 30;
    internal const int FacilityDuration = 180;
    internal const int YardCost = 55, YardDuration = 360, IndustryCost = 24, DepotCost = 40;
    internal const int YardNavalCapacity = 6, DepotNavalCapacity = 3, DriveCost = 36;
    internal const int StandardWingDuration = 480, ImprovedWingDuration = 300, ImprovedWingCost = 42;
    private readonly FleetSelection?[] _surveyUnits = new FleetSelection?[CelestialSystem.BodyCount];
    private readonly int[] _queuedWingTicks = new int[WingGoal];
    private readonly ShipDesign[] _queuedDesigns = new ShipDesign[WingGoal];
    private readonly int[] _yards = new int[CelestialSystem.BodyCount], _depots = new int[CelestialSystem.BodyCount];
    private readonly int[] _queuedYards = new int[WingGoal];
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
    internal int NavalCapacity => YardCount * YardNavalCapacity + DepotCount * DepotNavalCapacity;
    internal int NavalUsed {
        get {
            int used = 0;
            for ( int squad = 0; squad < SquadCount; ++squad )
                if ( IsPlayerSquad( squad ) && Role( squad ) == HullRole.Corvette ) used += ShipCount( squad );
            return used;
        }
    }
    internal int NavalReserved => QueuedWings * FrontierWingSize;

    private int Online( ReadOnlySpan<int> projects ) {
        int count = 0;
        foreach ( int deadline in projects )
            if ( deadline > 0 && deadline <= Tick ) ++count;
        return count;
    }

    private Vector2 YardPosition( int body, int tick ) {
        var (sin, cos) = MathF.SinCos( tick * (float)StepSeconds * 0.23f - MathF.PI / 2 );
        return _bodies[body].Center + new Vector2( cos, sin ) * (OrbitRadius( body, 1 ) + 4);
    }
    internal int QueuedWings { get; private set; }
    internal int WingRemaining => Remaining( _wingFinishTick );
    internal int CurrentWingCost => (IndustryUpgrade ? ImprovedWingCost : WingCost) + (Frontier ? Design.Surcharge : 0);
    internal int CurrentWingDuration => (IndustryUpgrade ? ImprovedWingDuration : StandardWingDuration) + (Frontier ? Design.BuildTicks : 0);
    internal int ActiveYard => QueuedWings == 0 ? -1 : _queuedYards[0];
    internal int OrdersAt( int body ) {
        int count = 0;
        for ( int i = 0; i < QueuedWings; ++i )
            if ( _queuedYards[i] == body ) ++count;
        return count;
    }
    internal int ActiveWingDuration => QueuedWings == 0 ? 0 : _queuedWingTicks[0];
    internal ShipDesign ActiveWingDesign => _queuedDesigns[0];
    internal bool CanBuildWing => CanQueueWing( YardBody );
    internal bool CanQueueWing( int body ) => (uint)body < (uint)_bodies.Length && YardAt( body ) == 0 && Ore >= CurrentWingCost &&
        (Frontier ? QueuedWings < _queuedWingTicks.Length && NavalUsed + NavalReserved + FrontierWingSize <= NavalCapacity
            : WingsBuilt + QueuedWings < WingGoal && !Lost);
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
        (unit.IsShip ? Alive( unit.Ship ) && SquadronOf( unit.Ship ) == unit.Squad : ShipCount( unit.Squad ) > 0) && (!Frontier || CanReach( unit, body ));

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
        if ( body <= 0 || body >= _bodies.Length || _yards[body] != 0 || !Frontier && YardBody >= 0 || !Planet( body ).Surveyed || Lost || Ore < YardCost ) return false;
        Ore -= YardCost;
        _yards[body] = Tick + YardDuration;
        return true;
    }

    internal bool ResearchIndustry( ) {
        if ( IndustryUpgrade || Science < IndustryCost || Lost ) return false;
        Science -= IndustryCost;
        IndustryUpgrade = true;
        return true;
    }

    internal bool ResearchDrive( ) {
        if ( !Frontier || DriveUpgrade || Science < DriveCost ) return false;
        Science -= DriveCost;
        DriveUpgrade = true;
        return true;
    }

    internal bool QueueWing( int body = -1 ) {
        if ( body < 0 ) body = YardBody;
        if ( !CanQueueWing( body ) ) return false;
        _queuedYards[QueuedWings] = body;
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
        int body = _queuedYards[0];
        var arrival = YardPosition( body, Tick + 1 ) + new Vector2( 4 + (Frontier ? WingsBuilt % 2 : WingsBuilt) * 4, 0 );
        int squad = AddSquad( Frontier ? FrontierWingSize : WingSize, 0, arrival, design: _queuedDesigns[0] );
        if ( Frontier ) Park( squad, body );
        ++WingsBuilt;
        --QueuedWings;
        _queuedYards.AsSpan( 1, QueuedWings ).CopyTo( _queuedYards );
        _queuedWingTicks.AsSpan( 1, QueuedWings ).CopyTo( _queuedWingTicks );
        _queuedDesigns.AsSpan( 1, QueuedWings ).CopyTo( _queuedDesigns );
        _wingFinishTick = QueuedWings == 0 ? 0 : Tick + 1 + _queuedWingTicks[0];
    }

    internal bool BuildFacility( int body, PlanetFacility facility, FleetSelection? worker = null ) {
        if ( body <= 0 || body >= _bodies.Length || !Planet( body ).Surveyed || Lost ) return false;
        if ( facility == PlanetFacility.Depot && !Frontier ) return false;
        var (projects, cost) = facility switch {
            PlanetFacility.Mine => (_mines, MineCost),
            PlanetFacility.Lab => (_labs, LabCost),
            _ => (_depots, DepotCost)
        };
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
