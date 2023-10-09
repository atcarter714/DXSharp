#region Using Directives
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi;
using Windows.Win32.Graphics.Dxgi.Common;
using DXSharp.Windows.Win32 ;
using DXSharp.Windows.Win32.Helpers ;
using winMD = Windows.Win32.Foundation ;

using DXGI_MODE_DESC = Windows.Win32.Graphics.Dxgi.Common.DXGI_MODE_DESC;
using DXGI_MODE_DESC1 = Windows.Win32.Graphics.Dxgi.DXGI_MODE_DESC1;
using DXGI_SAMPLE_DESC = Windows.Win32.Graphics.Dxgi.Common.DXGI_SAMPLE_DESC;
using DXGI_SWAP_EFFECT = Windows.Win32.Graphics.Dxgi.DXGI_SWAP_EFFECT;
#endregion

namespace DXSharp.DXGI ;


// ---------------------------------------------------------------------------------------
// DXGI Enumerations ::
// ---------------------------------------------------------------------------------------

// Issue: the auto-documentation had "D2D1_ALPHA_MODE" instead of DXGI_ALPHA_MODE
// but the link was actually correct, oddly enough ...


/// <summary>Identifies the alpha value, transparency behavior, of a surface.</summary>
/// <remarks>
/// <para>For more information about alpha mode, see <a href="https://docs.microsoft.com/windows/desktop/api/dcommon/ne-dcommon-d2d1_alpha_mode">DXGI_ALPHA_MODE</a>.</para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ne-dxgi1_2-dxgi_alpha_mode#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
public enum AlphaMode: uint {
	/// <summary>Indicates that the transparency behavior is not specified.</summary>
	Unspecified = 0U,
	/// <summary>Indicates that the transparency behavior is premultiplied. Each color is first scaled by the alpha value. The alpha value itself is the same in both straight and premultiplied alpha. Typically, no color channel value is greater than the alpha channel value. If a color channel value in a premultiplied format is greater than the alpha channel, the standard source-over blending math results in an additive blend.</summary>
	Premultiplied = 1U,
	/// <summary>Indicates that the transparency behavior is not premultiplied. The alpha channel indicates the transparency of the color.</summary>
	Straight = 2U,
	/// <summary>Indicates to ignore the transparency behavior.</summary>
	Ignore = 3U,
	/// <summary>
	/// <para>Forces this enumeration to compile to 32 bits in size. Without this value, some compilers would allow this enumeration to compile to a size other than 32 bits. This value is not used.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ne-dxgi1_2-dxgi_alpha_mode#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	/// <remarks>
	/// For these C# bindings this should be a non-issue. The underlying type is already uint/UInt32, which is 32-bit.
	/// </remarks>
	ForceDWORD = 4294967295U,
}

/// <summary>
/// Flags indicating the method the raster uses to create an image on a surface.
/// </summary>
/// <remarks>
/// <para>See <a href="https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/bb173067(v=vs.85)">DXGI_MODE_SCANLINE_ORDER</a> for more info.</para>
/// </remarks>
public enum ScanlineOrder {
	/// <summary>
	/// Scanline order is unspecified.
	/// </summary>
	Unspecified = 0,
	/// <summary>
	/// The image is created from the first scanline to the last without skipping any.
	/// </summary>
	Progressive = 1,
	/// <summary>
	/// The image is created beginning with the upper field.
	/// </summary>
	UpperFieldFirst = 2,
	/// <summary>
	/// The image is created beginning with the lower field.
	/// </summary>
	LowerFieldFirst = 3,
};

/// <summary>Identifies resize behavior when the back-buffer size does not match the size of the target output.</summary>
/// <remarks>
/// <para>The DXGI_SCALING_NONE value is supported only for flip presentation model swap chains that you create with the 
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL</a> 
/// value. You pass these values in a call to 
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforhwnd">IDXGIFactory2::CreateSwapChainForHwnd</a>, 
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcorewindow">IDXGIFactory2::CreateSwapChainForCoreWindow</a>, 
/// or  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition">IDXGIFactory2::CreateSwapChainForComposition</a>. 
/// 
/// DXGI_SCALING_ASPECT_RATIO_STRETCH will prefer to use a horizontal fill, otherwise it will use a vertical fill, using the 
/// following logic. 
/// <pre class="syntax" xml:space="preserve"><code>
/// // C++ version (C# translation needed):
/// float aspectRatio = backBufferWidth / float(backBufferHeight); 
/// 
///	// Horizontal fill float scaledWidth = outputWidth; float scaledHeight = outputWidth / aspectRatio; 
///	if (scaledHeight &gt;= outputHeight) { 
///		// Do vertical fill 
///		scaledWidth = outputHeight * aspectRatio; 
///		scaledHeight = outputHeight; 
///	} 
///	
/// float offsetX = (outputWidth - scaledWidth) * 0.5f; 
///	float offsetY = (outputHeight - scaledHeight) * 0.5f; 
///	rect.left = static_cast&lt;LONG&gt;(offsetX); 
///	rect.top = static_cast&lt;LONG&gt;(offsetY); 
/// rect.right = static_cast&lt;LONG&gt;(offsetX + scaledWidth); 
/// rect.bottom = static_cast&lt;LONG&gt;(offsetY + scaledHeight); 
/// rect.left = std::max&lt;LONG&gt;(0, rect.left); 
/// rect.top = std::max&lt;LONG&gt;(0, rect.top); 
/// rect.right = std::min&lt;LONG&gt;(static_cast&lt;LONG&gt;(outputWidth), rect.right); 
/// rect.bottom = std::min&lt;LONG&gt;(static_cast&lt;LONG&gt;(outputHeight), rect.bottom); 
/// </code></pre> 
/// 
/// Note that <i>outputWidth</i> and <i>outputHeight</i> are the pixel sizes of the presentation target size. In the case of <b>CoreWindow</b>, this requires converting the <i>logicalWidth</i> and <i>logicalHeight</i> values from DIPS to pixels using the window's DPI property.</para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ne-dxgi1_2-dxgi_scaling#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
public enum Scaling {
	/// <summary>
	/// Directs DXGI to make the back-buffer contents scale to fit the presentation target size. 
	/// This is the implicit behavior of DXGI when you call the 
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">IDXGIFactory::CreateSwapChain</a> method.
	/// </summary>
	Stretch = 0,
	/// <summary>
	/// <para>Directs DXGI to make the back-buffer contents appear without any scaling when the presentation target size is not equal to the back-buffer size. The top edges of the back buffer and presentation target are aligned together. If the WS_EX_LAYOUTRTL style is associated with the <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a> handle to the target output window, the right edges of the back buffer and presentation target are aligned together; otherwise, the left edges are aligned together. All target area outside the back buffer is filled with window background color. This value specifies that all target areas outside the back buffer of a swap chain are filled with the background color that you specify in a call to <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-setbackgroundcolor">IDXGISwapChain1::SetBackgroundColor</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ne-dxgi1_2-dxgi_scaling#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	None = 1,
	/// <summary>
	/// <para>Directs DXGI to make the back-buffer contents scale to fit the presentation 
	/// target size, while preserving the aspect ratio of the back-buffer. If the scaled 
	/// back-buffer does not fill the presentation area, it will be centered with black 
	/// borders. This constant is supported on Windows Phone 8 and Windows 10. Note that 
	/// with legacy Win32 window swapchains, this works the same as DXGI_SCALING_STRETCH.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ne-dxgi1_2-dxgi_scaling#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	AspectRatioStretch = 2,
};

/// <summary>
/// Flags indicating how an image is stretched to fit a given monitor's resolution.
/// </summary>
/// <remarks>
/// Selecting the CENTERED or STRETCHED modes can result in a mode change even if 
/// you specify the native resolution of the display in the DXGI_MODE_DESC. If you 
/// know the native resolution of the display and want to make sure that you do not 
/// initiate a mode change when transitioning a swap chain to full screen (either via 
/// ALT+ENTER or IDXGISwapChain::SetFullscreenState), you should use UNSPECIFIED.
/// <para>
/// This enum is used by the DXGI_MODE_DESC1 and DXGI_SWAP_CHAIN_FULLSCREEN_DESC structures.
/// </para>
/// More information at <a href="https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/bb173066(v=vs.85)">DXGI_MODE_SCALING enumeration</a>
/// </remarks>
public enum ScalingMode {
	/// <summary>
	/// Unspecified scaling.
	/// </summary>
	Unspecified = 0,
	/// <summary>
	/// Specifies no scaling. The image is centered on the display. 
	/// This flag is typically used for a fixed-dot-pitch display 
	/// (such as an LED display).
	/// </summary>
	Centered = 1,
	/// <summary>
	/// Specifies stretched scaling.
	/// </summary>
	Stretched = 2,
};

/// <summary>
/// Flags for surface and resource creation options.
/// </summary>
/// <remarks>
/// For more information see: 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/dxgi-usage">DXGI_USAGE</a>
/// </remarks>
public enum Usage: long {
	/// <summary>
	/// No flags
	/// </summary>
	Unknown = 0x0000,
	/// <summary>
	/// The surface or resource is used as a back buffer. You don’t need to pass 
	/// Usage.BackBuffer when you create a swap chain. But you can determine 
	/// whether a resource belongs to a swap chain when you call DXGI.IResource.GetUsage 
	/// and get Usage.BackBuffer ...
	/// </summary>
	BackBuffer = 1L << (2 + 4),
	/// <summary>
	/// This flag is for internal use only.
	/// </summary>
	DiscardOnPresent_internal   = 1L << (5 + 4),
	/// <summary>
	/// Use the surface or resource for reading only.
	/// </summary>
	ReadOnly                    = 1L << (4 + 4),
	/// <summary>
	/// Use the surface or resource as an output render target.
	/// </summary>
	RenderTargetOutput          = 1L << (1 + 4),
	/// <summary>
	/// Use the surface or resource as an input to a shader.
	/// </summary>
	ShaderInput                 = 1L << (0 + 4),
	/// <summary>
	/// Share the surface or resource.
	/// </summary>
	Shared                      = 1L << (3 + 4),
	/// <summary>
	/// Use the surface or resource for unordered access.
	/// </summary>
	UnorderedAccess             = 1L << (6 + 4),
};

/// <summary>Options for handling pixels in a display surface after calling DXGI.ISwapChain1.Present1.</summary>
/// <remarks>
/// <para>
/// <see href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#">Read more on docs.microsoft.com</see>.
/// </para>
/// </remarks>
public enum SwapEffect {
	/// <summary>
	/// <para>Use this flag to specify the bit-block transfer (bitblt) model and to specify that DXGI discard the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. This flag is valid for a swap chain with more than one back buffer, although, applications only have read and write access to buffer 0. Use this flag to enable the display driver to select the most efficient presentation technique for the swap chain. <b>Direct3D 12:  </b>This enumeration value is never supported. D3D12 apps must using <b>FlipSequential</b> or <b>FlipDiscard</b>. <div class="alert"><b>Note</b>  There are differences between full screen exclusive and full screen UWP. If you are porting a Direct3D 11 application to UWP on a Windows PC, be aware that the use of  <b>Discard</b> when creating swap chains does not behave the same way in UWP as it does in Win32, and its use may be detrimental to GPU performance. This is because UWP applications are forced into FLIP swap modes (even if other swap modes are set), because this reduces the computation time used by the memory copies originally done by the older bitblt model. The recommended approach is to manually convert DX11 Discard swap chains to use flip models within UWP,  using <b>FlipDiscard</b> instead of <b>Discard</b> where possible. Refer to the Example below, and see <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/for-best-performance--use-dxgi-flip-model">this article</a> for more information.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Discard         = 0,
	/// <summary>
	/// <para>Use this flag to specify the bitblt model and to specify that DXGI persist the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. Use this option to present the contents of the swap chain in order, from the first buffer (buffer 0) to the last buffer. This flag cannot be used with multisampling. <b>Direct3D 12:  </b>This enumeration value is never supported. D3D12 apps must using <b>FlipSequential</b> or <b>FlipDiscard</b>.</para>
	/// <para><div class="alert"><b>Note</b>  For best performance, use <b>FlipSequential</b> instead of <b>Sequential</b>. See <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/for-best-performance--use-dxgi-flip-model">this article</a> for more information.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Sequential      = 1,
	/// <summary>
	/// <para>Use this flag to specify the flip presentation model and to specify that DXGI persist the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. This flag cannot be used with multisampling.</para>
	/// <para><b>Direct3D 11:  </b>This enumeration value is supported starting with Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	FlipSequential  = 3,
	/// <summary>
	/// <para>Use this flag to specify the flip presentation model and to specify that DXGI discard the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. This flag cannot be used with multisampling and partial presentation. See <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/dxgi-1-4-improvements">DXGI 1.4 Improvements</a>.</para>
	/// <para><b>Direct3D 11:  </b>This enumeration value is supported starting with Windows 10.</para>
	/// <para><div class="alert"><b>Note</b>  Windows Store apps must use <b>FlipSequential</b> or <b>FlipDiscard</b>. </div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	FlipDiscard     = 4,
};

// CsWin32 did not generate an enumerated type for this?
/// <summary>
/// Options for swap-chain behavior.
/// </summary>
/// <remarks>
/// These values can be combined as bitflags for many unique combinations
/// with different behavior.
/// <para><b>NOTES:</b></para>
/// You don't need to set SwapChainFlag.DisplayOnly for swap chains that 
/// you create in full-screen mode with the 
/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">DXGI.IFactory.CreateSwapChain method</a> 
/// because those swap chains already behave as if SwapChainFlag.DisplayOnly 
/// is set. That is, presented content is not accessible by remote access or through 
/// the <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi1_2/nn-dxgi1_2-idxgioutputduplication">desktop duplication APIs</a>.
/// <para>
/// For more information, please see 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG</a>
/// </para>
/// </remarks>
[Flags]
public enum SwapChainFlags {
	/// <summary>
	/// No flags
	/// </summary>
	NONE = 0,
	/// <summary>
	/// <para><b>Value: 1</b></para>
	/// Set this flag to turn off automatic image rotation; that is, do not perform a 
	/// rotation when transferring the contents of the front buffer to the monitor.
	/// Use this flag to avoid a bandwidth penalty when an application expects to handle rotation. 
	/// This option is valid only during full-screen mode.
	/// </summary>
	NonPreRotated = 1,
	/// <summary>
	/// <para><b>Value: 2</b></para>
	/// Set this flag to enable an application to switch modes by calling 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-resizetarget">DXGI.ISwapChain.ResizeTarget</a>.
	/// When switching from windowed to full-screen mode, the display mode (or monitor resolution) will 
	/// be changed to match the dimensions of the application window.
	/// </summary>
	AllowModeSwitch = 2,
	/// <summary>
	/// <para><b>Value: 4</b></para>
	/// Set this flag to enable an application to render using GDI on a swap chain or a surface.
	/// This will allow the application to call 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi/nf-dxgi-idxgisurface1-getdc">DXGI.ISurface1.GetDC</a> 
	/// on the 0th back buffer or a surface.
	/// <para>This flag is <b>not</b> applicable for Direct3D 12.</para>
	/// </summary>
	GDICompatible = 4,
	/// <summary>
	/// <para><b>Value: 8</b></para>
	/// Set this flag to indicate that the swap chain might contain protected content; therefore, 
	/// the operating system supports the creation of the swap chain only when driver and hardware 
	/// protection is used. If the driver and hardware do not support content protection, the call 
	/// to create a resource for the swap chain fails.
	/// <para><b>Direct3D 11:</b>  This enumeration value is supported starting with Windows 8.</para>
	/// </summary>
	RestrictedContent = 8,
	/// <summary>
	/// <para><b>Value: 16</b></para>
	/// Set this flag to indicate that shared resources that are created within the swap chain must be 
	/// protected by using the driver’s mechanism for restricting access to shared surfaces.
	/// <para><b>Direct3D 11:</b>  This enumeration value is supported starting with Windows 8.</para>
	/// </summary>
	RestrictSharedResourceDriver = 16,
	/// <summary>
	/// <para><b>Value: 32</b></para>
	/// Set this flag to restrict presented content to the local displays. Therefore, the presented content is not accessible via remote accessing or through the 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi1_2/nn-dxgi1_2-idxgioutputduplication">desktop duplication APIs</a>
	/// <para>This flag supports the window content protection features of Windows. Applications can use this flag to protect their own onscreen window content from being captured or copied through a specific set of public operating system features and APIs.</para>
	/// <para>If you use this flag with windowed (HWND or IWindow) swap chains where another process created the HWND, the owner of the HWND must use the 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-setwindowdisplayaffinity">SetWindowDisplayAffinity</a> 
	/// function appropriately in order to allow calls to 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-present">DXGI.ISwapChain.Present</a> 
	/// or 
	/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">DXGI.ISwapChain1.Present1</a> 
	/// to succeed.</para>
	/// <para><b>Direct3D 11:</b>  This enumeration value is supported starting with Windows 8.</para>
	/// </summary>
	DisplayOnly = 32,
	/// <summary>
	/// <para><b>Value: 64</b></para>
	/// Set this flag to create a waitable object you can use to ensure rendering does not 
	/// begin while a frame is still being presented. When this flag is used, the swapchain's 
	/// latency must be set with the 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-setmaximumframelatency">DXGI.ISwapChain2.SetMaximumFrameLatency</a> 
	/// API instead of 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi/nf-dxgi-idxgidevice1-setmaximumframelatency">DXGI.IDevice1.SetMaximumFrameLatency</a>.
	/// <para>This flag isn't supported in full-screen mode, unless the render API is Direct3D 12.</para>
	/// <para><b>Direct3D 11:</b>  This enumeration value is supported starting with Windows 8.1.</para>
	/// </summary>
	FrameLatencyWaitableObject = 64,
	/// <summary>
	/// <para><b>Value: 128</b></para>
	/// Set this flag to create a swap chain in the foreground layer for multi-plane rendering. 
	/// This flag can only be used with 
	/// <a href="https://learn.microsoft.com/en-us/uwp/api/Windows.UI.Core.CoreWindow">CoreWindow</a> 
	/// swap chains, which are created with 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcorewindow">CreateSwapChainForCoreWindow</a>. 
	/// Apps should not create foreground swap chains if 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi1_3/nf-dxgi1_3-idxgioutput2-supportsoverlays">DXGI.IOutput2.SupportsOverlays</a> 
	/// indicates that hardware support for overlays is not available.
	/// 
	/// <para>Note that IDXGISwapChain::ResizeBuffers cannot be used to add or remove this flag.</para>
	/// <para><b>Direct3D 11:</b>  This enumeration value is supported starting with Windows 8.1.</para>
	/// </summary>
	ForegroundLayer = 128,
	/// <summary>
	/// <para><b>Value: 256</b></para>
	/// Set this flag to create a swap chain for full-screen video.
	/// <para><b>Direct3D 11:</b>  This enumeration value is supported starting with Windows 8.1.</para>
	/// </summary>
	FullscreenVideo = 256,
	/// <summary>
	/// <para><b>Value: 512</b></para>
	/// Set this flag to create a swap chain for YUV video.
	/// <para><b>Direct3D 11:</b>  This enumeration value is supported starting with Windows 8.1.</para>
	/// </summary>
	YUVVideo = 512,
	/// <summary>
	/// <para><b>Value: 1024</b></para>
	/// Indicates that the swap chain should be created such that all underlying resources 
	/// can be protected by the hardware. Resource creation will fail if hardware content 
	/// protection is not supported.
	/// 
	/// <para><b>This flag has the following restrictions:</b></para>
	/// <para>This flag can only be used with swap effect DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL.</para>
	/// </summary>
	/// <remarks>
	/// <b>NOTE: </b>Creating a swap chain using this flag does not automatically guarantee that 
	/// hardware protection will be enabled for the underlying allocation. Some implementations 
	/// require that the DRM components are first initialized prior to any guarantees of protection.
	/// 
	/// <para><b>Windows Versions:</b>  This enumeration value is supported starting with Windows 10.</para>
	/// </remarks>
	HW_Protected = 1024,
	/// <summary>
	/// <para><b>Value: 2048</b></para>
	/// Tearing support is a requirement to enable displays that support variable refresh 
	/// rates to function properly when the application presents a swap chain tied to a full 
	/// screen borderless window. Win32 apps can already achieve tearing in fullscreen exclusive 
	/// mode by calling 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-setfullscreenstate">SetFullscreenState</a>(true), 
	/// but the recommended approach for Win32 developers is to use this tearing flag instead.
	/// 
	/// <para>This flag requires the use of a DXGI_SWAP_EFFECT_FLIP_* swap effect.</para>
	/// </summary>
	/// <remarks>
	/// To check for hardware support of this feature, refer to 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi1_5/nf-dxgi1_5-idxgifactory5-checkfeaturesupport">DXGI.IFactory5.CheckFeatureSupport</a>. 
	/// For usage information refer to 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-present">DXGI.ISwapChain.Present</a> 
	/// and the 
	/// <a href="https://learn.microsoft.com/en-us/windows/desktop/direct3ddxgi/dxgi-present">DXGI_PRESENT flags</a>.
	/// <para><b>NOTE:</b> <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-resizebuffers">DXGI.ISwapChain.ResizeBuffers</a> can't be used to add or remove this flag.</para>
	/// </remarks>
	AllowTearing = 2048,
	/// <summary>
	/// <para><b>Value: 4096</b></para>
	/// Undocumented flag for holographi displays ???
	/// </summary>
	RestrictedToAllHolographicDisplays = 4096
};


public enum Residency {
	/// <summary>The resource is located in video memory.</summary>
	FullResident = 1,
	/// <summary>At least some of the resource is located in CPU memory.</summary>
	SharedMemory = 2,
	/// <summary>At least some of the resource has been paged out to the hard drive.</summary>
	EvictedToDisk = 3,
} ;



// ---------------------------------------------------------------------------------------
// DXGI Structures ::
// ---------------------------------------------------------------------------------------


/// <summary>Represents a rational number.</summary>
/// <remarks>
/// <para>This structure is a member of the 
/// <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXG.ModeDescription</a> structure.
/// The <b>DXGI_RATIONAL</b> structure operates under the following rules: </para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_rational#">Read more on docs.microsoft.com</see>.</para>
/// <para>The DXGI_RATIONAL structure operates under the following rules:</para>
/// <para>0/0 is legal and will be interpreted as 0/1.</para>
/// <para>0/anything is interpreted as zero.</para>
/// <para>If you are representing a whole number, the denominator should be 1.</para>
/// 
/// </remarks>
[DebuggerDisplay( "Fraction: {numerator}/{denominator} (Float: {AsFloat}f)" )]
public struct Rational: IEquatable< Rational > {
	/// <summary>
	/// A Rational with a value of zero
	/// </summary>
	public static readonly Rational Zero = (0x00u, 0x01u);

	/// <summary>
	/// Gets the rational value as a float
	/// </summary>
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	public float AsFloat => denominator == 0 ? 0f : numerator / denominator;



	internal Rational( in DXGI_RATIONAL rational ) {
		this.numerator = rational.Numerator;
		this.denominator = rational.Denominator;
	}

	internal unsafe Rational( DXGI_RATIONAL* pRational ) {
		fixed( Rational* pThis = &this )
			*pThis = *((Rational*)pRational);
	}

	/// <summary>
	/// Creates a new rational value
	/// </summary>
	public Rational() {
		this.numerator = 0;
		this.denominator = 1;
	}

	/// <summary>
	/// Creates a new rational value
	/// </summary>
	/// <param name="numerator">
	/// The numerator value (denominator will be set to 1)
	/// </param>
	public Rational( uint numerator ) {
		this.numerator = numerator;
		this.denominator = 1;
	}

	/// <summary>
	/// Creates a new rational value
	/// </summary>
	/// <param name="numerator">The numerator value</param>
	/// <param name="denominator">The denominator value</param>
	public Rational( uint numerator, uint denominator ) {
#if DEBUG || !STRIP_CHECKS
		if( denominator == 0 )
			throw new ArgumentOutOfRangeException( nameof( denominator ),
				"Rationals should not have a denominator of zero!" );
#endif

		this.numerator = numerator;
		this.denominator = 1;
	}

	/// <summary>
	/// Creates a new Rational value
	/// </summary>
	/// <param name="values">Tuple holding numerator and denominator values</param>
	public Rational( (uint numerator, uint denominator) values ) {
#if DEBUG || !STRIP_CHECKS
		if( values.denominator == 0 )
			throw new ArgumentOutOfRangeException( "denominator",
				"Rationals should not have a denominator of zero!" );
#endif

		this.numerator = values.numerator;
		this.denominator = values.denominator;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	uint numerator;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	uint denominator;



	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> An unsigned integer value representing the top of the rational number.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_rational#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint Numerator { get => numerator; set => numerator = value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> An unsigned integer value representing the bottom of the rational number.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_rational#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint Denominator { get => denominator; set => denominator = value; }





	/// <summary>
	/// Reduces the rational/fraction value
	/// </summary>
	public void Reduce() {
		uint k = GDC( this );
		numerator /= k;
		denominator /= k;
	}

	/// <summary>
	/// Gets the reduced value of this rational/fraction value
	/// </summary>
	/// <returns>Reduced rationa/fraction value</returns>
	public Rational Reduced() {
		var copy = this;
		copy.Reduce();
		return copy;
	}

	/// <summary>
	/// Determines if the given object and this value are equal
	/// </summary>
	/// <param name="obj">object to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public override bool Equals( [NotNullWhen( true )] object? obj ) =>
		obj is Rational r && Equals( r );

	/// <summary>
	/// Determines if the given rational value and this value are equal
	/// </summary>
	/// <param name="other">Rational value to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public bool Equals( Rational other ) =>
		(other.numerator == 0 && this.numerator == 0) ||
		(other.denominator == 0 && this.denominator == 0) ||
		(other.numerator == this.numerator && other.denominator == this.denominator);

	/// <summary>
	/// Gets the 32-bit hash code of this rational value
	/// </summary>
	/// <returns>32-bit hash code</returns>
	public override int GetHashCode() => (numerator, denominator).GetHashCode();



	/// <summary>
	/// Finds the greatest common denominator of a rational value
	/// </summary>
	/// <param name="r">A rational/fraction value</param>
	/// <returns>Greatest common denominator</returns>
	/// <exception cref="PerformanceException">
	/// Thrown in DEBUG build if the search for greatest common 
	/// denominator takes an unreasonably long amount of time 
	/// </exception>
	public static uint GDC( Rational r ) {
		uint n = r.numerator, m = r.denominator;

		if( n < m ) {
			(m, n) = (n, m);
		}

#if DEBUG || !STRIP_CHECKS
		uint count = 0x00;
		const uint MAX_ITERATIONS = 100000;
#endif
		while( m > 0 ) {
			uint tmp = n % m;
			n = m;
			m = tmp;

#if DEBUG || !STRIP_CHECKS
			if( ++count > MAX_ITERATIONS ) {
				throw new PerformanceException( $"Rational.GDC(): " +
					$"Algorithm has run over {MAX_ITERATIONS} iterations to determine greatest common demoninator. " +
					$"Consider expressing this value in a more reduced form or check to ensure it is valid." );
			}
#endif
		}

		return n;
	}



	/// <summary>
	/// Converts a tuple of uints to Rational
	/// </summary>
	/// <param name="values">Value tuple of two uint values</param>
	public static implicit operator Rational( (uint numerator, uint denominator) values ) => new( values );

	/// <summary>
	/// Converts an unsigned value into rational form
	/// </summary>
	/// <param name="value">A whole, unsigned value</param>
	public static implicit operator Rational( uint value ) => new( value );

	public static bool operator ==( Rational a, Rational b ) => a.Equals( b );
	public static bool operator !=( Rational a, Rational b ) => !a.Equals( b );
	public static bool operator ==( Rational a, uint b ) => a.Equals( b );
	public static bool operator !=( Rational a, uint b ) => !a.Equals( b );
	public static bool operator ==( uint a, Rational b ) => a.Equals( b );
	public static bool operator !=( uint a, Rational b ) => !a.Equals( b );
} ;


/// <summary>Describes multi-sampling parameters for a resource.</summary>
/// <remarks>
/// <para>This structure is a member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">DXGI_SWAP_CHAIN_DESC1</a> structure. The default sampler mode, with no anti-aliasing, has a count of 1 and a quality level of 0. If multi-sample antialiasing is being used, all bound render targets and depth buffers must have the same sample counts and quality levels. </para>
/// <para>This doc was truncated.</para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_sample_desc#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
public struct SampleDescription {
	internal SampleDescription( in DXGI_SAMPLE_DESC desc ) {
		this.count = desc.Count;
		this.quality = desc.Quality;
	}

	internal unsafe SampleDescription( DXGI_SAMPLE_DESC* pDesc ) {
		fixed( SampleDescription* pThis = &this ) {
			*pThis = *((SampleDescription*)pDesc);
		}
	}

	/// <summary>
	/// Creates a new SampleDescription
	/// </summary>
	/// <param name="count">Number of multisamples per pixel</param>
	/// <param name="quality">The image quality level</param>
	public SampleDescription( uint count, uint quality ) {
		this.count = count;
		this.quality = quality;
	}

	/// <summary>
	/// Creates a new SampleDescription
	/// </summary>
	/// <param name="values">Tuple holding count and quality values</param>
	public SampleDescription( (uint count, uint quality) values ) {
		this.count = values.count;
		this.quality = values.quality;
	}

	[DebuggerBrowsable( DebuggerBrowsableState.Never )] uint count, quality ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of multisamples per pixel.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_sample_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint Count { get => count; set => count = value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The image quality level. The higher the quality, the lower the performance. The valid range is between zero and one less than the level returned by <a href="https://docs.microsoft.com/windows/desktop/api/d3d10/nf-d3d10-id3d10device-checkmultisamplequalitylevels">ID3D10Device::CheckMultisampleQualityLevels</a> for Direct3D 10 or <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11device-checkmultisamplequalitylevels">ID3D11Device::CheckMultisampleQualityLevels</a> for Direct3D 11. For Direct3D 10.1 and Direct3D 11, you can use two special quality level values. For more information about these quality level values, see Remarks.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_sample_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint Quality { get => quality; set => quality = value; }

	
	public static implicit operator SampleDescription(
		(uint count, uint quality) values ) => new( values.count, values.quality );

	public static implicit operator (uint count, uint quality)( SampleDescription sDesc ) =>
		(count: sDesc.Count, quality: sDesc.Quality);
} ;


//typedef struct DXGI_SWAP_CHAIN_DESC
//{
//	DXGI_MODE_DESC BufferDesc;
//	DXGI_SAMPLE_DESC SampleDesc;
//	DXGI_USAGE BufferUsage;
//	UINT BufferCount;
//	HWND OutputWindow;
//	BOOL Windowed;
//	DXGI_SWAP_EFFECT SwapEffect;
//	UINT Flags;
//} DXGI_SWAP_CHAIN_DESC;

/// <summary>Describes a swap chain.</summary>
/// <remarks>
/// <para>This structure is used by the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-getdesc">GetDesc</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">CreateSwapChain</a> methods. In full-screen mode, there is a dedicated front buffer; in windowed mode, the desktop is the front buffer. If you create a swap chain with one buffer, specifying <b>DXGI_SWAP_EFFECT_SEQUENTIAL</b> does not cause the contents of the single buffer to be swapped with the front buffer. For performance information about flipping swap-chain buffers in full-screen application, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/d3d10-graphics-programming-guide-dxgi">Full-Screen Application Performance Hints</a>.</para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
public struct SwapChainDescription {
	internal SwapChainDescription( in DXGI_SWAP_CHAIN_DESC desc ) {
		//this.BufferDesc = desc.BufferDesc;
		//this.SampleDesc = desc.SampleDesc;
		//this.BufferUsage = (Usage) desc.BufferUsage;
		//this.BufferCount = desc.BufferCount;
		//this.OutputWindow = desc.OutputWindow;
		//this.Windowed = desc.Windowed;
		//this.SwapEffect = (SwapEffect) desc.SwapEffect;
		//this.Flags = (SwapChainFlags) desc.Flags;
		this.desc = desc;
	}

	internal unsafe SwapChainDescription( DXGI_SWAP_CHAIN_DESC* pDesc ) => desc = *pDesc;

	/// <summary>
	/// Creates a new SwapChainDescription
	/// </summary>
	/// <param name="bufferDesc">The swapchain ModeDescription for the display</param>
	/// <param name="sampleDesc">The multisampling settings description</param>
	/// <param name="bufferUsage">The DXGI Usage flags</param>
	/// <param name="bufferCount">The backbuffer count</param>
	/// <param name="outputWindow">Handle to the output window</param>
	/// <param name="windowed">Indicates if swapchain is in windowed mode</param>
	/// <param name="swapEffect">The swap effect flags</param>
	/// <param name="flags">Additional swapchain flags</param>
	public SwapChainDescription( ModeDescription bufferDesc, SampleDescription sampleDesc, Usage bufferUsage,
		uint bufferCount, HWND outputWindow, bool windowed, SwapEffect swapEffect, SwapChainFlags flags ) {
		this.BufferDesc = bufferDesc;
		this.SampleDesc = sampleDesc;
		this.BufferUsage = bufferUsage;
		this.BufferCount = bufferCount;
		this.OutputWindow = outputWindow;
		this.Windowed = windowed;
		this.SwapEffect = swapEffect;
		this.Flags = flags;
	}

	//ModeDescription bufferDesc;
	//SampleDescription sampleDesc;
	//Usage bufferUsage;
	//uint bufferCount;
	//HWND outputWindow;
	//BOOL windowed;
	//SwapEffect swapEffect;
	//uint flags;

	// just wrap the internal type:
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	DXGI_SWAP_CHAIN_DESC desc;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	internal DXGI_SWAP_CHAIN_DESC InternalValue => desc;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXGI_MODE_DESC</a></b> A <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXGI_MODE_DESC</a> structure that describes the backbuffer display mode.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ModeDescription BufferDesc { get => desc.BufferDesc; set => desc.BufferDesc = value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a> structure that describes multi-sampling parameters.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public SampleDescription SampleDesc { get => desc.SampleDesc; set => desc.SampleDesc = value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a> enumerated type that describes the surface usage and CPU access options for the back buffer. The back buffer can be used for shader input or render-target output.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public Usage BufferUsage { get => (Usage)desc.BufferUsage; set => desc.BufferUsage = (DXGI_USAGE)value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A value that describes the number of buffers in the swap chain. When you call  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">IDXGIFactory::CreateSwapChain</a> to create a full-screen swap chain, you typically include the front buffer in this value. For more information about swap-chain buffers, see Remarks.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint BufferCount { get => desc.BufferCount; set => desc.BufferCount = value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a></b> An <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a> handle to the output window. This member must not be <b>NULL</b>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public HWND OutputWindow { get => desc.OutputWindow; set => desc.OutputWindow = value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">BOOL</a></b> A Boolean value that specifies whether the output is in windowed mode. <b>TRUE</b> if the output is in windowed mode; otherwise, <b>FALSE</b>. We recommend that you create a windowed swap chain and allow the end user to change the swap chain to full screen through <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-setfullscreenstate">IDXGISwapChain::SetFullscreenState</a>; that is, do not set this member to FALSE to force the swap chain to be full screen. However, if you create the swap chain as full screen, also provide the end user with a list of supported display modes through the <b>BufferDesc</b> member because a swap chain that is created with an unsupported display mode might cause the display to go black and prevent the end user from seeing anything. For more information about choosing windowed verses full screen, see <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">IDXGIFactory::CreateSwapChain</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public bool Windowed { get => desc.Windowed; set => desc.Windowed = value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT</a> enumerated type that describes options for handling the contents of the presentation buffer after presenting a surface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public SwapEffect SwapEffect { get => (SwapEffect)desc.SwapEffect; set => desc.SwapEffect = (DXGI_SWAP_EFFECT)value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG</a> enumerated type that describes options for swap-chain behavior.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public SwapChainFlags Flags { get => (SwapChainFlags)desc.Flags; set => desc.Flags = (uint)value; }
} ;



// -----------------------------------------------

#region SwapChainDescription1 Structure Layout
//ModeDescription bufferDesc;
//SampleDescription sampleDesc;
//Usage bufferUsage;
//uint bufferCount;
//HWND outputWindow;
//BOOL windowed;
//SwapEffect swapEffect;
//uint flags;
#endregion

// -----------------------------------------------

/// <summary>Describes a swap chain.</summary>
/// <remarks>
/// <para>This structure is used by the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-getdesc">GetDesc</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">CreateSwapChain</a> methods. In full-screen mode, there is a dedicated front buffer; in windowed mode, the desktop is the front buffer. If you create a swap chain with one buffer, specifying <b>DXGI_SWAP_EFFECT_SEQUENTIAL</b> does not cause the contents of the single buffer to be swapped with the front buffer. For performance information about flipping swap-chain buffers in full-screen application, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/d3d10-graphics-programming-guide-dxgi">Full-Screen Application Performance Hints</a>.</para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
[DebuggerDisplay("{this.ToString()}")]
public struct SwapChainDescription1 {
	internal SwapChainDescription1( in DXGI_SWAP_CHAIN_DESC1 desc ) => this.desc = desc ;
	internal unsafe SwapChainDescription1( DXGI_SWAP_CHAIN_DESC1* pDesc ) => desc = *pDesc;

	/// <summary>
	/// Creates a new SwapChainDescription1
	/// </summary>
	/// <param name="width">The buffer width</param>
	/// <param name="height">The buffer height</param>
	/// <param name="format">The DXGI buffer format</param>
	/// <param name="stereo">Will swapchain render in stereoscopic mode (e.g., VR mode?)</param>
	/// <param name="sampleDesc">The multisample settings description</param>
	/// <param name="bufferUsage">The buffer usage flags</param>
	/// <param name="bufferCount">Number of backbuffers to use</param>
	/// <param name="scaling">The scaling flags</param>
	/// <param name="swapEffect">The swap effect flags</param>
	/// <param name="alphaMode">The alpha blending settings description</param>
	/// <param name="flags">Additional swapchain flags</param>
	public SwapChainDescription1(
		uint width, uint height, Format format, bool stereo, SampleDescription sampleDesc,
		Usage bufferUsage, uint bufferCount, Scaling scaling, SwapEffect swapEffect,
		AlphaMode alphaMode, SwapChainFlags flags = default ) {
		this.Width = width;
		this.Height = height;
		this.Format = format;
		this.Stereo = stereo;
		this.SampleDesc = sampleDesc;
		this.BufferUsage = bufferUsage;
		this.BufferCount = bufferCount;
		this.Scaling = scaling;
		this.SwapEffect = swapEffect;
		this.AlphaMode = alphaMode;
		this.Flags = flags;
	}

	/// <summary>
	/// Creates a new SwapChainDescription1
	/// </summary>
	/// <param name="width">The buffer width</param>
	/// <param name="height">The buffer height</param>
	/// <param name="format">The DXGI buffer format</param>
	/// <param name="stereo">Will swapchain render in stereoscopic mode (e.g., VR mode?)</param>
	/// <param name="sampleDesc">The multisample settings description</param>
	/// <param name="bufferUsage">The buffer usage flags</param>
	/// <param name="bufferCount">Number of backbuffers to use</param>
	/// <param name="scaling">The scaling flags</param>
	/// <param name="swapEffect">The swap effect flags</param>
	/// <param name="alphaMode">The alpha blending settings description</param>
	/// <param name="flags">Additional swapchain flags</param>
	public SwapChainDescription1(
		int width, int height, Format format, bool stereo, SampleDescription sampleDesc,
		Usage bufferUsage, uint bufferCount, Scaling scaling, SwapEffect swapEffect,
		AlphaMode alphaMode, SwapChainFlags flags = default ) {
		this.Width = (uint)width;
		this.Height = (uint)height;
		this.Format = format;
		this.Stereo = stereo;
		this.SampleDesc = sampleDesc;
		this.BufferUsage = bufferUsage;
		this.BufferCount = bufferCount;
		this.Scaling = scaling;
		this.SwapEffect = swapEffect;
		this.AlphaMode = alphaMode;
		this.Flags = flags;
	}

	/// <summary>
	/// Creates a SwapChainDescription1 from a SwapChainDescription
	/// </summary>
	/// <param name="desc">A SwapChainDescription structure</param>
	/// <param name="scaling">Additional Scaling flags (default is None)</param>
	/// <param name="alphaMode">Additional AlphaMode flags (default is Straight)</param>
	public SwapChainDescription1( SwapChainDescription desc, Scaling scaling = Scaling.None,
		AlphaMode alphaMode = AlphaMode.Straight ) {
		this.Width = desc.BufferDesc.Width;
		this.Height = desc.BufferDesc.Height;
		this.Format = desc.BufferDesc.Format;
		this.Stereo = false;
		this.SampleDesc = desc.SampleDesc;
		this.BufferUsage = desc.BufferUsage;
		this.BufferCount = desc.BufferCount;
		this.Scaling = scaling;
		this.SwapEffect = desc.SwapEffect;
		this.AlphaMode = alphaMode;
		this.Flags = desc.Flags;
	}
	
	
	//! simply wrap up the internal struct type:
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	DXGI_SWAP_CHAIN_DESC1 desc;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	internal DXGI_SWAP_CHAIN_DESC1 _InternalValue => desc;


	/// <summary>
	/// A value that describes the resolution width. If you specify the width as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the width from the output window and assigns this width 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned width value.
	/// </summary>
	public uint Width { get => desc.Width; set => desc.Width = value; }

	/// <summary>
	/// A value that describes the resolution height. If you specify the height as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the height from the output window and assigns this height 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned height value.
	/// </summary>
	public uint Height { get => desc.Height; set => desc.Height = value; }

	/// <summary>
	/// A DXGI.Format structure describing the display format.
	/// </summary>
	public Format Format { get => (Format)desc.Format; set => desc.Format = (DXGI_FORMAT)value; }

	/// <summary>
	/// Specifies whether the full-screen display mode is stereo. True if stereo; otherwise, false.
	/// </summary>
	public bool Stereo { get => desc.Stereo; set => desc.Stereo = value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a> structure that describes multi-sampling parameters.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public SampleDescription SampleDesc { get => desc.SampleDesc; set => desc.SampleDesc = value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a> enumerated type that describes the surface usage and CPU access options for the back buffer. The back buffer can be used for shader input or render-target output.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public Usage BufferUsage { get => (Usage)desc.BufferUsage; set => desc.BufferUsage = (DXGI_USAGE)value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A value that describes the number of buffers in the swap chain. When you call  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">IDXGIFactory::CreateSwapChain</a> to create a full-screen swap chain, you typically include the front buffer in this value. For more information about swap-chain buffers, see Remarks.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint BufferCount { get => desc.BufferCount; set => desc.BufferCount = value; }

	/// <summary>
	/// <para>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_scaling">DXGI_SCALING</a>-typed value that identifies resize behavior if the size of the back buffer is not equal to the target output.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public Scaling Scaling { get => (Scaling)desc.Scaling; set => desc.Scaling = (DXGI_SCALING)value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT</a> enumerated type that describes options for handling the contents of the presentation buffer after presenting a surface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public SwapEffect SwapEffect { get => (SwapEffect)desc.SwapEffect; set => desc.SwapEffect = (DXGI_SWAP_EFFECT)value; }

	/// <summary>
	/// <para>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_alpha_mode">DXGI_ALPHA_MODE</a>-typed value that identifies the transparency behavior of the swap-chain back buffer.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	internal AlphaMode AlphaMode { get => (AlphaMode)desc.AlphaMode; set => desc.AlphaMode = (DXGI_ALPHA_MODE)value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG</a> enumerated type that describes options for swap-chain behavior.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public SwapChainFlags Flags { get => (SwapChainFlags)desc.Flags; set => 
			desc.Flags = (DXGI_SWAP_CHAIN_FLAG)(uint)value; }


	/// <summary>
	/// Converts this <see cref="SwapChainDescription1"/> structure to a string
	/// formatted in a JSON-like way, which is easy to save, load and parse ...
	/// </summary>
	/// <returns>This SwapChainDescription1 value as a formatted string.</returns>
	/// <remarks>May need to change ordering/naming a bit in the future (OK for now) ...</remarks>
	public override string ToString( ) =>
		$"\"SwapChainDescription1\": [\n" +
			$"\t\"Resolution\": \"{Width}x{Height}\", \"Format\": \"{Format}\", \n" +
			$"\t\"Buffers\": [ \"Count\": {BufferCount}, \"Usage\": {BufferUsage} ], \n" +
			$"\t\"SampleDesc\": [ \"Count\": {SampleDesc.Count}, \"Quality\": {SampleDesc.Quality} ], \n" +
			$"\t\"SwapEffect\": {SwapEffect}, \"Stereo\": {Stereo}, \"Scaling\": {Scaling}, \n" +
			$"\t\"AlphaMode\": {AlphaMode}, \"Flags\": {Flags} \n" +
		$"]" ;


	/// <summary>
	/// Converts a SwapChainDescription to a SwapChainDescription1 structure
	/// </summary>
	/// <param name="desc">A SwapChainDescription strcuture</param>
	public static explicit operator SwapChainDescription1( SwapChainDescription desc ) => new( desc );

} ;



//typedef struct DXGI_SWAP_CHAIN_FULLSCREEN_DESC
//{
//	DXGI_RATIONAL RefreshRate;
//	DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;
//	DXGI_MODE_SCALING Scaling;
//	BOOL Windowed;
//}
//DXGI_SWAP_CHAIN_FULLSCREEN_DESC;

/// <summary>
/// Describes full-screen mode for a swap chain.
/// </summary>
[DebuggerDisplay( "FullScreen Mode = Windowed: {Windowed} @ {RefreshRate.AsFloat}Hz ({Scaling})" )]
public struct SwapChainFullscreenDescription {
	internal SwapChainFullscreenDescription( in DXGI_SWAP_CHAIN_FULLSCREEN_DESC desc ) {
		//this.desc.RefreshRate = desc.RefreshRate;
		//this.desc.ScanlineOrdering = desc.ScanlineOrdering;
		//this.desc.Scaling = desc.Scaling;
		//this.desc.Windowed = desc.Windowed;
		this.desc = desc;
	}

	internal unsafe SwapChainFullscreenDescription( DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pDesc ) => this.desc = *pDesc;

	/// <summary>
	/// Creates a new SwapChainFullscreenDescription structure
	/// </summary>
	/// <param name="refreshRate">The refresh rate</param>
	/// <param name="scanlineOrdering">The scaline ordering flags</param>
	/// <param name="scalingMode">The scaling mode flags</param>
	/// <param name="windowed">Indicates if display mode is windowed</param>
	public SwapChainFullscreenDescription( Rational refreshRate, ScanlineOrder scanlineOrdering,
		ScalingMode scalingMode, bool windowed ) {
		this.desc.RefreshRate = refreshRate;
		this.desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)scanlineOrdering;
		this.desc.Scaling = (DXGI_MODE_SCALING)scalingMode;
		this.desc.Windowed = windowed;
	}



	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	DXGI_SWAP_CHAIN_FULLSCREEN_DESC desc;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	internal DXGI_SWAP_CHAIN_FULLSCREEN_DESC InternalValue => desc;



	/// <summary>
	/// A <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_rational?redirectedfrom=MSDN">DXGI.Rational</a> 
	/// structure describing the refresh rate in hertz
	/// </summary>
	public Rational RefreshRate { get => desc.RefreshRate; set => desc.RefreshRate = value; }

	/// <summary>
	/// A member of the DXGI.ScanlineOrdering enumerated type describing the scanline drawing mode.
	/// </summary>
	public ScanlineOrder ScanlineOrdering { get => (ScanlineOrder)desc.ScanlineOrdering; set => desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => (ScalingMode)desc.Scaling; set => desc.Scaling = (DXGI_MODE_SCALING)value; }

	/// <summary>
	/// A Boolean value that specifies whether the swap chain is in windowed mode. 
	/// TRUE if the swap chain is in windowed mode; otherwise, FALSE.
	/// </summary>
	public bool Windowed { get => desc.Windowed; set => desc.Windowed = value; }
};


//typedef struct DXGI_MODE_DESC
//{
//	UINT Width;
//	UINT Height;
//	DXGI_RATIONAL RefreshRate;
//	DXGI_FORMAT Format;
//	DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;
//	DXGI_MODE_SCALING Scaling;
//}
//DXGI_MODE_DESC;

/// <summary>
/// Describes a display mode.
/// </summary>
/// <remarks>
/// For more info see: <a href="https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXGI_MODE_DESC</a>
/// </remarks>
[DebuggerDisplay( "Mode = {Width}x{Height} @ {RefreshRate.AsFloat}Hz ({Scaling})" )]
public struct ModeDescription {
	internal ModeDescription( in DXGI_MODE_DESC modeDesc ) => this.desc = modeDesc;

	internal unsafe ModeDescription( DXGI_MODE_DESC* pModeDesc ) => this.desc = *pModeDesc;

	/// <summary>
	/// Creates a new DXGI.ModeDescription
	/// </summary>
	/// <param name="width">Width of the display mode</param>
	/// <param name="height">Height of the display mode</param>
	/// <param name="refreshRate">Refresh rate of the monitor</param>
	/// <param name="format">The resource format of the display mode</param>
	/// <param name="scanlineOrdering">The scanline ordering of the display mode</param>
	/// <param name="scaling">The scaling of the display mode</param>
	public ModeDescription( uint width, uint height, Rational refreshRate, Format format = Format.R8G8B8A8_UNORM,
		ScanlineOrder scanlineOrdering = ScanlineOrder.Unspecified, ScalingMode scaling = ScalingMode.Centered ) =>
		this.desc = new DXGI_MODE_DESC( width, height, refreshRate, format, scanlineOrdering, scaling );

	/// <summary>
	/// Creates a new ModeDescription out of a ModeDescription1
	/// </summary>
	/// <param name="modeDesc1">A ModeDescription1 structure</param>
	public ModeDescription( in ModeDescription1 modeDesc1 ) {
		unsafe {
			fixed( ModeDescription1* pData = (&modeDesc1) )
				this.desc = *((DXGI_MODE_DESC*)pData);
		}
	}



	//uint width;
	//uint height;
	//Rational refreshRate;
	//Format format;
	//ScanlineOrder scanlineOrdering;
	//ScalingMode scaling;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )] DXGI_MODE_DESC desc ;



	/// <summary>
	/// A value that describes the resolution width. If you specify the width as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the width from the output window and assigns this width 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned width value.
	/// </summary>
	public uint Width { get => desc.Width; set => desc.Width = value; }

	/// <summary>
	/// A value that describes the resolution height. If you specify the height as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the height from the output window and assigns this height 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned height value.
	/// </summary>
	public uint Height { get => desc.Height; set => desc.Height = value; }

	/// <summary>
	/// A <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_rational?redirectedfrom=MSDN">DXGI.Rational</a> 
	/// structure describing the refresh rate in hertz
	/// </summary>
	public Rational RefreshRate { get => desc.RefreshRate; set => desc.RefreshRate = value; }

	/// <summary>
	/// A DXGI.Format structure describing the display format.
	/// </summary>
	public Format Format { get => (Format)desc.Format; set => desc.Format = (DXGI_FORMAT)value; }

	/// <summary>
	/// A member of the DXGI.ScanlineOrder enumerated type describing the scanline drawing mode.
	/// </summary>
	public ScanlineOrder ScanlineOrdering { get => (ScanlineOrder)desc.ScanlineOrdering; set => desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => (ScalingMode)desc.Scaling; set => desc.Scaling = (DXGI_MODE_SCALING)value; }
	
	public static explicit operator ModeDescription( ModeDescription1 mode ) => new( mode );
} ;


//typedef struct DXGI_MODE_DESC1
//{
//	UINT Width;
//	UINT Height;
//	DXGI_RATIONAL RefreshRate;
//	DXGI_FORMAT Format;
//	DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;
//	DXGI_MODE_SCALING Scaling;
//	BOOL Stereo;
//}
//DXGI_MODE_DESC1;

/// <summary>
/// Describes a display mode and whether the display mode supports stereo.
/// </summary>
/// <remarks>
/// ModeDescription1 is identical to ModeDescription except that ModeDescription1 includes the Stereo member.
/// </remarks>
[DebuggerDisplay( "Mode = {Width}x{Height} @ {RefreshRate.AsFloat}Hz ({Scaling})" )]
public struct ModeDescription1 {
	internal ModeDescription1( in DXGI_MODE_DESC1 modeDesc ) => this.desc = modeDesc;

	internal unsafe ModeDescription1( DXGI_MODE_DESC1* pModeDesc ) => desc = *pModeDesc;

	/// <summary>
	/// Creates a new DXGI.ModeDescription
	/// </summary>
	/// <param name="width">Width of the display mode</param>
	/// <param name="height">Height of the display mode</param>
	/// <param name="refreshRate">Refresh rate of the monitor</param>
	/// <param name="format">The resource format of the display mode</param>
	/// <param name="scanlineOrdering">The scanline ordering of the display mode</param>
	/// <param name="scaling">The scaling of the display mode</param>
	/// <param name="stereo">Indicates if rendering in stereo (e.g., for VR/Mixed Reality) mode</param>
	public ModeDescription1( uint width, uint height, Rational refreshRate, Format format = Format.R8G8B8A8_UNORM,
		ScanlineOrder scanlineOrdering = ScanlineOrder.Unspecified, ScalingMode scaling = ScalingMode.Centered, bool stereo = false ) {
		//this.desc.Width = width;
		//this.desc.Height = height;
		//this.desc.RefreshRate = refreshRate;
		//this.desc.Format = (DXGI_FORMAT)format;
		//this.desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)scanlineOrdering;
		//this.desc.Scaling = (DXGI_MODE_SCALING)scaling;
		//this.desc.Stereo = stereo;

		this.desc = new DXGI_MODE_DESC1( width, height, refreshRate,
			format, scanlineOrdering, scaling, stereo );
	}

	/// <summary>
	/// Creates a new ModeDescription1 from a ModeDescription and optional stereo (bool) value
	/// </summary>
	/// <param name="desc">A ModeDescription structure</param>
	/// <param name="stereo">Indicates if rendering in stereo (e.g., for VR/Mixed Reality) mode</param>
	public ModeDescription1( ModeDescription desc, bool stereo = false ) {
		this.desc = desc;
		this.desc.Stereo = stereo;
	}

	//uint width;
	//uint height;
	//Rational refreshRate;
	//Format format;
	//ScanlineOrder scanlineOrdering;
	//ScalingMode scaling;
	//bool stereo;
	DXGI_MODE_DESC1 desc;


	/// <summary>
	/// A value that describes the resolution width. If you specify the width as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the width from the output window and assigns this width 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned width value.
	/// </summary>
	public uint Width { get => desc.Width; set => desc.Width = value; }

	/// <summary>
	/// A value that describes the resolution height. If you specify the height as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the height from the output window and assigns this height 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned height value.
	/// </summary>
	public uint Height { get => desc.Height; set => desc.Height = value; }

	/// <summary>
	/// A <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_rational?redirectedfrom=MSDN">DXGI.Rational</a> 
	/// structure describing the refresh rate in hertz
	/// </summary>
	public Rational RefreshRate { get => desc.RefreshRate; set => desc.RefreshRate = value; }

	/// <summary>
	/// A DXGI.Format structure describing the display format.
	/// </summary>
	public Format Format { get => (Format)desc.Format; set => desc.Format = (DXGI_FORMAT)value; }

	/// <summary>
	/// A member of the DXGI.ScanlineOrder enumerated type describing the scanline drawing mode.
	/// </summary>
	public ScanlineOrder ScanlineOrdering { get => (ScanlineOrder)desc.ScanlineOrdering; set => desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => (ScalingMode)desc.Scaling; set => desc.Scaling = (DXGI_MODE_SCALING)value; }

	/// <summary>
	/// Specifies whether the full-screen display mode is stereo. True if stereo; otherwise, false.
	/// </summary>
	public bool Stereo { get => desc.Stereo; set => desc.Stereo = value; }



	/// <summary>
	/// Converts a ModeDescription to a ModeDescription1 structure
	/// </summary>
	/// <param name="desc">A ModeDescription structure</param>
	public static implicit operator ModeDescription1( ModeDescription desc ) => new( desc );


} ;



[StructLayout( LayoutKind.Sequential )]
public struct OutputDescription {
	[MarshalAs(UnmanagedType.LPStr, SizeConst = 32)]
	public NativeStr32 DeviceName ;
	
	public Rect        DesktopCoordinates ;
	public bool        AttachedToDesktop ;
	public Rotation    Rotation ;
	public Win32Handle Monitor ;
	
	public OutputDescription( in OutputDescription desc ) {
		unsafe { fixed( OutputDescription* pThis = &this )
				*( (OutputDescription*)pThis ) = desc ; }
	}
	
	public OutputDescription( in NativeStr32 deviceName, 
							  in Rect desktopCoordinates, 
							  bool attachedToDesktop, 
							  Rotation rotation, 
							  nint monitor ) {
		this.DeviceName = deviceName ;
		this.DesktopCoordinates = desktopCoordinates ;
		this.AttachedToDesktop = attachedToDesktop ;
		this.Rotation = rotation ;
		this.Monitor = monitor ;
	}
	
	public OutputDescription( in NativeStr32 deviceName, 
							  in Rect desktopCoordinates, 
							  bool attachedToDesktop, 
							  Rotation rotation, 
							  Win32Handle monitor ) {
		this.DeviceName = deviceName ;
		this.DesktopCoordinates = desktopCoordinates ;
		this.AttachedToDesktop = attachedToDesktop ;
		this.Rotation = rotation ;
		this.Monitor = monitor ;
	}
	
	public OutputDescription( in DXGI_OUTPUT_DESC desc) {
		this.DeviceName         = new( desc.DeviceName ) ;
		this.DesktopCoordinates = desc.DesktopCoordinates ;
		this.AttachedToDesktop  = desc.AttachedToDesktop ;
		this.Rotation           = (Rotation)desc.Rotation ;
		this.Monitor            = desc.Monitor ;
	}
	
	public static implicit operator OutputDescription( in DXGI_OUTPUT_DESC desc ) => new( desc ) ;
	public static implicit operator DXGI_OUTPUT_DESC( in OutputDescription desc ) {
		DXGI_OUTPUT_DESC d = default ;
		d.DeviceName         = desc.DeviceName.ToString( ) ;
		d.DesktopCoordinates = desc.DesktopCoordinates ;
		d.AttachedToDesktop  = desc.AttachedToDesktop ;
		d.Rotation           = (DXGI_MODE_ROTATION)desc.Rotation ;
		d.Monitor            = desc.Monitor ;
		return d ;
	}
} ;



[StructLayout( LayoutKind.Sequential )]
public struct FrameStatistics {
	public uint PresentCount, PresentRefreshCount, SyncRefreshCount ;
	public long SyncQPCTime, SyncGPUTime ;
	
	public FrameStatistics( in DXGI_FRAME_STATISTICS stats ) {
		this.PresentCount       = stats.PresentCount ;
		this.PresentRefreshCount = stats.PresentRefreshCount ;
		this.SyncRefreshCount   = stats.SyncRefreshCount ;
		this.SyncQPCTime        = stats.SyncQPCTime ;
		this.SyncGPUTime        = stats.SyncGPUTime ;
	}
	
	public static implicit operator FrameStatistics( in DXGI_FRAME_STATISTICS stats ) => new( stats ) ;
	public static implicit operator DXGI_FRAME_STATISTICS( in FrameStatistics stats ) {
		 DXGI_FRAME_STATISTICS s = default ;
		 s.PresentCount       = stats.PresentCount ;
		 s.PresentRefreshCount = stats.PresentRefreshCount ;
		 s.SyncRefreshCount   = stats.SyncRefreshCount ;
		 s.SyncQPCTime        = stats.SyncQPCTime ;
		 s.SyncGPUTime        = stats.SyncGPUTime ;
		 return s ;
	 }
} ;

public enum Rotation {
	Identity  = 1,
	Rotate90  = 2,
	Rotate180 = 3,
	Rotate270 = 4,
} ;


[StructLayout( LayoutKind.Sequential )]
public struct AdapterDescription {
	public NativeStr128 Description; // A string that contains the adapter description.
	public uint VendorId; // The PCI ID of the hardware vendor.
	public uint DeviceId; // The PCI ID of the hardware device.
	public uint SubSysId; // The PCI ID of the sub system.
	public uint Revision; // The PCI ID of the revision number of the adapter.
	public ulong DedicatedVideoMemory; // The amount of video memory that is not shared with the CPU.
	public ulong DedicatedSystemMemory; // The amount of system memory that is not shared with the CPU.
	public ulong SharedSystemMemory; // The amount of system memory that is shared with the CPU.
	public Luid AdapterLuid; // A unique value that identifies the adapter.
	public uint Flags; // Identifies the adapter type. The value is a bitwise OR of DXGI_ADAPTER_FLAG enumeration constants.
	
	public AdapterDescription( in AdapterDescription desc ) {
		this.Description = desc.Description ;
		this.VendorId = desc.VendorId ;
		this.DeviceId = desc.DeviceId ;
		this.SubSysId = desc.SubSysId ;
		this.Revision = desc.Revision ;
		this.DedicatedVideoMemory = desc.DedicatedVideoMemory ;
		this.DedicatedSystemMemory = desc.DedicatedSystemMemory ;
		this.SharedSystemMemory = desc.SharedSystemMemory ;
		this.AdapterLuid = desc.AdapterLuid ;
		this.Flags = desc.Flags ;
	}
	
	public AdapterDescription( in DXGI_ADAPTER_DESC desc ) {
		this.Description           = new( desc.Description ) ;
		this.VendorId              = desc.VendorId ;
		this.DeviceId              = desc.DeviceId ;
		this.SubSysId              = desc.SubSysId ;
		this.Revision              = desc.Revision ;
		this.DedicatedVideoMemory  = desc.DedicatedVideoMemory ;
		this.DedicatedSystemMemory = desc.DedicatedSystemMemory ;
		this.SharedSystemMemory    = desc.SharedSystemMemory ;
		this.AdapterLuid           = new( desc.AdapterLuid ) ;
		this.Flags                 = default ;
	}
	
	public static implicit operator AdapterDescription( in DXGI_ADAPTER_DESC desc ) => new( desc ) ;
	public static implicit operator DXGI_ADAPTER_DESC( in AdapterDescription desc ) {
		DXGI_ADAPTER_DESC d = default ;
		d.Description           = desc.Description.ToString( ) ;
		d.VendorId              = desc.VendorId ;
		d.DeviceId              = desc.DeviceId ;
		d.SubSysId              = desc.SubSysId ;
		d.Revision              = desc.Revision ;
		d.DedicatedVideoMemory  = (nuint)desc.DedicatedVideoMemory ;
		d.DedicatedSystemMemory = (nuint)desc.DedicatedSystemMemory ;
		d.SharedSystemMemory    = (nuint)desc.SharedSystemMemory ;
		d.AdapterLuid           = desc.AdapterLuid ;
		//d.Flags                 = (uint)desc.Flags ;
		#warning "DXGI_ADAPTER_DESC" doesn't have a "Flags", that is in DXGI_ADAPTER_DESC1+ ...
		return d ;
	}
} ;



[Serializable,
 DebuggerDisplay("LUID: {LowPart}:{HighPart}"),
 StructLayout(LayoutKind.Sequential)]
public struct Luid: IEquatable<Luid> {
	public uint LowPart ; public int HighPart ;
	public Luid( uint low, int high ) { LowPart = low ; HighPart = high ; }
	public Luid( in LUID luid ) { LowPart = luid.LowPart ; HighPart = luid.HighPart ; }
	public unsafe Luid( LUID* pLuid ) { LowPart = pLuid->LowPart ; HighPart = pLuid->HighPart ; }
	public static implicit operator Luid( in LUID luid ) => new( luid ) ;
	public static implicit operator LUID( in Luid luid ) => new LUID {
		LowPart = luid.LowPart, HighPart = luid.HighPart
	} ;
	
	public static bool operator ==( in Luid a, in Luid b ) => 
		a.LowPart == b.LowPart && a.HighPart == b.HighPart ;
	public static bool operator !=( in Luid a, in Luid b ) =>
	 		a.LowPart != b.LowPart || a.HighPart != b.HighPart ;
	
	public override int GetHashCode( ) => HashCode.Combine( LowPart, HighPart ) ;
	public override bool Equals( object? obj ) => obj is Luid luid && this == luid ;
	public override string ToString( ) => $"LUID: {{ {LowPart}, {HighPart} }}" ;
	public bool Equals( Luid other ) => LowPart == other.LowPart 
										&& HighPart == other.HighPart ;
} ;


public struct SharedResource {
	public Win32Handle Handle ;
	public SharedResource( Win32Handle handle ) => Handle = handle ;
	public static implicit operator SharedResource( Win32Handle handle ) => new( handle ) ;
	public static implicit operator Win32Handle( SharedResource handle ) => handle.Handle ;
	public static implicit operator DXGI_SHARED_RESOURCE( SharedResource res ) => 
		new( ) { Handle = res.Handle } ;
} ;


/// <summary>
/// Represents flags that can be used with the DXGI present operations.
/// </summary>
/// <remarks>
/// These flags specify how the presentation will behave in various situations.
/// For detailed information, see the
/// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/dxgi-present">
/// official documentation
/// </a>.
/// </remarks>
[Flags] public enum PresentFlags: uint {
	Default = 0,                             // Present a frame from each buffer (starting with the current buffer) to the output.
	DoNotSequence = 0x00000002,              // Present a frame from the current buffer to the output.
	Test = 0x00000001,                       // Do not present the frame to the output.
	Restart = 0x00000004,                    // Specifies that the runtime will discard outstanding queued presents.
	DoNotWait = 0x00000008,                  // Specifies that the runtime will fail the presentation.
	RestrictToOutput = 0x00000010,           // Indicates that presentation content will be shown only on the particular output.
	StereoPreferRight = 0x00000020,          // Indicates that if the stereo present must be reduced to mono, right-eye viewing is used.
	StereoTemporaryMono = 0x00000040,        // Indicates that the presentation should use the left buffer as a mono buffer.
	UseDuration = 0x00000100,                // This flag must be set by media apps that are currently using a custom present duration.
	AllowTearing = 0x00000200                // This value is supported starting in Windows 8.1.
} ;


public struct PresentParameters {
	/// <summary>
	/// The number of updated rectangles that you update in the
	/// back buffer for the presented frame. The operating system
	/// uses this information to optimize presentation. You can set
	/// this member to 0 to indicate that you update the whole frame.
	/// </summary>
	public uint DirtyRectsCount ;

	/// <summary>
	/// A list of updated rectangles that you update in the back buffer
	/// for the presented frame. An application must update every single
	/// pixel in each rectangle that it reports to the runtime; the
	/// application cannot assume that the pixels are saved from the
	/// previous frame. For more information about updating dirty
	/// rectangles, see Remarks. You can set this member to <b>NULL</b>
	/// if <b>DirtyRectsCount</b> is 0. An application must not update
	/// any pixel outside of the dirty rectangles.
	/// </summary>
	public unsafe Rect* pDirtyRects ;

	/// <summary>
	/// <para>A pointer to the scrolled rectangle. The scrolled rectangle is the
	/// rectangle of the previous frame from which the runtime bit-block transfers
	/// (bitblts) content. The runtime also uses the scrolled rectangle to optimize
	/// presentation in terminal server and indirect display scenarios. The scrolled
	/// rectangle also describes the destination rectangle, that is, the region on the
	/// current frame that is filled with scrolled content. You can set this member to
	/// <b>NULL</b> to indicate that no content is scrolled from the previous frame.</para>
	/// <para>
	/// <see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ns-dxgi1_2-dxgi_present_parameters#members">
	/// Read more on docs.microsoft.com</see>.
	/// </para>
	/// </summary>
	public unsafe Rect* pScrollRect ;

	/// <summary>
	/// A pointer to the offset of the scrolled area that goes from the source rectangle
	/// (of previous frame) to the destination rectangle (of current frame). You can set
	/// this member to <b>NULL</b> to indicate no offset.
	/// </summary>
	public unsafe Point* pScrollOffset ;
	
	
	public unsafe PresentParameters( uint dirtyRectsCount = 0,
										  Rect* pDirtyRects = null,
										  Rect* pScrollRect = null, 
										  Point* pScrollOffset = null ) {
		this.pDirtyRects     = pDirtyRects ;
		this.pScrollRect     = pScrollRect ;
		this.pScrollOffset   = pScrollOffset ;
		this.DirtyRectsCount = dirtyRectsCount ;
	}
	
	public unsafe PresentParameters( DXGI_PRESENT_PARAMETERS parameters ) {
		this.pDirtyRects     = (Rect *)parameters.pDirtyRects ;
		this.pScrollRect     = (Rect *)parameters.pScrollRect ;
		this.DirtyRectsCount = parameters.DirtyRectsCount ;
		this.pScrollOffset   = parameters.pScrollOffset ;
	}
	
	public static implicit operator PresentParameters( in DXGI_PRESENT_PARAMETERS parameters ) => new( parameters ) ;
	public static unsafe implicit operator DXGI_PRESENT_PARAMETERS( in PresentParameters parameters ) => new( ) {
			pDirtyRects     = (RECT *)parameters.pDirtyRects,
			pScrollRect     = (RECT *)parameters.pScrollRect,
			DirtyRectsCount = parameters.DirtyRectsCount,
			pScrollOffset   = parameters.pScrollOffset
		} ;
} ;



public enum ModeRotation {
	Unspecified = 0,
	Identity    = 1,
	Rotate90    = 2,
	Rotate180   = 3,
	Rotate270   = 4
} ;



[StructLayout( LayoutKind.Sequential )]
public struct RGBA {
	public float R, G, B, A ;
	public RGBA(float r, float g, float b, float a) {
		R = r; G = g; B = b; A = a;
	}
	
	public static implicit operator DXGI_RGBA(RGBA color) => new DXGI_RGBA {
			r = color.R, g = color.G, b = color.B, a = color.A
	} ;
	public static implicit operator RGBA(DXGI_RGBA color) => new RGBA(color.r, color.g, color.b, color.a);
} ;



public enum ColorSpaceType: uint {
	RGBFullG22NoneP709           = 0,
	RGBFullG10NoneP709           = 1,
	RGBStudioG22NoneP709         = 2,
	RGBStudioG22NoneP2020        = 3,
	Reserved                     = 4,
	YCbCrFullG22NoneP709X601     = 5,
	YCbCrStudioG22LeftP601       = 6,
	YCbCrFullG22LeftP601         = 7,
	YCbCrStudioG22LeftP709       = 8,
	YCbCrFullG22LeftP709         = 9,
	YCbCrStudioG22LeftP2020      = 10,
	YCbCrFullG22LeftP2020        = 11,
	RGBFullG2084NoneP2020        = 12,
	YCbCrStudioG2084LeftP2020    = 13,
	RGBStudioG2084NoneP2020      = 14,
	YCbCrStudioG22TopLeftP2020   = 15,
	YCbCrStudioG2084TopLeftP2020 = 16,
	RGBFullG22NoneP2020          = 17,
	YCbCrStudioGHlgTopLeftP2020  = 18,
	YCbCrFullGHlgTopLeftP2020    = 19,
	RGBStudioG24NoneP709         = 20,
	RGBStudioG24NoneP2020        = 21,
	YCbCrStudioG24LeftP709       = 22,
	YCbCrStudioG24LeftP2020      = 23,
	YCbCrStudioG24TopLeftP2020   = 24,
	Custom                       = 0xFFFFFFFF,
} ;



// ----------------------------------------------------
// Output Duplication Data Structures:
// ----------------------------------------------------

[StructLayout( LayoutKind.Sequential )]
public struct OutputDuplicationDescription {
	/// <summary>A <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXGI_MODE_DESC</a> structure that describes the display mode of the duplicated output.</summary>
	public ModeDescription ModeDescription ;
	/// <summary>A member of the <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173065(v=vs.85)">DXGI_MODE_ROTATION</a> enumerated type that describes how the duplicated output rotates an image.</summary>
	public ModeRotation Rotation ;
	/// <summary>Specifies whether the resource that contains the desktop image is already located in system memory. <b>TRUE</b> if the resource is in system memory; otherwise, <b>FALSE</b>. If this value is <b>TRUE</b> and  the application requires CPU access, it can use the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-mapdesktopsurface">IDXGIOutputDuplication::MapDesktopSurface</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-unmapdesktopsurface">IDXGIOutputDuplication::UnMapDesktopSurface</a> methods to avoid copying the data into a staging buffer.</summary>
	public bool DesktopImageInSystemMemory ;
	
	public OutputDuplicationDescription( in DXGI_OUTDUPL_DESC desc ) {
		this.ModeDescription = desc.ModeDesc ;
		this.Rotation = (ModeRotation)desc.Rotation ;
		this.DesktopImageInSystemMemory = desc.DesktopImageInSystemMemory ;
	}
	
	public static implicit operator OutputDuplicationDescription( in DXGI_OUTDUPL_DESC desc ) => new( desc ) ;
	public static implicit operator DXGI_OUTDUPL_DESC( in OutputDuplicationDescription desc ) => new( ) {
		ModeDesc = desc.ModeDescription,
		Rotation = (DXGI_MODE_ROTATION)desc.Rotation,
		DesktopImageInSystemMemory = desc.DesktopImageInSystemMemory
	} ;
} ;


[StructLayout( LayoutKind.Sequential )]
public struct OutputDuplicationPointerPosition {
	/// <summary>
	/// The position of the hardware cursor relative to the top-left of the adapter output.
	/// </summary>
	public Point Position ;

	/// <summary>
	/// Specifies whether the hardware cursor is visible. <b>TRUE</b> if visible; otherwise, <b>FALSE</b>.
	/// If the hardware cursor is not visible, the calling application does not display the cursor in the client.
	/// </summary>
	public bool Visible {
		get => _visible != 0 ;
		set => _visible = value ? 1 : 0 ;
	}
	internal BOOL _visible ;

	public OutputDuplicationPointerPosition( in DXGI_OUTDUPL_POINTER_POSITION pos ) {
		this.Position = pos.Position ;
		this._visible = pos.Visible ;
	}

	public static implicit operator OutputDuplicationPointerPosition( in DXGI_OUTDUPL_POINTER_POSITION pos ) => new( pos ) ;
	public static implicit operator DXGI_OUTDUPL_POINTER_POSITION( in OutputDuplicationPointerPosition pos ) => new( ) {
		Position = pos.Position,
		Visible  = pos.Visible
	} ;
} ;


[StructLayout(LayoutKind.Sequential)]
public struct OutputDuplicationFrameInfo {
	/// <summary>
	/// <para>The time stamp of the last update of the desktop image.  The operating system calls the <a href="https://docs.microsoft.com/windows/desktop/api/profileapi/nf-profileapi-queryperformancecounter">QueryPerformanceCounter</a> function to obtain the value. A zero value indicates that the desktop image was not updated since an application last called the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-acquirenextframe">IDXGIOutputDuplication::AcquireNextFrame</a> method to acquire the next frame of the desktop image.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_frame_info#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public long LastPresentTime ;

	/// <summary>
	/// <para>The time stamp of the last update to the mouse.  The operating system calls the <a href="https://docs.microsoft.com/windows/desktop/api/profileapi/nf-profileapi-queryperformancecounter">QueryPerformanceCounter</a> function to obtain the value. A zero value indicates that the position or shape of the mouse was not updated since an application last called the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-acquirenextframe">IDXGIOutputDuplication::AcquireNextFrame</a> method to acquire the next frame of the desktop image.  The mouse position is always supplied for a mouse update. A new pointer shape is indicated by a non-zero value in the <b>PointerShapeBufferSize</b> member.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_frame_info#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public long LastMouseUpdateTime ;

	/// <summary>
	/// The number of frames that the operating system accumulated in the desktop image surface since
	/// the calling application processed the last desktop image.  For more information about this
	/// number, see Remarks.
	/// </summary>
	public uint AccumulatedFrames ;

	/// <summary>
	/// Specifies whether the operating system accumulated updates by coalescing dirty regions.
	/// Therefore, the dirty regions might contain unmodified pixels. <b>TRUE</b> if dirty regions
	/// were accumulated; otherwise, <b>FALSE</b>.
	/// </summary>
	public bool RectsCoalesced { get => _rectsCoalesced != 0 ; set => _rectsCoalesced = value ? 1 : 0 ; }
	internal BOOL _rectsCoalesced ;

	/// <summary>
	/// Specifies whether the desktop image might contain protected content that was already
	/// blacked out in the desktop image.  <b>TRUE</b> if protected content was already blacked;
	/// otherwise, <b>FALSE</b>. The application can use this information to notify the remote
	/// user that some of the desktop content might be protected and therefore not visible.
	/// </summary>
	public bool ProtectedContentMaskedOut { get => _protectedContentMaskedOut != 0 ; set => _protectedContentMaskedOut = value ? 1 : 0 ; }
	internal BOOL _protectedContentMaskedOut ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_pointer_position">
	/// DXGI_OUTDUPL_POINTER_POSITION
	/// </a>
	/// structure that describes the most recent mouse position if the <b>LastMouseUpdateTime</b> member is a non-zero value;
	/// otherwise, this value is ignored. This value provides the coordinates of the location where the top-left-hand corner
	/// of the pointer shape is drawn; this value is not the desktop position of the hot spot.
	/// </summary>
	public OutputDuplicationPointerPosition PointerPosition ;

	/// <summary>
	/// Size in bytes of the buffers to store all the desktop update metadata for this frame.
	/// For more information about this size, see Remarks.
	/// </summary>
	public uint TotalMetadataBufferSize ;

	/// <summary>
	/// Size in bytes of the buffer to hold the new pixel data for the mouse shape.
	/// For more information about this size, see Remarks.
	/// </summary>
	public uint PointerShapeBufferSize ;
	
	public OutputDuplicationFrameInfo( in DXGI_OUTDUPL_FRAME_INFO info ) {
		this.LastPresentTime = info.LastPresentTime ;
		this.LastMouseUpdateTime = info.LastMouseUpdateTime ;
		this.AccumulatedFrames = info.AccumulatedFrames ;
		this._rectsCoalesced = info.RectsCoalesced ;
		this._protectedContentMaskedOut = info.ProtectedContentMaskedOut ;
		this.PointerPosition = info.PointerPosition ;
		this.TotalMetadataBufferSize = info.TotalMetadataBufferSize ;
		this.PointerShapeBufferSize = info.PointerShapeBufferSize ;
	}
	
	public static implicit operator OutputDuplicationFrameInfo( in DXGI_OUTDUPL_FRAME_INFO info ) => new( info ) ;
	public static implicit operator DXGI_OUTDUPL_FRAME_INFO( in OutputDuplicationFrameInfo info ) => new( ) {
		LastPresentTime = info.LastPresentTime,
		LastMouseUpdateTime = info.LastMouseUpdateTime,
		AccumulatedFrames = info.AccumulatedFrames,
		RectsCoalesced = info.RectsCoalesced,
		ProtectedContentMaskedOut = info.ProtectedContentMaskedOut,
		PointerPosition = info.PointerPosition,
		TotalMetadataBufferSize = info.TotalMetadataBufferSize,
		PointerShapeBufferSize = info.PointerShapeBufferSize
	} ;
} ;


[StructLayout(LayoutKind.Sequential)]
public struct OutputDuplicationMoveRect {
	/// <summary>The starting position of a rectangle.</summary>
	public Point SourcePoint ;

	/// <summary>The target region to which to move a rectangle.</summary>
	public Rect DestinationRect ;
	
	public OutputDuplicationMoveRect( Point srcPoint, Rect dstRect ) {
		this.SourcePoint = srcPoint ;
		this.DestinationRect = dstRect ;
	}
	public OutputDuplicationMoveRect( in DXGI_OUTDUPL_MOVE_RECT rect ) {
		this.SourcePoint = rect.SourcePoint ;
		this.DestinationRect = rect.DestinationRect ;
	}
	
	public static implicit operator OutputDuplicationMoveRect( in DXGI_OUTDUPL_MOVE_RECT rect ) => new( rect ) ;
	public static implicit operator DXGI_OUTDUPL_MOVE_RECT( in OutputDuplicationMoveRect rect ) => new( ) {
		SourcePoint = rect.SourcePoint,
		DestinationRect = rect.DestinationRect
	} ;
} ;


[StructLayout(LayoutKind.Sequential)]
public struct OutputDuplicationPointerShapeInfo {
	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_outdupl_pointer_shape_type">
	/// DXGI_OUTDUPL_POINTER_SHAPE_TYPE</a>-typed value that specifies the type of cursor shape.
	/// </summary>
	public uint Type ;

	/// <summary>The width in pixels of the mouse cursor.</summary>
	public uint Width ;

	/// <summary>The height in scan lines of the mouse cursor.</summary>
	public uint Height ;

	/// <summary>The width in bytes of the mouse cursor.</summary>
	public uint Pitch ;

	/// <summary>
	/// The position of the cursor's hot spot relative to its upper-left pixel.
	/// An application does not use the hot spot when it determines where to
	/// draw the cursor shape.
	/// </summary>
	public Point HotSpot ;
	
	public OutputDuplicationPointerShapeInfo( uint type, uint width, uint height, uint pitch, Point hotSpot ) {
		this.Type = type ;
		this.Width = width ;
		this.Height = height ;
		this.Pitch = pitch ;
		this.HotSpot = hotSpot ;
	}
	internal OutputDuplicationPointerShapeInfo( in DXGI_OUTDUPL_POINTER_SHAPE_INFO info ) {
		this.Type = info.Type ;
		this.Width = info.Width ;
		this.Height = info.Height ;
		this.Pitch = info.Pitch ;
		this.HotSpot = info.HotSpot ;
	}
	
	public static implicit operator OutputDuplicationPointerShapeInfo( in DXGI_OUTDUPL_POINTER_SHAPE_INFO info ) => new( info ) ;
	public static implicit operator DXGI_OUTDUPL_POINTER_SHAPE_INFO( in OutputDuplicationPointerShapeInfo info ) => new( ) {
		Type = info.Type,
		Width = info.Width,
		Height = info.Height,
		Pitch = info.Pitch,
		HotSpot = info.HotSpot
	} ;
}


// ====================================================
