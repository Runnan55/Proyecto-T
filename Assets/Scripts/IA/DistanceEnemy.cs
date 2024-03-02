using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class DistanceEnemy : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float detectionRange = 10f;
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    public float shotDistance = 5f;
    public GameObject arrowPrefab; 
    public Transform arrowSpawnPoint; 

    private NavMeshAgent agent;
    private GameObject player;
    private float timer;
    private float attackTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        attackTimer = 0f;
        player = GameObject.FindGameObjectWithTag("Player");
        SetRandomDestination();
    }

    void Update()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0;
        float distanceToPlayer = directionToPlayer.magnitude;


        if (distanceToPlayer <= detectionRange || distanceToPlayer <= attackRange)
        {

            if (distanceToPlayer <= attackRange)
            {

                if (Time.time >= attackTimer)
                {
                    AttackPlayer(directionToPlayer);
                    transform.rotation = Quaternion.LookRotation(directionToPlayer);
                    attackTimer = Time.time + attackCooldown;
                }
            }

            MoveToPlayer(directionToPlayer);
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SetRandomDestination();
                timer = wanderTimer;
            }
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        agent.SetDestination(hit.position);
    }

    void MoveToPlayer(Vector3 directionToPlayer)
    {
        Vector3 desiredPosition = player.transform.position - directionToPlayer.normalized * shotDistance;
        agent.SetDestination(desiredPosition);
    }

    void AttackPlayer(Vector3 directionToPlayer)
    {
        Debug.Log("Ataque");
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        arrow.transform.rotation = Quaternion.LookRotation(directionToPlayer);
    }
}