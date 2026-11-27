using System.Diagnostics;
using System.Numerics;
using DXSharp.Graphics;
using DXSharp.Framework.Hosting;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

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
public readonly record struct DesktopFrame( GraphicsCommandList Commands, RenderTargetView Target, Size Size, TimeSpan Elapsed ) {
    /// <summary>The frame slot whose prior submission has retired. Use it to select per-slot upload storage.</summary>
    public int FrameSlotIndex { get; init; }
    /// <summary>The borrowed depth target, when enabled. Rebind it when returning from offscreen passes.</summary>
    public DepthTarget? Depth { get; init; }
}

/// <summary>Totals from a desktop run or a presentation callback.</summary>
/// <param name="Frames">Number of completed present calls.</param>
/// <param name="Resizes">Number of back-buffer size changes after initialization.</param>
public readonly record struct DesktopRunResult( int Frames, int Resizes );

/// <summary>WinForms scheduling over the shared frame renderer.</summary>
public static class DesktopGame {
    /// <summary>Runs on the calling STA thread. The device remains caller-owned.</summary>
    public static DesktopRunResult Run( GraphicsDevice device, DesktopGameOptions options, Action<DesktopFrame> draw,
        Action<Form, DesktopRunResult>? framePresented = null ) {
        ArgumentNullException.ThrowIfNull( device ); ArgumentNullException.ThrowIfNull( options ); ArgumentNullException.ThrowIfNull( draw );
        if ( Thread.CurrentThread.GetApartmentState( ) != ApartmentState.STA ) throw new InvalidOperationException( "The desktop game must run on an STA thread." );
        using var icon = Environment.ProcessPath is { } path ? Icon.ExtractAssociatedIcon( path ) : null;
        using var window = new GameWindow { Text = options.Title, ClientSize = options.ClientSize, StartPosition = FormStartPosition.CenterScreen };
        if ( icon is not null ) window.Icon = icon;
        window.Show( );
        using var renderer = new FrameRenderer( device, window.Handle, new( ) {
            Title = options.Title, ClientSize = window.ClientSize, ClearColor = options.ClearColor,
            EnableDepth = options.EnableDepth, FramesInFlight = options.FramesInFlight, FrameLimit = options.FrameLimit
        } );
        using var timer = new System.Windows.Forms.Timer { Interval = 16 };
        var clock = Stopwatch.StartNew( ); TimeSpan previous = clock.Elapsed; bool rendering = false, closing = false;
        void Draw( RenderFrame frame ) => draw( new( frame.Commands, frame.Target, frame.Size, frame.Elapsed ) {
            FrameSlotIndex = frame.FrameSlotIndex, Depth = frame.Depth
        } );
        void Tick( object? sender, EventArgs args ) {
            if ( rendering || closing || window.IsDisposed ) return;
            if ( window.CloseRequested ) {
                var drain = renderer.BeginDrain( );
                if ( !drain.IsCompleted ) return;
                closing = true; timer.Stop( );
                drain.GetAwaiter( ).GetResult( ); renderer.Dispose( ); window.FinishClose( ); return;
            }
            if ( window.IsDisposed || window.Disposing || window.WindowState == FormWindowState.Minimized ) { previous = clock.Elapsed; return; }
            rendering = true; timer.Stop( );
            try {
                TimeSpan now = clock.Elapsed;
                if ( renderer.Render( window.ClientSize, now - previous, Draw ) ) {
                    var stats = renderer.Statistics; framePresented?.Invoke( window, new( stats.Frames, stats.Resizes ) );
                    if ( options.FrameLimit > 0 && stats.Frames >= options.FrameLimit ) window.Close( );
                }
                previous = now;
            } finally { rendering = false; if ( !window.IsDisposed ) timer.Start( ); }
        }
        void Idle( object? sender, EventArgs args ) {
            while ( !rendering && !window.CloseRequested && !window.IsDisposed && window.WindowState != FormWindowState.Minimized &&
                !PInvoke.PeekMessageW( out _, default, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE ) ) Tick( null, EventArgs.Empty );
        }
        timer.Tick += Tick; Application.Idle += Idle; timer.Start( );
        try { Application.Run( window ); }
        finally { timer.Stop( ); Application.Idle -= Idle; timer.Tick -= Tick; }
        var result = renderer.Statistics; return new( result.Frames, result.Resizes );
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

}
