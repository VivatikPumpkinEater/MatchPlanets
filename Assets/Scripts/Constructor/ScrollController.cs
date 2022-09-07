using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    [SerializeField] private ScrollItem[] _scrollItems = new ScrollItem[] { };

    public System.Action<Token, Image, GameObject> SelectedToken;
    public System.Action<Image> CurrentIcon;

    private void Start()
    {
        foreach (var scrollItem in _scrollItems)
        {
            scrollItem.Button.onClick.AddListener
            (
                () => SelectedToken?.Invoke(scrollItem.TokenPrefab, scrollItem.Button.image, scrollItem.SpawnPoint)
            );
        }
    }
}

[Serializable]
public struct ScrollItem
{
    public Button Button;
    public Token TokenPrefab;
    public GameObject SpawnPoint;
}