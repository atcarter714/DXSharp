#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.DXGI ;
#endregion
namespace DXSharp.Direct3D12 ;


// ===================================================================================
// ID3D12GraphicsCommandList interface
// ===================================================================================

[ProxyFor(typeof(ID3D12GraphicsCommandList))]
public interface IGraphicsCommandList: ICommandList {
	
	// ---------------------------------------------------------------------------------

	/// <summary>Indicates that recording to the command list has finished. (ID3D12GraphicsCommandList.Close)</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns <b>S_OK</b> if successful; otherwise, returns one of the following values:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>The runtime will validate that the command list has not previously been closed.  If an error was encountered during recording, the error code is returned here.  The runtime won't call the close device driver interface (DDI) in this case.</remarks>
	void Close( ) ;
	
	
	/// <summary>Resets a command list back to its initial state as if a new command list was just created. (ID3D12GraphicsCommandList.Reset)</summary>
	/// <param name="pAllocator">
	/// <para>Type: <b>ID3D12CommandAllocator*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandallocator">ID3D12CommandAllocator</a> object that the device creates command lists from.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-reset#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pInitialState">
	/// <para>Type: <b>ID3D12PipelineState*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> object that contains the initial pipeline state for the command list.  This is optional and can be NULL.  If NULL, the runtime sets a dummy initial pipeline state so that drivers don't have to deal with undefined state.  The overhead for this is low, particularly for a command list, for which the overall cost of recording the command list likely dwarfs the cost of one initial state setting.  So there is little cost in  not setting the initial pipeline state parameter if it isn't convenient. For bundles on the other hand, it might make more sense to try to set the initial state parameter since bundles are likely smaller overall and can be reused frequently.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-reset#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns <b>S_OK</b> if successful; otherwise, returns one of the following values:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>By using <b>Reset</b>, you can re-use command list tracking structures without any allocations. Unlike <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandallocator-reset">ID3D12CommandAllocator::Reset</a>, you can call <b>Reset</b> while the command list is still being executed. A typical pattern is to submit a command list and then immediately reset it to reuse the allocated memory for another command list. You can use <b>Reset</b> for both direct command lists and bundles.</para>
	/// <para>The command allocator that <b>Reset</b> takes as input can be associated with no more than one recording command list at a time.  The allocator type, direct command list or bundle, must match the type of command list that is being created.</para>
	/// <para>If a bundle doesn't specify a resource heap, it can't make changes to which descriptor tables are bound. Either way, bundles can't change the resource heap within the bundle. If a heap is specified for a bundle, the heap must match the calling 'parent' command list’s heap.</para>
	/// <para><h3><a id="Runtime_validation"></a><a id="runtime_validation"></a><a id="RUNTIME_VALIDATION"></a>Runtime validation</h3> Before an app calls <b>Reset</b>, the command list must be in the "closed" state.  <b>Reset</b> will fail if the command list isn't in the "closed" state.</para>
	/// <para><div class="alert"><b>Note</b>  If a call to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-close">ID3D12GraphicsCommandList::Close</a> fails, the command list can never be reset.  Calling <b>Reset</b> will result in the same error being returned that <b>ID3D12GraphicsCommandList::Close</b> returned. </div> <div> </div> After <b>Reset</b> succeeds, the command list is left in the "recording" state.  <b>Reset</b> will fail if it would cause the maximum concurrently recording command list limit, which is specified at device creation, to be exceeded.</para>
	/// <para>Apps must specify a command list allocator.  The runtime will ensure that an allocator is never associated with more than one recording command list at the same time.</para>
	/// <para><b>Reset</b> fails for bundles that are referenced by a not yet submitted command list.</para>
	/// <para><h3><a id="Debug_layer"></a><a id="debug_layer"></a><a id="DEBUG_LAYER"></a>Debug layer</h3> The debug layer will also track graphics processing unit (GPU) progress and issue an error if it can't prove that there are no outstanding executions of the command list.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-reset#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void Reset( ICommandAllocator pAllocator, IPipelineState pInitialState ) ;

	
	/// <summary>Resets the state of a direct command list back to the state it was in when the command list was created. (ID3D12GraphicsCommandList.ClearState)</summary>
	/// <param name="pPipelineState">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> object that contains the initial pipeline state for the command list.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearstate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>It is invalid to call <b>ClearState</b> on a bundle.  If an app calls <b>ClearState</b> on a bundle, the call to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-close">Close</a> will return <b>E_FAIL</b>.</para>
	/// <para>When <b>ClearState</b> is called, all currently bound resources are unbound.  The primitive topology is set to <a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/ne-d3dcommon-d3d_primitive_topology">D3D_PRIMITIVE_TOPOLOGY_UNDEFINED</a>.  Viewports, scissor rectangles, stencil reference value, and the blend factor are set to empty values (all zeros).  Predication is disabled.</para>
	/// <para>The app-provided pipeline state object becomes bound as the currently set pipeline state object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearstate#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ClearState( ID3D12PipelineState pPipelineState ) ;


	/// <summary>Draws non-indexed, instanced primitives.</summary>
	/// <param name="VertexCountPerInstance">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Number of vertices to draw.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawinstanced#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="InstanceCount">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Number of instances to draw.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawinstanced#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="StartVertexLocation">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Index of the first vertex.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawinstanced#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="StartInstanceLocation">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A value added to each index before reading per-instance data from a vertex buffer.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawinstanced#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>A draw API submits work to the rendering pipeline.</para>
	/// <para>Instancing might extend performance by reusing the same geometry to draw multiple objects in a scene. One example of instancing could be to draw the same object with different positions and colors.</para>
	/// <para>The vertex data for an instanced draw call typically comes from a vertex buffer that is bound to the pipeline. But, you could also provide the vertex data from a shader that has instanced data identified with a system-value semantic (SV_InstanceID).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawinstanced#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void DrawInstanced( uint VertexCountPerInstance, uint InstanceCount,
						uint StartVertexLocation,    uint StartInstanceLocation ) ;
	

	/// <summary>Draws indexed, instanced primitives.</summary>
	/// <param name="IndexCountPerInstance">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Number of indices read from the index buffer for each instance.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawindexedinstanced#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="InstanceCount">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Number of instances to draw.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawindexedinstanced#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="StartIndexLocation">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The location of the first index read by the GPU from the index buffer.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawindexedinstanced#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BaseVertexLocation">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">INT</a></b> A value added to each index before reading a vertex from the vertex buffer.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawindexedinstanced#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="StartInstanceLocation">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A value added to each index before reading per-instance data from a vertex buffer.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawindexedinstanced#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>A draw API submits work to the rendering pipeline. Instancing might extend performance by reusing the same geometry to draw multiple objects in a scene. One example of instancing could be to draw the same object with different positions and colors. Instancing requires multiple vertex buffers: at least one for per-vertex data and a second buffer for per-instance data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-drawindexedinstanced#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void DrawIndexedInstanced( uint IndexCountPerInstance, uint InstanceCount, 
							   uint StartIndexLocation, int BaseVertexLocation, 
							   uint StartInstanceLocation ) ;

	
	/// <summary>Executes a compute shader on a thread group.</summary>
	/// <param name="ThreadGroupCountX">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of groups dispatched in the x direction. <i>ThreadGroupCountX</i> must be less than or equal to D3D11_CS_DISPATCH_MAX_THREAD_GROUPS_PER_DIMENSION (65535).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-dispatch#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ThreadGroupCountY">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of groups dispatched in the y direction. <i>ThreadGroupCountY</i> must be less than or equal to D3D11_CS_DISPATCH_MAX_THREAD_GROUPS_PER_DIMENSION (65535).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-dispatch#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ThreadGroupCountZ">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of groups dispatched in the z direction.  <i>ThreadGroupCountZ</i> must be less than or equal to D3D11_CS_DISPATCH_MAX_THREAD_GROUPS_PER_DIMENSION (65535). In feature level 10 the value for <i>ThreadGroupCountZ</i> must be 1.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-dispatch#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>You call the <b>Dispatch</b> method to execute commands in a compute shader. A compute shader can be run on many threads in parallel, within a thread group. Index a particular thread, within a thread group using a 3D vector given by (x,y,z).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-dispatch#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void Dispatch( uint ThreadGroupCountX, uint ThreadGroupCountY, uint ThreadGroupCountZ ) ;

	
	/// <summary>Copies a region of a buffer from one resource to another.</summary>
	/// <param name="pDstBuffer">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> Specifies the destination <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copybufferregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DstOffset">
	/// <para>Type: <b>UINT64</b> Specifies a UINT64 offset (in bytes) into the destination resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copybufferregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcBuffer">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> Specifies the source  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copybufferregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="SrcOffset">
	/// <para>Type: <b>UINT64</b> Specifies a UINT64 offset (in bytes) into the source resource, to start the copy from.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copybufferregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumBytes">
	/// <para>Type: <b>UINT64</b> Specifies the number of bytes to copy.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copybufferregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>Consider using the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copyresource">CopyResource</a> method when copying an entire resource, and use this method for copying regions of a resource.</para>
	/// <para><b>CopyBufferRegion</b> may be used to initialize resources which alias the same heap memory. See <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createplacedresource">CreatePlacedResource</a> for more details.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copybufferregion#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CopyBufferRegion( IResource pDstBuffer, ulong DstOffset, 
						   IResource pSrcBuffer, ulong SrcOffset, 
						   ulong NumBytes ) ;
	

	/// <summary>This method uses the GPU to copy texture data between two locations. Both the source and the destination may reference texture data located within either a buffer resource or a texture resource.</summary>
	/// <param name="pDst">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_texture_copy_location">D3D12_TEXTURE_COPY_LOCATION</a>*</b> Specifies the destination <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_texture_copy_location">D3D12_TEXTURE_COPY_LOCATION</a>. The subresource referred to must be in the D3D12_RESOURCE_STATE_COPY_DEST state.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytextureregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DstX">
	/// <para>Type: <b>UINT</b> The x-coordinate of the upper left corner of the destination region.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytextureregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DstY">
	/// <para>Type: <b>UINT</b> The y-coordinate of the upper left corner of the destination region. For a 1D subresource, this must be zero.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytextureregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DstZ">
	/// <para>Type: <b>UINT</b> The z-coordinate of the upper left corner of the destination region. For a 1D or 2D subresource, this must be zero.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytextureregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_texture_copy_location">D3D12_TEXTURE_COPY_LOCATION</a>*</b> Specifies the source <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_texture_copy_location">D3D12_TEXTURE_COPY_LOCATION</a>. The subresource referred to must be in the D3D12_RESOURCE_STATE_COPY_SOURCE state.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytextureregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcBox">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_box">D3D12_BOX</a>*</b> Specifies an optional  D3D12_BOX that sets the size of the source texture to copy.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytextureregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>The source box must be within the size of the source resource. The destination offsets, (x, y, and z), allow the source box to be offset when writing into the destination resource; however, the dimensions of the source box and the offsets must be within the size of the resource. If you try and copy outside the destination resource or specify a source box that is larger than the source resource, the behavior of <b>CopyTextureRegion</b> is undefined. If you created a device that supports the <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-layers">debug layer</a>, the debug output reports an error on this invalid <b>CopyTextureRegion</b> call. Invalid parameters to <b>CopyTextureRegion</b> cause undefined behavior and might result in incorrect rendering, clipping, no copy, or even the removal of the rendering device.</para>
	/// <para>If the resources are buffers, all coordinates are in bytes; if the resources are textures, all coordinates are in texels. <b>CopyTextureRegion</b> performs the copy on the GPU (similar to a <c>memcpy</c> by the CPU). As a consequence, the source and destination resources:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytextureregion#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CopyTextureRegion( in TextureCopyLocation pDst, 
							uint DstX, uint DstY, uint DstZ, 
							in TextureCopyLocation pSrc,
							[Optional] in Box pSrcBox ) ;
	

	/// <summary>Copies the entire contents of the source resource to the destination resource.</summary>
	/// <param name="pDstResource">
	/// <para>Type: <b>ID3D12Resource*</b> A pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> interface that represents the destination resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copyresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcResource">
	/// <para>Type: <b>ID3D12Resource*</b> A pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> interface that represents the source resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copyresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><b>CopyResource</b> operations are performed on the GPU, and do not incur a significant CPU workload linearly dependent on the size of the data to copy. <b>CopyResource</b> can be used to initialize resources that alias the same heap memory. See <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource">CreatePlacedResource</a> for more details.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copyresource#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CopyResource( IResource pDstResource, IResource pSrcResource ) ;

	
	/// <summary>Copies tiles from buffer to tiled resource or vice versa. (ID3D12GraphicsCommandList.CopyTiles)</summary>
	/// <param name="pTiledResource">
	/// <para>Type: <b>ID3D12Resource*</b> A pointer to a tiled resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytiles#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pTileRegionStartCoordinate">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tiled_resource_coordinate">TiledResourceCoordinate</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tiled_resource_coordinate">TiledResourceCoordinate</a> structure that describes the starting coordinates of the tiled resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytiles#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pTileRegionSize">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tile_region_size">TileRegionSize</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tile_region_size">TileRegionSize</a> structure that describes the size of the tiled region.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytiles#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pBuffer">
	/// <para>Type: <b>ID3D12Resource*</b> A pointer to an <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> that represents a default, dynamic, or staging buffer.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytiles#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BufferStartOffsetInBytes">
	/// <para>Type: <b>UINT64</b> The offset in bytes into the buffer at <i>pBuffer</i> to start the operation.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytiles#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Flags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_tile_copy_flags">TileCopyFlags</a></b> A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_tile_copy_flags">TileCopyFlags</a>-typed values that are combined by using a bitwise OR operation and that identifies how to copy tiles.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytiles#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><b>CopyTiles</b> drops write operations to unmapped areas and handles read operations from unmapped areas (except on Tier_1 tiled resources, where reading and writing unmapped areas is invalid - refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_tiled_resources_tier">D3D12_TILED_RESOURCES_TIER</a>).</para>
	/// <para>If a copy operation involves writing to the same memory location multiple times because multiple locations in the destination resource are mapped to the same tile memory, the resulting write operations to multi-mapped tiles are non-deterministic and non-repeatable; that is, accesses to the tile memory happen in whatever order the hardware happens to execute the copy operation. The tiles involved in the copy operation can't include tiles that contain packed mipmaps or results of the copy operation are undefined. To transfer data to and from mipmaps that the hardware packs into the one-or-more tiles that constitute the packed mips, you must use the standard (that is, non-tile specific) copy APIs like <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytextureregion">CopyTextureRegion</a>. <b>CopyTiles</b> does copy data in a slightly different pattern than the standard copy methods. The memory layout of the tiles in the non-tiled buffer resource side of the copy operation is linear in memory within 64 KB tiles, which the hardware and driver swizzle and de-swizzle per tile as appropriate when they transfer to and from a tiled resource. For multisample antialiasing (MSAA) surfaces, the hardware and driver traverse each pixel's samples in sample-index order before they move to the next pixel. For tiles that are partially filled on the right side (for a surface that has a width not a multiple of tile width in pixels), the pitch and stride to move down a row is the full size in bytes of the number pixels that would fit across the tile if the tile was full. So, there can be a gap between each row of pixels in memory. Mipmaps that are smaller than a tile are not packed together in the linear layout, which might seem to be a waste of memory space, but as mentioned you can't use <b>CopyTiles</b> to copy to mipmaps that the hardware packs together. You can just use generic copy APIs, like <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytextureregion">CopyTextureRegion</a>, to copy small mipmaps individually.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytiles#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CopyTiles( IResource pTiledResource,
					in TiledResourceCoordinate pTileRegionStartCoordinate, 
					in TileRegionSize pTileRegionSize,
					IResource pBuffer,
					ulong BufferStartOffsetInBytes,
					TileCopyFlags Flags ) ;

	
	/// <summary>Copy a multi-sampled resource into a non-multi-sampled resource.</summary>
	/// <param name="pDstResource">
	/// <para>Type: [in] <b>ID3D12Resource*</b> Destination resource. Must be a created on a <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type">D3D12_HEAP_TYPE_DEFAULT</a> heap and be single-sampled. See <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvesubresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DstSubresource">
	/// <para>Type: [in] <b>UINT</b> A zero-based index, that identifies the destination subresource. Use <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12calcsubresource">D3D12CalcSubresource</a> to calculate the subresource index if the parent resource is complex.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvesubresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcResource">
	/// <para>Type: [in] <b>ID3D12Resource*</b> Source resource. Must be multisampled.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvesubresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="SrcSubresource">
	/// <para>Type: [in] <b>UINT</b> The source subresource of the source resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvesubresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Format">
	/// <para>Type: [in] <b>Format</b> A <a href="https://docs.microsoft.com/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format">Format</a> that indicates how the multisampled resource will be resolved to a single-sampled resource. See remarks.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvesubresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><h3><a id="Debug_layer"></a><a id="debug_layer"></a><a id="DEBUG_LAYER"></a>Debug layer</h3> The debug layer will issue an error if the subresources referenced by the source view is not in the  <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_RESOLVE_SOURCE</a> state. The debug layer will issue an error if the destination buffer is not in the  <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_RESOLVE_DEST</a> state. The source and destination resources must be the same resource type and have the same dimensions. In addition, they must have compatible formats. There are three scenarios for this: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvesubresource#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ResolveSubresource( IResource pDstResource, uint DstSubresource, 
							 IResource pSrcResource, uint SrcSubresource, 
							 Format Format) ;

	
	/// <summary>Bind information about the primitive type, and data order that describes input data for the input assembler stage. (ID3D12GraphicsCommandList.IASetPrimitiveTopology)</summary>
	/// <param name="PrimitiveTopology">
	/// <para>Type: <b>D3D12_PRIMITIVE_TOPOLOGY</b> The type of primitive and ordering of the primitive data (see <a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/ne-d3dcommon-d3d_primitive_topology">D3D_PRIMITIVE_TOPOLOGY</a>).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-iasetprimitivetopology#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-iasetprimitivetopology">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void IASetPrimitiveTopology( PrimitiveTopology PrimitiveTopology ) ;

	
	/// <summary>Bind an array of viewports to the rasterizer stage of the pipeline. (ID3D12GraphicsCommandList.RSSetViewports)</summary>
	/// <param name="NumViewports">
	/// <para>Type: <b>UINT</b> Number of viewports to bind. The range of valid values is (0, D3D12_VIEWPORT_AND_SCISSORRect_OBJECT_COUNT_PER_PIPELINE).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-rssetviewports#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pViewports">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_viewport">D3D12_VIEWPORT</a>*</b> An array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_viewport">D3D12_VIEWPORT</a> structures to bind to the device.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-rssetviewports#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>All viewports must be set atomically as one operation. Any viewports not defined by the call are disabled.</para>
	/// <para>Which viewport to use is determined by the <a href="https://docs.microsoft.com/windows/desktop/direct3dhlsl/dx-graphics-hlsl-semantics">SV_ViewportArrayIndex</a> semantic output by a geometry shader; if a geometry shader does not specify the semantic, Direct3D will use the first viewport in the array.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-rssetviewports#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void RSSetViewports( uint NumViewports, in Span< Viewport > pViewports ) ;

	
	/// <summary>Binds an array of scissor rectangles to the rasterizer stage.</summary>
	/// <param name="NumRects">
	/// <para>Type: <b>UINT</b> The number of scissor rectangles to bind.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-rssetscissorrects#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pRects">
	/// <para>Type: <b>const D3D12_Rect*</b> An array of scissor rectangles.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-rssetscissorrects#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>All scissor rectangles must be set atomically as one operation. Any scissor rectangles not defined by the call are disabled.</para>
	/// <para>Which scissor rectangle to use is determined by the <b>SV_ViewportArrayIndex</b> semantic output by a geometry shader
	/// (see shader semantic syntax). If a geometry shader does not make use of the <b>SV_ViewportArrayIndex</b> semantic then Direct3D
	/// will use the first scissor rectangle in the array.</para>
	/// <para>Each scissor rectangle in the array corresponds to a viewport in an array of viewports
	/// (see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-rssetviewports">RSSetViewports</a>).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-rssetscissorrects#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void RSSetScissorRects( uint NumRects, in Span< Rect > pRects ) ;

	
	/// <summary>Sets the blend factor that modulate values for a pixel shader, render target, or both.</summary>
	/// <param name="BlendFactor">
	/// <para>Type: <b>const FLOAT[4]</b> Array of blend factors, one for each RGBA component.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>If you created the blend-state object with [D3D12_BLEND_BLEND_FACTOR](./ne-d3d12-d3d12_blend.md) or **D3D12_BLEND_INV_BLEND_FACTOR**, then the blending stage uses the non-NULL array of blend factors. Otherwise,the blending stage doesn't use the non-NULL array of blend factors; the runtime stores the blend factors. If you pass NULL, then the runtime uses or stores a blend factor equal to `{ 1, 1, 1, 1 }`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void OMSetBlendFactor( float[ ] BlendFactor ) ;

	
	/// <summary>Sets the reference value for depth stencil tests.</summary>
	/// <param name="StencilRef">
	/// <para>Type: <b>UINT</b> Reference value to perform against when doing a depth-stencil test.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetstencilref#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetstencilref">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void OMSetStencilRef( uint StencilRef ) ;

	/// <summary>Sets all shaders and programs most of the fixed-function state of the graphics processing unit (GPU) pipeline.</summary>
	/// <param name="pPipelineState">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a>*</b> Pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> containing the pipeline state data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setpipelinestate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setpipelinestate">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetPipelineState( IPipelineState pPipelineState ) ;

	/// <summary>Notifies the driver that it needs to synchronize multiple accesses to resources. (ID3D12GraphicsCommandList.ResourceBarrier)</summary>
	/// <param name="NumBarriers">
	/// <para>Type: <b>UINT</b> The number of submitted barrier descriptions.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resourcebarrier#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pBarriers">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_barrier">D3D12_RESOURCE_BARRIER</a>*</b> Pointer to an array of barrier descriptions.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resourcebarrier#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>> [!NOTE] > A resource to be used for the [D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states) state must be created in that state, and then never transitioned out of it. Nor may a resource that was created not in that state be transitioned into it. For more info, see [Acceleration structure memory restrictions](https://microsoft.github.io/DirectX-Specs/d3d/Raytracing.html#acceleration-structure-memory-restrictions) in the DirectX raytracing (DXR) functional specification on GitHub. There are three types of barrier descriptions: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resourcebarrier#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ResourceBarrier( uint NumBarriers,
						  ResourceBarrier[ ] pBarriers ) ;


	/// <summary>Executes a bundle.</summary>
	/// <param name="pCommandList">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12graphicscommandlist">ID3D12GraphicsCommandList</a>*</b> Specifies the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12graphicscommandlist">ID3D12GraphicsCommandList</a> that determines the bundle to be executed.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executebundle#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>Bundles inherit all state from the parent command list on which <b>ExecuteBundle</b> is called, except the pipeline state object and primitive topology. All of the state that is set in a bundle will affect the state of the parent command list. Note that <b>ExecuteBundle</b> is not a predicated operation.</para>
	/// <para><h3><a id="Runtime_validation"></a><a id="runtime_validation"></a><a id="RUNTIME_VALIDATION"></a>Runtime validation</h3> The runtime will validate that the "callee" is a bundle and that the "caller" is a direct command list.  The runtime will also validate that the bundle has been closed.  If the contract is violated, the runtime will silently drop the call. Validation failure will result in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-close">Close</a> returning E_INVALIDARG.</para>
	/// <para><h3><a id="Debug_layer"></a><a id="debug_layer"></a><a id="DEBUG_LAYER"></a>Debug layer</h3> The debug layer will issue a warning in the same cases where the runtime will fail. The debug layer will issue a warning if a predicate is set when <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-executecommandlists">ExecuteCommandList</a> is called. Also, the debug layer will issue an error if it detects that any resource reference by the command list has been destroyed.</para>
	/// <para>The debug layer will also validate that the command allocator associated with the bundle has not been reset since <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-close">Close</a> was called on the command list.  This validation occurs at <b>ExecuteBundle</b> time, and when the parent command list is executed on a command queue.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executebundle#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ExecuteBundle( IGraphicsCommandList pCommandList ) ;


	/// <summary>Changes the currently bound descriptor heaps that are associated with a command list.</summary>
	/// <param name="NumDescriptorHeaps">
	/// <para>Type: [in] <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Number of descriptor heaps to bind.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setdescriptorheaps#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppDescriptorHeaps">
	/// <para>Type: [in] <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12descriptorheap">ID3D12DescriptorHeap</a>*</b> A pointer to an array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12descriptorheap">ID3D12DescriptorHeap</a> objects for the heaps to set on the command list. You can only bind descriptor heaps of type [**D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV**](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setdescriptorheaps) and [**D3D12_DESCRIPTOR_HEAP_TYPE_SAMPLER**](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setdescriptorheaps). Only one descriptor heap of each type can be set at one time, which means a maximum of 2 heaps (one sampler, one CBV/SRV/UAV) can be set at one time.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setdescriptorheaps#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><b>SetDescriptorHeaps</b> can be called on a bundle, but the bundle descriptor heaps must match the calling command list descriptor heap. For more information on bundle restrictions, refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/recording-command-lists-and-bundles">Creating and Recording Command Lists and Bundles</a>. All previously set heaps are unset by the call. At most one heap of each shader-visible type can be set in the call. Changing descriptor heaps can incur a pipeline flush on some hardware. Because of this, it is recommended to use a single shader-visible heap of each type, and set it once per frame, rather than regularly changing the bound descriptor heaps. Instead, use [**ID3D12Device::CopyDescriptors**](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors) and [**ID3D12Device::CopyDescriptorsSimple**](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple) to copy the required descriptors from shader-opaque heaps to the single shader-visible heap as required during rendering.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setdescriptorheaps#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetDescriptorHeaps( uint NumDescriptorHeaps,
							 IDescriptorHeap[ ] ppDescriptorHeaps ) ;

	
	/// <summary>Sets the layout of the compute root signature.</summary>
	/// <param name="pRootSignature">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a> object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootsignature">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetComputeRootSignature( IRootSignature pRootSignature ) ;

	
	/// <summary>Sets the layout of the graphics root signature.</summary>
	/// <param name="pRootSignature">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a> object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootsignature">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetGraphicsRootSignature( IRootSignature pRootSignature ) ;

	
	/// <summary>Sets a descriptor table into the compute root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b>UINT</b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootdescriptortable#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BaseDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_gpu_descriptor_handle">GPUDescriptorHandle</a></b> A GPU_descriptor_handle object for the base descriptor to set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootdescriptortable#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootdescriptortable">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetComputeRootDescriptorTable( uint RootParameterIndex, GPUDescriptorHandle BaseDescriptor ) ;

	
	/// <summary>Sets a descriptor table into the graphics root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootdescriptortable#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BaseDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_gpu_descriptor_handle">GPUDescriptorHandle</a></b> A GPU_descriptor_handle object for the base descriptor to set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootdescriptortable#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootdescriptortable">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetGraphicsRootDescriptorTable( uint RootParameterIndex, GPUDescriptorHandle BaseDescriptor ) ;

	
	/// <summary>Sets a constant in the compute root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b>UINT</b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputeroot32bitconstant#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="SrcData">
	/// <para>Type: <b>UINT</b> The source data for the constant to set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputeroot32bitconstant#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DestOffsetIn32BitValues">
	/// <para>Type: <b>UINT</b> The offset, in 32-bit values, to set the constant in the root signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputeroot32bitconstant#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputeroot32bitconstant">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetComputeRoot32BitConstant( uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues ) ;

	
	/// <summary>Sets a constant in the graphics root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsroot32bitconstant#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="SrcData">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The source data for the constant to set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsroot32bitconstant#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DestOffsetIn32BitValues">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The offset, in 32-bit values, to set the constant in the root signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsroot32bitconstant#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsroot32bitconstant">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetGraphicsRoot32BitConstant( uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues ) ;

	
	/// <summary>Sets a group of constants in the compute root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputeroot32bitconstants#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Num32BitValuesToSet">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of constants to set in the root signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputeroot32bitconstants#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcData">
	/// <para>Type: <b>const void*</b> The source data for the group of constants to set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputeroot32bitconstants#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DestOffsetIn32BitValues">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The offset, in 32-bit values, to set the first constant of the group in the root signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputeroot32bitconstants#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputeroot32bitconstants">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetComputeRoot32BitConstants( uint RootParameterIndex, uint Num32BitValuesToSet, 
									   nint pSrcData, uint DestOffsetIn32BitValues ) ;
	

	/// <summary>Sets a group of constants in the graphics root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsroot32bitconstants#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Num32BitValuesToSet">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of constants to set in the root signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsroot32bitconstants#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcData">
	/// <para>Type: <b>const void*</b> The source data for the group of constants to set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsroot32bitconstants#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DestOffsetIn32BitValues">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The offset, in 32-bit values, to set the first constant of the group in the root signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsroot32bitconstants#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsroot32bitconstants">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetGraphicsRoot32BitConstants( uint RootParameterIndex, uint Num32BitValuesToSet, 
										nint pSrcData, uint DestOffsetIn32BitValues ) ;
	

	/// <summary>Sets a CPU descriptor handle for the constant buffer in the compute root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b>UINT</b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootconstantbufferview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BufferLocation">
	/// <para>Type: <b>D3D12_GPU_VIRTUAL_ADDRESS</b> Specifies the D3D12_GPU_VIRTUAL_ADDRESS of the constant buffer.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootconstantbufferview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootconstantbufferview">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetComputeRootConstantBufferView( uint RootParameterIndex, ulong BufferLocation ) ;

	
	/// <summary>Sets a CPU descriptor handle for the constant buffer in the graphics root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootconstantbufferview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BufferLocation">
	/// <para>Type: <b>D3D12_GPU_VIRTUAL_ADDRESS</b> The GPU virtual address of the constant buffer. D3D12_GPU_VIRTUAL_ADDRESS is a typedef'd alias of UINT64.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootconstantbufferview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootconstantbufferview">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetGraphicsRootConstantBufferView( uint RootParameterIndex, ulong BufferLocation ) ;
	

	/// <summary>Sets a CPU descriptor handle for the shader resource in the compute root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b>UINT</b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootshaderresourceview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BufferLocation">
	/// <para>Type: <b>D3D12_GPU_VIRTUAL_ADDRESS</b> The GPU virtual address of the buffer. D3D12_GPU_VIRTUAL_ADDRESS is a typedef'd alias of UINT64.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootshaderresourceview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootshaderresourceview">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetComputeRootShaderResourceView( uint RootParameterIndex, ulong BufferLocation ) ;
	

	/// <summary>Sets a CPU descriptor handle for the shader resource in the graphics root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootshaderresourceview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BufferLocation">
	/// <para>Type: <b>D3D12_GPU_VIRTUAL_ADDRESS</b> The GPU virtual address of the Buffer. Textures are not supported. D3D12_GPU_VIRTUAL_ADDRESS is a typedef'd alias of UINT64.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootshaderresourceview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootshaderresourceview">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetGraphicsRootShaderResourceView( uint RootParameterIndex, ulong BufferLocation ) ;
	

	/// <summary>Sets a CPU descriptor handle for the unordered-access-view resource in the compute root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootunorderedaccessview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BufferLocation">
	/// <para>Type: <b>D3D12_GPU_VIRTUAL_ADDRESS</b> The GPU virtual address of the buffer. D3D12_GPU_VIRTUAL_ADDRESS is a typedef'd alias of UINT64.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootunorderedaccessview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootunorderedaccessview">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetComputeRootUnorderedAccessView( uint RootParameterIndex, ulong BufferLocation ) ;
	

	/// <summary>Sets a CPU descriptor handle for the unordered-access-view resource in the graphics root signature.</summary>
	/// <param name="RootParameterIndex">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The slot number for binding.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootunorderedaccessview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BufferLocation">
	/// <para>Type: <b>D3D12_GPU_VIRTUAL_ADDRESS</b> The GPU virtual address of the buffer. D3D12_GPU_VIRTUAL_ADDRESS is a typedef'd alias of UINT64.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootunorderedaccessview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootunorderedaccessview">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetGraphicsRootUnorderedAccessView( uint RootParameterIndex, ulong BufferLocation ) ;
	

	/// <summary>Sets the view for the index buffer.</summary>
	/// <param name="pView">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_index_buffer_view">IndexBufferView</a>*</b> The view specifies the index buffer's address, size, and <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">Format</a>, as a pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_index_buffer_view">IndexBufferView</a> structure.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-iasetindexbuffer#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>Only one index buffer can be bound to the graphics pipeline at any one time.</remarks>
	void IASetIndexBuffer( [Optional] in IndexBufferView pView ) ;
	
	
	/// <summary>Sets a CPU descriptor handle for the vertex buffers.</summary>
	/// <param name="StartSlot">
	/// <para>Type: <b>UINT</b> Index into the device's zero-based array to begin setting vertex buffers.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-iasetvertexbuffers#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumViews">
	/// <para>Type: <b>UINT</b> The number of views in the <i>pViews</i> array.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-iasetvertexbuffers#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pViews">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_vertex_buffer_view">VertexBufferView</a>*</b> Specifies the vertex buffer views in an array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_vertex_buffer_view">VertexBufferView</a> structures.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-iasetvertexbuffers#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-iasetvertexbuffers">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void IASetVertexBuffers( uint StartSlot, uint NumViews, [Optional] Span< VertexBufferView > pViews ) ;
	
	
	/// <summary>Sets the stream output buffer views.</summary>
	/// <param name="StartSlot">
	/// <para>Type: <b>UINT</b> Index into the device's zero-based array to begin setting stream output buffers.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-sosettargets#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumViews">
	/// <para>Type: <b>UINT</b> The number of entries in the <i>pViews</i> array.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-sosettargets#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pViews">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_stream_output_buffer_view">StreamOutputBufferView</a>*</b> Specifies an array of  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_stream_output_buffer_view">StreamOutputBufferView</a> structures.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-sosettargets#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-sosettargets">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SOSetTargets( uint StartSlot, uint NumViews, [Optional] Span< StreamOutputBufferView > pViews ) ;
	
	
	/// <summary>Sets CPU descriptor handles for the render targets and depth stencil.</summary>
	/// <param name="NumRenderTargetDescriptors">
	/// <para>Type: <b>UINT</b> The number of entries in the <i>pRenderTargetDescriptors</i> array (ranges between 0 and <b>D3D12_SIMULTANEOUS_RENDER_TARGET_COUNT</b>). If this parameter is nonzero, the number of entries in the array to which pRenderTargetDescriptors points must equal the number in this parameter.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetrendertargets#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pRenderTargetDescriptors">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a>*</b> Specifies an array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a> structures that describe the CPU descriptor handles that represents the start of the heap of render target descriptors. If this parameter is NULL and NumRenderTargetDescriptors is 0, no render targets are bound.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetrendertargets#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="RTsSingleHandleToDescriptorRange">
	/// <para>Type: <b>BOOL</b> <b>True</b> means the handle passed in is the pointer to a contiguous range of <i>NumRenderTargetDescriptors</i>  descriptors.  This case is useful if the set of descriptors to bind already happens to be contiguous in memory (so all that’s needed is a handle to the first one).  For example, if  <i>NumRenderTargetDescriptors</i> is 3 then the memory layout is taken as follows: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetrendertargets#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDepthStencilDescriptor">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a> structure that describes the CPU descriptor handle that represents the start of the heap that holds the depth stencil descriptor. If this parameter is NULL, no depth stencil descriptor is bound.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetrendertargets#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetrendertargets">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void OMSetRenderTargets( uint NumRenderTargetDescriptors,
							 [Optional] Span< CPUDescriptorHandle > pRenderTargetDescriptors,
							 bool RTsSingleHandleToDescriptorRange,
							 [Optional] Span< CPUDescriptorHandle > pDepthStencilDescriptor ) ;


	/// <summary>Clears the depth-stencil resource. (ID3D12GraphicsCommandList.ClearDepthStencilView)</summary>
	/// <param name="DepthStencilView">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the start of the heap for the depth stencil to be cleared.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-cleardepthstencilview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="clearFlags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_clear_flags">D3D12_CLEAR_FLAGS</a></b> A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_clear_flags">D3D12_CLEAR_FLAGS</a> values that are combined by using a bitwise OR operation. The resulting value identifies the type of data to clear (depth buffer, stencil buffer, or both).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-cleardepthstencilview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Depth">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">FLOAT</a></b> A value to clear the depth buffer with. This value will be clamped between 0 and 1.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-cleardepthstencilview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Stencil">
	/// <para>Type: <b>UINT8</b> A value to clear the stencil buffer with.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-cleardepthstencilview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumRects">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of rectangles in the array that the <i>pRects</i> parameter specifies.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-cleardepthstencilview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pRects">
	/// <para>Type: <b>const <b>D3D12_Rect</b>*</b> An array of <b>D3D12_Rect</b> structures for the rectangles in the resource view to clear. If <b>NULL</b>, <b>ClearDepthStencilView</b> clears the entire resource view.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-cleardepthstencilview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>Only direct and bundle command lists support this operation. <b>ClearDepthStencilView</b> may be used to initialize resources which alias the same heap memory. See <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createplacedresource">CreatePlacedResource</a> for more details. <h3><a id="Runtime_validation"></a><a id="runtime_validation"></a><a id="RUNTIME_VALIDATION"></a>Runtime validation</h3> For floating-point inputs, the runtime will set denormalized values to 0 (while preserving NANs). Validation failure will result in the call to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-close">Close</a> returning <b>E_INVALIDARG</b>. <h3><a id="Debug_layer"></a><a id="debug_layer"></a><a id="DEBUG_LAYER"></a>Debug layer</h3> The debug layer will issue errors if the input colors are denormalized. The debug layer will issue an error if the subresources referenced by the view are not in the appropriate state. For <b>ClearDepthStencilView</b>, the state must be in the state <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_DEPTH_WRITE</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-cleardepthstencilview#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ClearDepthStencilView( CPUDescriptorHandle DepthStencilView, 
								ClearFlags clearFlags, 
								float Depth, 
								byte Stencil,
								uint NumRects,
								Span< Rect > pRects ) ;


	/// <summary>Sets all the elements in a render target to one value.</summary>
	/// <param name="RenderTargetView">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Specifies a CPUDescriptorHandle structure that describes the CPU descriptor handle that represents the start of the heap for the render target to be cleared.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearrendertargetview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ColorRGBA">
	/// <para>Type: <b>const FLOAT[4]</b> A 4-component array that represents the color to fill the render target with.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearrendertargetview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumRects">
	/// <para>Type: <b>UINT</b> The number of rectangles in the array that the <i>pRects</i> parameter specifies.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearrendertargetview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pRects">
	/// <para>Type: <b>const D3D12_Rect*</b> An array of <b>D3D12_Rect</b> structures for the rectangles in the resource view to clear. If <b>NULL</b>, <b>ClearRenderTargetView</b> clears the entire resource view.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearrendertargetview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><b>ClearRenderTargetView</b> may be used to initialize resources which alias the same heap memory. See <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createplacedresource">CreatePlacedResource</a> for more details. <h3><a id="Runtime_validation"></a><a id="runtime_validation"></a><a id="RUNTIME_VALIDATION"></a>Runtime validation</h3> For floating-point inputs, the runtime will set denormalized values to 0 (while preserving NANs). Validation failure will result in the call to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-close">Close</a> returning <b>E_INVALIDARG</b>.</para>
	/// <para><h3><a id="Debug_layer"></a><a id="debug_layer"></a><a id="DEBUG_LAYER"></a>Debug layer</h3> The debug layer will issue errors if the input colors are denormalized. The debug layer will issue an error if the subresources referenced by the view are not in the appropriate state. For <b>ClearRenderTargetView</b>, the state must be <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_RENDER_TARGET</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearrendertargetview#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ClearRenderTargetView( CPUDescriptorHandle RenderTargetView, 
								float[ ] ColorRGBA,
								uint NumRects,
								Span< Rect > pRects = default ) ;


	/// <summary>Sets all the elements in a unordered-access view (UAV) to the specified integer values.</summary>
	/// <param name="ViewGPUHandleInCurrentHeap">
	/// <para>Type: [in] **[GPUDescriptorHandle](./ns-d3d12-d3d12_gpu_descriptor_handle.md)** A [GPUDescriptorHandle](./ns-d3d12-d3d12_gpu_descriptor_handle.md) that references an initialized descriptor for the unordered-access view (UAV) that is to be cleared. This descriptor must be in a shader-visible descriptor heap, which must be set on the command list via [SetDescriptorHeaps](nf-d3d12-id3d12graphicscommandlist-setdescriptorheaps.md).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ViewCPUHandle">
	/// <para>Type: [in] **[CPUDescriptorHandle](./ns-d3d12-d3d12_cpu_descriptor_handle.md)** A [CPUDescriptorHandle](./ns-d3d12-d3d12_cpu_descriptor_handle.md) in a non-shader visible descriptor heap that references an initialized descriptor for the unordered-access view (UAV) that is to be cleared. > [!IMPORTANT] > This descriptor must not be in a shader-visible descriptor heap. This is to allow drivers that implement the clear as a fixed-function hardware operation (rather than as a dispatch) to efficiently read from the descriptor, as shader-visible heaps may be created in **WRITE_BACK** memory (similar to **D3D12_HEAP_TYPE_UPLOAD** heap types), and CPU reads from this type of memory are prohibitively slow.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pResource">
	/// <para>Type: [in] **[ID3D12Resource](./nn-d3d12-id3d12resource.md)\*** A pointer to the [ID3D12Resource](./nn-d3d12-id3d12resource.md) interface that represents the unordered-access-view (UAV) resource to clear.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Values">
	/// <para>Type: [in] **const UINT[4]** A 4-component array that containing the values to fill the unordered-access-view resource with.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumRects">
	/// <para>Type: [in] **UINT** The number of rectangles in the array that the *pRects* parameter specifies.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pRects">
	/// <para>Type: [in] **const [D3D12_Rect](/windows/win32/direct3d12/d3d12-rect)\*** An array of **D3D12_Rect** structures for the rectangles in the resource view to clear. If **NULL**, **ClearUnorderedAccessViewUint** clears the entire resource view.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks></remarks>
	void ClearUnorderedAccessViewUint( GPUDescriptorHandle ViewGPUHandleInCurrentHeap,
									   CPUDescriptorHandle ViewCPUHandle,
									   IResource           pResource,
									   uint[ ]             Values, 
									   uint                NumRects, 
									   in Span< Rect >     pRects ) ;
	
	
	/// <summary>Sets all the elements in a unordered access view to the specified float values.</summary>
	/// <param name="ViewGPUHandleInCurrentHeap">
	/// <para>Type: [in] **[GPUDescriptorHandle](./ns-d3d12-d3d12_gpu_descriptor_handle.md)** A [GPUDescriptorHandle](./ns-d3d12-d3d12_gpu_descriptor_handle.md) that references an initialized descriptor for the unordered-access view (UAV) that is to be cleared. This descriptor must be in a shader-visible descriptor heap, which must be set on the command list via [SetDescriptorHeaps](nf-d3d12-id3d12graphicscommandlist-setdescriptorheaps.md).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewfloat#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ViewCPUHandle">
	/// <para>Type: [in] **[CPUDescriptorHandle](./ns-d3d12-d3d12_cpu_descriptor_handle.md)** A [CPUDescriptorHandle](./ns-d3d12-d3d12_cpu_descriptor_handle.md) in a non-shader visible descriptor heap that references an initialized descriptor for the unordered-access view (UAV) that is to be cleared. > [!IMPORTANT] > This descriptor must not be in a shader-visible descriptor heap. This is to allow drivers that implement the clear as a fixed-function hardware operation (rather than as a dispatch) to efficiently read from the descriptor, as shader-visible heaps may be created in **WRITE_BACK** memory (similar to **D3D12_HEAP_TYPE_UPLOAD** heap types), and CPU reads from this type of memory are prohibitively slow.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewfloat#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pResource">
	/// <para>Type: [in] **[ID3D12Resource](./nn-d3d12-id3d12resource.md)\*** A pointer to the [ID3D12Resource](./nn-d3d12-id3d12resource.md) interface that represents the unordered-access-view (UAV) resource to clear.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewfloat#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Values">
	/// <para>Type: [in] **const FLOAT[4]** A 4-component array that containing the values to fill the unordered-access-view resource with.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewfloat#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumRects">
	/// <para>Type: [in] **UINT** The number of rectangles in the array that the *pRects* parameter specifies.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewfloat#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pRects">
	/// <para>Type: [in] **const [D3D12_Rect](/windows/win32/direct3d12/d3d12-rect)\*** An array of **D3D12_Rect** structures for the rectangles in the resource view to clear. If **NULL**, **ClearUnorderedAccessViewFloat** clears the entire resource view.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewfloat#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks></remarks>
	void ClearUnorderedAccessViewFloat( GPUDescriptorHandle ViewGPUHandleInCurrentHeap,
										CPUDescriptorHandle ViewCPUHandle,
										IResource           pResource,
										float[ ]            Values, 
										uint                NumRects, 
										in Span< Rect >     pRects ) ;


	/// <summary>Discards a resource.</summary>
	/// <param name="pResource">
	/// <para>Type: [in] <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> interface for the resource to discard.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-discardresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pRegion">
	/// <para>Type: [in, optional] <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_discard_region">DiscardRegion</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_discard_region">DiscardRegion</a> structure that describes details for the discard-resource operation.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-discardresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>The semantics of <b>DiscardResource</b> change based on the command list type. For <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_command_list_type">D3D12_COMMAND_LIST_TYPE_DIRect</a>, the following two rules apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-discardresource#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void DiscardResource( IResource pResource, [Optional] in Span< DiscardRegion > pRegion ) ;
	
	
	/// <summary>Starts a query running. (ID3D12GraphicsCommandList.BeginQuery)</summary>
	/// <param name="pQueryHeap">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12queryheap">ID3D12QueryHeap</a>*</b> Specifies the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12queryheap">ID3D12QueryHeap</a> containing the query.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-beginquery#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Type">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_query_type">D3D12_QUERY_TYPE</a></b> Specifies one member of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_query_type">D3D12_QUERY_TYPE</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-beginquery#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Index">
	/// <para>Type: <b>UINT</b> Specifies the index of the query within the query heap.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-beginquery#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/queries">Queries</a> for more information about D3D12 queries.</remarks>
	void BeginQuery( IQueryHeap pQueryHeap, QueryType Type, uint Index ) ;
	

	/// <summary>Ends a running query.</summary>
	/// <param name="pQueryHeap">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12queryheap">ID3D12QueryHeap</a>*</b> Specifies the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12queryheap">ID3D12QueryHeap</a> containing the query.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-endquery#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Type">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_query_type">D3D12_QUERY_TYPE</a></b> Specifies one member of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_query_type">D3D12_QUERY_TYPE</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-endquery#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Index">
	/// <para>Type: <b>UINT</b> Specifies the index of the query in the query heap.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-endquery#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/queries">Queries</a> for more information about D3D12 queries.</remarks>
	void EndQuery( IQueryHeap pQueryHeap, QueryType Type, uint Index ) ;

	
	/// <summary>Extracts data from a query. ResolveQueryData works with all heap types (default, upload, and readback).  ResolveQueryData works with all heap types (default, upload, and readback). .</summary>
	/// <param name="pQueryHeap">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12queryheap">ID3D12QueryHeap</a>*</b> Specifies the  <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12queryheap">ID3D12QueryHeap</a> containing the queries to resolve.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvequerydata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Type">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_type">D3D12_QUERY_TYPE</a></b> Specifies the type of query, one member of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_type">D3D12_QUERY_TYPE</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvequerydata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="StartIndex">
	/// <para>Type: <b>UINT</b> Specifies an index of the first query to resolve.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvequerydata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumQueries">
	/// <para>Type: <b>UINT</b> Specifies the number of queries to resolve.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvequerydata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDestinationBuffer">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> Specifies an <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> destination buffer, which must be in the state <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_COPY_DEST</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvequerydata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="AlignedDestinationBufferOffset">
	/// <para>Type: <b>UINT64</b> Specifies an alignment offset into the destination buffer. Must be a multiple of 8 bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvequerydata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><b>ResolveQueryData</b> performs a batched operation that writes query data into a destination buffer.  Query data is written contiguously to the destination buffer, and the parameter. <b>ResolveQueryData</b> turns application-opaque query data in an application-opaque query heap into adapter-agnostic values usable by your application. Resolving queries within a heap that have not been completed (so have had [**ID3D12GraphicsCommandList::BeginQuery**](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-beginquery) called for them, but not [**ID3D12GraphicsCommandList::EndQuery**](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-endquery)), or that have been uninitialized, results in undefined behavior and might cause device hangs or removal. The debug layer will emit an error if it detects an application has resolved incomplete or uninitialized queries. > [!NOTE] > Resolving incomplete or uninitialized queries is undefined behavior because the driver might internally store GPUVAs or other data within unresolved queries. And so attempting to resolve these queries on uninitialized data could cause a page fault or device hang. Older versions of the debug layer didn't validate this behavior. Binary occlusion queries write 64-bits per query. The least significant bit is either 0 (the object was entirely occluded) or 1 (at least 1 sample of the object would have been drawn). The rest of the bits are 0. Occlusion queries write 64-bits per query. The value is the number of samples that passed testing. Timestamp queries write 64-bits per query, which is a tick value that must be compared to the respective command queue frequency (see [Timing](/windows/win32/direct3d12/timing)). Pipeline statistics queries write a [**D3D12_QUERY_DATA_PIPELINE_STATISTICS**](/windows/win32/api/d3d12/ns-d3d12-d3d12_query_data_pipeline_statistics) structure per query. All stream-out statistics queries write a [**D3D12_QUERY_DATA_SO_STATISTICS**](/windows/win32/api/d3d12/ns-d3d12-d3d12_query_data_so_statistics) structure per query. The core runtime will validate the following. </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvequerydata#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ResolveQueryData( IQueryHeap pQueryHeap, QueryType Type, uint StartIndex, uint NumQueries, 
						   IResource pDestinationBuffer, ulong AlignedDestinationBufferOffset ) ;

	
	/// <summary>Sets a rendering predicate.</summary>
	/// <param name="pBuffer">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> The buffer, as an <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>, which must be in the [**D3D12_RESOURCE_STATE_PREDICATION**](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states) or [**D3D21_RESOURCE_STATE_INDIRect_ARGUMENT**](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states) state (both values are identical, and provided as aliases for clarity), or **NULL** to disable predication.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setpredication#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="AlignedBufferOffset">
	/// <para>Type: <b>UINT64</b> The aligned buffer offset, as a UINT64.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setpredication#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Operation">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_predication_op">D3D12_PREDICATION_OP</a></b> Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_predication_op">D3D12_PREDICATION_OP</a>, such as D3D12_PREDICATION_OP_EQUAL_ZERO or D3D12_PREDICATION_OP_NOT_EQUAL_ZERO.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setpredication#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>Use this method to denote that subsequent rendering and resource manipulation commands are not actually performed if the resulting predicate data of the predicate is equal to the operation specified.</para>
	/// <para>Unlike Direct3D 11, in Direct3D 12 predication state is not inherited by direct command lists, and predication is always respected (there are no predication hints). All direct command lists begin with predication disabled. Bundles do inherit predication state. It is legal for the same predicate to be bound multiple times.</para>
	/// <para>Illegal API calls will result in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-close">Close</a> returning an error, or <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-executecommandlists">ID3D12CommandQueue::ExecuteCommandLists</a> dropping the command list and removing the device.</para>
	/// <para>The debug layer will issue errors whenever the runtime validation fails.</para>
	/// <para>Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/predication">Predication</a> for more information.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setpredication#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetPredication( IResource pBuffer, ulong AlignedBufferOffset, PredicationOp Operation ) ;
	

	/// <summary>Not intended to be called directly.  Use the PIX event runtime to insert events into a command list. (ID3D12GraphicsCommandList.SetMarker)</summary>
	/// <param name="Metadata">
	/// <para>Type: <b>UINT</b> Internal.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setmarker#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>const void*</b> Internal.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setmarker#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Size">
	/// <para>Type: <b>UINT</b> Internal.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setmarker#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>This is a support method used internally by the PIX event runtime.  It is not intended to be called directly. To insert instrumentation markers at the current location within a D3D12 command list, use the <b>PIXSetMarker</b> function.  This is provided by the <a href="https://devblogs.microsoft.com/pix/winpixeventruntime/">WinPixEventRuntime</a> NuGet package.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setmarker#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetMarker(uint Metadata, [Optional] nint pData, uint Size ) ;
	

	/// <summary>Not intended to be called directly.  Use the PIX event runtime to insert events into a command list. (ID3D12GraphicsCommandList.BeginEvent)</summary>
	/// <param name="Metadata">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Internal.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-beginevent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">void</a>*</b> Internal.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-beginevent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Size">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Internal.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-beginevent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>This is a support method used internally by the PIX event runtime.  It is not intended to be called directly. To mark the start of an instrumentation region at the current location within a D3D12 command list, use the <b>PIXBeginEvent</b> function or <b>PIXScopedEvent</b> macro.  These are provided by the <a href="https://devblogs.microsoft.com/pix/winpixeventruntime/">WinPixEventRuntime</a> NuGet package.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-beginevent#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void BeginEvent( uint Metadata, [Optional] nint pData, uint Size ) ;

	
	/// <summary>Not intended to be called directly.  Use the PIX event runtime to insert events into a command list. (ID3D12GraphicsCommandList.EndEvent)</summary>
	/// <remarks>
	/// <para>This is a support method used internally by the PIX event runtime.  It is not intended to be called directly. To mark the end of an instrumentation region at the current location within a D3D12 command list, use the <b>PIXEndEvent</b> function or <b>PIXScopedEvent</b> macro.  These are provided by the <a href="https://devblogs.microsoft.com/pix/winpixeventruntime/">WinPixEventRuntime</a> NuGet package.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-endevent#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void EndEvent( ) ;


	/// <summary>Apps perform indirect draws/dispatches using the ExecuteIndirect method.</summary>
	/// <param name="pCommandSignature">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandsignature">ID3D12CommandSignature</a>*</b> Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandsignature">ID3D12CommandSignature</a>. The data referenced by <i>pArgumentBuffer</i> will be interpreted depending on the contents of the command signature. Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/indirect-drawing">Indirect Drawing</a> for the APIs that are used to create a command signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executeindirect#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="MaxCommandCount">
	/// <para>Type: <b>UINT</b> There are two ways that command counts can be specified: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executeindirect#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pArgumentBuffer">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> Specifies one or more <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> objects, containing the command arguments.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executeindirect#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ArgumentBufferOffset">
	/// <para>Type: <b>UINT64</b> Specifies an offset into <i>pArgumentBuffer</i> to identify the first command argument.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executeindirect#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pCountBuffer">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> Specifies a pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executeindirect#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="CountBufferOffset">
	/// <para>Type: <b>UINT64</b> Specifies a UINT64 that is the offset into <i>pCountBuffer</i>, identifying the argument count.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executeindirect#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>The semantics of this API are defined with the following pseudo-code: Non-NULL pCountBuffer:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executeindirect#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ExecuteIndirect( ICommandSignature pCommandSignature,
						  uint MaxCommandCount,
						  IResource pArgumentBuffer,
						  ulong ArgumentBufferOffset,
						  IResource pCountBuffer,
						  ulong CountBufferOffset ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( ID3D12GraphicsCommandList ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new GraphicsCommandList( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new GraphicsCommandList( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) =>
		new GraphicsCommandList( (ID3D12GraphicsCommandList?)pComObj! ) ;
	// ==================================================================================
} ;


// ===================================================================================
// ID3D12GraphicsCommandList1 interface ::
// ===================================================================================

[ProxyFor(typeof(ID3D12GraphicsCommandList1))]
public interface IGraphicsCommandList1 : IGraphicsCommandList {
	
	// ---------------------------------------------------------------------------------

	/// <summary>Atomically copies a primary data element of type UINT from one resource to another, along with optional dependent resources.</summary>
	/// <param name="pDstBuffer">
	/// <para>Type: <b>ID3D12Resource*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> The resource that the UINT primary data element is copied into.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DstOffset">
	/// <para>Type: <b>UINT64</b> An offset into the destination resource buffer that specifies where the primary data element is copied into, in bytes. This offset combined with the base address of the resource buffer must result in a memory address that's naturally aligned for UINT values.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcBuffer">
	/// <para>Type: <b>ID3D12Resource*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> The resource that the UINT primary data element is copied from. This data is typically an address, index, or other handle that shader code can use to locate the most-recent version of latency-sensitive information.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="SrcOffset">
	/// <para>Type: <b>UINT64</b> An offset into the source resource buffer that specifies where the primary data element is copied from, in bytes. This offset combined with the base address of the resource buffer must result in a memory address that's naturally aligned for UINT values.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Dependencies">
	/// <para>Type: <b>UINT</b> The number of dependent resources.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppDependentResources">
	/// <para>Type: <b>ID3D12Resource*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_reads_(Dependencies)</c> An array of resources that contain the dependent elements of the data payload.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDependentSubresourceRanges">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_subresource_range_uint64">D3D12_SUBRESOURCE_RANGE_UINT64</a>*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values?view=vs-2015">SAL</a>: <c>_In_reads_(Dependencies)</c> An array of subresource ranges that specify the dependent elements of the data payload. These elements are completely updated before the primary data element is itself atomically copied. This ensures that the entire operation is logically atomic; that is, the primary data element never refers to an incomplete data payload.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>This method is typically used to update resources for which normal rendering pipeline latency can be detrimental to user experience. For example, an application can compute a view matrix from the latest user input (such as from the sensors of a head-mounted display), and use this function to update and activate this matrix in command lists already dispatched to the GPU to reduce perceived latency between input and rendering.</remarks>
	void AtomicCopyBufferUINT( IResource                 pDstBuffer,
							   ulong                     DstOffset,
							   IResource                 pSrcBuffer,
							   ulong                     SrcOffset,
							   uint                      Dependencies,
							   IResource[]               ppDependentResources,
							   in SubresourceRangeUInt64 pDependentSubresourceRanges ) ;


	/// <summary>Atomically copies a primary data element of type UINT64 from one resource to another, along with optional dependent resources.</summary>
	/// <param name="pDstBuffer">
	/// <para>Type: <b>ID3D12Resource*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values?view=vs-2015">SAL</a>: <c>_In_</c> The resource that the UINT64 primary data element is copied into.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint64#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DstOffset">
	/// <para>Type: <b>UINT64</b> An offset into the destination resource buffer that specifies where the primary data element is copied into, in bytes. This offset combined with the base address of the resource buffer must result in a memory address that's naturally aligned for UINT64 values.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint64#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcBuffer">
	/// <para>Type: <b>ID3D12Resource*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> The resource that the UINT64 primary data element is copied from. This data is typically an address, index, or other handle that shader code can use to locate the most-recent version of latency-sensitive information.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint64#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="SrcOffset">
	/// <para>Type: <b>UINT64</b> An offset into the source resource buffer that specifies where the primary data element is copied from, in bytes. This offset combined with the base address of the resource buffer must result in a memory address that's naturally aligned for UINT64 values.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint64#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Dependencies">
	/// <para>Type: <b>UINT</b> The number of dependent resources.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint64#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppDependentResources">
	/// <para>Type: <b>ID3D12Resource*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_reads_(Dependencies)</c> An array of resources that contain the dependent elements of the data payload.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint64#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDependentSubresourceRanges">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_subresource_range_uint64">D3D12_SUBRESOURCE_RANGE_UINT64</a>*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_reads_(Dependencies)</c> An array of subresource ranges that specify the dependent elements of the data payload. These elements are completely updated before the primary data element is itself atomically copied. This ensures that the entire operation is logically atomic; that is, the primary data element never refers to an incomplete data payload.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-atomiccopybufferuint64#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>This method is typically used to update resources for which normal rendering pipeline latency can be detrimental to user experience. For example, an application can compute a view matrix from the latest user input (such as from the sensors of a head-mounted display), and use this function to update and activate this matrix in command lists already dispatched to the GPU to reduce perceived latency between input and rendering.</remarks>
	void AtomicCopyBufferUINT64( IResource                 pDstBuffer,
								 ulong                     DstOffset,
								 IResource                 pSrcBuffer,
								 ulong                     SrcOffset,
								 uint                      Dependencies,
								 IResource[]               ppDependentResources,
								 in SubresourceRangeUInt64 pDependentSubresourceRanges ) ;
	

	/// <summary>This method enables you to change the depth bounds dynamically.</summary>
	/// <param name="Min">
	/// <para>Type: <b>FLOAT</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> Specifies the minimum depth bounds. The default value is 0. NaN values silently convert to 0.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-omsetdepthbounds#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Max">
	/// <para>Type: <b>FLOAT</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> Specifies the maximum depth bounds. The default value is 1. NaN values silently convert to 0.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-omsetdepthbounds#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>Depth-bounds testing allows pixels and samples to be discarded if the currently-stored depth value is outside the range specified by <i>Min</i> and <i>Max</i>, inclusive. If the currently-stored depth value of the pixel or sample is inside this range, then the depth-bounds test passes and it is rendered; otherwise, the depth-bounds test fails and the pixel or sample is discarded. Note that the depth-bounds test considers the currently-stored depth value, not the depth value generated by the executing pixel shader. To use depth-bounds testing, the application must use the new <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device2-createpipelinestate">CreatePipelineState</a> method to enable depth-bounds testing on the PSO and then can use this command list method to change the depth-bounds dynamically. OMSetDepthBounds is an optional feature. Use the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a> method to determine whether or not this feature is supported by the user-mode driver. Support for this feature is reported through the [D3D12_FEATURE_D3D12_OPTIONS2](./ne-d3d12-d3d12_feature.md) structure.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-omsetdepthbounds#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void OMSetDepthBounds( float Min, float Max ) ;


	/// <summary>This method configures the sample positions used by subsequent draw, copy, resolve, and similar operations.</summary>
	/// <param name="NumSamplesPerPixel">
	/// <para>Type: <b>UINT</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> Specifies the number of samples to take, per pixel. This value can be 1, 2, 4, 8, or 16, otherwise the SetSamplePosition call is dropped. The number of samples must match the sample count configured in the PSO at draw time, otherwise the behavior is undefined.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-setsamplepositions#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumPixels">
	/// <para>Type: <b>UINT</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> Specifies the number of pixels that sample patterns are being specified for. This value can be either 1 or 4, otherwise the SetSamplePosition call is dropped. A value of 1 configures a single sample pattern to be used for each pixel; a value of 4 configures separate sample patterns for each pixel in a 2x2 pixel grid which is repeated over the render-target or viewport space, aligned to even coordinates. Note that the maximum number of combined samples can't exceed 16, otherwise the call is dropped. If NumPixels is set to 4, NumSamplesPerPixel can specify no more than 4 samples.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-setsamplepositions#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSamplePositions">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_sample_position">D3D12_SAMPLE_POSITION</a>*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_reads_(NumSamplesPerPixel*NumPixels)</c> Specifies an array of D3D12_SAMPLE_POSITION elements. The size of the array is NumPixels * NumSamplesPerPixel. If NumPixels is set to 4, then the first group of sample positions corresponds to the upper-left pixel in the 2x2 grid of pixels; the next group of sample positions corresponds to the upper-right pixel, the next group to the lower-left pixel, and the final group to the lower-right pixel. If centroid interpolation is used during rendering, the order of positions for each pixel determines centroid-sampling priority. That is, the first covered sample in the order specified is chosen as the centroid sample location.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-setsamplepositions#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>The operational semantics of sample positions are determined by the various draw, copy, resolve, and other operations that can occur. <b>CommandList:</b> In the absence of any prior calls to SetSamplePositions in a CommandList, samples assume the default position based on the Pipeline State Object (PSO). The default positions are determined either by the SAMPLE_DESC portion of the PSO if it is present, or by the standard sample positions if the RASTERIZER_DESC portion of the PSO has ForcedSampleCount set to a value greater than 0. After SetSamplePosition has been called, subsequent draw calls must use a PSO that specifies a matching sample count either using the SAMPLE_DESC portion of the PSO, or ForcedSampleCount in the RASTERIZER_DESC portion of the PSO. SetSamplePositions can only be called on a graphics CommandList. It can't be called in a bundle; bundles inherit sample position state from the calling CommandList and don't modify it. Calling SetSamplePositions(0, 0, NULL) reverts the sample positions to their default values. <b>Clear RenderTarget:</b> Sample positions are ignored when clearing a render target. <b>Clear DepthStencil:</b> When clearing the depth portion of a depth-stencil surface or any region of it, the sample positions must be set to match those of future rendering to the cleared surface or region; the contents of any uncleared regions produced using different sample positions become undefined. When clearing the stencil portion of a depth-stencil surface or any region of it, the sample positions are ignored. <b>Draw to RenderTarget:</b> When drawing to a render target the sample positions can be changed for each draw call, even when drawing to a region that overlaps previous draw calls. The current sample positions determine the operational semantics of each draw call and samples are taken from taken from the stored contents of the render target, even if the contents were produced using different sample positions. <b>Draw using DepthStencil:</b> When drawing to a depth-stencil surface (read or write) or any region of it, the sample positions must be set to match those used to clear the affected region previously. To use a different sample position, the target region must be cleared first. The pixels outside the clear region are unaffected. Hardware may store the depth portion or a depth-stencil surface as plane equations, and evaluate them to produce depth values when the application issues a read. Only the rasterizer and output-merger are required to support programmable sample positions of the depth portion of a depth-stencil surface. Any other read or write of the depth portion that has been rendered with sample positions set may ignore them and instead sample at the standard positions. <b>Resolve RenderTarget:</b> When resolving a render target or any region of it, the sample positions are ignored; these APIs operate only on stored color values. <b>Resolve DepthStencil:</b> When resolving the depth portion of a depth-stencil surface or any region of it, the sample positions must be set to match those of past rendering to the resolved surface or region. To use a different sample position, the target region must be cleared first. When resolving the stencil portion of a depth-stencil surface or any region of it, the sample positions are ignored; stencil resolves operate only on stored stencil values. <b>Copy RenderTarget:</b> When copying from a render target, the sample positions are ignored regardless of whether it is a full or partial copy. <b>Copy DepthStencil (Full Subresource):</b> When copying a full subresource from a depth-stencil surface, the sample positions must be set to match the sample positions used to generate the source surface. To use a different sample position, the target region must be cleared first. On some hardware properties of the source surface (such as stored plane equations for depth values) transfer to the destination. Therefore, if the destination surface is subsequently drawn to, the sample positions originally used to generate the source content need to be used with the destination surface. The API requires this on all hardware for consistency even if it may only apply to some. <b>Copy DepthStencil (Partial Subresource):</b> When copying a partial subresource from a depth-stencil surface, the sample positions must be set to match the sample positions used to generate the source surface, similarly to copying a full subresource. However, if the content of an affected destination subresources is only partially covered by the copy, the contents of the uncovered portion within those subresources becomes undefined unless all of it was generated using the same sample positions as the copy source. To use a different sample position, the target region must be cleared first. When copying a partial subresource from the stencil portion of a depth-stencil surface, the sample postions are ignored. It doesn’t matter what sample positions were used to generate content for any other areas of the destination buffer not covered by the copy – those contents remain valid. <b>Shader SamplePos:</b> The HLSL SamplePos intrinsic is not aware of programmable sample positions and results returned to shaders calling this on a surface rendered with programmable positions is undefined. Applications must pass coordinates into their shader manually if needed. Similarly evaluating attributes by sample index is undefined with programmable sample positions. <b>Transitioning out of DEPTH_READ or DEPTH_WRITE state:</b> If a subresource in DEPTH_READ or DEPTH_WRITE state is transitioned to any other state, including COPY_SOURCE or RESOLVE_SOURCE, some hardware might need to decompress the surface. Therefore, the sample positions must be set on the command list to match those used to generate the content in the source surface. Furthermore, for any subsequent transitions of the surface while the same depth data remains in it, the sample positions must continue to match those set on the command list. To use a different sample position, the target region must be cleared first. If an application wants to minimize the decompressed area when only a portion needs to be used, or just to preserve compression, ResolveSubresourceRegion() can be called in DECOMPRESS mode with a rect specified.  This will decompress just the relevant area to a separate resource leaving the source intact on some hardware, though on other hardware even the source area is decompressed. The separate explicitly decompressed resource can then be transitioned to the desired state (such as SHADER_RESOURCE). <b>Transitioning out of RENDER_TARGET state:</b> If a subresource in RENDER_TARGET state is transitioned to anything other than COPY_SOURCE or RESOLVE_SOURCE, some implementations may need to decompress the surface. This decompression is agnostic to sample positions. If an application wants to minimize the decompressed area when only a portion needs to be used, or just to preserve compression, ResolveSubresourceRegion() can be called in DECOMPRESS mode with a rect specified.  This will decompress just the relevant area to a separate resource leaving the source intact on some hardware, though on other hardware even the source area is decompressed. The separate explicitly decompressed resource can then be transitioned to the desired state (such as SHADER_RESOURCE).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-setsamplepositions#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetSamplePositions( uint NumSamplesPerPixel, uint NumPixels, in SamplePosition pSamplePositions ) ;


	/// <summary>Copy a region of a multisampled or compressed resource into a non-multisampled or non-compressed resource.</summary>
	/// <param name="pDstResource">
	/// <para>Type: <b>ID3D12Resource*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> Destination resource. Must be created with the <b>D3D11_USAGE_DEFAULT</b> flag and must be single-sampled unless its to be resolved from a compressed resource (<b>D3D12_RESOLVE_MODE_DECOMPRESS</b>); in this case it must have the same sample count as the compressed source.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-resolvesubresourceregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DstSubresource">
	/// <para>Type: <b>UINT</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> A zero-based index that identifies the destination subresource. Use <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12calcsubresource">D3D12CalcSubresource</a> to calculate the subresource index if the parent resource is complex.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-resolvesubresourceregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DstX">
	/// <para>Type: <b>UINT</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> The X coordinate of the left-most edge of the destination region. The width of the destination region is the same as the width of the source rect.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-resolvesubresourceregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DstY">
	/// <para>Type: <b>UINT</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> The Y coordinate of the top-most edge of the destination region. The height of the destination region is the same as the height of the source rect.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-resolvesubresourceregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcResource">
	/// <para>Type: <b>ID3D12Resource*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> Source resource. Must be multisampled or compressed.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-resolvesubresourceregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="SrcSubresource">
	/// <para>Type: <b>UINT</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> A zero-based index that identifies the source subresource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-resolvesubresourceregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcRect">
	/// <para>Type: <b>D3D12_RECT*</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_opt_</c> Specifies the rectangular region of the source resource to be resolved. Passing NULL for <i>pSrcRect</i> specifies that the entire subresource is to be resolved.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-resolvesubresourceregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="format">
	/// <para>Type: <b>DXGI_FORMAT</b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> A DXGI_FORMAT that specifies how the source and destination resource formats are consolidated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-resolvesubresourceregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="resolveMode">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resolve_mode">D3D12_RESOLVE_MODE</a></b> <a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> Specifies the operation used to resolve the source samples. When using the <b>D3D12_RESOLVE_MODE_DECOMPRESS</b> operation, the sample count can be larger than 1 as long as the source and destination have the same sample count, and source and destination may specify the same resource as long as the source rect aligns with the destination X and Y coordinates, in which case decompression occurs in place. When using the <b>D3D12_RESOLVE_MODE_MIN</b>, <b>D3D12_RESOLVE_MODE_MAX</b>, or <b>D3D12_RESOLVE_MODE_AVERAGE</b> operation, the destination must have a sample count of 1.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-resolvesubresourceregion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>ResolveSubresourceRegion operates like <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvesubresource">ResolveSubresource</a> but allows for only part of a resource to be resolved and for source samples to be resolved in several ways. Partial resolves can be useful in multi-adapter scenarios; for example, when the rendered area has been partitioned across adapters, each adapter might only need to resolve the portion of a subresource that corresponds to its assigned partition.</remarks>
	void ResolveSubresourceRegion( IResource           pDstResource,
								   uint                DstSubresource,
								   uint                DstX,
								   uint                DstY,
								   IResource           pSrcResource,
								   uint                SrcSubresource,
								   [Optional] in Rect? pSrcRect,
								   Format              format,
								   ResolveMode         resolveMode ) ;

	
	/// <summary>Set a mask that controls which view instances are enabled for subsequent draws.</summary>
	/// <param name="Mask">
	/// <para>Type: <b>UINT</b> A mask that specifies which views are enabled or disabled. If bit <i>i</i> starting from the least-significant bit is set, view instance <i>i</i> is enabled.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-setviewinstancemask#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>The view instance mask only affects PSOs that declare view instance masking by specifying the D3D12_VIEW_INSTANCING_FLAG_ENABLE_VIEW_INSTANCE_MASKING flag during their creation. Attempting to create a PSO that declares view instance masking will fail on adapters that don't support view instancing. The view instance mask defaults to 0 which disables all views. This forces applications that declare view instance masking to explicitly choose the views to enable, otherwise nothing will be rendered. If the view instance mask enabled all views by default the application might not remember to disable unused views, resulting in lost performance due to wasted work. Bundles don't inherit their view instance mask from their caller, defaulting to 0 instead. This is because the mask setting must be known when the bundle is recorded if it affects how an implementation records draws. The view instance mask set by a bundle does persist to the caller after the bundle completes, however. These inheritance semantics are similar to those of PSOs. No shader code paths that are dependent on SV_ViewID are executed at any shader stage for view instances that are masked off and no clipping, viewport processing, or rasterization is performed. Implementations that inspect the mask during rendering can incur a small performance penalty over PSOs that don't declare view instance masking at all, but usually the penalty can be overcome by the performance savings that result from skipping the work associated with the masked off views. Depending on the frequency and amount of skipped work, the performance gains can be significant.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-setviewinstancemask#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetViewInstanceMask( uint Mask ) ;
	
	// ---------------------------------------------------------------------------------
	// Static Interface Members:
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( ID3D12GraphicsCommandList1 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList1).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new GraphicsCommandList( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new GraphicsCommandList( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) =>
		new GraphicsCommandList( (ID3D12GraphicsCommandList?)pComObj! ) ;
	// ==================================================================================
} ;


// ===================================================================================
// ID3D12GraphicsCommandList2 interface ::
// ===================================================================================

[ProxyFor( typeof( ID3D12GraphicsCommandList2 ) )]
public interface IGraphicsCommandList2: IGraphicsCommandList1 {
	
	// ---------------------------------------------------------------------------------

	/// <summary>Writes a number of 32-bit immediate values to the specified buffer locations directly from the command stream. (ID3D12GraphicsCommandList2.WriteBufferImmediate)</summary>
	/// <param name="Count">The number of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_writebufferimmediate_parameter">D3D12_WRITEBUFFERIMMEDIATE_PARAMETER</a> structures that are pointed to by <i>pParams</i> and <i>pModes</i>.</param>
	/// <param name="pParams">The address of an array containing a number of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_writebufferimmediate_parameter">D3D12_WRITEBUFFERIMMEDIATE_PARAMETER</a> structures equal to <i>Count</i>.</param>
	/// <param name="pModes">The address of an array containing a number of  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_writebufferimmediate_mode">D3D12_WRITEBUFFERIMMEDIATE_MODE</a> structures equal to <i>Count</i>. The default value is <b>null</b>; passing <b>null</b> causes the system to write all immediate values using <b>D3D12_WRITEBUFFERIMMEDIATE_MODE_DEFAULT</b>.</param>
	/// <remarks>
	/// <para><b>WriteBufferImmediate</b> performs <i>Count</i> number of 32-bit writes: one for each value and destination specified in <i>pParams</i>. The receiving buffer (resource) must be in the <b>D3D12_RESOURCE_STATE_COPY_DEST</b> state to be a valid destination for <b>WriteBufferImmediate</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist2-writebufferimmediate#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void WriteBufferImmediate( uint Count, 
							   in Span< WriteBufferImmediateParameter > pParams,
							   Span< WriteBufferImmediateMode > pModes = default ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( ID3D12GraphicsCommandList2 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList2).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new GraphicsCommandList2( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new GraphicsCommandList2( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new GraphicsCommandList2( (ID3D12GraphicsCommandList3?)pComObj! ) ;
	// ===================================================================================
} ;


// ===================================================================================
// ID3D12GraphicsCommandList3 interface ::
// ===================================================================================

[ProxyFor( typeof( ID3D12GraphicsCommandList3 ) )]
public interface IGraphicsCommandList3: IGraphicsCommandList2 {
	// ---------------------------------------------------------------------------------

	/// <summary>Specifies whether or not protected resources can be accessed by subsequent commands in the command list.</summary>
	/// <param name="pProtectedResourceSession">
	/// <para>Type: **[ID3D12ProtectedResourceSession](./nn-d3d12-id3d12protectedresourcesession.md)\*** An optional pointer to an **ID3D12ProtectedResourceSession**. You can obtain an **ID3D12ProtectedResourceSession** by calling [ID3D12Device4::CreateProtectedResourceSession](./nf-d3d12-id3d12device4-createprotectedresourcesession.md).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist3-setprotectedresourcesession#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>If set, indicates that protected resources can be accessed with the given session. Access to protected resources can only happen after <b>SetProtectedResourceSession</b> is called with a valid session. The command list state is cleared when calling this method. If you pass <b>NULL</b>, then no protected resources can be accessed.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist3-setprotectedresourcesession">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetProtectedResourceSession( IProtectedResourceSession pProtectedResourceSession ) ;
	
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof( ID3D12GraphicsCommandList3 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static IInstantiable IInstantiable. Instantiate( ) => new GraphicsCommandList3( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new GraphicsCommandList3( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new GraphicsCommandList3( (ID3D12GraphicsCommandList3?)pComObj! ) ;
	// ===================================================================================
} ;


// ===================================================================================
// ID3D12GraphicsCommandList4 interface ::
// ===================================================================================

[ProxyFor( typeof( ID3D12GraphicsCommandList4 ) )]
public interface IGraphicsCommandList4: IGraphicsCommandList3 {
	// ---------------------------------------------------------------------------------
	
	/// <summary>Marks the beginning of a render pass by binding a set of output resources for the duration of the render pass. These bindings are to one or more render target views (RTVs), and/or to a depth stencil view (DSV).</summary>
	/// <param name="NumRenderTargets">A <b>UINT</b>. The number of render targets being bound.</param>
	/// <param name="pRenderTargets">A pointer to a constant <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_pass_render_target_desc">D3D12_RENDER_PASS_RENDER_TARGET_DESC</a>, which describes bindings (fixed for the duration of the render pass) to one or more render target views (RTVs), as well as their beginning and ending access characteristics.</param>
	/// <param name="pDepthStencil">A pointer to a constant <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_pass_depth_stencil_desc">D3D12_RENDER_PASS_DEPTH_STENCIL_DESC</a>, which describes a binding (fixed for the duration of the render pass) to a depth stencil view (DSV), as well as its beginning and ending access characteristics.</param>
	/// <param name="Flags">A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_render_pass_flags">D3D12_RENDER_PASS_FLAGS</a>. The nature/requirements of the render pass; for example, whether it is a suspending or a resuming render pass, or whether it wants to write to unordered access view(s).</param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-beginrenderpass">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void BeginRenderPass( uint NumRenderTargets,
						  RenderPassRenderTargetDescription[]   pRenderTargets,
						  in RenderPassDepthStencilDescription? pDepthStencil = null,
						  RenderPassFlags Flags = RenderPassFlags.None ) ;

	
	/// <summary>Marks the ending of a render pass.</summary>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-endrenderpass">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void EndRenderPass( ) ;


	/// <summary>Initializes the specified meta command.</summary>
	/// <param name="pMetaCommand">A pointer to an <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12metacommand">ID3D12MetaCommand</a> representing the meta command to initialize.</param>
	/// <param name="pInitializationParametersData">An optional pointer to a constant structure containing the values of the parameters for initializing the meta command.</param>
	/// <param name="InitializationParametersDataSizeInBytes">A <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">SIZE_T</a> containing the size of the structure pointed to by <i>pInitializationParametersData</i>, if set, otherwise 0.</param>
	/// <returns>If this method succeeds, it returns <b>S_OK</b>. Otherwise, it returns an <b>HRESULT</b> error code.</returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-initializemetacommand">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void InitializeMetaCommand( IMetaCommand    pMetaCommand,
								[Optional] nint pInitializationParametersData,
								nuint           InitializationParametersDataSizeInBytes = 0x00U ) ;


	/// <summary>Records the execution (or invocation) of the specified meta command into a graphics command list.</summary>
	/// <param name="pMetaCommand">A pointer to an <b>ID3D12MetaCommand</b> representing the meta command to initialize.</param>
	/// <param name="pExecutionParametersData">An optional pointer to a constant structure containing the values of the parameters for executing the meta command.</param>
	/// <param name="ExecutionParametersDataSizeInBytes">A <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">SIZE_T</a> containing the size of the structure pointed to by <i>pExecutionParametersData</i>, if set, otherwise 0.</param>
	/// <returns>If this method succeeds, it returns <b>S_OK</b>. Otherwise, it returns an <b>HRESULT</b> error code.</returns>
	/// <remarks>
	/// <para>Your application is responsible for setting up the resources supplied to a meta command in the state required according to the meta command specification. The meta command definition specification defines the expected resource state for each parameter. Your application is responsible for inserting unordered access view (UAV) barriers for input resources before the meta command's algorithm can consume them. You're also responsible for inserting the UAV barrier for the output resources when you intend to read them back. During an algorithm invocation, the driver may insert as many UAV barriers to output resources as are needed to synchronize the output resource usage in the algorithm implementation. From your application's point of view, you should assume that all out and in/out resources are written to by the meta command, including scratch memory.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-executemetacommand#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ExecuteMetaCommand( IMetaCommand    pMetaCommand,
							 [Optional] nint pExecutionParametersData,
							 nuint           ExecutionParametersDataSizeInBytes = 0x00U ) ;
	

	/// <summary>Performs a raytracing acceleration structure build on the GPU and optionally outputs post-build information immediately after the build.</summary>
	/// <param name="pDesc">Description of the acceleration structure to build.</param>
	/// <param name="NumPostbuildInfoDescs">Size of the <i>pPostbuildInfoDescs</i> array.  Set to 0 if no post-build info is needed.</param>
	/// <param name="pPostbuildInfoDescs">Optional array of descriptions for post-build info to generate describing properties of the acceleration structure that was built.</param>
	/// <remarks>
	/// <para>This method can be called on graphics or compute command lists but not from bundles. Post-build information can also be obtained separately from an already built acceleration structure by calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-emitraytracingaccelerationstructurepostbuildinfo">EmitRaytracingAccelerationStructurePostbuildInfo</a>.  The advantage of generating post-build info along with a build is that a barrier isn’t needed in between the build completing and requesting post-build information, enabling scenarios where the app needs the post-build info right away.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-buildraytracingaccelerationstructure#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void BuildRaytracingAccelerationStructure( in BuildRaytracingAccelerationStructureDescription pDesc, 
											   uint NumPostbuildInfoDescs,
											   [Optional] Span< RaytracingAccelerationStructurePostBuildInfoDescription > pPostbuildInfoDescs ) ;


	/// <summary>
	/// Emits post-build properties for a set of acceleration structures. This enables applications to know the output resource requirements for performing acceleration
	/// structure operations via ID3D12GraphicsCommandList4::CopyRaytracingAccelerationStructure.
	/// </summary>
	/// <param name="pDesc">A <see cref="RaytracingAccelerationStructurePostBuildInfoDescription"/> object describing post-build information to generate.</param>
	/// <param name="NumSourceAccelerationStructures">Number of pointers to acceleration structure GPU virtual addresses pointed to by <i>pSourceAccelerationStructureData</i>.
	/// This number also affects the destination (output), which will be a contiguous array of <b>NumSourceAccelerationStructures</b> output structures, where the type of the
	/// structures depends on <i>InfoType</i> field of the supplied in the <i>pDesc</i> description.</param>
	/// <param name="pSourceAccelerationStructureData">
	/// <para>Pointer to array of GPU virtual addresses of size <i>NumSourceAccelerationStructures</i>.
	/// The address must be aligned to 256 bytes, defined as
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_BYTE_ALIGNMENT</a>.
	/// The memory pointed to must be in state
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-emitraytracingaccelerationstructurepostbuildinfo#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>This method can be called from graphics or compute command lists but not from bundles.</remarks>
	void EmitRaytracingAccelerationStructurePostbuildInfo(
		in RaytracingAccelerationStructurePostBuildInfoDescription pDesc,
		uint NumSourceAccelerationStructures,
		ulong[ ] pSourceAccelerationStructureData ) ;


	/// <summary>Copies a source acceleration structure to destination memory while applying the specified transformation.</summary>
	/// <param name="DestAccelerationStructureData">
	/// <para>The destination memory. The required size can be discovered by calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-emitraytracingaccelerationstructurepostbuildinfo">EmitRaytracingAccelerationStructurePostbuildInfo</a> beforehand, if necessary for the specified <i>Mode</i>. The destination start address must be aligned to 256 bytes, defined as <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_BYTE_ALIGNMENT</a>, regardless of the specified <i>Mode</i>. The destination memory range cannot overlap source. Otherwise, results are undefined. The resource state that the memory pointed to must be in depends on the <i>Mode</i> parameter. For more information, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_copy_mode">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_COPY_MODE</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-copyraytracingaccelerationstructure#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="SourceAccelerationStructureData">
	/// <para>The address of the acceleration structure or other type of data to copy/transform based on the specified <i>Mode</i>.  The data remains unchanged and usable.  The operation only copies the data  pointed to by <i>SourceAccelerationStructureData</i> and not any other data, such as acceleration structures, that the source data may point to.  For example, in the case of a top-level acceleration structure, any bottom-level acceleration structures that it points to are not copied in the operation. The source memory must be aligned to 256 bytes, defined as <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_BYTE_ALIGNMENT</a>, regardless of the specified <i>Mode</i>. The resource state that the memory pointed to must be in depends on the <i>Mode</i> parameter. For more information, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_copy_mode">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_COPY_MODE</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-copyraytracingaccelerationstructure#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Mode">The type of copy operation to perform. For more information, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_copy_mode">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_COPY_MODE</a>.</param>
	/// <remarks>
	/// <para>Since raytracing acceleration structures may contain internal pointers and have a device dependent opaque layout, copying them around or otherwise manipulating them requires a dedicated API so that drivers can handle the requested operation. This method can be called from graphics or compute command lists but not from bundles.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-copyraytracingaccelerationstructure#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CopyRaytracingAccelerationStructure( ulong DestAccelerationStructureData,
											  ulong SourceAccelerationStructureData,
											  RaytracingAccelerationStructureCopyMode Mode ) ;
	
	
	/// <summary>Sets a state object on the command list.</summary>
	/// <param name="pStateObject">The state object to set on the command list. In the current release, this can only be of type <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_state_object_type">D3D12_STATE_OBJECT_TYPE_RAYTRACING_PIPELINE</a>.</param>
	/// <remarks>
	/// <para>This method can be called from graphics or compute command lists and bundles. This method is an alternative to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setpipelinestate">ID3D12GraphicsCommandList::SetPipelineState</a>, which is only defined for graphics and compute shaders.  There is only one pipeline state active on a command list at a time, so either call sets the current pipeline state.  The distinction between the calls is that each sets particular types of pipeline state only.  In the current release, <b>SetPipelineState1</b> is only used for setting raytracing pipeline state.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-setpipelinestate1#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetPipelineState1( IStateObject pStateObject ) ;

	
	/// <summary>Launch the threads of a ray generation shader.</summary>
	/// <param name="pDesc">A description of the ray dispatch</param>
	/// <remarks>
	/// <para>This method can be called from graphics or compute command lists and bundles.</para>
	/// <para>A raytracing pipeline state must be set on the command list. Otherwise, the behavior of this call is undefined. There are 3 dimensions passed in to set the grid size:
	/// width/height/depth.  These dimensions are constrained such that width * height * depth &lt;= 2^30. Exceeding this produces undefined behavior. If any grid dimension is 0,
	/// no threads are launched.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-dispatchrays#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void DispatchRays( in DispatchRaysDescription pDesc ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( ID3D12GraphicsCommandList4 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList4).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new GraphicsCommandList4( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new GraphicsCommandList4( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new GraphicsCommandList4( (ID3D12GraphicsCommandList4?)pComObj! ) ;
	// ===================================================================================
} ;


// ===================================================================================
// ID3D12GraphicsCommandList5 interface ::
// ===================================================================================

[ProxyFor( typeof( ID3D12GraphicsCommandList5 ) )]
public interface IGraphicsCommandList5: IGraphicsCommandList4 {
	// ---------------------------------------------------------------------------------
	
	/// <summary>The ID3D12GraphicsCommandList5::RSSetShadingRate method (d3d12.h) sets the base shading rate, and combiners, for variable-rate shading (VRS).</summary>
	/// <param name="baseShadingRate">
	/// <para>A constant from the <see cref="ShadingRate"/> enumeration describing the base shading rate to set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist5-rssetshadingrate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="combiners">
	/// <para>An (optional) pointer to a constant array of <see cref="ShadingRateCombiner"/> values containing the shading rate combiners to set.
	/// The count of <see cref="ShadingRateCombiner"/> elements in the array must be equal
	/// to the constant <b>D3D12_RS_SET_SHADING_RATE_COMBINER_COUNT</b>, which is equal to <b>2</b>.
	/// Because per-primitive and screen-space image-based <a href="https://learn.microsoft.com/en-us/windows/win32/direct3d12/vrs">VRS</a> isn't supported on Tier1, for these values to be meaningful,
	/// the adapter requires Tier2 VRS support. See <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options6">D3D12_FEATURE_DATA_D3D12_OPTIONS6</a> and
	/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/ne-d3d12-d3d12_variable_shading_rate_tier">D3D12_VARIABLE_SHADING_RATE_TIER</a>.<para/>
	/// A <b>NULL</b> pointer is equivalent to the default shading combiners, which are both <see cref="ShadingRateCombiner.Passthrough"/>.
	/// The algorithm for final shading-rate is determined by the following.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist5-rssetshadingrate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	void RSSetShadingRate( ShadingRate baseShadingRate, [Optional] Span< ShadingRateCombiner > combiners ) ;

	
	/// <summary>The ID3D12GraphicsCommandList5::RSSetShadingRateImage method (d3d12.h) sets the screen-space shading-rate image for variable-rate shading (VRS).</summary>
	/// <param name="shadingRateImage">
	/// <para>Type: **[ID3D12Resource](/windows/desktop/api/d3d12/nn-d3d12-id3d12resource)\*** An optional pointer to an [ID3D12Resource](/windows/desktop/api/d3d12/nn-d3d12-id3d12resource) representing a screen-space shading-rate image. If **NULL**, the effect is the same as having a shading-rate image where all values are a shading rate of 1x1. This texture must have the [**D3D12_RESOURCE_STATE_SHADING_RATE_SOURCE**](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states) state applied. The tile-size of the shading-rate image can be determined via [**D3D12_FEATURE_DATA_D3D12_OPTIONS6**](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options6). The size of the shading-rate image should therefore be </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist5-rssetshadingrateimage#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>For the screen-space shading-rate image to take affect, [**ID3D12GraphicsCommandList5::RSSetShadingRate**](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist5-rssetshadingrate) must have been called to set the combiners for shading. Else, with the default combiners (both [**D3D12_SHADING_RATE_COMBINER_PASSTHROUGH**](/windows/win32/api/d3d12/ne-d3d12-d3d12_shading_rate_combiner)), the screen-space shading-rate image is ignored in determining shading granularity. The second combiner passed to  [**ID3D12GraphicsCommandList5::RSSetShadingRate**] is the one which applies to the shading-rate image, which occurs after the global shading rate and the per-primitive shading rate have been combined. The algorithm for final shading-rate is determined by </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist5-rssetshadingrateimage#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void RSSetShadingRateImage( IResource shadingRateImage ) ;
	
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( ID3D12GraphicsCommandList5 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList5).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new GraphicsCommandList5( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new GraphicsCommandList5( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new GraphicsCommandList5( (ID3D12GraphicsCommandList5?)pComObj! ) ;
	// ===================================================================================
} ;


// ===================================================================================
// ID3D12GraphicsCommandList6 interface ::
// ===================================================================================

[ProxyFor( typeof( ID3D12GraphicsCommandList6 ) )]
public interface IGraphicsCommandList6: IGraphicsCommandList5 {
	// ---------------------------------------------------------------------------------

	void DispatchMesh( uint ThreadGroupCountX, uint ThreadGroupCountY, uint ThreadGroupCountZ ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( ID3D12GraphicsCommandList6 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList6).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new GraphicsCommandList6( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new GraphicsCommandList6( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new GraphicsCommandList6( (ID3D12GraphicsCommandList6?)pComObj! ) ;
	// ===================================================================================
} ;


// ===================================================================================
// ID3D12GraphicsCommandList7 interface ::
// ===================================================================================
[ProxyFor( typeof( ID3D12GraphicsCommandList7 ) )]
public interface IGraphicsCommandList7: IGraphicsCommandList6 {
	// ---------------------------------------------------------------------------------
	
	/// <summary>Adds a collection of barriers into a graphics command list recording.</summary>
	/// <param name="NumBarrierGroups">Number of barrier groups pointed to by *pBarrierGroups*.</param>
	/// <param name="pBarrierGroups">Pointer to an array of [D3D12_BARRIER_GROUP](/windows/win32/api/d3d12/ns-d3d12-d3d12_barrier_group) objects.</param>
	void Barrier( uint NumBarrierGroups, BarrierGroup[] pBarrierGroups ) ;

	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( ID3D12GraphicsCommandList7 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList7).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new GraphicsCommandList7( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new GraphicsCommandList7( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new GraphicsCommandList7( (ID3D12GraphicsCommandList7?)pComObj! ) ;
	
	// ===================================================================================
} ;

[ProxyFor( typeof( ID3D12GraphicsCommandList8 ) )]
public interface IGraphicsCommandList8: IGraphicsCommandList7 {
	// ---------------------------------------------------------------------------------

	void OMSetFrontAndBackStencilRef( uint FrontStencilRef, uint BackStencilRef ) ;

	// ---------------------------------------------------------------------------------

	new static Type ComType => typeof( ID3D12GraphicsCommandList8 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12GraphicsCommandList8 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference( data ) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new GraphicsCommandList8( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new GraphicsCommandList8( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) =>
		new GraphicsCommandList8( (ID3D12GraphicsCommandList8?)pComObj! ) ;

	// ===================================================================================
} ;

[ProxyFor(typeof(ID3D12GraphicsCommandList9))]
public interface IGraphicsCommandList9: IGraphicsCommandList8 {
	// ---------------------------------------------------------------------------------

	void RSSetDepthBias( float DepthBias, float DepthBiasClamp, float SlopeScaledDepthBias ) ;

	void IASetIndexBufferStripCutValue( IndexBufferStripCutValue IBStripCutValue ) ;
	
	// ---------------------------------------------------------------------------------

	new static Type ComType => typeof( ID3D12GraphicsCommandList8 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12GraphicsCommandList9 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference( data ) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new GraphicsCommandList9( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new GraphicsCommandList9( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) =>
		new GraphicsCommandList9( (ID3D12GraphicsCommandList9?)pComObj! ) ;

	// ===================================================================================
} ;

