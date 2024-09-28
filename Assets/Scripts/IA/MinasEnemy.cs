using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinasEnemy : Enemy
{
    private NavMeshAgent agent;          // Control del movimiento del enemigo
    private Transform player;            // Transform del jugador para seguimiento
    public float mineThrowDistance = 10f; // Distancia a la que lanza minas
    public GameObject bombPrefab;        // Prefab de la mina
    public float timeBetweenMines = 5f;  // Tiempo entre lanzamientos de minas
    public float safeDistance = 5f;      // Distancia segura para alejarse del jugador
    private float mineCooldown;          // Enfriamiento entre minas
    public Transform ShootBombs;         // Posición desde donde se disparan las minas

    private float mineThrowDistanceSqr;  // Cuadrado de la distancia de lanzamiento para optimizar cálculos
    private float safeDistanceSqr;       // Cuadrado de la distancia segura

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mineCooldown = timeBetweenMines;

        // Cachear las distancias al cuadrado para evitar cálculos repetidos
        mineThrowDistanceSqr = mineThrowDistance * mineThrowDistance;
        safeDistanceSqr = safeDistance * safeDistance;

        // Encontrar el jugador por su etiqueta
        StartCoroutine(InitializePlayerComponents());

        StartCoroutine(InitializePlayerTransform());
    }
    public IEnumerator InitializePlayerTransform()
    {
        yield return new WaitForSeconds(0.25f);
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }
    void Update()
    {
        if (player == null) return;

        float distanceToPlayerSqr = (transform.position - player.position).sqrMagnitude;

        if (distanceToPlayerSqr < safeDistanceSqr)
        {
            MoveAwayFromPlayer();
        }
        else if (distanceToPlayerSqr > mineThrowDistanceSqr)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopAgent();
        }

        // Control de lanzamiento de minas
        HandleMineLaunch(distanceToPlayerSqr);
    }

    // Moverse a una posición opuesta al jugador para mantener la distancia segura
    void MoveAwayFromPlayer()
    {
        Vector3 directionToPlayer = transform.position - player.position;
        Vector3 newPosition = transform.position + directionToPlayer.normalized * safeDistance;
        if (agent.destination != newPosition)
        {
            agent.SetDestination(newPosition);
        }
    }

    // Moverse hacia el jugador hasta estar a la distancia adecuada para lanzar minas
    void MoveTowardsPlayer()
    {
        if (agent.destination != player.position)
        {
            agent.SetDestination(player.position);
        }
    }

    // Detener al agente cuando está en rango de lanzamiento
    void StopAgent()
    {
        if (agent.hasPath)
        {
            agent.ResetPath(); // Detener el movimiento
        }
    }

    // Manejar el lanzamiento de minas
    void HandleMineLaunch(float distanceToPlayerSqr)
    {
        if (distanceToPlayerSqr <= mineThrowDistanceSqr && mineCooldown <= 0f)
        {
            LaunchMine();
            mineCooldown = timeBetweenMines; // Reiniciar el cooldown
        }
        else
        {
            // Reducir el enfriamiento
            mineCooldown -= Time.deltaTime;
        }
    }

    // Lanza una mina en la posición del jugador
    void LaunchMine()
    {
        Instantiate(bombPrefab, ShootBombs.position, ShootBombs.rotation);
    }
}