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

using static DXSharp.Windows.COM.COMUtility ;
#endregion

namespace DXSharp.Windows.COM ;


public class ComPtr: IDisposable {
	#region Const & Readonly Fields
	/// <summary>Value of a "null" (zero) pointer/address.</summary>
	public const int NULL_PTR = 0x0000000000000000 ;
	/// <summary>The GUID of the IUnknown COM interface.</summary>
	public static readonly Guid GUID_IUNKNOWN =
		Guid.Parse( "00000000-0000-0000-C000-000000000046" ) ;
	//! ComPtr tracking collection:
	static readonly HashSet< nint > _allPtrs = new( ) ;
	#endregion
	
	nint _unknownAddress ;
	internal nint IUnknownAddress => _unknownAddress ;
	
	//! Constructors & Initializers:
	void _setBasePointer( IntPtr pUnknown ) {
		this._unknownAddress = pUnknown ;
		TrackPointer( this._unknownAddress ) ;
		AddRef( this._unknownAddress ) ;
	}
	
	public ComPtr( nint ptr ) {
		if( !ptr.IsValid() ) throw new ArgumentNullException( nameof(ptr), 
							$"{nameof(ComPtr)} -> c'tor( {nameof(IntPtr)} ) :: " +
							$"The pointer is a {nameof(NULL_PTR)} (0x00000000) value!" ) ;
		if( !Exists(ptr) ) throw new ArgumentException( nameof(ptr),
							$"{nameof(ComPtr)} -> c'tor( {nameof(IntPtr)} ) :: " +
							$"The pointer is not a valid COM object!" ) ;

		var _hr = QueryInterface< IUnknown >( ptr, out nint pUnknown ) ;
		if( _hr.Failed ) throw new ArgumentException( nameof(ptr),
							$"{nameof(ComPtr)} -> c'tor( {nameof(IntPtr)} ) :: " +
							$"The pointer is not a valid COM object implementing {nameof(IUnknown)} interface!" ) ;
		
		_setBasePointer( pUnknown ) ;
	}
	public ComPtr( int   addr ): this( (nint)addr ) { }
	public ComPtr( uint  addr ): this( (nint)addr ) { }
	public ComPtr( long  addr ): this( (nint)addr ) { }
	public ComPtr( ulong addr ): this( (nint)addr ) { }
	
	public ComPtr( [NotNull] in IUnknown comInterface ): this( GetAddressIUnknown(comInterface) ) { }
	public ComPtr( [NotNull] in IUnknownWrapper comInterface ): this( comInterface.BasePointer ) { }
	public ComPtr( [NotNull] in IUnknownWrapper< IUnknown > comInterface ): this( comInterface.Pointer ) { }
	public ComPtr( [MaybeNull] in object? comObj ) {
		if( comObj is null ) throw new ArgumentNullException( nameof(comObj) ) ;
		if( !comObj.GetType( ).IsCOMObject ) throw new ArgumentException( nameof(comObj),
							$"{nameof(ComPtr)} -> c'tor( {nameof(Object)} ) :: " +
													$"The object is not a valid COM object!" ) ;
		
		nint pUnknown = GetIUnknownForObject( comObj ) ;
		if ( !pUnknown.IsValid() ) throw new ArgumentException( nameof(comObj),
							$"{nameof(ComPtr)} -> c'tor( {nameof(Object)} ) :: " +
										$"The object is not a valid COM object implementing {nameof(IUnknown)}!" ) ;

		_setBasePointer( pUnknown ) ;
	}
	
	//! System.Object overrides:
	public override string ToString( ) => $"COM OBJECT[ 0x{IUnknownAddress:X} ]" ;
	public override int GetHashCode( ) => IUnknownAddress.GetHashCode( ) ;
	public override bool Equals( object? obj ) => 
		( obj is ComPtr ptr && ptr.IUnknownAddress == IUnknownAddress )
			|| ( obj is nint address && address == IUnknownAddress ) ;
	
	//! IDisposable:
	public bool Disposed => !IUnknownAddress.IsValid() ;
	~ComPtr( ) => Dispose( ) ;
	public void Dispose( ) {
		if( IUnknownAddress.IsValid() ) 
			Release( ref _unknownAddress ) ;
		UntrackPointer( this._unknownAddress ) ;
		GC.SuppressFinalize( this ) ;
	}
	
	
	//! Internal Pointer Tracking:
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
	public readonly nint InterfaceVPtr ;
	public T? Interface { get ; init ; }
	
	public ComPtr( [NotNull] in T comInterface ): 
					base( GetAddressIUnknown(comInterface) ) {
		if( comInterface is null ) throw new ArgumentNullException( nameof(comInterface) ) ;
		var _hr = QueryInterface<T>( base.IUnknownAddress, out nint ptrToInterface ) ;
		if( _hr.Failed ) throw new COMException($"{nameof(ComPtr<T>)} -> c'tor( {nameof(IUnknown)} ) :: " +
												$"Failed to get a COM interface pointer to {nameof(T)}!", _hr.Value ) ;
		this.Interface = comInterface ;
		this.InterfaceVPtr = ptrToInterface ;
	}
	
	public ComPtr( nint address ): base( address ) {
		var _hr = QueryInterface( address, out T _interface ) ;
		if( _hr.Failed ) throw new ArgumentException( nameof(address),
							$"{nameof(ComPtr)}<{typeof(T).FullName}> -> c'tor( {nameof(IntPtr)} ) :: " +
							$"The pointer is not a valid COM object implementing {typeof(T).FullName} interface!" ) ;
		
		_hr = QueryInterface<T>( address, out nint ptrToInterface ) ;
		if( _hr.Failed ) throw new COMException($"{nameof(ComPtr<T>)} -> c'tor( {nameof(IntPtr)} ) :: " +
												$"Failed to get the interface pointer!", _hr.Value ) ;
		
		this.InterfaceVPtr = ptrToInterface ;
		Interface = _interface ;
	}
} ;
