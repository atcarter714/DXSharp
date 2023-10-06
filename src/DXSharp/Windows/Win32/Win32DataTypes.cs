using System.Runtime.InteropServices;

namespace DXSharp.Windows.Win32;

//! TODO: Incomplete definitions generated via AI

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct CreateStruct
{
	public IntPtr lpCreateParams;
	public IntPtr hInstance;
	public IntPtr hMenu;
	public IntPtr hwndParent;
	public int    cy;
	public int    cx;
	public int    y;
	public int    x;
	public int    style;
	public string lpszName;
	public string lpszClass;
	public uint   dwExStyle;
};

public enum WindowLongParam
{
	GWLP_USERDATA	= -21,
	GWL_EXSTYLE     = -20,
	GWLP_HINSTANCE  = -6,
	GWLP_HWNDPARENT = -8,
	GWL_ID          = -12,
	GWL_STYLE       = -16,
	GWL_USERDATA    = -21,
	GWL_WNDPROC     = -4,
	DWLP_MSGRESULT  = 0,
	DWLP_DLGPROC    = 4,
	DWLP_USER       = 8
}

[Flags]
public enum WindowStyles : uint
{
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
	WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
	WS_POPUPWINDOW      = (WS_POPUP | WS_BORDER | WS_SYSMENU),
	WS_CHILDWINDOW      = WS_CHILD,
}

public enum WindowPositions
{
	HWND_TOP       = 0,
	HWND_BOTTOM    = 1,
	HWND_TOPMOST   = -1,
	HWND_NOTOPMOST = -2,
	CW_USEDEFAULT  = unchecked((int)0x80000000)
}

public enum CursorType
{
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
	IDC_HELP        = 32651
}
