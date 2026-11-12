using System.Numerics;

namespace FleetBattler;

internal enum HudIcon { Ore, Energy, Science, Colony, Design, Orders, Log, Merge, Split, Yard }

internal sealed partial class FleetHud {
    private static readonly Vector4 OreTint = new( 0.92f, 0.68f, 0.44f, 1 );
    private static readonly Vector4 EnergyTint = new( 0.99f, 0.84f, 0.36f, 1 );
    private static readonly Vector4 ScienceTint = new( 0.48f, 0.85f, 1, 1 );
    private static readonly ulong[] Icons = [
        0x020e3ae7ddf1c, 0x00820c7e30c30, 0x0d8e7793dce36, 0x1ff1e3eef8e08, 0x08b1f77c70e08,
        0x1fe0dd83760ff, 0x0f913a44e913e, 0x02041cfe20a22, 0x088a08fe70408, 0x18f5ff55ff5e3
    ];

    private void Icon( HudIcon icon, float x, float y, float size, Vector4 tint ) {
        ulong bits = Icons[(int)icon];
        _glyphs[_count++] = new( new Vector4( x, y, size, size ) * _scale, tint, (uint)bits, (uint)(bits >> 32), 7, _overlay );
    }

    private void IconButton( float x, float y, float width, HudIcon icon, ReadOnlySpan<char> title, bool enabled, bool active = false ) {
        Rect( x, y, width, 21, active ? Active : Well );
        Vector4 tint = active ? White : enabled ? SpaceDraw.Cyan : Disabled;
        Icon( icon, x + 4, y + 5, 11, tint );
        Text( title, x + 19, y + 7, 1, tint );
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

    private const float YardWidth = 422, YardTop = 61, YardHeight = 240;
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
        for ( int i = 0; i < 3; ++i ) {
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
            Text( order.Hull switch { HullBlueprint.Scout => "SURVEY SCOUT", HullBlueprint.Miner => "CONSTRUCTOR",
                _ => order.Design.Weapon == WeaponKind.Rail ? "RAIL CORVETTE" : "PULSE CORVETTE" }, x + 239, row + 4, 1, White );
            int remaining = fleet.ShipRemaining( body );
            if ( i == 0 ) label.TryWrite( $"BUILDING / {Seconds( remaining, speed )} SEC", out length );
            else length = CopyStatus( "WAITING", label );
            Text( label[..length], x + 239, row + 15, 1, Label );
            if ( i == 0 ) Progress( new( x + 239, row + 25 ), 128, 1f - (float)remaining / order.Duration, EnergyTint );
            Button( x + 386, row + 4, 20, "X", true );
        }
        Text( "ONE HULL PER ORDER", x + 11, y + 222, 1, Label );
        _overlay = 0;
    }

    private static HudHit HitShipyard( Layout layout, Vector2 p, FleetSimulation fleet, int body ) {
        float x = YardLeft( layout ), y = YardTop;
        if ( !Contains( p, x, y, YardWidth, YardHeight ) ) return default;
        if ( Contains( p, x + 390, y + 8, 22, 21 ) ) return new( HudAction.CloseYard );
        for ( int i = 0; i < 3; ++i )
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

    private void ResearchPanel( float x, float y, FleetSimulation fleet, int speed ) {
        Text( "RESEARCH PROJECTS", x + 11, y + 42, 1.25f, White );
        Text( fleet.ScienceIncome == 0 ? "BUILD A RESEARCH LAB TO START" : "ONE ACTIVE PROJECT / COST RESERVED ON START", x + 11, y + 60, 1, Label );
        Span<char> line = stackalloc char[40];
        for ( int i = 0; i < 3; ++i ) {
            var topic = (ResearchTopic)i;
            var tech = FleetSimulation.Tech( topic );
            float row = y + 77 + i * 53;
            Rect( x + 10, row, 330, 49, fleet.ActiveResearch == topic ? Active : Well );
            Text( tech.Name, x + 17, row + 6, 1.1f, White );
            Text( tech.Benefit, x + 17, row + 20, 1, Label );
            if ( fleet.Researched( topic ) ) {
                Text( "COMPLETE", x + 17, row + 34, 1, SpaceDraw.Cyan );
                continue;
            }
            if ( fleet.ActiveResearch == topic ) {
                line.TryWrite( $"{Seconds( fleet.ResearchRemaining, speed )} SEC LEFT", out int length );
                Text( line[..length], x + 17, row + 33, 1, ScienceTint );
                Progress( new( x + 124, row + 36 ), 105, 1f - (float)fleet.ResearchRemaining / tech.Duration, ScienceTint );
                Button( x + 270, row + 25, 64, "CANCEL", true );
            } else {
                Cost( tech.Cost, x + 17, row + 33 );
                line.TryWrite( $"{Seconds( tech.Duration, speed )} SEC AT {speed}X", out int length );
                Text( line[..length], x + 104, row + 34, 1, Label );
                Button( x + 270, row + 25, 64, "START", fleet.CanResearch( topic ) );
            }
        }
    }
}
