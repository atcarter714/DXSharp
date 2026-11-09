# DXSharp AdvancedSample

AdvancedSample draws a lit, rotating cube using the desktop framework and the generated COM graphics API. It uploads world and world-view-projection matrices as root constants, uses a D32 depth buffer, and updates the projection and depth target when the window is resized.

```powershell
dotnet run --project samples/AdvancedSample -c Debug
dotnet run --project samples/AdvancedSample -c Debug -- --warp --frames=4
```

The earlier scene, camera and shadow-map experiments under `Framework/` and `Assets/` remain as reference source. The runnable project compiles the modern entry point. [Microsoft's D3D12 Multithreading sample](https://github.com/microsoft/DirectX-Graphics-Samples/tree/master/Samples/Desktop/D3D12Multithreading) is a reference for a later port with shadow passes and parallel command recording.
