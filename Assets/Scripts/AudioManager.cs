using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource = null;
    [SerializeField] private AudioSource _effectSource = null;

    [SerializeField] private List<BGmusicClips> _bgClips = new List<BGmusicClips>();
    public static AudioManager Instance = null;

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
        _musicSource.volume = volume;
    }
    
    public void VolumeEffect(float volume)
    {
        _effectSource.volume = volume;
    }

    public void SetBGMusic(string placement)
    {
        foreach (var bgClip in _bgClips)
        {
            if (bgClip.Placement == placement)
            {
                _musicSource.clip = bgClip.AudioClip;
                _musicSource.Play();
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
