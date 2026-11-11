using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Assets;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace StaticModelViewer;

internal static class Program {
    private const string Shader = """
        cbuffer Scene : register(b0) {
            row_major float4x4 WorldViewProjection;
            row_major float4x4 NormalToWorld;
        };
        struct VertexInput { float3 position : POSITION; float3 normal : NORMAL; float3 color : COLOR; };
        struct Vertex { float4 position : SV_Position; float3 normal : NORMAL; float3 color : COLOR; };
        Vertex VS(VertexInput input) {
            Vertex output;
            output.position = mul(float4(input.position, 1), WorldViewProjection);
            output.normal = mul(input.normal, (float3x3)NormalToWorld);
            output.color = input.color;
            return output;
        }
        float4 PS(Vertex input) : SV_Target {
            float diffuse = saturate(dot(normalize(input.normal), normalize(float3(-0.55, 0.8, 0.35))));
            float3 litColor = saturate(input.color * (0.24 + 0.76 * diffuse));
            return float4(pow(litColor, 1.0 / 2.2), 1);
        }
        """;

    [STAThread]
    public static int Main( string[] args ) {
        try {
            return Run( args );
        } catch ( Exception error ) {
            Console.Error.WriteLine( error );
            return 1;
        }
    }

    private static int Run( string[] args ) {
        Application.SetUnhandledExceptionMode( UnhandledExceptionMode.ThrowException );
        ApplicationConfiguration.Initialize( );
#if DEBUG
        const bool debugLayer = true;
#else
        const bool debugLayer = false;
#endif
        int frameLimit = int.Parse( args.FirstOrDefault( x => x.StartsWith( "--frames=", StringComparison.Ordinal ) )?[9..] ?? "0" );
        ArgumentOutOfRangeException.ThrowIfNegative( frameLimit );
        string path = args.FirstOrDefault( x => x.StartsWith( "--model=", StringComparison.Ordinal ) )?[8..]
            ?? Path.Combine( AppContext.BaseDirectory, "Models", "Patrol.gltf" );
        var model = GltfModel.Load( path );
        Console.WriteLine( $"Loaded {model.Primitives.Length} shared primitives and {model.Instances.Length} instances from {Path.GetFileName( path )}." );

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

        var meshes = new Mesh[model.Primitives.Length];
        try {
            Upload( device, model, meshes );
            var result = Render( device, pipeline, model, meshes, frameLimit );
            var diagnostics = device.GetDiagnostics( );
            foreach ( string message in diagnostics.Messages ) Console.WriteLine( message );
            if ( diagnostics.DeviceRemovedReason < 0 ||
                diagnostics.Messages.Any( x => x.Contains( "SEVERITY_ERROR" ) || x.Contains( "SEVERITY_CORRUPTION" ) ) ) return 1;
            if ( frameLimit > 0 && result != frameLimit ) return 1;
            Console.WriteLine( $"StaticModelViewer closed cleanly after {result} frames." );
            return 0;
        } finally {
            foreach ( var mesh in meshes ) mesh?.Dispose( );
        }
    }

    private static void Upload( GraphicsDevice device, StaticModel model, Mesh[] meshes ) {
        using var queue = device.CreateCommandQueue( );
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        for ( int i = 0; i < meshes.Length; ++i ) {
            var primitive = model.Primitives[i];
            if ( primitive.Vertices.Length <= ushort.MaxValue + 1 ) {
                var indices = new ushort[primitive.Indices.Length];
                for ( int j = 0; j < indices.Length; ++j ) indices[j] = (ushort)primitive.Indices[j];
                meshes[i] = device.CreateMesh( commands, primitive.Vertices, indices );
            } else {
                meshes[i] = device.CreateMesh( commands, primitive.Vertices, primitive.Indices );
            }
        }
        commands.Close( );
        queue.Wait( queue.Submit( commands ) );
    }

    private static int Render( GraphicsDevice device, PipelineState pipeline, StaticModel model, Mesh[] meshes, int frameLimit ) {
        var center = model.BoundsMin * 0.5f + model.BoundsMax * 0.5f;
        float extent = MathF.Max( Vector3.Distance( center, model.BoundsMax ), 0.001f );
        if ( !float.IsFinite( extent ) )
            throw new InvalidDataException( "The model's dimensions exceed the viewer's numeric range." );
        var centerModel = Matrix4x4.CreateTranslation( -center ) * Matrix4x4.CreateScale( 1 / extent );
        var draws = new (Mesh Mesh, Matrix4x4 World, Matrix4x4 NormalToWorld)[model.Instances.Length];
        for ( int i = 0; i < draws.Length; ++i ) {
            var instance = model.Instances[i];
            if ( !Matrix4x4.Invert( instance.WorldTransform, out var inverse ) )
                throw new InvalidDataException( "The model has a non-invertible instance transform." );
            draws[i] = (meshes[instance.PrimitiveIndex], instance.WorldTransform * centerModel, Matrix4x4.Transpose( inverse ));
        }
        float angle = 2.4f;
        var result = DesktopGame.Run( device, new( ) {
            Title = "DXSharp · Static Model Viewer", FrameLimit = frameLimit, EnableDepth = true
        }, frame => {
            angle += Math.Min( (float)frame.Elapsed.TotalSeconds, 0.1f ) * 0.28f;
            float aspect = (float)frame.Size.Width / frame.Size.Height;
            float distance = 3.2f / MathF.Min( aspect, 1 );
            var eye = new Vector3( MathF.Cos( angle ), 0.63f, MathF.Sin( angle ) ) * distance;
            var view = Matrix4x4.CreateLookAt( eye, Vector3.Zero, Vector3.UnitY );
            var projection = Matrix4x4.CreatePerspectiveFieldOfView( MathF.PI / 3, aspect, 0.1f, eye.Length( ) + 4 );
            var viewProjection = view * projection;
            frame.Commands.SetGraphicsPipeline( pipeline );
            Span<Matrix4x4> matrices = stackalloc Matrix4x4[2];
            foreach ( var draw in draws ) {
                matrices[0] = draw.World * viewProjection;
                matrices[1] = draw.NormalToWorld;
                frame.Commands.SetGraphicsConstants( MemoryMarshal.Cast<Matrix4x4, uint>( matrices ) );
                frame.Commands.DrawMesh( draw.Mesh );
            }
        } );
        return result.Frames;
    }
}
