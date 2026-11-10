# Present a buffered frame

Run [ModernTriangle](../../samples/ModernTriangle) from the repository root.

```powershell
dotnet run --project samples/ModernTriangle -c Debug -- --warp --buffered --resize-test --frames=8
```

Expect a colored triangle and eight presentations, including one after each forced resize. The sample writes adapter and validation details to `artifacts/modern-triangle/diagnostics.json`.

The host runs on an STA thread and owns the window, swap chain, queue, and frame slots. The callback records the scene between the host's transitions to render target and back to present. A minimal draw callback uses a compiled pipeline and procedural vertices.

```csharp
var result = DesktopGame.Run(device, new()
{
    Title = "My renderer",
    FramesInFlight = 2,
    FrameLimit = 8
}, frame =>
{
    frame.Commands.SetGraphicsPipeline(pipeline);
    frame.Commands.Draw(3);
});
```

Keep `device` and `pipeline` alive for the host call. A `DesktopFrame` lends its command list and render-target view for the callback. In two-slot mode the host waits for a slot's fence before resetting its allocator. Resize and shutdown drain the queue before releasing the old back buffers.

[HelloTexture](../../samples/HelloTexture) extends the same loop with a texture upload and sampler. [LitCube](../../samples/LitCube) adds per-frame transforms and a depth buffer.

Next, [dispatch and validate a compute workload](03-compute.md).
