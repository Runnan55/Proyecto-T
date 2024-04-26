using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class borrarEjemploProfe : MonoBehaviour
{
    Coroutine cor;
    // Start is called before the first frame update
    void Start()
    {
       cor = StartCoroutine(micorutina());
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StopCoroutine(cor);
        }
    }

    IEnumerator micorutina()
    {
        while (true)
        {
            print("estoy en el bucle");
            yield return new WaitForSeconds(1);
        }
        

    }
}
