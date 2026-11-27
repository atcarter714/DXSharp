using System.Numerics;

namespace FleetBattler;

internal readonly record struct ResourceCost( int Ore, int Energy, int Science = 0 );
internal enum ResearchTopic { Industry, Weapons, Drive, Habitation, Astrometry, Escorts, Fusion, FleetLogistics, HeavyIndustry,
    PrecisionSensors, WideSearch, HeavyFrames, CapitalFrames, GunneryDoctrine, ShieldDoctrine, SurfaceEngineering, OrbitalEngineering,
    ColdTraps, Radiators, AttitudeJets, Occultation, ParallelSlips, ThermalStores, ConvoyWindows, ClosedEcology, ClimateEngineering, Count }
internal readonly record struct HullPlan( string Name, HullRole Role, ResourceCost Cost, int Duration );
internal enum HullBlueprint { Scout, Miner, Corvette, Colony, Frigate, MiningShip, Destroyer, Cruiser, Battleship, Carrier, Titan, Count }
internal readonly record struct ShipyardOrder( HullBlueprint Hull, ShipDesign Design, ResourceCost Cost, int Duration );
internal readonly record struct YardLaunch( int Body, int Ship, HullBlueprint Hull );

internal sealed partial class FleetSimulation {
    internal const int YardQueueLimit = 4;
    private const int EarthOutput = 3;

    private readonly ShipyardOrder[] _shipOrdersAtYards = new ShipyardOrder[CelestialSystem.BodyCount * YardQueueLimit];
    private readonly int[] _yardQueueSizes = new int[CelestialSystem.BodyCount], _yardShipReady = new int[CelestialSystem.BodyCount];
    private int _navalReserved, _minersReserved, _scoutsReserved, _colonyReserved;
    private int _researchWork;
    internal CampaignFaction PlayerFaction { get; private set; } = new();

    internal void AttachFaction( CampaignFaction faction ) => PlayerFaction = faction;
    internal string FactionName( FactionId faction ) => faction == FactionId.Sol ? PlayerFaction.Name : Factions.Name( faction );

    internal int Energy { get; private set; }
    internal bool FrontierProgression { get; set; }
    private int EconomyInterval => FrontierProgression ? 720 : IncomeInterval;
    private int ScienceInterval => FrontierProgression ? EconomyInterval * 2 : EconomyInterval;
    internal bool Established => Frontier && Solar;
    internal ResearchTopic? ActiveResearch { get; private set; }
    internal int ResearchRate => ScienceIncome == 0 ? 0 : FrontierProgression ? Math.Clamp( 1 + ScienceIncome / 18, 1, 3 ) : Math.Max( 1, (ScienceIncome + 5) / 6 );
    internal int ResearchRemaining => ResearchRate == 0 || ActiveResearch is null ? 0 : (_researchWork + ResearchRate - 1) / ResearchRate;
    internal float ResearchProgress => ActiveResearch is { } topic ? 1 - (float)_researchWork / ResearchWork( Tech( topic ) ) : 0;
    internal bool HabitationUpgrade => Researched( ResearchTopic.Habitation );
    internal bool AstrometryUpgrade => Researched( ResearchTopic.Astrometry );
    internal int FireControlDamage => PlayerFaction.Damage + (WeaponUpgrade ? Tech( ResearchTopic.Weapons ).Damage : 0);
    internal int FireControlRange => PlayerFaction.Range + (WeaponUpgrade ? Tech( ResearchTopic.Weapons ).Range : 0) +
        (Researched( ResearchTopic.GunneryDoctrine ) ? Tech( ResearchTopic.GunneryDoctrine ).Range : 0);
    internal int ScoutScanRate => Math.Max( PlayerFaction.Scan, Researched( ResearchTopic.PrecisionSensors ) ? Tech( ResearchTopic.PrecisionSensors ).ScanRate
        : AstrometryUpgrade ? Tech( ResearchTopic.Astrometry ).ScanRate : 2 );
    internal bool EscortsUpgrade => Researched( ResearchTopic.Escorts );
    internal bool FusionUpgrade => Researched( ResearchTopic.Fusion );
    internal ResearchTopic LastTechnology { get; private set; }
    internal int CompletedResearch { get; private set; }
    internal YardLaunch LastLaunch { get; private set; }
    internal int ShipsLaunched { get; private set; }
    internal int CorvettesLaunched { get; private set; }
    internal int EnergyIncome => (Established ? EarthOutput : 0) + StructureOutput( PlanetFacility.Solar ) +
        StructureOutput( PlanetFacility.Soletta ) + (CharterEconomy == EconomicCharter.Power ? 1 : 0);
    internal int EnergyUpkeep => Math.Max( 0, -PlayerFaction.Upkeep + (NavalUsed + 1) / 2 +
        (Online( _labs ) + Online( _yards ) + Online( _depots ) + Online( _observatories )) / 3 -
        (Researched( ResearchTopic.FleetLogistics ) ? Tech( ResearchTopic.FleetLogistics ).UpkeepReduction : 0) -
        (Researched( ResearchTopic.ConvoyWindows ) ? Tech( ResearchTopic.ConvoyWindows ).UpkeepReduction : 0) );
    internal int ScienceIncome => StructureOutput( PlanetFacility.Lab ) + StructureOutput( PlanetFacility.Observatory );
    internal ResourceCost ResourceCapacity {
        get {
            if ( !Frontier ) return new( 999, 999, 999 );
            int ground = Online( _surfaceStores ), orbit = Online( _orbitalStores );
            return new( 400 + 250 * ground + 180 * orbit, 400 + 100 * ground + 250 * orbit + (Researched( ResearchTopic.ThermalStores ) ? 150 : 0), 400 + 50 * ground + 50 * orbit );
        }
    }

    internal int OreRoyalty { get; private set; }
    private int AfterRoyalty( int gross ) => gross - (gross * OreRoyalty + 99) / 100;

    internal int OreIncome {
        get {
            int sum = 0;
            for ( int body = 1; body < _bodies.Length; ++body ) sum += ExtractionIncome( body, Tick );
            return AfterRoyalty( sum );
        }
    }

    internal int ExtractionIncome( int body, int tick ) {
        int sum = _mines[body] > 0 && _mines[body] <= tick ? Frontier ? DepositYield( body ) : 8 : 0;
        if ( _mines[body] > 0 && _mines[body] <= tick && Frontier )
            sum = Math.Max( 1, sum * Facility( PlanetFacility.Mine ).Output / 2 ) * SiteIntegrity( body, PlanetFacility.Mine ) / 100 +
                SiteUpgradeBonus( body, PlanetFacility.Mine ) * SiteIntegrity( body, PlanetFacility.Mine ) / 100;
        if ( _skimmers[body] > 0 && _skimmers[body] <= tick )
            sum += (Math.Max( 1, OrbitalExtractionYield( body ) * Facility( PlanetFacility.Skimmer ).Output / 2 ) +
                SiteUpgradeBonus( body, PlanetFacility.Skimmer )) * SiteIntegrity( body, PlanetFacility.Skimmer ) / 100;
        if ( sum > 0 && CharterEconomy == EconomicCharter.Extraction ) ++sum;
        if ( sum > 0 && Researched( ResearchTopic.HeavyIndustry ) ) sum += Tech( ResearchTopic.HeavyIndustry ).ExtractionBonus;
        if ( sum > 0 && Researched( ResearchTopic.ColdTraps ) ) sum += Tech( ResearchTopic.ColdTraps ).ExtractionBonus;
        return sum > 0 ? sum + PlayerFaction.Extraction : 0;
    }

    internal int OrbitalExtractionYield( int body ) => Colonizable( body ) ? DepositYield( body ) / 2 : DepositYield( body );

    internal static Technology Tech( ResearchTopic topic ) => ResearchCatalog.Current[topic];

    internal bool Researched( ResearchTopic topic ) => PlayerFaction.Knows( topic );

    internal string? ResearchBlock( ResearchTopic topic ) {
        if ( !Frontier || (uint)topic >= (uint)ResearchCatalog.Current.Count ) return "PROJECT UNAVAILABLE";
        var tech = Tech( topic );
        if ( Researched( topic ) ) return "COMPLETE";
        if ( tech.Exclusive is { } group ) {
            for ( int i = 0; i < ResearchCatalog.Current.Count; ++i ) {
                var other = (ResearchTopic)i;
                if ( other != topic && Researched( other ) && Tech( other ).Exclusive == group )
                    return $"SPECIALIZED IN {Tech( other ).Name}";
            }
        }
        foreach ( var prerequisite in tech.Requires )
            if ( !Researched( prerequisite ) ) return Tech( prerequisite ).Name;
        if ( PlayerFaction.ResearchSystem is { } researchSystem )
            return researchSystem == this ? "ANOTHER PROJECT ACTIVE" : $"RESEARCH ACTIVE IN {researchSystem.SystemName}";
        if ( ScienceIncome == 0 ) return "BUILD A LAB";
        return CanAfford( tech.Cost ) ? null : "INSUFFICIENT RESOURCES";
    }

    internal bool CanResearch( ResearchTopic topic ) => ResearchBlock( topic ) is null;

    internal bool StartResearch( ResearchTopic topic ) {
        if ( !CanResearch( topic ) ) return false;
        var tech = Tech( topic );
        Spend( tech.Cost );
        PlayerFaction.ResearchSystem = this;
        ActiveResearch = topic;
        _researchWork = ResearchWork( tech );
        return true;
    }

    private int ResearchWork( Technology tech ) => FrontierProgression ? tech.Duration * 8 : tech.Duration;

    internal bool CancelResearch( ) {
        if ( ActiveResearch is not { } topic ) return false;
        Refund( Tech( topic ).Cost );
        PlayerFaction.ResearchSystem = null;
        ActiveResearch = null;
        _researchWork = 0;
        return true;
    }

    private void AdvanceIndustry( ) {
        if ( !Frontier ) return;
        if ( (Tick + 1) % EconomyInterval == 0 ) {
            Energy = Math.Clamp( Energy + EnergyIncome - EnergyUpkeep, 0, ResourceCapacity.Energy );
            GatherBeltOre( );
            RestoreSites();
        }
        AdvanceContracts( );
        if ( ActiveResearch is { } topic && (_researchWork -= ResearchRate) <= 0 ) {
            PlayerFaction.Unlock( topic );
            PlayerFaction.ResearchSystem = null;
            ActiveResearch = null;
            _researchWork = 0;
            LastTechnology = topic;
            ++CompletedResearch;
        }
        for ( int body = 1; body < _bodies.Length; ++body ) {
            if ( _yardQueueSizes[body] == 0 ) continue;
            if ( SiteIntegrity( body, PlanetFacility.Yard ) < 50 ) {
                ++_yardShipReady[body];
                continue;
            }
            if ( _yardShipReady[body] > Tick + 1 ) continue;
            var order = YardQueue( body )[0];
            var position = YardPosition( body, Tick + 1 );
            var radial = Vector2.Normalize( position - _bodies[body].Center );
            var arrival = position + radial * (3.5f + ShipsLaunched % 3 * 2.5f);
            int squad = AddSquad( 1, (int)FactionId.Sol, arrival, Hull( order.Hull ).Role, order.Design );
            Park( squad, body, Tick + 1 );
            LastLaunch = new( body, _squads[squad].First, order.Hull );
            if ( order.Hull == HullBlueprint.MiningShip ) AssignBeltMiner( squad );
            ++ShipsLaunched;
            if ( order.Hull == HullBlueprint.Corvette ) ++CorvettesLaunched;
            RemoveShipOrder( body, 0, Tick + 1 );
        }
    }

    private static readonly ShipDesign HeavyTemplate = new( WeaponKind.Rail, true, Reactor: ReactorModule.HighOutput );
    internal ShipDesign FrigateDesign { get; private set; } = HeavyTemplate;
    private readonly ShipDesign?[] _heavyDesigns = new ShipDesign?[(int)HullBlueprint.Count];
    internal static ReadOnlySpan<HullBlueprint> WarshipBlueprints =>
        [HullBlueprint.Corvette, HullBlueprint.Frigate, HullBlueprint.Destroyer, HullBlueprint.Cruiser, HullBlueprint.Battleship, HullBlueprint.Carrier, HullBlueprint.Titan];

    internal ShipDesign Blueprint( HullBlueprint hull ) => hull switch {
        HullBlueprint.Corvette => Design,
        HullBlueprint.Frigate => FrigateDesign,
        >= HullBlueprint.Destroyer and < HullBlueprint.Count => _heavyDesigns[(int)hull] ?? HeavyTemplate,
        _ => default
    };

    internal bool SaveDesign( HullBlueprint hull, ShipDesign design ) {
        if ( !WarshipBlueprints.Contains( hull ) || !design.Valid ) return false;
        for ( int i = 0; i < (int)ModuleSlot.Count; ++i ) {
            var slot = (ModuleSlot)i;
            if ( GameCatalog.Module( slot, design.Variant( slot ) ).Requirement is { } requirement && !Researched( requirement ) )
                return false;
        }
        switch ( hull ) {
            case HullBlueprint.Corvette: Design = design; break;
            case HullBlueprint.Frigate: FrigateDesign = design; break;
            default: _heavyDesigns[(int)hull] = design; break;
        }
        return true;
    }

    internal static int NavalWeight( HullRole role ) => GameCatalog.Hull( role ).Naval;
    internal static bool IsWarship( HullRole role ) => NavalWeight( role ) > 0;
    internal static HullRole BlueprintRole( HullBlueprint blueprint ) => blueprint switch {
        HullBlueprint.Colony => HullRole.Colony, HullBlueprint.Scout => HullRole.Scout, HullBlueprint.Miner => HullRole.Miner,
        HullBlueprint.Corvette => HullRole.Corvette, HullBlueprint.Frigate => HullRole.Frigate, HullBlueprint.MiningShip => HullRole.Prospector,
        HullBlueprint.Destroyer => HullRole.Destroyer, HullBlueprint.Cruiser => HullRole.Cruiser, HullBlueprint.Battleship => HullRole.Battleship,
        HullBlueprint.Carrier => HullRole.Carrier, HullBlueprint.Titan => HullRole.Titan,
        _ => throw new ArgumentOutOfRangeException( nameof(blueprint) )
    };

    internal HullPlan Hull( HullBlueprint blueprint ) => Hull( blueprint, Blueprint( blueprint ) );

    internal HullPlan Hull( HullBlueprint blueprint, ShipDesign design ) {
        var role = BlueprintRole( blueprint );
        var definition = GameCatalog.Hull( role );
        if ( definition.Naval == 0 ) return new( definition.Name, role, definition.Cost, definition.Ticks );
        int ticks = Math.Max( 30, definition.Ticks - (IndustryUpgrade ? definition.ImprovedTicks : 0) ) + design.ExtraTicks;
        if ( Researched( ResearchTopic.ParallelSlips ) ) ticks = (ticks * 4 + 4) / 5;
        return new( definition.Name, role, new( Math.Max( 0, definition.Cost.Ore - (IndustryUpgrade ? definition.ImprovedOre : 0) ) + design.ExtraOre,
            definition.Cost.Energy + design.ExtraEnergy, definition.Cost.Science ),
            ticks );
    }

    private static bool MiningBelt( int system, int body ) => system == 0
        ? body is SolarCatalog.Ceres or SolarCatalog.Vesta or SolarCatalog.Pluto or CelestialSystem.OuterBelt
        : body == CelestialSystem.OuterBelt;

    private void AssignBeltMiner( int squad ) {
        var unit = new FleetSelection( squad );
        int closest = -1;
        float distance = float.MaxValue;
        for ( int body = 1; body < _bodies.Length; ++body ) {
            if ( !MiningBelt( SystemIndex, body ) || !CanReach( unit, body ) ) continue;
            float candidate = Vector2.DistanceSquared( Center( unit ), _bodies[body].Center );
            if ( candidate < distance ) { distance = candidate; closest = body; }
        }
        if ( closest >= 0 ) Orbit( unit, closest );
    }

    private void GatherBeltOre( ) {
        int income = 0;
        for ( int squad = 0; squad < SquadCount; ++squad ) {
            if ( Role( squad ) != HullRole.Prospector || !IsPlayerSquad( squad ) ) continue;
            var order = _squads[squad].Order;
            if ( MiningBelt( SystemIndex, order.Body - 1 ) && order.Transfer.End <= Tick ) income += ShipCount( squad );
        }
        if ( income > 0 ) Ore = Math.Min( ResourceCapacity.Ore, Ore + income );
    }

    internal bool CanOpenYard( int body ) => Frontier && (uint)body < (uint)_bodies.Length && body > 0 &&
        (YardAt( body ) == 0 || Established && body == Homeworld);
    internal ReadOnlySpan<ShipyardOrder> YardQueue( int body ) =>
        _shipOrdersAtYards.AsSpan( body * YardQueueLimit, _yardQueueSizes[body] );
    internal int ShipRemaining( int body ) => Remaining( _yardShipReady[body] );

    internal string? ShipOrderBlock( int body, HullBlueprint blueprint ) {
        if ( !CanOpenYard( body ) ) return "REQUIRES AN ONLINE SHIPYARD";
        if ( _yardQueueSizes[body] == YardQueueLimit ) return "QUEUE FULL";
        if ( TravelBerths <= 0 ) return "NO FREE BERTHS";
        if ( (uint)blueprint >= (uint)HullBlueprint.Count ) return "UNKNOWN HULL";
        int weight = NavalWeight( Hull( blueprint ).Role );
        if ( weight > 0 ) {
            if ( YardAt( body ) != 0 ) return "REQUIRES AN ORBITAL SHIPYARD";
            if ( GameCatalog.Hull( BlueprintRole( blueprint ) ).Requirement is { } tech && !Researched( tech ) ) return $"REQUIRES {Tech( tech ).Name}";
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
            case HullBlueprint.Colony: _colonyReserved += delta; break;
            default: _navalReserved += NavalWeight( BlueprintRole( hull ) ) * delta; break;
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
