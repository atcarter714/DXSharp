using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal sealed class ComputeTraffic : IDisposable {
    internal const int MaxAgents = 65_536, MaxDebris = 16_384;
    private const uint GroupSize = 64;
    private readonly PipelineState? _orbits, _steering, _debrisPipeline;
    private readonly GpuResource? _agents, _instances, _bodies;
    private readonly int _bodyCount;
    private int _lastSystem = -1;

    private const string Compute = """
        struct Agent { float2 center; float2 velocity; };
        struct Ship { float2 center; float2 heading; float4 tint; float4 motion; };
        RWStructuredBuffer<Agent> Agents : register(u0);
        RWStructuredBuffer<Ship> Ships : register(u1);
        RWStructuredBuffer<float4> Bodies : register(u2);
        cbuffer Scene : register(b0) {
            uint ShipCount; uint BodyCount; uint Reset; float Delta;
            float Time; float Radius; uint SectorIndex; float Pad;
            float4 MajorBodies[4];
        };

        uint Mix(uint x) {
            x ^= x >> 16;
            x *= 0x7feb352d;
            x ^= x >> 15;
            x *= 0x846ca68b;
            return x ^ (x >> 16);
        }
        float Unit(uint x) { return float(Mix(x) >> 8) * (1.0 / 16777216.0); }

        [numthreads(64,1,1)]
        void OrbitField(uint3 threadId : SV_DispatchThreadID) {
            uint id = threadId.x;
            if (id >= BodyCount) return;
            if (id < 4) { Bodies[id] = MajorBodies[id]; return; }
            uint seed = id + SectorIndex*16381;
            float a = Unit(seed*13) * 6.2831853 + Time*(0.027 + Unit(seed*17)*0.12);
            float lane = Radius*(0.56 + 0.27*Unit(seed*7));
            float2 center = MajorBodies[0].xy + float2(cos(a)*lane, sin(a)*lane*0.79);
            Bodies[id] = float4(center, 0.09 + pow(Unit(seed*23),3)*0.52, Unit(seed*31));
        }

        [numthreads(64,1,1)]
        void Steering(uint3 threadId : SV_DispatchThreadID) {
            uint id = threadId.x;
            if (id >= ShipCount) return;
            uint seed = id + SectorIndex*21611;
            uint bodyIndex = 1 + (Mix(seed + 41) % (BodyCount-1));
            float4 body = Bodies[bodyIndex];
            float phase = Unit(seed*5+7)*6.2831853 + Time*(0.28+Unit(seed*13+1)*0.21);
            float orbitRadius = body.z*1.68 + 2.8 + Unit(seed*17+9)*6.8;
            float2 target = body.xy + float2(cos(phase),sin(phase))*orbitRadius;
            Agent agent;
            if (Reset != 0) {
                agent.center = target + float2(Unit(seed*19+3)-0.5,Unit(seed*23+3)-0.5)*2.4;
                agent.velocity = float2(-sin(phase),cos(phase))*orbitRadius*0.32;
            } else agent = Agents[id];
            float2 desired = (target-agent.center)*2.3;
            desired *= min(1,9 / max(length(desired),0.01));
            agent.velocity = lerp(agent.velocity,desired,saturate(Delta*3.2));
            agent.center += agent.velocity*Delta;
            Agents[id] = agent;
            Ship ship;
            ship.center = agent.center;
            ship.heading = length(agent.velocity) > 0.001 ? normalize(agent.velocity) : float2(-sin(phase),cos(phase));
            ship.tint = float4(0.58,0.66,0.76,0.68);
            ship.motion = float4(0,Unit(seed),0.28,0);
            Ships[id] = ship;
        }
        """;

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
            float radius = abs(input.uv.x)*0.89 + abs(input.uv.y)*0.74;
            clip(0.94-radius);
            float3 tint = lerp(float3(0.14,0.18,0.25),float3(0.53,0.45,0.35),input.shade);
            return float4(tint * (0.6+0.4*saturate(1-input.uv.x)),1);
        }
        """;

    internal ComputeTraffic( GraphicsDevice device, ShaderCompiler compiler, int ships, int debris ) {
        if ( ships is < 0 or > MaxAgents ) throw new ArgumentOutOfRangeException( nameof(ships) );
        if ( debris is < 0 or > MaxDebris ) throw new ArgumentOutOfRangeException( nameof(debris) );
        ShipCount = (uint)ships;
        DebrisCount = (uint)debris;
        _bodyCount = CelestialSystem.BodyCount + debris;
        try {
            using var computeRoot = device.CreateRootSignature( 3, 24 );
            _orbits = device.CreateComputePipeline( computeRoot, compiler.Compile( Compute, "OrbitField", "cs_6_0" ) );
            _steering = device.CreateComputePipeline( computeRoot, compiler.Compile( Compute, "Steering", "cs_6_0" ) );
            using var debrisRoot = device.CreateGraphicsBufferRootSignature( 20 );
            _debrisPipeline = device.CreateGraphicsPipeline( debrisRoot, compiler.Compile( DebrisShader, "VS", "vs_6_0" ),
                compiler.Compile( DebrisShader, "PS", "ps_6_0" ), PixelFormat.Rgba8Unorm, depthEnabled: true );
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

    internal void Finish( GraphicsCommandList commands ) {
        commands.Transition( Instances, ResourceState.VertexAndConstantBuffer, ResourceState.Common );
        commands.Transition( Bodies, ResourceState.NonPixelShaderResource, ResourceState.Common );
    }

    public void Dispose( ) {
        _bodies?.Dispose( );
        _instances?.Dispose( );
        _agents?.Dispose( );
        _debrisPipeline?.Dispose( );
        _steering?.Dispose( );
        _orbits?.Dispose( );
    }
}
