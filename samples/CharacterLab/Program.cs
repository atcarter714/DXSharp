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
#if DEBUG
            bool debug=!args.Contains("--no-debug");
#else
            bool debug=false;
#endif
            using var device=GraphicsDevice.Create(new(){UseWarp=args.Contains("--warp"),UseAgility=!args.Contains("--system-runtime"),EnableDebugLayer=debug});
            using var scene=new CharacterScene(device,modelPath,frameLimit>0);
#if NATIVE_HOST
            Console.WriteLine("Host: native Win32 (no Windows Forms reference).");
            var result=NativeGame.Run(device,new(){Title="DXSharp · Character Lab — Native Win32",ClientSize=new(1280,800),EnableDepth=true,FramesInFlight=2,FrameLimit=frameLimit},scene.Draw,(window,stats)=>{
                if(scene.UpdateTitle(stats.Frames) is {} title)window.Title=title;
                if(args.Contains("--resize-test") && stats.Frames==2)window.ResizeClient(new(960,640));
            });
#else
            var desktop=DesktopGame.Run(device,new(){Title="DXSharp · Character Lab — GPU Skinning",ClientSize=new(1280,800),EnableDepth=true,FramesInFlight=2,FrameLimit=frameLimit},
                frame=>scene.Draw(new(frame.Commands,frame.Target,frame.Size,frame.Elapsed,frame.FrameSlotIndex)),(window,stats)=>{
                    if(scene.UpdateTitle(stats.Frames) is {} title)window.Text=title;
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
            Console.WriteLine($"PASS: CharacterLab presented {result.Frames} GPU-skinned frames; persistent geometry; mean CPU recording {scene.MeanRecordingMilliseconds:F2} ms.");return 0;
        } catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
