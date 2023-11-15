#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
#region Using Directives
using DXSharp.Direct3D12 ;
using System.Runtime.InteropServices;
using DXSharp.Windows.COM ;

#endregion
namespace Windows.Win32.Graphics.Direct3D12;
	

[ComImport, Guid("82BC481C-6B9B-4030-AEDB-7EE3D1DF1E63"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown),]
public interface ID3D12DeviceRemovedExtendedDataSettings: IUnknown {
	/// <summary>Configures the enablement settings for Device Removed Extended Data (DRED) auto-breadcrumbs.</summary>
	/// <param name="Enablement">A <see cref="DREDEnablement"/> value. The default is <see cref="DREDEnablement.SystemControlled"/>.</param>
	/// <remarks>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12deviceremovedextendeddatasettings-setautobreadcrumbsenablement">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] void SetAutoBreadcrumbsEnablement( DREDEnablement Enablement ) ;

	/// <summary>Configures the enablement settings for Device Removed Extended Data (DRED) page fault reporting.</summary>
	/// <param name="Enablement">A <see cref="DREDEnablement"/> value. The default is <see cref="DREDEnablement.SystemControlled"/>.</param>
	/// <remarks>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12deviceremovedextendeddatasettings-setpagefaultenablement">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] void SetPageFaultEnablement( DREDEnablement Enablement ) ;

	/// <summary>Configures the enablement settings for Device Removed Extended Data (DRED) Watson dump creation.</summary>
	/// <param name="Enablement">A <see cref="DREDEnablement"/> value. The default is <see cref="DREDEnablement.SystemControlled"/>.</param>
	/// <remarks>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12deviceremovedextendeddatasettings-setwatsondumpenablement">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] void SetWatsonDumpEnablement( DREDEnablement Enablement ) ;
} ;