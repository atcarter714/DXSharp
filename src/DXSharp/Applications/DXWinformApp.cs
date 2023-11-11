/* NOTE :: WILL BE MOVED ...
 * All of this in the \Applications folder will be moved ...
 * Since we want DXSharp to be a modular set of packages which don't
 * force you to have lots of dependencies and bloat you don't want or
 * need, all of this stuff will be moved into another library/package
 * like "DXSharp.Framework" or something, which provides the user with
 * these kinds of classes/types and features to quickly get an app up
 * and running in C# with DirectX 12 ...
 */

#region Using Directives
using System.Runtime.Versioning ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using static Windows.Win32.PInvoke ;
#endregion
namespace DXSharp.Applications ;


[SupportedOSPlatform("windows7.0")]
public class DXWinformApp: DXAppBase, IDXWinformApp {
	internal static readonly CancellationTokenSource AppCancelTokenSource = new( ) ;
	readonly object _gameStateLock = new( ) ;
	volatile bool _abort = false ;
	
	RenderForm? _form ;
	ITimeProvider? _time ;
	AppSettings? _settings ;
	public Form? MainForm => _form ;
	
	public override IAppWindow? Window => _form ;
	public override AppSettings Settings => ( _settings ??= base.Settings ) ;
	public override ITimeProvider Time => ( _time ??= new Time(AppCancelTokenSource) ) ;
	
	
	Task[ ]? _simulationTasks ;
	Action[ ]? workActions = null ;
	ParallelOptions?  workActionConfig ;
	CancellationToken cancelTick, cancelDraw ;
	ManualResetEvent _startFrameSignal = new( false ),
					 _endFrameSignal = new( false ) ;
	
	protected Action? MainLoopCallback { get ; set ; }
	protected Action? RenderLoopCallback { get ; set ; }
	protected Action? SimulationLoopCallback { get ; set ; }
	
	// --------------------------------------
	// Constructors:
	// --------------------------------------
	
	public DXWinformApp( ) => _settings = AppSettings.Default ;

	public DXWinformApp( in AppSettings? settings ) {
		ArgumentNullException.ThrowIfNull( settings, nameof( settings ) ) ;
		_settings   = settings ;
	}
	
	public DXWinformApp( in RenderForm window, in AppSettings? settings = null ) {
		ArgumentNullException.ThrowIfNull( window, nameof( window ) ) ;
		_form       = window ;
		_settings   = settings ?? AppSettings.Default ;
	}
	
	// --------------------------------------
	// Event Handlers:
	// --------------------------------------
	
	protected override void OnAppQuit( object? sender, EventArgs e ) => Shutdown( ) ;
	
	// --------------------------------------
	// Instance Methods:
	// --------------------------------------
	
	public override void Initialize( ) {
		Application.SetCompatibleTextRenderingDefault( true ) ;
		Application.SetDefaultFont( Settings.StyleSettings.Font ) ;
		
		_form = new RenderForm( Title ) ;
		_form.Show( ) ;
		_form.ForeColor = Color.Black ;
		Window?.SetTitle( Settings.Title ) ;
		_form.ClientSize = DesiredSize ;
		Window?.SetSize( DesiredSize ) ;
		
		_form.PauseRendering += ( s, e )  => IsPaused = true ;
		_form.ResumeRendering += ( s, e ) => IsPaused = false ;
		_form.UserResized += ( s, e )  => DesiredSize = e.NewSize ;
		
		// Configure parallelism and thread setup:
		cancelTick = AppCancelTokenSource.Token ;
		cancelDraw = AppCancelTokenSource.Token ;
		Action renderFunc   = _RenderWork,
			   computeFunc  = _ComputeWork,
			   simulateFunc = _SimulationWork ;
		
		workActions = new [ ] { simulateFunc, renderFunc, computeFunc } ;
		workActionConfig = new ParallelOptions {
			MaxDegreeOfParallelism = Math.Min( workActions.Length, HardwareInfo.MaxParallelism ),
			CancellationToken      = AppCancelTokenSource.Token,
			TaskScheduler          = null,
		} ;
		
		this.IsPaused = _abort = false ;
		
		base.Initialize( ) ;
	}
	
	public override void Run( ) {
	    Time.Start( ) ;
	    IsPaused  = !(IsRunning = true) ;
	    RenderLoop.Run( _form, _MainLoop ) ;
	    
	    //_DispatchJobs( ) ;
	}
	public override void Shutdown( ) {
		if( _Quitting ) return ;
		AppCancelTokenSource.Cancel( ) ;
		
		base.Shutdown( ) ;
		_abort = true ;
		_time = null ;
	}
	
	public override void Load( ) { }
	public override void Unload( ) { }
	
	protected virtual void _RenderWork( ) => Thread.Sleep( 1 ) ;
	protected virtual void _ComputeWork( ) => Thread.Sleep( 1 ) ;
	protected virtual void _SimulationWork( ) => Thread.Sleep( 1 ) ;
	
	public override void Tick( float delta ) => Time?.Update( ) ;

	public override void Update( ) {
		if( !IsRunning ) return ;
		if( !IsPaused ) return ;
		Time?.Update( ) ;
	}
	public override void Draw( ) {
		if( !IsRunning ) return ;
	}
	
	
	// Main Thread Game Loop:
	void _MainLoop( ) {
		if ( _abort || !IsRunning || IsPaused ) return ;
		
		// Signal the start of a frame to simulation threads
		_startFrameSignal.Set( ) ;
		Update( ) ;
		Draw( ) ;
	}
	
	// Simulation Thread Loops:
	void _DispatchJobs( ) {
		//! Return if already running, aborted or paused:
		if ( _abort || IsRunning || IsPaused ) return ;
		
		//! Parallel Thread Settings Checks ::
		if( !Settings.AdvancedSettings?.EnableParallelJobs ?? false ) return ;
		if( workActions is not { Length: > 0 } ) return ;
		if( workActionConfig is null ) return ;
		
		Parallel.Invoke( workActionConfig, workActions ) ;
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
		catch { return null ; }
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
	
#region Dispose Pattern Overrides
	protected override void OnDestroyManaged( ) { }
	protected override void OnDestroyUnmanaged( ) { }
#endregion
} ;