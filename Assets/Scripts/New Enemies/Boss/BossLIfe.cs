using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLIfe : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public EnemyVision enemyVision; // Referencia al script de visi�n

    public float frontDamageMultiplier = 0.25f;
    public float normalDamageMultiplier = 1f;
    
    public int contador=0;
  
    public int contador2 = 0;


    public Slider healthBar;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;  // Establece el valor m�ximo
            healthBar.value = currentHealth;  // Establece el valor inicial
        }
    }


    public void TakeDamage(float damage)
    {
        // Verificar si el jugador est� dentro del rango de visi�n y en el �ngulo
        if (enemyVision.PlayerEnRango() && enemyVision.DetectarAngulo())
        {
            // Si el jugador est� dentro del �ngulo de visi�n, aplicar da�o reducido
            damage *= frontDamageMultiplier;
            contador++;
            contador2++;

        }
        else
        {
            // Si el jugador est� fuera del �ngulo de visi�n, aplicar da�o normal
            damage *= normalDamageMultiplier;
            contador++;
            contador2++;

        }

        currentHealth -= damage;
        Debug.Log($"Da�o recibido: {damage}. Vida restante: {currentHealth}");
        if (healthBar != null)
        {
            healthBar.value = currentHealth;  // Actualiza el valor del Slider
        }

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