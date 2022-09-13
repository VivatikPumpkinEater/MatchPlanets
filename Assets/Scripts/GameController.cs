using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private LineController _lineController;

    [SerializeField] private CellInfo _cellPrefab;
    [SerializeField] private Token[] _tokensPrefab;

    [SerializeField] private Pool _effectsPool;

    [SerializeField] private float _destroyTime;
    [SerializeField] private float _moveTime;

    [SerializeField] private PointsManager _pointsManager;
    [SerializeField] private TokensTargetController _tokensTargetController;

    [SerializeField] private Transform _endPoint;

    [SerializeField] private RipplePostProcessor _ripplePostProcessor;

    public System.Action<int> StepsLoadedEvent;
    public List<Token> Tokens => _tokens;

    private List<Token> _tokens = new List<Token>();

    private Dictionary<string, Token> _tokensType = new Dictionary<string, Token>();

    private TokenMove _tokenMove;

    private Dictionary<Vector3, CellInfo> _fields = new Dictionary<Vector3, CellInfo>();
    private List<Vector3> _spawnPoints = new List<Vector3>();
    
    private bool _lvlTokenTarget;
    
    private int _moveTimeMilliseconds;
    private int _destroyTimeMilliseconds;

    private void Awake()
    {
        _lineController.DestroyedTokensEvent += tokens => DeleteToken(tokens).Forget();

        _moveTimeMilliseconds = (int)(_moveTime * 1000);
        _destroyTimeMilliseconds = (int)(_destroyTime * 1000);
    }

    public void Load(LvlData lvlData)
    {
        ConstructTokensType();

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

            _fields.Add(cell.transform.position, cell);
        }

        foreach (var spawnPoint in lvlData.SpawnPoints)
        {
            _spawnPoints.Add(spawnPoint);
        }

        StepsLoadedEvent?.Invoke(lvlData.StepCount);

        _pointsManager.InitPerfectScore(lvlData.ScoreForStars);

        switch (lvlData.LevelTarget)
        {
            case LevelTarget.Points:
                _pointsManager.InitPointsTarget(lvlData.PointsTarget);
                break;
            case LevelTarget.Tokens:
                _lvlTokenTarget = true;

                var tokensTargets = lvlData.TokenTargets;
                foreach (var tokenTarget in tokensTargets)
                {
                    _tokensTargetController.SetUpTargetsToken(tokenTarget.Type, tokenTarget.Sprite, tokenTarget.Count);
                }

                break;
            default:
                _pointsManager.InitPointsTarget(lvlData.ScoreForStars);
                break;
        }

        SpawnToken(_fields);

        FSM.Status = GameStatus.Game;
    }

    private void ConstructTokensType()
    {
        foreach (var token in _tokensPrefab)
        {
            _tokensType.Add(token.GetType().ToString(), token);
        }
    }

    private Token ExtractToken(string type)
    {
        return _tokensType[type];
    }

    private void SpawnToken(Dictionary<Vector3, CellInfo> field)
    {
        foreach (var cell in field.Keys)
        {
            if (!field[cell].ActualToken)
            {
                var token = Instantiate(_tokensPrefab[Random.Range(0, 5)],
                    field[cell].transform.position, Quaternion.identity);

                var tokenTransform = token.transform;
                tokenTransform.parent = transform;

                tokenTransform.localScale = Vector3.zero;
                tokenTransform.localRotation = Quaternion.Euler(0, 0, 360);

                tokenTransform.DOScale(Vector3.one, 1f);
                tokenTransform.DOLocalRotate(Vector3.zero, 1f);

                field[cell].ActualToken = token;

                _tokens.Add(token);
            }
        }
    }


    private void SpawnToken()
    {
        var newTokens = new List<Token>();

        foreach (var spawnPoint in _spawnPoints)
        {
            var position = spawnPoint + Vector3.down;

            if (_fields.ContainsKey(position))
            {
                if (_fields[position].ActualToken == null)
                {
                    var token = Instantiate(_tokensPrefab[Random.Range(0, 5)],
                        _fields[position].transform.position + Vector3.up,
                        Quaternion.identity);

                    token.transform.parent = transform;

                    _fields[position].ActualToken = token;

                    _tokens.Add(token);

                    newTokens.Add(token);
                }
            }
        }

        foreach (var tokens in newTokens)
        {
            tokens.Moving = true;
            tokens.transform.DOMove(tokens.transform.position + Vector3.down, _moveTime)
                .OnComplete(() => tokens.Moving = false);
        }
    }

    private async UniTaskVoid DeleteToken(Token[] tokens)
    {
        FSM.Status = GameStatus.Wait;

        foreach (var token in tokens)
        {
            if (token && _fields.ContainsKey(token.transform.position) && _fields[token.transform.position].ActualToken)
            {
                AudioManager.LoadEffect("TokenDestroy");

                var position = token.transform.position;

                _fields[position].ActualToken = null;

                _tokens.Remove(token);

                _effectsPool.GetFreeElement(position);

                var checkNeighbours = CheckNeighbours(position);

                foreach (var token1 in checkNeighbours)
                {
                    DestroyTokens(token1);
                }

                if (token.Bonus)
                {
                    var toDestroy = token.Bonus.Activate();

                    switch (token.Bonus.GetType().ToString())
                    {
                        case "Bomb":
                            AudioManager.LoadEffect("Boom");
                            _ripplePostProcessor.RippleEffect(token.transform.position);
                            break;
                        case "Rocket":
                            AudioManager.LoadEffect("Rocket");
                            break;
                    }

                    foreach (var key in toDestroy)
                    {
                        if (_fields.ContainsKey(key) && _fields[key].ActualToken && !_fields[key].ActualToken.Bonus)
                        {
                            var tmp = _fields[key].ActualToken;

                            DestroyTokens(tmp);
                        }
                    }

                    await UniTask.Delay(_destroyTimeMilliseconds);
                }

                if (_endPoint != null)
                {
                    token.SpriteRenderer.sortingOrder = 5;

                    token.transform.DOScale(Vector3.zero, _moveTime * 12);
                    token.transform.DOJump(_endPoint.position, 3, 1, _moveTime * 10).OnComplete
                    (
                        () =>
                        {
                            Destroy(token.gameObject);
                            _pointsManager.AddedPoint(40);

                            if (_lvlTokenTarget)
                            {
                                _tokensTargetController.MinusTargetCount(token.GetType().ToString());
                            }
                        });
                }
                else
                {
                    Destroy(token.gameObject);
                }

                await UniTask.Delay(_destroyTimeMilliseconds);
            }
        }

        await UniTask.Delay(_destroyTimeMilliseconds);

        Movement3().Forget();
    }

    private void DestroyTokens(Token token)
    {
        if (token.Destroy())
        {
            var position = token.transform.position;
            _fields[position].ActualToken = null;
            _tokens.Remove(token);
            _effectsPool.GetFreeElement(position);

            token.SpriteRenderer.sortingOrder = 5;

            token.transform.DOScale(Vector3.zero, _moveTime * 12);
            token.transform.DOJump(_endPoint.position, 3, 1, _moveTime * 10).OnComplete
            (
                () =>
                {
                    Destroy(token.gameObject);
                    _pointsManager.AddedPoint(40);

                    if (_lvlTokenTarget)
                    {
                        _tokensTargetController.MinusTargetCount(token.GetType().ToString());
                    }
                });
        }
        else
        {
            if (_lvlTokenTarget)
            {
                _tokensTargetController.MinusTargetCount(token.GetType().ToString());
            }
        }
    }

    private async UniTaskVoid Movement3()
    {
        var accept = true;

        while (accept)
        {
            accept = false;

            if (_tokens.Count == 0)
            {
                SpawnToken();
                await UniTask.Delay(_moveTimeMilliseconds);
            }

            foreach (var key in _fields.Keys)
            {
                var cell = _fields[key];

                if (!cell.ActualToken)
                {
                    var tokenMove = SearchToken(key);

                    if (!tokenMove.Token)
                    {
                        continue;
                    }

                    if (tokenMove.Token && !tokenMove.Token.Moving)
                    {
                        tokenMove.Token.Moving = true;

                        _fields[tokenMove.StartPosition].ActualToken = null;

                        _fields[tokenMove.EndPosition].ActualToken = tokenMove.Token;

                        var moveDuration = _moveTime * tokenMove.Iteration;

                        tokenMove.Token.transform.DOMove(tokenMove.EndPosition, moveDuration).OnComplete(() =>
                        {
                            tokenMove.Token.Moving = false;
                        });
                    }

                    accept = true;
                }
            }

            if (accept)
            {
                SpawnToken();
            }

            await UniTask.DelayFrame(0);
        }

        await UniTask.DelayFrame(0);

        Debug.Log("END MOVE");


        FSM.Status = GameStatus.Game;
    }

    private TokenMove SearchToken(Vector3 start)
    {
        _tokenMove = new TokenMove();

        for (var y = 1; y < 12; y++)
        {
            var tokenPos = start + Vector3.up * y;

            if ((_fields.ContainsKey(tokenPos) && _fields[tokenPos].ActualToken &&
                 _fields[tokenPos].ActualToken.Type.Equals(TokenType.Ice)) || !_fields.ContainsKey(tokenPos))
            {
                break;
            }
            else
            {
                if (SearchHelper(tokenPos, start, y))
                {
                    return _tokenMove;
                }
            }
        }

        for (var i = 0; i < 2; i++)
        {
            var tokenPos = Vector3.positiveInfinity;

            switch (i)
            {
                case 0:
                    tokenPos = start + Vector3.up + Vector3.left;
                    break;
                case 1:
                    tokenPos = start + Vector3.up + Vector3.right;
                    break;
            }

            if (SearchHelper(tokenPos, start))
            {
                return _tokenMove;
            }
        }

        return _tokenMove;
    }

    private bool SearchHelper(Vector3 tokenPos, Vector3 start)
    {
        if (_fields.ContainsKey(tokenPos) && _fields[tokenPos].ActualToken)
        {
            if (_fields[tokenPos].ActualToken.Type.Equals(TokenType.Ice))
            {
                return false;
            }

            _tokenMove.Token = _fields[tokenPos].ActualToken;
            _tokenMove.StartPosition = _fields[tokenPos].transform.position;
            _tokenMove.EndPosition = start;
            _tokenMove.Iteration = 1;

            return true;
        }

        return false;
    }

    private bool SearchHelper(Vector3 tokenPos, Vector3 start, int iteration)
    {
        if (SearchHelper(tokenPos, start))
        {
            _tokenMove.Iteration = iteration;

            return true;
        }

        return false;
    }

    private List<Token> CheckNeighbours(Vector3 start)
    {
        var checkPosition = new Vector3[4];
        checkPosition[0] = start + Vector3.up;
        checkPosition[1] = start + Vector3.right;
        checkPosition[2] = start + Vector3.down;
        checkPosition[3] = start + Vector3.left;

        return
            (from position in checkPosition
                where _fields.ContainsKey(position) && _fields[position].ActualToken &&
                      (_fields[position].ActualToken.Type.Equals(TokenType.Ice) ||
                       _fields[position].ActualToken.Type.Equals(TokenType.Rock))
                select _fields[position].ActualToken
            ).ToList();
    }
}

public struct TokenMove
{
    public Token Token;
    public Vector3 StartPosition;
    public Vector3 EndPosition;
    public int Iteration;
}