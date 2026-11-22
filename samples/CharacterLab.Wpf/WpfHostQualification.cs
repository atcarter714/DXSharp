using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using DXSharp.Framework.Hosting;
using DXSharp.Framework.Wpf;
using DXSharp.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace DXSharp.CharacterLab.Wpf;

internal static class WpfHostQualification {
    internal static int Run( GraphicsDevice device, CharacterScene scene ) {
        var application = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        int result = 1;
        application.Startup += async ( _, _ ) => {
            try { await Exercise( device, scene ); result = 0; }
            catch ( Exception error ) { Console.Error.WriteLine( error ); }
            finally { application.Shutdown( result ); }
        };
        application.Run( ); return result;
    }

    private static async Task Exercise( GraphicsDevice device, CharacterScene scene ) {
        var viewport = new D3D12Viewport( );
        var editor = new TextBox { Text = "WPF retains keyboard focus", Margin = new( 8 ) };
        var holder = new Border { Child = viewport };
        var layout = new DockPanel( ); DockPanel.SetDock( editor, Dock.Top ); layout.Children.Add( editor ); layout.Children.Add( holder );
        KeyboardNavigation.SetTabNavigation( layout, KeyboardNavigationMode.Cycle );
        var window = new Window { Title = "DXSharp WPF host qualification", Width = 640, Height = 420, Content = layout };
        var watchdog = new DispatcherTimer { Interval = TimeSpan.FromSeconds( 90 ) };
        watchdog.Tick += ( _, _ ) => throw new TimeoutException( "WPF host qualification exceeded 90 seconds." );
        watchdog.Start( );
        try {
            window.Show( ); await Dispatcher.Yield( DispatcherPriority.ApplicationIdle );
            scene.Camera.InputEnabled = ( ) => viewport.HasInputFocus;
            scene.Camera.PointerEnabled = ( ) => viewport.AcceptsPointerInput;
            viewport.Start( device, new( ) { EnableDepth = true, FramesInFlight = 2 }, scene.Draw );
            await Frames( viewport, 3 );
            var first = viewport.PixelSize;
            window.Width += 160; window.Height += 80;
            await Frames( viewport, viewport.Statistics.Frames + 3 );
            Require( viewport.Statistics.Resizes > 0 && viewport.PixelSize.Width > first.Width, "Physical resize did not reach the swap chain." );
            var dpi = VisualTreeHelper.GetDpi( viewport );
            Require( Math.Abs( viewport.PixelSize.Width - viewport.ActualWidth * dpi.DpiScaleX ) <= 2 &&
                Math.Abs( viewport.PixelSize.Height - viewport.ActualHeight * dpi.DpiScaleY ) <= 2, "DIP-to-physical client sizing disagrees with WPF DPI." );
            Require( viewport.FocusViewport( ) && viewport.HasInputFocus, "Cannot focus the child HWND." );
            editor.Focus( ); Keyboard.Focus( editor ); await Dispatcher.Yield( DispatcherPriority.Input );
            Require( !viewport.HasInputFocus && Keyboard.FocusedElement == editor, "Viewport stole focus from the WPF text editor." );
            Require( ((IKeyboardInputSink)viewport).TabInto( new( FocusNavigationDirection.Next ) ) && viewport.HasInputFocus, "Tab navigation did not enter the viewport." );
            PInvoke.SendMessageW( new HWND( viewport.Handle ), 0x100, new WPARAM( 9 ), default );
            Require( !viewport.HasInputFocus, "Tab navigation did not leave the viewport." );

            window.WindowState = WindowState.Minimized;
            int paused = viewport.Statistics.Frames; await Task.Delay( 120 );
            Require( viewport.Statistics.Frames == paused, "Minimized host kept presenting." );
            window.WindowState = WindowState.Normal; await Frames( viewport, paused + 2 );
            viewport.Visibility = Visibility.Collapsed;
            paused = viewport.Statistics.Frames; await Task.Delay( 120 );
            Require( viewport.Statistics.Frames == paused, "Collapsed host kept presenting." );
            viewport.Visibility = Visibility.Visible; await Frames( viewport, paused + 2 );

            holder.Child = null; await Dispatcher.Yield( DispatcherPriority.ApplicationIdle );
            paused = viewport.Statistics.Frames; await Task.Delay( 120 );
            Require( viewport.Statistics.Frames == paused, "Unloaded host retained its rendering subscription." );
            holder.Child = viewport; await Frames( viewport, paused + 3 );
            Console.WriteLine( "PASS: WPF physical-pixel layout/DPI, resize, text focus, tab entry, minimize/restore, visibility and unload/reload." );

            await viewport.StopAsync( );
            var failure = new TaskCompletionSource<Exception>( TaskCreationOptions.RunContinuationsAsynchronously );
            void Failed( Exception error ) => failure.TrySetResult( error );
            viewport.RenderFailed += Failed;
            viewport.Start( device, new( ) { FramesInFlight = 2 }, _ => throw new ApplicationException( "intentional WPF render failure" ) );
            var reported = await failure.Task.WaitAsync( TimeSpan.FromSeconds( 30 ) );
            viewport.RenderFailed -= Failed;
            Require( reported is ApplicationException && viewport.Failure == reported, "Render failure was lost or changed." );
            await viewport.StopAsync( );
            viewport.Start( device, new( ) { FramesInFlight = 2 }, _ => { } );
            await Frames( viewport, 2 );
            Require( viewport.Failure is null, "Restart did not clear the previous session failure." );
            var closed = new TaskCompletionSource( TaskCreationOptions.RunContinuationsAsynchronously );
            nint handle = viewport.Handle;
            window.Closed += ( _, _ ) => closed.TrySetResult( );
            viewport.FramePresented += statistics => { if ( statistics.Frames >= 3 ) window.Close( ); };
            await closed.Task.WaitAsync( TimeSpan.FromSeconds( 30 ) );
            await viewport.StopAsync( ); viewport.Dispose( );
            Require( !PInvoke.IsWindow( new HWND( handle ) ), "Child HWND survived disposal." );
            paused = viewport.Statistics.Frames; await Task.Delay( 100 );
            Require( viewport.Statistics.Frames == paused, "Disposed viewport kept rendering queued callbacks." );
            Console.WriteLine( "PASS: WPF render-failure reporting, drained restart, close from presentation, HWND teardown and inert queued callbacks." );
        } finally {
            watchdog.Stop( );
            try { await viewport.StopAsync( ); } finally { viewport.Dispose( ); window.Close( ); }
        }
    }

    private static async Task Frames( D3D12Viewport viewport, int count ) {
        if ( viewport.Statistics.Frames >= count ) return;
        var completion = new TaskCompletionSource( TaskCreationOptions.RunContinuationsAsynchronously );
        void Presented( RenderStatistics statistics ) { if ( statistics.Frames >= count ) completion.TrySetResult( ); }
        void Failed( Exception error ) => completion.TrySetException( error );
        viewport.FramePresented += Presented; viewport.RenderFailed += Failed;
        try { await completion.Task.WaitAsync( TimeSpan.FromSeconds( 30 ) ); }
        finally { viewport.FramePresented -= Presented; viewport.RenderFailed -= Failed; }
    }
    private static void Require( bool condition, string message ) { if ( !condition ) throw new InvalidOperationException( message ); }
}
