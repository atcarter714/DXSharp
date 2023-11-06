// Implements an idiomatic C# version of IDXGIAdapter interface:
// https://docs.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiadapter
#region Using Directives
using System.Collections.ObjectModel ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.DXGI ;


// ------------------------------------------------------------------------------
// Version :: IDXGIAdapter
// ------------------------------------------------------------------------------

/// <summary>The <see cref="IAdapter"/> interface represents a display subsystem (including one or more GPUs, DACs and video memory).</summary>
/// <remarks>
/// Proxy contract for the native <see cref="IDXGIAdapter"/> COM interface.
/// DXGI documentation: 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiadapter#inheritance">
/// IDXGIAdapter
/// </a>
/// </remarks>
[SupportedOSPlatform("windows6.0")]
[ProxyFor(typeof(IDXGIAdapter))]
public interface IAdapter: IObject, IInstantiable {
	internal static readonly ReadOnlyDictionary<Guid, Func<IDXGIAdapter, IInstantiable> > _adapterCreationFunctions =
		new( new Dictionary<Guid, Func<IDXGIAdapter, IInstantiable> > {
			{ IAdapter.IID, ( pComObj ) => new Adapter( pComObj ) },
			{ IAdapter1.IID, ( pComObj ) => new Adapter1( (pComObj as IDXGIAdapter1)! ) },
			{ IAdapter2.IID, ( pComObj ) => new Adapter2( (pComObj as IDXGIAdapter2)! ) },
			{ IAdapter3.IID, ( pComObj ) => new Adapter3( (pComObj as IDXGIAdapter3)! ) },
			{ IAdapter4.IID, ( pComObj ) => new Adapter4( (pComObj as IDXGIAdapter4)! ) },
		} ) ;
	
	
	/// <summary>
	/// Enumerate adapter (video card) outputs.
	/// </summary>
	/// <param name="index">The index of the output.</param>
	/// <param name="ppOutput">interface at the position specified by the Output parameter.</param>
	/// /// <returns>
	/// A code that indicates success or failure (see DXGI_ERROR ).
	/// DXGI_ERROR_NOT_FOUND is returned if the index is greater than the number of outputs.
	/// If the adapter came from a device created using D3D_DRIVER_TYPE_WARP, then the
	/// adapter has no outputs, so DXGI_ERROR_NOT_FOUND is returned.
	/// </returns>
	/// <remarks>
	/// To know when you've reached the end of the collection of outputs, simply check for either
	/// a null value or HResult code DXGI_ERROR_NOT_FOUND.<para/>
	/// For additional technical details on this, see the
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR documentation</a>.
	/// </remarks>
	HResult EnumOutputs( uint index, out IOutput? ppOutput ) ;
	
	
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
	new static Type ComType => typeof(IDXGIAdapter) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid  {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIAdapter).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable. Instantiate( ) => new Adapter( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Adapter( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Adapter( (IDXGIAdapter)pComObj! ) ;
	// ================================================================================
} ;


// ------------------------------------------------------------------------------
// Version :: IDXGIAdapter1
// ------------------------------------------------------------------------------

[SupportedOSPlatform("windows6.1")]
[ProxyFor(typeof(IDXGIAdapter1))]
public interface IAdapter1: IAdapter {
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
	
	new static Type ComType => typeof(IDXGIAdapter1) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIAdapter1).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable. Instantiate( )                => new Adapter1( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr pComObj ) => new Adapter1( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Adapter1( (IDXGIAdapter1)pComObj! ) ;
	
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------
// Version :: IDXGIAdapter2
// ------------------------------------------------------------------------------

[SupportedOSPlatform( "windows8.0" )]
[ProxyFor( typeof( IDXGIAdapter2 ) )]
public interface IAdapter2: IAdapter1 {
	
	// ---------------------------------------------------------------------------------
	
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
	
	// ---------------------------------------------------------------------------------
	
	new static Type ComType => typeof( IDXGIAdapter2 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIAdapter2 ).GUID
															   .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new Adapter2( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Adapter2( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Adapter( (IDXGIAdapter2)pComObj! ) ;
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------
// Version :: IDXGIAdapter3
// ------------------------------------------------------------------------------

[ProxyFor( typeof( IDXGIAdapter3 ) )]
public interface IAdapter3: IAdapter2 {
	// ---------------------------------------------------------------------------------

	/// <summary>Registers to receive notification of hardware content protection teardown events.</summary>
	/// <param name="hEvent">
	/// <para>Type: <b>HANDLE</b> A handle to the event object that the operating system sets when hardware content protection teardown occurs. The <a href="https://docs.microsoft.com/windows/desktop/api/synchapi/nf-synchapi-createeventa">CreateEvent</a> or <a href="https://docs.microsoft.com/windows/desktop/api/synchapi/nf-synchapi-openeventa">OpenEvent</a> function returns this handle.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-registerhardwarecontentprotectionteardownstatusevent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pdwCookie">
	/// <para>Type: <b>DWORD*</b> A pointer to a key value that an application can pass to the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-unregisterhardwarecontentprotectionteardownstatus">IDXGIAdapter3::UnregisterHardwareContentProtectionTeardownStatus</a> method to unregister the notification event that <i>hEvent</i> specifies.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-registerhardwarecontentprotectionteardownstatusevent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> If this method succeeds, it returns <b>S_OK</b>. Otherwise, it returns an <b>HRESULT</b> error code.</para>
	/// </returns>
	/// <remarks>
	/// <para>Call <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11videodevice-getcontentprotectioncaps">ID3D11VideoDevice::GetContentProtectionCaps</a>() to check for the presence of the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ne-d3d11-d3d11_content_protection_caps">D3D11_CONTENT_PROTECTION_CAPS_HARDWARE_TEARDOWN</a>  capability to know whether the hardware contains an automatic teardown mechanism. After the event is signaled, the application can call <a href="https://docs.microsoft.com/windows/desktop/api/d3d11_1/nf-d3d11_1-id3d11videocontext1-checkcryptosessionstatus">ID3D11VideoContext1::CheckCryptoSessionStatus</a> to determine the impact of the hardware teardown for a specific <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nn-d3d11-id3d11cryptosession">ID3D11CryptoSession</a> interface.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-registerhardwarecontentprotectionteardownstatusevent#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void RegisterHardwareContentProtectionTeardownStatusEvent( Win32Handle hEvent, out uint pdwCookie ) ;

	/// <summary>Unregisters an event to stop it from receiving notification of hardware content protection teardown events.</summary>
	/// <param name="dwCookie">
	/// <para>Type: <b>DWORD</b> A key value for the window or event to unregister. The  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-registerhardwarecontentprotectionteardownstatusevent">IDXGIAdapter3::RegisterHardwareContentProtectionTeardownStatusEvent</a> method returns this value.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-unregisterhardwarecontentprotectionteardownstatus#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-unregisterhardwarecontentprotectionteardownstatus">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void UnregisterHardwareContentProtectionTeardownStatus( uint dwCookie ) ;

	/// <summary>This method informs the process of the current budget and process usage.</summary>
	/// <param name="NodeIndex">
	/// <para>Type: <b>UINT</b> Specifies the device's physical adapter for which the video memory information is queried. For single-GPU operation, set this to zero. If there are multiple GPU nodes, set this to the index of the node (the device's physical adapter) for which the video memory information is queried. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-queryvideomemoryinfo#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="memorySegmentGroup">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/ne-dxgi1_4-dxgi_memory_segment_group">DXGI_MEMORY_SEGMENT_GROUP</a></b> Specifies a DXGI_MEMORY_SEGMENT_GROUP that identifies the group as local or non-local.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-queryvideomemoryinfo#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pVideoMemoryInfo">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/ns-dxgi1_4-dxgi_query_video_memory_info">DXGI_QUERY_VIDEO_MEMORY_INFO</a>*</b> Fills in a DXGI_QUERY_VIDEO_MEMORY_INFO structure with the current values.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-queryvideomemoryinfo#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>Applications must explicitly manage their usage of physical memory explicitly and keep usage within the budget assigned to the application process. Processes that cannot kept their usage within their assigned budgets will likely experience stuttering, as they are intermittently frozen and paged-out to allow other processes to run.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-queryvideomemoryinfo#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	unsafe void QueryVideoMemoryInfo( uint NodeIndex,
									  MemorySegmentGroup memorySegmentGroup,
									  in QueryVideoMemoryInfo pVideoMemoryInfo ) ;

	/// <summary>This method sends the minimum required physical memory for an application, to the OS.</summary>
	/// <param name="NodeIndex">
	/// <para>Type: <b>UINT</b> Specifies the device's physical adapter for which the video memory information is being set. For single-GPU operation, set this to zero. If there are multiple GPU nodes, set this to the index of the node (the device's physical adapter) for which the video memory information is being set. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-setvideomemoryreservation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="memorySegmentGroup">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/ne-dxgi1_4-dxgi_memory_segment_group">DXGI_MEMORY_SEGMENT_GROUP</a></b> Specifies a DXGI_MEMORY_SEGMENT_GROUP that identifies the group as local or non-local.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-setvideomemoryreservation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Reservation">
	/// <para>Type: <b>UINT64</b> Specifies a UINT64 that sets the minimum required physical memory, in bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-setvideomemoryreservation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/win32/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>Applications are encouraged to set a video reservation to denote the amount of physical memory they cannot go without. This value helps the OS quickly minimize the impact of large memory pressure situations.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-setvideomemoryreservation#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetVideoMemoryReservation( uint NodeIndex, 
									MemorySegmentGroup memorySegmentGroup, 
									ulong Reservation ) ;

	/// <summary>This method establishes a correlation between a CPU synchronization object and the budget change event.</summary>
	/// <param name="hEvent">
	/// <para>Type: <b>HANDLE</b> Specifies a HANDLE for the event.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-registervideomemorybudgetchangenotificationevent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pdwCookie">
	/// <para>Type: <b>DWORD*</b> A key value for the window or event to unregister. The  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-registerhardwarecontentprotectionteardownstatusevent">IDXGIAdapter3::RegisterHardwareContentProtectionTeardownStatusEvent</a> method returns this value.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-registervideomemorybudgetchangenotificationevent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code.</para>
	/// </returns>
	/// <remarks>Instead of calling <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-queryvideomemoryinfo">QueryVideoMemoryInfo</a> regularly, applications can use CPU synchronization objects to efficiently wake threads when budget changes occur.</remarks>
	void RegisterVideoMemoryBudgetChangeNotificationEvent( Win32Handle hEvent, out uint pdwCookie ) ;

	/// <summary>This method stops notifying a CPU synchronization object whenever a budget change occurs. An application may switch back to polling the information regularly.</summary>
	/// <param name="dwCookie">
	/// <para>Type: <b>DWORD</b> A key value for the window or event to unregister. The  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-registerhardwarecontentprotectionteardownstatusevent">IDXGIAdapter3::RegisterHardwareContentProtectionTeardownStatusEvent</a> method returns this value.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-unregistervideomemorybudgetchangenotification#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>An application may switch back to polling for the information regularly.</remarks>
	void UnregisterVideoMemoryBudgetChangeNotification( uint dwCookie ) ;

	// ---------------------------------------------------------------------------------

	new static Type ComType => typeof( IDXGIAdapter3 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIAdapter3 ).GUID
															   .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new Adapter3( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Adapter3( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Adapter( (IDXGIAdapter3)pComObj! ) ;
	// ==================================================================================
} ;


// ------------------------------------------------------------------------------
// Version :: IDXGIAdapter4
// ------------------------------------------------------------------------------

[SupportedOSPlatform( "windows10.0.15063" )]
[ProxyFor( typeof( IDXGIAdapter4 ) )]
public interface IAdapter4: IAdapter3 {
	// ---------------------------------------------------------------------------------
	
	/// <summary>Gets a Microsoft DirectX Graphics Infrastructure (DXGI) 1.6 description of an adapter or video card. This description includes information about ACG compatibility.</summary>
	/// <param name="pDesc">
	/// <para>A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_6/ns-dxgi1_6-dxgi_adapter_desc3">DXGI_ADAPTER_DESC3</a> structure that describes the adapter. This parameter must not be <b>NULL</b>. On <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature level</a> 9 graphics hardware, early versions of  <b>GetDesc3</b> (<a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1">GetDesc1</a>, and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter-getdesc">GetDesc</a>) return zeros for the PCI ID in the <b>VendorId</b>, <b>DeviceId</b>, <b>SubSysId</b>, and <b>Revision</b> members of the adapter description structure and “Software Adapter” for the description string in the <b>Description</b> member. <b>GetDesc3</b> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiadapter2-getdesc2">GetDesc2</a> return the actual feature level 9 hardware values in these members.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgiadapter4-getdesc3#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>Returns S_OK if successful; otherwise, returns E_INVALIDARG if the <i>pDesc</i> parameter is <b>NULL</b>.</returns>
	/// <remarks>
	/// <para>Use the <b>GetDesc3</b> method to get a DXGI 1.6 description of an adapter.  To get a DXGI 1.2 description, use the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiadapter2-getdesc2">IDXGIAdapter2::GetDesc2</a> method. To get a DXGI 1.1 description, use the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1">IDXGIAdapter1::GetDesc1</a> method. To get a DXGI 1.0 description, use the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter-getdesc">IDXGIAdapter::GetDesc</a> method. The Windows Display Driver Model (WDDM) scheduler can preempt the graphics processing unit (GPU)'s execution of application tasks. The granularity at which the GPU can be preempted from performing its current task in the WDDM 1.1 or earlier driver model is a direct memory access (DMA) buffer for graphics tasks or a compute packet for compute tasks. The GPU can switch between tasks only after it completes the currently executing unit of work, a DMA buffer or a compute packet. A DMA buffer is the largest independent unit of graphics work that the WDDM scheduler can submit to the GPU. This buffer contains a set of GPU instructions that the WDDM driver and GPU use. A compute packet is the largest independent unit of compute work that the WDDM scheduler can submit to the GPU. A compute packet contains dispatches (for example, calls to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11devicecontext-dispatch">ID3D11DeviceContext::Dispatch</a> method), which contain thread groups. The WDDM 1.2 or later driver model allows the GPU to be preempted at finer granularity levels than a DMA buffer or compute packet. You can use the <b>GetDesc3</b> or <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiadapter2-getdesc2">GetDesc2</a> methods to retrieve the granularity levels for graphics and compute tasks.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/nf-dxgi1_6-idxgiadapter4-getdesc3#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetDesc3( out AdapterDescription3 pDesc) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof( IDXGIAdapter4 ) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIAdapter4 ).GUID
															   .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Adapter4( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Adapter4( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Adapter( (IDXGIAdapter4)pComObj! ) ;
	// ==================================================================================
};