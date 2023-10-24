#region Using Directives
using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using System.Runtime.InteropServices ;
using System.Runtime.CompilerServices ;

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
	DEBUG = 0x0001,
} ;


// Native Interop Signatures: -----------------------------------------------------------------------------------------
//HRESULT CreateDXGIFactory( in global::System.Guid riid, out object ppFactory );
//HRESULT CreateDXGIFactory( global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppFactory );
//HRESULT CreateDXGIFactory1( in global::System.Guid riid, out object ppFactory );
//HRESULT CreateDXGIFactory1( global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppFactory );
//HRESULT CreateDXGIFactory2( uint Flags, in global::System.Guid riid, out object ppFactory );
//HRESULT CreateDXGIFactory2( uint Flags, global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppFactory );

//HRESULT DXGIGetDebugInterface1( uint Flags, in global::System.Guid riid, out object pDebug );
//HRESULT DXGIGetDebugInterface1( uint Flags, global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object pDebug );
//HRESULT DXGIDeclareAdapterRemovalSupport();
// ---------------------------------------------------------------------------------------------------------------------



/// <summary>
/// Defines the DXGI-related functions of the Windows SDK
/// </summary>
/// <remarks>
/// See the documentation for the 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/d3d10-graphics-reference-dxgi-functions">DXGI functions</a> 
/// for a complete list with additional information
/// </remarks>
internal static partial class DXGIFunctions {

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
		var factory = DXGIFunctions.CreateDXGIFactory<T>( typeof(T).GUID, out var hr ) ;
		_ = hr.ThrowOnFailure( ) ;
		return factory ;
	}


	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl(_MAXOPT_)]
	internal static T? CreateDXGIFactory1<T>( out HRESULT hr ) where T : IDXGIFactory1 {
		hr = PInvoke.CreateDXGIFactory1( typeof( T ).GUID, out var factory ) ;
		_ = hr.ThrowOnFailure( ) ;
		return (T)factory ;
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
		var factory2 = DXGIFunctions.CreateDXGIFactory2< T >( Flags, typeof(T).GUID, out hr ) ;
		return (T)factory2! ;
	}

	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="Flags">Creation flags</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory2< T >( FactoryCreateFlags Flags ) where T : IDXGIFactory2 {
		var factory = CreateDXGIFactory2<T>( Flags, out var hr ) ;
		_ = hr.ThrowOnFailure( ) ;
		return factory ;
	}
	
} ;

// ---------------------------------------------------------------------------------------------------