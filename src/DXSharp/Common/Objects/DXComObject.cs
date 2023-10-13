using DXSharp.Windows.COM ;
namespace DXSharp.Objects ;


public abstract class DXComObject: DisposableObject, IDXCOMObject {
	protected override void DisposeUnmanaged( ) => ComPtrBase?.Dispose( ) ;

	public abstract ComPtr? ComPtrBase { get ; }
	public int RefCount => (int)( ComPtrBase?.RefCount ?? 0 ) ;
} ;


#region IDisposable
/*~DXComObject( ) { Dispose( false ) ; }
public bool Disposed => ComPtrBase?.Disposed ?? true ;
void Dispose( bool disposing ) {
	ComPtrBase?.Dispose( ) ;

	if ( disposing )
		DisposeManaged( ) ;

	DisposeUnmanaged( ) ;
}
protected virtual void DisposeManaged( )   { }
protected virtual void DisposeUnmanaged( ) { }
public void Dispose( ) => Dispose( true ) ;
public ValueTask DisposeAsync( ) => new( Task.Run( Dispose ) ) ;*/
#endregion
