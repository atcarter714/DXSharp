// ReSharper disable IdentifierTypo
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

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
	#region Recycle Array Caches
	//! Array of 1 element for use in single-transition calls (avoids allocations):
	static readonly ResourceBarrier[ ] _singleTransitionCache = new ResourceBarrier[ 1 ] ;
	//! Array of 1 element for use in single-descriptor calls (avoids allocations):
	protected static readonly CPUDescriptorHandle[ ] _singleCPUDescHandleCache 
		= new CPUDescriptorHandle[ 1 ] ;
		#endregion
	
	
	ComPtr< ID3D12GraphicsCommandList >? _comPtr ;
	public new ComPtr< ID3D12GraphicsCommandList >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12GraphicsCommandList >( ) ;
	public override ID3D12GraphicsCommandList? ComObject => ComPointer?.Interface ;
	
	
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
	
	protected float[ ] _clearColorCache = new float[ 4 ] ;
	// ---------------------------------------------------------------------------------

	public void Close( ) => ComObject?.Close( ) ;


	public void Reset( ICommandAllocator? pAllocator, IPipelineState? pInitialState ) {
		var allocator = (IComObjectRef< ID3D12CommandAllocator >?)pAllocator ;
		var initialState = (IComObjectRef< ID3D12PipelineState >?)pInitialState ;
#if DEBUG || DEBUG_COM || DEV_BUILD
		ArgumentNullException.ThrowIfNull( pAllocator, nameof(pAllocator) ) ;
		ArgumentNullException.ThrowIfNull( allocator?.ComObject, nameof(allocator.ComObject) ) ;
		ArgumentNullException.ThrowIfNull( initialState, nameof(pInitialState) ) ;
		ArgumentNullException.ThrowIfNull( initialState.ComObject, nameof(initialState.ComObject) ) ;
#endif
		
		var device = ComObject ?? throw new NullReferenceException( nameof( IGraphicsCommandList ) ) ;
		device.Reset( allocator.ComObject, initialState.ComObject ) ;
	}

	
	public void ClearState( ID3D12PipelineState pPipelineState ) => ComObject!.ClearState( pPipelineState ) ;

	
	public void DrawInstanced( uint VertexCountPerInstance, uint InstanceCount, 
							   uint StartVertexLocation,    uint StartInstanceLocation ) => 
		ComObject!.DrawInstanced( VertexCountPerInstance, InstanceCount, 
								  StartVertexLocation, StartInstanceLocation ) ;
	

	public void DrawIndexedInstanced( uint IndexCountPerInstance, uint InstanceCount, 
									  uint StartIndexLocation,    int  BaseVertexLocation, 
									  uint StartInstanceLocation ) => 
		ComObject!.DrawIndexedInstanced( IndexCountPerInstance, InstanceCount,
										 StartIndexLocation, BaseVertexLocation,
										 StartInstanceLocation ) ;

	
	public void Dispatch( uint ThreadGroupCountX, uint ThreadGroupCountY, uint ThreadGroupCountZ ) => 
		ComObject!.Dispatch( ThreadGroupCountX, ThreadGroupCountY, ThreadGroupCountZ ) ;

	
	public void CopyBufferRegion( IResource pDstBuffer, ulong DstOffset, 
								  IResource pSrcBuffer, ulong SrcOffset, 
								  ulong     NumBytes ) {
		var src = (Resource)pSrcBuffer ;
		var dst = (Resource)pDstBuffer ;
		ComObject!.CopyBufferRegion( dst.ComObject, DstOffset,
									 src.ComObject, SrcOffset,
									 NumBytes ) ;
	}
	

	public void CopyTextureRegion( in TextureCopyLocation pDst, 
								   uint DstX, uint DstY, uint DstZ, 
								   in TextureCopyLocation pSrc,
								   [Optional] in Box pSrcBox ) {
		unsafe { fixed ( Box* pBox = &pSrcBox ) {
				ComObject!.CopyTextureRegion( pDst, DstX, DstY, DstZ, pSrc, (D3D12_BOX *)pBox ) ;
			}
		}
	}
	

	public void CopyResource( IResource pDstResource, IResource pSrcResource ) {
		ArgumentNullException.ThrowIfNull(pDstResource, nameof(pDstResource));
		ArgumentNullException.ThrowIfNull(pSrcResource, nameof(pSrcResource));
		var src = (Resource?)pSrcResource;
		var dst = (Resource?)pDstResource ;
		ComObject!.CopyResource( dst?.ComObject, src?.ComObject ) ;
	}

	
	public void CopyTiles( IResource                  pTiledResource,
						   in TiledResourceCoordinate pTileRegionStartCoordinate, 
						   in TileRegionSize          pTileRegionSize,
						   IResource                  pBuffer,
						   ulong                      bufferStartOffsetInBytes,
						   TileCopyFlags              flags ) {
		unsafe {
			var tiledResource = (Resource)pTiledResource ;
			var buffer = (Resource)pBuffer ;
			fixed( void* pTiledResCoord = &pTileRegionStartCoordinate, pRegionSize = &pTileRegionSize) {
				ComObject!.CopyTiles( tiledResource.ComObject, 
									  (D3D12_TILED_RESOURCE_COORDINATE *)pTiledResCoord, 
									  (D3D12_TILE_REGION_SIZE *)pRegionSize,
									  buffer.ComObject, 
									  bufferStartOffsetInBytes, 
									  (D3D12_TILE_COPY_FLAGS)flags ) ;
			}
		}
	}

	
	public void ResolveSubresource( IResource pDstResource, uint DstSubresource, 
									IResource pSrcResource, uint SrcSubresource, 
									Format    Format) {
		var src = (Resource)pSrcResource ;
		var dst = (Resource)pDstResource ;
		ComObject!.ResolveSubresource( dst.ComObject, DstSubresource,
									   src.ComObject, SrcSubresource,
									   (DXGI_FORMAT)Format ) ;
	}

	
	public void IASetPrimitiveTopology( PrimitiveTopology PrimitiveTopology ) => 
		ComObject!.IASetPrimitiveTopology( (D3D_PRIMITIVE_TOPOLOGY)PrimitiveTopology ) ;

	
	public void RSSetViewports( uint NumViewports, in Span< Viewport > pViewports ) {
		unsafe {
			fixed( Viewport* pViewportsPtr = &pViewports[0] ) {
				ComObject!.RSSetViewports( NumViewports, (D3D12_VIEWPORT *)pViewportsPtr ) ;
			}
		}
	}

	
	public void RSSetScissorRects( uint NumRects, in Span< Rect > pRects ) {
		unsafe { fixed( Rect* pRectsPtr = &pRects[0] ) {
				ComObject!.RSSetScissorRects( NumRects, (RECT *)pRectsPtr ) ;
			}
		}
	}

	
	public void OMSetBlendFactor( float[ ] BlendFactor ) {
		unsafe {
			fixed( float* pBlendF = &BlendFactor[0] )
				ComObject!.OMSetBlendFactor( pBlendF ) ;
		}
	}

	
	public void OMSetStencilRef( uint StencilRef ) => ComObject!.OMSetStencilRef( StencilRef ) ;
	
	public void SetPipelineState( IPipelineState pPipelineState ) => 
		ComObject!.SetPipelineState( ( (IComObjectRef<ID3D12PipelineState>)pPipelineState ).ComObject ) ;
	
	
	public void ResourceBarrier( uint numBarriers, ResourceBarrier[ ] pBarriers ) {
		unsafe {
			var _barriers = new D3D12_RESOURCE_BARRIER[ pBarriers.Length ] ;
			for (int i = 0; i < pBarriers.Length; ++i) {
				_barriers[i] = pBarriers[i] ;
			}
			var _barriers2 = Unsafe.As< D3D12_RESOURCE_BARRIER[ ] >( pBarriers ) ;
			var cmdList = (ID3D12GraphicsCommandList *) ComPointer!.InterfaceVPtr ;


			var vtable = *( nint**)ComPointer.InterfaceVPtr ;
			var getDescriptor = (delegate* unmanaged[ Stdcall, MemberFunction ]
				< ID3D12GraphicsCommandList*, uint, D3D12_RESOURCE_BARRIER*, void >)( vtable[ 26 ] ) ;
			
			fixed( D3D12_RESOURCE_BARRIER* barriersPtr = &_barriers2[ 0] )
				getDescriptor( cmdList, numBarriers, barriersPtr ) ;
		}
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void ResourceBarrier( in ResourceBarrier barrier ) {
		_singleTransitionCache[ 0 ] = barrier ;
		ResourceBarrier( 1, _singleTransitionCache ) ;
	}

	
	public void ExecuteBundle( IGraphicsCommandList pCommandList ) {
		var commandList = (IComObjectRef< ID3D12GraphicsCommandList >)pCommandList ;
		ComObject!.ExecuteBundle( commandList.ComObject ) ;
	}


	public void SetDescriptorHeaps( uint NumDescriptorHeaps, IDescriptorHeap[ ] ppDescriptorHeaps ) {
		ID3D12DescriptorHeap[ ] descriptorHeaps = new ID3D12DescriptorHeap[ NumDescriptorHeaps ] ;
		
		for( int i = 0; i < NumDescriptorHeaps; ++i )
			descriptorHeaps[ i ] = ( (IComObjectRef<ID3D12DescriptorHeap>)ppDescriptorHeaps[ i ] ).ComObject 
										?? throw new NullReferenceException() ;

		ComObject!.SetDescriptorHeaps( NumDescriptorHeaps, descriptorHeaps ) ;
	}

	
	public void SetComputeRootSignature( IRootSignature pRootSignature ) {
		var rootSignature = (IComObjectRef< ID3D12RootSignature >)pRootSignature 
							?? throw new ArgumentNullException( nameof(pRootSignature) ) ;
		ComObject!.SetComputeRootSignature( rootSignature.ComObject ) ;
	}

	
	public void SetGraphicsRootSignature( IRootSignature? pRootSignature ) {
		var rootSignature = (IComObjectRef< ID3D12RootSignature >?)pRootSignature 
							?? throw new ArgumentNullException( nameof(pRootSignature) ) ;
		ComObject!.SetGraphicsRootSignature( rootSignature.ComObject ) ;
	}

	
	public void SetComputeRootDescriptorTable( uint RootParameterIndex, GPUDescriptorHandle BaseDescriptor ) => 
		ComObject!.SetComputeRootDescriptorTable( RootParameterIndex, BaseDescriptor ) ;

	
	public void SetGraphicsRootDescriptorTable( uint RootParameterIndex, GPUDescriptorHandle BaseDescriptor ) => 
		ComObject!.SetGraphicsRootDescriptorTable( RootParameterIndex, BaseDescriptor ) ;

	
	public void SetComputeRoot32BitConstant( uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues ) => 
		ComObject!.SetComputeRoot32BitConstant( RootParameterIndex, SrcData, DestOffsetIn32BitValues ) ;

	
	public void SetGraphicsRoot32BitConstant( uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues ) => 
		ComObject!.SetGraphicsRoot32BitConstant( RootParameterIndex, SrcData, DestOffsetIn32BitValues ) ;

	
	public void SetComputeRoot32BitConstants( uint RootParameterIndex, uint Num32BitValuesToSet, 
											  nint pSrcData, uint DestOffsetIn32BitValues ) {
		unsafe {
			ComObject!.SetComputeRoot32BitConstants( RootParameterIndex, 
													 Num32BitValuesToSet, 
													 (void *)pSrcData,
													 DestOffsetIn32BitValues ) ;
		}
	}
	

	public void SetGraphicsRoot32BitConstants( uint RootParameterIndex, uint Num32BitValuesToSet, 
											   nint pSrcData, uint DestOffsetIn32BitValues ) {
		unsafe {
			ComObject!.SetGraphicsRoot32BitConstants( RootParameterIndex, 
													  Num32BitValuesToSet, 
													  (void *)pSrcData,
													  DestOffsetIn32BitValues ) ;
		}
	}
	

	public void SetComputeRootConstantBufferView( uint RootParameterIndex, ulong BufferLocation ) => 
		ComObject!.SetComputeRootConstantBufferView( RootParameterIndex, BufferLocation ) ;

	
	public void SetGraphicsRootConstantBufferView( uint RootParameterIndex, ulong BufferLocation ) => 
		ComObject!.SetGraphicsRootConstantBufferView( RootParameterIndex, BufferLocation ) ;
	

	public void SetComputeRootShaderResourceView( uint RootParameterIndex, ulong BufferLocation ) => 
		ComObject!.SetComputeRootShaderResourceView( RootParameterIndex, BufferLocation ) ;
	

	public void SetGraphicsRootShaderResourceView( uint RootParameterIndex, ulong BufferLocation ) => 
		ComObject!.SetGraphicsRootShaderResourceView( RootParameterIndex, BufferLocation ) ;
	
	public void SetComputeRootUnorderedAccessView( uint RootParameterIndex, ulong BufferLocation ) => 
		ComObject!.SetComputeRootUnorderedAccessView( RootParameterIndex, BufferLocation ) ;
	

	public void SetGraphicsRootUnorderedAccessView( uint RootParameterIndex, ulong BufferLocation ) => 
		ComObject!.SetGraphicsRootUnorderedAccessView( RootParameterIndex, BufferLocation ) ;
	

	public void IASetIndexBuffer( [Optional] in IndexBufferView pView ) {
		unsafe {
			fixed ( IndexBufferView* viewPtr = &pView )
				ComObject!.IASetIndexBuffer( (D3D12_INDEX_BUFFER_VIEW*)viewPtr ) ;
		}
	}
	
	
	public void IASetVertexBuffers( uint startSlot, uint numViews,
									[Optional] Span< VertexBufferView > pViews ) {
		unsafe {
			fixed ( VertexBufferView* pViewsPtr = pViews )
				ComObject!.IASetVertexBuffers( startSlot, numViews, (D3D12_VERTEX_BUFFER_VIEW*)pViewsPtr ) ;
		}
	}
	
	
	public void SOSetTargets( uint startSlot, uint numViews,
							  [Optional] Span< StreamOutputBufferView > pViews ) {
		unsafe {
			fixed ( StreamOutputBufferView* pViewsPtr = pViews )
				ComObject!.SOSetTargets( startSlot, numViews, (D3D12_STREAM_OUTPUT_BUFFER_VIEW*)pViewsPtr ) ;
		}
	}
	
	
	public void OMSetRenderTargets( uint numRenderTargetDescriptors,
									[Optional] Span< CPUDescriptorHandle > pRenderTargetDescriptors,
									[Optional] bool RTsSingleHandleToDescriptorRange,
									[Optional] CPUDescriptorHandle pDepthStencilDescriptor ) {
		unsafe {
			fixed ( CPUDescriptorHandle* pRenderTargetDescriptorsPtr = pRenderTargetDescriptors ) {
				D3D12_CPU_DESCRIPTOR_HANDLE* pTargetDescHandle =
					pRenderTargetDescriptors is {Length: > 0} ? 
						(D3D12_CPU_DESCRIPTOR_HANDLE *)pRenderTargetDescriptorsPtr : null ;
				
				D3D12_CPU_DESCRIPTOR_HANDLE* pDepthDescHandle = 
					pDepthStencilDescriptor != CPUDescriptorHandle.Null ? 
						(D3D12_CPU_DESCRIPTOR_HANDLE *)&pDepthStencilDescriptor : null ;
				
				ComObject!.OMSetRenderTargets( numRenderTargetDescriptors, pTargetDescHandle,
											   RTsSingleHandleToDescriptorRange, pDepthDescHandle ) ;
			}
		}
	}
	
	public void OMSetRenderTargets( CPUDescriptorHandle pRenderTargetDescriptor,
									bool RTsSingleHandleToDescriptorRange = false,
									CPUDescriptorHandle pDepthStencilDescriptor = default ) {
		_singleCPUDescHandleCache[ 0 ] = pRenderTargetDescriptor ;
		OMSetRenderTargets( 1, _singleCPUDescHandleCache, 
							RTsSingleHandleToDescriptorRange, pDepthStencilDescriptor ) ;
	}
	
	
	public void ClearDepthStencilView( CPUDescriptorHandle depthStencilView,
									   ClearFlags clearFlags,
									   float depth, byte stencil,
									   [Optional] uint numRects,
									   [Optional] Span< Rect > pRects ) {
		unsafe {
			fixed ( Rect* pRectsPtr = pRects ) {
				RECT* _rectsPtr = pRects is {Length: > 0} ? (RECT*)pRectsPtr : null ;
				ComObject!.ClearDepthStencilView( depthStencilView, (D3D12_CLEAR_FLAGS)clearFlags,
												  depth, stencil, numRects, _rectsPtr ) ;
			}
		}
	}


	public void ClearRenderTargetView( CPUDescriptorHandle renderTargetView,
									   ColorF              colorRgba,
									   uint                numRects = 0,
									   Span< Rect >        pRects = default ) {
		if( pRects is not { Length: > 0 } )
			pRects = default ;

		unsafe {
			//! Copy the color to cache array:
			//  (Way to avoid allocations temporarily)
			fixed( float* pColor = &_clearColorCache[ 0 ] ) {
				ColorF* pDst = (ColorF *)pColor ;
				*pDst = colorRgba ;
			}
			
			fixed ( Rect* pRectsPtr = ( pRects == default ? null : pRects ) ) {
				ComObject!.ClearRenderTargetView( renderTargetView,
												  _clearColorCache,
												  pRectsPtr is null ? 0 : numRects,
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
				ComObject!.ClearUnorderedAccessViewUint( ViewGPUHandleInCurrentHeap,
														 ViewCPUHandle,
														 resource.ComObject,
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
				ComObject!.ClearUnorderedAccessViewFloat( ViewGPUHandleInCurrentHeap, 
														  ViewCPUHandle, resource.ComObject,
														  Values, NumRects, (RECT *)pRectsPtr ) ;
		}
	}


	public void DiscardResource( IResource pResource, [Optional] in Span< DiscardRegion > pRegion ) {
		var resource = (Resource)pResource ;
		unsafe {
			fixed( DiscardRegion* pRegionPtr = pRegion ) {
				ComObject!.DiscardResource( resource.ComObject, (D3D12_DISCARD_REGION*)pRegionPtr ) ;
			}
		}
	}
	
	
	public void BeginQuery( IQueryHeap pQueryHeap, QueryType Type, uint Index ) {
		var queryHeap = (IComObjectRef< ID3D12QueryHeap >)pQueryHeap 
						?? throw new NullReferenceException( ) ;
		ComObject!.BeginQuery( queryHeap.ComObject, (D3D12_QUERY_TYPE)Type, Index ) ;
	}
	
	
	public void EndQuery( IQueryHeap pQueryHeap, QueryType Type, uint Index ) {
		var queryHeap = (IComObjectRef< ID3D12QueryHeap >)pQueryHeap 
						?? throw new NullReferenceException( ) ;
		ComObject!.EndQuery( queryHeap.ComObject, (D3D12_QUERY_TYPE)Type, Index ) ;
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
		ComObject!.ResolveQueryData( queryHeap.ComObject, (D3D12_QUERY_TYPE)Type, 
									 StartIndex, NumQueries,
									 resource.ComObject, AlignedDestinationBufferOffset ) ;
	}

	
	public void SetPredication( IResource pBuffer, ulong AlignedBufferOffset, PredicationOp Operation ) {
		var resource = (Resource)pBuffer ;
		ComObject!.SetPredication( resource.ComObject, AlignedBufferOffset, (D3D12_PREDICATION_OP)Operation ) ;
	}
	
	public void SetMarker(uint Metadata, [Optional] nint pData, uint Size ) {
		unsafe {
			ComObject!.SetMarker( Metadata, (void*)pData, Size ) ;
		}
	}
	
	public void BeginEvent( uint Metadata, [Optional] nint pData, uint Size ) {
		unsafe {
			ComObject!.BeginEvent( Metadata, (void*)pData, Size ) ;
		}
	}

	public void EndEvent( ) => ComObject!.EndEvent( ) ;
	
	public void ExecuteIndirect( ICommandSignature pCommandSignature,
								 uint              MaxCommandCount,
								 IResource         pArgumentBuffer,
								 ulong             ArgumentBufferOffset,
								 IResource         pCountBuffer,
								 ulong             CountBufferOffset ) {
		var commandSignature = (IComObjectRef< ID3D12CommandSignature >)pCommandSignature ;
		var argumentBuffer = (Resource)pArgumentBuffer ;
		var countBuffer = (Resource)pCountBuffer ;
		ComObject!.ExecuteIndirect( commandSignature.ComObject, MaxCommandCount, 
									argumentBuffer.ComObject, ArgumentBufferOffset, 
									countBuffer.ComObject, CountBufferOffset ) ;
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
	public override ID3D12GraphicsCommandList1? ComObject => ComPointer?.Interface ;
	
	
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
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
		var src = (Resource)pSrcBuffer ;
		var dst = (Resource)pDstBuffer ;
		
		// Copy dependent resources to an array (optimize later):
		ID3D12Resource[ ] dependentResources = new ID3D12Resource[ ppDependentResources.Length ] ;
		for ( int i = 0; i < dependentResources.Length; ++i ) {
			dependentResources[ i ] = ( (Resource)ppDependentResources[ i ] ).ComObject!
#if DEBUG || DEBUG_COM || DEV_BUILD
									  ?? throw new ArgumentException( $"{nameof(IGraphicsCommandList1)}.{nameof(AtomicCopyBufferUINT)} :: " +
																	  $"Collection of dependent resources cannot contain null values!" )
#endif
				;
		}
		
		unsafe {
			fixed(SubresourceRangeUInt64* pDependentRanges = &pDependentSubresourceRanges)
				cmdList.AtomicCopyBufferUINT( dst.ComObject, DstOffset,
											  src.ComObject, SrcOffset,
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
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
		var src = (Resource)pSrcBuffer ;
		var dst = (Resource)pDstBuffer ;
		
		// Copy dependent resources to an array (optimize later):
		ID3D12Resource[ ] dependentResources = new ID3D12Resource[ ppDependentResources.Length ] ;
		for ( int i = 0; i < dependentResources.Length; ++i ) {
			dependentResources[ i ] = ( (Resource)ppDependentResources[ i ] ).ComObject!
#if DEBUG || DEBUG_COM || DEV_BUILD
									  ?? throw new ArgumentException( $"{nameof(IGraphicsCommandList1)}.{nameof(AtomicCopyBufferUINT)} :: " +
																	  $"Collection of dependent resources cannot contain null values!" )
#endif
				;
		}
		
		unsafe {
			fixed(SubresourceRangeUInt64* pDependentRanges = &pDependentSubresourceRanges)
				cmdList.AtomicCopyBufferUINT64( dst.ComObject, DstOffset,
												src.ComObject, SrcOffset,
												Dependencies, dependentResources,
												(D3D12_SUBRESOURCE_RANGE_UINT64 *)pDependentRanges ) ;
		}
	}
	

	public void OMSetDepthBounds( float Min, float Max ) => ComObject!.OMSetDepthBounds( Min, Max ) ;
	

	public void SetSamplePositions( uint NumSamplesPerPixel, uint NumPixels, in SamplePosition pSamplePositions ) {
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
		
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
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
		var src = (Resource)pSrcResource ;
		var dst = (Resource)pDstResource ;
		
		unsafe {
			Unsafe.SkipInit( out RECT srcRect ) ;
			RECT* srcRectPtr = null ;
			if ( pSrcRect.HasValue ) {
				srcRect = pSrcRect.Value ;
				srcRectPtr = &srcRect ;
			}
			
			cmdList.ResolveSubresourceRegion( dst.ComObject, DstSubresource, DstX, DstY,
											  src.ComObject, SrcSubresource, srcRectPtr,
											  (DXGI_FORMAT)format, (D3D12_RESOLVE_MODE)resolveMode ) ;
		}
	}

	
	public void SetViewInstanceMask( uint Mask ) => ComObject!.SetViewInstanceMask( Mask ) ;
	
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
	public override ID3D12GraphicsCommandList2? ComObject => ComPointer?.Interface ;
	
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
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
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

	public override ID3D12GraphicsCommandList3? ComObject => ComPointer?.Interface ;

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
		ComObject!.SetProtectedResourceSession( protectedResourceSession.ComObject ) ;
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

	public override ID3D12GraphicsCommandList4? ComObject => ComPointer?.Interface ;

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
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
		var descArray = Unsafe.As< D3D12_RENDER_PASS_RENDER_TARGET_DESC[ ] >( pRenderTargets ) ;
		cmdList.BeginRenderPass( NumRenderTargets, descArray,
								 (pDepthStencil ?? default ),
								 (D3D12_RENDER_PASS_FLAGS)Flags ) ;
	}

	
	public void EndRenderPass( ) => ComObject!.EndRenderPass( ) ;
	

	public void InitializeMetaCommand( IMetaCommand    pMetaCommand,
									   [Optional] nint pInitializationParametersData,
									   nuint           InitializationParametersDataSizeInBytes = 0x00U ) {
		unsafe {
			var metaCommand = (IComObjectRef< ID3D12MetaCommand >)pMetaCommand ;
			ComObject!.InitializeMetaCommand( metaCommand.ComObject, (void*)pInitializationParametersData,
											  InitializationParametersDataSizeInBytes ) ;
		}
	}


	public void ExecuteMetaCommand( IMetaCommand    pMetaCommand,
									[Optional] nint pExecutionParametersData,
									nuint           ExecutionParametersDataSizeInBytes = 0x00U ) {
		unsafe {
			var metaCommand = (IComObjectRef< ID3D12MetaCommand >)pMetaCommand ;
			ComObject!.ExecuteMetaCommand( metaCommand.ComObject, (void *)pExecutionParametersData,
										   ExecutionParametersDataSizeInBytes ) ;
		}
	}


	public void BuildRaytracingAccelerationStructure( in BuildRaytracingAccelerationStructureDescription pDesc,
													  uint NumPostbuildInfoDescs,
													  [Optional] Span< RaytracingAccelerationStructurePostBuildInfoDescription > 
														  pPostbuildInfoDescs ) {
		
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
		
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
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
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
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
		cmdList.CopyRaytracingAccelerationStructure( DestAccelerationStructureData, SourceAccelerationStructureData,
													 ( D3D12_RAYTRACING_ACCELERATION_STRUCTURE_COPY_MODE ) Mode ) ;
	}

	
	public void SetPipelineState1( IStateObject pStateObject ) {
		var stateObject = (IComObjectRef< ID3D12StateObject >)pStateObject ;
		ComObject!.SetPipelineState1( stateObject.ComObject ) ;
	}


	public void DispatchRays( in DispatchRaysDescription pDesc ) {
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
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

	public override ID3D12GraphicsCommandList5? ComObject => ComPointer?.Interface ;

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
		var cmdList = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			if( combiners is { Length: > 0 } ) {
				fixed ( ShadingRateCombiner* combinerSpanPtr = combiners ) {
					D3D12_SHADING_RATE_COMBINER* combinersPtr = (D3D12_SHADING_RATE_COMBINER*)combinerSpanPtr ;
					cmdList.RSSetShadingRate( (D3D12_SHADING_RATE)baseShadingRate,
											  combinersPtr ) ;
				}
			}
			else cmdList.RSSetShadingRate( (D3D12_SHADING_RATE)baseShadingRate, null ) ;
		}
	}
	
	public void RSSetShadingRateImage( IResource shadingRateImage ) {
		var resource = (IComObjectRef< ID3D12Resource >)shadingRateImage ;
		ComObject!.RSSetShadingRateImage( resource.ComObject ) ;
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

	public override ID3D12GraphicsCommandList6? ComObject => ComPointer?.Interface ;

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
		ComObject!.DispatchMesh( ThreadGroupCountX, ThreadGroupCountY, ThreadGroupCountZ ) ;
	
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

	public override ID3D12GraphicsCommandList7? ComObject => ComPointer?.Interface ;

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
		var cmdList       = ComObject ?? throw new NullReferenceException( ) ;
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

	public override ID3D12GraphicsCommandList8? ComObject => ComPointer?.Interface ;

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
		ComObject!.OMSetFrontAndBackStencilRef( FrontStencilRef, BackStencilRef ) ;

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

	public override ID3D12GraphicsCommandList9? ComObject => ComPointer?.Interface ;

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
		ComObject!.RSSetDepthBias( DepthBias, DepthBiasClamp, SlopeScaledDepthBias ) ;

	public void IASetIndexBufferStripCutValue( IndexBufferStripCutValue IBStripCutValue ) =>
		ComObject!.IASetIndexBufferStripCutValue( ( D3D12_INDEX_BUFFER_STRIP_CUT_VALUE ) IBStripCutValue ) ;

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