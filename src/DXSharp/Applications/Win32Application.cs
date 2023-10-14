#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using DXSharp.Windows.COM ;
using Windows.Win32.Foundation ;
using DXSharp.Windows.Win32.Helpers ;
using Windows.Win32.UI.WindowsAndMessaging ;
using static Windows.Win32.PInvoke ; //! <-- CsWin32
#endregion
namespace DXSharp.Windows.Win32.XTensions ;

public class Win32Application {
	static HWND _hWnd ;
	static HInstance _hInstance ;
	public static HWND _HWnd => _hWnd ;
	static DXApp? _sample = null ;
	
	public static unsafe int Run( DXApp? sample, HMODULE hInstance,
								  ShowWindowCommands nCmdShow = ShowWindowCommands.SW_SHOWDEFAULT ) {
		ArgumentNullException.ThrowIfNull( sample, nameof(sample) ) ;
		
		_hInstance = hInstance ;
		_sample = sample ;
		
		// Create GCHandle for the program:
		GCHandle   sampleHandle = GCHandle.Alloc( sample, 
												  GCHandleType.WeakTrackResurrection ) ;
		InstHandle instHandle   = new(hInstance, false) ;
		var samplePtr = GCHandle.ToIntPtr( sampleHandle ) ;
		
		// Define window class:
		WNDCLASS_STYLES ClassStyles = new( ) { } ;
		WNDCLASSEXW windowClass = new( ) {
			cbSize        = (uint)Marshal.SizeOf< WNDCLASSEXW >(),
			style         = WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_VREDRAW,
			lpfnWndProc   = WindowProc,
			hInstance     = hInstance,
			hCursor       = (HCURSOR)PInvoke.LoadCursor(instHandle, CursorType.IDC_ARROW.ToString())
								.DangerousGetHandle(),
			lpszClassName = "DXAppClass".ToPWSTR(),
		};

		// Register window class
		ushort regResult = PInvoke.RegisterClassEx(windowClass);
		if( regResult == 0 ) {
			// Handle error: Registration failed
			// Use Marshal.GetLastWin32Error() to get the error code
		}

		// Create window
		_hWnd = PInvoke.CreateWindowEx(
			0,
			windowClass.lpszClassName.ToString( ),
			sample.Title,
			(WINDOW_STYLE)WindowStyles.WS_OVERLAPPEDWINDOW,
			(int)WindowPositions.CW_USEDEFAULT,
			(int)WindowPositions.CW_USEDEFAULT,
			sample.Width, sample.Height,
			HWND.Null,
			default,
			instHandle,
			(void *)samplePtr
		) ;

		if( _hWnd.IsNull ) {
			// Handle error: Window creation failed
			// Use Marshal.GetLastWin32Error() to get the error code
		}
		
		// Show the window:
		PInvoke.ShowWindow( _hWnd, (SHOW_WINDOW_CMD)nCmdShow ) ;
		//... rest of code

		// Don't forget to free the handle at the end of your method
		sampleHandle.Free( ) ;
		return 0x00000000 ;
	}
	
	static LRESULT WindowProc( HWND hWnd, uint message, WPARAM wParam, LPARAM lParam ) {
		if( !hWnd.IsNull ) {
			nint ptr = GetWindowLong( hWnd,
									  (WINDOW_LONG_PTR_INDEX) WindowLongParam.GWLP_USERDATA ) ;
			if( ptr.IsValid() ) {
				_sample = (DXApp)( (GCHandle.FromIntPtr(ptr) )
								  .Target as DXApp )! ;
			}
		}

		switch( message ) {
			case (uint)WindowMessage.WM_CREATE: {
				CreateStruct createStruct = Marshal.PtrToStructure< CreateStruct >( lParam ) ;
				PInvoke.SetWindowLong( hWnd,(WINDOW_LONG_PTR_INDEX) WindowLongParam.GWLP_USERDATA, 
										  (int)GCHandle.ToIntPtr( GCHandle.Alloc( createStruct.lpCreateParams ) ) );
				return (LRESULT)0 ;
			}
			/*case (uint)WindowMessage.WM_CREATE:
				CreateStruct createStruct = Marshal.PtrToStructure<CreateStruct>(lParam);
				PInvoke.SetWindowLong(hWnd, (WINDOW_LONG_PTR_INDEX)WindowLongParam.GWLP_USERDATA, 
									  (int)createStruct.lpCreateParams) ;
				return (LRESULT)0 ;*/

			case (uint)WindowMessage.WM_KEYDOWN:
				_sample?.OnKeyDown( (byte)wParam.Value );
				return (LRESULT)0;
			case (uint)WindowMessage.WM_KEYUP:
				_sample?.OnKeyUp( (byte)wParam.Value );
				return (LRESULT)0;

			case (uint)WindowMessage.WM_PAINT:
				if( _sample != null ) {
					_sample.OnUpdate( ) ;
					_sample.OnRender( ) ;
				}
				return (LRESULT)0 ;

			case (uint)WindowMessage.WM_DESTROY:
				PInvoke.PostQuitMessage( 0 ) ;
				return (LRESULT)0 ;
		}
		
		// Handle any messages the switch statement didn't ...
		return DefWindowProc( hWnd, message, wParam, lParam ) ;
	}
	
} ;