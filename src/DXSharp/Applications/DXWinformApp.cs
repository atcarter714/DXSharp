#region Using Directives
using System.Threading ;
using System.Diagnostics ;
using System.Threading.Tasks ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using static Windows.Win32.PInvoke ;
#endregion
namespace DXSharp.Applications ;


public class DXWinformApp: IDXWinformApp {
	const string DEFAULT_APP_NAME = "DXSharp" ;
	
	Time? _time ;
	RenderForm? _form ;
	AppSettings _settings ;
	public Form? Window => _form ;
	
	public string Title { get ; protected set ; }
	public bool IsPaused { get ; protected set ; }
	public bool IsRunning { get ; protected set ; }
	public bool IsInitialized { get ; protected set ; }
	public ITimeProvider GameTime { get ; protected set ; }
	
	public Size DesiredSize { get ; protected set ; }
	public Size CurrentSize => _form?.ClientSize ?? Size.Empty ;
	
	
	public DXWinformApp( ) {
		_time = new( ) ;
		_settings   = new( DEFAULT_APP_NAME, ( 1280, 720 ) ) ;
		Title       = DEFAULT_APP_NAME ;
		DesiredSize = (USize)( 1280, 720 ) ;
	}
	public DXWinformApp( in AppSettings settings ) {
		_time       = new( ) ;
		_settings   = settings ;
		Title       = settings.Title ;
		DesiredSize = settings.WindowSize ;
	}
	
	public virtual void Initialize( ) {
		_form = new( this.Title ) ;
		_form.Text = DEFAULT_APP_NAME ;
		_form.ClientSize = new( 1280, 720 ) ;
		_form.Show( ) ;
		
		_time ??= new( ) ;
		GameTime = _time ;
		this.IsInitialized = true ;
	}

	public virtual void Shutdown( ) {
		GameTime?.Stop( ) ;
		_time = null ;
		_form?.Close( ) ;
		this.IsInitialized = false ;
	}

	public virtual void Load( ) { }
	public virtual void Unload( ) { }

	protected virtual void _RunUpdates( ) {
		Task.Run( ( ) => {
					  GameTime?.Update( ) ;
				  } ) ;
	}
	protected virtual void _RunRendering( ) {
		Task.Run( ( ) => {
					  
				  } ) ;
	}
	
	public virtual void Tick( float delta ) {
		
	}
	public virtual void Draw( ) { }

	public virtual void Run( ) {
		GameTime ??= (_time ??= new()) ;
		GameTime.Start( ) ;
		RenderLoop.Run( _form, _MainLoop ) ;
	}
	
	void _MainLoop( ) {
		if ( !IsRunning || IsPaused ) return ;
		float delta = (float)(GameTime?.DeltaTime.TotalSeconds ?? 0f) ;
		Task.Run( _RunUpdates ) ;
		Task.Run( _RunRendering ) ;
		Tick( delta ) ;
		Draw( ) ;
	}
	
	
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