using System.Numerics;
using System.Runtime.InteropServices;

namespace DXSharp.Assets;

/// <summary>A model vertex with a position, unit normal, and linear RGB base color.</summary>
/// <param name="Position">The position in mesh-local coordinates.</param>
/// <param name="Normal">The unit normal in mesh-local coordinates.</param>
/// <param name="Color">The linear RGB base color, including the material's tint.</param>
[StructLayout( LayoutKind.Sequential )]
public readonly record struct ModelVertex( Vector3 Position, Vector3 Normal, Vector3 Color );

/// <summary>Decoded sRGB base color pixels shared by the primitives that reference an image.</summary>
public sealed class StaticTexture {
    private readonly byte[] _pixels;

    internal StaticTexture( int width, int height, byte[] pixels ) {
        Width = width;
        Height = height;
        _pixels = pixels;
    }

    /// <summary>Gets the image width in pixels.</summary>
    public int Width { get; }

    /// <summary>Gets the image height in pixels.</summary>
    public int Height { get; }

    /// <summary>Gets tightly packed, top-to-bottom RGBA8 pixels with sRGB RGB channels.</summary>
    public ReadOnlySpan<byte> Pixels => _pixels;
}

/// <summary>A model vertex extended with normalized base color texture coordinates.</summary>
/// <param name="Position">The mesh-local position.</param>
/// <param name="Normal">The mesh-local unit normal.</param>
/// <param name="Color">The linear vertex and material tint.</param>
/// <param name="TexCoord">The base color UV.</param>
[StructLayout( LayoutKind.Sequential )]
public readonly record struct TexturedModelVertex( Vector3 Position, Vector3 Normal, Vector3 Color, Vector2 TexCoord );

/// <summary>Linear material factors for a static model primitive.</summary>
/// <param name="Metallic">Metallic factor in the range zero to one.</param>
/// <param name="Roughness">Perceptual roughness in the range zero to one.</param>
/// <param name="Emissive">Linear RGB emission.</param>
public readonly record struct StaticMaterial( float Metallic, float Roughness, Vector3 Emissive );

/// <summary>Shared geometry for one triangle-list primitive.</summary>
public sealed class StaticModelPrimitive {
    private readonly ModelVertex[] _vertices;
    private readonly uint[] _indices;
    private readonly Vector2[]? _texCoords;

    internal StaticModelPrimitive( ModelVertex[] vertices, uint[] indices, Vector2[]? texCoords, StaticTexture? baseColor, StaticMaterial surface ) {
        _vertices = vertices;
        _indices = indices;
        _texCoords = texCoords;
        BaseColorTexture = baseColor;
        Surface = surface;
    }

    /// <summary>Gets the mesh-local vertices.</summary>
    public ReadOnlySpan<ModelVertex> Vertices => _vertices;

    /// <summary>Gets the base color coordinates parallel to <see cref="Vertices"/>, or an empty span.</summary>
    public ReadOnlySpan<Vector2> TexCoords => _texCoords;

    /// <summary>Gets the optional sRGB base color image. Sample with linear repeat filtering.</summary>
    public StaticTexture? BaseColorTexture { get; }

    /// <summary>Gets the material response factors for this primitive.</summary>
    public StaticMaterial Surface { get; }

    /// <summary>Copies interleaved geometry suitable for a textured vertex stream.</summary>
    /// <param name="destination">A destination of at least <see cref="Vertices"/> length.</param>
    public void CopyTexturedVertices( Span<TexturedModelVertex> destination ) {
        if ( destination.Length < _vertices.Length ) throw new ArgumentException( "Vertex destination is too short.", nameof(destination) );
        for ( int i = 0; i < _vertices.Length; ++i ) {
            var vertex = _vertices[i];
            destination[i] = new( vertex.Position, vertex.Normal, vertex.Color, _texCoords is null ? Vector2.Zero : _texCoords[i] );
        }
    }

    /// <summary>Gets the zero-based triangle-list indices in glTF winding order.</summary>
    public ReadOnlySpan<uint> Indices => _indices;
}

/// <summary>Places a shared primitive in the model's scene.</summary>
/// <param name="PrimitiveIndex">The zero-based index into <see cref="StaticModel.Primitives"/>.</param>
/// <param name="WorldTransform">The mesh-to-scene transform in row-vector convention.</param>
public readonly record struct StaticModelInstance( int PrimitiveIndex, Matrix4x4 WorldTransform );

/// <summary>Contains the shared geometry, instance transforms, and bounds of a static scene.</summary>
public sealed class StaticModel {
    private readonly StaticModelPrimitive[] _primitives;
    private readonly StaticModelInstance[] _instances;

    internal StaticModel( StaticModelPrimitive[] primitives, StaticModelInstance[] instances, Vector3 min, Vector3 max ) {
        _primitives = primitives;
        _instances = instances;
        BoundsMin = min;
        BoundsMax = max;
    }

    /// <summary>Gets the shared geometry.</summary>
    public ReadOnlySpan<StaticModelPrimitive> Primitives => _primitives;

    /// <summary>Gets the placed primitives in scene traversal order.</summary>
    public ReadOnlySpan<StaticModelInstance> Instances => _instances;

    /// <summary>Gets the lower corner of the conservative axis-aligned scene bounds.</summary>
    public Vector3 BoundsMin { get; }

    /// <summary>Gets the upper corner of the conservative axis-aligned scene bounds.</summary>
    public Vector3 BoundsMax { get; }
}
