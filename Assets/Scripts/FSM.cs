using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FSM
{
    public static bool Game { get; private set; } = false;
    public static bool Pause { get; private set; } = false;
    public static bool Wait { get; private set; } = false;
    public static bool EndLvl { get; private set; } = false;

    public static void SetGameStatus(GameStatus status)
    {
        switch (status)
        {
            case GameStatus.Game:
                Game = true;
                Pause = false;
                Wait = false;
                EndLvl = false;
                break;
            case GameStatus.Pause:
                Game = false;
                Pause = true;
                Wait = false;
                EndLvl = false;
                break;
            case GameStatus.Wait:
                Game = false;
                Pause = false;
                Wait = true;
                EndLvl = false;
                break;
            case GameStatus.EndLvl:
                Game = false;
                Pause = false;
                Wait = false;
                EndLvl = true;
                break;
        }
    }
}

public enum GameStatus
{
    Game,
    Pause,
    Wait,
    EndLvl,
}