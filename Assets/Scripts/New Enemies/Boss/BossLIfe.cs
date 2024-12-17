using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLIfe : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public EnemyVision enemyVision; // Referencia al script de visi�n

    public float frontDamageMultiplier = 0.25f;
    public float normalDamageMultiplier = 1f;
    
    public int contador=0;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(float damage)
    {
        // Verificar si el jugador est� dentro del rango de visi�n y en el �ngulo
        if (enemyVision.PlayerEnRango() && enemyVision.DetectarAngulo())
        {
            // Si el jugador est� dentro del �ngulo de visi�n, aplicar da�o reducido
            damage *= frontDamageMultiplier;
        }
        else
        {
            // Si el jugador est� fuera del �ngulo de visi�n, aplicar da�o normal
            damage *= normalDamageMultiplier;
            contador++;
        }

        currentHealth -= damage;
        Debug.Log($"Da�o recibido: {damage}. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemigo derrotado.");
        Destroy(gameObject);
    }
}