using DXSharp.Assets;
using DXSharp.Graphics;

namespace DXSharp.Samples;

internal sealed class StaticMeshSet : IDisposable {
    private readonly Mesh[] _meshes;
    private readonly SampledTexture2D[] _materials;
    private readonly Dictionary<StaticTexture, SampledTexture2D> _textures = new( ReferenceEqualityComparer.Instance );
    private SampledTexture2D? _white;

    internal StaticMeshSet( GraphicsDevice device, GraphicsCommandList commands, StaticModel model, float scale = 1 ) {
        _meshes = new Mesh[model.Primitives.Length];
        _materials = new SampledTexture2D[_meshes.Length];
        try {
            for ( int i = 0; i < _meshes.Length; ++i ) {
                var primitive = model.Primitives[i];
                var vertices = new TexturedModelVertex[primitive.Vertices.Length];
                primitive.CopyTexturedVertices( vertices );
                if ( scale != 1 ) {
                    for ( int v = 0; v < vertices.Length; ++v )
                        vertices[v] = vertices[v] with { Position = vertices[v].Position * scale };
                }
                _materials[i] = Material( primitive.BaseColorTexture );
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

    internal void Draw( GraphicsCommandList commands, int primitive, uint count = 1, uint start = 0 ) {
        commands.SetGraphicsTexture( _materials[primitive], 1 );
        commands.DrawMesh( _meshes[primitive], count, start );
    }

    public void Dispose( ) {
        foreach ( var mesh in _meshes ) mesh?.Dispose( );
        foreach ( var texture in _textures.Values ) texture.Dispose( );
        _white?.Dispose( );
    }
}
