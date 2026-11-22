using System.Diagnostics;
using System.Numerics;
using DXSharp.Assets;
using DXSharp.Framework;
using DXSharp.Framework.Hosting;
#if !NATIVE_HOST
using DXSharp.Framework.Desktop;
#endif
using DXSharp.Framework.Graphics;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace DXSharp.CharacterLab;

internal static class Program {
    [STAThread]
    private static int Main(string[] args) {
        try {
#if !NATIVE_HOST
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            ApplicationConfiguration.Initialize();
#endif
            string modelPath=args.FirstOrDefault(a=>a.StartsWith("--model="))?[8..]??Path.Combine(AppContext.BaseDirectory,"Assets","animatedKnightInPlateArmour.glb");
            int frameLimit=int.Parse(args.FirstOrDefault(a=>a.StartsWith("--frames="))?[9..]??"0");
            var model=SkinnedModel.Load(modelPath);
            Console.WriteLine($"CharacterLab: {model.SourceVertices.Length:N0} vertices, {model.Skin.Joints.Length} joints, {model.Animations.Length} clips.");
#if DEBUG
            bool debug=!args.Contains("--no-debug");
#else
            bool debug=false;
#endif
            using var device=GraphicsDevice.Create(new(){UseWarp=args.Contains("--warp"),UseAgility=!args.Contains("--system-runtime"),EnableDebugLayer=debug});
            using var renderer=new SkinnedModelRenderer(device,model,2);
            Console.WriteLine("Precompiled DXIL; no runtime shader compilation.");
            int root=Array.FindIndex(model.Nodes,n=>n.Name.Equals("pelvis",StringComparison.OrdinalIgnoreCase));
            var player=new AnimationPlayer(model,root);
            var pose=new SkeletonPose(model.Nodes);var camera=new OrbitCamera();
            Console.WriteLine(device.AdapterName);
            Console.WriteLine("Drag: orbit | +/-: zoom | 1-9: clip | Space: pause | R: idle | [ / ]: speed");
            Console.WriteLine("Persistent GPU skinning; in-place horizontal root motion; four material maps; two fenced frame slots.");
            double drawMilliseconds=0;long draws=0;double elapsed=0;
            void Draw(RenderFrame frame) {
                long start=Stopwatch.GetTimestamp();
                double dt=frame.Elapsed.TotalSeconds;camera.Update((float)dt);
                for(int i=0;i<model.Animations.Length && i<9;++i) if(camera.WasPressed(0x31+i)) player.Play(i,loop:i<5);
                if(camera.WasPressed(0x20))player.Paused=!player.Paused;
                if(camera.WasPressed(0x52))player.Play(model.FindAnimation("Idle"));
                if(camera.WasPressed(0xDB))player.Speed=Math.Max(0.1f,player.Speed-0.1f);
                if(camera.WasPressed(0xDD))player.Speed=Math.Min(2.5f,player.Speed+0.1f);
                // Bounded qualification crosses several clips rather than showing only the initial pose.
                if(frameLimit>0){player.Play((int)(draws%model.Animations.Length),0);dt=0.37;}
                player.Update(dt);player.Sample(pose);
                var view=Matrix4x4.CreateLookAt(camera.Eye,new(0,0.92f,0),Vector3.UnitY);
                var projection=Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI/3,Math.Max(1,frame.Size.Width)/(float)Math.Max(1,frame.Size.Height),0.08f,100);
                renderer.Draw(frame.Commands,pose,Matrix4x4.Identity,view*projection,camera.Eye,frame.FrameSlotIndex);
                drawMilliseconds+=Stopwatch.GetElapsedTime(start).TotalMilliseconds;++draws;elapsed+=frame.Elapsed.TotalSeconds;
            }
            string? UpdateTitle(int frames) {
                if(frames%60!=0)return null;
                string title=$"DXSharp · Character Lab | {player.CurrentClipName} | {60/Math.Max(elapsed,0.001):F0} FPS | CPU record {drawMilliseconds/Math.Max(draws,1):F2} ms";
                elapsed=0;return title;
            }
#if NATIVE_HOST
            Console.WriteLine("Host: native Win32 (no Windows Forms reference).");
            var result=NativeGame.Run(device,new(){Title="DXSharp · Character Lab — Native Win32",ClientSize=new(1280,800),EnableDepth=true,FramesInFlight=2,FrameLimit=frameLimit},Draw,(window,stats)=>{
                if(UpdateTitle(stats.Frames) is {} title)window.Title=title;
                if(args.Contains("--resize-test") && stats.Frames==2)window.ResizeClient(new(960,640));
            });
#else
            var desktop=DesktopGame.Run(device,new(){Title="DXSharp · Character Lab — GPU Skinning",ClientSize=new(1280,800),EnableDepth=true,FramesInFlight=2,FrameLimit=frameLimit},
                frame=>Draw(new(frame.Commands,frame.Target,frame.Size,frame.Elapsed,frame.FrameSlotIndex)),(window,stats)=>{
                    if(UpdateTitle(stats.Frames) is {} title)window.Text=title;
                    if(args.Contains("--close-test") && stats.Frames==1) window.FormClosed+=(_,_)=>{
                        // Exercise dispatch after renderer retirement; queued timers must be inert.
                        Thread.Sleep(40);Application.DoEvents();
                    };
                });
            var result=new RenderStatistics(desktop.Frames,desktop.Resizes);
#endif
#if NATIVE_HOST
            if(Process.GetCurrentProcess().Modules.Cast<ProcessModule>().Any(m=>m.ModuleName.Equals("dxcompiler.dll",StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("The precompiled native sample loaded DXC at runtime.");
#endif
            var diagnostics=device.GetDiagnostics();foreach(var message in diagnostics.Messages)Console.WriteLine(message);
            if(diagnostics.DeviceRemovedReason<0 || diagnostics.Messages.Any(m=>m.Contains("SEVERITY_ERROR")||m.Contains("SEVERITY_CORRUPTION")))return 1;
            if(frameLimit>0 && result.Frames!=frameLimit)return 1;
            Console.WriteLine($"PASS: CharacterLab presented {result.Frames} GPU-skinned frames; persistent geometry; mean CPU recording {drawMilliseconds/Math.Max(draws,1):F2} ms.");return 0;
        } catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
