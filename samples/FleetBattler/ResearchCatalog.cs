using System.Text.Json;

namespace FleetBattler;

internal enum ResearchBranch { Industry, Propulsion, Sensors, Warfare, Infrastructure, Doctrine, Count }
internal enum ContentArt : uint { Scout, Miner, Corvette, Yard, Mine, Lab, Receiver, Flyby, Burn }

internal sealed record Technology( string Name, string Benefit, string Description, ContentArt Artwork,
    ResourceCost Cost, int Duration, ResearchBranch Branch, ResearchTopic[] Requires,
    int Damage, int Range, int ScanRate, string? Exclusive, int UpkeepReduction, int ExtractionBonus, int SurveyBonus, EffectStyle? PulseEffect );

internal sealed class ResearchCatalog {
    private static readonly Lazy<ResearchCatalog> Default = new( () => {
        using var document = FleetContent.Read( "research", "projects" );
        return Parse( document.RootElement );
    } );
    private readonly Technology[] _projects;
    private readonly ResearchTopic[][] _branches;

    private readonly Dictionary<string, ResearchTopic> _ids;
    private ResearchCatalog( Technology[] projects, ResearchTopic[][] branches, Dictionary<string, ResearchTopic> ids ) =>
        (_projects, _branches, _ids) = (projects, branches, ids);
    internal int Count => _projects.Length;
    internal ResearchTopic Resolve( string id ) => _ids.TryGetValue( id, out var topic ) ? topic
        : throw new InvalidDataException( $"Unknown research project '{id}'." );

    internal static ResearchCatalog Current => Default.Value;
    internal Technology this[ResearchTopic topic] => _projects[(int)topic];
    internal ReadOnlySpan<ResearchTopic> Topics( ResearchBranch branch ) {
        var all = _branches[(int)branch % (int)ResearchBranch.Count];
        int offset = (int)branch / (int)ResearchBranch.Count * 4;
        return all.AsSpan( offset, Math.Min( 4, all.Length - offset ) );
    }
    internal int PageCount( ResearchBranch branch ) => (_branches[(int)branch % (int)ResearchBranch.Count].Length + 3) / 4;
    internal ResearchBranch NextPage( ResearchBranch branch ) {
        int count = (int)ResearchBranch.Count;
        return (ResearchBranch)((int)branch % count + ((int)branch / count + 1) % PageCount( branch ) * count);
    }
    internal ResearchBranch PageOf( ResearchTopic topic ) {
        var branch = this[topic].Branch;
        return branch + Array.IndexOf( _branches[(int)branch], topic ) / 4 * (int)ResearchBranch.Count;
    }

    internal static ResearchCatalog Load( string path ) {
        using var input = File.OpenRead( path );
        using var document = JsonDocument.Parse( input );
        return Parse( document.RootElement );
    }

    internal static ResearchCatalog Parse( JsonElement root ) {
        if ( Number( root, "version", 1, 1 ) != 1 ) throw new InvalidDataException( "Unsupported research catalog version." );
        var ids = Enum.GetValues<ResearchTopic>( ).Where( t => t != ResearchTopic.Count )
            .ToDictionary( t => t.ToString( ), t => t, StringComparer.OrdinalIgnoreCase );
        var rows = root.GetProperty( "projects" );
        foreach ( var row in rows.EnumerateArray( ) ) {
            string id = row.GetProperty( "id" ).GetString( ) ?? "";
            if ( id.Length is 0 or > 80 || id.Equals( "Count", StringComparison.OrdinalIgnoreCase ) ||
                !id.All( c => char.IsAsciiLetterOrDigit( c ) || c is '.' or '-' or '_' ) )
                throw new InvalidDataException( $"Invalid research ID '{id}'." );
            ids.TryAdd( id, (ResearchTopic)ids.Count );
        }
        ResearchTopic Resolve( JsonElement value ) => ids.TryGetValue( value.GetString( ) ?? "", out var found )
            ? found : throw new InvalidDataException( $"Unknown prerequisite '{value}'." );
        var projects = new Technology[ids.Count];
        var branches = Enumerable.Range( 0, (int)ResearchBranch.Count ).Select( _ => new List<ResearchTopic>( ) ).ToArray( );
        foreach ( var item in root.GetProperty( "projects" ).EnumerateArray( ) ) {
            var topic = Resolve( item.GetProperty( "id" ) );
            if ( projects[(int)topic] is not null )
                throw new InvalidDataException( $"Invalid or duplicate research project {topic}." );
            var branch = Named<ResearchBranch>( item, "branch" );
            if ( branch == ResearchBranch.Count ) throw new InvalidDataException( "Unknown research category." );
            var prerequisites = item.GetProperty( "requires" ).EnumerateArray( ).Select( Resolve ).ToArray( );
            if ( prerequisites.Distinct( ).Count( ) != prerequisites.Length )
                throw new InvalidDataException( $"Invalid prerequisites for {topic}." );
            projects[(int)topic] = new( Copy( item, "name", 36 ), Copy( item, "benefit", 64 ), Copy( item, "description", 640 ),
                Named<ContentArt>( item, "artwork" ),
                new( Number( item, "ore", 0, 400, 0 ), Number( item, "energy", 0, 400, 0 ), Number( item, "science", 0, 400, 0 ) ),
                Number( item, "work", 1, 36_000 ), branch, prerequisites,
                Number( item, "damageBonus", 0, 60, 0 ), Number( item, "rangeBonus", 0, 60, 0 ),
                Number( item, "scanRate", 2, 12, 2 ),
                item.TryGetProperty( "exclusive", out _ ) ? Copy( item, "exclusive", 32 ) : null,
                Number( item, "upkeepReduction", 0, 8, 0 ), Number( item, "extractionBonus", 0, 8, 0 ),
                Number( item, "surveyBonus", 0, 30, 0 ),
                item.TryGetProperty( "pulseEffect", out var effect ) ? CatalogJson.EnumId<EffectStyle>( effect ) : null );
            branches[(int)branch].Add( topic );
        }
        if ( projects.Any( item => item is null ) ) throw new InvalidDataException( "The research catalog must define every built-in project." );
        if ( branches.Any( branch => branch.Count == 0 ) ) throw new InvalidDataException( "Research categories require at least one project." );
        var marks = new byte[projects.Length];
        for ( int i = 0; i < projects.Length; ++i ) Visit( (ResearchTopic)i );
        return new( projects, branches.Select( branch => branch.ToArray( ) ).ToArray( ), ids );

        void Visit( ResearchTopic topic ) {
            ref byte mark = ref marks[(int)topic];
            if ( mark == 1 ) throw new InvalidDataException( $"Research prerequisite cycle at {topic}." );
            if ( mark == 2 ) return;
            mark = 1;
            foreach ( var required in projects[(int)topic].Requires ) {
                if ( projects[(int)topic].Exclusive is { } group && projects[(int)required].Exclusive == group )
                    throw new InvalidDataException( $"Conflicting prerequisite for {topic}." );
                Visit( required );
            }
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
