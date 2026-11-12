using System.Numerics;

namespace FleetBattler;

internal readonly record struct SolarBody( string Name, string Kind, int Parent, float Axis, float Eccentricity,
    float Radius, float Phase, float Period, Vector3 Color, bool Rings = false );

internal static class SolarCatalog {
    // Planetary a and e are rounded from the JPL J2000 approximate elements. Phases, sizes,
    // distances and periods are scaled independently for the tactical map.
    private static readonly SolarBody[] Catalog = [
        new( "SUN", "G TYPE STAR", -1, 0, 0, 0.120f, 0, 0, new( 1, 0.80f, 0.43f ) ),
        new( "EARTH", "HOMEWORLD", 0, Axis( 1 ), 0.0167f, 0.052f, 0.22f, Period( 1 ), new( 0.22f, 0.53f, 0.82f ) ),
        new( "JUPITER", "GAS GIANT", 0, Axis( 5.2029f ), 0.0484f, 0.106f, 0.84f, Period( 5.2029f ), new( 0.82f, 0.65f, 0.51f ) ),
        new( "MOON", "EARTH SATELLITE", 1, 0.145f, 0.0549f, 0.023f, 0.17f, 39, new( 0.65f, 0.67f, 0.72f ) ),
        new( "KUIPER BELT", "OUTER PROSPECT", 0, Axis( 43 ), 0.06f, 0.022f, 0.09f, Period( 43 ), new( 0.45f, 0.62f, 0.71f ) ),
        new( "MERCURY", "ROCKY PLANET", 0, Axis( 0.3871f ), 0.2056f, 0.023f, 0.73f, Period( 0.3871f ), new( 0.65f, 0.58f, 0.54f ) ),
        new( "VENUS", "CLOUD WORLD", 0, Axis( 0.7233f ), 0.0068f, 0.047f, 0.54f, Period( 0.7233f ), new( 0.92f, 0.70f, 0.44f ) ),
        new( "MARS", "ROCKY PLANET", 0, Axis( 1.5237f ), 0.0934f, 0.033f, 0.33f, Period( 1.5237f ), new( 0.78f, 0.39f, 0.25f ) ),
        new( "SATURN", "RINGED GAS GIANT", 0, Axis( 9.5367f ), 0.0539f, 0.082f, 0.35f, Period( 9.5367f ), new( 0.79f, 0.68f, 0.50f ), true ),
        new( "URANUS", "ICE GIANT", 0, Axis( 19.1892f ), 0.0473f, 0.063f, 0.70f, Period( 19.1892f ), new( 0.52f, 0.83f, 0.84f ) ),
        new( "NEPTUNE", "ICE GIANT", 0, Axis( 30.0699f ), 0.0086f, 0.061f, 0.92f, Period( 30.0699f ), new( 0.29f, 0.48f, 0.84f ) )
    ];

    internal static ref readonly SolarBody Get( int index ) => ref Catalog[index];

    internal static void Evaluate( int tick, float radius, Span<BodyPose> bodies ) {
        double seconds = tick * FleetSimulation.StepSeconds;
        bodies[0] = new( Vector2.Zero, Catalog[0].Radius * radius );
        for ( int i = 1; i < bodies.Length; ++i ) {
            ref readonly var body = ref Catalog[i];
            var position = bodies[body.Parent].Center +
                CelestialSystem.OrbitPoint( body.Axis * radius, body.Eccentricity, body.Phase + seconds / body.Period );
            bodies[i] = new( position, body.Radius * radius );
        }
    }

    private static float Axis( float au ) => 0.12f + 0.34f * MathF.Log2( 1 + 3 * au );
    private static float Period( float au ) => 128 * MathF.Pow( au, 0.75f );
}
