#region Using Directives
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Represents a DXGI Device object.</summary>
public class Device: Object,
					 IDevice {
	public new static Type ComType => typeof(IDXGIDevice) ;
	public new static Guid InterfaceGUID => typeof(IDXGIDevice).GUID ;
	static IDXCOMObject IInstantiable.Instantiate( ) => new Device( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint ptr ) => new Device( ptr ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom obj ) => 
		new Device( ( obj as IDXGIDevice )! ) ;
	
	public new IDXGIDevice? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIDevice >? ComPointer { get ; protected set ; }

	
	internal Device( ) { }
	internal Device( nint ptr ) => ComPointer = new( ptr ) ;
	internal Device( in ComPtr< IDXGIDevice > dxgiObj ) => this.ComPointer = dxgiObj ;
	internal Device( in IDXGIDevice dxgiObj ) => ComPointer = new( dxgiObj ) ;

	
	public IAdapter GetAdapter< T >( ) where T: class, IAdapter, IInstantiable {
		_throwIfNull( ) ;
		COMObject!.GetAdapter( out var ppAdapter ) ;
		return (T)T.Instantiate( ppAdapter ) ;
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

	public void QueryResourceResidency( in Resource?[] ppResources, 
										out Span< Residency > statusSpan, 
										uint numResources ) {
		_throwIfNull( ) ;
		 
		unsafe {
			Residency* stackPtr = stackalloc Residency[ ppResources.Length ] ;
			
			var array = ppResources
						.Select< Resource, object >( p => p?.COMObject! )?
								.ToArray( ) ;
			
			this.COMObject?.QueryResourceResidency( array,
													(DXGI_RESIDENCY *)stackPtr,
														numResources ) ;
			
			statusSpan = new Residency[ ppResources.Length ] ;
			for ( var i = 0; i < ppResources.Length; ++i )
				statusSpan[ i ] = stackPtr[ i ] ;
		}
	}


	/*public void QueryResourceResidency( in  Resource?[ ] ppResources, 
										out Span< Residency > statusSpan, 
										uint numResources ) {
		_throwIfNull( ) ;

		unsafe {
			Residency* stackPtr = stackalloc Residency[ ppResources.Length ] ;

			var array = ppResources
						.Select< Resource, object >( p => p?.COMObject! )?
								.ToArray( ) ;
			
			this.COMObject?.QueryResourceResidency( array,
													(DXGI_RESIDENCY *)stackPtr,
														numResources ) ;
			
			statusSpan = new Residency[ ppResources.Length ] ;
			for ( var i = 0; i < ppResources.Length; ++i )
				statusSpan[ i ] = stackPtr[ i ] ;
		}
	}
	*/

	
	public void SetGPUThreadPriority( int priority ) {
		_throwIfNull( ) ;
		this.COMObject?.SetGPUThreadPriority( priority ) ;
	}

	
	public void GetGPUThreadPriority( out int pPriority ) {
		_throwIfNull( ) ;
		this.COMObject!.GetGPUThreadPriority( out pPriority ) ;
	}
	
	
	
	protected virtual void _throwIfNull( ) {
		if ( this.ComPointer?.Disposed ?? true )
			throw new NullReferenceException( $"{nameof( Device )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
	}
} ;