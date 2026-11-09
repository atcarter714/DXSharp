using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Foundation;
using DXSharp.Interop;

namespace DXSharp.Graphics;

/// <summary>A command-memory allocator with explicit pending-submission tracking.</summary>
public sealed class CommandAllocator : GraphicsObject {
    internal readonly ComOwner<ID3D12CommandAllocator> Native;
    internal int Pending;
    /// <summary>The command-list type this allocator serves.</summary>
    public QueueKind Kind { get; }
    internal CommandAllocator( ComOwner<ID3D12CommandAllocator> owner, QueueKind kind ) : base( owner ) { Native = owner; Kind = kind; }
    internal override ComReference Retain( ) => Native.Retain( );
    /// <summary>Reclaims command memory after all associated submissions complete.</summary>
    public void Reset( ) {
        if ( Volatile.Read( ref Pending ) != 0 ) throw new InvalidOperationException( "The allocator still has GPU work pending." );
        Results.Check( Native.Interface.Reset( ), "CommandAllocator.Reset" );
    }
}

/// <summary>A generated-COM command list. Recording is single-threaded per list; independent lists share no scratch state.</summary>
public sealed unsafe class GraphicsCommandList : GraphicsObject {
    internal readonly ComOwner<ID3D12GraphicsCommandList> Native;
    internal CommandAllocator Allocator;
    internal QueueKind Kind => Allocator.Kind;
    private Dictionary<object, ComReference> _retained = new( ReferenceEqualityComparer.Instance );
    private readonly D3D12_RESOURCE_BARRIER[] _barrier = new D3D12_RESOURCE_BARRIER[1];
    private readonly float[] _color = new float[4];
    private int _state; // 0 recording, 1 closed, 2 submitted; reset becomes legal after retirement.
    internal GraphicsCommandList( ComOwner<ID3D12GraphicsCommandList> owner, CommandAllocator allocator ) : base( owner ) {
        Native = owner; Allocator = allocator; DeviceIdentity = allocator.DeviceIdentity; Track( allocator );
    }
    internal override ComReference Retain( ) => Native.Retain( );
    private void Recording( ) {
        ObjectDisposedException.ThrowIf( IsDisposed, this );
        if ( _state != 0 ) throw new InvalidOperationException( "The command list is not recording." );
    }
    private void Track( GraphicsObject value ) { RequireSameDevice( value ); if ( !_retained.ContainsKey( value ) ) _retained.Add( value, value.Retain( ) ); }

    internal void RetainDependency( GraphicsObject value ) { Recording( ); Track( value ); }
    internal void RetainNative( object key, ComReference lease ) {
        Recording( ); if ( !_retained.TryAdd( key, lease ) ) lease.Dispose( );
    }

    /// <summary>Closes the current recording.</summary>
    public void Close( ) { Recording( ); Results.Check( Native.Interface.Close( ), "CommandList.Close" ); _state = 1; }
    /// <summary>Starts a new recording after its previous submission completes. Reset the allocator separately.</summary>
    public void Reset( CommandAllocator allocator ) {
        RequireSameDevice( allocator );
        if ( _state == 2 ) throw new InvalidOperationException( "The command list still has GPU work pending." );
        if ( allocator.Kind != Kind ) throw new ArgumentException( "The allocator kind does not match the command list." );
        Results.Check( Native.Interface.Reset( allocator.Native.Interface, null! ), "CommandList.Reset" );
        ReleaseRecording( ); Allocator = allocator; Track( allocator ); _state = 0;
    }
    /// <summary>Records a transition while retaining the native resource until submission retirement.</summary>
    public void Transition( GpuResource resource, ResourceState before, ResourceState after ) {
        Recording( ); Track( resource );
        using var lease = resource.Native.Borrow( );
        _barrier[0] = default;
        _barrier[0].Type = D3D12_RESOURCE_BARRIER_TYPE.D3D12_RESOURCE_BARRIER_TYPE_TRANSITION;
        _barrier[0].Anonymous.Transition = new( ) {
            pResource = (ID3D12Resource_unmanaged*)lease.Pointer,
            StateBefore = (D3D12_RESOURCE_STATES)before, StateAfter = (D3D12_RESOURCE_STATES)after, Subresource = uint.MaxValue
        };
        Native.Interface.ResourceBarrier( 1, _barrier );
    }
    /// <summary>Orders dependent UAV operations on a resource.</summary>
    public void UavBarrier( GpuResource resource ) {
        Recording( ); Track( resource ); using var lease = resource.Native.Borrow( );
        _barrier[0] = default; _barrier[0].Type = D3D12_RESOURCE_BARRIER_TYPE.D3D12_RESOURCE_BARRIER_TYPE_UAV;
        _barrier[0].Anonymous.UAV.pResource = (ID3D12Resource_unmanaged*)lease.Pointer;
        Native.Interface.ResourceBarrier( 1, _barrier );
    }
    /// <summary>Copies a bounded buffer region. Supply appropriate source/destination states.</summary>
    public void CopyBuffer( GpuResource source, GpuResource destination, ulong size, ulong sourceOffset = 0, ulong destinationOffset = 0 ) {
        Recording( ); source.ValidateRange( sourceOffset, size ); destination.ValidateRange( destinationOffset, size );
        Track( source ); Track( destination );
        Native.Interface.CopyBufferRegion( destination.Native.Interface, destinationOffset, source.Native.Interface, sourceOffset, size );
    }
    internal void CopyTextureFromUpload( GpuResource upload, GpuResource texture, D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint ) {
        Recording( );
        Track( upload );
        Track( texture );
        using var uploadLease = upload.Native.Borrow( );
        using var textureLease = texture.Native.Borrow( );
        D3D12_TEXTURE_COPY_LOCATION source = new( ) {
            pResource = (ID3D12Resource_unmanaged*)uploadLease.Pointer,
            Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_PLACED_FOOTPRINT
        };
        source.Anonymous.PlacedFootprint = footprint;
        D3D12_TEXTURE_COPY_LOCATION target = new( ) {
            pResource = (ID3D12Resource_unmanaged*)textureLease.Pointer,
            Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_SUBRESOURCE_INDEX
        };
        Native.Interface.CopyTextureRegion( target, 0, 0, 0, source, null );
    }

    /// <summary>Binds t0 for a graphics pipeline created with a texture root signature.
    /// This replaces the current shader-visible CBV/SRV/UAV heap.</summary>
    public void SetGraphicsTexture( SampledTexture2D texture ) {
        ArgumentNullException.ThrowIfNull( texture );
        Recording( );
        Track( texture.Resource );
        Track( texture.Heap );
        Native.Interface.SetDescriptorHeaps( 1, [texture.Heap.Native.Interface] );
        Native.Interface.SetGraphicsRootDescriptorTable( 0, texture.Heap.Native.Interface.GetGPUDescriptorHandleForHeapStart( ) );
    }

    /// <summary>Sets a viewport and scissor rectangle.</summary>
    public void SetViewport( uint width, uint height ) {
        Recording( ); if ( width == 0 || height == 0 ) throw new ArgumentOutOfRangeException( nameof(width) );
        D3D12_VIEWPORT viewport = new( ) { Width = width, Height = height, MaxDepth = 1 };
        RECT scissor = new( ) { right = checked((int)width), bottom = checked((int)height) };
        Native.Interface.RSSetViewports( 1, &viewport ); Native.Interface.RSSetScissorRects( 1, &scissor );
    }
    /// <summary>Clears a render target.</summary>
    public void Clear( RenderTargetView target, float red, float green, float blue, float alpha = 1 ) {
        Recording( ); Track( target.Heap ); Track( target.Resource );
        _color[0] = red; _color[1] = green; _color[2] = blue; _color[3] = alpha;
        Native.Interface.ClearRenderTargetView( target.Handle, _color, 0, null );
    }
    /// <summary>Binds a render target.</summary>
    public void SetRenderTarget( RenderTargetView target ) {
        Recording( ); Track( target.Heap ); Track( target.Resource );
        var handle = target.Handle;
        Native.Interface.OMSetRenderTargets( 1, &handle, false, null );
    }
    /// <summary>Binds a color target and a D32 depth target, optionally clearing depth to 1.</summary>
    public void SetRenderTarget( RenderTargetView target, DepthTarget depth, bool clearDepth = false ) {
        ArgumentNullException.ThrowIfNull( depth );
        Recording( );
        Track( target.Heap );
        Track( target.Resource );
        Track( depth.Heap );
        Track( depth.Resource );
        var colorHandle = target.Handle;
        var depthHandle = depth.Heap.CpuHandle( 0 );
        if ( clearDepth )
            Native.Interface.ClearDepthStencilView( depthHandle, D3D12_CLEAR_FLAGS.D3D12_CLEAR_FLAG_DEPTH, 1, 0, 0, null );
        Native.Interface.OMSetRenderTargets( 1, &colorHandle, false, &depthHandle );
    }

    /// <summary>Copies inline values to b0 for a graphics pipeline with a graphics constants root signature.</summary>
    public void SetGraphicsConstants( ReadOnlySpan<uint> values ) {
        Recording( );
        if ( values.Length is 0 or > 64 ) throw new ArgumentOutOfRangeException( nameof(values) );
        fixed ( uint* pointer = values )
            Native.Interface.SetGraphicsRoot32BitConstants( 0, (uint)values.Length, pointer, 0 );
    }

    /// <summary>Binds a graphics root signature and pipeline.</summary>
    public void SetGraphicsPipeline( PipelineState pipeline ) {
        Recording( ); if ( pipeline.IsCompute ) throw new ArgumentException( "A graphics pipeline is required." );
        Track( pipeline );
        if ( !_retained.ContainsKey( pipeline.RootNative ) ) _retained.Add( pipeline.RootNative, pipeline.RootNative.Retain( ) );
        Native.Interface.SetPipelineState( pipeline.Native.Interface );
        Native.Interface.SetGraphicsRootSignature( pipeline.RootNative.Interface );
        Native.Interface.IASetPrimitiveTopology( D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST );
    }
    /// <summary>Binds a compute root signature and pipeline.</summary>
    public void SetComputePipeline( PipelineState pipeline ) {
        Recording( ); if ( !pipeline.IsCompute ) throw new ArgumentException( "A compute pipeline is required." );
        Track( pipeline );
        if ( !_retained.ContainsKey( pipeline.RootNative ) ) _retained.Add( pipeline.RootNative, pipeline.RootNative.Retain( ) );
        Native.Interface.SetPipelineState( pipeline.Native.Interface );
        Native.Interface.SetComputeRootSignature( pipeline.RootNative.Interface );
    }
    /// <summary>Binds a buffer to a compute root UAV.</summary>
    public void SetComputeBuffer( uint parameterIndex, GpuResource buffer ) {
        Recording( ); Track( buffer ); Native.Interface.SetComputeRootUnorderedAccessView( parameterIndex, buffer.GpuAddress );
    }
    /// <summary>Draws a triangle-list vertex sequence.</summary>
    public void Draw( uint vertexCount, uint instanceCount = 1 ) { Recording( ); Native.Interface.DrawInstanced( vertexCount, instanceCount, 0, 0 ); }
    /// <summary>Records a compute dispatch.</summary>
    public void Dispatch( uint x, uint y = 1, uint z = 1 ) {
        Recording( ); if ( x == 0 || y == 0 || z == 0 ) throw new ArgumentOutOfRangeException( nameof(x) );
        Native.Interface.Dispatch( x, y, z );
    }

    internal List<ComReference> Submit( ) {
        if ( _state != 1 ) throw new InvalidOperationException( "Close the command list before submission; retire previous submissions before reuse." );
        Track( this ); _state = 2; Interlocked.Increment( ref Allocator.Pending );
        var result = _retained.Values.ToList( ); _retained.Clear( ); return result;
    }
    internal void Retired( ) { _state = 1; Interlocked.Decrement( ref Allocator.Pending ); }
    private void ReleaseRecording( ) { foreach ( var lease in _retained.Values ) lease.Dispose( ); _retained.Clear( ); }
    /// <inheritdoc />
    public override void Dispose( ) { ReleaseRecording( ); base.Dispose( ); }
}

/// <summary>A queue whose submissions retain native resources through fence completion. Dispose to drain pending work.</summary>
public sealed class CommandQueue : GraphicsObject {
    internal readonly ComOwner<ID3D12CommandQueue> Native;
    private readonly ComOwner<ID3D12Fence> _fence;
    private readonly ComOwner<ID3D12Device> _device;
    private readonly AutoResetEvent _event = new( false );
    private readonly object _gate = new( );
    private readonly List<Pending> _pending = new( );
    private readonly ID3D12CommandList[] _execute = new ID3D12CommandList[1];
    private ulong _nextValue;
    /// <summary>The work type of this queue.</summary>
    public QueueKind Kind { get; }
    internal CommandQueue( ComOwner<ID3D12CommandQueue> owner, ComOwner<ID3D12Fence> fence, GraphicsDevice device, QueueKind kind ) : base( owner ) {
        Native = owner; _fence = fence; _device = device.Native.Clone( ); Kind = kind;
    }
    internal override ComReference Retain( ) => Native.Retain( );
    /// <summary>Submits a closed list and returns its monotonic completion value.</summary>
    public ulong Submit( GraphicsCommandList commands ) {
        RequireSameDevice( commands );
        if ( commands.Kind != Kind ) throw new ArgumentException( "The command-list kind does not match this queue." );
        lock ( _gate ) {
            _ = Native.Interface;
            ulong value = checked(_nextValue + 1);
            var native = commands.Native.Interface;
            var pending = new Pending( value, commands, commands.Submit( ) );
            _pending.Add( pending ); _nextValue = value; SubmissionRoots.Add( this );
            _execute[0] = native;
            try {
                Native.Interface.ExecuteCommandLists( 1, _execute );
                int result = Native.Interface.Signal( _fence.Interface, value );
                pending.SignalFailure = result;
                Results.Check( result, "CommandQueue.Signal" );
            } finally { _execute[0] = null!; }
            return value;
        }
    }
    /// <summary>Waits for a submitted value. A timeout retains pending resources and prevents premature reuse.</summary>
    public void Wait( ulong value, TimeSpan? timeout = null ) {
        lock ( _gate ) {
            if ( value > _nextValue ) throw new ArgumentOutOfRangeException( nameof(value) );
            Pending? failed = _pending.Find( x => x.Value <= value && x.SignalFailure < 0 );
            if ( failed is not null ) throw new GraphicsException( "CommandQueue.Signal", failed.SignalFailure );
            TimeSpan duration = timeout ?? TimeSpan.FromSeconds( 30 );
            if ( duration < TimeSpan.Zero || duration.TotalMilliseconds > int.MaxValue ) throw new ArgumentOutOfRangeException( nameof(timeout) );
            long started = Environment.TickCount64;
            ulong completed = _fence.Interface.GetCompletedValue( );
            if ( completed < value ) Results.Check( _fence.Interface.SetEventOnCompletion( value,
                new HANDLE( _event.SafeWaitHandle.DangerousGetHandle( ) ) ), "Fence.SetEventOnCompletion" );
            while ( completed < value ) {
                double remaining = duration.TotalMilliseconds - (Environment.TickCount64 - started);
                if ( remaining <= 0 || !_event.WaitOne( TimeSpan.FromMilliseconds( Math.Max( 0, remaining ) ) ) )
                    throw new TimeoutException( $"GPU fence {value} did not complete; submission resources are still retained." );
                // A previous timed-out wait may have left an older registration on this event.
                completed = _fence.Interface.GetCompletedValue( );
            }
            if ( completed == ulong.MaxValue ) throw new GraphicsException( "Fence/device removed", _device.Interface.GetDeviceRemovedReason( ).Value );
            RetireThrough( completed );
        }
    }
    /// <summary>Signals a fresh fence and waits for earlier queue work, including swap-chain presents.</summary>
    public void WaitIdle( ) {
        lock ( _gate ) {
            ulong value = checked(_nextValue + 1);
            Results.Check( Native.Interface.Signal( _fence.Interface, value ), "CommandQueue.Signal(idle)" );
            _nextValue = value;
            Wait( value );
        }
    }
    private void PollCompletion( ) {
        if ( !Monitor.TryEnter( _gate ) ) return;
        try {
            if ( !IsDisposed ) RetireThrough( _fence.Interface.GetCompletedValue( ) );
        } finally { Monitor.Exit( _gate ); }
    }
    private void RetireThrough( ulong completed ) {
        while ( _pending.Count > 0 && _pending[0].Value <= completed ) {
            var item = _pending[0]; _pending.RemoveAt( 0 );
            item.Commands.Retired( ); foreach ( var lease in item.References ) lease.Dispose( );
        }
        if ( _pending.Count == 0 ) SubmissionRoots.Remove( this );
    }
    /// <inheritdoc />
    public override void Dispose( ) {
        lock ( _gate ) {
            if ( IsDisposed ) return;
            try { WaitIdle( ); }
            catch ( GraphicsException ) when ( _device.Interface.GetDeviceRemovedReason( ).Value < 0 ) { RetireThrough( ulong.MaxValue ); }
            _event.Dispose( ); _fence.Dispose( ); _device.Dispose( ); base.Dispose( );
        }
    }
    // Pending GPU work is an external root. Collection of a forgotten queue must not
    // finalize the resources the GPU is still using. Polling never waits on a busy queue.
    private static class SubmissionRoots {
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<CommandQueue, byte> Queues = new( );
        private static readonly Timer Poller = new( _ => {
            foreach ( var queue in Queues.Keys ) {
                try { queue.PollCompletion( ); }
                catch ( Exception error ) { System.Diagnostics.Debug.WriteLine( $"DXSharp retirement retained a queue after: {error}" ); }
            }
        }, null, 100, 100 );
        internal static void Add( CommandQueue queue ) { Queues.TryAdd( queue, 0 ); GC.KeepAlive( Poller ); }
        internal static void Remove( CommandQueue queue ) => Queues.TryRemove( queue, out _ );
    }
    private sealed class Pending( ulong value, GraphicsCommandList commands, List<ComReference> references ) {
        internal readonly ulong Value = value;
        internal readonly GraphicsCommandList Commands = commands;
        internal readonly List<ComReference> References = references;
        internal int SignalFailure;
    }
}
