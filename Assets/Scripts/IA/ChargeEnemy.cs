using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ChargeEnemy : MonoBehaviour
{
    public float attackRange = 3f; 
    public float detectionRange = 5f; 
    public Transform player; 
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float viewAngle = 60f; 
    public Collider frontCollider; 
    public Collider backCollider;
    public float chargeSpeed = 10f;

    private NavMeshAgent agent;
    private float timer;
    private Vector3 targetPosition;
    private bool charging = false;


    bool playerDetected = false; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        SetRandomDestination();

       
        if (frontCollider != null)
            frontCollider.enabled = true;

 
        if (backCollider != null)
            backCollider.enabled = false;
    }

    void Update()
    {
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        
        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (distanceToPlayer <= detectionRange && angleToPlayer <= viewAngle / 2f)
        {
            playerDetected = true;
           
            if (backCollider != null)
                backCollider.enabled = false;
            
            if (frontCollider != null)
                frontCollider.enabled = true;
            Debug.Log("collider activo");

            if (!charging)
            {
                charging = true;
                Invoke("ChargeAttack", 2f); // Espera 2 segundos antes de cargar el ataque
            }
        }
        else
        {
            playerDetected = false;

            if (backCollider != null)
                backCollider.enabled = true;

            if (frontCollider != null)
                frontCollider.enabled = false;
            Debug.Log("collider desactivo");

        }

        if (distanceToPlayer <= attackRange && !playerDetected)
        {
            Debug.Log("La meti por detras");
        }

        if (!agent.pathPending && agent.remainingDistance < 0.1f)
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
        targetPosition = hit.position;
        agent.SetDestination(targetPosition);
    }
    void ChargeAttack()
    {
        agent.enabled = false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        Vector3 chargeTargetPosition = player.position;

        Vector3 chargeDirection = (chargeTargetPosition - transform.position).normalized;
        Vector3 chargeDestination = transform.position + chargeDirection * 10f; // Cambia 10f por la distancia que quieras que el enemigo avance

        agent.enabled = true;
        agent.speed = chargeSpeed;
        agent.SetDestination(chargeDestination);

        charging = false;
    }

    
}