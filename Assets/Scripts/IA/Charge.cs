using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Charge : Enemy
{
    public Transform player;
    public Collider attackCollider; // Collider de ataque
    public float detectionRadius = 10f;
    public float attackDistance = 2f;
    public float waitTime = 2f;
    public float chargeSpeed = 10f;
    public float normalSpeed = 3.5f;
    public float chargeDistance = 5f; // Distancia que se embestir�
    public float chargeDuration = 1f; // Duraci�n de la embestida

    private NavMeshAgent agent;
    private bool isAttacking = false;
    private bool isWaiting = false;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
        animator = GetComponent<Animator>(); // Obtener el Animator

        // Desactivar el collider de ataque al inicio
        attackCollider.enabled = false;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si el enemigo est� en espera, el jugador est� detr�s del enemigo y el enemigo no est� atacando
        if (isWaiting && !IsPlayerBehindEnemy() && !isAttacking)
        {
            // Activa el collider de ataque
            attackCollider.enabled = true;
        }
        else
        {
            // Desactiva el collider de ataque
            attackCollider.enabled = false;
        }

        // Si el jugador est� dentro del radio de detecci�n y el enemigo no est� atacando ni esperando
        if (distanceToPlayer <= detectionRadius && !isAttacking && !isWaiting)
        {
            if (distanceToPlayer > attackDistance)
            {
                // Moverse hacia el jugador
                agent.SetDestination(player.position);
                animator.SetBool("Walk", true);
            }
            else
            {
                // Comienza la espera antes de atacar
                StartCoroutine(WaitAndAttack());
            }
        }
        else
        {
            animator.SetBool("Walk", false);

        }
    }

    private IEnumerator WaitAndAttack()
    {
        isWaiting = true;
        agent.isStopped = true; // Detener al enemigo
        Debug.Log("Enemy waiting...");
        yield return new WaitForSeconds(waitTime);

        isWaiting = false;
        StartCoroutine(ChargeAttack());
    }

    private IEnumerator ChargeAttack()
    {
        isAttacking = true;
        agent.isStopped = false;
        agent.speed = chargeSpeed;
        animator.SetBool("Walk", true);
        Debug.Log("Enemy charging...");
        Vector3 chargeDirection = (player.position - transform.position).normalized;
        Vector3 chargeTarget = transform.position + chargeDirection * chargeDistance;

        // Movimiento de embestida
        agent.SetDestination(chargeTarget);

        yield return new WaitForSeconds(chargeDuration);

        // Vuelve a la velocidad normal
        agent.speed = normalSpeed;
        agent.SetDestination(transform.position); // Detener el movimiento

        // Mirar al jugador despu�s de la carga
        transform.LookAt(player.position);
        Debug.Log("Enemy charge completed and looking at player.");

        isAttacking = false;
        animator.SetBool("Walk", false);

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    // M�todo para verificar si el jugador est� detr�s del enemigo
    private bool IsPlayerBehindEnemy()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 forwardDirection = transform.forward;
        forwardDirection.y = 0; // Ignorar la componente Y para evitar problemas en terrenos inclinados
        directionToPlayer.y = 0; // Ignorar la componente Y para evitar problemas en terrenos inclinados

        // Calcula el �ngulo entre los dos vectores
        float angleToPlayer = Vector3.Dot(forwardDirection.normalized, directionToPlayer.normalized);

        // Si el �ngulo es mayor que 0.5, el jugador est� detr�s del enemigo
        return angleToPlayer > 0.5f;
    }

    public void DesactivarMovimientos()
    {

        agent.enabled = false; // Desactiva el NavMeshAgent
    }

    public void ReactivarMovimientos()
    {

        agent.enabled = true; // Desactiva el NavMeshAgent

    }

}