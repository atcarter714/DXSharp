using System.Numerics;

namespace FleetBattler;

internal enum CommandTab { Hidden, Economy, Fleet, Events }
internal enum CommandAction { Toggle, Economy, Fleet, Events, Worker, Scout, Drive, Wing, Weapon, Defense, Earlier, Later }

internal sealed partial class FleetHud {
    private static (float X, float Y) CommandOrigin( Layout layout ) => ((layout.Width - 285) / 2, 61);

    private void CommandPanel( Layout layout, FleetSimulation fleet, CommandTab tab, int dispatch, int speed ) {
        Button( layout.ClockX - 89, 19, 83, tab == CommandTab.Hidden ? "COMMAND" : "CLOSE TAB", true, tab != CommandTab.Hidden );
        if ( tab == CommandTab.Hidden ) return;
        var (x, y) = CommandOrigin( layout );
        _overlay = 1;
        PanelAt( x, y, 285, CommandHeight( tab ), SpaceDraw.Cyan );
        Button( x + 9, y + 9, 84, "ECONOMY", true, tab == CommandTab.Economy );
        Button( x + 100, y + 9, 84, "FLEET", true, tab == CommandTab.Fleet );
        Button( x + 191, y + 9, 84, "EVENTS", true, tab == CommandTab.Events );
        Span<char> label = stackalloc char[45];
        switch ( tab ) {
            case CommandTab.Economy:
                label.TryWrite( $"ORE {fleet.Ore:D3}  SCI {fleet.Science:D3}  MINERS {fleet.ActiveMiners( )}", out int length );
                Text( label[..length], x + 10, y + 43, 1.15f, White );
                label.TryWrite( $"HOMEWORLD / {CelestialSystem.Name( fleet.SystemIndex, FleetSimulation.Homeworld )}", out length );
                Text( label[..length], x + 10, y + 63, 1, Label );
                int rate = 0;
                for ( int body = 1; body < CelestialSystem.BodyCount; ++body )
                    if ( fleet.Planet( body ).MineRemaining == 0 ) rate += fleet.DepositYield( body );
                label.TryWrite( $"MINE OUTPUT {rate} ORE PER CYCLE", out length );
                Text( label[..length], x + 10, y + 81, 1, Label );
                if ( fleet.WorkerRemaining > 0 )
                    label.TryWrite( $"{(fleet.CivilianOrder == HullRole.Miner ? "MINER" : "SCOUT")} BUILDING / {Seconds( fleet.WorkerRemaining )} SEC", out length );
                else label.TryWrite( $"BUILD MINER / {FleetSimulation.WorkerCost} ORE", out length );
                Button( x + 9, y + 100, 267, label[..length], fleet.CanQueueWorker );
                if ( fleet.QueuedWings > 0 ) label.TryWrite( $"QUEUE {fleet.QueuedWings} / {Seconds( fleet.WingRemaining )} SEC LEFT", out length );
                else label.TryWrite( $"QUEUE WING / {fleet.CurrentWingCost} ORE", out length );
                Button( x + 9, y + 128, 267, label[..length], fleet.CanBuildWing );
                Button( x + 9, y + 155, 267, "REPLACE SCOUT / 24 ORE", fleet.CanQueueScout );
                Button( x + 9, y + 182, 267, fleet.DriveUpgrade ? "LONG BURN DRIVE ONLINE" : "RESEARCH LONG BURN DRIVE / 36 SCI",
                    !fleet.DriveUpgrade && fleet.Science >= FleetSimulation.DriveCost );
                break;
            case CommandTab.Fleet:
                var design = fleet.Design;
                label.TryWrite( $"CORVETTE WING / 3 HULLS / {fleet.CurrentWingCost} ORE", out length );
                Text( label[..length], x + 10, y + 43, 1.05f, White );
                label.TryWrite( $"HULL {FleetSimulation.HullCapacity} / SHIELD {design.Shield} / ARMOR {design.Armor}", out length );
                Text( label[..length], x + 10, y + 63, 1, Label );
                Button( x + 9, y + 83, 267, design.Weapon == WeaponKind.Rail ? "WEAPON / RAIL / BYPASSES SHIELDS" : "WEAPON / PULSE / FAST CYCLING", true );
                Button( x + 9, y + 110, 267, design.Plating ? "DEFENSE / HEAVY PLATING" : "DEFENSE / SHIELD GENERATOR", true );
                label.TryWrite( $"NAVAL {fleet.NavalUsed} USED / {fleet.NavalReserved} QUEUED / {fleet.NavalCapacity} MAX", out length );
                Text( label[..length], x + 10, y + 139, 1, Label );
                label.TryWrite( $"BUILD TIME {Seconds( fleet.CurrentWingDuration )} SEC AT {speed}X", out length );
                Text( label[..length], x + 10, y + 155, 1, Label );
                break;
            case CommandTab.Events:
                var incident = fleet.Dispatch( dispatch );
                var (title, first, second) = IncidentText( incident );
                Text( title, x + 10, y + 43, 1.25f, White );
                Text( first, x + 10, y + 71, 1, Label );
                Text( second, x + 10, y + 91, 1, Label );
                Button( x + 9, y + 125, 78, "EARLIER", dispatch > 0 );
                Button( x + 197, y + 125, 78, "LATER", dispatch + 1 < fleet.DispatchCount );
                label.TryWrite( $"{dispatch + 1}/{fleet.DispatchCount}", out length );
                Text( label[..length], x + 127, y + 132, 1.05f, Label );
                break;
        }
        _overlay = 0;

        int Seconds( int ticks ) => (int)Math.Ceiling( ticks * FleetSimulation.StepSeconds / (speed * 0.25) );
    }

    internal static string IncidentTitle( FrontierEvent incident ) => IncidentText( incident ).Title;

    private static (string Title, string First, string Second) IncidentText( FrontierEvent incident ) => incident switch {
        FrontierEvent.Arrival => ("FRONTIER CHARTER", "SURVEY A MOON OR GIANT WITH YOUR SCOUT", "SEND A MINER TO BUILD ON A DEPOSIT"),
        FrontierEvent.Prospect => ("ORE SIGNATURE CONFIRMED", "SELECT A MINER AND INSPECT THE BODY", "PLACE A MINE TO BEGIN EXTRACTION"),
        FrontierEvent.Distress => ("DISTRESS SIGNAL", "AMBER HAS NOTICED YOUR EXTRACTION", "BUILD A YARD AND KEEP WORKERS CLEAR"),
        FrontierEvent.Raid => ("AMBER RAID INBOUND", "CONTACTS ARE APPROACHING THE YARD", "MOVE CIVILIANS CLEAR AND DEPLOY ESCORTS"),
        FrontierEvent.Counterattack => ("AMBER COUNTERATTACK", "RAIL CORVETTES ARE APPROACHING", "RAIL SHOTS BYPASS SHIELDS"),
        FrontierEvent.Secured => ("SYSTEM SECURED", "THE RAIDERS HAVE BEEN DRIVEN OFF", "YOUR OUTPOST CAN EXPAND"),
        _ => throw new ArgumentOutOfRangeException( nameof(incident) )
    };

    private static float CommandHeight( CommandTab tab ) => tab == CommandTab.Economy ? 213 : 177;

    private static HudHit HitCommand( Layout layout, Vector2 p, CommandTab tab ) {
        if ( Contains( p, layout.ClockX - 89, 19, 83, 21 ) ) return Action( CommandAction.Toggle );
        if ( tab == CommandTab.Hidden ) return default;
        var (x, y) = CommandOrigin( layout );
        if ( !Contains( p, x, y, 285, CommandHeight( tab ) ) ) return default;
        if ( Contains( p, x + 9, y + 9, 84, 21 ) ) return Action( CommandAction.Economy );
        if ( Contains( p, x + 100, y + 9, 84, 21 ) ) return Action( CommandAction.Fleet );
        if ( Contains( p, x + 191, y + 9, 84, 21 ) ) return Action( CommandAction.Events );
        if ( tab == CommandTab.Economy && Contains( p, x + 9, y + 100, 267, 21 ) ) return Action( CommandAction.Worker );
        if ( tab == CommandTab.Economy && Contains( p, x + 9, y + 128, 267, 21 ) ) return Action( CommandAction.Wing );
        if ( tab == CommandTab.Economy && Contains( p, x + 9, y + 155, 267, 21 ) ) return Action( CommandAction.Scout );
        if ( tab == CommandTab.Economy && Contains( p, x + 9, y + 182, 267, 21 ) ) return Action( CommandAction.Drive );
        if ( tab == CommandTab.Fleet && Contains( p, x + 9, y + 83, 267, 21 ) ) return Action( CommandAction.Weapon );
        if ( tab == CommandTab.Fleet && Contains( p, x + 9, y + 110, 267, 21 ) ) return Action( CommandAction.Defense );
        if ( tab == CommandTab.Events && Contains( p, x + 9, y + 125, 78, 21 ) ) return Action( CommandAction.Earlier );
        if ( tab == CommandTab.Events && Contains( p, x + 197, y + 125, 78, 21 ) ) return Action( CommandAction.Later );
        return new( HudAction.Panel );

        static HudHit Action( CommandAction action ) => new( HudAction.Command, (int)action );
    }
}
