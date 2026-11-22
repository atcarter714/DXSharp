# DXSharp SDK samples

Open `DXSharp.sln` or run a project with `dotnet run --project samples/<name> -c Debug`. Windowed samples accept `--warp --frames=4` for bounded smoke runs. See [release qualification](../doc/SDK-Release-Qualification.md) for the qualification plan.

| Sample | What it exercises |
| --- | --- |
| [BasicSample](BasicSample) | Minimal triangle using the current desktop host, fences and DXC |
| [DepthStencil](DepthStencil) | Depth/stencil formats, masks, native interop, readback, and early owner disposal |
| [ModernTriangle](ModernTriangle) | Minimal graphics pipeline and resize qualification |
| [HelloTexture](HelloTexture) | Texture upload, SRV and sampler |
| [InstancedGrid](InstancedGrid) | 256 animated quads in one instanced draw |
| [LitCube](LitCube) | Indexed mesh upload, vertex input layouts, lighting and D32 depth |
| [StaticModelViewer](StaticModelViewer) | glTF and GLB import, shared geometry, node transforms, and directional lighting |
| [FleetBattler](FleetBattler) | Fleet skirmishes, box selection, planetary development, and compute-driven civilian traffic |
| [PlanarShadows](PlanarShadows) | Orbiting camera, multiple pipelines and projected shadows |
| [ModernCompute](ModernCompute) | DXC compute readback, DirectML Add, and matrix inference with fused ReLU |
| [NBodyGravity](NBodyGravity) | Ping-pong GPU simulation, instanced drawing, and a seeded CPU reference |
| [DxrHelloWorld](DxrHelloWorld) | DXR acceleration structures, shader table, and image readback |

Every runnable sample project is included in DXSharp.sln and consumes the current SDK through project references. AdvancedSample remains reference source without a project file.

CharacterLab adds persistent GPU skinning, animation playback and material maps; see [CharacterLab](CharacterLab/README.md).
