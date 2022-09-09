using System.Collections.Generic;
using UnityEngine;

public class LoadLvl : MonoBehaviour
{
    [SerializeField] private LvlsConstruct _lvlsConstruct = null;

    [SerializeField] private CellInfo _cellPrefab = null;

    [SerializeField] private Token[] _tokens = new Token[] { };
    
    public System.Action<Dictionary<Vector3, CellInfo>, List<Vector3>, LvlData> LvlLoadedEvent;
    public System.Action<int> StepsLoadedEvent;
    
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
    
    public void Load(LvlData lvlData)
    {
        var field = new Dictionary<Vector3, CellInfo>();
        var spawnPoints = new List<Vector3>();

        foreach (var cellData in lvlData.Field)
        {
            var cell = Instantiate(_cellPrefab, cellData.Position, Quaternion.identity);

            if (!cellData.TokenType.Equals("null"))
            {
                var token = Instantiate(ExtractToken(cellData.TokenType), cell.transform);
                token.Hp = cellData.TokenHp;
                token.Init();
                cell.ActualToken = token;
            }

            field.Add(cell.transform.position, cell);
        }

        foreach (var spawnPoint in lvlData.SpawnPoints)
        {
            spawnPoints.Add(spawnPoint);
        }
        
        LvlLoadedEvent?.Invoke(field, spawnPoints, lvlData);

        StepsLoadedEvent?.Invoke(lvlData.StepCount);
    }

    private Token ExtractToken(string type)
    {
        return _tokensType[type];
    }
}