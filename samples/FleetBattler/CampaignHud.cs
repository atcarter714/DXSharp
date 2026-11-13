using System.Numerics;

namespace FleetBattler;

internal sealed partial class FleetHud {
    internal static DateOnly StarDate( int campaignTick ) =>
        new DateOnly( 2184, 1, 1 ).AddDays( Math.Max( 0, campaignTick ) / 120 );

    private void ClockPanel( Layout layout, int tick, int speed, bool paused, string mapButton ) {
        float x = layout.ClockX;
        Rect( x, 16, 125, 34, Well );
        var date = StarDate( tick );
        Span<char> text = stackalloc char[22];
        text.TryWrite( $"{date.Year:D4}.{date.Month:D2}.{date.Day:D2}", out int length );
        Text( text[..length], x + 36, 19, 1, White );
        Rect( x + 4, 18, 26, 28, Active );
        Text( paused ? ">" : "II", x + 10, 28, 1, paused ? EnergyTint : SpaceDraw.Cyan );
        Button( x + 35, 29, 22, "-", speed > 1 );
        text.TryWrite( $"{speed}X", out length );
        Text( paused ? "PAUSED" : text[..length], x + 58, 38, 1, paused ? EnergyTint : SpaceDraw.Cyan );
        Button( x + 101, 29, 22, "+", speed < 4 );
        Button( x + 132, 19, 66, mapButton, true );
    }

    private static HudHit HitClock( Layout layout, Vector2 p ) {
        float x = layout.ClockX;
        if ( Contains( p, x + 4, 18, 26, 28 ) ) return new( HudAction.Pause );
        if ( Contains( p, x + 35, 29, 22, 21 ) ) return new( HudAction.Slower );
        if ( Contains( p, x + 101, 29, 22, 21 ) ) return new( HudAction.Speed );
        if ( Contains( p, x + 132, 19, 66, 21 ) ) return new( HudAction.Galaxy );
        return default;
    }

    private void ChartSummary( Layout layout, FleetSimulation fleet, int planet ) {
        float x = 14, y = 63, width = 251;
        PanelAt( x, y, width, 103, Label );
        Icon( HudIcon.Science, x + 10, y + 9, 14, SpaceDraw.Cyan );
        Text( "TELESCOPE CHART", x + 30, y + 11, 1.27f, White );
        WrapText( fleet.SystemIndex == FleetSimulation.Proxima
            ? "A silent precursor receiver is marked on this chart. Plan a crewed expedition from Sol."
            : "Stellar observations show the arrangement of the major bodies. Local contacts require a ship in this system.",
            x + 10, y + 33, width - 20, 42, 1, Label );
        Button( x + 10, y + 76, 132, "BACK TO GALAXY", true );
        if ( planet < 0 ) return;
        x = layout.Width - 243;
        PanelAt( x, y, 229, 66, Label );
        FitText( fleet.BodyName( planet ), x + 10, y + 10, 186, 1.2f, White );
        Button( x + 201, y + 5, 20, "X", true );
        Text( planet == 0 ? "SYSTEM PRIMARY" : "BODY FROM REMOTE SURVEY", x + 10, y + 34, 1, Label );
        Text( "NO LOCAL SENSOR DATA", x + 10, y + 49, 1, Label );
    }

    private static HudHit HitChart( Layout layout, Vector2 p, int planet ) {
        var clock = HitClock( layout, p );
        if ( clock.Action != HudAction.None ) return clock;
        if ( Contains( p, 24, 139, 132, 21 ) ) return new( HudAction.Galaxy );
        if ( planet >= 0 && Contains( p, layout.Width - 42, 68, 20, 21 ) ) return new( HudAction.Planet, (int)PlanetAction.Close );
        return Contains( p, 14, 12, layout.Width - 28, 41 ) || Contains( p, 14, 63, 251, 103 ) ||
            planet >= 0 && Contains( p, layout.Width - 243, 63, 229, 66 ) ? new( HudAction.Panel ) : default;
    }
}
