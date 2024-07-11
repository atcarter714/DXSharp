#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Pageable))]
internal abstract class Pageable: DeviceChild,
								  IPageable,
								  IComObjectRef< ID3D12Pageable >,
								  IUnknownWrapper< ID3D12Pageable > {
	ComPtr< ID3D12Pageable >? _comPtr ;
	public new virtual ComPtr< ID3D12Pageable >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12Pageable>(  ) ;
	
	public override ID3D12Pageable? ComObject => ComPointer?.Interface ;
	
	protected Pageable( ) {
		_comPtr = ComResources?.GetPointer<ID3D12Pageable>(  ) ;
	}
	protected Pageable( nint childAddr ) {
		_comPtr = new( childAddr ) ;
		_initOrAdd( _comPtr ) ;
	}
	protected Pageable( ID3D12Pageable child ) {
		_comPtr = new( child ) ;
		_initOrAdd( _comPtr ) ;
	}
	protected Pageable( ComPtr< IUnknown > childPtr ) => _initOrAdd( _comPtr! ) ;


	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Pageable).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	public new static Type ComType => typeof(ID3D12Pageable) ;
} ;
