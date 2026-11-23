# SDK support matrix

This matrix distinguishes declarations, managed workflows and runtime qualification. Preview 3 adds .NET 10/11 assets; it does not declare every native feature production-qualified.

## Build and deployment

| Dimension | Contract |
| --- | --- |
| Build SDK | .NET SDK 11.0.100, pinned in global.json |
| Modern package targets | net10.0-windows10.0.26100.0 and net11.0-windows10.0.26100.0 |
| Assets package | net10.0 and net11.0 |
| Graphics architecture | Windows x64; other architectures are not qualified |
| Windows API target | Windows SDK 10.0.26100.0; older Windows builds require separate qualification |
| Native runtime dependencies | Agility 1.620.2, DXC 1.9.2609.5, optional DirectML 1.15.4 |
| Native declarations | CsWin32 0.3.371, Win32Metadata 71.0.45-preview, Agility 1.620.2 WinMD |
| Compatibility packages | Existing RCW surface remains separate; modern AOT promises do not apply to it |
| Desktop hosts | Native Win32 and WinForms HWND presentation; optional WPF child-HWND viewport ([contract](Wpf-Hosting.md)); optional WinUI 3 composition viewport ([contract](WinUI-Hosting.md)) |
| Linux | Portable native COM fixture only; no D3D12 renderer support |

Samples default to .NET 10. Select the newer runtime with `-p:DXSharpRuntimeMajor=11` or the qualification scripts' `-RuntimeMajor 11` option. Each runtime has its own JIT/NativeAOT CI jobs and evidence. Building the checkout requires the pinned SDK; consuming the net10 package assets does not require changing an application's target to net11.

## Feature qualification

| Family | Native declarations | Managed workflow | Required runtime evidence |
| --- | --- | --- | --- |
| D3D12/DXGI | Current pinned retail inventory | Devices, queues, resources, pipelines, meshes, depth/stencil and HWND presentation | WARP pixels/numerics, lifecycle and hardware checks |
| DXC | Pinned declarations | Compilation and include callbacks | Native callback fixture and real shader compilation |
| DirectML | Pinned declarations | Add/GEMM with optional fused ReLU | Expected-value checks on supported runtime/device |
| D3D9/10/11 and on12 interop | Pinned declarations | No equivalent modern owned framework | Individual native workloads before broader support claims |
| D2D/DirectWrite/WIC/composition | Pinned declarations | Native escape hatch | Family-specific end-to-end qualification remains |
| Audio/input | Pinned audio, Raw Input, DirectInput/HID, XInput and older GameInput | Selected sample-local behavior | Current GameInput, X3DAudio and reusable audio/input integrations remain |
| DirectStorage | Missing separate metadata source | None | Explicitly unsupported/unqualified |
| Media Foundation/D3D12 video | Not projected | None | Separate binding and workload qualification remains |
| Advanced D3D12 features | Availability varies by pinned interface | Selected native recording paths | Query interface/features and qualify each claimed workflow |

CI validates only the matrix recorded for its exact commit. A skipped hardware capability is not a pass, compilation is not execution, and a name/IID gate is not a complete ABI proof. Retain adapter/driver/runtime details with hardware results. See [qualification](SDK-Release-Qualification.md), [coverage and migration notes](API-Coverage.md), and [the parity roadmap](Competitive-Parity-Roadmap.md).

## Recording and ownership

Only one command list may record with an allocator at a time. Close or dispose the active recording before reusing that allocator; an allocator reset also requires completed submissions. A command-list reset requires a closed, retired list. These checks reject invalid use before native reset/create calls. Native recording callbacks must preserve the list's allocator and recording state and declare their DXSharp dependencies.

Recording remains single-threaded per list. Applications synchronize owner disposal, allocator reset and submissions, and remain responsible for barriers, resource states and cross-queue ordering. Recorded resource references are retained through the relevant submission fence.

## Allocation infrastructure in preview 3

Preview 3 adds bounded descriptor pools and upload rings with per-recording allocation leases. See [resource allocation](tutorials/08-resource-allocation.md) for ownership, no-overwrite and exhaustion contracts. A retired recording must be reset before another submission because its suballocations may have been reclaimed. Multi-queue lifetime tracking does not supply queue ordering or resource barriers. Readback rings and general texture/pipeline expansion remain planned.

## Preview 4 development: texture transfers

Bounded readback rings, typed single-plane sampled Texture2D resources, explicit mip/array/cube descriptions, pitched subresource upload/readback and BC block transfer are implemented in the next increment. Native format/plane support is queried. DDS parsing, generated mips, depth/video planes and GPU-side queue handoffs remain outside this increment. See [texture transfers](tutorials/09-texture-transfers.md); runtime qualification is recorded by the exact CI commit.

## Preview 4 development: framework and character workload

Framework is now a modern Core/Assets consumer, with GameClock, AnimationPlayer and a persistent GPU skinning/material renderer. Its former experimental public helpers are removed in an explicit breaking migration; see [Framework vision](Framework-Vision.md). CharacterLab uses project references in the checkout and the same isolated package qualification as every other sample. Native camera/message bindings are shared through DXSharp.Native; Desktop schedules frames from idle with Present pacing and retains a timer wakeup for minimized/closing states. Qualification status belongs to the exact CI run, not this description.
