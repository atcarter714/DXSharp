using System.Collections.Concurrent ;
using DXSharp ;
using DXSharp.Windows.COM ;

namespace Windows.Win32.Graphics.Direct3D12 ;

internal class COMResourceManager: DisposableObject
{
	internal COMPointerCollection _pointers = new( ) ;

	internal bool Disposed => _pointers.Pointers.IsEmpty ;
	internal bool IsObjectAlive( ) => !Disposed ;

	public void Add( Type getType, ComPtr< ID3D12Debug > comPointer ) => 
		_pointers.Add( comPointer ) ;
	
	protected override ValueTask DisposeUnmanaged( ) {
		
		
		return ValueTask.CompletedTask ;
	}
}

internal class COMPointerCollection
{
	internal ConcurrentDictionary< nint, Type > UniqueObjects { get ; } = new( ) ;
	internal ConcurrentDictionary< UID32, ComPtr > Pointers { get ; } = new( ) ;
	
	
	public bool Add( ComPtr ptr ) {
		UniqueObjects.TryAdd( ptr.BaseAddress, ptr.ComType ) ;
		var id = UID32.CreateInstanceIDFrom( ptr ) ;
		return Pointers.TryAdd( id, ptr ) ;
	}
}

internal class ComRef: DisposableObject {
	public Type ComBaseType { get ; }
	public nint UnknownPointer { get ; }
	public int TotalRefCount { get ; private set ; }
	public List< ComPtr > ComPointers { get ; } = new( ) ;
	public Dictionary< Type, nint > InterfacePtrs { get ; } = new( ) ;
	
	protected override ValueTask DisposeUnmanaged( ) {
		return ValueTask.CompletedTask;
	}

	protected ComPtr< T > GetInterface< T >( ) where T: IUnknown => null ;
}

internal class ComRefEntry: DisposableObject
{
	public nint InterfaceVTable { get ; }
	public List< ComPtr > ComPointers { get ; } = new( ) ;


	public ComPtr< T > GetPointer< T >( ) where T: IUnknown {
		
	}
	
	protected override ValueTask DisposeUnmanaged( ) {
		int length = ComPointers.Count ;
		for ( int i = 0; i < length; ++i ) {
			if( ComPointers[ i ]?.Disposed ?? true ) continue ;
			ComPointers[ i ].Dispose( ) ;
		}
		return ValueTask.CompletedTask ;
	}

	public override async ValueTask DisposeAsync( ) {
		await Task.Run( Dispose ) ;
	}
}