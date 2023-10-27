using System.Collections.Concurrent ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;
using IDevice = DXSharp.Direct3D12.IDevice ;

namespace DXSharp.Applications ;


/// <summary>
/// A customizable container for storing DirectX 12 native resources
/// and their managed wrappers for initialization and cleanup ...
/// </summary>
public interface IDXGraphics: IDisposable, IAsyncDisposable {
	ConcurrentStack< IUnknownWrapper< IUnknown > > PipelineObjects { get ; }
	
	
} ;

public class DXGraphics: DisposableObject, IDXGraphics
{
	
	protected override async ValueTask DisposeUnmanaged( ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		if ( Disposed || PipelineObjects is null )
			throw new ObjectDisposedException( $"{nameof(DXGraphics)} :: " +
											   $"No pipeline objects exist." ) ;
#endif
		CancellationToken  token         = new( ) ;
		Stack< Task > disposalTasks = new( PipelineObjects.Count ) ;
		
		while ( !PipelineObjects.IsEmpty ) {
			PipelineObjects.TryPop( out var next ) ;
			
			if ( next is not null && !next.Disposed ) {
				Action releaseAction = async ( ) => await _releaseAsync( next ) ;
				Task task = new( releaseAction, token ) ;
				
				task.Start( ) ;
				disposalTasks.Push( task ) ;
			}
		}
		
		while ( disposalTasks is not { Count: < 1 } ) {
			if ( disposalTasks.Peek( ).IsCompleted ) {
				var nextTask = disposalTasks.Pop( ) ;
				nextTask.Dispose( ) ;
			}
			else await disposalTasks.Peek( ) ;
		}
		
		static async ValueTask _releaseAsync( IAsyncDisposable disposable ) =>
			await Task.Run( disposable.DisposeAsync, new CancellationToken(  ) ) ;
	}
	
	public ConcurrentStack< IUnknownWrapper< IUnknown > > PipelineObjects { get ; }
	
	public IDevice? GraphicsDevice { get ; protected set ; }
	public ISwapChain? SwapChain { get ; protected set ; }
	
	public DXGraphics( ) => PipelineObjects = new( ) ;
	public DXGraphics( params IUnknownWrapper< IUnknown >[ ] objects ) => PipelineObjects = new( objects ) ;
	public DXGraphics( IEnumerable< IUnknownWrapper<IUnknown> > objects ) => PipelineObjects = new( objects ) ;

	public void Add< T >( IUnknownWrapper< T > obj ) where T: IUnknown {
#if DEBUG || DEBUG_COM || DEV_BUILD
			ArgumentNullException.ThrowIfNull( obj, nameof(obj) ) ;
#endif
		if ( obj is IDevice deviceWrapper )
			this.GraphicsDevice = deviceWrapper ;
		
		PipelineObjects.Push( (IUnknownWrapper< IUnknown >) obj ) ;
	}
	
}