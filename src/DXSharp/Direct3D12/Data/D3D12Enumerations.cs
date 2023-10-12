using Windows.Win32.Graphics.Direct3D12 ;

namespace DXSharp.Direct3D12 ;


[Equivalent( typeof( D3D12_COMMAND_LIST_TYPE ) )]
public enum CommandListType {
	/// <summary>Specifies a command buffer that the GPU can execute. A direct command list doesn't inherit any GPU state.</summary>
	DIRECT = 0,
	/// <summary>Specifies a command buffer that can be executed only directly via a direct command list. A bundle command list inherits all GPU state (except for the currently set pipeline state object and primitive topology).</summary>
	BUNDLE = 1,
	/// <summary>Specifies a command buffer for computing.</summary>
	COMPUTE = 2,
	/// <summary>Specifies a command buffer for copying.</summary>
	COPY = 3,
	/// <summary>Specifies a command buffer for video decoding.</summary>
	VIDEO_DECODE = 4,
	/// <summary>Specifies a command buffer for video processing.</summary>
	VIDEO_PROCESS = 5,
	VIDEO_ENCODE = 6,
	NONE         = -1,
}

[Flags, Equivalent( typeof( D3D12_COMMAND_QUEUE_FLAGS ) )]
public enum CommandQueueFlags {
	/// <summary>Indicates a default command queue.</summary>
	NONE = 0x00000000,
	/// <summary>Indicates that the GPU timeout should be disabled for this command queue.</summary>
	DISABLE_GPU_TIMEOUT = 0x00000001,
}

[Flags, Equivalent( typeof( D3D12_TILE_RANGE_FLAGS ) )]
public enum TileRangeFlags {
	/// <summary>No tile-mapping flags are specified.</summary>
	NONE = 0,
	/// <summary>The tile range is <b>NULL</b>.</summary>
	NULL = 1,
	/// <summary>Skip the tile range.</summary>
	SKIP = 2,
	/// <summary>Reuse a single tile in the tile range.</summary>
	REUSE_SINGLE_TILE = 4,
}

[Flags, Equivalent( typeof( D3D12_TILE_MAPPING_FLAGS ) )]
public enum TileMappingFlags {
	/// <summary>No tile-mapping flags are specified.</summary>
	NONE = 0x00000000,
	/// <summary>Unsupported, do not use.</summary>
	NO_HAZARD = 0x00000001,
}

[Flags, Equivalent( typeof( D3D12_HEAP_FLAGS ) )]
public enum HeapFlags {
	/// <summary>No options are specified.</summary>
	NONE = 0x00000000,
	/// <summary>The heap is shared. Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/shared-heaps">Shared Heaps</a>.</summary>
	SHARED = 0x00000001,
	/// <summary>The heap isn't allowed to contain buffers.</summary>
	DENY_BUFFERS = 0x00000004,
	/// <summary>The heap is allowed to contain swap-chain surfaces.</summary>
	ALLOW_DISPLAY = 0x00000008,
	/// <summary>The heap is allowed to share resources across adapters. Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/shared-heaps">Shared Heaps</a>. A protected session cannot be mixed with resources that are shared across adapters.</summary>
	SHARED_CROSS_ADAPTER = 0x00000020,
	/// <summary>The heap is not allowed to store Render Target (RT) and/or Depth-Stencil (DS) textures.</summary>
	DENY_RT_DS_TEXTURES = 0x00000040,
	/// <summary>The heap is not allowed to contain resources with D3D12_RESOURCE_DIMENSION_TEXTURE1D, D3D12_RESOURCE_DIMENSION_TEXTURE2D, or D3D12_RESOURCE_DIMENSION_TEXTURE3D  unless either D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET or D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL are present. Refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_dimension">D3D12_RESOURCE_DIMENSION</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_flags">D3D12_RESOURCE_FLAGS</a>.</summary>
	DENY_NON_RT_DS_TEXTURES = 0x00000080,
	/// <summary>Unsupported. Do not use.</summary>
	HARDWARE_PROTECTED = 0x00000100,
	/// <summary>The heap supports MEM_WRITE_WATCH functionality, which causes the system to track the pages that are written to in the committed memory region. This flag can't be combined with the D3D12_HEAP_TYPE_DEFAULT or D3D12_CPU_PAGE_PROPERTY_UNKNOWN flags. Applications are discouraged from using this flag themselves because it prevents tools from using this functionality.</summary>
	ALLOW_WRITE_WATCH = 0x00000200,
	/// <summary>
	/// <para>Ensures that atomic operations will be atomic on this heap's memory, according to components able to see the memory. Creating a heap with this flag will fail under either of these conditions. - The heap type is **D3D12_HEAP_TYPE_DEFAULT**, and the heap can be visible on multiple nodes, but the device does *not* support [**D3D12_CROSS_NODE_SHARING_TIER_3**](./ne-d3d12-d3d12_cross_node_sharing_tier.md). - The heap is CPU-visible, but the heap type is *not* **D3D12_HEAP_TYPE_CUSTOM**. Note that heaps with this flag might be a limited resource on some systems.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	ALLOW_SHADER_ATOMICS = 0x00000400,
	/// <summary>
	/// <para>The heap is created in a non-resident state and must be made resident using [ID3D12Device::MakeResident](./nf-d3d12-id3d12device-makeresident.md) or [ID3D12Device3::EnqueueMakeResident](./nf-d3d12-id3d12device3-enqueuemakeresident.md). By default, the final step of heap creation is to make the heap resident, so this flag skips this step and allows the application to decide when to do so.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	CREATE_NOT_RESIDENT = 0x00000800,
	/// <summary>Allows the OS to not zero the heap created. By default, committed resources and heaps are almost always zeroed upon creation. This flag allows this to be elided in some scenarios. However, it doesn't guarantee it. For example, memory coming from other processes still needs to be zeroed for data protection and process isolation. This can lower the overhead of creating the heap.</summary>
	CREATE_NOT_ZEROED = 0x00001000,
	TOOLS_USE_MANUAL_WRITE_TRACKING = 0x00002000,
	/// <summary>The heap is allowed to store all types of buffers and/or textures. This is an alias; for more details, see "Aliases" in the Remarks section.</summary>
	ALLOW_ALL_BUFFERS_AND_TEXTURES = 0x00000000,
	/// <summary>The heap is only allowed to store buffers. This is an alias; for more details, see "Aliases" in the Remarks section.</summary>
	ALLOW_ONLY_BUFFERS = 0x000000C0,
	/// <summary>The heap is only allowed to store non-RT, non-DS textures. This is an alias; for more details, see "Aliases" in the Remarks section.</summary>
	ALLOW_ONLY_NON_RT_DS_TEXTURES = 0x00000044,
	/// <summary>The heap is only allowed to store RT and/or DS textures. This is an alias; for more details, see "Aliases" in the Remarks section.</summary>
	ALLOW_ONLY_RT_DS_TEXTURES = 0x00000084,
}

[Flags, Equivalent( typeof( D3D12_RESOURCE_FLAGS ) )]
public enum ResourceFlags {
	/// <summary>No options are specified.</summary>
	NONE = 0x00000000,

	/// <summary>
	/// <para>Allows a render target view to be created for the resource; and also enables the resource to transition into the state of [D3D12_RESOURCE_STATE_RENDER_TARGET](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states). Some adapter architectures allocate extra memory for textures with this flag to reduce the effective bandwidth during common rendering. This characteristic may not be beneficial for textures that are never rendered to, nor is it available for textures compressed with BC formats. Your application should avoid setting this flag when rendering will never occur. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	ALLOW_RENDER_TARGET = 0x00000001,

	/// <summary>
	/// <para>Allows a depth stencil view to be created for the resource, as well as enables the resource to transition into the state of [D3D12_RESOURCE_STATE_DEPTH_WRITE](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states) and/or **D3D12_RESOURCE_STATE_DEPTH_READ**. Most adapter architectures allocate extra memory for textures with this flag to reduce the effective bandwidth, and maximize optimizations for early depth-test. Your application should avoid setting this flag when depth operations will never occur. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	ALLOW_DEPTH_STENCIL = 0x00000002,

	/// <summary>
	/// <para>Allows an unordered access view to be created for the resource, as well as enables the resource to transition into the state of [D3D12_RESOURCE_STATE_UNORDERED_ACCESS](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states). Some adapter architectures must resort to less efficient texture layouts in order to provide this functionality. If a texture is rarely used for unordered access, then it might be worth having two textures around and copying between them. One texture would have this flag, while the other wouldn't. Your application should avoid setting this flag when unordered access operations will never occur. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	ALLOW_UNORDERED_ACCESS = 0x00000004,

	/// <summary>
	/// <para>Disallows a shader resource view from being created for the resource, as well as disables the resource from transitioning into the state of [D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE](/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states) or **D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE**. Some adapter architectures experience increased bandwidth for depth stencil textures when shader resource views are precluded. If a texture is rarely used for shader resources, then it might be worth having two textures around and copying between them. One texture would have this flag, while the other wouldn't. Your application should set this flag when depth stencil textures will never be used from shader resource views. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	DENY_SHADER_RESOURCE = 0x00000008,

	/// <summary>
	/// <para>Allows the resource to be used for cross-adapter data, as well as those features enabled by **D3D12_RESOURCE_FLAGS::ALLOW_SIMULTANEOUS_ACCESS**. Cross-adapter resources commonly preclude techniques that reduce effective texture bandwidth during usage, and some adapter architectures might require different caching behavior. Your application should avoid setting this flag when the resource data will never be used with another adapter. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	ALLOW_CROSS_ADAPTER = 0x00000010,

	/// <summary>
	/// <para>Allows a resource to be simultaneously accessed by multiple different queues, devices, or processes (for example, allows a resource to be used with [ResourceBarrier](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-resourcebarrier) transitions performed in more than one command list executing at the same time). Simultaneous access allows multiple readers and one writer, as long as the writer doesn't concurrently modify the texels that other readers are accessing. Some adapter architectures can't leverage techniques to reduce effective texture bandwidth during usage. However, your application should avoid setting this flag when multiple readers are not required during frequent, non-overlapping writes to textures. Use of this flag can compromise resource fences to perform waits, and prevent any compression being used with a resource. The following restrictions and interactions apply: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	ALLOW_SIMULTANEOUS_ACCESS = 0x00000020,

	/// <summary>
	/// <para>Specfies that this resource may be used only as a decode reference frame. It may be written to or read only by the video decode operation. [D3D12_VIDEO_DECODE_TIER_1](../d3d12video/ne-d3d12video-d3d12_video_decode_tier.md) and [D3D12_VIDEO_DECODE_TIER_2](../d3d12video/ne-d3d12video-d3d12_video_decode_tier.md) may report [D3D12_VIDEO_DECODE_CONFIGURATION_FLAG_REFERENCE_ONLY_ALLOCATIONS_REQUIRED](../d3d12video/ne-d3d12video-d3d12_video_decode_configuration_flags.md) in the [D3D12_FEATURE_DATA_VIDEO_DECODE_SUPPORT](../d3d12video/ns-d3d12video-d3d12_feature_data_video_decode_support.md) structure configuration flag. If that happens, then your application must allocate reference frames with the **D3D12_RESOURCE_FLAGS::VIDEO_DECODE_REFERENCE_ONLY** resource flag. [D3D12_VIDEO_DECODE_TIER_3](../d3d12video/ne-d3d12video-d3d12_video_decode_tier.md) must not set the [D3D12_VIDEO_DECODE_CONFIGURATION_FLAG_REFERENCE_ONLY_ALLOCATIONS_REQUIRED] (../d3d12video/ne-d3d12video-d3d12_video_decode_configuration_flags) configuration flag, and must not require the use of this resource flag.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	VIDEO_DECODE_REFERENCE_ONLY = 0x00000040,

	/// <summary>Specfies that this resource may be used only as an encode reference frame. It may be written to or read only by the video encode operation.</summary>
	VIDEO_ENCODE_REFERENCE_ONLY = 0x00000080,

	/// <summary>
	/// <para>Reserved for future use. Don't use. Requires the DirectX 12 Agility SDK 1.7 or later. Indicates that a buffer is to be used as a raytracing acceleration structure.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	RAYTRACING_ACCELERATION_STRUCTURE = 0x00000100,
} ;

[Flags, Equivalent( typeof( D3D12_PIPELINE_STATE_FLAGS ) )]
public enum PipelineStateFlags {
	/// <summary>Indicates no flags.</summary>
	NONE = 0x00000000,
	/// <summary>
	/// <para>Indicates that the pipeline state should be compiled with additional information to assist debugging. This can only be set on WARP devices.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	TOOL_DEBUG                     = 0x00000001,
	DYNAMIC_DEPTH_BIAS             = 0x00000004,
	DYNAMIC_INDEX_BUFFER_STRIP_CUT = 0x00000008,
} ;

[Flags, Equivalent( typeof( D3D12_DESCRIPTOR_HEAP_FLAGS ) )]
public enum DescriptorHeapFlags {
	/// <summary>Indicates default usage of a heap.</summary>
	NONE = 0x00000000,
	/// <summary>
	/// <para>The flag [D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE](/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_flags) can optionally be set on a descriptor heap to indicate it is be bound on a command list for reference by shaders. Descriptor heaps created <i>without</i> this flag allow applications the option to stage descriptors in CPU memory before copying them to a shader visible descriptor heap, as a convenience. But it is also fine for applications to directly create descriptors into shader visible descriptor heaps with no requirement to stage anything on the CPU. Descriptor heaps bound via [ID3D12GraphicsCommandList::SetDescriptorHeaps](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setdescriptorheaps) must have the **D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE** flag set, else the debug layer will produce an error. Descriptor heaps with the **D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE** flag can't be used as the source heaps in calls to [ID3D12Device::CopyDescriptors](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors) or [ID3D12Device::CopyDescriptorsSimple](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple), because they could be resident in **WRITE_COMBINE** memory or GPU-local memory, which is very inefficient to read from. This flag only applies to CBV/SRV/UAV descriptor heaps, and sampler descriptor heaps. It does not apply to other descriptor heap types since shaders do not directly reference the other types. Attempting to create an RTV/DSV heap with **D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE** results in a debug layer error.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	SHADER_VISIBLE = 0x00000001,
} ;



[Equivalent( typeof( D3D12_DESCRIPTOR_HEAP_TYPE ) )]
public enum DescriptorHeapType {
	/// <summary>The descriptor heap for the combination of constant-buffer, shader-resource, and unordered-access views.</summary>
	CBV_SRV_UAV = 0,
	/// <summary>The descriptor heap for the sampler.</summary>
	SAMPLER = 1,
	/// <summary>The descriptor heap for the render-target view.</summary>
	RTV = 2,
	/// <summary>The descriptor heap for the depth-stencil view.</summary>
	DSV = 3,
	/// <summary>The number of types of descriptor heaps.</summary>
	NUM_TYPES = 4,
} ;



[Equivalent( typeof( D3D12_SRV_DIMENSION ) )]
public enum SRVDimension {
	/// <summary>The type is unknown.</summary>
	UNKNOWN = 0,
	/// <summary>The resource is a buffer.</summary>
	BUFFER = 1,
	/// <summary>The resource is a 1D texture.</summary>
	TEXTURE1D = 2,
	/// <summary>The resource is an array of 1D textures.</summary>
	TEXTURE1DARRAY = 3,
	/// <summary>The resource is a 2D texture.</summary>
	TEXTURE2D = 4,
	/// <summary>The resource is an array of 2D textures.</summary>
	TEXTURE2DARRAY = 5,
	/// <summary>The resource is a multisampling 2D texture.</summary>
	TEXTURE2DMS = 6,
	/// <summary>The resource is an array of multisampling 2D textures.</summary>
	TEXTURE2DMSARRAY = 7,
	/// <summary>The resource is a 3D texture.</summary>
	TEXTURE3D = 8,
	/// <summary>The resource is a cube texture.</summary>
	TEXTURECUBE = 9,
	/// <summary>The resource is an array of cube textures.</summary>
	TEXTURECUBEARRAY = 10,
	/// <summary>The resource is a raytracing acceleration structure.</summary>
	RAYTRACING_ACCELERATION_STRUCTURE = 11,
} ;