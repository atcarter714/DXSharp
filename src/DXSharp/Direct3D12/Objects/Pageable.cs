using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

public abstract class Pageable: DeviceChild, IPageable {
	ComPtr< ID3D12Pageable >? IPageable.ComPointer => ComPointer ;
	public new ID3D12Pageable? COMObject => ComPointer?.Interface ;
	public new virtual ComPtr< ID3D12Pageable >? ComPointer { get ; }
	
	ID3D12DeviceChild? IDeviceChild.COMObject => COMObject ;
	ID3D12DeviceChild? IComObjectRef< ID3D12DeviceChild >.COMObject => COMObject ;
	ComPtr< ID3D12DeviceChild >? IDeviceChild.ComPointer => 
		new( ComPointer?.Interface as ID3D12DeviceChild
			 ?? throw new ObjectDisposedException( nameof(ComPointer) ) ) ;
	
	protected Pageable( ) { }
	protected Pageable( nint childAddr ): base( childAddr ) => 
		ComPointer = new( childAddr ) ;
	protected Pageable( ID3D12Pageable child ): base( child ) => 
		ComPointer = new( child ) ;
	protected Pageable( ComPtr< IUnknown > childPtr ):
		base( childPtr.Interface as ID3D12Pageable ?? throw new InvalidCastException() ) => 
		ComPointer = (ComPtr< ID3D12Pageable >?)childPtr ?? throw new InvalidCastException() ;
} ;

	//public override ComPtr? ComPtrBase => ComPointer ;
	/*public ID3D12CommandSignature? COMObject => ComPointer?.Interface ;
	public IntPtr PointerToIUnknown => ComPointer?.BaseAddress ?? 0x0000 ;
	public ComPtr< ID3D12CommandSignature >? ComPointer { get ; protected set ; }
	
	
	ComPtr< ID3D12DeviceChild > _comPointer ;
	ComPtr< ID3D12Pageable >    _comPointer1 ;*/