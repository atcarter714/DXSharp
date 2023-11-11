#region Using Directives
using Windows.System ;
using DXSharp.Applications ;
using AdvancedDXS.Framework.Graphics ;
#endregion
namespace AdvancedDXS.Framework ;


public class DXSApp: DXWinformApp {
	// -----------------------------------------------------------------------------------------
	public static DXSApp? Instance { get; private set ; }
	GraphicsSettings  _graphicsSettings ;
	GraphicsPipeline? graphics ;
	// -----------------------------------------------------------------------------------------
	
	public DXSApp( AppSettings settings ): base( settings ) {
		if( Instance is not null )
			throw new InvalidOperationException( "Only one instance of DXSApp is allowed" ) ;
		
		Instance          = this ;
		_graphicsSettings = new( 2 ) ;
	}

	
	
	// -----------------------------------------------------------------------------------------
	
	public override void Initialize( ) {
		base.Initialize( ) ;
		
		Window?.SetSize( Settings.WindowSize ) ;
		graphics = new( _graphicsSettings ) ;
		
		graphics.LoadAssets( ) ;
	}

	public override void Draw( ) {
		base.Draw( ) ;
	}
	public override void Load( ) {
		if( graphics is null )
			throw new InvalidOperationException( "Graphics is null/uninitialized!" ) ;
		
		base.Load( ) ;
		graphics.LoadPipeline( ) ;
	}


	// -----------------------------------------------------------------------------------------
	// =========================================================================================
} ;