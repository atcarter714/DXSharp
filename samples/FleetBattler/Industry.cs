using System.Numerics;

namespace FleetBattler;

internal readonly record struct ResourceCost( int Ore, int Energy, int Science = 0 );
internal enum ResearchTopic { Industry, Weapons, Drive, Habitation, Astrometry, Count }
internal readonly record struct Technology( string Name, string Benefit, ResourceCost Cost, int Duration );
internal readonly record struct HullPlan( string Name, HullRole Role, ResourceCost Cost, int Duration );
internal enum HullBlueprint { Scout, Miner, Corvette }
internal readonly record struct ShipyardOrder( HullBlueprint Hull, ShipDesign Design, ResourceCost Cost, int Duration );
internal readonly record struct YardLaunch( int Body, int Ship, HullBlueprint Hull );

internal sealed partial class FleetSimulation {
    internal const int YardQueueLimit = 4;
    internal const int SolarOutput = 8;
    private const int EarthOutput = 3;

    private readonly ShipyardOrder[] _shipOrdersAtYards = new ShipyardOrder[CelestialSystem.BodyCount * YardQueueLimit];
    private readonly int[] _yardQueueSizes = new int[CelestialSystem.BodyCount], _yardShipReady = new int[CelestialSystem.BodyCount];
    private int _navalReserved, _minersReserved, _scoutsReserved;
    private int _researchWork;
    internal int Energy { get; private set; }
    internal bool Established => Frontier && Solar;
    internal ResearchTopic? ActiveResearch { get; private set; }
    internal int ResearchRate => ScienceIncome == 0 ? 0 : Math.Max( 1, (ScienceIncome + 5) / 6 );
    internal int ResearchRemaining => ResearchRate == 0 || ActiveResearch is null ? 0 : (_researchWork + ResearchRate - 1) / ResearchRate;
    internal float ResearchProgress => ActiveResearch is { } topic ? 1 - (float)_researchWork / Tech( topic ).Duration : 0;
    internal bool HabitationUpgrade { get; private set; }
    internal bool AstrometryUpgrade { get; private set; }
    internal ResearchTopic LastTechnology { get; private set; }
    internal int CompletedResearch { get; private set; }
    internal YardLaunch LastLaunch { get; private set; }
    internal int ShipsLaunched { get; private set; }
    internal int CorvettesLaunched { get; private set; }
    internal int EnergyIncome => (Established ? EarthOutput : 0) + Online( _solarArrays ) * SolarOutput;
    internal int ScienceIncome => Online( _labs ) * 6 + Online( _observatories ) * 10;
    internal int OreIncome {
        get {
            int sum = 0;
            for ( int i = 1; i < _bodies.Length; ++i )
                if ( ExtractionRemaining( i ) == 0 ) sum += DepositYield( i );
            return sum;
        }
    }

    internal static Technology Tech( ResearchTopic topic ) => topic switch {
        ResearchTopic.Industry => new( "ORBITAL FABRICATION", "LOWER CORVETTE ORE AND BUILD TIME", new( 0, 12, IndustryCost ), 960 ),
        ResearchTopic.Weapons => new( "FIRE CONTROL", "+4 CORVETTE DAMAGE / +3 RANGE", new( 0, 16, UpgradeCost ), 1140 ),
        ResearchTopic.Drive => new( "LONG BURN DRIVE", "UNLOCK OUTER PLANETS AND KUIPER BELT", new( 0, 20, DriveCost ), 1320 ),
        ResearchTopic.Habitation => new( "HABITAT ENGINEERING", "UNLOCK O NEILL HABITATS", new( 0, 26, 60 ), 1680 ),
        ResearchTopic.Astrometry => new( "DEEP SKY ASTROMETRY", "SCOUTS SCAN 50 PCT FASTER", new( 0, 18, 48 ), 1200 ),
        _ => throw new ArgumentOutOfRangeException( nameof(topic) )
    };

    internal bool Researched( ResearchTopic topic ) => topic switch {
        ResearchTopic.Industry => IndustryUpgrade, ResearchTopic.Weapons => WeaponUpgrade, ResearchTopic.Drive => DriveUpgrade,
        ResearchTopic.Habitation => HabitationUpgrade, ResearchTopic.Astrometry => AstrometryUpgrade, _ => false
    };

    internal bool CanResearch( ResearchTopic topic ) => (uint)topic < (uint)ResearchTopic.Count && Frontier && ActiveResearch is null && !Researched( topic ) &&
        ScienceIncome > 0 && CanAfford( Tech( topic ).Cost );

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
        if ( (Tick + 1) % IncomeInterval == 0 ) Energy = Math.Min( 999, Energy + EnergyIncome );
        AdvanceContracts( );
        if ( ActiveResearch is { } topic && (_researchWork -= ResearchRate) <= 0 ) {
            switch ( topic ) {
                case ResearchTopic.Industry: IndustryUpgrade = true; break;
                case ResearchTopic.Weapons: WeaponUpgrade = true; break;
                case ResearchTopic.Drive: DriveUpgrade = true; break;
                case ResearchTopic.Habitation: HabitationUpgrade = true; break;
                case ResearchTopic.Astrometry: AstrometryUpgrade = true; break;
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

    internal HullPlan Hull( HullBlueprint blueprint ) => blueprint switch {
        HullBlueprint.Scout => new( "SURVEY SCOUT", HullRole.Scout, new( 24, 6 ), 240 ),
        HullBlueprint.Miner => new( "MINER / CONSTRUCTOR", HullRole.Miner, new( 20, 5 ), 240 ),
        HullBlueprint.Corvette => new( "CORVETTE", HullRole.Corvette,
            new( (IndustryUpgrade ? 14 : 20) + Design.ExtraOre, 8 + Design.ExtraEnergy ),
            (IndustryUpgrade ? 100 : 160) + Design.ExtraTicks ),
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
        if ( blueprint == HullBlueprint.Corvette ) {
            if ( YardAt( body ) != 0 ) return "REQUIRES AN ORBITAL SHIPYARD";
            if ( !Design.Valid ) return "BLUEPRINT EXCEEDS REACTOR POWER";
            if ( NavalUsed + _navalReserved >= NavalCapacity ) return "NAVAL CAPACITY FULL";
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
        _shipOrdersAtYards[body * YardQueueLimit + index] = new( blueprint, Design, hull.Cost, hull.Duration );
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
        }
    }

    internal bool CanAfford( ResourceCost cost ) => Ore >= cost.Ore && Energy >= cost.Energy && Science >= cost.Science;
    private void Spend( ResourceCost cost ) {
        Ore -= cost.Ore;
        Energy -= cost.Energy;
        Science -= cost.Science;
    }

    private void Refund( ResourceCost cost ) {
        Ore = Math.Min( 999, Ore + cost.Ore );
        Energy = Math.Min( 999, Energy + cost.Energy );
        Science = Math.Min( 999, Science + cost.Science );
    }
}
