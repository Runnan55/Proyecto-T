using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossHealth : MonoBehaviour
{
    public float maxHealth = 1000f;
    public float currentHealth;
  // public Slider healthSlider;
    public GameObject secondBoss; 


    void Start()
    {
        currentHealth = maxHealth;
        //healthSlider.maxValue = maxHealth;
       // healthSlider.value = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
       // healthSlider.value = currentHealth;
        if (currentHealth <= 0)
        {
            Debug.Log("muerto");
            Die();

        }
    }

    public void Die()
    {

        if (secondBoss != null)
        {
            secondBoss.SetActive(true);
        }
        // Implementar lo que sucede cuando el jefe muere.
        Destroy(gameObject);
    }
}