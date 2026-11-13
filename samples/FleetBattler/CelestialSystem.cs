using System.Numerics;

namespace FleetBattler;

internal readonly record struct BodyPose( Vector2 Center, float Radius );

internal static class CelestialSystem {
    internal const int OuterBelt = 4, FrontierBodyCount = 5, BodyCount = 15;
    internal const float BeltAxis = 1.72f;
    internal const float RockyAxis = 0.45f, GiantAxis = 0.91f, MoonAxis = 0.145f;
    internal const float RockyEccentricity = 0.16f, GiantEccentricity = 0.12f, MoonEccentricity = 0.08f;
    private static readonly string[] Names = ["HELIOS", "PROXIMA", "NEREID", "VESPER", "TETHYS", "PEREGRINE"];
    private static readonly Vector2[] Positions = [new( 0.25f, 0.40f ), new( 0.51f, 0.63f ), new( 0.75f, 0.34f ),
        new( 0.31f, 0.76f ), new( 0.55f, 0.22f ), new( 0.79f, 0.68f )];
    internal static ReadOnlySpan<string> Systems => Names;
    internal static ReadOnlySpan<Vector2> MapPositions => Positions;

    internal static string Name( int system, int body ) => (system, body) switch {
        (0, 0) => "HELIOS", (0, 1) => "AURELIA", (0, 2) => "BRONTES", (0, 3) => "SELENE", (0, 4) => "FAR REACH",
        (1, 0) => "PROXIMA", (1, 1) => "RECEIVER WORLD", (1, 2) => "MAGNUS", (1, 3) => "EMBER", (1, 4) => "CINDER BELT",
        (2, 0) => "NEREID", (2, 1) => "PELAGOS", (2, 2) => "THALASSA", (2, 3) => "ECHO", (2, 4) => "DARK SHOAL",
        (3, 0) => "VESPER", (3, 1) => "HEARTH", (3, 2) => "FARLIGHT", (3, 3) => "CINDER", (3, 4) => "LANTERN BELT",
        (4, 0) => "TETHYS", (4, 1) => "QUILL", (4, 2) => "OBERON", (4, 3) => "WATCH", (4, 4) => "SILENT RING",
        (5, 0) => "PEREGRINE", (5, 1) => "FAR HAVEN", (5, 2) => "CROWN", (5, 3) => "WISP", (5, 4) => "ICE WAKE",
        _ => throw new ArgumentOutOfRangeException( nameof(body) )
    };

    internal static void Evaluate( int system, int tick, float radius, Span<BodyPose> bodies ) {
        if ( system == 0 && bodies.Length == BodyCount ) {
            SolarCatalog.Evaluate( tick, radius, bodies );
            return;
        }
        double seconds = tick * FleetSimulation.StepSeconds;
        var sun = new Vector2( -0.31f + system * 0.045f, -0.33f + system * 0.07f ) * radius;
        var rocky = sun + OrbitPoint( RockyAxis * radius, RockyEccentricity, 0.24 + system * 0.25 + seconds / 115 );
        var giant = sun + OrbitPoint( GiantAxis * radius, GiantEccentricity, 0.98 + system * 0.16 + seconds / 310 );
        var moon = rocky + OrbitPoint( MoonAxis * radius, MoonEccentricity, 0.21 + system * 0.21 + seconds / 39 );
        bodies[0] = new( sun, 0.117f * radius );
        bodies[1] = new( rocky, 0.064f * radius );
        bodies[2] = new( giant, 0.103f * radius );
        bodies[3] = new( moon, 0.026f * radius );
        if ( bodies.Length > OuterBelt )
            bodies[OuterBelt] = new( sun + OrbitPoint( BeltAxis * radius, 0.06f, 0.09 + system * 0.15 + seconds / 860 ), 0.033f * radius );
    }

    internal static Vector2 OrbitPoint( float axis, float eccentricity, double phase ) {
        double mean = (phase - Math.Floor( phase )) * Math.Tau;
        double anomaly = eccentricity > 0.8 ? Math.PI : mean;
        int iterations = eccentricity > 0.8 ? 12 : 5;
        for ( int i = 0; i < iterations; ++i )
            anomaly -= (anomaly - eccentricity * Math.Sin( anomaly ) - mean) / (1 - eccentricity * Math.Cos( anomaly ));
        return new( (float)(axis * (Math.Cos( anomaly ) - eccentricity)),
            (float)(axis * Math.Sqrt( 1 - eccentricity * eccentricity ) * Math.Sin( anomaly )) );
    }

    internal static Vector4 Project( BodyPose body, float boardRadius, Matrix4x4 viewProjection, int width, int height, float halfHeight ) {
        var clip = Vector4.Transform( new Vector4( body.Center.X, 0, body.Center.Y, 1 ), viewProjection );
        return new( (clip.X / clip.W + 1) * width * 0.5f, (1 - clip.Y / clip.W) * height * 0.5f,
            body.Radius * height / (2 * halfHeight), 0 );
    }
}
