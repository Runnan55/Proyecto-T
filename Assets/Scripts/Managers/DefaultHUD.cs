using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultHUD : MonoBehaviour
{
    public string startSceneName;

    public void StartButton()
    {
        SceneManager.LoadScene(startSceneName);
    }
}
