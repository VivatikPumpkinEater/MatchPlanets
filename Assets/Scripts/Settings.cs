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

    [SerializeField] private Sprite[] _soundsIcon = new Sprite[] { };

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
        _musicButton.onClick.AddListener(MuteMusic);
        _lastMusicValue = _musicSlider.value;

        _soundSlider.onValueChanged.AddListener(ChangeSoundValue);
        _soundButton.onClick.AddListener(MuteSound);
        _lastSoundValue = _soundSlider.value;
    }

    private void CloseSettings()
    {
        CloseSettingsScreen?.Invoke();
        Debug.Log("CLOSE SETTINGS");
    }

    private void ChangeMusicValue(float value)
    {
        _audioManager.VolumeMusic(value);

        if (value != 0f)
        {
            _lastMusicValue = _musicSlider.value;
        }
    }

    private void ChangeSoundValue(float value)
    {
        _audioManager.VolumeEffect(value);

        if (value != 0f)
        {
            _lastSoundValue = _soundSlider.value;
        }
    }

    private void MuteMusic()
    {
        _muteMusic = !_muteMusic;

        _audioManager.MuteMusic(_muteMusic);

        switch (_muteMusic)
        {
            case true:
                _musicSlider.value = 0f;
                break;
            case false:
                _musicSlider.value = _lastMusicValue;
                break;
        }
    }

    private void MuteSound()
    {
        _muteSound = !_muteSound;

        _audioManager.MuteEffect(_muteSound);

        switch (_muteSound)
        {
            case true:
                _soundSlider.value = 0f;
                break;
            case false:
                _soundSlider.value = _lastSoundValue;
                break;
        }
    }
}