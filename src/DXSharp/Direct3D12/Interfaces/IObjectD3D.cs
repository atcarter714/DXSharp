#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>
/// The common interface from which D3D12 COM objects such as <see cref="IDevice"/>
/// and <see cref="IDeviceChild"/> inherit from. It provides methods to associate
/// private data and annotate object names.
/// </summary>
/// <remarks>
/// <para>The native <see cref="ID3D12Object"/> interface inherits from the <see cref="IUnknown"/> interface.
/// Likewise, this interface inherits from <see cref="IDXCOMObject"/>, which represents a proxy for an
/// <see cref="IUnknown"/> interface.</para>
/// <para>
/// <b>NOTE:</b> You can learn more about the
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/nn-d3d12-id3d12object">ID3D12Object</a>
/// interface in the official
/// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3d12/direct3d-12-graphics">Direct3D 12 documentation</a> ...
/// </para>
/// </remarks>
[ProxyFor( typeof(ID3D12Object), nameof(IObject), "D3D12" ),
 NativeLibrary("d3d12.dll", nameof(ID3D12Object), "d3d12.h"), ]
public interface IObject: IDXCOMObject {
	// ---------------------------------------------------------------------------------

	/// <summary>Associates a name with the device object. This name is for use in debug diagnostics and tools.</summary>
	/// <param name="name">
	/// <para>Type: <b>LPCWSTR</b>
	/// A <b>NULL</b>-terminated <b>UNICODE</b> string that contains the name to associate with the device object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12object-setname#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>
	/// Type: <see cref="HResult"/>
	/// (see: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT documentation</a> for more info</b>)
	/// This method returns one of the
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.
	/// </para>
	/// </returns>
	/// <remarks>
	/// <para>This method takes <b>UNICODE</b> names.<para/>
	/// Note that this is simply a convenience wrapper around
	/// <see cref="IDXCOMObject.SetPrivateData"/> with the <see cref="Guid"/> "<i><see cref="COMUtility.WKPDID_D3DDebugObjectNameW"/></i>".
	/// Therefore names which are set with `SetName` can be retrieved with <see cref="IDXCOMObject.GetPrivateData"/> with the same GUID.
	/// Additionally, D3D12 supports narrow strings for names, using the "<i><see cref="COMUtility.WKPDID_D3DDebugObjectName"/></i>" GUID directly instead.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12object-setname#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetName( string name ) ;
	
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