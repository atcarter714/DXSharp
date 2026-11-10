using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace PlanarShadows;

internal static class Program {
    private static readonly Vector3 Light = Vector3.Normalize(new Vector3(-0.6f, 1, 0.45f));
    private const string Shader = """
        cbuffer Scene : register(b0) {
            row_major float4x4 transform;
            float4 tint;
        };
        struct Vertex { float4 position : SV_Position; float3 local : LOCAL; float light : LIGHT; };
        Vertex FloorVS(uint id : SV_VertexID) {
            const float2 corners[6] = {
                float2(-1,-1), float2(-1,1), float2(1,1),
                float2(-1,-1), float2(1,1), float2(1,-1)
            };
            Vertex v;
            v.local = float3(corners[id].x, 0, corners[id].y) * 5;
            v.position = mul(float4(v.local, 1), transform);
            v.light = 1;
            return v;
        }
        Vertex PyramidVS(uint id : SV_VertexID) {
            const float3 corners[5] = {
                float3(-1,0,-1), float3(1,0,-1), float3(1,0,1),
                float3(-1,0,1), float3(0,1.65,0)
            };
            const uint indices[12] = { 0,1,4, 1,2,4, 2,3,4, 3,0,4 };
            const float light[4] = { 0.6, 0.8, 1.0, 0.72 };
            Vertex v;
            v.local = corners[indices[id]];
            v.position = mul(float4(v.local, 1), transform);
            v.light = lerp(1, light[id / 3], tint.a);
            return v;
        }
        float4 FloorPS(Vertex v) : SV_Target {
            float checker = fmod(abs(floor(v.local.x) + floor(v.local.z)), 2);
            return float4(tint.rgb * lerp(0.8, 1.0, checker), 1);
        }
        float4 PyramidPS(Vertex v) : SV_Target { return float4(tint.rgb * v.light, 1); }
        """;

    private static readonly (Vector3 Position, Vector4 Color, float Scale)[] Objects = [
        (new(-2.2f, 0, -0.8f), new(0.14f, 0.64f, 0.95f, 1), 0.88f),
        (new(0, 0, 0.7f), new(0.91f, 0.48f, 0.18f, 1), 1.15f),
        (new(2.2f, 0, -0.7f), new(0.32f, 0.84f, 0.57f, 1), 0.78f)
    ];

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct DrawConstants(Matrix4x4 Transform, Vector4 Tint);

    [STAThread]
    public static int Main(string[] args) {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        ApplicationConfiguration.Initialize();
#if DEBUG
        const bool debug = true;
#else
        const bool debug = false;
#endif
        int frameLimit = int.Parse(args.FirstOrDefault(x => x.StartsWith("--frames=", StringComparison.Ordinal))?[9..] ?? "0");
        using var device = GraphicsDevice.Create(new() {
            UseWarp = args.Contains("--warp"),
            UseAgility = !args.Contains("--system-runtime"),
            EnableDebugLayer = debug && !args.Contains("--no-debug")
        });
        Console.WriteLine(device.AdapterName);
        using var compiler = new ShaderCompiler();
        using var root = device.CreateGraphicsConstantsRootSignature(20);
        using var floor = CreatePipeline("FloorVS", "FloorPS");
        using var pyramid = CreatePipeline("PyramidVS", "PyramidPS");

        // Project along the light direction onto y=0, then offset above the floor.
        var shadow = new Matrix4x4(
            1, 0, 0, 0,
            -Light.X / Light.Y, 0, -Light.Z / Light.Y, 0,
            0, 0, 1, 0,
            0, 0.012f, 0, 1);
        float time = 0;
        var result = DesktopGame.Run(device, new() {
            Title = "DXSharp · Planar Shadows", FrameLimit = frameLimit, EnableDepth = true
        }, frame => {
            time += Math.Min((float)frame.Elapsed.TotalSeconds, 0.1f);
            float orbit = 0.38f + time * 0.28f;
            var eye = new Vector3(8.5f * MathF.Sin(orbit), 5.8f, 8.5f * MathF.Cos(orbit));
            var view = Matrix4x4.CreateLookAt(eye, Vector3.Zero, Vector3.UnitY);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 3, (float)frame.Size.Width / frame.Size.Height, 0.1f, 50);
            var viewProjection = view * projection;
            Span<Matrix4x4> worlds = stackalloc Matrix4x4[Objects.Length];
            for (int i = 0; i < Objects.Length; i++) {
                var obj = Objects[i];
                worlds[i] = Matrix4x4.CreateScale(obj.Scale) * Matrix4x4.CreateRotationY(time * (0.65f + i * 0.2f))
                    * Matrix4x4.CreateTranslation(obj.Position);
            }

            frame.Commands.SetGraphicsPipeline(floor);
            Draw(frame.Commands, viewProjection, new(0.25f, 0.29f, 0.36f, 1), 6);

            frame.Commands.SetGraphicsPipeline(pyramid);
            foreach (var world in worlds) Draw(frame.Commands, world * shadow * viewProjection, new(0.09f, 0.11f, 0.16f, 0), 12);
            for (int i = 0; i < worlds.Length; i++) Draw(frame.Commands, worlds[i] * viewProjection, Objects[i].Color, 12);
        });
        var diagnostics = device.GetDiagnostics();
        foreach (string message in diagnostics.Messages) Console.WriteLine(message);
        if (diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any(x => x.Contains("SEVERITY_ERROR") || x.Contains("SEVERITY_CORRUPTION"))) return 1;
        if (frameLimit > 0 && result.Frames != frameLimit) return 1;
        Console.WriteLine($"PlanarShadows closed cleanly after {result.Frames} frames.");
        return 0;

        PipelineState CreatePipeline(string vs, string ps) =>
            device.CreateGraphicsPipeline(root, compiler.Compile(Shader, vs, "vs_6_0"),
                compiler.Compile(Shader, ps, "ps_6_0"), PixelFormat.Rgba8Unorm, depthEnabled: true);
    }

    private static void Draw(GraphicsCommandList commands, Matrix4x4 transform, Vector4 tint, uint vertices) {
        var constants = new DrawConstants(transform, tint);
        commands.SetGraphicsConstants(MemoryMarshal.Cast<DrawConstants, uint>(MemoryMarshal.CreateReadOnlySpan(ref constants, 1)));
        commands.Draw(vertices);
    }
}
