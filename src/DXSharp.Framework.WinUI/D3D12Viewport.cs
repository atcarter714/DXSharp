using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Framework.Hosting;
using DXSharp.Graphics;
using DXSharp.Interop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi;

namespace DXSharp.Framework.WinUI;

/// <summary>A UI-thread-owned WinUI 3 composition viewport using the shared fenced frame renderer.</summary>
/// <remarks>The caller owns the device and scene and stops/disposes the viewport before releasing either.
/// Windows App SDK dependencies are confined to this optional assembly and its consumers.</remarks>
public sealed class D3D12Viewport : UserControl, IDisposable {
    private readonly int _thread = Environment.CurrentManagedThreadId;
    private readonly SwapChainPanel _panel = new( );
    private readonly Stopwatch _clock = new( );
    private readonly Action<RenderFrame> _record;
    private GraphicsDevice? _device;
    private WindowRenderOptions? _options;
    private Action<RenderFrame>? _draw;
    private FrameRenderer? _renderer;
    private NativeInterface<ISwapChainPanelNative>? _panelNative;
    private NativeInterface<IDXGISwapChain>? _chainNative;
    private Task? _stop;
    private RenderStatistics _retired;
    private TimeSpan _previous;
    private Vector2 _appliedScale;
    private nint _ownerWindow;
    private bool _started, _subscribed, _drawing, _disposed, _inside, _captured, _suspended, _scheduled;

    /// <summary>Creates a viewport usable as ordinary composed WinUI content.</summary>
    public D3D12Viewport( ) {
        _record = Record; Content = _panel; IsTabStop = true;
        Loaded += OnLoaded; Unloaded += OnUnloaded;
        PointerEntered += ( _, _ ) => _inside = true;
        PointerExited += ( _, _ ) => _inside = false;
        PointerPressed += OnPointerPressed;
        PointerReleased += ( _, _ ) => ReleasePointer( );
        PointerCaptureLost += ( _, _ ) => _captured = false;
        LostFocus += ( _, _ ) => ReleasePointer( );
        PointerWheelChanged += ( _, args ) => {
            if ( HasInputFocus ) { WheelScrolled?.Invoke( args.GetCurrentPoint( _panel ).Properties.MouseWheelDelta ); args.Handled = true; }
        };
    }
    /// <summary>The native composition panel, exposed for layout/transform configuration. Do not replace its swap chain.</summary>
    public SwapChainPanel Panel => _panel;
    /// <summary>Current composition scale, including XAML and monitor scale.</summary>
    public Vector2 CompositionScale => new( _panel.CompositionScaleX, _panel.CompositionScaleY );
    /// <summary>The requested physical back-buffer size, rounded up from panel layout and composition scale.</summary>
    public Size PixelSize {
        get {
            CheckThread( ); var scale = CompositionScale;
            if ( !double.IsFinite( _panel.ActualWidth ) || !double.IsFinite( _panel.ActualHeight ) ||
                !float.IsFinite( scale.X ) || !float.IsFinite( scale.Y ) || scale.X <= 0 || scale.Y <= 0 ) return Size.Empty;
            return new( checked((int)Math.Ceiling( _panel.ActualWidth * scale.X )), checked((int)Math.Ceiling( _panel.ActualHeight * scale.Y )) );
        }
    }
    /// <summary>Whether the viewport, rather than a WinUI editor/control, owns keyboard focus.</summary>
    public bool HasInputFocus {
        get {
            CheckThread( ); var active = PInvoke.GetActiveWindow( );
            return FocusState != Microsoft.UI.Xaml.FocusState.Unfocused && !active.IsNull &&
                (_ownerWindow == 0 || active == new HWND( _ownerWindow ));
        }
    }
    /// <summary>Whether pointer polling belongs to the viewport or its captured drag.</summary>
    public bool AcceptsPointerInput => HasInputFocus && (_inside || _captured);
    /// <summary>Host-driven suspension, for example when an owning view is inactive.</summary>
    public bool IsSuspended { get => _suspended; set { CheckThread( ); _suspended = value; _previous = _clock.Elapsed; } }
    /// <summary>Totals accumulated through unload/reload in the current session.</summary>
    public RenderStatistics Statistics {
        get { CheckThread( ); var s = _renderer?.Statistics ?? default; return new( _retired.Frames + s.Frames, _retired.Resizes + s.Resizes ); }
    }
    /// <summary>The last failure, cleared by Start.</summary>
    public Exception? Failure { get; private set; }
    /// <summary>Raised on the UI thread after presentation. Stopping/closing here is allowed.</summary>
    public event Action<RenderStatistics>? FramePresented;
    /// <summary>Raised on the UI thread when rendering fails. Failure retains the exception.</summary>
    public event Action<Exception>? RenderFailed;
    /// <summary>Mouse wheel input from the focused viewport, in Win32 wheel-delta units.</summary>
    public event Action<int>? WheelScrolled;

    /// <summary>Starts rendering on load. Supply an owning window to suspend presentation while it is minimized.</summary>
    public void Start( GraphicsDevice device, WindowRenderOptions options, Action<RenderFrame> draw, Window? owner = null ) {
        CheckThread( ); ObjectDisposedException.ThrowIf( _disposed, this );
        ArgumentNullException.ThrowIfNull( device ); ArgumentNullException.ThrowIfNull( options ); ArgumentNullException.ThrowIfNull( draw );
        if ( _drawing || _started || _stop is { IsCompleted: false } ) throw new InvalidOperationException( "Stop and await the current session before starting another." );
        if ( options.FramesInFlight is < 1 or > 3 || options.FrameLimit < 0 ) throw new ArgumentOutOfRangeException( nameof(options) );
        _device = device; _options = options; _draw = draw; _retired = default; Failure = null; _stop = null;
        _ownerWindow = owner is null ? 0 : WinRT.Interop.WindowNative.GetWindowHandle( owner );
        _started = true; _previous = TimeSpan.Zero; _clock.Restart( );
        if ( IsLoaded ) Attach( );
    }
    /// <summary>Stops scheduling, drains outstanding GPU work, detaches the panel and releases presentation resources.</summary>
    public Task StopAsync( ) {
        CheckThread( );
        if ( _disposed ) return Task.CompletedTask;
        if ( _drawing ) throw new InvalidOperationException( "Stop after recording, for example in FramePresented." );
        _started = false; Detach( ); ReleasePointer( );
        return _stop ??= RetireAsync( );
    }
    private void CheckThread( ) { if ( Environment.CurrentManagedThreadId != _thread ) throw new InvalidOperationException( "The WinUI viewport belongs to another thread." ); }
    private void OnLoaded( object sender, RoutedEventArgs args ) { if ( _started ) Attach( ); }
    private async void OnUnloaded( object sender, RoutedEventArgs args ) {
        Detach( ); ReleasePointer( );
        try { await (_stop ??= RetireAsync( )); }
        catch ( Exception error ) { Report( error ); }
        finally { _stop = null; if ( _started && IsLoaded && !_disposed ) Attach( ); }
    }
    private void Attach( ) {
        if ( _subscribed || _disposed ) return;
        _previous = _clock.Elapsed; CompositionTarget.Rendering += OnRendering; _subscribed = true;
    }
    private void Detach( ) {
        if ( !_subscribed ) return;
        CompositionTarget.Rendering -= OnRendering; _subscribed = false;
    }
    private void OnPointerPressed( object sender, PointerRoutedEventArgs args ) {
        if ( !args.GetCurrentPoint( _panel ).Properties.IsLeftButtonPressed ) return;
        Focus( Microsoft.UI.Xaml.FocusState.Pointer ); _captured = CapturePointer( args.Pointer ); args.Handled = true;
    }
    private void ReleasePointer( ) { ReleasePointerCaptures( ); _captured = false; }
    private void CreateRenderer( Size size ) {
        var renderer = FrameRenderer.CreateForComposition( _device!, _options! with { ClientSize = size } );
        NativeInterface<ISwapChainPanelNative>? panel = null;
        NativeInterface<IDXGISwapChain>? chain = null;
        try {
            nint inspectable = WinRT.MarshalInspectable<SwapChainPanel>.FromManaged( _panel );
            try {
                Guid id = typeof(ISwapChainPanelNative).GUID;
                Marshal.ThrowExceptionForHR( Marshal.QueryInterface( inspectable, in id, out nint pointer ) );
                panel = NativeInterface<ISwapChainPanelNative>.Adopt( pointer );
            } finally { WinRT.MarshalInspectable<SwapChainPanel>.DisposeAbi( inspectable ); }
            chain = renderer.QuerySwapChainInterface<IDXGISwapChain>( );
            var scale = CompositionScale; renderer.SetCompositionScale( scale.X, scale.Y );
            Marshal.ThrowExceptionForHR( panel.Interface.SetSwapChain( chain.Pointer ) );
            _renderer = renderer; _panelNative = panel; _chainNative = chain; _appliedScale = scale;
            renderer.SurfaceResized += ReattachSwapChain;
        } catch {
            if ( panel is not null ) panel.Interface.SetSwapChain( 0 );
            chain?.Dispose( ); panel?.Dispose( ); renderer.Dispose( ); throw;
        }
    }
    private void ReattachSwapChain( ) => Marshal.ThrowExceptionForHR( _panelNative!.Interface.SetSwapChain( _chainNative!.Pointer ) );
    private void OnRendering( object? sender, object args ) {
        if ( _scheduled || !_started || _disposed ) return;
        _scheduled = true;
        // Leave the XAML composition callback before recording or notifying subscribers that may mutate layout.
        if ( !DispatcherQueue.TryEnqueue( RenderTurn ) ) _scheduled = false;
    }
    private void RenderTurn( ) {
        try { RenderOnce( ); }
        finally { _scheduled = false; }
    }
    private void Record( RenderFrame frame ) {
        // Accumulate time across skipped GPU-busy turns; animation must not slow down when frames are unavailable.
        _previous = _clock.Elapsed; _draw!( frame );
    }
    private void RenderOnce( ) {
        if ( !_started || _disposed || _drawing || _stop is { IsCompleted: false } ) return;
        var now = _clock.Elapsed;
        if ( _suspended || !IsLoaded || Visibility != Visibility.Visible ||
            (_ownerWindow != 0 && PInvoke.IsIconic( new HWND( _ownerWindow ) )) ) { _previous = now; return; }
        try {
            var size = PixelSize;
            if ( size.Width <= 0 || size.Height <= 0 ) { _previous = now; return; }
            if ( _renderer is null ) CreateRenderer( size );
            var scale = CompositionScale;
            if ( scale != _appliedScale ) { _renderer!.SetCompositionScale( scale.X, scale.Y ); _appliedScale = scale; }
            bool presented;
            _drawing = true;
            try { presented = _renderer!.RenderAvailable( size, now - _previous, _record ); }
            finally { _drawing = false; }
            if ( presented ) {
                var statistics = Statistics; FramePresented?.Invoke( statistics );
                if ( _options!.FrameLimit > 0 && statistics.Frames >= _options.FrameLimit ) _ = FinishAsync( );
            }
        } catch ( Exception error ) { Failure = error; _ = FinishAsync( error ); }
    }
    private async Task FinishAsync( Exception? error = null ) {
        try { await StopAsync( ); }
        catch ( Exception stopError ) { error = error is null ? stopError : new AggregateException( error, stopError ); }
        if ( error is not null ) Report( error );
    }
    private void Report( Exception error ) { Failure = error; _started = false; Detach( ); RenderFailed?.Invoke( error ); }
    private async Task RetireAsync( ) {
        var renderer = _renderer;
        if ( renderer is null ) return;
        _retired = Statistics; _renderer = null; renderer.SurfaceResized -= ReattachSwapChain;
        try { await renderer.BeginDrain( ); }
        finally {
            try { if ( _panelNative is not null ) Marshal.ThrowExceptionForHR( _panelNative.Interface.SetSwapChain( 0 ) ); }
            finally { _chainNative?.Dispose( ); _chainNative = null; _panelNative?.Dispose( ); _panelNative = null; renderer.Dispose( ); }
        }
    }
    /// <summary>Disposes after a completed StopAsync. Async draining must finish while WinUI still dispatches messages.</summary>
    public void Dispose( ) {
        CheckThread( ); if ( _disposed ) return;
        if ( _drawing || _renderer is not null || _stop is { IsCompleted: false } )
            throw new InvalidOperationException( "Await StopAsync before disposing the WinUI viewport." );
        _disposed = true; _started = false; Detach( ); ReleasePointer( );
        Loaded -= OnLoaded; Unloaded -= OnUnloaded; _draw = null; _device = null; _clock.Stop( );
    }
}
