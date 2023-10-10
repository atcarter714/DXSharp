using System.Runtime.InteropServices ;
using DXSharp.DXGI ;

namespace DXSharp.Applications ;

/// <summary>A basic contract for a DXSharp application.</summary>
public interface IDXApp: IDisposable,
						 IAsyncDisposable {
	string Title { get; }
	bool IsRunning { get; }
	Size DesiredSize { get; }
	Size CurrentSize { get; }
	
	void Initialize( ) ;
	void Shutdown( ) ;
	
	void Load( ) ;
	void Unload( ) ;
	
	void Tick( float delta ) ;
	void Draw( ) ;
	void Run( ) ;
} ;

/// <summary>A basic contract for a DXSharp WinForms-based application.</summary>
public interface IDXWinformApp: IDXApp { Form? Window { get; } } ;


public class initclass {
	void initd3d() {
		// create a factory:
		var factory = Factory.Create( ) ;
		factory.EnumAdapters< Adapter >( 0, out var adapter ) ;
	}
}