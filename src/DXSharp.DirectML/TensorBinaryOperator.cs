using System.Runtime.InteropServices;
using Windows.Win32.AI.MachineLearning.DirectML;
using Windows.Win32.Graphics.Direct3D12;
using DXSharp.Graphics;
using DXSharp.Interop;

namespace DXSharp.MachineLearning;

/// <summary>A compiled DirectML operator with two dense Float32 inputs.</summary>
/// <remarks>Execute is synchronous and serializes this operator. Each execution owns its descriptor,
/// temporary and persistent allocations, and initializes persistent data before dispatch.</remarks>
public abstract unsafe class TensorBinaryOperator : GraphicsObject {
    private readonly ComOwner<IDMLCompiledOperator> _native;
    private readonly ComOwner<IDMLDevice1> _device;
    private readonly GraphicsDevice _graphics;
    private readonly object _gate = new( );
    private readonly bool _hasOptionalThirdInput;
    /// <summary>Shape of the first input.</summary>
    public TensorShape LeftShape { get; }
    /// <summary>Shape of the second input.</summary>
    public TensorShape RightShape { get; }
    /// <summary>Shape of the output.</summary>
    public TensorShape OutputShape { get; }
    internal TensorBinaryOperator( ComOwner<IDMLCompiledOperator> owner, DirectMlDevice device,
        TensorShape left, TensorShape right, TensorShape output, bool hasOptionalThirdInput = false ) : base( owner ) {
        _native = owner;
        _device = device.Native.Clone( );
        try { _graphics = device.Graphics.Clone( ); }
        catch { _device.Dispose( ); throw; }
        LeftShape = left; RightShape = right; OutputShape = output;
        _hasOptionalThirdInput = hasOptionalThirdInput;
        DeviceIdentity = device.DeviceIdentity;
    }
    internal override ComReference Retain( ) => _native.Retain( );

    /// <summary>Initializes and executes the operator on a direct or compute queue from the same device.
    /// The default-heap UAV buffers must be ready for use on the queue. Each recording begins from Common after buffer decay.</summary>
    public void Execute( CommandQueue queue, GpuResource left, GpuResource right, GpuResource output ) {
        lock ( _gate ) {
            RequireSameDevice( queue ); RequireSameDevice( left ); RequireSameDevice( right ); RequireSameDevice( output );
            if ( queue.Kind == QueueKind.Copy ) throw new ArgumentException( "DirectML requires a direct or compute queue.", nameof(queue) );
            ValidateBuffer( left, LeftShape );
            ValidateBuffer( right, RightShape );
            ValidateBuffer( output, OutputShape );
            if ( ReferenceEquals( output, left ) || ReferenceEquals( output, right ) ) throw new ArgumentException( "The output must not alias an input.", nameof(output) );
            Guid id = typeof(IDMLOperatorInitializer).GUID;
            Results.Check( _device.Interface.CreateOperatorInitializer( 1, [_native.Interface], &id, out nint pointer ), "DirectML.CreateOperatorInitializer" );
            using var initializer = ComOwner<IDMLOperatorInitializer>.Adopt( pointer );
            DML_BINDING_PROPERTIES init = initializer.Interface.GetBindingProperties( ), execute = _native.Interface.GetBindingProperties( );
            uint descriptors = Math.Max( 1, Math.Max( init.RequiredDescriptorCount, execute.RequiredDescriptorCount ) );
            using var heap = CreateHeap( descriptors );
            using var temporary = CreateScratch( Math.Max( init.TemporaryResourceSize, execute.TemporaryResourceSize ) );
            using var persistent = CreateScratch( execute.PersistentResourceSize );
            id = typeof(IDMLCommandRecorder).GUID;
            Results.Check( _device.Interface.CreateCommandRecorder( &id, out pointer ), "DirectML.CreateCommandRecorder" );
            using var recorder = ComOwner<IDMLCommandRecorder>.Adopt( pointer );
            using var allocator = _graphics.CreateCommandAllocator( queue.Kind );
            using var commands = _graphics.CreateCommandList( allocator );
            using var initLease = initializer.Borrow( );
            DML_BINDING_TABLE_DESC_unmanaged description = new( ) {
                Dispatchable = (IDMLDispatchable_unmanaged*)initLease.Pointer,
                CPUDescriptorHandle = heap.Native.Interface.GetCPUDescriptorHandleForHeapStart( ),
                GPUDescriptorHandle = heap.Native.Interface.GetGPUDescriptorHandleForHeapStart( ), SizeInDescriptors = descriptors
            };
            id = typeof(IDMLBindingTable).GUID;
            Results.Check( _device.Interface.CreateBindingTable( &description, &id, out pointer ), "DirectML.CreateBindingTable" );
            using var table = ComOwner<IDMLBindingTable>.Adopt( pointer );
            BindResource( table.Interface, temporary, init.TemporaryResourceSize, 0 );
            BindResource( table.Interface, persistent, execute.PersistentResourceSize, 1 );
            PrepareDispatch( commands, heap, temporary, init.TemporaryResourceSize, persistent );
            commands.RetainNative( initializer, initializer.Retain( ) ); commands.RetainNative( table, table.Retain( ) );
            commands.RetainNative( recorder, recorder.Retain( ) ); commands.RetainDependency( this );
            recorder.Interface.RecordDispatch( commands.Native.Interface, initializer.Interface, table.Interface );
            if ( persistent is not null ) commands.UavBarrier( persistent );
            if ( init.TemporaryResourceSize != 0 ) commands.UavBarrier( temporary! );
            commands.Close( ); queue.Wait( queue.Submit( commands ) );

            allocator.Reset( ); commands.Reset( allocator );
            using var executeLease = _native.Borrow( );
            description.Dispatchable = (IDMLDispatchable_unmanaged*)executeLease.Pointer;
            Results.Check( table.Interface.Reset( &description ), "DirectML.BindingTable.Reset" );
            BindResource( table.Interface, temporary, execute.TemporaryResourceSize, 0 );
            BindResource( table.Interface, persistent, execute.PersistentResourceSize, 2 );
            using var a = left.Native.Borrow( ); using var b = right.Native.Borrow( ); using var result = output.Native.Borrow( );
            DML_BUFFER_BINDING* buffers = stackalloc DML_BUFFER_BINDING[3];
            buffers[0] = Buffer( a.Pointer, LeftShape.SizeInBytes );
            buffers[1] = Buffer( b.Pointer, RightShape.SizeInBytes );
            buffers[2] = Buffer( result.Pointer, OutputShape.SizeInBytes );
            DML_BINDING_DESC* inputs = stackalloc DML_BINDING_DESC[3] {
                Binding( &buffers[0] ), Binding( &buffers[1] ), default
            };
            DML_BINDING_DESC target = Binding( &buffers[2] );
            table.Interface.BindInputs( _hasOptionalThirdInput ? 3u : 2u, inputs );
            table.Interface.BindOutputs( 1, &target );
            PrepareDispatch( commands, heap, temporary, execute.TemporaryResourceSize, persistent );
            commands.Transition( left, ResourceState.Common, ResourceState.UnorderedAccess );
            if ( !ReferenceEquals( left, right ) ) commands.Transition( right, ResourceState.Common, ResourceState.UnorderedAccess );
            commands.Transition( output, ResourceState.Common, ResourceState.UnorderedAccess );
            commands.RetainDependency( this ); commands.RetainDependency( left ); commands.RetainDependency( right ); commands.RetainDependency( output );
            commands.RetainNative( table, table.Retain( ) ); commands.RetainNative( recorder, recorder.Retain( ) );
            recorder.Interface.RecordDispatch( commands.Native.Interface, _native.Interface, table.Interface );
            commands.UavBarrier( output ); commands.Close( ); queue.Wait( queue.Submit( commands ) );
        }
    }
    private static void ValidateBuffer( GpuResource buffer, TensorShape shape ) {
        buffer.ValidateRange( 0, shape.SizeInBytes );
        if ( buffer.Memory != BufferMemory.Default || !buffer.SupportsUnorderedAccess )
            throw new ArgumentException( "DirectML bindings require default-heap UAV buffers." );
    }

    private DescriptorHeap CreateHeap( uint capacity ) {
        D3D12_DESCRIPTOR_HEAP_DESC description = new( ) { Type = D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV,
            Flags = D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE, NumDescriptors = capacity };
        Guid id = typeof(ID3D12DescriptorHeap).GUID;
        Results.Check( _graphics.Native.Interface.CreateDescriptorHeap( &description, &id, out nint pointer ), "DirectML.CreateDescriptorHeap" );
        return new( ComOwner<ID3D12DescriptorHeap>.Adopt( pointer ), capacity,
            _graphics.Native.Interface.GetDescriptorHandleIncrementSize( description.Type ) ) { DeviceIdentity = DeviceIdentity };
    }
    private GpuResource? CreateScratch( ulong size ) => size == 0 ? null : _graphics.CreateBuffer( size, unorderedAccess: true );
    private static void PrepareDispatch( GraphicsCommandList commands, DescriptorHeap heap, GpuResource? temporary,
        ulong temporarySize, GpuResource? persistent ) {
        commands.RetainDependency( heap );
        if ( temporarySize != 0 ) commands.Transition( temporary!, ResourceState.Common, ResourceState.UnorderedAccess );
        if ( persistent is not null ) commands.Transition( persistent, ResourceState.Common, ResourceState.UnorderedAccess );
        commands.Native.Interface.SetDescriptorHeaps( 1, [heap.Native.Interface] );
    }
    private static void BindResource( IDMLBindingTable table, GpuResource? resource, ulong size, int kind ) {
        if ( size == 0 ) return;
        using var lease = resource!.Native.Borrow( );
        DML_BUFFER_BINDING buffer = Buffer( lease.Pointer, size ); DML_BINDING_DESC binding = Binding( &buffer );
        if ( kind == 0 ) table.BindTemporaryResource( &binding );
        else if ( kind == 1 ) table.BindOutputs( 1, &binding );
        else table.BindPersistentResource( &binding );
    }
    private static DML_BINDING_DESC Binding( DML_BUFFER_BINDING* buffer ) => new( ) { Type = DML_BINDING_TYPE.DML_BINDING_TYPE_BUFFER, Desc = buffer };
    private static DML_BUFFER_BINDING Buffer( nint pointer, ulong size ) => new( ) {
        Buffer = (ID3D12Resource_unmanaged*)pointer, SizeInBytes = size
    };
    /// <inheritdoc />
    public override void Dispose( ) { lock ( _gate ) { base.Dispose( ); _device.Dispose( ); _graphics.Dispose( ); } }
}
