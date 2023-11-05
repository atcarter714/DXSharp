#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp.Objects ;


[Wrapper(typeof(IUnknown))]
[Wrapper(typeof(IDXGIObject))]
[Wrapper(typeof(ID3D12Object))]
internal abstract class DXComObject: DisposableObject,
									 IDXCOMObject {
	//! ---------------------------------------------------------------------------------
	protected ComObject? ComResources { get ; set ; }
	protected internal void _initOrAdd<T>( ComPtr<T> ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}
	
	ComPtr? _comPtr ;
	
	public ComPtr? ComPtrBase => ComPointer ;
	public virtual ComPtr? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IUnknown >( )! ;
	
	public virtual IUnknown? COMObject =>
		(IUnknown)ComPointer?.InterfaceObjectRef! ;
	
	//! ---------------------------------------------------------------------------------
	
	//! IDisposable:
	protected override ValueTask DisposeUnmanaged( ) {
		_comPtr?.Dispose( ) ;
		return ValueTask.CompletedTask ;
	}

	protected override void Dispose( bool disposing ) {
		if( disposing ) DisposeManaged( ) ;
		DisposeUnmanaged( ) ;
		base.Dispose( disposing ) ;
	}

	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}
	
	
	// ----------------------------------------------------------------------------------
	
	public void GetPrivateData( in Guid name, ref uint pDataSize, nint pData = 0x0000 ) {
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
	
	public unsafe void SetPrivateData< TData >( in Guid name, uint DataSize, nint pData ) {
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
	
	public void SetPrivateDataInterface< T >( in Guid name, in T? pUnknown )
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

		hr.SetAsLastErrorForThread( ) ;
		
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

