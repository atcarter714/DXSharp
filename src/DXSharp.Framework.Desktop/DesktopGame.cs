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
}

/// <summary>A frame whose command list and render target are borrowed for the draw callback.</summary>
public readonly record struct DesktopFrame( GraphicsCommandList Commands, RenderTargetView Target, Size Size, TimeSpan Elapsed );

/// <summary>Totals from a desktop run.</summary>
public readonly record struct DesktopRunResult( int Frames, int Resizes );

/// <summary>Runs a synchronous WinForms frame loop with a single allocator and two back buffers.</summary>
public static class DesktopGame {
    /// <summary>Runs on the calling STA thread. The host owns the window, submission and presentation.
    /// The caller owns the device and scene resources. Record draw commands only during the draw callback.</summary>
    public static DesktopRunResult Run( GraphicsDevice device, DesktopGameOptions options, Action<DesktopFrame> draw,
        Action<Form, DesktopRunResult>? framePresented = null ) {
        ArgumentNullException.ThrowIfNull( device );
        ArgumentNullException.ThrowIfNull( options );
        ArgumentNullException.ThrowIfNull( draw );
        if ( options.ClientSize.Width <= 0 || options.ClientSize.Height <= 0 || options.FrameLimit < 0 )
            throw new ArgumentOutOfRangeException( nameof(options) );
        if ( Thread.CurrentThread.GetApartmentState( ) != ApartmentState.STA )
            throw new InvalidOperationException( "The desktop game must run on an STA thread." );

        using var window = new Form { Text = options.Title, ClientSize = options.ClientSize, StartPosition = FormStartPosition.CenterScreen };
        window.Show( );
        Size size = window.ClientSize;
        using var queue = device.CreateCommandQueue( );
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        commands.Close( );
        using var swap = device.CreateSwapChain( queue, window.Handle, (uint)size.Width, (uint)size.Height );
        using var heap = device.CreateRenderTargetHeap( swap.BufferCount );
        using var timer = new System.Windows.Forms.Timer { Interval = 1 };
        var buffers = new GpuResource?[swap.BufferCount];
        var targets = new RenderTargetView[swap.BufferCount];
        DepthTarget? depth = null;
        var clock = Stopwatch.StartNew( );
        int frames = 0, resizes = 0;
        TimeSpan previous = TimeSpan.Zero;

        try {
            AcquireBuffers( );
            window.FormClosing += ( _, _ ) => timer.Stop( );
            timer.Tick += ( _, _ ) => {
                if ( window.Disposing || !window.IsHandleCreated || window.WindowState == FormWindowState.Minimized ) {
                    previous = clock.Elapsed;
                    return;
                }
                Size clientSize = window.ClientSize;
                if ( clientSize.Width <= 0 || clientSize.Height <= 0 ) return;
                // Native presentation can dispatch messages. Keep nested timer ticks out of the frame.
                timer.Stop( );
                bool resized = clientSize != size;
                if ( resized ) queue.WaitIdle( );
                allocator.Reset( );
                commands.Reset( allocator );
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
                queue.Wait( queue.Submit( commands ) );
                swap.Present( );
                ++frames;
                framePresented?.Invoke( window, new( frames, resizes ) );
                if ( window.IsDisposed || window.Disposing || !window.IsHandleCreated ) return;
                if ( options.FrameLimit > 0 && frames >= options.FrameLimit ) window.Close( );
                else timer.Start( );
            };
            timer.Start( );
            Application.Run( window );
            return new( frames, resizes );
        } finally {
            timer.Stop( );
            try { queue.WaitIdle( ); }
            finally { ReleaseBuffers( ); }
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
}
