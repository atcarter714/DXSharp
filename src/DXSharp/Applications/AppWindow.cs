namespace DXSharp.Applications ;


/// <summary>
/// The base interface contract for DXSharp application windows.
/// The application framework will use this interface to interact with the window,
/// rather than a concrete implementation, so that the application can be agnostic
/// from the underlying windowing system and can support multiple windowing systems
/// (e.g., WinForms, WPF, UWP, WinUI, Avalonia, Win32, etc.).
/// </summary>
public interface IAppWindow: IWin32Window {
	Size Size { get ; }
	Point Location { get ; }
	string Title { get ; }
	
	bool IsChild { get ; }
	bool IsVisible { get ; }
	bool IsMinimized { get ; }
	bool IsMaximized { get ; }
	
	void Show( ) ;
	void Hide( ) ;
	void Close( ) ;
	void Minimize( ) ;
	void Maximize( ) ;
	
	void Invalidate( ) ;
	void SetSize( in Size newSize ) ;
	void SetTitle( in string newTitle ) ;
	void SetPosition( in Point newLocation ) ;
	
	event DPIChangedEventHandler? DPIChanged ;
	event UserResizeEventHandler? UserResized ;
	event ContentsResizedEventHandler? ContentsResized ;
} ;


/// <summary>
/// The (<see langword="abstract"/>) base class for implementing
/// <b>DXSharp</b> application windows.
/// </summary>
public abstract class AppWindow: DisposableObject, IAppWindow {
	bool _destroyed ;
	public abstract nint Handle { get ; }
	public string Title { get ; }
	
	public abstract Size Size { get ; }
	public abstract Point Location { get ; }
	public abstract bool IsChild { get ; }
	public abstract bool IsVisible { get ; }
	public abstract bool IsMinimized { get ; }
	public abstract bool IsMaximized { get ; }
	public virtual bool IsDisposed => (this.Handle == nint.Zero) ;
	
	~AppWindow( ) {
		this.Dispose( false ) ;
	}
	protected AppWindow( ) => this.Title = AppSettings.DEFAULT_APP_NAME ;
	protected AppWindow( string captionText ) => this.Title = captionText ;

	public abstract void Show( ) ;
	public abstract void Hide( ) ;
	public abstract void Close( ) ;
	public abstract void Minimize( ) ;
	public abstract void Maximize( ) ;
	public abstract void Invalidate( ) ;
	public abstract void SetSize( in Size newSize ) ;
	public abstract void SetTitle( in string newTitle ) ;
	public abstract void SetPosition( in Point newLocation ) ;
	
	public abstract event DPIChangedEventHandler? DPIChanged ;
	public abstract event UserResizeEventHandler? UserResized ;
	public abstract event ContentsResizedEventHandler? ContentsResized ;

	protected override ValueTask DisposeUnmanaged( ) => ValueTask.CompletedTask ;
	
	protected override void Dispose( bool disposing ) {
		base.Dispose( disposing ) ;
		_destroyed = true ;
	}
} ;
