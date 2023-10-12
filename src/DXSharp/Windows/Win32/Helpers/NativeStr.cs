using System.Diagnostics ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;

namespace DXSharp.Windows.Win32.Helpers ;

[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode), 
 DebuggerDisplay("{pStr.ToString()}")]
public struct FixedStr32 {
	public const int MaxLength = 32 ;
	__char_32 pStr ;
	
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
}

[Serializable, DebuggerDisplay( "{pStr.ToString()}" )]
[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
public struct FixedStr128 {
	public const int MaxLength = 128 ;
	__char_128 pStr ;
	
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

	public override string ToString( ) => pStr.ToString( ) ?? string.Empty ;
	
	public static implicit operator __char_128( FixedStr128 str ) => str.pStr ;
	public static implicit operator FixedStr128( __char_128 str ) => new( str ) ;
}