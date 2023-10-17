using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

public class PipelineState: Pageable, IPipelineState {
	public new static Type ComType => typeof( ID3D12PipelineState ) ;
	public new static Guid InterfaceGUID => typeof( ID3D12PipelineState ).GUID ;
	
	// ------------------------------------------------------------------------------------------
	public new ID3D12PipelineState? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12PipelineState >? ComPointer { get ; protected set ; }
	// ------------------------------------------------------------------------------------------
	
	internal PipelineState( ) { }
	internal PipelineState( nint ptr ) => ComPointer = new( ptr ) ;
	internal PipelineState( ComPtr< ID3D12PipelineState > comObject ) => ComPointer = comObject ;
	internal PipelineState( ID3D12PipelineState? comObject ) => ComPointer = new( comObject! ) ;
} ;