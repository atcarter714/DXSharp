#region Using Directives
using System.Drawing ;
using DXSharp.Applications ;
using DXSharp.Direct3D12 ;
using HelloTexture ;
#endregion


Console.WriteLine("\"Hello Texture\" sample started ...") ;
AppSettings Settings = new( "DXSharp: Hello Texture",
							(1920, 1080),
							new AppSettings.Style {
								FontSize        = 13,
								FontName        = "Consolas",
								BackBufferColor = Color.Black,
								ForegroundColor = Color.Black,
								BackgroundColor = SystemColors.Window,
							} ) ;



#if DEBUG || DEBUG_COM
//! Debug Settings --------------------------------------------------------------------
const GPUBasedValidationFlags gpuBasedValidationFlags  = GPUBasedValidationFlags.None ;
const bool                    EnableGPUBasedValidation = true ;
const bool                    EnableAutoName           = true ;
// ------------------------------------------------------------------------------------
var debug6 = DebugHelper.Init( EnableGPUBasedValidation, 
							   gpuBasedValidationFlags, 
							   EnableAutoName ) ;
// ------------------------------------------------------------------------------------
#endif


// Initialize & run the app:
await using IDXApp app = new BasicApp( Settings ) ;
app.Initialize( ) ;
app.Run( ) ;
app.Shutdown( ) ;


#if DEBUG || DEBUG_COM
//! Destroy the debug layer in debug mode:
debug6.DisableDebugLayer( ) ;
await debug6.DisposeAsync( ) ;
#endif