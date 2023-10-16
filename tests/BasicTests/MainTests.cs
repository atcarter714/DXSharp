#pragma warning disable CS1591

#region Using Directives

/* Unmerged change from project 'BasicTests (net6.0-windows10.0.22621.0)'
Before:
using Microsoft.Win32;
After:
using ABI.Windows.UI;

using DXSharp.DXGI;

using Microsoft.Win32;
*/
using DXSharp.DXGI;


/* Unmerged change from project 'BasicTests (net6.0-windows10.0.22621.0)'
Before:
using System.Runtime.InteropServices;

using System.Drawing;
After:
using System.Drawing;
using System.Runtime.InteropServices;
*/
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms ;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D;
/* Unmerged change from project 'BasicTests (net6.0-windows10.0.22621.0)'
Before:
using Windows.Win32.Graphics.Dxgi;
using static Windows.Win32.PInvoke;

using DXSharp.DXGI;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Dxgi.Common;
After:
using Windows.Win32.Sms;
using static Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D;
*/
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;
/* Unmerged change from project 'BasicTests (net6.0-windows10.0.22621.0)'
Before:
using WinRT;
using Windows.Devices.Sms;
using ABI.Windows.UI;
using System.Drawing;
After:
using Windows.Devices.Graphics.Dxgi.Common;

using WinRT;

using static Windows.Win32.PInvoke;
*/
using static Windows.Win32.PInvoke;

#endregion

namespace BasicTests;


[TestFixture, FixtureLifeCycle( LifeCycle.SingleInstance )]
public class D3D12GraphicsInterop
{
	/* Native Pipeline Objects
		D3D12_VIEWPORT m_viewport;
		D3D12_RECT m_scissorRect;
		ComPtr<IDXGISwapChain3> m_swapChain;
		ComPtr<ID3D12Device> m_device;
		ComPtr<ID3D12Resource> m_renderTargets[FrameCount];
		ComPtr<ID3D12CommandAllocator> m_commandAllocator;
		ComPtr<ID3D12CommandQueue> m_commandQueue;
		ComPtr<ID3D12RootSignature> m_rootSignature;
		ComPtr<ID3D12DescriptorHeap> m_rtvHeap;
		ComPtr<ID3D12PipelineState> m_pipelineState;
		ComPtr<ID3D12GraphicsCommandList> m_commandList;
		UINT m_rtvDescriptorSize;
	 */

	RECT _rect;
	D3D12_VIEWPORT _viewport;
	const int   _width = 1024,
				_height = 768,
				_bufferCount = 2;

	// Disable the NUnit warning/error about disposing of the form (we handle it in OneTimeTearDown)
#pragma warning disable NUnit1032
	static RenderForm? _renderForm;
#pragma warning restore NUnit1032

	static IDXGIFactory7? _factory;
	static List<IDXGIAdapter4> _adapters;
	static ID3D12Debug5? _debugController;

	static readonly ID3D12Fence? _fence1;
	static ID3D12Device9? _device;
	static IDXGISwapChain4? _swapChain;
	static readonly ID3D12DescriptorHeap? _rtvHeap;
	static readonly ID3D12Resource[]? _renderTargets;
	static readonly ID3D12RootSignature? _rootSignature;
	static readonly ID3D12PipelineState? _pipelineState;

	static ID3D12CommandQueue? _commandQueue;
	static ID3D12CommandAllocator? _commandAlloc;
	static ID3D12GraphicsCommandList1? _commandList;


	static IDXGIAdapter4? GetBestAdapter() {
		int i = 0, index = -1; long maxvRam = Int64.MinValue;
		DXGI_ADAPTER_DESC3 desc = default;

		unsafe
		{
			DXGI_ADAPTER_DESC3* pDesc = &desc;
			foreach (var adapter in _adapters)
			{
				adapter.GetDesc3(pDesc);
				if (desc.Flags.HasFlag(DXGI_ADAPTER_FLAG3.DXGI_ADAPTER_FLAG3_SOFTWARE)) continue;

				// Most VRAM yet?
				if ((long)desc.DedicatedVideoMemory > maxvRam)
				{
					index = i;
					maxvRam = (long)desc.DedicatedVideoMemory;
				}

				++i;
			}

		}

		return index > -1 ? _adapters[ index ] : null;
	}

	static bool EnumAdapters<T>( T? factoryX ) where T : IDXGIFactory2 {
		Assert.IsNotNull( factoryX );

		// Iterate over adapters (maximum of 16 times)
		for( uint index = 0; index <= 0x10; ++index ) {
			try {

				// Try to get next adapter"
				factoryX!.EnumAdapters1( index, out var ppAdapter );

				// Validate adapter object and obtain DXGI COM interface:
				if( ppAdapter is null )
					throw new NullReferenceException( $"enumAdapters< {typeof( T ).Name} >(): " +
						"Adapter interface from EnumAdapters1 is null!" );

				// Get COM interface ref:
				IDXGIAdapter4? adptr4 = default;
				Assert.DoesNotThrow( () =>
				{
					adptr4 = ppAdapter as IDXGIAdapter4 ??
						throw new COMException( $"enumAdapters< {typeof( T ).Name} >(): " +
						$"Cannot obtain IDXGIAdapter4* from {ppAdapter.GetType().Name} {nameof( ppAdapter )}!" );
				} );

				// Assert and add it to list:
				Assert.IsNotNull( adptr4 );
				_adapters.Add( adptr4 );
			}
			catch( COMException comEx ) {

				// Are we finished with all adapters?
				if( comEx.HResult == HResult.DXGI_ERROR_NOT_FOUND )
					return true;

				throw;
			}
			catch {
				return true;
			}
		}

		return _adapters?.Count > 0;
	}



	[OneTimeSetUp]
	public void Setup() {
		_adapters = new List<IDXGIAdapter4>( 0x04 );

		_rect = new RECT( 0, 0, _width, _height );
		_viewport = new D3D12_VIEWPORT {
			Height = _height, Width = _width,
			MaxDepth = 1, MinDepth = 0, TopLeftX = 0, TopLeftY = 0
		};
	}

	[OneTimeTearDown]
	public void Cleanup() {
		if( _factory is not null )
			_ = Marshal.ReleaseComObject( _factory );
		if( _adapters is not null )
			foreach( var a in _adapters )
				if( a is not null ) _ = Marshal.ReleaseComObject( a );
		if( _debugController is not null )
			_ = Marshal.ReleaseComObject( _debugController );
		if( _fence1 is not null )
			_ = Marshal.ReleaseComObject( _fence1 );
		if( _commandList is not null )
			_ = Marshal.ReleaseComObject( _commandList );
		if( _commandQueue is not null )
			_ = Marshal.ReleaseComObject( _commandQueue );
		if( _commandAlloc is not null )
			_ = Marshal.ReleaseComObject( _commandAlloc );
		if( _device is not null )
			_ = Marshal.ReleaseComObject( _device );
		if( _swapChain is not null )
			_ = Marshal.ReleaseComObject( _swapChain );
		if( _renderForm is not null )
			_renderForm.Dispose( ) ;
	}


	[Test, Order( 0 )]
	public void Create_DXGI_Factory() {
		var hr = CreateDXGIFactory2(0u, typeof(IDXGIFactory7).GUID, out object? factoryObj);
		Assert.True( hr.Succeeded );

		Assert.DoesNotThrow( () =>
		{
			_factory = factoryObj as IDXGIFactory7 ??
				throw new COMException( "Create_DXGI_Factory(): Failed to obtain IDXGIFactory7 reference!" );
		} );
		Assert.NotNull( _factory );
	}

	[Test, Order( 1 )]
	public void Get_DXGI_Adapters() {
		Assert.That( _factory, Is.Not.Null );
		bool good = EnumAdapters( _factory );
		Assert.That( good, Is.True );
		Assert.NotZero( _adapters.Count );
	}

	[Test, Order( 2 )]
	public void Create_Direct3D_Debug_Layer( ) {
		// Create a debug layer:
		_debugController = default;
		var hr = D3D12GetDebugInterface( typeof(ID3D12Debug5).GUID, out object? debugLayerObj );

		if( hr.Succeeded ) {
			Assert.DoesNotThrow( () =>
			{

				// Verify and cast the results:
				_debugController = debugLayerObj as ID3D12Debug5 ??
					throw new COMException( "Could not create ID3D12Debug5 layer!" );
			} );
			Assert.IsNotNull( _debugController );
			_debugController.EnableDebugLayer();
		}
		else throw new COMException( "Failed to create debug layer for Direct3D12!" );
	}
	
	
	[STAThread, Test, Order( 3 )]
	public void Create_RenderForm( ) {
		void _initRenderForm( ) {
			if ( _renderForm is null ) 
				throw new NullReferenceException( ) ;
			_renderForm.BackColor       = Color.White ;
			_renderForm.ClientSize      = new Size( _width, _height ) ;
			_renderForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D ;
			//System.Windows.Forms.MessageBox.Show( $"{_renderForm.Handle}", "Handle:" ) ;
			_renderForm.HandleCreated +=
				( o, s ) => _showRenderForm( ) ;
		}
		void _showRenderForm( ) {
			if ( _renderForm is null ) 
				throw new NullReferenceException( ) ;
			var presentForm = ( ) => {
								  _renderForm.Activate( ) ;
								  if( !_renderForm.Visible )
									  _renderForm.Show( ) ;
							  } ;
			
			if( _renderForm.InvokeRequired )
				_renderForm.Invoke( presentForm ) ;
			else presentForm( ) ;
		}

		
		// Initialize the render form:
		_renderForm = new RenderForm( "DX# Render Form (Unit Test)" ) ;
		if( _renderForm.InvokeRequired )
			_renderForm.Invoke( _initRenderForm ) ;
		else _initRenderForm( ) ;

		Thread.Sleep( 3500 ) ;
		
		Assert.IsNotNull( _renderForm ) ;
		Assert.IsTrue( _renderForm.IsHandleCreated ) ;
		_showRenderForm( ) ;
		Thread.Sleep( 2000 ) ;
		Assert.IsTrue( _renderForm.Visible ) ;
	}

	[Test, Order( 4 )]
	public void Create_D3D12_Device() {
		Assert.IsNotNull( _factory );

		// Get an adapter:
		Assert.IsNotEmpty( _adapters );
		var adapter = GetBestAdapter();
		Assert.IsNotNull( adapter );

		// Create device and swapchain for d3d12
		var hr = D3D12CreateDevice( adapter, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_0,
			typeof(ID3D12Device9).GUID, out object? deviceObj );

		Assert.IsTrue( hr.Succeeded );

		_device = null;
		Assert.DoesNotThrow( () =>
		{
			_device = deviceObj as ID3D12Device9 ??
				throw new COMException( "Initialization of ID3D12Device9 interface failed!" );
		} );

		Assert.IsNotNull( _device );
	}

	[Test, Order( 5 )]
	public unsafe void Create_D3D12_CommandQueue() {
		// Describe and create the command queue:
		_commandQueue = default;
		var creatorId = typeof(ID3D12Device9).GUID;
		var cmdQueueId = typeof(ID3D12CommandQueue).GUID;

		var queueDesc = new D3D12_COMMAND_QUEUE_DESC()
		{
			Flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE,
			Type = D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT,
		};

		_device.CreateCommandQueue1( &queueDesc, &creatorId, &cmdQueueId, out object? cmdQObj );

		Assert.DoesNotThrow( () =>
		{
			_commandQueue = cmdQObj as ID3D12CommandQueue ??
				throw new COMException( "Create_D3D12_CommandQueue(): Cannot obtain ID3D12CommandQueue* for COM interface!" );
		} );
		Assert.IsNotNull( _commandQueue );
	}

	[Test, Order( 6 )]
	public void Create_D3D12_CommandList()
	{
		unsafe {
			Guid uid = typeof(ID3D12CommandList).GUID; Guid* pId = &uid;

			Assert.IsNotNull( _device );
			Assert.DoesNotThrow( () =>
			{
				_device.CreateCommandList1( 0x00u,
											D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT,
											D3D12_COMMAND_LIST_FLAGS.D3D12_COMMAND_LIST_FLAG_NONE,
											pId,
											out object cmdListObj );

				_commandList = cmdListObj as ID3D12GraphicsCommandList5 ??
					throw new COMException( "Create_D3D12_CommandList(): " +
						"Cannot obtain reference to D3D12 command list!" );
			} );
			Assert.IsNotNull( _commandList ); 
		}
	}

	[Test, Order( 7 )]
	public unsafe void Create_DXGI_SwapChain() {
		Assert.IsNotNull( _renderForm );
		Assert.IsNotNull( _commandQueue );

		// Create swapchain descriptions:
		var scDesc = new SwapChainDescription1( _renderForm.ClientSize.Width, _renderForm.ClientSize.Height,
			Format.R8G8B8A8_UNORM, false, new SampleDescription(1, 0), Usage.RenderTargetOutput, _bufferCount,
			Scaling.Stretch, SwapEffect.FlipDiscard, AlphaMode.Ignore, SwapChainFlags.FrameLatencyWaitableObject );
		var scfsDesc = new SwapChainFullscreenDescription( 60u, ScanlineOrder.Unspecified, ScalingMode.Stretched, true );

		var pSwapChainDesc = (DXGI_SWAP_CHAIN_DESC1*)&scDesc;
		var pScfsDesc = (DXGI_SWAP_CHAIN_FULLSCREEN_DESC*)&scDesc;
		IDXGISwapChain1 swapChainObj = default;

		Assert.DoesNotThrow( () =>
		{
			_factory?.CreateSwapChainForHwnd(
				_commandQueue,
				(HWND)_renderForm.Handle,
				pSwapChainDesc,
				pScfsDesc,
				null,
				out swapChainObj );

			_swapChain = swapChainObj as IDXGISwapChain4 ??
				throw new COMException( "Initialization of IDXGISwapChain4 interface failed!" );
		} );
		Assert.IsNotNull( _swapChain );
	}

	[Test, Order( 8 )]
	public unsafe void Create_D3D12_CommandAlloc() {

		Assert.IsNotNull( _device );

		Assert.DoesNotThrow( () =>
		{
			Guid id = typeof(ID3D12CommandAllocator).GUID;

			_device.CreateCommandAllocator(
				D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT,
				&id, out object? allocObj );

			_commandAlloc = allocObj as ID3D12CommandAllocator ??
				throw new COMException( "Create_D3D12_CommandAlloc(): " +
					"Cannot obtain reference to D3D12 command allocator!" );
		} );
		Assert.IsNotNull( _commandList );
	}
};

/*
 
		// Get the size of the message.
		uint messageLength = 0;
		HRESULT hr = pInfoQueue->GetMessage(DXGI_DEBUG_ALL, 0, NULL, &messageLength);
		if (hr == S_FALSE)
		{

			// Allocate space and get the message.
			DXGI_INFO_QUEUE_MESSAGE* pMessage = (DXGI_INFO_QUEUE_MESSAGE*)malloc(messageLength);
			hr = pInfoQueue->GetMessage(DXGI_DEBUG_ALL, 0, pMessage, &messageLength);

			// Do something with the message and free it
			if (hr == S_OK)
			{

				// ...
				// ...
				// ...
				free(pMessage);
			}
		}
 
 */ 