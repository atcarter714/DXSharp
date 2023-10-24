#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Direct3D12 ;


// ------------------------------
// D3D12_RENDER_TARGET_VIEW_DESC:
// ------------------------------

/// <summary>Describes the subresources from a resource that are accessible by using a render-target view.</summary>
/// <remarks>
/// <para>Pass a render-target-view description into <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createrendertargetview">ID3D12Device::CreateRenderTargetView</a> to create a render-target view. A render-target view can't use the following formats: </para>
/// <para>This doc was truncated.</para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_target_view_desc#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_RENDER_TARGET_VIEW_DESC ) )]
public struct RenderTargetViewDescription {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value that specifies the viewing format.</summary>
	public Format Format ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_rtv_dimension">D3D12_RTV_DIMENSION</a>-typed value that specifies how the render-target resource will be accessed. This type specifies how the resource will be accessed. This member also determines which _RTV to use in the following union.</summary>
	public RTVDimension ViewDimension ;

	/// <summary>Reads the <see cref="RenderTargetViewDescription"/> data as a union.</summary>
	public _anon_rtv_union ReadAs ;

	[StructLayout( LayoutKind.Explicit )]
	public partial struct _anon_rtv_union {
		[FieldOffset( 0 )] public BufferRTV       Buffer ;
		[FieldOffset( 0 )] public Tex1DRTV        Tex1D ;
		[FieldOffset( 0 )] public Tex1DArrayRTV   Texture1DArray ;
		[FieldOffset( 0 )] public Tex2DRTV        Texture2D ;
		[FieldOffset( 0 )] public Tex2DArrayRTV   Texture2DArray ;
		[FieldOffset( 0 )] public Tex2DMSRTV      Texture2DMS ;
		[FieldOffset( 0 )] public Tex2DMSArrayRTV Texture2DMSArray ;
		[FieldOffset( 0 )] public Tex3DRTV        Texture3D ;
	} ;
	
	
	// ----------------------------------------------------
	// Constructors:
	// ----------------------------------------------------
	
	public RenderTargetViewDescription( Format format, BufferRTV buffer = default ) {
		Format        = format ;
		ViewDimension = RTVDimension.Buffer ;
		ReadAs        = default ; ReadAs.Buffer = buffer ;
	}
	
	public RenderTargetViewDescription( Format format, Tex1DRTV tex1D ) {
		Format        = format ;
		ViewDimension = RTVDimension.Texture1D ;
		ReadAs        = default ; ReadAs.Tex1D = tex1D ;
	}
	
	public RenderTargetViewDescription( Format format, Tex1DArrayRTV tex1DArray ) {
		Format        = format ;
		ViewDimension = RTVDimension.Texture1DArray ;
		ReadAs        = default ; ReadAs.Texture1DArray = tex1DArray ;
	}
	
	public RenderTargetViewDescription( Format format, Tex2DRTV tex2D ) {
		Format        = format ;
		ViewDimension = RTVDimension.Texture2D ;
		ReadAs        = default ; ReadAs.Texture2D = tex2D ;
	}
	
	public RenderTargetViewDescription( Format format, Tex2DArrayRTV tex2DArray ) {
		Format        = format ;
		ViewDimension = RTVDimension.Texture2DArray ;
		ReadAs        = default ; ReadAs.Texture2DArray = tex2DArray ;
	}
	
	public RenderTargetViewDescription( Format format, Tex2DMSRTV tex2DMS ) {
		Format        = format ;
		ViewDimension = RTVDimension.Texture2DMS ;
		ReadAs        = default ; ReadAs.Texture2DMS = tex2DMS ;
	}
	
	public RenderTargetViewDescription( Format format, Tex2DMSArrayRTV tex2DMSArray ) {
		Format        = format ;
		ViewDimension = RTVDimension.Texture2DMSArray ;
		ReadAs        = default ; ReadAs.Texture2DMSArray = tex2DMSArray ;
	}
	
	public RenderTargetViewDescription( Format format, Tex3DRTV tex3D ) {
		Format        = format ;
		ViewDimension = RTVDimension.Texture3D ;
		ReadAs        = default ; ReadAs.Texture3D = tex3D ;
	}
	
} ;



// ------------------------------
// RTV Descriptors:
// ------------------------------


[StructLayout(LayoutKind.Sequential),
	ProxyFor(typeof(D3D12_BUFFER_RTV))]
public struct BufferRTV {
	/// <summary>Number of elements between the beginning of the buffer and the first element to access.</summary>
	public ulong FirstElement ;
	/// <summary>The total number of elements in the view.</summary>
	public uint NumElements ;
	
	
	/// <summary>Creates a new <see cref="BufferRTV"/> instance.</summary>
	/// <param name="firstElement">Number of elements between the beginning of the buffer and the first element to access.</param>
	/// <param name="numElements">The total number of elements in the view.</param>
	public BufferRTV( ulong firstElement = 0, uint numElements = 1 ) {
		FirstElement = firstElement ;
		NumElements  = numElements ;
	}
} ;


[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_TEX1D_RTV))]
public struct Tex1DRTV {
	/// <summary>The index of the mipmap level to use mip slice.</summary>
	public uint MipSlice ;
	
	/// <summary>Creates a new <see cref="Tex1DRTV"/> instance.</summary>
	/// <param name="mipSlice">The index of the mipmap level to use mip slice.</param>
	public Tex1DRTV( uint mipSlice = 0 ) => MipSlice = mipSlice ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX1D_ARRAY_RTV ) )]
public struct Tex1DArrayRTV {
	/// <summary>The index of the mipmap level to use mip slice.</summary>
	public uint MipSlice ;
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice ;
	/// <summary>Number of textures to use.</summary>
	public uint ArraySize ;
	
	/// <summary>Creates a new <see cref="Tex1DArrayRTV"/> instance.</summary>
	/// <param name="mipSlice">The index of the mipmap level to use mip slice.</param>
	/// <param name="firstArraySlice">The index of the first texture to use in an array of textures.</param>
	/// <param name="arraySize">Number of textures to use.</param>
	public Tex1DArrayRTV( uint mipSlice = 0, uint firstArraySlice = 0, uint arraySize = 1 ) {
		MipSlice        = mipSlice ;
		FirstArraySlice = firstArraySlice ;
		ArraySize       = arraySize ;
	}
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_RTV ) )]
public struct Tex2DRTV {
	/// <summary>The index of the mipmap level to use.</summary>
	public uint MipSlice ;
	/// <summary>The index (plane slice number) of the plane to use in the texture.</summary>
	public uint PlaneSlice ;
	
	/// <summary>Creates a new <see cref="Tex2DRTV"/> instance.</summary>
	/// <param name="mipSlice">The index of the mipmap level to use.</param>
	/// <param name="planeSlice">The index (plane slice number) of the plane to use in the texture.</param>
	public Tex2DRTV( uint mipSlice = 0, uint planeSlice = 0 ) {
		MipSlice = mipSlice ; PlaneSlice = planeSlice ;
	}
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_ARRAY_RTV ) )]
public struct Tex2DArrayRTV {
	/// <summary>The index of the mipmap level to use mip slice.</summary>
	public uint MipSlice ;
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice ;
	/// <summary>Number of textures in the array to use in the render target view, starting from <b>FirstArraySlice</b>.</summary>
	public uint ArraySize ;
	/// <summary>The index (plane slice number) of the plane to use in an array of textures.</summary>
	public uint PlaneSlice ;
	
	/// <summary>Creates a new <see cref="Tex2DArrayRTV"/> instance.</summary>
	/// <param name="mipSlice">The index of the mipmap level to use mip slice.</param>
	/// <param name="firstArraySlice">The index of the first texture to use in an array of textures.</param>
	/// <param name="arraySize">Number of textures in the array to use in the render target view, starting from <b>FirstArraySlice</b>.</param>
	/// <param name="planeSlice">The index (plane slice number) of the plane to use in an array of textures.</param>
	public Tex2DArrayRTV( uint mipSlice = 0, uint firstArraySlice = 0, uint arraySize = 1, uint planeSlice = 0 ) {
		MipSlice        = mipSlice ;
		FirstArraySlice = firstArraySlice ;
		ArraySize       = arraySize ;
		PlaneSlice      = planeSlice ;
	}
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2DMS_RTV ) )]
public struct Tex2DMSRTV {
	/// <summary>Integer of any value. See remarks.</summary>
	public uint UnusedField_NothingToDefine ;
	
	/// <summary>Creates a new <see cref="Tex2DMSRTV"/> instance.</summary>
	/// <param name="unusedField_NothingToDefine">Integer of any value. See remarks.</param>
	public Tex2DMSRTV( uint unusedField_NothingToDefine = 0 ) => 
		UnusedField_NothingToDefine = unusedField_NothingToDefine ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2DMS_ARRAY_RTV ) )]
public struct Tex2DMSArrayRTV {
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice ;
	/// <summary>The number of textures to use.</summary>
	public uint ArraySize ;
	
	/// <summary>Creates a new <see cref="Tex2DMSArrayRTV"/> instance.</summary>
	/// <param name="firstArraySlice">The index of the first texture to use in an array of textures.</param>
	/// <param name="arraySize">The number of textures to use.</param>
	public Tex2DMSArrayRTV( uint firstArraySlice = 0, uint arraySize = 1 ) {
		FirstArraySlice = firstArraySlice ;
		ArraySize       = arraySize ;
	}
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX3D_RTV ) )]
public struct Tex3DRTV {
	/// <summary>The index of the mipmap level to use mip slice.</summary>
	public uint MipSlice ;
	
	/// <summary>First depth level to use.</summary>
	public uint FirstWSlice ;
	
	/// <summary>
	/// Number of depth levels to use in the render-target view, starting from <b>FirstWSlice</b>.
	/// A value of -1 indicates all of the slices along the w axis, starting from <b>FirstWSlice</b>.
	/// </summary>
	public uint WSize ;
	
	
	/// <summary>Creates a new <see cref="Tex3DRTV"/> instance.</summary>
	/// <param name="mipSlice">The index of the mipmap level to use mip slice.</param>
	/// <param name="firstWSlice">First depth level to use.</param>
	/// <param name="wSize">
	/// Number of depth levels to use in the render-target view, starting from <b>FirstWSlice</b>.
	/// A value of <see cref="uint.MaxValue"/> indicates all of the slices along the w axis, starting from <b>FirstWSlice</b>.
	/// </param>
	public Tex3DRTV( uint mipSlice = 0, uint firstWSlice = 0, uint wSize = uint.MaxValue ) {
		MipSlice    = mipSlice ;
		FirstWSlice = firstWSlice ;
		WSize       = wSize ;
	}
} ;

