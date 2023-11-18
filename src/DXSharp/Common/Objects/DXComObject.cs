#region Using Directives
using System.Diagnostics ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.System.Com ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;

using static DXSharp.InteropUtils ;
using IUnknown = DXSharp.Windows.COM.IUnknown ;
#endregion
namespace DXSharp ;


/// <summary>Wrapper for native DirectX COM object interfaces.</summary>
#if DEBUG || DEV_BUILD || DEBUG_COM
[DebuggerDisplay("Name: {Name}, Type: {ComType.Name}", 
				 Name = nameof(DXComObject))]
#endif

[Wrapper(typeof(IUnknown))]
[Wrapper(typeof(IDXGIObject))]
[Wrapper(typeof(ID3D12Object))]
internal abstract class DXComObject: DisposableObject,
									 IDXCOMObject {
	//! ---------------------------------------------------------------------------------
	const int MAX_NAME_BYTES = 512,
			  MAX_NAME_LEN   = (MAX_NAME_BYTES * sizeof(char)) ;
	
	//! ---------------------------------------------------------------------------------
	int _extraRefs, _lastMarshalRefCount ;
	
	/// <summary>The COM object management instance.</summary>
	protected COMResource? ComResources { get ; set ; }
	protected void _initOrAdd< T >( ComPtr<T> ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer( ptr ) ;
	}
	ComPtr? _comPtr ;
	
	public ComPtr? ComPtrBase => ComPointer ;
	public virtual ComPtr? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IUnknown >( )! ;
	
	[DebuggerBrowsable(DebuggerBrowsableState.Collapsed),
		DebuggerDisplay( "{}" )]
	public virtual IUnknown? ComObject =>
		(IUnknown)ComPointer?.InterfaceObjectRef! ;
	
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public string? Name {
		get => GetObjectName( ) ;
		set => SetObjectName( value ?? "?" ) ;
	}


#if DEBUG || DEV_BUILD || DEBUG_COM
	public string DebugDisplayStr => 
		$"Type: {ComType.Name} " +
		$"(Name: {(Name ?? ComType.GUID.ToString())})" ;
#endif
	
	//! ---------------------------------------------------------------------------------
	
	public HResult QueryInterface( in Guid riid, out nint ppvObject ) {
		unsafe {
			var vTableAddr = ComPtrBase?.BaseAddress ?? NULL_PTR ;
			if( vTableAddr is NULL_PTR ) {
				ppvObject = NULL_PTR ;
#if DEBUG || DEV_BUILD || DEBUG_COM
				throw new NullReferenceException($"{nameof(DXComObject)} :: " +
												 $"{nameof(ComPtrBase)} is null (no COM RCW object attached).") ;
#else
				return HResult.E_FAIL ;
#endif
			}
			
			var _qryInterfaceFn =
				( (delegate* unmanaged< IUnknownUnsafe*, Guid*, void**, HRESULT >)( *( *(void ***)vTableAddr + 0) ) ) ;
													/* +0 is for the IUnknown.QueryInterface slot */

			ppvObject = default ;
			HResult hr = default ;
			var thisInst = (IUnknownUnsafe *)vTableAddr ;
			fixed( void* _riid = &riid, _ppvObject = &ppvObject ) {
				++_extraRefs ;
				hr = _qryInterfaceFn( thisInst,
											(Guid *)_riid,
												(void **)_ppvObject ) ;
			}
			
			return hr ;
		}
	}

	public HResult QueryInterface< T >( out T? ppvUnk ) where T: IDXCOMObject, IInstantiable {
		var hr = QueryInterface( T.Guid, out var ppvObject ) ;
		var wrapper = ( T? )T.Instantiate( ppvObject ) ;
		if ( hr.Succeeded && wrapper is not null ) {
			ppvUnk = wrapper ;
			return hr ;
		}
		
		ppvUnk = default ;
		return hr ;
	}
	
	
	public uint AddRef( ) {
		unsafe {
			var vTableAddr = ComPtrBase!.BaseAddress ;
			var _marshalStyleCall =
				( (delegate* unmanaged< nint, uint >)( *( *(void ***)vTableAddr + 1) ) ) ;
													/* +1 is for the IUnknown.AddRef slot */
													
			uint rc = _marshalStyleCall( vTableAddr ) ;
			_lastMarshalRefCount = (int)rc ;
			++_extraRefs ;
			return rc ;
		}
	}
	
	public uint Release( ) {
		unsafe {
			var vTableAddr = ComPtrBase!.BaseAddress ;
			var _marshalStyleCall =
				( (delegate* unmanaged< nint, uint >)( *( *(void ***)vTableAddr + 2) ) ) ;
													/* +2 is for the IUnknown.Release slot */
			uint rc = _marshalStyleCall( vTableAddr ) ;
			_lastMarshalRefCount = (int)rc ;
			--_extraRefs ;
			return rc ;
		}
	}

	
	public HResult GetPrivateData( in Guid name, ref uint pDataSize, nint pData = 0x0000 ) {
		// Obtain interface references:
		var _name = name ;
		
		// Get the interface object:
		var comObj = this.ComPtrBase?.InterfaceObjectRef
#if DEBUG || DEV_BUILD || DEBUG_COM
					 ?? throw new ObjectDisposedException( nameof( this.ComPtrBase ) )
#endif
		 					 ;
		 
		// Get the data: ---------------------------------------------------------------
		HResult hr = default ;
		//! Is it a D3D12 object?
		if ( comObj is ID3D12Object d3d12Object ) {
			unsafe { hr = d3d12Object.GetPrivateData( &_name, ref pDataSize, (void*)pData ) ; }
		}
		//! Is it a DXGI object?
		else if ( comObj is IDXGIObject dxgiObject ) {
			unsafe { hr = dxgiObject.GetPrivateData( &_name, ref pDataSize, (void*)pData ) ; }
		}
		// ----------------------------------------------------------------------------------
		
		hr.SetAsLastErrorForThread( ) ;
		return hr ;
	}
	
	public unsafe HResult SetPrivateData( in Guid name, uint DataSize, nint pData ) {
		// Obtain interface references:
		HResult hr = default ;
		var _name = name ;
		
		// Get the interface object:
		var comObj = this.ComPtrBase?.InterfaceObjectRef
#if DEBUG || DEV_BUILD || DEBUG_COM
					 ?? throw new ObjectDisposedException( nameof( this.ComPtrBase ) )
#endif
		 					 ;
		
		// Set the data: ---------------------------------------------------------------
		
		//! Is it a D3D12 object?
		if ( comObj is ID3D12Object d3d12Object ) {
			unsafe { hr = d3d12Object.SetPrivateData( &_name, DataSize, (void*)pData ) ; }
		}
		//! Is it a DXGI object?
		else if ( comObj is IDXGIObject dxgiObject ) {
			unsafe { hr = dxgiObject.SetPrivateData( &_name, DataSize, (void*)pData ) ; }
		}
		hr.SetAsLastErrorForThread( ) ;
		return hr ;
	}
	
	public HResult SetPrivateDataInterface< T >( in Guid name, in T? pUnknown ) where T: IDXCOMObject {
		// Obtain interface references:
		HResult hr = default ;
		var _name = name ;
		
		// Get the interface object:
		var comObj = this.ComPtrBase?.InterfaceObjectRef
#if DEBUG || DEV_BUILD || DEBUG_COM
					 ?? throw new ObjectDisposedException( nameof( this.ComPtrBase ) )
#endif 
					 ;
		
		// Get the ref to the IUnknown interface:
		var dataObj = ( (IComObjectRef< IUnknown >?)pUnknown )?.ComObject
#if DEBUG || DEV_BUILD || DEBUG_COM
					  ?? throw new NullReferenceException( nameof( pUnknown ) )
#endif
					  ;
		
		// Set the data: ---------------------------------------------------------------
		
		//! Is it a D3D12 object?
		if ( comObj is ID3D12Object d3d12Object ) {
			unsafe { hr = d3d12Object.SetPrivateDataInterface( &_name, ( (IUnknown)dataObj! ?? null ) ) ; }
		}
		//! Is it a DXGI object?
		else if ( comObj is IDXGIObject dxgiObject ) {
			unsafe { hr = dxgiObject.SetPrivateDataInterface( &_name, dataObj ) ; }
		}
		hr.SetAsLastErrorForThread( ) ;
		return hr ;
	}

	
	// -------------------------------------------------
	// Extra Methods:
	// -------------------------------------------------
	
	public string GetObjectName( ) {
		using var pcwStr = _getDBGName( this ) ;
		string name = pcwStr?.ToString( ) ??
#if DEBUG || DEV_BUILD || DEBUG_COM
					  throw new NullReferenceException( $"{nameof(DXComObject)} ({ComType.Name}) :: " +
														$"Object name is null!" ) ;
#else
						  "?" ;
#endif
		return name ;
	}
	
	public void SetObjectName( string name ) {
		PCWSTR _name = name ;
		_setDBGName( _name, this ) ;
	}
	
	// ---------------------------------------------------------------------------------
	// Static methods:
	// ---------------------------------------------------------------------------------
	
	internal static unsafe void _setDBGName( in PCWSTR name, in IDXCOMObject obj ) =>
		obj.SetPrivateData( COMUtility.WKPDID_D3DDebugObjectName,
							(uint)name.Length * sizeof(char), (nint)name.Value ) ;
	
	internal static PCWSTR _setDBGName( string name, in IDXCOMObject obj ) {
		unsafe {
			PCWSTR _name = name ;
			obj.SetPrivateData( COMUtility.WKPDID_D3DDebugObjectName,
								(uint)name.Length * sizeof( char ),
								(nint)_name.Value ) ;
			return _name ;
		}
	}
	
	
	internal static PCWSTR? _getDBGName( in IDXCOMObject obj ) {
		unsafe {
			uint nameSize = 0 ;
			var hr = obj.GetPrivateData( COMUtility.WKPDID_D3DDebugObjectName, ref nameSize ) ;
			if ( nameSize < 1 || hr.Failed ) return null ;
			
			int nameLen = System.Math.Min( ((int)nameSize / sizeof(char)), 
												 MAX_NAME_LEN ) ;
			
			char* pName = stackalloc char[ nameLen + 1 ] ;
			pName[ nameLen ] = '\0' ; //! Set a null terminator
			
			hr = obj.GetPrivateData( COMUtility.WKPDID_D3DDebugObjectName,
									ref nameSize, (nint)pName ) ;
			if( hr.Failed || nameSize < 1 ) return null ;
			
			return PCWSTR.Create( pName, nameLen ) ;
		}
	}
	
	
	internal static bool _DestroyImmediate( in DXComObject obj ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ObjectDisposedException.ThrowIf( obj?.ComResources is null, 
										 nameof(obj.ComResources) ) ;
#endif
		
		var comRes   = obj.ComResources ;
		var refCount = comRes.TotalRefCount ;
		int relCount = comRes.FinalRelease( ) ;
		
		return (comRes?.Disposed ?? true) 
			   && (refCount == relCount) ;
	}
	
	//! ---------------------------------------------------------------------------------
	//! IDisposable:
	~DXComObject( ) => Dispose( false ) ;
	protected override ValueTask DisposeUnmanaged( ) {
		while( _extraRefs > 0 ) {
			Release( ) ;
		}
		if( !ComResources?.Disposed ?? false ) {
			ComResources.Dispose( ) ;
		}
		return ValueTask.CompletedTask ;
	}
	protected override void Dispose( bool disposing ) {
		base.Dispose( disposing ) ;
		if( disposing )
			DisposeManaged( ) ;
		
		/*var v = DisposeUnmanaged( ) ;
		v.GetAwaiter(  ).GetResult(  );*/
	}
	public override async ValueTask DisposeAsync( ) => await Task.Run( Dispose ) ;

	//! ---------------------------------------------------------------------------------
	public static Type ComType => typeof(IUnknown) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IUnknown).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// =================================================================================
} ;

