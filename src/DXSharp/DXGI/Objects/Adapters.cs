// Implements an idiomatic C# version of IDXGIAdapter interface:
// https://docs.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiadapter
#region Using Directives
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
public class Adapter: Object, IAdapter, IConstructable< Adapter >, 
					  IConstructable< Adapter, IDXGIAdapter > {
	public IDXGIAdapter? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIAdapter >? ComPointer { get ; protected set ; }
	
	//! Constructors:
	internal Adapter( ) { }
	public Adapter( nint nativePtr ): base(nativePtr) { }
	public Adapter( IDXGIAdapter dxObject ): base(dxObject) { }
	
	//! IAdapter (base) Interface:
	public void GetDesc( out AdapterDescription pDesc ) {
		_throwIfDestroyed( ) ;
		unsafe {
			DXGI_ADAPTER_DESC desc = default ;
			COMObject!.GetDesc( &desc ) ;
			pDesc = desc ;
		}
	}
	
	
	/// <summary>
	/// Enumerate adapter (video card) outputs.
	/// </summary>
	/// <param name="Output">The index of the output.</param>
	/// <param name="ppOutput"><see cref="TOutput"/> interface at the position specified by the Output parameter.</param>
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
	public HResult EnumOutputs< TOutput >( uint Output, out TOutput? ppOutput )
													where TOutput: class, IOutput, new( ) {
		ppOutput = default ; HResult hr = default ;
		unsafe {
			hr = COMObject!.EnumOutputs( Output, out IDXGIOutput? pOutput ) ;
			if ( hr.Failed ) { ppOutput = default ; return hr ; }
			
			( ppOutput = new() )
				.SetComPointer( new ComPtr< IDXGIOutput >(pOutput) ) ;
			//! Previously: (TOutput)( (IOutput)new Output( pOutput ) ) ;
		}
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

	
	internal static ConstructWrapper< IObject, IDXGIObject >? 
		ConstructFunction => (a) => _getNew( (IDXGIAdapter)a ) ;
	static Adapter _getNew( IDXGIAdapter unknown ) => new ( unknown ) ;
	public static Adapter ConstructEmpty( ) => new( ) ;
	public static Adapter ConstructWith( IDXGIAdapter arg1 ) {
		Adapter adapter = _getNew( arg1 ) ;
		adapter.ComPointer = new ComPtr< IDXGIAdapter >(arg1) ;
		return adapter ;
	}
} ;

public class Adapter1: Adapter, IAdapter1 {
	public new IDXGIAdapter1? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIAdapter1 >? ComPointer { get ; protected set ; }
	
	internal Adapter1( ) { }
	public Adapter1( nint nativePtr ): base(nativePtr) { }
	public Adapter1( IDXGIAdapter1 dxObject ): base(dxObject) { }
	
	public void GetDesc1( out AdapterDescription1 pDesc ) { 
		_throwIfDestroyed( ) ;
		unsafe {
			DXGI_ADAPTER_DESC1 desc = default ;
			COMObject!.GetDesc1( &desc ) ;
			pDesc = desc ;
		}
	}
	
} ;