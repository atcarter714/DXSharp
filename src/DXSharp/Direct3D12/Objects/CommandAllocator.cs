#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12CommandAllocator))]
internal class CommandAllocator: Pageable,
								 ICommandAllocator,
								 IComObjectRef< ID3D12CommandAllocator >,
								 IUnknownWrapper< ID3D12CommandAllocator > {
	// -------------------------------------------------------------------------------------------------------
	ComPtr< ID3D12CommandAllocator >? _comPtr ;
	public override ID3D12CommandAllocator? ComObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12CommandAllocator >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12CommandAllocator>(  ) ;
	// -------------------------------------------------------------------------------------------------------
	
	internal CommandAllocator( ) {
		_comPtr = ComResources?.GetPointer<ID3D12CommandAllocator>(  ) ;
	}
	internal CommandAllocator( nint childAddr ) {
		_comPtr = new( childAddr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal CommandAllocator( ID3D12CommandAllocator child ) {
		_comPtr = new( child ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal CommandAllocator( ComPtr< IUnknown > childPtr ) => _initOrAdd( _comPtr! ) ;
	
	// -------------------------------------------------------------------------------------------------------
	
	public void Reset( ) => ComObject!.Reset( ) ;
	
	// -------------------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12CommandAllocator) ;
	
	public new static ref readonly Guid Guid {
    	[MethodImpl( MethodImplOptions.AggressiveInlining )]
    	get {
    		ReadOnlySpan< byte > data = typeof(ID3D12CommandAllocator).GUID
    																	   .ToByteArray( ) ;
    		
    		return ref Unsafe
    				   .As< byte, Guid >( ref MemoryMarshal
    										  .GetReference(data) ) ;
    	}
    }
		
	// =======================================================================================================
} ;