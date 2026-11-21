using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;
using Windows.Win32.Graphics.Dxgi.Common;

namespace HelloTexture;

internal static class Program {
    // Based on Microsoft's D3D12 Hello Texture sample. Vertex data is generated in HLSL.
    private const string Shader = """
        cbuffer Options : register(b0) { float MipLevel; };
        Texture2D<float4> Checker : register(t0);
        SamplerState PointClamp : register(s0);
        struct Vertex { float4 position : SV_Position; float2 uv : TEXCOORD; };
        Vertex VS(uint id : SV_VertexID) {
            const float2 positions[3] = { float2(0, 0.72), float2(0.68, -0.65), float2(-0.68, -0.65) };
            const float2 uvs[3] = { float2(0.5, 0), float2(1, 1), float2(0, 1) };
            Vertex output;
            output.position = float4(positions[id], 0, 1);
            output.uv = uvs[id];
            return output;
        }
        float4 PS(Vertex input) : SV_Target { return Checker.SampleLevel(PointClamp, input.uv, MipLevel); }
        """;

    [STAThread]
    public static int Main( string[] args ) {
        Application.SetUnhandledExceptionMode( UnhandledExceptionMode.ThrowException );
        ApplicationConfiguration.Initialize( );
#if DEBUG
        const bool debugLayer = true;
#else
        const bool debugLayer = false;
#endif
        int frameLimit = int.Parse( args.FirstOrDefault( x => x.StartsWith( "--frames=", StringComparison.Ordinal ) )?[9..] ?? "0" );
        using var device = GraphicsDevice.Create( new( ) {
            UseWarp = args.Contains( "--warp" ),
            UseAgility = !args.Contains( "--system-runtime" ),
            EnableDebugLayer = debugLayer && !args.Contains( "--no-debug" )
        } );
        Console.WriteLine( device.AdapterName );
        using var uploadQueue = device.CreateCommandQueue( );
        using var compiler = new ShaderCompiler( );
        using var root = device.CreateTextureRootSignature( 1 );
        using var pipeline = device.CreateGraphicsPipeline( root, compiler.Compile( Shader, "VS", "vs_6_0" ), compiler.Compile( Shader, "PS", "ps_6_0" ) );
        const int size = 250; // Exercises row-pitch padding.
        using var descriptors = device.CreateDescriptorPool( 2 );
        using var uploadRing = device.CreateUploadRing( 1024 * 1024 );
        using var uploadAllocator = device.CreateCommandAllocator( );
        using var uploads = device.CreateCommandList( uploadAllocator );
        using var texture = device.CreateTexture2D( new(size, size, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, MipLevels: 3), descriptors );
        using var secondTexture = device.CreateTexture2D( texture.Description, descriptors );
        uploads.UploadTexture( texture, uploadRing, Mips( size, alternate: false ) );
        uploads.UploadTexture( secondTexture, uploadRing, Mips( size, alternate: true ) );
        uploads.Transition( texture, ResourceState.CopyDestination, ResourceState.PixelShaderResource );
        uploads.Transition( secondTexture, ResourceState.CopyDestination, ResourceState.PixelShaderResource );
        uploads.Close( );
        uploadQueue.Wait( uploadQueue.Submit( uploads ) );
        Console.WriteLine( $"Shared texture heap: {descriptors.Capacity} descriptors; upload ring reclaimed {uploadRing.AvailableBytes} bytes." );
        int frameIndex = 0;

        var result = DesktopGame.Run( device, new( ) { Title = "DXSharp · Hello Texture", FrameLimit = frameLimit }, frame => {
            frame.Commands.SetGraphicsPipeline( pipeline );
            uint mip = (uint)(frameIndex++ / 90 % 3);
            frame.Commands.SetGraphicsConstants( [BitConverter.SingleToUInt32Bits( mip )] );
            frame.Commands.SetGraphicsTexture( (frameIndex / 270 & 1) == 0 ? texture : secondTexture, 1 );
            frame.Commands.Draw( 3 );
        } );
        var diagnostics = device.GetDiagnostics( );
        foreach ( string message in diagnostics.Messages ) Console.WriteLine( message );
        if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( x => x.Contains( "SEVERITY_ERROR" ) || x.Contains( "SEVERITY_CORRUPTION" ) ) ) return 1;
        if ( frameLimit > 0 && result.Frames != frameLimit ) return 1;
        Console.WriteLine( $"HelloTexture closed cleanly after {result.Frames} frames." );
        return 0;
    }

    private static TextureSubresourceData[] Mips( int size, bool alternate ) {
        var data = new TextureSubresourceData[3];
        for ( int mip = 0; mip < data.Length; ++mip ) {
            int width = Math.Max( 1, size >> mip );
            var pixels = Checkerboard( width );
            for ( int i = 0; i < pixels.Length; i += 4 ) {
                if ( alternate ) (pixels[i], pixels[i + 2]) = (pixels[i + 2], pixels[i]);
                if ( mip > 0 ) pixels[i + (mip == 1 ? 0 : 2)] = 220;
            }
            data[mip] = new( pixels, (uint)width * 4 );
        }
        return data;
    }

    private static byte[] Checkerboard( int size ) {
        byte[] pixels = new byte[size * size * 4];
        for ( int y = 0; y < size; ++y ) {
            for ( int x = 0; x < size; ++x ) {
                int i = (y * size + x) * 4;
                bool light = ((x * 8 / size ^ y * 8 / size) & 1) == 0;
                pixels[i] = light ? (byte)239 : (byte)24;
                pixels[i + 1] = light ? (byte)245 : (byte)107;
                pixels[i + 2] = light ? (byte)231 : (byte)160;
                pixels[i + 3] = 255;
            }
        }
        return pixels;
    }
}
