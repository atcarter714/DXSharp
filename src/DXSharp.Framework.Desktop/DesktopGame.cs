using System.Diagnostics;
using System.Numerics;
using DXSharp.Graphics;

namespace DXSharp.Framework.Desktop;

/// <summary>Settings for a windowed Direct3D 12 application.</summary>
public sealed record DesktopGameOptions {
    /// <summary>The initial window title.</summary>
    public string Title { get; init; } = "DXSharp";
    /// <summary>The initial client area in pixels.</summary>
    public Size ClientSize { get; init; } = new( 1024, 720 );
    /// <summary>The RGBA clear color.</summary>
    public Vector4 ClearColor { get; init; } = new( 0.025f, 0.035f, 0.065f, 1 );
    /// <summary>Create and clear a D32_FLOAT depth target for each frame.</summary>
    public bool EnableDepth { get; init; }
    /// <summary>Close after this many presentations. Zero runs until the window closes.</summary>
    public int FrameLimit { get; init; }
    /// <summary>Number of frame slots. One waits for rendering before presenting. Two waits on slot reuse.</summary>
    public int FramesInFlight { get; init; } = 1;
}

/// <summary>A frame whose command list and render target are borrowed for the draw callback.</summary>
/// <param name="Commands">The open command list. The host closes and submits it after the callback.</param>
/// <param name="Target">The current back buffer's render-target view.</param>
/// <param name="Size">The current renderable client area in pixels.</param>
/// <param name="Elapsed">Elapsed time since the previous draw, excluding time spent minimized.</param>
public readonly record struct DesktopFrame( GraphicsCommandList Commands, RenderTargetView Target, Size Size, TimeSpan Elapsed );

/// <summary>Totals from a desktop run or a presentation callback.</summary>
/// <param name="Frames">Number of completed present calls.</param>
/// <param name="Resizes">Number of back-buffer size changes after initialization.</param>
public readonly record struct DesktopRunResult( int Frames, int Resizes );

/// <summary>Runs a WinForms frame loop with two back buffers and one or two frame slots.</summary>
public static class DesktopGame {
    /// <summary>Runs a windowed render loop on the calling STA thread until the window closes.</summary>
    /// <param name="device">A caller-owned device that stays alive for the duration of the run.</param>
    /// <param name="options">Window dimensions, depth, presentation limit and number of frame slots.</param>
    /// <param name="draw">Records scene commands into the borrowed frame before submission.</param>
    /// <param name="framePresented">Optional callback on the calling thread after each present call.</param>
    /// <returns>Presentation and resize totals.</returns>
    /// <exception cref="InvalidOperationException">The calling thread is outside an STA.</exception>
    public static DesktopRunResult Run( GraphicsDevice device, DesktopGameOptions options, Action<DesktopFrame> draw,
        Action<Form, DesktopRunResult>? framePresented = null ) {
        ArgumentNullException.ThrowIfNull( device );
        ArgumentNullException.ThrowIfNull( options );
        ArgumentNullException.ThrowIfNull( draw );
        if ( options.ClientSize.Width <= 0 || options.ClientSize.Height <= 0 || options.FrameLimit < 0 || options.FramesInFlight is < 1 or > 2 )
            throw new ArgumentOutOfRangeException( nameof(options) );
        if ( Thread.CurrentThread.GetApartmentState( ) != ApartmentState.STA )
            throw new InvalidOperationException( "The desktop game must run on an STA thread." );

        using var window = new GameWindow { Text = options.Title, ClientSize = options.ClientSize, StartPosition = FormStartPosition.CenterScreen };
        window.Show( );
        var queue = device.CreateCommandQueue( );
        Task? drain = null;
        try { return RunWindow( device, options, draw, framePresented, window, queue, Drain ); }
        finally { if ( drain is null ) queue.Dispose( ); }

        Task Drain( ) => drain ??= Task.Run( queue.Dispose );
    }

    private static DesktopRunResult RunWindow( GraphicsDevice device, DesktopGameOptions options, Action<DesktopFrame> draw,
        Action<Form, DesktopRunResult>? framePresented, GameWindow window, CommandQueue queue, Func<Task> drainQueue ) {
        Size size = window.ClientSize;
        using var first = new FrameSlot( device );
        using var second = options.FramesInFlight == 2 ? new FrameSlot( device ) : null;
        using var swap = device.CreateSwapChain( queue, window.Handle, (uint)size.Width, (uint)size.Height );
        using var heap = device.CreateRenderTargetHeap( swap.BufferCount );
        using var timer = new System.Windows.Forms.Timer { Interval = 1 };
        var buffers = new GpuResource?[swap.BufferCount];
        var targets = new RenderTargetView[swap.BufferCount];
        DepthTarget? depth = null;
        var clock = Stopwatch.StartNew( );
        int frames = 0, resizes = 0;
        TimeSpan previous = TimeSpan.Zero;
        Exception? frameFailure = null;

        try {
            AcquireBuffers( );
            timer.Tick += ( _, _ ) => {
                if ( window.CloseRequested ) {
                    // Keep the HWND and its message pump alive until the presentation queue drains.
                    var drain = drainQueue( );
                    if ( !drain.IsCompleted ) return;
                    timer.Stop( );
                    if ( drain.IsCompletedSuccessfully ) {
                        ReleaseBuffers( );
                        swap.Dispose( );
                    }
                    window.FinishClose( );
                    return;
                }
                if ( window.Disposing || !window.IsHandleCreated || window.WindowState == FormWindowState.Minimized ) {
                    previous = clock.Elapsed;
                    return;
                }
                Size clientSize = window.ClientSize;
                if ( clientSize.Width <= 0 || clientSize.Height <= 0 ) return;
                // Native presentation can dispatch messages. Keep nested timer ticks out of the frame.
                timer.Stop( );
                try {
                    bool resized = clientSize != size;
                    if ( resized ) queue.WaitIdle( );
                    var slot = second is not null && (frames & 1) != 0 ? second : first;
                    if ( slot.Completion != 0 ) queue.Wait( slot.Completion );
                    var commands = slot.Begin( );
                    if ( resized ) {
                        ReleaseBuffers( );
                        swap.Resize( (uint)clientSize.Width, (uint)clientSize.Height );
                        size = clientSize;
                        AcquireBuffers( );
                        ++resizes;
                    }
                    uint index = swap.CurrentBackBufferIndex;
                    var buffer = buffers[index]!;
                    var target = targets[index];
                    commands.Transition( buffer, ResourceState.Present, ResourceState.RenderTarget );
                    commands.SetViewport( (uint)size.Width, (uint)size.Height );
                    Vector4 clear = options.ClearColor;
                    commands.Clear( target, clear.X, clear.Y, clear.Z, clear.W );
                    if ( depth is null ) commands.SetRenderTarget( target );
                    else commands.SetRenderTarget( target, depth, clearDepth: true );
                    TimeSpan now = clock.Elapsed;
                    draw( new( commands, target, size, now - previous ) );
                    previous = now;
                    commands.Transition( buffer, ResourceState.RenderTarget, ResourceState.Present );
                    commands.Close( );
                    slot.Completion = queue.Submit( commands );
                    if ( second is null ) queue.Wait( slot.Completion );
                    swap.Present( );
                    ++frames;
                    framePresented?.Invoke( window, new( frames, resizes ) );
                    if ( !window.CloseRequested && window.IsHandleCreated && !window.Disposing &&
                         options.FrameLimit > 0 && frames >= options.FrameLimit ) window.Close( );
                } finally {
                    if ( window.IsHandleCreated && !window.Disposing ) timer.Start( );
                }
            };
            timer.Start( );
            Application.Run( window );
            return new( frames, resizes );
        } catch ( Exception error ) {
            frameFailure = error;
            throw;
        } finally {
            timer.Stop( );
            try { drainQueue( ).GetAwaiter( ).GetResult( ); }
            catch ( Exception drainFailure ) when ( frameFailure is not null ) {
                throw new AggregateException( "Rendering and presentation queue shutdown both failed.", frameFailure, drainFailure );
            } finally { ReleaseBuffers( ); }
        }

        void AcquireBuffers( ) {
            for ( uint i = 0; i < swap.BufferCount; ++i ) {
                buffers[i] = swap.AcquireBackBuffer( i );
                targets[i] = device.CreateRenderTargetView( buffers[i]!, heap, i );
            }
            if ( options.EnableDepth ) depth = device.CreateDepthTarget( (uint)size.Width, (uint)size.Height );
        }

        void ReleaseBuffers( ) {
            depth?.Dispose( );
            depth = null;
            foreach ( var buffer in buffers ) buffer?.Dispose( );
            Array.Clear( buffers );
            Array.Clear( targets );
        }
    }

    private sealed class GameWindow : Form {
        internal bool CloseRequested { get; private set; }
        private bool _drained;

        internal void FinishClose( ) {
            _drained = true;
            Close( );
        }

        protected override void OnFormClosing( FormClosingEventArgs e ) {
            if ( _drained ) {
                e.Cancel = false;
                return;
            }
            if ( !CloseRequested ) {
                base.OnFormClosing( e );
                if ( e.Cancel ) return;
                CloseRequested = true;
            }
            e.Cancel = true;
        }
    }

    private sealed class FrameSlot : IDisposable {
        private readonly CommandAllocator _allocator;
        private readonly GraphicsCommandList _commands;
        internal ulong Completion;

        internal FrameSlot( GraphicsDevice device ) {
            _allocator = device.CreateCommandAllocator( );
            try {
                _commands = device.CreateCommandList( _allocator );
                try { _commands.Close( ); }
                catch { _commands.Dispose( ); throw; }
            } catch { _allocator.Dispose( ); throw; }
        }

        internal GraphicsCommandList Begin( ) {
            _allocator.Reset( );
            _commands.Reset( _allocator );
            return _commands;
        }

        public void Dispose( ) {
            _commands.Dispose( );
            _allocator.Dispose( );
        }
    }
}
