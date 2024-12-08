using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotProyectil : MonoBehaviour
{
    public Transform shootPoint; // Transform para la posición de disparo
    public float moveSpeed = 2f; // Velocidad del robot
    public GameObject projectilePrefab; // Prefab del proyectil
    public float shootCooldown = 1f; // Tiempo entre disparos
    private float shootTimer;

    private Transform player; // Referencia al jugador
    private Vector3 direction; // Dirección de movimiento del robot

    public bool moveLeft = true; // Si es true, el robot se mueve a la izquierda, si es false a la derecha

    public float rotationSpeed = 5f; // Velocidad de rotación hacia el jugador

    void Start()
    {
        // Obtener la referencia al jugador por su tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = shootCooldown;

        // Definir la dirección de movimiento según moveLeft
        direction = moveLeft ? Vector3.left : Vector3.right;
    }

    void Update()
    {
        // Mover el robot hacia la izquierda o derecha según el valor de moveLeft
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
        // Mover el robot en la dirección X (izquierda o derecha)
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    void RotateTowardsPlayer()
    {
        // Calcula la dirección hacia el jugador
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Ignora la componente Y para que no se incline

        // Calcula la rotación deseada para mirar al jugador
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // Aplica la rotación de manera suave
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void Shoot()
    {
        if (player != null && shootPoint != null)
        {
            // Crear el proyectil en la posición de disparo (shootPoint)
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            // Calcular la dirección del proyectil hacia el jugador
            Vector3 direction = (player.position - shootPoint.position).normalized;
            projectile.GetComponent<FlechaBot>().SetDirection(direction); // Pasamos la dirección al proyectil
        }
    }

    // Destruir al colisionar con un trigger de pared
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Walls"))
        {
            Destroy(gameObject); // Destruir el robot si colisiona con los límites
        }
    }
}