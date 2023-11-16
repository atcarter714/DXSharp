#region Using Directives
using System.Drawing ;
using System.Collections ;
using System.Diagnostics ;
using System.Collections.ObjectModel ;
using System.Runtime.InteropServices ;

using DXSharp.DXGI ;
#endregion
namespace DXSharp.Framework.Graphics ;


/// <summary>Represents a display output refresh rate.</summary>
public readonly struct RefreshRate: IEquatable< RefreshRate >, 
									IFormattable {
	public static readonly RefreshRate Zero = new( 0, 0 ) ;
	public static readonly RefreshRate _60Hz = new( 60, 1 ) ;
	public static readonly RefreshRate _120Hz = new( 120, 1 ) ;
	public static readonly RefreshRate _144Hz = new( 144, 1 ) ;
	public static readonly RefreshRate _240Hz = new( 240, 1 ) ;
	public static readonly RefreshRate _360Hz = new( 360, 1 ) ;
	
	
	/// <summary>The refresh rate in Hertz (Hz).</summary>
	public Rational Hz { get ; }
	/// <summary>The numerator of the refresh rate in Hertz (Hz).</summary>
	public uint Numerator => Hz.Numerator ;
	/// <summary>The denominator of the refresh rate in Hertz (Hz).</summary>
	public uint Denominator => Hz.Denominator ;
	/// <summary>The maximum frames per second (FPS) for the refresh rate.</summary>
	public uint MaxFPS => Hz.Numerator / Hz.Denominator ;
	
	
	public RefreshRate( Rational hz = default ) => Hz = hz ;
	public RefreshRate( uint numerator, uint denominator = 1 ) {
		Hz = ( numerator, denominator ) ;
	}
	
	
	public override int GetHashCode( ) => Hz.GetHashCode( ) ;
	public bool Equals( RefreshRate other ) => Hz.Equals( other.Hz ) ;
	public override bool Equals( object? obj ) => obj is RefreshRate other && Equals( other ) ;

	public override string ToString( ) =>
		Hz.Numerator switch {
			not 0 when Hz.Denominator is 1     => $"{Hz.Numerator:G}Hz",
			not 0 when Hz.Denominator is not 1 => $"{Hz.Numerator:G}/{Hz.Denominator:G}Hz",
			_                                  => throw new DivideByZeroException( ),
		} ;
	
	public string ToString( string? fmtStr, IFormatProvider? formatProvider ) {
		if ( string.IsNullOrEmpty( fmtStr ) ) fmtStr = "G" ;
		if ( formatProvider is null ) 
			formatProvider = System.Globalization.CultureInfo.CurrentCulture ;
		
		return fmtStr.ToUpperInvariant( ) switch {
			"G"  => $"{Hz.Numerator:G}Hz" + (Hz.Denominator is not 1 ? $"/{Hz.Denominator:G}" : string.Empty ),
			"X"  => $"{Hz.Numerator:X}Hz" + (Hz.Denominator is not 1 ? $"/{Hz.Denominator:X}" : string.Empty ),
			"XG" => $"{Hz.Numerator:XG}Hz" + (Hz.Denominator is not 1 ? $"/{Hz.Denominator:XG}" : string.Empty ),
			_    => throw new FormatException( $"The {fmtStr} format string is not supported." ),
		} ;
	}

	
	
	#region Operators
	public static implicit operator Rational( in RefreshRate rate ) => rate.Hz ;
	public static implicit operator RefreshRate( in Rational hz ) => new( hz ) ;
	public static implicit operator RefreshRate( in (uint numerator, uint denominator) hz ) => 
															new( hz.numerator, hz.denominator ) ;
	
	public static bool operator ==( in RefreshRate left, in RefreshRate right ) => left.Hz == right.Hz ;
	public static bool operator !=( in RefreshRate left, in RefreshRate right ) => left.Hz != right.Hz ;
	public static bool operator < ( in RefreshRate left, in RefreshRate right ) => left.Hz <  right.Hz ;
	public static bool operator > ( in RefreshRate left, in RefreshRate right ) => left.Hz >  right.Hz ;
	public static bool operator <=( in RefreshRate left, in RefreshRate right ) => left.Hz <= right.Hz ;
	public static bool operator >=( in RefreshRate left, in RefreshRate right ) => left.Hz >= right.Hz ;
	
	public static bool operator < ( in RefreshRate left, in Rational    right ) => left.Hz <  right ;
	public static bool operator > ( in RefreshRate left, in Rational    right ) => left.Hz >  right ;
	public static bool operator <=( in RefreshRate left, in Rational    right ) => left.Hz <= right ;
	public static bool operator >=( in RefreshRate left, in Rational    right ) => left.Hz >= right ;
	public static bool operator ==( in RefreshRate left, in Rational    right ) => left.Hz == right ;
	public static bool operator !=( in RefreshRate left, in Rational    right ) => left.Hz != right ;
	public static bool operator < ( in Rational    left, in RefreshRate right ) => left <  right.Hz ;
	public static bool operator > ( in Rational    left, in RefreshRate right ) => left >  right.Hz ;
	public static bool operator <=( in Rational    left, in RefreshRate right ) => left <= right.Hz ;
	public static bool operator >=( in Rational    left, in RefreshRate right ) => left >= right.Hz ;
	public static bool operator ==( in Rational    left, in RefreshRate right ) => left == right.Hz ;
	public static bool operator !=( in Rational    left, in RefreshRate right ) => left != right.Hz ;
	 
	public static bool operator < ( in RefreshRate left, float right ) => left.Hz < right ;
	public static bool operator > ( in RefreshRate left, float right ) => left.Hz > right ;
	public static bool operator <=( in RefreshRate left, float right ) => left.Hz <= right ;
	public static bool operator >=( in RefreshRate left, float right ) => left.Hz >= right ;
	public static bool operator < ( float left, in RefreshRate right ) => left <  right.Hz ;
	public static bool operator > ( float left, in RefreshRate right ) => left >  right.Hz ;
	public static bool operator <=( float left, in RefreshRate right ) => left <= right.Hz ;
	public static bool operator >=( float left, in RefreshRate right ) => left >= right.Hz ;
	#endregion
} ;
