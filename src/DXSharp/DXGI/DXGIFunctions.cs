#region Using Directives
using global::System.Runtime.CompilerServices;

/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using global::System.Runtime.InteropServices;
After:
using global::System.Runtime.InteropServices;
using global::Windows.Win32;
*/
using global::System.Runtime.InteropServices;
/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using Windows.Win32.Foundation;
After:
using System.Runtime.Versioning;

using Windows.Win32.Foundation;
*/

using global::Windows.Win32;

using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi;
/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using global::Windows.Win32;
using Win32 = global::Windows.Win32;
using System.Runtime.Versioning;
#endregion
After:
using Win32 = global::Windows.Win32;
#endregion
*/

#endregion

namespace DXSharp.DXGI;

/// <summary>
/// Valid values include the DXGI_CREATE_FACTORY_DEBUG (0x01) flag, and zero.
/// </summary>
/// <remarks>
/// <b><h3>Note:</h3></b> 
/// This flag will be set by the D3D runtime if:
/// The system creates an implicit factory during device creation.
/// The D3D11_CREATE_DEVICE_DEBUG flag is specified during device creation, 
/// for example using D3D11CreateDevice (or the swapchain method, or the 
/// Direct3D 10 equivalents).
/// </remarks>
public enum FactoryCreateFlags: uint
{
	/// <summary>
	/// No flags
	/// </summary>
	None = 0x00,
	/// <summary>
	/// Enable debug layer
	/// </summary>
	DEBUG = 0x01,
};


/// <summary>
/// Defines the DXGI-related functions of the Windows SDK
/// </summary>
/// <remarks>
/// See the documentation for the 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/d3d10-graphics-reference-dxgi-functions">DXGI functions</a> 
/// for a complete list with additional information
/// </remarks>
public static partial class DXGIFunctions
{
	// Native Interop Signatures:
	//HRESULT CreateDXGIFactory( in global::System.Guid riid, out object ppFactory );
	//HRESULT CreateDXGIFactory( global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppFactory );
	//HRESULT CreateDXGIFactory1( in global::System.Guid riid, out object ppFactory );
	//HRESULT CreateDXGIFactory1( global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppFactory );
	//HRESULT CreateDXGIFactory2( uint Flags, in global::System.Guid riid, out object ppFactory );
	//HRESULT CreateDXGIFactory2( uint Flags, global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppFactory );

	//HRESULT DXGIGetDebugInterface1( uint Flags, in global::System.Guid riid, out object pDebug );
	//HRESULT DXGIGetDebugInterface1( uint Flags, global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object pDebug );
	//HRESULT DXGIDeclareAdapterRemovalSupport();

	#region Internal Methods

	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
	internal static T? CreateDXGIFactory<T>( out HRESULT hr ) where T : IDXGIFactory {
		unsafe {
			var riid = typeof( T ).GUID;
			hr = PInvoke.CreateDXGIFactory( &riid, out object? factoryObj );
			return hr.Succeeded ? (T)factoryObj : default( T );
		}
	}

	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	/// <exception cref="COMException">Thrown if the call fails and contains detailed error information</exception>
	[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
	internal static T? CreateDXGIFactory<T>() where T : IDXGIFactory {
		var factory = DXGIFunctions.CreateDXGIFactory<T>( out var hr );
		_ = hr.ThrowOnFailure();
		return factory;
	}


	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
	internal static T? CreateDXGIFactory1<T>( out HRESULT hr ) where T : IDXGIFactory {
		unsafe {
			var riid = typeof( T ).GUID;
			hr = PInvoke.CreateDXGIFactory1( &riid, out object? factoryObj );
			return hr.Succeeded ? (T)factoryObj : default( T );
		}
	}

	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	/// <exception cref="COMException">Thrown if the call fails and contains detailed error information</exception>
	[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
	internal static T? CreateDXGIFactory1<T>() where T : IDXGIFactory {
		var factory = CreateDXGIFactory1<T>( out var hr );
		_ = hr.ThrowOnFailure();
		return factory;
	}


	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="Flags">Creation flags</param>
	/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
	internal static T? CreateDXGIFactory2<T>( FactoryCreateFlags Flags, out HRESULT hr ) where T : IDXGIFactory2 {
		unsafe {
			var riid = typeof( T ).GUID;
			hr = PInvoke.CreateDXGIFactory2( (uint)Flags, &riid, out object? factoryObj );
			return hr.Succeeded ? (T)factoryObj : default( T );
		}
	}

	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="Flags">Creation flags</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
	internal static T? CreateDXGIFactory2<T>( FactoryCreateFlags Flags ) where T : IDXGIFactory2 {
		var factory = CreateDXGIFactory2<T>( Flags, out var hr );
		_ = hr.ThrowOnFailure();
		return factory;
	}

	#endregion


};


//public interface IFactory: IObject
//{
//	/// <summary>Enumerates the adapters (video cards).</summary>
//	/// <param name="Adapter">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The index of the adapter to enumerate.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <param name="ppAdapter">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>**</b> The address of a pointer to an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> interface at the position specified by the <i>Adapter</i> parameter.  This parameter must not be <b>NULL</b>.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <returns>
//	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_NOT_FOUND</a> if the index is greater than or equal to the number of adapters in the local system, or <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> if <i>ppAdapter</i> parameter is <b>NULL</b>.</para>
//	/// </returns>
//	/// <remarks>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters">Learn more about this API from docs.microsoft.com</see>.</para>
//	/// </remarks>
//	void EnumAdapters(uint Adapter, out DXSharp.DXGI.IAdapter ppAdapter);

//	/// <summary>Allows DXGI to monitor an application's message queue for the alt-enter key sequence (which causes the application to switch from windowed to full screen or vice versa).</summary>
//	/// <param name="WindowHandle">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a></b> The handle of the window that is to be monitored. This parameter can be <b>NULL</b>; but only if *Flags* is also 0.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-makewindowassociation#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <param name="Flags">Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b></param>
//	/// <returns>
//	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> if <i>WindowHandle</i> is invalid, or E_OUTOFMEMORY.</para>
//	/// </returns>
//	/// <remarks>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-makewindowassociation">Learn more about this API from docs.microsoft.com</see>.</para>
//	/// </remarks>
//	void MakeWindowAssociation(HWND WindowHandle, uint Flags);

//	/// <summary>Get the window through which the user controls the transition to and from full screen.</summary>
//	/// <param name="pWindowHandle">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a>*</b> A pointer to a window handle.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-getwindowassociation#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <returns>
//	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns a code that indicates success or failure. <b>S_OK</b> indicates success, <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> indicates <i>pWindowHandle</i> was passed in as <b>NULL</b>.</para>
//	/// </returns>
//	/// <remarks>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-getwindowassociation">Learn more about this API from docs.microsoft.com</see>.</para>
//	/// </remarks>
//	void GetWindowAssociation(in HWND pWindowHandle);

//	/// <summary>Creates a swap chain.</summary>
//	/// <param name="pDevice">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> For Direct3D 11, and earlier versions of Direct3D, this is a pointer to the Direct3D device for the swap chain. For Direct3D 12 this is a pointer to a direct command queue (refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a>) . This parameter cannot be <b>NULL</b>.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <param name="pDesc">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_swap_chain_desc">DXGI_SWAP_CHAIN_DESC</a>*</b> A pointer to a  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_swap_chain_desc">DXGI_SWAP_CHAIN_DESC</a> structure for the swap-chain description. This parameter cannot be <b>NULL</b>.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <param name="ppSwapChain">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiswapchain">IDXGISwapChain</a>**</b> A pointer to a variable that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiswapchain">IDXGISwapChain</a> interface for the swap chain that <b>CreateSwapChain</b> creates.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <returns>
//	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b></para>
//	/// <para><a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a>  if <i>pDesc</i> or <i>ppSwapChain</i> is <b>NULL</b>, DXGI_STATUS_OCCLUDED if you request full-screen mode and it is unavailable, or E_OUTOFMEMORY. Other error codes defined by the type of device passed in may also be returned.</para>
//	/// </returns>
//	/// <remarks>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain">Learn more about this API from docs.microsoft.com</see>.</para>
//	/// </remarks>
//	HRESULT CreateSwapChain(object pDevice, in SwapChainDescription pDesc, out DXSharp.DXGI.ISwapChain ppSwapChain);

//	/// <summary>Create an adapter interface that represents a software adapter.</summary>
//	/// <param name="Module">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HMODULE</a></b> Handle to the software adapter's dll. HMODULE can be obtained with <a href="https://docs.microsoft.com/windows/desktop/api/libloaderapi/nf-libloaderapi-getmodulehandlea">GetModuleHandle</a> or <a href="https://docs.microsoft.com/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibrarya">LoadLibrary</a>.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <param name="ppAdapter">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>**</b> Address of a pointer to an adapter (see <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>).</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <returns>
//	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> A <a href="/windows/desktop/direct3ddxgi/dxgi-error">return code</a> indicating success or failure.</para>
//	/// </returns>
//	/// <remarks>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter">Learn more about this API from docs.microsoft.com</see>.</para>
//	/// </remarks>
//	void CreateSoftwareAdapter( HINSTANCE Module, out DXSharp.DXGI.IAdapter ppAdapter ) { }
//};


//public class DXGIFunctions
//{
//	IDXGIFactory factory;

//	/// <summary>
//	/// Gets the GUID of the underlying COM interface IDXGIFactory
//	/// </summary>
//	public static Guid COM_GUID => typeof( IDXGIFactory ).GUID;

//	public virtual void Dispose() { }

//	public void SetPrivateData<T>( uint DataSize, IntPtr pData ) { }
//	public void SetPrivateDataInterface<T>( object pUnknown ) { }
//	public void GetPrivateData<T>( in uint pDataSize, IntPtr pData ) { }
//	//public void GetParent<T>( out object ppParent ) { }

//	/// <summary>Enumerates the adapters (video cards).</summary>
//	/// <param name="Adapter">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The index of the adapter to enumerate.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <param name="ppAdapter">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>**</b> The address of a pointer to an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> interface at the position specified by the <i>Adapter</i> parameter.  This parameter must not be <b>NULL</b>.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <returns>
//	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_NOT_FOUND</a> if the index is greater than or equal to the number of adapters in the local system, or <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> if <i>ppAdapter</i> parameter is <b>NULL</b>.</para>
//	/// </returns>
//	/// <remarks>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters">Learn more about this API from docs.microsoft.com</see>.</para>
//	/// </remarks>
//	//public void EnumAdapters( uint Adapter, out DXSharp.DXGI.IAdapter ppAdapter ) { }

//	/// <summary>Allows DXGI to monitor an application's message queue for the alt-enter key sequence (which causes the application to switch from windowed to full screen or vice versa).</summary>
//	/// <param name="WindowHandle">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a></b> The handle of the window that is to be monitored. This parameter can be <b>NULL</b>; but only if *Flags* is also 0.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-makewindowassociation#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <param name="Flags">Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b></param>
//	/// <returns>
//	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> if <i>WindowHandle</i> is invalid, or E_OUTOFMEMORY.</para>
//	/// </returns>
//	/// <remarks>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-makewindowassociation">Learn more about this API from docs.microsoft.com</see>.</para>
//	/// </remarks>
//	public void MakeWindowAssociation( HWND WindowHandle, uint Flags ) { }

//	/// <summary>Get the window through which the user controls the transition to and from full screen.</summary>
//	/// <param name="pWindowHandle">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a>*</b> A pointer to a window handle.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-getwindowassociation#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <returns>
//	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns a code that indicates success or failure. <b>S_OK</b> indicates success, <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> indicates <i>pWindowHandle</i> was passed in as <b>NULL</b>.</para>
//	/// </returns>
//	/// <remarks>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-getwindowassociation">Learn more about this API from docs.microsoft.com</see>.</para>
//	/// </remarks>
//	public void GetWindowAssociation( in HWND pWindowHandle ) { }

//	/// <summary>Creates a swap chain.</summary>
//	/// <param name="pDevice">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> For Direct3D 11, and earlier versions of Direct3D, this is a pointer to the Direct3D device for the swap chain. For Direct3D 12 this is a pointer to a direct command queue (refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a>) . This parameter cannot be <b>NULL</b>.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <param name="pDesc">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_swap_chain_desc">DXGI_SWAP_CHAIN_DESC</a>*</b> A pointer to a  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_swap_chain_desc">DXGI_SWAP_CHAIN_DESC</a> structure for the swap-chain description. This parameter cannot be <b>NULL</b>.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <param name="ppSwapChain">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiswapchain">IDXGISwapChain</a>**</b> A pointer to a variable that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiswapchain">IDXGISwapChain</a> interface for the swap chain that <b>CreateSwapChain</b> creates.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <returns>
//	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b></para>
//	/// <para><a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a>  if <i>pDesc</i> or <i>ppSwapChain</i> is <b>NULL</b>, DXGI_STATUS_OCCLUDED if you request full-screen mode and it is unavailable, or E_OUTOFMEMORY. Other error codes defined by the type of device passed in may also be returned.</para>
//	/// </returns>
//	/// <remarks>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain">Learn more about this API from docs.microsoft.com</see>.</para>
//	/// </remarks>
//	//public HRESULT CreateSwapChain( object pDevice, in SwapChainDescription pDesc, out DXSharp.DXGI.ISwapChain ppSwapChain ) { }

//	/// <summary>Create an adapter interface that represents a software adapter.</summary>
//	/// <param name="Module">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HMODULE</a></b> Handle to the software adapter's dll. HMODULE can be obtained with <a href="https://docs.microsoft.com/windows/desktop/api/libloaderapi/nf-libloaderapi-getmodulehandlea">GetModuleHandle</a> or <a href="https://docs.microsoft.com/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibrarya">LoadLibrary</a>.</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <param name="ppAdapter">
//	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>**</b> Address of a pointer to an adapter (see <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>).</para>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter#parameters">Read more on docs.microsoft.com</see>.</para>
//	/// </param>
//	/// <returns>
//	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> A <a href="/windows/desktop/direct3ddxgi/dxgi-error">return code</a> indicating success or failure.</para>
//	/// </returns>
//	/// <remarks>
//	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter">Learn more about this API from docs.microsoft.com</see>.</para>
//	/// </remarks>
//	//public void CreateSoftwareAdapter( HINSTANCE Module, out DXSharp.DXGI.IAdapter ppAdapter ) { }
//}