#region Using Directives
using System.Runtime.InteropServices ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Applications ;


/// <summary>A basic contract for a DXSharp application.</summary>
public interface IDXApp: IDisposable,
						 IAsyncDisposable {
	string Title { get; }
	bool IsPaused { get; }
	bool IsRunning { get; }
	Size DesiredSize { get; }
	Size CurrentSize { get; }
	IAppWindow? Window { get; }
	AppSettings? Settings { get; }
	ITimeProvider? GameTime { get ; }
	bool IsInitialized { get ; }
	
	void Initialize( ) ;
	void Shutdown( ) ;
	
	void Load( ) ;
	void Unload( ) ;
	
	void Run( ) ;
	void Draw( ) ;
	void Update( ) ;
	void Tick( float delta ) ;
} ;

/// <summary>A basic contract for a DXSharp WinForms-based application.</summary>
public interface IDXWinformApp: IDXApp { Form? MainForm { get; } } ;

