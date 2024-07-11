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


public static partial class COMUtility {
	#region Constant & ReadOnly Values
	/// <summary>COM IID values and utilities.</summary>
	public static class IIDs {
		public static readonly Guid IID_OF_IUnknown         = new( "00000000-0000-0000-C000-000000000046" ) ;
		
		// D3D12 Interfaces:
		public static readonly Guid IID_OF_ID3D12Object         = new( "C4FEC28F-7966-4E95-9F94-F431CB56C3B8" ) ;
		public static readonly Guid IID_OF_ID3D12DeviceChild    = new( "905DB94B-A00C-4140-9DF5-2B64CA9EA357" ) ;
		public static readonly Guid IID_OF_ID3D12Pageable       = new( "63ee58fb-1268-4835-86da-f008ce62f0d6" ) ;
		public static readonly Guid IID_OF_ID3D12RootSignature  = new( "C54A6B66-72DF-4EE8-8BE5-A946A1429214" ) ;
		public static readonly Guid IID_OF_ID3D12DescriptorHeap = new( "8efb471d-616c-4f49-90f7-127bb763fa51" ) ;
		public static readonly Guid IID_OF_ID3D12PipelineState  = new( "765A30F3-F624-4C6F-A828-ACE948622445" ) ;
		public static readonly Guid IID_OF_ID3D12Device         = new( "189819F1-1DB6-4B57-BE54-1821339B85F7" ) ;
		public static readonly Guid IID_OF_ID3D12CommandList    = new( "7116D91C-E7E4-47CE-B8C6-EC8168F437E5" ) ;
		public static readonly Guid IID_OF_ID3D12CommandQueue   = new( "0EC870A6-5D7E-4C22-8CFC-5BAAE07616ED" ) ;
		public static readonly Guid IID_OF_ID3D12Fence          = new( "0A753DCF-C4D8-4B91-ADF6-BE5A60D95A76" ) ;
		public static readonly Guid IID_OF_ID3D12Resource       = new( "696442BE-A72E-4059-BC79-5B5C98040FAD" ) ;
		public static readonly Guid IID_OF_ID3D12Heap           = new( "6B3B2502-6E51-45B3-90EE-9884265E8DF3" ) ;
		public static readonly Guid IID_OF_ID3D12GraphicsCommandList = new( "5B160D0F-AC1B-4185-8BA8-B3AE42A5A455" ) ;
		
		// DXGI Interfaces:
		public static readonly Guid IID_OF_IDXGIObject  = new("AEC22FB8-76F3-4639-9BE0-28EB43A67A2E") ;
		public static readonly Guid IID_OF_IDXGIFactory = new( "7B7166EC-21C7-44AE-B21A-C9AE321AE369" ) ;
		public static readonly Guid IID_OF_IDXGIAdapter = new( "2411E7E1-12AC-4CCF-BD14-9798E8534DC0" ) ;
		public static readonly Guid IID_OF_IDXGISwapChain = new( "310D36A0-D2E7-4C0A-AA04-6A9D23B8886A" ) ;
		public static readonly Guid IID_OF_IDXGIOutput  = new( "AE02EEDB-C735-4690-8D52-5A8DC20213AA" ) ;
		public static readonly Guid IID_OF_IDXGIDevice  = new( "54EC77FA-1377-44E6-8C32-88FD5F44C84C" ) ;
		public static readonly Guid IID_OF_IDXGIDeviceSubObject = new( "3D3E0379-F9DE-4D58-BB6C-18D62992F1A6" ) ;
		public static readonly Guid IID_OF_IDXGIDeviceChild  = new( "905DB94B-A00C-4140-9DF5-2B64CA9EA357" ) ;
		public static readonly Guid IID_OF_IDXGISurface  = new( "CAFCB56C-6AC3-4889-BF47-9E23BBD260EC" ) ;
		public static readonly Guid IID_OF_IDXGISurface1 = new( "4AE63092-6327-4C1B-80AE-BFE12EA32B86" ) ;
		public static readonly Guid IID_OF_IDXGIResource = new( "035F3AB4-482E-4E50-B41F-8A7F8BD8960B" ) ;
		public static readonly Guid IID_OF_IDXGIKeyedMutex = new( "9D8E1289-D7B3-465F-8126-250E349AF85D" ) ;
		 
		
		
		//! TODO: See if this works ...
		public static Guid IID_PPV_ARGS( in nint pUnknown ) {
			if( pUnknown is NULL_PTR ) return IID_OF_IUnknown ;
			var _rcw = Marshal.GetObjectForIUnknown( pUnknown ) ;
			if( _rcw is null ) return IID_OF_IUnknown ;
			return _rcw.GetType( ).GUID ;
		}
	}
	
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
	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
	public static bool Exists( nint pUnknown ) {
		bool valid =
			GetRCWObject( pUnknown ) is IUnknown ;
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
	
	[MethodImpl(_MAXOPT_)] public static int GetRefCount( nint pUnknown ) {
		try {
			if ( pUnknown is NULL_PTR ) return 0 ;
			int r = AddRef( pUnknown ) ;
			r = Release( pUnknown ) ;
			return r ;
		}
		catch { return 0 ; }
	}
	
	
	[MethodImpl(_MAXOPT_)] public static int AddRef( nint pUnknown ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		if( !pUnknown.IsValid() ) return NULL_PTR ;
#endif
		return Marshal.AddRef( pUnknown ) ;
	}
	[MethodImpl(_MAXOPT_)] public static int Release( nint pUnknown ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		if( !pUnknown.IsValid() ) return NULL_PTR ;
#endif
		return Marshal.Release( pUnknown ) ;
	}
	[MethodImpl(_MAXOPT_)] public static int Release( ref nint pUnknown ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		if( !pUnknown.IsValid() ) return NULL_PTR ;
#endif
		int c = Release( pUnknown ) ; 
		pUnknown = NULL_PTR ;
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
		var _rcw = ( Marshal.GetTypedObjectForIUnknown( pUnknown, type ) as IUnknown ) ;
		return _rcw ;
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
		/*var pUnknownForObject = Marshal.GetIUnknownForObject( iObj ) ;
		var hr = QueryInterface< T >( pUnknownForObject, out nint pInterface ) ;*/
		nint   addr = Marshal.GetComInterfaceForObject( iObj, typeof(T) ) ;
		return addr ;
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
	public static HResult QueryInterface< TInterface >( nint pUnknown, out TInterface comObj )
																		where TInterface: IUnknown {
		var hr = QueryInterface< TInterface >( pUnknown, out nint pInterface ) ;
		int count = Marshal.Release( pUnknown ) ;
		
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

	
	public static TOut? Cast< TIn, TOut >( TIn obj ) where TIn:  IDXCOMObject, IInstantiable
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
