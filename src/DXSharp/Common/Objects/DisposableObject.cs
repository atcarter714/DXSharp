namespace DXSharp ;


/// <summary>Abstract base class of objects containing unmanaged resources.</summary>
public abstract class DisposableObject: IDisposable, 
										IAsyncDisposable {
	public virtual bool Disposed { get ; protected set ; } = false ;
	~DisposableObject( ) => Dispose( false ) ;

	protected virtual void Dispose( bool disposing ) {
		if ( disposing ) DisposeManaged( ) ;
		DisposeUnmanaged( ) ;
		this.Disposed = true ;
	}
	
	protected void DisposeManaged( ) { }
	protected abstract ValueTask DisposeUnmanaged( ) ;

	public void Dispose( ) => Dispose( true ) ;

	
	public virtual ValueTask DisposeAsync( ) => new( Task.Run( Dispose ) ) ;
} ;