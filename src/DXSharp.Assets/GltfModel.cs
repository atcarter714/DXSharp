using System.Numerics;
using SharpGLTF.Schema2;
using SharpGLTF.Validation;

namespace DXSharp.Assets;

/// <summary>Imports self-contained glTF 2.0 and GLB files as static triangle meshes.</summary>
/// <remarks>
/// The importer uses the default scene, or the first scene if no default is declared. It preserves
/// local geometry, node transforms, and linear base colors for opaque materials. Materials are reduced
/// to base color. Textures, skinning, animation, morph targets, and required extensions are unsupported.
/// </remarks>
public static class GltfModel {
    /// <summary>Loads a self-contained model from disk.</summary>
    /// <param name="path">The path to a glTF JSON or GLB file.</param>
    /// <returns>The geometry and instances of the selected scene.</returns>
    public static StaticModel Load( string path ) {
        using var stream = File.OpenRead( path );
        return Read( stream );
    }

    /// <summary>Reads a self-contained model at the current position of a seekable stream.</summary>
    /// <param name="stream">The input stream, which remains open.</param>
    /// <returns>The geometry and instances of the selected scene.</returns>
    /// <exception cref="ArgumentException">The stream cannot be read or seeked.</exception>
    /// <exception cref="InvalidDataException">The model has invalid data or no renderable scene.</exception>
    /// <exception cref="NotSupportedException">The model uses features outside the static import format.</exception>
    public static StaticModel Read( Stream stream ) {
        ArgumentNullException.ThrowIfNull( stream );
        if ( !stream.CanRead || !stream.CanSeek )
            throw new ArgumentException( "glTF input must be readable and seekable.", nameof( stream ) );

        var reader = ReadContext.Create( name =>
            throw new NotSupportedException( $"External glTF resource '{name}' requires a self-contained glTF or GLB export." ) );
        reader.Validation = ValidationMode.Strict;
        using var input = new BorrowedStream( stream );
        ModelRoot model;
        try {
            model = reader.ReadSchema2( input );
        } catch ( ModelException error ) {
            throw new InvalidDataException( "The glTF asset failed validation.", error );
        }

        if ( model.ExtensionsRequired.FirstOrDefault( ) is { } extension )
            throw new NotSupportedException( $"The glTF asset requires unsupported extension '{extension}'." );
        if ( model.LogicalAnimations.Count != 0 )
            throw new NotSupportedException( "Animation is outside the static glTF import format." );

        var scene = model.DefaultScene ?? model.LogicalScenes.FirstOrDefault( )
            ?? throw new InvalidDataException( "The glTF asset does not contain a scene." );
        var primitives = new List<StaticModelPrimitive>( );
        var bounds = new List<(Vector3 Min, Vector3 Max)>( );
        var instances = new List<StaticModelInstance>( );
        var primitiveIndices = new Dictionary<MeshPrimitive, int>( ReferenceEqualityComparer.Instance );
        var nodes = new Stack<(Node Node, Matrix4x4 Parent)>( );
        foreach ( var node in scene.VisualChildren.Reverse( ) )
            nodes.Push( (node, Matrix4x4.Identity) );

        var min = new Vector3( float.PositiveInfinity );
        var max = new Vector3( float.NegativeInfinity );
        while ( nodes.TryPop( out var entry ) ) {
            var (node, parent) = entry;
            var world = node.LocalMatrix * parent;
            foreach ( var child in node.VisualChildren.Reverse( ) )
                nodes.Push( (child, world) );

            if ( node.Mesh is not { } mesh ) continue;
            if ( node.Skin is not null || node.GetGpuInstancing( ) is not null )
                throw new NotSupportedException( $"Node {node.LogicalIndex} uses skinning or a GPU instancing extension." );
            float determinant = world.GetDeterminant( );
            if ( !float.IsFinite( determinant ) || determinant == 0 )
                throw new InvalidDataException( $"Node {node.LogicalIndex} has a non-invertible scene transform." );

            foreach ( var primitive in mesh.Primitives ) {
                if ( !primitiveIndices.TryGetValue( primitive, out int index ) ) {
                    index = primitives.Count;
                    var geometry = ReadPrimitive( primitive );
                    primitives.Add( geometry );
                    primitiveIndices.Add( primitive, index );
                    var localMin = new Vector3( float.PositiveInfinity );
                    var localMax = new Vector3( float.NegativeInfinity );
                    foreach ( var vertex in geometry.Vertices ) {
                        localMin = Vector3.Min( localMin, vertex.Position );
                        localMax = Vector3.Max( localMax, vertex.Position );
                    }
                    bounds.Add( (localMin, localMax) );
                }

                instances.Add( new( index, world ) );
                var (lower, upper) = bounds[index];
                for ( int corner = 0; corner < 8; ++corner ) {
                    var position = Vector3.Transform( new Vector3(
                        (corner & 1) == 0 ? lower.X : upper.X,
                        (corner & 2) == 0 ? lower.Y : upper.Y,
                        (corner & 4) == 0 ? lower.Z : upper.Z ), world );
                    if ( !float.IsFinite( position.X ) || !float.IsFinite( position.Y ) || !float.IsFinite( position.Z ) )
                        throw new InvalidDataException( $"Node {node.LogicalIndex} produces non-finite scene bounds." );
                    min = Vector3.Min( min, position );
                    max = Vector3.Max( max, position );
                }
            }
        }

        if ( instances.Count == 0 )
            throw new InvalidDataException( "The selected glTF scene does not contain triangle meshes." );
        return new( primitives.ToArray( ), instances.ToArray( ), min, max );
    }

    private static StaticModelPrimitive ReadPrimitive( MeshPrimitive primitive ) {
        if ( primitive.DrawPrimitiveType != PrimitiveType.TRIANGLES || primitive.MorphTargetsCount != 0 )
            throw new NotSupportedException( "Static glTF meshes require triangle lists without morph targets." );

        var material = primitive.Material;
        if ( material is not null && (material.Alpha != AlphaMode.OPAQUE || material.Channels.Any( x => x.Texture is not null )) )
            throw new NotSupportedException( "Static glTF meshes require opaque, untextured materials." );
        var tint = material?.FindChannel( "BaseColor" )?.Color ?? Vector4.One;

        var positions = primitive.GetVertexAccessor( "POSITION" )?.AsVector3Array( )
            ?? throw new InvalidDataException( "A glTF primitive is missing POSITION data." );
        var normals = primitive.GetVertexAccessor( "NORMAL" )?.AsVector3Array( );
        var colors = primitive.GetVertexAccessor( "COLOR_0" )?.AsColorArray( 1 );
        var sourceIndices = primitive.GetIndices( );
        var indices = new uint[sourceIndices?.Count ?? positions.Count];
        if ( indices.Length == 0 || indices.Length % 3 != 0 )
            throw new InvalidDataException( "A glTF triangle list must contain complete triangles." );

        for ( int i = 0; i < indices.Length; ++i ) {
            uint index = sourceIndices is null ? (uint)i : sourceIndices[i];
            if ( index >= positions.Count )
                throw new InvalidDataException( $"glTF index {index} exceeds the vertex count {positions.Count}." );
            indices[i] = index;
        }

        var vertices = new ModelVertex[normals is null ? indices.Length : positions.Count];
        if ( normals is not null ) {
            for ( int i = 0; i < vertices.Length; ++i )
                vertices[i] = new( positions[i], normals[i], BaseColor( i ) );
        } else {
            for ( int i = 0; i < indices.Length; i += 3 ) {
                var a = positions[(int)indices[i]];
                var b = positions[(int)indices[i + 1]];
                var c = positions[(int)indices[i + 2]];
                var cross = Vector3.Cross( b - a, c - a );
                // Scale before normalizing to avoid squaring a very large or very small cross product.
                var magnitude = Vector3.Abs( cross );
                float scale = MathF.Max( magnitude.X, MathF.Max( magnitude.Y, magnitude.Z ) );
                var normal = scale > 0 && float.IsFinite( scale ) ? Vector3.Normalize( cross / scale ) : Vector3.UnitY;
                vertices[i] = new( a, normal, BaseColor( (int)indices[i] ) );
                vertices[i + 1] = new( b, normal, BaseColor( (int)indices[i + 1] ) );
                vertices[i + 2] = new( c, normal, BaseColor( (int)indices[i + 2] ) );
                indices[i] = (uint)i;
                indices[i + 1] = (uint)(i + 1);
                indices[i + 2] = (uint)(i + 2);
            }
        }
        return new( vertices, indices );

        Vector3 BaseColor( int index ) {
            var rgba = tint * (colors is null ? Vector4.One : colors[index]);
            return new( rgba.X, rgba.Y, rgba.Z );
        }
    }

    // The GLB reader disposes its input. Keep the caller's stream open without copying it.
    private sealed class BorrowedStream( Stream source ) : Stream {
        public override bool CanRead => source.CanRead;
        public override bool CanSeek => source.CanSeek;
        public override bool CanWrite => false;
        public override long Length => source.Length;
        public override long Position { get => source.Position; set => source.Position = value; }
        public override int Read( byte[] buffer, int offset, int count ) => source.Read( buffer, offset, count );
        public override int Read( Span<byte> buffer ) => source.Read( buffer );
        public override int ReadByte( ) => source.ReadByte( );
        public override long Seek( long offset, SeekOrigin origin ) => source.Seek( offset, origin );
        public override void Flush( ) { }
        public override void SetLength( long value ) => throw new NotSupportedException( );
        public override void Write( byte[] buffer, int offset, int count ) => throw new NotSupportedException( );
    }
}
