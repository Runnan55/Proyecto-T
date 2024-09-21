using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultHUD : MonoBehaviour
{
    private static DefaultHUD _instance;
    public static DefaultHUD Instance 
    {
        get { return _instance; }
    }

    protected bool initialized = false;
    public string startSceneName;

    protected void Awake()
    {

        if (_instance != null)
        {
            Debug.LogWarning("DefaultHUD:: Duplicate instance of DefaultHUD, deleting second one (called '"+ this.gameObject.name + "').");
            Destroy(this.gameObject);
        }

        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    protected virtual void Start()
    {
        initialize();
    }
    
    public virtual void initialize()
    {
        if (!initialized)
        {
            initialized = true;
        }
    }

    #region Buttons

    public void StartButton()
    {
        SceneManager.LoadScene(startSceneName);
    }
    
    #endregion Buttons
}

