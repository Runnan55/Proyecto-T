using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicBomb : MonoBehaviour
{
    private Transform player;                 // El objetivo al que se dirige la bomba (jugador)
    public float heightOffset = 10f;         // Altura m�xima de la par�bola en relaci�n a la posici�n actual
    public float gravity = -9.81f;           // Gravedad personalizada para el proyectil
    public float damageRadius = 5f;          // Radio de da�o de la explosi�n
    public float damageAmount = 20f;         // Da�o infligido en el �rea
    private string targetTag = "Player";      // La etiqueta del objetivo (Player)
    public GameObject explosionEffect;       // Efecto visual para la explosi�n (opcional)

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
        // Lanza la bomba hacia el objetivo si a�n no ha sido lanzada
        if (!hasLaunched && player != null)
        {
            Launch();
            hasLaunched = true;
        }

        // Aplicar gravedad manualmente al Rigidbody para asegurar la trayectoria parab�lica
        if (hasLaunched)
        {
            rb.velocity += Vector3.up * gravity * Time.deltaTime; // Aplicar la gravedad manualmente
        }
    }

    void Launch()
    {
        // Calcular la trayectoria parab�lica para que aterrice en el objetivo
        Vector3 targetPosition = player.position;
        Vector3 launchVelocity = CalculateParabolicVelocity(transform.position, targetPosition, heightOffset);

        // Asignar la velocidad calculada al Rigidbody
        rb.velocity = launchVelocity;
    }

    // M�todo que calcula la velocidad inicial para una trayectoria parab�lica
    Vector3 CalculateParabolicVelocity(Vector3 start, Vector3 end, float heightOffset)
    {
        // Calcular la diferencia de posiciones
        Vector3 distance = end - start;

        // Dividir la distancia horizontal (x y z)
        Vector3 horizontalDistance = new Vector3(distance.x, 0, distance.z);

        // Tiempo de vuelo basado en la distancia horizontal y la altura m�xima deseada
        float timeToTarget = Mathf.Sqrt(-2 * heightOffset / gravity) + Mathf.Sqrt(2 * (distance.y - heightOffset) / gravity);

        // Calcular la velocidad inicial necesaria para la trayectoria horizontal
        Vector3 horizontalVelocity = horizontalDistance / timeToTarget;

        // Calcular la velocidad vertical inicial para alcanzar la altura m�xima deseada
        float verticalVelocity = Mathf.Sqrt(-2 * gravity * heightOffset);

        // Combinar la velocidad horizontal y vertical para obtener la velocidad inicial completa
        Vector3 initialVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

        return initialVelocity;
    }

    // Detectar colisi�n con el jugador usando OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si la colisi�n es con el objeto que tiene la etiqueta "Player"
        if (other.CompareTag(targetTag)|| other.CompareTag("Ground"))
        {
            Explode();
        }
    }

    void Explode()
    {
        // Crear un �rea de da�o en la posici�n de la bomba
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, damageRadius);

        // Aplicar da�o a todos los objetos en el radio de explosi�n con la etiqueta del jugador
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

        // Mostrar el efecto visual de explosi�n (opcional)
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Destruir la bomba tras la explosi�n
        Destroy(gameObject);
    }

    // Visualizar el �rea de da�o en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}