using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetConstructItem : MonoBehaviour
{
    [SerializeField] private Button _iconButton;
    [SerializeField] private TMP_InputField _tokensCountTarget;

    public Button IconButton => _iconButton;

    public Sprite SpriteToken { get; private set; }
    public string Type { get; private set; }
    public TMP_InputField TokenCount => _tokensCountTarget;

    public void SetUp(Sprite sprite, string type)
    {
        _iconButton.image.sprite = sprite;
        SpriteToken = sprite;
        Type = type;
    }
}