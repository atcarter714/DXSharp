using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12PipelineLibrary))]
internal class PipelineLibrary: DeviceChild,
							  IPipelineLibrary {
	
	public new ID3D12PipelineLibrary? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12PipelineLibrary >? ComPointer { get ; protected set ; }
	
	internal PipelineLibrary( ) { }
	internal PipelineLibrary( ComPtr< ID3D12PipelineLibrary > comPtr ) => ComPointer = comPtr ;
	internal PipelineLibrary( nint address ) => ComPointer = new( address ) ;
	internal PipelineLibrary( ID3D12PipelineLibrary comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
}

[Wrapper(typeof(ID3D12PipelineLibrary1))]
internal class PipelineLibrary1: PipelineLibrary, 
							   IPipelineLibrary1 {
	public new ID3D12PipelineLibrary1? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12PipelineLibrary1 >? ComPointer { get ; protected set ; }
	
	internal PipelineLibrary1( ) { }
	internal PipelineLibrary1( ComPtr< ID3D12PipelineLibrary1 > comPtr ) => ComPointer = comPtr ;
	internal PipelineLibrary1( nint address ) => ComPointer = new( address ) ;
	internal PipelineLibrary1( ID3D12PipelineLibrary1 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;