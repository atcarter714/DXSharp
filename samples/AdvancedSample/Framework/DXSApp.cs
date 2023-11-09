using DXSharp.Applications ;

namespace AdvancedDXS.Framework ;


public class DXSApp: DXWinformApp {
	public static DXSApp? Instance { get; private set ; }
	GraphicsSettings  _graphicsSettings ;
	GraphicsPipeline? graphics ;

	public DXSApp( AppSettings settings ): base( settings ) {
		if( Instance is not null )
			throw new InvalidOperationException( "Only one instance of DXSApp is allowed" ) ;
		
		Instance          = this ;
		_graphicsSettings = new( 2 ) ;
	}

	public override void Initialize( ) {
		base.Initialize( ) ;
		graphics = new( _graphicsSettings ) ;
		graphics.LoadPipeline( ) ;
		graphics.LoadAssets( ) ;
	}
}