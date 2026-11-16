using System.Numerics;
using DXSharp.Graphics;

namespace FleetBattler;

internal enum GalaxyAction { None, Return, Star, Fleet, Move, FindScout, View, Pause, Slower, Faster, Survey, RosterRow, RosterClose, RosterPrevious, RosterNext }

internal readonly record struct GalaxyHit( GalaxyAction Action, int System = -1 );
internal readonly record struct GalaxyRoute( int Source, int Destination, string Status, string? Block, int Selected,
    bool InTransit = false, float Progress = 0, bool CanSurvey = false, string? Quote = null );

internal sealed partial class FleetHud {
    internal void RecordGalaxy( GraphicsCommandList commands, PipelineState pipeline, GpuResource upload,
        int currentSystem, int width, int height, bool frontier, bool charted, ReadOnlySpan<int> hulls,
        GalaxyRoute route, int campaignTick, int speed, bool paused, Briefing? briefing = null, FleetSimulation? roster = null,
        CommandSelection? selected = null, int rosterPage = 0, ReadOnlySpan<FleetTransit> transfers = default, ReadOnlySpan<MinorNotice> notices = default, long noticeNow = 0 ) {
        if ( width < 350 || height < 240 ) return;
        Begin( width, height );
        var layout = new Layout( width / _scale, height / _scale );
        PanelAt( 14, 12, layout.Width - 28, 41, SpaceDraw.Cyan );
        Icon( HudIcon.Chart, 24, 22, 16, SpaceDraw.Cyan );
        Text( "GALAXY MAP", 47, 24, 1.45f, White );
        ClockPanel( layout, campaignTick, speed, paused, "RETURN" );
        if ( frontier ) {
            int systems = CelestialSystem.Systems.Length;
            Span<float> lanes = stackalloc float[systems * systems];
            lanes.Fill( -1 );
            foreach ( var flight in transfers ) {
                int index = flight.Source * systems + flight.Destination;
                float progress = (float)(campaignTick - flight.Departure) / (flight.Arrival - flight.Departure);
                lanes[index] = Math.Max( lanes[index], progress );
            }
            for ( int i = 0; i < lanes.Length; ++i )
                if ( lanes[i] >= 0 ) DrawLane( i / systems, i % systems, lanes[i], true );
            if ( route.Source != route.Destination && !route.InTransit )
                DrawLane( route.Source, route.Destination, 0, false );
        }

        void DrawLane( int source, int destination, float progress, bool flying ) {
            var from = GalaxyNode( layout, source, frontier );
            var to = GalaxyNode( layout, destination, frontier );
            for ( int i = 3; i < 29; ++i ) {
                var p = Vector2.Lerp( from, to, i / 32f );
                Rect( p.X - 1, p.Y - 1, 3, 3, i / 32f < progress ? SpaceDraw.Cyan : Well );
            }
            if ( !flying ) return;
            var marker = Vector2.Lerp( from, to, Math.Clamp( progress, 0, 1 ) );
            Icon( HudIcon.Ship, marker.X - 6, marker.Y - 6, 13, EnergyTint );
        }
        Span<char> count = stackalloc char[6];
        for ( int i = 0; i < CelestialSystem.Systems.Length; ++i ) {
            var center = GalaxyNode( layout, i, frontier );
            bool known = !frontier || i == 0 || charted;
            string name = i == 0 && frontier ? "SOL" : CelestialSystem.Systems[i];
            float textWidth = TextWidth( name.Length, 1.2f );
            float left = center.X - textWidth / 2;
            Rect( left - 6, center.Y + 19, textWidth + 12, 20, Panel, 1 );
            Text( name, left, center.Y + 24, 1.2f, i == route.Destination || i == currentSystem ? SpaceDraw.Cyan : known ? White : Disabled );
            if ( !frontier || hulls[i] == 0 ) continue;
            var badge = GalaxyBadge( center );
            Rect( badge.X, badge.Y, badge.Z, badge.W, route.Source == i ? Active : Well );
            Icon( HudIcon.Ship, badge.X + 4, badge.Y + 4, 12, SpaceDraw.Cyan );
            hulls[i].TryFormat( count, out int length );
            Text( count[..length], badge.X + 20, badge.Y + 6, 1, White );
        }
        if ( frontier ) DrawGalaxyRoute( layout, route );
        else {
            PanelAt( 14, layout.Bottom, layout.Width - 28, 45, SpaceDraw.Cyan );
            Text( "SELECT A STAR SYSTEM TO ENTER", 23, layout.Bottom + 14, 1, Label );
        }
        if ( roster is not null && selected is not null ) DrawGalaxyRoster( layout, roster, selected, rosterPage );
        DrawMinorNotices( layout, notices, noticeNow );
        if ( briefing is { } prompt ) DrawBriefing( layout, prompt );
        Submit( commands, pipeline, upload, width, height );
    }

    private const int RosterRows = 5;
    private static Vector4 RosterArea( Layout layout ) => new( layout.Width - 214, 62, 200, 181 );
    private static int RosterSize( FleetSimulation roster ) {
        int count = 0;
        for ( int squad = 0; squad < roster.SquadCount; ++squad )
            if ( roster.IsPlayerSquad( squad ) && roster.IsFleetLeader( squad ) ) ++count;
        return count;
    }

    private static int RosterSquad( FleetSimulation roster, int index ) {
        for ( int squad = 0; squad < roster.SquadCount; ++squad )
            if ( roster.IsPlayerSquad( squad ) && roster.IsFleetLeader( squad ) && index-- == 0 ) return squad;
        return -1;
    }

    private void DrawGalaxyRoster( Layout layout, FleetSimulation roster, CommandSelection selected, int page ) {
        var area = RosterArea( layout );
        float x = area.X, y = area.Y;
        _overlay = 1;
        PanelAt( x, y, area.Z, area.W, SpaceDraw.Cyan );
        FitText( roster.SystemIndex == 0 ? "SOL / FLEETS" : $"{roster.SystemName} / FLEETS", x + 9, y + 9, 147, 1.15f, White );
        Button( x + 172, y + 4, 22, "X", true );
        Span<char> number = stackalloc char[6];
        for ( int row = 0; row < RosterRows; ++row ) {
            int squad = RosterSquad( roster, page * RosterRows + row );
            if ( squad < 0 ) break;
            float top = y + 33 + row * 23;
            int id = roster.FleetId( squad );
            bool active = false, idle = true;
            for ( int ship = 0; ship < roster.Count; ++ship ) {
                if ( !roster.Alive( ship ) || roster.FleetId( roster.SquadronOf( ship ) ) != id ) continue;
                active |= selected.Ships[ship];
                idle &= roster.TravelReady( ship );
            }
            Rect( x + 7, top, 186, 20, active ? Active : Well );
            Icon( idle ? HudIcon.Ship : HudIcon.Orders, x + 12, top + 5, 10, idle ? SpaceDraw.Cyan : EnergyTint );
            var name = roster.FleetName( squad ) ?? roster.FirstShipName( squad ) ?? FleetSimulation.RoleName( roster.Role( squad ) );
            FitText( name, x + 27, top + 7, 135, 1, White );
            roster.FleetStrength( squad ).TryFormat( number, out int length );
            Text( number[..length], x + 167, top + 7, 1, Label );
        }
        Button( x + 9, y + 151, 28, "-", page > 0 );
        Button( x + 163, y + 151, 28, "+", (page + 1) * RosterRows < RosterSize( roster ) );
        Text( "SHIFT ADD / CTRL TOGGLE", x + 39, y + 160, 0.9f, Label );
        _overlay = 0;
    }

    private static GalaxyHit HitGalaxyRoster( Layout layout, Vector2 p, FleetSimulation roster, int page ) {
        var a = RosterArea( layout );
        if ( Contains( p, a.X + 172, a.Y + 4, 22, 21 ) ) return new( GalaxyAction.RosterClose );
        if ( page > 0 && Contains( p, a.X + 9, a.Y + 151, 28, 21 ) ) return new( GalaxyAction.RosterPrevious );
        if ( (page + 1) * RosterRows < RosterSize( roster ) && Contains( p, a.X + 163, a.Y + 151, 28, 21 ) ) return new( GalaxyAction.RosterNext );
        for ( int row = 0; row < RosterRows; ++row )
            if ( Contains( p, a.X + 7, a.Y + 33 + row * 23, 186, 20 ) && RosterSquad( roster, page * RosterRows + row ) is >= 0 and var squad )
                return new( GalaxyAction.RosterRow, squad );
        return default;
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
        Icon( route.Destination == FleetSimulation.Proxima ? HudIcon.Route : HudIcon.Chart, x + 10, y + 8, 16, SpaceDraw.Cyan );
        string source = route.Source == 0 ? "SOL" : CelestialSystem.Systems[route.Source];
        string destination = route.Destination == 0 ? "SOL" : CelestialSystem.Systems[route.Destination];
        FitText( route.Source == route.Destination ? $"{source} / SYSTEM CHART" : $"{source} TO {destination}",
            x + 33, y + 9, w - 47, 1.3f, White );
        FitText( route.Status, x + 11, y + 33, w - 22, 1, Label );
        if ( route.InTransit ) {
            Progress( new( x + 12, y + 61 ), w - 24, route.Progress, SpaceDraw.Cyan );
        } else {
            if ( route.Quote is { } quote ) FitText( quote, x + 11, y + 50, w - 22, 1, EnergyTint );
            FitText( route.Block ?? $"READY / {route.Selected} SHIPS SELECTED", x + 11, y + 69, w - 22, 1,
                route.Block is null ? SpaceDraw.Cyan : White );
        }

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
        var (icon, label, narrow) = action switch {
            GalaxyAction.FindScout => (HudIcon.Crew, "FIND SCOUT", "SCOUT"),
            GalaxyAction.Move => (HudIcon.Launch, "MOVE FLEET", "MOVE"),
            GalaxyAction.Survey => (HudIcon.Science, "SURVEY", "SURVEY"),
            GalaxyAction.View => (HudIcon.Chart, "VIEW SYSTEM", "VIEW"),
            _ => (HudIcon.Ship, "", "")
        };
        if ( label.Length == 0 ) return;
        IconButton( x, y, width, icon, TextWidth( label.Length, 1 ) <= width - 22 ? label : narrow,
            enabled: true, active: action == GalaxyAction.Move );
    }

    private static (GalaxyAction First, GalaxyAction Second, GalaxyAction Third, GalaxyAction Fourth) RouteActions( GalaxyRoute route ) =>
        (GalaxyAction.FindScout, route.CanSurvey ? GalaxyAction.Survey : GalaxyAction.None,
            route.Block is null && !route.InTransit ? GalaxyAction.Move : GalaxyAction.None, GalaxyAction.View);

    internal static GalaxyHit HitGalaxy( Vector2 pixel, int width, int height, bool frontier, ReadOnlySpan<int> hulls, GalaxyRoute route, FleetSimulation? roster = null, int rosterPage = 0 ) {
        if ( width < 350 || height < 240 ) return default;
        float scale = HudScale( width, height );
        var layout = new Layout( width / scale, height / scale );
        var p = pixel / scale;
        var clock = HitClock( layout, p ).Action;
        if ( clock != HudAction.None ) return new( clock switch {
            HudAction.Galaxy => GalaxyAction.Return, HudAction.Pause => GalaxyAction.Pause,
            HudAction.Slower => GalaxyAction.Slower, _ => GalaxyAction.Faster
        } );
        var rosterArea = RosterArea( layout );
        if ( roster is not null && Contains( p, rosterArea.X, rosterArea.Y, rosterArea.Z, rosterArea.W ) )
            return HitGalaxyRoster( layout, p, roster, rosterPage );
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
            if ( Vector2.DistanceSquared( p, center ) <= 23 * 23 || Contains( p, center.X - 49, center.Y + 18, 98, 22 ) ) return new( GalaxyAction.Star, i );
        }
        return default;
    }
}
