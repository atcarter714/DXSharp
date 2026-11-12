using System.Text.Json;

namespace FleetBattler;

internal sealed class Jukebox {
    private static readonly string[] Situations = ["system", "galaxy", "contested", "victory"];
    private static readonly string[] Defaults = ["@survey", "@cartography", "@signal", "@arrival"];
    private readonly string[][] _tracks = Defaults.Select( track => new[] { track } ).ToArray( );

    internal bool Shuffle { get; private set; }
    internal float MusicVolume { get; private set; } = 0.24f;
    internal float EffectsVolume { get; private set; } = 0.50f;

    internal static Jukebox Read( string path ) {
        if ( !File.Exists( path ) ) return new( );
        using var stream = File.OpenRead( path );
        using var document = JsonDocument.Parse( stream );
        return Parse( document.RootElement );
    }

    internal static Jukebox Parse( JsonElement root ) {
        if ( root.ValueKind != JsonValueKind.Object ) throw new JsonException( "Expected a jukebox configuration object." );
        var result = new Jukebox {
            Shuffle = root.TryGetProperty( "shuffle", out var shuffle ) && shuffle.ValueKind == JsonValueKind.True,
            MusicVolume = Volume( root, "musicVolume", 0.24f ),
            EffectsVolume = Volume( root, "effectsVolume", 0.50f )
        };
        if ( root.TryGetProperty( "playlists", out var lists ) && lists.ValueKind == JsonValueKind.Object ) {
            for ( int i = 0; i < Situations.Length; ++i ) {
                if ( !lists.TryGetProperty( Situations[i], out var items ) || items.ValueKind != JsonValueKind.Array ) continue;
                var tracks = items.EnumerateArray( ).Where( item => item.ValueKind == JsonValueKind.String )
                    .Select( item => item.GetString( )! ).Where( item => !string.IsNullOrWhiteSpace( item ) ).Select( NormalizeTrack ).ToArray( );
                if ( tracks.Length != 0 ) result._tracks[i] = tracks;
            }
        }

        bool procedural = root.TryGetProperty( "useProceduralTransitions", out var setting ) && setting.ValueKind == JsonValueKind.True;
        if ( !procedural && HasFiles( result._tracks[0] ) ) {
            for ( int i = 1; i < Situations.Length; ++i ) {
                var tracks = result._tracks[i];
                if ( tracks.Length == 1 && tracks[0] == Defaults[i] ) result._tracks[i] = result._tracks[0];
            }
        }
        return result;
    }

    internal string[] Tracks( int situation ) => _tracks[situation];
    internal string Fallback( int situation ) => Defaults[ReferenceEquals( _tracks[situation], _tracks[0] ) ? 0 : situation];

    internal PlaylistRotation[] CreateRotations( Random random ) {
        var rotations = new PlaylistRotation[_tracks.Length];
        for ( int i = 0; i < _tracks.Length; ++i ) {
            int equivalent = Array.FindIndex( _tracks, tracks => tracks.SequenceEqual( _tracks[i], StringComparer.OrdinalIgnoreCase ) );
            rotations[i] = equivalent == i ? new( _tracks[i], Shuffle, random ) : rotations[equivalent];
        }
        return rotations;
    }

    private static string NormalizeTrack( string track ) => track.StartsWith( '@' ) &&
        Path.GetExtension( track ).Equals( ".ogg", StringComparison.OrdinalIgnoreCase ) ? track[1..] : track;

    private static bool HasFiles( string[] tracks ) => Array.Exists( tracks, track => !track.StartsWith( '@' ) );
    private static float Volume( JsonElement root, string name, float fallback ) =>
        root.TryGetProperty( name, out var value ) && value.ValueKind == JsonValueKind.Number && value.TryGetSingle( out float level ) &&
        float.IsFinite( level ) ? Math.Clamp( level, 0, 1 ) : fallback;
}

internal sealed class PlaylistRotation {
    private readonly string[] _tracks;
    private readonly bool _shuffle;
    private readonly Random _random;
    private int _next;

    internal PlaylistRotation( string[] tracks, bool shuffle, Random random ) {
        _tracks = shuffle ? tracks.Distinct( StringComparer.OrdinalIgnoreCase ).ToArray( ) : tracks;
        _shuffle = shuffle;
        _random = random;
    }

    internal int Count => _tracks.Length;

    internal string Next( string? lastPlayed ) {
        if ( _shuffle ) {
            if ( _next == 0 ) _random.Shuffle( _tracks.AsSpan( ) );
            if ( _next + 1 < _tracks.Length && StringComparer.OrdinalIgnoreCase.Equals( _tracks[_next], lastPlayed ) )
                (_tracks[_next], _tracks[_next + 1]) = (_tracks[_next + 1], _tracks[_next]);
        }
        string track = _tracks[_next];
        _next = (_next + 1) % _tracks.Length;
        return track;
    }
}
