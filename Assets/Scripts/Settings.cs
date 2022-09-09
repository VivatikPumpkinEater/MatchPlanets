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

    [SerializeField] private Sprite[] _musicIcons;
    [SerializeField] private Sprite[] _soundIcons;
    
    private bool _muteMusic = false;
    private bool _muteSound = false;

    private TMP_Text _vibroStatus = null;
    private bool _vibro = true;

    private const string vibroOn = "Vibro On";
    private const string vibroOff = "Vibro Off";
    
    private void Start()
    {
        _musicSlider.onValueChanged.AddListener(ChangeMusicValue);

        _soundSlider.onValueChanged.AddListener(ChangeSoundValue);

        _musicSlider.value = AudioManager.GetCurrentMusicVolume();
        _soundSlider.value = AudioManager.GetCurrentSoundVolume();
        
        _vibroButton.onClick.AddListener(ChangeVibrationStatus);
        _vibroStatus = _vibroButton.GetComponentInChildren<TMP_Text>();
        
        UpdateVibroButton();
    }

    private void ChangeMusicValue(float value)
    {
        AudioManager.ChangeMusicVolume(value);

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
        AudioManager.ChangeEffectsVolume(value);

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
        VibrationManager.ChangeVibrationStatus();
        
        UpdateVibroButton();
    }

    private void UpdateVibroButton()
    {
        switch (VibrationManager.GetVibrationStatus())
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