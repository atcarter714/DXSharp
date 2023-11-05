#pragma warning disable CS8629 // Nullable value type may be null.
#region Using Directives
using System.Buffers ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.DXGI ;
#endregion
namespace DXSharp.Direct3D12 ;


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
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_protected_resource_session_desc#members">Read more on docs.microsoft.com </a>.</para>
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
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_pass_beginning_access">Learn more about this API from docs.microsoft.com </a>.</para>
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
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_render_pass_ending_access">Learn more about this API from docs.microsoft.com </a>.</para>
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

			var src = (Resource)pSrcResource 
#if DEBUG || DEBUG_COM || DEV_BUILD
					  ?? throw new ArgumentNullException( nameof(pSrcResource) )
#endif
				;
			var dst = (Resource)pDstResource
#if DEBUG || DEBUG_COM || DEV_BUILD
					  ?? throw new ArgumentNullException( nameof(pDstResource) )
#endif
				;
			
			var resolve =
				new RenderPassEndingAccessResolveParametersUnmanaged( (ResourceUnmanaged*)( src?.ComPointer?.InterfaceVPtr ),
																	  (ResourceUnmanaged*)( dst?.ComPointer?.InterfaceVPtr ),
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
		Resource src = (Resource)pSrcResource
#if DEBUG || DEBUG_COM || DEV_BUILD
					   ?? throw new ArgumentNullException( nameof(pSrcResource) )
#endif
					   ;
		Resource dst = (Resource)pDstResource 
#if DEBUG || DEBUG_COM || DEV_BUILD
					   ?? throw new ArgumentNullException( nameof(pDstResource) ) 
#endif
					   ;
		
		this.pSrcResource = (ResourceUnmanaged *)( src?.ComPointer?.InterfaceVPtr ) ;
		this.pDstResource = (ResourceUnmanaged *)( dst?.ComPointer?.InterfaceVPtr ) ;
		SubresourceCount  = subresourceCount ; this.pSubresourceParameters = null ;
		Format            = format ; ResolveMode = resolveMode ; PreserveResolveSource = preserveResolveSource ;
		
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


/// <summary>Describes a pipeline state stream.</summary>
/// <remarks>
/// <para>Use this structure with the <see cref="IDevice2.CreatePipelineState"/> method to create pipeline state objects.
/// The format of the provided stream should consist of an alternating set of <see cref="PipelineStateSubObjectType"/>, and the
/// corresponding subobject types for them (for example, <see cref="PipelineStateSubObjectType.Rasterizer"/> pairs with <see cref="RasterizerDescription"/>).
/// In terms of alignment, the D3D12 runtime expects subobjects to be individual struct pairs of enum-struct, rather than a continuous
/// set of fields. It also expects them to be aligned to the natural word alignment of the system. This can be achieved either using
/// `alignas(void*)`, or making a `union` of the enum + subobject and a `void*`.<para/>
/// <b>[IMPORTANT]</b> It isn't sufficient to simply union the <see cref="PipelineStateSubObjectType"/> with a <b>void*</b>, because this will result in certain subobjects being misaligned.
/// 
/// For example, <see cref="PipelineStateSubObjectType.PrimitiveTopology"/> is followed by a <see cref="PrimitiveTopology"/> enum.
/// If the subobject type is unioned with a <b>void*</b>, then there will be additional padding between these 2 members, resulting
/// in corruption of the stream. Because of this, you should union the entire subobject struct with a <b>void*</b>, when `alignas`
/// is not available An example of a suitable subobject for use with <see cref="RasterizerDescription"/> is shown here: </para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_pipeline_state_stream_desc#">Read more on docs.microsoft.com </a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_PIPELINE_STATE_STREAM_DESC))]
public partial struct PipelineStateStreamDescription {
	/// <summary>
	/// <para><a href="https://docs.microsoft.com/cpp/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_</c> Specifies the size of the opaque data structure pointed to by the pPipelineStateSubobjectStream member, in bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_pipeline_state_stream_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public nuint SizeInBytes ;

	/// <summary>
	/// <para><a href="https://docs.microsoft.com/cpp/code-quality/annotating-function-parameters-and-return-values">SAL</a>: <c>_In_reads_(_Inexpressible_("Dependentonsizeofsubobjects"))</c> Specifies the address of a data structure that describes as a bytestream an arbitrary pipeline state subobject.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_pipeline_state_stream_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public nint pPipelineStateSubobjectStream ;
	
	public PipelineStateStreamDescription( nuint sizeInBytes, nint pPipelineStateSubobjectStream ) {
		SizeInBytes = sizeInBytes ; this.pPipelineStateSubobjectStream = pPipelineStateSubobjectStream ;
	}
	
} ;


/// <summary>Describes a portion of a texture for the purpose of texture copies.</summary>
[EquivalentOf( typeof( D3D12_TEXTURE_COPY_LOCATION ) )]
public struct TextureCopyLocation {
	/// <summary>Specifies the resource which will be used for the copy operation.<div> </div>When <b>Type</b> is D3D12_TEXTURE_COPY_TYPE_PLACED_FOOTPRINT, <b>pResource</b> must point to a buffer resource.<div> </div>When <b>Type</b> is D3D12_TEXTURE_COPY_TYPE_SUBRESOURCE_INDEX, <b>pResource</b> must point to a texture resource.</summary>
	public ID3D12Resource pResource ;

	/// <summary>
	/// <para>Specifies which type of resource location this is: a subresource of a texture, or a description of a texture layout which can be applied to a buffer. This <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_texture_copy_type">D3D12_TEXTURE_COPY_TYPE</a> enum indicates which union member to use.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_texture_copy_location#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public TextureCopyType Type ;

	public _locationUnion Location ;
	[StructLayout( LayoutKind.Explicit )]
	public partial struct _locationUnion {
		[FieldOffset( 0 )] public PlacedSubresourceFootprint PlacedFootprint ;
		[FieldOffset( 0 )] public uint                       SubresourceIndex ;

		public _locationUnion( PlacedSubresourceFootprint placedFootprint ) {
			Unsafe.SkipInit( out this ) ;
			PlacedFootprint = placedFootprint ;
		}

		public _locationUnion( uint subresourceIndex ) {
			Unsafe.SkipInit( out this ) ;
			SubresourceIndex = subresourceIndex ;
		}

		public static implicit operator _locationUnion( PlacedSubresourceFootprint placedFootprint ) =>
			new( placedFootprint ) ;

		public static implicit operator _locationUnion( uint subresourceIndex ) => new( subresourceIndex ) ;
	}
	
	public TextureCopyLocation( IResource pResource, in PlacedSubresourceFootprint placedFootprint ) {
		ArgumentNullException.ThrowIfNull( pResource, nameof(pResource) ) ;
		var resource = (IComObjectRef< ID3D12Resource >)pResource ;
		this.pResource = resource.ComObject! ; 
		Type = TextureCopyType.PlacedFootprint ; 
		Location = new( placedFootprint ) ;
	}
	public TextureCopyLocation( IResource pResource, uint subresourceIndex ) {
		ArgumentNullException.ThrowIfNull( pResource, nameof(pResource) ) ;
		var resource = (IComObjectRef< ID3D12Resource >)pResource ;
		this.pResource = resource.ComObject! ; 
		Type = TextureCopyType.Index ; 
		Location = new( subresourceIndex ) ;
	}

	/*public unsafe TextureCopyLocation( ResourceUnmanaged* pResource, in PlacedSubresourceFootprint placedFootprint ) {
		 
	}*/
} ;


/// <summary>Describes parameters needed to allocate resources, including offset.</summary>
/// <remarks>This structure is used by the <see cref="Direct3D12.IDevice4.GetResourceAllocationInfo1"/> method.</remarks>
[EquivalentOf( typeof( D3D12_RESOURCE_ALLOCATION_INFO1 ) )]
public partial struct ResourceAllocationInfo1 {
	/// <summary>
	/// <para>The offset, in bytes, of the resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_allocation_info1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong Offset ;

	/// <summary>
	/// <para>The alignment value for the resource; one of 4KB (4096), 64KB (65536), or 4MB (4194304) alignment.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_allocation_info1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong Alignment ;

	/// <summary>
	/// <para>The size, in bytes, of the resource.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_allocation_info1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ulong SizeInBytes ;
	
	public ResourceAllocationInfo1( ulong offset = 0UL, ulong alignment = 0UL, ulong sizeInBytes = 0UL ) {
		Offset = offset ; Alignment = alignment ; SizeInBytes = sizeInBytes ;
	}
	
	public static implicit operator ResourceAllocationInfo1( in (uint Offset, uint Alignment, uint SizeInBytes) values ) =>
		new( values.Offset, values.Alignment, values.SizeInBytes ) ;
} ;


/// <summary>Defines flags that specify states related to a graphics command list. Values can be bitwise OR'd together.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_graphics_states">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_META_COMMAND_DESC))]
public partial struct MetaCommandDescription {
	
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/guiddef/ns-guiddef-guid">GUID</a></b> A <a href="https://docs.microsoft.com/windows/win32/api/guiddef/ns-guiddef-guid">GUID</a> uniquely identifying the meta command.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public Guid Id ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">LPCWSTR</a></b> The meta command name.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public PCWSTR Name ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_graphics_states">D3D12_GRAPHICS_STATES</a></b> Declares the command list states that are modified by the call to initialize the meta command. If all state bits are set, then that's equivalent to calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearstate">ID3D12GraphicsCommandList::ClearState</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public GraphicsStates InitializationDirtyState ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_graphics_states">D3D12_GRAPHICS_STATES</a></b> Declares the command list states that are modified by the call to execute the meta command. If all state bits are set, then that's equivalent to calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-clearstate">ID3D12GraphicsCommandList::ClearState</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public GraphicsStates ExecutionDirtyState ;
	
	
	public MetaCommandDescription( Guid id, PCWSTR name,
								   GraphicsStates initializationDirtyState = GraphicsStates.None, 
								   GraphicsStates executionDirtyState = GraphicsStates.None ) {
		Id = id ; Name = name ; 
		InitializationDirtyState = initializationDirtyState ; 
		ExecutionDirtyState = executionDirtyState ;
	}
} ;


/// <summary>Describes a parameter to a meta command.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_parameter_desc">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( D3D12_META_COMMAND_PARAMETER_DESC ) )]
public partial struct MetaCommandParameterDescription {
	
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">LPCWSTR</a></b> The parameter name.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_parameter_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public PCWSTR Name;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_meta_command_parameter_type">D3D12_META_COMMAND_PARAMETER_TYPE</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_meta_command_parameter_type">D3D12_META_COMMAND_PARAMETER_TYPE</a> specifying the parameter type.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_parameter_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public MetaCommandParameterType Type ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_meta_command_parameter_flags">D3D12_META_COMMAND_PARAMETER_FLAGS</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_meta_command_parameter_flags">D3D12_META_COMMAND_PARAMETER_FLAGS</a> specifying the parameter flags.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_parameter_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public MetaCommandParameterFlags Flags ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATES</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATES</a> specifying the expected state of a resource parameter.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_parameter_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public ResourceStates RequiredResourceState ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The 4-byte aligned offset for this parameter, within the structure containing the parameter values, which you pass when creating/initializing/executing the meta command, as appropriate.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_meta_command_parameter_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public uint StructureOffset ;
	
	
	public MetaCommandParameterDescription( PCWSTR name, 
											MetaCommandParameterType type = default,
											MetaCommandParameterFlags flags = default,
											ResourceStates requiredResourceState = ResourceStates.Common,
											uint structureOffset = 0U ) {
		Name = name ; Type = type ; Flags = flags ; 
		RequiredResourceState = requiredResourceState ; StructureOffset = structureOffset ;
	}
} ;


/// <summary>Description of a state object. Pass this structure into ID3D12Device::CreateStateObject.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_state_object_desc">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_STATE_OBJECT_DESC))]
public partial struct StateObjectDescription {
	/// <summary>The type of the state object.</summary>
	public StateObjectType Type ;

	/// <summary>Size of the <i>pSubobjects</i> array.</summary>
	public uint NumSubobjects ;

	/// <summary>An array of subobject definitions.</summary>
	public unsafe D3D12_STATE_SUBOBJECT* pSubobjects;
} ;


/// <summary>Represents a subobject within a state object description. Use with [D3D12_STATE_OBJECT_DESC](/windows/win32/api/d3d12/ns-d3d12-d3d12_state_object_desc).</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_state_subobject">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(D3D12_STATE_SUBOBJECT))]
public partial struct StateSubObject {
	/// <summary>A <see cref="StateSubObjectType"/> specifying the type of the state subobject.</summary>
	public StateSubObjectType Type ;

	/// <summary>Pointer to state object description of the type specified in the *Type* parameter.</summary>
	public nint pDesc ;
	
	
	public StateSubObject( StateSubObjectType type = default, nint pDesc = 0x00000000 ) {
		Type = type ; this.pDesc = pDesc ;
	}
} ;


/// <summary>
/// Describes a resource, such as a texture, including a mip region.
/// This structure is used in several methods.
/// </summary>
[EquivalentOf( typeof( D3D12_RESOURCE_DESC1 ) )]
public partial struct ResourceDescription1 {
	
	/// <summary>One member of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_dimension">D3D12_RESOURCE_DIMENSION</a>, specifying the dimensions of the resource (for example, D3D12_RESOURCE_DIMENSION_TEXTURE1D), or whether it is a buffer ((D3D12_RESOURCE_DIMENSION_BUFFER).</summary>
	public ResourceDimension Dimension ;

	/// <summary>Specifies the alignment.</summary>
	public ulong Alignment ;

	/// <summary>Specifies the width of the resource.</summary>
	public ulong Width ;

	/// <summary>Specifies the height of the resource.</summary>
	public uint Height ;

	/// <summary>Specifies the depth of the resource, if it is 3D, or the array size if it is an array of 1D or 2D resources.</summary>
	public ushort DepthOrArraySize ;

	/// <summary>Specifies the number of MIP levels.</summary>
	public ushort MipLevels ;

	/// <summary>Specifies one member of <a href="https://docs.microsoft.com/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>.</summary>
	public Format Format ;

	/// <summary>Specifies a <a href="https://docs.microsoft.com/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a> structure.</summary>
	public SampleDescription SampleDesc ;

	/// <summary>Specifies one member of <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_texture_layout">D3D12_TEXTURE_LAYOUT</a>.</summary>
	public TextureLayout Layout ;

	/// <summary>Bitwise-OR'd flags, as <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_resource_flags">D3D12_RESOURCE_FLAGS</a> enumeration constants.</summary>
	public ResourceFlags Flags ;

	/// <summary>A <see cref="MipRegion"/> struct.</summary>
	public MipRegion SamplerFeedbackMipRegion ;
	
	
	public ResourceDescription1( ResourceDimension dimension = ResourceDimension.Unknown,
								 ulong alignment = 0UL,
								 ulong width = 0UL,
								 uint height = 1U,
								 ushort depthOrArraySize = 1,
								 ushort mipLevels = 1,
								 Format format = Format.UNKNOWN,
								 SampleDescription sampleDesc = default,
								 TextureLayout layout = TextureLayout.Unknown,
								 ResourceFlags flags = ResourceFlags.None,
								 MipRegion samplerFeedbackMipRegion = default ) {
		Dimension = dimension ; Alignment = alignment ; Width = width ; Height = height ; 
		DepthOrArraySize = depthOrArraySize ; MipLevels = mipLevels ; Format = format ; 
		SampleDesc = sampleDesc ; Layout = layout ; Flags = flags ; 
		SamplerFeedbackMipRegion = samplerFeedbackMipRegion ;
	}
} ;


/// <summary>Describes the dimensions of a mip region.</summary>
[EquivalentOf(typeof(D3D12_MIP_REGION))]
public partial struct MipRegion {
	/// <summary>The width of the mip region.</summary>
	public uint Width ;

	/// <summary>The height of the mip region.</summary>
	public uint Height ;

	/// <summary>The depth of the mip region.</summary>
	public uint Depth ;
	
	
	public MipRegion( uint width = 0U, uint height = 0U, uint depth = 0U ) {
		Width = width ; Height = height ; Depth = depth ;
	}
	
	public static implicit operator MipRegion( in (uint Width, uint Height, uint Depth) values ) =>
		new( values.Width, values.Height, values.Depth ) ;
} ;


/// <summary>Describes a shader cache session.</summary>
[EquivalentOf(typeof(D3D12_SHADER_CACHE_SESSION_DESC))]
public partial struct ShaderCacheSessionDescription {
	/// <summary>
	/// <para>A unique identifier to give to this specific cache.
	/// Caches with different identifiers are stored side by side. Caches with the same identifier are shared across all sessions in the same process.
	/// Creating a disk cache with the same identifier as an already-existing cache opens that cache, unless the **Version** doesn't matches.
	/// In that case, if there are no other sessions open to that cache, it is cleared and re-created.
	/// If there are existing sessions, then <see cref="IDevice9.CreateShaderCacheSession"/> returns <b>DXGI_ERROR_ALREADY_EXISTS</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_cache_session_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public Guid Identifier ;

	/// <summary>
	/// <para>Specifies the kind of cache.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_cache_session_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public ShaderCacheMode Mode ;

	/// <summary>
	/// <para>Modifies the behavior of the cache.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_cache_session_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public ShaderCacheFlags Flags ;
	
	/// <summary>
	/// <para>For in-memory caches, this is the only storage available. For disk caches, all entries that are stored or found are temporarily stored in memory, until evicted by newer entries. This value determines the size of that temporary storage. Defaults to 1KB.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_cache_session_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public uint MaximumInMemoryCacheSizeBytes ;

	/// <summary>
	/// <para>Specifies how many entries can be stored in memory. Defaults to 128.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_cache_session_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public uint MaximumInMemoryCacheEntries ;

	/// <summary>
	/// <para>For disk caches, controls the maximum file size. Defaults to 128MB.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_cache_session_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public uint MaximumValueFileSizeBytes ;

	/// <summary>
	/// <para>Can be used to implicitly clear caches when an application or component update is done. If the version doesn't match the version stored in the cache, then it will be wiped and re-created.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_shader_cache_session_desc#members">Read more on docs.microsoft.com </a>.</para>
	/// </summary>
	public ulong Version ;
	
	
	public ShaderCacheSessionDescription( Guid identifier, ShaderCacheMode mode = ShaderCacheMode.Memory,
										  ShaderCacheFlags flags = ShaderCacheFlags.None,
										  uint maximumInMemoryCacheSizeBytes = 1024U,
										  uint maximumInMemoryCacheEntries = 128U,
										  uint maximumValueFileSizeBytes = 128U,
										  ulong version = 0UL ) {
		Identifier = identifier ; Mode = mode ; Flags = flags ; 
		MaximumInMemoryCacheSizeBytes = maximumInMemoryCacheSizeBytes ; 
		MaximumInMemoryCacheEntries = maximumInMemoryCacheEntries ; 
		MaximumValueFileSizeBytes = maximumValueFileSizeBytes ; 
		Version = version ;
	}
} ;


[EquivalentOf(typeof(D3D12_SAMPLER_DESC2))]
public partial struct SamplerDescription2 {
	public Filter Filter ;
	public TextureAddressMode AddressU ;
	public TextureAddressMode AddressV ;
	public TextureAddressMode AddressW ;
	public float MipLODBias ;
	public uint MaxAnisotropy ;
	public ComparisonFunction ComparisonFunc ;
	public _borderColorUnion BorderColor ;
	public float MinLOD ;
	public float MaxLOD ;
	public SamplerFlags Flags ;

	[StructLayout( LayoutKind.Explicit )]
	public partial struct _borderColorUnion {
		[FieldOffset( 0 )] public __float_4 FloatBorderColor ;
		[FieldOffset( 0 )] public __uint_4 UintBorderColor ;
		
		public _borderColorUnion( __float_4 floatBorderColor ) {
			Unsafe.SkipInit( out this ) ;
			FloatBorderColor = floatBorderColor ;
		}
		public _borderColorUnion( __uint_4 uintBorderColor ) {
			Unsafe.SkipInit( out this ) ;
			UintBorderColor = uintBorderColor ;
		}
		public static implicit operator _borderColorUnion( __float_4 floatBorderColor ) =>
			new( floatBorderColor ) ;
		public static implicit operator _borderColorUnion( __uint_4 uintBorderColor ) =>
			new( uintBorderColor ) ;
	}
	
	public SamplerDescription2( Filter filter = Filter.MinMagMipPoint,
								TextureAddressMode addressU = TextureAddressMode.Clamp,
								TextureAddressMode addressV = TextureAddressMode.Clamp,
								TextureAddressMode addressW = TextureAddressMode.Clamp,
								float mipLODBias = 0.0f,
								uint maxAnisotropy = 0,
								ComparisonFunction comparisonFunc = ComparisonFunction.Never,
								__float_4 borderColor = default,
								float minLOD = 0.0f,
								float maxLOD = float.MaxValue,
								SamplerFlags flags = SamplerFlags.None ) {
		Filter = filter ; AddressU = addressU ; AddressV = addressV ; AddressW = addressW ; 
		MipLODBias = mipLODBias ; MaxAnisotropy = maxAnisotropy ; ComparisonFunc = comparisonFunc ; 
		BorderColor = new( borderColor ) ; MinLOD = minLOD ; MaxLOD = maxLOD ; Flags = flags ;
	}
	
	public SamplerDescription2( Filter filter, TextureAddressMode addressU, TextureAddressMode addressV, TextureAddressMode addressW,
								float mipLODBias, uint maxAnisotropy, ComparisonFunction comparisonFunc, 
								__uint_4 borderColor, float minLOD, float maxLOD, SamplerFlags flags ) {
		Filter = filter ; AddressU = addressU ; AddressV = addressV ; AddressW = addressW ; 
		MipLODBias = mipLODBias ; MaxAnisotropy = maxAnisotropy ; ComparisonFunc = comparisonFunc ; 
		BorderColor = new( borderColor ) ; MinLOD = minLOD ; MaxLOD = maxLOD ; Flags = flags ;
	}
} ;

