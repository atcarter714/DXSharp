using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;

namespace DXSharp.DXGI ;


//! Concrete Base Implementation:
public class DeviceSubObject: Object,
							  IDeviceSubObject {
	public override ComPtr? ComPtrBase => ComPointer ;
	internal IDXGIDeviceSubObject? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIDeviceSubObject >? ComPointer { get ; protected set ; }
	
	internal DeviceSubObject( ) { }
	public DeviceSubObject( nint ptr ): base( ptr ) {
		ComPointer = new( ptr ) ;
	}
	public DeviceSubObject( in IDXGIDeviceSubObject dxgiObj ): base( dxgiObj ) {
		ComPointer = new( dxgiObj ) ;
	}

	
	public T GetDevice< T >( ) where T: Device {
		_throwIfDestroyed( ) ;

		unsafe {
			var riid = typeof( T ).GUID ;
			this.COMObject!.GetDevice( &riid, out var ppDevice ) ;
			return (T)( new Device( ( ppDevice as IDXGIDevice )! ) ) ;
		}
	}
} ;
