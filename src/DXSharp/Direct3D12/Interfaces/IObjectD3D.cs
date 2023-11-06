#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof(ID3D12Object) )]
public interface IObject: IDXCOMObject {
	// ---------------------------------------------------------------------------------
	
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
	void SetName( string Name ) ;

#if DEBUG || DEBUG_COM || DEV_BUILD
	/// <summary>A debug name (<see cref="string"/>) for this COM interface.</summary>
	string DebugName { get ; }
	
	/// <summary>Gets the debug name of this COM interface.</summary>
	/// <returns>
	/// <para>Type: <b>string</b> The debug name of this COM interface.</para>
	/// </returns>
	string GetDebugName( ) ;
#endif
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12Object) ;
	public new static Guid IID => (ComType.GUID) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Object).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==================================================================================
} ;






	
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