# WinUI 3 composition hosting

`DXSharp.Framework.WinUI` is an optional Preview 4 adapter. Its `D3D12Viewport` is a WinUI `UserControl` containing a `SwapChainPanel`; it uses Framework's existing frame slots, depth targets, fences and resize handling. Core and Framework do not reference Windows App SDK. WPF and native Win32 consumers do not acquire it transitively.

The adapter pins the stable `Microsoft.WindowsAppSDK.WinUI` component to 2.7.0. It uses the WinUI 3 `ISwapChainPanelNative` IID from that package's `microsoft.ui.xaml.media.dxinterop.h`, distinct from the similarly named UWP interface. The composition factory receives the D3D12 direct command queue and uses flip-sequential/STRETCH presentation. Factory outputs are queried for the required swap-chain interface, with independent owning references for renderer and panel attachment.

## Using the control

Reference `DXSharp.Framework.WinUI` and enable `UseWinUI` in a Windows x64 application. Add the control to your XAML layout or construct it in code:

```csharp
var viewport = new DXSharp.Framework.WinUI.D3D12Viewport( );
sceneGrid.Children.Add( viewport );
viewport.Start( device, new WindowRenderOptions {
    EnableDepth = true, FramesInFlight = 2
}, frame => scene.Draw( frame ), owner: window );
```

The device and scene remain caller-owned. `Start` may precede loading. Calls, rendering callbacks, panel attachment and disposal belong to the creating UI thread, with the normal WinUI dispatcher synchronization context. Pass the owning window to pause presentation while minimized; use `IsSuspended` for other application visibility policies. Loading schedules frames through `CompositionTarget.Rendering`. Unloading drains and detaches presentation resources; reloading resumes the active session.

Panel layout is in XAML units. Back buffers use `ceil(ActualWidth * CompositionScaleX)` and the corresponding height. `RenderFrame.Size` and `PixelSize` are physical pixels. The swap-chain matrix applies the inverse composition scale, including after resizing, so scaling is applied once. `Panel` exposes the underlying panel for layout/transform configuration; callers must not replace its swap chain. Ordinary XAML overlays can appear above it, unlike the WPF child-HWND adapter. Overlays that should pass pointer input through must set `IsHitTestVisible=false`.

Clicking the viewport focuses and captures it. `HasInputFocus` and `AcceptsPointerInput` let shared input code avoid consuming text-editor actions; `WheelScrolled` reports wheel input. Tab navigation enters and leaves the viewport. Logical XAML focus alone does not authorize keyboard polling while the owning window is inactive. Applications own action mapping and text/IME behavior; this host does not replace GameInput.

Await `StopAsync` before disposing the viewport, scene resources or device. It detaches scheduling, drains outstanding queue work, calls `SetSwapChain(null)` and releases both native interface leases. Cancel the first window close, await stopping, dispose the viewport, then close the window. `CharacterWindow.CloseOrderly` implements this sequence. Use `FramePresented` for post-recording shutdown; do not stop or dispose inside the draw callback. `RenderFailed` reports stopped/drained rendering and `Failure` retains the exception. A new session can start after the stop completes. Device recreation remains the caller's responsibility.

## Sample and qualification

Run `dotnet run --project samples/CharacterLab.WinUI -c Release`. The sample uses an unpackaged x64 apphost and bundles the Windows App SDK runtime with `WindowsAppSDKSelfContained=true`; it still requires the selected .NET runtime. An MSIX packaging project is unnecessary. Launch the generated executable when deploying; invoking its DLL with `dotnet` bypasses the apphost's embedded activation manifest.

The code-created application explicitly provides WinUI XAML type metadata and initializes control resources before creating its window. Normal markup-based WinUI applications receive that metadata from the XAML compiler.

The same CharacterScene and OrbitCamera sources serve WinForms, native Win32, WPF and WinUI. WinUI playback controls, a text editor and an overlaid XAML badge demonstrate composition without a second renderer. Shaders remain precompiled, and runtime DXC binaries are excluded.

`--warp --frames=9` bounds presentation. `--warp --host-test` exercises physical resize, nonuniform XAML scale, focus, tab navigation and window activation isolation, suspension, minimize/restore, unload/reload, intentional render failure, restart and close from a presentation callback. Source and isolated package consumers join Windows CI on .NET 10 and 11. The qualification target is Windows x64 JIT; the separate native COM NativeAOT fixture does not establish WinUI NativeAOT support. Hardware pacing and monitor-to-monitor DPI transitions need interactive hardware review.

References: [composition swap-chain creation](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition), [WinUI native panel attachment](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/win32/microsoft.ui.xaml.media.dxinterop/nf-microsoft-ui-xaml-media-dxinterop-iswapchainpanelnative-setswapchain), [SwapChainPanel](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.swapchainpanel), and [self-contained Windows App SDK deployment](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/self-contained-deploy/deploy-self-contained-apps).
