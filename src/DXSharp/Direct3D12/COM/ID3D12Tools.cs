#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
#region Using Directives
using System.Runtime.InteropServices;
using Windows.Win32.Foundation ;
using DXSharp.Windows.COM ;

#endregion
namespace Windows.Win32.Graphics.Direct3D12;


[ComImport, Guid("7071E1F0-E84B-4B33-974F-12FA49DE65C5"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown),]
public interface ID3D12Tools: IUnknown {
	
	/// <summary>This method enables tools such as PIX to instrument shaders.</summary>
	/// <param name="bEnable">
	/// <para>TRUE to enable shader instrumentation; otherwise, FALSE.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12tools-enableshaderinstrumentation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>Do not use this interface in your application, it's not intended or supported for any scenario other than to enable tooling such as PIX.
	/// Developer Mode must be enabled for this interface to respond.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12tools-enableshaderinstrumentation#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] void EnableShaderInstrumentation( BOOL bEnable ) ;

	/// <summary>Determines whether shader instrumentation is enabled.</summary>
	/// <returns>
	/// <para>Returns TRUE if shader instrumentation is enabled; otherwise FALSE.</para>
	/// </returns>
	/// <remarks>
	/// <para>Do not use this interface in your application, it's not intended or supported for
	/// any scenario other than to enable tooling such as PIX.
	/// Developer Mode must be enabled for this interface to respond.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12tools-shaderinstrumentationenabled#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] BOOL ShaderInstrumentationEnabled( ) ;
} ;