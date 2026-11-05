using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D.Dxc;
using DXSharp.Interop;

namespace DXSharp.Shaders;

/// <summary>Copied, independently owned compilation outputs. Diagnostics include warnings on success.</summary>
public sealed record ShaderCompilation( int Status, ReadOnlyMemory<byte> Bytecode, string Diagnostics, IReadOnlyDictionary<string, ReadOnlyMemory<byte>> Outputs ) {
    /// <summary>Whether HLSL compilation succeeded.</summary>
    public bool Succeeded => Status >= 0;
    /// <summary>Throws with compiler diagnostics if the shader did not compile.</summary>
    public ShaderCompilation RequireSuccess( ) {
        if ( !Succeeded ) throw new InvalidOperationException( $"HLSL compilation failed (0x{Status:X8}):{Environment.NewLine}{Diagnostics}" );
        return this;
    }
}

/// <summary>A synchronous DXC compiler using generated COM, scoped source storage, and copied outputs.</summary>
public sealed unsafe class ShaderCompiler : IDisposable {
    private readonly ComOwner<IDxcCompiler3> _compiler;
    private readonly object _gate = new( );
    /// <summary>Loads the packaged DXC compiler without depending on the working directory.</summary>
    public ShaderCompiler( ) {
        NativeRuntime.Initialize( );
        Guid compilerClass = new( "73e22d93-e6ce-47f3-b5bf-f0664f39c1b0" ), compilerId = typeof(IDxcCompiler3).GUID;
        Results.Check( PInvoke.DxcCreateInstance( &compilerClass, &compilerId, out nint pointer ), "DxcCreateInstance(Compiler3)" );
        _compiler = ComOwner<IDxcCompiler3>.Adopt( pointer );
    }

    /// <summary>Compiles UTF-8 HLSL. Includes are resolved by the supplied callback; each output owns its bytes.</summary>
    public ShaderCompilation Compile( string source, string entryPoint, string profile, IEnumerable<string>? arguments = null,
        Func<string, ReadOnlyMemory<byte>>? includeResolver = null ) {
        ArgumentNullException.ThrowIfNull( source );
        ArgumentException.ThrowIfNullOrWhiteSpace( entryPoint ); ArgumentException.ThrowIfNullOrWhiteSpace( profile );
        var options = new List<string>( ) { "-E", entryPoint, "-T", profile };
        if ( arguments is not null ) options.AddRange( arguments );
        if ( options.Any( value => string.IsNullOrEmpty( value ) || value.Contains( '\0' ) ) ) throw new ArgumentException( "Compiler arguments cannot be empty or contain NUL." );
        byte[] bytes = Encoding.UTF8.GetBytes( source );
        using var nativeArguments = new Arguments( options );
        using var includes = includeResolver is null ? null : new IncludeHandler( includeResolver );
        lock ( _gate ) {
            fixed ( byte* input = bytes ) fixed ( nint* args = nativeArguments.Values ) {
                DxcBuffer buffer = new( ) { Ptr = input, Size = (nuint)bytes.Length, Encoding = 65001 };
                Guid resultId = typeof(IDxcResult).GUID;
                Results.Check( _compiler.Interface.Compile( &buffer, (PCWSTR*)args, (uint)options.Count, includes!, &resultId, out nint resultPointer ), "Compiler3.Compile" );
                using var result = ComOwner<IDxcResult>.Adopt( resultPointer );
                HRESULT status;
                Results.Check( result.Interface.GetStatus( &status ), "DxcResult.GetStatus" );
                var outputs = new Dictionary<string, ReadOnlyMemory<byte>>( StringComparer.Ordinal );
                foreach ( var (kind, name) in OutputKinds ) {
                    if ( !result.Interface.HasOutput( kind ) ) continue;
                    outputs.Add( name, CopyOutput( result.Interface, kind ) );
                }
                outputs.TryGetValue( "Object", out var bytecode );
                outputs.TryGetValue( "Errors", out var errors );
                return new( status.Value, bytecode, Encoding.UTF8.GetString( errors.Span ).TrimEnd( '\0' ), outputs );
            }
        }
    }

    private static readonly (DXC_OUT_KIND Kind, string Name)[] OutputKinds = [
        (DXC_OUT_KIND.DXC_OUT_OBJECT, "Object"), (DXC_OUT_KIND.DXC_OUT_ERRORS, "Errors"),
        (DXC_OUT_KIND.DXC_OUT_REFLECTION, "Reflection"), (DXC_OUT_KIND.DXC_OUT_PDB, "Pdb"),
        (DXC_OUT_KIND.DXC_OUT_SHADER_HASH, "Hash"), (DXC_OUT_KIND.DXC_OUT_ROOT_SIGNATURE, "RootSignature"),
        (DXC_OUT_KIND.DXC_OUT_DISASSEMBLY, "Disassembly")
    ];
    private static byte[] CopyOutput( IDxcResult result, DXC_OUT_KIND kind ) {
        Guid blobId = typeof(IDxcBlob).GUID;
        nint pointer = 0, name = 0;
        int hr = result.GetOutput( kind, &blobId, (void**)&pointer, (IDxcBlobUtf16_unmanaged**)&name );
        try {
            Results.Check( hr, $"DxcResult.GetOutput({kind})" );
            nint owned = pointer; pointer = 0;
            using var blob = ComOwner<IDxcBlob>.Adopt( owned );
            return new ReadOnlySpan<byte>( blob.Interface.GetBufferPointer( ), checked((int)blob.Interface.GetBufferSize( )) ).ToArray( );
        } finally {
            if ( pointer != 0 ) UnknownCalls.Release( pointer );
            if ( name != 0 ) UnknownCalls.Release( name );
        }
    }
    /// <summary>Releases compiler references after any current compilation returns.</summary>
    public void Dispose( ) { lock ( _gate ) _compiler.Dispose( ); }

    private sealed class Arguments : IDisposable {
        internal readonly nint[] Values;
        internal Arguments( List<string> values ) {
            Values = new nint[values.Count];
            try { for ( int i = 0; i < values.Count; ++i ) Values[i] = Marshal.StringToCoTaskMemUni( values[i] ); }
            catch { Dispose( ); throw; }
        }
        public void Dispose( ) { foreach ( nint pointer in Values ) Marshal.FreeCoTaskMem( pointer ); Array.Clear( Values ); }
    }
}

[GeneratedComClass]
internal sealed unsafe partial class IncludeHandler( Func<string, ReadOnlyMemory<byte>> resolver ) : IDxcIncludeHandler, IDisposable {
    private readonly List<ShaderSourceBlob> _sources = new( );
    public HRESULT LoadSource( PCWSTR filename, out IDxcBlob source ) {
        source = null!;
        try {
            lock ( _sources ) {
                var blob = new ShaderSourceBlob( resolver( filename.ToString( ) ).Span );
                _sources.Add( blob ); source = blob; return new HRESULT( 0 );
            }
        } catch ( Exception error ) { return new HRESULT( Marshal.GetHRForException( error ) ); }
    }
    public void Dispose( ) { lock ( _sources ) { foreach ( var source in _sources ) source.Dispose( ); _sources.Clear( ); } }
}

[GeneratedComClass]
internal sealed unsafe partial class ShaderSourceBlob : IDxcBlob, IDisposable {
    private nint _pointer;
    private readonly nuint _length;
    internal ShaderSourceBlob( ReadOnlySpan<byte> source ) {
        _length = (nuint)source.Length; _pointer = (nint)NativeMemory.Alloc( checked(_length + 1) );
        source.CopyTo( new Span<byte>( (void*)_pointer, source.Length ) ); ((byte*)_pointer)[source.Length] = 0;
    }
    public void* GetBufferPointer( ) => (void*)_pointer;
    public nuint GetBufferSize( ) => _length;
    ~ShaderSourceBlob( ) => Dispose( );
    public void Dispose( ) { NativeMemory.Free( (void*)Interlocked.Exchange( ref _pointer, 0 ) ); GC.SuppressFinalize( this ); }
}
