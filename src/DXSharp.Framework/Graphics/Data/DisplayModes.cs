#region Using Directives
using System.Collections ;
using System.Diagnostics ;
using DXSharp.Applications ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Framework.Graphics ;


/// <summary>
/// Represents a display mode, which is a combination of a <see cref="Resolution"/>,
/// a <see cref="RefreshRate"/>, and a flag indicating if the display is stereo.
/// </summary>
[DebuggerDisplay("ToString()")]
public readonly struct DisplayMode: IEquatable< DisplayMode > {
	/// <summary>The default resource format to use when none is specified.</summary>
	public const Format DefaultResourceFormat = Format.R8G8B8A8_UNORM ;
	/// <summary>A default display mode to use when none is specified.</summary>
	public static readonly DisplayMode Default = new( new( AppSettings.DEFAULT_WINDOW_SIZE ), 
													  (60, 1), false,
													  DefaultResourceFormat, AdditionalInfo.DefaultInfo ) ;
	
	// -----------------------------------------------------------------------------
	/// <summary>The refresh rate of the display mode.</summary>
	public RefreshRate RefreshRate { get ; }
	/// <summary>The resolution of the display mode.</summary>
	public Resolution Resolution { get ; }
	/// <summary>Indicates if the display mode is stereo.</summary>
	public bool IsStereo { get ; }
	/// <summary>The render target resource format of the mode.</summary>
	public Format ResourceFormat { get ; }
	/// <summary>Additional information about the display mode.</summary>
	public AdditionalInfo ExtendedInfo { get ; }
							= AdditionalInfo.DefaultInfo ;
	
	public ulong TotalPixels => ((ulong)Resolution.Width) * Resolution.Height ;
	
	
	 
	// -----------------------------------------------------------------------------
	
	/// <summary>
	/// Encapsulates less frequently used information
	/// about a display mode.
	/// </summary>
	/// <remarks>Includes the <see cref="Scaling"/> and <see cref="ScanlineOrder"/>.</remarks>
	public readonly struct AdditionalInfo: IEquatable<AdditionalInfo> {
		/// <summary>The default settings for less frequently used display configuration properties.</summary>
		public static readonly AdditionalInfo DefaultInfo = new( ) ;
		
		/// <summary>Indicates how the display mode is scaled.</summary>
		public ScalingMode Scaling { get ; } = ScalingMode.Unspecified ;
		/// <summary>Indicates the scanline order of the display mode.</summary>
		public ScanlineOrder ScanlineOrder { get ; } = ScanlineOrder.Unspecified ;
		
		
		/// <summary>
		/// Creates a new set of <see cref="AdditionalInfo"/> to provide
		/// extended information about a display mode.
		/// </summary>
		/// <param name="scaling">Indicates how the display mode is scaled.</param>
		/// <param name="scanlineOrder">Indicates the scanline order of the display mode.</param>
		public AdditionalInfo( ScalingMode scaling = ScalingMode.Unspecified,
							   ScanlineOrder scanlineOrder = ScanlineOrder.Unspecified ) {
			Scaling = scaling ;
			ScanlineOrder = scanlineOrder ;
		}
		

		public bool Equals( AdditionalInfo other ) => 
						(Scaling == other.Scaling) && 
							(ScanlineOrder == other.ScanlineOrder) ;
		public override bool Equals( object? obj ) => 
			obj is AdditionalInfo other && Equals(other) ||
			( obj is ValueTuple<ScalingMode, ScanlineOrder> values ) &&
					(Scaling == values.Item1) && (ScanlineOrder == values.Item2) ;
		public override int GetHashCode( ) => HashCode.Combine( Scaling, ScanlineOrder ) ;
		
		
		public override string ToString( ) => 
			$"\"{nameof(DisplayMode.AdditionalInfo)}\": {{ \"{nameof(ScalingMode)}\": \"{Scaling}\", " +
														$"\"{nameof(ScanlineOrder)}\": \"{ScanlineOrder}\" }}" ;
		

		
		public static implicit operator AdditionalInfo( in (ScalingMode scaling, ScanlineOrder scanlineOrder) values ) =>
			new( values.scaling, values.scanlineOrder ) ;
		
		public static bool operator ==( in AdditionalInfo left, in AdditionalInfo right ) =>
				(left.Scaling == right.Scaling) && (left.ScanlineOrder == right.ScanlineOrder) ;
		public static bool operator !=( in AdditionalInfo left, in AdditionalInfo right ) =>
				(left.Scaling != right.Scaling) || (left.ScanlineOrder != right.ScanlineOrder) ;
	}
	
	
	// -----------------------------------------------------------------------------
	
	public DisplayMode( Resolution resolution,
							RefreshRate refreshRate = default,
								bool isStereo = false,
									Format resourceFormat = 
										Format.R8G8B8A8_UNORM ) {
		Resolution = resolution ;
		RefreshRate = refreshRate ;
		IsStereo = isStereo ;
		ResourceFormat = resourceFormat ;
	}
	
	public DisplayMode( Resolution  resolution,
							RefreshRate refreshRate = default,
								bool isStereo = false,
									Format resourceFormat =
										Format.R8G8B8A8_UNORM, 
											AdditionalInfo extendedInfo 
												= default ):
										this( resolution, refreshRate, 
											  isStereo, resourceFormat ) => ExtendedInfo = extendedInfo ;

	// -----------------------------------------------------------------------------
	
	public override string ToString( ) {
		return   $"{nameof(DisplayMode)}: {Resolution} @ {RefreshRate}Hz" +
										$"{(IsStereo ? " (Stereo)" : string.Empty)}" +
										$"{(ResourceFormat != DefaultResourceFormat ? 
										$" ({ResourceFormat})" : string.Empty)}" +
										$"{(ExtendedInfo != AdditionalInfo.DefaultInfo ? 
										$" ({ExtendedInfo})" : string.Empty)}" ;
	}

	public override int GetHashCode( ) => HashCode.Combine( RefreshRate, Resolution, IsStereo, 
																ResourceFormat, ExtendedInfo ) ;
	
	public override bool Equals( object? obj ) => obj is DisplayMode other && Equals( other ) ;
	public bool Equals( DisplayMode other ) => RefreshRate == other.RefreshRate
												   && Resolution == other.Resolution
														&& IsStereo == other.IsStereo ;

	// -----------------------------------------------------------------------------
	// Operators:
	// -----------------------------------------------------------------------------
	
	//! Conversions: --------------------------------------------------------------
	public static implicit operator DisplayMode( in (RefreshRate refreshRate, Resolution resolution, 
													 bool isStereo, Format format) values ) =>
														new( values.resolution, values.refreshRate, 
															 values.isStereo, values.format ) ;
	
	public static implicit operator DisplayMode( in ModeDescription modeDesc ) =>
										new( ( modeDesc.Width, modeDesc.Height ), 
												modeDesc.RefreshRate, false, modeDesc.Format,
													new(modeDesc.Scaling, modeDesc.ScanlineOrdering) ) ;
	
	//! Comparisons: --------------------------------------------------------------
	public static bool operator ==( DisplayMode left, DisplayMode right ) =>
											left.Resolution == right.Resolution && 
												left.RefreshRate == right.RefreshRate && 
													left.IsStereo == right.IsStereo ;
	
	public static bool operator !=( DisplayMode left, DisplayMode right ) =>
	 										left.Resolution != right.Resolution || 
												left.RefreshRate != right.RefreshRate || 
													left.IsStereo != right.IsStereo ;
	// =============================================================================
} ;

