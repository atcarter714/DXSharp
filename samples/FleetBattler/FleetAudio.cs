using System.Text.Json;
using System.Windows.Media;

namespace FleetBattler;

internal sealed class FleetAudio : IDisposable {
    private readonly string _folder;
    private readonly Jukebox _jukebox;
    private readonly PlaylistRotation[] _rotations;
    private readonly MusicDirector _director;
    private readonly MediaPlayer _music = new( ), _effect = new( );
    private readonly bool _available;
    private bool _initialized;
    private string? _lastPlayed, _playingTrack;
    private bool _muted;
    private int _playRequest;
    private long _alertUntil;
    private readonly HashSet<string> _badTracks = new( StringComparer.OrdinalIgnoreCase );

    internal FleetAudio( bool muted ) {
        _muted = muted;
        _folder = Path.Combine( AppContext.BaseDirectory, "Music" );
        try {
            _jukebox = Jukebox.Read( Path.Combine( _folder, "jukebox.json" ) );
        } catch ( Exception error ) when ( error is IOException or JsonException or UnauthorizedAccessException ) {
            Console.WriteLine( $"Jukebox configuration: {error.Message}" );
            _jukebox = new( );
        }
        _rotations = _jukebox.CreateRotations( Random.Shared );
        _director = new( _rotations[(int)MusicSituation.System] != _rotations[(int)MusicSituation.Victory] );
        try {
            _music.Volume = _jukebox.MusicVolume;
            _effect.Volume = _jukebox.EffectsVolume;
            _music.MediaEnded += OnEnded;
            _music.MediaFailed += OnFailed;
            _effect.MediaFailed += ( _, error ) => Console.WriteLine( $"Fleet audio cue: {error.ErrorException.Message}" );
            _available = true;
        } catch ( Exception error ) when ( error is InvalidOperationException or NotSupportedException ) {
            Console.WriteLine( $"Fleet audio unavailable: {error.Message}" );
        }
    }

    internal void SetSituation( int system, bool galaxy, bool contested, bool victory ) {
        var previous = _director.Situation;
        bool startVictory = _director.Update( system, galaxy, contested, victory );
        if ( _muted ) _director.FinishVictory( );
        bool continuing = _initialized && _rotations[(int)_director.Situation] == _rotations[(int)previous];
        _initialized = true;
        if ( !_available || _muted || continuing && !startVictory ) return;
        PlayNext( );
    }

    internal void Toggle( ) {
        if ( !_available ) return;
        _muted = !_muted;
        if ( _muted ) {
            ++_playRequest;
            _director.FinishVictory( );
            CloseMusic( );
            _effect.Close( );
        } else if ( _initialized ) {
            PlayNext( );
        }
    }

    internal void Cue( string name ) {
        if ( !_available || _muted ) return;
        bool alert = name == "hostile";
        if ( !alert && Environment.TickCount64 < _alertUntil ) return;
        try {
            _effect.Close( );
            _effect.Open( new Uri( SynthAudio.PathFor( name ) ) );
            _effect.Play( );
            if ( alert ) _alertUntil = Environment.TickCount64 + 500;
        } catch ( Exception error ) when ( error is IOException or UnauthorizedAccessException or InvalidOperationException or NotSupportedException ) {
            Console.WriteLine( $"Fleet audio cue: {error.Message}" );
        }
    }

    private void OnEnded( object? sender, EventArgs args ) {
        if ( _playingTrack is null ) return;
        _director.FinishVictory( );
        PlayNext( );
    }

    private void OnFailed( object? sender, ExceptionEventArgs args ) {
        if ( _playingTrack is not { } track ) return;
        if ( _badTracks.Add( track ) ) Console.WriteLine( $"Fleet music track {track}: {args.ErrorException.Message}" );
        PlayNext( );
    }

    private async void PlayNext( ) {
        if ( _muted || !_available || !_initialized ) return;
        int request = ++_playRequest;
        CloseMusic( );
        var situation = _director.Situation;
        var rotation = _rotations[(int)situation];
        int playlistAttempts = 2 * rotation.Count - 1;
        bool victory = situation == MusicSituation.Victory;
        int attempts = victory ? playlistAttempts : playlistAttempts + 1;
        for ( int attempt = 0; attempt < attempts; ++attempt ) {
            string choice = attempt == playlistAttempts ? _jukebox.Fallback( (int)situation ) : rotation.Next( _lastPlayed );
            if ( _badTracks.Contains( choice ) ) continue;
            try {
                string? file = Resolve( choice );
                if ( file is null || !File.Exists( file ) ) {
                    if ( _badTracks.Add( choice ) ) Console.WriteLine( $"Fleet music track unavailable: {choice}" );
                    continue;
                }
                if ( Path.GetExtension( file ).Equals( ".ogg", StringComparison.OrdinalIgnoreCase ) ) {
                    string source = file;
                    file = await Task.Run( ( ) => VorbisTrack.Prepare( source ) );
                    if ( request != _playRequest ) return;
                }
                _playingTrack = choice;
                _lastPlayed = choice;
                _music.Open( new Uri( file ) );
                _music.Play( );
                return;
            } catch ( Exception error ) when ( error is IOException or InvalidDataException or UnauthorizedAccessException or InvalidOperationException or NotSupportedException or ArgumentException ) {
                if ( request != _playRequest ) return;
                CloseMusic( );
                if ( _badTracks.Add( choice ) ) Console.WriteLine( $"Fleet music track {choice}: {error.Message}" );
            }
        }
        if ( victory ) {
            _director.FinishVictory( );
            PlayNext( );
        }
    }

    private void CloseMusic( ) {
        _playingTrack = null;
        _music.Close( );
    }

    private string? Resolve( string choice ) {
        if ( choice.StartsWith( '@' ) ) return SynthAudio.PathFor( choice[1..] );
        if ( Path.IsPathRooted( choice ) ) return null;
        string path = Path.GetFullPath( choice, _folder );
        string relative = Path.GetRelativePath( _folder, path );
        return relative == ".." || relative.StartsWith( ".." + Path.DirectorySeparatorChar, StringComparison.Ordinal ) ? null : path;
    }

    public void Dispose( ) {
        if ( !_available ) return;
        _muted = true;
        ++_playRequest;
        _music.MediaEnded -= OnEnded;
        _music.MediaFailed -= OnFailed;
        _effect.Close( );
        CloseMusic( );
    }
}
