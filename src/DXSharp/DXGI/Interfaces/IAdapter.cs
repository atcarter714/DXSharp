// Implements an idiomatic C# version of IDXGIAdapter interface:
// https://docs.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiadapter
#region Using Directives
using System.Runtime.Versioning ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


// ------------------------------------------------------------------------------
// Version :: IDXGIAdapter
// ------------------------------------------------------------------------------

/// <summary>Proxy contract for the native <see cref="IDXGIAdapter"/> COM interface.</summary>
/// <remarks>
/// DXGI documentation: 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiadapter#inheritance">
/// IDXGIAdapter
/// </a>
/// </remarks>
[SupportedOSPlatform("windows6.0")]
[ProxyFor(typeof(IDXGIAdapter))]
public interface IAdapter: IObject,
						   IComObjectRef< IDXGIAdapter >,
						   IUnknownWrapper< IDXGIAdapter >,
						   IInstantiable {
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGIAdapter >? ComPointer { get ; }
	new IDXGIAdapter? COMObject => ComPointer?.Interface ;
	IDXGIAdapter? IComObjectRef< IDXGIAdapter >.COMObject => COMObject ;
	// ---------------------------------------------------------------------------------
	
	/// <summary>Enumerate adapter (video card) outputs.</summary>
	/// <param name="index">
	///     <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The index of the output.</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiadapter-enumoutputs#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppOutput">
	///     <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgioutput">IDXGIOutput</a>**</b> The address of a pointer to an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgioutput">IDXGIOutput</a> interface at the position specified by the <i>Output</i> parameter.</para>
	///     <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiadapter-enumoutputs#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> A code that indicates success or failure (see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>). DXGI_ERROR_NOT_FOUND is returned if the index is greater than the number of outputs. If the adapter came from a device created using D3D_DRIVER_TYPE_WARP, then the adapter has no outputs, so DXGI_ERROR_NOT_FOUND is returned.</para>
	/// </returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  If you call this API in a Session 0 process, it returns <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_NOT_CURRENTLY_AVAILABLE</a>.</div> <div> </div> When the <b>EnumOutputs</b> method succeeds and fills the <i>ppOutput</i> parameter with the address of the pointer to the output interface, <b>EnumOutputs</b> increments the output interface's reference count. To avoid a memory leak, when you finish using the output interface, call the <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nf-unknwn-iunknown-release">Release</a> method to decrement the reference count. <b>EnumOutputs</b> first returns the output on which the desktop primary is displayed. This output corresponds with an index of zero. <b>EnumOutputs</b> then returns other outputs.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiadapter-enumoutputs#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult EnumOutputs< T >( uint index, out T? ppOutput ) 
		where T: class, IOutput, IInstantiable ;
	

	/// <summary>Gets a DXGI 1.0 description of an adapter (or video card).</summary>
	/// <param name="pDesc">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_adapter_desc">DXGI_ADAPTER_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_adapter_desc">DXGI_ADAPTER_DESC</a> structure that describes the adapter. This parameter must not be <b>NULL</b>. On <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature level</a> 9 graphics hardware, <b>GetDesc</b> returns zeros for the PCI ID in the <b>VendorId</b>, <b>DeviceId</b>, <b>SubSysId</b>, and <b>Revision</b> members of <b>DXGI_ADAPTER_DESC</b> and “Software Adapter” for the description string in the <b>Description</b> member.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiadapter-getdesc#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise returns E_INVALIDARG if the <i>pDesc</i> parameter is <b>NULL</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para>Graphics apps can use the DXGI API to retrieve an accurate set of graphics memory values on systems that have Windows Display Driver Model (WDDM) drivers. The following are the critical steps involved. </para>
	/// <para>This doc was truncated.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiadapter-getdesc#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetDesc( out AdapterDescription pDesc ) ;

	
	/// <summary>Checks whether the system supports a device interface for a graphics component.</summary>
	/// <param name="InterfaceName">
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> The GUID of the interface of the device version for which support is being checked. This should usually be __uuidof(IDXGIDevice), which returns the version number of the Direct3D 9 UMD (user mode driver) binary. Since WDDM 2.3, all driver components within a driver package (D3D9, D3D11, and D3D12) have been required to share a single version number, so this is a good way to query the driver version regardless of which API is being used.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiadapter-checkinterfacesupport#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pUMDVersion">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/winnt/ns-winnt-large_integer-r1">LARGE_INTEGER</a>*</b> The user mode driver version of <i>InterfaceName</i>. This is  returned only if the interface is supported, otherwise this parameter will be <b>NULL</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiadapter-checkinterfacesupport#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> S_OK indicates that the interface is supported, otherwise DXGI_ERROR_UNSUPPORTED is returned (For more information, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>).</para>
	/// </returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  You can  use <b>CheckInterfaceSupport</b> only to  check whether a Direct3D 10.x interface is supported, and only on Windows Vista SP1 and later versions of the operating system. If you try to use <b>CheckInterfaceSupport</b> to check whether a Direct3D 11.x and later version interface is supported, <b>CheckInterfaceSupport</b> returns DXGI_ERROR_UNSUPPORTED. Therefore, do not use <b>CheckInterfaceSupport</b>. Instead, to verify whether the operating system supports a particular interface, try to create the interface. For example, if you call the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11device-createblendstate">ID3D11Device::CreateBlendState</a> method and it fails, the operating system does not support the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nn-d3d11-id3d11blendstate">ID3D11BlendState</a> interface.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiadapter-checkinterfacesupport#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void CheckInterfaceSupport( in Guid InterfaceName, out long pUMDVersion ) ;
	
	
	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof(IDXGIAdapter) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(IDXGIAdapter).GUID ;
	static IDXCOMObject IInstantiable.Instantiate( ) => new Adapter( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint pComObj ) => new Adapter( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Adapter( (IDXGIAdapter)pComObj! ) ;
	// ================================================================================
} ;


// ------------------------------------------------------------------------------
// Version :: IDXGIAdapter1
// ------------------------------------------------------------------------------

[SupportedOSPlatform("windows6.1")]
[ProxyFor(typeof(IDXGIAdapter1))]
public interface IAdapter1: IAdapter,
							IComObjectRef< IDXGIAdapter1 >,
							IUnknownWrapper< IDXGIAdapter1 > {
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGIAdapter1 >? ComPointer { get ; }
	new IDXGIAdapter1? COMObject => ComPointer?.Interface ;
	IDXGIAdapter1? IComObjectRef< IDXGIAdapter1 >.COMObject => COMObject ;
	// ---------------------------------------------------------------------------------
	
	/// <summary>Gets a DXGI 1.1 description of an adapter (or video card).</summary>
	/// <param name="pDesc">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_adapter_desc1">DXGI_ADAPTER_DESC1</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_adapter_desc1">DXGI_ADAPTER_DESC1</a> structure that describes the adapter. This parameter must not be <b>NULL</b>. On <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature level</a> 9 graphics hardware, <b>GetDesc1</b> returns zeros for the PCI ID in the <b>VendorId</b>, <b>DeviceId</b>, <b>SubSysId</b>, and <b>Revision</b> members of <b>DXGI_ADAPTER_DESC1</b> and “Software Adapter” for the description string in the <b>Description</b> member.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns E_INVALIDARG if the <i>pDesc</i> parameter is <b>NULL</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method is not supported by DXGI 1.0, which shipped in Windows Vista and Windows Server 2008. DXGI 1.1 support is required, which is available on Windows 7, Windows Server 2008 R2, and as an update to Windows Vista with Service Pack 2 (SP2) (<a href="https://support.microsoft.com/topic/application-compatibility-update-for-windows-vista-windows-server-2008-windows-7-and-windows-server-2008-r2-february-2010-3eb7848b-9a76-85fe-98d0-729e3827ea60">KB 971644</a>) and Windows Server 2008 (<a href="https://support.microsoft.com/kb/971512/">KB 971512</a>). Use the <b>GetDesc1</b> method to get a DXGI 1.1 description of an adapter.  To get a DXGI 1.0 description, use the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> method.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetDesc1( out AdapterDescription1 pDesc ) ;
	
	// ---------------------------------------------------------------------------------

	static Type IUnknownWrapper.ComType => typeof(IDXGIAdapter1) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(IDXGIAdapter1).GUID ;

	static IDXCOMObject IInstantiable.Instantiate( ) => new Adapter1( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint pComObj ) => new Adapter1( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Adapter1( (IDXGIAdapter1)pComObj! ) ;
	
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------
// Version :: IDXGIAdapter2
// ------------------------------------------------------------------------------

[SupportedOSPlatform("windows8.0")]
[ProxyFor(typeof(IDXGIAdapter2))]
public interface IAdapter2: IAdapter1,
							IComObjectRef< IDXGIAdapter2 >,
							IUnknownWrapper< IDXGIAdapter2 > {
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGIAdapter2 >? ComPointer { get ; }
	new IDXGIAdapter2? COMObject => ComPointer?.Interface ;
	IDXGIAdapter2? IComObjectRef< IDXGIAdapter2 >.COMObject => COMObject ;

	static Type IUnknownWrapper.ComType => typeof(IDXGIAdapter2) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(IDXGIAdapter2).GUID ;
	
	static IDXCOMObject IInstantiable.Instantiate( ) => new Adapter2( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint pComObj ) => new Adapter2( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Adapter( (IDXGIAdapter2)pComObj! ) ;
	// ==================================================================================
	
	
	
	/// <summary>Gets a Microsoft DirectX Graphics Infrastructure (DXGI) 1.2 description of an adapter or video card.</summary>
	/// <param name="pDesc">
	/// <para>A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_adapter_desc2">DXGI_ADAPTER_DESC2</a> structure that describes the adapter. This parameter must not be <b>NULL</b>. On <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature level</a> 9 graphics hardware, earlier versions of  <b>GetDesc2</b> (<a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter-getdesc">GetDesc</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1">GetDesc1</a>) return zeros for the PCI ID in the <b>VendorId</b>, <b>DeviceId</b>, <b>SubSysId</b>, and <b>Revision</b> members of the adapter description structure and “Software Adapter” for the description string in the <b>Description</b> member. <b>GetDesc2</b> returns the actual feature level 9 hardware values in these members.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiadapter2-getdesc2#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>Returns S_OK if successful; otherwise, returns E_INVALIDARG if the <i>pDesc</i> parameter is <b>NULL</b>.</returns>
	/// <remarks>
	/// <para>Use the <b>GetDesc2</b> method to get a DXGI 1.2 description of an adapter.  To get a DXGI 1.1 description, use the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1">IDXGIAdapter1::GetDesc1</a> method. To get a DXGI 1.0 description, use the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter-getdesc">IDXGIAdapter::GetDesc</a> method. The Windows Display Driver Model (WDDM) scheduler can preempt the GPU's execution of application tasks. The granularity at which the GPU can be preempted from performing its current task in the WDDM 1.1 or earlier driver model is a direct memory access (DMA) buffer for graphics tasks or a compute packet for compute tasks. The GPU can switch between tasks only after it completes the currently executing unit of work, a DMA buffer or a compute packet. A DMA buffer is the largest independent unit of graphics work that the WDDM scheduler can submit to the GPU. This buffer contains a set of GPU instructions that the WDDM driver and GPU use. A compute packet is the largest independent unit of compute work that the WDDM scheduler can submit to the GPU. A compute packet contains dispatches (for example, calls to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11devicecontext-dispatch">ID3D11DeviceContext::Dispatch</a> method), which contain thread groups. The WDDM 1.2 or later driver model allows the GPU to be preempted at finer granularity levels than a DMA buffer or compute packet. You can use the <b>GetDesc2</b> method to retrieve the granularity levels for graphics and compute tasks.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiadapter2-getdesc2#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetDesc2( out AdapterDescription2 pDesc ) ;
}


// ------------------------------------------------------------------------------
// Version :: IDXGIAdapter...
// ------------------------------------------------------------------------------

