using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] private Texture[] _textures;
    [SerializeField] private Bonus[] _bonus;

    public static LineController Instance;
    
    public event System.Action<Token[]> DestroyedTokensEvent;
    public event System.Action<TokenType> ActualTokenTypeEvent;
    public event System.Action TurnCompletedEvent;

    public bool InProgress { get; private set; }
    
    private List<Token> _tokensInChain = new List<Token>();

    private LineRenderer _lineRenderer;
    private LineRenderer Line => _lineRenderer = _lineRenderer ? _lineRenderer : GetComponent<LineRenderer>();

    private Dictionary<string, Bonus> _bonusData = new Dictionary<string, Bonus>();

    private TokenType _actualType;

    private int _frameStep;
    private float _fpsCounter;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }

        Instance = this;

        FillBonus();
    }

    private void Update()
    {
        if (!InProgress) return;
        _fpsCounter += Time.deltaTime;

        if (_fpsCounter >= 1f / 30f)
        {
            _frameStep++;

            if (_frameStep == _textures.Length)
            {
                _frameStep = 0;
            }

            Line.material.SetTexture(MainTex, _textures[_frameStep]);

            _fpsCounter = 0f;
        }
    }

    private void FillBonus()
    {
        foreach (var bonus in _bonus)
        {
            if (!_bonusData.ContainsKey(bonus.GetType().ToString()))
            {
                _bonusData.Add(bonus.GetType().ToString(), bonus);
            }
        }
    }

    public void AddPosition(Vector3 position, Token token)
    {
        if (FSM.Status == GameStatus.Game)
        {
            if (Line.positionCount == 0)
            {
                InProgress = true;

                _actualType = token.Type;

                ActualTokenTypeEvent?.Invoke(_actualType);
            }

            if (token.Type.Equals(_actualType))
            {
                if (!_tokensInChain.Contains(token))
                {
                    if (_tokensInChain.Count < 1)
                    {
                        Line.SetPosition(Line.positionCount++, position);
                        _tokensInChain.Add(token);

                        Selected(token.transform);
                    }
                    else if (Vector3.Magnitude(token.transform.position - _tokensInChain[^1].transform.position) < 1.5)
                    {
                        Line.SetPosition(Line.positionCount++, position);
                        _tokensInChain.Add(token);

                        Selected(token.transform);

                        if (_tokensInChain.Count % 5 == 0)
                        {
                            token.Bonus = Instantiate(_bonusData["Rocket"], token.transform);
                        }

                        if (_tokensInChain.Count % 8 == 0)
                        {
                            token.Bonus = Instantiate(_bonusData["Bomb"], token.transform);
                        }
                    }
                }
                else if (Line.positionCount >= 2 &&
                         token.transform.position == Line.GetPosition(Line.positionCount - 2))
                {
                    UnSelected(_tokensInChain[^1].transform);

                    if (_tokensInChain[^1].Bonus)
                    {
                        Destroy(_tokensInChain[^1].Bonus.gameObject);
                        _tokensInChain[^1].Bonus = null;
                    }

                    _tokensInChain.RemoveAt(_tokensInChain.Count - 1);
                    Line.positionCount -= 1;
                }
            }
        }
    }

    public void ClearLine()
    {
        if (FSM.Status == GameStatus.Game)
        {
            InProgress = false;

            Line.positionCount = 0;

            UnSelected(_tokensInChain);

            if (_tokensInChain.Count >= 3)
            {
                var tokenToDestroy = new Token[_tokensInChain.Count];
                _tokensInChain.CopyTo(tokenToDestroy);

                DestroyedTokensEvent?.Invoke(tokenToDestroy);
                TurnCompletedEvent?.Invoke();
            }

            _tokensInChain.Clear();

            _actualType = TokenType.Null;

            ActualTokenTypeEvent?.Invoke(_actualType);
        }
    }

    private void Selected(Transform select)
    {
        VibrationManager.GetVibration(VibrationType.Pop);
        AudioManager.LoadEffect("AddToken");

        select.DOShakeScale(0.1f).OnComplete
        (
            () => select.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.1f)
        );
    }

    private void UnSelected(Transform unSelect)
    {
        AudioManager.LoadEffect("MinusToken");
        unSelect.DOScale(Vector3.one, 0.2f);
    }

    private void UnSelected(List<Token> tokens)
    {
        foreach (var token in tokens)
        {
            UnSelected(token.transform);
        }
    }
}