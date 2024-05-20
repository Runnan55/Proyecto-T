using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float damageAmount; //poner daÃ±o en el inspector

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        BossHealth bossHealth = other.GetComponent<BossHealth>();
        if (enemy != null)
        {
            enemy.ReceiveDamage(damageAmount);
        }

        if (bossHealth != null)
        {
            bossHealth.TakeDamage(damageAmount);
        }
    }
}
