using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Assets;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal static class FleetDraw {
    internal const uint InstanceStride = 48;

    private const string Shader = """
        cbuffer Scene : register(b0) { row_major float4x4 ViewProjection; };
        struct Input {
            float3 position : POSITION;
            float3 normal : NORMAL;
            float3 color : COLOR;
            float2 center : TEXCOORD0;
            float2 heading : TEXCOORD1;
            float4 tint : TEXCOORD2;
            float4 motion : TEXCOORD3;
        };
        struct Output { float4 position : SV_Position; float3 color : COLOR; };
        Output VS(Input input) {
            float2 right = float2(-input.heading.y, input.heading.x);
            float scale = input.motion.z > 0 ? input.motion.z : 1;
            float2 xz = input.center + (right * input.position.x - input.heading * input.position.z) * scale;
            float2 normalXZ = right * input.normal.x - input.heading * input.normal.z;
            float3 normal = float3(normalXZ.x, input.normal.y, normalXZ.y);
            float light = 0.30 + 0.70 * saturate(dot(normal, normalize(float3(-0.45, 0.85, -0.35))));
            Output output;
            output.position = mul(float4(xz.x, input.position.y * scale, xz.y, 1), ViewProjection);
            output.color = input.color * input.tint.rgb * (light * input.tint.a);
            return output;
        }
        float4 PS(Output input) : SV_Target {
            return float4(pow(saturate(input.color), 1.0 / 2.2), 1);
        }
        """;

    internal static PipelineState CreatePipeline( GraphicsDevice device, RootSignature root, ShaderCompiler compiler, bool depthEnabled ) =>
        device.CreateGraphicsPipeline( root, compiler.Compile( Shader, "VS", "vs_6_0" ),
            compiler.Compile( Shader, "PS", "ps_6_0" ), [
                new( "POSITION", VertexFormat.Float3, 0 ),
                new( "NORMAL", VertexFormat.Float3, 12 ),
                new( "COLOR", VertexFormat.Float3, 24 ),
                new( "TEXCOORD", VertexFormat.Float2, 0, 0, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float2, 8, 1, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float4, 16, 2, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float4, 32, 3, 1, 1 )
            ], depthEnabled: depthEnabled );

    private const string ThrusterShader = """
        cbuffer Scene : register(b0) { row_major float4x4 ViewProjection; float4 Time; };
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
        };
        Output VS(Input input, uint vertex : SV_VertexID) {
            const float2 corners[6] = {float2(-1,0),float2(1,0),float2(-1,1),float2(-1,1),float2(1,0),float2(1,1)};
            float engine = vertex < 6 ? -0.91 : 0.91;
            float2 uv = corners[vertex % 6];
            float flutter = sin(Time.x * 31.0 + input.motion.y * 31.7 + engine * 3.5);
            float plumeLength = (0.43 + 1.9 * input.motion.x) * (1 + 0.13 * flutter);
            float2 right = float2(-input.heading.y,input.heading.x);
            float2 world = input.center + right*(engine + uv.x*0.29) - input.heading*(1.12 + uv.y*plumeLength);
            Output output;
            output.position = mul(float4(world.x,0.026,world.y,1),ViewProjection);
            output.uv = uv;
            output.glow = float4(input.tint.z > input.tint.x ? float3(0.24,0.78,1) : float3(1,0.57,0.24),input.motion.x);
            return output;
        }
        float4 PS(Output input) : SV_Target {
            float edge = abs(input.uv.x) / max(0.08,1-input.uv.y);
            clip(min(1-edge,input.glow.a-0.025));
            float core = (1-edge) * (1-input.uv.y);
            float3 flame = lerp(input.glow.rgb*0.37,lerp(input.glow.rgb,float3(0.92,0.97,1),0.7),pow(core,0.45));
            return float4(flame*(0.65+0.35*input.glow.a),1);
        }
        """;

    internal static PipelineState CreateThrusters( GraphicsDevice device, RootSignature root, ShaderCompiler compiler ) =>
        device.CreateGraphicsPipeline( root, compiler.Compile( ThrusterShader, "VS", "vs_6_0" ),
            compiler.Compile( ThrusterShader, "PS", "ps_6_0" ), [
                new( "TEXCOORD", VertexFormat.Float2, 0, 0, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float2, 8, 1, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float4, 16, 2, 1, 1 ),
                new( "TEXCOORD", VertexFormat.Float4, 32, 3, 1, 1 )
            ], depthEnabled: true );

    internal static void RecordThrusters( GraphicsCommandList commands, PipelineState pipeline,
        GpuResource instances, uint count, Matrix4x4 viewProjection, float seconds ) {
        Span<uint> constants = stackalloc uint[20];
        constants.Clear( );
        MemoryMarshal.Write( MemoryMarshal.AsBytes( constants ), in viewProjection );
        constants[16] = BitConverter.SingleToUInt32Bits( seconds );
        commands.SetGraphicsPipeline( pipeline );
        commands.SetGraphicsConstants( constants );
        commands.SetVertexBuffer( 1, instances, InstanceStride );
        commands.Draw( 12, count );
    }

    internal static void RecordUploads( GraphicsDevice device, GraphicsCommandList commands, StaticModel model, Span<Mesh> meshes ) {
        for ( int i = 0; i < meshes.Length; ++i ) {
            var primitive = model.Primitives[i];
            if ( primitive.Vertices.Length <= ushort.MaxValue + 1 ) {
                var indices = new ushort[primitive.Indices.Length];
                for ( int j = 0; j < indices.Length; ++j ) indices[j] = (ushort)primitive.Indices[j];
                meshes[i] = device.CreateMesh( commands, primitive.Vertices, indices );
            } else {
                meshes[i] = device.CreateMesh( commands, primitive.Vertices, primitive.Indices );
            }
        }
    }

    internal static void Record( GraphicsCommandList commands, PipelineState pipeline, ReadOnlySpan<Mesh> meshes,
        GpuResource instances, uint count, Matrix4x4 viewProjection ) {
        commands.SetGraphicsPipeline( pipeline );
        ReadOnlySpan<Matrix4x4> constants = [viewProjection];
        commands.SetGraphicsConstants( MemoryMarshal.Cast<Matrix4x4, uint>( constants ) );
        commands.SetVertexBuffer( 1, instances, InstanceStride );
        foreach ( var mesh in meshes ) commands.DrawMesh( mesh, count );
    }
}
