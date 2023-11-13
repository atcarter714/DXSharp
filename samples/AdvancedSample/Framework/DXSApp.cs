#region Using Directives
using Windows.System ;
using DXSharp.Applications ;
using AdvancedDXS.Framework.Graphics ;
using DXSharp.Framework.Graphics ;

#endregion
namespace AdvancedDXS.Framework ;


public class DXSApp: DXWinformApp {
	// -----------------------------------------------------------------------------------------
	/// <summary>Gets the singleton instance of the <see cref="DXSApp"/> class.</summary>
	public static DXSApp? Instance { get; private set ; }
	GraphicsSettings  _graphicsSettings ;
	GraphicsPipeline? graphics ;
	// -----------------------------------------------------------------------------------------
	
	public DXSApp( AppSettings settings,
				   GraphicsSettings? graphicsSettings = null ): base( settings ) {
		if( Instance is not null )
			throw new InvalidOperationException( "Only one instance of DXSApp is allowed" ) ;
		else Instance = this ;
		
		this._graphicsSettings = graphicsSettings
								  ?? GraphicsSettings.Default ;
	}
	
	
	// -----------------------------------------------------------------------------------------
	
	public override DXSApp Initialize( ) {
		base.Initialize( ) ;
		//Window?.SetSize( Settings.WindowSize ) ;
		
		graphics = new( _graphicsSettings ) ;
		graphics.LoadAssets( ) ;
		return this ;
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