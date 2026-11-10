using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace NBodyGravity;

internal static class Program {
    private const float StepSize = 0.014f;
    private const float Gravity = 0.0018f;
    private const float SofteningSquared = 0.04f;
    private const float Tolerance = 0.0003f;
    private const uint ThreadGroupSize = 64;

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct Body(Vector4 PositionMass, Vector4 Velocity);

    [STAThread]
    public static int Main(string[] args) {
        try {
            bool verify = args.Contains("--verify");
            int count = ReadInt(args, "--bodies=", verify ? 65 : 1024, 1, 4096);
            int frames = ReadInt(args, "--frames=", 0, 0, int.MaxValue);
            int steps = ReadInt(args, "--steps=", 5, 1, 64);
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
            using var root = device.CreateRootSignature(computeBufferCount: 2, computeConstantCount: 4);
            using var integrator = device.CreateComputePipeline(root,
                compiler.Compile(ReadShader("Integrate.hlsl"), "Step", "cs_6_0"));

            Body[] seed = Seed(count);
            ulong byteCount = checked((ulong)MemoryMarshal.AsBytes(seed.AsSpan()).Length);
            using var first = device.CreateBuffer(byteCount, unorderedAccess: true);
            using var second = device.CreateBuffer(byteCount, unorderedAccess: true);
            using (var uploadQueue = device.CreateCommandQueue(QueueKind.Compute)) {
                using var allocator = device.CreateCommandAllocator(QueueKind.Compute);
                using var commands = device.CreateCommandList(allocator);
                using var upload = device.CreateBuffer(byteCount, BufferMemory.Upload);
                upload.Write<Body>(seed);
                commands.Transition(first, ResourceState.Common, ResourceState.CopyDestination);
                commands.CopyBuffer(upload, first, byteCount);
                commands.Transition(first, ResourceState.CopyDestination, ResourceState.Common);
                commands.Close();
                uploadQueue.Wait(uploadQueue.Submit(commands));
            }

            if (verify) {
                Verify(device, integrator, first, second, seed, byteCount, steps);
            } else {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                ApplicationConfiguration.Initialize();
                using var drawRoot = device.CreateGraphicsBufferRootSignature(1);
                string shader = ReadShader("Particles.hlsl");
                using var particles = device.CreateGraphicsPipeline(drawRoot,
                    compiler.Compile(shader, "VS", "vs_6_0"), compiler.Compile(shader, "PS", "ps_6_0"));
                GpuResource source = first, destination = second;
                var result = DesktopGame.Run(device, new() {
                    Title = $"DXSharp · N-Body Gravity · {count:N0} bodies",
                    FrameLimit = frames, FramesInFlight = 2
                }, frame => {
                    var commands = frame.Commands;
                    RecordStep(commands, integrator, source, destination, count, ResourceState.NonPixelShaderResource);
                    commands.SetGraphicsPipeline(particles);
                    commands.SetGraphicsConstants([BitConverter.SingleToUInt32Bits((float)frame.Size.Width / frame.Size.Height)]);
                    commands.SetGraphicsShaderBuffer(destination);
                    commands.Draw(6, (uint)count);
                    commands.Transition(destination, ResourceState.NonPixelShaderResource, ResourceState.Common);
                    (source, destination) = (destination, source);
                }, (window, progress) => {
                    if (frames > 0 && progress.Frames == frames)
                        Console.WriteLine($"Presented {frames} frames. Draining the presentation queue.");
                });
                if (frames > 0 && result.Frames != frames)
                    throw new InvalidOperationException($"Expected {frames} presented frames, got {result.Frames}.");
                Console.WriteLine($"PASS: presented {result.Frames} N-body frames.");
            }

            var diagnostics = device.GetDiagnostics();
            foreach (string message in diagnostics.Messages) Console.WriteLine(message);
            if (diagnostics.DeviceRemovedReason < 0 ||
                diagnostics.Messages.Any(x => x.Contains("SEVERITY_ERROR") || x.Contains("SEVERITY_CORRUPTION")))
                throw new InvalidOperationException("Graphics validation failed.");
            return 0;
        } catch (Exception error) {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static void Verify(GraphicsDevice device, PipelineState integrator, GpuResource source,
        GpuResource destination, Body[] seed, ulong byteCount, int steps) {
        using var queue = device.CreateCommandQueue(QueueKind.Compute);
        using var allocator = device.CreateCommandAllocator(QueueKind.Compute);
        using var commands = device.CreateCommandList(allocator);
        using var readback = device.CreateBuffer(byteCount, BufferMemory.Readback);
        for (int i = 0; i < steps; ++i) {
            var state = i == steps - 1 ? ResourceState.CopySource : ResourceState.Common;
            RecordStep(commands, integrator, source, destination, seed.Length, state);
            (source, destination) = (destination, source);
        }
        commands.CopyBuffer(source, readback, byteCount);
        commands.Transition(source, ResourceState.CopySource, ResourceState.Common);
        commands.Close();
        queue.Wait(queue.Submit(commands));

        var actual = new Body[seed.Length];
        readback.Read<Body>(actual);
        var expected = seed;
        for (int i = 0; i < steps; ++i) expected = CpuStep(expected);
        float worst = 0;
        for (int i = 0; i < actual.Length; ++i) {
            float error = MathF.Max(Vector4.Distance(actual[i].PositionMass, expected[i].PositionMass),
                Vector4.Distance(actual[i].Velocity, expected[i].Velocity));
            if (!float.IsFinite(error) || error > Tolerance)
                throw new InvalidOperationException($"Body {i} after {steps} steps differs by {error:G6} (limit {Tolerance:G6}).");
            worst = MathF.Max(worst, error);
        }
        Console.WriteLine($"PASS: {seed.Length} bodies, {steps} steps, maximum GPU/CPU error {worst:G6}.");
    }

    private static void RecordStep(GraphicsCommandList commands, PipelineState pipeline, GpuResource source,
        GpuResource destination, int count, ResourceState destinationAfter) {
        commands.Transition(source, ResourceState.Common, ResourceState.UnorderedAccess);
        commands.Transition(destination, ResourceState.Common, ResourceState.UnorderedAccess);
        commands.SetComputePipeline(pipeline);
        commands.SetComputeBuffer(0, source);
        commands.SetComputeBuffer(1, destination);
        commands.SetComputeConstants(2, [(uint)count, BitConverter.SingleToUInt32Bits(StepSize),
            BitConverter.SingleToUInt32Bits(Gravity), BitConverter.SingleToUInt32Bits(SofteningSquared)]);
        commands.Dispatch(((uint)count + ThreadGroupSize - 1) / ThreadGroupSize);
        commands.Transition(source, ResourceState.UnorderedAccess, ResourceState.Common);
        commands.Transition(destination, ResourceState.UnorderedAccess, destinationAfter);
    }

    private static Body[] Seed(int count) {
        var random = new Random(0xD125);
        var bodies = new Body[count];
        for (int i = 0; i < count; ++i) {
            float side = (i & 1) == 0 ? -1 : 1;
            float radius = 0.85f * MathF.Sqrt(random.NextSingle());
            float angle = random.NextSingle() * MathF.Tau;
            var (sin, cos) = MathF.SinCos(angle);
            Vector3 offset = new(cos * radius, sin * radius, (random.NextSingle() - 0.5f) * 0.25f);
            Vector3 velocity = side * new Vector3(-offset.Y * 0.5f, offset.X * 0.5f + 0.2f, 0);
            bodies[i] = new(new Vector4(offset + new Vector3(side * 1.1f, 0, 0), 1 + 0.4f * side),
                new Vector4(velocity, 0));
        }
        return bodies;
    }

    private static Body[] CpuStep(Body[] source) {
        var result = new Body[source.Length];
        for (int i = 0; i < source.Length; ++i) {
            var body = source[i];
            Vector3 position = new(body.PositionMass.X, body.PositionMass.Y, body.PositionMass.Z);
            Vector3 acceleration = Vector3.Zero;
            foreach (var neighbor in source) {
                var p = neighbor.PositionMass;
                Vector3 displacement = new(p.X - position.X, p.Y - position.Y, p.Z - position.Z);
                float inverse = 1 / MathF.Sqrt(Vector3.Dot(displacement, displacement) + SofteningSquared);
                acceleration += displacement * (p.W * inverse * inverse * inverse);
            }
            var velocity = body.Velocity + new Vector4(acceleration * (Gravity * StepSize), 0);
            result[i] = new(body.PositionMass + velocity * StepSize, velocity);
        }
        return result;
    }

    private static string ReadShader(string name) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, name));

    private static int ReadInt(string[] args, string prefix, int fallback, int min, int max) {
        string? value = args.FirstOrDefault(x => x.StartsWith(prefix, StringComparison.Ordinal));
        if (value is null) return fallback;
        if (!int.TryParse(value.AsSpan(prefix.Length), out int parsed) || parsed < min || parsed > max)
            throw new ArgumentException($"{prefix} requires a value from {min} through {max}.");
        return parsed;
    }
}
