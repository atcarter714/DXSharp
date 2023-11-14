#region Using Directives
using System.Drawing ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Direct3D12.Debug ;
using DXSharp.Applications ;
using DXSharp.Direct3D12 ;

using HelloTexture ;
#endregion


AppSettings Settings = new( "DXSharp: Hello Texture",
							(1920, 1080),
							new AppSettings.Style {
								FontSize        = 13,
								FontName        = "Consolas",
								BackBufferColor = Color.Black,
								ForegroundColor = Color.Black,
								BackgroundColor = SystemColors.Window,
							} ) ;


//! Enable the debug layer in debug mode:
#if DEBUG || DEBUG_COM
var hr = D3D12.GetDebugInterface( out IDebug6? debug6 ) ;
hr.ThrowOnFailure( ) ;
ObjectDisposedException.ThrowIf( debug6 is null, typeof( IDebug6 ) ) ;

debug6.EnableDebugLayer( ) ;
debug6.SetEnableAutoName( true ) ;
debug6.SetEnableGPUBasedValidation( true ) ;
debug6.SetGPUBasedValidationFlags( GPUBasedValidationFlags.None ) ;
#endif


// Initialize & run the app:
await using IDXApp app = new BasicApp( Settings ) ;
app.Initialize( ) ;
app.Run( ) ;


//! Destroy the debug layer in debug mode:
#if DEBUG || DEBUG_COM
debug6.DisableDebugLayer( ) ;
await debug6.DisposeAsync( ) ;
#endif