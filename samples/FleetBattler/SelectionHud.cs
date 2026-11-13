using System.Numerics;

namespace FleetBattler;

internal sealed partial class FleetHud {
    private static Vector4 SelectionDetailsArea( Layout layout ) =>
        new( layout.Width - 311, 242, 297, MathF.Min( 246, layout.Bottom - 247 ) );

    private void SelectionDetails( Layout layout, FleetSimulation fleet, ReadOnlySpan<bool> selected, FleetSelection unit, int speed ) {
        var panel = SelectionDetailsArea( layout );
        if ( panel.W < 90 ) return;
        float x = panel.X, y = panel.Y;
        PanelAt( x, y, panel.Z, panel.W, SpaceDraw.Cyan );
        bool single = unit.IsShip && fleet.Alive( unit.Ship );
        var role = fleet.Role( unit.Squad );
        Art( HullArt( role ), x + 9, y + 9, 56, 39 );
        Span<char> line = stackalloc char[66];
        string heading = single ? fleet.ShipName( unit.Ship ) ?? FleetSimulation.RoleName( role ) : "FLEET READINESS";
        FitText( heading, x + 74, y + 10, 210, 1.4f, White );
        var (count, hull, maximum) = MeasureSelection( fleet, selected );
        if ( single ) {
            var order = fleet.Plan( unit );
            var phase = PhaseName( fleet.RoutePhase( unit ) );
            string position = order.Body >= 0 ? fleet.BodyName( order.Body ) : order.Target >= 0 ? "HOSTILE CONTACT" : "OPEN SPACE";
            FitText( $"{position} / {phase}", x + 74, y + 31, 212, 1, SpaceDraw.Cyan );
        } else {
            line.TryWrite( $"{count} HULLS / {fleet.SelectionStance( selected )}", out int n );
            FitText( line[..n], x + 74, y + 32, 212, 1, SpaceDraw.Cyan );
        }

        if ( single ) { hull = fleet.Hull( unit.Ship ); maximum = FleetSimulation.MaxHull( role ); }
        Icon( HudIcon.Health, x + 10, y + 59, 13, SpaceDraw.Cyan );
        line.TryWrite( $"HULL {hull}/{maximum}", out int length );
        Text( line[..length], x + 29, y + 62, 1, White );
        Progress( new( x + 151, y + 64 ), 132, maximum == 0 ? 0 : (float)hull / maximum, SpaceDraw.Cyan );
        if ( panel.W < 120 ) return;
        if ( !single ) {
            DrawComposition( fleet, selected, x + 10, y + 82, panel.W - 90 );
            return;
        }

        var flight = fleet.Flight( unit.Squad );
        line.TryWrite( $"CRUISE {flight.Cruise:F0} / THRUST {flight.Acceleration:F0}", out length );
        Icon( HudIcon.Retreat, x + 10, y + 81, 12, EnergyTint );
        FitText( line[..length], x + 29, y + 84, 252, 1, Label );
        int ticks = fleet.RouteTicks( unit );
        if ( ticks > 0 ) line.TryWrite( $"ARRIVAL IN {Seconds( ticks, speed )} SEC", out length );
        else line.TryWrite( $"{FleetSimulation.RoleName( role )} / {fleet.Service( unit )}", out length );
        Icon( HudIcon.Orders, x + 10, y + 99, 12, ScienceTint );
        FitText( line[..length], x + 29, y + 102, 256, 1, Label );
        if ( !FleetSimulation.IsWarship( role ) ) {
            if ( panel.W >= 155 )
                WrapText( role switch {
                    HullRole.Scout => "SURVEY UNSCANNED BODIES. HOLDS POSITION WHILE ITS SENSOR SWEEP RUNS.",
                    HullRole.Miner => "BUILDS AND SERVICES ORBITAL SITES. REQUIRES A SURVEY BEFORE CONSTRUCTION.",
                    HullRole.Colony => "CARRIES SETTLERS TO A SURVEYED BODY WITH A LANDING DEPOT.",
                    _ => "CIVILIAN SUPPORT HULL."
                }, x + 11, y + 123, 275, panel.W - 128, 1, Label );
            return;
        }
        if ( panel.W < 145 ) return;
        var fit = fleet.Loadout( unit.Ship );
        line.TryWrite( $"SHIELD {fleet.Shield( unit.Ship )}/{fit.Shield}   ARMOR {fleet.Armor( unit.Ship )}/{fit.Armor}", out length );
        Text( line[..length], x + 11, y + 123, 1, White );
        for ( int i = 0; i < (int)ModuleSlot.Count && 154 + 14 * i < panel.W; ++i ) {
            float top = y + 143 + 14 * i;
            Rect( x + 9, top, 279, 13, Well );
            Icon( i == (int)ModuleSlot.Power ? HudIcon.Energy : HudIcon.Design, x + 11, top + 2, 9, Label );
            Text( ModuleLabels[i], x + 27, top + 3, 0.93f, Label );
            FitText( fit.Installed( (ModuleSlot)i ), x + 99, top + 3, 184, 0.93f, White );
        }
    }

    private static (int Count, int Hull, int Maximum) MeasureSelection( FleetSimulation fleet, ReadOnlySpan<bool> selected ) {
        int count = 0, hull = 0, maximum = 0;
        for ( int ship = 0; ship < Math.Min( fleet.Count, selected.Length ); ++ship ) {
            if ( !selected[ship] || !fleet.Alive( ship ) ) continue;
            ++count;
            hull += fleet.Hull( ship );
            maximum += FleetSimulation.MaxHull( fleet.Role( fleet.SquadronOf( ship ) ) );
        }
        return (count, hull, maximum);
    }

    private void DrawComposition( FleetSimulation fleet, ReadOnlySpan<bool> selected, float x, float y, float height ) {
        Span<int> roles = stackalloc int[(int)HullRole.Frigate + 1];
        for ( int ship = 0; ship < Math.Min( fleet.Count, selected.Length ); ++ship )
            if ( selected[ship] && fleet.Alive( ship ) ) ++roles[(int)fleet.Role( fleet.SquadronOf( ship ) )];
        Span<char> line = stackalloc char[32];
        int row = 0;
        for ( int i = 0; i < roles.Length && (row + 1) * 23 <= height; ++i ) {
            if ( roles[i] == 0 ) continue;
            float top = y + row++ * 23;
            var role = (HullRole)i;
            Rect( x, top, 276, 21, Well );
            Art( HullArt( role ), x + 1, top + 1, 27, 19 );
            line.TryWrite( $"{FleetSimulation.RoleName( role )} / {roles[i]}", out int length );
            Text( line[..length], x + 35, top + 7, 1, White );
        }
    }
}
