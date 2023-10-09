#region Using Directives
#pragma warning disable CS1591, CS1573, CS0465, CS0649, CS8019, CS1570, CS1584, CS1658, CS0436, CS8981

using Windows.Win32.Foundation ;
using global::System;
using global::System.Diagnostics;
using global::System.Diagnostics.CodeAnalysis;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.Versioning;

using winmdroot = global::Windows.Win32;
#endregion


namespace DXSharp.Windows.Win32 {
	[DebuggerDisplay( "{Value}" )]
	public readonly struct HWnd {
		public static readonly HWnd Null = new(0x00000000) ;
		
		public readonly nint Value ;
		public HWnd( nint value ) => Value = value ;
		
		public static implicit operator nint( HWnd value ) => value.Value ;
		public static implicit operator HWnd( nint value ) => new( value ) ;
		public static implicit operator HWnd( HWND value ) => new( value.Value ) ;
		public static implicit operator HWND( HWnd value ) => new( value.Value ) ;
		public static bool operator ==( HWnd left, HWnd right ) => left.Value == right.Value ;
		public static bool operator !=( HWnd left, HWnd right ) => left.Value != right.Value ;
		public static bool operator ==( HWnd left, HWND right ) => left.Value == right.Value ;
		public static bool operator !=( HWnd left, HWND right ) => left.Value != right.Value ;
		public static bool operator ==( HWND left, HWnd right ) => left.Value == right.Value ;
		public static bool operator !=( HWND left, HWnd right ) => left.Value != right.Value ;

		public override int GetHashCode( ) => this.Value.GetHashCode( ) ;
		public bool Equals( HWnd other ) => this.Value == other.Value ;
		public override bool Equals( object? obj ) =>
			obj is HWnd other && Equals( other ) ;
		
		public Win32Handle ToWin32Handle( ) => new( this.Value ) ;
	} ;
}

namespace Windows.Win32.Foundation {
	/// <summary>
	/// A handle to a window or control
	/// </summary>
	[DebuggerDisplay( "{Value}" )]
	public readonly struct HWND: IEquatable< HWND > {
		/// <summary>The internal pointer value of this HWND.</summary>
		public readonly nint Value ;
		/// <summary>Equivalent to a null (0) pointer.</summary>
		public static HWND Null => default ;
		/// <summary>Indicates if this HWND is null.</summary>
		public bool IsNull => Value == default ;
		
		
		/// <summary>Creates a new HWND from a pointer value.</summary>
		/// <param name="value">Pointer value</param>
		public HWND( nint value ) => this.Value = value ;
		
		public static bool operator !=( HWND left, HWND right ) => !( left == right ) ;
		public static bool operator ==( HWND left, HWND right ) => left.Value == right.Value ;
		public static implicit operator nint( HWND value ) => value.Value ;
		public static implicit operator HWND( nint value ) => new HWND( value ) ;

		/// <summary>
		/// Determines if the given HWND is equal to this one
		/// </summary>
		/// <param name="other">HWND to compare</param>
		/// <returns>True if they are equal, otherwise false</returns>
		public bool Equals( HWND other ) => this.Value == other.Value ;

		/// <summary>
		/// Determines if the given object and this HWND value are equal
		/// </summary>
		/// <param name="obj">Object to compare to</param>
		/// <returns>True if equal, otherwise false</returns>
		public override bool Equals( object obj ) =>
									obj is HWND other && this.Equals( other ) ;

		/// <summary>
		/// Gets the hash code of this HWND value
		/// </summary>
		/// <returns>Int32 hash code</returns>
		public override int GetHashCode() => this.Value.GetHashCode( ) ;
	} ;
}
