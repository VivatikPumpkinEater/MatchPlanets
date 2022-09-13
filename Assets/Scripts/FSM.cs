
public static class FSM
{
    public static GameStatus Status { get; set; } = GameStatus.Wait;
}

public enum GameStatus
{
    Game,
    Pause,
    Wait,
    EndLvl,
    Loading,
    Loaded
}