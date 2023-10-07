#region Using Directives
using System ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion

namespace DXSharp.DXGI ;

public interface DXGIObjWrapper< T_DXGI >:  IUnknownWrapper
											where T_DXGI: IUnknown {
	T_DXGI? COMObject { get ; }
} ;

// -----------------------------------------------------------------

public interface IDevice: IUnknown< IDevice > {
	void GetAdapter< T >( out T ppAdapter )
							where T: Adapter ;
	void CreateSurface( in SurfaceDescription pDesc,
						uint numSurfaces, uint usage,
						in SharedResource pSharedResource,
						out Surface ppSurface ) ;
	void QueryResourceResidency( in IUnknown[ ] ppResources,
								 out Residency[ ] pResidencyStatus,
								 uint numResources ) ;
	void SetGPUThreadPriority( int priority ) ;
	void GetGPUThreadPriority( out int pPriority ) ;
} ;

public interface IDevice< T_DXGI > where T_DXGI: IDXGIDevice, IDevice {
	T_DXGI COMObject { get ; }
} ;

// -----------------------------------------------------------------
// Device Sub-Object Types:
// -----------------------------------------------------------------

//! Abstract Base Interface:
public interface IDeviceSubObject: IUnknown< IDeviceSubObject > {
	void GetDevice< T >( out T ppDevice ) where T: class, IDevice ;
} ;
public interface IDeviceSubObject< T_DXGI >: IDeviceSubObject
							   where T_DXGI: IDXGIDeviceSubObject {
	T_DXGI? COMObject { get ; }
} ;

//! Concrete Base Implementation:
public class DeviceSubObject: Object,
							  IDeviceSubObject,
							  IDeviceSubObject< IDXGIDeviceSubObject > {

	public IDXGIDeviceSubObject? COMObject { get ; init ; }
	
	public void GetDevice< T >( out T ppDevice )
									where T: class, IDevice {
		
		var _obj = Marshal.GetObjectForIUnknown( this.ComPtr?.Address ?? 0 ) ;
		if ( _obj is IDXGIDeviceSubObject subObject ) {
			subObject.GetDevice( typeof(IDXGIDevice).GUID, out var pDevice ) ;
			ppDevice = (T) pDevice ;
		}
	}
} ;