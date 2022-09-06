using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] private Animator _loading = null;

    [SerializeField] private LvlsConstruct _lvlsData = null;

    public static Loading Instance = null;

    public int CurrentStars { get; set; } = 0;
    
    private int _currentLvlNumber = 0;
    private LvlData _currentLvlData;

    private Coroutine _loadingCoroutine = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Load(LvlData lvlData, int lvlNumber)
    {
        _currentLvlNumber = lvlNumber;
        _currentLvlData = lvlData;

        //_loading.gameObject.transform.position = position;

        Load(lvlData);
    }

    private void Load(LvlData lvlData)
    {
        if (_loading == null)
        {
            _loading = GetComponentInChildren<Animator>();
        }
        _loading.SetTrigger("Start");

        if (_loadingCoroutine != null)
        {
            StopCoroutine(_loadingCoroutine);
            _loadingCoroutine = null;
        }

        _loadingCoroutine = StartCoroutine(LoadingLvl(lvlData));
    }

    public void LoadNextLvl()
    {

        _loading.SetTrigger("Start");

        if (_loadingCoroutine != null)
        {
            StopCoroutine(_loadingCoroutine);
            _loadingCoroutine = null;
        }

        if(_currentLvlNumber <= _lvlsData.LvlsData.Count - 1)
        {
            LvlData lvlData = _lvlsData.LvlsData[_currentLvlNumber];
            _currentLvlData = lvlData;
            
            _loadingCoroutine = StartCoroutine(LoadingLvl(lvlData));

            _currentLvlNumber++;
        }
        else
        {
            _loadingCoroutine = StartCoroutine(LoadingLvl(0));
        }
    }

    public void RestartLvl()
    {
        Load(_currentLvlData);
    }

    public void Load(int scene)
    {
        _loading.SetTrigger("Start");

        if (_loadingCoroutine != null)
        {
            StopCoroutine(_loadingCoroutine);
            _loadingCoroutine = null;
        }

        _loadingCoroutine = StartCoroutine(LoadingLvl(scene));
    }

    public void UpdateLvlData()
    {
        Debug.Log(_lvlsData.LvlsData[_currentLvlNumber - 1].Name);

        var lvlData = _lvlsData.LvlsData[_currentLvlNumber - 1];
        lvlData.LevelPassed = true;

        if (lvlData.Stars < CurrentStars)
        {
            lvlData.Stars = CurrentStars;
        }

        _lvlsData.LvlsData[_currentLvlNumber - 1] = lvlData;

        if (_lvlsData.LvlsData.Count != _currentLvlNumber)
        {
            var data = _lvlsData.LvlsData[_currentLvlNumber];
            data.LevelUnlock = true;

            _lvlsData.LvlsData[_currentLvlNumber] = data;
        }
    }

    private IEnumerator LoadingLvl(LvlData lvlData) //bad
    {
        CurrentStars = 0;
        
        yield return new WaitForSeconds(2f);

        var nextLvl = SceneManager.LoadSceneAsync(1);

        while (!nextLvl.isDone)
        {
            yield return null;
        }

        AudioManager.Instance.SetBGMusic("Game");

        _loading.SetTrigger("End");

        FindObjectOfType<LoadLvl>().Load(lvlData);
    }

    private IEnumerator LoadingLvl(int scene)
    {
        yield return new WaitForSeconds(2f);

        var nextLvl = SceneManager.LoadSceneAsync(scene);

        while (!nextLvl.isDone)
        {
            yield return null;
        }

        _loading.SetTrigger("End");
    }
}