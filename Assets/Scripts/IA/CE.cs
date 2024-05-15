using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CE : Enemy
{
    public float attackRange = 3f;
    public float detectionRange = 5f;
    private Transform player;
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public Collider backCollider;
    public float chargeSpeed = 10f;
    public float chargeDistance = 15f;
    public GameObject damageEffect;


    private NavMeshAgent agent;
    private float timer;
    private Vector3 targetPosition;
    private bool charging = false;
    private Animator animator;
    private Rigidbody rb;
    private bool empujar = true;

    private enum EnemyState
    {
        Wander,
        Detect,
        Charge
    }

    private EnemyState currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        SetRandomDestination();
        if (backCollider != null)
            backCollider.enabled = false;

        // Initial state
        currentState = EnemyState.Wander;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Wander:
                UpdateWanderState();
                animator.SetBool("Walk", true);

                break;
            case EnemyState.Detect:
                animator.SetBool("Walk", false);
                UpdateDetectState();
                break;
            case EnemyState.Charge:
                animator.SetBool("Walk", true);

                UpdateChargeState();
                break;
        }
    }

    void UpdateWanderState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Detect;
            return;
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

    void UpdateDetectState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange)
        {
            currentState = EnemyState.Wander;
            return;
        }

        if (!charging)
        {

            charging = true;
            Invoke("ChargeAttack", 2f);
        }
    }

    void UpdateChargeState()
    {
        // If already charging, no need to do anything in Update()
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
        animator.SetBool("Walk", true);

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
    }

    public override void ReceiveDamage(float amount)
    {
        base.ReceiveDamage(amount);

        if (damageEffect != null)
        {
            damageEffect.SetActive(true);
            CancelInvoke();

            Invoke("DisableDamageEffect", 2f);
        }

        if (empujar)
        {
            Empuje();
        }

    }
    public void Empuje()
    {
        Rigidbody enemyRigidbody = GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            // Cambia el valor de la fuerza seg�n lo necesites
            float force = 100f;
            enemyRigidbody.AddForce(-transform.forward * force, ForceMode.Impulse);
        }
    }
    void WaitTime()
    {
        animator.SetBool("Walk", false);

        charging = false;
        transform.LookAt(player);
        Vector3 directionToPlayer = player.position - transform.position;
        Ray ray = new Ray(transform.position, directionToPlayer);
        RaycastHit hit;

        // Verificar si el raycast detecta al jugador
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == player)
            {
                
                backCollider.enabled = false; // Si ve al jugador, el backCollider está desactivado
            }
            else
            {
                backCollider.enabled = true; // Si ve al jugador, el backCollider está desactivado

            }
        }
    }

    public void DesactivarMovimientos()
    {
        empujar = false;
        animator.applyRootMotion = false;
        animator.enabled = false;

        agent.enabled = false; // Desactiva el NavMeshAgent
        CancelInvoke();
    }

    public void ReactivarMovimientos()
    {
        animator.applyRootMotion = true;
        animator.enabled = true;
        empujar = true;
        charging = false;
    }
}