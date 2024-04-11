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
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ReceiveDamage(damage);
                //Debug.Log("hitmarker");
            }
        }
   }

 

   
    
   
}