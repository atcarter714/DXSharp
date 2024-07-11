#region Using Directives
using System ;
using System.Numerics ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Intrinsics ;
using DXSharp ;
#endregion
namespace DXSharp.Math ;

/*
 typedef union __declspec(intrin_type) __declspec(align(16)) __m128 {
     float               m128_f32[4];
     unsigned __int64    m128_u64[2];
     __int8              m128_i8[16];
     __int16             m128_i16[8];
     __int32             m128_i32[4];
     __int64             m128_i64[2];
     unsigned __int8     m128_u8[16];
     unsigned __int16    m128_u16[8];
     unsigned __int32    m128_u32[4];
 } __m128;
 */

[StructLayout( LayoutKind.Explicit, Size = 16)]
public unsafe partial struct __m128 {
	[FieldOffset(0)] public fixed float  m128_f32[ 4 ] ;
	[FieldOffset(0)] public fixed ulong  m128_u64[ 2 ] ;
	[FieldOffset(0)] public fixed byte   m128_i8[ 16 ] ;
	[FieldOffset(0)] public fixed short  m128_i16[ 8 ] ;
	[FieldOffset(0)] public fixed int    m128_i32[ 4 ] ;
	[FieldOffset(0)] public fixed long   m128_i64[ 2 ] ;
	[FieldOffset(0)] public fixed byte   m128_u8[ 16 ] ;
	[FieldOffset(0)] public fixed ushort m128_u16[ 8 ] ;
	[FieldOffset(0)] public fixed uint   m128_u32[ 4 ] ;
	
	
	#region Constructors
	public __m128( float f0, float f1 = 0f, 
				   float f2 = 0f, float f3 = 0f ) {
		Unsafe.SkipInit( out this ) ;
		m128_f32[ 0 ] = f0 ;
		m128_f32[ 1 ] = f1 ;
		m128_f32[ 2 ] = f2 ;
		m128_f32[ 3 ] = f3 ;
	}
	
	public __m128( ulong u0, ulong u1 = 0u ) {
		Unsafe.SkipInit( out this ) ;
		m128_u64[ 0 ] = u0 ;
		m128_u64[ 1 ] = u1 ;
	}
	
	public __m128( byte b0, byte b1 = 0, byte b2 = 0, byte b3 = 0,
				   byte b4 = 0, byte b5 = 0, byte b6 = 0, byte b7 = 0,
				   byte b8 = 0, byte b9 = 0, byte b10 = 0, byte b11 = 0,
				   byte b12 = 0, byte b13 = 0, byte b14 = 0, byte b15 = 0 ) {
		Unsafe.SkipInit( out this ) ;
		m128_i8[ 0 ]  = b0 ; m128_i8[ 1 ]   = b1 ;
		m128_i8[ 2 ]  = b2 ; m128_i8[ 3 ]   = b3 ;
		m128_i8[ 4 ]  = b4 ; m128_i8[ 5 ]   = b5 ;
		m128_i8[ 6 ]  = b6 ; m128_i8[ 7 ]   = b7 ;
		m128_i8[ 8 ]  = b8 ; m128_i8[ 9 ]   = b9 ;
		m128_i8[ 10 ] = b10 ; m128_i8[ 11 ] = b11 ;
		m128_i8[ 12 ] = b12 ; m128_i8[ 13 ] = b13 ;
		m128_i8[ 14 ] = b14 ; m128_i8[ 15 ] = b15 ;
	}
	
	public __m128( sbyte b0, sbyte b1 = 0, sbyte b2 = 0, sbyte b3 = 0,
				   sbyte b4 = 0, sbyte b5 = 0, sbyte b6 = 0, sbyte b7 = 0,
				   sbyte b8 = 0, sbyte b9 = 0, sbyte b10 = 0, sbyte b11 = 0,
				   sbyte b12 = 0, sbyte b13 = 0, sbyte b14 = 0, sbyte b15 = 0 ) {
		Unsafe.SkipInit( out this ) ;
		m128_i8[ 0 ]  = (byte)b0 ; m128_i8[ 1 ]   = (byte)b1 ;
		m128_i8[ 2 ]  = (byte)b2 ; m128_i8[ 3 ]   = (byte)b3 ;
		m128_i8[ 4 ]  = (byte)b4 ; m128_i8[ 5 ]   = (byte)b5 ;
		m128_i8[ 6 ]  = (byte)b6 ; m128_i8[ 7 ]   = (byte)b7 ;
		m128_i8[ 8 ]  = (byte)b8 ; m128_i8[ 9 ]   = (byte)b9 ;
		m128_i8[ 10 ] = (byte)b10 ; m128_i8[ 11 ] = (byte)b11 ;
		m128_i8[ 12 ] = (byte)b12 ; m128_i8[ 13 ] = (byte)b13 ;
		m128_i8[ 14 ] = (byte)b14 ; m128_i8[ 15 ] = (byte)b15 ;
	}
	
	public __m128( short s0, short s1 = 0, short s2 = 0, short s3 = 0,
				   short s4 = 0, short s5 = 0, short s6 = 0, short s7 = 0 ) {
		Unsafe.SkipInit( out this ) ;
		m128_i16[ 0 ] = s0 ; m128_i16[ 1 ] = s1 ;
		m128_i16[ 2 ] = s2 ; m128_i16[ 3 ] = s3 ;
		m128_i16[ 4 ] = s4 ; m128_i16[ 5 ] = s5 ;
		m128_i16[ 6 ] = s6 ; m128_i16[ 7 ] = s7 ;
	}
	
	public __m128( ushort s0, ushort s1 = 0, ushort s2 = 0, ushort s3 = 0,
				   ushort s4 = 0, ushort s5 = 0, ushort s6 = 0, ushort s7 = 0 ) {
		Unsafe.SkipInit( out this ) ;
		m128_u16[ 0 ] = s0 ; m128_u16[ 1 ] = s1 ;
		m128_u16[ 2 ] = s2 ; m128_u16[ 3 ] = s3 ;
		m128_u16[ 4 ] = s4 ; m128_u16[ 5 ] = s5 ;
		m128_u16[ 6 ] = s6 ; m128_u16[ 7 ] = s7 ;
	}
	
	public __m128( int i0, int i1 = 0, int i2 = 0, int i3 = 0 ) {
		Unsafe.SkipInit( out this ) ;
		m128_i32[ 0 ] = i0 ;
		m128_i32[ 1 ] = i1 ;
		m128_i32[ 2 ] = i2 ;
		m128_i32[ 3 ] = i3 ;
	}
	
	public __m128( long l0, long l1 = 0 ) {
		Unsafe.SkipInit( out this ) ;
		m128_i64[ 0 ] = l0 ;
		m128_i64[ 1 ] = l1 ;
	}
	
	public __m128( uint u0, uint u1 = 0U, uint u2 = 0U, uint u3 = 0U ) {
		Unsafe.SkipInit( out this ) ;
		m128_u32[ 0 ] = u0 ;
		m128_u32[ 1 ] = u1 ;
		m128_u32[ 2 ] = u2 ;
		m128_u32[ 3 ] = u3 ;
	}
	#endregion
	
	
	public ref T GetRef< T >( int index ) where T: unmanaged {
		fixed( __m128* p = &this ) {
			return ref *(T *)( ((byte *)p + index * sizeof(T)) ) ;
		}
	}
	
	public T Get< T >( int index ) where T: unmanaged {
		fixed( __m128* p = &this ) {
			return Unsafe.Read< T >( ((byte *)p + index * sizeof(T)) ) ;
		}
	}
	
	public void Set< T >( int index, in T value ) where T: unmanaged {
		fixed( __m128* p = &this ) {
			Unsafe.Write( ((byte *)p + index * sizeof(T)), value ) ;
		}
	}
	
	public static Vector128< T > ToVector128< T >( __m128 value ) where T: unmanaged {
		return Unsafe.As< __m128, Vector128< T > >( ref value ) ;
	}
	public static Vector128< T > ToVector128< T >( __m128* value ) where T: unmanaged {
		return Unsafe.As< __m128, Vector128< T > >( ref *value ) ;
	}
	
	#region Vector128< T > Implicit Conversions
	public static implicit operator __m128( Vector128< float > value ) => 
		Unsafe.As< Vector128< float >, __m128 >( ref value ) ;
	public static implicit operator Vector128< float >( __m128 value ) => 
		Unsafe.As< __m128, Vector128< float > >( ref value ) ;
	
	public static implicit operator __m128( Vector128< ulong > value ) =>
	 		Unsafe.As< Vector128< ulong >, __m128 >( ref value ) ;
	public static implicit operator Vector128< ulong >( __m128 value ) =>
	 		Unsafe.As< __m128, Vector128< ulong > >( ref value ) ;
	
	public static implicit operator __m128( Vector128< byte > value ) =>
	 		Unsafe.As< Vector128< byte >, __m128 >( ref value ) ;
	public static implicit operator Vector128< byte >( __m128 value ) =>
	 		Unsafe.As< __m128, Vector128< byte > >( ref value ) ;
	
	public static implicit operator __m128( Vector128< sbyte > value ) =>
	 		Unsafe.As< Vector128< sbyte >, __m128 >( ref value ) ;
	public static implicit operator Vector128< sbyte >( __m128 value ) =>
	 		Unsafe.As< __m128, Vector128< sbyte > >( ref value ) ;
	
	public static implicit operator __m128( Vector128< short > value ) =>
	 		Unsafe.As< Vector128< short >, __m128 >( ref value ) ;
	public static implicit operator Vector128< short >( __m128 value ) =>
	 		Unsafe.As< __m128, Vector128< short > >( ref value ) ;
	
	public static implicit operator __m128( Vector128< ushort > value ) =>
	 		Unsafe.As< Vector128< ushort >, __m128 >( ref value ) ;
	public static implicit operator Vector128< ushort >( __m128 value ) =>
	 		Unsafe.As< __m128, Vector128< ushort > >( ref value ) ;
	
	public static implicit operator __m128( Vector128< int > value ) =>
	 		Unsafe.As< Vector128< int >, __m128 >( ref value ) ;
	public static implicit operator Vector128< int >( __m128 value ) =>
	 		Unsafe.As< __m128, Vector128< int > >( ref value ) ;
	
	public static implicit operator __m128( Vector128< uint > value ) =>
	 		Unsafe.As< Vector128< uint >, __m128 >( ref value ) ;
	public static implicit operator Vector128< uint >( __m128 value ) =>
	 		Unsafe.As< __m128, Vector128< uint > >( ref value ) ;
	
	public static implicit operator __m128( Vector128< long > value ) =>
	 		Unsafe.As< Vector128< long >, __m128 >( ref value ) ;
	public static implicit operator Vector128< long >( __m128 value ) =>
	 		Unsafe.As< __m128, Vector128< long > >( ref value ) ;
	#endregion
	
} ;


//! to be continued ...
public struct XMVECTOR {
	__m128 m128 ;
	
	public XMVECTOR( float f0, float f1 = 0f, 
					  float f2 = 0f, float f3 = 0f ) => 
								m128 = new __m128( f0, f1, f2, f3 ) ;
} ;