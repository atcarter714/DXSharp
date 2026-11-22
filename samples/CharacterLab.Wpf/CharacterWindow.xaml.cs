using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DXSharp.Framework.Hosting;
using DXSharp.Graphics;

namespace DXSharp.CharacterLab.Wpf;

internal partial class CharacterWindow : Window {
    private readonly CharacterScene _scene;
    private readonly int _limit;
    private bool _closing, _allowClose, _ready;
    internal Exception? Failure { get; private set; }
    internal RenderStatistics Statistics => Viewport.Statistics;

    internal CharacterWindow( GraphicsDevice device, CharacterScene scene, int limit ) {
        _scene = scene; _limit = limit;
        InitializeComponent( );
        ModelSummary.Text = $"{scene.Model.SourceVertices.Length:N0} vertices\n{scene.Model.Skin.Joints.Length} joints · {scene.Model.Animations.Length} clips";
        Clips.ItemsSource = scene.Model.Animations.Select( clip => clip.Name );
        Clips.SelectedIndex = scene.Model.FindAnimation( "Idle" ); _ready = true;
        scene.Camera.InputEnabled = ( ) => Viewport.HasInputFocus;
        scene.Camera.PointerEnabled = ( ) => Viewport.AcceptsPointerInput;
        Viewport.FramePresented += Presented;
        Viewport.RenderFailed += error => { Failure = error; Close( ); };
        Viewport.Start( device, new( ) { EnableDepth = true, FramesInFlight = 2 }, scene.Draw );
        Closing += ClosingWindow;
    }
    private void Presented( RenderStatistics statistics ) {
        if ( _scene.UpdateTitle( statistics.Frames ) is { } title ) Title = title;
        if ( statistics.Frames == 1 || statistics.Frames % 30 == 0 ) {
            var pixels = Viewport.PixelSize;
            Status.Text = $"{_scene.Player.CurrentClipName} · {pixels.Width} × {pixels.Height} px · CPU record {_scene.MeanRecordingMilliseconds:F2} ms";
        }
        if ( _limit > 0 && statistics.Frames >= _limit ) Close( );
    }
    private void ClipChanged( object sender, SelectionChangedEventArgs args ) { if ( _ready && Clips.SelectedIndex >= 0 ) _scene.Player.Play( Clips.SelectedIndex ); }
    private void PauseChanged( object sender, RoutedEventArgs args ) { if ( _ready ) _scene.Player.Paused = Paused.IsChecked == true; }
    private void SpeedChanged( object sender, RoutedPropertyChangedEventArgs<double> args ) { if ( _ready ) _scene.Player.Speed = (float)args.NewValue; }
    private void ResetCamera( object sender, RoutedEventArgs args ) => _scene.Camera.Reset( );
    private async void ClosingWindow( object? sender, CancelEventArgs args ) {
        if ( _allowClose ) return;
        args.Cancel = true;
        if ( _closing ) return;
        _closing = true;
        try { await Viewport.StopAsync( ); }
        catch ( Exception error ) { Failure ??= error; }
        finally {
            try { Viewport.Dispose( ); } catch ( Exception error ) { Failure ??= error; }
            _allowClose = true; Close( );
        }
    }
}
