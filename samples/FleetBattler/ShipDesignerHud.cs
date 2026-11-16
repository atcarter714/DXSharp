using System.Numerics;

namespace FleetBattler;

internal enum DesignerControl { None, Close, Hull, Slot, Module, Save, Reset }
internal readonly record struct DesignerHit( DesignerControl Control, int Index = 0 );

internal sealed class ShipDesigner {
    internal HullBlueprint Hull { get; private set; } = HullBlueprint.Corvette;
    internal ShipDesign Draft { get; private set; }
    internal ModuleSlot Slot { get; private set; }
    internal int Option { get; private set; } = -1;
    internal string Message { get; private set; } = "SELECT OR DRAG A MODULE";
    internal bool Dragging { get; private set; }

    internal void Open( FleetSimulation fleet, HullBlueprint hull ) {
        Hull = hull;
        Draft = fleet.Blueprint( hull );
        Slot = ModuleSlot.Weapon;
        Option = -1;
        Dragging = false;
        Message = "SELECT OR DRAG A MODULE";
    }

    internal void Click( FleetSimulation fleet, DesignerHit hit ) {
        switch ( hit.Control ) {
            case DesignerControl.Hull: Open( fleet, (HullBlueprint)hit.Index ); break;
            case DesignerControl.Slot:
                if ( Option >= 0 && Slot == (ModuleSlot)hit.Index ) Drop( (ModuleSlot)hit.Index );
                else { Slot = (ModuleSlot)hit.Index; Option = -1; Message = "SELECT OR DRAG A MODULE"; }
                break;
            case DesignerControl.Module:
                var module = GameCatalog.Module( Slot, hit.Index );
                if ( module.Requirement is { } tech && !fleet.Researched( tech ) ) {
                    Message = $"REQUIRES {FleetSimulation.Tech( tech ).Name}";
                    Option = -1;
                    break;
                }
                Option = hit.Index;
                Dragging = true;
                Message = $"PLACE {module.Name}";
                break;
            case DesignerControl.Save:
                Message = fleet.SaveDesign( Hull, Draft ) ? "DESIGN SAVED / NEW BUILDS AND REFITS"
                    : !Draft.Valid ? "REACTOR OVERLOAD / CHANGE MODULES" : "A FITTED MODULE REQUIRES RESEARCH";
                break;
            case DesignerControl.Reset: Open( fleet, Hull ); break;
        }
    }

    internal void Release( DesignerHit target ) {
        if ( !Dragging ) return;
        Dragging = false;
        if ( target.Control == DesignerControl.Slot ) Drop( (ModuleSlot)target.Index );
    }

    private void Drop( ModuleSlot target ) {
        if ( target != Slot ) { Message = "MODULE DOES NOT FIT THIS SLOT"; return; }
        Draft = Draft.Install( Slot, Option );
        Option = -1;
        Message = Draft.Valid ? "FITTED / SAVE TO APPLY" : "FITTED / REACTOR OVERLOAD";
    }
}

internal sealed partial class FleetHud {
    private readonly record struct DesignerLayout( float X, float Y, float Width, float Height ) {
        internal float HullWidth => Width >= 540 ? 118 : 82;
        internal float FitWidth => Width - HullWidth - StatsWidth - 40;
        internal float StatsWidth => Width >= 540 ? 146 : 106;
        internal float FitX => X + 18 + HullWidth;
        internal float StatsX => X + Width - StatsWidth - 12;
        internal float ModuleY => Y + Height - 89;
        internal float SlotY( int i ) => ModuleY - (6 - i) * 21 - 6;
        internal float OptionWidth( int count ) => (Width - 24 - (count - 1) * 5) / count;
    }

    private static DesignerLayout DesignerArea( Layout layout ) {
        float width = Math.Min( 920, layout.Width - 28 );
        float height = Math.Min( 550, layout.Height - 75 );
        return new( (layout.Width - width) * 0.5f, 60, width, height );
    }

    private void DrawDesigner( Layout layout, FleetSimulation fleet, ShipDesigner designer, Vector2? pointer ) {
        var a = DesignerArea( layout );
        float x = a.X, y = a.Y;
        var draft = designer.Draft;
        var hull = fleet.Hull( designer.Hull, draft );
        _overlay = 2;
        PanelAt( x, y, a.Width, a.Height, SpaceDraw.Cyan );
        Icon( HudIcon.Design, x + 12, y + 10, 16, SpaceDraw.Cyan );
        Text( "SHIP DESIGNER", x + 36, y + 13, 1.45f, White );
        Button( x + a.Width - 35, y + 7, 24, "X", true );
        if ( a.Height < 285 ) {
            FitText( "ENLARGE THE WINDOW TO FIT MODULES", x + 13, y + 49, a.Width - 27, 1, Label );
            _overlay = 0;
            return;
        }
        float side = x + 10;
        var blueprints = FleetSimulation.WarshipBlueprints;
        for ( int i = 0; i < blueprints.Length; ++i ) {
            var item = blueprints[i];
            var definition = GameCatalog.Hull( FleetSimulation.BlueprintRole( item ) );
            bool unlocked = definition.Requirement is not { } requirement || fleet.Researched( requirement );
            float row = y + 37 + i * 21;
            Rect( side, row, a.HullWidth, 19, item == designer.Hull ? Active : Well );
            FitText( definition.Name, side + 5, row + 6, a.HullWidth - 9, 1, unlocked ? White : Disabled );
        }

        float previewHeight = a.SlotY( 0 ) - y - 61;
        if ( previewHeight >= 26 ) {
            Art( HullArt( hull.Role ), a.FitX, y + 34, a.FitWidth, previewHeight, crop: true );
            Rect( a.FitX, y + 34 + previewHeight, a.FitWidth, 17, Well );
            FitText( hull.Name, a.FitX + 5, y + 38 + previewHeight, a.FitWidth - 10, 1, SpaceDraw.Cyan );
        }

        for ( int i = 0; i < (int)ModuleSlot.Count; ++i ) {
            var slot = (ModuleSlot)i;
            float row = a.SlotY( i );
            Rect( a.FitX, row, a.FitWidth, 18, designer.Slot == slot ? Active : Well );
            float label = a.FitWidth >= 215 ? 78 : 58;
            FitText( ModuleLabels[i], a.FitX + 5, row + 6, label - 6, 1, Label );
            FitText( draft.Installed( slot ), a.FitX + label, row + 6, a.FitWidth - label - 5, 1, White );
        }

        float stat = a.StatsX;
        float power = (float)draft.PowerUsed / Math.Max( 1, draft.PowerAvailable );
        var tint = draft.Valid ? ScienceTint : SpaceDraw.Amber;
        FitText( $"POWER {draft.PowerUsed}/{draft.PowerAvailable}", stat, y + 44, a.StatsWidth, 1.1f, tint );
        Progress( new( stat, y + 61 ), a.StatsWidth - 4, Math.Clamp( power, 0, 1 ), tint );
        var motion = FleetSimulation.Flight( hull.Role, draft, fleet.DriveUpgrade );
        FitText( $"HULL {FleetSimulation.MaxHull( hull.Role )}", stat, y + 78, a.StatsWidth, 1, Label );
        FitText( $"ARMOR {draft.Armor}", stat, y + 95, a.StatsWidth, 1, Label );
        FitText( $"SHIELD {draft.Shield}", stat, y + 112, a.StatsWidth, 1, Label );
        FitText( $"DMG {draft.Damage} / RNG {draft.Range}", stat, y + 129, a.StatsWidth, 1, White );
        FitText( $"CRUISE {motion.Cruise:F0}", stat, y + 146, a.StatsWidth, 1, White );
        Cost( hull.Cost, stat, y + 165 );
        Button( stat, a.ModuleY - 32, a.StatsWidth - 3, "SAVE DESIGN", draft.Valid );
        Button( stat, a.ModuleY - 56, a.StatsWidth - 3, "RESET FIT", true );

        var options = GameCatalog.Options( designer.Slot );
        float cardWidth = a.OptionWidth( options.Length );
        for ( int i = 0; i < options.Length; ++i ) {
            var option = options[i];
            bool unlocked = option.Requirement is not { } requirement || fleet.Researched( requirement );
            float left = x + 12 + i * (cardWidth + 5);
            Rect( left, a.ModuleY, cardWidth, 43, designer.Option == i ? Active : Well );
            FitText( option.Name, left + 4, a.ModuleY + 6, cardWidth - 8, 1, unlocked ? White : Disabled );
            if ( unlocked ) {
                FitText( option.Supply > 0 ? $"+{option.Supply} POWER" : $"{option.Draw} POWER", left + 4, a.ModuleY + 22, cardWidth - 8, 0.9f, EnergyTint );
            } else FitText( FleetSimulation.Tech( option.Requirement!.Value ).Name, left + 4, a.ModuleY + 22, cardWidth - 8, 0.9f, Disabled );
        }
        FitText( designer.Message, x + 12, a.ModuleY + 56, a.Width - 24, 1, draft.Valid ? Label : SpaceDraw.Amber );
        if ( designer.Dragging && designer.Option >= 0 && pointer is { } pixel ) {
            var point = Vector2.Clamp( pixel / _scale + new Vector2( 12, -14 ), new( 4, 54 ), new( layout.Width - 151, layout.Height - 28 ) );
            _overlay = 3;
            Rect( point.X, point.Y, 147, 21, Active );
            FitText( GameCatalog.Module( designer.Slot, designer.Option ).Name, point.X + 5, point.Y + 7, 137, 1, White );
        }
        _overlay = 0;
    }

    internal static DesignerHit HitDesigner( Vector2 pixel, int width, int height, ShipDesigner designer ) {
        float scale = HudScale( width, height );
        var p = pixel / scale;
        var a = DesignerArea( new( width / scale, height / scale ) );
        if ( Contains( p, a.X + a.Width - 35, a.Y + 7, 24, 21 ) ) return new( DesignerControl.Close );
        if ( a.Height < 285 ) return default;
        for ( int i = 0; i < FleetSimulation.WarshipBlueprints.Length; ++i )
            if ( Contains( p, a.X + 10, a.Y + 37 + i * 21, a.HullWidth, 19 ) )
                return new( DesignerControl.Hull, (int)FleetSimulation.WarshipBlueprints[i] );
        for ( int i = 0; i < (int)ModuleSlot.Count; ++i )
            if ( Contains( p, a.FitX, a.SlotY( i ), a.FitWidth, 18 ) ) return new( DesignerControl.Slot, i );
        int count = GameCatalog.Options( designer.Slot ).Length;
        float cardWidth = a.OptionWidth( count );
        for ( int i = 0; i < count; ++i )
            if ( Contains( p, a.X + 12 + i * (cardWidth + 5), a.ModuleY, cardWidth, 43 ) ) return new( DesignerControl.Module, i );
        if ( Contains( p, a.StatsX, a.ModuleY - 32, a.StatsWidth - 3, 21 ) ) return new( DesignerControl.Save );
        if ( Contains( p, a.StatsX, a.ModuleY - 56, a.StatsWidth - 3, 21 ) ) return new( DesignerControl.Reset );
        return default;
    }
}
