#region Using Directives
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
#endregion
namespace DXSharp.Direct3D12 ;


[SupportedOSPlatform("windows8.1")]
internal static class D3D12UT {
	
	internal static unsafe uint GetVendorIdFromDevice< T >( T pDevice ) 
												where T : ID3D12Device {
		ArgumentNullException.ThrowIfNull( pDevice, nameof(pDevice) ) ;
		const string msg = "Failed to get the vendor ID from the device!" ;
		
		try {
			// Get the adapter LUID:
			var luid = pDevice.GetAdapterLuid( ) ;

			// Obtain a DXGI factory:
			IDXGIFactory7? dxgiFactory = default ;
			var hr = PInvoke.CreateDXGIFactory2( 0x00U,
												 typeof( IDXGIFactory7 ).GUID,
												 out var factoryObj ) ;

			if ( hr.Failed )
				throw new COMException( $"{nameof( D3D12UT )} (internal) :: " +
										$"Failed to create {typeof( T ).Name}!", hr ) ;

			// Get the adapter by its LUID:
			IDXGIAdapter4? pAdapter = default ;
			dxgiFactory?.EnumAdapterByLuid( luid, typeof( IDXGIAdapter4 ).GUID, out object? adapterObj ) ;
			
			// Get the desc and vendor ID:
			DXGI_ADAPTER_DESC1 desc = default ;
			pAdapter?.GetDesc1( &desc ) ;
			return desc.VendorId ;
		}

		// Failed:
#if DEBUG || DEV_BUILD || DEBUG_COM
		catch ( COMException comErr ) {
			throw new
				DirectXComError( $"{nameof(D3D12UT)} (internal) :: {msg}", comErr ) ;
		}
		catch ( Exception err ) {
			throw new
				DXSharpException( $"{nameof(D3D12UT)} (internal) :: {msg}", err ) ;
		}
#else
		finally { }
		return 0x00000000 ;
#endif
	}
	
} ;