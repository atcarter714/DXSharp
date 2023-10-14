using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12.Objects ;


public class Resource: Object, IResource,
					   IInstantiable< Resource > {
	
	public new ID3D12Resource? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Resource >? ComPointer { get ;  protected set ; }
	
	internal Resource( ) { }
	internal Resource( ComPtr<ID3D12Resource> ptr ) => ComPointer = ptr ;
	internal Resource( ID3D12Resource _interface ) => ComPointer = new( _interface ) ;
	internal Resource( nint address ) => ComPointer = new( address ) ;
	


	static Resource IInstantiable< Resource >.Instantiate( ) => new( ) ;
	static IDXCOMObject IInstantiable.Instantiate( ) => new Resource( ) ;
	
	
	//ID3D12Resource? _cachedObject ;
	ID3D12Pageable? IPageable.COMObject => COMObject ;
	ID3D12DeviceChild? IDeviceChild.COMObject => COMObject ;
	ID3D12Pageable? IComObjectRef< ID3D12Pageable >.COMObject => COMObject ;
	ID3D12DeviceChild? IComObjectRef< ID3D12DeviceChild >.COMObject => COMObject ;
	
	ComPtr< ID3D12Pageable >? IPageable.ComPointer => new( COMObject! ) ;
	ComPtr< ID3D12DeviceChild >? IDeviceChild.ComPointer => new( COMObject! ) ;
	ComPtr< ID3D12DeviceChild >? IUnknownWrapper< ID3D12DeviceChild >
		.ComPointer => new( COMObject! ) ;

	ComPtr< ID3D12Pageable >? IUnknownWrapper< ID3D12Pageable >.ComPointer => new( COMObject! ) ;
	
} ;