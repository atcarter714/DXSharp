using System.Text;
using System.Text.Json;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace ModernTriangle;

internal static class Program {
    private const string Shader = """
        struct Vertex { float4 position : SV_Position; float3 color : COLOR; };
        Vertex VS(uint id : SV_VertexID) {
            float2 positions[3] = { float2(0, 0.65), float2(0.65, -0.65), float2(-0.65, -0.65) };
            float3 colors[3] = { float3(1, 0.2, 0.3), float3(0.2, 1, 0.3), float3(0.2, 0.3, 1) };
            Vertex value; value.position = float4(positions[id], 0, 1); value.color = colors[id]; return value;
        }
        float4 PS(Vertex value) : SV_Target { return float4(value.color, 1); }
        """;

    [STAThread]
    public static int Main( string[] args ) {
        GraphicsDevice? device = null;
        string evidence = Path.GetFullPath( GetOption( args, "--evidence=" ) ?? "artifacts/modern-triangle" );
        Directory.CreateDirectory( evidence );
        try {
            bool resizeTest = args.Contains( "--resize-test" );
            bool headless = args.Contains( "--headless" );
            int frameLimit = int.Parse( GetOption( args, "--frames=" ) ?? (resizeTest ? "4" : "0") );
            if ( frameLimit < 0 ) throw new ArgumentOutOfRangeException( nameof(frameLimit) );
            if ( resizeTest && (headless || frameLimit < 3) )
                throw new ArgumentException( "--resize-test requires a window and at least three frames." );
#if DEBUG
            const bool debugLayer = true;
#else
            const bool debugLayer = false;
#endif
            device = GraphicsDevice.Create( new( ) {
                UseWarp = args.Contains( "--warp" ), UseAgility = !args.Contains( "--system-runtime" ),
                EnableDebugLayer = debugLayer && !args.Contains( "--no-debug" )
            } );
            Console.WriteLine( device.AdapterName );
            WriteDiagnostics( device, evidence );
            using var compiler = new ShaderCompiler( );
            QualifyCompiler( compiler );
            ComputeRoundTrip( device, compiler );
            if ( !headless ) {
                Application.SetUnhandledExceptionMode( UnhandledExceptionMode.ThrowException );
                ApplicationConfiguration.Initialize( );
                using var root = device.CreateRootSignature( );
                using var pipeline = device.CreateGraphicsPipeline( root, compiler.Compile( Shader, "VS", "vs_6_0" ), compiler.Compile( Shader, "PS", "ps_6_0" ) );
                Size initialSize = default;
                int previousResizes = 0;
                bool stressGc = args.Contains( "--gc-stress" );
                var result = DesktopGame.Run( device, new( ) { Title = "DXSharp · Generated COM · .NET 10", FrameLimit = frameLimit }, frame => {
                    frame.Commands.SetGraphicsPipeline( pipeline );
                    frame.Commands.Draw( 3 );
                }, ( window, progress ) => {
                    if ( resizeTest ) {
                        if ( progress.Frames == 1 ) {
                            initialSize = window.ClientSize;
                            window.ClientSize = new( initialSize.Width + 64, initialSize.Height + 48 );
                        } else if ( progress.Frames is 2 or 3 ) {
                            if ( progress.Resizes == previousResizes )
                                throw new InvalidOperationException( "The test window size change did not resize the swap chain." );
                            if ( progress.Frames == 2 ) window.ClientSize = initialSize;
                            else Console.WriteLine( "PASS: rendered and presented after both swap-chain resizes." );
                        }
                    }
                    previousResizes = progress.Resizes;
                    if ( stressGc && (progress.Frames - 1) % 30 == 0 ) { GC.Collect( ); GC.WaitForPendingFinalizers( ); }
                } );
                if ( resizeTest && result.Frames < 3 ) throw new InvalidOperationException( "The window closed before the resize test completed." );
                Console.WriteLine( $"Rendered {result.Frames} frames." );
            }
            var diagnostics = WriteDiagnostics( device, evidence );
            if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( x => x.Contains( "SEVERITY_ERROR" ) || x.Contains( "SEVERITY_CORRUPTION" ) ) )
                throw new InvalidOperationException( "The graphics validation layer reported errors; see diagnostics.json." );
            Console.WriteLine( "PASS: DXC, include callbacks, compute/readback, and requested rendering completed." );
            return 0;
        } catch ( Exception error ) {
            File.WriteAllText( Path.Combine( evidence, "failure.txt" ), error.ToString( ) );
            Console.Error.WriteLine( error );
            if ( device is not null ) { try { WriteDiagnostics( device, evidence ); } catch ( Exception diagnosticError ) { Console.Error.WriteLine( diagnosticError ); } }
            return 1;
        } finally { device?.Dispose( ); }
    }

    private static string? GetOption( string[] args, string prefix ) => args.FirstOrDefault( x => x.StartsWith( prefix, StringComparison.Ordinal ) )?[prefix.Length..];
    private static GraphicsDiagnostics WriteDiagnostics( GraphicsDevice device, string directory ) {
        var value = device.GetDiagnostics( );
        File.WriteAllText( Path.Combine( directory, "diagnostics.json" ), JsonSerializer.Serialize( value, new JsonSerializerOptions { WriteIndented = true } ) );
        return value;
    }
    private static void QualifyCompiler( ShaderCompiler compiler ) {
        var failed = compiler.Compile( "not valid HLSL", "Main", "cs_6_0" );
        if ( failed.Succeeded || string.IsNullOrWhiteSpace( failed.Diagnostics ) ) throw new InvalidOperationException( "DXC failure diagnostics were lost." );
        int calls = 0;
        var valid = compiler.Compile( "#include \"constant.hlsli\"\nRWStructuredBuffer<uint> output : register(u0); [numthreads(1,1,1)] void Main() { output[0] = VALUE; }",
            "Main", "cs_6_0", includeResolver: name => {
                if ( !name.EndsWith( "constant.hlsli", StringComparison.Ordinal ) ) throw new FileNotFoundException( name );
                ++calls; GC.Collect( ); return Encoding.UTF8.GetBytes( "#define VALUE 1234\n" );
            } ).RequireSuccess( );
        if ( calls == 0 || valid.Bytecode.IsEmpty ) throw new InvalidOperationException( "The managed include callback did not produce bytecode." );
    }
    private static void ComputeRoundTrip( GraphicsDevice device, ShaderCompiler compiler ) {
        using var queue = device.CreateCommandQueue( QueueKind.Compute );
        using var allocator = device.CreateCommandAllocator( QueueKind.Compute );
        using var commands = device.CreateCommandList( allocator );
        using var root = device.CreateRootSignature( 1 );
        var shader = compiler.Compile( "RWStructuredBuffer<uint> output : register(u0); [numthreads(1,1,1)] void Main() { output[0] = 1234; }", "Main", "cs_6_0" ).RequireSuccess( );
        using var pipeline = device.CreateComputePipeline( root, shader );
        using var output = device.CreateBuffer( 4, initialState: ResourceState.UnorderedAccess, unorderedAccess: true );
        using var readback = device.CreateBuffer( 4, BufferMemory.Readback );
        commands.SetComputePipeline( pipeline ); commands.SetComputeBuffer( 0, output ); commands.Dispatch( 1 );
        commands.UavBarrier( output ); commands.Transition( output, ResourceState.UnorderedAccess, ResourceState.CopySource );
        commands.CopyBuffer( output, readback, 4 ); commands.Close( );
        // Recording leases survive disposal of the public owners, before submission.
        root.Dispose( ); pipeline.Dispose( ); output.Dispose( );
        GC.Collect( ); GC.WaitForPendingFinalizers( );
        queue.Wait( queue.Submit( commands ) );
        Span<uint> result = stackalloc uint[1]; readback.Read( result );
        if ( result[0] != 1234 ) throw new InvalidOperationException( $"Compute readback: expected 1234, got {result[0]}." );
        Console.WriteLine( "PASS: compute/readback = 1234; native dependencies survived public disposal and GC." );
    }

}
