using System.Text.Json;
using System.Windows.Media;

namespace FleetBattler;

internal sealed class FleetAudio : IDisposable {
    private static readonly string[] Situations = ["system", "galaxy", "contested", "victory"];
    private static readonly string[] Defaults = ["@survey", "@cartography", "@signal", "@arrival"];
    private readonly string _folder;
    private readonly string[][] _tracks = new string[Situations.Length][];
    private readonly MediaPlayer _music = new( ), _effect = new( );
    private readonly bool _available;
    private int _situation = -1, _nextTrack, _consecutiveFailures;
    private bool _muted;

    internal FleetAudio( bool muted ) {
        _muted = muted;
        _folder = Path.Combine( AppContext.BaseDirectory, "Music" );
        float musicVolume = 0.24f, effectsVolume = 0.50f;
        for ( int i = 0; i < _tracks.Length; ++i ) _tracks[i] = [Defaults[i]];
        try {
            var path = Path.Combine( _folder, "jukebox.json" );
            if ( File.Exists( path ) ) {
                using var config = JsonDocument.Parse( File.ReadAllBytes( path ) );
                var root = config.RootElement;
                musicVolume = Volume( root, "musicVolume", musicVolume );
                effectsVolume = Volume( root, "effectsVolume", effectsVolume );
                if ( root.TryGetProperty( "playlists", out var lists ) ) {
                    for ( int i = 0; i < _tracks.Length; ++i ) {
                        if ( !lists.TryGetProperty( Situations[i], out var items ) ) continue;
                        var files = items.EnumerateArray( ).Select( value => value.GetString( ) )
                            .Where( value => !string.IsNullOrWhiteSpace( value ) ).Cast<string>( ).ToArray( );
                        if ( files.Length > 0 ) _tracks[i] = files;
                    }
                }
            }
        } catch ( Exception error ) when ( error is IOException or JsonException or InvalidOperationException or UnauthorizedAccessException ) {
            Console.WriteLine( $"Jukebox configuration: {error.Message}" );
        }
        try {
            _music.Volume = musicVolume;
            _effect.Volume = effectsVolume;
            _music.MediaEnded += OnEnded;
            _music.MediaFailed += OnFailed;
            _effect.MediaFailed += ( _, error ) => Console.WriteLine( $"Fleet audio cue: {error.ErrorException.Message}" );
            _available = true;
        } catch ( Exception error ) when ( error is InvalidOperationException or NotSupportedException ) {
            Console.WriteLine( $"Fleet audio unavailable: {error.Message}" );
        }
    }

    internal void SetSituation( bool galaxy, bool contested, bool victory ) {
        int situation = galaxy ? 1 : victory ? 3 : contested ? 2 : 0;
        if ( situation == _situation ) return;
        _situation = situation;
        _nextTrack = _consecutiveFailures = 0;
        if ( _available && !_muted ) PlayNext( );
    }

    internal void Toggle( ) {
        if ( !_available ) return;
        _muted = !_muted;
        if ( _muted ) {
            _music.Close( );
            _effect.Close( );
        } else if ( _situation >= 0 ) {
            _nextTrack = _consecutiveFailures = 0;
            PlayNext( );
        }
    }

    internal void Cue( string name ) {
        if ( !_available || _muted ) return;
        try {
            _effect.Close( );
            _effect.Open( new Uri( SynthAudio.PathFor( name ) ) );
            _effect.Play( );
        } catch ( Exception error ) when ( error is IOException or UnauthorizedAccessException or InvalidOperationException or NotSupportedException ) {
            Console.WriteLine( $"Fleet audio cue: {error.Message}" );
        }
    }

    private void OnEnded( object? sender, EventArgs args ) {
        _consecutiveFailures = 0;
        PlayNext( );
    }

    private void OnFailed( object? sender, ExceptionEventArgs args ) {
        Console.WriteLine( $"Fleet music: {args.ErrorException.Message}" );
        if ( ++_consecutiveFailures <= _tracks[_situation].Length ) PlayNext( );
    }

    private void PlayNext( ) {
        if ( _muted || !_available || _situation < 0 ) return;
        var tracks = _tracks[_situation];
        for ( int attempt = 0; attempt <= tracks.Length; ++attempt ) {
            string choice = attempt == tracks.Length || _consecutiveFailures >= tracks.Length
                ? Defaults[_situation] : tracks[_nextTrack++ % tracks.Length];
            try {
                string? file = Resolve( choice );
                if ( file is null || !File.Exists( file ) ) {
                    Console.WriteLine( $"Fleet music track unavailable: {choice}" );
                    continue;
                }
                _music.Close( );
                _music.Open( new Uri( file ) );
                _music.Play( );
                return;
            } catch ( Exception error ) when ( error is IOException or UnauthorizedAccessException or InvalidOperationException or NotSupportedException or ArgumentException ) {
                Console.WriteLine( $"Fleet music track {choice}: {error.Message}" );
            }
        }
    }

    private string? Resolve( string choice ) {
        if ( choice.StartsWith( '@' ) ) return SynthAudio.PathFor( choice[1..] );
        if ( Path.IsPathRooted( choice ) ) return null;
        string path = Path.GetFullPath( choice, _folder );
        string relative = Path.GetRelativePath( _folder, path );
        return relative == ".." || relative.StartsWith( ".." + Path.DirectorySeparatorChar, StringComparison.Ordinal ) ? null : path;
    }

    private static float Volume( JsonElement root, string name, float fallback ) =>
        root.TryGetProperty( name, out var value ) && value.ValueKind == JsonValueKind.Number && value.TryGetSingle( out float level ) &&
        float.IsFinite( level ) ? Math.Clamp( level, 0, 1 ) : fallback;

    public void Dispose( ) {
        if ( !_available ) return;
        _muted = true;
        _music.MediaEnded -= OnEnded;
        _music.MediaFailed -= OnFailed;
        _effect.Close( );
        _music.Close( );
    }
}
