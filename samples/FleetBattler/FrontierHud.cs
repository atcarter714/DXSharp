using System.Numerics;

namespace FleetBattler;

internal enum CommandTab { Hidden, Economy, Fleet, Orders, Events }
internal enum CommandAction { Toggle, Economy, Fleet, Orders, Events, Worker, Scout, Drive, Wing,
    Pulse, Rail, Shield, Plating, CancelCivilian, CancelFirstWing, CancelSecondWing, Earlier, Later }

internal sealed partial class FleetHud {
    private const float OfficeWidth = 328;
    private static (float X, float Y) CommandOrigin( Layout layout ) => ((layout.Width - OfficeWidth) / 2, 61);
    private static float CommandHeight( CommandTab tab ) => tab switch {
        CommandTab.Economy => 227, CommandTab.Fleet => 229, CommandTab.Orders => 205, _ => 162
    };

    private void CommandPanel( Layout layout, FleetSimulation fleet, CommandTab tab, int dispatch, int speed ) {
        if ( layout.Width >= 700 ) {
            Nav( 0, "COLONY", CommandTab.Economy );
            Nav( 1, "DESIGN", CommandTab.Fleet );
            Nav( 2, "ORDERS", CommandTab.Orders );
            Nav( 3, "LOG", CommandTab.Events );
        } else Button( layout.ClockX - 89, 19, 83, tab == CommandTab.Hidden ? "OPERATIONS" : "CLOSE", true, tab != CommandTab.Hidden );
        if ( tab == CommandTab.Hidden ) return;

        var (x, y) = CommandOrigin( layout );
        _overlay = 1;
        PanelAt( x, y, OfficeWidth, CommandHeight( tab ), SpaceDraw.Cyan );
        Button( x + 10, y + 8, 71, "COLONY", true, tab == CommandTab.Economy );
        Button( x + 89, y + 8, 71, "DESIGN", true, tab == CommandTab.Fleet );
        Button( x + 168, y + 8, 71, "ORDERS", true, tab == CommandTab.Orders );
        Button( x + 247, y + 8, 71, "LOG", true, tab == CommandTab.Events );
        Span<char> label = stackalloc char[60];
        switch ( tab ) {
            case CommandTab.Economy:
                Text( "COLONY AND RESEARCH", x + 11, y + 42, 1.25f, White );
                label.TryWrite( $"{fleet.BodyName( FleetSimulation.Homeworld )}  /  {fleet.Ore} ORE  /  {fleet.Science} SCI", out int length );
                Text( label[..length], x + 11, y + 62, 1, Label );
                int oreRate = 0, scienceRate = 0;
                for ( int body = 1; body < fleet.Bodies.Length; ++body ) {
                    var status = fleet.Planet( body );
                    if ( status.MineRemaining == 0 ) oreRate += fleet.DepositYield( body );
                    if ( status.LabRemaining == 0 ) scienceRate += 6;
                }
                label.TryWrite( $"PER CYCLE  +{oreRate} ORE  +{scienceRate} SCI", out length );
                Text( label[..length], x + 11, y + 80, 1, Label );
                Button( x + 10, y + 101, 308, "BUILD MINER  /  20 ORE", fleet.CanQueueWorker );
                Button( x + 10, y + 130, 308, "REPLACE SCOUT  /  24 ORE", fleet.CanQueueScout );
                Button( x + 10, y + 159, 308, fleet.DriveUpgrade ? "LONG BURN DRIVE ONLINE" : "LONG BURN DRIVE  /  36 SCI",
                    !fleet.DriveUpgrade && fleet.Science >= FleetSimulation.DriveCost );
                Text( fleet.WorkerRemaining > 0 ? "CIVILIAN PRODUCTION ACTIVE  /  ORDERS" : "CLICK A BODY TO MANAGE ITS SITES",
                    x + 11, y + 195, 1, Label );
                break;
            case CommandTab.Fleet:
                var design = fleet.Design;
                Text( "CORVETTE BLUEPRINT", x + 11, y + 41, 1.27f, White );
                Text( "LOADOUT FOR NEW WINGS", x + 11, y + 59, 1, Label );
                Button( x + 10, y + 76, 150, "PULSE  /  RAPID FIRE", true, design.Weapon == WeaponKind.Pulse );
                Button( x + 168, y + 76, 150, "RAIL  /  PIERCING", true, design.Weapon == WeaponKind.Rail );
                Button( x + 10, y + 102, 150, "SHIELD GENERATOR", true, !design.Plating );
                Button( x + 168, y + 102, 150, "HEAVY PLATING", true, design.Plating );
                label.TryWrite( $"PER SHIP  HULL {FleetSimulation.HullCapacity}  SHIELD {design.Shield}  ARMOR {design.Armor}", out length );
                Text( label[..length], x + 11, y + 132, 1, Label );
                label.TryWrite( $"{design.Damage} DAMAGE  /  {design.Range} RANGE  /  {design.Reload}T RELOAD", out length );
                Text( label[..length], x + 11, y + 149, 1, Label );
                label.TryWrite( $"3 SHIPS  /  {fleet.CurrentWingCost} ORE  /  {Seconds( fleet.CurrentWingDuration, speed )} SEC AT {speed}X", out length );
                Text( label[..length], x + 11, y + 168, 1, White );
                if ( fleet.CanBuildWing ) label.TryWrite( $"QUEUE AT {fleet.BodyName( fleet.YardBody )}", out length );
                else length = CopyStatus( fleet.YardRemaining != 0 ? "REQUIRES AN ONLINE SHIPYARD" : "QUEUE / ORE / NAVAL LIMIT", label );
                Button( x + 10, y + 189, 308, label[..length], fleet.CanBuildWing );
                break;
            case CommandTab.Orders:
                Text( "PRODUCTION QUEUE", x + 11, y + 42, 1.25f, White );
                label.TryWrite( $"NAVAL {fleet.NavalUsed} ACTIVE  +{fleet.NavalReserved} QUEUED  /  {fleet.NavalCapacity}", out length );
                Text( label[..length], x + 11, y + 61, 1, Label );
                for ( int i = 0; i < FleetSimulation.WingGoal; ++i ) {
                    float row = y + 80 + 31 * i;
                    if ( i >= fleet.WingQueue.Length ) {
                        Text( i == 0 ? "NO WINGS QUEUED" : "SECOND SLOT OPEN", x + 11, row + 7, 1, Label );
                        continue;
                    }
                    var job = fleet.WingQueue[i];
                    label.TryWrite( $"{fleet.BodyName( job.Body )}  {(job.Design.Weapon == WeaponKind.Rail ? "RAIL" : "PULSE")} WING", out length );
                    Text( label[..length], x + 11, row, 1, White );
                    if ( i == 0 ) label.TryWrite( $"{Seconds( fleet.WingRemaining, speed )} SEC LEFT  /  {job.Cost} ORE", out length );
                    else label.TryWrite( $"WAITING  /  {job.Cost} ORE", out length );
                    Text( label[..length], x + 11, row + 12, 1, Label );
                    Button( x + 246, row, 72, "CANCEL", true );
                }
                if ( fleet.WorkerRemaining > 0 ) {
                    label.TryWrite( $"{(fleet.CivilianOrder == HullRole.Miner ? "MINER" : "SCOUT")}  /  {Seconds( fleet.WorkerRemaining, speed )} SEC LEFT", out length );
                    Text( label[..length], x + 11, y + 150, 1, White );
                    Button( x + 246, y + 145, 72, "CANCEL", true );
                } else Text( "CIVILIAN BERTH OPEN", x + 11, y + 150, 1, Label );
                Text( "SITE PROJECTS ARE LISTED ON EACH BODY", x + 11, y + 182, 1, Label );
                break;
            case CommandTab.Events:
                var (title, first, second) = IncidentText( fleet.Dispatch( dispatch ) );
                Text( title, x + 11, y + 43, 1.23f, White );
                Text( first, x + 11, y + 69, 1, Label );
                Text( second, x + 11, y + 88, 1, Label );
                Button( x + 10, y + 116, 74, "EARLIER", dispatch > 0 );
                Button( x + 244, y + 116, 74, "LATER", dispatch + 1 < fleet.DispatchCount );
                label.TryWrite( $"{dispatch + 1}/{fleet.DispatchCount}", out length );
                Text( label[..length], x + 148, y + 123, 1, Label );
                break;
        }
        _overlay = 0;

        void Nav( int slot, string name, CommandTab target ) =>
            Button( layout.ClockX - 312 + slot * 78, 19, 72, name, true, tab == target );
    }

    internal static string IncidentTitle( FrontierEvent incident ) => IncidentText( incident ).Title;
    private static (string Title, string First, string Second) IncidentText( FrontierEvent incident ) => incident switch {
        FrontierEvent.Arrival => ("FRONTIER CHARTER", "SURVEY A MOON OR PLANET WITH YOUR SCOUT", "SEND A MINER TO BUILD ON A DEPOSIT"),
        FrontierEvent.Prospect => ("ORE SIGNATURE CONFIRMED", "SELECT A MINER AND INSPECT THE BODY", "PLACE AN EXTRACTION RIG"),
        FrontierEvent.Distress => ("DISTRESS SIGNAL", "AMBER HAS NOTICED YOUR EXTRACTION", "BUILD A YARD AND KEEP WORKERS CLEAR"),
        FrontierEvent.Raid => ("AMBER RAID INBOUND", "CONTACTS ARE APPROACHING THE YARD", "MOVE CIVILIANS CLEAR AND DEPLOY ESCORTS"),
        FrontierEvent.Counterattack => ("AMBER COUNTERATTACK", "RAIL CORVETTES ARE APPROACHING", "RAIL SHOTS BYPASS SHIELDS"),
        FrontierEvent.Secured => ("SYSTEM SECURED", "THE RAIDERS HAVE BEEN DRIVEN OFF", "YOUR OUTPOST CAN EXPAND"),
        _ => throw new ArgumentOutOfRangeException( nameof(incident) )
    };

    private static HudHit HitCommand( Layout layout, Vector2 p, CommandTab tab ) {
        if ( layout.Width >= 700 ) {
            for ( int i = 0; i < 4; ++i )
                if ( Contains( p, layout.ClockX - 312 + i * 78, 19, 72, 21 ) )
                    return Action( (CommandAction)((int)CommandAction.Economy + i) );
        } else if ( Contains( p, layout.ClockX - 89, 19, 83, 21 ) ) return Action( CommandAction.Toggle );
        if ( tab == CommandTab.Hidden ) return default;
        var (x, y) = CommandOrigin( layout );
        if ( !Contains( p, x, y, OfficeWidth, CommandHeight( tab ) ) ) return default;
        for ( int i = 0; i < 4; ++i )
            if ( Contains( p, x + 10 + i * 79, y + 8, 71, 21 ) )
                return Action( (CommandAction)((int)CommandAction.Economy + i) );
        var relative = p - new Vector2( x, y );
        return tab switch {
            CommandTab.Economy when Contains( relative, 10, 101, 308, 21 ) => Action( CommandAction.Worker ),
            CommandTab.Economy when Contains( relative, 10, 130, 308, 21 ) => Action( CommandAction.Scout ),
            CommandTab.Economy when Contains( relative, 10, 159, 308, 21 ) => Action( CommandAction.Drive ),
            CommandTab.Fleet when Contains( relative, 10, 76, 150, 21 ) => Action( CommandAction.Pulse ),
            CommandTab.Fleet when Contains( relative, 168, 76, 150, 21 ) => Action( CommandAction.Rail ),
            CommandTab.Fleet when Contains( relative, 10, 102, 150, 21 ) => Action( CommandAction.Shield ),
            CommandTab.Fleet when Contains( relative, 168, 102, 150, 21 ) => Action( CommandAction.Plating ),
            CommandTab.Fleet when Contains( relative, 10, 189, 308, 21 ) => Action( CommandAction.Wing ),
            CommandTab.Orders when Contains( relative, 246, 80, 72, 21 ) => Action( CommandAction.CancelFirstWing ),
            CommandTab.Orders when Contains( relative, 246, 111, 72, 21 ) => Action( CommandAction.CancelSecondWing ),
            CommandTab.Orders when Contains( relative, 246, 145, 72, 21 ) => Action( CommandAction.CancelCivilian ),
            CommandTab.Events when Contains( relative, 10, 116, 74, 21 ) => Action( CommandAction.Earlier ),
            CommandTab.Events when Contains( relative, 244, 116, 74, 21 ) => Action( CommandAction.Later ),
            _ => new( HudAction.Panel )
        };

        static HudHit Action( CommandAction action ) => new( HudAction.Command, (int)action );
    }
}
