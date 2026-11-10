using DXSharp.Interop;
using Windows.Win32.AI.MachineLearning.DirectML;

namespace DXSharp.MachineLearning;

/// <summary>A compiled Float32 matrix product, optionally followed by a fused ReLU.</summary>
public sealed class TensorGemmOperator : TensorBinaryOperator {
    internal TensorGemmOperator( ComOwner<IDMLCompiledOperator> owner, DirectMlDevice device,
        TensorShape left, TensorShape right, TensorShape output ) : base( owner, device, left, right, output, hasOptionalThirdInput: true ) { }
}
