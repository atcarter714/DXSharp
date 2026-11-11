using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal enum PlanetAction { None, Survey, Mine, Lab, Research }
internal enum HudAction { None, Panel, Pause, Speed, Galaxy, Wing, Squad, Planet }
internal readonly record struct HudHit( HudAction Action, int Index = 0 );

internal sealed class FleetHud {
    private const uint Stride = 48;
    internal const uint BufferBytes = 1024 * Stride;
    private static readonly Vector4 Panel = new( 0.025f, 0.049f, 0.083f, 1 );
    private static readonly Vector4 Well = new( 0.066f, 0.125f, 0.19f, 1 );
    private static readonly Vector4 Active = new( 0.093f, 0.23f, 0.32f, 1 );
    private static readonly Vector4 Disabled = new( 0.52f, 0.63f, 0.73f, 1 );
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
            output.position = float4(pixel/Viewport.xy*float2(2,-2)+float2(-1,1), input.bits.z == 1 ? 0.04 : input.bits.z == 2 ? 0.025 : input.bits.z == 3 ? 0.018 : 0.01, 1);
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
        FleetSelection selected, int width, int height, bool oreShort, ReadOnlySpan<Vector4> bodies = default,
        int selectionCount = -1, int planet = -1, string? notice = null, Vector4? drag = null, Vector2? beacon = null,
        ReadOnlySpan<bool> selectedShips = default, int speed = 1, bool paused = false ) {
        if ( width < 350 || height < 240 ) return;
        Begin( width, height );
        var layout = new Layout( width / _scale, height / _scale );
        Span<char> text = stackalloc char[72];
        var siteTint = fleet.Contested || fleet.Owner == 1 ? SpaceDraw.Amber : fleet.Owner == 0 ? SpaceDraw.Cyan : Label;
        var site = fleet.Contested ? "CONTESTED" : fleet.Capturing == 0 ? "CYAN CAPTURING"
            : fleet.Capturing == 1 ? "AMBER CAPTURING" : fleet.Owner == 0 ? "CYAN CONTROL"
            : fleet.Owner == 1 ? "AMBER CONTROL" : "UNCLAIMED";

        PanelAt( 14, 12, layout.Width - 28, 41, SpaceDraw.Cyan );
        Text( CelestialSystem.Systems[fleet.SystemIndex], 23, 20, 1.9f, White );
        text.TryWrite( $"ORE {fleet.Ore:D3}   SCI {fleet.Science:D3}", out int length );
        Text( text[..length], 23, 39, 1.05f, Label );
        Button( layout.ClockX, 19, 64, paused ? "RESUME" : "PAUSE", true, paused );
        text.TryWrite( $"{speed}X", out length );
        Button( layout.ClockX + 70, 19, 36, text[..length], true );
        Button( layout.ClockX + 112, 19, 66, "GALAXY", true );

        PanelAt( 14, 63, 157, layout.OutlinerHeight( fleet ), SpaceDraw.Cyan );
        Text( "FLEET OUTLINER", 23, 72, 1.35f, White );
        text.TryWrite( $"{fleet.CyanCount} CYAN   {fleet.AmberCount} AMBER", out length );
        Text( text[..length], 23, 89, 1.05f, Label );
        int row = 0;
        for ( int squad = 0; squad < fleet.SquadCount && row < layout.MaxRows; ++squad ) {
            if ( !fleet.IsPlayerSquad( squad ) ) continue;
            int alive = fleet.ShipCount( squad );
            text.TryWrite( $"{squad + 1:D2} {(fleet.IsScoutSquad( squad ) ? "SCOUT" : "CORVETTE")}  {alive:D2}", out length );
            bool active = selectionCount != 0 && selected.Squad == squad;
            if ( !selectedShips.IsEmpty ) active |= fleet.AnySelectedInSquad( squad, selectedShips );
            Button( 23, 106 + row++ * 24, 139, text[..length], alive != 0, active );
        }

        float right = layout.Right;
        PanelAt( right, 63, 171, 76, siteTint );
        Text( "MINING RELAY", right + 9, 72, 1.35f, White );
        Text( site, right + 9, 89, 1.1f, siteTint );
        float fraction = fleet.CaptureTicks > 0 ? (float)fleet.CaptureTicks / FleetSimulation.CaptureDuration
            : fleet.Owner >= 0 ? 1 : 0;
        Rect( right + 9, 108, 153, 2, Well );
        if ( fraction > 0 ) Rect( right + 9, 108, 153 * fraction, 2, siteTint );
        text.TryWrite( $"WINGS {fleet.WingsBuilt}/{FleetSimulation.WingGoal}  AMBER {fleet.AmberCount}", out length );
        Text( text[..length], right + 9, 120, 1.05f, Label );

        for ( int i = 0; i < bodies.Length; ++i ) {
            var body = bodies[i] / _scale;
            var name = CelestialSystem.Name( fleet.SystemIndex, i );
            float textWidth = TextWidth( name.Length, 1 );
            float x = body.X - textWidth / 2, y = body.Y + body.Z + 9;
            if ( x < 0 || x + textWidth > layout.Width || y < 3 || y > layout.Bottom - 12 ||
                layout.Blocks( new( body.X, y ), fleet, planet ) ) continue;
            Rect( x - 4, y - 3, textWidth + 8, 7 * GlyphPitch( 1 ) / _scale + 6, Panel, 1 );
            Text( name, x, y, 1, i == planet ? SpaceDraw.Cyan : i == 0 ? SpaceDraw.Amber : Label );
            if ( i > 0 ) {
                var status = fleet.Planet( i );
                if ( status.MineRemaining >= 0 ) FacilityMarker( body.X - body.Z - 6, body.Y - 4, status.MineRemaining, SpaceDraw.Amber );
                if ( status.LabRemaining >= 0 ) FacilityMarker( body.X + body.Z + 1, body.Y - 4, status.LabRemaining, SpaceDraw.Cyan );
            }
        }

        if ( beacon is { } beaconPixel ) {
            var p = beaconPixel / _scale;
            if ( p.X >= 30 && p.X <= layout.Width - 30 && p.Y > 16 && p.Y < layout.Bottom - 15 && !layout.Blocks( p, fleet, planet ) ) {
                Rect( p.X - 22, p.Y + 6, 44, 12, Panel, 1 );
                Text( "RELAY", p.X - TextWidth( 5, 1 ) / 2, p.Y + 9, 1, siteTint );
            }
        }

        if ( planet >= 0 && layout.PlanetVisible ) PlanetPanel( fleet, planet, right, selectionCount );
        else if ( layout.PlanetVisible ) {
            PanelAt( right, 147, 171, 64, SpaceDraw.Cyan );
            Text( "SELECTION", right + 9, 156, 1.35f, White );
            if ( selectionCount == 0 ) text.TryWrite( $"NO SHIPS SELECTED", out length );
            else if ( selected.IsShip ) text.TryWrite( $"SHIP {selected.Ship + 1:D2}  HULL {fleet.Hull( selected.Ship )}/{FleetSimulation.HullCapacity}", out length );
            else text.TryWrite( $"{(selectionCount < 0 ? fleet.ShipCount( selected.Squad ) : selectionCount)} SHIPS SELECTED", out length );
            Text( text[..length], right + 9, 174, 1.1f, Label );
            Text( "CLICK A WORLD TO DEVELOP", right + 9, 192, 1, Label );
        }

        if ( drag is { } box ) {
            box /= _scale;
            float w = box.Z - box.X, h = box.W - box.Y;
            Rect( box.X, box.Y, w, 1, SpaceDraw.Cyan );
            Rect( box.X, box.W, w, 1, SpaceDraw.Cyan );
            Rect( box.X, box.Y, 1, h, SpaceDraw.Cyan );
            Rect( box.Z, box.Y, 1, h, SpaceDraw.Cyan );
        }

        PanelAt( 14, layout.Bottom, layout.Width - 28, 50, SpaceDraw.Cyan );
        string instruction = notice ?? (fleet.Lost ? "DEFEAT / CYAN FLEET LOST" : fleet.Won
            ? fleet.AmberCount == 0 ? "VICTORY / AMBER DEFEATED" : "VICTORY / WING GOAL MET"
            : fleet.InBattle ? "ENGAGEMENT / RIGHT CLICK AMBER TO ATTACK"
            : selectionCount != 0 && fleet.Orbiting( selected ) ? "ORBIT ORDER ACTIVE"
            : oreShort && fleet.Ore < FleetSimulation.WingCost ? "NEED 60 ORE TO BUILD"
            : fleet.Owner == 0 ? "HOLD THE RELAY TO MINE ORE" : "CAPTURE THE RELAY OR DEFEAT AMBER");
        float instructionScale = layout.Width < 530 ? 1 : 1.16f;
        int instructionCapacity = (int)((layout.Width - 159) / TextWidth( 1, instructionScale ));
        Text( instruction.AsSpan( 0, Math.Min( instruction.Length, instructionCapacity ) ), 23, layout.Bottom + 9, instructionScale,
            fleet.Won ? SpaceDraw.Cyan : fleet.Lost ? SpaceDraw.Amber : White );
        Text( layout.Width >= 585 ? "DRAG SELECT  /  SHIFT ADD  /  CTRL TOGGLE  /  RIGHT CLICK ORDER"
            : "DRAG SELECT  /  RIGHT CLICK ORDER", 23, layout.Bottom + 31, 1, Label );
        Button( layout.Width - 127, layout.Bottom + 14, 104, "BUILD WING", fleet.CanBuildWing );
        Submit( commands, pipeline, upload, width, height );
    }

    private void PlanetPanel( FleetSimulation fleet, int planet, float right, int selectionCount ) {
        PanelAt( right, 147, 171, planet == 0 ? 43 : 157, SpaceDraw.Cyan );
        Text( CelestialSystem.Name( fleet.SystemIndex, planet ), right + 9, 156, 1.35f, White );
        if ( planet == 0 ) {
            Text( "STELLAR BODY", right + 9, 174, 1.05f, Label );
            return;
        }

        var status = fleet.Planet( planet );
        Span<char> line = stackalloc char[32];
        line.TryWrite( $"SURVEY {status.Survey * 100 / FleetSimulation.SurveyDuration} PCT", out int length );
        Text( status.Surveyed ? "SURVEY COMPLETE" : line[..length], right + 9, 174, 1.1f, status.Surveyed ? SpaceDraw.Cyan : Label );
        Button( right + 9, 189, 153, "SURVEY / E", !status.Surveyed && selectionCount != 0 && !fleet.Lost );
        Button( right + 9, 216, 153, status.MineRemaining == 0 ? "MINE ACTIVE" : status.MineRemaining > 0 ? "MINE BUILDING" : "MINE / 35 ORE",
            status.Surveyed && status.MineRemaining < 0 && fleet.Ore >= FleetSimulation.MineCost && !fleet.Lost );
        Button( right + 9, 243, 153, status.LabRemaining == 0 ? "LAB ACTIVE" : status.LabRemaining > 0 ? "LAB BUILDING" : "LAB / 45 ORE",
            status.Surveyed && status.LabRemaining < 0 && fleet.Ore >= FleetSimulation.LabCost && !fleet.Lost );
        Button( right + 9, 270, 153, fleet.WeaponUpgrade ? "WEAPONS UPGRADED" : "WEAPONS / 30 SCI",
            !fleet.WeaponUpgrade && fleet.Science >= FleetSimulation.UpgradeCost && !fleet.Lost );
    }

    private void PanelAt( float x, float y, float width, float height, Vector4 accent ) {
        Rect( x, y, width, height, Panel, 1 );
        Rect( x, y, 2, height, accent );
    }

    private void Button( float x, float y, float width, ReadOnlySpan<char> label, bool enabled, bool selected = false ) {
        Rect( x, y, width, 21, selected ? Active : Well );
        if ( selected ) Rect( x, y, 2, 21, SpaceDraw.Cyan, 3 );
        Text( label, x + 6, y + 7, 1.04f, selected ? White : enabled ? SpaceDraw.Cyan : Disabled );
    }

    private void FacilityMarker( float x, float y, int remaining, Vector4 color ) {
        Rect( x, y, 7, 7, Panel, 1 );
        Rect( x + 1, y + 1, 5, 5, remaining == 0 ? color : Label );
        Rect( x - 2, y + 3, 2, 1, color );
        Rect( x + 7, y + 3, 2, 1, color );
    }

    internal static HudHit Hit( Vector2 pixel, int width, int height, FleetSimulation fleet, int planet ) {
        if ( width < 350 || height < 240 ) return default;
        float scale = HudScale( width, height );
        var layout = new Layout( width / scale, height / scale );
        var p = pixel / scale;
        if ( !layout.Blocks( p, fleet, planet ) ) return default;
        if ( Contains( p, layout.ClockX, 19, 64, 21 ) ) return new( HudAction.Pause );
        if ( Contains( p, layout.ClockX + 70, 19, 36, 21 ) ) return new( HudAction.Speed );
        if ( Contains( p, layout.ClockX + 112, 19, 66, 21 ) ) return new( HudAction.Galaxy );
        if ( Contains( p, layout.Width - 127, layout.Bottom + 14, 104, 21 ) ) return new( HudAction.Wing );
        if ( p.X >= 23 && p.X < 162 && p.Y >= 106 ) {
            float rowY = p.Y - 106;
            int wanted = (int)(rowY / 24);
            if ( wanted < layout.MaxRows && rowY % 24 < 21 ) {
                for ( int squad = 0; squad < fleet.SquadCount; ++squad )
                    if ( fleet.IsPlayerSquad( squad ) && wanted-- == 0 ) return new( HudAction.Squad, squad );
            }
        }
        if ( planet > 0 && layout.PlanetVisible && Contains( p, layout.Right + 9, 189, 153, 102 ) ) {
            float rowY = p.Y - 189;
            if ( rowY % 27 < 21 ) return new( HudAction.Planet, 1 + (int)(rowY / 27) );
        }
        return new( HudAction.Panel );
    }

    private static bool Contains( Vector2 p, float x, float y, float width, float height ) =>
        p.X >= x && p.X < x + width && p.Y >= y && p.Y < y + height;

    private readonly record struct Layout( float Width, float Height ) {
        internal float Right => Width - 185;
        internal float ClockX => Width - 201;
        internal float Bottom => Height - 64;
        internal bool PlanetVisible => Bottom >= 312;
        internal int MaxRows => Math.Clamp( (int)((Bottom - 113) / 24), 1, 8 );

        internal float OutlinerHeight( FleetSimulation fleet ) {
            int count = 0;
            for ( int i = 0; i < fleet.SquadCount; ++i )
                if ( fleet.IsPlayerSquad( i ) ) ++count;
            return 46 + 24 * Math.Min( count, MaxRows );
        }

        internal bool Blocks( Vector2 p, FleetSimulation fleet, int planet ) =>
            Contains( p, 14, 12, Width - 28, 41 ) ||
            Contains( p, 14, 63, 157, OutlinerHeight( fleet ) ) ||
            Contains( p, Right, 63, 171, 76 ) ||
            PlanetVisible && Contains( p, Right, 147, 171, planet > 0 ? 157 : planet == 0 ? 43 : 64 ) ||
            Contains( p, 14, Bottom, Width - 28, 50 );
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
        PanelAt( logicalWidth - 117, 14, 103, 37, SpaceDraw.Cyan );
        Button( logicalWidth - 108, 22, 85, "RETURN", true );
        for ( int i = 0; i < CelestialSystem.Systems.Length; ++i ) {
            var center = CelestialSystem.MapPositions[i] * new Vector2( logicalWidth, logicalHeight );
            string name = CelestialSystem.Systems[i];
            float textWidth = TextWidth( name.Length, 1.3f );
            float left = center.X - textWidth / 2;
            Rect( left - 6, center.Y + 19, textWidth + 12, 7 * GlyphPitch( 1.3f ) / _scale + 10, Panel, 1 );
            Text( name, left, center.Y + 24, 1.3f, i == currentSystem ? SpaceDraw.Cyan : White );
        }
        Rect( 14, logicalHeight - 43, logicalWidth - 28, 29, Panel, 1 );
        Text( "CLICK SYSTEM TO ENTER   G RETURN   F11 FULLSCREEN", 23, logicalHeight - 33, 1, Label );
        Submit( commands, pipeline, upload, width, height );
    }

    internal static bool IsGalaxyReturn( Vector2 pixel, int width, int height ) {
        if ( width < 350 || height < 240 ) return false;
        float scale = HudScale( width, height );
        return Contains( pixel / scale, width / scale - 108, 22, 85, 21 );
    }

    private void Begin( int width, int height ) {
        _count = 0;
        _scale = HudScale( width, height );
    }

    private static float HudScale( int width, int height ) => Math.Clamp( MathF.Min( height / 500f, width / 660f ), 0.8f, 2.5f );

    private void Submit( GraphicsCommandList commands, PipelineState pipeline, GpuResource upload, int width, int height ) {
        upload.Write<Glyph>( _glyphs.AsSpan( 0, _count ) );
        commands.SetGraphicsPipeline( pipeline );
        commands.SetGraphicsConstants( [BitConverter.SingleToUInt32Bits( width ), BitConverter.SingleToUInt32Bits( height ), 0, 0] );
        commands.SetVertexBuffer( 0, upload, Stride );
        commands.Draw( 6, (uint)_count );
    }

    private void Rect( float x, float y, float width, float height, Vector4 color, uint layer = 2 ) =>
        _glyphs[_count++] = new( new Vector4( x, y, width, height ) * _scale, color, uint.MaxValue, 7, layer, 0 );

    private float GlyphPitch( float size ) => MathF.Max( 1, MathF.Round( size * _scale ) );
    private float TextWidth( int count, float size ) => count * 6 * GlyphPitch( size ) / _scale;

    private void Text( ReadOnlySpan<char> text, float x, float y, float scale, Vector4 tint ) {
        // Align each bitmap cell to complete screen pixels, including at fractional HUD scales.
        float pitch = GlyphPitch( scale );
        float pixelX = MathF.Round( x * _scale ), pixelY = MathF.Round( y * _scale );
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
                _glyphs[_count++] = new( new Vector4( pixelX, pixelY, 5 * pitch, 7 * pitch ), tint, (uint)bits, (uint)(bits >> 32), 0, 0 );
            }
            pixelX += 6 * pitch;
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
