﻿// ------------------------------------------------------------------------------
// <auto-generated>
//		This code was (partially) generated by CsWin32!
//		https://github.com/microsoft/CsWin32/
// </auto-generated>
// ------------------------------------------------------------------------------

#pragma warning disable CS1591, CS1573, CS0465, CS0649, CS8019, CS1570, CS1584, CS1658, CS0436, CS8981

#region Using Directives

using Windows.Win32.Foundation ;
using global::System;
using global::System.Diagnostics;
using global::System.Diagnostics.CodeAnalysis;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.Versioning;

using winmdroot = global::Windows.Win32;
#endregion


namespace DXSharp.Windows.Win32
{
	public readonly struct HInstance: IEquatable< HInstance > {
		readonly HINSTANCE Value ;
		
		public HInstance( IntPtr value ) => Value = (HINSTANCE)value ;
		
		public override int GetHashCode( ) => Value.GetHashCode( ) ;
		public bool Equals( HInstance other ) => Value.Equals( other.Value ) ;
		public override bool Equals( object obj ) => obj is HInstance other && Equals( other ) ;
		
		public static bool operator ==( HInstance left, HInstance right ) => left.Value == right.Value ;
		public static bool operator !=( HInstance left, HInstance right ) => !(left == right) ;
		public static implicit operator IntPtr( HInstance value ) => value.Value ;
		public static implicit operator HInstance( IntPtr value ) => new HInstance( value ) ;
		public static implicit operator HINSTANCE ( HInstance value ) => value.Value ;
		public static implicit operator HInstance ( HINSTANCE value ) => new HInstance( value ) ;
		public static implicit operator HMODULE ( HInstance value ) => (HMODULE)(IntPtr)value.Value ;
		public static implicit operator HInstance ( HMODULE value ) => new HInstance( value ) ;
	} ;
}

//! WinMDRoot Implementation: Windows.Win32.Foundation
namespace Windows.Win32.Foundation {

/// <summary>A Win32 handle equivalent to a pointer</summary>
[DebuggerDisplay( "{Value}" )]
public readonly struct HINSTANCE: IEquatable<HINSTANCE>
{
	/// <summary>
	/// The value of this HINSTANCE
	/// </summary>
	public readonly IntPtr Value;

	/// <summary>
	/// Creates a new HINSTANCE
	/// </summary>
	/// <param name="value">Value of the HINSTANCE</param>
	public HINSTANCE( IntPtr value ) => this.Value = value;

	/// <summary>
	/// A null value HINSTANCE
	/// </summary>
	public static readonly HINSTANCE Null = default;

	/// <summary>
	/// Indicates if this HINSTANCE value is a null handle/pointer
	/// </summary>
	public bool IsNull => (Value == Null);


	public static implicit operator IntPtr( HINSTANCE value ) => value.Value;
	public static explicit operator HINSTANCE( IntPtr value ) => new HINSTANCE( value );
	public static bool operator ==( HINSTANCE left, HINSTANCE right ) => left.Value == right.Value;
	public static bool operator !=( HINSTANCE left, HINSTANCE right ) => !(left == right);


	/// <summary>
	/// Determines if the given HINSTANCE is equal to this one
	/// </summary>
	/// <param name="other">HINSTANCE to compare</param>
	/// <returns>True if they are equal, otherwise false</returns>
	public bool Equals( HINSTANCE other ) => this.Value == other.Value;

	/// <summary>
	/// Determines if the given object is equal to this one
	/// </summary>
	/// <param name="obj">object to compare</param>
	/// <returns>True if they are equal, otherwise false</returns>
	public override bool Equals( object obj ) => obj is HINSTANCE other && this.Equals( other );

	/// <summary>
	/// Gets the hash code of this HINSTANCE value
	/// </summary>
	/// <returns>Int32 hash code</returns>
	public override int GetHashCode( ) => this.Value.GetHashCode( ) ;
};
}

