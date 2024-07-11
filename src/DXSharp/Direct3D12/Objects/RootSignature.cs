#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12RootSignature))]
internal class RootSignature: DeviceChild,
							  IRootSignature,
							  IComObjectRef< ID3D12RootSignature >,
							  IUnknownWrapper< ID3D12RootSignature > {
	// ------------------------------------------------------------------------------------------
	ComPtr< ID3D12RootSignature >? _comPtr ;
	public new virtual ComPtr< ID3D12RootSignature >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12RootSignature >( ) ;
	public override ID3D12RootSignature? ComObject => ComPointer?.Interface ;
	
	// ------------------------------------------------------------------------------------------

	internal RootSignature( ) {
		_comPtr = ComResources?.GetPointer< ID3D12RootSignature >( ) ;
		_initOrAdd( _comPtr! ) ;
	}
	internal RootSignature( nint childAddr ) {
		_comPtr = new( childAddr ) ;
		_initOrAdd( _comPtr! ) ;
	}
	internal RootSignature( ID3D12RootSignature child ) {
		_comPtr = new( child ) ;
		_initOrAdd( _comPtr! ) ;
	}
	internal RootSignature( ComPtr< ID3D12RootSignature > childPtr ) => _initOrAdd( _comPtr = childPtr ) ;
	
	// ------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( ID3D12RootSignature ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12RootSignature).GUID
																 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==========================================================================================
} ;