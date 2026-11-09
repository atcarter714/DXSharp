# DXSharp SDK samples

Open `DXSharp.sln` or run a project with `dotnet run --project samples/<name> -c Debug`. Windowed samples accept `--warp --frames=4` for bounded smoke runs. See [release qualification](../doc/SDK-Release-Qualification.md) for the qualification plan.

| Sample | What it exercises |
| --- | --- |
| [ModernTriangle](ModernTriangle) | Minimal graphics pipeline and resize qualification |
| [HelloTexture](HelloTexture) | Texture upload, SRV and sampler |
| [InstancedGrid](InstancedGrid) | 256 animated quads in one instanced draw |
| [LitCube](LitCube) | Transforms, lighting and a D32 depth buffer |
| [PlanarShadows](PlanarShadows) | Orbiting camera, multiple pipelines and projected shadows |
| [ModernCompute](ModernCompute) | DXC compute, GPU readback and optional DirectML |
| [NBodyGravity](NBodyGravity) | Ping-pong GPU simulation, instanced drawing, and a seeded CPU reference |

[BasicSample](BasicSample) remains a standalone legacy crash reproduction outside the sample solution. The original [AdvancedSample](AdvancedSample) experiments are available as reference source.
