#region Using Directives
#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using winmdroot = Windows.Win32;
#endregion


namespace DXSharp.Windows.Win32 {
	[DebuggerDisplay( "{Value}" )]
	public readonly partial struct LParam: IEquatable< LParam > {
		public readonly nint Value ;
		public LParam( nint value ) => this.Value = value ;

		public static implicit operator nint( LParam value ) => value.Value ;
		public static implicit operator LParam( nint value ) => new LParam( value ) ;
		public static bool operator ==( LParam left, LParam right ) => left.Value == right.Value ;
		public static bool operator !=( LParam left, LParam right ) => !( left == right ) ;

		public override int GetHashCode( ) => this.Value.GetHashCode( ) ;
		public bool Equals( LParam other ) => this.Value == other.Value ;
		public override bool Equals( object? obj ) => obj is LParam other && this.Equals( other ) ;
	} ;
}

namespace Windows.Win32.Foundation {
	[DebuggerDisplay( "{Value}" )]
	public readonly struct LPARAM: IEquatable< LPARAM > {
		public readonly nint Value ;
		public LPARAM( nint value ) => this.Value = value ;

		public static implicit operator nint( LPARAM value ) => value.Value ;
		public static implicit operator LPARAM( nint value ) => new LPARAM( value ) ;
		public static bool operator ==( LPARAM left, LPARAM right ) => left.Value == right.Value ;
		public static bool operator !=( LPARAM left, LPARAM right ) => !( left == right ) ;

		public bool Equals( LPARAM other ) => this.Value == other.Value ;
		public override bool Equals( object obj ) => obj is LPARAM other && this.Equals( other ) ;
		public override int GetHashCode() => this.Value.GetHashCode( ) ;
	} ;
}