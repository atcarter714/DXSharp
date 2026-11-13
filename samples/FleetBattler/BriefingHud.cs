using System.Numerics;

namespace FleetBattler;

internal readonly record struct Briefing( string Title, string Source, string Body, ContentArt Artwork,
    string Accept, string Dismiss, string AcceptEffect = "", string DismissEffect = "" );

internal sealed partial class FleetHud {
    private static Vector4 BriefingArea( Layout layout ) {
        float width = MathF.Min( 504, layout.Width - 28 );
        float height = MathF.Min( 370, layout.Height - 22 );
        return new( (layout.Width - width) / 2, (layout.Height - height) / 2, width, height );
    }

    private void DrawBriefing( Layout layout, Briefing briefing ) {
        var area = BriefingArea( layout );
        float x = area.X, y = area.Y, width = area.Z, height = area.W;
        bool compact = height < 300;
        float artHeight = compact ? 50 : 112;
        _overlay = 3;
        PanelAt( x, y, width, height, EnergyTint );
        Rect( x + 2, y + 1, width - 3, 45, Well );
        Icon( HudIcon.Log, x + 13, y + 11, 15, EnergyTint );
        FitText( briefing.Title, x + 36, y + 12, width - 48, 1.6f, White );
        FitText( briefing.Source, x + 36, y + 31, width - 48, 1, Label );
        Art( (Tile)briefing.Artwork, x + 12, y + 53, width - 24, artHeight, crop: true );
        float bodyY = y + 62 + artHeight, buttonY = y + height - 80;
        WrapText( briefing.Body, x + 14, bodyY, width - 28, buttonY - bodyY - 5, 1, White );
        Choice( buttonY, briefing.Accept, briefing.AcceptEffect, EnergyTint );
        Choice( buttonY + 34, briefing.Dismiss, briefing.DismissEffect, SpaceDraw.Cyan );
        _overlay = 0;

        void Choice( float top, string name, string effect, Vector4 tint ) {
            Rect( x + 12, top, width - 24, 29, Well, 2 );
            Rect( x + 12, top, 2, 29, tint, 3 );
            FitText( name, x + 21, top + (effect.Length == 0 ? 10 : 5), width - 40, 1.05f, tint );
            if ( effect.Length > 0 ) FitText( effect, x + 21, top + 17, width - 40, 0.9f, Label );
        }
    }

    private static HudHit HitBriefing( Layout layout, Vector2 p ) {
        var area = BriefingArea( layout );
        if ( Contains( p, area.X + 12, area.Y + area.W - 80, area.Z - 24, 29 ) ) return new( HudAction.Briefing, 1 );
        if ( Contains( p, area.X + 12, area.Y + area.W - 46, area.Z - 24, 29 ) ) return new( HudAction.Briefing, 0 );
        return new( HudAction.Panel );
    }

    private void FitText( ReadOnlySpan<char> text, float x, float y, float width, float size, Vector4 tint ) {
        int cells = Math.Max( 0, (int)(width / TextWidth( 1, size )) );
        Text( text[..Math.Min( cells, text.Length )], x, y, size, tint );
    }

    private void WrapText( ReadOnlySpan<char> text, float x, float y, float width, float height, float size, Vector4 tint ) {
        int cells = Math.Max( 1, (int)(width / TextWidth( 1, size )) );
        float line = 7 * GlyphPitch( size ) / _scale + 4;
        for ( float row = 0; row + line <= height && !text.IsEmpty; row += line ) {
            int hardBreak = text.IndexOf( '\n' );
            if ( hardBreak < 0 ) hardBreak = text.Length;
            int count = Math.Min( cells, hardBreak );
            if ( hardBreak > cells && text[..count].LastIndexOf( ' ' ) is > 0 and var word ) count = word;
            Text( text[..count], x, y + row, size, tint );
            text = text[(count == hardBreak && count < text.Length ? count + 1 : count)..].TrimStart( ' ' );
        }
    }
}
