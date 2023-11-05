#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.DXGI ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>
/// Provides detail about the adapter architecture, so that your application can better optimize for certain adapter properties.
/// </summary>
/// <remarks>
/// <b>NOTE:</b><para/>
/// This structure has been superseded by the <see cref="FeatureDataArchitecture1"/> structure.
/// If your application targets Windows 10, version 1703 (Creators' Update) or higher,
/// then use D3D12_FEATURE_DATA_ARCHITECTURE1 (and D3D12_FEATURE_ARCHITECTURE1) instead.
/// </remarks>
[StructLayout(LayoutKind.Sequential),
 EquivalentOf(typeof(D3D12_FEATURE_DATA_ARCHITECTURE))]
public partial struct FeatureDataArchitecture {
	/// <summary>
	/// <para>In multi-adapter operation, this indicates which physical adapter of the device is relevant. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>. <b>NodeIndex</b> is filled out by the application before calling <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a>, as the application can retrieve details about the architecture of each adapter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NodeIndex ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support a tile-based renderer. The runtime sets this member to <b>TRUE</b> if the hardware and driver support a tile-based renderer.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL TileBasedRenderer ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support UMA. The runtime sets this member to <b>TRUE</b> if the hardware and driver support UMA.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL UMA ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support cache-coherent UMA. The runtime sets this member to <b>TRUE</b> if the hardware and driver support cache-coherent UMA.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL CacheCoherentUMA ;
	
	
	public FeatureDataArchitecture( uint nodeIndex = 0, bool tileBasedRenderer = false, 
									bool uma = false, bool cacheCoherentUMA = false ) {
		NodeIndex         = nodeIndex ;
		TileBasedRenderer = tileBasedRenderer ;
		UMA               = uma ;
		CacheCoherentUMA  = cacheCoherentUMA ;
	}
} ;


/// <summary>
/// See documentation for: 
/// <a href="https://docs.microsoft.com/en-us/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1">D3D12_FEATURE_DATA_ARCHITECTURE1</a>
/// </summary>
/// <remarks>
/// Update to D3D12 since Windows 10 Creators Update (1703).
/// Used by <see cref="ID3D12Device.CheckFeatureSupport"/>.
/// </remarks>
[StructLayout(LayoutKind.Sequential),
 EquivalentOf(typeof(D3D12_FEATURE_DATA_ARCHITECTURE1))]
public partial struct FeatureDataArchitecture1 {
	/// <summary>
	/// <para>In multi-adapter operation, this indicates which physical adapter of the device is relevant. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>. <b>NodeIndex</b> is filled out by the application before calling <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a>, as the application can retrieve details about the architecture of each adapter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NodeIndex ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support a tile-based renderer. The runtime sets this member to <b>TRUE</b> if the hardware and driver support a tile-based renderer.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL TileBasedRenderer ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support UMA. The runtime sets this member to <b>TRUE</b> if the hardware and driver support UMA.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL UMA ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support cache-coherent UMA. The runtime sets this member to <b>TRUE</b> if the hardware and driver support cache-coherent UMA.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL CacheCoherentUMA ;

	/// <summary>
	/// <para><a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-structs-and-classes">SAL</a>: <c>_Out_</c> Specifies whether the hardware and driver support isolated Memory Management Unit (MMU). The runtime sets this member to <b>TRUE</b> if the GPU honors CPU page table properties like <b>MEM_WRITE_WATCH</b> (for more information, see <a href="https://docs.microsoft.com/windows/win32/api/memoryapi/nf-memoryapi-virtualalloc">VirtualAlloc</a>) and <b>PAGE_READONLY</b> (for more information, see <a href="https://docs.microsoft.com/windows/win32/Memory/memory-protection-constants">Memory Protection Constants</a>). If <b>TRUE</b>, the application must take care to no use memory with these page table properties with the GPU, as the GPU might trigger these page table properties in unexpected ways. For example, GPU write operations might be coarser than the application expects, particularly writes from within shaders. Certain write-watch pages might appear dirty, even when it isn't obvious how GPU writes may have affected them. GPU operations associated with upload and readback heap usage scenarios work well with write-watch pages, but might occasionally generate false positives that can be safely ignored.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL IsolatedMMU ;
	
	
	public FeatureDataArchitecture1( uint nodeIndex = 0, bool tileBasedRenderer = false, 
									 bool uma = false, bool cacheCoherentUMA = false, bool isolatedMMU = false ) {
		NodeIndex         = nodeIndex ;
		TileBasedRenderer = tileBasedRenderer ;
		UMA               = uma ;
		CacheCoherentUMA  = cacheCoherentUMA ;
		IsolatedMMU       = isolatedMMU ;
	}
} ;



/// <summary>Describes info about the feature levels supported by the current graphics driver.</summary>
/// <remarks>See <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a>.</remarks>
[EquivalentOf(typeof(D3D12_FEATURE_DATA_FEATURE_LEVELS))]
public partial struct FeatureDataFeatureLevels {
	/// <summary>The number of <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature levels</a> in the array at <b>pFeatureLevelsRequested</b>.</summary>
	public uint NumFeatureLevels ;

	/// <summary>A pointer to an array of <a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/ne-d3dcommon-d3d_feature_level">D3D_FEATURE_LEVEL</a>s that the application is requesting for the driver and hardware to evaluate.</summary>
	public unsafe D3DFeatureLevel* pFeatureLevelsRequested ;

	/// <summary>The maximum <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature level</a> that the driver and hardware support.</summary>
	public D3DFeatureLevel MaxSupportedFeatureLevel ;
	
	
	public unsafe FeatureDataFeatureLevels( uint             numFeatureLevels,
											D3DFeatureLevel* pFeatureLevelsRequested, 
											D3DFeatureLevel  maxSupportedFeatureLevel ) {
		NumFeatureLevels        = numFeatureLevels ;
		this.pFeatureLevelsRequested = pFeatureLevelsRequested ;
		MaxSupportedFeatureLevel = maxSupportedFeatureLevel ;
	}
} ;


/// <summary>Describes which resources are supported by the current graphics driver for a given format. (D3D12_FEATURE_DATA_FORMAT_SUPPORT)</summary>
/// <remarks>
/// <para>Refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d11/typed-unordered-access-view-loads">Typed unordered access view loads</a>
/// for an example use of this structure. Also see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_format_support#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_FORMAT_SUPPORT ) )]
public partial struct FeatureDataFormatSupport {
	
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value for the format to return info about.</summary>
	public Format Format ;

	/// <summary>A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_format_support1">D3D12_FORMAT_SUPPORT1</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies which resources are supported.</summary>
	public FormatSupport1 Support1 ;

	/// <summary>A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_format_support2">D3D12_FORMAT_SUPPORT2</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies which unordered resource options are supported.</summary>
	public FormatSupport2 Support2 ;
	
	
	public FeatureDataFormatSupport( Format format, FormatSupport1 support1, FormatSupport2 support2 ) {
		Format   = format ;
		Support1 = support1 ;
		Support2 = support2 ;
	}
} ;


/// <summary>Describes the multi-sampling image quality levels for a given format and sample count.</summary>
/// <remarks>See <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a>.</remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_MULTISAMPLE_QUALITY_LEVELS ) )]
public partial struct FeatureDataMultisampleQualityLevels {
	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value
	/// for the format to return info about.
	/// </summary>
	public Format Format ;

	/// <summary>The number of multi-samples per pixel to return info about.</summary>
	public uint SampleCount ;

	/// <summary>
	/// <para>Flags to control quality levels, as a bitwise-OR'd combination of
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_multisample_quality_level_flags">D3D12_MULTISAMPLE_QUALITY_LEVEL_FLAGS</a>
	/// enumeration constants. The resulting value specifies options for determining quality levels.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_multisample_quality_levels#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public MultisampleQualityLevelFlags Flags ;

	/// <summary>The number of quality levels.</summary>
	public uint NumQualityLevels ;
	
	
	public FeatureDataMultisampleQualityLevels( Format format = Format.UNKNOWN, 
												uint sampleCount = 0U, 
												MultisampleQualityLevelFlags flags = MultisampleQualityLevelFlags.None, 
												uint numQualityLevels = 0U ) {
		Format           = format ;
		SampleCount      = sampleCount ;
		Flags            = flags ;
		NumQualityLevels = numQualityLevels ;
	}
} ;


/// <summary>Describes a DXGI data format and plane count.</summary>
/// <remarks>See <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a>.</remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_FORMAT_INFO ) )]
public partial struct FeatureDataFormatInfo {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value for the format to return info about.</summary>
	public Format Format ;

	/// <summary>The number of planes to provide information about.</summary>
	public byte PlaneCount ;
	
	
	public FeatureDataFormatInfo( Format format = Format.UNKNOWN, byte planeCount = 0 ) {
		Format     = format ;
		PlaneCount = planeCount ;
	}
} ;


/// <summary>Details the adapter's GPU virtual address space limitations, including maximum address bits per resource and per process.</summary>
/// <remarks>See the enumeration constant D3D12_FEATURE_GPU_VIRTUAL_ADDRESS_SUPPORT in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a> enumeration.</remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_GPU_VIRTUAL_ADDRESS_SUPPORT ) )]
public partial struct FeatureDataGPUVirtualAddressSupport {
	/// <summary>
	/// <para>The maximum GPU virtual address bits per resource. Some adapters have significantly less bits available per resource than per process, while other adapters have significantly greater bits available per resource than per process. The latter scenario tends to happen in less common scenarios, like when running a 32-bit process on certain UMA adapters. When per resource capabilities are greater than per process, the greater per resource capabilities can only be leveraged by reserved resources or NULL mapped pages.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_gpu_virtual_address_support#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint MaxGPUVirtualAddressBitsPerResource ;

	/// <summary>
	/// <para>The maximum GPU virtual address bits per process. When this value is nearly equal to the available residency budget, <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-evict">Evict</a> will not be a feasible option to manage residency. See <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-makeresident">MakeResident</a> for more details.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_gpu_virtual_address_support#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint MaxGPUVirtualAddressBitsPerProcess ;
	
	
	public FeatureDataGPUVirtualAddressSupport( uint maxGPUVirtualAddressBitsPerResource = 0, 
												uint maxGPUVirtualAddressBitsPerProcess = 0 ) {
		MaxGPUVirtualAddressBitsPerResource = maxGPUVirtualAddressBitsPerResource ;
		MaxGPUVirtualAddressBitsPerProcess  = maxGPUVirtualAddressBitsPerProcess ;
	}
} ;



/// <summary>Contains the supported shader model.</summary>
/// <remarks>
/// Refer to the enumeration constant <see cref="D3D12Feature.ShaderModel"/>.
/// When used with the <see cref="IDevice.CheckFeatureSupport"/> function, before calling the function initialize the *HighestShaderModel* field
/// to the highest shader model that your application understands. After the function completes successfully, the *HighestShaderModel* field
/// contains the highest shader model that is both supported by the device and no higher than the shader model passed in.<para/>
/// <para>
/// <b>[!NOTE]</b> <see cref="IDevice.CheckFeatureSupport"/> returns **E_INVALIDARG** if *HighestShaderModel* isn't known by the current runtime.
/// For that reason, we recommend that you call this in a loop with decreasing shader models to determine the highest supported shader model.
/// Alternatively, use the caps checking helper to simplify this; see the blog post:<para/>
/// <a href="https://devblogs.microsoft.com/directx/introducing-a-new-api-for-checking-feature-support-in-direct3d-12/">
/// Introducing a New API for Checking Feature Support in Direct3D 12.
/// </a>
/// </para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_shader_model#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_SHADER_MODEL ) )]
public partial struct FeatureDataShaderModel {
	/// <summary>Specifies one member of [D3D_SHADER_MODEL](/windows/win32/api/d3d12/ne-d3d12-d3d_shader_model) that indicates the maximum supported shader model.</summary>
	public ShaderModel HighestShaderModel ;
	
	public FeatureDataShaderModel( ShaderModel highestShaderModel = ShaderModel.Model6_5 ) {
		HighestShaderModel = highestShaderModel ;
	}
} ;


/// <summary>Indicates the level of support for protected resource sessions.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_PROTECTED_RESOURCE_SESSION_SUPPORT ) )]
public partial struct FeatureDataProtectedResourceSessionSupport {
	/// <summary>
	/// <para>An input field, indicating the adapter index to query.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_support#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint NodeIndex ;

	/// <summary>
	/// Indicates if there is protected resource session support for the node.
	/// </summary>
	public ProtectedResourceSessionSupportFlags Support ;
	
	
	public FeatureDataProtectedResourceSessionSupport( uint nodeIndex = 0, 
													   ProtectedResourceSessionSupportFlags support = 
														   ProtectedResourceSessionSupportFlags.None ) {
		NodeIndex = nodeIndex ;
		Support   = support ;
	}
} ;


/// <summary>Indicates root signature version support.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_root_signature">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_ROOT_SIGNATURE ) )]
public partial struct FeatureDataRootSignature {
	/// <summary>
	/// On input, specifies the highest version
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d_root_signature_version">D3D_ROOT_SIGNATURE_VERSION</a>
	/// to check for. On output specifies the highest version, up to the input version specified, actually available.
	/// </summary>
	public RootSignatureVersion HighestVersion ;
	
	public FeatureDataRootSignature( RootSignatureVersion highestVersion = RootSignatureVersion.Version1 ) {
		HighestVersion = highestVersion ;
	}
} ;


/// <summary>Describes the level of shader caching supported in the current graphics driver. (D3D12_FEATURE_DATA_SHADER_CACHE)</summary>
/// <remarks>
/// <para>Use this structure with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a> to determine the level of support offered for the optional shader-caching features. See the enumeration constant D3D12_FEATURE_SHADER_CACHE in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a> enumeration.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_shader_cache#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_SHADER_CACHE ) )]
public partial struct FeatureDataShaderCache {
	/// <summary>
	/// <para>Indicates the level of caching supported.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_shader_cache#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ShaderCacheSupportFlags SupportFlags ;
	
	public FeatureDataShaderCache( ShaderCacheSupportFlags supportFlags = ShaderCacheSupportFlags.None ) => 
		SupportFlags = supportFlags ;
} ;


/// <summary>Details the adapter's support for prioritization of different command queue types.</summary>
/// <remarks>
/// <para>Use this structure with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a>
/// to determine the priority levels supported by various command queue types. See the enumeration constant <b>D3D12_FEATURE_COMMAND_QUEUE_PRIORITY</b> in the
/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a> enumeration.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_command_queue_priority#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_COMMAND_QUEUE_PRIORITY ) )]
public partial struct FeatureDataCommandQueuePriority {
	/// <summary>
	/// <para><a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-structs-and-classes">SAL</a>: <c>_In_</c> The type of the command list you're interested in.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_command_queue_priority#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public CommandListType CommandListType ;

	/// <summary>
	/// <para><a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-structs-and-classes">SAL</a>: <c>_In_</c> The priority level you're interested in.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_command_queue_priority#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint Priority ;

	/// <summary>
	/// <para><a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-structs-and-classes">SAL</a>: <c>_Out_</c> On return, contains true if the specfied command list type supports the specified priority level; otherwise, false.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_command_queue_priority#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL PriorityForTypeIsSupported ;
	
	
	public FeatureDataCommandQueuePriority( CommandListType commandListType            = CommandListType.None, 
											uint            priority                   = 0U, 
											bool            priorityForTypeIsSupported = false ) {
		CommandListType           = commandListType ;
		Priority                  = priority ;
		PriorityForTypeIsSupported = priorityForTypeIsSupported ;
	}
} ;


/// <summary>Provides detail about whether the adapter supports creating heaps from existing system memory.</summary>
/// <remarks>For a variety of performance and compatibility reasons, applications should not make use of this feature except for diagnostic purposes. In particular, heaps created using this feature only support system-memory heaps with cross-adapter properties, which precludes many optimization opportunities that typical application scenarios could otherwise take advantage of.</remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_EXISTING_HEAPS ) )]
public partial struct FeatureDataExistingHeaps {
	/// <summary><b>TRUE</b> if the adapter can create a heap from existing system memory. Otherwise, <b>FALSE</b>.</summary>
	public BOOL Supported ;
	
	public FeatureDataExistingHeaps( bool supported = false ) => Supported = supported ;
} ;


/// <summary>Indicates the level of support for heap serialization.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_SERIALIZATION ) )]
public partial struct FeatureDataSerialization {
	/// <summary>
	/// <para>An input field, indicating the adapter index to query.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_serialization#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint NodeIndex;

	/// <summary>
	/// <para>An output field, indicating the tier of heap serialization support.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_serialization#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public HeapSerializationTier HeapSerializationTier ;
	
	
	public FeatureDataSerialization( uint nodeIndex = 0, 
									 HeapSerializationTier heapSerializationTier = 
										 HeapSerializationTier.Tier0 ) {
		NodeIndex            = nodeIndex ;
		HeapSerializationTier = heapSerializationTier ;
	}
} ;


/// <summary>Indicates the level of support for the sharing of resources between different adapters&mdash;for example, multiple GPUs.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_CROSS_NODE ) )]
public partial struct FeatureDataCrossNode {
	/// <summary>
	/// <para>Indicates the tier of cross-adapter sharing support.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_cross_node#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public CrossNodeSharingTier SharingTier ;

	/// <summary>
	/// <para>Indicates there is support for shader instructions which operate across adapters.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_cross_node#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL AtomicShaderInstructions ;
	
	
	public FeatureDataCrossNode( CrossNodeSharingTier sharingTier = CrossNodeSharingTier.NotSupported,
								 bool atomicShaderInstructions = false ) {
		SharingTier              = sharingTier ;
		AtomicShaderInstructions = atomicShaderInstructions ;
	}
} ;


/// <summary>This feature is currently in preview.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_DISPLAYABLE ) )]
public partial struct FeatureDataDisplayable {
	/// <summary></summary>
	public BOOL DisplayableTexture ;

	/// <summary></summary>
	public SharedResourceCompatibilityTier SharedResourceCompatibilityTier ;
	
	
	public FeatureDataDisplayable( bool displayableTexture = false, 
								   SharedResourceCompatibilityTier sharedResourceCompatibilityTier = 
									   SharedResourceCompatibilityTier.Tier0 ) {
		DisplayableTexture             = displayableTexture ;
		SharedResourceCompatibilityTier = sharedResourceCompatibilityTier ;
	}
} ;


/// <summary>Indicates the level of support that the adapter provides for metacommands.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_QUERY_META_COMMAND ) )]
public partial struct FeatureDataQueryMetaCommand {
	
	/// <summary>
	/// <para>The fixed GUID that identifies the metacommand to query about.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_query_meta_command#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public Guid CommandId ;

	/// <summary>
	/// <para>For single GPU operation, this is zero.
	/// If there are multiple GPU nodes, a bit is set to identify a node (the device's physical adapter).
	/// Each bit in the mask corresponds to a single node. Only 1 bit must be set.
	/// Refer to <a href="https://learn.microsoft.com/en-us/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_query_meta_command#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NodeMask ;

	/// <summary>
	/// <para>A pointer to a buffer containing the query input data. Allocate *QueryInputDataSizeInBytes* bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_query_meta_command#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public nint pQueryInputData ;

	/// <summary>
	/// <para>The size of the buffer pointed to by *pQueryInputData*, in bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_query_meta_command#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public nuint QueryInputDataSizeInBytes ;

	/// <summary>
	/// <para>A pointer to a buffer containing the query output data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_query_meta_command#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public unsafe nint pQueryOutputData ;

	/// <summary>
	/// <para>The size of the buffer pointed to by *pQueryOutputData*, in bytes.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_query_meta_command#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public nuint QueryOutputDataSizeInBytes ;
	
	
	public unsafe FeatureDataQueryMetaCommand( Guid commandId, uint nodeMask, 
											   nint pQueryInputData, nuint queryInputDataSizeInBytes, 
											   nint pQueryOutputData, nuint queryOutputDataSizeInBytes ) {
		CommandId                  = commandId ;
		NodeMask                   = nodeMask ;
		this.pQueryInputData       = pQueryInputData ;
		QueryInputDataSizeInBytes  = queryInputDataSizeInBytes ;
		this.pQueryOutputData      = pQueryOutputData ;
		QueryOutputDataSizeInBytes = queryOutputDataSizeInBytes ;
	}
} ;


/// <summary>Indicates a count of protected resource session types.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_PROTECTED_RESOURCE_SESSION_TYPE_COUNT ) )]
public partial struct FeatureDataProtectedResourceSessionTypeCount {
	/// <summary>
	/// <para>An input parameter which, in multi-adapter operation, indicates which physical adapter of the device this operation applies to.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_type_count#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint NodeIndex ;

	/// <summary>
	/// <para>An output parameter containing the number of protected resource session types supported by the driver.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_type_count#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint Count ;
	
	public FeatureDataProtectedResourceSessionTypeCount( uint nodeIndex = 0, uint count = 0 ) {
		NodeIndex = nodeIndex ;
		Count     = count ;
	}
} ;


/// <summary>Indicates a list of protected resource session types.</summary>
[EquivalentOf(typeof( D3D12_FEATURE_DATA_PROTECTED_RESOURCE_SESSION_TYPES))]
public partial struct FeatureDataProtectedResourceSessionTypes {
	/// <summary>
	/// <para>An input parameter which, in multi-adapter operation, indicates which physical adapter of the device this operation applies to.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_types#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint NodeIndex ;

	/// <summary>
	/// <para>An input parameter indicating the size of the *pTypes* array. This must match the count returned through the [D3D12_FEATURE_PROTECTED_RESOURCE_SESSION_TYPE_COUNT](ne-d3d12-d3d12_feature.md) query.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_types#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint Count ;

	/// <summary>
	/// <para>An output parameter containing an array populated with the supported protected resource session types.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_types#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public unsafe Guid* pTypes ;
	
	
	/// <summary>
	/// Gets a span of <see cref="Guid"/> values that represent the supported protected resource session types.
	/// </summary>
	public unsafe Span< Guid > Types => new( pTypes, (int) Count ) ;
	
	
	public unsafe FeatureDataProtectedResourceSessionTypes( uint nodeIndex = 0, 
															uint count = 0, 
															Guid* pTypes = null ) {
		NodeIndex = nodeIndex ;
		Count     = count ;
		this.pTypes = pTypes ;
	}
} ;

