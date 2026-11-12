using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Graphics;
using DXSharp.Shaders;
using StbImageSharp;

namespace FleetBattler;

internal static class SpaceDraw {
    internal const uint Constants = 60;
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

        float3 Body(float3 sky, float2 pixel, float4 body, uint kind) {
            float2 p = (pixel - body.xy) / body.z;
            [branch] if (any(abs(p) > (kind == 0 ? 3.6 : 2.5))) return sky;
            float d = length(p);
            if (kind == 0) {
                float a = atan2(p.y, p.x);
                float corona = exp(-max(d - 0.94, 0) * 6.5) *
                    (0.77 + 0.23 * sin(a * 23 + Noise(p*7)*4 + Viewport.z*0.35));
                float limb = max(d - 0.95, 0);
                float nearGlow = exp(-limb*limb*14.5);
                float farGlow = exp(-limb*limb*1.55) * (1-smoothstep(3.1,3.55,d));
                sky += float3(1,0.35,0.065) * (corona*0.37 + nearGlow*0.16);
                sky += float3(1,0.49,0.16) * farGlow * 0.12;
                float granules = Noise(p*21 + Viewport.z*0.08);
                return lerp(sky, lerp(float3(0.98,0.38,0.08),float3(1,0.94,0.64),0.58 + granules*0.42),
                            1 - smoothstep(0.95, 1.01, d));
            }

            float2 ringPlane = float2(p.x * 0.947 - p.y * 0.321, p.x * 0.321 + p.y * 0.947);
            float ringDistance = length(ringPlane * float2(1, 3.4));
            float rings = smoothstep(1.19, 1.25, ringDistance) * (1 - smoothstep(1.93, 2.04, ringDistance));
            rings *= 0.38 + 0.57 * smoothstep(-0.3,0.7,sin(ringDistance*59.0));
            float3 ringTint = float3(0.76,0.66,0.48);
            if (kind == 2 && ringPlane.y < 0) sky = lerp(sky, ringTint, rings*0.65);

            float3 n = float3(p.x, -p.y, sqrt(saturate(1 - d*d)));
            float2 towardSun = Star.xy - body.xy;
            float3 light = normalize(float3(towardSun.x, -towardSun.y, max(length(towardSun)*0.60, 1)));
            float diffuse = 0.14 + 0.86 * saturate(dot(n, light));
            float3 albedo;
            if (kind == 1) {
                float land = Noise(n.xy*6.5 + n.z*2.7) * 0.72 + Noise(n.xy*16.0)*0.28;
                albedo = lerp(float3(0.12,0.38,0.76), float3(0.30,0.62,0.38), smoothstep(0.49,0.60,land));
                float cloud = smoothstep(0.62,0.83,Noise(n.xy*float2(7,19) + n.z*3));
                albedo = lerp(albedo,float3(0.86,0.94,0.99),cloud*0.58);
                sky += float3(0.16,0.42,0.83) * exp(-abs(d-1.0)*16.0) * 0.24;
            } else if (kind == 2) {
                float bands = 0.5 + 0.5*sin(n.y*27 + Noise(n.xy*5)*2.4 + n.x*0.8);
                albedo = lerp(float3(0.58,0.34,0.28),float3(0.97,0.78,0.56),bands);
            } else if (MoonOrbit.w > 0) {
                float2 uv = float2(0.5 + atan2(n.x,max(n.z,0.0001)) / 6.2831853, acos(clamp(n.y,-1,1)) / 3.14159265);
                albedo = MoonMap.SampleLevel(MoonSampler,float2(frac(uv.x+Viewport.z*0.005),clamp(uv.y,0.002,0.998)),0).rgb;
                return lerp(sky,pow(saturate(albedo*diffuse),1.0/2.2),1-smoothstep(0.97,1.015,d));
            } else {
                albedo = float3(0.69,0.73,0.79) * (0.47 + 0.48*Noise(p*11));
            }
            sky = lerp(sky, albedo * diffuse, 1 - smoothstep(0.97,1.015,d));
            if (kind == 2 && ringPlane.y >= 0) sky = lerp(sky,ringTint,rings*0.65);
            return sky;
        }

        float3 MapNode(float2 pixel, float4 node) {
            float d = length(pixel - node.xy) / max(1, Viewport.y / 720);
            float disk = 1 - smoothstep(4.5,6,d);
            float border = (1 - smoothstep(1,2,abs(d-13))) * (0.38 + node.z*0.55);
            return lerp(float3(0.34,0.52,0.93),float3(0.21,0.88,1),node.z) * (disk+border+0.34*exp(-d*0.14));
        }

        float4 PS(float4 position : SV_Position) : SV_Target {
            float2 pixel = position.xy;
            float2 p = (pixel - 0.5*Viewport.xy) / Viewport.y;
            float2 dust = p * float2(3.2,5.8) + float2(1.2,-1.8);
            float nebula = saturate(Noise(dust)*0.66 + Noise(dust*2.5)*0.34 - 0.30);
            float band = exp(-abs(p.y - p.x*0.34 + 0.12)*4.1);
            float3 sky = float3(0.018,0.026,0.055) + float3(0.12,0.092,0.22)*nebula*band
                       + float3(0.016,0.058,0.081)*saturate(1-length(p+float2(0.36,-0.23)));
            sky += Stars(p, 68, 0.69)*0.39 + Stars(p+0.19, 29, 0.88)*0.74;

            if (Viewport.w == 0) {
                float arm = atan2(p.y,p.x) + length(p)*9.4;
                float spiral = pow(saturate(0.53 + 0.47*sin(3*arm)),3) * exp(-length(p)*4);
                sky += float3(0.10,0.15,0.27) * spiral * (0.45 + 0.55*Noise(p*36));
                sky += MapNode(pixel, Star) + MapNode(pixel, Rocky) + MapNode(pixel, Giant);
                return float4(saturate(sky),1);
            }

            float2 ndc = pixel / Viewport.xy * float2(2,-2) + float2(-1,1);
            float3 nearPoint = mul(float4(ndc,0,1), InverseViewProjection).xyz;
            float3 ray = mul(float4(0,0,1,0), InverseViewProjection).xyz;
            float2 world = (nearPoint - ray*(nearPoint.y/ray.y)).xz;
            float2 orbitWorld = world - ray.xz * (0.11*MoonOrbit.w/ray.y);
            float tracks = Orbit(orbitWorld,Orbits.xy,Orbits.z,0.16) + Orbit(orbitWorld,Orbits.xy,Orbits.w,0.12)
                         + Orbit(orbitWorld,MoonOrbit.xy,MoonOrbit.z,0.08);
            sky += float3(0.066,0.108,0.172) * saturate(tracks);

            sky = Body(sky,pixel,Star,0);
            sky = Body(sky,pixel,Giant,2);
            sky = Body(sky,pixel,Rocky,1);
            sky = Body(sky,pixel,Moon,3);
            if (SiteTint.w == 0 && Site.w > 0) {
                float belt = Orbit(orbitWorld,Orbits.xy,Site.w,0.06);
                sky += float3(0.15,0.17,0.22) * belt;
                sky = Body(sky,pixel,Site,3);
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
            sky += SiteTint.rgb * (1 - saturate(distanceToSite/max(Site.z,0.001))) * 0.085;

            float2 selectionDelta = world - Selected.xy;
            float selectionRadius = max(Selected.z, Viewport.w*11);
            float selectionRing = Line(length(selectionDelta)-selectionRadius, Viewport.w*0.65);
            float bracket = step(0.7, max(abs(normalize(selectionDelta + 0.0001).x),abs(normalize(selectionDelta + 0.0001).y)));
            sky = lerp(sky,float3(0.28,0.92,1),selectionRing*bracket*0.8*Selected.w);
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
        double seconds = fleet.Tick * FleetSimulation.StepSeconds;
        var bodies = fleet.Bodies;
        float worldPerPixel = halfHeight * 2 / height;
        var center = fleet.VisualCenter( selected, interpolation );
        var plan = fleet.Plan( selected );
        var target = plan.Target >= 0 ? fleet.VisualPosition( plan.Target, interpolation ) : plan.Destination;
        int side = fleet.Capturing >= 0 ? fleet.Capturing : fleet.Owner;
        var tint = side switch { 0 => Cyan, 1 => Amber, _ => Neutral };
        float progress = fleet.CaptureTicks > 0 ? (float)fleet.CaptureTicks / FleetSimulation.CaptureDuration : fleet.Owner >= 0 ? 1 : 0;
        Span<Vector4> values = stackalloc Vector4[15];
        MemoryMarshal.CreateReadOnlySpan( ref inverse, 1 ).CopyTo( MemoryMarshal.Cast<Vector4, Matrix4x4>( values[..4] ) );
        values[4] = new( width, height, (float)seconds, worldPerPixel );
        for ( int i = 0; i < bodies.Length; ++i )
            values[5 + i] = CelestialSystem.Project( bodies[i], r, viewProjection, width, height, halfHeight );
        values[9] = new( bodies[0].Center.X, bodies[0].Center.Y, CelestialSystem.RockyAxis * r, CelestialSystem.GiantAxis * r );
        values[10] = new( bodies[1].Center.X, bodies[1].Center.Y, CelestialSystem.MoonAxis * r, useMoonMap ? 1 : 0 );
        values[11] = new( fleet.Site.X, fleet.Site.Y, fleet.SiteRadius, progress );
        if ( fleet.Frontier ) {
            var belt = CelestialSystem.Project( bodies[CelestialSystem.OuterBelt], r, viewProjection, width, height, halfHeight );
            values[11] = belt with { W = CelestialSystem.BeltAxis * r };
        }
        values[12] = fleet.Frontier ? Vector4.Zero : tint;
        values[13] = new( center.X, center.Y, selected.IsShip ? 2.8f : 3.5f + MathF.Sqrt( fleet.ShipCount( selected.Squad ) ) * 2.3f, showSelection ? 1 : 0 );
        float orderMode = !showSelection || plan.Order == FlightOrder.Idle ? 0
            : !fleet.Frontier || plan.Order == FlightOrder.Attack ? 1 : fleet.RouteTicks( selected ) > 0 ? 2 : 0;
        values[14] = new( target.X, target.Y, orderMode, (float)plan.Order );
        Draw( commands, pipeline, moon, values );
    }

    internal static void RecordGalaxy( GraphicsCommandList commands, PipelineState pipeline, SampledTexture2D moon, int currentSystem, int width, int height ) {
        Span<Vector4> values = stackalloc Vector4[15];
        values.Clear( );
        values[4] = new( width, height, 0, 0 );
        var points = CelestialSystem.MapPositions;
        for ( int i = 0; i < points.Length; ++i )
            values[5 + i] = new( points[i].X * width, points[i].Y * height, i == currentSystem ? 1 : 0, 0 );
        Draw( commands, pipeline, moon, values );
    }

    private static void Draw( GraphicsCommandList commands, PipelineState pipeline, SampledTexture2D moon, ReadOnlySpan<Vector4> values ) {
        commands.SetGraphicsPipeline( pipeline );
        commands.SetGraphicsConstants( MemoryMarshal.Cast<Vector4, uint>( values ) );
        commands.SetGraphicsTexture( moon, 1 );
        commands.Draw( 3 );
    }
}
