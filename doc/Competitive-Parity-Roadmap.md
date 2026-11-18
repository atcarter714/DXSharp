# Competitive parity roadmap

Updated 18 November 2026. Execution baseline: preview 2 integration merged through [#31](https://github.com/atcarter714/DXSharp/pull/31), commit `3413542d3e7e69d13b7c78502e527602b43313ff`.

## Product target

Build a production-qualified Windows x64 DirectX SDK for .NET 10 and .NET 11, with native API coverage, useful managed facilities, reproducible distribution and measured behavior competitive with maintained alternatives.

The first comparison is Vortice's relevant Windows game-development surface. Compare native-shaped interop against TerraFX, and D3D12 workflows against Silk.NET. SharpDX is a historical compatibility reference. Silk's Vulkan/OpenGL/OpenXR ecosystem is a separate portability expansion; implementing every backend is not an exit condition for the Windows SDK milestone.

Keep three explicit support columns for every feature: **native declaration**, **managed workflow**, and **runtime-qualified workload**. Version pins, adapter requirements and architecture apply independently to each.

## Release baseline and measured gaps

Preview 2 remains the .NET 10 Windows x64 candidate already qualified before consolidation. Its release workflow requalifies the resulting main commit before creating GitHub assets and attempting NuGet publication. A GitHub prerelease and accepted NuGet packages are separate outcomes; record both.

The audit compared the built public Native assembly with Microsoft's official retail Agility 1.620.2 WinMD: **645 of 752 top-level D3D12 type names match; 107 are missing**. This is a declaration inventory, not a functionality percentage or ABI proof. The current generator input is Win32Metadata 71.0.37-preview, while the packaged runtime is Agility 1.620.1.

Actual missing binding families include DirectStorage, Media Foundation/D3D12 video, and X3DAudio. Current GameInput needs a separately pinned versioned API source. Core's general textures, descriptors, allocation, queue orchestration and presentation conveniences need expansion even where native declarations already exist.

References: [Microsoft Agility releases](https://devblogs.microsoft.com/directx/directx12agility/), [Vortice](https://github.com/amerkoleci/Vortice.Windows), [Silk.NET](https://github.com/dotnet/Silk.NET), [TerraFX](https://github.com/terrafx/terrafx.interop.windows), [GameInput](https://www.nuget.org/packages/Microsoft.GameInput). See [current coverage](API-Coverage.md), [architecture](SDK-Architecture.md) and [release qualification](SDK-Release-Qualification.md).

## Iteration plan

Use two-week planning timeboxes. Each tracking issue is a prioritized workstream with a bounded first slice and explicit continuation work. Re-estimate after Sprint 1; the four headings do not promise full API/toolkit/platform parity in eight weeks. Favor one complete, verified consumer path over several unqualified declarations.

| Order | Tracking | First slice | Completion evidence |
| --- | --- | --- | --- |
| 1 | [Runtime and native foundation #32](https://github.com/atcarter714/DXSharp/issues/32) | .NET 10/11 package/runtime qualification; current retail D3D12 metadata | Both runtime matrices and external consumers pass; current declaration delta accounted for |
| 2 | [Rendering infrastructure #33](https://github.com/atcarter714/DXSharp/issues/33) | Fence-retired upload/readback and descriptor allocation; one consumer migration | Stable multi-frame reclamation, readback/pixel results and bounded allocation behavior |
| 3 | [Windows game APIs #34](https://github.com/atcarter714/DXSharp/issues/34) | DirectStorage and current GameInput | Deterministic I/O data and completion checks; input/hotplug/focus/callback teardown qualification |
| 4 | [Production and performance #35](https://github.com/atcarter714/DXSharp/issues/35) | Regression/diagnostic workload and equivalent benchmark harness | Reproducible crash/lifetime evidence and published comparable measurements |

## Sprint detail

### Sprint 1: qualify .NET 10/11 and refresh the native API baseline

Tracking: [#32](https://github.com/atcarter714/DXSharp/issues/32).

Establish the next preview's supported runtime and binding contract after preview 2. Start from main after #31.

Required:
- [ ] Multi-target the five modern packages for net10.0 and net11.0 (Windows packages retain Windows SDK 10.0.26100.0); keep compatibility packages separate.
- [ ] Qualify clean package consumption, source-generated COM, JIT and NativeAOT on both runtimes. Update SDK selection, lock files, output-path assumptions and package checks together.
- [ ] Pin current retail Agility 1.620.2 runtime/metadata and regenerate the D3D12 surface. Resolve the audit's 107 missing top-level type names or explicitly document any unsupported projection.
- [ ] Report native coverage by source/version. A missing reference namespace must report unsupported/unmeasured rather than silently looking complete.
- [ ] Add signature/IID/inheritance/layout checks for supported ABI shapes, and negative tests for command-list/allocator recording ownership, pending work, failure cleanup and native recording dependencies.
- [ ] Establish a public support matrix: generated declarations, managed helpers and runtime-qualified workloads are separate columns.

Exit: both runtimes pass their advertised gates; exact package assets and external consumers are verified; the retail declaration delta is accounted for; invalid allocator reuse has an actionable deterministic failure.

Two-week planning timebox: runtime/package qualification and retail metadata are the committed first slice. ABI expansion pulls in by capacity; unfinished work is carried explicitly, never counted as qualified. Planning is not a fixed delivery-date promise.

### Sprint 2: generalize resource allocation, textures and rendering bindings

Tracking: [#33](https://github.com/atcarter714/DXSharp/issues/33).

Build reusable rendering infrastructure that Fleet Battler and the independent Noesis integration can share. Depends on Sprint 1's ownership/support contract.

Priority order:
- [ ] Implement fence-retired upload/readback rings and a descriptor allocator, with multi-frame wraparound and exhaustion behavior.
- [ ] Add general texture descriptions, subresource uploads, mip chains and DDS/BC fixtures; then arrays/cubemaps. Keep feature/capability failures explicit.
- [ ] Expose general root-signature/resource/sampler binding and configurable blend/rasterizer/MSAA pipeline state.
- [ ] Add GPU-side fence handoffs between copy, compute and graphics queues. Verify shared resource lifetime across the last consuming queue.
- [ ] Evaluate an optional D3D12MA adapter versus a minimal placed-resource allocator; record the choice, budgets and fragmentation behavior before committing the public API.
- [ ] Move a representative multi-texture scene onto these APIs and rebuild Fleet Battler and the external Noesis consumer.

Exit: repeated allocation/reclamation stays within the declared budget, pixel/numerical fixtures pass, and resources remain valid through all consuming queue fences.

The initial two-week slice is allocator/upload/descriptor contracts plus one consumer; general texture/pipeline expansion continues in the following slice if capacity requires. No claim of complete rendering-toolkit parity from this issue alone.

### Sprint 3: close Windows game API gaps with qualified integrations

Tracking: [#34](https://github.com/atcarter714/DXSharp/issues/34).

Reach a documented Windows gamedev binding baseline against Vortice, keeping raw declarations separate from optional managed helpers.

Priority order:
- [ ] Add separately pinned DirectStorage declarations and redistribution; verify real file-to-resource loading, completion/status reporting and supported decompression with deterministic data.
- [ ] Pin GameInput 4.1's versioned API source/runtime; qualify keyboard/mouse/controller input, focus, hotplug, capture and callback teardown.
- [ ] Add X3DAudio and qualify spatial output parameters; build reusable XAudio2 streaming/voice lifecycle helpers with device-change and shutdown tests.
- [ ] Generate Media Foundation and D3D12 video families with explicit namespace coverage. Verify one supported playback/decode/capture path before advertising a managed media toolkit.
- [ ] Document HWND/DPI/text-input/IME responsibilities and add reusable host boundaries for future WinUI/WPF/Avalonia adapters.
- [ ] Keep optional integrations and native redistributables out of consumers that do not select them, where package boundaries allow.

Exit: coverage matrix names each upstream version, architecture and tested workload; supported calls and unsupported driver/device cases are distinguishable; native dependency packaging works from a clean external application.

First two-week slice: DirectStorage and current GameInput. Audio and video are ordered continuation work, not an assumption that all media functionality fits in that timebox.

### Sprint 4: qualify production workloads and benchmark competitive parity

Tracking: [#35](https://github.com/atcarter714/DXSharp/issues/35).

Turn API breadth into measured production evidence. Depends on the infrastructure from Sprints 1-3; advanced feature families can be scheduled independently when their prerequisite APIs are ready.

- [ ] Qualify a textured PBR scene and one glTF animated/skinned character with offline asset manifests, deterministic fixtures and explicit import errors.
- [ ] Run the same workloads through Fleet Battler and the separate Noesis consumer; include resize, minimize/restore, DPI, focus, fullscreen, close and repeated startup/shutdown.
- [ ] Add structured DRED/device-removal capture, GPU markers, bounded recovery/teardown and native live-object reports.
- [ ] Record NVIDIA, AMD and Intel qualification separately; feature skips are not passes. Add HDR/VRR/latency and multi-window checks to configurations that advertise them.
- [ ] Build equivalent Release/JIT and NativeAOT benchmarks against Vortice and Silk.NET's D3D12 path; use TerraFX for native-shaped interop comparisons. Pin versions and normalize workloads, validation flags and presentation settings.
- [ ] Measure CPU submission time, allocations/frame, p50/p95/p99 frame time, upload/descriptor throughput, working set and retained GPU memory. Publish reproduction commands and raw results before making speed claims.
- [ ] Qualify advanced facilities one at a time: indirect rendering, enhanced barriers, mesh shaders/work graphs and general DXR updates/compaction. A declaration is not runtime support.
- [ ] Update the feature/support matrix and release notes from recorded evidence.

Initial timebox: production regression/diagnostics plus the reproducible benchmark harness. Hardware collection, character/toolkit work and advanced GPU features remain explicit follow-on slices until their acceptance checks pass. Four sprint headings are a planning sequence, not a promise of complete Windows/Silk portability parity in eight weeks.

## Acceptance and release policy

- Build clean external package consumers on both advertised runtimes; qualify JIT and NativeAOT separately. Retain native dependency versions and the exact source commit in release evidence.
- A coverage gate must identify absent reference families, new declarations, signature/layout changes and intentional exclusions. A zero-sized denominator is not full coverage.
- Use native fixtures for ABI/lifetime boundaries, WARP numerical/pixel checks for deterministic graphics, and hardware runs for driver/capability/presentation behavior. Unsupported features are recorded as skips, not passes.
- Qualify allocator/list recording states, early owner disposal, submission failure cleanup and retirement across all consuming queues. Native resource states and cross-queue ordering remain explicit contracts.
- Keep Fleet Battler as a package-based flagship consumer. Keep Noesis in its independent optional integration repository; its SDK license/runtime is not a dependency of the open DXSharp SDK.
- Use existing source/solution organization: libraries under `src`, examples under `samples`, documentation under `doc`; existing Libraries/Samples solution groups. New capabilities do not require a new top-level SDK folder.
- Prefer small integration PRs back to main, one acceptance workload each. Release new immutable preview versions after the applicable gates pass; avoid recreating a long stack of sample branches.
- A support or performance claim is made only after its recorded evidence exists. Generated COM and AOT support are not exclusive advantages over Vortice/TerraFX/Silk.

## Competitive scorecard

| Dimension | Baseline to record | Evidence required for a parity claim |
| --- | --- | --- |
| Native API coverage | Upstream package/header/WinMD version per family | Declaration and normalized ABI-shape diff, with exclusions |
| Managed API usefulness | Same application operation in each SDK | Complete resource, synchronization, error and disposal behavior |
| CPU submission | Equivalent draws, dispatches, descriptors and uploads | Release build, pinned versions, identical validation flags and repeated measurements |
| Frame pacing | Same scene and presentation settings | p50/p95/p99 frame time; CPU and GPU timing distinguished |
| Allocation and memory | Small and stress workloads | Managed bytes/frame, working set, upload peaks, retained GPU memory and reclamation |
| Reliability | Startup, close, resize, DPI, focus and fullscreen scenarios | Bounded tests, soak logs, debug-layer/live-object and device-removal evidence |
| Distribution | New project restoring only intended public packages | Correct assets/dependencies for each supported runtime/architecture |
| Tooling/content | Static PBR, then animated/skinned glTF fixture | Deterministic import, offline manifest, rendering result and explicit unsupported features |

Set regression budgets after collecting the initial reproducible baseline. Do not substitute screenshot quality or a type-name coverage percentage for performance or production readiness.

## Ordered continuation backlog

After the first slices: general DDS/BC/mip/array/cube textures; richer root signatures and pipeline state; cross-queue resource handoffs; placed-resource/residency policy; X3DAudio and reusable streaming audio; Media Foundation and D3D12 video; WinUI/WPF/Avalonia host adapters; HDR/VRR/latency controls; offline asset cooking, PBR and character animation; general DXR update/compaction; indirect rendering, enhanced barriers, mesh shaders and work graphs.

DirectML remains useful for explicit same-device GPU work. General model deployment should be evaluated through a separate optional Windows ML/ONNX integration, following Microsoft's current direction. Do not tie that inference dependency to the base graphics package.

Extend to ARM64 or additional graphics backends only with corresponding generation, packaging and runtime qualification. No blanket claim of support for unreleased future .NET versions.
