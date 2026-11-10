using DXSharp.Graphics;
using DXSharp.MachineLearning;
using DXSharp.Shaders;

#if DEBUG
const bool debugLayer = true;
#else
const bool debugLayer = false;
#endif

using var device = GraphicsDevice.Create( new( ) {
    UseWarp = args.Contains( "--warp" ), UseAgility = !args.Contains( "--system-runtime" ), EnableDebugLayer = debugLayer && !args.Contains( "--no-debug" )
} );
Console.WriteLine( device.AdapterName );
try {
    ShaderRoundTrip( args.Contains( "--latest-shader-model" ) );
    if ( args.Contains( "--directml" ) ) DirectMlRoundTrip( );
    if ( args.Contains( "--gemm" ) ) InferenceRoundTrip( );
    var diagnostics = device.GetDiagnostics( );
    foreach ( string message in diagnostics.Messages ) Console.WriteLine( message );
    if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any( x => x.Contains( "SEVERITY_ERROR" ) || x.Contains( "SEVERITY_CORRUPTION" ) ) )
        throw new InvalidOperationException( "Graphics validation failed." );
    Console.WriteLine( "PASS: requested numerical qualifications completed." ); return 0;
} catch ( Exception error ) {
    Console.Error.WriteLine( error );
    foreach ( string message in device.GetDiagnostics( ).Messages ) Console.Error.WriteLine( message );
    return 1;
}

void ShaderRoundTrip( bool latestShaderModel ) {
    const string source = "RWStructuredBuffer<uint> values : register(u0); [numthreads(1,1,1)] void Main() { values[0] = 1234; }";
    using var compiler = new ShaderCompiler( );
    string profile = "cs_6_0";
    if ( latestShaderModel ) {
        Version retailCompilerCeiling = new( 6, 9 );
        Version supported = device.GetHighestShaderModel( );
        if ( supported < new Version( 6, 0 ) )
            throw new NotSupportedException( $"This sample requires Shader Model 6.0. The device reports {supported}." );
        Version selected = supported < retailCompilerCeiling ? supported : retailCompilerCeiling;
        profile = $"cs_{selected.Major}_{selected.Minor}";
        if ( selected != retailCompilerCeiling ) {
            // Compile the retail ceiling even when this adapter cannot execute it.
            _ = compiler.Compile( source, "Main", "cs_6_9" ).RequireSuccess( );
        }
        Console.WriteLine( $"Device Shader Model {supported}. Qualifying DXC 6.9 and dispatching {profile}." );
    }
    var shader = compiler.Compile( source, "Main", profile ).RequireSuccess( );
    using var queue = device.CreateCommandQueue( QueueKind.Compute ); using var allocator = device.CreateCommandAllocator( QueueKind.Compute );
    using var commands = device.CreateCommandList( allocator ); using var root = device.CreateRootSignature( 1 );
    using var pipeline = device.CreateComputePipeline( root, shader );
    using var output = device.CreateBuffer( 4, unorderedAccess: true );
    using var readback = device.CreateBuffer( 4, BufferMemory.Readback );
    commands.Transition( output, ResourceState.Common, ResourceState.UnorderedAccess );
    commands.SetComputePipeline( pipeline ); commands.SetComputeBuffer( 0, output ); commands.Dispatch( 1 );
    commands.UavBarrier( output ); commands.Transition( output, ResourceState.UnorderedAccess, ResourceState.CopySource );
    commands.CopyBuffer( output, readback, 4 ); commands.Close( );
    root.Dispose( ); pipeline.Dispose( ); output.Dispose( ); GC.Collect( ); GC.WaitForPendingFinalizers( );
    queue.Wait( queue.Submit( commands ) );
    Span<uint> value = stackalloc uint[1]; readback.Read( value );
    if ( value[0] != 1234 ) throw new InvalidOperationException( $"Shader readback mismatch: {value[0]}." );
    Console.WriteLine( "PASS: DXC -> compute -> readback = 1234, after early owner disposal and GC." );
}
void DirectMlRoundTrip( ) {
    using var ml = DirectMlDevice.Create( device );
    using var add = ml.CompileAdd( new TensorShape( 1, 1, 1, 4 ) );
    ml.Dispose( );
    var values = ExecuteOperator( add, [1, 2, 3, 4], [10, 20, 30, 40] );
    if ( !values.AsSpan( ).SequenceEqual( [11f, 22f, 33f, 44f] ) )
        throw new InvalidOperationException( $"DirectML Add mismatch: {string.Join( ", ", values )}." );
    Console.WriteLine( "PASS: DirectML Add = [11, 22, 33, 44]." );
}

void InferenceRoundTrip( ) {
    const int rows = 2, shared = 3, columns = 4;
    ReadOnlySpan<float> features = [2, -1, 3, -2, 4, 1];
    ReadOnlySpan<float> weights = [1, 2, -1, 0, 3, -2, 0, 1, -1, 1, 2, -2];
    using var ml = DirectMlDevice.Create( device );
    using var layer = ml.CompileGemm( rows, shared, columns, relu: true );
    ml.Dispose( );
    var values = ExecuteOperator( layer, features, weights );
    for ( int row = 0; row < rows; ++row ) {
        for ( int column = 0; column < columns; ++column ) {
            float expected = 0;
            for ( int k = 0; k < shared; ++k )
                expected += features[row * shared + k] * weights[k * columns + column];
            expected = Math.Max( 0, expected );
            float actual = values[row * columns + column];
            if ( !float.IsFinite( actual ) || MathF.Abs( actual - expected ) > 1e-5f )
                throw new InvalidOperationException( $"DirectML GEMM+ReLU [{row}, {column}]: expected {expected}, got {actual}." );
        }
    }
    Console.WriteLine( $"PASS: DirectML GEMM+ReLU = [{string.Join( ", ", values )}]." );
}

float[] ExecuteOperator( TensorBinaryOperator operation, ReadOnlySpan<float> leftValues, ReadOnlySpan<float> rightValues ) {
    using var queue = device.CreateCommandQueue( );
    using var allocator = device.CreateCommandAllocator( );
    using var commands = device.CreateCommandList( allocator );
    ulong leftSize = operation.LeftShape.SizeInBytes, rightSize = operation.RightShape.SizeInBytes, outputSize = operation.OutputShape.SizeInBytes;
    if ( (ulong)leftValues.Length != operation.LeftShape.ElementCount || (ulong)rightValues.Length != operation.RightShape.ElementCount )
        throw new ArgumentException( "Input data must match the compiled tensor shapes." );
    using var left = device.CreateBuffer( leftSize, unorderedAccess: true );
    using var right = device.CreateBuffer( rightSize, unorderedAccess: true );
    using var output = device.CreateBuffer( outputSize, unorderedAccess: true );
    using var upload = device.CreateBuffer( checked(leftSize + rightSize), BufferMemory.Upload );
    using var readback = device.CreateBuffer( outputSize, BufferMemory.Readback );
    upload.Write( leftValues );
    upload.Write( rightValues, leftSize );
    commands.Transition( left, ResourceState.Common, ResourceState.CopyDestination );
    commands.Transition( right, ResourceState.Common, ResourceState.CopyDestination );
    commands.CopyBuffer( upload, left, leftSize );
    commands.CopyBuffer( upload, right, rightSize, sourceOffset: leftSize );
    commands.Close( );
    upload.Dispose( );
    GC.Collect( );
    queue.Wait( queue.Submit( commands ) );

    operation.Execute( queue, left, right, output );
    allocator.Reset( );
    commands.Reset( allocator );
    commands.Transition( output, ResourceState.Common, ResourceState.CopySource );
    commands.CopyBuffer( output, readback, outputSize );
    commands.Close( );
    queue.Wait( queue.Submit( commands ) );
    var values = new float[checked((int)operation.OutputShape.ElementCount)];
    readback.Read<float>( values );
    return values;
}
