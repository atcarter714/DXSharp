#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12CommandSignature))]
internal class CommandSignature: Pageable,
								 ICommandSignature,
								 IComObjectRef< ID3D12CommandSignature >,
								 IUnknownWrapper< ID3D12CommandSignature > {
	// -------------------------------------------------------------------------------------------------------
	ComPtr< ID3D12CommandSignature >? _comPtr ;
	public override ID3D12CommandSignature? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12CommandSignature >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer<ID3D12CommandSignature>( ) ;
	// -------------------------------------------------------------------------------------------------------
	
	
	internal CommandSignature( ) { }
	internal CommandSignature( nint childAddr ): base( childAddr ) { }
	internal CommandSignature( ID3D12Pageable child ): base( child ) { }
	internal CommandSignature( ComPtr< IUnknown > childPtr ): base( childPtr ) { }
	
	
	// -------------------------------------------------------------------------------------------------------
	
	public new static ref readonly Guid Guid {
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12CommandSignature).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	public new static Type ComType => typeof(ID3D12CommandSignature) ;
	// =======================================================================================================
} ;