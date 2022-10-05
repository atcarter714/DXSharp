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

public struct ModeDescription
{
	uint width;
	uint height;
	DXGI_RATIONAL refreshRate;
};