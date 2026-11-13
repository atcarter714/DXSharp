using System.Numerics;

namespace FleetBattler;

internal readonly record struct Briefing( string Title, string First, string Second, string Accept, string Dismiss );

internal sealed partial class FleetHud {
    private const float BriefingWidth = 338, BriefingHeight = 122;

    private static Vector2 BriefingOrigin( Layout layout ) =>
        new( (layout.Width - BriefingWidth) / 2, (layout.Height - BriefingHeight) / 2 );

    private void DrawBriefing( Layout layout, Briefing briefing ) {
        var at = BriefingOrigin( layout );
        _overlay = 3;
        PanelAt( at.X, at.Y, BriefingWidth, BriefingHeight, EnergyTint );
        Text( briefing.Title, at.X + 12, at.Y + 11, 1.3f, White );
        Text( briefing.First, at.X + 12, at.Y + 37, 1, Label );
        Text( briefing.Second, at.X + 12, at.Y + 55, 1, Label );
        Button( at.X + 12, at.Y + 88, 160, briefing.Accept, true );
        Button( at.X + 179, at.Y + 88, 147, briefing.Dismiss, true );
        _overlay = 0;
    }

    private static HudHit HitBriefing( Layout layout, Vector2 p ) {
        var at = BriefingOrigin( layout );
        if ( Contains( p, at.X + 12, at.Y + 88, 160, 21 ) ) return new( HudAction.Briefing, 1 );
        if ( Contains( p, at.X + 179, at.Y + 88, 147, 21 ) ) return new( HudAction.Briefing, 0 );
        return new( HudAction.Panel );
    }
}
