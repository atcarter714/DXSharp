#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows.COM ;
#endregion

namespace DXSharp.DXGI ;


/*public interface IDeviceSubObject< T_DXGI >: IDeviceSubObject
	where T_DXGI: IDXGIDeviceSubObject {
	T_DXGI? COMObject { get ; }
} ;*/
/*public interface IDeviceSubObject< T_DXGI >: IDeviceSubObject,
											 IDXGIObjWrapper< T_DXGI >
												where T_DXGI:   //IUnknown, 
																IDXGIObject, 
																IDXGIDeviceSubObject { } ;*/

//! Concrete Base Implementation:
public class DeviceSubObject: Object,
							  IDeviceSubObject {
	internal IDXGIDeviceSubObject? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIDeviceSubObject >? ComPointer { get ; protected set ; }
	
	internal DeviceSubObject( ) => this.ComPointer = new( ) ;
	
	public DeviceSubObject( nint ptr ): base( ptr ) {
		if( !ptr.IsValid() ) throw new NullReferenceException( $"{nameof(DeviceSubObject)} :: " +
															   $"The internal COM interface is destroyed/null." ) ;
		
		var dxgiObj = COMUtility.GetDXGIObject< IDXGIDeviceSubObject >( ptr ) ;
		if ( dxgiObj is null ) throw new
			COMException( $"{nameof(DeviceSubObject)}.ctor( {nameof(ptr)}: 0x{ptr:X} ): " +
				$"Unable to initialize COM object reference from given address!" ) ;
		
		this.COMObject = dxgiObj ;
		//! TODO: Calling :base(ptr) constructor is assigning this, but for IDXGIObject ... needs investigation/testing
		this.ComPointer = new( dxgiObj ) ;
	}
	public DeviceSubObject( in IDXGIDeviceSubObject dxgiObj ): base( dxgiObj ) {
		this.COMObject = dxgiObj ?? throw new ArgumentNullException( nameof(dxgiObj) ) ;
		this.ComPointer = new( dxgiObj ) ;
	}

	
	public T GetDevice< T >( ) where T: Device {
		_throwIfNull( ) ;

		unsafe {
			var riid = typeof( T ).GUID ;
			this.COMObject!.GetDevice( &riid, out var ppDevice ) ;
			return (T)( new Device( ( ppDevice as IDXGIDevice )! ) ) ;
		}
	}
} ;



public class Device: Object,
					 IDevice {
	public IDXGIDevice? COMObject { get ; init ; }
	public new ComPtr< IDXGIDevice >? ComPointer { get ; init ; }

	public Device( nint ptr ): base( ptr ) {
		if ( !ptr.IsValid( ) )
			throw new NullReferenceException( $"{nameof( Device )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;

		var dxgiObj = COMUtility.GetDXGIObject< IDXGIDevice >( ptr ) ;
		if ( dxgiObj is null )
			throw new
				COMException( $"{nameof( Device )}.ctor( {nameof( ptr )}: 0x{ptr:X} ): " +
							  $"Unable to initialize COM object reference from given address!" ) ;

		this.COMObject = dxgiObj ;
	}
	public Device( in IDXGIDevice dxgiObj ): base( dxgiObj ) {
		this.COMObject = dxgiObj ?? throw new ArgumentNullException( nameof(dxgiObj) ) ;
	}

	
	public T GetAdapter<T>( ) where T: class, IAdapter {
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
			for ( var i = 0; i < ppResources.Length; i++ )
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
	

	
	//! warning -----------------------------------
	#warning Potentially unnecessary overrides/duplication of base methods
	//! TODO: Figure out if we actually need to override/hide the base versions
	public new void GetParent< T >( out T ppParent ) where T: IUnknownWrapper {
		_throwIfDestroyed( ) ;
		_throwIfNull( ) ;
		ppParent = default! ;
		
		unsafe {
			object parent = default! ;
			var riid = typeof( T ).GUID ;
			this.COMObject?.GetParent( &riid, out parent ) ;
			ppParent = (T)parent! ;
		}
	}

	public new void GetPrivateData< TData >( out uint pDataSize, nint pData ) where TData: unmanaged {
		_throwIfDestroyed( ) ;
		_throwIfNull( ) ;

		uint dataSize = 0U ;
		Guid name     = typeof( IDXGIDevice ).GUID ;
		
		unsafe { this.COMObject!.GetPrivateData( &name, ref dataSize, (void*)pData ) ; }
		pDataSize = dataSize ;
	}

	public new void SetPrivateData< T >( uint DataSize, nint pData ) {
		_throwIfDestroyed( ) ;
		_throwIfNull( ) ;

		uint dataSize = 0U ;
		Guid name     = typeof( IDXGIDevice ).GUID ;
		unsafe {
			this.COMObject!.SetPrivateData( &name, dataSize, (void*)pData ) ;
		}
	}

	public new void SetPrivateDataInterface< T >( in T pUnknown ) where T: IUnknownWrapper< IUnknown > {
		_throwIfDestroyed( ) ;
		_throwIfNull( ) ;
		
		Guid name = typeof(IDXGIDevice).GUID ;
		unsafe {
			this.COMObject!.SetPrivateDataInterface( &name, pUnknown ) ;
		}
	}
	//! warning -----------------------------------
	
	
	
	protected virtual void _throwIfNull( ) {
		if ( this.COMObject is null || this.ComPointer is null )
			throw new NullReferenceException( $"{nameof( Device )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
	}
} ;