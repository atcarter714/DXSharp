using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal sealed class FleetHud {
    private const uint Stride = 48;
    internal const uint BufferBytes = 384 * Stride;
    private static readonly Vector4 Panel = new( 0.029f, 0.052f, 0.091f, 1 );
    private static readonly Vector4 Label = new( 0.65f, 0.76f, 0.87f, 1 );
    private static readonly Vector4 White = new( 0.94f, 0.97f, 1.0f, 1 );
    private readonly Glyph[] _glyphs = new Glyph[BufferBytes / Stride];
    private int _count;

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
        int selected, int width, int height, bool oreShort ) {
        if ( width < 350 || height < 240 ) return;
        _count = 0;
        Span<char> text = stackalloc char[72];
        float right = width - 165;
        var siteTint = fleet.Contested ? SpaceDraw.Amber : fleet.Owner == 0 ? SpaceDraw.Cyan : fleet.Owner == 1 ? SpaceDraw.Amber : SpaceDraw.Neutral;
        Rect( 14, 14, 145, 55, Panel, 1 );
        Rect( 14, 14, 2, 55, SpaceDraw.Cyan );
        Text( "FLEET COMMAND", 23, 23, 1.6f, White );
        text.TryWrite( $"SQUAD {selected + 1:D2}   {fleet.ShipCount( selected )} SHIPS", out int length );
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

        float bottom = height - 60;
        Rect( 14, bottom, width - 28, 46, Panel, 1 );
        Rect( 14, bottom, 2, 46, SpaceDraw.Cyan );
        string instruction = fleet.WingsBuilt == FleetSimulation.WingGoal ? "OBJECTIVE COMPLETE"
            : oreShort && fleet.Ore < FleetSimulation.WingCost ? "NEED 60 ORE TO BUILD"
            : fleet.Ore >= FleetSimulation.WingCost ? "PRESS B TO LAUNCH A WING"
            : fleet.Owner == 0 ? "HOLD THE BEACON TO MINE ORE" : "SEND CYAN SQUADS TO THE BEACON";
        Text( instruction, 23, bottom + 8, 1.25f, fleet.WingsBuilt == FleetSimulation.WingGoal ? SpaceDraw.Cyan : White );
        Text( width >= 470 ? "L CLICK SELECT   R CLICK MOVE   B BUILD   WHEEL ZOOM   MID DRAG PAN"
            : "L CLICK SELECT   R CLICK MOVE   B BUILD   WHEEL ZOOM", 23, bottom + 29, 1, Label );

        upload.Write<Glyph>( _glyphs.AsSpan( 0, _count ) );
        commands.SetGraphicsPipeline( pipeline );
        commands.SetGraphicsConstants( [BitConverter.SingleToUInt32Bits( width ), BitConverter.SingleToUInt32Bits( height ), 0, 0] );
        commands.SetVertexBuffer( 0, upload, Stride );
        commands.Draw( 6, (uint)_count );
    }

    private void Rect( float x, float y, float width, float height, Vector4 color, uint layer = 2 ) =>
        _glyphs[_count++] = new( new( x, y, width, height ), color, uint.MaxValue, 7, layer, 0 );

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
                _glyphs[_count++] = new( new( x, y, 5 * scale, 7 * scale ), tint, (uint)bits, (uint)(bits >> 32), 0, 0 );
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
