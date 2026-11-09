using System.Text;
using System.Text.Json;
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
                using var window = new Form { Text = "DXSharp · Generated COM · .NET 10", ClientSize = new( 1024, 720 ) };
                window.Show( );
                using var renderer = new Renderer( device, compiler, window );
                int frames = 0;
                Size initialSize = window.ClientSize;
                using var timer = new System.Windows.Forms.Timer { Interval = 1 };
                window.FormClosing += ( _, _ ) => timer.Stop( );
                timer.Tick += ( _, _ ) => {
                    if ( window.Disposing || !window.IsHandleCreated || window.WindowState == FormWindowState.Minimized ) return;
                    Size clientSize = window.ClientSize;
                    if ( clientSize.Width <= 0 || clientSize.Height <= 0 ) return;
                    // Native presentation can dispatch window messages. Do not render from a nested tick.
                    timer.Stop( );
                    bool resized = renderer.Render( clientSize );
                    if ( resizeTest ) {
                        if ( frames == 0 ) {
                            window.ClientSize = new Size( initialSize.Width + 64, initialSize.Height + 48 );
                        } else if ( frames is 1 or 2 ) {
                            if ( !resized ) throw new InvalidOperationException( "The test window size change did not resize the swap chain." );
                            if ( frames == 1 ) window.ClientSize = initialSize;
                            else Console.WriteLine( "PASS: rendered and presented after both swap-chain resizes." );
                        }
                    }
                    if ( args.Contains( "--gc-stress" ) && frames % 30 == 0 ) { GC.Collect( ); GC.WaitForPendingFinalizers( ); }
                    ++frames;
                    if ( frameLimit > 0 && frames >= frameLimit ) window.Close( );
                    else if ( window.IsHandleCreated && !window.Disposing ) timer.Start( );
                };
                // Propagate rendering failures to the outer evidence handler.
                timer.Start( ); Application.Run( window ); timer.Stop( );
                if ( resizeTest && frames < 3 ) throw new InvalidOperationException( "The window closed before the resize test completed." );
                Console.WriteLine( $"Rendered {frames} frames." );
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

    private sealed class Renderer : IDisposable {
        private readonly GraphicsDevice _device;
        private readonly CommandQueue _queue;
        private readonly CommandAllocator _allocator;
        private readonly GraphicsCommandList _commands;
        private readonly SwapChain _swap;
        private readonly DescriptorHeap _rtvHeap;
        private readonly RootSignature _root;
        private readonly PipelineState _pipeline;
        private readonly GpuResource?[] _buffers = new GpuResource?[2];
        private readonly RenderTargetView[] _views = new RenderTargetView[2];
        private uint _width, _height;
        internal Renderer( GraphicsDevice device, ShaderCompiler compiler, Form window ) {
            _device = device;
            _width = (uint)window.ClientSize.Width; _height = (uint)window.ClientSize.Height;
            _queue = device.CreateCommandQueue( ); _allocator = device.CreateCommandAllocator( );
            _commands = device.CreateCommandList( _allocator ); _commands.Close( );
            _swap = device.CreateSwapChain( _queue, window.Handle, _width, _height );
            _rtvHeap = device.CreateRenderTargetHeap( 2 ); _root = device.CreateRootSignature( );
            _pipeline = device.CreateGraphicsPipeline( _root, compiler.Compile( Shader, "VS", "vs_6_0" ), compiler.Compile( Shader, "PS", "ps_6_0" ) );
            AcquireBuffers( );
        }
        private void AcquireBuffers( ) {
            for ( uint i = 0; i < 2; ++i ) {
                _buffers[i] = _swap.AcquireBackBuffer( i );
                _views[i] = _device.CreateRenderTargetView( _buffers[i]!, _rtvHeap, i );
            }
        }
        internal bool Render( Size clientSize ) {
            uint width = (uint)clientSize.Width, height = (uint)clientSize.Height;
            bool resized = width != _width || height != _height;
            if ( resized ) _queue.WaitIdle( );
            _allocator.Reset( );
            _commands.Reset( _allocator );
            if ( resized ) {
                foreach ( var buffer in _buffers ) buffer?.Dispose( );
                Array.Clear( _buffers );
                Array.Clear( _views );
                Console.WriteLine( $"Resizing swap chain from {_width}x{_height} to {width}x{height}." );
                _swap.Resize( width, height );
                _width = width;
                _height = height;
                AcquireBuffers( );
            }
            uint index = _swap.CurrentBackBufferIndex;
            _commands.Transition( _buffers[index]!, ResourceState.Present, ResourceState.RenderTarget );
            _commands.SetViewport( _width, _height ); _commands.Clear( _views[index], 0.025f, 0.035f, 0.065f );
            _commands.SetRenderTarget( _views[index] ); _commands.SetGraphicsPipeline( _pipeline ); _commands.Draw( 3 );
            _commands.Transition( _buffers[index]!, ResourceState.RenderTarget, ResourceState.Present );
            _commands.Close( ); _queue.Wait( _queue.Submit( _commands ) ); _swap.Present( );
            return resized;
        }
        public void Dispose( ) {
            _queue.WaitIdle( ); _commands.Dispose( );
            foreach ( var buffer in _buffers ) buffer?.Dispose( ); Array.Clear( _views );
            _pipeline.Dispose( ); _root.Dispose( ); _rtvHeap.Dispose( ); _swap.Dispose( ); _allocator.Dispose( ); _queue.Dispose( );
        }
    }
}
