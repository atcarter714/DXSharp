namespace Windows.Win32.Foundation;

internal readonly partial struct WPARAM
{
	// Error CS0030  Cannot convert type 'nuint' to 'System.IntPtr'	
	//! The compiler was very weird about nint/IntPtr and nuint/UIntPtr for some
	//! odd reason even though they're technically supposed to be the same thing ...

	public static explicit operator nint( WPARAM value ) => (nint)value.Value;
	//public static implicit operator UIntPtr( WPARAM value ) => value.Value;
	//public static explicit operator nint( WPARAM value ) => (nint)value.Value;
	//public static implicit operator WPARAM( UIntPtr value ) => new( value );
}