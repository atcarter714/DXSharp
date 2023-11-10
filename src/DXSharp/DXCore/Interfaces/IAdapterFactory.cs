#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.DXCore ;

using DXSharp.DXGI ;
using DXSharp.Windows ;
#endregion
namespace DXSharp.DXCore.Interfaces ;

//! TODO: Complete this interface by converting it to our DXSharp "style" and standards ...
// (i.e., replacing parameters types, renaming things, using `nint` instead of `void*`, etc.)

public interface IAdapterFactory {
	
	/// <summary>Generates a list of adapter objects representing the current adapter state of the system, and meeting the criteria specified.</summary>
	/// <param name="numAttributes">
	/// <para>Type: **uint32_t** The number of elements in the array pointed to by the *filterAttributes* argument.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-createadapterlist#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="filterAttributes">
	/// <para>Type: **const GUID\*** A pointer to an array of adapter attribute GUIDs. For a list of attribute GUIDs, see [DXCore adapter attribute GUIDs]( /windows/win32/dxcore/dxcore-adapter-attribute-guids). At least one GUID must be provided. In the case that more than one GUID is provided in the array, only adapters that meet *all* of the requested attributes will be included in the returned list.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-createadapterlist#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier ( GUID) of the interface that you wish to be returned in *ppvAdapterList*. This is expected to be the interface identifier ( IID) of [IDXCoreAdapterList]( /windows/win32/api/dxcore_interface/nn-dxcore_interface-idxcoreadapterlist).</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-createadapterlist#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvAdapterList">
	/// <para>Type: **void\*\*** The address of a pointer to an interface with the IID specified in the *riid* parameter. Upon successful return, *\*ppvAdapterList* ( the dereferenced address) contains a pointer to the the adapter list created.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-createadapterlist#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT]( /windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**]( /windows/win32/com/structure-of-com-error-codes) [error code]( /windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_INVALIDARG|`nullptr` was provided for *filterAttributes*, or 0 was provided for *numAttributes*.| |E_NOINTERFACE|An invalid value was provided for *riid*.| |E_POINTER|`nullptr` was provided for *ppvAdapterList*.|</para>
	/// </returns>
	/// <remarks>
	/// <para>Even if no adapters are found, as long as the arguments are valid, **CreateAdapterList** creates a valid [IDXCoreAdapterList]( /windows/win32/api/dxcore_interface/nn-dxcore_interface-idxcoreadapterlist) object, and returns **S_OK**. Once generated, the adapters in this specific list won't change. But the list will be considered stale if one of the adapters later becomes invalid, or if a new adapter arrives that meets the provided filter criteria. The list returned by **CreateAdapterList** is not ordered in any particular way, and multiple calls to **CreateAdapterList** may produce differently ordered lists. The resulting list is not ordered in any particular way, but the ordering of a list is consistent across multiple calls, and even across operating system restarts. The ordering may change upon system configuration changes, including the addition or removal of an adapter, or a driver update on an existing adapter.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-createadapterlist#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	unsafe void CreateAdapterList( uint numAttributes, Guid* filterAttributes, Guid* riid, out object ppvAdapterList ) ;

	
	/// <summary>Retrieves the DXCore adapter object ( [IDXCoreAdapter]( /windows/win32/api/dxcore_interface/nn-dxcore_interface-idxcoreadapter)) for a specified Luid, if available.</summary>
	/// <param name="adapterLuid">
	/// <para>Type: **[Luid]( /windows/win32/api/winnt/ns-winnt-_luid) const\&** The locally unique value that identifies the adapter instance.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-getadapterbyluid#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="riid">
	/// <para>Type: **REFIID** A reference to the globally unique identifier ( GUID) of the interface that you wish to be returned in *ppvAdapter*. This is expected to be the interface identifier ( IID) of [IDXCoreAdapter]( /windows/win32/api/dxcore_interface/nn-dxcore_interface-idxcoreadapter).</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-getadapterbyluid#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="ppvAdapter">
	/// <para>Type: **void\*\*** The address of a pointer to an interface with the IID specified in the *riid* parameter. Upon successful return, *\*ppvAdapter* ( the dereferenced address) contains a pointer to the the DXCore adapter created.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-getadapterbyluid#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT]( /windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**]( /windows/win32/com/structure-of-com-error-codes) [error code]( /windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |DXGI_ERROR_DEVICE_REMOVED|The adapter Luid passed in *adapterLuid* is recognized, but the adapter is no longer in a valid state.| |E_INVALIDARG|No such adapter Luid as the value passed in *adapterLuid* is available through DXCore.| |E_NOINTERFACE|An invalid value was provided for *riid*.| |E_POINTER|`nullptr` was provided for *ppvAdapter*.|</para>
	/// </returns>
	/// <remarks>Multiple calls passing the same [Luid]( /windows/win32/api/winnt/ns-winnt-_luid) return identical interface pointers. As a result, it's safe to compare interface pointers to determine whether multiple pointers refer to the same adapter object.</remarks>
	unsafe void GetAdapterByLuid( Luid* adapterLuid, Guid* riid, out object ppvAdapter ) ;

	
	/// <summary>Determines whether a specified notification type is supported by the operating system ( OS).</summary>
	/// <param name="notificationType">
	/// <para>Type: **[DXCoreNotificationType]( /windows/win32/api/dxcore_interface/ne-dxcore_interface-dxcorenotificationtype)** The type of notification that you're querying about support for. See the table in [DXCoreNotificationType]( /windows/win32/api/dxcore_interface/ne-dxcore_interface-dxcorenotificationtype) for info about the notification types.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-isnotificationtypesupported#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **bool** Returns `true` if the notification type is supported by the system. Otherwise, returns `false`.</para>
	/// </returns>
	/// <remarks>You can call **IsNotificationTypeSupported** to determine whether a given notification type is known to this version of the OS. For example, a notification type that's introduced in a particular version of Windows is unknown to previous versions of Windows.</remarks>
	bool IsNotificationTypeSupported( DXCoreNotificationType notificationType ) ;

	
	/// <summary>Registers to receive notifications of specific conditions from a DXCore adapter or adapter list.</summary>
	/// <param name="dxCoreObject">
	/// <para>The DXCore object (<see cref="IDXCoreAdapter"/> or <see cref="IDXCoreAdapterList"/>) whose notifications you're subscribing to.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-registereventnotification#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="notificationType">
	/// <para>The type of notification that you're registering for. See the table in <see cref="DXCoreNotificationType"/> for info about what types are valid with which kinds of objects.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-registereventnotification#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="callbackFunction">
	/// <para>A pointer to a callback function ( implemented by your application), which is called by the DXCore object for notification events.
	/// For the signature of the function, see <see cref="PFN_DXCORE_NOTIFICATION_CALLBACK"/>.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-registereventnotification#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="callbackContext">
	/// <para>Type: **void\*** An optional pointer to an object containing context info. This object is passed to your callback function when the notification is raised.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-registereventnotification#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="eventCookie"><para>
	/// A pointer to a <b>uint32_t</b> value.
	/// If successful, the function dereferences the pointer and sets the value to a non-zero cookie value representing this registration.
	/// Use this cookie value to unregister from the notification by calling <see cref="IAdapterFactory.UnregisterEventNotification"/>
	/// If unsuccessful, the function dereferences the pointer and sets the value to zero, which represents an invalid cookie value.
	/// </para>
	/// <para>
	/// <a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-registereventnotification#parameters">
	/// Read more on docs.microsoft.com</a>.
	/// </para>
	/// </param>
	/// <returns>
	/// <para>If the function succeeds, it returns <b>S_OK</b>.
	/// Otherwise, it returns an <see cref="HResult"/> of:<para/>
	/// <b>DXGI_ERROR_INVALID_CALL</b>: <i>notificationType</i> is unsupported by the operating system (OS) ...<para/>
	/// <b>E_INVALIDARG</b>: `nullptr` was provided for *dxCoreObject*, or if an invalid <i>notificationType</i> and <i>dxCoreObject</i> combination was provided.<para/>
	/// <b>E_POINTER</b>: `nullptr` was provided for either <i>callbackFunction</i> or <i>eventCookie</i>.<para/>
	/// </para>
	/// </returns>
	/// <remarks><para>
	/// You use <b>RegisterEventNotification</b> to register for events raised by <see cref="IDXCoreAdapterList"/> and
	/// <see cref="IDXCoreAdapter"/> interfaces. These notification types are supported:<para/>
	/// <para><b>AdapterListStale</b>: Indicates that the list of adapters meeting your filter criteria has changed.
	/// If the adapter list is stale at the time of registration, then your callback is immediately called.
	/// This callback occurs at most one time per registration.</para>
	/// <para><b>AdapterNoLongerValid</b>: Indicates that the adapter is no longer valid. If the adapter is invalid at registration time, then your callback
	/// is immediately called.</para>
	/// <para><b>AdapterBudgetChange</b>: Indicates that a memory budgeting event has occurred, and that you should call
	/// <see cref="IDXCoreAdapter.QueryState"/> with <c>DXCoreAdapterState::AdapterMemoryBudget</c> to evaluate the current memory budget state.
	/// Upon registration, an initial callback will always occur to allow you to query the initial state.</para>
	/// <b>AdapterHardwareContentProtectionTeardown</b>: Indicates that you should re-evaluate the current crypto session status;
	/// for example, by calling <c>ID3D11VideoContext1::CheckCryptoSessionStatus</c> to determine the impact of the hardware teardown
	/// for a specific <b>ID3D11CryptoSession</b> interface. Upon registration, an initial callback will always occur to allow you to query the initial state.|
	/// A call to the function that you provide in *callbackFunction* is made asynchronously on a background thread by DXCore when the detected event occurs.
	/// No guarantee is made as to the ordering or timing of callbacks; multiple callbacks may occur in any order, or even simultaneously.
	/// It's even possible for your callback to be invoked before **RegisterEventNotification** has completed.
	/// In that case, DXCore guarantees that your *eventCookie* is set before your callback is called.
	/// Multiple callbacks for a specific registration will be serialized in order. Callbacks may occur at any time until you call
	/// <see cref="UnregisterEventNotification"/>, and it completes. Callbacks occur on their own threads, and you may make calls into the DXCore API
	/// on those threads, including <b>UnregisterEventNotification</b>, and releasing of the <i>dxCoreObject</i>.<para/>
	///
	/// <b>[!IMPORTANT]</b> Before you destroy the DXCore object represented by the <i>dxCoreObject</i> argument passed to
	/// <see cref="RegisterEventNotification"/>, you must use the cookie value to unregister that object from notifications by calling
	/// <see cref="UnregisterEventNotification"/>. If you don't do that, then a fatal exception is raised when the situation is detected.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-registereventnotification#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	unsafe void RegisterEventNotification( object dxCoreObject,
										   DXCoreNotificationType notificationType, 
										   PFN_DXCORE_NOTIFICATION_CALLBACK callbackFunction, 
										   [Optional] void* callbackContext, out uint eventCookie ) ;

	
	/// <summary>Unregisters from a notification that you previously registered for.</summary>
	/// <param name="eventCookie">
	/// <para>Type: **uint32_t** The cookie value ( returned when you called [IDXCoreAdapterFactory::RegisterEventNotification]( /windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-registereventnotification)) representing a prior registration that you now wish to unregister for.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-unregistereventnotification#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT]( /windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**]( /windows/win32/com/structure-of-com-error-codes) [error code]( /windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| |E_INVALIDARG|The value of *eventCookie* is not a valid cookie representing a prior registration.|</para>
	/// </returns>
	/// <remarks>
	/// <para>**UnregisterEventNotification** returns only after all pending/in-progress callbacks for this registration have completed. DXCore guarantees that no new callbacks will occur for this registration after **UnregisterEventNotification** has returned. However, to avoid a deadlock, if you call **UnregisterEventNotification** from within your callback, then **UnregisterEventNotification** doesn't wait for the active callback to complete. > [!IMPORTANT] > Before you destroy the DXCore object represented by the *dxCoreObject* argument passed to [IDXCoreAdapterFactory::RegisterEventNotification]( /windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-registereventnotification), you must use the cookie value to unregister that object from notifications by calling **UnregisterEventNotification**. If you don't do that, then a fatal exception is raised when the situation is detected. Once you unregister a cookie value, that value is then eligible for being returned by a subsequent registration</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/dxcore_interface/nf-dxcore_interface-idxcoreadapterfactory-unregistereventnotification#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void UnregisterEventNotification( uint eventCookie ) ;
} ;