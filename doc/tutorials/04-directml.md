# Run a DirectML operator

The [ModernCompute](../../samples/ModernCompute) sample includes a DirectML tensor addition with a deterministic result.

```powershell
dotnet run --project samples/ModernCompute -c Debug -- --warp --directml
```

Expect `DirectML Add = [11, 22, 33, 44]` after the shader readback check.

1. Create a `DirectMlDevice` from the same graphics device that owns the buffers.
2. Compile the Add operator for a `TensorShape(1, 1, 1, 4)`. The compiled operator keeps the underlying DirectML and graphics references it needs.
3. Upload two arrays of four floats. Prepare UAV-capable default buffers and submit the upload.
4. Execute the operator with buffers from that device. Copy the output to readback memory and wait before inspecting it.

Read [the implementation](../../samples/ModernCompute/Program.cs) for the full sequence and buffer states. The sample qualifies tensor binding and operator ownership. Use the same numerical-check pattern for a larger model or operator chain, with shape and tolerance derived from the operation.
