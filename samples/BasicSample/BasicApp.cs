#region Using Directives
using DXSharp.Direct3D12 ;
using DXSharp.Applications ;
#endregion
namespace BasicSample ;


public class BasicApp: DXWinformApp {
	IDebug? dbgLayer ;
	Graphics? graphics ;

	public BasicApp( AppSettings? settings = null ): base( settings ) { }
	
	public override void Initialize( ) {
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
	}

	public override void Draw( ) {
		base.Draw( ) ;
		graphics!.OnRender( ) ;
	}
	
} ;