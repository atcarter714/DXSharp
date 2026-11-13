using System.Text.Json;

namespace FleetBattler;

internal enum ResearchBranch { Industry, Propulsion }
internal enum ContentArt : uint { Scout, Miner, Corvette, Yard, Mine, Lab }

internal sealed record Technology( string Name, string Benefit, string Description, ContentArt Artwork,
    ResourceCost Cost, int Duration, ResearchBranch Branch, ResearchTopic[] Requires,
    int Damage, int Range, int ScanRate );

internal sealed class ResearchCatalog {
    private static readonly Lazy<ResearchCatalog> Default = new( () => Load( Path.Combine( AppContext.BaseDirectory, "Content", "research.json" ) ) );
    private readonly Technology[] _projects;
    private readonly ResearchTopic[][] _branches;

    private ResearchCatalog( Technology[] projects, ResearchTopic[][] branches ) => (_projects, _branches) = (projects, branches);

    internal static ResearchCatalog Current => Default.Value;
    internal Technology this[ResearchTopic topic] => _projects[(int)topic];
    internal ReadOnlySpan<ResearchTopic> Topics( ResearchBranch branch ) => _branches[(int)branch];

    internal static ResearchCatalog Load( string path ) {
        using var input = File.OpenRead( path );
        using var document = JsonDocument.Parse( input );
        return Parse( document.RootElement );
    }

    internal static ResearchCatalog Parse( JsonElement root ) {
        if ( Number( root, "version", 1, 1 ) != 1 ) throw new InvalidDataException( "Unsupported research catalog version." );
        var projects = new Technology[(int)ResearchTopic.Count];
        var branches = new List<ResearchTopic>[] { [], [] };
        foreach ( var item in root.GetProperty( "projects" ).EnumerateArray( ) ) {
            var topic = Named<ResearchTopic>( item, "id" );
            if ( topic == ResearchTopic.Count || projects[(int)topic] is not null )
                throw new InvalidDataException( $"Invalid or duplicate research project {topic}." );
            var branch = Named<ResearchBranch>( item, "branch" );
            var prerequisites = item.GetProperty( "requires" ).EnumerateArray( ).Select( value => Named<ResearchTopic>( value ) ).ToArray( );
            if ( prerequisites.Contains( ResearchTopic.Count ) || prerequisites.Distinct( ).Count( ) != prerequisites.Length )
                throw new InvalidDataException( $"Invalid prerequisites for {topic}." );
            projects[(int)topic] = new( Copy( item, "name", 36 ), Copy( item, "benefit", 64 ), Copy( item, "description", 640 ),
                Named<ContentArt>( item, "artwork" ),
                new( Number( item, "ore", 0, 400, 0 ), Number( item, "energy", 0, 400, 0 ), Number( item, "science", 0, 400, 0 ) ),
                Number( item, "work", 1, 36_000 ), branch, prerequisites,
                Number( item, "damageBonus", 0, 60, 0 ), Number( item, "rangeBonus", 0, 60, 0 ),
                Number( item, "scanRate", 2, 12, 2 ) );
            branches[(int)branch].Add( topic );
        }
        if ( projects.Any( item => item is null ) ) throw new InvalidDataException( "The research catalog must define every built-in project." );
        if ( branches.Any( branch => branch.Count == 0 ) ) throw new InvalidDataException( "Both research branches must have projects." );
        var marks = new byte[projects.Length];
        for ( int i = 0; i < projects.Length; ++i ) Visit( (ResearchTopic)i );
        return new( projects, [branches[0].ToArray( ), branches[1].ToArray( )] );

        void Visit( ResearchTopic topic ) {
            ref byte mark = ref marks[(int)topic];
            if ( mark == 1 ) throw new InvalidDataException( $"Research prerequisite cycle at {topic}." );
            if ( mark == 2 ) return;
            mark = 1;
            foreach ( var required in projects[(int)topic].Requires ) Visit( required );
            mark = 2;
        }
    }

    private static string Copy( JsonElement item, string key, int max ) {
        string? value = item.GetProperty( key ).GetString( );
        if ( string.IsNullOrWhiteSpace( value ) || value.Length > max )
            throw new InvalidDataException( $"Research field {key} must contain 1 to {max} characters." );
        return value.Trim( ).ToUpperInvariant( );
    }

    private static int Number( JsonElement item, string key, int min, int max, int? fallback = null ) {
        if ( !item.TryGetProperty( key, out var value ) && fallback is { } amount ) return amount;
        if ( value.ValueKind == JsonValueKind.Number && value.TryGetInt32( out int parsed ) && parsed >= min && parsed <= max ) return parsed;
        throw new InvalidDataException( $"Research field {key} must be an integer from {min} to {max}." );
    }

    private static T Named<T>( JsonElement item, string key ) where T : struct, Enum => Named<T>( item.GetProperty( key ) );
    private static T Named<T>( JsonElement value ) where T : struct, Enum {
        string? name = value.GetString( );
        if ( name is not null && Enum.TryParse( name, ignoreCase: true, out T result ) &&
             string.Equals( Enum.GetName( result ), name, StringComparison.OrdinalIgnoreCase ) ) return result;
        throw new InvalidDataException( $"Unknown {typeof(T).Name} '{name}'." );
    }
}
