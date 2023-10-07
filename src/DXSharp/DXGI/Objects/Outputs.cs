using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;
using DXGI ;
using DXSharp.Windows.COM ;

namespace DXSharp.DXGI ;

//public interface IOuput: IUnknown< IOuput > { } ;

public class Output: Object, IOutput {
	#region Internal Constructors
	internal Output( nint ptr ): base( ptr ) =>
		this._idxgiOutput ??= Marshal.GetObjectForIUnknown( ptr ) as IDXGIOutput 
							  ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
																   $"The internal COM interface is destroyed/null." ) ;
	internal Output( in IDXGIOutput dxgiObj ):
		base( Marshal.GetIUnknownForObject(dxgiObj) ) =>
		this._idxgiOutput ??= dxgiObj ?? throw new ArgumentNullException( nameof(dxgiObj) ) ;
	#endregion
	
	protected IDXGIOutput? _idxgiOutput ;
	protected IDXGIOutput dxgiOutput => _idxgiOutput ??=
				Marshal.GetObjectForIUnknown( this.Pointer ) as IDXGIOutput 
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

	public void GetDisplayModeList( Format enumFormat, uint flags,
									out uint pNumModes, ModeDescription[ ] pDescription ) {
		pNumModes = 0 ;
		uint modeCount = 0 ;
		pDescription = Array.Empty< ModeDescription >( ) ; //! (zero allocation/garbage)
		unsafe {
			// First, call the function just to get the count (no pointer for results):
			dxgiOutput.GetDisplayModeList( (DXGI_FORMAT)enumFormat,
										   flags, ref modeCount ) ;
			
			// Now, allocate the memory and call the function again:
			if ( pNumModes is 0 ) return ;
			var _alloc = stackalloc DXGI_MODE_DESC[ (int)pNumModes ] ;
			
			// This time, we have a pointer telling it where to write the results:
			dxgiOutput.GetDisplayModeList( (DXGI_FORMAT)enumFormat,
										   flags, ref pNumModes,
											_alloc ) ; // (ptr to stack allocation)
			
			// Finally, copy the results into the managed array:
			pDescription = new ModeDescription[ pNumModes ] ;
			for ( int i = 0 ; i < pNumModes && i < pDescription.Length ; ++i )
				pDescription[ i ] = new( _alloc[ i ] ) ;
		}
		//! Stack allocation 
	}

	public void FindClosestMatchingMode( in ModeDescription pModeToMatch, 
										 out ModeDescription pClosestMatch,
										 IUnknown pConcernedDevice ) {
		pClosestMatch = default ;
		unsafe {
			DXGI_MODE_DESC result = default, modeToMatch_ = pModeToMatch ;
			dxgiOutput.FindClosestMatchingMode( &modeToMatch_, &result,
											   pConcernedDevice ) ;
			pClosestMatch = new( result ) ;
		}
	}

	public void WaitForVBlank( ) => dxgiOutput.WaitForVBlank( ) ;

	public void TakeOwnership( IUnknown pDevice, bool exclusive ) {
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

	public void SetDisplaySurface< T >( T pScanoutSurface ) where T: Surface {
		if ( pScanoutSurface is null ) throw new 
			ArgumentNullException( nameof(pScanoutSurface) ) ;
		
		dxgiOutput.SetDisplaySurface( pScanoutSurface.ComPtr.Interface.Pointer ) ;
	}
	public void GetDisplaySurfaceData( ISurface pDestination ) { }

	public void GetFrameStatistics( out FrameStatistics pStats ) { }
}