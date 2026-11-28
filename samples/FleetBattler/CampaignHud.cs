using System.Globalization;
using System.Numerics;

namespace FleetBattler;

internal readonly record struct MinorNotice( string? Text, long Time );

// A duration in simulation ticks. On the campaign calendar it reads as days, months or years of game time;
// elsewhere as real seconds at the current speed. The "c" format is a compact form for narrow buttons.
internal readonly struct GameSpan( int ticks, float speed, bool calendar ) : ISpanFormattable {
    public bool TryFormat( Span<char> destination, out int written, ReadOnlySpan<char> format, IFormatProvider? provider ) {
        bool compact = format is "c";
        var culture = CultureInfo.InvariantCulture;
        if ( !calendar ) {
            int seconds = (int)Math.Ceiling( ticks * FleetSimulation.StepSeconds / speed );
            return compact ? destination.TryWrite( culture, $"{seconds}S", out written ) : destination.TryWrite( culture, $"{seconds} SEC", out written );
        }
        double days = Pacing.DaysOf( Math.Max( 0, ticks ) );
        if ( days < 60 ) {
            int whole = Math.Max( 1, (int)Math.Ceiling( days ) );
            return compact ? destination.TryWrite( culture, $"{whole}D", out written )
                : destination.TryWrite( culture, $"{whole} {(whole == 1 ? "DAY" : "DAYS")}", out written );
        }
        if ( days < 365 ) {
            double months = days / 30.44;
            return compact ? destination.TryWrite( culture, $"{months:0.#}MO", out written ) : destination.TryWrite( culture, $"{months:0.#} MONTHS", out written );
        }
        double years = days / SolarCatalog.EarthYearDays;
        return compact ? destination.TryWrite( culture, $"{years:0.0}Y", out written ) : destination.TryWrite( culture, $"{years:0.0} YEARS", out written );
    }

    public string ToString( string? format, IFormatProvider? provider ) {
        Span<char> text = stackalloc char[24];
        return TryFormat( text, out int written, format, provider ) ? new string( text[..written] ) : "";
    }

    public override string ToString( ) => ToString( null, null );
}

internal sealed partial class FleetHud {
    // Campaign scenarios show durations on the game calendar; set by each Record call.
    private bool _calendar;
    private GameSpan TimeLeft( int ticks, float speed ) => new( ticks, speed, _calendar );

    // The star date from calendar minutes; see FleetSimulation.CalendarTick for campaign ticks.
    internal static DateOnly StarDate( int calendarMinutes ) =>
        new DateOnly( 2184, 1, 1 ).AddDays( (int)SolarCatalog.CalendarDays( calendarMinutes ) );

    private void ClockPanel( Layout layout, int campaignTick, float speed, bool paused, string mapButton ) {
        float x = layout.ClockX;
        Rect( x, 16, 184, 34, Well );
        int tick = Pacing.CalendarMinutes( campaignTick, _calendar );
        var date = StarDate( tick );
        Span<char> text = stackalloc char[24];
        int hour = (tick / 60) % 24;
        text.TryWrite( $"{date.Year:D4}.{date.Month:D2}.{date.Day:D2} {hour:D2}:00", out int length );
        FitText( text[..length], x + 39, 20, 140, 1.1f, White );
        Rect( x + 3, 19, 29, 28, Active );
        Icon( paused ? HudIcon.Play : HudIcon.Pause, x + 10, 26, 14, paused ? EnergyTint : SpaceDraw.Cyan );
        Button( x + 111, 28, 18, "-", speed > FleetClock.MinSpeed );
        text.TryWrite( $"{speed:0.###}X", out length );
        FitText( text[..length], x + 132, 36, 29, .85f, paused ? EnergyTint : SpaceDraw.Cyan );
        Button( x + 163, 28, 18, "+", speed < FleetClock.MaxSpeed );
        Text( paused ? "PAUSED" : "CAMPAIGN DATE", x + 42, 37, 0.9f, paused ? EnergyTint : Label );
        Button( layout.Width - 82, 22, 66, mapButton, true );
    }

    private static HudHit HitClock( Layout layout, Vector2 p ) {
        float x = layout.ClockX;
        if ( Contains( p, x + 3, 19, 29, 28 ) ) return new( HudAction.Pause );
        if ( Contains( p, x + 111, 28, 18, 21 ) ) return new( HudAction.Slower );
        if ( Contains( p, x + 163, 28, 18, 21 ) ) return new( HudAction.Speed );
        if ( Contains( p, layout.Width - 82, 22, 66, 21 ) ) return new( HudAction.Galaxy );
        return default;
    }

    private void DrawMinorNotices( Layout layout, ReadOnlySpan<MinorNotice> entries, long now ) {
        float width = Math.Min( 318, layout.Width - 28 );
        float x = layout.Width - width - 14, y = layout.Height - 69;
        _overlay = 1;
        foreach ( var entry in entries ) {
            if ( entry.Text is not { } text || now - entry.Time >= 5400 ) continue;
            float fade = Math.Min( 1, (5400 - (now - entry.Time)) / 1000f );
            PanelAt( x, y - 29, width, 29, Vector4.Lerp( Panel, SpaceDraw.Cyan, fade ) );
            Icon( HudIcon.Log, x + 7, y - 21, 12, Vector4.Lerp( Panel, ScienceTint, fade ) );
            FitText( text, x + 26, y - 18, width - 32, 1, Vector4.Lerp( Panel, White, fade ) );
            y -= 35;
        }
        _overlay = 0;
    }

    private void ChartSummary( Layout layout, FleetSimulation fleet, int planet ) {
        float x = 14, y = 63, width = 251;
        PanelAt( x, y, width, 103, Label );
        Icon( HudIcon.Chart, x + 10, y + 9, 14, SpaceDraw.Cyan );
        Text( "TELESCOPE CHART", x + 30, y + 11, 1.27f, White );
        WrapText( fleet.SystemIndex == FleetSimulation.Proxima
            ? "Telescopes see the primary and major bodies. Send a scout to locate the receiver and survey the system."
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
