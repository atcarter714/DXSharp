#region Using Directives
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;
#endregion

namespace DXSharp.DXGI ;

[Wrapper(typeof(IDXGIDevice))]
public interface IDevice: IObject,
						  IComObjectRef< IDXGIDevice >,
						  IUnknownWrapper< IDXGIDevice >, IInstantiable {
	public new static Guid InterfaceGUID => typeof( IDXGIDevice ).GUID ;
	
	new Type ComType => typeof( IDXGIDevice ) ;
	new IDXGIDevice? COMObject => ComPointer?.Interface ;
	new IDXGIDevice? ComObject => ComPointer?.Interface ;
	new ComPtr< IDXGIDevice >? ComPointer { get ; }
	
	// ----------------------------------------------------------
	// Interface Methods:
	// ----------------------------------------------------------

	IAdapter GetAdapter< T >( ) where T: class, IAdapter, IInstantiable ;
	/*{
		this.COMObject!.GetAdapter( out var ppAdapter ) ;
		var _obj = (IAdapter)( T.Instantiate( ) ??
							   throw new TypeInitializationException( nameof( T ), null ) ) ;
		_obj.SetComPointer( new ComPtr< IDXGIAdapter >(ppAdapter) ) ;
		return _obj ;
	}*/
	
	
	internal void CreateSurface( in   SurfaceDescription pDesc,
								 uint numSurfaces, uint usage,
								 in   SharedResource pSharedResource,
								 out  Span< Surface > ppSurface ) ;
	
	
	void QueryResourceResidency( in  Resource?[ ] ppResources, 
								 out Span< Residency > statusSpan, 
								 uint numResources ) ;
	
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
