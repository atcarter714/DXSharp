#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


public abstract class DeviceChild: Object, IDeviceChild {
	public new ID3D12DeviceChild? COMObject => ComPointer?.Interface ;
	public new virtual ComPtr< ID3D12DeviceChild >? ComPointer { get ; }

	protected DeviceChild( ) =>
		ComPointer = null ;
	protected DeviceChild( nint childAddr ) =>
		ComPointer = new( childAddr ) ;
	protected DeviceChild( ID3D12DeviceChild child ) =>
		ComPointer = new( child ) ;
	protected DeviceChild( ComPtr< ID3D12DeviceChild > childPtr ) =>
		ComPointer = childPtr ;
} ;