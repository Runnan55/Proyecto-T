using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAttack : MonoBehaviour
{
    public float damageAmount = 10f; // Cantidad de daño que se aplica al jugador

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

            // Destruir el cubo después de la colisión
            Destroy(gameObject);
        }
    }
}