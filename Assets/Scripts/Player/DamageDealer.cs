using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageDealer : MonoBehaviour
{
    public float damageAmount;
    private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // Solo verificar si ya ha recibido daño cuando NO esté en god mode
        if (!MovimientoJugador.godMode && damagedObjects.Contains(other.gameObject))
        {
            return; // Si el objeto ya ha recibido daño, no hacer nada
        }

        EnemyLife enemy = other.GetComponent<EnemyLife>();
        BossLIfe bossLife = other.GetComponent<BossLIfe>();
        BossSpawner bossSpawner = other.GetComponent<BossSpawner>();
        BossArmProtector bossArmProtector = other.GetComponent<BossArmProtector>();

        //Debug.Log("Entrando en contacto con: " + other.name);
        if (enemy != null)
        {
            enemy.ReceiveDamage(damageAmount);
           // Debug.Log("dano");
            if (!MovimientoJugador.godMode)
            {
                damagedObjects.Add(other.gameObject); // Solo registrar en modo normal
            }
        }

        if (bossLife != null)
        {
            bossLife.TakeDamage(damageAmount);
            if (!MovimientoJugador.godMode)
            {
                damagedObjects.Add(other.gameObject); // Solo registrar en modo normal
            }
        }

        if (bossArmProtector != null)
        {
            bossArmProtector.TakeDamage(damageAmount);
            if (!MovimientoJugador.godMode)
            {
                damagedObjects.Add(other.gameObject); // Solo registrar en modo normal
            }
        }
        if (bossSpawner != null)
        {
            bossSpawner.TakeDamage(damageAmount);
            if (!MovimientoJugador.godMode)
            {
                damagedObjects.Add(other.gameObject); // Solo registrar en modo normal
            }
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