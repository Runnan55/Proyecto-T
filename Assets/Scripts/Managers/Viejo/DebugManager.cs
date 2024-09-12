using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class DebugManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene("Hub");
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            SceneManager.LoadScene("Strength");
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene("HighPriestess");
        }
        
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SceneManager.LoadScene("Devil");
        }
    }
}
