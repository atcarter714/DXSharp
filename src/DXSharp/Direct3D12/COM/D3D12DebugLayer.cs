#region Using Directives
#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
using DXSharp.Windows.COM ;
using global::System;
using global::System.Diagnostics;
using global::System.Diagnostics.CodeAnalysis;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.Versioning;
using winmdroot = global::Windows.Win32;
#endregion
namespace Windows.Win32.Graphics.Direct3D12 ;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown), 
 ComImport, Guid("344488B7-6846-474B-B989-F027448245E0"),]
public interface ID3D12Debug: IUnknown {
	/// <summary>Enables the debug layer. (ID3D12Debug.EnableDebugLayer)</summary>
	/// <remarks>To enable the debug layers using this API, it must be called before the D3D12 device is created. Calling this API after creating the D3D12 device will cause the D3D12 runtime to remove the device.</remarks>
	[PreserveSig] void EnableDebugLayer( ) ;
} ;