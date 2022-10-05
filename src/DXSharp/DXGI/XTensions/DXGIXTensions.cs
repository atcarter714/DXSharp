#region Using Directives
using System;

using Windows.Win32.Graphics.Dxgi.Common;
#endregion

namespace DXSharp.DXGI.XTensions;

/// <summary>
/// Contains DXGI related extension methods
/// </summary>
public static partial class DXGIXTensions
{
	internal static ScanlineOrder AsScanlineOrder( this DXGI_MODE_SCANLINE_ORDER sorder ) => (ScanlineOrder) sorder;
	internal static DXGI_MODE_SCANLINE_ORDER AsDXGI_MODE_SCANLINE_ORDER( this ScanlineOrder sorder ) => (DXGI_MODE_SCANLINE_ORDER) sorder;
}
