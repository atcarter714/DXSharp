using Windows.Win32.Graphics.Direct3D12 ;

namespace DXSharp.Direct3D12 ;


[EquivalentOf( typeof( D3D12_COMMAND_LIST_TYPE ) )]
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

[Flags, EquivalentOf( typeof( D3D12_COMMAND_QUEUE_FLAGS ) )]
public enum CommandQueueFlags {
	/// <summary>Indicates a default command queue.</summary>
	NONE = 0x00000000,
	/// <summary>Indicates that the GPU timeout should be disabled for this command queue.</summary>
	DISABLE_GPU_TIMEOUT = 0x00000001,
}

[Flags, EquivalentOf( typeof( D3D12_TILE_RANGE_FLAGS ) )]
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

[Flags, EquivalentOf( typeof( D3D12_TILE_MAPPING_FLAGS ) )]
public enum TileMappingFlags {
	/// <summary>No tile-mapping flags are specified.</summary>
	NONE = 0x00000000,
	/// <summary>Unsupported, do not use.</summary>
	NO_HAZARD = 0x00000001,
}

[Flags, EquivalentOf( typeof( D3D12_HEAP_FLAGS ) )]
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
	/// <summary>The heap is not allowed to contain resources with TEXTURE1D, TEXTURE2D, or TEXTURE3D  unless either D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET or D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL are present. Refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_dimension">D3D12_RESOURCE_DIMENSION</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_flags">D3D12_RESOURCE_FLAGS</a>.</summary>
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

[Flags, EquivalentOf( typeof( D3D12_RESOURCE_FLAGS ) )]
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

[Flags, EquivalentOf( typeof( D3D12_PIPELINE_STATE_FLAGS ) )]
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

[Flags, EquivalentOf( typeof( D3D12_DESCRIPTOR_HEAP_FLAGS ) )]
public enum DescriptorHeapFlags {
	/// <summary>Indicates default usage of a heap.</summary>
	NONE = 0x00000000,
	/// <summary>
	/// <para>The flag [D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE](/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_flags) can optionally be set on a descriptor heap to indicate it is be bound on a command list for reference by shaders. Descriptor heaps created <i>without</i> this flag allow applications the option to stage descriptors in CPU memory before copying them to a shader visible descriptor heap, as a convenience. But it is also fine for applications to directly create descriptors into shader visible descriptor heaps with no requirement to stage anything on the CPU. Descriptor heaps bound via [ID3D12GraphicsCommandList::SetDescriptorHeaps](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-setdescriptorheaps) must have the **D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE** flag set, else the debug layer will produce an error. Descriptor heaps with the **D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE** flag can't be used as the source heaps in calls to [ID3D12Device::CopyDescriptors](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptors) or [ID3D12Device::CopyDescriptorsSimple](/windows/win32/api/d3d12/nf-d3d12-id3d12device-copydescriptorssimple), because they could be resident in **WRITE_COMBINE** memory or GPU-local memory, which is very inefficient to read from. This flag only applies to CBV/SRV/UAV descriptor heaps, and sampler descriptor heaps. It does not apply to other descriptor heap types since shaders do not directly reference the other types. Attempting to create an RTV/DSV heap with **D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE** results in a debug layer error.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_descriptor_heap_flags#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	SHADER_VISIBLE = 0x00000001,
} ;



[EquivalentOf( typeof( D3D12_DESCRIPTOR_HEAP_TYPE ) )]
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



[EquivalentOf( typeof( D3D12_SRV_DIMENSION ) )]
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


[Flags, EquivalentOf( typeof( D3D12_BUFFER_SRV_FLAGS ) )]
public enum BufferSRVFlags {
	/// <summary>Indicates a default view.</summary>
	NONE = 0x00000000,
	/// <summary>View the buffer as raw. For more info about raw viewing of buffers, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-resources-intro">Raw Views of Buffers</a>.</summary>
	RAW = 0x00000001,
} ;

[EquivalentOf( typeof( D3D12_RTV_DIMENSION ) )]
public enum RTVDimension {
	/// <summary>Do not use this value, as it will cause <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createrendertargetview">ID3D12Device::CreateRenderTargetView</a> to fail.</summary>
	UNKNOWN = 0,
	/// <summary>The resource will be accessed as a buffer.</summary>
	BUFFER = 1,
	/// <summary>The resource will be accessed as a 1D texture.</summary>
	TEXTURE1D = 2,
	/// <summary>The resource will be accessed as an array of 1D textures.</summary>
	TEXTURE1DARRAY = 3,
	/// <summary>The resource will be accessed as a 2D texture.</summary>
	TEXTURE2D = 4,
	/// <summary>The resource will be accessed as an array of 2D textures.</summary>
	TEXTURE2DARRAY = 5,
	/// <summary>The resource will be accessed as a 2D texture with multisampling.</summary>
	TEXTURE2DMS = 6,
	/// <summary>The resource will be accessed as an array of 2D textures with multisampling.</summary>
	TEXTURE2DMSARRAY = 7,
	/// <summary>The resource will be accessed as a 3D texture.</summary>
	TEXTURE3D = 8,
} ;

[EquivalentOf( typeof(D3D12_DSV_DIMENSION) )]
public enum DSVDimension {
	/// <summary><b>UNKNOWN</b> is not a valid value for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_depth_stencil_view_desc">D3D12_DEPTH_STENCIL_VIEW_DESC</a> and is not used.</summary>
	UNKNOWN = 0,
	/// <summary>The resource will be accessed as a 1D texture.</summary>
	TEXTURE1D = 1,
	/// <summary>The resource will be accessed as an array of 1D textures.</summary>
	TEXTURE1DARRAY = 2,
	/// <summary>The resource will be accessed as a 2D texture.</summary>
	TEXTURE2D = 3,
	/// <summary>The resource will be accessed as an array of 2D textures.</summary>
	TEXTURE2DARRAY = 4,
	/// <summary>The resource will be accessed as a 2D texture with multi sampling.</summary>
	TEXTURE2DMS = 5,
	/// <summary>The resource will be accessed as an array of 2D textures with multi sampling.</summary>
	TEXTURE2DMSARRAY = 6,
} ;

[Flags, EquivalentOf( typeof( D3D12_DSV_FLAGS ) )]
public enum DSVFlags {
	/// <summary>Indicates a default view.</summary>
	D3D12_DSV_FLAG_NONE = 0x00000000,
	/// <summary>Indicates that depth values are read only.</summary>
	D3D12_DSV_FLAG_READ_ONLY_DEPTH = 0x00000001,
	/// <summary>Indicates that stencil values are read only.</summary>
	D3D12_DSV_FLAG_READ_ONLY_STENCIL = 0x00000002,
} ;

[EquivalentOf( typeof( D3D12_FILTER ) )]
public enum Filter {
	/// <summary>Use point sampling for minification, magnification, and mip-level sampling.</summary>
	MIN_MAG_MIP_POINT = 0,
	/// <summary>Use point sampling for minification and magnification; use linear interpolation for mip-level sampling.</summary>
	MIN_MAG_POINT_MIP_LINEAR = 1,
	/// <summary>Use point sampling for minification; use linear interpolation for magnification; use point sampling for mip-level sampling.</summary>
	MIN_POINT_MAG_LINEAR_MIP_POINT = 4,
	/// <summary>Use point sampling for minification; use linear interpolation for magnification and mip-level sampling.</summary>
	MIN_POINT_MAG_MIP_LINEAR = 5,
	/// <summary>Use linear interpolation for minification; use point sampling for magnification and mip-level sampling.</summary>
	MIN_LINEAR_MAG_MIP_POINT = 16,
	/// <summary>Use linear interpolation for minification; use point sampling for magnification; use linear interpolation for mip-level sampling.</summary>
	MIN_LINEAR_MAG_POINT_MIP_LINEAR = 17,
	/// <summary>Use linear interpolation for minification and magnification; use point sampling for mip-level sampling.</summary>
	MIN_MAG_LINEAR_MIP_POINT = 20,
	/// <summary>Use linear interpolation for minification, magnification, and mip-level sampling.</summary>
	MIN_MAG_MIP_LINEAR = 21,
	MIN_MAG_ANISOTROPIC_MIP_POINT = 84,
	/// <summary>Use anisotropic interpolation for minification, magnification, and mip-level sampling.</summary>
	ANISOTROPIC = 85,
	/// <summary>Use point sampling for minification, magnification, and mip-level sampling. Compare the result to the comparison value.</summary>
	COMPARISON_MIN_MAG_MIP_POINT = 128,
	/// <summary>Use point sampling for minification and magnification; use linear interpolation for mip-level sampling. Compare the result to the comparison value.</summary>
	COMPARISON_MIN_MAG_POINT_MIP_LINEAR = 129,
	/// <summary>Use point sampling for minification; use linear interpolation for magnification; use point sampling for mip-level sampling. Compare the result to the comparison value.</summary>
	COMPARISON_MIN_POINT_MAG_LINEAR_MIP_POINT = 132,
	/// <summary>Use point sampling for minification; use linear interpolation for magnification and mip-level sampling. Compare the result to the comparison value.</summary>
	COMPARISON_MIN_POINT_MAG_MIP_LINEAR = 133,
	/// <summary>Use linear interpolation for minification; use point sampling for magnification and mip-level sampling. Compare the result to the comparison value.</summary>
	COMPARISON_MIN_LINEAR_MAG_MIP_POINT = 144,
	/// <summary>Use linear interpolation for minification; use point sampling for magnification; use linear interpolation for mip-level sampling. Compare the result to the comparison value.</summary>
	COMPARISON_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 145,
	/// <summary>Use linear interpolation for minification and magnification; use point sampling for mip-level sampling. Compare the result to the comparison value.</summary>
	COMPARISON_MIN_MAG_LINEAR_MIP_POINT = 148,
	/// <summary>Use linear interpolation for minification, magnification, and mip-level sampling. Compare the result to the comparison value.</summary>
	COMPARISON_MIN_MAG_MIP_LINEAR = 149,
	COMPARISON_MIN_MAG_ANISOTROPIC_MIP_POINT = 212,
	/// <summary>Use anisotropic interpolation for minification, magnification, and mip-level sampling. Compare the result to the comparison value.</summary>
	COMPARISON_ANISOTROPIC = 213,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_MIP_POINT</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MINIMUM_MIN_MAG_MIP_POINT = 256,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_POINT_MIP_LINEAR</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MINIMUM_MIN_MAG_POINT_MIP_LINEAR = 257,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_POINT_MAG_LINEAR_MIP_POINT</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MINIMUM_MIN_POINT_MAG_LINEAR_MIP_POINT = 260,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_POINT_MAG_MIP_LINEAR</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MINIMUM_MIN_POINT_MAG_MIP_LINEAR = 261,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_LINEAR_MAG_MIP_POINT</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MINIMUM_MIN_LINEAR_MAG_MIP_POINT = 272,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_LINEAR_MAG_POINT_MIP_LINEAR</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MINIMUM_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 273,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_LINEAR_MIP_POINT</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MINIMUM_MIN_MAG_LINEAR_MIP_POINT = 276,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_MIP_LINEAR</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MINIMUM_MIN_MAG_MIP_LINEAR = 277,
	MINIMUM_MIN_MAG_ANISOTROPIC_MIP_POINT = 340,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">ANISOTROPIC</a> and instead of filtering them return the minimum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the minimum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MINIMUM_ANISOTROPIC = 341,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_MIP_POINT</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MAXIMUM_MIN_MAG_MIP_POINT = 384,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_POINT_MIP_LINEAR</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MAXIMUM_MIN_MAG_POINT_MIP_LINEAR = 385,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_POINT_MAG_LINEAR_MIP_POINT</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MAXIMUM_MIN_POINT_MAG_LINEAR_MIP_POINT = 388,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_POINT_MAG_MIP_LINEAR</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MAXIMUM_MIN_POINT_MAG_MIP_LINEAR = 389,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_LINEAR_MAG_MIP_POINT</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MAXIMUM_MIN_LINEAR_MAG_MIP_POINT = 400,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_LINEAR_MAG_POINT_MIP_LINEAR</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MAXIMUM_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 401,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_LINEAR_MIP_POINT</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MAXIMUM_MIN_MAG_LINEAR_MIP_POINT = 404,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">MIN_MAG_MIP_LINEAR</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MAXIMUM_MIN_MAG_MIP_LINEAR = 405,
	MAXIMUM_MIN_MAG_ANISOTROPIC_MIP_POINT = 468,
	/// <summary>Fetch the same set of texels as <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_filter">ANISOTROPIC</a> and instead of filtering them return the maximum of the texels.  Texels that are weighted 0 during filtering aren't counted towards the maximum.  You can query support for this filter type from the <b>MinMaxFiltering</b> member in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options1">D3D11_FEATURE_DATA_D3D11_OPTIONS1</a> structure.</summary>
	MAXIMUM_ANISOTROPIC = 469,
} ;

[EquivalentOf( typeof( D3D12_TEXTURE_ADDRESS_MODE ) )]
public enum TextureAddressMode {
	/// <summary>
	/// <para>Tile the texture at every (u,v) integer junction. For example, for u values between 0 and 3, the texture is repeated three times.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_address_mode#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	WRAP = 1,
	/// <summary>
	/// <para>Flip the texture at every (u,v) integer junction. For u values between 0 and 1, for example, the texture is addressed normally; between 1 and 2, the texture is flipped (mirrored); between 2 and 3, the texture is normal again; and so on.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_address_mode#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	MIRROR = 2,
	/// <summary>Texture coordinates outside the range [0.0, 1.0] are set to the texture color at 0.0 or 1.0, respectively.</summary>
	CLAMP = 3,
	/// <summary>Texture coordinates outside the range [0.0, 1.0] are set to the border color specified in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_sampler_desc">D3D12_SAMPLER_DESC</a> or HLSL code.</summary>
	BORDER = 4,
	/// <summary>
	/// <para>Similar to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">MIRROR</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_address_mode">CLAMP</a>. Takes the absolute value of the texture coordinate (thus, mirroring around 0), and then clamps to the maximum value.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_address_mode#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	MIRROR_ONCE = 5,
} ;

[EquivalentOf( typeof( D3D12_COMPARISON_FUNC ) )]
public enum ComparisonFunction {
	NONE = 0,
	/// <summary>Never pass the comparison.</summary>
	NEVER = 1,
	/// <summary>If the source data is less than the destination data, the comparison passes.</summary>
	LESS = 2,
	/// <summary>If the source data is equal to the destination data, the comparison passes.</summary>
	EQUAL = 3,
	/// <summary>If the source data is less than or equal to the destination data, the comparison passes.</summary>
	LESS_EQUAL = 4,
	/// <summary>If the source data is greater than the destination data, the comparison passes.</summary>
	GREATER = 5,
	/// <summary>If the source data is not equal to the destination data, the comparison passes.</summary>
	NOT_EQUAL = 6,
	/// <summary>If the source data is greater than or equal to the destination data, the comparison passes.</summary>
	GREATER_EQUAL = 7,
	/// <summary>Always pass the comparison.</summary>
	ALWAYS = 8,
} ;

[EquivalentOf( typeof( D3D12_RESOURCE_DIMENSION ) )]
public enum ResourceDimension {
	/// <summary>Resource is of unknown type.</summary>
	UNKNOWN = 0,
	/// <summary>Resource is a buffer.</summary>
	BUFFER = 1,
	/// <summary>Resource is a 1D texture.</summary>
	TEXTURE1D = 2,
	/// <summary>Resource is a 2D texture.</summary>
	TEXTURE2D = 3,
	/// <summary>Resource is a 3D texture.</summary>
	TEXTURE3D = 4,
} ;

[EquivalentOf( typeof( D3D12_TEXTURE_LAYOUT ) )]
public enum TextureLayout {
	/// <summary>
	/// <para>Indicates that the layout is unknown, and is likely adapter-dependent. During creation, the driver chooses the most efficient layout based on other resource properties, especially resource size and flags. Prefer this choice unless certain functionality is required from another texture layout. Zero-copy texture upload optimizations exist for UMA architectures; see <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-writetosubresource">ID3D12Resource::WriteToSubresource</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_layout#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	UNKNOWN = 0,
	/// <summary>
	/// <para>Indicates that data for the texture is stored in row-major order (sometimes called "pitch-linear order"). This texture layout locates consecutive texels of a row contiguously in memory, before the texels of the next row. Similarly, consecutive texels of a particular depth or array slice are contiguous in memory before the texels of the next depth or array slice. Padding may exist between rows and between depth or array slices to align collections of data. A stride is the distance in memory between rows, depth, or array slices; and it includes any padding. This texture layout enables sharing of the texture data between multiple adapters, when other layouts aren't available. Many restrictions apply, because this layout is generally not efficient for extensive usage: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_layout#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	ROW_MAJOR = 1,
	/// <summary>
	/// <para>Indicates that the layout within 64KB tiles and tail mip packing is up to the driver. No standard swizzle pattern. This texture layout is arranged into contiguous 64KB regions, also known as tiles, containing near equilateral amount of consecutive number of texels along each dimension. Tiles are arranged in row-major order. While there is no padding between tiles, there are typically unused texels within the last tile in each dimension. The layout of texels within the tile is undefined. Each subresource immediately follows where the previous subresource end, and the subresource order follows the same sequence as subresource ordinals. However, tail mip packing is adapter-specific. For more details, see tiled resource tier and <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getresourcetiling">ID3D12Device::GetResourceTiling</a>. This texture layout enables partially resident or sparse texture scenarios when used together with virtual memory page mapping functionality. This texture layout must be used together with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-createreservedresource">ID3D12Device::CreateReservedResource</a> to enable the usage of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-updatetilemappings">ID3D12CommandQueue::UpdateTileMappings</a>. Some restrictions apply to textures with this layout: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_layout#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	_64KB_UNDEFINED_SWIZZLE = 2,
	/// <summary>
	/// <para>Indicates that a default texture uses the standardized swizzle pattern. This texture layout is arranged the same way that 64KB_UNDEFINED_SWIZZLE is, except that the layout of texels within the tile is defined. Tail mip packing is adapter-specific. This texture layout enables optimizations when marshaling data between multiple adapters or between the CPU and GPU. The amount of copying can be reduced when multiple components understand the texture memory layout. This layout is generally more efficient for extensive usage than row-major layout, due to the rotationally invariant locality of neighboring texels. This layout can typically only be used with adapters that support standard swizzle, but exceptions exist for cross-adapter shared heaps. The restrictions for this layout are that the following aren't supported: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_layout#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	_64KB_STANDARD_SWIZZLE = 3,
} ;

[EquivalentOf( typeof( D3D12_HEAP_TYPE ) )]
public enum HeapType {
	/// <summary>Specifies the default heap. This heap type experiences the most bandwidth for the GPU, but cannot provide CPU access. The GPU can read and write to the memory from this pool, and resource transition barriers may be changed. The majority of heaps and resources are expected to be located here, and are typically populated through resources in upload heaps.</summary>
	DEFAULT = 1,
	/// <summary>
	/// <para>Specifies a heap used for uploading. This heap type has CPU access optimized for uploading to the GPU, but does not experience the maximum amount of bandwidth for the GPU. This heap type is best for CPU-write-once, GPU-read-once data; but GPU-read-once is stricter than necessary. GPU-read-once-or-from-cache is an acceptable use-case for the data; but such usages are hard to judge due to differing GPU cache designs and sizes. If in doubt, stick to the GPU-read-once definition or profile the difference on many GPUs between copying the data to a _DEFAULT heap vs. reading the data from an _UPLOAD heap. Resources in this heap must be created with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE</a>_GENERIC_READ and cannot be changed away from this. The CPU address for such heaps is commonly not efficient for CPU reads. The following are typical usages for _UPLOAD heaps: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	UPLOAD = 2,
	/// <summary>
	/// <para>Specifies a heap used for reading back. This heap type has CPU access optimized for reading data back from the GPU, but does not experience the maximum amount of bandwidth for the GPU. This heap type is best for GPU-write-once, CPU-readable data. The CPU cache behavior is write-back, which is conducive for multiple sub-cache-line CPU reads. Resources in this heap must be created with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE</a>_COPY_DEST, and cannot be changed away from this.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	READBACK = 3,
	/// <summary>
	/// <para>Specifies a custom heap. The application may specify the memory pool and CPU cache properties directly, which can be useful for UMA optimizations, multi-engine, multi-adapter, or other special cases. To do so, the application is expected to understand the adapter architecture to make the right choice. For more details, see <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a>_ARCHITECTURE, <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture">D3D12_FEATURE_DATA_ARCHITECTURE</a>, and <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcustomheapproperties">GetCustomHeapProperties</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_heap_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	CUSTOM = 4,
	GPU_UPLOAD = 5,
} ;

[Flags, EquivalentOf( typeof( D3D12_RESOURCE_STATES ) )]
public enum ResourceStates {
	/// <summary>
	/// <para>Your application should transition to this state only for accessing a resource across different graphics engine types. Specifically, a resource must be in the COMMON state before being used on a COPY queue (when previously used on DIRECT/COMPUTE), and before being used on DIRECT/COMPUTE (when previously used on COPY). This restriction doesn't exist when accessing data between DIRECT and COMPUTE queues. The COMMON state can be used for all usages on a Copy queue using the implicit state transitions. For more info, in <a href="https://docs.microsoft.com/windows/win32/direct3d12/user-mode-heap-synchronization">Multi-engine synchronization</a>, find "common". Additionally, textures must be in the COMMON state for CPU access to be legal, assuming the texture was created in a CPU-visible heap in the first place.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	COMMON = 0x00000000,
	/// <summary>A subresource must be in this state when it is accessed by the GPU as a vertex buffer or constant buffer. This is a read-only state.</summary>
	VERTEX_AND_CONSTANT_BUFFER = 0x00000001,
	/// <summary>A subresource must be in this state when it is accessed by the 3D pipeline as an index buffer. This is a read-only state.</summary>
	INDEX_BUFFER = 0x00000002,
	/// <summary>
	/// <para>The resource is used as a render target. A subresource must be in this state when it is rendered to, or when it is cleared with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearrendertargetview">ID3D12GraphicsCommandList::ClearRenderTargetView</a>. This is a write-only state. To read from a render target as a shader resource, the resource must be in either **NON_PIXEL_SHADER_RESOURCE** or **PIXEL_SHADER_RESOURCE**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	RENDER_TARGET = 0x00000004,
	/// <summary>The resource is used for unordered access. A subresource must be in this state when it is accessed by the GPU via an unordered access view. A subresource must also be in this state when it is cleared with <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewuint">ID3D12GraphicsCommandList::ClearUnorderedAccessViewInt</a> or <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearunorderedaccessviewfloat">ID3D12GraphicsCommandList::ClearUnorderedAccessViewFloat</a>. This is a read/write state.</summary>
	UNORDERED_ACCESS = 0x00000008,
	/// <summary>**DEPTH_WRITE** is a state that is mutually exclusive with other states. You should use it for <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-cleardepthstencilview">ID3D12GraphicsCommandList::ClearDepthStencilView</a> when the flags (see <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_clear_flags">D3D12_CLEAR_FLAGS</a>) indicate a given subresource should be cleared (otherwise the subresource state doesn't matter), or when using it in a writable depth stencil view (see <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_dsv_flags">D3D12_DSV_FLAGS</a>) when the PSO has depth write enabled (see <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_desc">D3D12_DEPTH_STENCIL_DESC</a>).</summary>
	DEPTH_WRITE = 0x00000010,
	/// <summary>DEPTH_READ is a state that can be combined with other states. It should be used when the subresource is in a read-only depth stencil view, or when depth write of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_desc">D3D12_DEPTH_STENCIL_DESC</a> is disabled. It can be combined with other read states (for example, **PIXEL_SHADER_RESOURCE**), such that the resource can be used for the depth or stencil test, and accessed by a shader within the same draw call. Using it when depth will be written by a draw call or clear command is invalid.</summary>
	DEPTH_READ = 0x00000020,
	/// <summary>The resource is used with a shader other than the pixel shader. A subresource must be in this state before being read by any stage (except for the pixel shader stage) via a shader resource view. You can still use the resource in a pixel shader with this flag as long as it also has the flag **PIXEL_SHADER_RESOURCE** set. This is a read-only state.</summary>
	NON_PIXEL_SHADER_RESOURCE = 0x00000040,
	/// <summary>The resource is used with a pixel shader. A subresource must be in this state before being read by the pixel shader via a shader resource view. This is a read-only state.</summary>
	PIXEL_SHADER_RESOURCE = 0x00000080,
	/// <summary>The resource is used with stream output. A subresource must be in this state when it is accessed by the 3D pipeline as a stream-out target. This is a write-only state.</summary>
	STREAM_OUT = 0x00000100,
	/// <summary>
	/// <para>The resource is used as an indirect argument. Subresources must be in this state when they are used as the argument buffer passed to the indirect drawing method <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-executeindirect">ID3D12GraphicsCommandList::ExecuteIndirect</a>. This is a read-only state.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	INDIRECT_ARGUMENT = 0x00000200,
	/// <summary>
	/// <para>The resource is used as the destination in a copy operation. Subresources must be in this state when they are used as the destination of copy operation, or a blt operation. This is a write-only state.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	COPY_DEST = 0x00000400,
	/// <summary>
	/// <para>The resource is used as the source in a copy operation. Subresources must be in this state when they are used as the source of copy operation, or a blt operation. This is a read-only state.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	COPY_SOURCE = 0x00000800,
	/// <summary>The resource is used as the destination in a resolve operation.</summary>
	RESOLVE_DEST = 0x00001000,
	/// <summary>The resource is used as the source in a resolve operation.</summary>
	RESOLVE_SOURCE = 0x00002000,
	/// <summary>
	/// <para>When a buffer is created with this as its initial state, it indicates that the resource is a raytracing acceleration structure, for use in <a href="nf-d3d12-id3d12graphicscommandlist4-buildraytracingaccelerationstructure.md">ID3D12GraphicsCommandList4::BuildRaytracingAccelerationStructure</a>, <a href="nf-d3d12-id3d12graphicscommandlist4-copyraytracingaccelerationstructure.md">ID3D12GraphicsCommandList4::CopyRaytracingAccelerationStructure</a>, or <a href="nf-d3d12-id3d12device-createshaderresourceview.md">ID3D12Device::CreateShaderResourceView</a> for the <a href="ne-d3d12-d3d12_srv_dimension.md">D3D12_SRV_DIMENSION_RAYTRACING_ACCELERATION_STRUCTURE</a> dimension. > [!NOTE] > A resource to be used for the **RAYTRACING_ACCELERATION_STRUCTURE** state must be created in that state, and then never transitioned out of it. Nor may a resource that was created not in that state be transitioned into it. For more info, see [Acceleration structure memory restrictions](https://microsoft.github.io/DirectX-Specs/d3d/Raytracing.html#acceleration-structure-memory-restrictions) in the DirectX raytracing (DXR) functional specification on GitHub.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_states#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	RAYTRACING_ACCELERATION_STRUCTURE = 0x00400000,
	/// <summary>Starting with Windows 10, version 1903 (10.0; Build 18362), indicates that the resource is a screen-space shading-rate image for variable-rate shading (VRS). For more info, see <a href="https://docs.microsoft.com/windows/win32/direct3d12/vrs">Variable-rate shading (VRS)</a>.</summary>
	SHADING_RATE_SOURCE = 0x01000000,
	/// <summary>GENERIC_READ is a logically OR'd combination of other read-state bits. This is the required starting state for an upload heap. Your application should generally avoid transitioning to GENERIC_READ when possible, since that can result in premature cache flushes, or resource layout changes (for example, compress/decompress), causing unnecessary pipeline stalls. You should instead transition resources only to the actually-used states.</summary>
	GENERIC_READ = 0x00000AC3,
	/// <summary>Equivalent to `NON_PIXEL_SHADER_RESOURCE | PIXEL_SHADER_RESOURCE`.</summary>
	ALL_SHADER_RESOURCE = 0x000000C0,
	/// <summary>Synonymous with COMMON.</summary>
	PRESENT = 0x00000000,
	/// <summary>The resource is used for <a href="https://docs.microsoft.com/windows/win32/direct3d12/predication">Predication</a>.</summary>
	PREDICATION = 0x00000200,
	/// <summary>The resource is used as a source in a decode operation. Examples include reading the compressed bitstream and reading from decode references,</summary>
	VIDEO_DECODE_READ = 0x00010000,
	/// <summary>The resource is used as a destination in the decode operation. This state is used for decode output and histograms.</summary>
	VIDEO_DECODE_WRITE = 0x00020000,
	/// <summary>The resource is used to read video data during video processing; that is, the resource is used as the source in a processing operation such as video encoding (compression).</summary>
	VIDEO_PROCESS_READ = 0x00040000,
	/// <summary>The resource is used to write video data during video processing; that is, the resource is used as the destination in a processing operation such as video encoding (compression).</summary>
	VIDEO_PROCESS_WRITE = 0x00080000,
	/// <summary>The resource is used as the source in an encode operation. This state is used for the input and reference of motion estimation.</summary>
	VIDEO_ENCODE_READ = 0x00200000,
	/// <summary>This resource is used as the destination in an encode operation. This state is used for the destination texture of a resolve motion vector heap operation.</summary>
	VIDEO_ENCODE_WRITE = 0x00800000,
} ;

[EquivalentOf( typeof( D3D12_RESOURCE_BARRIER_TYPE ) )]
public enum ResourceBarrierType {
	/// <summary>A transition barrier that indicates a transition of a set of subresources between different usages. The caller must specify the before and after usages of the subresources.</summary>
	TRANSITION = 0,
	/// <summary>An aliasing barrier that indicates a transition between usages of 2 different resources that have mappings into the same tile pool. The caller can specify both the before and the after resource. Note that one or both resources can be <b>NULL</b>, which indicates that any tiled resource could cause aliasing.</summary>
	ALIASING = 1,
	/// <summary>An unordered access view (UAV) barrier that indicates all UAV accesses (reads or writes) to a particular resource must complete before any future UAV accesses (read or write) can begin.</summary>
	UAV = 2,
} ;

[EquivalentOf( typeof( D3D12_RESOURCE_BARRIER_FLAGS ) )]
public enum ResourceBarrierFlags {
	/// <summary>No flags.</summary>
	NONE = 0x00000000,
	/// <summary>This starts a barrier transition in a new state, putting a resource in a temporary no-access condition.</summary>
	BEGIN_ONLY = 0x00000001,
	/// <summary>This barrier completes a transition, setting a new state and restoring active access to a resource.</summary>
	END_ONLY = 0x00000002,
} ;

[Flags, EquivalentOf( typeof( D3D12_FENCE_FLAGS ) )]
public enum FenceFlags {
	/// <summary>No options are specified.</summary>
	NONE = 0x00000000,
	/// <summary>The fence is shared.</summary>
	SHARED = 0x00000001,
	/// <summary>The fence is shared with another GPU adapter.</summary>
	SHARED_CROSS_ADAPTER = 0x00000002,
	/// <summary>The fence is of the non-monitored type. Non-monitored fences should only be used when the adapter doesn't support monitored fences, or when a fence is shared with an adapter that doesn't support monitored fences.</summary>
	NON_MONITORED = 0x00000004,
} ;


[EquivalentOf( typeof( D3D12_QUERY_HEAP_TYPE ) )]
public enum QueryHeapType {
	/// <summary>This returns a binary 0/1 result:  0 indicates that no samples passed depth and stencil testing, 1 indicates that at least one sample passed depth and stencil testing.  This enables occlusion queries to not interfere with any GPU performance optimization associated with depth/stencil testing.</summary>
	OCCLUSION = 0,

	/// <summary>Indicates that the heap is for high-performance timing data.</summary>
	TIMESTAMP = 1,

	/// <summary>Indicates the heap is to contain pipeline data. Refer to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_query_data_pipeline_statistics">D3D12_QUERY_DATA_PIPELINE_STATISTICS</a>.</summary>
	PIPELINE_STATISTICS = 2,

	/// <summary>Indicates the heap is to contain stream output data. Refer to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_query_data_so_statistics">D3D12_QUERY_DATA_SO_STATISTICS</a>.</summary>
	SO_STATISTICS = 3,

	/// <summary>
	/// <para>Indicates the heap is to contain video decode statistics data. Refer to [D3D12_QUERY_DATA_VIDEO_DECODE_STATISTICS](../d3d12video/ns-d3d12video-d3d12_query_data_video_decode_statistics.md). Video decode statistics can only be queried from video decode command lists (<a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_command_list_type">D3D12_COMMAND_LIST_TYPE_VIDEO_DECODE</a>). See <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_type">D3D12_QUERY_TYPE_DECODE_STATISTICS</a> for more details.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_heap_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	VIDEO_DECODE_STATISTICS = 4,

	/// <summary>
	/// <para>Indicates the heap is to contain timestamp queries emitted exclusively by copy command lists. Copy queue timestamps can only be queried from a copy command list, and a copy command list can not emit to a regular timestamp query Heap. Support for this query heap type is not universal. You must use <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a> with [D3D12_FEATURE_D3D12_OPTIONS3](./ne-d3d12-d3d12_feature.md) to determine whether the adapter supports copy queue timestamp queries.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_query_heap_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	COPY_QUEUE_TIMESTAMP = 5,
	PIPELINE_STATISTICS1 = 7,
} ;

[EquivalentOf( typeof(D3D12_PRIMITIVE_TOPOLOGY_TYPE) )]
public enum PrimitiveTopology {
	/// <summary>The shader has not been initialized with an input primitive type.</summary>
	UNDEFINED = 0,
	/// <summary>Interpret the input primitive as a point.</summary>
	POINT = 1,
	/// <summary>Interpret the input primitive as a line.</summary>
	LINE = 2,
	/// <summary>Interpret the input primitive as a triangle.</summary>
	TRIANGLE = 3,
	/// <summary>Interpret the input primitive as a control point patch.</summary>
	PATCH = 4,
} ;

[EquivalentOf( typeof( D3D12_INDEX_BUFFER_STRIP_CUT_VALUE ) )]
public enum IndexBufferStripCutValue {
	/// <summary>Indicates that there is no cut value.</summary>
	DISABLED = 0,
	/// <summary>Indicates that 0xFFFF should be used as the cut value.</summary>
	_0xFFFF = 1,
	/// <summary>Indicates that 0xFFFFFFFF should be used as the cut value.</summary>
	_0xFFFFFFFF = 2,
} ;

[EquivalentOf( typeof( D3D12_DEPTH_WRITE_MASK ) )]
public enum DepthWriteMask {
	/// <summary>Turn off writes to the depth-stencil buffer.</summary>
	ZERO = 0,
	/// <summary>Turn on writes to the depth-stencil buffer.</summary>
	ALL = 1,
} ;

[EquivalentOf( typeof( D3D12_STENCIL_OP ) )]
public enum StencilOperation {
	/// <summary>Keep the existing stencil data.</summary>
	KEEP = 1,
	/// <summary>Set the stencil data to 0.</summary>
	ZERO = 2,
	/// <summary>Set the stencil data to the reference value set by calling <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetstencilref">ID3D12GraphicsCommandList::OMSetStencilRef</a>.</summary>
	REPLACE = 3,
	/// <summary>Increment the stencil value by 1, and clamp the result.</summary>
	INCR_SAT = 4,
	/// <summary>Decrement the stencil value by 1, and clamp the result.</summary>
	SAT = 5,
	/// <summary>Invert the stencil data.</summary>
	INVERT = 6,
	/// <summary>Increment the stencil value by 1, and wrap the result if necessary.</summary>
	INCR = 7,
	/// <summary>Decrement the stencil value by 1, and wrap the result if necessary.</summary>
	DECR = 8,
} ;

[EquivalentOf( typeof( D3D12_BLEND ) )]
public enum Blend {
	/// <summary>The blend factor is (0, 0, 0, 0). No pre-blend operation.</summary>
	ZERO = 1,

	/// <summary>The blend factor is (1, 1, 1, 1). No pre-blend operation.</summary>
	ONE = 2,

	/// <summary>The blend factor is (Rₛ, Gₛ, Bₛ, Aₛ), that is color data (RGB) from a pixel shader. No pre-blend operation.</summary>
	SRC_COLOR = 3,

	/// <summary>The blend factor is (1 - Rₛ, 1 - Gₛ, 1 - Bₛ, 1 - Aₛ), that is color data (RGB) from a pixel shader. The pre-blend operation inverts the data, generating 1 - RGB.</summary>
	INV_SRC_COLOR = 4,

	/// <summary>The blend factor is (Aₛ, Aₛ, Aₛ, Aₛ), that is alpha data (A) from a pixel shader. No pre-blend operation.</summary>
	SRC_ALPHA = 5,

	/// <summary>The blend factor is ( 1 - Aₛ, 1 - Aₛ, 1 - Aₛ, 1 - Aₛ), that is alpha data (A) from a pixel shader. The pre-blend operation inverts the data, generating 1 - A.</summary>
	INV_SRC_ALPHA = 6,

	/// <summary>The blend factor is (A<sub>d</sub> A<sub>d</sub> A<sub>d</sub> A<sub>d</sub>), that is alpha data from a render target. No pre-blend operation.</summary>
	DEST_ALPHA = 7,

	/// <summary>The blend factor is (1 - A<sub>d</sub> 1 - A<sub>d</sub> 1 - A<sub>d</sub> 1 - A<sub>d</sub>), that is alpha data from a render target. The pre-blend operation inverts the data, generating 1 - A.</summary>
	INV_DEST_ALPHA = 8,

	/// <summary>The blend factor is (R<sub>d</sub>, G<sub>d</sub>, B<sub>d</sub>, A<sub>d</sub>), that is color data from a render target. No pre-blend operation.</summary>
	DEST_COLOR = 9,

	/// <summary>The blend factor is (1 - R<sub>d</sub>, 1 - G<sub>d</sub>, 1 - B<sub>d</sub>, 1 - A<sub>d</sub>), that is color data from a render target. The pre-blend operation inverts the data, generating 1 - RGB.</summary>
	INV_DEST_COLOR = 10,

	/// <summary>
	/// <para>The blend factor is (f, f, f, 1); where f = min(Aₛ, 1 - A<sub>d</sub>). The pre-blend operation clamps the data to 1 or less.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_blend#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	SRC_ALPHA_SAT = 11,

	/// <summary>The blend factor is the blend factor set with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor">ID3D12GraphicsCommandList::OMSetBlendFactor</a>. No pre-blend operation.</summary>
	BLEND_FACTOR = 14,

	/// <summary>The blend factor is the blend factor set with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor">ID3D12GraphicsCommandList::OMSetBlendFactor</a>. The pre-blend operation inverts the blend factor, generating 1 - blend_factor.</summary>
	INV_BLEND_FACTOR = 15,

	/// <summary>The blend factor is data sources both as color data output by a pixel shader. There is no pre-blend operation. This blend factor supports dual-source color blending.</summary>
	SRC1_COLOR = 16,

	/// <summary>The blend factor is data sources both as color data output by a pixel shader. The pre-blend operation inverts the data, generating 1 - RGB. This blend factor supports dual-source color blending.</summary>
	INV_SRC1_COLOR = 17,

	/// <summary>The blend factor is data sources as alpha data output by a pixel shader. There is no pre-blend operation. This blend factor supports dual-source color blending.</summary>
	SRC1_ALPHA = 18,

	/// <summary>The blend factor is data sources as alpha data output by a pixel shader. The pre-blend operation inverts the data, generating 1 - A. This blend factor supports dual-source color blending.</summary>
	INV_SRC1_ALPHA = 19,

	/// <summary>
	/// <para>The blend factor is (A, A, A, A), where the constant, A, is taken from the blend factor set with [OMSetBlendFactor](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor). To successfully use this constant on a target machine, the [D3D12_FEATURE_DATA_D3D12_OPTIONS13](ns-d3d12-d3d12_feature_data_d3d12_options13.md) returned from [capability querying](/windows/win32/direct3d12/capability-querying) must have its *AlphaBlendFactorSupported* set to `TRUE`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_blend#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	ALPHA_FACTOR = 20,

	/// <summary>
	/// <para>The blend factor is (1 – A, 1 – A, 1 – A, 1 – A), where the constant, A, is taken from the blend factor set with [OMSetBlendFactor](/windows/win32/api/d3d12/nf-d3d12-id3d12graphicscommandlist-omsetblendfactor). To successfully use this constant on a target machine, the [D3D12_FEATURE_DATA_D3D12_OPTIONS13](ns-d3d12-d3d12_feature_data_d3d12_options13.md) returned from [capability querying](/windows/win32/direct3d12/capability-querying) must have its *AlphaBlendFactorSupported* set to `TRUE`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_blend#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	INV_ALPHA_FACTOR = 21,
} ;
 
[EquivalentOf( typeof( D3D12_BLEND_OP ) )]
public enum BlendOperation {
	/// <summary>Add source 1 and source 2.</summary>
	ADD = 1,
	/// <summary>Subtract source 1 from source 2.</summary>
	SUBTRACT = 2,
	/// <summary>Subtract source 2 from source 1.</summary>
	REV_SUBTRACT = 3,
	/// <summary>Find the minimum of source 1 and source 2.</summary>
	MIN = 4,
	/// <summary>Find the maximum of source 1 and source 2.</summary>
	MAX = 5,
} ;

[EquivalentOf( typeof( D3D12_LOGIC_OP ) )]
public enum LogicOperation {
	/// <summary>Set the logical operation to CLEAR.</summary>
	CLEAR = 0,
	/// <summary>Set the logical operation to SET.</summary>
	SET = 1,
	/// <summary>Set the logical operation to COPY.</summary>
	COPY = 2,
	/// <summary>Set the logical operation to COPY_INVERTED.</summary>
	COPY_INVERTED = 3,
	/// <summary>Set the logical operation to NOOP.</summary>
	NOOP = 4,
	/// <summary>Set the logical operation to INVERT.</summary>
	INVERT = 5,
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
	EQUIV = 11,
	/// <summary>Set the logical operation to AND_REVERSE.</summary>
	AND_REVERSE = 12,
	/// <summary>Set the logical operation to AND_INVERTED.</summary>
	AND_INVERTED = 13,
	/// <summary>Set the logical operation to OR_REVERSE.</summary>
	OR_REVERSE = 14,
	/// <summary>Set the logical operation to OR_INVERTED.</summary>
	OR_INVERTED = 15,
} ;

[EquivalentOf( typeof( D3D12_FILL_MODE ) )]
public enum FillMode {
	/// <summary>Draw lines connecting the vertices. Adjacent vertices are not drawn.</summary>
	WIREFRAME = 2,
	/// <summary>Fill the triangles formed by the vertices. Adjacent vertices are not drawn.</summary>
	SOLID = 3,
} ;

[EquivalentOf( typeof( D3D12_CULL_MODE ) )]
public enum CullMode {
	/// <summary>Always draw all triangles.</summary>
	NONE = 1,
	/// <summary>Do not draw triangles that are front-facing.</summary>
	FRONT = 2,
	/// <summary>Do not draw triangles that are back-facing.</summary>
	BACK = 3,
} ;

[EquivalentOf( typeof( D3D12_CONSERVATIVE_RASTERIZATION_MODE ) )]
public enum ConservativeRasterizationMode {
	/// <summary>Conservative rasterization is off.</summary>
	OFF = 0,
	/// <summary>Conservative rasterization is on.</summary>
	ON = 1,
} ;


[EquivalentOf( typeof( D3D12_INPUT_CLASSIFICATION ) )]
public enum InputClassification {
	/// <summary>Input data is per-vertex data.</summary>
	PerVertexData = 0,

	/// <summary>Input data is per-instance data.</summary>
	PerInstanceData = 1,
} ;

[EquivalentOf( typeof(D3D12_CPU_PAGE_PROPERTY) )]
public enum CPUPageProperty {
	/// <summary>The CPU-page property is unknown.</summary>
	UNKNOWN = 0,
	/// <summary>The CPU cannot access the heap, therefore no page properties are available.</summary>
	NOT_AVAILABLE = 1,
	/// <summary>The CPU-page property is write-combined.</summary>
	WRITE_COMBINE = 2,
	/// <summary>The CPU-page property is write-back.</summary>
	WRITE_BACK = 3,
} ;

[EquivalentOf( typeof(D3D12_MEMORY_POOL) )]
public enum MemoryPool {
	/// <summary>The memory pool is unknown.</summary>
	UNKNOWN = 0,
	/// <summary>
	/// <para>The memory pool is L0. L0 is the physical system memory pool. When the adapter is discrete/NUMA, this pool has greater bandwidth for the CPU and less bandwidth for the GPU. When the adapter is UMA, this pool is the only one which is valid.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_memory_pool#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	L0 = 1,
	/// <summary>
	/// <para>The memory pool is L1. L1 is typically known as the physical video memory pool. L1 is only available when the adapter is discrete/NUMA, and has greater bandwidth for the GPU and cannot even be accessed by the CPU. When the adapter is UMA, this pool is not available.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_memory_pool#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	L1 = 2,
} ;

[EquivalentOf( typeof(D3D12_INDIRECT_ARGUMENT_TYPE) )]
public enum IndirectArgumentType {
	/// <summary>Indicates the type is a Draw call.</summary>
	DRAW = 0,
	/// <summary>Indicates the type is a DrawIndexed call.</summary>
	DRAW_INDEXED = 1,
	/// <summary>Indicates the type is a Dispatch call.</summary>
	DISPATCH = 2,
	/// <summary>Indicates the type is a vertex buffer view.</summary>
	VERTEX_BUFFER_VIEW = 3,
	/// <summary>Indicates the type is an index buffer view.</summary>
	INDEX_BUFFER_VIEW = 4,
	/// <summary>Indicates the type is a constant.</summary>
	CONSTANT = 5,
	/// <summary>Indicates the type is a constant buffer view (CBV).</summary>
	CONSTANT_BUFFER_VIEW = 6,
	/// <summary>Indicates the type is a shader resource view (SRV).</summary>
	SHADER_RESOURCE_VIEW = 7,
	/// <summary>Indicates the type is an unordered access view (UAV).</summary>
	UNORDERED_ACCESS_VIEW = 8,
	DISPATCH_RAYS = 9,
	DISPATCH_MESH = 10,
} ;


//!!! ------------------------------------------------------------------------------------------------ !!!//

#warning This type doesn't seem to get created by CsWin32
/// <summary>Identifies the tier level at which tiled resources are supported.</summary>
/// <remarks>For more information, see:
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/ne-d3d12-d3d12_tiled_resources_tier">
/// <b>D3D12_TILED_RESOURCES_TIER</b> enumeration (<i>d3d12.h</i>)</a>.</remarks>
[ProxyFor(typeof(D3D12_INDIRECT_ARGUMENT_TYPE))]
public enum TiledResourcesTier { NotSupported = 0, Tier1 = 1, Tier2 = 2, Tier3 = 3, Tier4 = 4 } ;

//!!! ------------------------------------------------------------------------------------------------ !!!//

[EquivalentOf( typeof( D3D12_TILE_COPY_FLAGS ) )]
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