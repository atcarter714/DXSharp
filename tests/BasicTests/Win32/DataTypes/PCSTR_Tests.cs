using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.InteropServices ;
using System.Text ;
using Windows.Win32.Foundation ;

namespace BasicTests.Win32.DataTypes ;

[Category( "Win32" )]
[TestFixture( Author = "Aaron T. Carter", 
			  TestOf = typeof(PCSTR) )]
public class PCSTR_Tests {
	const string PERSISTENT_STR1 = "This is a persistent, unmanaged string.",
				 PERSISTENT_STR2 = "The unmanaged memory contains ASCII characters." ;
	const int MAX_LEN = 0x1000 ;
	PCSTR     _persistentSTR1 ;
	PCSTR     _persistentSTR2 ;
	

	[SetUp] public void Setup( ) {
		//! Initialize persistent test strings with duplicate values:
		string _txt1 = "This is a persistent, unmanaged string.", 
			   _txt2 = "The unmanaged memory contains ASCII characters." ;
		_persistentSTR1 = _txt1 ; _persistentSTR2 = _txt2 ;
		
		//! Validate the strings are the same:
		Assert.Multiple( ( ) => {
							 Assert.That( _txt1, Is.EqualTo( PERSISTENT_STR1 ) ) ;
							 Assert.That( _txt2, Is.EqualTo( PERSISTENT_STR2 ) ) ;
						 } ) ;
	}
	
	[TearDown] public void TearDown( ) { unsafe {
			Assert.That( (long)_persistentSTR1.Value, Is.Not.Zero ) ;
			Assert.That( (long)_persistentSTR2.Value, Is.Not.Zero ) ;
			_persistentSTR1.Dispose( ) ;
			_persistentSTR2.Dispose( ) ;
	}}

	
	// ---------------------------------------------------------------------------------
	// Tests:
	// ---------------------------------------------------------------------------------
	
	[Test] public void Test_Memory_Integrity( ) { unsafe {
		// Create a managed string, and unmanaged PCSTR copy in RAM:
		string managedStr1   = "Hello, DXSharp Unit Testing!" ;
		using PCSTR pcStr1 = managedStr1 ;
		Assert.That( (long)pcStr1.Value, Is.Not.Zero ) ;
		
		// Length checks:
		int strLen1 = managedStr1.Length ;
		int pcstrLen1 = _strlen( pcStr1.Value ) ;
		Assert.That( strLen1, Is.EqualTo(pcstrLen1) ) ;
		Assert.That( pcstrLen1, Is.EqualTo(pcStr1.Length) ) ;
		
		// Create byte buffer of ASCII-encoded chars from managed string:
		var asciiBytes = Encoding.ASCII.GetBytes( managedStr1 )
																.AsSpan( ) ;
		Assert.That( asciiBytes.Length, Is.EqualTo(managedStr1.Length) ) ;
		
		
		PCSTR* strPtr1  = &pcStr1 ;
		byte** _pBuffer = (byte **)&pcStr1 ;
		byte*  cstr1    = (byte *)(strPtr1->Value) ;
		Assert.IsTrue( *_pBuffer == cstr1 ) ; //! pointer to pointer cast: ensures struct layout is correct
		
		Span< byte > cstr1Span = new( cstr1, strLen1 ) ;
		Assert.That( cstr1Span.Length, Is.EqualTo(managedStr1.Length) ) ;
		
		// Compare the ASCII bytes from the managed string to the unmanaged PCSTR:
		for ( int i = 0 ; i < asciiBytes.Length ; ++i ) {
			Assert.That( asciiBytes[ i ], Is.EqualTo(cstr1Span[ i ]) ) ;
		}
		
		// Create a 2nd managed string from the unmanaged PCSTR:
		string managedStr2 = _asciiToString( cstr1 ) ;
		Assert.That( pcStr1 == managedStr2 ) ; // tests implicit operator and equality logic ...
	}}

	[Test] public void Test_String_Memory_Persistence( ) {
		//! In this test, we're just making sure the PCSTR
		//! memory created in [Setup] is persistent/valid :
		unsafe {
			// Check the addresses: Make sure they're non-zero, and not equal:
			byte* p1 = _persistentSTR1.Value, p2 = _persistentSTR2.Value ;
			long addr1 = (long)p1, addr2 = (long)p2 ;
			Assert.Multiple( ( ) => {
								 Assert.That( addr1, Is.Not.Zero ) ;
								 Assert.That( addr2, Is.Not.Zero ) ;
								 Assert.That( addr1, Is.Not.EqualTo( addr2 ) ) ;
							 } ) ;
			
			// Check the lengths:
			int len1 = _persistentSTR1.Length, len2 = _persistentSTR2.Length ;
			Assert.Multiple( ( ) => {
								 Assert.That( len1, Is.Not.Zero ) ;
								 Assert.That( len2, Is.Not.Zero ) ;
								 Assert.That( len1, Is.Not.EqualTo(len2) ) ;
								 Assert.That( len1, Is.EqualTo( _persistentSTR1.Length ) ) ;
								 Assert.That( len2, Is.EqualTo( _persistentSTR2.Length ) ) ;
							 } ) ;

#if DEBUG
			Debug.WriteLine( $"{nameof(Test_String_Memory_Persistence)} :: " +
							 $"Printing {nameof(_persistentSTR1)}" ) ;
			
			// Print the string:
			for ( int i = 0 ; i < len1 ; ++i ) {
				Debug.Write( (char)p1[ i ] ) ;
			}
			Debug.WriteLine( '\n' ) ;
			
			Debug.WriteLine( $"{nameof(Test_String_Memory_Persistence)} :: " +
							 $"Printing {nameof(_persistentSTR2)}" ) ;
			
			// Print the string:
			for ( int i = 0 ; i < len2 ; ++i ) {
				Debug.Write( (char)p2[ i ] ) ;
			}
			Debug.WriteLine( '\n' ) ;
#endif
		}
	}
	
	
	
	// -----------------------------------
	// Test Helper Methods:
	// -----------------------------------
	
	static unsafe int _strlen( in byte* pBuffer ) {
		byte* p = pBuffer ;
		if ( p is null ) return 0 ;
		
		while ( *p != 0x00 ) p++ ;
		return checked( (int)( p - pBuffer ) ) ;
	}
	
	static unsafe string _asciiToString( [NotNull] in byte* pBuffer, 
														int len = 0 ) {
		Assert.IsTrue( (pBuffer is not null) ) ;
		if( len <= 0 ) len = _strlen( pBuffer ) ;
		
		Span< byte > ascii = new( pBuffer, len ) ;
		return Encoding.ASCII.GetString( ascii ) ;
	}
	
	static unsafe bool _strcmp( in byte* left, in byte* right ) {
		byte* l = left, r = right ; //! parameters can't be captured in closures
		Assert.Multiple( ( ) => {
							 Assert.That( ( l is not null ) ) ;
							 Assert.That( ( r is not null ) ) ;
						 } ) ;
		
		for ( int count = 0 ;
			  *l == *r ;
			  ++l, ++r, ++count ) {
			if( count >= MAX_LEN || *l == 0x00b ) return true ;
		}
		return false ;
	}
	
	// ==================================================================================
} ;
