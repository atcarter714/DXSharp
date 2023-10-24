#region Using Directives
using System.Numerics ;
using System.Runtime.Versioning ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;

using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.DXGI ;


public class SwapChain: DeviceSubObject, ISwapChain {
	public enum ColorSpaceSupportFlags: uint { Present = 0x1, OverlayPresent = 0x2, } ;
	
	public new static Type ComType => typeof( IDXGISwapChain ) ;
	public new static Guid InterfaceGUID => ComType.GUID ;
	public static SwapChain? Instantiate( ) => new( ) ;
	
	
	public new ComPtr< IDXGISwapChain >? ComPointer { get ; protected set; }
	public new IDXGISwapChain? COMObject => ComPointer?.Interface ;
	
	// -----------------------------------------------------------------
	// Constructors:
	// -----------------------------------------------------------------
	
	internal SwapChain( ) { }
	internal SwapChain( nint comObject ) => 
		ComPointer = new( comObject ) ;
	internal SwapChain( in IDXGISwapChain comObject ) => 
		ComPointer = new( comObject ) ;
	internal SwapChain( ComPtr<IDXGISwapChain> otherPtr ) => 
		this.ComPointer = otherPtr ;
	
	// -----------------------------------------------------------------

	public void Present( uint syncInterval, PresentFlags flags ) => 
						COMObject!.Present( syncInterval, (uint)flags ) ;

	//public void GetBuffer( uint buffer, out Direct3D12.IResource pSurface ) {
	public void GetBuffer< TResource >( uint buffer, out TResource pSurface ) where TResource: IDXCOMObject, IInstantiable {
		unsafe {
			Guid guid = TResource.InterfaceGUID ;
			COMObject!.GetBuffer( buffer, &guid, out var comObj ) ;
			
#if DEBUG || DEBUG_COM || DEV_BUILD
			if( comObj is null ) throw new DirectXComError( $"{nameof(SwapChain)}.{nameof(GetBuffer)} :: " +
															$"Failed to obtain buffer resource of type \"{typeof(TResource).Name}\" " +
															$"(GUID: {typeof(TResource).GUID})" ) ;
#endif
			var surface = (TResource)TResource.Instantiate( (IUnknown)comObj ) ;
			pSurface = surface ;
		}
	}

	public void SetFullscreenState( bool fullscreen, in IOutput? pTarget ) => 
		COMObject!.SetFullscreenState( fullscreen, pTarget?.COMObject ) ;

	public void GetFullscreenState( out bool pFullscreen, out IOutput? ppTarget ) {
		ppTarget = null ;
		unsafe {
			BOOL fullscreen = false ;
			COMObject!.GetFullscreenState( &fullscreen, out var output ) ;
			ppTarget = (IOutput)new Output( output ) ;
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

	public IOutput? GetContainingOutput( ) {
		COMObject!.GetContainingOutput( out var output ) ;
		if( output is null ) return null ;
		return (IOutput) new Output(output) ;
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
	public new static Type ComType => typeof( IDXGISwapChain1 ) ;
	public new static Guid InterfaceGUID => ComType.GUID ;
	public new static SwapChain1? Instantiate( ) => new( ) ;
	public static ISwapChain? Instantiate( nint address ) => new SwapChain1( address ) ;
	public static ISwapChain? Instantiate< ICom >( ICom obj ) where ICom: IUnknown? => 
		new SwapChain1( (IDXGISwapChain1)obj! ) ;

	public new IDXGISwapChain1? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGISwapChain1 >? ComPointer { get ; protected set ; }
	
	internal SwapChain1( ) { }
	internal SwapChain1( nint ptr ) => ComPointer = new( ptr ) ;
	internal SwapChain1( in IDXGISwapChain1? comObject ) => ComPointer = new( comObject! ) ;
	internal SwapChain1( ComPtr< IDXGISwapChain1 > otherPtr ) => ComPointer = otherPtr ;


	public SwapChainDescription1 GetDesc1( ) {
		GetDesc1( out var desc ) ;
		return desc ;
	}
	public void GetDesc1( out SwapChainDescription1 pDesc ) {
		
		unsafe {
			DXGI_SWAP_CHAIN_DESC1 result = default ;
			COMObject!.GetDesc1( &result ) ;
			pDesc = result ;
		}
	}
	
	public void GetFullscreenDesc( out SwapChainFullscreenDescription pDesc ) {
		
		unsafe {
			DXGI_SWAP_CHAIN_FULLSCREEN_DESC result = default ;
			COMObject!.GetFullscreenDesc( &result ) ;
			pDesc = new( result ) ;
		}
	}

	public void GetHwnd( out HWND pHwnd ) {
		
		unsafe {
			HWND result = default ;
			COMObject!.GetHwnd( &result ) ;
			pHwnd = result ;
		}
	}

	public void GetCoreWindow( Guid riid, out IUnknown? ppUnk ) {
		
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
		
		unsafe {
			DXGI_PRESENT_PARAMETERS desc = pPresentParameters ;
			COMObject!.Present1( syncInterval, (uint)flags, &desc ) ;
		}
	}

	public bool IsTemporaryMonoSupported( ) => COMObject!.IsTemporaryMonoSupported( ) ;
	
	public void GetRestrictToOutput( out IOutput ppRestrictToOutput ) {
		
		unsafe {
			COMObject!.GetRestrictToOutput( out var output ) ;
			ppRestrictToOutput = new Output( output ) ;
		}
	}

	public void SetBackgroundColor( in RGBA pColor ) {
		
		unsafe {
			fixed( RGBA* p = &pColor ) {
				COMObject!.SetBackgroundColor( (DXGI_RGBA *)p ) ;
			}
		}
	}

	public void GetBackgroundColor( out RGBA pColor ) {
		
		pColor = default ;
		unsafe {
			DXGI_RGBA result = default ;
			COMObject!.GetBackgroundColor( &result ) ;
			pColor = result ;
		}
	}

	public void SetRotation( ModeRotation rotation ) {
		
		COMObject!.SetRotation( (DXGI_MODE_ROTATION)rotation ) ;
	}

	public void GetRotation( out ModeRotation pRotation ) {
		
		unsafe {
			DXGI_MODE_ROTATION result = default ;
			COMObject!.GetRotation( &result ) ;
			pRotation = (ModeRotation)result ;
		}
	}

} ;



public class SwapChain2: SwapChain1, ISwapChain2 {
	public new static Type ComType => typeof( IDXGISwapChain2 ) ;
	public new static Guid InterfaceGUID => ComType.GUID ;
	public new static SwapChain2? Instantiate( ) => new( ) ;
	public new static ISwapChain? Instantiate( nint address ) => new SwapChain2( address ) ;
	public new static ISwapChain? Instantiate< ICom >( ICom obj ) where ICom: IUnknown? => 
		new SwapChain2( (IDXGISwapChain2)obj! ) ;

	public new IDXGISwapChain2? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGISwapChain2 >? ComPointer { get ; protected set ; }
	
	internal SwapChain2( ) { }
	internal SwapChain2( nint ptr ) => ComPointer = new( ptr ) ;
	internal SwapChain2( in IDXGISwapChain2? comObject ) => ComPointer = new( comObject! ) ;
	internal SwapChain2( ComPtr< IDXGISwapChain2 > otherPtr ) => ComPointer = otherPtr ;

	
	
	public void SetSourceSize( uint width, uint height ) {
		
		COMObject!.SetSourceSize( width, height ) ;
	}

	public void GetSourceSize( out uint pWidth, out uint pHeight ) => 
		COMObject!.GetSourceSize( out pWidth, out pHeight ) ;

	public void SetMaximumFrameLatency( uint maxLatency ) {
		
		COMObject!.SetMaximumFrameLatency( maxLatency ) ;
	}

	public void GetMaximumFrameLatency( out uint pMaxLatency ) {
		
		COMObject!.GetMaximumFrameLatency( out pMaxLatency ) ;
	}

	public Win32Handle GetFrameLatencyWaitableObject( ) => COMObject!.GetFrameLatencyWaitableObject( ) ;

	public void SetMatrixTransform( in Matrix3x2 pMatrix ) {
		
		unsafe {
			fixed ( Matrix3x2* p = &pMatrix ) {
				COMObject!.SetMatrixTransform( (DXGI_MATRIX_3X2_F*)p ) ;
			}
		}
	}
}



[SupportedOSPlatform("windows10.0.10240")]
public class SwapChain3: SwapChain2, ISwapChain3 {
	public new static Guid InterfaceGUID => ComType.GUID ;
	public new static Type ComType => typeof( IDXGISwapChain3 ) ;
	public new static SwapChain3? Instantiate( ) => new( ) ;
	public new static ISwapChain? Instantiate( nint address ) => new SwapChain3( address ) ;
	public new static ISwapChain? Instantiate< ICom >( ICom obj ) where ICom: IUnknown? =>
		new SwapChain3( (IDXGISwapChain3)obj! ) ;

	
	public new IDXGISwapChain3? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGISwapChain3 >? ComPointer { get ; protected set ; }

	
	internal SwapChain3( ) { }
	internal SwapChain3( nint ptr ) => ComPointer = new( ptr ) ;
	internal SwapChain3( in IDXGISwapChain3? comObject ) => ComPointer = new( comObject! ) ;
	internal SwapChain3( ComPtr< IDXGISwapChain3 > otherPtr ) => ComPointer = otherPtr ;



	[SupportedOSPlatform("windows10.0.10240")]
	public uint GetCurrentBackBufferIndex( ) => COMObject!.GetCurrentBackBufferIndex( ) ;

	
	[SupportedOSPlatform("windows10.0.10240")]
	public void CheckColorSpaceSupport( ColorSpaceType colorSpace, out ColorSpaceSupportFlags pColorSpaceSupport ) {
		unsafe {
			COMObject!.CheckColorSpaceSupport( (DXGI_COLOR_SPACE_TYPE)colorSpace, out var support ) ;
			pColorSpaceSupport = (ColorSpaceSupportFlags)support ;
		}
	}
	
	
	[SupportedOSPlatform("windows10.0.10240")]
	public void SetColorSpace1( ColorSpaceType colorSpace ) {
		
		COMObject!.SetColorSpace1( (DXGI_COLOR_SPACE_TYPE)colorSpace ) ;
	}

	
	[SupportedOSPlatform("windows10.0.10240")]
	public void ResizeBuffers1( uint bufferCount, 
								uint width, uint height, 
								Format newFormat,
								SwapChainFlags swapChainFlags,
								in uint[ ] pCreationNodeMask,
								in IUnknown[ ] ppPresentQueue ) {
		
		unsafe { fixed ( uint* pMask = pCreationNodeMask ) {
				COMObject!.ResizeBuffers1( bufferCount, width, height, (DXGI_FORMAT)newFormat,
											   (uint)swapChainFlags, pCreationNodeMask, ppPresentQueue ) ;
			}
		}
	}
	
}






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