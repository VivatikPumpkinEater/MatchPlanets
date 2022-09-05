using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lvl/lvlPack", fileName = "LvlsData")]
public class LvlsConstruct : ScriptableObject
{
    [field: SerializeField] public List<LvlData> LvlsData = new List<LvlData>();
}

[Serializable]
public struct LvlData
{
    public string Name;
    public int StepCount;
    public List<CellData> Field;
    public List<Vector3> SpawnPoints;
    public int ScoreForStars;

    public bool LevelUnlock;
    public bool LevelPassed;
    public int Stars;

    public LevelTarget LevelTarget;
    
    public int PointsTarget;
    public List<TokenTarget> TokenTargets;
}

[Serializable]
public struct CellData
{
    public Vector3 Position;
    public string TokenType;
    public int TokenHp;
}

[Serializable]
public enum LevelTarget
{
    Points,
    Tokens
}

[Serializable]
public struct TokenTarget
{
    public string Type;
    public int Count;
    public Sprite Sprite;
}
