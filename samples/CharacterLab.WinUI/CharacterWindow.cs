using DXSharp.Framework.Hosting;
using DXSharp.Framework.WinUI;
using DXSharp.Graphics;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;

namespace DXSharp.CharacterLab.WinUI;

internal sealed class CharacterWindow : Window {
    private readonly TaskCompletionSource _completion = new( TaskCreationOptions.RunContinuationsAsynchronously );
    private readonly CharacterScene _scene;
    private readonly GraphicsDevice _device;
    private readonly int _limit;
    private bool _closing, _closed, _ready;
    internal D3D12Viewport Viewport { get; } = new( );
    internal Grid SceneArea { get; } = new( );
    internal TextBox Editor { get; } = new( ) { Text = "Edit character notes here", Margin = new( 0, 12, 0, 0 ) };
    internal Task Completion => _completion.Task;
    internal Exception? Failure { get; private set; }
    internal RenderStatistics Statistics => Viewport.Statistics;

    internal CharacterWindow( GraphicsDevice device, CharacterScene scene, int limit, bool testing = false ) {
        _device = device; _scene = scene; _limit = limit;
        Title = "DXSharp · Character Lab — WinUI 3 Composition";
        var root = new Grid { Padding = new( 18 ), Background = Brush( 0x0B, 0x11, 0x1D ), RequestedTheme = ElementTheme.Dark };
        root.RowDefinitions.Add( new( ) { Height = GridLength.Auto } ); root.RowDefinitions.Add( new( ) ); root.RowDefinitions.Add( new( ) { Height = GridLength.Auto } );
        var header = new StackPanel { Margin = new( 0, 0, 0, 16 ) };
        header.Children.Add( new TextBlock { Text = "ARKAEN / DXSHARP", Foreground = Brush( 0x64, 0xD8, 0xCD ), FontSize = 12 } );
        header.Children.Add( new TextBlock { Text = "CHARACTER LAB · WINUI 3", FontSize = 28 } ); root.Children.Add( header );
        var body = new Grid { ColumnSpacing = 16 };
        body.ColumnDefinitions.Add( new( ) { Width = new( 240 ) } ); body.ColumnDefinitions.Add( new( ) ); Grid.SetRow( body, 1 ); root.Children.Add( body );
        var controls = new StackPanel { Spacing = 12 };
        controls.Children.Add( new TextBlock { Text = "PLAYBACK", Foreground = Brush( 0x64, 0xD8, 0xCD ) } );
        controls.Children.Add( new TextBlock { Text = $"{scene.Model.SourceVertices.Length:N0} vertices\n{scene.Model.Skin.Joints.Length} joints · {scene.Model.Animations.Length} clips" } );
        var clips = new ComboBox { ItemsSource = scene.Model.Animations.Select( a => a.Name ).ToArray( ), SelectedIndex = scene.Model.FindAnimation( "Idle" ), HorizontalAlignment = HorizontalAlignment.Stretch };
        clips.SelectionChanged += ( _, _ ) => { if ( _ready && clips.SelectedIndex >= 0 ) scene.Player.Play( clips.SelectedIndex ); }; controls.Children.Add( clips );
        var pause = new CheckBox { Content = "Pause animation" };
        pause.Checked += ( _, _ ) => scene.Player.Paused = true; pause.Unchecked += ( _, _ ) => scene.Player.Paused = false; controls.Children.Add( pause );
        controls.Children.Add( new TextBlock { Text = "Playback speed" } );
        var speed = new Slider { Minimum = .1, Maximum = 2.5, Value = 1 };
        speed.ValueChanged += ( _, args ) => scene.Player.Speed = (float)args.NewValue; controls.Children.Add( speed );
        var reset = new Button { Content = "Reset camera", HorizontalAlignment = HorizontalAlignment.Stretch };
        reset.Click += ( _, _ ) => scene.Camera.Reset( ); controls.Children.Add( reset );
        controls.Children.Add( new TextBlock { Text = "Drag the scene to orbit; wheel or +/− to zoom. 1–9 changes clip; Space pauses. Editing below retains keyboard input.", TextWrapping = TextWrapping.Wrap } );
        controls.Children.Add( Editor ); body.Children.Add( controls );
        Grid.SetColumn( SceneArea, 1 ); body.Children.Add( SceneArea ); SceneArea.Children.Add( Viewport );
        var overlay = new Border { Background = Brush( 0x10, 0x27, 0x36 ), Padding = new( 12 ), Margin = new( 16 ),
            HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, IsHitTestVisible = false,
            Child = new TextBlock { Text = "LIVE COMPOSITION\nXAML overlay over a D3D12 swap chain", Foreground = Brush( 0x64, 0xD8, 0xCD ) } };
        SceneArea.Children.Add( overlay );
        var status = new TextBlock { Margin = new( 0, 12, 0, 0 ), Text = "Direct3D 12 · WinUI SwapChainPanel · precompiled shaders" }; Grid.SetRow( status, 2 ); root.Children.Add( status );
        Content = root; AppWindow.Resize( new SizeInt32( 1280, 820 ) );
        scene.Camera.InputEnabled = ( ) => Viewport.HasInputFocus;
        scene.Camera.PointerEnabled = ( ) => Viewport.AcceptsPointerInput;
        Viewport.WheelScrolled += delta => scene.Camera.Zoom( -delta / 120f * .3f );
        Viewport.RenderFailed += error => { Failure = error; if ( !testing ) CloseOrderly( ); };
        Viewport.FramePresented += statistics => {
            if ( scene.UpdateTitle( statistics.Frames ) is { } title ) Title = title;
            if ( statistics.Frames == 1 || statistics.Frames % 30 == 0 ) {
                var p = Viewport.PixelSize; var s = Viewport.CompositionScale;
                status.Text = $"{scene.Player.CurrentClipName} · {p.Width} × {p.Height} px · scale {s.X:F2}/{s.Y:F2} · CPU record {scene.MeanRecordingMilliseconds:F2} ms";
            }
            if ( _limit > 0 && statistics.Frames >= _limit ) CloseOrderly( );
        };
        AppWindow.Closing += ( _, args ) => { if ( !_closed ) { args.Cancel = true; CloseOrderly( ); } };
        Closed += ( _, _ ) => _completion.TrySetResult( );
        VisibilityChanged += ( _, args ) => Viewport.IsSuspended = !args.Visible;
        _ready = true;
        Restart( );
    }
    internal void Restart( Action<RenderFrame>? draw = null ) {
        Failure = null; Viewport.Start( _device, new( ) { EnableDepth = true, FramesInFlight = 2 }, draw ?? _scene.Draw, this );
    }
    internal async Task StopAsync( ) { await Viewport.StopAsync( ); if ( _closing ) Viewport.Dispose( ); }
    internal async void CloseOrderly( ) {
        if ( _closing ) return;
        _closing = true;
        try { await Viewport.StopAsync( ); Viewport.Dispose( ); }
        catch ( Exception error ) { Failure ??= error; }
        finally { _closed = true; Close( ); _completion.TrySetResult( ); }
    }
    private static SolidColorBrush Brush( byte r, byte g, byte b ) => new( Windows.UI.Color.FromArgb( 255, r, g, b ) );
}
