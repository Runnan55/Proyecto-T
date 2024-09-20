using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultHUD : MonoBehaviour
{
    public string startSceneName;

    #region Buttons

    public void StartButton()
    {
        SceneManager.LoadScene(startSceneName);
    }
    
    #endregion Buttons
}

