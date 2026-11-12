using System.Numerics;

namespace FleetBattler;

internal enum HullRole : byte { Corvette, Scout, Miner }
internal enum WeaponKind : byte { None, Pulse, Rail }
internal enum FrontierEvent : byte { Arrival, Prospect, Distress, Raid, Counterattack, Secured }
internal readonly record struct ShipDesign( WeaponKind Weapon, bool Plating ) {
    internal int Shield => Plating ? 12 : 34;
    internal int Armor => Plating ? 32 : 10;
    internal int Surcharge => Plating ? 8 : 0;
    internal int BuildTicks => Plating ? 60 : 0;
    internal int Range => Weapon == WeaponKind.Rail ? 21 : 16;
    internal int Reload => Weapon == WeaponKind.Rail ? 64 : 38;
    internal int Damage => Weapon == WeaponKind.Rail ? 22 : 13;
}

internal sealed partial class FleetSimulation {
    internal const int Homeworld = 1, WorkerCost = 20, WorkerDuration = 240;
    private const int FrontierCapacity = 32, FrontierWingSize = 3, MaxWorkers = 3;
    private readonly FrontierEvent[] _dispatches = new FrontierEvent[8];
    private readonly FleetSelection?[] _constructionWorkers = new FleetSelection?[CelestialSystem.BodyCount];
    private int _workerReadyTick, _workersBuilt, _raidTick, _raidsLaunched, _deposits;
    private int _surveyedProspects;
    private bool _secured;
    internal bool Frontier { get; }
    internal int DispatchCount { get; private set; }
    internal int WorkerRemaining => Remaining( _workerReadyTick );
    internal int Deposits => _deposits;
    internal int ActiveRaids => _raidsLaunched;
    internal bool CanQueueWorker => Frontier && !Lost && _workerReadyTick == 0 && _workersBuilt < MaxWorkers && Ore >= WorkerCost;
    internal ShipDesign Design { get; private set; } = new( WeaponKind.Pulse, false );
    internal HullRole Role( int squad ) => _squads[squad].Role;
    internal ShipDesign Loadout( int ship ) => _squads[_squadOfShip[ship]].Design;
    internal FrontierEvent Dispatch( int index ) => _dispatches[index];
    internal bool HasDeposit( int body ) => Frontier && (_deposits & (1 << body)) != 0;
    internal int DepositYield( int body ) => HasDeposit( body ) ? ProspectYield( body ) : 0;
    private int ProspectYield( int body ) => (body + SystemIndex) % 2 == 0 ? 12 : 8;

    internal void ConfigureDesign( bool weapon ) {
        if ( !Frontier ) return;
        Design = weapon ? Design with { Weapon = Design.Weapon == WeaponKind.Pulse ? WeaponKind.Rail : WeaponKind.Pulse }
            : Design with { Plating = !Design.Plating };
    }

    private void OpenFrontier( ) {
        _survey[Homeworld] = SurveyDuration;
        Ore = 50;
        Vector2 home = _bodies[Homeworld].Center + new Vector2( -OrbitRadius( Homeworld, 1 ), 0 );
        AddSquad( 1, 0, home, HullRole.Scout );
        AddSquad( 1, 0, home + new Vector2( -5, 5 ), HullRole.Miner );
        AddSquad( 1, 0, home + new Vector2( -5, -5 ), HullRole.Miner );
        SendDispatch( FrontierEvent.Arrival );
    }

    internal bool QueueWorker( ) {
        if ( !CanQueueWorker ) return false;
        Ore -= WorkerCost;
        _workerReadyTick = Tick + WorkerDuration;
        return true;
    }

    internal bool CanPlaceMine( int body, FleetSelection? unit ) {
        if ( !Frontier ) return true;
        return HasDeposit( body ) && unit is { } worker && (uint)worker.Squad < (uint)SquadCount &&
            IsPlayerSquad( worker.Squad ) && Role( worker.Squad ) == HullRole.Miner &&
            (worker.IsShip ? Alive( worker.Ship ) && SquadronOf( worker.Ship ) == worker.Squad : ShipCount( worker.Squad ) > 0);
    }

    private bool AssignMine( int body, FleetSelection? unit ) {
        if ( !CanPlaceMine( body, unit ) ) return false;
        var worker = unit!.Value;
        for ( int i = 1; i < _constructionWorkers.Length; ++i )
            if ( _constructionWorkers[i]?.Squad == worker.Squad ) return false;
        _constructionWorkers[body] = worker;
        SetOrder( worker, OrbitalOrder( worker, body, survey: false ) );
        return true;
    }

    internal bool MineEnRoute( int body ) =>
        _constructionWorkers[body] is not null && _mines[body] > Tick && !MineOnSite( body );
    private bool MineOnSite( int body ) => _constructionWorkers[body] is { } worker &&
        InSurveyRange( worker, body );

    private void AdvanceMine( int body ) {
        if ( _constructionWorkers[body] is not { } worker ) return;
        bool alive = worker.IsShip ? Alive( worker.Ship ) : ShipCount( worker.Squad ) != 0;
        if ( !alive || EffectiveOrder( worker ).Body != body + 1 ) {
            _constructionWorkers[body] = null;
            _mines[body] = 0;
            Ore = Math.Min( 999, Ore + MineCost );
            return;
        }
        if ( !MineOnSite( body ) ) ++_mines[body];
        if ( _mines[body] <= Tick + 1 ) _constructionWorkers[body] = null;
    }

    private void RevealProspect( int body ) {
        ++_surveyedProspects;
        _deposits |= 1 << body;
        if ( _surveyedProspects == 1 ) SendDispatch( FrontierEvent.Prospect );
    }

    private void AdvanceFrontier( ) {
        if ( !Frontier || Lost || Won ) return;
        if ( _workerReadyTick != 0 && Tick + 1 >= _workerReadyTick ) {
            _workerReadyTick = 0;
            ++_workersBuilt;
            AddSquad( 1, 0, _bodies[Homeworld].Center + new Vector2( -OrbitRadius( Homeworld, 1 ), 0 ), HullRole.Miner );
        }
        if ( _raidsLaunched == 0 && _raidTick == 0 && HasOperatingMine( ) ) {
            _raidTick = Tick + (ProspectYield( FirstMineBody( ) ) > 8 ? 840 : 1080);
            SendDispatch( FrontierEvent.Distress );
        }
        if ( _raidsLaunched == 0 && _raidTick != 0 && Tick >= _raidTick ) LaunchRaid( 2, FrontierEvent.Raid );
        if ( _raidsLaunched == 1 && AmberCount == 0 && WingsBuilt > 0 ) LaunchRaid( 3, FrontierEvent.Counterattack );
        if ( _raidsLaunched == 2 && AmberCount == 0 ) {
            _secured = true;
            SendDispatch( FrontierEvent.Secured );
        }
        if ( AmberCount == 0 || Tick % 90 != 0 ) return;

        var destination = YardBody >= 0 ? Shipyard : FirstMinePosition( );
        for ( int i = 0; i < SquadCount; ++i ) {
            if ( IsPlayerSquad( i ) || ShipCount( i ) == 0 ) continue;
            int target = -1;
            float distance = 24 * 24;
            for ( int ship = 0; ship < Count; ++ship ) {
                if ( !Alive( ship ) || SideOf( ship ) != 0 ) continue;
                float d = Vector2.DistanceSquared( _positions[ship], Center( i ) );
                if ( d >= distance ) continue;
                distance = d;
                target = ship;
            }
            if ( target >= 0 ) Attack( new( i ), target );
            else Move( i, destination );
        }
    }

    private bool HasOperatingMine( ) => FirstMineBody( ) != 0;

    private int FirstMineBody( ) {
        for ( int i = 1; i < _mines.Length; ++i )
            if ( _mines[i] > 0 && _mines[i] <= Tick ) return i;
        return 0;
    }

    private Vector2 FirstMinePosition( ) {
        int body = FirstMineBody( );
        return body == 0 ? _bodies[Homeworld].Center : _bodies[body].Center + new Vector2( OrbitRadius( body, 1 ), 0 );
    }

    private void LaunchRaid( int count, FrontierEvent incident ) {
        var target = YardBody >= 0 ? Shipyard : FirstMinePosition( );
        var origin = target + new Vector2( target.X < 0 ? 48 : -48, target.Y < 0 ? 35 : -35 );
        int squad = SquadCount;
        var design = new ShipDesign( incident == FrontierEvent.Raid ? WeaponKind.Pulse : WeaponKind.Rail, false );
        AddSquad( count, 1, Vector2.Clamp( origin, new( -BoardRadius + 5 ), new( BoardRadius - 5 ) ), design: design );
        Move( squad, target );
        ++_raidsLaunched;
        SendDispatch( incident );
    }

    private void SendDispatch( FrontierEvent incident ) => _dispatches[DispatchCount++] = incident;
}
