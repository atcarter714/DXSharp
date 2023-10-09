﻿#region Using Directives

using System.Numerics ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion

namespace DXSharp.DXGI ;

public class SwapChain: DeviceSubObject, ISwapChain {
	public enum ColorSpaceSupportFlags: uint { Present = 0x1, OverlayPresent = 0x2, } ;
	
	public new IDXGISwapChain? COMObject { get ; init ; }
	public new ComPtr< IDXGISwapChain >? ComPointer { get ; init ; }
	
	internal SwapChain( ) { }
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

public class SwapChain1: SwapChain, ISwapChain1 {
	internal SwapChain1( ) { }
	public SwapChain1( nint comObject ): base( comObject ) { }
	public SwapChain1( in IDXGISwapChain1? comObject ): base( comObject! ) { }
	public SwapChain1( ComPtr< IDXGISwapChain1 > otherPtr ): 
		this(otherPtr.InterfaceVPtr) { }

	public new IDXGISwapChain1? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGISwapChain1 > ComPointer { get ; protected set ; }

	public SwapChainDescription1 GetDesc1( ) {
		GetDesc1( out var desc ) ;
		return desc ;
	}
	public void GetDesc1( out SwapChainDescription1 pDesc ) {
		_throwIfDestroyed( ) ;
		unsafe {
			DXGI_SWAP_CHAIN_DESC1 result = default ;
			COMObject!.GetDesc1( &result ) ;
			pDesc = result ;
		}
	}
	
	public void GetFullscreenDesc( out SwapChainFullscreenDescription pDesc ) {
		_throwIfDestroyed( ) ;
		unsafe {
			DXGI_SWAP_CHAIN_FULLSCREEN_DESC result = default ;
			COMObject!.GetFullscreenDesc( &result ) ;
			pDesc = new( result ) ;
		}
	}

	public void GetHwnd( out HWND pHwnd ) {
		_throwIfDestroyed( ) ;
		unsafe {
			HWND result = default ;
			COMObject!.GetHwnd( &result ) ;
			pHwnd = result ;
		}
	}

	public void GetCoreWindow( Guid riid, out IUnknown? ppUnk ) {
		_throwIfDestroyed( ) ;
		unsafe {
			COMObject!.GetCoreWindow( &riid, out var unk ) ;
			ppUnk = unk as IUnknown ;
		}
	}
	public void GetCoreWindowAs< T >( out T? ppUnk ) where T: IUnknown {
		GetCoreWindow( typeof(T).GUID, out var unk ) ;
		ppUnk = (T)unk! ;
	}
	
	public void Present1( uint syncInterval, PresentFlags flags, in PresentParameters pPresentParameters ) {
		_throwIfDestroyed( ) ;
		unsafe {
			DXGI_PRESENT_PARAMETERS desc = pPresentParameters ;
			COMObject!.Present1( syncInterval, (uint)flags, &desc ) ;
		}
	}

	public bool IsTemporaryMonoSupported( ) => COMObject!.IsTemporaryMonoSupported( ) ;
	
	public void GetRestrictToOutput( out IOutput ppRestrictToOutput ) {
		_throwIfDestroyed( ) ;
		unsafe {
			COMObject!.GetRestrictToOutput( out IDXGIOutput? output ) ;
			ppRestrictToOutput = (IOutput)new Output( output ) ;
		}
		GetRestrictToOutput( out Output1 o ) ;
	}

	public void SetBackgroundColor( in RGBA pColor ) {
		_throwIfDestroyed( ) ;
	}

	public void GetBackgroundColor( out RGBA pColor ) {
		_throwIfDestroyed( ) ;
	}

	public void SetRotation( ModeRotation rotation ) {
		_throwIfDestroyed( ) ;
	}

	public void GetRotation( out ModeRotation pRotation ) {
		_throwIfDestroyed( ) ;
	}

	public void SetSourceSize( uint width, uint height ) {
		_throwIfDestroyed( ) ;
	}

	public void GetSourceSize( out uint pWidth, out uint pHeight ) {
		_throwIfDestroyed( ) ;
	}

	public void SetMaximumFrameLatency( uint maxLatency ) {
		_throwIfDestroyed( ) ;
	}

	public void GetMaximumFrameLatency( out uint pMaxLatency ) {
		_throwIfDestroyed( ) ;
	}

	public void GetFrameLatencyWaitableObject( out HANDLE pHandle ) {
		_throwIfDestroyed( ) ;
	}

	public void SetMatrixTransform( in Matrix3x2 pMatrix ) {
		_throwIfDestroyed( ) ;
	}

	public void GetMatrixTransform( out Matrix3x2 pMatrix ) {
		_throwIfDestroyed( ) ;
	}

	public uint GetCurrentBackBufferIndex() {
		_throwIfDestroyed( ) ;
	}

	public void CheckColorSpaceSupport( ColorSpaceType colorSpace, out ColorSpaceSupportFlags pColorSpaceSupport ) {
		_throwIfDestroyed( ) ;
	}

	public void SetColorSpace1( ColorSpaceType colorSpace ) {
		_throwIfDestroyed( ) ;
	}

	public void ResizeBuffers1( uint      bufferCount,       uint          width, uint height, Format newFormat, SwapChainFlags swapChainFlags,
								in uint[] pCreationNodeMask, in IUnknown[] ppPresentQueue ) {
		_throwIfDestroyed( ) ;
	}
}