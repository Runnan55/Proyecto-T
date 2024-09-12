using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    
    // Simplemente un singleton para el EventSystem

    private static EventSystem _instance;
    public static EventSystem Instance 
    {
        get { return _instance; }
    }

    void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("EventSystem:: Duplicate instance of EventSystem, deleting second one.");
             Destroy(this.gameObject);
        }

        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}