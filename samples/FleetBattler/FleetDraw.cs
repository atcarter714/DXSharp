using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Assets;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal static class FleetDraw {
    internal const uint InstanceStride = 32;

    private const string Shader = """
        cbuffer Scene : register(b0) { row_major float4x4 ViewProjection; };
        struct Input {
            float3 position : POSITION;
            float3 normal : NORMAL;
            float3 color : COLOR;
            float2 center : TEXCOORD0;
            float2 heading : TEXCOORD1;
            float4 tint : TEXCOORD2;
        };
        struct Output { float4 position : SV_Position; float3 color : COLOR; };
        Output VS(Input input) {
            float2 right = float2(-input.heading.y, input.heading.x);
            float2 xz = input.center + right * input.position.x - input.heading * input.position.z;
            float2 normalXZ = right * input.normal.x - input.heading * input.normal.z;
            float3 normal = float3(normalXZ.x, input.normal.y, normalXZ.y);
            float light = 0.30 + 0.70 * saturate(dot(normal, normalize(float3(-0.45, 0.85, -0.35))));
            Output output;
            output.position = mul(float4(xz.x, input.position.y, xz.y, 1), ViewProjection);
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
                new( "TEXCOORD", VertexFormat.Float4, 16, 2, 1, 1 )
            ], depthEnabled: depthEnabled );

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
