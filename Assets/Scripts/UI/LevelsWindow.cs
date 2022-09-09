using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelsWindow : Window
{
    [SerializeField] private Button _closeButton = null;
    
    private Camera _camera = null;
    private Vector3 _screenSize;

    private Vector3 _lvlScreenDefaultPosition;
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

        _lvlScreenDefaultPosition = new Vector2(_screenSize.x / 2, _screenSize.y * 4.5f);
        
        _center = new Vector2(_screenSize.x / 2, _screenSize.y / 2);
        
        transform.position = _lvlScreenDefaultPosition;

        _closeButton.onClick.AddListener(BackToMenu);
    }

    private void BackToMenu()
    {
        UIManager.Open<StartWindow>();
    }

    protected override void SelfOpen()
    {
        transform.DOMove(_center, 1f);
    }

    protected override void SelfClose()
    {
        transform.DOMove(_lvlScreenDefaultPosition, 0.5f);
    }
}
