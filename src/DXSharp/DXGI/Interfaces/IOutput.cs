using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;

namespace DXSharp.DXGI ;

// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput
// ------------------------------------------------------------------------------------------------
// https://learn.microsoft.com/en-us/windows/desktop/api/DXGI/nn-dxgi-idxgioutput


public interface IOutput: IObject, DXGIWrapper<IDXGIOutput> {
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

// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput1
// ------------------------------------------------------------------------------------------------
// https://docs.microsoft.com/en-us/windows/win32/api/dxgi1_2/nn-dxgi1_2-idxgioutput1


public interface IOutput1: IOutput, DXGIWrapper< IDXGIOutput1 > {
	/*static ConstructWrapper< IObject, IDXGIObject >? 
		IObjectConstruction.ConstructFunction => (o) => new Output( o ) ;*/

	void GetDisplayModeList1( Format enumFormat,
							  uint flags,
							  out uint pNumModes,
							  out Span< ModeDescription1 > pDescription ) ;
	
	void FindClosestMatchingMode1( in  ModeDescription1 pModeToMatch, 
								   out ModeDescription1 pClosestMatch,
								   IUnknownWrapper      pConcernedDevice ) ;
	
	void GetDisplaySurfaceData1( IResource pDestination ) ;
	
	void DuplicateOutput( IDevice pDevice, out IOutputDuplication? ppOutputDuplication ) ;
} ;