# DXSharp

**Direct3D 12, GPU compute, and DirectML for C# and .NET 10.**

DXSharp exposes D3D12 devices, queues, resources, pipelines, and swap chains through typed .NET APIs. Use it to build a renderer, an engine, a graphics tool, or a GPU compute application with explicit control over command recording and resource lifetimes.

![DXSharp logo](file/img/DXSharp_Logo_00_512.png)

[Samples](samples/README.md) · [Tutorials](doc/tutorials/README.md) · [SDK qualification](doc/SDK-Release-Qualification.md) · [MIT license](LICENSE.md)

## Start here

The current SDK is a preview for Windows x64. The repository pins the .NET 10 SDK in [global.json](global.json) and includes the Agility SDK, DXC, and optional DirectML runtime through package dependencies. Use Windows Graphics Tools for debug-layer validation. The `--warp` examples use Microsoft's software adapter.

```powershell
git clone https://github.com/atcarter714/DXSharp.git
cd DXSharp
dotnet run --project samples/ModernTriangle -c Debug -- --warp --frames=4
dotnet run --project samples/ModernCompute -c Debug -- --warp --directml --gemm
```

The first command presents four frames and exits. The second verifies GPU compute readback and DirectML tensor addition and matrix inference. For a clean package consumer, start with [Create your first DXSharp application](doc/tutorials/01-get-started.md).

## Architecture

| Package | Responsibility |
| --- | --- |
| `DXSharp.Core` | D3D12, DXGI, DXC, and generated COM interop |
| `DXSharp.Framework.Desktop` | WinForms presentation loop, swap-chain resize, and frame-slot retirement |
| `DXSharp.DirectML` | Device and operator ownership, tensor binding, and DirectML execution |
| `DXSharp`, `DXSharp.Framework`, `DXSharp.Dxc` | Compatibility packages for the original RCW-based SDK |

The modern API uses CsWin32 declarations and .NET source-generated COM projections. Managed owners release their native references deterministically. A command list takes its own references to the objects it records, and the queue holds them through fence completion. This lets applications give resources explicit lifetimes even when work is still in flight.

DXC returns owned compilation output and diagnostics. The desktop host supplies a small presentation loop while applications retain control of render and compute commands. The [interop design](doc/Generated-Com-Foundation.md) explains the ownership model, the native runtime selection, and the qualification matrix.

## Build and qualification

From an x64 Visual Studio Developer PowerShell with the C++ build tools and Windows SDK installed, run:

```powershell
./build.ps1 -Configuration Release
./build/VerifySamples.ps1
```

The build uses locked dependencies, runs the Windows test gate and a native COM fixture, then packs and validates a fresh consumer. Windows CI also runs WARP graphics and numerical workloads. The native interop fixture runs with JIT and NativeAOT on Windows and Linux x64. The [release checklist](doc/SDK-Release-Qualification.md) tracks hardware validation and remaining API coverage. See [Azure Pipelines](devops/readme.md) for the CI definition and artifact retention.

## History

Aaron Carter started DXSharp to prove that C# could support a full Direct3D 12 workflow with the feel of SlimDX and SharpDX. The original project brought up a broad set of DirectX bindings, examples, and build tools. Its hand-maintained COM wrappers also showed where ownership and interface projection needed to be simpler. The .NET 10 SDK builds on that experience with generated interop, explicit lifetimes, and repeatable GPU checks.

For contributions, open an issue or a pull request with a small reproduction and the adapter, runtime, and validation details. DXSharp is [MIT licensed](LICENSE.md).
