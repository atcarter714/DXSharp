// DXSharp Basic Sample:

using System.Drawing ;
using BasicSample ;
using DXSharp.Applications ;

static class Program
{
	static AppSettings Settings = new( "DXSharp: Hello Triangle",
									   (1920, 1080),
									   new AppSettings.Style {
											FontSize = 13,
											FontName = "Consolas",
											BackBufferColor = Color.Black,
											ForegroundColor = Color.Black,
											BackgroundColor = SystemColors.Window,
									   }) ;
	
	[STAThread]
	static int Main( string[ ] args ) {
		BasicApp app = new( Settings ) ;
		app.Initialize( ) ;
		app.Run( ) ;
		return 0 ;
	}
}