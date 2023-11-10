
using System.Runtime.Versioning ;
using Windows.System ;
using DXSharp.Applications ;

/// <summary>Abstract base class for DXSharp applications.</summary>
public abstract class DXAppBase: IDXApp {
	protected bool _Quitting { get ; private set ; }
	
	public virtual string Title {
		get => Window?.Title ?? string.Empty ; 
		protected set => Window?.SetTitle( value ) ;
	}
	public virtual bool IsPaused { get; protected set ; }
	public virtual bool IsRunning { get ; protected set ; }
	
	public Size CurrentSize => ( Window?.Size ?? Size.Empty ) ;
	public Size DesiredSize { get ; protected set ; }
	
	public abstract IAppWindow? Window { get ; }
	public abstract ITimeProvider? Time { get ; }
	public virtual bool IsInitialized { get ; protected set ; }
	public virtual AppSettings Settings => AppSettings.Default ;
	
	
	// --------------------------------------
	// Instance Methods ::
	// --------------------------------------
	static readonly Version MinVersionForMemoryNotifications = 
		new( 10, 0, 10240, 0 ) ;
	
	[SupportedOSPlatform("windows10.0.10240.0")]
	void _registerForMemoryNotifications( ) {
		MemoryManager.AppMemoryUsageIncreased     += OnAppMemoryUsageIncreased ;
		MemoryManager.AppMemoryUsageDecreased     += OnAppMemoryUsageDecreased ;
		MemoryManager.AppMemoryUsageLimitChanging += OnAppMemoryUsageLimitChange ;
	}
	
	
	protected abstract void OnAppQuit( object? sender, EventArgs e ) ;
	
	public virtual void Initialize( ) {
		Application.EnableVisualStyles( ) ;
		Application.ApplicationExit += OnAppQuit ;
		Application.SetCompatibleTextRenderingDefault( true ) ;
		Application.SetHighDpiMode( HighDpiMode.PerMonitorV2 ) ;
		Application.SetDefaultFont( Settings.StyleSettings.Font ) ;
		//Application.Idle += ( s, e ) => { if ( Window is not null ) Window.Invalidate( ) ; } ;
		
		
		DesiredSize = Settings.WindowSize ;
		Title = Settings.Title ;
		
		this.IsInitialized = true ;
		
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
#endif*/
		// ======================================================================================
	}

	public virtual void Shutdown( ) {
		_Quitting = true ;
		
		Application.ApplicationExit -= OnAppQuit ;
		this.IsInitialized = false ;
		this.IsRunning     = false ;
		Time?.Stop( ) ;
		
		if ( Window is not null && Window.IsVisible ) 
				Window.Close( ) ;
	}

	public virtual void Load( ) { }
	public virtual void Unload( ) { }

	public virtual void Run( ) { }
	public virtual void Draw( ) { }
	public virtual void Update( ) { }
	public virtual void Tick( float delta ) { }
	
	
	protected virtual void OnAppMemoryUsageIncreased( object? sender, object e ) { }
	protected virtual void OnAppMemoryUsageDecreased( object? sender, object e ) { }
	protected virtual void OnAppMemoryUsageLimitChange( object? sender, object e ) { }
	
#region Dispose Pattern
	protected virtual void OnDestroyManaged( ) { }
	protected virtual void OnDestroyUnmanaged( ) { }
	
	void Dispose( bool disposing ) {
		OnDestroyUnmanaged( ) ;
		if ( disposing ) OnDestroyManaged( ) ;
	}
	async ValueTask DisposeAsyncCore( ) => await Task.Run( OnDestroyUnmanaged );

	public void Dispose( ) {
		Dispose( true ) ;
		GC.SuppressFinalize( this ) ;
	}
	public async ValueTask DisposeAsync( ) {
		await DisposeAsyncCore( ) ;
		GC.SuppressFinalize( this ) ;
	}
	~DXAppBase( ) => Dispose( false ) ;
#endregion
	
} ;
