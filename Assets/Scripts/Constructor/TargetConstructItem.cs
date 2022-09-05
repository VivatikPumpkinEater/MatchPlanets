using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetConstructItem : MonoBehaviour
{
    [SerializeField] private Button _iconButton = null;
    [SerializeField] private TMP_InputField _tokensCountTarget = null;

    public Button IconButton => _iconButton;
    
    public Sprite SpriteToken { get; private set; } = null;
    public string Type { get; private set; } = String.Empty;
    public TMP_InputField TokenCount => _tokensCountTarget;

    public void SetUp(Sprite sprite, string type)
    {
        _iconButton.image.sprite = sprite;
        SpriteToken = sprite;
        Type = type;
    }
}
