#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981

#region Using Directives
using DXSharp.Windows.Win32 ;
using System.Runtime.InteropServices ;
using DXSharp.Windows.COM ;

#endregion
namespace Windows.Win32.Graphics.Direct3D12;


/// <summary>
/// Part of a contract between D3D11On12 diagnostic layers and graphics diagnostics tools.
/// This interface facilitates diagnostics tools to capture information at a lower level
/// than the DXGI swapchain.
/// </summary>
/// <remarks>
/// You may want to use this interface to enable diagnostic tools to capture usage patterns that don't use DXGI swap chains for presentation.
/// If so, you can access this interface via QueryInterface from a D3D12 command queue. Note that this interface is not supported when there
/// are no diagnostic tools present, so your application mustn't rely on it existing.
/// </remarks>
[ComImport, Guid( "0ADF7D52-929C-4E61-ADDB-FFED30DE66EF" ), 
 InterfaceType( ComInterfaceType.InterfaceIsIUnknown ), ]
public interface ID3D12SharingContract: IUnknown {
	
	/// <summary>Shares a resource (or subresource) between the D3D layers and diagnostics tools.</summary>
	/// <param name="pResource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12resource">ID3D12Resource</a>*</b> A pointer to the resource that contains the final frame contents. This resource is treated as the *back buffer* of the **Present**.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12sharingcontract-present#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Subresource">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> An unsigned 32bit subresource id.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12sharingcontract-present#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="window">If provided, indicates which window the tools should use for displaying additional diagnostic information.</param>
	/// <remarks>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12sharingcontract-present">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	[PreserveSig] void Present( ID3D12Resource pResource, uint Subresource, HWnd window ) ;
	
	/// <summary>Signals a shared fence between the D3D layers and diagnostics tools.</summary>
	/// <param name="pFence">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a>*</b> A pointer to the shared fence to signal.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12sharingcontract-sharedfencesignal#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="FenceValue">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT64</a></b> An unsigned 64bit value to signal the shared fence with.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12sharingcontract-sharedfencesignal#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12sharingcontract-sharedfencesignal">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	[PreserveSig] void SharedFenceSignal( ID3D12Fence pFence, ulong FenceValue ) ;

	[PreserveSig] unsafe void BeginCapturableWork( Guid* guid ) ;
	
	[PreserveSig] unsafe void EndCapturableWork( Guid* guid ) ;
} ;
