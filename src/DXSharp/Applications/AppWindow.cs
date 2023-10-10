namespace DXSharp.Applications ;

public interface IAppWindow {
	bool IsChild { get ; }
	bool IsVisible { get ; }
	bool IsMinimized { get ; }
	bool IsMaximized { get ; }
	
	void Show( ) ;
	void Hide( ) ;
	void Close( ) ;
	void Minimize( ) ;
	void Maximize( ) ;
} ;

public class AppWindow: IAppWindow {
	public bool IsChild { get ; }
	public bool IsVisible { get ; }
	public bool IsMinimized { get ; }
	public bool IsMaximized { get ; }
	
	public virtual void Show( ) { }
	public virtual void Hide( ) { }
	public virtual void Close( ) { }
	public virtual void Minimize( ) { }
	public virtual void Maximize( ) { }
}