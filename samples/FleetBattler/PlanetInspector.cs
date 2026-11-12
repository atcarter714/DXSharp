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
        NavigationPhase.Intercept => "INTERCEPT", _ => "PARKED"
    };

    private void FlightCard( Layout layout, FleetSimulation fleet, FleetSelection unit, int speed ) {
        float top = 70 + layout.OutlinerHeight( fleet );
        if ( top + 80 >= layout.Bottom ) return;
        PanelAt( 14, top, 157, 74, SpaceDraw.Cyan );
        Text( "FLIGHT PLAN", 23, top + 8, 1.25f, White );
        var plan = fleet.Plan( unit );
        var phase = fleet.RoutePhase( unit );
        Text( plan.Body >= 0 ? CelestialSystem.Name( fleet.SystemIndex, plan.Body ) :
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
        Text( CelestialSystem.Name( fleet.SystemIndex, body ), x + 10, y + 8, 1.6f, White );
        Button( x + width - 29, y + 6, 21, "X", true );
        var kind = body switch { 0 => "PRIMARY STAR", 1 => "HOMEWORLD", 2 => "GAS GIANT", 3 => "INNER MOON", _ => "OUTER BELT" };
        var status = fleet.Planet( body );
        Text( kind, x + 10, y + 28, 1.03f, Label );
        if ( body == 0 ) return;
        Span<char> line = stackalloc char[40];
        int length;
        if ( status.Surveyed )
            line.TryWrite( $"SURVEYED / {(fleet.HasDeposit( body ) ? fleet.DepositYield( body ) : 0)} ORE POTENTIAL", out length );
        else if ( body == CelestialSystem.OuterBelt && !fleet.DriveUpgrade )
            length = CopyStatus( "OUT OF RANGE / RESEARCH DRIVE", line );
        else if ( status.SurveyUnit is { } scout )
            line.TryWrite( $"SCOUT {scout.Squad + 1:D2} / {status.Survey * 100 / FleetSimulation.SurveyDuration} PCT", out length );
        else length = CopyStatus( "UNSURVEYED / ASSIGN SCOUT", line );
        Text( line[..length], x + 10, y + 45, 1.02f, status.Surveyed ? SpaceDraw.Cyan : Label );
        if ( status.SurveyUnit is not null || !status.Surveyed && unit is { } selected && fleet.CanSurvey( selected, body ) )
            Button( x + width - 66, y + 24, 57, status.SurveyUnit is null ? "SCAN" : "CANCEL", true );

        Site( 0, Tile.Mine, "EXTRACTION RIG", status.MineRemaining, FleetSimulation.FacilityDuration, FleetSimulation.MineCost,
            status.MineRemaining == 0 ? "ORE PRODUCTION" : fleet.MineEnRoute( body ) ? "MINER EN ROUTE" : null,
            fleet.DepositYield( body ), status.Surveyed && fleet.CanPlaceMine( body, unit ) );
        Site( 1, Tile.Lab, "RESEARCH LAB", status.LabRemaining, FleetSimulation.FacilityDuration, FleetSimulation.LabCost,
            status.LabRemaining == 0 ? "SCIENCE PRODUCTION" : null, 6, status.Surveyed );
        Site( 2, Tile.Yard, "ORBITAL SHIPYARD", fleet.YardAt( body ), FleetSimulation.YardDuration, FleetSimulation.YardCost,
            fleet.YardAt( body ) == 0 ? fleet.ActiveYard == body ? "WING IN PRODUCTION" : fleet.CanQueueWing( body ) ? "QUEUE WING" : "ONLINE / NAVAL CAPACITY" : null,
            FleetSimulation.YardNavalCapacity, status.Surveyed );
        Site( 3, Tile.Yard, "LOGISTICS DEPOT", fleet.DepotAt( body ), FleetSimulation.FacilityDuration, FleetSimulation.DepotCost,
            fleet.DepotAt( body ) == 0 ? "ONLINE / NAVAL CAPACITY" : null, FleetSimulation.DepotNavalCapacity, status.Surveyed );
        if ( layout.InspectorHeight >= 246 ) {
            int active = (status.MineRemaining == 0 ? 1 : 0) + (status.LabRemaining == 0 ? 1 : 0) +
                (fleet.YardAt( body ) == 0 ? 1 : 0) + (fleet.DepotAt( body ) == 0 ? 1 : 0);
            line.TryWrite( $"SITE OPERATIONS  /  {active} ONLINE", out length );
            Text( line[..length], x + 10, y + 228, 1.03f, Label );
        }

        void Site( int index, Tile tile, string name, int remaining, int duration, int cost, string? description, int output, bool available ) {
            float top = y + SiteTop + index * SiteHeight;
            Span<char> details = stackalloc char[44];
            int chars;
            bool producing = index == 2 && remaining == 0 && fleet.ActiveYard == body;
            bool enabled = remaining < 0 && available && fleet.Ore >= cost || index == 2 && fleet.CanQueueWing( body );
            Rect( x + 8, top, width - 16, 34, enabled ? Active : Well );
            Art( tile, x + 9, top + 1, 48, 32, remaining >= 0 || enabled );
            Text( name, x + 64, top + 4, 1.04f, White );
            if ( producing ) details.TryWrite( $"WING BUILDING / {Seconds( fleet.WingRemaining, speed )} SEC", out chars );
            else if ( remaining > 0 ) details.TryWrite( $"{description ?? "BUILDING"} / {Seconds( remaining, speed )} SEC", out chars );
            else if ( remaining == 0 ) {
                if ( index == 2 && fleet.OrdersAt( body ) > 0 )
                    details.TryWrite( $"{fleet.OrdersAt( body )} WING WAITING", out chars );
                else if ( description == "QUEUE WING" ) details.TryWrite( $"QUEUE WING / {fleet.CurrentWingCost} ORE", out chars );
                else details.TryWrite( $"{description} / +{output}", out chars );
            } else details.TryWrite( $"{(available ? "BUILD" : index == 0 && status.Surveyed ? "NEEDS DEPOSIT AND MINER" : "SURVEY REQUIRED")} / {cost} ORE", out chars );
            int maxChars = (int)((width - 72) / TextWidth( 1, 1 ));
            Text( details[..Math.Min( chars, maxChars )], x + 64, top + 16, 1, enabled ? SpaceDraw.Cyan : Label );
            if ( remaining > 0 || producing ) {
                float fraction = producing ? 1 - (float)fleet.WingRemaining / fleet.ActiveWingDuration : 1 - (float)remaining / duration;
                Rect( x + 64, top + 29, width - 74, 2, Panel, 3 );
                if ( fraction > 0 ) Rect( x + 64, top + 29, (width - 74) * fraction, 2, SpaceDraw.Cyan, 5 );
            }
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
        if ( row >= 0 && row < 4 * SiteHeight && row % SiteHeight < 34 && point.X >= x + 8 && point.X < x + width - 8 )
            return Planet( (int)(row / SiteHeight) switch {
                0 => PlanetAction.Mine, 1 => PlanetAction.Lab,
                2 => fleet.YardAt( body ) == 0 ? PlanetAction.SiteWing : PlanetAction.Yard,
                _ => PlanetAction.Depot
            } );
        return new( HudAction.Panel );

        static HudHit Planet( PlanetAction action ) => new( HudAction.Planet, (int)action );
    }

    private readonly partial record struct Layout {
        internal float InspectorWidth => Width < 590 ? 227 : 282;
        internal float InspectorX => Width - InspectorWidth - 14;
        internal float InspectorHeight => MathF.Min( 247, Bottom - InspectorTop - 6 );
    }
}
