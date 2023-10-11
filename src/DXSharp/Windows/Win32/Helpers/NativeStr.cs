using System.Diagnostics ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;

namespace DXSharp.Windows.Win32.Helpers ;

[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode), 
 DebuggerDisplay("{pStr.ToString()}")]
public struct NativeStr32 {
	public const int MaxLength = 32 ;
	__char_32 pStr ;
	
	public NativeStr32( string str ) => pStr = str ;
	public unsafe NativeStr32( char* buffer ) {
		pStr = default ;
		fixed ( NativeStr32* pThis = &this ) {
			for ( int i = 0 ; i < MaxLength && buffer[ i ] != '\0' ; ++i ) {
				if( i * sizeof(char) >= sizeof(NativeStr32) ) break ; // Prevent buffer overflow
				( (char*)pThis )[ i ] = buffer[ i ] ;
			}
		}
	}
	public NativeStr32( __char_32 str ) => pStr = str ;
	public unsafe NativeStr32( __char_32* pStr ) => this.pStr = *pStr ;
	public NativeStr32( in NativeStr32 str32 ) => pStr = str32.pStr ;
	public NativeStr32( char[ ] array ) {
		pStr = default ;
		unsafe { fixed ( NativeStr32* pThis = &this ) {
				for ( int i = 0 ; i < MaxLength && array[ i ] != '\0' ; ++i ) {
					if ( i * sizeof( char ) >= sizeof( NativeStr32 ) ) break ; // Prevent buffer overflow
					( (char*)pThis )[ i ] = array[ i ] ;
				}
			}
		}
	}
}

[Serializable, DebuggerDisplay( "{pStr.ToString()}" )]
[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
public struct NativeStr128 {
	public const int MaxLength = 128 ;
	__char_128 pStr ;
	
	public NativeStr128( string str ) => pStr = str ;
	public unsafe NativeStr128( char* buffer ) {
		pStr = default ;
		fixed ( NativeStr128* pThis = &this ) {
			for ( int i = 0 ; i < MaxLength && buffer[ i ] != '\0' ; ++i ) {
				if ( i * sizeof( char ) >= sizeof( NativeStr128 ) ) break ; // Prevent buffer overflow
				( (char*)pThis )[ i ] = buffer[ i ] ;
			}
		}
	}
	public NativeStr128( __char_128 str ) => pStr = str ;
	public unsafe NativeStr128( __char_128* pStr ) => this.pStr = *pStr ;
	public NativeStr128( in NativeStr128 str128 ) => pStr = str128.pStr ;
	public NativeStr128( char[ ] array ) {
		pStr = default ;
		unsafe { fixed ( NativeStr128* pThis = &this ) {
				for ( int i = 0 ; i < MaxLength && array[ i ] != '\0' ; ++i ) {
					if ( i * sizeof( char ) >= sizeof( NativeStr128 ) ) break ; // Prevent buffer overflow
					( (char*)pThis )[ i ] = array[ i ] ;
				}
			}
		}
	}

	public override string ToString( ) => pStr.ToString( ) ?? string.Empty ;
	
	public static implicit operator __char_128( NativeStr128 str ) => str.pStr ;
	public static implicit operator NativeStr128( __char_128 str ) => new( str ) ;
}