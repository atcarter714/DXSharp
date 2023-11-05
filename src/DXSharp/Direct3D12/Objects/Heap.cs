#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Heap))]
internal class Heap: Pageable,
				   IHeap,
				   IComObjectRef< ID3D12Heap >,
				   IUnknownWrapper< ID3D12Heap > {
	// ------------------------------------------------------------------------------------------
	ComPtr< ID3D12Heap >? _comPtr ;
	public override ID3D12Heap? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Heap >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12Heap>( ) ;
	// ------------------------------------------------------------------------------------------
	
	internal Heap( ) {
		 _comPtr = ComResources?.GetPointer< ID3D12Heap >( ) ;
	}
	 
	internal Heap( ComPtr< ID3D12Heap > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	
	internal Heap( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	internal Heap( ID3D12Heap comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ------------------------------------------------------------------------------------------
	
	public HeapDescription GetDesc( ) => COMObject!.GetDesc( ) ;
	
	// ------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12Heap) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Heap).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// ==========================================================================================
} ;


[Wrapper( typeof( ID3D12Heap1 ) )]
internal class Heap1: Heap,
					  IHeap1,
					  IComObjectRef< ID3D12Heap1 >,
					  IUnknownWrapper< ID3D12Heap1 > {
	ComPtr<ID3D12Heap1>? _comPtr ;
	public new virtual ComPtr<ID3D12Heap1>? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12Heap1 >( ) ;
	public override ID3D12Heap1? COMObject => ComPointer?.Interface ;
	
	// ------------------------------------------------------------------------------------------
	
	internal Heap1( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Heap1 >( ) ;
	}
	internal Heap1( ComPtr< ID3D12Heap1 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Heap1( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Heap1( ID3D12Heap1 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}

	// ------------------------------------------------------------------------------------------

	public void GetProtectedResourceSession( in Guid riid, out IProtectedResourceSession ppProtectedSession ) {
		var heap = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( Guid* pRiid = &riid ) {
				heap.GetProtectedResourceSession( pRiid, out var ppv ) ;
				ppProtectedSession = new ProtectedResourceSession( (ID3D12ProtectedResourceSession)ppv ) ;
			}
		}
	}
	
	// ------------------------------------------------------------------------------------------

	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Heap1).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	public new static Type ComType => typeof(ID3D12Heap1) ;
	
	// ==========================================================================================
} ;