using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private static InteractionManager _instance;
    public static InteractionManager Instance 
    {
        get { return _instance; }
    }

    private bool initialized = false;

    void Awake()
    {

        if (_instance != null)
        {
            Debug.LogWarning("InteractionManager:: Duplicate instance of InteractionManager, deleting second one.");
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

    public void initialize()
    {
        if (!initialized)
        {
            initialized = true;
        }
    }
}
