# DXSharp 1.0 qualification

The supported SDK surface is the generated COM stack on .NET 10. Publish a support matrix for Windows versions, architectures, runtime modes, Agility SDK, DXC and DirectML before tagging 1.0. Qualify each advertised combination with a clean external package consumer. Document the legacy RCW API as a compatibility surface and give migration examples for ownership, queues, and window lifecycle.

## Ownership contract

Legacy `ComPtr<T>` acquires its own typed reference from an RCW or borrowed raw pointer. If the canonical IUnknown pointer differs, the owner holds a separate identity reference. The owner accounts for its acquisitions independently of the native return count. Clone and Cast create owners. Detach transfers one typed reference and the recipient releases it. Use borrowed pointers and dispose owners in a valid COM apartment. Synchronize use of borrowed pointers with owner mutation. Legacy `IUnknown` RCW calls now bind to managed extensions. Recompile binaries that called the former interface members.

The generated COM path has its own ownership and GPU retirement tests. A successful native call proves ABI reachability. Lifetime qualification also requires failure cleanup, zero unexpected live objects, and reclamation at the right queue fence. Microsoft documents that interface references are balanced individually and that AddRef and Release return values are diagnostic. [Implementing reference counting](https://learn.microsoft.com/windows/win32/com/implementing-reference-counting)

## Release gates

| Layer | Required evidence |
| --- | --- |
| Managed contracts | Cover null, invalid range, disposed state, repeat disposal, transfer, replacement failure, and concurrent operations wherever promised. Run isolated tests in both configurations. |
| ABI and COM | Check each shipped interface's IID, inheritance and slot ordering against pinned metadata. Compare size, offsets, argument/return layouts, callbacks and calling conventions for every handwritten shim and ABI shape. Use an instrumented native fixture for exact AddRef/Release balance, distinct interface addresses, failure rollback and final destruction. Run JIT and NativeAOT for each architecture advertised. |
| D3D12 and DXGI | Run WARP create/submit/readback tests with known outputs, debug errors fatal, and bounded waits. Exercise queue/allocator reuse, early owner disposal, presentation, resize, minimize/restore and close. Validate sampled pixels or numerical results where a draw/dispatch can silently do nothing. |
| Resilience | Test repeated startup/shutdown, outstanding work, injected HRESULTs, device removal diagnostics and bounded teardown. Retain logs, adapter/driver data, native live-object reports, DRED and dumps for failure triage. |
| Hardware | Run a release soak on the RTX 3080 Ti and qualify additional vendors and architectures before advertising them. Gate DXR and DirectML on queried capabilities and record unsupported configurations separately from passes. |
| Distribution | Locked restore, deterministic packages, symbols and license assets. Consume packages from a clean checkout on each supported architecture and validate packaged native dependencies in the advertised JIT/AOT modes. |

Every public ownership transition and every handwritten ABI boundary needs direct coverage. Exercise generated forwarding methods by interface family and ABI shape, with representative real calls across inherited slots. Track behavior and uncovered risk in the support matrix rather than setting one solution-wide line coverage target. Keep hardware-dependent checks separately runnable with evidence and require them for the features they qualify.

## Sample sequence

The [sample index](../samples/README.md) lists runnable projects. The next ports should each introduce a distinct SDK capability and include a bounded verification mode.

| Port | Scope | Acceptance |
| --- | --- | --- |
| [Hello Frame Buffering](https://learn.microsoft.com/en-us/samples/microsoft/directx-graphics-samples/d3d12-hello-world-samples-win32/) | Per-frame allocators and fence values, two or more frames in flight | Render through repeated resize and close with outstanding work, verify retirement and no debug errors |
| [N-Body Gravity](https://github.com/microsoft/DirectX-Graphics-Samples/tree/master/Samples/Desktop/D3D12nBodyGravity) | Ping-pong structured buffers, compute/graphics queues, particle draw | Compare a seeded small step with a CPU reference, then qualify multi-frame queue synchronization |
| [Raytracing Hello World](https://learn.microsoft.com/en-us/samples/microsoft/directx-graphics-samples/d3d12-raytracing-samples-win32/) | BLAS, TLAS, shader table, ray dispatch and output presentation | Check capability and alignment, validate a small image on DXR hardware, exercise resize and rebuild |
| [Multithreading](https://github.com/microsoft/DirectX-Graphics-Samples/tree/master/Samples/Desktop/D3D12Multithreading) | Shadow map, frame resources and parallel command recording | Validate depth and shadow results and stress frame resource reuse |

Frame buffering comes first because it establishes the frame lifetime contract used by the larger samples. Keep the planar projection sample as a small rendering example alongside the eventual shadow-map port.
