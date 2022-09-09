using System;
using System.Collections;
using AppodealAds.Unity.Api;
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

    private Coroutine _coroutine = null;

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

        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(WaitEndSteps());
        }
    }

    private IEnumerator WaitEndSteps()
    {
        while (FSM.Status == GameStatus.Wait)
        {
            yield return null;
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

        yield return new WaitForSeconds(1f);

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private void ShowWinPanel()
    {
        _endStatus.text = "Level Complete";
        StartCoroutine(ShowStars(Loading.Instance.CurrentStars));
    }

    private IEnumerator ShowStars(int starCount)
    {
        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < starCount; i++)
        {
            _stars[i].gameObject.SetActive(true);

            _stars[i].transform.DOScale(Vector3.one, 0.3f);
            
           AudioManager.LoadEffect("Star");

            yield return new WaitForSeconds(0.4f);
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
        if(Appodeal.isLoaded(Appodeal.REWARDED_VIDEO)) {
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