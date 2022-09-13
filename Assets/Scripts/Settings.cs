using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Button _musicButton;
    [SerializeField] private Button _soundButton;

    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;

    [SerializeField] private Button _vibroButton;

    [SerializeField] private Sprite[] _musicIcons;
    [SerializeField] private Sprite[] _soundIcons;
    
    private bool _muteMusic;
    private bool _muteSound;

    private TMP_Text _vibroStatus;

    private const string VibroOn = "Vibro On";
    private const string VibroOff = "Vibro Off";
    
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
                _vibroStatus.text = VibroOn;
                break;
            case false:
                _vibroStatus.text = VibroOff;
                break;
        }
    }
}