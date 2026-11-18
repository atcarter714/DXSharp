using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace BasicSample;

internal static class Program {
    [STAThread]
    private static int Main( string[] args ) {
        try {
            Application.SetUnhandledExceptionMode( UnhandledExceptionMode.ThrowException );
            ApplicationConfiguration.Initialize( );
            int frames = int.Parse( args.FirstOrDefault( value => value.StartsWith( "--frames=", StringComparison.Ordinal ) )?[9..] ?? "0" );
#if DEBUG
            const bool debug = true;
#else
            const bool debug = false;
#endif
            using var device = GraphicsDevice.Create( new( ) {
                UseWarp = args.Contains( "--warp" ), UseAgility = !args.Contains( "--system-runtime" ),
                EnableDebugLayer = (debug || args.Contains( "--debug" )) && !args.Contains( "--no-debug" )
            } );
            Console.WriteLine( device.AdapterName );
            using var compiler = new ShaderCompiler( );
            string shader = File.ReadAllText( Path.Combine( AppContext.BaseDirectory, "shader1.hlsl" ) );
            using var root = device.CreateRootSignature( );
            using var pipeline = device.CreateGraphicsPipeline( root,
                compiler.Compile( shader, "VS", "vs_6_0" ).RequireSuccess( ),
                compiler.Compile( shader, "PS", "ps_6_0" ).RequireSuccess( ) );
            var result = DesktopGame.Run( device, new( ) {
                Title = "DXSharp - Hello Triangle", FrameLimit = frames,
                FramesInFlight = args.Contains( "--buffered" ) ? 2 : 1
            }, frame => {
                frame.Commands.SetGraphicsPipeline( pipeline );
                frame.Commands.Draw( 3 );
            } );
            var diagnostics = device.GetDiagnostics( );
            foreach ( string message in diagnostics.Messages ) Console.WriteLine( message );
            if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( message =>
                message.Contains( "SEVERITY_ERROR" ) || message.Contains( "SEVERITY_CORRUPTION" ) ) ) return 1;
            if ( frames > 0 && result.Frames != frames ) return 1;
            Console.WriteLine( $"BasicSample closed cleanly after {result.Frames} frames." );
            return 0;
        } catch ( Exception error ) { Console.Error.WriteLine( error ); return 1; }
    }
}
