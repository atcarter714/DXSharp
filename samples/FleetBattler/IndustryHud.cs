using System.Numerics;

namespace FleetBattler;

internal enum HudIcon { Ore, Energy, Science, Colony, Design, Orders, Log, Merge, Split, Yard, Build, Repair, Retreat, Health, Ship }

internal sealed partial class FleetHud {
    private static readonly Vector4 OreTint = new( 0.92f, 0.68f, 0.44f, 1 );
    private static readonly Vector4 EnergyTint = new( 0.99f, 0.84f, 0.36f, 1 );
    private static readonly Vector4 ScienceTint = new( 0.48f, 0.85f, 1, 1 );
    private static readonly ulong[] Icons = [
        0x020e3ae7ddf1c, 0x00820c7e30c30, 0x0d8e7793dce36, 0x1ff1e3eef8e08, 0x08b1f77c70e08,
        0x1fe0dd83760ff, 0x0f913a44e913e, 0x02041cfe20a22, 0x088a08fe70408, 0x18f5ff55ff5e3, 0x03c187f306180, 0x0437df1c7030c, 0x00020c3ffff00, 0x03870e7ff838e, 0x01993ffffc704
    ];

    private void Icon( HudIcon icon, float x, float y, float size, Vector4 tint ) {
        ulong bits = Icons[(int)icon];
        _glyphs[_count++] = new( new Vector4( x, y, size, size ) * _scale, tint, (uint)bits, (uint)(bits >> 32), 7, _overlay );
    }

    private void IconButton( float x, float y, float width, HudIcon icon, ReadOnlySpan<char> title, bool enabled, bool active = false ) {
        Rect( x, y, width, 21, active ? Active : Well );
        Vector4 tint = active ? White : enabled ? SpaceDraw.Cyan : Disabled;
        Icon( icon, x + 4, y + 5, 11, tint );
        FitText( title, x + 19, y + 7, width - 22, 1, tint );
    }

    private void Wallet( FleetSimulation fleet, float x, float y, bool income = false ) {
        Resource( HudIcon.Ore, fleet.Ore, fleet.OreIncome, OreTint, x );
        Resource( HudIcon.Energy, fleet.Energy, fleet.EnergyIncome, EnergyTint, x + (income ? 98 : 55) );
        Resource( HudIcon.Science, fleet.Science, fleet.ScienceIncome, ScienceTint, x + (income ? 196 : 110) );

        void Resource( HudIcon icon, int balance, int rate, Vector4 tint, float left ) {
            Icon( icon, left, y, 10, tint );
            Span<char> line = stackalloc char[18];
            int length;
            if ( income ) line.TryWrite( $"{balance}  +{rate}", out length );
            else line.TryWrite( $"{balance}", out length );
            Text( line[..length], left + 14, y + 1, 1, tint );
        }
    }

    private const float YardWidth = 422, YardTop = 61, YardHeight = 323;
    private static float YardLeft( Layout layout ) => (layout.Width - YardWidth) / 2;

    private void DrawShipyard( Layout layout, FleetSimulation fleet, int body, int speed ) {
        float x = YardLeft( layout ), y = YardTop;
        _overlay = 2;
        PanelAt( x, y, YardWidth, YardHeight, EnergyTint );
        Icon( HudIcon.Yard, x + 11, y + 10, 16, EnergyTint );
        Text( fleet.YardAt( body ) == 0 ? "ORBITAL SHIPYARD" : "CIVILIAN DOCK", x + 35, y + 10, 1.45f, White );
        Text( fleet.BodyName( body ), x + 35, y + 32, 1, Label );
        Button( x + 390, y + 8, 22, "X", true );
        Wallet( fleet, x + 216, y + 31 );
        Rect( x + 220, y + 59, 1, 163, Well );
        Text( "SHIP DESIGNS", x + 11, y + 60, 1.15f, White );
        Text( "BUILD QUEUE", x + 233, y + 60, 1.15f, White );

        Span<char> label = stackalloc char[44];
        int length;
        for ( int i = 0; i < (int)HullBlueprint.Count; ++i ) {
            var blueprint = (HullBlueprint)i;
            var hull = fleet.Hull( blueprint );
            float row = y + 80 + i * 45;
            Rect( x + 10, row, 202, 41, Well );
            Art( HullArt( hull.Role ), x + 11, row + 2, 34, 33 );
            Text( hull.Name, x + 50, row + 5, 1, White );
            string? block = fleet.ShipOrderBlock( body, blueprint );
            Cost( hull.Cost, x + 50, row + 17 );
            if ( block is null ) label.TryWrite( $"{Seconds( hull.Duration, speed )} SEC", out length );
            else length = CopyStatus( block, label );
            Text( label[..length], x + 50, row + 29, 1, block is null ? Label : Disabled );
            if ( block is null ) Button( x + 170, row + 13, 37, "ADD", true );
        }

        var orders = fleet.YardQueue( body );
        for ( int i = 0; i < FleetSimulation.YardQueueLimit; ++i ) {
            float row = y + 80 + i * 33;
            Rect( x + 232, row, 179, 30, i == 0 && !orders.IsEmpty ? Active : Well );
            if ( i >= orders.Length ) {
                Text( i == 0 ? "IDLE" : "OPEN", x + 240, row + 8, 1, Label );
                continue;
            }
            var order = orders[i];
            Text( order.Hull switch { HullBlueprint.Scout => "SURVEY SCOUT", HullBlueprint.Miner => "CONSTRUCTOR", HullBlueprint.Colony => "COLONY SHIP", HullBlueprint.Frigate => "ESCORT FRIGATE",
                _ => order.Design.Weapon == WeaponKind.Rail ? "RAIL CORVETTE" : "PULSE CORVETTE" }, x + 239, row + 4, 1, White );
            int remaining = fleet.ShipRemaining( body );
            if ( i == 0 ) label.TryWrite( $"BUILDING / {Seconds( remaining, speed )} SEC", out length );
            else length = CopyStatus( "WAITING", label );
            Text( label[..length], x + 239, row + 15, 1, Label );
            if ( i == 0 ) Progress( new( x + 239, row + 25 ), 128, 1f - (float)remaining / order.Duration, EnergyTint );
            Button( x + 386, row + 4, 20, "X", true );
        }
        Text( "ONE HULL PER ORDER", x + 11, y + 307, 1, Label );
        _overlay = 0;
    }

    private static HudHit HitShipyard( Layout layout, Vector2 p, FleetSimulation fleet, int body ) {
        float x = YardLeft( layout ), y = YardTop;
        if ( !Contains( p, x, y, YardWidth, YardHeight ) ) return default;
        if ( Contains( p, x + 390, y + 8, 22, 21 ) ) return new( HudAction.CloseYard );
        for ( int i = 0; i < (int)HullBlueprint.Count; ++i )
            if ( Contains( p, x + 170, y + 93 + i * 45, 37, 21 ) && fleet.ShipOrderBlock( body, (HullBlueprint)i ) is null )
                return new( HudAction.BuildShip, i );
        for ( int i = 0; i < fleet.YardQueue( body ).Length; ++i )
            if ( Contains( p, x + 386, y + 84 + i * 33, 20, 21 ) ) return new( HudAction.CancelShip, i );
        return new( HudAction.Panel );
    }

    private void Cost( ResourceCost cost, float x, float y ) {
        if ( cost.Ore > 0 ) { Value( HudIcon.Ore, cost.Ore, OreTint ); x += 37; }
        if ( cost.Energy > 0 ) { Value( HudIcon.Energy, cost.Energy, EnergyTint ); x += 37; }
        if ( cost.Science > 0 ) Value( HudIcon.Science, cost.Science, ScienceTint );

        void Value( HudIcon icon, int amount, Vector4 tint ) {
            Icon( icon, x, y, 9, tint );
            Span<char> text = stackalloc char[10];
            text.TryWrite( $"{amount}", out int length );
            Text( text[..length], x + 12, y + 1, 1, tint );
        }
    }

    private static ReadOnlySpan<ResearchTopic> ResearchTopics( bool propulsion ) =>
        ResearchCatalog.Current.Topics( propulsion ? ResearchBranch.Propulsion : ResearchBranch.Industry );

    private void ResearchPanel( float x, float y, FleetSimulation fleet, int speed, bool propulsion ) {
        Text( propulsion ? "PROPULSION AND SURVEY" : "INDUSTRY AND DEFENSE", x + 11, y + 42, 1.25f, White );
        Button( x + 237, y + 39, 98, propulsion ? "INDUSTRY" : "PROPULSION", true );
        Span<char> line = stackalloc char[64];
        if ( fleet.ResearchRate == 0 ) Text( "BUILD A LAB OR OBSERVATORY TO START", x + 11, y + 62, 1, Label );
        else {
            line.TryWrite( $"LAB THROUGHPUT {fleet.ResearchRate}X / {fleet.ScienceIncome} SCI PER CYCLE", out int n );
            Text( line[..n], x + 11, y + 62, 1, Label );
        }
        var topics = ResearchTopics( propulsion );
        for ( int i = 0; i < topics.Length; ++i ) {
            var topic = topics[i];
            var tech = FleetSimulation.Tech( topic );
            float row = y + 82 + i * 54;
            float inset = tech.Requires.Length == 0 ? 0 : 14;
            if ( inset > 0 ) {
                Rect( x + 11 + inset - 7, row - 5, 1, 19, Label );
                Rect( x + 11 + inset - 7, row + 14, 7, 1, Label );
            }
            Rect( x + 10 + inset, row, 330 - inset, 49, fleet.ActiveResearch == topic ? Active : Well );
            Art( (Tile)tech.Artwork, x + 14 + inset, row + 4, 28, 27 );
            FitText( tech.Name, x + 47 + inset, row + 6, 278 - inset, 1.1f, White );
            FitText( tech.Benefit, x + 47 + inset, row + 20, 278 - inset, 1, Label );
            if ( fleet.Researched( topic ) ) {
                Text( "COMPLETE", x + 17 + inset, row + 34, 1, SpaceDraw.Cyan );
                continue;
            }
            if ( fleet.ActiveResearch == topic ) {
                line.TryWrite( $"{Seconds( fleet.ResearchRemaining, speed )} SEC LEFT", out int length );
                Text( line[..length], x + 17 + inset, row + 33, 1, ScienceTint );
                Progress( new( x + 145, row + 36 ), 105, fleet.ResearchProgress, ScienceTint );
                Button( x + 270, row + 25, 64, "CANCEL", true );
            } else if ( fleet.ResearchBlock( topic ) is { } block && block != "INSUFFICIENT RESOURCES" ) {
                line.TryWrite( $"REQUIRES / {block}", out int length );
                FitText( line[..length], x + 17 + inset, row + 34, 310 - inset, 1, Disabled );
            } else {
                Cost( tech.Cost, x + 17 + inset, row + 33 );
                int rate = Math.Max( 1, fleet.ResearchRate );
                line.TryWrite( $"{Seconds( (tech.Duration + rate - 1) / rate, speed )} SEC", out int length );
                Text( line[..length], x + 139 + inset, row + 34, 1, Label );
                Button( x + 270, row + 25, 64, "START", fleet.CanResearch( topic ) );
            }
        }
    }
}
