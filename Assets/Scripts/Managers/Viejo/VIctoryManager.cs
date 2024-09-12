using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIctoryManager : MonoBehaviour
{
    public LevelManager levelManager;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();

        if (levelManager != null)
        {
            Debug.Log("LevelManager encontrado");
        }
        else
        {
            Debug.Log("LevelManager no encontrado");
        }
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            levelManager.OnLevelCompleted();
        }    
    }
}