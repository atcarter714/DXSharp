#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>Specifies the type of a raytracing acceleration structure.</summary>
/// <remarks>
/// Bottom-level acceleration structures each consist of a set of geometries that are building blocks for a scene.
/// A top-level acceleration structure represents a set of instances of bottom-level acceleration structures.
/// </remarks>
[EquivalentOf(typeof(D3D12_RAYTRACING_ACCELERATION_STRUCTURE_TYPE))]
public enum RaytracingAccelerationStructureType {
	/// <summary>Top-level acceleration structure.</summary>
	TopLevel = 0,
	/// <summary>Bottom-level acceleration structure.</summary>
	BottomLevel = 1,
} ;


/// <summary>
/// Specifies flags for the build of a raytracing acceleration structure. Use a value from this enumeration with the
/// <see cref="BuildRaytracingAccelerationStructureInputs"/> structure that provides input to the acceleration structure build operation.
/// </summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_build_flags">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[Flags, EquivalentOf(typeof(D3D12_RAYTRACING_ACCELERATION_STRUCTURE_BUILD_FLAGS))]
public enum RaytracingAccelerationStructureBuildFlags {
	/// <summary>No options specified for the acceleration structure build.</summary>
	None = 0x00000000,
	/// <summary>
	/// <para>Build the acceleration structure such that it supports future updates (via the flag <b>PERFORM_UPDATE</b>) instead of the app having to entirely rebuild the structure.  This option may result in increased memory consumption, build times, and lower raytracing performance.  Future updates, however, should be faster than building the equivalent acceleration structure from scratch. This flag can only be set on an initial acceleration structure build, or on an update where the source acceleration structure specified <b>ALLOW_UPDATE</b>.  In other words, after an acceleration structure was been built without <b>ALLOW_UPDATE</b>, no other acceleration structures can be created from it via updates.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_build_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	AllowUpdate = 0x00000001,
	/// <summary>
	/// <para>Enables the option to compact the acceleration structure by calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-copyraytracingaccelerationstructure">CopyRaytracingAccelerationStructure</a> using compact mode, specified with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_copy_mode">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_COPY_MODE_COMPACT</a>. This option may result in increased memory consumption and build times.  After future compaction, however, the resulting acceleration structure should consume a smaller memory footprint than building the acceleration structure from scratch. This flag is compatible with all other flags.  If specified as part of an acceleration structure update, the source acceleration structure must have also been built with this flag.  In other words, after an acceleration structure was been built without <b>ALLOW_COMPACTION</b>, no other acceleration structures can be created from it via updates that specify <b>ALLOW_COMPACTION</b>.</para>
	/// <para>Specifying ALLOW_COMPACTION may increase pre-compaction acceleration structure size versus not specifying ALLOW_COMPACTION.</para>
	/// <para>If multiple incremental builds are performed before finally compacting, there may be redundant compaction related work performed.</para>
	/// <para>The size required for the compacted acceleration structure can be queried before compaction via <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-emitraytracingaccelerationstructurepostbuildinfo">EmitRaytracingAccelerationStructurePostbuildInfo</a>. See <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_raytracing_acceleration_structure_postbuild_info_compacted_size_desc">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_POSTBUILD_INFO_COMPACTED_SIZE_DESC</a> for more information on properties of compacted acceleration structure size. <div class="alert"><b>Note</b>  When <b>ALLOW_UPDATE</b> is specified, there is certain information that needs to be retained in the acceleration structure, and compaction will only help so much. However, if the pipeline knows that the acceleration structure will no longer be updated, it can make the structure more compact.  Some apps may benefit from compacting twice - once after the initial build, and again after the acceleration structure has settled to a static state, if that occurs.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_build_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	AllowCompaction = 0x00000002,
	/// <summary>
	/// <para>Construct a high quality acceleration structure that maximizes raytracing performance at the expense of additional build time.  Typically, the implementation will take 2-3 times the build time than the default setting in order to get better tracing performance. This flag is recommended for static geometry in particular.  It is compatible with all other flags except for <b>PREFER_FAST_BUILD</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_build_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	PreferFastTrace = 0x00000004,
	/// <summary>
	/// <para>Construct a lower quality acceleration structure, trading raytracing performance for build speed.  Typically, the implementation will take 1/2 to 1/3 the build time than default setting, with a sacrifice in tracing performance. This flag is compatible with all other flags except for <b>PREFER_FAST_BUILD</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_build_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	PreferFastBuild = 0x00000008,
	/// <summary>
	/// <para>Minimize the amount of scratch memory used during the acceleration structure build as well as the size of the result.  This option may result in increased build times and/or raytracing times. This is orthogonal to the <b>ALLOW_COMPACTION</b> flag and the explicit acceleration structure compaction that it enables.  Combining the flags can mean both the initial acceleration structure as well as the result of compacting it use less memory. The impact of using this flag for a build is reflected in the result of calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device5-getraytracingaccelerationstructureprebuildinfo">GetRaytracingAccelerationStructurePrebuildInfo</a> before doing the build to retrieve memory requirements for the build. This flag is compatible with all other flags.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_build_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	MinimizeMemory = 0x00000010,
	/// <summary>
	/// <para>Perform an acceleration structure update, as opposed to building from scratch.  This is faster than a full build, but can negatively impact raytracing performance, especially if the positions of the underlying objects have changed significantly from the original build of the acceleration structure before updates. If the addresses of the source and destination acceleration structures are identical, the update is performed in-place.  Any other overlapping of address ranges of the source and destination is invalid.  For non-overlapping source and destinations, the source acceleration structure is unmodified.  The memory requirement for the output acceleration structure is the same as in the input acceleration structure The source acceleration structure must have been built with <b>ALLOW_UPDATE</b>. This flag is compatible with all other flags.  The other flags selections, aside from <b>ALLOW_UPDATE</b> and <b>PERFORM_UPDATE</b>, must match the flags in the source acceleration structure. Acceleration structure updates can be performed in unlimited succession, as long as the source acceleration structure was created with <b>ALLOW_UPDATE</b> and the flags for the update build continue to specify <b>ALLOW_UPDATE</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_build_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	PerformUpdate = 0x00000020,
} ;


[Flags, EquivalentOf( typeof( D3D12_RAYTRACING_GEOMETRY_TYPE ) )]
public enum RaytracingGeometryType {
	/// <summary>The geometry consists of triangles.</summary>
	Triangles = 0,
	/// <summary>The geometry procedurally is defined during raytracing by intersection shaders.  For the purpose of acceleration structure builds, the geometry’s bounds are described with axis-aligned bounding boxes using the  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_raytracing_geometry_aabbs_desc">D3D12_RAYTRACING_GEOMETRY_AABBS_DESC</a> structure.</summary>
	ProceduralPrimitiveAABBs = 1,
} ;


[Flags, EquivalentOf( typeof( D3D12_RAYTRACING_GEOMETRY_FLAGS ) )]
public enum RaytracingGeometryFlags {
	/// <summary>No options specified.</summary>
	None = 0x00000000,
	/// <summary>When rays encounter this geometry, the geometry acts as if no any hit shader is present.  It is recommended that apps use this flag liberally, as it can enable important ray-processing optimizations.  Note that this behavior can be overridden on a per-instance basis with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_raytracing_instance_flags">D3D12_RAYTRACING_INSTANCE_FLAGS</a> and on a per-ray basis using ray flags in <b>TraceRay</b>.</summary>
	Opaque = 0x00000001,
	/// <summary>
	/// <para>By default, the system is free to trigger an any hit shader more than once for a given ray-primitive intersection.
	/// This flexibility helps improve the traversal efficiency of acceleration structures in certain cases.
	/// For instance, if the acceleration structure is implemented internally with bounding volumes, the implementation may find it
	/// beneficial to store relatively long triangles in multiple bounding boxes rather than a larger single box. However, some
	/// application use cases require that intersections be reported to the any hit shader at most once. This flag enables that guarantee
	/// for the given geometry, potentially with some performance impact. This flag applies to all geometry types.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_geometry_flags#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	NoDuplicateAnyHitInvocation = 0x00000002,
} ;


[Flags, EquivalentOf( typeof( D3D12_RAYTRACING_ACCELERATION_STRUCTURE_COPY_MODE ) )]
public enum RaytracingAccelerationStructureCopyMode {
	/// <summary>
	/// <para>Copy an acceleration structure while fixing any self-referential pointers that may be present so that the destination is a self-contained copy of the source.  Any external pointers to other acceleration structures remain unchanged from source to destination in the copy.  The size of the destination is identical to the size of the source. > [!IMPORTANT] > The source memory must be in state [**D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE**](/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states). > The destination memory must be in state [**D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE**](/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_copy_mode#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Clone = 0,
	/// <summary>
	/// <para>Produces a functionally equivalent acceleration structure to source in the destination, similar to the clone mode, but also fits the destination into a potentially smaller, and certainly not larger, memory footprint.  The size required for the destination can be retrieved beforehand from <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-emitraytracingaccelerationstructurepostbuildinfo">EmitRaytracingAccelerationStructurePostbuildInfo</a>. This mode is only valid if the source acceleration structure was originally built with the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_build_flags">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_BUILD_FLAG_ALLOW_COMPACTION</a>  flag, otherwise results are undefined. Compacting geometry requires the entire acceleration structure to be constructed, which is why you must first build and then compact the structure. > [!IMPORTANT] > The source memory must be in state [**D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE**](/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states). > The destination memory must be in state [**D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE**](/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_copy_mode#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Compact = 1,
	/// <summary>
	/// <para>The destination is takes  the layout described in <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_build_raytracing_acceleration_structure_tools_visualization_header">D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_TOOLS_VISUALIZATION_HEADER</a>.  The size required for the destination can be retrieved beforehand from <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-emitraytracingaccelerationstructurepostbuildinfo">EmitRaytracingAccelerationStructurePostbuildInfo</a>. This mode is only intended for tools such as PIX, though nothing stops any app from using it.  The output is essentially the inverse of an acceleration structure build.  This overall structure with is sufficient for tools/PIX to be able to give the application some visual sense of the acceleration structure the driver made out of the app’s input.  Visualization can help reveal driver bugs in acceleration structures if what is shown grossly mismatches the data the application used to create the acceleration structure, beyond allowed tolerances. For top-level acceleration structures, the output includes a set of instance descriptions that are identical to the data used in the original build and in the same order.  For bottom-level acceleration structures, the output includes a set of geometry descriptions roughly matching the data used in the original build.  The output is only a rough match for the original in part because of the tolerances allowed in the specification for acceleration structures and in part due to the inherent complexity of reporting exactly the same structure as is conceptually encoded.  For example. axis-aligned bounding boxes (AABBs) returned for procedural primitives could be more conservative (larger) in volume and even different in number than what is actually in the acceleration structure representation.  Geometries, each with its own geometry description, appear in the same order as in the original acceleration, as shader table indexing calculations depend on this.</para>
	/// <para>> [!IMPORTANT] > The source memory must be in state [**D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE**](/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states). > The destination memory must be in state [**D3D12_RESOURCE_STATE_UNORDERED_ACCESS**](/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states). This mode is only permitted when developer mode is enabled in the OS.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_copy_mode#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	VisualizationDecodeForTools = 2,
	/// <summary>
	/// <para>Destination takes the layout and size described in the documentation for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_raytracing_acceleration_structure_postbuild_info_serialization_desc">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_POSTBUILD_INFO_SERIALIZATION_DESC</a>, itself a structure generated with a call to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist4-emitraytracingaccelerationstructurepostbuildinfo">EmitRaytracingAccelerationStructurePostbuildInfo</a>. This mode serializes an acceleration structure so that an app or tools can store it to a file for later reuse, typically on a different device instance, via deserialization. When serializing a top-level acceleration structure, the bottom-level acceleration structures it refers to do not have to still be present or intact in memory.  Likewise, bottom-level acceleration structures can be serialized independent of whether any top-level acceleration structures are pointing to them.  In other words, the order of serialization of acceleration structures doesn’t matter. > [!IMPORTANT] > The source memory must be in state [**D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE**](/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states). > The destination memory must be in state [**D3D12_RESOURCE_STATE_UNORDERED_ACCESS**](/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_copy_mode#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Serialize = 3,
	/// <summary>
	/// <para>The source must be a serialized acceleration structure, with any pointers, directly after the header, fixed to point to their new locations. For more information, see  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_raytracing_acceleration_structure_postbuild_info_serialization_desc">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_POSTBUILD_INFO_SERIALIZATION_DESC</a>. The destination gets an acceleration structure that is functionally equivalent to the acceleration structure that was originally serialized.  It does not matter what order top-level and bottom-level acceleration structures are deserialized, as long as by the time a top-level acceleration structure is used for raytracing or acceleration structure updates the bottom-level acceleration structures it references are present. Deserialization can only be performed on the same device and driver version on which the data was serialized. Otherwise, the results are undefined. This mode is only intended for tools such as PIX, though nothing stops any app from using it, but this mode is only permitted when developer mode is enabled in the OS.   This copy operation is not intended to be used for caching acceleration structures, because running a full acceleration structure build is likely to be faster than loading one from disk.</para>
	/// <para>> [!IMPORTANT] > The source memory must be in state [**D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE**](/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states). > The destination memory must be in state [**D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE**](/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_copy_mode#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Deserialize = 4,
} ;


/// <summary>
/// Specifies the type of acceleration structure post-build info that can be retrieved with calls
/// to EmitRaytracingAccelerationStructurePostbuildInfo and BuildRaytracingAccelerationStructure.
/// </summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_postbuild_info_type">
/// Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_RAYTRACING_ACCELERATION_STRUCTURE_POSTBUILD_INFO_TYPE))]
public enum RaytracingAccelerationStructurePostbuildInfoType {
	/// <summary>
	/// The post-build info is space requirements for an acceleration structure after compaction. For more information, see
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_raytracing_acceleration_structure_postbuild_info_compacted_size_desc">COMPACTED_SIZE_DESC</a>.
	/// </summary>
	CompactedSize = 0,
	/// <summary>The post-build info is space requirements for generating tools visualization for an acceleration structure. For more information, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_raytracing_acceleration_structure_postbuild_info_tools_visualization_desc">TOOLS_VISUALIZATION_DESC</a>.</summary>
	ToolsVisualization = 1,
	/// <summary>The post-build info is space requirements for serializing an acceleration structure. For more information, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_raytracing_acceleration_structure_postbuild_info_serialization_desc">SERIALIZATION_DESC</a>.</summary>
	Serialization = 2,
	/// <summary>The post-build info is size of the current acceleration structure. For more information, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_raytracing_acceleration_structure_postbuild_info_current_size_desc">CURRENT_SIZE_DESC</a>.</summary>
	CurrentSize = 3,
} ;