# Readback rings and texture subresources

`ReadbackRing` shares the bounded range allocator and recording leases used by `UploadRing`. Its default placement alignment is 512 bytes; choose a smaller power-of-two alignment explicitly for buffer-only transfers. Allocation owners retain CPU results after GPU completion, and recorded uses retain their ranges after early owner or ring disposal.

```csharp
using var upload = uploads.Allocate( 256 );
using var result = readbacks.Allocate( 256 );
upload.Write<uint>( [714, 1234] );
commands.CopyBuffer( upload, result, 8 );
commands.Close( );
queue.Wait( queue.Submit( commands ) );
Span<uint> values = stackalloc uint[2];
result.Read<uint>( values );
```

Reads throw while a declared use is still recorded, closed but unsubmitted, or pending GPU completion. Unwritten contents are undefined. The helper cannot infer undeclared native writes; native callbacks must retain the allocation. Application code still controls queue ordering and resource states.

## Texture layout and transfers

`Texture2DDescription` specifies a typed DXGI color format, explicit mip count and array size. Setting `Cube` requires square faces and a multiple of six array slices; a size of 12 represents two cubes. The device queries format support and plane count. Depth, typeless, multiplanar and multisampled resources are outside this sampling/transfer contract.

```csharp
using var texture = device.CreateTexture2D(
    new( width, height, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, MipLevels: 3 ), descriptors );
commands.UploadTexture( texture, uploads, mipData );
commands.Transition( texture, ResourceState.CopyDestination, ResourceState.CopySource );
using var result = commands.ReadbackTexture( texture, readbacks );
commands.Close( );
queue.Wait( queue.Submit( commands ) );
byte[] baseMip = new byte[result.GetDataSize( 0 )];
result.Read( 0, baseMip );
```

`TextureSubresourceData` supplies `ReadOnlyMemory<byte>` plus its CPU row stride. Input order is `mip + arraySlice * MipLevels`, including cube faces. Data is copied synchronously into the ring during recording. The SDK asks `GetCopyableFootprints` for placement, padded row stride, unpadded row byte count and row count. Source row padding is skipped; native upload padding is supplied automatically. For BC formats, rows contain compressed blocks, not decoded pixels. Readback removes native padding and returns the original packed block bytes.

`UploadTexture` accepts a first-subresource index and a contiguous input range. `ReadbackTexture` accepts a first index and count; zero count captures all remaining subresources. Relative index zero in the returned result corresponds to its `FirstSubresource`. Source textures must be in `CopySource`, upload destinations in `CopyDestination`. Transition helpers affect all subresources; no automatic resource-state tracker or GPU-side cross-queue handoff is introduced.

Bind the texture through `SetGraphicsTexture(texture, parameterIndex)` after setting the matching pipeline/root signature. The root table and shader must agree with the SRV dimension: Texture2D, Texture2DArray, TextureCube or TextureCubeArray. Descriptor and resource leases survive public owner disposal through submission completion.

`HelloTexture` uploads two three-level mip chains into shared pools and explicitly selects mip levels in HLSL. The Windows fixture performs byte-exact padded-row, mip-array, cube-array and BC1 round trips, modifies only the last selected subresource, verifies the untouched subresources, and checks early disposal and reads blocked by a GPU fence.

This is a transfer and sampling layer. DDS parsing, automatic mip generation, render/UAV texture descriptions, depth and video plane transfers remain separate work.

Reference: [GetCopyableFootprints](https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-getcopyablefootprints).
