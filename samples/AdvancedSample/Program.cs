using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace AdvancedDXS;

internal static class Program {
    private const string Shader = """
        cbuffer Scene : register(b0) {
            row_major float4x4 WorldViewProjection;
            row_major float4x4 World;
        };
        struct Vertex { float4 position : SV_Position; float3 color : COLOR; };
        Vertex VS(uint id : SV_VertexID) {
            const float3 corners[8] = {
                float3(-1,-1,-1), float3(1,-1,-1), float3(1,1,-1), float3(-1,1,-1),
                float3(-1,-1, 1), float3(1,-1, 1), float3(1,1, 1), float3(-1,1, 1)
            };
            const uint indices[36] = {
                0,3,2, 0,2,1,  4,5,6, 4,6,7,  0,4,7, 0,7,3,
                1,2,6, 1,6,5,  3,7,6, 3,6,2,  0,1,5, 0,5,4
            };
            const float3 normals[6] = {
                float3(0,0,-1), float3(0,0,1), float3(-1,0,0),
                float3(1,0,0), float3(0,1,0), float3(0,-1,0)
            };
            const float3 colors[6] = {
                float3(0.18,0.7,0.92), float3(0.27,0.85,0.68), float3(0.25,0.4,0.82),
                float3(0.97,0.55,0.22), float3(0.8,0.9,1), float3(0.62,0.34,0.88)
            };
            uint face = id / 6;
            float3 normal = mul(normals[face], (float3x3)World);
            float light = 0.28 + 0.72 * saturate(dot(normal, normalize(float3(0.45, 0.8, 0.65))));
            Vertex output;
            output.position = mul(float4(corners[indices[id]], 1), WorldViewProjection);
            output.color = colors[face] * light;
            return output;
        }
        float4 PS(Vertex input) : SV_Target { return float4(input.color, 1); }
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
        using var compiler = new ShaderCompiler( );
        using var root = device.CreateGraphicsConstantsRootSignature( 32 );
        using var pipeline = device.CreateGraphicsPipeline( root, compiler.Compile( Shader, "VS", "vs_6_0" ),
            compiler.Compile( Shader, "PS", "ps_6_0" ), PixelFormat.Rgba8Unorm, depthEnabled: true );

        var view = Matrix4x4.CreateLookAt( new Vector3( 0, 1.4f, 5.8f ), Vector3.Zero, Vector3.UnitY );
        float angle = 0.65f;
        var result = DesktopGame.Run( device, new( ) {
            Title = "DXSharp · Advanced Sample · Lit Cube", FrameLimit = frameLimit, EnableDepth = true
        }, frame => {
            angle += Math.Min( (float)frame.Elapsed.TotalSeconds, 0.1f ) * 0.9f;
            var world = Matrix4x4.CreateRotationX( 0.45f ) * Matrix4x4.CreateRotationY( angle );
            var projection = Matrix4x4.CreatePerspectiveFieldOfView( MathF.PI / 3, (float)frame.Size.Width / frame.Size.Height, 0.1f, 100 );
            ReadOnlySpan<Matrix4x4> matrices = [world * view * projection, world];
            frame.Commands.SetGraphicsPipeline( pipeline );
            frame.Commands.SetGraphicsConstants( MemoryMarshal.Cast<Matrix4x4, uint>( matrices ) );
            frame.Commands.Draw( 36 );
        } );
        var diagnostics = device.GetDiagnostics( );
        foreach ( string message in diagnostics.Messages ) Console.WriteLine( message );
        if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( x => x.Contains( "SEVERITY_ERROR" ) || x.Contains( "SEVERITY_CORRUPTION" ) ) ) return 1;
        if ( frameLimit > 0 && result.Frames != frameLimit ) return 1;
        Console.WriteLine( $"AdvancedSample closed cleanly after {result.Frames} frames." );
        return 0;
    }
}
