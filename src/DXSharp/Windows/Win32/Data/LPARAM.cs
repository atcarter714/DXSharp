#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981

#region Using Directives
using System.Diagnostics ;
using System.Runtime.CompilerServices ;

using DXSharp.Windows.Win32 ;
using static DXSharp.InteropUtils ;
#endregion


namespace DXSharp.Windows.Win32 {
	[DebuggerDisplay( "{Value}" )]
	public readonly partial struct LParam: IEquatable< LParam > {
		public readonly nint Value ;
		public LParam( nint value )  => this.Value = value ;
		public LParam( int value )   => this.Value = (nint)value ;
		public LParam( uint value )  => this.Value = (nint)value ;
		public LParam( long value )  => this.Value = (nint)value ;
		public LParam( ulong value ) => this.Value = (nint)value ;
		public LParam( nuint value ) => this.Value = (nint)value ;

		public override bool Equals( object? obj ) => 
						obj is LParam other && this.Equals( other ) ;
		public bool Equals( LParam other ) => this.Value == other.Value ;
		public override int GetHashCode( ) => this.Value.GetHashCode( ) ;
		
		public ushort HiWord  => MathBin.HIWORD( (nuint)this.Value ) ;
		public ushort LoWord  => MathBin.LOWORD( (nuint)this.Value ) ;
		
		[MethodImpl(_MAXOPT_)] public static unsafe implicit operator LParam( void* value ) => new LParam( (nint)value ) ;
		[MethodImpl(_MAXOPT_)] public static unsafe implicit operator void*( LParam value ) => (void*)value.Value ;
		[MethodImpl(_MAXOPT_)] public static implicit operator LParam( nint value ) => new LParam( value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator nint( LParam value ) => value.Value ;
		[MethodImpl(_MAXOPT_)] public static implicit operator LParam( long value ) => new LParam( (nint)value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator LParam( int value ) => new LParam( (nint)value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator LParam( short value ) => new LParam( (nint)value ) ;
		
		[MethodImpl(_MAXOPT_)] public static explicit operator LParam( nuint value ) => new LParam( (nint)value ) ;
		[MethodImpl(_MAXOPT_)] public static explicit operator LParam( ulong value ) => new LParam( (nint)value ) ;
		[MethodImpl(_MAXOPT_)] public static explicit operator LParam( uint value ) => new LParam( (nint)value ) ;
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( LParam left, LParam right ) => left.Value == right.Value ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( LParam left, LParam right ) => !( left == right ) ;
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( LParam left, nint   right ) => left.Value == right ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( LParam left, nint   right ) => !( left == right ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator ==( nint   left, LParam right ) => left == right.Value ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( nint   left, LParam right ) => !( left == right ) ;
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( LParam left, long   right ) => left.Value == right ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( LParam left, long   right ) => left.Value != right ;
		[MethodImpl(_MAXOPT_)] public static bool operator ==( long   left, LParam right ) => left == right.Value ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( long   left, LParam right ) => left != right.Value ;
		[MethodImpl(_MAXOPT_)] public static bool operator ==( LParam left, int    right ) => left.Value == right ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( LParam left, int    right ) => left.Value != right ;
	} ;
}


namespace Windows.Win32.Foundation {
	[DebuggerDisplay( "{Value}" )]
	public readonly struct LPARAM: IEquatable< LPARAM > {
		public readonly nint Value ;
		public LPARAM( nint value ) => this.Value = value ;
		
		public override bool Equals( object? obj ) => obj is LPARAM other && this.Equals( other ) ;
		public bool Equals( LPARAM  other ) => this.Value == other.Value ;
		public override int  GetHashCode( ) => this.Value.GetHashCode( ) ;
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( LPARAM left, LParam right ) =>  ( left.Value == right.Value ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator ==( LParam left, LPARAM right ) =>  ( left.Value == right.Value ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator ==( LPARAM left, LPARAM right ) => left.Value == right.Value ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( LPARAM left, LPARAM right ) => !( left == right ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( LPARAM left, LParam right ) => !( left == right ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( LParam left, LPARAM right ) => !( left == right ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator ==( LPARAM left, nint   right ) =>  ( left.Value == right ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator ==( nint   left, LPARAM right ) =>  ( left == right.Value ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( LPARAM left, nint   right ) => !( left == right ) ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( nint   left, LPARAM right ) => !( left == right ) ;
		
		[MethodImpl(_MAXOPT_)] public static implicit operator LPARAM( LParam value ) => new( value.Value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator LPARAM( nuint value ) => new( (nint)value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator LPARAM( nint value ) => new LPARAM( value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator LPARAM( ulong value ) => new( (nint)value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator LPARAM( long value ) => new( (nint)value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator LPARAM( uint value ) => new( (nint)value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator LPARAM( int value ) => new( (nint)value ) ;
		
		[MethodImpl(_MAXOPT_)] public static implicit operator LParam( LPARAM value ) => new( value.Value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator nuint( LPARAM value ) => (nuint)value.Value ;
		[MethodImpl(_MAXOPT_)] public static implicit operator ulong( LPARAM value ) => (ulong)value.Value ;
		[MethodImpl(_MAXOPT_)] public static implicit operator long( LPARAM value ) => (long)value.Value ;
		[MethodImpl(_MAXOPT_)] public static explicit operator uint( LPARAM value ) => (uint)value.Value ;
		[MethodImpl(_MAXOPT_)] public static explicit operator int( LPARAM value ) => (int)value.Value ;
		[MethodImpl(_MAXOPT_)] public static implicit operator nint( LPARAM value ) => value.Value ;
	} ;
}