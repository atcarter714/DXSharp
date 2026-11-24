using DXSharp.Framework.Hosting;
using DXSharp.Framework.WinUI;
using DXSharp.Graphics;
using DXSharp.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;
using Windows.Storage.Pickers;
using System.Runtime.InteropServices;

namespace DXSharp.VideoPlayer;

internal sealed class PlayerWindow : Window {
    private readonly TaskCompletionSource _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly VideoTextureRenderer _renderer;
    private readonly TextBlock _status = new() { TextWrapping = TextWrapping.Wrap, FontSize = 12 };
    private readonly TextBlock _source = new() { TextTrimming = TextTrimming.CharacterEllipsis, FontSize = 14 };
    private readonly Slider _timeline = new() { Minimum = 0, Maximum = 1, HorizontalAlignment = HorizontalAlignment.Stretch };
    private readonly int _limit;
    private bool _updating, _closing, _closed, _mediaError;
    internal D3D12Viewport Viewport { get; } = new();
    internal MediaPlayer Player { get; }
    internal Task Completion => _completion.Task;
    internal RenderStatistics Statistics => Viewport.Statistics;
    internal long VideoFrames => _renderer.FramesTransferred;
    internal VideoFrame LatestFrame => _renderer.LatestFrame;
    internal Exception? Failure { get; private set; }

    internal PlayerWindow(GraphicsDevice device, MediaPlayer player, string file, int limit) {
        Player = player; _limit = limit; _renderer = new(device, player);
        Title = "DXSharp · Media Lab";
        var root = new Grid { Padding = new(20), RowSpacing = 12, RequestedTheme = ElementTheme.Dark, Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255,10,16,26)) };
        root.RowDefinitions.Add(new() { Height = GridLength.Auto }); root.RowDefinitions.Add(new());
        root.RowDefinitions.Add(new() { Height = GridLength.Auto }); root.RowDefinitions.Add(new() { Height = GridLength.Auto }); root.RowDefinitions.Add(new() { Height = GridLength.Auto });
        var title = new StackPanel { Spacing = 4 };
        title.Children.Add(new TextBlock { Text = "ARKAEN / DXSHARP · MEDIA LAB", FontSize = 24 }); title.Children.Add(_source); root.Children.Add(title);
        var scene = new Grid(); scene.Children.Add(Viewport); Grid.SetRow(scene,1); root.Children.Add(scene);
        var badge = new Border { IsHitTestVisible = false, Background = new SolidColorBrush(Windows.UI.Color.FromArgb(220,12,30,40)), Padding = new(10), Margin = new(12), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top,
            Child = new TextBlock { Text = "MEDIA FOUNDATION → DXSHARP D3D12\nFrame-server playback · precompiled shaders", FontSize = 12 } }; scene.Children.Add(badge);
        Grid.SetRow(_timeline,2); root.Children.Add(_timeline);
        _timeline.ValueChanged += (_,e) => { if (!_updating && Player.IsReady) TryAction(() => Player.Seek(TimeSpan.FromSeconds(e.NewValue))); };
        var commands = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        var open = new Button { Content = "Open file…" }; open.Click += async (_,_) => {
            try {
                var picker = new FileOpenPicker(); WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(this));
                foreach (string extension in new[] { ".mp4", ".wmv", ".avi", ".mov", ".mp3", ".wav" }) picker.FileTypeFilter.Add(extension);
                var selected = await picker.PickSingleFileAsync(); if (selected is not null && !_closing) Open(selected.Path);
            } catch (Exception error) { _status.Text = error.Message; }
        }; commands.Children.Add(open);
        var play = new Button { Content = "Play / pause" }; play.Click += (_,_) => TryAction(() => { if (Player.IsPaused) Player.Play(); else Player.Pause(); }); commands.Children.Add(play);
        var restart = new Button { Content = "Restart" }; restart.Click += (_,_) => TryAction(() => { Player.Seek(TimeSpan.Zero); Player.Play(); }); commands.Children.Add(restart);
        var loop = new CheckBox { Content = "Loop" }; loop.Checked += (_,_) => TryAction(() => Player.Loop = true); loop.Unchecked += (_,_) => TryAction(() => Player.Loop = false); commands.Children.Add(loop);
        var mute = new CheckBox { Content = "Mute" }; mute.Checked += (_,_) => TryAction(() => Player.Muted = true); mute.Unchecked += (_,_) => TryAction(() => Player.Muted = false); commands.Children.Add(mute);
        var volume = new Slider { Minimum = 0, Maximum = 1, Value = .65, Width = 130 }; volume.ValueChanged += (_,e) => TryAction(() => Player.Volume = e.NewValue);
        commands.Children.Add(new TextBlock { Text = "Volume", VerticalAlignment = VerticalAlignment.Center }); commands.Children.Add(volume);
        var controlScroll = new ScrollViewer { Content = commands, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollMode = ScrollMode.Enabled, VerticalScrollMode = ScrollMode.Disabled };
        Grid.SetRow(controlScroll,3); root.Children.Add(controlScroll); Grid.SetRow(_status,4); root.Children.Add(_status);
        Content = root; AppWindow.Resize(new SizeInt32(1180,780));
        Open(file); Player.Volume = .65;
        Viewport.RenderFailed += error => { Failure = error; CloseOrderly(); };
        Viewport.FramePresented += stats => {
            if (stats.Frames % 12 == 0 && !_mediaError) {
                _updating = true;
                try { _timeline.Maximum = Math.Max(1,Player.Duration.TotalSeconds); _timeline.Value = Math.Min(_timeline.Maximum,Player.Position.TotalSeconds); }
                finally { _updating = false; }
                var s = Player.Statistics; var f = LatestFrame;
                _status.Text = $"{Player.Position:mm\\:ss\\.fff} / {Player.Duration:mm\\:ss} · {(Player.IsPaused ? "PAUSED" : Player.IsEnded ? "ENDED" : "PLAYING")} · {f.Width}×{f.Height} · audio {(Player.HasAudio ? "present" : "none")}\n" +
                    $"Transferred {s.FramesTransferred} · dropped {s.FramesDropped?.ToString() ?? "unavailable"} · video PTS − A/V clock {s.PresentationOffset.TotalMilliseconds:+0.0;-0.0;0.0} ms · hardware decode unmeasured\n{s.TransferPath}";
            }
            if (_limit > 0 && stats.Frames >= _limit && VideoFrames > 1) CloseOrderly();
        };
        Viewport.Start(device,new() { FramesInFlight = 2 },frame => {
            if (_mediaError) return;
            try { _renderer.Draw(frame); }
            catch (COMException error) { _mediaError = true; Console.Error.WriteLine(error); _status.Text = $"Playback error: {error.Message}"; Failure = error; }
        },this);
        AppWindow.Closing += (_,e) => { if (!_closed) { e.Cancel = true; CloseOrderly(); } };
        Closed += (_,_) => _completion.TrySetResult();
        VisibilityChanged += (_,e) => Viewport.IsSuspended = !e.Visible;
    }
    private void TryAction(Action action) { try { action(); } catch (Exception error) { _status.Text = error.Message; } }
    internal void Open(string path) { Player.Open(path); Failure = null; _mediaError = false; _source.Text = path; Player.Play(); }
    internal async Task StopAsync() { await Viewport.StopAsync(); Viewport.Dispose(); _renderer.Dispose(); }
    internal async void CloseOrderly() {
        if (_closing) return; _closing = true;
        try { await StopAsync(); }
        catch (Exception error) { Failure ??= error; }
        finally { _closed = true; Close(); _completion.TrySetResult(); }
    }
}
