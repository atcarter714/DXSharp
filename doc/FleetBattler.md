# DXSharp Fleet Battler

The flagship sample starts with one star system, two factions, and a playable fleet skirmish. Scout a resource site, claim it, spend its output on reinforcements, then send ships to intercept a rival fleet. Use a fixed tick and seeded scenario so the same command stream can drive a replay, a headless test, or the visual sample.

The [Fleet Battler sample](../samples/FleetBattler) implements the first steering and rendering slice. The default scenario packs 1,024 ships into a single instance stream and draws the patrol mesh in three indexed instanced calls. It supports fleet selection, movement orders, camera zoom, a CPU benchmark, and a bounded WARP resize run.

| Slice | Implementation | Acceptance |
| --- | --- | --- |
| Simulation and replay | Stable entity IDs and typed SoA storage for position, velocity, faction, and health. Keep orders, resource accounting, and combat on the CPU. Use contiguous spans for steering and a spatial grid for neighbor queries. | Replay the same commands to the same tick hash on the same build and architecture. Compare SIMD and scalar kernels within documented tolerances. |
| Renderer and workload | Upload changed instance ranges through frame-local staging. Draw instanced ships. Use GPU culling, indirect draws, and compute-driven thruster and impact particles as the workload grows. Fence uploads and descriptor reuse. | Check representative output, resize and teardown. Measure steady-state allocations, CPU simulation and submission times, GPU timestamps, and peak working set at fixed fleet sizes. |
| Asset and scene pipeline | Cook a Houdini-exported ship kit and textures into the MSBuild asset manifest. Add one rigged hangar character with a Cascadeur-authored animation as a follow-on consumer of the skinned-model sample. | A clean build, an incremental asset edit, and a packaged build load the correct assets with their license and provenance recorded. |
| Scale and optional effects | Add a tactical map and multiple systems after the skirmish is playable. Add DXR and newer Shader Model variants to isolated effects where their measured cost or visual benefit justifies them. | Compare equivalent workloads at fixed counts, resolution, settings, and driver. Keep a WARP path for functional tests and collect performance on hardware. |

## Kepler 2.0 data path

Bake a small system catalog in Houdini and keep the source and frame explicit. The orbital-element input retains `a_AU`, `e`, `i_deg`, `Omega_deg`, `w_deg`, `M_deg`, `epoch_jd`, and `P_days`. Record the origin body, time scale, reference frame, and units. Convert axes and handedness once at the import boundary and verify the mapping with a tilted prograde orbit. Keep Horizons elements, Horizons or SPICE state samples, and seeded fictional elements as separate source kinds. Pin the vector query or kernel identity and validity interval in fixture metadata. [JPL ephemerides](https://ssd.jpl.nasa.gov/ephem.html) · [SPICE time systems](https://naif.jpl.nasa.gov/pub/naif/toolkit_docs/C/req/time.html)

For an elliptical orbit, advance time from the epoch, compute mean anomaly, solve Kepler's equation, and convert to true anomaly. Carter's normalized phase is `u = wrap01(ν / τ)`, where `τ = 2π`. Recover the position from the phase and stored orbit geometry. True anomaly advances at a variable rate. Treat phase as a reconstruction coordinate and use time as the simulation input. Direct state samples use a separate bounded interpolation path. Compare the GPU orbit evaluator against a double-precision CPU reference for wraparound, high eccentricity, inclined planes, and parent-relative moon motion. Compare real-system fixtures with source vectors within documented time spans and tolerances.

Keep CPU authority for commands, economy, combat, and replay. Use a GPU evaluator to reconstruct visible celestial transforms and a sector-relative origin for fleet presentation. Rebase in high precision before packing float positions for the GPU. Chunk the CPU SoA by work ranges, give each worker exclusive writes, and commit commands and reductions in stable order. Measure arrays plus spans and SIMD first. Use aligned unmanaged slabs and blittable GPU records where the upload, pinning, or allocation measurements justify them.

## Shared GPU effects

Use the buffer-driven VFX Graph pattern for one persistent emitter pool per effect family. Stream compact burst records for impacts and destruction, and derive continuous thruster emitters from visible ship instances and authored sockets. Keep particle state GPU-resident. Run bounded spawn, update, and compact passes, then use indirect draws to consume the visible counts. Define capacity, overflow policy, and frame-slot lifetime up front. When saturated, preserve priority for gameplay-readable effects and count dropped cosmetic spawns. [Unity VFX GraphicsBuffer binding](https://docs.unity.com/engine/6000.7/script-reference/unityengine/vfx/visualeffect/setgraphicsbuffer)

Start on a single graphics queue with batched submissions. Synchronize where writes are consumed by later UAV passes, shader reads, or indirect argument reads. Use the narrow required resource transitions and avoid per-emitter barriers and CPU readbacks. Try a compute queue only when GPU timestamps show overlap worth the queue handoff, and fence cross-queue dependencies and slot reuse. Record CPU simulation and submission time, GPU pass timings, upload bytes, barrier count, and pool occupancy at 1k, 10k, and 100k synthetic entities. These counts define benchmarks. Establish performance budgets from measured results. [Direct3D 12 enhanced barriers](https://learn.microsoft.com/windows-hardware/drivers/display/enhanced-barriers)

## Asset toolkit

Make FBX and glTF/GLB first-class inputs to a separate asset cooker shipped with the toolkit. Use a pinned importer behind an offline boundary, with [ufbx](https://ufbx.github.io/) as the FBX candidate. Feed both formats into the same mesh, material, skeleton, and clip representation. Keep raw file loading available for tools and development. Release builds load a versioned, validated binary pack with GPU-ready streams, packed indices, compressed textures, bounds, sockets, and baked animation clips. Keep import dependencies out of the graphics runtime package. Support DDS for GPU textures, common image sources such as PNG/JPEG, and static OBJ as a compatibility input. USD can remain a Houdini scene interchange path while the runtime consumes cooked assets.

Qualify FBX with an actual Houdini or Cascadeur export and the same rig exported as GLB. Check coordinate and unit conversion, winding, pivots, skin weights, inverse bind transforms, named clips, clip duration, and pose parity at selected frames. Record exporter version and import flags alongside each fixture. Use bulk transfers over the native boundary and profile mesh upload, clip sampling, and skinning independently. [Houdini FBX character export](https://www.sidefx.com/docs/houdini/nodes/sop/kinefx--rop_fbxcharacteroutput.html) · [Houdini glTF character export](https://www.sidefx.com/docs/houdini/nodes/sop/kinefx--rop_gltfcharacteroutput.html)

## Houdini art list

| Priority | Asset | First delivery |
| --- | --- | --- |
| First playable | Scout, interceptor, mining hauler, and frigate | Four distinct silhouettes from a modular hull kit. Two faction material variants, three LODs per hull, a collision proxy, and named engine, weapon, and docking sockets. |
| First playable | Refinery and mineable asteroid field | One refinery with a docking ring and several seeded asteroid variants. Export separate placement, bounds, and render data so instancing and culling can be tested independently. |
| First playable | Kepler system | One star, a rocky world, a gas giant, and a moon. Deliver catalog and orbit debug geometry alongside scalable body meshes, masks, and atmosphere parameters. |
| First playable | Thrusters, weapon streaks, shield hits, hull impacts, and destruction | Reusable atlases, mesh debris, curves, and emitter presets. Use socket attributes and shared pools so a ship variant adds content without adding an emitter instance. |
| Next asset milestone | Carrier, capital ship, and hangar crew | Carrier and capital variants stress larger hull LODs and socket counts. Export one skinned crew member with idle, locomotion, and a docking gesture in FBX and GLB. |
| Next effect milestone | Warp entry, mining beam, and refinery activity | Author effect parameters and distance LODs. Use these to qualify sustained emitters and scene-scale visibility budgets. |

## Shader capability policy

- **Retail.** Qualify Shader Model 6.9 with the bundled DXC. Samples can keep a lower baseline and opt into newer profiles per pass. Query the adapter and runtime through `GraphicsDevice.GetHighestShaderModel()` and check the feature tier a shader needs separately.
- **Preview.** Shader Model 6.10 is a developer preview. Test it with the matching preview DXC, Agility SDK, and vendor driver in a separate qualification run. Keep the public preview packages on the qualified retail native dependencies.
- **Dependency updates.** Review the [Agility SDK releases](https://devblogs.microsoft.com/directx/directx12agility/) and [DXC releases](https://github.com/microsoft/DirectXShaderCompiler/releases) together. Update the native pins, lock files, and native asset verification in one change. Run WARP, NativeAOT, and target-hardware checks for the affected shaders before promoting the update. Record the compiler, Agility runtime, adapter, driver, and shader profile in the evidence. [Microsoft's Shader Model 6.10 announcement](https://devblogs.microsoft.com/directx/shader-model-6-10-agilitysdk-720-preview/)

Use `dotnet run --project samples/ModernCompute -c Release -- --latest-shader-model` on hardware. Add `--warp` for WARP or `--system-runtime` to test the OS runtime. The probe reports the device ceiling and dispatch profile, compiles a Shader Model 6.9 shader, and validates the selected profile with compute readback.
