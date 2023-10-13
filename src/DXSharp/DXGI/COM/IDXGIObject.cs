#region Using Directives
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using DXSharp.Windows ;
using winmdRoot = Windows.Win32;

using DXSharp.Windows.COM ;
#endregion

namespace Windows.Win32.Graphics.Dxgi ;


[ComImport, Guid("AEC22FB8-76F3-4639-9BE0-28EB43A67A2E"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown),]
public unsafe interface IDXGIObject: IUnknown {
	/// <summary>Sets application-defined data to the object and associates that data with a GUID.</summary>
	/// <param name="Name">
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> A GUID that identifies the data. Use this GUID in a call to <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiobject-getprivatedata">GetPrivateData</a> to get the data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="DataSize">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The size of the object's data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>const void*</b> A pointer to the object's data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> values.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>SetPrivateData</b> makes a copy of the specified data and stores it with the object. Private data that <b>SetPrivateData</b> stores in the object occupies the same storage space as private data that is stored by associated Direct3D objects (for example, by a Microsoft Direct3D 11 device through <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11device-setprivatedata">ID3D11Device::SetPrivateData</a> or by a Direct3D 11 child device through <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11devicechild-setprivatedata">ID3D11DeviceChild::SetPrivateData</a>). The <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-layers">debug layer</a> reports memory leaks by outputting a list of object interface pointers along with their friendly names. The default friendly name is "&lt;unnamed&gt;". You can set the friendly name so that you can determine if the corresponding object interface pointer caused the leak. To set the friendly name, use the <b>SetPrivateData</b> method and the well-known private data GUID (<b>WKPDID_D3DDebugObjectName</b>) that is in D3Dcommon.h. For example, to give pContext a friendly name of <i>My name</i>, use the following code:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-setprivatedata#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[return: MarshalAs(UnmanagedType.I4)]
	[PreserveSig] HResult SetPrivateData( Guid* Name, uint DataSize,
									   [Optional, MaybeNull] void* pData ) ;

	
	/// <summary>Set an interface in the object's private data.</summary>
	/// <param name="Name">
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> A GUID identifying the interface.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-setprivatedatainterface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pUnknown">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> The interface to set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-setprivatedatainterface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the following <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>This API associates an interface pointer with the object. When the interface is set its reference count is incremented. When the data are overwritten (by calling SPD or SPDI with the same GUID) or the object is destroyed, ::Release() is called and the interface's reference count is decremented.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-setprivatedatainterface#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[return: MarshalAs(UnmanagedType.I4)]
	[PreserveSig] HResult SetPrivateDataInterface< T >( Guid* Name,
													  [MarshalAs(0x19)] T? pUnknown ) 
																						where T: IUnknown ;
	
	
	/// <summary>Get a pointer to the object's data.</summary>
	/// <param name="Name">
	/// <para>
	/// Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">
	/// REFGUID</a></b> A GUID identifying the data.
	/// </para>
	/// <para>
	/// <a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">
	/// Read more on docs.microsoft.com</a>.
	/// </para>
	/// </param>
	/// <param name="pDataSize">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b>
	/// The size of the data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>void*</b> Pointer to the data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b>
	/// Returns one of the following <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>
	/// If the data returned is a pointer to an <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>,
	/// or one of its derivative classes, previously set by
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiobject-setprivatedatainterface">IDXGIObject::SetPrivateDataInterface</a>,
	/// you must call <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nf-unknwn-iunknown-release">::Release()</a> on the pointer before the
	/// pointer is freed to decrement the reference count. You can pass <b>GUID_DeviceType</b> in the <i>Name</i> parameter of <b>GetPrivateData</b> to
	/// retrieve the device type from the display adapter object
	/// (<a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a>,
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter1">IDXGIAdapter1</a>,
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nn-dxgi1_2-idxgiadapter2">IDXGIAdapter2</a>).
	/// <b>To get the type of device on which the display adapter was created</b> </para>
	/// <para>
	/// <a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-getprivatedata#">
	/// Read more on docs.microsoft.com</a>.
	/// </para>
	/// </remarks>
	[return: MarshalAs(UnmanagedType.I4)]
	[PreserveSig] HResult GetPrivateData( Guid* Name, ref uint pDataSize, 
										  [Optional, MaybeNull] void* pData ) ;
	
	
	/// <summary>Gets the parent of the object.</summary>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The ID of the requested interface.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-getparent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppParent">
	/// <para>Type: <b>void**</b> The address of a pointer to the parent object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-getparent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> values.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiobject-getparent">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetParent( Guid* riid, [MarshalAs(0x19)] out IUnknown ppParent ) ;
} ;
