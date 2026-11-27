namespace FleetBattler;

internal enum GameScreen { MainMenu, ResearchArchive, Loading, Flight, PauseMenu }
internal enum MenuAction { Launch, NextScenario, Faction, ResearchArchive, Resume, ReturnToTitle, Sound, Display, Exit, Music }
internal readonly record struct ScenarioChoice( string Id, string Name, string Description ) {
    internal bool Skirmish => Id.Length == 0;
}

internal sealed class GameFlow( ScenarioChoice[] scenarios, int initialScenario, bool showTitle ) {
    private static readonly MenuAction[] TitleActions =
        [MenuAction.Launch, MenuAction.NextScenario, MenuAction.Faction, MenuAction.ResearchArchive, MenuAction.Sound, MenuAction.Display, MenuAction.Exit, MenuAction.Music];
    private static readonly MenuAction[] PauseActions =
        [MenuAction.Resume, MenuAction.Sound, MenuAction.Display, MenuAction.ReturnToTitle, MenuAction.Exit, MenuAction.Music];
    private int _scenario = initialScenario;

    internal GameScreen Screen { get; private set; } = showTitle ? GameScreen.MainMenu : GameScreen.Flight;
    internal int Focus { get; private set; }
    internal ScenarioChoice Scenario => scenarios[_scenario];
    internal bool IsTitle => Screen is GameScreen.MainMenu or GameScreen.ResearchArchive or GameScreen.Loading;
    internal ReadOnlySpan<MenuAction> Actions => Screen switch {
        GameScreen.MainMenu => TitleActions,
        GameScreen.PauseMenu => PauseActions,
        _ => []
    };

    internal void FocusRow( int row ) {
        if ( (uint)row < (uint)Actions.Length ) Focus = row;
    }

    internal void MoveFocus( int direction ) {
        int count = Actions.Length;
        if ( count != 0 ) Focus = (Focus + count + direction) % count;
    }

    internal void Navigate( MenuAction action ) {
        switch ( Screen, action ) {
            case (GameScreen.MainMenu, MenuAction.NextScenario):
                _scenario = (_scenario + 1) % scenarios.Length;
                return;
            case (GameScreen.MainMenu, MenuAction.Launch): Screen = GameScreen.Loading; break;
            case (GameScreen.MainMenu, MenuAction.ResearchArchive): Screen = GameScreen.ResearchArchive; break;
            case (GameScreen.PauseMenu, MenuAction.Resume): Screen = GameScreen.Flight; break;
            case (GameScreen.PauseMenu, MenuAction.ReturnToTitle): Screen = GameScreen.MainMenu; break;
            default: return;
        }
        Focus = 0;
    }

    internal void Back() {
        Screen = Screen switch {
            GameScreen.ResearchArchive => GameScreen.MainMenu,
            GameScreen.Flight => GameScreen.PauseMenu,
            GameScreen.PauseMenu => GameScreen.Flight,
            _ => Screen
        };
        Focus = 0;
    }

    internal void FinishLoading() {
        if ( Screen != GameScreen.Loading ) throw new InvalidOperationException( "Launch must be requested before flight begins." );
        Screen = GameScreen.Flight;
    }
}

internal sealed class GameSettings {
    internal bool Muted, Fullscreen, ShowMenu, MenuTest, AudioTest;
}
