using System.Diagnostics;
using System.Numerics;
using DXSharp.Assets;
using DXSharp.Framework;
using DXSharp.Framework.Graphics;
using DXSharp.Framework.Hosting;
using DXSharp.Graphics;

namespace DXSharp.CharacterLab;

internal sealed class CharacterScene : IDisposable {
    private readonly SkinnedModelRenderer _renderer;
    private readonly SkeletonPose _pose;
    private readonly bool _bounded;
    private double _recordMilliseconds, _titleElapsed;
    internal SkinnedModel Model { get; }
    internal AnimationPlayer Player { get; }
    internal OrbitCamera Camera { get; } = new( );
    internal long Frames { get; private set; }
    internal double MeanRecordingMilliseconds => _recordMilliseconds / Math.Max( 1, Frames );

    internal CharacterScene( GraphicsDevice device, string modelPath, bool bounded ) {
        Model = SkinnedModel.Load( modelPath );
        _bounded = bounded;
        int root = Array.FindIndex( Model.Nodes, n => n.Name.Equals( "pelvis", StringComparison.OrdinalIgnoreCase ) );
        Player = new( Model, root ); _pose = new( Model.Nodes );
        _renderer = new( device, Model, 2 );
        Console.WriteLine( $"CharacterLab: {Model.SourceVertices.Length:N0} vertices, {Model.Skin.Joints.Length} joints, {Model.Animations.Length} clips." );
        Console.WriteLine( device.AdapterName );
        Console.WriteLine( "Precompiled DXIL; persistent GPU skinning; four material maps; two fenced frame slots." );
        Console.WriteLine( "Drag: orbit | +/-: zoom | 1-9: clip | Space: pause | R: idle | [ / ]: speed" );
    }
    internal void Draw( RenderFrame frame ) {
        long start = Stopwatch.GetTimestamp( );
        double dt = frame.Elapsed.TotalSeconds; Camera.Update( (float)dt );
        for ( int i = 0; i < Model.Animations.Length && i < 9; ++i ) if ( Camera.WasPressed( 0x31 + i ) ) Player.Play( i, loop: i < 5 );
        if ( Camera.WasPressed( 0x20 ) ) Player.Paused = !Player.Paused;
        if ( Camera.WasPressed( 0x52 ) ) Player.Play( Model.FindAnimation( "Idle" ) );
        if ( Camera.WasPressed( 0xDB ) ) Player.Speed = Math.Max( .1f, Player.Speed - .1f );
        if ( Camera.WasPressed( 0xDD ) ) Player.Speed = Math.Min( 2.5f, Player.Speed + .1f );
        if ( _bounded ) { Player.Play( (int)(Frames % Model.Animations.Length), 0 ); dt = .37; }
        Player.Update( dt ); Player.Sample( _pose );
        var view = Matrix4x4.CreateLookAt( Camera.Eye, new( 0, .92f, 0 ), Vector3.UnitY );
        var projection = Matrix4x4.CreatePerspectiveFieldOfView( MathF.PI / 3, Math.Max( 1, frame.Size.Width ) / (float)Math.Max( 1, frame.Size.Height ), .08f, 100 );
        _renderer.Draw( frame.Commands, _pose, Matrix4x4.Identity, view * projection, Camera.Eye, frame.FrameSlotIndex );
        _recordMilliseconds += Stopwatch.GetElapsedTime( start ).TotalMilliseconds; ++Frames; _titleElapsed += frame.Elapsed.TotalSeconds;
    }
    internal string? UpdateTitle( int frames ) {
        if ( frames % 60 != 0 ) return null;
        string title = $"DXSharp · Character Lab | {Player.CurrentClipName} | {60 / Math.Max( _titleElapsed, .001 ):F0} FPS | CPU record {MeanRecordingMilliseconds:F2} ms";
        _titleElapsed = 0; return title;
    }
    public void Dispose( ) => _renderer.Dispose( );
}
