using System.Security.Cryptography;
using System.Text;
using NVorbis;

namespace FleetBattler;

internal static class VorbisTrack {
    internal static string Prepare( string source ) {
        var file = new FileInfo( source );
        var key = Convert.ToHexString( SHA256.HashData( Encoding.UTF8.GetBytes( $"{file.FullName}\0{file.Length}\0{file.LastWriteTimeUtc.Ticks}" ) ) );
        string folder = Path.Combine( Path.GetTempPath( ), "DXSharp-FleetBattler", "vorbis-pcm-v1" );
        Directory.CreateDirectory( folder );
        string target = Path.Combine( folder, key + ".wav" );
        if ( File.Exists( target ) ) return target;
        string temporary = Path.Combine( folder, $"{key}.{Guid.NewGuid( ):N}.tmp" );
        try {
            using ( var input = File.OpenRead( source ) )
            using ( var output = File.Create( temporary ) )
                Decode( input, output );
            try { File.Move( temporary, target ); }
            catch ( IOException ) when ( File.Exists( target ) ) { }
            return target;
        } finally {
            File.Delete( temporary );
        }
    }

    internal static void Decode( Stream input, Stream output ) {
        using var reader = new VorbisReader( input, closeOnDispose: false );
        int channels = reader.Channels, rate = reader.SampleRate;
        if ( channels is not (1 or 2) || rate is < 8000 or > 192000 )
            throw new NotSupportedException( "Music requires mono or stereo Ogg Vorbis at 8–192 kHz." );
        using var writer = new BinaryWriter( output, Encoding.UTF8, leaveOpen: true );
        long header = output.Position;
        writer.Write( "RIFF"u8 ); writer.Write( 0 ); writer.Write( "WAVEfmt "u8 );
        writer.Write( 16 ); writer.Write( (short)1 ); writer.Write( (short)channels );
        writer.Write( rate ); writer.Write( rate * channels * 2 ); writer.Write( (short)(channels * 2) ); writer.Write( (short)16 );
        writer.Write( "data"u8 ); writer.Write( 0 );
        var samples = new float[8192];
        long bytes = 0;
        for ( int count; (count = reader.ReadSamples( samples, 0, samples.Length )) > 0; ) {
            bytes += count * 2L;
            if ( bytes > 512 * 1024 * 1024 ) throw new InvalidDataException( "Decoded music exceeds the 512 MiB cache entry limit." );
            if ( count % channels != 0 ) throw new InvalidDataException( "Vorbis decoder returned a partial PCM frame." );
            for ( int i = 0; i < count; ++i )
                writer.Write( (short)Math.Round( Math.Clamp( samples[i], -1, 1 ) * short.MaxValue ) );
        }
        if ( bytes == 0 ) throw new InvalidDataException( "The Ogg Vorbis file contains no audio samples." );
        long end = output.Position;
        output.Position = header + 4;
        writer.Write( (int)bytes + 36 );
        output.Position = header + 40;
        writer.Write( (int)bytes );
        output.Position = end;
    }
}
