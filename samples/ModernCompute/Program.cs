using DXSharp.Graphics;
using DXSharp.MachineLearning;
using DXSharp.Shaders;

using var device = GraphicsDevice.Create( new( ) {
    UseWarp = args.Contains( "--warp" ), UseAgility = !args.Contains( "--system-runtime" ), EnableDebugLayer = !args.Contains( "--no-debug" )
} );
Console.WriteLine( device.AdapterName );
try {
    ShaderRoundTrip( );
    if ( args.Contains( "--directml" ) ) DirectMlRoundTrip( );
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

void ShaderRoundTrip( ) {
    using var compiler = new ShaderCompiler( );
    var shader = compiler.Compile( "RWStructuredBuffer<uint> values : register(u0); [numthreads(1,1,1)] void Main() { values[0] = 1234; }", "Main", "cs_6_0" ).RequireSuccess( );
    using var queue = device.CreateCommandQueue( QueueKind.Compute ); using var allocator = device.CreateCommandAllocator( QueueKind.Compute );
    using var commands = device.CreateCommandList( allocator ); using var root = device.CreateRootSignature( 1 );
    using var pipeline = device.CreateComputePipeline( root, shader );
    using var output = device.CreateBuffer( 4, initialState: ResourceState.UnorderedAccess, unorderedAccess: true );
    using var readback = device.CreateBuffer( 4, BufferMemory.Readback );
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
    using var ml = DirectMlDevice.Create( device ); using var add = ml.CompileAdd( new TensorShape( 1, 1, 1, 4 ) );
    using var queue = device.CreateCommandQueue( ); using var allocator = device.CreateCommandAllocator( );
    using var commands = device.CreateCommandList( allocator );
    using var left = device.CreateBuffer( 16, initialState: ResourceState.CopyDestination, unorderedAccess: true );
    using var right = device.CreateBuffer( 16, initialState: ResourceState.CopyDestination, unorderedAccess: true );
    using var output = device.CreateBuffer( 16, initialState: ResourceState.UnorderedAccess, unorderedAccess: true );
    using var upload = device.CreateBuffer( 32, BufferMemory.Upload );
    using var readback = device.CreateBuffer( 16, BufferMemory.Readback );
    upload.Write<float>( [1, 2, 3, 4, 10, 20, 30, 40] );
    commands.CopyBuffer( upload, left, 16 ); commands.CopyBuffer( upload, right, 16, sourceOffset: 16 );
    commands.Transition( left, ResourceState.CopyDestination, ResourceState.UnorderedAccess );
    commands.Transition( right, ResourceState.CopyDestination, ResourceState.UnorderedAccess );
    commands.Close( ); upload.Dispose( ); GC.Collect( ); queue.Wait( queue.Submit( commands ) );
    // The operator owns its DirectML and graphics references independently of this facade.
    ml.Dispose( ); add.Execute( queue, left, right, output );
    allocator.Reset( ); commands.Reset( allocator );
    commands.Transition( output, ResourceState.UnorderedAccess, ResourceState.CopySource );
    commands.CopyBuffer( output, readback, 16 ); commands.Close( ); queue.Wait( queue.Submit( commands ) );
    Span<float> values = stackalloc float[4]; readback.Read( values );
    if ( !values.SequenceEqual( [11f, 22f, 33f, 44f] ) ) throw new InvalidOperationException( $"DirectML mismatch: {string.Join( ", ", values.ToArray( ) )}." );
    Console.WriteLine( "PASS: DirectML Add = [11, 22, 33, 44]." );
}
