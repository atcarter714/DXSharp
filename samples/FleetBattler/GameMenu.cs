using System.Numerics;
using DXSharp.Graphics;

namespace FleetBattler;

internal sealed partial class FleetHud {
    internal static SampledTexture2D LoadMenuArtwork( GraphicsDevice device, GraphicsCommandList commands ) {
        using var input = File.OpenRead( Path.Combine( AppContext.BaseDirectory, "Art", "FlightDeck.jpg" ) );
        var image = StbImageSharp.ImageResult.FromStream( input, StbImageSharp.ColorComponents.RedGreenBlueAlpha );
        return device.CreateSampledTexture2D( commands, (uint)image.Width, (uint)image.Height, image.Data, srgb: true );
    }

    private static (float X, float Y, float Width) MenuLayout( int width, int height ) {
        float scale = HudScale( width, height );
        float w = width / scale, h = height / scale;
        return (w * 0.075f, Math.Max( 12, (h - 350) / 2 ), Math.Min( 310, w * 0.54f ));
    }

    internal static bool MenuRightHalf( int width, int height, int pixelX ) {
        var (x, _, w) = MenuLayout( width, height );
        return pixelX / HudScale( width, height ) >= x + w / 2;
    }

    internal static int MenuHit( int width, int height, Vector2 pixel, int count ) {
        var p = pixel / HudScale( width, height );
        var (x, y, w) = MenuLayout( width, height );
        float row = p.Y - y - 64;
        return p.X >= x && p.X < x + w && row >= 0 && row % 35 < 30 && row / 35 < count ? (int)(row / 35) : -1;
    }

    internal void DrawMenu( GraphicsCommandList commands, PipelineState pipeline, GpuResource upload, SampledTexture2D artwork, int width, int height,
        GameScreen screen, string scenario, string description, ReadOnlySpan<string> actions, int focus, string quote ) {
        Begin( width, height );
        var (x, y, w) = MenuLayout( width, height );
        float right = x + w + 22, available = width / _scale - right - x;
        bool loading = screen == GameScreen.Loading;
        Text( screen == GameScreen.PauseMenu ? "FLIGHT PAUSED" : "FLEET BATTLER", x, y, 3.1f, White );
        FitText( scenario, x, y + 38, w, 1.3f, SpaceDraw.Cyan );
        float side = Math.Min( available, 236 );
        _glyphs[_count++] = new( new Vector4( right, y, side, side ) * _scale, White, screen == GameScreen.PauseMenu ? 1u : 0u, 0, 8, 0 );
        WrapText( loading ? quote : description, right, y + side + 13, available, 110, 1.15f, Label );
        if ( loading ) Text( "PREPARING FLIGHT", x, y + 78, 1.6f, White );
        else for ( int i = 0; i < actions.Length; ++i ) {
            float top = y + 64 + 35 * i;
            Rect( x, top, w, 30, i == focus ? Active : Well );
            if ( i == focus ) Rect( x, top, 3, 30, SpaceDraw.Cyan, 3 );
            FitText( actions[i], x + 12, top + 10, w - 23, 1.3f, i == focus ? White : Label );
        }
        if ( screen == GameScreen.MainMenu ) WrapText( quote, x, y + 351, width / _scale - 2*x, 45, 1, Label );
        Submit( commands, pipeline, upload, width, height, artwork );
    }
}
