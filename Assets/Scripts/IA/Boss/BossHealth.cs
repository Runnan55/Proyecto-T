using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossHealth : MonoBehaviour
{
    public float maxHealth = 20f;
    public float currentHealth;
    public GameObject secondBoss; 


    void Start()
    {
        currentHealth = maxHealth;
       
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.Log("muerto");
            ActiveBoss();
            Die();

        }
    }
    void ActiveBoss()
    {
        if (secondBoss != null)
        {

            secondBoss.SetActive(true);

        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}