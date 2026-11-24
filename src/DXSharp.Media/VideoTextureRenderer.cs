using System.Numerics;
using DXSharp.Framework.Hosting;
using DXSharp.Graphics;
using DXSharp.Shaders;
using Windows.Win32.Graphics.Dxgi.Common;

namespace DXSharp.Media;

/// <summary>Aspect-correct D3D12 presentation of a MediaPlayer's BGRA frames using persistent frame-slot textures and fence-retired uploads.</summary>
/// <remarks>Call Draw only with a retired frame slot supplied by FrameRenderer. The device, player and renderer share the application thread.
/// Dispose after stopping the host. Precompiled shaders are embedded; runtime DXC is unnecessary.</remarks>
public sealed class VideoTextureRenderer : IDisposable {
    private readonly GraphicsDevice _device;
    private readonly MediaPlayer _player;
    private readonly DescriptorPool _descriptors;
    private readonly Texture2D?[] _textures;
    private readonly long[] _versions;
    private readonly RootSignature _root;
    private readonly PipelineState _pipeline;
    private readonly TextureSubresourceData[] _data = new TextureSubresourceData[1];
    private UploadRing? _uploads;
    private ulong _capacity;
    private VideoFrame _latest;
    private long _version, _generation;
    private bool _disposed;
    /// <summary>Constructs the renderer for the host's one to three frame slots.</summary>
    public VideoTextureRenderer(GraphicsDevice device, MediaPlayer player, int frameSlots = 2) {
        ArgumentNullException.ThrowIfNull(device); ArgumentNullException.ThrowIfNull(player);
        if (frameSlots is < 1 or > 3) throw new ArgumentOutOfRangeException(nameof(frameSlots));
        _device = device; _player = player; _textures = new Texture2D[frameSlots]; _versions = new long[frameSlots];
        _descriptors = device.CreateDescriptorPool((uint)frameSlots * 2);
        _root = device.CreateTextureRootSignature(2, linearFiltering: true, wrap: false);
        try { _pipeline = device.CreateGraphicsPipeline(_root, Shader("VS"), Shader("PS")); }
        catch { _root.Dispose(); _descriptors.Dispose(); throw; }
    }
    private static ShaderBytecode Shader(string entry) {
        using var input = typeof(VideoTextureRenderer).Assembly.GetManifestResourceStream($"DXSharp.Media.Video.{entry}.dxil") ?? throw new InvalidOperationException("Missing precompiled media shader.");
        using var output = new MemoryStream(); input.CopyTo(output); return ShaderBytecode.CopyFrom(output.ToArray());
    }
    /// <summary>Number of due frames obtained from the player.</summary>
    public long FramesTransferred => _version;
    /// <summary>Most recently obtained frame; pixels remain borrowed from the player.</summary>
    public VideoFrame LatestFrame => _latest;
    /// <summary>Transfers at most one due frame, updates the retired slot if needed, and draws a letterboxed quad.</summary>
    public void Draw(RenderFrame frame) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if ((uint)frame.FrameSlotIndex >= _textures.Length) throw new ArgumentOutOfRangeException(nameof(frame));
        if (_generation != _player.Generation) { _latest = default; _generation = _player.Generation; }
        if (_player.TryGetFrame(out var video)) { _latest = video; ++_version; }
        if (_latest.Pixels.IsEmpty) return;
        int slot = frame.FrameSlotIndex;
        var texture = _textures[slot]; bool fresh = texture is null || texture.Description.Width != _latest.Width || texture.Description.Height != _latest.Height;
        if (fresh) {
            texture?.Dispose(); _textures[slot] = texture = _device.CreateTexture2D(new(_latest.Width, _latest.Height, DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM), _descriptors);
        }
        if (fresh || _versions[slot] != _version) {
            ulong size = checked((((ulong)_latest.Width * 4 + 255) & ~255UL) * _latest.Height + 512);
            ulong capacity = size * (ulong)_textures.Length;
            if (_uploads is null || capacity > _capacity) { _uploads?.Dispose(); _uploads = _device.CreateUploadRing(capacity); _capacity = capacity; }
            if (!fresh) frame.Commands.Transition(texture!, ResourceState.PixelShaderResource, ResourceState.CopyDestination);
            _data[0] = new(_latest.Pixels, _latest.RowPitch); frame.Commands.UploadTexture(texture!, _uploads, _data);
            frame.Commands.Transition(texture!, ResourceState.CopyDestination, ResourceState.PixelShaderResource); _versions[slot] = _version;
        }
        float videoAspect = (float)_player.DisplayAspectRatio;
        if (!(videoAspect > 0)) videoAspect = _latest.Width / (float)_latest.Height;
        float targetAspect = frame.Size.Width / (float)Math.Max(1, frame.Size.Height);
        float x = Math.Min(1, videoAspect / targetAspect), y = Math.Min(1, targetAspect / videoAspect);
        frame.Commands.SetGraphicsPipeline(_pipeline); frame.Commands.SetGraphicsConstants([BitConverter.SingleToUInt32Bits(x), BitConverter.SingleToUInt32Bits(y)]);
        frame.Commands.SetGraphicsTexture(texture!, 1); frame.Commands.Draw(6);
    }
    /// <summary>Releases the renderer owners after host shutdown. Recorded native resources retain their submission leases.</summary>
    public void Dispose() {
        if (_disposed) return; _disposed = true;
        foreach (var texture in _textures) texture?.Dispose();
        _uploads?.Dispose(); _pipeline.Dispose(); _root.Dispose(); _descriptors.Dispose();
    }
}
