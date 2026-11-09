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
            DML_BUFFER_TENSOR_DESC buffer = new( ) {
                DataType = DML_TENSOR_DATA_TYPE.DML_TENSOR_DATA_TYPE_FLOAT32,
                DimensionCount = (uint)shape.Rank, Sizes = sizes, TotalTensorSizeInBytes = shape.SizeInBytes
            };
            DML_TENSOR_DESC tensor = new( ) { Type = DML_TENSOR_TYPE.DML_TENSOR_TYPE_BUFFER, Desc = &buffer };
            DML_ELEMENT_WISE_ADD_OPERATOR_DESC addition = new( ) { ATensor = &tensor, BTensor = &tensor, OutputTensor = &tensor };
            DML_OPERATOR_DESC description = new( ) { Type = DML_OPERATOR_TYPE.DML_OPERATOR_ELEMENT_WISE_ADD, Desc = &addition };
            Guid operatorId = typeof(IDMLOperator).GUID;
            nint pointer = 0;
            Results.Check( Native.Interface.CreateOperator( &description, &operatorId, (void**)&pointer ), "DirectML.CreateOperator(Add)" );
            using var op = ComOwner<IDMLOperator>.Adopt( pointer );
            Guid compiledId = typeof(IDMLCompiledOperator).GUID;
            Results.Check( Native.Interface.CompileOperator( op.Interface, 0, &compiledId, (void**)&pointer ), "DirectML.CompileOperator(Add)" );
            var compiled = ComOwner<IDMLCompiledOperator>.Adopt( pointer );
            try { return new( compiled, Native.Clone( ), Graphics.Clone( ), shape ) { DeviceIdentity = DeviceIdentity }; }
            catch { compiled.Dispose( ); throw; }
        }
    }
    /// <inheritdoc />
    public override void Dispose( ) { base.Dispose( ); Graphics.Dispose( ); }
}
