using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal sealed class FleetHud {
    private const uint Stride = 48;
    internal const uint BufferBytes = 512 * Stride;
    private static readonly Vector4 Panel = new( 0.029f, 0.052f, 0.091f, 1 );
    private static readonly Vector4 Label = new( 0.65f, 0.76f, 0.87f, 1 );
    private static readonly Vector4 White = new( 0.94f, 0.97f, 1.0f, 1 );
    private readonly Glyph[] _glyphs = new Glyph[BufferBytes / Stride];
    private int _count;
    private float _scale = 1;

    [StructLayout( LayoutKind.Sequential )]
    private readonly record struct Glyph( Vector4 Rectangle, Vector4 Tint, uint Low, uint High, uint Panel, uint Padding );

    private const string Shader = """
        cbuffer Hud : register(b0) { float4 Viewport; };
        struct Input { float4 rect : POSITION; float4 tint : COLOR; uint4 bits : TEXCOORD; };
        struct Output {
            float4 position : SV_Position;
            float2 uv : TEXCOORD0;
            nointerpolation uint2 bits : TEXCOORD1;
            nointerpolation float4 tint : COLOR;
        };
        Output VS(Input input, uint id : SV_VertexID) {
            const float2 corners[6] = {
                float2(0,0), float2(1,0), float2(0,1),
                float2(0,1), float2(1,0), float2(1,1)
            };
            Output output;
            output.uv = corners[id];
            float2 pixel = input.rect.xy + output.uv * input.rect.zw;
            output.position = float4(pixel/Viewport.xy*float2(2,-2)+float2(-1,1), input.bits.z == 1 ? 0.04 : input.bits.z == 2 ? 0.025 : 0.01, 1);
            output.bits = input.bits.xy;
            output.tint = input.tint;
            return output;
        }
        float4 PS(Output input) : SV_Target {
            uint2 cell = min(uint2(input.uv*float2(5,7)), uint2(4,6));
            uint index = cell.x + cell.y*5;
            uint bits = index < 32 ? input.bits.x : input.bits.y;
            clip((float)((bits >> (index & 31)) & 1) - 0.5);
            return input.tint;
        }
        """;

    internal static PipelineState CreatePipeline( GraphicsDevice device, RootSignature root, ShaderCompiler compiler ) =>
        device.CreateGraphicsPipeline( root, compiler.Compile( Shader, "VS", "vs_6_0" ),
            compiler.Compile( Shader, "PS", "ps_6_0" ), [
                new( "POSITION", VertexFormat.Float4, 0, InstanceStepRate: 1 ),
                new( "COLOR", VertexFormat.Float4, 16, InstanceStepRate: 1 ),
                new( "TEXCOORD", VertexFormat.UInt4, 32, InstanceStepRate: 1 )
            ], depthEnabled: true );

    internal void Record( GraphicsCommandList commands, PipelineState pipeline, GpuResource upload, FleetSimulation fleet,
        FleetSelection selected, int width, int height, bool oreShort, ReadOnlySpan<Vector4> bodies = default ) {
        if ( width < 350 || height < 240 ) return;
        Begin( width, height );
        float logicalWidth = width / _scale, logicalHeight = height / _scale;
        Span<char> text = stackalloc char[72];
        float right = logicalWidth - 165;
        var siteTint = fleet.Contested ? SpaceDraw.Amber : fleet.Owner == 0 ? SpaceDraw.Cyan : fleet.Owner == 1 ? SpaceDraw.Amber : SpaceDraw.Neutral;
        Rect( 14, 14, 145, 55, Panel, 1 );
        Rect( 14, 14, 2, 55, SpaceDraw.Cyan );
        Text( "FLEET COMMAND", 23, 23, 1.6f, White );
        int length;
        if ( selected.IsShip ) text.TryWrite( $"SHIP {selected.Ship + 1:D2}    SQUAD {selected.Squad + 1:D2}", out length );
        else text.TryWrite( $"SQUAD {selected.Squad + 1:D2}   {fleet.ShipCount( selected.Squad )} SHIPS", out length );
        Text( text[..length], 23, 49, 1, SpaceDraw.Cyan );

        Rect( right, 14, 151, 80, Panel, 1 );
        Rect( right, 14, 2, 80, siteTint );
        Text( "MINING BEACON", right + 9, 23, 1.35f, White );
        var site = fleet.Contested ? "CONTESTED" : fleet.Capturing == 0 ? "CYAN CAPTURING"
            : fleet.Capturing == 1 ? "AMBER CAPTURING" : fleet.Owner == 0 ? "CYAN CONTROL"
            : fleet.Owner == 1 ? "AMBER CONTROL" : "NEUTRAL";
        Text( site, right + 9, 42, 1, siteTint );
        text.TryWrite( $"ORE {fleet.Ore:D3}    WINGS {fleet.WingsBuilt}/{FleetSimulation.WingGoal}", out length );
        Text( text[..length], right + 9, 62, 1, Label );
        float fraction = fleet.CaptureTicks > 0 ? (float)fleet.CaptureTicks / FleetSimulation.CaptureDuration
            : Math.Clamp( (float)fleet.Ore / FleetSimulation.WingCost, 0, 1 );
        if ( fraction > 0 ) Rect( right + 9, 78, 130 * fraction, 2, siteTint );

        for ( int i = 0; i < bodies.Length; ++i ) {
            var body = bodies[i] / _scale;
            var name = CelestialSystem.Name( fleet.SystemIndex, i );
            float x = body.X - name.Length * 3, y = body.Y + body.Z + 9;
            if ( x < 0 || x + name.Length * 6 > logicalWidth || y < 3 || y > logicalHeight - 75 ) continue;
            Rect( x - 4, y - 3, name.Length * 6 + 8, 13, Panel, 1 );
            Text( name, x, y, 1, i == 0 ? SpaceDraw.Amber : Label );
        }

        float bottom = logicalHeight - 68;
        Rect( 14, bottom, logicalWidth - 28, 54, Panel, 1 );
        Rect( 14, bottom, 2, 54, SpaceDraw.Cyan );
        string instruction = fleet.Orbiting( selected ) ? "ORBIT ORDER ACTIVE" : fleet.WingsBuilt == FleetSimulation.WingGoal ? "OBJECTIVE COMPLETE"
            : oreShort && fleet.Ore < FleetSimulation.WingCost ? "NEED 60 ORE TO BUILD"
            : fleet.Ore >= FleetSimulation.WingCost ? "PRESS B TO LAUNCH A WING"
            : fleet.Owner == 0 ? "HOLD THE BEACON TO MINE ORE" : "SEND CYAN SQUADS TO THE BEACON";
        Text( instruction, 23, bottom + 8, 1.25f, fleet.WingsBuilt == FleetSimulation.WingGoal ? SpaceDraw.Cyan : White );
        Text( logicalWidth >= 460 ? "L CLICK SHIP   SHIFT CLICK SQUAD   R CLICK MOVE OR ORBIT"
            : "L CLICK SHIP   SHIFT GROUP   R CLICK MOVE", 23, bottom + 27, 1, Label );
        Text( logicalWidth >= 460 ? "G GALAXY   F11 FULLSCREEN   B BUILD   R REJOIN   WHEEL ZOOM"
            : "G MAP  F11 FULLSCREEN  B BUILD  R REJOIN", 23, bottom + 39, 1, Label );
        Submit( commands, pipeline, upload, width, height );
    }

    internal void RecordGalaxy( GraphicsCommandList commands, PipelineState pipeline, GpuResource upload,
        int currentSystem, int width, int height ) {
        if ( width < 350 || height < 240 ) return;
        Begin( width, height );
        float logicalWidth = width / _scale, logicalHeight = height / _scale;
        Rect( 14, 14, 182, 55, Panel, 1 );
        Rect( 14, 14, 2, 55, SpaceDraw.Cyan );
        Text( "GALAXY MAP", 23, 23, 1.6f, White );
        Text( "SELECT A STAR SYSTEM", 23, 47, 1, Label );
        for ( int i = 0; i < CelestialSystem.Systems.Length; ++i ) {
            var center = CelestialSystem.MapPositions[i] * new Vector2( logicalWidth, logicalHeight );
            string name = CelestialSystem.Systems[i];
            float left = center.X - name.Length * 4;
            Rect( left - 6, center.Y + 19, name.Length * 8 + 12, 20, Panel, 1 );
            Text( name, left, center.Y + 24, 1.3f, i == currentSystem ? SpaceDraw.Cyan : White );
        }
        Rect( 14, logicalHeight - 43, logicalWidth - 28, 29, Panel, 1 );
        Text( "CLICK SYSTEM TO ENTER   G RETURN   F11 FULLSCREEN", 23, logicalHeight - 33, 1, Label );
        Submit( commands, pipeline, upload, width, height );
    }

    private void Begin( int width, int height ) {
        _count = 0;
        _scale = Math.Clamp( MathF.Min( height / 640f, width / 700f ), 1, 3 );
    }

    private void Submit( GraphicsCommandList commands, PipelineState pipeline, GpuResource upload, int width, int height ) {
        upload.Write<Glyph>( _glyphs.AsSpan( 0, _count ) );
        commands.SetGraphicsPipeline( pipeline );
        commands.SetGraphicsConstants( [BitConverter.SingleToUInt32Bits( width ), BitConverter.SingleToUInt32Bits( height ), 0, 0] );
        commands.SetVertexBuffer( 0, upload, Stride );
        commands.Draw( 6, (uint)_count );
    }

    private void Rect( float x, float y, float width, float height, Vector4 color, uint layer = 2 ) =>
        _glyphs[_count++] = new( new Vector4( x, y, width, height ) * _scale, color, uint.MaxValue, 7, layer, 0 );

    private void Text( ReadOnlySpan<char> text, float x, float y, float scale, Vector4 tint ) {
        foreach ( char c in text ) {
            uint[]? rows = null;
            int index = c switch {
                >= 'A' and <= 'Z' => c - 'A',
                >= '0' and <= '9' => c - '0' + 26,
                '/' => 36, '-' => 37, _ => -1
            };
            if ( index >= 0 ) rows = Font[index];
            if ( rows is not null ) {
                ulong bits = 0;
                for ( int row = 0; row < 7; ++row ) bits |= (ulong)rows[row] << (row * 5);
                _glyphs[_count++] = new( new Vector4( x, y, 5 * scale, 7 * scale ) * _scale, tint, (uint)bits, (uint)(bits >> 32), 0, 0 );
            }
            x += 6 * scale;
        }
    }

    // Rows are stored with the leftmost pixel in the low bit.
    private static readonly uint[][] Font = [
        [14,17,17,31,17,17,17], [15,17,17,15,17,17,15], [30,1,1,1,1,1,30],
        [15,17,17,17,17,17,15], [31,1,1,15,1,1,31], [31,1,1,15,1,1,1],
        [30,1,1,29,17,17,30], [17,17,17,31,17,17,17], [31,4,4,4,4,4,31],
        [28,8,8,8,9,9,6], [17,9,5,3,5,9,17], [1,1,1,1,1,1,31],
        [17,27,21,21,17,17,17], [17,19,21,25,17,17,17], [14,17,17,17,17,17,14],
        [15,17,17,15,1,1,1], [14,17,17,17,21,9,22], [15,17,17,15,5,9,17],
        [30,1,1,14,16,16,15], [31,4,4,4,4,4,4], [17,17,17,17,17,17,14],
        [17,17,17,17,17,10,4], [17,17,17,21,21,27,17], [17,17,10,4,10,17,17],
        [17,17,10,4,4,4,4], [31,16,8,4,2,1,31],
        [14,17,25,21,19,17,14], [4,6,4,4,4,4,14], [14,17,16,8,4,2,31],
        [31,16,8,12,16,17,14], [8,12,10,9,31,8,8], [31,1,1,15,16,16,15],
        [12,2,1,15,17,17,14], [31,16,8,4,2,2,2], [14,17,17,14,17,17,14],
        [14,17,17,30,16,8,6], [16,16,8,4,2,1,1], [0,0,0,31,0,0,0]
    ];
}
