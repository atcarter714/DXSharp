using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;
using StbImageSharp;

namespace FleetBattler;

internal sealed class SpaceParticles : IDisposable {
    private const int Capacity = 8192;
    private const uint Stride = 48;
    private readonly record struct Particle( Vector4 Placement, Vector4 Tint, Vector4 Sprite );
    private readonly Particle[] _particles = new Particle[Capacity];
    private readonly GpuResource[] _uploads;
    private readonly PipelineState _pipeline;
    private readonly SampledTexture2D _atlas;
    private int _count;

    private const string Shader = """
        cbuffer Scene : register(b0) { float4 Viewport; };
        Texture2D Atlas : register(t0);
        SamplerState LinearClamp : register(s0);
        struct Input { float4 placement:POSITION; float4 tint:COLOR; float4 sprite:TEXCOORD; };
        struct Output {
            float4 position:SV_Position;
            float2 uv:TEXCOORD;
            nointerpolation float4 tint:COLOR;
            nointerpolation float2 frames:TEXCOORD1;
        };
        Output VS(Input input, uint vertex:SV_VertexID) {
            const float2 corners[6] = {float2(-1,-1),float2(1,-1),float2(-1,1),float2(-1,1),float2(1,-1),float2(1,1)};
            float2 uv = corners[vertex];
            float s,c;
            sincos(input.sprite.y,s,c);
            float2 corner = float2(c*uv.x-s*uv.y,s*uv.x+c*uv.y);
            float2 pixel = input.placement.xy + corner*input.placement.z;
            Output output;
            output.position = float4(pixel/Viewport.xy*float2(2,-2)+float2(-1,1),input.placement.w,1);
            output.uv = uv*.5+.5;
            output.tint = input.tint;
            output.frames = input.sprite.xz;
            return output;
        }
        float3 Frame(float2 uv, uint index) {
            float2 tile = float2(index % 4,index / 4);
            float4 sprite = Atlas.Sample(LinearClamp, (tile + lerp(.004,.996,uv))*.25);
            return sprite.rgb * sprite.a;
        }
        float4 PS(Output input):SV_Target {
            uint first = (uint)input.frames.x;
            float3 emission = Frame(input.uv,first);
            if (input.frames.y > 0) emission = lerp(emission,Frame(input.uv,min(first+1,8)),input.frames.y);
            return float4(emission*input.tint.rgb*input.tint.a,0);
        }
        """;

    internal SpaceParticles( GraphicsDevice device, GraphicsCommandList setup, ShaderCompiler compiler, int frameSlots ) {
        using var root = device.CreateTextureRootSignature( 4, linearFiltering: true, wrap: false );
        using var file = File.OpenRead( Path.Combine( AppContext.BaseDirectory, "Art", "Particles", "SpaceFx.png" ) );
        var pixels = ImageResult.FromStream( file, ColorComponents.RedGreenBlueAlpha );
        _atlas = device.CreateSampledTexture2D( setup, (uint)pixels.Width, (uint)pixels.Height, pixels.Data, srgb: false );
        _uploads = new GpuResource[frameSlots];
        try {
            _pipeline = device.CreateGraphicsPipeline( root, compiler.Compile( Shader, "VS", "vs_6_0" ),
                compiler.Compile( Shader, "PS", "ps_6_0" ), [
                    new( "POSITION", VertexFormat.Float4, 0, InstanceStepRate: 1 ),
                    new( "COLOR", VertexFormat.Float4, 16, InstanceStepRate: 1 ),
                    new( "TEXCOORD", VertexFormat.Float4, 32, InstanceStepRate: 1 )
                ], PixelFormat.Rgba8Unorm, new() { DepthEnabled = true, DepthWriteEnabled = false, DepthComparison = ComparisonFunction.Less }, additiveBlend: true );
            for ( int i = 0; i < _uploads.Length; ++i ) _uploads[i] = device.CreateBuffer( Capacity * Stride, BufferMemory.Upload );
        } catch { Dispose(); throw; }
    }

    internal void Begin( Vector4 star, Size size, float time ) {
        _count = 0;
        if ( star.Z < 14 || star.Z > size.Height * 2 ) return;
        for ( int i = 0; i < 18; ++i ) {
            float age = Frac( i * .61803f + time * (.26f + i % 5 * .031f) );
            float theta = i * 2.39996f + time * .027f + age * .18f;
            var (s, c) = MathF.SinCos( theta );
            var origin = new Vector2( star.X, star.Y ) + new Vector2( c, s ) * star.Z * (1.02f + age * .37f);
            Add( origin, star.Z * (.092f + .17f * age), .9998f, new( 1, .47f, .18f, .26f * Fade( age ) ), 11, theta );
        }
    }

    internal void Exhaust( ReadOnlySpan<ShipInstance> ships, ReadOnlySpan<ThrusterEmitter> emitters, Matrix4x4 view, Size size, float pixelsPerUnit, float time ) {
        foreach ( var ship in ships ) {
            float throttle = ship.Motion.X;
            if ( throttle < .08f ) continue;
            var style = (ExhaustStyle)(int)ship.Motion.Y;
            Vector3 color = style switch {
                ExhaustStyle.HighThrust => new( 1, .62f, .22f ), ExhaustStyle.Ion => new( .31f, .69f, 1 ),
                ExhaustStyle.Fusion => new( .77f, .44f, 1 ), _ => new( 1, .44f, .19f )
            };
            foreach ( var emitter in emitters ) {
                float seed = ship.Motion.Y + emitter.Position.X * .36f;
                int sparks = style == ExhaustStyle.HighThrust ? 7 : style == ExhaustStyle.Ion ? 3 : 5;
                for ( int i = 0; i < sparks; ++i ) {
                    float age = Frac( time * 1.85f + seed + (float)i / sparks );
                    var local = emitter.Position + emitter.Direction * (emitter.Width * (1 + 24 * throttle * age));
                    local.X += MathF.Sin( seed * 19 + i * 2.4f ) * emitter.Width * age;
                    local.Y += MathF.Cos( seed * 19 + i * 2.4f ) * emitter.Width * age;
                    var right = new Vector2( -ship.Heading.Y, ship.Heading.X );
                    var world = ship.Center + (right * local.X - ship.Heading * local.Z) * ship.Motion.Z;
                    var (pixel, depth) = Project( world, local.Y * ship.Motion.Z, view, size );
                    float radius = Math.Clamp( emitter.Width * ship.Motion.Z * pixelsPerUnit * (.62f + .85f * age), 1, 42 );
                    Add( pixel, radius, depth, new( color, throttle * Fade( age ) * .9f ), i % 3 == 1 ? 10 : 11, seed + age );
                }
            }
        }
    }

    internal void Explosions( ReadOnlySpan<BattleEffect> effects, Matrix4x4 view, Size size ) {
        foreach ( var effect in effects ) {
            if ( effect.Style.Z != 1 ) continue;
            var (pixel, depth) = Project( new( effect.Segment.X, effect.Segment.Y ), .75f, view, size );
            float age = effect.Style.X;
            float sprite = age * 8;
            float scale = Math.Clamp( size.Height / 720f, 1, 2.8f );
            Add( pixel, (5 + 32 * MathF.Sqrt( age )) * scale, depth, new( 1, .92f, .78f, (1 - age) * .92f ),
                (int)sprite, effect.Segment.X, Frac( sprite ) );
            Add( pixel, (8 + 19 * age) * scale, depth, new( 1, .59f, .28f, .45f * MathF.Pow( 1 - age, 4 ) ), 9, 0 );
        }
    }

    internal void Draw( DesktopFrame frame ) {
        if ( _count == 0 ) return;
        var commands = frame.Commands;
        var upload = _uploads[frame.FrameSlotIndex];
        upload.Write<Particle>( _particles.AsSpan( 0, _count ) );
        ReadOnlySpan<Vector4> viewport = [new( frame.Size.Width, frame.Size.Height, 0, 0 )];
        commands.SetGraphicsPipeline( _pipeline );
        commands.SetGraphicsConstants( MemoryMarshal.Cast<Vector4, uint>( viewport ) );
        commands.SetGraphicsTexture( _atlas, 1 );
        commands.SetVertexBuffer( 0, upload, Stride );
        commands.Draw( 6, (uint)_count );
    }

    private void Add( Vector2 pixel, float radius, float depth, Vector4 tint, int frame, float angle, float blend = 0 ) {
        if ( _count < Capacity )
            _particles[_count++] = new( new( pixel.X, pixel.Y, radius, depth ), tint, new( frame, angle, blend, 0 ) );
    }

    private static (Vector2 Pixel, float Depth) Project( Vector2 point, float height, Matrix4x4 view, Size size ) {
        var clip = Vector4.Transform( new Vector4( point.X, height, point.Y, 1 ), view );
        return (new( (.5f + clip.X / clip.W * .5f) * size.Width, (.5f - clip.Y / clip.W * .5f) * size.Height ), clip.Z / clip.W);
    }
    private static float Frac( float value ) => value - MathF.Floor( value );
    private static float Fade( float age ) => MathF.Min( age * 8, 1 ) * (1 - age) * (1 - age);

    public void Dispose() {
        foreach ( var upload in _uploads ) upload?.Dispose();
        _pipeline?.Dispose();
        _atlas.Dispose();
    }
}
