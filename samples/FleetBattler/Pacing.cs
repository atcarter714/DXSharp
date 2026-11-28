using System.Text.Json;

namespace FleetBattler;

// Campaign pacing. Content/pacing.json sets how long things take on the campaign calendar, so the tempo can be
// tuned without code changes. Physics still steps at FleetSimulation.StepSeconds; only the calendar and the
// durations measured against it change. Skirmish and sandbox scenarios keep their tactical timings.
internal sealed record PacingTable(
    int CrossingTicks, int CrossingOverheadTicks, float PreFusion, (ResearchTopic Topic, float Factor)[] Transit,
    float InSystemTransfer, int SurveyTicks, float StructureScale, int[] StructureTicks, float HullScale,
    float ResearchScale, int EconomyCycleTicks, int ColonyLandingTicks, int ColonyGrowthTicks,
    int FirstRaidTicks, int RaidDelayMinTicks, int RaidDelayMaxTicks, double MoonPeriodOffsetDays, int ClimateTicks ) {

    internal static PacingTable Parse( JsonElement root ) {
        var rows = new Dictionary<string, JsonElement>( StringComparer.OrdinalIgnoreCase );
        var transit = new List<(ResearchTopic, float)>( );
        var structures = new int[SiteConstructionCount];
        foreach ( var row in root.GetProperty( "pacing" ).EnumerateArray( ) ) {
            string id = row.GetProperty( "id" ).GetString( ) ?? "";
            if ( id.StartsWith( "transit.", StringComparison.OrdinalIgnoreCase ) )
                transit.Add( (ResearchCatalog.Current.Resolve( id["transit.".Length..] ), Factor( row, id, .05f, 1 )) );
            else if ( id.StartsWith( "build.", StringComparison.OrdinalIgnoreCase ) )
                structures[(int)Structure( id["build.".Length..] )] = Pacing.Ticks( Days( row, id ) );
            else rows[id] = row;
        }
        JsonElement Row( string id ) => rows.TryGetValue( id, out var row ) ? row
            : throw new InvalidDataException( $"pacing.json is missing the '{id}' row." );
        int Ticks( string id ) => Pacing.Ticks( Days( Row( id ), id ) );
        float Scale( string id, float max ) => Factor( Row( id ), id, .01f, max );
        int raidMin = Ticks( "raid-delay-min" ), raidMax = Ticks( "raid-delay-max" );
        int crossing = Ticks( "proxima-crossing" ), overhead = Ticks( "crossing-overhead" );
        if ( raidMax < raidMin ) throw new InvalidDataException( "pacing.json raid-delay-max must not be below raid-delay-min." );
        if ( overhead >= crossing ) throw new InvalidDataException( "pacing.json crossing-overhead must be shorter than proxima-crossing." );
        return new( crossing, overhead, Scale( "pre-fusion", 10 ), transit.ToArray( ), Scale( "in-system-transfer", 100 ),
            Ticks( "survey" ), Scale( "structure-scale", 1000 ), structures, Scale( "hull-scale", 1000 ), Scale( "research-scale", 1000 ),
            Ticks( "economy-cycle" ), Ticks( "colony-landing" ), Ticks( "colony-growth" ), Ticks( "first-raid" ), raidMin, raidMax,
            Days( Row( "moon-period-offset" ), "moon-period-offset", 0 ), Ticks( "climate-project" ) );
    }

    private static PlanetFacility Structure( string name ) =>
        Enum.TryParse<PlanetFacility>( name, true, out var facility ) && Enum.IsDefined( facility ) && !int.TryParse( name, out _ ) ? facility
            : throw new InvalidDataException( $"pacing.json names unknown structure '{name}'." );

    private const int SiteConstructionCount = (int)PlanetFacility.Soletta + 1;

    private static float Days( JsonElement row, string id, float min = 1f / Pacing.TicksPerDay ) =>
        Required( CatalogJson.Number( row, "days", min, 36_500, float.NaN ), id, "days" );

    private static float Factor( JsonElement row, string id, float min, float max ) =>
        Required( CatalogJson.Number( row, "factor", min, max, float.NaN ), id, "factor" );

    private static float Required( float value, string id, string key ) =>
        float.IsNaN( value ) ? throw new InvalidDataException( $"pacing.json row '{id}' requires '{key}'." ) : value;
}

internal static class Pacing {
    // A campaign tick spans 24 calendar minutes, so one game day passes each real second at 1x.
    internal const int TicksPerDay = 60;
    internal const int CalendarMinutesPerTick = SolarCatalog.TicksPerDay / TicksPerDay;

    private static PacingTable? _table;
    internal static PacingTable Table => _table ??= Load( );

    private static PacingTable Load( ) {
        using var document = FleetContent.Read( "pacing", "pacing" );
        return PacingTable.Parse( document.RootElement );
    }

    internal static int Ticks( double days ) => checked((int)Math.Round( days * TicksPerDay ));
    internal static double DaysOf( int ticks ) => (double)ticks / TicksPerDay;
    internal static int Scale( int ticks, float factor ) => Math.Max( 1, checked((int)Math.Round( ticks * (double)factor )) );

    // Converts a simulation tick to the calendar minutes the ephemeris and star date use.
    internal static int CalendarMinutes( int tick, bool campaign ) => campaign ? checked(tick * CalendarMinutesPerTick) : tick;

    // Sol to Proxima takes CrossingTicks with the fusion drive; other routes scale the cruise part by chart distance.
    internal static int InterstellarTicks( float distance, bool fusion, Func<ResearchTopic, bool> researched ) {
        var table = Table;
        double cruise = (table.CrossingTicks - table.CrossingOverheadTicks) * (distance / ProximaDistance);
        double ticks = table.CrossingOverheadTicks + cruise;
        if ( !fusion ) ticks *= table.PreFusion;
        foreach ( var (topic, factor) in table.Transit )
            if ( researched( topic ) ) ticks *= factor;
        return Math.Max( TicksPerDay, checked((int)Math.Round( ticks )) );
    }

    internal static float ProximaDistance => System.Numerics.Vector2.Distance(
        CelestialSystem.MapPositions[0], CelestialSystem.MapPositions[FleetSimulation.Proxima] );
}
