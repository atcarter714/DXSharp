#region Using Directives

using System.Diagnostics ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Represents a DXGI Device object.</summary>
[DebuggerDisplay($"{nameof(Device)}: {nameof(ComPointer)} = ComPointer")]
public class Device: Object,
					 IDevice {
	public new static Type ComType => typeof(IDXGIDevice) ;
	public new static Guid InterfaceGUID => typeof(IDXGIDevice).GUID ;
	static IInstantiable IInstantiable. Instantiate( )            => new Device( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr ptr ) => new Device( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom obj ) => 
		new Device( ( obj as IDXGIDevice )! ) ;
	
	public new IDXGIDevice? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIDevice >? ComPointer { get ; protected set ; }

	
	internal Device( ) { }
	internal Device( nint ptr ) => ComPointer = new( ptr ) ;
	internal Device( in ComPtr< IDXGIDevice > dxgiObj ) => this.ComPointer = dxgiObj ;
	internal Device( in IDXGIDevice dxgiObj ) => ComPointer = new( dxgiObj ) ;

	
	public IAdapter GetAdapter< T >( ) where T: class, IAdapter, IInstantiable {
		var device = this.COMObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
			;

		device.GetAdapter( out var ppAdapter ) ;
		return (T)T.Instantiate( ppAdapter ) ;
	}
	
	
	/* NOTE: -----------------------------------
		 The CreateSurface method creates a buffer to exchange data between one or more devices. 
		 It is used internally, and you should not directly call it.
		 The runtime automatically creates an IDXGISurface interface when it creates a Direct3D 
		 resource object that represents a surface. 
		 ---------------------------------------------------------------------------------------- */
	
	/// <summary>Returns a surface. This method is used internally and you should not call it directly in your application.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_surface_desc">DXGI_SURFACE_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_surface_desc">DXGI_SURFACE_DESC</a> structure that describes the surface.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="numSurfaces">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of surfaces to create.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="usage">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a></b> A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a> flag that specifies how the surface is expected to be used.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSharedResource">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_shared_resource">DXGI_SHARED_RESOURCE</a>*</b> An optional pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_shared_resource">DXGI_SHARED_RESOURCE</a> structure that contains shared resource information for opening views of such resources.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppSurface">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgisurface">IDXGISurface</a>**</b> The address of an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgisurface">IDXGISurface</a> interface pointer to the first created surface.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; an error code otherwise.  For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>The <b>CreateSurface</b> method creates a buffer to exchange data between one or more devices. It is used internally, and you should not directly call it. The runtime automatically creates an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgisurface">IDXGISurface</a> interface when it creates a Direct3D resource object that represents a surface. For example, the runtime creates an <b>IDXGISurface</b> interface when it calls <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11device-createtexture2d">ID3D11Device::CreateTexture2D</a> or <a href="https://docs.microsoft.com/windows/desktop/api/d3d10/nf-d3d10-id3d10device-createtexture2d">ID3D10Device::CreateTexture2D</a> to create a 2D texture. To retrieve the <b>IDXGISurface</b> interface that represents the 2D texture surface, call <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(q)">ID3D11Texture2D::QueryInterface</a> or <b>ID3D10Texture2D::QueryInterface</b>. In this call, you must pass the identifier of <b>IDXGISurface</b>. If the 2D texture has only a single MIP-map level and does not consist of an array of textures, <b>QueryInterface</b> succeeds and returns a pointer to the <b>IDXGISurface</b> interface pointer. Otherwise, <b>QueryInterface</b> fails and does not return the pointer to <b>IDXGISurface</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	public void CreateSurface( in SurfaceDescription pDesc,
							   uint numSurfaces, uint usage,
							   ref SharedResource? pSharedResource,
							   out Span< Surface > ppSurface ) {
		ppSurface = default! ;
		if( numSurfaces <= 0U ) return ;
		
		var device = this.COMObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
			;

		unsafe {
			DXGI_SHARED_RESOURCE* sharedResOut =  null ;
			DXGI_SHARED_RESOURCE localSharedRes = default ;
			if ( pSharedResource is not null ) {
				localSharedRes = pSharedResource.Value ;
				sharedResOut = &localSharedRes ;
			}
			
			fixed ( SurfaceDescription* pDescPtr = &pDesc ) {
				IDXGISurface[ ] surfaceData = new IDXGISurface[ numSurfaces ] ;
				device.CreateSurface( (DXGI_SURFACE_DESC *) pDescPtr, 
									  numSurfaces,
									  (DXGI_USAGE)usage,
									  sharedResOut,
									  surfaceData ) ;
				
				pSharedResource = localSharedRes ;
				
				ppSurface = new Surface[ numSurfaces ] ;
				for ( int i = 0; i < numSurfaces; ++i ) {
					ppSurface[ i ] = new( surfaceData[i] ) ;
				}
			}
		}
	}

	public void QueryResourceResidency( in Resource?[ ] ppResources, 
										out Span< Residency > statusSpan, 
										uint numResources ) {
		var device = this.COMObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
			;

		unsafe {
			Residency* stackPtr = stackalloc Residency[ ppResources.Length ] ;
			
			var array = ppResources
						.Select< Resource?, object >( p => p?.COMObject! )?
								.ToArray( ) ;
			
			device.QueryResourceResidency( array, (DXGI_RESIDENCY *)stackPtr, numResources ) ;
			
			statusSpan = new Residency[ ppResources.Length ] ;
			for ( var i = 0; i < ppResources.Length; ++i )
				statusSpan[ i ] = stackPtr[ i ] ;
		}
	}

	
	
	public void SetGPUThreadPriority( int priority ) {
		var device = this.COMObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
			;
		
		device.SetGPUThreadPriority( priority ) ;
	}

	
	public void GetGPUThreadPriority( out int pPriority ) {
		var device = this.COMObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
			;
		device.GetGPUThreadPriority( out pPriority ) ;
	}
	
	
	
	/*
	protected virtual void _throwIfNull( ) {
		if ( this.ComPointer?.Disposed ?? true )
			throw new NullReferenceException( $"{nameof( Device )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
	}*/
} ;