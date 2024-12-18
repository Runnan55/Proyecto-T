using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTp : MonoBehaviour
{
    public string scene;
    void OnTriggerEnter()
    {
        SceneManager.LoadScene(scene);
    }
}
