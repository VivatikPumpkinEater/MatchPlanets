using System;
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

    [SerializeField] private Transform _greenEndValue = null;

    [SerializeField] private RipplePostProcessor _ripplePostProcessor = null;

    public List<Token> Tokens => _tokens;

    private List<Token> _tokens = new List<Token>();
    private List<Vector3> _emptyCells = new List<Vector3>();

    private Dictionary<Vector3, CellInfo> _fields = new Dictionary<Vector3, CellInfo>();
    private List<Vector3> _spawnPoints = new List<Vector3>();
    private Dictionary<Token, Vector3> test = new Dictionary<Token, Vector3>();

    private bool _move = false;
    private bool _lvlTokenTarget = false;

    private void Awake()
    {
        _fieldGenerator.LvlField += InitLvl;
        _fieldGenerator.LvlFieldWithPoints += InitLvl;
        _fieldGenerator.LvlFieldWithTokens += InitLvl;
        _lineController.TokenToDestroy += tokens => StartCoroutine(DeleteToken(tokens));
    }

    private void InitLvl(Dictionary<Vector3, CellInfo> field, List<Vector3> spawnPoints, int scoreTarget)
    {
        _fields = field;
        _spawnPoints = spawnPoints;

        _pointsManager.InitPerfectScore(scoreTarget);
        //SpawnToken(_fields);

        //StartCoroutine(Movement2());

        SpawnToken(field);

        FSM.SetGameStatus(GameStatus.Game);
    }

    private void InitLvl(Dictionary<Vector3, CellInfo> field, List<Vector3> spawnPoints, int pointsTarget,
        int scoreTarget)
    {
        _pointsManager.InitPointsTarget(pointsTarget);
        InitLvl(field, spawnPoints, scoreTarget);
    }

    private void InitLvl(Dictionary<Vector3, CellInfo> field, List<Vector3> spawnPoints, List<TokenTarget> tokenTargets,
        int scoreTarget)
    {
        _lvlTokenTarget = true;

        foreach (var tokenTarget in tokenTargets)
        {
            _tokensTargetController.SetUpTargetsToken(tokenTarget.Type, tokenTarget.Sprite, tokenTarget.Count);
        }

        InitLvl(field, spawnPoints, scoreTarget);
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
        List<Token> newTokens = new List<Token>();

        foreach (var spawnPoint in _spawnPoints)
        {
            Vector3 position = spawnPoint + Vector3.down;

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
            if (_fields.ContainsKey(token.transform.position) && _fields[token.transform.position].ActualToken)
            {
                AudioManager.Instance.GetEffect("TokenDestroy");
                
                _fields[token.transform.position].ActualToken = null;

                _tokens.Remove(token);

                _effectsPool.GetFreeElement(token.transform.position);

                var checkNeigthbor = CheckHen(token.transform.position);

                foreach (var token1 in checkNeigthbor)
                {
                    DestroyTokens(token1);
                }

                if (token.Bonus)
                {
                    var toDestroy = token.Bonus.Activate();

                    switch (token.Bonus.GetType().ToString())
                    {
                        case"Bomb":
                            AudioManager.Instance.GetEffect("Boom");
                            _ripplePostProcessor.RippleEffect(token.transform.position);
                            break;
                        case "Rocket":
                            AudioManager.Instance.GetEffect("Rocket");
                            break;
                    }

                    foreach (var key in toDestroy)
                    {
                        if (_fields.ContainsKey(key) && _fields[key].ActualToken && !_fields[key].ActualToken.Bonus)
                        {
                            Token tmp = _fields[key].ActualToken;

                            DestroyTokens(tmp);
                        }
                    }

                    yield return new WaitForSecondsRealtime(_destroyTime);
                }

                if (_greenEndValue != null)
                {
                    token.SpriteRenderer.sortingOrder = 5;

                    token.transform.DOScale(Vector3.zero, _moveTime * 12);
                    token.transform.DOJump(_greenEndValue.position, 3, 1, _moveTime * 10).OnComplete
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

                //_emptyCells.Add(token.transform.position);

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
            token.transform.DOJump(_greenEndValue.position, 3, 1, _moveTime * 10).OnComplete
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

    private IEnumerator Movement()
    {
        int i = 1;

        while (i > 0)
        {
            i = 0;

            if (_tokens.Count == 0)
            {
                SpawnToken();
                yield return new WaitForSeconds(_moveTime);
            }

            foreach (var token in _tokens)
            {
                Vector3 position = Vector3.zero;

                switch (0)
                {
                    case 0:
                        position = token.transform.position + Vector3.down;
                        break;
                    case 1:
                        position = token.transform.position + Vector3.down + Vector3.left;
                        break;
                    case 2:
                        position = token.transform.position + Vector3.down + Vector3.right;
                        break;
                }

                if (_fields.ContainsKey(position))
                {
                    if (!_fields[position].ActualToken)
                    {
                        _fields[token.transform.position].ActualToken = null;
                        _fields[position].ActualToken = token;

                        if (test.ContainsKey(token))
                        {
                            test[token] = position;
                        }
                        else
                        {
                            test.Add(token, position);
                        }

                        i = 1;

                        break;
                    }
                }
            }

            foreach (var token in test.Keys)
            {
                token.transform.DOMove(test[token], _moveTime);
            }

            test.Clear();

            if (i == 1)
            {
                SpawnToken();
                yield return new WaitForSeconds(_moveTime);
            }
            else
            {
                yield return new WaitForSeconds(_moveTime / 10);
            }
        }

        Debug.Log("End Dest");
    }

    private IEnumerator Movement2()
    {
        bool accept = true;

        float moveTime = _moveTime;

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
                CellInfo cell = _fields[key];

                if (!cell.ActualToken)
                {
                    TokenMove tokenMove = SearchToken(key);

                    if (tokenMove.Token && !tokenMove.Token.Moving)
                    {
                        tokenMove.Token.Moving = true;

                        _fields[tokenMove.startPosition].ActualToken = null;

                        //Debug.Log(tokenMove.startPosition);

                        _fields[tokenMove.endPosition].ActualToken = tokenMove.Token;

                        float moveDuration = _moveTime * tokenMove.Iteration;

                        tokenMove.Token.transform.DOMove(tokenMove.endPosition, moveDuration).OnComplete(() =>
                        {
                            tokenMove.Token.transform.DOLocalJump(tokenMove.Token.transform.position, 0.2f, 1, 0.2f)
                                .OnComplete(() =>
                                    tokenMove.Token.Moving = false);
                        });
                    }

                    if (tokenMove.Token && tokenMove.Token.Moving)
                    {
                        accept = true;
                    }
                }
            }

            if (accept)
            {
                SpawnToken();

                yield return new WaitForSeconds(_moveTime);
            }
        }

        yield return new WaitForSeconds(0);

        Debug.Log("END MOVE");

        if (!FSM.EndLvl)
        {
            FSM.SetGameStatus(GameStatus.Game);
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
                CellInfo cell = _fields[key];

                if (!cell.ActualToken)
                {
                    TokenMove tokenMove = SearchToken(key);

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
        TokenMove tokenMove = new TokenMove();

        for (int y = 1; y < 12; y++)
        {
            Vector3 tokenPos = start + Vector3.up * y;

            if (_fields.ContainsKey(tokenPos) && _fields[tokenPos].ActualToken)
            {
                if (_fields[tokenPos].ActualToken.GetType().ToString() == "Ice")
                {
                    break;
                }

                tokenMove.Token = _fields[tokenPos].ActualToken;
                tokenMove.startPosition = _fields[tokenPos].transform.position;
                tokenMove.endPosition = start;
                tokenMove.Iteration = y;

                return tokenMove;
            }
            else if (!_fields.ContainsKey(tokenPos))
            {
                break;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            Vector3 tokenPos;

            switch (i)
            {
                case 0:
                    tokenPos = start + Vector3.up + Vector3.left;

                    if (_fields.ContainsKey(tokenPos) && _fields[tokenPos].ActualToken)
                    {
                        if (_fields[tokenPos].ActualToken.GetType().ToString() == "Ice")
                        {
                            break;
                        }

                        tokenMove.Token = _fields[tokenPos].ActualToken;
                        tokenMove.startPosition = _fields[tokenPos].transform.position;
                        tokenMove.endPosition = start;
                        tokenMove.Iteration = 1;

                        return tokenMove;
                    }

                    break;
                case 1:
                    tokenPos = start + Vector3.up + Vector3.right;

                    if (_fields.ContainsKey(tokenPos) && _fields[tokenPos].ActualToken)
                    {
                        if (_fields[tokenPos].ActualToken.GetType().ToString() == "Ice")
                        {
                            break;
                        }

                        tokenMove.Token = _fields[tokenPos].ActualToken;
                        tokenMove.startPosition = _fields[tokenPos].transform.position;
                        tokenMove.endPosition = start;
                        tokenMove.Iteration = 1;

                        return tokenMove;
                    }

                    break;
            }
        }

        return tokenMove;
    }

    private List<Token> CheckHen(Vector3 start) // соседей 
    {
        Vector3[] checkPosition = new Vector3[4];
        checkPosition[0] = start + Vector3.up;
        checkPosition[1] = start + Vector3.right;
        checkPosition[2] = start + Vector3.down;
        checkPosition[3] = start + Vector3.left;

        List<Token> tokensToDestroy = new List<Token>();

        for (int i = 0; i < checkPosition.Length; i++)
        {
            if (_fields.ContainsKey(checkPosition[i]) && _fields[checkPosition[i]].ActualToken && (
                    _fields[checkPosition[i]].ActualToken.GetType().ToString() == "Ice" ||
                    _fields[checkPosition[i]].ActualToken.GetType().ToString() == "Rock"))
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