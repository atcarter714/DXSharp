#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12CommandQueue))]
internal class CommandQueue: Pageable,
							 ICommandQueue,
							 IComObjectRef< ID3D12CommandQueue >,
							 IUnknownWrapper< ID3D12CommandQueue > {
	// ------------------------------------------------------------------------------------------
	ComPtr< ID3D12CommandQueue >? _comPtr ;
	public new virtual ComPtr< ID3D12CommandQueue >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12CommandQueue >( ) ;
	
	public override ID3D12CommandQueue? ComObject => ComPointer?.Interface ;
	ID3D12CommandQueue cmdQueue => ComObject ?? throw new NullReferenceException( ) ;

	// ------------------------------------------------------------------------------------------
	
	internal CommandQueue( ) {
		_comPtr = ComResources?.GetPointer< ID3D12CommandQueue >( ) ;
		if ( _comPtr is not null ) 
			_initOrAdd( _comPtr ) ;
	}
	
	internal CommandQueue( nint childAddr ): base( childAddr ) {
		_comPtr = ComResources?.GetPointer< ID3D12CommandQueue >( ) ;
		if ( _comPtr is not null ) 
			_initOrAdd( _comPtr ) ;
	}
	
	internal CommandQueue( ID3D12Pageable child ): base( child ) {
		_comPtr = ComResources?.GetPointer< ID3D12CommandQueue >( ) ;
		if ( _comPtr is not null ) 
			_initOrAdd( _comPtr ) ;
	}
	
	internal CommandQueue( ComPtr< IUnknown > childPtr ): base( childPtr ) {
		_comPtr = ComResources?.GetPointer< ID3D12CommandQueue >( ) ;
		if ( _comPtr is not null ) 
			_initOrAdd( _comPtr ) ;
	}
	
	
	// ------------------------------------------------------------------------------------------
	// ICommandQueue Methods ::
	// ------------------------------------------------------------------------------------------
	
	public void EndEvent( ) => cmdQueue.EndEvent( ) ;

	
	public void CopyTileMappings( IResource pDstResource, in TiledResourceCoordinate pDstRegionStartCoordinate,
								  IResource pSrcResource, in TiledResourceCoordinate pSrcRegionStartCoordinate,
								  in TileRegionSize pRegionSize, TileMappingFlags Flags ) {
		var src = (Resource) pSrcResource ;
		var dst = (Resource) pDstResource ;
		cmdQueue.CopyTileMappings( dst.ComObject, pDstRegionStartCoordinate, 
								   src.ComObject, pSrcRegionStartCoordinate, 
								   pRegionSize, (D3D12_TILE_MAPPING_FLAGS)Flags ) ;
	}

	
	public void UpdateTileMappings( IResource pResource,
									uint NumResourceRegions,
									[Optional] in Span< TiledResourceCoordinate > pResourceRegionStartCoordinates,
									[Optional] in Span< TileRegionSize > pResourceRegionSizes,
									IHeap pHeap,
									uint NumRanges,
									[Optional] in Span< TileRangeFlags > pRangeFlags,
									uint[ ] pHeapRangeStartOffsets,
									uint[ ] pRangeTileCounts,
									TileMappingFlags flags ) {
		var heap = (IComObjectRef< ID3D12Heap >) pHeap ;
		unsafe {
			var resource = (Resource) pResource ;
			fixed ( void* pRegionStartCoords = &pResourceRegionStartCoordinates[0],
						  pRegionSizes = &pResourceRegionSizes[0], pFlags = &pRangeFlags[0] ) {
				cmdQueue.UpdateTileMappings( resource.ComObject,
											 NumResourceRegions,
											 (D3D12_TILED_RESOURCE_COORDINATE *)pRegionStartCoords,
											 (D3D12_TILE_REGION_SIZE *)pRegionSizes,
											 heap.ComObject,
											 NumRanges, 
											 (D3D12_TILE_RANGE_FLAGS *)pFlags,
											 pHeapRangeStartOffsets, 
											 pRangeTileCounts,
											 (D3D12_TILE_MAPPING_FLAGS)flags ) ;
			}
		}
	}

	
	public void ExecuteCommandLists< C >( uint NumCommandLists, Span< C > ppCommandLists ) where C: ICommandList {
		var pCommandLists = new ID3D12CommandList[ NumCommandLists ] ;
		
		for ( int i = 0; i < ppCommandLists.Length; ++i ) {
			pCommandLists[ i ] = ( (IComObjectRef<ID3D12CommandList>)ppCommandLists[ i ] ).ComObject!
#if DEBUG || DEBUG_COM || DEV_BUILD
								 ?? throw new NullReferenceException( )
#endif
								 ;
		}

		cmdQueue.ExecuteCommandLists( NumCommandLists, pCommandLists ) ;
	}
	
	
	
	public unsafe void SetMarker( uint Metadata, [Optional] nint pData, uint Size ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		System.Diagnostics.Debug.Write( $"The Direct3D method " +
					 $"{nameof(ID3D12CommandQueue)}::{nameof(SetMarker)} " +
					 $"should not be called directly by applications." ) ;
#endif
		cmdQueue.SetMarker( Metadata, (void *)pData, Size ) ;
	}

	
	public void Signal( IFence pFence, ulong Value ) {
		var fence = (IComObjectRef< ID3D12Fence >) pFence ;
		cmdQueue.Signal( fence.ComObject, Value ) ;
	}

	
	public void Wait( IFence pFence, ulong Value ) {
		var fence = (IComObjectRef< ID3D12Fence >) pFence ;
		cmdQueue.Wait( fence.ComObject, Value ) ;
	}


	
	public void GetTimestampFrequency( out ulong pFrequency ) => 
		cmdQueue.GetTimestampFrequency( out pFrequency ) ;
	

	public void GetClockCalibration( out ulong pGpuTimestamp, out ulong pCpuTimestamp ) => 
		cmdQueue.GetClockCalibration( out pGpuTimestamp, out pCpuTimestamp ) ;

	
	public CommandQueueDescription GetDesc( ) => cmdQueue.GetDesc( ) ;

	
	public unsafe void BeginEvent( uint Metadata, void* pData, uint Size ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		System.Diagnostics.Debug.Write( $"The Direct3D method " +
					 $"{nameof(ID3D12CommandQueue)}::{nameof(BeginEvent)} " +
					 $"should not be called directly by applications." ) ;
#endif
		cmdQueue.BeginEvent( Metadata, pData, Size ) ;
	}
	
	// ------------------------------------------------------------------------------------------
	
	
	// --------------------------------------------------------------
	// Static Members:
	// --------------------------------------------------------------
	
	public new static Type ComType => typeof( ID3D12CommandQueue ) ;

	public new static ref readonly Guid Guid {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12CommandQueue).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}

	// ==============================================================
} ;