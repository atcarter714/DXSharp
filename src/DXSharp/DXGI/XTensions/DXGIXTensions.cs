#region Using Directives

using System.Reflection ;
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

namespace DXSharp.DXGI.XTensions ;


/// <summary>
/// Contains DXGI related extension methods
/// </summary>
public static partial class DXGIXTensions {
	internal static ScanlineOrder AsScanlineOrder( this DXGI_MODE_SCANLINE_ORDER slOrder ) => 
																		(ScanlineOrder)slOrder ;
	
	internal static DXGI_MODE_SCANLINE_ORDER AsDXGI_MODE_SCANLINE_ORDER( this ScanlineOrder slOrder ) => 
																					(DXGI_MODE_SCANLINE_ORDER)slOrder ;
	
	/// <summary>
	/// Indicates if this IUknown COM interface is alive
	/// </summary>
	/// <param name="comObj">This IUnknown instance</param>
	/// <returns>True if alive, otherwise false</returns>
	public static bool IsAlive( this IUnknownWrapper? comObj ) => comObj is { BasePointer: not 0 } ;
	
	
	internal static DXGI_FORMAT AsDXGI_FORMAT( this Format format ) => (DXGI_FORMAT)format ;
	internal static Format AsFormat( this DXGI_FORMAT format ) => (Format)format ;
} ;