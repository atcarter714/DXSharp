# DirectX API coverage

The preview 3 candidate exposes public native bindings through DXSharp.Native. Core builds its owned graphics API on the same declarations. Consumers can use the generated interfaces and NativeInterface<T> without generating another set of DirectX types.

The baseline is Microsoft.Windows.SDK.Win32Metadata 71.0.45-preview, CsWin32 0.3.371, Agility 1.620.2 D3D12 metadata, .NET 10/11, and Windows x64. The Windows metadata package supplies the non-D3D12 families; its modular D3D12 provider selects the pinned Agility WinMD. Exactly one D3D12 provider is selected. Native API names live under Windows.Win32. Entry points and constants use Windows.Win32.PInvoke. NativeInterface<T> ownership helpers live under DXSharp.Interop.

## Measured native surface

ApiCoverage reads the pinned WinMD and built assembly with System.Reflection.Metadata. It compares top-level public type names, method names, field names, native entry-point names, and interface GUID attributes. Properties generated from native fields count under their property names. Callback Invoke declarations emitted as function-pointer wrappers are excluded from the member-name comparison. Shared Direct3D and raw input cover their root namespaces, with GameInput and XInput measured separately. Other rows include child namespaces.

| Family | Public types / metadata types | Public member names / metadata member names |
| --- | ---: | ---: |
| Direct3D 12 | 752 / 752 | 4882 / 4882 |
| DXGI | 136 / 136 | 1130 / 1130 |
| DXC | 35 / 35 | 202 / 202 |
| Shared Direct3D | 33 / 33 | 664 / 664 |
| FXC | 6 / 6 | 95 / 95 |
| D3D11-on-12 | 5 / 5 | 11 / 11 |
| D3D9-on-12 | 4 / 4 | 11 / 11 |
| DirectML | 213 / 213 | 1103 / 1103 |
| Direct3D 11 | 391 / 391 | 3877 / 3877 |
| Direct3D 9 | 202 / 202 | 2244 / 2244 |
| Direct3D 10 | 177 / 177 | 1873 / 1873 |
| Direct2D | 359 / 359 | 1600 / 1600 |
| DirectWrite | 207 / 207 | 1287 / 1287 |
| WIC | 139 / 139 | 948 / 948 |
| Audio, including XAudio2 and DirectSound | 474 / 474 | 3647 / 3647 |
| Raw input | 20 / 20 | 91 / 91 |
| DirectInput and HID | 141 / 141 | 5546 / 5546 |
| UIAnimation | 51 / 51 | 249 / 249 |
| DirectComposition | 62 / 62 | 294 / 294 |
| XInput | 16 / 16 | 117 / 117 |
| GameInput | 80 / 80 | 802 / 802 |

These counts measure the pinned metadata surface. They do not verify every overload, ABI layout, callback, native version, feature tier, or driver. In particular, interface generation through ID3D12Device16 does not imply that the selected runtime or adapter implements every interface. Query the required interface and check feature support before use.

Run a normal build, then run the coverage gate.

```powershell
./build/VerifyApiCoverage.ps1 -Configuration Debug
```

The normal build.ps1 invokes this gate for both target assemblies and saves reports under artifacts/api-coverage/<configuration>/<framework>.json. The standalone tool accepts semicolon-separated metadata and assembly lists. Missing reference metadata fails the gate for required families; DirectStorage is explicitly reported as unsupported/unqualified until its separate source is added. Overlapping metadata type definitions are rejected. These checks verify names and interface identifiers, not full signatures, layouts or runtime behavior. The gate also exercises missing-reference, overlapping-input and modified-IID negative fixtures.

## Ownership and native recording

QueryNativeInterface<T> returns an independent native reference and a source-generated projection. NativeInterface<T>.Adopt consumes an existing reference. FromBorrowed acquires a new reference. Dispose the lease after its final CPU call. GPU resources referenced by native commands must remain owned through the relevant completion fence.

RecordNative takes the recording callback and its DXSharp resource, descriptor-heap, and pipeline dependencies. The command list retains those dependencies until the submission retires. The callback keeps the command list open and preserves its allocator. External native resources still require an owner held through GPU completion.

## Depth and stencil

Core supports D16_UNORM, D24_UNORM_S8_UINT, D32_FLOAT, and D32_FLOAT_S8X24_UINT targets. The original two-argument CreateDepthTarget call continues to select D32_FLOAT. DepthStencilState configures depth comparison, writes, stencil masks, and independent front/back operations. SetRenderTargets binds up to eight color targets plus an optional depth-stencil target.

The DepthStencil sample uses a native RGBA render target, writes a stencil mask, draws through the mask, copies pixels to readback, and checks the center and corner. It also checks rejection of stencil clears on depth-only formats and disposes owners before submission to exercise retained dependencies. VerifySamples.ps1 includes its WARP run.

## Remaining parity work

| Area | Remaining work |
| --- | --- |
| DirectStorage | Obtain a separately pinned header/metadata source, generate bindings, and qualify queue, file, compression, and status operations. The current WinMD contains no DirectStorage declarations. |
| Runtime qualification | Run the existing COM fixture under Windows JIT and NativeAOT, and run WARP and hardware graphics gates. Extend workload tests for newly public interfaces. |
| API ergonomics | Add owned convenience operations where application workloads reveal gaps. Native declarations already expose the measured metadata operations. |
| New SDK revisions | Compare new Windows, Agility, DXC, DirectML, and DirectStorage versions against the pinned baseline, regenerate, and rerun ABI and workload qualification. |
| Vortice and Silk.NET comparison | Native coverage must be compared per API and version. This pass adds the metadata families above. It does not establish equivalent helpers, asset pipelines, windowing, or platform support. |

Primary references are [Microsoft Direct3D 12 interfaces](https://learn.microsoft.com/windows/win32/direct3d12/direct3d-12-interfaces), [CsWin32 generation](https://microsoft.github.io/CsWin32/docs/getting-started.html), [Vortice.Windows](https://github.com/amerkoleci/Vortice.Windows), and [Silk.NET](https://github.com/dotnet/Silk.NET).

## Native API migration in preview 3

The current upstream metadata no longer declares the older shared-Direct3D shader-cache surface (`D3D_SHADER_CACHE_*`, `D3D_VERSION_NUMBER` and `ID3DShaderCache*`), along with generated extensions and now-unused `SC_HANDLE` helpers. These declarations are absent from the selected retail reference. This is a source/binary compatibility change from preview 2; it does not establish that the underlying native APIs were removed from Windows. Consumers of those names need an explicit migration review against the selected SDK/header version. The owned graphics API keeps its existing signatures.

The retail type inventory accounts for the 107 D3D12 names missing from preview 2, including `ID3D12Device16`, graphics command-list versions 11/12, `ID3D12Resource3`, and current diagnostics/cache declarations. Interface availability still requires QueryInterface and adapter/runtime feature checks.
