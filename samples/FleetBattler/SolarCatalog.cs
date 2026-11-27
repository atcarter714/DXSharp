using System.Numerics;

namespace FleetBattler;

internal readonly record struct SolarBody( string Name, string Kind, int Parent, float Axis, float Eccentricity,
    float Radius, float Phase, float Period, Vector3 Color, bool Rings = false, float PhysicalAxis = 0,
    int Habitability = 0, BodyClass Class = BodyClass.SmallBody, float RotationHours = 24, bool RetrogradeOrbit = false, string Id = "" );

internal static class SolarCatalog {
    internal const float Spacing = 3.2f;
    internal const double EarthYearDays = 365.25636;
    internal const int EarthYearTicks = 14_400;
    internal static double CalendarDays( int tick ) => Math.Max( 0, tick ) * (EarthYearDays / EarthYearTicks);
    internal const float MainBeltInnerAu = 2.1f, MainBeltOuterAu = 3.3f;
    internal const float KuiperInnerAu = 30, KuiperOuterAu = 50;
    internal const int Ceres = 11, Vesta = 12, Pluto = 13, Halley = 14, Charon = 15, Europa = 16, Titan = 17, Triton = 18;
    // Planetary a and e are rounded from the JPL J2000 approximate elements. Phases, sizes,
    // and distances are compressed for the tactical map. Periods share the campaign calendar.
    private static readonly SolarBody[] Catalog = Load();

    private static SolarBody[] Load() {
        using var data = FleetContent.Read( "bodies", "bodies" );
        var bodies = new SolarBody[CelestialSystem.BodyCount];
        foreach ( var row in data.RootElement.GetProperty( "bodies" ).EnumerateArray() ) {
            int i = CatalogJson.Int( row, "index", 0, bodies.Length - 1 );
            if ( bodies[i].Name is not null ) throw new InvalidDataException( $"Duplicate body index {i}." );
            int parent = CatalogJson.Int( row, "parent", i == 0 ? -1 : 0, i - 1 );
            float au = CatalogJson.Number( row, "axisAu", 0.001f, 1000, 0 );
            float axis = au > 0 ? Axis( au ) : CatalogJson.Number( row, "mapAxis", 0, 100, 0 );
            float days = CatalogJson.Number( row, "periodDays", 0, 1_000_000, Period( au ) );
            if ( i > 0 && (axis <= 0 || days <= 0) ) throw new InvalidDataException( $"Body {i} requires a positive axis and period." );
            var rgb = row.GetProperty( "color" ).EnumerateArray().Select( c => c.GetSingle() ).ToArray();
            if ( rgb.Length != 3 || rgb.Any( c => !float.IsFinite( c ) || c < 0 || c > 1 ) )
                throw new InvalidDataException( $"Invalid color for body {i}." );
            bodies[i] = new( CatalogJson.Text( row, "name", 32 ), CatalogJson.Text( row, "kind", 40 ), parent, axis,
                CatalogJson.Number( row, "eccentricity", 0, 0.999f, 0 ), CatalogJson.Number( row, "radius", 0.001f, 1, 0.05f ),
                CatalogJson.Number( row, "phase", 0, 1, 0 ), days, new( rgb[0], rgb[1], rgb[2] ),
                row.TryGetProperty( "rings", out var rings ) && rings.GetBoolean(),
                row.TryGetProperty( "retrogradeComet", out var comet ) && comet.GetBoolean() ? au : 0,
                CatalogJson.Int( row, "habitability", 0, 100, 0 ),
                row.TryGetProperty( "class", out var type ) ? CatalogJson.EnumId<BodyClass>( type ) : i switch {
                    0 => BodyClass.Star, 2 or 8 => BodyClass.GasGiant, 3 => BodyClass.Moon,
                    1 or 5 or 6 or 7 => BodyClass.Terrestrial, 9 or 10 => BodyClass.IceGiant, _ => BodyClass.SmallBody
                }, CatalogJson.Number( row, "rotationHours", -10000, 10000, 24 ),
                row.TryGetProperty( "retrogradeOrbit", out var reverse ) && reverse.GetBoolean(), CatalogJson.Text( row, "id", 80 ) );
        }
        if ( bodies.Any( body => body.Name is null ) ) throw new InvalidDataException( "Missing solar body." );
        return bodies;
    }

    internal static ref readonly SolarBody Get( int index ) => ref Catalog[index];
    internal static int Find( string id ) => Array.FindIndex( Catalog, body => string.Equals( body.Id, id, StringComparison.OrdinalIgnoreCase ) );

    internal static float SpinRadians( int tick, float interpolation, float hours ) {
        if ( hours == 0 ) return 0;
        double days = (Math.Max( 0, tick - 1 + (double)interpolation )) * (EarthYearDays / EarthYearTicks);
        return (float)(Math.IEEERemainder( days * 24 / hours, 1 ) * Math.Tau);
    }

    internal static void Evaluate( int tick, float radius, Span<BodyPose> bodies ) {
        double days = CalendarDays( tick );
        bodies[0] = new( Vector2.Zero, Catalog[0].Radius * radius );
        for ( int i = 1; i < bodies.Length; ++i ) {
            ref readonly var body = ref Catalog[i];
            var position = bodies[body.Parent].Center +
                TrackOffset( i, body.Phase + days / body.Period ) * radius;
            bodies[i] = new( position, body.Radius * radius );
        }
    }

    internal static Vector2 TrackOffset( int index, double phase ) {
        ref readonly var body = ref Catalog[index];
        if ( body.PhysicalAxis == 0 ) return CelestialSystem.OrbitPoint( body.Axis, body.Eccentricity, body.RetrogradeOrbit ? -phase : phase );
        // Compress the actual radius so the comet still clears the star at perihelion.
        var physical = CelestialSystem.OrbitPoint( body.PhysicalAxis, body.Eccentricity, -phase );
        float radius = physical.Length( );
        return physical * (Axis( radius ) / radius);
    }

    internal static float Axis( float au ) => Spacing * (0.12f + 0.34f * MathF.Log2( 1 + 3 * au ));

    internal static float HeliocentricAxis( int body ) {
        while ( Catalog[body].Parent > 0 ) body = Catalog[body].Parent;
        return Catalog[body].Axis;
    }
    private static float Period( float au ) => (float)EarthYearDays * MathF.Pow( au, 1.5f );
}
