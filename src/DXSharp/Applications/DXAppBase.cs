
using DXSharp.Applications ;

/// <summary>Abstract base class for DXSharp applications.</summary>
public abstract class DXAppBase: IDXApp {
	public virtual string Title {
		get => Window?.Title ?? string.Empty ; 
		protected set => Window?.SetTitle( value ?? string.Empty ) ;
	}
	public virtual bool IsPaused { get; protected set ; }
	public virtual bool IsRunning { get ; protected set ; }
	public Size CurrentSize => ( Window?.Size ?? Size.Empty ) ;
	public Size DesiredSize { get ; protected set ; }
	
	public abstract IAppWindow? Window { get ; }
	public abstract ITimeProvider? GameTime { get ; }
	public virtual AppSettings Settings => AppSettings.Default ;
	public virtual bool IsInitialized { get ; protected set ; }
	
	// --------------------------------------
	// Instance Methods ::
	// --------------------------------------
	
	protected abstract void OnAppQuit( object? sender, EventArgs e ) ;
	
	public virtual void Initialize( ) {
		DesiredSize = Settings?.WindowSize ?? AppSettings.DEFAULT_WINDOW_SIZE ;
		Title = Settings?.Title ?? AppSettings.DEFAULT_APP_NAME ;
		Application.ApplicationExit += OnAppQuit ;
		this.IsInitialized = true ;
	}

	public virtual void Shutdown( ) {
		Application.ApplicationExit -= OnAppQuit ;
		this.IsInitialized = false ;
		this.IsRunning     = false ;
		GameTime?.Stop( ) ;
		
		if ( Window is not null ) { Window.Close( ) ; }
	}

	public virtual void Load( ) { }
	public virtual void Unload( ) { }

	public virtual void Run( ) { }
	public virtual void Draw( ) { }
	public virtual void Update( ) { }
	public virtual void Tick( float delta ) { }
	
	
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
