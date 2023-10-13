using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;


public class Fence: Pageable, IFence, IInstantiable< Fence > {
	static Fence IInstantiable< Fence >.Instantiate( ) => new( ) ;
	static IDXCOMObject IInstantiable.  Instantiate( ) => new Fence( ) ;
	
	ComPtr< ID3D12Pageable >? IPageable.ComPointer => new( this.ComPointer.Interface ) ;
	ComPtr< ID3D12DeviceChild >? IDeviceChild.ComPointer => new( this.ComPointer.Interface ) ;
	ComPtr< ID3D12DeviceChild >? IUnknownWrapper< ID3D12DeviceChild >.ComPointer => new( this.ComPointer.Interface ) ;

	ID3D12Pageable? IPageable.COMObject => COMObject ;
	ID3D12DeviceChild? IDeviceChild.COMObject => COMObject ;
	ID3D12Pageable? IComObjectRef< ID3D12Pageable >.COMObject => COMObject ;
	ID3D12DeviceChild? IComObjectRef< ID3D12DeviceChild >.COMObject => COMObject ;
	
	
	public new ID3D12Fence? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Fence >? ComPointer { get ; protected set ; }
	
	internal Fence( ) { }
	internal Fence( ComPtr< ID3D12Fence > comPtr ) => ComPointer = comPtr ;
	internal Fence( nint address ) => ComPointer = new( address ) ;
	internal Fence( ID3D12Fence comObject ) => ComPointer = new( comObject ) ;

}