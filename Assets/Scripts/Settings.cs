using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Button _musicButton = null;
    [SerializeField] private Button _soundButton = null;

    [SerializeField] private Slider _musicSlider = null;
    [SerializeField] private Slider _soundSlider = null;

    [SerializeField] private Button _vibroButton = null;
    
    [SerializeField] private Button _closeButton = null;

    [SerializeField] private Sprite[] _musicIcons = new Sprite[] { };
    [SerializeField] private Sprite[] _soundIcons = new Sprite[] { };
    
    public System.Action CloseSettingsScreen;

    private AudioManager _audioManager = null;

    private bool _muteMusic = false;
    private bool _muteSound = false;

    private TMP_Text _vibroStatus = null;
    private bool _vibro = true;

    private const string vibroOn = "Vibro On";
    private const string vibroOff = "Vibro Off";
    
    private void Start()
    {
        _audioManager = AudioManager.Instance;

        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(CloseSettings);
        }

        if (!_audioManager) return;

        _musicSlider.onValueChanged.AddListener(ChangeMusicValue);

        _soundSlider.onValueChanged.AddListener(ChangeSoundValue);

        _musicSlider.value = AudioManager.Instance.CurrentMusicVolume;
        _soundSlider.value = AudioManager.Instance.CurrentSoundVolume;
        
        _vibroButton.onClick.AddListener(ChangeVibrationStatus);
        _vibroStatus = _vibroButton.GetComponentInChildren<TMP_Text>();
        UpdateVibroButton();
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

    private void ChangeVibrationStatus()
    {
        VibrationManager.Instance.VibrationAccess();
        
        UpdateVibroButton();
    }

    private void UpdateVibroButton()
    {
        switch (VibrationManager.Instance.Vibration)
        {
            case true:
                _vibroStatus.text = vibroOn;
                break;
            case false:
                _vibroStatus.text = vibroOff;
                break;
        }
    }
}