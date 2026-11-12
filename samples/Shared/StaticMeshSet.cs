using System.Numerics;
using DXSharp.Assets;
using DXSharp.Graphics;

namespace DXSharp.Samples;

internal sealed class StaticMeshSet : IDisposable {
    private readonly Mesh[] _meshes;
    private readonly SampledTexture2D[] _materials;
    private readonly StaticMaterial[] _surfaces;
    private readonly Dictionary<StaticTexture, SampledTexture2D> _textures = new( ReferenceEqualityComparer.Instance );
    private SampledTexture2D? _white;

    internal StaticMeshSet( GraphicsDevice device, GraphicsCommandList commands, StaticModel model, float scale = 1, bool flattenScene = false ) {
        _meshes = new Mesh[flattenScene ? model.Instances.Length : model.Primitives.Length];
        _materials = new SampledTexture2D[_meshes.Length];
        _surfaces = new StaticMaterial[_meshes.Length];
        try {
            for ( int i = 0; i < _meshes.Length; ++i ) {
                var primitive = model.Primitives[flattenScene ? model.Instances[i].PrimitiveIndex : i];
                var vertices = new TexturedModelVertex[primitive.Vertices.Length];
                primitive.CopyTexturedVertices( vertices );
                if ( flattenScene ) {
                    var world = model.Instances[i].WorldTransform;
                    if ( !Matrix4x4.Invert( world, out var inverse ) ) throw new InvalidDataException( "Non-invertible mesh transform." );
                    var normalTransform = Matrix4x4.Transpose( inverse );
                    for ( int v = 0; v < vertices.Length; ++v )
                        vertices[v] = vertices[v] with {
                            Position = Vector3.Transform( vertices[v].Position, world ) * scale,
                            Normal = Vector3.Normalize( Vector3.TransformNormal( vertices[v].Normal, normalTransform ) )
                        };
                } else if ( scale != 1 ) {
                    for ( int v = 0; v < vertices.Length; ++v )
                        vertices[v] = vertices[v] with { Position = vertices[v].Position * scale };
                }
                _materials[i] = Material( primitive.BaseColorTexture );
                _surfaces[i] = primitive.Surface;
                if ( vertices.Length <= ushort.MaxValue + 1 ) {
                    var indices = new ushort[primitive.Indices.Length];
                    for ( int j = 0; j < indices.Length; ++j ) indices[j] = (ushort)primitive.Indices[j];
                    _meshes[i] = device.CreateMesh<TexturedModelVertex>( commands, vertices, indices );
                } else {
                    _meshes[i] = device.CreateMesh<TexturedModelVertex>( commands, vertices, primitive.Indices );
                }
            }
        } catch {
            Dispose( );
            throw;
        }

        SampledTexture2D Material( StaticTexture? image ) {
            if ( image is null ) return _white ??= device.CreateSampledTexture2D( commands, 1, 1, [255, 255, 255, 255], srgb: true );
            if ( !_textures.TryGetValue( image, out var texture ) ) {
                texture = device.CreateSampledTexture2D( commands, (uint)image.Width, (uint)image.Height, image.Pixels, srgb: true );
                _textures.Add( image, texture );
            }
            return texture;
        }
    }

    internal int Count => _meshes.Length;

    internal void Draw( GraphicsCommandList commands, int primitive, uint materialOffset, uint count = 1, uint start = 0 ) {
        var surface = _surfaces[primitive];
        commands.SetGraphicsConstants( [BitConverter.SingleToUInt32Bits( surface.Metallic ), BitConverter.SingleToUInt32Bits( surface.Roughness ), 0, 0,
            BitConverter.SingleToUInt32Bits( surface.Emissive.X ), BitConverter.SingleToUInt32Bits( surface.Emissive.Y ),
            BitConverter.SingleToUInt32Bits( surface.Emissive.Z ), 0], materialOffset );
        commands.SetGraphicsTexture( _materials[primitive], 1 );
        commands.DrawMesh( _meshes[primitive], count, start );
    }

    public void Dispose( ) {
        foreach ( var mesh in _meshes ) mesh?.Dispose( );
        foreach ( var texture in _textures.Values ) texture.Dispose( );
        _white?.Dispose( );
    }
}
