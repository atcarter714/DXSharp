using DXSharp.Framework.Hosting;
using DXSharp.Graphics;
using DXSharp.Shaders;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi;

namespace InteropQualification;

internal static class HostShaderQualification {
    internal static void Run() {
        using var device = GraphicsDevice.Create(new() { UseWarp = true });
        using (var queue = device.CreateCommandQueue()) {
            using var chain = device.CreateCompositionSwapChain(queue, 64, 48);
            if (!chain.IsComposition || chain.BufferCount != 2) throw new InvalidOperationException("Composition swap-chain identity failed.");
            using (var buffer = chain.AcquireBackBuffer(0)) { }
            chain.SetCompositionScale(1.25f, 1.5f);
            queue.WaitIdle(); chain.Resize(80, 72);
            using var native = chain.QueryNativeInterface<IDXGISwapChain2>();
            unsafe {
                DXGI_SWAP_CHAIN_DESC1 description;
                System.Runtime.InteropServices.Marshal.ThrowExceptionForHR((int)native.Interface.GetDesc1(&description));
                DXGI_MATRIX_3X2_F matrix;
                System.Runtime.InteropServices.Marshal.ThrowExceptionForHR((int)native.Interface.GetMatrixTransform(&matrix));
                if (description.Width != 80 || description.Height != 72 ||
                    Math.Abs(matrix._11 - .8f) > .00001f || Math.Abs(matrix._22 - 1f/1.5f) > .00001f)
                    throw new InvalidOperationException("Composition description or inverse scale did not survive resize.");
            }
            bool rejected = false;
            try { chain.SetCompositionScale(float.NaN, 1); } catch (ArgumentOutOfRangeException) { rejected = true; }
            if (!rejected) throw new InvalidOperationException("Non-finite composition scale accepted.");
            using var compute = device.CreateCommandQueue(QueueKind.Compute);
            rejected = false;
            try { using var invalid = device.CreateCompositionSwapChain(compute, 64, 48); } catch (ArgumentException) { rejected = true; }
            if (!rejected) throw new InvalidOperationException("Composition accepted a non-direct queue.");
        }
        Console.WriteLine("PASS: composition factory ABI, owned interface query, buffers, resize, inverse scale and invalid input guards.");
        string folder = Path.Combine(Path.GetTempPath(), "dxsharp include " + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(folder);
        try {
            string source = Path.Combine(folder, "test.hlsl");
            File.WriteAllText(Path.Combine(folder, "value.hlsli"), "#define VALUE 7\n");
            File.WriteAllText(source, "#include \"value.hlsli\"\nRWStructuredBuffer<uint> Data:register(u0);[numthreads(1,1,1)]void CS(){Data[0]=VALUE+EXTRA;}");
            using var compiler = new ShaderCompiler();
            var result = compiler.CompileFile(source, new() { Profile = "cs_6_0", EntryPoint = "CS", Defines = ["EXTRA=3"], DebugInformation = true }).RequireSuccess();
            var input = result.Bytecode.ToArray(); var bytecode = ShaderBytecode.CopyFrom(input); input[0] = 0;
            if (bytecode.Bytes.Span[0] != 'D') throw new InvalidOperationException("Shader bytecode did not own its input.");
            string saved = Path.Combine(folder, "shader.dxil"); bytecode.Save(saved);
            var loaded = ShaderBytecode.Load(saved);
            if (!loaded.Bytes.Span.SequenceEqual(bytecode.Bytes.Span)) throw new InvalidOperationException("Shader bytecode roundtrip failed.");
            using var root = device.CreateRootSignature(1); using var pipeline = device.CreateComputePipeline(root, loaded);
            try { ShaderBytecode.CopyFrom(new byte[32]); throw new InvalidOperationException("Invalid bytecode accepted."); } catch (ArgumentException) { }
            Console.WriteLine("PASS: typed DXC options, filesystem includes, owned shader bytecode, disk roundtrip and precompiled PSO.");
        } finally { Directory.Delete(folder, true); }

        nint handle = 0;
        var stats = NativeGame.Run(device, new() { ClientSize = new(320,240), FramesInFlight = 2, FrameLimit = 7 }, _ => { }, (window, value) => {
            handle = window.Handle;
            if (value.Frames == 2) window.ResizeClient(new(400,280));
        });
        if (stats.Frames != 7 || stats.Resizes < 1 || PInvoke.IsWindow(new HWND(handle))) throw new InvalidOperationException("Native presentation/resize/teardown failed.");
        bool propagated = false;
        try { NativeGame.Run(device, new() { ClientSize = new(160,120), FrameLimit = 2 }, _ => throw new ApplicationException("intentional render failure")); }
        catch (ApplicationException error) when (error.Message == "intentional render failure") { propagated = true; }
        if (!propagated) throw new InvalidOperationException("Native render exception was not propagated.");
        var closed = NativeGame.Run(device, new() { ClientSize = new(160,120) }, _ => { }, (window, _) => window.RequestClose());
        if (closed.Frames != 1) throw new InvalidOperationException("Native close/drain failed.");
        var diagnostics = device.GetDiagnostics();
        if (diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any(x => x.Contains("SEVERITY_ERROR") || x.Contains("SEVERITY_CORRUPTION")))
            throw new InvalidOperationException("Native host emitted device errors: " + string.Join(Environment.NewLine, diagnostics.Messages));
        Console.WriteLine("PASS: native Win32 presentation, resize, frame-slot reuse, render failure, close/drain and HWND teardown.");
    }
}
