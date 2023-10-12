#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Security ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
using DXSharp.Windows.Win32.Helpers ;
#endregion
namespace DXSharp.Direct3D12 ;

/// <summary>Wrapper interface for the native ID3D12Device COM interface</summary>
[Wrapper( typeof( ID3D12Device ) ) ]
public interface IDevice: IObject, IUnknownWrapper< ID3D12Device > {
	static Guid IUnknownWrapper< ID3D12Device >.InterfaceGUID => 
		typeof(ID3D12DeviceChild).GUID ;
	
	
	
	/// <summary>Reports the number of physical adapters (nodes) that are associated with this device.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of physical adapters (nodes) that this device has.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getnodecount">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	uint GetNodeCount( ) ;
	
	/// <summary>Creates a command queue.</summary>
	/// <param name="pDesc">
	/// <para>Type: [in] <b>const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_command_queue_desc">D3D12_COMMAND_QUEUE_DESC</a>*</b> Specifies a **D3D12_COMMAND_QUEUE_DESC** that describes the command queue.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandqueue#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b><b>REFIID</b></b> The globally unique identifier (GUID) for the command queue interface. See **Remarks**. An input parameter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandqueue#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppCommandQueue">
	/// <para>Type: [out] <b><b>void</b>**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a> interface for the command queue.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandqueue#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the command queue. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 return codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>The <b>REFIID</b>, or <b>GUID</b>, of the interface to the command queue can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12CommandQueue) will get the <b>GUID</b> of the interface to a command queue.</remarks>
	/// void CreateCommandQueue( D3D12_COMMAND_QUEUE_DESC* pDesc, Guid* riid, out object ppCommandQueue ) ;
	void CreateCommandQueue( in CommandQueueDescription pDesc, in Guid riid, out ICommandQueue ppCommandQueue ) ;
	
	/// <summary>Creates a command allocator object.</summary>
	/// <param name="type">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_command_list_type">D3D12_COMMAND_LIST_TYPE</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_command_list_type">D3D12_COMMAND_LIST_TYPE</a>-typed value that specifies the type of command allocator to create. The type of command allocator can be the type that records either direct command lists or bundles.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandallocator#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the command allocator interface (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandallocator">ID3D12CommandAllocator</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the command allocator can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12CommandAllocator) will get the <b>GUID</b> of the interface to a command allocator.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandallocator#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppCommandAllocator">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandallocator">ID3D12CommandAllocator</a> interface for the command allocator.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandallocator#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the command allocator. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>The device creates command lists from the command allocator.</remarks>
	void CreateCommandAllocator( CommandListType type, in Guid riid, out ICommandAllocator ppCommandAllocator ) ;

	/// <summary>Creates a graphics pipeline state object.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_graphics_pipeline_state_desc">D3D12_GRAPHICS_PIPELINE_STATE_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_graphics_pipeline_state_desc">D3D12_GRAPHICS_PIPELINE_STATE_DESC</a> structure that describes graphics pipeline state.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-creategraphicspipelinestate#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the pipeline state interface (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the pipeline state can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12PipelineState) will get the <b>GUID</b> of the interface to a pipeline state.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-creategraphicspipelinestate#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppPipelineState">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> interface for the pipeline state object. The pipeline state object is an immutable state object.  It contains no methods.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-creategraphicspipelinestate#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the pipeline state object. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-creategraphicspipelinestate">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateGraphicsPipelineState( in GraphicsPipelineStateDescription pDesc,
											 in Guid riid,
											 out IPipelineState ppPipelineState ) ;

	/// <summary>Creates a compute pipeline state object.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_compute_pipeline_state_desc">D3D12_COMPUTE_PIPELINE_STATE_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_compute_pipeline_state_desc">D3D12_COMPUTE_PIPELINE_STATE_DESC</a> structure that describes compute pipeline state.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcomputepipelinestate#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the pipeline state interface (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the pipeline state can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12PipelineState) will get the <b>GUID</b> of the interface to a pipeline state.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcomputepipelinestate#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppPipelineState">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> interface for the pipeline state object. The pipeline state object is an immutable state object.  It contains no methods.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcomputepipelinestate#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the pipeline state object. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcomputepipelinestate">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateComputePipelineState( in ComputePipelineStateDescription pDesc,
											in Guid riid, out IPipelineState ppPipelineState) ;

	/// <summary>Creates a command list.</summary>
	/// <param name="nodeMask">
	/// <para>Type: **[UINT](/windows/win32/WinProg/windows-data-types)** For single-GPU operation, set this to zero. If there are multiple GPU nodes, then set a bit to identify the node (the device's physical adapter) for which to create the command list. Each bit in the mask corresponds to a single node. Only one bit must be set. Also see [Multi-adapter systems](/windows/win32/direct3d12/multi-engine).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="type">
	/// <para>Type: **[D3D12_COMMAND_LIST_TYPE](./ne-d3d12-d3d12_command_list_type.md)** Specifies the type of command list to create.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pCommandAllocator">
	/// <para>Type: **[ID3D12CommandAllocator](./nn-d3d12-id3d12commandallocator.md)\*** A pointer to the command allocator object from which the device creates command lists.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pInitialState">
	/// <para>Type: **[ID3D12PipelineState](./nn-d3d12-id3d12pipelinestate.md)\*** An optional pointer to the pipeline state object that contains the initial pipeline state for the command list. If it is `nullptr`, then the runtime sets a dummy initial pipeline state, so that drivers don't have to deal with undefined state. The overhead for this is low, particularly for a command list, for which the overall cost of recording the command list likely dwarfs the cost of a single initial state setting. So there's little cost in not setting the initial pipeline state parameter, if doing so is inconvenient. For bundles, on the other hand, it might make more sense to try to set the initial state parameter (since bundles are likely smaller overall, and can be reused frequently).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the command list interface to return in *ppCommandList*.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppCommandList">
	/// <para>Type: **void\*\*** A pointer to a memory block that receives a pointer to the [ID3D12CommandList](./nn-d3d12-id3d12commandlist.md) or [ID3D12GraphicsCommandList](./nn-d3d12-id3d12graphicscommandlist.md) interface for the command list.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the command list.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>The device creates command lists from the command allocator.</remarks>
	void CreateCommandList( uint nodeMask, CommandListType type,
								ICommandAllocator pCommandAllocator, 
								IPipelineState pInitialState,
								in Guid riid, out ICommandList ppCommandList ) ;

	/// <summary>
	/// Gets information about the features that are supported by the current graphics driver. (ID3D12Device.CheckFeatureSupport)
	/// </summary>
	/// <param name="Feature">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a></b> A constant from the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a> enumeration describing the feature(s) that you want to query for support.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pFeatureSupportData">
	/// <para>Type: <b>void*</b> A pointer to a data structure that corresponds to the value of the <i>Feature</i> parameter. To determine the corresponding data structure for each constant, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="FeatureSupportDataSize">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The size of the structure pointed to by the <i>pFeatureSupportData</i> parameter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns <b>S_OK</b> if successful. Returns <b>E_INVALIDARG</b> if an unsupported data type is passed to the <i>pFeatureSupportData</i> parameter or if a size mismatch is detected for the <i>FeatureSupportDataSize</i> parameter.</para>
	/// </returns>
	/// <remarks>
	/// <para>As a usage example, to check for ray tracing support, specify the <a href="../d3d12/ns-d3d12-d3d12_feature_data_d3d12_options5.md">D3D12_FEATURE_DATA_D3D12_OPTIONS5</a> structure in the <i>pFeatureSupportData</i> parameter. When the function completes successfully, access the <i>RaytracingTier</i> field (which specifies the supported ray tracing tier) of the now-populated <b>D3D12_FEATURE_DATA_D3D12_OPTIONS5</b> structure. For more info, see <a href="https://docs.microsoft.com/windows/desktop/direct3d12/capability-querying">Capability Querying</a>. <h3><a id="Hardware_support_for_DXGI_Formats"></a><a id="hardware_support_for_dxgi_formats"></a><a id="HARDWARE_SUPPORT_FOR_DXGI_FORMATS"></a>Hardware support for DXGI Formats</h3> To view tables of DXGI formats and hardware features, refer to: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CheckFeatureSupport( D3D12Feature Feature, nint pFeatureSupportData, uint FeatureSupportDataSize ) ;
	
	/// <summary>Creates a descriptor heap object.</summary>
	/// <param name="pDescriptorHeapDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_heap_desc">D3D12_DESCRIPTOR_HEAP_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_heap_desc">D3D12_DESCRIPTOR_HEAP_DESC</a> structure that describes the heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdescriptorheap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b><b>REFIID</b></b> The globally unique identifier (<b>GUID</b>) for the descriptor heap interface. See Remarks. An input parameter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdescriptorheap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvHeap">
	/// <para>Type: <b><b>void</b>**</b> A pointer to a memory block that receives a pointer to the descriptor heap. <i>ppvHeap</i> can be NULL, to enable capability testing. When <i>ppvHeap</i> is NULL, no object will be created and S_FALSE will be returned when <i>pDescriptorHeapDesc</i> is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdescriptorheap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the descriptor heap object. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>The <b>REFIID</b>, or <b>GUID</b>, of the interface to the descriptor heap can be obtained by using the __uuidof() macro. For example, __uuidof(<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12descriptorheap">ID3D12DescriptorHeap</a>) will get the <b>GUID</b> of the interface to a descriptor heap.</remarks>
	void CreateDescriptorHeap( in DescriptorHeapDescription pDescriptorHeapDesc,
									  in Guid riid, out IDescriptorHeap ppvHeap ) ;
	
	
	/// <summary>Gets the size of the handle increment for the given type of descriptor heap. This value is typically used to increment a handle into a descriptor array by the correct amount.</summary>
	/// <param name="DescriptorHeapType">The <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a>-typed value that specifies the type of descriptor heap to get the size of the handle increment for.</param>
	/// <returns>Returns the size of the handle increment for the given type of descriptor heap, including any necessary padding.</returns>
	/// <remarks>The descriptor size returned by this method is used as one input to the helper structures <a href="https://docs.microsoft.com/windows/desktop/direct3d12/cd3dx12-cpu-descriptor-handle">CD3DX12_CPU_DESCRIPTOR_HANDLE</a> and <a href="https://docs.microsoft.com/windows/desktop/direct3d12/cd3dx12-gpu-descriptor-handle">CD3DX12_GPU_DESCRIPTOR_HANDLE</a>.</remarks>
	uint GetDescriptorHandleIncrementSize( DescriptorHeapType DescriptorHeapType ) ;
	
	/// <summary>Creates a root signature layout.</summary>
	/// <param name="nodeMask">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT</a></b> For single GPU operation, set this to zero. If there are multiple GPU nodes, set bits to identify the nodes (the  device's physical adapters) to which the root signature is to apply. Each bit in the mask corresponds to a single node. Refer to <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pBlobWithRootSignature">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">void</a>*</b> A pointer to the source data for the serialized signature.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="blobLengthInBytes">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">SIZE_T</a></b> The size, in bytes, of the block of memory that <i>pBlobWithRootSignature</i> points to.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b><b>REFIID</b></b> The globally unique identifier (<b>GUID</b>) for the root signature interface. See Remarks. An input parameter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvRootSignature">
	/// <para>Type: <b><b>void</b>**</b> A pointer to a memory block that receives a pointer to the root signature.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns <b>S_OK</b> if successful; otherwise, returns one of the <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// <para>This method returns <b>E_INVALIDARG</b> if the blob that <i>pBlobWithRootSignature</i> points to is invalid.</para>
	/// </returns>
	/// <remarks>
	/// <para>If an application procedurally generates a <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_root_signature_desc">D3D12_ROOT_SIGNATURE_DESC</a> data structure, it must pass a pointer to this <b>D3D12_ROOT_SIGNATURE_DESC</b> in a call to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-d3d12serializerootsignature">D3D12SerializeRootSignature</a> to make the serialized form. The application then passes the serialized form to <i>pBlobWithRootSignature</i> in a call to <b>ID3D12Device::CreateRootSignature</b>.</para>
	/// <para>The <b>REFIID</b>, or <b>GUID</b>, of the interface to the root signature layout can be obtained by using the __uuidof() macro. For example, __uuidof(<a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a>) will get the <b>GUID</b> of the interface to a root signature.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateRootSignature( uint nodeMask,
							  nint pBlobWithRootSignature, 
							  nuint blobLengthInBytes, in Guid riid, 
								out IRootSignature ppvRootSignature ) ;
	

	/// <summary>Creates a constant-buffer view for accessing resource data.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_constant_buffer_view_desc">D3D12_CONSTANT_BUFFER_VIEW_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_constant_buffer_view_desc">D3D12_CONSTANT_BUFFER_VIEW_DESC</a> structure that describes the constant-buffer view.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createconstantbufferview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the start of the heap that holds the constant-buffer view.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createconstantbufferview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createconstantbufferview">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateConstantBufferView( [Optional] in ConstBufferViewDescription pDesc,
													CPUDescriptorHandle DestDescriptor ) ;

	/// <summary>Creates a shader-resource view for accessing data in a resource. (ID3D12Device.CreateShaderResourceView)</summary>
	/// <param name="pResource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> object that represents the shader resource. At least one of <i>pResource</i> or <i>pDesc</i>  must be provided. A null <i>pResource</i> is used to initialize a null descriptor, which guarantees D3D11-like null binding behavior (reading 0s, writes are discarded), but must have a valid <i>pDesc</i> in order to determine the descriptor type.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_shader_resource_view_desc">D3D12_SHADER_RESOURCE_VIEW_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_shader_resource_view_desc">D3D12_SHADER_RESOURCE_VIEW_DESC</a> structure that describes the shader-resource view. A null <i>pDesc</i> is used to initialize a default descriptor, if possible. This behavior is identical to the D3D11 null descriptor behavior, where defaults are filled in. This behavior inherits the resource format and dimension (if not typeless) and for buffers SRVs target a full buffer and are typed (not raw or structured), and for textures SRVs target a full texture, all mips and all array slices. Not all resources support null descriptor initialization.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the shader-resource view. This handle can be created in a shader-visible or non-shader-visible descriptor heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para><h3><a id="Processing_YUV_4_2_0_video_formats"></a><a id="processing_yuv_4_2_0_video_formats"></a><a id="PROCESSING_YUV_4_2_0_VIDEO_FORMATS"></a>Processing YUV 4:2:0 video formats</h3> An app must map the luma (Y) plane separately from the chroma (UV) planes. Developers do this by calling <b>CreateShaderResourceView</b> twice for the same texture and passing in 1-channel and 2-channel formats. Passing in a 1-channel format compatible with the Y plane maps only the Y plane. Passing in a 2-channel format compatible with the UV planes (together) maps only the U and V planes as a single resource view. YUV 4:2:0 formats are listed in <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateShaderResourceView( IResource pResource,
									[Optional] in ShaderResourceViewDescription pDesc, 
										CPUDescriptorHandle DestDescriptor ) ;

	/// <summary>Creates a view for unordered accessing.</summary>
	/// <param name="pResource">
	/// <para>Type: [in, optional] <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> object that represents the unordered access. At least one of <i>pResource</i> or <i>pDesc</i> must be provided. A null <i>pResource</i> is used to initialize a null descriptor, which guarantees Direct3D 11-like null binding behavior (reading 0s, writes are discarded), but must have a valid <i>pDesc</i> in order to determine the descriptor type.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createunorderedaccessview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pCounterResource">
	/// <para>Type: [in, optional] <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> The <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> for the counter (if any) associated with the UAV. If <i>pCounterResource</i> is not specified, then the <b>CounterOffsetInBytes</b> member of the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_buffer_uav">D3D12_BUFFER_UAV</a> structure must be 0. If <i>pCounterResource</i> is specified, then there is a counter associated with the UAV, and the runtime performs validation of the following requirements: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createunorderedaccessview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: [in, optional] <b>const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_unordered_access_view_desc">D3D12_UNORDERED_ACCESS_VIEW_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_unordered_access_view_desc">D3D12_UNORDERED_ACCESS_VIEW_DESC</a> structure that describes the unordered-access view. A null <i>pDesc</i> is used to initialize a default descriptor, if possible. This behavior is identical to the D3D11 null descriptor behavior, where defaults are filled in. This behavior inherits the resource format and dimension (if not typeless) and for buffers UAVs target a full buffer and are typed, and for textures UAVs target the first mip and all array slices. Not all resources support null descriptor initialization.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createunorderedaccessview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the start of the heap that holds the unordered-access view.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createunorderedaccessview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createunorderedaccessview">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateUnorderedAccessView( IResource pResource,
											IResource pCounterResource,
												out UnorderedAccessViewDescription pDesc,
													CPUDescriptorHandle DestDescriptor ) ;

	/// <summary>Creates a render-target view for accessing resource data. (ID3D12Device.CreateRenderTargetView)</summary>
	/// <param name="pResource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> object that represents the render target.</para>
	/// <para>At least one of <i>pResource</i> or <i>pDesc</i>  must be provided. A null <i>pResource</i> is used to initialize a null descriptor, which guarantees D3D11-like null binding behavior (reading 0s, writes are discarded), but must have a valid <i>pDesc</i> in order to determine the descriptor type.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrendertargetview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_target_view_desc">D3D12_RENDER_TARGET_VIEW_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_target_view_desc">D3D12_RENDER_TARGET_VIEW_DESC</a> structure that describes the render-target view. A null <i>pDesc</i> is used to initialize a default descriptor, if possible. This behavior is identical to the D3D11 null descriptor behavior, where defaults are filled in. This behavior inherits the resource format and dimension (if not typeless) and RTVs target the first mip and all array slices. Not all resources support null descriptor initialization.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrendertargetview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the destination where the newly-created render target view will reside.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrendertargetview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrendertargetview">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateRenderTargetView( IResource pResource, 
									[Optional] in RenderTargetViewDesc pDesc, 
										CPUDescriptorHandle DestDescriptor ) ;

	/// <summary>Creates a depth-stencil view for accessing resource data.</summary>
	/// <param name="pResource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> object that represents the depth stencil.</para>
	/// <para>At least one of <i>pResource</i> or <i>pDesc</i>  must be provided. A null <i>pResource</i> is used to initialize a null descriptor, which guarantees D3D11-like null binding behavior (reading 0s, writes are discarded), but must have a valid <i>pDesc</i> in order to determine the descriptor type.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdepthstencilview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_depth_stencil_view_desc">D3D12_DEPTH_STENCIL_VIEW_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_depth_stencil_view_desc">D3D12_DEPTH_STENCIL_VIEW_DESC</a> structure that describes the depth-stencil view.</para>
	/// <para>A null <i>pDesc</i> is used to initialize a default descriptor, if possible. This behavior is identical to the D3D11 null descriptor behavior, where defaults are filled in. This behavior inherits the resource format and dimension (if not typeless) and DSVs target the  first mip and all array slices. Not all resources support null descriptor initialization.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdepthstencilview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the start of the heap that holds the depth-stencil view.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdepthstencilview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdepthstencilview">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateDepthStencilView( IResource pResource,
									 [Optional] in DepthStencilViewDesc pDesc,
											CPUDescriptorHandle DestDescriptor ) ;

	
	/// <summary>Create a sampler object that encapsulates sampling information for a texture.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_sampler_desc">D3D12_SAMPLER_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_sampler_desc">D3D12_SAMPLER_DESC</a> structure that describes the sampler.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsampler#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the start of the heap that holds the sampler.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsampler#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsampler">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateSampler( in SamplerDescription pDesc, CPUDescriptorHandle DestDescriptor );

	
	/// <summary>Copies descriptors from a source to a destination. (ID3D12Device.CopyDescriptors)</summary>
	/// <param name="NumDestDescriptorRanges">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of destination descriptor ranges to copy to.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDestDescriptorRangeStarts">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a>*</b> An array of <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> objects to copy to. All the destination and source descriptors must be in heaps of the same [D3D12_DESCRIPTOR_HEAP_TYPE](/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDestDescriptorRangeSizes">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> An array of destination descriptor range sizes to copy to.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="NumSrcDescriptorRanges">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of source descriptor ranges to copy from.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pSrcDescriptorRangeStarts">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a>*</b> An array of <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> objects to copy from. > [!IMPORTANT] > All elements in the *pSrcDescriptorRangeStarts* parameter must be in a non shader-visible descriptor heap. This is because shader-visible descriptor heaps may be created in **WRITE_COMBINE** memory or GPU local memory, which is prohibitively slow to read from. If your application manages descriptor heaps via copying the descriptors required for a given pass or frame from local "storage" descriptor heaps to the GPU-bound descriptor heap, use shader-opaque heaps for the storage heaps and copy into the GPU-visible heap as required.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pSrcDescriptorRangeSizes">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> An array of source descriptor range sizes to copy from.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DescriptorHeapsType">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a></b> The <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a>-typed value that specifies the type of descriptor heap to copy with. This is required as different descriptor types may have different sizes. Both the source and destination descriptor heaps must have the same type, else the debug layer will emit an error.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>Where applicable, prefer [**ID3D12Device::CopyDescriptorsSimple**](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple) to this method. It can have a better CPU cache miss rate due to the linear nature of the copy.</remarks>
	void CopyDescriptors( uint NumDestDescriptorRanges,
						  out Span< CPUDescriptorHandle > pDestDescriptorRangeStarts,
						  Span< uint > pDestDescriptorRangeSizes, uint NumSrcDescriptorRanges,
						  in Span< CPUDescriptorHandle > pSrcDescriptorRangeStarts,
						  in Span< uint > pSrcDescriptorRangeSizes,
						  DescriptorHeapType DescriptorHeapsType ) ;
	

	/// <summary>Copies descriptors from a source to a destination. (ID3D12Device.CopyDescriptorsSimple)</summary>
	/// <param name="NumDescriptors">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of descriptors to copy.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DestDescriptorRangeStart">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> A <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> that describes the destination descriptors to start to copy to. The destination and source descriptors must be in heaps of the same [D3D12_DESCRIPTOR_HEAP_TYPE](/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="SrcDescriptorRangeStart">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> A <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> that describes the source descriptors to start to copy from. > [!IMPORTANT] > The *SrcDescriptorRangeStart* parameter must be in a non shader-visible descriptor heap. This is because shader-visible descriptor heaps may be created in **WRITE_COMBINE** memory or GPU local memory, which is prohibitively slow to read from. If your application manages descriptor heaps via copying the descriptors required for a given pass or frame from local "storage" descriptor heaps to the GPU-bound descriptor heap, then use shader-opaque heaps for the storage heaps and copy into the GPU-visible heap as required.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DescriptorHeapsType">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a></b> The <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a>-typed value that specifies the type of descriptor heap to copy with. This is required as different descriptor types may have different sizes. Both the source and destination descriptor heaps must have the same type, else the debug layer will emit an error.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>Where applicable, prefer this method to [**ID3D12Device::CopyDescriptors**](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors). It can have a better CPU cache miss rate due to the linear nature of the copy.</remarks>
	void CopyDescriptorsSimple( uint NumDescriptors, 
						   CPUDescriptorHandle DestDescriptorRangeStart, 
						   CPUDescriptorHandle SrcDescriptorRangeStart, 
						   DescriptorHeapType DescriptorHeapsType ) ;
	
	/// <summary>Gets the size and alignment of memory required for a collection of resources on this adapter.</summary>
	/// <param name="visibleMask">
	/// <para>Type: **[UINT](/windows/win32/WinProg/windows-data-types)** For single-GPU operation, set this to zero. If there are multiple GPU nodes, then set bits to identify the nodes (the device's physical adapters). Each bit in the mask corresponds to a single node. Also see [Multi-adapter systems](/windows/win32/direct3d12/multi-engine).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="numResourceDescs">
	/// <para>Type: **[UINT](/windows/win32/WinProg/windows-data-types)** The number of resource descriptors in the *pResourceDescs* array.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pResourceDescs">
	/// <para>Type: **const [ResourceDescription](./ns-d3d12-d3d12_resource_desc.md)\*** An array of <b>ResourceDescription</b> structures that described the resources to get info about.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[D3D12_RESOURCE_ALLOCATION_INFO](./ns-d3d12-d3d12_resource_allocation_info.md)** A [D3D12_RESOURCE_ALLOCATION_INFO](./ns-d3d12-d3d12_resource_allocation_info.md) structure that provides info about video memory allocated for the specified array of resources. If an error occurs, then **D3D12_RESOURCE_ALLOCATION_INFO::SizeInBytes** equals **UINT64_MAX**.</para>
	/// </returns>
	/// <remarks>
	/// <para>When you're using [CreatePlacedResource](./nf-d3d12-id3d12device-createplacedresource.md), your application must use **GetResourceAllocationInfo** in order to understand the size and alignment characteristics of texture resources. The results of this method vary depending on the particular adapter, and must be treated as unique to this adapter and driver version. Your application can't use the output of **GetResourceAllocationInfo** to understand packed mip properties of textures. To understand packed mip properties of textures, your application must use [GetResourceTiling](./nf-d3d12-id3d12device-getresourcetiling.md). Texture resource sizes significantly differ from the information returned by **GetResourceTiling**, because some adapter architectures allocate extra memory for textures to reduce the effective bandwidth during common rendering scenarios. This even includes textures that have constraints on their texture layouts, or have standardized texture layouts. That extra memory can't be sparsely mapped nor remapped by an application using [CreateReservedResource](./nf-d3d12-id3d12device-createreservedresource.md) and [UpdateTileMappings](./nf-d3d12-id3d12commandqueue-updatetilemappings.md), so it isn't reported by **GetResourceTiling**. Your application can forgo using **GetResourceAllocationInfo** for buffer resources ([D3D12_RESOURCE_DIMENSION_BUFFER](./ne-d3d12-d3d12_resource_dimension.md)). Buffers have the same size on all adapters, which is merely the smallest multiple of 64KB that's greater or equal to [ResourceDescription::Width](./ns-d3d12-d3d12_resource_desc.md). When multiple resource descriptions are passed in, the C++ algorithm for calculating a structure size and alignment are used. For example, a three-element array with two tiny 64KB-aligned resources and a tiny 4MB-aligned resource, reports differing sizes based on the order of the array. If the 4MB aligned resource is in the middle, then the resulting **Size** is 12MB. Otherwise, the resulting **Size** is 8MB. The **Alignment** returned would always be 4MB, because it's the superset of all alignments in the resource array.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	ResourceAllocationInfo GetResourceAllocationInfo( uint visibleMask,
													  uint numResourceDescs,
													  Span< ResourceDescription > pResourceDescs ) ;

	
	/// <summary>Divulges the equivalent custom heap properties that are used for non-custom heap types, based on the adapter's architectural properties.</summary>
	/// <param name="nodeMask">
	/// <para>Type: <b>UINT</b> For single-GPU operation, set this to zero. If there are multiple GPU nodes, set a bit to identify the node (the  device's physical adapter). Each bit in the mask corresponds to a single node. Only 1 bit must be set. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcustomheapproperties#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="heapType">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type">D3D12_HEAP_TYPE</a></b> A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type">D3D12_HEAP_TYPE</a>-typed value that specifies the heap to get properties for. D3D12_HEAP_TYPE_CUSTOM is not supported as a parameter value.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcustomheapproperties#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_properties">D3D12_HEAP_PROPERTIES</a></b> Returns a <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_properties">D3D12_HEAP_PROPERTIES</a> structure that provides properties for the specified heap. The <b>Type</b> member of the returned D3D12_HEAP_PROPERTIES is always D3D12_HEAP_TYPE_CUSTOM.</para>
	/// <para>When <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture">D3D12_FEATURE_DATA_ARCHITECTURE</a>::UMA is FALSE, the returned D3D12_HEAP_PROPERTIES members convert as follows:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcustomheapproperties">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	HeapProperties GetCustomHeapProperties( uint nodeMask, HeapType heapType ) ;
	
	
	/// <summary>Creates both a resource and an implicit heap, such that the heap is big enough to contain the entire resource, and the resource is mapped to the heap.</summary>
	/// <param name="pHeapProperties">
	/// <para>Type: **const [D3D12_HEAP_PROPERTIES](./ns-d3d12-d3d12_heap_properties.md)\*** A pointer to a **D3D12_HEAP_PROPERTIES** structure that provides properties for the resource's heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="HeapFlags">
	/// <para>Type: **[D3D12_HEAP_FLAGS](./ne-d3d12-d3d12_heap_flags.md)** Heap options, as a bitwise-OR'd combination of **D3D12_HEAP_FLAGS** enumeration constants.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: **const [ResourceDescription](./ns-d3d12-d3d12_resource_desc.md)\*** A pointer to a **ResourceDescription** structure that describes the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="InitialResourceState">
	/// <para>Type: **[ResourceStates](./ne-d3d12-d3d12_resource_states.md)** The initial state of the resource, as a bitwise-OR'd combination of **ResourceStates** enumeration constants. When you create a resource together with a [D3D12_HEAP_TYPE_UPLOAD](./ne-d3d12-d3d12_heap_type.md) heap, you must set *InitialResourceState* to [D3D12_RESOURCE_STATE_GENERIC_READ](./ne-d3d12-d3d12_resource_states.md). When you create a resource together with a [D3D12_HEAP_TYPE_READBACK](./ne-d3d12-d3d12_heap_type.md) heap, you must set *InitialResourceState* to [D3D12_RESOURCE_STATE_COPY_DEST](./ne-d3d12-d3d12_resource_states.md).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: **const [ClearValue](./ns-d3d12-d3d12_clear_value.md)\*** Specifies a **ClearValue** structure that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the [D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET](./ne-d3d12-d3d12_resource_flags.md) or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, you should choose the value with which the clear operation will most commonly be called. You can call the clear operation with other values, but those operations won't be as efficient as when the value matches the one passed in to resource creation. When you use [D3D12_RESOURCE_DIMENSION_BUFFER](./ne-d3d12-d3d12_resource_dimension.md), you must set *pOptimizedClearValue* to `nullptr`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riidResource">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the resource interface to return in *ppvResource*. While *riidResource* is most commonly the **GUID** of [ID3D12Resource](./nn-d3d12-id3d12resource.md), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created resource object. *ppvResource* can be `nullptr`, to enable capability testing. When *ppvResource* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the resource.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method creates both a resource and a heap, such that the heap is big enough to contain the entire resource, and the resource is mapped to the heap. The created heap is known as an implicit heap, because the heap object can't be obtained by the application. Before releasing the final reference on the resource, your application must ensure that the GPU will no longer read nor write to this resource. The implicit heap is made resident for GPU access before the method returns control to your application. Also see [Residency](/windows/win32/direct3d12/residency). The resource GPU VA mapping can't be changed. See [ID3D12CommandQueue::UpdateTileMappings](./nf-d3d12-id3d12commandqueue-updatetilemappings.md) and [Volume tiled resources](/windows/win32/direct3d12/volume-tiled-resources). This method may be called by multiple threads concurrently.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateCommittedResource( in HeapProperties pHeapProperties,
									  HeapFlags HeapFlags, in ResourceDescription pDesc,
										  ResourceStates InitialResourceState,
											  [Optional] in ClearValue pOptimizedClearValue,
													in Guid riidResource, out IResource ppvResource ) ;
	
	
	/// <summary>Creates a heap that can be used with placed resources and reserved resources.</summary>
	/// <param name="pDesc">
	/// <para>Type: **const [D3D12_HEAP_DESC](./ns-d3d12-d3d12_heap_desc.md)\*** A pointer to a constant **D3D12_HEAP_DESC** structure that describes the heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createheap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the heap interface to return in *ppvHeap*. While *riidResource* is most commonly the **GUID** of [IHeap](./nn-d3d12-id3d12heap.md), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createheap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvHeap">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created heap object. *ppvHeap* can be `nullptr`, to enable capability testing. When *ppvHeap* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createheap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the heap.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>**CreateHeap** creates a heap that can be used with placed resources and reserved resources. Before releasing the final reference on the heap, your application must ensure that the GPU will no longer read or write to this heap. A placed resource object holds a reference on the heap it is created on; but a reserved resource doesn't hold a reference for each mapping made to a heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createheap#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateHeap( in HeapDescription pDesc, in Guid riid, out IHeap ppvHeap ) ;
	
	
	/// <summary>Creates a resource that is placed in a specific heap. Placed resources are the lightest weight resource objects available, and are the fastest to create and destroy.</summary>
	/// <param name="pHeap">
	/// <para>Type: [in] **<a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12heap">IHeap</a>*** A pointer to the **IHeap** interface that represents the heap in which the resource is placed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="HeapOffset">
	/// <para>Type: **<a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a>** The offset, in bytes, to the resource. The *HeapOffset* must be a multiple of the resource's alignment, and *HeapOffset* plus the resource size must be smaller than or equal to the heap size. <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo">**GetResourceAllocationInfo**</a> must be used to understand the sizes of texture resources.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: [in] **const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_desc">ResourceDescription</a>*** A pointer to a **ResourceDescription** structure that describes the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="InitialState">
	/// <para>Type: **<a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states">ResourceStates</a>** The initial state of the resource, as a bitwise-OR'd combination of **ResourceStates** enumeration constants. When a resource is created together with a **D3D12_HEAP_TYPE_UPLOAD** heap, *InitialState* must be **D3D12_RESOURCE_STATE_GENERIC_READ**. When a resource is created together with a **D3D12_HEAP_TYPE_READBACK** heap, *InitialState* must be **D3D12_RESOURCE_STATE_COPY_DEST**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: [in, optional] **const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_clear_value">ClearValue</a>*** Specifies a **ClearValue** that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the **D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET** or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, your application should choose the value that the clear operation will most commonly be called with. Clear operations can be called with other values, but those operations will not be as efficient as when the value matches the one passed into resource creation. *pOptimizedClearValue* must be NULL when used with **D3D12_RESOURCE_DIMENSION_BUFFER**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** The globally unique identifier (**GUID**) for the resource interface. This is an input parameter. The **REFIID**, or **GUID**, of the interface to the resource can be obtained by using the `__uuidof` macro. For example, `__uuidof(ID3D12Resource)` gets the **GUID** of the interface to a resource. Although **riid** is, most commonly, the GUID for <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">**ID3D12Resource**</a>, it may be any **GUID** for any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: [out, optional] **void**** A pointer to a memory block that receives a pointer to the resource. *ppvResource* can be NULL, to enable capability testing. When *ppvResource* is NULL, no object will be created and S_FALSE will be returned when *pResourceDesc* and other parameters are valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **<a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a>** This method returns **E_OUTOFMEMORY** if there is insufficient memory to create the resource. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>**CreatePlacedResource** is similar to fully mapping a reserved resource to an offset within a heap; but the virtual address space associated with a heap may be reused as well. Placed resources are lighter weight to create and destroy than committed resources are. This is because no heap is created nor destroyed during those operations. In addition, placed resources enable an even lighter weight technique to reuse memory than resource creation and destruction&mdash;that is, reuse through aliasing, and aliasing barriers. Multiple placed resources may simultaneously overlap each other on the same heap, but only a single overlapping resource can be used at a time. There are two placed resource usage semantics&mdash;a simple model, and an advanced model. We recommend that you choose the simple model (it maximizes graphics tool support across the diverse ecosystem of GPUs), unless and until you find that you need the advanced model for your app.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreatePlacedResource( IHeap pHeap, ulong HeapOffset, 
							   in ResourceDescription pDesc,
							   ResourceStates InitialState,
							   [Optional] in ClearValue pOptimizedClearValue,
							   in Guid riid, out IResource ppvResource ) ;
	
	/// <summary>Creates a resource that is reserved, and not yet mapped to any pages in a heap.</summary>
	/// <param name="pDesc">
	/// <para>Type: **const [ResourceDescription](./ns-d3d12-d3d12_resource_desc.md)\*** A pointer to a **ResourceDescription** structure that describes the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="InitialState">
	/// <para>Type: **[ResourceStates](./ne-d3d12-d3d12_resource_states.md)** The initial state of the resource, as a bitwise-OR'd combination of **ResourceStates** enumeration constants.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: **const [ClearValue](./ns-d3d12-d3d12_clear_value.md)\*** Specifies a **ClearValue** structure that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the [D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET](./ne-d3d12-d3d12_resource_flags.md) or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, you should choose the value with which the clear operation will most commonly be called. You can call the clear operation with other values, but those operations won't be as efficient as when the value matches the one passed in to resource creation. When you use [D3D12_RESOURCE_DIMENSION_BUFFER](./ne-d3d12-d3d12_resource_dimension.md), you must set *pOptimizedClearValue* to `nullptr`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the resource interface to return in *ppvResource*. See **Remarks**. While *riidResource* is most commonly the **GUID** of [ID3D12Resource](./nn-d3d12-id3d12resource.md), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created resource object. *ppvResource* can be `nullptr`, to enable capability testing. When *ppvResource* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/win32/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the resource.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>**CreateReservedResource** is equivalent to [D3D11_RESOURCE_MISC_TILED](../d3d11/ne-d3d11-d3d11_resource_misc_flag.md) in Direct3D 11. It creates a resource with virtual memory only, no backing store. You need to map the resource to physical memory (that is, to a heap) using <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-copytilemappings">CopyTileMappings</a> and <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-updatetilemappings">UpdateTileMappings</a>. These resource types can only be created when the adapter supports tiled resource tier 1 or greater. The tiled resource tier defines the behavior of accessing a resource that is not mapped to a heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateReservedResource( in ResourceDescription pDesc, 
								 ResourceStates InitialState, 
								 [Optional] in ClearValue pOptimizedClearValue, 
								 in Guid riid, out IResource ppvResource ) ;
	

	/// <summary>Creates a shared handle to a heap, resource, or fence object.</summary>
	/// <param name="pObject">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12devicechild">ID3D12DeviceChild</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12devicechild">ID3D12DeviceChild</a> interface that represents the heap, resource, or fence object to create for sharing. The following interfaces (derived from <b>ID3D12DeviceChild</b>) are supported:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pAttributes">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/aa379560(v=vs.85)">SECURITY_ATTRIBUTES</a>*</b> A pointer to a <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/aa379560(v=vs.85)">SECURITY_ATTRIBUTES</a> structure that contains two separate but related data members: an optional security descriptor, and a <b>Boolean</b> value that determines whether child processes can inherit the returned handle.</para>
	/// <para>Set this parameter to <b>NULL</b> if you want child processes that the application might create to not  inherit  the handle returned by <b>CreateSharedHandle</b>, and if you want the resource that is associated with the returned handle to get a default security descriptor.</para>
	/// <para>The <b>lpSecurityDescriptor</b> member of the structure specifies a <a href="https://docs.microsoft.com/windows/desktop/api/winnt/ns-winnt-security_descriptor">SECURITY_DESCRIPTOR</a> for the resource. Set this member to <b>NULL</b> if you want the runtime to assign a default security descriptor to the resource that is associated with the returned handle. The ACLs in the default security descriptor for the resource come from the primary or impersonation token of the creator. For more info, see <a href="https://docs.microsoft.com/windows/desktop/Sync/synchronization-object-security-and-access-rights">Synchronization Object Security and Access Rights</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Access">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">DWORD</a></b> Currently the only value this parameter accepts is GENERIC_ALL.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Name">
	/// <para>Type: <b>LPCWSTR</b> A <b>NULL</b>-terminated <b>UNICODE</b> string that contains the name to associate with the shared heap. The name is limited to MAX_PATH characters. Name comparison is case-sensitive.</para>
	/// <para>If <i>Name</i> matches the name of an existing resource, <b>CreateSharedHandle</b> fails with <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_NAME_ALREADY_EXISTS</a>. This occurs because these objects share the same namespace.</para>
	/// <para>The name can have a "Global\" or "Local\" prefix to explicitly create the object in the global or session namespace. The remainder of the name can contain any character except the backslash character (\\). For more information, see <a href="https://docs.microsoft.com/windows/desktop/TermServ/kernel-object-namespaces">Kernel Object Namespaces</a>. Fast user switching is implemented using Terminal Services sessions. Kernel object names must follow the guidelines outlined for Terminal Services so that applications can support multiple users.</para>
	/// <para>The object can be created in a private namespace. For more information, see <a href="https://docs.microsoft.com/windows/desktop/Sync/object-namespaces">Object Namespaces</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pHandle">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HANDLE</a>*</b> A pointer to a variable that receives the NT HANDLE value to the resource to share. You can use this handle in calls to access the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns one of the following values:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>Both heaps and committed resources can be shared. Sharing a committed resource shares the implicit heap along with the committed resource description, such that a compatible resource description can be mapped to the heap from another device. For Direct3D 11 and Direct3D 12 interop scenarios, a shared fence is opened in DirectX 11 with the <a href="https://docs.microsoft.com/windows/win32/api/d3d11_4/nf-d3d11_4-id3d11device5-opensharedfence">ID3D11Device5::OpenSharedFence</a> method, and a shared resource is opened with the <a href="https://docs.microsoft.com/windows/win32/api/d3d11_1/nf-d3d11_1-id3d11device1-opensharedresource1">ID3D11Device::OpenSharedResource1</a> method. For Direct3D 12, a shared handle is opened with the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandle">ID3D12Device::OpenSharedHandle</a> or the ID3D12Device::OpenSharedHandleByName method.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateSharedHandle( IDeviceChild pObject,
							 [Optional] in SecurityAttributes pAttributes, 
								uint Access, PCWSTR Name, out Win32Handle pHandle ) ;
	
	
	/// <summary>Opens a handle for shared resources, shared heaps, and shared fences, by using HANDLE and REFIID.</summary>
	/// <param name="NTHandle">
	/// <para>Type: <b>HANDLE</b> The handle that was output by the call to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle">ID3D12Device::CreateSharedHandle</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for one of the following interfaces:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvObj">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to one of the following interfaces:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandle">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void OpenSharedHandle( Win32Handle NTHandle, in Guid riid, out object ppvObj ) ;

	/// <summary>Opens a handle for shared resources, shared heaps, and shared fences, by using Name and Access.</summary>
	/// <param name="Name">
	/// <para>Type: <b>LPCWSTR</b> The name that was optionally passed as the <i>Name</i> parameter in the call to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createsharedhandle">ID3D12Device::CreateSharedHandle</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandlebyname#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Access">
	/// <para>Type: <b>DWORD</b> The access level that was specified in the <i>Access</i> parameter in the call to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createsharedhandle">ID3D12Device::CreateSharedHandle</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandlebyname#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pNTHandle">
	/// <para>Type: <b>HANDLE*</b> Pointer to the shared handle.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandlebyname#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandlebyname">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void OpenSharedHandleByName( PCWSTR Name, uint Access, ref HANDLE pNTHandle ) ;
	
	/// <summary>Makes objects resident for the device.</summary>
	/// <param name="NumObjects">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of objects  in the <i>ppObjects</i> array to make resident for the device.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-makeresident#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppObjects">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>*</b> A pointer to a memory block that contains an array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a> interface pointers for the objects.</para>
	/// <para>Even though most D3D12 objects inherit from <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>, residency changes are only supported on the following objects: Descriptor Heaps, Heaps, Committed Resources, and Query Heaps</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-makeresident#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>MakeResident</b> loads the data associated with a resource from disk, and re-allocates the memory from the resource's appropriate memory pool. This method should be called on the object which owns the physical memory.</para>
	/// <para>Use this method, and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a>, to manage GPU video memory, noting that this was done automatically in D3D11, but now has to be done by the app in D3D12. <b>MakeResident</b> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a> can help applications manage the residency budget on many adapters. <b>MakeResident</b> explicitly pages-in data and, then, precludes page-out so the GPU can access the data. <b>Evict</b> enables page-out. Some GPU architectures do not benefit from residency manipulation, due to the lack of sufficient GPU virtual address space. Use <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_gpu_virtual_address_support">D3D12_FEATURE_DATA_GPU_VIRTUAL_ADDRESS_SUPPORT</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-queryvideomemoryinfo">IDXGIAdapter3::QueryVideoMemoryInfo</a> to recognize when the maximum GPU VA space per-process is too small or roughly the same size as the residency budget. For such architectures, the residency budget will always be constrained by the amount of GPU virtual address space. <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a> will not free-up any residency budget on such systems.</para>
	/// <para>Applications must handle <b>MakeResident</b> failures, even if there appears to be enough residency budget available. Physical memory fragmentation and adapter architecture quirks can preclude the utilization of large contiguous ranges. Applications should free up more residency budget before trying again.</para>
	/// <para><b>MakeResident</b> is ref-counted, such that <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a> must be called the same amount of times as <b>MakeResident</b> before <b>Evict</b> takes effect. Objects that support residency are made resident during creation, so a single <b>Evict</b> call will actually evict the object. Applications must use fences to ensure the GPU doesn't use non-resident objects. <b>MakeResident</b> must return before the GPU executes a command list that references the object. <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a> must be called after the GPU finishes executing a command list that references the object. Evicted objects still consume the same GPU virtual address and same amount of GPU virtual address space. Therefore, resource descriptors and other GPU virtual address references are not invalidated after <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-makeresident#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void MakeResident< P >( uint NumObjects, Span< P > ppObjects ) where P : IPageable ;
	
	/// <summary>Enables the page-out of data, which precludes GPU access of that data.</summary>
	/// <param name="NumObjects">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of objects in the <i>ppObjects</i> array to evict from the device.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-evict#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppObjects">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>*</b> A pointer to a memory block that contains an array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a> interface pointers for the objects.</para>
	/// <para>Even though most D3D12 objects inherit from <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>, residency changes are only supported on the following objects: Descriptor Heaps, Heaps, Committed Resources, and Query Heaps</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-evict#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>Evict</b> persists the data associated with a resource to disk, and then removes the resource from the memory pool where it was located. This method should be called on the object which owns the physical memory: either a committed resource (which owns both virtual  and physical memory assignments) or a heap - noting that reserved resources do not have physical memory, and placed resources are borrowing memory from a heap.</para>
	/// <para>Refer to the remarks for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-makeresident">MakeResident</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-evict#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void Evict< P >( uint NumObjects, Span< P > ppObjects ) where P : IPageable ;
	
	/// <summary>Creates a fence object. (ID3D12Device.CreateFence)</summary>
	/// <param name="InitialValue">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT64</a></b> The initial value for the fence.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createfence#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Flags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_fence_flags">D3D12_FENCE_FLAGS</a></b> A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_fence_flags">D3D12_FENCE_FLAGS</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies options for the fence.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createfence#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the fence interface (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the fence can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12Fence) will get the <b>GUID</b> of the interface to a fence.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createfence#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppFence">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a> interface that is used to access the fence.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createfence#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HRESULT</a></b> Returns <b>S_OK</b> if successful; otherwise, returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createfence">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateFence( ulong InitialValue, FenceFlags Flags, 
							in Guid riid, out IFence ppFence ) ;

	/// <summary>Gets the reason that the device was removed.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns the reason that the device was removed.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getdeviceremovedreason">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetDeviceRemovedReason( ) ;

	/// <summary>Gets a resource layout that can be copied. Helps the app fill-in D3D12_PLACED_SUBRESOURCE_FOOTPRINT and D3D12_SUBRESOURCE_FOOTPRINT when suballocating space in upload heaps.</summary>
	/// <param name="pResourceDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_resource_desc">ResourceDescription</a>*</b> A description of the resource, as a pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_resource_desc">ResourceDescription</a> structure.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="FirstSubresource">
	/// <para>Type: <b>UINT</b> Index of the first subresource in the resource. The range of valid values is 0 to D3D12_REQ_SUBRESOURCES.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="NumSubresources">
	/// <para>Type: <b>UINT</b> The number of subresources in the resource.  The range of valid values is 0 to (D3D12_REQ_SUBRESOURCES - <i>FirstSubresource</i>).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="BaseOffset">
	/// <para>Type: <b>UINT64</b> The offset, in bytes, to the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pLayouts">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint">D3D12_PLACED_SUBRESOURCE_FOOTPRINT</a>*</b> A pointer to an array (of length <i>NumSubresources</i>) of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint">D3D12_PLACED_SUBRESOURCE_FOOTPRINT</a> structures, to be filled with the description and placement of each subresource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pNumRows">
	/// <para>Type: <b>UINT*</b> A pointer to an array (of length <i>NumSubresources</i>) of integer  variables, to be filled with the number of rows for each subresource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pRowSizeInBytes">
	/// <para>Type: <b>UINT64*</b> A pointer to an array (of length <i>NumSubresources</i>) of integer variables, each entry to be filled with the unpadded size in bytes of a row, of each subresource.</para>
	/// <para>For example, if a Texture2D resource has a width of 32 and bytes per pixel of 4, then <i>pRowSizeInBytes</i> returns 128. <i>pRowSizeInBytes</i> should not be confused with <b>row pitch</b>, as examining <i>pLayouts</i> and getting the row pitch from that will give you 256 as it is aligned to D3D12_TEXTURE_DATA_PITCH_ALIGNMENT.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pTotalBytes">
	/// <para>Type: <b>UINT64*</b> A pointer to an integer variable, to be filled with the total size, in bytes.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para>This routine assists the application in filling out <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint">D3D12_PLACED_SUBRESOURCE_FOOTPRINT</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_subresource_footprint">D3D12_SUBRESOURCE_FOOTPRINT</a> structures, when suballocating space in upload heaps. The resulting structures are GPU adapter-agnostic, meaning that the values will not vary from one GPU adapter to the next. <b>GetCopyableFootprints</b> uses specified details about resource formats, texture layouts, and alignment requirements (from the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_resource_desc">ResourceDescription</a> structure)  to fill out the subresource structures. Applications have access to all these details, so this method, or a variation of it, could be  written as part of the app.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetCopyableFootprints( in ResourceDescription pResourceDesc, uint FirstSubresource, 
								uint NumSubresources, ulong BaseOffset,
								[Optional] in PlacedSubresourceFootprint? pLayouts,
												[Out] Span< uint > pNumRows, 
													[Out] Span< ulong > pRowSizeInBytes, 
														out ulong pTotalBytes ) ;
	
	
	/// <summary>Creates a query heap. A query heap contains an array of queries.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_query_heap_desc">D3D12_QUERY_HEAP_DESC</a>*</b> Specifies the query heap in a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_query_heap_desc">D3D12_QUERY_HEAP_DESC</a> structure.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createqueryheap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> Specifies a REFIID that uniquely identifies the heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createqueryheap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvHeap">
	/// <para>Type: <b>void**</b> Specifies a pointer to the heap, that will be returned on successful completion of the method. <i>ppvHeap</i> can be NULL, to enable capability testing. When <i>ppvHeap</i> is NULL, no object will be created and S_FALSE will be returned when <i>pDesc</i> is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createqueryheap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/queries">Queries</a> for more information.</remarks>
	void CreateQueryHeap( in QueryHeapDescription pDesc, in Guid riid, out IHeap ppvHeap ) ;
	
	/// <summary>A development-time aid for certain types of profiling and experimental prototyping.</summary>
	/// <param name="Enable">
	/// <para>Type: <b>BOOL</b> Specifies a BOOL that turns the stable power state on or off.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-setstablepowerstate#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method is only useful during the development of applications. It enables developers to profile GPU usage of multiple algorithms without experiencing artifacts from <a href="https://en.wikipedia.org/wiki/Dynamic_frequency_scaling">dynamic frequency scaling</a>. Do not call this method in normal execution for a shipped application. This method only works while the machine is in <a href="https://docs.microsoft.com/windows/uwp/get-started/enable-your-device-for-development">developer mode</a>. If developer mode is not enabled, then device removal will occur. Instead, call this method in response to an off-by-default, developer-facing switch. Calling it in response to command line parameters, config files, registry keys, and developer console commands are reasonable usage scenarios. A stable power state typically fixes GPU clock rates at a slower setting that is significantly lower than that experienced by users under normal application load. This reduction in clock rate affects the entire system. Slow clock rates are required to ensure processors don’t exhaust power, current, and thermal limits. Normal usage scenarios commonly leverage a processors ability to dynamically over-clock. Any conclusions made by comparing two designs under a stable power state should be double-checked with supporting results from real usage scenarios.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-setstablepowerstate#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetStablePowerState( bool Enable ) ;

	/// <summary>This method creates a command signature.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_command_signature_desc">D3D12_COMMAND_SIGNATURE_DESC</a>*</b> Describes the command signature to be created with the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_command_signature_desc">D3D12_COMMAND_SIGNATURE_DESC</a> structure.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandsignature#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pRootSignature">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a>*</b> Specifies the  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a> that the command signature applies to.</para>
	/// <para>The root signature is required if any of the commands in the signature will update bindings on the pipeline. If the only command present is a draw or dispatch, the root signature parameter can be set to NULL.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandsignature#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the command signature interface (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandsignature">ID3D12CommandSignature</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the command signature can be obtained by using the __uuidof() macro. For example, __uuidof(<b>ID3D12CommandSignature</b>) will get the <b>GUID</b> of the interface to a command signature.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandsignature#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvCommandSignature">
	/// <para>Type: <b>void**</b> Specifies a pointer, that on successful completion of the method will point to the created command signature (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandsignature">ID3D12CommandSignature</a>).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandsignature#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandsignature">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateCommandSignature( in CommandSignatureDescription pDesc,
								 IRootSignature pRootSignature, in Guid riid,
									out ICommandSignature ppvCommandSignature ) ;

	
	/// <summary>Gets info about how a tiled resource is broken into tiles. (ID3D12Device.GetResourceTiling)</summary>
	/// <param name="pTiledResource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> Specifies a tiled <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>  to get info about.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pNumTilesForEntireResource">
	/// <para>Type: <b>UINT*</b> A pointer to a variable that receives the number of tiles needed to store the entire tiled resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pPackedMipDesc">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_packed_mip_info">D3D12_PACKED_MIP_INFO</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_packed_mip_info">D3D12_PACKED_MIP_INFO</a> structure that <b>GetResourceTiling</b> fills with info about how the tiled resource's mipmaps are packed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pStandardTileShapeForNonPackedMips">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tile_shape">D3D12_TILE_SHAPE</a>*</b> Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tile_shape">D3D12_TILE_SHAPE</a> structure that <b>GetResourceTiling</b> fills with info about the tile shape. This is info about how pixels fit in the tiles, independent of tiled resource's dimensions, not including packed mipmaps. If the entire tiled resource is packed, this parameter is meaningless because the tiled resource has no defined layout for packed mipmaps. In this situation, <b>GetResourceTiling</b> sets the members of D3D12_TILE_SHAPE to zeros.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pNumSubresourceTilings">
	/// <para>Type: <b>UINT*</b> A pointer to a variable that contains the number of tiles in the subresource. On input, this is the number of subresources to query tilings for; on output, this is the number that was actually retrieved at <i>pSubresourceTilingsForNonPackedMips</i> (clamped to what's available).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="FirstSubresourceTilingToGet">
	/// <para>Type: <b>UINT</b> The number of the first subresource tile to get. <b>GetResourceTiling</b> ignores this parameter if the number that <i>pNumSubresourceTilings</i> points to is 0.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pSubresourceTilingsForNonPackedMips">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_subresource_tiling">D3D12_SUBRESOURCE_TILING</a>*</b> Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_subresource_tiling">D3D12_SUBRESOURCE_TILING</a> structure that <b>GetResourceTiling</b> fills with info about subresource tiles. If subresource tiles are part of packed mipmaps, <b>GetResourceTiling</b> sets the members of D3D12_SUBRESOURCE_TILING to zeros, except the <i>StartTileIndexInOverallResource</i> member, which <b>GetResourceTiling</b> sets to D3D12_PACKED_TILE (0xffffffff). The D3D12_PACKED_TILE constant indicates that the whole <b>D3D12_SUBRESOURCE_TILING</b> structure is meaningless for this situation, and the info that the <i>pPackedMipDesc</i> parameter points to applies.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para>To estimate the total resource size of textures needed when calculating heap sizes and calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createplacedresource">CreatePlacedResource</a>, use <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo">GetResourceAllocationInfo</a> instead of <b>GetResourceTiling</b>. <b>GetResourceTiling</b> cannot be used for this.</para>
	/// <para>For more information on tiled resources, refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/volume-tiled-resources">Volume Tiled Resources</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetResourceTiling( ID3D12Resource pTiledResource,
							out uint pNumTilesForEntireResource,
							out D3D12_PACKED_MIP_INFO pPackedMipDesc,
							out TileShape pStandardTileShapeForNonPackedMips,
							ref uint pNumSubresourceTilings, uint FirstSubresourceTilingToGet,
							out SubresourceTiling pSubresourceTilingsForNonPackedMips ) ;

	/// <summary>Gets a locally unique identifier for the current device (adapter).</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">LUID</a></b> The locally unique identifier for the adapter.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method returns a unique identifier for the adapter that is specific to the adapter hardware. Applications can use this identifier to define robust mappings across various APIs (Direct3D 12, DXGI).</para>
	/// <para>A locally unique identifier (LUID) is a 64-bit value that is guaranteed to be unique only on the system on which it was generated. The uniqueness of a locally unique identifier (LUID) is guaranteed only until the system is restarted.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getadapterluid#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	Luid GetAdapterLuid( ) ;
} ;

// ========================================================================================
// ---------- MISSING TYPES ----------
// ========================================================================================


// --- ENUM TYPES ----------------------------





// --- STRUCT TYPES -------------------------



// --- INTERFACE TYPES ----------------------

