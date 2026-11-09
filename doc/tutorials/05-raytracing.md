# Trace a triangle and verify the image

Run [DxrHelloWorld](../../samples/DxrHelloWorld) on a DXR-capable adapter.

```powershell
dotnet run --project samples/DxrHelloWorld -c Debug -- --require-dxr
```

The sample produces a 256 × 192 PPM image. The triangle uses barycentric color, and rays that miss use a fixed background. The sample checks the readback image against a CPU reference and allows a small tolerance at triangle edges and color quantization boundaries.

1. Use `SupportsRaytracing` to check the selected device. Compile the ray-generation, miss, and closest-hit exports with `ShaderCompiler.CompileLibrary`.
2. Build a BLAS from the triangle vertices and a TLAS with one instance. The builder sizes and owns the scratch allocations until the build fence completes.
3. Create the state object. Its global root signature uses an output UAV at `u0` and the TLAS at `t0`. The pipeline copies its three identifiers into aligned shader records.
4. Transition the output buffer from `Common` to `UnorderedAccess` and record `DispatchRays`. Transition to `CopySource`, copy to a readback buffer, submit, and wait on the fence.

The sample disposes its public scene, pipeline, and output owners after recording. The submitted work retains the native dependencies. A capability skip verifies shader compilation but leaves dispatch and image verification to a DXR-capable device. Use `--require-dxr` on a hardware validation machine so a skip fails the run.
