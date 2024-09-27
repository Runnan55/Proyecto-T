using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinasEnemy : Enemy
{
    private NavMeshAgent agent;           // El NavMeshAgent que maneja el movimiento del enemigo
    private Transform player;             // Transform del jugador para seguimiento
    public float mineThrowDistance = 10f; // Distancia a la que lanza minas
    public GameObject minePrefab;        // Prefab de la mina
    public float timeBetweenMines = 5f;  // Tiempo entre lanzamientos de minas
    public float safeDistance = 5f;     // Distancia segura para alejarse del jugador

    private float mineCooldown;          // Enfriamiento entre minas

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mineCooldown = timeBetweenMines;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        // Distancia actual entre el enemigo y el jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si el jugador está a menos de la distancia segura, alejarse
        if (distanceToPlayer < mineThrowDistance)
        {
            MoveAwayFromPlayer();
        }
        // Si el jugador está fuera del rango de lanzamiento de minas, acercarse
        else if (distanceToPlayer > mineThrowDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // Detenerse en la posición actual cuando está en el rango de lanzamiento
            agent.SetDestination(transform.position);
        }

        // Si está a una distancia adecuada para lanzar minas y el cooldown se ha completado
        if (distanceToPlayer <= mineThrowDistance && mineCooldown <= 0f)
        {
            LaunchMine();
            mineCooldown = timeBetweenMines;
        }
        else
        {
            // Reducir el enfriamiento
            mineCooldown -= Time.deltaTime;
        }
    }

    // Moverse a una posición opuesta al jugador para mantener la distancia segura
    void MoveAwayFromPlayer()
    {
        Vector3 directionToPlayer = transform.position - player.position;
        Vector3 newPosition = transform.position + directionToPlayer.normalized * safeDistance;
        agent.SetDestination(newPosition);
    }

    // Moverse hacia el jugador hasta estar a la distancia adecuada para lanzar minas
    void MoveTowardsPlayer()
    {
        // Mover directamente hacia la posición del jugador
        agent.SetDestination(player.position);
    }

    // Lanza una mina al jugador o a una posición cercana a él
    void LaunchMine()
    {
        Vector3 mineSpawnPosition = player.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        Instantiate(minePrefab, mineSpawnPosition, Quaternion.identity);
    }
}