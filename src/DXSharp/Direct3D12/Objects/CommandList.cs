#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12CommandList))]
internal class CommandList: DeviceChild,
							ICommandList,
							IComObjectRef< ID3D12CommandList >,
							IUnknownWrapper< ID3D12CommandList > {
	// ------------------------------------------------------------------------------------------
	ComPtr< ID3D12CommandList >? _comPtr ;
	public new ComPtr< ID3D12CommandList >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12CommandList>(  ) ;

	public override ID3D12CommandList? COMObject => ComPointer?.Interface ;
	// ------------------------------------------------------------------------------------------
	
	internal CommandList( ) {
		_comPtr = ComResources?.GetPointer<ID3D12CommandList>(  ) ;
	}
	internal CommandList( nint childAddr ) {
		_comPtr = new( childAddr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal CommandList( ID3D12CommandList child ) {
		_comPtr = new( child ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ------------------------------------------------------------------------------------------

	public CommandListType GetListType( ) => (CommandListType)COMObject!.GetType( ) ;

	// ------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12CommandList) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12CommandList).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// ------------------------------------------------------------------------------------------
} ;