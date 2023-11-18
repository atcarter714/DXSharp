#region Using Directives
using System.Linq ;
using System.Diagnostics ;
using System.Collections.Concurrent ;
using System.Runtime.InteropServices ;
using Microsoft.VisualBasic ;
using static DXSharp.Windows.COM.COMUtility ;
#endregion
namespace DXSharp.Windows.COM ;


// ---------------------------------------------------------------------------------------------------------------------
// Data Records:
// ---------------------------------------------------------------------------------------------------------------------

/// <summary>
/// The base record for representations of a native COM interface reference.
/// </summary>
/// <param name="InterfaceType">The type of COM interface RCW object.</param>
/// <param name="VTablePtr">Pointer to COM the interface's VTable</param>
/// <param name="InterfaceRCW"> The COM RCW as a <see cref="IUnknown"/> object reference.</param>
internal record ComInterfaceRef( Type InterfaceType,
								 nint VTablePtr,
								 IUnknown InterfaceRCW ) ;

/// <summary>
/// A record which represents a native COM interface reference.
/// </summary>
/// <param name="VTablePtr">Pointer to the COM interface's VTable.</param>
/// <param name="InterfaceObject">Reference to the COM RCW object.</param>
/// <typeparam name="T">The type of COM RCW object.</typeparam>
internal record ComInterfaceRef< T >( nint VTablePtr,
									  T InterfaceObject ):
				ComInterfaceRef( typeof(T), VTablePtr, InterfaceObject ) 
									where T: IUnknown {
	
	/// <summary>
	/// Gets a reference to the COM RCW object as a <typeparamref name="T"/> instance.
	/// </summary>
	public T InterfaceObject { get ; init ; } = InterfaceObject ;
} ;



// ---------------------------------------------------------------------------------------------------------------------
// COMResource Class:
// ---------------------------------------------------------------------------------------------------------------------

/// <summary>
/// Represents a COM object resource which has one or more interface instances.
/// </summary>
#if DEBUG || DEBUG_COM || DEV_BUILD
[DebuggerDisplay("{DebugDisplayStr,nq}")]
#endif
internal class COMResource: DisposableObject {
	nint _pUnknown ;
	Type _initialType ;
	
	ConcurrentDictionary< nint, int > _refCounts = new( ) ;
	ConcurrentDictionary< ComInterfaceRef, ComPtr > _pointers  = new( ) ;
	
	public bool IsAlive => _pointers.Count > 0 ;
	public override bool Disposed { get ; protected set ; }
	
	public int TotalRefCount {
		get {
			int count = 0 ;
			foreach ( var entry in _refCounts ) {
				count += entry.Value ;
			}
			return count ;
		}
	}
	
	
#if DEBUG || DEBUG_COM || DEV_BUILD
	public string DebugDisplayStr => 
		$"COMResource: [ \"{_initialType.Name}\", {_initialType.GUID} ] " +
			$"(Refs: {TotalRefCount}, ComPtrs: {_pointers.Count})" ;
#endif
	
	// ----------------------------------------------------------
	// Constructors:
	// ----------------------------------------------------------

	public COMResource( ComPtr ptr ) {
		ComInterfaceRef ptrRef = new( ptr.ComType,
									  ptr.BaseAddress,
									  (IUnknown)ptr.InterfaceObjectRef! ) ;
		
		_pointers.TryAdd( ptrRef, ptr ) ;
		_initialType = ptr.ComType ;
		_pUnknown    = ptr.BaseAddress ;
	}

	public COMResource( nint pUnknown ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		if ( !pUnknown.IsValid( ) ) 
			throw new ArgumentNullException( nameof( pUnknown ), 
											 $"The pointer \"{nameof(pUnknown)}\" is null!" ) ;
#endif
		
		_pUnknown    = pUnknown ;
		_initialType = typeof( IUnknown ) ;
		ComPtr< IUnknown >? ptr = new( pUnknown ) ;
		AddPointer( ptr ) ;
	}
	
	public COMResource( nint address, Type type ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		if ( !address.IsValid( ) ) 
			throw new ArgumentNullException( nameof( address ), 
											 $"The pointer \"{nameof(address)}\" is null!" ) ;
		if ( !type.IsCOMObject )
			throw new TypeInitializationException( nameof( type ),
												   new DirectXComError( HResult.E_FAIL,
																		$"The type \"{type.Name}\" is not a COM interface type." ) ) ;
#endif
		
		_initialType = type ;
		_pUnknown    = address ;
		var    _obj       = Marshal.GetObjectForIUnknown( address ) ;
		var    _interface = Marshal.GetComInterfaceForObject( _obj, type ) ;
		int refCount = Marshal.Release( address ) ;
		
		ComPtr? ptr = (ComPtr) Activator.CreateInstance( typeof( ComPtr<> )
															.MakeGenericType( type ), 
																_interface )! ;
		
		//! Decrease the reference counts by 1, since we added to them:
		int count = (int)( ( ptr.InterfaceObjectRef as IUnknown )?.Release( ) ?? 0x00U ) ;
		Marshal.Release( address ) ;
		
		AddPointer( ptr ) ;
	}
	
	public COMResource( IUnknown @interface ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ArgumentNullException.ThrowIfNull( @interface, nameof( @interface ) ) ;
#endif
		
		_initialType = @interface.GetType( ) ;
		_pUnknown    = GetAddressIUnknown( @interface ) ;
		var interfacePtr = Marshal.GetComInterfaceForObject( @interface, _initialType ) ;
		
		ComPtr? ptr = (ComPtr) Activator.CreateInstance( typeof( ComPtr<> )
															.MakeGenericType( _initialType ), 
														 interfacePtr )! ;
		
		AddPointer( ptr ) ;
	}

	// ----------------------------------------------------------
	// Instance Methods:
	// ----------------------------------------------------------
	
	
	/// <summary>Initializes a new <see cref="ComPtr{T}"/> instance and tracks it internally.
	/// </summary><typeparam name="T">The type of COM interface RCW object to create a <see cref="ComPtr{T}"/> for.</typeparam>
	/// <returns>If successful, a new <see cref="ComPtr{T}"/> instance is returned.</returns>
	protected ComPtr< T >? _initNewPointer< T >( ) where T: IUnknown {
		var _ptrType = typeof( T ) ;
		
		foreach ( var entry in _pointers ) {
			if ( _ptrType.IsAssignableFrom( entry.Value.ComType ) ) {
				if ( entry.Value.InterfaceObjectRef is T _interface ) {
					ComInterfaceRef< T > newRef = new( entry.Key.VTablePtr, _interface ) ;
					ComPtr< T > newPtr = ComPtr< T >.AttachedTo( _interface ) ;
					
					newPtr.IncrementReferences( ) ;
					_pointers.TryAdd( newRef, newPtr ) ;
					_refCounts.AddOrUpdate( newPtr.InterfaceVPtr, 1,
											( n, c ) => ++c ) ;
					return newPtr ;
				}
			}
		}
		
		// Try querying the COM object for the interface:
		var hr = QueryInterface< T >( _pUnknown, out nint pInterface ) ;
#if DEBUG || DEBUG_COM || DEV_BUILD
		hr.ThrowOnFailure( ) ;
		if ( !pInterface.IsValid( ) )
			throw new DirectXComError( $"No such COM interface \"{typeof( T ).Name}\" " +
									   $"supported by this object!" ) ;
#endif
		
		Release( _pUnknown ) ; // Decrease the reference count by 1, since we added to it.
		
		ComPtr< T > ptr = new( pInterface ) ;
		ComInterfaceRef< T > ptrRef = new( pInterface, ptr.Interface! ) ;
		
		_pointers.TryAdd( ptrRef, ptr ) ;
		_refCounts.AddOrUpdate( ptr.InterfaceVPtr,
								( n ) => 1, ( n, c ) => c + 1 ) ;
		return ptr ;
	}

	
	/// <summary>
	/// Adds a new <see cref="ComPtr"/> instance to the internal references data.
	/// </summary>
	/// <param name="ptr">
	/// The <see cref="ComPtr"/> instance to add to the internal references data.
	/// </param>
	internal void AddPointer( ComPtr ptr ) {
		ComInterfaceRef ptrRef = new( ptr.ComType,
									  ptr.BaseAddress,
									  (IUnknown)ptr.InterfaceObjectRef! ) ;
		
		//_interfaces.Add( ptrRef ) ;
		_pointers.TryAdd( ptrRef, ptr ) ;
		_refCounts.AddOrUpdate( ptr.BaseAddress,
								( n ) => 1, ( n, c ) => c + 1 ) ;
	}
	
	
	/// <summary>Adds a new <see cref="ComPtr{T}"/> instance to the internal references data.</summary>
	/// <param name="ptr">The <see cref="ComPtr{T}"/> instance to add to the internal references data.</param>
	/// <typeparam name="T">The type of COM interface RCW object.</typeparam>
	internal void AddPointer< T >( ComPtr< T > ptr ) where T: IUnknown {
		ComInterfaceRef< T > ptrRef = new( ptr.InterfaceVPtr, ptr.Interface! ) ;
		
		_pointers.TryAdd( ptrRef, ptr ) ;
		_refCounts.AddOrUpdate( ptr.InterfaceVPtr,
								( n ) => 1, ( n, c ) => c + 1 ) ;
	}
	
	
	/// <summary>
	/// Gets a <see cref="ComPtr{T}"/> instance for the given COM interface type.
	/// </summary>
	/// <typeparam name="T">The type of COM interface RCW.</typeparam>
	/// <returns>
	/// If successful, a <see cref="ComPtr{T}"/> instance is returned.
	/// </returns>
	/// <exception cref="ObjectDisposedException">
	/// Thrown if the COM object has been disposed.
	/// </exception>
	/// <exception cref="DirectXComError">
	/// Thrown if the COM object does not support the given interface type.
	/// </exception>
	internal ComPtr< T > GetPointer< T >( ) where T: IUnknown {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( ( !IsAlive || Disposed ), 
											typeof(COMResource) ) ;
#endif
		
		// Search for existing ComPtr< T >:
		foreach ( var entry in _pointers ) {
			if ( entry.Value.ComType == typeof( T ) )
				return (ComPtr< T >)entry.Value ;
		}
		
		// Try to simply create a ComPtr< T >:
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
		int                  rc     = Release( _pUnknown ) ; // Decr. ref count (QueryInterface + 1) ...
		
		_pointers.TryAdd( ptrRef, ptr ) ;
		_refCounts.AddOrUpdate( ptr.InterfaceVPtr,
								( n ) => 1, ( n, c ) => c + 1 ) ;
		return ptr ;
	}

	
	/// <summary>
	/// Retrieves a reference to the given COM interface type
	/// specified by <typeparamref name="TInterface"/>.
	/// </summary>
	/// <typeparam name="TInterface">The desired COM interface type.</typeparam>
	/// <returns>
	/// If successful, a reference to the COM interface object is returned.
	/// Otherwise, an exception is thrown or the result can be <see langword="null"/>.
	/// </returns>
	/// <exception cref="NullReferenceException">
	/// Thrown if the COM object does not support the given interface type.
	/// </exception>
	/// <exception cref="DirectXComError">
	/// Thrown if the COM object does not support the given interface type.
	/// </exception>
	internal TInterface? GetReferenceTo< TInterface >( ) where TInterface: IUnknown {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( ( !IsAlive || Disposed ), typeof(COMResource) ) ;
#endif
		
		// Check for an existing object reference:
		foreach ( var entry in _pointers ) {
			if ( entry.Value.ComType == typeof( TInterface ) ) {
				try { return (TInterface?)entry.Value.InterfaceObjectRef
							 ?? throw new NullReferenceException( ) ; }
				catch { break ; }
			}
		}
		
		
		// Try to simply create a ComPtr< TInterface >:
		var newPtr = _initNewPointer< TInterface >( ) ;
		if ( newPtr is not null ) return newPtr.Interface! ;
		
		
		// At this point, we must try to query the COM object for the interface:
		var hr = QueryInterface< TInterface >( _pUnknown, out nint pInterface ) ;
#if DEBUG || DEBUG_COM || DEV_BUILD
		hr.SetAsLastErrorForThread( ) ;
		hr.ThrowOnFailure( ) ;
		
		if ( !pInterface.IsValid( ) )
			throw new DirectXComError( $"No such COM interface \"{typeof( TInterface ).Name}\" " +
									   $"supported by this object!" ) ;
#endif
		
		// Now create the ComPtr< TInterface > and ComInterfaceRef< TInterface >:
		ComPtr< TInterface > ptr    = new( pInterface ) ;
		ComInterfaceRef< TInterface > ptrRef = new( pInterface, ptr.Interface! ) ;
		
		
		//! Update the internal references data:
		_pointers.TryAdd( ptrRef, ptr ) ;
		_refCounts.AddOrUpdate( ptr.InterfaceVPtr,
								( n ) => 1, ( n, c ) => c + 1 ) ;
		
		//! Return the obtained interface object:
		return ptr.Interface! ;
	}
	
	
	/// <summary>
	/// Creates a new wrapper object of specified type <typeparamref name="T"/>
	/// with an instance of a COM RCW object reference of type <typeparamref name="I"/>.
	/// </summary>
	/// <param name="interface">The COM RCW object reference to use as the initial interface instance.</param>
	/// <typeparam name="T">The type of <see cref="IDXCOMObject"/> wrapper object to create.</typeparam>
	/// <typeparam name="I">The type of COM interface to create a <see cref="ComPtr{I}"/> for.</typeparam>
	/// <returns>
	/// If successful, a new <see cref="IDXCOMObject"/> wrapper object instance is returned.
	/// </returns>
	internal T CreateWrapperObjectInstance< T, I >( I? @interface ) where T: IDXCOMObject, IInstantiable
														   where I: IUnknown => (T)T.Instantiate( @interface ) ;
	
	
	/// <summary>
	/// Creates a new <see cref="COMResource"/> object instance with
	/// an initial <see cref="ComPtr{I}"/> for the desired COM interface.
	/// </summary>
	/// <typeparam name="T">The type of <see cref="IDXCOMObject"/> wrapper object to create.</typeparam>
	/// <typeparam name="I">The type of COM interface to create a <see cref="ComPtr{I}"/> for.</typeparam>
	/// <returns>
	/// If successful, a new <see cref="COMResource"/> object instance is returned.
	/// </returns>
	internal COMResource CreateObjectInstance< T, I >( ) where T: IDXCOMObject, IInstantiable
														 where I: IUnknown {
		var comptr = new ComPtr< I >( this._pUnknown ) ;
		return new( comptr ) ;
	}

	internal int FinalRelease( ) {
		int count = 0 ;
		List< Task > tasks = new( 2 ) ;
		var allComData = _pointers.Select(
										  kvp => 
											   ( Info: kvp.Key, Ptr: kvp.Value ) ).ToArray( ) ;
		
		var interfaceAddresses = allComData.Select( entry => entry.Info.VTablePtr )
												   .Distinct( )
													.ToArray( ) ;
		
		foreach ( var entry in allComData ) {
			if ( entry.Ptr is not { Disposed: false } alivePtr ) continue ;
			
			tasks.Add( alivePtr.DisposeAsync( )
									.AsTask( ) ) ;
		}
		
		bool done = Task.WaitAll( tasks.ToArray( ), 
					  TimeSpan.FromMilliseconds(256) ) ;
#if DEBUG || DEBUG_COM || DEV_BUILD
		if( !done ) {
			Debug.WriteLine( "Failed to dispose all COM pointers!" ) ;
			throw new DirectXComError( $"{nameof( COMResource )} ({_initialType.Name}) :: " +
									   $"Failed to release resources." ) ;
		}
#endif
		int released = tasks.Count ;
		_refCounts.Clear( ) ;
		_pointers.Clear( ) ;
		tasks.Clear( ) ;
		Disposed = true ;
		
		return (count - released) ;
	}
	
	
	// ----------------------------------------------------------
	// Disposable Methods:
	// ----------------------------------------------------------

	/// <summary>
	/// Destroys the COM object and all of its interface references.
	/// </summary>
	/// <remarks>
	/// This is done by calling the (inherited) <see cref="DisposableObject.Dispose()"/> or
	/// <see cref="DisposableObject.DisposeAsync"/> methods on the
	/// <see cref="ComPtr"/> instances.
	/// </remarks>
	protected virtual void Destroy( ) {
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
	
	
	/// <summary>Disposes of unmanaged/native resources, such as COM objects.</summary>
	/// <returns>
	/// A <see cref="ValueTask"/> representing the asynchronous operation.
	/// </returns>
	protected override async ValueTask DisposeUnmanaged( ) => await Task.Run( Destroy ) ;

	
	// ----------------------------------------------------------
	// Static Methods:
	// ----------------------------------------------------------
	
	public static COMResource Create( nint pUnknown ) => new( pUnknown ) ;
	public static COMResource Create( IUnknown pUnknown ) => new( pUnknown ) ;
	public static COMResource Create< T >( object pRCW ) where T: IUnknown => new( (T)pRCW ) ;
	
	
	public static COMResource Create( ComPtr ptr ) => new( ptr ) ;
	public static COMResource CreateFrom( COMResource other ) => new( other._pUnknown ) ;
	public static COMResource CreateForType< T >( COMResource other ) where T: IUnknown {
		ComPtr< T > ptr = other.GetPointer< T >( ) ;
		return new( ptr ) ;
	}
	
	// ==========================================================
} ;

