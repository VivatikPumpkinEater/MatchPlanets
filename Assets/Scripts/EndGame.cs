using System;
using System.Collections;
using System.Collections.Generic;
using AppodealAds.Unity.Common;
using AppodealAds.Unity.Api;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour , IRewardedVideoAdListener
{
    [SerializeField] private GameObject _endPanel = null;
    [SerializeField] private TMP_Text _endStatus = null;

    [SerializeField] private Button _resume = null;

    [SerializeField] private GameObject _adsPanel = null;
    [SerializeField] private Button _showAds = null;

    [SerializeField] private Image[] _stars = new Image[] { };

    public static EndGame Instance = null;
    
    public System.Action<int> AddSteps;

    private int _gameOver = 0;
    private FinishType _finishType;

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

    public void FinishedLvl(FinishType type)
    {
        _finishType = type;

        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(WaitEndSteps());
        }
    }

    private IEnumerator WaitEndSteps()
    {
        while (FSM.Wait)
        {
            yield return null;
        }
        FSM.SetGameStatus(GameStatus.EndLvl);
        
        _endPanel.SetActive(true);
        _endPanel.transform.localScale = Vector3.zero;

        switch (_finishType)
        {
            case FinishType.Win:
                Loading.Instance.UpdateLvlData();
                ShowWinPanel();
                break;
            case FinishType.Lose:
                ShowLosePanel();
                break;
        }
    }

    private void ShowWinPanel()
    {
        _endPanel.transform.DOScale(Vector3.one, 0.3f);
        
        _endStatus.text = "Level Complete";
        _resume.gameObject.SetActive(true);
        StartCoroutine(ShowStars(Loading.Instance.CurrentStars));
    }

    private IEnumerator ShowStars(int starCount)
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < starCount; i++)
        {
            _stars[i].gameObject.SetActive(true);

            _stars[i].transform.DOScale(Vector3.one, 0.3f);

            yield return new WaitForSeconds(0.4f);
        }
    }

    private void ShowLosePanel()
    {
        _endPanel.transform.DOScale(Vector3.one, 0.3f);

        _endStatus.text = "You LOSE";

        _resume.gameObject.SetActive(false);

        if (_gameOver == 0)
        {
            _adsPanel.SetActive(true);
            _showAds.onClick.AddListener(ShowRewardVideo);
        }
    }

    private void ResumeLevel()
    {
        FSM.SetGameStatus(GameStatus.Game);
        
        _adsPanel.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => _adsPanel.SetActive(false));
        
        _gameOver++;
        AddSteps?.Invoke(5);

        _endPanel.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => _endPanel.SetActive(false));
    }

    private void ShowRewardVideo()
    {
        Appodeal.show(Appodeal.REWARDED_VIDEO);
    }

    public void onRewardedVideoLoaded(bool isPrecache)
    {
        _showAds.GetComponentInChildren<TMP_Text>().text = "SHOW REWARDED VIDEO";
        Debug.Log("onRewardedVideoLoaded");
        Debug.Log($"getPredictedEcpm(): {Appodeal.getPredictedEcpm(Appodeal.REWARDED_VIDEO)}");
    }

    public void onRewardedVideoFailedToLoad()
    {
        Debug.Log("onRewardedVideoFailedToLoad");
    }

    public void onRewardedVideoShowFailed()
    {
        Debug.Log("onRewardedVideoShowFailed");
    }

    public void onRewardedVideoShown()
    {
        Debug.Log("onRewardedVideoShown");
    }

    public void onRewardedVideoClosed(bool finished)
    {
        _showAds.GetComponentInChildren<TMP_Text>().text = "CACHE REWARDED VIDEO";
        Debug.Log($"onRewardedVideoClosed. Finished - {finished}");
    }

    public void onRewardedVideoFinished(double amount, string name)
    {
        Debug.Log("onRewardedVideoFinished. Reward: " + amount + " " + name);
        
        ResumeLevel();
    }

    public void onRewardedVideoExpired()
    {
        Debug.Log("onRewardedVideoExpired");
    }

    public void onRewardedVideoClicked()
    {
        Debug.Log("onRewardedVideoClicked");
    }
}

public enum FinishType
{
    Win,
    Lose
}