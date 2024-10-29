using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float damageAmount; //poner daÃ±o en el inspector

    private void OnTriggerEnter(Collider other)
    {
        EnemyLife enemy = other.GetComponent<EnemyLife>();
        BossHealth bossHealth = other.GetComponent<BossHealth>();
        if (enemy != null)
        {
            enemy.ReceiveDamage(damageAmount);
            Debug.Log("dano");
        }

        if (bossHealth != null)
        {
            bossHealth.TakeDamage(damageAmount);
        }
    }
}
