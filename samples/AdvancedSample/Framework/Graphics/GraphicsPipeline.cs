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
using IDevice = DXSharp.Direct3D12.IDevice ;
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
	
	public const int FrameCount = 3,
					 NumContexts = 3,
					 CommandListCount = 3,
					 CommandListPre = 0,
					 CommandListMid = 1,
					 CommandListPost = 2 ;
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

	uint rtvDescSize = 0 ;
	int frameIndex   = 0 ;
	uint frameCount  = 3 ;
	
	IFactory7? _factory ;
	IDevice10? _device ;
	ISwapChain4? _swapChain ;
	IDescriptorHeap? _rtvHeap, _dsvHeap ;
	
	ICommandQueue? _cmdQueue ;
	IGraphicsCommandList9? _commandList ;
	ICommandAllocator? _commandAllocator ;
	
	IGraphicsCommandList9[ ] _commandLists = 
		new IGraphicsCommandList9[ 1 ] ;
	
	IFence1?        fence ;
	ulong           fenceValue ;
	AutoResetEvent? fenceEvent ;
	
	ResourceBarrier[ ] transitionBarriers 
		= new ResourceBarrier[ 1 ] ;
	
	// -----------------------------------------------------
	public override IDevice10? GraphicsDevice => _device ;
	
	public float AspectRatio => MainViewport.Width / MainViewport.Height ;
	public Viewport MainViewport => Viewports?[ 0 ] ?? default ;
	
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
									ICommandQueue.IID, out _cmdQueue ) ;
		if( _cmdQueue is null ) throw new DXSharpException( "Failed to create command queue!" ) ;
		else Add( _cmdQueue ) ;
		
		// Describe the swap chain:
		var swapChainDesc = new SwapChainDescription1 {
			Width = _settings.CurrentMode.Resolution.Width,
			Height = _settings.CurrentMode.Resolution.Height,
			SampleDesc  = ( 1, 0 ),
			Format      = Format.R8G8B8A8_UNORM,
			SwapEffect  = SwapEffect.FlipDiscard,
			BufferUsage = Usage.RenderTargetOutput,
			BufferCount = _settings.RenderTargetProperties.BufferCount,
		} ;
		var swapchainFSDesc = new SwapChainFullscreenDescription {
			RefreshRate = new Rational( 60, 1 ),
			Scaling     = ScalingMode.Stretched,
			Windowed    = true,
		} ;
		
		// Create the swapchain:
		_factory.CreateSwapChainForHwnd( _cmdQueue, _wnd, swapChainDesc, swapchainFSDesc, 
										 null, out var swapChain1 ) ;
		var swapChain = COMUtility.Cast< ISwapChain1, ISwapChain4 >( swapChain1 ) ;
		if( swapChain is null ) throw new DXSharpException( "Failed to create swap chain!" ) ;
		else Add( swapChain ) ;
		swapChain1?.DisposeAsync( ) ;
		
		// Set the window association flags:
		_factory.MakeWindowAssociation( _wnd, WindowAssociation.NoAltEnter ) ;
		
		// Create the descriptor heaps:
		uint frameCount  = _settings.RenderTargetProperties.BufferCount ;
		DescriptorHeapDescription rtvHeapDesc = new( DescriptorHeapType.RTV, frameCount ) ;
		_device.CreateDescriptorHeap( rtvHeapDesc, IDescriptorHeap.IID, out IDescriptorHeap? rtvHeap ) ;
		if( rtvHeap is null ) throw new DXSharpException( "Failed to create RTV descriptor heap!" ) ;
		
		DescriptorHeapDescription dsvHeapDesc = new( DescriptorHeapType.CBV_SRV_UAV,
													 1, DescriptorHeapFlags.ShaderVisible ) ;
		_device.CreateDescriptorHeap( dsvHeapDesc, IDescriptorHeap.IID, out IDescriptorHeap? dsvHeap ) ;
		if( dsvHeap is null ) throw new DXSharpException( "Failed to create DSV descriptor heap!" ) ;
		var rtvHandle       = rtvHeap!.GetCPUDescriptorHandleForHeapStart( ) ;
		rtvDescSize = _device.GetDescriptorHandleIncrementSize( DescriptorHeapType.RTV ) ;
		
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
		// Create an empty root signature using AllowInputAssemblerInputLayout flag:
		var serializedRootSig = 
			new RootSignatureDescription( RootSignatureFlags.AllowInputAssemblerInputLayout )
				.Serialize( ) ?? throw new NullReferenceException( "Failed to serialize root signature!" ) ;
		
		GraphicsDevice!.CreateRootSignature( 0,
											 serializedRootSig.Pointer, 
											 serializedRootSig.Size,
											 IRootSignature.IID, out m_rootSignature ) ;

		
		
		// Create synchronization objects:
		GraphicsDevice.CreateFence( 0, FenceFlags.None,
								   IFence1.IID, out var _ppFence ) ;
		fence = (IFence1?) _ppFence ?? 
				throw new NullReferenceException( "Failed to create fence!" ) ;
		
		// Create event handle for synchronization:
		fenceEvent = new(false ) ;
		fenceValue = 1 ;
	}

	public override void OnRender( ) {
		// Wait for the previous frame to complete.
		WaitForPreviousFrame( ) ;
		
		// Populate command list:
		PopulateCommandList( ) ;
		
		// Execute the command list:
		_cmdQueue!.ExecuteCommandLists< IGraphicsCommandList >( (uint)_commandLists.Length, _commandLists ) ;
		
		// Present the frame:
		SwapChain!.Present( 1, 0 ) ;
		frameIndex ^= 1 ; //! Alternate between the two back buffers.
	}

	void PopulateCommandList( ) {
		// Command list allocators can only be reset when the associated 
		// command lists have finished execution on the GPU; apps should use 
		// fences to determine GPU execution progress.
		_commandAllocator!.Reset( ) ;

		// However, when ExecuteCommandList() is called on a particular command list,
		// that command list can then be reset at any time and must be before re-recording.
		_commandList!.Reset( CommandAllocator, PipelineState ) ;


		// Set necessary state.
		_commandList.SetGraphicsRootSignature( RootSignature ) ;
		_commandList.RSSetViewports( 1, Viewports ) ;
		_commandList.RSSetScissorRects( 1, ScissorRects ) ;

		// Indicate that the back buffer will be used as a render target:
		transitionBarriers[ 0 ] = ResourceBarrier.Transition( RenderTargets![ frameIndex ], 
															  ResourceStates.Present, 
															  ResourceStates.RenderTarget ) ;
		_commandList.ResourceBarrier( 1, transitionBarriers ) ;
		
		
		// Set the render target for the output merger stage:
		var rtvHandle = _rtvHeap!.GetCPUDescriptorHandleForHeapStart( ) ;
		//rtvHandle += frameIndex * rtvDescSize ;
		rtvHandle.Offset( frameIndex, rtvDescSize ) ;
		_commandList.OMSetRenderTargets(1,
									   new[ ] { rtvHandle },
									   false ) ;

		// Clear the render target:
		_commandList.ClearRenderTargetView( rtvHandle, BackgroundColor ) ;

		// Draw the triangle:
		//_commandList.IASetPrimitiveTopology( PrimitiveTopology.D3D_TriangleList ) ;
		//_commandList.IASetVertexBuffers(0, 1, new[ ] { vertexBufferView } ) ;
		//_commandList.DrawInstanced( 3, 1, 0, 0 ) ;
		
		
		// Indicate that the back buffer will now be used to present:
		transitionBarriers[ 0 ] = ResourceBarrier.Transition( RenderTargets[ frameIndex ],
															  ResourceStates.RenderTarget, 
															  ResourceStates.Present ) ;

		_commandList.ResourceBarrier( 1, transitionBarriers ) ;
		
		// Done recording commands ...
		_commandList.Close( ) ;
	}
	
	void WaitForPreviousFrame( ) {
		ulong localFence = fenceValue ;
		CommandQueue!.Signal( fence, fenceValue++ ) ;
		
		// Wait until the previous frame is finished
		if ( fence!.GetCompletedValue( ) < localFence ) {
			var fenceEventHandle = fenceEvent!.SafeWaitHandle.DangerousGetHandle( ) ;
			fence.SetEventOnCompletion( localFence, fenceEventHandle ) ;
			fenceEvent.WaitOne( ) ;
		}
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

