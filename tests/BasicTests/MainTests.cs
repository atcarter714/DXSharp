#pragma warning disable CS1591

#region Using Directives
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi;
using static Windows.Win32.PInvoke;

using DXSharp.DXGI;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Dxgi.Common;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Direct3D;
using WinRT;
using Windows.Devices.Sms;
using ABI.Windows.UI;
using System.Drawing;

#endregion

namespace BasicTests.DirectX12;


[TestFixture, FixtureLifeCycle(LifeCycle.SingleInstance)]
public class D3D12_Graphics_Interop
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

	RECT rect;
	D3D12_VIEWPORT viewport;
	
	static int	width			= 1024, 
				height			= 768, 
				bufferCount		= 2;

	static RenderForm? renderForm;
	static IDXGIFactory7? factory;
	static List<IDXGIAdapter4> adapters;
	static ID3D12Debug5? debugController;

	static ID3D12Fence? fence1;
	static ID3D12Device9? device;
	static IDXGISwapChain4? swapChain;
	static ID3D12DescriptorHeap? rtvHeap;
	static ID3D12Resource[]? renderTargets;
	static ID3D12RootSignature? rootSignature;
	static ID3D12PipelineState? pipelineState;

	static ID3D12CommandQueue? commandQueue;
	static ID3D12CommandAllocator? commandAlloc;
	static ID3D12GraphicsCommandList1? commandList;



	IDXGIAdapter4? getBestAdapter() {
		int i = 0, index = -1; long maxvRam = long.MinValue;
		foreach( var adapter in adapters ) {
			adapter.GetDesc3(out var desc);
			if ( desc.Flags.HasFlag(DXGI_ADAPTER_FLAG3.DXGI_ADAPTER_FLAG3_SOFTWARE) ) continue;

			// Most VRAM yet?
			if((long)desc.DedicatedVideoMemory > maxvRam) {
				index = i;
				maxvRam = (long)desc.DedicatedVideoMemory;
			}
			++i;
		}

		return index > -1 ? adapters[ index ]: null;
	}

	bool enumAdapters<T>( T? factoryX ) where T : IDXGIFactory2 {
		Assert.IsNotNull( factoryX );

		// Iterate over adapters (maximum of 16 times)
		for ( uint index = 0; index <= 0x10; ++index ) { try {

				// Try to get next adapter"
				factoryX.EnumAdapters1( index, out var ppAdapter );
				Assert.IsNotNull( ppAdapter );

				// Validate adapter object and obtain DXGI COM interface:
				if (ppAdapter is null)
					throw new NullReferenceException($"enumAdapters< {typeof(T).Name} >(): " +
						"Adapter interface from EnumAdapters1 is null!");

				// Get COM interface ref:
				IDXGIAdapter4? adptr4 = default;
				Assert.DoesNotThrow( () => {
					adptr4 = ppAdapter as IDXGIAdapter4 ??
						throw new COMException($"enumAdapters< {typeof(T).Name} >(): " +
						$"Cannot obtain IDXGIAdapter4* from {ppAdapter.GetType().Name} {nameof(ppAdapter)}!"); });

				// Assert and add it to list:
				Assert.IsNotNull(adptr4);
				adapters.Add(adptr4);
			}
			catch ( COMException comEx ) {

				// Are we finished with all adapters?
				if ( comEx.HResult == HRESULT.DXGI_ERROR_NOT_FOUND )
					return true;

				throw comEx;
			}
		}

		return adapters?.Count > 0;
	}



	[OneTimeSetUp]
	public void Setup() {
		adapters = new List<IDXGIAdapter4>( 0x04 );

		rect = new RECT( 0, 0, width, height );
		viewport = new D3D12_VIEWPORT() { Height = height, Width = width, 
			MaxDepth = 1, MinDepth = 0, TopLeftX = 0, TopLeftY = 0 };
	}

	[OneTimeTearDown]
	public void Cleanup()
	{
		if(factory is not null) 
			Marshal.ReleaseComObject(factory);
		if(adapters is not null)
			foreach (var a in adapters)
				if (a is not null) Marshal.ReleaseComObject(a);
		if (debugController is not null)
			Marshal.ReleaseComObject(debugController);
		if (fence1 is not null)
			Marshal.ReleaseComObject(fence1);
		if (commandList is not null)
			Marshal.ReleaseComObject(commandList);
		if (commandQueue is not null)
			Marshal.ReleaseComObject(commandQueue);
		if (commandAlloc is not null)
			Marshal.ReleaseComObject(commandAlloc);
		if (device is not null)
			Marshal.ReleaseComObject(device);
		if (swapChain is not null)
			Marshal.ReleaseComObject(swapChain);
	}


	[Test, Order(0)]
	public void Create_DXGI_Factory()
	{
		var hr = CreateDXGIFactory2(0u, typeof(IDXGIFactory7).GUID, out var factoryObj);
		Assert.True(hr.Succeeded);

		Assert.DoesNotThrow( () => {
			factory = factoryObj as IDXGIFactory7 ??
				throw new COMException("Create_DXGI_Factory(): Failed to obtain IDXGIFactory7 reference!");
		});
		Assert.NotNull(factory);
	}

	[Test, Order(1)]
	public void Get_DXGI_Adapters()
	{
		Assert.IsNotNull(factory);
		Assert.DoesNotThrow(() => { enumAdapters( factory ); });
		Assert.NotZero(adapters.Count);
	}

	[Test, Order(2)]
	public void Create_Direct3D_Debug_Layer()
	{
		// Create a debug layer:
		debugController = default;
		var hr = D3D12GetDebugInterface( typeof(ID3D12Debug5).GUID, out var debugLayerObj );

		if ( hr.Succeeded ) {
			Assert.DoesNotThrow( () => {

				// Verify and cast the results:
				debugController = debugLayerObj as ID3D12Debug5 ??
					throw new COMException("Could not create ID3D12Debug5 layer!");
			});
			Assert.IsNotNull(debugController);
			debugController.EnableDebugLayer();
		}
		else throw new COMException("Failed to create debug layer for Direct3D12!");
	}

	[Test, Order(3)]
	public void Create_RenderForm()
	{
		// Initialize the render form:
		renderForm = new RenderForm( "DX# Render Form (Unit Test)" ) {
			ClientSize = new System.Drawing.Size( width, height ),
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D,
			BackColor = System.Drawing.Color.White,
		};
		Assert.IsNotNull( renderForm );

		renderForm.Show();
		renderForm.Activate();
		renderForm.Focus();
		Assert.IsTrue( renderForm.Visible );
	}

	[Test, Order(4)]
	public void Create_D3D12_Device()
	{
		Assert.IsNotNull(factory);

		// Get an adapter:
		Assert.IsNotEmpty( adapters );
		var adapter = getBestAdapter();
		Assert.IsNotNull( adapter );

		// Create device and swapchain for d3d12
		var hr = D3D12CreateDevice( adapter, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_0,
			typeof(ID3D12Device9).GUID, out var deviceObj );
		
		Assert.IsTrue( hr.Succeeded );

		device = null;
		Assert.DoesNotThrow( () => {
			device = deviceObj as ID3D12Device9 ??
				throw new COMException( "Initialization of ID3D12Device9 interface failed!" ); });

		Assert.IsNotNull( device );
	}

	[Test, Order(5)]
	public void Create_D3D12_CommandQueue()
	{
		// Describe and create the command queue:
		commandQueue = default;
		var creatorID = typeof(ID3D12Device9).GUID;
		var cmdQueueID = typeof(ID3D12CommandQueue).GUID;

		D3D12_COMMAND_QUEUE_DESC queueDesc = new D3D12_COMMAND_QUEUE_DESC() {
			Flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE,
			Type = D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT,
		};
		
		device.CreateCommandQueue1( queueDesc, creatorID, cmdQueueID, out var cmdQObj );

		Assert.DoesNotThrow( () => {
			commandQueue = cmdQObj as ID3D12CommandQueue ??
				throw new COMException("Create_D3D12_CommandQueue(): Cannot obtain ID3D12CommandQueue* for COM interface!"); });
		Assert.IsNotNull( commandQueue );
	}

	[Test, Order(6)]
	public void Create_D3D12_CommandList()
	{
		Assert.IsNotNull(device);

		Assert.DoesNotThrow( () =>
		{
			device.CreateCommandList1(	0x00u,
										D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT,
										D3D12_COMMAND_LIST_FLAGS.D3D12_COMMAND_LIST_FLAG_NONE,
										typeof( ID3D12GraphicsCommandList5 ).GUID,
										out object cmdListObj);

			commandList = cmdListObj as ID3D12GraphicsCommandList5 ??
				throw new COMException("Create_D3D12_CommandList(): " +
					"Cannot obtain reference to D3D12 command list!");
		});
		Assert.IsNotNull(commandList);
	}

	[Test, Order(7)]
	public void Create_DXGI_SwapChain()
	{
		Assert.IsNotNull( renderForm );
		Assert.IsNotNull( commandQueue );

		// Create swapchain descriptions:
		var scDesc = new SwapChainDescription1( renderForm.ClientSize.Width, renderForm.ClientSize.Height,
			Format.R8G8B8A8_UNORM, false, new SampleDescription(1, 0), Usage.RenderTargetOutput, (uint)bufferCount,
			Scaling.Stretch, SwapEffect.FlipDiscard, AlphaMode.Ignore, SwapChainFlags.FrameLatencyWaitableObject );
		var scfsDesc = new SwapChainFullscreenDescription( 60u, ScanlineOrder.Unspecified, ScalingMode.Stretched, true );

		Assert.DoesNotThrow( () => {
			factory.CreateSwapChainForHwnd(
				commandQueue,
				(HWND)renderForm.Handle,
				scDesc.InternalValue,
				scfsDesc.InternalValue,
				null,
				out var swapChainObj);

			swapChain = swapChainObj as IDXGISwapChain4 ??
				throw new COMException( "Initialization of IDXGISwapChain4 interface failed!" );
		});
		Assert.IsNotNull(swapChain);
	}

	[Test, Order(8)]
	public void Create_D3D12_CommandAlloc() {

		Assert.IsNotNull( device );

		Assert.DoesNotThrow( () => {
		
			device.CreateCommandAllocator( 
				D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT, 
				typeof( ID3D12CommandAllocator ).GUID, out var allocObj );

			commandAlloc = allocObj as ID3D12CommandAllocator ??
				throw new COMException( "Create_D3D12_CommandAlloc(): " +
					"Cannot obtain reference to D3D12 command allocator!" );
		} );
		Assert.IsNotNull( commandList );
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