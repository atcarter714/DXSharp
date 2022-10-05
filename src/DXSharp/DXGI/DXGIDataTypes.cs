#region Using Directives
using global::System;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;

using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D11;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;

using global::Windows.Win32;
using Win32 = global::Windows.Win32;
using Windows.Win32.Graphics.Dxgi.Common;
using static System.Net.WebRequestMethods;

#endregion

namespace DXSharp.DXGI;



/// <summary>Represents a rational number.</summary>
/// <remarks>
/// <para>In the native DXGI API, this structure is a member of the 
/// <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXGI_MODE_DESC</a> structure.
/// The <b>DXGI_RATIONAL</b> structure operates under the following rules: </para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_rational#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
public struct Rational
{
	/// <summary>
	/// Creates a new rational
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

/// <summary>
/// Flags indicating the method the raster uses to create an image on a surface.
/// </summary>
/// <remarks>
/// <para>See <a href="https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/bb173067(v=vs.85)">DXGI_MODE_SCANLINE_ORDER</a> for more info.</para>
/// </remarks>
public enum ScanlineOrder
{
	Unspecified		= 0,
	Progressive		= 1,
	UpperFieldFirst = 2,
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
/// More info at <a href="https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXGI_MODE_DESC</a>
/// </remarks>
public struct ModeDescription
{
	uint width;
	uint height;
	Rational refreshRate;
	Format format;
	ScanlineOrder scanlineOrdering;
	ScalingMode scaling;

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
	/// 
	/// </summary>
	public Rational RefreshRate { get => refreshRate; set => refreshRate = value; }

	/// <summary>
	/// 
	/// </summary>
	public Format Format { get => format; set => format = value; }

	/// <summary>
	/// 
	/// </summary>
	public ScanlineOrder ScanlineOrdering { get => scanlineOrdering; set => scanlineOrdering = value; }

	/// <summary>
	/// 
	/// </summary>
	public ScalingMode Scaling { get => scaling; set => scaling = value; }
};