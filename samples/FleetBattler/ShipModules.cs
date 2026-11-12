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
    internal int Shield => Shielding switch { ShieldModule.None => 0, ShieldModule.Reinforced => 52, _ => 34 };
    internal int Armor => StatModifier<int>.FlatBonus( Plating ? 22 : 0 ).Apply( 10 );
    internal int Range => StatModifier<int>.FlatBonus( Computer == ComputerModule.Targeting ? 4 : 0 ).Apply( Weapon == WeaponKind.Rail ? 21 : 16 );
    internal int Reload => StatModifier<int>.FlatBonus( Computer == ComputerModule.RapidCycle ? -8 : 0 ).Apply( Weapon == WeaponKind.Rail ? 64 : 38 );
    internal int Damage => Weapon == WeaponKind.Rail ? 22 : 13;
    internal int PowerAvailable => Reactor == ReactorModule.HighOutput ? 17 : 12;
    internal int PowerUsed => (Weapon == WeaponKind.Rail ? 4 : 3) +
        (Shielding switch { ShieldModule.None => 0, ShieldModule.Reinforced => 5, _ => 3 }) +
        (Computer == ComputerModule.General ? 1 : 3) + (Drive == DriveModule.HighThrust ? 5 : 3);
    internal bool Valid => PowerUsed <= PowerAvailable;
    internal int ExtraOre => (Plating ? 3 : 0) + (Shielding == ShieldModule.Reinforced ? 5 : 0) +
        (Computer == ComputerModule.General ? 0 : 3) + (Reactor == ReactorModule.HighOutput ? 5 : 0) + (Drive == DriveModule.Cruise ? 0 : 4);
    internal int ExtraEnergy => (Plating ? 2 : 0) + (Shielding == ShieldModule.Reinforced ? 3 : 0) +
        (Reactor == ReactorModule.HighOutput ? 4 : 0) + (Drive == DriveModule.HighThrust ? 2 : 0);
    internal int ExtraTicks => (Plating ? 20 : 0) + (Reactor == ReactorModule.HighOutput ? 30 : 0) + (Drive == DriveModule.Cruise ? 0 : 20);

    internal float Handling( ) {
        var mass = StatModifier<float>.Multiplier( Plating ? 0.84f : 1f );
        var thrust = StatModifier<float>.Multiplier( Drive == DriveModule.HighThrust ? 1.34f : Drive == DriveModule.LongRange ? 0.93f : 1f );
        return (mass + thrust).Apply( 1f );
    }

    internal ShipDesign Cycle( ModuleSlot slot ) => slot switch {
        ModuleSlot.Weapon => this with { Weapon = Weapon == WeaponKind.Rail ? WeaponKind.Pulse : WeaponKind.Rail },
        ModuleSlot.Armor => this with { Plating = !Plating },
        ModuleSlot.Shield => this with { Shielding = (ShieldModule)(((int)Shielding + 1) % 3) },
        ModuleSlot.Computer => this with { Computer = (ComputerModule)(((int)Computer + 1) % 3) },
        ModuleSlot.Power => this with { Reactor = Reactor == ReactorModule.Standard ? ReactorModule.HighOutput : ReactorModule.Standard },
        ModuleSlot.Drive => this with { Drive = (DriveModule)(((int)Drive + 1) % 3) },
        _ => throw new ArgumentOutOfRangeException( nameof(slot) )
    };

    internal string Installed( ModuleSlot slot ) => slot switch {
        ModuleSlot.Weapon => Weapon == WeaponKind.Rail ? "RAIL BATTERY" : "PULSE BATTERY",
        ModuleSlot.Armor => Plating ? "HEAVY PLATING" : "STANDARD PLATING",
        ModuleSlot.Shield => Shielding switch { ShieldModule.None => "EMPTY SLOT", ShieldModule.Reinforced => "REINFORCED FIELD", _ => "STANDARD FIELD" },
        ModuleSlot.Computer => Computer switch { ComputerModule.Targeting => "LONG RANGE TARGETING", ComputerModule.RapidCycle => "RAPID CYCLE", _ => "GENERAL PURPOSE" },
        ModuleSlot.Power => Reactor == ReactorModule.HighOutput ? "HIGH OUTPUT REACTOR" : "STANDARD REACTOR",
        ModuleSlot.Drive => Drive switch { DriveModule.HighThrust => "HIGH THRUST DRIVE", DriveModule.LongRange => "LONG RANGE DRIVE", _ => "CRUISE DRIVE" },
        _ => throw new ArgumentOutOfRangeException( nameof(slot) )
    };
}

internal readonly record struct FlightStats( float Cruise, float Acceleration, float TurnRate, float Range );

internal sealed partial class FleetSimulation {
    internal void CycleModule( ModuleSlot slot ) => Design = Design.Cycle( slot );
    internal FlightStats Flight( int squad ) => Flight( Role( squad ), _squads[squad].Design, DriveUpgrade && IsPlayerSquad( squad ) );

    internal static FlightStats Flight( HullRole role, ShipDesign design, bool improved ) {
        var baseline = role switch {
            HullRole.Scout => new FlightStats( 29, 61, 1, 1 ),
            HullRole.Miner => new FlightStats( 20, 44, 0.75f, 1 ),
            HullRole.Freighter => new FlightStats( 18, 39, 0.65f, 1 ),
            _ => new FlightStats( 25, 55, 1, 1 )
        };
        float handling = role == HullRole.Corvette ? design.Handling( ) : 1;
        float range = role == HullRole.Corvette && design.Drive == DriveModule.LongRange ? 1.2f : 1f;
        var propulsion = StatModifier<float>.Multiplier( handling ) +
            StatModifier<float>.Multiplier( improved ? 1.18f : 1f );
        return new( propulsion.Apply( baseline.Cruise ),
            (propulsion + StatModifier<float>.FlatBonus( improved ? 12f : 0f )).Apply( baseline.Acceleration ),
            baseline.TurnRate * handling, range );
    }
}
