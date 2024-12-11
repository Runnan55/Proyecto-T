using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugTest : MonoBehaviour
{
    public GameObject DebugMenu;
    public GameObject player;
    public Life life;
    public MovimientoJugador movement;

    //
    public string healthString;
    public string speedString;

    private static DebugTest _instance;
    public static DebugTest Instance 
    {
        get { return _instance; }
    }

    private bool initialized = false;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedPlayerSearch());
    }

    IEnumerator DelayedPlayerSearch()
    {
        yield return new WaitForSeconds(1f);

        player = GameObject.FindWithTag("Player");
        
        if (player != null)
        {
            life = player.GetComponent<Life>();
            movement = player.GetComponent<MovimientoJugador>();
        }

        else
        {
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                //Debug.LogWarning("Player not found in the scene.");
            }
        }
    }

    void Start()
    {
        initialize();
    }

    void Update()
    {
        OpenDebugMenu();

        if (Input.GetKeyDown(KeyCode.F3))
        {
            SceneManager.LoadScene("DebugLevel");
        }

        //healthString = ("") life != null ? life.currentHealth.ToString() : "No player found";
        //speedString = movement != null ? movement.speed.ToString() : "No player found";
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
        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.P))
        {
            DebugMenu.SetActive(!DebugMenu.activeSelf);
        }
    }

    #region Levels

    public void OpenDebugLevel()
    {
        SceneManager.LoadScene("DebugLevel");
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenBTBoss()
    {
        SceneManager.LoadScene("BTBoss");
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

     public void OpenGimmick()
    {
        SceneManager.LoadScene("Gimmick");
    }

    public void RestartActualLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    #endregion Levels

    #region Player

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
            life.fullHealth();
        }
    }

    public void halfHealth()
    {
        if (life != null)
        {
            life.halfHealth();
        }
    }

    public void plus30Secs()
    {
        if (life != null)
        {
            life.plus30Secs();
        }
    }

    public void minus30Secs()
    {
        if (life != null)
        {
            life.minus30Secs();
        }
    }

    public void speedUp()
    {
        if (movement != null)
        {
            movement.speedUp();
        }
    }

    public void speedDown()
    {
        if (movement != null)
        {
            movement.speedDown();
        }
    }

    #endregion Player
}