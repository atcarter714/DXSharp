namespace DXSharp.Applications ;

/// <summary>A basic contract for a DXSharp application.</summary>
public interface IDXApp: IDisposable,
						 IAsyncDisposable {
	string Title { get; }
	Size DesiredSize { get; }
	Size CurrentSize { get; }
	
	void Initialize( ) ;
	void Shutdown( ) ;
	
	void Load( ) ;
	void Unload( ) ;
	
	void Tick( float delta ) ;
	void Draw( ) ;
} ;


/// <summary>A basic contract for a DXSharp WinForms-based application.</summary>
public interface IDXWinformApp: IDXApp {
	Form? Window { get; }
} ;