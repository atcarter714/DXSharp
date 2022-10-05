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

#endregion

namespace DXSharp.DXGI;

// ---------------------------------------------------------------------------------------
// DXGI Enumerations ::
// ---------------------------------------------------------------------------------------


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
[Flags] public enum SwapChainFlag
{
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
public struct Rational
{
	/// <summary>
	/// Creates a new rational value
	/// </summary>
	/// <param name="numerator">
	/// The numerator value (denominator will be set to 1)
	/// </param>
	public Rational( uint numerator )
	{
		this.numerator = numerator;
		this.denominator = 1;
	}

	/// <summary>
	/// Creates a new rational value
	/// </summary>
	/// <param name="numerator">The numerator value</param>
	/// <param name="denominator">The denominator value</param>
	public Rational( uint numerator, uint denominator )
	{
		this.numerator = numerator;
		this.denominator = 1;
	}

	/// <summary>
	/// Creates a new Rational value
	/// </summary>
	/// <param name="values">Tuple holding numerator and denominator values</param>
	public Rational( (uint numerator, uint denominator) values ) {
		this.numerator = values.numerator;
		this.denominator = values.denominator;
	}

	uint numerator;
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
	
	uint count;
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
	//ModeDescription bufferDesc;
	//SampleDescription sampleDesc;
	//Usage bufferUsage;
	//uint bufferCount;
	//HWND outputWindow;
	//BOOL windowed;
	//SwapEffect swapEffect;
	//uint flags;

	// wrap the internal type:
	DXGI_SWAP_CHAIN_DESC desc;
	
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
	public SwapChainFlag Flags { get => (SwapChainFlag)desc.Flags; set => desc.Flags = (uint)value; }
};

//typedef struct DXGI_SWAP_CHAIN_FULLSCREEN_DESC
//{
//	DXGI_RATIONAL RefreshRate;
//	DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;
//	DXGI_MODE_SCALING Scaling;
//	BOOL Windowed;
//}
//DXGI_SWAP_CHAIN_FULLSCREEN_DESC;

public struct SwapChainFullscreenDescription
{
	
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
public struct ModeDescription
{
	internal unsafe ModeDescription( DXGI_MODE_DESC* pModeDesc ) {
		fixed ( ModeDescription* pThis = &this ) {
			*pThis = *((ModeDescription*) pModeDesc);
		}
	}

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
		ScanlineOrder scanlineOrdering = ScanlineOrder.Unspecified, ScalingMode scaling = ScalingMode.Centered )
	{
		this.width = width;
		this.height = height;
		this.refreshRate = refreshRate;
		this.format = format;
		this.scanlineOrdering = scanlineOrdering;
		this.scaling = scaling;
	}

	uint width;
	uint height;
	Rational refreshRate;
	Format format;
	ScanlineOrder scanlineOrdering;
	ScalingMode scaling;

	// Note:
	// Instead of auto-properties I've gone for a full prop w/ backing field declaration
	// I suspect it may be faster to translate these structs to/from the internal interop
	// types by using a pointer in a fixed statement and copying the whole thing

	/// <summary>
	/// A value that describes the resolution width. If you specify the width as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the width from the output window and assigns this width 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned width value.
	/// </summary>
	public uint Width { get => width; set => width = value; }

	/// <summary>
	/// A value that describes the resolution height. If you specify the height as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the height from the output window and assigns this height 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned height value.
	/// </summary>
	public uint Height { get => height; set => height = value; }

	/// <summary>
	/// A <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_rational?redirectedfrom=MSDN">DXGI.Rational</a> 
	/// structure describing the refresh rate in hertz
	/// </summary>
	public Rational RefreshRate { get => refreshRate; set => refreshRate = value; }

	/// <summary>
	/// A DXGI.Format structure describing the display format.
	/// </summary>
	public Format Format { get => format; set => format = value; }

	/// <summary>
	/// A member of the DXGI.ScanlineOrder enumerated type describing the scanline drawing mode.
	/// </summary>
	public ScanlineOrder ScanlineOrdering { get => scanlineOrdering; set => scanlineOrdering = value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => scaling; set => scaling = value; }


	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	internal unsafe static ModeDescription MemCopyFrom( in DXGI_MODE_DESC mode ) {
		fixed ( DXGI_MODE_DESC* pMode = &mode )
			return MemCopyFrom( pMode );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	internal static unsafe ModeDescription MemCopyFrom( DXGI_MODE_DESC* pMode ) {
		ModeDescription desc = default;
		*(&desc) = *((ModeDescription*) pMode);
		return desc;
	}

	
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
public struct ModeDescription1
{
	internal unsafe ModeDescription1( DXGI_MODE_DESC1* pModeDesc ) {
		fixed ( ModeDescription1* pThis = &this ) {
			*pThis = *((ModeDescription1*) pModeDesc);
		}
	}

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
		this.width = width;
		this.height = height;
		this.refreshRate = refreshRate;
		this.format = format;
		this.scanlineOrdering = scanlineOrdering;
		this.scaling = scaling;
		this.stereo = stereo;
	}

	uint width;
	uint height;
	Rational refreshRate;
	Format format;
	ScanlineOrder scanlineOrdering;
	ScalingMode scaling;
	bool stereo;

	
	/// <summary>
	/// A value that describes the resolution width. If you specify the width as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the width from the output window and assigns this width 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned width value.
	/// </summary>
	public uint Width { get => width; set => width = value; }

	/// <summary>
	/// A value that describes the resolution height. If you specify the height as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the height from the output window and assigns this height 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned height value.
	/// </summary>
	public uint Height { get => height; set => height = value; }

	/// <summary>
	/// A <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_rational?redirectedfrom=MSDN">DXGI.Rational</a> 
	/// structure describing the refresh rate in hertz
	/// </summary>
	public Rational RefreshRate { get => refreshRate; set => refreshRate = value; }

	/// <summary>
	/// A DXGI.Format structure describing the display format.
	/// </summary>
	public Format Format { get => format; set => format = value; }

	/// <summary>
	/// A member of the DXGI.ScanlineOrder enumerated type describing the scanline drawing mode.
	/// </summary>
	public ScanlineOrder ScanlineOrdering { get => scanlineOrdering; set => scanlineOrdering = value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => scaling; set => scaling = value; }

	/// <summary>
	/// Specifies whether the full-screen display mode is stereo. True if stereo; otherwise, false.
	/// </summary>
	public bool Stereo { get => stereo; set => stereo = value; }


	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	internal unsafe static ModeDescription1 MemCopyFrom( in DXGI_MODE_DESC1 mode ) {
		fixed ( DXGI_MODE_DESC1* pMode = &mode )
			return MemCopyFrom( pMode );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	internal static unsafe ModeDescription1 MemCopyFrom( DXGI_MODE_DESC1* pMode ) {
		ModeDescription1 desc = default;
		*(&desc) = *((ModeDescription1*) pMode);
		return desc;
	}
};