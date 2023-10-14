using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32.Helpers;
using DXSharp.Windows.Win32.XTensions;


class HelloWin32App: DXApp {
	const uint FrameCount = 2;
	// Pipeline objects.
	//Viewport m_viewport ;
	//Rect     m_scissorRect ;
	//uint     m_rtvDescriptorSize ;
	const uint w = 1024, h = 768 ; //! default window size
	
	//! Typical objects used for DX12 rendering:
	/*ComPtr<ID3D12Device>?           m_device ;
	ComPtr<IDXGISwapChain3>?        m_swapChain ;
	ComPtr<ID3D12CommandQueue>?     m_commandQueue ;
	ComPtr<ID3D12CommandAllocator>? m_commandAllocator ;
	
	ComPtr<ID3D12DescriptorHeap>?      m_rtvHeap ;
	ComPtr<ID3D12GraphicsCommandList>? m_commandList ;
	ComPtr<ID3D12RootSignature>?       m_rootSignature ;
	ComPtr<ID3D12PipelineState>?       m_pipelineState ;*/
	
	ComPtr<ID3D12Resource>?[ ]?        m_renderTargets =
					new ComPtr<ID3D12Resource>[FrameCount] ;
	
	// -- Constructor ----------------------------------------------

	public HelloWin32App( uint width = w, uint height = h, string name = "Hello DXSharp App" ):
		base( width, height, "Hello DXSharp App" ) { }

	// -- Instance Methods ------------------------------------------
	
	public override void OnUpdate( ) { }
	public override void OnRender( ) { }

	public override void OnKeyDown(byte wParam)
	{
		base.OnKeyDown(wParam);

		// Handle other keydown events if needed.
	}
	public override void OnKeyUp(byte wParam)
	{
		base.OnKeyUp(wParam);

		// Handle other keyup events if needed.
	}
	
	// --------------------------------------------------------------------
	// Entry Point / Startup Code ::
	// --------------------------------------------------------------------
	
	internal static void Run( params string[ ] args ) {
		// Initialize the application.
		var app = new HelloWin32App( ) ;
        
		// Pseudocode: Parse arguments (if DXApp doesn't automatically do this).
		app.ParseCommandLineArgs( args ) ;

		// Pseudocode: Assuming Win32Application has a static 'Run' method that takes DXApp.
	}
	
	// ====================================================================
}