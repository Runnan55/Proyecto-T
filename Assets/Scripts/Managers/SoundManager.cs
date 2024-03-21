using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;


#region ENUMS //    ***** ENUMS ***** 

public enum AudioFx
{
    breath,
    click,
    death,
    attack,
    shoot,
}

public enum AudioMusic
{
    menuMusic,
    levelMusic,
    ambience,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] public List<AudioClip> m_fxClips;
    [SerializeField] public List<AudioClip> m_musicClips;

    [SerializeField] public AudioSource fxAudioSource;
    [SerializeField] public AudioSource musicAudioSource;

    [SerializeField] public AudioMixer audioMixer;

#endregion ENUMS

    #region PLAYS //    ***** PLAYS ***** 

    public void PlayAudioClip(AudioClip audioClip, AudioSource audioSource)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayAudioSource(AudioSource audioSource)
    {
        audioSource.Play();
    }

    public void PlayFx(AudioFx audioFx, bool isLooping = true)
    {
        fxAudioSource.clip = m_fxClips[(int)audioFx];
        fxAudioSource.Play();
        SetAudioSourceLoop(fxAudioSource, isLooping);
    }
    
    public void PlayFx(AudioFx audioFx, AudioSource fxAudioSource, bool isLooping = true)
    {
        fxAudioSource.clip = m_fxClips[(int)audioFx];
        fxAudioSource.Play();
        SetAudioSourceLoop(fxAudioSource, isLooping);
    }

    public void PlayMusic(AudioMusic audioMusic, bool isLooping = true)
    {
        musicAudioSource.clip = m_musicClips[(int)audioMusic];
        musicAudioSource.Play();
        SetAudioSourceLoop(musicAudioSource, isLooping);
    }

    public void PlayMusic(AudioMusic audioMusic, AudioSource audioSource, bool isLooping = true)
    {
        audioSource.clip = m_musicClips[(int)audioMusic];
        audioSource.Play();
        SetAudioSourceLoop(audioSource, isLooping);
    }

    #endregion PLAYS

    #region AUDIOSOURCES //    ***** AUDIOSOURCES ***** 

    public void SetAudioSourceLoop(AudioSource audioSource, bool isLoop)
    {
        audioSource.loop = isLoop;
    }

    public void StopAudioSource(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void PauseAudioSource(AudioSource audioSource)
    {
        audioSource.Pause();
    }

    public void MuteAudioSource(AudioSource audioSource)
    {
        audioSource.mute = true;
    }

    public void UnMuteAudioSource(AudioSource audioSource)
    {
        audioSource.mute = false;
    }

    public void ToggleMuteAudioSource(AudioSource audioSource)
    {
        audioSource.mute = !audioSource.mute;
    }
    
    #endregion AUDIOSOURCES
    
    #region VOLUMES //    ***** VOLUMES ***** 

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetFxVolume(float volume)
    {
        audioMixer.SetFloat("FxVolume", volume);
    }

    public void SaveVolumes()
    {
        float tempValue;
        if (audioMixer.GetFloat("MusicVolume", out tempValue))
            PlayerPrefs.SetFloat("MusicVolume", tempValue);

        if (audioMixer.GetFloat("FxVolume", out tempValue))
            PlayerPrefs.SetFloat("FxVolume", tempValue);
            
        PlayerPrefs.Save();
    }

    public void SetVolumes()
    {
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        audioMixer.SetFloat("FXVolume", PlayerPrefs.GetFloat("FXVolume"));

        PlayerPrefs.Save();
    }

    #endregion VOLUMES
}

/*
Para reproducir sonidos en otros scripts con el GameManager:

 1. Añadir los sonidos en el inspector del GameObject del SoundManager

 2. Añadir los sonidos al enum al principio del script del SoundManager

 3. Referenciar en el script desde el que se quiere reproducir el sonido

    Ejemplo:
    
    // Audio
    public AudioSource MusicAudioSource; 

    void Start()
    {
        // Music AudioSource
        MusicAudioSource = GameManager.Instance.GetComponent<SoundManager>().musicAudioSource;
    }

    ///

    // Audio
    public AudioSource FxAudioSource; 

    void Start()
    {
        // Fx AudioSource
        FxAudioSource = GameManager.Instance.GetComponent<SoundManager>().fxAudioSource;
    }

 4. Reproducir sonido (musicofx.nombresonido, audiosource, loop)

    Ejemplo:

    GameManager.Instance.GetComponent<SoundManager>().PlayMusic(AudioMusic.MenuMusic, MusicAudioSource, true);

    ///

    GameManager.Instance.GetComponent<SoundManager>().PlayFx(AudioFx.click, FxAudioSource, false);
*/