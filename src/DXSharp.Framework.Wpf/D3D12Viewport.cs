using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using DXSharp.Framework.Hosting;
using DXSharp.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using PixelSize = System.Drawing.Size;

namespace DXSharp.Framework.Wpf;

/// <summary>A dispatcher-owned Direct3D 12 child HWND using the shared framework renderer.</summary>
/// <remarks>The device and scene remain caller-owned. Stop and dispose this control before disposing them.
/// WPF HWND airspace restrictions apply: WPF visuals cannot be composed over the viewport.</remarks>
public sealed class D3D12Viewport : HwndHost {
    private GraphicsDevice? _device;
    private WindowRenderOptions? _options;
    private Action<RenderFrame>? _draw;
    private FrameRenderer? _renderer, _retiring;
    private Window? _owner;
    private Task? _drain;
    private HWND _window;
    private readonly Stopwatch _clock = new( );
    private TimeSpan _previous, _lastComposition = TimeSpan.MinValue;
    private RenderStatistics _retiredStatistics;
    private bool _started, _subscribed, _drawing, _disposed;

    /// <summary>Creates a viewport that can also be declared in XAML and started after initialization.</summary>
    public D3D12Viewport( ) {
        Focusable = true;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        IsVisibleChanged += OnVisibilityChanged;
    }

    /// <summary>Physical client pixels, read from the HWND after WPF's layout and DPI conversion.</summary>
    public PixelSize PixelSize {
        get {
            VerifyAccess( );
            if ( _window.IsNull ) return PixelSize.Empty;
            if ( !PInvoke.GetClientRect( _window, out var rect ) ) throw new Win32Exception( );
            return new( rect.right - rect.left, rect.bottom - rect.top );
        }
    }
    /// <summary>Whether keyboard focus belongs to this native viewport rather than a WPF editor.</summary>
    public bool HasInputFocus { get { VerifyAccess( ); return !_window.IsNull && PInvoke.GetFocus( ) == _window; } }
    /// <summary>Whether a pointer drag may start or continue within this viewport.</summary>
    public bool AcceptsPointerInput {
        get {
            VerifyAccess( );
            if ( _window.IsNull || !HasInputFocus ) return false;
            if ( PInvoke.GetCapture( ) == _window ) return true;
            if ( !PInvoke.GetCursorPos( out System.Drawing.Point point ) || !PInvoke.GetWindowRect( _window, out var bounds ) ) return false;
            return point.X >= bounds.left && point.Y >= bounds.top && point.X < bounds.right && point.Y < bounds.bottom;
        }
    }
    /// <summary>Presentations and resizes accumulated across unload/reload of the same session.</summary>
    public RenderStatistics Statistics {
        get {
            VerifyAccess( );
            var current = _renderer?.Statistics ?? default;
            return new( _retiredStatistics.Frames + current.Frames, _retiredStatistics.Resizes + current.Resizes );
        }
    }
    /// <summary>The last rendering or presentation failure, retained until the next Start.</summary>
    public Exception? Failure { get; private set; }
    /// <summary>Raised on the dispatcher after a successful presentation. Closing or stopping here is allowed.</summary>
    public event Action<RenderStatistics>? FramePresented;
    /// <summary>Raised on the dispatcher after a failed session has drained. The exception is also retained in Failure.</summary>
    public event Action<Exception>? RenderFailed;

    /// <summary>Starts a session, immediately if loaded or on the next Loaded event. Resources remain caller-owned.</summary>
    public void Start( GraphicsDevice device, WindowRenderOptions options, Action<RenderFrame> draw ) {
        VerifyAccess( ); ObjectDisposedException.ThrowIf( _disposed, this );
        ArgumentNullException.ThrowIfNull( device ); ArgumentNullException.ThrowIfNull( options ); ArgumentNullException.ThrowIfNull( draw );
        if ( _drawing || _started || _retiring is not null ) throw new InvalidOperationException( "Stop and await the current session before starting another." );
        if ( options.FramesInFlight is < 1 or > 3 || options.FrameLimit < 0 ) throw new ArgumentOutOfRangeException( nameof(options) );
        _device = device; _options = options; _draw = draw; Failure = null; _retiredStatistics = default;
        _previous = TimeSpan.Zero; _lastComposition = TimeSpan.MinValue; _clock.Restart( ); _started = true;
        if ( IsLoaded ) Attach( );
    }

    /// <summary>Stops scheduling and asynchronously drains GPU work while WPF continues processing messages.</summary>
    public async Task StopAsync( ) {
        VerifyAccess( );
        if ( _drawing ) throw new InvalidOperationException( "Stop after recording, for example in FramePresented." );
        _started = false; Detach( );
        await RetireRenderer( );
        CompleteRetirement( ); _clock.Stop( );
    }

    /// <summary>Moves keyboard focus into the native viewport.</summary>
    public bool FocusViewport( ) {
        VerifyAccess( );
        if ( _window.IsNull || !IsEnabled || !IsVisible ) return false;
        Keyboard.Focus( this ); PInvoke.SetFocus( _window ); return HasInputFocus;
    }

    /// <inheritdoc />
    protected override unsafe HandleRef BuildWindowCore( HandleRef hwndParent ) {
        fixed ( char* name = "STATIC" ) {
            _window = PInvoke.CreateWindowExW( 0, new( name ), default,
                WINDOW_STYLE.WS_CHILD | WINDOW_STYLE.WS_VISIBLE | WINDOW_STYLE.WS_CLIPCHILDREN | WINDOW_STYLE.WS_CLIPSIBLINGS | WINDOW_STYLE.WS_TABSTOP,
                0, 0, 1, 1, new HWND( hwndParent.Handle ), default, default, null );
        }
        if ( _window.IsNull ) throw new Win32Exception( );
        return new( this, (nint)_window.Value );
    }

    /// <inheritdoc />
    protected override void DestroyWindowCore( HandleRef hwnd ) {
        Detach( );
        try { WaitForDrain( RetireRenderer( ) ); CompleteRetirement( ); }
        finally {
            _window = default;
            if ( !PInvoke.DestroyWindow( new HWND( hwnd.Handle ) ) ) throw new Win32Exception( );
        }
    }

    /// <inheritdoc />
    protected override bool TabIntoCore( TraversalRequest request ) => FocusViewport( );

    /// <inheritdoc />
    protected override nint WndProc( nint hwnd, int message, nint wparam, nint lparam, ref bool handled ) {
        if ( message == 0x201 ) { FocusViewport( ); PInvoke.SetCapture( _window ); }
        if ( message == 0x202 && PInvoke.GetCapture( ) == _window ) PInvoke.ReleaseCapture( );
        if ( message == 0x8 && PInvoke.GetCapture( ) == _window ) PInvoke.ReleaseCapture( );
        if ( message == 0x100 && wparam == 9 ) {
            bool reverse = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            handled = ((IKeyboardInputSink)this).KeyboardInputSite?.OnNoMoreTabStops( new( reverse ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next ) ) ?? false;
        }
        if ( message == 0x14 ) { handled = true; return 1; }
        return base.WndProc( hwnd, message, wparam, lparam, ref handled );
    }

    private void OnLoaded( object sender, RoutedEventArgs args ) {
        _owner = Window.GetWindow( this );
        if ( _owner is not null ) _owner.StateChanged += OnOwnerStateChanged;
        if ( _started ) Attach( );
    }
    private void OnUnloaded( object sender, RoutedEventArgs args ) {
        if ( _owner is not null ) _owner.StateChanged -= OnOwnerStateChanged;
        _owner = null;
        Detach( ); WaitForDrain( RetireRenderer( ) ); CompleteRetirement( ); _previous = _clock.Elapsed;
    }
    private void OnOwnerStateChanged( object? sender, EventArgs args ) => _previous = _clock.Elapsed;
    private void OnVisibilityChanged( object sender, DependencyPropertyChangedEventArgs args ) => _previous = _clock.Elapsed;
    private void Attach( ) {
        if ( _subscribed || _disposed || _window.IsNull ) return;
        _previous = _clock.Elapsed; _lastComposition = TimeSpan.MinValue;
        CompositionTarget.Rendering += OnRendering; _subscribed = true;
    }
    private void Detach( ) {
        if ( !_subscribed ) return;
        CompositionTarget.Rendering -= OnRendering; _subscribed = false;
    }
    private void OnRendering( object? sender, EventArgs args ) {
        if ( !_started || _disposed || _drawing || _retiring is not null ) return;
        if ( args is RenderingEventArgs composition ) {
            if ( composition.RenderingTime == _lastComposition ) return;
            _lastComposition = composition.RenderingTime;
        }
        var now = _clock.Elapsed;
        if ( !IsVisible || Window.GetWindow( this )?.WindowState == WindowState.Minimized ) { _previous = now; return; }
        var size = PixelSize;
        if ( size.Width <= 0 || size.Height <= 0 ) { _previous = now; return; }
        try {
            _renderer ??= new( _device!, (nint)_window, _options! with { ClientSize = size } );
            bool presented;
            _drawing = true;
            try { presented = _renderer.Render( size, now - _previous, _draw! ); }
            finally { _drawing = false; }
            _previous = now;
            if ( presented ) {
                var statistics = Statistics; FramePresented?.Invoke( statistics );
                if ( _options!.FrameLimit > 0 && statistics.Frames >= _options.FrameLimit ) _ = StopAsync( );
            }
        } catch ( Exception error ) { Failure = error; _ = ReportFailureAsync( error ); }
    }
    private async Task ReportFailureAsync( Exception error ) {
        try { await StopAsync( ); }
        catch ( Exception drainError ) { Failure = new AggregateException( error, drainError ); }
        RenderFailed?.Invoke( Failure! );
    }
    private Task RetireRenderer( ) {
        if ( _renderer is { } renderer ) {
            var drain = renderer.BeginDrain( );
            _retiredStatistics = Statistics; _retiring = renderer; _drain = drain; _renderer = null;
        }
        return _drain ?? Task.CompletedTask;
    }
    private void CompleteRetirement( ) {
        if ( _retiring is null ) return;
        _drain!.GetAwaiter( ).GetResult( ); _retiring.Dispose( ); _retiring = null; _drain = null;
    }
    private void WaitForDrain( Task drain ) {
        if ( !drain.IsCompleted && !Dispatcher.HasShutdownStarted ) {
            var frame = new DispatcherFrame( );
            _ = drain.ContinueWith( _ => Dispatcher.BeginInvoke( DispatcherPriority.Send, new Action( ( ) => frame.Continue = false ) ), TaskScheduler.Default );
            Dispatcher.PushFrame( frame );
        }
        drain.GetAwaiter( ).GetResult( );
    }
    /// <inheritdoc />
    protected override void Dispose( bool disposing ) {
        if ( disposing && !_disposed ) {
            VerifyAccess( );
            if ( _drawing ) throw new InvalidOperationException( "Dispose after the render callback has returned." );
            _disposed = true; _started = false; Detach( );
            WaitForDrain( RetireRenderer( ) ); CompleteRetirement( );
            Loaded -= OnLoaded; Unloaded -= OnUnloaded;
            IsVisibleChanged -= OnVisibilityChanged;
            if ( _owner is not null ) _owner.StateChanged -= OnOwnerStateChanged;
            _owner = null;
            _draw = null; _device = null; _clock.Stop( );
        }
        base.Dispose( disposing );
    }
}
