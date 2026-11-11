using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp;
using DXSharp.Graphics;
using DXSharp.Interop;
using DXSharp.Shaders;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;

namespace InteropQualification;

internal static unsafe class MeshQualification {
    private const uint Width = 61, Height = 47;
    private const string Shader = """
        struct Input {
            float2 position : POSITION;
            float2 offset : TEXCOORD;
            float4 color : COLOR;
        };
        struct Output { float4 position : SV_Position; float4 color : COLOR; };
        Output VS(Input input) {
            Output value;
            value.position = float4(input.position + input.offset, 0, 1);
            value.color = input.color;
            return value;
        }
        float4 PS(Output input) : SV_Target { return input.color; }
        """;

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct Instance( Vector2 Offset, Vector4 Color );

    internal static void Run( ) {
#if DEBUG
        const bool debug = true;
#else
        const bool debug = false;
#endif
        using var device = GraphicsDevice.Create( new( ) { UseWarp = true, EnableDebugLayer = debug } );
        using var compiler = new ShaderCompiler( );
        var vertex = compiler.Compile( Shader, "VS", "vs_6_0" );
        var pixel = compiler.Compile( Shader, "PS", "ps_6_0" );
        ValidateArguments( device, vertex, pixel );
        Render( device, vertex, pixel, instanceStepRate: 1, splitIndexFormats: true );
        Render( device, vertex, pixel, instanceStepRate: 1, splitIndexFormats: false );
        Render( device, vertex, pixel, instanceStepRate: 2, splitIndexFormats: false );
        var diagnostics = device.GetDiagnostics( );
        if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any(
            message => message.Contains( "SEVERITY_ERROR" ) || message.Contains( "SEVERITY_CORRUPTION" ) ) )
            throw new InvalidOperationException( $"Mesh validation failed: {string.Join( Environment.NewLine, diagnostics.Messages )}" );
        Console.WriteLine( "PASS: indexed mesh pixels, UInt16/UInt32, instance streams, and early owner disposal." );
    }

    private static void Render( GraphicsDevice device, ShaderCompilation vertex, ShaderCompilation pixel,
        uint instanceStepRate, bool splitIndexFormats ) {
        using var queue = device.CreateCommandQueue( );
        using var uploadAllocator = device.CreateCommandAllocator( );
        using var uploads = device.CreateCommandList( uploadAllocator );
        Vector2[] vertices = [new(-0.3f,-0.6f), new(0.3f,-0.6f), new(0.3f,0.6f), new(-0.3f,0.6f)];
        using var small = device.CreateMesh<Vector2>( uploads, vertices, new ushort[] { 0, 1, 2, 0, 2, 3 } );
        var largeVertices = new Vector2[65_537];
        vertices.AsSpan( ).CopyTo( largeVertices.AsSpan( 65_533 ) );
        using var large = device.CreateMesh<Vector2>( uploads, largeVertices,
            new uint[] { 65_533, 65_534, 65_535, 65_533, 65_535, 65_536 } );
        uploads.Close( );
        using var root = device.CreateRootSignature( );
        using var pipeline = device.CreateGraphicsPipeline( root, vertex, pixel, [
            new( "POSITION", VertexFormat.Float2, 0 ),
            new( "TEXCOORD", VertexFormat.Float2, 0, Slot: 1, InstanceStepRate: instanceStepRate ),
            new( "COLOR", VertexFormat.Float4, 8, Slot: 1, InstanceStepRate: instanceStepRate )
        ] );
        using var instances = device.CreateBuffer( 3 * 24, BufferMemory.Upload );
        instances.Write<Instance>( [
            new( new(99,99), new(0,0,1,1) ),
            new( new(-0.45f,0), new(1,0,0,1) ), new( new(0.45f,0), new(0,1,0,1) )
        ] );
        using var target = CreateTarget( device );
        using var heap = device.CreateRenderTargetHeap( 1 );
        var view = device.CreateRenderTargetView( target, heap, 0 );
        D3D12_RESOURCE_DESC description = target.Native.Interface.GetDesc( );
        D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint;
        ulong readbackBytes;
        device.Native.Interface.GetCopyableFootprints( &description, 0, 1, 0, &footprint, null, null, &readbackBytes );
        using var readback = device.CreateBuffer( readbackBytes, BufferMemory.Readback );
        using var drawAllocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( drawAllocator );
        commands.SetViewport( Width, Height );
        commands.Clear( view, 0, 0, 0 );
        commands.SetRenderTarget( view );
        commands.SetGraphicsPipeline( pipeline );
        commands.SetVertexBuffer( 1, instances, 24, byteOffset: 24, sizeInBytes: 48 );
        if ( splitIndexFormats ) {
            commands.DrawMesh( small );
            commands.DrawMesh( large, startInstance: 1 );
        } else commands.DrawMesh( small, instanceCount: 2 * instanceStepRate );
        commands.Transition( target, ResourceState.RenderTarget, ResourceState.CopySource );
        CopyPixels( commands, target, readback, footprint );
        commands.Close( );

        // Both recordings must retain their resources independently before either submission.
        small.Dispose( );
        large.Dispose( );
        instances.Dispose( );
        root.Dispose( );
        pipeline.Dispose( );
        target.Dispose( );
        heap.Dispose( );
        GC.Collect( );
        GC.WaitForPendingFinalizers( );
        _ = queue.Submit( uploads );
        queue.Wait( queue.Submit( commands ) );
        var bytes = new byte[checked((int)readbackBytes)];
        readback.Read<byte>( bytes );
        ValidatePixels( bytes, footprint );
    }

    private static GpuResource CreateTarget( GraphicsDevice device ) {
        D3D12_RESOURCE_DESC description = new( ) {
            Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            Width = Width, Height = Height, DepthOrArraySize = 1, MipLevels = 1,
            Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, SampleDesc = new( ) { Count = 1 },
            Flags = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET
        };
        D3D12_HEAP_PROPERTIES heap = new( ) {
            Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT, CreationNodeMask = 1, VisibleNodeMask = 1
        };
        Guid id = typeof(ID3D12Resource).GUID;
        nint pointer = 0;
        Results.Check( device.Native.Interface.CreateCommittedResource( &heap, 0, &description,
            D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET, null, &id, (void**)&pointer ), "Create mesh qualification target" );
        return new( ComOwner<ID3D12Resource>.Adopt( pointer ), 0, BufferMemory.Default ) { DeviceIdentity = device.DeviceIdentity };
    }

    private static void CopyPixels( GraphicsCommandList commands, GpuResource source, GpuResource destination,
        D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint ) {
        commands.RetainDependency( destination );
        using var sourceLease = source.Native.Borrow( );
        using var destinationLease = destination.Native.Borrow( );
        D3D12_TEXTURE_COPY_LOCATION from = new( ) {
            pResource = (ID3D12Resource_unmanaged*)sourceLease.Pointer,
            Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_SUBRESOURCE_INDEX
        };
        D3D12_TEXTURE_COPY_LOCATION to = new( ) {
            pResource = (ID3D12Resource_unmanaged*)destinationLease.Pointer,
            Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_PLACED_FOOTPRINT
        };
        to.Anonymous.PlacedFootprint = footprint;
        commands.Native.Interface.CopyTextureRegion( to, 0, 0, 0, from, null );
    }

    private static void ValidatePixels( byte[] bytes, D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint ) {
        int colored = 0;
        for ( uint y = 0; y < Height; ++y ) {
            for ( uint x = 0; x < Width; ++x ) {
                float px = 2 * (x + 0.5f) / Width - 1;
                float py = 1 - 2 * (y + 0.5f) / Height;
                // Ignore the rasterization boundary while checking every interior and exterior pixel.
                if ( MathF.Abs( MathF.Abs( py ) - 0.6f ) < 0.025f ||
                     MathF.Abs( MathF.Abs( px ) - 0.15f ) < 0.025f ||
                     MathF.Abs( MathF.Abs( px ) - 0.75f ) < 0.025f ) continue;
                bool inside = MathF.Abs( py ) < 0.6f && MathF.Abs( px ) > 0.15f && MathF.Abs( px ) < 0.75f;
                byte red = inside && px < 0 ? byte.MaxValue : (byte)0;
                byte green = inside && px > 0 ? byte.MaxValue : (byte)0;
                int offset = checked((int)(footprint.Offset + y * footprint.Footprint.RowPitch + x * 4));
                if ( bytes[offset] != red || bytes[offset + 1] != green || bytes[offset + 2] != 0 || bytes[offset + 3] != 255 )
                    throw new InvalidOperationException( $"Indexed mesh pixel ({x}, {y}) differs." );
                if ( inside ) ++colored;
            }
        }
        if ( colored < 700 ) throw new InvalidOperationException( "Mesh qualification checked too few colored pixels." );
    }

    private static void ValidateArguments( GraphicsDevice device, ShaderCompilation vertex, ShaderCompilation pixel ) {
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        using var root = device.CreateRootSignature( );
        Vector2[] vertices = [new(0,0), new(1,0), new(0,1)];
        Expect<ArgumentException>( ( ) => device.CreateMesh<Vector2>( commands, Array.Empty<Vector2>( ), new ushort[] { 0 } ) );
        Expect<ArgumentOutOfRangeException>( ( ) => device.CreateMesh<Vector2>( commands, vertices, new ushort[] { 3 } ) );
        Expect<ArgumentOutOfRangeException>( ( ) => device.CreateMesh<Vector2>( commands, vertices, new uint[] { uint.MaxValue } ) );
        Expect<ArgumentException>( ( ) => device.CreateGraphicsPipeline( root, vertex, pixel, [default(VertexElement)] ) );
        Expect<ArgumentException>( ( ) => device.CreateGraphicsPipeline( root, vertex, pixel, [new("POSITION", (VertexFormat)999, 0)] ) );
        Expect<ArgumentException>( ( ) => device.CreateGraphicsPipeline( root, vertex, pixel, [new("POSITION", VertexFormat.Float4, 2040)] ) );
        using var buffer = device.CreateBuffer( 64, BufferMemory.Upload );
        using var readback = device.CreateBuffer( 64, BufferMemory.Readback );
        Expect<ArgumentOutOfRangeException>( ( ) => commands.SetVertexBuffer( 32, buffer, 8 ) );
        Expect<ArgumentOutOfRangeException>( ( ) => commands.SetVertexBuffer( 0, buffer, 0 ) );
        Expect<ArgumentException>( ( ) => commands.SetVertexBuffer( 0, readback, 8 ) );
        Expect<ArgumentException>( ( ) => commands.SetIndexBuffer( buffer, IndexFormat.UInt16, byteOffset: 1 ) );
        Expect<ArgumentOutOfRangeException>( ( ) => commands.SetIndexBuffer( buffer, (IndexFormat)0 ) );
        using var mesh = device.CreateMesh<Vector2>( commands, vertices, new ushort[] { 0, 1, 2 } );
        mesh.Dispose( );
        Expect<ObjectDisposedException>( ( ) => commands.DrawMesh( mesh ) );
        commands.Close( );
        Expect<InvalidOperationException>( ( ) => device.CreateMesh<Vector2>( commands, vertices, new ushort[] { 0, 1, 2 } ) );
        using var computeAllocator = device.CreateCommandAllocator( QueueKind.Compute );
        using var compute = device.CreateCommandList( computeAllocator );
        Expect<InvalidOperationException>( ( ) => device.CreateMesh<Vector2>( compute, vertices, new ushort[] { 0, 1, 2 } ) );
        Expect<InvalidOperationException>( ( ) => compute.SetVertexBuffer( 0, buffer, 8 ) );
    }

    private static void Expect<T>( Action action ) where T : Exception {
        try { action( ); }
        catch ( T ) { return; }
        throw new InvalidOperationException( $"Expected {typeof(T).Name}." );
    }
}
