﻿#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper( typeof( ID3D12Heap ) )]
public interface IHeap: IPageable {
	/// <summary>Gets the properties of a heap.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">HRESULT</a></b></para>
	/// <para>Returns one of the following values:</para>
	/// <list type="bullet">
	/// <item>
	/// <term>S_OK</term>
	/// </item>
	/// <item>
	/// <term>E_OUTOFMEMORY</term>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// <para>For more info about the heap properties, see <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/nn-d3d12-id3d12heap">
	/// ID3D12Heap interface</a> documentation (defined in: "d3d12.h").</para>
	/// </remarks>
	/// <seealso href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12heap-getdesc">ID3D12Heap::GetDesc method</seealso>
	/// <seealso href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_desc">D3D12_HEAP_DESC structure</seealso>
	void GetDesc( out HeapDescription pDesc ) ;
} ;


[Wrapper( typeof( ID3D12Heap ) )]
public interface IHeap< ID3D12 >: IHeap,
								  IPageable< ID3D12 >
	where ID3D12: ID3D12Heap, ID3D12Pageable, ID3D12DeviceChild, IUnknown { } ;

