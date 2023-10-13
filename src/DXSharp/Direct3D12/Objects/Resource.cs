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
}


	/*public void Map( uint Subresource, in Range pReadRange, out nint ppData ) {
		unsafe { fixed( Range* _pRange = &pReadRange ) {
				void* _pData = default ;
				COMObject.Map( Subresource, (D3D12_RANGE *)_pRange, &_pData ) ;
				ppData = (nint)_pData! ;
			}
		}
	}

	public void Unmap( uint Subresource, in Range pWrittenRange ) {
		unsafe { fixed ( Range* _pRange = &pWrittenRange ) {
				void* _pData = default ;
				COMObject.Unmap( Subresource, (D3D12_RANGE*)_pRange ) ;
			}
		}
	}

	public ResourceDescription GetDesc( ) => COMObject.GetDesc( ) ;
	

	public ulong GetGPUVirtualAddress() {
		throw new NotImplementedException( ) ;
	}

	public void WriteToSubresource( uint DstSubresource, out Box pDstBox, IntPtr pSrcData, uint SrcRowPitch, uint SrcDepthPitch ) {
		throw new NotImplementedException( ) ;
	}

	public void ReadFromSubresource( nint pDstData, uint DstRowPitch, uint DstDepthPitch, uint SrcSubresource,
									 in Box? pSrcBox = null ) {
		throw new NotImplementedException( ) ;
	}

	public void GetHeapProperties( out HeapProperties pHeapProperties, out HeapFlags pHeapFlags ) {
		unsafe {
			D3D12_HEAP_FLAGS _flags = 0 ;
			D3D12_HEAP_PROPERTIES _props = default ;
			COMObject.GetHeapProperties( &_props, &_flags ) ;
			pHeapFlags = (HeapFlags)_flags ;
			pHeapProperties = _props ;
		}
	}*/