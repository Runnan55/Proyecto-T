using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotProyectil : MonoBehaviour
{
    public Transform shootPoint; // Transform para la posici�n de disparo
    public float moveSpeed = 2f; // Velocidad del robot
    public GameObject projectilePrefab; // Prefab del proyectil
    public float shootCooldown = 1f; // Tiempo entre disparos
    private float shootTimer;

    private Transform player; // Referencia al jugador
    private Vector3 direction; // Direcci�n de movimiento del robot

    public bool moveLeft = true; // Si es true, el robot se mueve a la izquierda, si es false a la derecha

    public float rotationSpeed = 5f; // Velocidad de rotaci�n hacia el jugador

    void Start()
    {
        // Obtener la referencia al jugador por su tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = shootCooldown;

        // Definir la direcci�n de movimiento seg�n moveLeft
        direction = moveLeft ? Vector3.left : Vector3.right;
    }

    void Update()
    {
        // Mover el robot hacia la izquierda o derecha seg�n el valor de moveLeft
        Move();
        RotateTowardsPlayer();

        // Disparar proyectiles
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
        }

    }

    private void Move()
    {
        // Mover el robot en la direcci�n X (izquierda o derecha)
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    void RotateTowardsPlayer()
    {
        // Calcula la direcci�n hacia el jugador
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Ignora la componente Y para que no se incline

        // Calcula la rotaci�n deseada para mirar al jugador
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // Aplica la rotaci�n de manera suave
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void Shoot()
    {
        if (player != null && shootPoint != null)
        {
            // Crear el proyectil en la posici�n de disparo (shootPoint)
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            // Calcular la direcci�n del proyectil hacia el jugador
            Vector3 direction = (player.position - shootPoint.position).normalized;
            projectile.GetComponent<FlechaBot>().SetDirection(direction); // Pasamos la direcci�n al proyectil
        }
    }

    // Destruir al colisionar con un trigger de pared
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Walls"))
        {
            Destroy(gameObject); // Destruir el robot si colisiona con los l�mites
        }
    }
}