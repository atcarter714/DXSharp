# N-Body Gravity

A seeded pair of particle clusters evolves on the GPU. The windowed run combines a softened all-pairs integration step and instanced billboard rendering on a direct queue. It uses two desktop frame slots. A headless verification mode executes multiple steps on a compute queue and compares GPU readback against the CPU reference.

```powershell
dotnet run --project samples/NBodyGravity -c Debug
dotnet run --project samples/NBodyGravity -c Debug -- --warp --verify --bodies=65 --steps=5
dotnet run --project samples/NBodyGravity -c Debug -- --warp --bodies=129 --frames=6
```

The tutorial covers [dispatch and state transitions](../../doc/tutorials/03-compute.md). This sample is inspired by [Microsoft's N-Body Gravity sample](https://learn.microsoft.com/en-us/samples/microsoft/directx-graphics-samples/d3d12-n-body-gravity-sample-win32/). It uses a single queue for simulation and display. A separate async compute queue with GPU-side fence handoffs remains an extension.
