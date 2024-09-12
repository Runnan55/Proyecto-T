using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreManager : MonoBehaviour
{
    private static CoreManager _instance;
    public static CoreManager Instance {
        get { return _instance; }
    }

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

            // This warning is very harmless, and can be ignored. It should indicate that you are returning to the 
            // first level in your game. If you get this warning in OTHER scenes, it means you have an FPECore
            // prefab in that scene, which you don't need. You can also delete the Debug.Log call if you want.
            Debug.Log("FPECore:: Duplicate FPECore, deleting duplicate instance. If you saw this message when loading a saved game into your 1st scene, ignore. Otherwise, it's still not a problem, but might indicate redundant FPECore in a secondary Scene file or something similar.");

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

        if (!HUDPrefab || !eventSystemPrefab || !interactionManagerPrefab || !playerPrefab || !inputManagerPrefab || !saveLoadManagerPrefab || !menuPrefab)
        {
            Debug.LogError("FPECore:: Missing prefab for core component. Game will not function correctly. See Inspector for object '" + gameObject.name + "' to ensure all fields are populated correctly.");
        }

        Instantiate(eventSystemPrefab, null);

        Instantiate(HUDPrefab, null);
        Instantiate(interactionManagerPrefab, null);


        GameObject player = Instantiate(playerPrefab, null);
        /* FPEPlayerStartLocation startLocation = GameObject.FindObjectOfType<FPEPlayerStartLocation>();

        if (startLocation != null)
        {

            player.transform.position = startLocation.gameObject.transform.position;
            Quaternion flatRotation = Quaternion.Euler(0.0f, startLocation.gameObject.transform.rotation.eulerAngles.y, 0.0f);
            player.transform.rotation = flatRotation;

        }
        else
        {

            Debug.LogWarning("FPECore:: No FPEPlayerStartLocation was found. Placing player at origin");
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;

        } */

        Instantiate(inputManagerPrefab, null);
        Instantiate(saveLoadManagerPrefab, null);
        Instantiate(menuPrefab, null);

        //HUD.Instance.initialize();
        //InteractionManager.Instance.initialize();

        // Lastly, load game options
        //SaveLoadManager.Instance.LoadGameOptions();


        // Helper checks //

        // Check if we're in the 0th scene. If we are, and there is no FPEMainMenu object, this will cause issues with player controls. Print an error and exit play mode.
        if(GameObject.FindObjectOfType<MainMenu>() == null && SceneManager.GetActiveScene().buildIndex == 0)
        {
            Debug.LogError("FPECore:: The scene '" + SceneManager.GetActiveScene().name + "' is at index 0, but index 0 is reserved for the Main Menu. This scene does not contain an FPEMainMenu object. You must add one or controls will not work as expected.");
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

/*             // If we're in the main menu scene, freeze the player
        if (SceneManager.GetActiveScene().buildIndex == SaveLoadManager.Instance.MainMenuSceneBuildIndex)
        {
            InteractionManager.Instance.suspendPlayerAndInteraction();
        } */

    }
    
}
