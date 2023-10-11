#region Using Directives
using System.Numerics ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


// -----------------------------------------------
// Version: IDXGISwapChain
// -----------------------------------------------

public interface ISwapChain: DXGIWrapper< IDXGISwapChain > {
	internal static abstract ISwapChain Instantiate( nint address ) ;
	
	void Present( uint syncInterval, PresentFlags flags ) ;
	
	void GetBuffer< TBuffer >( uint buffer, out TBuffer? pSurface )
		where TBuffer:  class, ISurface, IUnknownWrapper< TBuffer, IDXGISurface >, new( ) ;
	
	void SetFullscreenState< TOutput >( bool fullscreen, in TOutput? pTarget )
		where TOutput: class, IOutput ;
	
	void GetFullscreenState< TOutput >( out bool pFullscreen, out TOutput? ppTarget ) 
														where TOutput: class, IOutput ;
	
	void GetDesc( out SwapChainDescription pDesc ) ;
		
	void ResizeBuffers( uint bufferCount, uint width, uint height,
						Format newFormat, SwapChainFlags swapChainFlags ) ;
	
	void ResizeTarget( in ModeDescription newTargetParameters ) ;
	
	TOutput? GetContainingOutput< TOutput >( ) where TOutput: class, IOutput ;
	void GetFrameStatistics( out FrameStatistics pStats ) ;
	uint GetLastPresentCount( ) ;
} ;

// -----------------------------------------------
// Version: IDXGISwapChain1
// -----------------------------------------------

public interface ISwapChain1: ISwapChain,
							  DXGIWrapper< IDXGISwapChain1 > {
	new IDXGISwapChain1? COMObject { get ; }
	new ComPtr< IDXGISwapChain1 > ComPointer { get ; }
	
	void GetDesc1( out SwapChainDescription1 pDesc ) ;
	void GetFullscreenDesc( out SwapChainFullscreenDescription pDesc ) ;
	
	void GetHwnd( out HWND pHwnd ) ;
	void GetCoreWindow( Guid riid, out IUnknown? ppUnk ) ;
	
	void Present1( uint syncInterval, PresentFlags flags,
				   in PresentParameters pPresentParameters ) ;
	
	bool IsTemporaryMonoSupported( ) ;
	
	void GetRestrictToOutput( out IOutput ppRestrictToOutput ) ;
	
	void SetBackgroundColor( in  RGBA pColor ) ;
	void GetBackgroundColor( out RGBA pColor ) ;
	
	void SetRotation( ModeRotation rotation ) ;
	void GetRotation( out ModeRotation pRotation ) ;
} ;

// -----------------------------------------------
// Version: IDXGISwapChain2
// -----------------------------------------------

// ...



	
/*
void SetSourceSize( uint width,  uint height ) ;
void GetSourceSize( out uint pWidth, out uint pHeight ) ;
void SetMaximumFrameLatency( uint maxLatency ) ;
void GetMaximumFrameLatency( out uint pMaxLatency ) ;
void GetFrameLatencyWaitableObject( out HANDLE pHandle ) ;
void SetMatrixTransform( in  Matrix3x2 pMatrix ) ;
void GetMatrixTransform( out Matrix3x2 pMatrix ) ;
uint GetCurrentBackBufferIndex( ) ;
void CheckColorSpaceSupport( ColorSpaceType colorSpace,
							 out SwapChain.ColorSpaceSupportFlags pColorSpaceSupport ) ;
void SetColorSpace1( ColorSpaceType colorSpace ) ;
void ResizeBuffers1( uint bufferCount, uint width, uint height,
					 Format newFormat, SwapChainFlags swapChainFlags,
					 in uint[ ] pCreationNodeMask,
					 in IUnknown[ ] ppPresentQueue ) ;
					 */