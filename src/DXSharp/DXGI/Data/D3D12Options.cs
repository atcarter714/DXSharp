#region Using Directives
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Direct3D12 ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Describes Direct3D 12 feature options in the current graphics driver.</summary>
/// <remarks>See <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a>.</remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS ) )]
public struct D3D12Options {
	/// <summary>
	/// <para>Specifies whether <b>double</b> types are allowed for shader operations. If <b>TRUE</b>, double types are allowed; otherwise <b>FALSE</b>. The supported operations are equivalent to Direct3D 11's <b>ExtendedDoublesShaderInstructions</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options">D3D11_FEATURE_DATA_D3D11_OPTIONS</a> structure.</para>
	/// <para>To use any HLSL shader that is compiled with a <b>double</b> type, the runtime must set <b>DoublePrecisionFloatShaderOps</b> to <b>TRUE</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL DoublePrecisionFloatShaderOps ;

	/// <summary>Specifies whether logic operations are available in blend state. The runtime sets this member to <b>TRUE</b> if logic operations are available in blend state and <b>FALSE</b> otherwise. This member is <b>FALSE</b> for feature level 9.1, 9.2, and 9.3.  This member is optional for feature level 10, 10.1, and 11.  This member is <b>TRUE</b> for feature level 11.1 and 12.</summary>
	public BOOL OutputMergerLogicOp ;

	/// <summary>A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_shader_min_precision_support">D3D12_SHADER_MIN_PRECISION_SUPPORT</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies minimum precision levels that the driver supports for shader stages. A value of zero indicates that the driver supports only full 32-bit precision for all shader stages.</summary>
	public ShaderMinPrecisionSupport MinPrecisionSupport ;

	/// <summary>Specifies whether the hardware and driver support tiled resources. The runtime sets this member to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_tiled_resources_tier">D3D12_TILED_RESOURCES_TIER</a>-typed value that indicates if the hardware and driver support tiled resources and at what tier level.</summary>
	public TiledResourcesTier TiledResourcesTier ;

	/// <summary>Specifies the level at which the hardware and driver support resource binding. The runtime sets this member to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_binding_tier">D3D12_RESOURCE_BINDING_TIER</a>-typed value that indicates the tier level.</summary>
	public ResourceBindingTier ResourceBindingTier ;

	/// <summary>Specifies whether pixel shader stencil ref is supported. If <b>TRUE</b>, it's supported; otherwise <b>FALSE</b>.</summary>
	public BOOL PSSpecifiedStencilRefSupported ;

	/// <summary>
	/// <para>Specifies whether the loading of additional formats for typed unordered-access views (UAVs) is supported. If <b>TRUE</b>, it's supported; otherwise <b>FALSE</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL TypedUAVLoadAdditionalFormats ;

	/// <summary>Specifies whether <a href="https://docs.microsoft.com/windows/desktop/direct3d12/directx-12-glossary">Rasterizer Order Views</a> (ROVs) are supported. If <b>TRUE</b>, they're supported; otherwise <b>FALSE</b>.</summary>
	public BOOL ROVsSupported ;

	/// <summary>Specifies the level at which the hardware and driver support conservative rasterization. The runtime sets this member to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_conservative_rasterization_tier">D3D12_CONSERVATIVE_RASTERIZATION_TIER</a>-typed value that indicates the tier level.</summary>
	public ConservativeRasterizationTier ConservativeRasterizationTier ;

	/// <summary>
	/// <para>Don't use this field; instead, use the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_gpu_virtual_address_support">D3D12_FEATURE_DATA_GPU_VIRTUAL_ADDRESS_SUPPORT</a> query (a structure with a <b>MaxGPUVirtualAddressBitsPerResource</b> member), which is more accurate.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint MaxGPUVirtualAddressBitsPerResource ;

	/// <summary>
	/// <para>TRUE if the hardware supports textures with the 64KB standard swizzle pattern. Support for this pattern enables zero-copy texture optimizations while providing near-equilateral locality for each dimension within the texture. For texture swizzle options and restrictions, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_layout">D3D12_TEXTURE_LAYOUT</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL StandardSwizzle64KBSupported ;

	/// <summary>
	/// <para>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_cross_node_sharing_tier">D3D12_CROSS_NODE_SHARING_TIER</a> enumeration constant that specifies the level of sharing across nodes of an adapter that has multiple nodes, such as Tier 1 Emulated, Tier 1, or Tier 2.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public CrossNodeSharingTier CrossNodeSharingTier ;

	/// <summary>
	/// <para>FALSE means the device only supports copy operations to and from cross-adapter row-major textures. TRUE means the device supports shader resource views, unordered access views, and render target views of cross-adapter row-major textures. "Cross-adapter" means between multiple adapters (even from different IHVs).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL CrossAdapterRowMajorTextureSupported ;

	/// <summary>
	/// <para>Whether the viewport (VP) and Render Target (RT) array index from any shader feeding the rasterizer are supported without geometry shader emulation. Compare the <b>VPAndRTArrayIndexFromAnyShaderFeedingRasterizer</b> member of the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_feature_data_d3d11_options3">D3D11_FEATURE_DATA_D3D11_OPTIONS3</a> structure. In <a href="https://docs.microsoft.com/windows/desktop/api/d3d12shader/nf-d3d12shader-id3d12shaderreflection-getrequiresflags">ID3D12ShaderReflection::GetRequiresFlags</a>, see the #define D3D_SHADER_REQUIRES_VIEWPORT_AND_RT_ARRAY_INDEX_FROM_ANY_SHADER_FEEDING_RASTERIZER.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL VPAndRTArrayIndexFromAnyShaderFeedingRasterizerSupportedWithoutGSEmulation ;

	/// <summary>
	/// <para>Specifies the level at which the hardware and driver require heap attribution related to resource type. The runtime sets this member to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_heap_tier">D3D12_RESOURCE_HEAP_TIER</a> enumeration constant.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ResourceHeapTier ResourceHeapTier ;
	
	
	public D3D12Options( bool doublePrecisionFloatShaderOps = false, 
						 bool outputMergerLogicOp = false, 
						 ShaderMinPrecisionSupport minPrecisionSupport = ShaderMinPrecisionSupport.None, 
						 TiledResourcesTier tiledResourcesTier = TiledResourcesTier.NotSupported, 
						 ResourceBindingTier resourceBindingTier = ResourceBindingTier.Tier1, 
						 bool pSSpecifiedStencilRefSupported = false, 
						 bool typedUAVLoadAdditionalFormats = false, 
						 bool rOVsSupported = false, 
						 ConservativeRasterizationTier conservativeRasterizationTier = ConservativeRasterizationTier.TierNotSupported, 
						 uint maxGPUVirtualAddressBitsPerResource = 0U, 
						 bool standardSwizzle64KBSupported = false, 
						 CrossNodeSharingTier crossNodeSharingTier = CrossNodeSharingTier.NotSupported, 
						 bool crossAdapterRowMajorTextureSupported = false, 
						 bool vPAndRTArrayIndexFromAnyShaderFeedingRasterizerSupportedWithoutGSEmulation = false, 
						 ResourceHeapTier resourceHeapTier = ResourceHeapTier.Tier1 ) {
		DoublePrecisionFloatShaderOps = doublePrecisionFloatShaderOps ;
		OutputMergerLogicOp = outputMergerLogicOp ;
		MinPrecisionSupport = minPrecisionSupport ;
		TiledResourcesTier = tiledResourcesTier ;
		ResourceBindingTier = resourceBindingTier ;
		PSSpecifiedStencilRefSupported = pSSpecifiedStencilRefSupported ;
		TypedUAVLoadAdditionalFormats = typedUAVLoadAdditionalFormats ;
		ROVsSupported = rOVsSupported ;
		ConservativeRasterizationTier = conservativeRasterizationTier ;
		MaxGPUVirtualAddressBitsPerResource = maxGPUVirtualAddressBitsPerResource ;
		StandardSwizzle64KBSupported = standardSwizzle64KBSupported ;
		CrossNodeSharingTier = crossNodeSharingTier ;
		CrossAdapterRowMajorTextureSupported = crossAdapterRowMajorTextureSupported ;
		VPAndRTArrayIndexFromAnyShaderFeedingRasterizerSupportedWithoutGSEmulation = vPAndRTArrayIndexFromAnyShaderFeedingRasterizerSupportedWithoutGSEmulation ;
		ResourceHeapTier = resourceHeapTier ;
	}
} ;


/// <summary>Describes the level of support for HLSL 6.0 wave operations.</summary>
/// <remarks>
/// <para>A "lane" is  single thread of execution. The shader models before version 6.0 expose only one of these at the language level, leaving expansion to parallel SIMD processing entirely up to the implementation.</para>
/// <para>A "wave" is  set of lanes (threads) executed simultaneously in the processor. No explicit barriers are required to guarantee that they execute in parallel. Similar concepts include "warp" and "wavefront".</para>
/// <para>This structure is used with the D3D12_FEATURE_D3D12_OPTIONS1 member of  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options1#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS1 ) )]
public struct D3D12Options1 {
	/// <summary>True if the driver supports HLSL 6.0 wave operations.</summary>
	public BOOL WaveOps ;

	/// <summary>Specifies the baseline number of lanes in the SIMD wave that this implementation can support. This term is sometimes known as "wavefront size" or "warp width". Currently apps should rely only on this minimum value for sizing workloads.</summary>
	public uint WaveLaneCountMin ;

	/// <summary>Specifies the maximum number of lanes in the SIMD wave that this implementation can support. This capability is reserved for future expansion, and is not expected to be used by current applications.</summary>
	public uint WaveLaneCountMax ;

	/// <summary>Specifies the total number of SIMD lanes on the hardware.</summary>
	public uint TotalLaneCount ;

	/// <summary>Indicates transitions are possible  in and out of the CBV, and indirect argument states, on compute command lists. If <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a> succeeds this value will always be true.</summary>
	public BOOL ExpandedComputeResourceStates ;

	/// <summary>Indicates that 64bit integer operations are supported.</summary>
	public BOOL Int64ShaderOps ;
	
	
	public D3D12Options1( bool waveOps = false, 
						  uint waveLaneCountMin = 0U, 
						  uint waveLaneCountMax = 0U, 
						  uint totalLaneCount = 0U, 
						  bool expandedComputeResourceStates = false, 
						  bool int64ShaderOps = false ) {
		WaveOps = waveOps ;
		WaveLaneCountMin = waveLaneCountMin ;
		WaveLaneCountMax = waveLaneCountMax ;
		TotalLaneCount = totalLaneCount ;
		ExpandedComputeResourceStates = expandedComputeResourceStates ;
		Int64ShaderOps = int64ShaderOps ;
	}
} ;


/// <summary>Indicates the level of support that the adapter provides for depth-bounds tests and programmable sample positions.</summary>
/// <remarks>
/// <para>Use this structure with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a> to determine the level of support offered for the optional Depth-bounds test and programmable sample positions features. See the enumeration constant D3D12_FEATURE_D3D12_OPTIONS2 in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE</a> enumeration.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options2#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS2 ) )]
public struct D3D12Options2 {
	/// <summary>
	/// <para><a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-structs-and-classes">SAL</a>: <c>_Out_</c> On return, contains true if depth-bounds tests are supported; otherwise, false.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options2#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL DepthBoundsTestSupported;

	/// <summary>
	/// <para><a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-structs-and-classes">SAL</a>: <c>_Out_</c> On return, contains a value that indicates the level of support offered for programmable sample positions.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options2#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ProgrammableSamplePositionsTier ProgrammableSamplePositionsTier ;

	
	public D3D12Options2( bool depthBoundsTestSupported = false,
						  ProgrammableSamplePositionsTier programmableSamplePositionsTier = 
							  ProgrammableSamplePositionsTier.NotSupported ) {
		DepthBoundsTestSupported = depthBoundsTestSupported ;
		ProgrammableSamplePositionsTier = programmableSamplePositionsTier ;
	}
} ;


/// <summary>Indicates the level of support that the adapter provides for timestamp queries, format-casting, immediate write, view instancing, and barycentrics.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options3">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS3 ) )]
public struct D3D12Options3 {
	/// <summary>Indicates whether timestamp queries are supported on copy queues.</summary>
	public BOOL CopyQueueTimestampQueriesSupported ;

	/// <summary>Indicates whether casting from one fully typed format to another, compatible, format is supported.</summary>
	public BOOL CastingFullyTypedFormatSupported ;

	/// <summary>Indicates the kinds of command lists that support the ability to write an immediate value directly from the command stream into a specified buffer.</summary>
	public CommandListSupportFlags WriteBufferImmediateSupportFlags ;

	/// <summary>Indicates the level of support the adapter has for view instancing.</summary>
	public ViewInstancingTier ViewInstancingTier ;

	/// <summary>Indicates whether barycentrics are supported.</summary>
	public BOOL BarycentricsSupported ;
	
	
	public D3D12Options3( bool copyQueueTimestampQueriesSupported = false, 
						  bool castingFullyTypedFormatSupported = false, 
						  CommandListSupportFlags writeBufferImmediateSupportFlags = CommandListSupportFlags.None, 
						  ViewInstancingTier viewInstancingTier = ViewInstancingTier.NotSupported, 
						  bool barycentricsSupported = false ) {
		CopyQueueTimestampQueriesSupported = copyQueueTimestampQueriesSupported ;
		CastingFullyTypedFormatSupported = castingFullyTypedFormatSupported ;
		WriteBufferImmediateSupportFlags = writeBufferImmediateSupportFlags ;
		ViewInstancingTier = viewInstancingTier ;
		BarycentricsSupported = barycentricsSupported ;
	}
} ;


/// <summary>Indicates the level of support for 64KB-aligned MSAA textures, cross-API sharing, and native 16-bit shader operations.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS4 ) )]
public struct D3D12Options4 {
	
	/// <summary>
	/// <para>Type: **[BOOL](/windows/desktop/winprog/windows-data-types)** Indicates whether 64KB-aligned MSAA textures are supported.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options4#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL MSAA64KBAlignedTextureSupported ;

	/// <summary>
	/// <para>Type: **[D3D12_SHARED_RESOURCE_COMPATIBILITY_TIER](/windows/desktop/api/d3d12/ne-d3d12-d3d12_shared_resource_compatibility_tier)** Indicates the tier of cross-API sharing support.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options4#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public SharedResourceCompatibilityTier SharedResourceCompatibilityTier ;

	/// <summary>
	/// <para>Type: **[BOOL](/windows/desktop/winprog/windows-data-types)** Indicates native 16-bit shader operations are supported. These operations require shader model 6_2. For more information, see the [16-Bit Scalar Types](https://github.com/microsoft/DirectXShaderCompiler/wiki/16-Bit-Scalar-Types) HLSL reference.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options4#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL Native16BitShaderOpsSupported ;

	public D3D12Options4( bool msaa64KBAlignedTextureSupported = false, 
						  SharedResourceCompatibilityTier sharedResourceCompatibilityTier = SharedResourceCompatibilityTier.Tier0, 
						  bool native16BitShaderOpsSupported = false ) {
		MSAA64KBAlignedTextureSupported = msaa64KBAlignedTextureSupported ;
		SharedResourceCompatibilityTier = sharedResourceCompatibilityTier ;
		Native16BitShaderOpsSupported = native16BitShaderOpsSupported ;
	}
} ;


/// <summary>Indicates the level of support that the adapter provides for render passes, ray tracing, and shader-resource view tier 3 tiled resources.</summary>
/// <remarks>Pass <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_feature">D3D12_FEATURE_D3D12_OPTIONS5</a> to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">ID3D12Device::CheckFeatureSupport</a> to retrieve a <b>D3D12_FEATURE_DATA_D3D12_OPTIONS5</b> structure.</remarks>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS5 ) )]
public struct D3D12Options5 {
	/// <summary>A boolean value indicating whether the options require shader-resource view tier 3 tiled resource support. For more information, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_tiled_resources_tier">D3D12_TILED_RESOURCES_TIER</a>.</summary>
	public BOOL SRVOnlyTiledResourceTier3 ;

	/// <summary>The extent to which a device driver and/or the hardware efficiently supports render passes. See <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_render_pass_tier">D3D12_RENDERPASS_TIER</a>.</summary>
	public RenderPassTier RenderPassesTier ;

	/// <summary></summary>
	public RaytracingTier RaytracingTier ;
	
	
	public D3D12Options5( bool srvOnlyTiledResourceTier3 = false, 
						  RenderPassTier renderPassesTier = RenderPassTier.Tier0, 
						  RaytracingTier raytracingTier = RaytracingTier.NotSupported ) {
		SRVOnlyTiledResourceTier3 = srvOnlyTiledResourceTier3 ;
		RenderPassesTier = renderPassesTier ;
		RaytracingTier = raytracingTier ;
	}
} ;


/// <summary>
/// Indicates the level of support that the adapter provides for variable-rate shading (VRS), and
/// indicates whether or not background processing is supported.
/// </summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS6 ) )]
public struct D3D12Options6 {
	
	/// <summary>
	/// <para>Type: <b>[BOOL](/windows/desktop/winprog/windows-data-types)</b> Indicates whether 2x4, 4x2, and 4x4 coarse pixel sizes are supported for single-sampled rendering; and whether coarse pixel size 2x4 is supported for 2x MSAA. `true` if those sizes are supported, otherwise `false`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options6#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL AdditionalShadingRatesSupported ;

	/// <summary>
	/// <para>Type: <b>[BOOL](/windows/desktop/winprog/windows-data-types)</b> Indicates whether the per-provoking-vertex (also known as per-primitive) rate can be used with more than one viewport. If so, then, in that case, that rate can be used when `SV_ViewportIndex` is written to. `true` if that rate can be used with more than one viewport, otherwise `false`.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options6#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL PerPrimitiveShadingRateSupportedWithViewportIndexing ;

	/// <summary>
	/// <para>Type: <b>[D3D12_VARIABLE_SHADING_RATE_TIER](/windows/desktop/api/d3d12/ne-d3d12-d3d12_variable_shading_rate_tier)</b> Indicates the shading rate tier.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options6#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public VariableShadingRateTier VariableShadingRateTier ;

	/// <summary>
	/// <para>Type: <b>[UINT](/windows/desktop/winprog/windows-data-types)</b> Indicates the tile size of the screen-space image as a **UINT**.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options6#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint ShadingRateImageTileSize ;

	/// <summary>
	/// <para>Type: <b>[BOOL](/windows/desktop/winprog/windows-data-types)</b> Indicates whether or not background processing is supported. `true` if background processing is supported, otherwise `false`. For more info, see the [Direct3D 12 background processing spec](https://microsoft.github.io/DirectX-Specs/d3d/BackgroundProcessing.html).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options6#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL BackgroundProcessingSupported ;
	
	
	public D3D12Options6( bool additionalShadingRatesSupported = false, 
						  bool perPrimitiveShadingRateSupportedWithViewportIndexing = false, 
						  VariableShadingRateTier variableShadingRateTier = VariableShadingRateTier.NotSupported, 
						  uint shadingRateImageTileSize = 0U, 
						  bool backgroundProcessingSupported = false ) {
		AdditionalShadingRatesSupported = additionalShadingRatesSupported ;
		PerPrimitiveShadingRateSupportedWithViewportIndexing = perPrimitiveShadingRateSupportedWithViewportIndexing ;
		VariableShadingRateTier = variableShadingRateTier ;
		ShadingRateImageTileSize = shadingRateImageTileSize ;
		BackgroundProcessingSupported = backgroundProcessingSupported ;
	}
} ;


/// <summary>Indicates the level of support that the adapter provides for mesh and amplification shaders, and for sampler feedback.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS7 ) )]
public struct D3D12Options7 {
	/// <summary>
	/// <para>Indicates the level of support for mesh and amplification shaders.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options7#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public MeshShaderTier MeshShaderTier ;

	/// <summary>
	/// <para>Indicates the level of support for sampler feedback.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options7#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public SamplerFeedbackTier SamplerFeedbackTier ;
	
	
	public D3D12Options7( MeshShaderTier meshShaderTier = MeshShaderTier.NotSupported, 
						  SamplerFeedbackTier samplerFeedbackTier = SamplerFeedbackTier.NotSupported ) {
		MeshShaderTier = meshShaderTier ;
		SamplerFeedbackTier = samplerFeedbackTier ;
	}
} ;


/// <summary>Indicates whether or not unaligned block-compressed textures are supported.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS8 ) )]
public struct D3D12Options8 {
	/// <summary>
	/// <para>Indicates whether or not unaligned block-compressed textures are supported.
	/// If `false`, then Direct3D 12 requires that the dimensions of the top-level mip of a block-compressed texture are aligned to multiples of 4 (such alignment requirements do not apply to less-detailed mips). If `true`, then no such alignment requirement applies to any mip level of a block-compressed texture.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options8#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL UnalignedBlockTexturesSupported ;
	
	
	public D3D12Options8( bool unalignedBlockTexturesSupported ) {
		UnalignedBlockTexturesSupported = unalignedBlockTexturesSupported ;
	}
} ;


/// <summary>
/// Indicates whether or not support exists for mesh shaders, values of *SV_RenderTargetArrayIndex* that are 8 or greater,
/// typed resource 64-bit integer atomics, derivative and derivative-dependent texture sample operations, and the level of
/// support for WaveMMA (wave_matrix) operations.
/// </summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS9 ) )]
public struct D3D12Options9 {

	/// <summary>
	/// <para>Indicates whether or not mesh shaders are supported. `true` if supported, otherwise `false`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options9#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL MeshShaderPipelineStatsSupported ;

	/// <summary>
	/// <para>Indicates whether or not values of *SV_RenderTargetArrayIndex* that are 8 or greater are supported. `true` if supported, otherwise `false`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options9#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL MeshShaderSupportsFullRangeRenderTargetArrayIndex ;

	/// <summary>
	/// <para>Indicates whether or not typed resource 64-bit integer atomics are supported. `true` if supported, otherwise `false`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options9#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL AtomicInt64OnTypedResourceSupported ;

	/// <summary>
	/// <para>Indicates whether or not 64-bit integer atomics are supported on `groupshared` variables. `true` if supported, otherwise `false`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options9#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL AtomicInt64OnGroupSharedSupported ;

	/// <summary>
	/// <para>Indicates whether or not derivative and derivative-dependent texture sample operations are supported. `true` if supported, otherwise `false`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options9#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL DerivativesInMeshAndAmplificationShadersSupported ;

	/// <summary>
	/// <para>Indicates the level of support for WaveMMA (wave_matrix) operations.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options9#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public WaveMMATier WaveMMATier ;
	
	
	public D3D12Options9( bool        meshShaderPipelineStatsSupported = false, 
						  bool        meshShaderSupportsFullRangeRenderTargetArrayIndex = false, 
						  bool        atomicInt64OnTypedResourceSupported = false, 
						  bool        atomicInt64OnGroupSharedSupported = false, 
						  bool        derivativesInMeshAndAmplificationShadersSupported = false, 
						  WaveMMATier waveMMATier = WaveMMATier.NotSupported ) {
		MeshShaderPipelineStatsSupported = meshShaderPipelineStatsSupported ;
		MeshShaderSupportsFullRangeRenderTargetArrayIndex = meshShaderSupportsFullRangeRenderTargetArrayIndex ;
		AtomicInt64OnTypedResourceSupported = atomicInt64OnTypedResourceSupported ;
		AtomicInt64OnGroupSharedSupported = atomicInt64OnGroupSharedSupported ;
		DerivativesInMeshAndAmplificationShadersSupported = derivativesInMeshAndAmplificationShadersSupported ;
		WaveMMATier = waveMMATier ;
	}
} ;


/// <summary>Indicates whether or not the SUM combiner can be used, and whether or not *SV_ShadingRate* can be set from a mesh shader.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS10 ) )]
public struct D3D12Options10 {
	/// <summary>
	/// <para>Indicates whether or not the SUM combiner can be used (this relates to <a href="https://learn.microsoft.com/en-us/windows/win32/direct3d12/vrs">Variable Rate Shading</a> Tier 2).
	/// `true` if it can, otherwise `false`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options10#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL VariableRateShadingSumCombinerSupported ;

	/// <summary>
	/// <para>Indicates whether or not *SV_ShadingRate* can be set from a mesh shader (this relates to <a href="https://learn.microsoft.com/en-us/windows/win32/direct3d12/vrs">Variable Rate Shading</a> Tier 2). `true` if it can, otherwise `false`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options10#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL MeshShaderPerPrimitiveShadingRateSupported ;
	
	
	public D3D12Options10( bool variableRateShadingSumCombinerSupported = false, 
						   bool meshShaderPerPrimitiveShadingRateSupported = false ) {
		VariableRateShadingSumCombinerSupported = variableRateShadingSumCombinerSupported ;
		MeshShaderPerPrimitiveShadingRateSupported = meshShaderPerPrimitiveShadingRateSupported ;
	}
} ;


/// <summary>Indicates whether or not 64-bit integer atomics on resources in descriptor heaps are supported.</summary>
[EquivalentOf( typeof( D3D12_FEATURE_DATA_D3D12_OPTIONS11 ) )]
public struct D3D12Options11 {
	
	/// <summary>
	/// <para>Indicates whether or not 64-bit integer atomics on resources in descriptor heaps are supported.
	/// `true` if supported, otherwise `false`.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options11#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public BOOL AtomicInt64OnDescriptorHeapResourceSupported ;
	
	public D3D12Options11( BOOL atomicInt64OnDescriptorHeapResourceSupported ) {
		AtomicInt64OnDescriptorHeapResourceSupported = atomicInt64OnDescriptorHeapResourceSupported ;
	}
	public D3D12Options11( bool atomicInt64OnDescriptorHeapResourceSupported ) {
		AtomicInt64OnDescriptorHeapResourceSupported = atomicInt64OnDescriptorHeapResourceSupported ;
	}
} ;