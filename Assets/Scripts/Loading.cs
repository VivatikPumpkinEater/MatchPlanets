using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] private Animator _loading = null;

    [SerializeField] private LvlsConstruct _lvlsData = null;

    public static Loading Instance = null;

    public int CurrentStars { get; set; } = 0;

    private static readonly int Start = Animator.StringToHash("Start");
    private static readonly int End = Animator.StringToHash("End");

    private int _currentLvlNumber = 0;
    private LvlData _currentLvlData;

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

        Load(lvlData);
    }

    public void LoadNextLvl()
    {
        if (!FSM.Status.Equals(GameStatus.Loading))
        {
            if (_currentLvlNumber <= _lvlsData.LvlsData.Count - 1)
            {
                var lvlData = _lvlsData.LvlsData[_currentLvlNumber];
                _currentLvlData = lvlData;

                LoadingLvl(lvlData).Forget();

                _currentLvlNumber++;
            }
            else
            {
                LoadingLvl(0).Forget();
            }
        }
    }

    public void RestartLvl()
    {
        Load(_currentLvlData);
    }

    private void Load(LvlData lvlData)
    {
        if (!FSM.Status.Equals(GameStatus.Loading))
        {
            LoadingLvl(lvlData).Forget();
        }
    }

    public void Load(int scene)
    {
        if (!FSM.Status.Equals(GameStatus.Loading))
        {
            LoadingLvl(scene).Forget();
        }
    }

    public void UpdateLvlData()
    {
        Debug.Log("Update lvl data");

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

    private async UniTaskVoid LoadingLvl(LvlData lvlData)
    {
        FSM.SetGameStatus(GameStatus.Loading);

        CurrentStars = 0;

        _loading.SetTrigger(Start);

        AudioManager.LoadEffect("Loading");

        await UniTask.Delay(2000);

        var nextLvl = SceneManager.LoadSceneAsync(1);

        while (!nextLvl.isDone)
        {
            await UniTask.DelayFrame(0);
        }
        
        FSM.SetGameStatus(GameStatus.Loaded);

        AudioManager.LoadBGMusic("Game");

        _loading.SetTrigger(End);

        FindObjectOfType<LoadLvl>().Load(lvlData);
    }

    private async UniTaskVoid LoadingLvl(int scene)
    {
        FSM.SetGameStatus(GameStatus.Loading);

        _loading.SetTrigger(Start);

        AudioManager.LoadEffect("Loading");

        await UniTask.Delay(2000);

        var nextLvl = SceneManager.LoadSceneAsync(scene);

        while (!nextLvl.isDone)
        {
            await UniTask.DelayFrame(0);
        }

        FSM.SetGameStatus(GameStatus.Loaded);

        AudioManager.LoadBGMusic("Menu");

        _loading.SetTrigger(End);
    }
}