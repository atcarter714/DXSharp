#region Using Directives
using System ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using winMD = Windows.Win32.Foundation ;
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
	/// <summary>No flags</summary>
	None            = 0x0,
	/// <summary>Ignore all</summary>
	NoWindowChanges = 0x1,
	/// <summary>Ignore Alt+Enter</summary>
	NoAltEnter      = 0x2,
	/// <summary>Ignore Print Screen key</summary>
	NoPrintScreen   = 0x4,
	/// <summary>Valid? (Needs documentation)</summary>
	Valid           = 0x7,
} ;



public class Factory: Object, IFactory {
	// -----------------------------------------------------------------------------------
	public static IFactory Create< TFactory >( ) where TFactory: class, IFactory {
		var dxgiFactory = DXGIFunctions.CreateDXGIFactory< IDXGIFactory >( ) 
			?? throw new DirectXComError( "Failed to create DXGI Factory!" ) ;
		return (new Factory( dxgiFactory )) ;
	}
	// -----------------------------------------------------------------------------------
	
	public ComPtr? ComPtrBase => ComPointer ;
	public IDXGIFactory? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIFactory >? ComPointer { get ; protected set ; }
	
	
	internal Factory( ) { }
	internal Factory( nint address ): base( address ) => 
		ComPointer = new( address ) ;
	internal Factory( IDXGIFactory factory ): base( factory ) => 
		ComPointer = new( factory ) ;
	internal Factory( object obj ): 
		base( COMUtility.GetIUnknownForObject(obj) ) { }

	
	public HResult CreateSwapChain< TDevice, TSwapChain >( in TDevice pDevice,
															   in SwapChainDescription desc,
																out TSwapChain? ppSwapChain )
											where TDevice: class, IUnknownWrapper< TDevice, IUnknown >
											where TSwapChain: class, ISwapChain {
		_throwIfDestroyed( ) ;
		ppSwapChain = default ;
		
		unsafe {
			var descCopy = desc ;
			var _hr = COMObject!.CreateSwapChain( pDevice.ComObject,
												(DXGI_SWAP_CHAIN_DESC *)&descCopy, 
													out IDXGISwapChain? pSwapChain ) ;
			
			if( pSwapChain is null || _hr.Failed ) return _hr ;
			var ptr = COMUtility.GetInterfaceAddress( pSwapChain ) ;
			ppSwapChain = (TSwapChain)(TSwapChain.Instantiate(ptr)) ;
		}
		return HResult.S_OK ;
	}
	
	public void CreateSoftwareAdapter< TAdapter >( HInstance Module, out TAdapter? ppAdapter ) 
												where TAdapter: class, IAdapter {
		_ = COMObject ?? throw new NullReferenceException( ) ;
		ppAdapter = default ;
		unsafe {
			COMObject.CreateSoftwareAdapter( Module, out IDXGIAdapter? pAdapter ) ;
			
			var adapter = TAdapter.ConstructInstance< TAdapter, IDXGIAdapter >( pAdapter ) 
							  as TAdapter ?? throw new NullReferenceException( ) ;
			//adapter.SetComPointer( new ComPtr< IDXGIAdapter >(pAdapter) ) ;
			
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

	/*public HResult EnumAdapters< TAdapter >( uint index, out TAdapter? ppAdapter ) 
												where TAdapter: class, IAdapter {
		ppAdapter = default ;
		var a = enumAdapters< Adapter >( index, out var adapter ) ;
		
		return HResult.S_OK ;
	}*/
	
	public HResult EnumAdapters< TAdapter >( uint index, out TAdapter? ppAdapter ) 
												where TAdapter: class, IAdapter {
		_ = COMObject ?? throw new NullReferenceException( ) ;
		ppAdapter = default ;
		
		unsafe {
			COMObject.EnumAdapters( index, out IDXGIAdapter? pAdapter ) ;
			
			var adapter = (TAdapter)( TAdapter.ConstructInstance<Adapter, IDXGIAdapter>(pAdapter) ) ;
			
			ppAdapter = adapter ;
		}
		
		return HResult.S_OK ;
	}
} ;

public class Factory1: Factory, IFactory1 {
	// -------------------------------------------------------------------------------------
	public new static IFactory Create< TFactory >( ) where TFactory: class, IFactory {
		var dxgiFactory = DXGIFunctions.CreateDXGIFactory1< IDXGIFactory1 >( ) 
			?? throw new DirectXComError( "Failed to create DXGI Factory!" ) ;
		return new Factory1( dxgiFactory ) ;
	}
	// -------------------------------------------------------------------------------------
	
	public new IDXGIFactory1? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIFactory1 >? ComPointer { get ; protected set ; }
	
	internal Factory1( ) { }
	public Factory1( nint ptr ): base(ptr) { }
	public Factory1( IDXGIFactory1 dxgiObj ): base(dxgiObj) { }
	
	/*public new HResult EnumAdapters< TAdapter >( uint index, out TAdapter? ppAdapter ) 
		where TAdapter: class, IAdapter {
		if( COMObject is null ) throw new NullReferenceException( ) ;
		ppAdapter = default ;
		
		COMObject.EnumAdapters1( index, out IDXGIAdapter1? pAdapter ) ; 
		var comptr = new ComPtr< IDXGIAdapter >( pAdapter ) ;
		ppAdapter = ((TAdapter)TAdapter.ConstructInstance<TAdapter, IDXGIAdapter>(pAdapter)!)
										?? throw new NullReferenceException( ) ;
		ppAdapter.SetComPointer( comptr ) ;
		
		return HResult.S_OK ;
	}*/
	
	public HResult EnumAdapters1< TAdapter >( uint index, out TAdapter? ppAdapter )
												where TAdapter: class, IAdapter1 {
		if( COMObject is null ) throw new NullReferenceException( ) ;
		ppAdapter = default ;
		
		var _hr = COMObject.EnumAdapters1( index, out IDXGIAdapter1? pAdapter ) ; 
		
		ppAdapter = ( (TAdapter.ConstructInstance< TAdapter, IDXGIAdapter1 >(pAdapter) )
										as TAdapter ?? throw new NullReferenceException() ) ;
		
		//ppAdapter.SetComPointer( comptr ) ;
		return HResult.S_OK ;
	}
} ;
