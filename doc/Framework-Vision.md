# DXSharp game framework direction

DXSharp's product goal is a modern .NET game-development framework and toolchain above its independently usable DirectX SDK: game lifecycle and timing, rendering/materials, content, animation, input/audio, scenes, diagnostics, templates and inspection tools. DirectXTK, DirectXMesh and DirectXTex are component/workflow references, not the upper limit of that goal. This is not a claim of present feature parity or a completed editor.

## Repository and package boundaries

Keep development in this repository while the SDK and framework evolve together. Core never depends on Framework, Assets or a window host. Assets remains device-independent. Framework depends on Core and Assets. Framework.Desktop implements Windows hosting over Framework. DirectML and the separately maintained Noesis integration are optional. All library projects live under `src` and the solution's Libraries group; samples live under `samples` and Samples.

Every in-repo sample uses project references. The existing isolated consumer gate converts those same projects to references to freshly built packages and builds them outside the checkout on both supported runtimes. A standalone download uses a pinned, actually published package version; do not point a download at an unpublished development version. No second implementation of CharacterLab is maintained.

## Preview 4: explicit Framework migration

The old `DXSharp.Framework` was an experimental RCW-dependent collection of display helpers and incomplete application/thread/lifecycle scaffolding. This increment replaces that unshipped modern surface with a generated-COM-based framework foundation. This is a source/binary breaking change for callers of the previous experimental Framework types; it is not a transparent replacement. Existing published versions and repository history retain that code. The original DXSharp and DXSharp.Dxc compatibility projects remain independently buildable, and compatibility package verification references them explicitly rather than obtaining them transitively through Framework.

Removed Framework APIs include AppManager, ThreadSystemBase, the miscellaneous lifecycle interfaces, Compressor/FolderEnumeration, DeviceCaps/DisplaySetup, the legacy display data/settings and extension helpers, IScreen and FrameworkErrorException. Consumers must retain their old version or migrate display/device work to Core and Desktop. No claim of a general scene system, full audio/input toolkit, application lifecycle replacement or content manager is made by this increment. Reintroduce useful facilities with complete contracts and real consumers.

Implemented first slice:

- GameClock: instance-owned fixed updates, render interpolation, pause and bounded catch-up with a dropped-time counter. The host supplies monotonic elapsed time.
- SkinnedModel: strict-validation GLB import for one opaque triangle primitive, one skinned mesh node, embedded images, four influences, explicit LINEAR/STEP channels and generated tangent frames. Unsupported morphs, sparse data, extra influences and interpolation modes fail explicitly.
- AnimationPlayer: shortest-path quaternion interpolation, continuous interrupted fades, looping/one-shot playback, in-place horizontal motion and optional translation extraction across loops. Root-velocity blending and root rotation extraction remain separate work.
- SkinnedModelRenderer: persistent vertex/index resources, per-frame joint/normal palettes, four material textures, and an optional reference grid. A caller must retire each frame slot before rewriting its palette.
- CharacterLab: the uploaded CC0 knight is the first shared workload. CPU deformation remains a reference; GPU deformation uses the same HLSL function in the vertex and verification compute paths.

The frame-time display measures CPU recording and presentation cadence separately from correctness tests. It is not a comparative benchmark or a guaranteed hardware performance target.

## Ordered continuation

1. Finish the Game/Application lifecycle and host scheduling contract using CharacterLab and Fleet Battler. Define initialize/load/update/render/unload and exception/cancellation behavior; keep timing and input coherent across focus, pause and minimize.
2. Generalize materials, sampler/pipeline descriptions, lighting and debug/sprite/font drawing; complete DDS/BC/mip content preparation and GPU-side queue handoffs.
3. Build a versioned incremental content pipeline with shader/texture/mesh processing, dependency hashes, source/license manifests and explicit unsupported import paths. Evaluate optional DirectXTex/DirectXMesh tools in the build process.
4. Add reusable scenes/transforms, content lifetime/async loading, input actions, audio and animation blending/events. Extract facilities from real Fleet Battler and character workloads rather than introducing empty interfaces.
5. Deliver project templates, asset/animation inspection, shader reload, profiling, packaging and deployment as the initial Game Studio experience. Expand editor functionality from those working tools.

Acceptance remains workload-based: CPU/GPU deformation agreement; clear allocation and fence ownership; package-only consumption of the same samples; JIT and NativeAOT on .NET 10/11; lifecycle/presentation checks and separately recorded hardware performance. Maintain the Windows API parity backlog alongside the framework work.

## Shader and host continuation

The next working slice adds precompiled Framework shader resources, typed runtime compiler options and a packaged incremental DXC task. FrameRenderer now separates frame slots/presentation from UI scheduling; DesktopGame keeps its API and NativeGame supplies an independent Win32 host. CharacterLab.Native shares the existing scene and has no WinForms reference. See [the shader/host contract](Shader-Build-and-Hosting.md) for packaging, cross-compilation limits and qualification. WinUI/WPF adapters and the full application lifecycle remain continuation work.
