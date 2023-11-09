#region Using Directives
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


[Wrapper(typeof(IUnknown))]
[Wrapper(typeof(IDXGIObject))]
[Wrapper(typeof(ID3D12Object))]
internal abstract class DXComObject: DisposableObject,
									 IDXCOMObject {
	//! ---------------------------------------------------------------------------------
	/// <summary>The COM object management instance.</summary>
	protected COMResource? ComResources { get ; set ; }
	protected internal void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}
	
	ComPtr? _comPtr ;
	
	public ComPtr? ComPtrBase => ComPointer ;
	public virtual ComPtr? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IUnknown >( )! ;
	
	public virtual IUnknown? ComObject =>
		(IUnknown)ComPointer?.InterfaceObjectRef! ;
	
	//! ---------------------------------------------------------------------------------

	public HResult QueryInterface( in Guid riid, out nint ppvObject ) {
		unsafe {
			var vTableAddr = ComPtrBase?.BaseAddress ?? NULL_PTR ;
			if( vTableAddr is NULL_PTR ) {
				ppvObject = 0x00000000 ;
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

			var thisInst = (IUnknownUnsafe *)vTableAddr ;
			fixed( void* _riid = &riid, _ppvObject = &ppvObject ) {
				return _qryInterfaceFn( thisInst,
											(Guid *)_riid,
												(void **)_ppvObject ) ;
			}
		}
	}

	public HResult QueryInterface< T >( out T ppvUnk ) where T: IDXCOMObject, IInstantiable {
		unsafe {
			var vTableAddr = ComPtrBase?.BaseAddress ?? NULL_PTR ;
			if( vTableAddr is NULL_PTR ) {
				ppvUnk = default ;
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

			var thisInst = (IUnknownUnsafe *)vTableAddr ;
			var _riid = typeof(T).GUID ;
			var _ppvObject = default( void* ) ;
			var hr = _qryInterfaceFn( thisInst,
											(Guid *)&_riid,
												(void **)&_ppvObject ) ;
			
			ppvUnk = (T)T.Instantiate( (nint)_ppvObject ) ;
			return hr ;
		}
	}

	public uint AddRef( ) {
		unsafe {
			var vTableAddr = ComPtrBase!.BaseAddress ;
			var _marshalStyleCall =
				( (delegate* unmanaged< nint, uint >)( *( *(void ***)vTableAddr + 1) ) ) ;
													/* +1 is for the IUnknown.AddRef slot */
			return _marshalStyleCall( vTableAddr ) ;
		}
	}
	
	public uint Release( ) {
		unsafe {
			var vTableAddr = ComPtrBase!.BaseAddress ;
			var _marshalStyleCall =
				( (delegate* unmanaged< nint, uint >)( *( *(void ***)vTableAddr + 2) ) ) ;
													/* +2 is for the IUnknown.Release slot */
			return _marshalStyleCall( vTableAddr ) ;
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
			unsafe { hr = dxgiObject.SetPrivateDataInterface( &_name, ( (IUnknown)dataObj! ?? null ) ) ; }
		}
		hr.SetAsLastErrorForThread( ) ;
		return hr ;
	}
	
	
	
	//! ---------------------------------------------------------------------------------
	//! IDisposable:
	~DXComObject( ) => Dispose( false ) ;
	protected override async ValueTask DisposeUnmanaged( ) {
		if( _comPtr is not null )
			await _comPtr.DisposeAsync( ) ;
	}
	protected override void Dispose( bool disposing ) {
		base.Dispose( disposing ) ;
		if( disposing ) 
			DisposeManaged( ) ;
		
#pragma warning disable CS4014
		DisposeUnmanaged( ) ;
#pragma warning restore CS4014
	}
	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}
	
	
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

