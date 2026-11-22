using System.IO;
using System.Windows;
using DXSharp.Graphics;

namespace DXSharp.CharacterLab.Wpf;

internal static class Program {
    [STAThread]
    private static int Main( string[] args ) {
        try {
            int limit = int.Parse( args.FirstOrDefault( a => a.StartsWith( "--frames=" ) )?[9..] ?? "0" );
            string model = args.FirstOrDefault( a => a.StartsWith( "--model=" ) )?[8..] ?? Path.Combine( AppContext.BaseDirectory, "Assets", "animatedKnightInPlateArmour.glb" );
#if DEBUG
            bool debug = !args.Contains( "--no-debug" );
#else
            bool debug = false;
#endif
            using var device = GraphicsDevice.Create( new( ) { UseWarp = args.Contains( "--warp" ), UseAgility = !args.Contains( "--system-runtime" ), EnableDebugLayer = debug } );
            using var scene = new CharacterScene( device, model, limit > 0 || args.Contains( "--host-test" ) );
            int exit;
            if ( args.Contains( "--host-test" ) ) exit = WpfHostQualification.Run( device, scene );
            else {
                var application = new Application( );
                var window = new CharacterWindow( device, scene, limit );
                application.Run( window );
                if ( window.Failure is { } failure ) throw new InvalidOperationException( "WPF rendering stopped.", failure );
                if ( limit > 0 && window.Statistics.Frames != limit ) throw new InvalidOperationException( "WPF frame limit was not reached." );
                Console.WriteLine( $"PASS: WPF CharacterLab presented {window.Statistics.Frames} GPU-skinned frames; mean CPU recording {scene.MeanRecordingMilliseconds:F2} ms." );
                exit = 0;
            }
            var diagnostics = device.GetDiagnostics( );
            foreach ( var message in diagnostics.Messages ) Console.WriteLine( message );
            if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( x => x.Contains( "SEVERITY_ERROR" ) || x.Contains( "SEVERITY_CORRUPTION" ) ) ) return 1;
            if ( AppDomain.CurrentDomain.GetAssemblies( ).Any( a => a.GetName( ).Name == "System.Windows.Forms" ) ) throw new InvalidOperationException( "WPF sample loaded Windows Forms." );
            return exit;
        } catch ( Exception error ) { Console.Error.WriteLine( error ); return 1; }
    }
}
