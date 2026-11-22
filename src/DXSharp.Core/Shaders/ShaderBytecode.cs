namespace DXSharp.Shaders;

/// <summary>An owned shader container that can be loaded without invoking DXC.</summary>
public sealed class ShaderBytecode {
    private readonly byte[] _bytes;
    private ShaderBytecode( byte[] bytes ) {
        if ( bytes.Length < 32 || bytes[0] != 'D' || bytes[1] != 'X' || bytes[2] != 'B' || bytes[3] != 'C' )
            throw new ArgumentException( "Expected a DXBC/DXIL shader container.", nameof(bytes) );
        _bytes = bytes;
    }
    /// <summary>The owned container bytes. Pipeline creation reads them synchronously.</summary>
    public ReadOnlyMemory<byte> Bytes => _bytes;
    /// <summary>Copies a precompiled DXBC/DXIL container into owned storage.</summary>
    public static ShaderBytecode CopyFrom( ReadOnlySpan<byte> bytes ) => new( bytes.ToArray( ) );
    /// <summary>Loads an existing compiled shader. No compiler DLL is loaded.</summary>
    public static ShaderBytecode Load( string path ) => new( File.ReadAllBytes( path ) );
    /// <summary>Reads a compiled shader from the stream's current position, leaving the stream open.</summary>
    public static ShaderBytecode Load( Stream stream ) {
        ArgumentNullException.ThrowIfNull( stream );
        using var copy = new MemoryStream( ); stream.CopyTo( copy ); return new( copy.ToArray( ) );
    }
    /// <summary>Writes the shader container to a file.</summary>
    public void Save( string path ) => File.WriteAllBytes( path, _bytes );
    internal ShaderCompilation AsCompilation( ) => new( 0, _bytes, string.Empty, new Dictionary<string, ReadOnlyMemory<byte>>( ) );
}
