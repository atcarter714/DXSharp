#region Using Directives

using System.Runtime.InteropServices ;
using Windows.System ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Gdi ;
#endregion
namespace DXSharp.Windows.Win32 ;


/// <summary>
/// Exposes native Win32 functions to DXSharp assemblies and user applications
/// in a "safe" and idiomatic way familiar to .NET developers ...
/// </summary>
/// <remarks>
/// The intent of the <see cref="Win32Functions"/> class is to provide a safe and idiomatic
/// way to call native unmanged Win32 functions from managed .NET code. The idea is to stay
/// as close as possible to the original Win32 API, but to make it easier to use from .NET
/// without a lot of boilerplate code and "noise" (i.e., tons of <c>fixed</c> and <c>unsafe</c>
/// blocks with pointers and <c>null</c> checks everywhere). Therefore, signatures will <i>not</i>
/// always be exactly the same as the original Win32 API, but will be familiar and easy to use for
/// anyone who has used Win32 or .NET before.
/// </remarks>
public static class Win32Functions {
	static RECT _tempRect = default ;

	/// <summary>
	/// The GetDC function retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen.
	/// </summary>
	/// <param name="hWnd">
	/// A handle to the window whose DC is to be retrieved. If this value is NULL, GetDC retrieves the DC for the entire screen.
	/// </param>
	/// <returns>
	/// If the function succeeds, the return value is a handle to the DC for the specified window's client area.
	/// If the function fails, the return value is <b>NULL</b> (i.e., <see cref="HWnd"/>.<see cref="HWnd.Null"/> or <c>0x00000000</c>).
	/// </returns>
	public static HDC GetDC( HWnd? hWnd = default ) => PInvoke.GetDC( hWnd ?? default ) ;
	
	

	public static void EnumDisplays( in HDC hdc = default, in Rect? lprcClip = null,
									  in MonitorSafeEnumProc? lpfnEnum = null, in LParam dwData = default ) {
		unsafe {
			RECT* _clipRectPtr = null ;
			if ( lprcClip is not null ) {
				_tempRect = lprcClip.Value ;
				var rectInRAM = (RECT *)Marshal.AllocHGlobal( sizeof(RECT) ) ;
			}
			//PInvoke.EnumDisplayMonitors( hdc, _clipRectPtr, lpfnEnum, dwData ) ;
		}
	}
} ;