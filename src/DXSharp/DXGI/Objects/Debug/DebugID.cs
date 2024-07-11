namespace DXSharp.DXGI ;


/// <summary>Defines the <b>DXGI_DEBUG_ID</b> enumerated "Debug ID" <see cref="Guid"/> values.</summary>
[EquivalentOf("DXGI_DEBUG_ID", "DXGIDebug.h")]
[NativeLibrary("dxguid.lib", "DXGI_DEBUG_ID", "DXGIDebug.h")]
public static class DebugID {
	/// <summary>All Direct3D and DXGI objects and private apps.</summary>
	public static readonly Guid All = new( 0xe48ae283, 0xda80, 0x490b, 0x87, 0xe6, 
												0x43, 0xe9, 0xa9, 0xcf, 0xda, 0x8 ) ;
	
	/// <summary>Direct3D and DXGI objects.</summary>
	public static readonly Guid DX = new( 0x35cdd7fc, 0x13b2, 0x421d, 0xa5, 0xd7,
											   0x7e, 0x44, 0x51, 0x28, 0x7d, 0x64 ) ;
	
	/// <summary>DXGI</summary>
	public static readonly Guid DXGI = new( 0x25cddaa4, 0xb1c6, 0x47e1, 0xac, 0x3e, 
												 0x98, 0x87, 0x5b, 0x5a, 0x2e, 0x2a ) ;
	
	/// <summary>Private apps.</summary>
	/// <remarks>Any messages that you add with <see cref="IInfoQueue.AddApplicationMessage"/>.</remarks>
	public static readonly Guid App = new( 0x6cd6e01, 0x4219, 0x4ebd, 0x87, 0x9, 
												0x27, 0xed, 0x23, 0x36, 0xc, 0x62 ) ;
	
	/// <summary>Direct3D 11.</summary>
	/// <remarks>Defined in D3D11SDKLayers.h.</remarks>
	public static readonly Guid D3D11 = new( 0x4b99317b, 0xac39, 0x4aa6, 0xbb, 0xb, 
												  0xba, 0xa0, 0x47, 0x84, 0x79, 0x8f ) ;
	
	
	
	/// <summary>Gets all defined <see cref="Guid"/> values as a <see cref="Span{Guid}"/>.</summary>
	/// <returns>
	/// A <see cref="Span{Guid}"/> containing all of the defined <b>DXGI_DEBUG_ID</b> <see cref="Guid"/> values.
	/// </returns>
	public static Span< Guid > GetAllGUIDsAsSpan( ) {
		var guids = new[ ] { All, DX, DXGI, App, D3D11 } ;
		return guids ;
	}
} ;
