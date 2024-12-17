using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBoss : MonoBehaviour
{
    public float explosionRadius = 5f; // Radio de la explosi�n
    public float explosionForce = 500f; // Fuerza f�sica de la explosi�n
    public int damage = 50; // Cantidad de da�o causado por la explosi�n
    public LayerMask damageLayer; // Capa de objetos que pueden recibir da�o (ej. Player)

    public GameObject explosionEffect; // Prefab de part�culas para la explosi�n
    public GameObject explosionPreview; // Prefab de part�culas para la explosi�n
    public float effectDuration = 1f; // Duraci�n del efecto visual antes de destruirlo

    private void Update()
    {
  
    }

    public void TriggerExplosion()
    {
        Debug.Log("Previsualizaci�n de explosi�n activada!");

        if (explosionPreview != null)
        {
            // Crear y mostrar la previsualizaci�n
            GameObject preview = Instantiate(explosionPreview, transform.position, Quaternion.identity);
            Destroy(preview, effectDuration); // Destruir la previsualizaci�n despu�s del tiempo definido

            // Iniciar la explosi�n despu�s del tiempo de previsualizaci�n
            StartCoroutine(StartExplosionAfterDelay(effectDuration));
        }
        else
        {
            Debug.LogWarning("No se asign� un prefab de previsualizaci�n para la explosi�n.");
        }
    }

    // Corrutina para gestionar el retardo de la explosi�n
    private IEnumerator StartExplosionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Esperar el tiempo de la previsualizaci�n

        Debug.Log("�Explosi�n activada!");

        if (explosionEffect != null)
        {
            // Crear el efecto de explosi�n
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, effectDuration); // Destruir las part�culas despu�s de la duraci�n
        }

        // Obtener todos los objetos dentro del radio de la explosi�n
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);

        foreach (Collider hit in hitColliders)
        {
            // Aplicar da�o si el objeto tiene el script "PlayerHealth"
            Life playerHealth = hit.GetComponent<Life>();
            if (playerHealth != null)
            {
                playerHealth.ModifyTime(-damage);
                Debug.Log($"El jugador recibi� {damage} de da�o por la explosi�n.");
            }

            // Aplicar fuerza f�sica si el objeto tiene un Rigidbody
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Opcional: A�adir efectos visuales o de sonido
        Destroy(gameObject, effectDuration); // Destruir el objeto de la explosi�n despu�s de ejecutarse
    }

  
    private void OnDrawGizmosSelected()
    {
        // Dibujar el �rea de efecto de la explosi�n en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}