using System.Text.Json;

namespace FleetBattler;

internal readonly record struct StoryReward( ResourceCost Stores, ResearchTopic? Unlock = null, int OreRoyalty = 0 );
internal sealed record StoryEvent( string Id, int SystemIndex, int Tick, int Surveys, ResearchTopic? Requires, Briefing Briefing,
    StoryReward Accept, StoryReward Decline, int Body );
internal sealed record FleetScenario( string Name, ResourceCost Stores, bool Raids, StoryEvent[] Events, PlanetFacility[] StartingSites, bool FrontierProgression );

internal static class ScenarioCatalog {
    internal static FleetScenario Load( string scenario ) {
        using var stories = FleetContent.Read( "lore", "fragments" );
        var fragments = stories.RootElement.GetProperty( "fragments" ).EnumerateArray( ).ToDictionary(
            row => row.GetProperty( "id" ).GetString( )!, row => CatalogJson.Text( row, "text", 640 ), StringComparer.OrdinalIgnoreCase );
        using var events = FleetContent.Read( "events", "events" );
        var dispatches = new Dictionary<string, StoryEvent>( StringComparer.OrdinalIgnoreCase );
        foreach ( var row in events.RootElement.GetProperty( "events" ).EnumerateArray( ) ) {
            string id = row.GetProperty( "id" ).GetString( )!;
            string fragment = row.GetProperty( "fragment" ).GetString( )!;
            if ( !fragments.TryGetValue( fragment, out var prose ) ) throw new InvalidDataException( $"Event {id} references unknown lore {fragment}." );
            string systemName = row.TryGetProperty( "system", out var system ) ? CatalogJson.Text( row, "system", 40 ).ToUpperInvariant() : "SOL";
            int systemIndex = systemName == "SOL" ? 0 : CelestialSystem.Systems.IndexOf( systemName );
            if ( systemIndex < 0 ) throw new InvalidDataException( $"Event {id} references unknown system {systemName}." );
            int body = -1;
            if ( row.TryGetProperty( "body", out _ ) ) {
                string bodyId = CatalogJson.Text( row, "body", 80 );
                body = systemIndex == 0 ? SolarCatalog.Find( bodyId ) : Enumerable.Range( 1, CelestialSystem.FrontierBodyCount - 1 )
                    .FirstOrDefault( index => string.Equals( CelestialSystem.Name( systemIndex, index ), bodyId, StringComparison.OrdinalIgnoreCase ), -1 );
                if ( body <= 0 ) throw new InvalidDataException( $"Event {id} references unknown survey target {bodyId}." );
            }
            var accept = ReadReward( row );
            bool choice = row.TryGetProperty( "declineReward", out var other );
            var decline = choice ? ReadReward( other ) : default;
            var briefing = new Briefing( CatalogJson.Text( row, "title", 60 ), CatalogJson.Text( row, "source", 70 ), prose,
                CatalogJson.EnumId<ContentArt>( row.GetProperty( "artwork" ) ), CatalogJson.Text( row, "accept", 40 ),
                choice ? CatalogJson.Text( other, "label", 40 ) : "DECLINE", Describe( accept ),
                choice ? Describe( decline ) : "KEEP CURRENT STORES" );
            dispatches.Add( id, new( id, systemIndex, CatalogJson.Int( row, "tick", 0, 10_000_000, 0 ),
                CatalogJson.Int( row, "surveys", 0, (systemIndex == 0 ? CelestialSystem.BodyCount : CelestialSystem.FrontierBodyCount) - 1, 0 ),
                CatalogJson.Requirement( row ), briefing, accept, decline, body ) );
        }
        using var scenarios = FleetContent.Read( "scenarios", "scenarios" );
        FleetScenario? chosen = null;
        foreach ( var row in scenarios.RootElement.GetProperty( "scenarios" ).EnumerateArray( ) ) {
            var sequence = row.GetProperty( "events" ).EnumerateArray( ).Select( node => dispatches.TryGetValue( node.GetString( )!, out var entry )
                ? entry : throw new InvalidDataException( $"Unknown scenario event '{node}'." ) ).ToArray( );
            if ( sequence.Distinct( ).Count( ) != sequence.Length ) throw new InvalidDataException( "Duplicate scenario event." );
            var stores = new ResourceCost( CatalogJson.Int( row, "ore", 0, 400 ), CatalogJson.Int( row, "energy", 0, 400 ), CatalogJson.Int( row, "science", 0, 400 ) );
            var definition = new FleetScenario( CatalogJson.Text( row, "name", 60 ), stores, row.GetProperty( "raids" ).GetBoolean( ), sequence,
                row.TryGetProperty( "startingSites", out var sites ) ? sites.EnumerateArray().Select( CatalogJson.EnumId<PlanetFacility> ).Distinct().ToArray() : [],
                row.TryGetProperty( "frontierProgression", out var progression ) && progression.GetBoolean() );
            if ( string.Equals( row.GetProperty( "id" ).GetString( ), scenario, StringComparison.OrdinalIgnoreCase ) ) chosen = definition;
        }
        return chosen ?? throw new InvalidDataException( $"Unknown scenario '{scenario}'." );
    }

    private static StoryReward ReadReward( JsonElement row ) => new( CatalogJson.Cost( row ),
        CatalogJson.Requirement( row, "unlock" ), CatalogJson.Int( row, "oreRoyalty", 0, 50, 0 ) );

    private static string Describe( StoryReward reward ) {
        var terms = new List<string>( 5 );
        if ( reward.Stores.Ore != 0 ) terms.Add( $"+{reward.Stores.Ore} ORE" );
        if ( reward.Stores.Energy != 0 ) terms.Add( $"+{reward.Stores.Energy} ENERGY" );
        if ( reward.Stores.Science != 0 ) terms.Add( $"+{reward.Stores.Science} SCIENCE" );
        if ( reward.OreRoyalty != 0 ) terms.Add( $"{reward.OreRoyalty}% EXTRACTION ROYALTY" );
        if ( reward.Unlock is { } technology ) terms.Add( FleetSimulation.Tech( technology ).Name );
        return terms.Count > 0 ? string.Join( " / ", terms ) : "KEEP CURRENT STORES";
    }
}

internal sealed partial class FleetSimulation {
    internal void StartScenario( FleetScenario scenario ) {
        FrontierProgression = scenario.FrontierProgression;
        (Ore, Energy, Science) = scenario.Stores;
        foreach ( var site in scenario.StartingSites ) SiteDeadlines( site )[Homeworld] = 1;
        // Zero denotes an empty site. Commission starting facilities on the first simulation tick.
        if ( scenario.StartingSites.Length > 0 ) Step();
    }
    internal bool MeetsTrigger( StoryEvent story ) => Tick >= story.Tick && _surveyedProspects >= story.Surveys &&
        (story.Body < 0 || Planet( story.Body ).Surveyed) &&
        (story.Requires is null || Researched( story.Requires.Value ));
    internal void ClaimDispatch( StoryReward reward ) {
        Refund( reward.Stores );
        if ( reward.Unlock is { } technology ) {
            if ( PlayerFaction.ResearchSystem is { } lab && lab.ActiveResearch == technology ) lab.CancelResearch();
            PlayerFaction.Unlock( technology );
        }
        OreRoyalty = Math.Max( OreRoyalty, reward.OreRoyalty );
    }
}
