using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAttack : MonoBehaviour
{
    public float damageAmount = 20f; // Cantidad de daño que se aplica al jugador
    private Life playerLife;

    void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el trigger colisionó con el jugador
        if (other.CompareTag("Player"))
        {
       
            if (playerLife != null)
            {
                // Aplicar daño al jugador
                playerLife.ModifyTime(-damageAmount);
            }
        }
    }
}