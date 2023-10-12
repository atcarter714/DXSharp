using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;

namespace DXSharp.Direct3D12 ;

[Wrapper(typeof(ID3D12PipelineState))]
public interface IPipelineState: IPageable< ID3D12PipelineState > {
	/// <summary>Gets the cached blob representing the pipeline state.</summary>
	/// <param name="ppBlob">
	/// <para>Type: <b>ID3DBlob**</b> After this method returns, points to the cached blob representing the pipeline state.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinestate-getcachedblob#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>Refer to the remarks for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cached_pipeline_state">D3D12_CACHED_PIPELINE_STATE</a>.</remarks>
	void GetCachedBlob( out ID3DBlob ppBlob ) ;
} ;
