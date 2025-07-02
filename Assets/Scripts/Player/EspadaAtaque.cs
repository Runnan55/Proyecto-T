using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EspadaAtaque : MonoBehaviour
{
  
   public float damage = 10f;

   public void OnTriggerEnter(Collider other)
   {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collider has collided with Enemy");
            
            // Primero intentar con la nueva clase EnemyLife
            EnemyLife enemyLife = other.gameObject.GetComponent<EnemyLife>();
            if (enemyLife != null)
            {
                enemyLife.ReceiveDamage(damage);
                Debug.Log("Daño aplicado a EnemyLife: " + damage);
                return;
            }
            
            // Si no tiene EnemyLife, intentar con la clase Enemy antigua
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ReceiveDamage(damage);
                Debug.Log("Daño aplicado a Enemy (clase antigua): " + damage);
            }
        }
   }
}