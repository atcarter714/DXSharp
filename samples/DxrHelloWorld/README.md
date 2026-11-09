# DXR Hello World

Compile a DXIL library, build a BLAS and TLAS for a triangle, and dispatch ray-generation, miss, and closest-hit shaders. The sample writes an image after comparing the GPU result with a CPU barycentric reference. It checks both hit and miss pixels.

```powershell
dotnet run --project samples/DxrHelloWorld -c Debug -- --require-dxr
dotnet run --project samples/DxrHelloWorld -c Debug -- --warp
```

The default output is `artifacts/dxr-hello-world.ppm`. Pass `--output=path.ppm` to change it. The sample always compiles the shader library. A runtime without DXR reports `SKIP`, and `--require-dxr` turns that into a failure. The [tutorial](../../doc/tutorials/05-raytracing.md) explains the resource states, shader table, and readback.

Inspired by [Microsoft's Direct3D 12 Raytracing Hello World](https://learn.microsoft.com/en-us/samples/microsoft/directx-graphics-samples/d3d12-raytracing-samples-win32/).
