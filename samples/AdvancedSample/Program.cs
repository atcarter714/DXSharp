﻿#region Using Directives
using System.Drawing ;

using DXSharp ;
using DXSharp.Applications ;
using AdvancedDXS.Framework ;

#if DEBUG || DEBUG_COM
using DXSharp.Framework.Debugging ;
using IDebugD3D  = DXSharp.Direct3D12.IDebug6 ;
using IDebugDXGI = DXSharp.DXGI.Debugging.IDebug1 ;
#endif
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
await using DebugSystem< IDebugD3D, IDebugDXGI > dbg = new( ) ;
dbg.Enable( ) ;
#endif



// Initialize & run the app:
var app = new DXSApp( Settings )
				.Initialize( ) ;

app.Run( ) ;
app?.Shutdown( ) ;