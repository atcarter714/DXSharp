#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi.Common ;
using DXSharp.DXGI ;

#endregion
namespace DXSharp.Direct3D12 ;


[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_BOX))]
public struct Box {
	/// <summary>The x position of the left hand side of the box.</summary>
	public uint left ;

	/// <summary>The y position of the top of the box.</summary>
	public uint top ;

	/// <summary>The z position of the front of the box.</summary>
	public uint front ;

	/// <summary>The x position of the right hand side of the box, plus 1. This means that <c>right - left</c> equals the width of the box.</summary>
	public uint right ;

	/// <summary>The y position of the bottom of the box, plus 1. This means that <c>bottom - top</c> equals the height of the box.</summary>
	public uint bottom ;

	/// <summary>The z position of the back of the box, plus 1. This means that <c>back - front</c> equals the depth of the box.</summary>
	public uint back ;
} ;


[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_COMMAND_QUEUE_DESC))]
public struct CommandQueueDescription {
	public CommandListType Type ;
	public int Priority ;
	public CommandQueueFlags Flags ;
	public uint NodeMask ;
}

[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_TILED_RESOURCE_COORDINATE))]
public struct TiledResourceCoordinate {
	/// <summary>The x-coordinate of the tiled resource.</summary>
	public uint X;
	/// <summary>The y-coordinate of the tiled resource.</summary>
	public uint Y;
	/// <summary>The z-coordinate of the tiled resource.</summary>
	public uint Z;
	
	/// <summary>
	/// <para>The index of the subresource for the tiled resource. For mipmaps that use nonstandard tiling, or are packed, or both use nonstandard tiling and are packed, any subresource value that indicates any of the packed mipmaps all refer to the same tile. Additionally, the X coordinate is used to indicate a tile within the packed mip region, rather than a logical region of a single subresource. The Y and Z coordinates must be zero.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tiled_resource_coordinate#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint Subresource ;
}

[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_TILE_REGION_SIZE))]
public struct TileRegionSize {
	/// <summary>The number of tiles in the tiled region.</summary>
	public uint NumTiles ;
	
	//! TODO: Come up with better way to handle bool vs BOOL size difference
	//! For now, we'll just use BOOL until we implement our own "BOOL" type
	/// <summary>
	/// <para>Specifies whether the runtime uses the <b>Width</b>, <b>Height</b>, and <b>Depth</b> members to define the region.</para>
	/// <para>If <b>TRUE</b>, the runtime uses the <b>Width</b>, <b>Height</b>, and <b>Depth</b> members to define the region. In this case,  <b>NumTiles</b> should be equal to <b>Width</b> *  <b>Height</b> * <b>Depth</b>. If <b>FALSE</b>, the runtime ignores the <b>Width</b>, <b>Height</b>, and <b>Depth</b> members and uses the <b>NumTiles</b> member to traverse tiles in the resource linearly across x, then y, then z (as applicable) and then spills over mipmaps/arrays in subresource order.  For example, use this technique to map an entire resource at once.</para>
	/// <para>Regardless of whether you specify <b>TRUE</b> or <b>FALSE</b> for <b>UseBox</b>, you use a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tiled_resource_coordinate">D3D12_TILED_RESOURCE_COORDINATE</a> structure to specify the starting location for the region within the resource as a separate parameter outside of this structure by using x, y, and z coordinates.</para>
	/// <para>When the region includes mipmaps that are packed with nonstandard tiling, <b>UseBox</b> must be <b>FALSE</b> because tile dimensions are not standard and the app only knows a count of how many tiles are consumed by the packed area, which is per array slice.  The corresponding (separate) starting location parameter uses x to offset into the flat range of tiles in this case, and y and z coordinates must each be 0.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tile_region_size#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL UseBox ;
	/// <summary>The width of the tiled region, in tiles. Used for buffer and 1D, 2D, and 3D textures.</summary>
	public uint Width ;
	/// <summary>The height of the tiled region, in tiles. Used for 2D and 3D textures.</summary>
	public ushort Height ;
	/// <summary>The depth of the tiled region, in tiles. Used for 3D textures or arrays. For arrays, used for advancing in depth jumps to next slice of same mipmap size, which isn't contiguous in the subresource counting space if there are multiple mipmaps.</summary>
	public ushort Depth ;
}

[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_HEAP_DESC))]
public struct HeapDescription {
	/// <summary>
	/// <para>The size, in bytes, of the heap. To avoid wasting memory, applications should pass <i>SizeInBytes</i> values which are multiples of the effective <i>Alignment</i>; but non-aligned <i>SizeInBytes</i> is also supported, for convenience. To find out how large a heap must be to support textures with undefined layouts and adapter-specific sizes, call <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo">ID3D12Device::GetResourceAllocationInfo</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ulong SizeInBytes ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_heap_properties">D3D12_HEAP_PROPERTIES</a> structure that describes the heap properties.</summary>
	public HeapProperties Properties;

	/// <summary>
	/// <para>The alignment value for the heap.  Valid values:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ulong Alignment ;

	/// <summary>
	/// <para>A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_heap_flags">D3D12_HEAP_FLAGS</a>-typed values that are combined by using a bitwise-OR operation. The resulting value identifies heap options. When creating heaps to support adapters with resource heap tier 1, an application must choose some flags.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public D3D12_HEAP_FLAGS Flags ;
}

[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_HEAP_PROPERTIES))]
public struct HeapProperties {
	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type">D3D12_HEAP_TYPE</a>-typed value that specifies the type of heap.</summary>
	public D3D12_HEAP_TYPE Type ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_cpu_page_property">D3D12_CPU_PAGE_PROPERTY</a>-typed value that specifies the CPU-page properties for the heap.</summary>
	public D3D12_CPU_PAGE_PROPERTY CPUPageProperty ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_memory_pool">D3D12_MEMORY_POOL</a>-typed value that specifies the memory pool for the heap.</summary>
	public D3D12_MEMORY_POOL MemoryPoolPreference ;

	/// <summary>
	/// <para>For multi-adapter operation, this indicates the node where the resource should be created. Exactly one bit of this UINT must be set. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>. Passing zero is equivalent to passing one, in order to simplify the usage of single-GPU adapters.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_properties#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint CreationNodeMask ;

	/// <summary>
	/// <para>For multi-adapter operation, this indicates the set of nodes where the resource is visible. <i>VisibleNodeMask</i> must have the same bit set that is set in <i>CreationNodeMask</i>. <i>VisibleNodeMask</i> can *also* have additional bits set for cross-node resources, but doing so can potentially reduce performance for resource accesses, so you should do so only when needed. Passing zero is equivalent to passing one, in order to simplify the usage of single-GPU adapters.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_properties#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint VisibleNodeMask ;
}

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TILE_SHAPE ) )]
public struct TileShape {
	/// <summary>The width in texels of the tile.</summary>
	public uint WidthInTexels ;

	/// <summary>The height in texels of the tile.</summary>
	public uint HeightInTexels ;

	/// <summary>The depth in texels of the tile.</summary>
	public uint DepthInTexels ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_PACKED_MIP_INFO ) )]
public struct PackedMipInfo {
	/// <summary>The number of standard mipmaps in the tiled resource.</summary>
	public byte NumStandardMips;

	/// <summary>
	/// <para>The number of packed mipmaps in the tiled resource.</para>
	/// <para>This number starts from the least detailed mipmap (either sharing tiles or using non standard tile layout). This number is 0 if no such packing is in the resource. For array surfaces, this value is the number of mipmaps that are packed for a given array slice where each array slice repeats the same packing.</para>
	/// <para>On Tier_2 tiled resources hardware, mipmaps that fill at least one standard shaped tile in all dimensions are not allowed to be included in the set of packed mipmaps. On Tier_1 hardware, mipmaps that are an integer multiple of one standard shaped tile in all dimensions are not allowed to be included in the set of packed mipmaps. Mipmaps with at least one dimension less than the standard tile shape may or may not be packed. When a given mipmap needs to be packed, all coarser mipmaps for a given array slice are considered packed as well.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_packed_mip_info#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public byte NumPackedMips;

	/// <summary>
	/// <para>The number of tiles for the packed mipmaps in the tiled resource.</para>
	/// <para>If there is no packing, this value is meaningless and is set to 0. Otherwise, it is set to the number of tiles that are needed to represent the set of packed mipmaps. The pixel layout within the packed mipmaps is hardware specific. If apps define only partial mappings for the set of tiles in packed mipmaps, read and write behavior is vendor specific and undefined. For arrays, this value is only the count of packed mipmaps within the subresources for each array slice.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_packed_mip_info#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NumTilesForPackedMips;

	/// <summary>
	/// <para>The offset of the first packed tile for the resource in the overall range of tiles. If <b>NumPackedMips</b> is 0, this value is meaningless and is 0. Otherwise, it is the offset of the first packed tile for the resource in the overall range of tiles for the resource. A value of 0 for <b>StartTileIndexInOverallResource</b> means the entire resource is packed. For array surfaces, this is the offset for the tiles that contain the packed mipmaps for the first array slice. Packed mipmaps for each array slice in arrayed surfaces are at this offset past the beginning of the tiles for each array slice.</para>
	/// <para><div class="alert"><b>Note</b>  The number of overall tiles, packed or not, for a given array slice is simply the total number of tiles for the resource divided by the resource's array size, so it is easy to locate the range of tiles for any given array slice, out of which <b>StartTileIndexInOverallResource</b> identifies which of those are packed. </div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_packed_mip_info#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint StartTileIndexInOverallResource;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_SUBRESOURCE_TILING ) )]
public struct SubresourceTiling {
	/// <summary>The width in tiles of the subresource.</summary>
	public uint WidthInTiles;

	/// <summary>The height in tiles of the subresource.</summary>
	public ushort HeightInTiles;

	/// <summary>The depth in tiles of the subresource.</summary>
	public ushort DepthInTiles;

	/// <summary>The index of the tile in the overall tiled subresource to start with.</summary>
	public uint StartTileIndexInOverallResource;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_RANGE ) )]
public struct Range {
	/// <summary>The offset, in bytes, denoting the beginning of a memory range.</summary>
	public nuint Begin ;

	/// <summary>
	/// <para>The offset, in bytes, denoting the end of a memory range. <b>End</b> is one-past-the-end.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_range#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public nuint End ;
} ;



[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_RESOURCE_DESC ) )]
public struct ResourceDescription {
	/// <summary>One member of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_dimension">D3D12_RESOURCE_DIMENSION</a>, specifying the dimensions of the resource (for example, D3D12_RESOURCE_DIMENSION_TEXTURE1D), or whether it is a buffer ((D3D12_RESOURCE_DIMENSION_BUFFER).</summary>
	public D3D12_RESOURCE_DIMENSION Dimension;

	/// <summary>Specifies the alignment.</summary>
	public ulong Alignment;

	/// <summary>Specifies the width of the resource.</summary>
	public ulong Width;

	/// <summary>Specifies the height of the resource.</summary>
	public uint Height;

	/// <summary>Specifies the depth of the resource, if it is 3D, or the array size if it is an array of 1D or 2D resources.</summary>
	public ushort DepthOrArraySize;

	/// <summary>Specifies the number of MIP levels.</summary>
	public ushort MipLevels;

	/// <summary>Specifies one member of  <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>.</summary>
	public Format Format;

	/// <summary>Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a> structure.</summary>
	public SampleDescription SampleDesc ;

	/// <summary>Specifies one member of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_layout">D3D12_TEXTURE_LAYOUT</a>.</summary>
	public D3D12_TEXTURE_LAYOUT Layout;

	/// <summary>Bitwise-OR'd flags, as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_flags">D3D12_RESOURCE_FLAGS</a> enumeration constants.</summary>
	public D3D12_RESOURCE_FLAGS Flags;
}


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_GRAPHICS_PIPELINE_STATE_DESC ) )]
public struct GraphicsPipelineStateDescription {
	/// <summary>A pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a> object.</summary>
	public ID3D12RootSignature pRootSignature;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the vertex shader.</summary>
	public D3D12_SHADER_BYTECODE VS;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the pixel shader.</summary>
	public D3D12_SHADER_BYTECODE PS;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the domain shader.</summary>
	public D3D12_SHADER_BYTECODE DS;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the hull shader.</summary>
	public D3D12_SHADER_BYTECODE HS;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the geometry shader.</summary>
	public D3D12_SHADER_BYTECODE GS;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_stream_output_desc">D3D12_STREAM_OUTPUT_DESC</a> structure that describes a streaming output buffer.</summary>
	public D3D12_STREAM_OUTPUT_DESC StreamOutput;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_blend_desc">D3D12_BLEND_DESC</a> structure that describes the blend state.</summary>
	public D3D12_BLEND_DESC BlendState;

	/// <summary>The sample mask for the blend state.</summary>
	public uint SampleMask;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_rasterizer_desc">D3D12_RASTERIZER_DESC</a> structure that describes the rasterizer state.</summary>
	public D3D12_RASTERIZER_DESC RasterizerState;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_desc">D3D12_DEPTH_STENCIL_DESC</a> structure that describes the depth-stencil state.</summary>
	public D3D12_DEPTH_STENCIL_DESC DepthStencilState;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_input_layout_desc">D3D12_INPUT_LAYOUT_DESC</a> structure that describes the input-buffer data for the input-assembler stage.</summary>
	public D3D12_INPUT_LAYOUT_DESC InputLayout;

	/// <summary>Specifies the properties of the index buffer in a  <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_index_buffer_strip_cut_value">D3D12_INDEX_BUFFER_STRIP_CUT_VALUE</a> structure.</summary>
	public D3D12_INDEX_BUFFER_STRIP_CUT_VALUE IBStripCutValue;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_primitive_topology_type">D3D12_PRIMITIVE_TOPOLOGY_TYPE</a>-typed value for the type of primitive, and ordering of the primitive data.</summary>
	public D3D12_PRIMITIVE_TOPOLOGY_TYPE PrimitiveTopologyType;

	/// <summary>The number of render target formats in the  <b>RTVFormats</b> member.</summary>
	public uint NumRenderTargets;

	/// <summary>An array of <a href="https://docs.microsoft.com/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed values for the render target formats.</summary>
	public __DXGI_FORMAT_8 RTVFormats;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value for the depth-stencil format.</summary>
	public Format DSVFormat ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a> structure that specifies multisampling parameters.</summary>
	public SampleDescription SampleDesc ;

	/// <summary>
	/// <para>For single GPU operation, set this to zero. If there are multiple GPU nodes, set bits to identify the nodes (the  device's physical adapters) for which the graphics pipeline state is to apply. Each bit in the mask corresponds to a single node. Refer to <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_graphics_pipeline_state_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NodeMask ;

	/// <summary>A cached pipeline state object, as a <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_cached_pipeline_state">D3D12_CACHED_PIPELINE_STATE</a> structure. pCachedBlob and CachedBlobSizeInBytes may be set to NULL and 0 respectively.</summary>
	public CachedPipelineState CachedPSO ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_flags">D3D12_PIPELINE_STATE_FLAGS</a> enumeration constant such as for "tool debug".</summary>
	public PipelineStateFlags Flags;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_COMPUTE_PIPELINE_STATE_DESC ) )]
public struct ComputePipelineStateDescription {
	/// <summary>A pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a> object.</summary>
	public IRootSignature pRootSignature ;
	
	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the compute shader.</summary>
	public ShaderBytecode CS ;

	/// <summary>
	/// <para>For single GPU operation, set this to zero. If there are multiple GPU nodes, set bits to identify the nodes (the  device's physical adapters) for which the compute pipeline state is to apply. Each bit in the mask corresponds to a single node. Refer to <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_compute_pipeline_state_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NodeMask ;

	/// <summary>A cached pipeline state object, as a <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_cached_pipeline_state">D3D12_CACHED_PIPELINE_STATE</a> structure. pCachedBlob and CachedBlobSizeInBytes may be set to NULL and 0 respectively.</summary>
	public CachedPipelineState CachedPSO ;
	
	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_flags">D3D12_PIPELINE_STATE_FLAGS</a> enumeration constant such as for "tool debug".</summary>
	public PipelineStateFlags Flags ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_CACHED_PIPELINE_STATE ) )]
public struct CachedPipelineState {
	/// <summary>Specifies pointer that references the memory location of the cache.</summary>
	public nint pCachedBlob ;

	/// <summary>Specifies the size of the cache in bytes.</summary>
	public ulong CachedBlobSizeInBytes ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_SHADER_BYTECODE ) )]
public struct ShaderBytecode {
	/// <summary>A pointer to a memory block that contains the shader data.</summary>
	public nint pShaderBytecode ;

	/// <summary>The size, in bytes, of the shader data that the <b>pShaderBytecode</b> member points to.</summary>
	public nuint BytecodeLength ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_DESCRIPTOR_HEAP_DESC ) )]
public struct DescriptorHeapDescription {
	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a>-typed value that specifies the types of descriptors in the heap.</summary>
	public DescriptorHeapType Type ;

	/// <summary>The number of descriptors in the heap.</summary>
	public uint NumDescriptors ;

	/// <summary>A combination of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_flags">D3D12_DESCRIPTOR_HEAP_FLAGS</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies options for the heap.</summary>
	public DescriptorHeapFlags Flags ;

	/// <summary>
	/// <para>For single-adapter operation, set this to zero. If there are multiple adapter nodes, set a bit to identify the node (one of the device's physical adapters) to which the descriptor heap applies. Each bit in the mask corresponds to a single node. Only one bit must be set. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_descriptor_heap_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NodeMask ;
	
	
	public DescriptorHeapDescription( DescriptorHeapType type, 
									  uint numDescriptors, 
									  DescriptorHeapFlags flags = 0, 
									  uint nodeMask = 0 ) {
		Type = type ;
		Flags = flags ;
		NodeMask = nodeMask ;
		NumDescriptors = numDescriptors ;
	}
	
	public DescriptorHeapDescription( in D3D12_DESCRIPTOR_HEAP_DESC desc ) {
		NodeMask       = desc.NodeMask ;
		NumDescriptors = desc.NumDescriptors ;
		Type           = ( DescriptorHeapType )desc.Type ;
		Flags          = ( DescriptorHeapFlags )desc.Flags ;
	}
	
	public static implicit operator D3D12_DESCRIPTOR_HEAP_DESC( in DescriptorHeapDescription desc ) => new D3D12_DESCRIPTOR_HEAP_DESC {
			NodeMask       = desc.NodeMask,
			NumDescriptors = desc.NumDescriptors,
			Type           = ( D3D12_DESCRIPTOR_HEAP_TYPE )desc.Type,
			Flags          = ( D3D12_DESCRIPTOR_HEAP_FLAGS )desc.Flags
		} ;
	public static implicit operator DescriptorHeapDescription( in D3D12_DESCRIPTOR_HEAP_DESC desc ) => new( desc ) ;
} ;


// -------------------------------------------------------
// Processor Descriptor Handles:
// -------------------------------------------------------

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_CPU_DESCRIPTOR_HANDLE ) )]
public struct CPUDescriptorHandle {
	/// <summary>The address of  the descriptor.</summary>
	public nuint ptr ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_GPU_DESCRIPTOR_HANDLE ) )]
public struct GPUDescriptorHandle {
	/// <summary>The address of the descriptor.</summary>
	public ulong ptr ;
} ;

// =======================================================



[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_CONSTANT_BUFFER_VIEW_DESC ) )]
public struct ConstBufferViewDescription {
	/// <summary>
	/// <para>The D3D12_GPU_VIRTUAL_ADDRESS of the constant buffer. D3D12_GPU_VIRTUAL_ADDRESS is a typedef'd alias of UINT64.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_constant_buffer_view_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ulong BufferLocation ;
	
	/// <summary>The size in bytes of the constant buffer.</summary>
	public uint SizeInBytes ;
	
	public ConstBufferViewDescription( ulong bufferLocation, uint sizeInBytes ) {
		BufferLocation = bufferLocation ;
		SizeInBytes    = sizeInBytes ;
	}
	public ConstBufferViewDescription(in D3D12_CONSTANT_BUFFER_VIEW_DESC desc) {
		BufferLocation = desc.BufferLocation ;
		SizeInBytes    = desc.SizeInBytes ;
	}
} ;


public struct ShaderResourceViewDescription {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value that specifies the viewing format. See remarks.</summary>
	public Format Format ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_srv_dimension">D3D12_SRV_DIMENSION</a>-typed value that specifies the resource type of the view. This type is the same as the resource type of the underlying resource. This member also determines which _SRV to use in the union below.</summary>
	public D3D12_SRV_DIMENSION ViewDimension;

	/// <summary>A value, constructed using the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_shader_component_mapping">D3D12_ENCODE_SHADER_4_COMPONENT_MAPPING</a> macro. The **D3D12_SHADER_COMPONENT_MAPPING** enumeration specifies what values from memory should be returned when the texture is accessed in a shader via this shader resource view (SRV). For example, it can route component 1 (green) from memory, or the constant `0`, into component 2 (`.b`) of the value given to the shader.</summary>
	public uint Shader4ComponentMapping;

	public _anon_union_A Anonymous;

	[StructLayout(LayoutKind.Explicit)]
	public partial struct _anon_union_A {
		[FieldOffset(0)]
		public D3D12_BUFFER_SRV Buffer;

		[FieldOffset(0)]
		public D3D12_TEX1D_SRV Texture1D;

		[FieldOffset(0)]
		public D3D12_TEX1D_ARRAY_SRV Texture1DArray;

		[FieldOffset(0)]
		public D3D12_TEX2D_SRV Texture2D;

		[FieldOffset(0)]
		public D3D12_TEX2D_ARRAY_SRV Texture2DArray;

		[FieldOffset(0)]
		public D3D12_TEX2DMS_SRV Texture2DMS;

		[FieldOffset(0)]
		public D3D12_TEX2DMS_ARRAY_SRV Texture2DMSArray;

		[FieldOffset(0)]
		public D3D12_TEX3D_SRV Texture3D;

		[FieldOffset(0)]
		public D3D12_TEXCUBE_SRV TextureCube;

		[FieldOffset(0)]
		public D3D12_TEXCUBE_ARRAY_SRV TextureCubeArray;

		[FieldOffset(0)]
		public D3D12_RAYTRACING_ACCELERATION_STRUCTURE_SRV RaytracingAccelerationStructure;
	}
}