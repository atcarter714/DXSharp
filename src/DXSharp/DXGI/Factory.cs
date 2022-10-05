#region Using Directives
using global::System;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;

using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D11;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;

using global::Windows.Win32;
using Win32 = global::Windows.Win32;
#endregion

namespace Windows.Win32
{
	internal static partial class PInvoke
	{
		///// <summary>
		///// Creates a DXGIFactoryX COM object
		///// </summary>
		///// <typeparam name="T">Type of DXGIFactoryX</typeparam>
		///// <param name="Flags">Creation flags</param>
		///// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
		///// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
		//[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		//internal static unsafe T? CreateDXGIFactory2<T>( uint Flags, out HRESULT hr ) where T : IDXGIFactory
		//{
		//	var riid = typeof( T ).GUID;
		//	hr = CreateDXGIFactory2( Flags, &riid, out var factoryObj );
		//	return hr.Succeeded ? (T) factoryObj : default( T );
		//}

		///// <summary>
		///// Creates a DXGIFactoryX COM object
		///// </summary>
		///// <typeparam name="T">Type of DXGIFactoryX</typeparam>
		///// <param name="Flags">Creation flags</param>
		///// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
		//[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		//internal static unsafe T? CreateDXGIFactory2<T>( uint Flags ) where T : IDXGIFactory
		//{
		//	var factory = CreateDXGIFactory2<T>( Flags, out var hr );
		//	hr.ThrowOnFailure();
		//	return factory;
		//}

		// internal partial interface IDXGIFactory { }
	}
}

namespace DXSharp.DXGI
{
	public static class Factory
	{
		// Native Interop Signatures:
		//HRESULT CreateDXGIFactory( in global::System.Guid riid, out object ppFactory );
		//HRESULT CreateDXGIFactory( global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppFactory );
		//HRESULT CreateDXGIFactory1( in global::System.Guid riid, out object ppFactory );
		//HRESULT CreateDXGIFactory1( global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppFactory );
		//HRESULT CreateDXGIFactory2( uint Flags, in global::System.Guid riid, out object ppFactory );
		//HRESULT CreateDXGIFactory2( uint Flags, global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppFactory );

		//HRESULT DXGIGetDebugInterface1( uint Flags, in global::System.Guid riid, out object pDebug );
		//HRESULT DXGIGetDebugInterface1( uint Flags, global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object pDebug );
		//HRESULT DXGIDeclareAdapterRemovalSupport();

		#region Internal Methods

		/// <summary>
		/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
		/// </summary>
		/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
		/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
		/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		internal static T? CreateDXGIFactory<T>( out HRESULT hr ) where T : IDXGIFactory
		{
			unsafe
			{
				var riid = typeof( T ).GUID;
				hr = PInvoke.CreateDXGIFactory( &riid, out var factoryObj );
				return hr.Succeeded ? (T) factoryObj : default( T );
			}
		}

		/// <summary>
		/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
		/// </summary>
		/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
		/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
		/// <exception cref="COMException">Thrown if the call fails and contains detailed error information</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		internal static T? CreateDXGIFactory<T>() where T : IDXGIFactory
		{
			var factory = Factory.CreateDXGIFactory<T>( out var hr );
			hr.ThrowOnFailure();
			return factory;
		}


		/// <summary>
		/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
		/// </summary>
		/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
		/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
		/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		internal static T? CreateDXGIFactory1<T>( out HRESULT hr ) where T : IDXGIFactory
		{
			unsafe
			{
				var riid = typeof( T ).GUID;
				hr = PInvoke.CreateDXGIFactory1( &riid, out var factoryObj );
				return hr.Succeeded ? (T) factoryObj : default( T );
			}
		}

		/// <summary>
		/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
		/// </summary>
		/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
		/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
		/// <exception cref="COMException">Thrown if the call fails and contains detailed error information</exception>
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		internal static T? CreateDXGIFactory1<T>() where T : IDXGIFactory
		{
			var factory = CreateDXGIFactory1<T>( out var hr );
			hr.ThrowOnFailure();
			return factory;
		}


		/// <summary>
		/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
		/// </summary>
		/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
		/// <param name="Flags">Creation flags</param>
		/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
		/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		internal static T? CreateDXGIFactory2<T>( uint Flags, out HRESULT hr ) where T : IDXGIFactory
		{
			unsafe
			{
				var riid = typeof( T ).GUID;
				hr = PInvoke.CreateDXGIFactory2( Flags, &riid, out var factoryObj );
				return hr.Succeeded ? (T) factoryObj : default( T );
			}
		}

		/// <summary>
		/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
		/// </summary>
		/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
		/// <param name="Flags">Creation flags</param>
		/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		internal static T? CreateDXGIFactory2<T>( uint Flags ) where T : IDXGIFactory
		{
			var factory = CreateDXGIFactory2<T>( Flags, out var hr );
			hr.ThrowOnFailure();
			return factory;
		}

		#endregion


	}
}