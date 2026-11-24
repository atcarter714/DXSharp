using System.Diagnostics;
using DXSharp.Media;
using Windows.Graphics;

namespace DXSharp.VideoPlayer;

internal static class PlaybackQualification {
    internal static async Task Run(PlayerWindow window, string source) {
        var player = window.Player;
        await Until(() => player.IsReady && window.VideoFrames >= 3,player);
        Require(Math.Abs(player.Duration.TotalSeconds - 6) < .2 && player.HasVideo,"Fixture duration/video stream is incorrect.");
        var frame = window.LatestFrame; Require(frame.Width == 320 && frame.Height == 180 && frame.RowPitch == 1280,"Incorrect BGRA frame geometry.");
        int colored = 0; var pixels = frame.Pixels.ToArray();
        for (int i=0;i<pixels.Length;i+=4) if (Math.Max(pixels[i],Math.Max(pixels[i+1],pixels[i+2])) - Math.Min(pixels[i],Math.Min(pixels[i+1],pixels[i+2])) > 70) ++colored;
        Require(colored > 5000,"Decoded color pattern is missing or channel data are blank.");
        player.Pause(); await Task.Delay(80); var paused = player.Position; await Task.Delay(180);
        Require(player.IsPaused && Math.Abs((player.Position-paused).TotalMilliseconds)<50,"Paused presentation clock advanced.");
        player.Volume=.25; player.Muted=true; Require(player.Muted && Math.Abs(player.Volume-.25)<.001,"Volume/mute state failed."); player.Muted=false;
        long generation=player.Generation; player.Seek(TimeSpan.FromSeconds(2));
        await Until(() => !player.IsSeeking && Math.Abs(player.Position.TotalSeconds-2)<.2,player);
        Require(player.Generation>generation,"Seek did not invalidate old frame generation.");
        player.Play(); long count=window.VideoFrames;
        await Until(() => window.VideoFrames>count+2 && window.LatestFrame.Generation==player.Generation,player);
        Require(Math.Abs(player.Statistics.PresentationOffset.TotalSeconds)<.5,"Video PTS is far from Media Engine clock.");
        window.AppWindow.Resize(new SizeInt32(840,660)); await Until(() => window.Statistics.Resizes>0,player);
        Require(window.Viewport.PixelSize.Width>0,"Resize lost the presentation surface.");
        player.Loop=false; player.Seek(player.Duration-TimeSpan.FromMilliseconds(150)); player.Play(); await Until(() => player.IsEnded,player);
        player.Loop=true; player.Seek(player.Duration-TimeSpan.FromMilliseconds(120)); player.Play(); await Until(() => player.Position.TotalSeconds<1 && !player.IsSeeking,player);
        Require(!player.IsEnded,"Loop did not restart playback.");
        for (int i=0;i<3;++i) {
            generation=player.Generation; window.Open(source);
            await Until(() => player.IsReady && window.LatestFrame.Generation>generation,player);
        }
        bool missing=false; try { player.Open(Path.Combine(Path.GetDirectoryName(source)!,"missing-file.mp4")); } catch(FileNotFoundException) { missing=true; }
        Require(missing,"Missing media path was accepted.");
        Console.WriteLine("PASS: Media Foundation BGRA colors/duration, presentation clock, pause, seek/generation, mute/volume, resize, end-of-stream, loop, repeated open and missing file.");
        window.CloseOrderly(); await window.Completion.WaitAsync(TimeSpan.FromSeconds(20));
        int presented=window.Statistics.Frames; await Task.Delay(120); Require(presented==window.Statistics.Frames,"Closed player retained presentation callbacks.");
        for (int i=0;i<2;++i) {
            using var reopened = new MediaPlayer(); reopened.Open(source); reopened.Play();
            await Until(() => reopened.TryGetFrame(out _),reopened);
        }
        player.Dispose(); bool disposed=false;
        try { player.Play(); } catch(ObjectDisposedException) { disposed=true; }
        Require(disposed,"Disposed player accepted playback."); player.Dispose();
        Console.WriteLine("PASS: video player drained shutdown and inert post-close callbacks. Audio-less CI fixture; audible A/V synchronization requires hardware review.");
    }
    private static async Task Until(Func<bool> predicate,MediaPlayer player) {
        var watch=Stopwatch.StartNew();
        while(!predicate()) { player.ThrowIfFailed(); if(watch.Elapsed>TimeSpan.FromSeconds(20)) throw new TimeoutException("Media qualification timed out."); await Task.Delay(10); }
    }
    private static void Require(bool condition,string message) { if(!condition) throw new InvalidOperationException(message); }
}
