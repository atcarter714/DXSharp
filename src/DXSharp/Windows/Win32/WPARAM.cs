using Windows.Win32.Foundation ;

namespace DXSharp.Windows.Win32 {
	public readonly partial struct WParam {
		readonly nuint Value ;
		public WParam( nuint value ) => Value = value ;
		
		public static implicit operator nuint( WParam  value ) => (nuint)value.Value ;
		public static implicit operator WParam( nuint  value ) => new( value ) ;
		
		public static implicit operator WParam( WPARAM value ) => new( value.Value ) ;
		public static implicit operator WPARAM( WParam value ) => new( value.Value ) ;
		
		public static implicit operator WParam( LPARAM value ) => new WParam( (nuint)value.Value ) ;
		public static implicit operator LPARAM( WParam value ) => new LPARAM( (nint)value.Value ) ;
	} ;
}

namespace Windows.Win32.Foundation {
	public readonly struct WPARAM {
		public readonly nuint Value ;
		public WPARAM( nuint value ) => Value = value ;
		public static implicit operator nuint( WPARAM value ) => (nuint)value.Value ;
		public static implicit operator WPARAM( nuint value ) => new WPARAM( value ) ;
		public static implicit operator LPARAM( WPARAM value ) => new LPARAM( (nint)value.Value ) ;
		public static implicit operator WPARAM( LPARAM value ) => new WPARAM( (nuint)value.Value ) ;
	} ;
}