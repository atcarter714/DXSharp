using System.Drawing;
using System.Numerics;
using DXSharp.Graphics;

namespace DXSharp.Framework.Hosting;

/// <summary>Settings shared by native and managed window hosts.</summary>
public sealed record WindowRenderOptions {
    /// <summary>Initial window title.</summary>
    public string Title { get; init; } = "DXSharp";
    /// <summary>Initial physical client size.</summary>
    public Size ClientSize { get; init; } = new( 1024, 720 );
    /// <summary>Color used to clear the back buffer.</summary>
    public Vector4 ClearColor { get; init; } = new( .025f, .035f, .065f, 1 );
    /// <summary>Creates a depth buffer matching the presentation surface.</summary>
    public bool EnableDepth { get; init; }
    /// <summary>Frame slots. Reuse waits for the slot's previous submission.</summary>
    public int FramesInFlight { get; init; } = 2;
    /// <summary>Bounded presentation count, or zero for an interactive run.</summary>
    public int FrameLimit { get; init; }
}

/// <summary>Borrowed frame resources valid only during the render callback.</summary>
public readonly record struct RenderFrame( GraphicsCommandList Commands, RenderTargetView Target, Size Size, TimeSpan Elapsed, int FrameSlotIndex ) {
    /// <summary>The optional borrowed depth target for this frame.</summary>
    public DepthTarget? Depth { get; init; }
}
/// <summary>Completed presentations and surface resizes.</summary>
public readonly record struct RenderStatistics( int Frames, int Resizes );

/// <summary>Window-system-independent frame slots with HWND or composition presentation. Public methods run on the creating thread; drain work runs on a worker.</summary>
/// <remarks>The host owns its presentation surface and keeps it alive until drain completes. The device remains caller-owned.</remarks>
public sealed class FrameRenderer : IDisposable {
    private readonly int _thread = Environment.CurrentManagedThreadId;
    private readonly GraphicsDevice _device;
    private readonly WindowRenderOptions _options;
    private readonly CommandQueue _queue;
    private readonly SwapChain _swap;
    private readonly DescriptorHeap _heap;
    private readonly FrameSlot[] _slots;
    private readonly GpuResource?[] _buffers;
    private readonly RenderTargetView[] _targets;
    private DepthTarget? _depth;
    private Size _size;
    private bool _rendering, _disposed;
    private Task? _drain, _resizeDrain;
    private bool _pendingPresent;
    /// <summary>Raised on the owning thread after back buffers are resized, before recording/presentation. Composition hosts reattach their panel here.</summary>
    public event Action? SurfaceResized;
    /// <summary>Current presentation totals.</summary>
    public RenderStatistics Statistics { get; private set; }

    /// <summary>Creates presentation resources for an externally owned live HWND.</summary>
    public FrameRenderer( GraphicsDevice device, nint window, WindowRenderOptions options ) : this( device, window, options, false ) { }
    /// <summary>Creates composition presentation resources. The caller attaches the queried native swap chain to its UI surface.</summary>
    public static FrameRenderer CreateForComposition( GraphicsDevice device, WindowRenderOptions options ) => new( device, 0, options, true );
    /// <summary>Queries an independently owned native view of the presentation chain. Detach the host and dispose this view before renderer disposal.</summary>
    public DXSharp.Interop.NativeInterface<T> QuerySwapChainInterface<T>( ) where T : class {
        CheckThread( ); ObjectDisposedException.ThrowIf( _disposed, this ); return _swap.QueryNativeInterface<T>( );
    }
    /// <summary>Applies composition scale on the creating thread. HWND renderers reject this operation.</summary>
    public void SetCompositionScale( float scaleX, float scaleY ) { CheckThread( ); ObjectDisposedException.ThrowIf( _disposed, this ); _swap.SetCompositionScale( scaleX, scaleY ); }
    private FrameRenderer( GraphicsDevice device, nint window, WindowRenderOptions options, bool composition ) {
        ArgumentNullException.ThrowIfNull( device ); ArgumentNullException.ThrowIfNull( options );
        if ( options.ClientSize.Width <= 0 || options.ClientSize.Height <= 0 || options.FramesInFlight is < 1 or > 3 || options.FrameLimit < 0 )
            throw new ArgumentOutOfRangeException( nameof(options) );
        _device = device; _options = options; _size = options.ClientSize;
        _queue = device.CreateCommandQueue( );
        var owners = new List<IDisposable>( );
        try {
            _swap = composition ? device.CreateCompositionSwapChain( _queue, (uint)_size.Width, (uint)_size.Height ) :
                device.CreateSwapChain( _queue, window, (uint)_size.Width, (uint)_size.Height ); owners.Add( _swap );
            _heap = device.CreateRenderTargetHeap( _swap.BufferCount ); owners.Add( _heap );
            _slots = new FrameSlot[options.FramesInFlight];
            for ( int i = 0; i < _slots.Length; ++i ) { _slots[i] = new( device ); owners.Add( _slots[i] ); }
            _buffers = new GpuResource?[_swap.BufferCount]; _targets = new RenderTargetView[_swap.BufferCount];
            AcquireBuffers( );
        } catch {
            _depth?.Dispose( );
            if ( _buffers is not null ) foreach ( var b in _buffers ) b?.Dispose( );
            for ( int i = owners.Count - 1; i >= 0; --i ) owners[i].Dispose( );
            _queue.Dispose( ); throw;
        }
    }

    /// <summary>Records and presents one frame. Zero-sized/minimized surfaces are skipped. Nested calls return false.</summary>
    public bool Render( Size size, TimeSpan elapsed, Action<RenderFrame> draw ) => RenderCore( size, elapsed, draw, false );
    /// <summary>Attempts a frame without waiting for GPU fences or DXGI presentation. Resize draining runs on a worker; false means retry on a later UI turn.</summary>
    /// <remarks>A submitted frame whose presentation is busy is retained and retried before any new recording. Draw is called only for retired frame slots.</remarks>
    public bool RenderAvailable( Size size, TimeSpan elapsed, Action<RenderFrame> draw ) => RenderCore( size, elapsed, draw, true );
    private bool RenderCore( Size size, TimeSpan elapsed, Action<RenderFrame> draw, bool nonblocking ) {
        CheckThread( ); ObjectDisposedException.ThrowIf( _disposed, this ); ArgumentNullException.ThrowIfNull( draw );
        if ( _drain is not null ) throw new InvalidOperationException( "Presentation is draining." );
        if ( _rendering || size.Width <= 0 || size.Height <= 0 ) return false;
        _rendering = true;
        try {
            if ( _pendingPresent ) {
                if ( !_swap.TryPresent( ) ) return false;
                _pendingPresent = false; Statistics = Statistics with { Frames = Statistics.Frames + 1 }; return true;
            }
            if ( _resizeDrain is not null ) {
                if ( !_resizeDrain.IsCompleted ) return false;
                _resizeDrain.GetAwaiter( ).GetResult( ); _resizeDrain = null;
                Resize( size );
            } else if ( size != _size ) {
                if ( nonblocking ) { _resizeDrain = Task.Run( _queue.WaitIdle ); return false; }
                _queue.WaitIdle( ); Resize( size );
            }
            int slotIndex = Statistics.Frames % _slots.Length; var slot = _slots[slotIndex];
            if ( slot.Completion != 0 ) {
                if ( nonblocking ) { if ( !_queue.TryWait( slot.Completion ) ) return false; }
                else _queue.Wait( slot.Completion );
            }
            var commands = slot.Begin( ); uint index = _swap.CurrentBackBufferIndex;
            var buffer = _buffers[index]!; var target = _targets[index];
            commands.Transition( buffer, ResourceState.Present, ResourceState.RenderTarget );
            commands.SetViewport( (uint)size.Width, (uint)size.Height );
            var c = _options.ClearColor; commands.Clear( target, c.X, c.Y, c.Z, c.W );
            if ( _depth is null ) commands.SetRenderTarget( target ); else commands.SetRenderTarget( target, _depth, clearDepth: true );
            draw( new( commands, target, size, elapsed, slotIndex ) { Depth = _depth } );
            commands.Transition( buffer, ResourceState.RenderTarget, ResourceState.Present ); commands.Close( );
            slot.Completion = _queue.Submit( commands );
            if ( nonblocking ) {
                _pendingPresent = !_swap.TryPresent( );
                if ( _pendingPresent ) return false;
            } else {
                if ( _slots.Length == 1 ) _queue.Wait( slot.Completion );
                _swap.Present( );
            }
            Statistics = Statistics with { Frames = Statistics.Frames + 1 }; return true;
        } finally { _rendering = false; }
    }
    private void Resize( Size size ) {
        // The drain has completed and retired the command recordings' back-buffer leases.
        ReleaseBuffers( ); _swap.Resize( (uint)size.Width, (uint)size.Height ); _size = size; AcquireBuffers( );
        Statistics = Statistics with { Resizes = Statistics.Resizes + 1 }; SurfaceResized?.Invoke( );
    }
    /// <summary>Starts draining on a worker so the UI host can continue dispatching messages until completion.</summary>
    public Task BeginDrain( ) {
        CheckThread( ); ObjectDisposedException.ThrowIf( _disposed, this );
        if ( _rendering ) throw new InvalidOperationException( "Cannot drain during recording or presentation." );
        return _drain ??= DrainAsync( );
    }
    private async Task DrainAsync( ) {
        try { if ( _resizeDrain is not null ) await _resizeDrain.ConfigureAwait( false ); }
        finally { await Task.Run( _queue.WaitIdle ).ConfigureAwait( false ); }
    }
    private void CheckThread( ) { if ( Environment.CurrentManagedThreadId != _thread ) throw new InvalidOperationException( "The frame renderer is owned by another thread." ); }
    private void AcquireBuffers( ) {
        for ( uint i = 0; i < _swap.BufferCount; ++i ) {
            _buffers[i] = _swap.AcquireBackBuffer( i ); _targets[i] = _device.CreateRenderTargetView( _buffers[i]!, _heap, i );
        }
        if ( _options.EnableDepth ) _depth = _device.CreateDepthTarget( (uint)_size.Width, (uint)_size.Height );
    }
    private void ReleaseBuffers( ) {
        _depth?.Dispose( ); _depth = null;
        foreach ( var buffer in _buffers ) buffer?.Dispose( ); Array.Clear( _buffers ); Array.Clear( _targets );
    }
    /// <summary>Drains and releases presentation resources. The caller keeps the HWND alive during this operation.</summary>
    public void Dispose( ) {
        CheckThread( ); if ( _disposed ) return;
        BeginDrain( ).GetAwaiter( ).GetResult( );
        foreach ( var slot in _slots ) slot.Dispose( ); ReleaseBuffers( ); _swap.Dispose( ); _heap.Dispose( ); _queue.Dispose( ); _disposed = true;
    }
    private sealed class FrameSlot : IDisposable {
        private readonly CommandAllocator _allocator;
        private readonly GraphicsCommandList _commands;
        internal ulong Completion;
        internal FrameSlot( GraphicsDevice device ) {
            _allocator = device.CreateCommandAllocator( );
            try { _commands = device.CreateCommandList( _allocator ); try { _commands.Close( ); } catch { _commands.Dispose( ); throw; } }
            catch { _allocator.Dispose( ); throw; }
        }
        internal GraphicsCommandList Begin( ) { _allocator.Reset( ); _commands.Reset( _allocator ); return _commands; }
        public void Dispose( ) { _commands.Dispose( ); _allocator.Dispose( ); }
    }
}
