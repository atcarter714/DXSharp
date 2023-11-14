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

using DXSharp.Windows ;
using DXSharp.Windows.Win32 ;
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
		ArgumentNullException.ThrowIfNull( settings, nameof(settings) ) ;
		_settings   = settings ;
	}
	
	public DXWinformApp( in RenderForm window, in AppSettings? settings = null ) {
		ArgumentNullException.ThrowIfNull( window, nameof( window ) ) ;
		_form       = window ;
		_settings   = settings ?? AppSettings.Default ;
	}
	

	// -------------------------------------------------------------------------------------------
	// Instance Methods:
	// -------------------------------------------------------------------------------------------
	
	//! Event Handler Overrides: -----------------------------------------------------------------
	protected override void OnAppQuit( object? sender, EventArgs e ) {
		_abort = true ;
		base.OnAppQuit( sender, e ) ;
		AppCancelTokenSource?.Cancel( ) ;
	}
	
	protected override void OnDPIChanged( object sender, DPIChangedEventArgs e ) {
		base.OnDPIChanged( sender, e ) ;
		
		/*this.DesiredSize = _form?.LogicalToDeviceUnits( e.NewRect.Size ) 
									?? e.NewRect.Size ;
		
		this.Window?.SetSize( this.DesiredSize ) ;*/
	}
	

	void OnWindowOnContentsResized( object? s, ContentsResizedEventArgs e ) {
		//this.DesiredSize = e.NewRectangle.Size ;
	}
	
	
	protected override void OnRegisterWindow( IAppWindow window ) {
		ObjectDisposedException.ThrowIf( _form is null, typeof(RenderForm) ) ;
		_form.ContentsResized        += OnWindowOnContentsResized ;
		_form.ClientSizeChanged      += OnClientSizeChanged ;
		_form.DPIChanged             += OnDPIChanged ;
		_form.WindowsMessageReceived += OnWndProc ;
		_form!.FormClosing           += OnAppQuit ;
	}
	
	protected override void OnUnregisterWindow( IAppWindow window ) {
		if( _form is null ) return ;
		_form.ContentsResized -= OnWindowOnContentsResized ;
		_form.DPIChanged      -= OnDPIChanged ;
		_form.FormClosing     -= OnAppQuit ;
	}
	
	
	// -------------------------------------------------------------------------------------------
	//! WinForm Event Handlers:
	void OnWndProc( IAppWindow window, Message msg, WParam wparam, LParam lparam ) {}
	
	void OnShowForm( object? sender, EventArgs e ) {
		_form!.Shown -= OnShowForm ;
		if( Title != Settings.Title ) Window!.SetTitle( Settings.Title ) ;
		if( Window!.Size != DesiredSize ) Window!.SetSize( DesiredSize ) ;
		
		_form.PauseRendering         += ( o, a ) => IsDrawingPaused = true ;
		_form.ResumeRendering        += ( o, a ) => IsDrawingPaused = false ;
	}
	void OnClientSizeChanged( object? sender, EventArgs e ) { }
	
	void OnWindowHandleCreated( object? sender, EventArgs e ) {
		_form!.HandleCreated   -= OnWindowHandleCreated ;
		_form!.HandleDestroyed += OnWindowHandleDestroyed ;
		OnRegisterWindow( _form ) ;
		
		if( !_form.Visible ) _form.Show( ) ;
		if ( _form!.IsMinimized )
			_form!.WindowState = FormWindowState.Normal ;
	}
	void OnWindowHandleDestroyed( object? sender, EventArgs e ) {
		if( _form is null ) return ;
		_form!.HandleDestroyed -= OnWindowHandleDestroyed ;
		_form!.HandleCreated += OnWindowHandleCreated ;
		OnUnregisterWindow( _form ) ;
	}
	// -------------------------------------------------------------------------------------------
	
	
	public override DXWinformApp Initialize( ) {
		var _appFont =
			Settings.StyleSettings.Font ?? new Font( Settings.StyleSettings.FontName 
														?? AppSettings.Style.DEFAULT_FONT_NAME, 
															Settings.StyleSettings.FontSize ) ;
		Application.SetCompatibleTextRenderingDefault( true ) ;
		Application.SetDefaultFont( _appFont ) ;
		DesiredSize = Settings.WindowSize ;
		
		_form = new RenderForm( Title, DesiredSize ) ;
		_form.HandleCreated   += OnWindowHandleCreated ;
		_form.Shown           += OnShowForm ;
		
		// Configure parallelism and thread setup:
		cancelTick = AppCancelTokenSource.Token ;
		cancelDraw = AppCancelTokenSource.Token ;
		Action renderFunc   = _RenderWork,
			   computeFunc  = _ComputeWork,
			   simulateFunc = _SimulationWork ;
		
		workActions = new [ ] { simulateFunc, renderFunc, computeFunc } ;
		workActionConfig = new ParallelOptions {
			MaxDegreeOfParallelism = Mathf.Min( workActions.Length,
												  HardwareInfo.MaxParallelism ),
			CancellationToken      = AppCancelTokenSource.Token,
			TaskScheduler          = null,
		} ;
		
		Application.SetHighDpiMode( HighDpiMode.PerMonitorV2 ) ;
		Application.EnableVisualStyles( ) ;
		
		this.IsPaused = _abort = false ;
		base.Initialize( ) ;
		return this ;
	}

	
	public override void Run( ) {
		base.Run( ) ;
		
	    RenderLoop.Run( _form, _MainLoop ) ;
	    _DispatchJobs( ) ;
	}
	public override void Shutdown( ) {
		if( _Quitting ) return ;
		base.Shutdown( ) ;
		_abort = true ;
		
		AppCancelTokenSource.Cancel( ) ; 
		_time = null ;
	}
	
	public override void Load( ) { }
	public override void Unload( ) { }
	
	protected virtual void _RenderWork( ) => Thread.Sleep( 1 ) ;
	protected virtual void _ComputeWork( ) => Thread.Sleep( 1 ) ;
	protected virtual void _SimulationWork( ) => Thread.Sleep( 1 ) ;
	
	
	public override void Tick( float delta ) {
		if( !IsRunning || !CanTick || IsPaused ) return ;
		if ( IsSimulationPaused ) return ;
		base.Tick( delta ) ;
	}
	public override void Update( ) {
		if( !IsRunning || !CanTick || IsPaused ) return ;
		if ( IsSimulationPaused ) return ;
		base.Update( ) ;
	}
	public override void Draw( ) {
		if( !IsRunning || !CanDraw || IsDrawingPaused ) return ;
		base.Draw( ) ;
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
		if ( _abort || IsRunning || IsPaused || !CanTick ) return ;
		
		//! Parallel Thread Settings Checks ::
		if( !Settings.AdvancedSettings?.EnableParallelJobs ?? false ) return ;
		if( workActions is not { Length: > 0 } ) return ;
		if( workActionConfig is null ) return ;
		
		Parallel.Invoke( workActionConfig, workActions ) ;
	}
	
	
	
	// -------------------------------------------------------------------------------------------
	#region Dispose Pattern Overrides
	protected override void DisposeManaged( ) {
		_abort = true ; //! Set abort flag to make sure threads will exit ...
		CanDraw = CanTick = IsRunning = IsInitialized = false ;
		
		try {
			//! Fire a cancellation request to all threads:
			if ( !AppCancelTokenSource?.IsCancellationRequested ?? false )
				AppCancelTokenSource?.Cancel( ) ;
			
			//! Try to "gently kill" any rogue threads that are still running:
			if( _simulationTasks is { Length: > 0 } ) {
				const int MAX_WAIT_TIME = 3500 ;
				var _waitTimeSpan1 = TimeSpan.FromMilliseconds( MAX_WAIT_TIME ) ;
				
				List< Task > waitingFor = new( _simulationTasks.Length ) ;
				var _waitCancelToken = AppCancelTokenSource?.Token
									   ?? new CancellationToken(false) ;
				
				//! Find any tasks that are still running and try waiting for them:
				foreach( Task? task in _simulationTasks ) {
					try {
						if ( task is { Status: TaskStatus.Running } ) {
							var _waitTask = task.WaitAsync( _waitTimeSpan1, _waitCancelToken ) ;
							waitingFor.Add( _waitTask ) ;
						}
					}
					catch ( Exception exWait ) { _OnError( exWait ) ; }
				}
				
				bool waitAllResult = Task.WaitAll( waitingFor.ToArray( ), 
												   MAX_WAIT_TIME, _waitCancelToken ) ;
				if ( !waitAllResult ) {
					//! If we still have tasks running, try to cancel them:
					foreach( Task? task in _simulationTasks ) {
						try {
							if ( task is { Status: TaskStatus.Running } ) {
								task.Wait( 100, new CancellationToken(true) ) ;
								task.Dispose( ) ;
							}
						}
						catch ( Exception exWait ) { _OnError( exWait ) ; }
					}
				}
				
				waitingFor.Clear( ) ;
				_simulationTasks = null ;
			}
			AppCancelTokenSource?.Dispose( ) ;
			
			if ( _form is { IsDisposed: false, Disposing: false } ) {
				if ( _form.IsHandleCreated ) {
					_form.HandleCreated   -= OnWindowHandleCreated ;
					_form.HandleDestroyed -= OnWindowHandleDestroyed ;
				}

				if ( _form.Visible ) _form.Close( ) ;
				_form?.Dispose( ) ;
			}
		}
		catch( Exception ex ) { _OnError( ex ) ; }
		finally { _form = null ; _time = null ; }
		
		
		static void _OnError( Exception ex ) {
#if DEBUG || DEV_BUILD
			System.Diagnostics.Debug.WriteLine( $"{nameof(DXWinformApp)} :: " +
												$"An error occurred in {nameof(DisposeManaged)} ...\n" +
												$"Error (HRESULT - 0x{ex.HResult:X}): \"{ex.Message}\"" ) ;
#endif
		}
	}
	
	protected override async ValueTask DisposeUnmanaged( ) {
		var _task1 = base.DisposeUnmanaged( ) ;
		var _task2 = Task.Run( Dispose ) ;
		await Task.WhenAll( _task1.AsTask( ), _task2 ) ;
	}
	#endregion
	// ===========================================================================================
} ;

