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
        // Comprobar si el objeto que entra en el trigger es el Boomerang o un ataque del jugador
        if (other.gameObject.CompareTag("Boomerang") || EsAtaqueDelJugador(other.gameObject))
        {
            // Activar la variable booleana
            Activado = true;

            // Cambiar el color del objeto a verde
            GetComponent<Renderer>().material.color = Color.green;
        }
    }

    // Método para comprobar si el objeto es un ataque del jugador
    bool EsAtaqueDelJugador(GameObject objeto)
    {
        // Comprobar si el nombre del objeto coincide con los nombres de los colliders de ataque
        return objeto.name == "AtaqueL1" || objeto.name == "AtaqueL2" || objeto.name == "AtaqueL3" || objeto.name == "CollidersAtaqueP";
    }
}
