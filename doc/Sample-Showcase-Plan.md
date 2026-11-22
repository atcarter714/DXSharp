# Visual samples and optional UI hosts

The sample lineup must demonstrate observable SDK behavior as coverage grows. Console readback fixtures remain correctness tests; they do not replace interactive graphics, media or streaming examples. This plan adds no completed-coverage claims for APIs that are still on the backlog.

## Immediate visual polish

- PlanarShadows: lower the directional light, lengthen the silhouettes, enlarge the receiving plane and adjust the camera. Increase receiver/shadow contrast and light rotating pyramid normals in world space with the same direction used for projection. Keep the simple opaque planar projection explicit; this is not a shadow-map or soft-shadow implementation.
- LitCube: move lighting into the pixel shader, normalize interpolated normals, add a warm key with specular response, a restrained cool fill and hemispherical ambient light. Convert linear output to sRGB explicitly for the existing UNORM target. This is an analytic lighting example, not a PBR material renderer.
- Keep the existing bounded WARP runs and debug-layer checks for both samples. Visual acceptance additionally needs an interactive hardware review; a successful smoke run alone does not establish appearance or frame pacing.

## Optional host adapters

Keep Core and Framework independent of WPF and Windows App SDK dependencies. Each reusable control belongs in an optional assembly in this repository and the normal Libraries solution group. Sample applications are separate executable projects. No copy of the graphics bindings or second rendering backend is needed.

| Host | Proposed assembly / existing location | Presentation contract |
| --- | --- | --- |
| Native Win32 | Existing DXSharp.Framework | NativeGame and HWND FrameRenderer; no Windows Forms dependency |
| Windows Forms | Existing DXSharp.Framework.Desktop | Existing DesktopGame adapter |
| WPF | Proposed DXSharp.Framework.Wpf | HwndHost child HWND using the existing frame renderer; WPF dispatcher owns scheduling and teardown |
| WinUI 3 | Proposed DXSharp.Framework.WinUI | Composition swap chain attached through Microsoft.UI.Xaml ISwapChainPanelNative; Windows App SDK dependency stays in this adapter |

WPF's child HWND has airspace restrictions: WPF elements cannot freely overlap or transform it as ordinary composed content. D3DImage is a different path requiring a Direct3D9 surface bridge and synchronization; it is not a direct D3D12 swap-chain target. Start with an honest HwndHost sample and assess a composed surface bridge separately if editor overlays require it.

WinUI needs a composition presentation surface in Framework before the adapter can reuse frame-slot retirement. For D3D12, CreateSwapChainForComposition receives the direct command queue. Use the WinUI 3 interop interface from microsoft.ui.xaml.media.dxinterop.h, not the similarly named UWP interface. Attach/detach on the UI thread and handle composition scale, pixel size, unloading and outstanding GPU work explicitly.

Acceptance: share the CharacterLab scene/rendering code; exercise repeated resizing, DPI/scale changes, minimize/restore or unload/reload, input focus, close during GPU work and device-loss reporting. Confirm consumers only acquire their chosen UI framework. Pin and qualify the Windows App SDK version when implementing WinUI; the native and WPF hosts must not require its runtime. A WinUI sample can be unpackaged; an MSIX packaging project is a deployment choice, not a requirement imposed by DirectX hosting.

## Coverage-driven showcase work

| Work item | Interactive demonstration | Required evidence |
| --- | --- | --- |
| Video and audio | Local licensed test clip decoded into a displayed texture with synchronized audio, play/pause/seek, volume/mute, format and hardware-decoding telemetry | Known color/time patterns, seek/end-of-stream, A/V clock drift, resize and teardown. Identify Media Foundation versus D3D video decode/processing paths explicitly; playing through a control is not proof of D3D12 video binding coverage. |
| Spatial audio | XAudio2 voices with X3DAudio emitter/listener motion, distance attenuation and channel routing | Deterministic DSP coefficient checks plus audible hardware verification; expose channel layout and device changes. X3DAudio computes spatial DSP parameters and does not itself play audio. |
| DirectStorage | Stream a prepared asset pack into a live scene; show queue state, read/decompression timings, bytes and active path | Compare decoded content hashes against the source, exercise missing/corrupt input and shutdown with pending requests. Distinguish file I/O, decompression and GPU visibility; report actual supported paths rather than silently presenting fallback as GPU decompression. Keep ordinary loading as a comparison. |
| DXR visual scene | Windowed camera-controlled scene with meshes and ground, visible ray-traced shadows and reflections, and a moving instance | Reuse the acceleration-structure/shader-table groundwork from DxrHelloWorld; add output presentation, TLAS updates, resize, fenced resource lifetime and deterministic image checks. Query DXR support and explain an unsupported result clearly; do not relabel rasterization as ray tracing. |

Order: optional WPF HWND and WinUI composition host slice; video/audio and spatial audio alongside their native coverage; DirectStorage alongside its native coverage; then a focused DXR visual showcase. Retain the general texture/DDS, pipeline configuration and cross-queue synchronization backlog as supporting SDK work. Full applications must consume public SDK APIs and join the same source/package qualification mechanism as existing samples.

## Platform references

- [WPF and Win32 interoperation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/wpf-and-win32-interoperation)
- [WPF and Direct3D9 interoperation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/wpf-and-direct3d9-interoperation)
- [WinUI 3 ISwapChainPanelNative::SetSwapChain](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/win32/microsoft.ui.xaml.media.dxinterop/nf-microsoft-ui-xaml-media-dxinterop-iswapchainpanelnative-setswapchain)
- [CreateSwapChainForComposition](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition)
