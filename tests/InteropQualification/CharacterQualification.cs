using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Assets;
using DXSharp.Framework;
using DXSharp.Framework.Graphics;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace InteropQualification;

internal static class CharacterQualification {
    internal static SkinnedModel Model { get; private set; } = null!;
    private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException("Character: "+message); }
    private static void Reject<T>(Action action) where T:Exception { try{action();}catch(T){return;}throw new InvalidOperationException("Expected "+typeof(T).Name); }
    internal static void RunPortable() {
        var q=Quaternion.CreateFromYawPitchRoll(0.4f,0.8f,-0.2f);
        var sampler=new AnimationSampler([0,1],[new(q.X,q.Y,q.Z,q.W),new(-q.X,-q.Y,-q.Z,-q.W)],"LINEAR",rotation:true);
        for(int i=0;i<=100;++i) { var v=sampler.Sample(i/100f);Check(Math.Abs(Quaternion.Dot(q,new(v.X,v.Y,v.Z,v.W)))>0.99999f,"antipodal rotation follows identical orientation"); }
        Reject<InvalidDataException>(()=>new AnimationSampler([1,0],[Vector4.Zero,Vector4.One],"LINEAR"));
        Reject<NotSupportedException>(()=>new AnimationSampler([0,1],[Vector4.Zero,Vector4.One],"CUBICSPLINE"));
        var clock=new GameClock(TimeSpan.FromMilliseconds(10),3);int updates=0;
        Action<GameTime> update=_=>++updates;
        var time=clock.Advance(TimeSpan.FromMilliseconds(25),update);Check(updates==2&&Math.Abs(time.Interpolation-0.5)<1e-9,"fixed updates and render interpolation");
        clock.Paused=true;clock.Advance(TimeSpan.FromSeconds(20),update);Check(updates==2,"pause avoids catchup");clock.Paused=false;
        clock.Advance(TimeSpan.FromMilliseconds(100),update);Check(updates==5&&clock.DroppedTime==TimeSpan.FromMilliseconds(70),"bounded catchup reports discarded time");
        Model=SkinnedModel.Load(Path.Combine(AppContext.BaseDirectory,"CharacterLab","knight.glb"));
        Check(Model.SourceVertices.Length==37141&&Model.Skin.Joints.Length==22&&Model.Animations.Length==9,"real asset inventory");
        Check(Model.NormalMap is not null&&Model.MetallicRoughnessMap is not null,"material maps survive tangent generation");
        int root=Array.FindIndex(Model.Nodes,n=>n.Name=="pelvis");var player=new AnimationPlayer(Model,root);var pose=new SkeletonPose(Model.Nodes);var previous=new SkeletonPose(Model.Nodes);
        var palette=new Matrix4x4[22];var cpu=new DeformedVertex[Model.SourceVertices.Length];
        for(int clip=0;clip<Model.Animations.Length;++clip) {
            player.Play(clip,0);player.Update(0.37);player.Sample(pose);Model.BuildPalette(pose,palette);Model.MeshSkin(pose,Matrix4x4.Identity,cpu);
            Check(cpu.All(v=>float.IsFinite(v.Position.X)&&float.IsFinite(v.Normal.Y)),"finite deformation across clips");
        }
        player.Play(1,0);player.Update(0.2);player.Play(3,0.5);player.Update(0.2);player.Sample(previous);player.Play(7,0.3);player.Sample(pose);
        for(int i=0;i<Model.Nodes.Length;++i)Check(Vector3.Distance(pose.Translation[i],previous.Translation[i])<1e-6&&Math.Abs(Quaternion.Dot(pose.Rotation[i],previous.Rotation[i]))>0.99999f,"interrupted fade preserves visible pose");
        player.Play(Model.FindAnimation("Walk_Root"),0);player.ExtractRootMotion=true;player.Update(Model.Animations[Model.FindAnimation("Walk_Root")].Duration*2);
        var track=Model.Animations[Model.FindAnimation("Walk_Root")].Tracks[root].Translation!;var a=track.Sample(0);var b=track.Sample(track.Duration);
        Check(Vector3.Distance(player.RootMotionDelta,new((b.X-a.X)*2,0,(b.Z-a.Z)*2))<1e-4,"root motion crosses two loops");
        player.Play(Model.FindAnimation("Attack"),0,loop:false);player.Update(100);Check(player.Completed,"non-looping clips hold last pose");
        player.Play(0,0);for(int i=0;i<100;++i){player.Update(0.001);player.Sample(pose);Model.BuildPalette(pose,palette);}
        long before=GC.GetAllocatedBytesForCurrentThread();for(int i=0;i<500;++i){player.Update(0.001);player.Sample(pose);Model.BuildPalette(pose,palette);}
        Check(GC.GetAllocatedBytesForCurrentThread()==before,"steady-state clip and palette evaluation allocate zero managed bytes");
        Console.WriteLine("PASS: real character clips, antipodal rotations, interrupted fades, root-motion loops, fixed timing and zero-allocation pose/palette evaluation.");
    }
    [StructLayout(LayoutKind.Sequential)]private struct Joint { public Matrix4x4 Pose,Normal; }
    internal static void RunGpu(GraphicsDevice device) {
        using var stream=typeof(SkinnedModelRenderer).Assembly.GetManifestResourceStream("DXSharp.Framework.Graphics.Skinned.hlsl")!;
        using var reader=new StreamReader(stream);string shader="#define VERIFY_SKIN\n"+reader.ReadToEnd();
        using var compiler=new ShaderCompiler();using var root=device.CreateRootSignature(3,1);using var pipeline=device.CreateComputePipeline(root,compiler.Compile(shader,"CS","cs_6_0"));
        var vertices=SkinnedMeshVertex.Create(Model);var pose=new SkeletonPose(Model.Nodes);var player=new AnimationPlayer(Model);
        var matrices=new Matrix4x4[22];var joints=new Joint[22];var cpu=new DeformedVertex[vertices.Length];var actual=new Vector4[vertices.Length*2];
        using var input=device.CreateBuffer((ulong)vertices.Length*80,unorderedAccess:true);using var palette=device.CreateBuffer(22*128,unorderedAccess:true);
        using var output=device.CreateBuffer((ulong)vertices.Length*32,unorderedAccess:true);
        using var staging=device.CreateBuffer((ulong)vertices.Length*80+22*128,BufferMemory.Upload);
        using var readback=device.CreateBuffer((ulong)vertices.Length*32,BufferMemory.Readback);
        using var queue=device.CreateCommandQueue();using var allocator=device.CreateCommandAllocator();using var commands=device.CreateCommandList(allocator);
        staging.Write<SkinnedMeshVertex>(vertices);commands.Transition(input,ResourceState.Common,ResourceState.CopyDestination);commands.CopyBuffer(staging,input,(ulong)vertices.Length*80);commands.Transition(input,ResourceState.CopyDestination,ResourceState.UnorderedAccess);
        // Explicit transitions before copies keep the test valid on subsequent iterations.
        commands.Close();queue.Wait(queue.Submit(commands));
        float maximumPosition=0,maximumNormal=0;
        for(int clip=0;clip<Model.Animations.Length;++clip)for(int sample=0;sample<3;++sample) {
            player.Play(clip,0);player.Update(Model.Animations[clip].Duration*(sample+0.31)/3);player.Sample(pose);Model.BuildPalette(pose,matrices);Model.MeshSkin(pose,Matrix4x4.Identity,cpu);
            for(int i=0;i<22;++i){Matrix4x4.Invert(matrices[i],out var inverse);joints[i]=new(){Pose=matrices[i],Normal=Matrix4x4.Transpose(inverse)};}
            staging.Write<Joint>(joints,(ulong)vertices.Length*80);
            allocator.Reset();commands.Reset(allocator);
            commands.Transition(palette,clip==0&&sample==0?ResourceState.Common:ResourceState.UnorderedAccess,ResourceState.CopyDestination);
            commands.CopyBuffer(staging,palette,22*128,sourceOffset:(ulong)vertices.Length*80);commands.Transition(palette,ResourceState.CopyDestination,ResourceState.UnorderedAccess);
            if(clip==0&&sample==0)commands.Transition(output,ResourceState.Common,ResourceState.UnorderedAccess);
            commands.SetComputePipeline(pipeline);commands.SetComputeBuffer(0,input);commands.SetComputeBuffer(1,palette);commands.SetComputeBuffer(2,output);commands.SetComputeConstants(3,[(uint)vertices.Length]);commands.Dispatch(((uint)vertices.Length+63)/64);
            commands.Transition(output,ResourceState.UnorderedAccess,ResourceState.CopySource);commands.CopyBuffer(output,readback,(ulong)vertices.Length*32);commands.Transition(output,ResourceState.CopySource,ResourceState.UnorderedAccess);
            commands.Close();queue.Wait(queue.Submit(commands));readback.Read<Vector4>(actual);
            for(int i=0;i<vertices.Length;++i){var p=actual[i*2];var n=actual[i*2+1];maximumPosition=Math.Max(maximumPosition,Vector3.Distance(new(p.X,p.Y,p.Z),cpu[i].Position));maximumNormal=Math.Max(maximumNormal,Vector3.Distance(new(n.X,n.Y,n.Z),cpu[i].Normal));}
        }
        Check(float.IsFinite(maximumPosition)&&maximumPosition<0.0001&&float.IsFinite(maximumNormal)&&maximumNormal<0.001,"GPU matches CPU skinning across 27 real poses");
        Console.WriteLine($"PASS: GPU/CPU skinning of {vertices.Length:N0} vertices across 27 poses; max position error {maximumPosition:G5}, normal error {maximumNormal:G5}.");
    }
}
