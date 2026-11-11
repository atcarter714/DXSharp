using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using DXSharp.Assets;

internal static class AssetQualification {
    internal static void Run( ) {
        string path = Path.Combine( AppContext.BaseDirectory, "Models", "Patrol.gltf" );
        var model = GltfModel.Load( path );
        Require( Marshal.SizeOf<ModelVertex>( ) == 36, "model vertex ABI" );
        Require( model.Primitives.Length == 3 && model.Instances.Length == 9, "three shared primitives for three ships" );

        for ( int i = 0; i < model.Instances.Length; ++i )
            Require( model.Instances[i].PrimitiveIndex == i % 3, "instance-to-primitive mapping" );
        Require( Near( model.Instances[0].WorldTransform.Translation, new( 1.15f, 0.25f, -1 ) ), "lead transform" );
        Require( Near( model.Instances[3].WorldTransform.Translation, new( 3.1f, 0, 2.1f ) ), "nested port transform" );
        Require( Near( model.Instances[6].WorldTransform.Translation, new( 3.1f, 0.45f, -4.1f ) ) &&
            model.Instances[6].WorldTransform.GetDeterminant( ) < 0, "mirrored starboard transform" );
        Require( Near( model.BoundsMin, new( -0.95f, -0.2016f, -5.1944f ) ) &&
            Near( model.BoundsMax, new( 4, 0.8532f, 3.1944f ) ), "transformed bounds" );

        var hull = model.Primitives[0];
        Require( hull.Indices.Length > hull.Vertices.Length && Near( hull.Vertices[0].Color, new( 0.11f, 0.27f, 0.48f ) ),
            "indexed geometry and material tint" );
        for ( int i = 1; i < model.Primitives.Length; ++i ) {
            var primitive = model.Primitives[i];
            Require( primitive.Vertices.Length == primitive.Indices.Length, "flat-normal geometry expanded per triangle" );
            for ( int j = 0; j < primitive.Indices.Length; j += 3 ) {
                var a = primitive.Vertices[j];
                var b = primitive.Vertices[j + 1];
                var c = primitive.Vertices[j + 2];
                var normal = Vector3.Normalize( Vector3.Cross( b.Position - a.Position, c.Position - a.Position ) );
                Require( primitive.Indices[j] == j && primitive.Indices[j + 1] == j + 1 && primitive.Indices[j + 2] == j + 2 &&
                    Near( a.Normal, normal ) && a.Normal == b.Normal && a.Normal == c.Normal, "flat normals follow triangle winding" );
            }
        }
        Require( Near( model.Primitives[2].Vertices[0].Color, new( 0.08f, 0.52f * 230 / 255, 1 ) ),
            "normalized vertex color multiplied by the material tint" );

        string json = File.ReadAllText( path );
        var document = JsonNode.Parse( json )!;
        using var glb = CreateGlb( document );
        var binary = GltfModel.Read( glb );
        Require( glb.CanRead && model.Instances.SequenceEqual( binary.Instances ) &&
            model.BoundsMin == binary.BoundsMin && model.BoundsMax == binary.BoundsMax, "GLB scene and stream ownership" );
        for ( int i = 0; i < model.Primitives.Length; ++i ) {
            Require( model.Primitives[i].Vertices.SequenceEqual( binary.Primitives[i].Vertices ) &&
                model.Primitives[i].Indices.SequenceEqual( binary.Primitives[i].Indices ), "GLB geometry matches JSON glTF" );
        }

        document["buffers"]![0]!["uri"] = "external.bin";
        RequireUnsupported( document, "External glTF resource" );
        document = JsonNode.Parse( json )!;
        document["materials"]![0]!["alphaMode"] = "BLEND";
        RequireUnsupported( document, "opaque" );
        Console.WriteLine( "PASS: static glTF and GLB geometry, transforms, material colors, and unsupported resources validated." );
    }

    private static MemoryStream CreateGlb( JsonNode document ) {
        var buffer = document["buffers"]![0]!.AsObject( );
        string uri = buffer["uri"]!.GetValue<string>( );
        byte[] bin = Convert.FromBase64String( uri[(uri.IndexOf( ',' ) + 1)..] );
        buffer.Remove( "uri" );
        byte[] json = Encoding.UTF8.GetBytes( document.ToJsonString( ) );
        int jsonLength = (json.Length + 3) & ~3;
        int binLength = (bin.Length + 3) & ~3;
        var stream = new MemoryStream( );
        using ( var writer = new BinaryWriter( stream, Encoding.UTF8, leaveOpen: true ) ) {
            writer.Write( 0x46546C67u );
            writer.Write( 2u );
            writer.Write( 28 + jsonLength + binLength );
            writer.Write( jsonLength );
            writer.Write( 0x4E4F534Au );
            writer.Write( json );
            for ( int i = json.Length; i < jsonLength; ++i ) writer.Write( (byte)' ' );
            writer.Write( binLength );
            writer.Write( 0x004E4942u );
            writer.Write( bin );
            for ( int i = bin.Length; i < binLength; ++i ) writer.Write( (byte)0 );
        }
        stream.Position = 0;
        return stream;
    }

    private static void RequireUnsupported( JsonNode document, string message ) {
        using var stream = new MemoryStream( Encoding.UTF8.GetBytes( document.ToJsonString( ) ) );
        try {
            GltfModel.Read( stream );
        } catch ( NotSupportedException error ) when ( error.Message.Contains( message, StringComparison.Ordinal ) ) {
            return;
        }
        throw new InvalidOperationException( $"Static asset qualification failed to reject {message}." );
    }

    private static bool Near( Vector3 actual, Vector3 expected ) => Vector3.DistanceSquared( actual, expected ) < 1e-8f;

    private static void Require( bool condition, string check ) {
        if ( !condition ) throw new InvalidOperationException( $"Static asset qualification failed: {check}." );
    }
}
