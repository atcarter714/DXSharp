#region Using Directives
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi.Common;

using DXSharp.Windows;
using DXSharp.Windows.COM;
using Windows.Win32.Graphics.Dxgi;
using WinRT;
#endregion

namespace DXSharp.DXGI.XTensions;

/// <summary>
/// Contains DXGI related extension methods
/// </summary>
public static partial class DXGIXTensions
{
	internal static ScanlineOrder AsScanlineOrder( this DXGI_MODE_SCANLINE_ORDER sorder ) => (ScanlineOrder) sorder;
	internal static DXGI_MODE_SCANLINE_ORDER AsDXGI_MODE_SCANLINE_ORDER( this ScanlineOrder sorder ) => (DXGI_MODE_SCANLINE_ORDER) sorder;

	/// <summary>
	/// Indicates if this IUknown COM interface is alive
	/// </summary>
	/// <param name="comObj">This IUnknown instance</param>
	/// <returns>True if alive, otherwise false</returns>
	public static bool IsAlive( this IUnknown comObj)
	{
		if( comObj.Pointer != IntPtr.Zero)
		{
			var obj = Marshal.GetObjectForIUnknown(comObj.Pointer);
			return obj is not null;
		}

		return false;
	}

	/// <summary>
	/// <para><b><h3>Extension Method:</h3></b></para>
	/// Retrieves a list of all installed adapters (GPUs)
	/// </summary>
	/// <returns>List of all installed adapters as IAdapter references</returns>
	/// <remarks>
	/// <b>NOTE:</b> This method is not part of the native IDXGIFactory COM interface and it
	/// does not increment/decrement ref counts or dispose of the underlying native
	/// resources. This responsibility is left to the application. This exists purely for
	/// developer "quality of life".
	/// </remarks>
	//public static List<IAdapter> GetAllAdapters( this IFactory factory )
	//{
	//	uint index = 0x00;
	//	HRESULT hr = default;
	//	const uint MAX_INDEX = 0x10;

	//	var adapters = new List<IAdapter>();

	//	while ( HRESULT.DXGI_ERROR_NOT_FOUND !=
	//		( hr = factory.EnumAdapters( index++, out var pAdapter ) ) && index <= MAX_INDEX ) {
	//		if (pAdapter is null) continue;
	//		adapters.Add(pAdapter);
	//	}

	//	return adapters;
	//}

	
};