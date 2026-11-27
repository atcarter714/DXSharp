using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Framework.Desktop;
using DXSharp.Graphics;
using DXSharp.Shaders;
using Windows.Win32.Graphics.Dxgi.Common;

namespace FleetBattler;

internal sealed class SceneBloom : IDisposable {
    private readonly GraphicsDevice _device;
    private readonly DescriptorPool _descriptors;
    private readonly DescriptorHeap _targets;
    private readonly PipelineState _extract, _reduce, _blur, _composite;
    private readonly record struct Surface( Texture2D Texture, RenderTargetView Target );
    private readonly Surface[] _levels = new Surface[4];
    private Texture2D? _scene;

    private const string Shader = """
        Texture2D Scene : register(t0);
        Texture2D NearGlow : register(t1);
        Texture2D WideGlow : register(t2);
        SamplerState LinearClamp : register(s0);
        cbuffer Settings : register(b0) { float4 Filter; float4 Emitter; };
        struct Surface { float4 position : SV_Position; float2 uv : TEXCOORD0; };
        Surface VS(uint vertex : SV_VertexID) {
            Surface result;
            result.uv = float2((vertex << 1) & 2, vertex & 2);
            result.position = float4(result.uv * float2(2,-2) + float2(-1,1), 0, 1);
            return result;
        }
        float3 ReadLinear(float2 uv) {
            return pow(max(Scene.SampleLevel(LinearClamp,uv,0).rgb,0),2.2);
        }
        float3 Highlight(float2 uv) {
            float3 color = ReadLinear(uv);
            float peak = max(color.r,max(color.g,color.b));
            float2 starOffset = (uv - Emitter.xy) * Emitter.zw;
            float star = 1 - smoothstep(.97,1.43,length(starOffset));
            float prominence = smoothstep(.18,.72,peak) * star;
            float spill = smoothstep(.49,.91,peak) * .16 * (1-star);
            return color * (prominence + spill);
        }
        float4 Extract(Surface input) : SV_Target {
            float2 offset = Filter.xy;
            float3 sum = Highlight(input.uv + offset) + Highlight(input.uv - offset) +
                Highlight(input.uv + offset*float2(1,-1)) + Highlight(input.uv + offset*float2(-1,1));
            return float4(sum*.25,1);
        }
        float4 Reduce(Surface input) : SV_Target {
            float2 d = Filter.xy;
            float4 sum = Scene.SampleLevel(LinearClamp,input.uv,0) * 4;
            sum += Scene.SampleLevel(LinearClamp,input.uv + d*float2(1,1),0);
            sum += Scene.SampleLevel(LinearClamp,input.uv + d*float2(-1,1),0);
            sum += Scene.SampleLevel(LinearClamp,input.uv + d*float2(1,-1),0);
            sum += Scene.SampleLevel(LinearClamp,input.uv - d,0);
            return sum*.125;
        }
        float4 Blur(Surface input) : SV_Target {
            float2 d = Filter.xy;
            float4 sum = Scene.SampleLevel(LinearClamp,input.uv,0) * .227027;
            sum += (Scene.SampleLevel(LinearClamp,input.uv + d*1.384615,0) +
                    Scene.SampleLevel(LinearClamp,input.uv - d*1.384615,0)) * .316216;
            sum += (Scene.SampleLevel(LinearClamp,input.uv + d*3.230769,0) +
                    Scene.SampleLevel(LinearClamp,input.uv - d*3.230769,0)) * .070270;
            return sum;
        }
        float4 Composite(Surface input) : SV_Target {
            float3 color = ReadLinear(input.uv);
            float3 near = NearGlow.SampleLevel(LinearClamp,input.uv,0).rgb;
            float3 wide = WideGlow.SampleLevel(LinearClamp,input.uv,0).rgb;
            float3 light = near*.34 + wide*.77;
            color += light * (1-saturate(color));
            return float4(pow(max(color,0),1.0/2.2),1);
        }
        """;

    internal SceneBloom( GraphicsDevice device, ShaderCompiler compiler ) {
        _device = device;
        using var root = device.CreateTextureRootSignature( 8, linearFiltering: true, wrap: false );
        using var compositeRoot = device.CreateTextureRootSignature( 4, 3, linearFiltering: true, wrap: false );
        var vertex = compiler.Compile( Shader, "VS", "vs_6_0" );
        _descriptors = device.CreateDescriptorPool( 8 );
        try {
            _targets = device.CreateRenderTargetHeap( 4 );
            _extract = CreatePass( "Extract" );
            _reduce = CreatePass( "Reduce" );
            _blur = CreatePass( "Blur" );
            _composite = device.CreateGraphicsPipeline( compositeRoot, vertex, compiler.Compile( Shader, "Composite", "ps_6_0" ) );
        } catch {
            Dispose();
            throw;
        }

        PipelineState CreatePass( string entry ) => device.CreateGraphicsPipeline( root, vertex,
            compiler.Compile( Shader, entry, "ps_6_0" ), PixelFormat.Rgba16Float );
    }

    internal void Apply( DesktopFrame frame, Vector4 star ) {
        uint width = (uint)frame.Size.Width, height = (uint)frame.Size.Height;
        var description = new Texture2DDescription( width, height, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM );
        if ( _scene?.Description != description ) Resize( description );
        var scene = _scene!;
        var commands = frame.Commands;
        commands.CopyRenderTarget( frame.Target, scene );
        commands.Transition( scene, ResourceState.CopyDestination, ResourceState.PixelShaderResource );

        Pass( commands, _extract, scene, _levels[0], ResourceState.CopyDestination, new( 1f / width, 1f / height ),
            new( star.X / width, star.Y / height, width / MathF.Max( star.Z, 1 ), height / MathF.Max( star.Z, 1 ) ) );
        Blur( commands, 0 );
        var narrow = _levels[0].Texture.Description;
        Pass( commands, _reduce, _levels[0].Texture, _levels[2], ResourceState.CopyDestination, new( 1.25f / narrow.Width, 1.25f / narrow.Height ) );
        Blur( commands, 2 );

        if ( frame.Depth is { } depth ) commands.SetRenderTarget( frame.Target, depth );
        else commands.SetRenderTarget( frame.Target );
        commands.SetViewport( width, height );
        commands.SetGraphicsPipeline( _composite );
        commands.SetGraphicsTextureTables( 1, [scene, _levels[0].Texture, _levels[2].Texture] );
        commands.Draw( 3 );
        commands.Transition( scene, ResourceState.PixelShaderResource, ResourceState.CopyDestination );
        foreach ( var level in _levels )
            commands.Transition( level.Texture, ResourceState.PixelShaderResource, ResourceState.CopyDestination );
    }

    private void Blur( GraphicsCommandList commands, int first ) {
        var result = _levels[first];
        var scratch = _levels[first + 1];
        var size = result.Texture.Description;
        Pass( commands, _blur, result.Texture, scratch, ResourceState.CopyDestination, new( 1.65f / size.Width, 0 ) );
        Pass( commands, _blur, scratch.Texture, result, ResourceState.PixelShaderResource, new( 0, 1.65f / size.Height ) );
    }

    private static void Pass( GraphicsCommandList commands, PipelineState pipeline, Texture2D input, Surface output,
        ResourceState before, Vector2 step, Vector4 emitter = default ) {
        var size = output.Texture.Description;
        commands.Transition( output.Texture, before, ResourceState.RenderTarget );
        commands.SetRenderTarget( output.Target );
        commands.SetViewport( size.Width, size.Height );
        commands.SetGraphicsPipeline( pipeline );
        ReadOnlySpan<Vector4> values = [new( step, 0, 0 ), emitter];
        commands.SetGraphicsConstants( MemoryMarshal.Cast<Vector4, uint>( values ) );
        commands.SetGraphicsTexture( input, 1 );
        commands.Draw( 3 );
        commands.Transition( output.Texture, ResourceState.RenderTarget, ResourceState.PixelShaderResource );
    }

    private void Resize( Texture2DDescription description ) {
        ReleaseSurfaces();
        try {
            _scene = _device.CreateTexture2D( description, _descriptors );
            for ( int i = 0; i < _levels.Length; ++i ) {
                uint divisor = i < 2 ? 4u : 14u;
                var texture = _device.CreateRenderTexture( new( Math.Max( 1, description.Width / divisor ),
                    Math.Max( 1, description.Height / divisor ), DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT ), _descriptors );
                _levels[i] = new( texture, default );
                _levels[i] = new( texture, _device.CreateRenderTargetView( texture, _targets, (uint)i ) );
            }
        } catch {
            ReleaseSurfaces();
            throw;
        }
    }

    private void ReleaseSurfaces() {
        _scene?.Dispose();
        _scene = null;
        foreach ( var level in _levels ) level.Texture?.Dispose();
        Array.Clear( _levels );
    }

    public void Dispose() {
        ReleaseSurfaces();
        _targets?.Dispose();
        _descriptors.Dispose();
        _composite?.Dispose();
        _blur?.Dispose();
        _reduce?.Dispose();
        _extract?.Dispose();
    }
}
