using System.Buffers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FleetBattler;

// Each pack is a directory. Records retain their order when replaced, so presentation
// and deterministic ID assignment agree between machines using the same pack list.
internal static class FleetContent {
    internal static string Bundled => Path.Combine( AppContext.BaseDirectory, "Content" );
    private static string[] _packs = [];
    internal static void Configure( IEnumerable<string> paths ) {
        _packs = paths.Select( Path.GetFullPath ).ToArray( );
        foreach ( string path in _packs )
            if ( !Directory.Exists( path ) ) throw new DirectoryNotFoundException( $"Missing Fleet Battler content pack: {path}" );
    }

    internal static JsonDocument Read( string file, string table ) {
        var records = new Dictionary<string, JsonNode>( StringComparer.OrdinalIgnoreCase );
        foreach ( string directory in new[] { Bundled }.Concat( _packs ) ) {
            string path = Path.Combine( directory, file + ".json" );
            if ( !File.Exists( path ) && directory != Bundled ) continue;
            using var input = File.OpenRead( path );
            var root = JsonNode.Parse( input );
            if ( root?["version"]?.GetValue<int>( ) != 1 || root[table] is not JsonArray rows )
                throw new InvalidDataException( $"{path} requires version 1 and a {table} array." );
            var seen = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
            foreach ( var node in rows ) {
                if ( node is not JsonObject row ) throw new InvalidDataException( $"Expected a record in {path}." );
                string id = row["id"]?.ToString( ) ?? "";
                if ( file == "modules" ) id = $"{row["slot"]}/{id}";
                if ( string.IsNullOrWhiteSpace( id ) || !seen.Add( id ) )
                    throw new InvalidDataException( $"Empty or duplicate ID '{id}' in {path}." );
                if ( file == "models" && row["file"] is { } model )
                    row["file"] = Path.GetFullPath( model.GetValue<string>( ), directory );
                records[id] = row;
            }
        }
        var merged = new JsonArray( records.Values.Select( row => row.DeepClone( ) ).ToArray( ) );
        // Write through Utf8JsonWriter: JsonSerializer needs reflection metadata that NativeAOT disables.
        var buffer = new ArrayBufferWriter<byte>( );
        using ( var writer = new Utf8JsonWriter( buffer ) )
            new JsonObject { ["version"] = 1, [table] = merged }.WriteTo( writer );
        return JsonDocument.Parse( buffer.WrittenMemory );
    }
}
