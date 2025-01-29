using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadManager : MonoBehaviour
{
    public TMP_Text loadingText;
    public Slider progressBar;

    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        // lee desde playerprefs la escena a cargar
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad");

        // verifica null
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("SceneToLoad no está configurado en PlayerPrefs.");
            yield break;
        }

        // inicia carga
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncOperation.allowSceneActivation = false;

        float minimumLoadTime = 0.5f; // tiempo minimo carga
        float elapsedTime = 0f;

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            if (progressBar != null)
            {
                progressBar.value = progress;
            }
            if (loadingText != null)
            {
                loadingText.text = "Loading... " + (progress * 100).ToString("F0") + "%";
            }

            elapsedTime += Time.deltaTime;

            // activar escena
            if (asyncOperation.progress >= 0.9f && elapsedTime >= minimumLoadTime)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}