#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981

#region Using Directives
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

using DXSharp ;
using DXSharp.Windows ;
using DXSharp.DXGI.Debug ;
using DXSharp.Windows.COM ;
#endregion
namespace Windows.Win32.Graphics.Dxgi ;


/// <summary>
/// This interface controls debug settings, and can
/// only be used if the debug layer is turned on.
/// </summary>
[SupportedOSPlatform("windows8.0")]
[ ComImport, Guid("119E7452-DE9E-40FE-8806-88F90C12B441"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ]
[ NativeLibrary("dxgi.dll", "IDXGIDebug", 
				"Dxgidebug.h") ]
public interface IDXGIDebug: IUnknown {
	/// <summary>Reports info about the lifetime of an object or objects.</summary>
	/// <param name="apiid">The globally unique identifier (GUID) of the object or objects to get info about. Use one of the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> GUIDs.</param>
	/// <param name="flags">A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_debug_rlo_flags">DXGI_DEBUG_RLO_FLAGS</a>-typed value that specifies the amount of info to report.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para> <a href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgidebug-reportliveobjects#">Read more on docs.microsoft.com </a>.</para>
	/// </remarks>
	HResult ReportLiveObjects( Guid apiid, DebugRLOFlags flags ) ;
} ;


[SupportedOSPlatform("windows8.1")]
[ComImport, Guid("C5A05F0C-16F2-4ADF-9F4D-A8C4D58AC550"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ]
public interface IDXGIDebug1: IDXGIDebug {
	
	/// <summary>Starts tracking leaks for the current thread.</summary>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgidebug1-enableleaktrackingforthread">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void EnableLeakTrackingForThread( ) ;

	/// <summary>Stops tracking leaks for the current thread.</summary>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgidebug1-disableleaktrackingforthread">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void DisableLeakTrackingForThread( ) ;

	/// <summary>Gets a value indicating whether leak tracking is turned on for the current thread.</summary>
	/// <returns><b>TRUE</b> if leak tracking is turned on for the current thread; otherwise, <b>FALSE</b>.</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgidebug1-isleaktrackingenabledforthread">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	[return: MarshalAs(UnmanagedType.Bool)]
	bool IsLeakTrackingEnabledForThread( ) ;
} ;