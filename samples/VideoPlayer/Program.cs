using DXSharp.Samples;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.XamlTypeInfo;
using DXSharp.Graphics;

namespace DXSharp.VideoPlayer;

internal static class Program {
    internal static int ExitCode = 1;
    internal static bool HadUnhandledException;
    [STAThread]
    private static int Main( string[] args ) {
        WinUISampleDiagnostics.Initialize( "VideoPlayer", args );
        try {
            Console.WriteLine( "Host: DXSharp Media Foundation video player; WinUI 3 composition." );
            WinRT.ComWrappersSupport.InitializeComWrappers( );
            Application.Start( parameters => {
                SynchronizationContext.SetSynchronizationContext( new DispatcherQueueSynchronizationContext( DispatcherQueue.GetForCurrentThread( ) ) );
                _ = new VideoApplication( args );
            } );
            return ExitCode;
        } catch ( Exception error ) { WinUISampleDiagnostics.Error( error ); return 1; }
        finally { WinUISampleDiagnostics.Shutdown( ); }
    }
}

internal sealed class VideoApplication : Application, IXamlMetadataProvider {
    private XamlControlsXamlMetaDataProvider? _metadata;
    private XamlControlsXamlMetaDataProvider Metadata => _metadata ??= new( );
    IXamlType IXamlMetadataProvider.GetXamlType( Type type ) => Metadata.GetXamlType( type );
    IXamlType IXamlMetadataProvider.GetXamlType( string name ) => Metadata.GetXamlType( name );
    XmlnsDefinition[] IXamlMetadataProvider.GetXmlnsDefinitions( ) => Metadata.GetXmlnsDefinitions( );
    private readonly string[] _arguments;
    private PlayerWindow? _window;
    internal VideoApplication( string[] arguments ) {
        _arguments = arguments;
        this.DispatcherShutdownMode = Microsoft.UI.Xaml.DispatcherShutdownMode.OnExplicitShutdown;
        UnhandledException += ( _, args ) => { WinUISampleDiagnostics.Error( args.Exception, args.Message ); Program.ExitCode = 1; Program.HadUnhandledException = true; args.Handled = true; _window?.CloseOrderly( ); };
    }
    protected override async void OnLaunched( LaunchActivatedEventArgs args ) {
        GraphicsDevice? device = null;
        DXSharp.Media.MediaPlayer? scene = null;
        try {
            WinUISampleDiagnostics.Stage( "initialize XAML resources" );
            XamlControlsXamlMetaDataProvider.Initialize( );
            Resources.MergedDictionaries.Add( new Microsoft.UI.Xaml.Controls.XamlControlsResources( ) );
            bool testing = _arguments.Contains( "--test" );
            int limit = int.Parse( _arguments.FirstOrDefault( a => a.StartsWith( "--frames=" ) )?[9..] ?? "0" );
            string source = _arguments.FirstOrDefault( a => a.StartsWith( "--file=" ) )?[7..] ?? System.IO.Path.Combine( AppContext.BaseDirectory, "Assets", testing || _arguments.Contains( "--silent-fixture" ) ? "test-video.mp4" : "test-av.mp4" );
#if DEBUG
            bool debug = !_arguments.Contains( "--no-debug" );
#else
            bool debug = false;
#endif
            WinUISampleDiagnostics.Stage( "create D3D12 device" );
            device = GraphicsDevice.Create( new( ) { UseWarp = _arguments.Contains( "--warp" ), UseAgility = !_arguments.Contains( "--system-runtime" ), EnableDebugLayer = debug } );
            WinUISampleDiagnostics.Stage( "create media engine" );
            scene = new( );
            WinUISampleDiagnostics.Stage( "construct window and controls" );
            _window = new( device, scene, source, limit );
            WinUISampleDiagnostics.Stage( "activate window and start presentation" ); _window.Activate( );
            if ( testing ) await PlaybackQualification.Run( _window, source );
            await _window.Completion;
            if ( _window.Failure is { } failure ) throw new InvalidOperationException( "Video rendering stopped.", failure );
            if ( limit > 0 && _window.Statistics.Frames < limit ) throw new InvalidOperationException( "Video frame limit was not reached." );
            var diagnostics = device.GetDiagnostics( );
            foreach ( var message in diagnostics.Messages ) Console.WriteLine( message );
            if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( m => m.Contains( "SEVERITY_ERROR" ) || m.Contains( "SEVERITY_CORRUPTION" ) ) || Program.HadUnhandledException ) throw new InvalidOperationException( "WinUI qualification reported a device or unhandled UI failure." );
            if ( AppDomain.CurrentDomain.GetAssemblies( ).Any( a => a.GetName( ).Name is "System.Windows.Forms" or "PresentationFramework" ) )
                throw new InvalidOperationException( "WinUI sample loaded another desktop UI framework." );
            Console.WriteLine( $"PASS: VideoPlayer presented {_window.Statistics.Frames} frames; media frames transferred {_window.VideoFrames}." );
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
