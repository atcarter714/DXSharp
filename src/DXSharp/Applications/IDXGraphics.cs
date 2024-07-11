#region Using Directives
using System.Buffers ;
using System.Collections.Concurrent ;
using System.Runtime.Versioning;
using DXSharp.DXGI ;
using DXSharp.Direct3D12 ;
using DXSharp.Windows.COM ;
using IDevice = DXSharp.Direct3D12.IDevice ;
using IResource = DXSharp.Direct3D12.IResource ;
#endregion
namespace DXSharp.Applications ;


/// <summary>
/// A customizable container for storing DirectX 12 native resources
/// and their managed wrappers for initialization and cleanup ...
/// </summary>
/// <remarks>
/// An <see cref="IDXGraphics"/> object holds a set of Direct3D pipeline objects
/// (COM interface wrappers), <see cref="IDisposable"/> objects and memory handles
/// (<see cref="MemoryHandle"/>) so they can be initialized and cleaned up in a group
/// and in a specific order, while writing less, cleaner code.
/// Particularly important pipeline objects like an <see cref="IDevice"/> or <see cref="ISwapChain"/>
/// are cached for easy access via the <see cref="GraphicsDevice"/> and <see cref="SwapChain"/> properties.
/// Other properties like these exist for other important/core types.
/// </remarks>
public interface IDXGraphics: IDisposable, IAsyncDisposable {
	// ----------------------------------------------------------------------------------
	public bool Disposed => PipelineObjects is not { Count: > 0 } ;
	public ConcurrentStack< MemoryHandle > AllocatedHandles { get ; }
	public ConcurrentStack< IDXCOMObject > PipelineObjects { get ; }
	public ConcurrentStack< IDisposable > Disposables { get ; }
	
	IDevice? GraphicsDevice { get ; }
	ISwapChain? SwapChain { get ; }
	
	IRootSignature? RootSignature { get ; }
	IPipelineState? PipelineState { get ; }
	
	ICommandList? PrimaryCommandList { get ; }
	ICommandQueue? CommandQueue { get ; }
	ICommandAllocator? CommandAllocator { get ; }
	
	List< IResource >? RenderTargets { get ; }
	// ----------------------------------------------------------------------------------
	
	public TItem? Find< TItem >( ) where TItem: IDXCOMObject? {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( Disposed, typeof(IDXGraphics) ) ;
#endif
		
		foreach ( var obj in PipelineObjects ) {
			if ( obj is TItem item ) return item ;
		}
		
		return default ;
	}
	
	public TItem? Find< TItem >( string name ) where TItem: Direct3D12.IObject {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( Disposed, typeof(IDXGraphics) ) ;
#endif
		 		
		foreach ( var obj in PipelineObjects ) {
			if ( obj is TItem item ) {
				uint size = 0 ;
				item.GetPrivateData( COMUtility.WKPDID_D3DDebugObjectNameW, ref size, 0 ) ;
			}
		}
		
		return default ;
	}
	
	public void AddBackBuffer( IResource buffer ) {
		PipelineObjects.Push( buffer ) ;
		RenderTargets?.Add( buffer ) ;
	}
	public void Add( MemoryHandle handle ) => AllocatedHandles.Push( handle ) ;
	public void Add( IDisposable disposable ) => Disposables.Push( disposable ) ;
	public void Add( IDXCOMObject obj ) => PipelineObjects.Push( obj ) ;
	public void Add< T >( IUnknownWrapper< T > obj ) where T: IUnknown => PipelineObjects.Push( (obj as IDXCOMObject)! ) ;
	
	// ----------------------------------------------------------------------------------
	
	/// <summary>Loads and initializes the Direct3D graphics pipeline.</summary>
	void LoadPipeline( ) ;
	
	/// <summary>Loads and initializes application and Direct3D assets/resources.</summary>
	void LoadAssets( ) ;
	
	// ----------------------------------------------------------------------------------

	public async ValueTask ReleasePipelineObjectsAsync( ) {
		List< Task > disposalTasks = new( AllocatedHandles.Count ) ;
		
		while ( PipelineObjects.TryPop( out var obj ) ) {
			var d = obj.DisposeAsync( ) ;
			disposalTasks.Add( d.AsTask( ) ) ;
		}
		
		await Task.WhenAll( disposalTasks ) ;
	}
	
	public async ValueTask ReleaseDisposablesAsync( ) {
		Action? _destroy = default ;
		List< Task > disposalTasks = new( AllocatedHandles.Count ) ;
		
		while ( Disposables.TryPop( out var obj ) ) {
			_destroy = ( ) => { obj.Dispose( ) ; } ;
			var d = Task.Run( _destroy ) ;
			disposalTasks.Add( d ) ;
		}
		
		await Task.WhenAll( disposalTasks ) ;
	}
	
	public async ValueTask ReleaseMemoryAsync( ) {
		Action? _destroy = default ;
		List< Task > disposalTasks = new( AllocatedHandles.Count ) ;
		while ( AllocatedHandles.TryPop( out var handle ) ){
			_destroy = ( ) => { handle.Dispose( ) ; } ;
			var d = Task.Run( _destroy ) ;
			disposalTasks.Add( d ) ;
		}
		await Task.WhenAll( disposalTasks ) ;
	}
	
	// ==================================================================================
} ;



/// <summary>Helps manage and dispose of native graphics resources in an organized group.</summary>
/// <remarks>
/// You can inherit from this class to create a more customized and ideal implementation
/// for your specific application. It simply provides a starting point for organizing and
/// managing native D3D/Win32/COM and memory resources for graphics in a group.
/// </remarks>
[SupportedOSPlatform( "windows8.0" )]
public abstract class DXGraphics: DisposableObject, IDXGraphics {
	// ----------------------------------------------------------------------------------
	//! Cache fields for important pipeline objects:
	protected ISwapChain?        m_swapChain ;
	protected IDevice?           m_graphicsDevice ;
	protected ICommandAllocator? m_commandAlloc ;
	protected ICommandList?      m_commandList ;
	protected ICommandQueue?     m_commandQueue ;
	protected IRootSignature?    m_rootSignature ;
	protected IPipelineState?    m_pipelineState ;
	// ----------------------------------------------------------------------------------
	
	public ConcurrentStack< MemoryHandle > AllocatedHandles { get ; init ; }
	public ConcurrentStack< IDXCOMObject > PipelineObjects  { get ; init ; }
	public ConcurrentStack< IDisposable > Disposables { get ; init ; }
	
	// ----------------------------------------------------------------------------------
	//! Lookup properties use cached fields for fast & easy access:
	public virtual IDevice? GraphicsDevice => m_graphicsDevice ??= Find< IDevice >( ) ;
	public virtual ISwapChain? SwapChain => m_swapChain ??= Find< ISwapChain >( ) ;
	public virtual ICommandQueue? CommandQueue => m_commandQueue ??= Find< ICommandQueue >( ) ;
	public virtual ICommandList? PrimaryCommandList => m_commandList ??= Find< ICommandList >( ) ;
	public virtual ICommandAllocator? CommandAllocator => m_commandAlloc ??= Find< ICommandAllocator >( ) ;
	public virtual IRootSignature? RootSignature => m_rootSignature ??= Find< IRootSignature >( ) ;
	public virtual IPipelineState? PipelineState => m_pipelineState ??= Find< IPipelineState >( ) ;
	
	public virtual IGraphicsCommandList[ ]? GfxCommandLists { get ; set ; }
	
	// ----------------------------------------------------------------------------------
	
	public virtual ColorF BackgroundColor { get ; set ; } = Color.Black ;
	public List< IResource >? RenderTargets { get ; } = new( ) ;
	public ClearValue[ ] ClearValues { get ; set ; }
	public Viewport[ ] Viewports { get ; set ; }
	public Rect[ ] ScissorRects { get ; set ; }
	
	// ----------------------------------------------------------------------------------

	public DXGraphics( ) {
		this.AllocatedHandles = new( ) ;
		this.PipelineObjects  = new( ) ;
		this.Disposables      = new( ) ;
		
		var scissorRect = Rect.FromXYWH( 0,0,
								   (int)AppSettings.DEFAULT_WIDTH, 
								   (int)AppSettings.DEFAULT_HEIGHT ) ;
		ScissorRects = new[ ] { scissorRect } ;
		Viewports    = new[ ] { Viewport.Default } ;
		ClearValues  = new[ ] { ClearValue.TextureDefault, } ;
	}
	
	public DXGraphics( params IDXCOMObject[ ] pipelineObjects ): this( ) => PipelineObjects.PushRange( pipelineObjects ) ;
	public DXGraphics( params IDisposable[ ]  disposables ): this( ) => this.Disposables.PushRange( disposables ) ;
	public DXGraphics( params MemoryHandle[ ] handles ): this( ) => this.AllocatedHandles.PushRange( handles ) ;

	public DXGraphics( IEnumerable< IDXCOMObject >? pipelineObjects = null,
					   IEnumerable< IDisposable >?  disposables     = null,
					   IEnumerable< MemoryHandle >? handles         = null ): this( ) {
		if ( pipelineObjects is not null ) PipelineObjects.PushRange( pipelineObjects.ToArray( ) ) ;
		if ( disposables is not null ) this.Disposables.PushRange( disposables.ToArray( ) ) ;
		if ( handles is not null ) this.AllocatedHandles.PushRange( handles.ToArray( ) ) ;
	}
	
	~DXGraphics( ) => Dispose( false ) ;
	
	// ----------------------------------------------------------------------------------
	
	IDevice? _findDevice( ) {
		if ( m_graphicsDevice is not null ) 
			return m_graphicsDevice ;
			
		for ( int i = 0; i < PipelineObjects.Count; ++i ) {
			if ( PipelineObjects.TryPeek( out var obj ) ) {
				if ( obj is IDevice device ) {
					m_graphicsDevice = device ;
					return m_graphicsDevice ;
				}
			}
		}
		return null ;
	}
	
	
	public void Add< T >( IUnknownWrapper< T > obj ) where T: IUnknown {
#if DEBUG || DEBUG_COM || DEV_BUILD
			ArgumentNullException.ThrowIfNull( obj, nameof(obj) ) ;
#endif
		if ( obj is IDevice deviceWrapper )
			m_graphicsDevice = deviceWrapper ;
		else if ( obj is ISwapChain swapChainWrapper )
			m_swapChain = swapChainWrapper ;
		else if ( obj is ICommandAllocator allocatorWrapper )
			m_commandAlloc = allocatorWrapper ;
		else if ( obj is ICommandList listWrapper )
			m_commandList = listWrapper ;
		else if ( obj is ICommandQueue queueWrapper )
			m_commandQueue = queueWrapper ;
		else if ( obj is IRootSignature rootSigWrapper )
			m_rootSignature = rootSigWrapper ;
		else if ( obj is IPipelineState pipelineStateWrapper )
			m_pipelineState = pipelineStateWrapper ;
		else
			PipelineObjects.Push( (IDXCOMObject) obj ) ;
		
		PipelineObjects.Push( (IDXCOMObject) obj ) ;
	}
	
	public void Add( IDXCOMObject obj ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
			ArgumentNullException.ThrowIfNull( obj, nameof(obj) ) ;
#endif
		
		if ( obj is IDevice deviceWrapper )
			m_graphicsDevice = deviceWrapper ;
		else if ( obj is ISwapChain swapChainWrapper )
			m_swapChain = swapChainWrapper ;
		else if ( obj is ICommandAllocator allocatorWrapper )
			m_commandAlloc = allocatorWrapper ;
		else if ( obj is ICommandList listWrapper )
			m_commandList = listWrapper ;
		else if ( obj is ICommandQueue queueWrapper )
			m_commandQueue = queueWrapper ;
		else if ( obj is IRootSignature rootSigWrapper )
			m_rootSignature = rootSigWrapper ;
		else if ( obj is IPipelineState pipelineStateWrapper )
			m_pipelineState = pipelineStateWrapper ;
		
		PipelineObjects.Push( obj ) ;
	}
	
	public void Add( IDisposable disposable ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
			ArgumentNullException.ThrowIfNull( disposable, nameof(disposable) ) ;
#endif
		Disposables.Push( disposable ) ;
	}
	
	
	public TItem? Find< TItem >( ) where TItem: IDXCOMObject? {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( Disposed, typeof(IDXGraphics) ) ;
#endif
		
		foreach ( var obj in PipelineObjects ) {
			if( obj is IDevice device ) m_graphicsDevice = device ;
			else if ( obj is ISwapChain swapChain ) m_swapChain = swapChain ;
			else if ( obj is ICommandAllocator allocator ) m_commandAlloc = allocator ;
			else if ( obj is ICommandList list ) m_commandList = list ;
			else if ( obj is ICommandQueue queue ) m_commandQueue = queue ;
			else if ( obj is IRootSignature rootSig ) m_rootSignature = rootSig ;
			else if ( obj is IPipelineState pipelineState ) m_pipelineState = pipelineState ;
			
			if ( obj is TItem item ) return item ;
		}
		
		return default ;
	}
	
	public TItem? Find< TItem >( string name ) where TItem: Direct3D12.IObject {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( Disposed, typeof(IDXGraphics) ) ;
#endif
		 		
		foreach ( var obj in PipelineObjects ) {
			if( obj is IDevice device ) m_graphicsDevice                    = device ;
			else if ( obj is ISwapChain swapChain ) m_swapChain             = swapChain ;
			else if ( obj is ICommandAllocator allocator ) m_commandAlloc   = allocator ;
			else if ( obj is ICommandList list ) m_commandList              = list ;
			else if ( obj is ICommandQueue queue ) m_commandQueue           = queue ;
			else if ( obj is IRootSignature rootSig ) m_rootSignature       = rootSig ;
			else if ( obj is IPipelineState pipelineState ) m_pipelineState = pipelineState ;

			if ( obj is TItem item ) {
				uint size = 0 ;
				item.GetPrivateData( COMUtility.WKPDID_D3DDebugObjectNameW, ref size, 0 ) ;
			}
		}
		
		return default ;
	}
	
	// ----------------------------------------------------------------------------------

	public abstract void LoadPipeline( ) ;
	public abstract void LoadAssets( ) ;

	public virtual void OnRender( ) { }
	
	
	// ----------------------------------------------------------------------------------
	
	protected override async ValueTask DisposeUnmanaged( ) {
		var _base = this as IDXGraphics ;
		var task1 = _base.ReleaseMemoryAsync( ).AsTask( ) ;
		var task2 = _base.ReleaseDisposablesAsync( ).AsTask( ) ;
		var task3 = _base.ReleasePipelineObjectsAsync( ).AsTask( ) ;
		await Task.WhenAll( task1, task2, task3 ) ;
	}

	public override async ValueTask DisposeAsync( ) {
		var _t1 = base.DisposeAsync( ).AsTask( ) ;
		var _t2 = Task.Run( Dispose ) ;
		await Task.WhenAll( _t1, _t2 ) ;
	}
	
	// ==================================================================================
} ;