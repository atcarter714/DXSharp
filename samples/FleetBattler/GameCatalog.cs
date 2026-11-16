using System.Text.Json;

namespace FleetBattler;

internal static class CatalogJson {
    internal static JsonDocument Open( string name ) {
        string path = Path.Combine( AppContext.BaseDirectory, "Content", name + ".json" );
        try {
            using var stream = File.OpenRead( path );
            var document = JsonDocument.Parse( stream );
            if ( document.RootElement.TryGetProperty( "version", out var version ) && version.TryGetInt32( out int value ) && value == 1 )
                return document;
            document.Dispose( );
            throw new InvalidDataException( $"{path} requires catalog version 1." );
        } catch ( JsonException error ) {
            throw new InvalidDataException( $"Cannot parse {path}: {error.Message}", error );
        }
    }

    internal static string Text( JsonElement item, string key, int limit ) {
        string? value = item.GetProperty( key ).GetString( )?.Trim( );
        return value is not null && value.Length > 0 && value.Length <= limit ? value.ToUpperInvariant( )
            : throw new InvalidDataException( $"{key} requires 1 to {limit} characters." );
    }

    internal static int Int( JsonElement item, string key, int min, int max, int? fallback = null ) {
        if ( !item.TryGetProperty( key, out var node ) && fallback is { } missing ) return missing;
        return node.ValueKind == JsonValueKind.Number && node.TryGetInt32( out int value ) && value >= min && value <= max ? value
            : throw new InvalidDataException( $"{key} requires an integer from {min} to {max}." );
    }

    internal static float Number( JsonElement item, string key, float min, float max, float fallback ) {
        if ( !item.TryGetProperty( key, out var node ) ) return fallback;
        return node.ValueKind == JsonValueKind.Number && node.TryGetSingle( out float value ) && value >= min && value <= max ? value
            : throw new InvalidDataException( $"{key} requires a number from {min} to {max}." );
    }

    internal static T EnumId<T>( JsonElement node ) where T : struct, Enum {
        string? name = node.GetString( );
        return Enum.TryParse<T>( name, true, out var result ) &&
            string.Equals( name, Enum.GetName( result ), StringComparison.OrdinalIgnoreCase ) ? result
            : throw new InvalidDataException( $"Unknown {typeof(T).Name} '{name}'." );
    }

    internal static ResearchTopic? Requirement( JsonElement item ) {
        if ( !item.TryGetProperty( "requires", out var node ) ) return null;
        var topic = EnumId<ResearchTopic>( node );
        return topic != ResearchTopic.Count ? topic : throw new InvalidDataException( "Invalid research requirement." );
    }

    internal static ResourceCost Cost( JsonElement item ) =>
        new( Int( item, "ore", 0, 10_000, 0 ), Int( item, "energy", 0, 10_000, 0 ), Int( item, "science", 0, 10_000, 0 ) );
}

internal sealed record HullDefinition( string Name, ResourceCost Cost, int Ticks, int Naval, int HitPoints,
    FlightStats Flight, float Scale, ResearchTopic? Requirement, int ImprovedOre, int ImprovedTicks );

internal sealed record ModuleDefinition( string Name, int Draw, int Supply, int Health, int Damage, int Range, int Reload,
    int Ore, int Energy, int Ticks, float Handling, float Reach, ResearchTopic? Requirement );

internal static class GameCatalog {
    private static readonly Lazy<HullDefinition[]> Hulls = new( ReadHulls );
    private static readonly Lazy<ModuleDefinition[][]> Modules = new( ReadModules );
    private static readonly Lazy<FacilityPlan[]> Facilities = new( ReadFacilities );
    private static readonly Lazy<string[][]> Names = new( ReadNames );
    internal static HullDefinition Hull( HullRole role ) => Hulls.Value[(int)role];
    internal static ReadOnlySpan<string> ShipNames( HullRole role ) => Names.Value[(int)role];
    internal static ReadOnlySpan<ModuleDefinition> Options( ModuleSlot slot ) => Modules.Value[(int)slot];
    internal static ModuleDefinition Module( ModuleSlot slot, int id ) => Modules.Value[(int)slot][id];
    internal static FacilityPlan Facility( PlanetFacility id ) => Facilities.Value[(int)id];

    private static int RoleCount => Enum.GetValues<HullRole>( ).Length;

    internal static void Validate( ) {
        _ = Hulls.Value;
        _ = Modules.Value;
        _ = Facilities.Value;
        _ = Names.Value;
    }

    private static HullDefinition[] ReadHulls( ) {
        using var doc = CatalogJson.Open( "hulls" );
        var table = new HullDefinition[RoleCount];
        foreach ( var row in doc.RootElement.GetProperty( "hulls" ).EnumerateArray( ) ) {
            var role = CatalogJson.EnumId<HullRole>( row.GetProperty( "id" ) );
            if ( table[(int)role] is not null ) throw new InvalidDataException( $"Duplicate hull {role}." );
            table[(int)role] = new( CatalogJson.Text( row, "name", 26 ), CatalogJson.Cost( row ),
                CatalogJson.Int( row, "ticks", 30, 36_000 ), CatalogJson.Int( row, "naval", 0, 20 ),
                CatalogJson.Int( row, "hitPoints", 1, 250 ),
                new( CatalogJson.Number( row, "cruise", 2, 80, 25 ), CatalogJson.Number( row, "acceleration", 5, 120, 55 ),
                    CatalogJson.Number( row, "turn", 0.2f, 2, 1 ), 1 ),
                CatalogJson.Number( row, "scale", 0.45f, 5, 1 ), CatalogJson.Requirement( row ),
                CatalogJson.Int( row, "improvedOre", 0, 100, 0 ), CatalogJson.Int( row, "improvedTicks", 0, 300, 0 ) );
        }
        if ( table.Any( entry => entry is null ) ) throw new InvalidDataException( "Every hull role requires an entry." );
        return table;
    }

    private static ModuleDefinition[][] ReadModules( ) {
        using var doc = CatalogJson.Open( "modules" );
        int[] counts = [2, 2, 3, 3, 2, 3];
        var table = counts.Select( count => new ModuleDefinition[count] ).ToArray( );
        foreach ( var row in doc.RootElement.GetProperty( "modules" ).EnumerateArray( ) ) {
            var slot = CatalogJson.EnumId<ModuleSlot>( row.GetProperty( "slot" ) );
            if ( slot == ModuleSlot.Count ) throw new InvalidDataException( "Unknown module slot." );
            var options = table[(int)slot];
            int id = CatalogJson.Int( row, "id", 0, options.Length - 1 );
            if ( options[id] is not null ) throw new InvalidDataException( $"Duplicate {slot} module {id}." );
            options[id] = new( CatalogJson.Text( row, "name", 26 ), CatalogJson.Int( row, "draw", 0, 50, 0 ),
                CatalogJson.Int( row, "supply", 0, 50, 0 ), CatalogJson.Int( row, "health", 0, 100, 0 ),
                CatalogJson.Int( row, "damage", 0, 70, 0 ), CatalogJson.Int( row, "range", 0, 60, 0 ),
                CatalogJson.Int( row, "reload", -20, 180, 0 ), CatalogJson.Int( row, "ore", 0, 120, 0 ),
                CatalogJson.Int( row, "energy", 0, 100, 0 ), CatalogJson.Int( row, "ticks", 0, 480, 0 ),
                CatalogJson.Number( row, "handling", 0.55f, 1.6f, 1 ), CatalogJson.Number( row, "reach", 1, 1.5f, 1 ),
                CatalogJson.Requirement( row ) );
        }
        if ( table.Any( row => row.Any( entry => entry is null ) ) ) throw new InvalidDataException( "Every module variant requires an entry." );
        return table;
    }

    private static FacilityPlan[] ReadFacilities( ) {
        using var doc = CatalogJson.Open( "structures" );
        var table = new FacilityPlan[FleetSimulation.FacilityCount];
        var seen = new bool[table.Length];
        foreach ( var row in doc.RootElement.GetProperty( "structures" ).EnumerateArray( ) ) {
            int i = (int)CatalogJson.EnumId<PlanetFacility>( row.GetProperty( "id" ) );
            if ( seen[i] ) throw new InvalidDataException( $"Duplicate structure {i}." );
            seen[i] = true;
            table[i] = new( CatalogJson.Text( row, "name", 28 ), CatalogJson.Cost( row ), CatalogJson.Int( row, "ticks", 30, 36_000 ),
                CatalogJson.EnumId<SiteLayer>( row.GetProperty( "layer" ) ), CatalogJson.Int( row, "output", 0, 50, 0 ),
                CatalogJson.Int( row, "upgradeBonus", 0, 30, 0 ), CatalogJson.Int( row, "upgradeTicks", 60, 36_000, 360 ),
                new( CatalogJson.Int( row, "upgradeOre", 0, 1000, 40 ), CatalogJson.Int( row, "upgradeEnergy", 0, 1000, 16 ) ),
                CatalogJson.Requirement( row ) );
        }
        if ( seen.Contains( false ) ) throw new InvalidDataException( "Every structure type requires an entry." );
        return table;
    }

    private static string[][] ReadNames( ) {
        using var doc = CatalogJson.Open( "ship-names" );
        var table = new string[RoleCount][];
        foreach ( var set in doc.RootElement.GetProperty( "names" ).EnumerateObject( ) ) {
            if ( !Enum.TryParse<HullRole>( set.Name, out var role ) || !Enum.IsDefined( role ) || table[(int)role] is not null )
                throw new InvalidDataException( $"Invalid name list {set.Name}." );
            var list = set.Value.EnumerateArray( ).Select( name => FleetSimulation.NormalizeName( name.GetString( ) ?? "" )
                ?? throw new InvalidDataException( $"Invalid ship name in {set.Name}." ) ).ToArray( );
            if ( list.Length == 0 || list.Distinct( StringComparer.Ordinal ).Count( ) != list.Length )
                throw new InvalidDataException( $"Name list {set.Name} must contain distinct names." );
            table[(int)role] = list;
        }
        if ( table.Any( list => list is null ) ) throw new InvalidDataException( "Provide at least one name for each hull role." );
        return table;
    }
}
