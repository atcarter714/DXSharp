#region Using Directives

using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.Direct3D12 ;


// ----------------------------------------------------------

[Wrapper( typeof( ID3D12Object ) )]
public interface IObject: IDXCOMObject, IUnknownWrapper {
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
			((ID3D12Object)ComPtrBase?.InterfaceObjectRef!)!
				.SetName( Name ) ;
} ;


/// <summary>Wrapper interface for the native ID3D12Object COM interface</summary>
[Wrapper( typeof( ID3D12Object ) )]
public interface IObject< ID3D12 >: IObject, IUnknownWrapper< ID3D12 >
	where ID3D12: ID3D12Object {
	void IObject.SetName( string Name ) => ComObject!.SetName( Name ) ;
} ;


// ----------------------------------------------------------

[Wrapper( typeof( ID3D12DeviceChild ) )]
public interface IDeviceChild: IObject {
	/// <summary>Gets a pointer to the device that created this interface.</summary>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The globally unique identifier (<b>GUID</b>) for the device interface. The <b>REFIID</b>, or <b>GUID</b>, of the interface to the device can be obtained by using the __uuidof() macro. For example, __uuidof(<a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12device">ID3D12Device</a>) will get the <b>GUID</b> of the interface to a device.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12devicechild-getdevice#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppvDevice">
	/// <para>Type: <b>void**</b> A pointer to a memory block that receives a pointer to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12device">ID3D12Device</a> interface for the device.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12devicechild-getdevice#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>Any returned interfaces have their reference count incremented by one, so be sure to call ::release() on the returned pointers before they are freed or else you will have a memory leak.</remarks>
	void GetDevice( in Guid riid, out IDevice ppvDevice ) {
		unsafe {
			Guid _riid = riid ;
			( (ID3D12DeviceChild)ComPtrBase?.InterfaceObjectRef! )!
				.GetDevice( &_riid, out var _ppvDevice ) ;
			ppvDevice = (IDevice)_ppvDevice ;
		}
	}
} ;


/*[Wrapper( typeof(ID3D12DeviceChild) )]
public interface IDeviceChild< ID3D12 >: IDeviceChild,
										 IObject< ID3D12 >
	where ID3D12: ID3D12DeviceChild, ID3D12Object, IUnknown { } ;*/


// ----------------------------------------------------------


[Wrapper(typeof(ID3D12Pageable))]
public interface IPageable: IDeviceChild { }

/*[Wrapper( typeof(ID3D12Pageable) )]
public interface IPageable< ID3D12 >: IPageable,
									  IDeviceChild< ID3D12 >
	where ID3D12: ID3D12Pageable, ID3D12DeviceChild, ID3D12Object, IUnknown { } ;*/


// ----------------------------------------------------------


// ----------------------------------------------------------