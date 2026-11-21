using DXSharp;
using DXSharp.Graphics;
using Windows.Win32;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;

namespace InteropQualification;

internal static unsafe class FeatureQueryQualification {
    internal static void Run() {
        foreach (bool agility in new[] { true, false }) {
            using var device = GraphicsDevice.Create(new() { UseWarp = true, UseAgility = agility, EnableDebugLayer = true });
            bool versioned = device.Native.TryQuery<ID3D12Device16>(typeof(ID3D12Device16).GUID, out var probe);
            probe?.Dispose();
            D3D12_FEATURE_DATA_FORMAT_INFO planes = new() { Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM };
            Results.Check(device.QueryFeatureSupport(D3D12_FEATURE.D3D12_FEATURE_FORMAT_INFO, &planes, (uint)sizeof(D3D12_FEATURE_DATA_FORMAT_INFO)), "QueryFeatureSupport(format planes)");
            D3D12_FEATURE_DATA_FORMAT_SUPPORT format = new() { Format = planes.Format };
            Results.Check(device.QueryFeatureSupport(D3D12_FEATURE.D3D12_FEATURE_FORMAT_SUPPORT, &format, (uint)sizeof(D3D12_FEATURE_DATA_FORMAT_SUPPORT)), "QueryFeatureSupport(format support)");
            if (planes.PlaneCount != 1 || (format.Support1 & D3D12_FORMAT_SUPPORT1.D3D12_FORMAT_SUPPORT1_TEXTURE2D) == 0)
                throw new InvalidOperationException("RGBA8 format capability query returned invalid data.");
            var shader = device.GetHighestShaderModel();
            if (shader.Major < 5) throw new InvalidOperationException("Invalid Shader Model capability.");
            _ = device.SupportsRaytracing;
            var diagnostics = device.GetDiagnostics();
            if (diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any(m =>
                m.Contains("SEVERITY_ERROR", StringComparison.Ordinal) || m.Contains("SEVERITY_CORRUPTION", StringComparison.Ordinal) ||
                m.Contains("CheckFeatureSupport is superseded", StringComparison.Ordinal)))
                throw new InvalidOperationException("Feature query diagnostics: " + string.Join(Environment.NewLine, diagnostics.Messages));
            Console.WriteLine($"PASS: {(agility ? "Agility" : "system")} feature queries via {(versioned ? "CheckFeatureSupport1" : "legacy fallback")}; metadata SDK {PInvoke.D3D12_SDK_VERSION}; no deprecated-query warning.");
        }
    }
}
