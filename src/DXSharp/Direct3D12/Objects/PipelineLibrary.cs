using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;


[Wrapper( typeof( ID3D12PipelineLibrary ) )]
internal class PipelineLibrary: DeviceChild,
								IPipelineLibrary,
								IComObjectRef< ID3D12PipelineLibrary >,
								IUnknownWrapper< ID3D12PipelineLibrary > {
	// -----------------------------------------------------------------------------------------------
	ComPtr< ID3D12PipelineLibrary >? _comPtr ;

	public new ComPtr< ID3D12PipelineLibrary >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12PipelineLibrary >( ) ;

	public override ID3D12PipelineLibrary? COMObject => ComPointer?.Interface ;
	// -----------------------------------------------------------------------------------------------

	internal PipelineLibrary( ) {
		_comPtr = ComResources?.GetPointer< ID3D12PipelineLibrary >( ) ;
	}

	internal PipelineLibrary( ComPtr< ID3D12PipelineLibrary > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}

	internal PipelineLibrary( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}

	internal PipelineLibrary( ID3D12PipelineLibrary comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}

	// -----------------------------------------------------------------------------------------------

	public void StorePipeline( string pName, ID3D12PipelineState pPipeline ) {
		using PCWSTR _name = pName ;
		COMObject!.StorePipeline( _name, pPipeline ) ;
	}
	
	public void StorePipeline( string pName, IPipelineState pPipeline ) {
		using PCWSTR _name    = pName ;
		var          pipeline = (IComObjectRef< ID3D12PipelineState >)pPipeline ;
		COMObject!.StorePipeline( _name, pipeline.COMObject ) ;
	}

	public void LoadGraphicsPipeline( string                               pName,
									  in  GraphicsPipelineStateDescription pDesc,
									  in  Guid                             riid,
									  out object                           ppPipelineState ) {
		var lib = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( void* pRiid = &riid ) {
				using PCWSTR _name = pName ;
				lib.LoadGraphicsPipeline( _name, pDesc, (Guid*)pRiid, out var pso ) ;
				ppPipelineState = new PipelineState( (ID3D12PipelineState)pso ) ;
			}
		}
	}

	public void LoadComputePipeline( string                              pName,
									 in  ComputePipelineStateDescription pDesc,
									 in  Guid                            riid,
									 out object                          ppPipelineState ) {
		var lib = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( void* pRiid = &riid ) {
				using PCWSTR _name = pName ;
				lib.LoadComputePipeline( _name, pDesc, (Guid*)pRiid, out var pso ) ;
				ppPipelineState = new PipelineState( (ID3D12PipelineState)pso ) ;
			}
		}
	}

	public nuint GetSerializedSize( ) {
		var lib = COMObject ?? throw new NullReferenceException( ) ;
		return lib.GetSerializedSize( ) ;
	}


	public void Serialize( nint pData, nuint DataSizeInBytes ) {
		var lib = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			lib.Serialize( (void*)pData, DataSizeInBytes ) ;
		}
	}

	// -----------------------------------------------------------------------------------------------

	public new static Type ComType => typeof( ID3D12PipelineLibrary ) ;

	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12PipelineLibrary ).GUID
																	   .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}

	// =======================================================================================================
} ;

[Wrapper(typeof(ID3D12PipelineLibrary1))]
internal class PipelineLibrary1: PipelineLibrary,
								 IPipelineLibrary1,
								 IComObjectRef< ID3D12PipelineLibrary1 >, 
								 IUnknownWrapper< ID3D12PipelineLibrary1 > {
	// -----------------------------------------------------------------------------------------------

	ComPtr< ID3D12PipelineLibrary1 >? _comPtr ;
	public new ComPtr< ID3D12PipelineLibrary1 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12PipelineLibrary1 >( ) ;
	
	public override ID3D12PipelineLibrary1? COMObject => ComPointer?.Interface ;
	
	// -----------------------------------------------------------------------------------------------

	internal PipelineLibrary1( ) {
		_comPtr = ComResources?.GetPointer< ID3D12PipelineLibrary1 >( ) ;
	}
	internal PipelineLibrary1( ComPtr< ID3D12PipelineLibrary1 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal PipelineLibrary1( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal PipelineLibrary1( ID3D12PipelineLibrary1 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -----------------------------------------------------------------------------------------------
	
	public void LoadPipeline( string  pName,
							  in  PipelineStateStreamDescription pDesc,
							  in Guid riid,
							  out IPipelineState ppPipelineState ) {
		var lib1 = COMObject ?? throw new NullReferenceException( ) ;
		unsafe { fixed ( void* pRiid = &riid, _desc = &pDesc ) {
				using PCWSTR _name = pName ;
				lib1.LoadPipeline( _name, (D3D12_PIPELINE_STATE_STREAM_DESC *)_desc,
								   (Guid *)pRiid, out var pso ) ;
				
				ppPipelineState = new PipelineState( (ID3D12PipelineState)pso ) ;
			}
		}
	}
	
	// -----------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof( ID3D12PipelineLibrary1 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12PipelineLibrary1 ).GUID
																	   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// ================================================================================================
} ;