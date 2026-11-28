using System.Numerics;

namespace FleetBattler;

internal enum HudIcon { Ore, Energy, Science, Colony, Design, Orders, Log, Merge, Split, Yard, Build, Repair, Retreat, Health, Ship, Chart, Route, Signal, Rally, Launch, Crew, Play, Pause, Flyby }

internal sealed partial class FleetHud {
    private static readonly Vector4 OreTint = new( 0.92f, 0.68f, 0.44f, 1 );
    private static readonly Vector4 EnergyTint = new( 0.99f, 0.84f, 0.36f, 1 );
    private static readonly Vector4 ScienceTint = new( 0.48f, 0.85f, 1, 1 );
    private static readonly HudIcon[] BranchIcons = [HudIcon.Yard, HudIcon.Flyby, HudIcon.Signal, HudIcon.Ship, HudIcon.Build, HudIcon.Rally];
    private static readonly ulong[] Icons = [
        0x020e3ae7ddf1c, 0x00820c7e30c30, 0x0d8e7793dce36, 0x1ff1e3eef8e08, 0x08b1f77c70e08,
        0x1fe0dd83760ff, 0x0f913a44e913e, 0x02041cfe20a22, 0x088a08fe70408, 0x18f5ff55ff5e3, 0x03c187f306180, 0x0437df1c7030c, 0x00020c3ffff00, 0x03870e7ff838e, 0x01993ffffc704,
        0x071149ab2511c, // Chart
        0x1830381038183, // Route
        0x07041c457511c, // Signal
        0x07040892f8e08, // Rally
        0x05042a7cf1e18, // Launch
        0x18e09c7d06e9c, // Crew
        0x01061c7870604, // Play
        0x0d9b366cd9b36, // Pause
        0x1030411370c7c, // Flyby
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
        Resource( HudIcon.Ore, fleet.Ore, fleet.OreIncome - fleet.BlockadeOrePerCycle, OreTint, x );
        Resource( HudIcon.Energy, fleet.Energy, fleet.EnergyIncome - fleet.EnergyUpkeep - fleet.BlockadeEnergyPerCycle, EnergyTint, x + (income ? 98 : 55) );
        Resource( HudIcon.Science, fleet.Science, fleet.ScienceIncome, ScienceTint, x + (income ? 196 : 110) );

        void Resource( HudIcon icon, int balance, int rate, Vector4 tint, float left ) {
            Icon( icon, left, y, 10, tint );
            Span<char> line = stackalloc char[18];
            int length;
            if ( income ) line.TryWrite( $"{balance}  {rate:+0;-0;0}", out length );
            else line.TryWrite( $"{balance}", out length );
            Text( line[..length], left + 14, y + 1, 1, tint );
        }
    }

    private const float YardWidth = 422, YardTop = 61, YardHeight = 333;
    private const int YardPageSize = 5;
    private const int YardPages = ((int)HullBlueprint.Count + YardPageSize - 1) / YardPageSize;
    private static float YardLeft( Layout layout ) => (layout.Width - YardWidth) / 2;

    private void DrawShipyard( Layout layout, FleetSimulation fleet, int body, float speed, int page ) {
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
        int start = page * YardPageSize;
        for ( int i = 0; i < YardPageSize && start + i < (int)HullBlueprint.Count; ++i ) {
            var blueprint = (HullBlueprint)(start + i);
            var hull = fleet.Hull( blueprint );
            float row = y + 80 + i * 45;
            Rect( x + 10, row, 202, 41, Well );
            Art( HullArt( hull.Role ), x + 11, row + 2, 34, 33 );
            Text( hull.Name, x + 50, row + 5, 1, White );
            string? block = fleet.ShipOrderBlock( body, blueprint );
            Cost( hull.Cost, x + 50, row + 17 );
            if ( block is null ) label.TryWrite( $"{TimeLeft( hull.Duration, speed )}", out length );
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
            FitText( fleet.Hull( order.Hull ).Name, x + 239, row + 4, 139, 1, White );
            int remaining = fleet.ShipRemaining( body );
            if ( i == 0 ) label.TryWrite( $"BUILDING / {TimeLeft( remaining, speed )}", out length );
            else length = CopyStatus( "WAITING", label );
            Text( label[..length], x + 239, row + 15, 1, Label );
            if ( i == 0 ) Progress( new( x + 239, row + 25 ), 128, 1f - (float)remaining / order.Duration, EnergyTint );
            Button( x + 386, row + 4, 20, "X", true );
        }
        Button( x + 10, y + 304, 24, "-", page > 0 );
        Button( x + 186, y + 304, 24, "+", page + 1 < YardPages );
        Text( $"HULL CATALOG {page + 1}/{YardPages}", x + 42, y + 311, 1, Label );
        _overlay = 0;
    }

    private static HudHit HitShipyard( Layout layout, Vector2 p, FleetSimulation fleet, int body, int page ) {
        float x = YardLeft( layout ), y = YardTop;
        if ( !Contains( p, x, y, YardWidth, YardHeight ) ) return default;
        if ( Contains( p, x + 390, y + 8, 22, 21 ) ) return new( HudAction.CloseYard );
        if ( page > 0 && Contains( p, x + 10, y + 304, 24, 21 ) ) return new( HudAction.YardPage, page - 1 );
        if ( page + 1 < YardPages && Contains( p, x + 186, y + 304, 24, 21 ) ) return new( HudAction.YardPage, page + 1 );
        int start = page * YardPageSize;
        for ( int i = 0; i < YardPageSize && start + i < (int)HullBlueprint.Count; ++i )
            if ( Contains( p, x + 170, y + 93 + i * 45, 37, 21 ) && fleet.ShipOrderBlock( body, (HullBlueprint)(start + i) ) is null )
                return new( HudAction.BuildShip, start + i );
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

    private static readonly string[] ResearchLabels = ["INDUSTRY", "PROPULSION", "SENSORS", "HULLS", "STRUCTURES", "DOCTRINE"];

    private void ResearchPanel( float x, float y, FleetSimulation fleet, float speed, ResearchBranch branch ) {
        for ( int i = 0; i < ResearchLabels.Length; ++i )
            IconButton( x + 10 + i % 3 * 110, y + 39 + i / 3 * 23, 106, BranchIcons[i], ResearchLabels[i], true, i == (int)branch % (int)ResearchBranch.Count );
        Span<char> line = stackalloc char[64];
        if ( fleet.ResearchRate == 0 ) Text( "BUILD A LAB OR OBSERVATORY TO START", x + 11, y + 94, 1, Label );
        else {
            line.TryWrite( $"LAB THROUGHPUT {fleet.ResearchRate}X / {fleet.ScienceIncome} SCI PER CYCLE", out int n );
            Text( line[..n], x + 11, y + 94, 1, Label );
        }
        int pages = ResearchCatalog.Current.PageCount( branch );
        if ( pages > 1 ) Button( x + 254, y + 87, 85, $"PAGE {(int)branch / (int)ResearchBranch.Count + 1}/{pages} >", true );
        var topics = ResearchCatalog.Current.Topics( branch );
        for ( int i = 0; i < topics.Length; ++i ) {
            var topic = topics[i];
            var tech = FleetSimulation.Tech( topic );
            float row = y + 112 + i * 54;
            float inset = tech.Requires.Length == 0 ? 0 : 14;
            if ( inset > 0 ) {
                Rect( x + 11 + inset - 7, row - 5, 1, 19, Label );
                Rect( x + 11 + inset - 7, row + 14, 7, 1, Label );
            }
            Rect( x + 10 + inset, row, 330 - inset, 49, fleet.ActiveResearch == topic ? Active : Well );
            Art( (Tile)tech.Artwork, x + 14 + inset, row + 4, 28, 27 );
            FitText( tech.Name, x + 47 + inset, row + 6, 278 - inset, 1.1f, White );
            FitText( tech.Benefit, x + 47 + inset, row + 20, 278 - inset, 1, tech.Exclusive is null ? Label : EnergyTint );
            if ( fleet.Researched( topic ) ) {
                Text( "COMPLETE", x + 17 + inset, row + 34, 1, SpaceDraw.Cyan );
                continue;
            }
            if ( fleet.ActiveResearch == topic ) {
                line.TryWrite( $"{TimeLeft( fleet.ResearchRemaining, speed )} LEFT", out int length );
                Text( line[..length], x + 17 + inset, row + 33, 1, ScienceTint );
                Progress( new( x + 145, row + 36 ), 105, fleet.ResearchProgress, ScienceTint );
                Button( x + 270, row + 25, 64, "CANCEL", true );
            } else if ( fleet.ResearchBlock( topic ) is { } block && block != "INSUFFICIENT RESOURCES" ) {
                line.TryWrite( $"REQUIRES / {block}", out int length );
                FitText( line[..length], x + 17 + inset, row + 34, 310 - inset, 1, Disabled );
            } else {
                Cost( tech.Cost, x + 17 + inset, row + 33 );
                line.TryWrite( $"{TimeLeft( fleet.ResearchTicks( tech ), speed )}", out int length );
                Text( line[..length], x + 139 + inset, row + 34, 1, Label );
                Button( x + 270, row + 25, 64, "START", fleet.CanResearch( topic ) );
            }
        }
    }
}
