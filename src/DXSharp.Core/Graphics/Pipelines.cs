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
    public RootSignature CreateRootSignature( uint computeBufferCount = 0 ) {
        if ( computeBufferCount > 32 ) throw new ArgumentOutOfRangeException( nameof(computeBufferCount) );
        Span<D3D12_ROOT_PARAMETER> parameters = stackalloc D3D12_ROOT_PARAMETER[checked((int)computeBufferCount)];
        parameters.Clear( );
        for ( uint i = 0; i < computeBufferCount; ++i ) {
            parameters[(int)i].ParameterType = D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_UAV;
            parameters[(int)i].Anonymous.Descriptor.ShaderRegister = i;
        }
        fixed ( D3D12_ROOT_PARAMETER* pointer = parameters ) {
            D3D12_VERSIONED_ROOT_SIGNATURE_DESC description = default;
            description.Version = D3D_ROOT_SIGNATURE_VERSION.D3D_ROOT_SIGNATURE_VERSION_1_0;
            description.Anonymous.Desc_1_0 = new( ) {
                NumParameters = computeBufferCount, pParameters = pointer,
                Flags = computeBufferCount == 0 ? D3D12_ROOT_SIGNATURE_FLAGS.D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT : 0
            };
            int hr = SerializeRoot( description, out nint blobPointer, out nint errorPointer );
            try {
                Results.Check( hr, "SerializeVersionedRootSignature" );
                nint owned = blobPointer; blobPointer = 0;
                using var blob = ComOwner<ID3DBlob>.Adopt( owned );
                Guid id = typeof(ID3D12RootSignature).GUID;
                Results.Check( Native.Interface.CreateRootSignature( 0, blob.Interface.GetBufferPointer( ), blob.Interface.GetBufferSize( ), &id, out nint rootPointer ), "CreateRootSignature" );
                return new( ComOwner<ID3D12RootSignature>.Adopt( rootPointer ) ) { DeviceIdentity = DeviceIdentity };
            } finally {
                if ( blobPointer != 0 ) UnknownCalls.Release( blobPointer );
                if ( errorPointer != 0 ) UnknownCalls.Release( errorPointer );
            }
        }
    }

    /// <summary>Creates a triangle-list graphics pipeline for SV_VertexID-generated vertices.</summary>
    public PipelineState CreateGraphicsPipeline( RootSignature root, ShaderCompilation vertex, ShaderCompilation pixel,
        PixelFormat targetFormat = PixelFormat.Rgba8Unorm ) {
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
