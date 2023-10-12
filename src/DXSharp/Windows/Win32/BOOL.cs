#region Using Directives
#pragma warning disable CS1591, CS1573, CS0465, CS0649, CS8019, CS1570, CS1584, CS1658, CS0436, CS8981

using global::System;
using global::System.Diagnostics;
using global::System.Diagnostics.CodeAnalysis;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.Versioning;

using winmdroot = global::Windows.Win32;
#endregion

// WinMD Implementation:
// (Windows.Win32.Foundation)
namespace Windows.Win32.Foundation {
	[DebuggerDisplay( "{Value} ({ToString( )})" )]
	public readonly struct BOOL: IEquatable< BOOL >, IEquatable< bool > {
		const short _MAXOPT_ = (0x100 | 0x200) ;
		
		/// <summary>The internal value of the Win32 BOOL</summary>
		public readonly int Value ;

		public BOOL( int  value ) => this.Value = value ;
		public BOOL( bool value ) => this.Value = value ? 1 : 0 ;

		[MethodImpl(_MAXOPT_)] public static implicit operator bool( BOOL value ) => value.Value != 0 ;
		[MethodImpl(_MAXOPT_)] public static implicit operator BOOL( bool value ) => new( value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator BOOL( int value ) => new( value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator int( BOOL value ) => value.Value ;
		[MethodImpl(_MAXOPT_)] public static bool operator ==( BOOL left, BOOL right ) => left.Value == right.Value ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( BOOL left, BOOL right ) => left.Value != right.Value ;
		[MethodImpl(_MAXOPT_)] public static bool operator ==( BOOL left, bool right ) => left.Value == ( right ? 1 : 0 ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( BOOL left, bool right ) => left.Value != ( right ? 1 : 0 ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator ==( bool left, BOOL right ) => ( left ? 1 : 0 ) == right.Value ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( bool left, BOOL right ) => ( left ? 1 : 0 ) != right.Value ;

		/// <summary>
		/// Determines if the given BOOL is equal to this value
		/// </summary>
		/// <param name="other">BOOL to compare to</param>
		/// <returns>True if equal, otherwise false</returns>
		public bool Equals( BOOL other ) => this.Value == other.Value ;

		public bool Equals( bool other ) =>
			other ? this.Value is not 0 : this.Value is 0 ;

		/// <summary>
		/// Determines if the given object is equal to this value
		/// </summary>
		/// <param name="obj">object to compare to</param>
		/// <returns>True if equal, otherwise false</returns>
		public override bool Equals( object? obj ) => obj is BOOL other && this.Equals( other ) ;
		
		/// <inheritdoc cref="bool.GetHashCode"/>
		public override int GetHashCode( ) => Value ;

		/// <summary>Returns the string representation of this boolean (BOOL) value.</summary>
		public override string ToString( ) => this.Value != 0 ? _TRUE_ : _FALSE_ ;
		const string _TRUE_ = "TRUE", _FALSE_ = "FALSE" ;
	} ;
}
