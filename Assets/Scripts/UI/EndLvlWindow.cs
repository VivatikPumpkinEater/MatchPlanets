using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndLvlWindow : Window
{
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private Button _resume;
    [SerializeField] private Button _restart;
    [SerializeField] private Button _home;
    
    protected void Awake()
    {
        FullScreen = false;
        
        _resume.onClick.AddListener(Resume);
        _restart.onClick.AddListener(Restart);
        _home.onClick.AddListener(Home);
    }

    protected override void Start()
    {
        base.Start();
        AdsManager.RewardVideoFinishedEvent += ResumeButtonStatus;
    }

    protected override void SelfOpen()
    {
        _endPanel.SetActive(true);
        _endPanel.transform.localScale = Vector3.zero;
        _endPanel.transform.DOScale(Vector3.one, 0.3f);
        
        if (EndGame.Instance.FinishType.Equals(FinishType.Win))
        {
            _resume.gameObject.SetActive(true);
        }
        else
        {
            _resume.gameObject.SetActive(false);
        }
    }

    protected override void SelfClose()
    {
        _endPanel.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => _endPanel.SetActive(false));
    }

    private void Resume()
    {
        switch (FSM.Status)
        {
            case GameStatus.EndLvl:
                Loading.Instance.LoadNextLvl();
                break;
            default:
                Close();
                break;
        }
    }

    private void Restart()
    {
        Loading.Instance.RestartLvl();
    }
    
    private void Home()
    {
        Loading.Instance.Load(0);
        
        //AudioManager.Instance.SetBGMusic("Menu");
    }

    private void ResumeButtonStatus(RewardVideoStatus rewardVideoStatus)
    {
        switch (rewardVideoStatus)
        {
            case RewardVideoStatus.Finished:
                _resume.gameObject.SetActive(true);
                break;
            case RewardVideoStatus.Cancel:
                _resume.gameObject.SetActive(false);
                break;
        }
    }
}
