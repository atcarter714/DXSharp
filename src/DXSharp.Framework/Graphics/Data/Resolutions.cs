#region Using Directives
using System.Drawing ;
using System.Diagnostics ;
using System.Collections.ObjectModel ;
using System.Runtime.InteropServices ;
#endregion
namespace DXSharp.Framework.Graphics ;


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
	
	public static readonly Resolution Empty = new( 0, 0 ) ;
	public static readonly Resolution SXGA = new( 1280, 1024 ) ;
	public static readonly Resolution HD = new( 1366, 768 ) ;
	public static readonly Resolution HDPlus = new( 1600, 900 ) ;
	public static readonly Resolution FHD = new( 1920, 1080 ) ;
	public static readonly Resolution WUXGA = new( 1920, 1200 ) ;
	public static readonly Resolution QHD = new( 2560, 1440 ) ;
	public static readonly Resolution WQHD = new( 3440, 1440 ) ;
	public static readonly Resolution UHD = new( 3840, 2160 ) ;
	public static readonly Resolution FUHD = new( 7680, 4320 ) ;
	public static readonly Resolution QUHD = new( 15360, 8640 ) ;
	public static readonly Resolution[ ] StandardResolutions = new[ ] {
		SXGA, HD, HDPlus, FHD, WUXGA, QHD, WQHD, UHD, FUHD, QUHD
	} ;
	
	
	/// <summary>The size (in pixels or "units") of this <see cref="Resolution"/> value.</summary>
	public USize Size { get ; }
	/// <summary>The width (in pixels or "units") of this <see cref="Resolution"/> value.</summary>
	public uint Width => Size.Width ;
	/// <summary>The height (in pixels or "units") of this <see cref="Resolution"/> value.</summary>
	public uint Height => Size.Height ;
	/// <summary>The aspect ratio of this <see cref="Resolution"/> value.</summary>
	public float AspectRatio => ( (float)Width / (float)Height ) ;
	/// <summary>The area (in pixels or "units") of this <see cref="Resolution"/> value.</summary>
	public uint Area => (Width * Height) ;

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
	
	public static bool operator ==( Resolution left, USize right ) => left.Size == right ;
	public static bool operator !=( Resolution left, USize right ) => left.Size != right ;
	 
	public static bool operator ==( USize left, Resolution right ) => left == right.Size ;
	public static bool operator !=( USize left, Resolution right ) => left != right.Size ;
	
	public static bool operator >( Resolution left, Resolution right ) => left.Size > right.Size ;
	public static bool operator <( Resolution left, Resolution right ) => left.Size < right.Size ;
	
	public static bool operator >=( Resolution left, Resolution right ) => left.Size >= right.Size ;
	public static bool operator <=( Resolution left, Resolution right ) => left.Size <= right.Size ;
	
	public static bool operator >( Resolution left, USize right ) => left.Size > right ;
	public static bool operator <( Resolution left, USize right ) => left.Size < right ;
	
	public static bool operator >=( Resolution left, USize right ) => left.Size >= right ;
	public static bool operator <=( Resolution left, USize right ) => left.Size <= right ;
	
	
	/// <summary>Lookup table of standard resolution names, keyed by their <see cref="Resolution"/> value.</summary>
	public static readonly ReadOnlyDictionary< Resolution, string > StandardResolutionNames =
		new( new Dictionary< Resolution, string >( ) {
			{ Empty,	"Empty" },
			{ SXGA,		"Super-eXtended Graphics Array (SXGA)" },
			{ HD,		"High Definition (HD)" },
			{ HDPlus,	"High Definition Plus (HD+)" },
			{ FHD,		"Full High Definition (FHD)" },
			{ FUHD,		"Full Ultra High Definition (FUHD)" },
			{ QUHD,		"Quad Ultra High Definition (QUHD)" },
			{ WUXGA,	"Wide Ultra Extended Graphics Array (WUXGA)" },
			{ QHD,		"Quad High Definition (QHD)" },
			{ WQHD,     "Wide Quad High Definition (WQHD)" },
			{ UHD,		"4K Ultra High Definition (UHD)" },
			{ FUHD,		"Full Ultra High Definition (FUHD)" },
			{ QUHD,		"Quad Ultra High Definition (QUHD)" },
			} ) ;
	// =================================================================================================================
} ;



