using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class DeathScreen : MonoBehaviour
{
    public GameObject deathScreen;

    void Start()
    {
        deathScreen.SetActive(false);
    }

    void Update()
    {
        if (deathScreen.activeSelf && Input.GetKeyDown(KeyCode.R)) 
        {
            ReloadScene();
        }
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
    }

    private void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}