using AppodealAds.Unity.Api;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [SerializeField] private TMP_Text _endStatus = null;

    [SerializeField] private Button _showAds = null;

    [SerializeField] private Image[] _stars = new Image[] { };

    public static EndGame Instance = null;

    public System.Action<int> AddSteps;

    private int _gameOver = 0;
    public FinishType FinishType;

    private bool _wait = false;

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
        AdsManager.RewardVideoEndEvent += EndShowRewardVideo;
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

        FSM.SetGameStatus(GameStatus.EndLvl);

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

        for (int i = 0; i < starCount; i++)
        {
            _stars[i].gameObject.SetActive(true);

            _stars[i].transform.DOScale(Vector3.one, 0.3f);

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
        switch (rewardVideoStatus)
        {
            case RewardVideoStatus.Finished:
                ResumeLevel();
                break;
        }
    }

    private void ResumeLevel()
    {
        FSM.SetGameStatus(GameStatus.Game);

        _showAds.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => _showAds.gameObject.SetActive(false));

        _gameOver++;
        AddSteps?.Invoke(5);
    }

    private void ShowRewardVideo()
    {
        if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
        {
            Appodeal.show(Appodeal.REWARDED_VIDEO);
        }
        else
        {
            _showAds.interactable = false;
        }
    }
}

public enum FinishType
{
    Win,
    Lose
}