#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using System.Runtime.Versioning ;
using Windows.UI.Core ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;

#endregion
namespace DXSharp.DXGI ;


// -----------------------------------------------
// Version: IDXGISwapChain
// -----------------------------------------------

[ProxyFor(typeof(IDXGISwapChain))]
public interface ISwapChain: IDeviceSubObject,
							 IInstantiable {
	
	/// <summary>Specifies color space support for the swap chain.</summary>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/ne-dxgi1_4-dxgi_swap_chain_color_space_support_flag">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	[EquivalentOf( typeof( DXGI_SWAP_CHAIN_COLOR_SPACE_SUPPORT_FLAG ) )]
	public enum ColorSpaceSupportFlags: uint {
		/// <summary>Color space support is not present.</summary>
		Unavailable = 0,
		/// <summary>Color space support is present.</summary>
		Present = 1,
		/// <summary>Overlay color space support is present.</summary>
		OverlayPresent = 2,
	} ;
	
	// ==================================================================================
	
	void GetDesc( out SwapChainDescription pDesc ) ;
	
	void Present( uint syncInterval, PresentFlags flags ) ;
	
	void GetBuffer< TResource >( uint buffer, out TResource pSurface )
												where TResource: IDXCOMObject, IInstantiable ;
	
	void SetFullscreenState( bool fullscreen, in IOutput? pTarget ) ;
	
	void GetFullscreenState( out bool pFullscreen, out IOutput? ppTarget ) ;
	
	void ResizeBuffers( uint bufferCount, 
						uint width, 
						uint height,
						Format newFormat, 
						SwapChainFlags swapChainFlags ) ;
	
	void ResizeTarget( in ModeDescription newTargetParameters ) ;
	
	uint GetLastPresentCount( ) ;
	
	void GetContainingOutput( out IOutput? containingOutput ) ;
	
	void GetFrameStatistics( out FrameStatistics pStats ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGISwapChain ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISwapChain ).GUID
																.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new SwapChain( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new SwapChain( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new SwapChain( (IDXGISwapChain)pComObj! ) ;
	// ==================================================================================
} ;


// -----------------------------------------------
// Version: IDXGISwapChain1
// -----------------------------------------------

public interface ISwapChain1: ISwapChain {
	// ---------------------------------------------------------------------------------
	void GetDesc1( out SwapChainDescription1 pDesc ) ;
	void GetFullscreenDesc( out SwapChainFullscreenDescription pDesc ) ;
	
	void GetHwnd( out HWND pHwnd ) ;
	void GetCoreWindow( Guid riid, out CoreWindow? ppUnk ) ;
	
	void Present1( uint syncInterval, 
				   PresentFlags flags,
				   in PresentParameters pPresentParameters ) ;
	
	bool IsTemporaryMonoSupported( ) ;
	
	void GetRestrictToOutput( out IOutput ppRestrictToOutput ) ;
	
	void SetBackgroundColor( in RGBA pColor ) ;
	void GetBackgroundColor( out RGBA pColor ) ;
	
	void SetRotation( ModeRotation rotation ) ;
	void GetRotation( out ModeRotation pRotation ) ;
	
	// ---------------------------------------------------------------------------------
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISwapChain1 ).GUID
																 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new SwapChain1( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new SwapChain1( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new SwapChain1( (IDXGISwapChain1)pComObj! ) ;
	// ==================================================================================
} ;

// -----------------------------------------------
// Version: IDXGISwapChain2
// -----------------------------------------------


public interface ISwapChain2: ISwapChain1 {
	// ---------------------------------------------------------------------------------
	
	/// <summary>Sets the source region to be used for the swap chain.</summary>
	/// <param name="size">The size of the swapchain region</param>
	/// <returns>
	/// <para>This method can return: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-setsourcesize">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetSourceSize( USize size ) ;

	/// <summary>Gets the source region used for the swap chain.</summary>
	/// <param name="size">The size of the swapchain region</param>
	/// <returns>This method can return error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-getsourcesize">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetSourceSize( out USize size ) ;

	/// <summary>Sets the number of frames that the swap chain is allowed to queue for rendering.</summary>
	/// <param name="MaxLatency">The maximum number of back buffer frames that will be queued for the swap chain. This value is 1 by default.</param>
	/// <returns>Returns S_OK if successful; otherwise, DXGI_ERROR_DEVICE_REMOVED if the device was removed.</returns>
	/// <remarks>This method is only valid for use on swap chains created with <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG_FRAME_LATENCY_WAITABLE_OBJECT</a>. Otherwise, the result will be DXGI_ERROR_INVALID_CALL.</remarks>
	void SetMaximumFrameLatency( uint MaxLatency ) ;

	/// <summary>Gets the number of frames that the swap chain is allowed to queue for rendering.</summary>
	/// <param name="pMaxLatency">The maximum number of back buffer frames that will be queued for the swap chain. This value is 1 by default, but should be set to 2 if the scene takes longer than it takes for one vertical refresh (typically about 16ms) to draw.</param>
	/// <returns>
	/// <para>Returns S_OK if successful; otherwise, returns one of the following members of the <a href="https://docs.microsoft.com/windows/desktop/direct3d9/d3derr">D3DERR</a> enumerated type: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-getmaximumframelatency">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetMaximumFrameLatency( out uint pMaxLatency ) ;

	/// <summary>Returns a waitable handle that signals when the DXGI adapter has finished presenting a new frame.</summary>
	/// <returns>A handle to the waitable object, or NULL if the swap chain was not created with <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG_FRAME_LATENCY_WAITABLE_OBJECT</a>.</returns>
	/// <remarks>
	/// <para>When an application is finished using the object handle returned by **IDXGISwapChain2::GetFrameLatencyWaitableObject**, use the <a href="https://docs.microsoft.com/windows/win32/api/handleapi/nf-handleapi-closehandle">CloseHandle</a> function to close the handle.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-getframelatencywaitableobject#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	Win32Handle GetFrameLatencyWaitableObject( ) ;

	/// <summary>Sets the transform matrix that will be applied to a composition swap chain upon the next present.</summary>
	/// <param name="pMatrix">The transform matrix to use for swap chain scaling and translation. This function can only be used with composition swap chains created by <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition">IDXGIFactory2::CreateSwapChainForComposition</a>. Only scale and translation components are allowed in the matrix.</param>
	/// <returns>
	/// <para><b>SetMatrixTransform</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-setmatrixtransform">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetMatrixTransform( in Matrix3x2F pMatrix ) ;

	/// <summary>Gets the transform matrix that will be applied to a composition swap chain upon the next present.</summary>
	/// <param name="pMatrix">
	/// <para>[out] The transform matrix currently used for swap chain scaling and translation.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-getmatrixtransform#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para><b>GetMatrixTransform</b> returns: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-getmatrixtransform">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetMatrixTransform( out Matrix3x2F pMatrix ) ;
	
	// ---------------------------------------------------------------------------------
	
	new static Type ComType => typeof( IDXGISwapChain2 ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISwapChain2 ).GUID
																 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable. Instantiate( ) => new SwapChain2( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new SwapChain2( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) =>
		new SwapChain2( (IDXGISwapChain2)pComObj! ) ;
	// ==================================================================================
} ;


[SupportedOSPlatform("windows10.0.10240")]
[ProxyFor(typeof(IDXGISwapChain3))]
public interface ISwapChain3: ISwapChain2 {
	
	/// <summary>Gets the index of the swap chain's current back buffer.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> Returns the index of the current back buffer.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-getcurrentbackbufferindex">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	uint GetCurrentBackBufferIndex( ) ;

	/// <summary>Checks the swap chain's support for color space.</summary>
	/// <param name="colorSpace">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ne-dxgicommon-dxgi_color_space_type">DXGI_COLOR_SPACE_TYPE</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ne-dxgicommon-dxgi_color_space_type">DXGI_COLOR_SPACE_TYPE</a>-typed value that specifies color space type to check support for.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-checkcolorspacesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pColorSpaceSupport">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> A pointer to a variable that receives a combination of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/ne-dxgi1_4-dxgi_swap_chain_color_space_support_flag">DXGI_SWAP_CHAIN_COLOR_SPACE_SUPPORT_FLAG</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies options for color space support.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-checkcolorspacesupport#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>S_OK</b> on success, or it returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-checkcolorspacesupport">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void CheckColorSpaceSupport( ColorSpaceType colorSpace, out ColorSpaceSupportFlags pColorSpaceSupport ) ;

	/// <summary>Sets the color space used by the swap chain. (IDXGISwapChain3.SetColorSpace1)</summary>
	/// <param name="colorSpace">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ne-dxgicommon-dxgi_color_space_type">DXGI_COLOR_SPACE_TYPE</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ne-dxgicommon-dxgi_color_space_type">DXGI_COLOR_SPACE_TYPE</a>-typed value that specifies the color space to set.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-setcolorspace1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>S_OK</b> on success, or it returns one of the error codes that are described in the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> topic.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-setcolorspace1">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetColorSpace1( ColorSpaceType colorSpace ) ;

	/// <summary>Changes the swap chain's back buffer size, format, and number of buffers, where the swap chain was created using a D3D12 command queue as an input device. This should be called when the application window is resized.</summary>
	/// <param name="bufferCount">
	/// <para>Type: <b>UINT</b> The number of buffers in the swap chain (including all back and front buffers). This number can be different from the number of buffers with which you created the swap chain. This number can't be greater than <b>DXGI_MAX_SWAP_CHAIN_BUFFERS</b>. Set this number to zero to preserve the existing number of buffers in the swap chain. You can't specify less than two buffers for the flip presentation model.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-resizebuffers1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="width">
	/// <para>Type: <b>UINT</b> The new width of the back buffer. If you specify zero, DXGI will use the width of the client area of the target window. You can't specify the width as zero if you called the <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition">IDXGIFactory2::CreateSwapChainForComposition</a> method to create the swap chain for a composition surface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-resizebuffers1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="height">
	/// <para>Type: <b>UINT</b> The new height of the back buffer. If you specify zero, DXGI will use the height of the client area of the target window. You can't specify the height as zero if you called the <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition">IDXGIFactory2::CreateSwapChainForComposition</a> method to create the swap chain for a composition surface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-resizebuffers1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="format">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a></b> A <a href="https://docs.microsoft.com/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>-typed value for the new format of the back buffer. Set this value to <a href="https://docs.microsoft.com/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT_UNKNOWN</a> to preserve the existing format of the back buffer. The flip presentation model supports a more restricted set of formats than the bit-block transfer (bitblt) model.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-resizebuffers1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="SwapChainFlags">
	/// <para>Type: <b>UINT</b> A combination of <a href="https://docs.microsoft.com/windows/win32/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG</a>-typed values that are combined by using a bitwise OR operation. The resulting value specifies options for swap-chain behavior.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-resizebuffers1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pCreationNodeMask">
	/// <para>Type: <b>const UINT*</b> An array of UINTs, of total size <i>BufferCount</i>, where the value indicates which node the back buffer should be created on. Buffers created using <b>ResizeBuffers1</b> with a non-null <i>pCreationNodeMask</i> array are visible to all nodes.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-resizebuffers1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppPresentQueue">
	/// <para>Type: <b>IUnknown*</b> An array of command queues (<a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a> instances), of total size <i>BufferCount</i>. Each queue provided must match the corresponding creation node mask specified in the <i>pCreationNodeMask</i> array. When <b>Present()</b> is called, in addition to rotating to the next buffer for the next frame, the swapchain will also rotate through these command queues. This allows the app to control which queue requires synchronization for a given present operation.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-resizebuffers1#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method is only valid to call when the swapchain was created using a D3D12 command queue (<a href="https://docs.microsoft.com/windows/win32/api/d3d12/nn-d3d12-id3d12commandqueue">ID3D12CommandQueue</a>) as an input device.</para>
	/// <para>When a swapchain is created on a multi-GPU adapter, the backbuffers are all created on node 1 and only a single command queue is supported. <b>ResizeBuffers1</b> enables applications to create backbuffers on different nodes, allowing a different command queue to be used with each node. These capabilities enable Alternate Frame Rendering (AFR) techniques to be used with the swapchain. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para>Also see the Remarks section in <a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-resizebuffers">IDXGISwapChain::ResizeBuffers</a>, all of which is relevant to <b>ResizeBuffers1</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiswapchain3-resizebuffers1#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	HResult ResizeBuffers1( uint bufferCount = 0U,
						 uint width = 0U, uint height = 0U,
						 Format format = Format.UNKNOWN,
						 SwapChainFlags SwapChainFlags = SwapChainFlags.None,
						 uint[ ]? pCreationNodeMask = null,
						 Direct3D12.ICommandQueue[ ]? ppPresentQueue = null ) ;

	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGISwapChain3 ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISwapChain3 ).GUID
																 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable. Instantiate( ) => new SwapChain3( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new SwapChain3( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new SwapChain3( (IDXGISwapChain3)pComObj! ) ;
	// ==================================================================================
} ;



/// <summary>
/// Extends <see cref="ISwapChain3"/> with an additional method to set video metadata.
/// </summary>
/// <remarks>
/// <b>WARNING</b>: <para/>
/// It is no longer recommended for apps to explicitly set HDR metadata on their swap chain using SetHDRMetaData.
/// Windows does not guarantee that swap chain metadata is sent to the monitor, and monitors do not handle HDR
/// metadata consistently. Therefore it's recommended that apps always tone-map content into the range reported
/// by the monitor. For more details on how to write apps that react dynamically to monitor capabilities, see
/// <a href="https://docs.microsoft.com/windows/win32/direct3darticles/high-dynamic-range">
/// Using DirectX with high dynamic range displays and Advanced Color.
/// </a>
/// </remarks>
[SupportedOSPlatform("windows10.0.10240")]
[ProxyFor(typeof(IDXGISwapChain4))]
public interface ISwapChain4: ISwapChain3 {
	
	/// <summary>This method sets High Dynamic Range (HDR) and Wide Color Gamut (WCG) header metadata.</summary>
	/// <param name="Type">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ne-dxgi1_5-dxgi_hdr_metadata_type">DXGI_HDR_METADATA_TYPE</a></b> Specifies one member of the  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ne-dxgi1_5-dxgi_hdr_metadata_type">DXGI_HDR_METADATA_TYPE</a> enum.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgiswapchain4-sethdrmetadata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Size">
	/// <para>Type: <b>UINT</b> Specifies the size of <i>pMetaData</i>, in bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgiswapchain4-sethdrmetadata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pMetaData">
	/// <para>Type: <b>void*</b> Specifies a void pointer that references the metadata, if it exists. Refer to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ns-dxgi1_5-dxgi_hdr_metadata_hdr10">DXGI_HDR_METADATA_HDR10</a> structure.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgiswapchain4-sethdrmetadata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method sets metadata to enable a monitor's output to be adjusted depending on its capabilities. However it does not change how pixel values are interpreted by Windows or monitors. To adjust the color space of the swap chain, use [**SetColorSpace1**](..\dxgi1_4\nf-dxgi1_4-idxgiswapchain3-setcolorspace1.md) instead. Applications should not rely on the metadata being sent to the monitor as the the metadata may be ignored. Monitors do not consistently process HDR metadata, resulting in varied appearance of your content across different monitors. In order to ensure more consistent output across a range of monitors, devices, and use cases, it is recommended to not use **SetHDRMetaData** and to instead tone-map content into the gamut and luminance range supported by the monitor. See [IDXGIOutput6::GetDesc1](../dxgi1_6/nf-dxgi1_6-idxgioutput6-getdesc1.md) to retrieve the monitor's supported gamut and luminance range. Monitors adhering to the VESA DisplayHDR standard will automatically perform a form of clipping for content outside of the monitor's supported gamut and luminance range. For more details on how to write apps that react dynamically to monitor capabilities, see [Using DirectX with high dynamic range displays and Advanced Color](/windows/win32/direct3darticles/high-dynamic-range).</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgiswapchain4-sethdrmetadata#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetHDRMetaData( HDRMetaDataType Type, uint Size, [Optional] in HDRMetaDataHDR10? pMetaData ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGISwapChain4 ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGISwapChain4 ).GUID
																 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new SwapChain4( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new SwapChain4( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new SwapChain4( (IDXGISwapChain4)pComObj! ) ;
	// ==================================================================================
} ;



