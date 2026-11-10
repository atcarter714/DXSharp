# DXSharp Fleet Battler

The flagship sample starts with one star system, two factions, and a playable fleet skirmish. Scout a resource site, claim it, spend its output on reinforcements, then send ships to intercept a rival fleet. Use a fixed tick and seeded scenario so the same command stream can drive a replay, a headless test, or the visual sample.

| Slice | Implementation | Acceptance |
| --- | --- | --- |
| Simulation and replay | Stable entity IDs and typed SoA storage for position, velocity, faction, and health. Keep orders, resource accounting, and combat on the CPU. Use contiguous spans for steering and a spatial grid for neighbor queries. | Replay the same commands to the same tick hash on the same build and architecture. Compare SIMD and scalar kernels within documented tolerances. |
| Renderer and workload | Upload changed instance ranges through frame-local staging. Draw instanced ships. Use GPU culling, indirect draws, and compute-driven thruster and impact particles as the workload grows. Fence uploads and descriptor reuse. | Check representative output, resize and teardown. Measure steady-state allocations, CPU simulation and submission times, GPU timestamps, and peak working set at fixed fleet sizes. |
| Asset and scene pipeline | Cook a Houdini-exported ship kit and textures into the MSBuild asset manifest. Add one rigged hangar character with a Cascadeur-authored animation as a follow-on consumer of the skinned-model sample. | A clean build, an incremental asset edit, and a packaged build load the correct assets with their license and provenance recorded. |
| Scale and optional effects | Add a tactical map and multiple systems after the skirmish is playable. Add DXR and newer Shader Model variants to isolated effects where their measured cost or visual benefit justifies them. | Compare equivalent workloads at fixed counts, resolution, settings, and driver. Keep a WARP path for functional tests and collect performance on hardware. |

## Shader capability policy

- **Retail.** Qualify Shader Model 6.9 with the bundled DXC. Samples can keep a lower baseline and opt into newer profiles per pass. Query the adapter and runtime through `GraphicsDevice.GetHighestShaderModel()` and check the feature tier a shader needs separately.
- **Preview.** Shader Model 6.10 is a developer preview. Test it with the matching preview DXC, Agility SDK, and vendor driver in a separate qualification run. Keep the public preview packages on the qualified retail native dependencies.
- **Dependency updates.** Review the [Agility SDK releases](https://devblogs.microsoft.com/directx/directx12agility/) and [DXC releases](https://github.com/microsoft/DirectXShaderCompiler/releases) together. Update the native pins, lock files, and native asset verification in one change. Run WARP, NativeAOT, and target-hardware checks for the affected shaders before promoting the update. Record the compiler, Agility runtime, adapter, driver, and shader profile in the evidence. [Microsoft's Shader Model 6.10 announcement](https://devblogs.microsoft.com/directx/shader-model-6-10-agilitysdk-720-preview/)

Use `dotnet run --project samples/ModernCompute -c Release -- --latest-shader-model` on hardware. Add `--warp` for WARP or `--system-runtime` to test the OS runtime. The probe reports the device ceiling and dispatch profile, compiles a Shader Model 6.9 shader, and validates the selected profile with compute readback.
