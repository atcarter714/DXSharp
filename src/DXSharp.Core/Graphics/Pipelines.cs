using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;
using DXSharp.Interop;
using DXSharp.Shaders;

namespace DXSharp.Graphics;

/// <summary>An owning root signature.</summary>
public sealed class RootSignature : GraphicsObject {
    internal readonly ComOwner<ID3D12RootSignature> Native;
    internal RootSignature( ComOwner<ID3D12RootSignature> owner ) : base( owner ) => Native = owner;
    internal override ComReference Retain( ) => Native.Retain( );
}

/// <summary>An owning pipeline with an independent root-signature lease.</summary>
public sealed class PipelineState : GraphicsObject {
    internal readonly ComOwner<ID3D12PipelineState> Native;
    internal readonly ComOwner<ID3D12RootSignature> RootNative;
    /// <summary>Whether the pipeline is a compute pipeline.</summary>
    public bool IsCompute { get; }
    internal PipelineState( ComOwner<ID3D12PipelineState> owner, ComOwner<ID3D12RootSignature> root, bool compute ) : base( owner ) {
        Native = owner; RootNative = root; IsCompute = compute;
    }
    internal override ComReference Retain( ) => Native.Retain( );
    /// <inheritdoc />
    public override void Dispose( ) { RootNative.Dispose( ); base.Dispose( ); }
}

public sealed unsafe partial class GraphicsDevice {
    [LibraryImport( "d3d12.dll", EntryPoint = "D3D12SerializeVersionedRootSignature" )]
    private static partial int SerializeRoot( in D3D12_VERSIONED_ROOT_SIGNATURE_DESC description, out nint blob, out nint errors );

    /// <summary>Creates an empty graphics signature, or a compute signature with root UAVs u0 through uN.</summary>
    public RootSignature CreateRootSignature( uint computeBufferCount = 0 ) => CreateRootSignature( computeBufferCount, 0 );

    /// <summary>Creates a compute signature with root UAVs followed by inline DWORD constants at b0.</summary>
    public RootSignature CreateRootSignature( uint computeBufferCount, uint computeConstantCount ) {
        if ( computeBufferCount > 32 ) throw new ArgumentOutOfRangeException( nameof(computeBufferCount) );
        if ( computeConstantCount > 64 - 2 * computeBufferCount ) throw new ArgumentOutOfRangeException( nameof(computeConstantCount) );
        uint count = computeBufferCount + (computeConstantCount == 0 ? 0u : 1u);
        Span<D3D12_ROOT_PARAMETER> parameters = stackalloc D3D12_ROOT_PARAMETER[(int)count];
        parameters.Clear( );
        for ( uint i = 0; i < computeBufferCount; ++i ) {
            parameters[(int)i].ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_UAV;
            parameters[(int)i].Anonymous.Descriptor.ShaderRegister = i;
        }
        if ( computeConstantCount != 0 ) parameters[^1] = InlineConstants( computeConstantCount );
        return CreateRootSignature( parameters, count == 0
            ? D3D12_ROOT_SIGNATURE_FLAGS.D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT : 0 );
    }

    /// <summary>Creates a graphics root signature with a pixel shader texture at t0 and a point-clamp sampler at s0.</summary>
    public RootSignature CreateTextureRootSignature( ) {
        D3D12_DESCRIPTOR_RANGE range = new( ) { RangeType = D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SRV, NumDescriptors = 1 };
        D3D12_ROOT_PARAMETER parameter = new( ) {
            ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_DESCRIPTOR_TABLE,
            ShaderVisibility = D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_PIXEL
        };
        parameter.Anonymous.DescriptorTable = new( ) { NumDescriptorRanges = 1, pDescriptorRanges = &range };
        var clamp = D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_CLAMP;
        D3D12_STATIC_SAMPLER_DESC sampler = new( ) {
            Filter = D3D12_FILTER.D3D12_FILTER_MIN_MAG_MIP_POINT,
            AddressU = clamp, AddressV = clamp, AddressW = clamp,
            MaxAnisotropy = 1, MaxLOD = float.MaxValue,
            ComparisonFunc = D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_ALWAYS,
            ShaderVisibility = D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_PIXEL
        };
        D3D12_VERSIONED_ROOT_SIGNATURE_DESC description = default;
        description.Version = D3D_ROOT_SIGNATURE_VERSION.D3D_ROOT_SIGNATURE_VERSION_1_0;
        description.Anonymous.Desc_1_0 = new( ) {
            NumParameters = 1, pParameters = &parameter,
            NumStaticSamplers = 1, pStaticSamplers = &sampler
        };
        return CreateRootSignature( description );
    }

    /// <summary>Creates a graphics root signature with up to 64 inline DWORDs at b0.</summary>
    public RootSignature CreateGraphicsConstantsRootSignature( uint valueCount ) {
        if ( valueCount is 0 or > 64 ) throw new ArgumentOutOfRangeException( nameof(valueCount) );
        return CreateRootSignature( [InlineConstants( valueCount )] );
    }

    /// <summary>Creates a graphics signature with constants at b0 and a vertex shader buffer at t0.</summary>
    public RootSignature CreateGraphicsBufferRootSignature( uint valueCount ) {
        if ( valueCount is 0 or > 62 ) throw new ArgumentOutOfRangeException( nameof(valueCount) );
        return CreateRootSignature( [InlineConstants( valueCount ), new( ) {
            ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_SRV,
            ShaderVisibility = D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_VERTEX
        }] );
    }

    private static D3D12_ROOT_PARAMETER InlineConstants( uint count ) {
        D3D12_ROOT_PARAMETER parameter = new( ) { ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_32BIT_CONSTANTS };
        parameter.Anonymous.Constants.Num32BitValues = count;
        return parameter;
    }

    private RootSignature CreateRootSignature( ReadOnlySpan<D3D12_ROOT_PARAMETER> parameters, D3D12_ROOT_SIGNATURE_FLAGS flags = 0 ) {
        fixed ( D3D12_ROOT_PARAMETER* pointer = parameters ) {
            D3D12_VERSIONED_ROOT_SIGNATURE_DESC description = default;
            description.Version = D3D_ROOT_SIGNATURE_VERSION.D3D_ROOT_SIGNATURE_VERSION_1_0;
            description.Anonymous.Desc_1_0 = new( ) { NumParameters = (uint)parameters.Length, pParameters = pointer, Flags = flags };
            return CreateRootSignature( description );
        }
    }

    private RootSignature CreateRootSignature( in D3D12_VERSIONED_ROOT_SIGNATURE_DESC description ) {
        int hr = SerializeRoot( description, out nint blobPointer, out nint errorPointer );
        try {
            Results.Check( hr, "SerializeVersionedRootSignature" );
            nint owned = blobPointer;
            blobPointer = 0;
            using var blob = ComOwner<ID3DBlob>.Adopt( owned );
            Guid id = typeof(ID3D12RootSignature).GUID;
            Results.Check( Native.Interface.CreateRootSignature( 0, blob.Interface.GetBufferPointer( ),
                blob.Interface.GetBufferSize( ), &id, out nint rootPointer ), "CreateRootSignature" );
            return new( ComOwner<ID3D12RootSignature>.Adopt( rootPointer ) ) { DeviceIdentity = DeviceIdentity };
        } finally {
            if ( blobPointer != 0 ) UnknownCalls.Release( blobPointer );
            if ( errorPointer != 0 ) UnknownCalls.Release( errorPointer );
        }
    }

    /// <summary>Creates a triangle-list graphics pipeline for SV_VertexID-generated vertices.</summary>
    public PipelineState CreateGraphicsPipeline( RootSignature root, ShaderCompilation vertex, ShaderCompilation pixel,
        PixelFormat targetFormat = PixelFormat.Rgba8Unorm ) =>
        CreateGraphicsPipeline( root, vertex, pixel, targetFormat, depthEnabled: false );

    /// <summary>Creates a triangle-list graphics pipeline with optional D32_FLOAT depth testing and writes.</summary>
    public PipelineState CreateGraphicsPipeline( RootSignature root, ShaderCompilation vertex, ShaderCompilation pixel,
        PixelFormat targetFormat, bool depthEnabled ) {
        RequireSameDevice( root );
        vertex.RequireSuccess( ); pixel.RequireSuccess( );
        if ( vertex.Bytecode.IsEmpty || pixel.Bytecode.IsEmpty ) throw new ArgumentException( "Both compiled shader objects are required." );
        using var lease = root.Native.Borrow( );
        fixed ( byte* vs = vertex.Bytecode.Span, ps = pixel.Bytecode.Span ) {
            D3D12_GRAPHICS_PIPELINE_STATE_DESC description = default;
            description.pRootSignature = (ID3D12RootSignature_unmanaged*)lease.Pointer;
            description.VS = new( ) { pShaderBytecode = vs, BytecodeLength = (nuint)vertex.Bytecode.Length };
            description.PS = new( ) { pShaderBytecode = ps, BytecodeLength = (nuint)pixel.Bytecode.Length };
            description.SampleMask = uint.MaxValue;
            description.RasterizerState.FillMode = D3D12_FILL_MODE.D3D12_FILL_MODE_SOLID;
            description.RasterizerState.CullMode = D3D12_CULL_MODE.D3D12_CULL_MODE_NONE;
            description.RasterizerState.DepthClipEnable = true;
            description.BlendState.RenderTarget.AsSpan( )[0] = new( ) {
                SrcBlend = D3D12_BLEND.D3D12_BLEND_ONE, DestBlend = D3D12_BLEND.D3D12_BLEND_ZERO, BlendOp = D3D12_BLEND_OP.D3D12_BLEND_OP_ADD,
                SrcBlendAlpha = D3D12_BLEND.D3D12_BLEND_ONE, DestBlendAlpha = D3D12_BLEND.D3D12_BLEND_ZERO, BlendOpAlpha = D3D12_BLEND_OP.D3D12_BLEND_OP_ADD,
                LogicOp = D3D12_LOGIC_OP.D3D12_LOGIC_OP_NOOP, RenderTargetWriteMask = 15
            };
            description.DepthStencilState.DepthFunc = D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_ALWAYS;
            if ( depthEnabled ) {
                description.DSVFormat = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT;
                description.DepthStencilState.DepthEnable = true;
                description.DepthStencilState.DepthWriteMask = D3D12_DEPTH_WRITE_MASK.D3D12_DEPTH_WRITE_MASK_ALL;
                description.DepthStencilState.DepthFunc = D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_LESS;
            }
            description.DepthStencilState.StencilReadMask = byte.MaxValue;
            description.DepthStencilState.StencilWriteMask = byte.MaxValue;
            description.DepthStencilState.FrontFace = new( ) {
                StencilFailOp = D3D12_STENCIL_OP.D3D12_STENCIL_OP_KEEP,
                StencilDepthFailOp = D3D12_STENCIL_OP.D3D12_STENCIL_OP_KEEP,
                StencilPassOp = D3D12_STENCIL_OP.D3D12_STENCIL_OP_KEEP,
                StencilFunc = D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_ALWAYS
            };
            description.DepthStencilState.BackFace = description.DepthStencilState.FrontFace;
            description.PrimitiveTopologyType = D3D12_PRIMITIVE_TOPOLOGY_TYPE.D3D12_PRIMITIVE_TOPOLOGY_TYPE_TRIANGLE;
            description.NumRenderTargets = 1; description.RTVFormats.AsSpan( )[0] = (DXGI_FORMAT)targetFormat; description.SampleDesc.Count = 1;
            Guid id = typeof(ID3D12PipelineState).GUID;
            Results.Check( Native.Interface.CreateGraphicsPipelineState( description, &id, out nint pointer ), "CreateGraphicsPipelineState" );
            return OwnPipeline( pointer, root, false );
        }
    }

    /// <summary>Creates a compute pipeline with an independently owned root signature.</summary>
    public PipelineState CreateComputePipeline( RootSignature root, ShaderCompilation shader ) {
        RequireSameDevice( root );
        shader.RequireSuccess( ); if ( shader.Bytecode.IsEmpty ) throw new ArgumentException( "A compiled shader object is required." );
        using var lease = root.Native.Borrow( );
        fixed ( byte* bytes = shader.Bytecode.Span ) {
            D3D12_COMPUTE_PIPELINE_STATE_DESC description = new( ) {
                pRootSignature = (ID3D12RootSignature_unmanaged*)lease.Pointer,
                CS = new( ) { pShaderBytecode = bytes, BytecodeLength = (nuint)shader.Bytecode.Length }
            };
            Guid id = typeof(ID3D12PipelineState).GUID;
            Results.Check( Native.Interface.CreateComputePipelineState( description, &id, out nint pointer ), "CreateComputePipelineState" );
            return OwnPipeline( pointer, root, true );
        }
    }
    private static PipelineState OwnPipeline( nint pointer, RootSignature root, bool compute ) {
        var owner = ComOwner<ID3D12PipelineState>.Adopt( pointer );
        try { return new( owner, root.Native.Clone( ), compute ) { DeviceIdentity = root.DeviceIdentity }; }
        catch { owner.Dispose( ); throw; }
    }
}
