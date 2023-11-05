using System.Drawing ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Applications ;
using DXSharp.Direct3D12 ;
using DXSharp.Direct3D12.Debug ;
using HelloTexture ;


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
IDebug6? debug6 = default ;
var hr = D3D12.GetDebugInterface( out debug6 ) ;
hr.ThrowOnFailure( ) ;
ObjectDisposedException.ThrowIf( debug6 is null, typeof( IDebug6 ) ) ;

IDebug3 debug3 = (IDebug3)debug6 ;
debug3?.EnableDebugLayer( ) ;
debug3?.SetEnableGPUBasedValidation( true ) ;
#endif

using IDXApp app = new BasicApp( Settings ) ;
app.Initialize( ) ;
app.Run( ) ;

#if DEBUG || DEBUG_COM
//debug3.DisposeAsync( ) ;
debug3?.Dispose( ) ;
System.Diagnostics.Debug.WriteLine( $"Disposed" ) ;
#endif

