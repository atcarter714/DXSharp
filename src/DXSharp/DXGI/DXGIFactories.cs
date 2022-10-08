#region Using Directives
using global::System;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.InteropServices.ComTypes;
using global::System.Runtime.InteropServices.WindowsRuntime;

using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi;

using global::Windows.Win32;
using Win32 = global::Windows.Win32;

using WinRT.Interop;

using DXSharp.DXGI;
using DXSharp.Windows.COM;
#endregion

namespace DXSharp.DXGI;



/// <summary>
/// Flags for making window association between
/// a SwapChain and a HWND (Window handle)
/// </summary>
[Flags] public enum MWAFlags: uint
{
	/// <summary>
	/// No flags
	/// </summary>
	None			= 0x0,
	/// <summary>
	/// Ignore all
	/// </summary>
	NoWindowChanges = 0x1,
	/// <summary>
	/// Ignore Alt+Enter
	/// </summary>
	NoAltEnter		= 0x2,
	/// <summary>
	/// Ignore Print Screen key
	/// </summary>
	NoPrintScreen	= 0x4,
	/// <summary>
	/// Valid? (Needs documentation)
	/// </summary>
	Valid			= 0x7,
};



/// <summary>
/// An IFactory is a wrapper of the native DirectX COM interface
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgifactory">IDXGIFactory</a>. 
/// The interface implements methods for generating DXGI objects (which also handle full screen transitions).
/// </summary>
public interface IFactory: IObject
{
	/// <summary>Enumerates the adapters (video cards).</summary>
	/// <param name="index">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The index of the adapter to enumerate.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppAdapter">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>**</b> The address of a pointer to an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> interface at the position specified by the <i>Adapter</i> parameter.  This parameter must not be <b>NULL</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_NOT_FOUND</a> if the index is greater than or equal to the number of adapters in the local system, or <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> if <i>ppAdapter</i> parameter is <b>NULL</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	HRESULT EnumAdapters( uint index, out IAdapter ppAdapter );

	/// <summary>Allows DXGI to monitor an application's message queue for the alt-enter key sequence (which causes the application to switch from windowed to full screen or vice versa).</summary>
	/// <param name="WindowHandle">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a></b> The handle of the window that is to be monitored. This parameter can be <b>NULL</b>; but only if *Flags* is also 0.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-makewindowassociation#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="Flags">Type: <b><a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgifactory-makewindowassociation">UINT</a></b></param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> if <i>WindowHandle</i> is invalid, or E_OUTOFMEMORY.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-makewindowassociation">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void MakeWindowAssociation( HWND WindowHandle, MWAFlags Flags );

	/// <summary>Get the window through which the user controls the transition to and from full screen.</summary>
	/// <param name="pWindowHandle">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a>*</b> A pointer to a window handle.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-getwindowassociation#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns a code that indicates success or failure. <b>S_OK</b> indicates success, <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> indicates <i>pWindowHandle</i> was passed in as <b>NULL</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-getwindowassociation">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetWindowAssociation( in HWND pWindowHandle );

	/// <summary>Creates a swap chain.</summary>
	/// <param name="pDevice">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> For Direct3D 11, and earlier versions of Direct3D, this is a pointer to the Direct3D device for the swap chain. For Direct3D 12 this is a pointer to a direct command queue (refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a>) . This parameter cannot be <b>NULL</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDesc">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_swap_chain_desc">DXGI_SWAP_CHAIN_DESC</a>*</b> A pointer to a  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_swap_chain_desc">DXGI_SWAP_CHAIN_DESC</a> structure for the swap-chain description. This parameter cannot be <b>NULL</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppSwapChain">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiswapchain">IDXGISwapChain</a>**</b> A pointer to a variable that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiswapchain">IDXGISwapChain</a> interface for the swap chain that <b>CreateSwapChain</b> creates.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b></para>
	/// <para><a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a>  if <i>pDesc</i> or <i>ppSwapChain</i> is <b>NULL</b>, DXGI_STATUS_OCCLUDED if you request full-screen mode and it is unavailable, or E_OUTOFMEMORY. Other error codes defined by the type of device passed in may also be returned.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	HRESULT CreateSwapChain( object pDevice, in SwapChainDescription pDesc, out ISwapChain ppSwapChain );

	/// <summary>Create an adapter interface that represents a software adapter.</summary>
	/// <param name="Module">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HMODULE</a></b> Handle to the software adapter's dll. HMODULE can be obtained with <a href="https://docs.microsoft.com/windows/desktop/api/libloaderapi/nf-libloaderapi-getmodulehandlea">GetModuleHandle</a> or <a href="https://docs.microsoft.com/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibrarya">LoadLibrary</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppAdapter">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>**</b> Address of a pointer to an adapter (see <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> A <a href="/windows/desktop/direct3ddxgi/dxgi-error">return code</a> indicating success or failure.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateSoftwareAdapter( HINSTANCE Module, out IAdapter ppAdapter );

};

public class Factory: Object, IFactory
{
	Factory( IDXGIFactory factory ): base(factory) { }

	public void CreateSoftwareAdapter(HINSTANCE Module, out IAdapter ppAdapter)
	{
		throw new NotImplementedException();
	}

	public HRESULT CreateSwapChain(object pDevice, in SwapChainDescription pDesc, out ISwapChain ppSwapChain)
	{
		throw new NotImplementedException();
	}

	public HRESULT EnumAdapters(uint index, out IAdapter ppAdapter)
	{
		throw new NotImplementedException();
	}

	public void GetWindowAssociation(in HWND pWindowHandle)
	{
		throw new NotImplementedException();
	}

	public void MakeWindowAssociation(HWND WindowHandle, MWAFlags Flags)
	{
		throw new NotImplementedException();
	}
}