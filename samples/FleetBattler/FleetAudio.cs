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
    private long _alertUntil, _retryAt, _lastProgress;
    private TimeSpan _position;
    private bool _preparing;
    private PlaylistRotation? _playingRotation;
    private long _startedAt, _fadeOutAt, _contactSince, _quietSince;
    private int _contactSystem = -1;
    private bool _contact;
    private double _volume;
    private const int MinimumPlayMs = 28_000, FadeMs = 2_200;
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
            _volume = _jukebox.MusicVolume;
            _music.Volume = 0;
            _effect.Volume = _jukebox.EffectsVolume;
            _music.MediaEnded += OnEnded;
            _music.MediaFailed += OnFailed;
            _effect.MediaFailed += ( _, error ) => Console.WriteLine( $"Fleet audio cue: {error.ErrorException.Message}" );
            _available = true;
        } catch ( Exception error ) when ( error is InvalidOperationException or NotSupportedException ) {
            Console.WriteLine( $"Fleet audio unavailable: {error.Message}" );
        }
    }

    internal void SetSituation( int system, bool galaxy, bool contested, bool victory, bool title = false ) {
        long now = Environment.TickCount64;
        if ( title || system != _contactSystem ) {
            _contactSystem = system;
            _contact = false;
            _contactSince = _quietSince = 0;
        }
        if ( contested && !title ) {
            _quietSince = 0;
            if ( _contactSince == 0 ) _contactSince = now;
            _contact |= now - _contactSince >= 1_500;
        } else {
            _contactSince = 0;
            if ( _quietSince == 0 ) _quietSince = now;
            if ( now - _quietSince >= 20_000 ) _contact = false;
        }
        _director.Update( system, galaxy, _contact, victory, title );
        if ( _muted ) _director.FinishVictory( );
        _initialized = true;
        if ( !_available || _muted ) return;
        if ( _preparing ) return;
        if ( _playingTrack is null ) {
            if ( now >= _retryAt ) PlayNext();
            return;
        }
        var position = _music.Position;
        if ( _playingRotation != _rotations[(int)_director.Situation] && position.TotalMilliseconds >= MinimumPlayMs && _fadeOutAt == 0 )
            _fadeOutAt = now;
        if ( _fadeOutAt != 0 && now - _fadeOutAt >= FadeMs ) { PlayNext(); return; }
        double gain = Math.Clamp( (now - _startedAt) / (double)FadeMs, 0, 1 );
        if ( _fadeOutAt != 0 ) gain *= 1 - (now - _fadeOutAt) / (double)FadeMs;
        if ( _music.NaturalDuration.HasTimeSpan )
            gain *= Math.Clamp( (_music.NaturalDuration.TimeSpan - position).TotalMilliseconds / FadeMs, 0, 1 );
        _music.Volume = _volume * gain;
        if ( position > _position ) {
            _position = position;
            _lastProgress = now;
        } else if ( now - _lastProgress >= 8000 ) {
            if ( _music.NaturalDuration.HasTimeSpan && _music.NaturalDuration.TimeSpan - position < TimeSpan.FromSeconds( 1 ) )
                FinishPlayingVictory();
            else if ( _badTracks.Add( _playingTrack ) )
                Console.WriteLine( $"Fleet music stopped advancing: {_playingTrack}" );
            PlayNext();
        }
    }

    internal bool Muted => _muted;
    internal TimeSpan MusicPosition => _music.Position;
    internal int MusicPercent => (int)Math.Round( _volume * 100 );
    internal void AdjustMusic( int amount ) {
        if ( _available ) _volume = Math.Clamp( (MusicPercent + amount) / 100d, 0, 1 );
    }

    internal void Toggle( ) {
        if ( !_available ) return;
        _muted = !_muted;
        if ( _muted ) {
            ++_playRequest;
            _preparing = false;
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
        FinishPlayingVictory();
        PlayNext( );
    }

    private void FinishPlayingVictory() {
        if ( _playingRotation == _rotations[(int)MusicSituation.Victory] ) _director.FinishVictory();
    }

    private void OnFailed( object? sender, ExceptionEventArgs args ) {
        if ( _playingTrack is not { } track ) return;
        if ( _badTracks.Add( track ) ) Console.WriteLine( $"Fleet music track {track}: {args.ErrorException.Message}" );
        PlayNext( );
    }

    private async void PlayNext( ) {
        if ( _muted || !_available || !_initialized ) return;
        int request = ++_playRequest;
        _preparing = true;
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
                _playingRotation = rotation;
                _lastPlayed = choice;
                _music.Open( new Uri( file ) );
                _music.Volume = 0;
                _music.Play( );
                _position = TimeSpan.Zero;
                _startedAt = _lastProgress = Environment.TickCount64;
                _preparing = false;
                return;
            } catch ( Exception error ) when ( error is IOException or InvalidDataException or UnauthorizedAccessException or InvalidOperationException or NotSupportedException or ArgumentException ) {
                if ( request != _playRequest ) return;
                CloseMusic( );
                if ( _badTracks.Add( choice ) ) Console.WriteLine( $"Fleet music track {choice}: {error.Message}" );
            }
        }
        _preparing = false;
        _retryAt = Environment.TickCount64 + 5000;
        if ( !victory ) _badTracks.Clear();
        if ( victory ) {
            _director.FinishVictory( );
            PlayNext( );
        }
    }

    private void CloseMusic( ) {
        _playingTrack = null;
        _playingRotation = null;
        _fadeOutAt = 0;
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
