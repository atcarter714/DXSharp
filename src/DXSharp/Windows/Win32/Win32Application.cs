using System.Runtime.InteropServices;
using DXSharp.Windows.Win32;
using DXSharp.Windows.Win32.Helpers;

using Windows.Win32;
using Windows.Win32.Foundation;
using static Windows.Win32.PInvoke;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DXSharp
{
	public class Win32Application
	{
		private static HWND _hWnd;
		public static HWND HWnd => _hWnd ;

		public static unsafe int Run( DXApp sample, HMODULE hInstance, int nCmdShow ) {
			// Create GCHandle for the program
			GCHandle sampleHandle = GCHandle.Alloc(sample);
			InstHandle instHandle = new InstHandle(hInstance, false);

			// Define window class
			WNDCLASS_STYLES ClassStyles = new() { };
			WNDCLASSEXW windowClass = new WNDCLASSEXW
			{
				cbSize        = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
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
				windowClass.lpszClassName.ToString(),
				sample.Title,
				(WINDOW_STYLE)WindowStyles.WS_OVERLAPPEDWINDOW,
				(int)WindowPositions.CW_USEDEFAULT,
				(int)WindowPositions.CW_USEDEFAULT,
				sample.Width,
				sample.Height,
				HWND.Null,
				default,
				instHandle,
				(void*)sampleHandle.AddrOfPinnedObject()
			);

			if( _hWnd == IntPtr.Zero ) {
				// Handle error: Window creation failed
				// Use Marshal.GetLastWin32Error() to get the error code
			}

			//... rest of your code

			// Don't forget to free the handle at the end of your method
			sampleHandle.Free();

			return 0;//(int)msg.wParam;
		}


		private static LRESULT WindowProc( HWND hWnd, uint message, WPARAM wParam, LPARAM lParam ) {
			DXApp sample = null;
			if( hWnd != IntPtr.Zero ) {

				IntPtr ptr = (IntPtr)GetWindowLong(hWnd, 
					(WINDOW_LONG_PTR_INDEX) WindowLongParam.GWLP_USERDATA);

				if( ptr != IntPtr.Zero ) {
					sample = (DXApp)((GCHandle)IntPtr.Zero).Target;
				}
			}

			switch( message ) {
				case (uint)WindowMessage.WM_CREATE: {
					CreateStruct createStruct = Marshal.PtrToStructure<CreateStruct>(lParam);
					PInvoke.SetWindowLong( hWnd,(WINDOW_LONG_PTR_INDEX) WindowLongParam.GWLP_USERDATA, 
											  (int)GCHandle.ToIntPtr( GCHandle.Alloc( createStruct.lpCreateParams ) ) );
					return (LRESULT)0;
				}

				case (uint)WindowMessage.WM_KEYDOWN:
					sample?.OnKeyDown( (byte)wParam.Value );
					return (LRESULT)0;


				case (uint)WindowMessage.WM_KEYUP:
					sample?.OnKeyUp( (byte)wParam.Value );
					return (LRESULT)0;

				case (uint)WindowMessage.WM_PAINT:
					if( sample != null ) {
						sample.OnUpdate();
						sample.OnRender();
					}
					return (LRESULT)0;

				case (uint)WindowMessage.WM_DESTROY:
					PInvoke.PostQuitMessage( 0 );
					return (LRESULT)0;
			}

			// Handle any messages the switch statement didn't.
			return PInvoke.DefWindowProc( hWnd, message, wParam, lParam );
		}
	}

}
