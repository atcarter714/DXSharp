using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Samples;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal readonly record struct ThrusterEmitter( Vector3 Position, Vector3 Direction, float Width );

internal static class FleetDraw {
    internal const uint InstanceStride = 48, MaterialConstants = 28;

    private const string Shader = """
        cbuffer Scene : register(b0) { row_major float4x4 ViewProjection; float4 Surface; float4 Emissive; float4 StarLight; };
        Texture2D BaseColor : register(t0);
        SamplerState SurfaceSampler : register(s0);
        struct Input {
            float2 uv : TEXCOORD4;
            float3 position : POSITION;
            float3 normal : NORMAL;
            float3 color : COLOR;
            float2 center : TEXCOORD0;
            float2 heading : TEXCOORD1;
            float4 tint : TEXCOORD2;
            float4 motion : TEXCOORD3;
        };
        struct Output { float4 position : SV_Position; float3 color : COLOR; float2 uv : TEXCOORD0; nointerpolation float construction : TEXCOORD1; float3 normal : NORMAL; float3 light : TEXCOORD2; };
        Output VS(Input input) {
            float2 right = float2(-input.heading.y, input.heading.x);
            float scale = input.motion.z > 0 ? input.motion.z : 1;
            float2 xz = input.center + (right * input.position.x - input.heading * input.position.z) * scale;
            float2 normalXZ = right * input.normal.x - input.heading * input.normal.z;
            float3 normal = float3(normalXZ.x, input.normal.y, normalXZ.y);
            float2 towardStar = StarLight.xy - xz;
            float3 lightDir = StarLight.w > 0 ? normalize(float3(towardStar.x,max(14,length(towardStar)*.31),towardStar.y)) :
                normalize(float3(-.45,.85,-.35));
            float light = .17 + .83 * saturate(dot(normal,lightDir));
            Output output;
            output.position = mul(float4(xz.x, input.position.y * scale, xz.y, 1), ViewProjection);
            output.color = input.color * input.tint.rgb * (light * input.tint.a);
            output.uv = input.uv;
            output.construction = input.motion.w;
            output.normal = normal;
            output.light = lightDir;
            return output;
        }
        float4 PS(Output input) : SV_Target {
            if (input.construction < 0) clip(frac((input.position.x+input.position.y)*0.2)-0.42);
            float3 color = input.color * BaseColor.Sample(SurfaceSampler,input.uv).rgb;
            float3 h = normalize(input.light+float3(0,1,0));
            float gloss = pow(saturate(dot(normalize(input.normal),h)), lerp(64,4,Surface.y));
            color = color*(1-Surface.x*0.10) + lerp(float3(0.04,0.04,0.04),color,Surface.x)*gloss*0.25 + Emissive.rgb;
            return float4(pow(saturate(color), 1.0 / 2.2), 1);
        }
        """;

    internal static PipelineState CreatePipeline( GraphicsDevice device, RootSignature root, ShaderCompiler compiler, bool depthEnabled ) =>
        device.CreateGraphicsPipeline( root, compiler.Compile( Shader, "VS", "vs_6_0" ),
            compiler.Compile( Shader, "PS", "ps_6_0" ), [
                new( "POSITION", VertexFormat.Float3, 0 ),
                new( "NORMAL", VertexFormat.Float3, 12 ),
                new( "COLOR", VertexFormat.Float3, 24 ),
                new( "TEXCOORD", VertexFormat.Float2, 36, 4 ),
                new( "TEXCOORD", VertexFormat.Float2, 0, 0, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float2, 8, 1, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float4, 16, 2, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float4, 32, 3, 1, 1 )
            ], depthEnabled: depthEnabled );

    private const string ThrusterShader = """
        cbuffer Scene : register(b0) { row_major float4x4 ViewProjection; float4 Socket; float4 AxisAndTime; };
        struct Input {
            float2 center : TEXCOORD0;
            float2 heading : TEXCOORD1;
            float4 tint : TEXCOORD2;
            float4 motion : TEXCOORD3;
        };
        struct Output {
            float4 position : SV_Position;
            float2 uv : TEXCOORD0;
            nointerpolation float4 glow : COLOR;
            nointerpolation float phase : TEXCOORD1;
        };
        Output VS(Input input, uint vertex : SV_VertexID) {
            const float2 corners[6] = {float2(-1,0),float2(1,0),float2(-1,1),float2(-1,1),float2(1,0),float2(1,1)};
            float2 uv = corners[vertex % 6];
            float phase = AxisAndTime.w * 19 + input.motion.y * 31.7 + Socket.x * 5;
            float3 axis = AxisAndTime.xyz;
            float3 basis = normalize(cross(abs(axis.y) > .97 ? float3(1,0,0) : float3(0,1,0), axis));
            if (vertex >= 6) basis = cross(axis, basis);
            float length = Socket.w * (3 + 12 * input.motion.x) * (1 + 0.07 * sin(phase));
            float3 local = Socket.xyz + axis * uv.y * length + basis * uv.x * Socket.w * 1.65;
            float2 right = float2(-input.heading.y,input.heading.x);
            float2 world = input.center + (right * local.x - input.heading * local.z) * input.motion.z;
            Output output;
            output.position = mul(float4(world.x,local.y * input.motion.z,world.y,1),ViewProjection);
            output.uv = uv;
            output.glow = float4(input.tint.z > input.tint.x ? float3(1,0.285,0.075) : float3(1,0.14,0.045),input.motion.x);
            output.phase = phase;
            return output;
        }
        float4 PS(Output input) : SV_Target {
            clip(input.glow.a - 0.025);
            float distance = input.uv.y;
            float spread = lerp(0.6, 0.22, distance);
            float radial = input.uv.x / spread;
            float fade = pow(saturate(1 - distance), 2.4);
            float cells = 0.84 + 0.16 * cos(distance * 54 - input.phase);
            float halo = exp(-radial * radial * 1.6) * fade;
            float core = exp(-radial * radial * 10) * fade * cells;
            float3 plasma = (input.glow.rgb * halo * .72 + core * float3(1,.75,.42)) * sqrt(input.glow.a);
            return float4(plasma, 0);
        }
        """;

    internal const int ThrusterConstants = 24;

    internal static PipelineState CreateThrusters( GraphicsDevice device, RootSignature root, ShaderCompiler compiler ) =>
        device.CreateGraphicsPipeline( root, compiler.Compile( ThrusterShader, "VS", "vs_6_0" ),
            compiler.Compile( ThrusterShader, "PS", "ps_6_0" ), [
                new( "TEXCOORD", VertexFormat.Float2, 0, 0, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float2, 8, 1, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float4, 16, 2, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float4, 32, 3, 1, 1 )
            ], PixelFormat.Rgba8Unorm, new() { DepthEnabled = true, DepthWriteEnabled = false, DepthComparison = ComparisonFunction.Less }, additiveBlend: true );

    internal static void RecordThrusters( GraphicsCommandList commands, PipelineState pipeline,
        GpuResource instances, uint count, Matrix4x4 viewProjection, float seconds,
        uint start, ReadOnlySpan<ThrusterEmitter> emitters ) {
        if ( count == 0 || emitters.IsEmpty ) return;
        Span<uint> constants = stackalloc uint[ThrusterConstants];
        MemoryMarshal.Write( MemoryMarshal.AsBytes( constants ), in viewProjection );
        commands.SetGraphicsPipeline( pipeline );
        commands.SetVertexBuffer( 1, instances, InstanceStride, (ulong)start * InstanceStride );
        foreach ( var emitter in emitters ) {
            var socket = new Vector4( emitter.Position, emitter.Width );
            var axis = new Vector4( emitter.Direction, seconds );
            MemoryMarshal.Write( MemoryMarshal.AsBytes( constants[16..] ), in socket );
            MemoryMarshal.Write( MemoryMarshal.AsBytes( constants[20..] ), in axis );
            commands.SetGraphicsConstants( constants );
            commands.Draw( 12, count );
        }
    }

    internal static void Record( GraphicsCommandList commands, PipelineState pipeline, StaticMeshSet meshes,
        GpuResource instances, uint count, Matrix4x4 viewProjection, uint start = 0, Vector2? star = null ) {
        if ( count == 0 ) return;
        commands.SetGraphicsPipeline( pipeline );
        SetScene( commands, viewProjection, star );
        commands.SetVertexBuffer( 1, instances, InstanceStride );
        for ( int i = 0; i < meshes.Count; ++i ) meshes.Draw( commands, i, 16, count, start );
    }

    internal static void SetScene( GraphicsCommandList commands, Matrix4x4 viewProjection, Vector2? star ) {
        ReadOnlySpan<Matrix4x4> constants = [viewProjection];
        commands.SetGraphicsConstants( MemoryMarshal.Cast<Matrix4x4, uint>( constants ) );
        ReadOnlySpan<Vector4> light = [new( star ?? default, 0, star.HasValue ? 1 : 0 )];
        commands.SetGraphicsConstants( MemoryMarshal.Cast<Vector4, uint>( light ), destinationOffset: 24 );
    }
}
