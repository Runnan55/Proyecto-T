using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiEnemy : Enemy
{
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float detectionRange = 10f;
    public float minChaseRange = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float visionAngle = 60f;
    public GameObject damageEffect;

    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private float timer;
    private float attackTimer;
    private GameObject player;
    private bool isChasingPlayer = false;
    private bool canAttack = true;
    private Animator animator;
    private bool empujar = true;



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        attackTimer = 0f;
        SetRandomDestination();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();  

    }

    void Update()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        animator.SetBool("Walk", true);


        if (!isChasingPlayer && (distanceToPlayer <= detectionRange || distanceToPlayer <= minChaseRange))
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < visionAngle * 0.5f)
            {
                isChasingPlayer = true;
                
            }
        }

        if (isChasingPlayer)
        {
            agent.ResetPath();
            if (distanceToPlayer <= attackRange + 1)
            {
                if (Time.time >= attackTimer)
                {
                    attackTimer = Time.time + attackCooldown;
                    animator.SetBool("Walk", false);

                    animator.SetBool("Attack", true);
                }
            }
            else
            {
                agent.SetDestination(player.transform.position);
                animator.SetBool("Walk", true);
                animator.SetBool("Attack", false);
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance < 0.1f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    SetRandomDestination();
                    timer = wanderTimer;
                    animator.SetBool("Walk", true);
                }
                else
                {
                    animator.SetBool("Walk", false);
                }
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

    public override void ReceiveDamage(float amount)
    {
        base.ReceiveDamage(amount);

        if (animator.GetBool("Attack"))
        {
            animator.SetBool("Attack", false);
            canAttack = false;
            Invoke("ResetAttack", attackCooldown);  // Espera el tiempo de cooldown para poder atacar de nuevo
            Debug.Log("Ataque cancelado");

        }
        if (damageEffect != null)
        {
            damageEffect.SetActive(true);

            Invoke("DisableDamageEffect", 2f);
        }

        if (empujar)
        {
            Empuje();
        }
    }
    private void DisableDamageEffect()
    {
        if (damageEffect != null)
        {
            damageEffect.SetActive(false);

        }
    }
    private void Empuje()
    {
        Rigidbody enemyRigidbody = GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            // Cambia el valor de la fuerza según lo necesites
            float force = 100f;
            enemyRigidbody.AddForce(-transform.forward * force, ForceMode.Impulse);
        }
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
        animator.SetBool("Attack", false);
    }
    public void ActiveNavMesh()
    {

        agent.enabled = true;

        animator.enabled = true;
        animator.applyRootMotion = true;

        empujar = true;
    }

    public void DesactiveNavMesh()
    {
        empujar = false;

        animator.applyRootMotion = false;
        animator.enabled = false;
        agent.enabled = false;
    }
}