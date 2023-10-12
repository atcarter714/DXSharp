#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Direct3D12 ;


// ------------------------------------------------------------------------------
// Enumerated Constants ::
// ------------------------------------------------------------------------------

[Equivalent( typeof( D3D12_UAV_DIMENSION ) )]
public enum UAVDimensions {
	/// <summary>The view type is unknown.</summary>
	UNKNOWN = 0,
	/// <summary>View the resource as a buffer.</summary>
	BUFFER = 1,
	/// <summary>View the resource as a 1D texture.</summary>
	TEXTURE1D = 2,
	/// <summary>View the resource as a 1D texture array.</summary>
	TEXTURE1DARRAY = 3,
	/// <summary>View the resource as a 2D texture.</summary>
	TEXTURE2D = 4,
	/// <summary>View the resource as a 2D texture array.</summary>
	TEXTURE2DARRAY   = 5,
	TEXTURE2DMS      = 6,
	TEXTURE2DMSARRAY = 7,
	/// <summary>View the resource as a 3D texture array.</summary>
	TEXTURE3D = 8,
} ;

[Flags, Equivalent( typeof( D3D12_BUFFER_UAV_FLAGS ) )]
public enum BufferUAVFlags {
	/// <summary>Indicates a default view.</summary>
	NONE = 0x00000000,
	/// <summary>
	/// <para>Resource contains raw, unstructured data.  Requires the UAV format to be <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT_R32_TYPELESS</a>. For more info about raw viewing of buffers, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-resources-intro">Raw Views of Buffers</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_buffer_uav_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	RAW = 0x00000001,
} ;


// ------------------------------------------------------------------------------
// D3D12_UNORDERED_ACCESS_VIEW_DESC ::
// ------------------------------------------------------------------------------

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_UNORDERED_ACCESS_VIEW_DESC ) )]
public struct UnorderedAccessViewDescription {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value that specifies the viewing format.</summary>
	public Format Format ;
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_uav_dimension">D3D12_UAV_DIMENSION</a>-typed value that specifies the resource type of the view. This type specifies how the resource will be accessed. This member also determines which _UAV to use in the union below.</summary>
	public UAVDimensions ViewDimension ;

	public _anon_uav_union ReadAs ;
	[StructLayout( LayoutKind.Explicit )]
	public partial struct _anon_uav_union {
		[FieldOffset( 0 )] public BufferUAV Buffer ;
		[FieldOffset( 0 )] public Tex1DUAV Texture1D ;
		[FieldOffset( 0 )] public Tex1DArrayUAV Texture1DArray ;

		[FieldOffset( 0 )] public Tex2DUAV Texture2D ;
		[FieldOffset( 0 )] public Tex2DArrayUAV Texture2DArray ;

		[FieldOffset( 0 )] public Tex2DMSUAV Texture2DMS ;
		[FieldOffset( 0 )] public Tex2DMSArrayUAV Texture2DMSArray ;

		[FieldOffset( 0 )] public Tex3DUAV Texture3D ;
	} ;
} ;



[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_BUFFER_UAV ) )]
public struct BufferUAV {
	/// <summary>The zero-based index of the first element to be accessed.</summary>
	public ulong FirstElement ;
	/// <summary>The number of elements in the resource. For structured buffers, this is the number of structures in the buffer.</summary>
	public uint NumElements ;
	/// <summary>The size of each element in the buffer structure (in bytes) when the buffer represents a structured buffer.</summary>
	public uint StructureByteStride ;
	/// <summary>The counter offset, in bytes.</summary>
	public ulong CounterOffsetInBytes ;
	
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_buffer_uav_flags">D3D12_BUFFER_UAV_FLAGS</a>-typed value that specifies the view options for the resource.</summary>
	public BufferUAVFlags Flags ;
} ;



// --------------------------------------------------
// Texture UAVs ::
// --------------------------------------------------

// D3D12_TEX1D_UAV 
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX1D_UAV ) )]
public struct Tex1DUAV {
	/// <summary>The mipmap slice index.</summary>
	public uint MipSlice;
} ;

// D3D12_TEX1D_ARRAY_UAV 
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX1D_ARRAY_UAV ) )]
public struct Tex1DArrayUAV {
	/// <summary>The mipmap slice index.</summary>
	public uint MipSlice ;
	/// <summary>The zero-based index of the first array slice to be accessed.</summary>
	public uint FirstArraySlice ;
	/// <summary>The number of slices in the array.</summary>
	public uint ArraySize ;
} ;

// D3D12_TEX2D_UAV 
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_UAV ) )]
public struct Tex2DUAV {
	/// <summary>The mipmap slice index.</summary>
	public uint MipSlice ;
	/// <summary>The index (plane slice number) of the plane to use in the texture.</summary>
	public uint PlaneSlice ;
} ;

// D3D12_TEX2D_ARRAY_UAV 
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_ARRAY_UAV ) )]
public struct Tex2DArrayUAV {
	/// <summary>The mipmap slice index.</summary>
	public uint MipSlice;
	/// <summary>The zero-based index of the first array slice to be accessed.</summary>
	public uint FirstArraySlice;
	/// <summary>The number of slices in the array.</summary>
	public uint ArraySize;
	/// <summary>The index (plane slice number) of the plane to use in an array of textures.</summary>
	public uint PlaneSlice;
} ;

// D3D12_TEX2DMS_UAV 
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2DMS_UAV ) )]
public struct Tex2DMSUAV { public uint UnusedField_NothingToDefine ; } ;

// D3D12_TEX2DMS_ARRAY_UAV 
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2DMS_ARRAY_UAV ) )]
public struct Tex2DMSArrayUAV {
	public uint FirstArraySlice;
	public uint ArraySize;
} ;

// D3D12_TEX3D_UAV 
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX3D_UAV ) )]
public struct Tex3DUAV {
	/// <summary>The mipmap slice index.</summary>
	public uint MipSlice ;
	/// <summary>The zero-based index of the first depth slice to be accessed.</summary>
	public uint FirstWSlice ;
	/// <summary>
	/// <para>The number of depth slices. Set to -1 to indicate all the depth slices from <b>FirstWSlice</b> to the last slice.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_tex3d_uav#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint WSize ;
} ;


// ------------------------------------------------------------------------------
// ------------------------------------------------------------------------------