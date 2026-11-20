# Descriptor pools and upload rings

`DescriptorPool` reserves contiguous ranges in a single descriptor heap. `UploadRing` reserves aligned, contiguous byte slices in one upload buffer. Both are bounded: `TryAllocate` returns false on exhaustion or fragmentation; `Allocate` throws. Neither waits for the GPU nor silently creates another heap or buffer.

```csharp
using var descriptors = device.CreateDescriptorPool( 128 );
using var uploads = device.CreateUploadRing( 4 * 1024 * 1024 );
using var allocator = device.CreateCommandAllocator( );
using var commands = device.CreateCommandList( allocator );

using var texture = device.CreateSampledTexture2D(
    commands, descriptors, uploads, width, height, rgbaPixels );
commands.Close( );
queue.Wait( queue.Submit( commands ) );
```

Texture creation reserves a descriptor for the returned texture and a temporary upload slice. The upload owner is disposed after recording; its bytes remain reserved until the recording is discarded or its submission completes. The texture owns its descriptor reservation. Each recorded draw retains that reservation independently.

`HelloTexture` uploads two textures through one ring and one descriptor heap. The rendering fixture samples two suballocated descriptors, checks sRGB and linear pixels, disposes texture owners before submission, and verifies reclamation after completion.

## Buffer slices

```csharp
using var slice = uploads.Allocate( 256 );
slice.Write<uint>( [714, 1234, 5678] );
commands.CopyBuffer( slice, destination, 12 );
// The slice owner can be disposed after its final CPU use. Its recorded range remains reserved.
```

The default alignment is 256 bytes, matching constant-buffer placement. Texture upload placement uses 512-byte alignment; row pitch comes from `GetCopyableFootprints`. An allocation never wraps across the end of the buffer. Free ranges coalesce after out-of-order retirement.

Managed writes freeze when an allocation is first recorded and become legal again only when all recorded uses retire or are discarded. This includes closed lists that have not yet been submitted. Dispose unused allocation owners explicitly so their reservations can be returned.

## Ownership and submissions

An allocation has one public owner and an independent lease for every command-list recording that uses it. Disposing the owner does not release any recording lease. Two different recordings submitted to different queues keep the same range reserved until both uses retire. Timed-out waits retain their leases. Disposing a pool stops new allocations; existing allocations can still use their backing object.

A recording is submitted once. After completion, reset the command allocator and list before another submission. Resubmitting the old native recording after its ranges have been reclaimed is rejected. Resetting or disposing an unsubmitted list releases its allocation leases after its native recording is discarded.

Recording remains single-threaded per list. Pool reservation and reclamation are synchronized; the application still supplies resource states and ordering between queues. Allocation lifetime tracking does not insert cross-queue waits or barriers.

## Native recording and descriptor tables

Call `commands.RetainAllocation(allocation)` before a native callback references an allocation. Declare other DXSharp object dependencies through `RecordNative` as usual. Keeping only the backing buffer or descriptor heap alive does not reserve a subrange.

`DescriptorAllocation.GetCpuHandle` and `GetGpuHandle` expose checked native handles. Native descriptor writes must obey the same no-overwrite rule; the SDK cannot intercept a write through a raw handle. `GraphicsDevice.CreateShaderResourceView` enforces managed write protection and retains the referenced resource through the allocation lifetime.

For custom root signatures, `SetDescriptorHeaps`, `SetGraphicsDescriptorTable`, and `SetComputeDescriptorTable` retain their allocation arguments. Bind the matching heap before setting a table, and rebind tables after changing heaps. A resource and sampler heap can be bound together. RTV/DSV pools must be created with `shaderVisible: false`.

## Current boundary

This increment provides bounded descriptor ranges, upload slices, managed recording leases, SRV writes, descriptor table binding and shared-pool RGBA8 texture uploads. It does not introduce a placed-resource memory allocator, a readback ring, general texture/mip/DDS loading, or GPU-side cross-queue handoff APIs. It does not pool the older convenience texture overload automatically; consumers opt in by supplying their pools.
