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
		
		//! Reducing number of debug checks for performance ...
		/*if( !Exists(ptr) ) throw new ArgumentException( nameof(ptr),
														$"{nameof(ComPtr)} -> c'tor( {nameof(IntPtr)} ) :: " +
														$"The pointer is not a valid COM object!" ) ;*/
		
		var _hr = QueryInterface< IUnknown >( ptr, out nint pUnknown ) ;
		if( _hr.Failed ) throw new ArgumentException( nameof(ptr),
													  $"{nameof(ComPtr)} -> c'tor( {nameof(IntPtr)} ) :: " +
													  $"The pointer is not a valid COM object implementing {nameof(IUnknown)} interface!" ) ;
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
	public ComPtr( [NotNull] in IUnknownWrapper< IUnknown > comInterface ): this( comInterface.Pointer ) { }
	public ComPtr( [MaybeNull] in object? comObj ) {
		ArgumentNullException.ThrowIfNull( comObj, nameof(comObj) ) ;
		if( !comObj.GetType( ).IsCOMObject ) throw new ArgumentException( nameof(comObj),
																		  $"{nameof(ComPtr)} -> c'tor( {nameof(Object)} ) :: " +
																		  $"The object is not a valid COM object!" ) ;
		
		nint pUnknown = GetIUnknownForObject( comObj ) ;
		if ( !pUnknown.IsValid() ) throw new ArgumentException( nameof(comObj),
																$"{nameof(ComPtr)} -> c'tor( {nameof(Object)} ) :: " +
																$"The object is not a valid COM object implementing {nameof(IUnknown)}!" ) ;

		_setBasePointer( pUnknown ) ;
		_comObjectRef = comObj ;
	}
	#endregion
	
		
	//! System.Object overrides:
	public override string ToString( ) => $"COM OBJECT[ Address: 0x{BaseAddress:X} ]" ;
	public override int GetHashCode( ) => BaseAddress.GetHashCode( ) ;
	public override bool Equals( object? obj ) => 
		( obj is ComPtr ptr && ptr.BaseAddress == BaseAddress )
			|| ( obj is nint address && address == BaseAddress ) ;
	
	//! IDisposable:
	public bool Disposed => !BaseAddress.IsValid() ;
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
		
		var _hr = QueryInterface< IUnknown >( newPtr, out nint pUnknown ) ;
		if( _hr.Failed ) throw new 
			ArgumentException( nameof(newPtr), errMsg +
											   $"Failed to obtain {nameof(IUnknown)} interface!" ) ;
#endif
		
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
} ;


public sealed class ComPtr< T >: ComPtr 
								 where T: IUnknown {
	nint _interfaceVPtr ;
	public nint InterfaceVPtr => _interfaceVPtr ;
	
	public T? Interface { get ; private set ; }
	
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
		/*var _hr = QueryInterface( address, out T _interface ) ;
		if( _hr.Failed ) throw new ArgumentException( nameof(address),
							$"{nameof(ComPtr)}<{typeof(T).FullName}> -> c'tor( {nameof(IntPtr)} ) :: " +
							$"The pointer is not a valid COM object implementing {typeof(T).FullName} interface!" ) ;
		
		_hr = QueryInterface< T >( address, out nint ptrToInterface ) ;
		if( _hr.Failed ) throw new COMException($"{nameof(ComPtr<T>)} -> c'tor( {nameof(IntPtr)} ) :: " +
												$"Failed to get the interface pointer!", _hr.Value ) ;*/
		
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
		if( !typeof(TInterface).IsCOMObject )
			throw new ArgumentException( nameof(newComObject), 
										 $"{nameof(ComPtr<T>)}<{typeof(T).FullName}> -> " + 
										 $"{nameof(Set)}( {newComObject.GetType().Name} {nameof(newComObject)} ) :: " +
										 $"The object is not a valid COM object!" ) ;
		ArgumentNullException.ThrowIfNull( nameof(newComObject) ) ;
		
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
		T? obj = GetReference( ) ?? throw new NullReferenceException() ;
		ComPtr< T > newComPtr = new( obj ) ;
		return newComPtr ;
	}
	
} ;
