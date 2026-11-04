# .NET 10 build baseline

This is the first migration step from the experimental .NET 7/8 implementation. It establishes a repeatable toolchain, dependency graph, build, small non-GPU test gate, and package-consumer build. It does not qualify the old COM ownership code, runtime shader compilation, sample rendering, NativeAOT, or DirectML for production.

## Prerequisites and commands

Install the SDK selected by `global.json` (10.0.403, allowing servicing patches in the same feature band) and PowerShell 7. Use Windows for tests and graphics. The Windows reference target is `net10.0-windows10.0.26100.0`; runtime OS/hardware support remains subject to the later deployment and interop qualification work.

From the checkout root:

```powershell
./build.ps1 -Configuration Debug
./build.ps1 -Configuration Release
```

Each invocation performs locked restore, solution compilation, the `BuildBaseline` tests, packing, and a build of an isolated temporary application that consumes `DXSharp.Framework` and its transitive DXSharp packages. The consumer uses a private package cache so an older build with the same preview version cannot satisfy the test. This is a package compile check, not a GPU/deployment acceptance test.

Cross-compilation on Linux/macOS is possible with the SDK's Windows targeting packs:

```powershell
./build.ps1 -Configuration Debug -SkipTests
```

The script requires explicit `-SkipTests` outside Windows; it does not report unexecuted Windows tests as passing.

For individual projects, `SolutionDir` and Visual Studio startup scripts are no longer prerequisites:

```powershell
dotnet restore src/DXSharp/DXSharp.csproj --locked-mode
dotnet build src/DXSharp/DXSharp.csproj --configuration Release --no-restore
```

Outputs are under `artifacts/bin`, `artifacts/obj`, `artifacts/test-results`, and `artifacts/packages`. The build log is `artifacts/build.binlog`. CI uploads package, test, and build evidence separately for Debug and Release.

## Dependency policy

Versions live in `Directory.Packages.props`; each project commits its `packages.lock.json`. Normal builds use `--locked-mode`. For an intentional dependency update, edit the central version, run `dotnet restore DXSharp.sln --force-evaluate -m:1`, review all lock-file changes, and rerun both configurations.

The legacy projection is deliberately frozen at CsWin32 0.3.49-beta and Win32Metadata 56.0.13-preview, the exact versions resolved by the original lower-bound ranges. This avoids mixing the baseline with the next step's generated-COM signature changes. Qualify CsWin32 0.3.367 and its matching metadata in the COM migration slice before replacing these pins.

Native build inputs move to retail Agility 1.620.1 and DXC 1.9.2609.5. The unused System.CommandLine dependency is removed, and the MSBuild utility dependency moves to patched 17.14.28 while the build-task project retains `netstandard2.0` for its host contract. No native runtime is downloaded by a prebuild script; dependencies come through NuGet restore.

Project-reference builds copy Agility files under `D3D12/` and DXC files next to the assembly, with debug-layer copies limited to Debug build output. **Copying Agility does not select it.** The existing runtime loader and NuGet native deployment/bootstrap must still be replaced and qualified in the subsequent interop/deployment work. These preview packages are not yet a standalone production distribution.

## Test boundaries

`BuildBaseline` labels the three existing DXGI data tests for mode-description conversions and rational values. Hosted Windows CI runs this group in both configurations and fails if the filter executes no tests.

The other tests remain present and compiled. Several create windows, depend on hardware/debug interfaces, exercise unqualified COM lifetime paths, or assume the developer's historical working directory. To run them deliberately on a suitable Windows machine:

```powershell
./build.ps1 -Configuration Debug -RunGraphicsTests
```

That flag runs the remaining suite in addition to the baseline. A failing full-suite run must be recorded and investigated; the baseline group is not evidence that all legacy tests pass. Hardware-independent ABI/ownership tests and a bounded WARP workload are the next migration gate.

## Build repairs and preserved scope

- Root and child `Directory.Build.*` files use explicit parent imports and file-relative paths. The active build no longer imports the old overlapping MSBuild/PowerShell/CSX orchestration.
- C# is pinned to 14.0, analysis to 10.0, and normal SDK configuration symbols are appended rather than overwritten. Legacy `DEV_BUILD` checks remain enabled in both configurations pending the ownership redesign; `STRIP_CHECKS` is not enabled.
- The Visual Studio shared-project container and HelloTexture's `.projitems` import remain available. The command-line build compiles the consuming project using the same shared sources.
- Solution configuration mappings now build DXC as Release in a Release/Any CPU build, and the build-tool project as Debug in a Debug/Any CPU build rather than the old InlineTasks configuration. The package-consumer check caught the DXC mismatch.
- An inaccessible `ABI.System.IDisposable` alias is corrected to `System.IDisposable`.
- RenderForm's existing false-valued properties receive designer default metadata required by modern WinForms analyzers; runtime defaults do not change.
- AdvancedSample supplies its actual command-list element type to the span-taking generic call. The legacy shader-target import is inactive; the sample's incomplete runtime implementation is not represented as working by a C# build.
- The existing strong-name key and friend-assembly declarations are retained. Packages are explicitly marked as experimental migration previews.
- Existing compiler/analyzer warnings remain visible. Warnings are not globally disabled or promoted to errors before their baseline is classified.

The README's architectural self-assessment remains applicable. This step creates the foundation for replacing the duplicated interface/wrapper and lifetime machinery; it does not perform that redesign.

## Recorded starting point

The original baseline is `2a3adc07e02a81d0615f3ae01ee37dd633b82cc1`. A direct project evaluation without `SolutionDir` fails to resolve the Agility import. Restoring with the original wiring reports duplicate Agility package references. Building the original core with its .NET 7/8 targets and SDK 10.0.403 reaches compilation but fails on the inaccessible `IDisposable` alias.

The original build-tool dependency, Microsoft.Build.Utilities.Core 17.8.3, also produces NuGet's high-severity advisory warning GHSA-w3q9-fxm7-j8fq. The upgraded dependency removes that baseline warning. Native execution and lifetime defects from the architecture assessment remain tracked for the next step.
