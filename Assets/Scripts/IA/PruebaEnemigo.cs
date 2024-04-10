using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PruebaEnemigo : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        attackTimer = 0f;
        SetRandomDestination();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
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
