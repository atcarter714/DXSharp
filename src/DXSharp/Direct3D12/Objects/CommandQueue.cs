#region Using Directives
using System.Diagnostics ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


public class CommandQueue: Pageable, ICommandQueue {
	public new static Type ComType => typeof( ID3D12CommandQueue ) ;
	public new static Guid InterfaceGUID => typeof( ID3D12CommandQueue ).GUID ;
	
	// ------------------------------------------------------------------------------------------
	public new ID3D12CommandQueue? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12CommandQueue >? ComPointer { get ; protected set ; }

	ID3D12CommandQueue cmdQueue => COMObject ?? throw new NullReferenceException( ) ;
	
	
	// ------------------------------------------------------------------------------------------
	// ICommandQueue Methods ::
	// ------------------------------------------------------------------------------------------
	
	public void EndEvent( ) => cmdQueue.EndEvent( ) ;

	
	public void CopyTileMappings( IResource pDstResource, in TiledResourceCoordinate pDstRegionStartCoordinate,
								  IResource pSrcResource, in TiledResourceCoordinate pSrcRegionStartCoordinate,
								  in TileRegionSize pRegionSize, TileMappingFlags Flags ) {
		cmdQueue.CopyTileMappings( pDstResource.COMObject, pDstRegionStartCoordinate, 
								   pSrcResource.COMObject, pSrcRegionStartCoordinate, 
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
		unsafe {
			fixed ( void* pRegionStartCoords = &pResourceRegionStartCoordinates[0],
						  pRegionSizes = &pResourceRegionSizes[0], pFlags = &pRangeFlags[0] ) {
				cmdQueue.UpdateTileMappings( pResource.COMObject,
											 NumResourceRegions,
											 (D3D12_TILED_RESOURCE_COORDINATE *)pRegionStartCoords,
											 (D3D12_TILE_REGION_SIZE *)pRegionSizes,
											 pHeap.COMObject, 
											 NumRanges, 
											 (D3D12_TILE_RANGE_FLAGS *)pFlags,
											 pHeapRangeStartOffsets, 
											 pRangeTileCounts,
											 (D3D12_TILE_MAPPING_FLAGS)flags ) ;
			}
		}
	}

	
	public void ExecuteCommandLists< C >( uint NumCommandLists, Span< C > ppCommandLists ) where C: ICommandList {
		
		/*var pCommandLists = new ID3D12CommandList[ NumCommandLists ] ;
		for ( int i = 0; i < NumCommandLists; i++ ) {
			
			pCommandLists[i] = ( ppCommandLists is Span<GraphicsCommandList> graphicsCommandLists ) ?
			ppCommandLists[ i ].COMObject
#if DEBUG || DEBUG_COM || DEV_BUILD
			                   ?? throw new NullReferenceException()
#endif
				;
		}*/

		if( typeof(C) == typeof(GraphicsCommandList) )
		{;
			var graphicsCommandLists = new ID3D12GraphicsCommandList[ ppCommandLists.Length ] ;
			for (int i = 0; i < ppCommandLists.Length; i++)
			{
				graphicsCommandLists[i] = (ID3D12GraphicsCommandList)
					((IGraphicsCommandList)ppCommandLists[i]).COMObject ;
			}
			cmdQueue.ExecuteCommandLists( NumCommandLists, graphicsCommandLists ) ;
			return ;
		}
		else
		{
			var commandLists = _copyAs< ID3D12GraphicsCommandList >( ppCommandLists ) ;
			cmdQueue.ExecuteCommandLists( NumCommandLists, commandLists );
			return;
		}


		TD3D[ ] _copyAs<TD3D>( Span<C> lists ) where TD3D : ID3D12CommandList {
			var listObjs = new TD3D[ lists.Length ] ;

			for (int i = 0; i < NumCommandLists; i++) {
				listObjs[i] = (TD3D)lists[i].COMObject ;
			}

			return listObjs ;
		}
	}

	
	public unsafe void SetMarker( uint Metadata, [Optional] nint pData, uint Size ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		System.Diagnostics.Debug.Write( $"The Direct3D method " +
					 $"{nameof(ID3D12CommandQueue)}::{nameof(SetMarker)} " +
					 $"should not be called directly by applications." ) ;
#endif
		cmdQueue.SetMarker( Metadata, (void *)pData, Size ) ;
	}

	
	public void Signal( IFence pFence, ulong Value ) => 
		cmdQueue.Signal( pFence.COMObject, Value ) ;

	
	public void Wait( IFence pFence, ulong Value ) => 
		cmdQueue.Wait( pFence.COMObject, Value ) ;

	
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
	
	internal CommandQueue( ) { }
	internal CommandQueue( nint ptr ) => ComPointer = new( ptr ) ;
	internal CommandQueue( ComPtr< ID3D12CommandQueue > comObject ) => ComPointer = comObject ;
	internal CommandQueue( ID3D12CommandQueue? comObject ) => ComPointer = new( comObject! ) ;
	
	
	public static IDXCOMObject Instantiate( ) => new CommandQueue( ) ;
	public static IDXCOMObject Instantiate( IntPtr pComObj ) => new CommandQueue( pComObj ) ;
	public static IDXCOMObject Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? => new CommandQueue( pComObj as ID3D12CommandQueue ) ;
} ;