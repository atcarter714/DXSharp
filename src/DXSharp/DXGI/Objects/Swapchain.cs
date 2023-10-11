#region Using Directives
using System.Numerics ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


public class SwapChain: DeviceSubObject, ISwapChain {
	public static ISwapChain Instantiate( IntPtr address ) {
		if( address == IntPtr.Zero ) return null ;
		return new SwapChain( address ) ;
	}
	
	public enum ColorSpaceSupportFlags: uint { Present = 0x1, OverlayPresent = 0x2, } ;
	
	public new IDXGISwapChain? COMObject { get ; init ; }
	public new ComPtr< IDXGISwapChain >? ComPointer { get ; init ; }
	
	internal SwapChain( ) { }
	public SwapChain( nint comObject ): base(comObject) { }
	public SwapChain( in IDXGISwapChain? comObject ): base(comObject!) { }
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
			
			pSurface = TBuffer.ConstructInstance<TBuffer, IDXGISurface>( (IDXGISurface)comObj )
							as TBuffer ;
		}
	}
	
	static Guid _getBufferIID< TBuffer >( )
		where TBuffer:	class, IUnknownWrapper< TBuffer, IDXGISurface >, new( ) => TBuffer.InterfaceGUID ;
	
	public void SetFullscreenState< TOutput >( bool fullscreen, in TOutput? pTarget )
														where TOutput: class, IOutput {
		ArgumentNullException.ThrowIfNull( pTarget, nameof(pTarget) ) ;
		COMObject!.SetFullscreenState( fullscreen, pTarget.COMObject ) ;
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
	
} ;



public class SwapChain1: SwapChain, ISwapChain1 {
	public static ISwapChain Instantiate( IntPtr address ) {
		if( address == IntPtr.Zero ) return null ;
		return new SwapChain1( address ) ;
	}
	
	internal SwapChain1( ) { }
	public SwapChain1( nint ptr ): base( ptr ) { 
		ComPointer = new( ptr ) ;
	}
	public SwapChain1( in IDXGISwapChain1? comObject ): base( comObject! ) {
		ComPointer = new( comObject! ) ;
	}
	public SwapChain1( ComPtr< IDXGISwapChain1 > otherPtr ): 
		this(otherPtr.InterfaceVPtr) => ComPointer = otherPtr ;

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
			COMObject!.GetRestrictToOutput( out var output ) ;
			ppRestrictToOutput = new Output( output ) ;
		}
	}

	public void SetBackgroundColor( in RGBA pColor ) {
		_throwIfDestroyed( ) ;
		unsafe {
			fixed( RGBA* p = &pColor ) {
				COMObject!.SetBackgroundColor( (DXGI_RGBA *)p ) ;
			}
		}
	}

	public void GetBackgroundColor( out RGBA pColor ) {
		_throwIfDestroyed( ) ;
		pColor = default ;
		unsafe {
			DXGI_RGBA result = default ;
			COMObject!.GetBackgroundColor( &result ) ;
			pColor = result ;
		}
	}

	public void SetRotation( ModeRotation rotation ) {
		_throwIfDestroyed( ) ;
		COMObject!.SetRotation( (DXGI_MODE_ROTATION)rotation ) ;
	}

	public void GetRotation( out ModeRotation pRotation ) {
		_throwIfDestroyed( ) ;
		unsafe {
			DXGI_MODE_ROTATION result = default ;
			COMObject!.GetRotation( &result ) ;
			pRotation = (ModeRotation)result ;
		}
	}
	
} ;



//! TODO: Find out what the deal with these is. Where the hell did these come from? lol
/*public void SetSourceSize( uint width, uint height ) { }
public void GetSourceSize( out uint pWidth, out uint pHeight ) { }
public void SetMaximumFrameLatency( uint maxLatency ) { }
public void GetMaximumFrameLatency( out uint pMaxLatency ) { }
public void GetFrameLatencyWaitableObject( out HANDLE pHandle ) { }
public void SetMatrixTransform( in Matrix3x2 pMatrix ) { }
public void GetMatrixTransform( out Matrix3x2 pMatrix ) { }
public uint GetCurrentBackBufferIndex() { }
public void CheckColorSpaceSupport( ColorSpaceType colorSpace, out ColorSpaceSupportFlags pColorSpaceSupport ) { }
public void SetColorSpace1( ColorSpaceType colorSpace ) { }
public void ResizeBuffers1( uint bufferCount, uint width, uint height, Format newFormat, SwapChainFlags swapChainFlags,
							in uint[] pCreationNodeMask, in IUnknown[] ppPresentQueue ) { }
							*/