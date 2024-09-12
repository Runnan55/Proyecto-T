using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }

            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }
}
