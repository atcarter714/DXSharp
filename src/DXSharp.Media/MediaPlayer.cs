using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using DXSharp.Interop;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Imaging;
using Windows.Win32.Media.MediaFoundation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;

namespace DXSharp.Media;

/// <summary>A borrowed BGRA video frame. Pixels are valid until the next frame transfer, Open or Dispose on this player.</summary>
/// <param name="Width">Physical width.</param><param name="Height">Physical height.</param><param name="RowPitch">Bytes per row.</param>
/// <param name="Pixels">Borrowed tightly packed BGRA bytes.</param><param name="Timestamp">Media presentation timestamp.</param><param name="Generation">Source/seek generation.</param>
public readonly record struct VideoFrame(uint Width, uint Height, uint RowPitch, ReadOnlyMemory<byte> Pixels, TimeSpan Timestamp, long Generation);

/// <summary>Playback and transfer telemetry. Presentation offset is video PTS minus the engine's A/V clock, not independently measured speaker latency.</summary>
/// <param name="FramesTransferred">Frames copied into caller-visible CPU storage.</param><param name="FramesDropped">Engine-reported dropped frames, or null if unavailable.</param>
/// <param name="FramesPerSecond">Engine-reported FPS, or null if unavailable.</param><param name="PresentationOffset">Most recent video PTS minus playback clock.</param>
/// <param name="TransferPath">Actual transfer backend.</param><param name="HardwareDecoding">Null when decoder acceleration has not been established.</param>
public readonly record struct PlaybackStatistics(long FramesTransferred, long? FramesDropped, double? FramesPerSecond, TimeSpan PresentationOffset, string TransferPath, bool? HardwareDecoding);

/// <summary>Thread-owned local-file Media Foundation playback. Media Engine owns audio output and A/V scheduling; frames are exposed for application rendering.</summary>
/// <remarks>Create, call and dispose on one COM-capable application thread. Dispose before that thread exits. No UI framework dependency is acquired.
/// This baseline transfers through a writable WIC bitmap into bounded CPU storage; it does not claim direct D3D12 decoding or zero-copy interop.</remarks>
public sealed class MediaPlayer : IDisposable {
    private readonly int _thread = Environment.CurrentManagedThreadId;
    private readonly EngineNotify _notify = new();
    private NativeInterface<IMFMediaEngine>? _engine;
    private NativeInterface<IMFMediaEngineEx>? _extended;
    private NativeInterface<IWICBitmap>? _bitmap;
    private MemoryMappedFile? _section;
    private MemoryMappedViewAccessor? _view;
    private byte[] _pixels = [];
    private uint _width, _height;
    private long _frames, _generation;
    private TimeSpan _offset;
    private bool _com, _mf, _disposed, _opened;
    private IMFMediaEngine Engine { get { Check(); return _engine!.Interface; } }

    /// <summary>Initializes COM/MF and creates an independent frame-server engine. Playback starts only after Open and Play.</summary>
    public unsafe MediaPlayer() {
        try {
            int hr = (int)PInvoke.CoInitializeEx(null, COINIT.COINIT_MULTITHREADED);
            if (hr < 0 && hr != unchecked((int)0x80010106)) CheckResult(hr);
            _com = hr >= 0;
            CheckResult((int)PInvoke.MFStartup(0x20070, 0)); _mf = true;
            CheckResult(MediaNative.CreateAttributes(out nint attributes, 3));
            using var store = NativeInterface<IMFAttributes>.Adopt(attributes);
            Guid callback = PInvoke.MF_MEDIA_ENGINE_CALLBACK, output = PInvoke.MF_MEDIA_ENGINE_VIDEO_OUTPUT_FORMAT;
            CheckResult((int)store.Interface.SetUnknown(&callback, _notify));
            CheckResult((int)store.Interface.SetUINT32(&output, 87)); // DXGI_FORMAT_B8G8R8A8_UNORM
            CheckResult(MediaNative.CreateInstance(PInvoke.CLSID_MFMediaEngineClassFactory, 0, 1, typeof(IEngineFactoryRaw).GUID, out nint factory));
            using var ownedFactory = NativeInterface<IEngineFactoryRaw>.Adopt(factory);
            CheckResult(ownedFactory.Interface.CreateInstance(0, store.Pointer, out nint engine));
            _engine = NativeInterface<IMFMediaEngine>.Adopt(engine);
            Guid extended = typeof(IMFMediaEngineEx).GUID;
            if (Marshal.QueryInterface(engine, in extended, out nint pointer) >= 0) _extended = NativeInterface<IMFMediaEngineEx>.Adopt(pointer);
        } catch { Dispose(); throw; }
    }
    private void Check() { if (Environment.CurrentManagedThreadId != _thread) throw new InvalidOperationException("MediaPlayer belongs to its creating thread."); ObjectDisposedException.ThrowIf(_disposed, this); }
    internal static void CheckResult(int result) { if (result < 0) Marshal.ThrowExceptionForHR(result); }
    /// <summary>Opens an existing local media file. Network URLs and protected playback are outside this API.</summary>
    public unsafe void Open(string path) {
        Check(); string file = Path.GetFullPath(path); if (!File.Exists(file)) throw new FileNotFoundException("Media file not found.", file);
        if (_opened) CheckResult((int)Engine.Pause()); _opened = false; _notify.Clear(); ReleaseBitmap(); _frames = 0; _offset = default; ++_generation;
        nint text = Marshal.StringToBSTR(new Uri(file).AbsoluteUri);
        try { CheckResult((int)Engine.SetSource(new BSTR((char*)text))); CheckResult((int)Engine.Load()); _opened = true; }
        finally { Marshal.FreeBSTR(text); }
    }
    /// <summary>Starts/resumes the current source.</summary>
    public void Play() { RequireSource(); CheckResult((int)Engine.Play()); }
    /// <summary>Pauses the presentation clock.</summary>
    public void Pause() { RequireSource(); CheckResult((int)Engine.Pause()); }
    /// <summary>Seeks within the finite source duration. Frames from the old seek generation are invalidated.</summary>
    public void Seek(TimeSpan position) {
        RequireSource(); double seconds = position.TotalSeconds;
        if (seconds < 0 || !double.IsFinite(seconds) || !double.IsFinite(Engine.GetDuration()) || seconds > Engine.GetDuration()) throw new ArgumentOutOfRangeException(nameof(position));
        ++_generation; _offset = default; CheckResult((int)(_extended is null ? Engine.SetCurrentTime(seconds) : _extended.Interface.SetCurrentTimeEx(seconds, MF_MEDIA_ENGINE_SEEK_MODE.MF_MEDIA_ENGINE_SEEK_MODE_NORMAL)));
    }
    private void RequireSource() { Check(); if (!_opened) throw new InvalidOperationException("Open a media source first."); ThrowIfFailed(); }
    /// <summary>Monotonically increasing source/seek generation for invalidating borrowed frames.</summary>
    public long Generation { get { Check(); return _generation; } }
    /// <summary>Display aspect ratio reported by the source, including pixel aspect ratio.</summary>
    public unsafe double DisplayAspectRatio { get { uint x = 0, y = 0; CheckResult((int)Engine.GetVideoAspectRatio(&x, &y)); return y == 0 ? 0 : x / (double)y; } }
    /// <summary>Current media time.</summary>
    public TimeSpan Position => Time(Engine.GetCurrentTime());
    /// <summary>Finite duration, or zero until metadata becomes available.</summary>
    public TimeSpan Duration => Time(Engine.GetDuration());
    /// <summary>Whether metadata and current playback data are available.</summary>
    public bool IsReady => Engine.GetReadyState() >= 2;
    /// <summary>Whether playback is paused.</summary>
    public bool IsPaused => Engine.IsPaused();
    /// <summary>Whether a seek is in progress.</summary>
    public bool IsSeeking => Engine.IsSeeking();
    /// <summary>Whether the current non-looping source reached end-of-stream.</summary>
    public bool IsEnded => Engine.IsEnded();
    /// <summary>Whether the source has an audio stream, once metadata is loaded.</summary>
    public bool HasAudio => Engine.HasAudio();
    /// <summary>Whether the source has a video stream.</summary>
    public bool HasVideo => Engine.HasVideo();
    /// <summary>Linear volume in [0,1].</summary>
    public double Volume { get => Engine.GetVolume(); set { if (!double.IsFinite(value) || value < 0 || value > 1) throw new ArgumentOutOfRangeException(nameof(value)); CheckResult((int)Engine.SetVolume(value)); } }
    /// <summary>Audio muting without changing playback time.</summary>
    public bool Muted { get => Engine.GetMuted(); set => CheckResult((int)Engine.SetMuted(value)); }
    /// <summary>Native end-of-stream looping.</summary>
    public bool Loop { get => Engine.GetLoop(); set => CheckResult((int)Engine.SetLoop(value)); }
    /// <summary>Throws a reported asynchronous media/decoder/output error on the application thread.</summary>
    public void ThrowIfFailed() { Check(); int error = Volatile.Read(ref _notify.Error); if (error != 0) throw new COMException($"Media Engine reported error {Volatile.Read(ref _notify.ErrorCode)} (HRESULT 0x{error:X8}).", error); }
    /// <summary>Returns one due frame. Poll on the render thread; S_FALSE, seeking and audio-only sources return false without allocation.</summary>
    public unsafe bool TryGetFrame(out VideoFrame frame) {
        RequireSource(); frame = default;
        if (!IsReady || !HasVideo) return false;
        uint width = 0, height = 0; CheckResult((int)Engine.GetNativeVideoSize(&width, &height));
        if (width == 0 || height == 0) return false;
        if (width != _width || height != _height) CreateBitmap(width, height);
        int result = (int)Engine.OnVideoStreamTick(out long ticks); CheckResult(result); if (result != 0) return false;
        RECT destination = new(0, 0, checked((int)width), checked((int)height)); MFARGB border = new() { rgbAlpha = 255 };
        CheckResult((int)Engine.TransferVideoFrame(_bitmap!.Interface, null, &destination, &border));
        // Continue ticking/transferring while seeking so the native frame-server can complete its presentation transition.
        // Suppress publication of the old seek generation until the engine reports completion.
        if (IsSeeking) return false;
        _view!.ReadArray(0, _pixels, 0, _pixels.Length);
        var time = TimeSpan.FromTicks(ticks); _offset = time - Position; ++_frames;
        frame = new(width, height, width * 4, _pixels, time, _generation); return true;
    }
    private void CreateBitmap(uint width, uint height) {
        if (width > 8192 || height > 8192) throw new NotSupportedException("The CPU-transfer backend accepts video up to 8192 pixels per dimension.");
        int bytes = checked((int)((ulong)width * height * 4));
        ReleaseBitmap();
        try {
            _section = MemoryMappedFile.CreateNew(null, bytes, MemoryMappedFileAccess.ReadWrite);
            _view = _section.CreateViewAccessor(0, bytes, MemoryMappedFileAccess.ReadWrite);
            CheckResult(MediaNative.CreateBitmap(width, height, PInvoke.GUID_WICPixelFormat32bppBGRA, _section.SafeMemoryMappedFileHandle.DangerousGetHandle(), width * 4, 0, 3, out nint bitmap));
            _bitmap = NativeInterface<IWICBitmap>.Adopt(bitmap); _pixels = new byte[bytes]; _width = width; _height = height;
        } catch { ReleaseBitmap(); throw; }
    }
    /// <summary>Snapshot of timing, dropped-frame and transfer-path diagnostics. Decoder acceleration is explicitly unmeasured.</summary>
    public PlaybackStatistics Statistics { get { Check(); return new(_frames, Statistic(MF_MEDIA_ENGINE_STATISTIC.MF_MEDIA_ENGINE_STATISTIC_FRAMES_DROPPED) is { } drops ? (long)drops : null,
        Statistic(MF_MEDIA_ENGINE_STATISTIC.MF_MEDIA_ENGINE_STATISTIC_FRAMES_PER_SECOND), _offset, "Media Foundation → WIC BGRA → CPU staging → D3D12 upload", null); } }
    private double? Statistic(MF_MEDIA_ENGINE_STATISTIC statistic) {
        if (_extended is null || (int)_extended.Interface.GetStatistics(statistic, out var value) < 0) return null;
        try { return value.vt switch { VARENUM.VT_UI4 => value.ulVal, VARENUM.VT_UI8 => value.uhVal, VARENUM.VT_R8 => value.dblVal, VARENUM.VT_R4 => value.fltVal, _ => null }; }
        finally { PInvoke.PropVariantClear(ref value); }
    }
    private static TimeSpan Time(double seconds) => double.IsFinite(seconds) && seconds >= 0 && seconds <= TimeSpan.MaxValue.TotalSeconds ? TimeSpan.FromSeconds(seconds) : TimeSpan.Zero;
    private void ReleaseBitmap() { _bitmap?.Dispose(); _bitmap = null; _view?.Dispose(); _view = null; _section?.Dispose(); _section = null; _pixels = []; _width = _height = 0; }
    /// <summary>Shuts down the engine before releasing callback, frame mapping and MF/COM initialization. Idempotent on the creating thread.</summary>
    public void Dispose() {
        if (_disposed) return; Check(); _disposed = true;
        try { if (_engine is not null) CheckResult((int)_engine.Interface.Shutdown()); }
        finally {
            _extended?.Dispose(); _extended = null; _engine?.Dispose(); _engine = null; _notify.Close(); ReleaseBitmap();
            try { if (_mf) CheckResult((int)PInvoke.MFShutdown()); }
            finally { _mf = false; if (_com) PInvoke.CoUninitialize(); _com = false; }
        }
    }
}

[GeneratedComInterface(Options = ComInterfaceOptions.ComObjectWrapper)]
[Guid("4D645ACE-26AA-4688-9BE1-DF3516990B93")]
internal partial interface IEngineFactoryRaw {
    [PreserveSig] int CreateInstance(uint flags, nint attributes, out nint engine);
    [PreserveSig] int CreateTimeRange(out nint range);
    [PreserveSig] int CreateError(out nint error);
}

[GeneratedComClass]
internal sealed partial class EngineNotify : IMFMediaEngineNotify {
    internal int Error, ErrorCode;
    private int _closed;
    internal void Clear() { Volatile.Write(ref Error, 0); Volatile.Write(ref ErrorCode, 0); }
    internal void Close() => Volatile.Write(ref _closed, 1);
    public HRESULT EventNotify(uint @event, nuint param1, uint param2) {
        if (@event == (uint)MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_NOTIFYSTABLESTATE) { PInvoke.SetEvent(new HANDLE((nint)param1)); return new(0); }
        if (Volatile.Read(ref _closed) != 0) return new(0);
        if (@event is (uint)MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_ERROR or (uint)MF_MEDIA_ENGINE_EVENT.MF_MEDIA_ENGINE_EVENT_STREAMRENDERINGERROR) {
            Volatile.Write(ref ErrorCode, unchecked((int)param1)); Volatile.Write(ref Error, unchecked((int)param2) < 0 ? unchecked((int)param2) : unchecked((int)0x80004005));
        }
        return new(0);
    }
}
