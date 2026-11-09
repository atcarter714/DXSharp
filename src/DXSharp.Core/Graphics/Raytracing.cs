using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Interop;
using DXSharp.Shaders;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;

namespace DXSharp.Graphics;

/// <summary>An opaque, non-indexed triangle mesh with one identity-transform instance.</summary>
public sealed class RaytracingScene : IDisposable {
    internal readonly GpuResource Bottom;
    internal readonly GpuResource Top;
    internal RaytracingScene( GpuResource bottom, GpuResource top ) { Bottom = bottom; Top = top; }

    /// <summary>Releases the public owners of the acceleration structures.</summary>
    public void Dispose( ) { Top.Dispose( ); Bottom.Dispose( ); }
}

/// <summary>A one-ray-type DXR pipeline with an independently owned shader table.</summary>
public sealed class RaytracingPipeline : IDisposable {
    internal const uint TableAlignment = 64;
    internal const uint IdentifierSize = 32;
    internal readonly ComOwner<ID3D12StateObject> State;
    internal readonly RootSignature Root;
    internal readonly GpuResource Table;

    internal RaytracingPipeline( ComOwner<ID3D12StateObject> state, RootSignature root, GpuResource table ) {
        State = state;
        Root = root;
        Table = table;
    }

    /// <summary>Releases the public owners of the shader table, state object and signature.</summary>
    public void Dispose( ) { Table.Dispose( ); State.Dispose( ); Root.Dispose( ); }
}

public sealed unsafe partial class GraphicsDevice {
    /// <summary>Queries whether this device supports a DXR pipeline.</summary>
    public bool SupportsRaytracing {
        get {
            D3D12_FEATURE_DATA_D3D12_OPTIONS5 options = default;
            int hr = Native.Interface.CheckFeatureSupport( D3D12_FEATURE.D3D12_FEATURE_D3D12_OPTIONS5, &options, (uint)sizeof(D3D12_FEATURE_DATA_D3D12_OPTIONS5) );
            if ( hr == unchecked((int)0x80070057) ) return false;
            Results.Check( hr, "CheckFeatureSupport(D3D12_OPTIONS5)" );
            return options.RaytracingTier != D3D12_RAYTRACING_TIER.D3D12_RAYTRACING_TIER_NOT_SUPPORTED;
        }
    }

    /// <summary>Builds BLAS and TLAS for one opaque mesh and waits for the supplied queue to finish the build.</summary>
    public RaytracingScene CreateRaytracingScene( CommandQueue queue, ReadOnlySpan<Vector3> vertices ) {
        RequireSameDevice( queue );
        if ( queue.Kind == QueueKind.Copy ) throw new ArgumentException( "A direct or compute queue is required.", nameof(queue) );
        if ( vertices.IsEmpty || vertices.Length % 3 != 0 ) throw new ArgumentException( "Provide complete triangles.", nameof(vertices) );

        using var dxr = RequireRaytracingDevice( );
        using var positions = CreateBuffer( checked((ulong)vertices.Length * (uint)sizeof(Vector3)), BufferMemory.Upload );
        positions.Write( vertices );
        D3D12_RAYTRACING_GEOMETRY_DESC geometry = new( ) {
            Type = D3D12_RAYTRACING_GEOMETRY_TYPE.D3D12_RAYTRACING_GEOMETRY_TYPE_TRIANGLES,
            Flags = D3D12_RAYTRACING_GEOMETRY_FLAGS.D3D12_RAYTRACING_GEOMETRY_FLAG_OPAQUE
        };
        geometry.Anonymous.Triangles = new( ) {
            VertexFormat = DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT,
            VertexCount = (uint)vertices.Length,
            VertexBuffer = new( ) { StartAddress = positions.GpuAddress, StrideInBytes = (uint)sizeof(Vector3) }
        };
        D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_INPUTS bottomInputs = new( ) {
            Type = D3D12_RAYTRACING_ACCELERATION_STRUCTURE_TYPE.D3D12_RAYTRACING_ACCELERATION_STRUCTURE_TYPE_BOTTOM_LEVEL,
            DescsLayout = D3D12_ELEMENTS_LAYOUT.D3D12_ELEMENTS_LAYOUT_ARRAY,
            NumDescs = 1
        };
        bottomInputs.Anonymous.pGeometryDescs = &geometry;
        var bottomInfo = Prebuild( dxr.Interface, bottomInputs );
        var bottom = CreateBuffer( bottomInfo.ResultDataMaxSizeInBytes, initialState: ResourceState.RaytracingAccelerationStructure, unorderedAccess: true );
        try {
            D3D12_RAYTRACING_INSTANCE_DESC instance = default;
            var transform = new Span<float>( &instance.Transform, 12 );
            transform[0] = transform[5] = transform[10] = 1;
            instance.InstanceMask = byte.MaxValue;
            instance.AccelerationStructure = bottom.GpuAddress;
            using var instances = CreateBuffer( (uint)sizeof(D3D12_RAYTRACING_INSTANCE_DESC), BufferMemory.Upload );
            instances.Write<D3D12_RAYTRACING_INSTANCE_DESC>( [instance] );
            D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_INPUTS topInputs = new( ) {
                Type = D3D12_RAYTRACING_ACCELERATION_STRUCTURE_TYPE.D3D12_RAYTRACING_ACCELERATION_STRUCTURE_TYPE_TOP_LEVEL,
                DescsLayout = D3D12_ELEMENTS_LAYOUT.D3D12_ELEMENTS_LAYOUT_ARRAY,
                NumDescs = 1
            };
            topInputs.Anonymous.InstanceDescs = instances.GpuAddress;
            var topInfo = Prebuild( dxr.Interface, topInputs );
            var top = CreateBuffer( topInfo.ResultDataMaxSizeInBytes, initialState: ResourceState.RaytracingAccelerationStructure, unorderedAccess: true );
            try {
                using var scratch = CreateBuffer( Math.Max( bottomInfo.ScratchDataSizeInBytes, topInfo.ScratchDataSizeInBytes ), unorderedAccess: true );
                using var allocator = CreateCommandAllocator( queue.Kind );
                using var commands = CreateCommandList( allocator );
                commands.Transition( scratch, ResourceState.Common, ResourceState.UnorderedAccess );
                commands.BuildAccelerationStructure( bottomInputs, bottom, scratch, positions );
                commands.UavBarrier( bottom );
                commands.UavBarrier( scratch );
                commands.BuildAccelerationStructure( topInputs, top, scratch, instances );
                commands.UavBarrier( top );
                commands.Close( );
                queue.Wait( queue.Submit( commands ) );
                return new( bottom, top );
            } catch { top.Dispose( ); throw; }
        } catch { bottom.Dispose( ); throw; }
    }

    /// <summary>Creates a one-ray-type pipeline with a global UAV at u0, TLAS at t0 and no local shader arguments.</summary>
    public RaytracingPipeline CreateRaytracingPipeline( ShaderCompilation library, string rayGeneration, string miss,
        string closestHit, uint payloadSizeInBytes ) {
        ArgumentNullException.ThrowIfNull( library );
        library.RequireSuccess( );
        if ( library.Bytecode.IsEmpty ) throw new ArgumentException( "A compiled DXIL library is required.", nameof(library) );
        ArgumentException.ThrowIfNullOrWhiteSpace( rayGeneration );
        ArgumentException.ThrowIfNullOrWhiteSpace( miss );
        ArgumentException.ThrowIfNullOrWhiteSpace( closestHit );
        if ( payloadSizeInBytes == 0 || payloadSizeInBytes % 4 != 0 ) throw new ArgumentOutOfRangeException( nameof(payloadSizeInBytes) );
        const string hitGroup = "DXSharpTriangleHitGroup";

        using var dxr = RequireRaytracingDevice( );
        var root = CreateRootSignature( [
            new( ) { ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_UAV },
            new( ) { ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_SRV }
        ] );
        try {
            using var rootLease = root.Native.Borrow( );
            fixed ( byte* bytes = library.Bytecode.Span )
            fixed ( char* hit = closestHit, group = hitGroup ) {
                D3D12_DXIL_LIBRARY_DESC dxil = new( ) {
                    DXILLibrary = new( ) { pShaderBytecode = bytes, BytecodeLength = (nuint)library.Bytecode.Length }
                };
                D3D12_HIT_GROUP_DESC hitDesc = new( ) {
                    HitGroupExport = new PCWSTR( group ), ClosestHitShaderImport = new PCWSTR( hit ),
                    Type = D3D12_HIT_GROUP_TYPE.D3D12_HIT_GROUP_TYPE_TRIANGLES
                };
                D3D12_RAYTRACING_SHADER_CONFIG shaderConfig = new( ) { MaxPayloadSizeInBytes = payloadSizeInBytes, MaxAttributeSizeInBytes = 8 };
                D3D12_RAYTRACING_PIPELINE_CONFIG pipelineConfig = new( ) { MaxTraceRecursionDepth = 1 };
                D3D12_GLOBAL_ROOT_SIGNATURE global = new( ) { pGlobalRootSignature = (ID3D12RootSignature_unmanaged*)rootLease.Pointer };
                D3D12_STATE_SUBOBJECT* parts = stackalloc D3D12_STATE_SUBOBJECT[5] {
                    new( ) { Type = D3D12_STATE_SUBOBJECT_TYPE.D3D12_STATE_SUBOBJECT_TYPE_DXIL_LIBRARY, pDesc = &dxil },
                    new( ) { Type = D3D12_STATE_SUBOBJECT_TYPE.D3D12_STATE_SUBOBJECT_TYPE_HIT_GROUP, pDesc = &hitDesc },
                    new( ) { Type = D3D12_STATE_SUBOBJECT_TYPE.D3D12_STATE_SUBOBJECT_TYPE_RAYTRACING_SHADER_CONFIG, pDesc = &shaderConfig },
                    new( ) { Type = D3D12_STATE_SUBOBJECT_TYPE.D3D12_STATE_SUBOBJECT_TYPE_RAYTRACING_PIPELINE_CONFIG, pDesc = &pipelineConfig },
                    new( ) { Type = D3D12_STATE_SUBOBJECT_TYPE.D3D12_STATE_SUBOBJECT_TYPE_GLOBAL_ROOT_SIGNATURE, pDesc = &global }
                };
                D3D12_STATE_OBJECT_DESC description = new( ) {
                    Type = D3D12_STATE_OBJECT_TYPE.D3D12_STATE_OBJECT_TYPE_RAYTRACING_PIPELINE, NumSubobjects = 5, pSubobjects = parts
                };
                Guid id = typeof(ID3D12StateObject).GUID;
                Results.Check( dxr.Interface.CreateStateObject( &description, &id, out nint pointer ), "CreateStateObject" );
                var state = ComOwner<ID3D12StateObject>.Adopt( pointer );
                try { return new( state, root, CreateShaderTable( state, rayGeneration, miss, hitGroup ) ); }
                catch { state.Dispose( ); throw; }
            }
        } catch { root.Dispose( ); throw; }
    }

    private GpuResource CreateShaderTable( ComOwner<ID3D12StateObject> state, string rayGeneration, string miss, string hitGroup ) {
        if ( !state.TryQuery<ID3D12StateObjectProperties>( typeof(ID3D12StateObjectProperties).GUID, out var properties ) )
            throw new InvalidOperationException( "The state object did not expose shader identifiers." );
        using var owner = properties!;
        const int stride = (int)RaytracingPipeline.TableAlignment;
        Span<byte> bytes = stackalloc byte[3 * stride];
        bytes.Clear( );
        CopyIdentifier( owner.Interface, rayGeneration, bytes );
        CopyIdentifier( owner.Interface, miss, bytes[stride..] );
        CopyIdentifier( owner.Interface, hitGroup, bytes[(2 * stride)..] );
        var table = CreateBuffer( (uint)bytes.Length, BufferMemory.Upload );
        try { table.Write<byte>( bytes ); return table; }
        catch { table.Dispose( ); throw; }
    }

    private static void CopyIdentifier( ID3D12StateObjectProperties properties, string name, Span<byte> target ) {
        fixed ( char* chars = name ) {
            void* id = properties.GetShaderIdentifier( new PCWSTR( chars ) );
            if ( id is null ) throw new ArgumentException( $"Missing DXR export '{name}'.", nameof(name) );
            new ReadOnlySpan<byte>( id, (int)RaytracingPipeline.IdentifierSize ).CopyTo( target );
        }
    }

    private ComOwner<ID3D12Device5> RequireRaytracingDevice( ) {
        if ( !SupportsRaytracing || !Native.TryQuery<ID3D12Device5>( typeof(ID3D12Device5).GUID, out var device ) )
            throw new NotSupportedException( "The selected device does not support DXR pipelines." );
        return device!;
    }

    private static D3D12_RAYTRACING_ACCELERATION_STRUCTURE_PREBUILD_INFO Prebuild( ID3D12Device5 device,
        D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_INPUTS inputs ) {
        D3D12_RAYTRACING_ACCELERATION_STRUCTURE_PREBUILD_INFO info = default;
        device.GetRaytracingAccelerationStructurePrebuildInfo( &inputs, &info );
        if ( info.ResultDataMaxSizeInBytes == 0 || info.ScratchDataSizeInBytes == 0 )
            throw new InvalidOperationException( "DXR returned invalid acceleration structure sizes." );
        return info;
    }
}

public sealed unsafe partial class GraphicsCommandList {
    /// <summary>Dispatches a one-ray-type pipeline into a uint output UAV. The caller transitions the output before use.</summary>
    public void DispatchRays( RaytracingPipeline pipeline, RaytracingScene scene, GpuResource output, uint width, uint height ) {
        ArgumentNullException.ThrowIfNull( pipeline );
        ArgumentNullException.ThrowIfNull( scene );
        ArgumentNullException.ThrowIfNull( output );
        Recording( );
        ulong pixels = (ulong)width * height;
        if ( pixels is 0 or > 1_073_741_824 ) throw new ArgumentOutOfRangeException( nameof(width) );
        if ( !output.SupportsUnorderedAccess ) throw new ArgumentException( "The output must support UAV access.", nameof(output) );
        output.ValidateRange( 0, pixels * sizeof(uint) );
        Track( pipeline.Root ); Track( pipeline.Table ); Track( scene.Top ); Track( scene.Bottom ); Track( output );
        RetainNative( pipeline.State, pipeline.State.Retain( ) );
        using var commands = RequireRaytracingList( );
        var list = commands.Interface;
        list.SetPipelineState1( pipeline.State.Interface );
        list.SetComputeRootSignature( pipeline.Root.Native.Interface );
        list.SetComputeRootUnorderedAccessView( 0, output.GpuAddress );
        list.SetComputeRootShaderResourceView( 1, scene.Top.GpuAddress );
        ulong address = pipeline.Table.GpuAddress;
        const uint stride = RaytracingPipeline.TableAlignment;
        D3D12_DISPATCH_RAYS_DESC dispatch = new( ) {
            RayGenerationShaderRecord = new( ) { StartAddress = address, SizeInBytes = RaytracingPipeline.IdentifierSize },
            MissShaderTable = new( ) { StartAddress = address + stride, SizeInBytes = stride, StrideInBytes = stride },
            HitGroupTable = new( ) { StartAddress = address + 2 * stride, SizeInBytes = stride, StrideInBytes = stride },
            Width = width, Height = height, Depth = 1
        };
        list.DispatchRays( &dispatch );
    }

    internal void BuildAccelerationStructure( D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_INPUTS inputs,
        GpuResource destination, GpuResource scratch, GpuResource source ) {
        Recording( ); Track( destination ); Track( scratch ); Track( source );
        using var commands = RequireRaytracingList( );
        D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_DESC build = new( ) {
            DestAccelerationStructureData = destination.GpuAddress, Inputs = inputs, ScratchAccelerationStructureData = scratch.GpuAddress
        };
        commands.Interface.BuildRaytracingAccelerationStructure( &build, 0, null );
    }

    private ComOwner<ID3D12GraphicsCommandList4> RequireRaytracingList( ) {
        if ( Kind == QueueKind.Copy || !Native.TryQuery<ID3D12GraphicsCommandList4>( typeof(ID3D12GraphicsCommandList4).GUID, out var list ) )
            throw new NotSupportedException( "A DXR-capable direct or compute command list is required." );
        return list!;
    }
}
