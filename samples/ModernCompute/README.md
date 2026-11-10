# ModernCompute

This headless sample validates DXC compilation, a compute dispatch, and GPU readback. Optional DirectML paths verify tensor addition and a two-row dense layer with fused ReLU.

```powershell
dotnet run --project samples/ModernCompute -c Debug -- --warp --directml --gemm
```

The checks expect `1234` from the compute shader, `[11, 22, 33, 44]` from Add, and `[0, 9, 4, 0, 9, 0, 4, 2]` from GEMM with ReLU. See the [compute](../../doc/tutorials/03-compute.md) and [DirectML](../../doc/tutorials/04-directml.md) lessons.
