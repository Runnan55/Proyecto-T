using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Charge : Enemy
{
    private Transform player;
    public Collider attackCollider;
    public float detectionRadius = 10f;
    public float attackDistance = 2f;
    public float waitTime = 2f;
    public float chargeSpeed = 10f;
    public float normalSpeed = 3.5f;
    public float chargeDistance = 5f;
    public float chargeDuration = 1f;
    public Collider dano;

    private NavMeshAgent agent;
    private bool isAttacking = false;
    private bool isWaiting = false;
    private Animator animator;

    void Awake()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
        animator = GetComponent<Animator>();
        dano.enabled = false;
        attackCollider.enabled = false;
    }

    void Update()
    {
        if (player == null) return; // Salir si no hay jugador

        float distanceToPlayerSqr = (player.position - transform.position).sqrMagnitude;
        float attackDistanceSqr = attackDistance * attackDistance;
        float detectionRadiusSqr = detectionRadius * detectionRadius;

        // Mantener al enemigo mirando al jugador si está en el rango de detección
        if (distanceToPlayerSqr <= detectionRadiusSqr && !isAttacking)
        {
            LookAtPlayer(); // Girar hacia el jugador en cada frame, excepto cuando está atacando
        }

        // Lógica para manejar las colisiones de ataque solo si el jugador está en rango y el enemigo no está atacando
        attackCollider.enabled = isWaiting && !IsPlayerBehindEnemy() && !isAttacking;

        if (distanceToPlayerSqr <= detectionRadiusSqr && !isAttacking && !isWaiting)
        {
            if (distanceToPlayerSqr > attackDistanceSqr)
            {
                if (!agent.hasPath || agent.destination != player.position)
                {
                    agent.SetDestination(player.position);
                    animator.SetBool("Walk", true);
                }
            }
            else
            {
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
        agent.isStopped = true;
        yield return new WaitForSeconds(waitTime);

        isWaiting = false;
        StartCoroutine(ChargeAttack());
    }

    private IEnumerator ChargeAttack()
    {
        isAttacking = true;
        agent.isStopped = false;
        dano.enabled = true;
        agent.speed = chargeSpeed;
        animator.SetBool("Walk", true);

        Vector3 chargeDirection = (player.position - transform.position).normalized;
        Vector3 chargeTarget = transform.position + chargeDirection * chargeDistance;

        // Configurar destino de carga
        agent.SetDestination(chargeTarget);

        yield return new WaitForSeconds(chargeDuration);

        agent.speed = normalSpeed;
        agent.SetDestination(transform.position); // Regresar a posición original
        dano.enabled = false;
        animator.SetBool("Walk", false);

        // Esperar 0.5 segundos después de la carga antes de mirar al jugador
        yield return new WaitForSeconds(0.5f);

        // Activar la mirada hacia el jugador
        StartCoroutine(LookAtPlayerForDuration(1f)); // Suavizar el giro al jugador por 1 segundo después de esperar
        isAttacking = false;
    }

    /// <summary>
    /// Mantiene al enemigo mirando hacia el jugador en todo momento.
    /// </summary>
    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0; // Mantener solo la rotación en el plano horizontal
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Suavizar la rotación
    }

    /// <summary>
    /// Mantiene al enemigo mirando hacia el jugador por una duración específica.
    /// </summary>
    private IEnumerator LookAtPlayerForDuration(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (player != null)
            {
                Vector3 lookDirection = (player.position - transform.position).normalized;
                lookDirection.y = 0; // Mantener solo la rotación en el plano horizontal

                // Suavizar la rotación hacia el jugador
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Esperar al siguiente frame
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    private bool IsPlayerBehindEnemy()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 forwardDirection = transform.forward;
        forwardDirection.y = 0;
        directionToPlayer.y = 0;

        return Vector3.Dot(forwardDirection.normalized, directionToPlayer.normalized) > 0.5f;
    }

    public void DesactivarMovimientos()
    {
        animator.applyRootMotion = false;
        attackCollider.enabled = true;
        agent.enabled = false; // Desactiva el NavMeshAgent
    }

    public void ReactivarMovimientos()
    {
        attackCollider.enabled = false;
        agent.enabled = true; // Reactiva el NavMeshAgent
        animator.applyRootMotion = true;
    }
}
