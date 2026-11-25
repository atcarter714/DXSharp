# DXSharp

**A modern GPU workload orchestration SDK for C# and .NET.**

<img src="https://raw.githubusercontent.com/atcarter714/DXSharp/main/file/img/DXSharp_Logo_00_512.png" alt="DXSharp logo" width="192" align="right" />

DXSharp brings DirectX 12, GPU compute, DXGI, the DXC shader compiler, DirectML, animation, and media into a modular .NET SDK. It gives engine builders, graphics programmers, and tool developers direct access to the Windows GPU environment, with expressive managed APIs and explicit control over native resources.

It is also a **game and engine laboratory**: a place to build interactive worlds, explore computational techniques, and turn working experiments into reusable infrastructure. Rendering, simulation, animation, machine learning, and media share the same underlying questions of state, dependencies, ownership, and execution.

> Preserve the freedom of creation. Unify the machinery of execution.

[![NuGet preview](https://img.shields.io/badge/NuGet-0.4.0--preview.4-004880)](https://www.nuget.org/packages/DXSharp.Core/0.4.0-preview.4)
[![SDK qualification](https://github.com/atcarter714/DXSharp/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/atcarter714/DXSharp/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)

[Get started](#get-started) · [Samples](samples/README.md) · [Tutorials](doc/tutorials/README.md) · [Release notes](https://github.com/atcarter714/DXSharp/releases/tag/v0.4.0-preview.4) · [Support matrix](doc/Support-Matrix.md) · [Roadmap](doc/Competitive-Parity-Roadmap.md)

## See it in motion

[![Watch the DXSharp Fleet Battler playtest on YouTube][fleet-thumbnail]][fleet-playtest]

**[Watch the Fleet Battler playtest →][fleet-playtest]**

Fleet Battler is our evolving strategy-game laboratory: explore star systems, survey worlds, develop colonies, fit ships, and command fleets. It brings rendering, asset loading, simulation, compute, input, and audio together in a playable application. Tactical gameplay runs on the CPU; civilian traffic and asteroid workloads use GPU compute, with traffic instance buffers consumed directly by rendering.

The video captures a work in progress. Follow the [sample and controls](samples/FleetBattler/README.md) and [development plan](doc/FleetBattler.md) as the playtests and implementation evolve.

## Build with DXSharp

- **Graphics and compute:** D3D12 devices, command queues and lists, resources, descriptors, pipelines, indexed/instanced meshes, depth/stencil, swap chains, compute dispatch, and initial DXR workflows.
- **Explicit lifetimes:** generated COM interop, deterministic native ownership, recorded dependency retention, and fence-based retirement. Descriptor pools and upload/readback rings support reusable frame allocations.
- **Shaders:** runtime DXC compilation with diagnostics and include callbacks, owned bytecode, and incremental MSBuild compilation for embedded or deployed shader assets.
- **Content and animation:** static glTF/GLB scenes, a defined skinned-GLB import contract, animation interpolation and fades, material maps, and persistent GPU skinning.
- **Application hosts:** lightweight native Win32 windows, WinForms, optional WPF viewports, and optional WinUI 3 composition with XAML overlays.
- **Media and numerical workloads:** optional Media Foundation playback with D3D12 video-texture rendering, plus same-device DirectML Add/GEMM execution and numerical checks.

The public native layer exposes broad declarations from pinned SDK metadata. The managed layers add ownership and reusable workflows. [API coverage](doc/API-Coverage.md) and the [support matrix](doc/Support-Matrix.md) distinguish those declarations from implemented and runtime-qualified features.

## Get started

**Current release: `0.4.0-preview.4` · Windows x64 · .NET 10 and .NET 11.**

### Use the NuGet packages

In a Windows x64 application targeting `net10.0-windows10.0.26100.0` or `net11.0-windows10.0.26100.0`:

```powershell
dotnet add package DXSharp.Core --version 0.4.0-preview.4
```

The smallest device-creation example:

```csharp
using DXSharp.Graphics;

using var device = GraphicsDevice.Create( );
Console.WriteLine( device.AdapterName );
```

Select the framework, host, asset, media, or DirectML packages as needed. Core remains independently usable. Native dependencies are staged through NuGet build targets. See the [first application tutorial](doc/tutorials/01-get-started.md) for the complete project and [shader tooling](doc/Shader-Build-and-Hosting.md) for build-time DXC integration.

### Run the samples

Building the checkout requires the SDK pinned in [global.json](global.json). Samples default to .NET 10; install its runtime too, or select .NET 11 with `-p:DXSharpRuntimeMajor=11`.

```powershell
git clone https://github.com/atcarter714/DXSharp.git
cd DXSharp
dotnet run --project samples/FleetBattler -c Release
```

Try GPU skinning, media playback, or a bounded software-adapter smoke run:

```powershell
dotnet run --project samples/CharacterLab.Native -c Release
dotnet run --project samples/VideoPlayer -c Release
dotnet run --project samples/ModernTriangle -c Debug -- --warp --frames=4
dotnet run --project samples/ModernCompute -c Debug -- --warp --directml --gemm
```

`--warp` selects Microsoft's software adapter. Install Windows Graphics Tools when using the D3D12 debug layer. Hardware features are queried at runtime.

## Choose your layers

| Package | Role |
| --- | --- |
| [`DXSharp.Native`](https://www.nuget.org/packages/DXSharp.Native) | Public generated graphics, imaging, media, audio, input, DXC, and DirectML declarations |
| [`DXSharp.Core`](https://www.nuget.org/packages/DXSharp.Core) | Owned D3D12/DXGI resources, command recording, synchronization, allocation helpers, pipelines, and runtime DXC |
| [`DXSharp.Assets`](https://www.nuget.org/packages/DXSharp.Assets) | Device-independent model, animation, material, and texture data |
| [`DXSharp.Framework`](https://www.nuget.org/packages/DXSharp.Framework) | Game timing, animation, GPU skinning, shared frame rendering, and native Win32 hosting |
| [`DXSharp.Framework.Desktop`](https://www.nuget.org/packages/DXSharp.Framework.Desktop) | WinForms application hosting |
| [`DXSharp.Framework.Wpf`](https://www.nuget.org/packages/DXSharp.Framework.Wpf) | Optional WPF child-HWND viewport |
| [`DXSharp.Framework.WinUI`](https://www.nuget.org/packages/DXSharp.Framework.WinUI) | Optional WinUI 3 composition viewport and XAML overlays |
| [`DXSharp.Media`](https://www.nuget.org/packages/DXSharp.Media) | Optional Media Foundation playback and video-texture rendering |
| [`DXSharp.DirectML`](https://www.nuget.org/packages/DXSharp.DirectML) | Optional DirectML device/operator ownership and tensor execution |
| [`DXSharp.Tools.Builds`](https://www.nuget.org/packages/DXSharp.Tools.Builds) | Incremental DXC shader compilation and embedding/staging through MSBuild |

The original `DXSharp` and `DXSharp.Dxc` projects preserve the RCW-based compatibility API. New development uses the modern packages above. Preview 4 replaces the old experimental Framework surface; existing Framework consumers should read the [migration notes](doc/Framework-Vision.md).

See [SDK architecture](doc/SDK-Architecture.md) for dependency boundaries, native access, and ownership contracts. Optional UI and media dependencies stay in their own packages; the separately maintained Noesis integration has its own repository and licensing requirements.

## Samples as laboratories

| Workload | What to explore |
| --- | --- |
| [Fleet Battler](samples/FleetBattler/README.md) | Strategy gameplay, orbital navigation, colonies, ship fitting, and compute-to-draw traffic |
| [CharacterLab](samples/CharacterLab/README.md) | Persistent GPU skinning, animation, and materials; a shared scene across WinForms, native Win32, WPF, and WinUI hosts |
| [VideoPlayer](samples/VideoPlayer) | Local-file playback, seeking, looping, volume, and D3D12 presentation in WinUI |
| [N-Body Gravity](samples/NBodyGravity/README.md) | GPU-resident ping-pong simulation and instanced rendering |
| [ModernCompute](samples/ModernCompute/README.md) | Compute readback, DirectML tensor operations, and capability checks |
| [DXR Hello World](samples/DxrHelloWorld/README.md) | Acceleration structures, shader tables, ray dispatch, and image readback |
| [Rendering samples](samples/README.md) | Triangles, textures, indexed lighting, instancing, depth/stencil, planar shadows, and static models |

Every runnable sample uses project references in the checkout. Package qualification converts those same projects into isolated NuGet consumers, so source development and package usage exercise the same implementations.

## Where we are going

DXSharp's direction is a composable GPU execution foundation with a capable .NET game framework and toolchain above it. Applications should be able to combine rendering, simulation, animation, numerical work, and media while retaining control over resource placement and execution. The broader scheduling and tooling model is an ongoing engineering goal.

Author in Houdini, Blender, Maya, Cascadeur, or the tools that suit the project. Conventional assets and procedural workflows should meet through explicit interchange and content-processing contracts. The SDK should support different creative processes and engine architectures.

Upcoming work includes broader SDK/workflow coverage, DirectStorage, current GameInput, spatial audio, a visual ray-tracing scene, richer materials and content processing, and reusable lifecycle/input/audio services. Templates, inspection tools, profiling, and an integrated game-development workflow form the longer-term framework direction. Windows PC is the current platform; Xbox remains a future target requiring separate platform qualification.

Follow the [public roadmap](doc/Competitive-Parity-Roadmap.md), [framework direction](doc/Framework-Vision.md), and [showcase plan](doc/Sample-Showcase-Plan.md). The current release is a preview; these documents distinguish delivered work from planned capabilities.

## Build and validation

From an x64 Visual Studio Developer PowerShell with the pinned .NET SDK, C++ build tools, and Windows SDK installed:

```powershell
./build.ps1 -Configuration Release
./build/VerifySamples.ps1
```

Open `DXSharp.sln` for IDE development. See [build requirements](doc/NET10-Build-Baseline.md) and [release qualification](doc/SDK-Release-Qualification.md) for the complete environment and commands.

Release gates cover native COM ownership under JIT and NativeAOT, WARP graphics/numerical checks, shader builds, and isolated package consumption. Preview 4 also passed fresh NuGet.org-only verification on .NET 10 and 11 for all ten package IDs, the DXC build task, and all 17 sample consumers ([release evidence](https://github.com/atcarter714/DXSharp/actions/runs/45675637951)).

The graphics runtime targets Windows x64. Linux CI exercises portable COM/CPU fixtures. Optional WPF, WinUI, and media workloads have their own runtime qualification; the interop NativeAOT checks do not imply NativeAOT support for every application host. See the [support matrix](doc/Support-Matrix.md) for exact boundaries and recorded hardware acceptance.

## Origins and contributions

Created by **Aaron T. Carter / Arkaen Solutions**, DXSharp began with the ambition of bringing the feel of SlimDX and SharpDX to modern Direct3D 12 development in C#. It now grows through complete workloads, shared infrastructure, and reproducible checks.

Issues, focused pull requests, sample improvements, and hardware reports are welcome. Include a reproduction, commit/package version, adapter/driver, runtime, and relevant validation output when reporting a bug.

DXSharp is [MIT licensed](LICENSE.md). Third-party runtimes and sample assets retain their respective licenses.

<!-- Update both video references when a new Fleet Battler playtest is published. -->
[fleet-playtest]: https://www.youtube.com/watch?v=G3qKUh5N6QU
[fleet-thumbnail]: https://img.youtube.com/vi/G3qKUh5N6QU/hqdefault.jpg
