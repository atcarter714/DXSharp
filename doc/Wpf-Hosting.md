# WPF hosting

`DXSharp.Framework.Wpf` is an optional Preview 4 assembly containing `D3D12Viewport`, a WPF `HwndHost` over the existing Framework `FrameRenderer`. It references Framework/Core/Assets and the WPF desktop framework. Core, Framework and the native Win32 host do not acquire WPF, Windows Forms or Windows App SDK dependencies through it. Windows x64 JIT on .NET 10 and 11 is the qualification target for this adapter; the SDK's separate NativeAOT fixture does not establish WPF/BAML NativeAOT support.

## Using the control

Reference the WPF package (or its project when developing in this repository), enable `UseWPF`, and place the control in a WPF layout:

```xml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:dx="clr-namespace:DXSharp.Framework.Wpf;assembly=DXSharp.Framework.Wpf">
  <Grid><dx:D3D12Viewport x:Name="Viewport" /></Grid>
</Window>
```

Initialize it on the dispatcher with a caller-owned device and scene:

```csharp
Viewport.Start( device, new WindowRenderOptions {
    EnableDepth = true, FramesInFlight = 2
}, frame => scene.Draw( frame ) );
```

`Start` may precede `Loaded`. `Rendering` scheduling uses WPF's composition callback; the control invokes the supplied draw callback once per composition timestamp and presents through the shared fenced frame slots. `RenderFrame.Size` and `PixelSize` are physical child-window pixels read using `GetClientRect`. Do not multiply them by the WPF DPI scale again. Arrange the control in device-independent WPF units normally. Resize rebuilds the shared swap-chain/depth resources after prior work retires. Minimized, invisible and unloaded controls stop presenting; restoration resets the elapsed-time boundary. An unloaded session can resume when the control is loaded again.

`FramePresented` runs after recording/presentation has returned. Use it for statistics and bounded shutdown. `RenderFailed` reports a stopped and drained session; `Failure` retains the error. Restart requires a completed stop. Call `StopAsync` outside the draw callback, then dispose the control before disposing scene resources and the device. In a WPF `Closing` handler, cancel the first close, await `StopAsync`, dispose the viewport, and issue the final close. `CharacterWindow.xaml.cs` implements that complete sequence. Forced disposal and native-window destruction also retire the renderer while the child HWND remains alive.

`HasInputFocus` distinguishes the child HWND from WPF text editors. `AcceptsPointerInput` restricts pointer polling to the focused viewport or its captured drag. Clicking the viewport acquires focus and pointer capture; release and focus loss relinquish capture. Tab navigation enters and leaves the native region. Applications remain responsible for their action mapping and text/IME policy; this control is not a replacement for GameInput.

## CharacterLab.Wpf and qualification

Run `dotnet run --project samples/CharacterLab.Wpf`; use `--warp --frames=9` for bounded presentation or `--warp --host-test` for host qualification. WPF controls select clips, pause playback, change speed, reset the camera and demonstrate independent text input. The same `CharacterScene` and camera sources serve WinForms, native Win32 and WPF. Skinning shaders remain precompiled.

The Windows gate runs resize and current-DPI pixel-size checks, focus transfer and tab entry/exit, minimize/restore, visibility suspension, unload/reload, intentional render failure, restart, close from a presentation callback and native-handle teardown. Both runtime versions also build and execute the same sample from isolated fresh packages in Debug/Release. Physical monitor-to-monitor DPI transitions and hardware frame pacing require separate interactive hardware review; these checks do not simulate a second monitor. The standalone sample rejects a loaded Windows Forms assembly and deployed runtime DXC binaries.

WPF HWND airspace restrictions apply: arbitrary WPF overlays, opacity and transforms over the native viewport are not supported. Adjacent WPF panels and separate popup windows are suitable. A `D3DImage` composed-surface bridge would be a separate Direct3D9 interop design, not this D3D12 swap-chain host. The separate [WinUI composition adapter](WinUI-Hosting.md) supports ordinary XAML overlays without this HWND airspace limitation.

References: [WPF/Win32 interoperation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/wpf-and-win32-interoperation), [hosting Win32 content](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/hosting-win32-content-in-wpf), and [composition rendering](https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.compositiontarget.rendering).
