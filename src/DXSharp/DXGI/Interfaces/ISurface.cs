#region Using Directives

using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning;
using Windows.Win32 ;
using Windows.Win32.Graphics.Gdi ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


[ProxyFor(typeof(IDXGISurface))]
public interface ISurface:  IDeviceSubObject,
							IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	[SuppressMessage( "Interoperability", 
					  "CA1416:Validate platform compatibility" )] 
	internal static readonly ReadOnlyDictionary< Guid, Func<IDXGISurface, IInstantiable> > _surfaceCreationFunctions =
		new( new Dictionary<Guid, Func<IDXGISurface, IInstantiable> > {
			{ ISurface.IID, ( pComObj ) => new Surface( pComObj ) },
			{ ISurface1.IID, ( pComObj ) => new Surface1( (pComObj as IDXGISurface1)! ) },
			{ ISurface2.IID, ( pComObj ) => new Surface2( (pComObj as IDXGISurface2)! ) },
		} ) ;
	// ---------------------------------------------------------------------------------
	
	/// <summary>Get a description of the surface.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_surface_desc">DXGI_SURFACE_DESC</a>*</b> A pointer to the surface description (see <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_surface_desc">DXGI_SURFACE_DESC</a>).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgisurface-getdesc#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgisurface-getdesc">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetDesc( out SurfaceDescription pDesc ) ;
	
	/// <summary>Get a pointer to the data contained in the surface, and deny GPU access to the surface.</summary>
	/// <param name="pLockedRect">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_mapped_rect">DXGI_MAPPED_RECT</a>*</b> A pointer to the surface data (see <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_mapped_rect">DXGI_MAPPED_RECT</a>).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgisurface-map#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="MapFlags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> CPU read-write flags. These flags can be combined with a logical OR.</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgisurface-map#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</para>
	/// </returns>
	/// <remarks>Use <b>IDXGISurface::Map</b> to access a surface from the CPU. To release a mapped surface (and allow GPU access) call <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgisurface-unmap">IDXGISurface::Unmap</a>.</remarks>
	void Map( ref MappedRect pLockedRect, MapFlags MapFlags ) ;
	
	/// <summary>Invalidate the pointer to the surface retrieved by IDXGISurface::Map and re-enable GPU access to the resource.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgisurface-unmap">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void Unmap( ) ;
	
	// --------------------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGISurface ) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGISurface).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// =================================================================================================
} ;


/// <summary>
/// The <a href="https://learn.microsoft.com/en-us/windows/win32/api/DXGI/nn-dxgi-idxgisurface1">IDXGISurface1</a> interface
/// extends the <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgisurface">IDXGISurface</a> by adding support for using Windows Graphics Device Interface (GDI) to render to a Microsoft
/// DirectX Graphics Infrastructure (DXGI) surface.
/// </summary>
[SupportedOSPlatform("windows6.1")]
[ProxyFor(typeof(IDXGISurface1))]
public interface ISurface1: ISurface  {
	
	/// <summary>Returns a device context (DC) that allows you to render to a Microsoft DirectX Graphics Infrastructure (DXGI) surface using Windows Graphics Device Interface (GDI).</summary>
	/// <param name="Discard">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">BOOL</a></b> A Boolean value that specifies whether to preserve Direct3D contents in the GDI DC. <b>TRUE</b> directs the runtime not to preserve Direct3D contents in the GDI DC; that is, the runtime discards the Direct3D contents. <b>FALSE</b> guarantees that Direct3D contents are available in the GDI DC.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgisurface1-getdc#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="phdc">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HDC</a>*</b> A pointer to an <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HDC</a> handle that represents the current device context for GDI rendering.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgisurface1-getdc#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, an error code.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method is not supported by DXGI 1.0, which shipped in Windows Vista and Windows Server 2008. DXGI 1.1 support is required, which is available on Windows 7, Windows Server 2008 R2, and as an update to Windows Vista with Service Pack 2 (SP2) (<a href="https://support.microsoft.com/topic/application-compatibility-update-for-windows-vista-windows-server-2008-windows-7-and-windows-server-2008-r2-february-2010-3eb7848b-9a76-85fe-98d0-729e3827ea60">KB 971644</a>) and Windows Server 2008 (<a href="https://support.microsoft.com/kb/971512/">KB 971512</a>). After you use the <b>GetDC</b> method to retrieve a DC, you can render to the DXGI surface by using GDI. The <b>GetDC</b> method readies the surface for GDI rendering and allows inter-operation between DXGI and GDI technologies. Keep the following in mind when using this method: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgisurface1-getdc#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetDC( bool Discard, ref HDC phdc ) ;

	/// <summary>Releases the GDI device context (DC) that is associated with the current surface and allows you to use Direct3D to render.</summary>
	/// <param name="pDirtyRect">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-rect">RECT</a>*</b> A pointer to a <b>RECT</b> structure that identifies the dirty region of the surface. A dirty region is any part of the surface that you used for GDI rendering and that you want to preserve. This area is used as a performance hint to graphics subsystem in certain scenarios. Do not use this parameter to restrict rendering to the specified rectangular region. If you pass in <b>NULL</b>, <b>ReleaseDC</b> considers the whole surface as dirty. Otherwise, <b>ReleaseDC</b> uses the area specified by the RECT as a performance hint to indicate what areas have been manipulated by GDI rendering. You can pass a pointer to an empty <b>RECT</b> structure (a rectangle with no position or area) if you didn't change any content.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgisurface1-releasedc#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> If this method succeeds, it returns <b>S_OK</b>. Otherwise, it returns an <b>HRESULT</b> error code.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method is not supported by DXGI 1.0, which shipped in Windows Vista and Windows Server 2008. DXGI 1.1 support is required, which is available on Windows 7, Windows Server 2008 R2, and as an update to Windows Vista with Service Pack 2 (SP2) (<a href="https://support.microsoft.com/topic/application-compatibility-update-for-windows-vista-windows-server-2008-windows-7-and-windows-server-2008-r2-february-2010-3eb7848b-9a76-85fe-98d0-729e3827ea60">KB 971644</a>) and Windows Server 2008 (<a href="https://support.microsoft.com/kb/971512/">KB 971512</a>). Use the <b>ReleaseDC</b> method to release the DC and indicate that your application finished all GDI rendering to this surface. You must call the <b>ReleaseDC</b> method before you can use Direct3D to perform additional rendering. Prior to resizing buffers you must release all outstanding DCs.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgisurface1-releasedc#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void ReleaseDC( in Rect? pDirtyRect = default ) ;
	
	
	new static Type ComType => typeof( IDXGISurface1 ) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGISurface1).GUID
															 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable. Instantiate( ) => new Surface1( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Surface1( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Surface1( (pComObj as IDXGISurface1)! ) ;

} ;


[SupportedOSPlatform( "windows8.0" )]
[ProxyFor( typeof( IDXGISurface2 ) )]
public interface ISurface2: ISurface1,
							IComObjectRef< IDXGISurface2 >,
							IUnknownWrapper< IDXGISurface2 > {
	// --------------------------------------------------------------------------------------------
	
	/// <summary>Gets the parent resource and subresource index that support a subresource surface.</summary>
	/// <param name="riid">The globally unique identifier (GUID)  of the requested interface type.</param>
	/// <param name="ppParentResource">A pointer to a buffer that receives a pointer to the parent resource object for the subresource surface.</param>
	/// <param name="pSubresourceIndex">A pointer to a variable that receives the index of the subresource surface.</param>
	/// <returns>
	/// <para>Returns S_OK if successful; otherwise, returns one of the following values: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>For subresource surface objects that the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiresource1-createsubresourcesurface">IDXGIResource1::CreateSubresourceSurface</a> method creates, <b>GetResource</b> simply returns the values that were used to create the subresource surface. Current objects that implement <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgisurface">IDXGISurface</a> are either resources or views.  <b>GetResource</b> for these objects returns “this” or the resource that supports the view respectively.  In this situation, the subresource index is 0.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgisurface2-getresource#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetResource< TRes >( in Guid riid, out TRes ppParentResource, out uint pSubresourceIndex )
																		where TRes: IUnknownWrapper, 
																					IInstantiable ;
	
	// --------------------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGISurface2 ) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGISurface2).GUID
															 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable. Instantiate( ) => new Surface2( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Surface2( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Surface2( (pComObj as IDXGISurface2)! ) ;
	// =================================================================================================
} ;
	  

