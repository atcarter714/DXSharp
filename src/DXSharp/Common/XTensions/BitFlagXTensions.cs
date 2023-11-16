using System.Collections ;
using System.Numerics ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using static DXSharp.DXSharpUtils ;

namespace DXSharp ;


public static class BitFlagXTensions
{
	[MethodImpl( _MAXOPT_ )]
	public static bool HasBitFlag< T >( this T value, T flag )
									where T: unmanaged, Enum {
		unsafe {
			T* p = &value, f = &flag ;
			int sz = sizeof(T) ;
			return sz switch {
					   sizeof(ulong)  => ( *(ulong*)p  & *( (ulong*)f ) )  == *(ulong *)p,
					   sizeof(uint)   => ( *(uint*)p   & *( (uint*)f ) )   == *(uint *)p,
					   sizeof(ushort) => ( *(ushort*)p & *( (ushort*)f ) ) == *(ushort *)p,
					   sizeof(byte)   => ( *(byte*)p   & *( (byte*)f ) )   == *(byte *)p,
					   _ => throw new NotSupportedException( $"Type {typeof( T )} is not supported" ),
				   } ;
		}
	}
	
	[MethodImpl( _MAXOPT_ )]
	public static bool[ ] GetBitFlags< T >( this T value )
									where T: unmanaged, Enum {
		unsafe {
			T* p = &value ;
			int sz = sizeof(T) ;
			int len = sz * 8 ;
			BitArray bits = new( sz * 8 ) ;
			switch( sz ) {
				case sizeof(ulong):
					ulong* u = (ulong*)p ;
					for( int i = 0 ; i < 64 ; ++i ) {
						bits[ i ] = ( *u & ( 1ul << i ) ) != 0 ;
					}
					break ;
				case sizeof(uint):
					uint* ui = (uint*)p ;
					for( int i = 0 ; i < 32 ; ++i ) {
						bits[ i ] = ( *ui & ( 1u << i ) ) != 0 ;
					}
					break ;
				case sizeof(ushort):
					ushort* us = (ushort*)p ;
					for( int i = 0 ; i < 16 ; ++i ) {
						bits[ i ] = ( *us & ( 1 << i ) ) != 0 ;
					}
					break ;
				case sizeof(byte):
					byte* b = (byte*)p ;
					for( int i = 0 ; i < 8 ; ++i ) {
						bits[ i ] = ( *b & ( 1 << i ) ) != 0 ;
					}
					break ;
				default:
					throw new NotSupportedException( $"Type {typeof( T )} is not supported" ) ;
			}
			bool[ ] ret = new bool[ len ] ;
			bits.CopyTo( ret, 0 ) ;
			return ret ;
		}
	}
	
	public static bool HasAnyBit< T >( this T value, T flags )
									where T: unmanaged, Enum {
		unsafe {
			T* p = &value, f = &flags ;
			int sz = sizeof(T) ;
			return sz switch {
					   sizeof(ulong)  => ( *(ulong*)p  & *( (ulong*)f ) )  != 0,
					   sizeof(uint)   => ( *(uint*)p   & *( (uint*)f ) )   != 0,
					   sizeof(ushort) => ( *(ushort*)p & *( (ushort*)f ) ) != 0,
					   sizeof(byte)   => ( *(byte*)p   & *( (byte*)f ) )   != 0,
					   _ => throw new NotSupportedException( $"Type {typeof( T )} is not supported" ),
				   } ;
		}
	}
} ;