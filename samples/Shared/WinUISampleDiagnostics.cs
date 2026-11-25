using System.Runtime.ExceptionServices;

namespace DXSharp.Samples;

// Shared by the unpackaged WinUI samples; no additional desktop framework is loaded.
internal static class WinUISampleDiagnostics {
    private static readonly object Gate = new( );
    private static string? _path;
    private static string _stage = "bootstrap";
    private static int _firstChanceCount;
    [ThreadStatic] private static bool _writing;
    private static Timer? _watchdog;

    internal static void Initialize( string name, string[] arguments ) {
        try {
            string directory = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "DXSharp", "logs" );
            Directory.CreateDirectory( directory );
            _path = Path.Combine( directory, $"{name}-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Environment.ProcessId}.log" );
            Console.WriteLine( $"Diagnostic log: {_path}" );
        } catch ( Exception error ) { Console.Error.WriteLine( $"Cannot create diagnostic log: {error.Message}" ); }
        Stage( $"bootstrap; .NET {Environment.Version}; OS {Environment.OSVersion}; {RuntimeInformation()}" );
        AppDomain.CurrentDomain.UnhandledException += ( _, args ) => Error( args.ExceptionObject as Exception ?? new Exception( args.ExceptionObject.ToString( ) ) );
        if ( arguments.Contains( "--diagnostics" ) ) AppDomain.CurrentDomain.FirstChanceException += FirstChance;
        if ( arguments.Contains( "--host-test" ) || arguments.Contains( "--test" ) ) {
            _watchdog = new Timer( _ => {
                Write( "FAIL: qualification exceeded 120 seconds; UI/native call may be blocked." );
                Environment.Exit( 124 );
            }, null, TimeSpan.FromSeconds( 120 ), Timeout.InfiniteTimeSpan );
        }
    }
    private static string RuntimeInformation( ) => System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString( );
    internal static void Stage( string stage ) { Volatile.Write( ref _stage, stage ); Write( stage ); }
    internal static void Error( Exception error, string? context = null ) => Write( $"FAIL at {Volatile.Read( ref _stage )}; HRESULT 0x{error.HResult:X8}; {context}\n{error}" );
    internal static void Shutdown( ) { _watchdog?.Dispose( ); _watchdog = null; }
    private static void FirstChance( object? sender, FirstChanceExceptionEventArgs args ) {
        if ( Interlocked.Increment( ref _firstChanceCount ) <= 200 ) Write( $"First-chance HRESULT 0x{args.Exception.HResult:X8}: {args.Exception}" );
    }
    private static void Write( string message ) {
        if ( _writing ) return;
        _writing = true;
        try {
            string line = $"{DateTime.UtcNow:O} [thread {Environment.CurrentManagedThreadId}] {message}";
            try { Console.Error.WriteLine( line ); } catch ( IOException ) { }
            lock ( Gate ) {
                if ( _path is null ) return;
                try { File.AppendAllText( _path, line + Environment.NewLine ); }
                catch ( IOException ) { _path = null; }
                catch ( UnauthorizedAccessException ) { _path = null; }
            }
        } finally { _writing = false; }
    }
}
