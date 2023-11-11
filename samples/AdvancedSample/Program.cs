#region Using Directives
using System.Drawing ;
using DXSharp.Direct3D12 ;
using DXSharp.Applications ;
using AdvancedDXS.Framework ;
#endregion

//! Create the app startup settings:
AppSettings Settings = new( "DXSharp: Advanced Sample",
							(1920 , 1080),
							new AppSettings.Style {
								FontSize        = 13,
								FontName        = "Consolas",
								BackBufferColor = Color.Black,
								ForegroundColor = Color.Black,
								BackgroundColor = SystemColors.Window,
							} ) ;


//! Enable the debug layer in debug mode:
#if DEBUG || DEBUG_COM
IDebug6? debug6 = default ;
var hr   = D3D12.GetDebugInterface( out debug6 ) ;
ObjectDisposedException.ThrowIf( debug6 is null, typeof( IDebug6 ) ) ;
hr.ThrowOnFailure( ) ;

debug6.EnableDebugLayer( ) ;
debug6.SetEnableAutoName( true ) ;
debug6.SetEnableGPUBasedValidation( true ) ;
#if !GPU_VALIDATE_DESC_ONLY
debug6.SetGPUBasedValidationFlags( GPUBasedValidationFlags.None ) ;
#else
debug6.SetGPUBasedValidationFlags( GPUBasedValidationFlags.DisableStateTracking ) ;
#endif
#endif


// Initialize & run the app:
using IDXApp app = new DXSApp( Settings ) ;
app.Initialize( ) ;
app.Run( ) ;


//! Destroy the debug layer in debug mode:
#if DEBUG || DEBUG_COM
debug6.DisableDebugLayer( ) ;
debug6.DisposeAsync( ) ;
#endif