#region Using Directives
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp.Direct3D12 ;


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
[EquivalentOf( typeof(D3D12_TILED_RESOURCES_TIER) )]
public enum TiledResourcesTier { NotSupported = 0, Tier1 = 1, Tier2 = 2, Tier3 = 3, Tier4 = 4 } ;

//!!! ------------------------------------------------------------------------------------------------ !!!//

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


[Flags, EquivalentOf(typeof(D3D12_ROOT_SIGNATURE_FLAGS))]
public enum RootSignatureFlags {
	/// <summary>Indicates default behavior.</summary>
	None = 0x00000000,
	/// <summary>The app is opting in to using the Input Assembler (requiring an input layout that defines a set of vertex buffer bindings). Omitting this flag can result in one root argument space being saved on some hardware. Omit this flag if the Input Assembler is not required, though the optimization is minor.</summary>
	AllowInputAssemblerInputLayout = 0x00000001,
	/// <summary>Denies the vertex shader access to the root signature.</summary>
	DenyVertexShaderRootAccess = 0x00000002,
	/// <summary>Denies the hull shader access to the root signature.</summary>
	DenyHullShaderRootAccess = 0x00000004,
	/// <summary>Denies the domain shader access to the root signature.</summary>
	DenyDomainShaderRootAccess = 0x00000008,
	/// <summary>Denies the geometry shader access to the root signature.</summary>
	DenyGeometryShaderRootAccess = 0x00000010,
	/// <summary>Denies the pixel shader access to the root signature.</summary>
	DenyPixelShaderRootAccess = 0x00000020,
	/// <summary>The app is opting in to using Stream Output. Omitting this flag can result in one root argument space being saved on some hardware. Omit this flag if Stream Output is not required, though the optimization is minor.</summary>
	AllowStreamOutput = 0x00000040,
	/// <summary>The root signature is to be used with raytracing shaders to define resource bindings sourced from shader records in shader tables.  This flag cannot be combined with any other root signature flags, which are all related to the graphics pipeline.  The absence of the flag means the root signature can be used with graphics or compute, where the compute version is also shared with raytracing’s global root signature.</summary>
	LocalRootSignature = 0x00000080,
	/// <summary>Denies the amplification shader access to the root signature.</summary>
	DenyAmplificationShaderRootAccess = 0x00000100,
	/// <summary>Denies the mesh shader access to the root signature.</summary>
	DenyMeshShaderRootAccess = 0x00000200,
	/// <summary>The shaders are allowed to index the CBV/SRV/UAV descriptor heap directly, using the *ResourceDescriptorHeap* built-in variable.</summary>
	CbvSrvUavHeapDirectlyIndexed = 0x00000400,
	/// <summary>The shaders are allowed to index the sampler descriptor heap directly, using the *SamplerDescriptorHeap* built-in variable.</summary>
	SamplerHeapDirectlyIndexed = 0x00000800,
} ;


[EquivalentOf(typeof(D3D12_SHADER_VISIBILITY))]
public enum ShaderVisibility {
	/// <summary>Specifies that all shader stages can access whatever is bound at the root signature slot.</summary>
	All = 0,
	/// <summary>Specifies that the vertex shader stage can access whatever is bound at the root signature slot.</summary>
	Vertex = 1,
	/// <summary>Specifies that the hull shader stage can access whatever is bound at the root signature slot.</summary>
	Hull = 2,
	/// <summary>Specifies that the domain shader stage can access whatever is bound at the root signature slot.</summary>
	Domain = 3,
	/// <summary>Specifies that the geometry shader stage can access whatever is bound at the root signature slot.</summary>
	Geometry = 4,
	/// <summary>Specifies that the pixel shader stage can access whatever is bound at the root signature slot.</summary>
	Pixel = 5,
	/// <summary>Specifies that the amplification shader stage can access whatever is bound at the root signature slot.</summary>
	Amplification = 6,
	/// <summary>Specifies that the mesh shader stage can access whatever is bound at the root signature slot.</summary>
	Mesh = 7,
} ;


[Flags, EquivalentOf(typeof(D3D12_STATIC_BORDER_COLOR))]
public enum StaticBorderColor {
	/// <summary>Indicates black, with the alpha component as fully transparent.</summary>
	TransparentBlack = 0,
	/// <summary>Indicates black, with the alpha component as fully opaque.</summary>
	OpaqueBlack = 1,
	/// <summary>Indicates white, with the alpha component as fully opaque.</summary>
	OpaqueWhite = 2,
	OpaqueBlackUint = 3,
	OpaqueWhiteUint = 4,
} ;


[Flags, EquivalentOf(typeof(D3D12_ROOT_PARAMETER_TYPE))]
public enum RootParameterType {
	/// <summary>The slot is for a descriptor table.</summary>
	DescriptorTable = 0,
	/// <summary>The slot is for root constants.</summary>
	Const32Bits = 1,
	/// <summary>The slot is for a constant-buffer view (CBV).</summary>
	CBV = 2,
	/// <summary>The slot is for a shader-resource view (SRV).</summary>
	SRV = 3,
	/// <summary>The slot is for a unordered-access view (UAV).</summary>
	UAV = 4,
} ;


[EquivalentOf(typeof(D3D12_DESCRIPTOR_RANGE_TYPE))]
public enum DescriptorRangeType {
	/// <summary>Specifies a range of SRVs.</summary>
	SRV = 0,
	/// <summary>Specifies a range of unordered-access views (UAVs).</summary>
	UAV = 1,
	/// <summary>Specifies a range of constant-buffer views (CBVs).</summary>
	CBV = 2,
	/// <summary>Specifies a range of samplers.</summary>
	Sampler = 3,
} ;


[EquivalentOf(typeof(D3D_ROOT_SIGNATURE_VERSION))]
public enum RootSignatureVersion {
	/// <summary>Version one of root signature layout.</summary>
	Version1 = 1,
	/// <summary>Version one of root signature layout.</summary>
	Version1_0 = 1,
	/// <summary>Version 1.1  of root signature layout. Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/root-signature-version-1-1">Root Signature Version 1.1</a>.</summary>
	Version1_1 = 2,
	Version1_2 = 3,
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
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	RootSignature = 0,

	/// <summary>
	/// <para>Indicates a vertex shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	VS = 1,

	/// <summary>
	/// <para>Indicates a pixel shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	PS = 2,

	/// <summary>
	/// <para>Indicates a domain shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	DS = 3,

	/// <summary>
	/// <para>Indicates a hull shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	HS = 4,

	/// <summary>
	/// <para>Indicates a geometry shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	GS = 5,

	/// <summary>
	/// <para>Indicates a compute shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	CS = 6,

	/// <summary>
	/// <para>Indicates a stream-output subobject type. The corresponding subobject type is **[D3D12_STREAM_OUTPUT_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_stream_output_desc)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	StreamOutput = 7,

	/// <summary>
	/// <para>Indicates a blend subobject type. The corresponding subobject type is **[D3D12_BLEND_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_blend_desc)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Blend = 8,

	/// <summary>
	/// <para>Indicates a sample mask subobject type. The corresponding subobject type is **UINT**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	SampleMask = 9,

	/// <summary>
	/// <para>Indicates indicates a rasterizer subobject type. The corresponding subobject type is **[D3D12_RASTERIZER_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_rasterizer_desc)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Rasterizer = 10,

	/// <summary>
	/// <para>Indicates a depth stencil subobject type. The corresponding subobject type is **[D3D12_DEPTH_STENCIL_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_desc)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	DepthStencil = 11,

	/// <summary>
	/// <para>Indicates an input layout subobject type. The corresponding subobject type is **[D3D12_INPUT_LAYOUT_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_input_layout_desc)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	InputLayout = 12,

	/// <summary>
	/// <para>Indicates an index buffer strip cut value subobject type. The corresponding subobject type is **[D3D12_INDEX_BUFFER_STRIP_CUT_VALUE](/windows/win32/api/d3d12/ne-d3d12-d3d12_index_buffer_strip_cut_value)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	IBStripCutValue = 13,

	/// <summary>
	/// <para>Indicates a primitive topology subobject type. The corresponding subobject type is **[D3D12_PRIMITIVE_TOPOLOGY_TYPE](/windows/win32/api/d3d12/ne-d3d12-d3d12_primitive_topology_type)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	PrimitiveTopology = 14,

	/// <summary>Indicates a render target formats subobject type. The corresponding subobject type is **[D3D12_RT_FORMAT_ARRAY](/windows/win32/api/d3d12/ns-d3d12-d3d12_rt_format_array)** structure, which wraps an array of render target formats along with a count of the array elements.</summary>
	RenderTargetFormats = 15,

	/// <summary>
	/// <para>Indicates a depth stencil format subobject. The corresponding subobject type is **[DXGI_FORMAT](/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	DepthStencilFormat = 16,

	/// <summary>
	/// <para>Indicates a sample description subobject type. The corresponding subobject type is **[DXGI_SAMPLE_DESC](/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	SampleDesc = 17,

	/// <summary>
	/// <para>Indicates a node mask subobject type. The corresponding subobject type is **[D3D12_NODE_MASK](/windows/win32/api/d3d12/ns-d3d12-d3d12_node_mask)** or **UINT**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	NodeMask = 18,

	/// <summary>
	/// <para>Indicates a cached pipeline state object subobject type. The corresponding subobject type is **[D3D12_CACHED_PIPELINE_STATE](/windows/win32/api/d3d12/ns-d3d12-d3d12_cached_pipeline_state)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	CachedPSO = 19,

	/// <summary>
	/// <para>Indicates a flags subobject type. The corresponding subobject type is **[D3D12_PIPELINE_STATE_FLAGS](/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_flags)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Flags = 20,

	/// <summary>
	/// <para>Indicates an expanded depth stencil subobject type. This expansion of the depth stencil subobject supports optional depth bounds checking. The corresponding subobject type is **[D3D12_DEPTH_STENCIL_DESC1](/windows/win32/api/d3d12/ns-d3d12-d3d12_depth_stencil_desc1)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	DepthStencil1 = 21,

	/// <summary>
	/// <para>Indicates a view instancing subobject type. The corresponding subobject type is **[D3D12_VIEW_INSTANCING_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_view_instancing_desc)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	ViewInstancing = 22,

	/// <summary>
	/// <para>Indicates an amplification shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	AS = 24,

	/// <summary>
	/// <para>Indicates a mesh shader subobject type. The corresponding subobject type is **[D3D12_SHADER_BYTECODE](/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_bytecode)**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_pipeline_state_subobject_type#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	MS = 25,
	DepthStencil2 = 26,
	Rasterizer1   = 27,
	Rasterizer2   = 28,

	/// <summary>A sentinel value that marks the exclusive upper-bound of valid values this enumeration represents.</summary>
	MaxValid = 29,
} ;
