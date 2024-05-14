using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIctoryManager : MonoBehaviour
{
    public LevelManager levelManager;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
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