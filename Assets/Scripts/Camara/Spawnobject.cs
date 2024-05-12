using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnobject : MonoBehaviour
{
    public GameObject cube;

    public SHAKECAMERA cameraShake;
    void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(cameraShake.shake());
        }
        
    }
}
