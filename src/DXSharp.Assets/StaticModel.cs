using System.Numerics;
using System.Runtime.InteropServices;

namespace DXSharp.Assets;

/// <summary>A model vertex with a position, unit normal, and linear RGB base color.</summary>
/// <param name="Position">The position in mesh-local coordinates.</param>
/// <param name="Normal">The unit normal in mesh-local coordinates.</param>
/// <param name="Color">The linear RGB base color, including the material's tint.</param>
[StructLayout( LayoutKind.Sequential )]
public readonly record struct ModelVertex( Vector3 Position, Vector3 Normal, Vector3 Color );

/// <summary>Shared geometry for one triangle-list primitive.</summary>
public sealed class StaticModelPrimitive {
    private readonly ModelVertex[] _vertices;
    private readonly uint[] _indices;

    internal StaticModelPrimitive( ModelVertex[] vertices, uint[] indices ) {
        _vertices = vertices;
        _indices = indices;
    }

    /// <summary>Gets the mesh-local vertices.</summary>
    public ReadOnlySpan<ModelVertex> Vertices => _vertices;

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
