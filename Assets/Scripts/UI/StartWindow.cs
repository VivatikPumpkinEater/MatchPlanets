using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : Window
{
    [SerializeField] private Button _start = null;
    [SerializeField] private Button _settings = null;
    [SerializeField] private Button _devPanel = null;

    private Camera _camera = null;
    private Vector3 _screenSize;
    
    private Vector3 _startScreenSecondPosition;
    private Vector3 _center;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _camera = Camera.main;
        _screenSize = new Vector3(_camera.pixelWidth, _camera.pixelHeight, 0);
        
        var lvlScreenDefaultPosition = new Vector2(_screenSize.x / 2, _screenSize.y * 4.5f);
        
        _startScreenSecondPosition = lvlScreenDefaultPosition;
        _startScreenSecondPosition.y *= -1;

        _center = new Vector2(_screenSize.x / 2, _screenSize.y / 2);

        transform.position = _center;

        _start.onClick.AddListener(OpenLvls);
        _settings.onClick.AddListener(OpenSettings);
        _devPanel.onClick.AddListener(OpenDevPanel);

        UIManager.CurrentWindow = this;
    }

    private void OpenLvls()
    {
        UIManager.Open<LevelsWindow>();
    }

    private void OpenSettings()
    {
        UIManager.Open<SettingsWindow>();
    }
    
    private void OpenDevPanel()
    {
        UIManager.Open<DevWindow>();
    }

    protected override void SelfOpen()
    {
        transform.DOMove(_center, 0.5f);
    }

    protected override void SelfClose()
    { 
        transform.DOMove(_startScreenSecondPosition, 0.5f);
    }
}
