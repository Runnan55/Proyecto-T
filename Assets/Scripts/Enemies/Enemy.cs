using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;
    public Life playerLife;

    void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ReceiveDamage(float amount) // (en segundos)
    {
        health -= amount;
        if (health < 0)
        {
            health = 0;
        }
        
        Debug.Log("Daño infringido: " + amount + ", Vida: " + health);
    }

    public virtual void AttackPlayer()
    {
        playerLife.ModifyTime(-damage);
    }
}