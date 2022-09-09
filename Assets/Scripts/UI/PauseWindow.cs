
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : Window
{
    [SerializeField] private GameObject _pausePanel = null;
    [SerializeField] private Button _pause = null;
    [SerializeField] private Button _resume = null;
    [SerializeField] private Button _home = null;
    [SerializeField] private Button _restart = null;
    protected override void Awake()
    {
        base.Awake();
        FullScreen = false;
        
        _resume.onClick.AddListener(Close);
        _pause.onClick.AddListener(OpenPause);
        _restart.onClick.AddListener(Restart);
        _home.onClick.AddListener(Home);
    }

    private void OpenPause()
    {
        UIManager.Open<PauseWindow>();
    }

    protected override void SelfOpen()
    {
        FSM.SetGameStatus(GameStatus.Pause);
        
        _pausePanel.gameObject.SetActive(true);
        _pausePanel.transform.localScale = Vector3.zero;
        _pausePanel.transform.DOScale(Vector3.one, 0.3f);
    }

    protected override void SelfClose()
    {
        _pausePanel.transform.DOScale(Vector3.zero, 0.3f)
            .OnComplete(() => _pausePanel.gameObject.SetActive(false));

        if (FSM.Status.Equals(GameStatus.Pause))
        {
            FSM.SetGameStatus(GameStatus.Game);
        }
    }
    
    private void Restart()
    {
        Loading.Instance.RestartLvl();
    }
    
    private void Home()
    {
        Loading.Instance.Load(0);
    }
}
