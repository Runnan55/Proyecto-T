using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAttack : MonoBehaviour
{
    public float damageAmount = 10f; // Cantidad de daño que se aplica al jugador
    public float destroyDelay = 2f;   // Retraso antes de destruir el cubo

    void Start()
    {
        // Destruir el cubo después de un retraso, independientemente de si colisiona con el jugador o no
        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el trigger colisionó con el jugador
        if (other.CompareTag("Player"))
        {
            // Obtener el componente Life del jugador
            Life playerLife = other.GetComponent<Life>();

            // Verificar si el jugador tiene el componente Life
            if (playerLife != null)
            {
                // Aplicar daño al jugador
                playerLife.ModifyTime(-damageAmount);
            }
        }
    }
}