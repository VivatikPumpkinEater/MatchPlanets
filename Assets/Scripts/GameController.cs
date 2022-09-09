using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private LoadLvl _fieldGenerator = null;
    [SerializeField] private LineController _lineController = null;

    [SerializeField] private Token[] _tokensPrefab = new Token[] { };

    [SerializeField] private Pool _effectsPool = null;

    [SerializeField] private float _destroyTime = 0.1f;
    [SerializeField] private float _moveTime = 0.1f;

    [SerializeField] private PointsManager _pointsManager = null;
    [SerializeField] private TokensTargetController _tokensTargetController = null;

    [SerializeField] private Transform _endPoint = null;

    [SerializeField] private RipplePostProcessor _ripplePostProcessor = null;

    public List<Token> Tokens => _tokens;

    private List<Token> _tokens = new List<Token>();

    private TokenMove _tokenMove;

    private Dictionary<Vector3, CellInfo> _fields;
    private List<Vector3> _spawnPoints;

    private bool _move = false;
    private bool _lvlTokenTarget = false;

    private void Awake()
    {
        _fieldGenerator.LvlLoadedEvent += InitLvl;
        _lineController.TokenToDestroy += tokens => StartCoroutine(DeleteToken(tokens));
    }

    private void InitLvl(Dictionary<Vector3, CellInfo> field, List<Vector3> spawnPoints, LvlData lvlData)
    {
        _fields = field;
        _spawnPoints = spawnPoints;

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
        }

        SpawnToken(field);

        FSM.SetGameStatus(GameStatus.Game);
    }

    private void SpawnToken(Dictionary<Vector3, CellInfo> field)
    {
        foreach (var cell in field.Keys)
        {
            if (!field[cell].ActualToken)
            {
                var token = Instantiate(_tokensPrefab[Random.Range(0, _tokensPrefab.Length)],
                    field[cell].transform.position, Quaternion.identity);

                token.transform.parent = transform;

                token.transform.localScale = Vector3.zero;
                token.transform.localRotation = Quaternion.Euler(0, 0, 360);

                token.transform.DOScale(Vector3.one, 1f);
                token.transform.DOLocalRotate(Vector3.zero, 1f);

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
                    var token = Instantiate(_tokensPrefab[Random.Range(0, _tokensPrefab.Length)],
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

    private IEnumerator DeleteToken(Token[] tokens)
    {
        FSM.SetGameStatus(GameStatus.Wait);

        foreach (var token in tokens)
        {
            if (token && _fields.ContainsKey(token.transform.position) && _fields[token.transform.position].ActualToken)
            {
                AudioManager.LoadEffect("TokenDestroy");

                _fields[token.transform.position].ActualToken = null;

                _tokens.Remove(token);

                _effectsPool.GetFreeElement(token.transform.position);

                var checkNeighbours = CheckNeighbours(token.transform.position);

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

                    yield return new WaitForSecondsRealtime(_destroyTime);
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

                yield return new WaitForSecondsRealtime(_destroyTime);
            }
        }

        yield return new WaitForSecondsRealtime(_destroyTime);

        StartCoroutine(Movement3());
    }

    private void DestroyTokens(Token token)
    {
        if (token.Destroy())
        {
            _fields[token.transform.position].ActualToken = null;
            _tokens.Remove(token);
            _effectsPool.GetFreeElement(token.transform.position);

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

    private IEnumerator Movement3()
    {
        bool accept = true;

        while (accept)
        {
            accept = false;

            if (_tokens.Count == 0)
            {
                SpawnToken();
                yield return new WaitForSeconds(_moveTime);
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

                        _fields[tokenMove.startPosition].ActualToken = null;

                        _fields[tokenMove.endPosition].ActualToken = tokenMove.Token;

                        float moveDuration = _moveTime * tokenMove.Iteration;

                        tokenMove.Token.transform.DOMove(tokenMove.endPosition, moveDuration).OnComplete(() =>
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

            yield return null;
        }

        yield return null;

        Debug.Log("END MOVE");


        FSM.SetGameStatus(GameStatus.Game);
    }

    private TokenMove SearchToken(Vector3 start)
    {
        _tokenMove = new TokenMove();

        for (int y = 1; y < 12; y++)
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

        for (int i = 0; i < 2; i++)
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
            _tokenMove.startPosition = _fields[tokenPos].transform.position;
            _tokenMove.endPosition = start;
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

        var tokensToDestroy = new List<Token>();

        for (int i = 0; i < checkPosition.Length; i++)
        {
            if (_fields.ContainsKey(checkPosition[i]) && _fields[checkPosition[i]].ActualToken && (
                    _fields[checkPosition[i]].ActualToken.Type.Equals(TokenType.Ice) ||
                    _fields[checkPosition[i]].ActualToken.Type.Equals(TokenType.Rock)))
            {
                tokensToDestroy.Add(_fields[checkPosition[i]].ActualToken);
            }
        }

        return tokensToDestroy;
    }
}

public struct TokenMove
{
    public Token Token;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public int Iteration;
}