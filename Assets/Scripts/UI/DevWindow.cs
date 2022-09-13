using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DevWindow : Window
{
    [SerializeField] private GameObject _developPanel;
    [SerializeField] private Button _close;
    
    protected void Awake()
    {
        FullScreen = false;
        
        _close.onClick.AddListener(Close);
    }
    
    protected override void SelfOpen()
    {
        _developPanel.gameObject.SetActive(true);
        _developPanel.transform.localScale = Vector3.zero;
        _developPanel.transform.DOScale(Vector3.one, 0.3f);
    }

    protected override void SelfClose()
    {
        _developPanel.transform.DOScale(Vector3.zero, 0.3f)
            .OnComplete(() => _developPanel.gameObject.SetActive(false));
    }
}
