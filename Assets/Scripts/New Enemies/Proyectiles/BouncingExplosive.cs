using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingExplosive : MonoBehaviour
{
    public float explosionRadius = 5f; // Radio del área de daño
    public float explosionDamage = 10f; // Daño que causará la explosión
    public float lifetime = 3f; // Tiempo antes de destruirse (3 segundos)
    public float blinkInterval = 0.1f; // Intervalo entre parpadeos
    public LayerMask playerLayer; // Capa de objetos que pueden recibir daño
    public float speed = 5f; // Velocidad del proyectil ajustable desde el Inspector

    public GameObject explosionPrefab;

    private Renderer rend;
    private Vector3 velocity;
    private bool isDestroyed = false;

    void Start()
    {
        // Obtener el componente Renderer para el parpadeo
        rend = GetComponent<Renderer>();

        // Dirección inicial del proyectil
        velocity = transform.forward;
        velocity.y = 0; // Mantener movimiento en el plano XZ si es necesario
        velocity = velocity.normalized * speed; // Usar la velocidad ajustable

        // Iniciar la corutina para destruir el proyectil después de 3 segundos
        StartCoroutine(DestructionTimer());
    }

    void Update()
    {
        // Si el proyectil está marcado para destruirse y ha pasado el tiempo, no hacer más nada
        if (isDestroyed)
            return;

        // Mover el proyectil continuamente
        transform.position += velocity * Time.deltaTime;
    }

    private IEnumerator DestructionTimer()
    {
        // Esperar hasta el último segundo antes de hacer el parpadeo
        yield return new WaitForSeconds(lifetime - 1f); // Esperar el tiempo restante para parpadeo

        // Parpadeo antes de la destrucción
        float elapsedTime = 0f;
        while (elapsedTime < 1f) // Parpadeo durante el último segundo
        {
            // Alternar la visibilidad del proyectil
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        // Crear el área de daño antes de la destrucción
        CreateExplosion();

        // Destruir el objeto
        Destroy(gameObject);
    }

    private void CreateExplosion()
    {
        // Crear una esfera que represente el área de daño
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, explosionRadius, playerLayer);
        foreach (Collider player in hitPlayers)
        {
            if (player.TryGetComponent(out Life playerHealth)) // Asegúrate de tener un script Life (salud del jugador)
            {
                playerHealth.ModifyTime(-explosionDamage); // Aquí asumo que ModifyTime es la función que reduce vida
            }
        }

        if (explosionPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(explosionInstance, 2f);
        }


        Debug.Log("Ataque en área realizado!");

        // Aquí puedes agregar efectos visuales de la explosión si es necesario (por ejemplo, una explosión de partículas).
        // Instanciar efectos de partículas o sonidos según lo que prefieras.
    }

  

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Walls")) // Rebote contra una pared
        {
            Vector3 normal = other.transform.forward;

            // Agregar una ligera desviación al ángulo de la normal para obtener un rebote con ángulo
            Vector3 reflectDir = Vector3.Reflect(velocity, normal);

            // Aplicar un pequeño ángulo aleatorio para desviar ligeramente el rebote
            float angleVariance = Random.Range(-15f, 15f);  // Desviación entre -15 y 15 grados
            reflectDir = Quaternion.Euler(0, angleVariance, 0) * reflectDir;

            // Asegurarnos de que la dirección del movimiento sea normalizada y multiplicada por la velocidad
            velocity = reflectDir.normalized * speed;

        }
        else if (other.CompareTag("Player")) // Colisión con el jugador
        {
            // Crear la explosión en el momento de la colisión
            CreateExplosion();

            // Destruir el proyectil al colisionar con el jugador
            Destroy(gameObject);
        }
    }
}