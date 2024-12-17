using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBoss : MonoBehaviour
{
    public float explosionRadius = 5f; // Radio de la explosión
    public float explosionForce = 500f; // Fuerza física de la explosión
    public int damage = 50; // Cantidad de daño causado por la explosión
    public LayerMask damageLayer; // Capa de objetos que pueden recibir daño (ej. Player)

    public GameObject explosionEffect; // Prefab de partículas para la explosión
    public GameObject explosionPreview; // Prefab de partículas para la explosión
    public float effectDuration = 1f; // Duración del efecto visual antes de destruirlo

    private void Update()
    {
  
    }

    public void TriggerExplosion()
    {
        Debug.Log("Previsualización de explosión activada!");

        if (explosionPreview != null)
        {
            // Crear y mostrar la previsualización
            GameObject preview = Instantiate(explosionPreview, transform.position, Quaternion.identity);
            Destroy(preview, effectDuration); // Destruir la previsualización después del tiempo definido

            // Iniciar la explosión después del tiempo de previsualización
            StartCoroutine(StartExplosionAfterDelay(effectDuration));
        }
        else
        {
            Debug.LogWarning("No se asignó un prefab de previsualización para la explosión.");
        }
    }

    // Corrutina para gestionar el retardo de la explosión
    private IEnumerator StartExplosionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Esperar el tiempo de la previsualización

        Debug.Log("¡Explosión activada!");

        if (explosionEffect != null)
        {
            // Crear el efecto de explosión
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, effectDuration); // Destruir las partículas después de la duración
        }

        // Obtener todos los objetos dentro del radio de la explosión
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);

        foreach (Collider hit in hitColliders)
        {
            // Aplicar daño si el objeto tiene el script "PlayerHealth"
            Life playerHealth = hit.GetComponent<Life>();
            if (playerHealth != null)
            {
                playerHealth.ModifyTime(-damage);
                Debug.Log($"El jugador recibió {damage} de daño por la explosión.");
            }

            // Aplicar fuerza física si el objeto tiene un Rigidbody
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Opcional: Añadir efectos visuales o de sonido
        Destroy(gameObject, effectDuration); // Destruir el objeto de la explosión después de ejecutarse
    }

  
    private void OnDrawGizmosSelected()
    {
        // Dibujar el área de efecto de la explosión en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}