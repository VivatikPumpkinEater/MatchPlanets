using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] private Texture[] _textures = new Texture[] { };
    [SerializeField] private Bonus[] _bonus = new Bonus[] { };

    public static LineController Instance = null;

    public bool InProgress { get; private set; } = false;

    public System.Action<Token[]> TokenToDestroy;
    public System.Action<TokenType> ActualTokenTypeEvent;
    public System.Action EndStep;

    private List<Token> _tokensInChain = new List<Token>();

    private LineRenderer _lineRenderer = null;
    private LineRenderer _line => _lineRenderer = _lineRenderer ? _lineRenderer : GetComponent<LineRenderer>();

    private Dictionary<string, Bonus> _bonusData = new Dictionary<string, Bonus>();

    private TokenType _actualType;

    private int _frameStep = 0;
    private float _fpsCounter = 0;

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
        if (InProgress)
        {
            _fpsCounter += Time.deltaTime;

            if (_fpsCounter >= 1f / 30f)
            {
                _frameStep++;

                if (_frameStep == _textures.Length)
                {
                    _frameStep = 0;
                }

                _line.material.SetTexture("_MainTex", _textures[_frameStep]);

                _fpsCounter = 0f;
            }
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
            if (_line.positionCount == 0)
            {
                InProgress = true;

                _actualType = token.Type;

                ActualTokenTypeEvent?.Invoke(_actualType);
            }

            if (token.Type.Equals(_actualType))
            {
                if (!_tokensInChain.Contains(token))
                {
                    VibrationManager.Instance.GetVibration(VibrationType.Pop);
                    AudioManager.Instance.GetEffect("AddToken");
                    
                    if (_tokensInChain.Count < 1)
                    {
                        _line.SetPosition(_line.positionCount++, position);
                        _tokensInChain.Add(token);

                        Selected(token.transform);
                    }
                    else if (Vector3.Magnitude(token.transform.position - _tokensInChain[^1].transform.position) < 1.5)
                    {
                        _line.SetPosition(_line.positionCount++, position);
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
                else if (_line.positionCount >= 2 &&
                         token.transform.position == _line.GetPosition(_line.positionCount - 2))
                {
                    AudioManager.Instance.GetEffect("MinusToken");
                    
                    UnSelected(_tokensInChain[^1].transform);

                    if (_tokensInChain[^1].Bonus)
                    {
                        Destroy(_tokensInChain[^1].Bonus.gameObject);
                        _tokensInChain[^1].Bonus = null;
                    }

                    _tokensInChain.RemoveAt(_tokensInChain.Count - 1);
                    _line.positionCount -= 1;
                }
            }
        }
    }

    public void ClearLine()
    {
        if (FSM.Status == GameStatus.Game)
        {
            InProgress = false;

            _line.positionCount = 0;

            UnSelected(_tokensInChain);

            if (_tokensInChain.Count >= 3)
            {
                Token[] tokenToDestroy = new Token[_tokensInChain.Count];
                _tokensInChain.CopyTo(tokenToDestroy);

                TokenToDestroy?.Invoke(tokenToDestroy);
                EndStep?.Invoke();
            }

            _tokensInChain.Clear();

            _actualType = TokenType.Null;

            ActualTokenTypeEvent?.Invoke(_actualType);
        }
    }

    private void Selected(Transform select)
    {
        select.DOShakeScale(0.1f).OnComplete
        (
            () => select.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.1f)
        );
    }

    private void UnSelected(Transform unSelect)
    {
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