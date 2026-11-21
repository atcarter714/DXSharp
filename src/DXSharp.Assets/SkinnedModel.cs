using System.Buffers.Binary;
using System.Diagnostics ;
using System.Numerics;
using System.Text.Json;
using StbImageSharp;

namespace DXSharp.Assets;

/// <summary>CPU reference deformation output.</summary>
/// <param name="Position">Mesh-local position.</param>
/// <param name="Normal">Mesh-local unit normal.</param>
/// <param name="Surface">Reserved linear surface multipliers; currently one.</param>
/// <param name="TexCoord">Material UV coordinates.</param>
public readonly record struct DeformedVertex(
    Vector3 Position,
    Vector3 Normal,
    Vector3 Surface,
    Vector2 TexCoord);

/// <summary>Bind-pose geometry with four normalized joint influences.</summary>
/// <param name="Position">Mesh-local position.</param>
/// <param name="Normal">Mesh-local unit normal.</param>
/// <param name="TexCoord">Material UV coordinates.</param>
/// <param name="Weights">Four normalized nonnegative influences.</param>
/// <param name="J0">Palette index for influence 0.</param>
/// <param name="J1">Palette index for influence 1.</param>
/// <param name="J2">Palette index for influence 2.</param>
/// <param name="J3">Palette index for influence 3.</param>
public readonly record struct SkinnedVertex(
    Vector3 Position,
    Vector3 Normal,
    Vector2 TexCoord,
    Vector4 Weights,
    uint J0, uint J1, uint J2, uint J3);

/// <summary>Decoded RGBA8 material image.</summary>
public sealed class ModelImage
{
    /// <summary>Image width in pixels.</summary>
    public required int Width { get; init; }
    /// <summary>Image height in pixels.</summary>
    public required int Height { get; init; }
    /// <summary>Tightly packed top-to-bottom RGBA8 pixels; color-space interpretation belongs to the material slot.</summary>
    public required byte[] Pixels { get; init; }


}

/// <summary>Authored skeleton node and local rest transform.</summary>
public sealed class SkeletonNode
{
    /// <summary>Authored node or animation name.</summary>
    public required string Name { get; init; }
    /// <summary>Child-node indices in the imported skeleton.</summary>
    public required int[] Children { get; init; }
    /// <summary>Local translation in glTF units.</summary>
    public required Vector3 Translation { get; init; }
    /// <summary>Local unit quaternion in XYZW order.</summary>
    public required Quaternion Rotation { get; init; }
    /// <summary>Local scale factors.</summary>
    public required Vector3 Scale { get; init; }
    /// <summary>Optional authored local matrix, used instead of TRS.</summary>
    public Matrix4x4? Matrix { get; init; }
}

/// <summary>Mesh attachment and palette ordering for one skin.</summary>
public sealed class SkinDefinition
{
    /// <summary>Skeleton node indices in palette order.</summary>
    public required int[] Joints { get; init; }
    /// <summary>Mesh-to-joint bind transforms in palette order, using row-vector matrices.</summary>
    public required Matrix4x4[] InverseBindMatrices { get; init; }
    /// <summary>Skeleton node containing the skinned mesh.</summary>
    public required int MeshNode { get; init; }
}

/// <summary>Immutable sampled keyframe data for one animated property.</summary>
public sealed class AnimationSampler
{
    readonly float[] _times;
    readonly Vector4[] _values;
    readonly bool _step;
    readonly bool _rotation;

    /// <summary>Copies strictly increasing finite key times and values; supports LINEAR and STEP. Rotation LINEAR uses shortest-path spherical interpolation.</summary>
    public AnimationSampler(float[] times, Vector4[] values, string interpolation, bool rotation = false)
    {
        ArgumentNullException.ThrowIfNull(times); ArgumentNullException.ThrowIfNull(values);
        _rotation = rotation;
        if (times.Any(t => !float.IsFinite(t) || t < 0) || times.Zip(times.Skip(1)).Any(pair => pair.First >= pair.Second))
            throw new InvalidDataException("Animation key times must be finite, nonnegative and strictly increasing.");
        if (times.Length == 0 || values.Length != times.Length)
            throw new InvalidDataException("Animation sampler input/output counts do not match.");
        _times = (float[])times.Clone();
        _values = (Vector4[])values.Clone();
        if (_values.Any(v => !float.IsFinite(v.X) || !float.IsFinite(v.Y) || !float.IsFinite(v.Z) || !float.IsFinite(v.W) || (rotation && v.LengthSquared() < 1e-12f))) throw new InvalidDataException("Invalid animation values.");
        _step = string.Equals(interpolation, "STEP", StringComparison.Ordinal);
        if (!_step && !string.Equals(interpolation, "LINEAR", StringComparison.Ordinal))
            throw new NotSupportedException($"Animation interpolation '{interpolation}' is not supported.");
    }

    /// <summary>Final key time in seconds.</summary>
    public float Duration => _times[^1];

    /// <summary>Samples a finite time in seconds, clamping outside the key range.</summary>
    public Vector4 Sample(float time)
    {
        if (!float.IsFinite(time)) throw new ArgumentOutOfRangeException(nameof(time));
        if (_times.Length == 1 || time <= _times[0]) return _values[0];
        if (time >= _times[^1]) return _values[^1];

        int next = Array.BinarySearch(_times, time);
        if (next >= 0) return _values[next];
        next = ~next;
        int previous = next - 1;
        if (_step) return _values[previous];

        float span = _times[next] - _times[previous];
        float amount = span > 0 ? (time - _times[previous]) / span : 0;
        if (!_rotation) return Vector4.Lerp(_values[previous], _values[next], amount);
        Vector4 a = _values[previous], b = _values[next];
        Quaternion q = Quaternion.Normalize(Quaternion.Slerp(new(a.X,a.Y,a.Z,a.W), new(b.X,b.Y,b.Z,b.W), amount));
        return new(q.X,q.Y,q.Z,q.W);
    }
}

/// <summary>Optional translation, rotation and scale channels for one node.</summary>
public sealed class AnimationTrack
{
    /// <summary>Local translation in glTF units.</summary>
    public AnimationSampler? Translation { get; set; }
    /// <summary>Local unit quaternion in XYZW order.</summary>
    public AnimationSampler? Rotation { get; set; }
    /// <summary>Local scale factors.</summary>
    public AnimationSampler? Scale { get; set; }
}

/// <summary>Named node-animation tracks and duration in seconds.</summary>
public sealed class AnimationClip
{
    /// <summary>Authored node or animation name.</summary>
    public required string Name { get; init; }
    /// <summary>Channels keyed by skeleton node index.</summary>
    public required Dictionary<int, AnimationTrack> Tracks { get; init; }
    /// <summary>Final key time in seconds.</summary>
    public required float Duration { get; init; }
}

/// <summary>Mutable local TRS values for one skeleton instance.</summary>
public sealed class SkeletonPose
{
    /// <summary>Creates mutable local TRS storage initialized from skeleton rest transforms.</summary>
    public SkeletonPose(SkeletonNode[] nodes)
    {
        Translation = nodes.Select(n => n.Translation).ToArray();
        Rotation = nodes.Select(n => n.Rotation).ToArray();
        Scale = nodes.Select(n => n.Scale).ToArray();
    }

    /// <summary>Local translation in glTF units.</summary>
    public Vector3[] Translation { get; }
    /// <summary>Local unit quaternion in XYZW order.</summary>
    public Quaternion[] Rotation { get; }
    /// <summary>Local scale factors.</summary>
    public Vector3[] Scale { get; }

    /// <summary>Copies all local transforms from an equally sized pose.</summary>
    public void CopyFrom(SkeletonPose other)
    {
        other.Translation.CopyTo(Translation, 0);
        other.Rotation.CopyTo(Rotation, 0);
        other.Scale.CopyTo(Scale, 0);
    }
}

/// <summary>A single-primitive, single-skin GLB model with CPU reference deformation and retained material images.</summary>
public sealed class SkinnedModel
{
    readonly SkeletonNode[] _nodes;
    readonly int[] _sceneRoots;
    readonly SkinnedVertex[] _sourceVertices;
    readonly Vector3[] _normals;
    readonly int[] _jointNodeIndices;
    readonly Matrix4x4[] _inverseBindMatrices;
    readonly Matrix4x4[] _nodeWorld, _palette, _normalPalette;
    readonly bool[] _visited;
    Vector4[] _tangentFrames = [];
    /// <summary>Unchanged bind-pose vertices and four normalized joint influences.</summary>
    public ReadOnlySpan<SkinnedVertex> SourceVertices => _sourceVertices;
    /// <summary>Mesh-local tangent frames; W contains bitangent handedness.</summary>
    public ReadOnlySpan<Vector4> Tangents => _tangentFrames;
    /// <summary>The optional linear tangent-space normal texture.</summary>
    public ModelImage? NormalMap => _normalMap;
    /// <summary>The optional linear metallic/roughness texture; occlusion uses its separate glTF texture.</summary>
    public ModelImage? MetallicRoughnessMap => _ormMap;
    /// <summary>The optional linear occlusion texture.</summary>
    public ModelImage? OcclusionMap => _occlusionMap;
    /// <summary>Builds a mesh-local joint palette into caller-owned storage. This model's scratch evaluation is single-threaded.</summary>
    public void BuildPalette(SkeletonPose pose, Span<Matrix4x4> destination) {
        ArgumentNullException.ThrowIfNull(pose);
        if (pose.Translation.Length != _nodes.Length) throw new ArgumentException("Pose does not match the skeleton.", nameof(pose));
        if (destination.Length < _jointNodeIndices.Length) throw new ArgumentException("Palette is too small.", nameof(destination));
        Array.Clear(_visited);
        foreach (int node in _sceneRoots) BuildWorld(node, Matrix4x4.Identity, pose, _nodeWorld, _visited);
        if (!Matrix4x4.Invert(_nodeWorld[_meshNodeIndex], out var inverseMesh)) throw new InvalidDataException("Singular mesh transform.");
        for (int i=0; i<_jointNodeIndices.Length; ++i) destination[i] = _inverseBindMatrices[i] * _nodeWorld[_jointNodeIndices[i]] * inverseMesh;
    }

    SkinnedModel(
        SkeletonNode[] nodes, int[] sceneRoots, SkinnedVertex[] sourceVertices,
        uint[] indices, int[] jointNodeIndices, Matrix4x4[] inverseBindMatrices,
        ModelImage baseColor, float metallic, float roughness,
        AnimationClip[] animations)
    {
        _nodes = nodes;
        _sceneRoots = sceneRoots;
        _sourceVertices = sourceVertices;
        Indices = indices;
        _jointNodeIndices = jointNodeIndices;
        _inverseBindMatrices = inverseBindMatrices;
        _nodeWorld = new Matrix4x4[nodes.Length]; _visited = new bool[nodes.Length];
        _palette = new Matrix4x4[jointNodeIndices.Length]; _normalPalette = new Matrix4x4[jointNodeIndices.Length];
        BaseColor = baseColor;
        MetallicFactor = metallic;
        RoughnessFactor = roughness;
        Animations = animations;

        _normals = new Vector3[sourceVertices.Length];
    }

    /// <summary>Imported node data. Treat model arrays as read-only after loading.</summary>
    public SkeletonNode[] Nodes => _nodes;
    /// <summary>Triangle-list indices into SourceVertices.</summary>
    public uint[] Indices { get; }
    /// <summary>Decoded base-color image, sampled as sRGB.</summary>
    public ModelImage BaseColor { get; }
    /// <summary>Linear metallic multiplier from the material.</summary>
    public float MetallicFactor { get; }
    /// <summary>Perceptual roughness multiplier from the material.</summary>
    public float RoughnessFactor { get; }
    /// <summary>Linear RGBA base-color multiplier.</summary>
    public Vector4 BaseColorFactor { get; private set; } = Vector4.One;
    /// <summary>Scale applied to the normal map's tangent-space XY components.</summary>
    public float NormalScale { get; private set; } = 1;
    /// <summary>Strength of the occlusion texture, from zero to one.</summary>
    public float OcclusionStrength { get; private set; } = 1;
    /// <summary>Imported clips; treat their channels as read-only.</summary>
    public AnimationClip[] Animations { get; }
    /// <summary>Skin attachment and joint palette ordering.</summary>
    public SkinDefinition Skin => new()
    {
        Joints = _jointNodeIndices,
        InverseBindMatrices = _inverseBindMatrices,
        MeshNode = FindMeshNode()
    };

    /// <summary>Loads one self-contained GLB triangle primitive and skin with embedded images and LINEAR/STEP TRS clips. Unsupported features fail explicitly.</summary>
    public static SkinnedModel Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path)) throw new FileNotFoundException("Skinned GLB was not found.", path);
        try { using var validation = File.OpenRead(path); _ = SharpGLTF.Schema2.ModelRoot.ReadGLB(validation, new SharpGLTF.Schema2.ReadSettings { Validation = SharpGLTF.Validation.ValidationMode.Strict }); }
        catch (SharpGLTF.Validation.ModelException error) { throw new InvalidDataException("The skinned GLB failed schema/content validation.", error); }
        using var stream = File.OpenRead(path);
        using var reader = new GlbReader(stream);
        return reader.Read();
    }

    /// <summary>Finds a clip by name, or returns the first clip when the name is absent.</summary>
    public int FindAnimation(string name)
    {
        for (int i = 0; i < Animations.Length; ++i)
            if (string.Equals(Animations[i].Name, name, StringComparison.OrdinalIgnoreCase))
                return i;
        return 0;
    }

    /// <summary>Evaluates CPU reference positions and inverse-transpose normals into caller-owned storage. Reuses model scratch and must run on one thread.</summary>
    public void MeshSkin(SkeletonPose pose, Matrix4x4 world, Span<DeformedVertex> destination)
    {
        if (destination.Length < _sourceVertices.Length)
            throw new ArgumentException("Destination vertex span is too small.", nameof(destination));

        BuildPalette(pose, _palette);
        var palette = _palette; var normalPalette = _normalPalette;
        for (int i = 0; i < palette.Length; ++i)
        {
            if (!Matrix4x4.Invert(palette[i], out Matrix4x4 inverse))
                normalPalette[i] = Matrix4x4.Identity;
            else
                normalPalette[i] = Matrix4x4.Transpose(inverse);
        }

        Matrix4x4 normalWorld = Matrix4x4.Identity;
        if (Matrix4x4.Invert(world, out Matrix4x4 inverseWorld))
            normalWorld = Matrix4x4.Transpose(inverseWorld);

        for (int i = 0; i < _sourceVertices.Length; ++i)
        {
            SkinnedVertex source = _sourceVertices[i];
            Vector3 position = Vector3.Zero;
            Vector3 normal = Vector3.Zero;
            float totalWeight = 0;

            Accumulate(source.J0, source.Weights.X);
            Accumulate(source.J1, source.Weights.Y);
            Accumulate(source.J2, source.Weights.Z);
            Accumulate(source.J3, source.Weights.W);

            if (totalWeight <= 1e-6f)
            {
                position = source.Position;
                normal = _normals[i];
            }
            else
            {
                position /= totalWeight;
                normal /= totalWeight;
            }

            position = Vector3.Transform(position, world);
            normal = Vector3.Normalize(Vector3.TransformNormal(normal, normalWorld));
            destination[i] = new(position, normal, Vector3.One, source.TexCoord);

            void Accumulate(uint joint, float weight)
            {
                if (weight <= 0 || joint >= palette.Length) return;
                position += Vector3.Transform(source.Position, palette[joint]) * weight;
                normal += Vector3.TransformNormal(_normals[i], normalPalette[joint]) * weight;
                totalWeight += weight;
            }
        }
    }

    int FindMeshNode()
    {
        for (int i = 0; i < _nodes.Length; ++i)
        {
            // This contract supports a single skin attachment.
            if (_meshNodeIndex == i) return i;
        }
        throw new InvalidDataException("The GLB has no skinned mesh node.");
    }

    int _meshNodeIndex = -1;

    void BuildWorld(int index, Matrix4x4 parent, SkeletonPose pose,
        Matrix4x4[] world, bool[] visited)
    {
        if (visited[index]) return;
        visited[index] = true;
        var node = _nodes[index];
        Matrix4x4 local = node.Matrix ??
            (Matrix4x4.CreateScale(pose.Scale[index]) *
             Matrix4x4.CreateFromQuaternion(Quaternion.Normalize(pose.Rotation[index])) *
             Matrix4x4.CreateTranslation(pose.Translation[index]));
        world[index] = local * parent;
        foreach (int child in node.Children)
            BuildWorld(child, world[index], pose, world, visited);
    }

    void GenerateTangents()
    {
        // Reconstruct tangent frames from UV triangles. Material maps remain pixel-shader inputs.
        _tangentFrames = new Vector4[_sourceVertices.Length];
        var tangents = new Vector3[_sourceVertices.Length];
        var bitangents = new Vector3[_sourceVertices.Length];

        for (int triangle = 0; triangle < Indices.Length; triangle += 3)
        {
            int i0 = checked((int)Indices[triangle]);
            int i1 = checked((int)Indices[triangle + 1]);
            int i2 = checked((int)Indices[triangle + 2]);
            var v0 = _sourceVertices[i0];
            var v1 = _sourceVertices[i1];
            var v2 = _sourceVertices[i2];

            Vector3 e1 = v1.Position - v0.Position;
            Vector3 e2 = v2.Position - v0.Position;
            Vector2 d1 = v1.TexCoord - v0.TexCoord;
            Vector2 d2 = v2.TexCoord - v0.TexCoord;
            float determinant = d1.X * d2.Y - d1.Y * d2.X;
            if (MathF.Abs(determinant) < 1e-8f) continue;

            float r = 1f / determinant;
            Vector3 tangent = (e1 * d2.Y - e2 * d1.Y) * r;
            Vector3 bitangent = (e2 * d1.X - e1 * d2.X) * r;
            tangents[i0] += tangent; tangents[i1] += tangent; tangents[i2] += tangent;
            bitangents[i0] += bitangent; bitangents[i1] += bitangent; bitangents[i2] += bitangent;
        }

        for (int i = 0; i < _sourceVertices.Length; ++i)
        {
            var source = _sourceVertices[i];
            Vector3 n = Vector3.Normalize(source.Normal);
            Vector3 t = tangents[i] - n * Vector3.Dot(n, tangents[i]);
            if (t.LengthSquared() < 1e-10f)
                t = Vector3.Normalize(Vector3.Cross(MathF.Abs(n.Y) < 0.95f ? Vector3.UnitY : Vector3.UnitX, n));
            else
                t = Vector3.Normalize(t);

            float handedness = Vector3.Dot(Vector3.Cross(n, t), bitangents[i]) < 0 ? -1f : 1f;

            _tangentFrames[i] = new Vector4(t, Vector3.Dot(Vector3.Cross(n, t), bitangents[i]) < 0 ? -1 : 1);
            _normals[i] = n;
        }


    }

    ModelImage? _normalMap;
    ModelImage? _ormMap;
    ModelImage? _occlusionMap;

    /// <summary>data for the supported single-primitive skinned GLB workflow.</summary>
    private sealed class GlbReader : IDisposable
    {
        readonly byte[] _jsonBytes;
        readonly byte[] _binary;
        readonly JsonDocument _document;
        readonly JsonElement _root;

        public GlbReader(Stream stream)
        {
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            byte[] bytes = memory.ToArray();
            if (bytes.Length < 20 || BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(0, 4)) != 0x46546C67)
                throw new InvalidDataException("Input is not a binary glTF (GLB) file.");
            uint version = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4, 4));
            uint declaredLength = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(8, 4));
            if (version != 2 || declaredLength != bytes.Length)
                throw new InvalidDataException("Unsupported or malformed GLB header.");

            int offset = 12;
            byte[]? json = null;
            byte[]? binary = null;
            while (offset + 8 <= bytes.Length)
            {
                uint length = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset, 4));
                uint type = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset + 4, 4));
                offset += 8;
                if (length > bytes.Length - offset) throw new InvalidDataException("GLB chunk exceeds file bounds.");
                if (type == 0x4E4F534A) json = bytes.AsSpan(offset, (int)length).ToArray();
                else if (type == 0x004E4942) binary = bytes.AsSpan(offset, (int)length).ToArray();
                offset += (int)length;
            }

            _jsonBytes = json ?? throw new InvalidDataException("GLB has no JSON chunk.");
            _binary = binary ?? throw new InvalidDataException("GLB has no BIN chunk.");
            _document = JsonDocument.Parse(_jsonBytes);
            _root = _document.RootElement;
        }

        /// <summary>Read for the supported single-primitive skinned GLB workflow.</summary>
        public SkinnedModel Read()
        {
            if (_root.GetProperty("asset").GetProperty("version").GetString() != "2.0")
                throw new NotSupportedException("Only glTF 2.0 assets are supported.");
            if (_root.TryGetProperty("extensionsRequired", out var required) && required.GetArrayLength() != 0)
                throw new NotSupportedException("This GLB requires glTF extensions that this sample does not implement.");

            var nodesJson = _root.GetProperty("nodes");
            var nodes = new SkeletonNode[nodesJson.GetArrayLength()];
            for (int i = 0; i < nodes.Length; ++i)
            {
                var n = nodesJson[i];
                nodes[i] = new SkeletonNode
                {
                    Name = n.TryGetProperty("name", out var name) ? name.GetString() ?? $"Node{i}" : $"Node{i}",
                    Children = n.TryGetProperty("children", out var children)
                        ? children.EnumerateArray().Select(x => x.GetInt32()).ToArray() : [],
                    Translation = ReadVector3(n, "translation", Vector3.Zero),
                    Rotation = Quaternion.Normalize(ReadQuaternion(n, "rotation", Quaternion.Identity)),
                    Scale = ReadVector3(n, "scale", Vector3.One),
                    Matrix = n.TryGetProperty("matrix", out var matrix) ? ReadMatrix(matrix) : null
                };
            }

            int sceneIndex = _root.TryGetProperty("scene", out var sceneValue) ? sceneValue.GetInt32() : 0;
            var scene = _root.GetProperty("scenes")[sceneIndex];
            int[] roots = scene.GetProperty("nodes").EnumerateArray().Select(x => x.GetInt32()).ToArray();

            int meshNode = -1;
            for (int i = 0; i < nodesJson.GetArrayLength(); ++i)
                if (nodesJson[i].TryGetProperty("mesh", out _) && nodesJson[i].TryGetProperty("skin", out _)) {
                    if (meshNode >= 0) throw new NotSupportedException("This import contract supports one skinned mesh node.");
                    meshNode = i;
                }
            if (meshNode < 0) throw new InvalidDataException("No node references both a mesh and a skin.");

            var skinJson = _root.GetProperty("skins")[nodesJson[meshNode].GetProperty("skin").GetInt32()];
            int[] joints = skinJson.GetProperty("joints").EnumerateArray().Select(x => x.GetInt32()).ToArray();
            if (joints.Length == 0 || joints.Length > 256)
                throw new InvalidDataException("The skin joint count is outside the supported range.");

            Matrix4x4[] inverseBind = new Matrix4x4[joints.Length];
            if (skinJson.TryGetProperty("inverseBindMatrices", out var ibmAccessor))
            {
                float[] values = ReadFloatAccessor(ibmAccessor.GetInt32(), 16);
                if (values.Length != joints.Length * 16)
                    throw new InvalidDataException("Inverse bind matrix count does not match the skin joints.");
                for (int i = 0; i < joints.Length; ++i)
                    inverseBind[i] = ReadMatrix(values.AsSpan(i * 16, 16));
            }
            else
            {
                Array.Fill(inverseBind, Matrix4x4.Identity);
            }

            var meshJson = _root.GetProperty("meshes")[nodesJson[meshNode].GetProperty("mesh").GetInt32()];
            var primitives = meshJson.GetProperty("primitives");
            if (primitives.GetArrayLength() != 1)
                throw new NotSupportedException("This import contract supports one skinned mesh primitive.");
            var primitive = primitives[0];
            if (primitive.TryGetProperty("mode", out var mode) && mode.GetInt32() != 4)
                throw new NotSupportedException("Only triangle-list primitives are supported.");

            if (primitive.TryGetProperty("targets", out _)) throw new NotSupportedException("Morph targets are outside this skin import contract.");
            var attributes = primitive.GetProperty("attributes");
            if (attributes.TryGetProperty("JOINTS_1", out _) || attributes.TryGetProperty("WEIGHTS_1", out _)) throw new NotSupportedException("Only four joint influences per vertex are supported.");
            float[] positions = ReadFloatAccessor(attributes.GetProperty("POSITION").GetInt32(), 3);
            float[] normals = ReadFloatAccessor(attributes.GetProperty("NORMAL").GetInt32(), 3);
            float[] uvs = ReadFloatAccessor(attributes.GetProperty("TEXCOORD_0").GetInt32(), 2);
            uint[] joints0 = ReadUnsignedAccessor(attributes.GetProperty("JOINTS_0").GetInt32(), 4);
            float[] weights = ReadFloatAccessor(attributes.GetProperty("WEIGHTS_0").GetInt32(), 4);
            uint[] indices = ReadUnsignedAccessor(primitive.GetProperty("indices").GetInt32(), 1);

            int vertexCount = positions.Length / 3;
            if (normals.Length != vertexCount * 3 || uvs.Length != vertexCount * 2 ||
                joints0.Length != vertexCount * 4 || weights.Length != vertexCount * 4)
                throw new InvalidDataException("Skinned vertex attribute counts do not match.");

            var sourceVertices = new SkinnedVertex[vertexCount];
            for (int i = 0; i < vertexCount; ++i)
            {
                var w = new Vector4(weights[i * 4], weights[i * 4 + 1], weights[i * 4 + 2], weights[i * 4 + 3]);
                float sum = w.X + w.Y + w.Z + w.W;
                if (!float.IsFinite(sum) || w.X < 0 || w.Y < 0 || w.Z < 0 || w.W < 0 || sum <= 1e-8f)
                    throw new InvalidDataException("Invalid skin weights.");
                w /= sum;
                for (int j=0;j<4;++j) if (joints0[i*4+j] >= joints.Length) throw new InvalidDataException("Joint index exceeds skin palette.");
                sourceVertices[i] = new(
                    new Vector3(positions[i * 3], positions[i * 3 + 1], positions[i * 3 + 2]),
                    Vector3.Normalize(new Vector3(normals[i * 3], normals[i * 3 + 1], normals[i * 3 + 2])),
                    new Vector2(uvs[i * 2], uvs[i * 2 + 1]),
                    w,
                    joints0[i * 4], joints0[i * 4 + 1], joints0[i * 4 + 2], joints0[i * 4 + 3]);
            }

            if (indices.Any(index => index >= vertexCount) || indices.Length % 3 != 0) throw new InvalidDataException("Invalid triangle indices.");
            int materialIndex = primitive.TryGetProperty("material", out var mat) ? mat.GetInt32() : -1;
            float metallic = 1f, roughness = 1f;
            ModelImage? baseColor = null, normalMap = null, ormMap = null, occlusionMap = null;
            if (materialIndex >= 0)
            {
                var material = _root.GetProperty("materials")[materialIndex];
                if (material.TryGetProperty("alphaMode",out var alpha) && alpha.GetString() != "OPAQUE") throw new NotSupportedException("This renderer supports opaque materials.");
                if (material.TryGetProperty("doubleSided",out var sided) && sided.GetBoolean()) throw new NotSupportedException("Double-sided materials need a separate pipeline contract.");
                if (material.TryGetProperty("pbrMetallicRoughness", out var pbr))
                {
                    if (pbr.TryGetProperty("metallicFactor", out var mf)) metallic = mf.GetSingle();
                    if (pbr.TryGetProperty("roughnessFactor", out var rf)) roughness = rf.GetSingle();
                    if (pbr.TryGetProperty("baseColorTexture", out var bc))
                        baseColor = ReadTexture(bc.GetProperty("index").GetInt32());
                    if (pbr.TryGetProperty("metallicRoughnessTexture", out var mr))
                        ormMap = ReadTexture(mr.GetProperty("index").GetInt32());
                }
                if (material.TryGetProperty("normalTexture", out var normal))
                    normalMap = ReadTexture(normal.GetProperty("index").GetInt32());
            }

            if (materialIndex >= 0 && _root.GetProperty("materials")[materialIndex].TryGetProperty("occlusionTexture", out var occlusion))
                occlusionMap = ReadTexture(occlusion.GetProperty("index").GetInt32());
            baseColor ??= WhiteImage();
            var animations = ReadAnimations();
            var asset = new SkinnedModel(
                nodes, roots, sourceVertices, indices, joints, inverseBind,
                baseColor, metallic, roughness, animations);
            if (materialIndex >= 0) {
                var settings = _root.GetProperty("materials")[materialIndex];
                if (settings.TryGetProperty("pbrMetallicRoughness", out var pbr) && pbr.TryGetProperty("baseColorFactor", out var factor))
                    asset.BaseColorFactor = new(factor[0].GetSingle(),factor[1].GetSingle(),factor[2].GetSingle(),factor[3].GetSingle());
                if (settings.TryGetProperty("normalTexture",out var normalSettings) && normalSettings.TryGetProperty("scale",out var scale)) asset.NormalScale=scale.GetSingle();
                if (settings.TryGetProperty("occlusionTexture",out var aoSettings) && aoSettings.TryGetProperty("strength",out var strength)) asset.OcclusionStrength=strength.GetSingle();
                foreach (var slot in new[]{"normalTexture","occlusionTexture"}) if(settings.TryGetProperty(slot,out var texture) && texture.TryGetProperty("texCoord",out var set) && set.GetInt32()!=0) throw new NotSupportedException("This material path supports TEXCOORD_0 only.");
                if(settings.TryGetProperty("pbrMetallicRoughness",out var materialPbr)) foreach(var slot in new[]{"baseColorTexture","metallicRoughnessTexture"}) if(materialPbr.TryGetProperty(slot,out var texture) && texture.TryGetProperty("texCoord",out var set) && set.GetInt32()!=0) throw new NotSupportedException("This material path supports TEXCOORD_0 only.");
            }
            asset._meshNodeIndex = meshNode;
            asset._normalMap = normalMap;
            asset._ormMap = ormMap; asset._occlusionMap = occlusionMap;
            asset.GenerateTangents();
            return asset;
        }

        AnimationClip[] ReadAnimations()
        {
            if (!_root.TryGetProperty("animations", out var animations)) return [];
            var result = new AnimationClip[animations.GetArrayLength()];
            for (int a = 0; a < result.Length; ++a)
            {
                var animation = animations[a];
                var samplers = animation.GetProperty("samplers");
                var tracks = new Dictionary<int, AnimationTrack>();
                float duration = 0;
                foreach (var channel in animation.GetProperty("channels").EnumerateArray())
                {
                    int samplerIndex = channel.GetProperty("sampler").GetInt32();
                    var samplerJson = samplers[samplerIndex];
                    int inputIndex = samplerJson.GetProperty("input").GetInt32();
                    int outputIndex = samplerJson.GetProperty("output").GetInt32();
                    string interpolation = samplerJson.TryGetProperty("interpolation", out var interp)
                        ? interp.GetString() ?? "LINEAR" : "LINEAR";

                    if (interpolation is not ("LINEAR" or "STEP")) throw new NotSupportedException("Only LINEAR and STEP clips are supported.");
                    float[] times = ReadFloatAccessor(inputIndex, 1);
                    float[] rawValues = ReadFloatAccessor(outputIndex, 0);
                    int components = GetComponentCount(GetAccessorType(outputIndex));
                    if (rawValues.Length != times.Length * components)
                        throw new InvalidDataException("Animation output count does not match its input key count.");
                    var values = new Vector4[times.Length];
                    for (int k = 0; k < times.Length; ++k)
                    {
                        int offset = k * components;
                        values[k] = components switch
                        {
                            3 => new(rawValues[offset], rawValues[offset + 1], rawValues[offset + 2], 0),
                            4 => new(rawValues[offset], rawValues[offset + 1], rawValues[offset + 2], rawValues[offset + 3]),
                            _ => throw new NotSupportedException("Animation outputs must be VEC3 or VEC4.")
                        };
                    }

                    var target = channel.GetProperty("target");
                    int node = target.GetProperty("node").GetInt32();
                    string path = target.GetProperty("path").GetString() ?? "";
                    if (!tracks.TryGetValue(node, out var track))
                        tracks.Add(node, track = new AnimationTrack());
                    var parsed = new AnimationSampler(times, values, interpolation, path == "rotation");
                    duration = Math.Max(duration, parsed.Duration);

                    switch (path)
                    {
                        case "translation": track.Translation = parsed; break;
                        case "rotation": track.Rotation = parsed; break;
                        case "scale": track.Scale = parsed; break;
                        default: throw new NotSupportedException($"Animation path '{path}' is not supported.");
                    }
                }

                string name = animation.TryGetProperty("name", out var n) ? n.GetString() ?? $"Clip{a}" : $"Clip{a}";
                result[a] = new AnimationClip { Name = name, Tracks = tracks, Duration = duration };
            }
            return result;
        }

        ModelImage ReadTexture(int textureIndex)
        {
            var texture = _root.GetProperty("textures")[textureIndex];
            int source = texture.GetProperty("source").GetInt32();
            var image = _root.GetProperty("images")[source];
            if (!image.TryGetProperty("bufferView", out var viewElement))
                throw new NotSupportedException("This import contract requires embedded GLB images.");
            var view = _root.GetProperty("bufferViews")[viewElement.GetInt32()];
            int offset = view.TryGetProperty("byteOffset", out var bo) ? bo.GetInt32() : 0;
            int length = view.GetProperty("byteLength").GetInt32();
            var encoded = _binary.AsSpan(offset, length).ToArray();
            var decoded = ImageResult.FromMemory(encoded, ColorComponents.RedGreenBlueAlpha);
            return new ModelImage { Width = decoded.Width, Height = decoded.Height, Pixels = decoded.Data };
        }

        float[] ReadFloatAccessor(int accessorIndex, int expectedComponents)
        {
            var accessor = _root.GetProperty("accessors")[accessorIndex];
            if (accessor.TryGetProperty("sparse",out _)) throw new NotSupportedException("Sparse accessors require expansion before this import path.");
            int components = GetComponentCount(accessor.GetProperty("type").GetString()!);
            if (expectedComponents != 0 && components != expectedComponents)
                throw new InvalidDataException($"Accessor {accessorIndex} has {components} components; expected {expectedComponents}.");
            int count = accessor.GetProperty("count").GetInt32();
            int componentType = accessor.GetProperty("componentType").GetInt32();
            int componentSize = ComponentSize(componentType);
            var view = _root.GetProperty("bufferViews")[accessor.GetProperty("bufferView").GetInt32()];
            int viewOffset = view.TryGetProperty("byteOffset", out var vo) ? vo.GetInt32() : 0;
            int accessorOffset = accessor.TryGetProperty("byteOffset", out var ao) ? ao.GetInt32() : 0;
            int stride = view.TryGetProperty("byteStride", out var bs) ? bs.GetInt32() : componentSize * components;
            bool normalized = accessor.TryGetProperty("normalized", out var norm) && norm.GetBoolean();
            int start = checked(viewOffset + accessorOffset);
            var values = new float[count * components];

            for (int i = 0; i < count; ++i)
            for (int c = 0; c < components; ++c)
            {
                int offset = checked(start + i * stride + c * componentSize);
                values[i * components + c] = ReadComponent(_binary, offset, componentType, normalized);
            }
            return values;
        }

        uint[] ReadUnsignedAccessor(int accessorIndex, int expectedComponents)
        {
            var accessor = _root.GetProperty("accessors")[accessorIndex];
            if (accessor.TryGetProperty("sparse",out _)) throw new NotSupportedException("Sparse accessors require expansion before this import path.");
            int components = GetComponentCount(accessor.GetProperty("type").GetString()!);
            if (components != expectedComponents)
                throw new InvalidDataException($"Accessor {accessorIndex} has an unexpected component count.");
            int count = accessor.GetProperty("count").GetInt32();
            int componentType = accessor.GetProperty("componentType").GetInt32();
            if (componentType is not (5121 or 5123 or 5125))
                throw new NotSupportedException("Unsigned accessor data must use UBYTE, USHORT or UINT.");
            int componentSize = ComponentSize(componentType);
            var view = _root.GetProperty("bufferViews")[accessor.GetProperty("bufferView").GetInt32()];
            int viewOffset = view.TryGetProperty("byteOffset", out var vo) ? vo.GetInt32() : 0;
            int accessorOffset = accessor.TryGetProperty("byteOffset", out var ao) ? ao.GetInt32() : 0;
            int stride = view.TryGetProperty("byteStride", out var bs) ? bs.GetInt32() : componentSize * components;
            int start = checked(viewOffset + accessorOffset);
            var values = new uint[count * components];

            for (int i = 0; i < count; ++i)
            for (int c = 0; c < components; ++c)
            {
                int offset = checked(start + i * stride + c * componentSize);
                values[i * components + c] = componentType switch
                {
                    5121 => _binary[offset],
                    5123 => BinaryPrimitives.ReadUInt16LittleEndian(_binary.AsSpan(offset, 2)),
                    5125 => BinaryPrimitives.ReadUInt32LittleEndian(_binary.AsSpan(offset, 4)),
                    _ => throw new UnreachableException()
                };
            }
            return values;
        }

        string GetAccessorType(int accessorIndex) =>
            _root.GetProperty("accessors")[accessorIndex].GetProperty("type").GetString()!;

        static int GetComponentCount(string type) => type switch
        {
            "SCALAR" => 1, "VEC2" => 2, "VEC3" => 3, "VEC4" => 4, "MAT4" => 16,
            _ => throw new NotSupportedException($"Accessor type '{type}' is not supported.")
        };

        static int ComponentSize(int type) => type switch
        {
            5120 or 5121 => 1,
            5122 or 5123 => 2,
            5125 or 5126 => 4,
            _ => throw new NotSupportedException($"glTF component type {type} is not supported.")
        };

        static float ReadComponent(byte[] data, int offset, int type, bool normalized) => type switch
        {
            5126 => BitConverter.ToSingle(data, offset),
            5121 => normalized ? data[offset] / 255f : data[offset],
            5123 => normalized
                ? BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(offset, 2)) / 65535f
                : BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(offset, 2)),
            5120 => normalized ? Math.Max((sbyte)data[offset] / 127f, -1f) : (sbyte)data[offset],
            5122 => normalized
                ? Math.Max(BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(offset, 2)) / 32767f, -1f)
                : BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(offset, 2)),
            5125 => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4)),
            _ => throw new NotSupportedException($"glTF component type {type} is not supported.")
        };

        static Vector3 ReadVector3(JsonElement node, string property, Vector3 fallback)
        {
            if (!node.TryGetProperty(property, out var value)) return fallback;
            return new(value[0].GetSingle(), value[1].GetSingle(), value[2].GetSingle());
        }

        static Quaternion ReadQuaternion(JsonElement node, string property, Quaternion fallback)
        {
            if (!node.TryGetProperty(property, out var value)) return fallback;
            return new(value[0].GetSingle(), value[1].GetSingle(), value[2].GetSingle(), value[3].GetSingle());
        }

        static Matrix4x4 ReadMatrix(JsonElement value)
        {
            Span<float> v = stackalloc float[16];
            for (int i = 0; i < 16; ++i) v[i] = value[i].GetSingle();
            return ReadMatrix(v);
        }

        static Matrix4x4 ReadMatrix(ReadOnlySpan<float> v) => new(
            v[0], v[1], v[2], v[3],
            v[4], v[5], v[6], v[7],
            v[8], v[9], v[10], v[11],
            v[12], v[13], v[14], v[15]);

        static ModelImage WhiteImage() =>
            new() { Width = 1, Height = 1, Pixels = [255, 255, 255, 255] };

        /// <summary>Dispose for the supported single-primitive skinned GLB workflow.</summary>
        public void Dispose() => _document.Dispose();
    }
}
