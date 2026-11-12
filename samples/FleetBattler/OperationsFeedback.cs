using System.Numerics;

namespace FleetBattler;

internal readonly record struct MapSite( Vector2 Pixel, int Body, PlanetFacility Facility, int Remaining, int Duration, bool Producing = false );
internal enum HoverKind { None, Ship, Site, Body }
internal readonly record struct MapHover( HoverKind Kind, int Index, Vector2 Target, Vector2 Pointer );

internal sealed partial class FleetHud {
    internal static string SiteName( PlanetFacility facility ) => FleetSimulation.Facility( facility ).Name;

    private void OperationsFeedback( Layout layout, FleetSimulation fleet, ReadOnlySpan<Vector4> bodies,
        ReadOnlySpan<MapSite> sites, MapHover hover, int speed, int planet ) {
        foreach ( var site in sites ) {
            if ( site.Remaining <= 0 ) {
                if ( site.Facility == PlanetFacility.Yard )
                    MapLabel( site.Pixel, fleet.YardAt( site.Body ) == 0 ? "SHIPYARD" : "CIVILIAN DOCK", layout, fleet, planet );
                continue;
            }
            var point = site.Pixel / _scale;
            if ( point.X < 28 || point.X >= layout.Width - 28 || point.Y < 70 || point.Y >= layout.Bottom - 17 ||
                 layout.Blocks( point, fleet, planet ) ) continue;
            bool enRoute = !site.Producing && fleet.SiteEnRoute( site.Body, site.Facility );
            Progress( point + new Vector2( -19, 12 ), 38, 1 - (float)site.Remaining / site.Duration, enRoute ? Label : SpaceDraw.Amber );
            string status = enRoute ? "EN ROUTE" : site.Producing ? "BUILDING SHIP" : "CONSTRUCTION";
            Text( status, point.X - TextWidth( status.Length, 1 ) / 2, point.Y + 18, 1, enRoute ? Label : SpaceDraw.Amber );
        }
        for ( int body = 1; body < bodies.Length; ++body ) {
            var status = fleet.Planet( body );
            if ( status.SurveyUnit is null ) continue;
            var p = new Vector2( bodies[body].X, bodies[body].Y + bodies[body].Z ) / _scale + new Vector2( 0, 22 );
            if ( p.X >= 20 && p.X < layout.Width - 20 && p.Y >= 55 && p.Y < layout.Bottom - 12 &&
                 !layout.Blocks( p, fleet, planet ) )
                Progress( p + new Vector2( -16, 0 ), 32, (float)status.Survey / FleetSimulation.SurveyDuration, SpaceDraw.Cyan );
        }
        if ( hover.Kind == HoverKind.None ) return;

        Span<char> title = stackalloc char[42], detail = stackalloc char[55];
        int titleLength, detailLength;
        float targetRadius = 10;
        var tint = SpaceDraw.Cyan;
        switch ( hover.Kind ) {
            case HoverKind.Ship:
                int ship = hover.Index;
                var role = fleet.Role( fleet.SquadronOf( ship ) );
                var faction = fleet.FactionOf( ship );
                var attitude = Factions.Attitude( FactionId.Sol, faction );
                tint = attitude == Disposition.Allied ? SpaceDraw.Cyan : attitude == Disposition.Hostile ? SpaceDraw.Amber : SpaceDraw.Neutral;
                title.TryWrite( $"{Factions.Name( faction )} / {role.ToString().ToUpperInvariant()}", out titleLength );
                detail.TryWrite( $"{attitude.ToString().ToUpperInvariant()} / HULL {fleet.Hull( ship )}  SHIELD {fleet.Shield( ship )}  ARMOR {fleet.Armor( ship )}", out detailLength );
                break;
            case HoverKind.Site:
                var site = sites[hover.Index];
                titleLength = CopyStatus( site.Facility == PlanetFacility.Yard && fleet.YardAt( site.Body ) != 0 && site.Remaining <= 0
                    ? "CIVILIAN DOCK" : SiteName( site.Facility ), title );
                if ( !site.Producing && fleet.SiteEnRoute( site.Body, site.Facility ) )
                    detailLength = CopyStatus( "CONSTRUCTOR EN ROUTE", detail );
                else if ( site.Remaining > 0 )
                    detail.TryWrite( $"{Seconds( site.Remaining, speed )} SEC LEFT / {fleet.BodyName( site.Body )}", out detailLength );
                else detail.TryWrite( $"ONLINE / {fleet.BodyName( site.Body )} / CLICK TO INSPECT", out detailLength );
                break;
            default:
                int body = hover.Index;
                titleLength = CopyStatus( fleet.BodyName( body ), title );
                targetRadius = MathF.Max( 10, bodies[body].Z / _scale + 3 );
                var status = fleet.Planet( body );
                if ( body == 0 ) detailLength = CopyStatus( "SYSTEM PRIMARY", detail );
                else if ( fleet.RequiresDrive( body ) && !fleet.DriveUpgrade ) detailLength = CopyStatus( "REQUIRES LONG BURN DRIVE", detail );
                else if ( status.SurveyUnit is not null )
                    detail.TryWrite( $"SURVEY {status.Survey * 100 / FleetSimulation.SurveyDuration} PCT", out detailLength );
                else if ( fleet.HasDeposit( body ) )
                    detail.TryWrite( $"DEPOSIT  {fleet.DepositYield( body )} ORE PER CYCLE", out detailLength );
                else detailLength = CopyStatus( status.Surveyed ? "SURVEYED / CLICK TO INSPECT" : "UNSURVEYED / SELECT A SCOUT", detail );
                break;
        }
        var target = hover.Target / _scale;
        var anchor = hover.Pointer / _scale;
        float w = Math.Clamp( MathF.Max( TextWidth( titleLength, 1.1f ), TextWidth( detailLength, 1 ) ) + 19, 142, layout.Width - 20 );
        var pos = Vector2.Clamp( anchor + new Vector2( 18, 15 ), new( 7, 56 ), new( layout.Width - w - 7, layout.Bottom - 51 ) );
        _overlay = 3;
        for ( int x = -1; x <= 1; x += 2 ) {
            for ( int y = -1; y <= 1; y += 2 ) {
                var corner = target + new Vector2( x, y ) * targetRadius;
                Rect( corner.X - (x > 0 ? 6 : 0), corner.Y, 7, 1, tint );
                Rect( corner.X, corner.Y - (y > 0 ? 6 : 0), 1, 7, tint );
            }
        }
        PanelAt( pos.X, pos.Y, w, 42, tint );
        Text( title[..titleLength], pos.X + 9, pos.Y + 8, 1.1f, White );
        Text( detail[..detailLength], pos.X + 9, pos.Y + 26, 1, Label );
        _overlay = 0;
    }

    private void Progress( Vector2 pos, float width, float progress, Vector4 tint ) {
        Rect( pos.X - 1, pos.Y - 1, width + 2, 5, Panel, 1 );
        Rect( pos.X, pos.Y, width, 3, Well );
        if ( progress > 0 ) Rect( pos.X, pos.Y, width * Math.Clamp( progress, 0, 1 ), 3, tint, 3 );
    }
}
