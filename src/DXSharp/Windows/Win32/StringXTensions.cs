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
	
	public static NativeStr32  ToNativeStr32( this string str ) => new( str ) ;
	public static NativeStr128 ToNativeStr128( this string str ) => new( str ) ;
} ;
