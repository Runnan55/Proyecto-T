using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EspadaAtaque : MonoBehaviour
{
   public Collider myCollider; // Añade esta línea

   public void OnTriggerEnter(Collider other)
   {
      
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collider has collided with Enemy");
        }
   }

   public void ActivarEspada()
   {
       myCollider.enabled = true; // Cambia esta línea
   }

   public void DesactivarEspada()
   {
       myCollider.enabled = false; // Y esta línea
   }
}