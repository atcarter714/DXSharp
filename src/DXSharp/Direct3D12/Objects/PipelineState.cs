#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12PipelineState))]
internal class PipelineState: Pageable,
							  IPipelineState,
							  IComObjectRef< ID3D12PipelineState >,
							  IUnknownWrapper< ID3D12PipelineState > {
	// ------------------------------------------------------------------------------------------
	ComPtr< ID3D12PipelineState >? _comPtr ;
	public new ComPtr< ID3D12PipelineState >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12PipelineState >( ) ;
	public override ID3D12PipelineState? COMObject => ComPointer?.Interface ;
	// ------------------------------------------------------------------------------------------
	
	internal PipelineState( ) {
		_comPtr = ComResources?.GetPointer< ID3D12PipelineState >( ) ;
	}
	internal PipelineState( ComPtr< ID3D12PipelineState > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal PipelineState( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal PipelineState( ID3D12PipelineState comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ------------------------------------------------------------------------------------------

	public IBlob GetCachedBlob( ) {
		COMObject!.GetCachedBlob( out var _blob ) ;
		return new Blob( _blob ) ;
	}
	
	// ------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( ID3D12PipelineState ) ;
	public new static ref readonly Guid Guid {
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12PipelineState ).GUID
																	 .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ==========================================================================================
} ;