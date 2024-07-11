#region Using Directives

using System.Collections ;
using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Intrinsics ;
using Windows.Win32 ;

using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp.Windows.Win32.Helpers ;


[DebuggerDisplay("{pStr.ToString()}")]
[Serializable, StructLayout(LayoutKind.Sequential, 
							CharSet = CharSet.Unicode, 
							Size = 32 * sizeof(char))]
public struct FixedStr32: IEquatable< FixedStr32 >, 
						  IEnumerable< char > {
	public const int MaxLength = 32 ;
	__char_32 pStr ;
	
	public char this[ int index ] {
		[MethodImpl(_MAXOPT_)] get => pStr[ index ] ;
		[MethodImpl(_MAXOPT_)] set => pStr[ index ] = value ;
	}
	
	public char this[ Index ind ] {
		[MethodImpl(_MAXOPT_)] get => pStr[ ind ] ;
		[MethodImpl(_MAXOPT_)] set => pStr[ ind ] = value ;
	}

	
	public FixedStr32( string str ) => pStr = str ;
	public unsafe FixedStr32( char* buffer ) {
		pStr = default ;
		fixed ( FixedStr32* pThis = &this ) {
			for ( int i = 0 ; i < MaxLength && buffer[ i ] != '\0' ; ++i ) {
				if( i * sizeof(char) >= sizeof(FixedStr32) ) break ; // Prevent buffer overflow
				( (char*)pThis )[ i ] = buffer[ i ] ;
			}
		}
	}
	public FixedStr32( __char_32 str ) => pStr = str ;
	public unsafe FixedStr32( __char_32* pStr ) => this.pStr = *pStr ;
	public FixedStr32( in FixedStr32 str32 ) => pStr = str32.pStr ;
	public FixedStr32( char[ ] array ) {
		pStr = default ;
		unsafe { fixed ( FixedStr32* pThis = &this ) {
				for ( int i = 0 ; i < MaxLength && array[ i ] != '\0' ; ++i ) {
					if ( i * sizeof( char ) >= sizeof( FixedStr32 ) ) break ; // Prevent buffer overflow
					( (char*)pThis )[ i ] = array[ i ] ;
				}
			}
		}
	}
	
	
	[UnscopedRef]
	public unsafe Span< char > AsSpan( ) {
		fixed( char* p = &this.pStr.Value[ 0 ] )
			return new Span< char >( p, MaxLength ) ;
	}
	
	[UnscopedRef]
	public unsafe  Span< Vector256<char> > AsVectorSpan( ) {
		fixed( char* p = &this.pStr.Value[ 0 ] )
			return new Span< Vector256<char> >( p, MaxLength / Vector256<char>.Count ) ;
	}
	
	
	
	public override int GetHashCode( ) {
		unsafe { fixed ( char* p = &this.pStr.Value[ 0 ] ) {
				Vector256< char >* pVec = (Vector256<char>*)p ;
				return HashCode.Combine( pVec[ 0 ], pVec[ 1 ], pVec[ 2 ], pVec[ 3 ],
										 pVec[ 4 ], pVec[ 5 ], pVec[ 6 ], pVec[ 7 ] ) ;
			}
		}
	}
	public override bool Equals( object? obj ) => obj is FixedStr32 other && Equals( other ) ;
	public override string ToString( ) => pStr.ToString( ) ?? string.Empty ;
	public bool Equals( FixedStr32 other ) => other == this ;
	IEnumerator IEnumerable.GetEnumerator( ) => GetEnumerator( ) ;
	public IEnumerator< char > GetEnumerator( ) {
		for ( int i = 0 ; i < MaxLength ; ++i ) {
			yield return pStr[ i ] ;
		}
	}

	
	public static void FillFrom( ref FixedStr32 str, in Span< char > data ) {
		if( data.IsEmpty ) return ;
		var vecSpan = str.AsVectorSpan( ) ;
		
		int vcount  = 0, ccount = 0, c_inc = Vector256<char>.Count ;
		unsafe { fixed( char* pSrc = &data [ 0 ], pDst = &str.pStr.Value[ 0 ] ) {
				Vector256<char>* pSrcVec = (Vector256<char>*)pSrc ;
				while ( ccount < data.Length ) {
					if( vcount >= vecSpan.Length ) break ;
					
					// If we don't have enough room for another 256-bit vector, copy the rest of the data ...
					if ( ccount + c_inc > data.Length ) {
						char* dstPos = pDst + ccount, srcPos = pSrc + ccount ;
						for ( int i = 0 ; i < data.Length - ccount ; ++i )
							dstPos[ i ] = srcPos[ i ] ;
						break ;
					}
					
					vecSpan[ vcount++ ] = pSrcVec[ vcount ] ;
					ccount += c_inc ;
				}
			}
		}
	}
	
	
	public static implicit operator __char_32( FixedStr32 str ) => str.pStr ;
	public static implicit operator FixedStr32( __char_32 str ) => new( str ) ;

	[MethodImpl( _MAXOPT_ )]
	public static bool operator ==( FixedStr32 left, FixedStr32 right ) {
		unsafe {
			Vector256< char >* lPtr = (Vector256< char >*)&left.pStr.Value[ 0 ] ;
			Vector256< char >* rPtr = (Vector256< char >*)&right.pStr.Value[ 0 ] ;
			return lPtr[ 0 ] == rPtr[ 0 ] 
				   && lPtr[ 1 ] == rPtr[ 1 ] ;
		}
	}
	[MethodImpl( _MAXOPT_ )]
	public static bool operator !=( FixedStr32 left, FixedStr32 right ) => !( left == right ) ;
} ;



/// <summary>Fixed-length structured string of 128 characters (256 bytes).</summary>
[DebuggerDisplay( "{pStr.ToString()}" )]
[Serializable, StructLayout( LayoutKind.Sequential, 
							 CharSet = CharSet.Unicode, 
							 Size = 128 * sizeof(char) )]
public struct FixedStr128: IEquatable< FixedStr128 >, 
						   IEnumerable< char > {
	public const int MaxLength = 128 ;
	__char_128 pStr ;
	
	public char this[ int index ] {
		[MethodImpl(_MAXOPT_)] get => pStr[ index ] ;
		[MethodImpl(_MAXOPT_)] set => pStr[ index ] = value ;
	}
	
	public char this[ Index ind ] {
		[MethodImpl(_MAXOPT_)] get => pStr[ ind ] ;
		[MethodImpl(_MAXOPT_)] set => pStr[ ind ] = value ;
	}
	
	
	public FixedStr128( string str ) => pStr = str ;
	public unsafe FixedStr128( char* buffer ) {
		pStr = default ;
		fixed ( FixedStr128* pThis = &this ) {
			for ( int i = 0 ; i < MaxLength && buffer[ i ] != '\0' ; ++i ) {
				if ( i * sizeof( char ) >= sizeof( FixedStr128 ) ) break ; // Prevent buffer overflow
				( (char*)pThis )[ i ] = buffer[ i ] ;
			}
		}
	}
	public FixedStr128( __char_128 str ) => pStr = str ;
	public unsafe FixedStr128( __char_128* pStr ) => this.pStr = *pStr ;
	public FixedStr128( in FixedStr128 str128 ) => pStr = str128.pStr ;
	public FixedStr128( char[ ] array ) {
		pStr = default ;
		unsafe { fixed ( FixedStr128* pThis = &this ) {
				for ( int i = 0 ; i < MaxLength && array[ i ] != '\0' ; ++i ) {
					if ( i * sizeof( char ) >= sizeof( FixedStr128 ) ) break ; // Prevent buffer overflow
					( (char*)pThis )[ i ] = array[ i ] ;
				}
			}
		}
	}

	
	[UnscopedRef]
	public unsafe Span< char > AsSpan( ) {
		fixed( char* p = &this.pStr.Value[ 0 ] )
			return new Span< char >( p, MaxLength ) ;
	}
	
	[UnscopedRef]
	public unsafe Span< Vector256<char> > AsVectorSpan( ) {
		fixed( char* p = &this.pStr.Value[ 0 ] )
			return new Span< Vector256<char> >( p, MaxLength / Vector256<char>.Count ) ;
	}


	public override int GetHashCode( ) {
		unsafe { fixed ( char* p = &this.pStr.Value[ 0 ] ) {
				Vector256< char >* pVec = (Vector256<char>*)p ;
				return HashCode.Combine( pVec[ 0 ], pVec[ 1 ], pVec[ 2 ], pVec[ 3 ],
										 pVec[ 4 ], pVec[ 5 ], pVec[ 6 ], pVec[ 7 ] ) ;
			}
		}
	}
	public override bool Equals( object? obj ) => obj is FixedStr128 other && Equals( other ) ;
	public override string ToString( ) => pStr.ToString( ) ?? string.Empty ;
	public bool Equals( FixedStr128 other ) => other == this ;
	IEnumerator IEnumerable.GetEnumerator( ) => GetEnumerator( ) ;
	public IEnumerator< char > GetEnumerator( ) {
		for ( int i = 0 ; i < MaxLength ; ++i ) {
			yield return pStr[ i ] ;
		}
	}

	
	public static void FillFrom( ref FixedStr128 str, in Span< char > data ) {
		if( data.IsEmpty ) return ;
		var vecSpan = str.AsVectorSpan( ) ;
		
		int vcount = 0, ccount = 0, c_inc = Vector256<char>.Count ;
		unsafe { fixed( char* pSrc = &data [ 0 ], pDst = &str.pStr.Value[ 0 ] ) {
				Vector256<char>* pSrcVec = (Vector256<char> *)pSrc ;
				while ( ccount < data.Length ) {
					if( vcount >= vecSpan.Length ) break ;
					
					// If we don't have enough room for another 256-bit vector, copy the rest of the data ...
					if ( ccount + c_inc > data.Length ) {
						char* dstPos = pDst + ccount, srcPos = pSrc + ccount ;
						for ( int i = 0 ; i < data.Length - ccount ; ++i )
							dstPos[ i ] = srcPos[ i ] ;
						break ;
					}
					
					vecSpan[ vcount++ ] =  pSrcVec[ vcount ] ;
					ccount += c_inc ;
				}
			}
		}
	}



	public static implicit operator __char_128( FixedStr128 str ) => str.pStr ;
	public static implicit operator FixedStr128( __char_128 str ) => new( str ) ;

	[MethodImpl( _MAXOPT_ )]
	public static bool operator ==( FixedStr128 left, FixedStr128 right ) {
		/* NOTE: -----------------------------------
		 //! Can be done this way, also, but unsafe/pointers are faster:
		 Span< Vector256<char> > leftSpan =
			MemoryMarshal.Cast< char, Vector256<char> >( left.AsSpan( ) ) ;
		Span< Vector256<char> > rightSpan =
			MemoryMarshal.Cast< char, Vector256<char> >( right.AsSpan( ) ) ;
		*/
		
		unsafe {
			Vector256< char >* lPtr = (Vector256< char >*)&left.pStr.Value[ 0 ] ;
			Vector256< char >* rPtr = (Vector256< char >*)&right.pStr.Value[ 0 ] ;
			for ( int i = 0 ; i < 0x08 ; ++i ) {
				if ( !( lPtr[ i ] == rPtr[ i ] ) ) return false ;
			}
		}
		return true ;
	}
	[MethodImpl( _MAXOPT_ )]
	public static bool operator !=( FixedStr128 left, FixedStr128 right ) => !( left == right ) ;
} ;