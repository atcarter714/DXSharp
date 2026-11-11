# Upload and draw an indexed mesh

Run the indexed cube on hardware or WARP.

```powershell
dotnet run --project samples/LitCube -c Debug
dotnet run --project samples/LitCube -c Debug -- --warp --frames=4
```

The cube has 24 vertices, with a distinct normal and color for each face, and 36 UInt16 indices. Its vertex shader reads the input assembler's attributes. The sample keeps its world and view-projection matrices in root constants.

## Match the shader input to memory

Use an unmanaged vertex struct with an explicit, stable layout. Field offsets in the pipeline must match that layout, including padding.

```csharp
[StructLayout(LayoutKind.Sequential)]
readonly record struct Vertex(Vector3 Position, Vector3 Normal, Vector3 Color);
```

The corresponding input elements use offsets 0, 12, and 24, and the buffer stride is 36 bytes.

```csharp
using var root = device.CreateGraphicsConstantsRootSignature(32);
using var pipeline = device.CreateGraphicsPipeline(root, vertexShader, pixelShader, [
    new("POSITION", VertexFormat.Float3, 0),
    new("NORMAL", VertexFormat.Float3, 12),
    new("COLOR", VertexFormat.Float3, 24)
], depthEnabled: true);
```

The semantic names and indices must match the compiled vertex shader. Every attribute has an explicit input slot and byte offset. The pipeline copies the semantic names and native descriptors during creation.

## Batch the upload

`CreateMesh<TVertex>` accepts UInt16 or UInt32 indices. It checks indices against the supplied vertex count, creates buffers in GPU working memory, and records copies and transitions into an open direct command list. Vertex and index data share one staging allocation per mesh.

```csharp
using var queue = device.CreateCommandQueue();
using var allocator = device.CreateCommandAllocator();
using var uploads = device.CreateCommandList(allocator);
using var mesh = device.CreateMesh<Vertex>(uploads, vertices, indices);
uploads.Close();
ulong uploaded = queue.Submit(uploads);
```

Multiple mesh creations can share that upload recording. To draw on the same direct queue, submit the upload recording before the draw recording. Queue order supplies that dependency without a CPU wait between the submissions. Call `queue.Wait(uploaded)` if the CPU needs confirmed completion. Synchronize explicitly before consuming the data from another queue.

The command list retains staging references even though the public staging owner is disposed during mesh creation. A failed recording can be discarded through command-list disposal or reset when allowed by its submission state.

## Record draws

```csharp
frame.Commands.SetGraphicsPipeline(pipeline);
frame.Commands.SetGraphicsConstants(MemoryMarshal.Cast<Matrix4x4, uint>(matrices));
frame.Commands.DrawMesh(mesh);
```

`DrawMesh` binds the mesh's vertex stream at slot zero and its index buffer, then draws all indices. For subranges or shared buffers, bind views with `SetVertexBuffer` and `SetIndexBuffer` and issue `DrawIndexed` with the required index range, base vertex, and starting instance. The caller validates these draw ranges and supplies resource states appropriate to any later buffer edits.

To add an instance stream, assign its elements another slot and a positive `InstanceStepRate`. Bind that stream with `SetVertexBuffer`, then supply the instance count and starting instance to `DrawMesh`. A step rate of one advances once per instance. Larger rates repeat each stream element for that many instances.

## Check ownership and output

Dispose a public mesh after its final recording. Each recording holds independent native references through fence retirement. Reusing an upload allocation, editing buffer data, and reusing an allocator require synchronization with the GPU work that uses them.

The Windows interop qualification renders to an offscreen RGBA target and checks interior and exterior pixels after readback. It exercises UInt16 and UInt32 indices, an index above 65,535, separate instance streams, offset views, starting instances, and step rates one and two. Upload and draw recordings share a queue, and public mesh and pipeline owners are disposed before either submission. The fixture runs under both JIT and NativeAOT in Windows CI.
