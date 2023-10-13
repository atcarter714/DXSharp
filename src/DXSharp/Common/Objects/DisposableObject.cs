namespace DXSharp ;


/// <summary>Abstract base class of objects containing unmanaged resources.</summary>
public abstract class DisposableObject: IDisposable, 
										IAsyncDisposable {
	public virtual bool Disposed { get ; protected set ; } = false ;
	~DisposableObject( ) => Dispose( false ) ;

	void Dispose( bool disposing ) {
		if ( disposing ) DisposeManaged( ) ;
		DisposeUnmanaged( ) ;
		this.Disposed = true ;
	}
	
	protected virtual void DisposeManaged( ) { }
	protected abstract void DisposeUnmanaged( ) ;
	
	public void Dispose( ) => Dispose( true ) ;
	public ValueTask DisposeAsync( ) => new( Task.Run( Dispose ) ) ;
} ;