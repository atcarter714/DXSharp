using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace LitCube;

internal static class Program {
    private const string Shader = """
        cbuffer Scene : register(b0) {
            row_major float4x4 WorldViewProjection;
            row_major float4x4 World;
        };
        struct Vertex {
            float4 position : SV_Position;
            float3 worldPosition : WORLDPOS;
            float3 normal : NORMAL;
            float3 color : COLOR;
        };
        struct VertexInput { float3 position : POSITION; float3 normal : NORMAL; float3 color : COLOR; };
        Vertex VS(VertexInput input) {
            Vertex output;
            output.position = mul(float4(input.position, 1), WorldViewProjection);
            output.worldPosition = mul(float4(input.position, 1), World).xyz;
            output.normal = mul(input.normal, (float3x3)World);
            output.color = input.color;
            return output;
        }
        float4 PS(Vertex input) : SV_Target {
            float3 n = normalize(input.normal);
            float3 v = normalize(float3(0, 1.4, 5.8) - input.worldPosition);
            float3 l = normalize(float3(-3.5, 4.0, 4.0) - input.worldPosition);
            float ndotl = saturate(dot(n, l));
            float3 h = normalize(l + v);
            float specular = pow(saturate(dot(n, h)), 48) * ndotl;
            float fill = saturate(dot(n, normalize(float3(1, 0.25, -0.6))));
            float hemisphere = n.y * 0.5 + 0.5;
            float3 ambient = lerp(float3(0.025, 0.03, 0.05), float3(0.12, 0.16, 0.22), hemisphere);
            float3 linearColor = input.color * (ambient + float3(1.05, 0.9, 0.7) * ndotl
                + float3(0.1, 0.2, 0.34) * fill) + float3(1.0, 0.92, 0.8) * specular * 0.65;
            // The target is UNORM, so perform the linear-to-sRGB transfer here.
            linearColor = saturate(linearColor);
            float3 srgb = lerp(12.92 * linearColor, 1.055 * pow(linearColor, 1.0 / 2.4) - 0.055,
                step(0.0031308, linearColor));
            return float4(srgb, 1);
        }
        """;

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct CubeVertex( Vector3 Position, Vector3 Normal, Vector3 Color );

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
            compiler.Compile( Shader, "PS", "ps_6_0" ), [
                new( "POSITION", VertexFormat.Float3, 0 ),
                new( "NORMAL", VertexFormat.Float3, 12 ),
                new( "COLOR", VertexFormat.Float3, 24 )
            ], depthEnabled: true );
        using var mesh = UploadCube( device );

        var view = Matrix4x4.CreateLookAt( new Vector3( 0, 1.4f, 5.8f ), Vector3.Zero, Vector3.UnitY );
        float angle = 0.65f;
        var result = DesktopGame.Run( device, new( ) {
            Title = "DXSharp · Lit Cube", FrameLimit = frameLimit, EnableDepth = true
        }, frame => {
            angle += Math.Min( (float)frame.Elapsed.TotalSeconds, 0.1f ) * 0.9f;
            var world = Matrix4x4.CreateRotationX( 0.45f ) * Matrix4x4.CreateRotationY( angle );
            var projection = Matrix4x4.CreatePerspectiveFieldOfView( MathF.PI / 3, (float)frame.Size.Width / frame.Size.Height, 0.1f, 100 );
            ReadOnlySpan<Matrix4x4> matrices = [world * view * projection, world];
            frame.Commands.SetGraphicsPipeline( pipeline );
            frame.Commands.SetGraphicsConstants( MemoryMarshal.Cast<Matrix4x4, uint>( matrices ) );
            frame.Commands.DrawMesh( mesh );
        } );
        var diagnostics = device.GetDiagnostics( );
        foreach ( string message in diagnostics.Messages ) Console.WriteLine( message );
        if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( x => x.Contains( "SEVERITY_ERROR" ) || x.Contains( "SEVERITY_CORRUPTION" ) ) ) return 1;
        if ( frameLimit > 0 && result.Frames != frameLimit ) return 1;
        Console.WriteLine( $"LitCube closed cleanly after {result.Frames} frames." );
        return 0;
    }
    private static Mesh UploadCube( GraphicsDevice device ) {
        ReadOnlySpan<Vector3> corners = [
            new(-1,-1,-1), new(1,-1,-1), new(1,1,-1), new(-1,1,-1),
            new(-1,-1, 1), new(1,-1, 1), new(1,1, 1), new(-1,1, 1)
        ];
        ReadOnlySpan<int> faces = [0,3,2,1, 4,5,6,7, 0,4,7,3, 1,2,6,5, 3,7,6,2, 0,1,5,4];
        ReadOnlySpan<Vector3> normals = [new(0,0,-1), new(0,0,1), new(-1,0,0), new(1,0,0), new(0,1,0), new(0,-1,0)];
        ReadOnlySpan<Vector3> colors = [
            new(0.18f,0.7f,0.92f), new(0.27f,0.85f,0.68f), new(0.25f,0.4f,0.82f),
            new(0.97f,0.55f,0.22f), new(0.8f,0.9f,1), new(0.62f,0.34f,0.88f)
        ];
        Span<CubeVertex> vertices = stackalloc CubeVertex[24];
        Span<ushort> indices = stackalloc ushort[36];
        for ( int face = 0; face < 6; ++face ) {
            for ( int corner = 0; corner < 4; ++corner )
                vertices[face * 4 + corner] = new( corners[faces[face * 4 + corner]], normals[face], colors[face] );
            ushort first = (ushort)(face * 4);
            ReadOnlySpan<ushort> faceIndices = [first, (ushort)(first + 1), (ushort)(first + 2),
                first, (ushort)(first + 2), (ushort)(first + 3)];
            faceIndices.CopyTo( indices.Slice( face * 6, 6 ) );
        }
        using var queue = device.CreateCommandQueue( );
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        var mesh = device.CreateMesh<CubeVertex>( commands, vertices, indices );
        try {
            commands.Close( );
            queue.Wait( queue.Submit( commands ) );
            return mesh;
        } catch {
            mesh.Dispose( );
            throw;
        }
    }

}
