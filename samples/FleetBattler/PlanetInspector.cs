using System.Numerics;
using DXSharp.Graphics;
using StbImageSharp;

namespace FleetBattler;

internal sealed partial class FleetHud {
    private const float InspectorTop = 147, SiteTop = 92;
    private enum Tile : uint { Scout, Miner, Corvette, Yard, Mine, Lab }

    internal static SampledTexture2D LoadAtlas( GraphicsDevice device, GraphicsCommandList commands ) {
        using var stream = File.OpenRead( Path.Combine( AppContext.BaseDirectory, "Art", "OperationsAtlas.jpg" ) );
        var image = ImageResult.FromStream( stream, ColorComponents.RedGreenBlueAlpha );
        return device.CreateSampledTexture2D( commands, (uint)image.Width, (uint)image.Height, image.Data, srgb: true );
    }

    private static Tile HullArt( HullRole role ) => role switch {
        HullRole.Scout => Tile.Scout, HullRole.Miner or HullRole.Colony or HullRole.Freighter => Tile.Miner, _ => Tile.Corvette
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
        float top = layout.FleetControls( fleet ) + 82;
        if ( top + 80 >= layout.Bottom ) return;
        PanelAt( 14, top, 157, 74, SpaceDraw.Cyan );
        Text( "FLIGHT PLAN", 23, top + 8, 1.25f, White );
        var plan = fleet.Plan( unit );
        var phase = fleet.RoutePhase( unit );
        Text( plan.Body >= 0 ? fleet.BodyName( plan.Body ) :
            phase == NavigationPhase.Intercept ? "HOSTILE TARGET" : "STELLAR ORBIT", 23, top + 25, 1.05f, Label );
        string status = fleet.Service( unit ) switch {
            ServiceOrder.Repair => phase == NavigationPhase.Parked ? "AT YARD / REPAIR" : "TRANSFER / REPAIR",
            ServiceOrder.Refit => phase == NavigationPhase.Parked ? "AT YARD / REFIT" : "TRANSFER / REFIT",
            ServiceOrder.Retreat => "RETREATING", _ => PhaseName( phase )
        };
        Text( status, 23, top + 41, 1, SpaceDraw.Cyan );
        int remaining = fleet.RouteTicks( unit );
        if ( remaining == 0 ) return;
        Span<char> text = stackalloc char[24];
        text.TryWrite( $"ARRIVAL IN {Seconds( remaining, speed )} SEC", out int length );
        Text( text[..length], 23, top + 56, 1, Label );
    }

    private void InspectPlanet( Layout layout, FleetSimulation fleet, int body, FleetSelection? unit, int speed, bool catalog, SiteLayer catalogLayer ) {
        float x = layout.InspectorX, y = InspectorTop, width = layout.InspectorWidth;
        PanelAt( x, y, width, body == 0 ? 60 : layout.InspectorHeight, catalog ? EnergyTint : SpaceDraw.Cyan );
        Text( fleet.BodyName( body ), x + 10, y + 8, 1.6f, White );
        Button( x + width - 29, y + 6, 21, "X", true );
        var kind = fleet.Solar ? SolarCatalog.Get( body ).Kind : body switch {
            0 => "PRIMARY STAR", 1 => "ROCKY PLANET", 2 => "GAS GIANT", 3 => "INNER MOON", _ => "OUTER BELT OBJECT"
        };
        Text( kind, x + 10, y + 28, 1.03f, Label );
        if ( body == 0 ) return;

        var status = fleet.Planet( body );
        Span<char> text = stackalloc char[58];
        int length;
        var colony = fleet.Colony( body );
        if ( colony.Ship is not null ) {
            text.TryWrite( $"{(fleet.Settling( body ) ? "SETTLING" : "COLONY SHIP EN ROUTE")} / {Seconds( colony.WorkRemaining, speed )} SEC", out length );
            Progress( new( x + 10, y + 56 ), width - 20, 1 - (float)colony.WorkRemaining / FleetSimulation.ColonyDuration, SpaceDraw.Cyan );
        } else if ( colony.Population > 0 )
            text.TryWrite( $"POP {colony.Population}/{colony.Housing} / {(fleet.ColonySupplied( body ) ? "SUPPLIED" : "NEEDS POWER OR DEPOT")}", out length );
        else if ( status.Surveyed ) {
            if ( unit is { } colonyUnit && fleet.Role( colonyUnit.Squad ) == HullRole.Colony &&
                 fleet.ColonyBlock( body, unit ) is { } need ) length = CopyStatus( need, text );
            else text.TryWrite( $"SURVEYED / {fleet.DepositYield( body )} ORE POTENTIAL", out length );
        }
        else if ( status.SurveyUnit is { } scout ) {
            text.TryWrite( $"SCOUT {scout.Squad + 1:D2} / {(fleet.SurveyWorking( body ) ? "SCANNING" : "EN ROUTE")} {(int)(status.SurveyProgress * 100)} PCT", out length );
            Progress( new( x + 10, y + 56 ), width - 20, status.SurveyProgress, ScienceTint );
        } else length = CopyStatus( fleet.RequiresDrive( body ) && !fleet.DriveUpgrade ? "OUT OF RANGE / RESEARCH DRIVE" : "UNSURVEYED / ASSIGN SCOUT", text );
        Text( text[..length], x + 10, y + 45, 1, status.Surveyed ? SpaceDraw.Cyan : Label );
        if ( status.SurveyUnit is not null || !status.Surveyed && unit is { } selected && fleet.CanSurvey( selected, body ) )
            Button( x + width - 66, y + 24, 57, status.SurveyUnit is null ? "SCAN" : "CANCEL", true );
        if ( status.Surveyed && fleet.Colonizable( body ) && colony.Population == 0 )
            Button( x + width - 77, y + 24, 68, colony.Ship is null ? "SETTLE" : "CANCEL",
                colony.Ship is not null || fleet.ColonyBlock( body, unit ) is null );

        var slots = fleet.Slots( body );
        int surface = fleet.OccupiedSlots( body, SiteLayer.Surface ), orbital = fleet.OccupiedSlots( body, SiteLayer.Orbit );
        if ( catalog )
            Button( x + 9, y + 61, 78, catalogLayer == SiteLayer.Orbit ? "SURFACE" : "ORBIT", true );
        else {
            text.TryWrite( $"S {surface}/{fleet.SurfaceWorkLimit( body )}  O {orbital}/{slots.Orbit}", out length );
            Text( text[..length], x + 10, y + 68, 1, Label );
        }
        IconButton( x + width - 66, y + 61, 57, catalog ? HudIcon.Orders : HudIcon.Build, catalog ? "SITES" : "BUILD", true );
        if ( fleet.CanOpenYard( body ) && !catalog )
            IconButton( x + width - 127, y + 61, 57, HudIcon.Yard, "YARD", true );
        Text( catalog ? catalogLayer == SiteLayer.Orbit ? "ORBITAL CONSTRUCTION" : "SURFACE CONSTRUCTION" : "SITE INVENTORY", x + 10, y + 84, 1, catalog ? EnergyTint : SpaceDraw.Cyan );

        int row = 0;
        foreach ( var facility in FleetSimulation.Facilities ) {
            if ( !ShowSite( fleet, body, facility, catalog, catalogLayer ) ) continue;
            var plan = FleetSimulation.Facility( facility );
            int remaining = fleet.SiteRemaining( body, facility );
            float top = y + SiteTop + 5 + row++ * layout.InspectorRow;
            string? block = catalog ? fleet.ConstructionBlock( body, facility, unit ) : null;
            bool available = catalog && block is null, online = remaining == 0, project = remaining > 0;
            var tile = facility switch {
                PlanetFacility.Mine or PlanetFacility.Skimmer => Tile.Mine,
                PlanetFacility.Lab or PlanetFacility.Observatory => Tile.Lab, PlanetFacility.SurfaceStore => Tile.Mine, _ => Tile.Yard
            };
            Rect( x + 8, top, width - 16, layout.InspectorRow - 2, available || online ? Active : Well );
            Art( tile, x + 9, top + 1, 32, layout.InspectorRow - 4, available || remaining >= 0 );
            Text( plan.Name, x + 45, top + 3, 1, White );
            if ( project ) {
                var worker = fleet.SiteWorker( body, facility );
                int id = worker is { IsShip: true } assigned ? assigned.Ship + 1 : (worker?.Squad ?? -1) + 1;
                if ( plan.Layer == SiteLayer.Surface ) text.TryWrite( $"LOCAL CREW / {Seconds( remaining, speed )} SEC", out length );
                else if ( fleet.SiteEnRoute( body, facility ) ) text.TryWrite( $"EN ROUTE / UNIT {id:D2}", out length );
                else text.TryWrite( $"UNIT {id:D2} / {Seconds( remaining, speed )} SEC", out length );
                Rect( x + width - 28, top + 3, 18, layout.InspectorRow - 7, Well );
                Text( "X", x + width - 23, top + 7, 1, SpaceDraw.Amber );
                if ( !fleet.SiteEnRoute( body, facility ) )
                    Progress( new( x + 45, top + layout.InspectorRow - 6 ), width - 77, 1f - (float)remaining / plan.Duration, SpaceDraw.Amber );
            } else if ( online ) {
                var (amount, kindText) = facility switch {
                    PlanetFacility.Mine => (fleet.DepositYield( body ), "ORE / CYCLE"),
                    PlanetFacility.Skimmer => (fleet.OrbitalExtractionYield( body ), "ORE / CYCLE"),
                    PlanetFacility.SurfaceStore => (250, "ORE CAP / 100 ENERGY"),
                    PlanetFacility.OrbitalStore => (180, "ORE CAP / 250 ENERGY"),
                    PlanetFacility.Lab => (6, "SCI / CYCLE"), PlanetFacility.Observatory => (10, "SCI / CYCLE"),
                    PlanetFacility.Solar => (FleetSimulation.SolarOutput, "ENERGY / CYCLE"),
                    PlanetFacility.Yard => (FleetSimulation.YardNavalCapacity, "NAVAL / REPAIR / REFIT"),
                    PlanetFacility.Habitat => (FleetSimulation.HabitatNavalCapacity, "NAVAL / HABITATION"),
                    _ => (FleetSimulation.DepotNavalCapacity, "NAVAL")
                };
                text.TryWrite( $"+{amount} {kindText}", out length );
            } else if ( block is not null ) length = CopyStatus( block, text );
            else {
                Text( plan.Layer == SiteLayer.Surface ? "SURFACE" : "ORBIT", x + 45, top + 13, 1, Label );
                Cost( plan.Cost, x + 102, top + 12 );
                Icon( HudIcon.Build, x + width - 24, top + 7, 11, EnergyTint );
                continue;
            }
            int maxChars = Math.Max( 0, (int)((width - (project ? 76 : 53)) / TextWidth( 1, 1 )) );
            Text( text[..Math.Min( length, maxChars )], x + 45, top + 13, 1, online ? SpaceDraw.Cyan : Label );
        }
        if ( row == 0 ) Text( !catalog ? "NO SITES BUILT OR ASSIGNED" : slots.For( catalogLayer ) == 0
            ? "NO BUILDABLE SLOTS HERE" : "ALL COMPATIBLE TYPES ASSIGNED", x + 10, y + SiteTop + 19, 1, Label );
        if ( !catalog ) {
            float footer = y + layout.InspectorHeight - 15;
            SiteSlotStrip( "SURFACE", x + 10, footer, slots.Surface, surface, fleet.SurfaceWorkLimit( body ) );
            SiteSlotStrip( "ORBIT", x + 111, footer, slots.Orbit, orbital, slots.Orbit );
        }
    }

    private void SiteSlotStrip( string label, float x, float y, int slots, int occupied, int staffed ) {
        Text( label, x, y + 2, 1, Label );
        float left = x + TextWidth( label.Length, 1 ) + 6;
        if ( slots == 0 ) { Text( "0", left, y + 2, 1, Disabled ); return; }
        for ( int slot = 0; slot < slots; ++slot ) {
            float cell = left + 12 * slot;
            Rect( cell, y, 9, 9, slot < occupied ? SpaceDraw.Cyan : slot < staffed ? Label : Disabled );
            if ( slot >= occupied ) Rect( cell + 1, y + 1, 7, 7, Well, 3 );
        }
    }

    private static bool ShowSite( FleetSimulation fleet, int body, PlanetFacility facility, bool catalog, SiteLayer catalogLayer ) =>
        catalog ? FleetSimulation.Facility( facility ).Layer == catalogLayer && fleet.FitsBody( body, facility ) && fleet.SiteRemaining( body, facility ) < 0 : fleet.SiteRemaining( body, facility ) >= 0;

    private static HudHit HitInspector( Layout layout, Vector2 point, int body, FleetSimulation fleet, bool catalog, SiteLayer catalogLayer ) {
        float x = layout.InspectorX, width = layout.InspectorWidth;
        if ( !Contains( point, x, InspectorTop, width, body == 0 ? 60 : layout.InspectorHeight ) ) return default;
        if ( Contains( point, x + width - 29, InspectorTop + 6, 21, 21 ) ) return Planet( PlanetAction.Close );
        if ( body == 0 ) return new( HudAction.Panel );
        var status = fleet.Planet( body );
        if ( !status.Surveyed && Contains( point, x + width - 66, InspectorTop + 24, 57, 21 ) )
            return Planet( status.SurveyUnit is null ? PlanetAction.Survey : PlanetAction.CancelSurvey );
        if ( status.Surveyed && fleet.Colonizable( body ) && fleet.Colony( body ) is { Population: 0 } colony &&
             Contains( point, x + width - 77, InspectorTop + 24, 68, 21 ) )
            return new( colony.Ship is null ? HudAction.Colonize : HudAction.CancelColony, body );
        if ( catalog && Contains( point, x + 9, InspectorTop + 61, 78, 21 ) ) return new( HudAction.BuildLayer );
        if ( Contains( point, x + width - 66, InspectorTop + 61, 57, 21 ) ) return new( HudAction.ToggleBuildCatalog );
        if ( !catalog && fleet.CanOpenYard( body ) && Contains( point, x + width - 127, InspectorTop + 61, 57, 21 ) )
            return new( HudAction.OpenYard, body );
        int row = 0;
        foreach ( var facility in FleetSimulation.Facilities ) {
            if ( !ShowSite( fleet, body, facility, catalog, catalogLayer ) ) continue;
            float top = InspectorTop + SiteTop + 5 + row++ * layout.InspectorRow;
            if ( !Contains( point, x + 8, top, width - 16, layout.InspectorRow - 2 ) ) continue;
            if ( catalog ) return new( HudAction.BuildSite, (int)facility );
            if ( fleet.SiteRemaining( body, facility ) > 0 && point.X >= x + width - 28 )
                return new( HudAction.CancelSite, (int)facility );
            if ( facility == PlanetFacility.Yard && fleet.YardAt( body ) == 0 ) return new( HudAction.OpenYard, body );
            break;
        }
        return new( HudAction.Panel );

        static HudHit Planet( PlanetAction action ) => new( HudAction.Planet, (int)action );
    }

    private readonly partial record struct Layout {
        internal float InspectorWidth => Width < 590 ? 227 : 282;
        internal float InspectorX => Width - InspectorWidth - 14;
        internal float InspectorHeight => MathF.Min( 302, Bottom - InspectorTop - 6 );
        internal float InspectorRow => MathF.Min( 30, (InspectorHeight - SiteTop - 22) / 7 );
        internal float FleetControls( FleetSimulation fleet ) => 70 + OutlinerHeight( fleet );
    }
}
