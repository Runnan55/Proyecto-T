using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemy : Enemy
{
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float detectionRange = 10f;
    public float minChaseRange = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float visionAngle = 60f;

    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private float timer;
    private float attackTimer;
    private GameObject player;
    private bool isChasingPlayer = false;
    private bool canAttack = true;

    // Enum para definir los estados del enemigo
    private enum EnemyState
    {
        Wander,
        Chase,
        Attack
    }

    private EnemyState currentState; // Estado actual del enemigo

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        attackTimer = 0f;
        SetRandomDestination();
        player = GameObject.FindGameObjectWithTag("Player");
        currentState = EnemyState.Wander; // Iniciar en el estado de deambular
    }

    void Update()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        switch (currentState)
        {
            case EnemyState.Wander:
                // Lógica para deambular
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0f)
                    {
                        SetRandomDestination();
                        timer = wanderTimer;
                    }
                }

                // Transición al estado de persecución si el jugador está en rango de detección
                if (distanceToPlayer <= detectionRange || distanceToPlayer <= minChaseRange)
                {
                    float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
                    if (angleToPlayer < visionAngle * 0.5f)
                    {
                        currentState = EnemyState.Chase;
                    }
                }
                break;

            case EnemyState.Chase:
                // Lógica para perseguir al jugador
                agent.ResetPath();
                if (distanceToPlayer <= attackRange + 1)
                {
                    currentState = EnemyState.Attack; // Transición al estado de ataque si el jugador está dentro del rango de ataque
                }
                else
                {
                    agent.SetDestination(player.transform.position);
                }
                break;

            case EnemyState.Attack:
                // Lógica para atacar al jugador
                if (Time.time >= attackTimer)
                {
                    AttackPlayer();
                    Debug.Log("-10");
                    attackTimer = Time.time + attackCooldown;
                }

                // Transición de regreso al estado de persecución si el jugador sale del rango de ataque
                if (distanceToPlayer > attackRange + 1)
                {
                    currentState = EnemyState.Chase;
                }
                break;
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        targetPosition = hit.position;

        agent.SetDestination(targetPosition);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canAttack)
        {
            AttackPlayer();
            Debug.Log("-10");
            canAttack = false;
            Invoke("ResetetAttack", 2f);
        }
    }

    private void ResetetAttack()
    {
        canAttack = true;
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
