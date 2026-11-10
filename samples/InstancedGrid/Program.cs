using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace InstancedGrid;

internal static class Program {
    private const uint GridSize = 16;
    private const string Shader = """
        cbuffer Grid : register(b0) { float time; float aspect; uint side; };
        struct Vertex { float4 position : SV_Position; float3 color : COLOR; };
        Vertex VS(uint vertex : SV_VertexID, uint instance : SV_InstanceID) {
            const float2 corners[6] = {
                float2(-1,-1), float2(-1,1), float2(1,1),
                float2(-1,-1), float2(1,1), float2(1,-1)
            };
            float2 cell = (float2(instance % side, instance / side) + 0.5) / side;
            float2 center = cell * 2 - 1;
            float wave = 0.5 + 0.5 * sin(12 * length(center) - 4 * time);
            float angle = wave * 1.57;
            float2 corner = corners[vertex];
            float2 rotated = float2(corner.x * cos(angle) - corner.y * sin(angle),
                                    corner.x * sin(angle) + corner.y * cos(angle));
            float2 fit = float2(min(1, 1 / aspect), min(1, aspect));
            Vertex output;
            output.position = float4(0.9 * fit * (center + rotated * (0.25 + 0.36 * wave) / side), 0, 1);
            output.color = lerp(float3(0.16, 0.34, 0.86), float3(0.18, 0.95, 0.71), wave);
            return output;
        }
        float4 PS(Vertex input) : SV_Target { return float4(input.color, 1); }
        """;

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
        using var root = device.CreateGraphicsConstantsRootSignature(3);
        using var pipeline = device.CreateGraphicsPipeline(root, compiler.Compile(Shader, "VS", "vs_6_0"),
            compiler.Compile(Shader, "PS", "ps_6_0"), PixelFormat.Rgba8Unorm);

        float time = 0;
        var result = DesktopGame.Run(device, new() { Title = "DXSharp · Instanced Grid", FrameLimit = frameLimit }, frame => {
            time += Math.Min((float)frame.Elapsed.TotalSeconds, 0.1f);
            float aspect = (float)frame.Size.Width / frame.Size.Height;
            ReadOnlySpan<uint> constants = [BitConverter.SingleToUInt32Bits(time), BitConverter.SingleToUInt32Bits(aspect), GridSize];
            frame.Commands.SetGraphicsPipeline(pipeline);
            frame.Commands.SetGraphicsConstants(constants);
            frame.Commands.Draw(6, GridSize * GridSize);
        });
        var diagnostics = device.GetDiagnostics();
        foreach (string message in diagnostics.Messages) Console.WriteLine(message);
        if (diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any(x => x.Contains("SEVERITY_ERROR") || x.Contains("SEVERITY_CORRUPTION"))) return 1;
        if (frameLimit > 0 && result.Frames != frameLimit) return 1;
        Console.WriteLine($"InstancedGrid closed cleanly after {result.Frames} frames.");
        return 0;
    }
}
