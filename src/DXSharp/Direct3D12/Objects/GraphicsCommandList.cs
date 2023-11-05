#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi.Common ;

using DXSharp.DXGI ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList))]
internal class GraphicsCommandList: CommandList,
									IGraphicsCommandList,
									IComObjectRef< ID3D12GraphicsCommandList >,
									IUnknownWrapper< ID3D12GraphicsCommandList > {
	ComPtr< ID3D12GraphicsCommandList >? _comPtr ;
	public new ComPtr< ID3D12GraphicsCommandList >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12GraphicsCommandList>( ) ;

	public override ID3D12GraphicsCommandList? COMObject => ComPointer?.Interface ;
	
	
	internal GraphicsCommandList( ) {
		_comPtr = ComResources?.GetPointer< ID3D12GraphicsCommandList >( ) ;
	}
	internal GraphicsCommandList( ComPtr< ID3D12GraphicsCommandList > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList( ID3D12GraphicsCommandList comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------------

	public void Close( ) => COMObject?.Close( ) ;


	public void Reset( ICommandAllocator pAllocator, IPipelineState pInitialState ) {
		var allocator = (IComObjectRef< ID3D12CommandAllocator >)pAllocator ;
		var initialState = (IComObjectRef< ID3D12PipelineState >)pInitialState ;
#if DEBUG || DEBUG_COM || DEV_BUILD
		ArgumentNullException.ThrowIfNull( pAllocator, nameof(pAllocator) ) ;
		ArgumentNullException.ThrowIfNull( allocator.COMObject, nameof(allocator.COMObject) ) ;
		ArgumentNullException.ThrowIfNull( initialState, nameof(pInitialState) ) ;
		ArgumentNullException.ThrowIfNull( initialState.COMObject, nameof(initialState.COMObject) ) ;
#endif
		
		var device = COMObject ?? throw new NullReferenceException( nameof( IGraphicsCommandList ) ) ;
		device.Reset( allocator.COMObject, initialState.COMObject ) ;
	}

	
	public void ClearState( ID3D12PipelineState pPipelineState ) => COMObject!.ClearState( pPipelineState ) ;

	
	public void DrawInstanced( uint VertexCountPerInstance, uint InstanceCount, 
							   uint StartVertexLocation,    uint StartInstanceLocation ) => 
		COMObject!.DrawInstanced( VertexCountPerInstance, InstanceCount, 
								  StartVertexLocation, StartInstanceLocation ) ;
	

	public void DrawIndexedInstanced( uint IndexCountPerInstance, uint InstanceCount, 
									  uint StartIndexLocation,    int  BaseVertexLocation, 
									  uint StartInstanceLocation ) => 
		COMObject!.DrawIndexedInstanced( IndexCountPerInstance, InstanceCount,
										 StartIndexLocation, BaseVertexLocation,
										 StartInstanceLocation ) ;

	
	public void Dispatch( uint ThreadGroupCountX, uint ThreadGroupCountY, uint ThreadGroupCountZ ) => 
		COMObject!.Dispatch( ThreadGroupCountX, ThreadGroupCountY, ThreadGroupCountZ ) ;

	
	public void CopyBufferRegion( IResource pDstBuffer, ulong DstOffset, 
								  IResource pSrcBuffer, ulong SrcOffset, 
								  ulong     NumBytes ) {
		var src = (Resource)pSrcBuffer ;
		var dst = (Resource)pDstBuffer ;
		COMObject!.CopyBufferRegion( dst.COMObject, DstOffset,
									 src.COMObject, SrcOffset,
									 NumBytes ) ;
	}
	

	public void CopyTextureRegion( in TextureCopyLocation pDst, 
								   uint DstX, uint DstY, uint DstZ, 
								   in TextureCopyLocation pSrc,
								   [Optional] in Box pSrcBox ) {
		unsafe { fixed ( Box* pBox = &pSrcBox ) {
				COMObject!.CopyTextureRegion( pDst, DstX, DstY, DstZ, pSrc, (D3D12_BOX *)pBox ) ;
			}
		}
	}
	

	public void CopyResource( IResource pDstResource, IResource pSrcResource ) {
		var src = (Resource)pSrcResource ;
		var dst = (Resource)pDstResource ;
		COMObject!.CopyResource( dst.COMObject, src.COMObject ) ;
	}

	
	public void CopyTiles( IResource                  pTiledResource,
						   in TiledResourceCoordinate pTileRegionStartCoordinate, 
						   in TileRegionSize          pTileRegionSize,
						   IResource                  pBuffer,
						   ulong                      BufferStartOffsetInBytes,
						   TileCopyFlags              Flags ) {
		unsafe {
			var tiledResource = (Resource)pTiledResource ;
			var buffer = (Resource)pBuffer ;
			fixed( void* pTiledResCoord = &pTileRegionStartCoordinate, pRegionSize = &pTileRegionSize) {
				COMObject!.CopyTiles( tiledResource.COMObject, 
									  (D3D12_TILED_RESOURCE_COORDINATE *)pTiledResCoord, 
									  (D3D12_TILE_REGION_SIZE *)pRegionSize,
									  buffer.COMObject, 
									  BufferStartOffsetInBytes, 
									  (D3D12_TILE_COPY_FLAGS)Flags ) ;
			}
		}
	}

	
	public void ResolveSubresource( IResource pDstResource, uint DstSubresource, 
									IResource pSrcResource, uint SrcSubresource, 
									Format    Format) {
		var src = (Resource)pSrcResource ;
		var dst = (Resource)pDstResource ;
		COMObject!.ResolveSubresource( dst.COMObject, DstSubresource,
									   src.COMObject, SrcSubresource,
									   (DXGI_FORMAT)Format ) ;
	}

	
	public void IASetPrimitiveTopology( PrimitiveTopology PrimitiveTopology ) => 
		COMObject!.IASetPrimitiveTopology( (D3D_PRIMITIVE_TOPOLOGY)PrimitiveTopology ) ;

	
	public void RSSetViewports( uint NumViewports, in Span< Viewport > pViewports ) {
		unsafe {
			fixed( Viewport* pViewportsPtr = &pViewports[0] ) {
				COMObject!.RSSetViewports( NumViewports, (D3D12_VIEWPORT *)pViewportsPtr ) ;
			}
		}
	}

	
	public void RSSetScissorRects( uint NumRects, in Span< Rect > pRects ) {
		unsafe { fixed( Rect* pRectsPtr = &pRects[0] ) {
				COMObject!.RSSetScissorRects( NumRects, (RECT *)pRectsPtr ) ;
			}
		}
	}

	
	public void OMSetBlendFactor( float[ ] BlendFactor ) => COMObject!.OMSetBlendFactor( BlendFactor ) ;

	
	public void OMSetStencilRef( uint StencilRef ) => COMObject!.OMSetStencilRef( StencilRef ) ;

	public void SetPipelineState( IPipelineState pPipelineState ) => 
		COMObject!.SetPipelineState( ( (IComObjectRef<ID3D12PipelineState>)pPipelineState ).COMObject ) ;

	public void ResourceBarrier( uint NumBarriers,
								 ResourceBarrier[ ] pBarriers ) {
		var commandList = COMObject ?? throw new NullReferenceException( ) ;
		unsafe
		{
			var _barriers = new D3D12_RESOURCE_BARRIER[pBarriers.Length];
			for (int i = 0; i < pBarriers.Length; ++i)
			{
				_barriers[i] = pBarriers[i] ;
			}
			var _barriers2 = Unsafe.As< D3D12_RESOURCE_BARRIER[ ] >( pBarriers ) ;
			var cmdList = (ID3D12GraphicsCommandList *) ComPointer!.InterfaceVPtr ;


			var vtable = *( nint**)ComPointer.InterfaceVPtr ;
			var getDescriptor = (delegate* unmanaged[ Stdcall, MemberFunction ]
				< ID3D12GraphicsCommandList*, uint, D3D12_RESOURCE_BARRIER*, void >)( vtable[ 26 ] ) ;

			fixed( D3D12_RESOURCE_BARRIER* barriersPtr = &_barriers2[ 0] )
				getDescriptor( cmdList, NumBarriers, barriersPtr ) ;
		}
	}


	public void ExecuteBundle( IGraphicsCommandList pCommandList ) {
		var commandList = (IComObjectRef< ID3D12GraphicsCommandList >)pCommandList ;
		COMObject!.ExecuteBundle( commandList.COMObject ) ;
	}


	public void SetDescriptorHeaps( uint NumDescriptorHeaps, IDescriptorHeap[ ] ppDescriptorHeaps ) {
		ID3D12DescriptorHeap[ ] descriptorHeaps = new ID3D12DescriptorHeap[ NumDescriptorHeaps ] ;
		
		for( int i = 0; i < NumDescriptorHeaps; ++i )
			descriptorHeaps[ i ] = ( (IComObjectRef<ID3D12DescriptorHeap>)ppDescriptorHeaps[ i ] ).COMObject 
										?? throw new NullReferenceException() ;

		COMObject!.SetDescriptorHeaps( NumDescriptorHeaps, descriptorHeaps ) ;
	}

	
	public void SetComputeRootSignature( IRootSignature pRootSignature ) => COMObject!.SetComputeRootSignature( pRootSignature.COMObject ) ;

	
	public void SetGraphicsRootSignature( IRootSignature pRootSignature ) => COMObject!.SetGraphicsRootSignature( pRootSignature.COMObject ) ;

	
	public void SetComputeRootDescriptorTable( uint RootParameterIndex, GPUDescriptorHandle BaseDescriptor ) => 
		COMObject!.SetComputeRootDescriptorTable( RootParameterIndex, BaseDescriptor ) ;

	
	public void SetGraphicsRootDescriptorTable( uint RootParameterIndex, GPUDescriptorHandle BaseDescriptor ) => 
		COMObject!.SetGraphicsRootDescriptorTable( RootParameterIndex, BaseDescriptor ) ;

	
	public void SetComputeRoot32BitConstant( uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues ) => 
		COMObject!.SetComputeRoot32BitConstant( RootParameterIndex, SrcData, DestOffsetIn32BitValues ) ;

	
	public void SetGraphicsRoot32BitConstant( uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues ) => 
		COMObject!.SetGraphicsRoot32BitConstant( RootParameterIndex, SrcData, DestOffsetIn32BitValues ) ;

	
	public void SetComputeRoot32BitConstants( uint RootParameterIndex, uint Num32BitValuesToSet, 
											  nint pSrcData, uint DestOffsetIn32BitValues ) {
		unsafe {
			COMObject!.SetComputeRoot32BitConstants( RootParameterIndex, 
													 Num32BitValuesToSet, 
													 (void *)pSrcData,
													 DestOffsetIn32BitValues ) ;
		}
	}
	

	public void SetGraphicsRoot32BitConstants( uint RootParameterIndex, uint Num32BitValuesToSet, 
											   nint pSrcData, uint DestOffsetIn32BitValues ) {
		unsafe {
			COMObject!.SetGraphicsRoot32BitConstants( RootParameterIndex, 
													  Num32BitValuesToSet, 
													  (void *)pSrcData,
													  DestOffsetIn32BitValues ) ;
		}
	}
	

	public void SetComputeRootConstantBufferView( uint RootParameterIndex, ulong BufferLocation ) => 
		COMObject!.SetComputeRootConstantBufferView( RootParameterIndex, BufferLocation ) ;

	
	public void SetGraphicsRootConstantBufferView( uint RootParameterIndex, ulong BufferLocation ) => 
		COMObject!.SetGraphicsRootConstantBufferView( RootParameterIndex, BufferLocation ) ;
	

	public void SetComputeRootShaderResourceView( uint RootParameterIndex, ulong BufferLocation ) => 
		COMObject!.SetComputeRootShaderResourceView( RootParameterIndex, BufferLocation ) ;
	

	public void SetGraphicsRootShaderResourceView( uint RootParameterIndex, ulong BufferLocation ) => 
		COMObject!.SetGraphicsRootShaderResourceView( RootParameterIndex, BufferLocation ) ;
	
	public void SetComputeRootUnorderedAccessView( uint RootParameterIndex, ulong BufferLocation ) => 
		COMObject!.SetComputeRootUnorderedAccessView( RootParameterIndex, BufferLocation ) ;
	

	public void SetGraphicsRootUnorderedAccessView( uint RootParameterIndex, ulong BufferLocation ) => 
		COMObject!.SetGraphicsRootUnorderedAccessView( RootParameterIndex, BufferLocation ) ;
	

	public void IASetIndexBuffer( [Optional] in IndexBufferView pView ) {
		unsafe {
			fixed ( IndexBufferView* viewPtr = &pView )
				COMObject!.IASetIndexBuffer( (D3D12_INDEX_BUFFER_VIEW*)viewPtr ) ;
		}
	}
	
	
	public void IASetVertexBuffers( uint StartSlot, uint NumViews, [Optional] Span< VertexBufferView > pViews ) {
		unsafe {
			fixed ( VertexBufferView* pViewsPtr = pViews )
				COMObject!.IASetVertexBuffers( StartSlot, NumViews, (D3D12_VERTEX_BUFFER_VIEW*)pViewsPtr ) ;
		}
	}
	
	
	public void SOSetTargets( uint StartSlot, uint NumViews, [Optional] Span< StreamOutputBufferView > pViews ) {
		unsafe {
			fixed ( StreamOutputBufferView* pViewsPtr = pViews )
				COMObject!.SOSetTargets( StartSlot, NumViews, (D3D12_STREAM_OUTPUT_BUFFER_VIEW*)pViewsPtr ) ;
		}
	}
	
	
	public void OMSetRenderTargets( uint                                   NumRenderTargetDescriptors,
									[Optional] Span< CPUDescriptorHandle > pRenderTargetDescriptors,
									bool                                   RTsSingleHandleToDescriptorRange,
									[Optional] Span< CPUDescriptorHandle > pDepthStencilDescriptor ) {
		unsafe {
			fixed ( CPUDescriptorHandle* pRenderTargetDescriptorsPtr = pRenderTargetDescriptors,
				   pDepthStencilDescriptorPtr = pDepthStencilDescriptor )
				COMObject!.OMSetRenderTargets( NumRenderTargetDescriptors,
											   (D3D12_CPU_DESCRIPTOR_HANDLE*)pRenderTargetDescriptorsPtr,
											   RTsSingleHandleToDescriptorRange,
											   (D3D12_CPU_DESCRIPTOR_HANDLE*)pDepthStencilDescriptorPtr ) ;
		}
	}


	public void ClearDepthStencilView( CPUDescriptorHandle DepthStencilView, 
									   ClearFlags          clearFlags, 
									   float               Depth, 
									   byte                Stencil,
									   uint                NumRects,
									   Span< Rect >        pRects ) {
		unsafe {
			fixed ( Rect* pRectsPtr = pRects ) {
				COMObject!.ClearDepthStencilView( DepthStencilView, (D3D12_CLEAR_FLAGS)clearFlags,
												  Depth, Stencil, NumRects, (RECT*)pRectsPtr ) ;
			}
		}
	}


	public void ClearRenderTargetView( CPUDescriptorHandle RenderTargetView, 
									   float[ ]            ColorRGBA,
									   uint                NumRects,
									   Span< Rect >        pRects = default ) {
		if( pRects is not { Length: > 0 } )
			pRects = default ;

		unsafe {
			fixed ( Rect* pRectsPtr = ( pRects == default ? null : pRects ) ) {
				COMObject!.ClearRenderTargetView( RenderTargetView,
												  ColorRGBA,
												  pRectsPtr is null ? 0 : NumRects,
												  (RECT*)pRectsPtr ) ;
			}
		}
	}


	public void ClearUnorderedAccessViewUint( GPUDescriptorHandle ViewGPUHandleInCurrentHeap,
											  CPUDescriptorHandle ViewCPUHandle,
											  IResource           pResource,
											  uint[ ]             Values, 
											  uint NumRects, 
											  in Span< Rect > pRects ) {
		var resource = (Resource)pResource ;
		unsafe { fixed ( Rect* pRectsPtr = pRects )
				COMObject!.ClearUnorderedAccessViewUint( ViewGPUHandleInCurrentHeap,
														 ViewCPUHandle,
														 resource.COMObject,
														 Values, NumRects, (RECT*)pRectsPtr ) ;
		}
	}

	
	public void ClearUnorderedAccessViewFloat( GPUDescriptorHandle ViewGPUHandleInCurrentHeap,
											   CPUDescriptorHandle ViewCPUHandle,
											   IResource           pResource,
											   float[ ]            Values, 
											   uint NumRects, 
											   in Span< Rect > pRects ) {
		var resource = (Resource)pResource ;
		unsafe { fixed( Rect* pRectsPtr = pRects )
				COMObject!.ClearUnorderedAccessViewFloat( ViewGPUHandleInCurrentHeap, 
														  ViewCPUHandle, resource.COMObject,
														  Values, NumRects, (RECT *)pRectsPtr ) ;
		}
	}


	public void DiscardResource( IResource pResource, [Optional] in Span< DiscardRegion > pRegion ) {
		var resource = (Resource)pResource ;
		unsafe {
			fixed( DiscardRegion* pRegionPtr = pRegion ) {
				COMObject!.DiscardResource( resource.COMObject, (D3D12_DISCARD_REGION*)pRegionPtr ) ;
			}
		}
	}
	
	
	public void BeginQuery( IQueryHeap pQueryHeap, QueryType Type, uint Index ) {
		var queryHeap = (IComObjectRef< ID3D12QueryHeap >)pQueryHeap 
						?? throw new NullReferenceException( ) ;
		COMObject!.BeginQuery( queryHeap.COMObject, (D3D12_QUERY_TYPE)Type, Index ) ;
	}
	
	
	public void EndQuery( IQueryHeap pQueryHeap, QueryType Type, uint Index ) {
		var queryHeap = (IComObjectRef< ID3D12QueryHeap >)pQueryHeap 
						?? throw new NullReferenceException( ) ;
		COMObject!.EndQuery( queryHeap.COMObject, (D3D12_QUERY_TYPE)Type, Index ) ;
	}

	
	public void ResolveQueryData( IQueryHeap pQueryHeap, 
								  QueryType  Type, 
								  uint       StartIndex, 
								  uint       NumQueries, 
								  IResource  pDestinationBuffer, 
								  ulong      AlignedDestinationBufferOffset ) {
		var queryHeap = (IComObjectRef< ID3D12QueryHeap >)pQueryHeap 
						?? throw new NullReferenceException( ) ;
		var resource = (Resource)pDestinationBuffer ;
		COMObject!.ResolveQueryData( queryHeap.COMObject, (D3D12_QUERY_TYPE)Type, 
									 StartIndex, NumQueries,
									 resource.COMObject, AlignedDestinationBufferOffset ) ;
	}

	
	public void SetPredication( IResource pBuffer, ulong AlignedBufferOffset, PredicationOp Operation ) {
		var resource = (Resource)pBuffer ;
		COMObject!.SetPredication( resource.COMObject, AlignedBufferOffset, (D3D12_PREDICATION_OP)Operation ) ;
	}
	
	public void SetMarker(uint Metadata, [Optional] nint pData, uint Size ) {
		unsafe {
			COMObject!.SetMarker( Metadata, (void*)pData, Size ) ;
		}
	}
	
	public void BeginEvent( uint Metadata, [Optional] nint pData, uint Size ) {
		unsafe {
			COMObject!.BeginEvent( Metadata, (void*)pData, Size ) ;
		}
	}

	public void EndEvent( ) => COMObject!.EndEvent( ) ;
	
	public void ExecuteIndirect( ICommandSignature pCommandSignature,
								 uint              MaxCommandCount,
								 IResource         pArgumentBuffer,
								 ulong             ArgumentBufferOffset,
								 IResource         pCountBuffer,
								 ulong             CountBufferOffset ) {
		var commandSignature = (IComObjectRef< ID3D12CommandSignature >)pCommandSignature ;
		var argumentBuffer = (Resource)pArgumentBuffer ;
		var countBuffer = (Resource)pCountBuffer ;
		COMObject!.ExecuteIndirect( commandSignature.COMObject, MaxCommandCount, 
									argumentBuffer.COMObject, ArgumentBufferOffset, 
									countBuffer.COMObject, CountBufferOffset ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// =====================================================================================
} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList1 ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList1))]
internal class GraphicsCommandList1: GraphicsCommandList,
								   IGraphicsCommandList1,
								   IComObjectRef< ID3D12GraphicsCommandList1 >,
								   IUnknownWrapper< ID3D12GraphicsCommandList1 > {
	
	ComPtr< ID3D12GraphicsCommandList1 >? _comPtr ;
	public new ComPtr< ID3D12GraphicsCommandList1 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12GraphicsCommandList1>( ) ;
	public override ID3D12GraphicsCommandList1? COMObject => ComPointer?.Interface ;
	
	
	internal GraphicsCommandList1( ) {
		_comPtr = ComResources?.GetPointer< ID3D12GraphicsCommandList1 >( ) ;
	}
	internal GraphicsCommandList1( ComPtr< ID3D12GraphicsCommandList1 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList1( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList1( ID3D12GraphicsCommandList1 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	
	
	public void AtomicCopyBufferUINT( IResource                 pDstBuffer,
									  ulong                     DstOffset,
									  IResource                 pSrcBuffer,
									  ulong                     SrcOffset,
									  uint                      Dependencies,
									  IResource[ ]              ppDependentResources,
									  in SubresourceRangeUInt64 pDependentSubresourceRanges ) {
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		var src = (Resource)pSrcBuffer ;
		var dst = (Resource)pDstBuffer ;
		
		// Copy dependent resources to an array (optimize later):
		ID3D12Resource[ ] dependentResources = new ID3D12Resource[ ppDependentResources.Length ] ;
		for ( int i = 0; i < dependentResources.Length; ++i ) {
			dependentResources[ i ] = ( (Resource)ppDependentResources[ i ] ).COMObject
#if DEBUG || DEBUG_COM || DEV_BUILD
									  ?? throw new ArgumentException( $"{nameof(IGraphicsCommandList1)}.{nameof(AtomicCopyBufferUINT)} :: " +
																	  $"Collection of dependent resources cannot contain null values!" )
#endif
				;
		}
		
		unsafe {
			fixed(SubresourceRangeUInt64* pDependentRanges = &pDependentSubresourceRanges)
				cmdList.AtomicCopyBufferUINT( dst.COMObject, DstOffset,
											  src.COMObject, SrcOffset,
											  Dependencies, dependentResources,
											  (D3D12_SUBRESOURCE_RANGE_UINT64 *)pDependentRanges ) ;
		}
	}


	public void AtomicCopyBufferUINT64( IResource                 pDstBuffer,
										ulong                     DstOffset,
										IResource                 pSrcBuffer,
										ulong                     SrcOffset,
										uint                      Dependencies,
										IResource[ ]              ppDependentResources,
										in SubresourceRangeUInt64 pDependentSubresourceRanges ) {
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		var src = (Resource)pSrcBuffer ;
		var dst = (Resource)pDstBuffer ;
		
		// Copy dependent resources to an array (optimize later):
		ID3D12Resource[ ] dependentResources = new ID3D12Resource[ ppDependentResources.Length ] ;
		for ( int i = 0; i < dependentResources.Length; ++i ) {
			dependentResources[ i ] = ( (Resource)ppDependentResources[ i ] ).COMObject
#if DEBUG || DEBUG_COM || DEV_BUILD
									  ?? throw new ArgumentException( $"{nameof(IGraphicsCommandList1)}.{nameof(AtomicCopyBufferUINT)} :: " +
																	  $"Collection of dependent resources cannot contain null values!" )
#endif
				;
		}
		
		unsafe {
			fixed(SubresourceRangeUInt64* pDependentRanges = &pDependentSubresourceRanges)
				cmdList.AtomicCopyBufferUINT64( dst.COMObject, DstOffset,
												src.COMObject, SrcOffset,
												Dependencies, dependentResources,
												(D3D12_SUBRESOURCE_RANGE_UINT64 *)pDependentRanges ) ;
		}
	}
	

	public void OMSetDepthBounds( float Min, float Max ) => COMObject!.OMSetDepthBounds( Min, Max ) ;
	

	public void SetSamplePositions( uint NumSamplesPerPixel, uint NumPixels, in SamplePosition pSamplePositions ) {
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		
		unsafe {
			fixed( SamplePosition* pSamplePos = &pSamplePositions )
				cmdList.SetSamplePositions( NumSamplesPerPixel, NumPixels, (D3D12_SAMPLE_POSITION *)pSamplePos ) ;
		}
	}

	
	public void ResolveSubresourceRegion( IResource           pDstResource,
										  uint                DstSubresource,
										  uint                DstX,
										  uint                DstY,
										  IResource           pSrcResource,
										  uint                SrcSubresource,
										  [Optional] in Rect? pSrcRect,
										  Format              format,
										  ResolveMode         resolveMode ) {
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		var src = (Resource)pSrcResource ;
		var dst = (Resource)pDstResource ;
		
		unsafe {
			Unsafe.SkipInit( out RECT srcRect ) ;
			RECT* srcRectPtr = null ;
			if ( pSrcRect.HasValue ) {
				srcRect = pSrcRect.Value ;
				srcRectPtr = &srcRect ;
			}
			
			cmdList.ResolveSubresourceRegion( dst.COMObject, DstSubresource, DstX, DstY,
											  src.COMObject, SrcSubresource, srcRectPtr,
											  (DXGI_FORMAT)format, (D3D12_RESOLVE_MODE)resolveMode ) ;
		}
	}

	
	public void SetViewInstanceMask( uint Mask ) => COMObject!.SetViewInstanceMask( Mask ) ;
	
	// ---------------------------------------------------------------------------------
	// Static Interface Members:
	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList1 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList1).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList2 ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList2))]
internal class GraphicsCommandList2: GraphicsCommandList1,
									 IGraphicsCommandList2,
									 IComObjectRef< ID3D12GraphicsCommandList2 >,
									 IUnknownWrapper< ID3D12GraphicsCommandList2 > {
	// ---------------------------------------------------------------------------------
	
	ComPtr< ID3D12GraphicsCommandList2 >? _comPtr ;
	public new ComPtr< ID3D12GraphicsCommandList2 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12GraphicsCommandList2>( ) ;
	public override ID3D12GraphicsCommandList2? COMObject => ComPointer?.Interface ;
	
	// ---------------------------------------------------------------------------------
	
	internal GraphicsCommandList2( ) {
		_comPtr = ComResources?.GetPointer< ID3D12GraphicsCommandList2 >( ) ;
	}
	internal GraphicsCommandList2( ComPtr< ID3D12GraphicsCommandList2 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList2( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList2( ID3D12GraphicsCommandList2 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------------

	public void WriteBufferImmediate( uint Count, in Span< WriteBufferImmediateParameter > pParams,
									  Span< WriteBufferImmediateMode > pModes = default ) {
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			D3D12_WRITEBUFFERIMMEDIATE_MODE* modesPtr = null ;
			fixed ( void* _params = &pParams[ 0 ], modes = &pModes[ 0 ] ) {
				if ( pModes is { Length: > 0 } ) modesPtr = (D3D12_WRITEBUFFERIMMEDIATE_MODE*)modes ;
				cmdList.WriteBufferImmediate( Count, (D3D12_WRITEBUFFERIMMEDIATE_PARAMETER*)_params, modesPtr ) ;
			}
		}
	}
	
	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList2 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList2).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList3 ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList3))]
internal class GraphicsCommandList3: GraphicsCommandList2,
									 IGraphicsCommandList3,
									 IComObjectRef< ID3D12GraphicsCommandList3 >,
									 IUnknownWrapper< ID3D12GraphicsCommandList3 > {
	// ---------------------------------------------------------------------------------
	
	ComPtr< ID3D12GraphicsCommandList3 >? _comPtr ;
	public new ComPtr< ID3D12GraphicsCommandList3 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12GraphicsCommandList3>( ) ;

	public override ID3D12GraphicsCommandList3? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	internal GraphicsCommandList3( ) {
		_comPtr = ComResources?.GetPointer< ID3D12GraphicsCommandList3 >( ) ;
	}
	internal GraphicsCommandList3( ComPtr< ID3D12GraphicsCommandList3 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList3( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList3( ID3D12GraphicsCommandList3 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public void SetProtectedResourceSession( IProtectedResourceSession pProtectedResourceSession ) {
		var protectedResourceSession = (IComObjectRef< ID3D12ProtectedResourceSession >)pProtectedResourceSession ;
		COMObject!.SetProtectedResourceSession( protectedResourceSession.COMObject ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList3 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList3 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList4 ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList4))]
internal class GraphicsCommandList4: GraphicsCommandList3,
									 IGraphicsCommandList4,
									 IComObjectRef< ID3D12GraphicsCommandList4 >,
									 IUnknownWrapper< ID3D12GraphicsCommandList4 > {
	// ---------------------------------------------------------------------------------
	
	ComPtr< ID3D12GraphicsCommandList4 >? _comPtr ;
	public new ComPtr< ID3D12GraphicsCommandList4 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12GraphicsCommandList4>( ) ;

	public override ID3D12GraphicsCommandList4? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	internal GraphicsCommandList4( ) {
		_comPtr = ComResources?.GetPointer< ID3D12GraphicsCommandList4 >( ) ;
	}
	internal GraphicsCommandList4( ComPtr< ID3D12GraphicsCommandList4 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList4( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList4( ID3D12GraphicsCommandList4 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public void BeginRenderPass( uint NumRenderTargets,
								 RenderPassRenderTargetDescription[ ]  pRenderTargets,
								 in RenderPassDepthStencilDescription? pDepthStencil = null,
								 RenderPassFlags Flags = RenderPassFlags.None ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ArgumentNullException.ThrowIfNull( pRenderTargets, nameof(pRenderTargets) ) ;
#endif
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		var descArray = Unsafe.As< D3D12_RENDER_PASS_RENDER_TARGET_DESC[ ] >( pRenderTargets ) ;
		cmdList.BeginRenderPass( NumRenderTargets, descArray,
								 (pDepthStencil ?? default ),
								 (D3D12_RENDER_PASS_FLAGS)Flags ) ;
	}

	
	public void EndRenderPass( ) => COMObject!.EndRenderPass( ) ;
	

	public void InitializeMetaCommand( IMetaCommand    pMetaCommand,
									   [Optional] nint pInitializationParametersData,
									   nuint           InitializationParametersDataSizeInBytes = 0x00U ) {
		unsafe {
			var metaCommand = (IComObjectRef< ID3D12MetaCommand >)pMetaCommand ;
			COMObject!.InitializeMetaCommand( metaCommand.COMObject, (void*)pInitializationParametersData,
											  InitializationParametersDataSizeInBytes ) ;
		}
	}


	public void ExecuteMetaCommand( IMetaCommand    pMetaCommand,
									[Optional] nint pExecutionParametersData,
									nuint           ExecutionParametersDataSizeInBytes = 0x00U ) {
		unsafe {
			var metaCommand = (IComObjectRef< ID3D12MetaCommand >)pMetaCommand ;
			COMObject!.ExecuteMetaCommand( metaCommand.COMObject, (void *)pExecutionParametersData,
										   ExecutionParametersDataSizeInBytes ) ;
		}
	}


	public void BuildRaytracingAccelerationStructure( in BuildRaytracingAccelerationStructureDescription pDesc,
													  uint NumPostbuildInfoDescs,
													  [Optional] Span< RaytracingAccelerationStructurePostBuildInfoDescription > 
														  pPostbuildInfoDescs ) {
		
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		
		unsafe {
			fixed( BuildRaytracingAccelerationStructureDescription* pDescPtr = &pDesc ) {
				fixed ( RaytracingAccelerationStructurePostBuildInfoDescription* pPostbuildInfoDescsPtr =
						   pPostbuildInfoDescs ) {
					cmdList
						.BuildRaytracingAccelerationStructure( (D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_DESC*)pDescPtr,
															   NumPostbuildInfoDescs,
															   (D3D12_RAYTRACING_ACCELERATION_STRUCTURE_POSTBUILD_INFO_DESC
																   *)pPostbuildInfoDescsPtr ) ;
				}
			}
		}
	}


	public void EmitRaytracingAccelerationStructurePostbuildInfo( in RaytracingAccelerationStructurePostBuildInfoDescription pDesc,
																  uint NumSourceAccelerationStructures,
																  ulong[ ] pSourceAccelerationStructureData ) {
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( RaytracingAccelerationStructurePostBuildInfoDescription* descPtr = &pDesc ) {
				cmdList.EmitRaytracingAccelerationStructurePostbuildInfo( (D3D12_RAYTRACING_ACCELERATION_STRUCTURE_POSTBUILD_INFO_DESC *) descPtr, 
																		  NumSourceAccelerationStructures, pSourceAccelerationStructureData ) ;
			}
		}
	}


	public void CopyRaytracingAccelerationStructure( ulong DestAccelerationStructureData,
													 ulong SourceAccelerationStructureData,
													 RaytracingAccelerationStructureCopyMode Mode ) {
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		cmdList.CopyRaytracingAccelerationStructure( DestAccelerationStructureData, SourceAccelerationStructureData,
													 ( D3D12_RAYTRACING_ACCELERATION_STRUCTURE_COPY_MODE ) Mode ) ;
	}

	
	public void SetPipelineState1( IStateObject pStateObject ) {
		var stateObject = (IComObjectRef< ID3D12StateObject >)pStateObject ;
		COMObject!.SetPipelineState1( stateObject.COMObject ) ;
	}


	public void DispatchRays( in DispatchRaysDescription pDesc ) {
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( DispatchRaysDescription* descPtr = &pDesc ) {
				cmdList.DispatchRays( (D3D12_DISPATCH_RAYS_DESC *)descPtr ) ;
			}
		}
	}
	
	
	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList4 ) ;

	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList4 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList5 ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList5))]
internal class GraphicsCommandList5: GraphicsCommandList4,
									 IGraphicsCommandList5,
									 IComObjectRef< ID3D12GraphicsCommandList5 >,
									 IUnknownWrapper< ID3D12GraphicsCommandList5 > {
	// ---------------------------------------------------------------------------------
	
	ComPtr< ID3D12GraphicsCommandList5 >? _comPtr ;
	public new ComPtr< ID3D12GraphicsCommandList5 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12GraphicsCommandList5>( ) ;

	public override ID3D12GraphicsCommandList5? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	internal GraphicsCommandList5( ) {
		_comPtr = ComResources?.GetPointer< ID3D12GraphicsCommandList5 >( ) ;
	}
	internal GraphicsCommandList5( ComPtr< ID3D12GraphicsCommandList5 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList5( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList5( ID3D12GraphicsCommandList5 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public void RSSetShadingRate( ShadingRate baseShadingRate, 
								  [Optional] Span< ShadingRateCombiner > combiners ) {
		var cmdList = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			if( combiners is { Length: > 0 } ) {
				D3D12_SHADING_RATE_COMBINER* combinersPtr = null ;
				fixed ( ShadingRateCombiner* combinerSpanPtr = combiners ) {
					combinersPtr = (D3D12_SHADING_RATE_COMBINER*)combinerSpanPtr ;
					cmdList.RSSetShadingRate( (D3D12_SHADING_RATE)baseShadingRate,
											  combinersPtr ) ;
				}
			}
			else cmdList.RSSetShadingRate( (D3D12_SHADING_RATE)baseShadingRate, null ) ;
		}
	}

	
	public void RSSetShadingRateImage( IResource shadingRateImage ) {
		var resource = (IComObjectRef< ID3D12Resource >)shadingRateImage ;
		COMObject!.RSSetShadingRateImage( resource.COMObject ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList5 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList5 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList6 ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList6))]
internal class GraphicsCommandList6: GraphicsCommandList5,
									 IGraphicsCommandList6,
									 IComObjectRef< ID3D12GraphicsCommandList6 >,
									 IUnknownWrapper< ID3D12GraphicsCommandList6 > {
	// ---------------------------------------------------------------------------------
	
	ComPtr< ID3D12GraphicsCommandList6 >? _comPtr ;
	public new ComPtr< ID3D12GraphicsCommandList6 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12GraphicsCommandList6>( ) ;

	public override ID3D12GraphicsCommandList6? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	internal GraphicsCommandList6( ) {
		_comPtr = ComResources?.GetPointer< ID3D12GraphicsCommandList6 >( ) ;
	}
	internal GraphicsCommandList6( ComPtr< ID3D12GraphicsCommandList6 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList6( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList6( ID3D12GraphicsCommandList6 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public void DispatchMesh( uint ThreadGroupCountX, uint ThreadGroupCountY, uint ThreadGroupCountZ ) =>
		COMObject!.DispatchMesh( ThreadGroupCountX, ThreadGroupCountY, ThreadGroupCountZ ) ;
	
	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList6 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12GraphicsCommandList6 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList7 ::
// ------------------------------------------------------------------------------------------

[Wrapper( typeof( ID3D12GraphicsCommandList7 ) )]
internal class GraphicsCommandList7: GraphicsCommandList6,
									 IGraphicsCommandList7,
									 IComObjectRef< ID3D12GraphicsCommandList7 >,
									 IUnknownWrapper< ID3D12GraphicsCommandList7 > {
	// ---------------------------------------------------------------------------------

	ComPtr< ID3D12GraphicsCommandList7 >? _comPtr ;
	
	public new ComPtr< ID3D12GraphicsCommandList7 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12GraphicsCommandList7 >( ) ;

	public override ID3D12GraphicsCommandList7? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	internal GraphicsCommandList7( ) {
		_comPtr = ComResources?.GetPointer< ID3D12GraphicsCommandList7 >( ) ;
	}

	internal GraphicsCommandList7( ComPtr< ID3D12GraphicsCommandList7 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}

	internal GraphicsCommandList7( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}

	internal GraphicsCommandList7( ID3D12GraphicsCommandList7 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}

	// ---------------------------------------------------------------------------------

	public void Barrier( uint NumBarrierGroups, BarrierGroup[ ] pBarrierGroups ) {
		var cmdList       = COMObject ?? throw new NullReferenceException( ) ;
		var barrierGroups = Unsafe.As< D3D12_BARRIER_GROUP[ ] >( pBarrierGroups ) ;
		cmdList.Barrier( NumBarrierGroups, barrierGroups ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof( ID3D12GraphicsCommandList7 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12GraphicsCommandList7 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	
	// ==================================================================================
} ;



// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList8 ::
// ------------------------------------------------------------------------------------------

[Wrapper( typeof( ID3D12GraphicsCommandList8 ) )]
internal class GraphicsCommandList8: GraphicsCommandList7,
									 IGraphicsCommandList8,
									 IComObjectRef< ID3D12GraphicsCommandList8 >,
									 IUnknownWrapper< ID3D12GraphicsCommandList8 > {
	// ---------------------------------------------------------------------------------

	ComPtr< ID3D12GraphicsCommandList8 >? _comPtr ;

	public new ComPtr< ID3D12GraphicsCommandList8 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12GraphicsCommandList8 >( ) ;

	public override ID3D12GraphicsCommandList8? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	internal GraphicsCommandList8( ) {
		_comPtr = ComResources?.GetPointer< ID3D12GraphicsCommandList8 >( ) ;
	}
	internal GraphicsCommandList8( ComPtr< ID3D12GraphicsCommandList8 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList8( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList8( ID3D12GraphicsCommandList8 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------------

	public void OMSetFrontAndBackStencilRef( uint FrontStencilRef, uint BackStencilRef ) => 
		COMObject!.OMSetFrontAndBackStencilRef( FrontStencilRef, BackStencilRef ) ;

	// ---------------------------------------------------------------------------------

	public new static Type ComType => typeof( ID3D12GraphicsCommandList8 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12GraphicsCommandList8 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}

	// ==========================================================================================
} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList9 ::
// ------------------------------------------------------------------------------------------

[Wrapper( typeof( ID3D12GraphicsCommandList9 ) )]
internal class GraphicsCommandList9: GraphicsCommandList8,
									 IGraphicsCommandList9,
									 IComObjectRef< ID3D12GraphicsCommandList9 >,
									 IUnknownWrapper< ID3D12GraphicsCommandList9 > {
	// ---------------------------------------------------------------------------------

	ComPtr< ID3D12GraphicsCommandList9 >? _comPtr ;

	public new ComPtr< ID3D12GraphicsCommandList9 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12GraphicsCommandList9 >( ) ;

	public override ID3D12GraphicsCommandList9? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	internal GraphicsCommandList9( ) {
		_comPtr = ComResources?.GetPointer< ID3D12GraphicsCommandList9 >( ) ;
	}
	internal GraphicsCommandList9( ComPtr< ID3D12GraphicsCommandList9 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList9( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal GraphicsCommandList9( ID3D12GraphicsCommandList9 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}

	// ---------------------------------------------------------------------------------

	public void RSSetDepthBias( float DepthBias, float DepthBiasClamp, float SlopeScaledDepthBias ) =>
		COMObject!.RSSetDepthBias( DepthBias, DepthBiasClamp, SlopeScaledDepthBias ) ;

	public void IASetIndexBufferStripCutValue( IndexBufferStripCutValue IBStripCutValue ) =>
		COMObject!.IASetIndexBufferStripCutValue( ( D3D12_INDEX_BUFFER_STRIP_CUT_VALUE ) IBStripCutValue ) ;

	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof( ID3D12GraphicsCommandList9 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12GraphicsCommandList9 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	
	// =================================================================================
} ;

// ==========================================================================================