using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestruirProyectil : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
         // Si el objeto con el que el proyectil está colisionando no tiene la etiqueta "Player"
        if (other.gameObject.tag != "Player")
        {
            // Destruye el proyectil
            Destroy(gameObject);
        }
    }
}
