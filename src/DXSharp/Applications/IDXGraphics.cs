#region Using Directives
using System.Buffers ;
using System.Collections.Concurrent ;

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
	
	ICommandList? CommandList { get ; }
	ICommandQueue? CommandQueue { get ; }
	ICommandAllocator? CommandAllocator { get ; }
	
	List< IResource >? BackBuffers { get ; }
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
		BackBuffers?.Add( buffer ) ;
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
public abstract class DXGraphics: DisposableObject, IDXGraphics {
	// ----------------------------------------------------------------------------------
	//! Cache fields for important pipeline objects:
	ISwapChain?        _swapChain ;
	IDevice?           _graphicsDevice ;
	ICommandAllocator? _commandAlloc ;
	ICommandList?      _commandList ;
	ICommandQueue?     _commandQueue ;
	IRootSignature?    _rootSignature ;
	IPipelineState?    _pipelineState ;
	// ----------------------------------------------------------------------------------
	
	public ConcurrentStack< MemoryHandle > AllocatedHandles { get ; init ; }
	public ConcurrentStack< IDXCOMObject > PipelineObjects  { get ; init ; }
	public ConcurrentStack< IDisposable > Disposables { get ; init ; }
	
	// ----------------------------------------------------------------------------------
	//! Lookup properties use cached fields for fast & easy access:
	public IDevice? GraphicsDevice => _graphicsDevice ??= Find< IDevice >( ) ;
	public ISwapChain? SwapChain => _swapChain ??= Find< ISwapChain >( ) ;
	public ICommandList? CommandList => _commandList ??= Find< ICommandList >( ) ;
	public ICommandQueue? CommandQueue => _commandQueue ??= Find< ICommandQueue >( ) ;
	public ICommandAllocator? CommandAllocator => _commandAlloc ??= Find< ICommandAllocator >( ) ;
	public IRootSignature? RootSignature => _rootSignature ??= Find< IRootSignature >( ) ;
	public IPipelineState? PipelineState => _pipelineState ??= Find< IPipelineState >( ) ;
	// ----------------------------------------------------------------------------------
	
	public virtual ColorF BackgroundColor { get ; set ; } = Color.Black ;
	public List< IResource >? BackBuffers { get ; } = new( ) ;
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
		if ( _graphicsDevice is not null ) 
			return _graphicsDevice ;
			
		for ( int i = 0; i < PipelineObjects.Count; ++i ) {
			if ( PipelineObjects.TryPeek( out var obj ) ) {
				if ( obj is IDevice device ) {
					_graphicsDevice = device ;
					return _graphicsDevice ;
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
			_graphicsDevice = deviceWrapper ;
		else if ( obj is ISwapChain swapChainWrapper )
			_swapChain = swapChainWrapper ;
		else if ( obj is ICommandAllocator allocatorWrapper )
			_commandAlloc = allocatorWrapper ;
		else if ( obj is ICommandList listWrapper )
			_commandList = listWrapper ;
		else if ( obj is ICommandQueue queueWrapper )
			_commandQueue = queueWrapper ;
		else if ( obj is IRootSignature rootSigWrapper )
			_rootSignature = rootSigWrapper ;
		else if ( obj is IPipelineState pipelineStateWrapper )
			_pipelineState = pipelineStateWrapper ;
		else
			PipelineObjects.Push( (IDXCOMObject) obj ) ;
		
		PipelineObjects.Push( (IDXCOMObject) obj ) ;
	}
	
	public void Add( IDXCOMObject obj ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
			ArgumentNullException.ThrowIfNull( obj, nameof(obj) ) ;
#endif
		
		if ( obj is IDevice deviceWrapper )
			_graphicsDevice = deviceWrapper ;
		else if ( obj is ISwapChain swapChainWrapper )
			_swapChain = swapChainWrapper ;
		else if ( obj is ICommandAllocator allocatorWrapper )
			_commandAlloc = allocatorWrapper ;
		else if ( obj is ICommandList listWrapper )
			_commandList = listWrapper ;
		else if ( obj is ICommandQueue queueWrapper )
			_commandQueue = queueWrapper ;
		else if ( obj is IRootSignature rootSigWrapper )
			_rootSignature = rootSigWrapper ;
		else if ( obj is IPipelineState pipelineStateWrapper )
			_pipelineState = pipelineStateWrapper ;
		
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
			if( obj is IDevice device ) _graphicsDevice = device ;
			else if ( obj is ISwapChain swapChain ) _swapChain = swapChain ;
			else if ( obj is ICommandAllocator allocator ) _commandAlloc = allocator ;
			else if ( obj is ICommandList list ) _commandList = list ;
			else if ( obj is ICommandQueue queue ) _commandQueue = queue ;
			else if ( obj is IRootSignature rootSig ) _rootSignature = rootSig ;
			else if ( obj is IPipelineState pipelineState ) _pipelineState = pipelineState ;
			
			if ( obj is TItem item ) return item ;
		}
		
		return default ;
	}
	
	public TItem? Find< TItem >( string name ) where TItem: Direct3D12.IObject {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( Disposed, typeof(IDXGraphics) ) ;
#endif
		 		
		foreach ( var obj in PipelineObjects ) {
			if( obj is IDevice device ) _graphicsDevice                    = device ;
			else if ( obj is ISwapChain swapChain ) _swapChain             = swapChain ;
			else if ( obj is ICommandAllocator allocator ) _commandAlloc   = allocator ;
			else if ( obj is ICommandList list ) _commandList              = list ;
			else if ( obj is ICommandQueue queue ) _commandQueue           = queue ;
			else if ( obj is IRootSignature rootSig ) _rootSignature       = rootSig ;
			else if ( obj is IPipelineState pipelineState ) _pipelineState = pipelineState ;

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
}