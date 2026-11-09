using System.Numerics;
using System.Text;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace DxrHelloWorld;

internal static class Program {
    private const uint Width = 256, Height = 192;
    private static readonly Vector3[] Vertices = [new(-0.75f, -0.62f, 0), new(0, 0.72f, 0), new(0.75f, -0.62f, 0)];
    private static readonly Vector3 Background = new(0.04f, 0.06f, 0.1f);

    public static int Main(string[] args) {
        try {
#if DEBUG
            const bool debug = true;
#else
            const bool debug = false;
#endif
            using var device = GraphicsDevice.Create(new() {
                UseWarp = args.Contains("--warp"),
                UseAgility = !args.Contains("--system-runtime"),
                EnableDebugLayer = debug && !args.Contains("--no-debug")
            });
            Console.WriteLine(device.AdapterName);
            using var compiler = new ShaderCompiler();
            var library = compiler.CompileLibrary(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Triangle.hlsl"))).RequireSuccess();
            if (library.Bytecode.IsEmpty) throw new InvalidOperationException("DXC returned an empty DXIL library.");
            Console.WriteLine($"PASS: compiled {library.Bytecode.Length:N0} bytes of DXR shader library.");
            if (!device.SupportsRaytracing) {
                Console.WriteLine("SKIP: the selected adapter/runtime does not expose DXR.");
                return args.Contains("--require-dxr") ? 1 : 0;
            }

            using var queue = device.CreateCommandQueue();
            using var scene = device.CreateRaytracingScene(queue, Vertices);
            using var pipeline = device.CreateRaytracingPipeline(library, "RayGen", "Miss", "ClosestHit", payloadSizeInBytes: 12);
            using var allocator = device.CreateCommandAllocator();
            using var commands = device.CreateCommandList(allocator);
            const ulong byteCount = Width * Height * sizeof(uint);
            using var output = device.CreateBuffer(byteCount, unorderedAccess: true);
            using var readback = device.CreateBuffer(byteCount, BufferMemory.Readback);

            commands.Transition(output, ResourceState.Common, ResourceState.UnorderedAccess);
            commands.DispatchRays(pipeline, scene, output, Width, Height);
            commands.Transition(output, ResourceState.UnorderedAccess, ResourceState.CopySource);
            commands.CopyBuffer(output, readback, byteCount);
            commands.Close();
            // Submission leases also hold the TLAS, BLAS, table and state object.
            pipeline.Dispose();
            scene.Dispose();
            output.Dispose();
            queue.Wait(queue.Submit(commands));

            var pixels = new uint[Width * Height];
            readback.Read<uint>(pixels);
            ValidateImage(pixels);
            var diagnostics = device.GetDiagnostics();
            foreach (var message in diagnostics.Messages) Console.WriteLine(message);
            if (diagnostics.DeviceRemovedReason < 0 ||
                diagnostics.Messages.Any(message => message.Contains("SEVERITY_ERROR") || message.Contains("SEVERITY_CORRUPTION")))
                throw new InvalidOperationException("DXR validation reported graphics errors.");

            const string prefix = "--output=";
            string outputPath = args.FirstOrDefault(arg => arg.StartsWith(prefix, StringComparison.Ordinal))?[prefix.Length..]
                ?? "artifacts/dxr-hello-world.ppm";
            WritePpm(outputPath, pixels);
            Console.WriteLine($"PASS: DXR hit and miss pixels validated. Image saved to {Path.GetFullPath(outputPath)}.");
            return 0;
        } catch (Exception error) {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static void ValidateImage(ReadOnlySpan<uint> pixels) {
        var a = Vertices[0];
        var ab = Vertices[1] - a;
        var ac = Vertices[2] - a;
        float determinant = ab.X * ac.Y - ab.Y * ac.X;
        int hit = 0, miss = 0;
        for (uint y = 0; y < Height; ++y) {
            for (uint x = 0; x < Width; ++x) {
                float px = 2 * (x + 0.5f) / Width - 1 - a.X;
                float py = 1 - 2 * (y + 0.5f) / Height - a.Y;
                float u = (px * ac.Y - py * ac.X) / determinant;
                float v = (ab.X * py - ab.Y * px) / determinant;
                float w = 1 - u - v;
                float edge = MathF.Min(w, MathF.Min(u, v));
                if (MathF.Abs(edge) < 0.005f) continue;
                Vector3 expected = edge > 0 ? new(w, u, v) : Background;
                uint actual = pixels[(int)(y * Width + x)];
                if ((actual >> 24) != 255 || !Near(actual, 0, expected.X) ||
                    !Near(actual, 8, expected.Y) || !Near(actual, 16, expected.Z))
                    throw new InvalidOperationException($"DXR pixel ({x}, {y}) was 0x{actual:X8}, expected RGB near {expected}.");
                if (edge > 0) ++hit;
                else ++miss;
            }
        }
        if (hit == 0 || miss == 0) throw new InvalidOperationException("The image check requires both hits and misses.");
        Console.WriteLine($"Verified {hit:N0} hit pixels and {miss:N0} miss pixels.");
    }

    private static bool Near(uint pixel, int shift, float expected) =>
        MathF.Abs((byte)(pixel >> shift) - expected * 255) <= 2;

    private static void WritePpm(string path, ReadOnlySpan<uint> pixels) {
        string fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        using var stream = File.Create(fullPath);
        stream.Write(Encoding.ASCII.GetBytes($"P6\n{Width} {Height}\n255\n"));
        var rgb = new byte[pixels.Length * 3];
        for (int i = 0; i < pixels.Length; ++i) {
            rgb[i * 3] = (byte)pixels[i];
            rgb[i * 3 + 1] = (byte)(pixels[i] >> 8);
            rgb[i * 3 + 2] = (byte)(pixels[i] >> 16);
        }
        stream.Write(rgb);
    }
}
