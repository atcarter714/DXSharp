using System.Numerics;

namespace FleetBattler;

internal readonly record struct BodyPose( Vector2 Center, float Radius );

internal static class CelestialSystem {
    internal const int OuterBelt = 4, FrontierBodyCount = 5, BodyCount = 45;
    internal const float FrontierBodyDisplayScale = 1.4f;
    internal const float BeltAxis = 1.72f;
    internal const float RockyAxis = 0.45f, GiantAxis = 0.91f, MoonAxis = 0.145f;
    internal const float RockyEccentricity = 0.16f, GiantEccentricity = 0.12f, MoonEccentricity = 0.08f;
    internal const int SystemCount = 128, AuthoredCount = 6;
    private static readonly string[] Names = ["HELIOS", "PROXIMA", "NEREID", "VESPER", "TETHYS", "PEREGRINE",
        .. Enumerable.Range( 1, SystemCount - AuthoredCount ).Select( number => $"DX {number:D3}" )];
    private static readonly Vector2[] Positions = BuildMap();
    private static Vector2[] BuildMap() {
        List<Vector2> positions = [
            new( .34f, .74f ), new( .24f, .67f ), new( .72f, .64f ),
            new( .24f, .31f ), new( .57f, .19f ), new( .81f, .37f )
        ];
        var random = new Random( 2184 );
        while ( positions.Count < SystemCount ) {
            float radius = .13f + .34f * MathF.Sqrt( random.NextSingle() );
            float arm = random.Next( 3 ) * MathF.Tau / 3;
            float angle = arm + radius * 8.7f + (random.NextSingle() - .5f) * .98f;
            var (sine, cosine) = MathF.SinCos( angle );
            var point = new Vector2( .5f + cosine * radius, .5f + sine * radius );
            if ( positions.All( other => Vector2.DistanceSquared( point, other ) >= .020f * .020f ) )
                positions.Add( point );
        }
        return positions.ToArray();
    }
    internal static ReadOnlySpan<string> Systems => Names;
    internal static ReadOnlySpan<Vector2> MapPositions => Positions;

    internal static string Name( int system, int body ) => (system, body) switch {
        (0, 0) => "HELIOS", (0, 1) => "AURELIA", (0, 2) => "BRONTES", (0, 3) => "SELENE", (0, 4) => "FAR REACH",
        (1, 0) => "PROXIMA", (1, 1) => "RECEIVER WORLD", (1, 2) => "MAGNUS", (1, 3) => "EMBER", (1, 4) => "CINDER BELT",
        (2, 0) => "NEREID", (2, 1) => "PELAGOS", (2, 2) => "THALASSA", (2, 3) => "ECHO", (2, 4) => "DARK SHOAL",
        (3, 0) => "VESPER", (3, 1) => "HEARTH", (3, 2) => "FARLIGHT", (3, 3) => "CINDER", (3, 4) => "LANTERN BELT",
        (4, 0) => "TETHYS", (4, 1) => "QUILL", (4, 2) => "OBERON", (4, 3) => "WATCH", (4, 4) => "SILENT RING",
        (5, 0) => "PEREGRINE", (5, 1) => "FAR HAVEN", (5, 2) => "CROWN", (5, 3) => "WISP", (5, 4) => "ICE WAKE",
        (_, 0) => Names[system],
        (_, 1) => $"{Names[system]} B",
        (_, 2) => $"{Names[system]} C",
        (_, 3) => $"{Names[system]} B I",
        (_, 4) => $"{Names[system]} BELT",
        _ => throw new ArgumentOutOfRangeException( nameof(body) )
    };

    internal static void Evaluate( int system, int tick, float radius, Span<BodyPose> bodies, bool campaign = false ) {
        if ( system == 0 && bodies.Length == BodyCount ) {
            SolarCatalog.Evaluate( tick, radius, bodies );
            return;
        }
        if ( system >= AuthoredCount ) {
            ref readonly var profile = ref Exoplanets.Get( system );
            var primary = new Vector2( -.18f, -.13f ) * radius;
            double campaignDays = SolarCatalog.CalendarDays( tick );
            bodies[0] = new( primary, profile.StarRadius * radius );
            bodies[1] = new( primary + OrbitPoint( profile.InnerAxis * radius, profile.Eccentricity, profile.Phase + campaignDays / profile.InnerPeriod ), profile.InnerRadius * radius );
            bodies[2] = new( primary + OrbitPoint( profile.OuterAxis * radius, GiantEccentricity, profile.Phase + .43 + campaignDays / profile.OuterPeriod ), profile.OuterRadius * radius );
            bodies[3] = new( bodies[1].Center + OrbitPoint( MoonAxis * radius, MoonEccentricity, profile.Phase + campaignDays / 39 ), .022f * radius );
            if ( bodies.Length > OuterBelt )
                bodies[OuterBelt] = new( primary + OrbitPoint( BeltAxis * radius, .06f, profile.Phase + campaignDays / 1100 ), .028f * radius );
            return;
        }
        double days = campaign || bodies.Length == BodyCount ? SolarCatalog.CalendarDays( tick ) : tick * FleetSimulation.StepSeconds;
        var sun = new Vector2( -0.31f + system * 0.045f, -0.33f + system * 0.07f ) * radius;
        var rocky = sun + OrbitPoint( RockyAxis * radius, RockyEccentricity, 0.24 + system * 0.25 + days / 115 );
        var giant = sun + OrbitPoint( GiantAxis * radius, GiantEccentricity, 0.98 + system * 0.16 + days / 310 );
        var moon = rocky + OrbitPoint( MoonAxis * radius, MoonEccentricity, 0.21 + system * 0.21 + days / 39 );
        bodies[0] = new( sun, 0.117f * radius );
        bodies[1] = new( rocky, 0.064f * radius );
        bodies[2] = new( giant, 0.103f * radius );
        bodies[3] = new( moon, 0.026f * radius );
        if ( bodies.Length > OuterBelt )
            bodies[OuterBelt] = new( sun + OrbitPoint( BeltAxis * radius, 0.06f, 0.09 + system * 0.15 + days / 860 ), 0.033f * radius );
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

    internal static Vector4 Project( BodyPose body, float boardRadius, Matrix4x4 viewProjection, int width, int height, float halfHeight, float displayScale = 1 ) {
        var clip = Vector4.Transform( new Vector4( body.Center.X, 0, body.Center.Y, 1 ), viewProjection );
        return new( (clip.X / clip.W + 1) * width * 0.5f, (1 - clip.Y / clip.W) * height * 0.5f,
            body.Radius * displayScale * height / (2 * halfHeight), 0 );
    }
}

internal enum BodyClass { Star, Terrestrial, SuperEarth, IceGiant, GasGiant, Moon, SmallBody }

internal readonly record struct ExoplanetSystem( int Temperature, float StarRadius, float InnerAxis, float OuterAxis,
    float InnerPeriod, float OuterPeriod, float InnerRadius, float OuterRadius, float Eccentricity, float Phase,
    BodyClass InnerClass, BodyClass OuterClass, int Habitability );

internal static class Exoplanets {
    private static readonly ExoplanetSystem[] Profiles = Enumerable.Range( 0, CelestialSystem.SystemCount )
        .Select( Generate ).ToArray();

    internal static ref readonly ExoplanetSystem Get( int system ) => ref Profiles[system];

    private static ExoplanetSystem Generate( int index ) {
        var random = new Random( 8239 + index * 997 );
        float population = random.NextSingle();
        var (mass, luminosity, temperature) = population switch {
            < .70f => (.35f, .022f, 3400),
            < .87f => (.72f, .24f, 4550),
            < .97f => (1f, 1f, 5750),
            _ => (1.25f, 2.6f, 6500)
        };
        float innerAu = MathF.Sqrt( luminosity ) * MathF.Pow( 2, (random.NextSingle() - .5f) * 4 );
        float outerAu = innerAu * (2.8f + random.NextSingle() * 2);
        float insolation = luminosity / (innerAu * innerAu);
        int score = Math.Clamp( 65 - (int)(MathF.Abs( MathF.Log2( insolation ) ) * 23) - random.Next( 3, 30 ), 0, 65 );
        // A temperate orbit is commoner than an atmosphere humans can use.
        if ( insolation is > .72f and < 1.28f && random.NextSingle() < .14f ) score = random.Next( 78, 94 );
        return new( temperature, .083f + .026f * mass, .48f + random.NextSingle() * .10f,
            .99f + random.NextSingle() * .2f, Period( innerAu ), Period( outerAu ),
            .033f + random.NextSingle() * .024f, .071f + random.NextSingle() * .041f,
            random.NextSingle() * .19f, random.NextSingle(),
            random.NextSingle() < .60f ? BodyClass.Terrestrial : BodyClass.SuperEarth,
            random.NextSingle() < .60f ? BodyClass.IceGiant : BodyClass.GasGiant, score );

        float Period( float axis ) => 365.25636f * MathF.Sqrt( axis * axis * axis / mass );
    }
}
