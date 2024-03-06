using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ChargeEnemy : Enemy
{
    public float attackRange = 3f;
    public float detectionRange = 5f;
    public Transform player;
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public Collider backCollider;
    public float chargeSpeed = 10f;
    public float chargeDistance = 15f;
    //public Animator animator;


    private NavMeshAgent agent;
    private float timer;
    private Vector3 targetPosition;
    private bool charging = false;
    private bool attack = true;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        SetRandomDestination();
        // animator = GetComponent<Animator>();

       
        if (backCollider != null)
            backCollider.enabled = false;
    }

    void Update()
    {

        

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);


        if (distanceToPlayer <= detectionRange )
        {
            if (!charging )
            {
                charging = true;
                Invoke("ChargeAttack", 2f); 
            }
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
        //animator.SetBool("Walk", true);

    }
    void ChargeAttack()
    {
            agent.enabled = false;

            Vector3 chargeTargetPosition = player.position;
            Vector3 chargeDirection = (chargeTargetPosition - transform.position).normalized;
            Vector3 chargeDestination = transform.position + chargeDirection * chargeDistance;
            transform.LookAt(player);


            agent.enabled = true;
            agent.speed = chargeSpeed;
            agent.SetDestination(chargeDestination);
            backCollider.enabled = false;


            Invoke("WaitTime", 2f);
            // animator.SetBool("Attack", true);

        
    }

    void WaitTime()
    {
        charging = false;
        transform.LookAt(player);
        backCollider.enabled = true;

        // animator.SetBool("Attack", false);

    }

    public void DesactivarMovimientos()
    {
        enabled = false;
    }
        public void ReactivarMovimientos()
    {
        enabled = true;
    }


}