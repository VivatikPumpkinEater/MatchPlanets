using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [Header("Screens")] [SerializeField] private Transform _startScreen = null;
    [SerializeField] private Transform _lvlScreen = null;
    [SerializeField] private Settings _settingsScreen = null;
    [SerializeField] private GameObject _developPanel = null;

    [Header("Buttons")] [SerializeField] private Button _play = null;
    [SerializeField] private Button _settings = null;
    [SerializeField] private Button _devPanel = null;
    [SerializeField] private Button _closeDevPanel = null;
    [SerializeField] private Button _back = null;

    [SerializeField] private GyroscopeController[] _gyroscopeControllers = new GyroscopeController[] { };

    private Camera _camera = null;
    private Vector3 _screenSize;

    private Vector3 _lvlScreenDefaultPosition;
    private Vector3 _startScreenSecondPosition;
    private Vector3 _center;

    private void Start()
    {
        _camera = Camera.main;
        _screenSize = new Vector3(_camera.pixelWidth, _camera.pixelHeight, 0);

        _lvlScreenDefaultPosition = new Vector2(_screenSize.x / 2, _screenSize.y * 4.5f);

        _startScreenSecondPosition = _lvlScreenDefaultPosition;
        _startScreenSecondPosition.y *= -1;

        _center = _startScreen.position;

        _lvlScreen.position = _lvlScreenDefaultPosition;

        _play.onClick.AddListener(ShowLvls);
        _back.onClick.AddListener(ShowStartScreen);
        _settings.onClick.AddListener(ShowSettings);
        _devPanel.onClick.AddListener(ShowDevelopPanel);
        _closeDevPanel.onClick.AddListener(CloseDevelopPanel);
    }

    private void ShowLvls()
    {
        foreach (var gyroscopeController in _gyroscopeControllers)
        {
            gyroscopeController.Speed = -250;
            gyroscopeController.Moving = true;
        }
        
        _startScreen.transform.DOMove(_startScreenSecondPosition, 0.5f);
        _lvlScreen.transform.DOMove(_center, 1f).OnComplete(() =>
        {
            foreach (var gyroscopeController in _gyroscopeControllers)
            {
                gyroscopeController.Moving = false;
            }
        });
    }

    private void ShowStartScreen()
    {
        foreach (var gyroscopeController in _gyroscopeControllers)
        {
            gyroscopeController.Speed = 250;
            gyroscopeController.Moving = true;
        }
        
        _startScreen.transform.DOMove(_center, 0.5f);
        _lvlScreen.transform.DOMove(_lvlScreenDefaultPosition, 0.5f).OnComplete(() =>
        {
            foreach (var gyroscopeController in _gyroscopeControllers)
            {
                gyroscopeController.Moving = false;
            }
        });
    }

    private void ShowSettings()
    {
        _settingsScreen.gameObject.SetActive(true);
        _settingsScreen.transform.localScale = Vector3.zero;
        _settingsScreen.transform.DOScale(Vector3.one, 0.3f);

        _settingsScreen.CloseSettingsScreen += CloseSettings;
    }
    
    private void ShowDevelopPanel()
    {
        _developPanel.gameObject.SetActive(true);
        _developPanel.transform.localScale = Vector3.zero;
        _developPanel.transform.DOScale(Vector3.one, 0.3f);
    }

    private void CloseSettings()
    {
        _settingsScreen.CloseSettingsScreen -= CloseSettings;

        _settingsScreen.transform.DOScale(Vector3.zero, 0.3f)
            .OnComplete(() => _settingsScreen.gameObject.SetActive(false));
    }
    
    private void CloseDevelopPanel()
    {
        _developPanel.transform.DOScale(Vector3.zero, 0.3f)
            .OnComplete(() => _developPanel.gameObject.SetActive(false));
    }
}