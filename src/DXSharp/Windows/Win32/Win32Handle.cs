#region Using Directives
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Gdi ;
using Windows.Win32.UI.Input ;
using Windows.Win32.UI.WindowsAndMessaging ;
using DXSharp.Windows.COM ;
#endregion

namespace DXSharp.Windows.Win32 ;


/// <summary>Represents any arbitrary pointer or Win32-style "handle".</summary>
public readonly partial struct Win32Handle {
	public readonly nint Value ;
	
	#region Constructors
	public Win32Handle( nint    value ) => Value = value ;
	public Win32Handle( HWND    value ) => Value = (nint)value ;
	public Win32Handle( HICON   value ) => Value = (nint)value ;
	public Win32Handle( HMENU   value ) => Value = (nint)value ;
	public Win32Handle( HHOOK   value ) => Value = (nint)value ;
	public Win32Handle( HBRUSH  value ) => Value = (nint)value ;
	public Win32Handle( HMODULE value ) => Value = (nint)value ;
	public Win32Handle( HCURSOR value ) => Value = (nint)value ;
	public Win32Handle( HGDIOBJ value ) => Value = (nint)value ;
	public Win32Handle( HMONITOR  value ) => Value = (nint)value ;
	public Win32Handle( HINSTANCE value ) => Value = (nint)value ;
	public Win32Handle( HRAWINPUT value ) => Value = (nint)value ;
	
	//public Win32Handle( HICONSM value ) => Value = (nint)value ; // ???
	#endregion

	#region Implicit Conversions
	public static implicit operator nint( Win32Handle             handle ) => handle.Value ;
	public static implicit operator Win32Handle( nint             value )  => new( value ) ;
	public static implicit operator HWND( Win32Handle             handle ) => (HWND)handle.Value ;
	public static implicit operator Win32Handle( HWND             value )  => new( value ) ;
	public static implicit operator HICON( Win32Handle            handle ) => (HICON)handle.Value ;
	public static implicit operator Win32Handle( HICON            value )  => new( value ) ;
	public static implicit operator HMENU( Win32Handle            handle ) => (HMENU)handle.Value ;
	public static implicit operator Win32Handle( HMENU            value )  => new( value ) ;
	public static implicit operator HHOOK( Win32Handle            handle ) => (HHOOK)handle.Value ;
	public static implicit operator Win32Handle( HHOOK            value )  => new( value ) ;
	public static implicit operator HBRUSH( Win32Handle           handle ) => (HBRUSH)handle.Value ;
	public static implicit operator Win32Handle( HBRUSH           value )  => new( value ) ;
	public static implicit operator HMODULE( Win32Handle          handle ) => (HMODULE)handle.Value ;
	public static implicit operator Win32Handle( HMODULE          value )  => new( value ) ;
	public static implicit operator HCURSOR( Win32Handle          handle ) => (HCURSOR)handle.Value ;
	public static implicit operator Win32Handle( HCURSOR          value )  => new( value ) ;
	public static implicit operator HGDIOBJ( Win32Handle          handle ) => (HGDIOBJ)handle.Value ;
	public static implicit operator Win32Handle( HGDIOBJ          value )  => new( value ) ;
	public static implicit operator HMONITOR( Win32Handle         handle ) => (HMONITOR)handle.Value ;
	public static implicit operator Win32Handle( HMONITOR         value )  => new( value ) ;
	public static implicit operator HINSTANCE( Win32Handle        handle ) => (HINSTANCE)handle.Value ;
	public static implicit operator Win32Handle( HINSTANCE        value )  => new( value ) ;
	public static implicit operator HRAWINPUT( Win32Handle        handle ) => (HRAWINPUT)handle.Value ;
	public static implicit operator Win32Handle( HRAWINPUT        value )  => new( value ) ;
	public static implicit operator Win32Handle( ComPtr           value )  => new( value.IUnknownAddress ) ;
	public static implicit operator ComPtr( Win32Handle           handle ) => new( handle.Value ) ;
	public static implicit operator Win32Handle( ComPtr<IUnknownWrapper> value )  => new( value.IUnknownAddress ) ;
	public static implicit operator ComPtr<IUnknownWrapper>( Win32Handle handle ) => new( handle.Value ) ;
	
	//public static implicit operator HICONSM( Win32Handle handle ) => (HICONSM)handle.Value ;
	//public static implicit operator Win32Handle( HICONSM value ) => new( value ) ;
	#endregion
} ;