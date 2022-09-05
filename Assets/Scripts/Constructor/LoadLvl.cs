using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLvl : MonoBehaviour
{
    [SerializeField] private LvlsConstruct lvlsConstruct = null;

    [SerializeField] private CellInfo _cellPrefab = null;

    [SerializeField] private Token[] _tokens = new Token[] { };

    public System.Action<Dictionary<Vector3, CellInfo>, List<Vector3>, int> LvlField;
    public System.Action<Dictionary<Vector3, CellInfo>, List<Vector3>, int, int> LvlFieldWithPoints;
    public System.Action<Dictionary<Vector3, CellInfo>, List<Vector3>, List<TokenTarget>, int> LvlFieldWithTokens;
    public System.Action<int> StepCount;
    
    private Dictionary<string, Token> _tokensType = new Dictionary<string, Token>();

    private void Awake()
    {
        ConstructTokensType();
    }

    private void ConstructTokensType()
    {
        foreach (var token in _tokens)
        {
            _tokensType.Add(token.GetType().ToString(), token);
        }
    }

    private void Load(int numberLvl)
    {
        Dictionary<Vector3, CellInfo> field = new Dictionary<Vector3, CellInfo>();
        List<Vector3> spawnPoints = new List<Vector3>();

        foreach (var cellData in lvlsConstruct.LvlsData[numberLvl].Field)
        {
            var cell = Instantiate(_cellPrefab, cellData.Position, Quaternion.identity);

            if (!cellData.TokenType.Equals("null"))
            {
                var token = Instantiate(ExtractToken(cellData.TokenType), cell.transform.position, Quaternion.identity);
                token.Hp = cellData.TokenHp;
                Debug.Log(token.Hp);
                token.Init();
                cell.ActualToken = token;
            }

            field.Add(cell.transform.position, cell);
        }

        foreach (var spawnPoint in lvlsConstruct.LvlsData[numberLvl].SpawnPoints)
        {
            spawnPoints.Add(spawnPoint);
        }

        LvlField?.Invoke(field, spawnPoints, lvlsConstruct.LvlsData[numberLvl].ScoreForStars);
        StepCount?.Invoke(lvlsConstruct.LvlsData[numberLvl].StepCount);
    }

    public void Load(LvlData lvlData)
    {
        Dictionary<Vector3, CellInfo> field = new Dictionary<Vector3, CellInfo>();
        List<Vector3> spawnPoints = new List<Vector3>();

        foreach (var cellData in lvlData.Field)
        {
            var cell = Instantiate(_cellPrefab, cellData.Position, Quaternion.identity);

            if (!cellData.TokenType.Equals("null"))
            {
                var token = Instantiate(ExtractToken(cellData.TokenType), cell.transform);
                token.Hp = cellData.TokenHp;
                Debug.Log(token.Hp);
                token.Init();
                cell.ActualToken = token;
            }

            field.Add(cell.transform.position, cell);
        }

        foreach (var spawnPoint in lvlData.SpawnPoints)
        {
            spawnPoints.Add(spawnPoint);
        }

        switch (lvlData.LevelTarget)
        {
            case LevelTarget.Points:
                LvlFieldWithPoints?.Invoke(field, spawnPoints, lvlData.PointsTarget, lvlData.ScoreForStars);
                break;
            case LevelTarget.Tokens:
                LvlFieldWithTokens?.Invoke(field, spawnPoints, lvlData.TokenTargets, lvlData.ScoreForStars);
                break;
            default:
                LvlField?.Invoke(field, spawnPoints, lvlData.ScoreForStars);
                break;
        }

        StepCount?.Invoke(lvlData.StepCount);
    }

    private Token ExtractToken(string type)
    {
        return _tokensType[type];
    }
}