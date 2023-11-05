#region Using Directives
using System.Buffers ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>Describes a raytracing acceleration structure. Pass this structure into <see cref="IGraphicsCommandList4.BuildRaytracingAccelerationStructure"/> to describe the accelerationstructure to be built.</summary>
/// <remarks><para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_build_raytracing_acceleration_structure_desc">Learn more about this API from docs.microsoft.com</a>.</para></remarks>
[EquivalentOf( typeof( D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_DESC ) )]
public struct BuildRaytracingAccelerationStructureDescription {
	/// <summary>
	/// <para>Location to store resulting acceleration structure.
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device5-getraytracingaccelerationstructureprebuildinfo">ID3D12Device5::GetRaytracingAccelerationStructurePrebuildInfo</a>
	/// reports the amount of memory required for the result here given a set of acceleration structure build parameters. The address must be aligned to 256 bytes, defined as
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_BYTE_ALIGNMENT</a>.<para/>
	/// <b>[!IMPORTANT]</b> The memory must be in state D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_build_raytracing_acceleration_structure_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong DestAccelerationStructureData ;

	/// <summary>
	/// Description of the input data for the acceleration structure build.  This is data is stored in a
	/// separate structure because it is also used with <b>GetRaytracingAccelerationStructurePrebuildInfo</b>.
	/// </summary>
	public BuildRaytracingAccelerationStructureInputs Inputs ;

	/// <summary>
	/// <para>Address of an existing acceleration structure if an acceleration structure update (an incremental build) is being requested, by setting
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_raytracing_acceleration_structure_build_flags">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_BUILD_FLAG_PERFORM_UPDATE</a>
	/// in the Flags parameter.  Otherwise this address must be NULL. If this address is the same as <i>DestAccelerationStructureData</i>, the update is to be performed in-place.
	/// Any other form of overlap of the source and destination memory is invalid and produces undefined behavior. The address must be aligned to 256 bytes, defined as
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_ACCELERATION_STRUCTURE_BYTE_ALIGNMENT</a>, which should automatically be the case
	/// because any existing acceleration structure passed in here would have already been required to be placed with such alignment.<para/>
	/// <b>[!IMPORTANT]</b> The memory must be in state <see cref="ResourceStates.RaytracingAccelerationStructure"/>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_build_raytracing_acceleration_structure_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong SourceAccelerationStructureData ;

	/// <summary>
	/// Location where the build will store temporary data. GetRaytracingAccelerationStructurePrebuildInfo reports the amount of scratch memory the implementation
	/// will need for a given set of acceleration structure build parameters.
	/// </summary>
	/// <remarks>
	/// <b>Important</b> The memory must be in state <see cref="ResourceStates.UnorderedAccess"/>.
	/// </remarks>
	public ulong ScratchAccelerationStructureData ;

	
	public BuildRaytracingAccelerationStructureDescription( ulong destAccelerationStructureData = 0U,
															BuildRaytracingAccelerationStructureInputs inputs = default,
															ulong sourceAccelerationStructureData = 0U,
															ulong scratchAccelerationStructureData = 0U ) {
		DestAccelerationStructureData = destAccelerationStructureData ;
		Inputs = inputs ;
		SourceAccelerationStructureData = sourceAccelerationStructureData ;
		ScratchAccelerationStructureData = scratchAccelerationStructureData ;
	}
} ;


/// <summary>Defines the inputs for a raytracing acceleration structure build operation. This structure is used by <see cref="IGraphicsCommandList4.BuildRaytracingAccelerationStructure"/> and ID3D12Device5::GetRaytracingAccelerationStructurePrebuildInfo.</summary>
/// <remarks>When used with  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device5-getraytracingaccelerationstructureprebuildinfo">GetRaytracingAccelerationStructurePrebuildInfo</a>, which actually perform a build, any parameter that is referenced via <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12_gpu_virtual_address">D3D12_GPU_VIRTUAL_ADDRESS</a> (an address in GPU memory), like <i>InstanceDescs</i>, will not be accessed by the operation.  So this memory does not need to be initialized yet or be in a particular resource state.  Whether GPU addresses are null or not can be inspected by the operation, even though the pointers are not dereferenced.</remarks>
[EquivalentOf( typeof( D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_INPUTS ) )]
public partial struct BuildRaytracingAccelerationStructureInputs {
	/// <summary>The type of acceleration structure to build.</summary>
	public RaytracingAccelerationStructureType Type ;

	/// <summary>The build flags.</summary>
	public RaytracingAccelerationStructureBuildFlags Flags ;

	/// <summary>
	/// <para>If <i>Type</i> is <b><see cref="RaytracingAccelerationStructureType.TopLevel"/></b>, this value is the number of instances, laid out based on <i>DescsLayout</i>.
	/// If <i>Type</i> is <b><see cref="RaytracingAccelerationStructureType.BottomLevel"/></b>, this value is the number of elements referred to by <i>pGeometryDescs</i> or
	/// <i>ppGeometryDescs</i>. Which of these fields  is used depends on <i>DescsLayout</i>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_build_raytracing_acceleration_structure_inputs#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint NumDescs ;

	/// <summary>How geometry descriptions are specified; either an array of descriptions or an array of pointers to descriptions.</summary>
	public ElementsLayout DescsLayout ;

	public _descriptionUnion Description ;

	[StructLayout( LayoutKind.Explicit )]
	public unsafe partial struct _descriptionUnion {
		[FieldOffset( 0 )] public ulong                           InstanceDescs ;
		[FieldOffset( 0 )] public RaytracingGeometryDescription*  pGeometryDescs ;
		[FieldOffset( 0 )] public RaytracingGeometryDescription** ppGeometryDescs ;

		public _descriptionUnion( ulong instanceDescs ) {
			InstanceDescs   = instanceDescs ;
			pGeometryDescs  = null ;
			ppGeometryDescs = null ;
		}
		public _descriptionUnion( RaytracingGeometryDescription* pGeometryDescs ) {
			InstanceDescs       = 0U ;
			this.pGeometryDescs = pGeometryDescs ;
			ppGeometryDescs     = null ;
		}
		public _descriptionUnion( RaytracingGeometryDescription** ppGeometryDescs ) {
			InstanceDescs        = 0U ;
			pGeometryDescs       = null ;
			this.ppGeometryDescs = ppGeometryDescs ;
		}
	}


	public static MemoryHandle? Create( RaytracingGeometryDescription[ ] geometryDescriptions,
										out BuildRaytracingAccelerationStructureInputs inputs,
										RaytracingAccelerationStructureType Type =
											RaytracingAccelerationStructureType.TopLevel,
										RaytracingAccelerationStructureBuildFlags Flags =
											RaytracingAccelerationStructureBuildFlags.None ) {
		ArgumentNullException.ThrowIfNull( geometryDescriptions, nameof( geometryDescriptions ) ) ;
		Unsafe.SkipInit( out inputs ) ;

		var mem    = new Memory< RaytracingGeometryDescription >( geometryDescriptions ) ;
		var handle = mem.Pin( ) ;

		inputs.Type        = Type ;
		inputs.Flags       = Flags ;
		inputs.NumDescs    = (uint)geometryDescriptions.Length ;
		inputs.DescsLayout = ElementsLayout.Array ;
		unsafe {
			inputs.Description = new( (RaytracingGeometryDescription*)handle.Pointer ) ;
		}

		return handle ;
	}

	public static unsafe MemoryHandle? Create( RaytracingGeometryDescription*[ ] geometryDescriptions,
											   out BuildRaytracingAccelerationStructureInputs inputs,
											   RaytracingAccelerationStructureType Type =
												   RaytracingAccelerationStructureType.TopLevel,
											   RaytracingAccelerationStructureBuildFlags Flags =
												   RaytracingAccelerationStructureBuildFlags.None ) {
		ArgumentNullException.ThrowIfNull( geometryDescriptions, nameof( geometryDescriptions ) ) ;
		Unsafe.SkipInit( out inputs ) ;
		var nintArray = Unsafe.As< nint[] >( geometryDescriptions ) ;
		var mem       = new Memory< nint >( nintArray ) ;
		var handle    = mem.Pin( ) ;

		inputs.Type        = Type ;
		inputs.Flags       = Flags ;
		inputs.NumDescs    = 1U ;
		inputs.DescsLayout = ElementsLayout.ArrayOfPointers ;
		inputs.Description = new( (RaytracingGeometryDescription**)handle.Pointer ) ;

		return handle ;
	}

	public static BuildRaytracingAccelerationStructureInputs Create( ulong instanceDescriptions, ElementsLayout layout,
																	 RaytracingAccelerationStructureType Type =
																		 RaytracingAccelerationStructureType.TopLevel,
																	 RaytracingAccelerationStructureBuildFlags Flags =
																		 RaytracingAccelerationStructureBuildFlags
																			 .None ) {
		Unsafe.SkipInit( out BuildRaytracingAccelerationStructureInputs inputs ) ;
		inputs.Description.InstanceDescs = instanceDescriptions ;
		inputs.Type                      = Type ;
		inputs.Flags                     = Flags ;
		inputs.DescsLayout               = layout ;
		inputs.NumDescs                  = 1U ;
		//! TODO: Figure out how to handle this
		return inputs ;
	}
} ;


/// <summary>
/// Describes a set of geometry that is used in the D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_INPUTS
/// structure to provide input data to a raytracing acceleration structure build operation.
/// </summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_geometry_desc">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_RAYTRACING_GEOMETRY_DESC ) )]
public partial struct RaytracingGeometryDescription {
	/// <summary>The type of geometry.</summary>
	public RaytracingGeometryType Type ;

	/// <summary>The geometry flags</summary>
	public RaytracingGeometryFlags Flags ;

	public _descriptionUnion Description ;

	[StructLayout( LayoutKind.Explicit )]
	public partial struct _descriptionUnion {
		[FieldOffset( 0 )] public RaytracingGeometryTrianglesDescription Triangles ;
		[FieldOffset( 0 )] public RaytracingGeometryAABBsDescription     AABBs ;

		public _descriptionUnion( RaytracingGeometryTrianglesDescription triangles ) {
			Unsafe.SkipInit( out this ) ;
			Triangles = triangles ;
		}

		public _descriptionUnion( RaytracingGeometryAABBsDescription aabbs ) {
			Unsafe.SkipInit( out this ) ;
			AABBs = aabbs ;
		}

		public static implicit operator _descriptionUnion( RaytracingGeometryTrianglesDescription triangles ) =>
			new( triangles ) ;

		public static implicit operator _descriptionUnion( RaytracingGeometryAABBsDescription aabbs ) => new( aabbs ) ;
	}
	
	
	public RaytracingGeometryDescription( RaytracingGeometryTrianglesDescription triangles, 
										  RaytracingGeometryFlags flags = RaytracingGeometryFlags.None ) {
		Type        = RaytracingGeometryType.Triangles ;
		Flags       = flags ;
		Description = new( triangles ) ;
	}
	
	public RaytracingGeometryDescription( RaytracingGeometryAABBsDescription aabbs, 
										  RaytracingGeometryFlags flags = RaytracingGeometryFlags.None ) {
		Type        = RaytracingGeometryType.ProceduralPrimitiveAABBs ;
		Flags       = flags ;
		Description = new( aabbs ) ;
	}
} ;


/// <summary>Describes a set of triangles used as raytracing geometry. The geometry pointed to by this struct are always in triangle list form, indexed or non-indexed. Triangle strips are not supported.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_geometry_triangles_desc">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_RAYTRACING_GEOMETRY_TRIANGLES_DESC ) )]
public partial struct RaytracingGeometryTrianglesDescription {
	/// <summary>
	/// <para>Address of a 3x4 affine transform matrix in row-major layout to be applied to the vertices in the <i>VertexBuffer</i> during an acceleration structure build.  The contents of <i>VertexBuffer</i> are not modified.  If a 2D vertex format is used, the transformation is applied with the third vertex component assumed to be zero. If <i>Transform3x4</i> is NULL the vertices will not be transformed. Using <i>Transform3x4</i> may result in increased computation and/or memory requirements for the acceleration structure build.</para>
	/// <para>The memory pointed to must be in state <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE</a>.  The address must be aligned to 16 bytes, defined as <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_TRANSFORM3X4_BYTE_ALIGNMENT</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_geometry_triangles_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong Transform3x4 ;

	/// <summary>
	/// <para>Format of the indices in the <i>IndexBuffer</i>.  Must be one of the following: </para>
	/// Format of the indices in the IndexBuffer. Must be one of the following:<para/>
	/// <b><see cref="Format.UNKNOWN"/></b> - when IndexBuffer is NULL<para/>
	/// <b><see cref="Format.R32_UINT"/></b><para/>
	/// <b><see cref="Format.R16_UINT"/></b><para/>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_geometry_triangles_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public Format IndexFormat ;

	/// <summary>
	/// <para>Format of the vertices in <i>VertexBuffer</i>.  Must be one of the following: </para><para/>
	/// <b><see cref="Format.R32G32_FLOAT"/></b> - third component is assumed 0<para/>
	/// <b><see cref="Format.R32G32B32_FLOAT"/></b><para/>
	/// <b><see cref="Format.R16G16_FLOAT"/></b> - third component is assumed 0<para/>
	/// <b><see cref="Format.R16G16B16A16_FLOAT"/></b> - A16 component is ignored, other data can be packed there, such as setting vertex stride to 6 bytes.<para/>
	/// <b><see cref="Format.R16G16_SNORM"/></b> - third component is assumed 0.<para/>
	/// <b><see cref="Format.R16G16B16A16_SNORM"/></b> - A16 component is ignored, other data can be packed there, such as setting vertex stride to 6 bytes.<para/>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_geometry_triangles_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public Format VertexFormat ;

	/// <summary>Number of indices in <i>IndexBuffer</i>.  Must be 0 if <i>IndexBuffer</i> is NULL.</summary>
	public uint IndexCount ;

	/// <summary>Number of vertices in <i>VertexBuffer</i>.</summary>
	public uint VertexCount ;

	/// <summary>
	/// <para>Array of vertex indices.  If NULL, triangles are non-indexed.  Just as with graphics, the address must be aligned to the size of <i>IndexFormat</i>.
	/// The memory pointed to must be in state <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE</a>.
	/// Note that if an app wants to share index buffer inputs between graphics input assembler and raytracing acceleration structure build input, it can always put a resource into a
	/// combination of read states simultaneously, e.g. <b><see cref="ResourceStates.IndexBuffer"/></b> | <b><see cref="ResourceStates.NonPixelShaderResource"/></b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_geometry_triangles_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong IndexBuffer ;

	/// <summary>
	/// <para>Array of vertices including a stride.  The alignment on the address and stride must be a multiple of the component size, so 4 bytes for formats with 32bit components and 2 bytes for formats with 16bit components.  Unlike graphics, there is no constraint on the stride, other than that the bottom 32bits of the value are all that are used – the field is UINT64 purely to make neighboring fields align cleanly/obviously everywhere.  Each vertex position is expected to be at the start address of the stride range and any excess space is ignored by acceleration structure builds.  This excess space might contain other app data such as vertex attributes, which the app is responsible for manually fetching in shaders, whether it is interleaved in vertex buffers or elsewhere. The memory pointed to must be in state <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE</a>.  Note that if an app wants to share vertex buffer inputs between graphics input assembler and raytracing acceleration structure build input, it can always put a resource into a combination of read states simultaneously, e.g. <b>D3D12_RESOURCE_STATE_VERTEX_AND_CONSTANT_BUFFER</b> | <b>D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE</b></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_geometry_triangles_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public GPUVirtualAddressAndStride VertexBuffer ;
	
	
	public RaytracingGeometryTrianglesDescription( ulong transform3X4 = 0U,
												   Format indexFormat  = Format.UNKNOWN,
												   Format vertexFormat = Format.UNKNOWN,
												   uint indexCount   = 0U,
												   uint vertexCount  = 0U,
												   ulong indexBuffer  = 0U,
												   GPUVirtualAddressAndStride vertexBuffer = default ) {
		Transform3x4 = transform3X4 ;
		IndexFormat  = indexFormat ;
		VertexFormat = vertexFormat ;
		IndexCount   = indexCount ;
		VertexCount  = vertexCount ;
		IndexBuffer  = indexBuffer ;
		VertexBuffer = vertexBuffer ;
	}
} ;


/// <summary>Represents a GPU virtual address and indexing stride.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_gpu_virtual_address_and_stride">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_GPU_VIRTUAL_ADDRESS_AND_STRIDE))]
public struct GPUVirtualAddressAndStride {
	/// <summary>The beginning of the virtual address range.</summary>
	public ulong StartAddress ;

	/// <summary>Defines indexing stride, such as for vertices.  Only the bottom 32 bits are used.  The field is 64 bits to make alignment of containing structures consistent everywhere.</summary>
	public ulong StrideInBytes ;
	
	public GPUVirtualAddressAndStride( ulong startAddress, ulong strideInBytes ) {
		StartAddress  = startAddress ;
		StrideInBytes = strideInBytes ;
	}
	
	public GPUVirtualAddressAndStride( ulong startAddress, uint strideInBytes ): 
		this( startAddress, (ulong)strideInBytes ) { }
	
	public static implicit operator GPUVirtualAddressAndStride( in (ulong startAddress, ulong strideInBytes) tuple ) =>
		new( tuple.startAddress, tuple.strideInBytes ) ;
} ;


/// <summary>
/// Describes a set of Axis-aligned bounding boxes that are used in the D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_INPUTS
/// structure to provide input data to a raytracing acceleration structure build operation.
/// </summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_geometry_aabbs_desc">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_RAYTRACING_GEOMETRY_AABBS_DESC))]
public struct RaytracingGeometryAABBsDescription {
	/// <summary>The number of AABBs pointed to in the contiguous array at <i>AABBs</i>.</summary>
	public ulong AABBCount ;

	/// <summary>
	/// The GPU memory location where an array of AABB descriptions is to be found, including the data stride between AABBs.
	/// The address and stride must each be aligned to 8 bytes, defined as The address must be aligned to 16 bytes, defined as
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants"> D3D12_RAYTRACING_AABB_BYTE_ALIGNMENT</a>.
	/// The stride can be 0.
	/// </summary>
	public GPUVirtualAddressAndStride AABBs ;
	
	public RaytracingGeometryAABBsDescription( ulong aabbCount, GPUVirtualAddressAndStride aabbs ) {
		AABBCount = aabbCount ; AABBs = aabbs ;
	}
	public RaytracingGeometryAABBsDescription( ulong aabbCount = 0UL, ulong aabbs = 0UL, ulong strideInBytes = 0UL ) {
		AABBCount = aabbCount ;
		AABBs     = new( aabbs, strideInBytes ) ;
	}
	
	public static implicit operator RaytracingGeometryAABBsDescription( in (ulong aabbCount, ulong aabbs, ulong strideInBytes) tuple ) =>
		new( tuple.aabbs, tuple.aabbCount, tuple.aabbs ) ;
	public static implicit operator RaytracingGeometryAABBsDescription( in (ulong aabbCount, GPUVirtualAddressAndStride aabs ) tuple ) =>
		new( tuple.aabs.StartAddress, tuple.aabbCount, tuple.aabs.StrideInBytes ) ;
} ;


/// <summary>Description of the post-build information to generate from an acceleration structure. Use this structure in calls to EmitRaytracingAccelerationStructurePostbuildInfo and BuildRaytracingAccelerationStructure.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_acceleration_structure_postbuild_info_desc">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_RAYTRACING_ACCELERATION_STRUCTURE_POSTBUILD_INFO_DESC ) )]
public partial struct RaytracingAccelerationStructurePostBuildInfoDescription {
	/// <summary>
	/// <para>Storage for the post-build info result.  Size required and the layout of the contents written by the system depend on the value of the <i>InfoType</i> field. The memory pointed to must be in state <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_UNORDERED_ACCESS</a>.  The memory must be aligned to the natural alignment for the members of the particular output structure being generated (e.g. 8 bytes for a struct with the largest members being UINT64).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_acceleration_structure_postbuild_info_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong DestBuffer ;

	/// <summary>A <see cref="RaytracingAccelerationStructurePostbuildInfoType"/> value specifying the type of post-build information to retrieve.</summary>
	public RaytracingAccelerationStructurePostbuildInfoType InfoType ;

	public RaytracingAccelerationStructurePostBuildInfoDescription( ulong destBuffer,
																	RaytracingAccelerationStructurePostbuildInfoType infoType ) {
		DestBuffer = destBuffer ; InfoType   = infoType ;
	}

	public static implicit operator RaytracingAccelerationStructurePostBuildInfoDescription(
		in (ulong destBuffer, RaytracingAccelerationStructurePostbuildInfoType infoType) tuple ) =>
			new( tuple.destBuffer, tuple.infoType ) ;
} ;


/// <summary>Describes the properties of a ray dispatch operation initiated with a call to ID3D12GraphicsCommandList4::DispatchRays.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_dispatch_rays_desc">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_DISPATCH_RAYS_DESC ) )]
public partial struct DispatchRaysDescription {
	/// <summary>
	/// <para>The shader record for the ray generation shader to use. The memory pointed to must be in state <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE</a>. The address must be aligned to 64 bytes, defined as <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_SHADER_TABLE_BYTE_ALIGNMENT</a>, and in the range [0...4096] bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_dispatch_rays_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public GPUVirtualAddressRange RayGenerationShaderRecord ;

	/// <summary>
	/// <para>The shader table for miss shaders. The stride is record stride, and must be aligned to 32 bytes, defined as <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_SHADER_RECORD_BYTE_ALIGNMENT</a>, and in the range [0...4096] bytes. 0 is allowed. The memory pointed to must be in state <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE</a>. The address must be aligned to 64 bytes, defined as <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_SHADER_TABLE_BYTE_ALIGNMENT</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_dispatch_rays_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public GPUVirtualAddressRangeAndStride MissShaderTable ;

	/// <summary>
	/// <para>The shader table for hit groups. The stride is record stride, and must be aligned to 32 bytes, defined as <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_SHADER_RECORD_BYTE_ALIGNMENT</a>, and in the range [0...4096] bytes. 0 is allowed. The memory pointed to must be in state <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE</a>. The address must be aligned to 64 bytes, defined as <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_SHADER_TABLE_BYTE_ALIGNMENT</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_dispatch_rays_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public GPUVirtualAddressRangeAndStride HitGroupTable ;

	/// <summary>
	/// <para>The shader table for callable shaders. The stride is record stride, and must be aligned to 32 bytes, defined as <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_SHADER_RECORD_BYTE_ALIGNMENT</a>. 0 is allowed. The memory pointed to must be in state <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE</a>. The address must be aligned to 64 bytes, defined as <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_RAYTRACING_SHADER_TABLE_BYTE_ALIGNMENT</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_dispatch_rays_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public GPUVirtualAddressRangeAndStride CallableShaderTable ;

	/// <summary>The width of the generation shader thread grid.</summary>
	public uint Width ;

	/// <summary>The height of the generation shader thread grid.</summary>
	public uint Height ;

	/// <summary>The depth of the generation shader thread grid.</summary>
	public uint Depth ;
	
	
	public DispatchRaysDescription( GPUVirtualAddressRange rayGenerationShaderRecord,
									GPUVirtualAddressRangeAndStride missShaderTable,
									GPUVirtualAddressRangeAndStride hitGroupTable,
									GPUVirtualAddressRangeAndStride callableShaderTable,
									uint width, uint height, uint depth ) {
		RayGenerationShaderRecord = rayGenerationShaderRecord ;
		MissShaderTable           = missShaderTable ;
		HitGroupTable             = hitGroupTable ;
		CallableShaderTable       = callableShaderTable ;
		Width                     = width ;
		Height                    = height ;
		Depth                     = depth ;
	}
} ;


/// <summary>Represents a GPU virtual address range.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_gpu_virtual_address_range">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_GPU_VIRTUAL_ADDRESS_RANGE))]
public partial struct GPUVirtualAddressRange {
	/// <summary>The beginning of the virtual address range.</summary>
	public ulong StartAddress ;

	/// <summary>The size of the virtual address range, in bytes.</summary>
	public ulong SizeInBytes ;
	
	
	public GPUVirtualAddressRange( ulong startAddress = 0UL, ulong sizeInBytes = 0UL ) {
		StartAddress = startAddress ;
		SizeInBytes  = sizeInBytes ;
	}
	
	public static implicit operator GPUVirtualAddressRange( in (ulong startAddress, ulong sizeInBytes) tuple ) =>
		new( tuple.startAddress, tuple.sizeInBytes ) ;
} ;


/// <summary>Represents a GPU virtual address range and stride.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_gpu_virtual_address_range_and_stride">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_GPU_VIRTUAL_ADDRESS_RANGE_AND_STRIDE))]
public partial struct GPUVirtualAddressRangeAndStride {
	/// <summary>The beginning of the virtual address range.</summary>
	public ulong StartAddress ;

	/// <summary>The size of the virtual address range, in bytes.</summary>
	public ulong SizeInBytes ;

	/// <summary>Defines the record-indexing stride within the memory range.</summary>
	public ulong StrideInBytes ;
	
	
	public GPUVirtualAddressRangeAndStride( ulong startAddress = 0UL, 
											ulong sizeInBytes = 0UL, 
											ulong strideInBytes = 0UL ) {
		StartAddress  = startAddress ;
		SizeInBytes   = sizeInBytes ;
		StrideInBytes = strideInBytes ;
	}
	
	public static implicit operator GPUVirtualAddressRangeAndStride( in (ulong startAddress, ulong sizeInBytes, ulong strideInBytes) tuple ) =>
		new( tuple.startAddress, tuple.sizeInBytes, tuple.strideInBytes ) ;
} ;


/// <summary>
/// Represents prebuild information about a raytracing acceleration structure.
/// Get an instance of this structure by calling GetRaytracingAccelerationStructurePrebuildInfo.
/// </summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_raytracing_acceleration_structure_prebuild_info">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_RAYTRACING_ACCELERATION_STRUCTURE_PREBUILD_INFO))]
public partial struct RaytracingAccelerationStructurePreBuildInfo {
	/// <summary>Size required to hold the result of an acceleration structure build based on the specified inputs.</summary>
	public ulong ResultDataMaxSizeInBytes ;

	/// <summary>Scratch storage on the GPU required during acceleration structure build based on the specified inputs.</summary>
	public ulong ScratchDataSizeInBytes ;
	
	public ulong UpdateScratchDataSizeInBytes ;
	
	
	public RaytracingAccelerationStructurePreBuildInfo( ulong resultDataMaxSizeInBytes     = 0UL, 
														ulong scratchDataSizeInBytes       = 0UL, 
														ulong updateScratchDataSizeInBytes = 0UL ) {
		ResultDataMaxSizeInBytes   = resultDataMaxSizeInBytes ;
		ScratchDataSizeInBytes     = scratchDataSizeInBytes ;
		UpdateScratchDataSizeInBytes = updateScratchDataSizeInBytes ;
	}
	
	public static implicit operator RaytracingAccelerationStructurePreBuildInfo( in (ulong resultDataMaxSizeInBytes, ulong scratchDataSizeInBytes, ulong updateScratchDataSizeInBytes) tuple ) =>
		new( tuple.resultDataMaxSizeInBytes, tuple.scratchDataSizeInBytes, tuple.updateScratchDataSizeInBytes ) ;
} ;