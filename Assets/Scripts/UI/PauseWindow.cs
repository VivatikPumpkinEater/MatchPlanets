using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : Window
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _pause;
    [SerializeField] private Button _resume;
    [SerializeField] private Button _home;
    [SerializeField] private Button _restart;
    protected void Awake()
    {
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
        FSM.Status = GameStatus.Pause;
        
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
            FSM.Status = GameStatus.Game;
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
