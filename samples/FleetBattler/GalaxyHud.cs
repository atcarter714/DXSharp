using System.Numerics;
using DXSharp.Graphics;

namespace FleetBattler;

internal enum GalaxyAction { None, Return, Star, Fleet, Move, FindScout, View, Pause, Slower, Faster, Survey, RosterRow, RosterClose, RosterPrevious, RosterNext, FusionResearch }

internal readonly record struct GalaxyHit( GalaxyAction Action, int System = -1 );
internal readonly record struct GalaxyRoute( int Source, int Destination, string Status, string? Block, int Selected,
    bool InTransit = false, float Progress = 0, bool CanSurvey = false, string? Quote = null, bool RallyCrew = false,
    bool FusionResearch = false, string? Readiness = null );

internal readonly record struct GalaxyView( float Zoom, Vector2 Offset ) {
    internal static GalaxyView Overview => new( 1, Vector2.Zero );
    internal float Scale => Zoom == 0 ? 1 : Zoom;

    internal GalaxyView ZoomAt( Vector2 cursor, float amount ) {
        float current = Scale;
        float next = Math.Clamp( current * amount, 1, 4 );
        if ( next == 1 ) return Overview;
        var offset = (cursor - new Vector2( .5f )) * (1 - next / current) + Offset * (next / current);
        return new( next, Limit( offset, next ) );
    }

    internal GalaxyView Pan( Vector2 delta ) => Zoom <= 1 ? Overview : this with { Offset = Limit( Offset + delta, Zoom ) };
    private static Vector2 Limit( Vector2 offset, float zoom ) =>
        Vector2.Clamp( offset, new( -(zoom - 1) * .5f ), new( (zoom - 1) * .5f ) );
}

internal sealed partial class FleetHud {
    internal void RecordGalaxy( GraphicsCommandList commands, PipelineState pipeline, GpuResource upload,
        int currentSystem, int width, int height, bool frontier, bool charted, ReadOnlySpan<int> hulls,
        GalaxyRoute route, int campaignTick, float speed, bool paused, Briefing? briefing = null, FleetSimulation? roster = null,
        CommandSelection? selected = null, int rosterPage = 0, ReadOnlySpan<FleetTransit> transfers = default, ReadOnlySpan<MinorNotice> notices = default, long noticeNow = 0, FleetCampaign? campaign = null, GalaxyView chart = default, int hovered = -1 ) {
        if ( width < 350 || height < 240 ) return;
        _calendar = campaign?.Calendar ?? roster?.FrontierProgression ?? false;
        Begin( width, height );
        var layout = new Layout( width / _scale, height / _scale );
        PanelAt( 14, 12, layout.Width - 28, 41, SpaceDraw.Cyan );
        Icon( HudIcon.Chart, 24, 22, 16, SpaceDraw.Cyan );
        Text( "GALAXY MAP", 47, 24, 1.45f, White );
        if ( layout.Width >= 480 ) Text( $"{CelestialSystem.SystemCount} SYSTEMS / {(chart.Zoom > 1 ? $"ZOOM {chart.Zoom:F1}X" : "OVERVIEW")}", 47, 40, .85f, Label );
        if ( layout.Width >= 720 ) FitText( "WHEEL ZOOM / MIDDLE DRAG PAN / HOME RESET", layout.Width - 327, 23, 220, .9f, Label );
        ClockPanel( layout, campaignTick, speed, paused, "RETURN" );
        var map = ChartArea( layout, frontier );
        Text( "REGIONAL CHART", map.X + 8, map.Y + 8, .9f, Label );
        if ( frontier ) {
            Span<(int Source, int Destination)> shown = stackalloc (int, int)[48];
            int shownCount = 0;
            foreach ( var flight in transfers ) {
                if ( flight.Source != route.Source && flight.Destination != route.Destination ) continue;
                if ( (flight.Source, flight.Destination) == (route.Source, route.Destination) && route.InTransit ) continue;
                var lane = (flight.Source, flight.Destination);
                if ( shownCount == shown.Length || shown[..shownCount].Contains( lane ) ) continue;
                shown[shownCount++] = lane;
                float progress = (float)(campaignTick - flight.Departure) / (flight.Arrival - flight.Departure);
                DrawLane( flight.Source, flight.Destination, progress, true );
            }
            if ( route.Source != route.Destination )
                DrawLane( route.Source, route.Destination, route.Progress, route.InTransit );
        }

        void DrawLane( int source, int destination, float progress, bool flying ) {
            var from = GalaxyNode( layout, source, frontier, chart );
            var to = GalaxyNode( layout, destination, frontier, chart );
            for ( int i = 1; i < 11; ++i ) {
                var p = Vector2.Lerp( from, to, i / 11f );
                if ( InsideChart( layout, frontier, p ) )
                    Rect( p.X - 1, p.Y - 1, 2, 2, i / 11f < progress ? SpaceDraw.Cyan : Label );
            }
            if ( !flying ) return;
            var marker = Vector2.Lerp( from, to, Math.Clamp( progress, 0, 1 ) );
            if ( !InsideChart( layout, frontier, marker ) ) return;
            Icon( HudIcon.Ship, marker.X - 6, marker.Y - 6, 13, EnergyTint );
        }
        for ( int i = 0; i < CelestialSystem.Systems.Length; ++i ) {
            var center = GalaxyNode( layout, i, frontier, chart );
            if ( !InsideChart( layout, frontier, center ) ) continue;
            bool hasFleet = frontier && i < hulls.Length && hulls[i] > 0;
            bool featured = i == route.Destination || i == currentSystem || i == hovered;
            bool authored = i < CelestialSystem.AuthoredCount;
            float temperature = authored ? 5300 : Exoplanets.Get( i ).Temperature;
            var tint = featured ? SpaceDraw.Cyan : Vector4.Lerp( new( .84f, .52f, .34f, 1 ),
                new( .64f, .80f, 1, 1 ), Math.Clamp( (temperature - 3400) / 3100, 0, 1 ) );
            bool marker = featured || hasFleet || authored;
            float diameter = featured ? 17 : marker ? 11 : charted ? 8 : 7;
            _glyphs[_count++] = new( new Vector4( center.X - diameter / 2, center.Y - diameter / 2, diameter, diameter ) * _scale,
                tint, marker ? 1u : 0u, 0, 9, 0 );

            if ( featured || hasFleet || (authored && chart.Zoom >= 1.4f) ) {
                string name = i == 0 && frontier ? "SOL" : CelestialSystem.Systems[i];
                float textWidth = TextWidth( name.Length, .93f );
                float left = center.X - textWidth / 2;
                Rect( left - 4, center.Y + 10, textWidth + 8, 14, Panel, 1 );
                Text( name, left, center.Y + 13, .93f, featured ? White : Label );
            }
            if ( hasFleet ) DrawFleetBadge( i, center, hulls[i] );
        }

        void DrawFleetBadge( int i, Vector2 center, int hulls ) {
            var badge = GalaxyBadge( center );
            Rect( badge.X, badge.Y, badge.Z, badge.W, route.Source == i ? Active : Well );
            Icon( HudIcon.Ship, badge.X + 4, badge.Y + 4, 12, SpaceDraw.Cyan );
            Span<char> count = stackalloc char[6];
            hulls.TryFormat( count, out int length );
            Text( count[..length], badge.X + 20, badge.Y + 6, 1, White );
        }
        if ( frontier ) DrawGalaxyRoute( layout, route, campaign?.OccupiedSystem( route.Destination ) );
        else {
            PanelAt( 14, layout.Bottom, layout.Width - 28, 45, SpaceDraw.Cyan );
            Text( "SELECT A STAR SYSTEM TO ENTER", 23, layout.Bottom + 14, 1, Label );
        }
        if ( roster is not null && selected is not null ) {
            DrawGalaxyRoster( layout, roster, selected, rosterPage );
            if ( layout.Width >= 820 && selected.Count > 0 )
                SelectionDetails( layout, roster, selected.Ships, selected.Focus( roster ), speed, GalaxyDetailsArea( layout ) );
        }
        DrawMinorNotices( layout, notices, noticeNow );
        if ( briefing is { } prompt ) DrawBriefing( layout, prompt );
        Submit( commands, pipeline, upload, width, height );
    }

    private static string FleetOutlinerName( FleetSimulation fleet, int squad ) =>
        fleet.FleetName( squad ) ?? fleet.FirstShipName( squad ) ?? FleetSimulation.RoleName( fleet.Role( squad ) );

    private const int RosterRows = 5;
    private static Vector4 GalaxyDetailsArea( Layout layout ) => new( 14, 63, 297, Math.Min( 246, layout.Height - 84 ) );
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
        FitText( $"{roster.SystemName} / FLEET OUTLINER", x + 9, y + 9, 155, 1.15f, White );
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
            var name = FleetOutlinerName( roster, squad );
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
        float width = MathF.Min( 230, layout.Width - 28 );
        return new( layout.Width - width - 14, layout.Height - 176, width, 162 );
    }

    private static Vector4 ChartArea( Layout layout, bool frontier ) {
        float right = frontier && layout.Width >= 620 ? RouteArea( layout ).X - 24 : layout.Width - 18;
        return new( 18, 58, right - 18, MathF.Max( 1, layout.Height - 82 ) );
    }

    private static bool InsideChart( Layout layout, bool frontier, Vector2 point ) {
        var area = ChartArea( layout, frontier );
        return Contains( point, area.X, area.Y, area.Z, area.W );
    }

    internal static Vector4 GalaxyAreaPixel( int width, int height, bool frontier ) {
        float scale = HudScale( width, height );
        return ChartArea( new( width / scale, height / scale ), frontier ) * scale;
    }

    internal static Vector2 GalaxyNodePixel( int index, int width, int height, bool frontier, GalaxyView chart = default ) {
        float scale = HudScale( width, height );
        return GalaxyNode( new( width / scale, height / scale ), index, frontier, chart ) * scale;
    }

    private static Vector2 GalaxyNode( Layout layout, int index, bool frontier, GalaxyView chart ) {
        var area = ChartArea( layout, frontier );
        float diameter = MathF.Min( area.Z, area.W );
        var diskSize = new Vector2( diameter * 1.17f, diameter * .98f );
        var center = new Vector2( area.X + area.Z / 2, area.Y + area.W / 2 );
        var offset = chart.Offset * new Vector2( layout.Width, layout.Height );
        return center + (CelestialSystem.MapPositions[index] - new Vector2( .5f )) * diskSize * chart.Scale + offset;
    }

    private static Vector4 GalaxyBadge( Vector2 center ) => new( center.X + 12, center.Y - 10, 40, 21 );

    private static Vector4 RouteButton( Vector4 panel, int index ) =>
        new( panel.X + 9 + (index % 2) * ((panel.Z - 13) / 2), panel.Y + 103 + 25 * (index / 2), (panel.Z - 24) / 2, 21 );

    private void DrawGalaxyRoute( Layout layout, GalaxyRoute route, FleetSimulation? colony ) {
        var panel = RouteArea( layout );
        float x = panel.X, y = panel.Y, w = panel.Z;
        _overlay = 2;
        PanelAt( x, y, w, panel.W, SpaceDraw.Cyan );
        Icon( HudIcon.Chart, x + 10, y + 8, 16, SpaceDraw.Cyan );
        string destination = route.Destination == 0 ? "SOL" : CelestialSystem.Systems[route.Destination];
        string source = route.Source == 0 ? "SOL" : CelestialSystem.Systems[route.Source];
        FitText( route.RallyCrew ? $"FIRST CROSSING / {destination}" : destination, x + 33, y + 10, w - 45, 1.5f, White );
        string summary = colony is not null && route.Source == route.Destination
            ? $"LOCAL / {colony.Ore} ORE   {colony.Energy} ENERGY   {colony.Science} SCI"
            : $"FROM {source} / {route.Selected} HULLS SELECTED";
        FitText( summary, x + 10, y + 32, w - 20, .95f, Label );
        if ( route.InTransit ) {
            FitText( route.Status, x + 10, y + 52, w - 20, 1, Label );
            Progress( new( x + 10, y + 74 ), w - 20, route.Progress, SpaceDraw.Cyan );
        } else {
            FitText( route.Quote ?? "LOCAL SYSTEM", x + 10, y + 50, w - 20, 1, EnergyTint );
            WrapText( route.Block ?? "READY TO DEPART", x + 10, y + 68, w - 20, 22, 1, route.Block is null ? SpaceDraw.Cyan : Label );
            if ( route.Readiness is { } readiness ) FitText( readiness, x + 10, y + 91, w - 20, .86f, Label );
            for ( int i = 0; i < 4; ++i ) {
                var button = RouteButton( panel, i );
                var action = RouteAction( route, i );
                GalaxyButton( button.X, button.Y, button.Z, action, route.RallyCrew, action != GalaxyAction.Move || route.Block is null );
            }
        }
        _overlay = 0;
    }

    private void GalaxyButton( float x, float y, float width, GalaxyAction action, bool rallyCrew, bool enabled ) {
        var (icon, label, narrow) = action switch {
            GalaxyAction.FindScout => rallyCrew ? (HudIcon.Crew, "RALLY CREW", "CREW" ) : (HudIcon.Crew, "FIND SCOUT", "SCOUT"),
            GalaxyAction.Move => rallyCrew ? (HudIcon.Launch, "LAUNCH EXPEDITION", "LAUNCH") : (HudIcon.Launch, "MOVE FLEET", "MOVE"),
            GalaxyAction.Survey => (HudIcon.Science, "SURVEY", "SURVEY"),
            GalaxyAction.FusionResearch => (HudIcon.Science, "FUSION RESEARCH", "RESEARCH"),
            GalaxyAction.View => (HudIcon.Chart, "VIEW SYSTEM", "VIEW"),
            _ => (HudIcon.Ship, "", "")
        };
        if ( label.Length == 0 ) return;
        IconButton( x, y, width, icon, TextWidth( label.Length, 1 ) <= width - 22 ? label : narrow,
            enabled: enabled, active: enabled && action == GalaxyAction.Move );
    }

    private static GalaxyAction RouteAction( GalaxyRoute route, int index ) => index switch {
        0 => GalaxyAction.View,
        1 => GalaxyAction.FindScout,
        2 when route.Destination != route.Source => GalaxyAction.Move,
        3 when route.CanSurvey => GalaxyAction.Survey,
        3 when route.FusionResearch => GalaxyAction.FusionResearch,
        _ => GalaxyAction.None
    };

    internal static GalaxyHit HitGalaxy( Vector2 pixel, int width, int height, bool frontier, ReadOnlySpan<int> hulls, GalaxyRoute route, FleetSimulation? roster = null, int rosterPage = 0, GalaxyView chart = default ) {
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
        var details = GalaxyDetailsArea( layout );
        if ( roster is not null && route.Selected > 0 && layout.Width >= 820 &&
             Contains( p, details.X, details.Y, details.Z, details.W ) ) return default;
        if ( frontier ) {
            var panel = RouteArea( layout );
            if ( Contains( p, panel.X, panel.Y, panel.Z, panel.W ) ) {
                if ( !route.InTransit ) {
                    for ( int index = 0; index < 4; ++index ) {
                        var button = RouteButton( panel, index );
                        if ( !Contains( p, button.X, button.Y, button.Z, button.W ) ) continue;
                        var action = RouteAction( route, index );
                        return action == GalaxyAction.Move && route.Block is not null ? default : new( action );
                    }
                }
                return default;
            }
        }
        if ( frontier ) {
            for ( int i = 0; i < hulls.Length; ++i ) {
                var badge = GalaxyBadge( GalaxyNode( layout, i, frontier, chart ) );
                if ( hulls[i] > 0 && Contains( p, badge.X, badge.Y, badge.Z, badge.W ) ) return new( GalaxyAction.Fleet, i );
            }
        }
        int closest = -1;
        float distance = float.PositiveInfinity;
        for ( int i = 0; i < CelestialSystem.SystemCount; ++i ) {
            var center = GalaxyNode( layout, i, frontier, chart );
            if ( !InsideChart( layout, frontier, center ) ) continue;
            float candidate = Vector2.DistanceSquared( p, center );
            float reach = i == route.Destination ? 14 : 10;
            if ( candidate <= reach * reach && candidate < distance ) { closest = i; distance = candidate; }
        }
        if ( closest >= 0 ) return new( GalaxyAction.Star, closest );
        return default;
    }
}
