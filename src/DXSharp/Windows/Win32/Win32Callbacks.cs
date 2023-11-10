using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Gdi ;
namespace DXSharp.Windows.Win32 ;



// -----------------------------------------------------------------------------------------------
// Unsafe/Unmanaged Signature Delegates ::
// -----------------------------------------------------------------------------------------------
public unsafe delegate BOOL _MONITORENUMPROC( HMONITOR param0, HDC    param1,
											  RECT*    param2, LPARAM param3 ) ;

public unsafe delegate LRESULT _WNDPROC( HWND   param0, uint   param1,
										 WPARAM param2, LPARAM param3 ) ;



// -----------------------------------------------------------------------------------------------
//! "Safe" versions of the above delegates, rearranged to a more
//! idiomatic C# style (i.e., naming, parameter order, etc) ...
// -----------------------------------------------------------------------------------------------

public delegate bool MonitorSafeEnumProc( HMonitor hMonitor, HDC hdcMonitor, LParam dwData,
										  in Rect? lprcMonitor = null ) ;

public delegate LResult WndProc( HWnd   hWnd, uint msg,
								 WParam wParam = default,
								 LParam lParam = default ) ;

