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
public struct RenderTargetViewDesc
{
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value that specifies the viewing format.</summary>
	public Format Format ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_rtv_dimension">D3D12_RTV_DIMENSION</a>-typed value that specifies how the render-target resource will be accessed. This type specifies how the resource will be accessed. This member also determines which _RTV to use in the following union.</summary>
	public RTVDimension ViewDimension ;

	/// <summary>Reads the <see cref="RenderTargetViewDesc"/> data as a union.</summary>
	public _anon_rtv_union ReadAs ;

	[StructLayout( LayoutKind.Explicit )]
	public partial struct _anon_rtv_union
	{
		[FieldOffset( 0 )] public BufferRTV       Buffer ;
		[FieldOffset( 0 )] public Tex1DRTV        Tex1D ;
		[FieldOffset( 0 )] public Tex1DArrayRTV   Texture1DArray ;
		[FieldOffset( 0 )] public Tex2DRTV        Texture2D ;
		[FieldOffset( 0 )] public Tex2DArrayRTV   Texture2DArray ;
		[FieldOffset( 0 )] public Tex2DMSRTV      Texture2DMS ;
		[FieldOffset( 0 )] public Tex2DMSArrayRTV Texture2DMSArray ;
		[FieldOffset( 0 )] public Tex3DRTV        Texture3D ;
	} ;


	public static implicit operator D3D12_RENDER_TARGET_VIEW_DESC( in RenderTargetViewDesc o ) {
		unsafe {
			fixed ( RenderTargetViewDesc* ptr = &o ) {
				return *(D3D12_RENDER_TARGET_VIEW_DESC*)ptr ;
			}
		}
	}

	public static implicit operator RenderTargetViewDesc( in D3D12_RENDER_TARGET_VIEW_DESC o ) {
		unsafe {
			fixed ( D3D12_RENDER_TARGET_VIEW_DESC* ptr = &o ) {
				return *(RenderTargetViewDesc*)ptr ;
			}
		}
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
} ;

[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_TEX1D_RTV))]
public struct Tex1DRTV {
	/// <summary>The index of the mipmap level to use mip slice.</summary>
	public uint MipSlice ;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX1D_ARRAY_RTV ) )]
public struct Tex1DArrayRTV {
	/// <summary>The index of the mipmap level to use mip slice.</summary>
	public uint MipSlice;
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice;
	/// <summary>Number of textures to use.</summary>
	public uint ArraySize;
} ;

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_RTV ) )]
public struct Tex2DRTV {
	/// <summary>The index of the mipmap level to use.</summary>
	public uint MipSlice;
	/// <summary>The index (plane slice number) of the plane to use in the texture.</summary>
	public uint PlaneSlice;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_ARRAY_RTV ) )]
public struct Tex2DArrayRTV {
	/// <summary>The index of the mipmap level to use mip slice.</summary>
	public uint MipSlice;
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice;
	/// <summary>Number of textures in the array to use in the render target view, starting from <b>FirstArraySlice</b>.</summary>
	public uint ArraySize;
	/// <summary>The index (plane slice number) of the plane to use in an array of textures.</summary>
	public uint PlaneSlice;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2DMS_RTV ) )]
public struct Tex2DMSRTV {
	/// <summary>Integer of any value. See remarks.</summary>
	public uint UnusedField_NothingToDefine;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2DMS_ARRAY_RTV ) )]
public struct Tex2DMSArrayRTV {
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice;
	/// <summary>The number of textures to use.</summary>
	public uint ArraySize;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX3D_RTV ) )]
public struct Tex3DRTV {
	/// <summary>The index of the mipmap level to use mip slice.</summary>
	public uint MipSlice;
	/// <summary>First depth level to use.</summary>
	public uint FirstWSlice;
	/// <summary>Number of depth levels to use in the render-target view, starting from <b>FirstWSlice</b>. A value of -1 indicates all of the slices along the w axis, starting from <b>FirstWSlice</b>.</summary>
	public uint WSize;
} ;

// ------------------------------

/*
[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( xxx ) )]
public struct xxxxxxxxxxxxxxx {
	
} ;*/
