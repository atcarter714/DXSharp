#region Using Directives
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;

using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp.Windows.COM ;


public static class COMUtility {
	#region Constant & ReadOnly Values
	public static readonly Guid DXGI_DEBUG_ALL = new Guid(0xe48ae283, 0xda80, 0x490b, 0x87, 0xe6, 0x43, 
														  0xe9, 0xa9, 0xcf, 0xda, 0x8) ;
	public static readonly Guid DXGI_DEBUG_DXGI = new Guid(0x25cddaa4, 0xb1c6, 0x47e1, 0xac, 0x3e, 0x98, 
														   0x87, 0x5b, 0x5a, 0x2e, 0x2a) ;
	
	public static readonly Guid WKPDID_D3DDebugObjectName = new( 0x429b8c22, 0x9188, 0x4b0c, 0x87, 0x42,
																 0xac, 0xb0, 0xbf, 0x85, 0xc2, 0x00 ) ;
	public static readonly Guid WKPDID_D3DDebugObjectNameW = new( 0x4cca5fd8, 0x921f, 0x42c8, 0x85, 0x66,
																  0x70, 0xca, 0xf2, 0xa9, 0xb7, 0x41 ) ;
	public static readonly Guid WKPDID_CommentStringW = new( 0xd0149dc0, 0x90e8, 0x4ec8, 0x81, 0x44,
															 0xe9, 0x00, 0xad, 0x26, 0x6b, 0xb2 ) ;
	public static readonly Guid WKPDID_D3D12UniqueObjectId = new( 0x1b39de15, 0xec04, 0x4bae, 0xba, 0x4d,
																  0x8c, 0xef, 0x79, 0xfc, 0x04, 0xc1 ) ;

	public static readonly Guid D3D_TEXTURE_LAYOUT_ROW_MAJOR = new( 0xb5dc234f, 0x72bb, 0x4bec, 0x97, 0x05,
																	0x8c, 0xf2, 0x58, 0xdf, 0x6b, 0x6c ) ;
	
	public static readonly Guid D3D_TEXTURE_LAYOUT_64KB_STANDARD_SWIZZLE = new( 0x4c0f29e3, 0x3f5f, 0x4d35,
																			  0x84, 0xc9, 0xbc, 0x09, 0x83,
																			  0xb6, 0x2c, 0x28 ) ;
	
	#endregion
	
	
	static HResult _lastHResult = HResult.S_OK ;
	internal static HResult LastHResult => _lastHResult ;
	
	
	//! TODO: Decide which of these versions (Exists vs IsDestroyed) to keep ...
	[MethodImpl(_MAXOPT_)] public static bool Exists( nint pUnknown ) {
		bool valid = 
			GetRCWObject( pUnknown ) is not null ;
		if( valid ) Release( pUnknown ) ;
		return valid ;
	}
	
	[MethodImpl(_MAXOPT_)] public static bool IsDestroyed( nint pUnknown ) {
		try {
			if( pUnknown is NULL_PTR ) return true ;
			int r = AddRef( pUnknown ) ;
			if( r is 0 ) return true ;
			Release( pUnknown ) ;
			return false ;
		}
		catch { return true ; }
	}
	
	[MethodImpl(_MAXOPT_)] public static nint GetIUnknownForObject( [MaybeNull] in object? obj ) {
		if( obj is null ) return NULL_PTR ;
		nint pIUnknownForObject = Marshal.GetIUnknownForObject( obj ) ;
		return pIUnknownForObject ;
	}
	
	
	[MethodImpl(_MAXOPT_)] public static int AddRef( nint pUnknown ) {
		if( pUnknown is NULL_PTR ) return 0 ;
		return Marshal.AddRef( pUnknown ) ;
	}
	[MethodImpl(_MAXOPT_)] public static int Release( nint pUnknown ) {
		if( !pUnknown.IsValid() ) return NULL_PTR ;
		return Marshal.Release( pUnknown ) ;
	}
	[MethodImpl(_MAXOPT_)] public static int Release( ref nint pUnknown ) {
		int c = Release( pUnknown ) ; pUnknown = NULL_PTR ;
		return c ;
	}
	
	
	
	[MethodImpl(_MAXOPT_)]
	public static object? GetRCWObject( nint pUnknown ) =>
		!pUnknown.IsValid( ) ? null : Marshal.GetObjectForIUnknown( pUnknown ) ;


	[MethodImpl( _MAXOPT_ )]
	public static T? GetCOMObject< T >( nint pUnknown )
										where T: IUnknown {
		var guid = __uuidof<T>( ) ;
		/*var hr = Marshal.QueryInterface( pUnknown, ref guid,
									out nint ptrToInterface ) ;
		_lastHResult = new( hr ) ;*/
		
		var _interface = (T?)GetCOMObject( typeof(T), pUnknown ) ;
		return _interface ;
	}

	public static IUnknown? GetCOMObject( Type type, nint pUnknown ) {
		
		var guid = type.GUID ;
		
		var _interface = Marshal
			.GetTypedObjectForIUnknown( pUnknown, type ) ;
		return ( IUnknown )_interface ;
	}

	
	
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
	public static bool TryGetBaseAddress( object? obj, out nint ptr ) {
		ptr = NULL_PTR ;
		if( obj is null ) return false ;
		ptr = Marshal.GetIUnknownForObject( obj ) ;
		if( ptr is NULL_PTR ) return false ;
		return true ;
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
	public static bool IsCOMObject< T >( in T? obj ) => 
		typeof(T).IsCOMObject && obj is not null && Marshal.IsComObject( obj ) ;
	[MethodImpl(_MAXOPT_)]
	public static bool IsCOMObjectOfType< T >( nint ptr ) => 
		ptr.IsValid() && GetRCWObject( ptr ) is T ;
	
	
	//! TODO: Test this method ...
	[MethodImpl(_MAXOPT_)]
	public static bool IsCOMObjectOfType( nint ptr, Type t ) =>
		ptr.IsValid( ) && ( GetRCWObject(ptr)?.GetType() == t ) ;
	
	

	#region Interface Queries
		
	//! TODO: Marshal.GetComInterfaceForObject( pInterface, wrapper.ComType ) may simplify this ...
	[MethodImpl( MethodImplOptions.AggressiveOptimization )]
	public static nint GetInterfaceVPointer< T, I >( [NotNull] T comObj ) =>
		Marshal.GetComInterfaceForObject< T, I >( comObj ?? throw new ArgumentNullException(nameof(comObj)) ) ;
	
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static HResult QueryInterface< TInterface >( nint pUnknown,
															out nint pInterface ) {
		var riid = typeof(TInterface).GUID ;
		var hr = (HResult)Marshal.QueryInterface( pUnknown, ref riid, out pInterface ) ;
		hr.SetAsLastErrorForThread( ) ;
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
	
	#endregion
	
	
	
	[MethodImpl(_MAXOPT_)] public static T? GetDXGIObject< T >( nint pUnknown )
		where T: class, IDXGIObject => GetRCWObject( pUnknown ) as T ;
	
	[MethodImpl(_MAXOPT_)] public static T? GetD3D12Object< T >( nint pUnknown )
		where T: class, ID3D12Object => GetRCWObject( pUnknown ) as T ;

	
	public static TOut Cast< TIn, TOut >( TIn obj ) where TIn:  IDXCOMObject, IInstantiable
													where TOut: TIn {
		var _wrapper = (IUnknownWrapper)obj ;
		var _other = (TOut)TOut.Instantiate( _wrapper.BasePointer ) ;
		return _other ;
	}
	
	// ------------------------------------------------------------------------------
	// Extension Methods:
	// ------------------------------------------------------------------------------
	
	[MethodImpl(_MAXOPT_)] public static bool IsValid( this nint  ptr ) => ptr is not NULL_PTR ;
	[MethodImpl(_MAXOPT_)] public static bool IsValid( this nuint ptr ) => ptr is not 0x0000000000000000U ;
	
	// ==============================================================================
} ;



/* Definitions from C/C++ Headers:
DEFINE_GUID( WKPDID_D3DDebugObjectName,0x429b8c22,0x9188,0x4b0c,0x87,0x42,0xac,0xb0,0xbf,0x85,0xc2,0x00 ) ;
DEFINE_GUID( WKPDID_D3DDebugObjectNameW,0x4cca5fd8,0x921f,0x42c8,0x85,0x66,0x70,0xca,0xf2,0xa9,0xb7,0x41) ;
DEFINE_GUID( WKPDID_CommentStringW,0xd0149dc0,0x90e8,0x4ec8,0x81, 0x44, 0xe9, 0x00, 0xad, 0x26, 0x6b, 0xb2 ) ;
DEFINE_GUID( WKPDID_D3D12UniqueObjectId, 0x1b39de15, 0xec04, 0x4bae, 0xba, 0x4d, 0x8c, 0xef, 0x79, 0xfc, 0x04, 0xc1 ) ;

DEFINE_GUID(D3D_TEXTURE_LAYOUT_ROW_MAJOR,0xb5dc234f,0x72bb,0x4bec,0x97,0x05,0x8c,0xf2,0x58,0xdf,0x6b,0x6c);
DEFINE_GUID(D3D_TEXTURE_LAYOUT_64KB_STANDARD_SWIZZLE,0x4c0f29e3,0x3f5f,0x4d35,0x84,0xc9,0xbc,0x09,0x83,0xb6,0x2c,0x28);
*/
