using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicBomb : MonoBehaviour
{
    private Transform player;                 // El objetivo al que se dirige la bomba (jugador)
    public float heightOffset = 10f;         // Altura máxima de la parábola en relación a la posición actual
    public float gravity = -9.81f;           // Gravedad personalizada para el proyectil
    public float damageRadius = 5f;          // Radio de daño de la explosión
    public float damageAmount = 20f;         // Daño infligido en el área
    private string targetTag = "Player";      // La etiqueta del objetivo (Player)
    public GameObject explosionEffect;       // Efecto visual para la explosión (opcional)

    private Rigidbody rb;
    private bool hasLaunched = false;        // Para controlar si ya se ha lanzado la bomba

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;               // Desactivar la gravedad de Unity para controlar el movimiento manualmente

        
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        // Lanza la bomba hacia el objetivo si aún no ha sido lanzada
        if (!hasLaunched && player != null)
        {
            Launch();
            hasLaunched = true;
        }

        // Aplicar gravedad manualmente al Rigidbody para asegurar la trayectoria parabólica
        if (hasLaunched)
        {
            rb.velocity += Vector3.up * gravity * Time.deltaTime; // Aplicar la gravedad manualmente
        }
    }

    void Launch()
    {
        // Calcular la trayectoria parabólica para que aterrice en el objetivo
        Vector3 targetPosition = player.position;
        Vector3 launchVelocity = CalculateParabolicVelocity(transform.position, targetPosition, heightOffset);

        // Asignar la velocidad calculada al Rigidbody
        rb.velocity = launchVelocity;
    }

    // Método que calcula la velocidad inicial para una trayectoria parabólica
    Vector3 CalculateParabolicVelocity(Vector3 start, Vector3 end, float heightOffset)
    {
        // Calcular la diferencia de posiciones
        Vector3 distance = end - start;

        // Dividir la distancia horizontal (x y z)
        Vector3 horizontalDistance = new Vector3(distance.x, 0, distance.z);

        // Tiempo de vuelo basado en la distancia horizontal y la altura máxima deseada
        float timeToTarget = Mathf.Sqrt(-2 * heightOffset / gravity) + Mathf.Sqrt(2 * (distance.y - heightOffset) / gravity);

        // Calcular la velocidad inicial necesaria para la trayectoria horizontal
        Vector3 horizontalVelocity = horizontalDistance / timeToTarget;

        // Calcular la velocidad vertical inicial para alcanzar la altura máxima deseada
        float verticalVelocity = Mathf.Sqrt(-2 * gravity * heightOffset);

        // Combinar la velocidad horizontal y vertical para obtener la velocidad inicial completa
        Vector3 initialVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

        return initialVelocity;
    }

    // Detectar colisión con el jugador usando OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si la colisión es con el objeto que tiene la etiqueta "Player"
        if (other.CompareTag(targetTag)|| other.CompareTag("Ground"))
        {
            Explode();
        }
    }

    void Explode()
    {
        // Crear un área de daño en la posición de la bomba
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, damageRadius);

        // Aplicar daño a todos los objetos en el radio de explosión con la etiqueta del jugador
        foreach (Collider hit in hitObjects)
        {
            if (hit.CompareTag(targetTag))
            {
                Life targetHealth = hit.GetComponent<Life>();
                if (targetHealth != null)
                {
                    targetHealth.ModifyTime(-damageAmount);
                }
            }
        }

        // Mostrar el efecto visual de explosión (opcional)
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Destruir la bomba tras la explosión
        Destroy(gameObject);
    }

    // Visualizar el área de daño en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}