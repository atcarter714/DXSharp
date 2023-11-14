#region Using Directives

using System.Runtime.Versioning;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.Applications ;


/// <summary>
/// Delegate which defines a managed signature for a <see cref="WndProc"/> function,
/// which is used to handle or listen to window messages.
/// </summary>
public delegate void WndProcDelegate( IAppWindow window, Message msg, 
									  WParam wParam, LParam lParam ) ;



/// <summary>Abstract base class for DXSharp applications.</summary>
[SupportedOSPlatform( "windows7.0" )]
public abstract class DXAppBase: DisposableObject, IDXApp {
	// ---------------------------------------------------------
	/// <summary>Indicates if a shutdown command or event has fired.</summary>
	/// <remarks>
	/// Child classes that inherit from this class can check this value to determine if they
	/// should abort loops/threads or halt execution of certain code paths.
	/// </remarks>
	protected bool _Quitting { get ; private set ; }
	bool _shutdownComplete = false ;
	// ---------------------------------------------------------
	
	
	public virtual bool CanDraw { get ; protected set ; }
	public virtual bool CanTick { get ; protected set ; }
	public virtual bool IsPaused { get; protected set ; }
	public virtual bool IsRunning { get ; protected set ; }
	public virtual bool IsDrawingPaused { get ; protected set ; }
	public virtual bool IsSimulationPaused { get ; protected set ; }
	
	public virtual string Title {
		get => Window?.Title ?? string.Empty ; 
		protected set => Window?.SetTitle( value ) ;
	}
	public Size CurrentSize => ( Window?.Size ?? Size.Empty ) ;
	public Size DesiredSize { get ; protected set ; } 
					= AppSettings.DEFAULT_WINDOW_SIZE ;
	
	public abstract IAppWindow? Window { get ; }
	public abstract ITimeProvider? Time { get ; }
	public virtual bool IsInitialized { get ; protected set ; }
	public virtual AppSettings Settings => AppSettings.Default ;
	
	
	// ---------------------------------------------------------
	// Event Handlers ::
	// ---------------------------------------------------------
	
	protected virtual void OnAppQuit( object? sender, EventArgs e ) {
		if ( this.IsRunning && !_Quitting ) Shutdown( ) ;
	}
	
	protected virtual void OnInitialized( ) {
		_shutdownComplete = false ;
		Application.ApplicationExit += OnAppQuit ;
	}
	
	protected abstract void OnRegisterWindow( IAppWindow window ) ;
	protected abstract void OnUnregisterWindow( IAppWindow window ) ;
	
	protected virtual void OnDPIChanged( object sender, DPIChangedEventArgs e ) {
		//this.DesiredSize = e.NewRect.Size ;
	}
	
	
	// ------------------------------------------------------------------------------------
	
	
	
	// ---------------------------------------------------------
	//! App Lifecycle Methods ::
	// ---------------------------------------------------------
	
	public virtual IDXApp Initialize( ) {
		DesiredSize = Settings.WindowSize ;
		Title = Settings.Title ;
		IsInitialized = true ;
		OnInitialized( ) ;
		return this ;
	}
	
	public virtual void Shutdown( ) {
		_Quitting = IsPaused = IsDrawingPaused = IsSimulationPaused = true ;
		IsRunning = CanTick = CanDraw = IsInitialized = false ;
		
		if ( Window is { IsVisible: true } ) 
				Window.Close( ) ;
		Time?.Stop( ) ;
		
		_shutdownComplete = true ;
	}
	
	public virtual void Load( ) { }
	public virtual void Unload( ) { }
	
	public virtual void Run( ) {
		Time?.Start( ) ;
		CanTick = CanDraw = true ;
		IsDrawingPaused = IsSimulationPaused = 
					IsPaused = !(IsRunning = true) ;
	}

	public virtual void Draw( ) { }

	public virtual void Update( ) {
		if( !IsRunning || !CanTick || IsPaused ) return ;
		Time?.Update( ) ;
	}
	
	public virtual void Tick( float delta ) {
		//if( !IsRunning || !CanTick || IsPaused ) return ;
		
	}
	
	
	// --------------------------------------------------------------------------------------
	#region Dispose Pattern
	
	protected override ValueTask DisposeUnmanaged( ) {
		if( !Window?.IsDisposed ?? false ) 
			Window?.Dispose( ) ;
		
		return ValueTask.CompletedTask ;
	}

	protected override void Dispose( bool disposing ) {
		if( !_shutdownComplete ) Shutdown( ) ;
		DisposeUnmanaged( ) ;
		if ( disposing ) DisposeManaged( ) ;
	}
	
	async ValueTask DisposeAsyncCore( ) => 
		await Task.Run( DisposeUnmanaged ) ;
	
	/*public void Dispose( ) {
		Dispose( true ) ;
		GC.SuppressFinalize( this ) ;
	}*/
	public override async ValueTask DisposeAsync( ) {
		await DisposeAsyncCore( ) ;
		GC.SuppressFinalize( this ) ;
	}
	
	~DXAppBase( ) => Dispose( false ) ;
	#endregion
	// ======================================================================================
} ;




//Application.Idle += ( s, e ) => { if ( Window is not null ) Window.Invalidate( ) ; } ;
/* Unfortunately, this is only for WinRT apps:
protected virtual void OnAppMemoryUsageIncreased( object? sender, object e ) { }
protected virtual void OnAppMemoryUsageDecreased( object? sender, object e ) { }
protected virtual void OnAppMemoryUsageLimitChange( object? sender, object e ) { }
	
 static readonly Version MinVersionForMemoryNotifications = 
	new( 10, 0, 10240, 0 ) ;

[SupportedOSPlatform("windows10.0.10240.0")]
void _registerForMemoryNotifications( ) {
	MemoryManager.AppMemoryUsageIncreased     += OnAppMemoryUsageIncreased ;
	MemoryManager.AppMemoryUsageDecreased     += OnAppMemoryUsageDecreased ;
	MemoryManager.AppMemoryUsageLimitChanging += OnAppMemoryUsageLimitChange ;
}


		
		// --------------------------------------------------------------------------------------
		//! Register for memory notifications:
		// --------------------------------------------------------------------------------------
		//  This uses a "platform guard" pattern to prevent the unsupported call site from being
		// reached on older versions of Windows 10 or earlier which cannot support the call ...
/*#if !WINDOWS10_0_17763_0_OR_GREATER
		return ;
#else
		if( Environment.OSVersion.Version >= MinVersionForMemoryNotifications )
#pragma warning disable CA1416
			_registerForMemoryNotifications( ) ;
#pragma warning restore CA1416
#endif* /
		// ======================================================================================

*/