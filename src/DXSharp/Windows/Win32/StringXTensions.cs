#region Using Directives
using System;
using System.Runtime.InteropServices;
using System.Text;
using Windows;
using Windows.Win32;
using Windows.Win32.Foundation;
#endregion

namespace DXSharp.Windows.Win32;

public static class StringXTensions  {
	/// <summary>
	/// Converts a string to a PWSTR.
	/// </summary>
	/// <param name="str">The string to convert.</param>
	/// <returns>A PWSTR that points to the string.</returns>
	public static unsafe PWSTR ToPWSTR( this string str ) {
		fixed ( char* p = str ) 
			return new PWSTR(p);
	}

	/// <summary>
	/// Converts a string to a PSTR.
	/// </summary>
	/// <param name="str">The string to convert.</param>
	/// <returns>A PSTR that points to the string.</returns>
	public static unsafe PSTR ToPSTR( this string str ) {
		byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
		fixed (byte* p = bytes) {
			return new PSTR(p);
		}
	}

	/// <summary>
	/// Converts a string to a LPWSTR.
	/// </summary>
	/// <param name="str">The string to convert.</param>
	/// <returns>A LPWSTR that points to the string.</returns>
	public static unsafe PWSTR ToLPWSTR( this string str ) {
		fixed (char* p = str) {
			return new PWSTR(p);
		}
	}

	/// <summary>
	/// Converts a string to a LPSTR.
	/// </summary>
	/// <param name="str">The string to convert.</param>
	/// <returns>A LPSTR that points to the string.</returns>
	public static unsafe PSTR ToLPSTR( this string str ) {
		byte[] bytes = Encoding.ASCII.GetBytes(str);
		fixed (byte* p = bytes) {
			return new PSTR(p);
		}
	}
} ;
