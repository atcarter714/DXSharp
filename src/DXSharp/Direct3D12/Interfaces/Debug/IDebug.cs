#region Using Directives
using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Direct3D12 ;
using DXSharp.Direct3D12.Debugging ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12Debug)), 
 NativeLibrary("d3d12", "ID3D12Debug",
			   "d3d12sdklayers.h", "DEBUG_LAYER")]
public interface IDebug: IInstantiable, IComIID, IDisposable, IAsyncDisposable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	[SuppressMessage( "Interoperability", 
					  "CA1416:Validate platform compatibility" )] 
	internal static readonly ReadOnlyDictionary< Guid, Func<IUnknown, IInstantiable> > _layerCreationFunctions =
		new( new Dictionary<Guid, Func< IUnknown, IInstantiable > > {
			{ IDebug.IID, ( pComObj ) => new Debug( (pComObj as ID3D12Debug)! ) },
			{ IDebug1.IID, ( pComObj ) => new Debug1( (pComObj as ID3D12Debug1)! ) },
			{ IDebug2.IID, ( pComObj ) => new Debug2( (pComObj as ID3D12Debug2)! ) },
			{ IDebug3.IID, ( pComObj ) => new Debug3( (pComObj as ID3D12Debug3)! ) },
			{ IDebug4.IID, ( pComObj ) => new Debug4( (pComObj as ID3D12Debug4)! ) },
			{ IDebug5.IID, ( pComObj ) => new Debug5( (pComObj as ID3D12Debug5)! ) },
			{ IDebug6.IID, ( pComObj ) => new Debug6( (pComObj as ID3D12Debug6)! ) },
		} ) ;
	// ---------------------------------------------------------------------------------

	
	/// <summary>Enables the debug layer. (ID3D12Debug.EnableDebugLayer)</summary>
	/// <remarks>
	/// To enable the debug layers using this API, it must be called before the D3D12 device is created.
	/// Calling this API after creating the D3D12 device will cause the D3D12 runtime to remove the device.
	/// </remarks>
	HResult EnableDebugLayer( ) ;
	
	static Type ComType => typeof( ID3D12Debug ) ;
	public static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Debug).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new Debugging.Debug( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr ptr ) => new Debugging.Debug( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< T >( T obj ) => new Debugging.Debug( (ID3D12Debug)obj! ) ;
} ;


[ProxyFor(typeof(ID3D12Debug1)), 
 NativeLibrary("d3d12", "ID3D12Debug1",
			   "d3d12sdklayers.h", "DEBUG_LAYER")]
public interface IDebug1: IInstantiable, IComIID, IDisposable, IAsyncDisposable {
	static Type ComType => typeof(ID3D12Debug1) ;
	public static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Debug1).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	
	/// <summary>Enables the debug layer. (ID3D12Debug1.EnableDebugLayer)</summary>
	/// <remarks>This method is identical to <a href="https://docs.microsoft.com/windows/desktop/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug-enabledebuglayer">ID3D12Debug::EnableDebugLayer</a>.</remarks>
	void EnableDebugLayer( ) ;

	/// <summary>This method enables or disables GPU-Based Validation (GBV) before creating a device with the debug layer enabled.</summary>
	/// <param name="enable">
	/// <para>Type: <b>BOOL</b> TRUE to enable GPU-Based Validation, otherwise FALSE.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug1-setenablegpubasedvalidation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>GPU-Based Validation can only be enabled/disabled prior to creating a device.  By default, GPU-Based Validation is disabled.  To disable GPU-Based Validation after initially enabling it the device must be fully released and recreated. For more information, see <a href="https://docs.microsoft.com/windows/desktop/direct3d12/using-d3d12-debug-layer-gpu-based-validation">Using D3D12 Debug Layer GPU-Based Validation</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug1-setenablegpubasedvalidation#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetEnableGPUBasedValidation( bool enable ) ;

	/// <summary>Enables or disables dependent command queue synchronization when using a D3D12 device with the debug layer enabled.</summary>
	/// <param name="enable">
	/// <para>Type: <b>BOOL</b> TRUE to enable Dependent Command Queue Synchronization, otherwise FALSE.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug1-setenablesynchronizedcommandqueuevalidation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>Dependent Command Queue Synchronization is a D3D12 Debug Layer feature that gives the debug layer the ability to track resource states more accurately when enabled.  Dependent Command Queue Synchronization is enabled by default. When Dependent Command Queue Synchronization is enabled, the debug layer holds back actual submission of GPU work until all outstanding fence <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-wait">Wait</a> conditions are met.  This gives the debug layer the ability to make reasonable assumptions about GPU state (such as resource states) on the CPU-timeline when multiple command queues are potentially doing concurrent work. With Dependent Command Queue Synchronization disabled, all resource states tracked by the debug layer are cleared each time <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-signal">ID3D12CommandQueue::Signal</a> is called.  This results in significantly less useful resource state validation. Disabling Dependent Command Queue Synchronization may reduce some debug layer performance overhead when using multiple command queues.  However, it is suggested to leave it enabled unless this overhead is problematic.  Note that applications that use only a single command queue will see no performance changes with Dependent Command Queue Synchronization disabled.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug1-setenablesynchronizedcommandqueuevalidation#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetEnableSynchronizedCommandQueueValidation( bool enable ) ;
	
	static IInstantiable IInstantiable.Instantiate( ) => new Debugging.Debug1( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr ptr ) => new Debugging.Debug1( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< T >( T obj ) => new Debugging.Debug1( (ID3D12Debug1)obj! ) ;
} ;


[ProxyFor( typeof( ID3D12Debug2 ) ),
 NativeLibrary( "d3d12", "ID3D12Debug2",
				"d3d12sdklayers.h", "DEBUG_LAYER" )]
public interface IDebug2: IInstantiable, IComIID, IDisposable, IAsyncDisposable {
	
	/// <summary>This method configures the level of GPU-based validation that the debug device is to perform at runtime. (ID3D12Debug2.SetGPUBasedValidationFlags)</summary>
	/// <param name="Flags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/ne-d3d12sdklayers-d3d12_gpu_based_validation_flags">D3D12_GPU_BASED_VALIDATION_FLAGS</a></b> Specifies the level of GPU-based validation to perform at runtime.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug2-setgpubasedvalidationflags#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// This method overrides the default behavior of GPU-based validation so it must be called before creating the Direct3D 12 device.
	/// These settings can't be changed or cancelled after the device is created. If you want to change the behavior of GPU-based validation
	/// at a later time, the device must be destroyed and recreated with different parameters.
	/// </remarks>
	void SetGPUBasedValidationFlags( GPUBasedValidationFlags Flags ) ;
	
	
	static IInstantiable IInstantiable.Instantiate( ) => new Debugging.Debug2( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr ptr ) => new Debugging.Debug2( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< T >( T obj ) => new Debugging.Debug2( (ID3D12Debug2)obj! ) ;
	
	static Type ComType => typeof( ID3D12Debug2 ) ;
	public static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12Debug2 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;


[ProxyFor( typeof( ID3D12Debug3 ) ),
 NativeLibrary( "d3d12", "ID3D12Debug3",
				"d3d12sdklayers.h", "DEBUG_LAYER" )]
public interface IDebug3: IDebug {
	
	/// <summary>This method enables or disables GPU-based validation (GBV) before creating a device with the debug layer enabled.</summary>
	/// <param name="enable">
	/// <para>Type: <b>BOOL</b> TRUE to enable GPU-based validation, otherwise FALSE.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug3-setenablegpubasedvalidation#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>GPU-based validation can be enabled/disabled only prior to creating a device. By default, GPU-based validation is disabled.
	/// To disable GPU-based validation after initially enabling it, the device must be fully released and recreated. For more information, see
	/// <a href="https://docs.microsoft.com/windows/win32/direct3d12/using-d3d12-debug-layer-gpu-based-validation">Using D3D12 Debug Layer GPU-based validation</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug3-setenablegpubasedvalidation#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetEnableGPUBasedValidation( bool enable ) ;

	/// <summary>Enables or disables dependent command queue synchronization when using a Direct3D 12 device with the debug layer enabled.</summary>
	/// <param name="enable">
	/// <para>Type: <b>BOOL</b> TRUE to enable Dependent Command Queue Synchronization, otherwise FALSE.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug3-setenablesynchronizedcommandqueuevalidation#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <remarks>
	/// <para>Dependent Command Queue Synchronization is a D3D12 Debug Layer feature that gives the debug layer the ability to track resource states more accurately when enabled.
	/// Dependent Command Queue Synchronization is enabled by default. When Dependent Command Queue Synchronization is enabled, the debug layer holds back actual submission of
	/// GPU work until all outstanding fence <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-wait">Wait</a> conditions are met. This
	/// gives the debug layer the ability to make reasonable assumptions about GPU state (such as resource states) on the CPU-timeline when multiple command queues are potentially
	/// doing concurrent work. With Dependent Command Queue Synchronization disabled, all resource states tracked by the debug layer are cleared each time
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12commandqueue-signal">ID3D12CommandQueue::Signal</a> is called. This results in significantly
	/// less useful resource state validation. Disabling Dependent Command Queue Synchronization may reduce some debug layer performance overhead when using multiple command queues.
	/// However, it is suggested to leave it enabled unless this overhead is problematic. Note that applications that use only a single command queue will see no performance changes
	/// with Dependent Command Queue Synchronization disabled.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug3-setenablesynchronizedcommandqueuevalidation#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetEnableSynchronizedCommandQueueValidation( bool enable ) ;

	/// <summary>This method configures the level of GPU-based validation that the debug device is to perform at runtime. (ID3D12Debug3.SetGPUBasedValidationFlags)</summary>
	/// <param name="flags">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12sdklayers/ne-d3d12sdklayers-d3d12_gpu_based_validation_flags">D3D12_GPU_BASED_VALIDATION_FLAGS</a></b>
	/// Specifies the level of GPU-based validation to perform at runtime.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug3-setgpubasedvalidationflags#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// This method overrides the default behavior of GPU-based validation so it must be called before creating the D3D12 Device.
	/// These settings can't be changed or cancelled after the device is created.
	/// If you want to change the behavior of GPU-based validation at a later time, the device must be destroyed and recreated with different parameters.
	/// </remarks>
	void SetGPUBasedValidationFlags( GPUBasedValidationFlags flags ) ;
	
	
	static IInstantiable IInstantiable.Instantiate( ) => new Debugging.Debug3( ) ; 
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new Debugging.Debug3( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< T >( T obj ) => new Debugging.Debug3( (ID3D12Debug3)obj! ) ;
	
	new static Type ComType => typeof( ID3D12Debug3 ) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12Debug3 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;


[ProxyFor( typeof( ID3D12Debug4 ) ),
 NativeLibrary( "d3d12", "ID3D12Debug4",
				"d3d12sdklayers.h", "DEBUG_LAYER" )]
public interface IDebug4: IDebug3 {

	/// <summary>Disables the debug layer.</summary>
	HResult DisableDebugLayer( )  ;
	
	static IInstantiable IInstantiable.Instantiate( ) => new Debugging.Debug4( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr ptr ) => new Debugging.Debug4( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< T >( T obj ) => new Debugging.Debug4( (ID3D12Debug4)obj! ) ;
	
	new static Type ComType => typeof( ID3D12Debug4 ) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12Debug ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;


[ProxyFor( typeof( ID3D12Debug5 ) ),
 NativeLibrary( "d3d12", "ID3D12Debug5",
				"d3d12sdklayers.h", "DEBUG_LAYER" )]
public interface IDebug5: IDebug4 {
	
	/// <summary>Configures the auto-naming of objects.</summary>
	/// <param name="enable">
	/// <para>Type: **[BOOL](/windows/desktop/winprog/windows-data-types)** `true` to enable auto-naming; `false` to disable auto-naming.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug5-setenableautoname#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>By default, objects are not named unless you use [ID3D12Object::SetName](/windows/win32/api/d3d12/nf-d3d12-id3d12object-setname) or [ID3D12Object::SetPrivateData](/windows/win32/api/d3d12/nf-d3d12-id3d12object-setprivatedata) to assign a name. It's a best practice to name all of your Direct3D 12 objects; at least in debug builds. Failing that, you might find it convenient to allow automatic name assignment in order to cover the gaps. Direct3D 12 objects created with auto-name enabled are automatically assigned a name, which is used for debug layer output and for DRED page fault data. So as not to create a dependency on a specific auto-naming format, you can't retrieve the auto-name strings by using ID3D12Object::GetName or [ID3D12Object::GetPrivateData](/windows/win32/api/d3d12/nf-d3d12-id3d12object-getprivatedata). But, to generate a unique name string, Direct3D 12 uses the locally-unique identifier (LUID) assigned to every **ID3D12DeviceChild** object at create time. You can retrieve that LUID by using **ID3D12Object::GetPrivateData** with the **REFGUID** value *WKPDID_D3D12UniqueObjectId*. You might find that useful for your own object-naming schemas. When debugging existing software, you can control auto-naming by using the *D3DConfig* graphics tools utility and the command `d3dconfig.exe device auto-debug-name=forced-on`. Any object given a name using [ID3D12Object::SetName](/windows/win32/api/d3d12/nf-d3d12-id3d12object-setname) or [ID3D12Object::SetPrivateData](/windows/win32/api/d3d12/nf-d3d12-id3d12object-setprivatedata) uses the assigned name instead of the auto-name.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12sdklayers/nf-d3d12sdklayers-id3d12debug5-setenableautoname#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	void SetEnableAutoName( bool enable ) ;
	
	static IInstantiable IInstantiable.Instantiate( ) => new Debugging.Debug5( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr ptr ) => new Debugging.Debug5( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< T >( T obj ) => new Debugging.Debug5( (ID3D12Debug5)obj! ) ;
	
	new static Type ComType => typeof( ID3D12Debug5 ) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12Debug5 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;


[ProxyFor( typeof( ID3D12Debug6 ) ),
 NativeLibrary( "d3d12", "ID3D12Debug3",
				"d3d12sdklayers.h", "DEBUG_LAYER" )]
public interface IDebug6: IDebug5 {
	
	/// <summary>TBD</summary>
	/// <param name="enable"></param>
	/// <remarks>Requires the DirectX 12 Agility SDK 1.7 or later.</remarks>
	void SetForceLegacyBarrierValidation( bool enable ) ;
	
	
	static IInstantiable IInstantiable.Instantiate( ) => new Debugging.Debug6( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr ptr ) => new Debugging.Debug6( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< T >( T obj ) => new Debugging.Debug6( (ID3D12Debug6)obj! ) ;
	
	public new static Type ComType => typeof(ID3D12Debug6) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12Debug6 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;
