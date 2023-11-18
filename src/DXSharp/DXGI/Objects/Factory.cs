#region Using Directives

using System.Collections.ObjectModel ;
using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;
using Windows.Win32 ;
using Windows.UI.Core ;
using Windows.Win32.Graphics.Dxgi ;
using winMD = Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Direct3D12 ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
using HResult = DXSharp.Windows.HResult ;

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


[SupportedOSPlatform("windows5.0")]
[Wrapper(typeof(IDXGIFactory))]
internal class Factory: Object,
						IFactory,
						IComObjectRef< IDXGIFactory >,
						IUnknownWrapper< IDXGIFactory > {
	// -----------------------------------------------------------------------------------
	ComPtr< IDXGIFactory >? _comPointer ;
	public new virtual ComPtr< IDXGIFactory >? ComPointer => 
		_comPointer ??= ComResources?.GetPointer< IDXGIFactory >( ) ;
	
	public override IDXGIFactory? ComObject => ComPointer?.Interface ;
	// -----------------------------------------------------------------------------------
	
	internal Factory( ) {
		_comPointer = ComResources?.GetPointer< IDXGIFactory >( ) ;
	}
	internal Factory( nint ptr ) {
		_comPointer = new( ptr ) ;
		_initOrAdd( _comPointer ) ;
	}
	internal Factory( IDXGIFactory factory ) {
		_comPointer = new( factory ) ;
		_initOrAdd( _comPointer ) ;
	}
	internal Factory( ComPtr< IDXGIFactory > ptr ) {
		_comPointer = ptr ;
		_initOrAdd( _comPointer ) ;
	}

	// -----------------------------------------------------------------------------------

	[SupportedOSPlatform( "windows8.0" )]
	public HResult CreateSwapChain( in ICommandQueue? pCmdQueue,
									in SwapChainDescription desc,
											out ISwapChain? ppSwapChain ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( ComPointer?.Disposed ?? true, nameof(Factory) ) ;
		ArgumentNullException.ThrowIfNull( pCmdQueue, nameof(pCmdQueue) ) ;
#endif
		
		unsafe {
			ppSwapChain = default ;
			var descCopy = desc ;
			var cmdQueue = (IComObjectRef< ID3D12CommandQueue >)pCmdQueue ;
			
			var _hr = ComObject!.CreateSwapChain( cmdQueue.ComObject,
												(DXGI_SWAP_CHAIN_DESC *)&descCopy, 
													out IDXGISwapChain? pSwapChain ) ;
			
			if( pSwapChain is null || _hr.Failed ) return _hr ;
			ppSwapChain = new SwapChain(pSwapChain) ;
			return _hr ;
		}
	}
	
	
	public void CreateSoftwareAdapter( HInstance Module, out IAdapter? ppAdapter ) {
		_ = ComObject ?? throw new NullReferenceException( ) ;
		ppAdapter = default ;
		ComObject.CreateSoftwareAdapter( Module, out IDXGIAdapter? pAdapter ) ;
		ppAdapter = new Adapter( pAdapter ) ;
	}

	
	public void MakeWindowAssociation( in HWnd WindowHandle, WindowAssociation Flags ) =>
		( _ = ComObject ?? throw new NullReferenceException() )
			.MakeWindowAssociation( WindowHandle, (uint)Flags ) ;

	
	public HWnd GetWindowAssociation( ) {
		GetWindowAssociation( out var h ) ;
		return h ;
	}
	
	
	public void GetWindowAssociation( out HWnd pWindowHandle ) {
		_ = ComObject ?? throw new NullReferenceException( ) ;
		unsafe {
			winMD.HWND handle = default ;
			ComObject.GetWindowAssociation( &handle ) ;
			pWindowHandle = new( handle ) ;
		}
	}
	
	
	public HResult EnumAdapters( uint index, out IAdapter? ppAdapter ) {
		var factory = ComObject ?? throw new NullReferenceException( ) ;
		ppAdapter = default ;
		
		var hr = factory.EnumAdapters( index, out IDXGIAdapter? pAdapter ) ;
		if ( hr.Failed ) {
			ppAdapter = null ;
			return hr ;
		}
		
		var adapter = ( new Adapter(pAdapter) ) ;
		ppAdapter = adapter ;
		
		return hr ;
	}
	
	// -----------------------------------------------------------------------------------
	
	public static TFactory Create< TFactory >( ) where TFactory: IFactory, IInstantiable {
		var dxgiFactory = DXGIFunctions.CreateDXGIFactory< IDXGIFactory >( )
						  ?? throw new DirectXComError( "Failed to create DXGI Factory!" ) ;

		var tfactory = (TFactory)TFactory.Instantiate( dxgiFactory ) ;
		return tfactory ;
	}
	
	// -----------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIFactory ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIFactory).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// ====================================================================================
} ;


[SupportedOSPlatform( "windows6.1" )]
[Wrapper(typeof(IDXGIFactory1))]
internal class Factory1: Factory,
						 IFactory1,
						 IComObjectRef< IDXGIFactory1 >,
						 IUnknownWrapper< IDXGIFactory1 > {
	// -------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIFactory1 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIFactory1).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	public new static TFactory Create< TFactory >( ) where TFactory: IFactory, IInstantiable {
		var dxgiFactory = DXGIFunctions.CreateDXGIFactory1< IDXGIFactory1 >( ) 
			?? throw new DirectXComError( "Failed to create DXGI Factory!" ) ;
		return (TFactory)TFactory.Instantiate( dxgiFactory ) ;
	}
	
	
	[SuppressMessage( "Interoperability", "CA1416:Validate platform compatibility" )] 
	static readonly ReadOnlyDictionary< Guid, Func<IDXGIAdapter, IInstantiable> > _adapterCreationFunctions =
		new( new Dictionary< Guid, Func< IDXGIAdapter, IInstantiable > >( ) {
			{ IAdapter.IID, ( a ) => new Adapter( a ) },
			{ IAdapter1.IID, ( a ) => new Adapter1( (a as IDXGIAdapter1)! ) },
			{ IAdapter2.IID, ( a ) => new Adapter2( (a as IDXGIAdapter2)! ) },
			{ IAdapter3.IID, ( a ) => new Adapter3( (a as IDXGIAdapter3)! ) },
			{ IAdapter4.IID, ( a ) => new Adapter4( (a as IDXGIAdapter4)! ) },
		} ) ;
	
	// -------------------------------------------------------------------------------------

	ComPtr< IDXGIFactory1 >? _comPointer1 ; 
	public override IDXGIFactory1? ComObject => ComPointer?.Interface ;
	public new virtual ComPtr< IDXGIFactory1 >? ComPointer =>
		_comPointer1 ??= ComResources?.GetPointer< IDXGIFactory1 >( ) ;
	

	internal Factory1( ) {
		_comPointer1 = ComResources?.GetPointer< IDXGIFactory1 >( ) ;
	}
	internal Factory1( nint ptr ) {
		_comPointer1 = new( ptr ) ;
		_initOrAdd( _comPointer1 ) ;
	}
	internal Factory1( IDXGIFactory1 factory ) {
		_comPointer1 = new( factory ) ;
		_initOrAdd( _comPointer1 ) ;
	}
	internal Factory1( ComPtr< IDXGIFactory1 > ptr ) {
		_comPointer1 = ptr ;
		_initOrAdd( _comPointer1 ) ;
	}

	
	public bool IsCurrent( ) => ComObject!.IsCurrent( ) ;
	
	[SupportedOSPlatform("windows6.1")]
	public HResult EnumAdapters1( uint index, out IAdapter1? ppAdapter ) {
		if( ComObject is null ) throw new NullReferenceException( ) ;
		ppAdapter = default ;
		
		var _hr = ComObject.EnumAdapters1( index, out IDXGIAdapter1? pAdapter ) ;
		if( _hr.Failed ) {
			ppAdapter = null ;
			return _hr ;
		}
		
		ppAdapter = new Adapter1( pAdapter ) ;
		return _hr ;
	}
	
} ;


[SupportedOSPlatform( "windows8.0" )]
[Wrapper(typeof(IDXGIFactory2))]
internal class Factory2: Factory1,
						 IFactory2,
						 IComObjectRef< IDXGIFactory2 >,
						 IUnknownWrapper< IDXGIFactory2 > {
	// -------------------------------------------------------------------------------------

	ComPtr< IDXGIFactory2 >? _comPointer2 ;
	public new virtual ComPtr< IDXGIFactory2 >? ComPointer =>
		_comPointer2 ??= ComResources?.GetPointer< IDXGIFactory2 >( ) ;
	public override IDXGIFactory2? ComObject => ComPointer?.Interface ;
	
	// -------------------------------------------------------------------------------------
	
	internal Factory2( ) {
		_comPointer2 = ComResources?.GetPointer< IDXGIFactory2 >( ) ;
	}
	internal Factory2( nint ptr ) {
		_comPointer2 = new( ptr ) ;
		_initOrAdd( _comPointer2 ) ;
	}
	internal Factory2( IDXGIFactory2 factory ) {
		_comPointer2 = new( factory ) ;
		_initOrAdd( _comPointer2 ) ;
	}
	internal Factory2( ComPtr< IDXGIFactory2 > ptr ) {
		_comPointer2 = ptr ;
		_initOrAdd( _comPointer2 ) ;
	}

	// -------------------------------------------------------------------------------------
	
	public bool IsWindowedStereoEnabled( ) => ComObject!.IsWindowedStereoEnabled( ) ;
	
	
	public void GetSharedResourceAdapterLuid( Win32Handle hResource, out Luid pLuid ) {
		unsafe {
			pLuid = default ;
			fixed( Luid* pLuidPtr = &pLuid )
				ComObject!.GetSharedResourceAdapterLuid( hResource, (winMD.LUID *) pLuidPtr ) ;
		}
	}

	
	public void CreateSwapChainForHwnd( ICommandQueue pCommandQueue, HWnd hWnd,
										in            SwapChainDescription1           pDesc,
										[Optional] in SwapChainFullscreenDescription? pFullscreenDesc,
										[Optional]    IOutput?                        pRestrictToOutput,
										out           ISwapChain1                     ppSwapChain ) {
		unsafe {
			void* fsDescPtr      = null ;
			var   device         = (IComObjectRef< ID3D12CommandQueue >)pCommandQueue ;
			var   restrictOutput = (IComObjectRef< IDXGIOutput >)pRestrictToOutput! ;
			
			fixed( void* _fsDesc = &pFullscreenDesc, _desc = &pDesc ) {
				if( pFullscreenDesc.HasValue ) fsDescPtr = _fsDesc ;
				
				ComObject!.CreateSwapChainForHwnd( device.ComObject,
												   hWnd,
												   (DXGI_SWAP_CHAIN_DESC1 *)_desc,
												   (DXGI_SWAP_CHAIN_FULLSCREEN_DESC *)fsDescPtr,
												   restrictOutput?.ComObject,
												   out var pSwapChain ) ;
				
				ppSwapChain = new SwapChain1( pSwapChain ?? throw new NullReferenceException( ) ) ;
			}
		}
	}

	
	public void CreateSwapChainForCoreWindow( ICommandQueue pCmdQueue,
											  CoreWindow pWindow,
											  in SwapChainDescription1 pDesc,
											  IOutput pRestrictToOutput,
											  out ISwapChain1 ppSwapChain ) {
		unsafe {
			var device = (IComObjectRef< ID3D12CommandQueue >)pCmdQueue ;
			var restrictOutput = (IComObjectRef< IDXGIOutput >)pRestrictToOutput ;
			fixed( void *_desc = &pDesc ) {
				ComObject!.CreateSwapChainForCoreWindow( device.ComObject,
														 pWindow,
														 (DXGI_SWAP_CHAIN_DESC1 *)_desc,
														 restrictOutput?.ComObject,
														 out var pSwapChain ) ;
				ppSwapChain = new SwapChain1( pSwapChain ?? throw new NullReferenceException( ) ) ;
			}
		}
	}


	public void RegisterStereoStatusWindow( HWnd WindowHandle, uint wMsg, out uint pdwCookie ) => 
		ComObject!.RegisterStereoStatusWindow( WindowHandle, wMsg, out pdwCookie ) ;
	
	public void RegisterStereoStatusEvent( Win32Handle hEvent, out uint pdwCookie ) =>
		ComObject!.RegisterStereoStatusEvent( hEvent, out pdwCookie ) ;

	
	public void UnregisterStereoStatus( uint dwCookie ) => 
		ComObject!.UnregisterStereoStatus( dwCookie ) ;

	public void RegisterOcclusionStatusWindow( HWnd WindowHandle, uint wMsg, out uint pdwCookie ) =>
		ComObject!.RegisterOcclusionStatusWindow( WindowHandle, wMsg, out pdwCookie ) ;
	
	public void RegisterOcclusionStatusEvent( Win32Handle hEvent, out uint pdwCookie ) =>
		ComObject!.RegisterOcclusionStatusEvent( hEvent, out pdwCookie ) ;
	
	public void UnregisterOcclusionStatus( uint dwCookie ) => 
		ComObject!.UnregisterOcclusionStatus( dwCookie ) ;

	public unsafe void CreateSwapChainForComposition( ICommandQueue pDevice,
																in SwapChainDescription1 pDesc,
																IOutput pRestrictToOutput,
																out ISwapChain1 ppSwapChain ) {
		var allocator      = (IComObjectRef< ID3D12CommandQueue >)pDevice ;
		var restrictOutput = (IComObjectRef< IDXGIOutput >)pRestrictToOutput ;
		fixed( void* _desc = &pDesc ) {
			ComObject!.CreateSwapChainForComposition( allocator.ComObject,
													  (DXGI_SWAP_CHAIN_DESC1*)_desc,
													  restrictOutput?.ComObject,
													  out var pSwapChain ) ;
			ppSwapChain = new SwapChain1( pSwapChain ) ;
		}
	}
	
	// -------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIFactory2 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIFactory2 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =====================================================================================
} ;


[SupportedOSPlatform( "windows8.1" )]
[Wrapper( typeof( IDXGIFactory3 ) )]
internal class Factory3: Factory2,
						 IFactory3,
						 IComObjectRef< IDXGIFactory3 >,
						 IUnknownWrapper< IDXGIFactory3 > {
	// -------------------------------------------------------------------------------------
	ComPtr< IDXGIFactory3 >? _comPointer3 ;
	public new virtual ComPtr< IDXGIFactory3 >? ComPointer =>
		_comPointer3 ??= ComResources?.GetPointer< IDXGIFactory3 >( ) ;
	public override IDXGIFactory3? ComObject => ComPointer?.Interface ;

	// -------------------------------------------------------------------------------------

	internal Factory3( ) {
		_comPointer3 = ComResources?.GetPointer< IDXGIFactory3 >( ) ;
	}

	internal Factory3( nint ptr ) {
		_comPointer3 = new( ptr ) ;
		_initOrAdd( _comPointer3 ) ;
	}

	internal Factory3( IDXGIFactory3 factory ) {
		_comPointer3 = new( factory ) ;
		_initOrAdd( _comPointer3 ) ;
	}

	internal Factory3( ComPtr< IDXGIFactory3 > ptr ) {
		_comPointer3 = ptr ;
		_initOrAdd( _comPointer3 ) ;
	}

	// -------------------------------------------------------------------------------------
	
	public FactoryCreateFlags GetCreationFlags( ) =>
		(FactoryCreateFlags)ComObject!.GetCreationFlags( ) ;
	
	// -------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIFactory3 ) ;

	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIFactory3 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =====================================================================================
} ;


[Wrapper(typeof(IDXGIFactory4))]
internal class Factory4: Factory3,
						 IFactory4,
						 IComObjectRef< IDXGIFactory4 >,
						 IUnknownWrapper< IDXGIFactory4 > {
	// -------------------------------------------------------------------------------------
	ComPtr< IDXGIFactory4 >? _comPointer4 ;
	public new virtual ComPtr< IDXGIFactory4 >? ComPointer =>
		_comPointer4 ??= ComResources?.GetPointer< IDXGIFactory4 >( ) ;
	public override IDXGIFactory4? ComObject => ComPointer?.Interface ;
	// -------------------------------------------------------------------------------------

	internal Factory4( ) {
		_comPointer4 = ComResources?.GetPointer< IDXGIFactory4 >( ) ;
	}

	internal Factory4( nint ptr ) {
		_comPointer4 = new( ptr ) ;
		_initOrAdd( _comPointer4 ) ;
	}

	internal Factory4( IDXGIFactory4 factory ) {
		_comPointer4 = new( factory ) ;
		_initOrAdd( _comPointer4 ) ;
	}

	internal Factory4( ComPtr< IDXGIFactory4 > ptr ) {
		_comPointer4 = ptr ;
		_initOrAdd( _comPointer4 ) ;
	}
	
	// -------------------------------------------------------------------------------------
	
	[SuppressMessage( "Interoperability", "CA1416:Validate platform compatibility" )]
	public HResult EnumAdapterByLuid< A >( Luid AdapterLuid, in Guid riid, out A? ppvAdapter ) where A: IAdapter {
		var     factory = ComObject ?? throw new NullReferenceException( ) ;
		HResult hr      = factory.EnumAdapterByLuid( AdapterLuid, riid, out var pAdapter ) ;
		
		var functionLookup = IAdapter._adapterCreationFunctions ;
		if ( !functionLookup.ContainsKey(riid) ) {
			ppvAdapter = default ;
			return HResult.E_NOINTERFACE ;
		}

		var createFn = functionLookup[ riid ] ;
		ppvAdapter = (A?)createFn( (pAdapter as IDXGIAdapter)! ) ;

#if DEBUG || DEBUG_COM
		if( ppvAdapter is null ) {
			if ( hr.Succeeded ) { // This means we had a valid COM object, but failed to create the wrapper:
				Debug.WriteLine( $"{nameof(Factory4)} :: " +
								 $"Failed to create {typeof(A).Name} with {pAdapter.GetType( ).Name}" ) ;
			}
		}
#endif
		return hr ;
	}
	
	
	[SuppressMessage( "Interoperability", "CA1416:Validate platform compatibility" )]
	public HResult EnumWarpAdapter< A >( in Guid riid, out A? ppvAdapter ) where A: IAdapter {
		var factory = ComObject ?? throw new NullReferenceException( ) ;
		HResult hr = factory.EnumWarpAdapter( riid, out var pAdapter ) ;
		
		var functionLookup = IAdapter._adapterCreationFunctions ;
		if ( !functionLookup.ContainsKey(riid) ) {
			ppvAdapter = default ;
			return HResult.E_NOINTERFACE ;
		}

		var createFn = functionLookup[ riid ] ;
		ppvAdapter = (A?)createFn( (pAdapter as IDXGIAdapter)! ) ;

#if DEBUG || DEBUG_COM
		if( ppvAdapter is null ) {
			if ( hr.Succeeded ) { // This means we had a valid COM object, but failed to create the wrapper:
				Debug.WriteLine( $"{nameof(Factory4)} :: " +
								 $"Failed to create {typeof(A).Name} with {pAdapter.GetType( ).Name}" ) ;
			}
		}
#endif
		return hr ;
	}
	
	// -------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIFactory4 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIFactory4 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =====================================================================================
} ;



[Wrapper(typeof(IDXGIFactory5))]
internal class Factory5: Factory4,
						 IFactory5,
						 IComObjectRef< IDXGIFactory5 >,
						 IUnknownWrapper< IDXGIFactory5 > {
	// -------------------------------------------------------------------------------------
	ComPtr< IDXGIFactory5 >? _comPointer5 ;
	public new virtual ComPtr< IDXGIFactory5 >? ComPointer =>
		_comPointer5 ??= ComResources?.GetPointer< IDXGIFactory5 >( ) ;
	public override IDXGIFactory5? ComObject => ComPointer?.Interface ;

	// -------------------------------------------------------------------------------------

	internal Factory5( ) {
		_comPointer5 = ComResources?.GetPointer< IDXGIFactory5 >( ) ;
	}

	internal Factory5( nint ptr ) {
		_comPointer5 = new( ptr ) ;
		_initOrAdd( _comPointer5 ) ;
	}

	internal Factory5( IDXGIFactory5 factory ) {
		_comPointer5 = new( factory ) ;
		_initOrAdd( _comPointer5 ) ;
	}

	internal Factory5( ComPtr< IDXGIFactory5 > ptr ) {
		_comPointer5 = ptr ;
		_initOrAdd( _comPointer5 ) ;
	}
	
	// -------------------------------------------------------------------------------------
	
	public void CheckFeatureSupport( Feature Feature, nint pFeatureSupportData, uint FeatureSupportDataSize ) {
		unsafe {
			ComObject!.CheckFeatureSupport( (DXGI_FEATURE)Feature,
											(void*)pFeatureSupportData,
											FeatureSupportDataSize ) ;
		}
	}
	
	// -------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIFactory5 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIFactory5 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =====================================================================================
} ;


[SupportedOSPlatform("windows10.0.17134")]
[Wrapper(typeof(IDXGIFactory6))]
internal class Factory6: Factory5,
						 IFactory6,
						 IComObjectRef< IDXGIFactory6 >,
						 IUnknownWrapper< IDXGIFactory6 > {
	// -------------------------------------------------------------------------------------
	ComPtr< IDXGIFactory6 >? _comPointer6 ;
	public new virtual ComPtr< IDXGIFactory6 >? ComPointer =>
		_comPointer6 ??= ComResources?.GetPointer< IDXGIFactory6 >( ) ;
	public override IDXGIFactory6? ComObject => ComPointer?.Interface ;
	// -------------------------------------------------------------------------------------

	internal Factory6( ) {
		_comPointer6 = ComResources?.GetPointer< IDXGIFactory6 >( ) ;
	}
	internal Factory6( nint ptr ) {
		_comPointer6 = new( ptr ) ;
		_initOrAdd( _comPointer6 ) ;
	}
	internal Factory6( IDXGIFactory6 factory ) {
		_comPointer6 = new( factory ) ;
		_initOrAdd( _comPointer6 ) ;
	}
	internal Factory6( ComPtr< IDXGIFactory6 > ptr ) {
		_comPointer6 = ptr ;
		_initOrAdd( _comPointer6 ) ;
	}

	// -------------------------------------------------------------------------------------

	public HResult EnumAdapterByGPUPreference< A >( uint adapterOrdinal, GPUPreference GpuPreference,
													in Guid riid, out A? ppvAdapter ) where A: IAdapter {
		var factory = ComObject ?? throw new NullReferenceException( ) ;
		HResult hr = factory.EnumAdapterByGpuPreference( adapterOrdinal, (DXGI_GPU_PREFERENCE)GpuPreference, riid, out var pAdapter ) ;
		if( hr.Failed || pAdapter is null ) {
			ppvAdapter = default ;
			return hr ;
		}
		
		//ppvAdapter = (A)A.Instantiate( pAdapter as IDXGIAdapter ) ;
		var functionLookup = IAdapter._adapterCreationFunctions ;
		if ( !functionLookup.ContainsKey(riid) ) {
			ppvAdapter = default ;
			return HResult.E_NOINTERFACE ;
		}
		
		var createFn = functionLookup[ riid ] ;
		ppvAdapter = (A?)createFn( (pAdapter as IDXGIAdapter)! ) ;

#if DEBUG || DEBUG_COM
		if( ppvAdapter is null ) {
			if ( hr.Succeeded ) { // This means we had a valid COM object, but failed to create the wrapper:
				Debug.WriteLine( $"{nameof(Factory6)} :: " +
								 $"Failed to create {typeof(A).Name} with {pAdapter.GetType( ).Name}" ) ;
			}
		}
#endif
		return hr ;
	}
	
	// -------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIFactory6 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIFactory6 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =====================================================================================
} ;


[SupportedOSPlatform("windows10.0.17763")]
[Wrapper(typeof(IDXGIFactory7))]
internal class Factory7: Factory6,
						 IFactory7,
						 IComObjectRef< IDXGIFactory7 >,
						 IUnknownWrapper< IDXGIFactory7 > {
	// -------------------------------------------------------------------------------------
	ComPtr< IDXGIFactory7 >? _comPointer7 ;
	public new virtual ComPtr< IDXGIFactory7 >? ComPointer =>
		_comPointer7 ??= ComResources?.GetPointer< IDXGIFactory7 >( ) ;
	public override IDXGIFactory7? ComObject => ComPointer?.Interface ;
	// -------------------------------------------------------------------------------------

	internal Factory7( ) {
		_comPointer7 = ComResources?.GetPointer< IDXGIFactory7 >( ) ;
	}
	internal Factory7( nint ptr ) {
		_comPointer7 = new( ptr ) ;
		_initOrAdd( _comPointer7 ) ;
	}
	internal Factory7( IDXGIFactory7 factory ) {
		_comPointer7 = new( factory ) ;
		_initOrAdd( _comPointer7 ) ;
	}
	internal Factory7( ComPtr< IDXGIFactory7 > ptr ) {
		_comPointer7 = ptr ;
		_initOrAdd( _comPointer7 ) ;
	}
	// -------------------------------------------------------------------------------------
	
	public void RegisterAdaptersChangedEvent( Win32Handle hEvent, out uint pdwCookie ) =>
		ComObject!.RegisterAdaptersChangedEvent( hEvent, out pdwCookie ) ;

	public void UnregisterAdaptersChangedEvent( uint dwCookie ) =>
		ComObject!.UnregisterAdaptersChangedEvent( dwCookie ) ;
	
	// -------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIFactory7 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIFactory7 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// =====================================================================================
} ;


