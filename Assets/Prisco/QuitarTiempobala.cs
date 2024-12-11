using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitarTiempobala : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        // Método que se llama cuando otro objeto entra en el trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Volver el tiempo bala a la normalidad
            MovimientoJugador.bulletTimeScale = 1f;
        }
    }
}
