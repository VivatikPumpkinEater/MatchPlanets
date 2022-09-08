using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button[] _restart = new Button[]{};
    [SerializeField] private Button[] _menu = new Button[]{};
    [SerializeField] private Button _resume = null;
    
    [SerializeField] private Button _pause = null;
    [SerializeField] private Button _closePause = null;
    [SerializeField] private GameObject _pauseScreen = null;

    private void Awake()
    {
        foreach (var button in _menu)
        {
            button.onClick.AddListener(OpenMenu);
        }
        
        foreach (var button in _restart)
        {
            button.onClick.AddListener(Restart);
        }
        
        _resume.onClick.AddListener(Resume);
        _pause.onClick.AddListener(Pause);
        _closePause.onClick.AddListener(ClosePause);
    }

    private void OpenMenu()
    {
        Loading.Instance.Load(0);
        
        AudioManager.Instance.SetBGMusic("Menu");
    }

    private void Resume()
    {
        Loading.Instance.LoadNextLvl();
    }

    private void Pause()
    {
        FSM.SetGameStatus(GameStatus.Pause);
        _pauseScreen.SetActive(true);
    }

    private void ClosePause()
    {
        FSM.SetGameStatus(GameStatus.Game);
        _pauseScreen.SetActive(false);
    }

    private void Restart()
    {
        Loading.Instance.RestartLvl();
    }
}
