using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip buttonHover;
    [SerializeField] private AudioClip buttonPressed;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic(musicClip,0.3f);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        
        sfxSource.PlayOneShot(clip, 0.5f);
    }

    public bool IsSFXplaying()
    {
        return sfxSource.isPlaying;
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.priority = 0;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void ButtonHover()
    {
        PlaySFX(buttonHover);
    }
    public void ButtonPressed()
    {
        PlaySFX(buttonPressed);
    }
}
