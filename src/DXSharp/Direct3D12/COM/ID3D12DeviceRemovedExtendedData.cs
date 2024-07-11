#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
#region Using Directives
using DXSharp.Direct3D12 ;
using System.Runtime.InteropServices;
using DXSharp.Windows.COM ;
#endregion
namespace Windows.Win32.Graphics.Direct3D12;


[ComImport, Guid( "98931D33-5AE8-4791-AA3C-1A73A2934E71" ),
 InterfaceType( ComInterfaceType.InterfaceIsIUnknown ),]
[global::System.CodeDom.Compiler.GeneratedCode( "Microsoft.Windows.CsWin32", "0.3.49-beta+91f5c15987" )]
public interface ID3D12DeviceRemovedExtendedData: IUnknown {
	
	/// <summary>Retrieves the Device Removed Extended Data (DRED) auto-breadcrumbs output after device removal.</summary>
	/// <param name="pOutput">An output parameter that takes the address of a [D3D12_DRED_AUTO_BREADCRUMBS_OUTPUT](ns-d3d12-d3d12_dred_auto_breadcrumbs_output.md) object. The object whose address is passed receives the data.</param>
	/// <returns>
	/// If the function succeeds, it returns **S_OK**. Otherwise, it returns an <see cref="HResult"/>.
	/// Returns **DXGI_ERROR_NOT_CURRENTLY_AVAILABLE** if the device is *not* in a removed state.
	/// Returns **DXGI_ERROR_UNSUPPORTED** if auto-breadcrumbs have not been enabled with <see cref="ID3D12DeviceRemovedExtendedDataSettings.SetAutoBreadcrumbsEnablement"/>.</returns>
	/// <remarks>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12deviceremovedextendeddata-getautobreadcrumbsoutput">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetAutoBreadcrumbsOutput( out DREDAutoBreadCrumbOutput pOutput ) ;

	/// <summary>Retrieves the Device Removed Extended Data (DRED) page fault data.</summary>
	/// <param name="pOutput">An output parameter that takes the address of a [D3D12_DRED_PAGE_FAULT_OUTPUT](ns-d3d12-d3d12_dred_page_fault_output.md) object.</param>
	/// <returns>If the function succeeds, it returns **S_OK**. Otherwise, it returns an [HRESULT](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/desktop/com/com-error-codes-10). Returns **DXGI_ERROR_NOT_CURRENTLY_AVAILABLE** if the device is *not* in a removed state. Returns **DXGI_ERROR_UNSUPPORTED** if page fault reporting has not been enabled with [ID3D12DeviceRemovedExtendedDataSettings::SetPageFaultEnablement](nf-d3d12-id3d12deviceremovedextendeddatasettings-setpagefaultenablement.md).</returns>
	/// <remarks>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12deviceremovedextendeddata-getpagefaultallocationoutput">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	unsafe void GetPageFaultAllocationOutput( DREDPageFaultOutput* pOutput ) ;
} ;
		