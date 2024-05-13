using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDamage : MonoBehaviour
{
    public int damageAmount = 30; // Cantidad de da�o que inflige el c�rculo

    private void OnTriggerStay(Collider other)
    {
        // Verifica si el objeto que colision� es el jugador
        if (other.CompareTag("Player"))
        {
            // Obt�n el componente de salud del jugador (supongamos que tiene un script llamado Health)
            Life playerHealth = other.GetComponent<Life>();

            // Si el jugador tiene el componente de salud
            if (playerHealth != null)
            {
                // Aplica el da�o al jugador
                playerHealth.ModifyTime(-damageAmount);
            }
        }
    }
}