using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : Window
{
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private Button _close;
    protected void Awake()
    {
        FullScreen = false;
        
        _close.onClick.AddListener(Close);
    }

    protected override void SelfOpen()
    {
        _settingsPanel.gameObject.SetActive(true);
        _settingsPanel.transform.localScale = Vector3.zero;
        _settingsPanel.transform.DOScale(Vector3.one, 0.3f);
    }

    protected override void SelfClose()
    {
        _settingsPanel.transform.DOScale(Vector3.zero, 0.3f)
            .OnComplete(() => _settingsPanel.gameObject.SetActive(false));
    }
}
