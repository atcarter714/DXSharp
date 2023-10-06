#region Using Directives
using System.Runtime.InteropServices;

using Windows.Win32.
/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using Windows.Win32.Graphics.Dxgi.Common;

using DXSharp.Windows;
using DXSharp.Windows.COM;
using Windows.Win32.Graphics.Dxgi;
using WinRT;
After:
using Windows.Win32.Graphics.Dxgi;
using Windows.Win32.Graphics.Dxgi.Common;

using WinRT;
*/
Graphics.Dxgi.Common;

using DXSharp.Windows.COM ;
#endregion

namespace DXSharp.DXGI.XTensions;

/// <summary>
/// Contains DXGI related extension methods
/// </summary>
public static partial class DXGIXTensions
{
	internal static ScanlineOrder AsScanlineOrder( this DXGI_MODE_SCANLINE_ORDER sorder ) => (ScanlineOrder)sorder;
	internal static DXGI_MODE_SCANLINE_ORDER AsDXGI_MODE_SCANLINE_ORDER( this ScanlineOrder sorder ) => (DXGI_MODE_SCANLINE_ORDER)sorder;

	/// <summary>
	/// Indicates if this IUknown COM interface is alive
	/// </summary>
	/// <param name="comObj">This IUnknown instance</param>
	/// <returns>True if alive, otherwise false</returns>
	public static bool IsAlive( this IUnknown comObj ) {
		if( comObj is not null ) {
			//var obj = Marshal.GetObjectForIUnknown(comObj.Pointer);
#warning Need to make a real "IsAlive" check
			return true;
		}

		return false;
	}

};