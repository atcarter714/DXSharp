using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using DXSharp.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DXSharp.Framework.Hosting;

/// <summary>A thread-owned Win32 window without Windows Forms. Its handle is borrowed; close through RequestClose.</summary>
public sealed unsafe class NativeGameWindow : IDisposable {
    private readonly int _thread = Environment.CurrentManagedThreadId;
    private readonly string _className = "DXSharp." + Guid.NewGuid( ).ToString( "N" );
    private readonly HINSTANCE _instance;
    private GCHandle _root;
    private HWND _window;
    private ushort _atom;
    private string _title;
    private Exception? _failure;
    private const WINDOW_STYLE Style = WINDOW_STYLE.WS_OVERLAPPEDWINDOW;
    /// <summary>Borrowed native handle, valid until disposal.</summary>
    public nint Handle => (nint)_window.Value;
    /// <summary>Whether the host should drain rendering and then destroy the window.</summary>
    public bool CloseRequested { get; private set; }
    /// <summary>Whether the window is minimized.</summary>
    public bool IsMinimized => PInvoke.IsIconic( _window );
    /// <summary>Current physical client size.</summary>
    public Size ClientSize {
        get { Check( PInvoke.GetClientRect( _window, out var rect ) ); return new( rect.right - rect.left, rect.bottom - rect.top ); }
    }
    /// <summary>Gets or sets the title on the creating thread.</summary>
    public string Title {
        get => _title;
        set { CheckThread( ); ArgumentNullException.ThrowIfNull( value ); Check( PInvoke.SetWindowTextW( _window, value ) ); _title = value; }
    }
    internal NativeGameWindow( WindowRenderOptions options ) {
        _title = options.Title;
        _instance = new HINSTANCE( NativeLibrary.GetMainProgramHandle( ) );
        _root = GCHandle.Alloc( this );
        try {
            fixed ( char* className = _className ) fixed ( char* title = _title ) {
                WNDCLASSEXW wc = new( ) { cbSize = (uint)sizeof(WNDCLASSEXW), lpfnWndProc = &WindowProc, hInstance = _instance,
                    lpszClassName = new PCWSTR( className ), hCursor = PInvoke.LoadCursorW( default, new PCWSTR( (char*)32512 ) ) };
                _atom = PInvoke.RegisterClassExW( wc ); if ( _atom == 0 ) throw new Win32Exception( );
                RECT rect = new( ) { right = options.ClientSize.Width, bottom = options.ClientSize.Height };
                Check( PInvoke.AdjustWindowRectExForDpi( ref rect, Style, false, 0, PInvoke.GetDpiForSystem( ) ) );
                _window = PInvoke.CreateWindowExW( 0, new PCWSTR( className ), new PCWSTR( title ), Style,
                    unchecked((int)0x80000000), unchecked((int)0x80000000), rect.right-rect.left, rect.bottom-rect.top,
                    default, default, _instance, (void*)GCHandle.ToIntPtr( _root ) );
                if ( _window.IsNull ) throw new Win32Exception( );
            }
            PInvoke.ShowWindow( _window, SHOW_WINDOW_CMD.SW_SHOW );
        } catch { Dispose( ); throw; }
    }
    /// <summary>Requests orderly shutdown. The HWND remains alive until its renderer drains.</summary>
    public void RequestClose( ) { CheckThread( ); CloseRequested = true; }
    /// <summary>Changes the client size in physical pixels on the creating thread.</summary>
    public void ResizeClient( Size size ) {
        CheckThread( );
        if ( size.Width <= 0 || size.Height <= 0 ) throw new ArgumentOutOfRangeException( nameof(size) );
        RECT rect = new( ) { right = size.Width, bottom = size.Height };
        Check( PInvoke.AdjustWindowRectExForDpi( ref rect, Style, false, 0, PInvoke.GetDpiForWindow( _window ) ) );
        Check( PInvoke.SetWindowPos( _window, default, 0, 0, rect.right-rect.left, rect.bottom-rect.top,
            SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE ) );
    }
    internal void Pump( bool reportErrors = true ) {
        while ( PInvoke.PeekMessageW( out var message, default, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE ) ) {
            if ( message.message == 0x12 ) { RequestClose( ); continue; }
            PInvoke.TranslateMessage( message ); PInvoke.DispatchMessageW( message );
        }
        if ( reportErrors && _failure is not null ) ExceptionDispatchInfo.Capture( _failure ).Throw( );
    }
    internal static void WaitForMessages( ) => PInvoke.MsgWaitForMultipleObjectsEx( 0, null, 16, QUEUE_STATUS_FLAGS.QS_ALLINPUT, MSG_WAIT_FOR_MULTIPLE_OBJECTS_EX_FLAGS.MWMO_INPUTAVAILABLE );
    [UnmanagedCallersOnly( CallConvs = [typeof(CallConvStdcall)] )]
    private static LRESULT WindowProc( HWND hwnd, uint message, WPARAM wparam, LPARAM lparam ) {
        NativeGameWindow? window = null;
        try {
            if ( message == 0x81 ) {
                var create = (CREATESTRUCTW*)lparam.Value;
                PInvoke.SetWindowLongPtrW( hwnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA, (nint)create->lpCreateParams );
            }
            nint token = PInvoke.GetWindowLongPtrW( hwnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA );
            if ( token != 0 ) window = GCHandle.FromIntPtr( token ).Target as NativeGameWindow;
            switch ( message ) {
                case 0x10: window?.RequestClose( ); return default;
                case 0x14: return new LRESULT( 1 ); // The renderer owns the full client area.
                case 0x201: case 0x207: PInvoke.SetCapture( hwnd ); break;
                case 0x202: case 0x208: PInvoke.ReleaseCapture( ); break;
                case 0x2E0:
                    var r = (RECT*)lparam.Value;
                    PInvoke.SetWindowPos( hwnd, default, r->left, r->top, r->right-r->left, r->bottom-r->top,
                        SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE ); return default;
                case 0x82: PInvoke.SetWindowLongPtrW( hwnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA, 0 ); break;
            }
            return PInvoke.DefWindowProcW( hwnd, message, wparam, lparam );
        } catch ( Exception error ) {
            if ( window is not null ) { window._failure = error; window.RequestClose( ); }
            return default; // Never unwind a managed exception through USER32.
        }
    }
    private void CheckThread( ) { if ( Environment.CurrentManagedThreadId != _thread ) throw new InvalidOperationException( "The native window is owned by another thread." ); }
    private static void Check( BOOL success ) { if ( !success ) throw new Win32Exception( Marshal.GetLastPInvokeError( ) ); }
    /// <summary>Destroys the window after its presentation work has drained.</summary>
    public void Dispose( ) {
        CheckThread( );
        if ( !_window.IsNull ) { Check( PInvoke.DestroyWindow( _window ) ); _window = default; }
        if ( _atom != 0 ) { fixed ( char* name = _className ) PInvoke.UnregisterClassW( new PCWSTR( name ), _instance ); _atom = 0; }
        if ( _root.IsAllocated ) _root.Free( );
    }
}

/// <summary>Native Win32 scheduling over the same renderer used by the managed desktop host.</summary>
public static class NativeGame {
    /// <summary>Runs a native HWND without WinForms. UI callbacks run on the caller's thread.</summary>
    public static RenderStatistics Run( GraphicsDevice device, WindowRenderOptions options, Action<RenderFrame> draw,
        Action<NativeGameWindow, RenderStatistics>? presented = null ) {
        ArgumentNullException.ThrowIfNull( device ); ArgumentNullException.ThrowIfNull( options ); ArgumentNullException.ThrowIfNull( draw );
        if ( options.ClientSize.Width <= 0 || options.ClientSize.Height <= 0 ) throw new ArgumentOutOfRangeException( nameof(options) );
        var previousDpi = PInvoke.SetThreadDpiAwarenessContext( new DPI_AWARENESS_CONTEXT( (nint)(-4) ) );
        try {
            using var window = new NativeGameWindow( options );
            using var renderer = new FrameRenderer( device, window.Handle, options with { ClientSize = window.ClientSize } );
            var clock = Stopwatch.StartNew( ); TimeSpan previous = clock.Elapsed;
            try {
                while ( !window.CloseRequested ) {
                    window.Pump( );
                    if ( window.CloseRequested ) break;
                    TimeSpan now = clock.Elapsed;
                    if ( window.IsMinimized ) { previous = now; NativeGameWindow.WaitForMessages( ); continue; }
                    if ( renderer.Render( window.ClientSize, now - previous, draw ) ) {
                        presented?.Invoke( window, renderer.Statistics );
                        if ( options.FrameLimit > 0 && renderer.Statistics.Frames >= options.FrameLimit ) window.RequestClose( );
                    } else NativeGameWindow.WaitForMessages( );
                    previous = now;
                }
                return renderer.Statistics;
            } finally {
                var drain = renderer.BeginDrain( );
                while ( !drain.IsCompleted ) { window.Pump( false ); NativeGameWindow.WaitForMessages( ); }
                drain.GetAwaiter( ).GetResult( );
            }
        } finally { if ( !previousDpi.IsNull ) PInvoke.SetThreadDpiAwarenessContext( previousDpi ); }
    }
}
