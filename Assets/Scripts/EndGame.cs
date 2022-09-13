using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [SerializeField] private TMP_Text _endStatus;

    [SerializeField] private Button _showAds;

    [SerializeField] private Image[] _stars;

    public static EndGame Instance;

    public event System.Action<int> AddedStepsEvent;
    
    public FinishType FinishType;

    private int _gameOver;
    private bool _wait;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }

        Instance = this;
    }

    private void Start()
    {
        AdsManager.RewardVideoFinishedEvent += EndShowRewardVideo;
    }

    public void FinishedLvl(FinishType type)
    {
        FinishType = type;

        if (!_wait)
        {
            _wait = true;
            WaitEndSteps().Forget();
        }
    }

    private async UniTaskVoid WaitEndSteps()
    {
        while (FSM.Status == GameStatus.Wait)
        {
            await UniTask.DelayFrame(0);
        }

        FSM.Status = GameStatus.EndLvl;

        switch (FinishType)
        {
            case FinishType.Win:
                AudioManager.LoadEffect("Win");
                Loading.Instance.UpdateLvlData();
                ShowWinPanel();
                break;
            case FinishType.Lose:
                AudioManager.LoadEffect("Lose");
                ShowLosePanel();
                break;
        }

        UIManager.Open<EndLvlWindow>();

        await UniTask.Delay(1000);

        _wait = false;
    }

    private void ShowWinPanel()
    {
        _endStatus.text = "Level Complete";
        ShowStars(Loading.Instance.CurrentStars).Forget();
    }

    private async UniTaskVoid ShowStars(int starCount)
    {
        await UniTask.Delay(300);

        for (var i = 0; i < starCount; i++)
        {
            var star = _stars[i];
            star.gameObject.SetActive(true);

            star.transform.DOScale(Vector3.one, 0.3f);

            AudioManager.LoadEffect("Star");

            await UniTask.Delay(400);
        }
    }

    private void ShowLosePanel()
    {
        _endStatus.text = "You LOSE";

        if (_gameOver == 0)
        {
            _showAds.gameObject.SetActive(true);
            _showAds.onClick.AddListener(ShowRewardVideo);
        }
    }

    private void EndShowRewardVideo(RewardVideoStatus rewardVideoStatus)
    {
        if (rewardVideoStatus == RewardVideoStatus.Finished)
        {
            ResumeLevel();
        }
    }

    private void ResumeLevel()
    {
        FSM.Status = GameStatus.Game;

        _showAds.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => _showAds.gameObject.SetActive(false));

        _gameOver++;
        AddedStepsEvent?.Invoke(5);
    }

    private void ShowRewardVideo()
    {
        AdsManager.LoadRewardVideo();
    }
}

public enum FinishType
{
    Win,
    Lose
}