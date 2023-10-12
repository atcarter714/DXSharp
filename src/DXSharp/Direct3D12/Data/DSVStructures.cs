#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Direct3D12 ;


[StructLayout(LayoutKind.Sequential),
	ProxyFor( typeof( D3D12_DEPTH_STENCIL_VIEW_DESC ) )]
public struct DepthStencilViewDesc {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value that specifies the viewing format.  For allowable formats, see Remarks.</summary>
	public Format Format ;
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_dsv_dimension">D3D12_DSV_DIMENSION</a>-typed value that specifies how the depth-stencil resource will be accessed. This member also determines which _DSV to use in the following union.</summary>
	public DSVDimension ViewDimension ;

	/// <summary>
	/// <para>A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_dsv_flags">D3D12_DSV_FLAGS</a> enumeration constants that are combined by using a bitwise OR operation. The resulting value specifies whether the texture is read only. Pass 0 to specify that it isn't read only; otherwise, pass one or more of the members of the <b>D3D12_DSV_FLAGS</b> enumerated type.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_view_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public DSVFlags Flags ;

	public _anon_dsv_union ReadAs ;
	[StructLayout( LayoutKind.Explicit )]
	public partial struct _anon_dsv_union {
		[FieldOffset( 0 )] public Tex1DDSV        Texture1D ;
		[FieldOffset( 0 )] public Tex1DArrayDSV   Texture1DArray ;
		[FieldOffset( 0 )] public Tex2DDSV        Texture2D ;
		[FieldOffset( 0 )] public Tex2DArrayDSV   Texture2DArray ;
		[FieldOffset( 0 )] public Tex2DMSDSV      Texture2DMS ;
		[FieldOffset( 0 )] public Tex2DMSArrayDSV Texture2DMSArray ;
	} ;
} ;


// --- Tex1D ---

[StructLayout(LayoutKind.Sequential),
	ProxyFor( typeof( D3D12_TEX1D_DSV ) )]
public struct Tex1DDSV {
	/// <summary>The index of the first mipmap level to use.</summary>
	public uint MipSlice ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX1D_ARRAY_DSV ) )]
public struct Tex1DArrayDSV {
	/// <summary>The index of the first mipmap level to use.</summary>
	public uint MipSlice;
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice;
	/// <summary>Number of textures to use.</summary>
	public uint ArraySize;
} ;

// --- Tex2D ---

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_DSV ) )]
public struct Tex2DDSV {
	/// <summary>The index of the first mipmap level to use.</summary>
	public uint MipSlice ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_ARRAY_DSV ) )]
public struct Tex2DArrayDSV {
	/// <summary>The index of the first mipmap level to use.</summary>
	public uint MipSlice ;
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice ;
	/// <summary>Number of textures to use.</summary>
	public uint ArraySize ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2DMS_DSV ) )]
public struct Tex2DMSDSV {
	/// <summary>Unused.</summary>
	public uint UnusedField_NothingToDefine;
} ;

[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_TEX2DMS_ARRAY_DSV))]
public struct Tex2DMSArrayDSV {
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice ;
	/// <summary>Number of textures to use.</summary>
	public uint ArraySize ;
} ;