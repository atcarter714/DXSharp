# Run a DirectML operator

The [ModernCompute](../../samples/ModernCompute) sample includes a DirectML tensor addition with a deterministic result.

```powershell
dotnet run --project samples/ModernCompute -c Debug -- --warp --directml
```

Expect `DirectML Add = [11, 22, 33, 44]` after the shader readback check.

1. Create a `DirectMlDevice` from the same graphics device that owns the buffers.
2. Compile the Add operator for a `TensorShape(1, 1, 1, 4)`. The compiled operator keeps the underlying DirectML and graphics references it needs.
3. Upload two arrays of four floats. Copy into UAV-capable default buffers and submit the upload. The buffers decay to Common at the submission boundary.
4. Execute the operator with buffers from that device. DXSharp records the transitions from Common to UAV for the dispatch. Copy the output to readback memory and wait before inspecting it.

Read [the implementation](../../samples/ModernCompute/Program.cs) for the full sequence and buffer states. The sample qualifies tensor binding and operator ownership. Use the same numerical-check pattern for a larger model or operator chain, with shape and tolerance derived from the operation.

## Run a small inference layer

```powershell
dotnet run --project samples/ModernCompute -c Debug -- --warp --gemm
```

The GEMM example multiplies two rows of three input features by a 3 × 4 weight matrix and applies a fused ReLU. It checks all eight outputs against a CPU reference, including values clamped to zero. The expected row-major result is `[0, 9, 4, 0, 9, 0, 4, 2]`.

`CompileGemm(2, 3, 4, relu: true)` uses the DirectML tensor shapes `[1, 1, 2, 3]`, `[1, 1, 3, 4]`, and `[1, 1, 2, 4]`. Each shape supplies the byte count used for allocation and binding. The compiled operator retains the device and reuses the same execution path as Add.
