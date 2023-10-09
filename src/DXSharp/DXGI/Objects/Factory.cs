#region Using Directives
using System ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Dxgi ;
using winMD = global::Windows.Win32.Foundation ;
using static Windows.Win32.PInvoke ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
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
	internal Factory( ) { }
	internal Factory( nint address ): base( address ) { }
	internal Factory( IDXGIFactory factory ): base( factory ) { }
	
	protected IDXGIFactory? _dxgiFactory ;
	protected IDXGIFactory? _dxgiFactoryFetch => _dxgiFactory ??=
		Marshal.GetObjectForIUnknown( this.BasePointer ) as IDXGIFactory ;
	public IDXGIFactory? COMObject => _dxgiFactoryFetch ;
	
	
	public HResult CreateSwapChain< TDevice, TSwapChain >( in TDevice pDevice,
															   in SwapChainDescription desc,
																out TSwapChain? ppSwapChain )
											where TDevice: class, IUnknownWrapper< TDevice, IUnknown >
											where TSwapChain: SwapChain, ISwapChain, new() {
		_throwIfDestroyed( ) ;
		ppSwapChain = default ;
			
		unsafe {
			var descCopy = desc ;
			var _hr = _dxgiFactoryFetch!.CreateSwapChain( pDevice.ComObject,
												(DXGI_SWAP_CHAIN_DESC*)&descCopy, 
													out IDXGISwapChain? pSwapChain ) ;
			
			if( pSwapChain is null || _hr.Failed ) return _hr ;
			
			ppSwapChain = new TSwapChain( ) ;
			ppSwapChain.SetComPointer( new(pSwapChain) ) ;
		}
		return HResult.S_OK ;
	}
	
	public void CreateSoftwareAdapter< TAdapter >( HInstance Module, out TAdapter? ppAdapter ) 
												where TAdapter: class, IAdapter, new( ) {
		_ = _dxgiFactoryFetch ?? throw new NullReferenceException( ) ;
		unsafe {
			_dxgiFactoryFetch
				.CreateSoftwareAdapter( Module, 
										out IDXGIAdapter? pAdapter ) ; 
			ppAdapter = default ; 
			ppAdapter = new TAdapter( ) ;
			ppAdapter.SetComPointer( new(pAdapter) ) ;
		}
	}

	public void MakeWindowAssociation( in HWnd WindowHandle, WindowAssociation Flags ) {
		_ = _dxgiFactoryFetch ?? throw new NullReferenceException( ) ;
		_dxgiFactoryFetch.MakeWindowAssociation( WindowHandle, (uint)Flags ) ;
	}

	public void GetWindowAssociation( in HWnd pWindowHandle ) {
		_ = _dxgiFactoryFetch ?? throw new NullReferenceException( ) ;
		unsafe {
			var handle = pWindowHandle ;
			_dxgiFactoryFetch.GetWindowAssociation( (winMD.HWND *)&handle ) ;
		}
	}

	public HResult EnumAdapters< TAdapter >( uint index, out TAdapter? ppAdapter ) 
												where TAdapter: class, IAdapter, new() {
		_ = _dxgiFactoryFetch ?? throw new NullReferenceException( ) ;
		unsafe {
			_dxgiFactoryFetch.EnumAdapters( index, out IDXGIAdapter? pAdapter ) ; 
			ppAdapter = default ;
			ppAdapter = new TAdapter( ) ;
			ppAdapter.SetComPointer( new(pAdapter) ) ;
		}
		return HResult.S_OK ;
	}

	public void MakeWindowAssociation( HWnd WindowHandle, WindowAssociation Flags ) {
		_ = _dxgiFactoryFetch ?? throw new NullReferenceException( ) ;
		_dxgiFactoryFetch.MakeWindowAssociation( WindowHandle, (uint)Flags ) ;
	}



	internal static TFactory New< TFactory >()
		where TFactory: class, IFactory< IDXGIFactory >, new( ) {
		return null ;
	}
	
} ;
