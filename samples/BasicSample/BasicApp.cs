#region Using Directives
using System ;
using System.Runtime.InteropServices ;
using System.Windows.Forms ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp ;
using DXSharp.DXGI;
using DXSharp.Windows;
using DXSharp.Direct3D12;
using DXSharp.Applications ;
using DXSharp.DXGI.XTensions ;
using DXSharp.Windows.COM ;
#endregion
namespace BasicSample ;


public class BasicApp: DXWinformApp {
	
	public override void Initialize( ) {
		base.Initialize( ) ;
#if DEBUG
		_enableDebugLayer( ) ;
#endif
		
		var factory = Factory.Create< Factory >( ) ;
		Adapter? adapter = (Adapter)factory.FindBestAdapter< Adapter >( )! ;
		
		adapter.GetDesc( out var desc) ;
		string gpuName = desc.Description.ToString( ) ?? string.Empty ;
		MessageBox.Show( $"Best GPU Adapter: {gpuName}" ) ;

		var deviceType = typeof( ID3D12Device ) ;
		HResult hr = PInvoke.D3D12CreateDevice( adapter.COMObject,
												D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_0,
												deviceType.GUID,
												out var _ppDevice ) ;
		if ( hr.Failed || _ppDevice is null ) {
			MessageBox.Show( $"Failed to initialize {deviceType.Name}!", 
							 "D3D12 ERROR (DX#):" ) ;
		}
		else MessageBox.Show( $"Successfully initialized {deviceType.Name}!", 
							   "DX# Success:" ) ;
	}

	
	
	static Adapter? _findBestAdapter( IFactory factory ) {
		Adapter?                                                  adapter     = null ;
		AdapterDescription                                        desc        = default ;
		List< (Adapter Adapter, AdapterDescription Description) > allAdapters = new( ) ;

		for ( int i = 0; i < 8; ++i ) {
			var hr = factory.EnumAdapters< Adapter >( 0, out var _adapter ) ;
			if ( hr.Failed ) {
				if ( hr == HResult.DXGI_ERROR_NOT_FOUND ) break ;
				throw new DirectXComError( hr, $"{nameof( DXWinformApp )} :: Error enumerating adapters! " +
											   $"HRESULT: 0x{hr.Value:X} ({hr.Value})" ) ;
			}

			if ( _adapter is null ) continue ;
			_adapter.GetDesc( out var _desc ) ;
			allAdapters.Add( ( _adapter, _desc ) ) ;

			if ( adapter is null ) {
				adapter = _adapter ;
				desc    = _desc ;
				continue ;
			}

			if ( _desc.DedicatedVideoMemory > desc.DedicatedVideoMemory ) {
				adapter = _adapter ;
				desc    = _desc ;
			}
		}

		return adapter ;
	}

	static unsafe ComPtr< ID3D12Debug >? _enableDebugLayer( ) {
		ComPtr< ID3D12Debug >? debugController = null ;
		try {
			var dbgGuid = typeof(ID3D12Debug).GUID ;
			var hrDbg   = PInvoke.D3D12GetDebugInterface( &dbgGuid, out var _debug ) ;
			if ( hrDbg.Failed || _debug is null )
				throw new DirectXComError( $"Failed to create the {nameof( ID3D12Debug )} layer!" ) ;
		
			debugController = new( (ID3D12Debug)_debug ) ;
			( (ID3D12Debug)debugController.Interface! )
				.EnableDebugLayer( ) ;
		}
		catch ( COMException ex ) { return null ; }
		catch ( Exception ex ) { return null ; }
		finally {  }
		return debugController ;
	}
}