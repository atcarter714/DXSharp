using Windows.Win32;
using Windows.Win32.AI.MachineLearning.DirectML;
using DXSharp.Graphics;
using DXSharp.Interop;

namespace DXSharp.MachineLearning;

/// <summary>A DirectML device with an independently owned reference to its Direct3D device.</summary>
public sealed unsafe class DirectMlDevice : GraphicsObject {
    internal readonly ComOwner<IDMLDevice1> Native;
    internal readonly GraphicsDevice Graphics;
    private DirectMlDevice( ComOwner<IDMLDevice1> owner, GraphicsDevice graphics ) : base( owner ) { Native = owner; Graphics = graphics; }
    internal override ComReference Retain( ) => Native.Retain( );
    /// <summary>Creates a DirectML device from the same adapter as an existing graphics device.</summary>
    public static DirectMlDevice Create( GraphicsDevice graphics, bool enableDebugLayer = false ) {
        ArgumentNullException.ThrowIfNull( graphics );
        Guid id = typeof(IDMLDevice1).GUID;
        Results.Check( PInvoke.DMLCreateDevice1( graphics.Native.Interface,
            enableDebugLayer ? DML_CREATE_DEVICE_FLAGS.DML_CREATE_DEVICE_FLAG_DEBUG : 0,
            DML_FEATURE_LEVEL.DML_FEATURE_LEVEL_1_0, &id, out nint pointer ), "DMLCreateDevice1" );
        var owner = ComOwner<IDMLDevice1>.Adopt( pointer );
        try { return new( owner, graphics.Clone( ) ) { DeviceIdentity = graphics.DeviceIdentity }; }
        catch { owner.Dispose( ); throw; }
    }
    /// <summary>Compiles A + B for three equal, dense Float32 tensors. Broadcasting is deliberately explicit in future operator overloads.</summary>
    public TensorAddOperator CompileAdd( TensorShape shape ) {
        ArgumentNullException.ThrowIfNull( shape );
        fixed ( uint* sizes = shape.Dimensions ) {
            var buffer = Buffer( shape, sizes );
            DML_TENSOR_DESC tensor = new( ) { Type = DML_TENSOR_TYPE.DML_TENSOR_TYPE_BUFFER, Desc = &buffer };
            DML_ELEMENT_WISE_ADD_OPERATOR_DESC addition = new( ) { ATensor = &tensor, BTensor = &tensor, OutputTensor = &tensor };
            DML_OPERATOR_DESC description = new( ) { Type = DML_OPERATOR_TYPE.DML_OPERATOR_ELEMENT_WISE_ADD, Desc = &addition };
            var compiled = CompileOperator( description );
            try { return new( compiled, this, shape ); }
            catch { compiled.Dispose( ); throw; }
        }
    }

    /// <summary>Compiles a row-major M×K by K×N Float32 product with an optional fused ReLU.</summary>
    public TensorGemmOperator CompileGemm( uint rows, uint shared, uint columns, bool relu = false ) {
        var leftShape = new TensorShape( 1, 1, rows, shared );
        var rightShape = new TensorShape( 1, 1, shared, columns );
        var outputShape = new TensorShape( 1, 1, rows, columns );
        fixed ( uint* m = leftShape.Dimensions, k = rightShape.Dimensions, n = outputShape.Dimensions ) {
            var leftBuffer = Buffer( leftShape, m );
            var rightBuffer = Buffer( rightShape, k );
            var outputBuffer = Buffer( outputShape, n );
            DML_TENSOR_DESC left = new( ) { Type = DML_TENSOR_TYPE.DML_TENSOR_TYPE_BUFFER, Desc = &leftBuffer };
            DML_TENSOR_DESC right = new( ) { Type = DML_TENSOR_TYPE.DML_TENSOR_TYPE_BUFFER, Desc = &rightBuffer };
            DML_TENSOR_DESC output = new( ) { Type = DML_TENSOR_TYPE.DML_TENSOR_TYPE_BUFFER, Desc = &outputBuffer };
            DML_ACTIVATION_RELU_OPERATOR_DESC activation = default;
            DML_OPERATOR_DESC reluDescription = new( ) { Type = DML_OPERATOR_TYPE.DML_OPERATOR_ACTIVATION_RELU, Desc = &activation };
            DML_GEMM_OPERATOR_DESC gemm = new( ) {
                ATensor = &left, BTensor = &right, OutputTensor = &output, Alpha = 1,
                FusedActivation = relu ? &reluDescription : null
            };
            DML_OPERATOR_DESC description = new( ) { Type = DML_OPERATOR_TYPE.DML_OPERATOR_GEMM, Desc = &gemm };
            var compiled = CompileOperator( description );
            try { return new( compiled, this, leftShape, rightShape, outputShape ); }
            catch { compiled.Dispose( ); throw; }
        }
    }

    private static DML_BUFFER_TENSOR_DESC Buffer( TensorShape shape, uint* sizes ) => new( ) {
        DataType = DML_TENSOR_DATA_TYPE.DML_TENSOR_DATA_TYPE_FLOAT32, DimensionCount = (uint)shape.Rank,
        Sizes = sizes, TotalTensorSizeInBytes = shape.SizeInBytes
    };

    private ComOwner<IDMLCompiledOperator> CompileOperator( DML_OPERATOR_DESC description ) {
        Guid operatorId = typeof(IDMLOperator).GUID;
        nint pointer = 0;
        Results.Check( Native.Interface.CreateOperator( &description, &operatorId, (void**)&pointer ), $"DirectML.CreateOperator({description.Type})" );
        using var op = ComOwner<IDMLOperator>.Adopt( pointer );
        Guid compiledId = typeof(IDMLCompiledOperator).GUID;
        Results.Check( Native.Interface.CompileOperator( op.Interface, 0, &compiledId, (void**)&pointer ), $"DirectML.CompileOperator({description.Type})" );
        return ComOwner<IDMLCompiledOperator>.Adopt( pointer );
    }

    /// <inheritdoc />
    public override void Dispose( ) { base.Dispose( ); Graphics.Dispose( ); }
}
