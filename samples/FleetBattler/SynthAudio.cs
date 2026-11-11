namespace FleetBattler;

internal static class SynthAudio {
    internal const int SampleRate = 22_050;
    private static readonly int[][] Motifs = [
        [48, 55, 60, 64, 62, 55, 57, 64],
        [50, 57, 62, 65, 67, 62, 57, 53],
        [45, 52, 57, 58, 55, 52, 57, 60],
        [48, 55, 60, 64, 67, 72, 67, 64]
    ];

    internal static string PathFor( string name ) {
        int track = Track( name );
        string folder = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ),
            "DXSharp", "FleetBattler", "Audio-v1" );
        Directory.CreateDirectory( folder );
        string file = Path.Combine( folder, name + ".wav" );
        if ( File.Exists( file ) ) return file;
        string temporary = Path.Combine( folder, Guid.NewGuid( ).ToString( "N" ) + ".tmp" );
        try {
            using ( var stream = new FileStream( temporary, FileMode.CreateNew, FileAccess.Write ) ) WriteWave( stream, track );
            try { File.Move( temporary, file ); }
            catch ( IOException ) when ( File.Exists( file ) ) { }
        } finally { File.Delete( temporary ); }
        return file;
    }

    internal static void WriteWave( Stream stream, string name ) => WriteWave( stream, Track( name ) );

    private static int Track( string name ) => name switch {
        "survey" => 0, "cartography" => 1, "signal" => 2, "arrival" => 3,
        "order" => 4, "launch" => 5, "capture" => 6, "laser" => 7, "burst" => 8, _ => throw new ArgumentException( "Unknown procedural audio name.", nameof(name) )
    };

    private static void WriteWave( Stream stream, int track ) {
        int count = track < 4 ? SampleRate * 12 : SampleRate * (track is 4 or 7 ? 16 : 42) / 100;
        using var writer = new BinaryWriter( stream, System.Text.Encoding.UTF8, leaveOpen: true );
        writer.Write( "RIFF"u8 ); writer.Write( 36 + 2 * count ); writer.Write( "WAVEfmt "u8 );
        writer.Write( 16 ); writer.Write( (short)1 ); writer.Write( (short)1 );
        writer.Write( SampleRate ); writer.Write( 2 * SampleRate ); writer.Write( (short)2 ); writer.Write( (short)16 );
        writer.Write( "data"u8 ); writer.Write( 2 * count );
        double seconds = (double)count / SampleRate;
        for ( int i = 0; i < count; ++i ) {
            double time = (double)i / SampleRate;
            double gain = Math.Min( 1, Math.Min( time * 12, (seconds - time) * 8 ) );
            double value = track < 4 ? Music( track, time ) : Cue( track, time, seconds );
            writer.Write( (short)Math.Round( Math.Clamp( value * gain, -1, 1 ) * short.MaxValue ) );
        }
    }

    private static double Music( int track, double time ) {
        double pulse = time * (track == 2 ? 4 : 2);
        int beat = (int)pulse;
        var motif = Motifs[track];
        int root = motif[(beat / 8 * 2) % motif.Length] - 12;
        double chord = Osc( root, time ) * 0.13 + Osc( root + 7, time ) * 0.052 + Osc( root + 12, time ) * 0.037;
        double note = motif[beat % motif.Length];
        double decay = Math.Exp( -(pulse - beat) * (track == 2 ? 4.0 : 3.2) );
        double attack = Math.Min( 1, (pulse - beat) * 24 );
        double shimmer = Osc( note + 12, time ) * 0.13 + Osc( note + 24, time ) * 0.025;
        return chord * (0.62 + 0.38 * Math.Sin( time * 0.7 ) * Math.Sin( time * 0.7 )) + shimmer * decay * attack;
    }

    private static double Cue( int track, double time, double duration ) {
        double pitch = track switch { 4 => 440, 5 => 280, 7 => 890, 8 => 160, _ => 525 };
        double rate = track switch { 4 => 750, 7 => -1500, 8 => -120, _ => 950 };
        double phase = Math.Tau * (pitch * time + rate * time * time);
        double fall = 1 - time / duration;
        return (Math.Sin( phase ) * 0.46 + Math.Sin( phase * 1.5 ) * 0.20) * fall * fall;
    }

    private static double Osc( double note, double time ) => Math.Sin( Math.Tau * 440 * Math.Pow( 2, (note - 69) / 12 ) * time );
}
