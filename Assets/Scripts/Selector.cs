using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private List<Token> _tokens = new List<Token>();

    private GameController _gameController = null;

    private TokenType _actualType;
    private Color _actualColor;

    private void Start()
    {
        _gameController = GetComponent<GameController>();
        _tokens = _gameController.Tokens;

        LineController.Instance.ActualTokenTypeEvent += Selected;
    }

    private void Selected(TokenType typeSelect)
    {
        foreach (var token in _tokens)
        {
            switch (typeSelect)
            {
                case TokenType.Null:
                    if (token.GetType().ToString() != _actualType.ToString())
                    {
                        ChangeAlpha(token.gameObject, 1f);
                    }
                    break;
                default:
                    if (token.GetType().ToString() != typeSelect.ToString())
                    {
                        ChangeAlpha(token.gameObject, 0.5f);
                    }
                    break;
            }
        }

        _actualType = typeSelect;
    }

    private void ChangeAlpha(GameObject sprite,float alphaValue)
    {
        var spriteRenderer = sprite.GetComponent<SpriteRenderer>();

        _actualColor = spriteRenderer.color;
        _actualColor.a = alphaValue;
        spriteRenderer.color = _actualColor;
    }
}
