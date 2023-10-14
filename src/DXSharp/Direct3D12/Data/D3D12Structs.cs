﻿#region Using Directives

using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
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

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_HEAP_DESC ) )]
public struct HeapDescription {
	/// <summary>
	/// <para>The size, in bytes, of the heap. To avoid wasting memory, applications should pass <i>SizeInBytes</i> values
	/// which are multiples of the effective <i>Alignment</i>; but non-aligned <i>SizeInBytes</i> is also supported, for convenience.
	/// To find out how large a heap must be to support textures with undefined layouts and adapter-specific sizes, call
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo">
	/// ID3D12Device::GetResourceAllocationInfo
	/// </a>.</para>
	/// <para>
	/// <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong SizeInBytes ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_heap_properties">D3D12_HEAP_PROPERTIES</a>
	/// structure that describes the heap properties.</summary>
	public HeapProperties Properties ;

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
	
	public static implicit operator HeapDescription( in D3D12_HEAP_DESC desc ) => new HeapDescription {
		SizeInBytes = desc.SizeInBytes,
		Properties  = desc.Properties,
		Alignment   = desc.Alignment,
		Flags       = desc.Flags
	} ;
	public static implicit operator D3D12_HEAP_DESC( in HeapDescription desc ) => new D3D12_HEAP_DESC {
		SizeInBytes = desc.SizeInBytes,
		Properties  = desc.Properties,
		Alignment   = desc.Alignment,
		Flags       = desc.Flags
	} ;
} ;



[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_HEAP_PROPERTIES ) )]
public struct HeapProperties {
	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type">D3D12_HEAP_TYPE</a>-typed value that specifies the type of heap.</summary>
	public HeapType Type ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_cpu_page_property">D3D12_CPU_PAGE_PROPERTY</a>-typed value that specifies the CPU-page properties for the heap.</summary>
	public CPUPageProperty CPUPageProperty ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_memory_pool">D3D12_MEMORY_POOL</a>-typed value that specifies the memory pool for the heap.</summary>
	public MemoryPool MemoryPoolPreference ;

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
	
	public static implicit operator HeapProperties( in D3D12_HEAP_PROPERTIES props ) => new HeapProperties {
			Type                 = (HeapType)props.Type,
			CPUPageProperty      = (CPUPageProperty)props.CPUPageProperty,
			MemoryPoolPreference = (MemoryPool)props.MemoryPoolPreference,
			CreationNodeMask     = props.CreationNodeMask,
			VisibleNodeMask      = props.VisibleNodeMask
	} ;
	public static implicit operator D3D12_HEAP_PROPERTIES( in HeapProperties props ) => new D3D12_HEAP_PROPERTIES {
			Type                 = (D3D12_HEAP_TYPE)props.Type,
			CPUPageProperty      = (D3D12_CPU_PAGE_PROPERTY)props.CPUPageProperty,
			MemoryPoolPreference = (D3D12_MEMORY_POOL)props.MemoryPoolPreference,
			CreationNodeMask     = props.CreationNodeMask,
			VisibleNodeMask      = props.VisibleNodeMask
	} ;
} ;


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
	
	
	public SubresourceTiling( in D3D12_SUBRESOURCE_TILING tiling ) {
		WidthInTiles               = tiling.WidthInTiles;
		HeightInTiles              = tiling.HeightInTiles;
		DepthInTiles               = tiling.DepthInTiles;
		StartTileIndexInOverallResource = tiling.StartTileIndexInOverallResource;
	}
	
	public static implicit operator SubresourceTiling( in D3D12_SUBRESOURCE_TILING tiling ) => new SubresourceTiling( tiling ) ;
	public static implicit operator D3D12_SUBRESOURCE_TILING( in SubresourceTiling tiling ) => new D3D12_SUBRESOURCE_TILING {
		WidthInTiles               = tiling.WidthInTiles,
		HeightInTiles              = tiling.HeightInTiles,
		DepthInTiles               = tiling.DepthInTiles,
		StartTileIndexInOverallResource = tiling.StartTileIndexInOverallResource
	} ;
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
	public ResourceDimension Dimension ;

	/// <summary>Specifies the alignment.</summary>
	public ulong Alignment ;

	/// <summary>Specifies the width of the resource.</summary>
	public ulong Width ;

	/// <summary>Specifies the height of the resource.</summary>
	public uint Height ;

	/// <summary>Specifies the depth of the resource, if it is 3D, or the array size if it is an array of 1D or 2D resources.</summary>
	public ushort DepthOrArraySize ;

	/// <summary>Specifies the number of MIP levels.</summary>
	public ushort MipLevels ;

	/// <summary>Specifies one member of  <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>.</summary>
	public Format Format ;

	/// <summary>Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a> structure.</summary>
	public SampleDescription SampleDesc ;

	/// <summary>Specifies one member of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_layout">D3D12_TEXTURE_LAYOUT</a>.</summary>
	public TextureLayout Layout ;

	/// <summary>Bitwise-OR'd flags, as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_flags">D3D12_RESOURCE_FLAGS</a> enumeration constants.</summary>
	public ResourceFlags Flags ;
	
	
	public ResourceDescription( ResourceDimension dimension, ulong alignment, 
								ulong width, uint height, ushort depthOrArraySize, 
								ushort mipLevels, Format format, SampleDescription sampleDesc, 
								TextureLayout layout, ResourceFlags flags ) {
		Dimension        = dimension ;
		Alignment        = alignment ;
		Width            = width ;
		Height           = height ;
		DepthOrArraySize = depthOrArraySize ;
		MipLevels        = mipLevels ;
		Format           = format ;
		SampleDesc       = sampleDesc ;
		Layout           = layout ;
		Flags            = flags ;
	}
	
	public ResourceDescription( in D3D12_RESOURCE_DESC desc ) {
		Dimension        = (ResourceDimension)desc.Dimension ;
		Alignment        = desc.Alignment ;
		Width            = desc.Width ;
		Height           = desc.Height ;
		DepthOrArraySize = desc.DepthOrArraySize ;
		MipLevels        = desc.MipLevels ;
		Format           = (Format)desc.Format ;
		SampleDesc       = (SampleDescription)desc.SampleDesc ;
		Layout           = (TextureLayout)desc.Layout ;
		Flags            = (ResourceFlags)desc.Flags ;
	}
	
	public static implicit operator ResourceDescription( in D3D12_RESOURCE_DESC desc ) => new ResourceDescription {
			Dimension       = (ResourceDimension)desc.Dimension,
			Alignment       = desc.Alignment,
			Width           = desc.Width,
			Height          = desc.Height,
			DepthOrArraySize= desc.DepthOrArraySize,
			MipLevels       = desc.MipLevels,
			Format          = (Format)desc.Format,
			SampleDesc      = (SampleDescription)desc.SampleDesc,
			Layout          = (TextureLayout)desc.Layout,
			Flags           = (ResourceFlags)desc.Flags
	} ;
	public static implicit operator D3D12_RESOURCE_DESC( in ResourceDescription desc ) => new D3D12_RESOURCE_DESC {
			Dimension       = (D3D12_RESOURCE_DIMENSION)desc.Dimension,
			Alignment       = desc.Alignment,
			Width           = desc.Width,
			Height          = desc.Height,
			DepthOrArraySize= desc.DepthOrArraySize,
			MipLevels       = desc.MipLevels,
			Format          = (DXGI_FORMAT)desc.Format,
			SampleDesc      = (DXGI_SAMPLE_DESC)desc.SampleDesc,
			Layout          = (D3D12_TEXTURE_LAYOUT)desc.Layout,
			Flags           = (D3D12_RESOURCE_FLAGS)desc.Flags
	} ;
} ;



/// <summary>Describes the rasterizer state.</summary>
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof(D3D12_RASTERIZER_DESC) )]
public struct RasterizerDescription {
	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_fill_mode">D3D12_FILL_MODE</a>-typed
	/// value that specifies the fill mode to use when rendering.
	/// </summary>
	public FillMode FillMode ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_cull_mode">D3D12_CULL_MODE</a>-typed value
	/// that specifies that triangles facing the specified direction are not drawn.
	/// </summary>
	public CullMode CullMode ;

	/// <summary>
	/// Determines if a triangle is front- or back-facing. If this member is <b>TRUE</b>, a triangle will be considered
	/// front-facing if its vertices are counter-clockwise on the render target and considered back-facing if they are clockwise.
	/// If this parameter is <b>FALSE</b>, the opposite is true.
	/// </summary>
	public BOOL FrontCounterClockwise ;

	/// <summary>
	/// Depth value added to a given pixel. For info about depth bias, see
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3d11/d3d10-graphics-programming-guide-output-merger-stage-depth-bias">Depth Bias</a>.
	/// </summary>
	public int DepthBias ;

	/// <summary>
	/// Maximum depth bias of a pixel. For info about depth bias, see
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3d11/d3d10-graphics-programming-guide-output-merger-stage-depth-bias">
	/// Depth Bias
	/// </a>.
	/// </summary>
	public float DepthBiasClamp ;

	/// <summary>
	/// Scalar on a given pixel's slope. For info about depth bias, see
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3d11/d3d10-graphics-programming-guide-output-merger-stage-depth-bias">
	/// Depth Bias
	/// </a>.
	/// </summary>
	public float SlopeScaledDepthBias ;

	/// <summary>
	/// <para>Specifies whether to enable clipping based on distance.</para>
	/// <para>The hardware always performs x and y clipping of rasterized coordinates. When <b>DepthClipEnable</b> is set to the default–<b>TRUE</b>,
	/// the hardware also clips the z value (that is, the hardware performs the last step of the following algorithm).</para>
	/// <para/><para/>
	/// <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_rasterizer_desc#members">
	/// Read more on docs.microsoft.com
	/// </a>.
	/// </summary>
	public BOOL DepthClipEnable ;

	/// <summary>
	/// Specifies whether to use the quadrilateral or alpha line anti-aliasing algorithm on multisample antialiasing (MSAA) render targets.
	/// Set to <b>TRUE</b> to use the quadrilateral line anti-aliasing algorithm and to <b>FALSE</b> to use the alpha line anti-aliasing
	/// algorithm. For more info about this member, see Remarks.
	/// </summary>
	public BOOL MultisampleEnable ;

	/// <summary>
	/// Specifies whether to enable line antialiasing; only applies if doing line drawing and <b>MultisampleEnable</b> is <b>FALSE</b>.
	/// For more info about this member, see Remarks.
	/// </summary>
	public BOOL AntialiasedLineEnable ;

	/// <summary>
	/// <para>
	/// Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b>
	/// The sample count that is forced while UAV rendering or rasterizing. Valid values are 0, 1, 4, 8, and optionally 16.
	/// 0 indicates that the sample count is not forced.
	/// <b>Note</b>  If you want to render with <b>ForcedSampleCount</b> set to 1 or greater, you must follow these guidelines: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_rasterizer_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint ForcedSampleCount ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_conservative_rasterization_mode">
	/// D3D12_CONSERVATIVE_RASTERIZATION_MODE</a>-typed value that identifies whether conservative rasterization is on or off.
	/// </summary>
	public ConservativeRasterizationMode ConservativeRaster ;
	
	
	public RasterizerDescription( FillMode fillMode, CullMode cullMode, BOOL frontCounterClockwise, int depthBias, float depthBiasClamp, float slopeScaledDepthBias, BOOL depthClipEnable, BOOL multisampleEnable, BOOL antialiasedLineEnable, uint forcedSampleCount, ConservativeRasterizationMode conservativeRaster ) {
		FillMode               = fillMode ;
		CullMode               = cullMode ;
		FrontCounterClockwise  = frontCounterClockwise ;
		DepthBias              = depthBias ;
		DepthBiasClamp         = depthBiasClamp ;
		SlopeScaledDepthBias   = slopeScaledDepthBias ;
		DepthClipEnable        = depthClipEnable ;
		MultisampleEnable      = multisampleEnable ;
		AntialiasedLineEnable  = antialiasedLineEnable ;
		ForcedSampleCount      = forcedSampleCount ;
		ConservativeRaster     = conservativeRaster ;
	}
	
	public RasterizerDescription( in D3D12_RASTERIZER_DESC desc ) {
		FillMode               = (FillMode)desc.FillMode ;
		CullMode               = (CullMode)desc.CullMode ;
		FrontCounterClockwise  = desc.FrontCounterClockwise ;
		DepthBias              = desc.DepthBias ;
		DepthBiasClamp         = desc.DepthBiasClamp ;
		SlopeScaledDepthBias   = desc.SlopeScaledDepthBias ;
		DepthClipEnable        = desc.DepthClipEnable ;
		MultisampleEnable      = desc.MultisampleEnable ;
		AntialiasedLineEnable  = desc.AntialiasedLineEnable ;
		ForcedSampleCount      = desc.ForcedSampleCount ;
		ConservativeRaster     = (ConservativeRasterizationMode)desc.ConservativeRaster ;
	}
	
	public static implicit operator RasterizerDescription( in D3D12_RASTERIZER_DESC desc ) => new RasterizerDescription {
			FillMode               = (FillMode)desc.FillMode,
			CullMode               = (CullMode)desc.CullMode,
			FrontCounterClockwise  = desc.FrontCounterClockwise,
			DepthBias              = desc.DepthBias,
			DepthBiasClamp         = desc.DepthBiasClamp,
			SlopeScaledDepthBias   = desc.SlopeScaledDepthBias,
			DepthClipEnable        = desc.DepthClipEnable,
			MultisampleEnable      = desc.MultisampleEnable,
			AntialiasedLineEnable  = desc.AntialiasedLineEnable,
			ForcedSampleCount      = desc.ForcedSampleCount,
			ConservativeRaster     = (ConservativeRasterizationMode)desc.ConservativeRaster
	} ;
	
	public static implicit operator D3D12_RASTERIZER_DESC( in RasterizerDescription desc ) => new D3D12_RASTERIZER_DESC {
			FillMode               = (D3D12_FILL_MODE)desc.FillMode,
			CullMode               = (D3D12_CULL_MODE)desc.CullMode,
			FrontCounterClockwise  = desc.FrontCounterClockwise,
			DepthBias              = desc.DepthBias,
			DepthBiasClamp         = desc.DepthBiasClamp,
			SlopeScaledDepthBias   = desc.SlopeScaledDepthBias,
			DepthClipEnable        = desc.DepthClipEnable,
			MultisampleEnable      = desc.MultisampleEnable,
			AntialiasedLineEnable  = desc.AntialiasedLineEnable,
			ForcedSampleCount      = desc.ForcedSampleCount,
			ConservativeRaster     = (D3D12_CONSERVATIVE_RASTERIZATION_MODE)desc.ConservativeRaster
	} ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_INPUT_ELEMENT_DESC ) )]
public struct InputElementDescription {
	/// <summary>The HLSL semantic associated with this element in a shader input-signature. See <a href="https://docs.microsoft.com/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics">HLSL Semantics</a> for more info.</summary>
	public PCSTR SemanticName ;

	/// <summary>
	/// <para>The semantic index for the element. A semantic index modifies a semantic, with an integer index number. A semantic index is only needed in a case where there is more than one element with the same semantic. For example, a 4x4 matrix would have four components each with the semantic name <b>matrix</b>, however each of the four component would have different semantic indices (0, 1, 2, and 3).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_input_element_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint SemanticIndex ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value that specifies the format of the element data.</summary>
	public Format Format ;

	/// <summary>An integer value that identifies the input-assembler. For more info, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/d3d10-graphics-programming-guide-input-assembler-stage-getting-started">Input Slots</a>. Valid values are between 0 and 15.</summary>
	public uint InputSlot ;

	/// <summary>
	/// <para>Optional. Offset, in bytes, to this element from the start of the vertex. Use D3D12_APPEND_ALIGNED_ELEMENT (0xffffffff) for convenience to define the current element directly after the previous one, including any packing if necessary.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_input_element_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint AlignedByteOffset ;

	/// <summary>A value that identifies the input data class for a single input slot.</summary>
	public InputClassification InputSlotClass ;

	/// <summary>
	/// <para>The number of instances to draw using the same per-instance data before advancing in the buffer by one element. This value must be 0 for an element that contains per-vertex data (the slot class is set to the D3D12_INPUT_PER_VERTEX_DATA member of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_input_classification">D3D12_INPUT_CLASSIFICATION</a>).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_input_element_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint InstanceDataStepRate ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_INPUT_LAYOUT_DESC ) )]
public struct InputLayoutDescription {
	/// <summary>
	/// An array of <see cref="InputLayoutDescription"/> structures that describe the input data for the input-assembler stage.
	/// </summary>
	/// <remarks>
	/// For more information about the native DirectX structure, see: 
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_input_element_desc">D3D12_INPUT_ELEMENT_DESC</a>
	/// </remarks>
	public nint pInputElementDescs ;
	
	/// <summary>The number of input-data types in the array of input elements that the <b>pInputElementDescs</b> member points to.</summary>
	public uint NumElements ;
	
	
	/// <summary>
	/// Gets or sets the array of <see cref="InputLayoutDescription"/> structures that describe the input data for the input-assembler stage.
	/// </summary>
	/// <remarks>
	/// <b>WARNING: Use with caution!</b><para/>
	/// Using the `set` accessor will allocate unmanaged memory for the array of <see cref="InputLayoutDescription"/> structures.
	/// The value of the <see cref="NumElements"/> property will be set to the length of the array, and the value of the
	/// <see cref="pInputElementDescs"/> property will be set to the address of the first element in the array in umanaged memory.
	/// This is not automatically garbage-collected for you like a managed array! It is your responsibility to free the memory block
	/// pointed to by <see cref="pInputElementDescs"/> when you are done with it. You can do this by calling <see cref="Marshal.FreeHGlobal(IntPtr)"/>
	/// or other memory management functions. If you create <b>InputLayoutDescription</b> structures in a loop and forget to free the memory,
	/// when the structure goes out of scope you will no longer know the allocation address, thus the memory will be leaked! Use wisely and
	/// don't forget to do your chores and clean up! This exists for convenience, but can turn out bad if misused!
	/// </remarks>
	internal InputLayoutDescription[ ]? InputLayoutDescriptions {
		get { unsafe {
				if ( pInputElementDescs is 0x0000 || NumElements is 0 ) return Array.Empty< InputLayoutDescription >( ) ;
				Span< InputLayoutDescription > descSpan = new( (InputLayoutDescription*)pInputElementDescs, (int)NumElements ) ;
				return descSpan.ToArray( ) ;
			}
		}
		set {
			unsafe {
				if( value is not { Length: > 0 } ) {
					pInputElementDescs = 0 ;
					NumElements = 0 ;
					return ;
				}
				fixed ( InputLayoutDescription* pValue = value ) {
					NumElements = (uint)value.Length ;
					
					Span< InputLayoutDescription > descSpan = new( pValue, value.Length ) ;
					nint address = Marshal.AllocHGlobal( sizeof(InputLayoutDescription) * value.Length ) ;
					descSpan.CopyTo( new( (InputLayoutDescription *)address, value.Length ) ) ;
					pInputElementDescs = address ;
				}
			}
		}
	}
	
	public InputLayoutDescription( nint pInputElementDescs, uint numElements ) {
		this.pInputElementDescs = pInputElementDescs ;
		this.NumElements = numElements ;
	}
	/// <summary>Creates an input LayoutDescription with backing unmanaged memory for the array of <see cref="InputLayoutDescription"/> structures.</summary>
	/// <remarks>
	/// <b>WARNING:</b> <para/>
	/// This allocates unmanaged memory which is not garbage-collected for you!
	/// See the warning of the <see cref="InputLayoutDescriptions"/> property for more information.
	/// Release the memory pointed to by <see cref="pInputElementDescs"/> when you are done with it
	/// by calling <see cref="Marshal.FreeHGlobal(IntPtr)"/> or other memory management functions.<para/>
	/// These things are marked `internal` to prevent them from wrecking people's day.
	/// </remarks>
	/// <param name="inputLayoutDescriptions">Managed array of InputLayoutDescription structures to allocate into unmanaged memory.</param>
	internal InputLayoutDescription( InputLayoutDescription[ ] inputLayoutDescriptions ) {
		this.InputLayoutDescriptions = inputLayoutDescriptions ;
		this.NumElements = (uint)inputLayoutDescriptions.Length ;
	}
	
	public InputLayoutDescription( in D3D12_INPUT_LAYOUT_DESC desc ) {
		unsafe { this.pInputElementDescs = (nint)desc.pInputElementDescs ; }
		this.NumElements = desc.NumElements ;
	}
	
	public static implicit operator D3D12_INPUT_LAYOUT_DESC( in InputLayoutDescription desc ) {
		unsafe { return new D3D12_INPUT_LAYOUT_DESC {
				pInputElementDescs = (D3D12_INPUT_ELEMENT_DESC*)desc.pInputElementDescs,
				NumElements = desc.NumElements
			} ;
		}
	}
	public static implicit operator InputLayoutDescription( in D3D12_INPUT_LAYOUT_DESC desc ) {
		unsafe { return new InputLayoutDescription {
				pInputElementDescs = (nint)desc.pInputElementDescs,
				NumElements = desc.NumElements
			} ;
		}
	}
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_SO_DECLARATION_ENTRY ) )]
public struct SODeclarationEntry {
	/// <summary>Zero-based, stream number.</summary>
	public uint Stream ;

	/// <summary>
	/// <para>Type of output element; possible values include: <b>"POSITION"</b>, <b>"NORMAL"</b>, or <b>"TEXCOORD0"</b>. Note that if <b>SemanticName</b> is <b>NULL</b> then <b>ComponentCount</b> can be greater than 4 and the described entry will be a gap in the stream out where no data will be written.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_so_declaration_entry#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public PCSTR SemanticName ;

	/// <summary>Output element's zero-based index. Use, for example, if you have more than one texture coordinate stored in each vertex.</summary>
	public uint SemanticIndex ;

	/// <summary>
	/// <para>The component of the entry to begin writing out to. Valid values are 0 to 3. For example, if you only wish to output to the y and z components of a position, <b>StartComponent</b> is 1 and <b>ComponentCount</b> is 2.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_so_declaration_entry#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public byte StartComponent ;

	/// <summary>
	/// <para>The number of components of the entry to write out to. Valid values are 1 to 4. For example, if you only wish to output to the y and z components of a position, <b>StartComponent</b> is 1 and <b>ComponentCount</b> is 2.  Note that if <b>SemanticName</b> is <b>NULL</b> then <b>ComponentCount</b> can be greater than 4 and the described entry will be a gap in the stream out where no data will be written.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_so_declaration_entry#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public byte ComponentCount ;

	/// <summary>
	/// <para>The associated stream output buffer that is bound to the pipeline. The valid range for <b>OutputSlot</b> is 0 to 3.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_so_declaration_entry#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public byte OutputSlot ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_STREAM_OUTPUT_DESC ) )]
public struct StreamOuputDescription {
	/// <summary>An array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_so_declaration_entry">D3D12_SO_DECLARATION_ENTRY</a> structures. Can't be <b>NULL</b> if <b>NumEntries</b> &gt; 0.</summary>
	public unsafe SODeclarationEntry* pSODeclaration ;

	/// <summary>The number of entries in the stream output declaration array that the <b>pSODeclaration</b> member points to.</summary>
	public uint NumEntries ;

	/// <summary>An array of buffer strides; each stride is the size of an element for that buffer.</summary>
	public unsafe uint* pBufferStrides ;

	/// <summary>The number of strides (or buffers) that the <b>pBufferStrides</b> member points to.</summary>
	public uint NumStrides ;

	/// <summary>The index number of the stream to be sent to the rasterizer stage.</summary>
	public uint RasterizedStream ;
	
	public unsafe StreamOuputDescription( SODeclarationEntry* pSODeclaration, uint numEntries, 
										  uint* pBufferStrides, uint numStrides, uint rasterizedStream ) {
		this.pSODeclaration = pSODeclaration ;
		this.NumEntries = numEntries ;
		this.pBufferStrides = pBufferStrides ;
		this.NumStrides = numStrides ;
		this.RasterizedStream = rasterizedStream ;
	}
	
	public StreamOuputDescription( in D3D12_STREAM_OUTPUT_DESC desc ) {
		unsafe {
			this.pSODeclaration = (SODeclarationEntry*)desc.pSODeclaration ;
			this.NumEntries = desc.NumEntries ;
			this.pBufferStrides = (uint*)desc.pBufferStrides ;
			this.NumStrides = desc.NumStrides ;
			this.RasterizedStream = desc.RasterizedStream ;
		}
	}
	
	public static implicit operator D3D12_STREAM_OUTPUT_DESC( in StreamOuputDescription desc ) {
		unsafe { return new D3D12_STREAM_OUTPUT_DESC {
				pSODeclaration = (D3D12_SO_DECLARATION_ENTRY*)desc.pSODeclaration,
				NumEntries = desc.NumEntries,
				pBufferStrides = (uint*)desc.pBufferStrides,
				NumStrides = desc.NumStrides,
				RasterizedStream = desc.RasterizedStream
			} ;
		}
	}
	
	public static implicit operator StreamOuputDescription( in D3D12_STREAM_OUTPUT_DESC desc ) {
		unsafe { return new StreamOuputDescription {
				pSODeclaration = (SODeclarationEntry*)desc.pSODeclaration,
				NumEntries = desc.NumEntries,
				pBufferStrides = (uint*)desc.pBufferStrides,
				NumStrides = desc.NumStrides,
				RasterizedStream = desc.RasterizedStream
			} ;
		}
	}
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_GRAPHICS_PIPELINE_STATE_DESC ) )]
public struct GraphicsPipelineStateDescription {
	/// <summary>A pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a> object.</summary>
	[MarshalAs(UnmanagedType.Interface)] public ID3D12RootSignature pRootSignature ;
	//public nint pRootSignature ;
	
	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the vertex shader.</summary>
	public ShaderBytecode VS;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the pixel shader.</summary>
	public ShaderBytecode PS;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the domain shader.</summary>
	public ShaderBytecode DS;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the hull shader.</summary>
	public ShaderBytecode HS;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode">D3D12_SHADER_BYTECODE</a> structure that describes the geometry shader.</summary>
	public ShaderBytecode GS;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_stream_output_desc">D3D12_STREAM_OUTPUT_DESC</a> structure that describes a streaming output buffer.</summary>
	public StreamOuputDescription StreamOutput;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_blend_desc">D3D12_BLEND_DESC</a> structure that describes the blend state.</summary>
	public BlendDescription BlendState;

	/// <summary>The sample mask for the blend state.</summary>
	public uint SampleMask;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_rasterizer_desc">D3D12_RASTERIZER_DESC</a> structure that describes the rasterizer state.</summary>
	public RasterizerDescription RasterizerState;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_desc">D3D12_DEPTH_STENCIL_DESC</a> structure that describes the depth-stencil state.</summary>
	public DepthStencilDesc DepthStencilState;
	
	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_input_layout_desc">D3D12_INPUT_LAYOUT_DESC</a> structure that describes the input-buffer data for the input-assembler stage.</summary>
	public InputLayoutDescription InputLayout;

	/// <summary>Specifies the properties of the index buffer in a  <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_index_buffer_strip_cut_value">D3D12_INDEX_BUFFER_STRIP_CUT_VALUE</a> structure.</summary>
	public IndexBufferStripCutValue IBStripCutValue;

	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_primitive_topology_type">D3D12_PRIMITIVE_TOPOLOGY_TYPE</a>-typed value for the type of primitive, and ordering of the primitive data.</summary>
	public PrimitiveTopology PrimitiveTopologyType;

	/// <summary>The number of render target formats in the  <b>RTVFormats</b> member.</summary>
	public uint NumRenderTargets;

	/// <summary>An array of <a href="https://docs.microsoft.com/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed values for the render target formats.</summary>
	public Format8 RTVFormats;

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
	public PipelineStateFlags Flags ;
	
	public GraphicsPipelineStateDescription( in D3D12_GRAPHICS_PIPELINE_STATE_DESC desc ) {
		this.pRootSignature        = desc.pRootSignature ;
		this.VS                    = desc.VS ;
		this.PS                    = desc.PS ;
		this.DS                    = desc.DS ;
		this.HS                    = desc.HS ;
		this.GS                    = desc.GS ;
		this.StreamOutput          = desc.StreamOutput ;
		this.BlendState            = new( desc.BlendState ) ;
		this.SampleMask            = desc.SampleMask ;
		this.RasterizerState       = desc.RasterizerState ;
		this.DepthStencilState     = desc.DepthStencilState ;
		this.InputLayout           = desc.InputLayout ;
		this.IBStripCutValue       = (IndexBufferStripCutValue)desc.IBStripCutValue ;
		this.PrimitiveTopologyType = (PrimitiveTopology)desc.PrimitiveTopologyType ;
		this.NumRenderTargets      = desc.NumRenderTargets ;
		this.RTVFormats            = desc.RTVFormats ;
		this.DSVFormat             = (Format)desc.DSVFormat ;
		this.SampleDesc            = desc.SampleDesc ;
		this.NodeMask              = desc.NodeMask ;
		this.CachedPSO             = desc.CachedPSO ;
		this.Flags                 = (PipelineStateFlags)desc.Flags ;
	}
	
	public static implicit operator D3D12_GRAPHICS_PIPELINE_STATE_DESC( in GraphicsPipelineStateDescription desc ) => 
		new D3D12_GRAPHICS_PIPELINE_STATE_DESC {
			pRootSignature        = desc.pRootSignature,
			VS                    = desc.VS,
			PS                    = desc.PS,
			DS                    = desc.DS,
			HS                    = desc.HS,
			GS                    = desc.GS,
			StreamOutput          = desc.StreamOutput,
			BlendState            = desc.BlendState,
			SampleMask            = desc.SampleMask,
			RasterizerState       = desc.RasterizerState,
			DepthStencilState     = desc.DepthStencilState,
			InputLayout           = desc.InputLayout,
			IBStripCutValue       = (D3D12_INDEX_BUFFER_STRIP_CUT_VALUE)desc.IBStripCutValue,
			PrimitiveTopologyType = (D3D12_PRIMITIVE_TOPOLOGY_TYPE)desc.PrimitiveTopologyType,
			NumRenderTargets      = desc.NumRenderTargets,
			RTVFormats            = desc.RTVFormats,
			DSVFormat             = (DXGI_FORMAT)desc.DSVFormat,
			SampleDesc            = desc.SampleDesc,
			NodeMask              = desc.NodeMask,
			CachedPSO             = desc.CachedPSO,
			Flags                 = (D3D12_PIPELINE_STATE_FLAGS)desc.Flags
	} ;
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
	
	
	
	public CachedPipelineState( nint pCachedBlob, ulong cachedBlobSizeInBytes ) {
		this.pCachedBlob = pCachedBlob ;
		this.CachedBlobSizeInBytes = cachedBlobSizeInBytes ;
	}
	
	public CachedPipelineState( in D3D12_CACHED_PIPELINE_STATE desc ) {
		unsafe { this.pCachedBlob = (nint)desc.pCachedBlob ; }
		this.CachedBlobSizeInBytes = desc.CachedBlobSizeInBytes ;
	}
	
	public static unsafe implicit operator D3D12_CACHED_PIPELINE_STATE( in CachedPipelineState desc ) => 
		new D3D12_CACHED_PIPELINE_STATE {
			pCachedBlob = (void *)desc.pCachedBlob,
			CachedBlobSizeInBytes = (nuint)desc.CachedBlobSizeInBytes,
		} ;
	
	public static unsafe implicit operator CachedPipelineState( in D3D12_CACHED_PIPELINE_STATE desc ) => new CachedPipelineState {
			pCachedBlob = (nint)desc.pCachedBlob,
			CachedBlobSizeInBytes = desc.CachedBlobSizeInBytes
	} ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_SHADER_BYTECODE ) )]
public struct ShaderBytecode {
	/// <summary>A pointer to a memory block that contains the shader data.</summary>
	public nint pShaderBytecode ;

	/// <summary>The size, in bytes, of the shader data that the <b>pShaderBytecode</b> member points to.</summary>
	public nuint BytecodeLength ;
	
	public ShaderBytecode( nint pShaderBytecode, nuint bytecodeLength ) {
		this.pShaderBytecode = pShaderBytecode ;
		this.BytecodeLength = bytecodeLength ;
	}
	
	public static unsafe implicit operator D3D12_SHADER_BYTECODE( in ShaderBytecode bytecode ) => new D3D12_SHADER_BYTECODE {
			pShaderBytecode = (void*)bytecode.pShaderBytecode,
			BytecodeLength  = bytecode.BytecodeLength
	} ;
	public static unsafe implicit operator ShaderBytecode( in D3D12_SHADER_BYTECODE bytecode ) => new ShaderBytecode {
			pShaderBytecode = (nint)bytecode.pShaderBytecode,
			BytecodeLength  = bytecode.BytecodeLength
	} ;
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


[StructLayout(LayoutKind.Sequential),
	ProxyFor(typeof(D3D12_SAMPLER_DESC))]
public struct SamplerDescription {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">D3D12_FILTER</a>-typed value that specifies the filtering method to use when sampling a texture.</summary>
	public Filter Filter;
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">D3D12_TEXTURE_ADDRESS_MODE</a>-typed value that specifies the method to use for resolving a u texture coordinate that is outside the 0 to 1 range.</summary>
	public TextureAddressMode AddressU;
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">D3D12_TEXTURE_ADDRESS_MODE</a>-typed value that specifies the method to use for resolving a v texture coordinate that is outside the 0 to 1 range.</summary>
	public TextureAddressMode AddressV;
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">D3D12_TEXTURE_ADDRESS_MODE</a>-typed value that specifies the method to use for resolving a w texture coordinate that is outside the 0 to 1 range.</summary>
	public TextureAddressMode AddressW;

	/// <summary>Offset from the calculated mipmap level. For example, if the runtime calculates that a texture should be sampled at mipmap level 3 and <b>MipLODBias</b> is 2, the texture will be sampled at mipmap level 5.</summary>
	public float MipLODBias;
	/// <summary>Clamping value used if <b>D3D12_FILTER_ANISOTROPIC</b> or <b>D3D12_FILTER_COMPARISON_ANISOTROPIC</b> is specified in <b>Filter</b>. Valid values are between 1 and 16.</summary>
	public uint MaxAnisotropy;
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_comparison_func">D3D12_COMPARISON_FUNC</a>-typed value that specifies a function that compares sampled data against existing sampled data.</summary>
	public ComparisonFunction ComparisonFunc;

	//! TODO: Vector4/float4 SIMD vector implementation (currently only have double-precision)
	/// <summary>RGBA border color to use if <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">D3D12_TEXTURE_ADDRESS_MODE_BORDER</a> is specified for <b>AddressU</b>, <b>AddressV</b>, or <b>AddressW</b>. Range must be between 0.0 and 1.0 inclusive.</summary>
	public __float_4 BorderColor;

	/// <summary>Lower end of the mipmap range to clamp access to, where 0 is the largest and most detailed mipmap level and any level higher than that is less detailed.</summary>
	public float MinLOD;

	/// <summary>Upper end of the mipmap range to clamp access to, where 0 is the largest and most detailed mipmap level and any level higher than that is less detailed. This value must be greater than or equal to <b>MinLOD</b>. To have no upper limit on LOD, set this member to a large value.</summary>
	public float MaxLOD;
} ;


[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_RESOURCE_ALLOCATION_INFO))]
public struct ResourceAllocationInfo {
	/// <summary>
	/// <para>Type: **[UINT64](/windows/win32/WinProg/windows-data-types)** The size, in bytes, of the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_allocation_info#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ulong SizeInBytes;

	/// <summary>
	/// <para>Type: **[UINT64](/windows/win32/WinProg/windows-data-types)** The alignment value for the resource; one of 4KB (4096), 64KB (65536), or 4MB (4194304) alignment.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_allocation_info#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ulong Alignment;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_CLEAR_VALUE ) )]
public struct ClearValue {
	/// <summary>
	/// <para>Specifies one member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a> enum. The format of the commonly cleared color follows the same validation rules as a view/ descriptor creation. In general, the format of the clear color can be any format in the same typeless group that the resource format belongs to. This <i>Format</i> must match the format of the view used during the clear operation. It indicates whether the <i>Color</i> or the <i>DepthStencil</i> member is valid and how to convert the values for usage with the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_clear_value#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public Format Format ;

	public _anon_clrval_union ReadAs ;
	[StructLayout( LayoutKind.Explicit )]
	public partial struct _anon_clrval_union {
		[FieldOffset( 0 )] public __float_4 Color ;
		[FieldOffset( 0 )] public DepthStencilValue DepthStencil ;
	} ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_DEPTH_STENCIL_VALUE ) )]
public struct DepthStencilValue {
	/// <summary>Specifies the depth value.</summary>
	public float Depth;
	/// <summary>Specifies the stencil value.</summary>
	public byte Stencil;
} ;

public struct BlendDescription {
	/// <summary>Specifies whether to use alpha-to-coverage as a multisampling technique when setting a pixel to a render target. For more info about using alpha-to-coverage, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/d3d10-graphics-programming-guide-blend-state">Alpha-To-Coverage</a>.</summary>
	public BOOL AlphaToCoverageEnable ;

	/// <summary>
	/// <para>Specifies whether to enable independent blending in simultaneous render targets. Set to <b>TRUE</b> to enable independent blending. If set to <b>FALSE</b>, only the <b>RenderTarget</b>[0] members are used; <b>RenderTarget</b>[1..7] are ignored. See the **Remarks** section for restrictions.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_blend_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL IndependentBlendEnable ;

	/// <summary>An array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_target_blend_desc">D3D12_RENDER_TARGET_BLEND_DESC</a> structures that describe the blend states for render targets; these correspond to the eight render targets that can be bound to the <a href="https://docs.microsoft.com/windows/desktop/direct3d11/d3d10-graphics-programming-guide-output-merger-stage">output-merger stage</a> at one time.</summary>
	public RTBlendDescription8 RenderTarget ;
	
	
	
	public BlendDescription( BOOL alphaToCoverageEnable, BOOL independentBlendEnable, 
							 RTBlendDescription8 renderTarget ) {
		AlphaToCoverageEnable = alphaToCoverageEnable ;
		IndependentBlendEnable = independentBlendEnable ;
		RenderTarget = renderTarget ;
	}
	
	public BlendDescription( in D3D12_BLEND_DESC desc ) {
		AlphaToCoverageEnable = desc.AlphaToCoverageEnable ;
		IndependentBlendEnable = desc.IndependentBlendEnable ;
		RenderTarget = desc.RenderTarget ;
	}
	
	public static implicit operator D3D12_BLEND_DESC( in BlendDescription desc ) => new D3D12_BLEND_DESC {
			AlphaToCoverageEnable = desc.AlphaToCoverageEnable,
			IndependentBlendEnable = desc.IndependentBlendEnable,
			RenderTarget = desc.RenderTarget
	} ;
	
	public static implicit operator BlendDescription( in D3D12_BLEND_DESC desc ) => new BlendDescription {
			AlphaToCoverageEnable = desc.AlphaToCoverageEnable,
			IndependentBlendEnable = desc.IndependentBlendEnable,
			RenderTarget = desc.RenderTarget
	} ;
} ;



[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_RENDER_TARGET_BLEND_DESC ) )]
public struct RTBlendDescription {
	/// <summary>
	/// <para>Specifies whether to enable (or disable) blending. Set to <b>TRUE</b> to enable blending. > [!NOTE] > It's not valid for *LogicOpEnable* and *BlendEnable* to both be **TRUE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_target_blend_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL BlendEnable ;

	/// <summary>
	/// <para>Specifies whether to enable (or disable) a logical operation. Set to <b>TRUE</b> to enable a logical operation. > [!NOTE] > It's not valid for *LogicOpEnable* and *BlendEnable* to both be **TRUE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_target_blend_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL LogicOpEnable ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_blend">D3D12_BLEND</a>-typed value that specifies the operation to perform on the RGB value that the pixel shader outputs. The <b>BlendOp</b> member defines how to combine the <b>SrcBlend</b> and <b>DestBlend</b> operations.</summary>
	public Blend SrcBlend ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_blend">D3D12_BLEND</a>-typed value that specifies the operation to perform on the current RGB value in the render target. The <b>BlendOp</b> member defines how to combine the <b>SrcBlend</b> and <b>DestBlend</b> operations.</summary>
	public Blend DestBlend ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_blend_op">D3D12_BLEND_OP</a>-typed value that defines how to combine the <b>SrcBlend</b> and <b>DestBlend</b> operations.</summary>
	public BlendOperation BlendOp ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_blend">D3D12_BLEND</a>-typed value that specifies the operation to perform on the alpha value that the pixel shader outputs. Blend options that end in _COLOR are not allowed. The <b>BlendOpAlpha</b> member defines how to combine the <b>SrcBlendAlpha</b> and <b>DestBlendAlpha</b> operations.</summary>
	public Blend SrcBlendAlpha ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_blend">D3D12_BLEND</a>-typed value that specifies the operation to perform on the current alpha value in the render target. Blend options that end in _COLOR are not allowed. The <b>BlendOpAlpha</b> member defines how to combine the <b>SrcBlendAlpha</b> and <b>DestBlendAlpha</b> operations.</summary>
	public Blend DestBlendAlpha ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_blend_op">D3D12_BLEND_OP</a>-typed value that defines how to combine the <b>SrcBlendAlpha</b> and <b>DestBlendAlpha</b> operations.</summary>
	public BlendOperation BlendOpAlpha ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_logic_op">D3D12_LOGIC_OP</a>-typed value that specifies the logical operation to configure for the render target.</summary>
	public LogicOperation LogicOp ;

	/// <summary>A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_color_write_enable">D3D12_COLOR_WRITE_ENABLE</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies a write mask.</summary>
	public byte RenderTargetWriteMask ;
	
	public RTBlendDescription( BOOL blendEnable, BOOL logicOpEnable, 
							   Blend srcBlend, Blend destBlend, BlendOperation blendOp, 
							   Blend srcBlendAlpha, Blend destBlendAlpha, BlendOperation blendOpAlpha, 
							   LogicOperation logicOp, byte renderTargetWriteMask ) {
		BlendEnable = blendEnable ;
		LogicOpEnable = logicOpEnable ;
		SrcBlend = srcBlend ;
		DestBlend = destBlend ;
		BlendOp = blendOp ;
		SrcBlendAlpha = srcBlendAlpha ;
		DestBlendAlpha = destBlendAlpha ;
		BlendOpAlpha = blendOpAlpha ;
		LogicOp = logicOp ;
		RenderTargetWriteMask = renderTargetWriteMask ;
	}
	
	public RTBlendDescription( in D3D12_RENDER_TARGET_BLEND_DESC desc ) {
		BlendEnable           = desc.BlendEnable ;
		LogicOpEnable         = desc.LogicOpEnable ;
		SrcBlend              = (Blend)desc.SrcBlend ;
		DestBlend             = (Blend)desc.DestBlend ;
		BlendOp               = (BlendOperation)desc.BlendOp ;
		SrcBlendAlpha         = (Blend)desc.SrcBlendAlpha ;
		DestBlendAlpha        = (Blend)desc.DestBlendAlpha ;
		BlendOpAlpha          = (BlendOperation)desc.BlendOpAlpha ;
		LogicOp               = (LogicOperation)desc.LogicOp ;
		RenderTargetWriteMask = desc.RenderTargetWriteMask ;
	}
	
	public static implicit operator D3D12_RENDER_TARGET_BLEND_DESC( in RTBlendDescription desc ) => new D3D12_RENDER_TARGET_BLEND_DESC {
			BlendEnable           = desc.BlendEnable,
			LogicOpEnable         = desc.LogicOpEnable,
			SrcBlend              = ( D3D12_BLEND )desc.SrcBlend,
			DestBlend             = ( D3D12_BLEND )desc.DestBlend,
			BlendOp               = ( D3D12_BLEND_OP )desc.BlendOp,
			SrcBlendAlpha         = ( D3D12_BLEND )desc.SrcBlendAlpha,
			DestBlendAlpha        = ( D3D12_BLEND )desc.DestBlendAlpha,
			BlendOpAlpha          = ( D3D12_BLEND_OP )desc.BlendOpAlpha,
			LogicOp               = ( D3D12_LOGIC_OP )desc.LogicOp,
			RenderTargetWriteMask = desc.RenderTargetWriteMask
	} ;
	public static implicit operator RTBlendDescription( in D3D12_RENDER_TARGET_BLEND_DESC desc ) => new( desc ) ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( __D3D12_RENDER_TARGET_BLEND_DESC_8 ) )]
public struct RTBlendDescription8 {
	const int SpanLength = 8 ;
	
	/// <summary>The length of the inline array.</summary>
	public readonly int Length => SpanLength ;

	public RTBlendDescription _0, _1, _2, _3, _4, _5, _6, _7 ;

	/// <summary>
	/// Gets this inline array as a span.
	/// </summary>
	/// <remarks>
	/// ⚠ Important ⚠: When this struct is on the stack, do not let the returned span outlive the stack frame that defines it.
	/// </remarks>
	[UnscopedRef]
	public Span< RTBlendDescription > AsSpan( ) =>
		MemoryMarshal.CreateSpan( ref _0, SpanLength ) ;

	/// <summary>
	/// Gets this inline array as a span.
	/// </summary>
	/// <remarks>
	/// ⚠ Important ⚠: When this struct is on the stack, do not let the returned span outlive the stack frame that defines it.
	/// </remarks>
	[UnscopedRef]
	public readonly ReadOnlySpan< RTBlendDescription > AsReadOnlySpan() =>
		MemoryMarshal.CreateReadOnlySpan( ref Unsafe.AsRef( _0 ), SpanLength ) ;

	public static implicit operator RTBlendDescription8( ReadOnlySpan< RTBlendDescription > value ) {
		Unsafe.SkipInit( out RTBlendDescription8 result ) ;
		value.CopyTo( result.AsSpan( ) ) ;
		int initLength = value.Length ;
		result.AsSpan( ).Slice( initLength, SpanLength - initLength ).Clear( ) ;
		return result ;
	}
	
	public static implicit operator RTBlendDescription8( Span< RTBlendDescription > value ) {
		Unsafe.SkipInit( out RTBlendDescription8 result ) ;
		value.CopyTo( result.AsSpan( ) ) ;
		int initLength = value.Length ;
		result.AsSpan( ).Slice( initLength, SpanLength - initLength ).Clear( ) ;
		return result ;
	}
	
	public static implicit operator RTBlendDescription8( in __D3D12_RENDER_TARGET_BLEND_DESC_8 value ) {
		Unsafe.SkipInit( out RTBlendDescription8 result ) ;
		result._0 = value._0 ;
		result._1 = value._1 ;
		result._2 = value._2 ;
		result._3 = value._3 ;
		result._4 = value._4 ;
		result._5 = value._5 ;
		result._6 = value._6 ;
		result._7 = value._7 ;
		return result ;
	}
	
	public static implicit operator __D3D12_RENDER_TARGET_BLEND_DESC_8( in RTBlendDescription8 value ) =>
		new __D3D12_RENDER_TARGET_BLEND_DESC_8 {
			_0 = value._0,
			_1 = value._1,
			_2 = value._2,
			_3 = value._3,
			_4 = value._4,
			_5 = value._5,
			_6 = value._6,
			_7 = value._7
		} ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_PLACED_SUBRESOURCE_FOOTPRINT ) )]
public struct PlacedSubresourceFootprint {
	/// <summary>
	/// <para>The offset of the subresource within the parent resource, in bytes. The offset between the start of the parent resource and this subresource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ulong Offset ;

	/// <summary>
	/// <para>The format, width, height, depth, and row-pitch of the subresource, as a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_subresource_footprint">D3D12_SUBRESOURCE_FOOTPRINT</a> structure.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public SubresourceFootprint Footprint ;
	
	
	public PlacedSubresourceFootprint( ulong offset, 
									   Format format, 
									   uint width, uint height, 
									   uint depth, uint rowPitch ) {
		Offset = offset ;
		Footprint = new( format, width, height, depth, rowPitch ) ;
	}
	
	public PlacedSubresourceFootprint( ulong offset, in SubresourceFootprint footprint ) {
		Offset = offset ;
		Footprint = footprint ;
	}
	
	public PlacedSubresourceFootprint( in D3D12_PLACED_SUBRESOURCE_FOOTPRINT desc ) {
		Offset = desc.Offset ;
		Footprint = desc.Footprint ;
	}
	
	
	public static implicit operator D3D12_PLACED_SUBRESOURCE_FOOTPRINT( in PlacedSubresourceFootprint desc ) => new D3D12_PLACED_SUBRESOURCE_FOOTPRINT {
			Offset    = desc.Offset,
			Footprint = desc.Footprint
	} ;
	
	public static implicit operator PlacedSubresourceFootprint( in D3D12_PLACED_SUBRESOURCE_FOOTPRINT desc ) => new( desc ) ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_SUBRESOURCE_FOOTPRINT ) )]
public struct SubresourceFootprint {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value that  specifies the viewing format.</summary>
	public Format Format ;
	/// <summary>The width of the subresource.</summary>
	public uint Width ;
	/// <summary>The height of the subresource.</summary>
	public uint Height ;
	/// <summary>The depth of the subresource.</summary>
	public uint Depth ;

	/// <summary>
	/// <para>The row pitch, or width, or physical size, in bytes, of the subresource data. This must be a multiple of D3D12_TEXTURE_DATA_PITCH_ALIGNMENT (256), and must be greater than or equal to the size of the data within a row.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_subresource_footprint#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint RowPitch ;
	
	public SubresourceFootprint( Format format,
								 uint width, uint height, 
								 uint depth, uint rowPitch ) {
		Format = format ;
		Width = width ;
		Height = height ;
		Depth = depth ;
		RowPitch = rowPitch ;
	}
	
	public SubresourceFootprint( in D3D12_SUBRESOURCE_FOOTPRINT desc ) {
		Format = (Format)desc.Format ;
		Width  = desc.Width ; Height = desc.Height ;
		Depth  = desc.Depth ; RowPitch = desc.RowPitch ;
	}
	
	public static implicit operator D3D12_SUBRESOURCE_FOOTPRINT( in SubresourceFootprint desc ) => new D3D12_SUBRESOURCE_FOOTPRINT {
			Format   = ( DXGI_FORMAT )desc.Format,
			Width    = desc.Width,
			Height   = desc.Height,
			Depth    = desc.Depth,
			RowPitch = desc.RowPitch
	} ;
	public static implicit operator SubresourceFootprint( in D3D12_SUBRESOURCE_FOOTPRINT desc ) => new( desc ) ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_QUERY_HEAP_DESC ) )]
public struct QueryHeapDescription {
	/// <summary>Specifies one member of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_heap_type">D3D12_QUERY_HEAP_TYPE</a>.</summary>
	public QueryHeapType Type ;

	/// <summary>Specifies the number of queries the heap should contain.</summary>
	public uint Count ;

	/// <summary>
	/// <para>For single GPU operation, set this to zero. If there are multiple GPU nodes, set a bit to identify the node (the  device's physical adapter) to which the query heap applies. Each bit in the mask corresponds to a single node. Only 1 bit must be set. Refer to <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_query_heap_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NodeMask ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_COMMAND_SIGNATURE_DESC ) )]
public struct CommandSignatureDescription {
	/// <summary>Specifies the size of each command in the drawing buffer, in bytes.</summary>
	public uint ByteStride ;
	/// <summary>Specifies the number of arguments in the command signature.</summary>
	public uint NumArgumentDescs ;
	
	/// <summary>
	/// <para>An array of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_indirect_argument_desc">D3D12_INDIRECT_ARGUMENT_DESC</a> structures, containing details of the arguments, including whether the argument is a vertex buffer, constant, constant buffer view, shader resource view, or unordered access view.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_command_signature_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public unsafe D3D12_INDIRECT_ARGUMENT_DESC* pArgumentDescs ;

	/// <summary>
	/// <para>For single GPU operation, set this to zero. If there are multiple GPU nodes, set bits to identify the nodes (the  device's physical adapters) for which the command signature is to apply. Each bit in the mask corresponds to a single node. Refer to <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_command_signature_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NodeMask ;
} ;


/// <summary>
/// Used with the <see cref="CommandSignatureDescription"/> structure
/// (<a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/ns-d3d12-d3d12_indirect_argument_desc">D3D12_COMMAND_SIGNATURE_DESC</a>).
/// </summary>
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_INDIRECT_ARGUMENT_DESC ) )]
public struct IndirectArgumentDescription {
	/// <summary>A single <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_indirect_argument_type">D3D12_INDIRECT_ARGUMENT_TYPE</a> enumeration constant.</summary>
	public IndirectArgumentType Type ;

	public _anon_argdesc_union Anonymous ;
	[StructLayout( LayoutKind.Explicit )]
	public partial struct _anon_argdesc_union {
		[FieldOffset( 0 )] public _vertexBufferUnion_ VertexBuffer ;
		[FieldOffset( 0 )] public _constantDataUnion_ Constant ;
		[FieldOffset( 0 )] public _constantBufferViewUnion_ ConstantBufferView ;
		[FieldOffset( 0 )] public _shaderResourceViewUnion_ ShaderResourceView ;
		[FieldOffset( 0 )] public _unorderedAccessViewUnion_ UnorderedAccessView ;
		
		public partial struct _vertexBufferUnion_ { public uint Slot ; } ;
		public partial struct _constantDataUnion_ { public uint RootParameterIndex, DestOffsetIn32BitValues, Num32BitValuesToSet ; } ;
		public partial struct _constantBufferViewUnion_ { public uint RootParameterIndex ; } ;
		public partial struct _shaderResourceViewUnion_ { public uint RootParameterIndex ; } ;
		public partial struct _unorderedAccessViewUnion_ { public uint RootParameterIndex ; } ;
	} ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_STREAM_OUTPUT_DESC ) )]
public struct StreamOutputDescription {
	/// <summary>
	/// An array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_so_declaration_entry">D3D12_SO_DECLARATION_ENTRY</a>
	/// structures. Can't be <b>NULL</b> if <b>NumEntries</b> &gt; 0.
	/// </summary>
	public unsafe SODeclarationEntry* pSODeclaration ;
	public unsafe Span< SODeclarationEntry > SODeclarations => new( pSODeclaration, ( int )NumEntries ) ;
	
	/// <summary>The number of entries in the stream output declaration array that the <b>pSODeclaration</b> member points to.</summary>
	public uint NumEntries ;

	/// <summary>An array of buffer strides; each stride is the size of an element for that buffer.</summary>
	public unsafe uint* pBufferStrides ;

	/// <summary>The number of strides (or buffers) that the <b>pBufferStrides</b> member points to.</summary>
	public uint NumStrides ;

	/// <summary>The index number of the stream to be sent to the rasterizer stage.</summary>
	public uint RasterizedStream ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_STREAM_OUTPUT_BUFFER_VIEW ) )]
public struct StreamOutputBufferView {
	/// <summary>
	/// <para>A D3D12_GPU_VIRTUAL_ADDRESS (a UINT64) that points to the stream output buffer.
	/// If <b>SizeInBytes</b> is 0, this member isn't used and can be any value.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_stream_output_buffer_view#members">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong BufferLocation ;

	/// <summary>The size of the stream output buffer in bytes.</summary>
	public ulong SizeInBytes ;

	/// <summary>
	/// <para>The location of the value of how much data has been filled into the buffer, as a D3D12_GPU_VIRTUAL_ADDRESS (a UINT64). This member can't be NULL; a filled size location must be supplied (which the hardware will increment as data is output). If <b>SizeInBytes</b> is 0, this member isn't used and can be any value.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_stream_output_buffer_view#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong BufferFilledSizeLocation ;
	
	public StreamOutputBufferView( ulong bufferLocation, ulong sizeInBytes, ulong bufferFilledSizeLocation ) {
		BufferLocation = bufferLocation ;
		SizeInBytes = sizeInBytes ;
		BufferFilledSizeLocation = bufferFilledSizeLocation ;
	}
	public StreamOutputBufferView( in D3D12_STREAM_OUTPUT_BUFFER_VIEW view ) {
		BufferLocation = view.BufferLocation ;
		SizeInBytes = view.SizeInBytes ;
		BufferFilledSizeLocation = view.BufferFilledSizeLocation ;
	}
	
	public static implicit operator D3D12_STREAM_OUTPUT_BUFFER_VIEW( in StreamOutputBufferView view ) => new D3D12_STREAM_OUTPUT_BUFFER_VIEW {
			BufferLocation = view.BufferLocation,
			SizeInBytes = view.SizeInBytes,
			BufferFilledSizeLocation = view.BufferFilledSizeLocation
	} ;
	public static implicit operator StreamOutputBufferView( in D3D12_STREAM_OUTPUT_BUFFER_VIEW view ) => new( view ) ;
} ;