#region Using Directives

using System.Drawing ;
using System.Windows.Forms ;
using DXSharp.Direct3D12 ;
using DXSharp.Applications ;
#endregion
namespace BasicSample ;


public class BasicApp: DXWinformApp {
	IDebug?   dbgLayer ;
	Graphics? graphics ;

	public BasicApp( AppSettings? settings = null ): base( settings ) { }
	
	public override BasicApp Initialize( ) {
		Application.SetHighDpiMode( HighDpiMode.PerMonitorV2 ) ;
		base.Initialize( ) ;
		Window!.SetTitle( Settings.Title) ;
		Window?.SetSize( Settings.WindowSize ) ;
		
#if DEBUG
		// Enable debug layer in debug builds:
		var hr = D3D12.GetDebugInterface( out dbgLayer ) ;
		hr.ThrowOnFailure( ) ;
		dbgLayer?.EnableDebugLayer( ) ;
#endif
		
		graphics = new( this ) ;
		graphics.LoadPipeline( ) ;
		graphics.LoadAssets( ) ;
		return this ;
	}

	public override void Draw( ) {
		base.Draw( ) ;
		graphics!.OnRender( ) ;
	}
	
} ;