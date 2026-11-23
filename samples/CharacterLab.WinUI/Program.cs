using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.XamlTypeInfo;
using DXSharp.Graphics;

namespace DXSharp.CharacterLab.WinUI;

internal static class Program {
    internal static int ExitCode;
    [STAThread]
    private static int Main( string[] args ) {
        try {
            Console.WriteLine( "Host: WinUI 3 composition; unpackaged apphost." );
            WinRT.ComWrappersSupport.InitializeComWrappers( );
            Application.Start( parameters => {
                SynchronizationContext.SetSynchronizationContext( new DispatcherQueueSynchronizationContext( DispatcherQueue.GetForCurrentThread( ) ) );
                _ = new CharacterApplication( args );
            } );
            return ExitCode;
        } catch ( Exception error ) { Console.Error.WriteLine( error ); return 1; }
    }
}

internal sealed class CharacterApplication : Application, IXamlMetadataProvider {
    private XamlControlsXamlMetaDataProvider? _metadata;
    private XamlControlsXamlMetaDataProvider Metadata => _metadata ??= new( );
    IXamlType IXamlMetadataProvider.GetXamlType( Type type ) => Metadata.GetXamlType( type );
    IXamlType IXamlMetadataProvider.GetXamlType( string name ) => Metadata.GetXamlType( name );
    XmlnsDefinition[] IXamlMetadataProvider.GetXmlnsDefinitions( ) => Metadata.GetXmlnsDefinitions( );
    private readonly string[] _arguments;
    private CharacterWindow? _window;
    internal CharacterApplication( string[] arguments ) {
        _arguments = arguments;
        UnhandledException += ( _, args ) => { Console.Error.WriteLine( args.Exception ); Program.ExitCode = 1; args.Handled = true; _window?.CloseOrderly( ); };
    }
    protected override async void OnLaunched( LaunchActivatedEventArgs args ) {
        GraphicsDevice? device = null;
        CharacterScene? scene = null;
        try {
            XamlControlsXamlMetaDataProvider.Initialize( );
            Resources.MergedDictionaries.Add( new Microsoft.UI.Xaml.Controls.XamlControlsResources( ) );
            int limit = int.Parse( _arguments.FirstOrDefault( a => a.StartsWith( "--frames=" ) )?[9..] ?? "0" );
            string model = _arguments.FirstOrDefault( a => a.StartsWith( "--model=" ) )?[8..] ?? System.IO.Path.Combine( AppContext.BaseDirectory, "Assets", "animatedKnightInPlateArmour.glb" );
#if DEBUG
            bool debug = !_arguments.Contains( "--no-debug" );
#else
            bool debug = false;
#endif
            device = GraphicsDevice.Create( new( ) { UseWarp = _arguments.Contains( "--warp" ), UseAgility = !_arguments.Contains( "--system-runtime" ), EnableDebugLayer = debug } );
            scene = new( device, model, limit > 0 || _arguments.Contains( "--host-test" ) );
            _window = new( device, scene, limit, _arguments.Contains( "--host-test" ) ); _window.Activate( );
            if ( _arguments.Contains( "--host-test" ) ) await WinUIHostQualification.Run( _window, device, scene );
            await _window.Completion;
            if ( _window.Failure is { } failure ) throw new InvalidOperationException( "WinUI rendering stopped.", failure );
            if ( limit > 0 && _window.Statistics.Frames != limit ) throw new InvalidOperationException( "WinUI frame limit was not reached." );
            var diagnostics = device.GetDiagnostics( );
            foreach ( var message in diagnostics.Messages ) Console.WriteLine( message );
            if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( m => m.Contains( "SEVERITY_ERROR" ) || m.Contains( "SEVERITY_CORRUPTION" ) ) ) Program.ExitCode = 1;
            if ( AppDomain.CurrentDomain.GetAssemblies( ).Any( a => a.GetName( ).Name is "System.Windows.Forms" or "PresentationFramework" ) )
                throw new InvalidOperationException( "WinUI sample loaded another desktop UI framework." );
            Console.WriteLine( $"PASS: WinUI CharacterLab presented {scene.Frames} GPU-skinned frames through composition; mean CPU recording {scene.MeanRecordingMilliseconds:F2} ms." );
        } catch ( Exception error ) { Console.Error.WriteLine( error ); Program.ExitCode = 1; }
        finally {
            if ( _window is not null ) await _window.StopAsync( );
            scene?.Dispose( ); device?.Dispose( ); Exit( );
        }
    }
}
