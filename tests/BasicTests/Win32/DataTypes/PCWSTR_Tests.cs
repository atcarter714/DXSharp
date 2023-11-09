using System.Diagnostics ;
using Windows.Win32.Foundation ;
namespace BasicTests.Win32.DataTypes ;



[Category( "Win32" )]
[TestFixture( Author = "Aaron T. Carter",
			  TestOf = typeof( PCWSTR ) )]
public class PCWSTR_Tests {
	// ---------------------------------------------------------------------------------
	const string PERSISTENT_STR1 = "This is a persistent, unmanaged string.",
				 PERSISTENT_STR2 = "The unmanaged memory contains ASCII characters." ;

	const int MAX_LEN = 0x1000 ;
	PCWSTR    _persistentSTR1 ;
	PCWSTR    _persistentSTR2 ;
	
	
	// ---------------------------------------------------------------------------------
	// Setup/TearDown:
	// ---------------------------------------------------------------------------------

	[SetUp] public void Setup( ) {
		//! Initialize persistent test strings with duplicate values:
		string _txt1 = "This is a persistent, unmanaged string.",
			   _txt2 = "The unmanaged memory contains ASCII characters." ;
		_persistentSTR1 = _txt1 ;
		_persistentSTR2 = _txt2 ;

		//! Validate the strings are the same:
		Assert.Multiple( ( ) =>
						 {
							 Assert.That( _txt1, Is.EqualTo( PERSISTENT_STR1 ) ) ;
							 Assert.That( _txt2, Is.EqualTo( PERSISTENT_STR2 ) ) ;
						 } ) ;
	}

	[TearDown] public void TearDown( ) {
		unsafe {
			Assert.That( (long)_persistentSTR1.Value, Is.Not.Zero ) ;
			Assert.That( (long)_persistentSTR2.Value, Is.Not.Zero ) ;
			_persistentSTR1.Dispose( ) ;
			_persistentSTR2.Dispose( ) ;
		}
	}
	
	
	// ---------------------------------------------------------------------------------
	// Tests:
	// ---------------------------------------------------------------------------------

	[Test] public unsafe void Test_Memory_Integrity( ) {
		// Perform the PCWSTR version of the ASCII string test:

		// Create a managed string, and unmanaged PCWSTR copy in RAM:
		string       managedStr1 = "Hello, DXSharp Unit Testing!" ;
		using PCWSTR pcStr1      = managedStr1 ;
		Assert.That( (long)pcStr1.Value, Is.Not.Zero ) ;

		// Length checks:
		int strLen1   = managedStr1.Length ;
		int pcstrLen1 = _wcslen( pcStr1.Value ) ;
		Assert.That( strLen1, Is.EqualTo( pcstrLen1 ) ) ;
		Assert.That( pcstrLen1, Is.EqualTo( pcStr1.Length ) ) ;

		// Create char buffer of from managed string:
		var chars = managedStr1.AsSpan( ) ;
		Assert.That( chars.Length, Is.EqualTo( managedStr1.Length ) ) ;

		PCWSTR* strPtr1  = &pcStr1 ;
		char**  _pBuffer = (char**)&pcStr1 ;
		char*   cstr1    = (char*)( strPtr1->Value ) ;
		Assert.IsTrue( *_pBuffer == cstr1 ) ; //! pointer to pointer cast: ensures struct layout is correct

		Span< char > cstr1Span = new( cstr1, strLen1 ) ;
		Assert.That( cstr1Span.Length, Is.EqualTo( managedStr1.Length ) ) ;

		// Compare the chars from the managed string to the unmanaged PCWSTR:
		for ( int i = 0; i < chars.Length; ++i ) {
			Assert.That( chars[ i ], Is.EqualTo( cstr1Span[ i ] ) ) ;
		}

		// Create a 2nd managed string from the unmanaged PCWSTR:
		string? managedStr2 = pcStr1 ;
		Assert.That( managedStr2, Is.Not.Null ) ;
		Assert.That( pcStr1 == managedStr2 ) ; // tests implicit operator and equality logic ...
	}
	
	[Test] public void Test_String_Memory_Persistence( ) {
		//! In this test, we're just making sure the PCWSTR
		//! memory created in [Setup] is persistent/valid :
		unsafe {
			// Check the addresses: Make sure they're non-zero, and not equal:
			char* p1 = _persistentSTR1.Value, p2 = _persistentSTR2.Value ;
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
				Debug.Write( p1[ i ] ) ;
			}
			Debug.WriteLine( '\n' ) ;
			
			Debug.WriteLine( $"{nameof(Test_String_Memory_Persistence)} :: " +
							 $"Printing {nameof(_persistentSTR2)}" ) ;
			
			// Print the string:
			for ( int i = 0 ; i < len2 ; ++i ) {
				Debug.Write( p2[ i ] ) ;
			}
			Debug.WriteLine( '\n' ) ;
#endif
		}
	}
	
	
	// -----------------------------------
	// Test Helper Methods:
	// -----------------------------------

	static unsafe int _wcslen( char* pcStr1Value ) {
		int len = 0 ;
		while ( pcStr1Value[ len ] != '\0' ) ++len ;
		return len ;
	}
	
	// ==================================================================================
} ;
