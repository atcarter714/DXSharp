// Implements an idiomatic C# version of IDXGIAdapter interface:
// https://docs.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiadapter
#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;

#endregion
namespace DXSharp.DXGI ;


/// <summary>
/// Manages the native <see cref="IDXGIAdapter"/> interface. The native COM interface represents
/// a display subsystem (including one or more GPUs, DACs and video memory).
/// </summary>
/// <remarks>
/// Go to <a href="https://learn.microsoft.com">Microsoft Learn</a> to learn more about
/// the native <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiadapter">IDXGIAdapter</a> interface.
/// </remarks>
[Wrapper(typeof(IDXGIAdapter))]
internal class Adapter: Object,
						IAdapter,
						IComObjectRef< IDXGIAdapter >,
						IUnknownWrapper< IDXGIAdapter > {
	// ---------------------------------------------------------------------------------

	ComPtr< IDXGIAdapter >? _comPtr ;
	public new virtual ComPtr< IDXGIAdapter >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIAdapter >( )! ;
	
	public override IDXGIAdapter? ComObject => ComPointer?.Interface ;
	// ---------------------------------------------------------------------------------
	
	protected AdapterDescription _descCached ;
	public virtual AdapterDescription Description {
		get {
			if( _descCached.DeviceId is 0 )
				GetDesc( out _descCached ) ;
			return _descCached ;
		}
	}
	
	public GPUVendor Vendor => 
		Description.VendorId switch {
			0x10DE => GPUVendor.Nvidia,
			0x1002 => GPUVendor.AMD,
			0x8086 => GPUVendor.Intel,
			_      => GPUVendor.Unknown,
		} ;
	
	// -------------------------------------------------------------------------------------
	
	internal Adapter( ) {
		_comPtr = ComResources?.GetPointer< IDXGIAdapter >( ) ;
	}
	internal Adapter( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Adapter( IDXGIAdapter adapter ) {
		_comPtr = new( adapter ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Adapter( ComPtr< IDXGIAdapter > ptr ) {
		_comPtr = ptr ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -------------------------------------------------------------------------------------
	
	//! IAdapter (base) Interface:
	public void GetDesc( out AdapterDescription pDesc ) {
		unsafe {
			DXGI_ADAPTER_DESC desc = default ;
			ComObject!.GetDesc( &desc ) ;
			pDesc = desc ;
		}
	}

	
	public HResult EnumOutputs( uint index, out IOutput? ppOutput ) {
		ppOutput = default ;
		HResult hr = ComObject!.EnumOutputs( index, out IDXGIOutput? pOutput ) ;
		ppOutput = pOutput is null ? null : new Output( pOutput ) ;
		return hr ;
	}
	
	
	public void CheckInterfaceSupport< TInterface >( out long pUMDVersion )
		where TInterface: IUnknown => CheckInterfaceSupport( typeof(TInterface).GUID, out pUMDVersion ) ;
	
	
	public void CheckInterfaceSupport( in Guid InterfaceName, out long pUMDVersion ) {
		pUMDVersion = 0 ;
		unsafe {
			Guid name = InterfaceName ;
			ComObject!.CheckInterfaceSupport( &name, out pUMDVersion ) ;
		}
	}

	// -------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof( IDXGIAdapter ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIAdapter).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	
	// ======================================================================================
} ;



[Wrapper(typeof(IDXGIAdapter1))]
internal class Adapter1: Adapter, 
						 IAdapter1,
						 IComObjectRef< IDXGIAdapter1 >,
						 IUnknownWrapper< IDXGIAdapter1 > {
	// ---------------------------------------------------------------------------------
	public override IDXGIAdapter1? ComObject => ComPointer?.Interface ;
	
	public new ComPtr< IDXGIAdapter1 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< IDXGIAdapter1 >( )! ;
	ComPtr< IDXGIAdapter1 >? _comPtr ;
	
	// ---------------------------------------------------------------------------------
	
	protected AdapterDescription1 _desc1Cached ;
	public AdapterDescription1 Description1 {
		get {
			if( _desc1Cached.DeviceId is 0 ) {
				GetDesc1( out _desc1Cached ) ;
				_descCached = new AdapterDescription {
					VendorId              = _desc1Cached.VendorId,
					DeviceId              = _desc1Cached.DeviceId,
					SubSysId              = _desc1Cached.SubSysId,
					Revision              = _desc1Cached.Revision,
					DedicatedVideoMemory  = _desc1Cached.DedicatedVideoMemory,
					DedicatedSystemMemory = _desc1Cached.DedicatedSystemMemory,
					SharedSystemMemory    = _desc1Cached.SharedSystemMemory,
					AdapterLuid           = _desc1Cached.AdapterLuid,
					Description           = _desc1Cached.Description,
				} ;
			}
			return _desc1Cached ;
		}
	}
	
	// -------------------------------------------------------------------------------------
	
	internal Adapter1( ) {
		_comPtr = ComResources?.GetPointer< IDXGIAdapter1 >( ) ;
	}
	internal Adapter1( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Adapter1( IDXGIAdapter1 adapter ) {
		_comPtr = new( adapter ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Adapter1( ComPtr< IDXGIAdapter1 > ptr ) {
		_comPtr = ptr ;
		_initOrAdd( _comPtr ) ;
	}

	// -------------------------------------------------------------------------------------

	public void GetDesc1( out AdapterDescription1 pDesc ) {
		unsafe {
			DXGI_ADAPTER_DESC1 desc = default ;
			ComObject!.GetDesc1( &desc ) ;
			pDesc = desc ;
		}
	}
	
	// -------------------------------------------------------------------------------------

	public new static Type ComType => typeof( IDXGIAdapter1 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIAdapter1).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	
	// ======================================================================================
} ;



// --------------------------------------------------------------------------------------------
// DXGI.Adapter2 Wrapper :: IDXGIAdapter2
// --------------------------------------------------------------------------------------------

/// <summary>
/// A wrapper for the native <see cref="IDXGIAdapter2"/> interface,
/// which represents a display subsystem, which includes one or more
/// GPUs, DACs, and video memory.
/// </summary>
/// <remarks>
/// A display subsystem is often referred to as a video card; however, on some computers, the display subsystem is part of the motherboard.
/// <para>To enumerate the display subsystems, use IDXGIFactory1::EnumAdapters1.</para>
/// <para>To get an interface to the adapter for a particular device, use
/// <see cref="Device.GetAdapter"/>.</para>
/// <para>To create a software adapter, use IDXGIFactory::CreateSoftwareAdapter.</para><para/>
/// Go to Microsoft documentation for
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nn-dxgi1_2-idxgiadapter2">IDXGIAdapter2</a>
/// for further details.
/// <seealso cref="IDXGIFactory1.EnumAdapters1"/>
/// <seealso cref="IDXGIDevice.GetAdapter"/>
/// <seealso cref="IDXGIFactory.CreateSoftwareAdapter"/>
/// </remarks>
[SupportedOSPlatform("windows8.0"),
 Wrapper(typeof(IDXGIAdapter2)),
 NativeLibrary( "dxgi.dll", "IDXGIAdapter2",
				"dxgi1_2.h", "Adapter2" )]
internal class Adapter2: Adapter1, IAdapter2,
					   IComObjectRef< IDXGIAdapter2 >,
					   IUnknownWrapper< IDXGIAdapter2 > {
	ComPtr< IDXGIAdapter2 >? _comPtr ;
	public new ComPtr< IDXGIAdapter2 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< IDXGIAdapter2 >( )! ;
	public override IDXGIAdapter2? ComObject => ComPointer?.Interface ;
	
	protected AdapterDescription2 _desc2Cached ;
	public AdapterDescription2 Description2 {
		get {
			if( _desc2Cached.DeviceId is 0 ) {
				GetDesc2( out _desc2Cached ) ;
				_desc1Cached = _desc2Cached.Description1 ;
				_descCached = new AdapterDescription {
					VendorId              = _desc2Cached.VendorId,
					DeviceId              = _desc2Cached.DeviceId,
					SubSysId              = _desc2Cached.SubSysId,
					Revision              = _desc2Cached.Revision,
					DedicatedVideoMemory  = _desc2Cached.DedicatedVideoMemory,
					DedicatedSystemMemory = _desc2Cached.DedicatedSystemMemory,
					SharedSystemMemory    = _desc2Cached.SharedSystemMemory,
					AdapterLuid           = _desc2Cached.AdapterLuid,
					Description           = _desc2Cached.Description,
				} ;
			}
			return _desc2Cached ;
		}
	}
	
	
	// --------------------------------------------------------------
	// Internal Constructors:
	// --------------------------------------------------------------
	
	internal Adapter2( ) {
		_comPtr = ComResources?.GetPointer< IDXGIAdapter2 >( ) ;
	}
	internal Adapter2( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Adapter2( IDXGIAdapter2 adapter ) {
		_comPtr = new( adapter ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Adapter2( ComPtr< IDXGIAdapter2 > ptr ) {
		_comPtr = ptr ;
		_initOrAdd( _comPtr ) ;
	}
	
	// --------------------------------------------------------------
	// Interface Instance Methods:
	// --------------------------------------------------------------
	
	public AdapterDescription2 GetDesc2( ) {
		unsafe {
			DXGI_ADAPTER_DESC2 desc = default ;
			ComObject!.GetDesc2( &desc ) ;
			return desc ;
		}
	}
	
	public void GetDesc2( out AdapterDescription2 pDesc ) { 
		unsafe {
			DXGI_ADAPTER_DESC2 desc = default ;
			ComObject!.GetDesc2( &desc ) ;
			pDesc = desc ;
		}
	}
	
	
	// --------------------------------------------------------------
	// Static Members:
	// --------------------------------------------------------------
	
	public new static Type ComType => typeof( IDXGIAdapter2 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIAdapter2).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}

	// ==============================================================
} ;


// --------------------------------------------------------------------------------------------
// DXGI.Adapter3 Wrapper :: IDXGIAdapter3
// --------------------------------------------------------------------------------------------

[Wrapper( typeof( IDXGIAdapter3 ) )]
internal class Adapter3: Adapter2,
						 IAdapter3,
						 IComObjectRef< IDXGIAdapter3 >,
						 IUnknownWrapper< IDXGIAdapter3 > {
	// --------------------------------------------------------------

	ComPtr< IDXGIAdapter3 >? _comPtr ;
	public new ComPtr< IDXGIAdapter3 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIAdapter3 >( )! ;

	public override IDXGIAdapter3? ComObject => ComPointer?.Interface ;
	
	// --------------------------------------------------------------
	
	internal Adapter3( ) {
		_comPtr = ComResources?.GetPointer< IDXGIAdapter3 >( ) ;
	}
	internal Adapter3( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Adapter3( IDXGIAdapter3 adapter ) {
		_comPtr = new( adapter ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Adapter3( ComPtr< IDXGIAdapter3 > ptr ) {
		_comPtr = ptr ;
		_initOrAdd( _comPtr ) ;
	}

	// --------------------------------------------------------------

	public unsafe void QueryVideoMemoryInfo( uint NodeIndex, 
											 MemorySegmentGroup memorySegmentGroup,
											 in QueryVideoMemoryInfo pVideoMemoryInfo ) {
		fixed ( QueryVideoMemoryInfo* p = &pVideoMemoryInfo ) {
			ComObject!.QueryVideoMemoryInfo( NodeIndex, 
											 (DXGI_MEMORY_SEGMENT_GROUP)memorySegmentGroup, 
											 (DXGI_QUERY_VIDEO_MEMORY_INFO *)p ) ;
		}
	}

	public void SetVideoMemoryReservation( uint NodeIndex, 
										   MemorySegmentGroup memorySegmentGroup, 
										   ulong Reservation ) {
		ComObject!.SetVideoMemoryReservation( NodeIndex,
											  (DXGI_MEMORY_SEGMENT_GROUP)memorySegmentGroup,
											  Reservation ) ;
	}

	public void UnregisterHardwareContentProtectionTeardownStatus( uint dwCookie ) => 
		ComObject!.UnregisterHardwareContentProtectionTeardownStatus( dwCookie ) ;

	public void UnregisterVideoMemoryBudgetChangeNotification( uint dwCookie ) => 
		ComObject!.UnregisterVideoMemoryBudgetChangeNotification( dwCookie ) ;

	public void RegisterHardwareContentProtectionTeardownStatusEvent( Win32Handle hEvent, out uint pdwCookie ) => 
		ComObject!.RegisterHardwareContentProtectionTeardownStatusEvent( hEvent, out pdwCookie ) ;

	public void RegisterVideoMemoryBudgetChangeNotificationEvent( Win32Handle hEvent, out uint pdwCookie ) => 
		ComObject!.RegisterVideoMemoryBudgetChangeNotificationEvent( hEvent, out pdwCookie ) ;
	
	// --------------------------------------------------------------

	public new static Type ComType => typeof( IDXGIAdapter3 ) ;

	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIAdapter3 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	
	// ==============================================================
} ;


// --------------------------------------------------------------------------------------------
// DXGI.Adapter4 Wrapper :: IDXGIAdapter4
// --------------------------------------------------------------------------------------------

[Wrapper( typeof( IDXGIAdapter4 ) )]
internal class Adapter4: Adapter3, 
						 IAdapter4,
						 IComObjectRef< IDXGIAdapter4 >,
						 IUnknownWrapper< IDXGIAdapter4 > {
	ComPtr< IDXGIAdapter4 >? _comPtr ;
	public new ComPtr< IDXGIAdapter4 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< IDXGIAdapter4 >( )! ;
	public override IDXGIAdapter4? ComObject => ComPointer?.Interface ;

	protected AdapterDescription3 _desc3Cached ;
	public AdapterDescription3 Description3 {
		get {
			if ( _desc3Cached.DeviceId is 0 ) {
				GetDesc3( out _desc3Cached ) ;
				_desc2Cached = new AdapterDescription2 {
					VendorId              = _desc3Cached.VendorId,
					DeviceId              = _desc3Cached.DeviceId,
					SubSysId              = _desc3Cached.SubSysId,
					Revision              = _desc3Cached.Revision,
					DedicatedVideoMemory  = _desc3Cached.DedicatedVideoMemory,
					DedicatedSystemMemory = _desc3Cached.DedicatedSystemMemory,
					SharedSystemMemory    = _desc3Cached.SharedSystemMemory,
					AdapterLuid           = _desc3Cached.AdapterLuid,
					Description           = _desc3Cached.Description,
					Flags                 = (AdapterFlag)_desc3Cached.Flags,
					ComputePreemptionGranularity = _desc3Cached.ComputePreemptionGranularity,
					GraphicsPreemptionGranularity = _desc3Cached.GraphicsPreemptionGranularity,
				} ;
				_desc1Cached = _desc2Cached.Description1 ;
				_descCached = new AdapterDescription {
					VendorId              = _desc3Cached.VendorId,
					DeviceId              = _desc3Cached.DeviceId,
					SubSysId              = _desc3Cached.SubSysId,
					Revision              = _desc3Cached.Revision,
					DedicatedVideoMemory  = _desc3Cached.DedicatedVideoMemory,
					DedicatedSystemMemory = _desc3Cached.DedicatedSystemMemory,
					SharedSystemMemory    = _desc3Cached.SharedSystemMemory,
					AdapterLuid           = _desc3Cached.AdapterLuid,
					Description           = _desc3Cached.Description,
				} ;
			}

			return _desc3Cached ;
		}
	}

	public void GetDesc3( out AdapterDescription3 pDesc ) {
		unsafe {
			DXGI_ADAPTER_DESC3 desc = default ;
			ComObject!.GetDesc3( &desc ) ;
			pDesc = desc ;
		}
	}
	
	internal Adapter4( ) {
		_comPtr = ComResources?.GetPointer< IDXGIAdapter4 >( ) ;
	}
	internal Adapter4( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Adapter4( IDXGIAdapter4 adapter ) {
		_comPtr = new( adapter ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Adapter4( ComPtr< IDXGIAdapter4 > ptr ) {
		_comPtr = ptr ;
		_initOrAdd( _comPtr ) ;
	}
	
	public new static Type ComType => typeof( IDXGIAdapter4 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIAdapter4).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;

