using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Button _musicButton = null;
    [SerializeField] private Button _soundButton = null;

    [SerializeField] private Slider _musicSlider = null;
    [SerializeField] private Slider _soundSlider = null;

    [SerializeField] private Button _closeButton = null;

    [SerializeField] private Sprite[] _musicIcons = new Sprite[] { };
    [SerializeField] private Sprite[] _soundIcons = new Sprite[] { };
    
    public System.Action CloseSettingsScreen;

    private AudioManager _audioManager = null;

    private bool _muteMusic = false;
    private bool _muteSound = false;

    private float _lastMusicValue = 0f;
    private float _lastSoundValue = 0f;

    private void Start()
    {
        _audioManager = AudioManager.Instance;

        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(CloseSettings);
        }

        if (!_audioManager) return;

        _musicSlider.onValueChanged.AddListener(ChangeMusicValue);
        _lastMusicValue = _musicSlider.value;

        _soundSlider.onValueChanged.AddListener(ChangeSoundValue);

        _lastSoundValue = _soundSlider.value;

        _musicSlider.value = AudioManager.Instance.CurrentMusicVolume;
        _soundSlider.value = AudioManager.Instance.CurrentSoundVolume;
    }

    private void CloseSettings()
    {
        CloseSettingsScreen?.Invoke();
    }

    private void ChangeMusicValue(float value)
    {
        _audioManager.VolumeMusic(value);

        if (value == 0f)
        {
            _muteMusic = true;
            _musicButton.image.sprite = _musicIcons[0];
        }

        if (value > 0f && _muteMusic)
        {
            _muteMusic = false;
            _musicButton.image.sprite = _musicIcons[1];
        }
    }

    private void ChangeSoundValue(float value)
    {
        _audioManager.VolumeEffect(value);

        if (value == 0f)
        {
            _muteSound = true;
            _soundButton.image.sprite = _soundIcons[0];
        }

        if (value > 0f && _muteSound)
        {
            _muteSound = false;
            _soundButton.image.sprite = _soundIcons[1];
        }
    }
}