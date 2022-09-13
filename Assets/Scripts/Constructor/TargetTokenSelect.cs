using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetTokenSelect : MonoBehaviour
{
    [SerializeField] private List<TargetToken> _targetTokens;

    public TargetConstructItem TargetConstructItem { get; set; }

    private void Awake()
    {
        foreach (var targetToken in _targetTokens)
        {
            targetToken.TokenButton.onClick.AddListener(()=> SetUpTarget(targetToken.SpriteToken.sprite, targetToken.Type));
        }
    }

    private void SetUpTarget(Sprite sprite, string type)
    {
        TargetConstructItem.SetUp(sprite, type);
        
        gameObject.SetActive(false);
    }
}

[Serializable]
public struct TargetToken
{
    public Button TokenButton;
    public Image SpriteToken;
    public string Type;
}
