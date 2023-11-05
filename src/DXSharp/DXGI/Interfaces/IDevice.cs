#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.DXGI ;


[ProxyFor(typeof(IDXGIDevice))]
public interface IDevice: IObject, IInstantiable {
	// ----------------------------------------------------------
	// Interface Methods:
	// ----------------------------------------------------------
	
	/// <summary>Gets the adapter for this device.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b>
	/// Returns S_OK if successful; otherwise, returns one of the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> that indicates failure.
	/// If the <i>pAdapter</i> parameter is <b>NULL</b> this method returns E_INVALIDARG.</para>
	/// </returns>
	/// <remarks>If the <b>GetAdapter</b> method succeeds, the reference count on the adapter interface will be incremented. To avoid a memory leak, be sure to release the interface when you are finished using it.</remarks>
	HResult GetAdapter( out IAdapter adapter ) ;
	
	/// <summary>Returns a surface. This method is used internally and you should not call it directly in your application.</summary>
	/// <param name="pDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_surface_desc">DXGI_SURFACE_DESC</a>*</b> A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_surface_desc">DXGI_SURFACE_DESC</a> structure that describes the surface.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="numSurfaces">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of surfaces to create.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="usage">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a></b> A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a> flag that specifies how the surface is expected to be used.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pSharedResource">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_shared_resource">DXGI_SHARED_RESOURCE</a>*</b> An optional pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ns-dxgi-dxgi_shared_resource">DXGI_SHARED_RESOURCE</a> structure that contains shared resource information for opening views of such resources.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppSurface">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgisurface">IDXGISurface</a>**</b> The address of an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgisurface">IDXGISurface</a> interface pointer to the first created surface.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; an error code otherwise.  For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>The <b>CreateSurface</b> method creates a buffer to exchange data between one or more devices. It is used internally, and you should not directly call it. The runtime automatically creates an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgisurface">IDXGISurface</a> interface when it creates a Direct3D resource object that represents a surface. For example, the runtime creates an <b>IDXGISurface</b> interface when it calls <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11device-createtexture2d">ID3D11Device::CreateTexture2D</a> or <a href="https://docs.microsoft.com/windows/desktop/api/d3d10/nf-d3d10-id3d10device-createtexture2d">ID3D10Device::CreateTexture2D</a> to create a 2D texture. To retrieve the <b>IDXGISurface</b> interface that represents the 2D texture surface, call <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(q)">ID3D11Texture2D::QueryInterface</a> or <b>ID3D10Texture2D::QueryInterface</b>. In this call, you must pass the identifier of <b>IDXGISurface</b>. If the 2D texture has only a single MIP-map level and does not consist of an array of textures, <b>QueryInterface</b> succeeds and returns a pointer to the <b>IDXGISurface</b> interface pointer. Otherwise, <b>QueryInterface</b> fails and does not return the pointer to <b>IDXGISurface</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-createsurface#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	internal void CreateSurface( in SurfaceDescription pDesc,
								 uint numSurfaces,
								 uint usage,
								 ref SharedResource? pSharedResource,
								 out Span< Surface > ppSurface  ) ;
	
	/// <summary>Gets the residency status of an array of resources.</summary>
	/// <param name="ppResources">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> An array of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiresource">IDXGIResource</a> interfaces.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-queryresourceresidency#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pResidencyStatus">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_residency">DXGI_RESIDENCY</a>*</b> An array of <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_residency">DXGI_RESIDENCY</a> flags. Each element describes the residency status for corresponding element in the <i>ppResources</i> argument array.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-queryresourceresidency#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="numResources">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of resources in the <i>ppResources</i> argument array and <i>pResidencyStatus</i> argument array.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-queryresourceresidency#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_DEVICE_REMOVED</a>, E_INVALIDARG, or E_POINTER (see <a href="https://docs.microsoft.com/windows/desktop/SecCrypto/common-hresult-values">Common HRESULT Values</a> and WinError.h for more information).</para>
	/// </returns>
	/// <remarks>
	/// <para>The information returned by the <i>pResidencyStatus</i> argument array describes the residency status at the time that the <b>QueryResourceResidency</b> method was called.</para>
	/// <para><div class="alert"><b>Note</b>
	/// The residency status will constantly change.</div>
	/// If you call the <b>QueryResourceResidency</b> method during a device removed state, the <i>pResidencyStatus</i> argument will return the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_residency">DXGI_RESIDENCY_RESIDENT_IN_SHARED_MEMORY</a> flag.
	/// <div class="alert"><b>Note</b>  This method should not be called every frame as it incurs a non-trivial amount of overhead.</div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-queryresourceresidency#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void QueryResourceResidency( in IResource?[ ] ppResources, 
								 out Span< Residency > pResidencyStatus, 
								 uint numResources ) ;
	
	
	/// <summary>Sets the GPU thread priority.</summary>
	/// <param name="priority">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">INT</a></b> A value that specifies the required GPU thread priority. This value must be between -7 and 7, inclusive, where 0 represents normal priority.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-setgputhreadpriority#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Return S_OK if successful; otherwise, returns E_INVALIDARG if the <i>Priority</i> parameter is invalid.</para>
	/// </returns>
	/// <remarks>
	/// <para>The values for the <i>Priority</i> parameter function as follows: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-setgputhreadpriority#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetGPUThreadPriority( int priority ) ;
	
	
	/// <summary>Gets the GPU thread priority.</summary>
	/// <param name="pPriority">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">INT</a>*</b> A pointer to a variable that receives a value that indicates the current GPU thread priority. The value will be between -7 and 7, inclusive, where 0 represents normal priority.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-getgputhreadpriority#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Return S_OK if successful; otherwise, returns E_POINTER if the <i>pPriority</i> parameter is <b>NULL</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice-getgputhreadpriority">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetGPUThreadPriority( out int pPriority ) ;
	
	// ----------------------------------------------------------
	public new static Type ComType => typeof( IDXGIDevice ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice).GUID
																	.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==========================================================
} ;



// -----------------------------------------------------------------
// Device Sub-Object Types:
// -----------------------------------------------------------------

//! Abstract Base Interface:
[ProxyFor(typeof(IDXGIDeviceSubObject))]
public interface IDeviceSubObject: IObject {
	T GetDevice< T >( ) where T: IDevice ;
	
	new static Type ComType => typeof( IDXGIDeviceSubObject ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDeviceSubObject).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
} ;



[ProxyFor(typeof(IDXGIDevice1))]
public interface IDevice1: IDevice {
	
	/// <summary>Sets the number of frames that the system is allowed to queue for rendering.</summary>
	/// <param name="MaxLatency">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The maximum number of back buffer frames that a driver can queue. The value defaults to 3, but can range from 1 to 16. A value of 0 will reset latency to the default.  For multi-head devices, this value is specified per-head.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice1-setmaximumframelatency#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, DXGI_ERROR_DEVICE_REMOVED if the device was removed.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method is not supported by DXGI 1.0, which shipped in Windows Vista and Windows Server 2008. DXGI 1.1 support is required, which is available on Windows 7, Windows Server 2008 R2, and as an update to Windows Vista with Service Pack 2 (SP2) (<a href="https://support.microsoft.com/topic/application-compatibility-update-for-windows-vista-windows-server-2008-windows-7-and-windows-server-2008-r2-february-2010-3eb7848b-9a76-85fe-98d0-729e3827ea60">KB 971644</a>) and Windows Server 2008 (<a href="https://support.microsoft.com/kb/971512/">KB 971512</a>). Frame latency is the number of frames that are allowed to be stored in a queue before submission for rendering.  Latency is often used to control how the CPU chooses between responding to user input and frames that are in the render queue.  It is often beneficial for applications that have no user input (for example, video playback) to queue more than 3 frames of data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice1-setmaximumframelatency#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetMaximumFrameLatency( uint MaxLatency ) ;

	/// <summary>Gets the number of frames that the system is allowed to queue for rendering.</summary>
	/// <param name="pMaxLatency">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> This value is set to the number of frames that can be queued for render. This value defaults to 3, but can range from 1 to 16.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice1-getmaximumframelatency#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful; otherwise, returns one of the following members of the <a href="https://docs.microsoft.com/windows/desktop/direct3d9/d3derr">D3DERR</a> enumerated type: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method is not supported by DXGI 1.0, which shipped in Windows Vista and Windows Server 2008. DXGI 1.1 support is required, which is available on Windows 7, Windows Server 2008 R2, and as an update to Windows Vista with Service Pack 2 (SP2) (<a href="https://support.microsoft.com/topic/application-compatibility-update-for-windows-vista-windows-server-2008-windows-7-and-windows-server-2008-r2-february-2010-3eb7848b-9a76-85fe-98d0-729e3827ea60">KB 971644</a>) and Windows Server 2008 (<a href="https://support.microsoft.com/kb/971512/">KB 971512</a>). Frame latency is the number of frames that are allowed to be stored in a queue before submission for rendering.  Latency is often used to control how the CPU chooses between responding to user input and frames that are in the render queue.  It is often beneficial for applications that have no user input (for example, video playback) to queue more than 3 frames of data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgidevice1-getmaximumframelatency#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetMaximumFrameLatency( out uint pMaxLatency ) ;
	
	
	new static Type ComType => typeof( IDXGIDevice1 ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice1).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
} ;


[ProxyFor(typeof(IDXGIDevice2))]
public interface IDevice2: IDevice1 {
	// ----------------------------------------------------------
	
	/// <summary>Allows the operating system to free the video memory of resources by discarding their content. (IDXGIDevice2.OfferResources)</summary>
	/// <param name="NumResources">The number of resources in the <i>ppResources</i> argument array.</param>
	/// <param name="ppResources">An array of pointers to <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiresource">IDXGIResource</a> interfaces for the resources to offer.</param>
	/// <param name="Priority">A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_offer_resource_priority">DXGI_OFFER_RESOURCE_PRIORITY</a>-typed value that indicates how valuable data is.</param>
	/// <returns>
	/// <para><b>OfferResources</b> returns:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>The priority value that the  <i>Priority</i> parameter specifies describes how valuable the caller considers the content to be.  The operating system uses the priority value to discard resources in order of priority. The operating system discards a resource that is offered with low priority before it discards a resource that is  offered with a higher priority. If you call <b>OfferResources</b> to offer a resource while the resource is bound to the pipeline, the resource is unbound.  You cannot call <b>OfferResources</b> on a resource that is mapped.  After you offer a resource, the resource cannot be mapped or bound to the pipeline until you call the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-reclaimresources">IDXGIDevice2::ReclaimResource</a> method to reclaim the resource. You cannot call <b>OfferResources</b> to offer immutable resources. To offer shared resources, call <b>OfferResources</b> on only one of the sharing devices.  To ensure exclusive access to the resources, you must use an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgikeyedmutex">IDXGIKeyedMutex</a> object and then call <b>OfferResources</b> only while you hold the mutex. In fact, you can't offer shared resources unless you use <b>IDXGIKeyedMutex</b> because offering shared resources without using <b>IDXGIKeyedMutex</b> isn't supported. <div class="alert"><b>Note</b>  The user mode display driver might not immediately offer the resources that you specified in a call to <b>OfferResources</b>. The driver can postpone offering them until the next call to <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-present">IDXGISwapChain::Present</a>, <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>, or <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11devicecontext-flush">ID3D11DeviceContext::Flush</a>. </div> <div> </div> <b>Platform Update for Windows 7:  </b>The runtime validates that <b>OfferResources</b> is used correctly on non-shared resources but doesn't perform the intended functionality. For more info about the Platform Update for Windows 7, see <a href="https://docs.microsoft.com/windows/desktop/direct3darticles/platform-update-for-windows-7">Platform Update for Windows 7</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-offerresources#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void OfferResources( uint NumResources,
						 IResource[ ] ppResources,
						 OfferResourcePriority Priority ) ;

	/// <summary>Restores access to resources that were previously offered by calling IDXGIDevice2::OfferResources.</summary>
	/// <param name="NumResources">The number of resources in the <i>ppResources</i> argument and <i>pDiscarded</i> argument arrays.</param>
	/// <param name="ppResources">An array of pointers to <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiresource">IDXGIResource</a> interfaces for the resources to reclaim.</param>
	/// <param name="pDiscarded">A pointer to an array that receives Boolean values. Each value in the array corresponds to a resource at the same index that the <i>ppResources</i> parameter specifies.  The runtime sets each Boolean value to TRUE if the corresponding resource’s content was discarded and is now undefined, or to FALSE if the corresponding resource’s old content is still intact.  The caller can pass in <b>NULL</b>, if the caller intends to fill the resources with new content regardless of whether the old content was discarded.</param>
	/// <returns>
	/// <para><b>ReclaimResources</b> returns:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>After you call <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-offerresources">IDXGIDevice2::OfferResources</a> to offer one or more resources, you must call <b>ReclaimResources</b> before you can use those resources again.  You must check the values in the array at <i>pDiscarded</i> to determine whether each resource’s content was discarded. If a resource’s content was discarded while it was offered, its current content is undefined. Therefore, you must overwrite the resource’s content before you use the resource. To reclaim shared resources, call <b>ReclaimResources</b> only on one of the sharing devices.  To ensure exclusive access to the resources, you must use an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgikeyedmutex">IDXGIKeyedMutex</a> object and then call <b>ReclaimResources</b> only while you hold the mutex. <b>Platform Update for Windows 7:  </b>The runtime validates that <b>ReclaimResources</b> is used correctly on non-shared resources but doesn't perform the intended functionality. For more info about the Platform Update for Windows 7, see <a href="https://docs.microsoft.com/windows/desktop/direct3darticles/platform-update-for-windows-7">Platform Update for Windows 7</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-reclaimresources#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ReclaimResources( uint NumResources,
						   IResource[ ] ppResources,
						   out Span< BOOL > pDiscarded ) ;

	/// <summary>Flushes any outstanding rendering commands and sets the specified event object to the signaled state after all previously submitted rendering commands complete.</summary>
	/// <param name="hEvent">
	/// <para>A handle to the event object. The <a href="https://docs.microsoft.com/windows/desktop/api/synchapi/nf-synchapi-createeventa">CreateEvent</a> or <a href="https://docs.microsoft.com/windows/desktop/api/synchapi/nf-synchapi-openeventa">OpenEvent</a> function returns this handle. All types of event objects (manual-reset, auto-reset, and so on) are supported. The handle must have the EVENT_MODIFY_STATE access right. For more information about access rights, see <a href="https://docs.microsoft.com/windows/desktop/Sync/synchronization-object-security-and-access-rights">Synchronization Object Security and Access Rights</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-enqueuesetevent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Returns <b>S_OK</b> if successful; otherwise, returns one of the following values: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>EnqueueSetEvent</b> calls the <a href="https://docs.microsoft.com/windows/desktop/api/synchapi/nf-synchapi-setevent">SetEvent</a> function on the event object after all previously submitted rendering commands complete or the device is removed. After an application calls <b>EnqueueSetEvent</b>, it  can immediately call the <a href="https://docs.microsoft.com/windows/desktop/api/synchapi/nf-synchapi-waitforsingleobject">WaitForSingleObject</a> function to put itself to sleep until rendering commands complete. You cannot use <b>EnqueueSetEvent</b> to determine work completion that is associated with presentation (<a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-present">IDXGISwapChain::Present</a>); instead, we recommend that you use <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-getframestatistics">IDXGISwapChain::GetFrameStatistics</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-enqueuesetevent#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void EnqueueSetEvent( Win32Handle hEvent ) ;

	// ----------------------------------------------------------
	new static Type ComType => typeof( IDXGIDevice2 ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice2).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==========================================================
} ;


[ProxyFor(typeof(IDXGIDevice3))]
public interface IDevice3: IDevice2 {
	/// <summary>Trims the graphics memory allocated by the IDXGIDevice3 DXGI device on the app's behalf.</summary>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgidevice3-trim">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void Trim( ) ;
	
	new static Type ComType => typeof( IDXGIDevice3 ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice3).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
} ;


[ProxyFor(typeof(IDXGIDevice4))]
public interface IDevice4: IDevice3 {
	
	/// <summary>Allows the operating system to free the video memory of resources, including both discarding the content and de-committing the memory.</summary>
	/// <param name="NumResources">
	/// <para>Type: <b>UINT</b> The number of resources in the <i>ppResources</i> argument array.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-offerresources1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppResources">
	/// <para>Type: <b>IDXGIResource*</b> An array of pointers to <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiresource">IDXGIResource</a> interfaces for the resources to offer.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-offerresources1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Priority">
	/// <para>Type: <b>DXGI_OFFER_RESOURCE_PRIORITY</b> A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_offer_resource_priority">DXGI_OFFER_RESOURCE_PRIORITY</a>-typed value that indicates how valuable data is.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-offerresources1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="Flags">
	/// <para>Type: <b>UINT</b> Specifies the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ne-dxgi1_5-dxgi_offer_resource_flags">DXGI_OFFER_RESOURCE_FLAGS</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-offerresources1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code, which can include E_INVALIDARG if a resource in the array, or the priority, is invalid.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>OfferResources1</b> (an extension of the original <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-offerresources">IDXGIDevice2::OfferResources</a> API) enables D3D based applications to allow de-committing of an allocation’s backing store to reduce system commit under low memory conditions. A de-committed allocation cannot be reused, so opting in to the new DXGI_OFFER_RESOURCE_FLAG_ALLOW_DECOMMIT flag means the new reclaim results must be properly handled. Refer to the flag descriptions in <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ne-dxgi1_5-dxgi_reclaim_resource_results">DXGI_RECLAIM_RESOURCE_RESULTS</a> and the Example below. <b>OfferResources1</b> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-reclaimresources1">ReclaimResources1</a> may <i>not</i> be used interchangeably with <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-offerresources">OfferResources</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-reclaimresources">ReclaimResources</a>.</para>
	/// <para>The priority value that the  <i>Priority</i> parameter specifies describes how valuable the caller considers the content to be.  The operating system uses the priority value to discard resources in order of priority. The operating system discards a resource that is offered with low priority before it discards a resource that is  offered with a higher priority. If you call <b>OfferResources1</b> to offer a resource while the resource is bound to the pipeline, the resource is unbound.  You cannot call <b>OfferResources1</b> on a resource that is mapped.  After you offer a resource, the resource cannot be mapped or bound to the pipeline until you call the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-reclaimresources1">ReclaimResources1</a> method to reclaim the resource. You cannot call <b>OfferResources1</b> to offer immutable resources. To offer shared resources, call <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgidevice2-offerresources">OfferResources1</a> on only one of the sharing devices.  To ensure exclusive access to the resources, you must use an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgikeyedmutex">IDXGIKeyedMutex</a> object and then call <b>OfferResources1</b> only while you hold the mutex. In fact, you can't offer shared resources unless you use <b>IDXGIKeyedMutex</b> because offering shared resources without using <b>IDXGIKeyedMutex</b> isn't supported. The user mode display driver might not immediately offer the resources that you specified in a call to <b>OfferResources1</b>. The driver can postpone offering them until the next call to <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-present">IDXGISwapChain::Present</a>, <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgiswapchain1-present1">IDXGISwapChain1::Present1</a>, or <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11devicecontext-flush">ID3D11DeviceContext::Flush</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-offerresources1#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void OfferResources1( uint                  NumResources,
						  IResource[ ]          ppResources,
						  OfferResourcePriority Priority = OfferResourcePriority.Normal, 
						  OfferResourceFlags    Flags    = OfferResourceFlags.AllowDecommit ) ;

	/// <summary>Restores access to resources that were previously offered by calling IDXGIDevice4::OfferResources1.</summary>
	/// <param name="NumResources">
	/// <para>Type: <b>UINT</b> The number of resources in the <i>ppResources</i> argument and <i>pResults</i> argument arrays.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-reclaimresources1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppResources">
	/// <para>Type: <b>IDXGIResource*</b> An array of pointers to <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgiresource">IDXGIResource</a> interfaces for the resources to reclaim.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-reclaimresources1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pResults">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ne-dxgi1_5-dxgi_reclaim_resource_results">DXGI_RECLAIM_RESOURCE_RESULTS</a>*</b> A pointer to an array that receives <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/ne-dxgi1_5-dxgi_reclaim_resource_results">DXGI_RECLAIM_RESOURCE_RESULTS</a> values. Each value in the array corresponds to a resource at the same index that the <i>ppResources</i> parameter specifies.  The caller can pass in <b>NULL</b>, if the caller intends to fill the resources with new content regardless of whether the old content was discarded.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-reclaimresources1#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code, including E_INVALIDARG if the resources are invalid.</para>
	/// </returns>
	/// <remarks>
	/// <para>After you call <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-offerresources1">OfferResources1</a> to offer one or more resources, you must call <b>ReclaimResources1</b> before you can use those resources again. To reclaim shared resources, call <b>ReclaimResources1</b> only on one of the sharing devices.  To ensure exclusive access to the resources, you must use an <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgikeyedmutex">IDXGIKeyedMutex</a> object and then call <b>ReclaimResources1</b> only while you hold the mutex.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/nf-dxgi1_5-idxgidevice4-reclaimresources1#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void ReclaimResources1( uint NumResources,
							IResource[ ] ppResources,
							out Span< ReclaimResourceResults > pResults ) ;

	
	new static Type ComType => typeof( IDXGIDevice4 ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice4).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
} ;