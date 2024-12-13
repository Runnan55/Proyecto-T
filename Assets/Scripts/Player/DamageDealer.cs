using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageDealer : MonoBehaviour
{
    public float damageAmount;
    private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (damagedObjects.Contains(other.gameObject))
        {
            return; // Si el objeto ya ha recibido daño, no hacer nada
        }

        EnemyLife enemy = other.GetComponent<EnemyLife>();
        BossLIfe bossLife = other.GetComponent<BossLIfe>();

        if (enemy != null)
        {
            enemy.ReceiveDamage(damageAmount);
           // Debug.Log("dano");
            damagedObjects.Add(other.gameObject); // Registrar que este objeto ha recibido daño
        }

        if (bossLife != null)
        {
            bossLife.TakeDamage(damageAmount);
            damagedObjects.Add(other.gameObject); // Registrar que este objeto ha recibido daño
        }
    }

    public void ResetDamage()
    {
        damagedObjects.Clear(); // Limpiar la lista de objetos dañados
    }

    private void OnDisable()
    {
        // Limpiar la lista cuando el objeto se desactive
        damagedObjects.Clear();
    }
}