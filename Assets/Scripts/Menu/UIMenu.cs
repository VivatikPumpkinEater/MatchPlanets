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

    [Header("Buttons")] [SerializeField] private Button _play = null;
    [SerializeField] private Button _settings = null;
    [SerializeField] private Button _back = null;

    private Camera _camera = null;
    private Vector3 _screenSize;

    private Vector3 _lvlScreenDefaultPosition;
    private Vector3 _startScreenSecondPosition;
    private Vector3 _center;

    private void Start()
    {
        _camera = Camera.main;
        _screenSize = new Vector3(_camera.pixelWidth, _camera.pixelHeight, 0);

        _lvlScreenDefaultPosition = new Vector2(_screenSize.x / 2, _screenSize.y * 1.5f);

        _startScreenSecondPosition = _lvlScreenDefaultPosition;
        _startScreenSecondPosition.y *= -1;

        _center = _startScreen.position;

        _lvlScreen.position = _lvlScreenDefaultPosition;

        _play.onClick.AddListener(ShowLvls);
        _back.onClick.AddListener(ShowStartScreen);
        _settings.onClick.AddListener(ShowSettings);
    }

    private void ShowLvls()
    {
        _startScreen.transform.DOMove(_startScreenSecondPosition, 0.5f);
        _lvlScreen.transform.DOMove(_center, 0.5f);
    }

    private void ShowStartScreen()
    {
        _startScreen.transform.DOMove(_center, 0.5f);
        _lvlScreen.transform.DOMove(_lvlScreenDefaultPosition, 0.5f);
    }

    private void ShowSettings()
    {
        _settingsScreen.gameObject.SetActive(true);
        _settingsScreen.transform.localScale = Vector3.zero;
        _settingsScreen.transform.DOScale(Vector3.one, 0.3f);

        _settingsScreen.CloseSettingsScreen += CloseSettings;
    }

    private void CloseSettings()
    {
        _settingsScreen.CloseSettingsScreen -= CloseSettings;

        _settingsScreen.transform.DOScale(Vector3.zero, 0.3f)
            .OnComplete(() => _settingsScreen.gameObject.SetActive(false));
    }
}