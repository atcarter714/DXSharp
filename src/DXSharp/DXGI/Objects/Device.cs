#region Using Directives
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;

/// <summary>Represents a DXGI Device object.</summary>
public class Device: Object, IDevice {
	public override ComPtr? ComPtrBase => ComPointer ;
	public IDXGIDevice? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIDevice >? ComPointer { get ; protected set ; }

	internal Device( ) { }
	internal Device( nint ptr ): base( ptr ) {
		ComPointer = new( ptr ) ;
	}
	internal Device( in IDXGIDevice dxgiObj ): base( dxgiObj ) {
		ComPointer = new( dxgiObj ) ;
	}

	
	public T GetAdapter< T >( ) where T: class, IAdapter {
		_throwIfNull( ) ;
		unsafe {
			var riid = typeof( T ).GUID ;
			this.COMObject!.GetAdapter( out var ppAdapter ) ;
			return (T)( (IAdapter)new Adapter(ppAdapter) ) ;
		}
	}

	
	
	/* NOTE: -----------------------------------
		 The CreateSurface method creates a buffer to exchange data between one or more devices. 
		 It is used internally, and you should not directly call it.
		 The runtime automatically creates an IDXGISurface interface when it creates a Direct3D 
		 resource object that represents a surface. 
		 ---------------------------------------------------------------------------------------- */
	public void CreateSurface( in SurfaceDescription pDesc,
								  uint numSurfaces, uint usage,
								  in  SharedResource pSharedResource,
												out Span< Surface > ppSurface ) {
		ppSurface = default! ;
		if( numSurfaces is 0U ) return ;
		_throwIfNull( ) ;

		unsafe {
			SharedResource sharedResOut = pSharedResource ;
			fixed ( SurfaceDescription* pDescPtr = &pDesc ) {
				IDXGISurface[ ] surfaceData = new IDXGISurface[ numSurfaces ] ;
				this.COMObject?.CreateSurface( (DXGI_SURFACE_DESC *) pDescPtr, 
											   numSurfaces, (DXGI_USAGE)usage,
											   (DXGI_SHARED_RESOURCE *)&sharedResOut, 
													surfaceData ) ;
				
				ppSurface = new Surface[ numSurfaces ] ;
				for ( var i = 0; i < numSurfaces; i++ ) {
					ppSurface[ i ] = new( surfaceData[i] ) ;
				}
			}
		}
	}
	
	public void QueryResourceResidency( in IResource[ ] ppResources, 
										out Residency[ ] pResidencyStatus, 
										uint numResources ) {
		_throwIfNull( ) ;

		unsafe {
			var pResourcesPtr = stackalloc nint[ ppResources.Length ] ;
			for ( var i = 0; i < ppResources.Length; ++i )
				pResourcesPtr[ i ] = ppResources[ i ].BasePointer ;

			Residency* pResidencyStatusPtr = stackalloc Residency[ ppResources.Length ] ;
			
			Span< object > ppResourcesSpan = new( pResourcesPtr, ppResources.Length ) ;
			this.COMObject?.QueryResourceResidency( ppResources, 
													(DXGI_RESIDENCY *)pResidencyStatusPtr,
														numResources ) ;

			pResidencyStatus = new Residency[ ppResources.Length ] ;
			for ( var i = 0; i < ppResources.Length; i++ )
				pResidencyStatus[ i ] = pResidencyStatusPtr[ i ] ;
		}
	}

	public void SetGPUThreadPriority( int priority ) {
		_throwIfNull( ) ;
		this.COMObject?.SetGPUThreadPriority( priority ) ;
	}

	public void GetGPUThreadPriority( out int pPriority ) {
		_throwIfNull( ) ;
		this.COMObject!.GetGPUThreadPriority( out pPriority ) ;
	}
	

	
	
	
	
	protected virtual void _throwIfNull( ) {
		if ( this.COMObject is null || this.ComPointer is null )
			throw new NullReferenceException( $"{nameof( Device )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
	}
} ;