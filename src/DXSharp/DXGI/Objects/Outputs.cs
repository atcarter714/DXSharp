#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Proxy contract for the native <see cref="IDXGIOutput"/> COM interface.</summary>
public class Output: Object, IOutput,
					 DXGIWrapper< IDXGIOutput > {
	#region Internal Constructors
	internal Output( nint ptr ): base( ptr ) =>
		this.COMObject ??= COMUtility.GetDXGIObject< IDXGIOutput >( ptr )
						   ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
																$"The internal COM interface is destroyed/null." ) ;
	internal Output( in IDXGIOutput dxgiObj ):
		base( COMUtility.GetInterfaceAddress( dxgiObj ) ) =>
		this.COMObject = dxgiObj ?? throw new ArgumentNullException( nameof(dxgiObj) ) ;
	#endregion
	
	
	public IDXGIOutput? COMObject { get ; init ; }
	protected IDXGIOutput? _idxgiOutput ;
	protected IDXGIOutput dxgiOutput => _idxgiOutput ??=
				Marshal.GetObjectForIUnknown( this.BasePointer ) as IDXGIOutput 
					?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														 $"The internal COM interface is destroyed/null." ) ;
	

	public void GetDescription( out OutputDescription pDescription ) {
		pDescription = new OutputDescription( ) ;
		unsafe {
			DXGI_OUTPUT_DESC result = default ;
			dxgiOutput.GetDesc( &result ) ;
			pDescription = new OutputDescription( result ) ;
		}
	}

	public uint GetDisplayModeCount( Format enumFormat, uint flags ) {
		uint modeCount = 0U ;
		unsafe {
			dxgiOutput.GetDisplayModeList( (DXGI_FORMAT)enumFormat,
										   flags, ref modeCount ) ;
		}
		return modeCount ;
	}
	
	public void GetDisplayModeList( Format enumFormat,
									uint flags, out uint pNumModes,
									out Span< ModeDescription > pDescription ) {
		pDescription = default ; pNumModes = 0U ; uint modeCount = 0U ;
		
		unsafe {
			// First, call the function just to get the count (no pointer for results):
			dxgiOutput.GetDisplayModeList( (DXGI_FORMAT)enumFormat,
										   flags, ref modeCount ) ;
			pNumModes = modeCount ;
			if ( pNumModes is 0U ) return ;
			
			// Now, allocate the memory and call the function again:
			var _alloc = stackalloc DXGI_MODE_DESC[ (int)pNumModes ] ;
			
			// This time, we have a pointer telling it where to write the results:
			dxgiOutput.GetDisplayModeList( (DXGI_FORMAT)enumFormat,
										   flags, ref pNumModes,
											_alloc ) ; // (ptr to stack allocation)
			
			// Initialize the Span (out) with the pointer and length:
			var descSpan = new Span< ModeDescription >( _alloc, (int)pNumModes ) ;
			pDescription = new ModeDescription[ pNumModes ] ;
			descSpan.CopyTo( pDescription ) ;
			
			// Copies the results into the managed array:
			/*pDescription = new ModeDescription[ pNumModes ] ;
			for ( int i = 0 ; i < pNumModes && i < pDescription.Length ; ++i )
				pDescription[ i ] = new( _alloc[ i ] ) ;*/
		}
	}

	public void FindClosestMatchingMode( in ModeDescription pModeToMatch, 
										 out ModeDescription pClosestMatch,
										 IUnknownWrapper pConcernedDevice ) {
		pClosestMatch = default ;
		unsafe {
			DXGI_MODE_DESC result = default, modeToMatch_ = pModeToMatch ;
			dxgiOutput.FindClosestMatchingMode( &modeToMatch_, &result,
											   pConcernedDevice ) ;
			pClosestMatch = new( result ) ;
		}
	}

	public void WaitForVBlank( ) => dxgiOutput.WaitForVBlank( ) ;

	public void TakeOwnership( IUnknownWrapper pDevice, bool exclusive ) {
		if ( pDevice is null ) throw new ArgumentNullException( nameof(pDevice) ) ;
		dxgiOutput.TakeOwnership( pDevice, exclusive ) ;
	}

	public void ReleaseOwnership( ) => dxgiOutput.ReleaseOwnership( ) ;

	public void GetGammaControlCapabilities( out GammaControlCapabilities pGammaCaps ) {
		pGammaCaps = default ;
		unsafe {
			DXGI_GAMMA_CONTROL_CAPABILITIES result = default ;
			dxgiOutput.GetGammaControlCapabilities( &result ) ;
			pGammaCaps = new( result ) ;
		}
	}

	public void SetGammaControl( in GammaControl pGammaData ) {
		unsafe { fixed( GammaControl* pGamma = &pGammaData )
				dxgiOutput.SetGammaControl( (DXGI_GAMMA_CONTROL *)pGamma ) ; }
	}

	public void GetGammaControl( out GammaControl pGammaData ) {
		pGammaData = default ;
		unsafe {
			DXGI_GAMMA_CONTROL result = default ;
			dxgiOutput.GetGammaControl( &result ) ;
			pGammaData = new( result ) ;
		}
	}

	public void SetDisplaySurface<T>( T pScanoutSurface ) where T: class, ISurface {
		ArgumentNullException.ThrowIfNull( pScanoutSurface, nameof(pScanoutSurface) ) ;
		dxgiOutput.SetDisplaySurface( pScanoutSurface.COMObject ) ;
	}

	public void GetDisplaySurfaceData( ISurface pDestination ) {
		unsafe {
			#warning Is this a CsWin32 bug? Seems like IDXGISurface::GetDisplaySurfaceData should take a pointer ...
			dxgiOutput.GetDisplaySurfaceData( pDestination.COMObject ) ;
		}
	}
	
	public void GetFrameStatistics( out FrameStatistics pStats ) {
		pStats = default ;
		unsafe {
			pStats = default ;
			DXGI_FRAME_STATISTICS result = default ;
			var pResult = &result ;
			dxgiOutput.GetFrameStatistics( pResult ) ;
			pStats = *( (FrameStatistics *)pResult ) ;
		}
	}
	
} ;