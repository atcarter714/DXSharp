/* NOTE:
 * This is actually a better way to add our implicit/explicit conversion operators
 * and handle conversions, casting, marshalling, etc between the structs created by
 * the Win32 metadata generator and our own structs. The idea is that we will, in
 * time, replace all the generator code with our own, more idiomatic ".NET style"
 * code, and CsWin32 is a bridge to: even if the code it generates is a bit ugly
 * and feels awkward and janky to use, it works and it shows us everything you must
 * implement to get the job done. From now on, we will do conversion operators to/from
 * CsWin32-generated types to DXSharp types by adding onto the partial type definitions
 * in files like these, so that when they're no longer needed we simply don't compile
 * the files with this code in them (rather than having to edit DXSharp types all over).
 */

#region Using Directives
using System ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi.Common ;
using DXSharp ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;

#endregion
namespace Windows.Win32.Graphics.Direct3D12 ;


//! We could actually write our own Roslyn generator to do this dirty work for us ...
public partial struct D3D12_CPU_DESCRIPTOR_HANDLE
{
	public D3D12_CPU_DESCRIPTOR_HANDLE( nuint ptr) {
		this.ptr = ptr ;
	}
	public D3D12_CPU_DESCRIPTOR_HANDLE( nint ptr) {
		this.ptr = (nuint)ptr ;
	}

	public static implicit operator nuint( in D3D12_CPU_DESCRIPTOR_HANDLE handle ) {
		nuint ptr = default ;
		ptr = handle.ptr ;
		return ptr ;
	}
	public static implicit operator D3D12_CPU_DESCRIPTOR_HANDLE( in nuint ptr ) {
		D3D12_CPU_DESCRIPTOR_HANDLE handle = default ;
		handle = new( ptr ) ;
		return handle ;
	}
	public static implicit operator nint( in D3D12_CPU_DESCRIPTOR_HANDLE handle ) {
		nint ptr = default ;
		ptr = (nint)handle.ptr ;
		return ptr ;
	}
	public static implicit operator D3D12_CPU_DESCRIPTOR_HANDLE( in nint ptr ) {
		D3D12_CPU_DESCRIPTOR_HANDLE handle = default ;
		handle = new( (nuint)ptr ) ;
		return handle ;
	}
}


[CsWin32, EquivalentOf(typeof(TiledResourceCoordinate))]
public partial struct D3D12_TILED_RESOURCE_COORDINATE {
	public D3D12_TILED_RESOURCE_COORDINATE( uint x = 0, uint y = 0, uint z = 0, uint subresource = 0u ) {
		X = x ;
		Y = y ;
		Z = z ;
		Subresource = subresource ;
	}
	
	public static implicit operator TiledResourceCoordinate( D3D12_TILED_RESOURCE_COORDINATE coord ) => new( coord.X, coord.Y, coord.Z, coord.Subresource ) ;
	public static implicit operator D3D12_TILED_RESOURCE_COORDINATE( TiledResourceCoordinate coord ) => new( coord.X, coord.Y, coord.Z, coord.Subresource ) ;
} ;


[CsWin32, EquivalentOf(typeof(CommandQueueDescription))]
public partial struct D3D12_COMMAND_QUEUE_DESC {
	public D3D12_COMMAND_QUEUE_DESC( D3D12_COMMAND_LIST_TYPE type, 
									 int priority = 0,
									 D3D12_COMMAND_QUEUE_FLAGS flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE, 
									 uint nodeMask = 0 ) {
		Flags    = flags ;
		Type     = type ;
		Priority = priority ;
		NodeMask = nodeMask ;
	}
	public D3D12_COMMAND_QUEUE_DESC( CommandListType type, int priority = 0, CommandQueueFlags flags = CommandQueueFlags.None, uint nodeMask = 0 ) {
		Flags    = (D3D12_COMMAND_QUEUE_FLAGS)flags ;
		Type     = (D3D12_COMMAND_LIST_TYPE)type ;
		Priority = priority ;
		NodeMask = nodeMask ;
	}
	
	public static implicit operator CommandQueueDescription( D3D12_COMMAND_QUEUE_DESC desc ) => 
		new( (CommandListType)desc.Type, desc.Priority, (CommandQueueFlags)desc.Flags, desc.NodeMask ) ;
	
	public static implicit operator D3D12_COMMAND_QUEUE_DESC( CommandQueueDescription desc ) =>
	 		new( (D3D12_COMMAND_LIST_TYPE)desc.Type, desc.Priority, (D3D12_COMMAND_QUEUE_FLAGS)desc.Flags, desc.NodeMask ) ;
} ;


[CsWin32, EquivalentOf( typeof( InputLayoutDescription ) )]
public partial struct D3D12_INPUT_LAYOUT_DESC {
	public static implicit operator D3D12_INPUT_LAYOUT_DESC( in InputLayoutDescription desc ) {
		unsafe { return new D3D12_INPUT_LAYOUT_DESC {
				pInputElementDescs = (D3D12_INPUT_ELEMENT_DESC*)desc.pInputElementDescs,
				NumElements        = desc.NumElements
			} ;
		}
	}
	
	public static implicit operator InputLayoutDescription( in D3D12_INPUT_LAYOUT_DESC desc ) {
		unsafe { return new InputLayoutDescription {
				pInputElementDescs = (nint)desc.pInputElementDescs,
				NumElements        = desc.NumElements
			} ;
		}
	}
} ;


[CsWin32, EquivalentOf( typeof( GraphicsPipelineStateDescription ) )]
public partial struct D3D12_GRAPHICS_PIPELINE_STATE_DESC {
	
	public static implicit operator D3D12_GRAPHICS_PIPELINE_STATE_DESC( in GraphicsPipelineStateDescription desc ) => 
		new D3D12_GRAPHICS_PIPELINE_STATE_DESC {
			pRootSignature        = desc.pRootSignature,
			VS                    = desc.VS,
			PS                    = desc.PS,
			DS                    = desc.DS,
			HS                    = desc.HS,
			GS                    = desc.GS,
			StreamOutput          = desc.StreamOutput,
			BlendState            = desc.BlendState,
			SampleMask            = desc.SampleMask,
			RasterizerState       = desc.RasterizerState,
			DepthStencilState     = desc.DepthStencilState,
			InputLayout           = desc.InputLayout,
			IBStripCutValue       = (D3D12_INDEX_BUFFER_STRIP_CUT_VALUE)desc.IBStripCutValue,
			PrimitiveTopologyType = (D3D12_PRIMITIVE_TOPOLOGY_TYPE)desc.PrimitiveTopologyType,
			NumRenderTargets      = desc.NumRenderTargets,
			RTVFormats            = desc.RTVFormats,
			DSVFormat             = (DXGI_FORMAT)desc.DSVFormat,
			SampleDesc            = desc.SampleDesc,
			NodeMask              = desc.NodeMask,
			CachedPSO             = desc.CachedPSO,
			Flags                 = (D3D12_PIPELINE_STATE_FLAGS)desc.Flags
	} ;

	public static implicit operator GraphicsPipelineStateDescription( in D3D12_GRAPHICS_PIPELINE_STATE_DESC desc ) =>
		new GraphicsPipelineStateDescription {
			pRootSignature        = desc.pRootSignature,
			VS                    = desc.VS,
			PS                    = desc.PS,
			DS                    = desc.DS,
			HS                    = desc.HS,
			GS                    = desc.GS,
			StreamOutput          = desc.StreamOutput,
			BlendState            = new( desc.BlendState ),
			SampleMask            = desc.SampleMask,
			RasterizerState       = desc.RasterizerState,
			DepthStencilState     = desc.DepthStencilState,
			InputLayout           = desc.InputLayout,
			IBStripCutValue       = (IndexBufferStripCutValue)desc.IBStripCutValue,
			PrimitiveTopologyType = (PrimitiveTopologyType)desc.PrimitiveTopologyType,
			NumRenderTargets      = desc.NumRenderTargets,
			RTVFormats            = desc.RTVFormats,
			DSVFormat             = (Format)desc.DSVFormat,
			SampleDesc            = desc.SampleDesc,
			NodeMask              = desc.NodeMask,
			CachedPSO             = desc.CachedPSO,
			Flags                 = (PipelineStateFlags)desc.Flags,
		} ;
} ;


[CsWin32, EquivalentOf( typeof( HeapProperties ) )]
public partial struct D3D12_HEAP_PROPERTIES {
	
	public static implicit operator HeapProperties( in D3D12_HEAP_PROPERTIES props ) => new HeapProperties {
		Type                 = (HeapType)props.Type,
		CPUPageProperty      = (CpuPageProperty)props.CPUPageProperty,
		MemoryPoolPreference = (MemoryPool)props.MemoryPoolPreference,
		CreationNodeMask     = props.CreationNodeMask,
		VisibleNodeMask      = props.VisibleNodeMask
	} ;
	
	public static implicit operator D3D12_HEAP_PROPERTIES( in HeapProperties props ) => new D3D12_HEAP_PROPERTIES {
		Type                 = (D3D12_HEAP_TYPE)props.Type,
		CPUPageProperty      = (D3D12_CPU_PAGE_PROPERTY)props.CPUPageProperty,
		MemoryPoolPreference = (D3D12_MEMORY_POOL)props.MemoryPoolPreference,
		CreationNodeMask     = props.CreationNodeMask,
		VisibleNodeMask      = props.VisibleNodeMask
	} ;
} ;


[CsWin32, EquivalentOf( typeof( ResourceDescription ) )]
public partial struct D3D12_RESOURCE_DESC {
	
	public static implicit operator ResourceDescription( in D3D12_RESOURCE_DESC desc ) => new ResourceDescription {
		Dimension        = (ResourceDimension)desc.Dimension,
		Alignment        = desc.Alignment,
		Width            = desc.Width,
		Height           = desc.Height,
		DepthOrArraySize = desc.DepthOrArraySize,
		MipLevels        = desc.MipLevels,
		Format           = (Format)desc.Format,
		SampleDesc       = (SampleDescription)desc.SampleDesc,
		Layout           = (TextureLayout)desc.Layout,
		Flags            = (ResourceFlags)desc.Flags
	} ;
	
	public static implicit operator D3D12_RESOURCE_DESC( in ResourceDescription desc ) => new D3D12_RESOURCE_DESC {
		Dimension        = (D3D12_RESOURCE_DIMENSION)desc.Dimension,
		Alignment        = desc.Alignment,
		Width            = desc.Width,
		Height           = desc.Height,
		DepthOrArraySize = desc.DepthOrArraySize,
		MipLevels        = desc.MipLevels,
		Format           = (DXGI_FORMAT)desc.Format,
		SampleDesc       = (DXGI_SAMPLE_DESC)desc.SampleDesc,
		Layout           = (D3D12_TEXTURE_LAYOUT)desc.Layout,
		Flags            = (D3D12_RESOURCE_FLAGS)desc.Flags
	} ;

} ;


[CsWin32, EquivalentOf( typeof( RenderTargetViewDescription ) )]
public partial struct D3D12_RENDER_TARGET_VIEW_DESC {
	
	public static implicit operator D3D12_RENDER_TARGET_VIEW_DESC( in RenderTargetViewDescription o ) {
		unsafe {
			fixed ( RenderTargetViewDescription* ptr = &o ) {
				return *(D3D12_RENDER_TARGET_VIEW_DESC*)ptr ;
			}
		}
	}

	public static implicit operator RenderTargetViewDescription( in D3D12_RENDER_TARGET_VIEW_DESC o ) {
		unsafe {
			fixed ( D3D12_RENDER_TARGET_VIEW_DESC* ptr = &o ) {
				return *(RenderTargetViewDescription*)ptr ;
			}
		}
	}
} ;


[CsWin32, EquivalentOf( typeof( BlendDescription ) )]
public partial struct D3D12_BLEND_DESC {
	public static implicit operator D3D12_BLEND_DESC( in BlendDescription o ) {
		unsafe {
			fixed ( BlendDescription* ptr = &o ) {
				return *(D3D12_BLEND_DESC*)ptr ;
			}
		}
	}
	public static implicit operator BlendDescription( in D3D12_BLEND_DESC o ) {
		unsafe {
			fixed ( D3D12_BLEND_DESC* ptr = &o ) {
				return *(BlendDescription*)ptr ;
			}
		}
	}
	
	/*public static implicit operator D3D12_BLEND_DESC( in BlendDescription desc ) => new D3D12_BLEND_DESC {
		AlphaToCoverageEnable  = desc.AlphaToCoverageEnable,
		IndependentBlendEnable = desc.IndependentBlendEnable,
		RenderTarget           = desc.RenderTarget
	} ;
	
	public static implicit operator BlendDescription( in D3D12_BLEND_DESC desc ) => new BlendDescription {
		AlphaToCoverageEnable  = desc.AlphaToCoverageEnable,
		IndependentBlendEnable = desc.IndependentBlendEnable,
		RenderTarget           = desc.RenderTarget
	} ;*/
} ;


[CsWin32, EquivalentOf( typeof( RTBlendDescription ) )]
public partial struct D3D12_RENDER_TARGET_BLEND_DESC {
	public static implicit operator D3D12_RENDER_TARGET_BLEND_DESC( in RTBlendDescription desc ) => new D3D12_RENDER_TARGET_BLEND_DESC {
			BlendEnable           = desc.BlendEnable,
			LogicOpEnable         = desc.LogicOpEnable,
			SrcBlend              = (D3D12_BLEND)desc.SrcBlend,
			DestBlend             = (D3D12_BLEND)desc.DestBlend,
			BlendOp               = (D3D12_BLEND_OP)desc.BlendOp,
			SrcBlendAlpha         = (D3D12_BLEND)desc.SrcBlendAlpha,
			DestBlendAlpha        = (D3D12_BLEND)desc.DestBlendAlpha,
			BlendOpAlpha          = (D3D12_BLEND_OP)desc.BlendOpAlpha,
			LogicOp               = (D3D12_LOGIC_OP)desc.LogicOp,
			RenderTargetWriteMask = (byte)desc.RenderTargetWriteMask
		} ;
	public static implicit operator RTBlendDescription( in D3D12_RENDER_TARGET_BLEND_DESC desc ) => new RTBlendDescription {
			BlendEnable           = desc.BlendEnable,
			LogicOpEnable         = desc.LogicOpEnable,
			SrcBlend              = (Blend)desc.SrcBlend,
			DestBlend             = (Blend)desc.DestBlend,
			BlendOp               = (BlendOperation)desc.BlendOp,
			SrcBlendAlpha         = (Blend)desc.SrcBlendAlpha,
			DestBlendAlpha        = (Blend)desc.DestBlendAlpha,
			BlendOpAlpha          = (BlendOperation)desc.BlendOpAlpha,
			LogicOp               = (LogicOperation)desc.LogicOp,
			RenderTargetWriteMask = (ColorWriteEnableFlags)desc.RenderTargetWriteMask
		} ;
} ;


[CsWin32, EquivalentOf( typeof( RasterizerDescription ) )]
public partial struct __D3D12_RENDER_TARGET_BLEND_DESC_8 {
	public static implicit operator RTBlendDescription8( in __D3D12_RENDER_TARGET_BLEND_DESC_8 value ) {
		Unsafe.SkipInit( out RTBlendDescription8 result ) ;
		result._0 = value._0 ;
		result._1 = value._1 ;
		result._2 = value._2 ;
		result._3 = value._3 ;
		result._4 = value._4 ;
		result._5 = value._5 ;
		result._6 = value._6 ;
		result._7 = value._7 ;
		return result ;
	}
	public static implicit operator __D3D12_RENDER_TARGET_BLEND_DESC_8( in RTBlendDescription8 value ) {
		Unsafe.SkipInit( out __D3D12_RENDER_TARGET_BLEND_DESC_8 result ) ;
		result._0 = value._0 ;
		result._1 = value._1 ;
		result._2 = value._2 ;
		result._3 = value._3 ;
		result._4 = value._4 ;
		result._5 = value._5 ;
		result._6 = value._6 ;
		result._7 = value._7 ;
		return result ;
	}
} ;


[CsWin32, EquivalentOf( typeof( StreamOutputDescription ) )]
public partial struct D3D12_STREAM_OUTPUT_DESC {
	public static implicit operator D3D12_STREAM_OUTPUT_DESC( in StreamOutputDescription desc ) {
		unsafe {
			return new D3D12_STREAM_OUTPUT_DESC
			{
				pSODeclaration   = (D3D12_SO_DECLARATION_ENTRY*)desc.pSODeclaration,
				NumEntries       = desc.NumEntries,
				pBufferStrides   = (uint*)desc.pBufferStrides,
				NumStrides       = desc.NumStrides,
				RasterizedStream = desc.RasterizedStream
			} ;
		}
	}
	public static implicit operator StreamOutputDescription( in D3D12_STREAM_OUTPUT_DESC desc ) {
		unsafe {
			return new StreamOutputDescription
			{
				pSODeclaration   = (SODeclarationEntry*)desc.pSODeclaration,
				NumEntries       = desc.NumEntries,
				pBufferStrides   = (uint*)desc.pBufferStrides,
				NumStrides       = desc.NumStrides,
				RasterizedStream = desc.RasterizedStream
			} ;
		}
	}
} ;


[CsWin32, EquivalentOf( typeof( SODeclarationEntry ) )]
public partial struct D3D12_SO_DECLARATION_ENTRY {
	public static implicit operator D3D12_SO_DECLARATION_ENTRY( in SODeclarationEntry entry ) => new D3D12_SO_DECLARATION_ENTRY {
		SemanticName         = entry.SemanticName,
		SemanticIndex        = entry.SemanticIndex,
		StartComponent       = entry.StartComponent,
		ComponentCount       = entry.ComponentCount,
		OutputSlot           = entry.OutputSlot
	} ;
	public static implicit operator SODeclarationEntry( in D3D12_SO_DECLARATION_ENTRY entry ) => new SODeclarationEntry {
		SemanticName         = entry.SemanticName,
		SemanticIndex        = entry.SemanticIndex,
		StartComponent       = entry.StartComponent,
		ComponentCount       = entry.ComponentCount,
		OutputSlot           = entry.OutputSlot
	} ;
} ;


[CsWin32, EquivalentOf( typeof( DepthStencilDesc ) )]
public partial struct D3D12_DEPTH_STENCIL_DESC {
	public static implicit operator DepthStencilDesc( D3D12_DEPTH_STENCIL_DESC desc ) => new DepthStencilDesc {
		DepthEnable      = desc.DepthEnable,
		DepthWriteMask   = (DepthWriteMask)desc.DepthWriteMask,
		DepthFunc        = (ComparisonFunction)desc.DepthFunc,
		StencilEnable    = desc.StencilEnable,
		StencilReadMask  = desc.StencilReadMask,
		StencilWriteMask = desc.StencilWriteMask,
		FrontFace        = (DepthStencilOpDesc)desc.FrontFace,
		BackFace         = (DepthStencilOpDesc)desc.BackFace,
	} ;
	public static implicit operator D3D12_DEPTH_STENCIL_DESC( DepthStencilDesc desc ) => new D3D12_DEPTH_STENCIL_DESC {
		DepthEnable      = desc.DepthEnable,
		DepthWriteMask   = (D3D12_DEPTH_WRITE_MASK)desc.DepthWriteMask,
		DepthFunc        = (D3D12_COMPARISON_FUNC)desc.DepthFunc,
		StencilEnable    = desc.StencilEnable,
		StencilReadMask  = desc.StencilReadMask,
		StencilWriteMask = desc.StencilWriteMask,
		FrontFace        = (D3D12_DEPTH_STENCILOP_DESC)desc.FrontFace,
		BackFace         = (D3D12_DEPTH_STENCILOP_DESC)desc.BackFace,
	} ;
} ;


[CsWin32, EquivalentOf( typeof( DepthStencilOpDesc ) )]
public partial struct D3D12_DEPTH_STENCILOP_DESC {
	public static implicit operator DepthStencilOpDesc( D3D12_DEPTH_STENCILOP_DESC desc ) => new( (StencilOperation)desc.StencilFailOp, 
			 (StencilOperation)desc.StencilDepthFailOp, 
			 (StencilOperation)desc.StencilPassOp, 
			 (ComparisonFunction)desc.StencilFunc ) ;
	public static implicit operator D3D12_DEPTH_STENCILOP_DESC( DepthStencilOpDesc desc ) => new D3D12_DEPTH_STENCILOP_DESC {
			StencilFailOp      = (D3D12_STENCIL_OP)desc.StencilFailOp,
			StencilDepthFailOp = (D3D12_STENCIL_OP)desc.StencilDepthFailOp,
			StencilPassOp      = (D3D12_STENCIL_OP)desc.StencilPassOp,
			StencilFunc        = (D3D12_COMPARISON_FUNC)desc.StencilFunc,
		} ;
} ;


[CsWin32, EquivalentOf( typeof( DepthStencilViewDesc ) )]
public partial struct D3D12_DEPTH_STENCIL_VIEW_DESC {
	public static implicit operator D3D12_DEPTH_STENCIL_VIEW_DESC( in DepthStencilViewDesc desc ) { unsafe {
			fixed(DepthStencilViewDesc* ptr = &desc)
				return *(D3D12_DEPTH_STENCIL_VIEW_DESC*)ptr ;
		}
	}
	public static implicit operator DepthStencilViewDesc( in D3D12_DEPTH_STENCIL_VIEW_DESC desc ) { unsafe {
			fixed(D3D12_DEPTH_STENCIL_VIEW_DESC* ptr = &desc)
				return *(DepthStencilViewDesc*)ptr ;
		}
	}
}


[CsWin32, EquivalentOf(typeof(SerializedDataDriverMatchingIdentifier))]
public partial struct D3D12_SERIALIZED_DATA_DRIVER_MATCHING_IDENTIFIER {
	public static implicit operator D3D12_SERIALIZED_DATA_DRIVER_MATCHING_IDENTIFIER( in SerializedDataDriverMatchingIdentifier o ) {
		unsafe {
			fixed ( SerializedDataDriverMatchingIdentifier* ptr = &o ) {
				return *(D3D12_SERIALIZED_DATA_DRIVER_MATCHING_IDENTIFIER*)ptr ;
			}
		}
	}
	public static implicit operator SerializedDataDriverMatchingIdentifier( in D3D12_SERIALIZED_DATA_DRIVER_MATCHING_IDENTIFIER o ) {
		unsafe {
			fixed ( D3D12_SERIALIZED_DATA_DRIVER_MATCHING_IDENTIFIER* ptr = &o ) {
				return *(SerializedDataDriverMatchingIdentifier*)ptr ;
			}
		}
	}
}


[CsWin32, EquivalentOf( typeof( ResourceBarrier ) )]
public partial struct D3D12_RESOURCE_BARRIER {
	public static implicit operator D3D12_RESOURCE_BARRIER(in ResourceBarrier o)
	{
		/*unsafe {
			fixed ( ResourceBarrier* ptr = &o ) {
				return *(D3D12_RESOURCE_BARRIER*)ptr ;
			}
		}*/
		unsafe
		{

			D3D12_RESOURCE_BARRIER dst = default;
			dst.Flags = (D3D12_RESOURCE_BARRIER_FLAGS)o.Flags;
			dst.Type = (D3D12_RESOURCE_BARRIER_TYPE)o.Type;
			switch (dst.Type)
			{
				case D3D12_RESOURCE_BARRIER_TYPE.D3D12_RESOURCE_BARRIER_TYPE_TRANSITION:
					dst.Anonymous.Transition = new( )
					{
						pResource = (ID3D12Resource_unmanaged*)o.Anonymous.Transition.pResource,
						StateBefore = (D3D12_RESOURCE_STATES)o.Anonymous.Transition.StateBefore,
						StateAfter = (D3D12_RESOURCE_STATES)o.Anonymous.Transition.StateAfter,
						Subresource = o.Anonymous.Transition.Subresource,
					} ;
					break;

				case D3D12_RESOURCE_BARRIER_TYPE.D3D12_RESOURCE_BARRIER_TYPE_ALIASING:
					dst.Anonymous.Aliasing = new()
					{
						pResourceAfter = (ID3D12Resource_unmanaged*)o.Anonymous.Aliasing.pResourceAfter,
						pResourceBefore = (ID3D12Resource_unmanaged*)o.Anonymous.Aliasing.pResourceBefore,
					};
					break;

				case D3D12_RESOURCE_BARRIER_TYPE.D3D12_RESOURCE_BARRIER_TYPE_UAV:
					dst.Anonymous.UAV = new()
					{
						pResource = (ID3D12Resource_unmanaged*)o.Anonymous.UAV.pResource,
					};
					break;

				default:
					break;
			}

			return dst;
		}
	}

	public static implicit operator ResourceBarrier( in D3D12_RESOURCE_BARRIER o ) {
		/*unsafe {
			fixed ( D3D12_RESOURCE_BARRIER* ptr = &o ) {
				return *(ResourceBarrier*)ptr ;
			}
		}*/
		unsafe {

			ResourceBarrier dst = default;
			dst.Flags = (ResourceBarrierFlags)o.Flags;
			dst.Type = (ResourceBarrierType)o.Type;
			switch( dst.Type ) {
				case ResourceBarrierType.Transition:
					dst.Anonymous.Transition = new()
					{
						pResource = (ResourceUnmanaged*)o.Anonymous.Transition.pResource,
						StateBefore = (ResourceStates)o.Anonymous.Transition.StateBefore,
						StateAfter = (ResourceStates)o.Anonymous.Transition.StateAfter,
						Subresource = o.Anonymous.Transition.Subresource,
					};
					break;

				case ResourceBarrierType.Aliasing:
					dst.Anonymous.Aliasing = new()
					{
						pResourceAfter = (ResourceUnmanaged*)o.Anonymous.Aliasing.pResourceAfter,
						pResourceBefore = (ResourceUnmanaged*)o.Anonymous.Aliasing.pResourceBefore,
					};
					break;

				case ResourceBarrierType.UAV:
					dst.Anonymous.UAV = new()
					{
						pResource = (ResourceUnmanaged*)o.Anonymous.UAV.pResource,
					};
					break;

				default:
					break;
			}
			
			return dst;
		}
	}
} ;

/*public partial struct _Anonymous_e__Union
{
	
}*/


[CsWin32, EquivalentOf( typeof( WriteBufferImmediateParameter ) )]
public partial struct D3D12_WRITEBUFFERIMMEDIATE_PARAMETER {
	public D3D12_WRITEBUFFERIMMEDIATE_PARAMETER( ulong dest = 0UL, uint value = 0U ) {
		Dest = dest ; Value = value ;
	}
	
	public static implicit operator D3D12_WRITEBUFFERIMMEDIATE_PARAMETER( in WriteBufferImmediateParameter o ) {
		unsafe {
			fixed ( WriteBufferImmediateParameter* ptr = &o ) {
				return *(D3D12_WRITEBUFFERIMMEDIATE_PARAMETER*)ptr ;
			}
		}
	}
	public static implicit operator WriteBufferImmediateParameter( in D3D12_WRITEBUFFERIMMEDIATE_PARAMETER o ) {
		unsafe {
			fixed ( D3D12_WRITEBUFFERIMMEDIATE_PARAMETER* ptr = &o ) {
				return *(WriteBufferImmediateParameter*)ptr ;
			}
		}
	}
} ;


[CsWin32, EquivalentOf(typeof(ProtectedResourceSessionDescription))]
public partial struct D3D12_PROTECTED_RESOURCE_SESSION_DESC {
	public D3D12_PROTECTED_RESOURCE_SESSION_DESC( uint nodeMask = 0, D3D12_PROTECTED_RESOURCE_SESSION_FLAGS flags = D3D12_PROTECTED_RESOURCE_SESSION_FLAGS.D3D12_PROTECTED_RESOURCE_SESSION_FLAG_NONE ) {
		NodeMask   = nodeMask ;
		this.Flags = flags ;
	}
	
	public static implicit operator D3D12_PROTECTED_RESOURCE_SESSION_DESC( in ProtectedResourceSessionDescription desc ) => new D3D12_PROTECTED_RESOURCE_SESSION_DESC {
		NodeMask = desc.NodeMask,
		Flags = (D3D12_PROTECTED_RESOURCE_SESSION_FLAGS)desc.Flags,
	} ;
	
	public static implicit operator ProtectedResourceSessionDescription( in D3D12_PROTECTED_RESOURCE_SESSION_DESC desc ) => new ProtectedResourceSessionDescription {
		NodeMask = desc.NodeMask,
		Flags = (ProtectedResourceSessionFlags)desc.Flags,
	} ;
} ;


[CsWin32, EquivalentOf(typeof(ProtectedResourceSessionDescription1))]
public partial struct D3D12_PROTECTED_RESOURCE_SESSION_DESC1
{
	public D3D12_PROTECTED_RESOURCE_SESSION_DESC1( uint nodeMask = 0, D3D12_PROTECTED_RESOURCE_SESSION_FLAGS flags = D3D12_PROTECTED_RESOURCE_SESSION_FLAGS.D3D12_PROTECTED_RESOURCE_SESSION_FLAG_NONE, Guid? protectionType = null ) {
		NodeMask = nodeMask ;
		this.Flags = flags ;
		ProtectionType = protectionType ?? Guid.Empty ;
	}
	
	public static implicit operator D3D12_PROTECTED_RESOURCE_SESSION_DESC1( in ProtectedResourceSessionDescription1 desc ) => new D3D12_PROTECTED_RESOURCE_SESSION_DESC1 {
		NodeMask = desc.NodeMask,
		Flags = (D3D12_PROTECTED_RESOURCE_SESSION_FLAGS)desc.Flags,
		ProtectionType = desc.ProtectionType,
	} ;
	
	public static implicit operator ProtectedResourceSessionDescription1( in D3D12_PROTECTED_RESOURCE_SESSION_DESC1 desc ) => new ProtectedResourceSessionDescription1 {
		NodeMask = desc.NodeMask,
		Flags = (ProtectedResourceSessionFlags)desc.Flags,
		ProtectionType = desc.ProtectionType,
	} ;
} ;



[CsWin32, EquivalentOf( typeof( RenderPassBeginningAccess ) )]
public partial struct D3D12_RENDER_PASS_BEGINNING_ACCESS {
	public static implicit operator D3D12_RENDER_PASS_BEGINNING_ACCESS( in RenderPassBeginningAccess desc ) {
		unsafe {
			Unsafe.SkipInit( out D3D12_RENDER_PASS_BEGINNING_ACCESS result ) ;
			var unionPtr = &result.Anonymous ;
			fixed( RenderPassBeginningAccess._accessUnion* pSrc = &desc.Access ) {
				var pDst = (RenderPassBeginningAccess._accessUnion *)unionPtr ;
				*pDst = *(RenderPassBeginningAccess._accessUnion*)pSrc ;
			}
			return result ;
		}
	}
	public static implicit operator RenderPassBeginningAccess( in D3D12_RENDER_PASS_BEGINNING_ACCESS desc ) {
		unsafe {
			Unsafe.SkipInit( out RenderPassBeginningAccess result ) ;
			var pDst = &result.Access ;
			
			fixed ( D3D12_RENDER_PASS_BEGINNING_ACCESS._Anonymous_e__Union* pSrc = &desc.Anonymous ) {
				*pDst = *(RenderPassBeginningAccess._accessUnion *)pSrc ;
			}

			return result ;
		}
	}
} ;


public partial struct D3D12_RENDER_PASS_ENDING_ACCESS
{
	public static implicit operator D3D12_RENDER_PASS_ENDING_ACCESS( in RenderPassEndingAccess desc ) {
		unsafe {
			Unsafe.SkipInit( out D3D12_RENDER_PASS_ENDING_ACCESS result ) ;
			var unionPtr = &result.Anonymous ;
			fixed( RenderPassEndingAccess._endAccessUnion* pSrc = &desc.Access ) {
				var pDst = (RenderPassEndingAccess._endAccessUnion *)unionPtr ;
				*pDst = *(RenderPassEndingAccess._endAccessUnion*)pSrc ;
			}
			return result ;
		}
	}
	public static implicit operator RenderPassEndingAccess( in D3D12_RENDER_PASS_ENDING_ACCESS desc ) {
		unsafe {
			Unsafe.SkipInit( out RenderPassEndingAccess result ) ;
			var pDst = &result.Access ;
			
			fixed ( D3D12_RENDER_PASS_ENDING_ACCESS._Anonymous_e__Union* pSrc = &desc.Anonymous ) {
				*pDst = *(RenderPassEndingAccess._endAccessUnion *)pSrc ;
			}

			return result ;
		}
	}
} ;


[CsWin32, EquivalentOf(typeof(RenderPassDepthStencilDescription))]
public partial struct D3D12_RENDER_PASS_DEPTH_STENCIL_DESC {
	public D3D12_RENDER_PASS_DEPTH_STENCIL_DESC( D3D12_CPU_DESCRIPTOR_HANDLE cpuDescriptor = default, 
												 D3D12_RENDER_PASS_BEGINNING_ACCESS depthBeginningAccess = default, 
												 D3D12_RENDER_PASS_BEGINNING_ACCESS stencilBeginningAccess = default, 
												 D3D12_RENDER_PASS_ENDING_ACCESS depthEndingAccess = default, 
												 D3D12_RENDER_PASS_ENDING_ACCESS stencilEndingAccess = default ) {
		this.cpuDescriptor          = cpuDescriptor ;
		this.DepthBeginningAccess   = depthBeginningAccess ;
		this.StencilBeginningAccess = stencilBeginningAccess ;
		this.DepthEndingAccess      = depthEndingAccess ;
		this.StencilEndingAccess    = stencilEndingAccess ;
	}
	
	public static implicit operator D3D12_RENDER_PASS_DEPTH_STENCIL_DESC( in RenderPassDepthStencilDescription desc ) => 
		new D3D12_RENDER_PASS_DEPTH_STENCIL_DESC {
		cpuDescriptor          = desc.cpuDescriptor,
		DepthBeginningAccess   = desc.DepthBeginningAccess,
		StencilBeginningAccess = desc.StencilBeginningAccess,
		DepthEndingAccess      = desc.DepthEndingAccess,
		StencilEndingAccess    = desc.StencilEndingAccess,
	} ;
	
	public static implicit operator RenderPassDepthStencilDescription( in D3D12_RENDER_PASS_DEPTH_STENCIL_DESC desc ) => 
		new RenderPassDepthStencilDescription {
		cpuDescriptor          = desc.cpuDescriptor,
		DepthBeginningAccess   = desc.DepthBeginningAccess,
		StencilBeginningAccess = desc.StencilBeginningAccess,
		DepthEndingAccess      = desc.DepthEndingAccess,
		StencilEndingAccess    = desc.StencilEndingAccess,
	} ;
} ;