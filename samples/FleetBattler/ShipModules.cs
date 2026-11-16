using System.Numerics;

namespace FleetBattler;

internal enum ShieldModule { Standard, Reinforced, None }
internal enum ComputerModule { General, Targeting, RapidCycle }
internal enum ReactorModule { Standard, HighOutput }
internal enum DriveModule { Cruise, HighThrust, LongRange }
internal enum ModuleSlot { Weapon, Armor, Shield, Computer, Power, Drive, Count }

// Fold flat bonuses before scaling. Combine sources without allocating modifier lists.
internal readonly record struct StatModifier<T>( T Flat, T Scale ) where T : INumber<T> {
    internal static StatModifier<T> FlatBonus( T amount ) => new( amount, T.One );
    internal static StatModifier<T> Multiplier( T factor ) => new( T.Zero, factor );
    public static StatModifier<T> operator +( StatModifier<T> left, StatModifier<T> right ) =>
        new( left.Flat + right.Flat, left.Scale * right.Scale );
    internal T Apply( T basis ) => (basis + Flat) * Scale;
}

internal readonly record struct ShipDesign( WeaponKind Weapon, bool Plating, ShieldModule Shielding = ShieldModule.Standard,
    ComputerModule Computer = ComputerModule.General, ReactorModule Reactor = ReactorModule.Standard, DriveModule Drive = DriveModule.Cruise ) {
    private ModuleDefinition WeaponStats => GameCatalog.Module( ModuleSlot.Weapon, Weapon == WeaponKind.Rail ? 1 : 0 );
    private ModuleDefinition ArmorStats => GameCatalog.Module( ModuleSlot.Armor, Plating ? 1 : 0 );
    private ModuleDefinition ShieldStats => GameCatalog.Module( ModuleSlot.Shield, (int)Shielding );
    private ModuleDefinition ComputerStats => GameCatalog.Module( ModuleSlot.Computer, (int)Computer );
    private ModuleDefinition PowerStats => GameCatalog.Module( ModuleSlot.Power, (int)Reactor );
    private ModuleDefinition DriveStats => GameCatalog.Module( ModuleSlot.Drive, (int)Drive );

    internal int Shield => ShieldStats.Health;
    internal int Armor => ArmorStats.Health;
    internal int Range => WeaponStats.Range + ComputerStats.Range;
    internal int Reload => Math.Max( 8, WeaponStats.Reload + ComputerStats.Reload );
    internal int Damage => WeaponStats.Damage;
    internal int PowerAvailable => PowerStats.Supply;
    internal int PowerUsed => WeaponStats.Draw + ArmorStats.Draw + ShieldStats.Draw + ComputerStats.Draw + DriveStats.Draw;
    internal bool Valid => PowerUsed <= PowerAvailable;
    internal int ExtraOre => WeaponStats.Ore + ArmorStats.Ore + ShieldStats.Ore + ComputerStats.Ore + PowerStats.Ore + DriveStats.Ore;
    internal int ExtraEnergy => WeaponStats.Energy + ArmorStats.Energy + ShieldStats.Energy + ComputerStats.Energy + PowerStats.Energy + DriveStats.Energy;
    internal int ExtraTicks => WeaponStats.Ticks + ArmorStats.Ticks + ShieldStats.Ticks + ComputerStats.Ticks + PowerStats.Ticks + DriveStats.Ticks;

    internal float Handling( ) => ArmorStats.Handling * DriveStats.Handling;
    internal float Reach => DriveStats.Reach;
    internal int Variant( ModuleSlot slot ) => slot switch {
        ModuleSlot.Weapon => Weapon == WeaponKind.Rail ? 1 : 0, ModuleSlot.Armor => Plating ? 1 : 0,
        ModuleSlot.Shield => (int)Shielding, ModuleSlot.Computer => (int)Computer, ModuleSlot.Power => (int)Reactor,
        ModuleSlot.Drive => (int)Drive, _ => throw new ArgumentOutOfRangeException( nameof(slot) )
    };
    internal ShipDesign Install( ModuleSlot slot, int option ) => slot switch {
        ModuleSlot.Weapon => this with { Weapon = option == 0 ? WeaponKind.Pulse : WeaponKind.Rail },
        ModuleSlot.Armor => this with { Plating = option != 0 }, ModuleSlot.Shield => this with { Shielding = (ShieldModule)option },
        ModuleSlot.Computer => this with { Computer = (ComputerModule)option }, ModuleSlot.Power => this with { Reactor = (ReactorModule)option },
        ModuleSlot.Drive => this with { Drive = (DriveModule)option }, _ => throw new ArgumentOutOfRangeException( nameof(slot) )
    };

    internal ShipDesign Cycle( ModuleSlot slot ) => slot switch {
        ModuleSlot.Weapon => this with { Weapon = Weapon == WeaponKind.Rail ? WeaponKind.Pulse : WeaponKind.Rail },
        ModuleSlot.Armor => this with { Plating = !Plating },
        ModuleSlot.Shield => this with { Shielding = (ShieldModule)(((int)Shielding + 1) % 3) },
        ModuleSlot.Computer => this with { Computer = (ComputerModule)(((int)Computer + 1) % 3) },
        ModuleSlot.Power => this with { Reactor = Reactor == ReactorModule.Standard ? ReactorModule.HighOutput : ReactorModule.Standard },
        ModuleSlot.Drive => this with { Drive = (DriveModule)(((int)Drive + 1) % 3) },
        _ => throw new ArgumentOutOfRangeException( nameof(slot) )
    };

    internal string Installed( ModuleSlot slot ) => GameCatalog.Module( slot, Variant( slot ) ).Name;
}

internal readonly record struct FlightStats( float Cruise, float Acceleration, float TurnRate, float Range );

internal sealed partial class FleetSimulation {
    internal void CycleModule( ModuleSlot slot, bool frigate = false ) {
        if ( frigate ) FrigateDesign = FrigateDesign.Cycle( slot );
        else Design = Design.Cycle( slot );
    }
    internal FlightStats Flight( int squad ) => Flight( Role( squad ), _squads[squad].Design, DriveUpgrade && IsPlayerSquad( squad ) );

    internal static FlightStats Flight( HullRole role, ShipDesign design, bool improved ) {
        var baseline = GameCatalog.Hull( role ).Flight;
        float handling = IsWarship( role ) ? design.Handling( ) : 1;
        float range = IsWarship( role ) ? design.Reach : 1;
        var propulsion = StatModifier<float>.Multiplier( handling ) +
            StatModifier<float>.Multiplier( improved ? 1.18f : 1f );
        return new( propulsion.Apply( baseline.Cruise ),
            (propulsion + StatModifier<float>.FlatBonus( improved ? 12f : 0f )).Apply( baseline.Acceleration ),
            baseline.TurnRate * handling, range );
    }
}
