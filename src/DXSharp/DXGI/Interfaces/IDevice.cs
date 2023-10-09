#region Using Directives
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;
#endregion

namespace DXSharp.DXGI ;

public interface IDevice: IObject,
						  DXGIWrapper< IDXGIDevice > {
						  //IUnknownWrapper< IDevice, IDXGIDevice > {
	
	T GetAdapter< T >(  ) where T: class, IAdapter ;
	
	internal void CreateSurface( in SurfaceDescription pDesc,
									uint numSurfaces, uint usage,
									in  SharedResource pSharedResource,
												out Span< Surface > ppSurface ) ;
	
	
	void QueryResourceResidency( in  IResource[ ] ppResources,
								 out Residency[ ] pResidencyStatus,
								 uint             numResources ) ;
	
	void SetGPUThreadPriority( int priority ) ;
	
	void GetGPUThreadPriority( out int pPriority ) ;
} ;


// -----------------------------------------------------------------
// Device Sub-Object Types:
// -----------------------------------------------------------------

//! Abstract Base Interface:
public interface IDeviceSubObject: IObject {
	T GetDevice<T>( ) where T: Device ;
} ;
