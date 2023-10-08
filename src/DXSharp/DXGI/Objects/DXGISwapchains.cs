using Windows.Win32.Graphics.Dxgi ;
using DXGI ;
using DXSharp.Windows.COM ;

namespace DXSharp.DXGI ;

public interface ISwapChain: IObject,
							 IDXGIObjWrapper< IDXGISwapChain > {
	 
	void Present( uint syncInterval, PresentFlags flags ) ;
	 
	void GetBuffer( uint buffer, ref Surface pSurface ) ;
	
	void SetFullscreenState( bool fullscreen, IOutput pTarget ) ;
	
	void GetFullscreenState( out bool pFullscreen, out IOutput ppTarget ) ;
	
	void GetDesc( out SwapChainDescription pDesc ) ;
		
	void ResizeBuffers( uint bufferCount, uint width, uint height,
						Format newFormat, SwapChainFlags swapChainFlags ) ;
	
	void ResizeTarget( in ModeDescription newTargetParameters ) ;
	
	TOutput GetContainingOutput< TOutput >( ) where TOutput: IOutput ;
	void GetFrameStatistics( out FrameStatistics pStats ) ;
	uint GetLastPresentCount( ) ;
} ;

public class SwapChain: Object, ISwapChain {
	public IDXGISwapChain? COMObject { get ; init ; }
	public new ComPtr< IDXGISwapChain >? ComPointer { get ; init ; }
}