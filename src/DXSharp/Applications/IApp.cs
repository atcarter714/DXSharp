#region Using Directives
using System.Runtime.InteropServices ;
using DXSharp.Windows.Win32 ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Applications ;


/// <summary>The basic contract for a DXSharp .NET Core application.</summary>
public interface IDXApp: IDisposable,
						 IAsyncDisposable {
	/// <summary>Title or caption text for the application.</summary>
	string Title { get; }
	/// <summary>Indicates if the application loop is paused.</summary>
	bool IsPaused { get; }
	/// <summary>Indicates if the application loop is actively running.</summary>
	bool IsRunning { get; }
	/// <summary>Gets the desired size the application wants to scale the window to.</summary>
	Size DesiredSize { get; }
	/// <summary>Gets the current application window/presentation size (in terms of client size).</summary>
	Size CurrentSize { get; }
	/// <summary>Gets an abstract reference to the main window for the application.</summary>
	IAppWindow? Window { get; }
	/// <summary>Gets the application's startup settings.</summary>
	AppSettings Settings { get; }
	/// <summary>Gets the application's time service provider.</summary>
	ITimeProvider? Time { get ; }
	/// <summary>Indicates if the application has been initialized.</summary>
	bool IsInitialized { get ; }

	bool CanDraw { get ; }
	bool CanTick { get ; }
	bool IsDrawingPaused { get ; }
	bool IsSimulationPaused { get ; }

	/// <summary>Initializes the application.</summary>
	/// <remarks>Called at startup.</remarks>
	IDXApp Initialize( ) ;
	/// <summary>Terminates the application.</summary>
	/// <remarks>Called upon window close or app exit event or request.</remarks>
	void Shutdown( ) ;
	
	/// <summary>Allows the application to load the pipeline and any content or assets.</summary>
	/// <remarks>
	/// <b>NOTE:</b> This method is called at least once at application startup. However,
	/// it could be called by the application at any time to reload the pipeline and/or
	/// load new content/assets into memory.
	/// </remarks>
	void Load( ) ;
	/// <summary>Allows the application to free loaded resources/memory and perform cleanup.</summary>
	/// <remarks>Called at least once when the application is shutting down.</remarks>
	void Unload( ) ;
	
	/// <summary>Starts the application (and time service provider) and runs the main Windows message loop.</summary>
	/// <remarks>
	/// This method is typically invoked near the application's entry point (e.g., in a "<i>Program.cs</i>" file).
	/// When called, it will start the internal Windows message loop, time service providers and initialization logic.
	/// </remarks>
	void Run( ) ;
	/// <summary>Allows the application to perform any of its rendering/drawing logic.</summary>
	/// <remarks>
	/// Invoked at least once per frame. However, the application can invoke this method whenever
	/// and however it wishes to perform custom rendering/drawing logic.
	/// </remarks>
	void Draw( ) ;
	/// <summary>Allows the application to perform any "simulation" or time-dependent "update" logic.</summary>
	/// <remarks>
	/// Invoked at least once per frame to tick the application's time service provider. However, the
	/// application can invoke this method whenever and however it wishes to perform custom simulation.
	/// If the application chooses to invoke <see cref="Update"/> more than once per frame, however, it
	/// does <i>not</i> need to invoke <c>base.Update( )</c> multiple times (only once is recommended).
	/// </remarks>
	void Update( ) ;
	/// <summary>Allows the application to perform any of its (synchronous or parallel) simulation or time-dependent "update" logic.</summary>
	/// <param name="delta">The precise amount of <b><c>delta</c></b> (elapsed time, in seconds) that has elapsed since the last frame end event.</param>
	/// <remarks>
	/// Invoked at least once per frame to tick the application's time service provider. However, the <see cref="Tick"/> method is a flexible
	/// mechanism for the application to perform any of its simulation or time-dependent "update" logic in a synchronous or parallel manner.
	/// </remarks>
	void Tick( float delta ) ;
} ;


/// <summary>A basic contract for a DXSharp WinForms-based application.</summary>
public interface IDXWinformApp: IDXApp { Form? MainForm { get; } } ;


//! TODO: Figure out how we can support all of these:
public interface IDXWPFApp: IDXApp {
	//System.Windows.Window? MainWindow { get; }
} ;
public interface IDXUWPApp: IDXApp {
	//Windows.UI.Xaml.Window? MainWindow { get; }
} ;
public interface IDXWinRTApp: IDXApp {
	//Windows.UI.Xaml.Window? MainWindow { get; }
} ;
public interface IDXWin32App: IDXApp {
	//HWnd MainWindow { get; }
} ;
public interface IDXWinUIApp: IDXWin32App {
	//Windows.UI.Xaml.Window? MainWindow { get; }
} ;

/* -------------------------------------------------------
 * NOTES:
 * -------------------------------------------------------
 * The additional UI platforms will require some additional dependencies.
 * The project will need to reference the following NuGet packages:
 * - Microsoft.Windows.SDK.Contracts
 * - Microsoft.Windows.SDK.Contracts.WPF
 * - Microsoft.Windows.SDK.Contracts.WinRT
 * - Microsoft.Windows.SDK.Contracts.WinUI
 * -------------------------------------------------------
 */