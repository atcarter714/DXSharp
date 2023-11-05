#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>
/// Provides methods for submitting command lists, synchronizing command list execution,
/// instrumenting the command queue, and updating resource tile mappings.
/// </summary>
[ProxyFor(typeof(ID3D12CommandQueue))]
public interface ICommandQueue: IPageable {
	
	/// <summary>Copies mappings from a source reserved resource to a destination reserved resource.</summary>
	/// <param name="pDstResource">A pointer to the destination reserved resource.</param>
	/// <param name="pDstRegionStartCoordinate">
	/// <para>A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tiled_resource_coordinate">D3D12_TILED_RESOURCE_COORDINATE</a> structure that describes the starting coordinates of the destination reserved resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-copytilemappings#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pSrcResource">A pointer to the source reserved resource.</param>
	/// <param name="pSrcRegionStartCoordinate">A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tiled_resource_coordinate">D3D12_TILED_RESOURCE_COORDINATE</a> structure that describes the starting coordinates of the source reserved resource.</param>
	/// <param name="pRegionSize">A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tile_region_size">D3D12_TILE_REGION_SIZE</a> structure that describes the size of the reserved region.</param>
	/// <param name="Flags">One member of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_tile_mapping_flags">D3D12_TILE_MAPPING_FLAGS</a>.</param>
	/// <remarks>
	/// <para>Use <b>CopyTileMappings</b> to copy the tile mappings from one reserved resource to another, either to duplicate a resource mapping, or to initialize a new mapping before modifying it using <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-updatetilemappings">UpdateTileMappings</a>. <b>CopyTileMappings</b> helps with tasks such as shifting mappings around within and across reserved resources, for example, scrolling tiles. The source and destination regions can overlap; the result of the copy in this situation is as if the source was saved to a temporary location and from there written to the destination.</para>
	/// <para>The destination and the source regions must each entirely fit in their resource or behavior is undefined and the debug layer will emit an error.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-copytilemappings#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CopyTileMappings( 
			IResource pDstResource,
			in TiledResourceCoordinate pDstRegionStartCoordinate,
			IResource pSrcResource,
			in TiledResourceCoordinate pSrcRegionStartCoordinate,
			in TileRegionSize pRegionSize,
			TileMappingFlags Flags
	) ;
	
	
	/// <summary>Updates mappings of tile locations in reserved resources to memory locations in a resource heap.</summary>
	/// <param name="pResource">A pointer to the reserved resource.</param>
	/// <param name="NumResourceRegions">The number of reserved resource regions.</param>
	/// <param name="pResourceRegionStartCoordinates">An array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tiled_resource_coordinate">D3D12_TILED_RESOURCE_COORDINATE</a> structures that describe the starting coordinates of the reserved resource regions. The <i>NumResourceRegions</i> parameter specifies the number of <b>D3D12_TILED_RESOURCE_COORDINATE</b> structures in the array.</param>
	/// <param name="pResourceRegionSizes">An array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_tile_region_size">D3D12_TILE_REGION_SIZE</a> structures that describe the sizes of the reserved resource regions. The <i>NumResourceRegions</i> parameter specifies the number of <b>D3D12_TILE_REGION_SIZE</b> structures in the array.</param>
	/// <param name="pHeap">A pointer to the resource heap.</param>
	/// <param name="NumRanges">The number of tile  ranges.</param>
	/// <param name="pRangeFlags">A pointer to an  array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_tile_range_flags">D3D12_TILE_RANGE_FLAGS</a> values that describes each tile range. The <i>NumRanges</i> parameter specifies the number of values in the array.</param>
	/// <param name="pHeapRangeStartOffsets">An array of offsets into the resource heap. These are 0-based tile offsets, counting in tiles (not bytes).</param>
	/// <param name="pRangeTileCounts">
	/// <para>An array of tiles. An array of values that specify the number of tiles in each tile range. The <i>NumRanges</i> parameter specifies the number of values in the array.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-updatetilemappings#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="flags">A combination of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_tile_mapping_flags">D3D12_TILE_MAPPING_FLAGS</a> values that are combined by using a bitwise OR operation.</param>
	/// <remarks>
	/// <para>Use <b>UpdateTileMappings</b> to map the virtual pages of a reserved resource to the physical pages of a heap. The mapping does not have to be in order. The operation is similar to  <a href="https://docs.microsoft.com/windows/desktop/api/d3d11_2/nf-d3d11_2-id3d11devicecontext2-updatetilemappings">ID3D11DeviceContext2::UpdateTileMappings</a> with the one key difference that D3D12 allows a reserved resource to have tiles from multiple heaps. In a single call to <b>UpdateTileMappings</b>, you can map one or more ranges of resource tiles to one or more ranges of heap tiles.</para>
	/// <para>You can organize the parameters of  <b>UpdateTileMappings</b> in these ways to perform an update:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-updatetilemappings#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void UpdateTileMappings( IResource pResource,
							 uint NumResourceRegions,
							 [Optional] in Span< TiledResourceCoordinate > pResourceRegionStartCoordinates,
							 [Optional] in Span< TileRegionSize > pResourceRegionSizes,
							 IHeap pHeap,
							 uint NumRanges,
							 [Optional] in Span< TileRangeFlags > pRangeFlags,
							 uint[ ] pHeapRangeStartOffsets,
							 uint[ ] pRangeTileCounts,
							 TileMappingFlags flags ) ;
	

	/// <summary>Submits an array of command lists for execution.</summary>
	/// <param name="NumCommandLists">The number of command lists to be executed.</param>
	/// <param name="ppCommandLists">The array of <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandlist">ID3D12CommandList</a> command lists to be executed.</param>
	/// <remarks>
	/// <para>Calling **ExecuteCommandLists** twice in succession (from the same thread, or different threads) guarantees that the first workload (A) finishes before the second workload (B). Calling **ExecuteCommandLists** with *two* command lists allows the driver to merge the two command lists such that the second command list (D) may begin executing work before all work from the first (C) has finished. Specifically, your application is allowed to insert a fence signal or wait between A and B, and the driver has no visibility into this, so the driver must ensure that everything in A is complete before the fence operation. There is no such opportunity in a single call to the API, so the driver is able to optimize that scenario. The driver is free to patch the submitted command lists. It is the calling application’s responsibility to ensure that the graphics processing unit (GPU) is not currently reading the any of the submitted command lists from a previous execution. Applications are encouraged to batch together command list executions to reduce fixed costs associated with submitted commands to the GPU.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-executecommandlists#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void ExecuteCommandLists< C >( uint NumCommandLists, Span< C > ppCommandLists )
														where C: ICommandList ;
	
	
	/// <summary>Not intended to be called directly. Use the PIX event runtime to insert events into a command queue. (ID3D12CommandQueue.SetMarker)</summary>
	/// <param name="Metadata">
	/// <para>Type: <b>UINT</b> Internal.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-setmarker#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>const void*</b> Internal.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-setmarker#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Size">
	/// <para>Type: <b>UINT</b> Internal.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-setmarker#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para>This is a support method used internally by the PIX event runtime. It is not intended to be called directly. To insert instrumentation markers at the current location within a D3D12 command queue, use the <b>PIXSetMarker</b> function. This is provided by the <a href="https://devblogs.microsoft.com/pix/winpixeventruntime/">WinPixEventRuntime</a> NuGet package.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-setmarker#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetMarker( uint Metadata, [Optional] nint pData, uint Size ) ;
	
	
	/// <summary>Updates a fence to a specified value.</summary>
	/// <param name="pFence">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a> object.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-signal#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Value">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a></b> The value to set the fence to.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-signal#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>Use this method to set a fence value from the GPU side. Use <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12fence-signal">ID3D12Fence::Signal</a> to set a fence from the CPU side.</remarks>
	void Signal( IFence pFence, ulong Value ) ;
	
	
	/// <summary>Queues a GPU-side wait, and returns immediately. A GPU-side wait is where the GPU waits until the specified fence reaches or exceeds the specified value.</summary>
	/// <param name="pFence">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a>*</b> A pointer to the <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a> object.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-wait#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Value">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a></b> The value that the command queue is waiting for the fence to reach or exceed.  So when  <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12fence-getcompletedvalue">ID3D12Fence::GetCompletedValue</a> is greater than or equal to <i>Value</i>, the wait is terminated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-wait#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>Because a wait is being queued, the API returns immediately. It's the command queue that waits (during which time no work is executed) until the fence specified reaches the requested value. If you want to perform a CPU-side wait (where the calling thread blocks until a fence reaches a particular value), then you should use the [**ID3D12Fence::SetEventOnCompletion**](./nf-d3d12-id3d12fence-seteventoncompletion.md) API in conjunction with [**WaitForSingleObject**](../synchapi/nf-synchapi-waitforsingleobject.md) (or a similar API).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-wait#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void Wait( IFence pFence, ulong Value ) ;

	
	/// <summary>This method is used to determine the rate at which the GPU timestamp counter increments.</summary>
	/// <param name="pFrequency">
	/// <para>Type: <b>UINT64*</b> The GPU timestamp counter frequency (in ticks/second).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-gettimestampfrequency#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>For more information, refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/timing">Timing</a>.</remarks>
	void GetTimestampFrequency( out ulong pFrequency ) ;

	
	/// <summary>This method samples the CPU and GPU timestamp counters at the same moment in time.</summary>
	/// <param name="pGpuTimestamp">
	/// <para>Type: <b>UINT64*</b> The value of the GPU timestamp counter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-getclockcalibration#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pCpuTimestamp">
	/// <para>Type: <b>UINT64*</b> The value of the CPU timestamp counter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-getclockcalibration#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>For more information, refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/timing">Timing</a>.</remarks>
	void GetClockCalibration( out ulong pGpuTimestamp, out ulong pCpuTimestamp ) ;

	
	/// <summary>Gets the description of the command queue.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_command_queue_desc">D3D12_COMMAND_QUEUE_DESC</a></b> The description of the command queue, as a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_command_queue_desc">D3D12_COMMAND_QUEUE_DESC</a> structure.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-getdesc">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	CommandQueueDescription GetDesc( ) ;
	
	
	
	// ---------------------------------------
	// INTERNAL CALLS (USE PIX EVENTS INSTEAD
	// ---------------------------------------
	
	/// <summary>Not intended to be called directly. Use the PIX event runtime to insert events into a command queue. (ID3D12CommandQueue.BeginEvent)</summary>
	/// <param name="Metadata">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Internal.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-beginevent#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>const void*</b> Internal.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-beginevent#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Size">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Internal.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-beginevent#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para>This is a support method used internally by the PIX event runtime. It is not intended to be called directly. To mark the start of an instrumentation region at the current location within a D3D12 command queue, use the <b>PIXBeginEvent</b> function or <b>PIXScopedEvent</b> macro. These are provided by the <a href="https://devblogs.microsoft.com/pix/winpixeventruntime/">WinPixEventRuntime</a> NuGet package.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-beginevent#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void BeginEvent( uint Metadata, void* pData, uint Size ) ;

	/// <summary>Not intended to be called directly. Use the PIX event runtime to insert events into a command queue. (ID3D12CommandQueue.EndEvent)</summary>
	/// <remarks>
	/// <para>This is a support method used internally by the PIX event runtime. It is not intended to be called directly. To mark the end of an instrumentation region at the current location within a D3D12 command queue, use the <b>PIXEndEvent</b> function or <b>PIXScopedEvent</b> macro. These are provided by the <a href="https://devblogs.microsoft.com/pix/winpixeventruntime/">WinPixEventRuntime</a> NuGet package.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-endevent#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void EndEvent( ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( ID3D12CommandQueue ) ;
	public new static Guid IID => (ComType.GUID) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12CommandQueue ).GUID
																	 .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) );
		}
	}
	
	public static IDXCOMObject Instantiate( ) => new CommandQueue( ) ;
	public static IDXCOMObject Instantiate( IntPtr pComObj ) => new CommandQueue( pComObj ) ;
	public static IDXCOMObject Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? => 
		new CommandQueue( ( pComObj as ID3D12CommandQueue )! ) ;
	
	// ==================================================================================
} ;
