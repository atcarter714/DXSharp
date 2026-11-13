using System.Numerics;
using DXSharp.Graphics;

namespace FleetBattler;

internal enum GalaxyAction { None, Return, Star, Fleet, Launch, SelectCrew, Rally, View, Pause, Slower, Faster, Research }

internal readonly record struct GalaxyHit( GalaxyAction Action, int System = -1 );
internal readonly record struct GalaxyRoute( int Source, int Destination, string Status, string? Block, int Selected,
    bool Charted, bool InTransit, bool Arrived, float Progress, bool CanRally, bool CanResearch = false );

internal sealed partial class FleetHud {
    internal void RecordGalaxy( GraphicsCommandList commands, PipelineState pipeline, GpuResource upload,
        int currentSystem, int width, int height, bool frontier, bool charted, ReadOnlySpan<int> hulls,
        GalaxyRoute route, int campaignTick, int speed, bool paused ) {
        if ( width < 350 || height < 240 ) return;
        Begin( width, height );
        var layout = new Layout( width / _scale, height / _scale );
        PanelAt( 14, 12, layout.Width - 28, 41, SpaceDraw.Cyan );
        Text( "GALAXY MAP", 23, 24, 1.45f, White );
        ClockPanel( layout, campaignTick, speed, paused, "RETURN" );
        if ( frontier ) {
            var from = GalaxyNode( layout, 0, frontier );
            var to = GalaxyNode( layout, FleetSimulation.Proxima, frontier );
            if ( charted || route.Destination == FleetSimulation.Proxima ) {
                for ( int i = 2; i < 30; ++i ) {
                    var p = Vector2.Lerp( from, to, i / 32f );
                    Rect( p.X - 1, p.Y - 1, 3, 3, i / 32f < route.Progress ? SpaceDraw.Cyan : Well );
                }
                if ( route.InTransit ) {
                    var p = Vector2.Lerp( from, to, route.Progress );
                    Icon( HudIcon.Ship, p.X - 6, p.Y - 6, 13, EnergyTint );
                }
            }
        }
        for ( int i = 0; i < CelestialSystem.Systems.Length; ++i ) {
            var center = GalaxyNode( layout, i, frontier );
            bool known = !frontier || i == 0 || charted;
            string name = !known ? "UNCHARTED" : i == 0 && frontier ? "SOL" : CelestialSystem.Systems[i];
            float textWidth = TextWidth( name.Length, 1.2f );
            float left = center.X - textWidth / 2;
            Rect( left - 6, center.Y + 19, textWidth + 12, 20, Panel, 1 );
            Text( name, left, center.Y + 24, 1.2f, i == route.Destination || i == currentSystem ? SpaceDraw.Cyan : known ? White : Disabled );
            if ( !frontier || hulls[i] == 0 ) continue;
            var badge = GalaxyBadge( center );
            Rect( badge.X, badge.Y, badge.Z, badge.W, route.Source == i ? Active : Well );
            Icon( HudIcon.Ship, badge.X + 4, badge.Y + 4, 12, SpaceDraw.Cyan );
            Text( hulls[i].ToString( ), badge.X + 20, badge.Y + 6, 1, White );
        }
        if ( frontier ) DrawGalaxyRoute( layout, route );
        else {
            PanelAt( 14, layout.Bottom, layout.Width - 28, 45, SpaceDraw.Cyan );
            Text( "SELECT A STAR SYSTEM TO ENTER", 23, layout.Bottom + 14, 1, Label );
        }
        Submit( commands, pipeline, upload, width, height );
    }

    private static Vector4 RouteArea( Layout layout ) {
        float width = MathF.Min( 490, layout.Width - 28 );
        return new( 14, layout.Height - 143, width, 129 );
    }

    internal static Vector2 GalaxyNodePixel( int index, int width, int height, bool frontier ) {
        float scale = HudScale( width, height );
        return GalaxyNode( new( width / scale, height / scale ), index, frontier ) * scale;
    }

    private static Vector2 GalaxyNode( Layout layout, int index, bool frontier ) {
        var position = CelestialSystem.MapPositions[index];
        float y = frontier ? 54 + position.Y * (layout.Height - 232) : position.Y * layout.Height;
        return new( position.X * layout.Width, y );
    }

    private static Vector4 GalaxyBadge( Vector2 center ) => new( center.X + 24, center.Y - 15, 40, 21 );

    private void DrawGalaxyRoute( Layout layout, GalaxyRoute route ) {
        var panel = RouteArea( layout );
        float x = panel.X, y = panel.Y, w = panel.Z;
        _overlay = 2;
        PanelAt( x, y, w, panel.W, SpaceDraw.Cyan );
        Icon( HudIcon.Ship, x + 10, y + 8, 16, SpaceDraw.Cyan );
        FitText( route.Destination == FleetSimulation.Proxima ? "EXPEDITION / SOL TO PROXIMA" :
            $"{CelestialSystem.Systems[route.Destination]} / SYSTEM CHART", x + 33, y + 9, w - 47, 1.3f, White );
        FitText( route.Status, x + 11, y + 33, w - 22, 1, Label );
        if ( route.InTransit ) {
            Progress( new( x + 12, y + 61 ), w - 24, route.Progress, SpaceDraw.Cyan );
        } else if ( route.Destination == FleetSimulation.Proxima && !route.Arrived ) {
            var cost = FleetSimulation.CrossingCost;
            FitText( $"ONE WAY / {cost.Ore} ORE / {cost.Energy} ENERGY / 120 SEC AT 1X", x + 11, y + 50, w - 22, 1, EnergyTint );
            FitText( route.Block ?? $"READY TO DEPART / {route.Selected} SHIPS SELECTED", x + 11, y + 69, w - 22, 1, route.Block is null ? SpaceDraw.Cyan : White );
        } else FitText( route.Block ?? "FLEET IN LOCAL ORBIT", x + 11, y + 58, w - 22, 1, Label );

        if ( !route.InTransit ) {
            float button = (w - 35) / 4;
            var actions = RouteActions( route );
            GalaxyButton( x + 10, y + 97, button, actions.First );
            GalaxyButton( x + 15 + button, y + 97, button, actions.Second );
            GalaxyButton( x + 20 + 2 * button, y + 97, button, actions.Third );
            GalaxyButton( x + 25 + 3 * button, y + 97, button, actions.Fourth );
        }
        _overlay = 0;
    }

    private void GalaxyButton( float x, float y, float width, GalaxyAction action ) {
        string label = action switch {
            GalaxyAction.SelectCrew => "SELECT CREW", GalaxyAction.Rally => "RALLY TO EARTH",
            GalaxyAction.Launch => "LAUNCH ONE WAY", GalaxyAction.View => "VIEW SYSTEM", GalaxyAction.Research => "OPEN RESEARCH", _ => ""
        };
        if ( label.Length == 0 ) return;
        Rect( x, y, width, 21, action == GalaxyAction.Launch ? Active : Well );
        FitText( label, x + 6, y + 7, width - 12, 1, SpaceDraw.Cyan );
    }

    private static (GalaxyAction First, GalaxyAction Second, GalaxyAction Third, GalaxyAction Fourth) RouteActions( GalaxyRoute route ) {
        if ( route.InTransit ) return default;
        if ( route.Destination != FleetSimulation.Proxima || route.Arrived )
            return (route.Charted ? GalaxyAction.View : GalaxyAction.None,
                route.CanResearch ? GalaxyAction.Research : GalaxyAction.None, GalaxyAction.None, GalaxyAction.None);
        return (route.Source == 0 ? GalaxyAction.SelectCrew : GalaxyAction.None,
            route.CanRally ? GalaxyAction.Rally : GalaxyAction.None,
            route.CanResearch ? GalaxyAction.Research : route.Block is null ? GalaxyAction.Launch : GalaxyAction.None,
            route.Charted ? GalaxyAction.View : GalaxyAction.None);
    }

    internal static GalaxyHit HitGalaxy( Vector2 pixel, int width, int height, bool frontier, ReadOnlySpan<int> hulls, GalaxyRoute route ) {
        if ( width < 350 || height < 240 ) return default;
        float scale = HudScale( width, height );
        var layout = new Layout( width / scale, height / scale );
        var p = pixel / scale;
        var clock = HitClock( layout, p ).Action;
        if ( clock != HudAction.None ) return new( clock switch {
            HudAction.Galaxy => GalaxyAction.Return, HudAction.Pause => GalaxyAction.Pause,
            HudAction.Slower => GalaxyAction.Slower, _ => GalaxyAction.Faster
        } );
        if ( frontier ) {
            var panel = RouteArea( layout );
            if ( Contains( p, panel.X, panel.Y, panel.Z, panel.W ) ) {
                float button = (panel.Z - 35) / 4;
                var actions = RouteActions( route );
                if ( Contains( p, panel.X + 10, panel.Y + 97, button, 21 ) ) return new( actions.First );
                if ( Contains( p, panel.X + 15 + button, panel.Y + 97, button, 21 ) ) return new( actions.Second );
                if ( Contains( p, panel.X + 20 + 2 * button, panel.Y + 97, button, 21 ) ) return new( actions.Third );
                if ( Contains( p, panel.X + 25 + 3 * button, panel.Y + 97, button, 21 ) ) return new( actions.Fourth );
                return default;
            }
        }
        if ( frontier ) {
            for ( int i = 0; i < hulls.Length; ++i ) {
                var badge = GalaxyBadge( GalaxyNode( layout, i, frontier ) );
                if ( hulls[i] > 0 && Contains( p, badge.X, badge.Y, badge.Z, badge.W ) ) return new( GalaxyAction.Fleet, i );
            }
        }
        for ( int i = 0; i < CelestialSystem.MapPositions.Length; ++i ) {
            var center = GalaxyNode( layout, i, frontier );
            if ( Vector2.DistanceSquared( p, center ) <= 23 * 23 ) return new( GalaxyAction.Star, i );
        }
        return default;
    }
}
