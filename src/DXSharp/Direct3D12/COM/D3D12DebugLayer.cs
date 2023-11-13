#region Using Directives
#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
using DXSharp.Direct3D12 ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using global::System.Runtime.InteropServices;
using winmdroot = global::Windows.Win32;
#endregion
namespace Windows.Win32.Graphics.Direct3D12 ;


/// <summary>An interface used to turn on the debug layer. See EnableDebugLayer for more information.</summary>
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown), 
 ComImport, Guid("344488B7-6846-474B-B989-F027448245E0"),]
public interface ID3D12Debug: IUnknown {
	/// <summary>Enables the debug layer. (ID3D12Debug.EnableDebugLayer)</summary>
	/// <remarks>To enable the debug layers using this API, it must be called before the D3D12 device is created. Calling this API after creating the D3D12 device will cause the D3D12 runtime to remove the device.</remarks>
	[PreserveSig] HResult EnableDebugLayer( ) ;
} ;


/// <summary>Adds GPU-Based Validation and Dependent Command Queue Synchronization to the debug layer.</summary>
[ComImport, Guid("AFFAA4CA-63FE-4D8E-B8AD-159000AF4304"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown),]
public interface ID3D12Debug1: IUnknown  {
	/// <summary>Enables the debug layer. (ID3D12Debug1.EnableDebugLayer)</summary>
	/// <remarks>This method is identical to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug-enabledebuglayer">ID3D12Debug::EnableDebugLayer</a>.</remarks>
	[PreserveSig] void EnableDebugLayer( ) ;

	/// <summary>This method enables or disables GPU-Based Validation (GBV) before creating a device with the debug layer enabled.</summary>
	/// <param name="Enable">
	/// <para>Type: <b>BOOL</b> TRUE to enable GPU-Based Validation, otherwise FALSE.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug1-setenablegpubasedvalidation#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para>GPU-Based Validation can only be enabled/disabled prior to creating a device.  By default, GPU-Based Validation is disabled.  To disable GPU-Based Validation after initially enabling it the device must be fully released and recreated. For more information, see <a href="https://docs.microsoft.com/windows/desktop/direct3d12/using-d3d12-debug-layer-gpu-based-validation">Using D3D12 Debug Layer GPU-Based Validation</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug1-setenablegpubasedvalidation#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	[PreserveSig] void SetEnableGPUBasedValidation( winmdroot.Foundation.BOOL Enable ) ;

	/// <summary>Enables or disables dependent command queue synchronization when using a D3D12 device with the debug layer enabled.</summary>
	/// <param name="Enable">
	/// <para>Type: <b>BOOL</b> TRUE to enable Dependent Command Queue Synchronization, otherwise FALSE.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug1-setenablesynchronizedcommandqueuevalidation#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para>Dependent Command Queue Synchronization is a D3D12 Debug Layer feature that gives the debug layer the ability to track resource states more accurately when enabled.  Dependent Command Queue Synchronization is enabled by default. When Dependent Command Queue Synchronization is enabled, the debug layer holds back actual submission of GPU work until all outstanding fence <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-wait">Wait</a> conditions are met.  This gives the debug layer the ability to make reasonable assumptions about GPU state (such as resource states) on the CPU-timeline when multiple command queues are potentially doing concurrent work. With Dependent Command Queue Synchronization disabled, all resource states tracked by the debug layer are cleared each time <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-signal">ID3D12CommandQueue::Signal</a> is called.  This results in significantly less useful resource state validation. Disabling Dependent Command Queue Synchronization may reduce some debug layer performance overhead when using multiple command queues.  However, it is suggested to leave it enabled unless this overhead is problematic.  Note that applications that use only a single command queue will see no performance changes with Dependent Command Queue Synchronization disabled.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug1-setenablesynchronizedcommandqueuevalidation#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	[PreserveSig] void SetEnableSynchronizedCommandQueueValidation( winmdroot.Foundation.BOOL Enable ) ;
} ;


/// <summary>
/// Adds configurable levels of GPU-based validation to the debug layer.
/// </summary>
[ComImport, Guid("93A665C4-A3B2-4E5D-B692-A26AE14E3374"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ID3D12Debug2: IUnknown {
	/// <summary>This method configures the level of GPU-based validation that the debug device is to perform at runtime. (ID3D12Debug2.SetGPUBasedValidationFlags)</summary>
	/// <param name="Flags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/ne-d3d12sdklayers-d3d12_gpu_based_validation_flags">D3D12_GPU_BASED_VALIDATION_FLAGS</a></b> Specifies the level of GPU-based validation to perform at runtime.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug2-setgpubasedvalidationflags#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>This method overrides the default behavior of GPU-based validation so it must be called before creating the Direct3D 12 device. These settings can't be changed or cancelled after the device is created. If you want to change the behavior of GPU-based validation at a later time, the device must be destroyed and recreated with different parameters.</remarks>
	[PreserveSig] void SetGPUBasedValidationFlags( GPUBasedValidationFlags Flags ) ;
} ;

[Guid( "5CF4E58F-F671-4FF1-A542-3686E3D153D1" ), InterfaceType( ComInterfaceType.InterfaceIsIUnknown ), ComImport( )]
[global::System.CodeDom.Compiler.GeneratedCode( "Microsoft.Windows.CsWin32", "0.3.18-beta+dc807e7787" )]
public interface ID3D12Debug3: ID3D12Debug {
	
	/// <summary>This method enables or disables GPU-based validation (GBV) before creating a device with the debug layer enabled.</summary>
	/// <param name="Enable">
	/// <para>Type: <b>BOOL</b> TRUE to enable GPU-based validation, otherwise FALSE.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug3-setenablegpubasedvalidation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>GPU-based validation can be enabled/disabled only prior to creating a device. By default, GPU-based validation is disabled. To disable GPU-based validation after initially enabling it, the device must be fully released and recreated. For more information, see <a href="https://docs.microsoft.com/windows/win32/direct3d12/using-d3d12-debug-layer-gpu-based-validation">Using D3D12 Debug Layer GPU-based validation</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug3-setenablegpubasedvalidation#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] void SetEnableGPUBasedValidation( winmdroot.Foundation.BOOL Enable ) ;

	/// <summary>Enables or disables dependent command queue synchronization when using a Direct3D 12 device with the debug layer enabled.</summary>
	/// <param name="Enable">
	/// <para>Type: <b>BOOL</b> TRUE to enable Dependent Command Queue Synchronization, otherwise FALSE.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug3-setenablesynchronizedcommandqueuevalidation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>Dependent Command Queue Synchronization is a D3D12 Debug Layer feature that gives the debug layer the ability to track resource states more accurately when enabled. Dependent Command Queue Synchronization is enabled by default. When Dependent Command Queue Synchronization is enabled, the debug layer holds back actual submission of GPU work until all outstanding fence <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-wait">Wait</a> conditions are met. This gives the debug layer the ability to make reasonable assumptions about GPU state (such as resource states) on the CPU-timeline when multiple command queues are potentially doing concurrent work. With Dependent Command Queue Synchronization disabled, all resource states tracked by the debug layer are cleared each time <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-signal">ID3D12CommandQueue::Signal</a> is called. This results in significantly less useful resource state validation. Disabling Dependent Command Queue Synchronization may reduce some debug layer performance overhead when using multiple command queues. However, it is suggested to leave it enabled unless this overhead is problematic. Note that applications that use only a single command queue will see no performance changes with Dependent Command Queue Synchronization disabled.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug3-setenablesynchronizedcommandqueuevalidation#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] void SetEnableSynchronizedCommandQueueValidation( winmdroot.Foundation.BOOL Enable ) ;

	/// <summary>This method configures the level of GPU-based validation that the debug device is to perform at runtime. (ID3D12Debug3.SetGPUBasedValidationFlags)</summary>
	/// <param name="Flags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12sdklayers/ne-d3d12sdklayers-d3d12_gpu_based_validation_flags">D3D12_GPU_BASED_VALIDATION_FLAGS</a></b> Specifies the level of GPU-based validation to perform at runtime.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug3-setgpubasedvalidationflags#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>This method overrides the default behavior of GPU-based validation so it must be called before creating the D3D12 Device. These settings can't be changed or cancelled after the device is created. If you want to change the behavior of GPU-based validation at a later time, the device must be destroyed and recreated with different parameters.</remarks>
	[PreserveSig] void SetGPUBasedValidationFlags( GPUBasedValidationFlags Flags ) ;
} ;


/// <summary>Adds the ability to disable the debug layer.</summary>
[ComImport, Guid("014B816E-9EC5-4A2F-A845-FFBE441CE13A"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ID3D12Debug4: ID3D12Debug3 {
	/// <summary>Disables the debug layer.</summary>
	[PreserveSig] HResult DisableDebugLayer( ) ;
} ;


/// <summary>Adds to the debug layer the ability to configure the auto-naming of objects.</summary>
[ComImport, Guid( "548D6B12-09FA-40E0-9069-5DCD589A52C9" ),
 InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface ID3D12Debug5: ID3D12Debug4 {
	/// <summary>Configures the auto-naming of objects.</summary>
	/// <param name="Enable">
	/// <para>Type: **[BOOL](/windows/desktop/winprog/windows-data-types)** `true` to enable auto-naming; `false` to disable auto-naming.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug5-setenableautoname#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>By default, objects are not named unless you use [ID3D12Object::SetName](/windows/win32/api/d3d12/nf-d3d12-id3d12object-setname) or [ID3D12Object::SetPrivateData](/windows/win32/api/d3d12/nf-d3d12-id3d12object-setprivatedata) to assign a name. It's a best practice to name all of your Direct3D 12 objects; at least in debug builds. Failing that, you might find it convenient to allow automatic name assignment in order to cover the gaps. Direct3D 12 objects created with auto-name enabled are automatically assigned a name, which is used for debug layer output and for DRED page fault data. So as not to create a dependency on a specific auto-naming format, you can't retrieve the auto-name strings by using ID3D12Object::GetName or [ID3D12Object::GetPrivateData](/windows/win32/api/d3d12/nf-d3d12-id3d12object-getprivatedata). But, to generate a unique name string, Direct3D 12 uses the locally-unique identifier (LUID) assigned to every **ID3D12DeviceChild** object at create time. You can retrieve that LUID by using **ID3D12Object::GetPrivateData** with the **REFGUID** value *WKPDID_D3D12UniqueObjectId*. You might find that useful for your own object-naming schemas. When debugging existing software, you can control auto-naming by using the *D3DConfig* graphics tools utility and the command `d3dconfig.exe device auto-debug-name=forced-on`. Any object given a name using [ID3D12Object::SetName](/windows/win32/api/d3d12/nf-d3d12-id3d12object-setname) or [ID3D12Object::SetPrivateData](/windows/win32/api/d3d12/nf-d3d12-id3d12object-setprivatedata) uses the assigned name instead of the auto-name.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug5-setenableautoname#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] void SetEnableAutoName( winmdroot.Foundation.BOOL Enable ) ;
} ;


/// <summary>Requires the DirectX 12 Agility SDK 1.7 or later.</summary>
[ComImport, Guid( "82A816D6-5D01-4157-97D0-4975463FD1ED" ),
 InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface ID3D12Debug6: ID3D12Debug5 {
	/// <summary>TBD</summary>
	/// <param name="Enable"></param>
	/// <remarks>Requires the DirectX 12 Agility SDK 1.7 or later.</remarks>
	[PreserveSig] void SetForceLegacyBarrierValidation( winmdroot.Foundation.BOOL Enable ) ;
} ;

