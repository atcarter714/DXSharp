#region Using Directives
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.DXGI ;

#endregion
namespace DXSharp.Direct3D12 ;


//! TODO: Find out why we have duplicate "FeatureLevel" enums (FeatureLevel and D3DFeatureLevel)
[EquivalentOf(typeof(D3D_FEATURE_LEVEL))]
public enum FeatureLevel {
	/// <summary>Allows Microsoft Compute Driver Model (MCDM) devices to be used, or more feature-rich devices (such as traditional GPUs) that support a superset of the functionality. MCDM is the overall driver model for compute-only; it's a scaled-down peer of the larger scoped Windows Device Driver Model (WDDM).</summary>
	D3D10Core = 4096,

	/// <summary>Targets features supported by [feature level](/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro) 9.1, including shader model 2.</summary>
	D3D9_1 = 37120,

	/// <summary>Targets features supported by [feature level](/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro) 9.2, including shader model 2.</summary>
	D3D9_2 = 37376,

	/// <summary>Targets features supported by [feature level](/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro) 9.3, including shader model 2.0b.</summary>
	D3D9_3 = 37632,

	/// <summary>Targets features supported by Direct3D 10.0, including shader model 4.</summary>
	D3D10_0 = 40960,

	/// <summary>Targets features supported by Direct3D 10.1, including shader model 4.</summary>
	D3D10_1 = 41216,

	/// <summary>Targets features supported by Direct3D 11.0, including shader model 5.</summary>
	D3D11_0 = 45056,

	/// <summary>Targets features supported by Direct3D 11.1, including shader model 5 and logical blend operations. This feature level requires a display driver that is at least implemented to WDDM for Windows 8 (WDDM 1.2).</summary>
	D3D11_1 = 45312,

	/// <summary>Targets features supported by Direct3D 12.0, including shader model 5.</summary>
	D3D12_0 = 49152,

	/// <summary>Targets features supported by Direct3D 12.1, including shader model 5.</summary>
	D3D12_1 = 49408,

	/// <summary>Targets features supported by Direct3D 12.2, including shader model 6.5. For more information about feature level 12_2, see its [specification page](https://microsoft.github.io/DirectX-Specs/d3d/D3D12_FeatureLevel12_2.html). Feature level 12_2 is available in Windows SDK builds 20170 and later.</summary>
	D3D12_2 = 49664,
} ;


/// <summary>Describes the set of features targeted by a Direct3D device.</summary>
/// <remarks>
/// <para>For an overview of the capabilities of each feature level, see [Direct3D feature levels](/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro). For information about limitations creating non-hardware-type devices on certain feature levels, see [Limitations creating WARP and reference devices](/windows/desktop/direct3d11/overviews-direct3d-11-devices-limitations).</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3dcommon/ne-d3dcommon-d3d_feature_level#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D_FEATURE_LEVEL ) )]
public enum D3DFeatureLevel {
	/// <summary>Allows Microsoft Compute Driver Model (MCDM) devices to be used, or more feature-rich devices (such as traditional GPUs) that support a superset of the functionality. MCDM is the overall driver model for compute-only; it's a scaled-down peer of the larger scoped Windows Device Driver Model (WDDM).</summary>
	Level1_0_Core = 4096,

	/// <summary>Targets features supported by [feature level](/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro) 9.1, including shader model 2.</summary>
	Level9_1 = 37120,

	/// <summary>Targets features supported by [feature level](/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro) 9.2, including shader model 2.</summary>
	Level9_2 = 37376,

	/// <summary>Targets features supported by [feature level](/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro) 9.3, including shader model 2.0b.</summary>
	Level9_3 = 37632,

	/// <summary>Targets features supported by Direct3D 10.0, including shader model 4.</summary>
	Level10_0 = 40960,

	/// <summary>Targets features supported by Direct3D 10.1, including shader model 4.</summary>
	Level10_1 = 41216,

	/// <summary>Targets features supported by Direct3D 11.0, including shader model 5.</summary>
	Level11_0 = 45056,

	/// <summary>Targets features supported by Direct3D 11.1, including shader model 5 and logical blend operations. This feature level requires a display driver that is at least implemented to WDDM for Windows 8 (WDDM 1.2).</summary>
	Level11_1 = 45312,

	/// <summary>Targets features supported by Direct3D 12.0, including shader model 5.</summary>
	Level12_0 = 49152,

	/// <summary>Targets features supported by Direct3D 12.1, including shader model 5.</summary>
	Level12_1 = 49408,

	/// <summary>Targets features supported by Direct3D 12.2, including shader model 6.5. For more information about feature level 12_2, see its [specification page](https://microsoft.github.io/DirectX-Specs/d3d/D3D12_FeatureLevel12_2.html). Feature level 12_2 is available in Windows SDK builds 20170 and later.</summary>
	Level12_2 = 49664,
} ;


[Flags, EquivalentOf( typeof(D3D12_CLEAR_FLAGS) )]
public enum ClearFlags {
	/// <summary>Indicates the depth buffer should be cleared.</summary>
	Depth = 0x00000001,
	/// <summary>Indicates the stencil buffer should be cleared.</summary>
	Stencil = 0x00000002,
} ;


[EquivalentOf( typeof( D3D12_COMMAND_LIST_TYPE ) )]
public enum CommandListType {
	/// <summary>Specifies a command buffer that the GPU can execute. A direct command list doesn't inherit any GPU state.</summary>
	Direct = 0,
	/// <summary>Specifies a command buffer that can be executed only directly via a direct command list. A bundle command list inherits all GPU state (except for the currently set pipeline state object and primitive topology).</summary>
	Bundle = 1,
	/// <summary>Specifies a command buffer for computing.</summary>
	Compute = 2,
	/// <summary>Specifies a command buffer for copying.</summary>
	Copy = 3,
	/// <summary>Specifies a command buffer for video decoding.</summary>
	VideoDecode = 4,
	/// <summary>Specifies a command buffer for video processing.</summary>
	VideoProcess = 5,
	VideoEncode = 6,
	None         = -1,
}


[Flags, EquivalentOf( typeof( D3D12_COMMAND_QUEUE_FLAGS ) )]
public enum CommandQueueFlags {
	/// <summary>Indicates a default command queue.</summary>
	None = 0x00000000,
	/// <summary>Indicates that the GPU timeout should be disabled for this command queue.</summary>
	DisableGPUTimeout = 0x00000001,
}


[Flags, EquivalentOf( typeof( D3D12_TILE_RANGE_FLAGS ) )]
public enum TileRangeFlags {
	/// <summary>No tile-mapping flags are specified.</summary>
	None = 0,
	/// <summary>The tile range is <b>NULL</b>.</summary>
	Null = 1,
	/// <summary>Skip the tile range.</summary>
	Skip = 2,
	/// <summary>Reuse a single tile in the tile range.</summary>
	ReuseSingleTile = 4,
}


[Flags, EquivalentOf( typeof( D3D12_TILE_MAPPING_FLAGS ) )]
public enum TileMappingFlags {
	/// <summary>No tile-mapping flags are specified.</summary>
	None = 0x00000000,
	/// <summary>Unsupported, do not use.</summary>
	NoHazard = 0x00000001,
}


[Flags, EquivalentOf( typeof( D3D12_HEAP_FLAGS ) )]
public enum HeapFlags {
	/// <summary>No options are specified.</summary>
	None = 0x00000000,

	/// <summary>The heap is shared. Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/shared-heaps">Shared Heaps</a>.</summary>
	Shared = 0x00000001,

	/// <summary>The heap isn't allowed to contain buffers.</summary>
	DenyBuffers = 0x00000004,

	/// <summary>The heap is allowed to contain swap-chain surfaces.</summary>
	AllowDisplay = 0x00000008,

	/// <summary>The heap is allowed to share resources across adapters. Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/shared-heaps">Shared Heaps</a>. A protected session cannot be mixed with resources that are shared across adapters.</summary>
	SharedCrossAdapter = 0x00000020,

	/// <summary>The heap is not allowed to store Render Target (RT) and/or Depth-Stencil (DS) textures.</summary>
	DenyRTDSTextures = 0x00000040,

	/// <summary>The heap is not allowed to contain resources with TEXTURE1D, TEXTURE2D, or TEXTURE3D  unless either D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET or D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL are present. Refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_dimension">D3D12_RESOURCE_DIMENSION</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_flags">D3D12_RESOURCE_FLAGS</a>.</summary>
	DenyNonRTDSTextures = 0x00000080,

	/// <summary>Unsupported. Do not use.</summary>
	HardwareProtected = 0x00000100,

	/// <summary>The heap supports MEM_WRITE_WATCH functionality, which causes the system to track the pages that are written to in the committed memory region. This flag can't be combined with the D3D12_HEAP_TYPE_DEFAULT or D3D12_CPU_PAGE_PROPERTY_UNKNOWN flags. Applications are discouraged from using this flag themselves because it prevents tools from using this functionality.</summary>
	AllowWriteWatch = 0x00000200,

	/// <summary>
	/// <para>Ensures that atomic operations will be atomic on this heap's memory, according to components able to see the memory. Creating a heap with this flag will fail under either of these conditions. - The heap type is **D3D12_HEAP_TYPE_DEFAULT**, and the heap can be visible on multiple nodes, but the device does *not* support [**D3D12_CROSS_NODE_SHARING_TIER_3**](./ne-d3d12-d3d12_cross_node_sharing_tier.md). - The heap is CPU-visible, but the heap type is *not* **D3D12_HEAP_TYPE_CUSTOM**. Note that heaps with this flag might be a limited resource on some systems.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	AllowShaderAtomics = 0x00000400,

	/// <summary>
	/// <para>The heap is created in a non-resident state and must be made resident using [ID3D12Device::MakeResident](./nf-d3d12-id3d12device-makeresident.md) or [ID3D12Device3::EnqueueMakeResident](./nf-d3d12-id3d12device3-enqueuemakeresident.md). By default, the final step of heap creation is to make the heap resident, so this flag skips this step and allows the application to decide when to do so.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	CreateNotResident = 0x00000800,

	/// <summary>Allows the OS to not zero the heap created. By default, committed resources and heaps are almost always zeroed upon creation. This flag allows this to be elided in some scenarios. However, it doesn't guarantee it. For example, memory coming from other processes still needs to be zeroed for data protection and process isolation. This can lower the overhead of creating the heap.</summary>
	CreateNotZeroed = 0x00001000,
	ToolsUseManualWriteTracking = 0x00002000,

	/// <summary>The heap is allowed to store all types of buffers and/or textures. This is an alias; for more details, see "Aliases" in the Remarks section.</summary>
	AllowAllBuffersAndTextures = 0x00000000,

	/// <summary>The heap is only allowed to store buffers. This is an alias; for more details, see "Aliases" in the Remarks section.</summary>
	AllowOnlyBuffers = 0x000000C0,

	/// <summary>The heap is only allowed to store non-RT, non-DS textures. This is an alias; for more details, see "Aliases" in the Remarks section.</summary>
	AllowOnlyNonRTDSTextures = 0x00000044,

	/// <summary>The heap is only allowed to store RT and/or DS textures. This is an alias; for more details, see "Aliases" in the Remarks section.</summary>
	AllowOnlyRTDSTextures = 0x00000084,
} ;


[Flags, EquivalentOf( typeof( D3D12_RESOURCE_FLAGS ) )]
public enum ResourceFlags {
	/// <summary>No options are specified.</summary>
	None = 0x00000000,

	/// <summary>
	/// <para>Allows a render target view to be created for the resource; and also enables the resource to transition into the state of [D3D12_RESOURCE_STATE_RENDER_TARGET](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states). Some adapter architectures allocate extra memory for textures with this flag to reduce the effective bandwidth during common rendering. This characteristic may not be beneficial for textures that are never rendered to, nor is it available for textures compressed with BC formats. Your application should avoid setting this flag when rendering will never occur. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	AllowRenderTarget = 0x00000001,

	/// <summary>
	/// <para>Allows a depth stencil view to be created for the resource, as well as enables the resource to transition into the state of [D3D12_RESOURCE_STATE_DEPTH_WRITE](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states) and/or **D3D12_RESOURCE_STATE_DEPTH_READ**. Most adapter architectures allocate extra memory for textures with this flag to reduce the effective bandwidth, and maximize optimizations for early depth-test. Your application should avoid setting this flag when depth operations will never occur. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	AllowDepthStencil = 0x00000002,

	/// <summary>
	/// <para>Allows an unordered access view to be created for the resource, as well as enables the resource to transition into the state of [D3D12_RESOURCE_STATE_UNORDERED_ACCESS](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states). Some adapter architectures must resort to less efficient texture layouts in order to provide this functionality. If a texture is rarely used for unordered access, then it might be worth having two textures around and copying between them. One texture would have this flag, while the other wouldn't. Your application should avoid setting this flag when unordered access operations will never occur. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	AllowUnorderedAccess = 0x00000004,

	/// <summary>
	/// <para>Disallows a shader resource view from being created for the resource, as well as disables the resource from transitioning into the state of [D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states) or **D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE**. Some adapter architectures experience increased bandwidth for depth stencil textures when shader resource views are precluded. If a texture is rarely used for shader resources, then it might be worth having two textures around and copying between them. One texture would have this flag, while the other wouldn't. Your application should set this flag when depth stencil textures will never be used from shader resource views. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	DenyShaderResource = 0x00000008,

	/// <summary>
	/// <para>Allows the resource to be used for cross-adapter data, as well as those features enabled by **D3D12_RESOURCE_FLAGS::ALLOW_SIMULTANEOUS_ACCESS**. Cross-adapter resources commonly preclude techniques that reduce effective texture bandwidth during usage, and some adapter architectures might require different caching behavior. Your application should avoid setting this flag when the resource data will never be used with another adapter. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	AllowCrossAdapter = 0x00000010,

	/// <summary>
	/// <para>Allows a resource to be simultaneously accessed by multiple different queues, devices, or processes (for example, allows a resource to be used with [ResourceBarrier](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resourcebarrier) transitions performed in more than one command list executing at the same time). Simultaneous access allows multiple readers and one writer, as long as the writer doesn't concurrently modify the texels that other readers are accessing. Some adapter architectures can't leverage techniques to reduce effective texture bandwidth during usage. However, your application should avoid setting this flag when multiple readers are not required during frequent, non-overlapping writes to textures. Use of this flag can compromise resource fences to perform waits, and prevent any compression being used with a resource. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	AllowSimultaneousAccess = 0x00000020,

	/// <summary>
	/// <para>Specfies that this resource may be used only as a decode reference frame. It may be written to or read only by the video decode operation. [D3D12_VIDEO_DECODE_TIER_1](../d3d12video/ne-d3d12video-d3d12_video_decode_tier.md) and [D3D12_VIDEO_DECODE_TIER_2](../d3d12video/ne-d3d12video-d3d12_video_decode_tier.md) may report [D3D12_VIDEO_DECODE_CONFIGURATION_FLAG_REFERENCE_ONLY_ALLOCATIONS_REQUIRED](../d3d12video/ne-d3d12video-d3d12_video_decode_configuration_flags.md) in the [D3D12_FEATURE_DATA_VIDEO_DECODE_SUPPORT](../d3d12video/ns-d3d12video-d3d12_feature_data_video_decode_support.md) structure configuration flag. If that happens, then your application must allocate reference frames with the **D3D12_RESOURCE_FLAGS::VIDEO_DECODE_REFERENCE_ONLY** resource flag. [D3D12_VIDEO_DECODE_TIER_3](../d3d12video/ne-d3d12video-d3d12_video_decode_tier.md) must not set the [D3D12_VIDEO_DECODE_CONFIGURATION_FLAG_REFERENCE_ONLY_ALLOCATIONS_REQUIRED] (../d3d12video/ne-d3d12video-d3d12_video_decode_configuration_flags) configuration flag, and must not require the use of this resource flag.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	VideoDecodeReferenceOnly = 0x00000040,

	/// <summary>Specfies that this resource may be used only as an encode reference frame. It may be written to or read only by the video encode operation.</summary>
	VideoEncodeReferenceOnly = 0x00000080,

	/// <summary>
	/// <para>Reserved for future use. Don't use. Requires the DirectX 12 Agility SDK 1.7 or later. Indicates that a buffer is to be used as a raytracing acceleration structure.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	RaytracingAccelerationStructure = 0x00000100,
} ;


[Flags, EquivalentOf( typeof( D3D12_PIPELINE_STATE_FLAGS ) )]
public enum PipelineStateFlags {
	/// <summary>Indicates no flags.</summary>
	None = 0x00000000,
	/// <summary>
	/// <para>Indicates that the pipeline state should be compiled with additional information to assist debugging. This can only be set on WARP devices.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	ToolDebug                  = 0x00000001,
	DynamicDepthBias           = 0x00000004,
	DynamicIndexBufferStripCut = 0x00000008,
} ;


[Flags, EquivalentOf( typeof( D3D12_DESCRIPTOR_HEAP_FLAGS ) )]
public enum DescriptorHeapFlags {
	/// <summary>Indicates default usage of a heap.</summary>
	None = 0x00000000,
	/// <summary>
	/// <para>The flag [D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE](/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_flags) can optionally be set on a descriptor heap to indicate it is be bound on a command list for reference by shaders. Descriptor heaps created <i>without</i> this flag allow applications the option to stage descriptors in CPU memory before copying them to a shader visible descriptor heap, as a convenience. But it is also fine for applications to directly create descriptors into shader visible descriptor heaps with no requirement to stage anything on the CPU. Descriptor heaps bound via [ID3D12GraphicsCommandList::SetDescriptorHeaps](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setdescriptorheaps) must have the **D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE** flag set, else the debug layer will produce an error. Descriptor heaps with the **D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE** flag can't be used as the source heaps in calls to [ID3D12Device::CopyDescriptors](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors) or [ID3D12Device::CopyDescriptorsSimple](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple), because they could be resident in **WRITE_COMBINE** memory or GPU-local memory, which is very inefficient to read from. This flag only applies to CBV/SRV/UAV descriptor heaps, and sampler descriptor heaps. It does not apply to other descriptor heap types since shaders do not directly reference the other types. Attempting to create an RTV/DSV heap with **D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE** results in a debug layer error.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	ShaderVisible = 0x00000001,
} ;


[EquivalentOf( typeof( D3D12_DESCRIPTOR_HEAP_TYPE ) )]
public enum DescriptorHeapType {
	/// <summary>The descriptor heap for the combination of constant-buffer, shader-resource, and unordered-access views.</summary>
	CBV_SRV_UAV = 0,
	/// <summary>The descriptor heap for the sampler.</summary>
	Sampler = 1,
	/// <summary>The descriptor heap for the render-target view.</summary>
	RTV = 2,
	/// <summary>The descriptor heap for the depth-stencil view.</summary>
	DSV = 3,
	/// <summary>The number of types of descriptor heaps.</summary>
	NumberOfTypes = 4,
} ;


[EquivalentOf( typeof( D3D12_SRV_DIMENSION ) )]
public enum SRVDimension {
	/// <summary>The type is unknown.</summary>
	Unknown = 0,
	/// <summary>The resource is a buffer.</summary>
	Buffer = 1,
	/// <summary>The resource is a 1D texture.</summary>
	Texture1D = 2,
	/// <summary>The resource is an array of 1D textures.</summary>
	Texture1Darray = 3,
	/// <summary>The resource is a 2D texture.</summary>
	Texture2D = 4,
	/// <summary>The resource is an array of 2D textures.</summary>
	Texture2Darray = 5,
	/// <summary>The resource is a multisampling 2D texture.</summary>
	Texture2Dms = 6,
	/// <summary>The resource is an array of multisampling 2D textures.</summary>
	Texture2Dmsarray = 7,
	/// <summary>The resource is a 3D texture.</summary>
	Texture3D = 8,
	/// <summary>The resource is a cube texture.</summary>
	Texturecube = 9,
	/// <summary>The resource is an array of cube textures.</summary>
	Texturecubearray = 10,
	/// <summary>The resource is a raytracing acceleration structure.</summary>
	RaytracingAccelerationStructure = 11,
} ;


[Flags, EquivalentOf( typeof( D3D12_BUFFER_SRV_FLAGS ) )]
public enum BufferSRVFlags {
	/// <summary>Indicates a default view.</summary>
	None = 0x00000000,
	/// <summary>View the buffer as raw. For more info about raw viewing of buffers, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-resources-intro">Raw Views of Buffers</a>.</summary>
	Raw = 0x00000001,
} ;


[EquivalentOf( typeof( D3D12_RTV_DIMENSION ) )]
public enum RTVDimension {
	/// <summary>Do not use this value, as it will cause <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createrendertargetview">ID3D12Device::CreateRenderTargetView</a> to fail.</summary>
	Unknown = 0,
	/// <summary>The resource will be accessed as a buffer.</summary>
	Buffer = 1,
	/// <summary>The resource will be accessed as a 1D texture.</summary>
	Texture1D = 2,
	/// <summary>The resource will be accessed as an array of 1D textures.</summary>
	Texture1DArray = 3,
	/// <summary>The resource will be accessed as a 2D texture.</summary>
	Texture2D = 4,
	/// <summary>The resource will be accessed as an array of 2D textures.</summary>
	Texture2DArray = 5,
	/// <summary>The resource will be accessed as a 2D texture with multisampling.</summary>
	Texture2DMS = 6,
	/// <summary>The resource will be accessed as an array of 2D textures with multisampling.</summary>
	Texture2DMSArray = 7,
	/// <summary>The resource will be accessed as a 3D texture.</summary>
	Texture3D = 8,
} ;


/// <summary>
/// Describes the subresources of a texture that are accessible from a depth-stencil view.
/// </summary>
/// <remarks>
/// Specify one of the values in this enumeration in the ViewDimension member of a <see cref="DepthStencilViewDesc"/> structure.<para/>
/// For additional details, see the Microsoft DirectX documentation for: 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/ne-d3d12-d3d12_dsv_dimension">D3D12_DSV_DIMENSION</a> and 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_view_desc">D3D12_DEPTH_STENCIL_VIEW_DESC</a>
/// </remarks>
[EquivalentOf( typeof(D3D12_DSV_DIMENSION) )]
public enum DSVDimension {
	/// <summary><b>UNKNOWN</b> is not a valid value for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_depth_stencil_view_desc">D3D12_DEPTH_STENCIL_VIEW_DESC</a> and is not used.</summary>
	Unknown = 0,
	/// <summary>The resource will be accessed as a 1D texture.</summary>
	Texture1D = 1,
	/// <summary>The resource will be accessed as an array of 1D textures.</summary>
	Texture1DArray = 2,
	/// <summary>The resource will be accessed as a 2D texture.</summary>
	Texture2D = 3,
	/// <summary>The resource will be accessed as an array of 2D textures.</summary>
	Texture2DArray = 4,
	/// <summary>The resource will be accessed as a 2D texture with multi sampling.</summary>
	Texture2DMS = 5,
	/// <summary>The resource will be accessed as an array of 2D textures with multi sampling.</summary>
	Texture2DMSArray = 6,
} ;


[Flags, EquivalentOf( typeof( D3D12_DSV_FLAGS ) )]
public enum DSVFlags {
	/// <summary>Indicates a default view.</summary>
	None = 0x00000000,
	/// <summary>Indicates that depth values are read only.</summary>
	ReadOnlyDepth = 0x00000001,
	/// <summary>Indicates that stencil values are read only.</summary>
	ReadOnlyStencil = 0x00000002,
} ;



/// <summary>Specifies filtering options during texture sampling.</summary>
/// <remarks>
/// <para>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_sampler_desc">D3D12_SAMPLER_DESC</a> structure.</para><para/>
/// <div class="alert">
/// <b>Note:</b> If you use different filter types for min versus mag filter, undefined behavior occurs in certain cases where the choice between whether magnification
/// or minification happens is ambiguous.  To prevent this undefined behavior, use filter modes that use similar filter operations for both min and mag (or use
/// anisotropic filtering, which avoids the issue as well).
/// </div>
/// During texture sampling, one or more texels are read and combined (this is calling filtering) to produce a single value.
/// Point sampling reads a single texel while linear sampling reads two texels (endpoints) and linearly interpolates a third
/// value between the endpoints. Microsoft High Level Shader Language (HLSL) texture-sampling functions also support comparison
/// filtering during texture sampling. Comparison filtering compares each sampled texel against a comparison value. The boolean
/// result is blended the same way that normal texture filtering is blended. You can use HLSL intrinsic texture-sampling functions
/// that implement texture filtering only or companion functions that use texture filtering with comparison filtering.<para/>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_filter#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_FILTER ) )]
public enum Filter {
	/// <summary>Use point sampling for minification, magnification, and mip-level sampling.</summary>
	MinMagMipPoint = 0,
	/// <summary>Use point sampling for minification and magnification; use linear interpolation for mip-level sampling.</summary>
	MinMagPointMipLinear = 1,
	/// <summary>Use point sampling for minification; use linear interpolation for magnification; use point sampling for mip-level sampling.</summary>
	MinPointMagLinearMipPoint = 4,
	/// <summary>Use point sampling for minification; use linear interpolation for magnification and mip-level sampling.</summary>
	MinPointMagMipLinear = 5,
	/// <summary>Use linear interpolation for minification; use point sampling for magnification and mip-level sampling.</summary>
	MinLinearMagMipPoint = 16,
	/// <summary>Use linear interpolation for minification; use point sampling for magnification; use linear interpolation for mip-level sampling.</summary>
	MinLinearMagPointMipLinear = 17,
	/// <summary>Use linear interpolation for minification and magnification; use point sampling for mip-level sampling.</summary>
	MinMagLinearMipPoint = 20,
	/// <summary>Use linear interpolation for minification, magnification, and mip-level sampling.</summary>
	MinMagMipLinear = 21,
	MinMagAnisotropicMipPoint = 84,
	/// <summary>Use anisotropic interpolation for minification, magnification, and mip-level sampling.</summary>
	Anisotropic = 85,
	/// <summary>Use point sampling for minification, magnification, and mip-level sampling. Compare the result to the comparison value.</summary>
	ComparisonMinMagMipPoint = 128,
	/// <summary>Use point sampling for minification and magnification; use linear interpolation for mip-level sampling. Compare the result to the comparison value.</summary>
	ComparisonMinMagPointMipLinear = 129,
	/// <summary>Use point sampling for minification; use linear interpolation for magnification; use point sampling for mip-level sampling. Compare the result to the comparison value.</summary>
	ComparisonMinPointMagLinearMipPoint = 132,
	/// <summary>Use point sampling for minification; use linear interpolation for magnification and mip-level sampling. Compare the result to the comparison value.</summary>
	ComparisonMinPointMagMipLinear = 133,
	/// <summary>Use linear interpolation for minification; use point sampling for magnification and mip-level sampling. Compare the result to the comparison value.</summary>
	ComparisonMinLinearMagMipPoint = 144,
	/// <summary>Use linear interpolation for minification; use point sampling for magnification; use linear interpolation for mip-level sampling. Compare the result to the comparison value.</summary>
	ComparisonMinLinearMagPointMipLinear = 145,
	/// <summary>Use linear interpolation for minification and magnification; use point sampling for mip-level sampling. Compare the result to the comparison value.</summary>
	ComparisonMinMagLinearMipPoint = 148,
	/// <summary>Use linear interpolation for minification, magnification, and mip-level sampling. Compare the result to the comparison value.</summary>
	ComparisonMinMagMipLinear = 149,
	ComparisonMinMagAnisotropicMipPoint = 212,
	/// <summary>Use anisotropic interpolation for minification, magnification, and mip-level sampling. Compare the result to the comparison value.</summary>
	ComparisonAnisotropic = 213,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_MIP_POINT</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MinimumMinMagMipPoint = 256,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_POINT_MIP_LINEAR</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MinimumMinMagPointMipLinear = 257,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_POINT_MAG_LINEAR_MIP_POINT</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MinimumMinPointMagLinearMipPoint = 260,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_POINT_MAG_MIP_LINEAR</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MinimumMinPointMagMipLinear = 261,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_LINEAR_MAG_MIP_POINT</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MinimumMinLinearMagMipPoint = 272,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_LINEAR_MAG_POINT_MIP_LINEAR</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MinimumMinLinearMagPointMipLinear = 273,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_LINEAR_MIP_POINT</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MinimumMinMagLinearMipPoint = 276,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_MIP_LINEAR</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MinimumMinMagMipLinear = 277,
	MinimumMinMagAnisotropicMipPoint = 340,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">ANISOTROPIC</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MinimumAnisotropic = 341,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_MIP_POINT</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MaximumMinMagMipPoint = 384,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_POINT_MIP_LINEAR</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MaximumMinMagPointMipLinear = 385,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_POINT_MAG_LINEAR_MIP_POINT</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MaximumMinPointMagLinearMipPoint = 388,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_POINT_MAG_MIP_LINEAR</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MaximumMinPointMagMipLinear = 389,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_LINEAR_MAG_MIP_POINT</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MaximumMinLinearMagMipPoint = 400,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_LINEAR_MAG_POINT_MIP_LINEAR</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MaximumMinLinearMagPointMipLinear = 401,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_LINEAR_MIP_POINT</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MaximumMinMagLinearMipPoint = 404,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_MIP_LINEAR</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MaximumMinMagMipLinear = 405,
	MaximumMinMagAnisotropicMipPoint = 468,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">ANISOTROPIC</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MaximumAnisotropic = 469,
} ;


[EquivalentOf( typeof( D3D12_TEXTURE_ADDRESS_MODE ) )]
public enum TextureAddressMode {
	/// <summary>
	/// <para>Tile the texture at every (u,v) integer junction. For example, for u values between 0 and 3, the texture is repeated three times.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_address_mode#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Wrap = 1,
	/// <summary>
	/// <para>Flip the texture at every (u,v) integer junction. For u values between 0 and 1, for example, the texture is addressed normally; between 1 and 2, the texture is flipped (mirrored); between 2 and 3, the texture is normal again; and so on.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_address_mode#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Mirror = 2,
	/// <summary>Texture coordinates outside the range [0.0, 1.0] are set to the texture color at 0.0 or 1.0, respectively.</summary>
	Clamp = 3,
	/// <summary>Texture coordinates outside the range [0.0, 1.0] are set to the border color specified in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_sampler_desc">D3D12_SAMPLER_DESC</a> or HLSL code.</summary>
	Border = 4,
	/// <summary>
	/// <para>Similar to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">MIRROR</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">CLAMP</a>. Takes the absolute value of the texture coordinate (thus, mirroring around 0), and then clamps to the maximum value.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_address_mode#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	MirrorOnce = 5,
} ;


[EquivalentOf( typeof( D3D12_COMPARISON_FUNC ) )]
public enum ComparisonFunction {
	None = 0,
	/// <summary>Never pass the comparison.</summary>
	Never = 1,
	/// <summary>If the source data is less than the destination data, the comparison passes.</summary>
	Less = 2,
	/// <summary>If the source data is equal to the destination data, the comparison passes.</summary>
	Equal = 3,
	/// <summary>If the source data is less than or equal to the destination data, the comparison passes.</summary>
	LessEqual = 4,
	/// <summary>If the source data is greater than the destination data, the comparison passes.</summary>
	Greater = 5,
	/// <summary>If the source data is not equal to the destination data, the comparison passes.</summary>
	NotEqual = 6,
	/// <summary>If the source data is greater than or equal to the destination data, the comparison passes.</summary>
	GreaterEqual = 7,
	/// <summary>Always pass the comparison.</summary>
	Always = 8,
} ;


[EquivalentOf( typeof( D3D12_RESOURCE_DIMENSION ) )]
public enum ResourceDimension {
	/// <summary>Resource is of unknown type.</summary>
	Unknown = 0,
	/// <summary>Resource is a buffer.</summary>
	Buffer = 1,
	/// <summary>Resource is a 1D texture.</summary>
	Texture1D = 2,
	/// <summary>Resource is a 2D texture.</summary>
	Texture2D = 3,
	/// <summary>Resource is a 3D texture.</summary>
	Texture3D = 4,
} ;


[EquivalentOf( typeof( D3D12_TEXTURE_LAYOUT ) )]
public enum TextureLayout {
	/// <summary>
	/// <para>Indicates that the layout is unknown, and is likely adapter-dependent. During creation, the driver chooses the most efficient layout based on other resource properties, especially resource size and flags. Prefer this choice unless certain functionality is required from another texture layout. Zero-copy texture upload optimizations exist for UMA architectures; see <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-writetosubresource">ID3D12Resource::WriteToSubresource</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_layout#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Unknown = 0,
	/// <summary>
	/// <para>Indicates that data for the texture is stored in row-major order (sometimes called "pitch-linear order"). This texture layout locates consecutive texels of a row contiguously in memory, before the texels of the next row. Similarly, consecutive texels of a particular depth or array slice are contiguous in memory before the texels of the next depth or array slice. Padding may exist between rows and between depth or array slices to align collections of data. A stride is the distance in memory between rows, depth, or array slices; and it includes any padding. This texture layout enables sharing of the texture data between multiple adapters, when other layouts aren't available. Many restrictions apply, because this layout is generally not efficient for extensive usage: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_layout#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	RowMajor = 1,
	/// <summary>
	/// <para>Indicates that the layout within 64KB tiles and tail mip packing is up to the driver. No standard swizzle pattern. This texture layout is arranged into contiguous 64KB regions, also known as tiles, containing near equilateral amount of consecutive number of texels along each dimension. Tiles are arranged in row-major order. While there is no padding between tiles, there are typically unused texels within the last tile in each dimension. The layout of texels within the tile is undefined. Each subresource immediately follows where the previous subresource end, and the subresource order follows the same sequence as subresource ordinals. However, tail mip packing is adapter-specific. For more details, see tiled resource tier and <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling">ID3D12Device::GetResourceTiling</a>. This texture layout enables partially resident or sparse texture scenarios when used together with virtual memory page mapping functionality. This texture layout must be used together with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource">ID3D12Device::CreateReservedResource</a> to enable the usage of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-updatetilemappings">ID3D12CommandQueue::UpdateTileMappings</a>. Some restrictions apply to textures with this layout: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_layout#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	UndefinedSwizzle_64KB = 2,
	/// <summary>
	/// <para>Indicates that a default texture uses the standardized swizzle pattern. This texture layout is arranged the same way that 64KB_UNDEFINED_SWIZZLE is, except that the layout of texels within the tile is defined. Tail mip packing is adapter-specific. This texture layout enables optimizations when marshaling data between multiple adapters or between the CPU and GPU. The amount of copying can be reduced when multiple components understand the texture memory layout. This layout is generally more efficient for extensive usage than row-major layout, due to the rotationally invariant locality of neighboring texels. This layout can typically only be used with adapters that support standard swizzle, but exceptions exist for cross-adapter shared heaps. The restrictions for this layout are that the following aren't supported: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_layout#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	StandardSwizzle_64KB = 3,
} ;


[EquivalentOf( typeof( D3D12_HEAP_TYPE ) )]
public enum HeapType {
	/// <summary>Specifies the default heap. This heap type experiences the most bandwidth for the GPU, but cannot provide CPU access. The GPU can read and write to the memory from this pool, and resource transition barriers may be changed. The majority of heaps and resources are expected to be located here, and are typically populated through resources in upload heaps.</summary>
	Default = 1,
	/// <summary>
	/// <para>Specifies a heap used for uploading. This heap type has CPU access optimized for uploading to the GPU, but does not experience the maximum amount of bandwidth for the GPU. This heap type is best for CPU-write-once, GPU-read-once data; but GPU-read-once is stricter than necessary. GPU-read-once-or-from-cache is an acceptable use-case for the data; but such usages are hard to judge due to differing GPU cache designs and sizes. If in doubt, stick to the GPU-read-once definition or profile the difference on many GPUs between copying the data to a _DEFAULT heap vs. reading the data from an _UPLOAD heap. Resources in this heap must be created with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE</a>_GENERIC_READ and cannot be changed away from this. The CPU address for such heaps is commonly not efficient for CPU reads. The following are typical usages for _UPLOAD heaps: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Upload = 2,
	/// <summary>
	/// <para>Specifies a heap used for reading back. This heap type has CPU access optimized for reading data back from the GPU, but does not experience the maximum amount of bandwidth for the GPU. This heap type is best for GPU-write-once, CPU-readable data. The CPU cache behavior is write-back, which is conducive for multiple sub-cache-line CPU reads. Resources in this heap must be created with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE</a>_COPY_DEST, and cannot be changed away from this.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	ReadBack = 3,
	/// <summary>
	/// <para>Specifies a custom heap. The application may specify the memory pool and CPU cache properties directly, which can be useful for UMA optimizations, multi-engine, multi-adapter, or other special cases. To do so, the application is expected to understand the adapter architecture to make the right choice. For more details, see <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a>_ARCHITECTURE, <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture">D3D12_FEATURE_DATA_ARCHITECTURE</a>, and <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcustomheapproperties">GetCustomHeapProperties</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Custom = 4,
	GpuUpload = 5,
} ;


[Flags, EquivalentOf( typeof( D3D12_RESOURCE_STATES ) )]
public enum ResourceStates {
	/// <summary>
	/// <para>Your application should transition to this state only for accessing a resource across different graphics engine types. Specifically, a resource must be in the COMMON state before being used on a COPY queue (when previously used on DIRECT/COMPUTE), and before being used on DIRECT/COMPUTE (when previously used on COPY). This restriction doesn't exist when accessing data between DIRECT and COMPUTE queues. The COMMON state can be used for all usages on a Copy queue using the implicit state transitions. For more info, in <a href="https://docs.microsoft.com/windows/win32/direct3d12/user-mode-heap-synchronization">Multi-engine synchronization</a>, find "common". Additionally, textures must be in the COMMON state for CPU access to be legal, assuming the texture was created in a CPU-visible heap in the first place.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Common = 0x00000000,
	/// <summary>A subresource must be in this state when it is accessed by the GPU as a vertex buffer or constant buffer. This is a read-only state.</summary>
	VertexAndConstantBuffer = 0x00000001,
	/// <summary>A subresource must be in this state when it is accessed by the 3D pipeline as an index buffer. This is a read-only state.</summary>
	IndexBuffer = 0x00000002,
	/// <summary>
	/// <para>The resource is used as a render target. A subresource must be in this state when it is rendered to, or when it is cleared with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearrendertargetview">ID3D12GraphicsCommandList::ClearRenderTargetView</a>. This is a write-only state. To read from a render target as a shader resource, the resource must be in either **NON_PIXEL_SHADER_RESOURCE** or **PIXEL_SHADER_RESOURCE**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	RenderTarget = 0x00000004,
	/// <summary>The resource is used for unordered access. A subresource must be in this state when it is accessed by the GPU via an unordered access view. A subresource must also be in this state when it is cleared with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewuint">ID3D12GraphicsCommandList::ClearUnorderedAccessViewInt</a> or <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewfloat">ID3D12GraphicsCommandList::ClearUnorderedAccessViewFloat</a>. This is a read/write state.</summary>
	UnorderedAccess = 0x00000008,
	/// <summary>**DEPTH_WRITE** is a state that is mutually exclusive with other states. You should use it for <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-cleardepthstencilview">ID3D12GraphicsCommandList::ClearDepthStencilView</a> when the flags (see <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_clear_flags">D3D12_CLEAR_FLAGS</a>) indicate a given subresource should be cleared (otherwise the subresource state doesn't matter), or when using it in a writable depth stencil view (see <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_dsv_flags">D3D12_DSV_FLAGS</a>) when the PSO has depth write enabled (see <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_desc">D3D12_DEPTH_STENCIL_DESC</a>).</summary>
	DepthWrite = 0x00000010,
	/// <summary>DEPTH_READ is a state that can be combined with other states. It should be used when the subresource is in a read-only depth stencil view, or when depth write of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_desc">D3D12_DEPTH_STENCIL_DESC</a> is disabled. It can be combined with other read states (for example, **PIXEL_SHADER_RESOURCE**), such that the resource can be used for the depth or stencil test, and accessed by a shader within the same draw call. Using it when depth will be written by a draw call or clear command is invalid.</summary>
	DepthRead = 0x00000020,
	/// <summary>The resource is used with a shader other than the pixel shader. A subresource must be in this state before being read by any stage (except for the pixel shader stage) via a shader resource view. You can still use the resource in a pixel shader with this flag as long as it also has the flag **PIXEL_SHADER_RESOURCE** set. This is a read-only state.</summary>
	NonPixelShaderResource = 0x00000040,
	/// <summary>The resource is used with a pixel shader. A subresource must be in this state before being read by the pixel shader via a shader resource view. This is a read-only state.</summary>
	PixelShaderResource = 0x00000080,
	/// <summary>The resource is used with stream output. A subresource must be in this state when it is accessed by the 3D pipeline as a stream-out target. This is a write-only state.</summary>
	StreamOut = 0x00000100,
	/// <summary>
	/// <para>The resource is used as an indirect argument. Subresources must be in this state when they are used as the argument buffer passed to the indirect drawing method <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executeindirect">ID3D12GraphicsCommandList::ExecuteIndirect</a>. This is a read-only state.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	IndirectArgument = 0x00000200,
	/// <summary>
	/// <para>The resource is used as the destination in a copy operation. Subresources must be in this state when they are used as the destination of copy operation, or a blt operation. This is a write-only state.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	CopyDest = 0x00000400,
	/// <summary>
	/// <para>The resource is used as the source in a copy operation. Subresources must be in this state when they are used as the source of copy operation, or a blt operation. This is a read-only state.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	CopySource = 0x00000800,
	/// <summary>The resource is used as the destination in a resolve operation.</summary>
	ResolveDest = 0x00001000,
	/// <summary>The resource is used as the source in a resolve operation.</summary>
	ResolveSource = 0x00002000,
	/// <summary>
	/// <para>When a buffer is created with this as its initial state, it indicates that the resource is a raytracing acceleration structure, for use in <a href="nf-d3d12-id3d12graphicscommandlist4-buildraytracingaccelerationstructure.md">ID3D12GraphicsCommandList4::BuildRaytracingAccelerationStructure</a>, <a href="nf-d3d12-id3d12graphicscommandlist4-copyraytracingaccelerationstructure.md">ID3D12GraphicsCommandList4::CopyRaytracingAccelerationStructure</a>, or <a href="nf-d3d12-id3d12device-createshaderresourceview.md">ID3D12Device::CreateShaderResourceView</a> for the <a href="ne-d3d12-d3d12_srv_dimension.md">D3D12_SRV_DIMENSION_RAYTRACING_ACCELERATION_STRUCTURE</a> dimension. > [!NOTE] > A resource to be used for the **RAYTRACING_ACCELERATION_STRUCTURE** state must be created in that state, and then never transitioned out of it. Nor may a resource that was created not in that state be transitioned into it. For more info, see [Acceleration structure memory restrictions](https://microsoft.github.io/DirectX-Specs/d3d/Raytracing.html#acceleration-structure-memory-restrictions) in the DirectX raytracing (DXR) functional specification on GitHub.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	RaytracingAccelerationStructure = 0x00400000,
	/// <summary>Starting with Windows 10, version 1903 (10.0; Build 18362), indicates that the resource is a screen-space shading-rate image for variable-rate shading (VRS). For more info, see <a href="https://docs.microsoft.com/windows/win32/direct3d12/vrs">Variable-rate shading (VRS)</a>.</summary>
	ShadingRateSource = 0x01000000,
	/// <summary>GENERIC_READ is a logically OR'd combination of other read-state bits. This is the required starting state for an upload heap. Your application should generally avoid transitioning to GENERIC_READ when possible, since that can result in premature cache flushes, or resource layout changes (for example, compress/decompress), causing unnecessary pipeline stalls. You should instead transition resources only to the actually-used states.</summary>
	GenericRead = 0x00000AC3,
	/// <summary>Equivalent to `NON_PIXEL_SHADER_RESOURCE | PIXEL_SHADER_RESOURCE`.</summary>
	AllShaderResource = 0x000000C0,
	/// <summary>Synonymous with COMMON.</summary>
	Present = 0x00000000,
	/// <summary>The resource is used for <a href="https://docs.microsoft.com/windows/win32/direct3d12/predication">Predication</a>.</summary>
	Predication = 0x00000200,
	/// <summary>The resource is used as a source in a decode operation. Examples include reading the compressed bitstream and reading from decode references,</summary>
	VideoDecodeRead = 0x00010000,
	/// <summary>The resource is used as a destination in the decode operation. This state is used for decode output and histograms.</summary>
	VideoDecodeWrite = 0x00020000,
	/// <summary>The resource is used to read video data during video processing; that is, the resource is used as the source in a processing operation such as video encoding (compression).</summary>
	VideoProcessRead = 0x00040000,
	/// <summary>The resource is used to write video data during video processing; that is, the resource is used as the destination in a processing operation such as video encoding (compression).</summary>
	VideoProcessWrite = 0x00080000,
	/// <summary>The resource is used as the source in an encode operation. This state is used for the input and reference of motion estimation.</summary>
	VideoEncodeRead = 0x00200000,
	/// <summary>This resource is used as the destination in an encode operation. This state is used for the destination texture of a resolve motion vector heap operation.</summary>
	VideoEncodeWrite = 0x00800000,
} ;


[EquivalentOf( typeof( D3D12_RESOURCE_BARRIER_TYPE ) )]
public enum ResourceBarrierType {
	/// <summary>A transition barrier that indicates a transition of a set of subresources between different usages. The caller must specify the before and after usages of the subresources.</summary>
	Transition = 0,
	/// <summary>An aliasing barrier that indicates a transition between usages of 2 different resources that have mappings into the same tile pool. The caller can specify both the before and the after resource. Note that one or both resources can be <b>NULL</b>, which indicates that any tiled resource could cause aliasing.</summary>
	Aliasing = 1,
	/// <summary>An unordered access view (UAV) barrier that indicates all UAV accesses (reads or writes) to a particular resource must complete before any future UAV accesses (read or write) can begin.</summary>
	UAV = 2,
} ;


[EquivalentOf( typeof( D3D12_RESOURCE_BARRIER_FLAGS ) )]
public enum ResourceBarrierFlags {
	/// <summary>No flags.</summary>
	None = 0x00000000,
	/// <summary>This starts a barrier transition in a new state, putting a resource in a temporary no-access condition.</summary>
	BeginOnly = 0x00000001,
	/// <summary>This barrier completes a transition, setting a new state and restoring active access to a resource.</summary>
	EndOnly = 0x00000002,
} ;


[Flags, EquivalentOf( typeof( D3D12_FENCE_FLAGS ) )]
public enum FenceFlags {
	/// <summary>No options are specified.</summary>
	None = 0x00000000,
	/// <summary>The fence is shared.</summary>
	Shared = 0x00000001,
	/// <summary>The fence is shared with another GPU adapter.</summary>
	SharedCrossAdapter = 0x00000002,
	/// <summary>The fence is of the non-monitored type. Non-monitored fences should only be used when the adapter doesn't support monitored fences, or when a fence is shared with an adapter that doesn't support monitored fences.</summary>
	NonMonitored = 0x00000004,
} ;


[EquivalentOf( typeof( D3D12_QUERY_HEAP_TYPE ) )]
public enum QueryHeapType {
	/// <summary>This returns a binary 0/1 result:  0 indicates that no samples passed depth and stencil testing, 1 indicates that at least one sample passed depth and stencil testing.  This enables occlusion queries to not interfere with any GPU performance optimization associated with depth/stencil testing.</summary>
	Occlusion = 0,

	/// <summary>Indicates that the heap is for high-performance timing data.</summary>
	Timestamp = 1,

	/// <summary>Indicates the heap is to contain pipeline data. Refer to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_query_data_pipeline_statistics">D3D12_QUERY_DATA_PIPELINE_STATISTICS</a>.</summary>
	PipelineStatistics = 2,

	/// <summary>Indicates the heap is to contain stream output data. Refer to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_query_data_so_statistics">D3D12_QUERY_DATA_SO_STATISTICS</a>.</summary>
	StreamOutputStatistics = 3,

	/// <summary>
	/// <para>Indicates the heap is to contain video decode statistics data. Refer to [D3D12_QUERY_DATA_VIDEO_DECODE_STATISTICS](../d3d12video/ns-d3d12video-d3d12_query_data_video_decode_statistics.md). Video decode statistics can only be queried from video decode command lists (<a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_command_list_type">D3D12_COMMAND_LIST_TYPE_VIDEO_DECODE</a>). See <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_type">D3D12_QUERY_TYPE_DECODE_STATISTICS</a> for more details.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_heap_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	VideoDecodeStatistics = 4,

	/// <summary>
	/// <para>Indicates the heap is to contain timestamp queries emitted exclusively by copy command lists. Copy queue timestamps can only be queried from a copy command list, and a copy command list can not emit to a regular timestamp query Heap. Support for this query heap type is not universal. You must use <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a> with [D3D12_FEATURE_D3D12_OPTIONS3](./ne-d3d12-d3d12_feature.md) to determine whether the adapter supports copy queue timestamp queries.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_heap_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	CopyQueueTimestamp = 5,
	PipelineStatistics1 = 7,
} ;


[EquivalentOf( typeof(D3D12_QUERY_TYPE) )]
public enum QueryType {
	/// <summary>Indicates the query is for depth/stencil occlusion counts.</summary>
	Occlusion = 0,

	/// <summary>
	/// <para>Indicates the query is for a binary depth/stencil occlusion statistics. This new query type acts like OCCLUSION except that it returns simply a binary 0/1 result:  0 indicates that no samples passed depth and stencil testing, 1 indicates that at least one sample passed depth and stencil testing.  This enables occlusion queries to not interfere with any GPU performance optimization associated with depth/stencil testing.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	BinaryOcclusion = 1,

	/// <summary>Indicates the query is for high definition GPU and CPU timestamps.</summary>
	Timestamp = 2,

	/// <summary>Indicates the query type is for graphics pipeline statistics, refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_query_data_pipeline_statistics">D3D12_QUERY_DATA_PIPELINE_STATISTICS</a>.</summary>
	PipelineStatistics = 3,

	/// <summary>Stream 0 output statistics. In Direct3D 12 there is no single stream output (SO) overflow query for all the output streams. Apps need to issue multiple single-stream queries, and then correlate the results. Stream output is the ability of the GPU to write vertices to a buffer. The stream output counters monitor progress.</summary>
	OutputStatisticsStream0 = 4,

	/// <summary>Stream 1 output statistics.</summary>
	OutputStatisticsStream1 = 5,

	/// <summary>Stream 2 output statistics.</summary>
	OutputStatisticsStream2 = 6,

	/// <summary>Stream 3 output statistics.</summary>
	OutputStatisticsStream3 = 7,

	/// <summary>
	/// <para>Video decode statistics. Refer to [D3D12_QUERY_DATA_VIDEO_DECODE_STATISTICS](../d3d12video/ns-d3d12video-d3d12_query_data_video_decode_statistics.md). Use this query type to determine if a video was successfully decoded. If decoding fails due to insufficient BitRate or FrameRate parameters set during creation of the decode heap, then the status field of the query is set to [D3D12_VIDEO_DECODE_STATUS_RATE_EXCEEDED](../d3d12video/ne-d3d12video-d3d12_video_decode_status.md) and the query also contains new BitRate and FrameRate values that would succeed. This query type can only be performed on video decode command lists [(D3D12_COMMAND_LIST_TYPE_VIDEO_DECODE)](/windows/desktop/api/d3d12/ne-d3d12-d3d12_command_list_type). This query type does not use [ID3D12VideoDecodeCommandList::BeginQuery](../d3d12video/nf-d3d12video-id3d12videodecodecommandlist-beginquery.md), only [ID3D12VideoDecodeCommandList::EndQuery](../d3d12video/nf-d3d12video-id3d12videodecodecommandlist-endquery.md). Statistics are recorded only for the most recent [ID3D12VideoDecodeCommandList::DecodeFrame](../d3d12video/nf-d3d12video-id3d12videodecodecommandlist-decodeframe.md) call in the same command list. Decode status structures are defined by the codec specification.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	VideoDecodeStatistics = 8,
	PipelineStatistics1 = 10,
} ;


[EquivalentOf( typeof( D3D12_PREDICATION_OP ) )]
public enum PredicationOp {
	/// <summary>Enables predication if all 64-bits are zero.</summary>
	EqualsZero = 0,
	/// <summary>Enables predication if at least one of the 64-bits are not zero.</summary>
	NotEqualsZero = 1,
} ;


/// <summary>
/// Specifies how the pipeline interprets geometry or hull shader input primitives.
/// </summary>
/// <remarks>
/// <seealso cref="PrimitiveTopology"/>
/// For more info, see: 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/ne-d3d12-d3d12_primitive_topology_type">
/// D3D12_PRIMITIVE_TOPOLOGY_TYPE 
/// </a>enumeration (defined in: <b><i>d3d12.h</i></b>)
/// </remarks>
[EquivalentOf( typeof(D3D12_PRIMITIVE_TOPOLOGY_TYPE) )]
public enum PrimitiveTopologyType {
	/// <summary>The shader has not been initialized with an input primitive type.</summary>
	Undefined = 0,
	/// <summary>Interpret the input primitive as a point.</summary>
	Point = 1,
	/// <summary>Interpret the input primitive as a line.</summary>
	Line = 2,
	/// <summary>Interpret the input primitive as a triangle.</summary>
	Triangle = 3,
	/// <summary>Interpret the input primitive as a control point patch.</summary>
	Patch = 4,
} ;



[EquivalentOf( typeof( D3D12_INDEX_BUFFER_STRIP_CUT_VALUE ) )]
public enum IndexBufferStripCutValue {
	/// <summary>Indicates that there is no cut value.</summary>
	Disabled    = 0,
	/// <summary>Indicates that 0xFFFF should be used as the cut value.</summary>
	Cut_0xFFFF     = 1,
	/// <summary>Indicates that 0xFFFFFFFF should be used as the cut value.</summary>
	Cut_0xFFFFFFFF = 2,
} ;


[EquivalentOf( typeof( D3D12_DEPTH_WRITE_MASK ) )]
public enum DepthWriteMask {
	/// <summary>Turn off writes to the depth-stencil buffer.</summary>
	Zero = 0,
	/// <summary>Turn on writes to the depth-stencil buffer.</summary>
	All = 1,
} ;


[EquivalentOf( typeof( D3D12_STENCIL_OP ) )]
public enum StencilOperation {
	/// <summary>Keep the existing stencil data.</summary>
	Keep = 1,
	/// <summary>Set the stencil data to 0.</summary>
	Zero = 2,
	/// <summary>Set the stencil data to the reference value set by calling <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetstencilref">ID3D12GraphicsCommandList::OMSetStencilRef</a>.</summary>
	Replace = 3,
	/// <summary>Increment the stencil value by 1, and clamp the result.</summary>
	IncrementSaturate = 4,
	/// <summary>Decrement the stencil value by 1, and clamp the result.</summary>
	Saturate = 5,
	/// <summary>Invert the stencil data.</summary>
	Invert = 6,
	/// <summary>Increment the stencil value by 1, and wrap the result if necessary.</summary>
	Increment = 7,
	/// <summary>Decrement the stencil value by 1, and wrap the result if necessary.</summary>
	Decrement = 8,
} ;


[EquivalentOf( typeof( D3D12_BLEND ) )]
public enum Blend {
	/// <summary>The blend factor is (0, 0, 0, 0). No pre-blend operation.</summary>
	Zero = 1,

	/// <summary>The blend factor is (1, 1, 1, 1). No pre-blend operation.</summary>
	One = 2,

	/// <summary>The blend factor is (Rₛ, Gₛ, Bₛ, Aₛ), that is color data (RGB) from a pixel shader. No pre-blend operation.</summary>
	SrcColor = 3,

	/// <summary>The blend factor is (1 - Rₛ, 1 - Gₛ, 1 - Bₛ, 1 - Aₛ), that is color data (RGB) from a pixel shader. The pre-blend operation inverts the data, generating 1 - RGB.</summary>
	InvSrcColor = 4,

	/// <summary>The blend factor is (Aₛ, Aₛ, Aₛ, Aₛ), that is alpha data (A) from a pixel shader. No pre-blend operation.</summary>
	SrcAlpha = 5,

	/// <summary>The blend factor is ( 1 - Aₛ, 1 - Aₛ, 1 - Aₛ, 1 - Aₛ), that is alpha data (A) from a pixel shader. The pre-blend operation inverts the data, generating 1 - A.</summary>
	InvSrcAlpha = 6,

	/// <summary>The blend factor is (A<sub>d</sub> A<sub>d</sub> A<sub>d</sub> A<sub>d</sub>), that is alpha data from a render target. No pre-blend operation.</summary>
	DestAlpha = 7,

	/// <summary>The blend factor is (1 - A<sub>d</sub> 1 - A<sub>d</sub> 1 - A<sub>d</sub> 1 - A<sub>d</sub>), that is alpha data from a render target. The pre-blend operation inverts the data, generating 1 - A.</summary>
	InvDestAlpha = 8,

	/// <summary>The blend factor is (R<sub>d</sub>, G<sub>d</sub>, B<sub>d</sub>, A<sub>d</sub>), that is color data from a render target. No pre-blend operation.</summary>
	DestColor = 9,

	/// <summary>The blend factor is (1 - R<sub>d</sub>, 1 - G<sub>d</sub>, 1 - B<sub>d</sub>, 1 - A<sub>d</sub>), that is color data from a render target. The pre-blend operation inverts the data, generating 1 - RGB.</summary>
	InvDestColor = 10,

	/// <summary>
	/// <para>The blend factor is (f, f, f, 1); where f = min(Aₛ, 1 - A<sub>d</sub>). The pre-blend operation clamps the data to 1 or less.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_blend#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	SrcAlphaSat = 11,

	/// <summary>The blend factor is the blend factor set with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor">ID3D12GraphicsCommandList::OMSetBlendFactor</a>. No pre-blend operation.</summary>
	BlendFactor = 14,

	/// <summary>The blend factor is the blend factor set with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor">ID3D12GraphicsCommandList::OMSetBlendFactor</a>. The pre-blend operation inverts the blend factor, generating 1 - blend_factor.</summary>
	InvBlendFactor = 15,

	/// <summary>The blend factor is data sources both as color data output by a pixel shader. There is no pre-blend operation. This blend factor supports dual-source color blending.</summary>
	Src1Color = 16,

	/// <summary>The blend factor is data sources both as color data output by a pixel shader. The pre-blend operation inverts the data, generating 1 - RGB. This blend factor supports dual-source color blending.</summary>
	InvSrc1Color = 17,

	/// <summary>The blend factor is data sources as alpha data output by a pixel shader. There is no pre-blend operation. This blend factor supports dual-source color blending.</summary>
	Src1Alpha = 18,

	/// <summary>The blend factor is data sources as alpha data output by a pixel shader. The pre-blend operation inverts the data, generating 1 - A. This blend factor supports dual-source color blending.</summary>
	InvSrc1Alpha = 19,

	/// <summary>
	/// <para>The blend factor is (A, A, A, A), where the constant, A, is taken from the blend factor set with [OMSetBlendFactor](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor). To successfully use this constant on a target machine, the [D3D12_FEATURE_DATA_D3D12_OPTIONS13](ns-d3d12-d3d12_feature_data_d3d12_options13.md) returned from [capability querying](/windows/win32/direct3d12/capability-querying) must have its *AlphaBlendFactorSupported* set to `TRUE`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_blend#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	AlphaFactor = 20,

	/// <summary>
	/// <para>The blend factor is (1 – A, 1 – A, 1 – A, 1 – A), where the constant, A, is taken from the blend factor set with [OMSetBlendFactor](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor). To successfully use this constant on a target machine, the [D3D12_FEATURE_DATA_D3D12_OPTIONS13](ns-d3d12-d3d12_feature_data_d3d12_options13.md) returned from [capability querying](/windows/win32/direct3d12/capability-querying) must have its *AlphaBlendFactorSupported* set to `TRUE`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_blend#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	InvAlphaFactor = 21,
} ;


[EquivalentOf( typeof( D3D12_BLEND_OP ) )]
public enum BlendOperation {
	/// <summary>Add source 1 and source 2.</summary>
	Add = 1,
	/// <summary>Subtract source 1 from source 2.</summary>
	Subtract = 2,
	/// <summary>Subtract source 2 from source 1.</summary>
	ReverseSubtract = 3,
	/// <summary>Find the minimum of source 1 and source 2.</summary>
	Min = 4,
	/// <summary>Find the maximum of source 1 and source 2.</summary>
	Max = 5,
} ;


[EquivalentOf( typeof( D3D12_LOGIC_OP ) )]
public enum LogicOperation {
	/// <summary>Set the logical operation to CLEAR.</summary>
	Clear = 0,
	/// <summary>Set the logical operation to SET.</summary>
	Set = 1,
	/// <summary>Set the logical operation to COPY.</summary>
	Copy = 2,
	/// <summary>Set the logical operation to COPY_INVERTED.</summary>
	CopyInverted = 3,
	/// <summary>Set the logical operation to NOOP.</summary>
	NOOP = 4,
	/// <summary>Set the logical operation to INVERT.</summary>
	Invert = 5,
	/// <summary>Set the logical operation to AND.</summary>
	AND = 6,
	/// <summary>Set the logical operation to NAND.</summary>
	NAND = 7,
	/// <summary>Set the logical operation to OR.</summary>
	OR = 8,
	/// <summary>Set the logical operation to NOR.</summary>
	NOR = 9,
	/// <summary>Set the logical operation to XOR.</summary>
	XOR = 10,
	/// <summary>Set the logical operation to EQUIV.</summary>
	Equiv = 11,
	/// <summary>Set the logical operation to AND_REVERSE.</summary>
	ANDReverse = 12,
	/// <summary>Set the logical operation to AND_INVERTED.</summary>
	ANDInverted = 13,
	/// <summary>Set the logical operation to OR_REVERSE.</summary>
	ORReverse = 14,
	/// <summary>Set the logical operation to OR_INVERTED.</summary>
	ORInverted = 15,
} ;


[EquivalentOf( typeof( D3D12_FILL_MODE ) )]
public enum FillMode {
	/// <summary>Draw lines connecting the vertices. Adjacent vertices are not drawn.</summary>
	Wireframe = 2,
	/// <summary>Fill the triangles formed by the vertices. Adjacent vertices are not drawn.</summary>
	Solid = 3,
} ;


[EquivalentOf( typeof( D3D12_CULL_MODE ) )]
public enum CullMode {
	/// <summary>Always draw all triangles.</summary>
	None = 1,
	/// <summary>Do not draw triangles that are front-facing.</summary>
	Front = 2,
	/// <summary>Do not draw triangles that are back-facing.</summary>
	Back = 3,
} ;


[EquivalentOf( typeof( D3D12_CONSERVATIVE_RASTERIZATION_MODE ) )]
public enum ConservativeRasterizationMode {
	/// <summary>Conservative rasterization is off.</summary>
	Off = 0,
	/// <summary>Conservative rasterization is on.</summary>
	On = 1,
} ;


[EquivalentOf( typeof( D3D12_INPUT_CLASSIFICATION ) )]
public enum InputClassification {
	/// <summary>Input data is per-vertex data.</summary>
	PerVertexData = 0,

	/// <summary>Input data is per-instance data.</summary>
	PerInstanceData = 1,
} ;


[EquivalentOf( typeof(D3D12_CPU_PAGE_PROPERTY) )]
public enum CpuPageProperty {
	/// <summary>The CPU-page property is unknown.</summary>
	Unknown = 0,
	/// <summary>The CPU cannot access the heap, therefore no page properties are available.</summary>
	NotAvailable = 1,
	/// <summary>The CPU-page property is write-combined.</summary>
	WriteCombine = 2,
	/// <summary>The CPU-page property is write-back.</summary>
	WriteBack = 3,
} ;


[EquivalentOf( typeof(D3D12_MEMORY_POOL) )]
public enum MemoryPool {
	/// <summary>The memory pool is unknown.</summary>
	Unknown = 0,
	/// <summary>
	/// <para>The memory pool is L0. L0 is the physical system memory pool. When the adapter is discrete/NUMA, this pool has greater bandwidth for the CPU and less bandwidth for the GPU. When the adapter is UMA, this pool is the only one which is valid.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_memory_pool#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	L0 = 1,
	/// <summary>
	/// <para>The memory pool is L1. L1 is typically known as the physical video memory pool. L1 is only available when the adapter is discrete/NUMA, and has greater bandwidth for the GPU and cannot even be accessed by the CPU. When the adapter is UMA, this pool is not available.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_memory_pool#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	L1 = 2,
} ;


[EquivalentOf( typeof(D3D12_INDIRECT_ARGUMENT_TYPE) )]
public enum IndirectArgumentType {
	/// <summary>Indicates the type is a Draw call.</summary>
	Draw = 0,
	/// <summary>Indicates the type is a DrawIndexed call.</summary>
	DrawIndexed = 1,
	/// <summary>Indicates the type is a Dispatch call.</summary>
	Dispatch = 2,
	/// <summary>Indicates the type is a vertex buffer view.</summary>
	VertexBufferView = 3,
	/// <summary>Indicates the type is an index buffer view.</summary>
	IndexBufferView = 4,
	/// <summary>Indicates the type is a constant.</summary>
	Constant = 5,
	/// <summary>Indicates the type is a constant buffer view (CBV).</summary>
	ConstantBufferView = 6,
	/// <summary>Indicates the type is a shader resource view (SRV).</summary>
	ShaderResourceView = 7,
	/// <summary>Indicates the type is an unordered access view (UAV).</summary>
	UnorderedAccessView = 8,
	DispatchRays = 9,
	DispatchMesh = 10,
} ;


//!!! ------------------------------------------------------------------------------------------------ !!!//


/// <summary>Identifies the tier level at which tiled resources are supported.</summary>
/// <remarks>For more information, see:
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/ne-d3d12-d3d12_tiled_resources_tier">
/// <b>D3D12_TILED_RESOURCES_TIER</b> enumeration (<i>d3d12.h</i>)</a>.</remarks>
[EquivalentOf( typeof( D3D12_TILED_RESOURCES_TIER ) )]
public enum TiledResourcesTier {
	/// <summary>
	/// <para>Indicates that textures cannot be created with the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_layout">D3D12_TEXTURE_LAYOUT_64KB_UNDEFINED_SWIZZLE</a> layout.</para>
	/// <para><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createreservedresource">ID3D12Device::CreateReservedResource</a> cannot be used, not even for buffers.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_tiled_resources_tier#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	NotSupported = 0,
	/// <summary>
	/// <para>Indicates that 2D textures can be created with the D3D12_TEXTURE_LAYOUT_64KB_UNDEFINED_SWIZZLE layout. Limitations exist for certain resource formats and properties. For more details, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_layout">D3D12_TEXTURE_LAYOUT_64KB_UNDEFINED_SWIZZLE</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createreservedresource">ID3D12Device::CreateReservedResource</a> can be used.</para>
	/// <para>GPU reads or writes to NULL mappings are undefined. Applications are encouraged to workaround this limitation by repeatedly mapping the same page to everywhere a NULL mapping would've been used.</para>
	/// <para>When the size of a texture mipmap level is an integer multiple of the standard tile shape for its format, it is guaranteed to be nonpacked.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_tiled_resources_tier#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	Tier1 = 1,
	/// <summary>
	/// <para>Indicates that a superset of Tier_1 functionality is supported, including this additional support:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_tiled_resources_tier#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	Tier2 = 2,
	/// <summary>Indicates that a superset of Tier 2 is supported, with the addition that 3D textures (<a href="https://docs.microsoft.com/windows/desktop/direct3d12/volume-tiled-resources">Volume Tiled Resources</a>) are supported.</summary>
	Tier3 = 3,
	/// <summary></summary>
	Tier4 = 4,
} ;

//!!! ------------------------------------------------------------------------------------------------ !!!//


/// <summary>Specifies how to copy a tile.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytiles">CopyTiles</a> method.</remarks>
[Flags, EquivalentOf( typeof( D3D12_TILE_COPY_FLAGS ) )]
public enum TileCopyFlags {
	/// <summary>No tile-copy flags are specified.</summary>
	None = 0x00000000,

	/// <summary>
	/// <para>Indicates that the GPU isn't currently referencing any of the portions of destination memory being written.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_tile_copy_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	NoHazard = 0x00000001,

	/// <summary>
	/// <para>Indicates that the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytiles">ID3D12GraphicsCommandList::CopyTiles</a> operation involves copying a linear buffer to a swizzled tiled resource. This means to copy tile data from the specified buffer location, reading tiles sequentially, to the specified tile region (in x,y,z order if the region is a box), swizzling to optimal hardware memory layout as needed. In this <b>ID3D12GraphicsCommandList::CopyTiles</b> call, you specify the source data with the  <i>pBuffer</i> parameter and the destination with the <i>pTiledResource</i> parameter.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_tile_copy_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	LinearBufferToSwizzledTiledResource = 0x00000002,

	/// <summary>
	/// <para>Indicates that the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytiles">ID3D12GraphicsCommandList::CopyTiles</a> operation involves copying a swizzled tiled resource to a linear buffer. This means to copy tile data from the tile region, reading tiles sequentially (in x,y,z order if the region is a box), to the specified buffer location, deswizzling to linear memory layout as needed. In this <b>ID3D12GraphicsCommandList::CopyTiles</b> call, you specify the source data with the <i>pTiledResource</i> parameter and the destination with the  <i>pBuffer</i> parameter.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_tile_copy_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	SwizzledTiledResourceToLinearBuffer = 0x00000004,
} ;



/// <summary>Identifies which components of each pixel of a render target are writable during blending.</summary>
/// <remarks>
/// This enum is used in the <see cref="RTBlendDescription"/> property.
/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_target_blend_desc">D3D12_RENDER_TARGET_BLEND_DESC</a> structure.</remarks>
[EquivalentOf( typeof( D3D12_COLOR_WRITE_ENABLE ) )]
public enum ColorWriteEnableFlags: byte {
	/// <summary>Allow data to be stored in the red component.</summary>
	Red = 1,

	/// <summary>Allow data to be stored in the green component.</summary>
	Green = 2,

	/// <summary>Allow data to be stored in the blue component.</summary>
	Blue = 4,

	/// <summary>Allow data to be stored in the alpha component.</summary>
	Alpha = 8,

	/// <summary>Allow data to be stored in all components.</summary>
	All = 15,
} ;


/// <summary>Defines constants that specify the lifetime state of a lifetime-tracked object.</summary>
[EquivalentOf(typeof(D3D12_LIFETIME_STATE))]
public enum LifetimeState {
	/// <summary>Specifies that the lifetime-tracked object is in use.</summary>
	InUse = 0,
	/// <summary>Specifies that the lifetime-tracked object is not in use.</summary>
	NotInUse = 1,
} ;


/// <summary>Specifies a resolve operation.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-resolvesubresourceregion">ID3D12GraphicsCommandList1::ResolveSubresourceRegion</a> function.</remarks>
[EquivalentOf(typeof(D3D12_RESOLVE_MODE))]
public enum ResolveMode {
	/// <summary>Resolves compressed source samples to their uncompressed values. When using this operation, the source and destination resources must have the same sample count, unlike the min, max, and average operations that require the destination to have a sample count of 1.</summary>
	Decompress = 0,
	/// <summary>Resolves the source samples to their minimum value. It can be used with any render target or depth stencil format.</summary>
	Min = 1,
	/// <summary>Resolves the source samples to their maximum value. It can be used with any render target or depth stencil format.</summary>
	Max = 2,
	/// <summary>Resolves the source samples to their average value. It can be used with any non-integer render target format, including the depth plane. It can't be used with integer render target formats, including the stencil plane.</summary>
	Average = 3,
	
	EncodeSamplerFeedback = 4,
	DecodeSamplerFeedback = 5,
} ;


/// <summary>Specifies the mode used by a WriteBufferImmediate operation.</summary>
/// <remarks>
/// <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_writebufferimmediate_mode">Learn more about this API from docs.microsoft.com</a>.
/// </remarks>
[EquivalentOf(typeof(D3D12_WRITEBUFFERIMMEDIATE_MODE))]
public enum WriteBufferImmediateMode {
	/// <summary>The write operation behaves the same as normal copy-write operations.</summary>
	Default = 0,
	/// <summary>The write operation is guaranteed to occur after all preceding commands in the command stream have started, including previous <b>WriteBufferImmediate</b> operations.</summary>
	MarkerIn = 1,
	/// <summary>The write operation is deferred until all previous commands in the command stream have completed through the GPU pipeline, including previous <b>WriteBufferImmediate</b> operations. Write operations that specify <b>MARKER_OUT</b> don't block subsequent operations from starting. If there are no previous operations in the command stream, then the write operation behaves as if <b>MARKER_IN</b> was specified.</summary>
	MarkerOut = 2,
} ;


/// <summary>Defines constants that specify protected session status.</summary>
[EquivalentOf(typeof(D3D12_PROTECTED_SESSION_STATUS))]
public enum ProtectedSessionStatus {
	/// <summary>Indicates that the protected session is in a valid state.</summary>
	OK = 0,
	/// <summary>Indicates that the protected session is not in a valid state.</summary>
	Invalid = 1,
} ;


/// <summary>Defines constants that specify protected resource session flags.</summary>
/// <remarks>Doesn't have any actual values: May be used in API updates ...</remarks>
[Flags, EquivalentOf( typeof( D3D12_PROTECTED_RESOURCE_SESSION_FLAGS ) )]
public enum ProtectedResourceSessionFlags {
	/// <summary>Specifies no flag.</summary>
	None = 0x00000000,
} ;


/// <summary>Defines constants that specify the stage of a parameter to a meta command.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_meta_command_parameter_stage">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_META_COMMAND_PARAMETER_STAGE))]
public enum MetaCommandParameterStage {
	/// <summary>Specifies that the parameter is used at the meta command creation stage.</summary>
	Creation = 0,
	/// <summary>Specifies that the parameter is used at the meta command initialization stage.</summary>
	Initialization = 1,
	/// <summary>Specifies that the parameter is used at the meta command execution stage.</summary>
	Execution = 2,
} ;


/// <summary>Defines constants that specify the flags for a parameter to a meta command. Values can be bitwise OR'd together.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_meta_command_parameter_flags">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[Flags, EquivalentOf(typeof(D3D12_META_COMMAND_PARAMETER_FLAGS))]
public enum MetaCommandParameterFlags {
	/// <summary>Specifies that the parameter is an input resource.</summary>
	Input = 0x00000001,
	/// <summary>Specifies that the parameter is an output resource.</summary>
	Output = 0x00000002,
} ;


/// <summary>Specifies the type of access that an application is given to the specified resource(s) at the transition into a render pass.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_render_pass_beginning_access_type">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_RENDER_PASS_BEGINNING_ACCESS_TYPE))]
public enum RenderPassBeginningAccessType {
	/// <summary>
	/// <para>Indicates that your application doesn't have any dependency on the prior contents of the resource(s). You also shouldn't have any expectations about those contents, because a display driver may return the previously-written contents, or it may return uninitialized data. You can be assured that reading from the resource(s) won't hang the GPU, even if you do get undefined data back. A read is defined as a traditional read from an unordered access view (UAV), a shader resource view (SRV), a constant buffer view (CBV), a vertex buffer view (VBV), an index buffer view (IBV), an IndirectArg binding/read, or a blend/depth-testing-induced read.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_render_pass_beginning_access_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Discard = 0,
	/// <summary>Indicates that your application has a dependency on the prior contents of the resource(s), so the contents must be loaded from main memory.</summary>
	Preserve = 1,
	/// <summary>
	/// <para>Indicates that your application needs the resource(s) to be cleared to a specific value (a value that your application specifies). This clear occurs whether or not you interact with the resource(s) during the render pass. You specify the clear value at <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-beginrenderpass">BeginRenderPass</a> time, in the <b>Clear</b> member of your <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_pass_beginning_access">D3D12_RENDER_PASS_BEGINNING_ACCESS</a> structure.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_render_pass_beginning_access_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Clear = 2,
	/// <summary>Indicates that your application will neither read from nor write  to the resource(s) during the render pass. You would most likely use this value to indicate that you won't be accessing the depth/stencil plane for a depth/stencil view (DSV). You must pair this value with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_render_pass_ending_access_type">D3D12_RENDER_PASS_ENDING_ACCESS_TYPE_NO_ACCESS</a> in the corresponding <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_pass_ending_access">D3D12_RENDER_PASS_ENDING_ACCESS</a> structure.</summary>
	NoAccess = 3,
	PreserveLocalRender = 4,
	PreserveLocalSRV = 5,
	PreserveLocalUAV = 6,
} ;


/// <summary>Specifies the type of access that an application is given to the specified resource(s) at the transition out of a render pass.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_render_pass_ending_access_type">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_RENDER_PASS_ENDING_ACCESS_TYPE ) )]
public enum RenderPassEndingAccessType {
	/// <summary>Indicates that your application won't have any future dependency on any data that you wrote to the resource(s) during this render pass. For example, a depth buffer that won't be textured from before it's written to again.</summary>
	Discard = 0,
	/// <summary>Indicates that your application will have a dependency on the written contents of the resource(s) in the future, and so they must be preserved.</summary>
	Preserve = 1,
	/// <summary>Indicates that the resource(s)—for example, a multi-sample anti-aliasing (MSAA) surface—should be directly resolved to a separate resource at the conclusion of the render pass. For a tile-based deferred renderer (TBDR), this should ideally happen while the MSAA contents are still in the tile cache. You should ensure that the resolve destination is in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_RESOLVE_DEST</a> resource state when the render pass ends. The resolve source is left in its initial resource state at the time the render pass ends.  A resolve operation submitted by a render pass doesn't implicitly change the state of any resource.</summary>
	Resolve = 2,
	/// <summary>Indicates that your application will neither read from nor write  to the resource(s) during the render pass. You would most likely use this value to indicate that you won't be accessing the depth/stencil plane for a depth/stencil view (DSV). You must pair this value with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_render_pass_beginning_access_type">D3D12_RENDER_PASS_BEGINNING_ACCESS_TYPE_NO_ACCESS</a> in the corresponding <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_pass_beginning_access">D3D12_RENDER_PASS_BEGINNING_ACCESS</a> structure.</summary>
	NoAccess = 3,
	PreserveLocalRender = 4,
	PreserveLocalSRV = 5,
	PreserveLocalUAV = 6,
} ;


/// <summary>Specifies the nature of the render pass; for example, whether it is a suspending or a resuming render pass.</summary>
/// <remarks><para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_render_pass_flags">Learn more about this API from docs.microsoft.com</a>.</para></remarks>
[EquivalentOf( typeof( D3D12_RENDER_PASS_FLAGS ) )]
public enum RenderPassFlags {
	/// <summary>Indicates that the render pass has no special requirements.</summary>
	None = 0x00000000,
	/// <summary>Indicates that writes to unordered access view(s) should be allowed during the render pass.</summary>
	AllowUAVWrites = 0x00000001,
	/// <summary>Indicates that this is a suspending render pass.</summary>
	SuspendingPass = 0x00000002,
	/// <summary>Indicates that this is a resuming render pass.</summary>
	ResumingPass = 0x00000004,
	BindReadOnlyDepth   = 0x00000008,
	BindReadOnlyStencil = 0x00000010,
} ;


/// <summary>Describes how the locations of elements are identified.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_elements_layout">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_ELEMENTS_LAYOUT))]
public enum ElementsLayout {
	/// <summary>For a data set of <i>n</i> elements, the pointer parameter points to the start of <i>n</i> elements in memory.</summary>
	Array = 0,
	/// <summary>For a data set of <i>n</i> elements, the pointer parameter points to an array of <i>n</i> pointers in memory, each pointing to an individual element of the set.</summary>
	ArrayOfPointers = 1,
} ;


/// <summary>Defines constants that specify the shading rate (for variable-rate shading, or VRS).</summary>
[EquivalentOf( typeof( D3D12_SHADING_RATE ) )]
public enum ShadingRate {
	/// <summary>Specifies no change to the shading rate.</summary>
	Rate_1X1 = 0,

	/// <summary>Specifies that the shading rate should reduce vertical resolution 2x.</summary>
	Rate_1X2 = 1,

	/// <summary>Specifies that the shading rate should reduce horizontal resolution 2x.</summary>
	Rate_2X1 = 4,

	/// <summary>Specifies that the shading rate should reduce the resolution of both axes 2x.</summary>
	Rate_2X2 = 5,

	/// <summary>Specifies that the shading rate should reduce horizontal resolution 2x, and reduce vertical resolution 4x.</summary>
	Rate_2X4 = 6,

	/// <summary>Specifies that the shading rate should reduce horizontal resolution 4x, and reduce vertical resolution 2x.</summary>
	Rate_4X2 = 9,

	/// <summary>Specifies that the shading rate should reduce the resolution of both axes 4x.</summary>
	Rate_4X4 = 10,
} ;


/// <summary>Defines constants that specify a shading rate combiner (for variable-rate shading, or VRS).</summary>
[EquivalentOf(typeof(D3D12_SHADING_RATE_COMBINER))]
public enum ShadingRateCombiner {
	/// <summary>Specifies the combiner `C.xy = A.xy`, for combiner (C) and inputs (A and B).</summary>
	Passthrough = 0,
	/// <summary>Specifies the combiner `C.xy = B.xy`, for combiner (C) and inputs (A and B).</summary>
	Override = 1,
	/// <summary>Specifies the combiner `C.xy = max(A.xy, B.xy)`, for combiner (C) and inputs (A and B).</summary>
	Min = 2,
	/// <summary>Specifies the combiner `C.xy = min(A.xy, B.xy)`, for combiner (C) and inputs (A and B).</summary>
	Max = 3,
	/// <summary>Specifies the combiner C.xy = min(maxRate, A.xy + B.xy)`, for combiner (C) and inputs (A and B).</summary>
	Sum = 4,
} ;


[EquivalentOf(typeof(D3D12_BARRIER_TYPE))]
public enum BarrierType {
	Global  = 0,
	Texture = 1,
	Buffer  = 2,
} ;


[Flags, EquivalentOf(typeof(D3D12_BARRIER_SYNC))]
public enum BarrierSync {
	None = 0x00000000,
	All = 0x00000001,
	Draw = 0x00000002,
	IndexInput = 0x00000004,
	VertexShading = 0x00000008,
	PixelShading = 0x00000010,
	DepthStencil = 0x00000020,
	RenderTarget = 0x00000040,
	ComputeShading = 0x00000080,
	Raytracing = 0x00000100,
	Copy = 0x00000200,
	Resolve = 0x00000400,
	ExecuteIndirect = 0x00000800,
	Predication = 0x00000800,
	AllShading = 0x00001000,
	NonPixelShading = 0x00002000,
	EmitRaytracingAccelerationStructurePostbuildInfo = 0x00004000,
	ClearUnorderedAccessView = 0x00008000,
	VideoDecode = 0x00100000,
	VideoProcess = 0x00200000,
	VideoEncode = 0x00400000,
	BuildRaytracingAccelerationStructure = 0x00800000,
	CopyRaytracingAccelerationStructure = 0x01000000,
	Split = unchecked((int)0x80000000),
} ;


[Flags, EquivalentOf(typeof(D3D12_BARRIER_ACCESS))]
public enum BarrierAccess {
	Common = 0x00000000,
	VertexBuffer = 0x00000001,
	ConstantBuffer = 0x00000002,
	IndexBuffer = 0x00000004,
	RenderTarget = 0x00000008,
	UnorderedAccess = 0x00000010,
	DepthStencilWrite = 0x00000020,
	DepthStencilRead = 0x00000040,
	ShaderResource = 0x00000080,
	StreamOutput = 0x00000100,
	IndirectArgument = 0x00000200,
	Predication = 0x00000200,
	CopyDest = 0x00000400,
	CopySource = 0x00000800,
	ResolveDest = 0x00001000,
	ResolveSource = 0x00002000,
	RaytracingAccelerationStructureRead = 0x00004000,
	RaytracingAccelerationStructureWrite = 0x00008000,
	ShadingRateSource = 0x00010000,
	VideoDecodeRead = 0x00020000,
	VideoDecodeWrite = 0x00040000,
	VideoProcessRead = 0x00080000,
	VideoProcessWrite = 0x00100000,
	VideoEncodeRead = 0x00200000,
	VideoEncodeWrite = 0x00400000,
	NoAccess = unchecked((int)0x80000000),
} ;


[EquivalentOf(typeof(D3D12_BARRIER_LAYOUT))]
public enum BarrierLayout {
	Undefined = -1,
	Common = 0,
	Present = 0,
	GenericRead = 1,
	RenderTarget = 2,
	UnorderedAccess = 3,
	DepthStencilWrite = 4,
	DepthStencilRead = 5,
	ShaderResource = 6,
	CopySource = 7,
	CopyDest = 8,
	ResolveSource = 9,
	ResolveDest = 10,
	ShadingRateSource = 11,
	VideoDecodeRead = 12,
	VideoDecodeWrite = 13,
	VideoProcessRead = 14,
	VideoProcessWrite = 15,
	VideoEncodeRead = 16,
	VideoEncodeWrite = 17,
	DirectQueueCommon = 18,
	DirectQueueGenericRead = 19,
	DirectQueueUnorderedAccess = 20,
	DirectQueueShaderResource = 21,
	DirectQueueCopySource = 22,
	DirectQueueCopyDest = 23,
	ComputeQueueCommon = 24,
	ComputeQueueGenericRead = 25,
	ComputeQueueUnorderedAccess = 26,
	ComputeQueueShaderResource = 27,
	ComputeQueueCopySource = 28,
	ComputeQueueCopyDest = 29,
	VideoQueueCommon = 30,
} ;


[Flags, EquivalentOf(typeof(D3D12_TEXTURE_BARRIER_FLAGS))]
public enum TextureBarrierFlags {
	None = 0x00000000,
	Discard = 0x00000001,
} ;


/// <summary>Specifies multiple wait flags for multiple fences.</summary>
/// <remarks>
/// This enum is used by the
/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device1-seteventonmultiplefencecompletion">SetEventOnMultipleFenceCompletion</a> method.
/// </remarks>
[Flags, EquivalentOf(typeof(D3D12_MULTIPLE_FENCE_WAIT_FLAGS))]
public enum MultiFenceWaitFlags {
	/// <summary>No flags are being passed. This means to use the default behavior, which is to wait for all fences before signaling the event.</summary>
	None = 0x00000000,
	/// <summary>Modifies behavior to indicate that the event should be signaled after any one of the fence values has been reached by its corresponding fence.</summary>
	Any = 0x00000001,
	/// <summary>An alias for **D3D12_MULTIPLE_FENCE_WAIT_FLAG_NONE**, meaning to use the default behavior and wait for all fences.</summary>
	All = 0x00000000,
} ;


/// <summary>Specifies broad residency priority buckets useful for quickly establishing an application priority scheme.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device1-setresidencypriority">SetResidencyPriority</a> method.</remarks>
[EquivalentOf(typeof(D3D12_RESIDENCY_PRIORITY))]
public enum ResidencyPriority {
	/// <summary>Indicates a minimum priority.</summary>
	Minimum = 671088640,
	/// <summary>Indicates a low priority.</summary>
	Low = 1342177280,
	/// <summary>Indicates a normal, medium, priority.</summary>
	Normal = 2013265920,
	/// <summary>Indicates a high priority. Applications are discouraged from using priories greater than this. For more information see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device1-setresidencypriority">ID3D12Device1::SetResidencyPriority</a>.</summary>
	High = -1610547200,
	/// <summary>Indicates a maximum priority. Applications are discouraged from using priorities greater than this; <b>D3D12_RESIDENCY_PRIORITY_MAXIMUM</b> is not guaranteed to be available. For more information see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device1-setresidencypriority">ID3D12Device1::SetResidencyPriority</a></summary>
	Maximum = -939524096,
} ;


/// <summary>Specifies the type of a sub-object in a pipeline state stream description.</summary>
/// <remarks>
/// This enum is used in the creation of pipeline state objects using the ID3D12Device1::CreatePipelineState method.
/// The CreatePipelineState method takes a D3D12_PIPELINE_STATE_STREAM_DESC as one of its parameters, this structure
/// in turn describes a bytestream made up of alternating D3D12_PIPELINE_STATE_SUBOBJECT_TYPE enumeration values and
/// their corresponding subobject description structs. This bytestream description can be made a concrete type by
/// defining a structure that has the same alternating pattern of alternating D3D12_PIPELINE_STATE_SUBOBJECT_TYPE
/// enumeration values and their corresponding subobject description structs as members.
/// </remarks>
[EquivalentOf(typeof(D3D12_PIPELINE_STATE_SUBOBJECT_TYPE))]
public enum PipelineStateSubObjectType
{
	/// <summary>
	/// <para>Indicates a root signature subobject type. The corresponding subobject type is **[ID3D12RootSignature](/windows/win32/api/d3d12/nn-d3d12-id3d12rootsignature)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	RootSignature = 0,

	/// <summary>
	/// <para>Indicates a vertex shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	VS = 1,

	/// <summary>
	/// <para>Indicates a pixel shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	PS = 2,

	/// <summary>
	/// <para>Indicates a domain shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	DS = 3,

	/// <summary>
	/// <para>Indicates a hull shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	HS = 4,

	/// <summary>
	/// <para>Indicates a geometry shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	GS = 5,

	/// <summary>
	/// <para>Indicates a compute shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	CS = 6,

	/// <summary>
	/// <para>Indicates a stream-output subobject type. The corresponding subobject type is **[D3D12_STREAM_OUTPUT_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_stream_output_desc)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	StreamOutput = 7,

	/// <summary>
	/// <para>Indicates a blend subobject type. The corresponding subobject type is **[D3D12_BLEND_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_blend_desc)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Blend = 8,

	/// <summary>
	/// <para>Indicates a sample mask subobject type. The corresponding subobject type is **UINT**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	SampleMask = 9,

	/// <summary>
	/// <para>Indicates indicates a rasterizer subobject type. The corresponding subobject type is **[D3D12_RASTERIZER_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_rasterizer_desc)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Rasterizer = 10,

	/// <summary>
	/// <para>Indicates a depth stencil subobject type. The corresponding subobject type is **[D3D12_DEPTH_STENCIL_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_desc)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	DepthStencil = 11,

	/// <summary>
	/// <para>Indicates an input layout subobject type. The corresponding subobject type is **[D3D12_INPUT_LAYOUT_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_input_layout_desc)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	InputLayout = 12,

	/// <summary>
	/// <para>Indicates an index buffer strip cut value subobject type. The corresponding subobject type is **[D3D12_INDEX_BUFFER_STRIP_CUT_VALUE](/windows/win32/api/d3d12/ne-d3d12-d3d12_index_buffer_strip_cut_value)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	IBStripCutValue = 13,

	/// <summary>
	/// <para>Indicates a primitive topology subobject type. The corresponding subobject type is **[D3D12_PRIMITIVE_TOPOLOGY_TYPE](/windows/win32/api/d3d12/ne-d3d12-d3d12_primitive_topology_type)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	PrimitiveTopology = 14,

	/// <summary>Indicates a render target formats subobject type. The corresponding subobject type is **[D3D12_RT_FORMAT_ARRAY](/windows/win32/api/d3d12/ns-d3d12-d3d12_rt_format_array)** structure, which wraps an array of render target formats along with a count of the array elements.</summary>
	RenderTargetFormats = 15,

	/// <summary>
	/// <para>Indicates a depth stencil format subobject. The corresponding subobject type is **[DXGI_FORMAT](/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	DepthStencilFormat = 16,

	/// <summary>
	/// <para>Indicates a sample description subobject type. The corresponding subobject type is **[DXGI_SAMPLE_DESC](/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	SampleDesc = 17,

	/// <summary>
	/// <para>Indicates a node mask subobject type. The corresponding subobject type is **[D3D12_NODE_MASK](/windows/win32/api/d3d12/ns-d3d12-d3d12_node_mask)** or **UINT**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	NodeMask = 18,

	/// <summary>
	/// <para>Indicates a cached pipeline state object subobject type. The corresponding subobject type is **[D3D12_CACHED_PIPELINE_STATE](/windows/win32/api/d3d12/ns-d3d12-d3d12_cached_pipeline_state)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	CachedPSO = 19,

	/// <summary>
	/// <para>Indicates a flags subobject type. The corresponding subobject type is **[D3D12_PIPELINE_STATE_FLAGS](/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_flags)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Flags = 20,

	/// <summary>
	/// <para>Indicates an expanded depth stencil subobject type. This expansion of the depth stencil subobject supports optional depth bounds checking. The corresponding subobject type is **[D3D12_DEPTH_STENCIL_DESC1](/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_desc1)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	DepthStencil1 = 21,

	/// <summary>
	/// <para>Indicates a view instancing subobject type. The corresponding subobject type is **[D3D12_VIEW_INSTANCING_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_view_instancing_desc)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	ViewInstancing = 22,

	/// <summary>
	/// <para>Indicates an amplification shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	AS = 24,

	/// <summary>
	/// <para>Indicates a mesh shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	MS = 25,
	DepthStencil2 = 26,
	Rasterizer1   = 27,
	Rasterizer2   = 28,

	/// <summary>A sentinel value that marks the exclusive upper-bound of valid values this enumeration represents.</summary>
	MaxValid = 29,
} ;


/// <summary>Specifies what type of texture copy is to take place.</summary>
/// <remarks>This enum is used by the <see cref="TextureCopyLocation"/> structure.</remarks>
[EquivalentOf(typeof(D3D12_TEXTURE_COPY_TYPE))]
public enum TextureCopyType {
	/// <summary>Indicates a subresource, identified by an index, is to be copied.</summary>
	Index = 0,
	
	/// <summary>
	/// Indicates a place footprint, identified by a
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_placed_subresource_footprint">D3D12_PLACED_SUBRESOURCE_FOOTPRINT</a>
	/// structure, is to be copied.
	/// </summary>
	PlacedFootprint = 1,
} ;


/// <summary>Specifies which resource heap tier the hardware and driver support.</summary>
/// <remarks>
/// <para>This enum is used by the <b>ResourceHeapTier</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a> structure.</para>
/// <para>This enum specifies which resource heap tier the hardware and driver support. Lower tiers require more heap attribution than greater tiers.</para>
/// <para>Resources can be categorized into the following types:</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_heap_tier#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_RESOURCE_HEAP_TIER))]
public enum ResourceHeapTier {
	/// <summary>
	/// <para>Indicates that heaps can only support resources from a single resource category. For the list of resource categories, see Remarks. In tier 1, these resource categories are mutually exclusive and cannot be used with the same heap. The resource category must be declared when creating a heap, using the correct <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_heap_flags">D3D12_HEAP_FLAGS</a> enumeration constant. Applications cannot create heaps with flags that allow all three categories.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_heap_tier#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Tier1 = 1,
	/// <summary>
	/// <para>Indicates that heaps can support resources from all three categories. For the list of resource categories, see Remarks. In tier 2, these resource categories can be mixed within the same heap. Applications may create heaps with flags that allow all three categories; but are not required to do so. Applications may be written to support tier 1 and seamlessly run on tier 2.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_heap_tier#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Tier2 = 2,
} ;


/// <summary>Describes minimum precision support options for shaders in the current graphics driver.</summary>
/// <remarks>
/// <para>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a> structure.</para>
/// <para>The returned info just indicates that the graphics hardware can perform HLSL operations at a lower precision than the standard 32-bit float precision, but doesn’t guarantee that the graphics hardware will actually run at a lower precision.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_shader_min_precision_support#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[Flags, EquivalentOf(typeof(D3D12_SHADER_MIN_PRECISION_SUPPORT))]
public enum ShaderMinPrecisionSupport {
	/// <summary>The driver supports only full 32-bit precision for all shader stages.</summary>
	None = 0x00000000,
	/// <summary>The driver supports 10-bit precision.</summary>
	_10Bit = 0x00000001,
	/// <summary>The driver supports 16-bit precision.</summary>
	_16Bit = 0x00000002,
}


/// <summary>Specifies the level of sharing across nodes of an adapter, such as Tier 1 Emulated, Tier 1, or Tier 2.</summary>
/// <remarks>This enum is used by the <b>CrossNodeSharingTier</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a> structure.</remarks>
[EquivalentOf(typeof(D3D12_CROSS_NODE_SHARING_TIER))]
public enum CrossNodeSharingTier {
	/// <summary>
	/// If an adapter has only 1 node, then cross-node sharing doesn't apply, so the <b>CrossNodeSharingTier</b> member of the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a>
	/// structure is set to D3D12_CROSS_NODE_SHARING_NOT_SUPPORTED.
	/// </summary>
	NotSupported = 0,
	/// <summary>
	/// <para>Tier 1 Emulated. Devices that set the <b>CrossNodeSharingTier</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a> structure to D3D12_CROSS_NODE_SHARING_TIER_1_EMULATED have Tier 1 support. However, drivers stage these copy operations through a driver-internal system memory allocation. This will cause these copy operations to consume time on the destination GPU as well as the source.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_cross_node_sharing_tier#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Tier1Emulated = 1,
	/// <summary>
	/// <para>Tier 1. Devices that set the <b>CrossNodeSharingTier</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a> structure to D3D12_CROSS_NODE_SHARING_TIER_1 only support the following cross-node copy operations: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_cross_node_sharing_tier#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Tier1 = 2,
	/// <summary>
	/// <para>Tier 2. Devices that set the <b>CrossNodeSharingTier</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a> structure to D3D12_CROSS_NODE_SHARING_TIER_2 support all operations across nodes, except for the following: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_cross_node_sharing_tier#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Tier2 = 3,
	/// <summary>Indicates support for D3D12_HEAP_FLAG_ALLOW_SHADER_ATOMICS on heaps that are visible to multiple nodes.</summary>
	Tier3 = 4,
} ;


/// <summary>Identifies the tier level of conservative rasterization.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a> structure.</remarks>
[EquivalentOf( typeof( D3D12_CONSERVATIVE_RASTERIZATION_TIER ) )]
public enum ConservativeRasterizationTier {
	/// <summary>Conservative rasterization is not supported.</summary>
	TierNotSupported = 0,
	/// <summary>Tier 1 enforces a maximum 1/2 pixel uncertainty region and does not support post-snap degenerates. This is good for tiled rendering, a texture atlas, light map generation and sub-pixel shadow maps.</summary>
	Tier1 = 1,
	/// <summary>Tier 2 reduces the maximum uncertainty region to 1/256 and requires post-snap degenerates not be culled. This tier is helpful for CPU-based algorithm acceleration (such as voxelization).</summary>
	Tier2 = 2,
	/// <summary>Tier 3 maintains a maximum 1/256 uncertainty region and adds support for inner input coverage. Inner input coverage adds the new value <c>SV_InnerCoverage</c> to High Level Shading Language (HLSL). This is a 32-bit scalar integer that can be specified on input to a pixel shader, and represents the underestimated conservative rasterization information (that is, whether a pixel is guaranteed-to-be-fully covered). This tier is helpful for occlusion culling.</summary>
	Tier3 = 3,
} ;


/// <summary>Identifies the tier of resource binding being used.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a> structure.</remarks>
[EquivalentOf(typeof(D3D12_RESOURCE_BINDING_TIER))]
public enum ResourceBindingTier {
	/// <summary>
	/// <para>Tier 1. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/hardware-support">Hardware Tiers</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_binding_tier#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Tier1 = 1,
	/// <summary>
	/// <para>Tier 2. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/hardware-support">Hardware Tiers</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_binding_tier#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Tier2 = 2,
	/// <summary>
	/// <para>Tier 3. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/hardware-support">Hardware Tiers</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_binding_tier#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Tier3 = 3,
} ;


[EquivalentOf(typeof(D3D12_PROGRAMMABLE_SAMPLE_POSITIONS_TIER))]
public enum ProgrammableSamplePositionsTier {
	/// <summary>Indicates that there's no support for programmable sample positions.</summary>
	NotSupported = 0,
	/// <summary>Indicates that there's tier 1 support for programmable sample positions. In tier 1, a single sample pattern can be specified to repeat for every pixel (<a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-setsamplepositions">SetSamplePosition</a> parameter <i>NumPixels</i> = 1) and ResolveSubResource is supported.</summary>
	Tier1 = 1,
	/// <summary>Indicates that there's tier 2 support for programmable sample positions. In tier 2, four separate sample patterns can be specified for each pixel in a 2x2 grid (<a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist1-setsamplepositions">SetSamplePosition</a> parameter <i>NumPixels</i> = 1) that repeats over the render-target or viewport, aligned on even coordinates .</summary>
	Tier2 = 2,
} ;


/// <summary>Used to determine which kinds of command lists are capable of supporting various operations.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_command_list_support_flags">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[Flags, EquivalentOf(typeof(D3D12_COMMAND_LIST_SUPPORT_FLAGS))]
public enum CommandListSupportFlags {
	/// <summary>No support.</summary>
	None = 0x00000000,
	/// <summary>Specifies that direct command lists can support direct mode.</summary>
	Direct = 0x00000001,
	/// <summary>Specifies that command list bundles can support bundle operations.</summary>
	Bundle = 0x00000002,
	/// <summary>Specifies that compute command lists can support compute operations in question.</summary>
	Compute = 0x00000004,
	/// <summary>Specifies that copy command lists can support the copy operation in question.</summary>
	Copy = 0x00000008,
	/// <summary>Specifies that video-decode command lists can support the video decode operation in question.</summary>
	VideoDecode = 0x00000010,
	/// <summary>Specifies that video-processing command lists can support the video processing operation is question.</summary>
	VideoProcess = 0x00000020,
	VideoEncode = 0x00000040,
} ;


/// <summary>Indicates the tier level at which view instancing is supported.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_view_instancing_tier">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_VIEW_INSTANCING_TIER))]
public enum ViewInstancingTier {
	/// <summary>View instancing is not supported.</summary>
	NotSupported = 0,
	/// <summary>View instancing is supported by draw-call level looping only.</summary>
	Tier1 = 1,
	/// <summary>View instancing is supported by draw-call level looping at worst, but the GPU can perform view instancing more efficiently in certain circumstances which are architecture-dependent.</summary>
	Tier2 = 2,
	/// <summary>
	/// <para>View instancing is supported and instancing begins with the first shader stage that references SV_ViewID or with rasterization if no shader stage references SV_ViewID. This means that redundant work is eliminated across view instances when it's not dependent on SV_ViewID. Before rasterization, work that doesn't directly depend on SV_ViewID is shared across all views; only work that depends on SV_ViewID is repeated for each view. <div class="alert"><b>Note</b>  If a hull shader produces tessellation factors that are dependent on SV_ViewID, then tessellation and all subsequent work must be repeated per-view. Similarly, if the amount of geometry produced by the geometry shader depends on SV_ViewID, then the geometry shader must be repeated per-view before proceeding to rasterization.</div> <div> </div> View instance masking only effects whether work that directly depends on SV_ViewID is performed, not the entire loop iteration (per-view). If the view instance mask is non-0, some work that depends on SV_ViewID might still be performed on masked-off pixels but will have no externally-visible effect; for example, no UAV writes are performed and clipping/rasterization is not invoked. If the view instance mask is 0 no work is performed, including work that's not dependent on SV_ViewID.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_view_instancing_tier#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	Tier3 = 3,
} ;


/// <summary>Defines constants that specify a cross-API sharing support tier.</summary>
[EquivalentOf(typeof(D3D12_SHARED_RESOURCE_COMPATIBILITY_TIER))]
public enum SharedResourceCompatibilityTier {
	/// <summary>
	/// <para>Related to [D3D11_SHARED_RESOURCE_TIER::D3D11_SHARED_RESOURCE_TIER_1](/windows/win32/api/d3d11/ne-d3d11-d3d11_shared_resource_tier). Specifies that the most basic level of cross-API sharing is supported, including the following resource data formats. * DXGI_FORMAT_R8G8B8A8_UNORM * DXGI_FORMAT_R8G8B8A8_UNORM_SRGB * DXGI_FORMAT_B8G8R8A8_UNORM * DXGI_FORMAT_B8G8R8A8_UNORM_SRGB * DXGI_FORMAT_B8G8R8X8_UNORM * DXGI_FORMAT_B8G8R8X8_UNORM_SRGB * DXGI_FORMAT_R10G10B10A2_UNORM * DXGI_FORMAT_R16G16B16A16_FLOAT</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_shared_resource_compatibility_tier#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	Tier0 = 0,
	/// <summary>
	/// <para>Related to [D3D11_SHARED_RESOURCE_TIER::D3D11_SHARED_RESOURCE_TIER_2](/windows/win32/api/d3d11/ne-d3d11-d3d11_shared_resource_tier). Specifies that cross-API sharing functionality of **D3D12_SHARED_RESOURCE_COMPATIBILITY_TIER_0** is supported, plus the following formats. * DXGI_FORMAT_R16G16B16A16_TYPELESS * DXGI_FORMAT_R10G10B10A2_TYPELESS * DXGI_FORMAT_R8G8B8A8_TYPELESS * DXGI_FORMAT_R8G8B8X8_TYPELESS * DXGI_FORMAT_R16G16_TYPELESS * DXGI_FORMAT_R8G8_TYPELESS * DXGI_FORMAT_R32_TYPELESS * DXGI_FORMAT_R16_TYPELESS * DXGI_FORMAT_R8_TYPELESS This level support is built into WDDM 2.4. Also see [Extended support for shared Texture2D resources](/windows/win32/direct3d11/direct3d-11-1-features#extended-support-for-shared-texture2d-resources).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_shared_resource_compatibility_tier#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	Tier1 = 1,
	/// <summary>
	/// <para>Related to [D3D11_SHARED_RESOURCE_TIER::D3D11_SHARED_RESOURCE_TIER_3](/windows/win32/api/d3d11/ne-d3d11-d3d11_shared_resource_tier). Specifies that cross-API sharing functionality of **D3D12_SHARED_RESOURCE_COMPATIBILITY_TIER_1** is supported, plus the following formats. * DXGI_FORMAT_NV12 (also see [Extended NV12 texture support](/windows/win32/direct3d11/direct3d-11-4-features#extended-nv12-texture-support))</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_shared_resource_compatibility_tier#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	Tier2 = 2,
} ;


/// <summary>Specifies the level of support for render passes on a graphics device.</summary>
/// <remarks>To determine the level of support for render passes for a graphics device, pass <see cref="D3D12Options5"/> struct.</remarks>
[EquivalentOf( typeof( D3D12_RENDER_PASS_TIER ) )]
public enum RenderPassTier {
	/// <summary>The user-mode display driver hasn't implemented render passes, and so the feature is provided only via software emulation. Render passes might not provide a performance advantage at this level of support.</summary>
	Tier0 = 0,
	/// <summary>The render passes feature is implemented by the user-mode display driver, and render target/depth buffer writes may be accelerated. Unordered access view (UAV) writes are not efficiently supported within the render pass.</summary>
	Tier1 = 1,
	/// <summary>The render passes feature is implemented by the user-mode display driver, render target/depth buffer writes may be accelerated, and unordered access view (UAV) writes (provided that writes in a render pass are not read until a subsequent render pass) are likely to be more efficient than issuing the same work without using a render pass.</summary>
	Tier2 = 2,
} ;


/// <summary>Specifies the level of ray tracing support on the graphics device.</summary>
/// <remarks>To determine the supported ray tracing tier for a graphics device, pass <see cref="D3D12Options5"/> struct.</remarks>
[EquivalentOf(typeof(D3D12_RAYTRACING_TIER))]
public enum RaytracingTier {
	/// <summary>No support for ray tracing on the device.  Attempts to create any ray tracing-related object will fail, and using ray tracing-related APIs on command lists results in undefined behavior.</summary>
	NotSupported = 0,
	/// <summary>The device supports tier 1 ray tracing functionality. In the current release, this tier represents all available ray tracing features.</summary>
	Tier1_0 = 10,
	/// <summary>The device supports tier 1.1 ray tracing functionality.</summary>
	Tier1_1 = 11,
} ;


/// <summary>Defines constants that specify a shading rate tier (for variable-rate shading, or VRS).</summary>
[EquivalentOf(typeof(D3D12_VARIABLE_SHADING_RATE_TIER))]
public enum VariableShadingRateTier {
	/// <summary>Specifies that variable-rate shading is not supported.</summary>
	NotSupported = 0,
	/// <summary>Specifies that variable-rate shading tier 1 is supported.</summary>
	Tier1 = 1,
	/// <summary>Specifies that variable-rate shading tier 2 is supported.</summary>
	Tier2 = 2,
} ;


/// <summary>Defines constants that specify mesh and amplification shader support.</summary>
[EquivalentOf(typeof(D3D12_MESH_SHADER_TIER))]
public enum MeshShaderTier {
	/// <summary>Specifies that mesh and amplification shaders are not supported.</summary>
	NotSupported = 0,
	/// <summary>Specifies that mesh and amplification shaders are supported.</summary>
	Tier1 = 10,
} ;


/// <summary>Defines constants that specify sampler feedback support.</summary>
[EquivalentOf(typeof(D3D12_SAMPLER_FEEDBACK_TIER))]
public enum SamplerFeedbackTier {
	/// <summary>Specifies that sampler feedback is not supported. Attempts at calling sampler feedback APIs represent an error.</summary>
	NotSupported = 0,
	/// <summary>
	/// <para>Specifies that sampler feedback is supported to tier 0.9. This indicates the following: Sampler feedback is supported for samplers with these texture addressing modes: * D3D12_TEXTURE_ADDRESS_MODE_WRAP * D3D12_TEXTURE_ADDRESS_MODE_CLAMP The Texture2D shader resource view passed in to feedback-writing HLSL methods has these restrictions: * The MostDetailedMip field must be 0. * The MipLevels count must span the full mip count of the resource. * The PlaneSlice field must be 0. * The ResourceMinLODClamp field must be 0. The Texture2DArray shader resource view passed in to feedback-writing HLSL methods has these restrictions: * All the limitations as in Texture2D above, and * The FirstArraySlice field must be 0. * The ArraySize field must span the full array element count of the resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_sampler_feedback_tier#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Tier0_9 = 90,
	/// <summary>Specifies sample feedback is supported to tier 1.0. This indicates that sampler feedback is supported for all texture addressing modes, and feedback-writing methods are supported irrespective of the passed-in shader resource view.</summary>
	Tier1_0 = 100,
} ;


/// <summary>Defines constants that specify a level of support for WaveMMA (wave_matrix) operations.</summary>
[EquivalentOf(typeof(D3D12_WAVE_MMA_TIER))]
public enum WaveMMATier {
	/// <summary>Specifies that WaveMMA (wave_matrix) operations are not supported.</summary>
	NotSupported = 0,
	/// <summary>Specifies that WaveMMA (wave_matrix) operations are supported.</summary>
	Tier1_0 = 10,
} ;


/// <summary>Specifies resources that are supported for a provided format.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_format_support">D3D12_FEATURE_DATA_FORMAT_SUPPORT</a> structure.</remarks>
[Flags, EquivalentOf(typeof(D3D12_FORMAT_SUPPORT1))]
public enum FormatSupport1 {
	/// <summary>No resources are supported.</summary>
	None = 0x00000000,
	/// <summary>Buffer resources supported.</summary>
	Buffer = 0x00000001,
	/// <summary>Vertex buffers supported.</summary>
	IAVertexBuffer = 0x00000002,
	/// <summary>Index buffers supported.</summary>
	IAIndexBuffer = 0x00000004,
	/// <summary>Streaming output buffers supported.</summary>
	SOBuffer = 0x00000008,
	/// <summary>1D texture resources supported.</summary>
	Texture1D = 0x00000010,
	/// <summary>2D texture resources supported.</summary>
	Texture2D = 0x00000020,
	/// <summary>3D texture resources supported.</summary>
	Texture3D = 0x00000040,
	/// <summary>Cube texture resources supported.</summary>
	TextureCube = 0x00000080,
	/// <summary>The HLSL <a href="https://docs.microsoft.com/windows/desktop/direct3dhlsl/dx-graphics-hlsl-to-load">Load</a> function for texture objects is supported.</summary>
	ShaderLoad = 0x00000100,
	/// <summary>
	/// <para>The HLSL <a href="https://docs.microsoft.com/windows/desktop/direct3dhlsl/dx-graphics-hlsl-to-sample">Sample</a> function for texture objects is supported. <div class="alert"><b>Note</b>  If the device supports the format as a resource (1D, 2D, 3D, or cube map) but doesn't support this option, the resource can still use the <a href="https://docs.microsoft.com/windows/desktop/direct3dhlsl/dx-graphics-hlsl-to-sample">Sample</a> method but must use only the point filtering sampler state to perform the sample.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_format_support1#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	ShaderSample = 0x00000200,
	/// <summary>
	/// <para>The HLSL <a href="https://docs.microsoft.com/windows/desktop/direct3dhlsl/dx-graphics-hlsl-to-samplecmp">SampleCmp</a> and <a href="https://docs.microsoft.com/windows/desktop/direct3dhlsl/dx-graphics-hlsl-to-samplecmplevelzero">SampleCmpLevelZero</a> functions for texture objects are supported. <div class="alert"><b>Note</b>  Windows 8 and later might provide limited support for these functions on Direct3D <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature levels</a> 9_1, 9_2, and 9_3. For more info, see <a href="https://docs.microsoft.com/previous-versions/windows/apps/jj262110(v=win.10)">Implementing shadow buffers for Direct3D feature level 9</a>. </div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_format_support1#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	ShaderSampleComparison = 0x00000400,
	/// <summary>Reserved.</summary>
	ShaderSampleMonoText = 0x00000800,
	/// <summary>Mipmaps are supported.</summary>
	Mip = 0x00001000,
	/// <summary>Render targets are supported.</summary>
	RenderTarget = 0x00004000,
	/// <summary>Blend operations supported.</summary>
	Blendable = 0x00008000,
	/// <summary>Depth stencils supported.</summary>
	DepthStencil = 0x00010000,
	/// <summary>Multisample antialiasing (MSAA) resolve operations are supported. For more info, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resolvesubresource">ID3D12GraphicsCommandList::ResolveSubresource</a>.</summary>
	MultisampleResolve = 0x00040000,
	/// <summary>Format can be displayed on screen.</summary>
	Display = 0x00080000,
	/// <summary>Format can't be cast to another format.</summary>
	CastWithinBitLayout = 0x00100000,
	/// <summary>Format can be used as a multi-sampled render target.</summary>
	MultisampleRendertarget = 0x00200000,
	/// <summary>Format can be used as a multi-sampled texture and read into a shader with the HLSL <a href="https://docs.microsoft.com/windows/desktop/direct3dhlsl/dx-graphics-hlsl-to-load">Load</a> function.</summary>
	MultisampleLoad = 0x00400000,
	/// <summary>Format can be used with the HLSL gather function. This value is available in DirectX 10.1 or higher.</summary>
	ShaderGather = 0x00800000,
	/// <summary>Format supports casting when the resource is a back buffer.</summary>
	BackBufferCast = 0x01000000,
	/// <summary>Format can be used for an unordered access view.</summary>
	TypedUnorderedAccessView = 0x02000000,
	/// <summary>Format can be used with the HLSL gather with comparison function.</summary>
	ShaderGatherComparison = 0x04000000,
	/// <summary>Format can be used with the decoder output.</summary>
	DecoderOutput = 0x08000000,
	/// <summary>Format can be used with the video processor output.</summary>
	VideoProcessorOutput = 0x10000000,
	/// <summary>Format can be used with the video processor input.</summary>
	VideoProcessorInput = 0x20000000,
	/// <summary>Format can be used with the video encoder.</summary>
	VideoEncoder = 0x40000000,
}


/// <summary>Specifies which unordered resource options are supported for a provided format.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_format_support">D3D12_FEATURE_DATA_FORMAT_SUPPORT</a> structure.</remarks>
[Flags, EquivalentOf(typeof(D3D12_FORMAT_SUPPORT2))]
public enum FormatSupport2 {
	/// <summary>No unordered resource options are supported.</summary>
	None = 0x00000000,
	/// <summary>Format supports atomic add.</summary>
	UAVAtomicAdd = 0x00000001,
	/// <summary>Format supports atomic bitwise operations.</summary>
	UAVAtomicBitwiseOps = 0x00000002,
	/// <summary>Format supports atomic compare with store or exchange.</summary>
	UAVAtomicCompareStoreOrCompareExchange = 0x00000004,
	/// <summary>Format supports atomic exchange.</summary>
	UAVAtomicExchange = 0x00000008,
	/// <summary>Format supports atomic min and max.</summary>
	UAVAtomicSignedMinOrMax = 0x00000010,
	/// <summary>Format supports atomic unsigned min and max.</summary>
	UAVAtomicUnsignedMinOrMax = 0x00000020,
	/// <summary>Format supports a typed load.</summary>
	UAVTypedLoad = 0x00000040,
	/// <summary>Format supports a typed store.</summary>
	UAVTypedStore = 0x00000080,
	/// <summary>Format supports logic operations in blend state.</summary>
	LogicOp = 0x00000100,
	/// <summary>Format supports tiled resources. Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/volume-tiled-resources">Volume Tiled Resources</a>.</summary>
	Tiled = 0x00000200,
	/// <summary>Format supports multi-plane overlays.</summary>
	MultiplaneOverlay = 0x00004000,
	SamplerFeedback = 0x00008000,
} ;


/// <summary>Specifies options for determining quality levels.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_multisample_quality_levels">D3D12_FEATURE_DATA_MULTISAMPLE_QUALITY_LEVELS</a> structure.</remarks>
[Flags, EquivalentOf(typeof(D3D12_MULTISAMPLE_QUALITY_LEVEL_FLAGS))]
public enum MultisampleQualityLevelFlags {
	/// <summary>No options are supported.</summary>
	None = 0x00000000,
	/// <summary>The number of quality levels can be determined for tiled resources.</summary>
	TiledResource = 0x00000001,
} ;


/// <summary>Specifies a shader model.</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_shader_model">D3D12_FEATURE_DATA_SHADER_MODEL</a> structure.</remarks>
[EquivalentOf(typeof(D3D_SHADER_MODEL))]
public enum ShaderModel {
	/// <summary>Indicates shader model 5.1.</summary>
	Model5_1 = 81,
	/// <summary>Indicates shader model 6.0. Compiling a shader model 6.0 shader requires using the DXC compiler (see [DirectX Shader Compiler](https://github.com/Microsoft/DirectXShaderCompiler)), and is not supported by legacy **FXC**.</summary>
	Model6_0 = 96,
	/// <summary>Indicates shader model 6.1.</summary>
	Model6_1 = 97,
	/// <summary></summary>
	Model6_2 = 98,
	/// <summary></summary>
	Model6_3 = 99,
	/// <summary>Shader model 6.4 support was added in Windows 10, Version 1903, and is required for DirectX Raytracing (DXR).</summary>
	Model6_4 = 100,
	/// <summary>Shader model 6.5 support was added in Windows 10, Version 2004, and is required for Direct Machine Learning.</summary>
	Model6_5 = 101,
	/// <summary>Shader model 6.6 support was added in Windows 11 and the DirectX 12 Agility SDK.</summary>
	Model6_6 = 102,
	/// <summary>Shader model 6.7 support was added in the DirectX 12 Agility SDK v1.6. See [Agility SDK 1.606.3: Shader Model 6.7 is now publicly available!](https://devblogs.microsoft.com/directx/shader-model-6-7/) on the DirectX developer blog.</summary>
	Model6_7 = 103,
	Model6_8 = 104,
	D3DHighestShaderModel = 104,
} ;


/// <summary>Defines constants that specify protected resource session support.</summary>
[Flags, EquivalentOf(typeof(D3D12_PROTECTED_RESOURCE_SESSION_SUPPORT_FLAGS))]
public enum ProtectedResourceSessionSupportFlags {
	/// <summary>Indicates that protected resource sessions are not supported.</summary>
	None = 0x00000000,
	/// <summary>Indicates that protected resource sessions are supported.</summary>
	Supported = 0x00000001,
} ;


/// <summary>Describes the level of support for shader caching in the current graphics driver. (D3D12_SHADER_CACHE_SUPPORT_FLAGS)</summary>
/// <remarks>This enum is used by the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_shader_cache">D3D_FEATURE_DATA_SHADER_CACHE</a> structure.</remarks>
[Flags, EquivalentOf(typeof(D3D12_SHADER_CACHE_SUPPORT_FLAGS))]
public enum ShaderCacheSupportFlags {
	/// <summary>Indicates that the driver does not support shader caching.</summary>
	None = 0x00000000,
	/// <summary>Indicates that the driver supports the CachedPSO member of the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_graphics_pipeline_state_desc">D3D12_GRAPHICS_PIPELINE_STATE_DESC</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_compute_pipeline_state_desc">D3D12_COMPUTE_PIPELINE_STATE_DESC</a> structures. This is always supported.</summary>
	SinglePSO = 0x00000001,
	/// <summary>Indicates that the driver supports the ID3D12PipelineLibrary interface, which provides application-controlled PSO grouping and caching. This is supported by drivers targetting the Windows 10 Anniversary Update.</summary>
	Library = 0x00000002,
	/// <summary>Indicates that the driver supports an OS-managed shader cache that stores compiled shaders in memory during the current run of the application.</summary>
	AutomaticInProcCache = 0x00000004,
	/// <summary>Indicates that the driver supports an OS-managed shader cache that stores compiled shaders on disk to accelerate future runs of the application.</summary>
	AutomaticDiskCache = 0x00000008,
	DriverManagedCache = 0x00000010,
	ShaderControlClear = 0x00000020,
	ShaderSessionDelete = 0x00000040,
} ;


/// <summary>Defines constants that specify heap serialization support.</summary>
[EquivalentOf(typeof(D3D12_HEAP_SERIALIZATION_TIER))]
public enum HeapSerializationTier {
	/// <summary>Indicates that heap serialization is not supported.</summary>
	Tier0 = 0,
	/// <summary>
	/// Indicates that heap serialization is supported.
	/// Your application can serialize resource data in heaps through copying APIs such as <see cref="IGraphicsCommandList.CopyResource"/>,
	/// without necessarily requiring an explicit <a href="https://learn.microsoft.com/en-us/windows/win32/direct3d12/using-resource-barriers-to-synchronize-resource-states-in-direct3d-12">state transition</a> of resources on those heaps.</summary>
	Tier1_0 = 10,
} ;


/// <summary>
/// Used with the EnqueuMakeResident function to choose how residency
/// operations proceed when the memory budget is exceeded.
/// </summary>
[Flags, EquivalentOf(typeof(D3D12_RESIDENCY_FLAGS))]
public enum ResidencyFlags {
	/// <summary>Specifies the default residency policy, which allows residency operations to succeed regardless of the application's current memory budget. EnqueueMakeResident returns E_OUTOFMEMORY only when there is no memory available.</summary>
	None = 0x00000000,
	/// <summary>Specifies that the EnqueueMakeResident function should return E_OUTOFMEMORY when the residency operation would exceed the application's current memory budget.</summary>
	DenyOverbudget = 0x00000001,
} ;


/// <summary>The D3D12_COMMAND_LIST_FLAGS enumeration specifies flags to be used when creating a command list.</summary>
[Flags, EquivalentOf(typeof(D3D12_COMMAND_LIST_FLAGS))]
public enum CommandListFlags {
	/// <summary>No flags specified.</summary>
	None = 0x00000000,
} ;


/// <summary>Defines flags that specify states related to a graphics command list. Values can be bitwise OR'd together.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_graphics_states">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[Flags, EquivalentOf(typeof(D3D12_GRAPHICS_STATES))]
public enum GraphicsStates {
	/// <summary>Specifies no state.</summary>
	None = 0x00000000,
	/// <summary>Specifies the state of the vertex buffer bindings on the input assembler stage.</summary>
	IAVertexBuffers = 0x00000001,
	/// <summary>Specifies the state of the index buffer binding on the input assembler stage.</summary>
	IAIndexBuffer = 0x00000002,
	/// <summary>Specifies the state of the primitive topology value set on the input assembler stage.</summary>
	IAPrimitiveTopology = 0x00000004,
	/// <summary>Specifies the state of the currently bound descriptor heaps.</summary>
	DescriptorHeap = 0x00000008,
	/// <summary>Specifies the state of the currently set graphics root signature.</summary>
	RootSignature = 0x00000010,
	/// <summary>Specifies the state of the currently set compute root signature.</summary>
	ComputeRootSignature = 0x00000020,
	/// <summary>Specifies the state of the viewports bound to the rasterizer stage.</summary>
	RSViewports = 0x00000040,
	/// <summary>Specifies the state of the scissor rectangles bound to the rasterizer stage.</summary>
	RSScissorRects = 0x00000080,
	/// <summary>Specifies the predicate state.</summary>
	Predication = 0x00000100,
	/// <summary>Specifies the state of the render targets bound to the output merger stage.</summary>
	OMRenderTargets = 0x00000200,
	/// <summary>Specifies the state of the reference value for depth stencil tests set on the output merger stage.</summary>
	OMStencilRef = 0x00000400,
	/// <summary>Specifies the state of the blend factor set on the output merger stage.</summary>
	OMBlendFactor = 0x00000800,
	/// <summary>Specifies the state of the pipeline state object.</summary>
	PipelineState = 0x00001000,
	/// <summary>Specifies the state of the buffer views bound to the stream output stage.</summary>
	SOTargets = 0x00002000,
	/// <summary>Specifies the state of the depth bounds set on the output merger stage.</summary>
	OMDepthBounds = 0x00004000,
	/// <summary>Specifies the state of the sample positions.</summary>
	SamplePositions = 0x00008000,
	/// <summary>Specifies the state of the view instances mask.</summary>
	ViewInstanceMask = 0x00010000,
} ;


/// <summary>Defines constants that specify the data type of a parameter to a meta command.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_meta_command_parameter_type">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_META_COMMAND_PARAMETER_TYPE))]
public enum MetaCommandParameterType {
	/// <summary>Specifies that the parameter is of type <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">FLOAT</a>.</summary>
	Float = 0,
	/// <summary>Specifies that the parameter is of type <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT64</a>.</summary>
	UInt64 = 1,
	/// <summary>Specifies that the parameter is a GPU virtual address.</summary>
	GPUVirtualAddress = 2,
	/// <summary>Specifies that the parameter is a CPU descriptor handle to a heap containing either constant buffer views, shader resource views, or unordered access views.</summary>
	CPUDescriptorHandleHeapType_CBV_SRV_UAV = 3,
	/// <summary>Specifies that the parameter is a GPU descriptor handle to a heap containing either constant buffer views, shader resource views, or unordered access views.</summary>
	GPUCPUDescriptorHandleHeapType_CBV_SRV_UAV = 4,
} ;


/// <summary>Specifies the type of a state object. Use with D3D12_STATE_OBJECT_DESC.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_state_object_type">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_STATE_OBJECT_TYPE))]
public enum StateObjectType {
	/// <summary>Collection state object.</summary>
	Collection = 0,
	/// <summary>Raytracing pipeline state object.</summary>
	RaytracingPipeline = 3,
} ;


/// <summary>The type of a state subobject. Use with D3D12_STATE_SUBOBJECT.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_state_subobject_type">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_STATE_SUBOBJECT_TYPE))]
public enum StateSubObjectType {
	/// <summary>Subobject type is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_state_object_config">D3D12_STATE_OBJECT_CONFIG</a>.</summary>
	StateObjectConfig = 0,
	/// <summary>Subobject type is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_global_root_signature">D3D12_GLOBAL_ROOT_SIGNATURE</a>.</summary>
	GlobalRootSignature = 1,
	/// <summary>Subobject type is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_local_root_signature">D3D12_LOCAL_ROOT_SIGNATURE</a>.</summary>
	LocalRootSignature = 2,
	/// <summary>
	/// <para>Subobject type is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_node_mask">D3D12_NODE_MASK</a>. > [!IMPORTANT] > On some versions of the DirectX Runtime, specifying a node via [**D3D12_NODE_MASK**](/windows/win32/api/d3d12/ns-d3d12-d3d12_node_mask) in a [**D3D12_STATE_SUBOBJECT**](/windows/win32/api/d3d12/ns-d3d12-d3d12_state_subobject) with type **D3D12_STATE_SUBOBJECT_TYPE_NODE_MASK**, the runtime will incorrectly handle a node mask value of `0`, which should use node #1, which will lead to errors when attempting to use the state object later. Specify an explicit node value of 1, or omit the [**D3D12_NODE_MASK**](/windows/win32/api/d3d12/ns-d3d12-d3d12_node_mask) subobject to avoid this issue.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_state_subobject_type#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	NodeMask = 3,
	/// <summary>Subobject type is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_dxil_library_desc">D3D12_DXIL_LIBRARY_DESC</a>.</summary>
	DXILLibrary = 5,
	/// <summary>Subobject type is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_existing_collection_desc">D3D12_EXISTING_COLLECTION_DESC</a>.</summary>
	ExistingCollection = 6,
	/// <summary>Subobject type is <a href="../d3d12/ns-d3d12-d3d12_subobject_to_exports_association.md">D3D12_SUBOBJECT_TO_EXPORTS_ASSOCIATION</a>.</summary>
	SubObjectToExportsAssociation = 7,
	/// <summary>Subobject type is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_dxil_subobject_to_exports_association">D3D12_DXIL_SUBOBJECT_TO_EXPORTS_ASSOCIATION</a>.</summary>
	DXILSubObjectToExportsAssociation = 8,
	/// <summary>Subobject type is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_shader_config">D3D12_RAYTRACING_SHADER_CONFIG</a>.</summary>
	RaytracingShaderConfig = 9,
	/// <summary>Subobject type is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_pipeline_config">D3D12_RAYTRACING_PIPELINE_CONFIG</a>.</summary>
	RaytracingPipelineConfig = 10,
	/// <summary>Subobject type is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_hit_group_desc">D3D12_HIT_GROUP_DESC</a></summary>
	HitGroup = 11,
	RaytracingPipelineConfig1 = 12,
	/// <summary>The maximum valid subobject type value.</summary>
	MaxValid = 13,
} ;


/// <summary>
/// Specifies the result of a call to <see cref="IDevice5.CheckDriverMatchingIdentifier"/> which queries whether
/// serialized data is compatible with the current device and driver version.
/// </summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_driver_matching_identifier_status">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_DRIVER_MATCHING_IDENTIFIER_STATUS))]
public enum DriverMatchingIdentifierStatus {
	/// <summary>Serialized data is compatible with the current device/driver.</summary>
	CompatibleWithDevice = 0,
	/// <summary>The specified <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_serialized_data_type">D3D12_SERIALIZED_DATA_TYPE</a> specified is unknown or unsupported.</summary>
	UnsupportedType = 1,
	/// <summary>Format of the data in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_serialized_data_driver_matching_identifier">D3D12_SERIALIZED_DATA_DRIVER_MATCHING_IDENTIFIER</a> is unrecognized.  This could indicate either corrupt data or the identifier was produced by a different hardware vendor.</summary>
	Unrecognized = 2,
	/// <summary>Serialized data is recognized, but its version is not compatible with the current driver. This result may indicate that the device is from the same hardware vendor but is an incompatible version.</summary>
	IncompatibleVersion = 3,
	/// <summary><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_serialized_data_type">D3D12_SERIALIZED_DATA_TYPE</a> specifies a data type that is not compatible with the type of serialized data.  As long as there is only a single defined serialized data type this error cannot not be produced.</summary>
	IncompatibleType = 4,
} ;


/// <summary>Defines constants that specify a shader cache's mode.</summary>
[EquivalentOf(typeof(D3D12_SHADER_CACHE_MODE))]
public enum ShaderCacheMode {
	/// <summary>Specifies that there's no backing file for this cache. All stores are discarded when the session object is destroyed.</summary>
	Memory = 0,
	/// <summary>Specifies that the session is backed by files on disk that persist from run to run unless cleared. For ways to clear a disk cache, see [ID3D12ShaderCacheSession::SetDeleteOnDestroy](nf-d3d12-id3d12shadercachesession-setdeleteondestroy.md).</summary>
	Disk = 1,
} ;


/// <summary>Defines constants that specify shader cache flags.</summary>
[EquivalentOf(typeof(D3D12_SHADER_CACHE_FLAGS))]
public enum ShaderCacheFlags {
	/// <summary>Specifies no flag.</summary>
	None = 0x00000000,
	/// <summary>Specifies that the cache is implicitly versioned by the driver being used. For multi-GPU systems, a cache created this way is stored side by side for each adapter on which the application runs. The *Version* field in the [D3D12_SHADER_CACHE_SESSION_DESC](ns-d3d12-d3d12_shader_cache_session_desc.md) struct (the cache description) is used as an additional constraint.</summary>
	DriverVersioned = 0x00000001,
	/// <summary>By default, caches are stored in temporary storage, and can be cleared by disk cleanup. This constant (not valid for UWP apps) specifies that the cache is instead stored in the current working directory.</summary>
	UseWorkingDir = 0x00000002,
} ;


/// <summary>Defines constants that specify a kind of shader cache.</summary>
[Flags, EquivalentOf(typeof(D3D12_SHADER_CACHE_KIND_FLAGS))]
public enum ShaderCacheKindFlags {
	/// <summary>Specifies a cache that's managed by Direct3D 12 to store driver compilations of application shaders.</summary>
	ImplicitD3DCacheForDriver = 0x00000001,
	/// <summary>Specifies a cache that's used to store Direct3D 12's conversions of one shader type to another (for example, DXBC shaders to DXIL shaders).</summary>
	ImplicitD3DConversions = 0x00000002,
	/// <summary>Specifies a cache that's managed by the driver. Operations for this cache are hints.</summary>
	ImplicitDriverManaged = 0x00000004,
	/// <summary>Specifies all shader cache sessions created by the [ID3D12Device9::CreateShaderCacheSession](nf-d3d12-id3d12device9-createshadercachesession.md) method. Requests to **CLEAR** with this flag apply to all currently active application cache sessions, as well as on-disk caches created without [D3D12_SHADER_CACHE_FLAG_USE_WORKING_DIR](ne-d3d12-d3d12_shader_cache_flags.md).</summary>
	ApplicationManaged = 0x00000008,
} ;



/// <summary>Defines constants that specify shader cache control options.</summary>
[Flags, EquivalentOf(typeof(D3D12_SHADER_CACHE_CONTROL_FLAGS))]
public enum ShaderCacheControlFlags {
	/// <summary>Specifies that the cache shouldn't be used to look up data, and shouldn't have new data stored in it. Attempts to use/create a cache while it's disabled result in **DXGI_ERROR_NOT_CURRENTLY_AVAILABLE**.</summary>
	Disable = 0x00000001,
	/// <summary>Specfies that use of the cache should be resumed.</summary>
	Enable = 0x00000002,
	/// <summary>Specfies that any existing contents of the cache should be deleted.</summary>
	Clear = 0x00000004,
} ;


[Flags, EquivalentOf(typeof(D3D12_SAMPLER_FLAGS))]
public enum SamplerFlags {
	None = 0x00000000,
	UIntBorderColor = 0x00000001,
	NonNormalizedCoordinates = 0x00000002,
} ;


/// <summary>Defines constants that specify a level of dynamic optimization to apply to GPU work that's subsequently submitted.</summary>
[EquivalentOf(typeof(D3D12_BACKGROUND_PROCESSING_MODE))]
public enum BackgroundProcessingMode {
	/// <summary>
	/// The default setting. Specifies that the driver may instrument workloads, and dynamically recompile shaders, in a low overhead,
	/// non-intrusive manner that avoids glitching the foreground workload.
	/// </summary>
	Allowed = 0,
	/// <summary>
	/// Specifies that the driver may instrument as aggressively as possible.
	/// The understanding is that causing glitches is fine while in this mode, because the current work is being submitted specifically to train the system.
	/// </summary>
	AllowIntrusiveMeasurements = 1,
	/// <summary>
	/// Specifies that background work should stop. This ensures that background shader recompilation won't consume CPU cycles.
	/// Available only in <b>Developer mode</b>.
	/// </summary>
	DisableBackgroundWork = 2,
	/// <summary>
	/// Specifies that all dynamic optimization should be disabled.
	/// For example, if you're doing an A/B performance comparison, then using this constant ensures that the driver doesn't change anything
	/// that might interfere with your results. Available only in <b>Developer mode</b>.
	/// </summary>
	DisableProfilingBySystem = 3,
} ;


/// <summary>
/// Specifies the type of serialized data.
/// Use a value from this enumeration when calling <see cref="IDevice5.CheckDriverMatchingIdentifier"/>
/// </summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_serialized_data_type">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_SERIALIZED_DATA_TYPE))]
public enum SerializedDataType {
	/// <summary>The serialized data is a raytracing acceleration structure.</summary>
	RaytracingAccelerationStructure = 0,
} ;


/// <summary>Defines constants that specify what should be done with the results of earlier workload instrumentation.</summary>
[EquivalentOf(typeof(D3D12_MEASUREMENTS_ACTION))]
public enum MeasurementsAction {
	/// <summary>The default setting. Specifies that all results should be kept.</summary>
	KeepAll = 0,
	/// <summary>Specifies that the driver has seen all the data that it's ever going to, so it should stop waiting for more and go ahead compiling optimized shaders.</summary>
	CommitResults = 1,
	/// <summary>Like <b>D3D12_MEASUREMENTS_ACTION_COMMIT_RESULTS</b>, but also specifies that your application doesn't care about glitches, so the runtime should ignore the usual idle priority rules and go ahead using as many threads as possible to get shader recompiles done fast. Available only in <b>Developer mode</b>.</summary>
	CommitResultsHighPriority = 2,
	/// <summary>Specifies that the optimization state should be reset; hinting that whatever has previously been measured no longer applies.</summary>
	DiscardPrevious = 3,
} ;


/// <summary>Specifies the volatility of both descriptors and the data they reference in a Root Signature 1.1 description, which can enable some driver optimizations.</summary>
/// <remarks>
/// <para>This enum is used by the  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_range1">D3D12_DESCRIPTOR_RANGE1</a> structure. To specify the volatility of just the data referenced by descriptors, refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_root_descriptor_flags">D3D12_ROOT_DESCRIPTOR_FLAGS</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_range_flags#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[Flags, EquivalentOf(typeof(D3D12_DESCRIPTOR_RANGE_FLAGS))]
public enum DescriptorRangeFlags {
	/// <summary>Default behavior. Descriptors are static, and default assumptions are made for data (for SRV/CBV: DATA_STATIC_WHILE_SET_AT_EXECUTE, and for UAV: DATA_VOLATILE).</summary>
	None = 0x00000000,
	/// <summary>
	/// <para>If this is the only flag set, then descriptors are volatile and default assumptions are made about data
	/// (for SRV/CBV: DATA_STATIC_WHILE_SET_AT_EXECUTE, and for UAV: DATA_VOLATILE). If this flag is combined with
	/// DATA_VOLATILE, then both descriptors and data are volatile, which is equivalent to Root Signature Version 1.0.
	/// If this flag is combined with DATA_STATIC_WHILE_SET_AT_EXECUTE, then descriptors are volatile. This still doesn’t
	/// allow them to change during command list execution so it is valid to combine the additional declaration that data
	/// is static while set via root descriptor table during execution – the underlying descriptors are effectively static
	/// for longer than the data is being promised to be static.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_range_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	DescriptorsVolatile = 0x00000001,
	/// <summary>Descriptors are static and the data is volatile.</summary>
	DataVolatile = 0x00000002,
	/// <summary>Descriptors are static and data is static while set at execute.</summary>
	DataStaticWhileSetAtExecute = 0x00000004,
	/// <summary>Both descriptors and data are static. This maximizes the potential for driver optimization.</summary>
	DataStatic = 0x00000008,
	/// <summary>Provides the same benefits as static descriptors (see **NONE**), except that the driver is not allowed to promote buffers to root descriptors as an optimization, because they must maintain bounds checks and root descriptors do not have those.</summary>
	DescriptorsStaticKeepingBufferBoundsChecks = 0x00010000,
} ;