using System.Drawing ;
using DXSharp.Applications ;
using DXSharp.Direct3D12.Debug ;


AppSettings Settings = new( "DXSharp: Hello Texture",
							(1280, 720),
							new AppSettings.Style {
								FontSize        = 13,
								FontName        = "Consolas",
								BackBufferColor = Color.Black,
								ForegroundColor = Color.Black,
								BackgroundColor = SystemColors.Window,
							} ) ;

#if DEBUG || DEBUG_COM
var debug = Debug.CreateDebugLayer( ) ;
debug.EnableDebugLayer( ) ;
#endif

IDXApp app = new DXWinformApp( Settings ) ;
app.Initialize( ) ;
app.Run( ) ;