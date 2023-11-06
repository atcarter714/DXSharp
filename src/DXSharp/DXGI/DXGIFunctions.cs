#region Using Directives
using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using System.Runtime.InteropServices ;
using System.Runtime.CompilerServices ;
using DXSharp.Windows ;
using static DXSharp.DXSharpUtils ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>
/// Valid values include the DXGI_CREATE_FACTORY_DEBUG (0x01) flag, and zero.
/// </summary>
/// <remarks>
/// <b><h3>Note:</h3></b> 
/// This flag will be set by the D3D runtime if:
/// The system creates an implicit factory during device creation.
/// The D3D11_CREATE_DEVICE_DEBUG flag is specified during device creation, 
/// for example using D3D11CreateDevice (or the swapchain method, or the 
/// Direct3D 10 equivalents).
/// </remarks>
public enum FactoryCreateFlags: uint {
	/// <summary>No flags</summary>
	None  = 0x0000,
	/// <summary>Enable debug layer</summary>
	Debug = 0x0001,
} ;



/// <summary>
/// Defines the DXGI-related functions of the Windows SDK
/// </summary>
/// <remarks>
/// See the documentation for the 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/d3d10-graphics-reference-dxgi-functions">DXGI functions</a> 
/// for a complete list with additional information
/// </remarks>
public static partial class DXGIFunctions {

	//! Factory creation functions:
	static readonly Dictionary< Guid, Func< IDXGIFactory, IInstantiable > > _factoryCreators = new( ) {
		{ IFactory.IID, (f) => new Factory(f) },
		{ IFactory1.IID, (f) => new Factory1( (f as IDXGIFactory1)! ) },
		{ IFactory2.IID, (f) => new Factory2( (f as IDXGIFactory2)! ) },
		{ IFactory3.IID, (f) => new Factory3( (f as IDXGIFactory3)! ) },
		{ IFactory4.IID, (f) => new Factory4( (f as IDXGIFactory4)! ) },
		{ IFactory5.IID, (f) => new Factory5( (f as IDXGIFactory5)! ) },
		{ IFactory6.IID, (f) => new Factory6( (f as IDXGIFactory6)! ) },
		{ IFactory7.IID, (f) => new Factory7( (f as IDXGIFactory7)! ) },
	} ;
	
	
	internal static unsafe TFactory? CreateDXGIFactory< TFactory >( Guid riid, out HRESULT hr ) 
																	where TFactory: IDXGIFactory {
		hr = PInvoke.CreateDXGIFactory( &riid, out object? factoryObj ) ;
		return hr.Succeeded ? (TFactory)factoryObj : default ;
	}

	internal static unsafe TFactory? CreateDXGIFactory1< TFactory >( Guid riid, out HRESULT hr ) 
																	 where TFactory: IDXGIFactory1 {
		hr = PInvoke.CreateDXGIFactory1( &riid, out object? factoryObj ) ;
		return hr.Succeeded ? (TFactory)factoryObj : default ;
	}
	
	internal static unsafe TFactory? CreateDXGIFactory2< TFactory >( FactoryCreateFlags Flags, Guid riid, out HRESULT hr ) 
																	 where TFactory: IDXGIFactory2 {
		hr = PInvoke.CreateDXGIFactory2( (uint)Flags, &riid, out object? factoryObj ) ;
		return hr.Succeeded ? (TFactory)factoryObj : default ;
	}
	
	
	
	public static HResult CreateDXGIFactory2< T >( FactoryCreateFlags flags, in Guid riid, out T factory ) 
																		where T : IFactory2, IInstantiable {
#if DEBUG || DEBUG_COM || DEV_BUILD
		if ( !_factoryCreators.ContainsKey( riid ) )
			throw new ArgumentException( $"Unrecognized GUID value: " +
										 $"{riid.ToString()}", nameof( riid ) ) ;
#endif
		
		HResult hr = PInvoke.CreateDXGIFactory2( (uint)flags, riid, out var factoryObj ) ;
		hr.SetAsLastErrorForThread( ) ;

		// Convert RCW object to IDXGIFactory:
		var dxgiFactoryBase = factoryObj as IDXGIFactory2
#if DEBUG || DEBUG_COM || DEV_BUILD
							  ?? throw new DirectXComError( $"The object returned from {nameof(CreateDXGIFactory2)} is invalid!" )
#else
			!
#endif
			;
		
		factory = (T)_factoryCreators[ riid ]( dxgiFactoryBase ) ;
		return hr ;
	}
	
	
	
	
	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory< T >( out HRESULT hr ) where T: IDXGIFactory {
		return DXGIFunctions.CreateDXGIFactory< T >( typeof(T).GUID, out hr ) ;
		
		/*unsafe {
			var riid = typeof(T).GUID ;
			hr = PInvoke.CreateDXGIFactory( &riid, out object? factoryObj ) ;
			return hr.Succeeded ? (T)factoryObj : default ;
		}*/
	}

	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	/// <exception cref="COMException">Thrown if the call fails and contains detailed error information</exception>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory< T >( ) where T : IDXGIFactory {
		var factory = DXGIFunctions.CreateDXGIFactory< T >( typeof(T).GUID, out var hr ) ;
		_ = hr.ThrowOnFailure( ) ;
		return factory ;
	}


	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="TFactory">Type of DXGIFactoryX</typeparam>
	/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl(_MAXOPT_)]
	internal static TFactory? CreateDXGIFactory1< TFactory >( out HRESULT hr ) where TFactory : IDXGIFactory1 {
		hr = PInvoke.CreateDXGIFactory1( typeof( TFactory ).GUID, out var factory ) ;
		_ = hr.ThrowOnFailure( ) ;
		return (TFactory)factory ;
	}
	
	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	/// <exception cref="COMException">Thrown if the call fails and contains detailed error information</exception>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory1< T >( ) where T : IDXGIFactory1 {
		var factory = CreateDXGIFactory1< T >( out var hr ) ;
		_ = hr.ThrowOnFailure( ) ;
		return factory;
	}


	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="Flags">Creation flags</param>
	/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory2< T >( FactoryCreateFlags Flags, out HRESULT hr ) 
																							where T : IDXGIFactory2 {
		var factory2 = CreateDXGIFactory2< T >( Flags, typeof(T).GUID, out hr ) ;
		return (T)factory2! ;
	}

	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="Flags">Creation flags</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory2< T >( FactoryCreateFlags Flags ) where T : IDXGIFactory2 {
		var factory = CreateDXGIFactory2< T >( Flags, out var hr ) ;
		_ = hr.ThrowOnFailure( ) ;
		return factory ;
	}
	
} ;

// ---------------------------------------------------------------------------------------------------