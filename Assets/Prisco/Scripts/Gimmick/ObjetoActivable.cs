using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoActivable : MonoBehaviour
{
    public bool Activado = false;
    public bool Mover = false; // Nueva variable para gestionar el movimiento
    public Transform[] posiciones; // Array de posiciones
    public float velocidad = 1f; // Velocidad de movimiento
    private int indiceActual = 0; // Índice de la posición actual

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mover && posiciones.Length > 0) // Usar Mover en lugar de Activado
        {
            // Mover el objeto hacia la posición actual
            transform.position = Vector3.MoveTowards(transform.position, posiciones[indiceActual].position, velocidad  * MovimientoJugador.bulletTimeScale * Time.deltaTime);

            // Verificar si llegó a la posición actual
            if (Vector3.Distance(transform.position, posiciones[indiceActual].position) < 0.01f)
            {
                // Cambiar al siguiente índice
                indiceActual = (indiceActual + 1) % posiciones.Length;
            }
        }
    }

     // Método que se llama cuando este objeto entra en contacto con otro trigger
  void OnTriggerEnter(Collider other)
    {
        // Comprobar si el objeto que entra en el trigger es el Boomerang o un ataque del jugador
        if (other.gameObject.CompareTag("Boomerang") || EsAtaqueDelJugador(other.gameObject))
        {
            // Activar la variable booleana
            Activado = true;
            Mover = true; // Activar el movimiento

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
