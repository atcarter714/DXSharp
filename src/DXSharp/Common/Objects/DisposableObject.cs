namespace DXSharp ;


/// <summary>Interface for objects containing unmanaged resources.</summary>
public interface IDisposableObject: IDisposable,
									 IAsyncDisposable {
	/// <summary>Gets a value indicating whether this object has been disposed.</summary>
	public bool Disposed { get ; }
} ;


/// <summary>Abstract base class of objects containing disposable unmanaged resources.</summary>
public abstract class DisposableObject: IDisposableObject {
	public virtual bool Disposed { get ; protected set ; } = false ;
	~DisposableObject( ) => Dispose( false ) ;
	
	
	/// <summary>Disposes of this object and its unmanaged resources.</summary>
	/// <param name="disposing">
	/// <see langword="true"/> if this method is being called from <see cref="Dispose()"/>,
	/// <see langword="false"/> if this method is being called from the finalizer.
	/// </param>
	protected virtual void Dispose( bool disposing ) {
		if ( disposing ) DisposeManaged( ) ;
		_ = DisposeUnmanaged( ) ;
		Disposed = true ;
	}
	
	/// <summary>
	/// Called when the object is being disposed of
	/// and its managed resources should be disposed.
	/// </summary>
	protected virtual void DisposeManaged( ) { }
	
	/// <summary>
	/// Called when the object is being disposed of
	/// and its unmanaged resources should be disposed.
	/// </summary>
	/// <returns>
	/// A <see cref="ValueTask"/> representing the asynchronous operation.
	/// </returns>
	protected abstract ValueTask DisposeUnmanaged( ) ;
	
	/// <summary>Disposes of this object and its unmanaged resources.</summary>
	public void Dispose( ) => Dispose( true ) ;
	/// <summary>Disposes of this object and its unmanaged resources asynchronously.</summary>
	public virtual async ValueTask DisposeAsync( ) => 
		await Task.Run( Dispose ) ;
} ;