using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingExplosive : MonoBehaviour
{
    public float explosionRadius = 5f; // Radio del �rea de da�o
    public float explosionDamage = 10f; // Da�o que causar� la explosi�n
    public float lifetime = 3f; // Tiempo antes de destruirse (3 segundos)
    public float blinkInterval = 0.1f; // Intervalo entre parpadeos
    public LayerMask playerLayer; // Capa de objetos que pueden recibir da�o
    public float speed = 5f; // Velocidad del proyectil ajustable desde el Inspector

    public GameObject explosionPrefab;

    private Renderer rend;
    private Vector3 velocity;
    private bool isDestroyed = false;

    void Start()
    {
        // Obtener el componente Renderer para el parpadeo
        rend = GetComponent<Renderer>();

        // Direcci�n inicial del proyectil
        velocity = transform.forward;
        velocity.y = 0; // Mantener movimiento en el plano XZ si es necesario
        velocity = velocity.normalized * speed; // Usar la velocidad ajustable

        // Iniciar la corutina para destruir el proyectil despu�s de 3 segundos
        StartCoroutine(DestructionTimer());
    }

    void Update()
    {
        // Si el proyectil est� marcado para destruirse y ha pasado el tiempo, no hacer m�s nada
        if (isDestroyed)
            return;

        // Mover el proyectil continuamente
        transform.position += velocity * Time.deltaTime;
    }

    private IEnumerator DestructionTimer()
    {
        // Esperar hasta el �ltimo segundo antes de hacer el parpadeo
        yield return new WaitForSeconds(lifetime - 1f); // Esperar el tiempo restante para parpadeo

        // Parpadeo antes de la destrucci�n
        float elapsedTime = 0f;
        while (elapsedTime < 1f) // Parpadeo durante el �ltimo segundo
        {
            // Alternar la visibilidad del proyectil
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        // Crear el �rea de da�o antes de la destrucci�n
        CreateExplosion();

        // Destruir el objeto
        Destroy(gameObject);
    }

    private void CreateExplosion()
    {
        // Crear una esfera que represente el �rea de da�o
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, explosionRadius, playerLayer);
        foreach (Collider player in hitPlayers)
        {
            if (player.TryGetComponent(out Life playerHealth)) // Aseg�rate de tener un script Life (salud del jugador)
            {
                playerHealth.ModifyTime(-explosionDamage); // Aqu� asumo que ModifyTime es la funci�n que reduce vida
            }
        }

        if (explosionPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(explosionInstance, 2f);
        }


        Debug.Log("Ataque en �rea realizado!");

        // Aqu� puedes agregar efectos visuales de la explosi�n si es necesario (por ejemplo, una explosi�n de part�culas).
        // Instanciar efectos de part�culas o sonidos seg�n lo que prefieras.
    }

  

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Walls")) // Rebote contra una pared
        {
            Vector3 normal = other.transform.forward;

            // Agregar una ligera desviaci�n al �ngulo de la normal para obtener un rebote con �ngulo
            Vector3 reflectDir = Vector3.Reflect(velocity, normal);

            // Aplicar un peque�o �ngulo aleatorio para desviar ligeramente el rebote
            float angleVariance = Random.Range(-15f, 15f);  // Desviaci�n entre -15 y 15 grados
            reflectDir = Quaternion.Euler(0, angleVariance, 0) * reflectDir;

            // Asegurarnos de que la direcci�n del movimiento sea normalizada y multiplicada por la velocidad
            velocity = reflectDir.normalized * speed;

        }
        else if (other.CompareTag("Player")) // Colisi�n con el jugador
        {
            // Crear la explosi�n en el momento de la colisi�n
            CreateExplosion();

            // Destruir el proyectil al colisionar con el jugador
            Destroy(gameObject);
        }
    }
}