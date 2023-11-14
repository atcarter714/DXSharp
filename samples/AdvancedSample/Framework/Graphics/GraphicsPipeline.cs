// --------------------------------------------------------
// Graphics Pipeline Notes:
// --------------------------------------------------------
// The plan is to figure out a good design/architecture we
// are happy with, then implement it in DXSharp.Framework as
// a reusable piece of boilerplate infrastructure ...
// --------------------------------------------------------
#region Using Directives
using Windows.Win32.Foundation ;

using DXSharp ;
using DXSharp.Dxc ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Direct3D12 ;
using DXSharp.Windows.COM ;
using DXSharp.Applications ;
using DXSharp.Framework.Graphics ;
using DXSharp.Windows.Win32 ;
#endregion
namespace AdvancedDXS.Framework.Graphics ;


public class GraphicsPipeline: DXGraphics {
	#region Constant/ReadOnly Members
	// -----------------------------------------------------
	// Constant & Read-Only Values:
	// -----------------------------------------------------
	const int MAX_ADAPTERS = 0x10 ;
	const FactoryCreateFlags FACTORY_FLAGS =
#if DEBUG
		FactoryCreateFlags.Debug ;
#else
		FactoryCreateFlags.None ;
#endif
	// ===================================================
	#endregion
	
	
	// -----------------------------------------------------
	// Instance Fields:
	// -----------------------------------------------------
	HWnd             _wnd ;
	DXSApp?          _app ;
	GraphicsSettings _settings ;
	ShaderBuilder _shaderBuilder ;
	PCWSTR _DBGName_Factory = default,
		   _DBGName_Device  = default ;
	
	IFactory7? _factory ;
	IDevice10? _device ;
	ISwapChain4? _swapChain ;
	IDescriptorHeap? _rtvHeap, _dsvHeap ;
	// =====================================================
	
	
	public GraphicsPipeline( GraphicsSettings? settings = null ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		_DBGName_Factory = "DXSharp.DXGI.IFactory7" ;
		_DBGName_Device  = "DXSharp.Direct3D12.IDevice12" ;
		Add( _DBGName_Factory ) ;
		Add( _DBGName_Device ) ;
#endif
		
		_app      = DXSApp.Instance ?? 
					throw new InvalidOperationException( "Application instance is not initialized!" ) ;
		_settings = settings ?? GraphicsSettings.Default ;
		
		_wnd    = _app.Window?.Handle ?? HWnd.Null ;
		if( _wnd.IsNull ) throw new DXSharpException( "Window handle is null!" ) ;
		
		_shaderBuilder = new( ) ;
		Add( _shaderBuilder ) ;
	}
	
	// -----------------------------------------------------
	
	
	public override void LoadPipeline( ) {
		// Create the DXGI factory:
		var hr = DXGIFunctions.CreateFactory2( IFactory7.IID, out var _f7 ) ;
		_factory = (IFactory7) _f7  ;
		
#if DEBUG || DEBUG_COM || DEV_BUILD
		hr.ThrowOnFailure( ) ;
		if( _factory is null ) throw new DXSharpException($"Failed to create DXGI factory! (hr = {hr})") ;
		_setDBGName( _DBGName_Factory, _factory ) ;
#endif
		// Get the best GPU:
		using var adapter = _getBestGPU( _factory ) ;
		
		// Create the D3D12 device:
		_device = D3D12.CreateDevice< IDevice10 >( adapter, FeatureLevel.D3D12_0 ) ;
		if ( _device is null ) throw new DXSharpException( $"Failed to create {nameof(IDevice10)}!" ) ;
		else Add( _device ) ;
		
		// Describe & create the command queue:
		_device.CreateCommandQueue( CommandQueueDescription.Default,
								   ICommandQueue.IID, out ICommandQueue? cmdQueue ) ;
		if( cmdQueue is null ) throw new DXSharpException( "Failed to create command queue!" ) ;
		else Add( cmdQueue ) ;
		
		// Describe the swap chain:
		var swapChainDesc = new SwapChainDescription1 {
			Width = _settings.DisplayMode.Resolution.Width,
			Height = _settings.DisplayMode.Resolution.Height,
			SampleDesc  = ( 1, 0 ),
			Format      = Format.R8G8B8A8_UNORM,
			SwapEffect  = SwapEffect.FlipDiscard,
			BufferUsage = Usage.RenderTargetOutput,
			BufferCount = _settings.BackBufferOptions.Count,
		} ;
		var swapchainFSDesc = new SwapChainFullscreenDescription {
			RefreshRate = new Rational( 60, 1 ),
			Scaling     = ScalingMode.Stretched,
			Windowed    = true,
		} ;
		
		// Create the swapchain:
		_factory.CreateSwapChainForHwnd( cmdQueue, _wnd, swapChainDesc, swapchainFSDesc, 
										 null, out var swapChain1 ) ;
		var swapChain = COMUtility.Cast< ISwapChain1, ISwapChain4 >( swapChain1 ) ;
		if( swapChain is null ) throw new DXSharpException( "Failed to create swap chain!" ) ;
		else Add( swapChain ) ;
		swapChain1?.DisposeAsync( ) ;
		
		
		// Set the window association flags:
		_factory.MakeWindowAssociation( _wnd, WindowAssociation.NoAltEnter ) ;
		
		// Create the descriptor heaps:
		uint frameCount  = _settings.BackBufferOptions.Count ;
		DescriptorHeapDescription rtvHeapDesc = new( DescriptorHeapType.RTV, frameCount ) ;
		_device.CreateDescriptorHeap( rtvHeapDesc, IDescriptorHeap.IID, out IDescriptorHeap? rtvHeap ) ;
		if( rtvHeap is null ) throw new DXSharpException( "Failed to create RTV descriptor heap!" ) ;
		
		DescriptorHeapDescription dsvHeapDesc = new( DescriptorHeapType.CBV_SRV_UAV,
													 1, DescriptorHeapFlags.ShaderVisible ) ;
		_device.CreateDescriptorHeap( dsvHeapDesc, IDescriptorHeap.IID, out IDescriptorHeap? dsvHeap ) ;
		if( dsvHeap is null ) throw new DXSharpException( "Failed to create DSV descriptor heap!" ) ;
		var rtvHandle       = rtvHeap!.GetCPUDescriptorHandleForHeapStart( ) ;
		var rtvDescSize = _device.GetDescriptorHandleIncrementSize( DescriptorHeapType.RTV ) ;
		
		// Create frame resources with a RTV for each frame:
		for ( uint n = 0; n < frameCount; ++n ) {
			swapChain.GetBuffer< IResource2 >( n, out var buffer ) ;
			_device.CreateRenderTargetView( buffer, default, rtvHandle ) ;
			rtvHandle.Offset( 1, rtvDescSize ) ;
		}
		
		
		static IAdapter4? _getBestGPU( IFactory7 factory ) {
			List< IAdapter1 > adapters = new( ) ;
			IAdapter4? bestAdapter = null ;
			ulong maxVRAM = 0 ;
			
			for( uint i = 0; i < MAX_ADAPTERS; ++i ) {
				IAdapter1? _adapter   = default ;
				var        hr   = factory.EnumAdapters1( i, out _adapter ) ;
				
				//! Is this the last adapter?
				if( hr == HResult.DXGI_ERROR_NOT_FOUND || _adapter is null ) break ;
				else hr.ThrowOnFailure( ) ;
				
				adapters.Add( _adapter ) ;
				var adapter = COMUtility.Cast< IAdapter1, IAdapter4 >( _adapter ) ;
				if ( adapter is null ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
					throw new DXSharpException( $"Failed to cast " +
												$"{nameof(IAdapter1)} to " +
												$"{nameof(IAdapter4)}!" ) ;
#else
					continue;
#endif
				}
				
				adapter.GetDesc3( out var _desc ) ;
				
				// Skip software and remote adapters:
				if( _desc.Flags.HasFlag( AdapterFlag3.Software ) 
					|| _desc.Flags.HasFlag( AdapterFlag3.Remote ) ) continue ;
				
				ulong vram = (ulong)_desc.DedicatedVideoMemory ;
				if( vram > maxVRAM ) {
					maxVRAM = vram ;
					bestAdapter = (IAdapter4)adapter ;
				}
			}
			foreach ( var adapter in adapters ) adapter.Dispose( ) ;
			return bestAdapter ;
		}
	}

	public override void LoadAssets( ) {
		
	}
	
	
	
	// -----------------------------------------------------
	// Static Helper Methods:
	// -----------------------------------------------------
	static unsafe void _setDBGName( in PCWSTR name, in IDXCOMObject obj ) =>
		obj.SetPrivateData( COMUtility.WKPDID_D3DDebugObjectName,
							(uint)name.Length * sizeof(char), (nint)name.Value ) ;
	static PCWSTR _setDBGName( string name, in IDXCOMObject obj ) {
		unsafe {
			PCWSTR _name = name ;
			obj.SetPrivateData( COMUtility.WKPDID_D3DDebugObjectName,
								(uint)name.Length * sizeof( char ),
										(nint)_name.Value ) ;
			return _name ;
		}
	}
	// =================================================================================================================
} ;

