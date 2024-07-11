using System.Runtime.InteropServices;
using Windows.Win32.UI.HiDpi ;

namespace DXSharp.Windows.Win32 ;

//! TODO: Review and finish the incomplete AI-generated definitions of Win32 enumerations and structures ...

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct CreateStruct {
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
} ;
