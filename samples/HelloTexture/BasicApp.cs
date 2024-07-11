using System.Windows.Forms ;
using DXSharp.Applications ;
namespace HelloTexture ;


public class BasicApp: DXWinformApp {
	public static BasicApp? Instance { get; private set ; }
	GraphicsSettings _graphicsSettings ;
	GraphicsPipeline? graphics ;
	
	public BasicApp( AppSettings settings ): base( settings ) {
		if( Instance is not null )
			throw new InvalidOperationException( "Only one instance of BasicApp is allowed" ) ;
		
		Instance = this ;
		_graphicsSettings = new( 2 ) ;
	}
	
	public override BasicApp Initialize( ) {
		base.Initialize( ) ;
		Window!.SetSize( Settings.WindowSize ) ;
		MainForm!.StartPosition = FormStartPosition.CenterScreen ;
		
		graphics = new( _graphicsSettings ) ;
		graphics.LoadPipeline( ) ;
		graphics.LoadAssets( ) ;
		return this ;
	}
} ;