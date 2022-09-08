using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource = null;

    [SerializeField] private Pool _pool = null;
    
    [SerializeField] private List<BGmusicClips> _bgClips = new List<BGmusicClips>();
    [SerializeField] private List<EffectClips> _effectClips = new List<EffectClips>();
    public static AudioManager Instance = null;

    public float CurrentMusicVolume{get; private set;} = 1;
    public float CurrentSoundVolume{get; private set;} = 1;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        InitVolume();
        SetBGMusic("Menu");
    }
    
    private void InitVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            CurrentMusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }

        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            CurrentSoundVolume = PlayerPrefs.GetFloat("SoundVolume");
        }
    }
    
    public void VolumeMusic(float volume)
    {
        CurrentMusicVolume = volume;
        _musicSource.volume = volume;
    }
    
    public void VolumeEffect(float volume)
    {
        CurrentSoundVolume = volume;
    }

    public void SetBGMusic(string placement)
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

    public void GetEffect(string name)
    {
        foreach (var effectClip in _effectClips)
        {
            if (effectClip.Name.Equals(name))
            {
                var poolObject = _pool.GetFreeElement();
                
                var audioSource = poolObject.GetComponent<AudioSource>();
                audioSource.clip = effectClip.AudioClip;
                audioSource.volume = CurrentSoundVolume;
                audioSource.pitch = Random.Range(1f, 1.5f);
                audioSource.Play();

                poolObject.ReturnToPool(effectClip.AudioClip.length);
                break;
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("MusicVolume", CurrentMusicVolume);
        PlayerPrefs.SetFloat("SoundVolume", CurrentSoundVolume);
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
