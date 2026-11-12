using System.Numerics;

namespace FleetBattler;

internal enum CommandTab { Hidden, Economy, Research, Fleet, Orders, Events }
internal enum CommandAction { Toggle, Economy, Research, Fleet, Orders, Events,
    Pulse, Rail, Shield, Plating, StartIndustry, StartWeapons, StartDrive, CancelResearch, Earlier, Later }

internal sealed partial class FleetHud {
    private const float OfficeWidth = 350;
    private static readonly string[] TabNames = ["COLONY", "RESEARCH", "DESIGN", "ORDERS", "LOG"];
    private static readonly HudIcon[] TabIcons = [HudIcon.Colony, HudIcon.Science, HudIcon.Design, HudIcon.Orders, HudIcon.Log];
    private static (float X, float Y) CommandOrigin( Layout layout ) => ((layout.Width - OfficeWidth) / 2, 61);
    private static float CommandHeight( CommandTab tab, FleetSimulation fleet ) => tab switch {
        CommandTab.Economy => 184, CommandTab.Research => 243, CommandTab.Fleet => 210,
        CommandTab.Orders => 82 + Math.Max( 1, YardRows( fleet ) ) * 21, _ => 162
    };

    private static int YardRows( FleetSimulation fleet ) {
        int count = 0;
        for ( int body = 1; body < fleet.Bodies.Length; ++body )
            if ( fleet.CanOpenYard( body ) ) ++count;
        return count;
    }

    private void CommandPanel( Layout layout, FleetSimulation fleet, CommandTab tab, int dispatch, int speed ) {
        if ( layout.Width >= 780 ) {
            for ( int i = 0; i < TabNames.Length; ++i )
                IconButton( layout.ClockX - 358 + i * 71, 19, 68, TabIcons[i], TabNames[i], true, (int)tab == i + 1 );
        } else IconButton( layout.ClockX - 89, 19, 83, HudIcon.Colony, tab == CommandTab.Hidden ? "COMMAND" : "CLOSE", true, tab != CommandTab.Hidden );
        if ( tab == CommandTab.Hidden ) return;

        var (x, y) = CommandOrigin( layout );
        _overlay = 1;
        PanelAt( x, y, OfficeWidth, CommandHeight( tab, fleet ), SpaceDraw.Cyan );
        for ( int i = 0; i < TabNames.Length; ++i )
            IconButton( x + 7 + i * 68, y + 8, 65, TabIcons[i], TabNames[i], true, (int)tab == i + 1 );
        Span<char> label = stackalloc char[56];
        switch ( tab ) {
            case CommandTab.Economy:
                Text( "COLONY ECONOMY", x + 11, y + 42, 1.25f, White );
                Text( "STOCKPILE AND INCOME PER CYCLE", x + 11, y + 62, 1, Label );
                Wallet( fleet, x + 11, y + 81, income: true );
                Text( "ORE / CONSTRUCTION AND HULL MATERIAL", x + 11, y + 103, 1, OreTint );
                Text( "ENERGY / SHIPYARDS AND INDUSTRY", x + 11, y + 119, 1, EnergyTint );
                Text( "SCIENCE / RESEARCH PROJECTS", x + 11, y + 135, 1, ScienceTint );
                if ( fleet.Established ) IconButton( x + 10, y + 153, 328, HudIcon.Yard, "OPEN EARTH CIVILIAN DOCK", true );
                else Text( "NO SOL PRESENCE IN THIS SYSTEM", x + 11, y + 155, 1, Label );
                break;
            case CommandTab.Research:
                ResearchPanel( x, y, fleet, speed );
                break;
            case CommandTab.Fleet:
                var design = fleet.Design;
                Text( "CORVETTE BLUEPRINT", x + 11, y + 41, 1.27f, White );
                Text( "LOADOUT FOR FUTURE SHIP ORDERS", x + 11, y + 59, 1, Label );
                Button( x + 10, y + 76, 160, "PULSE / RAPID FIRE", true, design.Weapon == WeaponKind.Pulse );
                Button( x + 178, y + 76, 160, "RAIL / PIERCING", true, design.Weapon == WeaponKind.Rail );
                Button( x + 10, y + 102, 160, "SHIELD GENERATOR", true, !design.Plating );
                Button( x + 178, y + 102, 160, "HEAVY PLATING", true, design.Plating );
                label.TryWrite( $"HULL {FleetSimulation.HullCapacity} / SHIELD {design.Shield} / ARMOR {design.Armor}", out int length );
                Text( label[..length], x + 11, y + 133, 1, Label );
                label.TryWrite( $"{design.Damage} DAMAGE / {design.Range} RANGE / {design.Reload}T RELOAD", out length );
                Text( label[..length], x + 11, y + 150, 1, Label );
                Cost( fleet.Hull( HullBlueprint.Corvette ).Cost, x + 12, y + 170 );
                Text( "PER HULL", x + 92, y + 171, 1, White );
                Text( "ORDER THIS DESIGN AT AN ORBITAL SHIPYARD", x + 11, y + 190, 1, Label );
                break;
            case CommandTab.Orders:
                Text( "PRODUCTION SITES", x + 11, y + 42, 1.25f, White );
                label.TryWrite( $"NAVAL {fleet.NavalUsed} ACTIVE  +{fleet.NavalReserved} QUEUED  /  {fleet.NavalCapacity}", out length );
                Text( label[..length], x + 11, y + 61, 1, Label );
                int row = 0;
                for ( int body = 1; body < fleet.Bodies.Length; ++body ) {
                    if ( !fleet.CanOpenYard( body ) ) continue;
                    int jobs = fleet.YardQueue( body ).Length;
                    if ( jobs == 0 ) label.TryWrite( $"{fleet.BodyName( body )}  /  IDLE", out length );
                    else label.TryWrite( $"{fleet.BodyName( body )}  /  {jobs} QUEUED  /  {Seconds( fleet.ShipRemaining( body ), speed )} SEC", out length );
                    IconButton( x + 10, y + 77 + row++ * 21, 328, HudIcon.Yard, label[..length], true );
                }
                if ( row == 0 ) Text( "NO SOL PRODUCTION SITES", x + 11, y + 79, 1, Label );
                break;
            case CommandTab.Events:
                if ( fleet.DispatchCount == 0 ) {
                    Text( "SYSTEM CONTACTS", x + 11, y + 43, 1.25f, White );
                    Text( "FREE MERCHANTS / NEUTRAL", x + 11, y + 69, 1, Label );
                    Text( "NO SOL PRESENCE OR CAMPAIGN DISPATCHES", x + 11, y + 88, 1, Label );
                    break;
                }
                var (title, first, second) = IncidentText( fleet.Dispatch( dispatch ) );
                Text( title, x + 11, y + 43, 1.23f, White );
                Text( first, x + 11, y + 69, 1, Label );
                Text( second, x + 11, y + 88, 1, Label );
                Button( x + 10, y + 116, 74, "EARLIER", dispatch > 0 );
                Button( x + 265, y + 116, 74, "LATER", dispatch + 1 < fleet.DispatchCount );
                label.TryWrite( $"{dispatch + 1}/{fleet.DispatchCount}", out length );
                Text( label[..length], x + 153, y + 123, 1, Label );
                break;
        }
        _overlay = 0;
    }

    internal static string IncidentTitle( FrontierEvent incident ) => IncidentText( incident ).Title;
    private static (string Title, string First, string Second) IncidentText( FrontierEvent incident ) => incident switch {
        FrontierEvent.Arrival => ("FRONTIER CHARTER", "SURVEY A MOON OR PLANET WITH YOUR SCOUT", "SEND A CONSTRUCTOR TO BUILD ON A DEPOSIT"),
        FrontierEvent.Prospect => ("ORE SIGNATURE CONFIRMED", "SELECT A CONSTRUCTOR AND INSPECT THE BODY", "PLACE AN EXTRACTION RIG"),
        FrontierEvent.Distress => ("DISTRESS SIGNAL", "RAIDERS HAVE NOTICED YOUR EXTRACTION", "BUILD A SHIPYARD AND KEEP CIVILIANS CLEAR"),
        FrontierEvent.Raid => ("AMBER RAID INBOUND", "RAIDERS ARE APPROACHING YOUR SITES", "OPEN A SHIPYARD TO ORDER ESCORTS"),
        FrontierEvent.Counterattack => ("RIVAL COMMAND CONTACT", "RAIL CORVETTES ARE APPROACHING", "RAIL SHOTS BYPASS SHIELDS"),
        FrontierEvent.Secured => ("SYSTEM SECURED", "HOSTILE CONTACTS HAVE BEEN DRIVEN OFF", "YOUR OUTPOST CAN EXPAND"),
        _ => throw new ArgumentOutOfRangeException( nameof(incident) )
    };

    private static HudHit HitCommand( Layout layout, Vector2 p, FleetSimulation fleet, CommandTab tab ) {
        if ( layout.Width >= 780 ) {
            for ( int i = 0; i < TabNames.Length; ++i )
                if ( Contains( p, layout.ClockX - 358 + i * 71, 19, 68, 21 ) )
                    return Action( (CommandAction)((int)CommandAction.Economy + i) );
        } else if ( Contains( p, layout.ClockX - 89, 19, 83, 21 ) ) return Action( CommandAction.Toggle );
        if ( tab == CommandTab.Hidden ) return default;
        var (x, y) = CommandOrigin( layout );
        if ( !Contains( p, x, y, OfficeWidth, CommandHeight( tab, fleet ) ) ) return default;
        for ( int i = 0; i < TabNames.Length; ++i )
            if ( Contains( p, x + 7 + i * 68, y + 8, 65, 21 ) )
                return Action( (CommandAction)((int)CommandAction.Economy + i) );
        var relative = p - new Vector2( x, y );
        if ( tab == CommandTab.Research ) {
            for ( int i = 0; i < 3; ++i ) {
                var topic = (ResearchTopic)i;
                if ( !Contains( relative, 270, 102 + i * 53, 64, 21 ) ) continue;
                if ( fleet.ActiveResearch == topic ) return Action( CommandAction.CancelResearch );
                if ( fleet.CanResearch( topic ) ) return Action( (CommandAction)((int)CommandAction.StartIndustry + i) );
            }
        }
        if ( tab == CommandTab.Orders ) {
            int row = 0;
            for ( int body = 1; body < fleet.Bodies.Length; ++body )
                if ( fleet.CanOpenYard( body ) && Contains( relative, 10, 77 + row++ * 21, 328, 21 ) ) return new( HudAction.OpenYard, body );
        }
        return tab switch {
            CommandTab.Economy when fleet.Established && Contains( relative, 10, 153, 328, 21 ) => new( HudAction.OpenYard, FleetSimulation.Homeworld ),
            CommandTab.Fleet when Contains( relative, 10, 76, 160, 21 ) => Action( CommandAction.Pulse ),
            CommandTab.Fleet when Contains( relative, 178, 76, 160, 21 ) => Action( CommandAction.Rail ),
            CommandTab.Fleet when Contains( relative, 10, 102, 160, 21 ) => Action( CommandAction.Shield ),
            CommandTab.Fleet when Contains( relative, 178, 102, 160, 21 ) => Action( CommandAction.Plating ),
            CommandTab.Events when Contains( relative, 10, 116, 74, 21 ) => Action( CommandAction.Earlier ),
            CommandTab.Events when Contains( relative, 265, 116, 74, 21 ) => Action( CommandAction.Later ),
            _ => new( HudAction.Panel )
        };

        static HudHit Action( CommandAction action ) => new( HudAction.Command, (int)action );
    }
}
