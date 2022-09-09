
public static class FSM
{
    public static GameStatus Status { get; private set; } = GameStatus.Wait;
    public static void SetGameStatus(GameStatus status)
    {
        Status = status;
    }
}

public enum GameStatus
{
    Game,
    Pause,
    Wait,
    EndLvl,
}