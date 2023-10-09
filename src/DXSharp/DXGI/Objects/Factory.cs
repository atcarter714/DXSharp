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
	
	public static IFactory< IDXGIFactory > Create( ) {
		var dxgiFactory = DXGIFunctions.CreateDXGIFactory< IDXGIFactory >( ) 
			?? throw new DirectXComError( "Failed to create DXGI Factory!" ) ;
		return new Factory( dxgiFactory ) ;
	}

	public new ComPtr< IDXGIFactory >? ComPointer { get ; protected set ; }
	public IDXGIFactory? COMObject => ComPointer?.Interface as IDXGIFactory ;
	
	
	internal Factory( ) { }
	internal Factory( nint address ): base( address ) { }
	internal Factory( IDXGIFactory factory ): base( factory ) { }
	internal Factory( object obj ): base(COMUtility.GetIUnknownForObject(obj)) { }

	
	public HResult CreateSwapChain< TDevice, TSwapChain >( in TDevice pDevice,
															   in SwapChainDescription desc,
																out TSwapChain? ppSwapChain )
											where TDevice: class, IUnknownWrapper< TDevice, IUnknown >
											where TSwapChain: SwapChain, ISwapChain, new() {
		_throwIfDestroyed( ) ;
		ppSwapChain = default ;
			
		unsafe {
			var descCopy = desc ;
			var _hr = COMObject!.CreateSwapChain( pDevice.ComObject,
												(DXGI_SWAP_CHAIN_DESC*)&descCopy, 
													out IDXGISwapChain? pSwapChain ) ;
			
			if( pSwapChain is null || _hr.Failed ) return _hr ;
			
			ppSwapChain = new TSwapChain( ) ;
			ppSwapChain.SetComPointer( new ComPtr<IDXGISwapChain>(pSwapChain) ) ;
		}
		return HResult.S_OK ;
	}
	
	public void CreateSoftwareAdapter< TAdapter >( HInstance Module, out TAdapter? ppAdapter ) 
												where TAdapter: class, IAdapter, new( ) {
		_ = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			COMObject
				.CreateSoftwareAdapter( Module,
										out IDXGIAdapter? pAdapter ) ; 
			ppAdapter = default ; 
			var adapter = new TAdapter( ) ;
			adapter.SetComPointer( new ComPtr<IDXGIAdapter>(pAdapter) ) ;
			ppAdapter = adapter ;
		}
	}

	public void MakeWindowAssociation( in HWnd WindowHandle, WindowAssociation Flags ) =>
		( _ = COMObject ?? throw new NullReferenceException() )
			.MakeWindowAssociation( WindowHandle, (uint)Flags ) ;

	public HWnd GetWindowAssociation( ) {
		GetWindowAssociation( out var h ) ;
		return h ;
	}
	public void GetWindowAssociation( out HWnd pWindowHandle ) {
		_ = COMObject ?? throw new NullReferenceException( ) ;
		unsafe {
			winMD.HWND handle = default ;
			COMObject.GetWindowAssociation( &handle ) ;
			pWindowHandle = new( handle ) ;
		}
	}

	
	public HResult EnumAdapters< TAdapter >( uint index, out TAdapter? ppAdapter ) 
												where TAdapter: class, IAdapter, new() {
		_ = COMObject ?? throw new NullReferenceException( ) ;
		ppAdapter = default ;
		unsafe {
			COMObject.EnumAdapters( index, out IDXGIAdapter? pAdapter ) ; 
			var adapter = new TAdapter( ) ;
			adapter.SetComPointer( new(pAdapter) ) ;
			ppAdapter = adapter ;
		}
		return HResult.S_OK ;
	}

	
	internal static TFactory New< TFactory >( )
		where TFactory: class, IFactory< IDXGIFactory >, new( ) {
		return null ;
	}
	
} ;

public class Factory1: Factory, IFactory1 {
	public new IDXGIFactory1? COMObject { get ; }
	public Factory1( IDXGIFactory1 dxgiObj ): base(dxgiObj) { }

	public HResult EnumAdapters1< TAdapter >( uint index, out TAdapter? ppAdapter )
		where TAdapter: class, IAdapter1, new( ) {
		if( COMObject is null ) throw new NullReferenceException( ) ;
		unsafe {
			COMObject.EnumAdapters1( index, out IDXGIAdapter1? pAdapter ) ; 
			ppAdapter = default ;
			ppAdapter = new TAdapter( ) ;
			ppAdapter.SetComPointer( new(pAdapter) ) ;
		}
		return HResult.S_OK ;
	}
} ;
