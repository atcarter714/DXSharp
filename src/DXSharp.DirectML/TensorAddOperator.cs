using DXSharp.Interop;
using Windows.Win32.AI.MachineLearning.DirectML;

namespace DXSharp.MachineLearning;

/// <summary>A compiled element-wise Float32 addition operator.</summary>
public sealed class TensorAddOperator : TensorBinaryOperator {
    /// <summary>The shape shared by both inputs and the output.</summary>
    public TensorShape Shape => OutputShape;

    internal TensorAddOperator( ComOwner<IDMLCompiledOperator> owner, DirectMlDevice device, TensorShape shape )
        : base( owner, device, shape, shape, shape ) { }
}
