using System.Reflection;
using System.Runtime.InteropServices;

namespace DXSharp.Interop;

internal static class NativeRuntime {
    private static readonly Dictionary<string, nint> Modules = new( StringComparer.OrdinalIgnoreCase );
    private static readonly object Gate = new( );
    static NativeRuntime( ) => NativeLibrary.SetDllImportResolver( typeof(NativeRuntime).Assembly, Resolve );

    internal static void Initialize( ) {
        if ( !OperatingSystem.IsWindows( ) ) throw new PlatformNotSupportedException( "DirectX execution requires Windows." );
    }

    private static nint Resolve( string name, Assembly assembly, DllImportSearchPath? searchPath ) {
        string file = name.EndsWith( ".dll", StringComparison.OrdinalIgnoreCase ) ? name : name + ".dll";
        string? path = file.ToLowerInvariant( ) switch {
            "d3d12.dll" or "dxgi.dll" => Path.Combine( Environment.SystemDirectory, file ),
            "dxcompiler.dll" or "dxil.dll" or "directml.dll" => Path.Combine( AppContext.BaseDirectory, file ),
            _ => null
        };
        if ( path is null ) return 0;
        lock ( Gate ) {
            if ( Modules.TryGetValue( path, out nint module ) ) return module;
            if ( !File.Exists( path ) ) throw new DllNotFoundException( $"Required native dependency was not found: {path}" );
            module = NativeLibrary.Load( path );
            Modules.Add( path, module );
            return module; // Modules remain loaded until process exit, including while finalizers run.
        }
    }
}
