using DXSharp.Graphics;
using DXSharp.Framework.Hosting;
using DXSharp.Framework.WinUI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;

namespace DXSharp.CharacterLab.WinUI;

internal static class WinUIHostQualification {
    internal static async Task Run( CharacterWindow window, GraphicsDevice device, CharacterScene scene ) {
        var viewport = window.Viewport;
        await Frames( viewport, 3 );
        await viewport.StopAsync( );
        System.Drawing.Size rendered = default;
        window.Restart( frame => { rendered = frame.Size; scene.Draw( frame ); } );
        await Frames( viewport, 3 );
        var original = viewport.PixelSize;
        window.AppWindow.Resize( new SizeInt32( 1000, 700 ) );
        await Frames( viewport, viewport.Statistics.Frames + 3 );
        Require( viewport.PixelSize != original && viewport.Statistics.Resizes > 0 && rendered == viewport.PixelSize, "WinUI resize did not reach the physical back buffers." );
        Require( viewport.Focus( FocusState.Programmatic ) && viewport.HasInputFocus, "Cannot focus the WinUI viewport." );
        window.Editor.Focus( FocusState.Programmatic );
        Require( !viewport.HasInputFocus && Equals( FocusManager.GetFocusedElement( viewport.XamlRoot ), window.Editor ), "Viewport stole text editor focus." );
        Require( FocusManager.TryMoveFocus( FocusNavigationDirection.Next ) && viewport.HasInputFocus, "Tab entry did not reach the viewport." );
        Require( FocusManager.TryMoveFocus( FocusNavigationDirection.Previous ) && !viewport.HasInputFocus, "Tab exit did not leave the viewport." );

        viewport.Focus( FocusState.Programmatic );
        var other = new Window { Content = new Microsoft.UI.Xaml.Controls.TextBox { Text = "Other active window" } };
        try {
            other.Activate( ); await Task.Delay( 120 );
            Require( !viewport.HasInputFocus, "Inactive viewport continued accepting keyboard input." );
        } finally { other.Close( ); window.Activate( ); }
        viewport.Focus( FocusState.Programmatic );
        await Frames( viewport, viewport.Statistics.Frames + 2 );

        var scale = viewport.CompositionScale;
        viewport.Panel.RenderTransform = new ScaleTransform { ScaleX = 1.25, ScaleY = 1.5 };
        await Frames( viewport, viewport.Statistics.Frames + 4 );
        Require( viewport.CompositionScale != scale && rendered == viewport.PixelSize, "Composition scale did not resize the back buffers." );
        viewport.Panel.RenderTransform = null;
        await Frames( viewport, viewport.Statistics.Frames + 3 );

        viewport.IsSuspended = true;
        int paused = viewport.Statistics.Frames; await Task.Delay( 120 );
        Require( viewport.Statistics.Frames == paused, "Suspended WinUI host kept presenting." );
        viewport.IsSuspended = false; await Frames( viewport, paused + 2 );
        var presenter = (OverlappedPresenter)window.AppWindow.Presenter;
        presenter.Minimize( ); await Task.Delay( 80 );
        paused = viewport.Statistics.Frames; await Task.Delay( 120 );
        Require( viewport.Statistics.Frames == paused, "Minimized WinUI host kept presenting." );
        presenter.Restore( ); window.Activate( ); await Frames( viewport, paused + 2 );

        window.SceneArea.Children.Remove( viewport ); await Task.Delay( 100 );
        paused = viewport.Statistics.Frames; await Task.Delay( 120 );
        Require( viewport.Statistics.Frames == paused, "Unloaded WinUI host kept presenting." );
        window.SceneArea.Children.Insert( 0, viewport ); await Frames( viewport, paused + 3 );
        Console.WriteLine( "PASS: WinUI composition presentation, physical resize, XAML scale, text focus, tab entry/exit, activation isolation, suspension, minimize/restore and unload/reload." );

        await viewport.StopAsync( );
        var failed = new TaskCompletionSource<Exception>( TaskCreationOptions.RunContinuationsAsynchronously );
        void OnFailed( Exception error ) => failed.TrySetResult( error );
        viewport.RenderFailed += OnFailed;
        window.Restart( _ => throw new ApplicationException( "intentional WinUI render failure" ) );
        var error = await failed.Task.WaitAsync( TimeSpan.FromSeconds( 40 ) );
        viewport.RenderFailed -= OnFailed;
        Require( error is ApplicationException && viewport.Failure == error, "WinUI render failure was lost." );
        await viewport.StopAsync( ); window.Restart( ); await Frames( viewport, 3 );
        Require( viewport.Failure is null && window.Failure is null, "Restart did not clear the intentional failure." );
        viewport.FramePresented += statistics => { if ( statistics.Frames >= 4 ) window.CloseOrderly( ); };
        await window.Completion.WaitAsync( TimeSpan.FromSeconds( 40 ) );
        paused = viewport.Statistics.Frames; await Task.Delay( 100 );
        Require( viewport.Statistics.Frames == paused, "Closed WinUI host retained a rendering callback." );
        Console.WriteLine( "PASS: WinUI render failure, drained restart, close during presentation, panel detach and inert callbacks." );
    }
    private static async Task Frames( D3D12Viewport viewport, int frames ) {
        if ( viewport.Statistics.Frames >= frames ) return;
        var completion = new TaskCompletionSource( TaskCreationOptions.RunContinuationsAsynchronously );
        void Presented( RenderStatistics statistics ) { if ( statistics.Frames >= frames ) completion.TrySetResult( ); }
        void Failed( Exception error ) => completion.TrySetException( error );
        viewport.FramePresented += Presented; viewport.RenderFailed += Failed;
        try { await completion.Task.WaitAsync( TimeSpan.FromSeconds( 40 ) ); }
        finally { viewport.FramePresented -= Presented; viewport.RenderFailed -= Failed; }
    }
    private static void Require( bool condition, string message ) { if ( !condition ) throw new InvalidOperationException( message ); }
}
