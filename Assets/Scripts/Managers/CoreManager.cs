using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreManager : MonoBehaviour
{
    private static CoreManager _instance;
    public static CoreManager Instance 
    {
        get { return _instance; }
    }

    [SerializeField, Tooltip("")]
    private GameObject debugManagerPrefab = null;

    [SerializeField, Tooltip("")]
    private GameObject eventSystemPrefab = null;

    [SerializeField, Tooltip("")]
    private GameObject interactionManagerPrefab = null;

    [SerializeField, Tooltip("")]
    private GameObject HUDPrefab = null;

    [SerializeField, Tooltip("")]
    private GameObject playerPrefab = null;

    [SerializeField, Tooltip("")]
    private GameObject inputManagerPrefab = null;

    [SerializeField, Tooltip("")]
    private GameObject saveLoadManagerPrefab = null;

    [SerializeField, Tooltip("")]
    private GameObject menuPrefab = null;

    private bool initialized = false;
    
    void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("Core:: Duplicate CoreManager, deleting duplicate instance. If you saw this message when loading a saved game into your 1st scene, ignore. Otherwise, it's still not a problem, but might indicate redundant FPECore in a secondary Scene file or something similar.");
            Destroy(this.gameObject);
        }

        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            if (!initialized)
            {
                initialized = true;
                initialize();
            }
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void initialize()
    {
        if (!HUDPrefab || !eventSystemPrefab || !interactionManagerPrefab || !playerPrefab || !inputManagerPrefab || !saveLoadManagerPrefab || !menuPrefab || !debugManagerPrefab)
        {
            Debug.LogError("Core:: Missing prefab for core component. Game will not function correctly. See Inspector for object '" + gameObject.name + "' to ensure all fields are populated correctly.");
            return;
        }

        Instantiate(eventSystemPrefab, null);
        Instantiate(HUDPrefab, null);
        Instantiate(interactionManagerPrefab, null);
        Instantiate(inputManagerPrefab, null);
        Instantiate(saveLoadManagerPrefab, null);
        Instantiate(menuPrefab, null);
        Instantiate(debugManagerPrefab, null);

        DefaultHUD.Instance.initialize();
        InteractionManager.Instance.initialize();

        // Lastly, load game options
        //SaveLoadManager.Instance.LoadGameOptions();

        // Helper checks //
        if(GameObject.FindObjectOfType<MainMenu>() == null && SceneManager.GetActiveScene().buildIndex == 0)
        {
            Debug.LogError("Core:: The scene '" + SceneManager.GetActiveScene().name + "' is at index 0, but index 0 is reserved for the Main Menu. This scene does not contain an FPEMainMenu object. You must add one or controls will not work as expected.");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Debug.Log("Anulando player al ser menú");

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Destroy(player);
            }
        }

        else
        {
            StartCoroutine(SpawnPlayerWithDelay(0.25f));
        }
    }

    private IEnumerator SpawnPlayerWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject player = Instantiate(playerPrefab, null);
        PlayerStartLocation startLocation = GameObject.FindObjectOfType<PlayerStartLocation>();

        if (startLocation != null)
        {
            player.transform.position = startLocation.gameObject.transform.position;
            Quaternion flatRotation = Quaternion.Euler(0.0f, startLocation.gameObject.transform.rotation.eulerAngles.y, 0.0f);
            player.transform.rotation = flatRotation;
            Debug.Log("Spawneando player en " + player.transform.position);
        }

        else
        {
            Debug.LogWarning("Core:: PlayerStartLocation was not found. Placing player at origin");
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
        }
    }
}