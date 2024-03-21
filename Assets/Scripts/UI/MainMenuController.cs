using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainCanvas, optionsCanvas;
    [SerializeField] private Slider fxSlider;
    [SerializeField] private Slider musicSlider;

    public AudioSource MusicAudioSource;
    public AudioSource FxAudioSource;
    [SerializeField] private float startDelay = 0f;

    public Animator animator;

    private void Awake()
    {
        //GameManager.Instance.GetComponent<SoundManager>().SetVolumes();
    }

private void Start()
{
    OnBackClicked();
    animator.SetBool("Fade", true);
    
    if(GameManager.Instance != null)
    {
        GameManager.Instance.GetComponent<SoundManager>().SetVolumes();
        GameManager.Instance.GetComponent<SoundManager>().PlayMusic(AudioMusic.menuMusic, MusicAudioSource, true);
    }
    
    else
    {
        Debug.LogError("GameManager.Instance null");
    }
}

    #region MainMenuMethods

    public void OnPlayClicked()
    {
        //animator.SetBool("Fade", false);
        GameManager.Instance.GetComponent<SoundManager>().PlayFx(AudioFx.click, FxAudioSource, false);
        Invoke("LoadScene", startDelay);
    }

    public void LoadScene()
    {
        GameManager.Instance.GetComponent<SoundManager>().PlayMusic(AudioMusic.levelMusic, MusicAudioSource, true);
        GameManager.Instance.GetComponent<SoundManager>().PlayMusic(AudioMusic.ambience, MusicAudioSource, true);
        SceneManager.LoadScene(AppScenes.Level);
        SceneManager.LoadScene("Level");
    }

    public void OnOptionsClicked()
    {
        mainCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
        GameManager.Instance.GetComponent<SoundManager>().PlayFx(AudioFx.click, FxAudioSource, false);
    }

    public void OnExitClicked()
    {
        GameManager.Instance.GetComponent<SoundManager>().PlayFx(AudioFx.click, FxAudioSource, false);
        Debug.Log("Game was exited");
        SoundManager.Instance.SaveVolumes();
        Application.Quit();
    }

    #endregion

    #region OptionsMenuMethods

    public void OnFxVolumeChanged()
    {
        GameManager.Instance.GetComponent<SoundManager>().SetFxVolume(SliderToFaderFloatConvertion(fxSlider.value));
        GameManager.Instance.GetComponent<SoundManager>().PlayFx(AudioFx.click, FxAudioSource, false);
    }

    public void OnMusicVolumeChanged()
    {
        GameManager.Instance.GetComponent<SoundManager>().SetMusicVolume(SliderToFaderFloatConvertion(musicSlider.value));
    }

    private float SliderToFaderFloatConvertion(float initialValue)
    {
        return initialValue * 80 - 80;
    }
    
    public void OnBackClicked()
    {
        GameManager.Instance.GetComponent<SoundManager>().SaveVolumes();
        GameManager.Instance.GetComponent<SoundManager>().PlayFx(AudioFx.click, FxAudioSource, false);
        optionsCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }

    #endregion
    
}
