#region Using Directives
using System ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using ABI.Windows.UI.WebUI ;
using DXSharp.Windows ;
using DXSharp.Windows.Win32 ;
using winMD = global::Windows.Win32.Foundation ;
#endregion

namespace DXSharp.DXGI;



/// <summary>
/// Flags for making window association between
/// a SwapChain and a HWND (Window handle)
/// </summary>
[Flags] public enum WindowAssociation: uint {
	/// <summary>
	/// No flags
	/// </summary>
	None            = 0x0,
	/// <summary>
	/// Ignore all
	/// </summary>
	NoWindowChanges = 0x1,
	/// <summary>
	/// Ignore Alt+Enter
	/// </summary>
	NoAltEnter      = 0x2,
	/// <summary>
	/// Ignore Print Screen key
	/// </summary>
	NoPrintScreen   = 0x4,
	/// <summary>
	/// Valid? (Needs documentation)
	/// </summary>
	Valid           = 0x7,
} ;



public class Factory: Object, IFactory< IDXGIFactory > {
	Factory( nint address ): base( address ) { }
	Factory( IDXGIFactory factory ): base( factory ) { }
	
	protected IDXGIFactory? _dxgiFactory ;
	protected IDXGIFactory? _dxgiFactoryFetch => _dxgiFactory ??=
		Marshal.GetObjectForIUnknown( this.BasePointer ) as IDXGIFactory ;
	public IDXGIFactory? COMObject => _dxgiFactoryFetch ;
	
	
	public HResult CreateSwapChain< T >( object pDevice,
										 in  SwapChainDescription desc,
										 out T ppSwapChain ) 
											where T: ISwapChain {
		//_ = _dxgiFactoryFetch ?? throw new NullReferenceException( ) ;
		_throwIfDestroyed( ) ;
		unsafe {
			var descCopy = desc ;
			_dxgiFactoryFetch!.CreateSwapChain( pDevice,
												(DXGI_SWAP_CHAIN_DESC*)&descCopy, 
													out IDXGISwapChain? pSwapChain ) ; 
			ppSwapChain = default ;
			ppSwapChain = new SwapChain( pSwapChain ) ;
		}
		
		unsafe {
			_dxgiFactoryFetch
				.CreateSwapChain( pDevice, desc, out IDXGISwapChain? pSwapChain ) ; 
			ppSwapChain = default ;
			ppSwapChain = new SwapChain( pSwapChain ) ;
		}
		return HResult.S_OK ;
	}

	public void CreateSoftwareAdapter<T>( HInstance Module, out T? ppAdapter ) where T: Adapter {
		_ = _dxgiFactoryFetch ?? throw new NullReferenceException( ) ;
		unsafe {
			_dxgiFactoryFetch
				.CreateSoftwareAdapter( Module, 
										out IDXGIAdapter? pAdapter ) ; 
			ppAdapter = default ; 
			ppAdapter = new Adapter( pAdapter ) ;
		}
	}

	public void GetWindowAssociation( in HWnd pWindowHandle ) { }

	public HResult EnumAdapters<T>( uint index, out T ppAdapter ) where T: Adapter {
		_ = _dxgiFactoryFetch ?? throw new NullReferenceException( ) ;
		unsafe {
			_dxgiFactoryFetch
				.EnumAdapters( index, 
								out IDXGIAdapter? pAdapter ) ; 
			ppAdapter = default ; 
			ppAdapter = new Adapter( pAdapter ) ;
		}
	}

	public void MakeWindowAssociation( HWnd WindowHandle, WindowAssociation Flags ) {
		_ = _dxgiFactoryFetch ?? throw new NullReferenceException( ) ;
		_dxgiFactoryFetch.MakeWindowAssociation( WindowHandle, (uint)Flags ) ;
	}

}
