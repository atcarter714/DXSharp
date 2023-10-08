using DXSharp.Windows.COM ;

namespace DXSharp.DXGI ;


//https://learn.microsoft.com/en-us/windows/desktop/api/DXGI/nn-dxgi-idxgioutput
public interface IOutput: IObject {
	void GetDescription( out OutputDescription pDescription ) ;
	
	void GetDisplayModeList( Format enumFormat,
							 uint flags,
							 out uint pNumModes,
							 out Span< ModeDescription > pDescription ) ;

	void FindClosestMatchingMode( in  ModeDescription pModeToMatch, 
								  out ModeDescription pClosestMatch,
								  IUnknownWrapper     pConcernedDevice ) ;

	void WaitForVBlank( ) ;
	
	void TakeOwnership( IUnknownWrapper pDevice, bool exclusive ) ;
	void ReleaseOwnership( ) ;
	
	void GetGammaControlCapabilities( out GammaControlCapabilities pGammaCaps ) ;
	void SetGammaControl( in              GammaControl             pGammaData ) ;
	void GetGammaControl( out             GammaControl             pGammaData ) ;
	
	void SetDisplaySurface<T>( T pScanoutSurface ) where T : class, ISurface ;
	void GetDisplaySurfaceData( ISurface pDestination ) ;
	
	void GetFrameStatistics( out FrameStatistics pStats ) ;
} ;