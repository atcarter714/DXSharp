#region Using Directives
using System ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp ;
using DXSharp.DXGI;
using DXSharp.Windows;
using DXSharp.Direct3D12;
using DXSharp.Applications ;
using DXSharp.Windows.COM ;
#endregion
namespace BasicSample ;


public class BasicApp: DXWinformApp {
	
	public override void Initialize( ) {
		base.Initialize( ) ;
		_enableDebugLayer( ) ;
		var factory = Factory.Create( ) ;
		
		Adapter1? adapter = null ;
		AdapterDescription1 desc = default ;
		List< (Adapter1 Adapter, AdapterDescription1 Description) > allAdapters = new( ) ;

		for ( int i = 0; i < 8; ++i ) {
			var hr = factory.EnumAdapters< Adapter1 >( 0, out var _adapter ) ;
			if ( hr.Failed ) {
				if ( hr == HResult.DXGI_ERROR_NOT_FOUND ) break ;
				throw new DirectXComError( hr, $"{nameof( DXWinformApp )} :: Error enumerating adapters! " +
											   $"HRESULT: 0x{hr.Value:X} ({hr.Value})" ) ;
			}
			if ( _adapter is null ) continue ;
			_adapter.GetDesc1( out var _desc ) ;
			allAdapters.Add( (_adapter, _desc) ) ;
			
			if ( adapter is null ) {
				adapter = _adapter ;
				desc	= _desc ;
				continue ;
			}
			
			if ( _desc.DedicatedVideoMemory > desc.DedicatedVideoMemory ) {
				adapter = _adapter ;
				desc	= _desc ;
			}
		}
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