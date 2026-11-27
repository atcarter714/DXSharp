using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Graphics;
using DXSharp.Shaders;
using StbImageSharp;

namespace FleetBattler;

internal static class SpaceDraw {
    internal const uint Constants = 63;
    internal static readonly Vector4 Cyan = new( 0.20f, 0.83f, 1.0f, 1 );
    internal static readonly Vector4 Amber = new( 1.0f, 0.58f, 0.24f, 1 );
    internal static readonly Vector4 Neutral = new( 0.81f, 0.88f, 0.66f, 1 );

    private const string Shader = """
        Texture2D<float4> MoonMap : register(t0);
        SamplerState MoonSampler : register(s0);
        cbuffer Scene : register(b0) {
            row_major float4x4 InverseViewProjection;
            float4 Viewport;
            float4 Star;
            float4 Rocky;
            float4 Giant;
            float4 Moon;
            float4 Orbits;
            float4 MoonOrbit;
            float4 Site;
            float4 SiteTint;
            float4 Selected;
            float4 Order;
            float3 BodySpin;
        };

        float4 VS(uint id : SV_VertexID) : SV_Position {
            return float4(float2((id << 1) & 2, id & 2) * float2(2,-2) + float2(-1,1), 0.9999, 1);
        }

        float Hash(float2 p) {
            float3 q = frac(p.xyx * 0.1031);
            q += dot(q, q.yzx + 33.33);
            return frac((q.x + q.y) * q.z);
        }

        float Noise(float2 p) {
            float2 i = floor(p), f = frac(p);
            f = f * f * (3 - 2*f);
            return lerp(lerp(Hash(i), Hash(i + float2(1,0)), f.x),
                        lerp(Hash(i + float2(0,1)), Hash(i + 1), f.x), f.y);
        }

        float3 Stars(float2 p, float cells, float threshold) {
            float2 q = p * cells, id = floor(q);
            float seed = Hash(id);
            float2 center = 0.2 + 0.6 * float2(Hash(id+17.7), Hash(id+69.1));
            float radius = lerp(0.034, 0.12, seed);
            float core = 1 - smoothstep(radius * 0.22, radius, length(frac(q) - center));
            return step(threshold, seed) * core * lerp(float3(0.41,0.58,0.93), float3(1,0.8,0.52), Hash(id+11.1));
        }

        float Line(float distance, float thickness) {
            return 1 - smoothstep(thickness, thickness + Viewport.w * 1.4, abs(distance));
        }

        float Orbit(float2 worldPosition, float2 focus, float axis, float eccentricity) {
            float2 delta = worldPosition - focus + float2(axis * eccentricity, 0);
            delta.y *= rsqrt(1 - eccentricity * eccentricity);
            return Line(length(delta) - axis, Viewport.w * 0.45);
        }

        """ + SpaceMaterials.Hlsl + """

        float3 Body(float3 sky, float2 pixel, float4 body, uint kind, float spin = 0) {
            float2 p = (pixel - body.xy) / body.z;
            [branch] if (any(abs(p) > (kind == 0 ? 3.6 : 2.5))) return sky;
            float d = length(p);
            if (kind == 0) {
                return StellarSurface(sky, p, d, Viewport.z, Star.w);
            }

            float2 ringPlane = float2(p.x * 0.947 - p.y * 0.321, p.x * 0.321 + p.y * 0.947);
            float ringDistance = length(ringPlane * float2(1, 3.4));
            float rings = smoothstep(1.19, 1.25, ringDistance) * (1 - smoothstep(1.93, 2.04, ringDistance));
            rings *= 0.38 + 0.57 * smoothstep(-0.3,0.7,sin(ringDistance*59.0));
            float3 ringTint = float3(0.76,0.66,0.48);
            float ringDepth = ringPlane.y * sqrt(3.4*3.4-1);
            float sphereDepth = sqrt(saturate(1-d*d));
            bool ringInFront = ringDepth > sphereDepth;
            if (kind == 2 && !ringInFront) sky = lerp(sky, ringTint, rings*0.65);

            float3 n = float3(p.x, -p.y, sqrt(saturate(1 - d*d)));
            float2 towardSun = Star.xy - body.xy;
            // The projected star direction supplies the sky-plane components. The third
            // component is the signed projection along the orthographic view direction.
            float3 light = normalize(float3(towardSun.x, -towardSun.y, kind == 5 || (kind == 3 && SiteTint.w == 0 && all(body.xy == Site.xy)) ? 0 : body.w) + float3(0,0,0.00001));
            float diffuse = 0.14 + 0.86 * saturate(dot(n, light));
            float3 albedo;
            bool iceGiant = kind == 6;
            if (kind == 1) {
                float longitude = atan2(n.x,n.z) + spin;
                float2 map = float2(cos(longitude),sin(longitude))*sqrt(saturate(1-n.y*n.y)) + n.y*float2(1.3,2.1);
                float land = Noise(map*2.6)*0.54 + Noise(map*6.8)*0.30 + Noise(map*19)*0.16;
                albedo = lerp(float3(0.12,0.38,0.76), float3(0.30,0.62,0.38), smoothstep(0.49,0.60,land));
                albedo = lerp(albedo,float3(0.84,0.92,0.96),smoothstep(0.74,0.93,abs(n.y)+0.14*Noise(map*14)));
                float2 wind = map*float2(5,11) + float2(Noise(map*4 + 3.2), Noise(map*4)*1.9);
                float cloud = smoothstep(0.48,0.77,Noise(wind)*0.68 + Noise(wind*3.7)*0.32);
                albedo = lerp(albedo,float3(0.86,0.94,0.99),cloud*0.58);
                float daylight = smoothstep(-0.19,0.36,dot(n,light));
                float horizon = pow(1-saturate(n.z),3);
                albedo += float3(0.03,0.29,0.59)*horizon*(0.18+daylight);
                float ocean = 1-smoothstep(0.49,0.60,land);
                albedo += float3(0.65,0.78,0.86)*ocean*(1-cloud)*pow(saturate(dot(n,normalize(light+float3(0,0,1)))),42);
                sky += float3(0.06,0.31,0.64)*exp(-max(d-1,0)*34)*(0.12+0.68*daylight);
            } else if (kind == 2 || kind == 4 || iceGiant) {
                float longitude = atan2(n.x,n.z) + spin;
                float2 belts = float2(cos(longitude),sin(longitude)) + n.y*float2(.8,3.1);
                float turbulence = Noise(belts*float2(5,18))*2.8 + Noise(belts*31)*0.8;
                float bands = 0.5 + 0.5*sin(n.y*29 + turbulence);
                albedo = lerp(float3(0.58,0.34,0.28),float3(0.97,0.78,0.56),bands);
                if (iceGiant) albedo = lerp(float3(.16,.32,.67),float3(.55,.84,.93),bands);
                float storm = length(float2(atan2(sin(longitude-.38),cos(longitude-.38))*3.7,(n.y+0.27)*10));
                if (!iceGiant) albedo = lerp(albedo,float3(0.71,0.32,0.19)*(0.85+0.15*cos(storm*17)),1-smoothstep(0.65,1.1,storm));
            } else if (kind == 5) {
                albedo = float3(0.76,0.42,0.30) * (0.62 + 0.33*Noise(p*11));
            } else if (kind == 3 && MoonOrbit.w > 0) {
                float2 uv = float2(0.5 + atan2(n.x,max(n.z,0.0001)) / 6.2831853, acos(clamp(n.y,-1,1)) / 3.14159265);
                albedo = MoonMap.SampleLevel(MoonSampler,float2(frac(uv.x+spin/6.2831853),clamp(uv.y,0.002,0.998)),0).rgb;
                return lerp(sky,pow(saturate(albedo*diffuse),1.0/2.2),1-smoothstep(0.97,1.015,d));
            } else {
                albedo = Regolith(n, light, spin);
                diffuse = 1;
            }
            sky = lerp(sky, albedo * diffuse, 1 - smoothstep(0.97,1.015,d));
            if (kind == 2 && ringInFront) sky = lerp(sky,ringTint,rings*0.65);
            return sky;
        }

        float4 PS(float4 position : SV_Position) : SV_Target {
            float2 pixel = position.xy;
            float2 p = (pixel - 0.5*Viewport.xy) / Viewport.y;
            float2 dust = p * float2(3.2,5.8) + float2(1.2,-1.8);
            float nebula = saturate(Noise(dust)*0.66 + Noise(dust*2.5)*0.34 - 0.30);
            float band = exp(-abs(p.y - p.x*0.34 + 0.12)*4.1);
            float3 sky = float3(0.008,0.011,0.022) + float3(0.067,0.044,0.096)*nebula*band
                       + float3(0.016,0.058,0.081)*saturate(1-length(p+float2(0.36,-0.23)));
            sky += (Stars(p, 68, 0.69)*0.39 + Stars(p+0.19, 29, 0.88)*0.74) * (Viewport.w == 0 ? .20 : .50);

            if (Viewport.w == 0) {
                float2 center = Star.xy + Star.zw*.5 + Rocky.xy;
                float diameter = max(1,min(Star.z,Star.w))*Rocky.z;
                float2 disk = (pixel-center)/diameter/float2(1.17,.98);
                float r = length(disk);
                float angle = atan2(disk.y,disk.x) - r*8.7;
                float lanes = pow(.5 + .5*cos(angle*3),4) * (1-smoothstep(.30,.53,r));
                float dustLane = .27 + .73*Noise(disk*22 + Noise(disk*7)*2.2);
                float halo = exp(-r*6.2);
                float nucleus = exp(-r*r*220);
                sky = sky*.55 + float3(.21,.34,.48)*lanes*dustLane*.43
                    + float3(.58,.28,.13)*halo*.37 + float3(.57,.32,.15)*nucleus*.47;
                return float4(saturate(sky),1);
            }

            float2 ndc = pixel / Viewport.xy * float2(2,-2) + float2(-1,1);
            float3 nearPoint = mul(float4(ndc,0,1), InverseViewProjection).xyz;
            float3 ray = mul(float4(0,0,1,0), InverseViewProjection).xyz;
            float2 world = (nearPoint - ray*(nearPoint.y/ray.y)).xz;
            float2 field = world * .0095;
            float distortion = Noise(field*.47 + 2.6);
            float clouds = Noise(field*1.9 + distortion*2.3)*.58 + Noise(field*5.7-distortion)*.29 + Noise(field*16)*.13;
            float filament = smoothstep(.43,.82,clouds)*exp(-abs(field.y + field.x*.28)*.17);
            sky += lerp(float3(.020,.037,.052),float3(.069,.037,.077),distortion) * filament;
            bool solar = SiteTint.x > 0.99 && SiteTint.w == 0;
            float tracks = Orbit(world,Orbits.xy,Orbits.z,solar ? 0.0167 : 0.16) + Orbit(world,Orbits.xy,Orbits.w,solar ? 0.0484 : 0.12)
                         + Orbit(world,MoonOrbit.xy,MoonOrbit.z,solar ? 0.0549 : 0.08);
            sky += float3(0.066,0.108,0.172) * saturate(tracks);

            sky = Body(sky,pixel,Star,0);
            sky += StellarFlare(pixel, Star.xy, Star.z, Viewport.xy);
            sky = Body(sky,pixel,Giant,solar ? 4 : fmod(SiteTint.z, 2) > .5 ? 6 : 2, BodySpin.y);
            sky = Body(sky,pixel,Rocky,SiteTint.z > 1.5 ? 7 : 1, BodySpin.x);
            sky = Body(sky,pixel,Moon,3, BodySpin.z);
            if (SiteTint.w == 0 && Site.w > 0) {
                float belt = Orbit(world,Orbits.xy,Site.w,solar ? 0.044 : 0.06);
                sky += float3(0.15,0.17,0.22) * belt;
                sky = Body(sky,pixel,Site,solar ? 5 : 3);
            }

            if (SiteTint.y > 0.99 && SiteTint.w == 0) {
                float haze = Noise(p*14 + float2(7.3,3.1)) * 0.055;
                float luma = dot(sky,float3(0.22,0.68,0.10));
                return float4(saturate(lerp(sky, luma*float3(0.59,0.8,1.13),0.73)*0.72 + haze*float3(0.31,0.56,0.94)),1);
            }

            float2 marker = world - Site.xy;
            float distanceToSite = length(marker);
            float arc = frac(atan2(marker.x,-marker.y) / 6.2831853 + 1);
            float ring = Line(distanceToSite-Site.z, Viewport.w*0.7);
            float crossMark = Line(abs(marker.x)+abs(marker.y) - Viewport.w*7, Viewport.w*0.85);
            float pulse = 0.65 + 0.35*sin(Viewport.z*3.4);
            float segment = step(0.18, frac(arc*12));
            float progress = step(arc, Site.w) * step(0.001,Site.w);
            sky = lerp(sky, SiteTint.rgb, saturate(ring*(segment*0.45 + progress*0.60) + crossMark*pulse)*SiteTint.w);
            sky += SiteTint.rgb * (1 - saturate(distanceToSite/max(Site.z,0.001))) * (0.085*SiteTint.w);

            float2 selectionDelta = world - Selected.xy;
            float selectionRadius = max(Selected.z, Viewport.w*11);
            float selectionRing = Line(length(selectionDelta)-selectionRadius, Viewport.w*0.65);
            float bracket = step(0.7, max(abs(normalize(selectionDelta + 0.0001).x),abs(normalize(selectionDelta + 0.0001).y)));
            sky = lerp(sky,float3(0.28,0.92,1),selectionRing*bracket*0.56*Selected.w);
            float2 orderVector = Order.xy - Selected.xy;
            float orderT = dot(selectionDelta,orderVector) / max(dot(orderVector,orderVector),0.0001);
            float commandDash = step(0.35,frac(orderT*length(orderVector)/(Viewport.w*12)));
            float route = Line(length(selectionDelta - orderVector*saturate(orderT)), Viewport.w*0.68) *
                         step(0,orderT)*step(orderT,1)*commandDash;
            float2 destinationOffset = abs(world-Order.xy);
            float destination = Line(destinationOffset.x+destinationOffset.y-Viewport.w*9, Viewport.w*0.95);
            float3 orderTint = Order.w == 4 ? float3(1,0.48,0.21) : Order.w == 3 ? float3(0.62,1,0.63) : float3(0.30,0.87,0.96);
            sky = lerp(sky,orderTint,saturate((route*0.73*step(Order.z,1.0)+destination)*min(1,Order.z)));
            return float4(saturate(sky),1);
        }
        """;

    internal static SampledTexture2D LoadMoon( GraphicsDevice device, GraphicsCommandList commands, string? path ) {
        if ( path is null ) return device.CreateSampledTexture2D( commands, 1, 1, [255, 255, 255, 255], srgb: true );
        using var stream = File.OpenRead( path );
        var info = ImageInfo.FromStream( stream );
        if ( info is not { Width: > 0 and <= 4096, Height: > 0 and <= 2048 } size || size.Width != 2 * size.Height )
            throw new InvalidDataException( "The Moon map must be a 2:1 equirectangular image no larger than 4096 by 2048." );
        stream.Position = 0;
        var pixels = ImageResult.FromStream( stream, ColorComponents.RedGreenBlueAlpha );
        if ( pixels.Width != size.Width || pixels.Height != size.Height )
            throw new InvalidDataException( "The Moon map dimensions changed during decoding." );
        return device.CreateSampledTexture2D( commands, (uint)pixels.Width, (uint)pixels.Height, pixels.Data, srgb: true );
    }

    internal static PipelineState CreatePipeline( GraphicsDevice device, RootSignature root, ShaderCompiler compiler ) =>
        device.CreateGraphicsPipeline( root, compiler.Compile( Shader, "VS", "vs_6_0" ),
            compiler.Compile( Shader, "PS", "ps_6_0" ), PixelFormat.Rgba8Unorm, depthEnabled: true );

    internal static void Record( GraphicsCommandList commands, PipelineState pipeline, SampledTexture2D moon, bool useMoonMap, FleetSimulation fleet, FleetSelection selected,
        Matrix4x4 viewProjection, Matrix4x4 inverse, int width, int height, float halfHeight, bool showSelection = true, float interpolation = 1 ) {
        float r = fleet.BoardRadius;
        float seconds = ComputeTraffic.RenderTime( fleet.WorldTick, interpolation );
        var bodies = fleet.Bodies;
        float worldPerPixel = halfHeight * 2 / height;
        var center = fleet.VisualCenter( selected, interpolation );
        var plan = fleet.Plan( selected );
        var target = plan.Target >= 0 ? fleet.VisualPosition( plan.Target, interpolation ) : plan.Destination;
        int side = fleet.Capturing >= 0 ? fleet.Capturing : fleet.Owner;
        var tint = side switch { 0 => Cyan, 1 => Amber, _ => Neutral };
        float progress = fleet.CaptureTicks > 0 ? (float)fleet.CaptureTicks / FleetSimulation.CaptureDuration : fleet.Owner >= 0 ? 1 : 0;
        Span<Vector4> values = stackalloc Vector4[16];
        MemoryMarshal.CreateReadOnlySpan( ref inverse, 1 ).CopyTo( MemoryMarshal.Cast<Vector4, Matrix4x4>( values[..4] ) );
        values[4] = new( width, height, seconds, worldPerPixel );
        var towardEye = -Vector3.Normalize( Vector3.TransformNormal( Vector3.UnitZ, inverse ) );
        var star = fleet.VisualBody( 0, interpolation ).Center;
        for ( int i = 0; i < CelestialSystem.OuterBelt; ++i ) {
            var pose = fleet.VisualBody( i, interpolation );
            var projected = CelestialSystem.Project( pose, r, viewProjection, width, height, halfHeight,
                fleet.Frontier ? CelestialSystem.FrontierBodyDisplayScale : 1 );
            var direction = star - pose.Center;
            projected.W = (direction.X * towardEye.X + direction.Y * towardEye.Z) / worldPerPixel;
            if ( i == 0 ) projected.W = fleet.SystemIndex >= CelestialSystem.AuthoredCount ? Exoplanets.Get( fleet.SystemIndex ).Temperature : 5772;
            values[5 + i] = projected;
        }
        values[9] = new( bodies[0].Center.X, bodies[0].Center.Y, (fleet.Solar ? SolarCatalog.Get( 1 ).Axis : fleet.SystemIndex >= CelestialSystem.AuthoredCount ? Exoplanets.Get( fleet.SystemIndex ).InnerAxis : CelestialSystem.RockyAxis) * r, (fleet.Solar ? SolarCatalog.Get( 2 ).Axis : fleet.SystemIndex >= CelestialSystem.AuthoredCount ? Exoplanets.Get( fleet.SystemIndex ).OuterAxis : CelestialSystem.GiantAxis) * r );
        var earth = fleet.VisualBody( 1, interpolation );
        values[10] = new( earth.Center.X, earth.Center.Y, (fleet.Solar ? SolarCatalog.Get( 3 ).Axis : CelestialSystem.MoonAxis) * r, useMoonMap ? 1 : 0 );
        values[11] = new( fleet.Site.X, fleet.Site.Y, fleet.SiteRadius, progress );
        if ( fleet.Frontier ) {
            var belt = CelestialSystem.Project( fleet.VisualBody( CelestialSystem.OuterBelt, interpolation ), r, viewProjection, width, height, halfHeight, fleet.Frontier ? CelestialSystem.FrontierBodyDisplayScale : 1 );
            if ( fleet.ChartOnly ) belt.Z = 0;
            values[11] = belt with { W = (fleet.Solar ? SolarCatalog.Get( CelestialSystem.OuterBelt ).Axis : CelestialSystem.BeltAxis) * r };
        }
        values[12] = fleet.Solar ? new Vector4( 1, 0, 0, 0 ) : fleet.ChartOnly ? new Vector4( 0, 1, 0, 0 ) : fleet.Frontier ? Vector4.Zero : tint;
        if ( fleet.Frontier && fleet.SystemIndex >= CelestialSystem.AuthoredCount ) {
            ref readonly var profile = ref Exoplanets.Get( fleet.SystemIndex );
            // Store independent body material flags without enlarging the root constants.
            values[12].Z = (profile.OuterClass == BodyClass.IceGiant ? 1 : 0) + (profile.Habitability < 70 ? 2 : 0);
        }
        values[13] = new( center.X, center.Y, selected.IsShip ? 2.8f : 3.5f + MathF.Sqrt( fleet.ShipCount( selected.Squad ) ) * 2.3f, showSelection ? 1 : 0 );
        float orderMode = !showSelection || plan.Order == FlightOrder.Idle ? 0
            : !fleet.Frontier ? 1 : 0;
        values[14] = new( target.X, target.Y, orderMode, (float)plan.Order );
        values[15] = new( Spin( 1, 26 ), Spin( 2, 12 ), Spin( 3, 936 ), 0 );
        Draw( commands, pipeline, moon, values );

        float Spin( int body, float fallback ) =>
            SolarCatalog.SpinRadians( fleet.WorldTick, interpolation, fleet.Solar ? SolarCatalog.Get( body ).RotationHours : fallback );
    }

    internal static void RecordGalaxy( GraphicsCommandList commands, PipelineState pipeline, SampledTexture2D moon, int width, int height, bool frontier, GalaxyView chart = default ) {
        Span<Vector4> values = stackalloc Vector4[16];
        values.Clear( );
        values[4] = new( width, height, 0, 0 );
        values[5] = FleetHud.GalaxyAreaPixel( width, height, frontier );
        values[6] = new( chart.Offset.X * width, chart.Offset.Y * height, chart.Scale, 0 );
        Draw( commands, pipeline, moon, values );
    }

    private static void Draw( GraphicsCommandList commands, PipelineState pipeline, SampledTexture2D moon, ReadOnlySpan<Vector4> values ) {
        commands.SetGraphicsPipeline( pipeline );
        commands.SetGraphicsConstants( MemoryMarshal.Cast<Vector4, uint>( values )[..(int)Constants] );
        commands.SetGraphicsTexture( moon, 1 );
        commands.Draw( 3 );
    }
}
