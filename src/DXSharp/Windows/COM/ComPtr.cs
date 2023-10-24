// COPYRIGHT NOTICES:
// --------------------------------------------------------------------------------
// Copyright © 2023 Arkaen Solutions, All Rights Reserved.
// (Original: Copyright © 2022 DXSharp - ATC - Aaron T. Carter)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------

#region Using Directives
using System.Runtime.InteropServices ;
using System.Diagnostics.CodeAnalysis ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using static DXSharp.Windows.COM.COMUtility ;
#endregion
namespace DXSharp.Windows.COM ;


public abstract class ComPtr: IDisposable {
	#region Const & Readonly Fields
	/// <summary>Value of a "null" (zero) pointer/address.</summary>
	public const int NULL_PTR = 0x0000000000000000 ;
	/// <summary>The GUID of the IUnknown COM interface.</summary>
	public static readonly Guid GUID_IUNKNOWN =
		Guid.Parse( "00000000-0000-0000-C000-000000000046" ) ;
	//! ComPtr tracking collection:
	static readonly HashSet< nint > _allPtrs = new( ) ;
	#endregion
	
	public abstract Type ComType { get ; }
	
	nint _baseAddr ;
	protected object? _comObjectRef ;
	internal uint RefCount => _refCount ;
	protected uint _refCount, _marshalRefCount ;
	
	internal nint BaseAddress => _baseAddr ;
	internal object? InterfaceObjectRef =>
		_comObjectRef ??= GetCOM_RCW( _baseAddr ) ;
	
	//! Constructors & Initializers:
	void _setBasePointer( nint pUnknown ) {
		this._baseAddr = pUnknown ;
		TrackPointer( this._baseAddr ) ;
		AddRef( this._baseAddr ) ;
	}

	
	#region Constructors
	internal ComPtr( ) { }
	public ComPtr( nint ptr ) {
#if (DEBUG || DEBUG_COM) || DEV_BUILD
		if( !ptr.IsValid() ) throw new ArgumentNullException( nameof(ptr), 
															  $"{nameof(ComPtr)} -> c'tor( {nameof(IntPtr)} ) :: " +
															  $"The pointer is a {nameof(NULL_PTR)} (0x00000000) value!" ) ;
		
		var _hr = QueryInterface< IUnknown >( ptr, out nint pUnknown ) ;
		if( _hr.Failed ) throw new ArgumentException( nameof(ptr),
													  $"{nameof(ComPtr)} -> c'tor( {nameof(IntPtr)} ) :: " +
													  $"The pointer is not a valid COM object implementing {nameof(IUnknown)} interface!" ) ;
		Release( ptr ) ;
#endif
		
		var pObject = GetCOM_RCW( ptr )
#if DEBUG || DEBUG_COM || DEV_BUILD
					  ?? throw new DirectXComError( $"{nameof(ComPtr)} -> c'tor( {nameof(IntPtr)} ) :: " +
													$"Failed to get the COM object!" )
#endif
			;
		
		_setBasePointer( pUnknown ) ;
		_comObjectRef = pObject ;
	}
	public unsafe ComPtr( void* ptr ): this( (nint)ptr ) { }
	public ComPtr( int addr ): this( (nint)addr ) { }
	public ComPtr( uint addr ): this( (nint)addr ) { }
	public ComPtr( long addr ): this( (nint)addr ) { }
	public ComPtr( ulong addr ): this( (nint)addr ) { }
	public ComPtr( [NotNull] in IUnknown comInterface ): this( GetAddressIUnknown(comInterface) ) { }
	public ComPtr( [NotNull] in IUnknownWrapper comInterface ): this( comInterface.BasePointer ) { }
	public ComPtr( [NotNull] in IUnknownWrapper< IUnknown > comInterface ): this( comInterface.BasePointer ) { }
	public ComPtr( [MaybeNull] in object? comObj ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ArgumentNullException.ThrowIfNull( comObj, nameof(comObj) ) ;
		if( !comObj.GetType( ).IsCOMObject ) throw new ArgumentException( nameof(comObj),
																		  $"{nameof(ComPtr)} -> c'tor( {nameof(Object)} ) :: " +
																		  $"The object is not a valid COM object!" ) ;
#endif
		
		nint pUnknown = GetIUnknownForObject( comObj ) ;
#if DEBUG || DEBUG_COM || DEV_BUILD
		if ( !pUnknown.IsValid() ) throw new ArgumentException( nameof(comObj),
																$"{nameof(ComPtr)} -> c'tor( {nameof(Object)} ) :: " +
																$"The object is not a valid COM object implementing {nameof(IUnknown)}!" ) ;
#endif

		_setBasePointer( pUnknown ) ;
		_comObjectRef = comObj ;
		Release( pUnknown ) ; // Release the reference we just added.
	}
	#endregion
	
		
	//! System.Object overrides:
	public override string ToString( ) => $"COM OBJECT[ Address: 0x{BaseAddress:X} ]" ;
	public override int GetHashCode( ) => BaseAddress.GetHashCode( ) ;
	public override bool Equals( object? obj ) => 
		( obj is ComPtr ptr && ptr.BaseAddress == BaseAddress )
			|| ( obj is nint address && address == BaseAddress ) ;
	
	//! IDisposable:
	public bool Disposed => !BaseAddress.IsValid( ) ;
	~ComPtr( ) => Dispose( ) ;
	public virtual void Dispose( ) {
		if( BaseAddress.IsValid() ) Release( ref _baseAddr ) ;
		UntrackPointer( this._baseAddr ) ;
		GC.SuppressFinalize( this ) ;
	}
	
	
	//! Internal Pointer Tracking & Management:
	internal virtual void Set( nint newPtr ) {
#if DEBUG
		string errMsg = $"{nameof(ComPtr)} -> {nameof(Set)}( {nameof(IntPtr)} ) :: " ;
		if( !newPtr.IsValid() ) throw new ArgumentNullException( nameof(newPtr), errMsg +
																	 $"The pointer is a \"{nameof(NULL_PTR)}\" (null/0x00000000) value!" ) ;
		if( !Exists(newPtr) ) throw new 
			ArgumentException( nameof(newPtr), errMsg + 
											   $"The pointer is not a valid COM object!" ) ;
#endif
		var _hr = QueryInterface< IUnknown >( newPtr, out nint pUnknown ) ;
		if( _hr.Failed ) throw new 
			ArgumentException( nameof(newPtr), errMsg +
											   $"Failed to obtain {nameof(IUnknown)} interface!" ) ;
		
		Release( ref _baseAddr ) ;
		_setBasePointer( pUnknown ) ;
	}

	internal virtual void Set< TInterface >( in TInterface? newComObject )
												where TInterface: IUnknown {
		nint pUnknown = GetIUnknownForObject( newComObject ) ;
		if ( !pUnknown.IsValid() ) throw new ArgumentException( nameof(newComObject), 
					$"{nameof(ComPtr)} -> Set( {nameof(Object)} ) :: " +
						$"The object is not a valid COM object implementing {nameof(IUnknown)}!" ) ;
		
		this.Set( pUnknown ) ;
	}
	
	internal static nint[ ] GetAllAllocations( ) =>
		_allPtrs.Where( p => p.IsValid() && !IsDestroyed(p) )
					.ToArray( ) ;
	
	internal static void TrackPointer( nint ptr ) {
		if( ptr.IsValid() ) _allPtrs?.Add( ptr ) ;
	}
	internal static void UntrackPointer( nint ptr ) => _allPtrs?.Remove( ptr ) ;
	
	
	internal static ComPtr< TInto > Convert< TFrom, TInto >( ComPtr< TFrom >? other ) 
																where TFrom: IUnknown 
																where TInto: IUnknown {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		if ( other.Disposed ) throw new ObjectDisposedException( nameof(other) ) ;

		Type srcType = typeof(TFrom), dstType = typeof(TInto) ;
		if ( srcType.IsAssignableTo(dstType) ) return new( other.BaseAddress ) ;
		
		throw new InvalidCastException( $"{nameof(ComPtr)} -> " +
										$"{nameof(Convert)}<{dstType.Name}, {srcType.FullName}> :: " +
										$"Failed to cast the COM object to {dstType.Name}!" ) ;
	}
} ;



public sealed class ComPtr< T >: ComPtr 
								 where T: IUnknown {
	public override Type ComType => typeof(T) ;
	
	nint _interfaceVPtr ;
	public Type InterfaceType => typeof(T) ;
	public T? Interface { get ; private set ; }
	public nint InterfaceVPtr => _interfaceVPtr ;
	
	
	internal ComPtr( ) { }
	public ComPtr( [NotNull] in T comInterface ): 
					base( GetAddressIUnknown(comInterface) ) {
		if( comInterface is null ) throw new ArgumentNullException( nameof(comInterface) ) ;
		var _hr = QueryInterface<T>( base.BaseAddress, out nint ptrToInterface ) ;
		if( _hr.Failed ) throw new COMException($"{nameof(ComPtr<T>)} -> c'tor( {nameof(IUnknown)} ) :: " +
												$"Failed to get a COM interface pointer to {nameof(T)}!", _hr.Value ) ;
		this.Interface      = comInterface ;
		this._interfaceVPtr = ptrToInterface ;
	}
	
	public ComPtr( nint address ): base(address) {
		if( !address.IsValid() )
			throw new ArgumentNullException( nameof(address),
				$"{nameof(ComPtr)}<{typeof(T).FullName}> -> c'tor( {nameof(IntPtr)} ) :: " +
							$"The pointer is a \"{nameof(NULL_PTR)}\" (null/0x{NULL_PTR:X}) value!" ) ;
		
		var _interface = GetCOMObject< T >(address) ;
		if( _interface is null ) throw new ArgumentException( nameof(address),
							$"{nameof(ComPtr)}<{typeof(T).FullName}> -> c'tor( {nameof(IntPtr)} ) :: " +
							$"The pointer is not a valid COM object implementing {typeof(T).FullName} interface!" ) ;
		
		Interface           = _interface ;
		this._interfaceVPtr = GetInterfaceVPointer< T, T >( _interface ) ;
		//Marshal.GetComInterfaceForObject( _interface, typeof(T) ) ;
	}
	
	
	internal uint IncrementReferences( ) {
		_marshalRefCount = Interface?.AddRef( ) ?? 0 ;
		return ++_refCount ;
	}
	internal T? GetReference( ) {
		GetReference( out _ ) ;
		return Interface! ;
	}
	internal uint GetReference( out T? pInterface ) {
		pInterface = default ;
		if( Disposed || Interface is null ) 
			throw new NullReferenceException( $"{nameof(ComPtr<T>)}<{typeof(T).FullName}> -> " +
							$"{nameof(ComPtr<T>.GetReference)} :: The internal {nameof(Interface)} is null!" ) ;
		
		pInterface = Interface ;
		return IncrementReferences( ) ;
	}

	internal uint ReleaseReference( bool final = false ) {
		if( Disposed || Interface is null )
			throw new NullReferenceException( $"{nameof(ComPtr<T>)}<{typeof(T).FullName}> -> " +
											  $"{nameof(ComPtr<T>.GetReference)} :: The internal {nameof(Interface)} is null!" ) ;
		
		_marshalRefCount = Interface.Release( ) ;
		return --_refCount ;
	}
	
	internal override void Set< TInterface >( in TInterface? newComObject ) where TInterface: default {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ArgumentNullException.ThrowIfNull( newComObject, nameof(newComObject) ) ;
		if( !typeof(TInterface).IsCOMObject )
			throw new ArgumentException( nameof(newComObject), 
										 $"{nameof(ComPtr<T>)}<{typeof(T).FullName}> -> " + 
										 $"{nameof(Set)}( {newComObject.GetType().Name} {nameof(newComObject)} ) :: " +
										 $"The object is not a valid COM object!" ) ;
		
		// Get the base IUnknown pointer:
		/*nint pUnknown = GetIUnknownForObject( newComObject ) ;
		if ( !pUnknown.IsValid() ) 
			throw new ArgumentException( nameof(newComObject), 
										 $"{nameof(ComPtr)}<{typeof(T).FullName}> -> Set( {nameof(Object)} ) :: " +
										 $"The object is not a valid COM object implementing {nameof(IUnknown)}!" ) ;*/
#endif
		
		// All checks passed, set the pointers:
		//base.Set( pUnknown ) ;
		base.Set( newComObject ) ;
		
		// Query the interface and see if it's actually part of the COM object:
		var _hr = QueryInterface< TInterface >( BaseAddress, 
														out nint pInterface ) ;
		if ( _hr.Failed ) {
			throw new ArgumentException( nameof(newComObject),
										 $"{nameof(ComPtr<T>)}<{typeof(T).FullName}> -> " + 
										 $"{nameof(Set)}( {newComObject?.GetType().Name} {nameof(newComObject)} ) :: " +
										 $"The object is not a valid COM object implementing {typeof(T).FullName}!" ) ;
		}
		
		
		_interfaceVPtr = pInterface ;
		if( newComObject is T newComObjectAsT )
			Interface = newComObjectAsT ;
		else {
			Interface = GetCOMObject< T >( _interfaceVPtr )
#if DEBUG || DEBUG_COM || DEV_BUILD
						?? throw new NullReferenceException( 
															$"{nameof(ComPtr<T>)}<{typeof(T).FullName}> -> " +
															$"{nameof(Set)}( {newComObject.GetType().Name} {nameof(newComObject)} ) :: " +
															$"Failed to get the COM object!" )
#endif
						;
			
		}
	}
	
	
	public ComPtr< T > Clone( ) {
		T? obj = GetReference( ) ?? throw new NullReferenceException( ) ;
		ComPtr< T > newComPtr = new( obj ) ;
		return newComPtr ;
	}
	
	public ComPtr< TOut > Cast< TOut >( ) where TOut: IUnknown {
		if( !InterfaceType.IsAssignableTo( typeof(TOut) )) throw new InvalidCastException( ) ;
		
		var _result = QueryInterface< TOut >( BaseAddress, out nint pInterface ) ;
		if( _result.Failed ) throw new COMException( $"{nameof(ComPtr<T>)}<{typeof(T).FullName}> -> " +
													  $"{nameof(Cast)}<{typeof(TOut).FullName}> :: " +
													  $"Failed to cast the COM object to {typeof(TOut).FullName}!" ) ;
		
		ComPtr< TOut > newComPtr = new( _result ) ;
		newComPtr.IncrementReferences( ) ;
		return newComPtr ;
	}


	// ----------------------------------------------------------
	// Operator Overloads:
	// ----------------------------------------------------------
	
	public static explicit operator ComPtr< IUnknown >?( ComPtr< T >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		return new( other.BaseAddress ) ;
	}

	public static explicit operator ComPtr< T >?( ComPtr< IUnknown >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		if( other.Interface is T _interface ) return new( other.BaseAddress ) ;
		
		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
										$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(T)}!" ) ;
	}

	public static explicit operator ComPtr< IDXGIObject >( ComPtr< T >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		
		if( other.InterfaceType.IsAssignableTo(typeof(IDXGIObject)) 
			&& other.Interface is IDXGIObject dxgiObj ) return new( dxgiObj ) ;
		
		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
										$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(ID3D12Object)}!" ) ;
	}

	public static explicit operator ComPtr< T >( ComPtr< IDXGIObject >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		
		if( typeof(T).IsAssignableFrom(typeof(IDXGIObject))
			&& other.Interface is T _interface ) return new( other.BaseAddress ) ;

		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
										$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(T)}!" ) ;
	}

	public static explicit operator ComPtr< ID3D12Object >( ComPtr< T >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		
		if( other.InterfaceType.IsAssignableTo(typeof(ID3D12Object)) 
			&& other.Interface is ID3D12Object d3dObj ) return new( d3dObj ) ;
		
		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
										$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(ID3D12Object)}!" ) ;
	}

	public static explicit operator ComPtr< T >( ComPtr< ID3D12Object >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		
		if( typeof(T).IsAssignableFrom(typeof(ID3D12Object))
			&& other.Interface is T _interface ) return new( other.BaseAddress ) ;

		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
										$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(T)}!" ) ;
	}
	
	
	public static explicit operator ComPtr< T >( DXGI.Object obj ) {
		ArgumentNullException.ThrowIfNull( obj, nameof(obj) ) ;
		if( obj.ComPointer?.Disposed ?? true ) throw new 
			ObjectDisposedException( nameof(obj) ) ;

		var other = obj.ComPointer ;
		if ( obj.ComPointer.InterfaceType.IsAssignableTo(typeof(T)) ) {
			var _result = new ComPtr< T >( obj.ComPointer.BaseAddress ) ;
			_result.IncrementReferences( ) ;
			return _result ;
		}
		
		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
			$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(T)}!" ) ;
	}
	
	public static explicit operator ComPtr< T >( Direct3D12.Object obj ) {
		ArgumentNullException.ThrowIfNull( obj, nameof(obj) ) ;
		if( obj.ComPointer?.Disposed ?? true ) throw new 
			ObjectDisposedException( nameof(obj) ) ;
		
		var other = obj.ComPointer ;
		if ( obj.ComPointer.InterfaceType.IsAssignableTo(typeof(T)) ) {
			var _result = new ComPtr< T >( obj.ComPointer.BaseAddress ) ;
			_result.IncrementReferences( ) ;
			return _result ;
		}
		
		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
			$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(T)}!" ) ;
	}
} ;
