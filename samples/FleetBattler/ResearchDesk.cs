using System.Numerics;
using DXSharp.Graphics;

namespace FleetBattler;

internal sealed class ResearchDesk {
    internal ResearchBranch Page;
    internal ResearchTopic Project = ResearchTopic.Industry;

    internal void Select( ResearchTopic topic ) {
        Project = topic;
        Page = ResearchCatalog.Current.PageOf( topic );
    }
}

internal sealed partial class FleetHud {
    internal static Vector2 ResearchBackPixel( int width, int height ) {
        float scale = HudScale( width, height );
        return new( width - 55 * scale, 22 * scale );
    }

    // Render and hit-test the same rectangles, including projects reached through a prerequisite link.
    internal bool ResearchStation( ResearchDesk desk, FleetSimulation? fleet, int width, int height,
        GraphicsCommandList? commands = null, PipelineState? pipeline = null, GpuResource? upload = null, Vector2? click = null ) {
        Begin( width, height );
        float w = width / _scale, h = height / _scale;
        float gap = 12, left = 16, split = Math.Clamp( w * .47f, 175, 400 ), right = split + gap;
        float detail = w - right - left;
        bool draw = commands is not null;
        bool Hit( float x, float y, float wide, float tall ) => click is { } p && Contains( p / _scale, x, y, wide, tall );
        if ( draw ) {
            Rect( 0, 0, w, h, Panel, 1 );
            Text( fleet is null ? "TECHNOLOGY ARCHIVE" : "RESEARCH DIRECTORATE", left, 16, 2.2f, White );
            FitText( fleet is null ? "CAMPAIGN REFERENCE / PROJECTS AND PREREQUISITES" :
                $"SCIENCE {fleet.Science}   /   LAB OUTPUT {fleet.ScienceIncome} PER CYCLE", left, 44, w - 120, 1, ScienceTint );
            Button( w - 93, 12, 76, fleet is null ? "BACK TO TITLE" : "BACK TO FLIGHT", true );
        }
        if ( Hit( w - 93, 12, 76, 21 ) ) return true;
        int branchCount = (int)ResearchBranch.Count;
        float button = (w - 2*left) / branchCount;
        for ( int i = 0; i < branchCount; ++i ) {
            float x = left + button * i;
            if ( draw ) IconButton( x, 65, button - 3, BranchIcons[i], ResearchLabels[i], true, i == (int)desk.Page % branchCount );
            if ( Hit( x, 65, button - 3, 21 ) ) { desk.Select( ResearchCatalog.Current.Topics( (ResearchBranch)i )[0] ); return false; }
        }
        var topics = ResearchCatalog.Current.Topics( desk.Page );
        int pages = ResearchCatalog.Current.PageCount( desk.Page );
        if ( draw ) {
            Text( "PROJECTS", left, 101, 1.35f, White );
            if ( pages > 1 ) Button( split - 89, 92, 89, $"PAGE {(int)desk.Page / branchCount + 1}/{pages}", true );
        }
        if ( pages > 1 && Hit( split - 89, 92, 89, 21 ) ) {
            desk.Select( ResearchCatalog.Current.Topics( ResearchCatalog.Current.NextPage( desk.Page ) )[0] ); return false;
        }
        float rowHeight = Math.Min( 64, (h - 149) / 4 );
        for ( int i = 0; i < topics.Length; ++i ) {
            var topic = topics[i];
            var project = FleetSimulation.Tech( topic );
            float y = 120 + i * rowHeight;
            if ( draw ) {
                Rect( left, y, split - left, rowHeight - 4, desk.Project == topic ? Active : Well );
                Art( (Tile)project.Artwork, left + 4, y + 4, 35, rowHeight - 12 );
                FitText( project.Name, left + 46, y + 7, split - left - 50, 1.1f, White );
                if ( fleet is not null ) {
                    string state = fleet.Researched( topic ) ? "COMPLETE" : fleet.ActiveResearch == topic ? "IN PROGRESS" :
                        fleet.CanResearch( topic ) ? "READY TO START" : "LOCKED";
                    FitText( state, left + 46, y + 24, split - left - 50, .95f, state == "LOCKED" ? Disabled : SpaceDraw.Cyan );
                }
            }
            if ( Hit( left, y, split - left, rowHeight - 4 ) ) { desk.Select( topic ); return false; }
        }
        var tech = FleetSimulation.Tech( desk.Project );
        float artHeight = Math.Clamp( h * .17f, 42, 110 );
        float title = 100 + artHeight;
        float action = h - 45;
        if ( draw ) {
            Art( (Tile)tech.Artwork, right, 93, detail, artHeight, crop: true );
            FitText( tech.Name, right, title, detail, 1.55f, White );
            FitText( tech.Benefit, right, title + 18, detail, 1.05f, EnergyTint );
            WrapText( tech.Description, right, title + 37, detail, Math.Min( 56, Math.Max( 12, action-title-98 ) ), 1, Label );
        }
        float linkY = title + Math.Min( 97, Math.Max( 55, action-title-53 ) );
        foreach ( var prerequisite in tech.Requires ) {
            if ( linkY + 21 >= action ) break;
            if ( draw ) Button( right, linkY, detail, $"REQUIRES / {FleetSimulation.Tech( prerequisite ).Name}", true );
            if ( Hit( right, linkY, detail, 21 ) ) { desk.Select( prerequisite ); return false; }
            linkY += 23;
        }
        if ( draw ) {
            Cost( tech.Cost, left, action );
            if ( fleet is not null ) {
                bool active = fleet.ActiveResearch == desk.Project;
                if ( active ) Progress( new( right, action - 7 ), detail, fleet.ResearchProgress, ScienceTint );
                Button( right, action, detail, active ? "CANCEL PROJECT" : fleet.ResearchBlock( desk.Project ) ?? "START PROJECT", active || fleet.CanResearch( desk.Project ) );
            }
            Submit( commands!, pipeline!, upload!, width, height );
        }
        if ( fleet is not null && Hit( right, action, detail, 21 ) ) {
            if ( fleet.ActiveResearch == desk.Project ) fleet.CancelResearch();
            else fleet.StartResearch( desk.Project );
        }
        return false;
    }
}
