using System;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    [SerializeField] private ScrollItem[] _scrollItems;

    public System.Action<Token, Image, GameObject> TokensSelectedEvent;

    private void Start()
    {
        foreach (var scrollItem in _scrollItems)
        {
            scrollItem.Button.onClick.AddListener
            (
                () => TokensSelectedEvent?.Invoke(scrollItem.TokenPrefab, scrollItem.Button.image, scrollItem.SpawnPoint)
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