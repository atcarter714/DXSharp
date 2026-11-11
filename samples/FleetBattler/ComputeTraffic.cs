using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal sealed class ComputeTraffic : IDisposable {
    internal const int MaxAgents = 65_536, MaxDebris = 16_384;
    private const uint GroupSize = 64;
    private readonly PipelineState? _orbits, _steering, _debrisPipeline, _freighters;
    private readonly GpuResource? _agents, _instances, _bodies;
    private readonly int _bodyCount;
    private int _lastSystem = -1;

    private const string DebrisShader = """
        cbuffer Scene : register(b0) { row_major float4x4 ViewProjection; float4 Viewport; };
        StructuredBuffer<float4> Bodies : register(t0);
        struct Output { float4 position : SV_Position; float2 uv : TEXCOORD; float shade : COLOR; };
        Output VS(uint id : SV_VertexID, uint instance : SV_InstanceID) {
            const float2 corners[6] = {float2(-1,-1),float2(1,-1),float2(-1,1),float2(-1,1),float2(1,-1),float2(1,1)};
            float4 rock = Bodies[instance+4];
            Output output;
            output.uv = corners[id];
            output.position = mul(float4(rock.x,-0.8,rock.y,1),ViewProjection);
            output.position.xy += output.uv * max(1.1,rock.z*Viewport.z) * (2 / Viewport.xy);
            output.shade = rock.w;
            return output;
        }
        float4 PS(Output input) : SV_Target {
            float a = atan2(input.uv.y,input.uv.x);
            float edge = 0.69 + 0.10*sin(a*3 + input.shade*13) + 0.08*cos(a*5 - input.shade*11);
            clip(edge-length(input.uv));
            float facet = 0.45 + 0.48 * saturate(0.55 - input.uv.x*0.57 + input.uv.y*0.31)
                        + 0.15 * sin(floor(a*2.2)*4.7 + input.shade*5);
            float3 tint = lerp(float3(0.18,0.20,0.26),float3(0.53,0.45,0.35),input.shade);
            return float4(tint * facet,1);
        }
        """;

    internal ComputeTraffic( GraphicsDevice device, ShaderCompiler compiler, int ships, int debris ) {
        if ( ships is < 0 or > MaxAgents ) throw new ArgumentOutOfRangeException( nameof(ships) );
        if ( debris is < 0 or > MaxDebris ) throw new ArgumentOutOfRangeException( nameof(debris) );
        ShipCount = (uint)ships;
        DebrisCount = (uint)debris;
        _bodyCount = CelestialSystem.BodyCount + debris;
        try {
            string compute = File.ReadAllText( Path.Combine( AppContext.BaseDirectory, "Shaders", "Traffic.hlsl" ) );
            using var computeRoot = device.CreateRootSignature( 3, 24 );
            _orbits = device.CreateComputePipeline( computeRoot, compiler.Compile( compute, "OrbitField", "cs_6_0" ) );
            _steering = device.CreateComputePipeline( computeRoot, compiler.Compile( compute, "Steering", "cs_6_0" ) );
            using var debrisRoot = device.CreateGraphicsBufferRootSignature( 20 );
            _debrisPipeline = device.CreateGraphicsPipeline( debrisRoot, compiler.Compile( DebrisShader, "VS", "vs_6_0" ),
                compiler.Compile( DebrisShader, "PS", "ps_6_0" ), PixelFormat.Rgba8Unorm, depthEnabled: true );
            string freighter = File.ReadAllText( Path.Combine( AppContext.BaseDirectory, "Shaders", "Freighter.hlsl" ) );
            using var freighterRoot = device.CreateGraphicsConstantsRootSignature( 16 );
            _freighters = device.CreateGraphicsPipeline( freighterRoot, compiler.Compile( freighter, "VS", "vs_6_0" ),
                compiler.Compile( freighter, "PS", "ps_6_0" ), [
                    new( "TEXCOORD", VertexFormat.Float2, 0, 0, 1, 1 ),
                    new( "TEXCOORD", VertexFormat.Float2, 8, 1, 1, 1 ),
                    new( "TEXCOORD", VertexFormat.Float4, 32, 3, 1, 1 )
                ], depthEnabled: true );
            _agents = device.CreateBuffer( (ulong)Math.Max( ships, 1 ) * 16, unorderedAccess: true );
            _instances = device.CreateBuffer( (ulong)Math.Max( ships, 1 ) * FleetDraw.InstanceStride, unorderedAccess: true );
            _bodies = device.CreateBuffer( (ulong)_bodyCount * 16, unorderedAccess: true );
        } catch {
            Dispose( );
            throw;
        }
    }

    internal uint ShipCount { get; }
    internal uint DebrisCount { get; }
    internal GpuResource Instances => _instances!;
    internal GpuResource Bodies => _bodies!;

    internal void RecordCompute( GraphicsCommandList commands, FleetSimulation fleet, float elapsed ) {
        Span<uint> constants = stackalloc uint[24];
        constants[0] = ShipCount;
        constants[1] = (uint)_bodyCount;
        constants[2] = _lastSystem == fleet.SystemIndex ? 0u : 1u;
        constants[3] = BitConverter.SingleToUInt32Bits( Math.Clamp( elapsed, 0, 0.05f ) );
        constants[4] = BitConverter.SingleToUInt32Bits( (float)(fleet.Tick * FleetSimulation.StepSeconds) );
        constants[5] = BitConverter.SingleToUInt32Bits( fleet.BoardRadius );
        constants[6] = (uint)fleet.SystemIndex;
        constants[7] = 0;
        var bodyConstants = MemoryMarshal.Cast<uint, Vector4>( constants[8..] );
        for ( int i = 0; i < fleet.Bodies.Length; ++i ) {
            var body = fleet.Bodies[i];
            bodyConstants[i] = new( body.Center.X, body.Center.Y, body.Radius, 0 );
        }

        commands.Transition( _agents!, ResourceState.Common, ResourceState.UnorderedAccess );
        commands.Transition( Instances, ResourceState.Common, ResourceState.UnorderedAccess );
        commands.Transition( Bodies, ResourceState.Common, ResourceState.UnorderedAccess );
        Bind( _orbits! );
        commands.SetComputeConstants( 3, constants );
        commands.Dispatch( ((uint)_bodyCount + GroupSize - 1) / GroupSize );
        if ( ShipCount > 0 ) {
            commands.UavBarrier( Bodies );
            Bind( _steering! );
            commands.SetComputeConstants( 3, constants );
            commands.Dispatch( (ShipCount + GroupSize - 1) / GroupSize );
        }
        commands.Transition( _agents!, ResourceState.UnorderedAccess, ResourceState.Common );
        commands.Transition( Instances, ResourceState.UnorderedAccess, ResourceState.VertexAndConstantBuffer );
        commands.Transition( Bodies, ResourceState.UnorderedAccess, ResourceState.NonPixelShaderResource );
        _lastSystem = fleet.SystemIndex;
        return;

        void Bind( PipelineState pipeline ) {
            commands.SetComputePipeline( pipeline );
            commands.SetComputeBuffer( 0, _agents! );
            commands.SetComputeBuffer( 1, Instances );
            commands.SetComputeBuffer( 2, Bodies );
        }
    }

    internal void RecordDebris( GraphicsCommandList commands, Matrix4x4 viewProjection, int width, int height, float halfHeight ) {
        if ( DebrisCount == 0 ) return;
        Span<Vector4> constants = stackalloc Vector4[5];
        MemoryMarshal.Write( MemoryMarshal.AsBytes( constants[..4] ), in viewProjection );
        constants[4] = new( width, height, height / (2 * halfHeight), 0 );
        commands.SetGraphicsPipeline( _debrisPipeline! );
        commands.SetGraphicsConstants( MemoryMarshal.Cast<Vector4, uint>( constants ) );
        commands.SetGraphicsShaderBuffer( Bodies );
        commands.Draw( 6, DebrisCount );
    }

    internal void RecordFreighters( GraphicsCommandList commands, Matrix4x4 viewProjection ) {
        if ( ShipCount == 0 ) return;
        ReadOnlySpan<Matrix4x4> constants = [viewProjection];
        commands.SetGraphicsPipeline( _freighters! );
        commands.SetGraphicsConstants( MemoryMarshal.Cast<Matrix4x4, uint>( constants ) );
        commands.SetVertexBuffer( 1, Instances, FleetDraw.InstanceStride );
        commands.Draw( 24, ShipCount );
    }

    internal void Finish( GraphicsCommandList commands ) {
        commands.Transition( Instances, ResourceState.VertexAndConstantBuffer, ResourceState.Common );
        commands.Transition( Bodies, ResourceState.NonPixelShaderResource, ResourceState.Common );
    }

    public void Dispose( ) {
        _bodies?.Dispose( );
        _instances?.Dispose( );
        _agents?.Dispose( );
        _debrisPipeline?.Dispose( );
        _freighters?.Dispose( );
        _steering?.Dispose( );
        _orbits?.Dispose( );
    }
}
