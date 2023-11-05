#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Resource))]
internal class Resource: Pageable,
						 IResource,
						 IComObjectRef< ID3D12Resource >,
						 IUnknownWrapper< ID3D12Resource > {
	// -----------------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Resource >? _comPtr ;
	public new virtual ComPtr< ID3D12Resource >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Resource >( ) ;
	public override ID3D12Resource? COMObject => ComPointer?.Interface ;
	
	// -----------------------------------------------------------------------------------------------------------------
	
	internal Resource( ) {
		 _comPtr = ComResources?.GetPointer< ID3D12Resource >( ) ;
	}
	internal Resource( nint ptr ) {
	_comPtr = new ComPtr< ID3D12Resource >( ptr ) ;
	_initOrAdd( _comPtr ) ;
	}
	internal Resource( ComPtr< ID3D12Resource > comPtr ) {
	_comPtr = comPtr ;
	_initOrAdd( _comPtr ) ;
	}
	internal Resource( ID3D12Resource ptr ) {
	_comPtr = new ComPtr< ID3D12Resource >( ptr ) ;
	_initOrAdd( _comPtr ) ;
	}
	
	// -----------------------------------------------------------------------------------------------------------------

	public void Map( uint Subresource, [Optional] in Range pReadRange, out nint ppData ) {
		unsafe { fixed( Range* pReadRangePtr = &pReadRange ) {
				void* pData = default ;
				COMObject!.Map( Subresource, (D3D12_RANGE *)pReadRangePtr, &pData ) ;
				ppData = (nint)pData! ;
			}
		}
	}
	
	public void Unmap( uint Subresource, [Optional] in Range pWrittenRange ) {
		unsafe { fixed( Range* pWrittenRangePtr = &pWrittenRange )
				COMObject!.Unmap( Subresource, (D3D12_RANGE *)pWrittenRangePtr ) ;
		}
	}
	
	public ResourceDescription GetDesc( ) => COMObject!.GetDesc( ) ;

	public ulong GetGPUVirtualAddress( ) => COMObject!.GetGPUVirtualAddress( ) ;
	
	public void WriteToSubresource( uint DstSubresource, out Box pDstBox, 
									nint pSrcData,       uint    SrcRowPitch, uint SrcDepthPitch ) {
		unsafe { fixed( Box* pDstBoxPtr = &pDstBox )
				COMObject!.WriteToSubresource( DstSubresource, (D3D12_BOX *)pDstBoxPtr,
											   (void *)pSrcData, SrcRowPitch, SrcDepthPitch ) ;
		}
	}
	
	public void ReadFromSubresource( nint    pDstData,      uint DstRowPitch,
									 uint    DstDepthPitch, uint SrcSubresource,
									 in Box? pSrcBox = null ) {
		Box _box = pSrcBox ?? default ;
		unsafe {
			Box* pSrcBoxPtr = &_box ;
			COMObject!.ReadFromSubresource( (void *)pDstData, DstRowPitch, DstDepthPitch,
										   SrcSubresource, (D3D12_BOX *)pSrcBoxPtr ) ;
		}
	}

	public void GetHeapProperties( out HeapProperties pHeapProperties, out HeapFlags pHeapFlags ) {
		unsafe {
			D3D12_HEAP_FLAGS _flags = 0 ;
			D3D12_HEAP_PROPERTIES _props = default ;
			COMObject!.GetHeapProperties( &_props, &_flags ) ;
			pHeapFlags = (HeapFlags)_flags ;
			pHeapProperties = _props ;
		}
	}
	
	
	// -----------------------------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12Resource) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Resource).GUID
															   .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// =================================================================================================================
} ;


[Wrapper(typeof(ID3D12Resource1))]
internal class Resource1: Resource,
						  IResource1,
						  IComObjectRef< ID3D12Resource1 >,
						  IUnknownWrapper< ID3D12Resource1 > {
	// -----------------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Resource1 >? _comPtr ;
	public new virtual ComPtr< ID3D12Resource1 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Resource1 >( ) ;
	public override ID3D12Resource1? COMObject => ComPointer?.Interface ;
	
	// -----------------------------------------------------------------------------------------------------------------
	
	internal Resource1( ) {
		 _comPtr = ComResources?.GetPointer< ID3D12Resource1 >( ) ;
	}
	internal Resource1( nint ptr ) {
	_comPtr = new ComPtr< ID3D12Resource1 >( ptr ) ;
	_initOrAdd( _comPtr ) ;
	}
	internal Resource1( ComPtr< ID3D12Resource1 > comPtr ) {
	_comPtr = comPtr ;
	_initOrAdd( _comPtr ) ;
	}
	internal Resource1( ID3D12Resource1 ptr ) {
	_comPtr = new ComPtr< ID3D12Resource1 >( ptr ) ;
	_initOrAdd( _comPtr ) ;
	}
	
	// -----------------------------------------------------------------------------------------------------------------
	
	public void GetProtectedResourceSession( in Guid riid, out IProtectedSession ppProtectedSession ) {
		unsafe {
			var guid = riid ;
			COMObject!.GetProtectedResourceSession( &guid, out var _session ) ;
#if DEBUG || DEBUG_COM || DEV_BUILD
			if( _session is null ) throw new NullReferenceException( ) ;
#endif
			
			ppProtectedSession = new ProtectedSession( ( _session as ID3D12ProtectedResourceSession )! ) ;
		}
	}
	
	// -----------------------------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12Resource1) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Resource1).GUID
															   .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// =================================================================================================================
} ;


[Wrapper(typeof(ID3D12Resource2))]
internal class Resource2: Resource1,
						  IResource2,
						  IComObjectRef< ID3D12Resource2 >,
						  IUnknownWrapper< ID3D12Resource2 > {
	// -----------------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Resource2 >? _comPtr ;
	public new virtual ComPtr< ID3D12Resource2 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Resource2 >( ) ;
	public override ID3D12Resource2? COMObject => ComPointer?.Interface ;
	
	// -----------------------------------------------------------------------------------------------------------------
	
	internal Resource2( ) {
		 _comPtr = ComResources?.GetPointer< ID3D12Resource2 >( ) ;
	}
	internal Resource2( nint ptr ) {
	_comPtr = new ComPtr< ID3D12Resource2 >( ptr ) ;
	_initOrAdd( _comPtr ) ;
	}
	internal Resource2( ComPtr< ID3D12Resource2 > comPtr ) {
	_comPtr = comPtr ;
	_initOrAdd( _comPtr ) ;
	}
	internal Resource2( ID3D12Resource2 ptr ) {
	_comPtr = new ComPtr< ID3D12Resource2 >( ptr ) ;
	_initOrAdd( _comPtr ) ;
	}
	
	// -----------------------------------------------------------------------------------------------------------------
	
	public ResourceDescription1 GetDesc1( ) => COMObject!.GetDesc1( ) ;
	
	// -----------------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Resource2) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Resource2).GUID
															   .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// =================================================================================================================
} ;