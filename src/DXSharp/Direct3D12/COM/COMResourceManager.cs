
#region Using Directives
using System.Collections.Concurrent ;
using System.Diagnostics ;
using static DXSharp.Windows.COM.COMUtility ;
#endregion
namespace DXSharp.Windows.COM ;


internal record ComInterfaceRef( Type InterfaceType,
								 nint VTablePtr,
								 IUnknown InterfaceRCW ) ;

internal record ComInterfaceRef< T >( nint VTablePtr,
									  T    InterfaceObject ):
	ComInterfaceRef( typeof(T), VTablePtr, InterfaceObject ) where T: IUnknown {
	public T InterfaceObject { get ; init ; } = InterfaceObject ;
} ;


internal class ComObject: DisposableObject {
	nint _pUnknown ;
	Type _initialType ;
	ConcurrentDictionary< nint, int > _refCounts = new( ) ;
	ConcurrentDictionary< ComInterfaceRef, ComPtr > _pointers  = new( ) ;
	public bool IsAlive => _pointers.Count > 0 ;
	public override bool Disposed { get ; protected set ; }

	public ComObject( ComPtr ptr ) {
		ComInterfaceRef ptrRef = new( ptr.ComType,
									  ptr.BaseAddress,
									  (IUnknown)ptr.InterfaceObjectRef ) ;

		//_interfaces.Add( ptrRef ) ;
		_pointers.TryAdd( ptrRef, ptr ) ;

		_initialType = ptr.ComType ;
		_pUnknown    = ptr.BaseAddress ;
	}


	public void AddPointer< T >( ComPtr< T > ptr ) where T: IUnknown {
		ComInterfaceRef<T> ptrRef = new( ptr.InterfaceVPtr, ptr.Interface! ) ;

		//_interfaces.Add( ptrRef ) ;
		_pointers.TryAdd( ptrRef, ptr ) ;
		_refCounts.AddOrUpdate( ptr.InterfaceVPtr,
								( n ) => 1, ( n, c ) => c + 1 ) ;
	}

	public ComPtr< T > GetPointer< T >( ) where T: IUnknown {
#if DEBUG || DEBUG_COM || DEV_BUILD
		if ( !IsAlive ) throw new ObjectDisposedException( nameof( ComObject ) ) ;
#endif
		foreach ( var entry in _pointers ) {
			if ( entry.Value.ComType == typeof( T ) )
				return (ComPtr< T >)entry.Value ;
		}

		var newPtr = _initNewPointer< T >( ) ;
		if ( newPtr is not null ) return newPtr ;

		// At this point, we must query the COM object for the interface:
		var hr = QueryInterface< T >( _pUnknown, out nint pInterface ) ;
#if DEBUG || DEBUG_COM || DEV_BUILD
		hr.ThrowOnFailure( ) ;
		if ( !pInterface.IsValid( ) )
			throw new DirectXComError( $"No such COM interface \"{typeof( T ).Name}\" " +
									   $"supported by this object!" ) ;
#endif
		ComPtr< T >          ptr    = new( pInterface ) ;
		ComInterfaceRef< T > ptrRef = new( pInterface, ptr.Interface! ) ;

		//_interfaces.Add( ptrRef ) ;
		_pointers.TryAdd( ptrRef, ptr ) ;
		_refCounts.AddOrUpdate( ptr.InterfaceVPtr,
								( n ) => 1, ( n, c ) => c + 1 ) ;
		return ptr ;
	}

	ComPtr< T >? _initNewPointer< T >( ) where T: IUnknown {
		var _ptrType = typeof( T ) ;
		foreach ( var entry in _pointers ) {
			if ( _ptrType.IsAssignableFrom( entry.Value.ComType ) ) {
				if ( entry.Value.InterfaceObjectRef is T _interface ) {
					ComPtr< T >          newPtr = new( _interface ) ;
					ComInterfaceRef< T > newRef = new( entry.Key.VTablePtr, _interface ) ;
					_pointers.TryAdd( newRef, newPtr ) ;

					_refCounts.AddOrUpdate( newPtr.InterfaceVPtr,
											( n ) => 1, ( n, c ) => c + 1 ) ;
					return newPtr ;
				}
			}
		}

		return null ;
	}
	
	public void Destroy( ) {
		foreach ( var entry in _pointers ) {
			if ( entry.Value is not { Disposed: false } ) continue ;
			entry.Value.Dispose( ) ;
		}
		
		_pointers.Clear( ) ;
		Disposed = true ;

#if DEBUG || DEBUG_COM || DEV_BUILD
		Debug.Write( "ComObject Destroyed: " ) ;
		foreach ( var counter in _refCounts ) {
			Debug.Write( $" {counter.Key:X} - {counter.Value}\n" ) ;
		}
#endif
	}

	protected override ValueTask DisposeUnmanaged( ) {
		Destroy( ) ;
		return ValueTask.CompletedTask ;
	}
} ;

