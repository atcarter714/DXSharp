#region Using Directives
using System;
using System.Runtime.InteropServices;
using System.Text;
using Windows;
using Windows.Win32;
using Windows.Win32.Foundation;
using DXSharp.Windows.Win32.Helpers ;

#endregion

namespace DXSharp.Windows.Win32;

public static class StringXTensions  {
	/// <summary>
	/// Converts a string to a PWSTR.
	/// </summary>
	/// <param name="str">The string to convert.</param>
	/// <returns>A PWSTR that points to the string.</returns>
	public static unsafe PWSTR ToPWSTR( this string str ) {
		var globalStr = Marshal.StringToHGlobalUni( str ) ;
		PWSTR pStr = new( (char*)globalStr ) ;
		return pStr ;
	}

	/// <summary>
	/// Converts a string to a PSTR.
	/// </summary>
	/// <param name="str">The string to convert.</param>
	/// <returns>A PSTR that points to the string.</returns>
	public static unsafe PSTR ToPSTR( this string str ) {
		var globalStr = Marshal.StringToHGlobalAnsi( str ) ;
		PSTR pStr = new( (byte*)globalStr ) ;
		return pStr ;
	}
	
	/// <summary>
	/// Converts a string to a PCWSTR.
	/// <para>PCWSTR is a pointer to a constant null-terminated Unicode string.</para>
	/// <para>PCWSTR is declared in WinNT.h as:</para>
	/// <code>typedef CONST WCHAR *PCWSTR, *LPCWSTR;</code>
	/// </summary>
	public static PCWSTR ToPCWSTR( this string str ) => new( str ) ;

	public static FixedStr32  ToFixedStr32( this string str ) => new( str ) ;
	public static FixedStr128 ToFixedStr128( this string str ) => new( str ) ;
} ;
