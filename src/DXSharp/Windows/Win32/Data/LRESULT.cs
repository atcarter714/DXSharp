using System.Diagnostics ;
using Windows.Win32.Foundation ;

namespace DXSharp.Windows.Win32 {
	[DebuggerDisplay( "{Value}" )]
	public readonly struct LResult: IEquatable< LResult >, 
										IEquatable<LRESULT> {
		public readonly nint Value ;
		public LResult( nint value ) => this.Value = value ;

		public override int GetHashCode( ) => this.Value.GetHashCode( ) ;
		public bool Equals( LResult other ) => ( this.Value == other.Value ) ;
		public bool Equals( LRESULT other ) => ( this.Value == other.Value ) ;

		public override bool Equals( object? obj )   => obj is LResult lr && Equals( lr ) ;

		public static implicit operator nint( LResult value ) => value.Value ;
		public static explicit operator LResult( nint value ) => new( value ) ;
		public static implicit operator LResult( LRESULT value ) => new( value.Value ) ;
		public static implicit operator LRESULT( LResult value ) => new( value.Value ) ;
		
		public static bool operator !=( LResult left, LResult right ) => !( left == right ) ;
		public static bool operator !=( LResult left, LRESULT right ) => !( left == right ) ;
		public static bool operator !=( LRESULT left, LResult right ) => !( left == right ) ;
		public static bool operator ==( LResult left, LResult right ) => left.Value == right.Value ;
		public static bool operator ==( LResult left, LRESULT right ) => left.Value == right.Value ;
		public static bool operator ==( LRESULT left, LResult right ) => left.Value == right.Value ;
	} ;
}

// --------------------------------------
// Windows.Win32.Foundation Version ::
// --------------------------------------

namespace Windows.Win32.Foundation {
	[DebuggerDisplay( "{Value}" )]
	public readonly struct LRESULT: IEquatable< LRESULT > {
		public readonly nint Value ;
		public LRESULT( nint value ) => this.Value = value ;

		public override int GetHashCode( ) => this.Value.GetHashCode( ) ;
		public bool Equals( LRESULT other ) => ( this.Value == other.Value ) ;
		public override bool Equals( object? obj ) => obj is LRESULT lr && Equals( lr ) ;
		
		public static implicit operator nint(LRESULT value) => value.Value ;
		public static explicit operator LRESULT(nint value) => new( value ) ;
		public static bool operator !=( LRESULT left, LRESULT right ) => !(left == right) ;
		public static bool operator ==( LRESULT left, LRESULT right ) => left.Value == right.Value ;
	} ;
}