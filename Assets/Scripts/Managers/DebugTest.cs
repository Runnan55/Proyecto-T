using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugTest : MonoBehaviour
{
    public GameObject DebugMenu;
    public Life life;

    private static DebugTest _instance;
    public static DebugTest Instance 
    {
        get { return _instance; }
    }

    private bool initialized = false;

    void Awake()
    {
        StartCoroutine(DelayedPlayerSearch());
    }

    IEnumerator DelayedPlayerSearch()
    {
        yield return new WaitForSeconds(2.0f);

    GameObject player = GameObject.FindWithTag("Player");
        
        if (player != null)
        {
            life = player.GetComponent<Life>();
        }
        else
        {
            Debug.LogError("PlayerCore(Clone) not found in the scene.");
        }
        
        if (_instance != null)
        {
            //Debug.LogWarning("Debug:: Duplicate instance of Debug, deleting second one.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        initialize();
    }

    void Update()
    {
        OpenDebugMenu();
    }

    public void initialize()
    {
        if (!initialized)
        {
            initialized = true;
        }
    }

    public void OpenDebugMenu()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            DebugMenu.SetActive(!DebugMenu.activeSelf);
        }
    }

    public void OpenDebugLevel()
    {
        SceneManager.LoadScene("DebugLevel");
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenTuto()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void OpenHub()
    {
        SceneManager.LoadScene("Hub");
    }

    public void OpenStrength()
    {
        SceneManager.LoadScene("Strength");
    }

    public void OpenPriestess()
    {
        SceneManager.LoadScene("HighPriestess");
    }

    public void OpenDevil()
    {
        SceneManager.LoadScene("Devil");
    }

    public void RestartActualLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
     
    public void EnableInvencibility()
    {
        if (life != null)
        {
            life.enableInvencibility();
        }
    }

    public void disableInvencibility()
    {
        if (life != null)
        {
            life.disableInvencibility();
        }
    }

    public void fullHealth()
    {
        if (life != null)
        {
            life.enableInvencibility();
        }
    }

    public void halfHealth()
    {
        if (life != null)
        {
            life.halfHealth();
        }
    }
}