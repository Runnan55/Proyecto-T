using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDamage : MonoBehaviour
{
    public int damageAmount = 30; // Cantidad de daño que inflige el círculo

    private void OnTriggerStay(Collider other)
    {
        // Verifica si el objeto que colisionó es el jugador
        if (other.CompareTag("Player"))
        {
            // Obtén el componente de salud del jugador (supongamos que tiene un script llamado Health)
            Life playerHealth = other.GetComponent<Life>();

            // Si el jugador tiene el componente de salud
            if (playerHealth != null)
            {
                // Aplica el daño al jugador
                playerHealth.ModifyTime(-damageAmount);
            }
        }
    }
}