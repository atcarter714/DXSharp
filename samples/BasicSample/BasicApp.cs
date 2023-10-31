#region Using Directives
using System.Windows.Forms ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp ;
using DXSharp.DXGI ;
using DXSharp.Windows ;
using DXSharp.Direct3D12 ;
using DXSharp.Applications ;
using DXSharp.Direct3D12.Debug ;
using DXSharp.DXGI.XTensions ;
using DXSharp.Windows.COM ;

using Device = DXSharp.Direct3D12.Device ;
using IDevice = DXSharp.Direct3D12.IDevice ;
using IResource = DXSharp.Direct3D12.IResource ;
using Resource = DXSharp.Direct3D12.Resource ;
#endregion
namespace BasicSample ;


public class BasicApp: DXWinformApp {
	IDebug? dbgLayer ;
	Graphics? graphics ;

	public BasicApp( AppSettings? settings = null ): base( settings ) { }
	
	public override void Initialize( ) {
		base.Initialize( ) ;
		Window!.SetTitle( Settings.Title) ;
		
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