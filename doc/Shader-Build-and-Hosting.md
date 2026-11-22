# Shader assets and window hosts

This Preview 4 development increment adds a real DXC build task, precompiled shader containers, a shared frame renderer and a Win32 host. Preview 3 remains the published package version. DXSharp.Tools.Builds is a seventh package in the next candidate set; do not reference an unpublished preview from a standalone download.

## Shader compilation

DXSharp.Native exposes the main DXC metadata family; Core's ShaderCompiler uses generated COM. ShaderCompileOptions adds profiles, optional library entry points, source names, HLSL language version, optimization, definitions, include directories, debug information, warnings-as-errors and additional arguments. CompileFile resolves includes through a generated managed include callback backed by filesystem reads. A supplied callback still supports virtual shader sources.

The build task uses the pinned Microsoft.Direct3D.DXC executable, independently of the application's runtime compiler. The old task returned success without compiling and processed only the first item. It and its obsolete task factory/inline definition have been replaced; the historical DxcTool.Build.targets/scripts are compatibility-era files, not the supported target below.

For a package consumer of the next release:

```xml
<ItemGroup>
  <PackageReference Include="DXSharp.Tools.Builds" Version="0.4.0-preview.4" PrivateAssets="all" />
  <DXSharpShader Include="Shaders/Scene.hlsl" EntryPoint="VS" Profile="vs_6_0"
                 Defines="USE_LIGHTING=1" IncludeDirectories="Shaders/Include"
                 LogicalName="MyGame.Shaders.Scene.VS.dxil" />
  <DXSharpShader Include="Shaders/Scene.hlsl" EntryPoint="PS" Profile="ps_6_0"
                 LogicalName="MyGame.Shaders.Scene.PS.dxil" />
</ItemGroup>
```

No shader profile whitelist prevents newer DXC stages or library profiles. Omit EntryPoint for a library profile. Use distinct outputs/logical names for permutations; OutputFile overrides the default `obj/.../shaders/<file>.<entry>.<profile>.dxil`. The task rejects output collisions. AdditionalArguments metadata accepts semicolon-separated DXC argument tokens; output-path flags belong to the task.

Shaders are embedded resources by default. Set DXSharpEmbedShaders=false to stage them as build/publish content, with optional TargetPath metadata. Debug configurations emit PDBs under obj; PDBs, state and manifests are not automatically published. The task's outputs participate in Clean. Compiler selection can be explicitly overridden with DXSharpDxcToolPath, including a native Linux executable for cross-compilation; the default pinned package path is Windows x64.

DXC preprocessing performs actual transitive/macro include resolution on each build. The task hashes its result, compiler binaries and options to decide whether compilation is necessary. This handles include deletion and include-search shadowing as well as edits. It avoids fragile Makefile dependency parsing, including DXC paths containing spaces. Successful shader outputs have XML dependency/hash manifests and state files; missing/corrupt output or missing debug artifacts forces rebuilding. No-op compilation preserves output timestamps.

Framework builds its skinning VS/PS with this task and embeds DXIL. Its default SkinnedModelRenderer constructor loads those shaders; the explicit ShaderCompiler constructor remains available for development. Framework can be compiled for portable CPU/COM-fixture work on Linux without a DXC executable, but that assembly cannot render the skinning workload without its shader resources and cannot be packed as a Framework release. Supply DXSharpDxcToolPath when cross-building a complete package.

ShaderBytecode.Load/CopyFrom own the container storage and can create graphics/compute pipelines without invoking DXC. The container check recognizes a DXBC/DXIL header; it is not an exhaustive DXIL validator. The existing ShaderCompilation overloads remain source-compatible. Pipeline creation still invokes the graphics runtime/driver; build-time HLSL compilation does not precompile a portable driver PSO.

DXSharpCopyDxcRuntime=false disables the SDK package's DXC DLL staging for consumers that exclusively load bytecode. Native declares DXC entry points but loads them only when called. This does not remove the Core NuGet dependency graph or prevent an application from explicitly compiling. CharacterLab.Native's isolated package check asserts no dxcompiler.dll/dxil.dll deployment and verifies no DXC compiler module was loaded.

## Shared renderer and hosts

FrameRenderer lives in Framework, with no Windows Forms dependency. It owns the direct queue, frame slots, HWND swap chain, targets, resizing and fence-based resource retirement. The caller owns the device and HWND. RenderFrame exposes borrowed resources and an already-retired frame-slot index. WindowRenderOptions describes physical size, clear/depth settings, frame slots and optional frame limit.

Public renderer methods are single-threaded. BeginDrain starts queue completion on a worker; the host keeps pumping messages and keeps its HWND alive until that task completes, then disposes presentation resources before destroying the window. Disposal while recording is rejected. Raw device/resource callers retain the underlying SDK synchronization responsibilities.

NativeGame runs a native window and message loop with no WinForms reference, tracks its instance through a rooted GCHandle and pointer-sized window data, catches managed exceptions at WNDPROC, handles DPI changes and mouse capture, skips minimized/zero-sized rendering, and pumps messages through queue drain. DPI awareness is scoped to the calling thread. NativeGameWindow's handle is borrowed: request close through its API rather than destroying the HWND externally. FrameRenderer and native window mutation/disposal belong to their creating thread.

DesktopGame preserves its existing API and delegates rendering/presentation to FrameRenderer. Its WinForms adapter owns UI scheduling, focus/minimize observation and orderly close. The same renderer is therefore exercised by both hosts.

CharacterLab and CharacterLab.Native compile the same Program/OrbitCamera source and use the same CC0 asset and Framework renderer. The native executable references Framework directly, disables UseWindowsForms and compiler-DLL staging, and prints its host/compiler mode. Run either with --warp --frames=9 for a bounded check; native also accepts --resize-test. No second scene/render implementation is maintained.

## Qualification

- verify_shader_build.py uses a fresh external consumer of the packed task; verifies two entry points, spaces, unchanged builds, transitive includes, changed definitions, errors, deleted includes, debug PDBs, dependency manifests and Clean.
- The Windows COM fixture covers typed runtime compilation/includes, owned bytecode load/save and precompiled pipeline creation. The native-host fixture covers presentation, resize, frame reuse, render exceptions, close/drain and destroyed HWNDs under JIT/NativeAOT.
- Package qualification builds all 14 runnable samples on .NET 10/11. Both CharacterLab hosts run from those packages, including native resize, compiler-free deployment checks and WinForms dispatch after renderer retirement.
- The existing sample matrix continues to exercise the WinForms adapter and GPU/CPU skinning correctness. WindowsAndMessaging, HiDpi and KeyboardAndMouse declarations are now namespace roots and coverage-gated, alongside the prior native families.

## Remaining work

This is the HWND host slice, not completed WPF/WinUI adapters. Next add a composition presentation surface for WinUI 3 SwapChainPanel, then optional WinUI and WPF HwndHost adapters with explicit UI thread/DPI/input contracts. WPF HWND airspace remains a host limitation; D3DImage needs a separate surface interoperability path.

DXC reflection object models, disassembly/validation/linking conveniences, shader reload/cache services, richer content cooking and optional compiler package decoupling remain continuation work. General Game/Application lifecycle, video, DirectStorage, X3DAudio and current GameInput remain active SDK/framework backlog items.
