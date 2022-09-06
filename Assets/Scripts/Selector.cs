using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Selector : MonoBehaviour
{
    [SerializeField]private List<Token> _tokens = new List<Token>();

    private GameController _gameController = null;

    private Token _actualType;
    private Color _actualColor;

    private void Start()
    {
        _gameController = GetComponent<GameController>();
        _tokens = _gameController.Tokens;

        LineController.Instance.ActualTokenType += Selected;
    }

    private void Selected(Token typeSelect)
    {
        foreach (var token in _tokens)
        {
            switch (typeSelect)
            {
                case null:
                    if (token.GetType() != _actualType.GetType())
                    {
                        ChangeAlpha(token.gameObject, 1f);
                    }
                    break;
                default:
                    if (token.GetType() != typeSelect.GetType())
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
