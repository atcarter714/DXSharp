using System.Numerics;

namespace FleetBattler;

internal enum FactionId { Sol, Raiders, Merchants, Rival, Compact, Consortium, Asteri, Veyran, Count }
internal enum Disposition { Allied, Neutral, Hostile }

internal sealed class CampaignFaction {
    private readonly SortedSet<ResearchTopic> _projects = [];
    internal string Name { get; set; } = Factions.Name( FactionId.Sol );
    internal FleetSimulation? ResearchSystem { get; set; }
    internal IEnumerable<ResearchTopic> Projects => _projects;
    internal int Extraction { get; private set; }
    internal int Survey { get; private set; }
    internal int Upkeep { get; private set; }
    internal int Damage { get; private set; }
    internal int Range { get; private set; }
    internal int Scan { get; private set; }
    internal EffectStyle? PulseEffect { get; private set; }

    internal bool Knows( ResearchTopic topic ) => _projects.Contains( topic );

    internal void Unlock( ResearchTopic topic ) {
        if ( !_projects.Add( topic ) || topic < ResearchTopic.Count ) return;
        var project = FleetSimulation.Tech( topic );
        Extraction += project.ExtractionBonus;
        Survey += project.SurveyBonus;
        Upkeep += project.UpkeepReduction;
        Damage += project.Damage;
        PulseEffect = project.PulseEffect ?? PulseEffect;
        Range += project.Range;
        Scan = Math.Max( Scan, project.ScanRate );
    }
}

internal static class Factions {
    private sealed record Identity( string Name, Vector4 Tint, HashSet<FactionId> Hostiles );
    private static readonly Identity[] Entries = Load( );
    internal static string Name( FactionId id ) => Entries[(int)id].Name;
    internal static Vector4 ShipTint( FactionId id ) => Entries[(int)id].Tint;
    internal static Disposition Attitude( FactionId from, FactionId toward ) => from == toward ? Disposition.Allied :
        Entries[(int)from].Hostiles.Contains( toward ) || Entries[(int)toward].Hostiles.Contains( from ) ? Disposition.Hostile : Disposition.Neutral;

    private static Identity[] Load( ) {
        using var data = FleetContent.Read( "factions", "factions" );
        var entries = new Identity[(int)FactionId.Count];
        foreach ( var row in data.RootElement.GetProperty( "factions" ).EnumerateArray( ) ) {
            var id = CatalogJson.EnumId<FactionId>( row.GetProperty( "id" ) );
            var color = row.GetProperty( "tint" ).EnumerateArray( ).Select( c => c.GetSingle( ) ).ToArray( );
            if ( id == FactionId.Count || color.Length != 4 || color.Any( c => !float.IsFinite( c ) || c < 0 ) )
                throw new InvalidDataException( $"Invalid faction {id}." );
            var hostiles = row.GetProperty( "hostiles" ).EnumerateArray( ).Select( CatalogJson.EnumId<FactionId> ).ToHashSet( );
            if ( hostiles.Contains( id ) || hostiles.Contains( FactionId.Count ) ) throw new InvalidDataException( $"Invalid rival for {id}." );
            entries[(int)id] = new( CatalogJson.Text( row, "name", 36 ), new( color[0], color[1], color[2], color[3] ), hostiles );
        }
        if ( entries.Any( e => e is null ) ) throw new InvalidDataException( "Missing faction definition." );
        return entries;
    }
}
