using System.Numerics;

namespace FleetBattler;

internal sealed partial class FleetHud {
    private void DrawFleetOrders( Layout layout, FleetSimulation fleet, ReadOnlySpan<bool> selection ) {
        float y = layout.FleetControls( fleet ) + 25;
        if ( y + 51 >= layout.Bottom ) return;
        var stance = fleet.SelectionStance( selection );
        string mode = stance switch { FleetStance.Passive => "PASSIVE", FleetStance.Aggressive => "AGGRESSIVE",
            FleetStance.Mixed => "MIXED", _ => "NORMAL" };
        Span<char> title = stackalloc char[24];
        title.TryWrite( $"STANCE / {mode}", out int count );
        IconButton( 14, y, 157, HudIcon.Orders, title[..count], true, stance == FleetStance.Aggressive );
        IconButton( 14, y + 25, 51, HudIcon.Retreat, "FLEE", true );
        IconButton( 68, y + 25, 51, HudIcon.Health, "FIX", fleet.YardCount > 0 );
        IconButton( 122, y + 25, 49, HudIcon.Design, "FIT", fleet.YardCount > 0 && fleet.Design.Valid );
    }

    private static HudHit HitFleetOrders( Layout layout, Vector2 p, FleetSimulation fleet ) {
        float y = layout.FleetControls( fleet ) + 25;
        if ( y + 51 >= layout.Bottom || !Contains( p, 14, y, 157, 51 ) ) return default;
        if ( p.Y < y + 21 ) return new( HudAction.Stance );
        if ( p.Y < y + 25 || p.Y >= y + 46 ) return new( HudAction.Panel );
        return p.X < 66 ? new( HudAction.ShipService, (int)ServiceOrder.Retreat )
            : p.X < 121 ? new( HudAction.ShipService, (int)ServiceOrder.Repair )
            : new( HudAction.ShipService, (int)ServiceOrder.Refit );
    }
}
