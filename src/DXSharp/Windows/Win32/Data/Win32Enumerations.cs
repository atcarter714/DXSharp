#pragma warning disable CA1069
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using Windows.Win32 ;
using Windows.Win32.UI.HiDpi ;
namespace DXSharp.Windows.Win32 ;


//! NOTE: Several functions re-use the same flags or the same value #define'd with different prefixes.
//  We will eventually consolidate these into a single enum, but for now we will keep them in their Win32 form.

/// <summary>
/// Flags for using native Win32 API functions such as
/// <see cref="PInvoke.GetWindowLong"/> and <see cref="PInvoke.SetWindowLong"/>.
/// </summary>
[EquivalentOf("GWL_*", "GetWindowLong*")]
[EquivalentOf("GWLP_*", "GetWindowLongPtr*")]
[NativeLibrary("user32.dll", "!", "winuser.h")]
public enum WindowLongParam {
	/// <summary>
	/// Retrieves the user data associated with the window.
	/// This data is intended for use by the application that created the window.
	/// Its value is initially zero.
	/// </summary>
	GWLP_USERDATA   = -21,
	/// <summary>
	/// Flags to get/set the <a href="https://learn.microsoft.com/en-us/windows/desktop/winmsg/extended-window-styles">extended window styles</a>.
	/// </summary>
	GWL_EXSTYLE     = -20,
	/// <summary>Retrieves a handle to the application instance.</summary>
	GWLP_HINSTANCE  = -6,
	/// <summary>Retrieves a handle to the parent window, if there is one.</summary>
	GWLP_HWNDPARENT = -8,
	/// <summary>Retrieves the identifier of the window.</summary>
	GWL_ID          = -12,
	/// <summary>
	/// Retrieves the <a href="https://learn.microsoft.com/en-us/windows/desktop/winmsg/window-styles">window styles</a>.
	/// </summary>
	GWL_STYLE       = -16,
	/// <summary>
	/// Retrieves the user data associated with the window.
	/// This data is intended for use by the application that created the window.
	/// Its value is initially zero.
	/// </summary>
	GWL_USERDATA    = -21,
	/// <summary>
	/// Retrieves the address of the window procedure, or a handle representing the address of the window procedure.
	/// You must use the CallWindowProc function to call the window procedure.
	/// </summary>
	GWL_WNDPROC     = -4,
	/// <summary>Retrieves the return value of a message processed in the dialog box procedure.</summary>
	DWLP_MSGRESULT  = 0,
	/// <summary>
	/// Retrieves the pointer to the dialog box procedure, or a handle representing the pointer to the dialog box procedure.
	/// You must use the CallWindowProc function to call the dialog box procedure.
	/// </summary>
	DWLP_DLGPROC    = 4,
	/// <summary>Retrieves extra information private to the application, such as handles or pointers.</summary>
	DWLP_USER       = 8,
} ;


[Flags] public enum WindowStyles: uint {
	WS_OVERLAPPED       = 0x00000000,
	WS_POPUP            = 0x80000000,
	WS_CHILD            = 0x40000000,
	WS_MINIMIZE         = 0x20000000,
	WS_VISIBLE          = 0x10000000,
	WS_DISABLED         = 0x08000000,
	WS_CLIPSIBLINGS     = 0x04000000,
	WS_CLIPCHILDREN     = 0x02000000,
	WS_MAXIMIZE         = 0x01000000,
	WS_CAPTION          = 0x00C00000,
	WS_BORDER           = 0x00800000,
	WS_DLGFRAME         = 0x00400000,
	WS_VSCROLL          = 0x00200000,
	WS_HSCROLL          = 0x00100000,
	WS_SYSMENU          = 0x00080000,
	WS_THICKFRAME       = 0x00040000,
	WS_GROUP            = 0x00020000,
	WS_TABSTOP          = 0x00010000,
	WS_MINIMIZEBOX      = 0x00020000,
	WS_MAXIMIZEBOX      = 0x00010000,
	WS_TILED            = WS_OVERLAPPED,
	WS_ICONIC           = WS_MINIMIZE,
	WS_SIZEBOX          = WS_THICKFRAME,
	WS_TILEDWINDOW      = WS_OVERLAPPEDWINDOW,
	WS_OVERLAPPEDWINDOW = ( WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX ),
	WS_POPUPWINDOW      = ( WS_POPUP | WS_BORDER | WS_SYSMENU ),
	WS_CHILDWINDOW      = WS_CHILD,
} ;


public enum WindowPositions {
	HWND_TOP       = 0,
	HWND_BOTTOM    = 1,
	HWND_TOPMOST   = -1,
	HWND_NOTOPMOST = -2,
	CW_USEDEFAULT  = unchecked( (int)0x80000000 )
} ;


/// <summary>Defines constants for the Win32 cursor types.</summary>
public enum CursorType {
	IDC_ARROW       = 32512,
	IDC_IBEAM       = 32513,
	IDC_WAIT        = 32514,
	IDC_CROSS       = 32515,
	IDC_UPARROW     = 32516,
	IDC_SIZE        = 32640,
	IDC_ICON        = 32641,
	IDC_SIZENWSE    = 32642,
	IDC_SIZENESW    = 32643,
	IDC_SIZEWE      = 32644,
	IDC_SIZENS      = 32645,
	IDC_SIZEALL     = 32646,
	IDC_NO          = 32648,
	IDC_HAND        = 32649,
	IDC_APPSTARTING = 32650,
	IDC_HELP        = 32651,
} ;


/// <summary>
/// Flags for the native Win32 API function <see cref="PInvoke.ShowWindow"/>.
/// </summary>
[Flags] public enum ShowWindowCommands {
	/// <summary>Hides the window and activates another window.</summary>
	SW_HIDE             = 0,
	/// <summary>
	/// Activates and displays a window. If the window is minimized, maximized,
	/// or arranged, the system restores it to its original size and position.
	/// An application should specify this flag when displaying the window for
	/// the first time.
	/// </summary>
	SW_SHOWNORMAL       = 1,
	/// <inheritdoc cref="SW_SHOWNORMAL"/>
	SW_NORMAL           = 1,
	/// <summary>Activates the window and displays it as a minimized window.</summary>
	SW_SHOWMINIMIZED    = 2,
	/// <summary>Activates the window and displays it as a maximized window.</summary>
	SW_SHOWMAXIMIZED    = 3,
	/// <inheritdoc cref="SW_SHOWMAXIMIZED"/>
	SW_MAXIMIZE         = 3,
	/// <summary>
	/// Displays a window in its most recent size and position. This value is similar to
	/// SW_SHOWNORMAL, except that the window is not activated.
	/// </summary>
	SW_SHOWNOACTIVATE   = 4,
	/// <summary>Activates the window and displays it in its current size and position.</summary>
	SW_SHOW             = 5,
	/// <summary>Minimizes the specified window and activates the next top-level window in the Z order.</summary>
	SW_MINIMIZE         = 6,
	/// <summary>Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.</summary>
	SW_SHOWMINNOACTIVE  = 7,
	/// <summary>Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated.</summary>
	SW_SHOWNA           = 8,
	/// <summary>
	/// Activates and displays the window. If the window is minimized, maximized, or arranged, the system restores it to its original size and position.
	/// An application should specify this flag when restoring a minimized window.
	/// </summary>
	SW_RESTORE          = 9,
	/// <summary>
	/// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
	/// </summary>
	SW_SHOWDEFAULT      = 10,
	/// <summary>
	/// Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.
	/// </summary>
	SW_FORCEMINIMIZE    = 11,
} ;


/// <summary>
/// Flags for the native Win32 API function <see cref="PInvoke.SetWindowPos"/>.
/// </summary>
[Flags] public enum SetWindowPosFlags: uint {
    /// <summary>
    /// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
    /// </summary>
    SWP_ASYNCWINDOWPOS = 0x4000,

    /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
    SWP_DEFERERASE = 0x2000,

    /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
    SWP_DRAWFRAME = 0x0020,

    /// <summary>
    /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
    /// </summary>
    SWP_FRAMECHANGED = 0x0020,

    /// <summary>Hides the window.</summary>
    SWP_HIDEWINDOW = 0x0080,

    /// <summary>
    /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
    /// </summary>
    SWP_NOACTIVATE = 0x0010,

    /// <summary>
    /// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
    /// </summary>
    SWP_NOCOPYBITS = 0x0100,

    /// <summary>Retains the current position (ignores X and Y parameters).</summary>
    SWP_NOMOVE = 0x0002,

    /// <summary>Does not change the owner window's position in the Z order.</summary>
    SWP_NOOWNERZORDER = 0x0200,

    /// <summary>
    /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
    /// </summary>
    SWP_NOREDRAW = 0x0008,

    /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
    SWP_NOREPOSITION = 0x0200,

    /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
    SWP_NOSENDCHANGING = 0x0400,

    /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
    SWP_NOSIZE = 0x0001,

    /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
    SWP_NOZORDER = 0x0004,

    /// <summary>Shows the window.</summary>
    SWP_SHOWWINDOW = 0x0040,
} ;


/// <summary>
/// Identifies dots per inch (dpi) awareness values. DPI awareness indicates how much
/// scaling work an application performs for DPI versus how much is done by the system.
/// </summary>
/// <remarks>
/// <para>Users have the ability to set the DPI scale factor on their displays independent of each other.
/// Some legacy applications are not able to adjust their scaling for multiple DPI settings.
/// In order for users to use these applications without content appearing too large or small on displays,
/// Windows can apply DPI virtualization to an application, causing it to be automatically scaled by the
/// system to match the DPI of the current display. The PROCESS_DPI_AWARENESS value indicates what level of
/// scaling your application handles on its own and how much is provided by Windows. Keep in mind that
/// applications scaled by the system may appear blurry and will read virtualized data about the monitor
/// to maintain compatibility.</para>
/// <para><b>Important:</b></para>
/// <para>
/// Previous versions of Windows required you to set the DPI awareness for the entire application.
/// Now the DPI awareness is tied to individual threads, processes, or windows. This means that the
/// DPI awareness can change while the app is running and that multiple windows can have their own
/// independent DPI awareness values. See DPI_AWARENESS for more information about how DPI awareness
/// currently works. The recommendations below about setting the DPI awareness in the application
/// manifest are still supported, but the current recommendation is to use the
/// <see cref="DPI_AWARENESS_CONTEXT"/>.
/// </para>
/// </remarks>
public enum PROCESS_DPI_AWARENESS {
	/// <summary>
	/// DPI unaware: This app does not scale for DPI changes and is always assumed to
	/// have a scale factor of 100% (96 DPI). It will be automatically scaled by the
	/// system on any other DPI setting.
	/// </summary>
	PROCESS_DPI_UNAWARE           = 0,
	/// <summary>
	/// System DPI aware: This app does not scale for DPI changes. It will query for the
	/// DPI once and use that value for the lifetime of the app. If the DPI changes, the
	/// app will not adjust to the new DPI value. It will be automatically scaled up or
	/// down by the system when the DPI changes from the system value.
	/// </summary>
	PROCESS_SYSTEM_DPI_AWARE      = 1,
	/// <summary>
	/// Per monitor DPI aware: This app checks for the DPI when it is created and adjusts the
	/// scale factor whenever the DPI changes. These applications are not automatically scaled
	/// by the system.
	/// </summary>
	PROCESS_PER_MONITOR_DPI_AWARE = 2,
} ;


// Header: Winuser.h
[Flags] public enum WMSizeParams: uint {
	/// <summary>The window has been resized, but neither the <b><c>SIZE_MINIMIZED</c></b> nor <b><c>SIZE_MAXIMIZED</c></b> value applies.</summary>
	Restored = 0,
	
	/// <summary>The window has been minimized.</summary>
	Minimized = 1,

	/// <summary>The window has been maximized.</summary>
	Maximized = 2,

	/// <summary>Message is sent to all pop-up windows when some other window has been restored to its former size.</summary>
	MaxShow = 3,

	/// <summary>Message is sent to all pop-up windows when some other window is maximized.</summary>
	MaxHide = 4,
} ;