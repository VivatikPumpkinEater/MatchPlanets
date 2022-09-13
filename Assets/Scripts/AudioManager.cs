using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;

    [SerializeField] private Pool _pool;
    
    [SerializeField] private List<BGmusicClips> _bgClips;
    [SerializeField] private List<EffectClips> _effectClips;
    
    private static AudioManager _instance = null;

    private float _currentMusicVolume = 1;
    private float _currentSoundVolume = 1;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        
        InitVolume();
        SetBGMusic("Menu");
    }
    
    private void InitVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            _currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }

        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            _currentSoundVolume = PlayerPrefs.GetFloat("SoundVolume");
        }
    }

    public static float GetCurrentMusicVolume()
    {
        return _instance._currentMusicVolume;
    }
    
    public static float GetCurrentSoundVolume()
    {
        return _instance._currentSoundVolume;
    }
    
    public static void LoadBGMusic(string placement)
    {
        _instance.SetBGMusic(placement);
    }

    public static void LoadEffect(string name)
    {
        _instance.GetEffect(name);
    }

    public static void ChangeMusicVolume(float volume)
    {
        _instance.VolumeMusic(volume);
    }
    
    public static void ChangeEffectsVolume(float volume)
    {
        _instance.VolumeEffect(volume);
    }
    
    private void VolumeMusic(float volume)
    {
        _currentMusicVolume = volume;
        _musicSource.volume = volume;
    }
    
    private void VolumeEffect(float volume)
    {
        _currentSoundVolume = volume;
    }

    private void SetBGMusic(string placement)
    {
        foreach (var bgClip in _bgClips)
        {
            if (bgClip.Placement.Equals(placement))
            {
                _musicSource.clip = bgClip.AudioClip;
                _musicSource.Play();
                break;
            }
        }
    }
    
    private void GetEffect(string name)
    {
        foreach (var effectClip in _effectClips)
        {
            if (effectClip.Name.Equals(name))
            {
                var poolObject = _pool.GetFreeElement();
                
                var audioSource = poolObject.GetComponent<AudioSource>();
                audioSource.clip = effectClip.AudioClip;
                audioSource.volume = _currentSoundVolume;
                audioSource.pitch = Random.Range(1f, 1.5f);
                audioSource.Play();

                poolObject.ReturnToPool(effectClip.AudioClip.length);
                break;
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("MusicVolume", _currentMusicVolume);
        PlayerPrefs.SetFloat("SoundVolume", _currentSoundVolume);
    }
}
[Serializable]
public struct BGmusicClips
{
    public string Placement;
    public AudioClip AudioClip;
}

[Serializable]
public struct EffectClips
{
    public string Name;
    public AudioClip AudioClip;
}
