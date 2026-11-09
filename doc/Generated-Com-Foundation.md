# Generated COM foundation

This slice establishes a second, independently usable path through DXSharp: generated native projections, deterministic C# owners, explicit command recording, shader compilation, presentation, and DirectML tensor execution. It builds on the .NET 10 baseline. The original SDK and its samples remain available while their public surface is ported.

The architectural direction follows the README's self-assessment: preserve DirectX semantics and direct access, make the managed layer idiomatic, and remove the duplicated public-interface/implementation hierarchy. The optional application framework remains separate from the graphics SDK.

## Assemblies and dependency direction

| Assembly | Responsibility | Depends on |
| --- | --- | --- |
| `DXSharp.Core` | Private generated DXGI/D3D12/DXC/DirectML declarations; native activation; ownership; public graphics and shader APIs | Native Agility and DXC SDK packages |
| `DXSharp.DirectML` | Public tensor shape, DirectML device, compiled dense Float32 addition, initialization/binding/execution | `DXSharp.Core`, native DirectML package |
| Existing `DXSharp`, `DXSharp.Dxc`, `DXSharp.Framework` | Existing proof-of-concept API during the port | Existing frozen projection |

The migration target is to promote the modern implementation into the main DXSharp package after API and runtime parity gates pass, then port the framework and retire the legacy lifetime machinery. The two implementations are a transition boundary, not a permanent duplication strategy. The modern samples depend only on the new path.

The generated declarations are implementation details. A signed friend relationship lets the DirectML component reuse the same declarations and ownership rules without a second native projection. Public graphics types are sealed owners; they do not mirror each native interface with another public interface and implementation pair. `GraphicsObject.QueryNativeInterface` supplies an independently owned `SafeHandle` for deliberate native integration.

Core uses CsWin32 **0.3.367**, Win32Metadata **71.0.37-preview**, `GeneratedComInterface`, `GeneratedComClass`, and `LibraryImport`. Runtime marshalling is disabled in the modern assemblies. CsWin32 runs as a build task; generated output lives under `artifacts/obj`, outside source globs. The legacy projects retain their previous generator pins. These are intentional per-project overrides, not a solution-wide projection replacement.

## Ownership contract

`ComOwner<T>.Adopt` consumes exactly one returned native interface reference. It creates a private `UniqueInstance` generated projection, which owns its own queried references. Disposal calls `ComObject.FinalRelease` on that unique projection and disposes the original native reference exactly once. Projection failure releases the consumed output. `Clone` obtains independent ownership. No public method returns a shared runtime RCW that the caller must manually release.

The only handwritten vtable calls are the invariant IUnknown slots: QueryInterface, AddRef and Release. A narrow source-generated `IFactory2Raw` declaration adapts DXGI's typed swap-chain output to a raw owning output; it inherits its preceding slots from metadata-generated `IDXGIFactory1`. This avoids mixing the default shared output projection with deterministic unique ownership. No graphics call uses handwritten vtable offsets.

Internal stack-scoped borrows use `SafeHandle.DangerousAddRef/DangerousRelease` and cannot escape to the heap. Borrow values must not be copied. The owner and native lease remain distinct; disposal of the public owner does not invalidate an active borrow or a separately retained submission reference. Callers synchronize disposal with CPU method calls on the same owner. DXC compilation and queue operations have their own locks; command-list recording and allocator management are single-threaded per object.

### GPU lifetime

Recording captures independent native references to resources, descriptor heaps, pipeline state, root signatures and the allocator. Submitting transfers those references to a queue completion record. The queue also retains the native command list. Fence completion releases that record and makes allocator/list reuse legal. Public wrappers can therefore be disposed after their final recording use, before submission, without releasing the native objects required by that submission.

Pending submissions root their queue independently of application references. A nonblocking completion poll retires finished records, so dropping the last managed queue reference cannot finalize an in-flight resource. Explicit `Wait` retires immediately. The poll does not turn a timeout or signal failure into success; unresolved work stays rooted. A removed device permits retiring all records because its execution has stopped. Queue disposal drains its work; a timeout throws and leaves the owner live for another wait/disposal attempt.

The caller still manages resource states, cross-queue synchronization, descriptor overwrites and concurrent use. A fence value from another queue is not interchangeable with this queue's completion token. Each queue owns a private fence. Objects from different device identities are rejected before recording or binding. This is a thin SDK contract, not an implicit render graph or a full D3D12 hazard validator.

## Native activation and distribution

Agility 1.620.1 is selected through `ID3D12SDKConfiguration1.CreateDeviceFactory(620, absoluteSdkPath)`. Debug configuration is applied to that local factory before device creation. This avoids the old module initializer, developer-mode SDK override and process-global SDK selection. `DeviceOptions.UseAgility = false` explicitly selects the system runtime.

System D3D12/DXGI load from the Windows system directory. DXC and DirectML load from `AppContext.BaseDirectory`; Agility resides under `D3D12/`. Successful module loads are cached for process lifetime so callbacks and finalizers cannot call into an unloaded module. Missing app-local dependencies fail with the expected absolute path.

Modern NuGet packages depend on the official native SDK packages and include `buildTransitive` copy targets. Build and publish copy the selected architecture's files to those same paths, including for a framework-dependent single-file consumer. DLLs remain outside the single-file executable. Debug layers are copied for Debug builds and excluded from publish. The separate DirectML package brings DirectML 1.15.4 only when that component is referenced. Native files are consumed from their original packages, with their original licenses.

Set `RuntimeIdentifier` to `win-x64`, `win-x86`, or `win-arm64` for deployment. Architecture selection otherwise defaults to x64; an arbitrary AnyCPU executable is not a portable native deployment. x64 is the qualification target in this slice. x86/ARM64 copy mappings are present but their ABI execution is not yet qualified. When updating native versions, update both `Directory.Packages.props` and the pinned package paths in `build/msbuild/DXSharp.*.targets`, then rerun the isolated consumer test.

## Implemented surface and remaining port

| Area | Implemented in the modern path | Subsequent work |
| --- | --- | --- |
| DXGI | Adapter preference/WARP selection; flip HWND swap chain; acquire/present/resize | Adapter enumeration and selection API, HDR/color space, tearing, fullscreen policy |
| D3D12 | Device/local factory; direct/compute/copy queues; allocators/lists; completion; buffer map/copy/barriers; RTV heaps; graphics/compute roots and PSOs | General textures/views, upload footprints, descriptor allocation/tables, samplers, stream PSOs, bundles, render passes, enhanced barriers |
| Shaders | DXC Compiler3/Result; UTF-8 source; arguments; generated include/blob callbacks; status and warnings; copied object/reflection/PDB/hash/root-signature/disassembly outputs | Structured reflection API, container/linker tooling, cache, cancellation through worker isolation |
| DirectML | Immutable dense Float32 shape; device; compiled equal-shape Add; initializer; descriptors; temporary/persistent allocations; binding; dispatch | General binding/execution sessions, reusable initialization/storage, GEMM/convolution/activations, graphs, broadcasting/strides/dtypes, feature queries |
| Diagnostics | Bounded InfoQueue snapshot, device-removed HRESULT, operation-preserving exceptions, sample evidence, ProcDump launcher | DRED setup and breadcrumb/page-fault decoding; live-object regression gates; device-recovery policy |
| Samples | ModernTriangle with resize, bounded frames and GC stress; ModernCompute with numerical DXC and optional DirectML checks | Texture, async upload, multi-frame synchronization, mesh/raytracing, richer DirectML workloads |

Declaring newer native interfaces in the generation manifest does not mean their friendly APIs or runtime paths have been qualified. General API completeness, zero-allocation hot paths and performance parity still require measured gates. `IsAotCompatible` is not yet advertised on the packages: the passing AOT console workloads qualify their exercised paths, while broader public API coverage remains part of the port. The WinForms triangle itself is not an AOT sample; ModernCompute is the console candidate.

## Build and qualification

Use an x64 Developer PowerShell with the C++ build tools available for the standard Windows build:

```powershell
./build.ps1 -Configuration Debug
./build.ps1 -Configuration Release
./build/VerifyInterop.ps1 -Configuration Release -PublishAot

dotnet run --project samples/ModernTriangle -c Debug -- --frames=600 --gc-stress
dotnet run --project samples/ModernTriangle -c Debug -- --headless --warp
dotnet run --project samples/ModernCompute -c Debug -- --directml
dotnet run --project samples/ModernCompute -c Release -- --warp --directml
```

ModernTriangle compiles shaders, checks failed DXC diagnostics and include callbacks, dispatches a known compute result, and renders only when headless mode is absent. It uses a fully synchronized frame path to isolate ABI/lifetime correctness before introducing multiple frames in flight. ModernCompute verifies `1234` from a compiled compute shader and, with `--directml`, `[11, 22, 33, 44]` from a native DirectML Add. Both exercise disposal of public owners before submission while native leases remain live. `--no-debug` disables validation when deliberately comparing a retail run. `--system-runtime` bypasses Agility.

The native C++ fixture checks actual vtables, inherited methods, 16-byte structure returns, buffer lengths, failed QI/projection cleanup, independent clones, disposal and scoped borrows, forgotten-owner finalization, repeated GC stress, native-held callbacks, and the actual generated DXC include/blob callback types. It does not need a GPU or the Windows DLLs. The tensor tests verify copied shapes, invalid dimensions and checked byte counts; binding and tensor descriptor sizes are compared with the platform C++ definitions (Windows SDK types on Windows). Include resolver exceptions are checked across the native HRESULT boundary. `VerifyInterop.ps1` builds the fixture with MSVC on Windows or g++ on Linux, executes the managed suite, and optionally publishes/runs its NativeAOT counterpart.

CI adds Windows/Linux JIT and AOT native-fixture runs, Windows WARP compute/DirectML checks, and a published AOT ModernCompute run. The isolated package consumer restores into a private cache, builds, publishes a single-file executable, compares deployed native file hashes to the selected official SDK package files, and rejects debug layers in publish output. CI configuration is an execution gate, not evidence that a workflow has already passed.

### Evidence from this development host

- Linux x64, SDK 10.0.403: full solution Debug and Release builds passed; new modern projects compiled without warnings. Existing legacy warnings remain visible.
- Native fixture JIT and Linux NativeAOT: **2,288 checks passed per run**, **2,004 native fixture objects destroyed per run**, **zero remaining native fixture objects**. This includes the generated DXC include/blob callback path, not actual DXC compilation on Linux.
- Linux NativeAOT publishing required a development-host workaround for MSB4216: an external, uncommitted MSBuild import ran the ILLink tasks in process instead of the failing out-of-process task host. The resulting native ELF executable passed the same suite. No workaround or warning suppression is shipped in the repository.
- Debug and Release packing and isolated consumer build/single-file publish passed, including deployed native hashes and debug-layer exclusion.
- GitHub Actions [run 41980630248](https://github.com/atcarter714/DXSharp/actions/runs/41980630248), implementation commit `222112ad1f968dbad12693f6a7e1a1972bcd1ca5`: all four jobs passed. Windows Debug/Release built the full solution, passed the three baseline tests and 2,288 native-fixture checks, packed the SDK, and verified isolated package build/publish output. Windows and Linux both passed the fixture under JIT and NativeAOT. No local task-host workaround was needed in CI.
- Windows WARP, JIT with validation enabled: DXC compute/readback returned `1234`; DirectML Add returned `[11, 22, 33, 44]`. The published Windows NativeAOT console repeated both numerical checks successfully. These runs also exercised early public-owner disposal, scoped mapping and forced GC.
- Windows interactive presentation, resize and long-duration rendering remain to be qualified on a desktop. The earlier baseline sample's Windows success was reported by the maintainer; the later CLR crash remains undiagnosed.

The Windows gates caught two defects during this slice. Verifiers now query MSBuild for output paths rather than assuming the absence of a platform subdirectory. The include callback returns the exception's failing HRESULT directly: `Marshal.GetHRForException` had left thread-local COM error information that changed a later unsupported-interface exception. Debug builds enable validation by default; Release samples use retail configuration. The Windows JIT numerical job deliberately uses Debug so the matching packaged SDK layers are available.

## Crash investigation

The original BasicSample remains useful as a reproduction. Keep its exact binaries and PDBs with the dump; do not infer the failure from a managed exception alone. A CLR access violation can originate at an incorrect ABI boundary, a released native object, or unrelated native code. This foundation removes several lifetime mechanisms from the new path, but is not proof of the original crash's cause or a verified fix.

With Sysinternals ProcDump available, launch either sample under full-dump capture:

```powershell
./build/CaptureCrash.ps1 `
  -Executable ./artifacts/bin/BasicSample/Debug/net10.0-windows10.0.26100.0/BasicSample.exe `
  -ProcDumpPath C:/Tools/ProcDump/procdump64.exe

./build/CaptureCrash.ps1 `
  -Executable ./artifacts/bin/ModernTriangle/Debug/net10.0-windows10.0.26100.0/ModernTriangle.exe `
  -ProcDumpPath C:/Tools/ProcDump/procdump64.exe `
  -Arguments @('--frames=100000', '--gc-stress')
```

If Developer PowerShell selects an `x64` platform subdirectory, use the matching executable path. MSBuild can report the assembly path with `dotnet msbuild <project.csproj> -p:Configuration=Debug -getProperty:TargetPath`; the apphost is beside it with an `.exe` extension.

The launcher records the executable hash, arguments, OS, architecture and SDK/runtime inventory, and asks ProcDump to launch only that executable and capture a full dump on an unhandled exception. It does not change global crash settings. ModernTriangle also writes `diagnostics.json` and managed failures under its evidence directory. Next investigation requires the crashing native instruction, native/managed stacks, loaded-module versions and device-removal evidence from that exact run.

Primary references: [source-generated COM](https://learn.microsoft.com/dotnet/standard/native-interop/comwrappers-source-generation), [unique ComObject.FinalRelease](https://learn.microsoft.com/dotnet/api/system.runtime.interopservices.marshalling.comobject.finalrelease), [DirectML binding](https://learn.microsoft.com/windows/ai/directml/dml-binding), [Microsoft's HelloDirectML](https://github.com/microsoft/DirectML/blob/master/Samples/HelloDirectML/main.cpp), [ProcDump](https://learn.microsoft.com/sysinternals/downloads/procdump).

## Desktop samples

`DXSharp.Framework.Desktop` hosts the WinForms window, frame timing, back buffers, resize, optional depth target, presentation, and shutdown. The caller owns its device, shaders, pipelines, and scene resources. The draw callback records into the frame's borrowed command list. The host currently uses one allocator and waits for each draw submission.

- **ModernTriangle** exercises the shared host, including its existing `--resize-test`.
- **HelloTexture** ports the checkerboard triangle from [Microsoft's D3D12 Hello Texture](https://github.com/microsoft/DirectX-Graphics-Samples/tree/master/Samples/Desktop/D3D12HelloWorld/src/HelloTexture). It uploads a single-mip RGBA8 texture with row padding and samples it through an SRV and static sampler.
- **AdvancedSample** exercises per-frame matrices, lighting, and a D32 depth target with a rotating cube. See [the sample README](../samples/AdvancedSample/readme.md).

Run either new sample with `--warp --frames=4` for the bounded Windows software-adapter check. With no arguments, the sample uses the hardware adapter and runs until its window closes.

The next Microsoft port candidates are [Hello Frame Buffering](https://github.com/microsoft/DirectX-Graphics-Samples/tree/master/Samples/Desktop/D3D12HelloWorld/src/HelloFrameBuffering) for multiple frames in flight, [ExecuteIndirect](https://github.com/microsoft/DirectX-Graphics-Samples/tree/master/Samples/Desktop/D3D12ExecuteIndirect) for GPU-driven draws, and [D3D12 Multithreading](https://github.com/microsoft/DirectX-Graphics-Samples/tree/master/Samples/Desktop/D3D12Multithreading) for a shadow pass and parallel recording.
