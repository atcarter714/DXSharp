#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;
using DXSharp.Direct3D12 ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Identifies the alpha value, transparency behavior, of a surface.</summary>
/// <remarks>
/// <para>For more information about alpha mode, see <a href="https://docs.microsoft.com/windows/desktop/api/dcommon/ne-dcommon-d2d1_alpha_mode">DXGI_ALPHA_MODE</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ne-dxgi1_2-dxgi_alpha_mode#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( DXGI_ALPHA_MODE ) )]
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
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ne-dxgi1_2-dxgi_alpha_mode#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	/// <remarks>
	/// For these C# bindings this should be a non-issue. The underlying type is already uint/UInt32, which is 32-bit.
	/// </remarks>
	ForceDWORD = 4294967295U,
} ;


/// <summary>
/// Flags indicating the method the raster uses to create an image on a surface.
/// </summary>
/// <remarks>
/// <para>See <a href="https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/bb173067(v=vs.85)">DXGI_MODE_SCANLINE_ORDER</a> for more info.</para>
/// </remarks>
[EquivalentOf(typeof(DXGI_MODE_SCANLINE_ORDER))]
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
} ;


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
/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ne-dxgi1_2-dxgi_scaling#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(DXGI_SCALING))]
public enum Scaling {
	/// <summary>
	/// Directs DXGI to make the back-buffer contents scale to fit the presentation target size. 
	/// This is the implicit behavior of DXGI when you call the 
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">IDXGIFactory::CreateSwapChain</a> method.
	/// </summary>
	Stretch = 0,
	/// <summary>
	/// <para>Directs DXGI to make the back-buffer contents appear without any scaling when the presentation target size is not equal to the back-buffer size. The top edges of the back buffer and presentation target are aligned together. If the WS_EX_LAYOUTRTL style is associated with the <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a> handle to the target output window, the right edges of the back buffer and presentation target are aligned together; otherwise, the left edges are aligned together. All target area outside the back buffer is filled with window background color. This value specifies that all target areas outside the back buffer of a swap chain are filled with the background color that you specify in a call to <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-setbackgroundcolor">IDXGISwapChain1::SetBackgroundColor</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ne-dxgi1_2-dxgi_scaling#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	None = 1,
	/// <summary>
	/// <para>Directs DXGI to make the back-buffer contents scale to fit the presentation 
	/// target size, while preserving the aspect ratio of the back-buffer. If the scaled 
	/// back-buffer does not fill the presentation area, it will be centered with black 
	/// borders. This constant is supported on Windows Phone 8 and Windows 10. Note that 
	/// with legacy Win32 window swapchains, this works the same as DXGI_SCALING_STRETCH.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ne-dxgi1_2-dxgi_scaling#members">Read more on docs.microsoft.com</a>.</para>
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
[EquivalentOf(typeof(DXGI_MODE_SCALING))]
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
[EquivalentOf(typeof(DXGI_USAGE))]
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
/// <a href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#">Read more on docs.microsoft.com</a>.
/// </para>
/// </remarks>
[EquivalentOf(typeof(DXGI_SWAP_EFFECT))]
public enum SwapEffect {
	/// <summary>
	/// <para>Use this flag to specify the bit-block transfer (bitblt) model and to specify that DXGI discard the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. This flag is valid for a swap chain with more than one back buffer, although, applications only have read and write access to buffer 0. Use this flag to enable the display driver to select the most efficient presentation technique for the swap chain. <b>Direct3D 12:  </b>This enumeration value is never supported. D3D12 apps must using <b>FlipSequential</b> or <b>FlipDiscard</b>. <div class="alert"><b>Note</b>  There are differences between full screen exclusive and full screen UWP. If you are porting a Direct3D 11 application to UWP on a Windows PC, be aware that the use of  <b>Discard</b> when creating swap chains does not behave the same way in UWP as it does in Win32, and its use may be detrimental to GPU performance. This is because UWP applications are forced into FLIP swap modes (even if other swap modes are set), because this reduces the computation time used by the memory copies originally done by the older bitblt model. The recommended approach is to manually convert DX11 Discard swap chains to use flip models within UWP,  using <b>FlipDiscard</b> instead of <b>Discard</b> where possible. Refer to the Example below, and see <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/for-best-performance--use-dxgi-flip-model">this article</a> for more information.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Discard         = 0,
	/// <summary>
	/// <para>Use this flag to specify the bitblt model and to specify that DXGI persist the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. Use this option to present the contents of the swap chain in order, from the first buffer (buffer 0) to the last buffer. This flag cannot be used with multisampling. <b>Direct3D 12:  </b>This enumeration value is never supported. D3D12 apps must using <b>FlipSequential</b> or <b>FlipDiscard</b>.</para>
	/// <para><div class="alert"><b>Note</b>  For best performance, use <b>FlipSequential</b> instead of <b>Sequential</b>. See <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/for-best-performance--use-dxgi-flip-model">this article</a> for more information.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Sequential      = 1,
	/// <summary>
	/// <para>Use this flag to specify the flip presentation model and to specify that DXGI persist the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. This flag cannot be used with multisampling.</para>
	/// <para><b>Direct3D 11:  </b>This enumeration value is supported starting with Windows 8.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	FlipSequential  = 3,
	/// <summary>
	/// <para>Use this flag to specify the flip presentation model and to specify that DXGI discard the contents of the back buffer after you call <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>. This flag cannot be used with multisampling and partial presentation. See <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/dxgi-1-4-improvements">DXGI 1.4 Improvements</a>.</para>
	/// <para><b>Direct3D 11:  </b>This enumeration value is supported starting with Windows 10.</para>
	/// <para><div class="alert"><b>Note</b>  Windows Store apps must use <b>FlipSequential</b> or <b>FlipDiscard</b>. </div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ne-dxgi-dxgi_swap_effect#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	FlipDiscard     = 4,
};


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
[Flags, EquivalentOf(typeof(DXGI_SWAP_CHAIN_FLAG))]
public enum SwapChainFlags {
	/// <summary>
	/// No flags
	/// </summary>
	None = 0,
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


/// <summary>Flags indicating the memory location of a resource.</summary>
/// <remarks>This enum is used by <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgidevice-queryresourceresidency">QueryResourceResidency</a>.</remarks>
[EquivalentOf(typeof(DXGI_RESIDENCY))]
public enum Residency {
	/// <summary>The resource is located in video memory.</summary>
	FullResident = 1,
	/// <summary>At least some of the resource is located in CPU memory.</summary>
	SharedMemory = 2,
	/// <summary>At least some of the resource has been paged out to the hard drive.</summary>
	EvictedToDisk = 3,
} ;


/// <summary>Identifies the granularity at which the graphics processing unit (GPU) can be preempted from performing its current graphics rendering task.</summary>
/// <remarks>
/// <para>You call the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiadapter2-getdesc2">IDXGIAdapter2::GetDesc2</a> method to
/// retrieve the granularity level at which the GPU can be preempted from performing its current graphics rendering task. The operating system specifies the
/// graphics granularity level in the  <b>GraphicsPreemptionGranularity</b> member of the
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_adapter_desc2">DXGI_ADAPTER_DESC2</a> structure.
/// The following figure shows granularity of graphics rendering tasks. </para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ne-dxgi1_2-dxgi_graphics_preemption_granularity#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
[EquivalentOf(typeof(DXGI_GRAPHICS_PREEMPTION_GRANULARITY))]
public enum GraphicsPreemptionGranularity {
	DMA_BUFFER_BOUNDARY  = 0,
	PRIMITIVE_BOUNDARY   = 1,
	TRIANGLE_BOUNDARY    = 2,
	PIXEL_BOUNDARY       = 3,
	INSTRUCTION_BOUNDARY = 4,
} ;


/// <summary>Identifies the granularity at which the graphics processing unit (GPU) can be preempted from performing its current compute task.</summary>
/// <remarks>
/// You call the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiadapter2-getdesc2">IDXGIAdapter2::GetDesc2</a>
/// method to retrieve the granularity level at which the GPU can be preempted from performing its current compute task. The operating system
/// specifies the compute granularity level in the  <b>ComputePreemptionGranularity</b> member of the
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_adapter_desc2">DXGI_ADAPTER_DESC2</a> structure.
/// </remarks>
[EquivalentOf(typeof(DXGI_COMPUTE_PREEMPTION_GRANULARITY))]
public enum ComputePreemptionGranularity {
	DMA_BUFFER_BOUNDARY   = 0,
	DISPATCH_BOUNDARY     = 1,
	THREAD_GROUP_BOUNDARY = 2,
	THREAD_BOUNDARY       = 3,
	INSTRUCTION_BOUNDARY  = 4,
} ;


/// <summary>Defines constants that specify a Direct3D 12 feature or feature set to query about.</summary>
/// <remarks>
/// Use a constant from this enumeration in a call to the <see cref="IDevice"/> interface's <b>CheckFeatureSupport</b> method 
/// to query a driver about support for various Direct3D 12 features. Each value in this enumeration has a corresponding data structure
/// that you must pass (by pointer reference) in the <i>pFeatureSupportData</i> parameter of <b>ID3D12Device::CheckFeatureSupport</b>.
/// </remarks>
[EquivalentOf(typeof(D3D12_FEATURE))]
public enum D3D12Feature {
	/// <summary>Indicates a query for the level of support for basic Direct3D 12 feature options. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options">D3D12_FEATURE_DATA_D3D12_OPTIONS</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS = 0,
	/// <summary>
	/// <para>Indicates a query for the adapter's architectural details, so that your application can better optimize for certain adapter properties. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture">D3D12_FEATURE_DATA_ARCHITECTURE</a>. <div class="alert"><b>Note</b>  This value has been superseded by the <b>D3D_FEATURE_DATA_ARCHITECTURE1</b> value. If your application targets Windows 10, version 1703 (Creators' Update) or higher, then use the <b>D3D_FEATURE_DATA_ARCHITECTURE1</b> value instead.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_feature#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	D3D12_FEATURE_ARCHITECTURE = 1,
	/// <summary>Indicates a query for info about the <a href="https://docs.microsoft.com/windows/win32/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature levels</a> supported. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_feature_levels">D3D12_FEATURE_DATA_FEATURE_LEVELS</a>.</summary>
	D3D12_FEATURE_FEATURE_LEVELS = 2,
	/// <summary>Indicates a query for the resources supported by the current graphics driver for a given format. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_format_support">D3D12_FEATURE_DATA_FORMAT_SUPPORT</a>.</summary>
	D3D12_FEATURE_FORMAT_SUPPORT = 3,
	/// <summary>Indicates a query for the image quality levels for a given format and sample count. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_multisample_quality_levels">D3D12_FEATURE_DATA_MULTISAMPLE_QUALITY_LEVELS</a>.</summary>
	D3D12_FEATURE_MULTISAMPLE_QUALITY_LEVELS = 4,
	/// <summary>Indicates a query for the DXGI data format. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_format_info">D3D12_FEATURE_DATA_FORMAT_INFO</a>.</summary>
	D3D12_FEATURE_FORMAT_INFO = 5,
	/// <summary>Indicates a query for the GPU's virtual address space limitations. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_gpu_virtual_address_support">D3D12_FEATURE_DATA_GPU_VIRTUAL_ADDRESS_SUPPORT</a>.</summary>
	D3D12_FEATURE_GPU_VIRTUAL_ADDRESS_SUPPORT = 6,
	/// <summary>Indicates a query for the supported shader model. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_shader_model">D3D12_FEATURE_DATA_SHADER_MODEL</a>.</summary>
	D3D12_FEATURE_SHADER_MODEL = 7,
	/// <summary>Indicates a query for the level of support for HLSL 6.0 wave operations. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options1">D3D12_FEATURE_DATA_D3D12_OPTIONS1</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS1 = 8,
	/// <summary>Indicates a query for the level of support for protected resource sessions. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_support">D3D12_FEATURE_DATA_PROTECTED_RESOURCE_SESSION_SUPPORT</a>.</summary>
	D3D12_FEATURE_PROTECTED_RESOURCE_SESSION_SUPPORT = 10,
	/// <summary>Indicates a query for root signature version support. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_root_signature">D3D12_FEATURE_DATA_ROOT_SIGNATURE</a>.</summary>
	D3D12_FEATURE_ROOT_SIGNATURE = 12,
	/// <summary>
	/// <para>Indicates a query for each adapter's architectural details, so that your application can better optimize for certain adapter properties. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1">D3D12_FEATURE_DATA_ARCHITECTURE1</a>. <div class="alert"><b>Note</b>  This value supersedes the <b>D3D_FEATURE_DATA_ARCHITECTURE</b> value. If your application targets Windows 10, version 1703 (Creators' Update) or higher, then use <b>D3D_FEATURE_DATA_ARCHITECTURE1</b>.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_feature#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	D3D12_FEATURE_ARCHITECTURE1 = 16,
	/// <summary>Indicates a query for the level of support for depth-bounds tests and programmable sample positions. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options2">D3D12_FEATURE_DATA_D3D12_OPTIONS2</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS2 = 18,
	/// <summary>Indicates a query for the level of support for shader caching. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_shader_cache">D3D12_FEATURE_DATA_SHADER_CACHE</a>.</summary>
	D3D12_FEATURE_SHADER_CACHE = 19,
	/// <summary>Indicates a query for the adapter's support for prioritization of different command queue types. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_command_queue_priority">D3D12_FEATURE_DATA_COMMAND_QUEUE_PRIORITY</a>.</summary>
	D3D12_FEATURE_COMMAND_QUEUE_PRIORITY = 20,
	/// <summary>Indicates a query for the level of support for timestamp queries, format-casting, immediate write, view instancing, and barycentrics. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options3">D3D12_FEATURE_DATA_D3D12_OPTIONS3</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS3 = 21,
	/// <summary>Indicates a query for whether or not the adapter supports creating heaps from existing system memory. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_existing_heaps">D3D12_FEATURE_DATA_EXISTING_HEAPS</a>.</summary>
	D3D12_FEATURE_EXISTING_HEAPS = 22,
	/// <summary>Indicates a query for the level of support for 64KB-aligned MSAA textures, cross-API sharing, and native 16-bit shader operations. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options4">D3D12_FEATURE_DATA_D3D12_OPTIONS4</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS4 = 23,
	/// <summary>Indicates a query for the level of support for heap serialization. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_serialization">D3D12_FEATURE_DATA_SERIALIZATION</a>.</summary>
	D3D12_FEATURE_SERIALIZATION = 24,
	/// <summary>Indicates a query for the level of support for the sharing of resources between different adapters; for example, multiple GPUs. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_cross_node">D3D12_FEATURE_DATA_CROSS_NODE</a>.</summary>
	D3D12_FEATURE_CROSS_NODE = 25,
	/// <summary>Starting with Windows 10, version 1809 (10.0; Build 17763), indicates a query for the level of support for render passes, ray tracing, and shader-resource view tier 3 tiled resources. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options5">D3D12_FEATURE_DATA_D3D12_OPTIONS5</a>.</summary>
	D3D12_FEATURE_D3D12_OPTIONS5 = 27,
	/// <summary>Starting with Windows 11 (Build 10.0.22000.194). The corresponding data structure for this value is [D3D12_FEATURE_DATA_DISPLAYABLE](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_displayable).</summary>
	D3D12_FEATURE_DISPLAYABLE = 28,
	/// <summary>
	/// <para>Starting with Windows 10, version 1903 (10.0; Build 18362), indicates a query for the level of support for variable-rate shading (VRS), and indicates whether or not background processing is supported. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS6](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options6). For more info, see <a href="https://docs.microsoft.com/windows/win32/direct3d12/vrs">Variable-rate shading (VRS)</a>, and the <a href="https://microsoft.github.io/DirectX-Specs/d3d/BackgroundProcessing.html">Direct3D 12 background processing spec</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_feature#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	D3D12_FEATURE_D3D12_OPTIONS6 = 30,
	/// <summary>Indicates a query for the level of support for metacommands. The corresponding data structure for this value is <a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_query_meta_command">D3D12_FEATURE_DATA_QUERY_META_COMMAND</a>.</summary>
	D3D12_FEATURE_QUERY_META_COMMAND = 31,
	/// <summary>
	/// <para>Starting with Windows 10, version 2004 (10.0; Build 19041), indicates a query for the level of support for mesh and amplification shaders, and for sampler feedback. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS7](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options7). For more info, see the [Mesh shader](https://microsoft.github.io/DirectX-Specs/d3d/MeshShader.html) and [Sampler feedback](https://microsoft.github.io/DirectX-Specs/d3d/SamplerFeedback.html) specs.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_feature#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	D3D12_FEATURE_D3D12_OPTIONS7 = 32,
	/// <summary>Starting with Windows 10, version 2004 (10.0; Build 19041), indicates a query to retrieve the count of protected resource session types. The corresponding data structure for this value is [D3D12_FEATURE_DATA_PROTECTED_RESOURCE_SESSION_TYPE_COUNT](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_type_count).</summary>
	D3D12_FEATURE_PROTECTED_RESOURCE_SESSION_TYPE_COUNT = 33,
	/// <summary>Starting with Windows 10, version 2004 (10.0; Build 19041), indicates a query to retrieve the list of protected resource session types. The corresponding data structure for this value is [D3D12_FEATURE_DATA_PROTECTED_RESOURCE_SESSION_TYPES](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_protected_resource_session_types).</summary>
	D3D12_FEATURE_PROTECTED_RESOURCE_SESSION_TYPES = 34,
	/// <summary>Starting with Windows 11 (Build 10.0.22000.194), indicates whether or not unaligned block-compressed textures are supported. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS8](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options8).</summary>
	D3D12_FEATURE_D3D12_OPTIONS8 = 36,
	/// <summary>Starting with Windows 11 (Build 10.0.22000.194), indicates whether or not support exists for mesh shaders, values of *SV_RenderTargetArrayIndex* that are 8 or greater, typed resource 64-bit integer atomics, derivative and derivative-dependent texture sample operations, and the level of support for WaveMMA (wave_matrix) operations. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS9](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options9).</summary>
	D3D12_FEATURE_D3D12_OPTIONS9 = 37,
	/// <summary>Starting with Windows 11 (Build 10.0.22000.194), indicates whether or not the SUM combiner can be used, and whether or not *SV_ShadingRate* can be set from a mesh shader. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS10](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options10).</summary>
	D3D12_FEATURE_D3D12_OPTIONS10 = 39,
	/// <summary>Starting with Windows 11 (Build 10.0.22000.194), indicates whether or not 64-bit integer atomics on resources in descriptor heaps are supported. The corresponding data structure for this value is [D3D12_FEATURE_DATA_D3D12_OPTIONS11](/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_d3d12_options11).</summary>
	D3D12_FEATURE_D3D12_OPTIONS11 = 40,
	D3D12_FEATURE_D3D12_OPTIONS12 = 41,
	D3D12_FEATURE_D3D12_OPTIONS13 = 42,
	D3D12_FEATURE_D3D12_OPTIONS14 = 43,
	D3D12_FEATURE_D3D12_OPTIONS15 = 44,
	D3D12_FEATURE_D3D12_OPTIONS16 = 45,
	D3D12_FEATURE_D3D12_OPTIONS17 = 46,
	D3D12_FEATURE_D3D12_OPTIONS18 = 47,
	D3D12_FEATURE_D3D12_OPTIONS19 = 48,
} ;


/// <summary>
/// Identifies the importance of a resource’s content when you call the
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-offerresources">IDXGIDevice2::OfferResources</a>
/// method to offer the resource.
/// </summary>
/// <remarks>Priority determines how likely the operating system is to discard an offered resource. Resources offered with lower priority are discarded first.</remarks>
[EquivalentOf(typeof(DXGI_OFFER_RESOURCE_PRIORITY))]
public enum OfferResourcePriority {
	/// <summary>
	/// The resource is low priority. The operating system discards a low priority resource before other offered resources with higher priority.
	/// It is a good programming practice to mark a resource as low priority if it has no useful content.
	/// </summary>
	Low = 1,
	/// <summary>The resource is normal priority. You mark a resource as normal priority if it has  content that is easy to regenerate.</summary>
	Normal = 2,
	/// <summary>
	/// The resource is high priority. The operating system discards other offered resources with lower priority before it discards a high priority resource.
	/// You mark a resource as high priority if it has useful content that is difficult to regenerate.
	/// </summary>
	High = 3,
} ;


/// <summary>Identifies the type of pointer shape.</summary>
/// <remarks>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ne-dxgi1_2-dxgi_outdupl_pointer_shape_type">Learn more about this API from docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(DXGI_OUTDUPL_POINTER_SHAPE_TYPE))]
public enum OutputDuplicationPointerShapeType {
	/// <summary>The pointer type is a monochrome mouse pointer, which is  a monochrome bitmap. The bitmap's size is specified by width and height in a 1 bits per pixel (bpp) device independent bitmap (DIB) format AND mask that is followed by another 1 bpp DIB format XOR mask of the same size.</summary>
	Monochrome = 1,
	/// <summary>The pointer type is a color mouse pointer, which is  a color bitmap. The bitmap's size is specified by width and height in a 32 bpp ARGB DIB format.</summary>
	Color = 2,
	/// <summary>The pointer type is a masked color mouse pointer.  A masked color mouse pointer is a 32 bpp ARGB format bitmap with the mask value in the alpha bits. The only allowed mask values are 0 and 0xFF. When the mask value is 0, the RGB value should replace the screen pixel. When the mask value is 0xFF, an XOR operation is performed on the RGB value and the screen pixel; the result replaces the screen pixel.</summary>
	MaskedColor = 4,
} ;

