#region Using Directives
using System.Collections ;
using System.Collections.ObjectModel ;
using System.Diagnostics ;
using System.Drawing ;
using System.Runtime.InteropServices ;
using DXSharp ;
using DXSharp.DXGI ;
#endregion
namespace AdvancedDXS.Framework.Graphics ;

/* "Standard" PC monitor resolutions include:
		1280 x 1024		"Super-eXtended Graphics Array (SXGA)"
		1366 x 768		"High Definition (HD)"
		1600 x 900		"High Definition Plus (HD+)"
		1920 x 1080		"Full High Definition (FHD)"
		1920 x 1200		"Wide Ultra Extended Graphics Array (WUXGA)"
		2560 x 1440		"Quad High Definition (QHD)"
		3440 x 1440		"Wide Quad High Definition (WQHD)"
		3840 x 2160		"4K" or "Ultra High Definition (UHD)"
 */



/// <summary>Represents a screen or image resolution.</summary>
/// <remarks>
/// Note that <see cref="Resolution"/> is implicitly convertible to <see cref="USize"/>,
/// as well as <see cref="System.Drawing.Size"/> and <see cref="ValueTuple{T1, T2}"/> of
/// two <see cref="uint"/> or <see cref="int"/> values (for the <see cref="Width"/> and <see cref="Height"/>).
/// </remarks>
[DebuggerDisplay("{Width}x{Height}")]
[StructLayout(LayoutKind.Sequential, Size = SizeInBytes)]
public readonly struct Resolution: IEquatable< Resolution>,
								   IEquatable< USize >, IFormattable {
	/// <summary>
	/// The size of a <see cref="Resolution"/> value
	/// (i.e., total <see cref="byte"/> count).
	/// </summary>
	public const int SizeInBytes = sizeof( uint ) * 2 ;
	
	
	/// <summary>The size (in pixels or "units") of this <see cref="Resolution"/> value.</summary>
	public USize Size { get ; }
	/// <summary>The width (in pixels or "units") of this <see cref="Resolution"/> value.</summary>
	public uint Width => Size.Width ;
	/// <summary>The height (in pixels or "units") of this <see cref="Resolution"/> value.</summary>
	public uint Height => Size.Height ;
	
	public Resolution( USize size ) => Size = size ;
	public Resolution( uint width, uint height ) => Size = ( width, height ) ;
	public Resolution( System.Drawing.Size size ) => Size = ( (uint)size.Width, (uint)size.Height ) ;
	public Resolution( System.Drawing.SizeF size ) => Size = ( (uint)float.Round(size.Width), 
															   (uint)float.Round(size.Height) ) ;

	
	public override int GetHashCode( ) => Size.GetHashCode( ) ;
	public bool Equals( Resolution other ) => this.Size == other.Size ;
	public bool Equals( USize other ) => this.Size == other ;
	public override bool Equals( object? obj ) => 
			obj is Resolution other && (this.Size == other.Size)
					|| obj is USize size && (this.Size == size) ;


	public override string ToString( ) => $"{Width}x{Height}" ;
	public string ToString( string? format, IFormatProvider? formatProvider ) {
		if ( string.IsNullOrEmpty( format ) ) format = "G" ;
		if ( formatProvider is null ) formatProvider = System.Globalization.CultureInfo.CurrentCulture ;
		
		bool isStdRes = StandardResolutionNames.ContainsKey( this ) ;
		return format.ToUpperInvariant( ) switch {
			"G"  => $"{Width:G}x{Height:G}",
			"X"  => $"{Width:X}x{Height:X}",
			"XG" => $"{Width:XG}x{Height:XG}" + (isStdRes ? "({StandardResolutionNames[ this ]})" : string.Empty),
			_    => throw new FormatException( $"The {format} format string is not supported." ),
		} ;
	}

	// -----------------------------------------------------------------------------------------------------------------
	public static implicit operator USize( Resolution res )  => res.Size ;
	public static implicit operator Resolution( USize size ) => new( size ) ;
	public static implicit operator Size( Resolution res ) => new( (int)res.Width, (int)res.Height ) ;
	public static implicit operator Resolution( Size size ) => new( (uint)size.Width, (uint)size.Height ) ;
	
	public static implicit operator Resolution( (uint width, uint height) size ) => new( size ) ;
	public static implicit operator Resolution( (int width, int height) size ) => new( (uint)size.width, (uint)size.height ) ;
	
	public static bool operator ==( Resolution left, Resolution right ) => left.Size == right.Size ;
	public static bool operator !=( Resolution left, Resolution right ) => left.Size != right.Size ;
	
	/// <summary>Lookup table of standard resolution names, keyed by their <see cref="Resolution"/> value.</summary>
	public static readonly ReadOnlyDictionary< Resolution, string > StandardResolutionNames =
		new( new Dictionary< Resolution, string >( ) {
			{ (1280U, 1024U),		"Super-eXtended Graphics Array (SXGA)" },
			{ (1366U, 768U),		"High Definition (HD)" },
			{ (1600U, 900U),		"High Definition Plus (HD+)" },
			{ (1920U, 1080U),		"Full High Definition (FHD)" },
			{ (1920U, 1200U),		"Wide Ultra Extended Graphics Array (WUXGA)" },
			{ (2560U, 1440U),		"Quad High Definition (QHD)" },
			{ (3440U, 1440U),		"Wide Quad High Definition (WQHD)" },
			{ (3840U, 2160U),		"4K Ultra High Definition (UHD)" },
			} ) ;
	// =================================================================================================================
} ;


/// <summary>Represents a display output refresh rate.</summary>
public readonly struct RefreshRate: IEquatable< RefreshRate > {
	
	/// <summary>The refresh rate in Hertz (Hz).</summary>
	public Rational Hz { get ; }
	/// <summary>The numerator of the refresh rate in Hertz (Hz).</summary>
	public uint Numerator => Hz.Numerator ;
	/// <summary>The denominator of the refresh rate in Hertz (Hz).</summary>
	public uint Denominator => Hz.Denominator ;
	
	
	public RefreshRate( Rational hz ) => Hz = hz ;
	public RefreshRate( uint numerator, uint denominator ) {
		Hz = ( numerator, denominator ) ;
	}
	
	
	public override int GetHashCode( ) => Hz.GetHashCode( ) ;
	public bool Equals( RefreshRate other ) => Hz.Equals( other.Hz ) ;
	public override bool Equals( object? obj ) => obj is RefreshRate other && Equals( other ) ;
	
	
	public static implicit operator Rational( RefreshRate rate ) => rate.Hz ;
	public static implicit operator RefreshRate( Rational hz ) => new( hz ) ;
	
	public static bool operator ==( RefreshRate left, RefreshRate right ) => left.Hz == right.Hz ;
	public static bool operator !=( RefreshRate left, RefreshRate right ) => left.Hz != right.Hz ;
} ;


/// <summary>
/// Represents a display mode, which is a combination of a <see cref="Resolution"/>,
/// a <see cref="RefreshRate"/>, and a flag indicating if the display is stereo.
/// </summary>
[DebuggerDisplay("{Resolution} @ {RefreshRate}Hz")]
public readonly struct DisplayMode: IEquatable< DisplayMode > {
	/// <summary>The refresh rate of the display mode.</summary>
	public RefreshRate RefreshRate { get ; }
	/// <summary>The resolution of the display mode.</summary>
	public Resolution Resolution { get ; }
	/// <summary>Indicates if the display mode is stereo.</summary>
	public bool IsStereo { get ; }
	
	
	public DisplayMode( Resolution resolution,
							RefreshRate refreshRate = default,
								bool isStereo = false ) {
		Resolution = resolution ;
		RefreshRate = refreshRate ;
		IsStereo = isStereo ;
	}
	
	
	public override int GetHashCode( ) => HashCode.Combine( RefreshRate, Resolution, IsStereo ) ;
	public override bool Equals( object? obj ) => obj is DisplayMode other && Equals( other ) ;
	public bool Equals( DisplayMode other ) => RefreshRate == other.RefreshRate
												   && Resolution == other.Resolution
														&& IsStereo == other.IsStereo ;


	public static implicit operator DisplayMode( in (RefreshRate refreshRate, Resolution resolution, bool isStereo) values ) =>
		new( values.resolution, values.refreshRate, values.isStereo ) ;
	
	public static bool operator ==( DisplayMode left, DisplayMode right ) =>
		left.Resolution == right.Resolution && left.RefreshRate == right.RefreshRate && left.IsStereo == right.IsStereo ;
	public static bool operator !=( DisplayMode left, DisplayMode right ) =>
	 		left.Resolution != right.Resolution || left.RefreshRate != right.RefreshRate || left.IsStereo != right.IsStereo ;
} ;



public sealed class DisplayOptions: IEnumerable< DisplayMode > {
	public List< DisplayMode > AvailableDisplayModes { get ; }
	
	public DisplayOptions( ) =>
		AvailableDisplayModes = new( ) ;
	public DisplayOptions( IEnumerable<DisplayMode> options ) =>
		AvailableDisplayModes = new( options ) ;
	public DisplayOptions( params DisplayMode[ ] options ) =>
		AvailableDisplayModes = new( options ) ;

	public IEnumerator< DisplayMode > GetEnumerator( ) => AvailableDisplayModes.GetEnumerator( ) ;
	IEnumerator IEnumerable.GetEnumerator( ) => ( (IEnumerable)AvailableDisplayModes ).GetEnumerator( ) ;
} ;


public class GraphicsSettings {
	public static readonly GraphicsSettings Default = new( ) {
		BufferCount = 2,
	} ;
	
	public uint BufferCount { get ; set ; } = 2 ;
	public DisplayOptions? DisplayOptions { get ; set ; }
	
	
	public GraphicsSettings( ) { }
	public GraphicsSettings( uint bufferCount ) {
		BufferCount = bufferCount ;
	}
} ;

