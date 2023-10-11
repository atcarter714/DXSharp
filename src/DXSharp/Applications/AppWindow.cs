namespace DXSharp.Applications ;

public interface IAppWindow: IWin32Window {
	Size Size { get ; }
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
	
	void SetSize( in Size newSize ) ;
	void SetPosition( in Point newLocation ) ;
	void SetTitle( in string newTitle ) ;
} ;


// may be pointless, but I'm not sure yet ...
public abstract class AppWindow: IAppWindow {
	public abstract nint Handle { get ; }
	public abstract Size Size { get ; }
	public abstract string Title { get ; }
	public abstract bool IsChild { get ; }
	public abstract bool IsVisible { get ; }
	public abstract bool IsMinimized { get ; }
	public abstract bool IsMaximized { get ; }

	public abstract void Show( ) ;
	public abstract void Hide( ) ;
	public abstract void Close( ) ;
	public abstract void Minimize( ) ;
	public abstract void Maximize( ) ;

	public abstract void SetSize( in Size newSize ) ;
	public abstract void SetPosition( in Point newLocation ) ;
	public abstract void SetTitle( in string newTitle ) ;
} ;
