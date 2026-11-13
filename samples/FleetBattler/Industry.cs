using System.Numerics;

namespace FleetBattler;

internal readonly record struct ResourceCost( int Ore, int Energy, int Science = 0 );
internal enum ResearchTopic { Industry, Weapons, Drive, Habitation, Astrometry, Escorts, Fusion, Count }
internal readonly record struct Technology( string Name, string Benefit, ResourceCost Cost, int Duration, ResearchTopic? Requires = null );
internal readonly record struct HullPlan( string Name, HullRole Role, ResourceCost Cost, int Duration );
internal enum HullBlueprint { Scout, Miner, Corvette, Colony, Frigate, Count }
internal readonly record struct ShipyardOrder( HullBlueprint Hull, ShipDesign Design, ResourceCost Cost, int Duration );
internal readonly record struct YardLaunch( int Body, int Ship, HullBlueprint Hull );

internal sealed partial class FleetSimulation {
    internal const int YardQueueLimit = 4;
    internal const int SolarOutput = 8;
    private const int EarthOutput = 3;

    private readonly ShipyardOrder[] _shipOrdersAtYards = new ShipyardOrder[CelestialSystem.BodyCount * YardQueueLimit];
    private readonly int[] _yardQueueSizes = new int[CelestialSystem.BodyCount], _yardShipReady = new int[CelestialSystem.BodyCount];
    private int _navalReserved, _minersReserved, _scoutsReserved, _colonyReserved;
    private int _researchWork;
    internal int Energy { get; private set; }
    internal bool Established => Frontier && Solar;
    internal ResearchTopic? ActiveResearch { get; private set; }
    internal int ResearchRate => ScienceIncome == 0 ? 0 : Math.Max( 1, (ScienceIncome + 5) / 6 );
    internal int ResearchRemaining => ResearchRate == 0 || ActiveResearch is null ? 0 : (_researchWork + ResearchRate - 1) / ResearchRate;
    internal float ResearchProgress => ActiveResearch is { } topic ? 1 - (float)_researchWork / Tech( topic ).Duration : 0;
    internal bool HabitationUpgrade { get; private set; }
    internal bool AstrometryUpgrade { get; private set; }
    internal bool EscortsUpgrade { get; private set; }
    internal bool FusionUpgrade { get; private set; }
    internal ResearchTopic LastTechnology { get; private set; }
    internal int CompletedResearch { get; private set; }
    internal YardLaunch LastLaunch { get; private set; }
    internal int ShipsLaunched { get; private set; }
    internal int CorvettesLaunched { get; private set; }
    internal int EnergyIncome => (Established ? EarthOutput : 0) + Online( _solarArrays ) * SolarOutput;
    internal int ScienceIncome => Online( _labs ) * 6 + Online( _observatories ) * 10;
    internal ResourceCost ResourceCapacity {
        get {
            if ( !Frontier ) return new( 999, 999, 999 );
            int ground = Online( _surfaceStores ), orbit = Online( _orbitalStores );
            return new( 400 + 250 * ground + 180 * orbit, 400 + 100 * ground + 250 * orbit, 400 + 50 * ground + 50 * orbit );
        }
    }

    internal int OreIncome {
        get {
            int sum = 0;
            for ( int body = 1; body < _bodies.Length; ++body ) sum += ExtractionIncome( body, Tick );
            return sum;
        }
    }

    internal int ExtractionIncome( int body, int tick ) {
        int sum = _mines[body] > 0 && _mines[body] <= tick ? Frontier ? DepositYield( body ) : 8 : 0;
        if ( _skimmers[body] > 0 && _skimmers[body] <= tick ) sum += OrbitalExtractionYield( body );
        return sum;
    }

    internal int OrbitalExtractionYield( int body ) => Colonizable( body ) ? DepositYield( body ) / 2 : DepositYield( body );

    internal static Technology Tech( ResearchTopic topic ) => topic switch {
        ResearchTopic.Industry => new( "ORBITAL FABRICATION", "LOWER CORVETTE ORE AND BUILD TIME", new( 0, 12, IndustryCost ), 960 ),
        ResearchTopic.Weapons => new( "FIRE CONTROL", "WARSHIP DAMAGE AND RANGE", new( 0, 16, UpgradeCost ), 1500, ResearchTopic.Industry ),
        ResearchTopic.Drive => new( "LONG BURN DRIVE", "REACH THE OUTER SOLAR SYSTEM", new( 0, 20, DriveCost ), 1680 ),
        ResearchTopic.Habitation => new( "HABITAT ENGINEERING", "BUILD O NEILL HABITATS", new( 0, 26, 60 ), 2100, ResearchTopic.Industry ),
        ResearchTopic.Astrometry => new( "DEEP SKY ASTROMETRY", "CHART STARS / SCAN FASTER", new( 0, 18, 48 ), 1740, ResearchTopic.Drive ),
        ResearchTopic.Escorts => new( "FLEET ESCORT SYSTEMS", "BUILD MODULAR FRIGATES", new( 20, 24, 52 ), 1920, ResearchTopic.Weapons ),
        ResearchTopic.Fusion => new( "PULSED FUSION", "OPEN EXPEDITION TO PROXIMA", new( 32, 32, 72 ), 2400, ResearchTopic.Astrometry ),
        _ => throw new ArgumentOutOfRangeException( nameof(topic) )
    };

    internal bool Researched( ResearchTopic topic ) => topic switch {
        ResearchTopic.Industry => IndustryUpgrade, ResearchTopic.Weapons => WeaponUpgrade, ResearchTopic.Drive => DriveUpgrade,
        ResearchTopic.Habitation => HabitationUpgrade, ResearchTopic.Astrometry => AstrometryUpgrade,
        ResearchTopic.Escorts => EscortsUpgrade, ResearchTopic.Fusion => FusionUpgrade, _ => false
    };

    internal string? ResearchBlock( ResearchTopic topic ) {
        if ( !Frontier || (uint)topic >= (uint)ResearchTopic.Count ) return "PROJECT UNAVAILABLE";
        var tech = Tech( topic );
        if ( Researched( topic ) ) return "COMPLETE";
        if ( tech.Requires is { } prerequisite && !Researched( prerequisite ) ) return Tech( prerequisite ).Name;
        if ( topic == ResearchTopic.Fusion && !HabitationUpgrade ) return Tech( ResearchTopic.Habitation ).Name;
        if ( ActiveResearch is not null ) return "ANOTHER PROJECT ACTIVE";
        if ( ScienceIncome == 0 ) return "BUILD A LAB";
        return CanAfford( tech.Cost ) ? null : "INSUFFICIENT RESOURCES";
    }

    internal bool CanResearch( ResearchTopic topic ) => ResearchBlock( topic ) is null;

    internal bool StartResearch( ResearchTopic topic ) {
        if ( !CanResearch( topic ) ) return false;
        var tech = Tech( topic );
        Spend( tech.Cost );
        ActiveResearch = topic;
        _researchWork = tech.Duration;
        return true;
    }

    internal bool CancelResearch( ) {
        if ( ActiveResearch is not { } topic ) return false;
        Refund( Tech( topic ).Cost );
        ActiveResearch = null;
        _researchWork = 0;
        return true;
    }

    private void AdvanceIndustry( ) {
        if ( !Frontier ) return;
        if ( (Tick + 1) % IncomeInterval == 0 ) Energy = Math.Min( ResourceCapacity.Energy, Energy + EnergyIncome );
        AdvanceContracts( );
        if ( ActiveResearch is { } topic && (_researchWork -= ResearchRate) <= 0 ) {
            switch ( topic ) {
                case ResearchTopic.Industry: IndustryUpgrade = true; break;
                case ResearchTopic.Weapons: WeaponUpgrade = true; break;
                case ResearchTopic.Drive: DriveUpgrade = true; break;
                case ResearchTopic.Habitation: HabitationUpgrade = true; break;
                case ResearchTopic.Astrometry: AstrometryUpgrade = true; break;
                case ResearchTopic.Escorts: EscortsUpgrade = true; break;
                case ResearchTopic.Fusion: FusionUpgrade = true; break;
            }
            ActiveResearch = null;
            _researchWork = 0;
            LastTechnology = topic;
            ++CompletedResearch;
        }
        for ( int body = 1; body < _bodies.Length; ++body ) {
            if ( _yardQueueSizes[body] == 0 || _yardShipReady[body] > Tick + 1 ) continue;
            var order = YardQueue( body )[0];
            var position = YardPosition( body, Tick + 1 );
            var radial = Vector2.Normalize( position - _bodies[body].Center );
            var arrival = position + radial * (3.5f + ShipsLaunched % 3 * 2.5f);
            int squad = AddSquad( 1, (int)FactionId.Sol, arrival, Hull( order.Hull ).Role, order.Design );
            Park( squad, body, Tick + 1 );
            LastLaunch = new( body, _squads[squad].First, order.Hull );
            ++ShipsLaunched;
            if ( order.Hull == HullBlueprint.Corvette ) ++CorvettesLaunched;
            RemoveShipOrder( body, 0, Tick + 1 );
        }
    }

    internal ShipDesign FrigateDesign { get; private set; } = new( WeaponKind.Rail, true, Reactor: ReactorModule.HighOutput );
    internal ShipDesign Blueprint( HullBlueprint hull ) => hull == HullBlueprint.Frigate ? FrigateDesign : Design;
    internal static int NavalWeight( HullRole role ) => role == HullRole.Frigate ? 2 : role == HullRole.Corvette ? 1 : 0;
    internal static bool IsWarship( HullRole role ) => NavalWeight( role ) > 0;

    internal HullPlan Hull( HullBlueprint blueprint ) => blueprint switch {
        HullBlueprint.Colony => new( "COLONY SHIP", HullRole.Colony, new( 96, 35 ), 780 ),
        HullBlueprint.Scout => new( "SURVEY SCOUT", HullRole.Scout, new( 24, 6 ), 240 ),
        HullBlueprint.Miner => new( "MINER / CONSTRUCTOR", HullRole.Miner, new( 20, 5 ), 240 ),
        HullBlueprint.Corvette => new( "CORVETTE", HullRole.Corvette,
            new( (IndustryUpgrade ? 14 : 20) + Design.ExtraOre, 8 + Design.ExtraEnergy ),
            (IndustryUpgrade ? 100 : 160) + Design.ExtraTicks ),
        HullBlueprint.Frigate => new( "ESCORT FRIGATE", HullRole.Frigate,
            new( 48 + FrigateDesign.ExtraOre, 17 + FrigateDesign.ExtraEnergy ), 420 + FrigateDesign.ExtraTicks ),
        _ => throw new ArgumentOutOfRangeException( nameof(blueprint) )
    };

    internal bool CanOpenYard( int body ) => Frontier && (uint)body < (uint)_bodies.Length && body > 0 &&
        (YardAt( body ) == 0 || Established && body == Homeworld);
    internal ReadOnlySpan<ShipyardOrder> YardQueue( int body ) =>
        _shipOrdersAtYards.AsSpan( body * YardQueueLimit, _yardQueueSizes[body] );
    internal int ShipRemaining( int body ) => Remaining( _yardShipReady[body] );

    internal string? ShipOrderBlock( int body, HullBlueprint blueprint ) {
        if ( !CanOpenYard( body ) ) return "REQUIRES AN ONLINE SHIPYARD";
        if ( _yardQueueSizes[body] == YardQueueLimit ) return "QUEUE FULL";
        if ( (uint)blueprint >= (uint)HullBlueprint.Count ) return "UNKNOWN HULL";
        int weight = NavalWeight( Hull( blueprint ).Role );
        if ( weight > 0 ) {
            if ( YardAt( body ) != 0 ) return "REQUIRES AN ORBITAL SHIPYARD";
            if ( blueprint == HullBlueprint.Frigate && !EscortsUpgrade ) return "REQUIRES ESCORT SYSTEMS";
            if ( !Blueprint( blueprint ).Valid ) return "BLUEPRINT EXCEEDS REACTOR POWER";
            if ( NavalUsed + _navalReserved + weight > NavalCapacity ) return "NAVAL CAPACITY FULL";
        }
        if ( blueprint == HullBlueprint.Colony ) {
            if ( YardAt( body ) != 0 ) return "REQUIRES AN ORBITAL SHIPYARD";
            if ( ActiveColonyShips( ) + _colonyReserved >= 2 ) return "COLONY SHIP LIMIT REACHED";
        }
        if ( blueprint == HullBlueprint.Scout && ActiveScouts( ) + _scoutsReserved >= 2 ) return "SCOUT LIMIT REACHED";
        if ( blueprint == HullBlueprint.Miner && ActiveMiners( ) + _minersReserved >= MaxWorkers ) return "CONSTRUCTOR LIMIT REACHED";
        var cost = Hull( blueprint ).Cost;
        if ( Ore < cost.Ore ) return "INSUFFICIENT ORE";
        if ( Energy < cost.Energy ) return "INSUFFICIENT ENERGY";
        return null;
    }

    internal bool QueueShip( int body, HullBlueprint blueprint ) {
        if ( ShipOrderBlock( body, blueprint ) is not null ) return false;
        var hull = Hull( blueprint );
        int index = _yardQueueSizes[body]++;
        _shipOrdersAtYards[body * YardQueueLimit + index] = new( blueprint, Blueprint( blueprint ), hull.Cost, hull.Duration );
        Spend( hull.Cost );
        ReserveShip( blueprint, 1 );
        if ( index == 0 ) _yardShipReady[body] = Tick + hull.Duration;
        return true;
    }

    internal bool CancelShip( int body, int index ) {
        if ( !CanOpenYard( body ) || (uint)index >= (uint)_yardQueueSizes[body] ) return false;
        Refund( YardQueue( body )[index].Cost );
        RemoveShipOrder( body, index, Tick );
        return true;
    }

    private void RemoveShipOrder( int body, int index, int start ) {
        int offset = body * YardQueueLimit;
        ReserveShip( _shipOrdersAtYards[offset + index].Hull, -1 );
        int size = --_yardQueueSizes[body];
        _shipOrdersAtYards.AsSpan( offset + index + 1, size - index ).CopyTo( _shipOrdersAtYards.AsSpan( offset + index ) );
        if ( index == 0 ) _yardShipReady[body] = size == 0 ? 0 : start + _shipOrdersAtYards[offset].Duration;
    }

    private void ReserveShip( HullBlueprint hull, int delta ) {
        switch ( hull ) {
            case HullBlueprint.Scout: _scoutsReserved += delta; break;
            case HullBlueprint.Miner: _minersReserved += delta; break;
            case HullBlueprint.Corvette: _navalReserved += delta; break;
            case HullBlueprint.Frigate: _navalReserved += 2 * delta; break;
            case HullBlueprint.Colony: _colonyReserved += delta; break;
        }
    }

    internal bool CanAfford( ResourceCost cost ) => Ore >= cost.Ore && Energy >= cost.Energy && Science >= cost.Science;
    private void Spend( ResourceCost cost ) {
        Ore -= cost.Ore;
        Energy -= cost.Energy;
        Science -= cost.Science;
    }

    private void Refund( ResourceCost cost ) {
        var cap = ResourceCapacity;
        Ore = Math.Min( cap.Ore, Ore + cost.Ore );
        Energy = Math.Min( cap.Energy, Energy + cost.Energy );
        Science = Math.Min( cap.Science, Science + cost.Science );
    }
}
