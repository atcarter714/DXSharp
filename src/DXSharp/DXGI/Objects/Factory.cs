#region Using Directives
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using winMD = Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;

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



public class Factory: Object, 
					  IFactory {
	// -----------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIFactory ) ;
	public new static Guid InterfaceGUID => typeof( IDXGIFactory ).GUID ;
	public static IFactory Create< TFactory >( ) where TFactory: class, IFactory, IInstantiable {
		var dxgiFactory = DXGIFunctions.CreateDXGIFactory< IDXGIFactory >( ) 
			?? throw new DirectXComError( "Failed to create DXGI Factory!" ) ;
		return (new Factory( dxgiFactory )) ;
	}
	// -----------------------------------------------------------------------------------
	
	
	public override ComPtr? ComPtrBase => ComPointer ;
	public new IDXGIFactory? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIFactory >? ComPointer { get ; protected set ; }
	
	
	internal Factory( ) { }
	internal Factory( nint address ) => 
		ComPointer = new( address ) ;
	internal Factory( IDXGIFactory factory ) => 
		ComPointer = new( factory ) ;
	internal Factory( ComPtr< IDXGIFactory > ptr ) => 
		ComPointer = ptr ;
	internal Factory( object obj ) =>
		ComPointer = new( COMUtility.GetIUnknownForObject(obj) ) ;


	public virtual HResult CreateSwapChain< TCmdObj, TSwapChain >( in TCmdObj pCmdQueue,
																   in SwapChainDescription desc,
																   out TSwapChain? ppSwapChain )
															where TCmdObj: class, IUnknownWrapper< ID3D12CommandQueue >
															where TSwapChain: class, ISwapChain {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( ComPointer?.Disposed ?? true, nameof(Factory) ) ;
		ArgumentNullException.ThrowIfNull( pCmdQueue, nameof(pCmdQueue) ) ;
		if( pCmdQueue.ComPointer is null ) throw new NullReferenceException( nameof(pCmdQueue.ComPointer) ) ;
#endif
		
		unsafe {
			ppSwapChain = default ;
			var descCopy = desc ;
			var cmdQueue = pCmdQueue.ComPointer.Interface ;
			
			var _hr = COMObject!.CreateSwapChain( cmdQueue,
												(DXGI_SWAP_CHAIN_DESC *)&descCopy, 
													out IDXGISwapChain? pSwapChain ) ;
			
			if( pSwapChain is null || _hr.Failed ) return _hr ;
			ppSwapChain = (TSwapChain)(TSwapChain.Instantiate(pSwapChain)) ;
			return _hr ;
		}
	}
	
	public void CreateSoftwareAdapter< TAdapter >( HInstance Module, out TAdapter? ppAdapter ) 
												where TAdapter: class, IAdapter, IInstantiable<TAdapter> {
		_ = COMObject ?? throw new NullReferenceException( ) ;
		ppAdapter = default ;
		COMObject.CreateSoftwareAdapter( Module, out IDXGIAdapter? pAdapter ) ;
		ppAdapter = (TAdapter)TAdapter.Instantiate( pAdapter ) ;
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
												where TAdapter: class, IAdapter {
		_ = COMObject ?? throw new NullReferenceException( ) ;
		ppAdapter = default ;
		
		var hr = COMObject.EnumAdapters( index, out IDXGIAdapter? pAdapter ) ;
		if ( hr.Failed ) {
			ppAdapter = null ;
			return hr ;
		}
		
		var adapter = (TAdapter)( TAdapter.Instantiate(pAdapter) ) ;
		ppAdapter = adapter ;
		
		return hr ;
	}
} ;



public class Factory1: Factory, IFactory1 {
	// -------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIFactory1 ) ;
	public new static Guid InterfaceGUID => typeof( IDXGIFactory1 ).GUID ;
	public new static IFactory Create< TFactory >( ) where TFactory: class, IFactory, IInstantiable {
		var dxgiFactory = DXGIFunctions.CreateDXGIFactory1< IDXGIFactory1 >( ) 
			?? throw new DirectXComError( "Failed to create DXGI Factory!" ) ;
		return new Factory1( dxgiFactory ) ;
	}
	// -------------------------------------------------------------------------------------

	public bool IsInitialized => !ComPointer?.Disposed ?? false ;
	public new IDXGIFactory1? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIFactory1 >? ComPointer { get ; protected set ; }
	
	internal Factory1( ) { }
	public Factory1( nint ptr ): base(ptr) => ComPointer = new( ptr ) ;
	public Factory1( IDXGIFactory1 dxgiObj ): base(dxgiObj) => ComPointer = new( dxgiObj ) ;
	public Factory1( ComPtr< IDXGIFactory1 > ptr ): base(ptr) => ComPointer = ptr ;
	public Factory1( object obj ): base(obj) =>
		ComPointer = new( COMUtility.GetIUnknownForObject(obj) ) ;


	public HResult EnumAdapters1< TAdapter >( uint index, out TAdapter? ppAdapter )
												where TAdapter: class, IAdapter1 {
		if( COMObject is null ) throw new NullReferenceException( ) ;
		ppAdapter = default ;
		
		var _hr = COMObject.EnumAdapters1( index, out IDXGIAdapter1? pAdapter ) ;
		if( _hr.Failed ) {
			ppAdapter = null ;
			return _hr ;
		}
		
		ppAdapter = ( (TAdapter.Instantiate(pAdapter) )
										as TAdapter ?? throw new NullReferenceException() ) ;
		
		return _hr ;
	}

	
	public override HResult CreateSwapChain< TDevice, TSwapChain >( in  TDevice pCmdQueue, in SwapChainDescription desc,
																	out TSwapChain? ppSwapChain ) where TSwapChain: class {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( ComPointer?.Disposed ?? true, nameof(Factory) ) ;
		ArgumentNullException.ThrowIfNull( pCmdQueue, nameof(pCmdQueue) ) ;
		if( pCmdQueue.ComPointer is null ) throw new NullReferenceException( nameof(pCmdQueue.ComPointer) ) ;
#endif

		var hr = COMObject.CreateSwapChain( pCmdQueue.ComPointer.Interface, desc, out var pSwapChain ) ;
		if(hr.Failed || pSwapChain is null) {
			ppSwapChain = null ;
			return hr ;
		}
		
		ppSwapChain = (TSwapChain)(TSwapChain.Instantiate(pSwapChain)) ;
		return hr ;
	}
} ;
