#region Using Directives

using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.InteropServices ;
using System.Runtime.CompilerServices ;
using System.Runtime.Versioning ;
using Windows.Win32 ;
using Windows.UI.Core ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows ;
using DXSharp.Direct3D12 ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
using HResult = DXSharp.Windows.HResult ;

#endregion
namespace DXSharp.DXGI ;

// -------------------------------------------------------------------------------------
// Interface Version: IDXGIFactory ::
// -------------------------------------------------------------------------------------

/// <summary>
/// An IFactory is a wrapper of the native DirectX COM interface
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgifactory">IDXGIFactory</a>. 
/// The interface implements methods for generating DXGI objects (which also handle full screen transitions).
/// </summary>
[SupportedOSPlatform( "windows5.0" )]
[ProxyFor(typeof(IDXGIFactory))]
public interface IFactory: IObject,
						   IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	[SuppressMessage( "Interoperability", 
					  "CA1416:Validate platform compatibility" )] 
	internal static readonly ReadOnlyDictionary< Guid, Func<IDXGIFactory, IInstantiable> > _factoryCreationFunctions =
		new( new Dictionary< Guid, Func<IDXGIFactory, IInstantiable> > {
			{ IFactory.IID, ( pComObj ) => new Factory( pComObj ) },
			{ IFactory1.IID, ( pComObj ) => new Factory1( (pComObj as IDXGIFactory1)! ) },
			{ IFactory2.IID, ( pComObj ) => new Factory2( (pComObj as IDXGIFactory2)! ) },
			{ IFactory3.IID, ( pComObj ) => new Factory3( (pComObj as IDXGIFactory3)! ) },
			{ IFactory4.IID, ( pComObj ) => new Factory4( (pComObj as IDXGIFactory4)! ) },
			{ IFactory5.IID, ( pComObj ) => new Factory5( (pComObj as IDXGIFactory5)! ) },
			{ IFactory6.IID, ( pComObj ) => new Factory6( (pComObj as IDXGIFactory6)! ) },
			{ IFactory7.IID, ( pComObj ) => new Factory7( (pComObj as IDXGIFactory7)! ) },
		} ) ;
	// ---------------------------------------------------------------------------------

	// -----------------------------------------------------------------------------------------------
	public const int MAX_ADAPTER_COUNT = 0x0F ;
	// -----------------------------------------------------------------------------------------------
	
	/// <summary>Enumerates the adapters (video cards).</summary>
	/// <param name="index">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The index of the adapter to enumerate.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppAdapter">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>**</b> The address of a pointer to an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> interface at the position specified by the <i>Adapter</i> parameter.  This parameter must not be <b>NULL</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_NOT_FOUND</a> if the index is greater than or equal to the number of adapters in the local system, or <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> if <i>ppAdapter</i> parameter is <b>NULL</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-enumadapters">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult EnumAdapters( uint index, out IAdapter? ppAdapter ) ;
	
	
	/// <summary>Allows DXGI to monitor an application's message queue for the alt-enter key sequence (which causes the application to switch from windowed to full screen or vice versa).</summary>
	/// <param name="WindowHandle">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a></b> The handle of the window that is to be monitored. This parameter can be <b>NULL</b>; but only if *Flags* is also 0.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-makewindowassociation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Flags">Type: <b><a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgifactory-makewindowassociation">UINT</a></b></param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> if <i>WindowHandle</i> is invalid, or E_OUTOFMEMORY.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-makewindowassociation">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void MakeWindowAssociation( in HWnd WindowHandle, WindowAssociation Flags ) ;

	/// <summary>Get the window through which the user controls the transition to and from full screen.</summary>
	/// <param name="pWindowHandle">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a>*</b> A pointer to a window handle.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-getwindowassociation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns a code that indicates success or failure. <b>S_OK</b> indicates success, <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> indicates <i>pWindowHandle</i> was passed in as <b>NULL</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-getwindowassociation">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetWindowAssociation( out HWnd pWindowHandle ) ;

	
	/// <summary>Creates a swap chain.</summary>
	/// <param name="pCmdQueue">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> For Direct3D 11, and earlier versions of Direct3D, this is a pointer to the Direct3D device for the swap chain. For Direct3D 12 this is a pointer to a direct command queue (refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a>) . This parameter cannot be <b>NULL</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="desc">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_swap_chain_desc">SwapChainDescription</a>*</b> A pointer to a  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_swap_chain_desc">SwapChainDescription</a> structure for the swap-chain description. This parameter cannot be <b>NULL</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppSwapChain">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiswapchain">IDXGISwapChain</a>**</b> A pointer to a variable that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiswapchain">IDXGISwapChain</a> interface for the swap chain that <b>CreateSwapChain</b> creates.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b></para>
	/// <para><a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a>  if <i>pDesc</i> or <i>ppSwapChain</i> is <b>NULL</b>, DXGI_STATUS_OCCLUDED if you request full-screen mode and it is unavailable, or E_OUTOFMEMORY. Other error codes defined by the type of device passed in may also be returned.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createswapchain">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult CreateSwapChain( in  ICommandQueue?       pCmdQueue,
							 in  SwapChainDescription desc,
							 out ISwapChain?          ppSwapChain ) ;

	
	/// <summary>Create an adapter interface that represents a software adapter.</summary>
	/// <param name="Module">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HMODULE</a></b> Handle to the software adapter's dll. HMODULE can be obtained with <a href="https://docs.microsoft.com/windows/desktop/api/libloaderapi/nf-libloaderapi-getmodulehandlea">GetModuleHandle</a> or <a href="https://docs.microsoft.com/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibrarya">LoadLibrary</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppAdapter">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>**</b> Address of a pointer to an adapter (see <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> A <a href="/windows/desktop/direct3ddxgi/dxgi-error">return code</a> indicating success or failure.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory-createsoftwareadapter">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateSoftwareAdapter( HInstance Module, out IAdapter? ppAdapter ) ;
	
	// -----------------------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGIFactory ) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIFactory).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Factory( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Factory( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Factory(( pComObj as IDXGIFactory )!) ;
	// -----------------------------------------------------------------------------------------------
	
	internal static virtual TFactory Create< TFactory >( ) where TFactory: IFactory, IInstantiable {
		var factory = DXGIFunctions.CreateDXGIFactory< IDXGIFactory >( out var hr ) ;
		
		return ( (TFactory)TFactory.Instantiate(factory) ) 
			   ?? throw new DirectXComError( hr, $"{nameof(TFactory)} -> Create :: " +
												 $"Failed to create DXGI Factory interface with " +
												 $"GUID: {TFactory.Guid}! HRESULT: {hr.Value}",
											 new($"{nameof(PInvoke.CreateDXGIFactory)} :: " +
												 $"Failed to create DXGI Factory interface! HRESULT: {hr.Value}") ) ;
	}
	
	// =====================================================================================
} ;


// -------------------------------------------------------------------------------------
// Interface Version: IDXGIFactory1 ::
// -------------------------------------------------------------------------------------

[SupportedOSPlatform( "windows6.1" )]
[ProxyFor(typeof(IDXGIFactory1))]
public interface IFactory1: IFactory {
	
	/// <summary>Enumerates both adapters (video cards) with or without outputs.</summary>
	/// <param name="index">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The index of the adapter to enumerate.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory1-enumadapters1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppAdapter">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter1">IDXGIAdapter1</a>**</b> The address of a pointer to an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter1">IDXGIAdapter1</a> interface at the position specified by the <i>Adapter</i> parameter.  This parameter must not be <b>NULL</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory1-enumadapters1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_NOT_FOUND</a> if the index is greater than or equal to the number of adapters in the local system, or <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a> if <i>ppAdapter</i> parameter is <b>NULL</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgifactory1-enumadapters1">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult EnumAdapters1( uint index, out IAdapter1? ppAdapter ) ;
	
	
	/// <summary>Informs an application of the possible need to re-enumerate adapters.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">BOOL</a></b> <b>FALSE</b>, if a new adapter is becoming available or the current adapter is going away. <b>TRUE</b>, no adapter changes. <b>IsCurrent</b> returns <b>FALSE</b> to inform the calling application to re-enumerate adapters.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method is not supported by DXGI 1.0, which shipped in WindowsVista and Windows Server2008. DXGI 1.1 support is required, which is available on Windows7, Windows Server2008R2, and as an update to WindowsVista with Service Pack2 (SP2) (<a href="https://support.microsoft.com/topic/application-compatibility-update-for-windows-vista-windows-server-2008-windows-7-and-windows-server-2008-r2-february-2010-3eb7848b-9a76-85fe-98d0-729e3827ea60">KB 971644</a>) and Windows Server2008 (<a href="https://support.microsoft.com/kb/971512/">KB 971512</a>).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgifactory1-iscurrent#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	bool IsCurrent( ) ;
	
	// -----------------------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGIFactory1 ) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIFactory1).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new Factory1( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr pComObj ) => new Factory1( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Factory1( ( pComObj as IDXGIFactory1 )! ) ;
	// -----------------------------------------------------------------------------------------------
	
	
	public static TFactory Create1< TFactory >( ) where TFactory: IFactory1, IInstantiable {
		var factory = DXGIFunctions.CreateDXGIFactory1< IDXGIFactory1 >( out var hr ) ;
		
		return ( (TFactory)TFactory.Instantiate(factory) ) 
			   ?? throw new DirectXComError( hr, $"{nameof(TFactory)} -> Create :: " +
												 $"Failed to create DXGI Factory interface with " +
												 $"GUID: {TFactory.Guid}! HRESULT: {hr.Value}",
											 new($"{nameof(PInvoke.CreateDXGIFactory1)} :: " +
												 $"Failed to create DXGI Factory interface! HRESULT: {hr.Value}") ) ;
	}
	
	// -----------------------------------------------------------------------------------------------
} ;


// -------------------------------------------------------------------------------------
// Interface Version: IDXGIFactory2 ::
// -------------------------------------------------------------------------------------

[SupportedOSPlatform( "windows8.0" )]
[ProxyFor(typeof(IDXGIFactory2))]
public interface IFactory2: IFactory1 {
	// ----------------------------------------------------------------------------------------------------

	/// <summary>Determines whether to use stereo mode.</summary>
	/// <returns>
	/// <para>Indicates whether to use stereo mode. <b>TRUE</b> indicates that you can use stereo mode; otherwise, <b>FALSE</b>. <b>Platform Update for Windows7:</b>On Windows7 or Windows Server2008R2 with the <a href="https://support.microsoft.com/help/2670838">Platform Update for Windows7</a> installed, <b>IsWindowedStereoEnabled</b> always returns FALSE because stereoscopic 3D display behavior isn’t available with the Platform Update for Windows7. For more info about the Platform Update for Windows7, see <a href="https://docs.microsoft.com/windows/desktop/direct3darticles/platform-update-for-windows-7">Platform Update for Windows 7</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>We recommend that windowed applications call <b>IsWindowedStereoEnabled</b> before they attempt to use stereo. <b>IsWindowedStereoEnabled</b> returns <b>TRUE</b> if both of the following items are true: </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-iswindowedstereoenabled#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	bool IsWindowedStereoEnabled( ) ;


	/// <summary>Creates a swap chain that is associated with an HWND handle to the output window for the swap chain.</summary>
	/// <param name="pCommandQueue">For Direct3D 11, and earlier versions of Direct3D, this is a pointer to the Direct3D device for the swap chain. For Direct3D 12 this is a pointer to a direct command queue (refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a>). This parameter cannot be <b>NULL</b>.</param>
	/// <param name="hWnd">The <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a> handle that is associated with the swap chain that <b>CreateSwapChainForHwnd</b> creates. This parameter cannot be <b>NULL</b>.</param>
	/// <param name="pDesc">A pointer to a  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">SwapChainDescription1</a> structure for the swap-chain description. This parameter cannot be <b>NULL</b>.</param>
	/// <param name="pFullscreenDesc">A pointer to a  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_fullscreen_desc">SwapChainFullscreenDescription</a> structure for the description of a full-screen swap chain. You can optionally set this parameter to create a full-screen swap chain. Set it to <b>NULL</b> to create a windowed swap chain.</param>
	/// <param name="pRestrictToOutput">
	///     <para>A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgioutput">IDXGIOutput</a> interface for the output to restrict content to. You must also pass the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-present">DXGI_PRESENT_RESTRICT_TO_OUTPUT</a> flag in a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a> call to force the content to appear blacked out on any other output. If you want to restrict the content to a different output, you must create a new swap chain. However, you can conditionally restrict content based on the <b>DXGI_PRESENT_RESTRICT_TO_OUTPUT</b> flag.</para>
	///     <para>Set this parameter to <b>NULL</b> if you don't want to restrict content to an output target.</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforhwnd#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppSwapChain">A pointer to a variable that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nn-dxgi1_2-idxgiswapchain1">IDXGISwapChain1</a> interface for the swap chain that <b>CreateSwapChainForHwnd</b> creates.</param>
	/// <returns>
	/// <para><b>CreateSwapChainForHwnd</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>Do not use this method in Windows Store apps. Instead, use <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcorewindow">IDXGIFactory2::CreateSwapChainForCoreWindow</a>.</div> <div></div> If you specify the width, height, or both (<b>Width</b> and <b>Height</b> members of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">SwapChainDescription1</a> that <i>pDesc</i> points to) of the swap chain as zero, the runtime obtains the size from the output window that the <i>hWnd</i> parameter specifies. You can subsequently call the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-getdesc1">IDXGISwapChain1::GetDesc1</a> method to retrieve the assigned width or height value. Because you can associate only one flip presentation model swap chain at a time with an <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a>, the Microsoft Direct3D11 policy of deferring the destruction of objects can cause problems if you attempt to destroy a flip presentation model swap chain and replace it with another swap chain. For more info about this situation, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11devicecontext-flush">Deferred Destruction Issues with Flip Presentation Swap Chains</a>. For info about how to choose a format for the swap chain's back buffer, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/converting-data-color-space">Converting data for the color space</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforhwnd#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateSwapChainForHwnd( ICommandQueue pCommandQueue, HWnd hWnd,
								 in            SwapChainDescription1           pDesc,
								 [Optional] in SwapChainFullscreenDescription? pFullscreenDesc,
								 [Optional]    IOutput?                        pRestrictToOutput,
								 out           ISwapChain1                     ppSwapChain ) ;
	
	
	/// <summary>Creates a swap chain that is associated with the CoreWindow object for the output window for the swap chain.</summary>
	/// <param name="pCmdQueue">For Direct3D 11, and earlier versions of Direct3D, this is a pointer to the Direct3D device for the swap chain. For Direct3D 12 this is a pointer to a direct command queue (refer to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a>). This parameter cannot be <b>NULL</b>.</param>
	/// <param name="pWindow">A pointer to the <a href="https://docs.microsoft.com/uwp/api/Windows.UI.Core.CoreWindow">CoreWindow</a> object that is associated with the swap chain that <b>CreateSwapChainForCoreWindow</b> creates.</param>
	/// <param name="pDesc">A pointer to a  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">SwapChainDescription1</a> structure for the swap-chain description. This parameter cannot be <b>NULL</b>.</param>
	/// <param name="pRestrictToOutput">A pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgioutput">IDXGIOutput</a> interface that the swap chain is restricted to. If the swap chain is moved to a different output, the content is black. You can optionally set this parameter to an output target that uses <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-present">DXGI_PRESENT_RESTRICT_TO_OUTPUT</a> to restrict the content on this output. If you do not set this parameter to restrict content on an output target, you can set it to <b>NULL</b>.</param>
	/// <param name="ppSwapChain">A pointer to a variable that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nn-dxgi1_2-idxgiswapchain1">IDXGISwapChain1</a> interface for the swap chain that <b>CreateSwapChainForCoreWindow</b> creates.</param>
	/// <returns>
	/// <para><b>CreateSwapChainForCoreWindow</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>Use this method in Windows Store apps rather than <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforhwnd">IDXGIFactory2::CreateSwapChainForHwnd</a>.</div> <div></div> If you specify the width, height, or both (<b>Width</b> and <b>Height</b> members of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">SwapChainDescription1</a> that <i>pDesc</i> points to) of the swap chain as zero, the runtime obtains the size from the output window that the <i>pWindow</i> parameter specifies. You can subsequently call the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-getdesc1">IDXGISwapChain1::GetDesc1</a> method to retrieve the assigned width or height value. Because you can associate only one flip presentation model swap chain (per layer) at a time with a <a href="https://docs.microsoft.com/uwp/api/Windows.UI.Core.CoreWindow">CoreWindow</a>, the Microsoft Direct3D11 policy of deferring the destruction of objects can cause problems if you attempt to destroy a flip presentation model swap chain and replace it with another swap chain. For more info about this situation, see <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11devicecontext-flush">Deferred Destruction Issues with Flip Presentation Swap Chains</a>. For info about how to choose a format for the swap chain's back buffer, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/converting-data-color-space">Converting data for the color space</a>. <h3><a id="Overlapping_swap_chains"></a><a id="overlapping_swap_chains"></a><a id="OVERLAPPING_SWAP_CHAINS"></a>Overlapping swap chains</h3> Starting with Windows8.1, it is possible to create an additional swap chain in the foreground layer. A foreground swap chain can be used to render UI elements at native resolution while scaling up real-time rendering in the background swap chain (such as gameplay). This enables scenarios where lower resolution rendering is required for faster fill rates, but without sacrificing UI quality. Foreground swap chains are created by setting the <b>DXGI_SWAP_CHAIN_FLAG_FOREGROUND_LAYER</b> swap chain flag in the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">SwapChainDescription1</a> that <i>pDesc</i> points to. Foreground swap chains must also use the <b>DXGI_ALPHA_MODE_PREMULTIPLIED</b> alpha mode, and must use <b>DXGI_SCALING_NONE</b>. Premultiplied alpha means that each pixel's color values are expected to be already multiplied by the alpha value before the frame is presented. For example, a 100% white BGRA pixel at 50% alpha is set to (0.5, 0.5, 0.5, 0.5). The alpha premultiplication step can be done in the output-merger stage by applying an app blend state (see <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nn-d3d11-id3d11blendstate">ID3D11BlendState</a>) with the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ns-d3d11-d3d11_render_target_blend_desc">D3D11_RENDER_TARGET_BLEND_DESC</a> structure's <b>SrcBlend</b> field set to <b>D3D11_SRC_ALPHA</b>. If the alpha premultiplication step is not done, colors on the foreground swap chain will be brighter than expected. The foreground swap chain will use multiplane overlays if supported by the hardware. Call <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_3/nf-dxgi1_3-idxgioutput2-supportsoverlays">IDXGIOutput2::SupportsOverlays</a> to query the adapter for overlay support. The following example creates a foreground swap chain for a CoreWindow:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcorewindow#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateSwapChainForCoreWindow( ICommandQueue            pCmdQueue,
									   CoreWindow               pWindow,
									   in SwapChainDescription1 pDesc,
									   IOutput                  pRestrictToOutput,
									   out ISwapChain1          ppSwapChain ) ;

	
	/// <summary>Identifies the adapter on which a shared resource object was created.</summary>
	/// <param name="hResource">A handle to a shared resource object. The <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiresource1-createsharedhandle">IDXGIResource1::CreateSharedHandle</a> method returns this handle.</param>
	/// <param name="pLuid">A pointer to a variable that receives a locally unique identifier (<a href="https://docs.microsoft.com/previous-versions/windows/hardware/drivers/ff549708(v=vs.85)">LUID</a>) value that identifies the adapter. <b>LUID</b> is defined in Dxgi.h. An <b>LUID</b> is a 64-bit value that is guaranteed to be unique only on the operating system on which it was generated. The uniqueness of an <b>LUID</b> is guaranteed only until the operating system is restarted.</param>
	/// <returns>
	/// <para><b>GetSharedResourceAdapterLuid</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>You cannot share resources across adapters. Therefore, you cannot open a shared resource on an adapter other than the adapter on which the resource was created.  Call <b>GetSharedResourceAdapterLuid</b> before you open a shared resource to ensure that the resource was created on the appropriate adapter. To open a shared resource, call the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11_1/nf-d3d11_1-id3d11device1-opensharedresource1">ID3D11Device1::OpenSharedResource1</a> or <a href="https://docs.microsoft.com/windows/desktop/api/d3d11_1/nf-d3d11_1-id3d11device1-opensharedresourcebyname">ID3D11Device1::OpenSharedResourceByName</a> method.</remarks>
	void GetSharedResourceAdapterLuid( Win32Handle hResource, out Luid pLuid ) ;

	
	/// <summary>Registers an application window to receive notification messages of changes of stereo status.</summary>
	/// <param name="WindowHandle">The handle of the window to send a notification message to when stereo status change occurs.</param>
	/// <param name="wMsg">Identifies the notification message to send.</param>
	/// <param name="pdwCookie">A pointer to a key value that an application can pass to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-unregisterstereostatus">IDXGIFactory2::UnregisterStereoStatus</a> method  to unregister the notification message that <i>wMsg</i> specifies.</param>
	/// <returns>
	/// <para><b>RegisterStereoStatusWindow</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-registerstereostatuswindow">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void RegisterStereoStatusWindow( HWnd WindowHandle, uint wMsg, out uint pdwCookie ) ;

	
	/// <summary>Registers to receive notification of changes in stereo status by using event signaling.</summary>
	/// <param name="hEvent">A handle to the event object that the operating system sets when notification of stereo status change occurs. The <a href="https://docs.microsoft.com/windows/desktop/api/synchapi/nf-synchapi-createeventa">CreateEvent</a> or <a href="https://docs.microsoft.com/windows/desktop/api/synchapi/nf-synchapi-openeventa">OpenEvent</a> function returns this handle.</param>
	/// <param name="pdwCookie">A pointer to a key value that an application can pass to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-unregisterstereostatus">IDXGIFactory2::UnregisterStereoStatus</a> method  to unregister the notification event that <i>hEvent</i> specifies.</param>
	/// <returns>
	/// <para><b>RegisterStereoStatusEvent</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-registerstereostatusevent">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void RegisterStereoStatusEvent( Win32Handle hEvent, out uint pdwCookie ) ;

	
	/// <summary>Unregisters a window or an event to stop it from receiving notification when stereo status changes.</summary>
	/// <param name="dwCookie">A key value for the window or event to unregister. The  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-registerstereostatuswindow">IDXGIFactory2::RegisterStereoStatusWindow</a> or  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-registerstereostatusevent">IDXGIFactory2::RegisterStereoStatusEvent</a> method returns this value.</param>
	/// <remarks><b>Platform Update for Windows7:</b>On Windows7 or Windows Server2008R2 with the <a href="https://support.microsoft.com/help/2670838">Platform Update for Windows7</a> installed, <b>UnregisterStereoStatus</b> has no effect. For more info about the Platform Update for Windows7, see <a href="https://docs.microsoft.com/windows/desktop/direct3darticles/platform-update-for-windows-7">Platform Update for Windows 7</a>.</remarks>
	void UnregisterStereoStatus( uint dwCookie ) ;

	
	/// <summary>Registers an application window to receive notification messages of changes of occlusion status.</summary>
	/// <param name="WindowHandle">The handle of the window to send a notification message to when occlusion status change occurs.</param>
	/// <param name="wMsg">Identifies the notification message to send.</param>
	/// <param name="pdwCookie">A pointer to a key value that an application can pass to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-unregisterocclusionstatus">IDXGIFactory2::UnregisterOcclusionStatus</a> method  to unregister the notification message that <i>wMsg</i> specifies.</param>
	/// <returns>
	/// <para><b>RegisterOcclusionStatusWindow</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>Apps choose the Windows message that Windows sends when occlusion status changes.</remarks>
	void RegisterOcclusionStatusWindow( HWnd WindowHandle, uint wMsg, out uint pdwCookie ) ;

	
	/// <summary>Registers to receive notification of changes in occlusion status by using event signaling.</summary>
	/// <param name="hEvent">A handle to the event object that the operating system sets when notification of occlusion status change occurs. The <a href="https://docs.microsoft.com/windows/desktop/api/synchapi/nf-synchapi-createeventa">CreateEvent</a> or <a href="https://docs.microsoft.com/windows/desktop/api/synchapi/nf-synchapi-openeventa">OpenEvent</a> function returns this handle.</param>
	/// <param name="pdwCookie">A pointer to a key value that an application can pass to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-unregisterocclusionstatus">IDXGIFactory2::UnregisterOcclusionStatus</a> method  to unregister the notification event that <i>hEvent</i> specifies.</param>
	/// <returns>
	/// <para><b>RegisterOcclusionStatusEvent</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>If you call <b>RegisterOcclusionStatusEvent</b> multiple times with the same event handle, <b>RegisterOcclusionStatusEvent</b> fails with <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_INVALID_CALL</a>. If you call <b>RegisterOcclusionStatusEvent</b> multiple times with the different event handles, <b>RegisterOcclusionStatusEvent</b> properly registers the events.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-registerocclusionstatusevent#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void RegisterOcclusionStatusEvent( Win32Handle hEvent, out uint pdwCookie ) ;

	
	/// <summary>Unregisters a window or an event to stop it from receiving notification when occlusion status changes.</summary>
	/// <param name="dwCookie">A key value for the window or event to unregister. The  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-registerocclusionstatuswindow">IDXGIFactory2::RegisterOcclusionStatusWindow</a> or  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-registerocclusionstatusevent">IDXGIFactory2::RegisterOcclusionStatusEvent</a> method returns this value.</param>
	/// <remarks><b>Platform Update for Windows7:</b>On Windows7 or Windows Server2008R2 with the <a href="https://support.microsoft.com/help/2670838">Platform Update for Windows7</a> installed, <b>UnregisterOcclusionStatus</b> has no effect. For more info about the Platform Update for Windows7, see <a href="https://docs.microsoft.com/windows/desktop/direct3darticles/platform-update-for-windows-7">Platform Update for Windows 7</a>.</remarks>
	void UnregisterOcclusionStatus( uint dwCookie ) ;

	
	/// <summary>Creates a swap chain that you can use to send Direct3D content into the DirectComposition API or a Xaml framework to compose in a window.</summary>
	/// <param name="pDevice">For Direct3D 11, and earlier versions of Direct3D, this is a pointer to the Direct3D device for the swap chain. For Direct3D 12 this is a pointer to a direct command queue (refer to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a>). This parameter cannot be <b>NULL</b>. Software drivers, like <a href="https://docs.microsoft.com/windows/win32/api/d3dcommon/ne-d3dcommon-d3d_driver_type">D3D_DRIVER_TYPE_REFERENCE</a>, are not supported for composition swap chains.</param>
	/// <param name="pDesc">
	/// <para>A pointer to a  <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">SwapChainDescription1</a> structure for the swap-chain description. This parameter cannot be <b>NULL</b>. You must specify the <a href="https://docs.microsoft.com/windows/win32/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL</a> value in the <b>SwapEffect</b> member of <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">SwapChainDescription1</a> because <b>CreateSwapChainForComposition</b> supports only <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/dxgi-flip-model">flip presentation model</a>. You must also specify the <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ne-dxgi1_2-dxgi_scaling">DXGI_SCALING_STRETCH</a> value in the <b>Scaling</b> member of <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">SwapChainDescription1</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pRestrictToOutput">
	/// <para>A pointer to the <a href="https://docs.microsoft.com/windows/win32/api/dxgi/nn-dxgi-idxgioutput">IDXGIOutput</a> interface for the output to restrict content to. You must also pass the <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/dxgi-present">DXGI_PRESENT_RESTRICT_TO_OUTPUT</a> flag in a <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a> call to force the content to appear blacked out on any other output. If you want to restrict the content to a different output, you must create a new swap chain. However, you can conditionally restrict content based on the <b>DXGI_PRESENT_RESTRICT_TO_OUTPUT</b> flag. Set this parameter to <b>NULL</b> if you don't want to restrict content to an output target.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppSwapChain">A pointer to a variable that receives a pointer to the <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nn-dxgi1_2-idxgiswapchain1">IDXGISwapChain1</a> interface for the swap chain that <b>CreateSwapChainForComposition</b> creates.</param>
	/// <returns>
	/// <para><b>CreateSwapChainForComposition</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>You can use composition swap chains with either: * <a href="https://docs.microsoft.com/windows/win32/directcomp/directcomposition-portal">DirectComposition</a>'s <a href="https://docs.microsoft.com/windows/win32/api/dcomp/nn-dcomp-idcompositionvisual">IDCompositionVisual</a> interface, * System XAML's [SwapChainPanel](/uwp/api/windows.ui.xaml.controls.swapchainpanel) or [SwapChainBackgroundPanel](/uwp/api/windows.ui.xaml.controls.swapchainbackgroundpanel) classes. * [Windows UI Library (WinUI) 3](https://docs.microsoft.com/windows/apps/winui/) XAML's [SwapChainPanel](/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.swapchainpanel) or [SwapChainBackgroundPanel](/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.swapchainbackgroundpanel) classes. For DirectComposition, you can call the <a href="https://docs.microsoft.com/windows/win32/api/dcomp/nf-dcomp-idcompositionvisual-setcontent">IDCompositionVisual::SetContent</a> method to set the swap chain as the content of a <a href="https://docs.microsoft.com/windows/win32/directcomp/basic-concepts">visual object</a>, which then allows you to bind the swap chain to the visual tree. For XAML, the <b>SwapChainBackgroundPanel</b> class exposes a classic COM interface <b>ISwapChainBackgroundPanelNative</b>. You can use the <a href="https://docs.microsoft.com/windows/win32/api/windows.ui.xaml.media.dxinterop/nf-windows-ui-xaml-media-dxinterop-iswapchainbackgroundpanelnative-setswapchain">ISwapChainBackgroundPanelNative::SetSwapChain</a> method to bind to the XAML UI graph. For info about how to use composition swap chains with XAML’s <b>SwapChainBackgroundPanel</b> class, see <a href="https://docs.microsoft.com/windows/uwp/gaming/directx-and-xaml-interop">DirectX and XAML interop</a>. The <a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-setfullscreenstate">IDXGISwapChain::SetFullscreenState</a>, <a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-resizetarget">IDXGISwapChain::ResizeTarget</a>, <a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-getcontainingoutput">IDXGISwapChain::GetContainingOutput</a>, <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-gethwnd">IDXGISwapChain1::GetHwnd</a>, and <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-getcorewindow">IDXGISwapChain::GetCoreWindow</a> methods aren't valid on this type of swap chain. If you call any of these methods on this type of swap chain, they fail. For info about how to choose a format for the swap chain's back buffer, see <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/converting-data-color-space">Converting data for the color space</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CreateSwapChainForComposition( ICommandQueue pDevice,
												  in SwapChainDescription1 pDesc,
												  IOutput pRestrictToOutput,
												  out ISwapChain1 ppSwapChain ) ;

	// ----------------------------------------------------------------------------------------------------
	new static Type ComType => typeof(IDXGIFactory2) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIFactory2).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Factory2( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Factory2( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Factory2( (pComObj as IDXGIFactory2)! ) ;
	// ====================================================================================================
} ;


// -------------------------------------------------------------------------------------
// Interface Version: IDXGIFactory3 ::
// -------------------------------------------------------------------------------------

[SupportedOSPlatform( "windows8.1" )]
[ProxyFor( typeof( IDXGIFactory3 ) )]
public interface IFactory3: IFactory2 {
	/// <summary>Gets the flags that were used when a Microsoft DirectX Graphics Infrastructure (DXGI) object was created.</summary>
	/// <returns>The creation flags.</returns>
	/// <remarks>The <b>GetCreationFlags</b> method returns flags that were passed to the  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_3/nf-dxgi1_3-createdxgifactory2">CreateDXGIFactory2</a> function, or were implicitly constructed by <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-createdxgifactory">CreateDXGIFactory</a>, <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-createdxgifactory1">CreateDXGIFactory1</a>,  <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-d3d11createdevice">D3D11CreateDevice</a>, or <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-d3d11createdeviceandswapchain">D3D11CreateDeviceAndSwapChain</a>.</remarks>
	FactoryCreateFlags GetCreationFlags( ) ;
	
	new static Type ComType => typeof(IDXGIFactory3) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIFactory3).GUID
															 .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Factory3( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Factory3( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Factory3( (pComObj as IDXGIFactory3)! ) ;
	
	/*public static void Create< T >( out T ppFactory ) where T: IFactory2, IInstantiable {
		var hr = DXGIFunctions.CreateDXGIFactory2< T >( FactoryCreateFlags.DEBUG, IFactory7.IID, out var factory ) ;
	}*/
} ;


 
// -------------------------------------------------------------------------------------
// Interface Version: IDXGIFactory4 ::
// -------------------------------------------------------------------------------------

[ProxyFor( typeof( IDXGIFactory4 ) )]
public interface IFactory4: IFactory3 {
	
	/// <summary>Outputs the IDXGIAdapter for the specified LUID.</summary>
	/// <param name="AdapterLuid">
	/// <para>Type: <b><a href="https://docs.microsoft.com/previous-versions/windows/hardware/drivers/ff549708(v=vs.85)">LUID</a></b> A unique value that identifies the adapter. See <a href="https://docs.microsoft.com/previous-versions/windows/hardware/drivers/ff549708(v=vs.85)">LUID</a> for a definition of the structure. <b>LUID</b> is defined in dxgi.h.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgifactory4-enumadapterbyluid#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (GUID) of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> object referenced by the <i>ppvAdapter</i> parameter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgifactory4-enumadapterbyluid#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvAdapter">
	/// <para>Type: <b>void**</b> The address of an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> interface pointer to the adapter. This parameter must not be NULL.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgifactory4-enumadapterbyluid#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>. See also Direct3D 12 Return Codes.</para>
	/// </returns>
	/// <remarks>
	/// <para>For Direct3D 12, it's no longer possible to backtrack from a device to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> that was used to create it. <b>IDXGIFactory4::EnumAdapterByLuid</b> enables an app to retrieve information about the adapter where a D3D12 device was created. <b>IDXGIFactory4::EnumAdapterByLuid</b> is designed to be paired with <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-getadapterluid">ID3D12Device::GetAdapterLuid</a>. For more information, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-1-4-improvements">DXGI 1.4 Improvements</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgifactory4-enumadapterbyluid#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void EnumAdapterByLuid< A >( Luid AdapterLuid, in Guid riid, out A ppvAdapter ) where A: IAdapter ;
	
	/// <summary>Provides an adapter which can be provided to D3D12CreateDevice to use the WARP renderer.</summary>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (GUID) of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> object referenced by the <i>ppvAdapter</i> parameter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgifactory4-enumwarpadapter#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvAdapter">
	/// <para>Type: <b>void**</b> The address of an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> interface pointer to the adapter. This parameter must not be NULL.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgifactory4-enumwarpadapter#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>. See also Direct3D 12 Return Codes.</para>
	/// </returns>
	/// <remarks>For more information, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-1-4-improvements">DXGI 1.4 Improvements</a>.</remarks>
	void EnumWarpAdapter< A >( in Guid riid, out A ppvAdapter ) where A: IAdapter ;
	
	
	new static Type ComType => typeof(IDXGIFactory4) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIFactory4).GUID
															 .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Factory4( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Factory4( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Factory4( (pComObj as IDXGIFactory4)! ) ;
} ;


// -------------------------------------------------------------------------------------
// Interface Version: IDXGIFactory5 ::
// -------------------------------------------------------------------------------------

[ProxyFor( typeof( IDXGIFactory5 ) )]
public interface IFactory5: IFactory4 {
	
	/// <summary>Used to check for hardware feature support.</summary>
	/// <param name="Feature">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ne-dxgi1_5-dxgi_feature">DXGI_FEATURE</a></b> Specifies one member of  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ne-dxgi1_5-dxgi_feature">DXGI_FEATURE</a> to query support for.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgifactory5-checkfeaturesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pFeatureSupportData">
	/// <para>Type: <b>void*</b> Specifies a pointer to a buffer that will be filled with data that describes the feature support.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgifactory5-checkfeaturesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="FeatureSupportDataSize">
	/// <para>Type: <b>UINT</b> The size, in bytes, of <i>pFeatureSupportData</i>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgifactory5-checkfeaturesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code.</para>
	/// </returns>
	/// <remarks>Refer to the description of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING</a>.</remarks>
	void CheckFeatureSupport( Feature Feature, nint pFeatureSupportData, uint FeatureSupportDataSize ) ;
	
	new static Type ComType => typeof(IDXGIFactory5) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIFactory5).GUID
															 .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Factory5( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Factory5( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Factory5( (pComObj as IDXGIFactory5)! ) ;
} ;


// -------------------------------------------------------------------------------------
// Interface Version: IDXGIFactory6 ::
// -------------------------------------------------------------------------------------


[SupportedOSPlatform("windows10.0.17134")]
[ProxyFor( typeof( IDXGIFactory6 ) )]
public interface IFactory6: IFactory5 {
	
	/// <summary>Enumerates graphics adapters based on a given GPU preference.</summary>
			/// <param name="Adapter">
			/// <para>Type: <b>UINT</b> The index of the adapter to enumerate. The indices are in order of the preference specified in <i>GpuPreference</i>—for example, if <b>DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE</b> is specified, then the highest-performing adapter is at index 0, the second-highest is at index 1, and so on.</para>
			/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgifactory6-enumadapterbygpupreference#parameters">Read more on docs.microsoft.com</see>.</para>
			/// </param>
			/// <param name="GpuPreference">
			/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ne-dxgi1_6-dxgi_gpu_preference">DXGI_GPU_PREFERENCE</a></b> The GPU preference for the app.</para>
			/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgifactory6-enumadapterbygpupreference#parameters">Read more on docs.microsoft.com</see>.</para>
			/// </param>
			/// <param name="riid">
			/// <para>Type: <b>REFIID</b> The globally unique identifier (GUID) of the <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nn-dxgi1_6-idxgifactory6">IDXGIAdapter</a> object referenced by the <i>ppvAdapter</i> parameter.</para>
			/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgifactory6-enumadapterbygpupreference#parameters">Read more on docs.microsoft.com</see>.</para>
			/// </param>
			/// <param name="ppvAdapter">
			/// <para>Type: <b>void**</b> The address of an <a href="https://docs.microsoft.com/windows/win32/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> interface pointer to the adapter. This parameter must not be NULL.</para>
			/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgifactory6-enumadapterbygpupreference#parameters">Read more on docs.microsoft.com</see>.</para>
			/// </param>
			/// <returns>
			/// <para>Type: <b>HRESULT</b> Returns <b>S_OK</b> if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
			/// </returns>
			/// <remarks>
			/// <para>This method allows developers to select which GPU they think is most appropriate for each device their app creates and utilizes. This method is similar to <a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgifactory1-enumadapters1">IDXGIFactory1::EnumAdapters1</a>, but it accepts a GPU preference to reorder the adapter enumeration. It returns the appropriate <b>IDXGIAdapter</b> for the given GPU preference. It is meant to be used in conjunction with the <b>D3D*CreateDevice</b> functions, which take in an <b>IDXGIAdapter*</b>. When <b>DXGI_GPU_PREFERENCE_UNSPECIFIED</b> is specified for the <i>GpuPreference</i> parameter, this method is equivalent to calling <a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgifactory1-enumadapters1">IDXGIFactory1::EnumAdapters1</a>. When <b>DXGI_GPU_PREFERENCE_MINIMUM_POWER</b> is specified for the <i>GpuPreference</i> parameter, the order of preference for the adapter returned in <i>ppvAdapter</i> will be:<dl> <dd>1. iGPUs (integrated GPUs)</dd> <dd>2. dGPUs (discrete GPUs)</dd> <dd>3. xGPUs (external GPUs)</dd> </dl></para>
			/// <para>When <b>DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE</b> is specified for the <i>GpuPreference</i> parameter, the order of preference for the adapter returned in <i>ppvAdapter</i> will be:<dl> <dd>1. xGPUs</dd> <dd>2. dGPUs</dd> <dd>3. iGPUs</dd> </dl></para>
			/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgifactory6-enumadapterbygpupreference#">Read more on docs.microsoft.com</see>.</para>
			/// </remarks>
	void EnumAdapterByGPUPreference< A >( uint Adapter, 
										  GPUPreference GpuPreference,
										  in Guid riid,
										  out A ppvAdapter ) where A: IAdapter ;
	
	
	new static Type ComType => typeof(IDXGIFactory6) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIFactory6).GUID
															 .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Factory6( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Factory6( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Factory6( (pComObj as IDXGIFactory6)! ) ;
} ;


// -------------------------------------------------------------------------------------
// Interface Version: IDXGIFactory7 ::
// -------------------------------------------------------------------------------------


[SupportedOSPlatform("windows10.0.17763")]
[ProxyFor( typeof( IDXGIFactory7 ) )]
public interface IFactory7: IFactory6 {
	
	/// <summary>Registers to receive notification of changes whenever the adapter enumeration state changes.</summary>
	/// <param name="hEvent">A handle to the event object.</param>
	/// <param name="pdwCookie">A key value for the registered event.</param>
	/// <returns>Returns <b>S_OK</b> if successful; an error code otherwise.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgifactory7-registeradapterschangedevent">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void RegisterAdaptersChangedEvent( Win32Handle hEvent, out uint pdwCookie ) ;

	/// <summary>Unregisters an event to stop receiving notifications when the adapter enumeration state changes.</summary>
	/// <param name="dwCookie">A key value for the event to unregister.</param>
	/// <returns>Returns <b>S_OK</b> if successful; an error code otherwise.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgifactory7-unregisteradapterschangedevent">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void UnregisterAdaptersChangedEvent( uint dwCookie ) ;
	
	new static Type ComType => typeof(IDXGIFactory7) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIFactory7).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Factory7( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Factory7( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Factory7( (pComObj as IDXGIFactory7)! ) ;
} ;

// -------------------------------------------------------------------------------------


