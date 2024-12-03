using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoActivable : MonoBehaviour
{
    public bool Activado = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     // Método que se llama cuando este objeto entra en contacto con otro trigger
    void OnTriggerEnter(Collider other)
    {
        // Comprobar si el objeto con el que colisiona tiene el tag "Proyectil"
        if (other.gameObject.CompareTag("Boomerang"))
        {
            // Activar la variable booleana
            Activado = true;

            GetComponent<Renderer>().material.color = Color.green;
        }
    }
  
}
