#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;

using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi.Common ;
using Windows.Win32.Security ;

using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Device))]
internal class Device: Object,
					 IDevice,
					 IComObjectRef< ID3D12Device >,
					 IUnknownWrapper< ID3D12Device > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device >? _comPtr ;
	public new ComPtr< ID3D12Device >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device >( ) ;
	public override ID3D12Device? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device >( ) ;
	}
	internal Device( ComPtr< ID3D12Device > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device( ID3D12Device comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------
	
	public uint GetNodeCount( ) => ComObject!.GetNodeCount( ) ;
	
	public void CreateCommandQueue( in CommandQueueDescription pDesc,
									in Guid                    riid, out ICommandQueue? ppCommandQueue ) {
		unsafe { fixed ( void* descPtr = &pDesc, riidPtr = &riid ) {
				ComObject!.CreateCommandQueue( (D3D12_COMMAND_QUEUE_DESC*)descPtr,
											   (Guid *)riidPtr, out var ppvCommandQueue ) ;
				ppCommandQueue = new CommandQueue( (ID3D12CommandQueue)ppvCommandQueue) ;
			}
		}
	}


	public void CreateCommandAllocator( CommandListType        type, in Guid riid,
										out ICommandAllocator? ppCommandAllocator ) {
		unsafe { fixed ( Guid* riidPtr = &riid ) {
				ComObject!.CreateCommandAllocator( (D3D12_COMMAND_LIST_TYPE)type, riidPtr,
														out var ppvCommandAllocator ) ;
				var               _allocator = (ID3D12CommandAllocator)ppvCommandAllocator ;
				CommandAllocator? allocator  = new( _allocator ) ;
				ppCommandAllocator = allocator ;
			}
		}
	}

	
	public void CreateGraphicsPipelineState( in  GraphicsPipelineStateDescription pDesc, in Guid riid,
											 out IPipelineState?                  ppPipelineState ) {
		unsafe { fixed ( Guid* riidPtr = &riid ) {
				ComObject!.CreateGraphicsPipelineState( pDesc, riidPtr, 
														out var ppvPipelineState ) ;
				ppPipelineState = new PipelineState( (ID3D12PipelineState)ppvPipelineState ) ;
			}
		}
	}

	
	public void CreateComputePipelineState( in ComputePipelineStateDescription pDesc,
											in Guid riid, out IPipelineState ppPipelineState ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		device.CreateComputePipelineState( pDesc, riid, out var ppvPipelineState ) ;
		ppPipelineState = new PipelineState( (ID3D12PipelineState)ppvPipelineState ) ;
	}


	public void CreateCommandList( uint                       nodeMask,
								   CommandListType            type,
								   ICommandAllocator          pCommandAllocator,
								   [Optional] IPipelineState? pInitialState,
								   in         Guid            riid,
								   out        ICommandList    ppCommandList ) {
		var device       = ComObject ?? throw new NullReferenceException( ) ;
		var allocator    = (IComObjectRef< ID3D12CommandAllocator >?)pCommandAllocator ;
		var initialState = (IComObjectRef< ID3D12PipelineState >?)pInitialState ;
		Guid _guid       = riid ;
		
		unsafe {
			var hr = device.CreateCommandList(  nodeMask,
												(D3D12_COMMAND_LIST_TYPE)type, 
												allocator?.ComObject,
												initialState?.ComObject,
												&_guid,
												out object _cmdList ) ;

#if DEBUG || DEBUG_COM || DEV_BUILD
			hr.ThrowOnFailure( ) ;
#endif
			
			if ( _guid == CommandList.Guid )
				ppCommandList = new CommandList( (ID3D12CommandList)_cmdList ) ;
			else
				ppCommandList = new GraphicsCommandList( (ID3D12GraphicsCommandList)_cmdList ) ;
		}
	}



	public void CheckFeatureSupport( D3D12Feature Feature, nint pFeatureSupportData, uint FeatureSupportDataSize ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			device.CheckFeatureSupport( (D3D12_FEATURE)Feature, (void *)pFeatureSupportData, FeatureSupportDataSize ) ;
		}
	}


	public void CreateDescriptorHeap( in DescriptorHeapDescription pDescriptorHeapDesc,
									  in Guid                      riid, out IDescriptorHeap? ppvHeap ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		device.CreateDescriptorHeap( pDescriptorHeapDesc, riid, out var ppvDescriptorHeap ) ;
		ppvHeap = new DescriptorHeap( (ID3D12DescriptorHeap)ppvDescriptorHeap ) ;
	}
	

	public uint GetDescriptorHandleIncrementSize( DescriptorHeapType DescriptorHeapType ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		return device.GetDescriptorHandleIncrementSize( (D3D12_DESCRIPTOR_HEAP_TYPE)DescriptorHeapType ) ;
	}
	
	
	public void CreateRootSignature( uint                nodeMask,
									 nint                pBlobWithRootSignature,
									 nuint               blobLengthInBytes, in Guid riid,
									 out IRootSignature? ppvRootSignature ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			device.CreateRootSignature( nodeMask, (void *)pBlobWithRootSignature,
										blobLengthInBytes, riid, out var ppvSignature ) ;
			ppvRootSignature = new RootSignature( (ID3D12RootSignature)ppvSignature ) ;
		}
	}


	public void CreateConstantBufferView( [Optional] in ConstBufferViewDescription pDesc,
										  in            CPUDescriptorHandle        DestDescriptor ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		device.CreateConstantBufferView( pDesc, DestDescriptor ) ;
	}


	public void CreateShaderResourceView( [Optional] IResource? pResource,
										  [Optional] ShaderResourceViewDescription pDesc,
										  CPUDescriptorHandle destDescriptor = default ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		var resource = (Resource?)pResource ;
		device.CreateShaderResourceView( resource?.ComObject, pDesc, destDescriptor ) ;
	}

	
	public void CreateUnorderedAccessView( IResource                         pResource,
										   IResource                         pCounterResource,
										   in UnorderedAccessViewDescription pDesc,
										   CPUDescriptorHandle               DestDescriptor ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		var resource = (Resource)pResource ;
		var counterResource = (Resource)pCounterResource ;
		device.CreateUnorderedAccessView( resource.ComObject, counterResource.ComObject, pDesc, DestDescriptor ) ;
	}


	public void CreateRenderTargetView( IResource                                pResource,
										[Optional] in RenderTargetViewDescription pDescription,
										CPUDescriptorHandle                       DestDescriptor ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		var resource = (Resource)pResource ;
		device.CreateRenderTargetView( resource.ComObject, pDescription, DestDescriptor ) ;
	}


	public void CreateDepthStencilView( IResource                          pResource,
										[Optional] in DepthStencilViewDesc pDesc,
										CPUDescriptorHandle                DestDescriptor ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		var resource = (Resource)pResource ;
		device.CreateDepthStencilView( resource.ComObject, pDesc, DestDescriptor ) ;
	}


	public void CreateSampler( in SamplerDescription pDesc, CPUDescriptorHandle DestDescriptor ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		device.CreateSampler( pDesc, DestDescriptor ) ;
	}


	public void CopyDescriptors( uint                           NumDestDescriptorRanges,
								 in Span< CPUDescriptorHandle > pDestDescriptorRangeStarts,
								 uint[ ]                        pDestDescriptorRangeSizes,
								 uint                           NumSrcDescriptorRanges,
								 in Span< CPUDescriptorHandle > pSrcDescriptorRangeStarts,
								 uint[ ]                        pSrcDescriptorRangeSizes,
								 DescriptorHeapType             DescriptorHeapsType ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed( CPUDescriptorHandle* pDestRangeStart = pDestDescriptorRangeStarts, pSrcRangeStart = pSrcDescriptorRangeStarts ) {
				fixed( uint* pDstSizes = &pDestDescriptorRangeSizes[0], pSrcSizes = &pSrcDescriptorRangeSizes[0] ) {
					device.CopyDescriptors( NumDestDescriptorRanges,
											(D3D12_CPU_DESCRIPTOR_HANDLE*)pDestRangeStart,
											pDstSizes,
											NumSrcDescriptorRanges,
											(D3D12_CPU_DESCRIPTOR_HANDLE*)pSrcRangeStart,
											pSrcSizes,
											(D3D12_DESCRIPTOR_HEAP_TYPE)DescriptorHeapsType ) ;
				}
			}
		}
	}


	public void CopyDescriptorsSimple( uint                NumDescriptors,
									   CPUDescriptorHandle DestDescriptorRangeStart,
									   CPUDescriptorHandle SrcDescriptorRangeStart,
									   DescriptorHeapType  DescriptorHeapsType ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		device.CopyDescriptorsSimple( NumDescriptors,
									  DestDescriptorRangeStart,
									  SrcDescriptorRangeStart,
									  ( D3D12_DESCRIPTOR_HEAP_TYPE )DescriptorHeapsType ) ;
	}


	public ResourceAllocationInfo GetResourceAllocationInfo( uint                        visibleMask,
															 uint                        numResourceDescs,
															 Span< ResourceDescription > pResourceDescs ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed( ResourceDescription* pResourceDesc = pResourceDescs ) {
				var allocInfo = device.GetResourceAllocationInfo( visibleMask, numResourceDescs, (D3D12_RESOURCE_DESC *)pResourceDesc ) ;
				return allocInfo ;
			}
		}
	}

	
	public HeapProperties GetCustomHeapProperties( uint nodeMask, HeapType heapType ) => 
		ComObject!.GetCustomHeapProperties( nodeMask, (D3D12_HEAP_TYPE)heapType ) ;


	public void CreateCommittedResource( in HeapProperties         pHeapProperties,
										 HeapFlags                 HeapFlags,
										 in ResourceDescription    pDesc,
										 ResourceStates            InitialResourceState,
										 [Optional] in ClearValue? pOptimizedClearValue,
										 in            Guid        riidResource,
										 out           IResource?  ppvResource ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe { fixed( void* _pHeapProps = &pHeapProperties, 
					   _pDesc = &pDesc,
					   _pClearValue = &pOptimizedClearValue,
					   _riidResource = &riidResource ) {

				D3D12_CLEAR_VALUE* pD3D12ClearValue =
					pOptimizedClearValue is null ? null 
						: (D3D12_CLEAR_VALUE*)_pClearValue ;

				device.CreateCommittedResource( (D3D12_HEAP_PROPERTIES *)_pHeapProps, 
												(D3D12_HEAP_FLAGS)HeapFlags,
											   (D3D12_RESOURCE_DESC*)_pDesc, 
											   (D3D12_RESOURCE_STATES)InitialResourceState,
												pD3D12ClearValue,
											   (Guid *)_riidResource, out var _resource ) ;
				
				ppvResource = new Resource( (ID3D12Resource)_resource ) ;
			}
		}
	}


	public void CreateHeap( in HeapDescription pDesc, in Guid riid, out IHeap ppvHeap ) {
		unsafe {
			HeapDescription _heapDesc = pDesc ;
			Guid _riid = riid ;
			ComObject!.CreateHeap( (D3D12_HEAP_DESC *)&_heapDesc, &_riid, out var _heap ) ;
			ppvHeap = new Heap( (ID3D12Heap)_heap ) ;
		}
	}


	public void CreatePlacedResource( IHeap                     pHeap, ulong HeapOffset,
									  in ResourceDescription    pDesc,
									  ResourceStates            InitialState,
									  [Optional] in ClearValue? pOptimizedClearValue,
									  in Guid riid, 
									  out IResource ppvResource ) { 
		unsafe {
			Guid _riid = riid ;
			var _pDesc = pDesc ;
			D3D12_CLEAR_VALUE* pClearValue = null ;

			if ( pOptimizedClearValue.HasValue ) {
				ClearValue _clrValue = pOptimizedClearValue.Value ;
				pClearValue = (D3D12_CLEAR_VALUE *)&_clrValue ;
			}
			
			var heap = (IComObjectRef< ID3D12Heap >)pHeap ;
			ComObject!.CreatePlacedResource( heap.ComObject, HeapOffset,
											(D3D12_RESOURCE_DESC*)&_pDesc,
											(D3D12_RESOURCE_STATES)InitialState,
											pClearValue,
											&_riid,
											out var _res ) ;

			ppvResource = new Resource( (ID3D12Resource)_res ) ;
		}
	}

	public void CreateReservedResource( in ResourceDescription    pDesc,
										ResourceStates            InitialState,
										[Optional] in ClearValue? pOptimizedClearValue,
										in            Guid        riid, out IResource ppvResource ) { unsafe {
			Guid _riid = riid ;
			var _pDesc = pDesc ;
			D3D12_CLEAR_VALUE* pClearValue = null ;
			
			if ( pOptimizedClearValue.HasValue ) {
				ClearValue _clrValue = pOptimizedClearValue.Value ;
				pClearValue = (D3D12_CLEAR_VALUE*)&_clrValue ;
			}
			
			ComObject!.CreateReservedResource( (D3D12_RESOURCE_DESC *)&_pDesc, 
											  (D3D12_RESOURCE_STATES)InitialState, 
											  pClearValue, 
											  &_riid,
											  out var _res ) ;
			
			ppvResource = new Resource( (ID3D12Resource)_res ) ;
		}
	}


	public void CreateSharedHandle( IDeviceChild pObject,
									[Optional] in SecurityAttributes pAttributes,
									uint Access, 
									string Name, 
									out Win32Handle pHandle ) {
		var deviceChild = (IComObjectRef< ID3D12DeviceChild >)pObject ;
		unsafe {
			fixed ( SecurityAttributes* _attrPtr = &pAttributes ) {
				Win32Handle _handle = default ;
				ComObject!.CreateSharedHandle( deviceChild.ComObject, (SECURITY_ATTRIBUTES *)_attrPtr, 
											   Access, Name, (HANDLE *)&_handle ) ;
				pHandle = _handle ;
			}
		}
	}


	public void OpenSharedHandle( Win32Handle NTHandle, in Guid riid, out object ppvObj ) {
		// Get the ID3D12Device
		var device = this.ComObject
					 ?? throw new Exception("COMObject is null");

		// Make local (fixed) Guid to easily obtain pointer:
		Guid _riid = riid ;
		unsafe {
			device.OpenSharedHandle( NTHandle, &_riid, out var ppv ) ;
			ppvObj = ppv ;
		}
	}

	public void OpenSharedHandleByName( string Name, uint Access, ref Win32Handle pNTHandle ) {
		// Get the ID3D12Device
		var device = this.ComObject
							  ?? throw new Exception("COMObject is null");

		// Copy Name to PCWSTR from string (uses implicit op):
		PCWSTR name   = Name ;
		HANDLE handle = pNTHandle ;
		
		// Call the OpenSharedHandleByName method 
		unsafe { device.OpenSharedHandleByName( name, Access, &handle ) ; }
		
		// Set the HANDLE
		pNTHandle = handle ;
	}

	
	public void MakeResident< P >( uint NumObjects, Span< P > ppObjects ) where P: IPageable {
		var pageables = new ID3D12Pageable[ NumObjects ] ;
		
		for ( int i = 0; i < NumObjects; ++i ) {
			var next = (IComObjectRef<ID3D12Pageable>?)ppObjects[i] ;
#if DEBUG || DEV_BUILD
			if( next is null ) throw new ArgumentNullException( nameof(ppObjects), 
																$"{nameof(IDevice)}.{nameof(MakeResident)} :: " +
																$"Span must not contain null references!" ) ;
#endif

			pageables[ i ] = next?.ComObject
#if DEBUG || DEBUG_COM || DEV_BUILD
							 ?? throw new ArgumentException( nameof( ppObjects ),
															 $"{nameof( IDevice )}.{nameof( MakeResident )} :: " +
															 $"Span must contain only COM objects!" )
#else
				!
#endif
				;
		}
		ComObject!.MakeResident( NumObjects, pageables ) ;
	}

	
	public void Evict< P >( uint NumObjects, Span< P > ppObjects ) where P: IPageable {
		// Extract the COM interface refs from the span of objects:
		// A more efficient unsafe overload taking a raw ID3D12Pageable** should be written ...
		var pageables     = new ID3D12Pageable[ ppObjects.Length ] ;
		var _pageableSpan = pageables.AsSpan( ) ;
			
		for( int i = 0; i < ppObjects.Length; i++ ) {
			var next = (IComObjectRef<ID3D12Pageable>?)ppObjects[ i ] ;
#if DEBUG || DEV_BUILD
			if( next is null ) throw new ArgumentNullException( nameof(ppObjects), 
																$"{nameof(IDevice)}.{nameof(Evict)} :: " +
																$"Span must not contain null references!" ) ;
			if ( next.ComObject is null )
				throw new ArgumentException( nameof( ppObjects ),
											 $"{nameof( IDevice )}.{nameof( Evict )} :: " +
											 $"Span must contain only COM objects!" ) ;
#endif
			
			_pageableSpan[ i ] = next?.ComObject! ;
		}
			
		ComObject!.Evict( NumObjects, pageables ) ;
	}

	
	public void CreateFence( ulong       InitialValue, 
							 FenceFlags  Flags,
							 in  Guid    riid,
							 out IFence? ppFence ) {
		unsafe { 
			Guid iid = riid ;
			ComObject!.CreateFence( InitialValue, 
									(D3D12_FENCE_FLAGS)Flags, 
									&iid, 
									out var fence ) ;
			ppFence = new Fence( (ID3D12Fence)fence ) ;
		}
	}

	
	public HResult GetDeviceRemovedReason( ) => 
		ComObject!.GetDeviceRemovedReason( ) ;

	
	public nint GetCopyableFootprints( in ResourceDescription                       pResourceDesc, 
									   uint                                         FirstSubresource, 
									   uint                                         NumSubresources, 
									   ulong                                        BaseOffset,
									   [Out] out Span< PlacedSubresourceFootprint > pLayouts,
									   [Out] out Span< uint >                       pNumRows,
									   [Out] out Span< ulong >                      pRowSizeInBytes,
									   out       ulong                              pTotalBytes ) {
		//! pLayouts: to be filled with the description and placement of each subresource
		//! pNumRows: to be filled with the number of rows for each subresource
		//! pRowSizeInBytes: to be filled with the unpadded size in bytes of a row, of each subresource

		uint[ ]?  _numRows        = new uint[ NumSubresources ] ;
		ulong[ ]? _rowSizeInBytes = new ulong[ NumSubresources ] ;
		
		unsafe {
			var _pLayouts = (D3D12_PLACED_SUBRESOURCE_FOOTPRINT*)
				( NativeMemory.Alloc( (nuint)
									  ( sizeof( PlacedSubresourceFootprint )
										* NumSubresources ) ) ) ;

			fixed ( void* _descPtr = &pResourceDesc,
						 _rowCounts = &_numRows[ 0 ],
						 _rowSizes = &_rowSizeInBytes[ 0 ] ) {
				
				ulong _totalBytes = 0 ;
				ComObject!.GetCopyableFootprints( (D3D12_RESOURCE_DESC*)_descPtr,
												  FirstSubresource,
												  NumSubresources,
												  BaseOffset,
												  _pLayouts,
												  (uint *)_rowCounts,
												  (ulong *)_rowSizes,
												  &_totalBytes ) ;

				pLayouts          = new( _pLayouts, (int)NumSubresources ) ;
				pNumRows        = _numRows ;
				pRowSizeInBytes = _rowSizeInBytes ;
				pTotalBytes	      = _totalBytes ;
				return (nint)_pLayouts ;
			}
		}
	}
	
	public void CreateQueryHeap( in QueryHeapDescription pDesc, in Guid riid, out IHeap ppvHeap ) {
		unsafe {
			fixed ( QueryHeapDescription* _pDesc = &pDesc ) {
				Guid _guid = riid;
				ComObject!.CreateQueryHeap( (D3D12_QUERY_HEAP_DESC *)_pDesc, 
											&_guid, out var result ) ;
				ppvHeap = new Heap( (ID3D12Heap)result ) ;
			}
		}
	}
	
	public void SetStablePowerState( bool Enable ) => 
		ComObject!.SetStablePowerState( Enable ) ;
	
	
	public void CreateCommandSignature( in CommandSignatureDescription pDesc,
										IRootSignature                 pRootSignature, in Guid riid,
										out ICommandSignature          ppvCommandSignature ) {
		unsafe {
			Guid _guid = riid ;
			fixed ( CommandSignatureDescription* _ppDesc = &pDesc ) {
				ComObject!.CreateCommandSignature( (D3D12_COMMAND_SIGNATURE_DESC *)_ppDesc, 
												  pRootSignature.ComObject, &_guid, 
												  out var signature ) ;
				ppvCommandSignature = new
					CommandSignature( (ID3D12CommandSignature)signature ) ;
			}
		}
	}
	
	public void GetResourceTiling( in  IResource         pTiledResource,
								   out uint              pNumTilesForEntireResource,
								   out PackedMipInfo     pPackedMipDesc,
								   out TileShape         pStandardTileShapeForNonPackedMips,
								   ref uint              pNumSubresourceTilings, 
								   uint                  FirstSubresourceTilingToGet,
								   ref SubresourceTiling pSubresourceTilingsForNonPackedMips ) {
		unsafe {
			var _pMipDesc = stackalloc D3D12_PACKED_MIP_INFO[ 1 ] ;
			var _ptileShapeForNonPacked = stackalloc D3D12_TILE_SHAPE[ 1 ] ;
			
			var resource = (Resource)pTiledResource ;
			fixed( void* pSubresourceTiling = &pSubresourceTilingsForNonPackedMips,
						 pNumSubTiles = &pNumSubresourceTilings ) {
				
				uint _numTilesForRsrc = 0 ;
				ComObject!.GetResourceTiling( resource.ComObject,
											  &_numTilesForRsrc,
											  _pMipDesc, _ptileShapeForNonPacked,
											  (uint *)pNumSubTiles,
											  FirstSubresourceTilingToGet,
											  (D3D12_SUBRESOURCE_TILING *)pSubresourceTiling ) ;
				
				pNumTilesForEntireResource = _numTilesForRsrc ;
			}
			
			pPackedMipDesc = *(PackedMipInfo *)_pMipDesc ;
			pStandardTileShapeForNonPackedMips = *(TileShape *)_ptileShapeForNonPacked ;
		}
	}
	
	
	public Luid GetAdapterLuid( ) => ComObject!.GetAdapterLuid( ) ;
	
	// -------------------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12Device) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device1))]
internal class Device1: Device,
						IDevice1,
						IComObjectRef< ID3D12Device1 >,
						IUnknownWrapper< ID3D12Device1 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device1 >? _comPtr ;
	public new ComPtr< ID3D12Device1 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device1 >( ) ;
	public override ID3D12Device1? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device1( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device1 >( ) ;
	}
	internal Device1( ComPtr< ID3D12Device1 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device1( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device1( ID3D12Device1 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------
	
	public void CreatePipelineLibrary( nint pLibraryBlob,
									   nuint BlobLength,
									   in  Guid riid,
									   out IPipelineLibrary ppPipelineLibrary ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			device.CreatePipelineLibrary( (void *)pLibraryBlob, BlobLength, &_guid, out var result ) ;
			ppPipelineLibrary = new PipelineLibrary( (ID3D12PipelineLibrary)result ) ;
		}
	}


	public void SetEventOnMultipleFenceCompletion( IFence[ ] ppFences,
												   ulong[ ] pFenceValues,
												   uint NumFences,
												   MultiFenceWaitFlags Flags,
												   Win32Handle hEvent ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		ID3D12Fence[ ] _fences = new ID3D12Fence[ NumFences ] ;
		for ( uint i = 0; i < NumFences; ++i ) {
			_fences[ i ] = ( (IComObjectRef<ID3D12Fence>)ppFences[ i ] ).ComObject!
#if DEBUG || DEBUG_COM || DEV_BUILD
						   ?? throw new NullReferenceException( )
#endif
			 						   ;
		}
		device.SetEventOnMultipleFenceCompletion( _fences,
												  pFenceValues,
												  NumFences,(D3D12_MULTIPLE_FENCE_WAIT_FLAGS)Flags,
												  hEvent ) ;
	}


	public void SetResidencyPriority( uint NumObjects,
							   IPageable[ ] ppObjects,
							   Span< ResidencyPriority > pPriorities ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		ID3D12Pageable[ ] _objects = new ID3D12Pageable[ NumObjects ] ;
		for ( uint i = 0; i < NumObjects; ++i ) {
			_objects[ i ] = ( (IComObjectRef< ID3D12Pageable >)ppObjects[ i ] ).ComObject!
#if DEBUG || DEBUG_COM || DEV_BUILD
							?? throw new NullReferenceException( ) 
#endif
							;
		}
		unsafe {
			fixed ( ResidencyPriority* pPriorities_ = pPriorities ) {
				device.SetResidencyPriority( NumObjects, _objects, (D3D12_RESIDENCY_PRIORITY *)pPriorities_ ) ;
			}
		}
	}

	// -------------------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12Device1) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device1).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device2))]
internal class Device2: Device1,
						IDevice2,
						IComObjectRef< ID3D12Device2 >,
						IUnknownWrapper< ID3D12Device2 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device2 >? _comPtr ;
	public new ComPtr< ID3D12Device2 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device2 >( ) ;
	public override ID3D12Device2? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device2( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device2 >( ) ;
	}
	internal Device2( ComPtr< ID3D12Device2 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device2( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device2( ID3D12Device2 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}

	// -------------------------------------------------------------------------------------------------------
	
	public void CreatePipelineState( in  PipelineStateStreamDescription pDesc,
									 in  Guid riid,
									 out IPipelineState ppPipelineState ) {
		unsafe {
			fixed ( void* pGuid = &riid, pPipelineDesc = &pDesc ) {
				ComObject!.CreatePipelineState( (D3D12_PIPELINE_STATE_STREAM_DESC *)pPipelineDesc, 
											   (Guid*)pGuid, 
											   out var stateObj ) ;
				
				ppPipelineState = new PipelineState( (ID3D12PipelineState)stateObj ) ;
			}
		}
	}
	
	// -------------------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12Device2) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device2).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device3))]
internal class Device3: Device2,
						IDevice3,
						IComObjectRef< ID3D12Device3 >,
						IUnknownWrapper< ID3D12Device3 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device3 >? _comPtr ;
	public new ComPtr< ID3D12Device3 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device3 >( ) ;
	public override ID3D12Device3? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device3( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device3 >( ) ;
	}
	internal Device3( ComPtr< ID3D12Device3 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device3( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device3( ID3D12Device3 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------

	public void EnqueueMakeResident( ResidencyFlags flags, uint numObjects, 
									 IPageable[ ] ppObjects, IFence pFenceToSignal,
									 ulong fenceValueToSignal ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		ID3D12Pageable[ ] _objects = new ID3D12Pageable[ numObjects ] ;
		for ( uint i = 0; i < numObjects; ++i ) {
			_objects[ i ] = ( (IComObjectRef< ID3D12Pageable >)ppObjects[ i ] ).ComObject!
#if DEBUG || DEBUG_COM || DEV_BUILD
							?? throw new NullReferenceException( )
#endif
			 							;
		}
		var fence = (IComObjectRef< ID3D12Fence >)pFenceToSignal ;
		device.EnqueueMakeResident( (D3D12_RESIDENCY_FLAGS)flags, numObjects, 
									_objects, fence.ComObject,
									fenceValueToSignal ) ;
	}

	public void OpenExistingHeapFromAddress( nint pAddress, in Guid riid, out IHeap ppvHeap ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			device.OpenExistingHeapFromAddress( (void *)pAddress, &_guid, out var heap ) ;
			ppvHeap = new Heap( (ID3D12Heap)heap ) ;
		}
	}

	public void OpenExistingHeapFromFileMapping( Win32Handle hFileMapping, in Guid riid, out IHeap ppvHeap ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			device.OpenExistingHeapFromFileMapping( hFileMapping, &_guid, out var heap ) ;
			ppvHeap = new Heap( (ID3D12Heap)heap ) ;
		}
	}

	// -------------------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12Device3) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device3).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device4))]
internal class Device4: Device3,
						IDevice4,
						IComObjectRef< ID3D12Device4 >,
						IUnknownWrapper< ID3D12Device4 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device4 >? _comPtr ;
	public new ComPtr< ID3D12Device4 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device4 >( ) ;
	public override ID3D12Device4? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device4( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device4 >( ) ;
	}
	internal Device4( ComPtr< ID3D12Device4 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device4( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device4( ID3D12Device4 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------

	public void CreateHeap1( in HeapDescription pDesc, 
							 IProtectedResourceSession pProtectedSession, 
							 in Guid riid,
							 out IHeap ppvHeap ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			var protectedSession = (IComObjectRef< ID3D12ProtectedResourceSession >)pProtectedSession ;
			fixed ( HeapDescription* _pDesc = &pDesc ) {
				device.CreateHeap1( (D3D12_HEAP_DESC *)_pDesc, 
									protectedSession.ComObject, 
									&_guid, 
									out var heap ) ;
				ppvHeap = new Heap( (ID3D12Heap)heap ) ;
			}
		}
	}

	public unsafe void CreateCommandList1( uint nodeMask, 
										   CommandListType type, 
										   CommandListFlags flags, 
										   in Guid riid,
										   out ICommandList ppCommandList ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		Guid _guid = riid ;
		device.CreateCommandList1( nodeMask, (D3D12_COMMAND_LIST_TYPE)type, (D3D12_COMMAND_LIST_FLAGS)flags, &_guid, out var list ) ;
		ppCommandList = new CommandList( (ID3D12CommandList)list ) ;
	}

	public void CreateCommittedResource1( in HeapProperties pHeapProperties, 
										  HeapFlags HeapFlags, 
										  in ResourceDescription pDesc,
										  ResourceStates InitialResourceState, 
										  in ClearValue? pOptimizedClearValue,
										  IProtectedResourceSession pProtectedSession, 
										  in Guid riidResource,
										  out IResource ppvResource ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riidResource ;
			var protectedSession = (IComObjectRef< ID3D12ProtectedResourceSession >)pProtectedSession ;
			
			fixed ( void* _pHeapProperties = &pHeapProperties, _pDesc = &pDesc ) {
				ClearValue _clrValue = pOptimizedClearValue ?? default ;
				ClearValue* pClearValue = pOptimizedClearValue.HasValue ? (ClearValue *)&_clrValue : null ;
				
				device.CreateCommittedResource1( (D3D12_HEAP_PROPERTIES *)_pHeapProperties, 
												 (D3D12_HEAP_FLAGS)HeapFlags, 
												 (D3D12_RESOURCE_DESC *)_pDesc, 
												 (D3D12_RESOURCE_STATES)InitialResourceState, 
												 (D3D12_CLEAR_VALUE *)pClearValue, 
												 protectedSession.ComObject, 
												 &_guid, 
												 out var resource ) ;
				ppvResource = new Resource( (ID3D12Resource)resource ) ;
			}
		}
	}

	public void CreateReservedResource1( in ResourceDescription pDesc, 
										 ResourceStates InitialState,
										 in ClearValue? pOptimizedClearValue, 
										 IProtectedResourceSession pProtectedSession,
										 in Guid riid, out IResource ppvResource ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			var protectedSession = (IComObjectRef< ID3D12ProtectedResourceSession >)pProtectedSession ;
			
			fixed ( ResourceDescription* _pDesc = &pDesc ) {
				ClearValue _clrValue = pOptimizedClearValue ?? default ;
				ClearValue* pClearValue = pOptimizedClearValue.HasValue ? (ClearValue *)&_clrValue : null ;
				
				device.CreateReservedResource1( (D3D12_RESOURCE_DESC *)_pDesc, 
											   (D3D12_RESOURCE_STATES)InitialState, 
											   (D3D12_CLEAR_VALUE *)pClearValue, 
											   protectedSession.ComObject, 
											   &_guid, 
											   out var resource ) ;
				
				ppvResource = new Resource( (ID3D12Resource)resource ) ;
			}
		}
	}

	
	public void CreateProtectedResourceSession( in  ProtectedResourceSessionDescription pDesc, in Guid riid,
												out IProtectedResourceSession           ppSession ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			fixed ( ProtectedResourceSessionDescription* _pDesc = &pDesc ) {
				device.CreateProtectedResourceSession( (D3D12_PROTECTED_RESOURCE_SESSION_DESC *)_pDesc, 
													   &_guid, 
													   out var session ) ;
				ppSession = new ProtectedResourceSession( (ID3D12ProtectedResourceSession)session ) ;
			}
		}
	}

	
	public ResourceAllocationInfo GetResourceAllocationInfo1( uint visibleMask, uint numResourceDescs, 
															  in Span< ResourceDescription > pResourceDescs,
															  in Span< ResourceAllocationInfo1 > pResourceAllocationInfo1 = default ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( ResourceDescription* _pResourceDescs = pResourceDescs ) {
				ResourceAllocationInfo1* _pResourceAllocationInfo1 = stackalloc ResourceAllocationInfo1[ pResourceAllocationInfo1.Length ] ;
				var result = device.GetResourceAllocationInfo1( visibleMask, numResourceDescs, 
																(D3D12_RESOURCE_DESC *)_pResourceDescs,
																(D3D12_RESOURCE_ALLOCATION_INFO1*)_pResourceAllocationInfo1 ) ;
				return result ;
			}
		}
	}

	// -------------------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12Device4) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device4).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// =======================================================================================================
} ;


[SupportedOSPlatform("windows10.0.17763")]
[Wrapper(typeof(ID3D12Device5))]
internal class Device5: Device4,
						IDevice5,
						IComObjectRef< ID3D12Device5 >,
						IUnknownWrapper< ID3D12Device5 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device5 >? _comPtr ;
	public new ComPtr< ID3D12Device5 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device5 >( ) ;
	public override ID3D12Device5? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device5( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device5 >( ) ;
	}
	internal Device5( ComPtr< ID3D12Device5 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device5( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device5( ID3D12Device5 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------

	public void RemoveDevice( ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		device.RemoveDevice( ) ;
	}

	
	public void CreateLifetimeTracker( ILifetimeOwner pOwner, in Guid riid, out ILifetimeTracker ppvTracker ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			var owner = (IComObjectRef< ID3D12LifetimeOwner >)pOwner ;
			device.CreateLifetimeTracker( owner.ComObject, &_guid, out var tracker ) ;
			ppvTracker = new LifetimeTracker( (ID3D12LifetimeTracker)tracker ) ;
		}
	}

	
	public void CreateMetaCommand( in Guid commandId,
								   uint nodeMask,
								   nint pCreationParametersData,
								   nuint CreationParametersDataSizeInBytes,
								   in Guid riid,
								   out IMetaCommand ppMetaCommand ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( Guid* _commandId = &commandId, _guid = &riid ) {
				device.CreateMetaCommand( _commandId, nodeMask, (void *)pCreationParametersData, 
										  CreationParametersDataSizeInBytes, _guid, out var command ) ;
				ppMetaCommand = new MetaCommand( (ID3D12MetaCommand)command ) ;
			}
		}
	}

	
	public void CreateStateObject( in StateObjectDescription pDesc, in Guid riid, out IStateObject ppStateObject ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			fixed ( StateObjectDescription* _pDesc = &pDesc ) {
				device.CreateStateObject( (D3D12_STATE_OBJECT_DESC *)_pDesc, &_guid, out var stateObject ) ;
				ppStateObject = new StateObject( (ID3D12StateObject)stateObject ) ;
			}
		}
	}

	
	public void EnumerateMetaCommands( ref uint pNumMetaCommands, out Span< MetaCommandDescription > pDescs ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			pDescs = new MetaCommandDescription[ (int)pNumMetaCommands ] ;
			fixed( MetaCommandDescription* descPtr = pDescs ) {
				device.EnumerateMetaCommands( ref pNumMetaCommands, (D3D12_META_COMMAND_DESC*)descPtr ) ;
			}
		}
	}

	
	public DriverMatchingIdentifierStatus CheckDriverMatchingIdentifier( SerializedDataType SerializedDataType,
																		 in SerializedDataDriverMatchingIdentifier
																			 pIdentifierToCheck ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( SerializedDataDriverMatchingIdentifier* _pIdentifierToCheck = &pIdentifierToCheck ) {
				return (DriverMatchingIdentifierStatus)
					device.CheckDriverMatchingIdentifier( (D3D12_SERIALIZED_DATA_TYPE)SerializedDataType, 
															 (D3D12_SERIALIZED_DATA_DRIVER_MATCHING_IDENTIFIER *)_pIdentifierToCheck ) ;
			}
		}
	}

	public void EnumerateMetaCommandParameters( in Guid commandId, 
												MetaCommandParameterStage Stage,
												out uint pTotalStructureSizeInBytes, 
												ref uint pParameterCount,
												out Span< MetaCommandParameterDescription > pParameterDescs ) {
		uint totalSize = 0 ;
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( Guid* _commandId = &commandId ) {
				pParameterDescs = new MetaCommandParameterDescription[ (int)pParameterCount ] ;
				fixed( MetaCommandParameterDescription* descPtr = pParameterDescs ) {
					device.EnumerateMetaCommandParameters( _commandId, (D3D12_META_COMMAND_PARAMETER_STAGE)Stage, 
														  &totalSize, ref pParameterCount, 
														  (D3D12_META_COMMAND_PARAMETER_DESC*)descPtr ) ;
					pTotalStructureSizeInBytes = totalSize ;
				}
			}
		}
	}

	
	public void GetRaytracingAccelerationStructurePrebuildInfo( in  BuildRaytracingAccelerationStructureInputs  pDesc,
																out RaytracingAccelerationStructurePreBuildInfo pInfo ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			pInfo = default ;
			fixed ( void* _pDesc = &pDesc, _pInfo = &pInfo ) {
				device.GetRaytracingAccelerationStructurePrebuildInfo( (D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_INPUTS *)_pDesc,
																	   (D3D12_RAYTRACING_ACCELERATION_STRUCTURE_PREBUILD_INFO *)_pInfo ) ;
			}
		}
	}

	
	// -------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device5) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device5).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device6))]
internal class Device6: Device5,
						IDevice6,
						IComObjectRef< ID3D12Device6 >,
						IUnknownWrapper< ID3D12Device6 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device6 >? _comPtr ;
	public new ComPtr< ID3D12Device6 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device6 >( ) ;
	public override ID3D12Device6? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device6( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device6 >( ) ;
	}
	internal Device6( ComPtr< ID3D12Device6 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device6( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device6( ID3D12Device6 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------
	
	public void SetBackgroundProcessingMode( BackgroundProcessingMode Mode,
											 MeasurementsAction MeasurementsAction,
											 Win32Handle hEventToSignalUponCompletion,
											 ref bool pbFurtherMeasurementsDesired ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			BOOL  _pbFurtherMeasurementsDesired = pbFurtherMeasurementsDesired ;
			BOOL* furtherMeasurementsDesired    = &_pbFurtherMeasurementsDesired ;
			device.SetBackgroundProcessingMode( (D3D12_BACKGROUND_PROCESSING_MODE)Mode,
												(D3D12_MEASUREMENTS_ACTION)MeasurementsAction,
												hEventToSignalUponCompletion,
												furtherMeasurementsDesired ) ;
			pbFurtherMeasurementsDesired = _pbFurtherMeasurementsDesired ;
		}
	}

	// -------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device6) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device6).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device7))]
internal class Device7: Device6,
						IDevice7,
						IComObjectRef< ID3D12Device7 >,
						IUnknownWrapper< ID3D12Device7 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device7 >? _comPtr ;
	public new ComPtr< ID3D12Device7 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device7 >( ) ;
	public override ID3D12Device7? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device7( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device7 >( ) ;
	}
	internal Device7( ComPtr< ID3D12Device7 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device7( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device7( ID3D12Device7 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------

	public void AddToStateObject( in StateObjectDescription pAddition, 
								  IStateObject pStateObjectToGrowFrom, 
								  in Guid riid,
								  out IStateObject ppNewStateObject ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			var stateObject = (IComObjectRef< ID3D12StateObject >)pStateObjectToGrowFrom ;
			fixed ( StateObjectDescription* _pAddition = &pAddition ) {
				device.AddToStateObject( (D3D12_STATE_OBJECT_DESC *)_pAddition, 
										 stateObject.ComObject, 
										 &_guid, 
										 out var newStateObject ) ;
				ppNewStateObject = new StateObject( (ID3D12StateObject)newStateObject ) ;
			}
		}
	}

	public void CreateProtectedResourceSession1( in  ProtectedResourceSessionDescription1 pDesc, in Guid riid,
												 out IProtectedResourceSession1           ppSession ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			fixed ( ProtectedResourceSessionDescription1* _pDesc = &pDesc ) {
				device.CreateProtectedResourceSession1( (D3D12_PROTECTED_RESOURCE_SESSION_DESC1 *)_pDesc, 
														&_guid, 
														out var session ) ;
				ppSession = new ProtectedResourceSession1( (ID3D12ProtectedResourceSession1)session ) ;
			}
		}
	}

	
	// -------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device7) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device7).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device8))]
internal class Device8: Device7,
						IDevice8,
						IComObjectRef< ID3D12Device8 >,
						IUnknownWrapper< ID3D12Device8 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device8 >? _comPtr ;
	public new ComPtr< ID3D12Device8 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device8 >( ) ;
	public override ID3D12Device8? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device8( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device8 >( ) ;
	}
	internal Device8( ComPtr< ID3D12Device8 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device8( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device8( ID3D12Device8 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------

	public void CreateCommittedResource2( in HeapProperties pHeapProperties, 
										  HeapFlags HeapFlags, 
										  in ResourceDescription1 pDesc,
										  ResourceStates InitialResourceState, 
										  in ClearValue? pOptimizedClearValue,
										  IProtectedResourceSession pProtectedSession, 
										  in Guid riidResource,
										  out IResource ppvResource ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riidResource ;
			var protectedSession = (IComObjectRef< ID3D12ProtectedResourceSession >)pProtectedSession ;
			
			fixed ( void* _pHeapProperties = &pHeapProperties, _pDesc = &pDesc ) {
				ClearValue _clrValue = pOptimizedClearValue ?? default ;
				ClearValue* pClearValue = pOptimizedClearValue.HasValue ? (ClearValue *)&_clrValue : null ;
				
				device.CreateCommittedResource2( (D3D12_HEAP_PROPERTIES *)_pHeapProperties, 
												 (D3D12_HEAP_FLAGS)HeapFlags, 
												 (D3D12_RESOURCE_DESC1 *)_pDesc, 
												 (D3D12_RESOURCE_STATES)InitialResourceState, 
												 (D3D12_CLEAR_VALUE *)pClearValue, 
												 protectedSession.ComObject, 
												 &_guid, 
												 out var resource ) ;
				ppvResource = new Resource( (ID3D12Resource)resource ) ;
			}
		}
	}

	
	public void CreatePlacedResource1( IHeap pHeap, ulong heapOffset,
									   in ResourceDescription1 pDesc,
									   ResourceStates InitialState,
									   in ClearValue? pOptimizedClearValue,
									   in Guid riid, out IResource ppvResource ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			var heap = (IComObjectRef< ID3D12Heap >)pHeap ;
			
			fixed ( ResourceDescription1* _pDesc = &pDesc ) {
				ClearValue _clrValue = pOptimizedClearValue ?? default ;
				ClearValue* pClearValue = pOptimizedClearValue.HasValue ? (ClearValue *)&_clrValue : null ;
				
				device.CreatePlacedResource1( heap.ComObject, heapOffset, (D3D12_RESOURCE_DESC1 *)_pDesc, 
											 (D3D12_RESOURCE_STATES)InitialState, 
											 (D3D12_CLEAR_VALUE *)pClearValue, 
											 &_guid, 
											 out var resource ) ;
				ppvResource = new Resource( (ID3D12Resource)resource ) ;
			}
		}
	}

	public void GetCopyableFootprints1( in ResourceDescription1 pResourceDesc, 
										uint FirstSubresource, uint NumSubresources,
										ulong BaseOffset,
										Span< PlacedSubresourceFootprint > pLayouts,
										out Span< uint > pNumRows,
										out Span< ulong > pRowSizeInBytes,
										out ulong pTotalBytes ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( ResourceDescription1* _pResourceDesc = &pResourceDesc ) {
				fixed ( PlacedSubresourceFootprint* _pLayouts = pLayouts ) {
					fixed ( ulong* _pTotalBytes = &pTotalBytes ) {
							uint[ ] numRows = new uint[ NumSubresources ] ;
							ulong[ ] rowSizeInBytes = new ulong[ NumSubresources ] ;
							
							fixed( void* _numRows = numRows, _rowSizeInBytes = rowSizeInBytes ) {
								device.GetCopyableFootprints1( (D3D12_RESOURCE_DESC1*)_pResourceDesc,
															   FirstSubresource,
															   NumSubresources,
															   BaseOffset,
															   (D3D12_PLACED_SUBRESOURCE_FOOTPRINT*)_pLayouts,
															   (uint *)_numRows,
															   (ulong *)_rowSizeInBytes,
															   _pTotalBytes ) ;
							}
							
							pNumRows = numRows ;
							pRowSizeInBytes = rowSizeInBytes ;
					}
				}
			}
		}
	}

	
	public ResourceAllocationInfo GetResourceAllocationInfo2( uint visibleMask, uint numResourceDescs, 
															  in Span< ResourceDescription1 > pResourceDescs,
															  in Span< ResourceAllocationInfo1 > pResourceAllocationInfo1 = default ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			D3D12_RESOURCE_ALLOCATION_INFO1* allocInfo1 = null ;
			
			fixed ( ResourceDescription1* resDescs = pResourceDescs ) {
				if ( pResourceAllocationInfo1 is not { Length: > 0 } ) {
					return device.GetResourceAllocationInfo2( visibleMask, numResourceDescs,
															  (D3D12_RESOURCE_DESC1 *)resDescs, null ) ;
				}
				
				fixed( ResourceAllocationInfo1* allocInfo = pResourceAllocationInfo1 ) {
					var result = device.GetResourceAllocationInfo2( visibleMask, numResourceDescs, 
																	(D3D12_RESOURCE_DESC1 *)resDescs,
																	(D3D12_RESOURCE_ALLOCATION_INFO1 *)allocInfo ) ;
					return result ;
				}
			}
		}
	}

	public void CreateSamplerFeedbackUnorderedAccessView( IResource pTargetedResource,
														  IResource pFeedbackResource,
														  CPUDescriptorHandle DestDescriptor ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		var targetedResource = (IComObjectRef< ID3D12Resource >)pTargetedResource ;
		var feedbackResource = (IComObjectRef< ID3D12Resource >)pFeedbackResource ;
		device.CreateSamplerFeedbackUnorderedAccessView( targetedResource.ComObject, 
														 feedbackResource.ComObject, 
														 DestDescriptor ) ;
	}

	// -------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device8) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device8).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device9))]
internal class Device9: Device8,
						IDevice9,
						IComObjectRef< ID3D12Device9 >,
						IUnknownWrapper< ID3D12Device9 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device9 >? _comPtr ;
	public new ComPtr< ID3D12Device9 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device9 >( ) ;
	public override ID3D12Device9? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device9( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device9 >( ) ;
	}
	internal Device9( ComPtr< ID3D12Device9 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device9( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device9( ID3D12Device9 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------

	public void CreateCommandQueue1( in  CommandQueueDescription pDesc, 
									 in Guid CreatorID, in Guid riid,
									 out ICommandQueue ppCommandQueue ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			Guid _creatorID = CreatorID ;
			fixed ( CommandQueueDescription* _pDesc = &pDesc ) {
				device.CreateCommandQueue1( (D3D12_COMMAND_QUEUE_DESC *)_pDesc, 
											&_creatorID, 
											&_guid, 
											out var commandQueue ) ;
				ppCommandQueue = new CommandQueue( (ID3D12CommandQueue)commandQueue ) ;
			}
		}
	}

	public void ShaderCacheControl( ShaderCacheKindFlags Kinds, ShaderCacheControlFlags Control ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		device.ShaderCacheControl( (D3D12_SHADER_CACHE_KIND_FLAGS)Kinds,
								   (D3D12_SHADER_CACHE_CONTROL_FLAGS)Control ) ;
	}

	public void CreateShaderCacheSession( in  ShaderCacheSessionDescription pDesc, in Guid riid,
										  out IShaderCacheSession ppvSession ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			fixed ( ShaderCacheSessionDescription* _pDesc = &pDesc ) {
				device.CreateShaderCacheSession( (D3D12_SHADER_CACHE_SESSION_DESC *)_pDesc, 
												 &_guid, 
												 out var session ) ;
				ppvSession = new ShaderCacheSession( (ID3D12ShaderCacheSession)session ) ;
			}
		}
	}
	
	
	// -------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device9) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device9).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device10))]
internal class Device10: Device9,
						 IDevice10,
						 IComObjectRef< ID3D12Device10 >,
						 IUnknownWrapper< ID3D12Device10 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device10 >? _comPtr ;
	public new ComPtr< ID3D12Device10 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device10 >( ) ;
	public override ID3D12Device10? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device10( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device10 >( ) ;
	}
	internal Device10( ComPtr< ID3D12Device10 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device10( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device10( ID3D12Device10 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------

	public void CreateCommittedResource3( in HeapProperties pHeapProperties,
										  HeapFlags HeapFlags,
										  in ResourceDescription1 pDesc,
										  BarrierLayout InitialLayout,
										  in ClearValue? pOptimizedClearValue,
										  IProtectedResourceSession pProtectedSession,
										  uint NumCastableFormats,
										  in  Span< Format > pCastableFormats,
										  in Guid riidResource, out IResource ppvResource ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riidResource ;
			var protectedSession = (IComObjectRef< ID3D12ProtectedResourceSession >)pProtectedSession ;
			
			fixed ( void* _pHeapProperties = &pHeapProperties, _pDesc = &pDesc ) {
				ClearValue _clrValue = pOptimizedClearValue ?? default ;
				ClearValue* pClearValue = pOptimizedClearValue.HasValue ? (ClearValue *)&_clrValue : null ;
				
				fixed ( Format* _pCastableFormats = pCastableFormats ) {
					device.CreateCommittedResource3( (D3D12_HEAP_PROPERTIES *)_pHeapProperties,
													 (D3D12_HEAP_FLAGS)HeapFlags,
													 (D3D12_RESOURCE_DESC1 *)_pDesc,
													 (D3D12_BARRIER_LAYOUT)InitialLayout,
													 (D3D12_CLEAR_VALUE *)pClearValue,
													 protectedSession.ComObject,
													 NumCastableFormats,
													 (DXGI_FORMAT *)_pCastableFormats,
													 &_guid,
													 out var resource ) ;
					ppvResource = new Resource( (ID3D12Resource)resource ) ;
				}
			}
		}
	}

	
	public void CreatePlacedResource2( IHeap pHeap, ulong HeapOffset, 
									   in ResourceDescription1 pDesc, 
									   BarrierLayout InitialLayout,
									   in ClearValue?    pOptimizedClearValue, 
									   uint NumCastableFormats,
									   in Span< Format > pCastableFormats, 
									   in Guid riid, out IResource ppvResource ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			var heap = (IComObjectRef< ID3D12Heap >)pHeap ;
			
			fixed ( ResourceDescription1* _pDesc = &pDesc ) {
				ClearValue _clrValue = pOptimizedClearValue ?? default ;
				ClearValue* pClearValue = pOptimizedClearValue.HasValue ? (ClearValue *)&_clrValue : null ;
				
				fixed ( Format* _pCastableFormats = pCastableFormats ) {
					device.CreatePlacedResource2( heap.ComObject, HeapOffset, (D3D12_RESOURCE_DESC1 *)_pDesc,
												 (D3D12_BARRIER_LAYOUT)InitialLayout,
												 (D3D12_CLEAR_VALUE *)pClearValue,
												 NumCastableFormats,
												 (DXGI_FORMAT *)_pCastableFormats,
												 &_guid,
												 out var resource ) ;
					ppvResource = new Resource( (ID3D12Resource)resource ) ;
				}
			}
		}
	}

	public void CreateReservedResource2( in ResourceDescription pDesc,
										 BarrierLayout InitialLayout,
										 in ClearValue? pOptimizedClearValue, 
										 IProtectedResourceSession pProtectedSession,
										 uint NumCastableFormats,
										 in Span< Format > pCastableFormats,
										 in Guid riid, out IResource ppvResource ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			Guid _guid = riid ;
			var protectedSession = (IComObjectRef< ID3D12ProtectedResourceSession >)pProtectedSession ;
			
			fixed ( ResourceDescription* _pDesc = &pDesc ) {
				ClearValue _clrValue = pOptimizedClearValue ?? default ;
				ClearValue* pClearValue = pOptimizedClearValue.HasValue ? (ClearValue *)&_clrValue : null ;
				
				fixed ( Format* _pCastableFormats = pCastableFormats ) {
					device.CreateReservedResource2( (D3D12_RESOURCE_DESC *)_pDesc,
												   (D3D12_BARRIER_LAYOUT)InitialLayout,
												   (D3D12_CLEAR_VALUE *)pClearValue,
												   protectedSession.ComObject,
												   NumCastableFormats,
												   (DXGI_FORMAT *)_pCastableFormats,
												   &_guid,
												   out var resource ) ;
					ppvResource = new Resource( (ID3D12Resource)resource ) ;
				}
			}
		}
	}
	
	
	// -------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device10) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device10).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device11))]
internal class Device11: Device10,
						 IDevice11,
						 IComObjectRef< ID3D12Device11 >,
						 IUnknownWrapper< ID3D12Device11 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device11 >? _comPtr ;
	public new ComPtr< ID3D12Device11 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device11 >( ) ;
	public override ID3D12Device11? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------

	internal Device11( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device11 >( ) ;
	}
	internal Device11( ComPtr< ID3D12Device11 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device11( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device11( ID3D12Device11 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------

	public void CreateSampler2( in SamplerDescription2 pDesc, CPUDescriptorHandle DestDescriptor ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( SamplerDescription2* _pDesc = &pDesc ) {
				device.CreateSampler2( (D3D12_SAMPLER_DESC2 *)_pDesc, DestDescriptor ) ;
			}
		}
	}

	// -------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device11) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device11).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device12))]
internal class Device12: Device11,
						 IDevice12,
						 IComObjectRef< ID3D12Device12 >,
						 IUnknownWrapper< ID3D12Device12 > {
	// -------------------------------------------------------------------------------------------------------
	
	ComPtr< ID3D12Device12 >? _comPtr ;
	public new ComPtr< ID3D12Device12 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12Device12 >( ) ;
	public override ID3D12Device12? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------------------------
	
	internal Device12( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Device12 >( ) ;
	}
	internal Device12( ComPtr< ID3D12Device12 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device12( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Device12( ID3D12Device12 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------------------------

	public ResourceAllocationInfo GetResourceAllocationInfo3( uint visibleMask, uint numResourceDescs,
															  in ResourceDescription1 pResourceDescs, 
															  uint[ ] pNumCastableFormats,
															  in Span< Format > ppCastableFormats = default,
															  in ResourceAllocationInfo1? pResourceAllocationInfo1 = null ) {
		var device = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			fixed ( void* resDescs = &pResourceDescs,
					pFormatCounts = &pNumCastableFormats[ 0 ],
					formats = &ppCastableFormats[ 0 ] ) {
				
				DXGI_FORMAT** _formats = (ppCastableFormats is { Length: > 0 } ) 
											 ? (DXGI_FORMAT**)&formats : null ;
				
				D3D12_RESOURCE_ALLOCATION_INFO1* allocInfo1 = null ;
				if ( pResourceAllocationInfo1 is not null ) {
					fixed( void* _allocInfo1 = &pResourceAllocationInfo1 ) {
						allocInfo1 = (D3D12_RESOURCE_ALLOCATION_INFO1 *)_allocInfo1 ;
					}
				}

				return device.GetResourceAllocationInfo3( visibleMask, numResourceDescs,
														  (D3D12_RESOURCE_DESC1*)resDescs,
														  (uint*)pFormatCounts,
														  _formats,
														  allocInfo1 ) ;
			}
		}
	}

	
	// -------------------------------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Device12) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Device12).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// =======================================================================================================
} ;