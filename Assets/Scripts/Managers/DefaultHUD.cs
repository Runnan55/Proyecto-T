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

    public GameObject BTClockImage;
    public GameObject BTPanelImage;

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

    public void EnableBulletTimeUI()
    {
        if (BTClockImage != null)
        {
            BTClockImage.SetActive(true);
        }

        if (BTPanelImage != null)
        {
            BTPanelImage.SetActive(true);
        }
    }

    public void DisableBulletTimeUI()
    {
        if (BTClockImage != null)
        {
            BTClockImage.SetActive(false);
        }

        if (BTPanelImage != null)
        {
            BTPanelImage.SetActive(false);
        }
    }

    #region Buttons

    public void StartButton()
    {
        SceneManager.LoadScene(startSceneName);
    }
    
    #endregion Buttons
}

