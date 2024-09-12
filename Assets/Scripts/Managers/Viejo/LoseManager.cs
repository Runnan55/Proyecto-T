using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseManager : MonoBehaviour
{
    public GameObject player;
    public PlayerMovement pm;
    public LevelManager levelManager;


    public bool losed = false;
    
    void Awake()
    {
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        levelManager = FindObjectOfType<LevelManager>();
    }
    
    // Start is called before the first frame update
    public void Lose ()
    {
        pm.speed = 0;
        pm.animator.Play("Idle");
        pm.enabled = false;

       // Time.timeScale = 0;

        losed = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && losed==true)
        {
            Lose(); // Ejecuta el método Lose
            levelManager.OnLevelFailed(); // Notifica al LevelManager que el nivel ha fallado
        }
    }

}
