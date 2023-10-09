#region Using Directives

using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using static Windows.Win32.PInvoke ;
#endregion
namespace DXSharp.Applications ;


public class DXWinformApp: IDXWinformApp {
	const string DEFAULT_APP_NAME = "DXSharp App" ;
	
	RenderForm? _form ;
	AppSettings _settings ;
	public Form? Window => _form ;
	
	public string Title { get ; }
	public Size DesiredSize { get ; }
	public Size CurrentSize => _form?.ClientSize ?? Size.Empty ;
	
	public DXWinformApp( ) {
		_settings   = new( DEFAULT_APP_NAME, ( 1280, 720 ) ) ;
		Title       = DEFAULT_APP_NAME ;
		DesiredSize = (USize)( 1280, 720 ) ;
	}
	public DXWinformApp( in AppSettings settings ) {
		_settings   = settings ;
		Title       = settings.Title ;
		DesiredSize = settings.WindowSize ;
	}
	
	public void Initialize( ) {
		_form = new( this.Title ) ;
		_form.Text = DEFAULT_APP_NAME ;
		_form.ClientSize = new( 1280, 720 ) ;
		_form.Show( ) ;

		unsafe {
			_enableDebugLayer( ) ;
			var factory = DXFunctions.CreateFactory< Factory >( ) ;
			//Factory factory = _createFactory( ) ;
			//factory.CreateSwapChain( null, new(), out var swapChain ) ;
		}
	}


	public virtual void Shutdown( ) { }

	public virtual void Load( ) { }
	public virtual void Unload( ) { }
	
	public virtual void Tick( float delta ) { }
	public virtual void Draw( ) { }

	
	// --------------------------------------
	// Static Methods ::
	// --------------------------------------
	
	static unsafe ComPtr< ID3D12Debug >? _enableDebugLayer( ) {
		ComPtr< ID3D12Debug >? debugController = null ;
		try {
			var dbgGuid = typeof(ID3D12Debug).GUID ;
			var hrDbg = D3D12GetDebugInterface( &dbgGuid, out var _debug ) ;
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
	
	static unsafe Factory _createFactory( ) {
		var guid   = typeof( IDXGIFactory ).GUID ;
		var hrFact = CreateDXGIFactory( &guid, out var ppFactory ) ;
		if ( hrFact.Failed || ppFactory is null )
			throw new DirectXComError( "Failed to create DXGI Factory!" ) ;
		var factory = new Factory( ( ppFactory as IDXGIFactory )! ) ;
		return factory ;
	}

	// ======================================

	#region Dispose Pattern
	protected virtual void OnDestroyUnmanaged( ) { }
	protected virtual void Dispose( bool disposing ) {
		OnDestroyUnmanaged( ) ;
		if ( disposing ) {
			_form?.Dispose( ) ;
		}
	}
	protected virtual async ValueTask DisposeAsyncCore( ) => OnDestroyUnmanaged( ) ;
	
	public void Dispose( ) {
		Dispose( true ) ;
		GC.SuppressFinalize( this ) ;
	}
	public async ValueTask DisposeAsync( ) {
		await DisposeAsyncCore( ) ;
		GC.SuppressFinalize( this ) ;
	}
	~DXWinformApp( ) => Dispose( false ) ;
	#endregion
} ;