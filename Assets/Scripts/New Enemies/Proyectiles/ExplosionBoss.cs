using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBoss : MonoBehaviour
{
    public float explosionRadius = 5f; // Radio de la explosión
    public float explosionForce = 500f; // Fuerza total de la explosión
    public int damage = 50; // Cantidad de daño causado por la explosión
    public LayerMask damageLayer; // Capa de objetos que pueden recibir daño (ej. Player)

    public GameObject explosionEffect; // Prefab de partículas para la explosión
    public GameObject explosionPreview; // Prefab de partículas para la previsualización de la explosión
    public float effectDuration = 1f; // Duración del efecto visual antes de destruirlo
    public float explosionDuration = 1f; // Duración de la fuerza de la explosión sobre el jugador (en segundos)

    // Color del cilindro generado para la previsualización y el efecto
    public Color previewColor = Color.yellow;
    public Color effectColor = Color.red;

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

        // Crear un cilindro dinámicamente que visualice el área de la explosión
        CreateExplosionPreviewCylinder();
    }

    // Crear un cilindro que visualice el área de la previsualización
    private void CreateExplosionPreviewCylinder()
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = transform.position; // Colocar el cilindro en la posición de la explosión

        // Ajustar el tamaño del cilindro según el radio de la explosión
        float height = 1f; // Altura del cilindro (puedes ajustarlo según sea necesario)
        cylinder.transform.localScale = new Vector3(explosionRadius * 2, height, explosionRadius * 2); // Escalar el cilindro

        // Cambiar el color del cilindro de previsualización
        Renderer cylinderRenderer = cylinder.GetComponent<Renderer>();
        if (cylinderRenderer != null)
        {
            cylinderRenderer.material.color = previewColor; // Asignar el color de la previsualización
        }

        // Destruir el cilindro después del tiempo de previsualización
        Destroy(cylinder, effectDuration);
    }

    // Crear un cilindro para el efecto de la explosión
    private void CreateExplosionEffectCylinder()
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = transform.position; // Colocar el cilindro en la posición de la explosión

        // Ajustar el tamaño del cilindro según el radio de la explosión
        float height = 1f; // Altura del cilindro (puedes ajustarlo según sea necesario)
        cylinder.transform.localScale = new Vector3(explosionRadius * 2, height, explosionRadius * 2); // Escalar el cilindro

        // Cambiar el color del cilindro de la explosión
        Renderer cylinderRenderer = cylinder.GetComponent<Renderer>();
        if (cylinderRenderer != null)
        {
            cylinderRenderer.material.color = effectColor; // Asignar el color del efecto de la explosión
        }

        // Destruir el cilindro después de un tiempo
        Destroy(cylinder, effectDuration);
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

        // Crear el cilindro para el efecto visual de la explosión
        CreateExplosionEffectCylinder();

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

            // Aplicar fuerza física de manera inmediata durante 1 segundo si el objeto tiene un Rigidbody
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                StartCoroutine(ApplyExplosionForceForOneSecond(rb)); // Aplicar fuerza por un segundo
            }
        }

        // Opcional: Añadir efectos visuales o de sonido
        Destroy(gameObject, effectDuration); // Destruir el objeto de la explosión después de ejecutarse
    }

    // Corutina para aplicar la fuerza de la explosión solo durante 1 segundo
    private IEnumerator ApplyExplosionForceForOneSecond(Rigidbody rb)
    {
        float elapsedTime = 0f;

        // Aplicar fuerza de forma instantánea
        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

        // Esperar 1 segundo para que no se aplique más fuerza
        while (elapsedTime < explosionDuration)
        {
            elapsedTime += Time.deltaTime; // Incrementar el tiempo transcurrido
            yield return null; // Esperar un frame antes de continuar
        }

        // Después de 1 segundo, no hacer nada más (no se aplica más fuerza)
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar el área de efecto de la explosión en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}