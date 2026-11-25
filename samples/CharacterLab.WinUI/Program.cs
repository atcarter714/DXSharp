using DXSharp.Samples;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.XamlTypeInfo;
using DXSharp.Graphics;

namespace DXSharp.CharacterLab.WinUI;

internal static class Program {
    internal static int ExitCode = 1;
    internal static bool HadUnhandledException;
    [STAThread]
    private static int Main( string[] args ) {
        WinUISampleDiagnostics.Initialize( "CharacterLab.WinUI", args );
        try {
            Console.WriteLine( "Host: WinUI 3 composition; unpackaged apphost." );
            WinRT.ComWrappersSupport.InitializeComWrappers( );
            Application.Start( parameters => {
                SynchronizationContext.SetSynchronizationContext( new DispatcherQueueSynchronizationContext( DispatcherQueue.GetForCurrentThread( ) ) );
                _ = new CharacterApplication( args );
            } );
            return ExitCode;
        } catch ( Exception error ) { WinUISampleDiagnostics.Error( error ); return 1; }
        finally { WinUISampleDiagnostics.Shutdown( ); }
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
        this.DispatcherShutdownMode = Microsoft.UI.Xaml.DispatcherShutdownMode.OnExplicitShutdown;
        UnhandledException += ( _, args ) => { WinUISampleDiagnostics.Error( args.Exception, args.Message ); Program.ExitCode = 1; Program.HadUnhandledException = true; args.Handled = true; _window?.CloseOrderly( ); };
    }
    protected override async void OnLaunched( LaunchActivatedEventArgs args ) {
        GraphicsDevice? device = null;
        CharacterScene? scene = null;
        try {
            WinUISampleDiagnostics.Stage( "initialize XAML resources" );
            XamlControlsXamlMetaDataProvider.Initialize( );
            Resources.MergedDictionaries.Add( new Microsoft.UI.Xaml.Controls.XamlControlsResources( ) );
            int limit = int.Parse( _arguments.FirstOrDefault( a => a.StartsWith( "--frames=" ) )?[9..] ?? "0" );
            string model = _arguments.FirstOrDefault( a => a.StartsWith( "--model=" ) )?[8..] ?? System.IO.Path.Combine( AppContext.BaseDirectory, "Assets", "animatedKnightInPlateArmour.glb" );
#if DEBUG
            bool debug = !_arguments.Contains( "--no-debug" );
#else
            bool debug = false;
#endif
            WinUISampleDiagnostics.Stage( "create D3D12 device" );
            device = GraphicsDevice.Create( new( ) { UseWarp = _arguments.Contains( "--warp" ), UseAgility = !_arguments.Contains( "--system-runtime" ), EnableDebugLayer = debug } );
            WinUISampleDiagnostics.Stage( "create character scene" );
            scene = new( device, model, limit > 0 || _arguments.Contains( "--host-test" ) );
            WinUISampleDiagnostics.Stage( "construct window and controls" );
            _window = new( device, scene, limit, _arguments.Contains( "--host-test" ) );
            WinUISampleDiagnostics.Stage( "activate window and start presentation" ); _window.Activate( );
            if ( _arguments.Contains( "--host-test" ) ) await WinUIHostQualification.Run( _window, device, scene );
            await _window.Completion;
            if ( _window.Failure is { } failure ) throw new InvalidOperationException( "WinUI rendering stopped.", failure );
            if ( limit > 0 && _window.Statistics.Frames != limit ) throw new InvalidOperationException( "WinUI frame limit was not reached." );
            var diagnostics = device.GetDiagnostics( );
            foreach ( var message in diagnostics.Messages ) Console.WriteLine( message );
            if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( m => m.Contains( "SEVERITY_ERROR" ) || m.Contains( "SEVERITY_CORRUPTION" ) ) || Program.HadUnhandledException ) throw new InvalidOperationException( "WinUI qualification reported a device or unhandled UI failure." );
            if ( AppDomain.CurrentDomain.GetAssemblies( ).Any( a => a.GetName( ).Name is "System.Windows.Forms" or "PresentationFramework" ) )
                throw new InvalidOperationException( "WinUI sample loaded another desktop UI framework." );
            Console.WriteLine( $"PASS: WinUI CharacterLab presented {scene.Frames} GPU-skinned frames through composition; mean CPU recording {scene.MeanRecordingMilliseconds:F2} ms." );
            Program.ExitCode = 0;
        } catch ( Exception error ) { WinUISampleDiagnostics.Error( error ); Program.ExitCode = 1; }
        finally {
            WinUISampleDiagnostics.Stage( "drain viewport and dispose scene/device" );
            try {
                try { if ( _window is not null ) await _window.StopAsync( ); }
                finally { try { scene?.Dispose( ); } finally { device?.Dispose( ); } }
            } catch ( Exception error ) { WinUISampleDiagnostics.Error( error ); Program.ExitCode = 1; }
            finally { Exit( ); }
        }
    }
}
