#region Using Directives
using global::System;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;

using Windows.Win32.Foundation;

using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D11;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;
using Windows.Win32.Graphics.Dxgi.Common;

using global::Windows.Win32;
using Win32 = global::Windows.Win32;

using DXGI_MODE_DESC = Windows.Win32.Graphics.Dxgi.Common.DXGI_MODE_DESC;
using DXGI_MODE_DESC1 = Windows.Win32.Graphics.Dxgi.DXGI_MODE_DESC1;

using DXGI_SWAP_EFFECT = Windows.Win32.Graphics.Dxgi.DXGI_SWAP_EFFECT;
using DXGI_SAMPLE_DESC = Windows.Win32.Graphics.Dxgi.Common.DXGI_SAMPLE_DESC;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

#endregion

namespace DXSharp.DXGI;

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
public enum AlphaMode: uint
{
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
public enum ScanlineOrder
{
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
public enum Scaling
{
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
public enum ScalingMode
{
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
public enum Usage: long
{
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
	DiscardOnPresent_internal	= 1L << (5 + 4),
	/// <summary>
	/// Use the surface or resource for reading only.
	/// </summary>
	ReadOnly					= 1L << (4 + 4),
	/// <summary>
	/// Use the surface or resource as an output render target.
	/// </summary>
	RenderTargetOutput			= 1L << (1 + 4),
	/// <summary>
	/// Use the surface or resource as an input to a shader.
	/// </summary>
	ShaderInput					= 1L << (0 + 4),
	/// <summary>
	/// Share the surface or resource.
	/// </summary>
	Shared						= 1L << (3 + 4),
	/// <summary>
	/// Use the surface or resource for unordered access.
	/// </summary>
	UnorderedAccess				= 1L << (6 + 4),
};

/// <summary>Options for handling pixels in a display surface after calling DXGI.ISwapChain1.Present1.</summary>
/// <remarks>
/// <para>
/// <see href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#">Read more on docs.microsoft.com</see>.
/// </para>
/// </remarks>
public enum SwapEffect
{
	/// <summary>
	/// <para>Use this flag to specify the bit-block transfer (bitblt) model and to specify that DXGI discard the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. This flag is valid for a swap chain with more than one back buffer, although, applications only have read and write access to buffer 0. Use this flag to enable the display driver to select the most efficient presentation technique for the swap chain. <b>Direct3D 12:  </b>This enumeration value is never supported. D3D12 apps must using <b>FlipSequential</b> or <b>FlipDiscard</b>. <div class="alert"><b>Note</b>  There are differences between full screen exclusive and full screen UWP. If you are porting a Direct3D 11 application to UWP on a Windows PC, be aware that the use of  <b>Discard</b> when creating swap chains does not behave the same way in UWP as it does in Win32, and its use may be detrimental to GPU performance. This is because UWP applications are forced into FLIP swap modes (even if other swap modes are set), because this reduces the computation time used by the memory copies originally done by the older bitblt model. The recommended approach is to manually convert DX11 Discard swap chains to use flip models within UWP,  using <b>FlipDiscard</b> instead of <b>Discard</b> where possible. Refer to the Example below, and see <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/for-best-performance--use-dxgi-flip-model">this article</a> for more information.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Discard			= 0,
	/// <summary>
	/// <para>Use this flag to specify the bitblt model and to specify that DXGI persist the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. Use this option to present the contents of the swap chain in order, from the first buffer (buffer 0) to the last buffer. This flag cannot be used with multisampling. <b>Direct3D 12:  </b>This enumeration value is never supported. D3D12 apps must using <b>FlipSequential</b> or <b>FlipDiscard</b>.</para>
	/// <para><div class="alert"><b>Note</b>  For best performance, use <b>FlipSequential</b> instead of <b>Sequential</b>. See <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/for-best-performance--use-dxgi-flip-model">this article</a> for more information.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Sequential		= 1,
	/// <summary>
	/// <para>Use this flag to specify the flip presentation model and to specify that DXGI persist the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. This flag cannot be used with multisampling.</para>
	/// <para><b>Direct3D 11:  </b>This enumeration value is supported starting with Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	FlipSequential	= 3,
	/// <summary>
	/// <para>Use this flag to specify the flip presentation model and to specify that DXGI discard the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. This flag cannot be used with multisampling and partial presentation. See <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/dxgi-1-4-improvements">DXGI 1.4 Improvements</a>.</para>
	/// <para><b>Direct3D 11:  </b>This enumeration value is supported starting with Windows 10.</para>
	/// <para><div class="alert"><b>Note</b>  Windows Store apps must use <b>FlipSequential</b> or <b>FlipDiscard</b>. </div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	FlipDiscard		= 4,
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
[Flags] public enum SwapChainFlags
{
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
public struct Rational: IEquatable<Rational>
{
	/// <summary>
	/// A Rational with a value of zero
	/// </summary>
	public static readonly Rational Zero = (0x00u, 0x01u);

	/// <summary>
	/// Gets the rational value as a float
	/// </summary>
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	public float AsFloat => denominator == 0 ? 0f : (float) (numerator / denominator);



	internal Rational( in DXGI_RATIONAL rational ) {
		this.numerator = rational.Numerator;
		this.denominator = rational.Denominator;
	}

	internal unsafe Rational( DXGI_RATIONAL* pRational ) {
		fixed ( Rational* pThis = &this )
			*pThis = *((Rational*) pRational);
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
		if ( denominator == 0 )
			throw new ArgumentOutOfRangeException( "denominator", 
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
		if ( values.denominator == 0 )
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
		obj is Rational r ? Equals(r) : false;

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

		if ( n < m ) {
			uint tmp = n;
			n = m;
			m = tmp;
		}
		
#if DEBUG || !STRIP_CHECKS
		uint count = 0x00;
		const uint MAX_ITERATIONS = 100000;
#endif
		while ( m > 0 ) {
			uint tmp = n % m;
			n = m;
			m = tmp;

#if DEBUG || !STRIP_CHECKS
			if ( ++count > MAX_ITERATIONS ) {
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
	public static implicit operator Rational( (uint numerator, uint denominator) values ) => new Rational( values );

	/// <summary>
	/// Converts an unsigned value into rational form
	/// </summary>
	/// <param name="value">A whole, unsigned value</param>
	public static implicit operator Rational( uint value ) => new Rational( value );

	public static bool operator ==( Rational a, Rational b ) => a.Equals( b );
	public static bool operator !=( Rational a, Rational b ) => !a.Equals( b );
	public static bool operator ==( Rational a, uint b ) => a.Equals( b );
	public static bool operator !=( Rational a, uint b ) => !a.Equals( b );
	public static bool operator ==( uint a, Rational b ) => a.Equals( b );
	public static bool operator !=( uint a, Rational b ) => !a.Equals( b );
};


/// <summary>Describes multi-sampling parameters for a resource.</summary>
/// <remarks>
/// <para>This structure is a member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">DXGI_SWAP_CHAIN_DESC1</a> structure. The default sampler mode, with no anti-aliasing, has a count of 1 and a quality level of 0. If multi-sample antialiasing is being used, all bound render targets and depth buffers must have the same sample counts and quality levels. </para>
/// <para>This doc was truncated.</para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_sample_desc#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
public struct SampleDescription
{
	internal SampleDescription( in DXGI_SAMPLE_DESC desc ) {
		this.count = desc.Count;
		this.quality = desc.Quality;
	}

	internal unsafe SampleDescription( DXGI_SAMPLE_DESC* pDesc ) {
		fixed( SampleDescription* pThis = &this ) {
			*pThis = *((SampleDescription*) pDesc);
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

	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	uint count;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	uint quality;

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
};


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
public struct SwapChainDescription
{
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
	public Usage BufferUsage { get => (Usage)desc.BufferUsage; set => desc.BufferUsage = (uint)value; }
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
};

/// <summary>Describes a swap chain.</summary>
/// <remarks>
/// <para>This structure is used by the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-getdesc">GetDesc</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">CreateSwapChain</a> methods. In full-screen mode, there is a dedicated front buffer; in windowed mode, the desktop is the front buffer. If you create a swap chain with one buffer, specifying <b>DXGI_SWAP_EFFECT_SEQUENTIAL</b> does not cause the contents of the single buffer to be swapped with the front buffer. For performance information about flipping swap-chain buffers in full-screen application, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/d3d10-graphics-programming-guide-dxgi">Full-Screen Application Performance Hints</a>.</para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
public struct SwapChainDescription1
{
	internal SwapChainDescription1( in DXGI_SWAP_CHAIN_DESC1 desc ) {
		//this.Width = desc.Width;
		//this.Height = desc.Height;
		//this.Format = (Format)desc.Format;
		//this.Stereo = desc.Stereo;
		//this.SampleDesc = desc.SampleDesc;
		//this.BufferUsage = (Usage)desc.BufferUsage;
		//this.BufferCount = desc.BufferCount;
		//this.Scaling = (Scaling)desc.Scaling;
		//this.SwapEffect = (SwapEffect)desc.SwapEffect;
		//this.AlphaMode = (AlphaMode)desc.AlphaMode;
		//this.Flags = (SwapChainFlags)desc.Flags;
		this.desc = desc;
	}

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
		AlphaMode alphaMode, SwapChainFlags flags = default )
	{
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

	//ModeDescription bufferDesc;
	//SampleDescription sampleDesc;
	//Usage bufferUsage;
	//uint bufferCount;
	//HWND outputWindow;
	//BOOL windowed;
	//SwapEffect swapEffect;
	//uint flags;

	// wrap the internal type:
	[DebuggerBrowsable( DebuggerBrowsableState.Never )] 
	DXGI_SWAP_CHAIN_DESC1 desc;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	internal DXGI_SWAP_CHAIN_DESC1 InternalValue => desc;


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
	public Format Format { get => (Format) desc.Format; set => desc.Format = (DXGI_FORMAT) value; }

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
	public Usage BufferUsage { get => (Usage) desc.BufferUsage; set => desc.BufferUsage = (uint) value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A value that describes the number of buffers in the swap chain. When you call  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">IDXGIFactory::CreateSwapChain</a> to create a full-screen swap chain, you typically include the front buffer in this value. For more information about swap-chain buffers, see Remarks.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint BufferCount { get => desc.BufferCount; set => desc.BufferCount = value; }

	/// <summary>
	/// <para>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_scaling">DXGI_SCALING</a>-typed value that identifies resize behavior if the size of the back buffer is not equal to the target output.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public Scaling Scaling { get => (Scaling)desc.Scaling; set => desc.Scaling = (DXGI_SCALING) value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT</a> enumerated type that describes options for handling the contents of the presentation buffer after presenting a surface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public SwapEffect SwapEffect { get => (SwapEffect) desc.SwapEffect; set => desc.SwapEffect = (DXGI_SWAP_EFFECT) value; }

	/// <summary>
	/// <para>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_alpha_mode">DXGI_ALPHA_MODE</a>-typed value that identifies the transparency behavior of the swap-chain back buffer.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	internal AlphaMode AlphaMode { get => (AlphaMode)desc.AlphaMode; set => desc.AlphaMode = (DXGI_ALPHA_MODE)value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG</a> enumerated type that describes options for swap-chain behavior.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public SwapChainFlags Flags { get => (SwapChainFlags) desc.Flags; set => desc.Flags = (uint) value; }


	/// <summary>
	/// Converts a SwapChainDescription to a SwapChainDescription1 structure
	/// </summary>
	/// <param name="desc">A SwapChainDescription strcuture</param>
	public static explicit operator SwapChainDescription1( SwapChainDescription desc ) => 
		new SwapChainDescription1( desc );

};



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
public struct SwapChainFullscreenDescription
{
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
	public ScanlineOrder ScanlineOrdering { get => (ScanlineOrder) desc.ScanlineOrdering; set => desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER) value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => (ScalingMode) desc.Scaling; set => desc.Scaling = (DXGI_MODE_SCALING) value; }

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
[DebuggerDisplay("Mode = {Width}x{Height} @ {RefreshRate.AsFloat}Hz ({Scaling})")]
public struct ModeDescription
{
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
			fixed ( ModeDescription1* pData = (&modeDesc1) )
				this.desc = *((DXGI_MODE_DESC*) pData);
		}
	}



	//uint width;
	//uint height;
	//Rational refreshRate;
	//Format format;
	//ScanlineOrder scanlineOrdering;
	//ScalingMode scaling;

	[DebuggerBrowsable( DebuggerBrowsableState.Never )] DXGI_MODE_DESC desc;



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
	public ScanlineOrder ScanlineOrdering { get => (ScanlineOrder)desc.ScanlineOrdering; set => desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER) value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => (ScalingMode)desc.Scaling; set => desc.Scaling = (DXGI_MODE_SCALING)value; }


	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//internal unsafe static ModeDescription MemCopyFrom( in DXGI_MODE_DESC mode ) {
	//	fixed ( DXGI_MODE_DESC* pMode = &mode )
	//		return MemCopyFrom( pMode );
	//}

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//internal static unsafe ModeDescription MemCopyFrom( DXGI_MODE_DESC* pMode ) {
	//	ModeDescription desc = default;
	//	*(&desc) = *((ModeDescription*) pMode);
	//	return desc;
	//}

	public static explicit operator ModeDescription( ModeDescription1 mode ) => new( mode );

};


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
public struct ModeDescription1
{
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
	public ModeDescription1( uint width, uint height, Rational refreshRate, Format format = Format.R8G8B8A8_UNORM,
		ScanlineOrder scanlineOrdering = ScanlineOrder.Unspecified, ScalingMode scaling = ScalingMode.Centered, bool stereo = false )
	{
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
	public Format Format { get => (Format) desc.Format; set => desc.Format = (DXGI_FORMAT) value; }

	/// <summary>
	/// A member of the DXGI.ScanlineOrder enumerated type describing the scanline drawing mode.
	/// </summary>
	public ScanlineOrder ScanlineOrdering { get => (ScanlineOrder) desc.ScanlineOrdering; set => desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER) value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => (ScalingMode) desc.Scaling; set => desc.Scaling = (DXGI_MODE_SCALING) value; }

	/// <summary>
	/// Specifies whether the full-screen display mode is stereo. True if stereo; otherwise, false.
	/// </summary>
	public bool Stereo { get => desc.Stereo; set => desc.Stereo = value; }



	/// <summary>
	/// Converts a ModeDescription to a ModeDescription1 structure
	/// </summary>
	/// <param name="desc">A ModeDescription structure</param>
	public static implicit operator ModeDescription1( ModeDescription desc ) => new ModeDescription1( desc );


};