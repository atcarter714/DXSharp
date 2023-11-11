#region Using Directives
using System.Diagnostics ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;

using DXSharp ;
using DXSharp.Windows.Win32 ;
using static DXSharp.InteropUtils ;
#endregion


namespace DXSharp.Windows.Win32 {
	[StructLayout(LayoutKind.Sequential, Size = SizeInBytes)]
	[EquivalentOf(typeof(WPARAM)), EquivalentOf(typeof(nuint))]
	public readonly partial struct WParam: IEquatable< WParam > {
		#region Constants & ReadOnly Static Members
#if __X64__ || __AMD64__ || _64BIT
		public const int SizeInBytes = 8 ;
#else
		public const int SizeInBytes = 4 ;
#endif
#if DEBUG || DEV_BUILD
		static WParam( ) {
#if _64BIT
			int wpSize = -1 ;
			unsafe { wpSize = sizeof(WParam) ; }
			Debug.Assert( wpSize is 8 && nuint.Size is 8 ) ;
#endif
		}
#endif
		#endregion
		
		readonly nuint _value ;
		public WParam( nuint value ) => _value = value ;
		public WParam( nint value )  => _value = (nuint)value ;
		public WParam( long value )  => _value = (nuint)value ;
		public WParam( ulong value ) => _value = (nuint)value ;
		public WParam( int value )   => _value = (nuint)value ;
		public WParam( uint value )  => _value = value ;
		
		
		public ushort HiWord  => MathBin.HIWORD( (nuint)this._value ) ;
		public ushort LoWord  => MathBin.LOWORD( (nuint)this._value ) ;
		
		public override int GetHashCode( ) => _value.GetHashCode( ) ;
		public bool Equals( WParam other ) => _value == other._value ;
		public override bool Equals( object? obj ) => obj is WParam other && other._value == _value
														|| obj is nuint  other2 && other2 == _value ;
		
		[MethodImpl(_MAXOPT_)] public static explicit operator WParam( int value ) => new( value ) ;
		[MethodImpl(_MAXOPT_)] public static explicit operator WParam( short value ) => new( value ) ;
		[MethodImpl(_MAXOPT_)] public static explicit operator WParam( nint value ) => new( value ) ;
		[MethodImpl(_MAXOPT_)] public static explicit operator WParam( LParam value ) => new( (nuint)value.Value ) ;
		
		[MethodImpl(_MAXOPT_)] public static explicit operator LParam( WParam value ) => new( (nint)value._value ) ;
		[MethodImpl(_MAXOPT_)] public static explicit operator nint( WParam value ) => (nint)value._value ;
		
		[MethodImpl(_MAXOPT_)] public static implicit operator WParam( nuint value ) => new( value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator nuint( WParam value ) => (nuint)value._value ;
		
		[MethodImpl(_MAXOPT_)] public static implicit operator WParam( byte value ) => new( value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator WParam( ushort value ) => new( value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator WParam( uint value ) => new( value ) ;
		[MethodImpl(_MAXOPT_)] public static implicit operator WParam( ulong value ) => new( value ) ;
		
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( WParam left, WParam right ) => left._value == right._value ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( WParam left, WParam right ) => !( left == right ) ;
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( WParam left, nuint right ) => left._value == right ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( WParam left, nuint right ) => !( left == right ) ;
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( nuint left, WParam right ) => left == right._value ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( nuint left, WParam right ) => !( left == right ) ;
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( WParam left, nint right ) => left._value == (nuint)right ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( WParam left, nint right ) => left._value != (nuint)right ;
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( nint left, WParam right ) => left == (nint)right._value ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( nint left, WParam right ) => left != (nint)right._value ;
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( WParam left, int right ) => left._value == (nuint)right ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( WParam left, int right ) => left._value != (nuint)right ;
		
		[MethodImpl(_MAXOPT_)] public static bool operator ==( int left, WParam right ) => left == (int)right._value ;
		[MethodImpl(_MAXOPT_)] public static bool operator !=( int left, WParam right ) => left != (int)right._value ;
	} ;
}


namespace Windows.Win32.Foundation {
	[CsWin32, EquivalentOf(typeof(WParam)), 
	 EquivalentOf(typeof(nuint))]
	public readonly struct WPARAM {
		public readonly nuint Value ;
		public WPARAM( nuint value ) => Value = value ;
		
		public static implicit operator WPARAM( WParam value ) => new( value ) ;
		public static implicit operator WParam( WPARAM value ) => new( value.Value ) ;
		public static implicit operator nuint(  WPARAM value ) => (nuint)value.Value ;
		public static implicit operator WPARAM( nuint value ) => new WPARAM( value ) ;
		public static implicit operator LPARAM( WPARAM value ) => new LPARAM( (nint)value.Value ) ;
		public static implicit operator WPARAM( LPARAM value ) => new WPARAM( (nuint)value.Value ) ;
	} ;
}