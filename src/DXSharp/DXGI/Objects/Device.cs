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
	
	internal IDXGIDeviceSubObject? COMObject { get ; init ; }
	public new ComPtr< IDXGIDeviceSubObject >? ComPointer { get ; init ; }

	void _throwIfNull( ) {
		if ( this.COMObject is null || this.ComPointer?.Interface is null ) 
			throw new NullReferenceException( $"{nameof(DeviceSubObject)} :: " + 
										$"The internal COM interface is destroyed/null." ) ;
	}
	
	
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

	public T GetDevice< T >( ) where T: class, IDevice {
		_throwIfNull( ) ;

		unsafe {
			var riid = typeof( T ).GUID ;
			this.COMObject?.GetDevice( &riid, out var ppDevice ) ;
			return new Device(  ) ;
		}
	}

	/*=>
	this.COMObject ??= COMUtility.GetDXGIObject<IDXGIDeviceSubObject>(ptr) as IDXGIDeviceSubObject
						  ?? throw new NullReferenceException( $"{nameof(DeviceSubObject)} :: " +
															   $"The internal COM interface is destroyed/null." ) ;*/
	
	/*public void GetDevice< T >( out T ppDevice ) where T: class, IDevice {
		var _obj = Marshal.GetObjectForIUnknown( this.ComPtr?.IUnknownAddress ?? 0 ) ;
		if ( _obj is IDXGIDeviceSubObject subObject ) {
			subObject.GetDevice( typeof(IDXGIDevice).GUID, out var pDevice ) ;
			ppDevice = (T) pDevice ;
		}
		else throw new NullReferenceException( $"{nameof(DeviceSubObject)} :: " +
											   $"The internal COM interface is destroyed/null." ) ;
	}*/
} ;

public class Device: Object,
					 IDevice
{

	internal IDXGIDevice? COMObject { get ; init ; }
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

	public T GetAdapter< T >() where T: class, IAdapter {
		_throwIfNull( ) ;

		unsafe {
			var riid = typeof( T ).GUID ;
			this.COMObject?.GetAdapter( &riid, out var ppAdapter ) ;
			return new Adapter( ) ;
		}
	}

	public void CreateSurface( in SurfaceDescription pDesc,
							   uint                  numSurfaces, uint usage,
							   in  SharedResource    pSharedResource,
							   out Surface           ppSurface ) {
		_throwIfNull( ) ;

		unsafe {
			fixed ( SurfaceDescription* pDescPtr = &pDesc ) {
				this.COMObject?.CreateSurface( pDescPtr, numSurfaces, usage,
											   pSharedResource, out var ppSurfacePtr ) ;
				ppSurface = new Surface( ppSurfacePtr ) ;
			}
		}
	}

	public void QueryResourceResidency( in  IUnknownWrapper[] ppResources,
										out Residency[]       pResidencyStatus,
										uint                  numResources ) {
		_throwIfNull( ) ;

		unsafe {
			var pResourcesPtr = stackalloc nint[ ppResources.Length ] ;
			for ( var i = 0; i < ppResources.Length; i++ )
				pResourcesPtr[ i ] = ppResources[ i ].BasePointer ;

			var pResidencyStatusPtr = stackalloc Residency[ ppResources.Length ] ;
			this.COMObject?.QueryResourceResidency( pResourcesPtr, pResidencyStatusPtr,
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

		this.COMObject?.GetGPUThreadPriority( out pPriority ) ;
	}

	internal Guid GetDXGIInterfaceGuid() {
		_throwIfNull( ) ;

		unsafe {
			this.COMObject?.Get( out var riid ) ;
			return riid ;
		}
	}

	public new void GetParent< T >( out T ppParent ) where T: IUnknownWrapper {
		_throwIfDestroyed( ) ;
		_throwIfNull( ) ;
		ppParent = default! ;
		
		unsafe {
			object ppParentPtr = null ;
			var riid = typeof( T ).GUID ;
			this.COMObject?.GetParent( &riid, out ppParentPtr ) ;
			ppParent = (T)ppParentPtr! ;
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

	public new void SetPrivateDataInterface< T >( in T pUnknown ) where T: IUnknownWrapper {
		_throwIfDestroyed( ) ;
		_throwIfNull( ) ;
		
		Guid name = typeof(IDXGIDevice).GUID ;
		unsafe {
			this.COMObject!.SetPrivateDataInterface( &name, pUnknown ) ;
		}
	}

	
	protected virtual void _throwIfNull( ) {
		if ( this.COMObject is null || this.ComPointer is null )
			throw new NullReferenceException( $"{nameof( Device )} :: " +
											  $"The internal COM interface is destroyed/null." ) ;
	}
} ;