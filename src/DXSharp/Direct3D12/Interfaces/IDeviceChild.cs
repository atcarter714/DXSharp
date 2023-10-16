#region Using Directives
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


// ----------------------------------------------------------

/// <summary>
/// An interface from which other core interfaces inherit from, including (but not limited to)
/// <see cref="IPipelineLibrary"/>,
/// <see cref="ICommandList"/>,
/// <see cref="IPageable"/>, and
/// <see cref="IRootSignature"/>.
/// It provides a method to get back to the device object it was created against.
/// </summary>
[ProxyFor( typeof( ID3D12DeviceChild ) )]
public interface IDeviceChild: IObject,
							   IComObjectRef< ID3D12DeviceChild >,
							   IUnknownWrapper< ID3D12DeviceChild > {
	// ---------------------------------------------------------------------------------
	new ComPtr< ID3D12DeviceChild >? ComPointer { get ; }
	new ID3D12DeviceChild? COMObject => ComPointer?.Interface ;
	
	//! Explicit Interface Implementations / Disambiguation ::
	ID3D12DeviceChild? IComObjectRef< ID3D12DeviceChild >.COMObject => COMObject ;
	ComPtr< ID3D12DeviceChild >? IUnknownWrapper< ID3D12DeviceChild >.ComPointer => ComPointer ;
	ComPtr< ID3D12Object >? IUnknownWrapper< ID3D12Object >.ComPointer => ComPointer?.Cast< ID3D12Object >( ) ;
	// ---------------------------------------------------------------------------------
	
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
			COMObject!.GetDevice( &_riid, out var _ppvDevice ) ;
			ppvDevice = (IDevice)_ppvDevice ;
		}
	}
	
	
	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof(ID3D12DeviceChild) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12DeviceChild).GUID ;
	// ==================================================================================
} ;