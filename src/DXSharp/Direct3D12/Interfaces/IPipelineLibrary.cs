#region Using Directives

using System.Collections.ObjectModel ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>
/// Manages a pipeline library, which is a collection of pipeline state objects
/// (PSOs) that can be retrieved or loaded by name.
/// </summary>
[ProxyFor(typeof(ID3D12PipelineLibrary))]
public interface IPipelineLibrary: IDeviceChild, IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	internal static readonly ReadOnlyDictionary< Guid, Func<ID3D12PipelineLibrary, IInstantiable> > _pipelineLibraryCreationFunctions =
		new( new Dictionary<Guid, Func<ID3D12PipelineLibrary, IInstantiable> > {
			{ IPipelineLibrary.IID, ( pComObj ) => new PipelineLibrary( pComObj ) },
			{ IPipelineLibrary1.IID, ( pComObj ) => new PipelineLibrary1( (pComObj as ID3D12PipelineLibrary1)! ) },
		} ) ;

	// ---------------------------------------------------------------------------------

	

	/// <summary>Adds the input PSO to an internal database with the corresponding name.</summary>
	/// <param name="pName">
	/// <para>Type: <b>LPCWSTR</b> Specifies a unique name for the library. Overwriting is not supported.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-storepipeline#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pPipeline">
	/// <para>Type: <b>ID3D12PipelineState*</b> Specifies the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> to add.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-storepipeline#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code, including E_INVALIDARG if the name already exists, E_OUTOFMEMORY if unable to allocate storage in the library.</para>
	/// </returns>
	/// <remarks>Refer to the remarks and examples for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary">CreatePipelineLibrary</a>.</remarks>
	void StorePipeline( string pName, ID3D12PipelineState pPipeline ) ;


	/// <summary>Retrieves the requested PSO from the library.</summary>
	/// <param name="pName">
	/// <para>Type: <b>LPCWSTR</b> The unique name of the PSO.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-loadgraphicspipeline#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_graphics_pipeline_state_desc">D3D12_GRAPHICS_PIPELINE_STATE_DESC</a>*</b> Specifies a description of the required PSO in a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_graphics_pipeline_state_desc">D3D12_GRAPHICS_PIPELINE_STATE_DESC</a> structure. This input description is matched against the data in the current library database, and stored in order to prevent duplication of PSO contents.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-loadgraphicspipeline#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> Specifies a REFIID for the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> object. Typically set this, and the following parameter, with the macro <c>IID_PPV_ARGS(&amp;PSO1)</c>, where <i>PSO1</i> is the name of the object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-loadgraphicspipeline#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppPipelineState">
	/// <para>Type: <b>void**</b> Specifies a pointer that will reference the returned PSO.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-loadgraphicspipeline#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code, which can include E_INVALIDARG if the name doesn’t exist, or if the input description doesn’t match the data in the library, and E_OUTOFMEMORY if unable to allocate the return PSO.</para>
	/// </returns>
	/// <remarks>Refer to the remarks and examples for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary">CreatePipelineLibrary</a>.</remarks>
	void LoadGraphicsPipeline( string                               pName,
							   in  GraphicsPipelineStateDescription pDesc,
							   in  Guid                             riid,
							   out object                           ppPipelineState ) ;


	/// <summary>Retrieves the requested PSO from the library. The input desc is matched against the data in the current library database, and remembered in order to prevent duplication of PSO contents.</summary>
	/// <param name="pName">
	/// <para>Type: <b>LPCWSTR</b> The unique name of the PSO.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-loadcomputepipeline#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_compute_pipeline_state_desc">D3D12_COMPUTE_PIPELINE_STATE_DESC</a>*</b> Specifies a description of the required PSO in a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_compute_pipeline_state_desc">D3D12_COMPUTE_PIPELINE_STATE_DESC</a> structure. This input description is matched against the data in the current library database, and stored in order to prevent duplication of PSO contents.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-loadcomputepipeline#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> Specifies a REFIID for the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> object. Typically set this, and the following parameter, with the macro <c>IID_PPV_ARGS(&amp;PSO1)</c>, where <i>PSO1</i> is the name of the object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-loadcomputepipeline#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppPipelineState">
	/// <para>Type: <b>void**</b> Specifies a pointer that will reference the returned PSO.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-loadcomputepipeline#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code, which can include E_INVALIDARG if the name doesn’t exist, or if the input description doesn’t match the data in the library, and E_OUTOFMEMORY if unable to allocate the return PSO.</para>
	/// </returns>
	/// <remarks>Refer to the remarks and examples for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary">CreatePipelineLibrary</a>.</remarks>
	void LoadComputePipeline( string                              pName,
							  in  ComputePipelineStateDescription pDesc,
							  in  Guid                            riid,
							  out object                          ppPipelineState ) ;


	/// <summary>Returns the amount of memory required to serialize the current contents of the database.</summary>
	/// <returns>
	/// <para>Type: <b>SIZE_T</b> This method returns a SIZE_T object, containing the size required in bytes.</para>
	/// </returns>
	/// <remarks>Refer to the remarks and examples for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary">CreatePipelineLibrary</a>.</remarks>
	nuint GetSerializedSize( ) ;


	/// <summary>Writes the contents of the library to the provided memory, to be provided back to the runtime at a later time.</summary>
	/// <param name="pData">
	/// <para>Type: <b>void*</b> Specifies a pointer to the data. This memory must be readable and writable up to the input size. This data can be saved and provided to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary">CreatePipelineLibrary</a> at a later time, including future instances of this or other processes. The data becomes invalidated if the runtime or driver is updated, and is not portable to other hardware or devices.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-serialize#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DataSizeInBytes">
	/// <para>Type: <b>SIZE_T</b> The size provided must be at least the size returned from <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12pipelinelibrary-getserializedsize">GetSerializedSize</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-serialize#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code, including E_INVALIDARG if the buffer provided isn’t big enough.</para>
	/// </returns>
	/// <remarks>Refer to the remarks and examples for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary">CreatePipelineLibrary</a>.</remarks>
	void Serialize( nint pData, nuint DataSizeInBytes ) ;
	
	// ---------------------------------------------------------------------------------
	
	new static Type ComType => typeof(ID3D12PipelineLibrary) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12PipelineLibrary).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new PipelineLibrary( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new PipelineLibrary( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom obj ) => new PipelineLibrary( ( obj as ID3D12PipelineLibrary )! ) ;
	// ==================================================================================
} ;



/// <summary>
/// Manages a pipeline library, which is a collection of pipeline state objects
/// (PSOs) that can be retrieved or loaded by name.
/// </summary>
[ProxyFor( typeof( ID3D12PipelineLibrary1 ) )]
public interface IPipelineLibrary1: IPipelineLibrary {
	// ---------------------------------------------------------------------------------

	/// <summary>Retrieves the requested PSO from the library. The pipeline stream description is matched against the library database and remembered in order to prevent duplication of PSO contents.</summary>
	/// <param name="pName"><para>The unique name of the PSO.</para></param>
	/// <param name="pDesc">Describes the required PSO using a <see cref="PipelineStateStreamDescription"/> structure.
	/// This description is matched against the library database and stored in order to prevent duplication of PSO contents.
	/// </param>
	/// <param name="riid">
	/// Specifies a REFIID for the ID3D12PipelineStateState object. Applications should typically set this argument and the following argument, ppPipelineState,
	/// by using the macro IID_PPV_ARGS(&amp;PSO1), where PSO1 is the name of the object.
	/// </param>
	/// <param name="ppPipelineState">Specifies the pointer that will reference the PSO after the function successfully returns.</param>
	/// <returns>
	/// <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b>
	/// This method returns an HRESULT success or error code, which can include E_INVALIDARG if the name doesn't exist or the stream description doesn't match the
	/// data in the library, and E_OUTOFMEMORY if the function is unable to allocate the resulting PSO.
	/// </returns>
	/// <remarks>
	/// This function takes the pipeline description as a <see cref="PipelineStateStreamDescription"/> and is a replacement for the
	/// <see cref="IPipelineLibrary.LoadGraphicsPipeline"/> and <see cref="IPipelineLibrary.LoadComputePipeline"/> functions,
	/// which take their pipeline description as the less-flexible <see cref="GraphicsPipelineStateDescription"/> and 
	/// <see cref="ComputePipelineStateDescription"/> structs, respectively.
	/// </remarks>
	void LoadPipeline( string  pName,
					   in  PipelineStateStreamDescription pDesc,
					   in Guid riid,
					   out IPipelineState ppPipelineState ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12PipelineLibrary1) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12PipelineLibrary1).GUID.ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new PipelineLibrary1( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new PipelineLibrary1( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom obj ) => new PipelineLibrary1( ( obj as ID3D12PipelineLibrary1 )! ) ;
	// ==================================================================================
}