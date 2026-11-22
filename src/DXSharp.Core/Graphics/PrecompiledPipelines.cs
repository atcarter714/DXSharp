using DXSharp.Shaders;

namespace DXSharp.Graphics;

public sealed unsafe partial class GraphicsDevice {
    /// <summary>Creates a graphics pipeline directly from precompiled shader containers.</summary>
    public PipelineState CreateGraphicsPipeline( RootSignature root, ShaderBytecode vertex, ShaderBytecode pixel,
        PixelFormat targetFormat = PixelFormat.Rgba8Unorm, bool depthEnabled = false ) =>
        CreateGraphicsPipeline( root, vertex, pixel, ReadOnlySpan<VertexElement>.Empty, targetFormat, depthEnabled );
    /// <summary>Creates an indexed/instanced graphics pipeline from precompiled shader containers.</summary>
    public PipelineState CreateGraphicsPipeline( RootSignature root, ShaderBytecode vertex, ShaderBytecode pixel,
        ReadOnlySpan<VertexElement> inputLayout, PixelFormat targetFormat = PixelFormat.Rgba8Unorm, bool depthEnabled = false ) {
        ArgumentNullException.ThrowIfNull( vertex ); ArgumentNullException.ThrowIfNull( pixel );
        return CreateGraphicsPipeline( root, vertex.AsCompilation( ), pixel.AsCompilation( ), inputLayout, targetFormat, depthEnabled );
    }
    /// <summary>Creates a compute pipeline from precompiled bytecode without invoking DXC.</summary>
    public PipelineState CreateComputePipeline( RootSignature root, ShaderBytecode shader ) {
        ArgumentNullException.ThrowIfNull( shader ); return CreateComputePipeline( root, shader.AsCompilation( ) );
    }
}
