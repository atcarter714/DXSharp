using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp.Graphics;
using DXSharp.Shaders;

namespace FleetBattler;

internal enum PlanetAction { None, Survey, Mine, Lab, Research, Yard, Industry, CancelSurvey, Orbit, Inspect, Depot, Drive, Close, Construction }
internal readonly record struct BodyMenu( Vector2 Pixel, int Body );
internal enum HudAction { None, Panel, Pause, Speed, Galaxy, Wing, Squad, Planet, Context, Command, OpenYard, CloseYard, BuildShip, CancelShip, BuildSite, CancelSite, MergeFleets, SplitFleets, ToggleBuildCatalog, Stance, ShipService, CycleModule, ResearchPage, MerchantDecision, BuildLayer, Colonize, CancelColony, DesignHull, ScrollFleet, Briefing, ShipDetails }
internal readonly record struct HudHit( HudAction Action, int Index = 0 );

internal sealed partial class FleetHud {
    private const uint Stride = 48;
    internal const uint BufferBytes = 4096 * Stride;
    private readonly SampledTexture2D _atlas;

    internal FleetHud( SampledTexture2D atlas ) => _atlas = atlas;
    private static readonly Vector4 Panel = new( 0.025f, 0.049f, 0.083f, 1 );
    private static readonly Vector4 Well = new( 0.066f, 0.125f, 0.19f, 1 );
    private static readonly Vector4 Active = new( 0.093f, 0.23f, 0.32f, 1 );
    private static readonly Vector4 Disabled = new( 0.52f, 0.63f, 0.73f, 1 );
    private static readonly Vector4 Label = new( 0.65f, 0.76f, 0.87f, 1 );
    private static readonly Vector4 White = new( 0.94f, 0.97f, 1.0f, 1 );
    private readonly Glyph[] _glyphs = new Glyph[BufferBytes / Stride];
    private int _count;
    private float _scale = 1;
    private uint _overlay;

    [StructLayout( LayoutKind.Sequential )]
    private readonly record struct Glyph( Vector4 Rectangle, Vector4 Tint, uint Low, uint High, uint Panel, uint Overlay );

    private const string Shader = """
        cbuffer Hud : register(b0) { float4 Viewport; };
        Texture2D<float4> Atlas : register(t0);
        SamplerState ArtSampler : register(s0);
        struct Input { float4 rect : POSITION; float4 tint : COLOR; uint4 bits : TEXCOORD; };
        struct Output {
            float4 position : SV_Position;
            float2 uv : TEXCOORD0;
            nointerpolation uint3 bits : TEXCOORD1;
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
            float depth = input.bits.z == 6 ? 0.9994 : input.bits.z == 1 ? 0.04 : input.bits.z == 2 ? 0.025 : input.bits.z == 3 ? 0.018 : input.bits.z == 4 ? 0.014 : input.bits.z == 5 ? 0.015 : 0.01;
            output.position = float4(pixel/Viewport.xy*float2(2,-2)+float2(-1,1),depth*(input.bits.w == 0 ? 1 : input.bits.w == 1 ? 0.15 : input.bits.w == 2 ? 0.02 : 0.002),1);
            output.bits = input.bits.xyz;
            output.tint = input.tint;
            return output;
        }
        float4 PS(Output input) : SV_Target {
            if (input.bits.z == 6) {
                float2 p = input.uv*4.4-2.2;
                float d = length(p);
                float2 ringPlane = float2(p.x*0.94-p.y*0.34,p.x*0.34+p.y*0.94);
                float ring = length(ringPlane*float2(1,3.15));
                if (d > 1) {
                    clip(input.bits.x != 0 && ring >= 1.24 && ring <= 2.05 ? 1 : -1);
                    return float4(input.tint.rgb*(0.51+0.21*sin(ring*72)),1);
                }
                float sunAngle = asfloat(input.bits.y);
                float2 sun = float2(cos(sunAngle),sin(sunAngle));
                float lit = 0.26+0.74*saturate(dot(p,sun)*0.72+sqrt(saturate(1-d*d))*0.75);
                float bands = 0.91+0.09*cos(p.y*24+p.x*2);
                return float4(input.tint.rgb*lit*bands,1);
            }
            if (input.bits.z == 4) {
                float2 tile = float2(input.bits.x % 3,input.bits.x / 3);
                float2 uv = (tile + lerp(0.02,0.98,input.uv)) / float2(3,2);
                return float4(pow(saturate(Atlas.Sample(ArtSampler,uv).rgb*input.tint.rgb),1.0/2.2),1);
            }
            uint columns = input.bits.z == 7 ? 7 : 5;
            uint2 cell = min(uint2(input.uv*float2(columns,7)), uint2(columns-1,6));
            uint index = cell.x + cell.y*columns;
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
        ReadOnlySpan<bool> selectedShips = default, int speed = 1, bool paused = false, FleetSelection? surveyUnit = null, BodyMenu? menu = null, Vector2? yard = null, CommandTab commandTab = CommandTab.Hidden, int dispatch = 0, ReadOnlySpan<MapSite> sites = default, MapHover hover = default, int yardInspector = -1, bool buildCatalog = false, bool secondResearchPage = false, SiteLayer catalogLayer = SiteLayer.Orbit, string? hostileAlert = null, int fleetScroll = 0, bool frigate = false, Briefing? briefing = null, ReadOnlySpan<Vector2> kuiperLabels = default, bool shipDetails = false ) {
        if ( width < 350 || height < 240 ) return;
        Begin( width, height );
        var layout = new Layout( width / _scale, height / _scale );
        Span<char> text = stackalloc char[72];
        var siteTint = fleet.Contested || fleet.Owner == 1 ? SpaceDraw.Amber : fleet.Owner == 0 ? SpaceDraw.Cyan : Label;
        var site = fleet.Contested ? "CONTESTED" : fleet.Capturing == 0 ? "CYAN CAPTURING"
            : fleet.Capturing == 1 ? "AMBER CAPTURING" : fleet.Owner == 0 ? "CYAN CONTROL"
            : fleet.Owner == 1 ? "AMBER CONTROL" : "UNCLAIMED";

        PanelAt( 14, 12, layout.Width - 28, 41, SpaceDraw.Cyan );
        Text( fleet.SystemName, 23, 20, 1.9f, White );
        text.TryWrite( $"ORE {fleet.Ore:D3}   SCI {fleet.Science:D3}", out int length );
        if ( fleet.Frontier ) Wallet( fleet, 23, 38 );
        else Text( text[..length], 23, 39, 1.05f, Label );
        Button( layout.ClockX, 19, 64, paused ? "RESUME" : "PAUSE", true, paused );
        text.TryWrite( $"{speed}X", out length );
        Button( layout.ClockX + 70, 19, 36, text[..length], true );
        Button( layout.ClockX + 112, 19, 66, "GALAXY", true );

        PanelAt( 14, 63, 157, layout.OutlinerHeight( fleet ), SpaceDraw.Cyan );
        Text( "FLEET OUTLINER", 23, 72, 1.35f, White );
        if ( fleet.Frontier ) text.TryWrite( $"{fleet.CyanCount} SOL  /  {fleet.AmberCount} HOSTILE", out length );
        else text.TryWrite( $"{fleet.CyanCount} CYAN   {fleet.AmberCount} AMBER", out length );
        Text( text[..length], 23, 89, 1.05f, Label );
        int offset = Math.Clamp( fleetScroll, 0, layout.ScrollLimit( fleet ) );
        int skip = offset;
        int row = 0;
        for ( int squad = 0; squad < fleet.SquadCount && row < layout.MaxRows; ++squad ) {
            if ( !fleet.IsPlayerSquad( squad ) || fleet.Frontier && !fleet.IsFleetLeader( squad ) ) continue;
            if ( skip-- > 0 ) continue;
            int alive = fleet.Frontier ? fleet.FleetStrength( squad ) : fleet.ShipCount( squad );
            var role = fleet.Role( squad );
            string shortRole = role is HullRole.Miner ? "MINER" : role is HullRole.Colony ? "COLONY" : FleetSimulation.RoleName( role );
            text.TryWrite( $"{squad + 1:D2} {shortRole} {alive:D2}", out length );
            if ( fleet.Frontier && alive > fleet.ShipCount( squad ) )
                text.TryWrite( $"FLEET {fleet.FleetId( squad ):D2} / {alive} HULLS", out length );
            if ( fleet.FleetName( squad ) is { } fleetName )
                length = CopyStatus( fleetName, text );
            bool active = selectionCount != 0 && selected.Squad == squad;
            if ( !selectedShips.IsEmpty ) {
                for ( int member = 0; member < fleet.SquadCount; ++member )
                    if ( (member == squad || fleet.Frontier && fleet.FleetId( member ) == fleet.FleetId( squad )) &&
                         fleet.AnySelectedInSquad( member, selectedShips ) ) { active = true; break; }
            }
            float rowTop = 106 + row++ * 24;
            if ( fleet.Frontier ) {
                Rect( 23, rowTop, 132, 21, active ? Active : Well );
                Art( HullArt( fleet.Role( squad ) ), 23, rowTop, 30, 21 );
                Text( text[..Math.Min( length, (int)(101 / TextWidth( 1, 1 )) )], 58, rowTop + 7, 1, active ? White : Label );
            } else Button( 23, rowTop, 139, text[..length], alive != 0, active );
        }

        if ( layout.ScrollLimit( fleet ) > 0 ) {
            var track = layout.FleetTrack( fleet );
            Rect( track.X, track.Y, track.Z, track.W, Well );
            float thumb = track.W * layout.MaxRows / layout.OutlinerCount( fleet );
            Rect( track.X, track.Y + (track.W - thumb) * offset / layout.ScrollLimit( fleet ), track.Z, thumb, SpaceDraw.Cyan );
        }

        if ( fleet.Frontier && layout.FleetControls( fleet ) + 21 < layout.Bottom ) {
            float top = layout.FleetControls( fleet );
            IconButton( 14, top, 76, HudIcon.Merge, "MERGE", fleet.CanMergeFleets( selectedShips ) );
            IconButton( 95, top, 76, HudIcon.Split, "SPLIT", fleet.CanSplitFleets( selectedShips ) );
        }
        if ( fleet.Frontier && selectionCount > 0 ) DrawFleetOrders( layout, fleet, selectedShips );
        if ( fleet.Frontier && surveyUnit is { } navigator )
            FlightCard( layout, fleet, navigator, speed );
        float right = layout.Right;
        PanelAt( right, 63, 171, 76, siteTint );
        if ( fleet.Frontier ) site = fleet.ExpeditionArrived ? fleet.ReceiverSecured ? "RECEIVER SECURED" : "RESTORE THE RECEIVER" : !fleet.Established ? "NO SOL PRESENCE" : fleet.Recovering ? "REBUILD THE FLEET" : fleet.ActiveRaids == 0 ? "SURVEY THE FRONTIER" : fleet.PirateControlledBody >= 0 ? "PIRATES RAIDING TRADE" : fleet.AmberCount > 0 ? "RAID IN PROGRESS" : "RAIDERS CLEARED";
        Text( fleet.Frontier ? "FRONTIER" : "MINING RELAY", right + 9, 72, 1.35f, White );
        Text( site, right + 9, 89, 1.1f, siteTint );
        float fraction = fleet.CaptureTicks > 0 ? (float)fleet.CaptureTicks / FleetSimulation.CaptureDuration
            : fleet.Owner >= 0 ? 1 : 0;
        Rect( right + 9, 108, 153, 2, Well );
        if ( fraction > 0 ) Rect( right + 9, 108, 153 * fraction, 2, siteTint );
        if ( fleet.RaidWarningTicks > 0 ) text.TryWrite( $"INBOUND / {Seconds( fleet.RaidWarningTicks, speed )} SEC", out length );
        else if ( fleet.Frontier ) text.TryWrite( $"NAVAL {fleet.NavalUsed}+{fleet.NavalReserved}/{fleet.NavalCapacity}", out length );
        else text.TryWrite( $"WINGS {fleet.WingsBuilt}/{FleetSimulation.WingGoal}  AMBER {fleet.AmberCount}", out length );
        Text( text[..length], right + 9, 120, 1.05f, Label );

        if ( fleet.Solar && bodies.Length == CelestialSystem.BodyCount ) {
            for ( int i = CelestialSystem.FrontierBodyCount; i < bodies.Length; ++i ) {
                var center = bodies[i];
                var light = new Vector2( bodies[0].X - center.X, bodies[0].Y - center.Y );
                ref readonly var world = ref SolarCatalog.Get( i );
                float radius = MathF.Max( center.Z, 2 ) * 2.2f;
                _glyphs[_count++] = new( new( center.X - radius, center.Y - radius, radius * 2, radius * 2 ),
                    new( world.Color, 1 ), world.Rings ? 1u : 0, BitConverter.SingleToUInt32Bits( MathF.Atan2( light.Y, light.X ) ), 6, 0 );
            }
        }
        for ( int i = 0; i < bodies.Length; ++i ) {
            var body = bodies[i] / _scale;
            var name = fleet.BodyName( i );
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

        if ( fleet.Solar ) {
            float nearest = 56;
            Vector2 anchor = default;
            for ( int i = 0; i < kuiperLabels.Length; ++i ) {
                var candidate = kuiperLabels[i] / _scale;
                float distance = Vector2.DistanceSquared( candidate, new( layout.Width * 0.5f, 76 ) );
                if ( candidate.X < 175 || candidate.X > layout.Width - 185 || candidate.Y < 60 ||
                     candidate.Y > layout.Bottom - 13 || distance >= nearest * nearest ) continue;
                nearest = MathF.Sqrt( distance );
                anchor = kuiperLabels[i];
            }
            if ( anchor != default ) MapLabel( anchor, "KUIPER BELT", layout, fleet, planet );
        }

        if ( beacon is { } beaconPixel ) {
            var p = beaconPixel / _scale;
            if ( p.X >= 30 && p.X <= layout.Width - 30 && p.Y > 16 && p.Y < layout.Bottom - 15 && !layout.Blocks( p, fleet, planet ) ) {
                Rect( p.X - 22, p.Y + 6, 44, 12, Panel, 1 );
                Text( "RELAY", p.X - TextWidth( 5, 1 ) / 2, p.Y + 9, 1, siteTint );
            }
        }

        if ( yard is { } yardPixel ) MapLabel( yardPixel, "YARD", layout, fleet, planet );
        if ( planet >= 0 && layout.PlanetVisible ) {
            if ( fleet.Frontier ) InspectPlanet( layout, fleet, planet, surveyUnit, speed, buildCatalog, catalogLayer );
            else PlanetPanel( fleet, planet, right, surveyUnit );
        }
        else if ( layout.PlanetVisible ) {
            PanelAt( right, 147, 171, 91, SpaceDraw.Cyan );
            Text( "SELECTION", right + 9, 156, 1.35f, White );
            if ( selectionCount == 0 ) text.TryWrite( $"NO SHIPS SELECTED", out length );
            else if ( selected.IsShip ) {
                if ( fleet.ShipName( selected.Ship ) is { } name ) length = CopyStatus( name, text );
                else text.TryWrite( $"SHIP {selected.Ship + 1:D2}  HULL {fleet.Hull( selected.Ship )}/{FleetSimulation.MaxHull( fleet.Role( selected.Squad ) )}", out length );
            }
            else if ( surveyUnit is { } unit ) text.TryWrite( $"SQUAD {unit.Squad + 1:D2}  {fleet.ShipCount( unit.Squad )} SHIPS", out length );
            else text.TryWrite( $"{(selectionCount < 0 ? fleet.ShipCount( selected.Squad ) : selectionCount)} SHIPS SELECTED", out length );
            Text( text[..length], right + 9, 174, 1.1f, Label );
            if ( selectionCount > 1 && surveyUnit is null )
                Text( "SELECT ONE FOR PLAN", right + 9, 192, 1, Label );
            else if ( selectionCount != 0 ) {
                var plan = fleet.Plan( selected );
                if ( plan.Body >= 0 ) text.TryWrite( $"{(plan.Order == FlightOrder.Survey ? "SURVEY" : "ORBIT")} {fleet.BodyName( plan.Body )}", out length );
                else if ( plan.Target >= 0 ) text.TryWrite( $"ATTACK SHIP {plan.Target + 1:D2}", out length );
                else length = CopyStatus( plan.Order == FlightOrder.Move ? "MOVING TO WAYPOINT" : fleet.Frontier ? "PARKED IN ORBIT" : "HOLDING POSITION", text );
                Text( text[..length], right + 9, 192, 1.05f, SpaceDraw.Cyan );
            }
            if ( fleet.Frontier && selectionCount == 1 && selected.IsShip && FleetSimulation.IsWarship( fleet.Role( selected.Squad ) ) ) {
                text.TryWrite( $"SHD {fleet.Shield( selected.Ship )}  ARM {fleet.Armor( selected.Ship )}", out length );
                Text( text[..length], right + 9, 213, 1, Label );
            } else Text( fleet.Frontier ? "F2 RENAME / RIGHT CLICK" : "RIGHT CLICK BODY", right + 9, 213, 1, Label );
            if ( fleet.Frontier && selected.IsShip && fleet.Alive( selected.Ship ) ) {
                Button( right + 96, 151, 66, shipDetails ? "CLOSE" : "DETAILS", true );
                if ( shipDetails ) ShipDetails( layout, fleet, selected );
            }
        }

        if ( layout.YardVisible && !fleet.Frontier ) YardPanel( fleet, right, layout.Bottom - 71, speed );

        if ( drag is { } box ) {
            box /= _scale;
            float w = box.Z - box.X, h = box.W - box.Y;
            Rect( box.X, box.Y, w, 1, SpaceDraw.Cyan );
            Rect( box.X, box.W, w, 1, SpaceDraw.Cyan );
            Rect( box.X, box.Y, 1, h, SpaceDraw.Cyan );
            Rect( box.Z, box.Y, 1, h, SpaceDraw.Cyan );
        }

        PanelAt( 14, layout.Bottom, layout.Width - 28, 50, SpaceDraw.Cyan );
        int routeTicks = selectionCount > 0 && fleet.Frontier && surveyUnit is not null ? fleet.RouteTicks( selected ) : 0;
        string? flightStatus = routeTicks > 0 ? $"{PhaseName( fleet.RoutePhase( selected ) )} / ETA {Seconds( routeTicks, speed )} SEC" : null;
        string instruction = notice ?? (fleet.Recovering ? "FLEET LOST / REBUILD AT HOMEWORLD OR A YARD"
            : flightStatus ?? (fleet.Frontier ? fleet.Objective
            : fleet.Lost ? "DEFEAT / CYAN FLEET LOST"
            : fleet.Won ? fleet.AmberCount == 0 ? "VICTORY / AMBER DEFEATED" : "VICTORY / WING GOAL MET"
            : fleet.InBattle ? "ENGAGEMENT / RIGHT CLICK HOSTILE TO ATTACK"
            : selectionCount != 0 && fleet.Orbiting( selected ) ? "ORBIT ORDER ACTIVE"
            : oreShort && fleet.Ore < fleet.CurrentWingCost ? "NEED ORE TO QUEUE A WING"
            : fleet.Owner == 0 ? "HOLD THE RELAY TO MINE ORE" : "CAPTURE THE RELAY OR DEFEAT AMBER"));
        float instructionScale = layout.Width < 530 ? 1 : 1.16f;
        int instructionCapacity = (int)((layout.Width - 159) / TextWidth( 1, instructionScale ));
        Text( instruction.AsSpan( 0, Math.Min( instruction.Length, instructionCapacity ) ), 23, layout.Bottom + 9, instructionScale,
            fleet.Won ? SpaceDraw.Cyan : fleet.Lost ? SpaceDraw.Amber : White );
        Text( fleet.Frontier && layout.Width >= 650 ? "J MERGE  /  SHIFT J SPLIT  /  MID PAN  /  SHIFT MID TILT  /  HOME RESET"
            : layout.Width >= 585 ? "MID PAN  /  SHIFT MID TILT  /  F FOCUS  /  HOME RESET"
            : "DRAG SELECT  /  RIGHT CLICK ORDER", 23, layout.Bottom + 31, 1, Label );
        Button( layout.Width - 127, layout.Bottom + 14, 104, fleet.Frontier ? "ORDERS" : "QUEUE WING", fleet.Frontier || fleet.CanBuildWing );
        if ( fleet.Frontier ) CommandPanel( layout, fleet, commandTab, dispatch, speed, secondResearchPage, frigate );
        if ( fleet.Frontier ) OperationsFeedback( layout, fleet, bodies, sites, hover, speed, planet );
        if ( yardInspector >= 0 && fleet.CanOpenYard( yardInspector ) ) DrawShipyard( layout, fleet, yardInspector, speed );
        if ( hostileAlert is not null ) {
            float alertWidth = MathF.Min( layout.Width - 28, 318 ), x = (layout.Width - alertWidth) / 2;
            float y = layout.Bottom - 56;
            PanelAt( x, y, alertWidth, 48, SpaceDraw.Amber );
            Text( "HOSTILE CONTACT", x + 9, y + 8, 1.28f, SpaceDraw.Amber );
            int characters = (int)((alertWidth - 18) / TextWidth( 1, 1.05f ));
            Text( hostileAlert.AsSpan( 0, Math.Min( hostileAlert.Length, characters ) ), x + 9, y + 29, 1.05f, White );
        }
        if ( menu is { } popup ) ContextPanel( layout, fleet, popup, surveyUnit, selectionCount != 0 );
        if ( briefing is { } prompt ) DrawBriefing( layout, prompt );
        Submit( commands, pipeline, upload, width, height );
    }

    private void ShipDetails( Layout layout, FleetSimulation fleet, FleetSelection unit ) {
        float left = layout.Width - 253, top = 242;
        PanelAt( left, top, 239, 145, SpaceDraw.Cyan );
        var role = fleet.Role( unit.Squad );
        Text( FleetSimulation.RoleName( role ), left + 9, top + 8, 1.2f, White );
        Span<char> line = stackalloc char[46];
        line.TryWrite( $"HULL {fleet.Hull( unit.Ship )}/{FleetSimulation.MaxHull( role )} / {PhaseName( fleet.RoutePhase( unit ) )}", out int length );
        Text( line[..length], left + 9, top + 28, 1, Label );
        var flight = fleet.Flight( unit.Squad );
        line.TryWrite( $"CRUISE {flight.Cruise:F0} / THRUST {flight.Acceleration:F0}", out length );
        Text( line[..length], left + 9, top + 43, 1, Label );
        if ( !FleetSimulation.IsWarship( role ) ) return;
        var fit = fleet.Loadout( unit.Ship );
        for ( int i = 0; i < (int)ModuleSlot.Count; ++i ) {
            float y = top + 61 + 13 * i;
            Text( ModuleLabels[i], left + 9, y, 1, Label );
            Text( fit.Installed( (ModuleSlot)i ), left + 65, y, 1, White );
        }
    }

    private static int CopyStatus( ReadOnlySpan<char> status, Span<char> destination ) {
        status.CopyTo( destination );
        return status.Length;
    }

    private void PlanetPanel( FleetSimulation fleet, int planet, float right, FleetSelection? surveyUnit ) {
        PanelAt( right, 147, 171, planet == 0 ? 43 : 184, SpaceDraw.Cyan );
        Text( fleet.BodyName( planet ), right + 9, 156, 1.35f, White );
        if ( planet == 0 ) {
            Text( "STELLAR BODY", right + 9, 174, 1.05f, Label );
            return;
        }

        var status = fleet.Planet( planet );
        Span<char> line = stackalloc char[32];
        string phase = status.SurveyUnit is null ? "SURVEY" : fleet.SurveyWorking( planet ) ? "SCANNING" : "EN ROUTE";
        line.TryWrite( $"{phase} {status.Survey * 100 / FleetSimulation.SurveyDuration} PCT", out int length );
        if ( status.Surveyed && fleet.HasDeposit( planet ) )
            line.TryWrite( $"DEPOSIT / {fleet.DepositYield( planet )} ORE", out length );
        Text( status.Surveyed && !fleet.HasDeposit( planet ) ? "SURVEY COMPLETE" : line[..length], right + 9, 174, 1.1f, status.Surveyed ? SpaceDraw.Cyan : Label );
        ReadOnlySpan<char> surveyLabel = SurveyLabel( status, surveyUnit, line );
        Button( right + 9, 189, 153, surveyLabel, !fleet.Lost && (status.SurveyUnit is not null ||
            surveyUnit is { } unit && fleet.CanSurvey( unit, planet )) );
        Button( right + 9, 216, 153, status.MineRemaining == 0 ? "MINE ACTIVE" : status.MineRemaining > 0 ? "MINE BUILDING" : "MINE / 35 ORE",
            status.Surveyed && status.MineRemaining < 0 && fleet.Ore >= FleetSimulation.MineCost && !fleet.Lost );
        Button( right + 9, 243, 153, status.LabRemaining == 0 ? "LAB ACTIVE" : status.LabRemaining > 0 ? "LAB BUILDING" : "LAB / 45 ORE",
            status.Surveyed && status.LabRemaining < 0 && fleet.Ore >= FleetSimulation.LabCost && !fleet.Lost );
        Button( right + 9, 270, 153, fleet.WeaponUpgrade ? "WEAPONS UPGRADED" : "WEAPONS / 30 SCI",
            !fleet.WeaponUpgrade && fleet.Science >= FleetSimulation.UpgradeCost && !fleet.Lost );
        Button( right + 9, 297, 153, fleet.YardBody == planet ? "SHIPYARD HERE" : fleet.YardBody >= 0 ? "YARD ELSEWHERE" : "SHIPYARD / 55 ORE",
            fleet.YardBody < 0 && status.Surveyed && fleet.Ore >= FleetSimulation.YardCost && !fleet.Lost );
    }

    private static ReadOnlySpan<char> SurveyLabel( PlanetStatus status, FleetSelection? selected, Span<char> text ) {
        if ( status.Surveyed ) return "SURVEY COMPLETE";
        if ( (status.SurveyUnit ?? selected) is not { } unit ) return "SELECT SURVEY UNIT";
        text.TryWrite( $"{(status.SurveyUnit is null ? "SURVEY" : "ACTIVE")} {(unit.IsShip ? "SHIP" : "SQUAD")} {(unit.IsShip ? unit.Ship + 1 : unit.Squad + 1):D2}", out int length );
        return text[..length];
    }

    private void YardPanel( FleetSimulation fleet, float right, float top, int speed ) {
        PanelAt( right, top, 171, 63, SpaceDraw.Cyan );
        Text( "ORBITAL SHIPYARD", right + 9, top + 8, 1.2f, White );
        Span<char> text = stackalloc char[40];
        int length;
        if ( fleet.Lost ) text.TryWrite( $"SHIPYARD OFFLINE", out length );
        else if ( fleet.YardRemaining < 0 ) text.TryWrite( $"SURVEY A WORLD", out length );
        else if ( fleet.YardRemaining > 0 ) text.TryWrite( $"CONSTRUCTION {Seconds( fleet.YardRemaining )} SEC", out length );
        else if ( fleet.QueuedWings == 0 ) text.TryWrite( $"IDLE  /  WING {fleet.CurrentWingCost} ORE", out length );
        else text.TryWrite( $"QUEUE {fleet.QueuedWings}  /  {Seconds( fleet.WingRemaining )} SEC", out length );
        Text( text[..length], right + 9, top + 24, 1, Label );
        if ( fleet.QueuedWings > 0 ) {
            Rect( right + 9, top + 36, 153, 2, Well );
            Rect( right + 9, top + 36, 153 * (1f - (float)fleet.WingRemaining / fleet.ActiveWingDuration), 2, SpaceDraw.Cyan, 3 );
        }
        Button( right + 9, top + 40, 153, fleet.IndustryUpgrade ? "INDUSTRY UPGRADED" : "INDUSTRY / 24 SCI",
            !fleet.IndustryUpgrade && fleet.Science >= FleetSimulation.IndustryCost && !fleet.Lost );

        int Seconds( int ticks ) => (int)Math.Ceiling( ticks * FleetSimulation.StepSeconds / (speed * 0.25) );
    }

    private void MapLabel( Vector2 pixel, string label, Layout layout, FleetSimulation fleet, int planet ) {
        var p = pixel / _scale;
        float width = TextWidth( label.Length, 1 );
        if ( p.X - width < 0 || p.X + width > layout.Width || p.Y < 12 || p.Y >= layout.Bottom - 20 || layout.Blocks( p, fleet, planet ) ) return;
        Rect( p.X - width / 2 - 4, p.Y + 6, width + 8, 15, Panel, 1 );
        Text( label, p.X - width / 2, p.Y + 9, 1, SpaceDraw.Neutral );
    }

    private void ContextPanel( Layout layout, FleetSimulation fleet, BodyMenu menu, FleetSelection? unit, bool hasSelection ) {
        var area = layout.Context( menu, _scale );
        _overlay = 2;
        PanelAt( area.X, area.Y, area.Z, area.W, SpaceDraw.Cyan );
        Text( fleet.BodyName( menu.Body ), area.X + 9, area.Y + 8, 1.3f, White );
        Button( area.X + 9, area.Y + 23, area.Z - 18, "INSPECT BODY", true );
        Button( area.X + 9, area.Y + 47, area.Z - 18, "ENTER ORBIT", hasSelection );
        if ( menu.Body > 0 ) {
            var status = fleet.Planet( menu.Body );
            Span<char> text = stackalloc char[32];
            Button( area.X + 9, area.Y + 71, area.Z - 18, status.SurveyUnit is null ? SurveyLabel( status, unit, text ) : "CANCEL SURVEY",
                status.SurveyUnit is not null || unit is { } selection && fleet.CanSurvey( selection, menu.Body ) );
            Button( area.X + 9, area.Y + 95, area.Z - 18, fleet.Frontier ? "BUILD CATALOG" : fleet.YardAt( menu.Body ) >= 0 ? "SHIPYARD ASSIGNED" : "SHIPYARD / 55 ORE",
                fleet.Frontier || fleet.YardBody < 0 && status.Surveyed && fleet.Ore >= FleetSimulation.YardCost && !fleet.Lost );
        }
        _overlay = 0;
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

    internal static HudHit Hit( Vector2 pixel, int width, int height, FleetSimulation fleet, int planet, BodyMenu? menu = null, CommandTab commandTab = CommandTab.Hidden, int yardInspector = -1,
        bool buildCatalog = false, bool hasSelection = false, int dispatch = 0, bool secondResearchPage = false, SiteLayer catalogLayer = SiteLayer.Orbit, int fleetScroll = 0, Briefing? briefing = null, bool shipDetails = false ) {
        if ( width < 350 || height < 240 ) return default;
        float scale = HudScale( width, height );
        var layout = new Layout( width / scale, height / scale );
        var p = pixel / scale;
        if ( briefing is not null ) return HitBriefing( layout, p );
        if ( menu is { } popup ) {
            var area = layout.Context( popup, scale );
            if ( Contains( p, area.X, area.Y, area.Z, area.W ) ) {
                float row = p.Y - area.Y - 23;
                if ( p.X >= area.X + 9 && p.X < area.X + area.Z - 9 && row >= 0 && row % 24 < 21 ) {
                    var action = (int)(row / 24) switch {
                        0 => PlanetAction.Inspect, 1 => PlanetAction.Orbit,
                        2 when popup.Body > 0 => fleet.Planet( popup.Body ).SurveyUnit is null ? PlanetAction.Survey : PlanetAction.CancelSurvey,
                        3 when popup.Body > 0 => fleet.Frontier ? PlanetAction.Construction : PlanetAction.Yard, _ => PlanetAction.None
                    };
                    if ( action != PlanetAction.None ) return new( HudAction.Context, (int)action );
                }
                return new( HudAction.Panel );
            }
            return default;
        }
        if ( fleet.Frontier ) {
            if ( yardInspector >= 0 && fleet.CanOpenYard( yardInspector ) ) {
                var yard = HitShipyard( layout, p, fleet, yardInspector );
                if ( yard.Action != HudAction.None ) return yard;
            }
            var command = HitCommand( layout, p, fleet, commandTab, dispatch, secondResearchPage );
            if ( command.Action != HudAction.None ) return command;
        }
        if ( fleet.Frontier && planet >= 0 && layout.PlanetVisible ) {
            var inspector = HitInspector( layout, p, planet, fleet, buildCatalog, catalogLayer );
            if ( inspector.Action != HudAction.None ) return inspector;
        }
        if ( fleet.Frontier && layout.FleetControls( fleet ) + 21 < layout.Bottom ) {
            float top = layout.FleetControls( fleet );
            if ( Contains( p, 14, top, 76, 21 ) ) return new( HudAction.MergeFleets );
            if ( Contains( p, 95, top, 76, 21 ) ) return new( HudAction.SplitFleets );
        }
        if ( fleet.Frontier && hasSelection ) {
            var orders = HitFleetOrders( layout, p, fleet );
            if ( orders.Action != HudAction.None ) return orders;
        }
        if ( fleet.Frontier && planet < 0 && layout.PlanetVisible && hasSelection ) {
            if ( Contains( p, layout.Right + 96, 151, 66, 21 ) ) return new( HudAction.ShipDetails );
            if ( shipDetails && Contains( p, layout.Width - 253, 242, 239, 145 ) ) return new( HudAction.Panel );
        }
        if ( !layout.Blocks( p, fleet, planet ) ) return default;
        var track = layout.FleetTrack( fleet );
        int limit = layout.ScrollLimit( fleet );
        if ( limit > 0 && Contains( p, track.X - 2, track.Y, track.Z + 4, track.W ) )
            return new( HudAction.ScrollFleet, Math.Clamp( (int)MathF.Round( limit * (p.Y - track.Y) / track.W ), 0, limit ) );
        if ( Contains( p, layout.ClockX, 19, 64, 21 ) ) return new( HudAction.Pause );
        if ( Contains( p, layout.ClockX + 70, 19, 36, 21 ) ) return new( HudAction.Speed );
        if ( Contains( p, layout.ClockX + 112, 19, 66, 21 ) ) return new( HudAction.Galaxy );
        if ( Contains( p, layout.Width - 127, layout.Bottom + 14, 104, 21 ) ) return fleet.Frontier ? new( HudAction.Command, (int)CommandAction.Orders ) : new( HudAction.Wing );
        if ( p.X >= 23 && p.X < 162 && p.Y >= 106 ) {
            float rowY = p.Y - 106;
            int wanted = (int)(rowY / 24);
            if ( wanted < layout.MaxRows && rowY % 24 < 21 ) {
                wanted += Math.Clamp( fleetScroll, 0, limit );
                for ( int squad = 0; squad < fleet.SquadCount; ++squad )
                    if ( fleet.IsPlayerSquad( squad ) && (!fleet.Frontier || fleet.IsFleetLeader( squad )) && wanted-- == 0 ) return new( HudAction.Squad, squad );
            }
        }
        if ( layout.YardVisible && !fleet.Frontier && Contains( p, layout.Right + 9, layout.Bottom - 31, 153, 21 ) )
            return new( HudAction.Planet, (int)PlanetAction.Industry );
        if ( !fleet.Frontier && planet > 0 && layout.PlanetVisible && Contains( p, layout.Right + 9, 189, 153, 129 ) ) {
            float rowY = p.Y - 189;
            if ( rowY % 27 < 21 ) return new( HudAction.Planet, 1 + (int)(rowY / 27) );
        }
        return new( HudAction.Panel );
    }

    private static bool Contains( Vector2 p, float x, float y, float width, float height ) =>
        p.X >= x && p.X < x + width && p.Y >= y && p.Y < y + height;

    private readonly partial record struct Layout( float Width, float Height ) {
        internal float Right => Width - 185;
        internal float ClockX => Width - 201;
        internal float Bottom => Height - 64;
        internal bool PlanetVisible => Bottom >= 339;
        internal bool YardVisible => Bottom >= 410;
        internal int MaxRows => Math.Clamp( (int)((Bottom - 113) / 24), 1, 8 );

        internal int OutlinerCount( FleetSimulation fleet ) {
            int count = 0;
            for ( int i = 0; i < fleet.SquadCount; ++i )
                if ( fleet.IsPlayerSquad( i ) && (!fleet.Frontier || fleet.IsFleetLeader( i )) ) ++count;
            return count;
        }
        internal float OutlinerHeight( FleetSimulation fleet ) => 46 + 24 * Math.Min( OutlinerCount( fleet ), MaxRows );
        internal int ScrollLimit( FleetSimulation fleet ) => Math.Max( 0, OutlinerCount( fleet ) - MaxRows );
        internal Vector4 FleetTrack( FleetSimulation fleet ) => new( 160, 106, 5, 24 * Math.Min( OutlinerCount( fleet ), MaxRows ) );

        internal Vector4 Context( BodyMenu menu, float scale ) {
            float width = Math.Clamp( Width - 12, 1, 213 );
            float height = menu.Body == 0 ? 75 : 123;
            var pos = Vector2.Clamp( menu.Pixel / scale + new Vector2( 9, 4 ), new( 6, 6 ), new( Width - width - 6, Height - height - 6 ) );
            return new( pos.X, pos.Y, width, height );
        }

        internal bool Blocks( Vector2 p, FleetSimulation fleet, int planet ) =>
            Contains( p, 14, 12, Width - 28, 41 ) ||
            Contains( p, 14, 63, 157, OutlinerHeight( fleet ) ) ||
            Contains( p, Right, 63, 171, 76 ) ||
            PlanetVisible && (fleet.Frontier && planet >= 0
                ? Contains( p, InspectorX, 147, InspectorWidth, planet == 0 ? 60 : InspectorHeight )
                : Contains( p, Right, 147, 171, planet > 0 ? 184 : planet == 0 ? 43 : 91 )) ||
            YardVisible && (!fleet.Frontier || planet < 0) && Contains( p, Right, Bottom - 71, 171, 63 ) ||
            Contains( p, 14, Bottom, Width - 28, 50 );
    }

    internal static bool ScrollOutliner( Vector2 pixel, int width, int height, FleetSimulation fleet, int delta, ref int offset ) {
        if ( width < 350 || height < 240 ) return false;
        float scale = HudScale( width, height );
        var layout = new Layout( width / scale, height / scale );
        if ( !Contains( pixel / scale, 14, 63, 157, layout.OutlinerHeight( fleet ) ) ) return false;
        offset = Math.Clamp( offset - delta, 0, layout.ScrollLimit( fleet ) );
        return true;
    }

    internal void RecordGalaxy( GraphicsCommandList commands, PipelineState pipeline, GpuResource upload,
        int currentSystem, int width, int height, bool frontier = false, bool charted = false, string? message = null, string? action = null ) {
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
            bool known = !frontier || i == 0 || charted;
            string name = !known ? "UNEXPLORED" : frontier && i == 0 ? "SOLAR SYSTEM" : CelestialSystem.Systems[i];
            float textWidth = TextWidth( name.Length, 1.3f );
            float left = center.X - textWidth / 2;
            Rect( left - 6, center.Y + 19, textWidth + 12, 7 * GlyphPitch( 1.3f ) / _scale + 10, Panel, 1 );
            Text( name, left, center.Y + 24, 1.3f, i == currentSystem ? SpaceDraw.Cyan : known ? White : Disabled );
        }
        Rect( 14, logicalHeight - 43, logicalWidth - 28, 29, Panel, 1 );
        var status = message ?? (frontier ? "SELECT CHARTED SYSTEM   /   G RETURN" : "SELECT SYSTEM   /   G RETURN");
        int messageWidth = (int)((logicalWidth - (action is null ? 38 : 178)) / TextWidth( 1, 1 ));
        Text( status.AsSpan( 0, Math.Min( status.Length, messageWidth ) ), 23, logicalHeight - 33, 1, Label );
        if ( action is not null ) Button( logicalWidth - 145, logicalHeight - 39, 122, action, true );
        Submit( commands, pipeline, upload, width, height );
    }

    internal static bool IsGalaxyAction( Vector2 pixel, int width, int height ) {
        float scale = HudScale( width, height );
        return Contains( pixel / scale, width / scale - 145, height / scale - 39, 122, 21 );
    }

    internal static bool IsGalaxyReturn( Vector2 pixel, int width, int height ) {
        if ( width < 350 || height < 240 ) return false;
        float scale = HudScale( width, height );
        return Contains( pixel / scale, width / scale - 108, 22, 85, 21 );
    }

    private void Begin( int width, int height ) {
        _count = 0;
        _overlay = 0;
        _scale = HudScale( width, height );
    }

    private static float HudScale( int width, int height ) => Math.Clamp( MathF.Min( height / 500f, width / 660f ), 0.8f, 2.5f );

    private void Submit( GraphicsCommandList commands, PipelineState pipeline, GpuResource upload, int width, int height ) {
        upload.Write<Glyph>( _glyphs.AsSpan( 0, _count ) );
        commands.SetGraphicsPipeline( pipeline );
        commands.SetGraphicsConstants( [BitConverter.SingleToUInt32Bits( width ), BitConverter.SingleToUInt32Bits( height ), 0, 0] );
        commands.SetGraphicsTexture( _atlas, 1 );
        commands.SetVertexBuffer( 0, upload, Stride );
        commands.Draw( 6, (uint)_count );
    }

    private void Rect( float x, float y, float width, float height, Vector4 color, uint layer = 2 ) =>
        _glyphs[_count++] = new( new Vector4( x, y, width, height ) * _scale, color, uint.MaxValue, 7, layer, _overlay );

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
            if ( c == '+' ) rows = Plus;
            else if ( index >= 0 ) rows = Font[index];
            if ( rows is not null ) {
                ulong bits = 0;
                for ( int row = 0; row < 7; ++row ) bits |= (ulong)rows[row] << (row * 5);
                _glyphs[_count++] = new( new Vector4( pixelX, pixelY, 5 * pitch, 7 * pitch ), tint, (uint)bits, (uint)(bits >> 32), 0, _overlay );
            }
            pixelX += 6 * pitch;
        }
    }

    private static readonly uint[] Plus = [0, 4, 4, 31, 4, 4, 0];

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
