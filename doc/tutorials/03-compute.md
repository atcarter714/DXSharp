# Dispatch compute and verify the results

[ModernCompute](../../samples/ModernCompute) compiles a shader with DXC, dispatches it, copies a UAV into readback memory, and checks for the value `1234`.

```powershell
dotnet run --project samples/ModernCompute -c Debug -- --warp
```

The root signature and HLSL register assignments must agree. The sample binds the output to `u0`. It closes and submits the command list and waits for the returned fence before reading the result. A GPU resource used by a recording stays retained even if the application disposes its public owner before submission.

[NBodyGravity](../../samples/NBodyGravity) uses two structured buffers, with each dispatch reading the previous state and writing the next. The render path transitions the output to a vertex shader resource and draws one billboard per body. Compute and drawing share a direct queue in the windowed sample. The headless run uses a compute queue and compares the GPU result with a seeded CPU integrator.

```powershell
dotnet run --project samples/NBodyGravity -c Debug -- --warp --verify --bodies=65 --steps=5
dotnet run --project samples/NBodyGravity -c Debug -- --bodies=1024
```

The first command checks all positions and velocities. The count of 65 exercises a partial 64-thread group. Each iteration swaps the input and output only after recording the dispatch and its transitions. Both buffers start in `Common`. Barriers order the UAV accesses and establish the next use before graphics or readback.

Next, [execute a DirectML operator](04-directml.md).
