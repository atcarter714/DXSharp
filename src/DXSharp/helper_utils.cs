
#pragma warning disable CS8981, CS1591

#region Using Directives
using global::System;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Graphics.Direct3D11;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;

using winmdroot = global::Windows.Win32;
#endregion

namespace DXSharp.AC;


/// <summary>
/// Enumeration representing the major GPU vendors
/// </summary>
public enum GPUVendor: uint
{
	Unknown		= 0x0000U,
	Nvidia		= 0x10DEU,
	AMD			= 0x1002U,
	Intel		= 0x8086U,
};


public static class AC_Utils
{
	public const uint VendorID_Nvidia   = 0x10DE;
	public const uint VendorID_AMD      = 0x1002;
	public const uint VendorID_Intel    = 0x8086;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ConvertDipsToPixels( float dips, float dpi ) => (int)(dips * dpi / 96.0f + 0.5f);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float ConvertPixelsToDips( int pixels, float dpi ) => (((float)pixels) * 96f / dpi);



	internal unsafe static uint GetVendorIdFromDevice<T>( T pDevice ) where T: ID3D12Device {

		try {
			// Get the adapter LUID:
			var luid = pDevice.GetAdapterLuid();

			// Obtain a DXGI factory:
			IDXGIFactory7? dxgiFactory = default;
			var hr = PInvoke.CreateDXGIFactory2( 0u, typeof(IDXGIFactory7).GUID, out var factoryObj );
			if ( hr.Failed )
				throw new COMException( "Failed to create IDXGIFactory!", hr );

			// Get the adapter by its LUID:
			IDXGIAdapter4? pAdapter = default;
			dxgiFactory?.EnumAdapterByLuid( luid, typeof( IDXGIAdapter4 ).GUID, out var adapterObj );

			// Get the desc and vendor ID:
			DXGI_ADAPTER_DESC1 desc = default;
			pAdapter?.GetDesc1( &desc );
			return desc.VendorId;
		}

		// Failed:
		catch ( Exception ) { return 0x00u; }
	}
};