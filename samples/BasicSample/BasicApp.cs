#region Using Directives
using System.Windows.Forms ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Direct3D12 ;
using DXSharp.Applications ;
using DXSharp.Direct3D12.Debug ;
using DXSharp.DXGI.XTensions ;
using DXSharp.Windows.COM ;

using Device = DXSharp.Direct3D12.Device ;
using IDevice = DXSharp.Direct3D12.IDevice ;
using IResource = DXSharp.Direct3D12.IResource ;
using Resource = DXSharp.Direct3D12.Resource ;
#endregion
namespace BasicSample ;


public class BasicApp: DXWinformApp {
	Debug? dbgLayer ;
	Graphics? graphics ;
	
	public override void Initialize( ) {
		base.Initialize( ) ;
		Window!.SetTitle( Settings.Title) ;
		Window.SetSize( new (1920, 1080) );
#if DEBUG
		// Enable debug layer in debug builds:
		dbgLayer = Debug.CreateDebugLayer( ) ;
		dbgLayer.EnableDebugLayer( ) ;
#endif
		
		graphics = new( this ) ;
		graphics.LoadPipeline( ) ;
		graphics.LoadAssets( ) ;
	}

	public override void Draw( ) {
		base.Draw( ) ;
		graphics!.OnRender( ) ;
	}
}


	
/*static Adapter? _findBestAdapter( IFactory factory ) {
	Adapter?                                                  adapter     = null ;
	AdapterDescription                                        desc        = default ;
	List< (Adapter Adapter, AdapterDescription Description) > allAdapters = new( ) ;

	for ( int i = 0; i < 8; ++i ) {
		var hr = factory.EnumAdapters< Adapter >( (uint)i, out var _adapter ) ;
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
}*/