using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class DistanceEnemy : Enemy
{
    public float detectionRange = 10f;
    public float attackRange = 10f;
    public float optimalDistance = 5f; // Distancia ideal entre el enemigo y el jugador para disparar
    public float moveBackDistance = 3f; // Distancia a la que el enemigo se aleja antes de volver a disparar
    public int shotsBeforeMove = 3;
    public float attackCooldown = 2f;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;

    private NavMeshAgent agent;
    private GameObject player;
    private float attackTimer;
    private int shotsFired = 0;
    private Vector3 directionToPlayer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                if (Time.time >= attackTimer)
                {
                    AttackPlayer(directionToPlayer);
                    transform.rotation = Quaternion.LookRotation(directionToPlayer);
                    attackTimer = Time.time + attackCooldown;
                    shotsFired++;
                    if (shotsFired >= shotsBeforeMove)
                    {
                        shotsFired = 0;
                        MoveAwayFromPlayer();
                    }
                }
            }
            else
            {
                MoveToPlayer();
            }
        }
    }

    void MoveToPlayer()
    {
        // Calcula la posición ideal para mantener la distancia óptima del jugador
        Vector3 desiredPosition = player.transform.position - directionToPlayer.normalized * optimalDistance;
        agent.SetDestination(desiredPosition);
    }

    void MoveAwayFromPlayer()
    {
        // Calcula una posición detrás del enemigo para alejarse del jugador antes de volver a disparar
        Vector3 moveBackPosition = transform.position - transform.forward * moveBackDistance;
        agent.SetDestination(moveBackPosition);
    }

    void AttackPlayer(Vector3 directionToPlayer)
    {
        Debug.Log("Ataque");
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        arrow.transform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

    public void ActiveNavMesh()
    {
        agent.enabled = true;
    }

    public void DesactiveNavMesh()
    {
        agent.enabled = false;
    }
}