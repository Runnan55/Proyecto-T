using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // No tiene singleton. Básicamente el script que controla todo del menú principal.
    // Interactúa con el EventSystem y con el SaveLoad

    public string startSceneName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitGame();
        }
    }

    public void exitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

        #else
            Application.Quit();

        #endif
    }

    #region Buttons

    public void StartButton()
    {
        SceneManager.LoadScene(startSceneName);
    }
    
    #endregion Buttons
}
