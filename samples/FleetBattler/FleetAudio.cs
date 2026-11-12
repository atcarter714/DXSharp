using System.Text.Json;
using System.Windows.Media;

namespace FleetBattler;

internal sealed class FleetAudio : IDisposable {
    private readonly string _folder;
    private readonly Jukebox _jukebox;
    private readonly MediaPlayer _music = new( ), _effect = new( );
    private readonly bool _available;
    private int _situation = -1, _nextTrack, _consecutiveFailures;
    private bool _muted;
    private int _playRequest;
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

    internal void SetSituation( bool galaxy, bool contested, bool victory ) {
        int situation = galaxy ? 1 : victory ? 3 : contested ? 2 : 0;
        if ( situation == _situation ) return;
        bool continuing = _situation >= 0 && _jukebox.Tracks( situation ).AsSpan( ).SequenceEqual( _jukebox.Tracks( _situation ) );
        _situation = situation;
        if ( continuing ) return;
        _nextTrack = _consecutiveFailures = 0;
        if ( _available && !_muted ) PlayNext( );
    }

    internal void Toggle( ) {
        if ( !_available ) return;
        _muted = !_muted;
        if ( _muted ) {
            ++_playRequest;
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
        if ( !_muted && _situation >= 0 && ++_consecutiveFailures <= _jukebox.Tracks( _situation ).Length ) PlayNext( );
    }

    private async void PlayNext( ) {
        if ( _muted || !_available || _situation < 0 ) return;
        int request = ++_playRequest;
        var tracks = _jukebox.Tracks( _situation );
        for ( int attempt = 0; attempt <= tracks.Length; ++attempt ) {
            string choice = attempt == tracks.Length || _consecutiveFailures >= tracks.Length
                ? _jukebox.Fallback( _situation ) : tracks[_nextTrack];
            _nextTrack = (_nextTrack + 1) % tracks.Length;
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
                _music.Close( );
                _music.Open( new Uri( file ) );
                _music.Play( );
                return;
            } catch ( Exception error ) when ( error is IOException or InvalidDataException or UnauthorizedAccessException or InvalidOperationException or NotSupportedException or ArgumentException ) {
                if ( request != _playRequest ) return;
                if ( _badTracks.Add( choice ) ) Console.WriteLine( $"Fleet music track {choice}: {error.Message}" );
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

    public void Dispose( ) {
        if ( !_available ) return;
        _muted = true;
        ++_playRequest;
        _music.MediaEnded -= OnEnded;
        _music.MediaFailed -= OnFailed;
        _effect.Close( );
        _music.Close( );
    }
}
