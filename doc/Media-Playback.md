# Media playback

Preview 4 development adds the optional `DXSharp.Media` assembly and one `samples/VideoPlayer` application. The application reuses the existing WinUI composition host. CharacterLab remains the shared host-regression scene; media functionality is implemented in a reusable library.

## Native declarations and ownership

DXSharp.Native projects the complete pinned `Windows.Win32.Media.MediaFoundation` namespace. The name/member/IID gate measures 1,650 top-level types and 8,484 member names against Win32Metadata 71.0.45-preview and the selected Agility metadata. This includes the pinned Direct3D 12 video interfaces located in that namespace. Declaration coverage does not establish every ABI shape, driver capability, codec or video-queue workflow.

`MediaNative` supplies raw owned-output overloads for MF attribute creation, in-process COM activation and writable WIC section bitmaps. Each successful output transfers one native reference. Media uses Core's `NativeInterface<T>` leases and a source-generated factory shim so output references and private projections are deterministically released. Media Engine callbacks use a generated managed COM class; native callback threads record errors, while the application thread owns playback, presentation and disposal.

## Playback contract

Create `MediaPlayer` on the application thread, call `Open` with an existing local file, then `Play`. Pause, seek, volume, mute and looping are exposed. Media Engine supplies the native playback pipeline, audio output and A/V presentation clock. No custom decode thread or unbounded managed frame queue is created.

`TryGetFrame` polls `OnVideoStreamTick` and transfers one due frame through an explicitly writable WIC bitmap. It returns borrowed tightly packed BGRA bytes, native video PTS and a source/seek generation. The bytes remain valid until the next frame transfer, Open or Dispose on that player. Copy them if retaining them. Missing frames and in-progress seeks return false. Open and Seek invalidate the renderer's old-generation display. Source and display aspect ratios remain distinct.

This initial backend is **Media Foundation → WIC BGRA → CPU staging → D3D12 upload**. It supplies SDR 8-bit presentation and does not claim zero-copy interop, HDR handling, or direct D3D12 decoding. Hardware acceleration is reported as unmeasured, not inferred from successful playback. Codec availability depends on installed Windows media components. Network/protected sources are outside the local-file wrapper.

`VideoTextureRenderer.Draw(RenderFrame)` uses persistent per-slot textures and the shared upload ring. The host must retire the supplied frame slot before reuse. Every slot receives the newest CPU frame before it is sampled, avoiding alternating stale frames when display and video rates differ. The source display aspect is fitted to the target with letterboxing. Shaders are precompiled and embedded in Media; the sample excludes runtime DXC DLLs.

Stop and await the viewport, dispose the video renderer, then dispose the player and device. MediaPlayer shuts down the engine before releasing its interfaces, callback owner, bitmap and mapping, and balances MF/COM initialization on the creating thread. Dispose is idempotent; other calls after disposal fail. The sample uses explicit WinUI dispatcher shutdown so asynchronous cleanup and post-close checks complete before the message loop exits.

## Sample and evidence

Run:

```powershell
dotnet run --project samples/VideoPlayer -c Release
dotnet run --project samples/VideoPlayer -c Release -- --file="C:\Media\clip.mp4"
dotnet run --project samples/VideoPlayer -c Release -- --warp --test
```

The sample provides local file selection, seek, play/pause/restart, loop, volume/mute and transfer diagnostics. The default original H.264/AAC fixture contains a color/motion pattern, white flashes and synchronized 880 Hz pulses. `Assets/generate_fixture.py` records the reproducible FFmpeg source and updates SHA-256 hashes. FFmpeg is a development-only fixture-generation tool, not a runtime/package dependency.

The `--test` fixture has no audio stream so hosted Windows machines without audio endpoints can execute the real video decoder and playback clock. The bounded tests check BGRA colors/geometry, duration, pause, seek generation, video PTS/clock offset, resize, end-of-stream, looping, reopening, missing paths, repeated engine creation/disposal and post-close callback inactivity. They join source and isolated package consumption on .NET 10/11. Windows Server CI installs its Media Foundation feature if absent.

The displayed PTS-minus-clock offset is not a measurement of physical speaker latency or independent A/V drift. Audible synchronization, endpoint/device changes and actual hardware decode behavior need separate workstation review using the audio-bearing fixture. A silent CI pass does not qualify audible output. Media JIT playback is the first runtime target; NativeAOT media callbacks need a dedicated playback qualification before an AOT media claim. SDK COM ownership/AOT gates remain independent.

References: [Media Engine](https://learn.microsoft.com/windows/win32/api/mfmediaengine/nn-mfmediaengine-imfmediaengine), [frame transfer](https://learn.microsoft.com/windows/win32/api/mfmediaengine/nf-mfmediaengine-imfmediaengine-transfervideoframe), [frame-server ticks](https://learn.microsoft.com/windows/win32/api/mfmediaengine/nf-mfmediaengine-imfmediaengine-onvideostreamtick), [D3D12 video](https://learn.microsoft.com/windows/win32/medfound/direct3d-12-video-overview).
