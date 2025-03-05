using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLIfe : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public EnemyVision enemyVision; // Referencia al script de visión

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
            healthBar.maxValue = maxHealth;  // Establece el valor máximo
            healthBar.value = currentHealth;  // Establece el valor inicial
        }
    }


    public void TakeDamage(float damage)
    {
        // Verificar si el jugador está dentro del rango de visión y en el ángulo
        if (enemyVision.PlayerEnRango() && enemyVision.DetectarAngulo())
        {
            // Si el jugador está dentro del ángulo de visión, aplicar daño reducido
            damage *= frontDamageMultiplier;
            contador++;
            contador2++;

        }
        else
        {
            // Si el jugador está fuera del ángulo de visión, aplicar daño normal
            damage *= normalDamageMultiplier;
            contador++;
            contador2++;

        }

        currentHealth -= damage;
        Debug.Log($"Daño recibido: {damage}. Vida restante: {currentHealth}");
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