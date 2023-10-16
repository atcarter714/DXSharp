#region Using Directives
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Inherited from objects that are tied to the device so that they can retrieve a pointer to it.</summary>
/// <remarks>
/// Represents a <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgidevicesubobject">
/// IDXGIDeviceSubObject interface
/// </a>.
/// </remarks>
public class DeviceSubObject: Object,
							  IDeviceSubObject {
	internal new IDXGIDeviceSubObject? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIDeviceSubObject >? ComPointer { get ; protected set ; }
	
	
	internal DeviceSubObject( ) { }
	internal DeviceSubObject( nint ptr ) {
		ComPointer      = new( ptr ) ;
		base.ComPointer = (ComPtr< IDXGIObject >)ComPointer ;
	}
	internal DeviceSubObject( in IDXGIDeviceSubObject dxgiObj ) {
		ComPointer      = new( dxgiObj ) ;
		base.ComPointer = (ComPtr< IDXGIObject >)ComPointer ;
	}
	internal DeviceSubObject( in ComPtr< IDXGIDeviceSubObject > dxgiObj ) {
		ComPointer      = dxgiObj ;
		base.ComPointer = (ComPtr< IDXGIObject >)ComPointer ;
	}

	
	public T GetDevice< T >( ) where T: Device {
		
		unsafe {
			var riid = typeof( T ).GUID ;
			this.COMObject!.GetDevice( &riid, out var ppDevice ) ;
			return (T)( new Device( ( ppDevice as IDXGIDevice )! ) ) ;
		}
	}
} ;
