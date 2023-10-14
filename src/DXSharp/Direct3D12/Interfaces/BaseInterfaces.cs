#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;

#endregion
namespace DXSharp.Direct3D12 ;


// ----------------------------------------------------------

[Wrapper( typeof( ID3D12Object ) )]
public interface IObject: IDXCOMObject, 
						  IUnknownWrapper< ID3D12Object >, 
						  IComObjectRef< ID3D12Object > {
	
	/// <summary>Associates a name with the device object. This name is for use in debug diagnostics and tools.</summary>
	/// <param name="Name">
	/// <para>Type: <b>LPCWSTR</b> A <b>NULL</b>-terminated <b>UNICODE</b> string that contains the name to associate with the device object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12object-setname#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method takes UNICODE names. Note that this is simply a convenience wrapper around [ID3D12Object::SetPrivateData](nf-d3d12-id3d12object-setprivatedata.md) with **WKPDID_D3DDebugObjectNameW**. Therefore names which are set with `SetName` can be retrieved with [ID3D12Object::GetPrivateData](nf-d3d12-id3d12object-getprivatedata.md) with the same GUID. Additionally, D3D12 supports narrow strings for names, using the **WKPDID_D3DDebugObjectName** GUID directly instead.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12object-setname#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetName( string Name ) =>
			((ID3D12Object)ComPtrBase?.InterfaceObjectRef!)
				.SetName( Name ) ;
	
	// ----------------------------------------------------------
	// Explicit Interface Implementations / Disambiguation ::
	// ----------------------------------------------------------
	
	Type IUnknownWrapper< ID3D12Object >.ComType => ComType ;
	nint IUnknownWrapper< ID3D12Object >.Pointer => Pointer ;
	ID3D12Object? IComObjectRef< ID3D12Object >.COMObject => COMObject ;
	ID3D12Object? IUnknownWrapper< ID3D12Object >.ComObject => ComObject ;
	bool IUnknownWrapper< ID3D12Object >.IsInitialized => IsInitialized ;
	
	static Guid IUnknownWrapper< ID3D12Object >.InterfaceGUID => 
		typeof(ID3D12Object).GUID ;
	
	// ============================================================
} ;


// ----------------------------------------------------------


[Wrapper( typeof( ID3D12DeviceChild ) )]
public interface IDeviceChild: IObject,
							   IComObjectRef< ID3D12DeviceChild >,
							   IUnknownWrapper< ID3D12DeviceChild > {
	/// <summary>Indicates if this instance is fully initialized.</summary>
	new bool IsInitialized => ( !ComPointer?.Disposed ) ?? false ;
	
	
	/// <summary>Gets a pointer to the device that created this interface.</summary>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the device interface. The <b>REFIID</b>, or <b>GUID</b>, of the interface to the device can be obtained by using the __uuidof() macro. For example, __uuidof(<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12device">ID3D12Device</a>) will get the <b>GUID</b> of the interface to a device.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12devicechild-getdevice#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvDevice">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12device">ID3D12Device</a> interface for the device.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12devicechild-getdevice#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>Any returned interfaces have their reference count incremented by one, so be sure to call ::release() on the returned pointers before they are freed or else you will have a memory leak.</remarks>
	void GetDevice( in Guid riid, out IDevice ppvDevice ) {
		unsafe {
			Guid _riid = riid ;
			( (ID3D12DeviceChild)ComPtrBase?.InterfaceObjectRef! )
				.GetDevice( &_riid, out var _ppvDevice ) ;
			ppvDevice = (IDevice)_ppvDevice ;
		}
	}
	
	
	internal new Type ComType => typeof(ID3D12DeviceChild) ;
	new ID3D12DeviceChild? COMObject { get ; }
	new ComPtr< ID3D12DeviceChild >? ComPointer { get ; }
	internal new nint Pointer => ComPointer?.BaseAddress ?? nint.Zero ;
	internal new ID3D12DeviceChild? ComObject => ComPointer!.Interface ;
	
	bool IUnknownWrapper< ID3D12DeviceChild >.IsInitialized => IsInitialized ;
	Type IUnknownWrapper< ID3D12DeviceChild >.ComType => ComType ;
	nint IUnknownWrapper< ID3D12DeviceChild >.Pointer => Pointer ;
	ID3D12DeviceChild? IUnknownWrapper< ID3D12DeviceChild >.ComObject => ComObject ;
	
	ID3D12DeviceChild? IComObjectRef< ID3D12DeviceChild >.COMObject => COMObject ;
	
	static Guid IUnknownWrapper< ID3D12DeviceChild >.InterfaceGUID =>
		typeof(ID3D12DeviceChild).GUID ;
	
	/*static HResult IUnknownWrapper< ID3D12DeviceChild >.QueryInterface<T>(
		in IUnknownWrapper<T> wrapper, out T obj ) =>
		COMUtility.QueryInterface( wrapper.BasePointer, out obj ) ;*/
} ;


// ----------------------------------------------------------


[Wrapper( typeof( ID3D12Pageable ) )]
public interface IPageable: IDeviceChild,
							IComObjectRef< ID3D12Pageable >, 
							IUnknownWrapper< ID3D12Pageable > {
	new ID3D12Pageable? COMObject { get ; }
	new ComPtr< ID3D12Pageable >? ComPointer { get ; }
	
	ID3D12DeviceChild? IDeviceChild.ComObject => COMObject ;
	ID3D12DeviceChild? IDeviceChild.COMObject => COMObject ;
	ID3D12Pageable? IUnknownWrapper< ID3D12Pageable >.ComObject => COMObject ;
	ID3D12DeviceChild? IUnknownWrapper< ID3D12DeviceChild >.ComObject => COMObject ;
	
	
	static Guid IUnknownWrapper< ID3D12Object >.InterfaceGUID => typeof(ID3D12Object).GUID ;
	static Guid IUnknownWrapper< ID3D12Pageable >.InterfaceGUID => typeof(ID3D12Pageable).GUID ;
	static Guid IUnknownWrapper< ID3D12DeviceChild >.InterfaceGUID => typeof(ID3D12DeviceChild).GUID ;
	
} ;


// ----------------------------------------------------------


	
// ---------------------------------------------------------------------
// IUnknownWrapper< ID3D12Pageable > Explicits/Disambiguations ::
// ---------------------------------------------------------------------
/*Type IUnknownWrapper<ID3D12Pageable>.ComType => ComType ;
nint IUnknownWrapper<ID3D12Pageable>.Pointer => Pointer ;
bool IUnknownWrapper<ID3D12Pageable>.IsInitialized => IsInitialized ;*/
// =====================================================================

	
/*static HResult IUnknownWrapper<ID3D12DeviceChild>
	.QueryInterface<T>(in IUnknownWrapper<T> wrapper, out T obj) =>
		COMUtility.QueryInterface( wrapper.BasePointer, out obj ) ;*/

/*bool IDeviceChild.IsInitialized => IsInitialized ;
bool IUnknownWrapper< ID3D12Pageable >.IsInitialized => IsInitialized ;*/
	
/*Type IDeviceChild.ComType => ComType ;
Type IUnknownWrapper< ID3D12Pageable >.ComType => ComType ;*/
	
/*nint IDeviceChild.Pointer => Pointer ;
nint IUnknownWrapper< ID3D12Pageable >.Pointer => Pointer ;
nint IUnknownWrapper< ID3D12DeviceChild >.Pointer => Pointer ;*/

	
/*bool IUnknownWrapper< ID3D12Object >.IsInitialized => IsInitialized ;
Type IUnknownWrapper< ID3D12Object >.ComType => ComType ;
nint IUnknownWrapper< ID3D12Object >.Pointer => Pointer ;
ID3D12Object? IUnknownWrapper< ID3D12Object >.ComObject => ComObject ;*/
	
	
/**/
	
	
/*static Guid IUnknownWrapper< ID3D12Object >.InterfaceGUID =>
	typeof(ID3D12Object).GUID ;
static Guid IUnknownWrapper< ID3D12DeviceChild >.InterfaceGUID =>
	typeof(ID3D12DeviceChild).GUID ;

static HResult IUnknownWrapper< ID3D12Object >.QueryInterface<T>(
	in IUnknownWrapper<T> wrapper, out T obj ) =>
	COMUtility.QueryInterface( wrapper.BasePointer, out obj ) ;

static HResult IUnknownWrapper< ID3D12DeviceChild >.QueryInterface<T>(
	in IUnknownWrapper<T> wrapper, out T obj ) =>
	COMUtility.QueryInterface( wrapper.BasePointer, out obj ) ;*/