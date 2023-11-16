#region Using Directives
using System.Collections ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.InteropServices ;
using DXSharp.DXGI ;

#endregion
namespace DXSharp.Framework.Graphics ;


/// <summary>A set of <see cref="DisplayMode"/> options for the graphics pipeline.</summary>
public sealed class DisplayOptions: IEnumerable< DisplayMode >,
									IEquatable< DisplayOptions > {
	// ----------------------------------------------------------------------------------
	/// <summary>
	/// A default set of common <see cref="DisplayMode"/> options
	/// which are generally supported by most graphics adapters and displays.
	/// </summary>
	/// <remarks>
	/// This is a good starting point for most applications, but you <i>cannot</i> rely
	/// on these modes being supported by local machine's graphics adapters and displays.
	/// </remarks>
	public static readonly IReadOnlyList< DisplayMode > DefaultModes = new DisplayMode[ ] {
		new ( new(1920, 1080),
						 ( 60, 1 ),
						 false, Format.R8G8B8A8_UNORM ),
		new ( new(1280, 720),
						 ( 60, 1 ),
						 false, Format.R8G8B8A8_UNORM ),
		new ( new(1920, 1080),
			  ( 60, 1 ),
			  false, Format.R8G8B8A8_UNORM_SRGB ),
		new ( new(1280, 720),
			  ( 60, 1 ),
			  false, Format.R8G8B8A8_UNORM_SRGB ),
		new ( new(1920, 1080),
			  ( 60, 1 ),
			  false, Format.B8G8R8A8_UNORM ),
	} ;
	/// <summary>The default set of <see cref="DisplayOptions"/> options.</summary>
	public static readonly DisplayOptions Default = new( DefaultModes ) ;
	// ----------------------------------------------------------------------------------
	
	/// <summary>Gets a list of available <see cref="DisplayMode"/> options.</summary>
	public List< DisplayMode > DisplayModes { get ; }
	
	/// <summary>Gets the <see cref="DisplayMode"/> at the specified index.</summary>
	public DisplayMode this[ int index ] => DisplayModes[ index ] ;
	
	/// <summary>Gets a reference to the <see cref="DisplayMode"/> at the specified index.</summary>
	public ref DisplayMode this[ uint index ] {
		get {
#if !STRIP_CHECKS
#if DEBUG || DEV_BUILD
			if( index >= DisplayModes.Count )
				throw new IndexOutOfRangeException( $"{nameof(index)}: {index}" ) ;
#endif
#endif
			
			var span = CollectionsMarshal.AsSpan( DisplayModes ) ;
			unsafe {
				fixed ( DisplayMode* ptr = span ) {
					return ref ptr[ index ] ;
				}
			}
		}
	}
	
	/// <summary>Gets the number of <see cref="DisplayMode"/> options.</summary>
	public int Count => DisplayModes.Count ;
	
	
	
	public DisplayOptions( ) =>
		DisplayModes = new( ) ;
	public DisplayOptions( IEnumerable< DisplayMode > options ) =>
		DisplayModes = new( options ) ;
	public DisplayOptions( params DisplayMode[ ] options ) =>
		DisplayModes = new( options ) ;
	public DisplayOptions( DisplayMode one ) {
		DisplayModes = new( ) ;
		DisplayModes.Add( one ) ;
	}
	
	// ----------------------------------------------------------------------------------
	
	public DisplayOptions WithFilter( Func< DisplayMode, bool > filter ) {
		var newOptions = new DisplayOptions( ) ;
		foreach( var mode in DisplayModes )
			if( filter( mode ) )
				newOptions.DisplayModes.Add( mode ) ;
		return newOptions ;
	}
	
	public DisplayMode? FindBestMatchFor( DisplayMode targetMode ) {
		const float weightResolution  = 0.64f,
					weightRefreshRate = 0.48f ;
		
		float bestMatchScore = 0.0f ;
		DisplayMode bestMode = default ;
		
		foreach( var availableMode in DisplayModes ) {
			// Skip modes that are not compatible:
			if( availableMode.IsStereo != targetMode.IsStereo 
					|| availableMode.ResourceFormat != targetMode.ResourceFormat )
						continue ;
			
			// Get composite comparison score:
			float compositeScore = GetComparisonScoreFor( targetMode, availableMode, 
														  weightRefreshRate, weightResolution ) ;
			
			// Check if this is the best match so far:
			if( compositeScore > bestMatchScore ) {
				bestMatchScore = compositeScore ;
				bestMode = availableMode ;
			}
		}
		
		return bestMode.Resolution.Area > 0 ? 
				   bestMode : null  ;
	}
	
	
	public IEnumerable< DisplayMode > GetVRStereoModes( ) => 
		DisplayModes.Where( mode => mode.IsStereo ) ;
	
	public IEnumerable< DisplayMode > GetNonVRStereoModes( ) =>
	 		DisplayModes.Where( mode => !mode.IsStereo ) ;
	
	public IEnumerable< DisplayMode > GetModesWithFormat( Format format ) =>
		DisplayModes.Where( mode => mode.ResourceFormat == format ) ;
	
	public DisplayMode GetModeWithHighestResolutionAndHz( ) {
		DisplayMode bestMode = default ;
		foreach( var mode in DisplayModes ) {
			if( mode.Resolution.Area > bestMode.Resolution.Area )
				bestMode = mode ;
			else if( mode.Resolution.Area == bestMode.Resolution.Area ) {
				if( mode.RefreshRate > bestMode.RefreshRate )
					bestMode = mode ;
			}
		}
		return bestMode ;
	}
	
	// ----------------------------------------------------------------------------------
	// Mode Ranking Functions:
	// ----------------------------------------------------------------------------------
	
	internal static float GetComparisonScoreFor( DisplayMode targetMode,
												 DisplayMode otherMode,
												 float weightRefreshRate = 0.32f ,
												 float weightResolution  = 1.10f ) {
		int totalPoints = 0 ;
		float compositeScore   = 0.0f,
			  resolutionScore  = 0.0f,
			  refreshRateScore = 0.0f ;
			
		// Get score for resolution:
		resolutionScore = _scoreResolutionDifference( otherMode.Resolution, targetMode.Resolution,
													  ref totalPoints, ref compositeScore ) ;
			
		// Get points for refresh rate and compute score:
		int refreshRatePts = _rankRefreshRateDifference( otherMode.RefreshRate, 
														 targetMode.RefreshRate,
														 ref totalPoints ) ;
		refreshRateScore = (refreshRatePts * weightRefreshRate) ;
			
		// Compute composite score:
		compositeScore = ( (resolutionScore * weightResolution) 
						   + refreshRateScore) ;
		return compositeScore ;
	}

	
	static float _scoreResolutionDifference( in Resolution available,
											 in Resolution desiredMode,
											 ref int currentTotalPoints,
											 ref float currentTotalScore ) {
		const float weightWidth       = 0.64f,
					weightHeight      = 0.48f,
					weightLarger      = 1.28f,
					weightSmaller	  = 0.64f,
					weightArea        = 1.00f,
					weightAspect 	  = 1.10f,
					weightAspectSame  = 1.28f,
					weightAspectDiff  = 0.88f,
					weightTotalBonus  = 0.10f ;
		
		// Get differences in width and height:
		int diffWidth  = System.Math.Abs( (int)available.Width - (int)desiredMode.Width ),
			diffHeight = System.Math.Abs( (int)available.Height - (int)desiredMode.Height ) ;
		
		// Get differences in area:
		bool largerA  = available.Area > desiredMode.Area,
			 largerB  = desiredMode.Area > available.Area,
			 sameArea = available.Area == desiredMode.Area ;
		int diffArea  = System.Math.Abs( (int)available.Area - (int)desiredMode.Area ) ;
		
		// Get differences in aspect ratio:
		float aspectDiff = System.Math.Abs( available.AspectRatio - desiredMode.AspectRatio ) ;
		bool  sameAspect = Mathf.Approximately( available.AspectRatio, desiredMode.AspectRatio ) ;
		
		
		int totalPoints = 0, 
			widthPts    = 0, heightPts = 0, 
			aspectPts   = 0, areaPts   = 0 ;
		float localCompositeScore = 0.0f ;
		widthPts  = _getPointsForDimension( diffWidth, ref totalPoints ) ;
		heightPts = _getPointsForDimension( diffHeight, ref totalPoints ) ;
		areaPts   = _getPointsPixelDifference( diffArea, ref totalPoints ) ;
		aspectPts = _getPointsAspectDifference( aspectDiff, ref totalPoints ) ;
		
		// Calculate composite score:
		localCompositeScore = (widthPts * weightWidth) + (heightPts * weightHeight) +
							  (areaPts * weightArea) + (aspectPts * weightAspect) ;
		
		// Apply weight for larger/smaller/match:
		localCompositeScore *= largerA ? weightLarger 
								: (largerB ? weightSmaller : 1.0f) ;
		// Apply weight for aspect ratio:
		if( sameAspect ) localCompositeScore *= weightAspectSame ;
		else localCompositeScore            *= weightAspectDiff ;
		
		// Apply bonus for total points:
		localCompositeScore += (totalPoints * weightTotalBonus) ;
		
		// Apply & return results:
		currentTotalScore += localCompositeScore ;
		currentTotalPoints += totalPoints ;
		return localCompositeScore ;
	}
	
	static int _getPointsForDimension( int diff, ref int points ) {
		int localScore = 0 ;
		
		// Rank width difference:
		if( diff is 0 )
			localScore += 5 ;
		
		else if( diff <= 16 )
			localScore += 4 ;
		
		else if( diff <= 32 )
			localScore += 3 ;
		
		else if( diff <= 64 )
			localScore += 2 ;
		
		else if( diff <= 128 )
			localScore += 1 ;
		
		else if( diff <= 256 )
			;
		
		else if( diff <= 512 )
			localScore -= 1 ;
		
		else if( diff <= 1024 )
			localScore -= 2 ;
		
		else if( diff <= 2048 )
			localScore -= 3 ;
		
		else if( diff <= 4096 )
			localScore -= 4 ;
		
		points += localScore ;
		return localScore ;
	}
	
	static int _getPointsAspectDifference( float diff, ref int points ) {
		int localScore = 0 ;
		
		if( Mathf.Approximately( diff, 0.0f ) )
			localScore += 5 ;
		
		else if( diff <= 0.10f )
			localScore += 4 ;
		
		else if( diff <= 0.20f )
			localScore += 3 ;
		
		else if( diff <= 0.40f )
			localScore += 2 ;
		
		else if( diff <= 0.80f )
			localScore += 1 ;
		
		else if( diff <= 1.0f )
			;
		
		else if( diff <= 2.0f )
			localScore -= 1 ;
		
		else if( diff <= 4.0f )
			localScore -= 2 ;
		
		else if( diff <= 8.0f )
			localScore -= 3 ;
		
		else if( diff <= 16.0f )
			localScore -= 4 ;
		
		points += localScore ;
		return localScore ;
	}
	
	static int _getPointsPixelDifference( int diffPixels, ref int score ) {
		int localScore = 0 ;
		
		if( diffPixels <= 16 )
			localScore += 4 ;
		
		else if( diffPixels <= 32 )
			localScore += 3 ;
		
		else if( diffPixels <= 64 )
			localScore += 2 ;
		
		else if( diffPixels <= 128 )
			localScore += 1 ;
			
		else if( diffPixels <= 256 )
			;
		
		else if( diffPixels <= 512 )
			localScore -= 1 ;
		
		else if( diffPixels <= 1024 )
			localScore -= 2 ;
		
		else if( diffPixels <= 2048 )
			localScore -= 3 ;
		
		else if( diffPixels <= 4096 )
			localScore -= 4 ;
		
		score += localScore ;
		return localScore ;
	}
	
	static int _rankRefreshRateDifference( in Rational a, in Rational b, ref int score ) {
		int localScore = 0 ;
		
		float fracA = (float)a.Numerator / (float)a.Denominator,
			  fracB = (float)b.Numerator / (float)b.Denominator ;

		if ( Mathf.Approximately( fracA, fracB ) ) {
			localScore += 5 ;
		}
		else {
			float diff = System.Math.Abs( fracA - fracB ) ;
			if( diff <= 1.0f )
				localScore += 4 ;
			
			else if( diff <= 4.0f )
				localScore += 3 ;
			
			else if( diff <= 8.0f )
				localScore += 2 ;
			
			else if( diff <= 16.0f )
				localScore += 1 ;
			
			else if( diff <= 24.0f )
				;
			
			else if( diff <= 32.0f )
				localScore -= 1 ;
			
			else if( diff <= 48.0f )
				localScore -= 2 ;
			
			else if( diff <= 64.0f )
				localScore -= 3 ;
			
			else if( diff <= 128.0f )
				localScore -= 4 ;
			
			else if( diff <= 256.0f )
				localScore -= 5 ;
		}
		
		score += localScore ;
		return localScore ;
	}
	
	// ----------------------------------------------------------------------------------
	

	
	// ----------------------------------------------------------------------------------
	#region Interface Implementations & Overrides
	public IEnumerator< DisplayMode > GetEnumerator( ) => 
		DisplayModes.GetEnumerator( ) ;
	IEnumerator IEnumerable.GetEnumerator( ) => 
		((IEnumerable)DisplayModes).GetEnumerator( ) ;
	
	public override int GetHashCode( ) => HashCode.Combine( DisplayModes.GetHashCode( ), 
															DisplayModes.Count ) ;

	public override bool Equals( object? obj ) => 
		obj is DisplayOptions other && Equals(other) ;
	
	public bool Equals( DisplayOptions? other ) => 
		( (ReferenceEquals( other?.DisplayModes, this.DisplayModes ) ||
		   (other?.DisplayModes?.SequenceEqual(DisplayModes) ?? false)) ) ;
	#endregion
	// ==================================================================================
} ;






		/*int _rankSizeDifference( int diffWidth, int diffHeight, ref int score ) {
			int localScore = 0 ;
			if( diffWidth <= 16 && diffHeight <= 16 )
				localScore += 4 ;
			
			else if( diffWidth <= 32 && diffHeight <= 32 )
				localScore += 3 ;
				
			else if( diffWidth <= 64 && diffHeight <= 64 )
				localScore += 2 ;
			
			else if( diffWidth <= 128 && diffHeight <= 128 )
				localScore += 1 ;
				
			else if( diffWidth <= 256 && diffHeight <= 256 )
				;
				
			else if( diffWidth <= 512 && diffHeight <= 512 )
				localScore -= 1 ;

			else if( diffWidth <= 1024 && diffHeight <= 1024 )
				localScore -= 2 ;
			
			else if( diffWidth <= 2048 && diffHeight <= 2048 )
				localScore -= 3 ;
			
			else if( diffWidth <= 4096 && diffHeight <= 4096 )
				localScore -= 4 ;
			
			score += localScore ;
			return localScore ;
		}*/