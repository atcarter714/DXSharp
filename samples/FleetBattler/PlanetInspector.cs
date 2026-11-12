using System.Numerics;
using DXSharp.Graphics;
using StbImageSharp;

namespace FleetBattler;

internal sealed partial class FleetHud {
    private const float InspectorTop = 147, SiteTop = 65, SiteHeight = 38;
    private enum Tile : uint { Scout, Miner, Corvette, Yard, Mine, Lab }

    internal static SampledTexture2D LoadAtlas( GraphicsDevice device, GraphicsCommandList commands ) {
        using var stream = File.OpenRead( Path.Combine( AppContext.BaseDirectory, "Art", "OperationsAtlas.jpg" ) );
        var image = ImageResult.FromStream( stream, ColorComponents.RedGreenBlueAlpha );
        return device.CreateSampledTexture2D( commands, (uint)image.Width, (uint)image.Height, image.Data, srgb: true );
    }

    private static Tile HullArt( HullRole role ) => role switch {
        HullRole.Scout => Tile.Scout, HullRole.Miner => Tile.Miner, _ => Tile.Corvette
    };

    private void Art( Tile tile, float x, float y, float width, float height, bool lit = true ) =>
        _glyphs[_count++] = new( new Vector4( x, y, width, height ) * _scale,
            lit ? Vector4.One : new( 0.32f, 0.41f, 0.49f, 1 ), (uint)tile, 0, 4, _overlay );

    private static int Seconds( int ticks, int speed ) => (int)Math.Ceiling( ticks * FleetSimulation.StepSeconds / (speed * 0.25) );

    private static string PhaseName( NavigationPhase phase ) => phase switch {
        NavigationPhase.Burn => "DEPARTURE BURN", NavigationPhase.Coast => "COAST",
        NavigationPhase.Brake => "BRAKING BURN", NavigationPhase.Correction => "COURSE CORRECTION",
        NavigationPhase.Intercept => "INTERCEPT", NavigationPhase.Turnover => "TURNOVER", _ => "PARKED"
    };

    private void FlightCard( Layout layout, FleetSimulation fleet, FleetSelection unit, int speed ) {
        float top = layout.FleetControls( fleet ) + 26;
        if ( top + 80 >= layout.Bottom ) return;
        PanelAt( 14, top, 157, 74, SpaceDraw.Cyan );
        Text( "FLIGHT PLAN", 23, top + 8, 1.25f, White );
        var plan = fleet.Plan( unit );
        var phase = fleet.RoutePhase( unit );
        Text( plan.Body >= 0 ? fleet.BodyName( plan.Body ) :
            phase == NavigationPhase.Intercept ? "HOSTILE TARGET" : "STELLAR ORBIT", 23, top + 25, 1.05f, Label );
        Text( PhaseName( phase ), 23, top + 41, 1, SpaceDraw.Cyan );
        int remaining = fleet.RouteTicks( unit );
        if ( remaining == 0 ) return;
        Span<char> text = stackalloc char[24];
        text.TryWrite( $"ARRIVAL IN {Seconds( remaining, speed )} SEC", out int length );
        Text( text[..length], 23, top + 56, 1, Label );
    }

    private void InspectPlanet( Layout layout, FleetSimulation fleet, int body, FleetSelection? unit, int speed ) {
        float x = layout.InspectorX, y = InspectorTop, width = layout.InspectorWidth;
        PanelAt( x, y, width, body == 0 ? 60 : layout.InspectorHeight, SpaceDraw.Cyan );
        Text( fleet.BodyName( body ), x + 10, y + 8, 1.6f, White );
        Button( x + width - 29, y + 6, 21, "X", true );
        var kind = fleet.Solar ? SolarCatalog.Get( body ).Kind : body switch { 0 => "PRIMARY STAR", 1 => "HOMEWORLD", 2 => "GAS GIANT", 3 => "INNER MOON", _ => "OUTER BELT" };
        var status = fleet.Planet( body );
        Text( kind, x + 10, y + 28, 1.03f, Label );
        if ( body == 0 ) return;
        Span<char> line = stackalloc char[40];
        int length;
        if ( status.Surveyed )
            line.TryWrite( $"SURVEYED / {(fleet.HasDeposit( body ) ? fleet.DepositYield( body ) : 0)} ORE POTENTIAL", out length );
        else if ( fleet.RequiresDrive( body ) && !fleet.DriveUpgrade )
            length = CopyStatus( "OUT OF RANGE / RESEARCH DRIVE", line );
        else if ( status.SurveyUnit is { } scout )
            line.TryWrite( $"SCOUT {scout.Squad + 1:D2} / {status.Survey * 100 / FleetSimulation.SurveyDuration} PCT", out length );
        else length = CopyStatus( "UNSURVEYED / ASSIGN SCOUT", line );
        Text( line[..length], x + 10, y + 45, 1.02f, status.Surveyed ? SpaceDraw.Cyan : Label );
        if ( status.SurveyUnit is not null || !status.Surveyed && unit is { } selected && fleet.CanSurvey( selected, body ) )
            Button( x + width - 66, y + 24, 57, status.SurveyUnit is null ? "SCAN" : "CANCEL", true );

        int index = 0;
        Span<char> details = stackalloc char[48];
        foreach ( var facility in FleetSimulation.Facilities ) {
            float top = y + SiteTop + index++ * layout.InspectorRow;
            int remaining = fleet.SiteRemaining( body, facility );
            var plan = FleetSimulation.Facility( facility );
            bool online = remaining == 0, project = remaining > 0;
            bool dock = facility == PlanetFacility.Yard && fleet.CanOpenYard( body );
            string? block = remaining < 0 ? fleet.ConstructionBlock( body, facility, unit ) : null;
            bool available = remaining < 0 && block is null;
            var tile = facility switch { PlanetFacility.Mine => Tile.Mine, PlanetFacility.Lab => Tile.Lab, _ => Tile.Yard };
            Rect( x + 8, top, width - 16, layout.InspectorRow - 3, available || online ? Active : Well );
            Art( tile, x + 9, top + 1, 42, layout.InspectorRow - 5, available || remaining >= 0 );
            Text( plan.Name, x + 57, top + 4, 1, White );
            int chars;
            if ( project ) {
                var worker = fleet.SiteWorker( body, facility );
                int id = worker is { IsShip: true } unitWorker ? unitWorker.Ship + 1 : (worker?.Squad ?? -1) + 1;
                if ( fleet.SiteEnRoute( body, facility ) ) details.TryWrite( $"EN ROUTE / UNIT {id:D2}", out chars );
                else details.TryWrite( $"UNIT {id:D2} / {Seconds( remaining, speed )} SEC", out chars );
                Button( x + width - 30, top + 3, 20, "X", true );
            } else if ( online ) {
                if ( facility == PlanetFacility.Yard )
                    details.TryWrite( $"+{FleetSimulation.YardNavalCapacity} NAVAL / {fleet.YardQueue( body ).Length} QUEUED", out chars );
                else {
                    var (amount, resource) = facility switch {
                        PlanetFacility.Mine => (fleet.DepositYield( body ), "ORE"),
                        PlanetFacility.Lab => (6, "SCI"),
                        PlanetFacility.Solar => (FleetSimulation.SolarOutput, "ENERGY"),
                        _ => (FleetSimulation.DepotNavalCapacity, "NAVAL")
                    };
                    details.TryWrite( $"+{amount} {resource}{(facility == PlanetFacility.Depot ? "" : " PER CYCLE")}", out chars );
                }
            } else {
                if ( block is null ) chars = CopyStatus( "BUILD", details );
                else chars = CopyStatus( block, details );
            }
            int maxChars = (int)((width - (project || dock ? 117 : 67)) / TextWidth( 1, 1 ));
            Text( details[..Math.Min( chars, maxChars )], x + 57, top + 17, 1, available || online ? SpaceDraw.Cyan : Label );
            if ( available ) Cost( plan.Cost, x + 93, top + 16 );
            if ( dock && !project ) Button( x + width - 50, top + 5, 42, "OPEN", true );
            if ( project && !fleet.SiteEnRoute( body, facility ) )
                Progress( new( x + 57, top + layout.InspectorRow - 8 ), width - 94, 1f - (float)remaining / plan.Duration, SpaceDraw.Amber );
        }
    }

    private static HudHit HitInspector( Layout layout, Vector2 point, int body, FleetSimulation fleet ) {
        float x = layout.InspectorX, width = layout.InspectorWidth;
        if ( !Contains( point, x, InspectorTop, width, body == 0 ? 60 : layout.InspectorHeight ) ) return default;
        if ( Contains( point, x + width - 29, InspectorTop + 6, 21, 21 ) ) return Planet( PlanetAction.Close );
        if ( body == 0 ) return new( HudAction.Panel );
        var status = fleet.Planet( body );
        if ( !status.Surveyed && Contains( point, x + width - 66, InspectorTop + 24, 57, 21 ) )
            return Planet( status.SurveyUnit is null ? PlanetAction.Survey : PlanetAction.CancelSurvey );
        float row = point.Y - InspectorTop - SiteTop;
        if ( row < 0 || row >= FleetSimulation.FacilityCount * layout.InspectorRow ||
             row % layout.InspectorRow >= layout.InspectorRow - 3 || point.X < x + 8 || point.X >= x + width - 8 )
            return new( HudAction.Panel );
        var facility = FleetSimulation.Facilities[(int)(row / layout.InspectorRow)];
        int remaining = fleet.SiteRemaining( body, facility );
        if ( remaining > 0 && point.X >= x + width - 30 ) return new( HudAction.CancelSite, (int)facility );
        if ( facility == PlanetFacility.Yard && fleet.CanOpenYard( body ) && remaining <= 0 && (remaining == 0 || point.X >= x + width - 50) )
            return new( HudAction.OpenYard, body );
        if ( remaining >= 0 ) return new( HudAction.Panel );
        return new( HudAction.BuildSite, (int)facility );

        static HudHit Planet( PlanetAction action ) => new( HudAction.Planet, (int)action );
    }

    private readonly partial record struct Layout {
        internal float InspectorWidth => Width < 590 ? 227 : 282;
        internal float InspectorX => Width - InspectorWidth - 14;
        internal float InspectorHeight => MathF.Min( 269, Bottom - InspectorTop - 6 );
        internal float InspectorRow => MathF.Min( 38, (InspectorHeight - SiteTop - 7) / FleetSimulation.FacilityCount );
        internal float FleetControls( FleetSimulation fleet ) => 70 + OutlinerHeight( fleet );
    }
}
