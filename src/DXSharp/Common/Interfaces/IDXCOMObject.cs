#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;

using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp ;


/*public interface IComObjectRef { nint PointerToIUnknown { get; } } ;*/
public interface IComObjectRef< T >
	where T: IUnknown {
	/// <summary>A reference to the native COM object interface.</summary>
	T? COMObject { get; }
} ;

public interface IInstantiable {
	public static abstract IDXCOMObject Instantiate( ) ;
	public static abstract IDXCOMObject Instantiate( nint pComObj ) ;
	public static abstract IDXCOMObject Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? ;
} ;

public interface IInstantiable< T >: IInstantiable
	where T: class, IInstantiable< T > {
	new static abstract T? Instantiate( nint ptr ) ;
} ;


//! TODO: Remove all the exhaustive safety checks after testing and merge into single execution path. (i.e, no branching)
/// <summary>Contract for the common COM interface shared by objects in DirectX.</summary>
public interface IDXCOMObject: IUnknownWrapper {
	
	/// <summary>Get a pointer to the object's data.</summary>
	/// <param name="name">User-defined name or GUID to associate with the data.</param>
	/// <param name="pDataSize">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> The size of the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>void*</b> Pointer to the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the following <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetPrivateData( in Guid name, ref uint pDataSize, nint pData = 0x0000 ) {
		HResult hr = HResult.E_FAIL ;
		try {
			Guid _name       = name ;
			uint _sizeResult = 0U ;
			bool _sizeCheck  = pData is NULL_PTR || !pData.IsValid( ) ;
			var comObj = this.ComPtrBase?.InterfaceObjectRef
						 ?? throw new ObjectDisposedException( nameof( this.ComPtrBase ) ) ;
			var underlyingType = comObj.GetType( ) ;

#if DEBUG || DEV_BUILD || DEBUG_COM //! Development safety/sanity checks (can remove after testing)
			bool isValid = comObj.GetType( ).IsCOMObject ;
			if ( !isValid ) {
				throw new DXSharpException( $"{nameof( IDXCOMObject )}.{nameof( GetPrivateData )}" +
											$"( {nameof( pDataSize )}, {nameof( pData )} ) :: " +
											$"The backing field of the internal " +
											$"{nameof( ComPtrBase )}.{nameof( ComPtrBase.InterfaceObjectRef )}'s " +
											$"internal property is not a valid COM object reference! \n" +
											$"(Invalid Type: {underlyingType.Name}, Guid: {underlyingType.GUID})" ) ;
			}
#endif

			// Request the size of the data?
			if ( _sizeCheck ) { pDataSize = _fetchDataSize( _name, comObj ) ; return ; }

			// Retrieve the data:
			bool _ok = false ;
			if ( comObj is IDXGIObject dxgiObject ) {
				unsafe { hr = dxgiObject.GetPrivateData( &_name, ref _sizeResult, (void*)pData ) ; }
				_ok = true ;
			}

			// Is it a D3D12 object?
			else if ( comObj is ID3D12Object d3d12Object ) {
				unsafe { hr = d3d12Object.GetPrivateData( &_name, ref _sizeResult, (void*)pData ) ; }
				_ok = true ;
			}
			
#if DEBUG || DEV_BUILD || DEBUG_COM //! Development safety/sanity checks (can remove after testing)
			if ( !_ok ) {
				throw new
					DXSharpException( $"{nameof( IDXCOMObject )}.{nameof( GetPrivateData )}" +
									  $"( {nameof( name )}, {nameof( pDataSize )}, {nameof( pData )} ) :: " +
									  $"The internal {nameof( ComPtrBase )}.{nameof( ComPtrBase.InterfaceObjectRef )} " +
									  $"is not a valid DXGI or D3D12 object!\n" + $"(Name: {underlyingType.Name}, GUID: {underlyingType.GUID})" ) ;
			}
#endif
			
			pDataSize = _sizeResult ;
		}

		finally { hr.SetAsLastErrorForThread( ) ; }

		//! Local function to get data size:
		unsafe uint _fetchDataSize( Guid _guid, object obj ) {
			bool _ok = false ;
			uint _dataSize = 0U ;
			HResult _hr = HResult.E_FAIL;
			
			// Is it a DXGI object?
			if ( obj is IDXGIObject _dxgiObj ) {
				_hr = _dxgiObj.GetPrivateData( &_guid, ref _dataSize, null ) ;
				_ok = true ;
			}

			// Is it a D3D12 object?
			else if ( obj is ID3D12Object d3d12Object ) {
				_hr = d3d12Object.GetPrivateData( &_guid, ref _dataSize, null ) ;
				_ok = true ;
			}
			
			_hr.SetAsLastErrorForThread( ) ;
			
#if DEBUG || DEV_BUILD || DEBUG_COM //! Development safety/sanity checks (can remove after testing)
			if ( !_ok ) {
				throw new DXSharpException( $"{nameof( IDXCOMObject )}.{nameof( GetPrivateData )}" +
											$"( {nameof( pDataSize )}, {nameof( pData )} ) :: " +
											$"The internal {nameof( ComPtrBase )}.{nameof( ComPtrBase.InterfaceObjectRef )} " +
											$"is not a valid DXGI or D3D12 object!\n" +
											$"(Name: {obj.GetType( ).Name}, GUID: {obj.GetType( ).GUID})" ) ;
			}
#endif
			return _dataSize ;
		}
	}
	
	/// <summary><para>
	/// Set data to an object's private data storage and associate it with a custom user-defined GUID
	/// (used to retrieve it with <see cref="GetPrivateData"/>).
	/// </para></summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> A GUID that identifies the data.Use this GUID in a call to<a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiobject-getprivatedata"> GetPrivateData</a> to get the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// <typeparam name="TData">The type of the data being set on the COM interface.</typeparam>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void SetPrivateData< TData >( in Guid name, uint DataSize, nint pData ) {
		HResult hr = default ;
		var guid = typeof(TData).GUID ;
		bool dataIsCOMObject = typeof(TData).IsCOMObject ;
		var comObj = this.ComPtrBase?.InterfaceObjectRef 
					 ?? throw new ObjectDisposedException( nameof(this.ComPtrBase) ) ;
		var underlyingType = comObj.GetType( ) ;
		
#if DEBUG || DEV_BUILD || DEBUG_COM //! Development safety/sanity checks (can remove after testing)
		bool isValid = comObj.GetType( ).IsCOMObject ;
		if( !isValid ) throw new DXSharpException( $"{nameof(IDXCOMObject)}.{nameof(SetPrivateData)}<{nameof(TData)}>" +
												   $"( {nameof(DataSize)}, {nameof(pData)} ) :: " +
												   $"The backing field of the internal " +
												   $"{nameof(ComPtrBase)}.{nameof(ComPtrBase.InterfaceObjectRef)}'s " +
												   $"internal property is not a valid COM object reference! \n" +
												   $"(Invalid Type: {underlyingType.Name}, Guid: {underlyingType.GUID})" ) ;
#endif
		
		// Set the data:
		bool _ok = false ;
		if ( comObj is IDXGIObject dxgiObject ) { 
			hr = dxgiObject.SetPrivateData( &guid, DataSize, (void *)pData ) ;
			_ok         = true ;
		}
		// Is it a D3D12 object?
		else if ( comObj is ID3D12Object d3d12Object ) { 
			hr = d3d12Object.SetPrivateData( &guid, DataSize, (void *)pData ) ;
			_ok         = true ;
		}
		
#if DEBUG || DEV_BUILD || DEBUG_COM //! Development safety/sanity checks (can remove after testing)
		if( !_ok ) throw new DXSharpException( $"{nameof(IDXCOMObject)}.{nameof(SetPrivateData)}<{nameof(TData)}>" +
											   $"( {nameof(DataSize)}, {nameof(pData)} ) :: " +
											   $"The internal {nameof(ComPtrBase)}.{nameof(ComPtrBase.InterfaceObjectRef)} " +
											   $"is not a valid DXGI or D3D12 object!\n" +
											   $"(Name: {underlyingType.Name}, GUID: {underlyingType.GUID})" ) ;
#endif
		hr.SetAsLastErrorForThread( ) ;
	}
	
	/// <summary>Set an interface in the object's private data.</summary>
	/// <param name="name">User-defined name or GUID to associate with the data.</param>
	/// <param name="pUnknown">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> The interface to set.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the following <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetPrivateDataInterface< T >( in Guid name, in T? pUnknown )
													where T: IUnknownWrapper {
		HResult hr = default ;
		var _name = name ;
		bool dataIsCOMObject = typeof(T).IsCOMObject ;
		
		var comObj = this.ComPtrBase?.InterfaceObjectRef
					 ?? throw new ObjectDisposedException( nameof( this.ComPtrBase ) ) ;
		var underlyingType = comObj.GetType( ) ;
		
#if DEBUG || DEV_BUILD || DEBUG_COM //! Development safety/sanity checks (can remove after testing)
		bool isValid = comObj.GetType( ).IsCOMObject ;
		if( !isValid ) {
			throw new DXSharpException( $"{nameof( IDXCOMObject )}.{nameof( SetPrivateDataInterface )}<{nameof(T)}>" +
										$"( {nameof( pUnknown )} ) :: " +
										$"The backing field of the internal " +
										$"{nameof( ComPtrBase )}.{nameof( ComPtrBase.InterfaceObjectRef )}'s " +
										$"internal property is not a valid COM object reference! \n" +
										$"(Invalid Type: {underlyingType.Name}, Guid: {underlyingType.GUID})" ) ;
		}
#endif
		
		// Get the pointer to the interface data:
		// (NOTE: Null pointers/objects are valid - used to clear data)
		var dataObj = pUnknown?.ComPtrBase?.InterfaceObjectRef ;
		
		// Set the data:
		bool _ok = false ;
		if ( comObj is IDXGIObject dxgiObject ) {
			unsafe { hr = dxgiObject.SetPrivateDataInterface( &_name, ( (IUnknown)dataObj! ?? null ) ) ; }
			_ok = true ;
		}
		
		// Is it a D3D12 object?
		else if ( comObj is ID3D12Object d3d12Object ) {
			unsafe { hr = d3d12Object.SetPrivateDataInterface( &_name, ( (IUnknown)dataObj! ?? null ) ) ; }
			_ok = true ;
		}
		
#if DEBUG || DEV_BUILD || DEBUG_COM //! Development safety/sanity checks (can remove after testing)
		if( !_ok ) {
			throw new DXSharpException( $"{nameof( IDXCOMObject )}.{nameof( SetPrivateDataInterface )}<{nameof(T)}>" +
										$"( {nameof( pUnknown )} ) :: " +
										$"The internal {nameof( ComPtrBase )}.{nameof( ComPtrBase.InterfaceObjectRef )} " +
										$"is not a valid DXGI or D3D12 object!\n" +
										$"(Name: {underlyingType.Name}, GUID: {underlyingType.GUID})" ) ;
		}
#endif
	}
} ;
