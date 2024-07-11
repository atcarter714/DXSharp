// ReSharper disable InconsistentNaming
#region Using Directives
using System.Text ;
using System.Diagnostics ;
using Windows.Win32.Foundation ;
#endregion
namespace DXSharp ;

//! TODO: Much of this code is AI-generated and needs to be tested ...


/// <summary>
/// Represents a string of ASCII characters.
/// </summary>
/// <remarks>
/// The <see cref="ASCIIString"/> type is implicitly convertible to/from
/// <see cref="string"/>, <see cref="char"/>[], <see cref="byte"/>[], and
/// <see cref="ReadOnlyMemory{T}"/>/<see cref="ReadOnlySpan{T}"/> of
/// <see cref="char"/> or <see cref="byte"/>. It is not intended to replace
/// the use of <see cref="string"/>, but rather to provide a type-safe alternative
/// with handy helper methods when dealing with buffers of ASCII characters
/// in the form of a <see cref="byte"/> array or unmanged memory.
/// </remarks>
[DebuggerDisplay("ToString()")]
public sealed class ASCIIString {
	#region Constant & Static ReadOnly Values
	const string AllASCIICharsStr =
		"\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u0009\u000A\u000B\u000C\u000D\u000E\u000F" +
		"\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F" +
		" !\"#$%&'()*+,-./0123456789:;<=>?@" +
		"ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`" +
		"abcdefghijklmnopqrstuvwxyz{|}~\u007F" ;
	
	internal static readonly Dictionary< char, ASCIIChar > _asciiMap 
		= new( byte.MaxValue ) ;
	
	internal static readonly Dictionary< ASCIIChar, char > _charMap 
		= new( byte.MaxValue ) ;
	#endregion
	
	static ASCIIString( ) {
		unsafe {
			int len = AllASCIICharsStr.Length ;
			byte* pOut = stackalloc byte[ len ] ;
			fixed ( char* p = AllASCIICharsStr ) {
				int c = Encoding.ASCII.GetBytes( p, len, pOut, len ) ;
				Debug.Assert( c is byte.MaxValue ) ;
			}
			for ( int i = 0 ; i < len ; i++ ) {
				_asciiMap.Add( AllASCIICharsStr[ i ], pOut[ i ] ) ;
			}
		}
		
		// Now make the reverse map:
		foreach ( var kvp in _asciiMap ) {
			_charMap.Add( kvp.Value, kvp.Key ) ;
		}
	}
	
	readonly ASCIIChar[ ] _chars ;
	
	// ---------------------------------------------------------
	// Properties:
	// ---------------------------------------------------------
	
	/// <summary>
	/// Gets the total number of <see cref="ASCIIChar"/> characters in the string.
	/// </summary>
	public int Length => _chars?.Length ?? 0 ;
	
	public ASCIIChar this[ int index ] {
		get => _chars[ index ] ;
		set => _chars[ index ] = value ;
	}
	
	public ref ASCIIChar this[ uint index ] => 
			ref _chars[ index ] ;
	
	// ---------------------------------------------------------
	// Constructors:
	// ---------------------------------------------------------
	
	#region Constructors
	public ASCIIString( string str ) {
		_chars = new ASCIIChar[ str.Length ] ;
		for ( int i = 0 ; i < str.Length ; i++ ) {
			_chars[ i ] = str[ i ] ;
		}
	}
	
	public ASCIIString( ASCIIString str ) {
		_chars = str._chars ;
	}
	
	public ASCIIString( char[ ] chars ) {
		ArgumentNullException.ThrowIfNull( chars ) ;
		_chars = chars.Length is 0 ? 
					 Array.Empty< ASCIIChar >( ) 
						: chars.Select( c => new ASCIIChar( c ) )
							   .ToArray( ) ;
	}
	
	public ASCIIString( ASCIIChar[ ] chars ) {
		ArgumentNullException.ThrowIfNull( chars ) ;
		_chars = chars.Length is 0 ? 
					 Array.Empty< ASCIIChar >( ) : chars ;
	}
	
	public ASCIIString( ReadOnlySpan< ASCIIChar > chars ) {
		_chars = chars.Length is 0 ? 
					 Array.Empty< ASCIIChar >( ) : chars.ToArray( ) ;
	}
	
	public ASCIIString( ReadOnlyMemory< ASCIIChar > chars ) {
		_chars = chars.Length is 0 ? 
					 Array.Empty< ASCIIChar >( ) : chars.ToArray( ) ;
	}
	
	public ASCIIString( ReadOnlySpan< char > chars ) {
		if( chars.Length is 0 ) {
			_chars = Array.Empty< ASCIIChar >( ) ;
			return ;
		}
		_chars = new ASCIIChar[ chars.Length ] ;
		for ( int i = 0 ; i < chars.Length ; i++ ) {
			_chars[ i ] = chars[ i ] ;
		}
	}
	
	public ASCIIString( ReadOnlyMemory< char > chars ) {
		if( chars.Length is 0 ) {
			_chars = Array.Empty< ASCIIChar >( ) ;
			return ;
		}
		_chars = new ASCIIChar[ chars.Length ] ;
		for ( int i = 0 ; i < chars.Length ; i++ ) {
			_chars[ i ] = chars.Span[ i ] ;
		}
	}
	
	public ASCIIString( ReadOnlySpan< byte > bytes ) {
		if( bytes.Length is 0 ) {
			_chars = Array.Empty< ASCIIChar >( ) ;
			return ;
		}
		_chars = new ASCIIChar[ bytes.Length ] ;
		for ( int i = 0 ; i < bytes.Length ; i++ ) {
			_chars[ i ] = bytes[ i ] ;
		}
	}

	public ASCIIString( ReadOnlyMemory< byte > bytes ) {
		if( bytes.Length is 0 ) {
			_chars = Array.Empty< ASCIIChar >( ) ;
			return ;
		}
		_chars = new ASCIIChar[ bytes.Length ] ;
		for ( int i = 0 ; i < bytes.Length ; i++ ) {
			_chars[ i ] = bytes.Span[ i ] ;
		}
	}
	
	public ASCIIString( in PCSTR str ) {
		_chars = new ASCIIChar[ str.Length ] ;
		unsafe {
			for ( int i = 0; i < str.Length; i++ ) {
				_chars[ i ] = str.Value[ i ] ;
			}
		}
	}
	#endregion
	
	
	// ---------------------------------------------------------
	// Instance Methods:
	// ---------------------------------------------------------
	
	public override string ToString( ) {
		if( _chars.Length is 0 ) return string.Empty ;
		StringBuilder sb = new( _chars.Length ) ;
		for ( int i = 0 ; i < _chars.Length ; i++ ) {
			sb.Append( _charMap[ _chars[i] ] ) ;
		}
		return sb.ToString( ) ;
	}
	
	public ASCIIString Clone( ) => new( this ) ;
	
	public ASCIIString Substring( int startIndex ) {
		if( startIndex < 0 ) throw new ArgumentOutOfRangeException( nameof( startIndex ) ) ;
		if( startIndex >= _chars.Length ) throw new ArgumentOutOfRangeException( nameof( startIndex ) ) ;
		return new ASCIIString( _chars.AsSpan( startIndex ) ) ;
	}
	
	public ASCIIString Substring( int startIndex, int length ) {
		if( startIndex < 0 ) throw new ArgumentOutOfRangeException( nameof( startIndex ) ) ;
		if( startIndex >= _chars.Length ) throw new ArgumentOutOfRangeException( nameof( startIndex ) ) ;
		if( length < 0 ) throw new ArgumentOutOfRangeException( nameof( length ) ) ;
		if( startIndex + length > _chars.Length ) throw new ArgumentOutOfRangeException( nameof( length ) ) ;
		return new ASCIIString( _chars.AsSpan( startIndex, length ) ) ;
	}
	
	public ASCIIString Trim( ) {
		int start = 0 ;
		int end = _chars.Length - 1 ;
		while ( start < end && _chars[ start ] == ' ' ) start++ ;
		while ( end > start && _chars[ end ] == ' ' ) end-- ;
		return Substring( start, end - start + 1 ) ;
	}
	
	public ASCIIString TrimStart( ) {
		int start = 0 ;
		while ( start < _chars.Length && _chars[ start ] == ' ' ) start++ ;
		return Substring( start ) ;
	}
	
	public ASCIIString TrimEnd( ) {
		int end = _chars.Length - 1 ;
		while ( end >= 0 && _chars[ end ] == ' ' ) end-- ;
		return Substring( 0, end + 1 ) ;
	}
	
	public ASCIIString ToLower( ) {
		ASCIIChar[ ] chars = new ASCIIChar[ _chars.Length ] ;
		for ( int i = 0 ; i < _chars.Length ; i++ ) {
			chars[ i ] = (byte)char.ToLower( _charMap[ _chars[ i ] ] ) ;
		}
		return new ASCIIString( chars ) ;
	}
	
	public ASCIIString ToUpper( ) {
		ASCIIChar[ ] chars = new ASCIIChar[ _chars.Length ] ;
		for ( int i = 0 ; i < _chars.Length ; i++ ) {
			chars[ i ] = (byte)char.ToUpper( _charMap[ _chars[ i ] ] ) ;
		}
		return new ASCIIString( chars ) ;
	}
	
	// ---------------------------------------------------------
	// Static Methods:
	// ---------------------------------------------------------
	
	#region Static Methods

	public static bool IsNullOrEmpty( ASCIIString? str ) =>
		str is null || str.Length is 0 ;
	
	public static bool IsNullOrWhiteSpace( ASCIIString? str ) {
		if( str is null ) return true ;
		for ( int i = 0 ; i < str.Length ; i++ ) {
			if( !ASCIIChar.IsWhiteSpace( str[ i ] ) ) return false ;
		}
		return true ;
	}
	
	public static ASCIIString Concat( ASCIIString str0, ASCIIString str1 ) {
		ASCIIChar[ ] chars = new ASCIIChar[ str0.Length + str1.Length ] ;
		for ( int i = 0 ; i < str0.Length ; i++ ) {
			chars[ i ] = str0[ i ] ;
		}
		for ( int i = 0 ; i < str1.Length ; i++ ) {
			chars[ i + str0.Length ] = str1[ i ] ;
		}
		return new ASCIIString( chars ) ;
	}
	
	public static ASCIIString Concat( ASCIIString str0, ASCIIString str1, ASCIIString str2 ) {
		ASCIIChar[ ] chars = new ASCIIChar[ str0.Length + str1.Length + str2.Length ] ;
		for ( int i = 0 ; i < str0.Length ; i++ ) {
			chars[ i ] = str0[ i ] ;
		}
		for ( int i = 0 ; i < str1.Length ; i++ ) {
			chars[ i + str0.Length ] = str1[ i ] ;
		}
		for ( int i = 0 ; i < str2.Length ; i++ ) {
			chars[ i + str0.Length + str1.Length ] = str2[ i ] ;
		}
		return new ASCIIString( chars ) ;
	}
	
	public static ASCIIString Concat( ASCIIString str0, ASCIIString str1, ASCIIString str2, ASCIIString str3 ) {
		ASCIIChar[ ] chars = new ASCIIChar[ str0.Length + str1.Length + str2.Length + str3.Length ] ;
		for ( int i = 0 ; i < str0.Length ; i++ ) {
			chars[ i ] = str0[ i ] ;
		}
		for ( int i = 0 ; i < str1.Length ; i++ ) {
			chars[ i + str0.Length ] = str1[ i ] ;
		}
		for ( int i = 0 ; i < str2.Length ; i++ ) {
			chars[ i + str0.Length + str1.Length ] = str2[ i ] ;
		}
		for ( int i = 0 ; i < str3.Length ; i++ ) {
			chars[ i + str0.Length + str1.Length + str2.Length ] = str3[ i ] ;
		}
		return new ASCIIString( chars ) ;
	}
	
	public static ASCIIString Concat( params ASCIIString[ ] strings ) {
		int len = 0 ;
		for ( int i = 0 ; i < strings.Length ; i++ ) {
			len += strings[ i ].Length ;
		}
		ASCIIChar[ ] chars = new ASCIIChar[ len ] ;
		int index = 0 ;
		for ( int i = 0 ; i < strings.Length ; i++ ) {
			for ( int j = 0 ; j < strings[ i ].Length ; j++ ) {
				chars[ index++ ] = strings[ i ][ j ] ;
			}
		}
		return new ASCIIString( chars ) ;
	}
	
	public static ASCIIString Join( ASCIIString separator, params ASCIIString[ ] strings ) {
		int len = 0 ;
		for ( int i = 0 ; i < strings.Length ; i++ ) {
			len += strings[ i ].Length ;
		}
		len += separator.Length * ( strings.Length - 1 ) ;
		ASCIIChar[ ] chars = new ASCIIChar[ len ] ;
		int index = 0 ;
		for ( int i = 0 ; i < strings.Length ; i++ ) {
			for ( int j = 0 ; j < strings[ i ].Length ; j++ ) {
				chars[ index++ ] = strings[ i ][ j ] ;
			}
			if( i < strings.Length - 1 ) {
				for ( int j = 0 ; j < separator.Length ; j++ ) {
					chars[ index++ ] = separator[ j ] ;
				}
			}
		}
		return new ASCIIString( chars ) ;
	}
	
	public static ASCIIString Join( ASCIIString separator, IEnumerable< ASCIIString > strings ) {
		int len = 0 ;
		IEnumerable< ASCIIString > asciiStrings = 
			strings as ASCIIString[ ] ?? strings.ToArray( ) ;
		
		foreach ( var str in asciiStrings ) {
			len += str.Length ;
		}
		
		len += separator.Length * ( asciiStrings.Count( ) - 1 ) ;
		ASCIIChar[ ] chars = new ASCIIChar[ len ] ;
		
		int index = 0 ;
		foreach ( var str in asciiStrings ) {
			for ( int j = 0 ; j < str.Length ; j++ ) {
				chars[ index++ ] = str[ j ] ;
			}
			
			if ( str == asciiStrings.Last( ) ) continue ;
			
			for ( int j = 0 ; j < separator.Length ; j++ ) {
					chars[ index++ ] = separator[ j ] ;
			}
		}
		
		return new( chars ) ;
	}
	
	public static ASCIIString Join( ASCIIString separator, IEnumerable< ASCIIString > strings, int startIndex, int count ) {
		int len = 0, i = 0 ;
		IEnumerable< ASCIIString > asciiStrings = 
			strings as ASCIIString[ ] ?? strings.ToArray( ) ;
		
		foreach ( var str in asciiStrings ) {
			if( i >= startIndex && i < startIndex + count ) {
				len += str.Length ;
			}
			i++ ;
		}
		
		len += separator.Length * ( count - 1 ) ;
		ASCIIChar[ ] chars = new ASCIIChar[ len ] ;
		
		int index = (i = 0) ;
		foreach ( var str in asciiStrings ) {
			if( i >= startIndex && i < startIndex + count ) {
				for ( int j = 0 ; j < str.Length ; j++ ) {
					chars[ index++ ] = str[ j ] ;
				}
				if( i < startIndex + count - 1 ) {
					for ( int j = 0 ; j < separator.Length ; j++ ) {
						chars[ index++ ] = separator[ j ] ;
					}
				}
			}
			i++ ;
		}
		return new( chars ) ;
	}
	
	public static ASCIIString Join( ASCIIString separator, ASCIIString[ ] strings, int startIndex, int count ) {
		int len = 0 ;
		for ( int i = startIndex ; i < startIndex + count ; i++ ) {
			len += strings[ i ].Length ;
		}
		len += separator.Length * ( count - 1 ) ;
		ASCIIChar[ ] chars = new ASCIIChar[ len ] ;
		int index = 0 ;
		for ( int i = startIndex ; i < startIndex + count ; i++ ) {
			for ( int j = 0 ; j < strings[ i ].Length ; j++ ) {
				chars[ index++ ] = strings[ i ][ j ] ;
			}
			if( i < startIndex + count - 1 ) {
				for ( int j = 0 ; j < separator.Length ; j++ ) {
					chars[ index++ ] = separator[ j ] ;
				}
			}
		}
		return new ASCIIString( chars ) ;
	}
	
	#endregion
	
	// ---------------------------------------------------------
	// Operators:
	// ---------------------------------------------------------
	
	
	
	// =========================================================
} ;



/// <summary>
/// Represents a single ASCII character.
/// </summary>
/// <remarks>
/// The <see cref="ASCIIChar"/> type is implicitly convertible to/from
/// <see cref="byte"/> and <see cref="char"/>. It is not intended to
/// replace the use of <see cref="byte"/> or <see cref="char"/> in
/// your application, but rather to provide a type-safe alternative
/// with handy helper methods when dealing with buffers of ASCII characters
/// in the form of a <see cref="byte"/> array or unmanged memory.
/// </remarks>
[DebuggerDisplay("ToString()")]
public readonly struct ASCIIChar: IEquatable< ASCIIChar >,
								  IComparable< ASCIIChar > {
	public readonly byte Value ;
	
	public ASCIIChar( byte b ) => Value = b ;
	
	public ASCIIChar( char c ) {
		if( ASCIIString._asciiMap.TryGetValue( c, out ASCIIChar b ) ) 
			Value = b.Value ;
		
		else throw new ArgumentException( $"The character '{c}' " +
										  $"is not a valid ASCII character!" ) ;
	}
	
	public override int GetHashCode( ) => Value ;
	public override string ToString( ) => ASCIIString._charMap[ this ].ToString( ) ;
	public bool Equals( ASCIIChar other ) => Value == other.Value ;
	public override bool Equals( object? obj ) => obj is ASCIIChar c && Equals( c ) ;
	public int CompareTo( ASCIIChar other ) => Value.CompareTo( other.Value ) ;
	
	
	// ---------------------------------------------------------
	// Static Methods:
	// ---------------------------------------------------------
	
	#region Static Methods
	
	public static bool IsWhiteSpace( ASCIIChar c ) => 
		c.Value is 0x20 or 0x09 or 0x0A or 0x0B or 0x0C or 0x0D ;
	
	public static bool IsDigit( ASCIIChar c ) => 
		c.Value is >= 0x30 and <= 0x39 ;
	
	public static bool IsLetter( ASCIIChar c ) => 
		c.Value is >= 0x41 and <= 0x5A or >= 0x61 and <= 0x7A ;
	
	public static bool IsLetterOrDigit( ASCIIChar c ) =>
	 		IsLetter( c ) || IsDigit( c ) ;
	
	public static bool IsPunctuation( ASCIIChar c ) =>
	 		c.Value is >= 0x21 and <= 0x2F 
					   or >= 0x3A and <= 0x40 
					   or >= 0x5B and <= 0x60 
					   or >= 0x7B and <= 0x7E ;
	
	public static bool IsSymbol( ASCIIChar c ) =>
	 		c.Value is >= 0x23 and <= 0x26 
					   or >= 0x2A and <= 0x2B 
					   or >= 0x3C and <= 0x3E 
					   or >= 0x5E and <= 0x60 
					   or >= 0x7C and <= 0x7E ;
	
	public static bool IsSeparator( ASCIIChar c ) =>
	 		c.Value is 0x20 or 0x09 or 0x0A or 0x0B or 0x0C or 0x0D ;
	
	public static bool IsControl( ASCIIChar c ) =>
	 		c.Value is <= 0x1F or >= 0x7F and <= 0x9F ;
	
	public static bool IsLower( ASCIIChar c ) =>
	 		c.Value is >= 0x61 and <= 0x7A ;
	
	public static bool IsUpper( ASCIIChar c ) =>
	 		c.Value is >= 0x41 and <= 0x5A ;
	
	public static ASCIIChar ToLower( ASCIIChar c ) =>
	 		IsUpper( c ) ? (byte)( c.Value + 0x20 ) : c ;
	
	public static ASCIIChar ToUpper( ASCIIChar c ) =>
	 		IsLower( c ) ? (byte)( c.Value - 0x20 ) : c ;
	
	
	public static ASCIIChar Parse( string str ) {
	 		if( str.Length is 0 ) throw new ArgumentException( "The string is empty!" ) ;
		if( str.Length > 1 ) throw new ArgumentException( "The string is too long!" ) ;
		return new ASCIIChar( str[ 0 ] ) ;
	}
	
	public static bool TryParse( string str, out ASCIIChar c ) {
		if( str.Length is 0 ) {
			c = default ;
			return false ;
		}
		if( str.Length > 1 ) {
			c = default ;
			return false ;
		}
		try {
			c = new ASCIIChar( str[ 0 ] ) ;
			return true ;
		}
		catch {
			c = default ;
			return false ;
		}
	}
	
	#endregion
	
	// ---------------------------------------------------------
	// Operators:
	// ---------------------------------------------------------
	
	#region Conversion Operators
	public static implicit operator ASCIIChar( byte b ) => new(b) ;
	public static implicit operator ASCIIChar( char c ) => new(c) ;
	public static implicit operator byte( ASCIIChar c ) => c.Value ;
	public static implicit operator char( ASCIIChar c ) => ASCIIString._charMap[ c ] ;
	#endregion
	
	#region Equality Operators
	public static bool operator ==( ASCIIChar a, ASCIIChar b ) => a.Value == b.Value ;
	public static bool operator !=( ASCIIChar a, ASCIIChar b ) => a.Value != b.Value ;
	public static bool operator ==( ASCIIChar a, byte b ) => a.Value == b ;
	public static bool operator !=( ASCIIChar a, byte b ) => a.Value != b ;
	public static bool operator ==( ASCIIChar a, char b ) => a == ASCIIString._asciiMap[ b ] ;
	public static bool operator !=( ASCIIChar a, char b ) => a != ASCIIString._asciiMap[ b ] ;
	
	public static bool operator ==( byte a, ASCIIChar b ) => a == b.Value ;
	public static bool operator !=( byte a, ASCIIChar b ) => a != b.Value ;
	public static bool operator ==( char a, ASCIIChar b ) => ASCIIString._asciiMap[ a ] == b ;
	public static bool operator !=( char a, ASCIIChar b ) => ASCIIString._asciiMap[ a ] != b ;
	#endregion

	#region Comparison Operators
	public static bool operator <( ASCIIChar a, ASCIIChar b ) => a.Value < b.Value ;
	public static bool operator >( ASCIIChar a, ASCIIChar b ) => a.Value > b.Value ;
	public static bool operator <( ASCIIChar a, byte b ) => a.Value < b ;
	public static bool operator >( ASCIIChar a, byte b ) => a.Value > b ;
	public static bool operator <( ASCIIChar a, char b ) => a < ASCIIString._asciiMap[ b ] ;
	public static bool operator >( ASCIIChar a, char b ) => a > ASCIIString._asciiMap[ b ] ;
	
	public static bool operator <( byte a, ASCIIChar b ) => a < b.Value ;
	public static bool operator >( byte a, ASCIIChar b ) => a > b.Value ;
	public static bool operator <( char a, ASCIIChar b ) => ASCIIString._asciiMap[ a ] < b ;
	public static bool operator >( char a, ASCIIChar b ) => ASCIIString._asciiMap[ a ] > b ;
	#endregion
	
	#region Arithmetic Operators
	public static ASCIIChar operator ++( ASCIIChar a ) => (byte)( a.Value + 1 ) ;
	public static ASCIIChar operator --( ASCIIChar a ) => (byte)( a.Value - 1 ) ;
	public static ASCIIChar operator +( ASCIIChar  a, byte b ) => (byte)( a.Value + b ) ;
	public static ASCIIChar operator -( ASCIIChar  a, byte b ) => (byte)( a.Value - b ) ;
	public static ASCIIChar operator +( ASCIIChar  a, ASCIIChar b ) => (byte)( a.Value + b.Value ) ;
	public static ASCIIChar operator -( ASCIIChar  a, ASCIIChar b ) => (byte)( a.Value - b.Value ) ;
	#endregion
	
	// =========================================================
} ;