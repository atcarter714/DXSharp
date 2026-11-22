# DXSharp SDK samples

Open `DXSharp.sln` or run a project with `dotnet run --project samples/<name> -c Debug`. Windowed samples accept `--warp --frames=4` for bounded smoke runs. See [release qualification](../doc/SDK-Release-Qualification.md) for the qualification plan.

| Sample | What it exercises |
| --- | --- |
| [BasicSample](BasicSample) | Minimal triangle using the current desktop host, fences and DXC |
| [DepthStencil](DepthStencil) | Depth/stencil formats, masks, native interop, readback, and early owner disposal |
| [ModernTriangle](ModernTriangle) | Minimal graphics pipeline and resize qualification |
| [HelloTexture](HelloTexture) | Texture upload, SRV and sampler |
| [InstancedGrid](InstancedGrid) | 256 animated quads in one instanced draw |
| [LitCube](LitCube) | Indexed mesh upload, per-pixel diffuse/specular lighting, hemispherical ambient and D32 depth |
| [StaticModelViewer](StaticModelViewer) | glTF and GLB import, shared geometry, node transforms, and directional lighting |
| [FleetBattler](FleetBattler) | Fleet skirmishes, box selection, planetary development, and compute-driven civilian traffic |
| [PlanarShadows](PlanarShadows) | Low directional light, world-space face lighting and extended planar shadows |
| [ModernCompute](ModernCompute) | DXC compute readback, DirectML Add, and matrix inference with fused ReLU |
| [NBodyGravity](NBodyGravity) | Ping-pong GPU simulation, instanced drawing, and a seeded CPU reference |
| [DxrHelloWorld](DxrHelloWorld) | DXR acceleration structures, shader table, and image readback |

Every runnable sample project is included in DXSharp.sln and consumes the current SDK through project references. AdvancedSample remains reference source without a project file.

CharacterLab adds persistent GPU skinning, animation playback and material maps; see [CharacterLab](CharacterLab/README.md).

`CharacterLab.Native` runs the same GPU-skinned scene as CharacterLab through the native Win32 host, with precompiled Framework shaders and no Windows Forms reference. Both samples participate in source and package qualification.

See the [visual showcase and optional UI host plan](../doc/Sample-Showcase-Plan.md) for WPF/WinUI adapters and the upcoming media, spatial audio, DirectStorage and interactive DXR samples. These planned samples are not yet implemented.
