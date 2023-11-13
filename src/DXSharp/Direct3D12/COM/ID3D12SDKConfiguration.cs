#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace Windows.Win32.Graphics.Direct3D12 ;


/// <summary>Provides SDK configuration methods.</summary>
/// <remarks>
/// A pointer to this interface can be retrieved by calling the D3D12GetInterface free function
/// with the <b><c>CLSID_D3D12SDKConfiguration</c></b> CLSID.
/// </remarks>
[ComImport, Guid("E9EB5314-33AA-42B2-A718-D77F58B1F1C7"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown),]
public interface ID3D12SDKConfiguration: IUnknown {
	// -----------------------------------------------------------------------
	/// <summary>Configures the SDK version to use.</summary>
	/// <param name="SDKVersion">
	/// <para>Type: **[UINT](/windows/win32/winprog/windows-data-types)** The SDK version to set.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12sdkconfiguration-setsdkversion#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="SDKPath">
	/// <para>Type: \_In\_z\_ **[LPCSTR](/windows/win32/winprog/windows-data-types)** A NULL-terminated string that provides the relative path to `d3d12core.dll` at the specified *SDKVersion*. The path is relative to the process exe of the caller. If `d3d12core.dll` isn't found, or isn't of the specified *SDKVersion*, then Direct3D 12 device creation fails.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12sdkconfiguration-setsdkversion#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, then it returns **S_OK**. Otherwise, it returns one of the [Direct3D 12 return codes](/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues).</para>
	/// </returns>
	/// <remarks>
	/// <para>This method can be used only in Windows Developer Mode. To set the SDK version using this API, you must call it before you create the Direct3D 12 device. Calling this API *after* creating the Direct3D 12 device will cause the Direct3D 12 runtime to remove the device. If the `d3d12core.dll` installed with the OS is newer than the SDK version specified, then the OS version is used instead. You can retrieve the version of a particular `D3D12Core.dll` from the exported symbol [**D3D12SDKVersion**](/windows/win32/direct3d12/nf-d3d12-d3d12sdkversion), which is a variable of type **UINT**, just like the variables exported from applications to enable use of the Agility SDK.</para>
	/// <para><see href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12sdkconfiguration-setsdkversion#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	[PreserveSig] HResult SetSDKVersion( uint SDKVersion, PCSTR SDKPath ) ;
	// -----------------------------------------------------------------------
} ;