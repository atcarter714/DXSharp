using System.Numerics ;
using System.Runtime.CompilerServices ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using AdvancedDXS.Framework.Scenes ;
using DXSharp ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;
using DXSharp.XTensions ;
using IDevice = DXSharp.Direct3D12.IDevice ;
using IResource = DXSharp.Direct3D12.IResource ;
using Range = DXSharp.Direct3D12.Range ;

namespace AdvancedDXS.Framework.Graphics ;

/* Native Class Definition:
 
class FrameResource
{
public:
    ICommandList* m_batchSubmit[NumContexts * 2 + CommandListCount];

    ComPtr< ICommandAllocator > m_commandAllocators[CommandListCount];
    ComPtr< IGraphicsCommandList > m_commandLists[CommandListCount];

    ComPtr< ICommandAllocator > m_shadowCommandAllocators[NumContexts];
    ComPtr< IGraphicsCommandList > m_shadowCommandLists[NumContexts];

    ComPtr< ICommandAllocator > m_sceneCommandAllocators[NumContexts];
    ComPtr< IGraphicsCommandList > m_sceneCommandLists[NumContexts];

    UINT64 m_fenceValue;

private:
    ComPtr< IPipelineState > m_pipelineState;
    ComPtr< IPipelineState > m_pipelineStateShadowMap;

	ComPtr< IResource > m_shadowTexture;
    ComPtr< IResource > m_shadowConstantBuffer;
    D3D12_CPU_DESCRIPTOR_HANDLE m_shadowDepthView;

    ComPtr< IResource > m_sceneConstantBuffer;
	SceneConstantBuffer* mp_shadowConstantBufferWO; // WRITE-ONLY pointer to the shadow pass constant buffer.
    SceneConstantBuffer* mp_sceneConstantBufferWO;  // WRITE-ONLY pointer to the scene pass constant buffer.

	GPUDescriptorHandle m_nullSrvHandle;    // Null SRV for out of bounds behavior.
    GPUDescriptorHandle m_shadowDepthHandle;
    GPUDescriptorHandle m_shadowCbvHandle;
    GPUDescriptorHandle m_sceneCbvHandle;


public:
    FrameResource( IDevice* pDevice,
                   IPipelineState* pPso,
                   IPipelineState* pShadowMapPso,
                   IDescriptorHeap* pDsvHeap,
                   IDescriptorHeap* pCbvSrvHeap,
                   D3D12_VIEWPORT* pViewport,
                   UINT frameResourceIndex ) ;
    ~FrameResource( ) ;


    void Init( ) ;
    void Finish( ) ;
    void SwapBarriers( ) ;


    void Bind( IGraphicsCommandList* pCommandList, BOOL scenePass,
               D3D12_CPU_DESCRIPTOR_HANDLE* pRtvHandle, 
               D3D12_CPU_DESCRIPTOR_HANDLE* pDsvHandle ) ;


    void WriteConstantBuffers( D3D12_VIEWPORT* pViewport,
                               Camera* pSceneCamera,
                               Camera lightCams[ NumLights ],
                               LightState lights[ NumLights ] ) ;
} ;
 */


public class FrameResource: DisposableObject {
	// private fields:
	IPipelineState m_pipelineState ;
	IPipelineState m_pipelineStateShadowMap ;

	IResource    m_shadowTexture ;
	IResource    m_shadowConstantBuffer ;
	CPUDescriptorHandle m_shadowDepthView ;

	IResource m_sceneConstantBuffer ;
	unsafe SceneConstBuffer* mp_shadowConstantBufferWO ; // WRITE-ONLY pointer to the shadow pass constant buffer.
	unsafe SceneConstBuffer* mp_sceneConstantBufferWO ;  // WRITE-ONLY pointer to the scene pass constant buffer.
	
	GPUDescriptorHandle m_nullSrvHandle ; // Null SRV for out of bounds behavior.
	GPUDescriptorHandle m_shadowDepthHandle ;
	GPUDescriptorHandle m_shadowCbvHandle ;
	GPUDescriptorHandle m_sceneCbvHandle ;
	
	// public fields:
	public ICommandList[ ]         m_batchSubmit ;
	public ICommandAllocator[ ]    m_commandAllocators ;
	public IGraphicsCommandList[ ] m_commandLists ;
	public ICommandAllocator[ ]    m_shadowCommandAllocators ;
	public IGraphicsCommandList[ ] m_shadowCommandLists ;
	public ICommandAllocator[ ]    m_sceneCommandAllocators ;
	public IGraphicsCommandList[ ] m_sceneCommandLists ;
	public ulong m_fenceValue ;
	
	public int NumContexts => GraphicsPipeline.NumContexts ;
	public int CommandListCount => GraphicsPipeline.CommandListCount ;
	
	
	public unsafe FrameResource( IDevice pDevice,
								 IPipelineState  pPso,
								 IPipelineState  pShadowMapPso,
								 IDescriptorHeap pDsvHeap,
								 IDescriptorHeap pCbvSrvHeap,
								 in Viewport     pViewport,
								 uint frameResourceIndex ) {
		int batchSubmitCount = NumContexts * 2 + CommandListCount ;
		m_batchSubmit = new ICommandList[ batchSubmitCount ] ;
		m_commandAllocators = new ICommandAllocator[ CommandListCount ] ;
		m_commandLists = new IGraphicsCommandList[ CommandListCount ] ;
		m_shadowCommandAllocators = new ICommandAllocator[ NumContexts ] ;
		m_shadowCommandLists = new IGraphicsCommandList[ NumContexts ] ;
		m_sceneCommandAllocators = new ICommandAllocator[ NumContexts ] ;
		m_sceneCommandLists = new IGraphicsCommandList[ NumContexts ] ;
		
		// Create the command allocators and lists for the main thread.
		for ( int i = 0; i < CommandListCount; ++i ) {
			pDevice.CreateCommandAllocator( CommandListType.Direct, 
											ICommandAllocator.IID, 
											out m_commandAllocators[ i ] ) ;
			
			pDevice.CreateCommandList(0, CommandListType.Direct,
									  m_commandAllocators[i], pPso,
									  IGraphicsCommandList9.IID,
									  out var _nextCmdList ) ;
			
			var list9 = (IGraphicsCommandList9)_nextCmdList ;
			m_commandLists[ i ] = list9 ;
			list9.Close( ) ;
		}
		
		// Create the command allocators and lists for the worker threads.
		for ( int i = 0; i < NumContexts; ++i ) {
			// For the shadow map:
			pDevice.CreateCommandAllocator( CommandListType.Direct, 
											ICommandAllocator.IID, 
											out m_shadowCommandAllocators[ i ] ) ;
			
			pDevice.CreateCommandList(0, CommandListType.Direct,
									  m_shadowCommandAllocators[i], pShadowMapPso,
									  IGraphicsCommandList9.IID,
									  out var _nextCmdList ) ;
			
			var list9 = (IGraphicsCommandList9)_nextCmdList ;
			m_shadowCommandLists[ i ] = list9 ;
			list9.Close( ) ;
			
			// For the scene pass:
			pDevice.CreateCommandAllocator( CommandListType.Direct, 
											ICommandAllocator.IID, 
											out m_sceneCommandAllocators[ i ] ) ;
			
			pDevice.CreateCommandList(0, CommandListType.Direct,
									  m_sceneCommandAllocators[i], pPso,
									  IGraphicsCommandList9.IID,
									  out _nextCmdList ) ;
			
			list9 = (IGraphicsCommandList9)_nextCmdList ;
			m_sceneCommandLists[ i ] = list9 ;
			list9.Close( ) ;
		}
		
		// Create the shadow map texture:
		var shadowTexDesc = new ResourceDescription {
			Dimension        = ResourceDimension.Texture2D,
			Alignment        = 0,
			Width            = (uint)pViewport.Width,
			Height           = (uint)pViewport.Height,
			DepthOrArraySize = 1,
			MipLevels        = 1,
			Format           = Format.R32_TYPELESS,
			SampleDesc       = SampleDescription.Default,
			Layout           = TextureLayout.Unknown,
			Flags            = ResourceFlags.AllowDepthStencil,
		} ;

		ClearValue clearValue = new( Format.D32_FLOAT, DepthStencilValue.Default ) ;
		pDevice.CreateCommittedResource( HeapProperties.Default, HeapFlags.None, shadowTexDesc,
										 ResourceStates.DepthWrite,
										 clearValue, IResource2.IID,
										 out m_shadowTexture! ) ;
		m_shadowTexture.SetName( "Shadow Map Texture" ) ;
		
		// Get a handle to the start of the descriptor heap then offset 
		// it based on the frame resource index.
		var dsvHandle = pDsvHeap.GetCPUDescriptorHandleForHeapStart( ) ;
		uint dsvDescriptorSize = pDevice.GetDescriptorHandleIncrementSize( DescriptorHeapType.DSV ) ;
		CPUDescriptorHandle depthHandle = dsvHandle + (1 + frameResourceIndex) * dsvDescriptorSize ;
		
		// Describe and create the shadow depth view and cache the CPU descriptor handle:
		var depthStencilViewDesc = new DepthStencilViewDesc( Format.D32_FLOAT, DSVFlags.None,
															 new Tex2DDSV( 0 ) ) ;
		pDevice.CreateDepthStencilView( m_shadowTexture, depthStencilViewDesc, depthHandle ) ;
		m_shadowDepthView = depthHandle ;
		
		// Get a handle to the start of the descriptor heap then offset it 
		// based on the existing textures and the frame resource index. Each 
		// frame has 1 SRV (shadow tex) and 2 CBVs.
		const uint nullSrvCount = 2 ;                              // Null descriptors at the start of the heap.
		int textureCount = AssetManager.Textures?.Length ?? 0 ;    // Diffuse + normal textures near the start of the heap.  Ideally, track descriptor heap contents/offsets at a higher level.
		uint cbvSrvDescriptorSize = pDevice.GetDescriptorHandleIncrementSize( DescriptorHeapType.CBV_SRV_UAV ) ;
		
		var cbvSrvCpuHandle = pCbvSrvHeap.GetCPUDescriptorHandleForHeapStart( ) ;
		var cbvSrvGpuHandle = pCbvSrvHeap.GetGPUDescriptorHandleForHeapStart( ) ;
		m_nullSrvHandle = cbvSrvGpuHandle ;
		cbvSrvCpuHandle.Offset( (int)( nullSrvCount + textureCount +
									   ( frameResourceIndex * GraphicsPipeline.FrameCount ) ), cbvSrvDescriptorSize ) ;
		cbvSrvGpuHandle.Offset( (int)( nullSrvCount + textureCount +
									   ( frameResourceIndex * GraphicsPipeline.FrameCount ) ), cbvSrvDescriptorSize ) ;
		
		
		// Describe and create a shader resource view (SRV) for the shadow depth 
		// texture and cache the GPU descriptor handle. This SRV is for sampling 
		// the shadow map from our shader. It uses the same texture that we use 
		// as a depth-stencil during the shadow pass.
		var shadowSrvDesc = new ShaderResourceViewDescription( Format.R32_FLOAT, 
															   ShaderComponentMapping.Default4ComponentMapping, 
															   new Tex2DSRV( 0, 1 ) ) ;
		pDevice.CreateShaderResourceView( m_shadowTexture, shadowSrvDesc, cbvSrvCpuHandle ) ;
		m_shadowDepthHandle = cbvSrvGpuHandle ;
		
		// Increment the descriptor handles:
		cbvSrvCpuHandle.Offset( cbvSrvDescriptorSize ) ;
		cbvSrvGpuHandle.Offset( cbvSrvDescriptorSize ) ;
		
		uint constantBufferSize = ( SceneConstBuffer.SizeInBytes + 
									(DXSharpUtils.D3D12_CONSTANT_BUFFER_DATA_PLACEMENT_ALIGNMENT - 1)) 
								  & ~(DXSharpUtils.D3D12_CONSTANT_BUFFER_DATA_PLACEMENT_ALIGNMENT - 1) ; //! must be a multiple 256 bytes
		
		pDevice.CreateCommittedResource( new(HeapType.Upload), HeapFlags.None,
										 ResourceDescription.Buffer(constantBufferSize),
										 ResourceStates.GenericRead, null,
										 IResource2.IID, out m_shadowConstantBuffer! ) ;
		
		pDevice.CreateCommittedResource( new(HeapType.Upload), HeapFlags.None,
										 ResourceDescription.Buffer(constantBufferSize),
										 ResourceStates.GenericRead, null,
										 IResource2.IID, out m_sceneConstantBuffer! ) ;
		
		// Map the constant buffers and cache their heap pointers:
		Range readRange = ( 0, 0 ) ;
		m_shadowConstantBuffer.Map( 0, readRange, out nint shadowCBAddr ) ;
		mp_shadowConstantBufferWO = (SceneConstBuffer *)shadowCBAddr ;
		m_sceneConstantBuffer.Map( 0, readRange, out nint sceneCBAddr ) ;
		mp_sceneConstantBufferWO = (SceneConstBuffer *)sceneCBAddr ;
		
		// Create the constant buffer views: one for the shadow pass and another for the scene pass:
		ConstBufferViewDescription cbvDesc = 
			new( m_shadowConstantBuffer.GetGPUVirtualAddress( ), constantBufferSize ) ;
		
		pDevice.CreateConstantBufferView( cbvDesc, cbvSrvCpuHandle ) ;
		m_shadowCbvHandle = cbvSrvGpuHandle ;
		
		// Increment the descriptor handles.
		cbvSrvCpuHandle.Offset( cbvSrvDescriptorSize ) ;
		cbvSrvGpuHandle.Offset( cbvSrvDescriptorSize ) ;
		
		// Describe and create the scene constant buffer view (CBV) and 
		// cache the GPU descriptor handle.
		cbvDesc.BufferLocation = m_sceneConstantBuffer.GetGPUVirtualAddress( ) ;
		pDevice.CreateConstantBufferView( cbvDesc, cbvSrvCpuHandle ) ;
		m_sceneCbvHandle = cbvSrvGpuHandle ;
		
		//! Batch up command lists for execution later:
		// *****************************************************************
		int offset = 1 ;
		uint batchSize = (uint)( m_sceneCommandLists.Length
								 + m_shadowCommandLists.Length + 3 ) ;
		
		// Copy shadow command lists:
		m_shadowCommandLists.CopyTo( m_batchSubmit, offset ) ;
		offset += m_shadowCommandLists.Length ;
		
		// Set mid command list:
		m_batchSubmit[ offset++ ] = 
			m_commandLists[ GraphicsPipeline.CommandListMid ] ;

		// Copy scene command lists:
		m_sceneCommandLists.CopyTo( m_batchSubmit, offset ) ;

		// Set post command list:
		m_batchSubmit[ batchSize - 1 ] = 
			m_commandLists[ GraphicsPipeline.CommandListPost ] ;
		// *****************************************************************
	}
	
	~FrameResource( ) => Dispose( false ) ;
	protected override async ValueTask DisposeUnmanaged( ) {
		var _killTasks = new Task[ 2 ] ;
		List< Task > destroyTasks = new( m_commandLists.Length ) ;
		
		for ( int i = 0; i < CommandListCount; ++i ) {
			var _destroyTask0 = m_commandAllocators[ i ].DisposeAsync( ) ;
			var _destroyTask1 = m_commandLists[ i ].DisposeAsync( ) ;
			_killTasks[ 0 ] = _destroyTask0.AsTask( ) ;
			_killTasks[ 1 ] = _destroyTask1.AsTask( ) ;
			destroyTasks.AddRange( _killTasks ) ;
		}
		await Task.WhenAll( destroyTasks ) ;
		destroyTasks.Clear( ) ;

		var _destroyShadowCB = m_shadowConstantBuffer.DisposeAsync( ).AsTask( ) ;
		var _destroySceneCB  = m_sceneConstantBuffer.DisposeAsync( ).AsTask( ) ;
		destroyTasks.Add( _destroyShadowCB ) ;
		destroyTasks.Add( _destroySceneCB ) ;
		Array.Resize( ref _killTasks, 4 ) ;
		for ( int i = 0; i < NumContexts; ++i ) {
			var _destroyTask0 = m_shadowCommandAllocators[ i ].DisposeAsync( ) ;
			var _destroyTask1 = m_shadowCommandLists[ i ].DisposeAsync( ) ;
			var _destroyTask2 = m_sceneCommandAllocators[ i ].DisposeAsync( ) ;
			var _destroyTask3 = m_sceneCommandLists[ i ].DisposeAsync( ) ;
			_killTasks[ 0 ] = _destroyTask0.AsTask( ) ;
			_killTasks[ 1 ] = _destroyTask1.AsTask( ) ;
			_killTasks[ 2 ] = _destroyTask2.AsTask( ) ;
			_killTasks[ 3 ] = _destroyTask3.AsTask( ) ;
			destroyTasks.AddRange( _killTasks ) ;
		}
		var _destroyShadowTex = m_shadowTexture.DisposeAsync( ).AsTask( ) ;
		destroyTasks.Add( _destroyShadowTex ) ;
		
		await Task.WhenAll( destroyTasks ) ;
		destroyTasks.Clear( ) ;
	}
	

	void Init( ) {
		// Reset the command allocators and lists for the main thread:
		for ( int i = 0; i < CommandListCount; ++i ) {
			m_commandAllocators[ i ].Reset( ) ;
			m_commandLists[ i ].Reset( m_commandAllocators[ i ], m_pipelineState ) ;
		}
		
		// Clear the depth stencil buffer in preparation for rendering the shadow map.
		m_commandLists[ GraphicsPipeline.CommandListPre ]
			.ClearDepthStencilView( m_shadowDepthView, ClearFlags.Depth, 
									1.0f, 0 ) ;

		// Reset the worker command allocators and lists.
		for ( int i = 0; i < NumContexts; ++i ) {
			// Reset shadow allocator & list:
			m_shadowCommandAllocators[ i ]
				.Reset( ) ;
			m_shadowCommandLists[ i ]
				.Reset( m_shadowCommandAllocators[i], 
						m_pipelineStateShadowMap ) ;
			
			// Reset scene allocator & list:
			m_sceneCommandAllocators[ i ]
				.Reset( ) ;
			m_sceneCommandLists[ i ]
				.Reset( m_sceneCommandAllocators[i], 
						m_pipelineState ) ;
		}
	}

	void Finish( ) {
		m_commandLists[ GraphicsPipeline.CommandListPost ]
			.ResourceBarrier( ResourceBarrier.Transition( m_shadowTexture, 
														  ResourceStates.PixelShaderResource, 
														  ResourceStates.DepthWrite ) ) ;
	}
	void SwapBarriers( ) { 
		// Transition the shadow map from writeable to readable:
		m_commandLists[ GraphicsPipeline.CommandListMid ]
			.ResourceBarrier( ResourceBarrier.Transition( m_shadowTexture,
														  ResourceStates.DepthWrite,
														  ResourceStates.PixelShaderResource ) ) ;
	}


	void Bind( IGraphicsCommandList   pCommandList, bool scenePass,
			   in CPUDescriptorHandle pRtvHandle,
			   in CPUDescriptorHandle pDsvHandle ) {
		if ( scenePass ) {
			// Scene pass. We use constant buf #2 and depth stencil #2
			// with rendering to the render target enabled:
			pCommandList.SetGraphicsRootDescriptorTable( 2, m_shadowDepthHandle ) ; // Set the shadow texture as an SRV.
			pCommandList.SetGraphicsRootDescriptorTable( 1, m_sceneCbvHandle ) ;
        
			if( pRtvHandle == CPUDescriptorHandle.Null ) throw new ArgumentNullException(nameof(pRtvHandle)) ;
			if( pDsvHandle == CPUDescriptorHandle.Null ) throw new ArgumentNullException(nameof(pDsvHandle)) ;
			
			pCommandList.OMSetRenderTargets( pRtvHandle, false, pDsvHandle ) ;
		}
		else {
			// Shadow pass. We use constant buf #1 and depth stencil #1
			// with rendering to the render target disabled.
			pCommandList.SetGraphicsRootDescriptorTable( 2, m_nullSrvHandle ) ; // Set a null SRV for the shadow texture.
			pCommandList.SetGraphicsRootDescriptorTable( 1, m_shadowCbvHandle ) ;

			pCommandList.OMSetRenderTargets(0, null,
											 false, m_shadowDepthView ) ; // No render target needed for the shadow pass.
		}
	}


	unsafe void WriteConstantBuffers( in Viewport pViewport,
									  Camera pSceneCamera,
									  Camera[ ] lightCams,
									  LightState[ ] lights ) {
		SceneConstBuffer sceneConsts = new( ),
						 shadowConsts = new( ) ;
		
		// Scale down the world a bit.
		sceneConsts.Model = Matrix4x4.CreateScale( 0.1f, 0.1f, 0.1f ) ;
		shadowConsts.Model = Matrix4x4.CreateScale( 0.1f, 0.1f, 0.1f ) ;
		
		// The scene pass is drawn from the camera.
		pSceneCamera.Get3DViewProjMatrices( out sceneConsts.View, out sceneConsts.Projection,
										   90.0f, pViewport.Width, pViewport.Height ) ;
		
		// The light pass is drawn from the first light.
		lightCams[ 0 ].Get3DViewProjMatrices( out shadowConsts.View, out shadowConsts.Projection,
											  90.0f, pViewport.Width, pViewport.Height ) ;
		
		for ( int i = 0; i < SceneConstBuffer.LightCount; i++ ) {
			shadowConsts.LightStates[ i ] = lights[ i ] ;
			sceneConsts.LightStates[ i ]  = lights[ i ] ;
		}
		
		// The shadow pass won't sample the shadow map, but rather write to it.
		shadowConsts.SampleShadowMap = false ;
		
		// The scene pass samples the shadow map.
		sceneConsts.SampleShadowMap = true ;
		
		shadowConsts.AmbientColor = sceneConsts.AmbientColor = new( 0.1f, 0.2f, 0.3f, 1.0f ) ;
		
		// Original C++ code used memcpy, like so:
		// memcpy(mp_sceneConstantBufferWO, &sceneConsts, sizeof(SceneConstantBuffer));
		// memcpy(mp_shadowConstantBufferWO, &shadowConsts, sizeof(SceneConstantBuffer));
		
		// We can use Unsafe.Write or manually write to the memory location:
		Unsafe.Write( mp_sceneConstantBufferWO, sceneConsts ) ;
		Unsafe.Write( mp_shadowConstantBufferWO, shadowConsts ) ;
		//*mp_sceneConstantBufferWO  = sceneConsts ;
		//*mp_shadowConstantBufferWO = shadowConsts ;
	}
	
} ;




//! Code snippets for constructing `m_batchSubmit`:
//! Initial Version:
/*
// Copy shadow command lists:
		Array.Copy( m_shadowCommandLists, 0,
					m_batchSubmit, offset,
					m_shadowCommandLists.Length ) ;
		offset += m_shadowCommandLists.Length ;
		
		// Set mid command list:
		m_batchSubmit[ offset++ ] =
			m_commandLists[ GraphicsPipeline.CommandListMid ] ;
		
		// Copy scene command lists:
		Array.Copy( m_sceneCommandLists, 0,
					m_batchSubmit, offset,
					m_sceneCommandLists.Length ) ;
		
		// Set post command list:
		m_batchSubmit[ batchSize - 1 ] =
			m_commandLists[ GraphicsPipeline.CommandListPost ] ;
*/

//! Alternate version 2:
		/*List< ICommandList> _batchSubmit = new( (int)batchSize ) ;
		_batchSubmit.AddRange( m_shadowCommandLists ) ;
		_batchSubmit.Add( m_commandLists[ GraphicsPipeline.CommandListMid ] ) ;
		_batchSubmit.AddRange( m_sceneCommandLists ) ;
		_batchSubmit.Add( m_commandLists[ GraphicsPipeline.CommandListPost ] ) ;
		m_batchSubmit = _batchSubmit.ToArray( ) ;*/

/*var depthDesc = new DepthStencilViewDesc {
 			Format        = Format.D32_FLOAT,
 			ViewDimension = DSVDimension.Texture2D,
 			Flags         = DSVFlags.None,
 			TexInfo       = new Tex2DDSV(0),
 		} ;
 		
uint batchSize = (uint)( m_sceneCommandLists.Length
								 + m_shadowCommandLists.Length + 3 ) ;
		
		m_batchSubmit[ 0 ] = m_commandLists[ GraphicsPipeline.CommandListPre ] ;
		memcpy( m_batchSubmit + 1, m_shadowCommandLists,
				m_shadowCommandLists.Length * sizeof(nint) ) ;
		
		m_batchSubmit[ m_shadowCommandLists.Length + 1 ] = 
			m_commandLists[ GraphicsPipeline.CommandListMid ] ;
		
		memcpy( m_batchSubmit + m_shadowCommandLists.Length + 2,
				m_sceneCommandLists,
				m_sceneCommandLists.Length * sizeof(nint) ) ;
		
		m_batchSubmit[ batchSize - 1 ] = m_commandLists[ GraphicsPipeline.CommandListPost ] ;
		
 		
*/