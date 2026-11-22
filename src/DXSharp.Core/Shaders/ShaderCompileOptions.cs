namespace DXSharp.Shaders;

/// <summary>Common DXC options with an unrestricted additional-argument path.</summary>
public sealed record ShaderCompileOptions {
    /// <summary>DXC target profile, including mesh, amplification and library profiles.</summary>
    public string Profile { get; init; } = "vs_6_0";
    /// <summary>Entry point; null for a library profile.</summary>
    public string? EntryPoint { get; init; } = "main";
    /// <summary>Source identity reported in diagnostics and debug information.</summary>
    public string? SourceName { get; init; }
    /// <summary>HLSL language version.</summary>
    public string LanguageVersion { get; init; } = "2021";
    /// <summary>Optimization level 0 through 3.</summary>
    public int OptimizationLevel { get; init; } = 3;
    /// <summary>Emit debug information into the compiled container.</summary>
    public bool DebugInformation { get; init; }
    /// <summary>Treat warnings as errors.</summary>
    public bool WarningsAsErrors { get; init; }
    /// <summary>Preprocessor definitions, each in NAME or NAME=VALUE form.</summary>
    public IReadOnlyList<string> Defines { get; init; } = Array.Empty<string>( );
    /// <summary>Include search directories. Without a callback, the compiler resolves includes from the filesystem.</summary>
    public IReadOnlyList<string> IncludeDirectories { get; init; } = Array.Empty<string>( );
    /// <summary>Additional DXC arguments preserved in supplied order.</summary>
    public IReadOnlyList<string> AdditionalArguments { get; init; } = Array.Empty<string>( );
    internal List<string> Arguments( ) {
        if ( OptimizationLevel is < 0 or > 3 ) throw new ArgumentOutOfRangeException( nameof(OptimizationLevel) );
        ArgumentException.ThrowIfNullOrWhiteSpace( LanguageVersion );
        ArgumentNullException.ThrowIfNull( Defines ); ArgumentNullException.ThrowIfNull( IncludeDirectories ); ArgumentNullException.ThrowIfNull( AdditionalArguments );
        var result = new List<string>( );
        if ( SourceName is not null ) result.Add( SourceName );
        result.AddRange( ["-HV", LanguageVersion, "-O" + OptimizationLevel] );
        if ( DebugInformation ) result.AddRange( ["-Zi", "-Qembed_debug"] );
        if ( WarningsAsErrors ) result.Add( "-WX" );
        foreach ( string value in Defines ) { result.Add( "-D" ); result.Add( value ); }
        foreach ( string value in IncludeDirectories ) { result.Add( "-I" ); result.Add( value ); }
        result.AddRange( AdditionalArguments ); return result;
    }
}

public sealed unsafe partial class ShaderCompiler {
    /// <summary>Compiles a shader or library using typed options and optional virtual include resolution.</summary>
    public ShaderCompilation Compile( string source, ShaderCompileOptions options, Func<string, ReadOnlyMemory<byte>>? includeResolver = null ) {
        ArgumentNullException.ThrowIfNull( options );
        return options.EntryPoint is null ? CompileLibrary( source, options.Profile, options.Arguments( ), includeResolver ) :
            Compile( source, options.EntryPoint, options.Profile, options.Arguments( ), includeResolver );
    }
    /// <summary>Compiles UTF-8 HLSL from disk, adding its directory to the include search path.</summary>
    public ShaderCompilation CompileFile( string path, ShaderCompileOptions options ) {
        ArgumentNullException.ThrowIfNull( options ); string fullPath = Path.GetFullPath( path );
        return Compile( File.ReadAllText( fullPath ), options with {
            SourceName = fullPath, IncludeDirectories = [Path.GetDirectoryName( fullPath )!, ..options.IncludeDirectories]
        } );
    }
}
