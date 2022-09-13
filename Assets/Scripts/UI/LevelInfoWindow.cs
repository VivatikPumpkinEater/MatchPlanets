using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoWindow : Window
{
    [SerializeField] private GameObject _levelInfo;
    [SerializeField] private Button _close;

    protected void Awake()
    {
        FullScreen = false;

        _close.onClick.AddListener(Close);
    }

    protected override void SelfOpen()
    {
        _levelInfo.gameObject.SetActive(true);
        _levelInfo.transform.localScale = Vector3.zero;
        _levelInfo.transform.DOScale(Vector3.one, 0.3f);
    }

    protected override void SelfClose()
    {
        _levelInfo.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => _levelInfo.gameObject.SetActive(false));
    }
}