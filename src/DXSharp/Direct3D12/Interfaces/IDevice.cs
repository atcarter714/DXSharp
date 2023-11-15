#region Using Directives

using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;
using Windows.Win32 ;

using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>
/// Represents a virtual adapter; it is used to create command allocators,
/// command lists, command queues, fences, resources, pipeline state objects,
/// heaps, root signatures, samplers, and many resource views.
/// </summary>
[ProxyFor( typeof(ID3D12Device) )]
public interface IDevice: IObject,
						  IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions for IDevice thru IDevice12:
	[SuppressMessage( "Interoperability", "CA1416:Validate platform compatibility" )]
	internal static readonly ReadOnlyDictionary< Guid, Func< ID3D12Device, IInstantiable > >
		_deviceCreationFunctions = new( new Dictionary< Guid, Func< ID3D12Device, IInstantiable > > {
				{ IDevice.IID, ( pComObj ) => new Device( pComObj ) },
				{ IDevice1.IID, ( pComObj ) => new Device1( ( pComObj as ID3D12Device1 )! ) },
				{ IDevice2.IID, ( pComObj ) => new Device2( ( pComObj as ID3D12Device2 )! ) },
				{ IDevice3.IID, ( pComObj ) => new Device3( ( pComObj as ID3D12Device3 )! ) },
				{ IDevice4.IID, ( pComObj ) => new Device4( ( pComObj as ID3D12Device4 )! ) },
				{ IDevice5.IID, ( pComObj ) => new Device5( ( pComObj as ID3D12Device5 )! ) },
				{ IDevice6.IID, ( pComObj ) => new Device6( ( pComObj as ID3D12Device6 )! ) },
				{ IDevice7.IID, ( pComObj ) => new Device7( ( pComObj as ID3D12Device7 )! ) },
				{ IDevice8.IID, ( pComObj ) => new Device8( ( pComObj as ID3D12Device8 )! ) },
				{ IDevice9.IID, ( pComObj ) => new Device9( ( pComObj as ID3D12Device9 )! ) },
				{ IDevice10.IID, ( pComObj ) => new Device10( ( pComObj as ID3D12Device10 )! ) },
				{ IDevice11.IID, ( pComObj ) => new Device11( ( pComObj as ID3D12Device11 )! ) },
				{ IDevice12.IID, ( pComObj ) => new Device12( ( pComObj as ID3D12Device12 )! ) }
			} ) ;
	
	// ---------------------------------------------------------------------------------
	
	/// <summary>Reports the number of physical adapters (nodes) that are associated with this device.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of physical adapters (nodes) that this device has.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getnodecount">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	uint GetNodeCount( ) ;

	/// <summary>Creates a command queue.</summary>
	/// <param name="pDesc">
	/// <para>Type: [in] <b>const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_command_queue_desc">D3D12_COMMAND_QUEUE_DESC</a>*</b> Specifies a **D3D12_COMMAND_QUEUE_DESC** that describes the command queue.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandqueue#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b><b>REFIID</b></b> The globally unique identifier (GUID) for the command queue interface. See **Remarks**. An input parameter.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandqueue#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppCommandQueue">
	/// <para>Type: [out] <b><b>void</b>**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a> interface for the command queue.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandqueue#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the command queue. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 return codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>The <b>REFIID</b>, or <b>GUID</b>, of the interface to the command queue can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12CommandQueue) will get the <b>GUID</b> of the interface to a command queue.</remarks>
	/// void CreateCommandQueue( D3D12_COMMAND_QUEUE_DESC* pDesc, Guid* riid, out object ppCommandQueue ) ;
	void CreateCommandQueue( in  CommandQueueDescription pDesc,
							 in  Guid                    riid, 
							 out ICommandQueue?          ppCommandQueue ) ;


	/// <summary>Creates a command allocator object.</summary>
	/// <param name="type">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_command_list_type">D3D12_COMMAND_LIST_TYPE</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_command_list_type">D3D12_COMMAND_LIST_TYPE</a>-typed value that specifies the type of command allocator to create. The type of command allocator can be the type that records either direct command lists or bundles.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandallocator#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the command allocator interface (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandallocator">ID3D12CommandAllocator</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the command allocator can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12CommandAllocator) will get the <b>GUID</b> of the interface to a command allocator.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandallocator#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppCommandAllocator">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandallocator">ID3D12CommandAllocator</a> interface for the command allocator.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandallocator#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the command allocator. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>The device creates command lists from the command allocator.</remarks>
	void CreateCommandAllocator( CommandListType        type, 
								 in  Guid               riid,
								 out ICommandAllocator? ppCommandAllocator ) ;


	/// <summary>Creates a graphics pipeline state object.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_graphics_pipeline_state_desc">D3D12_GRAPHICS_PIPELINE_STATE_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_graphics_pipeline_state_desc">D3D12_GRAPHICS_PIPELINE_STATE_DESC</a> structure that describes graphics pipeline state.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-creategraphicspipelinestate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the pipeline state interface (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the pipeline state can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12PipelineState) will get the <b>GUID</b> of the interface to a pipeline state.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-creategraphicspipelinestate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppPipelineState">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> interface for the pipeline state object. The pipeline state object is an immutable state object.  It contains no methods.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-creategraphicspipelinestate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the pipeline state object. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-creategraphicspipelinestate">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateGraphicsPipelineState( in  GraphicsPipelineStateDescription pDesc, 
									  in  Guid                             riid,
									  out IPipelineState?                  ppPipelineState ) ;


	/// <summary>Creates a compute pipeline state object.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_compute_pipeline_state_desc">D3D12_COMPUTE_PIPELINE_STATE_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_compute_pipeline_state_desc">D3D12_COMPUTE_PIPELINE_STATE_DESC</a> structure that describes compute pipeline state.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcomputepipelinestate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the pipeline state interface (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the pipeline state can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12PipelineState) will get the <b>GUID</b> of the interface to a pipeline state.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcomputepipelinestate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppPipelineState">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pipelinestate">ID3D12PipelineState</a> interface for the pipeline state object. The pipeline state object is an immutable state object.  It contains no methods.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcomputepipelinestate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the pipeline state object. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcomputepipelinestate">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateComputePipelineState( in ComputePipelineStateDescription pDesc,
									 in Guid riid, 
									 out IPipelineState ppPipelineState ) ;

	/// <summary>Creates a command list.</summary>
	/// <param name="nodeMask">
	///     <para>Type: **[UINT](/windows/win32/WinProg/windows-data-types)** For single-GPU operation, set this to zero. If there are multiple GPU nodes, then set a bit to identify the node (the device's physical adapter) for which to create the command list. Each bit in the mask corresponds to a single node. Only one bit must be set. Also see [Multi-adapter systems](/windows/win32/direct3d12/multi-engine).</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="type">
	///     <para>Type: **[D3D12_COMMAND_LIST_TYPE](./ne-d3d12-d3d12_command_list_type.md)** Specifies the type of command list to create.</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pCommandAllocator">
	///     <para>Type: **[ID3D12CommandAllocator](./nn-d3d12-id3d12commandallocator.md)\*** A pointer to the command allocator object from which the device creates command lists.</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pInitialState">
	///     <para>Type: **[ID3D12PipelineState](./nn-d3d12-id3d12pipelinestate.md)\*** An optional pointer to the pipeline state object that contains the initial pipeline state for the command list. If it is `nullptr`, then the runtime sets a dummy initial pipeline state, so that drivers don't have to deal with undefined state. The overhead for this is low, particularly for a command list, for which the overall cost of recording the command list likely dwarfs the cost of a single initial state setting. So there's little cost in not setting the initial pipeline state parameter, if doing so is inconvenient. For bundles, on the other hand, it might make more sense to try to set the initial state parameter (since bundles are likely smaller overall, and can be reused frequently).</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	///     <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the command list interface to return in *ppCommandList*.</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppCommandList">
	///     <para>Type: **void\*\*** A pointer to a memory block that receives a pointer to the [ID3D12CommandList](./nn-d3d12-id3d12commandlist.md) or [ID3D12GraphicsCommandList](./nn-d3d12-id3d12graphicscommandlist.md) interface for the command list.</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandlist#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the command list.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>The device creates command lists from the command allocator.</remarks>
	void CreateCommandList(uint nodeMask,
		CommandListType type,
		ICommandAllocator pCommandAllocator,
		[Optional] IPipelineState? pInitialState,
		in Guid riid,
		out ICommandList ppCommandList) ;
	
	
	/// <summary>
	/// Gets information about the features that are supported by the current graphics driver. (ID3D12Device.CheckFeatureSupport)
	/// </summary>
	/// <param name="Feature">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a></b> A constant from the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a> enumeration describing the feature(s) that you want to query for support.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pFeatureSupportData">
	/// <para>Type: <b>void*</b> A pointer to a data structure that corresponds to the value of the <i>Feature</i> parameter. To determine the corresponding data structure for each constant, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="FeatureSupportDataSize">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The size of the structure pointed to by the <i>pFeatureSupportData</i> parameter.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns <b>S_OK</b> if successful. Returns <b>E_INVALIDARG</b> if an unsupported data type is passed to the <i>pFeatureSupportData</i> parameter or if a size mismatch is detected for the <i>FeatureSupportDataSize</i> parameter.</para>
	/// </returns>
	/// <remarks>
	/// <para>As a usage example, to check for ray tracing support, specify the <a href="../d3d12/ns-d3d12-d3d12_feature_data_d3d12_options5.md">D3D12_FEATURE_DATA_D3D12_OPTIONS5</a> structure in the <i>pFeatureSupportData</i> parameter. When the function completes successfully, access the <i>RaytracingTier</i> field (which specifies the supported ray tracing tier) of the now-populated <b>D3D12_FEATURE_DATA_D3D12_OPTIONS5</b> structure. For more info, see <a href="https://docs.microsoft.com/windows/desktop/direct3d12/capability-querying">Capability Querying</a>. <h3><a id="Hardware_support_for_DXGI_Formats"></a><a id="hardware_support_for_dxgi_formats"></a><a id="HARDWARE_SUPPORT_FOR_DXGI_FORMATS"></a>Hardware support for DXGI Formats</h3> To view tables of DXGI formats and hardware features, refer to: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CheckFeatureSupport( D3D12Feature Feature, nint pFeatureSupportData, uint FeatureSupportDataSize ) ;


	/// <summary>Creates a descriptor heap object.</summary>
	/// <param name="pDescriptorHeapDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_heap_desc">D3D12_DESCRIPTOR_HEAP_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_heap_desc">D3D12_DESCRIPTOR_HEAP_DESC</a> structure that describes the heap.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdescriptorheap#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b><b>REFIID</b></b> The globally unique identifier (<b>GUID</b>) for the descriptor heap interface. See Remarks. An input parameter.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdescriptorheap#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvHeap">
	/// <para>Type: <b><b>void</b>**</b> A pointer to a memory block that receives a pointer to the descriptor heap. <i>ppvHeap</i> can be NULL, to enable capability testing. When <i>ppvHeap</i> is NULL, no object will be created and S_FALSE will be returned when <i>pDescriptorHeapDesc</i> is valid.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdescriptorheap#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the descriptor heap object. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>The <b>REFIID</b>, or <b>GUID</b>, of the interface to the descriptor heap can be obtained by using the __uuidof() macro. For example, __uuidof(<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12descriptorheap">ID3D12DescriptorHeap</a>) will get the <b>GUID</b> of the interface to a descriptor heap.</remarks>
	void CreateDescriptorHeap( in  DescriptorHeapDescription pDescriptorHeapDesc,
							   in  Guid                      riid, 
							   out IDescriptorHeap?          ppvHeap ) ;


	/// <summary>Gets the size of the handle increment for the given type of descriptor heap. This value is typically used to increment a handle into a descriptor array by the correct amount.</summary>
	/// <param name="DescriptorHeapType">The <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a>-typed value that specifies the type of descriptor heap to get the size of the handle increment for.</param>
	/// <returns>Returns the size of the handle increment for the given type of descriptor heap, including any necessary padding.</returns>
	/// <remarks>The descriptor size returned by this method is used as one input to the helper structures <a href="https://docs.microsoft.com/windows/desktop/direct3d12/cd3dx12-cpu-descriptor-handle">CD3DX12_CPU_DESCRIPTOR_HANDLE</a> and <a href="https://docs.microsoft.com/windows/desktop/direct3d12/cd3dx12-gpu-descriptor-handle">CD3DX12_GPU_DESCRIPTOR_HANDLE</a>.</remarks>
	uint GetDescriptorHandleIncrementSize( DescriptorHeapType DescriptorHeapType ) ;



	/// <summary>Creates a root signature layout.</summary>
	/// <param name="nodeMask">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT</a></b> For single GPU operation, set this to zero. If there are multiple GPU nodes, set bits to identify the nodes (the  device's physical adapters) to which the root signature is to apply. Each bit in the mask corresponds to a single node. Refer to <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pBlobWithRootSignature">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">void</a>*</b> A pointer to the source data for the serialized signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="blobLengthInBytes">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">SIZE_T</a></b> The size, in bytes, of the block of memory that <i>pBlobWithRootSignature</i> points to.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b><b>REFIID</b></b> The globally unique identifier (<b>GUID</b>) for the root signature interface. See Remarks. An input parameter.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvRootSignature">
	/// <para>Type: <b><b>void</b>**</b> A pointer to a memory block that receives a pointer to the root signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns <b>S_OK</b> if successful; otherwise, returns one of the <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// <para>This method returns <b>E_INVALIDARG</b> if the blob that <i>pBlobWithRootSignature</i> points to is invalid.</para>
	/// </returns>
	/// <remarks>
	/// <para>If an application procedurally generates a <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_root_signature_desc">D3D12_ROOT_SIGNATURE_DESC</a> data structure, it must pass a pointer to this <b>D3D12_ROOT_SIGNATURE_DESC</b> in a call to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-d3d12serializerootsignature">D3D12SerializeRootSignature</a> to make the serialized form. The application then passes the serialized form to <i>pBlobWithRootSignature</i> in a call to <b>ID3D12Device::CreateRootSignature</b>.</para>
	/// <para>The <b>REFIID</b>, or <b>GUID</b>, of the interface to the root signature layout can be obtained by using the __uuidof() macro. For example, __uuidof(<a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a>) will get the <b>GUID</b> of the interface to a root signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrootsignature#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateRootSignature( uint                nodeMask,
							  nint                pBlobWithRootSignature,
							  nuint               blobLengthInBytes, 
							  in  Guid            riid,
							  out IRootSignature? ppvRootSignature ) ;


	/// <summary>Creates a constant-buffer view for accessing resource data.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_constant_buffer_view_desc">D3D12_CONSTANT_BUFFER_VIEW_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_constant_buffer_view_desc">D3D12_CONSTANT_BUFFER_VIEW_DESC</a> structure that describes the constant-buffer view.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createconstantbufferview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the start of the heap that holds the constant-buffer view.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createconstantbufferview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createconstantbufferview">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateConstantBufferView( [Optional] in ConstBufferViewDescription pDesc,
								   in CPUDescriptorHandle DestDescriptor ) ;


	/// <summary>Creates a shader-resource view for accessing data in a resource. (ID3D12Device.CreateShaderResourceView)</summary>
	/// <param name="pResource">
	///     <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> object that represents the shader resource. At least one of <i>pResource</i> or <i>pDesc</i>  must be provided. A null <i>pResource</i> is used to initialize a null descriptor, which guarantees D3D11-like null binding behavior (reading 0s, writes are discarded), but must have a valid <i>pDesc</i> in order to determine the descriptor type.</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDesc">
	///     <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_shader_resource_view_desc">D3D12_SHADER_RESOURCE_VIEW_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_shader_resource_view_desc">D3D12_SHADER_RESOURCE_VIEW_DESC</a> structure that describes the shader-resource view. A null <i>pDesc</i> is used to initialize a default descriptor, if possible. This behavior is identical to the D3D11 null descriptor behavior, where defaults are filled in. This behavior inherits the resource format and dimension (if not typeless) and for buffers SRVs target a full buffer and are typed (not raw or structured), and for textures SRVs target a full texture, all mips and all array slices. Not all resources support null descriptor initialization.</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="destDescriptor">
	///     <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the shader-resource view. This handle can be created in a shader-visible or non-shader-visible descriptor heap.</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><h3><a id="Processing_YUV_4_2_0_video_formats"></a><a id="processing_yuv_4_2_0_video_formats"></a><a id="PROCESSING_YUV_4_2_0_VIDEO_FORMATS"></a>Processing YUV 4:2:0 video formats</h3> An app must map the luma (Y) plane separately from the chroma (UV) planes. Developers do this by calling <b>CreateShaderResourceView</b> twice for the same texture and passing in 1-channel and 2-channel formats. Passing in a 1-channel format compatible with the Y plane maps only the Y plane. Passing in a 2-channel format compatible with the UV planes (together) maps only the U and V planes as a single resource view. YUV 4:2:0 formats are listed in <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateShaderResourceView( [Optional] IResource? pResource,
								   [Optional] ShaderResourceViewDescription pDesc,
								   CPUDescriptorHandle destDescriptor = default ) ;
	

	/// <summary>Creates a view for unordered accessing.</summary>
	/// <param name="pResource">
	/// <para>Type: [in, optional] <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> object that represents the unordered access. At least one of <i>pResource</i> or <i>pDesc</i> must be provided. A null <i>pResource</i> is used to initialize a null descriptor, which guarantees Direct3D 11-like null binding behavior (reading 0s, writes are discarded), but must have a valid <i>pDesc</i> in order to determine the descriptor type.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createunorderedaccessview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pCounterResource">
	/// <para>Type: [in, optional] <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> The <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> for the counter (if any) associated with the UAV. If <i>pCounterResource</i> is not specified, then the <b>CounterOffsetInBytes</b> member of the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_buffer_uav">D3D12_BUFFER_UAV</a> structure must be 0. If <i>pCounterResource</i> is specified, then there is a counter associated with the UAV, and the runtime performs validation of the following requirements: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createunorderedaccessview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: [in, optional] <b>const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_unordered_access_view_desc">D3D12_UNORDERED_ACCESS_VIEW_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_unordered_access_view_desc">D3D12_UNORDERED_ACCESS_VIEW_DESC</a> structure that describes the unordered-access view. A null <i>pDesc</i> is used to initialize a default descriptor, if possible. This behavior is identical to the D3D11 null descriptor behavior, where defaults are filled in. This behavior inherits the resource format and dimension (if not typeless) and for buffers UAVs target a full buffer and are typed, and for textures UAVs target the first mip and all array slices. Not all resources support null descriptor initialization.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createunorderedaccessview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the start of the heap that holds the unordered-access view.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createunorderedaccessview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createunorderedaccessview">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateUnorderedAccessView( IResource                         pResource,
									IResource                         pCounterResource,
									in UnorderedAccessViewDescription pDesc,
									CPUDescriptorHandle               DestDescriptor ) ;


	/// <summary>Creates a render-target view for accessing resource data. (ID3D12Device.CreateRenderTargetView)</summary>
	/// <param name="pResource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> object that represents the render target.</para>
	/// <para>At least one of <i>pResource</i> or <i>pDesc</i>  must be provided. A null <i>pResource</i> is used to initialize a null descriptor, which guarantees D3D11-like null binding behavior (reading 0s, writes are discarded), but must have a valid <i>pDesc</i> in order to determine the descriptor type.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrendertargetview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDescription">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_target_view_desc">D3D12_RENDER_TARGET_VIEW_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_target_view_desc">D3D12_RENDER_TARGET_VIEW_DESC</a> structure that describes the render-target view. A null <i>pDesc</i> is used to initialize a default descriptor, if possible. This behavior is identical to the D3D11 null descriptor behavior, where defaults are filled in. This behavior inherits the resource format and dimension (if not typeless) and RTVs target the first mip and all array slices. Not all resources support null descriptor initialization.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrendertargetview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the destination where the newly-created render target view will reside.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrendertargetview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createrendertargetview">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateRenderTargetView( IResource                                pResource,
								 [Optional] in RenderTargetViewDescription pDescription,
								 CPUDescriptorHandle                       DestDescriptor ) ;


	/// <summary>Creates a depth-stencil view for accessing resource data.</summary>
	/// <param name="pResource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a> object that represents the depth stencil.</para>
	/// <para>At least one of <i>pResource</i> or <i>pDesc</i>  must be provided. A null <i>pResource</i> is used to initialize a null descriptor, which guarantees D3D11-like null binding behavior (reading 0s, writes are discarded), but must have a valid <i>pDesc</i> in order to determine the descriptor type.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdepthstencilview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_depth_stencil_view_desc">D3D12_DEPTH_STENCIL_VIEW_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_depth_stencil_view_desc">D3D12_DEPTH_STENCIL_VIEW_DESC</a> structure that describes the depth-stencil view.</para>
	/// <para>A null <i>pDesc</i> is used to initialize a default descriptor, if possible. This behavior is identical to the D3D11 null descriptor behavior, where defaults are filled in. This behavior inherits the resource format and dimension (if not typeless) and DSVs target the  first mip and all array slices. Not all resources support null descriptor initialization.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdepthstencilview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the start of the heap that holds the depth-stencil view.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdepthstencilview#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createdepthstencilview">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateDepthStencilView( IResource                          pResource,
								 [Optional] in DepthStencilViewDesc pDesc,
								 CPUDescriptorHandle                DestDescriptor ) ;


	/// <summary>Create a sampler object that encapsulates sampling information for a texture.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_sampler_desc">D3D12_SAMPLER_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_sampler_desc">D3D12_SAMPLER_DESC</a> structure that describes the sampler.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsampler#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> Describes the CPU descriptor handle that represents the start of the heap that holds the sampler.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsampler#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsampler">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateSampler( in SamplerDescription pDesc, CPUDescriptorHandle DestDescriptor ) ;


	/// <summary>Copies descriptors from a source to a destination. (ID3D12Device.CopyDescriptors)</summary>
	/// <param name="NumDestDescriptorRanges">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of destination descriptor ranges to copy to.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDestDescriptorRangeStarts">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a>*</b> An array of <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> objects to copy to. All the destination and source descriptors must be in heaps of the same [D3D12_DESCRIPTOR_HEAP_TYPE](/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDestDescriptorRangeSizes">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> An array of destination descriptor range sizes to copy to.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumSrcDescriptorRanges">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of source descriptor ranges to copy from.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcDescriptorRangeStarts">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a>*</b> An array of <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> objects to copy from. > [!IMPORTANT] > All elements in the *pSrcDescriptorRangeStarts* parameter must be in a non shader-visible descriptor heap. This is because shader-visible descriptor heaps may be created in **WRITE_COMBINE** memory or GPU local memory, which is prohibitively slow to read from. If your application manages descriptor heaps via copying the descriptors required for a given pass or frame from local "storage" descriptor heaps to the GPU-bound descriptor heap, use shader-opaque heaps for the storage heaps and copy into the GPU-visible heap as required.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSrcDescriptorRangeSizes">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> An array of source descriptor range sizes to copy from.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DescriptorHeapsType">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a></b> The <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a>-typed value that specifies the type of descriptor heap to copy with. This is required as different descriptor types may have different sizes. Both the source and destination descriptor heaps must have the same type, else the debug layer will emit an error.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>Where applicable, prefer [**ID3D12Device::CopyDescriptorsSimple**](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple) to this method. It can have a better CPU cache miss rate due to the linear nature of the copy.</remarks>
	void CopyDescriptors( uint                           NumDestDescriptorRanges,
						  in Span< CPUDescriptorHandle > pDestDescriptorRangeStarts,
						  uint[]                         pDestDescriptorRangeSizes,
						  uint                           NumSrcDescriptorRanges,
						  in Span< CPUDescriptorHandle > pSrcDescriptorRangeStarts,
						  uint[]                         pSrcDescriptorRangeSizes,
						  DescriptorHeapType             DescriptorHeapsType ) ;


	/// <summary>Copies descriptors from a source to a destination. (ID3D12Device.CopyDescriptorsSimple)</summary>
	/// <param name="NumDescriptors">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of descriptors to copy.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DestDescriptorRangeStart">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> A <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> that describes the destination descriptors to start to copy to. The destination and source descriptors must be in heaps of the same [D3D12_DESCRIPTOR_HEAP_TYPE](/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="SrcDescriptorRangeStart">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> A <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">CPUDescriptorHandle</a></b> that describes the source descriptors to start to copy from. > [!IMPORTANT] > The *SrcDescriptorRangeStart* parameter must be in a non shader-visible descriptor heap. This is because shader-visible descriptor heaps may be created in **WRITE_COMBINE** memory or GPU local memory, which is prohibitively slow to read from. If your application manages descriptor heaps via copying the descriptors required for a given pass or frame from local "storage" descriptor heaps to the GPU-bound descriptor heap, then use shader-opaque heaps for the storage heaps and copy into the GPU-visible heap as required.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DescriptorHeapsType">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a></b> The <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_heap_type">D3D12_DESCRIPTOR_HEAP_TYPE</a>-typed value that specifies the type of descriptor heap to copy with. This is required as different descriptor types may have different sizes. Both the source and destination descriptor heaps must have the same type, else the debug layer will emit an error.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>Where applicable, prefer this method to [**ID3D12Device::CopyDescriptors**](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors). It can have a better CPU cache miss rate due to the linear nature of the copy.</remarks>
	void CopyDescriptorsSimple( uint                NumDescriptors,
								CPUDescriptorHandle DestDescriptorRangeStart,
								CPUDescriptorHandle SrcDescriptorRangeStart,
								DescriptorHeapType  DescriptorHeapsType ) ;


	/// <summary>Gets the size and alignment of memory required for a collection of resources on this adapter.</summary>
	/// <param name="visibleMask">
	/// <para>Type: **[UINT](/windows/win32/WinProg/windows-data-types)** For single-GPU operation, set this to zero. If there are multiple GPU nodes, then set bits to identify the nodes (the device's physical adapters). Each bit in the mask corresponds to a single node. Also see [Multi-adapter systems](/windows/win32/direct3d12/multi-engine).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="numResourceDescs">
	/// <para>Type: **[UINT](/windows/win32/WinProg/windows-data-types)** The number of resource descriptors in the *pResourceDescs* array.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pResourceDescs">
	/// <para>Type: **const [ResourceDescription](./ns-d3d12-d3d12_resource_desc.md)\*** An array of <b>ResourceDescription</b> structures that described the resources to get info about.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[D3D12_RESOURCE_ALLOCATION_INFO](./ns-d3d12-d3d12_resource_allocation_info.md)** A [D3D12_RESOURCE_ALLOCATION_INFO](./ns-d3d12-d3d12_resource_allocation_info.md) structure that provides info about video memory allocated for the specified array of resources. If an error occurs, then **D3D12_RESOURCE_ALLOCATION_INFO::SizeInBytes** equals **UINT64_MAX**.</para>
	/// </returns>
	/// <remarks>
	/// <para>When you're using [CreatePlacedResource](./nf-d3d12-id3d12device-createplacedresource.md), your application must use **GetResourceAllocationInfo** in order to understand the size and alignment characteristics of texture resources. The results of this method vary depending on the particular adapter, and must be treated as unique to this adapter and driver version. Your application can't use the output of **GetResourceAllocationInfo** to understand packed mip properties of textures. To understand packed mip properties of textures, your application must use [GetResourceTiling](./nf-d3d12-id3d12device-getresourcetiling.md). Texture resource sizes significantly differ from the information returned by **GetResourceTiling**, because some adapter architectures allocate extra memory for textures to reduce the effective bandwidth during common rendering scenarios. This even includes textures that have constraints on their texture layouts, or have standardized texture layouts. That extra memory can't be sparsely mapped nor remapped by an application using [CreateReservedResource](./nf-d3d12-id3d12device-createreservedresource.md) and [UpdateTileMappings](./nf-d3d12-id3d12commandqueue-updatetilemappings.md), so it isn't reported by **GetResourceTiling**. Your application can forgo using **GetResourceAllocationInfo** for buffer resources ([D3D12_RESOURCE_DIMENSION_BUFFER](./ne-d3d12-d3d12_resource_dimension.md)). Buffers have the same size on all adapters, which is merely the smallest multiple of 64KB that's greater or equal to [ResourceDescription::Width](./ns-d3d12-d3d12_resource_desc.md). When multiple resource descriptions are passed in, the C++ algorithm for calculating a structure size and alignment are used. For example, a three-element array with two tiny 64KB-aligned resources and a tiny 4MB-aligned resource, reports differing sizes based on the order of the array. If the 4MB aligned resource is in the middle, then the resulting **Size** is 12MB. Otherwise, the resulting **Size** is 8MB. The **Alignment** returned would always be 4MB, because it's the superset of all alignments in the resource array.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	ResourceAllocationInfo GetResourceAllocationInfo( uint                        visibleMask,
													  uint                        numResourceDescs,
													  Span< ResourceDescription > pResourceDescs ) ;



	/// <summary>Divulges the equivalent custom heap properties that are used for non-custom heap types, based on the adapter's architectural properties.</summary>
	/// <param name="nodeMask">
	/// <para>Type: <b>UINT</b> For single-GPU operation, set this to zero. If there are multiple GPU nodes, set a bit to identify the node (the  device's physical adapter). Each bit in the mask corresponds to a single node. Only 1 bit must be set. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcustomheapproperties#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="heapType">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type">D3D12_HEAP_TYPE</a></b> A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type">D3D12_HEAP_TYPE</a>-typed value that specifies the heap to get properties for. D3D12_HEAP_TYPE_CUSTOM is not supported as a parameter value.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcustomheapproperties#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_properties">D3D12_HEAP_PROPERTIES</a></b> Returns a <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_properties">D3D12_HEAP_PROPERTIES</a> structure that provides properties for the specified heap. The <b>Type</b> member of the returned D3D12_HEAP_PROPERTIES is always D3D12_HEAP_TYPE_CUSTOM.</para>
	/// <para>When <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture">D3D12_FEATURE_DATA_ARCHITECTURE</a>::UMA is FALSE, the returned D3D12_HEAP_PROPERTIES members convert as follows:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcustomheapproperties">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HeapProperties GetCustomHeapProperties( uint nodeMask, HeapType heapType ) ;


	/// <summary>Creates both a resource and an implicit heap, such that the heap is big enough to contain the entire resource, and the resource is mapped to the heap.</summary>
	/// <param name="pHeapProperties">
	/// <para>Type: **const [D3D12_HEAP_PROPERTIES](./ns-d3d12-d3d12_heap_properties.md)\*** A pointer to a **D3D12_HEAP_PROPERTIES** structure that provides properties for the resource's heap.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="HeapFlags">
	/// <para>Type: **[D3D12_HEAP_FLAGS](./ne-d3d12-d3d12_heap_flags.md)** Heap options, as a bitwise-OR'd combination of **D3D12_HEAP_FLAGS** enumeration constants.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: **const [ResourceDescription](./ns-d3d12-d3d12_resource_desc.md)\*** A pointer to a **ResourceDescription** structure that describes the resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="InitialResourceState">
	/// <para>Type: **[ResourceStates](./ne-d3d12-d3d12_resource_states.md)** The initial state of the resource, as a bitwise-OR'd combination of **ResourceStates** enumeration constants. When you create a resource together with a [D3D12_HEAP_TYPE_UPLOAD](./ne-d3d12-d3d12_heap_type.md) heap, you must set *InitialResourceState* to [D3D12_RESOURCE_STATE_GENERIC_READ](./ne-d3d12-d3d12_resource_states.md). When you create a resource together with a [D3D12_HEAP_TYPE_READBACK](./ne-d3d12-d3d12_heap_type.md) heap, you must set *InitialResourceState* to [D3D12_RESOURCE_STATE_COPY_DEST](./ne-d3d12-d3d12_resource_states.md).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: **const [ClearValue](./ns-d3d12-d3d12_clear_value.md)\*** Specifies a **ClearValue** structure that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the [D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET](./ne-d3d12-d3d12_resource_flags.md) or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, you should choose the value with which the clear operation will most commonly be called. You can call the clear operation with other values, but those operations won't be as efficient as when the value matches the one passed in to resource creation. When you use [D3D12_RESOURCE_DIMENSION_BUFFER](./ne-d3d12-d3d12_resource_dimension.md), you must set *pOptimizedClearValue* to `nullptr`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riidResource">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the resource interface to return in *ppvResource*. While *riidResource* is most commonly the **GUID** of [ID3D12Resource](./nn-d3d12-id3d12resource.md), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created resource object. *ppvResource* can be `nullptr`, to enable capability testing. When *ppvResource* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the resource.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method creates both a resource and a heap, such that the heap is big enough to contain the entire resource, and the resource is mapped to the heap. The created heap is known as an implicit heap, because the heap object can't be obtained by the application. Before releasing the final reference on the resource, your application must ensure that the GPU will no longer read nor write to this resource. The implicit heap is made resident for GPU access before the method returns control to your application. Also see [Residency](/windows/win32/direct3d12/residency). The resource GPU VA mapping can't be changed. See [ID3D12CommandQueue::UpdateTileMappings](./nf-d3d12-id3d12commandqueue-updatetilemappings.md) and [Volume tiled resources](/windows/win32/direct3d12/volume-tiled-resources). This method may be called by multiple threads concurrently.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommittedresource#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateCommittedResource( in HeapProperties         pHeapProperties,
								  HeapFlags                 HeapFlags,
								  in ResourceDescription    pDesc,
								  ResourceStates            InitialResourceState,
								  [Optional] in ClearValue? pOptimizedClearValue,
								  in            Guid        riidResource,
								  out           IResource?  ppvResource ) ;


	/// <summary>Creates a heap that can be used with placed resources and reserved resources.</summary>
	/// <param name="pDesc">
	/// <para>Type: **const [D3D12_HEAP_DESC](./ns-d3d12-d3d12_heap_desc.md)\*** A pointer to a constant **D3D12_HEAP_DESC** structure that describes the heap.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createheap#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the heap interface to return in *ppvHeap*. While *riidResource* is most commonly the **GUID** of [IHeap](./nn-d3d12-id3d12heap.md), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createheap#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvHeap">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created heap object. *ppvHeap* can be `nullptr`, to enable capability testing. When *ppvHeap* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createheap#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the heap.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>**CreateHeap** creates a heap that can be used with placed resources and reserved resources. Before releasing the final reference on the heap, your application must ensure that the GPU will no longer read or write to this heap. A placed resource object holds a reference on the heap it is created on; but a reserved resource doesn't hold a reference for each mapping made to a heap.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createheap#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateHeap( in HeapDescription pDesc, in Guid riid, out IHeap ppvHeap ) ;


	/// <summary>
	/// Creates a resource that is placed in a specific heap. Placed resources are the lightest
	/// weight resource objects available, and are the fastest to create and destroy.
	/// </summary>
	/// <param name="pHeap">
	/// <para>Type: [in] <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12heap">IHeap</a>*
	/// A pointer to the <b>IHeap</b> interface that represents the heap in which the resource is placed.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="HeapOffset">
	/// <para>Type: <a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a>
	/// The offset, in bytes, to the resource. The *HeapOffset* must be a multiple of the resource's alignment, and
	/// *HeapOffset* plus the resource size must be smaller than or equal to the heap size.
	/// <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo">
	/// GetResourceAllocationInfo</a> must be used to understand the sizes of texture resources.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: [in] **const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_desc">
	/// ResourceDescription</a>* A pointer to a <b>ResourceDescription</b> structure that describes the resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="InitialState">
	/// <para>Type: **<a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states">ResourceStates</a>**
	/// The initial state of the resource, as a bitwise-OR'd combination of ResourceStates enumeration constants.
	/// When a resource is created together with a **D3D12_HEAP_TYPE_UPLOAD** heap, *InitialState* must be
	/// **D3D12_RESOURCE_STATE_GENERIC_READ**. When a resource is created together with a **D3D12_HEAP_TYPE_READBACK**
	/// heap, *InitialState* must be **D3D12_RESOURCE_STATE_COPY_DEST**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: [in, optional] **const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_clear_value">
	/// ClearValue</a>*** Specifies a **ClearValue** that describes the default value for a clear color. *pOptimizedClearValue*
	/// specifies a value for which clear operations are most optimal. When the created resource is a texture with either the
	/// **D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET** or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, your application should
	/// choose the value that the clear operation will most commonly be called with. Clear operations can be called with other values,
	/// but those operations will not be as efficient as when the value matches the one passed into resource creation.
	/// *pOptimizedClearValue* must be NULL when used with **D3D12_RESOURCE_DIMENSION_BUFFER**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the resource interface.
	/// This is an input parameter. The <b>REFIID</b>, or <b>GUID</b>, of the interface to the resource can be
	/// obtained by using the `__uuidof` macro. For example, `__uuidof(ID3D12Resource)` gets the <b>GUID</b>
	/// of the interface to a resource. Although <b>riid</b> is, most commonly, the GUID for
	/// <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource"><b>ID3D12Resource</b></a>,
	/// it may be any <b>GUID</b> for any interface. If the resource object doesn't support the interface for this **GUID**,
	/// then creation fails with **E_NOINTERFACE**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: [out, optional] <b>void**</b> A pointer to a memory block that receives a pointer to the resource.
	/// *ppvResource* can be NULL, to enable capability testing. When *ppvResource* is NULL, no object will be created and
	/// S_FALSE will be returned when *pResourceDesc* and other parameters are valid.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b>
	/// This method returns **E_OUTOFMEMORY** if there is insufficient memory to create the resource.
	/// See <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">
	/// Direct3D 12 Return Codes
	/// </a> for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>
	/// <b>CreatePlacedResource</b> is similar to fully mapping a reserved resource to an offset within a heap;
	/// but the virtual address space associated with a heap may be reused as well. Placed resources are lighter
	/// weight to create and destroy than committed resources are. This is because no heap is created nor destroyed
	/// during those operations. In addition, placed resources enable an even lighter weight technique to reuse
	/// memory than resource creation and destruction; that is, reuse through aliasing, and aliasing barriers.
	/// Multiple placed resources may simultaneously overlap each other on the same heap, but only a single
	/// overlapping resource can be used at a time. There are two placed resource usage semantics; a simple
	/// model, and an advanced model. We recommend that you choose the simple model (it maximizes graphics tool
	/// support across the diverse ecosystem of GPUs), unless and until you find that you need the advanced model
	/// for your app.
	/// </para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createplacedresource#">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreatePlacedResource( IHeap                     pHeap, ulong HeapOffset,
							   in ResourceDescription    pDesc,
							   ResourceStates            InitialState,
							   [Optional] in ClearValue? pOptimizedClearValue,
							   in            Guid        riid, out IResource ppvResource ) ;


	/// <summary>Creates a resource that is reserved, and not yet mapped to any pages in a heap.</summary>
	/// <param name="pDesc">
	/// <para>Type: **const [ResourceDescription](./ns-d3d12-d3d12_resource_desc.md)\*** A pointer to a **ResourceDescription** structure that describes the resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="InitialState">
	/// <para>Type: **[ResourceStates](./ne-d3d12-d3d12_resource_states.md)** The initial state of the resource, as a bitwise-OR'd combination of **ResourceStates** enumeration constants.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: **const [ClearValue](./ns-d3d12-d3d12_clear_value.md)\*** Specifies a **ClearValue** structure that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the [D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET](./ne-d3d12-d3d12_resource_flags.md) or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, you should choose the value with which the clear operation will most commonly be called. You can call the clear operation with other values, but those operations won't be as efficient as when the value matches the one passed in to resource creation. When you use [D3D12_RESOURCE_DIMENSION_BUFFER](./ne-d3d12-d3d12_resource_dimension.md), you must set *pOptimizedClearValue* to `nullptr`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the resource interface to return in *ppvResource*. See **Remarks**. While *riidResource* is most commonly the **GUID** of [ID3D12Resource](./nn-d3d12-id3d12resource.md), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created resource object. *ppvResource* can be `nullptr`, to enable capability testing. When *ppvResource* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/win32/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the resource.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>**CreateReservedResource** is equivalent to [D3D11_RESOURCE_MISC_TILED](../d3d11/ne-d3d11-d3d11_resource_misc_flag.md) in Direct3D 11. It creates a resource with virtual memory only, no backing store. You need to map the resource to physical memory (that is, to a heap) using <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-copytilemappings">CopyTileMappings</a> and <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-updatetilemappings">UpdateTileMappings</a>. These resource types can only be created when the adapter supports tiled resource tier 1 or greater. The tiled resource tier defines the behavior of accessing a resource that is not mapped to a heap.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateReservedResource( in ResourceDescription    pDesc,
								 ResourceStates            InitialState,
								 [Optional] in ClearValue? pOptimizedClearValue,
								 in            Guid        riid, out IResource ppvResource ) ;


	/// <summary>Creates a shared handle to a heap, resource, or fence object.</summary>
	/// <param name="pObject">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12devicechild">ID3D12DeviceChild</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12devicechild">ID3D12DeviceChild</a> interface that represents the heap, resource, or fence object to create for sharing. The following interfaces (derived from <b>ID3D12DeviceChild</b>) are supported:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pAttributes">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/aa379560(v=vs.85)">SECURITY_ATTRIBUTES</a>*</b> A pointer to a <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/aa379560(v=vs.85)">SECURITY_ATTRIBUTES</a> structure that contains two separate but related data members: an optional security descriptor, and a <b>Boolean</b> value that determines whether child processes can inherit the returned handle.</para>
	/// <para>Set this parameter to <b>NULL</b> if you want child processes that the application might create to not  inherit  the handle returned by <b>CreateSharedHandle</b>, and if you want the resource that is associated with the returned handle to get a default security descriptor.</para>
	/// <para>The <b>lpSecurityDescriptor</b> member of the structure specifies a <a href="https://docs.microsoft.com/windows/desktop/api/winnt/ns-winnt-security_descriptor">SECURITY_DESCRIPTOR</a> for the resource. Set this member to <b>NULL</b> if you want the runtime to assign a default security descriptor to the resource that is associated with the returned handle. The ACLs in the default security descriptor for the resource come from the primary or impersonation token of the creator. For more info, see <a href="https://docs.microsoft.com/windows/desktop/Sync/synchronization-object-security-and-access-rights">Synchronization Object Security and Access Rights</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Access">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">DWORD</a></b> Currently the only value this parameter accepts is GENERIC_ALL.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Name">
	/// <para>Type: <b>LPCWSTR</b> A <b>NULL</b>-terminated <b>UNICODE</b> string that contains the name to associate with the shared heap. The name is limited to MAX_PATH characters. Name comparison is case-sensitive.</para>
	/// <para>If <i>Name</i> matches the name of an existing resource, <b>CreateSharedHandle</b> fails with <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_NAME_ALREADY_EXISTS</a>. This occurs because these objects share the same namespace.</para>
	/// <para>The name can have a "Global\" or "Local\" prefix to explicitly create the object in the global or session namespace. The remainder of the name can contain any character except the backslash character (\\). For more information, see <a href="https://docs.microsoft.com/windows/desktop/TermServ/kernel-object-namespaces">Kernel Object Namespaces</a>. Fast user switching is implemented using Terminal Services sessions. Kernel object names must follow the guidelines outlined for Terminal Services so that applications can support multiple users.</para>
	/// <para>The object can be created in a private namespace. For more information, see <a href="https://docs.microsoft.com/windows/desktop/Sync/object-namespaces">Object Namespaces</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pHandle">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HANDLE</a>*</b> A pointer to a variable that receives the NT HANDLE value to the resource to share. You can use this handle in calls to access the resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns one of the following values:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>Both heaps and committed resources can be shared. Sharing a committed resource shares the implicit heap along with the committed resource description, such that a compatible resource description can be mapped to the heap from another device. For Direct3D 11 and Direct3D 12 interop scenarios, a shared fence is opened in DirectX 11 with the <a href="https://docs.microsoft.com/windows/win32/api/d3d11_4/nf-d3d11_4-id3d11device5-opensharedfence">ID3D11Device5::OpenSharedFence</a> method, and a shared resource is opened with the <a href="https://docs.microsoft.com/windows/win32/api/d3d11_1/nf-d3d11_1-id3d11device1-opensharedresource1">ID3D11Device::OpenSharedResource1</a> method. For Direct3D 12, a shared handle is opened with the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandle">ID3D12Device::OpenSharedHandle</a> or the ID3D12Device::OpenSharedHandleByName method.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateSharedHandle( IDeviceChild                     pObject,
							 [Optional] in SecurityAttributes pAttributes,
							 uint                             Access, string Name, out Win32Handle pHandle ) ;


	/// <summary>Opens a handle for shared resources, shared heaps, and shared fences, by using HANDLE and REFIID.</summary>
	/// <param name="NTHandle">
	/// <para>Type: <b>HANDLE</b> The handle that was output by the call to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createsharedhandle">ID3D12Device::CreateSharedHandle</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandle#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for one of the following interfaces:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandle#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvObj">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to one of the following interfaces:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandle#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandle">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void OpenSharedHandle( Win32Handle NTHandle, in Guid riid, out object ppvObj ) ;

	/// <summary>Opens a handle for shared resources, shared heaps, and shared fences, by using Name and Access.</summary>
	/// <param name="Name">
	/// <para>Type: <b>LPCWSTR</b> The name that was optionally passed as the <i>Name</i> parameter in the call to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createsharedhandle">ID3D12Device::CreateSharedHandle</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandlebyname#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Access">
	/// <para>Type: <b>DWORD</b> The access level that was specified in the <i>Access</i> parameter in the call to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createsharedhandle">ID3D12Device::CreateSharedHandle</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandlebyname#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pNTHandle">
	/// <para>Type: <b>HANDLE*</b> Pointer to the shared handle.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandlebyname#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-opensharedhandlebyname">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void OpenSharedHandleByName( string Name, uint Access, ref Win32Handle pNTHandle ) ;



	/// <summary>Makes objects resident for the device.</summary>
	/// <param name="NumObjects">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of objects  in the <i>ppObjects</i> array to make resident for the device.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-makeresident#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppObjects">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>*</b> A pointer to a memory block that contains an array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a> interface pointers for the objects.</para>
	/// <para>Even though most D3D12 objects inherit from <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>, residency changes are only supported on the following objects: Descriptor Heaps, Heaps, Committed Resources, and Query Heaps</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-makeresident#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>MakeResident</b> loads the data associated with a resource from disk, and re-allocates the memory from the resource's appropriate memory pool. This method should be called on the object which owns the physical memory.</para>
	/// <para>Use this method, and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a>, to manage GPU video memory, noting that this was done automatically in D3D11, but now has to be done by the app in D3D12. <b>MakeResident</b> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a> can help applications manage the residency budget on many adapters. <b>MakeResident</b> explicitly pages-in data and, then, precludes page-out so the GPU can access the data. <b>Evict</b> enables page-out. Some GPU architectures do not benefit from residency manipulation, due to the lack of sufficient GPU virtual address space. Use <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_gpu_virtual_address_support">D3D12_FEATURE_DATA_GPU_VIRTUAL_ADDRESS_SUPPORT</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-queryvideomemoryinfo">IDXGIAdapter3::QueryVideoMemoryInfo</a> to recognize when the maximum GPU VA space per-process is too small or roughly the same size as the residency budget. For such architectures, the residency budget will always be constrained by the amount of GPU virtual address space. <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a> will not free-up any residency budget on such systems.</para>
	/// <para>Applications must handle <b>MakeResident</b> failures, even if there appears to be enough residency budget available. Physical memory fragmentation and adapter architecture quirks can preclude the utilization of large contiguous ranges. Applications should free up more residency budget before trying again.</para>
	/// <para><b>MakeResident</b> is ref-counted, such that <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a> must be called the same amount of times as <b>MakeResident</b> before <b>Evict</b> takes effect. Objects that support residency are made resident during creation, so a single <b>Evict</b> call will actually evict the object. Applications must use fences to ensure the GPU doesn't use non-resident objects. <b>MakeResident</b> must return before the GPU executes a command list that references the object. <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a> must be called after the GPU finishes executing a command list that references the object. Evicted objects still consume the same GPU virtual address and same amount of GPU virtual address space. Therefore, resource descriptors and other GPU virtual address references are not invalidated after <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-makeresident#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void MakeResident< P >( uint NumObjects, Span< P > ppObjects ) where P: IPageable ;


	/// <summary>Enables the page-out of data, which precludes GPU access of that data.</summary>
	/// <param name="NumObjects">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of objects in the <i>ppObjects</i> array to evict from the device.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-evict#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppObjects">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>*</b> A pointer to a memory block that contains an array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a> interface pointers for the objects.</para>
	/// <para>Even though most D3D12 objects inherit from <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>, residency changes are only supported on the following objects: Descriptor Heaps, Heaps, Committed Resources, and Query Heaps</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-evict#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>Evict</b> persists the data associated with a resource to disk, and then removes the resource from the memory pool where it was located. This method should be called on the object which owns the physical memory: either a committed resource (which owns both virtual  and physical memory assignments) or a heap - noting that reserved resources do not have physical memory, and placed resources are borrowing memory from a heap.</para>
	/// <para>Refer to the remarks for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-makeresident">MakeResident</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-evict#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void Evict< P >( uint NumObjects, Span< P > ppObjects ) where P: IPageable ;


	/// <summary>Creates a fence object. (ID3D12Device.CreateFence)</summary>
	/// <param name="InitialValue">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT64</a></b> The initial value for the fence.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createfence#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Flags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_fence_flags">D3D12_FENCE_FLAGS</a></b> A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_fence_flags">D3D12_FENCE_FLAGS</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies options for the fence.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createfence#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the fence interface (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the fence can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12Fence) will get the <b>GUID</b> of the interface to a fence.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createfence#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppFence">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a> interface that is used to access the fence.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createfence#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HRESULT</a></b> Returns <b>S_OK</b> if successful; otherwise, returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createfence">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateFence( ulong       InitialValue, 
					  FenceFlags  Flags,
					  in  Guid    riid,
					  out IFence? ppFence ) ;


	/// <summary>Gets the reason that the device was removed.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns the reason that the device was removed.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getdeviceremovedreason">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult GetDeviceRemovedReason( ) ;


	/// <summary>
	/// Gets a resource layout that can be copied. Helps the app fill-in
	/// D3D12_PLACED_SUBRESOURCE_FOOTPRINT and D3D12_SUBRESOURCE_FOOTPRINT
	/// when suballocating space in upload heaps.
	/// </summary>
	/// <param name="pResourceDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_resource_desc">ResourceDescription</a>*</b> A description of the resource, as a pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_resource_desc">ResourceDescription</a> structure.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="FirstSubresource">
	/// <para>Type: <b>UINT</b> Index of the first subresource in the resource. The range of valid values is 0 to D3D12_REQ_SUBRESOURCES.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="NumSubresources">
	/// <para>Type: <b>UINT</b> The number of subresources in the resource.  The range of valid values is 0 to (D3D12_REQ_SUBRESOURCES - <i>FirstSubresource</i>).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="BaseOffset">
	/// <para>Type: <b>UINT64</b> The offset, in bytes, to the resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pLayouts">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint">D3D12_PLACED_SUBRESOURCE_FOOTPRINT</a>*</b> A pointer to an array (of length <i>NumSubresources</i>) of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint">D3D12_PLACED_SUBRESOURCE_FOOTPRINT</a> structures, to be filled with the description and placement of each subresource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pNumRows">
	/// <para>Type: <b>UINT*</b> A pointer to an array (of length <i>NumSubresources</i>) of integer  variables, to be filled with the number of rows for each subresource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pRowSizeInBytes">
	/// <para>Type: <b>UINT64*</b> A pointer to an array (of length <i>NumSubresources</i>) of integer variables, each entry to be filled with the unpadded size in bytes of a row, of each subresource.</para>
	/// <para>For example, if a Texture2D resource has a width of 32 and bytes per pixel of 4, then <i>pRowSizeInBytes</i> returns 128. <i>pRowSizeInBytes</i> should not be confused with <b>row pitch</b>, as examining <i>pLayouts</i> and getting the row pitch from that will give you 256 as it is aligned to D3D12_TEXTURE_DATA_PITCH_ALIGNMENT.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pTotalBytes">
	/// <para>Type: <b>UINT64*</b> A pointer to an integer variable, to be filled with the total size, in bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><b>WARNING: </b></para>
	/// This function allocates unmanaged memory for the <see cref="PlacedSubresourceFootprint"/>
	/// data returned by the API, and gives it to you in the form of a <see cref="Span{T}"/>.
	/// Additionally, it returns a <see cref="nint"/> (pointer) to the allocated memory of the Span. 
	/// It is the responsibility of the application to <i>release this memory when finished using it</i>.
	/// Failure to do so will create a memory leak which can be detrimental to performance ...<para/>
	/// <para>This routine assists the application in filling out <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint">D3D12_PLACED_SUBRESOURCE_FOOTPRINT</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_subresource_footprint">D3D12_SUBRESOURCE_FOOTPRINT</a> structures, when suballocating space in upload heaps. The resulting structures are GPU adapter-agnostic, meaning that the values will not vary from one GPU adapter to the next. <b>GetCopyableFootprints</b> uses specified details about resource formats, texture layouts, and alignment requirements (from the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_resource_desc">ResourceDescription</a> structure)  to fill out the subresource structures. Applications have access to all these details, so this method, or a variation of it, could be  written as part of the app.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	nint GetCopyableFootprints( in ResourceDescription                       pResourceDesc,
								uint                                         FirstSubresource,
								uint                                         NumSubresources,
								ulong                                        BaseOffset,
								[Out] out Span< PlacedSubresourceFootprint > pLayouts,
								[Out] out Span< uint >                       pNumRows,
								[Out] out Span< ulong >                      pRowSizeInBytes,
								[Out] out ulong                              pTotalBytes ) ;


	/// <summary>Creates a query heap. A query heap contains an array of queries.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_query_heap_desc">D3D12_QUERY_HEAP_DESC</a>*</b> Specifies the query heap in a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_query_heap_desc">D3D12_QUERY_HEAP_DESC</a> structure.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createqueryheap#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> Specifies a REFIID that uniquely identifies the heap.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createqueryheap#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvHeap">
	/// <para>Type: <b>void**</b> Specifies a pointer to the heap, that will be returned on successful completion of the method. <i>ppvHeap</i> can be NULL, to enable capability testing. When <i>ppvHeap</i> is NULL, no object will be created and S_FALSE will be returned when <i>pDesc</i> is valid.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createqueryheap#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/queries">Queries</a> for more information.</remarks>
	void CreateQueryHeap( in QueryHeapDescription pDesc, in Guid riid, out IHeap ppvHeap ) ;



	/// <summary>A development-time aid for certain types of profiling and experimental prototyping.</summary>
	/// <param name="Enable">
	/// <para>Type: <b>BOOL</b> Specifies a BOOL that turns the stable power state on or off.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-setstablepowerstate#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method is only useful during the development of applications. It enables developers to profile GPU usage of multiple algorithms without experiencing artifacts from <a href="https://en.wikipedia.org/wiki/Dynamic_frequency_scaling">dynamic frequency scaling</a>. Do not call this method in normal execution for a shipped application. This method only works while the machine is in <a href="https://docs.microsoft.com/windows/uwp/get-started/enable-your-device-for-development">developer mode</a>. If developer mode is not enabled, then device removal will occur. Instead, call this method in response to an off-by-default, developer-facing switch. Calling it in response to command line parameters, config files, registry keys, and developer console commands are reasonable usage scenarios. A stable power state typically fixes GPU clock rates at a slower setting that is significantly lower than that experienced by users under normal application load. This reduction in clock rate affects the entire system. Slow clock rates are required to ensure processors don’t exhaust power, current, and thermal limits. Normal usage scenarios commonly leverage a processors ability to dynamically over-clock. Any conclusions made by comparing two designs under a stable power state should be double-checked with supporting results from real usage scenarios.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-setstablepowerstate#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetStablePowerState( bool Enable ) ;



	/// <summary>This method creates a command signature.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_command_signature_desc">D3D12_COMMAND_SIGNATURE_DESC</a>*</b> Describes the command signature to be created with the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_command_signature_desc">D3D12_COMMAND_SIGNATURE_DESC</a> structure.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pRootSignature">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a>*</b> Specifies the  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12rootsignature">ID3D12RootSignature</a> that the command signature applies to.</para>
	/// <para>The root signature is required if any of the commands in the signature will update bindings on the pipeline. If the only command present is a draw or dispatch, the root signature parameter can be set to NULL.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the command signature interface (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandsignature">ID3D12CommandSignature</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the command signature can be obtained by using the __uuidof() macro. For example, __uuidof(<b>ID3D12CommandSignature</b>) will get the <b>GUID</b> of the interface to a command signature.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvCommandSignature">
	/// <para>Type: <b>void**</b> Specifies a pointer, that on successful completion of the method will point to the created command signature (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandsignature">ID3D12CommandSignature</a>).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandsignature#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createcommandsignature">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateCommandSignature( in CommandSignatureDescription pDesc,
								 IRootSignature pRootSignature, 
								 in Guid riid,
								 out ICommandSignature ppvCommandSignature ) ;


	/// <summary>
	/// Gets info about how a tiled resource is broken into tiles.
	/// (ID3D12Device.GetResourceTiling)
	/// </summary>
	/// <param name="pTiledResource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> Specifies a tiled <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>  to get info about.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pNumTilesForEntireResource">
	/// <para>Type: <b>UINT*</b> A pointer to a variable that receives the number of tiles needed to store the entire tiled resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pPackedMipDesc">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_packed_mip_info">D3D12_PACKED_MIP_INFO</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_packed_mip_info">D3D12_PACKED_MIP_INFO</a> structure that <b>GetResourceTiling</b> fills with info about how the tiled resource's mipmaps are packed.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pStandardTileShapeForNonPackedMips">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tile_shape">D3D12_TILE_SHAPE</a>*</b> Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tile_shape">D3D12_TILE_SHAPE</a> structure that <b>GetResourceTiling</b> fills with info about the tile shape. This is info about how pixels fit in the tiles, independent of tiled resource's dimensions, not including packed mipmaps. If the entire tiled resource is packed, this parameter is meaningless because the tiled resource has no defined layout for packed mipmaps. In this situation, <b>GetResourceTiling</b> sets the members of D3D12_TILE_SHAPE to zeros.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pNumSubresourceTilings">
	/// <para>Type: <b>UINT*</b> A pointer to a variable that contains the number of tiles in the subresource. On input, this is the number of subresources to query tilings for; on output, this is the number that was actually retrieved at <i>pSubresourceTilingsForNonPackedMips</i> (clamped to what's available).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="FirstSubresourceTilingToGet">
	/// <para>Type: <b>UINT</b> The number of the first subresource tile to get. <b>GetResourceTiling</b> ignores this parameter if the number that <i>pNumSubresourceTilings</i> points to is 0.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSubresourceTilingsForNonPackedMips">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_subresource_tiling">D3D12_SUBRESOURCE_TILING</a>*</b> Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_subresource_tiling">D3D12_SUBRESOURCE_TILING</a> structure that <b>GetResourceTiling</b> fills with info about subresource tiles. If subresource tiles are part of packed mipmaps, <b>GetResourceTiling</b> sets the members of D3D12_SUBRESOURCE_TILING to zeros, except the <i>StartTileIndexInOverallResource</i> member, which <b>GetResourceTiling</b> sets to D3D12_PACKED_TILE (0xffffffff). The D3D12_PACKED_TILE constant indicates that the whole <b>D3D12_SUBRESOURCE_TILING</b> structure is meaningless for this situation, and the info that the <i>pPackedMipDesc</i> parameter points to applies.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>To estimate the total resource size of textures needed when calculating heap sizes and calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createplacedresource">CreatePlacedResource</a>, use <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo">GetResourceAllocationInfo</a> instead of <b>GetResourceTiling</b>. <b>GetResourceTiling</b> cannot be used for this.</para>
	/// <para>For more information on tiled resources, refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/volume-tiled-resources">Volume Tiled Resources</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetResourceTiling( in  IResource         pTiledResource,
							out uint              pNumTilesForEntireResource,
							out PackedMipInfo     pPackedMipDesc,
							out TileShape         pStandardTileShapeForNonPackedMips,
							ref uint              pNumSubresourceTilings,
							uint                  FirstSubresourceTilingToGet,
							ref SubresourceTiling pSubresourceTilingsForNonPackedMips ) ;
	
	
	
	/// <summary>Gets a locally unique identifier for the current device (adapter).</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">LUID</a></b> The locally unique identifier for the adapter.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method returns a unique identifier for the adapter that is specific to the adapter hardware. Applications can use this identifier to define robust mappings across various APIs (Direct3D 12, DXGI).</para>
	/// <para>A locally unique identifier (LUID) is a 64-bit value that is guaranteed to be unique only on the system on which it was generated. The uniqueness of a locally unique identifier (LUID) is guaranteed only until the system is restarted.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getadapterluid#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	Luid GetAdapterLuid( ) ;
	
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12Device) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable. Instantiate( ) => new Device( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Device( (ID3D12Device)pComObj! ) ;
	// ==================================================================================
} ;


/// <summary>
/// Represents a virtual adapter, and expands on the range of methods
/// provided by <see cref="IDevice"/>.
/// </summary>
[ProxyFor( typeof( ID3D12Device1 ) )]
public interface IDevice1: IDevice {

	/// <summary>Creates a cached pipeline library.</summary>
	/// <param name="pLibraryBlob">
	/// <para>Type: [in] **const void\*** If the input library blob is empty, then the initial content of the library is empty. If the input library blob is not empty, then it is validated for integrity, parsed, and the pointer is stored. The pointer provided as input to this method must remain valid for the lifetime of the object returned. For efficiency reasons, the data is not copied.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="BlobLength">
	/// <para>Type: **[SIZE_T](/windows/win32/winprog/windows-data-types)** Specifies the length of *pLibraryBlob* in bytes.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** Specifies a unique REFIID for the [ID3D12PipelineLibrary](./nn-d3d12-id3d12pipelinelibrary.md) object.
	/// Typically set this and the following parameter with the macro `IID_PPV_ARGS`, where **Library** is the name of the object.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="ppPipelineLibrary">
	/// <para>Type: [out] **void\*\*** Returns a pointer to the created library.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/win32/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10), including **E_INVALIDARG** if the blob is corrupted or unrecognized, **D3D12_ERROR_DRIVER_VERSION_MISMATCH** if the provided data came from an old driver or runtime, and **D3D12_ERROR_ADAPTER_NOT_FOUND** if the data came from different hardware. If you pass `nullptr` for *pPipelineLibrary* then the runtime still performs the validation of the blob but avoid creating the actual library and returns S_FALSE if the library would have been created. Also, the feature requires an updated driver, and attempting to use it on old drivers will return DXGI_ERROR_UNSUPPORTED.</para>
	/// </returns>
	/// <remarks>
	/// <para>A pipeline library enables the following operations. - Adding pipeline state objects (PSOs) to an existing library object (refer to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-storepipeline">StorePipeline</a>). - Serializing a PSO library into a contiguous block of memory for disk storage (refer to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-serialize">Serialize</a>). - De-serializing a PSO library from persistent storage (this is handled by <b>CreatePipelineLibrary</b>). - Retrieving individual PSOs from the library (refer to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-loadcomputepipeline">LoadComputePipeline</a> and <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinelibrary-loadgraphicspipeline">LoadGraphicsPipeline</a>). At no point in the lifecycle of a pipeline library is there duplication between PSOs with identical sub-components. A recommended solution for managing the lifetime of the provided pointer while only having to ref-count the returned interface is to leverage <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12object-setprivatedatainterface">ID3D12Object::SetPrivateDataInterface</a>, and use an object which implements <b>IUnknown</b>, and frees the memory when the ref-count reaches 0.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-createpipelinelibrary#">Read more on docs.microsoft.com </a>.</para>
	/// </remarks>
	void CreatePipelineLibrary( nint                 pLibraryBlob,
								nuint                BlobLength,
								in  Guid             riid,
								out IPipelineLibrary ppPipelineLibrary ) ;


	/// <summary>Specifies an event that should be fired when one or more of a collection of fences reach specific values.</summary>
	/// <param name="ppFences">
	/// <para>Type: <b>ID3D12Fence*</b> An array of length <i>NumFences</i> that specifies the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a> objects.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-seteventonmultiplefencecompletion#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="pFenceValues">
	/// <para>Type: <b>const UINT64*</b> An array of length <i>NumFences</i> that specifies the fence values required for the event is to be signaled.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-seteventonmultiplefencecompletion#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="NumFences">
	/// <para>Type: <b>UINT</b> Specifies the number of fences to be included.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-seteventonmultiplefencecompletion#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="Flags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_multiple_fence_wait_flags">D3D12_MULTIPLE_FENCE_WAIT_FLAGS</a></b> Specifies one  of the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_multiple_fence_wait_flags">D3D12_MULTIPLE_FENCE_WAIT_FLAGS</a> that determines how to proceed.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-seteventonmultiplefencecompletion#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="hEvent">
	/// <para>Type: <b>HANDLE</b> A handle to the event object.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-seteventonmultiplefencecompletion#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code.</para>
	/// </returns>
	/// <remarks>
	/// <para>To specify a single fence refer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12fence-seteventoncompletion">SetEventOnCompletion</a> method. If *hEvent* is a null handle, then this API will not return until the specified fence value(s) have been reached.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-seteventonmultiplefencecompletion#">Read more on docs.microsoft.com </a>.</para>
	/// </remarks>
	void SetEventOnMultipleFenceCompletion( IFence[]            ppFences,
											ulong[]             pFenceValues,
											uint                NumFences,
											MultiFenceWaitFlags Flags,
											Win32Handle         hEvent ) ;


	/// <summary>This method sets residency priorities of a specified list of objects.</summary>
	/// <param name="NumObjects">
	/// <para>Type: <b>UINT</b> Specifies the number of objects in the <i>ppObjects</i> and <i>pPriorities</i> arrays.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-setresidencypriority#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="ppObjects">
	/// <para>Type: <b>ID3D12Pageable*</b> Specifies an array, of length <i>NumObjects</i>, containing references to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a> objects.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-setresidencypriority#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="pPriorities">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_residency_priority">D3D12_RESIDENCY_PRIORITY</a>*</b> Specifies an array, of length <i>NumObjects</i>, of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_residency_priority">D3D12_RESIDENCY_PRIORITY</a> values for the list of objects.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-setresidencypriority#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code.</para>
	/// </returns>
	/// <remarks>For more information, refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/residency">Residency</a>.</remarks>
	void SetResidencyPriority( uint                      NumObjects,
							   IPageable[]               ppObjects,
							   Span< ResidencyPriority > pPriorities ) ;


	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( ID3D12Device1 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12Device1 ).GUID.ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable.Instantiate( ) => new Device1( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device1( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device1( (ID3D12Device1)pComObj! ) ;
	// ==================================================================================
} ;


/// <summary>
/// Represents a virtual adapter. This interface extends <see cref="IDevice1"/>
/// to create pipeline state objects from pipeline state stream descriptions.
/// </summary>
/// <remarks>
/// <b>Note:</b> This interface was introduced in Windows 10 Creators Update.
/// Applications targeting Windows 10 Creators Update should use this interface
/// instead of earlier or later versions. Applications targeting an earlier or
/// later version of Windows 10 should use the appropriate version of the
/// <see cref="IDevice"/> interface.
/// </remarks>
[ProxyFor( typeof( ID3D12Device2 ) )]
public interface IDevice2: IDevice1 {
	// ---------------------------------------------------------------------------------
	
	/// <summary>Creates a pipeline state object from a pipeline state stream description.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="../d3d12/ns-d3d12-d3d12_pipeline_state_stream_desc.md">D3D12_PIPELINE_STATE_STREAM_DESC</a>*</b> The address of a <a href="../d3d12/ns-d3d12-d3d12_pipeline_state_stream_desc.md">D3D12_PIPELINE_STATE_STREAM_DESC</a> structure that describes the pipeline state.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device2-createpipelinestate#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the pipeline state interface (<a href="../d3d12/nn-d3d12-id3d12pipelinestate.md">ID3D12PipelineState</a>). The <b>REFIID</b>, or <b>GUID</b>, of the interface to the pipeline state can be obtained by using the __uuidof() macro. For example, __uuidof(ID3D12PipelineState) will get the <b>GUID</b> of the interface to a pipeline state.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device2-createpipelinestate#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="ppPipelineState">
	/// <para>Type: <b>void**</b> <a href="https://docs.microsoft.com/cpp/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_COM_Outptr_</c> A pointer to a memory block that receives a pointer to the <a href="../d3d12/nn-d3d12-id3d12pipelinestate.md">ID3D12PipelineState</a> interface for the pipeline state object. The pipeline state object is an immutable state object. It contains no methods.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device2-createpipelinestate#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to create the pipeline state object. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>This function takes the pipeline description as a <a href="../d3d12/ns-d3d12-d3d12_pipeline_state_stream_desc.md">D3D12_PIPELINE_STATE_STREAM_DESC</a> and combines the functionality of the <a href="../d3d12/nf-d3d12-id3d12device-creategraphicspipelinestate.md">ID3D12Device::CreateGraphicsPipelineState</a> and <a href="../d3d12/nf-d3d12-id3d12device-createcomputepipelinestate.md">ID3D12Device::CreateComputePipelineState</a> functions, which take their pipeline description as the less-flexible <a href="../d3d12/ns-d3d12-d3d12_graphics_pipeline_state_desc.md">D3D12_GRAPHICS_PIPELINE_STATE_DESC</a> and <a href="../d3d12/ns-d3d12-d3d12_compute_pipeline_state_desc.md">D3D12_COMPUTE_PIPELINE_STATE_DESC</a> structs, respectively.</remarks>
	void CreatePipelineState( in PipelineStateStreamDescription pDesc,
							  in Guid riid,
							  out IPipelineState ppPipelineState ) ;

	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12Device2) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device2).GUID.ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Device2( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr pComObj ) => new Device2( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device2( (ID3D12Device2)pComObj! ) ;
	// ==================================================================================
}


/// <summary>
/// Represents a virtual adapter. This interface extends <see cref="IDevice2"/> to support
/// the creation of special-purpose diagnostic heaps in system memory that persist even in
/// the event of a GPU-fault or device-removed scenario.
/// </summary>
/// <remarks>
/// <b>Note:</b> This interface, introduced in the Windows 10 Fall Creators Update, is the
/// latest version of the ID3D12Device interface. Applications targeting the Windows 10
/// Fall Creators Update and later should use this interface instead of earlier versions.
/// </remarks>
[ProxyFor( typeof( ID3D12Device3 ) )]
public interface IDevice3: IDevice2 {
	
	/// <summary>
	/// Creates a special-purpose diagnostic heap in system memory from an address.
	/// The created heap can persist even in the event of a GPU-fault or device-removed scenario.
	/// </summary>
	/// <param name="pAddress">
	/// <para>The address used to create the heap.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-openexistingheapfromaddress#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>The globally unique identifier (<see cref="Guid"/>) for the heap interface
	/// (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12heap">ID3D12Heap</a>).
	/// The <b>REFIID</b>, or <b><see cref="Guid"/></b>, of the interface to the heap can be obtained by using the
	/// <b>__uuidof()</b> macro. For example, <b>__uuidof(ID3D12Heap)</b> will retrieve the <b>GUID</b> of the interface to a heap.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-openexistingheapfromaddress#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="ppvHeap">
	/// <para>A pointer to a memory block.
	/// On success, the D3D12 runtime will write a pointer to the newly-opened heap into the memory block.
	/// The type of the pointer depends on the provided <b>riid</b> parameter.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-openexistingheapfromaddress#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to open the existing heap.
	/// See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>The heap is created in system memory and permits CPU access. It wraps the entire VirtualAlloc region.
	/// Heaps can be used for placed and reserved resources, as orthogonally as other heaps.
	/// Restrictions may still exist based on the flags that cannot be app-chosen.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-openexistingheapfromaddress#">Read more on docs.microsoft.com </a>.</para>
	/// </remarks>
	void OpenExistingHeapFromAddress( nint pAddress, in Guid riid, out IHeap ppvHeap ) ;

	
	/// <summary>
	/// Creates a special-purpose diagnostic heap in system memory from a file mapping object.
	/// The created heap can persist even in the event of a GPU-fault or device-removed scenario.
	/// </summary>
	/// <param name="hFileMapping">
	/// <para>The handle to the file mapping object to use to create the heap.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-openexistingheapfromfilemapping#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>The globally unique identifier (<see cref="Guid"/>) for the heap interface
	/// (<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12heap">ID3D12Heap</a>).
	/// The <b>REFIID</b>, or <see cref="Guid"/>, of the interface to the heap can be obtained by using the **__uuidof()** macro.
	/// For example, **__uuidof(ID3D12Heap)** will retrieve the **GUID** of the interface to a heap.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-openexistingheapfromfilemapping#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="ppvHeap">
	/// <para>A pointer to a memory block. On success, the D3D12 runtime will write a pointer to the newly-opened heap into the memory block.
	/// The type of the pointer depends on the provided <i>riid</i> parameter.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-openexistingheapfromfilemapping#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <returns>
	/// <para>This method returns <b>E_OUTOFMEMORY</b> if there is insufficient memory to open the existing heap.
	/// See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>
	/// for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>The heap is created in system memory, and it permits CPU access. It wraps the entire VirtualAlloc region.
	/// Heaps can be used for placed and reserved resources, as orthogonally as other heaps.
	/// Restrictions may still exist based on the flags that cannot be app-chosen.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-openexistingheapfromfilemapping#">Read more on docs.microsoft.com </a>.</para>
	/// </remarks>
	void OpenExistingHeapFromFileMapping( Win32Handle hFileMapping, in Guid riid, out IHeap ppvHeap ) ;

	
	/// <summary>Asynchronously makes objects resident for the device.</summary>
	/// <param name="flags">
	/// <para><b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_residency_flags">D3D12_RESIDENCY_FLAGS</a></b>
	/// Controls whether the objects should be made resident if the application is over its memory budget.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-enqueuemakeresident#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="NumObjects">
	/// <para>The number of objects in the <i>ppObjects</i> array to make resident for the device.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-enqueuemakeresident#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="ppObjects">
	/// <para><b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>*</b>
	/// A pointer to a memory block; contains an array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>
	/// interface pointers for the objects.</para>
	/// <para>Even though most D3D12 objects inherit from <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>,
	/// residency changes are only supported on the following: </para>
	/// <para>This doc was truncated.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-enqueuemakeresident#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="pFenceToSignal">
	/// <para><b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a>*</b>
	/// A pointer to the fence used to signal when the work is done.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-enqueuemakeresident#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="FenceValueToSignal">
	/// <para>An unsigned 64-bit value signaled to the fence when the work is done.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-enqueuemakeresident#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b>
	/// This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>EnqueueMakeResident</b> performs the same actions as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-makeresident">MakeResident</a>,
	/// but does not wait for the resources to be made resident. Instead, <b>EnqueueMakeResident</b> signals a fence when the work is done.
	/// The system will not allow work that references the resources that are being made resident by using <b>EnqueueMakeResident</b> before
	/// its fence is signaled. Instead, calls to this API are guaranteed to signal their corresponding fence in order, so the same fence can
	/// be used from call to call.</para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device3-enqueuemakeresident#">Read more on docs.microsoft.com </a>.</para>
	/// </remarks>
	void EnqueueMakeResident( ResidencyFlags flags,
							  uint NumObjects,
							  IPageable[ ] ppObjects,
							  IFence pFenceToSignal,
							  ulong FenceValueToSignal ) ;

	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof( ID3D12Device3 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12Device3 ).GUID.ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Device3( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device3( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device3( (ID3D12Device3)pComObj! ) ;
	// ==================================================================================
} ;


/// <summary>Represents a virtual adapter.</summary>
/// <remarks>This interface extends <see cref="IDevice3"/>.</remarks>
[ProxyFor( typeof( ID3D12Device4 ) )]
public interface IDevice4: IDevice3 {
	
	/// <summary>Creates a command list in the closed state.</summary>
	/// <param name="nodeMask">
	/// <para>Type: **[UINT](/windows/win32/WinProg/windows-data-types)** For single-GPU operation, set this to zero. If there are multiple GPU nodes, then set a bit to identify the node (the device's physical adapter) for which to create the command list. Each bit in the mask corresponds to a single node. Only one bit must be set. Also see [Multi-adapter systems](/windows/win32/direct3d12/multi-engine).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommandlist1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="type">
	/// <para>Type: **[D3D12_COMMAND_LIST_TYPE](./ne-d3d12-d3d12_command_list_type.md)** Specifies the type of command list to create.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommandlist1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="flags">
	/// <para>Type: **[D3D12_COMMAND_LIST_FLAGS](./ne-d3d12-d3d12_command_list_flags.md)** Specifies creation flags.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommandlist1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the command list interface to return in *ppCommandList*.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommandlist1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppCommandList">
	/// <para>Type: **void\*\*** A pointer to a memory block that receives a pointer to the [ID3D12CommandList](./nn-d3d12-id3d12commandlist.md) or [ID3D12GraphicsCommandList](./nn-d3d12-id3d12graphicscommandlist.md) interface for the command list.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommandlist1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the command list.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks></remarks>
	unsafe void CreateCommandList1( uint nodeMask,
									CommandListType type, 
									CommandListFlags flags,
									in Guid riid, 
									out ICommandList ppCommandList ) ;

	
	/// <summary>Creates an object that represents a session for content protection.</summary>
	/// <param name="pDesc">
	/// <para>Type: **const [D3D12_PROTECTED_RESOURCE_SESSION_DESC](./ns-d3d12-d3d12_protected_resource_session_desc.md)\*** A pointer to a constant **D3D12_PROTECTED_RESOURCE_SESSION_DESC** structure, describing the session to create.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createprotectedresourcesession#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the [ID3D12ProtectedResourceSession](./nn-d3d12-id3d12protectedresourcesession.md) interface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createprotectedresourcesession#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppSession">
	/// <para>Type: **void\*\*** A pointer to a memory block that receives an [ID3D12ProtectedResourceSession](./nn-d3d12-id3d12protectedresourcesession.md) interface pointer to the created session object.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createprotectedresourcesession#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns></returns>
	/// <remarks></remarks>
	void CreateProtectedResourceSession( in ProtectedResourceSessionDescription pDesc,
										 in Guid riid,
										 out IProtectedResourceSession ppSession ) ;

	
	/// <summary>Creates both a resource and an implicit heap (optionally for a protected session), such that the heap is big enough to contain the entire resource, and the resource is mapped to the heap. (ID3D12Device4::CreateCommittedResource1)</summary>
	/// <param name="pHeapProperties">
	/// <para>Type: **const [D3D12_HEAP_PROPERTIES](./ns-d3d12-d3d12_heap_properties.md)\*** A pointer to a **D3D12_HEAP_PROPERTIES** structure that provides properties for the resource's heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommittedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="HeapFlags">
	/// <para>Type: **[D3D12_HEAP_FLAGS](./ne-d3d12-d3d12_heap_flags.md)** Heap options, as a bitwise-OR'd combination of **D3D12_HEAP_FLAGS** enumeration constants.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommittedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: **const [D3D12_RESOURCE_DESC](./ns-d3d12-d3d12_resource_desc.md)\*** A pointer to a **D3D12_RESOURCE_DESC** structure that describes the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommittedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="InitialResourceState">
	/// <para>Type: **[D3D12_RESOURCE_STATES](./ne-d3d12-d3d12_resource_states.md)** The initial state of the resource, as a bitwise-OR'd combination of **D3D12_RESOURCE_STATES** enumeration constants. When you create a resource together with a [D3D12_HEAP_TYPE_UPLOAD](./ne-d3d12-d3d12_heap_type.md) heap, you must set *InitialResourceState* to [D3D12_RESOURCE_STATE_GENERIC_READ](./ne-d3d12-d3d12_resource_states.md). When you create a resource together with a [D3D12_HEAP_TYPE_READBACK](./ne-d3d12-d3d12_heap_type.md) heap, you must set *InitialResourceState* to [D3D12_RESOURCE_STATE_COPY_DEST](./ne-d3d12-d3d12_resource_states.md).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommittedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: **const [D3D12_CLEAR_VALUE](./ns-d3d12-d3d12_clear_value.md)\*** Specifies a **D3D12_CLEAR_VALUE** structure that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the [D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET](./ne-d3d12-d3d12_resource_flags.md) or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, you should choose the value with which the clear operation will most commonly be called. You can call the clear operation with other values, but those operations won't be as efficient as when the value matches the one passed in to resource creation. When you use [D3D12_RESOURCE_DIMENSION_BUFFER](./ne-d3d12-d3d12_resource_dimension.md), you must set *pOptimizedClearValue* to `nullptr`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommittedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pProtectedSession">
	/// <para>Type: **[ID3D12ProtectedResourceSession](./nn-d3d12-id3d12protectedresourcesession.md)\*** An optional pointer to an object that represents a session for content protection. If provided, this session indicates that the resource should be protected. You can obtain an **ID3D12ProtectedResourceSession** by calling [ID3D12Device4::CreateProtectedResourceSession](./nf-d3d12-id3d12device4-createprotectedresourcesession.md).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommittedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riidResource">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the resource interface to return in *ppvResource*. While *riidResource* is most commonly the **GUID** of [ID3D12Resource](./nn-d3d12-id3d12resource.md), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommittedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created resource object. *ppvResource* can be `nullptr`, to enable capability testing. When *ppvResource* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommittedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the resource.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method creates both a resource and a heap, such that the heap is big enough to contain the entire resource, and the resource is mapped to the heap. The created heap is known as an implicit heap, because the heap object can't be obtained by the application. Before releasing the final reference on the resource, your application must ensure that the GPU will no longer read nor write to this resource. The implicit heap is made resident for GPU access before the method returns control to your application. Also see [Residency](/windows/win32/direct3d12/residency). The resource GPU VA mapping can't be changed. See [ID3D12CommandQueue::UpdateTileMappings](./nf-d3d12-id3d12commandqueue-updatetilemappings.md) and [Volume tiled resources](/windows/win32/direct3d12/volume-tiled-resources). This method may be called by multiple threads concurrently.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createcommittedresource1#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateCommittedResource1( in HeapProperties pHeapProperties,
								   HeapFlags HeapFlags,
								   in ResourceDescription pDesc,
								   ResourceStates InitialResourceState,
								   [Optional] in ClearValue? pOptimizedClearValue,
								   IProtectedResourceSession pProtectedSession,
								   in Guid riidResource,
								   out IResource ppvResource ) ;

	
	/// <summary>Creates a heap (optionally for a protected session) that can be used with placed resources and reserved resources.</summary>
	/// <param name="pDesc">
	/// <para>Type: **const [D3D12_HEAP_DESC](./ns-d3d12-d3d12_heap_desc.md)\*** A pointer to a constant **D3D12_HEAP_DESC** structure that describes the heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createheap1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pProtectedSession">
	/// <para>Type: **[ID3D12ProtectedResourceSession](./nn-d3d12-id3d12protectedresourcesession.md)\*** An optional pointer to an object that represents a session for content protection. If provided, this session indicates that the heap should be protected. You can obtain an **ID3D12ProtectedResourceSession** by calling [ID3D12Device4::CreateProtectedResourceSession](./nf-d3d12-id3d12device4-createprotectedresourcesession.md). A heap with a protected session can't be created with the [D3D12_HEAP_FLAG_SHARED_CROSS_ADAPTER](/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_flags) flag.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createheap1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the heap interface to return in *ppvHeap*. While *riidResource* is most commonly the **GUID** of [ID3D12Heap](./nn-d3d12-id3d12heap.md), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createheap1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvHeap">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created heap object. *ppvHeap* can be `nullptr`, to enable capability testing. When *ppvHeap* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createheap1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the heap.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>**CreateHeap1** creates a heap that can be used with placed resources and reserved resources. Before releasing the final reference on the heap, your application must ensure that the GPU will no longer read or write to this heap. A placed resource object holds a reference on the heap it is created on; but a reserved resource doesn't hold a reference for each mapping made to a heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createheap1#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateHeap1( in HeapDescription pDesc,
					  IProtectedResourceSession pProtectedSession,
					  in Guid riid,
					  out IHeap ppvHeap ) ;
	

	/// <summary>Creates a resource (optionally for a protected session) that is reserved, and not yet mapped to any pages in a heap.</summary>
	/// <param name="pDesc">
	/// <para>A pointer to a <see cref="ResourceDescription"/> structure that describes the resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createreservedresource1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="InitialState">
	/// <para>The initial state of the resource, as a bitwise-OR'd combination of **D3D12_RESOURCE_STATES** enumeration constants.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createreservedresource1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pOptimizedClearValue">
	/// <para>Specifies a <see cref="ClearValue"/> structure that describes the default value for a clear color.
	/// <i>pOptimizedClearValue</i> specifies a value for which clear operations are most optimal.
	/// When the created resource is a texture with either the <see cref="ResourceFlags.AllowRenderTarget"/> or
	/// <see cref="ResourceFlags.AllowDepthStencil"/> flags, you should choose the value with which the clear operation will most commonly be called.
	/// You can call the clear operation with other values, but those operations won't be as efficient as when the value matches the one passed in
	/// to resource creation. When you use <see cref="ResourceDimension.Buffer"/>, you must set <i>pOptimizedClearValue</i> to `nullptr`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createreservedresource1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pProtectedSession">
	/// <para>An optional pointer to an object that represents a session for content protection.
	/// If provided, this session indicates that the resource should be protected.
	/// You can obtain an **ID3D12ProtectedResourceSession** by calling <see cref="IDevice4.CreateProtectedResourceSession"/>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createreservedresource1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>A reference to the globally unique identifier (<see cref="Guid"/>) of the resource interface to return in <i>ppvResource</i>.
	/// (See Remarks). While *riidResource* is most commonly the <see cref="Guid"/> of <see cref="IResource"/>, it may be the <see cref="Guid"/>
	/// of any interface. If the resource object doesn't support the interface for this <see cref="Guid"/>, then creation fails with <b>E_NOINTERFACE</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createreservedresource1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>An optional pointer to a memory block that receives the requested interface pointer to the created resource object.
	/// <i>ppvResource</i> can be `nullptr`, to enable capability testing. When <i>ppvResource</i> is `nullptr`,
	/// no object is created, and <b>S_FALSE</b> is returned when <i>pDesc</i> is valid.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createreservedresource1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>If the function succeeds, it returns <b>S_OK</b>.
	/// Otherwise, it returns an <see cref="HResult"/> [error code]:<para/>
	/// <b>E_OUTOFMEMORY</b> Returned when there is insufficient memory to create the resource.|</para>
	/// </returns>
	/// <remarks>
	/// <para>**CreateReservedResource** is equivalent to <b>D3D11_RESOURCE_MISC_TILED</b> in Direct3D 11.
	/// It creates a resource with virtual memory only, no backing store.
	/// You need to map the resource to physical memory (that is, to a heap) using
	/// <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-copytilemappings">CopyTileMappings</a>
	/// and <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-updatetilemappings">UpdateTileMappings</a>.
	/// These resource types can only be created when the adapter supports tiled resource tier 1 or greater.
	/// The tiled resource tier defines the behavior of accessing a resource that is not mapped to a heap.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createreservedresource1#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateReservedResource1( in ResourceDescription pDesc,
								  ResourceStates InitialState,
								  [Optional] in ClearValue? pOptimizedClearValue,
								  IProtectedResourceSession pProtectedSession,
								  in Guid riid,
								  out IResource ppvResource ) ;

	
	/// <summary>
	/// Gets rich info about the size and alignment of memory required for a collection of resources on this adapter.
	/// </summary>
	/// <param name="visibleMask">
	/// <para>For single-GPU operation, set this to zero. If there are multiple GPU nodes, then set bits to identify the nodes (the device's physical adapters). Each bit in the mask corresponds to a single node. Also see [Multi-adapter systems](/windows/win32/direct3d12/multi-engine).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-getresourceallocationinfo1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="numResourceDescs">
	/// <para>The number of resource descriptors in the *pResourceDescs* array. This is also the size (the number of elements in) *pResourceAllocationInfo1*.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-getresourceallocationinfo1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pResourceDescs">
	/// <para>Type: **const [D3D12_RESOURCE_DESC](./ns-d3d12-d3d12_resource_desc.md)\*** An array of **D3D12_RESOURCE_DESC** structures that described the resources to get info about.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-getresourceallocationinfo1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pResourceAllocationInfo1">
	/// <para>Type: **[D3D12_RESOURCE_ALLOCATION_INFO1](./ns-d3d12-d3d12_resource_allocation_info1.md)\*** An array of [D3D12_RESOURCE_ALLOCATION_INFO1](./ns-d3d12-d3d12_resource_allocation_info1.md) structures, containing additional details for each resource description passed as input. This makes it simpler for your application to allocate a heap for multiple resources, and without manually computing offsets for where each resource should be placed.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-getresourceallocationinfo1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[D3D12_RESOURCE_ALLOCATION_INFO](./ns-d3d12-d3d12_resource_allocation_info.md)** A [D3D12_RESOURCE_ALLOCATION_INFO](./ns-d3d12-d3d12_resource_allocation_info.md) structure that provides info about video memory allocated for the specified array of resources.</para>
	/// </returns>
	/// <remarks>
	/// <para>When you're using [CreatePlacedResource](./nf-d3d12-id3d12device-createplacedresource.md), your application must use **GetResourceAllocationInfo** in order to understand the size and alignment characteristics of texture resources. The results of this method vary depending on the particular adapter, and must be treated as unique to this adapter and driver version. Your application can't use the output of **GetResourceAllocationInfo** to understand packed mip properties of textures. To understand packed mip properties of textures, your application must use [GetResourceTiling](./nf-d3d12-id3d12device-getresourcetiling.md). Texture resource sizes significantly differ from the information returned by **GetResourceTiling**, because some adapter architectures allocate extra memory for textures to reduce the effective bandwidth during common rendering scenarios. This even includes textures that have constraints on their texture layouts, or have standardized texture layouts. That extra memory can't be sparsely mapped nor remapped by an application using [CreateReservedResource](./nf-d3d12-id3d12device-createreservedresource.md) and [UpdateTileMappings](./nf-d3d12-id3d12commandqueue-updatetilemappings.md), so it isn't reported by **GetResourceTiling**. Your application can forgo using **GetResourceAllocationInfo** for buffer resources ([D3D12_RESOURCE_DIMENSION_BUFFER](./ne-d3d12-d3d12_resource_dimension.md)). Buffers have the same size on all adapters, which is merely the smallest multiple of 64KB that's greater or equal to [D3D12_RESOURCE_DESC::Width](./ns-d3d12-d3d12_resource_desc.md). When multiple resource descriptions are passed in, the C++ algorithm for calculating a structure size and alignment are used. For example, a three-element array with two tiny 64KB-aligned resources and a tiny 4MB-aligned resource, reports differing sizes based on the order of the array. If the 4MB aligned resource is in the middle, then the resulting **Size** is 12MB. Otherwise, the resulting **Size** is 8MB. The **Alignment** returned would always be 4MB, because it's the superset of all alignments in the resource array.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device4-getresourceallocationinfo1#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	ResourceAllocationInfo GetResourceAllocationInfo1( uint visibleMask,
													   uint numResourceDescs,
													   in Span< ResourceDescription > pResourceDescs,
													   [Optional] in Span< ResourceAllocationInfo1 > pResourceAllocationInfo1 ) ;

	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device4) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device4).GUID.ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable.Instantiate( ) => new Device4( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr pComObj ) => new Device4( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device4( (ID3D12Device4)pComObj! ) ;
	// ==================================================================================
}


/// <summary>Represents a virtual adapter.</summary>
/// <remarks>
/// This interface extends <see cref="IDevice4"/>.<para/>
/// <b>NOTE:</b> This interface, introduced in Windows 10, version 1809, is the latest version of the
/// <see cref="IDevice"/> interface. Applications targeting Windows 10, version 1809 and later should
/// use this interface instead of earlier versions.<para/>
/// (Requires Windows 10, version 10.0.17763 or later.)
/// </remarks>
[SupportedOSPlatform( "windows10.0.17763" )]
[ProxyFor( typeof( ID3D12Device5 ) )]
public interface IDevice5: IDevice4 {

	/// <summary>Creates a lifetime tracker associated with an application-defined callback; the callback receives notifications when the lifetime of a tracked object is changed.</summary>
	/// <param name="pOwner">
	/// <para>A pointer to an **ID3D12LifetimeOwner** interface representing the application-defined callback.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createlifetimetracker#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>A reference to the interface identifier (IID) of the interface to return in *ppvTracker*.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createlifetimetracker#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvTracker">
	/// <para>A pointer to a memory block that receives the requested interface pointer to the created object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createlifetimetracker#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	void CreateLifetimeTracker( ILifetimeOwner pOwner, in Guid riid, out ILifetimeTracker ppvTracker ) ;

	/// <summary>You can call **RemoveDevice** to indicate to the Direct3D 12 runtime that the GPU device encountered a problem, and can no longer be used.</summary>
	/// <remarks>
	/// <para>Because device removal triggers all fences to be signaled to `UINT64_MAX`, you can create a callback for device removal using an event. </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-removedevice#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void RemoveDevice( ) ;

	/// <summary>Queries reflection metadata about available meta commands.</summary>
	/// <param name="pNumMetaCommands">
	/// <para>Type: [in, out] <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a> containing the number of meta commands to query for. This field determines the size of the <i>pDescs</i> array, unless <i>pDescs</i> is <b>nullptr</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-enumeratemetacommands#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDescs">
	/// <para>Type: [out, optional] **[D3D12_META_COMMAND_DESC](./ns-d3d12-d3d12_meta_command_desc.md)\*** An optional pointer to an array of [D3D12_META_COMMAND_DESC](./ns-d3d12-d3d12_meta_command_desc.md) containing the descriptions of the available meta commands. Pass `nullptr` to have the number of available meta commands returned in <i>pNumMetaCommands</i>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-enumeratemetacommands#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If this method succeeds, it returns <b>S_OK</b>. Otherwise, it returns an [HRESULT](/windows/win32/com/structure-of-com-error-codes) error code.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-enumeratemetacommands">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void EnumerateMetaCommands( ref uint pNumMetaCommands, out Span< MetaCommandDescription > pDescs ) ;


	/// <summary>Queries reflection metadata about the parameters of the specified meta command.</summary>
	/// <param name="commandId">
	/// <para>Type: <b>REFIID</b> A reference to the globally unique identifier (GUID) of the meta command whose parameters you wish to be returned in <i>pParameterDescs</i>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-enumeratemetacommandparameters#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Stage">
	/// <para>Type: <b>D3D12_META_COMMAND_PARAMETER_STAGE</b> A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_meta_command_parameter_stage">D3D12_META_COMMAND_PARAMETER_STAGE</a> specifying the stage of the parameters that you wish to be included in the query.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-enumeratemetacommandparameters#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pTotalStructureSizeInBytes">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> An optional pointer to a <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a> containing the size of the structure containing the parameter values, which you pass when creating/initializing/executing the meta command, as appropriate.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-enumeratemetacommandparameters#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pParameterCount">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a> containing the number of parameters to query for. This field determines the size of the <i>pParameterDescs</i> array, unless <i>pParameterDescs</i> is <b>nullptr</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-enumeratemetacommandparameters#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pParameterDescs">
	/// <para>Type: <b>D3D12_META_COMMAND_PARAMETER_DESC*</b> An optional pointer to an array of  <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_parameter_desc">D3D12_META_COMMAND_PARAMETER_DESC</a> containing the descriptions of the parameters. Pass <b>nullptr</b> to have the parameter count returned in <i>pParameterCount</i>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-enumeratemetacommandparameters#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b>HRESULT</b> If this method succeeds, it returns <b>S_OK</b>. Otherwise, it returns an <b>HRESULT</b> error code.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-enumeratemetacommandparameters">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void EnumerateMetaCommandParameters( in Guid                                     commandId,
										 MetaCommandParameterStage                   Stage,
										 out uint                                    pTotalStructureSizeInBytes,
										 ref uint                                    pParameterCount,
										 out Span< MetaCommandParameterDescription > pParameterDescs ) ;


	/// <summary>Creates an instance of the specified meta command.</summary>
	/// <param name="commandId">
	/// <para>Type: <b>REFIID</b> A reference to the globally unique identifier (GUID) of the meta command that you wish to instantiate.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createmetacommand#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="nodeMask">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT</a></b> For single-adapter operation, set this to zero. If there are multiple adapter nodes, set a bit to identify the node (one of the device's physical adapters) to which the meta command applies. Each bit in the mask corresponds to a single node. Only one bit must be set. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createmetacommand#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pCreationParametersData">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">void</a>*</b> An optional pointer to a constant structure containing the values of the parameters for creating the meta command.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createmetacommand#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="CreationParametersDataSizeInBytes">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">SIZE_T</a></b> A <a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">SIZE_T</a> containing the size of the structure pointed to by <i>pCreationParametersData</i>, if set, otherwise 0.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createmetacommand#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> A reference to the globally unique identifier (GUID) of the interface that you wish to be returned in <i>ppMetaCommand</i>. This is expected to be the GUID of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12metacommand">ID3D12MetaCommand</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createmetacommand#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppMetaCommand">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the meta command. This is the address of a pointer to an <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12metacommand">ID3D12MetaCommand</a>, representing  the meta command created.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createmetacommand#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b>HRESULT</b> If this method succeeds, it returns <b>S_OK</b>. Otherwise, it returns an <b>HRESULT</b> error code. </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createmetacommand">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateMetaCommand( in Guid          commandId,
							uint             nodeMask,
							[Optional] nint  pCreationParametersData,
							nuint            CreationParametersDataSizeInBytes,
							in  Guid         riid,
							out IMetaCommand ppMetaCommand ) ;


	/// <summary>Creates an [ID3D12StateObject](/windows/win32/api/d3d12/nn-d3d12-id3d12stateobject).</summary>
	/// <param name="pDesc">The description of the state object to create.</param>
	/// <param name="riid">The GUID of the interface to create. Use <i>__uuidof(ID3D12StateObject)</i>.</param>
	/// <param name="ppStateObject">The returned state object.</param>
	/// <returns>
	/// <para>Returns S_OK if successful; otherwise, returns one of the following values: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-createstateobject">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateStateObject( in StateObjectDescription pDesc, in Guid riid, out IStateObject ppStateObject ) ;


	/// <summary>Query the driver for resource requirements to build an acceleration structure.</summary>
	/// <param name="pDesc">
	/// <para>Description of the acceleration structure build. This structure is shared with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-buildraytracingaccelerationstructure">BuildRaytracingAccelerationStructure</a>.  For more information, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_build_raytracing_acceleration_structure_inputs">D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_INPUTS</a>. The implementation is allowed to look at all the CPU parameters in this struct and nested structs.  It may not inspect/dereference any GPU virtual addresses, other than to check to see if a pointer is NULL or not, such as the optional transform in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_raytracing_geometry_triangles_desc">D3D12_RAYTRACING_GEOMETRY_TRIANGLES_DESC</a>, without dereferencing it. In other words, the calculation of resource requirements for the acceleration structure does not depend on the actual geometry data (such as vertex positions), rather it can only depend on overall properties, such as the number of triangles, number of instances etc.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-getraytracingaccelerationstructureprebuildinfo#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pInfo">The result of the query.</param>
	/// <remarks>
	/// <para>The input acceleration structure description is the same as what goes into <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-buildraytracingaccelerationstructure">BuildRaytracingAccelerationStructure</a>. The result of this function lets the application provide the correct amount of output storage and scratch storage to <b>BuildRaytracingAccelerationStructure</b> given the same geometry. Builds can also be done with the same configuration passed to <b>GetAccelerationStructurePrebuildInfo</b> overall except equal or smaller counts for the number of geometries/instances or the  number of vertices/indices/AABBs in any given geometry.  In this case the storage requirements reported with the original sizes passed to <b>GetRaytracingAccelerationStructurePrebuildInfo</b> will be valid – the build may actually consume less space but not more.  This is handy for app scenarios where having conservatively large storage allocated for acceleration structures is fine. This method is on the device interface as opposed to command list on the assumption that drivers must be able to calculate resource requirements for an acceleration structure build from only looking at the CPU-visible portions of the call, without having to dereference any pointers to GPU memory containing actual vertex data, index data, etc.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-getraytracingaccelerationstructureprebuildinfo#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetRaytracingAccelerationStructurePrebuildInfo( in  BuildRaytracingAccelerationStructureInputs  pDesc,
														 out RaytracingAccelerationStructurePreBuildInfo pInfo ) ;

	/// <summary>Reports the compatibility of serialized data, such as a serialized raytracing acceleration structure resulting from a call to CopyRaytracingAccelerationStructure with mode D3D12_RAYTRACING_ACCELERATION_STRUCTURE_COPY_MODE_SERIALIZE, with the current device/driver.</summary>
	/// <param name="SerializedDataType">The type of the serialized data. For more information, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_serialized_data_type">D3D12_SERIALIZED_DATA_TYPE</a>.</param>
	/// <param name="pIdentifierToCheck">Identifier from the header of the serialized data to check with the driver. For more information, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_serialized_data_driver_matching_identifier">D3D12_SERIALIZED_DATA_DRIVER_MATCHING_IDENTIFIER</a>.</param>
	/// <returns>The returned compatibility status. For more information, see <a href="../d3d12/ne-d3d12-d3d12_driver_matching_identifier_status.md">D3D12_DRIVER_MATCHING_IDENTIFIER_STATUS</a>.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device5-checkdrivermatchingidentifier">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	DriverMatchingIdentifierStatus CheckDriverMatchingIdentifier( SerializedDataType SerializedDataType,
																  in SerializedDataDriverMatchingIdentifier
																	  pIdentifierToCheck ) ;


	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof( ID3D12Device5 ) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12Device5 ).GUID.ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable.Instantiate( ) => new Device5( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device5( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device5( (ID3D12Device5)pComObj! ) ;
	// ==================================================================================
} ;


/// <summary>
/// Represents a virtual adapter and extends the
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/nn-d3d12-id3d12device5">ID3D12Device5</a> interface.
/// </summary>
/// <remarks>Defines the <see cref="SetBackgroundProcessingMode"/> method.</remarks>
[ProxyFor( typeof( ID3D12Device6 ) )]
public interface IDevice6: IDevice5 {
	
	/// <summary>Sets the mode for driver background processing optimizations.</summary>
	/// <param name="Mode">
	/// <para>Type: **[D3D12_BACKGROUND_PROCESSING_MODE](./ne-d3d12-d3d12_background_processing_mode.md)** The level of dynamic optimization to apply to GPU work that's subsequently submitted.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device6-setbackgroundprocessingmode#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="MeasurementsAction">
	/// <para>Type: **[D3D12_MEASUREMENTS_ACTION](./ne-d3d12-d3d12_measurements_action.md)** The action to take with the results of earlier workload instrumentation.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device6-setbackgroundprocessingmode#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="hEventToSignalUponCompletion">
	/// <para>Type: **[HANDLE](/windows/win32/winprog/windows-data-types)** An optional handle to signal when the function is complete. For example, if *MeasurementsAction* is set to [D3D12_MEASUREMENTS_ACTION_COMMIT_RESULTS](./ne-d3d12-d3d12_measurements_action.md), then *hEventToSignalUponCompletion* is signaled when all resulting compilations have finished.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device6-setbackgroundprocessingmode#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pbFurtherMeasurementsDesired">
	/// <para>Type: **[BOOL](/windows/win32/winprog/windows-data-types)\*** An optional pointer to a Boolean value. The function sets the value to `true` to indicate that you should continue profiling, otherwise, `false`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device6-setbackgroundprocessingmode#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns></returns>
	/// <remarks>
	/// <para>A graphics driver can use idle-priority background CPU threads to dynamically recompile shader programs. That can improve GPU performance by specializing shader code to better match details of the hardware that it's running on, and/or the context in which it's being used. As a developer, you don't have to do anything to benefit from this feature (over time, as drivers adopt background processing optimizations, existing shaders will automatically be tuned more efficiently). But, when you're profiling your code, you'll probably want to call **SetBackgroundProcessingMode** to make sure that any driver background processing optimizations have taken place before you take timing measurements. Here's an example. </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device6-setbackgroundprocessingmode#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetBackgroundProcessingMode( BackgroundProcessingMode Mode, 
									  MeasurementsAction       MeasurementsAction,
									  Win32Handle              hEventToSignalUponCompletion,
									  ref bool                 pbFurtherMeasurementsDesired ) ;
	
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device6) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device6).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable.Instantiate( ) => new Device6( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device6( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device6( (ID3D12Device6)pComObj! ) ;
	// ==================================================================================
}


[ProxyFor( typeof( ID3D12Device7 ) )]
public interface IDevice7: IDevice6 {
	
	/// <summary>Incrementally add to an existing state object. This incurs lower CPU overhead than creating a state object from scratch that is a superset of an existing one.</summary>
	/// <param name="pAddition">
	/// <para>Type: \_In\_ **const [D3D12_STATE_OBJECT_DESC](./ns-d3d12-d3d12_state_object_desc.md)\*** Description of state object contents to add to existing state object. To help generate this see the **CD3D12_STATE_OBJECT_DESC** helper in class in `d3dx12.h`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device7-addtostateobject#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pStateObjectToGrowFrom">
	/// <para>Type: \_In\_ **[ID3D12StateObject](./nn-d3d12-id3d12stateobject.md)\*** Existing state object, which can be in use (for example, active raytracing) during this operation. The existing state object must not be of type **Collection**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device7-addtostateobject#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: \_In\_ **REFIID** Must be the IID of the [ID3D12StateObject](./nn-d3d12-id3d12stateobject.md) interface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device7-addtostateobject#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppNewStateObject">
	/// <para>Type: \_COM\_Outptr\_ **void\*\*** Returned state object. Behavior is undefined if shader identifiers are retrieved for new shaders from this call and they are accessed via shader tables by any already existing or in-flight command list that references some older state object. Use of the new shaders added to the state object can occur only from commands (such as **DispatchRays** or **ExecuteIndirect** calls) recorded in a command list after the call to **AddToStateObject**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device7-addtostateobject#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>**S_OK** for success. **E_INVALIDARG**, **E_OUTOFMEMORY** on failure. The debug layer provides detailed status information.</returns>
	/// <remarks>For more info, see [AddToStateObject](https://microsoft.github.io/DirectX-Specs/d3d/Raytracing.html#addtostateobject).</remarks>
	void AddToStateObject( in StateObjectDescription pAddition,
						   IStateObject              pStateObjectToGrowFrom,
						   in  Guid                  riid,
						   out IStateObject          ppNewStateObject ) ;
	
	/// <summary>Revises the [**ID3D12Device4::CreateProtectedResourceSession**](../d3d12/nf-d3d12-id3d12device4-createprotectedresourcesession.md) method with provision **GUID** that indicates the type of protected resource session.</summary>
	/// <param name="pDesc">
	/// <para>Type: \_In\_ **const [D3D12_PROTECTED_RESOURCE_SESSION_DESC1](./ns-d3d12-d3d12_protected_resource_session_desc1.md)\*** A pointer to a constant **D3D12_PROTECTED_RESOURCE_SESSION_DESC1** structure, describing the session to create.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device7-createprotectedresourcesession1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: \_In\_ **REFIID** The GUID of the interface to a protected session. Most commonly, [ID3D12ProtectedResourceSession1](./nn-d3d12-id3d12protectedresourcesession1.md), although it may be any **GUID** for any interface. If the protected session object doesn't support the interface for this **GUID**, the getter will return **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device7-createprotectedresourcesession1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppSession">
	/// <para>Type: \_COM\_Outptr\_ **void\*\*** A pointer to a memory block that receives a pointer to the session for the given protected session (the specific interface type returned depends on *riid*).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device7-createprotectedresourcesession1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	void CreateProtectedResourceSession1( in  ProtectedResourceSessionDescription1 pDesc,
										  in  Guid riid,
										  out IProtectedResourceSession1 ppSession ) ;

	
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device7) ;
	public new static Guid IID => ( ComType.GUID ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device7).GUID.ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable. Instantiate( ) => new Device7( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device7( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device7( (ID3D12Device7)pComObj! ) ;
	// ==================================================================================
}


[ProxyFor( typeof( ID3D12Device8 ) )]
public interface IDevice8: IDevice7 {

	/// <summary>Gets rich info about the size and alignment of memory required for a collection of resources on this adapter. (ID3D12Device8::GetResourceAllocationInfo2)</summary>
	/// <param name="visibleMask">
	/// <para>Type: **[UINT](/windows/win32/WinProg/windows-data-types)** For single-GPU operation, set this to zero. If there are multiple GPU nodes, then set bits to identify the nodes (the device's physical adapters). Each bit in the mask corresponds to a single node. Also see [Multi-adapter systems](/windows/win32/direct3d12/multi-engine).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getresourceallocationinfo2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="numResourceDescs">
	/// <para>Type: **[UINT](/windows/win32/WinProg/windows-data-types)** The number of resource descriptors in the *pResourceDescs* array. This is also the size (the number of elements in) *pResourceAllocationInfo1*.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getresourceallocationinfo2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pResourceDescs">
	/// <para>Type: **const [D3D12_RESOURCE_DESC1](./ns-d3d12-d3d12_resource_desc1.md)\*** An array of **D3D12_RESOURCE_DESC1** structures that described the resources to get info about.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getresourceallocationinfo2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pResourceAllocationInfo1">
	/// <para>Type: **[D3D12_RESOURCE_ALLOCATION_INFO1](./ns-d3d12-d3d12_resource_allocation_info1.md)\*** An array of [D3D12_RESOURCE_ALLOCATION_INFO1](./ns-d3d12-d3d12_resource_allocation_info1.md) structures, containing additional details for each resource description passed as input. This makes it simpler for your application to allocate a heap for multiple resources, and without manually computing offsets for where each resource should be placed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getresourceallocationinfo2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[D3D12_RESOURCE_ALLOCATION_INFO](./ns-d3d12-d3d12_resource_allocation_info.md)** A [D3D12_RESOURCE_ALLOCATION_INFO](./ns-d3d12-d3d12_resource_allocation_info.md) structure that provides info about video memory allocated for the specified array of resources.</para>
	/// </returns>
	/// <remarks>For remarks, see [ID3D12Device4::GetResourceAllocationInfo1](./nf-d3d12-id3d12device4-getresourceallocationinfo1.md).</remarks>
	ResourceAllocationInfo GetResourceAllocationInfo2( uint visibleMask, 
													   uint numResourceDescs, 
													   in Span< ResourceDescription1 > pResourceDescs,
													   [Optional] in Span< ResourceAllocationInfo1 > pResourceAllocationInfo1 ) ;

	
	/// <summary>Creates both a resource and an implicit heap (optionally for a protected session), such that the heap is big enough to contain the entire resource, and the resource is mapped to the heap.</summary>
	/// <param name="pHeapProperties">
	/// <para>Type: \_In\_ **const [D3D12_HEAP_PROPERTIES](/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_properties)\*** A pointer to a **D3D12_HEAP_PROPERTIES** structure that provides properties for the resource's heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createcommittedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="HeapFlags">
	/// <para>Type: **[D3D12_HEAP_FLAGS](/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_flags)** Heap options, as a bitwise-OR'd combination of **D3D12_HEAP_FLAGS** enumeration constants.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createcommittedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: **const [D3D12_RESOURCE_DESC1](/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_desc1)\*** A pointer to a **D3D12_RESOURCE_DESC1** structure that describes the resource, including a mip region.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createcommittedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="InitialResourceState">
	/// <para>Type: **[D3D12_RESOURCE_STATES](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states)** The initial state of the resource, as a bitwise-OR'd combination of **D3D12_RESOURCE_STATES** enumeration constants. When you create a resource together with a [D3D12_HEAP_TYPE_UPLOAD](/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type) heap, you must set *InitialResourceState* to [D3D12_RESOURCE_STATE_GENERIC_READ](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states). When you create a resource together with a [D3D12_HEAP_TYPE_READBACK](/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type) heap, you must set *InitialResourceState* to [D3D12_RESOURCE_STATE_COPY_DEST](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createcommittedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: **const [D3D12_CLEAR_VALUE](/windows/win32/api/d3d12/ns-d3d12-d3d12_clear_value)\*** Specifies a **D3D12_CLEAR_VALUE** structure that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the [D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags) or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, you should choose the value with which the clear operation will most commonly be called. You can call the clear operation with other values, but those operations won't be as efficient as when the value matches the one passed in to resource creation. When you use [D3D12_RESOURCE_DIMENSION_BUFFER](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_dimension), you must set *pOptimizedClearValue* to `nullptr`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createcommittedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pProtectedSession">
	/// <para>Type: **[ID3D12ProtectedResourceSession](/windows/win32/api/d3d12/nn-d3d12-id3d12protectedresourcesession)\*** An optional pointer to an object that represents a session for content protection. If provided, this session indicates that the resource should be protected. You can obtain an **ID3D12ProtectedResourceSession** by calling [ID3D12Device4::CreateProtectedResourceSession](/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createprotectedresourcesession).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createcommittedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riidResource">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the resource interface to return in *ppvResource*. While *riidResource* is most commonly the **GUID** of [ID3D12Resource](/windows/win32/api/d3d12/nn-d3d12-id3d12resource), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createcommittedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created resource object. *ppvResource* can be `nullptr`, to enable capability testing. When *ppvResource* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createcommittedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the resource.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method creates both a resource and a heap, such that the heap is big enough to contain the entire resource, and the resource is mapped to the heap. The created heap is known as an implicit heap, because the heap object can't be obtained by the application. Before releasing the final reference on the resource, your application must ensure that the GPU will no longer read nor write to this resource. The implicit heap is made resident for GPU access before the method returns control to your application. Also see [Residency](/windows/win32/direct3d12/residency). The resource GPU VA mapping can't be changed. See [ID3D12CommandQueue::UpdateTileMappings](/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-updatetilemappings) and [Volume tiled resources](/windows/win32/direct3d12/volume-tiled-resources). This method may be called by multiple threads concurrently.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createcommittedresource2#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateCommittedResource2( in HeapProperties pHeapProperties,
								   HeapFlags HeapFlags,
								   in ResourceDescription1 pDesc, 
								   ResourceStates InitialResourceState,
								   [Optional] in ClearValue? pOptimizedClearValue,
								   IProtectedResourceSession pProtectedSession,
								   in Guid riidResource,
								   out IResource ppvResource ) ;
	

	/// <summary>Creates a resource that is placed in a specific heap. Placed resources are the lightest weight resource objects available, and are the fastest to create and destroy.</summary>
	/// <param name="pHeap">
	/// <para>Type: [in] **<a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12heap">ID3D12Heap</a>*** A pointer to the **ID3D12Heap** interface that represents the heap in which the resource is placed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createplacedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="HeapOffset">
	/// <para>Type: **<a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a>** The offset, in bytes, to the resource. The *HeapOffset* must be a multiple of the resource's alignment, and *HeapOffset* plus the resource size must be smaller than or equal to the heap size. <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo">**GetResourceAllocationInfo**</a> must be used to understand the sizes of texture resources.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createplacedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: [in] **const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_desc1">D3D12_RESOURCE_DESC1</a>*** A pointer to a **D3D12_RESOURCE_DESC1** structure that describes the resource, including a mip region.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createplacedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="InitialState">
	/// <para>Type: **<a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATES</a>** The initial state of the resource, as a bitwise-OR'd combination of **D3D12_RESOURCE_STATES** enumeration constants. When a resource is created together with a **D3D12_HEAP_TYPE_UPLOAD** heap, *InitialState* must be **D3D12_RESOURCE_STATE_GENERIC_READ**. When a resource is created together with a **D3D12_HEAP_TYPE_READBACK** heap, *InitialState* must be **D3D12_RESOURCE_STATE_COPY_DEST**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createplacedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: [in, optional] **const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_clear_value">D3D12_CLEAR_VALUE</a>*** Specifies a **D3D12_CLEAR_VALUE** that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the **D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET** or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, your application should choose the value that the clear operation will most commonly be called with. Clear operations can be called with other values, but those operations will not be as efficient as when the value matches the one passed into resource creation. *pOptimizedClearValue* must be NULL when used with **D3D12_RESOURCE_DIMENSION_BUFFER**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createplacedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** The globally unique identifier (**GUID**) for the resource interface. This is an input parameter. The **REFIID**, or **GUID**, of the interface to the resource can be obtained by using the `__uuidof` macro. For example, `__uuidof(ID3D12Resource)` gets the **GUID** of the interface to a resource. Although **riid** is, most commonly, the GUID for <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">**ID3D12Resource**</a>, it may be any **GUID** for any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createplacedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: [out, optional] **void**** A pointer to a memory block that receives a pointer to the resource. *ppvResource* can be NULL, to enable capability testing. When *ppvResource* is NULL, no object will be created and S_FALSE will be returned when *pResourceDesc* and other parameters are valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createplacedresource1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **<a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a>** This method returns **E_OUTOFMEMORY** if there is insufficient memory to create the resource. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>See [ID3D12Device::CreatePlacedResource](./nf-d3d12-id3d12device-createplacedresource.md).</remarks>
	void CreatePlacedResource1( IHeap pHeap, 
								ulong HeapOffset,
								in ResourceDescription1 pDesc, 
								ResourceStates InitialState, 
								[Optional] in ClearValue? pOptimizedClearValue, 
								in Guid riid, 
								out IResource ppvResource ) ;


	/// <summary>For purposes of sampler feedback, creates a descriptor suitable for binding.</summary>
	/// <param name="pTargetedResource">
	/// <para>Type: \_In\_opt\_ **[ID3D12Resource](./nn-d3d12-id3d12heap.md)\*** The targeted resource, such as a texture, to create a descriptor for.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createsamplerfeedbackunorderedaccessview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pFeedbackResource">
	/// <para>Type: \_In\_opt\_ **[ID3D12Resource](./nn-d3d12-id3d12heap.md)\*** The feedback resource, such as a texture, to create a descriptor for.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createsamplerfeedbackunorderedaccessview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DestDescriptor">
	/// <para>Type: \_In\_ **[D3D12_CPU_DESCRIPTOR_HANDLE](./ns-d3d12-d3d12_cpu_descriptor_handle.md)** The CPU descriptor handle.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createsamplerfeedbackunorderedaccessview#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-createsamplerfeedbackunorderedaccessview">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateSamplerFeedbackUnorderedAccessView( IResource pTargetedResource,
												   IResource pFeedbackResource,
												   CPUDescriptorHandle DestDescriptor ) ;

	/// <summary>Gets a resource layout that can be copied. Helps your app fill in [D3D12_PLACED_SUBRESOURCE_FOOTPRINT](../d3d12/ns-d3d12-d3d12_placed_subresource_footprint.md) and [D3D12_SUBRESOURCE_FOOTPRINT](../d3d12/ns-d3d12-d3d12_subresource_footprint.md) when suballocating space in upload heaps.</summary>
	/// <param name="pResourceDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_desc1">D3D12_RESOURCE_DESC1</a>*</b> A description of the resource, as a pointer to a **D3D12_RESOURCE_DESC1** structure.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getcopyablefootprints1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="FirstSubresource">
	/// <para>Type: [in] <b>UINT</b> Index of the first subresource in the resource. The range of valid values is 0 to D3D12_REQ_SUBRESOURCES.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getcopyablefootprints1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="NumSubresources">
	/// <para>Type: [in] <b>UINT</b> The number of subresources in the resource. The range of valid values is 0 to (D3D12_REQ_SUBRESOURCES - <i>FirstSubresource</i>).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getcopyablefootprints1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="BaseOffset">
	/// <para>Type: <b>UINT64</b> The offset, in bytes, to the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getcopyablefootprints1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pLayouts">
	/// <para>Type: [out, optional] <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint">D3D12_PLACED_SUBRESOURCE_FOOTPRINT</a>*</b> A pointer to an array (of length <i>NumSubresources</i>) of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint">D3D12_PLACED_SUBRESOURCE_FOOTPRINT</a> structures, to be filled with the description and placement of each subresource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getcopyablefootprints1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pNumRows">
	/// <para>Type: [out, optional] <b>UINT*</b> A pointer to an array (of length <i>NumSubresources</i>) of integer  variables, to be filled with the number of rows for each subresource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getcopyablefootprints1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pRowSizeInBytes">
	/// <para>Type: [out, optional] <b>UINT64*</b> A pointer to an array (of length <i>NumSubresources</i>) of integer variables, each entry to be filled with the unpadded size in bytes of a row, of each subresource. For example, if a Texture2D resource has a width of 32 and bytes per pixel of 4, then <i>pRowSizeInBytes</i> returns 128. <i>pRowSizeInBytes</i> should not be confused with <b>row pitch</b>, as examining <i>pLayouts</i> and getting the row pitch from that will give you 256 as it is aligned to D3D12_TEXTURE_DATA_PITCH_ALIGNMENT.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getcopyablefootprints1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pTotalBytes">
	/// <para>Type: [out, optional] <b>UINT64*</b> A pointer to an integer variable, to be filled with the total size, in bytes.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device8-getcopyablefootprints1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>For remarks and examples, see [ID3D12Device::GetCopyableFootprints](./nf-d3d12-id3d12device-getcopyablefootprints.md).</remarks>
	void GetCopyableFootprints1( in ResourceDescription1 pResourceDesc,
								 uint FirstSubresource,
								 uint NumSubresources,
								 ulong BaseOffset,
								 [Optional] Span< PlacedSubresourceFootprint > pLayouts,
								 out Span< uint > pNumRows,
								 out Span< ulong > pRowSizeInBytes, 
								 out ulong pTotalBytes ) ;
	
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device8) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device8).GUID.ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable. Instantiate( ) => new Device8( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device8( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device8( (ID3D12Device8)pComObj! ) ;
	// ==================================================================================
}


[ProxyFor( typeof( ID3D12Device9 ) )]
public interface IDevice9: IDevice8 {
	
	/// <summary>Creates an object that grants access to a shader cache, potentially opening an existing cache or creating a new one.</summary>
	/// <param name="pDesc">
	/// <para>Type: \_In\_ **const [D3D12_SHADER_CACHE_SESSION_DESC](ns-d3d12-d3d12_shader_cache_session_desc.md)\*** A **D3D12_SHADER_CACHE_SESSION_DESC** structure describing the shader cache session to create.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device9-createshadercachesession#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **[REFIID](/openspecs/windows_protocols/ms-oaut/bbde795f-5398-42d8-9f59-3613da03c318)** The globally unique identifier (GUID) for the shader cache session interface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device9-createshadercachesession#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvSession">
	/// <para>Type: \_COM\_Outptr\_opt\_ **void\*\*** A pointer to a memory block that receives a pointer to the [ID3D12ShaderCacheSession](nn-d3d12-id3d12shadercachesession.md) interface for the shader cache session.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device9-createshadercachesession#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| | DXGI_ERROR_ALREADY_EXISTS | You tried to create a cache with an existing identifier. See [D3D12_SHADER_CACHE_SESSION_DESC::Identifier](ns-d3d12-d3d12_shader_cache_session_desc.md). |</para>
	/// </returns>
	/// <remarks></remarks>
	void CreateShaderCacheSession( in ShaderCacheSessionDescription pDesc, in Guid riid, out IShaderCacheSession ppvSession ) ;

	/// <summary>Modifies the behavior of caches used internally by Direct3D or by the driver.</summary>
	/// <param name="Kinds">
	/// <para>Type: **[D3D12_SHADER_CACHE_KIND_FLAGS](ne-d3d12-d3d12_shader_cache_kind_flags.md)** The caches to modify. Any one of these caches may or may not exist.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device9-shadercachecontrol#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Control">
	/// <para>Type: **[D3D12_SHADER_CACHE_CONTROL_FLAGS](ne-d3d12-d3d12_shader_cache_control_flags.md)** The way in which to modify the caches. You can't pass both **DISABLE** and **ENABLE** at the same time; and you must pass at least one flag.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device9-shadercachecontrol#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns></returns>
	/// <remarks></remarks>
	void ShaderCacheControl( ShaderCacheKindFlags Kinds, ShaderCacheControlFlags Control ) ;

	/// <summary>Creates a command queue with a creator ID.</summary>
	/// <param name="pDesc">
	/// <para>Type: \_In\_ **const [D3D12_COMMAND_QUEUE_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_command_queue_desc)\*** Specifies a **D3D12_COMMAND_QUEUE_DESC** that describes the command queue.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device9-createcommandqueue1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="CreatorID">
	/// <para>Type: **[REFIID](/openspecs/windows_protocols/ms-oaut/bbde795f-5398-42d8-9f59-3613da03c318)** A creator ID. See **Remarks**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device9-createcommandqueue1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **[REFIID](/openspecs/windows_protocols/ms-oaut/bbde795f-5398-42d8-9f59-3613da03c318)** The globally unique identifier (GUID) for the command queue interface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device9-createcommandqueue1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppCommandQueue">
	/// <para>Type: \_COM\_Outptr\_ **void\*\*** A pointer to a memory block that receives a pointer to the [ID3D12CommandQueue](/windows/win32/api/d3d12/nn-d3d12-id3d12commandqueue) interface for the command queue.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device9-createcommandqueue1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** Returns **E_OUTOFMEMORY** if there's insufficient memory to create the command queue; otherwise **S_OK**. See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>When multiple components in the same process are sharing a single Direct3D 12 device, often they will end up with separate workloads on independent command queues. In some hardware implementations, independent queues can run in parallel only with specific other command queues. Direct3D 12 applies a first-come, first-serve grouping for queues, which might not work well for all application or component designs. To help inform Direct3D 12's grouping of queues, you can specify a *creator ID* (which is unique per component) that restricts the grouping to other queues with the same ID. When possible, a component should choose the same unique ID for all of its queues. Microsoft has reserved a few well-known creator IDs for use by Microsoft-developed implementations of APIs on top of Direct3D 12.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device9-createcommandqueue1#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateCommandQueue1( in CommandQueueDescription pDesc,
							  in Guid CreatorID, in Guid riid,
							  out ICommandQueue ppCommandQueue ) ;
	
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device9) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device9).GUID.ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable. Instantiate( ) => new Device9( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device9( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device9( (ID3D12Device9)pComObj! ) ;
	// ==================================================================================
}


/// <summary>Represents a virtual adapter.</summary>
/// <remarks>Requires the DirectX 12 Agility SDK 1.7 or later.</remarks>
[ProxyFor( typeof( ID3D12Device10 ) )]
public interface IDevice10: IDevice9 {

	/// <summary>Creates a committed resource with an initial layout rather than an initial state.</summary>
	/// <param name="pHeapProperties">
	/// <para>Type: \_In\_ **const [D3D12_HEAP_PROPERTIES](/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_properties)\*** A pointer to a **D3D12_HEAP_PROPERTIES** structure that provides properties for the resource's heap.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createcommittedresource3#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="HeapFlags">
	/// <para>Type: **[D3D12_HEAP_FLAGS](/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_flags)** Heap options, as a bitwise-OR'd combination of **D3D12_HEAP_FLAGS** enumeration constants.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createcommittedresource3#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: **const [D3D12_RESOURCE_DESC1](/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_desc1)\*** A pointer to a **D3D12_RESOURCE_DESC1** structure that describes the resource, including a mip region.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createcommittedresource3#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="InitialLayout">The initial layout of the texture resource; **D3D12_BARRIER_LAYOUT::D3D12_BARRIER_LAYOUT_UNDEFINED** for buffers.</param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: **const [D3D12_CLEAR_VALUE](/windows/win32/api/d3d12/ns-d3d12-d3d12_clear_value)\*** Specifies a **D3D12_CLEAR_VALUE** structure that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the [D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags) or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, you should choose the value with which the clear operation will most commonly be called. You can call the clear operation with other values, but those operations won't be as efficient as when the value matches the one passed in to resource creation. When you use [D3D12_RESOURCE_DIMENSION_BUFFER](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_dimension), you must set *pOptimizedClearValue* to `nullptr`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createcommittedresource3#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pProtectedSession">
	/// <para>Type: **[ID3D12ProtectedResourceSession](/windows/win32/api/d3d12/nn-d3d12-id3d12protectedresourcesession)\*** An optional pointer to an object that represents a session for content protection. If provided, this session indicates that the resource should be protected. You can obtain an **ID3D12ProtectedResourceSession** by calling [ID3D12Device4::CreateProtectedResourceSession](/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createprotectedresourcesession).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createcommittedresource3#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="NumCastableFormats">The number of elements in *pCastableFormats*.</param>
	/// <param name="pCastableFormats">A contiguous array of [DXGI_FORMAT](/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format) structures that this resource can be cast to.</param>
	/// <param name="riidResource">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the resource interface to return in *ppvResource*. While *riidResource* is most commonly the **GUID** of [ID3D12Resource](/windows/win32/api/d3d12/nn-d3d12-id3d12resource), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createcommittedresource3#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created resource object. *ppvResource* can be `nullptr`, to enable capability testing. When *ppvResource* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createcommittedresource3#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the resource.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks></remarks>
	void CreateCommittedResource3( in HeapProperties pHeapProperties,
								   HeapFlags HeapFlags,
								   in ResourceDescription1 pDesc,
								   BarrierLayout InitialLayout,
								   [Optional] in ClearValue? pOptimizedClearValue,
								   IProtectedResourceSession pProtectedSession,
								   uint NumCastableFormats,
								   [Optional] in Span< Format > pCastableFormats,
								   in Guid riidResource,
								   out IResource ppvResource ) ;
	

	/// <summary>Creates a resource that is placed in a specific heap. Placed resources are the lightest weight resource objects available, and are the fastest to create and destroy.</summary>
	/// <param name="pHeap">
	/// <para>Type: [in] **<a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12heap">ID3D12Heap</a>*** A pointer to the **ID3D12Heap** interface that represents the heap in which the resource is placed.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createplacedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="HeapOffset">
	/// <para>Type: **<a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a>** The offset, in bytes, to the resource. The *HeapOffset* must be a multiple of the resource's alignment, and *HeapOffset* plus the resource size must be smaller than or equal to the heap size. <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourceallocationinfo">**GetResourceAllocationInfo**</a> must be used to understand the sizes of texture resources.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createplacedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: [in] **const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_desc">D3D12_RESOURCE_DESC</a>*** A pointer to a **D3D12_RESOURCE_DESC** structure that describes the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createplacedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="InitialLayout">The initial layout of the texture resource; **D3D12_BARRIER_LAYOUT::D3D12_BARRIER_LAYOUT_UNDEFINED** for buffers.</param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: [in, optional] **const <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_clear_value">D3D12_CLEAR_VALUE</a>*** Specifies a **D3D12_CLEAR_VALUE** that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the **D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET** or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, your application should choose the value that the clear operation will most commonly be called with. Clear operations can be called with other values, but those operations will not be as efficient as when the value matches the one passed into resource creation. *pOptimizedClearValue* must be NULL when used with **D3D12_RESOURCE_DIMENSION_BUFFER**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createplacedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="NumCastableFormats">The number of elements in *pCastableFormats*.</param>
	/// <param name="pCastableFormats">A contiguous array of [DXGI_FORMAT](/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format) structures that this resource can be cast to.</param>
	/// <param name="riid">
	/// <para>Type: **REFIID** The globally unique identifier (**GUID**) for the resource interface. This is an input parameter. The **REFIID**, or **GUID**, of the interface to the resource can be obtained by using the `__uuidof` macro. For example, `__uuidof(ID3D12Resource)` gets the **GUID** of the interface to a resource. Although **riid** is, most commonly, the GUID for <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12resource">**ID3D12Resource**</a>, it may be any **GUID** for any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createplacedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: [out, optional] **void**** A pointer to a memory block that receives a pointer to the resource. *ppvResource* can be NULL, to enable capability testing. When *ppvResource* is NULL, no object will be created and S_FALSE will be returned when *pResourceDesc* and other parameters are valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createplacedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **<a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a>** This method returns **E_OUTOFMEMORY** if there is insufficient memory to create the resource. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>See **Remarks** for [ID3D12Device::CreatePlacedResource](nf-d3d12-id3d12device-createplacedresource.md).</remarks>
	void CreatePlacedResource2( IHeap pHeap, ulong HeapOffset, 
								in ResourceDescription1 pDesc, 
								BarrierLayout InitialLayout, 
								[Optional] in ClearValue? pOptimizedClearValue, 
								uint NumCastableFormats, 
								[Optional] in Span< Format > pCastableFormats, 
								in Guid riid,
								out IResource ppvResource ) ;

	/// <summary>Creates a resource that is reserved, and not yet mapped to any pages in a heap.</summary>
	/// <param name="pDesc">
	/// <para>Type: **const [D3D12_RESOURCE_DESC](./ns-d3d12-d3d12_resource_desc.md)\*** A pointer to a **D3D12_RESOURCE_DESC** structure that describes the resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createreservedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="InitialLayout">The initial layout of the texture resource; **D3D12_BARRIER_LAYOUT::D3D12_BARRIER_LAYOUT_UNDEFINED** for buffers.</param>
	/// <param name="pOptimizedClearValue">
	/// <para>Type: **const [D3D12_CLEAR_VALUE](./ns-d3d12-d3d12_clear_value.md)\*** Specifies a **D3D12_CLEAR_VALUE** structure that describes the default value for a clear color. *pOptimizedClearValue* specifies a value for which clear operations are most optimal. When the created resource is a texture with either the [D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET](./ne-d3d12-d3d12_resource_flags.md) or **D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL** flags, you should choose the value with which the clear operation will most commonly be called. You can call the clear operation with other values, but those operations won't be as efficient as when the value matches the one passed in to resource creation. When you use [D3D12_RESOURCE_DIMENSION_BUFFER](./ne-d3d12-d3d12_resource_dimension.md), you must set *pOptimizedClearValue* to `nullptr`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createreservedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pProtectedSession">
	/// <para>Type: **[ID3D12ProtectedResourceSession](/windows/win32/api/d3d12/nn-d3d12-id3d12protectedresourcesession)\*** An optional pointer to an object that represents a session for content protection. If provided, this session indicates that the resource should be protected. You can obtain an **ID3D12ProtectedResourceSession** by calling [ID3D12Device4::CreateProtectedResourceSession](/windows/win32/api/d3d12/nf-d3d12-id3d12device4-createprotectedresourcesession).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createreservedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="NumCastableFormats">The number of elements in *pCastableFormats*.</param>
	/// <param name="pCastableFormats">A contiguous array of [DXGI_FORMAT](/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format) structures that this resource can be cast to.</param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier (**GUID**) of the resource interface to return in *ppvResource*. See **Remarks**. While *riidResource* is most commonly the **GUID** of [ID3D12Resource](./nn-d3d12-id3d12resource.md), it may be the **GUID** of any interface. If the resource object doesn't support the interface for this **GUID**, then creation fails with **E_NOINTERFACE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createreservedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvResource">
	/// <para>Type: **void\*\*** An optional pointer to a memory block that receives the requested interface pointer to the created resource object. *ppvResource* can be `nullptr`, to enable capability testing. When *ppvResource* is `nullptr`, no object is created, and **S_FALSE** is returned when *pDesc* is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device10-createreservedresource2#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/win32/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_OUTOFMEMORY|There is insufficient memory to create the resource.| See [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues) for other possible return values.</para>
	/// </returns>
	/// <remarks>See **Remarks** for [ID3D12Device.CreateReservedResource](nf-d3d12-id3d12device-createreservedresource.md).</remarks>
	void CreateReservedResource2( in ResourceDescription pDesc,
								  BarrierLayout InitialLayout,
								  [Optional] in ClearValue? pOptimizedClearValue,
								  IProtectedResourceSession pProtectedSession,
								  uint NumCastableFormats,
								  [Optional] in Span< Format > pCastableFormats,
								  in Guid riid,
								  out IResource ppvResource ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12Device10) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device10).GUID.ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable. Instantiate( ) => new Device10( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device10( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device10( (ID3D12Device10)pComObj! ) ;
	// ==================================================================================
}


[ProxyFor( typeof( ID3D12Device11 ) )]
public interface IDevice11: IDevice10 {
	// ---------------------------------------------------------------------------------

	void CreateSampler2( in SamplerDescription2 pDesc, CPUDescriptorHandle DestDescriptor ) ;

	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( ID3D12Device11 ) ;
	public new static Guid IID => ( ComType.GUID ) ;

	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12Device11 ).GUID.ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}

	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable.Instantiate( ) => new Device11( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device11( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device11( (ID3D12Device11)pComObj! ) ;
	// ==================================================================================
} ;


[ProxyFor( typeof( ID3D12Device12 ) )]
public interface IDevice12: IDevice11 {
	// ---------------------------------------------------------------------------------
	
	ResourceAllocationInfo GetResourceAllocationInfo3( uint visibleMask, uint numResourceDescs,
													   in ResourceDescription1 pResourceDescs,
													   uint[ ] pNumCastableFormats,
													   [Optional] in Span< Format > ppCastableFormats,
													   [Optional] in ResourceAllocationInfo1? pResourceAllocationInfo1 ) ;

	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12Device12) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device12).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// ---------------------------------------------------------------------------------
	static IInstantiable IInstantiable. Instantiate( ) => new Device12( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Device12( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Device12( (ID3D12Device12)pComObj! ) ;
	// ==================================================================================
}










