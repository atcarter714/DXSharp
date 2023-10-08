#region Using Directives

using System.Numerics ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion

namespace DXSharp.DXGI ;

public interface ISwapChain1: ISwapChain,
							  IDXGIObjWrapper< IDXGISwapChain1 > {
	
	void GetDesc1( out SwapChainDescription1 pDesc ) ;
	void GetFullscreenDesc( out SwapChainFullscreenDescription pDesc ) ;
	
	void GetHwnd( out HWND pHwnd ) ;
	void GetCoreWindow( Guid riid, out IUnknown ppUnk ) ;
	
	void Present1( uint syncInterval, PresentFlags flags,
				   in PresentParameters pPresentParameters ) ;
	
	bool IsTemporaryMonoSupported( ) ;
	
	void GetRestrictToOutput( out IOutput ppRestrictToOutput ) ;
	
	void SetBackgroundColor( in RGBA pColor ) ;
	void GetBackgroundColor( out RGBA pColor ) ;
	
	void SetRotation( ModeRotation rotation ) ;
	void GetRotation( out ModeRotation pRotation ) ;
	
	void SetSourceSize( uint width, uint height ) ;
	void GetSourceSize( out uint pWidth, out uint pHeight ) ;
	
	void SetMaximumFrameLatency( uint maxLatency ) ;
	void GetMaximumFrameLatency( out uint pMaxLatency ) ;
	
	void GetFrameLatencyWaitableObject( out HANDLE pHandle ) ;
	
	void SetMatrixTransform( in Matrix3x2 pMatrix ) ;
	void GetMatrixTransform( out Matrix3x2 pMatrix ) ;
	
	uint GetCurrentBackBufferIndex( ) ;
	
	void CheckColorSpaceSupport( ColorSpaceType colorSpace,
								 out SwapChain.ColorSpaceSupportFlags pColorSpaceSupport ) ;
	
	void SetColorSpace1( ColorSpaceType colorSpace ) ;
	
	void ResizeBuffers1( uint bufferCount, uint width, uint height,
						 Format newFormat, SwapChainFlags swapChainFlags,
						 in uint[] pCreationNodeMask,
						 in IUnknown[] ppPresentQueue ) ;
	
} ;

public interface ISwapChain: IObject,
							 IDXGIObjWrapper< IDXGISwapChain > {
	 
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


public class SwapChain: DeviceSubObject, ISwapChain {
	public enum ColorSpaceSupportFlags: uint { Present = 0x1, OverlayPresent = 0x2, } ;
	
	public new IDXGISwapChain? COMObject { get ; init ; }
	public new ComPtr< IDXGISwapChain >? ComPointer { get ; init ; }
	
	public SwapChain( nint comObject ): base( comObject ) {
		/*ComPointer = new ComPtr< IDXGISwapChain >( comObject ) ;
		COMObject = ComPointer.Interface ;
		PresentFlags f ;*/
	}
	public SwapChain( in IDXGISwapChain? comObject ): base( comObject! ) {
		ArgumentNullException.ThrowIfNull( comObject, nameof(comObject) ) ;
		this.COMObject  = comObject ;
		this.ComPointer = new( comObject ) ;
	}
	public SwapChain( ComPtr< IDXGISwapChain > otherPtr ) {
		this.ComPointer = otherPtr ;
		this.COMObject  = otherPtr.Interface ;
		uint r = this.AddRef( ) ;
	}
	
	public void Present( uint syncInterval, PresentFlags flags ) => 
						COMObject!.Present( syncInterval, (uint)flags ) ;

	public void GetBuffer< TBuffer >( uint buffer, out TBuffer? pSurface )
													where TBuffer:  class, ISurface,
																	IUnknownWrapper< TBuffer, IDXGISurface >, 
																	new( ) {
		pSurface = null ;
		unsafe {
			Guid guid = _getBufferIID< TBuffer >( ) ;
			COMObject!.GetBuffer( buffer, &guid, out var comObj ) ;
			pSurface = (TBuffer.CreateInstanceOf< TBuffer, IDXGISurface >( (IDXGISurface)comObj )) ;
		}
	}
	
	static Guid _getBufferIID< TBuffer >( )
		where TBuffer:	class, IUnknownWrapper< TBuffer, IDXGISurface >, new( ) => TBuffer.InterfaceGUID ;
	
	public void SetFullscreenState< TOutput >( bool fullscreen, in TOutput? pTarget )
														where TOutput: class, IOutput {
		ArgumentNullException.ThrowIfNull( pTarget, nameof(pTarget) ) ;
		COMObject!.SetFullscreenState( fullscreen, (IDXGIOutput)pTarget.ComObject! ) ;
	}
	
	public void GetFullscreenState< TOutput >( out bool pFullscreen, out TOutput? ppTarget ) 
																where TOutput: class, IOutput {
		ppTarget = null ;
		unsafe {
			BOOL fullscreen = false ;
			COMObject!.GetFullscreenState( &fullscreen, out var output ) ;
			ppTarget = (TOutput)( (IOutput)new Output(output) ) ;
			pFullscreen = fullscreen ;
		}
	}
	
	public void GetDesc( out SwapChainDescription pDesc ) {
		unsafe {
			DXGI_SWAP_CHAIN_DESC result = default ;
			COMObject!.GetDesc( &result ) ;
			pDesc = result ;
		}
	}
	
	public void ResizeBuffers( uint bufferCount, uint width, uint height,
							   Format newFormat, SwapChainFlags swapChainFlags ) {
		COMObject!.ResizeBuffers( bufferCount, width, height,
									  (DXGI_FORMAT)newFormat,
										(uint)swapChainFlags ) ;
	}
	
	public void ResizeTarget( in ModeDescription newTargetParameters ) {
		unsafe {
			DXGI_MODE_DESC result = newTargetParameters ;
			COMObject!.ResizeTarget( &result ) ;
		}
	}
	
	public TOutput? GetContainingOutput< TOutput >( ) where TOutput: class, IOutput {
		COMObject!.GetContainingOutput( out var output ) ;
		if( output is null ) return null ;
		return (TOutput)((IOutput) new Output(output)) ;
	}
	
	public void GetFrameStatistics( out FrameStatistics pStats ) {
		unsafe {
			DXGI_FRAME_STATISTICS result = default ;
			COMObject!.GetFrameStatistics( &result ) ;
			pStats = result ;
		}
	}
	
	public uint GetLastPresentCount( ) {
		COMObject!.GetLastPresentCount( out var count ) ;
		return count ;
	}
	
}