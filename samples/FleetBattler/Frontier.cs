using System.Numerics;

namespace FleetBattler;

internal enum HullRole : byte { Corvette, Scout, Miner, Freighter }
internal enum WeaponKind : byte { None, Pulse, Rail }
internal enum FrontierEvent : byte { Arrival, Prospect, Distress, Raid, Counterattack, Secured, MerchantOffer }
internal sealed partial class FleetSimulation {
    internal const int Homeworld = 1;
    // A one-survivor wing can retain three storage slots while consuming one naval slot.
    private const int FrontierCapacity = 320, MaxWorkers = 5;
    private readonly FrontierEvent[] _dispatches = new FrontierEvent[8];
    private int _raidTick, _raidsLaunched, _deposits;
    private int _surveyedProspects;
    private bool _secured;
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
        if ( Won ) return;
        if ( _raidsLaunched == 0 && _raidTick == 0 && HasOperatingMine( ) ) {
            _raidTick = Tick + (ProspectYield( FirstMineBody( ) ) > 8 ? 840 : 1080);
            SendDispatch( FrontierEvent.Distress );
        }
        if ( _raidsLaunched == 0 && _raidTick != 0 && Tick >= _raidTick ) LaunchRaid( 2, FrontierEvent.Raid );
        if ( _raidsLaunched == 1 && AmberCount == 0 && CorvettesLaunched > 0 ) LaunchRaid( 3, FrontierEvent.Counterattack );
        if ( _raidsLaunched == 2 && AmberCount == 0 ) {
            _secured = true;
            SendDispatch( FrontierEvent.Secured );
        }
        if ( AmberCount == 0 || Tick % 90 != 0 ) return;

        var destination = YardBody >= 0 ? Shipyard : FirstMinePosition( );
        for ( int i = 0; i < SquadCount; ++i ) {
            if ( !IsHostile( (int)FactionId.Sol, _squads[i].Faction ) || ShipCount( i ) == 0 ) continue;
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
        return body == 0 ? _bodies[Homeworld].Center : SitePositionAt( body, FitsBody( body, PlanetFacility.Mine ) ? PlanetFacility.Mine : PlanetFacility.Skimmer, Tick, _bodies[body].Center );
    }

    private void LaunchRaid( int count, FrontierEvent incident ) {
        var target = YardBody >= 0 ? Shipyard : FirstMinePosition( );
        var origin = target + new Vector2( target.X < 0 ? 48 : -48, target.Y < 0 ? 35 : -35 );
        var design = new ShipDesign( incident == FrontierEvent.Raid ? WeaponKind.Pulse : WeaponKind.Rail, false );
        int faction = (int)(incident == FrontierEvent.Raid ? FactionId.Raiders : FactionId.Rival);
        int squad = AddSquad( count, faction, Vector2.Clamp( origin, new( -BoardRadius + 5 ), new( BoardRadius - 5 ) ), design: design );
        Move( squad, target );
        ++_raidsLaunched;
        SendDispatch( incident );
    }

    private void SendDispatch( FrontierEvent incident ) => _dispatches[DispatchCount++] = incident;
}
