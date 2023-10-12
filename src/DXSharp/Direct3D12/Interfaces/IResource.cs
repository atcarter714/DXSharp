using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;

namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Resource))]
public interface IResource: IPageable< ID3D12Resource > {
	/// <summary>Gets a CPU pointer to the specified subresource in the resource, but may not disclose the pointer value to applications. Map also invalidates the CPU cache, when necessary, so that CPU reads to this address reflect any modifications made by the GPU.</summary>
	/// <param name="Subresource">
	/// <para>Type: <b>UINT</b> Specifies the index number of the subresource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-map#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pReadRange">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_range">D3D12_RANGE</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_range">D3D12_RANGE</a> structure that describes the range of memory to access. This indicates the region the CPU might read, and the coordinates are subresource-relative. A null pointer indicates the entire subresource might be read by the CPU. It is valid to specify the CPU won't read any data by passing a range where <b>End</b> is less than or equal to <b>Begin</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-map#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppData">
	/// <para>Type: <b><b>void</b>**</b> A pointer to a memory block that receives a pointer to the resource data. A null pointer is valid and is useful to cache a CPU virtual address range for methods like <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12resource-writetosubresource">WriteToSubresource</a>. When <i>ppData</i> is not NULL, the pointer returned is never offset by any values in <i>pReadRange</i>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-map#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>Map</b> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12resource-unmap">Unmap</a> can be called by multiple threads safely. Nested <b>Map</b> calls are supported and are ref-counted. The first call to <b>Map</b> allocates a CPU virtual address range for the resource. The last call to <b>Unmap</b> deallocates the CPU virtual address range. The CPU virtual address is commonly returned to the application; but manipulating the contents of textures with unknown layouts precludes disclosing the CPU virtual address. See <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12resource-writetosubresource">WriteToSubresource</a> for more details. Applications cannot rely on the address being consistent, unless <b>Map</b> is persistently nested.</para>
	/// <para>Pointers returned by <b>Map</b> are not guaranteed to have all the capabilities of normal pointers, but most applications won't notice a difference in normal usage. For example, pointers with WRITE_COMBINE behavior have weaker CPU memory ordering guarantees than WRITE_BACK behavior. Memory accessible by both CPU and GPU are not guaranteed to share the same atomic memory guarantees that the CPU has, due to PCIe limitations. Use fences for synchronization.</para>
	/// <para>There are two usage model categories for <b>Map</b>, simple and advanced. The simple usage models maximize tool performance, so applications are recommended to stick with the simple models until the advanced models are proven to be required by the app.</para>
	/// <para><h3><a id="Simple_Usage_Models"></a><a id="simple_usage_models"></a><a id="SIMPLE_USAGE_MODELS"></a>Simple Usage Models</h3> Applications should stick to the heap type abstractions of UPLOAD, DEFAULT, and READBACK, in order to support all adapter architectures reasonably well. Applications should avoid CPU reads from pointers to resources on UPLOAD heaps, even accidently. CPU reads will work, but are prohibitively slow on many common GPU architectures, so consider the following: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-map#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void Map( uint Subresource, [Optional] in Range pReadRange, out nint ppData ) ;
	
	/// <summary>Invalidates the CPU pointer to the specified subresource in the resource.</summary>
	/// <param name="Subresource">
	/// <para>Type: <b>UINT</b> Specifies the index of the subresource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-unmap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pWrittenRange">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_range">D3D12_RANGE</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_range">D3D12_RANGE</a> structure that describes the range of memory to unmap. This indicates the region the CPU might have modified, and the coordinates are subresource-relative. A null pointer indicates the entire subresource might have been modified by the CPU. It is valid to specify the CPU didn't write any data by passing a range where <b>End</b> is less than or equal to <b>Begin</b>. This parameter is only used by tooling, and not for correctness of the actual unmap operation.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-unmap#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>Refer to the extensive Remarks and Examples for the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12resource-map">Map</a> method.</remarks>
	void Unmap( uint Subresource, [Optional] in Range pWrittenRange ) ;
	
	/// <summary>Gets the resource description.</summary>
	/// <returns>
	/// <para>This method has no parameters.</para>
	/// <para>Type: **[**D3D12\_RESOURCE\_DESC**](/windows/desktop/api/d3d12/ns-d3d12-d3d12_resource_desc)** A Direct3D 12 resource description structure.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/direct3d12/id3d12resource-getdesc">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	ResourceDescription GetDesc( ) ;

	/// <summary>This method returns the GPU virtual address of a buffer resource.</summary>
	/// <returns>
	/// <para>Type: <b>D3D12_GPU_VIRTUAL_ADDRESS</b> This method returns the GPU virtual address. D3D12_GPU_VIRTUAL_ADDRESS is a typedef'd synonym of UINT64.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method is only useful for buffer resources, it will return zero for all texture resources. For more information on the use of GPU virtual addresses, refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/indirect-drawing">Indirect Drawing</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-getgpuvirtualaddress#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	ulong GetGPUVirtualAddress( ) ;
	
	/// <summary>Uses the CPU to copy data into a subresource, enabling the CPU to modify the contents of most textures with undefined layouts.</summary>
	/// <param name="DstSubresource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Specifies the index of the subresource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-writetosubresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDstBox">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_box">D3D12_BOX</a>*</b> A pointer to a box that defines the portion of the destination subresource to copy the resource data into. If NULL, the data is written to the destination subresource with no offset. The dimensions of the source must fit the destination (see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_box">D3D12_BOX</a>).</para>
	/// <para>An empty box results in a no-op. A box is empty if the top value is greater than or equal to the bottom value, or the left value is greater than or equal to the right value, or the front value is greater than or equal to the back value. When the box is empty, this method doesn't perform any operation.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-writetosubresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pSrcData">
	/// <para>Type: <b>const void*</b> A pointer to the source data in memory.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-writetosubresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="SrcRowPitch">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The distance from one row of source data to the next row.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-writetosubresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="SrcDepthPitch">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The distance from one depth slice of source data to the next.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-writetosubresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>The resource should first be mapped using <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12resource-map">Map</a>. Textures must be in the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_states">D3D12_RESOURCE_STATE_COMMON</a> state for CPU access through <b>WriteToSubresource</b> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12resource-readfromsubresource">ReadFromSubresource</a> to be legal; but buffers do not. For efficiency, ensure the bounds and alignment of the extents within the box are ( 64 / [bytes per pixel] ) pixels horizontally. Vertical bounds and alignment should be 2 rows, except when 1-byte-per-pixel formats are used, in which case 4 rows are recommended. Single depth slices per call are handled efficiently. It is recommended but not necessary to provide pointers and strides which are 128-byte aligned.</para>
	/// <para>When writing to sub mipmap levels, it is recommended to use larger width and heights than described above. This is because small mipmap levels may actually be stored within a larger block of memory, with an opaque amount of offsetting which can interfere with alignment to cache lines.</para>
	/// <para><b>WriteToSubresource</b> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12resource-readfromsubresource">ReadFromSubresource</a> enable near zero-copy optimizations for UMA adapters, but can prohibitively impair the efficiency of discrete/ NUMA adapters as the texture data cannot reside in local video memory. Typical applications should stick to discrete-friendly upload techniques, unless they recognize the adapter architecture is UMA. For more details on uploading, refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-copytextureregion">CopyTextureRegion</a>, and for more details on UMA, refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_feature_data_architecture">D3D12_FEATURE_DATA_ARCHITECTURE</a>. On UMA systems, this routine can be used to minimize the cost of memory copying through the loop optimization known as <a href="https://en.wikipedia.org/wiki/Loop_tiling">loop tiling</a>. By breaking up the upload into chucks that comfortably fit in the CPU cache, the effective bandwidth between the CPU and main memory more closely achieves theoretical maximums.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-writetosubresource#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void WriteToSubresource( uint DstSubresource, out Box pDstBox, nint pSrcData, uint SrcRowPitch, uint SrcDepthPitch ) ;

	/// <summary>Uses the CPU to copy data from a subresource, enabling the CPU to read the contents of most textures with undefined layouts.</summary>
	/// <param name="pDstData">
	/// <para>Type: <b>void*</b> A pointer to the destination data in memory.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-readfromsubresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DstRowPitch">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The distance from one row of destination data to the next row.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-readfromsubresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DstDepthPitch">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The distance from one depth slice of destination data to the next.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-readfromsubresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="SrcSubresource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Specifies the index of the subresource to read from.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-readfromsubresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pSrcBox">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_box">D3D12_BOX</a>*</b> A pointer to a box that defines the portion of the destination subresource to copy the resource data from. If NULL, the data is read from the destination subresource with no offset. The dimensions of the destination must fit the destination (see <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_box">D3D12_BOX</a>).</para>
	/// <para>An empty box results in a no-op. A box is empty if the top value is greater than or equal to the bottom value, or the left value is greater than or equal to the right value, or the front value is greater than or equal to the back value. When the box is empty, this method doesn't perform any operation.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-readfromsubresource#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>See the Remarks section for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12resource-writetosubresource">WriteToSubresource</a>.</remarks>
	void ReadFromSubresource( nint pDstData, uint DstRowPitch, uint DstDepthPitch, uint SrcSubresource, in Box? pSrcBox = null ) ;

	/// <summary>Retrieves the properties of the resource heap, for placed and committed resources.</summary>
	/// <param name="pHeapProperties">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_heap_properties">D3D12_HEAP_PROPERTIES</a>*</b> Pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_heap_properties">D3D12_HEAP_PROPERTIES</a> structure, that on successful completion of the method will contain the resource heap properties.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-getheapproperties#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pHeapFlags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_heap_flags">D3D12_HEAP_FLAGS</a>*</b> Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_heap_flags">D3D12_HEAP_FLAGS</a> variable, that on successful completion of the method will contain any miscellaneous heap flags.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-getheapproperties#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>. If the resource was created as reserved, E_INVALIDARG is returned.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method only works on placed and committed resources, not on reserved resources. If the resource was created as reserved, E_INVALIDARG is returned. The pages could be mapped to none, one, or more heaps.</para>
	/// <para>For more information, refer to <a href="https://docs.microsoft.com/windows/desktop/direct3d12/memory-management">Memory Management in Direct3D 12</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12resource-getheapproperties#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetHeapProperties( out HeapProperties pHeapProperties, out HeapFlags pHeapFlags ) ;

} ;