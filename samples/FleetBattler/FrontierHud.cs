using System.Numerics;

namespace FleetBattler;

internal enum CommandTab { Hidden, Economy, Research, Fleet, Orders, Events }
internal enum CommandAction { Toggle, Economy, Research, Fleet, Orders, Events,
    StartIndustry, StartWeapons, StartDrive, StartHabitation, StartAstrometry, CancelResearch, Earlier, Later }

internal sealed partial class FleetHud {
    private const float OfficeWidth = 350;
    private static readonly string[] TabNames = ["COLONY", "RESEARCH", "DESIGN", "ORDERS", "LOG"];
    private static readonly HudIcon[] TabIcons = [HudIcon.Colony, HudIcon.Science, HudIcon.Design, HudIcon.Orders, HudIcon.Log];
    private static (float X, float Y) CommandOrigin( Layout layout ) => ((layout.Width - OfficeWidth) / 2, 61);
    private static float CommandHeight( CommandTab tab, FleetSimulation fleet ) => tab switch {
        CommandTab.Economy => 226, CommandTab.Research => 251, CommandTab.Fleet => 289, CommandTab.Events => 239,
        CommandTab.Orders => 82 + Math.Max( 1, YardRows( fleet ) ) * 21, _ => 162
    };

    private static int YardRows( FleetSimulation fleet ) {
        int count = 0;
        for ( int body = 1; body < fleet.Bodies.Length; ++body )
            if ( fleet.CanOpenYard( body ) ) ++count;
        return count;
    }

    private void CommandPanel( Layout layout, FleetSimulation fleet, CommandTab tab, int dispatch, int speed, bool secondResearchPage ) {
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
                if ( fleet.Established ) {
                    var cap = fleet.ResourceCapacity;
                    label.TryWrite( $"CAP / {cap.Ore} ORE / {cap.Energy} ENERGY / {cap.Science} SCI", out int capLength );
                    Text( label[..capLength], x + 11, y + 152, 1, White );
                    if ( fleet.MerchantAgreement == MerchantDecision.Trade )
                        Text( "TRADE / 5 ENERGY FOR 12 ORE EVERY 5 CYCLES", x + 11, y + 170, 1, EnergyTint );
                    IconButton( x + 10, y + 193, 328, HudIcon.Yard, "OPEN EARTH CIVILIAN DOCK", true );
                } else Text( "NO SOL PRESENCE IN THIS SYSTEM", x + 11, y + 155, 1, Label );
                break;
            case CommandTab.Research:
                ResearchPanel( x, y, fleet, speed, secondResearchPage );
                break;
            case CommandTab.Fleet:
                var design = fleet.Design;
                var flight = FleetSimulation.Flight( HullRole.Corvette, design, fleet.DriveUpgrade );
                Art( Tile.Corvette, x + 10, y + 41, 61, 35 );
                Text( "CORVETTE / MODULE FIT", x + 80, y + 43, 1.2f, White );
                label.TryWrite( $"REACTOR LOAD {design.PowerUsed}/{design.PowerAvailable}", out int length );
                Text( label[..length], x + 80, y + 62, 1, design.Valid ? EnergyTint : SpaceDraw.Amber );
                for ( int i = 0; i < (int)ModuleSlot.Count; ++i ) {
                    var slot = (ModuleSlot)i;
                    float top = y + 85 + i * 23;
                    Rect( x + 10, top, 330, 21, Well );
                    Text( ModuleLabels[i], x + 17, top + 7, 1, Label );
                    Text( design.Installed( slot ), x + 94, top + 7, 1, White );
                    Button( x + 277, top, 59, "CHANGE", true );
                }
                label.TryWrite( $"HULL 72  ARMOR {design.Armor}  SHIELD {design.Shield}  DMG {design.Damage}  RNG {design.Range}", out length );
                Text( label[..length], x + 12, y + 232, 1, Label );
                label.TryWrite( $"CRUISE {flight.Cruise:F0}  THRUST {flight.Acceleration:F0}  RELOAD {design.Reload}T", out length );
                Text( label[..length], x + 12, y + 249, 1, Label );
                Cost( fleet.Hull( HullBlueprint.Corvette ).Cost, x + 12, y + 267 );
                Text( design.Valid ? "PER HULL / NEW ORDERS" : "OVER POWER / CHANGE MODULES", x + 95, y + 268, 1,
                    design.Valid ? White : SpaceDraw.Amber );
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
                    Text( "NO SOL PRESENCE", x + 11, y + 64, 1, Label );
                    int contactRow = 0;
                    for ( int side = (int)FactionId.Raiders; side < (int)FactionId.Count; ++side ) {
                        var faction = (FactionId)side;
                        int vessels = fleet.FactionCount( faction );
                        if ( vessels == 0 ) continue;
                        label.TryWrite( $"{Factions.Name( faction )} / {vessels} HULL(S)", out int n );
                        IconButton( x + 10, y + 87 + 25 * contactRow++, 328, HudIcon.Log, label[..n], true );
                    }
                    break;
                }
                var incident = fleet.Dispatch( dispatch );
                var (title, first, second) = IncidentText( incident );
                Art( incident switch {
                    FrontierEvent.Arrival => Tile.Scout, FrontierEvent.Prospect => Tile.Mine,
                    FrontierEvent.Distress or FrontierEvent.Raid or FrontierEvent.Counterattack => Tile.Corvette,
                    FrontierEvent.Secured => Tile.Yard, _ => Tile.Miner
                }, x + 11, y + 42, 101, 66 );
                Text( title, x + 121, y + 46, 1.23f, White );
                Text( incident == FrontierEvent.MerchantOffer ? "FREE MERCHANTS" : "SOL AUTHORITY DISPATCH", x + 121, y + 68, 1, Label );
                Text( first, x + 11, y + 115, 1, Label );
                Text( second, x + 11, y + 130, 1, Label );
                if ( incident == FrontierEvent.MerchantOffer ) MerchantChoice( x, y, fleet );
                Button( x + 10, y + 209, 74, "EARLIER", dispatch > 0 );
                Button( x + 265, y + 209, 74, "LATER", dispatch + 1 < fleet.DispatchCount );
                label.TryWrite( $"{dispatch + 1}/{fleet.DispatchCount}", out length );
                Text( label[..length], x + 153, y + 216, 1, Label );
                break;
        }
        _overlay = 0;
    }

    private static readonly string[] ModuleLabels = ["WEAPON", "ARMOR", "SHIELD", "COMPUTER", "REACTOR", "DRIVE"];

    private void MerchantChoice( float x, float y, FleetSimulation fleet ) {
        if ( fleet.MerchantAgreement != MerchantDecision.Pending ) {
            Text( fleet.MerchantAgreement == MerchantDecision.Trade ? "TRADE ROUTE ACTIVE / FREE MERCHANTS ALLIED"
                : "SURVEY DATA ACQUIRED / FREE MERCHANTS NEUTRAL", x + 11, y + 165, 1, SpaceDraw.Cyan );
            return;
        }
        Button( x + 11, y + 148, 328, "TRADE / 12 ENERGY FOR 25 ORE AND A TRADE ROUTE", fleet.CanAcceptContract( MerchantDecision.Trade ) );
        Button( x + 11, y + 174, 328, "RESEARCH / 15 ORE FOR 18 SCIENCE", fleet.CanAcceptContract( MerchantDecision.Research ) );
    }

    internal static string IncidentTitle( FrontierEvent incident ) => IncidentText( incident ).Title;
    private static (string Title, string First, string Second) IncidentText( FrontierEvent incident ) => incident switch {
        FrontierEvent.Arrival => ("FRONTIER CHARTER", "SURVEY A MOON OR PLANET WITH YOUR SCOUT", "SEND A CONSTRUCTOR TO BUILD ON A DEPOSIT"),
        FrontierEvent.Prospect => ("ORE SIGNATURE CONFIRMED", "SELECT A CONSTRUCTOR AND INSPECT THE BODY", "PLACE AN EXTRACTION RIG"),
        FrontierEvent.Distress => ("DISTRESS SIGNAL", "RAIDERS HAVE NOTICED YOUR EXTRACTION", "BUILD A SHIPYARD AND KEEP CIVILIANS CLEAR"),
        FrontierEvent.Raid => ("AMBER RAID INBOUND", "RAIDERS ARE APPROACHING YOUR SITES", "OPEN A SHIPYARD TO ORDER ESCORTS"),
        FrontierEvent.Counterattack => ("RIVAL COMMAND CONTACT", "RAIL CORVETTES ARE APPROACHING", "RAIL SHOTS BYPASS SHIELDS"),
        FrontierEvent.MerchantOffer => ("MERCHANT OFFER", "A HAULER OFFERS TO SUPPLY THE NEW SETTLEMENT", "FUND A TRADE ROUTE OR BUY THEIR SURVEY DATA"),
        FrontierEvent.Secured => ("SYSTEM SECURED", "HOSTILE CONTACTS HAVE BEEN DRIVEN OFF", "YOUR OUTPOST CAN EXPAND"),
        _ => throw new ArgumentOutOfRangeException( nameof(incident) )
    };

    private static HudHit HitCommand( Layout layout, Vector2 p, FleetSimulation fleet, CommandTab tab, int dispatch, bool secondResearchPage ) {
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
            if ( Contains( relative, 266, 39, 69, 21 ) ) return new( HudAction.ResearchPage );
            var topics = ResearchTopics( secondResearchPage );
            for ( int i = 0; i < topics.Length; ++i ) {
                var topic = topics[i];
                if ( !Contains( relative, 270, 107 + i * 54, 64, 21 ) ) continue;
                if ( fleet.ActiveResearch == topic ) return Action( CommandAction.CancelResearch );
                if ( fleet.CanResearch( topic ) ) return Action( (CommandAction)((int)CommandAction.StartIndustry + (int)topic) );
            }
        }
        if ( tab == CommandTab.Fleet ) {
            for ( int i = 0; i < (int)ModuleSlot.Count; ++i )
                if ( Contains( relative, 277, 85 + i * 23, 59, 21 ) ) return new( HudAction.CycleModule, i );
        }
        if ( tab == CommandTab.Events && fleet.DispatchCount > dispatch && dispatch >= 0 && fleet.Dispatch( dispatch ) == FrontierEvent.MerchantOffer ) {
            if ( Contains( relative, 11, 148, 328, 21 ) && fleet.CanAcceptContract( MerchantDecision.Trade ) )
                return new( HudAction.MerchantDecision, (int)MerchantDecision.Trade );
            if ( Contains( relative, 11, 174, 328, 21 ) && fleet.CanAcceptContract( MerchantDecision.Research ) )
                return new( HudAction.MerchantDecision, (int)MerchantDecision.Research );
        }
        if ( tab == CommandTab.Orders ) {
            int row = 0;
            for ( int body = 1; body < fleet.Bodies.Length; ++body )
                if ( fleet.CanOpenYard( body ) && Contains( relative, 10, 77 + row++ * 21, 328, 21 ) ) return new( HudAction.OpenYard, body );
        }
        return tab switch {
            CommandTab.Economy when fleet.Established && Contains( relative, 10, 193, 328, 21 ) => new( HudAction.OpenYard, FleetSimulation.Homeworld ),
            CommandTab.Events when Contains( relative, 10, 209, 74, 21 ) => Action( CommandAction.Earlier ),
            CommandTab.Events when Contains( relative, 265, 209, 74, 21 ) => Action( CommandAction.Later ),
            _ => new( HudAction.Panel )
        };

        static HudHit Action( CommandAction action ) => new( HudAction.Command, (int)action );
    }
}
