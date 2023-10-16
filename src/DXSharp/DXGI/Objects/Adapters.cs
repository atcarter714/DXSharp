// Implements an idiomatic C# version of IDXGIAdapter interface:
// https://docs.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiadapter
#region Using Directives
using System.Runtime.Versioning ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
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
public class Adapter: Object, IAdapter {
	// -------------------------------------------------------------------------------------
	//public new static Type ComType => typeof(IDXGIAdapter) ;
	///public new static Guid InterfaceGUID => typeof(IDXGIAdapter).GUID ;
	static IDXCOMObject IInstantiable.Instantiate( ) => new Adapter( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint ptr ) => new Adapter( ptr ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom obj ) => 
		new Adapter( ( obj as IDXGIAdapter )! ) ;
	
	public new ComPtr< IDXGIAdapter >? ComPointer { get ; protected set ; }
	public new IDXGIAdapter? COMObject => ComPointer?.Interface ;
	// -------------------------------------------------------------------------------------
	
	
	//! Constructors:
	internal Adapter( ) { }
	internal Adapter( ComPtr< IDXGIAdapter > comPtr ) => this.ComPointer = comPtr ;
	internal Adapter( nint address ) => ComPointer = new( address ) ;
	internal Adapter( IDXGIAdapter dxObject ) => ComPointer = new( dxObject ) ;

	
	//! IAdapter (base) Interface:
	public void GetDesc( out AdapterDescription pDesc ) {
		unsafe {
			DXGI_ADAPTER_DESC desc = default ;
			COMObject!.GetDesc( &desc ) ;
			pDesc = desc ;
		}
	}


	/// <summary>
	/// Enumerate adapter (video card) outputs.
	/// </summary>
	/// <param name="index">The index of the output.</param>
	/// <param name="ppOutput"><typeparam name="TOutput"/> interface at the position specified by the Output parameter.</param>
	/// <typeparam name="TOutput">The type of <see cref="IOutput"/> elements to enumerate through.</typeparam>
	/// <returns>
	/// A code that indicates success or failure (see DXGI_ERROR ).
	/// DXGI_ERROR_NOT_FOUND is returned if the index is greater than the number of outputs.
	/// If the adapter came from a device created using D3D_DRIVER_TYPE_WARP, then the
	/// adapter has no outputs, so DXGI_ERROR_NOT_FOUND is returned.
	/// </returns>
	/// <remarks>
	/// To know when you've reached the end of the collection of outputs, simply check for either
	/// a null value or HResult code DXGI_ERROR_NOT_FOUND.<para/>
	/// For additional technical details on this, see the
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR documentation</a>.
	/// </remarks>
	public HResult EnumOutputs< TOutput >( uint index, out TOutput? ppOutput )
													where TOutput: class, IOutput, IInstantiable {
		ppOutput = default ; HResult hr = default ;
		hr = COMObject!.EnumOutputs( index, out IDXGIOutput? pOutput ) ;
		if ( hr.Failed ) { ppOutput = default ; return hr ; }
		
		( ppOutput = (TOutput)TOutput.Instantiate(pOutput) )
			.SetComPointer( new ComPtr< IDXGIOutput >(pOutput) ) ;
	
		return hr ;
	}
	
	
	public void CheckInterfaceSupport< TInterface >( out long pUMDVersion )
		where TInterface: IUnknown => CheckInterfaceSupport( typeof(TInterface).GUID, out pUMDVersion ) ;
	
	
	public void CheckInterfaceSupport( in Guid InterfaceName, out long pUMDVersion ) {
		pUMDVersion = 0 ;
		unsafe {
			Guid name = InterfaceName ;
			COMObject!.CheckInterfaceSupport( &name, out pUMDVersion ) ;
		}
	}
	
} ;



public class Adapter1: Adapter, IAdapter1 {
	public static IDXCOMObject Instantiate( ) => new Adapter1( ) ;
	public static IDXCOMObject Instantiate( nint ptr ) => new Adapter1( ptr ) ;
	public static IDXCOMObject Instantiate< ICom >( ICom obj ) where ICom: IUnknown? => 
		new Adapter1( ( obj as IDXGIAdapter1 )! ) ;
	
	
	public new IDXGIAdapter1? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIAdapter1 >? ComPointer { get ; protected set ; }
	
	
	AdapterDescription1 _desc1Cached = default ;
	public AdapterDescription1 Description1 {
		get {
			if( _desc1Cached.DeviceId is 0 )
				GetDesc1( out _desc1Cached ) ;
			return _desc1Cached ;
		}
	}
	
	public GPUVendor Vendor => 
		Description1.VendorId switch {
			0x10DE => GPUVendor.Nvidia,
			0x1002 => GPUVendor.AMD,
			0x8086 => GPUVendor.Intel,
			_      => GPUVendor.Unknown,
		} ;
	
	
	internal Adapter1( ) { }
	internal Adapter1( nint address ) => ComPointer = new( address ) ;
	internal Adapter1( IDXGIAdapter1 dxObject ) => ComPointer = new( dxObject ) ;
	
	
	public void GetDesc1( out AdapterDescription1 pDesc ) {
		unsafe {
			DXGI_ADAPTER_DESC1 desc = default ;
			COMObject!.GetDesc1( &desc ) ;
			pDesc = desc ;
		}
	}
	
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
/// <see cref="Device.GetAdapter{T}"/>.</para>
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
public class Adapter2: Adapter1, IAdapter2,
					   IInstantiable< Adapter2 > {
	
	public new IDXGIAdapter2? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIAdapter2 >? ComPointer { get ; protected set ; }
	
	AdapterDescription2 _desc2Cached = default ;
	public AdapterDescription2 Description2 {
		get {
			if( _desc2Cached.DeviceId is 0 )
				GetDesc2( out _desc2Cached ) ;
			return _desc2Cached ;
		}
	}
	
	public GPUVendor Vendor => Description2.VendorId switch {
		0x10DE => GPUVendor.Nvidia,
		0x1002 => GPUVendor.AMD,
		0x8086 => GPUVendor.Intel,
		_ => GPUVendor.Unknown,
	} ;
	
	// --------------------------------------------------------------
	// Internal Constructors:
	// --------------------------------------------------------------
	
	internal Adapter2( ) { }
	internal Adapter2( nint address ) => ComPointer = new( address ) ;
	internal Adapter2( IDXGIAdapter2 dxObject ) => ComPointer = new( dxObject ) ;
	internal Adapter2( ComPtr< IDXGIAdapter2 > comPtr ) => this.ComPointer = comPtr ;
	
	
	// --------------------------------------------------------------
	// Interface Instance Methods:
	// --------------------------------------------------------------
	
	public AdapterDescription2 GetDesc2( ) {
		
		unsafe {
			DXGI_ADAPTER_DESC2 desc = default ;
			COMObject!.GetDesc2( &desc ) ;
			return desc ;
		}
	}
	
	public void GetDesc2( out AdapterDescription2 pDesc ) { 
		
		unsafe {
			DXGI_ADAPTER_DESC2 desc = default ;
			COMObject!.GetDesc2( &desc ) ;
			pDesc = desc ;
		}
	}
	
	
	// --------------------------------------------------------------
	// Static Methods:
	// --------------------------------------------------------------
	
	#region IInstantiable Implementation:
	static Adapter2 IInstantiable< Adapter2 >.Instantiate( nint ptr ) => new( ptr ) ;
	public static IDXCOMObject Instantiate( ) => new Adapter2( ) ;
	public static IDXCOMObject Instantiate( nint ptr ) => new Adapter2( ptr ) ;
	public static IDXCOMObject Instantiate< ICom >( ICom? obj ) where ICom: IUnknown? => 
		new Adapter2( ( obj as IDXGIAdapter2 )! ) ;
	#endregion
	
	// ==============================================================
} ;