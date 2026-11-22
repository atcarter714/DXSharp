using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Assets;
using DXSharp.Graphics;
using DXSharp.Shaders;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;

namespace DXSharp.Framework.Graphics;

/// <summary>A resident skinned vertex with a tangent frame and four joint influences.</summary>
/// <param name="Position">Bind-pose mesh-local position.</param>
/// <param name="Normal">Bind-pose unit normal.</param>
/// <param name="Tangent">Tangent XYZ and bitangent handedness W.</param>
/// <param name="TexCoord">Material UV coordinates.</param>
/// <param name="Weights">Four normalized joint weights.</param>
/// <param name="J0">First joint index.</param>
/// <param name="J1">Second joint index.</param>
/// <param name="J2">Third joint index.</param>
/// <param name="J3">Fourth joint index.</param>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct SkinnedMeshVertex(Vector3 Position, Vector3 Normal, Vector4 Tangent, Vector2 TexCoord, Vector4 Weights,
    uint J0,uint J1,uint J2,uint J3) {
    /// <summary>Creates the GPU bind-pose stream once from an imported model.</summary>
    public static SkinnedMeshVertex[] Create(SkinnedModel model) {
        ArgumentNullException.ThrowIfNull(model);
        var vertices=new SkinnedMeshVertex[model.SourceVertices.Length];
        for(int i=0;i<vertices.Length;++i) { var s=model.SourceVertices[i]; vertices[i]=new(s.Position,s.Normal,model.Tangents[i],s.TexCoord,s.Weights,s.J0,s.J1,s.J2,s.J3); }
        return vertices;
    }
}

/// <summary>Vertex-shader skeletal rendering with persistent geometry, frame-owned palettes and four material texture tables.</summary>
/// <remarks>Draw on one thread. A frame slot may be overwritten only after its prior submission retires. DesktopFrame supplies such a slot.
/// Model data and pose evaluation are single-threaded. Dispose after final recording; command lists retain GPU dependencies through completion.</remarks>
public sealed unsafe class SkinnedModelRenderer : IDisposable {
    private readonly SkinnedModel _model;
    private readonly Matrix4x4[] _palette;
    private readonly Joint[] _joints;
    private readonly GpuResource[] _frames;
    private readonly Texture2D[] _textures=new Texture2D[4];
    private readonly List<IDisposable> _owners=[];
    private readonly Mesh _mesh, _floor;
    private readonly RootSignature _root;
    private readonly PipelineState _pipeline;
    private readonly ulong _sceneOffset;
    private bool _disposed;
    [StructLayout(LayoutKind.Sequential)]
    private struct Joint { public Matrix4x4 Pose, Normal; }
    [StructLayout(LayoutKind.Sequential)]
    private struct Scene { public Matrix4x4 Wvp, World, Normal; public Vector4 Surface, Camera, BaseFactor, MaterialSettings; }
    /// <summary>Creates resident geometry and materials. Initialization performs one upload submission and wait.</summary>
    public SkinnedModelRenderer(GraphicsDevice device, ShaderCompiler compiler, SkinnedModel model, int frameSlots=2) {
        ArgumentNullException.ThrowIfNull(device); ArgumentNullException.ThrowIfNull(compiler); ArgumentNullException.ThrowIfNull(model);
        if(frameSlots is <1 or >3) throw new ArgumentOutOfRangeException(nameof(frameSlots));
        _model=model; _palette=new Matrix4x4[model.Skin.Joints.Length]; _joints=new Joint[_palette.Length+1]; _frames=new GpuResource[frameSlots];
        _sceneOffset = ((ulong)_joints.Length*128+255)&~255UL;
        try {
            _root=Own(CreateSignature(device));
            string shader=ReadShader();
            _pipeline=Own(device.CreateGraphicsPipeline(_root,compiler.Compile(shader,"VS","vs_6_0"),compiler.Compile(shader,"PS","ps_6_0"),[
                new("POSITION",VertexFormat.Float3,0),new("NORMAL",VertexFormat.Float3,12),new("TANGENT",VertexFormat.Float4,24),
                new("TEXCOORD",VertexFormat.Float2,40),new("BLENDWEIGHT",VertexFormat.Float4,48),new("BLENDINDICES",VertexFormat.UInt4,64)
            ],depthEnabled:true));
            var descriptors=Own(device.CreateDescriptorPool(4));
            using var ring=device.CreateUploadRing(UploadBytes(model));
            using var queue=device.CreateCommandQueue(); using var allocator=device.CreateCommandAllocator(); using var upload=device.CreateCommandList(allocator);
            ModelImage?[] images=[model.BaseColor,model.NormalMap,model.MetallicRoughnessMap,model.OcclusionMap];
            byte[][] fallback=[[255,255,255,255],[128,128,255,255],[255,255,255,255],[255,255,255,255]];
            for(int i=0;i<4;++i) {
                var image=images[i];uint width=(uint)(image?.Width??1),height=(uint)(image?.Height??1);
                var texture=Own(device.CreateTexture2D(new(width,height,i==0?DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM),descriptors));
                _textures[i]=texture;upload.UploadTexture(texture,ring,[new(image?.Pixels??fallback[i],width*4)]);
                upload.Transition(texture,ResourceState.CopyDestination,ResourceState.PixelShaderResource);
            }
            _mesh=Own(device.CreateMesh<SkinnedMeshVertex>(upload,SkinnedMeshVertex.Create(model),model.Indices));
            uint identity=(uint)_palette.Length;
            SkinnedMeshVertex Floor(float x,float z,float u,float v)=>new(new(x,0,z),Vector3.UnitY,new(1,0,0,1),new(u,v),new(1,0,0,0),identity,identity,identity,identity);
            _floor=Own(device.CreateMesh<SkinnedMeshVertex>(upload,[Floor(-5,-5,0,0),Floor(-5,5,0,5),Floor(5,5,5,5),Floor(5,-5,5,0)],new ushort[]{0,1,2,0,2,3}));
            upload.Close();queue.Wait(queue.Submit(upload));
            for(int i=0;i<_frames.Length;++i) _frames[i]=Own(device.CreateBuffer(checked(_sceneOffset+512),BufferMemory.Upload));
        } catch { Dispose();throw; }
    }
    /// <summary>Records a skinned model and optional reference grid using a completed frame slot. Matrices use System.Numerics row-vector conventions.</summary>
    public void Draw(GraphicsCommandList commands,SkeletonPose pose,Matrix4x4 world,Matrix4x4 viewProjection,Vector3 camera,int frameSlot,bool drawGrid=true,float exposure=1) {
        ObjectDisposedException.ThrowIf(_disposed,this);
        if((uint)frameSlot>=_frames.Length) throw new ArgumentOutOfRangeException(nameof(frameSlot));
        if(!float.IsFinite(exposure)||exposure<=0) throw new ArgumentOutOfRangeException(nameof(exposure));
        _model.BuildPalette(pose,_palette);
        for(int i=0;i<_palette.Length;++i) {
            if(!Matrix4x4.Invert(_palette[i],out var inverse)) throw new InvalidOperationException("Singular skin transform.");
            _joints[i]=new(){Pose=_palette[i],Normal=Matrix4x4.Transpose(inverse)};
        }
        _joints[^1]=new(){Pose=Matrix4x4.Identity,Normal=Matrix4x4.Identity};
        _frames[frameSlot].Write<Joint>(_joints);
        commands.SetGraphicsPipeline(_pipeline);commands.SetGraphicsShaderBuffer(1,_frames[frameSlot]);commands.SetGraphicsTextureTables(2,_textures);
        if(drawGrid) { Scene floor=new(){Wvp=viewProjection,World=Matrix4x4.Identity,Normal=Matrix4x4.Identity,Surface=new(0,1,1,1),Camera=new(camera,exposure),BaseFactor=Vector4.One,MaterialSettings=Vector4.One}; SetScene(commands,ref floor,_frames[frameSlot],_sceneOffset);commands.DrawMesh(_floor); }
        if(!Matrix4x4.Invert(world,out var inverseWorld)) throw new ArgumentException("Singular world transform.",nameof(world));
        Scene scene=new(){Wvp=world*viewProjection,World=world,Normal=Matrix4x4.Transpose(inverseWorld),Surface=new(_model.MetallicFactor,_model.RoughnessFactor,1,0),Camera=new(camera,exposure),BaseFactor=_model.BaseColorFactor,MaterialSettings=new(_model.NormalScale,_model.OcclusionStrength,0,0)};
        SetScene(commands,ref scene,_frames[frameSlot],_sceneOffset+256);commands.DrawMesh(_mesh);
    }
    private static void SetScene(GraphicsCommandList commands,ref Scene scene,GpuResource buffer,ulong offset) { buffer.Write<Scene>(MemoryMarshal.CreateReadOnlySpan(ref scene,1),offset); commands.SetGraphicsConstantBuffer(0,buffer,offset); }
    private T Own<T>(T value) where T:IDisposable { _owners.Add(value);return value; }
    private static ulong UploadBytes(SkinnedModel model) {
        ulong size=0;ModelImage?[] images=[model.BaseColor,model.NormalMap,model.MetallicRoughnessMap,model.OcclusionMap];
        foreach(var image in images) size=checked(size+((((ulong)(image?.Width??1)*4+255)&~255UL)*(ulong)(image?.Height??1)+511)&~511UL);
        return Math.Max(size,4096);
    }
    private static string ReadShader() {
        using var stream=typeof(SkinnedModelRenderer).Assembly.GetManifestResourceStream("DXSharp.Framework.Graphics.Skinned.hlsl")??throw new InvalidOperationException("Missing skinning shader.");
        using var reader=new StreamReader(stream);return reader.ReadToEnd();
    }
    private static RootSignature CreateSignature(GraphicsDevice device) {
        D3D12_ROOT_PARAMETER* p=stackalloc D3D12_ROOT_PARAMETER[6];new Span<D3D12_ROOT_PARAMETER>(p,6).Clear();
        p[0].ParameterType=D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_CBV;
        p[1].ParameterType=D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_SRV;p[1].ShaderVisibility=D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_VERTEX;p[1].Anonymous.Descriptor.RegisterSpace=1;
        D3D12_DESCRIPTOR_RANGE* ranges=stackalloc D3D12_DESCRIPTOR_RANGE[4];new Span<D3D12_DESCRIPTOR_RANGE>(ranges,4).Clear();
        for(int i=0;i<4;++i) {
            ranges[i].RangeType=D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SRV;ranges[i].NumDescriptors=1;ranges[i].BaseShaderRegister=(uint)i;
            p[i+2].ParameterType=D3D12_ROOT_PARAMETER_TYPE.D3D12_ROOT_PARAMETER_TYPE_DESCRIPTOR_TABLE;p[i+2].ShaderVisibility=D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_PIXEL;
            p[i+2].Anonymous.DescriptorTable=new(){NumDescriptorRanges=1,pDescriptorRanges=ranges+i};
        }
        D3D12_STATIC_SAMPLER_DESC sampler=new(){Filter=D3D12_FILTER.D3D12_FILTER_MIN_MAG_MIP_LINEAR,AddressU=D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP,AddressV=D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP,AddressW=D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP,MaxAnisotropy=1,MaxLOD=float.MaxValue,ShaderVisibility=D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_PIXEL,ComparisonFunc=D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_ALWAYS};
        D3D12_VERSIONED_ROOT_SIGNATURE_DESC description=default;description.Version=D3D_ROOT_SIGNATURE_VERSION.D3D_ROOT_SIGNATURE_VERSION_1_0;
        description.Anonymous.Desc_1_0=new(){NumParameters=6,pParameters=p,NumStaticSamplers=1,pStaticSamplers=&sampler,Flags=D3D12_ROOT_SIGNATURE_FLAGS.D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT};
        return device.CreateRootSignature(description);
    }
    /// <summary>Releases public GPU owners; previously recorded submissions retain their dependencies.</summary>
    public void Dispose() { if(_disposed)return;_disposed=true;for(int i=_owners.Count-1;i>=0;--i)_owners[i].Dispose();_owners.Clear(); }
}
