#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.Direct3D12 ;


// ----------------------------------------------------------

[Wrapper( typeof( ID3D12Object ) )]
public interface IObject: IDirectXCOMObject {
	void SetName( string name ) ;
} ;

/// <summary>Wrapper interface for the native ID3D12Object COM interface</summary>
[Wrapper( typeof( ID3D12Object ) )]
public interface IObject< ID3D12 >: IObject,
									IUnknownWrapper< ID3D12 >
									where ID3D12: ID3D12Object, IUnknown {
	ID3D12? IUnknownWrapper<ID3D12>.ComObject => ComObject ;
	Type IUnknownWrapper<ID3D12>.ComType => typeof(ID3D12) ;
	ComPtr<ID3D12>? IUnknownWrapper<ID3D12>.ComPointer => ComPointer ;
	
	new void SetName( string name ) => 
		ComObject!.SetName( name ) ;
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
	void GetDevice( in Guid riid, out IDevice ppvDevice ) ;
} ;

[Wrapper( typeof( ID3D12DeviceChild ) )]
public interface IDeviceChild< ID3D12 >: IDeviceChild,
										 IObject< ID3D12 >
	where ID3D12: ID3D12DeviceChild, ID3D12Object, IUnknown {
	
} ;

// ----------------------------------------------------------


[Wrapper(typeof(ID3D12Pageable))]
public interface IPageable: IDeviceChild { }

[Wrapper( typeof( ID3D12Pageable ) )]
public interface IPageable< ID3D12 >: IPageable,
									  IDeviceChild< ID3D12 >
	where ID3D12: ID3D12Pageable, ID3D12DeviceChild, IUnknown {
	
} ;


// ----------------------------------------------------------


// ----------------------------------------------------------

