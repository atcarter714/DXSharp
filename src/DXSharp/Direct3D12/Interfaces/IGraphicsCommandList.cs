#region Using Directives

using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12GraphicsCommandList))]
public unsafe interface IGraphicsCommandList: ICommandList,
											  IComObjectRef< ID3D12GraphicsCommandList >,
											  IUnknownWrapper< ID3D12GraphicsCommandList > {
	// ---------------------------------------------------------------------------------
	new ComPtr< ID3D12GraphicsCommandList > ComPointer { get ; }
	new ID3D12GraphicsCommandList? COMObject => ComPointer?.Interface ;
	ID3D12GraphicsCommandList? IComObjectRef< ID3D12GraphicsCommandList >.COMObject => COMObject ;
	ComPtr< ID3D12GraphicsCommandList >? IUnknownWrapper< ID3D12GraphicsCommandList >.ComPointer => ComPointer ;
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
	void Reset( ID3D12CommandAllocator pAllocator, ID3D12PipelineState pInitialState ) ;

	
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
						uint StartVertexLocation, uint StartInstanceLocation ) ;
	

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
	void CopyBufferRegion( ID3D12Resource pDstBuffer, ulong DstOffset, 
						   ID3D12Resource pSrcBuffer, ulong SrcOffset, 
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
	void CopyTextureRegion( in D3D12_TEXTURE_COPY_LOCATION pDst, 
							uint DstX, uint DstY, uint DstZ, 
							in D3D12_TEXTURE_COPY_LOCATION pSrc, 
							[Optional] D3D12_BOX* pSrcBox ) ;
	

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
	void CopyResource(ID3D12Resource pDstResource, ID3D12Resource pSrcResource) ;

	
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
	void CopyTiles( ID3D12Resource pTiledResource,
					TiledResourceCoordinate* pTileRegionStartCoordinate, 
					TileRegionSize* pTileRegionSize,
					ID3D12Resource pBuffer,
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
	void ResolveSubresource( ID3D12Resource pDstResource, uint DstSubresource, 
							 ID3D12Resource pSrcResource, uint SrcSubresource, 
							 Format Format) ;

	
	/// <summary>Bind information about the primitive type, and data order that describes input data for the input assembler stage. (ID3D12GraphicsCommandList.IASetPrimitiveTopology)</summary>
	/// <param name="PrimitiveTopology">
	/// <para>Type: <b>D3D12_PRIMITIVE_TOPOLOGY</b> The type of primitive and ordering of the primitive data (see <a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/ne-d3dcommon-d3d_primitive_topology">D3D_PRIMITIVE_TOPOLOGY</a>).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-iasetprimitivetopology#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-iasetprimitivetopology">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void IASetPrimitiveTopology( D3D_PRIMITIVE_TOPOLOGY PrimitiveTopology ) ;
	
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
	void RSSetViewports(uint NumViewports, D3D12_VIEWPORT* pViewports) ;

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
	/// <para>Which scissor rectangle to use is determined by the <b>SV_ViewportArrayIndex</b> semantic output by a geometry shader (see shader semantic syntax). If a geometry shader does not make use of the <code>SV_ViewportArrayIndex</c> semantic then Direct3D will use the first scissor rectangle in the array.</para>
	/// <para>Each scissor rectangle in the array corresponds to a viewport in an array of viewports (see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-rssetviewports">RSSetViewports</a>).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-rssetscissorrects#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void RSSetScissorRects(uint NumRects, in Rect pRects) ;

	/// <summary>Sets the blend factor that modulate values for a pixel shader, render target, or both.</summary>
	/// <param name="BlendFactor">
	/// <para>Type: <b>const FLOAT[4]</b> Array of blend factors, one for each RGBA component.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>If you created the blend-state object with [D3D12_BLEND_BLEND_FACTOR](./ne-d3d12-d3d12_blend.md) or **D3D12_BLEND_INV_BLEND_FACTOR**, then the blending stage uses the non-NULL array of blend factors. Otherwise,the blending stage doesn't use the non-NULL array of blend factors; the runtime stores the blend factors. If you pass NULL, then the runtime uses or stores a blend factor equal to `{ 1, 1, 1, 1 }`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void OMSetBlendFactor([MarshalAs(UnmanagedType.LPArray,SizeConst = 4)] float[] BlendFactor) ;

	/// <summary>Sets the reference value for depth stencil tests.</summary>
	/// <param name="StencilRef">
	/// <para>Type: <b>UINT</b> Reference value to perform against when doing a depth-stencil test.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetstencilref#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetstencilref">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void OMSetStencilRef(uint StencilRef) ;

	/// <summary>Sets all shaders and programs most of the fixed-function state of the graphics processing unit (GPU) pipeline.</summary>
	/// <param name="pPipelineState">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a>*</b> Pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> containing the pipeline state data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setpipelinestate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setpipelinestate">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetPipelineState(ID3D12PipelineState pPipelineState) ;

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
						  D3D12_RESOURCE_BARRIER[ ] pBarriers ) ;
	
	
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
	void ExecuteBundle( ID3D12GraphicsCommandList pCommandList ) ;

	
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
							 ID3D12DescriptorHeap[ ] ppDescriptorHeaps) ;

	/// <summary>Sets the layout of the compute root signature.</summary>
	/// <param name="pRootSignature">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a> object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setcomputerootsignature">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetComputeRootSignature(ID3D12RootSignature pRootSignature) ;

	/// <summary>Sets the layout of the graphics root signature.</summary>
	/// <param name="pRootSignature">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a> object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setgraphicsrootsignature">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetGraphicsRootSignature(ID3D12RootSignature pRootSignature) ;

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
	void SetComputeRootDescriptorTable(uint RootParameterIndex, GPUDescriptorHandle BaseDescriptor) ;

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
	void SetGraphicsRootDescriptorTable(uint RootParameterIndex, GPUDescriptorHandle BaseDescriptor) ;

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
	void SetComputeRoot32BitConstant(uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues) ;

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
	void SetGraphicsRoot32BitConstant(uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues) ;

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
	void SetComputeRoot32BitConstants(uint RootParameterIndex, uint Num32BitValuesToSet, void* pSrcData, uint DestOffsetIn32BitValues) ;

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
	void SetGraphicsRoot32BitConstants(uint RootParameterIndex, uint Num32BitValuesToSet, void* pSrcData, uint DestOffsetIn32BitValues) ;

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
	void SetComputeRootConstantBufferView(uint RootParameterIndex, ulong BufferLocation) ;

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
	void SetGraphicsRootConstantBufferView(uint RootParameterIndex, ulong BufferLocation) ;

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
	void SetComputeRootShaderResourceView(uint RootParameterIndex, ulong BufferLocation) ;

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
	void SetGraphicsRootShaderResourceView(uint RootParameterIndex, ulong BufferLocation) ;

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
	void SetComputeRootUnorderedAccessView(uint RootParameterIndex, ulong BufferLocation) ;

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
	void SetGraphicsRootUnorderedAccessView(uint RootParameterIndex, ulong BufferLocation) ;

	/// <summary>Sets the view for the index buffer.</summary>
	/// <param name="pView">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_index_buffer_view">IndexBufferView</a>*</b> The view specifies the index buffer's address, size, and <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">Format</a>, as a pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_index_buffer_view">IndexBufferView</a> structure.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-iasetindexbuffer#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>Only one index buffer can be bound to the graphics pipeline at any one time.</remarks>
	void IASetIndexBuffer([Optional] IndexBufferView* pView) ;
	
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
	void IASetVertexBuffers(uint StartSlot, uint NumViews, [Optional] VertexBufferView* pViews) ;

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
	void SOSetTargets(uint StartSlot, uint NumViews, [Optional] StreamOutputBufferView* pViews) ;

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
	void OMSetRenderTargets(uint NumRenderTargetDescriptors, [Optional] CPUDescriptorHandle* pRenderTargetDescriptors, BOOL RTsSingleHandleToDescriptorRange, [Optional] CPUDescriptorHandle* pDepthStencilDescriptor) ;

	/// <summary>Clears the depth-stencil resource. (ID3D12GraphicsCommandList.ClearDepthStencilView)</summary>
	/// <param name="DepthStencilView">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the start of the heap for the depth stencil to be cleared.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-cleardepthstencilview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ClearFlags">
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
	void ClearDepthStencilView(CPUDescriptorHandle DepthStencilView, D3D12_CLEAR_FLAGS ClearFlags, float Depth, byte Stencil, uint NumRects, Rect* pRects) ;

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
	void ClearRenderTargetView(CPUDescriptorHandle RenderTargetView, [MarshalAs(UnmanagedType.LPArray,SizeConst = 4)] float[] ColorRGBA, uint NumRects, [Optional] Rect* pRects) ;

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
	void ClearUnorderedAccessViewUint(GPUDescriptorHandle ViewGPUHandleInCurrentHeap, CPUDescriptorHandle ViewCPUHandle, ID3D12Resource pResource, [MarshalAs(UnmanagedType.LPArray,SizeConst = 4)] uint[] Values, uint NumRects, Rect* pRects) ;

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
											   ID3D12Resource pResource, 
											   [MarshalAs(UnmanagedType.LPArray,SizeConst = 4)] 
											   float[] Values, uint NumRects, Rect* pRects) ;

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
	void DiscardResource(ID3D12Resource pResource, [Optional] DiscardRegion* pRegion) ;
	
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
	void BeginQuery(ID3D12QueryHeap pQueryHeap, D3D12_QUERY_TYPE Type, uint Index) ;

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
	void EndQuery(ID3D12QueryHeap pQueryHeap, D3D12_QUERY_TYPE Type, uint Index) ;

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
	void ResolveQueryData(ID3D12QueryHeap pQueryHeap, D3D12_QUERY_TYPE Type, uint StartIndex, uint NumQueries, ID3D12Resource pDestinationBuffer, ulong AlignedDestinationBufferOffset) ;

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
	void SetPredication(ID3D12Resource pBuffer, ulong AlignedBufferOffset, D3D12_PREDICATION_OP Operation) ;

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
	void SetMarker(uint Metadata, [Optional] void* pData, uint Size) ;

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
	void BeginEvent(uint Metadata, [Optional] void* pData, uint Size) ;

	
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
	void ExecuteIndirect( ID3D12CommandSignature pCommandSignature, 
						  uint MaxCommandCount, 
						  ID3D12Resource pArgumentBuffer, 
						  ulong ArgumentBufferOffset, 
						  ID3D12Resource pCountBuffer, 
						  ulong CountBufferOffset ) ;

	
	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof(ID3D12GraphicsCommandList) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12GraphicsCommandList).GUID ;
	// ==================================================================================
	
}