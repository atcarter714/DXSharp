#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Direct3D12 ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_DEPTH_STENCIL_DESC ) )]
public struct DepthStencilDesc {
	
	/// <summary>Specifies whether to enable depth testing. Set this member to <b>TRUE</b> to enable depth testing.</summary>
	public BOOL DepthEnable ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_depth_write_mask">D3D12_DEPTH_WRITE_MASK</a>-typed value that identifies a portion of the depth-stencil buffer that can be modified by depth data.</summary>
	public DepthWriteMask DepthWriteMask ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_comparison_func">D3D12_COMPARISON_FUNC</a>-typed value that identifies a function that compares depth data against existing depth data.</summary>
	public ComparisonFunction DepthFunc ;

	/// <summary>Specifies whether to enable stencil testing. Set this member to <b>TRUE</b> to enable stencil testing.</summary>
	public BOOL StencilEnable ;

	/// <summary>Identify a portion of the depth-stencil buffer for reading stencil data.</summary>
	public byte StencilReadMask ;

	/// <summary>Identify a portion of the depth-stencil buffer for writing stencil data.</summary>
	public byte StencilWriteMask ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_depth_stencilop_desc">D3D12_DEPTH_STENCILOP_DESC</a> structure that describes how to use the results of the depth test and the stencil test for pixels whose surface normal is facing towards the camera.</summary>
	public DepthStencilOpDesc FrontFace ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_depth_stencilop_desc">D3D12_DEPTH_STENCILOP_DESC</a> structure that describes how to use the results of the depth test and the stencil test for pixels whose surface normal is facing away from the camera.</summary>
	public DepthStencilOpDesc BackFace ;
	
	
	
	public static readonly DepthStencilDesc Default = new( ) {
		DepthEnable      = true,
		DepthWriteMask   = DepthWriteMask.All,
		DepthFunc        = ComparisonFunction.Less,
		StencilEnable    = false,
		StencilReadMask  = 0xFF,
		StencilWriteMask = 0xFF,
		FrontFace        = DepthStencilOpDesc.Default,
		BackFace         = DepthStencilOpDesc.Default,
	} ;
	
	
	public DepthStencilDesc( bool depthEnable, DepthWriteMask depthWriteMask, ComparisonFunction depthFunc,
							 bool stencilEnable, byte stencilReadMask, byte stencilWriteMask,
							 DepthStencilOpDesc frontFace, DepthStencilOpDesc backFace ) {
		DepthEnable = depthEnable ;
		DepthWriteMask = depthWriteMask ;
		DepthFunc = depthFunc ;
		StencilEnable = stencilEnable ;
		StencilReadMask = stencilReadMask ;
		StencilWriteMask = stencilWriteMask ;
		FrontFace = frontFace ;
		BackFace = backFace ;
	}
	
	public DepthStencilDesc( bool depthEnable = true,
							 DepthWriteMask depthWriteMask = DepthWriteMask.All,
							 ComparisonFunction depthFunc = ComparisonFunction.Less,
							 bool stencilEnable = false, byte stencilReadMask = 0xFF, byte stencilWriteMask = 0xFF,
							 DepthStencilOpDesc? frontFace = null, DepthStencilOpDesc? backFace = null ) {
		DepthEnable = depthEnable ;
		DepthWriteMask = depthWriteMask ;
		DepthFunc = depthFunc ;
		StencilEnable = stencilEnable ;
		StencilReadMask = stencilReadMask ;
		StencilWriteMask = stencilWriteMask ;
		FrontFace = frontFace ?? DepthStencilOpDesc.Default ;
		BackFace = backFace ?? DepthStencilOpDesc.Default ;
	}
} ;
	

[StructLayout(LayoutKind.Sequential),
	ProxyFor( typeof( D3D12_DEPTH_STENCILOP_DESC ) )]
public struct DepthStencilOpDesc {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_stencil_op">D3D12_STENCIL_OP</a>-typed value that identifies the stencil operation to perform when stencil testing fails.</summary>
	public StencilOperation StencilFailOp ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_stencil_op">D3D12_STENCIL_OP</a>-typed value that identifies the stencil operation to perform when stencil testing passes and depth testing fails.</summary>
	public StencilOperation StencilDepthFailOp ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_stencil_op">D3D12_STENCIL_OP</a>-typed value that identifies the stencil operation to perform when stencil testing and depth testing both pass.</summary>
	public StencilOperation StencilPassOp ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_comparison_func">D3D12_COMPARISON_FUNC</a>-typed value that identifies the function that compares stencil data against existing stencil data.</summary>
	public ComparisonFunction StencilFunc ;
	
	
	
	public static readonly DepthStencilOpDesc Default = new( ) {
		StencilFailOp      = StencilOperation.Keep,
		StencilDepthFailOp = StencilOperation.Keep,
		StencilPassOp      = StencilOperation.Keep,
		StencilFunc        = ComparisonFunction.Always,
	} ;
	
	public DepthStencilOpDesc( StencilOperation stencilFailOp = StencilOperation.Keep,
							   StencilOperation stencilDepthFailOp = StencilOperation.Keep,
							   StencilOperation stencilPassOp = StencilOperation.Keep,
							   ComparisonFunction stencilFunc = ComparisonFunction.Always ) {
		StencilFailOp = stencilFailOp ;
		StencilDepthFailOp = stencilDepthFailOp ;
		StencilPassOp = stencilPassOp ;
		StencilFunc = stencilFunc ;
	}
} ;
	

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

	
	public DepthStencilViewDesc( Format format, DSVFlags flags, Tex1DDSV texture1D ) {
		Flags         = flags ; Format = format ;
		ViewDimension = DSVDimension.Texture1D ;
		ReadAs        = new( ) { Texture1D = texture1D } ;
	}
	public DepthStencilViewDesc( Format format, DSVFlags flags, Tex1DArrayDSV texture1DArray ) {
		Flags         = flags ; Format = format ;
		ViewDimension = DSVDimension.Texture1DArray ;
		ReadAs        = new( ) { Texture1DArray = texture1DArray } ;
	}
	public DepthStencilViewDesc( Format format, DSVFlags flags, Tex2DDSV texture2D ) {
		Flags         = flags ; Format = format ;
		ViewDimension = DSVDimension.Texture2D ;
		ReadAs        = new( ) { Texture2D = texture2D } ;
	}
	public DepthStencilViewDesc( Format format, DSVFlags flags, Tex2DArrayDSV texture2DArray ) {
		Flags         = flags ; Format = format ;
		ViewDimension = DSVDimension.Texture2DArray ;
		ReadAs        = new( ) { Texture2DArray = texture2DArray } ;
	}
	public DepthStencilViewDesc( Format format, DSVFlags flags, Tex2DMSDSV texture2DMS ) {
		Flags         = flags ; Format = format ;
		ViewDimension = DSVDimension.Texture2DMS ;
		ReadAs        = new( ) { Texture2DMS = texture2DMS } ;
	}
	public DepthStencilViewDesc( Format format, DSVFlags flags, Tex2DMSArrayDSV texture2DMSArray ) {
		Flags         = flags ; Format = format ;
		ViewDimension = DSVDimension.Texture2DMSArray ;
		ReadAs        = new( ) { Texture2DMSArray = texture2DMSArray } ;
	}
} ;


// --- Tex1D ---

[StructLayout(LayoutKind.Sequential),
	ProxyFor( typeof( D3D12_TEX1D_DSV ) )]
public struct Tex1DDSV {
	/// <summary>The index of the first mipmap level to use.</summary>
	public uint MipSlice ;
	
	public Tex1DDSV( uint mipSlice ) => MipSlice = mipSlice ;
	public static implicit operator uint( Tex1DDSV dsv ) => dsv.MipSlice ;
	public static implicit operator Tex1DDSV( uint mipSlice ) => new( mipSlice ) ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX1D_ARRAY_DSV ) )]
public struct Tex1DArrayDSV {
	/// <summary>The index of the first mipmap level to use.</summary>
	public uint MipSlice ;
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice ;
	/// <summary>Number of textures to use.</summary>
	public uint ArraySize ;
	
	
	public Tex1DArrayDSV( uint mipSlice, uint firstArraySlice, uint arraySize ) {
		MipSlice = mipSlice ; FirstArraySlice = firstArraySlice ; ArraySize = arraySize ;
	}
	public static implicit operator Tex1DArrayDSV( in (uint mipSlice, uint firstArraySlice, uint arraySize) dsv ) =>
		new( dsv.mipSlice, dsv.firstArraySlice, dsv.arraySize ) ;
} ;

// --- Tex2D ---

[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2D_DSV ) )]
public struct Tex2DDSV {
	/// <summary>The index of the first mipmap level to use.</summary>
	public uint MipSlice ;
	
	public Tex2DDSV( uint mipSlice ) => MipSlice = mipSlice ;
	public static implicit operator uint( Tex2DDSV dsv ) => dsv.MipSlice ;
	public static implicit operator Tex2DDSV( uint mipSlice ) => new( mipSlice ) ;
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
	
	public Tex2DArrayDSV( uint mipSlice, uint firstArraySlice, uint arraySize ) {
		MipSlice = mipSlice ; FirstArraySlice = firstArraySlice ; ArraySize = arraySize ;
	}
	public static implicit operator Tex2DArrayDSV( in (uint mipSlice, uint firstArraySlice, uint arraySize) dsv ) =>
		new( dsv.mipSlice, dsv.firstArraySlice, dsv.arraySize ) ;
} ;


[StructLayout( LayoutKind.Sequential ),
 ProxyFor( typeof( D3D12_TEX2DMS_DSV ) )]
public struct Tex2DMSDSV {
	/// <summary>Unused.</summary>
	public uint UnusedField_NothingToDefine ;
	
	
	public static readonly Tex2DMSDSV Default =
		new( ) { UnusedField_NothingToDefine = 0 } ;
	internal Tex2DMSDSV( uint unusedField_NothingToDefine ) =>
		UnusedField_NothingToDefine = unusedField_NothingToDefine ;
} ;


[StructLayout(LayoutKind.Sequential),
 ProxyFor(typeof(D3D12_TEX2DMS_ARRAY_DSV))]
public struct Tex2DMSArrayDSV {
	/// <summary>The index of the first texture to use in an array of textures.</summary>
	public uint FirstArraySlice ;
	/// <summary>Number of textures to use.</summary>
	public uint ArraySize ;
	
	public Tex2DMSArrayDSV( uint firstArraySlice, uint arraySize ) {
		FirstArraySlice = firstArraySlice ; ArraySize = arraySize ;
	}
	public static implicit operator Tex2DMSArrayDSV( in (uint firstArraySlice, uint arraySize) dsv ) =>
		new( dsv.firstArraySlice, dsv.arraySize ) ;
} ;