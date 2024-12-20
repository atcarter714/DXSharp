﻿#region Using Directives
using System.Buffers ;
using System.Drawing ;
using System.Runtime.CompilerServices ;
using System.Windows.Forms;

using DXSharp ;
using DXSharp.DXGI ;
using DXSharp.Direct3D12 ;
using DXSharp.Applications ;
using DXSharp.DXGI.XTensions ;
using DXSharp.Windows.Win32 ;
using DXSharp.Direct3D12.Shader ;
using DXSharp.Direct3D12.XTensions ;

using Vector2   = DXSharp.Vector2 ;
using Vector3   = DXSharp.Vector3 ;
using Vector4   = DXSharp.Vector4 ;
using IDevice   = DXSharp.Direct3D12.IDevice ;
using IResource = DXSharp.Direct3D12.IResource ;
#endregion
namespace BasicSample ;


/// <summary>
/// Simple graphics class to create and
/// destroy the D3D12 pipeline and assets.
/// </summary>
public class Graphics: DisposableObject {
	// --------------------------------------------------------------------------------------------------
	const string shaderFilePath = @"shader1.hlsl" ;
	public const int FrameCount = 2 ;
	
	HWnd                          hwnd ;
	IResource?                    vertexBuffer ;
	VertexBufferView              vertexBufferView ;
	readonly List< MemoryHandle > cleanupList   = new( ) ;
	readonly List< IDisposable >  _disposables  = new( ) ;
	readonly IResource?[]         renderTargets = new IResource?[ FrameCount ] ;
	
	uint                  rtvDescriptorSize ;
	IDescriptorHeap?      rtvHeap ;
	ICommandAllocator?    commandAllocator ;
	ICommandQueue?        commandQueue ;
	IRootSignature?       rootSignature ;
	IPipelineState?       pipelineState ;
	IGraphicsCommandList? commandList ;
	
	IFence?         fence ;
	AutoResetEvent? fenceEvent ;
	ulong           fenceValue ;
	uint            frameIndex ;
	ColorF          clearColor4 ;

	// --------------------------------------------------------------------------------------------------
	
	public IDXApp Application { get ; init ; }
	
	Rect[ ] scissorRects = new Rect[ 1 ] ;
	Viewport[ ] viewports = new Viewport[ 1 ] ;
	public Rect ScissorRect { get ; protected set ; }
	public Viewport MainViewport { get ; protected set ; }
	
	public IDevice? GraphicsDevice { get ; protected set ; }
	public ISwapChain? SwapChain { get ; protected set ; }
	
	public float AspectRatio => MainViewport.Width / MainViewport.Height ;
	ResourceBarrier[ ] transitionBarriers = new ResourceBarrier[ 1 ] ;
	
	float[ ] clearColor = new float[ 4 ] { 0.0f, 0.2f, 0.4f, 1.0f } ;
	protected ColorF bufferColor =>
		Application.Settings.StyleSettings?.BackBufferColor 
			?? AppSettings.Style.DEFAULT_BUFFER_COLOR ;
	
	// --------------------------------------------------------------------------------------------------
	
	public Graphics( IDXApp app ) => Application = app ;
	
	// --------------------------------------------------------------------------------------------------
	
	public void LoadPipeline( ) {
		hwnd = Application.Window!.Handle ;
		AppSettings settings = Application.Settings ?? AppSettings.Default ;
		
		// Create viewport and scissor rect:
		MainViewport = new Viewport( settings.WindowSize.Width, 
								 settings.WindowSize.Height ) ;
		viewports[ 0 ] = MainViewport ;
		ScissorRect = new Rect( 0, 0,
								settings.WindowSize.Width, 
								settings.WindowSize.Height ) ;
		scissorRects[ 0 ] = ScissorRect ;
		
		clearColor4 = settings?.StyleSettings?.BackBufferColor 
					  ?? AppSettings.Style.DEFAULT_BUFFER_COLOR ;
		
		// Create DXGI factory:
		using var factory = IFactory1.Create1< IFactory1 >( ) ;
		
		// Find best adapter (uses DXGI.XTensions):
		using IAdapter1? adapter = (IAdapter1)factory.FindBestAdapter( )! ;
		adapter.GetDesc1( out var _adapterDesc) ;
		
		// Create device (default is FeatureLevel.D3D_12_0):
		IDevice? device = D3D12.CreateDevice< IDevice >( adapter, FeatureLevel.D3D12_0 ) ;
		this.GraphicsDevice = device ?? throw new ApplicationException($"Failed to create device");
		
		// Create command queue:
		var cmdQDesc = new CommandQueueDescription {
			Flags = CommandQueueFlags.None,
			Type = CommandListType.Direct,
			NodeMask = 0,
			Priority = 0,
		} ;
		device.CreateCommandQueue( cmdQDesc, ICommandQueue.IID, out commandQueue ) ;
		
		// Create swapchain:
		var bufferDesc = new ModeDescription {
			Width            = settings!.WindowSize.Width,
			Height           = settings.WindowSize.Height,
			Format           = Format.R8G8B8A8_UNORM,
			RefreshRate      = new Rational( 60, 1 ),
			ScanlineOrdering = ScanlineOrder.Unspecified,
			Scaling          = ScalingMode.Unspecified,
		} ;
		var swapChainDescription = new SwapChainDescription {
			BufferCount  = FrameCount,
			SampleDesc   = ( 1, 0 ),
			SwapEffect   = SwapEffect.FlipDiscard,
			Flags        = SwapChainFlags.None,
			BufferUsage  = Usage.RenderTargetOutput,
			OutputWindow = hwnd,
			BufferDesc   = bufferDesc,
			Windowed     = true,
		} ;
		var hr = factory.CreateSwapChain( commandQueue, swapChainDescription, out var swapChain ) ;
		if( hr.Failed || swapChain is null ) throw new DirectXComError( hr, "Failed to create SwapChain!" ) ;
		this.SwapChain = swapChain ;
		
		// Set window association:
		factory.MakeWindowAssociation( hwnd, WindowAssociation.NoAltEnter ) ;
		
		
		// Create descriptor heap:
		var rtvHeapDesc = new DescriptorHeapDescription {
			NodeMask       = 0,
			NumDescriptors = FrameCount,
			Type           = DescriptorHeapType.RTV,
			Flags          = DescriptorHeapFlags.None,
		} ;
		device.CreateDescriptorHeap( rtvHeapDesc,
									 IDescriptorHeap.IID,
									 out rtvHeap ) ;
		
		rtvDescriptorSize = device.GetDescriptorHandleIncrementSize( DescriptorHeapType.RTV ) ;
		
		// Create render target views:
		_createRenderTargetViews( ) ;
		
		// Create command allocator:
		GraphicsDevice.CreateCommandAllocator( CommandListType.Direct, 
											   ICommandAllocator.IID, 
											   out commandAllocator ) ;
		
		void _createRenderTargetViews( ) {
			var rtvHandle = rtvHeap!.GetCPUDescriptorHandleForHeapStart( ) ;
			var rtvDesc   = new RenderTargetViewDescription {
				Format        = bufferDesc.Format,
				ViewDimension = RTVDimension.Texture2D,
			} ;
			
			for ( uint i = 0 ; i < FrameCount ; ++i ) {
				swapChain!.GetBuffer< IResource >( i, out var renderTarget ) ;
				GraphicsDevice.CreateRenderTargetView( renderTarget, rtvDesc, rtvHandle ) ;
				renderTargets[ i ] = renderTarget ;
				rtvHandle.ptr += rtvDescriptorSize ;
			}
		}
	}

	public void LoadAssets( ) {
		// Create an empty root signature using AllowInputAssemblerInputLayout flag:
		var serializedRootSig = 
			new RootSignatureDescription( RootSignatureFlags.AllowInputAssemblerInputLayout )
				.Serialize( ) ?? throw new NullReferenceException( "Failed to serialize root signature!" ) ;
		
		GraphicsDevice!.CreateRootSignature( 0,
											serializedRootSig.Pointer, 
											serializedRootSig.GetBufferSize( ),
											IRootSignature.IID, out rootSignature ) ;
		
		
		// Create the pipeline state, which includes compiling and loading shaders.
		var hr = ShaderCompiler.CompileFromFile( shaderFilePath,
												   default,
												   default,
												   "VSMain",
												   "vs_5_0", 0, 0,
												   out var vertexShaderBlob,
												   out var vertexShaderErrorMsgs ) ;
		if ( hr.Failed || vertexShaderBlob is null ) {
			string errorMessage = "Failed to compile vertex shader!" ;
			if ( vertexShaderErrorMsgs is not null ) {
				string error = ShaderCompiler.GetErrorMessage( vertexShaderErrorMsgs ) 
							   ?? $"No error message available!" ;
				errorMessage += $"\n{error}" ;
				MessageBox.Show(errorMessage, "Shader Compilation Error:") ;
			}
			throw new DirectXComError( hr, errorMessage ) ;
		}
		
		hr = ShaderCompiler.CompileFromFile( shaderFilePath,
											   default,
											   default,
											   "PSMain", 
											   "ps_5_0", 0, 0, 
											   out var pixelShaderBlob, 
											   out var pixelShaderErrorMsgs ) ;
		if ( hr.Failed || pixelShaderBlob is null ) {
			string errorMessage = "Failed to compile pixel shader!";
			if ( pixelShaderErrorMsgs is not null ) {
				string error = ShaderCompiler.GetErrorMessage( pixelShaderErrorMsgs ) 
							   ?? $"No error message available!" ;
				errorMessage += $"\n{error}" ;
				MessageBox.Show(errorMessage, "Shader Compilation Error:") ;
			}
			throw new DirectXComError( hr, errorMessage ) ;
		}
		
		// Create shader bytecode structures:
		ShaderBytecode vertexShader     = ShaderBytecode.FromShaderBlob( vertexShaderBlob ) ;
		ShaderBytecode pixelShader      = ShaderBytecode.FromShaderBlob( pixelShaderBlob ) ;
		_disposables.Add( vertexShaderBlob ) ; _disposables.Add( pixelShaderBlob ) ;
		
		// Define the vertex input layout:
		var inputElementDescs = new[ ] {
				new InputElementDescription {
					SemanticName    = "POSITION",
					InputSlot         = 0,
					SemanticIndex     = 0,
					AlignedByteOffset = 0,
					Format            = Format.R32G32B32_FLOAT, 
				},
				
				new InputElementDescription {
					SemanticName    = "COLOR",
					InputSlot         = 0,
					SemanticIndex     = 0,
					AlignedByteOffset = 12,
					Format            = Format.R32G32B32A32_FLOAT, 
				},
		} ;
		Memory< InputElementDescription > inputElements = new( inputElementDescs ) ;
		var inputLayoutMemory = InputLayoutDescription.Create( inputElements, out var inputLayoutDescription ) ;
		cleanupList.Add( inputLayoutMemory ) ;
		
		// Describe and create the graphics pipeline state object (PSO):
		var psoDesc = new GraphicsPipelineStateDescription( ) {
			InputLayout           = inputLayoutDescription,
			RootSignature         = rootSignature!,
			VS                    = vertexShader,
			PS                    = pixelShader,
			RasterizerState       = RasterizerDescription.Default,
			BlendState            = BlendDescription.Default,
			DSVFormat             = Format.D32_FLOAT,
			DepthStencilState     = new DepthStencilDesc { DepthEnable = false, StencilEnable = false },
			SampleMask            = int.MaxValue,
			PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
			NumRenderTargets      = 1,
			Flags                 = PipelineStateFlags.None,
			SampleDesc            = SampleDescription.Default,
			StreamOutput          = new StreamOutputDescription( ), // ??
			RTVFormats            = new( f0: Format.R8G8B8A8_UNORM ),
		} ;
		
		// Create the PSO:
		GraphicsDevice.CreateGraphicsPipelineState( psoDesc,
													IPipelineState.IID, 
													out pipelineState ) ;
		
		// Create the command list:
		GraphicsDevice.CreateCommandList( 0,
										  CommandListType.Direct,
										  commandAllocator!, 
										  pipelineState,
										  IGraphicsCommandList.IID,
										  out var cmdList ) ;
		this.commandList = (IGraphicsCommandList)cmdList ;
		
		// Create the vertex buffer & define geometry of a triangle:
		Vector3 p0 = ( 0, 0, 0 ) ;
		var triangleVertices = new[ ] {
				new VertexPosCol {
					Position = (0.0f, 0.25f * AspectRatio, 0.0f ), 
					Color = Color.Red,
				},
				new VertexPosCol {
					Position = (0.25f, -0.25f * AspectRatio, 0.0f), 
					Color = Color.Green,
				},
				new VertexPosCol {
					Position = (-0.25f, -0.25f * AspectRatio, 0.0f), 
					Color = Color.Blue,
				},
		};
		int vertexSize       = VertexPosCol.SizeInBytes ;
		int vertexBufferSize = ( vertexSize * triangleVertices.Length ) ;
		
		
		// Note: using upload heaps to transfer static data like vert buffers is not recommended.
		// Every time the GPU needs it, the upload heap will be marshalled over. Please read up on
		// Default Heap usage. An upload heap is used here for code simplicity and because there are
		// very few verts to actually transfer ...
		var resDesc  = new ResourceDescription { Alignment = VertexPosCol.SizeInBytes } ;
		GraphicsDevice.CreateCommittedResource( new ( HeapType.Upload ), HeapFlags.None,
												ResourceDescription.Buffer( (ulong)vertexBufferSize ),
									  ResourceStates.GenericRead,
												null,
												IResource.IID, out vertexBuffer ) ;
#if DEBUG || DEBUG_COM || DEV_BUILD
		if ( vertexBuffer is null )
			throw new DXSharpException( $"Failed to create vertex buffer!" ) ;
#endif
		
		
		// Copy the triangle data to the vertex buffer.
		nint mappedResource = nint.Zero ;
		vertexBuffer?.Map(0 , default, out mappedResource ) ;
		unsafe {
			VertexPosCol* pVertexDataBegin = (VertexPosCol *)mappedResource ;
			Memory< VertexPosCol > vertices = new( triangleVertices ) ;
			using var hVertices = vertices.Pin( ) ;
			Unsafe.CopyBlock( pVertexDataBegin, hVertices.Pointer, (uint)vertexBufferSize ) ;
		}
		vertexBuffer!.Unmap(0 ) ;

		// Initialize the vertex buffer view:
		vertexBufferView = new VertexBufferView {
			BufferLocation = vertexBuffer.GetGPUVirtualAddress( ),
			StrideInBytes  = VertexPosCol.SizeInBytes,
			SizeInBytes    = (uint)vertexBufferSize,
		} ;

		// Command lists are created in the recording state, but there is nothing
		// to record yet. The main loop expects it to be closed, so close it now.
		commandList.Close( ) ;
		
		// Create synchronization objects.
		GraphicsDevice.CreateFence(0, FenceFlags.None,
								   IFence.IID, out fence ) ;
		fenceValue = 1 ;
		
		// Create an event handle to use for frame synchronization.
		fenceEvent = new AutoResetEvent(false ) ;
	}
	
	public void OnRender( ) {
		// Wait for the previous frame to complete.
		WaitForPreviousFrame( ) ;
		
		// Populate command list:
		PopulateCommandList( ) ;
		
		// Execute the command list:
		//! What is this crap ???? ...
		commandQueue!.ExecuteCommandLists< IGraphicsCommandList >( 1, 
										  new IGraphicsCommandList[ ] {
											  commandList!
										  } ) ;
		
		// Present the frame:
		SwapChain!.Present( 1, 0 ) ;
		frameIndex ^= 1 ; //! Alternate between the two back buffers.
	}
	
	
	void WaitForPreviousFrame( ) {
		ulong localFence = fenceValue ;
		commandQueue!.Signal( fence, fenceValue++ ) ;
		
		// Wait until the previous frame is finished
		if ( fence!.GetCompletedValue( ) < localFence ) {
			var fenceEventHandle = fenceEvent!.SafeWaitHandle.DangerousGetHandle( ) ;
			fence.SetEventOnCompletion( localFence, fenceEventHandle ) ;
			fenceEvent.WaitOne( ) ;
		}
	}
	
	void PopulateCommandList( ) {
		// Command list allocators can only be reset when the associated 
		// command lists have finished execution on the GPU; apps should use 
		// fences to determine GPU execution progress.
		commandAllocator!.Reset( ) ;

		// However, when ExecuteCommandList() is called on a particular command list,
		// that command list can then be reset at any time and must be before re-recording.
		commandList!.Reset( commandAllocator, pipelineState ) ;


		// Set necessary state.
		commandList.SetGraphicsRootSignature( rootSignature ) ;
		commandList.RSSetViewports( 1, viewports.AsSpan( ) ) ;
		commandList.RSSetScissorRects( 1, scissorRects.AsSpan( ) ) ;

		// Indicate that the back buffer will be used as a render target:
		transitionBarriers[ 0 ] = ResourceBarrier.Transition( renderTargets[ frameIndex ], 
															  ResourceStates.Present, 
															  ResourceStates.RenderTarget ) ;
		commandList.ResourceBarrier( 1, transitionBarriers ) ;
		
		
		// Set the render target for the output merger stage:
		var rtvHandle = rtvHeap!.GetCPUDescriptorHandleForHeapStart( ) ;
		rtvHandle += frameIndex * rtvDescriptorSize ;
		commandList.OMSetRenderTargets( rtvHandle ) ;

		// Clear the render target:
		commandList.ClearRenderTargetView( rtvHandle, bufferColor ) ;

		// Draw the triangle:
		commandList.IASetPrimitiveTopology( PrimitiveTopology.D3D_TriangleList ) ;
		commandList.IASetVertexBuffers(0, 1, new[ ] { vertexBufferView } ) ;
		commandList.DrawInstanced( 3, 1, 0, 0 ) ;

		// Indicate that the back buffer will now be used to present.

		transitionBarriers[ 0 ] = ResourceBarrier.Transition( renderTargets[ frameIndex ],
															  ResourceStates.RenderTarget, 
															  ResourceStates.Present ) ;

		commandList.ResourceBarrier( 1, transitionBarriers ) ;
		
		// Done recording commands ...
		commandList.Close( ) ;
	}
	
	
	// --------------------------------------------------------------------------------------------------
	
	protected override ValueTask DisposeUnmanaged( ) {
		WaitForPreviousFrame( ) ;
		foreach ( var target in renderTargets ) {
			target?.Dispose( ) ;
		}

		try {
			foreach ( var handle in cleanupList )
				handle.Dispose( ) ;
		}
		finally { cleanupList.Clear( ) ; }

		try {
			foreach ( var disposable in _disposables )
				disposable.Dispose( ) ;
		}
		finally { _disposables.Clear( ) ; }
		
		commandAllocator?.Dispose( ) ;
		commandQueue?.Dispose( ) ;
		rootSignature?.Dispose( ) ;
		rtvHeap?.Dispose( ) ;
		pipelineState?.Dispose( ) ;
		commandList?.Dispose( ) ;
		fence?.Dispose( ) ;
		SwapChain?.Dispose( ) ;
		GraphicsDevice?.Dispose( ) ;
		
		return ValueTask.CompletedTask ;
	}

	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}
	
	// ==================================================================================================
}