using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource = null;
    [SerializeField] private AudioSource _effectSource = null;

    [SerializeField] private Pool _pool = null;
    
    [SerializeField] private List<BGmusicClips> _bgClips = new List<BGmusicClips>();
    [SerializeField] private List<EffectClips> _effectClips = new List<EffectClips>();
    public static AudioManager Instance = null;

    private float _currentMusicVolume = 0;
    private float _currentSoundVolume = 1;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        SetBGMusic("Menu");
    }

    public void MuteMusic(bool mute)
    {
        _musicSource.mute = mute;
    }

    public void MuteEffect(bool mute)
    {
        _effectSource.mute = mute;
    }

    public void VolumeMusic(float volume)
    {
        _currentMusicVolume = volume;
        _musicSource.volume = volume;
    }
    
    public void VolumeEffect(float volume)
    {
        _currentSoundVolume = volume;
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
                audioSource.volume = _currentSoundVolume;
                audioSource.pitch = Random.Range(1f, 1.5f);
                audioSource.Play();

                poolObject.ReturnToPool(effectClip.AudioClip.length);
                break;
            }
        }
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
