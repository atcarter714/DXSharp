﻿using System.Runtime.InteropServices ;
using System.Text ;
using System.Windows.Forms ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp ;
using DXSharp.Applications ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;

namespace HelloTexture ;


public class GraphicsPipeline: DXGraphics {
	const int MAX_ADAPTERS = 0x10 ;
	const FactoryCreateFlags FACTORY_FLAGS =
#if DEBUG
		FactoryCreateFlags.Debug ;
#else
		FactoryCreateFlags.None ;
#endif
	HWnd             _wnd ;
	BasicApp?        _app ;
	GraphicsSettings _settings ;
	PCWSTR _DBGName_Factory = default,
		   _DBGName_Device = default ;
	
	IFactory7 _factory ;
	IDevice12? _device ;

	public GraphicsPipeline( GraphicsSettings? settings = null ) {
		_DBGName_Factory = "DXSharp.DXGI.IFactory7" ;
		_DBGName_Device = "DXSharp.Direct3D12.IDevice12" ;
		Add( _DBGName_Factory ) ;
		Add( _DBGName_Device ) ;
		
		_app      = BasicApp.Instance ?? 
					throw new InvalidOperationException( "Application instance is not initialized!" ) ;
		_settings = settings ?? GraphicsSettings.Default ;
		
		_wnd    = _app.Window.Handle ;
	}

	
	unsafe void _setDBGName( in PCWSTR name, in IDXCOMObject obj ) {
		obj.SetPrivateData( COMUtility.WKPDID_D3DDebugObjectName,
									(uint)name.Length * sizeof(char), (nint)name.Value ) ;
	}
	
	
	public override void LoadPipeline( ) {
		// Create the DXGI factory:
		var hr = DXGIFunctions.CreateDXGIFactory2( FACTORY_FLAGS, IFactory7.IID, out _factory ) ;
		hr.ThrowOnFailure( ) ;
		_setDBGName( _DBGName_Factory, _factory ) ;
		
		// Get the best GPU:
		using var adapter = _getBestGPU( _factory ) ;
		
		// Create the D3D12 device:
		var device = D3D12.CreateDevice< IDevice10 >( adapter, FeatureLevel.D3D12_0 ) ;
		
		// Describe & create the command queue:
		CommandQueueDescription queueDesc = new( CommandListType.Direct ) ;
		device.CreateCommandQueue( queueDesc, ICommandQueue.IID, out ICommandQueue? cmdQueue ) ;
		
		// Describe the swap chain:
		var swapChainDesc = new SwapChainDescription1 {
			BufferCount = _settings.BufferCount,
			Width = 0, Height = 0,
			Format = Format.R8G8B8A8_UNORM,
			BufferUsage = Usage.RenderTargetOutput,
			SwapEffect = SwapEffect.FlipDiscard,
			SampleDesc = ( 1, 0 ),
		} ;
		var swapchainFSDesc = new SwapChainFullscreenDescription {
			RefreshRate = new Rational( 60, 1 ),
			Scaling     = ScalingMode.Stretched,
			Windowed    = true,
		} ;
		
		// Create the swapchain:
		_factory.CreateSwapChainForHwnd( cmdQueue, _wnd, swapChainDesc, swapchainFSDesc, 
										 null, out ISwapChain1? swapChain ) ;
		_factory.MakeWindowAssociation( _wnd, WindowAssociation.NoAltEnter ) ;
		
		// Create the descriptor heaps:
		uint frameCount  = _settings.BufferCount ;
		DescriptorHeapDescription rtvHeapDesc = new( DescriptorHeapType.RTV, frameCount ) ;
		device.CreateDescriptorHeap( rtvHeapDesc, IDescriptorHeap.IID, out IDescriptorHeap? rtvHeap ) ;
		
		DescriptorHeapDescription dsvHeapDesc = new( DescriptorHeapType.CBV_SRV_UAV, 1, 
													 DescriptorHeapFlags.ShaderVisible ) ;
		device.CreateDescriptorHeap( dsvHeapDesc, IDescriptorHeap.IID, out IDescriptorHeap? dsvHeap ) ;
		
		var rtvHandle       = rtvHeap.GetCPUDescriptorHandleForHeapStart( ) ;
		var rtvDescSize = device.GetDescriptorHandleIncrementSize( DescriptorHeapType.RTV ) ;
		
		// Create frame resources with a RTV for each frame:
		for ( uint n = 0; n < frameCount; ++n ) {
			swapChain.GetBuffer< IResource2 >( n, out var buffer ) ;
			device.CreateRenderTargetView( buffer, default, rtvHandle ) ;
			rtvHandle.Offset( 1, rtvDescSize ) ;
		}
		
		static IAdapter4? _getBestGPU( IFactory7 factory ) {
			IAdapter4? bestAdapter = null ;
			ulong maxVRAM = 0 ;
			
			for( uint i = 0; i < MAX_ADAPTERS; ++i ) {
				IAdapter1? _adapter   = default ;
				var        hr   = factory.EnumAdapters1( i, out _adapter ) ;
				
				//! Is this the last adapter?
				if( hr == HResult.DXGI_ERROR_NOT_FOUND || _adapter is null ) break ;
				else hr.ThrowOnFailure( ) ;
				
				var adapter = COMUtility.Cast< IAdapter1, IAdapter4 >( _adapter ) ;
				
				
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
			return bestAdapter ;
		}
	}

	public override void LoadAssets( ) { }
}



/*
	void _setDBGName( in string name, in IDXCOMObject? obj ) {
		var    pStr = Marshal.StringToCoTaskMemAuto( name ) ;
		obj?.SetPrivateData< char >( COMUtility.WKPDID_D3DDebugObjectName,
									  (uint)name.Length * sizeof(char), (nint)pStr ) ;
	}

 */