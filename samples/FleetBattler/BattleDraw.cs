using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal static class BattleDraw {
    internal const uint EffectStride = 32;
    internal const uint ConstantCount = 20;
    internal const int RouteSegments = 16;

    private const string Effects = """
        cbuffer Scene : register(b0) { row_major float4x4 ViewProjection; float4 Viewport; };
        struct Input { float4 segment : POSITION; float4 style : COLOR; };
        struct Output {
            float4 position : SV_Position;
            float2 uv : TEXCOORD0;
            nointerpolation float4 style : COLOR;
        };
        Output VS(Input input, uint id : SV_VertexID) {
            const float2 corners[6] = {float2(0,-1),float2(1,-1),float2(0,1),float2(0,1),float2(1,-1),float2(1,1)};
            float2 uv = corners[id];
            float2 screenScale = float2(Viewport.x,Viewport.y)*0.5;
            float4 start = mul(float4(input.segment.x,0.36,input.segment.y,1),ViewProjection);
            float4 end = mul(float4(input.segment.z,0.36,input.segment.w,1),ViewProjection);
            float2 delta = (end.xy/end.w-start.xy/start.w)*screenScale;
            float2 right = float2(-delta.y,delta.x) / max(0.01,length(delta));
            float2 pos = lerp(start.xy/start.w,end.xy/end.w,uv.x) + right*uv.y*Viewport.z*2.4/screenScale;
            if (input.style.z == 1) {
                uv.x = uv.x*2-1;
                pos = start.xy/start.w + uv*(3+input.style.x*18)*Viewport.z/screenScale;
            }
            Output output;
            output.position = float4(pos,input.style.z == 2 ? 0.07 : 0.065,1);
            output.uv = uv;
            output.style = input.style;
            return output;
        }
        float4 PS(Output input) : SV_Target {
            if (input.style.z == 2) {
                clip(0.55-abs(input.uv.y));
                clip(frac(input.uv.x*1.3+input.style.x)-0.26);
                return float4(0.32,0.91,0.92,1);
            }
            float3 faction = input.style.y < 0.5 ? float3(0.21,0.86,1) : float3(1,0.42,0.12);
            if (input.style.z == 1) {
                float radius = length(input.uv);
                float ring = 1-smoothstep(0.12,0.25,abs(radius-0.72));
                float core = 1-smoothstep(0.04,0.43,radius);
                float flare = max(ring*0.87,core*(1-input.style.x));
                clip(flare*(1-input.style.x)-0.10);
                return float4(lerp(faction,float3(1,0.92,0.67),core)*flare*(1-input.style.x),1);
            }
            if (input.style.w > 0.5) {
                float streak = 1 - smoothstep(0.03,0.14,abs(input.uv.x - saturate(input.style.x * 1.6)));
                clip(streak * (1 - abs(input.uv.y)) - 0.12);
                return float4(lerp(faction,float3(1,0.87,0.38),0.8)*streak,1);
            }
            float head = saturate(input.style.x*2.6+0.18);
            float beam = 1 - smoothstep(0.03,0.32,abs(input.uv.x-head));
            clip(beam*(1-abs(input.uv.y))-0.09);
            return float4(lerp(faction,float3(0.96,0.99,1),beam*(1-abs(input.uv.y)))*(1-input.style.x*0.58),1);
        }
        """;

    private const string Markers = """
        cbuffer Scene : register(b0) { row_major float4x4 ViewProjection; float4 Viewport; };
        struct Input { float2 center : TEXCOORD0; float4 tint : TEXCOORD2; float4 motion : TEXCOORD3; };
        struct Output {
            float4 position : SV_Position;
            float2 uv : TEXCOORD;
            nointerpolation float4 marker : COLOR;
        };
        Output VS(Input input, uint id : SV_VertexID) {
            const float2 corners[6] = {float2(-1,-1),float2(1,-1),float2(-1,1),float2(-1,1),float2(1,-1),float2(1,1)};
            float2 uv = corners[id];
            float4 projected = mul(float4(input.center.x,0.1,input.center.y,1),ViewProjection);
            float size = clamp(1.86*Viewport.w, 6, 80);
            Output output;
            output.position = float4(projected.xy/projected.w + uv*(2*size/Viewport.xy),0.06,1);
            if (input.motion.w >= 0.98 && input.motion.w < 2)
                output.position = float4(2,2,0.06,1);
            output.uv = uv;
            output.marker = float4(input.motion.w >= 2 ? 1 : 0,input.motion.w >= 2 ? input.motion.w-2 : input.motion.w,
                                   input.tint.z > input.tint.x ? 0 : 1,size);
            return output;
        }
        float4 PS(Output input) : SV_Target {
            float2 uv = input.uv;
            float unit = Viewport.z / input.marker.w;
            float corner = step(2.5,Viewport.w)*step(0.67,abs(uv.x))*step(0.67,abs(uv.y))*
                max(step(1-1.3*unit,abs(uv.x)),step(1-1.3*unit,abs(uv.y))) * input.marker.x;
            float bar = step(0.68,uv.y)*step(uv.y,0.68+2.7*unit)*step(abs(uv.x),0.57) *
                max(input.marker.x,step(input.marker.y,0.98));
            clip(max(corner,bar)-0.5);
            float3 tint = input.marker.z < 0.5 ? float3(0.19,0.85,1) : float3(1,0.48,0.22);
            if (bar > 0.5 && (uv.x+0.57)/1.14 > input.marker.y) tint *= 0.18;
            return float4(tint,1);
        }
        """;

    internal static PipelineState CreateEffects( GraphicsDevice device, RootSignature root, ShaderCompiler compiler ) =>
        device.CreateGraphicsPipeline( root, compiler.Compile( Effects, "VS", "vs_6_0" ), compiler.Compile( Effects, "PS", "ps_6_0" ), [
            new( "POSITION", VertexFormat.Float4, 0, InstanceStepRate: 1 ),
            new( "COLOR", VertexFormat.Float4, 16, InstanceStepRate: 1 )
        ], depthEnabled: true );

    internal static PipelineState CreateMarkers( GraphicsDevice device, RootSignature root, ShaderCompiler compiler ) =>
        device.CreateGraphicsPipeline( root, compiler.Compile( Markers, "VS", "vs_6_0" ), compiler.Compile( Markers, "PS", "ps_6_0" ), [
            new( "TEXCOORD", VertexFormat.Float2, 0, 0, 1, 1 ),
            new( "TEXCOORD", VertexFormat.Float4, 16, 2, 1, 1 ),
            new( "TEXCOORD", VertexFormat.Float4, 32, 3, 1, 1 )
        ], depthEnabled: true );

    internal static int FillRoute( FleetSimulation fleet, FleetSelection unit, Span<BattleEffect> output ) {
        Span<Vector2> points = stackalloc Vector2[RouteSegments + 1];
        if ( !fleet.CopyRoute( unit, points, out _ ) ) return 0;
        for ( int i = 0; i < RouteSegments; ++i ) {
            var start = points[i];
            var end = points[i + 1];
            output[i] = new( new( start.X, start.Y, end.X, end.Y ), new( i * 0.3f, 0, 2, 0 ) );
        }
        return RouteSegments;
    }

    internal static void Record( GraphicsCommandList commands, PipelineState pipeline, GpuResource instances, uint count,
        Matrix4x4 viewProjection, int width, int height, float halfHeight, bool effects ) {
        if ( count == 0 ) return;
        Span<Vector4> constants = stackalloc Vector4[5];
        MemoryMarshal.Write( MemoryMarshal.AsBytes( constants[..4] ), in viewProjection );
        constants[4] = new( width, height, Math.Clamp( height / 740f, 1, 2.5f ), height / (2 * halfHeight) );
        commands.SetGraphicsPipeline( pipeline );
        commands.SetGraphicsConstants( MemoryMarshal.Cast<Vector4, uint>( constants ) );
        commands.SetVertexBuffer( effects ? 0u : 1u, instances, effects ? EffectStride : FleetDraw.InstanceStride );
        commands.Draw( 6, count );
    }
}
