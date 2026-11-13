using System.Numerics;

namespace FleetBattler;

internal enum HullRole : byte { Corvette, Scout, Miner, Freighter, Colony, Frigate }
internal enum WeaponKind : byte { None, Pulse, Rail }
internal enum FrontierEvent : byte { Arrival, Prospect, Distress, Raid, Counterattack, Secured, MerchantOffer, Interdiction }
internal sealed partial class FleetSimulation {
    internal const int Homeworld = 1;
    // A one-survivor wing can retain three storage slots while consuming one naval slot.
    private const int FrontierCapacity = 320, MaxWorkers = 5;
    private readonly FrontierEvent[] _dispatches = new FrontierEvent[9];
    private int _raidTick, _raidsLaunched, _deposits;
    private int _surveyedProspects;
    private bool _secured;
    private int _nextPatrol;
    internal const int RaidPreparation = 2400, PatrolInterval = 4200;
    internal string Objective => !Solar ? !ExpeditionArrived ? "NO SOL SENSOR COVERAGE" : ReceiverSecured ? "FIRST CROSSING COMPLETE"
        : ReceiverOnline ? "DEFEND THE PROXIMA RECEIVER" : "SURVEY RECEIVER WORLD / BUILD AN ORBITAL LAB"
        : RaidWarningTicks > 0 ? "PREPARE ESCORTS FOR THE INBOUND RAID"
        : AmberCount > 0 ? "PROTECT EXTRACTION AND INTERCEPT HOSTILES"
        : ColoniesFounded == 0 && _secured ? "SUPPLY AND SETTLE A WORLD"
        : FusionUpgrade ? "SELECT EARTH ORBIT FLEET / OPEN GALAXY MAP"
        : _secured ? "RESEARCH INTERSTELLAR PROPULSION"
        : !HasOperatingMine( ) ? "SURVEY A BODY / BUILD AN EXTRACTION SITE" : "BUILD A YARD AND EQUIP AN ESCORT";
    internal int RaidWarningTicks => _raidsLaunched == 0 ? Math.Max( 0, _raidTick - Tick ) : 0;
    internal bool PaidPatrol { get; private set; }

    internal bool FundPatrol( ) {
        if ( RaidWarningTicks == 0 || PaidPatrol || Energy < 15 ) return false;
        Spend( new( 0, 15 ) );
        PaidPatrol = true;
        _raidTick += 900;
        return true;
    }
    internal bool Frontier { get; }
    internal int DispatchCount { get; private set; }
    internal int Deposits => _deposits;
    internal int ActiveRaids => _raidsLaunched;
    internal bool Recovering => Established && CyanCount == 0 && !Won;
    private int ActiveScouts( ) {
        int count = 0;
        for ( int i = 0; i < SquadCount; ++i )
            if ( IsScoutSquad( i ) ) count += ShipCount( i );
        return count;
    }
    internal ShipDesign Design { get; private set; } = new( WeaponKind.Pulse, false );
    internal HullRole Role( int squad ) => _squads[squad].Role;
    internal ShipDesign Loadout( int ship ) => _squads[_squadOfShip[ship]].Design;
    internal FrontierEvent Dispatch( int index ) => _dispatches[index];
    internal bool HasDeposit( int body ) => Frontier && (_deposits & (1 << body)) != 0;
    internal int DepositYield( int body ) => HasDeposit( body ) ? ProspectYield( body ) : 0;
    private int ProspectYield( int body ) => body == CelestialSystem.OuterBelt ? 20 : (body + SystemIndex) % 2 == 0 ? 12 : 8;

    internal void SetWeapon( WeaponKind weapon ) => Design = Design with { Weapon = weapon };
    internal void SetPlating( bool plating ) => Design = Design with { Plating = plating };

    private void OpenFrontier( ) {
        _survey[Homeworld] = SurveyRequired;
        _population[Homeworld] = 8;
        _nextGrowth[Homeworld] = ColonyGrowthInterval;
        Ore = 50;
        Energy = 25;
        Vector2 home = _bodies[Homeworld].Center + new Vector2( -OrbitRadius( Homeworld, 1 ), 0 );
        Park( AddSquad( 1, 0, home, HullRole.Scout ), Homeworld );
        Park( AddSquad( 1, 0, home + new Vector2( -5, 5 ), HullRole.Miner ), Homeworld );
        Park( AddSquad( 1, 0, home + new Vector2( -5, -5 ), HullRole.Miner ), Homeworld );
        SendDispatch( FrontierEvent.Arrival );
    }

    private void RevealProspect( int body ) {
        ++_surveyedProspects;
        _deposits |= 1 << body;
        if ( _surveyedProspects == 1 ) {
            SendDispatch( FrontierEvent.Prospect );
            SendDispatch( FrontierEvent.MerchantOffer );
        }
    }

    private void AdvanceFrontier( ) {
        if ( !Established ) return;
        if ( Tick % IncomeInterval == 0 && ShipsLost > 0 && Ore < MineCost && !HasOperatingMine( ) )
            Ore = Math.Min( MineCost, Ore + 2 );
        AdvancePiracy( );
        if ( !_battle ) return;
        if ( _raidsLaunched == 0 && _raidTick == 0 && HasOperatingMine( ) ) {
            _raidTick = Tick + RaidPreparation;
            SendDispatch( FrontierEvent.Distress );
        }
        if ( _raidsLaunched == 0 && _raidTick != 0 && Tick >= _raidTick ) LaunchRaid( PaidPatrol ? 1 : 2, FrontierEvent.Raid );
        if ( _raidsLaunched == 1 && AmberCount == 0 && CorvettesLaunched > 0 ) LaunchRaid( 3, FrontierEvent.Counterattack );
        if ( !_secured && _raidsLaunched == 2 && AmberCount == 0 ) {
            _secured = true;
            _nextPatrol = Tick + PatrolInterval;
            SendDispatch( FrontierEvent.Secured );
        }
        if ( _secured && AmberCount == 0 && Tick >= _nextPatrol && HasOperatingMine( ) ) {
            LaunchRaid( 2, FrontierEvent.Interdiction );
            _nextPatrol = Tick + PatrolInterval;
        }
        if ( AmberCount == 0 || Tick % 90 != 0 ) return;

        for ( int i = 0; i < SquadCount; ++i ) {
            if ( !IsHostile( (int)FactionId.Sol, _squads[i].Faction ) || ShipCount( i ) == 0 ) continue;
            bool pirate = _squads[i].Faction == (int)FactionId.Raiders;
            var destination = pirate || YardBody < 0 ? FirstMinePosition( ) : Shipyard;
            int target = -1;
            float distance = 24 * 24;
            for ( int ship = 0; ship < Count; ++ship ) {
                if ( !Alive( ship ) || SideOf( ship ) != 0 ||
                     pirate && !IsWarship( Role( SquadronOf( ship ) ) ) ) continue;
                float d = Vector2.DistanceSquared( _positions[ship], Center( i ) );
                if ( d >= distance ) continue;
                distance = d;
                target = ship;
            }
            if ( target >= 0 ) Attack( new( i ), target );
            else if ( _squads[i].Order.Enemy != 0 || Vector2.DistanceSquared( _squads[i].Order.Target, destination ) > 100 ) Move( i, destination );
        }
    }

    private bool HasOperatingMine( ) => FirstMineBody( ) != 0;

    private int FirstMineBody( ) {
        for ( int i = 1; i < _mines.Length; ++i )
            if ( ExtractionRemaining( i ) == 0 ) return i;
        return 0;
    }

    private Vector2 FirstMinePosition( ) {
        int body = FirstMineBody( );
        return body == 0 ? _bodies[Homeworld].Center : ExtractionPosition( body );
    }

    private void LaunchRaid( int count, FrontierEvent incident ) {
        var target = incident != FrontierEvent.Counterattack || YardBody < 0 ? FirstMinePosition( ) : Shipyard;
        var origin = target + new Vector2( target.X < 0 ? 48 : -48, target.Y < 0 ? 35 : -35 );
        var design = new ShipDesign( incident == FrontierEvent.Counterattack ? WeaponKind.Rail : WeaponKind.Pulse, false );
        int faction = (int)(incident == FrontierEvent.Counterattack ? FactionId.Rival : FactionId.Raiders);
        int squad = AddSquad( count, faction, Vector2.Clamp( origin, new( -NavigationRadius + 5 ), new( NavigationRadius - 5 ) ), design: design );
        Move( squad, target );
        ++_raidsLaunched;
        if ( incident != FrontierEvent.Interdiction || _raidsLaunched == 3 ) SendDispatch( incident );
    }

    private void SendDispatch( FrontierEvent incident ) => _dispatches[DispatchCount++] = incident;
}
