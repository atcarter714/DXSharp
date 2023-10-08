﻿#region Using Directives
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using Object = DXSharp.DXGI.Object ;
#endregion

namespace DXSharp.Windows.COM ;


public static class COMUtility {
	/// <summary>Value of a "null" (zero) pointer/address.</summary>
	public const int NULL_PTR = 0x0000000000000000 ;
	internal const short _MAXOPT_ = (0x100|0x200) ;
	
	static HResult _lastHResult = HResult.S_OK ;
	internal static HResult LastHResult => _lastHResult ;
	
	//! TODO: Decide which of these versions (Exists vs IsDestroyed) to keep ...
	[MethodImpl(_MAXOPT_)] public static bool Exists( nint pUnknown ) =>
					pUnknown.IsValid( ) && GetCOM_RCW( pUnknown ) is not null ;
	
	[MethodImpl(_MAXOPT_)] public static bool IsDestroyed( nint pUnknown ) {
		try {
			if( pUnknown is NULL_PTR ) return true ;
			int r = AddRef( pUnknown ) ;
			if( r is 0 ) return true ;
			Release( pUnknown ) ;
			return false ;
		}
		catch ( COMException comError ) { return true ; }
		catch ( AccessViolationException e ) { return true ; }
	}
	
	
	[MethodImpl(_MAXOPT_)] public static nint GetIUnknownForObject( [MaybeNull] in object? obj ) {
		if( obj is null ) return NULL_PTR ;
		nint pIUnknownForObject = Marshal.GetIUnknownForObject( obj ) ;
		return pIUnknownForObject ;
	}
	
	
	[MethodImpl(_MAXOPT_)]
	public static int AddRef( nint pUnknown ) {
		if( pUnknown is NULL_PTR ) return 0 ;
		return Marshal.AddRef( pUnknown ) ;
	}
	[MethodImpl(_MAXOPT_)]
	public static int Release( nint pUnknown ) {
		if( !pUnknown.IsValid() ) return NULL_PTR ;
		return Marshal.Release( pUnknown ) ;
	}
	[MethodImpl(_MAXOPT_)]
	public static int Release( ref nint pUnknown ) {
		int c = Release( pUnknown ) ; pUnknown = NULL_PTR ;
		return c ;
	}
	
	
	
	[MethodImpl(_MAXOPT_)]
	public static object? GetCOM_RCW( nint pUnknown ) =>
		pUnknown.IsValid( ) ? null : Marshal.GetObjectForIUnknown( pUnknown ) ;
	
	
	[MethodImpl(_MAXOPT_)]
	public static T? GetCOMObject< T >( nint pUnknown )
									where T: IUnknown => (T)GetCOM_RCW( pUnknown )! ;
	
	[MethodImpl(_MAXOPT_)]
	public static void GetCOMObject< T >( nint pUnknown, out T? ppObject ) where T: IUnknown =>
												ppObject = (T)GetCOM_RCW( pUnknown )! ;
	
	[MethodImpl(_MAXOPT_)]
	public static T? GetCOMObject< T >( [NotNull] in ComPtr< T > pUnknown ) where T: IUnknown =>
															(T)GetCOM_RCW( pUnknown.BaseAddress )! ;
	
	
	[MethodImpl(_MAXOPT_)]
	public static nint GetPtrToInterface< T, TInterface >( [NotNull] in T iObj )
															where TInterface: IUnknown {
		if( !typeof(TInterface).IsCOMObject ) throw new
			TypeAccessException( $"{nameof(COMUtility)} :: " +
								  $"The type '{typeof(TInterface).FullName}' " +
									$"is not a valid COM object type." ) ;
		if ( !IsCOMObject(iObj) )
			throw new ArgumentException( $"{nameof( COMUtility )} :: " +
										 $"The object '{iObj}' is not a COM object.", nameof( iObj ) ) ;
		if( iObj is null ) throw new ArgumentNullException( nameof(iObj) ) ;
		
		var pUnknownForObject = Marshal.GetIUnknownForObject( iObj ) ;
		if( !pUnknownForObject.IsValid() ) return NULL_PTR ;
		
		var _hr = QueryInterface< TInterface >( pUnknownForObject, out nint pInterface ) ;
		_lastHResult = _hr ;
		return pInterface ;
	}
	
	[MethodImpl(_MAXOPT_)]
	public static nint GetAddressIUnknown< T >( [NotNull] in T iObj ) where T: IUnknown =>
														Marshal.GetIUnknownForObject( iObj ) ;
	
	[MethodImpl(_MAXOPT_)]
	public static nint GetInterfaceAddress< T >( [NotNull] in T iObj )
													where T: IUnknown {
		var pUnknownForObject = Marshal.GetIUnknownForObject( iObj ) ;
		var hr = QueryInterface< T >( pUnknownForObject, out nint pInterface ) ;
		_lastHResult = hr ;
		return pInterface ;
	}
	
	[MethodImpl(_MAXOPT_)]
	public static bool TryGetAddress( object? obj, out nint ptr ) {
		ptr = NULL_PTR ;
		if( obj is null ) return false ;
		ptr = Marshal.GetIUnknownForObject( obj ) ;
		if( ptr is NULL_PTR ) return false ;
		return true ;
	}
	[MethodImpl(_MAXOPT_)]
	public static bool TryGetComPtr( object? obj, out ComPtr? comPtr ) {
		if( TryGetAddress(obj, out nint ptr) ) {
			comPtr = new( ptr ) ;
			return true ;
		}
		comPtr = null ;
		return false ;
	}
	
	
	[MethodImpl(_MAXOPT_)]
	public static ComPtr< T > GetComPtrIUnknown< T >( [NotNull] in T iObj )
													where T: IUnknown =>
														new( GetAddressIUnknown(iObj) ) ;
	[MethodImpl(_MAXOPT_)]
	public static ComPtr< T > GetComPtrTo< T, TInterface >( [NotNull] in T iObj )
																	where T: IUnknown
																	where TInterface: IUnknown =>
														new( GetPtrToInterface<T, TInterface>(iObj) ) ;
	
	[MethodImpl(_MAXOPT_)]
	public static bool IsCOMObject( in object obj ) => Marshal.IsComObject( obj ) ;
	[MethodImpl(_MAXOPT_)]
	public static bool IsCOMObject< T >( in T? obj ) {
		if( obj is null ) return false ;
		return IsCOMObject( obj ) ;
	}
	[MethodImpl(_MAXOPT_)]
	public static bool IsCOMObjectOfType< T >( nint ptr ) => 
		ptr.IsValid() && GetCOM_RCW( ptr ) is T ;
	
	
	
	//! TODO: Test this method ...
	[MethodImpl(_MAXOPT_)]
	public static bool IsCOMObjectOfType( nint ptr, Type t ) =>
		ptr.IsValid( ) && ( GetCOM_RCW(ptr)?.GetType() == t ) ;
	
	

	#region Interface Queries
		
	//! TODO: Marshal.GetComInterfaceForObject( pInterface, wrapper.ComType ) may simplify this ...

	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static HResult QueryInterface< TInterface >( nint pUnknown,
															out nint pInterface ) {
		var riid = typeof(TInterface).GUID ;
		var hr = (HResult)Marshal.QueryInterface( pUnknown, ref riid, out pInterface ) ;
		_lastHResult = hr ;
		return hr ;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static HResult QueryInterface< TInterface >( nint pUnknown,
														out TInterface comObj ) where TInterface: IUnknown {
		var hr = QueryInterface< TInterface >( pUnknown, out nint pInterface ) ;
		object? comObjectRef = Marshal.GetObjectForIUnknown( pInterface ) ;
		comObj = (TInterface)comObjectRef ;
		_lastHResult = hr ;
		return hr ;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static HResult QueryInterface< TWrapper, TInterface >( this TWrapper wrapper,
																	nint pUnknown, out TInterface comObj )
																		where TWrapper:
																			IUnknownWrapper< TWrapper, TInterface > 
																		where TInterface: IUnknown {
		if( wrapper is null ) throw new ArgumentNullException( nameof(wrapper) ) ;
		
		var riid = (wrapper.ComType).GUID ;
		var hr = (HResult)Marshal.QueryInterface( pUnknown, ref riid,
												  out var pInterface ) ;
		
		object comObjectRef = Marshal.GetObjectForIUnknown( pInterface ) ;
		comObj = (TInterface)comObjectRef ;
		
		_lastHResult = hr ;
		return hr ;
	}

	#endregion
	
	
	
	[MethodImpl(_MAXOPT_)]
	public static T? GetDXGIObject< T >( nint pUnknown )
		where T: class, IDXGIObject => GetCOM_RCW( pUnknown ) as T ;
	
	[MethodImpl(_MAXOPT_)]
	public static T? GetD3D12Object< T >( nint pUnknown )
		where T: class, ID3D12Object => GetCOM_RCW( pUnknown ) as T ;
		
	// ------------------------------------------------------------------------------
	// Extension Methods:
	// ------------------------------------------------------------------------------
	
	[MethodImpl(_MAXOPT_)] public static bool IsValid( this nint  ptr ) => ptr is not NULL_PTR ;
	[MethodImpl(_MAXOPT_)] public static bool IsValid( this nuint ptr ) => ptr is not 0x0000000000000000U ;
	
	// ==============================================================================
} ;