using System.Numerics;
using DXSharp.Graphics;
using DXSharp.Shaders;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;

internal static class Program {
    private const uint Width = 64, Height = 64;
    private const string Shader = """
        cbuffer Draw : register(b0) { float scale; float r; float g; float b; }
        float4 VS(uint id : SV_VertexID) : SV_Position {
            float2 positions[6] = {
                float2(-1,-1), float2(-1,1), float2(1,1),
                float2(-1,-1), float2(1,1), float2(1,-1)
            };
            return float4(positions[id] * scale, 0.5, 1);
        }
        float4 VertexVS(float2 position : POSITION) : SV_Position { return float4(position * scale, 0.5, 1); }
        float4 PS() : SV_Target { return float4(r,g,b,1); }
        """;

    private static int Main( string[] args ) {
        try {
            using var device = GraphicsDevice.Create( new( ) {
                UseWarp = args.Contains( "--warp" ),
                EnableDebugLayer = !args.Contains( "--no-debug" )
            } );
            using var compiler = new ShaderCompiler( );
            foreach ( var format in Enum.GetValues<DepthStencilFormat>( ) ) {
                Verify( device, compiler, format, false );
                Verify( device, compiler, format, true );
                Console.WriteLine( $"PASS: {format} creation, clears, generated/vertex-buffer rendering, readback, and submission ownership." );
            }
            var diagnostics = device.GetDiagnostics( );
            if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( value =>
                value.Contains( "SEVERITY_ERROR" ) || value.Contains( "SEVERITY_CORRUPTION" ) ) )
                throw new InvalidOperationException( string.Join( Environment.NewLine, diagnostics.Messages ) );
            return 0;
        } catch ( Exception error ) {
            Console.Error.WriteLine( error );
            return 1;
        }
    }

    private static unsafe void Verify( GraphicsDevice device, ShaderCompiler compiler, DepthStencilFormat format, bool vertexInput ) {
        using var queue = device.CreateCommandQueue( );
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        using var depth = device.CreateDepthTarget( Width, Height, format );
        using var nativeDevice = device.QueryNativeInterface<ID3D12Device>( );
        D3D12_RESOURCE_DESC description = new( ) {
            Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            Width = Width, Height = Height, DepthOrArraySize = 1, MipLevels = 1,
            Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
            SampleDesc = new( ) { Count = 1 },
            Flags = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET
        };
        D3D12_HEAP_PROPERTIES memory = new( ) {
            Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT, CreationNodeMask = 1, VisibleNodeMask = 1
        };
        D3D12_CLEAR_VALUE clear = new( ) { Format = description.Format };
        clear.Anonymous.Color[3] = 1;
        using var color = device.CreateCommittedResource( description, memory, ResourceState.RenderTarget, optimizedClear: clear );
        using var heap = device.CreateRenderTargetHeap( 1 );
        var target = device.CreateRenderTargetView( color, heap, 0 );
        using var root = device.CreateGraphicsConstantsRootSignature( 4 );
        var vertex = compiler.Compile( Shader, vertexInput ? "VertexVS" : "VS", "vs_6_0" ).RequireSuccess( );
        var pixel = compiler.Compile( Shader, "PS", "ps_6_0" ).RequireSuccess( );
        var writeFace = new StencilFaceState { Pass = StencilOperation.Replace };
        var maskState = new DepthStencilState {
            Format = format, DepthEnabled = true, DepthWriteEnabled = false,
            DepthComparison = ComparisonFunction.Always,
            StencilEnabled = depth.HasStencil, FrontFace = writeFace, BackFace = writeFace
        };
        var testFace = new StencilFaceState { Comparison = ComparisonFunction.Equal };
        var testState = maskState with { FrontFace = testFace, BackFace = testFace };
        VertexElement[] layout = vertexInput ? [new("POSITION", VertexFormat.Float2, 0)] : [];
        using var vertices = vertexInput ? device.CreateBuffer( 6 * 8, BufferMemory.Upload ) : null;
        vertices?.Write<Vector2>( [new(-1,-1), new(-1,1), new(1,1), new(-1,-1), new(1,1), new(1,-1)] );
        using var mask = device.CreateGraphicsPipeline( root, vertex, pixel, layout, PixelFormat.Rgba8Unorm, maskState );
        using var paint = device.CreateGraphicsPipeline( root, vertex, pixel, layout, PixelFormat.Rgba8Unorm, testState );

        bool rejected = false;
        if ( !depth.HasStencil ) {
            try { commands.ClearDepthStencil( depth, DepthStencilClear.Stencil ); }
            catch ( ArgumentException ) { rejected = true; }
            if ( !rejected ) throw new InvalidOperationException( "A depth-only target accepted a stencil clear." );
        }
        commands.Clear( target, 0, 0, 0, 1 );
        commands.ClearDepthStencil( depth, depth.HasStencil ? DepthStencilClear.DepthAndStencil : DepthStencilClear.Depth );
        commands.SetRenderTarget( target, depth );
        commands.SetViewport( Width, Height );
        commands.SetStencilReference( 1 );
        if ( vertices is not null ) commands.SetVertexBuffer( 0, vertices, 8 );
        commands.SetGraphicsPipeline( mask );
        commands.SetGraphicsConstants( [Float( 0.5f ), Float( 1 ), Float( 0 ), Float( 0 )] );
        commands.Draw( 6 );
        commands.SetGraphicsPipeline( paint );
        commands.SetGraphicsConstants( [Float( 1 ), Float( 0 ), Float( 1 ), Float( 0 )] );
        commands.Draw( 6 );

        D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint;
        ulong bytes;
        nativeDevice.Interface.GetCopyableFootprints( &description, 0, 1, 0, &footprint, null, null, &bytes );
        using var readback = device.CreateBuffer( bytes, BufferMemory.Readback );
        commands.Transition( color, ResourceState.RenderTarget, ResourceState.CopySource );
        using ( var source = color.QueryNativeInterface<ID3D12Resource>( ) )
        using ( var destination = readback.QueryNativeInterface<ID3D12Resource>( ) ) {
            var sourceLocation = new D3D12_TEXTURE_COPY_LOCATION {
                pResource = (ID3D12Resource_unmanaged*)source.Pointer,
                Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_SUBRESOURCE_INDEX
            };
            var destinationLocation = new D3D12_TEXTURE_COPY_LOCATION {
                pResource = (ID3D12Resource_unmanaged*)destination.Pointer,
                Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_PLACED_FOOTPRINT
            };
            destinationLocation.Anonymous.PlacedFootprint = footprint;
            commands.RecordNative( native => native.CopyTextureRegion( destinationLocation, 0, 0, 0, sourceLocation, null ),
                color, readback );
        }
        commands.Close( );
        // Exercise the same early-disposal lifetime contract used by external UI renderers.
        vertices?.Dispose( );
        depth.Dispose( );
        color.Dispose( );
        heap.Dispose( );
        mask.Dispose( );
        paint.Dispose( );
        GC.Collect( );
        GC.WaitForPendingFinalizers( );
        queue.Wait( queue.Submit( commands ) );

        byte[] pixels = new byte[checked((int)bytes)];
        readback.Read<byte>( pixels );
        int center = checked((int)(footprint.Offset + (Height / 2) * footprint.Footprint.RowPitch + (Width / 2) * 4));
        int corner = checked((int)(footprint.Offset + footprint.Footprint.RowPitch + 4));
        if ( pixels[center] != 0 || pixels[center + 1] != 255 || pixels[center + 2] != 0 )
            throw new InvalidOperationException( "The center pixel did not pass the stencil test." );
        byte expected = depth.HasStencil ? (byte)0 : (byte)255;
        if ( pixels[corner] != 0 || pixels[corner + 1] != expected || pixels[corner + 2] != 0 )
            throw new InvalidOperationException( "The corner pixel did not match the stencil mask." );
    }

    private static uint Float( float value ) => BitConverter.SingleToUInt32Bits( value );
}
