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

    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private float timer;
    private float attackTimer;
    private GameObject player;
    private bool isChasingPlayer = false;
    private bool isAttacking = false; // Nueva variable para controlar el estado de ataque
    private Animator animator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        attackTimer = 0f;
        SetRandomDestination();
        animator = GetComponent<Animator>();
        StartCoroutine(InitializePlayer());
    }

    private IEnumerator InitializePlayer()
    {
        yield return new WaitForSeconds(0.25f);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Si está atacando, no debe moverse ni caminar
        if (isAttacking)
        {
            agent.ResetPath();
            return;
        }

        // Cambiar a estado de caminata si no está atacando
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
            if (distanceToPlayer <= attackRange)
            {
                if (Time.time >= attackTimer)
                {
                    attackTimer = Time.time + attackCooldown;
                    StartAttack(); // Iniciar ataque
                }
            }
            else
            {
                agent.SetDestination(player.transform.position);
                animator.SetBool("Walk", true);
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
        CancelAttack(); // Cancelar el ataque si recibe daño
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isAttacking)  // Solo si está en la animación de ataque
        {
            AttackPlayer();
        }
    }

    // Método para iniciar el ataque
    private void StartAttack()
    {
        isAttacking = true; // El zombi está atacando
        agent.ResetPath(); // Detener el movimiento
        animator.SetBool("Walk", false); // Detener la animación de caminar
        animator.SetTrigger("Attack"); // Iniciar la animación de ataque
    }

    // Método llamado desde la animación para aplicar el daño
    public void ApplyDamage()
    {
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            AttackPlayer();
        }
    }

    // Método para cancelar el ataque
    public void CancelAttack()
    {
        isAttacking = false; // El zombi ya no está atacando
        animator.ResetTrigger("Attack"); // Resetear el trigger de ataque
        animator.SetBool("Walk", true); // Retomar la animación de caminar
    }

    // Método para llamar desde el Animator cuando termina la animación de ataque
    public void OnAttackAnimationEnd()
    {
        CancelAttack(); // Indicar que terminó el ataque
    }

    public void ActiveNavMesh()
    {
        agent.enabled = true;
        animator.applyRootMotion = true;
    }

    public void DesactiveNavMesh()
    {
        animator.applyRootMotion = false;
        agent.enabled = false;
    }
}