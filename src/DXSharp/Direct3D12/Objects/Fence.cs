#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Fence))]
internal class Fence: Pageable,
					  IFence,
					  IComObjectRef< ID3D12Fence >,
					  IUnknownWrapper< ID3D12Fence >  {
	// -----------------------------------------------------------------------------------------------
	ComPtr< ID3D12Fence >? _comPtr ;
	public override ID3D12Fence? COMObject => ComPointer?.Interface ;
	public new virtual ComPtr< ID3D12Fence >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12Fence>( ) ;
	// -----------------------------------------------------------------------------------------------

	internal Fence( ) {
		 _comPtr = ComResources?.GetPointer< ID3D12Fence >( ) ;
	}

	internal Fence( ComPtr< ID3D12Fence > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Fence( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Fence( ID3D12Fence comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -----------------------------------------------------------------------------------------------
	
	public ulong GetCompletedValue( ) => COMObject!.GetCompletedValue( ) ;

	public void SetEventOnCompletion( ulong Value, Win32Handle hEvent ) => 
		COMObject!.SetEventOnCompletion( Value, hEvent ) ;

	public void Signal( ulong Value ) => COMObject!.Signal( Value ) ;
	
	// -----------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Fence) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Fence).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ===============================================================================================
} ;


[Wrapper( typeof( ID3D12Fence1 ) )]
internal class Fence1: Fence,
					   IFence1,
					   IComObjectRef< ID3D12Fence1 >,
					   IUnknownWrapper< ID3D12Fence1 >  {
	// -----------------------------------------------------------------------------------------------
	ComPtr< ID3D12Fence1 >? _comPtr ;
	public override ID3D12Fence1? COMObject => ComPointer?.Interface ;
	public new virtual ComPtr< ID3D12Fence1 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12Fence1>( ) ;

	// -----------------------------------------------------------------------------------------------

	internal Fence1( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Fence1 >( ) ;
	}
	internal Fence1( ComPtr< ID3D12Fence1 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Fence1( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Fence1( ID3D12Fence1 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}

	// -----------------------------------------------------------------------------------------------
	
	public FenceFlags GetCreationFlags( ) {
		var fence = COMObject ?? throw new NullReferenceException( ) ;
		return (FenceFlags)fence.GetCreationFlags( ) ;
	}
	
	// -----------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Fence1) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Fence1).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ===============================================================================================

} ;