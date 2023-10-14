#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Direct3D12 ;
#endregion
namespace DXSharp.DXGI ;



[ProxyFor(typeof(DXGI_GRAPHICS_PREEMPTION_GRANULARITY))]
public enum GraphicsPreemptionGranularity {
	DMA_BUFFER_BOUNDARY  = 0,
	PRIMITIVE_BOUNDARY   = 1,
	TRIANGLE_BOUNDARY    = 2,
	PIXEL_BOUNDARY       = 3,
	INSTRUCTION_BOUNDARY = 4,
} ;

[ProxyFor(typeof(DXGI_COMPUTE_PREEMPTION_GRANULARITY))]
public enum ComputePreemptionGranularity {
	DMA_BUFFER_BOUNDARY   = 0,
	DISPATCH_BOUNDARY     = 1,
	THREAD_GROUP_BOUNDARY = 2,
	THREAD_BOUNDARY       = 3,
	INSTRUCTION_BOUNDARY  = 4,
} ;



/// <summary>Defines constants that specify a Direct3D 12 feature or feature set to query about.</summary>
/// <remarks>
/// Use a constant from this enumeration in a call to the <see cref="IDevice"/> interface's <b>CheckFeatureSupport</b> method 
/// to query a driver about support for various Direct3D 12 features. Each value in this enumeration has a corresponding data structure
/// that you must pass (by pointer reference) in the <i>pFeatureSupportData</i> parameter of <b>ID3D12Device::CheckFeatureSupport</b>.
/// </remarks>
[ProxyFor(typeof(D3D12_FEATURE))]
public enum D3D12Feature {
	/// <summary>Indicates a query for the level of support for basic Direct3D 12 feature options. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS = 0,
	/// <summary>
	/// <para>Indicates a query for the adapter's architectural details, so that your application can better optimize for certain adapter properties. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture">D3D12_FEATURE_DATA_ARCHITECTURE</a>. <div class="alert"><b>Note</b>  This value has been superseded by the <b>D3D_FEATURE_DATA_ARCHITECTURE1</b> value. If your application targets Windows 10, version 1703 (Creators' Update) or higher, then use the <b>D3D_FEATURE_DATA_ARCHITECTURE1</b> value instead.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_feature#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	D3D12_FEATURE_ARCHITECTURE = 1,
	/// <summary>Indicates a query for info about the <a href="https://docs.microsoft.com/windows/win32/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature levels</a> supported. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_feature_levels">D3D12_FEATURE_DATA_FEATURE_LEVELS</a>.</summary>
	D3D12_FEATURE_FEATURE_LEVELS = 2,
	/// <summary>Indicates a query for the resources supported by the current graphics driver for a given format. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_format_support">D3D12_FEATURE_DATA_FORMAT_SUPPORT</a>.</summary>
	D3D12_FEATURE_FORMAT_SUPPORT = 3,
	/// <summary>Indicates a query for the image quality levels for a given format and sample count. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_multisample_quality_levels">D3D12_FEATURE_DATA_MULTISAMPLE_QUALITY_LEVELS</a>.</summary>
	D3D12_FEATURE_MULTISAMPLE_QUALITY_LEVELS = 4,
	/// <summary>Indicates a query for the DXGI data format. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_format_info">D3D12_FEATURE_DATA_FORMAT_INFO</a>.</summary>
	D3D12_FEATURE_FORMAT_INFO = 5,
	/// <summary>Indicates a query for the GPU's virtual address space limitations. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_gpu_virtual_address_support">D3D12_FEATURE_DATA_GPU_VIRTUAL_ADDRESS_SUPPORT</a>.</summary>
	D3D12_FEATURE_GPU_VIRTUAL_ADDRESS_SUPPORT = 6,
	/// <summary>Indicates a query for the supported shader model. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_shader_model">D3D12_FEATURE_DATA_SHADER_MODEL</a>.</summary>
	D3D12_FEATURE_SHADER_MODEL = 7,
	/// <summary>Indicates a query for the level of support for HLSL 6.0 wave operations. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options1">D3D12_FEATURE_DATA_D3D12_OPTIONS1</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS1 = 8,
	/// <summary>Indicates a query for the level of support for protected resource sessions. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_support">D3D12_FEATURE_DATA_PROTECTED_RESOURCE_SESSION_SUPPORT</a>.</summary>
	D3D12_FEATURE_PROTECTED_RESOURCE_SESSION_SUPPORT = 10,
	/// <summary>Indicates a query for root signature version support. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_root_signature">D3D12_FEATURE_DATA_ROOT_SIGNATURE</a>.</summary>
	D3D12_FEATURE_ROOT_SIGNATURE = 12,
	/// <summary>
	/// <para>Indicates a query for each adapter's architectural details, so that your application can better optimize for certain adapter properties. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1">D3D12_FEATURE_DATA_ARCHITECTURE1</a>. <div class="alert"><b>Note</b>  This value supersedes the <b>D3D_FEATURE_DATA_ARCHITECTURE</b> value. If your application targets Windows 10, version 1703 (Creators' Update) or higher, then use <b>D3D_FEATURE_DATA_ARCHITECTURE1</b>.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_feature#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	D3D12_FEATURE_ARCHITECTURE1 = 16,
	/// <summary>Indicates a query for the level of support for depth-bounds tests and programmable sample positions. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options2">D3D12_FEATURE_DATA_D3D12_OPTIONS2</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS2 = 18,
	/// <summary>Indicates a query for the level of support for shader caching. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_shader_cache">D3D12_FEATURE_DATA_SHADER_CACHE</a>.</summary>
	D3D12_FEATURE_SHADER_CACHE = 19,
	/// <summary>Indicates a query for the adapter's support for prioritization of different command queue types. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_command_queue_priority">D3D12_FEATURE_DATA_COMMAND_QUEUE_PRIORITY</a>.</summary>
	D3D12_FEATURE_COMMAND_QUEUE_PRIORITY = 20,
	/// <summary>Indicates a query for the level of support for timestamp queries, format-casting, immediate write, view instancing, and barycentrics. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options3">D3D12_FEATURE_DATA_D3D12_OPTIONS3</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS3 = 21,
	/// <summary>Indicates a query for whether or not the adapter supports creating heaps from existing system memory. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_existing_heaps">D3D12_FEATURE_DATA_EXISTING_HEAPS</a>.</summary>
	D3D12_FEATURE_EXISTING_HEAPS = 22,
	/// <summary>Indicates a query for the level of support for 64KB-aligned MSAA textures, cross-API sharing, and native 16-bit shader operations. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options4">D3D12_FEATURE_DATA_D3D12_OPTIONS4</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS4 = 23,
	/// <summary>Indicates a query for the level of support for heap serialization. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_serialization">D3D12_FEATURE_DATA_SERIALIZATION</a>.</summary>
	D3D12_FEATURE_SERIALIZATION = 24,
	/// <summary>Indicates a query for the level of support for the sharing of resources between different adapters&mdash;for example, multiple GPUs. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_cross_node">D3D12_FEATURE_DATA_CROSS_NODE</a>.</summary>
	D3D12_FEATURE_CROSS_NODE = 25,
	/// <summary>Starting with Windows 10, version 1809 (10.0; Build 17763), indicates a query for the level of support for render passes, ray tracing, and shader-resource view tier 3 tiled resources. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options5">D3D12_FEATURE_DATA_D3D12_OPTIONS5</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS5 = 27,
	/// <summary>Starting with Windows 11 (Build 10.0.22000.194). The corresponding data structure for this value is [D3D12_FEATURE_DATA_DISPLAYABLE](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_displayable).</summary>
	D3D12_FEATURE_DISPLAYABLE = 28,
	/// <summary>
	/// <para>Starting with Windows 10, version 1903 (10.0; Build 18362), indicates a query for the level of support for variable-rate shading (VRS), and indicates whether or not background processing is supported. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS6](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options6). For more info, see <a href="https://docs.microsoft.com/windows/win32/direct3d12/vrs">Variable-rate shading (VRS)</a>, and the <a href="https://microsoft.github.io/DirectX-Specs/d3d/BackgroundProcessing.html">Direct3D 12 background processing spec</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_feature#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	D3D12_FEATURE_D3D12_OPTIONS6 = 30,
	/// <summary>Indicates a query for the level of support for metacommands. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_query_meta_command">D3D12_FEATURE_DATA_QUERY_META_COMMAND</a>.</summary>
	D3D12_FEATURE_QUERY_META_COMMAND = 31,
	/// <summary>
	/// <para>Starting with Windows 10, version 2004 (10.0; Build 19041), indicates a query for the level of support for mesh and amplification shaders, and for sampler feedback. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS7](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options7). For more info, see the [Mesh shader](https://microsoft.github.io/DirectX-Specs/d3d/MeshShader.html) and [Sampler feedback](https://microsoft.github.io/DirectX-Specs/d3d/SamplerFeedback.html) specs.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_feature#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	D3D12_FEATURE_D3D12_OPTIONS7 = 32,
	/// <summary>Starting with Windows 10, version 2004 (10.0; Build 19041), indicates a query to retrieve the count of protected resource session types. The corresponding data structure for this value is [D3D12_FEATURE_DATA_PROTECTED_RESOURCE_SESSION_TYPE_COUNT](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_type_count).</summary>
	D3D12_FEATURE_PROTECTED_RESOURCE_SESSION_TYPE_COUNT = 33,
	/// <summary>Starting with Windows 10, version 2004 (10.0; Build 19041), indicates a query to retrieve the list of protected resource session types. The corresponding data structure for this value is [D3D12_FEATURE_DATA_PROTECTED_RESOURCE_SESSION_TYPES](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_types).</summary>
	D3D12_FEATURE_PROTECTED_RESOURCE_SESSION_TYPES = 34,
	/// <summary>Starting with Windows 11 (Build 10.0.22000.194), indicates whether or not unaligned block-compressed textures are supported. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS8](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options8).</summary>
	D3D12_FEATURE_D3D12_OPTIONS8 = 36,
	/// <summary>Starting with Windows 11 (Build 10.0.22000.194), indicates whether or not support exists for mesh shaders, values of *SV_RenderTargetArrayIndex* that are 8 or greater, typed resource 64-bit integer atomics, derivative and derivative-dependent texture sample operations, and the level of support for WaveMMA (wave_matrix) operations. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS9](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options9).</summary>
	D3D12_FEATURE_D3D12_OPTIONS9 = 37,
	/// <summary>Starting with Windows 11 (Build 10.0.22000.194), indicates whether or not the SUM combiner can be used, and whether or not *SV_ShadingRate* can be set from a mesh shader. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS10](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options10).</summary>
	D3D12_FEATURE_D3D12_OPTIONS10 = 39,
	/// <summary>Starting with Windows 11 (Build 10.0.22000.194), indicates whether or not 64-bit integer atomics on resources in descriptor heaps are supported. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS11](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options11).</summary>
	D3D12_FEATURE_D3D12_OPTIONS11 = 40,
	D3D12_FEATURE_D3D12_OPTIONS12 = 41,
	D3D12_FEATURE_D3D12_OPTIONS13 = 42,
	D3D12_FEATURE_D3D12_OPTIONS14 = 43,
	D3D12_FEATURE_D3D12_OPTIONS15 = 44,
	D3D12_FEATURE_D3D12_OPTIONS16 = 45,
	D3D12_FEATURE_D3D12_OPTIONS17 = 46,
	D3D12_FEATURE_D3D12_OPTIONS18 = 47,
	D3D12_FEATURE_D3D12_OPTIONS19 = 48,
} ;