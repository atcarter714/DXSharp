#region Using Directives

using System.Buffers ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp ;
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
public struct FeatureDataArchitecture {
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
public struct FeatureDataArchitecture1 {
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
} ;



[EquivalentOf( typeof( D3D12_RANGE_UINT64 ) )]
public struct RangeUInt64 {
	/// <summary>The offset, in bytes, denoting the beginning of a memory range.</summary>
	public ulong Begin ;

	/// <summary>
	/// <para>The offset, in bytes, denoting the end of a memory range. <b>End</b> is one-past-the-end.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_range_uint64#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong End ;
	
	public RangeUInt64( ulong begin = 0UL, ulong end = 0UL ) {
		Begin = begin ; End = end ;
	}
	
	public static implicit operator RangeUInt64( (ulong begin, ulong end) range ) => new( range.begin, range.end ) ;
	public static implicit operator (ulong begin, ulong end)( RangeUInt64 range ) => ( range.Begin, range.End ) ;
} ;


[EquivalentOf(typeof(D3D12_SUBRESOURCE_RANGE_UINT64))]
public struct SubresourceRangeUInt64 {
	/// <summary>The index of the subresource.</summary>
	public uint Subresource ;
	
	/// <summary>A memory range within the subresource.</summary>
	public RangeUInt64 Range ;
	
	public SubresourceRangeUInt64( uint subresource = 0U, RangeUInt64 range = default ) {
		Subresource = subresource ; Range = range ;
	}
	
	public static implicit operator SubresourceRangeUInt64( (uint subresource, RangeUInt64 range) range ) => new( range.subresource, range.range ) ;
	public static implicit operator (uint subresource, RangeUInt64 range)( SubresourceRangeUInt64 range ) => ( range.Subresource, range.Range ) ;
} ;


[EquivalentOf( typeof( D3D12_SAMPLE_POSITION ) )]
public struct SamplePosition {
	/// <summary>A signed sub-pixel coordinate value in the X axis.</summary>
	public sbyte X ;
	/// <summary>A signed sub-pixel coordinate value in the Y axis.</summary>
	public sbyte Y ;
	
	public SamplePosition( sbyte x = 0, sbyte y = 0 ) {
		X = x ; Y = y ;
	}
	
	public static implicit operator SamplePosition( (sbyte x, sbyte y) pos ) => new( pos.x, pos.y ) ;
	public static implicit operator (sbyte x, sbyte y)( SamplePosition pos ) => ( pos.X, pos.Y ) ;
	
	public static explicit operator SamplePosition( Vector2 pos ) => new( (sbyte)pos.X, (sbyte)pos.Y ) ;
	public static explicit operator Vector2( SamplePosition pos ) => new( pos.X, pos.Y ) ;
} ;



[EquivalentOf(typeof(D3D12_WRITEBUFFERIMMEDIATE_PARAMETER))]
public struct WriteBufferImmediateParameter {
	/// <summary>The GPU virtual address at which to write the value. The address must be aligned to a 32-bit (4-byte) boundary.</summary>
	public ulong Dest ;

	/// <summary>The 32-bit value to write.</summary>
	public uint Value ;
	
	public WriteBufferImmediateParameter( ulong dest = 0UL, uint value = 0U ) {
		Dest = dest ; Value = value ;
	}
	
	public static implicit operator WriteBufferImmediateParameter( (ulong dest, uint value) param ) => new( param.dest, param.value ) ;
	public static implicit operator (ulong dest, uint value)( WriteBufferImmediateParameter param ) => ( param.Dest, param.Value ) ;
} ;


/// <summary>Describes flags for a protected resource session, per adapter.</summary>
[EquivalentOf( typeof( D3D12_PROTECTED_RESOURCE_SESSION_DESC ) )]
public struct ProtectedResourceSessionDescription {
	/// <summary>
	/// The node mask. For single GPU operation, set this to zero. If there are multiple GPU nodes, then set a bit to identify the node (the device's physical adapter)
	/// to which the protected session applies. Each bit in the mask corresponds to a single node. Only 1 bit may be set.<para/>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_protected_resource_session_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint NodeMask;

	/// <summary>
	/// <para>Type: **[D3D12_PROTECTED_RESOURCE_SESSION_FLAGS](./ne-d3d12-d3d12_protected_resource_session_flags.md)** Specifies the supported crypto sessions options.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_protected_resource_session_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ProtectedResourceSessionFlags Flags ;
	
	public ProtectedResourceSessionDescription( uint nodeMask = 0U, ProtectedResourceSessionFlags flags = ProtectedResourceSessionFlags.None ) {
		NodeMask = nodeMask ; Flags = flags ;
	}
} ;


/// <summary>Describes flags and protection type for a protected resource session, per adapter.</summary>
[EquivalentOf( typeof( D3D12_PROTECTED_RESOURCE_SESSION_DESC1 ) )]
public struct ProtectedResourceSessionDescription1 {
	/// <summary>
	/// <para>The node mask. For single GPU operation, set this to zero. If there are multiple GPU nodes, then set a bit to identify the node
	/// (the device's physical adapter) to which the protected session applies. Each bit in the mask corresponds to a single node. Only 1 bit may be set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_protected_resource_session_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint NodeMask ;

	/// <summary>
	/// <para>Specifies the supported crypto sessions options.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_protected_resource_session_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ProtectedResourceSessionFlags Flags ;

	/// <summary>
	/// <para>The GUID that represents the protection type. Microsoft defines <b>D3D12_PROTECTED_RESOURCES_SESSION_HARDWARE_PROTECTED</b>.
	/// Using the <b>D3D12_PROTECTED_RESOURCES_SESSION_HARDWARE_PROTECTED</b> GUID is equivalent to calling ID3D12Device4::CreateProtectedResourceSession.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_protected_resource_session_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public Guid ProtectionType ;

	
	public ProtectedResourceSessionDescription1( uint nodeMask = 0U, 
												 ProtectedResourceSessionFlags flags 
													 = ProtectedResourceSessionFlags.None, 
												 Guid protectionType = default ) {
		NodeMask = nodeMask ; Flags = flags ; ProtectionType = protectionType ;
	}
} ;

[EquivalentOf( typeof( D3D12_RENDER_PASS_RENDER_TARGET_DESC ) )]
public struct RenderPassRenderTargetDescription {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">D3D12_CPU_DESCRIPTOR_HANDLE</a>. The CPU descriptor handle corresponding to the render target view(s) (RTVs).</summary>
	public CPUDescriptorHandle cpuDescriptor ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_pass_beginning_access">D3D12_RENDER_PASS_BEGINNING_ACCESS</a>. The access to the RTV(s) requested at the transition into a render pass.</summary>
	public RenderPassBeginningAccess BeginningAccess ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_pass_ending_access">D3D12_RENDER_PASS_ENDING_ACCESS</a>. The access to the RTV(s) requested at the transition out of a render pass.</summary>
	public RenderPassEndingAccess EndingAccess ;
} ;


/// <summary>Describes the access to resource(s) that is requested by an application at the transition into a render pass.</summary>
/// <remarks>
/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_pass_beginning_access">Learn more about this API from docs.microsoft.com</see>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_RENDER_PASS_BEGINNING_ACCESS))]
public struct RenderPassBeginningAccess {
	/// <summary>A <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_render_pass_beginning_access_type">D3D12_RENDER_PASS_BEGINNING_ACCESS_TYPE</a>. The type of access being requested.</summary>
	public RenderPassBeginningAccessType Type ;

	public _accessUnion Access ;
	
	[StructLayout( LayoutKind.Explicit )]
	public partial struct _accessUnion {
		[FieldOffset( 0 )] public RenderPassBeginningAccessClearParameters Clear ;
		[FieldOffset( 0 )] public RenderPassBeginningAccessPreserveLocalParameters PreserveLocal ;

		public _accessUnion( RenderPassBeginningAccessClearParameters clear ) {
			PreserveLocal = default ;
			Clear         = clear ;
		}
		public _accessUnion( RenderPassBeginningAccessPreserveLocalParameters preserveLocal ) {
			Clear         = default ;
			PreserveLocal = preserveLocal ;
		}
	} ;
	
	public RenderPassBeginningAccess( RenderPassBeginningAccessType type, RenderPassBeginningAccessClearParameters clear ) {
		Type = type ; Access = new( clear ) ;
	}
	public RenderPassBeginningAccess( RenderPassBeginningAccessType type, RenderPassBeginningAccessPreserveLocalParameters preserveLocal ) {
		Type = type ; Access = new( preserveLocal ) ;
	}
	
	
} ;


/// <summary>Describes the clear value to which resource(s) should be cleared at the beginning of a render pass.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_pass_beginning_access_clear_parameters">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_RENDER_PASS_BEGINNING_ACCESS_CLEAR_PARAMETERS))]
public struct RenderPassBeginningAccessClearParameters {
	/// <summary>
	/// A <see cref="ClearValue"/> (equivalent to: <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_clear_value">D3D12_CLEAR_VALUE</a>).
	/// The clear value to which the resource(s) should be cleared.
	/// </summary>
	public ClearValue ClearValue ;
	
	public RenderPassBeginningAccessClearParameters( in ClearValue clearValue = default ) => ClearValue = clearValue ;
	public RenderPassBeginningAccessClearParameters( Format format, ColorF color ) => ClearValue = new( format, color ) ;
	public RenderPassBeginningAccessClearParameters( Format format, DepthStencilValue depthStencil ) => ClearValue = new( format, depthStencil ) ;
	
	public static implicit operator RenderPassBeginningAccessClearParameters( ClearValue clearValue ) => new( clearValue ) ;
	public static implicit operator ClearValue( RenderPassBeginningAccessClearParameters clear ) => clear.ClearValue ;
	public static implicit operator RenderPassBeginningAccessClearParameters( (Format format, ColorF color) clear ) => new( clear.format, clear.color ) ;
	public static implicit operator RenderPassBeginningAccessClearParameters( (Format format, DepthStencilValue depthStencil) clear ) => new( clear.format, clear.depthStencil ) ;
} ;


[EquivalentOf( typeof( D3D12_RENDER_PASS_BEGINNING_ACCESS_PRESERVE_LOCAL_PARAMETERS ) )]
public struct RenderPassBeginningAccessPreserveLocalParameters {
	public uint AdditionalWidth ;
	public uint AdditionalHeight ;

	public RenderPassBeginningAccessPreserveLocalParameters( uint additionalWidth = 0U, uint additionalHeight = 0U ) {
		AdditionalWidth  = additionalWidth ;
		AdditionalHeight = additionalHeight ;
	}
	
	public static implicit operator RenderPassBeginningAccessPreserveLocalParameters( 
		( uint additionalWidth, uint additionalHeight ) preserveLocal ) =>
			new( preserveLocal.additionalWidth, preserveLocal.additionalHeight ) ;
} ;


/// <summary>Describes the access to resource(s) that is requested by an application at the transition out of a render pass.</summary>
/// <remarks>
/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_pass_ending_access">Learn more about this API from docs.microsoft.com</see>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_RENDER_PASS_ENDING_ACCESS))]
public partial struct RenderPassEndingAccess {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_render_pass_ending_access_type">D3D12_RENDER_PASS_ENDING_ACCESS_TYPE</a>. The type of access being requested.</summary>
	public RenderPassEndingAccessType Type ;

	public _endAccessUnion Access ;

	[StructLayout( LayoutKind.Explicit )]
	public partial struct _endAccessUnion {
		[FieldOffset( 0 )] public RenderPassEndingAccessResolveParametersUnmanaged Resolve ;
		[FieldOffset( 0 )] public RenderPassBeginningAccessPreserveLocalParameters PreserveLocal ;
		
		public _endAccessUnion( RenderPassEndingAccessResolveParametersUnmanaged resolve ) {
			Unsafe.SkipInit( out this ) ;
			Resolve       = resolve ;
		}
		public _endAccessUnion( RenderPassBeginningAccessPreserveLocalParameters preserveLocal ) {
			Unsafe.SkipInit( out this ) ;
			PreserveLocal = preserveLocal ;
		}
	} ;
	
	public RenderPassEndingAccess( RenderPassEndingAccessType type, RenderPassEndingAccessResolveParametersUnmanaged resolve ) {
		Type = type ; Access = new( resolve ) ;
	}
	public RenderPassEndingAccess( RenderPassEndingAccessType type, RenderPassBeginningAccessPreserveLocalParameters preserveLocal ) {
		Type = type ; Access = new( preserveLocal ) ;
	}
	
	
	public static RenderPassEndingAccess Create( out MemoryHandle? pSubresourceParametersHandle,
												 IResource pSrcResource, 
												 IResource pDstResource, 
												 uint subresourceCount = 0U, 
												 RenderPassEndingAccessResolveSubresourceParameters[ ]? pSubresourceParameters = null, 
												 Format format = Format.UNKNOWN, 
												 ResolveMode resolveMode = default, 
												 bool preserveResolveSource = default ) {
		unsafe {
			pSubresourceParametersHandle = default ;
			if ( pSubresourceParameters is not null ) {
				Memory< RenderPassEndingAccessResolveSubresourceParameters > mem = new( pSubresourceParameters ) ;
				pSubresourceParametersHandle = mem.Pin( ) ;
			}
			else pSubresourceParametersHandle = null ;
			
			RenderPassEndingAccessResolveSubresourceParameters* pSubresourceParametersPtr = pSubresourceParametersHandle is null ? null :
				(RenderPassEndingAccessResolveSubresourceParameters *)pSubresourceParametersHandle.Value.Pointer ;
			
			var resolve =
				new RenderPassEndingAccessResolveParametersUnmanaged( (ResourceUnmanaged*)pSrcResource?.ComPointer?.InterfaceVPtr,
																	  (ResourceUnmanaged*)pDstResource?.ComPointer?.InterfaceVPtr,
																	  subresourceCount, 
																	  pSubresourceParametersPtr,
																	  format, resolveMode, 
																	  preserveResolveSource ) ;
			return new( RenderPassEndingAccessType.Resolve, resolve ) ;
		}
	}
	
	
	public static unsafe RenderPassEndingAccess Create( ResourceUnmanaged* pSrcResource = null,
														ResourceUnmanaged* pDstResource = null,
														uint subresourceCount = 0U,
														RenderPassEndingAccessResolveSubresourceParameters*
															pSubresourceParameters = null,
														Format format = Format.UNKNOWN,
														ResolveMode resolveMode = default,
														bool preserveResolveSource = default ) {
		var resolve = new RenderPassEndingAccessResolveParametersUnmanaged( pSrcResource, pDstResource, 
																			subresourceCount, pSubresourceParameters, 
																			format, resolveMode, preserveResolveSource ) ;
		return new( RenderPassEndingAccessType.Resolve, resolve ) ;
	}
} ;


[EquivalentOf(typeof(D3D12_RENDER_PASS_ENDING_ACCESS_RESOLVE_PARAMETERS_unmanaged))]
public unsafe partial struct RenderPassEndingAccessResolveParametersUnmanaged {
	public ResourceUnmanaged* pSrcResource ;
	public ResourceUnmanaged* pDstResource ;
	public uint SubresourceCount ;

	/// <summary>
	/// A pointer to a constant array of <see cref="RenderPassEndingAccessResolveSubresourceParameters"/>.
	/// These subresources can be a subset of the render target's array slices, but you can't target
	/// subresources that aren't part of the render target view (RTV) or the depth/stencil view (DSV).
	/// </summary>
	/// <remarks>
	/// <b>Note:</b><para/>
	/// This pointer is directly referenced by the command list, and the memory for this array must remain alive and intact until <see cref="IGraphicsCommandList4.EndRenderPass"/> is called.
	/// </remarks>
	public RenderPassEndingAccessResolveSubresourceParameters* pSubresourceParameters ;
	
	public Span< RenderPassEndingAccessResolveSubresourceParameters > SubresourceParameters => 
		new( pSubresourceParameters, (int)SubresourceCount ) ;
	

	public Format Format ;
	public ResolveMode ResolveMode ;
	public BOOL PreserveResolveSource ;
	
	
	public RenderPassEndingAccessResolveParametersUnmanaged( ResourceUnmanaged* pSrcResource = null, 
															 ResourceUnmanaged* pDstResource = null, 
															 uint subresourceCount = 0U, 
															 RenderPassEndingAccessResolveSubresourceParameters* pSubresourceParameters = null, 
															 Format format = Format.UNKNOWN,
															 ResolveMode resolveMode = default,
															 bool preserveResolveSource = default ) {
		this.pSrcResource = pSrcResource ; this.pDstResource = pDstResource ; 
		SubresourceCount = subresourceCount ; this.pSubresourceParameters = pSubresourceParameters ; 
		Format = format ; ResolveMode = resolveMode ; PreserveResolveSource = preserveResolveSource ;
	}
	
	public RenderPassEndingAccessResolveParametersUnmanaged( out MemoryHandle? pSubresourceParametersHandle,
															 IResource pSrcResource,
															 IResource pDstResource,
															 uint subresourceCount = 0U,
															 RenderPassEndingAccessResolveSubresourceParameters[ ]?
																 pSubresourceParameters = default,
															 Format format = Format.UNKNOWN,
															 ResolveMode resolveMode = default,
															 bool preserveResolveSource = default ) {
		this.pSrcResource = (ResourceUnmanaged *)pSrcResource?.ComPointer?.InterfaceVPtr ;
		this.pDstResource = (ResourceUnmanaged *)pDstResource?.ComPointer?.InterfaceVPtr ;
		SubresourceCount = subresourceCount ; this.pSubresourceParameters = null ;
		Format = format ; ResolveMode = resolveMode ; PreserveResolveSource = preserveResolveSource ;
		
		if( pSubresourceParameters is not null ) {
			Memory< RenderPassEndingAccessResolveSubresourceParameters > mem = new( pSubresourceParameters ) ;
			pSubresourceParametersHandle = mem.Pin( ) ;
			this.pSubresourceParameters = (RenderPassEndingAccessResolveSubresourceParameters *)
				pSubresourceParametersHandle.Value.Pointer ;
		}
		else
			pSubresourceParametersHandle = null ;
	}
}


/// <summary>Describes the subresources involved in resolving at the conclusion of a render pass.</summary>
/// <remarks>
/// <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_pass_ending_access_resolve_subresource_parameters">
/// Learn more about this API from docs.microsoft.com</a>.
/// </remarks>
[EquivalentOf( typeof( D3D12_RENDER_PASS_ENDING_ACCESS_RESOLVE_SUBRESOURCE_PARAMETERS ) )]
public partial struct RenderPassEndingAccessResolveSubresourceParameters {
	/// <summary>A <b>UINT</b>. The source subresource.</summary>
	public uint SrcSubresource ;

	/// <summary>A <b>UINT</b>. The destination subresource.</summary>
	public uint DstSubresource ;

	/// <summary>A <b>UINT</b>. The x coordinate within the destination subresource.</summary>
	public uint DstX ;

	/// <summary>A <b>UINT</b>. The y coordinate within the destination subresource.</summary>
	public uint DstY ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-rect">D3D12_RECT</a>. The rectangle within the source subresource.</summary>
	public Rect SrcRect ;

	
	public RenderPassEndingAccessResolveSubresourceParameters( uint srcSubresource = 0U, 
															   uint dstSubresource = 0U, 
															   uint dstX = 0U, uint dstY = 0U, 
															   Rect srcRect = default ) {
		SrcSubresource = srcSubresource ; DstSubresource = dstSubresource ; 
		DstX = dstX ; DstY = dstY ; SrcRect = srcRect ;
	}
} ;


[EquivalentOf( typeof( D3D12_RENDER_PASS_ENDING_ACCESS_PRESERVE_LOCAL_PARAMETERS ) )]
public partial struct RenderPassEndingAccessPreserveLocalParameters {
	public uint AdditionalWidth ;
	public uint AdditionalHeight ;
	
	public RenderPassEndingAccessPreserveLocalParameters( uint additionalWidth = 0U, uint additionalHeight = 0U ) {
		AdditionalWidth = additionalWidth ; AdditionalHeight = additionalHeight ;
	}
} ;


[EquivalentOf(typeof(D3D12_RENDER_PASS_DEPTH_STENCIL_DESC))]
public partial struct RenderPassDepthStencilDescription {
	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">D3D12_CPU_DESCRIPTOR_HANDLE</a>.
	/// The CPU descriptor handle corresponding to the depth stencil view (DSV).
	/// </summary>
	public CPUDescriptorHandle cpuDescriptor;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_pass_beginning_access">D3D12_RENDER_PASS_BEGINNING_ACCESS</a>.
	/// The access to the depth buffer requested at the transition into a render pass.
	/// </summary>
	public RenderPassBeginningAccess DepthBeginningAccess ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_pass_beginning_access">D3D12_RENDER_PASS_BEGINNING_ACCESS</a>.
	/// The access to the stencil buffer requested at the transition into a render pass.
	/// </summary>
	public RenderPassBeginningAccess StencilBeginningAccess ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_pass_ending_access">D3D12_RENDER_PASS_ENDING_ACCESS</a>.
	/// The access to the depth buffer requested at the transition out of a render pass.
	/// </summary>
	public RenderPassEndingAccess DepthEndingAccess ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_render_pass_ending_access">D3D12_RENDER_PASS_ENDING_ACCESS</a>.
	/// The access to the stencil buffer requested at the transition out of a render pass.
	/// </summary>
	public RenderPassEndingAccess StencilEndingAccess ;
	
	
	public RenderPassDepthStencilDescription( CPUDescriptorHandle cpuDescriptor = default, 
											  RenderPassBeginningAccess depthBeginningAccess   = default, 
											  RenderPassBeginningAccess stencilBeginningAccess = default, 
											  RenderPassEndingAccess    depthEndingAccess      = default, 
											  RenderPassEndingAccess    stencilEndingAccess    = default ) {
		this.cpuDescriptor = cpuDescriptor ;
		DepthBeginningAccess = depthBeginningAccess ;
		StencilBeginningAccess = stencilBeginningAccess ;
		DepthEndingAccess = depthEndingAccess ;
		StencilEndingAccess = stencilEndingAccess ;
	}
} ;


[EquivalentOf( typeof( D3D12_BARRIER_GROUP ) )]
public partial struct BarrierGroup {
	/// <summary>The type of barriers in the group.</summary>
	public BarrierType Type ;

	/// <summary>The number of barriers in the group.</summary>
	public uint NumBarriers ;

	public _barrierUnion Barrier ;
	[StructLayout( LayoutKind.Explicit )]
	public unsafe partial struct _barrierUnion {
		[FieldOffset( 0 )] public unsafe GlobalBarrier*           pGlobalBarriers ;
		[FieldOffset( 0 )] public unsafe TextureBarrierUnmanaged* pTextureBarriers ;
		[FieldOffset( 0 )] public unsafe BufferBarrierUnmanaged*  pBufferBarriers ;
		
		public _barrierUnion( GlobalBarrier* pGlobalBarriers ) {
			Unsafe.SkipInit( out this ) ;
			this.pGlobalBarriers = pGlobalBarriers ;
		}
		public _barrierUnion( TextureBarrierUnmanaged* pTextureBarriers ) {
			Unsafe.SkipInit( out this ) ;
			this.pTextureBarriers = pTextureBarriers ;
		}
		public _barrierUnion( BufferBarrierUnmanaged* pBufferBarriers ) {
			Unsafe.SkipInit( out this ) ;
			this.pBufferBarriers = pBufferBarriers ;
		}
		
		public static implicit operator _barrierUnion( GlobalBarrier* pGlobalBarriers ) => new( pGlobalBarriers ) ;
		public static implicit operator _barrierUnion( TextureBarrierUnmanaged* pTextureBarriers ) => new( pTextureBarriers ) ;
		public static implicit operator _barrierUnion( BufferBarrierUnmanaged* pBufferBarriers ) => new( pBufferBarriers ) ;
	}
	
	public unsafe BarrierGroup( uint numBarriers, GlobalBarrier* pGlobalBarriers ) {
		Type = BarrierType.Global ; NumBarriers = numBarriers ; 
		Barrier = pGlobalBarriers ;
	}
	public unsafe BarrierGroup( uint numBarriers, TextureBarrierUnmanaged* pTextureBarriers ) {
		Type = BarrierType.Texture ; NumBarriers = numBarriers ; 
		Barrier = pTextureBarriers ;
	}
	public unsafe BarrierGroup( uint numBarriers, BufferBarrierUnmanaged* pBufferBarriers ) {
		Type = BarrierType.Buffer ; NumBarriers = numBarriers ; 
		Barrier = pBufferBarriers ;
	}
} ;


/// <summary>
/// Describes a resource memory access barrier. Used by global, texture, and buffer barriers
/// to indicate when resource memory must be made visible for a specific access type.
/// </summary>
[EquivalentOf( typeof( D3D12_GLOBAL_BARRIER ) )]
public partial struct GlobalBarrier {
	/// <summary>Synchronization scope of all preceding GPU work that must be completed before executing the barrier.</summary>
	public BarrierSync SyncBefore ;

	/// <summary>Synchronization scope of all subsequent GPU work that must wait until the barrier execution is finished.</summary>
	public BarrierSync SyncAfter ;

	/// <summary>Access bits corresponding with any relevant resource usage since the preceding barrier or the start of **ExecuteCommandLists** scope.</summary>
	public BarrierAccess AccessBefore ;

	/// <summary>Access bits corresponding with any relevant resource usage after the barrier completes.</summary>
	public BarrierAccess AccessAfter ;


	public GlobalBarrier( BarrierSync   syncBefore   = BarrierSync.None,
						  BarrierSync   syncAfter    = BarrierSync.None,
						  BarrierAccess accessBefore = BarrierAccess.Common,
						  BarrierAccess accessAfter  = BarrierAccess.Common ) {
		SyncBefore   = syncBefore ;
		SyncAfter    = syncAfter ;
		AccessBefore = accessBefore ;
		AccessAfter  = accessAfter ;
	}
} ;

[EquivalentOf( typeof( D3D12_TEXTURE_BARRIER_unmanaged ) )]
public partial struct TextureBarrierUnmanaged {
	public BarrierSync   SyncBefore,   SyncAfter ;
	public BarrierAccess AccessBefore, AccessAfter ;
	public BarrierLayout LayoutBefore, LayoutAfter ;
	public unsafe ResourceUnmanaged*      pResource ;
	public        BarrierSubresourceRange Subresources ;
	public        TextureBarrierFlags     Flags ;
	
	
	public unsafe TextureBarrierUnmanaged( BarrierSync   syncBefore   = BarrierSync.None,
									BarrierSync   syncAfter    = BarrierSync.None,
									BarrierAccess accessBefore = BarrierAccess.Common,
									BarrierAccess accessAfter  = BarrierAccess.Common,
									BarrierLayout layoutBefore = default,
									BarrierLayout layoutAfter  = default,
									ResourceUnmanaged* pResource = null,
									BarrierSubresourceRange subresources = default,
									TextureBarrierFlags flags = TextureBarrierFlags.None ) {
		SyncBefore   = syncBefore ;
		SyncAfter    = syncAfter ;
		AccessBefore = accessBefore ;
		AccessAfter  = accessAfter ;
		LayoutBefore = layoutBefore ;
		LayoutAfter  = layoutAfter ;
		this.pResource = pResource ;
		Subresources = subresources ;
		Flags = flags ;
	}
} ;


[EquivalentOf(typeof(D3D12_BARRIER_SUBRESOURCE_RANGE))]
public partial struct BarrierSubresourceRange {
	/// <summary>
	/// The index of the first mip level in the range; or a subresource index, if <b>NumMipLevels</b> is zero.
	/// If a subresource index, then you can use the value `0xffffffff` to specify all subresources.
	/// </summary>
	public uint IndexOrFirstMipLevel ;

	/// <summary>Number of mip levels in the range, or zero to indicate that <b>IndexOrFirstMipLevel</b> is a subresource index.</summary>
	public uint NumMipLevels ;

	/// <summary>Index of first array slice in the range. Ignored if <b>NumMipLevels</b> is zero.</summary>
	public uint FirstArraySlice ;

	/// <summary>Number of array slices in the range. Ignored if <b>NumMipLevels</b> is zero.</summary>
	public uint NumArraySlices ;

	/// <summary>First plane slice in the range. Ignored if <b>NumMipLevels</b> is zero.</summary>
	public uint FirstPlane ;

	/// <summary>Number of plane slices in the range. Ignored if <b>NumMipLevels</b> is zero.</summary>
	public uint NumPlanes ;
	
	
	public BarrierSubresourceRange( uint indexOrFirstMipLevel = 0U, 
									uint numMipLevels = 0U, 
									uint firstArraySlice = 0U, 
									uint numArraySlices = 0U, 
									uint firstPlane = 0U, 
									uint numPlanes = 0U ) {
		IndexOrFirstMipLevel = indexOrFirstMipLevel ; NumMipLevels = numMipLevels ; 
		FirstArraySlice = firstArraySlice ; NumArraySlices = numArraySlices ; 
		FirstPlane = firstPlane ; NumPlanes = numPlanes ;
	}
} ;


[EquivalentOf(typeof(D3D12_BUFFER_BARRIER_unmanaged))]
public partial struct BufferBarrierUnmanaged {
	public BarrierSync   SyncBefore,   SyncAfter ;
	public BarrierAccess AccessBefore, AccessAfter ;
	public unsafe ResourceUnmanaged* pResource ;
	public ulong Offset ;
	public ulong Size ;
	
	
	public unsafe BufferBarrierUnmanaged( BarrierSync   syncBefore   = BarrierSync.None,
										  BarrierSync   syncAfter    = BarrierSync.None,
										  BarrierAccess accessBefore = BarrierAccess.Common,
										  BarrierAccess accessAfter  = BarrierAccess.Common,
										  ResourceUnmanaged* pResource = null,
										  ulong offset = 0UL,
										  ulong size = 0UL ) {
		SyncBefore   = syncBefore ;
		SyncAfter    = syncAfter ;
		AccessBefore = accessBefore ;
		AccessAfter  = accessAfter ;
		this.pResource = pResource ;
		Offset = offset ;
		Size = size ;
	}
} ;