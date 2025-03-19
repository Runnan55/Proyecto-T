using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBoss : MonoBehaviour
{
    public float explosionRadius = 5f; // Radio de la explosi�n
    public float explosionForce = 500f; // Fuerza total de la explosi�n
    public int damage = 50; // Cantidad de da�o causado por la explosi�n
    public LayerMask damageLayer; // Capa de objetos que pueden recibir da�o (ej. Player)

    public GameObject explosionEffect; // Prefab de part�culas para la explosi�n
    public GameObject explosionPreview; // Prefab de part�culas para la previsualizaci�n de la explosi�n
    public float effectDuration = 1f; // Duraci�n del efecto visual antes de destruirlo
    public float explosionDuration = 1f; // Duraci�n de la fuerza de la explosi�n sobre el jugador (en segundos)

    // Color del cilindro generado para la previsualizaci�n y el efecto
    public Color previewColor = Color.yellow;
    public Color effectColor = Color.red;

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

        // Crear un cilindro din�micamente que visualice el �rea de la explosi�n
        CreateExplosionPreviewCylinder();
    }

    // Crear un cilindro que visualice el �rea de la previsualizaci�n
    private void CreateExplosionPreviewCylinder()
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = transform.position; // Colocar el cilindro en la posici�n de la explosi�n

        // Ajustar el tama�o del cilindro seg�n el radio de la explosi�n
        float height = 1f; // Altura del cilindro (puedes ajustarlo seg�n sea necesario)
        cylinder.transform.localScale = new Vector3(explosionRadius * 2, height, explosionRadius * 2); // Escalar el cilindro

        // Cambiar el color del cilindro de previsualizaci�n
        Renderer cylinderRenderer = cylinder.GetComponent<Renderer>();
        if (cylinderRenderer != null)
        {
            cylinderRenderer.material.color = previewColor; // Asignar el color de la previsualizaci�n
        }

        // Destruir el cilindro despu�s del tiempo de previsualizaci�n
        Destroy(cylinder, effectDuration);
    }

    // Crear un cilindro para el efecto de la explosi�n
    private void CreateExplosionEffectCylinder()
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = transform.position; // Colocar el cilindro en la posici�n de la explosi�n

        // Ajustar el tama�o del cilindro seg�n el radio de la explosi�n
        float height = 1f; // Altura del cilindro (puedes ajustarlo seg�n sea necesario)
        cylinder.transform.localScale = new Vector3(explosionRadius * 2, height, explosionRadius * 2); // Escalar el cilindro

        // Cambiar el color del cilindro de la explosi�n
        Renderer cylinderRenderer = cylinder.GetComponent<Renderer>();
        if (cylinderRenderer != null)
        {
            cylinderRenderer.material.color = effectColor; // Asignar el color del efecto de la explosi�n
        }

        // Destruir el cilindro despu�s de un tiempo
        Destroy(cylinder, effectDuration);
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

        // Crear el cilindro para el efecto visual de la explosi�n
        CreateExplosionEffectCylinder();

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

            // Aplicar fuerza f�sica de manera inmediata durante 1 segundo si el objeto tiene un Rigidbody
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                StartCoroutine(ApplyExplosionForceForOneSecond(rb)); // Aplicar fuerza por un segundo
            }
        }

        // Opcional: A�adir efectos visuales o de sonido
        Destroy(gameObject, effectDuration); // Destruir el objeto de la explosi�n despu�s de ejecutarse
    }

    // Corutina para aplicar la fuerza de la explosi�n solo durante 1 segundo
    private IEnumerator ApplyExplosionForceForOneSecond(Rigidbody rb)
    {
        float elapsedTime = 0f;

        // Aplicar fuerza de forma instant�nea
        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

        // Esperar 1 segundo para que no se aplique m�s fuerza
        while (elapsedTime < explosionDuration)
        {
            elapsedTime += Time.deltaTime; // Incrementar el tiempo transcurrido
            yield return null; // Esperar un frame antes de continuar
        }

        // Despu�s de 1 segundo, no hacer nada m�s (no se aplica m�s fuerza)
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar el �rea de efecto de la explosi�n en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}